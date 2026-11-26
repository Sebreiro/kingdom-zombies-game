'use strict';

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');
const network = require('../../tools/lan-checks/network.cjs');
const terminal = require('../../tools/lan-checks/terminal.cjs');
const {environment} = require('../../tools/lan-checks/check.cjs');
const {lines} = require('../../tools/lan-checks/rendered.cjs');

const key = {escape: 4194305, enter: 4194309, menu: 4194370, left: 65, right: 68, interact: 69,
  inventory: 73, jump: 32, food1: 49, shift: 4194325};
const stash = {TargetKind: 'Stash', TargetId: 'foundation.stash'};
const current = peer => lines(path.join(peer.directory, 'frames.ndjson')).at(-1);
const actor = (row, id = row.phase06.control) => row.phase06.World.Actors.find(entry => entry.Id === id);
const own = row => row.phase06.Views.own;
const nearby = row => row.phase06.Views.nearby?.Rows ?? [];
const external = row => row.phase06.Views.external;
const item = (row, scope, definition) => row.phase06.Views[scope]?.Rows.find(entry => entry.DefinitionId === definition);
const reference = ({Id, Stamp}) => ({Id, Stamp});
function motionInterval(first, last, movement) {
  const elapsed = last.phase06.World.Elapsed - first.phase06.World.Elapsed;
  const distance = actor(last)?.X - actor(first)?.X;
  if (actor(first)?.Movement !== movement || actor(last)?.Movement !== movement || elapsed < 48 || distance <= 0)
    return null;
  const spent = first.hud.Stamina - last.hud.Stamina;
  return {elapsed, distance, speed: distance / elapsed, spent};
}
const wait = (state, test, label, ms = 15_000) => network.waitFor(state.peers.filter(peer => !peer.quitRequested),
  Date.now() + ms, test, label);
const activate = (state, peer, control) => network.activate(peer, state, Date.now() + 5000, control);
const inject = (state, peer, request) => network.input(peer, state, Date.now() + 5000, request);
const hold = (state, peer, code, pressed) => network.key(peer, state, Date.now() + 5000, code, pressed);

async function tap(state, peer, code) {
  await hold(state, peer, code, true);
  await hold(state, peer, code, false);
}

function observe(state, peer, predicate, label) {
  return wait(state, () => {
    const frame = current(peer);
    if (!frame?.phase06?.World || !predicate(frame)) return null;
    assert.equal(frame.pid, peer.pid, label);
    assert.equal(frame.nonce, state.nonce, label);
    assert.equal(frame.provenance, 'read-only-state-observation');
    return frame;
  }, label);
}

async function open(context, rendered = false) {
  const nonce = path.basename(context.directory);
  assert.match(nonce, /^[\da-f-]{36}$/);
  fs.writeFileSync(path.join(context.directory, 'observation.owner'), nonce, {flag: 'wx'});
  const state = {...context, nonce, rendered, runRoot: context.directory,
    platform: process.platform + '-' + process.arch, env: {...environment(context.root), KZ_PHASE06_OWNED: nonce},
    peers: [], port: await network.allocatePort()};
  state.project = await network.prepareProject(state);
  return state;
}

// Each role owns one distinctly named profile (UI-SPEC player identity markers). The root sits outside the role
// directory and is passed as the retained root, so a rejoin of the same role keeps the same profile.
const profileNames = {solo: 'Survivor', host: 'Host', 'guest-1': 'Guest 1', 'guest-2': 'Guest 2', 'guest-3': 'Guest 3'};

function profileRoot(state, role) {
  assert(Object.hasOwn(profileNames, role), 'unnamed phase06 role ' + role);
  const root = path.join(state.directory, 'profiles', role);
  const file = path.join(root, 'profiles.json');
  if (!fs.existsSync(file)) {
    fs.mkdirSync(root, {recursive: true});
    const id = crypto.randomBytes(16).toString('hex');
    const profiles = [{id, name: profileNames[role], ordinal: 1}];
    fs.writeFileSync(file, JSON.stringify({schema: 1, selected: id, profiles}), {flag: 'wx'});
  }
  return root;
}

async function launch(state, role) {
  const previous = state.peers.find(peer => peer.role === role);
  if (previous) return previous;
  const peer = network.launch(state, role, state.port, profileRoot(state, role));
  state.peers.push(peer);
  await wait(state, () => current(peer)?.pid === peer.pid && current(peer)?.profile?.PlayerId === peer.profile,
    role + ' ordinary profile and menu');
  return peer;
}

function seed(peer, mode) {
  if (mode) fs.writeFileSync(path.join(peer.directory, 'fixture.request'), mode, {flag: 'wx'});
}

async function enter(state, role, mode = null) {
  const peer = await launch(state, role);
  seed(peer, mode);
  await activate(state, peer, role === 'host' ? 'Menu/Host' : 'Menu/Start');
  const first = await observe(state, peer, row => row.ready && row.phase06.FixtureName === mode,
    role + ' current initial authority');
  assert(!fs.existsSync(path.join(peer.directory, 'fixture.request')), 'owned fixture request was consumed');
  return {peer, first};
}

async function join(state, role) {
  const peer = await launch(state, role);
  await activate(state, peer, 'Menu/Join');
  await inject(state, peer, {action: 'address', address: '127.0.0.1:' + state.port});
  await activate(state, peer, 'Browser/JoinAddress');
  await observe(state, peer, row => row.ready && row.phase06.control, role + ' installed public and private state');
  return peer;
}

async function leaveWorkspace(peer, state, deadline) {
  if (current(peer)?.inventoryOpen) {
    await tap(state, peer, key.inventory);
    await network.waitFor([peer], deadline, () => !current(peer)?.inventoryOpen, 'close native workspace before Quit');
  }
  await network.stop(peer, state, deadline);
}

async function close(state, stop = leaveWorkspace) {
  const errors = [];
  for (const peer of [...state.peers].reverse()) {
    try { await stop(peer, state, Date.now() + 7000); }
    catch (error) { errors.push(error.message); }
  }
  for (const peer of state.peers) {
    try {
      await network.cleanup(peer);
      assert(!/(?:SCRIPT ERROR:|ERROR:|Unhandled exception|FATAL)/.test(peer.output),
        peer.role + ' native error ' + peer.output.slice(0, 1600));
    } catch (error) { errors.push(error.message); }
  }
  assert.deepEqual(errors, []);
  // A rendered group writes the record in its own close, after its owned input driver has also closed.
  if (!state.rendered) ended(state);
}

// Both group kinds write the record only after every role's terminal record proves an ordinary Quit with exit 0.
function ended(state) {
  const roles = state.peers.map(peer => {
    const record = terminal.validate(JSON.parse(fs.readFileSync(path.join(peer.directory, 'terminal.json'), 'utf8')));
    assert(record.pid === peer.pid && record.role === peer.role && !record.expectedLoss,
      peer.role + ' did not complete its ordinary Quit');
    return {role: peer.role, pid: peer.pid};
  });
  fs.writeFileSync(path.join(state.directory, 'ordinary-cleanup.json'), JSON.stringify({nonce: state.nonce, roles}),
    {flag: 'wx'});
}

function evidence(state, id, peers, observations, captures = []) {
  assert(peers.length && Object.keys(observations).length, id + ' lacks observations');
  const roles = peers.map(({role, pid, profile}) => ({role, pid, profile}));
  const file = path.join(state.directory, id + '.json');
  // Receipts of other requests seen while this case awaited its own; recorded, never accepted (see receipt()).
  const interleaved = (state.interleaved ?? []).filter(entry => entry.case === id);
  state.interleaved = [];
  fs.writeFileSync(file, JSON.stringify({id, revision: state.revision, nonce: state.nonce, rendered: state.rendered,
    roles, observations, interleaved, captures}, null, 2), {flag: 'wx'});
  return [file, path.join(state.directory, 'ordinary-cleanup.json'),
    ...peers.flatMap(peer => ['launch.json', 'frames.ndjson', 'terminal.json', 'console.log']
    .map(name => path.join(peer.directory, name))), ...captures];
}

// Each case starts with no interleaved receipts and tags the ones it records with its own id, so a case that fails
// before writing its evidence cannot leave its receipts in the next case's record.
function begin(state, id) {
  state.case = id;
  state.interleaved = [];
}

// The scenario group runs a case through this wrapper, which starts its receipt record.
const tracked = cases => Object.fromEntries(Object.entries(cases).map(([id, run]) => [id,
  (state, context) => { begin(state, id); return run(state, context); }]));

async function reset(state) {
  state.interleaved = [];
  const peer = state.peers.find(entry => entry.role === 'solo');
  if (!peer || !current(peer)?.ready) return;
  await tap(state, peer, key.escape);
  if (current(peer).ready) await tap(state, peer, key.escape);
  await wait(state, () => !current(peer)?.ready && !current(peer)?.phase06, 'ordinary solo reset');
}

// Collects every row of a stream numbered `from` or later across reads. The shared reader keeps only its latest 256
// rows, and diagnostics (frame) and the draw observer (drawFrame) number each row they write consecutively, so a
// number missing between `from` and the newest collected row means rows were dropped between reads. The span then
// fails closed with FRAME_SPAN_GAP instead of judging an incomplete interval.
function contiguous(read, key, from) {
  const rows = new Map();
  return () => {
    for (const row of read()) if (row[key] >= from) rows.set(row[key], row);
    const ordered = [...rows.values()].sort((a, b) => a[key] - b[key]);
    const gap = ordered.findIndex((row, index) => row[key] !== from + index);
    if (gap >= 0) {
      throw new Error('FRAME_SPAN_GAP ' + key + ' ' + (from + gap) + ' missing before ' + ordered[gap][key]);
    }
    return ordered;
  };
}

// Every committed frame of a peer written after `before` (a frame record), collected without gaps.
const framesAfter = (peer, before) => contiguous(() => lines(path.join(peer.directory, 'frames.ndjson')), 'frame',
  (before?.frame ?? 0) + 1);

// A receipt belongs to a submitted request only when its kind matches, it is not a replay and its sequence follows
// the one observed before submission. When the driver knows the sequence the game allocated to the request (a DEBUG
// phase06 action names it in its input ack), only that exact sequence is the receipt. The workspace submits its own
// window, page and close requests, and their receipts can replace the requested one within a single poll (F13: place
// Applied for 45 ms, then workspace Ready), so every frame written since submission is read, without gaps, not only
// the latest. Other receipts seen while waiting, before the accepted one or replacing it, are recorded in the case
// evidence and never accepted.
// `frames` may be a framesAfter(peer, before) reader the caller has already been polling while it gave the input.
function receipt(state, peer, kind, before, label, options = {}) {
  const {ms = 15_000, submitted = null, frames = framesAfter(peer, before)} = options;
  const interleaved = state.interleaved ??= [];
  return wait(state, () => {
    const seen = receiptsAfter(frames(), before);
    const index = seen.findIndex(({entry}) => matches(entry, kind, submitted));
    for (const [position, {row, entry}] of seen.entries()) {
      if (position === index || interleaved.some(noted => noted.role === peer.role && sameReceipt(noted, entry)))
        continue;
      interleaved.push({case: state.case, role: peer.role, awaiting: kind, submitted,
        relation: index < 0 || position < index ? 'before' : 'after', frame: row.frame, ...entry});
    }
    if (index < 0) return null;
    const {row} = seen[index];
    assert.equal(row.pid, peer.pid, label);
    assert.equal(row.nonce, state.nonce, label);
    assert.equal(row.provenance, 'read-only-state-observation');
    return row;
  }, label, ms);
}

const matches = (entry, kind, submitted) => entry.kind === kind && entry.replayed === false
  && (submitted === null || entry.sequence === submitted);
const sameReceipt = (a, b) => a.sequence === b.sequence && a.kind === b.kind && a.status === b.status
  && a.replayed === b.replayed;

// Each distinct receipt committed after `before` (a frame record), in order, with the first frame that showed it.
function receiptsAfter(rows, before) {
  const after = {frame: before?.frame ?? 0, sequence: before?.runAction?.sequence ?? 0};
  const seen = [];
  for (const row of rows) {
    const entry = row.runAction;
    if (row.frame <= after.frame || !row.phase06?.World || !(entry?.sequence > after.sequence)) continue;
    if (!seen.some(noted => sameReceipt(noted.entry, entry))) seen.push({row, entry});
  }
  return seen;
}

async function action(state, peer, kind, request, expected) {
  const before = current(peer);
  const submitted = await inject(state, peer, {action: 'phase06-action', kind, request});
  assert(Number.isSafeInteger(submitted) && submitted > (before.runAction?.sequence ?? 0),
    kind + ' input ack names the submitted sequence: ' + submitted);
  const result = await receipt(state, peer, kind, before, kind + ' receipt', {submitted});
  if (expected) assert.equal(result.runAction.status, expected, kind);
  return result;
}

async function move(state, peer, code, predicate, label) {
  await hold(state, peer, code, true);
  try { return await observe(state, peer, predicate, label); }
  finally { await hold(state, peer, code, false); }
}

async function shot(state, peer) {
  for (const pressed of [true, false]) await inject(state, peer, {action: 'click-world', x: 760, y: 370, pressed});
}

async function world(state) {
  const {peer, first} = await enter(state, 'solo', 'field');
  const initial = first.phase06.World;
  const hostile = initial.Actors.filter(entry => entry.Hostile);
  assert.equal(hostile.length, 3);
  assert.equal(hostile[0].X, hostile[1].X);
  assert.notEqual(hostile[0].Id, hostile[1].Id);
  assert.deepEqual(initial.Defenses.map(row => [row.Id, row.Living]),
    [['fixture.living-wall', true], ['fixture.broken-wall', false]]);
  await hold(state, peer, key.right, true);
  let walked, walking, sprinted, sprint;
  try {
    const walkStart = await observe(state, peer, row => actor(row)?.Movement === 'walk', 'walk starts');
    walked = await observe(state, peer, row => !!(walking = motionInterval(walkStart, row, 'walk')),
      'committed walking interval');
    await hold(state, peer, key.shift, true);
    const sprintStart = await observe(state, peer, row => actor(row)?.Movement === 'faster', 'sprint starts');
    sprinted = await observe(state, peer, row => {
      sprint = motionInterval(sprintStart, row, 'faster');
      return sprint?.speed > walking.speed && sprint.spent > 0;
    }, 'committed sprint displacement and stamina');
  } finally {
    await hold(state, peer, key.right, false);
    await hold(state, peer, key.shift, false);
  }
  await tap(state, peer, key.jump);
  const jumped = await observe(state, peer, row => actor(row)?.Y > initial.Ground, 'native air pose');
  assert.equal(actor(jumped).Movement, 'jump');
  assert.deepEqual(jumped.phase06.World.Actors.filter(entry => entry.Hostile).map(({Id, X}) => [Id, X]),
    hostile.map(({Id, X}) => [Id, X]));
  return evidence(state, 'phase06.headless.world', [peer], {first, walked, walking, sprinted, sprint, jumped});
}

async function start(state) {
  const {peer, first} = await enter(state, 'solo');
  const config = JSON.parse(fs.readFileSync(path.join(state.root, 'src/godot-game/game/content/config/run.json')));
  assert.equal(first.phase06.World.Actors.length, config.startingSurvivors);
  assert(first.phase06.World.Actors.every(entry => !entry.Hostile));
  assert.equal(first.phase06.World.Actors.filter(entry => entry.BindingId).length, 1);
  assert.deepEqual(own(first).Rows.map(entry => entry.DefinitionId).sort(), [...config.startingOutfit].sort());
  await move(state, peer, key.right, row => nearby(row).some(target => target.Id === stash.TargetId),
    'normal stash reaches current actor');
  await action(state, peer, 'select', stash);
  const firstPage = await observe(state, peer, row => external(row)?.Header.Page === 1, 'normal stash page one');
  const pages = [external(firstPage)];
  for (let page = 2; page <= pages[0].Header.PageCount; page++) {
    await action(state, peer, 'browse', {Token: pages[0].Token, Page: page});
    pages.push(external(await observe(state, peer, row => external(row)?.Header.Page === page, 'normal stash page ' + page)));
  }
  const stored = pages.flatMap(page => page.Rows);
  assert.equal(new Set(stored.map(entry => entry.Id)).size, stored.length);
  assert.equal(stored.length, config.initialStash.reduce((total, stack) => total + stack.stacks, 0));
  for (const stack of config.initialStash) {
    const stacks = stored.filter(entry => entry.DefinitionId === stack.item);
    assert.equal(stacks.length, stack.stacks, stack.item);
    assert(stacks.every(entry => entry.Quantity === stack.stackQuantity), stack.item);
  }
  return evidence(state, 'phase06.headless.start', [peer], {first, pages});
}

async function inventory(state) {
  const {peer, first} = await enter(state, 'solo', 'field');
  await tap(state, peer, key.inventory);
  const shown = await observe(state, peer, row => row.inventoryOpen && row.inventorySlots === 11, 'native equip view');
  const browsing = await jumpWhileBrowsing(state, peer);
  const bag = item(shown, 'own', 'IT-23');
  assert(bag?.StorageWidth > 0);
  const windows = own => inject(state, peer, {action: 'phase06-workspace', own, external: []});
  await windows([bag.Id]);
  const carried = await observe(state, peer, row => item(row, 'own.1', 'IT-22'), 'carried bag contents');
  const pouch = item(carried, 'own.1', 'IT-22');
  await windows([bag.Id, pouch.Id]);
  const nesting = await observe(state, peer, row => item(row, 'own.2', 'IT-08'), 'actual nested berries');
  const berries = item(nesting, 'own.2', 'IT-08');
  const assigned = await action(state, peer, 'assign', {Item: reference(berries), Slot: 1}, 'Applied');
  assert.equal(own(assigned).Header.Quick[0].ItemId, berries.Id);
  // Collected from the assignment on, so the food receipt is judged over every frame since it.
  const sinceAssigned = framesAfter(peer, assigned);
  await tap(state, peer, key.inventory);
  await observe(state, peer, row => sinceAssigned().length > 0 && !row.inventoryOpen, 'inventory closed');
  await tap(state, peer, key.food1);
  const consumed = await receipt(state, peer, 'food', assigned, 'native quick food receipt', {frames: sinceAssigned});
  assert.equal(consumed.runAction.status, 'Consumed');
  await tap(state, peer, key.food1);
  const cooldown = await receipt(state, peer, 'food', consumed, 'native cooldown receipt');
  assert.match(cooldown.runAction.status, /cooldown/i);
  await windows([bag.Id, pouch.Id]);
  const changed = await observe(state, peer, row => item(row, 'own.2', 'IT-08')?.Quantity === berries.Quantity - 1,
    'one real berry spent');
  const stale = await action(state, peer, 'assign', {Item: reference(berries), Slot: 2}, 'StaleItem');
  assert(!own(stale).Header.Quick.some(entry => entry.Slot === 2));
  return evidence(state, 'phase06.headless.inventory', [peer], {first, shown, browsing, carried, nesting, assigned,
    consumed, cooldown, changed, stale});
}

// The workspace captures the jump key (D-08, D-37): every committed frame after a Space tap keeps it open and grounded.
async function jumpWhileBrowsing(state, peer) {
  const before = lines(path.join(peer.directory, 'frames.ndjson')).filter(row => row.phase06?.World).at(-1);
  assert.equal(before.inventoryOpen, true, 'JUMP_KEY_BASELINE');
  // Read at every poll, so the judged span holds every committed frame from Space to the later frame, or fails.
  const frames = framesAfter(peer, before);
  await tap(state, peer, key.jump);
  const later = await observe(state, peer, row => frames().length > 0 && row.frame > before.frame
    && row.phase06.World.Elapsed >= before.phase06.World.Elapsed + 250, 'committed frames after Space while browsing');
  const span = frames().filter(row => row.frame <= later.frame && row.phase06?.World);
  assert(span.length > 1, 'JUMP_KEY_SPAN');
  for (const row of span) {
    assert.equal(row.inventoryOpen, true, 'JUMP_KEY_CLOSED_INVENTORY');
    assert.equal(actor(row).Y, row.phase06.World.Ground, 'JUMP_KEY_LEFT_GROUND');
    assert.notEqual(actor(row).Movement, 'jump', 'JUMP_KEY_STARTED_JUMP');
  }
  return later;
}

async function combat(state) {
  const {peer, first} = await enter(state, 'solo', 'bow');
  assert.equal(item(first, 'own', 'IT-02')?.Quantity, 1);
  assert.equal(actor(first).ArrowReady, true);
  await shot(state, peer);
  const fired = await observe(state, peer, row => !actor(row).ArrowReady && !item(row, 'own', 'IT-02'), 'actual arrow spent');
  assert(item(fired, 'own', 'IT-07').Condition < item(first, 'own', 'IT-07').Condition, 'actual bow wear');
  // Collected from the spent arrow on, through the cooldown, so the empty-ammo receipt is judged over every frame.
  const sinceFired = framesAfter(peer, fired);
  await observe(state, peer, row => sinceFired().length > 0 && actor(row)?.StrikeAt === null, 'bow cooldown ended');
  await shot(state, peer);
  const empty = await receipt(state, peer, 'attack', fired, 'empty ammo receipt', {frames: sinceFired});
  assert.equal(empty.runAction.status, 'EmptyAmmo');
  assert.equal(item(empty, 'own', 'IT-07').Condition, item(fired, 'own', 'IT-07').Condition);
  await inject(state, peer, {action: 'fixture-attack', Target: 'fixture.bow'});
  await inject(state, peer, {action: 'fixture-step', Elapsed: 500});
  const wall = await observe(state, peer, row => row.phase06.World.Defenses.find(entry => entry.Id === 'fixture.living-wall')
    ?.Living === false, 'actual hostile projectile destroyed living defense');
  return evidence(state, 'phase06.headless.combat', [peer], {first, fired, empty, wall});
}

async function lifecycle(state) {
  const {peer, first} = await enter(state, 'solo', 'life');
  await inject(state, peer, {action: 'fixture-attack', Target: 'fixture.melee-a'});
  const dead = await observe(state, peer, row => row.spectator?.Visible && row.phase06.deathEpoch > 0, 'committed lethal transition');
  assert.equal(dead.hud, null);
  assert(!own(dead), 'dead control cannot read private carried views');
  const corpse = dead.phase06.World.Corpses.find(body => body.SurvivorId === first.phase06.control);
  assert(corpse?.Equipment.some(entry => entry.DefinitionId === 'IT-23'));
  assert.equal(dead.spectator.CanRespawn, false);
  const ready = await observe(state, peer, row => row.spectator?.CanRespawn, 'one second host-time gate');
  await activate(state, peer, 'SpectatorCard/Respawn');
  const successor = await observe(state, peer, row => row.phase06.epoch > 0 && row.phase06.control !== first.phase06.control,
    'explicit respawn assigned a different real survivor');
  assert.equal(successor.bindingId, first.bindingId);
  assert.equal(successor.playerId, first.playerId);
  assert.deepEqual(own(successor).Rows.map(entry => entry.DefinitionId).sort(), ['IT-11', 'IT-12', 'IT-17']);
  await inject(state, peer, {action: 'fixture-step', Elapsed: 180000});
  const cleanup = await observe(state, peer, row => !row.phase06.World.Loot.some(entry => entry.Id === 'fixture.empty'),
    'empty loot expiry');
  assert(cleanup.phase06.World.Loot.some(entry => entry.Id === 'fixture.supplies'));
  assert(cleanup.phase06.World.Corpses.some(entry => entry.Id === corpse.Id));
  return evidence(state, 'phase06.headless.lifecycle', [peer], {first, dead, ready, successor, cleanup});
}

function privacy(agreement, peers) {
  assert.equal(new Set(peers.map(peer => peer.pid)).size, 4);
  assert.equal(new Set(agreement.map(row => row.bindingId)).size, 4);
  assert.equal(new Set(agreement.map(row => row.phase06.control)).size, 4);
  assert.equal(new Set(agreement.map(row => row.snapshot.SessionId)).size, 1);
  assert.equal(agreement[0].phase06.World.Actors.filter(entry => !entry.Hostile).length, 8);
  const personal = [];
  for (const row of agreement) {
    assert.equal(row.snapshot.Participants.length, 4);
    assert(row.snapshot.Participants.every(entry => entry.Ready));
    assert.deepEqual(peers.map(peer => row.snapshot.Participants.find(entry => entry.Actor.PlayerId === peer.profile)
      ?.Label), peers.map(peer => profileNames[peer.role]), 'each player publishes its own distinct profile name');
    assert.equal(own(row).Header.SurvivorId, row.phase06.control);
    assert(own(row).Rows.length >= 3 && own(row).Rows.every(entry => entry.Location.TargetId === row.phase06.control));
    personal.push(...own(row).Rows.map(entry => entry.Id));
    assert(!/"(?:Stamp|Quantity|ItemId|RootTime|Quick|Health|Stamina)"/.test(JSON.stringify(row.phase06.World)),
      'private fields absent from public publication');
  }
  assert.equal(new Set(personal).size, personal.length, 'recipient inventory instances are disjoint');
}

async function lanPrivacy(state) {
  const {peer: host} = await enter(state, 'host', 'wire');
  const peers = [host];
  for (let index = 1; index <= 3; index++) peers.push(await join(state, 'guest-' + index));
  const agreement = await wait(state, () => {
    const latest = peers.map(current);
    return latest.every(row => row?.phase06?.World?.Loot.filter(entry =>
      entry.Id.startsWith('fixture.wire-persistent-loot-')).length === 420 && own(row)?.Rows.length
      && row.snapshot?.Participants.length === 4 && row.snapshot.Participants.every(entry => entry.Ready)) && latest;
  }, 'four real recipient publications');
  privacy(agreement, peers);
  const watcher = peers[3];
  const before = agreement[3], ownRevision = own(before).Revision;
  const reachable = row => nearby(row).some(entry => entry.Kind === 'Stash' && entry.Id === stash.TargetId);
  assert(reachable(before));
  const apart = await move(state, watcher, key.left, row => nearby(row).length === 0 && actor(row).X < actor(before).X,
    'passive stash disappears on moving out of range');
  const back = await move(state, watcher, key.right, row => reachable(row) && actor(row).X > actor(apart).X,
    'passive stash appears on moving into range');
  assert([apart, back].every(row => own(row).Revision === ownRevision && !external(row)),
    'unrelated owned item rows were not republished and no target was opened');
  assert(apart.phase06.World.Elapsed > before.phase06.World.Elapsed && back.phase06.World.Elapsed > apart.phase06.World.Elapsed);
  assert(Buffer.byteLength(JSON.stringify(back.phase06.World)) > 32768, 'real public state exceeds a single packet');
  assert(Math.abs(back.snapshot.Participants.find(row => row.BindingId === back.bindingId).Actor.PositionSubunits
    - (actor(back).X - back.phase06.World.OriginX)) <= 0.5, 'current roster position travels with its world');
  await tap(state, watcher, key.interact);
  const selector = await observe(state, watcher, settledNearbyWorkspace,
    'guest opened the nearby selector and installed its equipped storage scopes');
  await activate(state, watcher, 'InventoryWorkspace/InventoryPanels/NextNearbyPage');
  const next = await observe(state, watcher, row => row.phase06.Views.nearby?.Header.Page === 2
    && nearby(row).length > 0 && !reachable(row), 'native next page keeps the bounded authorized target list');
  assert.equal(own(next).Revision, own(selector).Revision, 'paging never replaces its unrelated inventory rows');
  assert.deepEqual(own(next).Rows, own(selector).Rows);
  const secondPageTarget = next.phase06.Views.nearby.Rows[0];
  const node = 'NearbyChoice_Pile_' + Buffer.from(secondPageTarget.Id).toString('hex').toUpperCase();
  await activate(state, watcher, 'InventoryWorkspace/InventoryPanels/NearbyChoices/Rows/' + node);
  const pile = await observe(state, watcher, row => external(row)?.Header.TargetId === secondPageTarget.Id,
    'new page offers actual authorized loot');
  await activate(state, watcher, 'InventoryWorkspace/InventoryPanels/NearbyTarget');
  await activate(state, watcher, 'InventoryWorkspace/InventoryPanels/PreviousNearbyPage');
  const returned = await observe(state, watcher, row => row.phase06.Views.nearby?.Header.Page === 1
    && reachable(row) && external(row)?.Token === external(pile).Token,
  'return to the reachable stash page preserves the previously selected loot authorization');
  const dense = await checkLooseItems(state, watcher, peers);
  await activate(state, watcher, 'InventoryWorkspace/InventoryPanels/NearbyChoices/Rows/NearbyChoice_Stash');
  const selected = await observe(state, watcher, row => external(row)?.Header.TargetId === stash.TargetId
    && row.nearby?.kind === 'Stash', 'actual native choice authorizes the approached stash');
  await tap(state, watcher, key.inventory);
  await observe(state, watcher, row => !row.inventoryOpen && !external(row), 'native stash selection closed');
  const guests = peers.slice(1, 3);
  for (const peer of guests) {
    await action(state, peer, 'select', stash, 'Ready');
    const open = await observe(state, peer, row => item(row, 'external', 'IT-23'), 'currently visible backpack');
    const bag = item(open, 'external', 'IT-23');
    await action(state, peer, 'place', {Item: reference(bag), Token: external(open).Token, Container: null,
      Amount: null, Destination: {Kind: 'equipment', TargetId: open.phase06.control, Slot: 'backpack',
        GridId: null, X: null, Y: null, Rotated: null}}, 'Applied');
    await observe(state, peer, row => item(row, 'own', 'IT-23')?.Id === bag.Id, 'guest equipped its own bag');
  }
  const views = await Promise.all(guests.map(peer => observe(state, peer, row => external(row)?.Header.Page === 1,
    'current guest stash view')));
  const contested = item(views[0], 'external', 'IT-01');
  assert(contested && external(views[1]).Rows.some(row => row.Id === contested.Id && row.Stamp === contested.Stamp));
  const receipts = await Promise.all(guests.map((peer, index) => action(state, peer, 'quick',
    {Item: reference(contested), Token: external(views[index]).Token, Deposit: false})));
  assert.deepEqual(receipts.map(row => row.runAction.status).sort(), ['AccessLost', 'Applied']);
  const corrected = await Promise.all(guests.map((peer, index) => {
    const storage = own(receipts[index]).Rows.filter(entry => entry.StorageWidth > 0).map(entry => entry.Id);
    return inject(state, peer, {action: 'phase06-workspace', own: storage, external: []})
      .then(() => observe(state, peer, row => storage.every(id => Object.entries(row.phase06.Views)
        .some(([scope, view]) => scope.startsWith('own.') && view.Header.ContainerId === id)) && external(row)?.Rows
        .every(entry => entry.Id !== contested.Id), 'corrected stash and own bag'));
  }));
  for (const [index, row] of corrected.entries()) {
    const claimed = Object.entries(row.phase06.Views).filter(([scope]) => scope.startsWith('own.'))
      .flatMap(([, view]) => view.Rows).find(entry => entry.Id === contested.Id);
    if (receipts[index].runAction.status === 'Applied') assert.equal(claimed?.Id, contested.Id);
    else assert.equal(claimed, undefined);
  }
  const guest = guests[0];
  await move(state, guest, key.left, row => !nearby(row).some(entry => entry.Id === stash.TargetId),
    'guest leaves interaction reach');
  const revoked = await observe(state, guest, row => !external(row), 'stale external scope revoked');
  const stopped = await observe(state, host, row => row.lastAcceptedIntent?.BindingId === revoked.bindingId
    && row.lastAcceptedIntent.Direction === 0, 'the host accepted the ordinary movement release');
  await observe(state, guest, row => row.runAction?.sequence >= stopped.lastAcceptedIntent.Sequence,
    'settle the movement receipt before submitting the remote selection');
  const remote = await action(state, guest, 'select', stash, 'AccessLost');
  assert(!external(remote), 'remote stash reveals no rows or token');
  return evidence(state, 'phase06.headless.lan-privacy', peers,
    {agreement, apart, back, next, pile, returned, dense, selected, views, receipts, corrected, revoked, remote});
}

function settledNearbyWorkspace(row) {
  const owned = own(row);
  if (!row.inventoryOpen || !owned) return false;
  const windows = Object.entries(row.phase06.Views).filter(([scope]) => scope.startsWith('own.'));
  return owned.Rows.filter(item => item.Location.Kind === 'equipment' && item.StorageWidth > 0)
    .every(item => windows.some(([, view]) => view.Header.ContainerId === item.Id && view.Revision === owned.Revision));
}

async function checkLooseItems(state, watcher, peers) {
  const target = 'fixture.supplies';
  const path = 'InventoryWorkspace/InventoryPanels/';
  const choice = 'NearbyChoices/Rows/NearbyChoice_Pile_' + Buffer.from(target).toString('hex').toUpperCase();
  await activate(state, watcher, path + choice);
  const first = await observe(state, watcher, row => external(row)?.Header.TargetId === target
    && external(row).Header.ItemPage === 1, 'dense pile opens in the normal authorized item list');
  assert.equal(external(first).Header.ItemCount, 4098);
  assert.equal(external(first).Rows.length, 64);
  assert.equal(external(first).Rows[0].Quantity, 17);
  assert(peers.filter(peer => peer !== watcher).every(peer => !external(current(peer))),
    'no dense pile rows or token reach the other three processes');
  const bag = item(first, 'external', 'IT-23');
  assert(bag?.Id);
  const opened = await openLooseBagTree(state, watcher, path, bag);
  await activate(state, watcher, path + 'NextNearbyItemsPage');
  const second = await observe(state, watcher, row => external(row)?.Header.ItemPage === 2,
    'native next item page installs authorized original loot rows');
  for (const id of [bag.Id, opened.pouch.Id]) {
    await inject(state, watcher, {action: 'focus', control: path + 'Bag_' + id + '/MoveBag'});
  }
  assert(windowFor(second, bag.Id) && windowFor(second, opened.pouch.Id),
    'the open bag and descendant retain exact authorized scopes while their root is off-page');
  assert.equal(external(second).Token, external(first).Token);
  assert.equal(external(second).Rows.length, 64);
  assert(external(second).Rows.every((row, index, rows) => row.Id > (rows[index - 1]?.Id ?? external(first).Rows.at(-1).Id)
    && row.Location.TargetId === target && row.Quantity === 1 && row.Stamp.length === 64));
  assert.equal(own(second).Revision, own(opened.contents).Revision);
  await activate(state, watcher, path + 'PreviousNearbyItemsPage');
  const again = await observe(state, watcher, row => external(row)?.Header.ItemPage === 1,
    'native previous item page returns the same original loot');
  assert.deepEqual(external(again).Rows, external(first).Rows);
  await activate(state, watcher, path + 'NextNearbyItemsPage');
  await observe(state, watcher, row => external(row)?.Header.ItemPage === 2, 'restore off-page bag view');
  const grip = path + 'Bag_' + opened.pouch.Id + '/MoveBag';
  await inject(state, watcher, {action: 'focus', control: grip});
  const refreshed = await updateLoosePile(state, peers[0], watcher, bag.Id, opened.pouch.Id, target);
  await inject(state, watcher, {action: 'phase06-focused', control: grip});
  await action(state, watcher, 'place', {Item: reference(bag), Token: external(refreshed).Token, Container: null,
    Amount: null, Destination: {Kind: 'equipment', TargetId: first.phase06.control, Slot: 'backpack',
      GridId: null, X: null, Y: null, Rotated: null}}, 'Applied');
  const moved = await observe(state, watcher, row => item(row, 'own', 'IT-23')?.Id === bag.Id && external(row)
    && !windowFor(row, bag.Id) && !windowFor(row, opened.pouch.Id),
  'moving the off-page bag to carried equipment revokes both external windows');
  await tap(state, watcher, key.escape);
  await observe(state, watcher, row => !row.inventoryOpen && !external(row),
    'one Escape closes the workspace after the transferred bag and descendant have closed');
  await tap(state, watcher, key.interact);
  await observe(state, watcher, row => row.inventoryOpen && !external(row),
    'open the nearby inventory selector without the transferred bag context');
  return {first, opened: opened.contents, second, again, refreshed, moved};
}

async function updateLoosePile(state, host, watcher, bagId, pouchId, target) {
  await action(state, host, 'select', {TargetKind: 'Pile', TargetId: target}, 'Ready');
  const open = await observe(state, host, row => item(row, 'external', 'IT-01'), 'host found a separate loose stack');
  await action(state, host, 'quick', {Item: reference(item(open, 'external', 'IT-01')),
    Token: external(open).Token, Deposit: false}, 'Applied');
  return observe(state, watcher, row => external(row)?.Header.ItemCount === 4097
    && windowFor(row, bagId) && windowFor(row, pouchId),
  'another controller’s ordinary withdrawal refreshes the active bag tree and page');
}

function windowFor(row, id) {
  return Object.entries(row.phase06.Views).find(([scope, view]) => scope.startsWith('external.')
    && view.Header.ContainerId === id && view.Token === external(row)?.Token)?.[1];
}

async function openLooseBagTree(state, peer, panels, bag) {
  const openBag = async control => {
    await inject(state, peer, {action: 'focus', control});
    await tap(state, peer, key.menu);
    await tap(state, peer, key.enter);
  };
  await openBag(panels + 'NearbyStorage/Rows/NearbyItem_' + bag.Id);
  const open = await observe(state, peer, row => windowFor(row, bag.Id)?.Rows.some(entry => entry.DefinitionId === 'IT-22'),
    'open the native loot bag from its committed row');
  const pouch = windowFor(open, bag.Id).Rows.find(entry => entry.DefinitionId === 'IT-22');
  await openBag(panels + 'Bag_' + bag.Id + '/BagStorage/Rows/ItemCells/Item_' + pouch.Id);
  const contents = await observe(state, peer, row => windowFor(row, pouch.Id)?.Rows.some(entry => entry.DefinitionId === 'IT-08'),
    'open the native nested pouch and observe its original berries');
  return {pouch, contents};
}

module.exports = {open, reset, close: state => close(state), cases: tracked({
  'phase06.headless.world': world,
  'phase06.headless.start': start,
  'phase06.headless.inventory': inventory,
  'phase06.headless.combat': combat,
  'phase06.headless.lifecycle': lifecycle,
  'phase06.headless.lan-privacy': lanPrivacy,
}), helpers: {open, close, ended, key, current, actor, own, nearby, external, item, wait, observe, launch, seed,
  evidence, privacy, inject, motionInterval, profileNames, receipt, receiptsAfter, contiguous, framesAfter, begin,
  tracked}};

