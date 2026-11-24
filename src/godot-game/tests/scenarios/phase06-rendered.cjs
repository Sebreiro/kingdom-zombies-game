'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const {createHash} = require('node:crypto');
const render = require('../../tools/lan-checks/rendered.cjs');
const terminal = require('../../tools/lan-checks/terminal.cjs');
const {helpers: native} = require('./phase06-headless.cjs');

const method = 'physical-SendInput';
const key = {...native.key, escape: 27, shift: 16, control: 17, enter: 13, rotate: 82};
const last = peer => render.draws(peer).at(-1);
const named = name => entry => entry.path === name || entry.path.endsWith('/' + name);
const title = text => entry => entry.text === text && entry.path.includes('/Options/');
const within = (parent, name) => entry => entry.path.includes(parent) && named(name)(entry);
const actor = native.actor;
const own = native.own;
const external = native.external;
const item = native.item;

async function open(context, roles) {
  assert.equal(process.platform, 'win32', 'saved physical procedure needs Windows');
  const state = await native.open(context, true);
  try {
    for (const role of roles) await native.launch(state, role);
    await render.until(state.peers, () => state.peers.every(peer => last(peer)?.drawn), 'current native root draws');
    state.input = render.driver(state, state.peers);
    await state.input.start();
    const assemblies = '.godot/mono/temp/bin/Debug';
    state.subject = ['KingdomZombies.dll', 'KingdomZombies.Core.dll'].map(name => {
      const bundled = path.join(state.project, assemblies, name);
      const built = path.join(state.root, 'src/godot-game/game', assemblies, name);
      const digest = file => createHash('sha256').update(fs.readFileSync(file)).digest('hex');
      assert.equal(digest(bundled), digest(built), 'current imported production assembly');
      return {name, sha256: digest(bundled)};
    });
    return state;
  } catch (error) {
    try { await close(state); } catch (cleanup) { error.message += ' / ' + cleanup.message; }
    throw error;
  }
}

function after(peer, predicate, label, before = 0) {
  return render.until([peer], () => {
    const frame = last(peer);
    if (!frame?.drawn || frame.drawFrame <= before || !predicate(frame)) return null;
    assert.equal(frame.pid, peer.pid);
    assert.equal(frame.nonce, peer.nonce);
    assert.equal(frame.role, peer.role);
    assert.equal(frame.profile, peer.profile);
    return frame;
  }, peer.role + ' ' + label);
}

function committed(peer, predicate, label, before = 0) {
  return after(peer, frame => frame.input.phase06?.World && predicate(frame.input, frame), label, before);
}

function control(peer, selector) {
  return after(peer, frame => frame.controls.some(entry => selector(entry) && entry.visible && !entry.disabled),
    'visible native control').then(frame => {
    const matches = frame.controls.filter(entry => selector(entry) && entry.visible && !entry.disabled);
    assert.equal(matches.length, 1, matches.map(entry => entry.path).join(','));

    const [point] = matches;
    assert(point.width > 0 && point.height > 0, point.path);
    return point;
  });
}

async function focus(state, peer) {
  const ack = await state.input.send(peer, {action: 'focus', method});
  assert.equal(ack.focused, true);
  await after(peer, frame => frame.focused, 'owned foreground');
}

function send(state, peer, request) {
  return state.input.send(peer, {method, ...request});
}

function press(state, peer, code, pressed) {
  return render.nativeKey(state, state.input, peer, code, pressed);
}

async function tap(state, peer, code) {
  await press(state, peer, code, true);
  await press(state, peer, code, false);
}

function pointer(state, peer, point, button, pressed) {
  return send(state, peer, {action: 'pointer', x: Math.round(point.x), y: Math.round(point.y), button, pressed});
}

async function click(state, peer, point, button = 'left') {
  const before = last(peer).drawFrame;
  if (button === 'left') await send(state, peer, {action: 'click', x: Math.round(point.x), y: Math.round(point.y)});
  else {
    await pointer(state, peer, point, button, true);
    await pointer(state, peer, point, button, false);
  }
  return before;
}

async function choose(state, peer, selector, button) {
  return click(state, peer, await control(peer, selector), button);
}

async function moveUntil(state, peer, code, predicate, label) {
  const before = last(peer).drawFrame;
  await press(state, peer, code, true);
  try { return await committed(peer, predicate, label, before); }
  finally { await press(state, peer, code, false); }
}

async function enter(state, peer, mode = null) {
  await focus(state, peer);
  native.seed(peer, mode);
  const before = await choose(state, peer, named(peer.role === 'host' ? 'Menu/Host' : 'Menu/Start'));
  const current = await committed(peer, input => input.ready && input.phase06.FixtureName === mode,
    'fresh normal Start and current binding', before);
  if (peer.previousSession) assert.notEqual(current.snapshot.SessionId, peer.previousSession, 'fresh session identity');
  assert(!fs.existsSync(path.join(peer.directory, 'fixture.request')));
  peer.previousSession = current.snapshot.SessionId;
  return current;
}

async function join(state, peer, session) {
  await focus(state, peer);
  const before = await choose(state, peer, named('Menu/Join'));
  await after(peer, draw => draw.controls.some(entry => entry.path === 'Browser/Address' && entry.visible),
    'ordinary join browser', before);
  const address = '127.0.0.1:' + state.port;
  await choose(state, peer, named('Browser/Address'));
  await press(state, peer, key.control, true);
  try { await tap(state, peer, 65); }
  finally { await press(state, peer, key.control, false); }
  await send(state, peer, {action: 'text', text: address});
  await after(peer, draw => draw.controls.some(entry => entry.path === 'Browser/Address' && entry.text === address),
    'native address field received text');
  await choose(state, peer, named('Browser/JoinAddress'));
  return committed(peer, row => row.ready && row.snapshot.SessionId === session && own(row)?.Rows.length > 0,
    'current recipient admission');
}

async function menu(state, peer) {
  await focus(state, peer);
  for (let attempt = 0; attempt < 20; attempt++) {
    if (last(peer)?.controls.some(entry => entry.path === 'Menu/Start' && entry.visible)) return;
    await tap(state, peer, key.escape);
    await after(peer, frame => frame.drawn, 'Escape redraw');
  }
  throw new Error(peer.role + ' did not return to ordinary menu');
}

async function reset(state) {
  for (const peer of [...state.peers].reverse()) await menu(state, peer);
  assert(state.peers.every(peer => !last(peer)?.input.phase06));
}

async function close(state) {
  let failure;
  try {
    await native.close(state, async peer => {
      if (!state.input) throw new Error('Owned input driver could not start');
      await menu(state, peer);
      await control(peer, named('Menu/Quit'));
      terminal.requestQuit(peer);
      await choose(state, peer, named('Menu/Quit'));
    });
  } catch (error) { failure = error; }
  finally {
    try { await state.input?.close(); } catch (error) { failure ??= error; }
    await require('../../tools/lan-checks/png-worker.cjs').close();
  }
  if (failure) throw failure;
  native.ended(state);
}

async function picture(state, peer, label, predicate, minimumFrame = 0) {
  const current = await committed(peer, predicate, label, minimumFrame);
  const {state: draw, image} = await render.capture(state, peer, label);
  assert(draw.drawFrame >= current.drawFrame && draw.drawFrame > minimumFrame, label + ' stale draw');
  assert.equal(draw.role, peer.role);
  assert.equal(draw.profile, peer.profile);
  assert.equal(draw.snapshot.SessionId, current.snapshot.SessionId);
  assert.equal(draw.bindingId, current.bindingId);
  assert(draw.input.phase06.Revision >= current.input.phase06.Revision && predicate(draw.input, draw),
    label + ' changed before its image');
  assert.equal(draw.width, 1280);
  assert.equal(draw.height, 720);
  assert.match(image.sha256, /^[\da-f]{64}$/);
  return {draw, image};
}

function finish(state, id, peers, start, observations, pictures) {
  const interactions = state.input.records.slice(start);
  assert(interactions.some(row => row.method === method && ['key', 'click', 'pointer', 'text'].includes(row.action)),
    id + ' has no physical input');
  for (const peer of peers) {
    assert(pictures.some(({draw}) => draw.pid === peer.pid), peer.role + ' has no current image');
    assert(interactions.some(row => row.pid === peer.pid && row.method === method
      && ['key', 'click', 'pointer', 'text'].includes(row.action)), peer.role + ' has no physical input');
  }
  const files = pictures.flatMap(({image}) => [image.path, image.statePath]);
  return native.evidence(state, id, peers, {subject: state.subject, ...observations,
    inputs: interactions, images: pictures.map(({image}) => image)}, [...files, state.input.file]);
}

async function stash(state, peer) {
  if (last(peer).input.phase06.Views.external?.Header.Kind === 'Stash') return last(peer);
  await tap(state, peer, key.interact);
  const show = await after(peer, draw => draw.input.nearby?.kind === 'Stash'
    || draw.controls.some(entry => named('NearbyChoice_Stash')(entry) && entry.visible), 'nearby stash selector');
  if (show.input.nearby.kind !== 'Stash') await choose(state, peer, named('NearbyChoice_Stash'));
  return committed(peer, row => external(row)?.Header.Kind === 'Stash' && row.nearby?.kind === 'Stash',
    'native and authoritative stash view');
}

async function bagWindow(state, peer, selector, id, scope) {
  await choose(state, peer, selector, 'right');
  await choose(state, peer, title('Open'));
  return committed(peer, (row, draw) => Object.entries(row.phase06.Views)
    .some(([name, view]) => name.startsWith(scope + '.') && view.Header.ContainerId === id)
      && draw.controls.some(entry => within('/Bag_' + id + '/', 'MoveBag')(entry) && entry.visible),
    'physically opened bag window');
}

async function drag(state, peer, from, to) {
  await pointer(state, peer, from, 'left', true);
  try {
    await pointer(state, peer, {x: from.x + (to.x > from.x ? 8 : -8), y: from.y});
    await pointer(state, peer, to);
  } finally { await pointer(state, peer, to, 'left', false); }
}

module.exports = {open, close, reset, enter, join, menu, key, last, after, committed, focus, send, press, tap,
  pointer, click, choose, control, named, title, within, actor, own, item, external, moveUntil, picture, finish,
  stash, bagWindow, drag, fixture: native.inject, privacy: native.privacy};

