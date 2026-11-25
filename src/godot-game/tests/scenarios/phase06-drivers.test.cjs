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

// The reproduced F13 stream (game-tests 069f23d0, guest-3 frames 701-757): the place receipt is the latest for 45 ms,
// then the workspace's own window request replaces it.
function receiptFrames(t, rows) {
  const directory = scratch(t);
  const peer = {role: 'guest-3', pid: 7, directory, quitRequested: false};
  const state = {nonce, peers: [peer]};
  const frame = ([number, sequence, kind, status, replayed = false]) => ({frame: number, pid: 7, nonce,
    provenance: 'read-only-state-observation', phase06: {World: {}}, runAction: {sequence, kind, status, replayed}});
  fs.writeFileSync(path.join(directory, 'frames.ndjson'), rows.map(row => JSON.stringify(frame(row)) + '\n').join(''));
  return {state, peer, before: frame(rows[0])};
}

test('Receipt_ExactKind_IgnoresInterleaved', async t => {
  await t.test('the replacing workspace receipt is recorded, and the place receipt is the one accepted', async t => {
    const {state, peer, before} = receiptFrames(t, [[701, 15, 'external-items-page', 'Ready'],
      [754, 16, 'place', 'Applied'], [757, 17, 'workspace', 'Ready']]);
    const row = await native.receipt(state, peer, 'place', before, 'place receipt');
    assert.deepEqual([row.frame, row.runAction.status], [754, 'Applied']);
    assert.deepEqual(state.interleaved.map(entry => [entry.sequence, entry.kind, entry.status, entry.relation]),
      [[17, 'workspace', 'Ready', 'after']]);
  });
  await t.test('receipts of other kinds before the match are recorded, never accepted', async t => {
    const {state, peer, before} = receiptFrames(t, [[1, 3, 'select', 'Ready'], [2, 4, 'workspace', 'Ready'],
      [3, 5, 'place', 'Applied']]);
    const row = await native.receipt(state, peer, 'place', before, 'place receipt');
    assert.equal(row.runAction.sequence, 5);
    assert.deepEqual(state.interleaved.map(entry => [entry.sequence, entry.kind, entry.awaiting, entry.relation]),
      [[4, 'workspace', 'place', 'before']]);
  });
  await t.test('a replayed result or an earlier sequence is never the submitted request\'s receipt', async t => {
    const {state, peer, before} = receiptFrames(t, [[1, 4, 'select', 'Ready'], [2, 5, 'place', 'Applied', true],
      [3, 3, 'place', 'Applied']]);
    await assert.rejects(native.receipt(state, peer, 'place', before, 'place receipt', 300),
      /SUBJECT_TIMEOUT place receipt/);
    assert.deepEqual(state.interleaved.map(entry => [entry.sequence, entry.replayed]), [[5, true]]);
  });
});

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

// A dead host's committed draw: the spectator camera, the drawn camera center and one teammate's position.
function heldDraw(drawFrame, cameraX, teammateX, extra = {}) {
  return {drawFrame, cameraX, input: {spectator: {Visible: true, Followed: null, Camera: -128, ...extra},
    phase06: {World: {Actors: [{Id: 'survivor-02', X: teammateX}]}}}};
}

test('Succession_CameraHold_RequiresDistinctDraws', async t => {
  await t.test('one pose over two distinct draws while the teammate travels is a hold', () => {
    assert.equal(lan.cameraHold([heldDraw(10, -128, -8000), heldDraw(11, -128, -7400),
      heldDraw(40, -128, -6850)], 'survivor-02', 1150), true);
  });
  await t.test('a single draw is rejected', () => {
    assert.equal(lan.cameraHold([heldDraw(10, -128, -6850)], 'survivor-02', 0), false);
  });
  await t.test('the same draw twice is not two committed draws', () => {
    assert.equal(lan.cameraHold([heldDraw(10, -128, -8000), heldDraw(10, -128, -6850)], 'survivor-02', 1150), false);
  });
  await t.test('a moving camera is rejected', () => {
    assert.equal(lan.cameraHold([heldDraw(10, -128, -8000), heldDraw(20, -118.4, -6850)], 'survivor-02', 1150),
      false);
    assert.equal(lan.cameraHold([heldDraw(10, -128, -8000), heldDraw(20, -128, -6850, {Camera: -118.4})],
      'survivor-02', 1150), false);
  });
  await t.test('a teammate that did not travel cannot separate the poses', () => {
    assert.equal(lan.cameraHold([heldDraw(10, -128, -8000), heldDraw(20, -128, -7900)], 'survivor-02', 1150), false);
  });
  await t.test('a followed or hidden spectator is not a hold', () => {
    assert.equal(lan.cameraHold([heldDraw(10, -128, -8000), heldDraw(20, -128, -6850,
      {Followed: {Survivor: 'survivor-02'}})], 'survivor-02', 1150), false);
    assert.equal(lan.cameraHold([heldDraw(10, -128, -8000), heldDraw(20, -128, -6850, {Visible: false})],
      'survivor-02', 1150), false);
  });
});

// Committed frames of a dead host: its status view times and the corpse recording the death.
function deadFrame(frame, rootTime, canRespawn, opensAt = 1115, deathAt = 115) {
  return {frame, spectator: {Visible: true, CanRespawn: canRespawn}, phase06: {Views: {status: {Header:
    {OpensAt: opensAt, RootTime: rootTime}}}, World: {Corpses: [{SurvivorId: 'survivor-01', At: deathAt}]}}};
}

test('Succession_RespawnGateAndHandover_FollowTheirRules', async t => {
  await t.test('disabled before the authoritative second and enabled after it', () => {
    const gate = lan.respawnGate([deadFrame(1, 152, false), deadFrame(2, 1100, false), deadFrame(3, 1137, true)],
      'survivor-01');
    assert.deepEqual(gate, {deathAt: 115, opens: 1115, disabledFrames: 2, enabledFrame: 3});
  });
  await t.test('an early enabled button or a gate other than one second is rejected', () => {
    assert.equal(lan.respawnGate([deadFrame(1, 152, false), deadFrame(2, 900, true)], 'survivor-01'), null);
    assert.equal(lan.respawnGate([deadFrame(1, 152, false, 615), deadFrame(2, 700, true, 615)], 'survivor-01'),
      null);
    assert.equal(lan.respawnGate([deadFrame(1, 1200, true)], 'survivor-01'), null, 'no disabled frame observed');
  });
  const notice = (frame, Camera, Smoothing) => ({frame, spectator: {TransferNotice: 'Control transferred · s-05',
    Camera, Smoothing}});
  await t.test('a successor within a quarter span hands over smoothly', () => {
    assert.deepEqual(lan.handover([notice(1, -110, true), notice(2, -119, true), notice(3, null, false)], -110, -128),
      {held: -110, target: -128, expected: 'smooth', observed: 'smooth'});
  });
  await t.test('a distant successor cuts, and a near cut does not meet the rule', () => {
    assert.equal(lan.handover([notice(1, 300, false)], -128, 300).observed, 'cut');
    assert.equal(lan.handover([notice(1, 300, false)], -128, 300).expected, 'cut');
    const near = lan.handover([notice(1, -128, false)], -110, -128);
    assert.notEqual(near.observed, near.expected);
  });
});

const inspectionFile = path.join(__dirname, 'phase06-inspection.cjs');
const inspection = require('./phase06-inspection.cjs');
const gameRoot = path.resolve(__dirname, '../../game');
const sha256 = data => require('node:crypto').createHash('sha256').update(data).digest('hex');

// A prepared project copy holding the named authored files, as open() prepares it.
function preparedProject(t, files) {
  const root = scratch(t);
  for (const file of files) {
    fs.mkdirSync(path.dirname(path.join(root, 'project', file)), {recursive: true});
    fs.copyFileSync(path.join(gameRoot, file), path.join(root, 'project', file));
  }
  return {root, build: {sourceProject: {path: 'project'}}};
}

test('InspectionDriver_FixtureIdentity_UsesExportedHash', async t => {
  await t.test('the driver binds the content.cjs hash, and evidence.cjs still has none (run-1 F10)', () => {
    assert.equal(typeof require('../../tools/asset-pipeline/content.cjs').hash, 'function');
    assert.equal(require('../../tools/asset-pipeline/evidence.cjs').hash, undefined);
    const source = stripped(fs.readFileSync(inspectionFile, 'utf8'));
    assert.match(source, /const\s*\{\s*hash\s*\}\s*=\s*require\(''\)/);
    assert.doesNotMatch(source, /evidence\.hash\b/);
  });
  await t.test('H14 builds its recipe with the original and prepared stimulus hashes', async () => {
    const state = preparedProject(t, ['content/fixtures/crowds.json']);
    const file = path.join(state.root, 'project/content/fixtures/crowds.json');
    const original = fs.readFileSync(file);
    const recipe = await inspection.recipes.get('heldout.H14')(state);
    const authored = JSON.parse(original);
    authored.actors.find(row => row.id === 'corpse-000').equipment = {};
    assert.deepEqual(recipe.stimulus, {path: 'content/fixtures/crowds.json', original: sha256(original),
      prepared: sha256(Buffer.from(JSON.stringify(authored)))});
    assert.notEqual(recipe.stimulus.prepared, recipe.stimulus.original);
    assert.deepEqual(fs.readFileSync(file), original, 'building the recipe edits nothing');
    assert.equal(typeof recipe.cleanup, 'function');
  });
  await t.test('an edit stays in the prepared copy and restore writes the original bytes back', () => {
    const state = preparedProject(t, ['content/animation/fits-c.json']);
    const file = path.join(state.root, 'project/content/animation/fits-c.json');
    const original = fs.readFileSync(file);
    const removed = inspection.fixture(state, 'content/animation/fits-c.json', () => null);
    assert.deepEqual(removed.identity, {path: 'content/animation/fits-c.json', original: sha256(original),
      prepared: null});
    removed.edit();
    assert.equal(fs.existsSync(file), false);
    removed.restore();
    assert.deepEqual(fs.readFileSync(file), original);
    assert.throws(() => inspection.fixture(state, '../outside.json', () => null));
  });
});

test('H16Expectations_AuthoredLocomotion_MatchSteps', async t => {
  const definition = JSON.parse(fs.readFileSync(path.join(gameRoot, 'content/animation/locomotion-melee.json'), 'utf8'));
  const {steps} = await inspection.recipes.get('heldout.H16')({root: path.resolve(gameRoot, '../../..')});
  const fields = step => Object.fromEntries(step.checks.filter(check => check.match === 'field')
    .map(check => [check.key, check.value]));
  // Step k from a clip start lands on authored sample k at the sum of the k earlier durations; walk loops.
  const expected = (clip, step) => {
    const rows = definition.clips[clip];
    const index = clip === 'walk' ? step % rows.length : step;
    const start = rows.slice(0, index).reduce((sum, row) => sum + row.duration, 0);
    return {movement: clip, row: rows[index], start};
  };
  const plan = {'start-walk': ['walk', 0], 'exact-second-frame': ['walk', 1], 'held-walk-endpoint': ['walk', 2],
    'start-jump': ['jump', 0], 'step-nonuniform-jump': ['jump', 1], 'step-held-jump': ['jump', 2],
    'step-jump-landing': ['jump', 3]};

  await t.test('the original H16 step identities and order are kept', () => {
    assert.deepEqual(steps.map(step => step.id), ['authored-walk', ...Object.keys(plan)]);
  });
  await t.test('every step expects the authored pace, stage, offset and time', () => {
    for (const [id, [clip, index]] of Object.entries(plan)) {
      const want = expected(clip, index);
      const got = fields(steps.find(step => step.id === id));
      assert.equal(got.movement, want.movement, id);
      assert.equal(got.lowerPose, want.row.pose, id);
      assert.equal(got.stage, want.row.stage, id);
      assert.deepEqual(got.lowerOffset, want.row.offset, id);
      assert(Math.abs(got.strideSeconds - want.start) < 1e-9, id + ' ' + got.strideSeconds + ' ' + want.start);
    }
  });
  await t.test('the run-1 failing step expects the authored airborne-down sample at 0.24 s', () => {
    const held = fields(steps.find(step => step.id === 'step-held-jump'));
    assert.deepEqual(held, {movement: 'jump', lowerPose: 'walk-near', stage: 'airborne-down', lowerOffset: [0, -3],
      strideSeconds: 0.24});
  });
});

// A Godot remote debugger stand-in: frames are a little-endian u32 length and an encoded [message, thread, payload].
function fakeDebugger(port) {
  const {encode} = require('../../tools/asset-pipeline/native-observer.cjs');
  const frame = value => {
    const body = encode(value), header = Buffer.alloc(4);
    header.writeUInt32LE(body.length);
    return Buffer.concat([header, body]);
  };
  const socket = require('node:net').connect(port, '127.0.0.1');
  let buffer = Buffer.alloc(0), requests = 0;
  socket.on('data', chunk => {
    buffer = Buffer.concat([buffer, chunk]);
    while (buffer.length >= 4 && buffer.length >= buffer.readUInt32LE() + 4) {
      buffer = buffer.subarray(buffer.readUInt32LE() + 4);
      requests++;
    }
  });
  return {socket, frame, get requests() { return requests; },
    tree: id => frame(['scene:scene_tree', 7, [0, 'root', 'Window', id, '', 0]])};
}

async function until(test, limit = 5000) {
  const end = Date.now() + limit;
  while (!test()) {
    assert(Date.now() < end, 'condition not reached');
    await new Promise(resolve => setTimeout(resolve, 5));
  }
}

// The run-1 inspection state when H16 failed: a failed step, the hub alive and a fixture possibly edited.
function failedInspection(t, session, files = ['content/animation/locomotion-melee.json']) {
  const state = {...preparedProject(t, files), relative: 'inspection-run', revision: 'test', invocation: 'test',
    budget: () => 1000, evidence: [], resets: [], session};
  return state;
}

function mockedSession(events, {checkpoint, release, restore} = {}) {
  return {usable: true, events,
    async release() { events.push('release'); await release?.(); },
    async checkpoint(item, step) {
      events.push('checkpoint ' + step.id);
      if (checkpoint) return checkpoint(item, step);
      throw new Error('RENDERED_NATIVE_SETTLE mocked');
    },
    async inspect() { throw new Error('RENDERED_NATIVE_DEADLINE mocked'); },
    async restore(item, steps, invocation, lifetime) {
      events.push('restore ' + item.id + ' ' + steps.map(step => step.id).join(',') + ' ' + lifetime);
      await restore?.();
      return {checkpoints: [], baseline: {path: 'baseline.json'}};
    }};
}

test('InspectionReset_AfterFailedStep_RestoresFixtureAndBaseline', async t => {
  const scenarios = require('../../tools/asset-pipeline/scenarios.cjs');

  await t.test('a late response to an abandoned native request is discarded, not a second answer (run-1 cause)',
    async () => {
      const controller = new AbortController();
      const observer = await scenarios.nativeObserver((ms = 40) => ms, controller.signal);
      const fake = fakeDebugger(Number(observer.endpoint.split(':').at(-1)));
      try {
        fake.socket.write(fake.frame(['set_pid', 7, [4242]]));
        // The settle deadline expires with the scene-tree request in flight, as it did after H16 step-held-jump.
        await assert.rejects(observer.snapshot(4242, [], Date.now() + 150), /RENDERED_NATIVE_DEADLINE/);
        const reset = observer.snapshot(4242, [], Date.now() + 5000);
        await until(() => fake.requests === 2);
        fake.socket.write(Buffer.concat([fake.tree(1), fake.tree(2)]));
        const observed = await reset;
        assert.equal(observed.tree.payload[3], 2, 'the reset request receives its own response');
      } finally {
        fake.socket.destroy();
        await observer.close();
      }
    });

  await t.test('reset after a failed step releases input, restores the fixture, then the baseline', async () => {
    const events = [];
    const state = failedInspection(t, mockedSession(events, {checkpoint: async (item, step) => {
      // The error step edits locomotion-melee.json in the prepared copy, then its selection fails.
      await step.drive.call(step, {click: async () => {}, tap: async () => {}, hold: async () => {},
        release: async () => {}});
    }}));
    const file = path.join(state.root, 'project/content/animation/locomotion-melee.json');
    const original = fs.readFileSync(file);
    const id = 'asset-pipeline.UIH.CHARACTER-PREVIEW.error';
    // The case's own cleanup fails once, as a locked file would, so only the reset can restore the bytes.
    const recipe = inspection.recipes.get('UIH.CHARACTER-PREVIEW.error');
    let failures = 1;
    inspection.recipes.set('UIH.CHARACTER-PREVIEW.error', async current => {
      const built = await recipe(current);
      return {...built, cleanup() {
        if (failures-- > 0) throw new Error('EBUSY mocked fixture restore');
        return built.cleanup();
      }};
    });
    t.after(() => inspection.recipes.set('UIH.CHARACTER-PREVIEW.error', recipe));
    await assert.rejects(inspection.cases[id](state), /EBUSY mocked/);
    assert.notDeepEqual(fs.readFileSync(file), original, 'the failed case left its fixture edit');
    await inspection.reset(state);
    assert.deepEqual(fs.readFileSync(file), original);
    assert.deepEqual(events, ['checkpoint failed-character', 'release',
      'restore ' + id + ' reset-2 2']);
    assert.equal(state.currentItem, null);
    const record = JSON.parse(fs.readFileSync(path.join(state.root, state.resets[0].path), 'utf8'));
    assert.deepEqual([record.id, record.completed, record.failures], [id, ['RELEASE', 'FIXTURE', 'BASELINE'], []]);
  });

  await t.test('a failed release still restores the fixture and names the failing stage', async () => {
    const events = [];
    const state = failedInspection(t, mockedSession(events, {release: async () => {
      throw new Error('RENDERED_PROCESS_LOST inspection-hub');
    }}));
    const variant = inspection.fixture(state, 'content/animation/locomotion-melee.json', () => Buffer.from('{}'));
    variant.edit();
    Object.assign(state, {currentItem: {id: 'asset-pipeline.heldout.H16'}, cleanup: variant.restore});
    await assert.rejects(inspection.reset(state),
      /^Error: INSPECTION_RESET_RELEASE asset-pipeline\.heldout\.H16: RENDERED_PROCESS_LOST inspection-hub$/);
    assert.equal(fs.readFileSync(path.join(state.root, 'project', variant.identity.path), 'utf8')[0], '{');
    assert.notEqual(fs.readFileSync(path.join(state.root, 'project', variant.identity.path), 'utf8'), '{}');
    assert.deepEqual(events, ['release'], 'an unusable hub gets no baseline restore');
  });

  await t.test('a baseline restore failure names its stage and native cause', async () => {
    const state = failedInspection(t, mockedSession([], {restore: async () => {
      throw new Error('RENDERED_NATIVE_SETTLE baseline');
    }}));
    state.currentItem = {id: 'asset-pipeline.heldout.H17'};
    await assert.rejects(inspection.reset(state),
      /INSPECTION_RESET_BASELINE asset-pipeline\.heldout\.H17: RENDERED_NATIVE_SETTLE baseline/);
  });
});

