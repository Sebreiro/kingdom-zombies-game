'use strict';

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');
const { launch, waitFor, frames, activate, actor, key, stop, cleanup } = require('../lan-checks/network.cjs');

function latest(peer, nonce, match) {
  const row = frames(peer).at(-1);
  if (!row) return null;
  assert.equal(row.nonce, nonce, 'OBSERVATION_NONCE');
  assert.equal(row.pid, peer.pid, 'OBSERVATION_PROCESS');
  assert.equal(row.provenance, 'read-only-state-observation', 'OBSERVATION_PROVENANCE');
  return match(row) ? row : null;
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
  try {
    const awaitRow = (match, description) => waitFor([peer], deadline, () => latest(peer, nonce, match), description);
    await awaitRow(row => !row.ready && !row.snapshot, 'ordinary main menu');
    await activate(peer, subject, deadline, 'Menu/Start');
    before = await awaitRow(row => row.ready && row.nativeActors === 1 && row.playerId === peer.profile
      && row.snapshot?.Participants.length === 1 && actor(row, peer.profile), 'one owned solo actor');
    await key(peer, subject, deadline, 68, true);
    const initialPosition = actor(before, peer.profile).PositionSubunits;
    moved = await awaitRow(row => row.direction === 1 && row.polledRight === true && row.inputEvents > before.inputEvents
      && actor(row, peer.profile)?.PositionSubunits > initialPosition, 'rightward production movement');
    assert.equal(actor(moved, peer.profile).ActorId, actor(before, peer.profile).ActorId, 'MOVED_ACTOR_ID');
    await key(peer, subject, deadline, 68, false);
    stopped = await awaitRow(row => row.frame > moved.frame && row.direction === 0 && row.polledRight === false,
      'released movement');
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
    nonce, revision: context.revision, pid: peer.pid, before, moved, stopped,
    inputDriver: 'synthetic-native-ParseInputEvent',
  }, null, 2) + '\n', { flag: 'wx' });
}

module.exports = { smoke };
