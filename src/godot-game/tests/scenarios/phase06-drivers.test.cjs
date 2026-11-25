'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const Module = require('node:module');
const network = require('../../tools/lan-checks/network.cjs');
const terminal = require('../../tools/lan-checks/terminal.cjs');
const run = require('./phase06-rendered.cjs');
const {helpers: native} = require('./phase06-headless.cjs');

const nonce = '00000000-0000-4000-8000-000000000621';
const soloFile = path.join(__dirname, 'phase06-solo.cjs');

function scratch(t) {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'kz-phase06-drivers-'));
  t.after(() => fs.rmSync(directory, {recursive: true, force: true}));
  return directory;
}

// A launched peer owns the files written at launch; terminal.json and console.log come from its cleanup.
function fakePeer(directory, role, pid) {
  const peer = {role, pid, profile: 'profile-' + role, nonce, directory: path.join(directory, role), output: '',
    sequence: 0, frame: 0, budget: () => 400};
  fs.mkdirSync(peer.directory);
  for (const name of ['launch.json', 'frames.ndjson']) fs.writeFileSync(path.join(peer.directory, name), '{}\n');
  return peer;
}

function writeDraw(peer, input, controls) {
  peer.frame += 1;
  const row = {nonce, pid: peer.pid, role: peer.role, profile: peer.profile, drawn: true, drawFrame: peer.frame,
    focused: true, input, controls};
  fs.appendFileSync(path.join(peer.directory, 'draws.ndjson'), JSON.stringify(row) + '\n');
  return row;
}

function control(pathName, text = '', extra = {}) {
  return {path: pathName, text, x: 640, y: 360, width: 120, height: 40, visible: true, disabled: false,
    clipped: false, ...extra};
}

const menuControls = () => [control('Menu/Start', 'Start'), control('Menu/Quit', 'Quit')];

function fakeInput(directory, react = () => {}) {
  const records = [];
  const file = path.join(directory, 'native-input-1.json');
  return {records, file,
    async send(peer, request) {
      const row = {...request, ok: true, pid: peer.pid, focused: true};
      records.push(row);
      react(peer, request);
      return row;
    },
    async close() { fs.writeFileSync(file, JSON.stringify({records}), {flag: 'wx'}); }};
}

// Mirrors terminal.cleanup: it writes the terminal record and console log, then validates the ordinary Quit.
function fakeCleanup(code) {
  return async peer => {
    peer.exit = {code: code(peer), signal: null, observedAt: String(process.hrtime.bigint())};
    const record = {nonce: peer.nonce, pid: peer.pid, role: peer.role, exit: peer.exit, streamsClosed: true,
      quitRequested: peer.quitRequested, quitAck: {action: 'ordinary-Quit', pid: peer.pid, nonce: peer.nonce,
        role: peer.role}, intentionalKill: false, cleanupKill: false};
    fs.writeFileSync(path.join(peer.directory, 'console.log'), peer.output);
    fs.writeFileSync(path.join(peer.directory, 'terminal.json'), JSON.stringify(record, null, 2), {flag: 'wx'});
    return terminal.validate(record);
  };
}

function renderedGroup(t, exitCode = () => 0) {
  const directory = scratch(t);
  const peers = [fakePeer(directory, 'host', 4101), fakePeer(directory, 'guest-1', 4102)];
  for (const peer of peers) writeDraw(peer, {inputEvents: 0}, menuControls());
  const state = {directory, nonce, revision: 'test', rendered: true, peers, input: fakeInput(directory)};
  const original = network.cleanup;
  network.cleanup = fakeCleanup(exitCode);
  t.after(() => { network.cleanup = original; });
  return state;
}

const cleanupRecord = state => path.join(state.directory, 'ordinary-cleanup.json');

test('Evidence_RenderedClose_ListsOnlyWrittenFiles', async t => {
  await t.test('successful rendered close writes every listed file', async t => {
    const state = renderedGroup(t);
    const listed = native.evidence(state, 'phase06.test.case', state.peers, {probe: true}, [state.input.file]);
    assert(listed.includes(cleanupRecord(state)), 'every case cites the ordinary cleanup record');
    await run.close(state);
    for (const file of listed) assert(fs.existsSync(file), 'listed evidence exists after close: ' + file);
    const record = JSON.parse(fs.readFileSync(cleanupRecord(state), 'utf8'));
    assert.deepEqual(record, {nonce, roles: [{role: 'host', pid: 4101}, {role: 'guest-1', pid: 4102}]});
  });

  await t.test('a role without an ordinary exit leaves the record absent', async t => {
    const state = renderedGroup(t, peer => peer.role === 'host' ? 1 : 0);
    await assert.rejects(run.close(state), /TERMINAL_NORMAL_QUIT host/);
    assert(!fs.existsSync(cleanupRecord(state)), 'failed close must not write the ordinary cleanup record');
  });

  await t.test('a failed ordinary Quit step leaves the record absent', async t => {
    const state = renderedGroup(t);
    writeDraw(state.peers[1], {inputEvents: 0}, [control('Menu/Start', 'Start')]);
    await assert.rejects(run.close(state), /RENDERED_DEADLINE|Menu\/Quit|visible native control/);
    assert(!fs.existsSync(cleanupRecord(state)), 'failed close must not write the ordinary cleanup record');
  });

  await t.test('the writer itself refuses a terminal record without exit 0', async t => {
    const state = renderedGroup(t, () => 3);
    for (const peer of state.peers) {
      terminal.requestQuit(peer);
      await assert.rejects(network.cleanup(peer), /TERMINAL_NORMAL_QUIT/);
    }
    assert.throws(() => native.ended(state), /TERMINAL_NORMAL_QUIT/);
    assert(!fs.existsSync(cleanupRecord(state)));
  });

  await t.test('headless close writes the same record after ordinary Quit', async t => {
    const state = renderedGroup(t);
    state.rendered = false;
    await native.close(state, async peer => terminal.requestQuit(peer));
    assert.deepEqual(JSON.parse(fs.readFileSync(cleanupRecord(state), 'utf8')).roles.map(row => row.pid), [4101, 4102]);
  });
});

function stripped(source) {
  return source.replace(/^\s*\/\/.*$/gm, '')
    .replace(/'(?:\\.|[^'\\\n])*'|"(?:\\.|[^"\\\n])*"|`(?:\\.|[^`\\])*`/g, "''")
    .replace(/\/(?![*/])(?:\\.|[^/\\\n])+\/[gimsuy]*(?=\s*[.,);])/g, '/x/');
}

function declaredNames(source) {
  const names = new Set();
  const add = text => text.split(',').map(part => part.split(':').pop().split('=')[0].replace(/[{}[\]().]/g, '').trim())
    .filter(Boolean).forEach(name => names.add(name));
  for (const [, name] of source.matchAll(/\b(?:function\s*\*?\s*|(?:const|let|var)\s+)([A-Za-z_$][\w$]*)/g)) names.add(name);
  for (const [, list] of source.matchAll(/\b(?:const|let|var)\s*\{([^}]*)\}\s*=/g)) add(list);
  for (const [, list] of source.matchAll(/\blet\s+([\w$\s,]+);/g)) add(list);
  for (const [, name] of source.matchAll(/,\s*([A-Za-z_$][\w$]*)\s*=(?![=>])/g)) names.add(name);
  for (const [, list] of source.matchAll(/function\s*[\w$]*\s*\(([^)]*)\)/g)) add(list);
  for (const [, list] of source.matchAll(/\(([^()]*)\)\s*=>/g)) add(list);
  for (const [, name] of source.matchAll(/([A-Za-z_$][\w$]*)\s*=>/g)) names.add(name);
  for (const [, name] of source.matchAll(/catch\s*\(\s*([\w$]+)\s*\)/g)) names.add(name);
  return names;
}

const keywords = new Set(['if', 'for', 'while', 'switch', 'catch', 'function', 'return', 'typeof', 'await', 'async',
  'new', 'super', 'import', 'delete', 'void', 'in', 'of', 'do', 'else', 'try', 'finally', 'throw', 'case']);

test('SoloDriver_AllCases_ReferenceDefinedHelpers', async t => {
  const source = stripped(fs.readFileSync(soloFile, 'utf8'));
  const exported = new Set(Object.keys(run));

  await t.test('every free call names a declared helper', () => {
    const declared = declaredNames(source);
    const called = [...source.matchAll(/(?<![\w$.])([A-Za-z_$][\w$]*)\s*\(/g)].map(match => match[1])
      .filter(name => !keywords.has(name));
    const undefinedCalls = [...new Set(called)]
      .filter(name => !declared.has(name) && !(name in globalThis) && name !== 'require');
    assert.deepEqual(undefinedCalls, [], 'phase06-solo.cjs calls undefined helpers');
  });

  await t.test('every run member and destructured helper is a rendered export', () => {
    const members = [...source.matchAll(/(?<![\w$.])run\.([A-Za-z_$][\w$]*)/g)].map(match => match[1]);
    const [, list] = source.match(/const\s*\{([^}]*)\}\s*=\s*run\s*;/);
    const unpacked = list.split(',').map(name => name.trim()).filter(Boolean);
    assert.deepEqual([...new Set([...members, ...unpacked])].filter(name => !exported.has(name)), []);
    assert(members.includes('moveUntil'), 'the world walk uses the shared native-input helper');
  });

  await t.test('every case reaches a defined helper and stops at the first wait', async () => {
    const calls = [];
    const stop = new Error('STUB_WAIT');
    const pure = new Set(['named', 'title', 'within', 'actor', 'own', 'nearby', 'external', 'item', 'motionInterval']);
    const stub = Object.fromEntries([...exported].map(name => [name, typeof run[name] !== 'function' ? run[name]
      : (...args) => {
        calls.push(name);
        return pure.has(name) ? run[name](...args) : Promise.reject(stop);
      }]));
    const solo = loadWith(soloFile, {'./phase06-rendered.cjs': stub});
    assert.deepEqual(Object.keys(solo.cases).sort(), ['phase06.solo.combat', 'phase06.solo.inventory',
      'phase06.solo.life', 'phase06.solo.movement', 'phase06.solo.stash', 'phase06.solo.world']);
    for (const [id, invoke] of Object.entries(solo.cases)) {
      calls.length = 0;
      await assert.rejects(invoke({peers: [{role: 'solo'}], input: {records: []}}), error => error === stop, id);
      assert(calls.length > 0 && calls.every(name => exported.has(name)), id + ' ' + calls.join(','));
    }
  });
});

// Shapes recorded in run-1 solo draws 914 (three carried grids still loading) and 927 (installed boards).
const hud = () => [control('RunHud/Health/Value', 'Health 100 / 100'), control('RunHud/CarriedLoad',
  'Load 8.02 kg · Normal'), control('RunHud/Actions/Slot1/Value', 'Empty'), control(
  'InventoryWorkspace/InventoryPanels/CloseInventory', 'Close inventory')];
const carried = name => 'InventoryWorkspace/InventoryPanels/CarriedStorage/Rows/' + name;
const equipped = [{Id: 1, DefinitionId: 'IT-11', StorageWidth: 2}, {Id: 2, DefinitionId: 'IT-12', StorageWidth: 2},
  {Id: 3, DefinitionId: 'IT-17', StorageWidth: null}, {Id: 58, DefinitionId: 'IT-23', StorageWidth: 4}];
const grid = (id, rows) => ({Header: {ContainerId: id}, Rows: rows.map(Id => ({Id}))});

function pendingDraw() {
  return {drawFrame: 914, input: {inventoryOpen: true, inventorySlots: 11, phase06: {Views: {own: {Rows: equipped},
    'own.2': grid(2, []), 'own.1': grid(1, []), 'own.3': grid(58, [63, 65, 66])}}},
  controls: [...hud(), control(carried('@Label@56'), 'Basic shirt'), control(carried('@Label@57'), 'Opening inventory…'),
    control(carried('@Label@58'), 'Trousers'), control(carried('@Label@59'), 'Opening inventory…'),
    control(carried('@Label@60'), 'Small backpack'), control(carried('@Label@61'), 'Opening inventory…')]};
}

function installedDraw() {
  return {drawFrame: 927, input: {inventoryOpen: true, inventorySlots: 11, phase06: {Views: {own: {Rows: equipped},
    'own.2': grid(63, [64]), 'own.1': grid(58, [63, 65, 66]), 'own.4': grid(2, []), 'own.3': grid(1, [])}}},
  controls: [...hud(), control(carried('@Label@101'), 'Basic shirt'), control(carried('ItemCells')),
    control(carried('@Label@102'), 'Trousers'), control(carried('@Label@104'), 'Small backpack'),
    ...[63, 65, 66].map(id => control(carried('@Control@105/Item_' + id))),
    control(carried('@Control@105/Item_65/@Label@106'), '9')]};
}

test('SettledView_PendingOrClipped_Rejected', async t => {
  await t.test('run-1 draw 914 with pending carried grids is rejected', () => {
    assert.equal(run.settledView(pendingDraw()), false);
  });

  await t.test('draw 927 with a clipped HUD or workspace text is rejected', () => {
    for (const target of ['RunHud/CarriedLoad', carried('@Label@104')]) {
      const draw = installedDraw();
      draw.controls.find(entry => entry.path === target).clipped = true;
      assert.equal(run.settledView(draw), false, target);
    }
  });

  await t.test('draw 927 with installed boards and unclipped text is accepted', () => {
    assert.equal(run.settledView(installedDraw()), true);
  });

  await t.test('an unreported clip state, a missing grid or an undrawn item is rejected', () => {
    const unreported = installedDraw();
    delete unreported.controls[0].clipped;
    const missing = installedDraw();
    delete missing.input.phase06.Views['own.4'];
    const undrawn = installedDraw();
    undrawn.controls = undrawn.controls.filter(entry => !entry.path.endsWith('/Item_66'));
    for (const draw of [unreported, missing, undrawn]) assert.equal(run.settledView(draw), false);
  });

  await t.test('a closed inventory needs only settled, unclipped text', () => {
    const world = {input: {inventoryOpen: false}, controls: hud()};
    assert.equal(run.settledView(world), true);
    world.controls.push(control('InventoryWorkspace/Feedback', 'Opening stash page…'));
    assert.equal(run.settledView(world), false);
  });

  await t.test('every solo picture uses the settled predicate', () => {
    const source = fs.readFileSync(soloFile, 'utf8');
    assert.equal(source.match(/run\.picture\(/g).length, 1, 'only the settled wrapper calls run.picture');
    assert.match(source, /const shot = [^;]*run\.picture\([^;]*run\.settledView\(draw\) && predicate\(row, draw\)/s);
    assert(source.match(/await shot\(/g).length >= 20, 'solo images go through the settled wrapper');
  });
});

const escapeKey = 4194305;

// The scripted game applies one D-14 effect per acknowledged Escape press and acknowledges every delivered key.
function scriptedEscapes(t, scenes, {acknowledge = () => true, apply = () => true} = {}) {
  const directory = scratch(t);
  const peer = fakePeer(directory, 'solo', 4201);
  let scene = 0, events = 0;
  const show = (lastInput) => writeDraw(peer, {inputEvents: events, lastInput, ...scenes[scene].input},
    scenes[scene].controls);
  show(null);
  const input = fakeInput(directory, (target, request) => {
    if (request.action !== 'key' || !acknowledge(request)) return;
    if (request.pressed && apply(scene)) scene = Math.min(scene + 1, scenes.length - 1);
    events += 1;
    show({key: request.key === 27 ? escapeKey : request.key, pressed: request.pressed, echo: false});
  });
  return {state: {directory, nonce, peers: [peer], input}, peer, sent: input.records};
}

const workspace = name => 'InventoryWorkspace/' + name;
const inRun = (controls, open = true) => ({input: {inventoryOpen: open, phase06: {Views: {}}},
  controls: [control('Menu/Start', 'Start', {visible: false}), ...controls]});
const d14Scenes = () => [
  inRun([control(workspace('ItemMenu/Options/QuantityToMove'), '3'), control(workspace('InventoryPanels/Bag_58/CloseBag_58')),
    control(workspace('InventoryPanels/Bag_63/CloseBag_63'))]),
  inRun([control(workspace('InventoryPanels/Bag_58/CloseBag_58')), control(workspace('InventoryPanels/Bag_63/CloseBag_63'))]),
  inRun([control(workspace('InventoryPanels/Bag_58/CloseBag_58'))]),
  inRun([]),
  inRun([], false),
  {input: {inventoryOpen: false}, controls: menuControls()},
];

test('Menu_EscapeEffects_FollowD14Order', async t => {
  await t.test('each acknowledged Escape shows the next D-14 effect until the menu', async t => {
    const {state, peer, sent} = scriptedEscapes(t, d14Scenes());
    const steps = await run.menu(state, peer);
    assert.deepEqual(steps, ['cancel drag or split preview', 'close bag window', 'close bag window',
      'close inventory window', 'return to ordinary menu']);
    const escapes = sent.filter(row => row.action === 'key');
    assert.equal(escapes.length, 10);
    assert(escapes.every((row, index) => row.key === 27 && row.pressed === (index % 2 === 0)));
  });

  await t.test('an acknowledged Escape without its effect fails with the step name', async t => {
    const {state, peer} = scriptedEscapes(t, d14Scenes(), {apply: scene => scene !== 2});
    await assert.rejects(run.menu(state, peer), /solo Escape acknowledged without its D-14 effect "close bag window"/);
  });

  await t.test('an open item menu closes before the bag windows', async t => {
    const scenes = d14Scenes().slice(2);
    scenes.unshift(inRun([control(workspace('ItemMenu/Options/@Label@116'), 'Berries'),
      control(workspace('InventoryPanels/Bag_58/CloseBag_58'))]));
    const {state, peer} = scriptedEscapes(t, scenes);
    assert.deepEqual((await run.menu(state, peer)).slice(0, 2), ['close item menu', 'close bag window']);
  });

  await t.test('an unacknowledged press still releases the key and reports the receipt', async t => {
    const {state, peer, sent} = scriptedEscapes(t, d14Scenes(), {acknowledge: request => !request.pressed});
    await assert.rejects(run.menu(state, peer), /RENDERED_DEADLINE native event solo\/27\/true/);
    assert.deepEqual(sent.filter(row => row.action === 'key').map(row => row.pressed), [true, false]);
  });

  await t.test('the menu already shown needs no Escape', async t => {
    const {state, peer, sent} = scriptedEscapes(t, d14Scenes().slice(-1));
    assert.deepEqual(await run.menu(state, peer), []);
    assert.equal(sent.filter(row => row.action === 'key').length, 0);
  });
});

function loadWith(file, stubs) {
  const loaded = new Module(file, module);
  loaded.filename = file;
  loaded.paths = Module._nodeModulePaths(path.dirname(file));
  const original = loaded.require.bind(loaded);
  loaded.require = id => stubs[id] ?? original(id);
  loaded._compile(fs.readFileSync(file, 'utf8'), file);
  return loaded.exports;
}

const lan = require('./phase06-lan.cjs').helpers;
const people = [['local', 'Host'], ['b1', 'Guest 1'], ['b2', 'Guest 2'], ['b3', 'Guest 3']];

// A receiver's draw: the snapshot's ready participants and the four roster rows FoundationHud.Project draws.
function rosterDraw(binding, texts = null) {
  const Participants = people.map(([BindingId, Label], index) => ({BindingId, Label, Slot: index + 1, Ready: true,
    Actor: {PlayerId: 'player-' + index}}));
  const rows = texts ?? Participants.map(entry => 'P' + entry.Slot + (entry.BindingId === binding ? ' (You)' : '')
    + ' · ' + entry.Label);
  return {bindingId: binding, snapshot: {Participants},
    controls: [control('Roster/Header', 'Connected · 4/4 players'), ...rows.map((text, index) =>
      control('Roster/Player' + (index + 1), text))]};
}

test('Roster_DistinctNames_MatchSnapshotSlots', async t => {
  await t.test('every receiver accepts its own correct roster', () => {
    for (const [binding] of people) assert.equal(lan.rosterMatches(rosterDraw(binding)), true, binding);
  });
  await t.test('swapped names are rejected', () => {
    assert.equal(lan.rosterMatches(rosterDraw('b1', ['P1 · Guest 1', 'P2 (You) · Host', 'P3 · Guest 2',
      'P4 · Guest 3'])), false);
  });
  await t.test('a duplicated (You) marker is rejected', () => {
    assert.equal(lan.rosterMatches(rosterDraw('b1', ['P1 (You) · Host', 'P2 (You) · Guest 1', 'P3 · Guest 2',
      'P4 · Guest 3'])), false);
  });
  await t.test('the marker on another player\'s row is rejected', () => {
    assert.equal(lan.rosterMatches(rosterDraw('b2', ['P1 · Host', 'P2 (You) · Guest 1', 'P3 · Guest 2',
      'P4 · Guest 3'])), false);
  });
  await t.test('equal names cannot prove distinct identities', () => {
    const draw = rosterDraw('b1', ['P1 · Player 1', 'P2 (You) · Player 1', 'P3 · Player 1', 'P4 · Player 1']);
    for (const entry of draw.snapshot.Participants) entry.Label = 'Player 1';
    assert.equal(lan.rosterMatches(draw), false);
  });
  await t.test('a hidden or missing row is rejected', () => {
    const hidden = rosterDraw('local');
    hidden.controls[4].visible = false;
    assert.equal(lan.rosterMatches(hidden), false);
    const missing = rosterDraw('local');
    missing.controls.pop();
    assert.equal(lan.rosterMatches(missing), false);
  });
  await t.test('the four controlled bodies must stand one body width apart', () => {
    const bodies = xs => ({actors: xs.map(screenX => ({screenX, visible: true}))});
    assert.equal(lan.spread(bodies([611.5, 635.5, 659.5, 683.5])), true);
    assert.equal(lan.spread(bodies([611.5, 621.1, 659.5, 683.5])), false, 'run-1 bodies 9.6 px apart');
    assert.equal(lan.spread(bodies([611.5, 611.5, 659.5, 683.5])), false);
    assert.equal(lan.spread(bodies([611.5, 635.5, 659.5])), false);
  });
});

