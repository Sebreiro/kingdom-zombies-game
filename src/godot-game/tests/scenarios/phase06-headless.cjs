'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const network = require('../../tools/lan-checks/network.cjs');
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

async function launch(state, role) {
  const previous = state.peers.find(peer => peer.role === role);
  if (previous) return previous;
  const peer = network.launch(state, role, state.port);
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
  if (!state.rendered) ended(state);
}

function ended(state) {
  fs.writeFileSync(path.join(state.directory, 'ordinary-cleanup.json'), JSON.stringify({
    nonce: state.nonce, roles: state.peers.map(({role, pid}) => ({role, pid})),
  }), {flag: 'wx'});
}

function evidence(state, id, peers, observations, captures = []) {
  assert(peers.length && Object.keys(observations).length, id + ' lacks observations');
  const roles = peers.map(({role, pid, profile}) => ({role, pid, profile}));
  const file = path.join(state.directory, id + '.json');
  fs.writeFileSync(file, JSON.stringify({id, revision: state.revision, nonce: state.nonce, rendered: state.rendered,
    roles, observations, captures}, null, 2), {flag: 'wx'});
  return [file, path.join(state.directory, 'ordinary-cleanup.json'),
    ...peers.flatMap(peer => ['launch.json', 'frames.ndjson', 'terminal.json', 'console.log']
    .map(name => path.join(peer.directory, name))), ...captures];
}

async function reset(state) {
  const peer = state.peers.find(entry => entry.role === 'solo');
  if (!peer || !current(peer)?.ready) return;
  await tap(state, peer, key.escape);
  if (current(peer).ready) await tap(state, peer, key.escape);
  await wait(state, () => !current(peer)?.ready && !current(peer)?.phase06, 'ordinary solo reset');
}

async function action(state, peer, kind, request, expected) {
  const previous = current(peer)?.runAction?.sequence ?? 0;
  await inject(state, peer, {action: 'phase06-action', kind, request});
  const result = await observe(state, peer, row => row.runAction?.sequence > previous, kind + ' receipt');
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
  await tap(state, peer, key.inventory);
  await observe(state, peer, row => !row.inventoryOpen, 'inventory closed');
  await tap(state, peer, key.food1);
  const consumed = await observe(state, peer, row => row.runAction?.sequence > assigned.runAction.sequence,
    'native quick food receipt');
  assert.equal(consumed.runAction.status, 'Consumed');
  await tap(state, peer, key.food1);
  const cooldown = await observe(state, peer, row => row.runAction?.sequence > consumed.runAction.sequence, 'native cooldown receipt');
  assert.match(cooldown.runAction.status, /cooldown/i);
  await windows([bag.Id, pouch.Id]);
  const changed = await observe(state, peer, row => item(row, 'own.2', 'IT-08')?.Quantity === berries.Quantity - 1,
    'one real berry spent');
  const stale = await action(state, peer, 'assign', {Item: reference(berries), Slot: 2}, 'StaleItem');
  assert(!own(stale).Header.Quick.some(entry => entry.Slot === 2));
  return evidence(state, 'phase06.headless.inventory', [peer], {first, shown, carried, nesting, assigned, consumed,
    cooldown, changed, stale});
}

async function combat(state) {
  const {peer, first} = await enter(state, 'solo', 'bow');
  assert.equal(item(first, 'own', 'IT-02')?.Quantity, 1);
  assert.equal(actor(first).ArrowReady, true);
  await shot(state, peer);
  const fired = await observe(state, peer, row => !actor(row).ArrowReady && !item(row, 'own', 'IT-02'), 'actual arrow spent');
  assert(item(fired, 'own', 'IT-07').Condition < item(first, 'own', 'IT-07').Condition, 'actual bow wear');
  await observe(state, peer, row => actor(row)?.StrikeAt === null, 'bow cooldown ended');
  await shot(state, peer);
  const empty = await observe(state, peer, row => row.runAction?.sequence > fired.runAction.sequence, 'empty ammo receipt');
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
  const selector = await observe(state, watcher, row => row.inventoryOpen && own(row),
    'guest opened the live nearby selector');
  await activate(state, watcher, 'InventoryWorkspace/InventoryPanels/NextNearbyPage');
  const next = await observe(state, watcher, row => row.phase06.Views.nearby?.Header.Page === 2
    && nearby(row).length > 0 && !reachable(row), 'native next page keeps the bounded authorized target list');
  assert.equal(own(next).Revision, own(selector).Revision, 'paging never replaces its unrelated inventory rows');
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
  await action(state, watcher, 'place', {Item: reference(bag), Token: external(second).Token, Container: null,
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
  return {first, opened: opened.contents, second, again, moved};
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

module.exports = {open, reset, close: state => close(state), cases: {
  'phase06.headless.world': world,
  'phase06.headless.start': start,
  'phase06.headless.inventory': inventory,
  'phase06.headless.combat': combat,
  'phase06.headless.lifecycle': lifecycle,
  'phase06.headless.lan-privacy': lanPrivacy,
}, helpers: {open, close, ended, key, current, actor, own, nearby, external, item, wait, observe, launch, seed,
  evidence, privacy, inject, motionInterval}};

