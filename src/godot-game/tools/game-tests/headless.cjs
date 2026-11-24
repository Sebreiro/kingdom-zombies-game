'use strict';

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');
const { launch, waitFor, frames, activate, actor, key, stop, cleanup } = require('../lan-checks/network.cjs');

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

function hasTransition(row, state) {
  return row.events?.some(event => event.startsWith('run-world:' + state + ':'));
}

function verifyTransitions(peer, nonce, stopped) {
  const current = stopped.events.filter(event => event.startsWith('run-world:'));
  const rows = frames(peer);
  const transitions = current.map(event => {
    const [receipt, hint] = event.split('|');
    const [, camp, danger, stash, elapsed, revision, position] = receipt.split(':');
    const frame = rows.find(row => row.snapshot?.Revision === Number(revision));
    assert.ok(frame?.ready && frame.events.includes(event), 'RUN_WORLD_POST_STEP_REVISION');
    verifyOwner(frame, peer, nonce);
    const owned = actor(frame, peer.profile);
    assert.equal(Number(position), owned.PositionSubunits, 'RUN_WORLD_CURRENT_POSITION');
    assert.equal(Number(elapsed), owned.ElapsedMilliseconds, 'RUN_WORLD_CURRENT_ELAPSED');
    return { state: [camp, danger, stash].join(':'), elapsed: Number(elapsed),
      revision: Number(revision), position: Number(position), frame: frame.frame, hint };
  });
  assert.deepEqual(transitions.map(transition => transition.state), [unsafe, safe], 'RUN_WORLD_TRANSITION_ORDER');
  const [before, after] = transitions;
  const instructions = 'Move: A / D or Left / Right arrows. Esc: Main menu.';
  assert.equal(before.hint, instructions + '\nShared stash out of reach. Rest is unsafe with an enemy nearby.',
    'RUN_WORLD_UNSAFE_HINT');
  assert.equal(after.hint, instructions + '\nShared stash in reach. Safe to rest.', 'RUN_WORLD_SAFE_HINT');
  assert.equal(before.position, -8000, 'RUN_WORLD_SPAWN');
  assert.ok(after.position >= -6500 && after.position > before.position, 'RUN_WORLD_MOVED_INTO_ACCESS');
  assert.ok(after.elapsed > before.elapsed && after.revision > before.revision, 'RUN_WORLD_ELAPSED_ADVANCED');
  return transitions;
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
  const deadline = Date.now() + 45_000;
  const peer = launch(subject, 'solo', 0);
  let failure;
  let before;
  let moved;
  let stopped;
  let worldTransitions;
  try {
    const awaitRow = (match, description) => waitFor([peer], deadline, () => latest(peer, nonce, match), description);
    await awaitRow(row => !row.ready && !row.snapshot, 'ordinary main menu');
    await activate(peer, subject, deadline, 'Menu/Start');
    before = await awaitRow(row => row.ready && row.nativeActors === 1 && row.playerId === peer.profile
      && row.snapshot?.Participants.length === 1 && actor(row, peer.profile)
      && hasTransition(row, unsafe), 'one owned solo actor with blocked stash and unsafe rest');
    await key(peer, subject, deadline, 68, true);
    const initialPosition = actor(before, peer.profile).PositionSubunits;
    moved = await awaitRow(row => row.direction === 1 && row.polledRight === true && row.inputEvents > before.inputEvents
      && actor(row, peer.profile)?.PositionSubunits > initialPosition && hasTransition(row, safe),
    'rightward production movement into stash reach and safe rest');
    assert.equal(actor(moved, peer.profile).ActorId, actor(before, peer.profile).ActorId, 'MOVED_ACTOR_ID');
    await key(peer, subject, deadline, 68, false);
    stopped = await awaitRow(row => row.frame > moved.frame && row.direction === 0 && row.polledRight === false,
      'released movement');
    worldTransitions = verifyTransitions(peer, nonce, stopped);
    await stop(peer, subject, deadline);
  } catch (error) {
    failure = error;
  } finally {
    try {
      await cleanup(peer);
    } catch (error) {
      failure ??= error;
    }
  }
  if (failure) throw failure;
  assert.ok(peer.output.trim(), 'NATIVE_RUN_LOG_MISSING');
  assert.ok(!/(?:SCRIPT ERROR:|ERROR:|Unhandled exception|FATAL)/.test(peer.output), 'NATIVE_ERROR_LOG');
  fs.writeFileSync(path.join(directory, 'movement.json'), JSON.stringify({
    nonce, revision: context.revision, pid: peer.pid, before, moved, stopped, worldTransitions,
    inputDriver: 'synthetic-native-ParseInputEvent',
  }, null, 2) + '\n', { flag: 'wx' });
}

module.exports = { smoke };
