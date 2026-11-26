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
const nearby = native.nearby;
const external = native.external;
const item = native.item;
// Copy the workspace shows while an authorized view is still being installed.
const pendingCopy = new Set(['Opening inventory…', 'Opening stash page…']);
// Every observed run text (FoundationDrawObserver): the HUD, workspace, spectator card and transfer notice layers,
// the world help and each roster row.
const textLayers = /^(?:(?:RunHud|InventoryWorkspace|SpectatorCard|ControlTransferred)(?:\/|$)|Hint$|Roster\/[^/]+$)/;
// FoundationInventoryWorkspace.PointerMove starts a drag once the pointer is this far from the press.
const dragStart = 4;

// A picture may show only a settled view: no pending view copy, every observed run text (textLayers) unclipped,
// and, while the inventory is open, an installed grid for every carried storage item with each of its items drawn.
function settledView(draw) {
  const shown = (draw?.controls ?? []).filter(entry => entry.visible);
  if (shown.some(entry => pendingCopy.has(entry.text))) return false;
  if (shown.some(entry => textLayers.test(entry.path) && entry.clipped !== false)) return false;
  const row = draw?.input;
  if (!row?.inventoryOpen) return true;
  const views = row.phase06?.Views;
  if (!views?.own) return false;
  const grids = Object.entries(views).filter(([scope]) => scope.startsWith('own.')).map(([, view]) => view);
  return views.own.Rows.filter(entry => entry.StorageWidth > 0).every(storage => {
    const grid = grids.find(view => view.Header?.ContainerId === storage.Id);
    return !!grid && grid.Rows.every(content => shown.some(entry => entry.path.includes('/CarriedStorage/')
      && entry.path.endsWith('/Item_' + content.Id)));
  });
}

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

// The release is always delivered, so a press whose receipt failed cannot leave the key held and turn the next
// press of the same key into an echo.
async function tap(state, peer, code) {
  let failure;
  try { await press(state, peer, code, true); } catch (error) { failure = error; }
  try { await press(state, peer, code, false); } catch (error) { failure ??= error; }
  if (failure) throw failure;
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

const shown = (draw, pattern) => draw.controls.filter(entry => entry.visible && pattern.test(entry.path)).length;
const menuShown = draw => draw.controls.some(entry => entry.path === 'Menu/Start' && entry.visible);
const dragging = /\/(?:PlacementPreview|QuantityToMove)$/;
const browsing = draw => draw.input.inventoryOpen === true;

// D-14: Escape cancels an active drag and leaves browsing open, then closes the active inventory window, and only
// then reaches the in-run Escape action that returns to the menu. The workspace closes its item menu, nearby target
// list and bag windows, in that order, before the workspace itself.
const escapeOrder = [
  {step: 'cancel drag or split preview', active: draw => browsing(draw) && shown(draw, dragging) > 0,
    done: draw => browsing(draw) && shown(draw, dragging) === 0},
  {step: 'close item menu', active: draw => browsing(draw) && shown(draw, /\/ItemMenu\//) > 0,
    done: draw => browsing(draw) && shown(draw, /\/ItemMenu\//) === 0},
  {step: 'leave nearby target list', active: draw => browsing(draw) && !!draw.input.phase06?.Views?.external
      && shown(draw, /\/NearbyChoice_[^/]+$/) > 0,
    done: draw => browsing(draw) && shown(draw, /\/NearbyChoice_[^/]+$/) === 0},
  {step: 'close bag window', active: draw => browsing(draw) && shown(draw, /\/CloseBag_[^/]+$/) > 0,
    done: (draw, before) => browsing(draw) && shown(draw, /\/CloseBag_[^/]+$/) < shown(before, /\/CloseBag_[^/]+$/)},
  {step: 'close inventory window', active: browsing, done: draw => !browsing(draw)},
  {step: 'return to ordinary menu', active: () => true, done: menuShown},
];

// Each Escape must be acknowledged as a native key and then show the effect its D-14 step names. A missing effect
// fails with that step; there is no silent retry. Returns the steps taken.
async function menu(state, peer) {
  await focus(state, peer);
  const steps = [];
  for (let count = 0; count < 16; count++) {
    const before = last(peer);
    if (menuShown(before)) return steps;
    const {step, done} = escapeOrder.find(entry => entry.active(before));
    await tap(state, peer, key.escape);
    try { await after(peer, draw => done(draw, before), 'Escape effect ' + step, before.drawFrame); }
    catch (error) {
      throw new Error(peer.role + ' Escape acknowledged without its D-14 effect "' + step + '": ' + error.message);
    }
    steps.push(step);
  }
  throw new Error(peer.role + ' did not reach the ordinary menu through the D-14 Escape order');
}

async function reset(state) {
  state.interleaved = [];
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

// One drawn frame separates the press, the start-threshold crossing and the destination, so the game handles the
// press before any motion. held(before) runs while the button is still down; its result is returned.
async function drag(state, peer, from, to, held = async () => null) {
  const before = last(peer).drawFrame;
  await pointer(state, peer, from, 'left', true);
  try {
    const pressed = await after(peer, () => true, 'drawn after native press', before + 1);
    await pointer(state, peer, {x: from.x + (to.x >= from.x ? 2 : -2) * dragStart, y: from.y});
    await after(peer, () => true, 'drawn after drag start threshold', pressed.drawFrame);
    await pointer(state, peer, to);
    return await held(before);
  } finally { await pointer(state, peer, to, 'left', false); }
}

module.exports = {open, close, reset, enter, join, menu, key, last, after, committed, focus, send, press, tap,
  pointer, click, choose, control, named, title, within, actor, own, nearby, item, external, moveUntil, picture, finish,
  stash, bagWindow, drag, settledView, fixture: native.inject, privacy: native.privacy,
  motionInterval: native.motionInterval, profileNames: native.profileNames};

