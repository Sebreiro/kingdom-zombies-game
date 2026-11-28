'use strict';

const assert = require('node:assert/strict');
const path = require('node:path');
const run = require('./phase06-rendered.cjs');
const {helpers: native} = require('./phase06-headless.cjs');
const {draws, lines} = require('../../tools/lan-checks/rendered.cjs');
const {named, within, key, actor, own, nearby, item, external} = run;
const roles = ['host', 'guest-1', 'guest-2', 'guest-3'];
const center = (row, id) => Math.max(-320, Math.min(320,
  (actor(row, id)?.X - row.phase06.World.OriginX) * 16 / 1000));
const stableCamera = (frame, draw) => frame.input.spectator.Camera === draw.input.spectator.Camera
  && Math.abs(frame.cameraX - draw.cameraX) < 0.02;
const follows = (row, draw, id) => row.spectator?.Followed?.Survivor === id
  && Math.abs(row.spectator.Camera - center(row, id)) < 0.02
  && Math.abs(draw.cameraX - row.spectator.Camera) < 0.02;

function formed(row, session) {
  return row.snapshot?.SessionId === session && row.snapshot.Participants.length === roles.length
    && row.snapshot.Participants.every(entry => entry.Ready) && row.phase06.World.Actors.length >= 8;
}

async function startLan(state, mode) {
  const [host, ...guests] = state.peers;
  const joined = await run.enter(state, host, mode);
  const session = joined.snapshot.SessionId;
  for (const peer of guests) await run.join(state, peer, session);
  const roster = await Promise.all(state.peers.map(peer => run.committed(peer,
    row => formed(row, session) && own(row)?.Rows.length >= 3, 'bound four-role recipient view')));
  const rows = roster.map(draw => draw.input);
  run.privacy(rows, state.peers);
  const identities = rows.map(row => ({binding: row.bindingId, player: row.playerId, actor: row.phase06.control}));
  return {host, guests, session, roster, identities};
}

// Every LAN image shows a settled, unclipped view in addition to the case's own state.
const shot = (state, peer, label, predicate, minimumFrame) => run.picture(state, peer, label,
  (row, draw) => run.settledView(draw) && predicate(row, draw), minimumFrame);

async function allPictures(state, label, condition) {
  return Promise.all(state.peers.map(peer => shot(state, peer, label + '-' + peer.role,
    (row, draw) => condition(row, draw, peer))));
}

// The roster names every ready participant 'P{slot} · {label}' in slot order and marks only the receiver's own row
// with ' (You)' after its number (FoundationHud.Project, UI-SPEC player identity markers). Names must be distinct.
function rosterMatches(draw) {
  const ready = (draw?.snapshot?.Participants ?? []).filter(entry => entry.Ready).sort((a, b) => a.Slot - b.Slot);
  if (ready.length !== roles.length || new Set(ready.map(entry => entry.Label)).size !== ready.length) return false;
  const rows = (draw.controls ?? []).filter(entry => /^Roster\/Player\d$/.test(entry.path) && entry.visible);
  return rows.length === ready.length && ready.every((entry, index) => rows.find(row => row.path === 'Roster/Player'
    + (index + 1))?.text === 'P' + entry.Slot + (entry.BindingId === draw.bindingId ? ' (You)' : '') + ' · '
    + entry.Label) && rows.filter(row => row.text.includes('(You)')).length === 1;
}

// Spec 3.1 defines u as about a standing survivor's body width; the world draws 1000 subunits (1 u) as 16 px.
const bodyWidth = 16;

// The four controlled bodies stand at pairwise different drawn X, at least one body width apart.
function spread(draw) {
  const bodies = (draw?.actors ?? []).filter(entry => entry.visible).map(entry => entry.screenX);
  return bodies.length === roles.length
    && bodies.every((x, index) => bodies.slice(index + 1).every(other => Math.abs(x - other) >= bodyWidth));
}

// Guest n walks right by at least 1.5 n u of its own position units; the host stays.
const separation = [0, 1500, 3000, 4500];

async function population(state) {
  const start = state.input.records.length;
  const {host, guests, session, roster, identities} = await startLan(state, null);
  assert(roster.every(draw => draw.input.phase06.World.Actors.length === 8
    && draw.input.phase06.World.Actors.every(entry => !entry.Hostile)));
  const targets = roster.map((draw, index) => ({id: identities[index].actor,
    x: actor(draw.input).X + separation[index]}));
  const movement = [];
  for (const [index, mover] of guests.entries()) {
    const target = targets[index + 1];
    await run.focus(state, mover);
    movement.push(await run.moveUntil(state, mover, key.right, row => actor(row)?.X >= target.x,
      mover.role + ' physically walks its survivor apart'));
  }
  const pictures = await allPictures(state, 'population', (row, draw) => formed(row, session)
    && targets.every(target => actor(row, target.id)?.X >= target.x)
    && actor(row, targets[0].id)?.X === actor(roster[0].input).X
    && draw.appearance.filter(entry => entry.kind === 'Actors' && entry.visible).length === 8
    && draw.appearance.filter(entry => entry.crown).length === 4
    && draw.actors.some(entry => entry.local && entry.insideCamera)
    && spread(draw) && rosterMatches(draw) && profilesBound(draw, state.peers));
  assert.equal(host, state.peers[0]);
  return run.finish(state, 'phase06.lan.population', state.peers, start, {identities, targets, movement}, pictures);
}

// Every receiver shows each player's own profile name on that player's participant.
function profilesBound(draw, peers) {
  return peers.every(peer => draw.snapshot.Participants.find(entry => entry.Actor.PlayerId === peer.profile)?.Label
    === run.profileNames[peer.role]);
}

// Ctrl+click in frame-separated native steps: Ctrl is acknowledged, the pointer rests on the cell for a drawn frame,
// and the click is handled while Ctrl is still held for another drawn frame. Run 1's burst (Ctrl, click and release
// delivered within draws 511-513) submitted no quick request at all. `frames` (a native.framesAfter reader) is read
// at every drawn step, so the peer's committed frames stay gap-free while the input is given. Returns the draw the
// click follows.
async function withdraw(state, peer, point, frames) {
  const drawn = () => { frames(); return true; };
  await run.press(state, peer, key.control, true);
  try {
    const before = run.last(peer).drawFrame;
    await run.pointer(state, peer, point);
    const resting = await run.after(peer, drawn, 'drawn with the pointer on the contested cell', before + 1);
    await run.click(state, peer, point);
    await run.after(peer, drawn, 'drawn after the Ctrl click', resting.drawFrame + 1);
    return resting;
  } finally { await run.press(state, peer, key.control, false); }
}

const money = row => row.phase06.Views.status.Header.Hud.Money;
const quick = (frames, before) => native.receiptsAfter(frames, before)
  .filter(({entry}) => entry.kind === 'quick' && entry.replayed === false).map(({row, entry}) => ({frame: row.frame,
    ...entry}));

async function conflicts(state) {
  const start = state.input.records.length;
  const {host, guests, session, identities} = await startLan(state, 'field');
  const [first, second] = guests;
  for (const peer of guests) {
    await run.focus(state, peer);
    await run.stash(state, peer);
  }
  const originals = await allPictures(state, 'conflict-original', row => formed(row, session)
    && !!own(row) && (row.bindingId === identities[0].binding || external(row)?.Header.Kind === 'Stash'));
  const source = item(originals[1].draw.input, 'external', 'IT-01');
  const secondItem = item(originals[2].draw.input, 'external', 'IT-01');
  assert(source && secondItem && source.Id === secondItem.Id && source.Stamp === secondItem.Stamp);
  // Each role's committed carried total: its own original money plus what its own applied withdrawal moved.
  const committed = new Map(state.peers.map((peer, index) => [peer, money(originals[index].draw.input)]));
  const occupied = within('/NearbyStorage/', 'Item_' + source.Id);
  const losingPoint = await run.control(second, occupied);

  await run.focus(state, first);
  const winnerBefore = run.last(first);
  const winnerFrames = native.framesAfter(first, winnerBefore.input);
  const winnerMark = run.pressMark(first);
  await withdraw(state, first, await run.control(first, occupied), winnerFrames);
  const winnerReceipt = await run.explainPress(first, winnerMark, native.receipt(state, first, 'quick',
    winnerBefore.input, 'one authority withdrawal', {frames: winnerFrames}));
  const winnerPress = run.pressSince(first, winnerMark);
  assert.equal(winnerReceipt.runAction.status, 'Applied', 'the first withdrawal applies');
  committed.set(first, committed.get(first) + source.Quantity);
  const winner = await run.committed(first, row => external(row)?.Rows.every(entry => entry.Id !== source.Id)
    && money(row) === committed.get(first), 'corrected source and the winner\'s committed total',
  winnerBefore.drawFrame);
  await run.focus(state, second);
  const before = run.last(second);
  const secondFrames = native.framesAfter(second, before.input);
  const loserMark = run.pressMark(second);
  const resting = await withdraw(state, second, losingPoint, secondFrames);
  // The click follows the resting draw: a cell still visible there sends a quick request, whose own receipt must
  // deny it; a cell already corrected there sends none. A draw sampled before the pointer moved cannot decide it.
  const staleCell = resting.controls.some(entry => occupied(entry) && entry.visible);
  const denial = staleCell ? await run.explainPress(second, loserMark, native.receipt(state, second, 'quick',
    before.input, 'contested withdrawal denial', {frames: secondFrames})) : null;
  const loserPress = run.pressSince(second, loserMark);
  assert(!denial || ['AccessLost', 'StaleItem'].includes(denial.runAction.status),
    'the losing attempt is denied, not applied: ' + denial?.runAction.status);
  const corrected = await run.committed(second, row => secondFrames().length > 0
    && external(row)?.Rows.every(entry => entry.Id !== source.Id) && money(row) === committed.get(second),
  'contested cell corrected without duplicate', before.drawFrame);
  const laterReceipts = quick(secondFrames(), before.input);
  assert(!laterReceipts.some(entry => entry.status === 'Applied'), 'losing attempt must not apply');
  const observedStale = laterReceipts.some(entry => entry.status === 'AccessLost' || entry.status === 'StaleItem');
  assert(observedStale || !staleCell, 'contested physical action lacked either visible correction or a denial');

  const still = actor(corrected.input).X;
  await run.tap(state, second, key.right);
  await run.committed(second, row => row.inventoryOpen && actor(row)?.X === still, 'panel captures movement');
  await run.choose(state, second, named('CloseInventory'));
  await run.moveUntil(state, second, key.left, row => !nearby(row).some(entry => entry.Kind === 'Stash'),
    'ordinary guest movement leaves stash reach');
  const denied = await shot(state, second, 'conflict-revoked', row => !external(row)
    && !nearby(row).some(entry => entry.Kind === 'Stash'));
  const final = await allPictures(state, 'conflict-current', (row, _, peer) => formed(row, session)
    && money(row) === committed.get(peer) && (external(row)?.Rows ?? []).every(entry => entry.Id !== source.Id));
  assert.equal(committed.get(host), money(originals[0].draw.input));
  return run.finish(state, 'phase06.lan.conflicts', state.peers, start,
    {source, winnerReceipt, winnerPress, winner, before, resting, staleCell, denial, loserPress, corrected,
      observedStale, laterReceipts,
      committed: state.peers.map(peer => ({role: peer.role, money: committed.get(peer)}))},
    [...originals, ...final, denied]);
}

async function succession(state) {
  const start = state.input.records.length;
  const {host, guests, session, identities} = await startLan(state, 'life');
  const previous = identities[0];
  const frames = from => native.contiguous(() => lines(path.join(host.directory, 'frames.ndjson')), 'frame', from);
  // The host's committed draws after a known draw, so a hold is judged over every draw between it and a later one.
  const span = from => {
    const later = native.contiguous(() => draws(host), 'drawFrame', from.drawFrame + 1);
    return to => [from, ...later().filter(draw => draw.drawFrame <= to.drawFrame && draw.input?.phase06?.World)];
  };
  await run.focus(state, host);
  await run.fixture(state, host, {action: 'fixture-attack', Target: 'fixture.melee-a'});
  const death = await shot(state, host, 'succession-death', (row, draw) => row.spectator?.Visible
    && !own(row) && !row.hud && Number.isFinite(row.spectator.Camera) && !row.spectator.Followed
    && !row.spectator.CanRespawn && respawnButton(draw)?.disabled === true
    && row.phase06.World.Corpses.some(entry => entry.SurvivorId === previous.actor && draw.appearance
      .some(art => art.id === entry.Id && art.kind === 'Corpses' && art.visible && !art.crown)));
  const dying = frames(death.draw.input.frame);
  const afterDeath = span(death.draw);
  const opened = await run.committed(host, (row, draw) => dying().length > 0 && afterDeath(draw).length > 0
    && row.spectator?.CanRespawn && respawnButton(draw)?.disabled === false,
  'Respawn enabled after the one second gate', death.draw.drawFrame);
  const gate = respawnGate(dying().filter(row => row.frame <= opened.input.frame), previous.actor);
  assert(gate, 'Respawn stays disabled before one second after death and is enabled after it (AC-02)');

  const initial = actor(death.draw.input, identities[1].actor).X;
  await run.focus(state, guests[0]);
  await run.moveUntil(state, guests[0], key.right, row => actor(row)?.X > initial + 1150,
    'living teammate separates from retired camera');
  const deathHold = await run.committed(host, (row, draw) => row.spectator?.Visible
    && actor(row, identities[1].actor).X > initial + 1150 && cameraHold(afterDeath(draw),
      identities[1].actor, 1150), 'death camera remains fixed while a teammate travels', death.draw.drawFrame);

  await run.focus(state, host);
  const order = eligible(deathHold.input);
  assert.deepEqual(order.map(entry => entry.SurvivorId), identities.slice(1).map(entry => entry.actor),
    'only living teammates, in slot order');
  const firstFollow = await run.choose(state, host, named('NextTeammate'));
  const next = await shot(state, host, 'succession-next', (row, draw) =>
    follows(row, draw, order[0].SurvivorId) && Math.abs(draw.cameraX - deathHold.cameraX) > 10
    && choicesMatch(row, draw), firstFollow);
  const previousFollow = await run.choose(state, host, named('PreviousTeammate'));
  const prior = await shot(state, host, 'succession-previous', (row, draw) =>
    follows(row, draw, order.at(-1).SurvivorId) && choicesMatch(row, draw), previousFollow);
  assert.equal(order[1].SurvivorId, identities[2].actor);
  await run.choose(state, host, named('Teammate_' + order[1].Slot));
  const chosen = await shot(state, host, 'succession-chosen', (row, draw) =>
    follows(row, draw, identities[2].actor) && row.spectator.CanRespawn && respawnButton(draw)?.disabled === false
    && choicesMatch(row, draw));
  const targetX = actor(chosen.draw.input, identities[2].actor).X;
  await run.focus(state, guests[1]);
  await run.moveUntil(state, guests[1], key.right, row => actor(row)?.X > targetX + 1150, 'followed teammate travels');
  const tracking = await run.committed(host, (row, draw) => follows(row, draw, identities[2].actor)
    && Math.abs(draw.cameraX - chosen.draw.cameraX) > 10
    && actor(row, identities[2].actor).Movement === 'idle', 'actual camera tracks selected teammate',
  chosen.draw.drawFrame);

  await run.menu(state, guests[1]);
  const hold = await shot(state, host, 'succession-lost', (row, draw) => row.spectator?.Visible
    && !row.spectator.Followed && !!row.spectator.FollowNotice && stableCamera(tracking, draw)
    && !eligible(row).some(entry => entry.SurvivorId === identities[2].actor), tracking.drawFrame);
  const other = actor(hold.draw.input, identities[3].actor).X;
  const afterLoss = span(hold.draw);
  await run.focus(state, guests[2]);
  await run.moveUntil(state, guests[2], key.right, row => actor(row)?.X > other + 300, 'different teammate travels');
  const lostHold = await run.committed(host, (row, draw) => row.spectator?.FollowNotice
    && actor(row, identities[3].actor)?.X > other + 300
    && cameraHold(afterLoss(draw), identities[3].actor, 300),
  'lost target camera holds across committed draws', hold.draw.drawFrame);

  await run.join(state, guests[1], session);
  await run.focus(state, host);
  // The host's view once the rejoined guest is ready again: the survivors then bound to nobody are the candidates.
  const respawnFrom = await run.committed(host, (row, draw) => formed(row, session) && row.spectator?.CanRespawn
    && stableCamera(lostHold, draw), 'four ready players before Respawn', lostHold.drawFrame);
  const transferring = frames(respawnFrom.input.frame + 1);
  const unbound = respawnFrom.input.phase06.World.Actors.filter(entry => !entry.Hostile && !entry.BindingId)
    .map(entry => entry.Id);
  await run.choose(state, host, named('Respawn'));
  const transfer = await shot(state, host, 'succession-transfer', (row, draw) => transferring().length > 0
    && formed(row, session) && row.phase06.epoch > 0 && row.phase06.control !== previous.actor
    && unbound.includes(row.phase06.control) && row.bindingId === previous.binding && row.playerId === previous.player
    && row.spectator?.TransferNotice === 'Control transferred · ' + row.phase06.control
    && draw.controls.some(entry => named('ControlTransferred')(entry) && entry.visible
      && entry.text === row.spectator.TransferNotice), respawnFrom.drawFrame);
  const successor = transfer.draw.input.phase06.control;
  assert(!identities.slice(1).some(entry => entry.actor === successor));
  const camera = handover(transferring().filter(row => row.frame <= transfer.draw.input.frame), respawnFrom.cameraX,
    center(transfer.draw.input, successor));
  assert(camera && camera.observed === camera.expected, 'transfer camera follows the quarter-span rule: '
    + JSON.stringify(camera));
  const final = await allPictures(state, 'succession-current', (row, draw, peer) => formed(row, session)
    && actor(row, successor)?.BindingId === previous.binding
    && (peer === host || row.phase06.control === identities[roles.indexOf(peer.role)].actor)
    && draw.appearance.some(entry => entry.id === successor && entry.crown && entry.visible));
  return run.finish(state, 'phase06.lan.succession', state.peers, start,
    {identities, gate, order, successor, unbound, camera, deathHold, tracking, lostHold},
    [death, next, prior, chosen, hold, transfer, ...final]);
}

const respawnButton = draw => draw.controls.find(entry => named('SpectatorCard/Respawn')(entry) && entry.visible);

// The dead player's living teammates from its own status view, ready in the roster and in slot order.
function eligible(row) {
  const ready = new Set((row.snapshot?.Participants ?? []).filter(entry => entry.Ready).map(entry => entry.BindingId));
  return (row.phase06.Views.status?.Header.Teammates ?? []).filter(entry => ready.has(entry.BindingId)
    && entry.BindingId !== row.bindingId).sort((a, b) => a.Slot - b.Slot);
}

// The selector lists exactly the eligible teammates' slots, in slot order.
function choicesMatch(row, draw) {
  const shown = draw.controls.filter(entry => /\/Teammates\/Teammate_\d+$/.test(entry.path) && entry.visible)
    .sort((a, b) => a.y - b.y).map(entry => Number(entry.path.split('_').at(-1)));
  return JSON.stringify(shown) === JSON.stringify(eligible(row).map(entry => entry.Slot));
}

// D-33/D-34: a held camera keeps one pose over at least two distinct committed draws while a teammate deliberately
// moves at least `distance` subunits between the first and the last of them. A single draw or a moving camera fails.
function cameraHold(span, teammate, distance) {
  const held = span.filter(draw => draw?.input?.spectator?.Visible && !draw.input.spectator.Followed);
  if (held.length !== span.length || new Set(held.map(draw => draw.drawFrame)).size < 2) return false;
  const [first] = held;
  const moved = actor(held.at(-1).input, teammate)?.X - actor(first.input, teammate)?.X;
  return held.every(draw => stableCamera(first, draw)) && Math.abs(moved) >= distance;
}

// AC-02 over committed frames from death to the enabled button: every frame before the authoritative opening time is
// disabled, the opening is one second after the recorded death, and some frame after it is enabled.
function respawnGate(rows, survivor) {
  const dead = rows.filter(row => row.spectator?.Visible && row.phase06?.Views?.status?.Header.OpensAt != null);
  const corpse = dead[0]?.phase06.World.Corpses.find(entry => entry.SurvivorId === survivor);
  if (!corpse || !dead.length) return null;
  const opens = dead[0].phase06.Views.status.Header.OpensAt;
  const time = row => row.phase06.Views.status.Header.RootTime;
  const early = dead.filter(row => time(row) < opens);
  const late = dead.filter(row => time(row) >= opens);
  if (opens - corpse.At !== 1000 || !early.length || !late.some(row => row.spectator.CanRespawn)
    || early.some(row => row.spectator.CanRespawn)) return null;
  return {deathAt: corpse.At, opens, disabledFrames: early.length, enabledFrame: late.find(row =>
    row.spectator.CanRespawn).frame};
}

// UI-SPEC: a successor within a quarter of the 1280 px camera span from the held center gets a 0.25 s smooth
// handover; a greater displacement cuts to it. Judged from the committed frames that show the transfer notice.
function handover(rows, held, target) {
  const transfer = rows.filter(row => row.spectator?.TransferNotice?.startsWith('Control transferred'));
  if (!transfer.length || !Number.isFinite(held) || !Number.isFinite(target)) return null;
  const expected = Math.abs(target - held) <= 1280 / 4 ? 'smooth' : 'cut';
  const between = value => value > Math.min(held, target) && value < Math.max(held, target);
  const observed = transfer.some(row => row.spectator.Smoothing && between(row.spectator.Camera)) ? 'smooth'
    : !transfer[0].spectator.Smoothing && Math.abs(transfer[0].spectator.Camera - target) < 0.02 ? 'cut' : 'none';
  return {held, target, expected, observed};
}

module.exports = {open: context => run.open(context, roles), reset: run.reset, close: run.close, cases: native.tracked({
  'phase06.lan.population': population,
  'phase06.lan.conflicts': conflicts,
  'phase06.lan.succession': succession,
}), helpers: {rosterMatches, spread, cameraHold, respawnGate, handover, eligible, choicesMatch}};

