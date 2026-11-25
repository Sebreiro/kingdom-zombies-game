'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const {randomUUID} = require('node:crypto');
const EventEmitter = require('node:events');
const workspace = require('../workspace-paths.cjs');
const {run} = require('../lan-checks/processes.cjs');
const lan = require('../lan-checks/evidence.cjs');
const {fixture} = require('../lan-checks/runner-fixture.cjs');
const {exact, reconcile, writeJson} = require('./report.cjs');

function scratch(t) {
  const base = workspace.locations().temp('game-tests', 'controls');
  fs.mkdirSync(base, {recursive: true});
  const directory = fs.mkdtempSync(path.join(base, 'run-'));
  t.after(() => fs.rmSync(directory, {recursive: true, force: true}));
  return directory;
}

test('missing dependency, actual assertion failure and timeout are rejected after a clean child', async t => {
  const cwd = scratch(t);
  const clean = await run(process.execPath, ['-e', 'console.log("clean control")'], {cwd});
  assert.equal(clean.stdout.trim(), 'clean control');
  await assert.rejects(run(path.join(cwd, 'missing-executable'), [], {cwd}), /ENOENT/);
  await assert.rejects(run(process.execPath, ['-e', 'require("node:assert").equal(1, 2)'], {cwd}),
    error => error.record?.code === 1 && error.record.stderr.includes('AssertionError'));
  await assert.rejects(run(process.execPath, ['-e', 'console.log("started"); setInterval(()=>{},1000)'], {cwd}, 800),
    error => error.record?.timedOut && error.record.stdout.includes('started'));
});

test('zero discovery, skipped required result and deleted evidence are rejected', t => {
  const directory = scratch(t);
  const selected = [{id: 'scenario'}];
  const result = () => [{id: 'scenario', status: 'pass', artifacts: ['scene.log']}];
  const log = path.join(directory, 'scene.log');
  fs.writeFileSync(log, 'clean control\n');
  assert.equal(reconcile(directory, selected, result())[0].status, 'pass');
  assert.throws(() => exact(['scenario'], []), /membership mismatch/);
  const skipped = result();
  skipped[0].status = 'skip';
  assert.equal(reconcile(directory, selected, skipped)[0].status, 'fail');
  fs.unlinkSync(log);
  assert.equal(reconcile(directory, selected, result())[0].status, 'fail');
});

test('the rendered LAN reconciliation rejects a missing peer in otherwise valid evidence', t => {
  const directory = scratch(t);
  const {context, selected, results} = fixture(directory);
  assert.deepEqual(lan.reconcile(selected, results, context), results);
  results[0].peers.pop();
  assert.throws(() => lan.reconcile(selected, results, context), /EXACT_MEMBERSHIP peers/);
});

test('later outcomes cannot overwrite an original run report', t => {
  const directory = scratch(t);
  const original = writeJson(directory, 'summary.json', {status: 'fail'});
  assert.throws(() => writeJson(directory, 'summary.json', {status: 'pass'}), /EEXIST/);
  assert.equal(JSON.parse(fs.readFileSync(original, 'utf8')).status, 'fail');
});

test('new scenario rows share one driver and reset between ordered cases', async t => {
  const {runScenarios} = require('./scenario-group.cjs');
  const context = {directory: scratch(t)};
  const rows = [{id: 'later.second', order: 2}, {id: 'later.first', order: 1}];
  const events = [];
  const driver = {
    open() { events.push('open'); return {value: 0}; },
    cases: {
      'later.first'(state) { events.push('first'); state.value++; return ['first.json']; },
      'later.second'(state) { events.push('second'); assert.equal(state.value, 0); return ['second.json']; }
    },
    reset(state) { events.push('reset'); state.value = 0; },
    close() { events.push('close'); }
  };
  const results = await runScenarios(context, rows, driver);
  assert.deepEqual(events, ['open', 'first', 'reset', 'second', 'close']);
  assert.deepEqual(results.map(row => [row.id, row.status]), [['later.first', 'pass'], ['later.second', 'pass']]);
});

function nativeScratch(t) {
  const root = workspace.checkoutRoot();
  return {root, directory: scratch(t), nonce: randomUUID(), platform: process.platform + '-' + process.arch,
    env: require('../lan-checks/check.cjs').environment(root)};
}

function assertCleanImport(context, project) {
  assert.equal(path.dirname(project), context.directory);
  assert(fs.readdirSync(path.join(project, '.godot/imported'))
    .some(name => /^donor-kit\.png-[0-9a-f]{32}\.ctex$/.test(name)), 'imported donor-kit atlas');
  assert.equal(fs.readFileSync(path.join(context.directory, 'game-import.stderr.log'), 'utf8').trim(), '');
  assert.doesNotMatch(fs.readFileSync(path.join(context.directory, 'game-import.stdout.log'), 'utf8'),
    /ERROR:|Failed to load|Error importing/);
}

function recordedChild() {
  const child = new EventEmitter();
  child.stdout = new EventEmitter();
  child.stderr = new EventEmitter();
  return child;
}

test('PreparedProject_InheritedRendered_LoadsImportedAtlases', async t => {
  const {nativeContext} = require('../gameplay-checks/check.cjs');
  const {createSession} = require('../gameplay-checks/rendered.cjs');
  const context = nativeScratch(t);
  const native = await nativeContext(context);
  assertCleanImport(context, native.project);
  assert.deepEqual(native.preparedProject.assemblies.map(row => row.name), ['KingdomZombies.dll', 'KingdomZombies.Core.dll']);
  const launches = [];
  const directory = path.join(context.directory, 'session');
  fs.mkdirSync(directory);
  const session = createSession({...native, directory, engine: 'recorded-engine'}, (executable, args, options) => {
    launches.push({executable, args, cwd: options.cwd});
    return recordedChild();
  });
  await session.close();
  assert.equal(launches.length, 1);
  const {args} = launches[0];
  assert.equal(args[args.indexOf('--path') + 1], native.project);
  assert.notEqual(args[args.indexOf('--path') + 1], path.join(context.root, 'src/godot-game/game'));
});
