'use strict';

const assert = require('node:assert/strict');
const run = require('./phase06-rendered.cjs');
const {draws} = require('../../tools/lan-checks/rendered.cjs');
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

async function allPictures(state, label, condition) {
  return Promise.all(state.peers.map(peer => run.picture(state, peer, label + '-' + peer.role,
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
    && spread(draw) && rosterMatches(draw) && profilesBound(draw, state.peers) && run.settledView(draw));
  assert.equal(host, state.peers[0]);
  return run.finish(state, 'phase06.lan.population', state.peers, start, {identities, targets, movement}, pictures);
}

// Every receiver shows each player's own profile name on that player's participant.
function profilesBound(draw, peers) {
  return peers.every(peer => draw.snapshot.Participants.find(entry => entry.Actor.PlayerId === peer.profile)?.Label
    === run.profileNames[peer.role]);
}

async function withdraw(state, peer, point) {
  await run.press(state, peer, key.control, true);
  try { await run.click(state, peer, point); }
  finally { await run.press(state, peer, key.control, false); }
}

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
  const occupied = within('/NearbyStorage/', 'Item_' + source.Id);
  const losingPoint = await run.control(second, occupied);

  await run.focus(state, first);
  const winnerSequence = run.last(first).input.runAction?.sequence ?? 0;
  await withdraw(state, first, await run.control(first, occupied));
  const winner = await run.committed(first, row => row.runAction?.sequence > winnerSequence
    && row.runAction.status === 'Applied' && external(row)?.Rows.every(entry => entry.Id !== source.Id),
    'one authority withdrawal and corrected source');
  await run.focus(state, second);
  const before = run.last(second);
  await withdraw(state, second, losingPoint);
  const corrected = await run.committed(second, row => external(row)?.Rows.every(entry => entry.Id !== source.Id)
    && row.phase06.Views.status.Header.Hud.Money === 0, 'contested cell corrected without duplicate',
    before.drawFrame);
  const laterReceipts = draws(second).filter(draw => draw.drawFrame > before.drawFrame
    && draw.input.runAction?.sequence > (before.input.runAction?.sequence ?? 0))
    .map(draw => draw.input.runAction.status);
  assert(!laterReceipts.includes('Applied'), 'losing attempt must not apply');
  const observedStale = laterReceipts.some(status => status === 'AccessLost' || status === 'StaleItem');
  assert(observedStale || !before.controls.some(entry => occupied(entry) && entry.visible),
    'contested physical action lacked either visible correction or a denial');

  const still = actor(corrected.input).X;
  await run.tap(state, second, key.right);
  await run.committed(second, row => row.inventoryOpen && actor(row)?.X === still, 'panel captures movement');
  await run.choose(state, second, named('CloseInventory'));
  await run.moveUntil(state, second, key.left, row => !nearby(row).some(entry => entry.Kind === 'Stash'),
    'ordinary guest movement leaves stash reach');
  const denied = await run.picture(state, second, 'conflict-revoked', row => !external(row)
    && !nearby(row).some(entry => entry.Kind === 'Stash'));
  const final = await allPictures(state, 'conflict-current', (row, _, peer) => formed(row, session)
    && row.phase06.Views.status.Header.Hud.Money === (peer === first ? source.Quantity : 0)
    && (peer === host || peer === second || external(row)?.Rows.every(entry => entry.Id !== source.Id)));
  return run.finish(state, 'phase06.lan.conflicts', state.peers, start,
    {source, winner, before, corrected, observedStale, laterReceipts}, [...originals, ...final, denied]);
}

async function succession(state) {
  const start = state.input.records.length;
  const {host, guests, session, identities} = await startLan(state, 'life');
  const previous = identities[0];
  await run.focus(state, host);
  await run.fixture(state, host, {action: 'fixture-attack', Target: 'fixture.melee-a'});
  const death = await run.picture(state, host, 'succession-death', (row, draw) => row.spectator?.Visible
    && !own(row) && !row.hud && Number.isFinite(row.spectator.Camera) && !row.spectator.Followed
    && row.phase06.World.Corpses.some(entry => entry.SurvivorId === previous.actor && draw.appearance
      .some(art => art.id === entry.Id && art.kind === 'Corpses' && art.visible && !art.crown)));
  const initial = actor(death.draw.input, identities[1].actor).X;
  await run.focus(state, guests[0]);
  await run.moveUntil(state, guests[0], key.right, row => actor(row)?.X > initial + 1150,
    'living teammate separates from retired camera');
  const deathHold = await run.committed(host, (row, draw) => row.spectator?.Visible
    && !row.spectator.Followed && actor(row, identities[1].actor).X > initial + 1150
    && stableCamera(death.draw, draw), 'death camera remains fixed while a teammate travels', death.draw.drawFrame);
  await run.focus(state, host);
  const firstFollow = await run.choose(state, host, named('NextTeammate'));
  const next = await run.picture(state, host, 'succession-next', (row, draw) =>
    follows(row, draw, identities[1].actor) && Math.abs(draw.cameraX - deathHold.cameraX) > 10, firstFollow);
  const previousFollow = await run.choose(state, host, named('PreviousTeammate'));
  const prior = await run.picture(state, host, 'succession-previous', row => row.spectator?.Followed?.Survivor
    === identities[3].actor, previousFollow);
  const slot = next.draw.snapshot.Participants.find(row => row.BindingId === identities[2].binding).Slot;
  await run.choose(state, host, named('Teammate_' + slot));
  const chosen = await run.picture(state, host, 'succession-chosen', (row, draw) =>
    follows(row, draw, identities[2].actor) && row.spectator.CanRespawn);
  const targetX = actor(chosen.draw.input, identities[2].actor).X;
  await run.focus(state, guests[1]);
  await run.moveUntil(state, guests[1], key.right, row => actor(row)?.X > targetX + 1150, 'followed teammate travels');
  const tracking = await run.committed(host, (row, draw) => follows(row, draw, identities[2].actor)
    && Math.abs(draw.cameraX - chosen.draw.cameraX) > 10
    && actor(row, identities[2].actor).Movement === 'idle', 'actual camera tracks selected teammate',
  chosen.draw.drawFrame);
  await run.menu(state, guests[1]);
  const hold = await run.picture(state, host, 'succession-lost', (row, draw) => row.spectator?.Visible
    && !row.spectator.Followed && !!row.spectator.FollowNotice && stableCamera(tracking, draw), tracking.drawFrame);
  const other = actor(hold.draw.input, identities[3].actor).X;
  await run.focus(state, guests[2]);
  await run.moveUntil(state, guests[2], key.right, row => actor(row)?.X > other + 300, 'different teammate travels');
  const lostHold = await run.committed(host, (row, draw) => row.spectator?.FollowNotice
    && !row.spectator.Followed && actor(row, identities[3].actor)?.X > other + 300 && stableCamera(hold.draw, draw),
  'lost target camera holds across committed draws', hold.draw.drawFrame);
  await run.join(state, guests[1], session);
  await run.focus(state, host);
  await run.choose(state, host, named('Respawn'));
  const transfer = await run.picture(state, host, 'succession-transfer', row => formed(row, session)
    && row.phase06.epoch > 0 && row.phase06.control !== previous.actor
    && row.bindingId === previous.binding && row.playerId === previous.player && !!row.spectator?.TransferNotice);
  const successor = transfer.draw.input.phase06.control;
  assert(!identities.slice(1).some(entry => entry.actor === successor));
  const final = await allPictures(state, 'succession-current', (row, draw, peer) => formed(row, session)
    && actor(row, successor)?.BindingId === previous.binding
    && (peer === host || row.phase06.control === identities[roles.indexOf(peer.role)].actor)
    && draw.appearance.some(entry => entry.id === successor && entry.crown && entry.visible));
  return run.finish(state, 'phase06.lan.succession', state.peers, start,
    {identities, successor, deathHold, tracking, lostHold}, [death, next, prior, chosen, hold, transfer, ...final]);
}

module.exports = {open: context => run.open(context, roles), reset: run.reset, close: run.close, cases: {
  'phase06.lan.population': population,
  'phase06.lan.conflicts': conflicts,
  'phase06.lan.succession': succession,
}, helpers: {rosterMatches, spread}};

