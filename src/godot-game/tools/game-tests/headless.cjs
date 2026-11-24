'use strict';

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');
const { launch, waitFor, frames, activate, actor, key, input, stop, cleanup,
  prepareProject } = require('../lan-checks/network.cjs');

function verifyOwner(row, peer, nonce) {
  assert.equal(row.nonce, nonce, 'OBSERVATION_NONCE');
  assert.equal(row.pid, peer.pid, 'OBSERVATION_PROCESS');
  assert.equal(row.provenance, 'read-only-state-observation', 'OBSERVATION_PROVENANCE');
}

function latest(peer, nonce, match) {
  const row = frames(peer).at(-1);
  if (!row) return null;
  verifyOwner(row, peer, nonce);
  return match(row) ? row : null;
}

const unsafe = 'outside:danger-near:stash-blocked';
const safe = 'inside:danger-clear:stash-reachable';
const ordinary = 'inside:danger-clear:stash-blocked';
const rightKey = 68;
const escapeKey = 4194305;
const instructions = 'Move: A / D or Left / Right arrows. Esc: Main menu.';
const fixtureInputs = {
  coordinateScale: '1000 subunits per world unit',
  origin: [0, 0], groundContact: 0, mainBuilding: [0, 0], refugeRange: 12000, dangerRange: 12000, stashRange: 4000,
  walls: [{ id: 'observation.wall', contact: [-6500, 0], completed: true, living: true }],
  blockers: [{ id: 'observation.barrier', minimum: [-6750, 1], maximum: [-6500, 3500] }],
  hostiles: [{ id: 'observation.hostile', contact: [-18501, 0] }],
  stash: { id: 'foundation.stash', contact: [-6000, 3000] },
};

function hasTransition(row, state) {
  return row.events?.some(event => event.startsWith('run-world:' + state + ':'));
}

function readySolo(row, peer) {
  const participant = row.snapshot?.Participants[0];
  return row.ready && row.nativeActors === 1 && row.playerId === peer.profile
    && row.snapshot?.Participants.length === 1 && participant?.Ready === true
    && row.bindingId === 'local' && participant.BindingId === row.bindingId
    && participant.Actor.PlayerId === peer.profile && participant.Actor.SessionId === row.snapshot.SessionId;
}

function verifyTransitions(peer, nonce, stopped) {
  const current = stopped.events.filter(event => event.startsWith('run-world:'));
  const rows = frames(peer);
  const transitions = current.map(event => {
    const [receipt, visibility, hint] = event.split('|');
    assert.equal(visibility, 'visible', 'RUN_WORLD_VISIBLE_NATIVE_HINT');
    const [, camp, danger, stash, elapsed, revision, position] = receipt.split(':');
    const frame = rows.find(row => row.snapshot?.Revision === Number(revision));
    assert.ok(frame?.ready && frame.events.includes(event), 'RUN_WORLD_POST_STEP_REVISION');
    verifyOwner(frame, peer, nonce);
    const owned = actor(frame, peer.profile);
    assert.equal(Number(position), owned.PositionSubunits, 'RUN_WORLD_CURRENT_POSITION');
    assert.equal(Number(elapsed), owned.ElapsedMilliseconds, 'RUN_WORLD_CURRENT_ELAPSED');
    return { state: [camp, danger, stash].join(':'), elapsed: Number(elapsed),
      revision: Number(revision), position: Number(position), frame: frame.frame, hint, visibility };
  });
  assert.deepEqual(transitions.map(transition => transition.state), [unsafe, safe], 'RUN_WORLD_TRANSITION_ORDER');
  const [before, after] = transitions;
  assert.equal(before.hint, instructions + '\nShared stash out of reach. Rest is unsafe with an enemy nearby.',
    'RUN_WORLD_UNSAFE_HINT');
  assert.equal(after.hint, instructions + '\nShared stash in reach. Safe to rest.', 'RUN_WORLD_SAFE_HINT');
  assert.equal(before.position, -8000, 'RUN_WORLD_SPAWN');
  assert.ok(after.position >= -6500 && after.position > before.position, 'RUN_WORLD_MOVED_INTO_ACCESS');
  assert.ok(after.elapsed > before.elapsed && after.revision > before.revision, 'RUN_WORLD_ELAPSED_ADVANCED');
  return transitions;
}

async function verifyFreshStart(peer, subject, deadline, awaitRow, seeded) {
  await key(peer, subject, deadline, rightKey, true);
  await awaitRow(row => row.direction === 1 && row.polledRight, 'right pressed before leaving the world');
  await key(peer, subject, deadline, escapeKey, true);
  const menu = await awaitRow(row => !row.ready && !row.snapshot && row.nativeActors === 0
    && row.direction === 0 && row.polledRight && row.lastInput?.key === escapeKey
    && row.lastInput.pressed === true, 'menu after Escape while right remained physically pressed');
  await key(peer, subject, deadline, escapeKey, false);
  await activate(peer, subject, deadline, 'Menu/Start');
  const started = await awaitRow(row => readySolo(row, peer) && hasTransition(row, ordinary),
    'fresh ordinary run without the owned geometry');
  assert.notEqual(started.snapshot.SessionId, seeded.snapshot.SessionId, 'RESTART_SESSION_REUSED');
  assert.notEqual(actor(started, peer.profile).ActorId, actor(seeded, peer.profile).ActorId, 'RESTART_ACTOR_REUSED');
  const checked = await awaitRow(row => row.frame >= started.frame + 2 && readySolo(row, peer),
    'fresh actor stays idle through further ticks');
  assert.ok(checked.polledRight, 'RESTART_PHYSICAL_KEY_STILL_HELD');
  assert.equal(checked.direction, 0, 'RESTART_CLEARED_HELD_INPUT');
  assert.equal(actor(checked, peer.profile).PositionSubunits, -8000, 'RESTART_FRESH_POSITION');
  const receipts = checked.events.filter(event => event.startsWith('run-world:'));
  assert.equal(receipts.length, 1, 'ORDINARY_START_SINGLE_WORLD_STATE');
  assert.equal(receipts[0].split('|').slice(1).join('|'),
    'visible|' + instructions + '\nShared stash out of reach. Safe to rest.', 'ORDINARY_START_HINT');
  await key(peer, subject, deadline, rightKey, false);
  return { menu, started, checked };
}

async function verifyActionInput(peer, subject, deadline, awaitRow) {
  const shift = 4194325;
  const space = 32;
  await key(peer, subject, deadline, shift, true);
  await key(peer, subject, deadline, rightKey, true);
  const sprint = await awaitRow(row => row.direction === 1 && row.runPose?.Movement === 'faster',
    'authored faster movement from ordinary Shift and D');
  await key(peer, subject, deadline, space, true);
  const jump = await awaitRow(row => row.runPose?.Movement === 'jump' && row.runPose.Height > 0,
    'ordinary Space reaches host jump and positive current height');
  await key(peer, subject, deadline, space, false);
  await key(peer, subject, deadline, rightKey, false);
  await key(peer, subject, deadline, shift, false);
  await awaitRow(row => row.runPose?.Height === 0 && row.direction === 0, 'returned to ground after release');

  const click = pressed => input(peer, subject, deadline, {action: 'click-world', x: 640, y: 512, pressed});
  await click(true);
  const attack = await awaitRow(row => row.runPose?.StrikeAt != null && row.runAction?.status === 'Accepted',
    'ordinary left click reaches one current melee action through the world GUI surface');
  assert.ok(Number.isFinite(attack.aim?.x) && attack.aim.y === 1750, 'NATIVE_ATTACK_AIM_COORDINATES');
  await click(true);
  const held = await awaitRow(row => row.frame > attack.frame + 2, 'repeated button remains physically held');
  assert.equal(held.runAction?.sequence, attack.runAction.sequence, 'HELD_ATTACK_IS_NOT_RESUBMITTED');
  await click(false);
  return { sprint, jump, attack, held };
}

async function verifyInventory(peer, subject, deadline, awaitRow) {
  const inventoryKey = 73;
  await key(peer, subject, deadline, inventoryKey, true);
  const open = await awaitRow(row => row.inventoryOpen && row.inventorySlots === 11,
    'ordinary I opens eleven native equipment slots');
  assert.ok(open.hud?.Health > 0 && open.hud.Stamina > 0 && open.hud.slots === 4,
    'COMMITTED_LIVE_HUD_REMOVES_NO_OWNED_PANEL');
  await key(peer, subject, deadline, inventoryKey, false);
  await input(peer, subject, deadline,
    {action: 'focus', control: 'InventoryWorkspace/InventoryPanels/equipment_torso-clothing'});
  const details = await awaitRow(row => row.inventoryOpen && row.inventoryDetails,
    'keyboard focus shows the equipped item details');
  await key(peer, subject, deadline, rightKey, true);
  const held = await awaitRow(row => row.inventoryOpen && row.polledRight
    && actor(row, peer.profile)?.ElapsedMilliseconds > actor(open, peer.profile).ElapsedMilliseconds + 30,
  'world time continues while inventory captures held movement');
  assert.equal(held.direction, 0, 'INVENTORY_BLOCKS_MOVEMENT');
  assert.equal(actor(held, peer.profile).PositionSubunits, actor(open, peer.profile).PositionSubunits,
    'INVENTORY_PRESERVES_ACTOR_POSITION');
  await key(peer, subject, deadline, escapeKey, true);
  await key(peer, subject, deadline, escapeKey, false);
  const closed = await awaitRow(row => row.ready && !row.inventoryOpen && !row.inventoryDetails
      && row.polledRight && row.direction === 0,
    'Escape dismisses inventory and keeps the old physical hold blocked');
  await key(peer, subject, deadline, rightKey, false);
  await activate(peer, subject, deadline, 'RunHud/Actions/OpenInventory');
  const button = await awaitRow(row => row.frame > closed.frame && row.inventoryOpen && row.hud?.slots === 4,
    'native Open inventory action reaches the same committed workspace');
  await key(peer, subject, deadline, escapeKey, true);
  await key(peer, subject, deadline, escapeKey, false);
  await awaitRow(row => row.frame > button.frame && !row.inventoryOpen, 'Escape closes the HUD-opened workspace');
  return {open, details, held, closed, button};
}

async function verifyNearby(peer, subject, deadline, awaitRow) {
  const interact = 69;
  const panelPath = 'InventoryWorkspace/InventoryPanels/';
  const tap = async code => {
    await key(peer, subject, deadline, code, true);
    await key(peer, subject, deadline, code, false);
  };
  await tap(interact);
  const open = await awaitRow(row => row.ready && row.inventoryOpen && row.inventorySlots === 11,
    'ordinary E opens reachable inventories without replacing carried equipment');
  if (open.nearby?.kind !== 'Stash') {
    await activate(peer, subject, deadline, panelPath + 'NearbyChoices/Rows/NearbyChoice_Stash');
  }
  const stash = await awaitRow(row => row.inventoryOpen && row.nearby?.kind === 'Stash' && row.nearby.page === 1,
    'native selection installs the actual authorized stash and initial page');
  assert.equal(stash.runAction?.status, 'Ready', 'NEARBY_AUTHORITY_ACCEPTED_TARGET');
  await activate(peer, subject, deadline, panelPath + 'NextStashPage');
  const next = await awaitRow(row => row.inventoryOpen && row.nearby?.page === 2,
    'native Next installs the current second stash page');
  await activate(peer, subject, deadline, panelPath + 'PreviousStashPage');
  const previous = await awaitRow(row => row.frame > next.frame && row.inventoryOpen && row.nearby?.page === 1,
    'native Previous uses the new grant and returns to the first page');
  await tap(escapeKey);
  const closed = await awaitRow(row => row.ready && !row.inventoryOpen && row.nearby?.page === 0,
    'Escape removes the authorized panel and its selected page');
  return {open, stash, next, previous, closed};
}

async function smoke(context, folder) {
  const nonce = crypto.randomUUID();
  const directory = path.join(folder, nonce);
  fs.mkdirSync(directory);
  fs.writeFileSync(path.join(directory, 'observation.owner'), nonce, { flag: 'wx' });
  const userdata = path.join(directory, 'userdata');
  fs.mkdirSync(userdata);
  const env = {
    ...context.env,
    KZ_RUN_WORLD_PROOF: nonce,
    APPDATA: userdata,
    LOCALAPPDATA: userdata,
    XDG_DATA_HOME: userdata,
    XDG_CONFIG_HOME: userdata,
    XDG_CACHE_HOME: userdata,
  };
  const subject = { ...context, env, directory, nonce, rendered: false, platform: process.platform + '-' + process.arch };
  subject.project = await prepareProject(subject);
  const deadline = Date.now() + 45_000;
  const peer = launch(subject, 'solo', 0);
  let failure;
  let before;
  let moved;
  let stopped;
  let worldTransitions;
  let steady;
  let fresh;
  let actions;
  let inventory;
  let nearby;
  let terminal;
  try {
    const awaitRow = (match, description) => waitFor([peer], deadline, () => latest(peer, nonce, match), description);
    await awaitRow(row => !row.ready && !row.snapshot && row.nativeActors === 0 && row.direction === 0,
      'ordinary main menu');
    await activate(peer, subject, deadline, 'Menu/Start');
    before = await awaitRow(row => readySolo(row, peer) && hasTransition(row, unsafe),
      'one owned solo actor with blocked stash and unsafe rest');
    await key(peer, subject, deadline, rightKey, true);
    const initialPosition = actor(before, peer.profile).PositionSubunits;
    moved = await awaitRow(row => row.direction === 1 && row.polledRight === true && row.inputEvents > before.inputEvents
      && actor(row, peer.profile)?.PositionSubunits > initialPosition && hasTransition(row, safe),
    'rightward production movement into stash reach and safe rest');
    assert.equal(actor(moved, peer.profile).ActorId, actor(before, peer.profile).ActorId, 'MOVED_ACTOR_ID');
    await key(peer, subject, deadline, rightKey, false);
    stopped = await awaitRow(row => row.frame > moved.frame && row.direction === 0 && row.polledRight === false
      && row.lastInput?.key === rightKey && row.lastInput.pressed === false && row.lastAcceptedIntent?.Direction === 0,
    'native release and accepted neutral movement');
    steady = await awaitRow(row => row.frame >= stopped.frame + 2 && readySolo(row, peer), 'stationary after release');
    assert.equal(actor(steady, peer.profile).PositionSubunits, actor(stopped, peer.profile).PositionSubunits,
      'RELEASE_STOPPED_MOVEMENT');
    worldTransitions = verifyTransitions(peer, nonce, stopped);
    nearby = await verifyNearby(peer, subject, deadline, awaitRow);
    fresh = await verifyFreshStart(peer, subject, deadline, awaitRow, before);
    actions = await verifyActionInput(peer, subject, deadline, awaitRow);
    inventory = await verifyInventory(peer, subject, deadline, awaitRow);
    await stop(peer, subject, deadline);
  } catch (error) {
    failure = error;
  } finally {
    try {
      terminal = await cleanup(peer);
    } catch (error) {
      failure ??= error;
    }
  }
  if (failure) throw failure;
  assert.equal(terminal.quitAck.action, 'ordinary-Quit', 'NATIVE_QUIT_ACKNOWLEDGED');
  assert.equal(terminal.quitAck.pid, peer.pid, 'NATIVE_QUIT_PROCESS');
  assert.equal(terminal.quitAck.nonce, nonce, 'NATIVE_QUIT_NONCE');
  assert.equal(terminal.exit.code, 0, 'NATIVE_QUIT_EXIT');
  assert.equal(terminal.cleanupKill, false, 'NATIVE_QUIT_WITHOUT_CLEANUP_KILL');
  assert.ok(peer.output.trim(), 'NATIVE_RUN_LOG_MISSING');
  assert.ok(!/(?:SCRIPT ERROR:|ERROR:|Unhandled exception|FATAL)/.test(peer.output), 'NATIVE_ERROR_LOG');
  fs.writeFileSync(path.join(directory, 'movement.json'), JSON.stringify({
    nonce, revision: context.revision, pid: peer.pid, before, moved, stopped, steady,
    fixtureInputs, worldBefore: worldTransitions[0], worldAfter: worldTransitions[1], fresh, actions, inventory, nearby,
    terminal,
    inputDriver: 'synthetic-native-ParseInputEvent',
  }, null, 2) + '\n', { flag: 'wx' });
}

module.exports = { smoke };
