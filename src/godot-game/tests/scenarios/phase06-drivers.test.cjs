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

function loadWith(file, stubs) {
  const loaded = new Module(file, module);
  loaded.filename = file;
  loaded.paths = Module._nodeModulePaths(path.dirname(file));
  const original = loaded.require.bind(loaded);
  loaded.require = id => stubs[id] ?? original(id);
  loaded._compile(fs.readFileSync(file, 'utf8'), file);
  return loaded.exports;
}

