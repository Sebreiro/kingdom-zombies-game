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

test('PreparedProject_RenderedLanSuite_UsesOneCopy', async t => {
  const network = require('../lan-checks/network.cjs');
  const native = require('../lan-checks/rendered.cjs');
  const scenarios = require('../lan-checks/scenarios.cjs');
  const suite = {...nativeScratch(t), ownsDesktopLock: true};
  const launched = [];
  t.mock.method(network, 'launch', (context, role) => {
    const directory = path.join(context.directory, role);
    fs.mkdirSync(directory);
    launched.push({role, project: context.project, rendered: context.rendered});
    return {role, directory, pid: 40_000 + launched.length, profile: 'profile-' + role, exit: {code: 0}};
  });
  t.mock.method(network, 'cleanup', async () => {});
  t.mock.method(native, 'until', async () => true);
  t.mock.method(native, 'driver', () => ({start: async () => {}, close: async () => {},
    send: async () => { throw new Error('RECORDED_DRIVER_ONLY'); }}));
  const {results, launches} = await scenarios.run(suite, [{id: 'rendered.recorded-launch', runGroup: 'G3', order: 1}]);
  const project = path.join(suite.directory, 'imported-game');
  assertCleanImport(suite, project);
  assert.deepEqual(launched.map(row => row.role), ['host', 'guest1', 'guest2', 'guest3']);
  assert(launched.every(row => row.project === project && row.rendered === true), 'every role launches the prepared copy');
  assert.deepEqual(launches.map(row => row.role), ['host', 'guest1', 'guest2', 'guest3']);
  assert.equal(results.length, 1);
  assert.equal(results[0].status, 'fail');
  assert.match(results[0].message, /RECORDED_DRIVER_ONLY/);
  assert.equal(results[0].preparedProject.path, 'imported-game');
  assert.deepEqual(results[0].preparedProject.assemblies.map(row => row.name), ['KingdomZombies.dll', 'KingdomZombies.Core.dll']);
});

test('RunScenarios_MockDriver_RecordsDurationsAndLaunches', async t => {
  const {runScenarios} = require('./scenario-group.cjs');
  const context = {directory: scratch(t)};
  const rows = [{id: 'mock.first', order: 1}, {id: 'mock.second', order: 2}, {id: 'mock.third', order: 3}];
  const peer = state => state.peers.push({role: 'peer-' + state.peers.length, pid: 50_000 + state.peers.length});
  const driver = {
    open() { return {peers: []}; },
    cases: {
      async 'mock.first'(state) { peer(state); await new Promise(resolve => setTimeout(resolve, 25)); return ['first.json']; },
      'mock.second'(state) { peer(state); throw new Error('mock failure'); },
      'mock.third'() { throw new Error('a blocked case must not run'); }
    },
    reset(state) { if (state.peers.length === 2) throw new Error('mock reset'); },
    close() {}
  };
  const results = await runScenarios(context, rows, driver);
  assert.deepEqual(results.map(row => row.status), ['pass', 'fail', 'blocked']);
  for (const result of results) {
    assert(Number.isSafeInteger(result.durationMs) && result.durationMs >= 0, result.id + ' durationMs');
    assert.equal(result.launchCount, 2, result.id + ' launchCount');
  }
  assert(results[0].durationMs >= 20, 'measured wall time');
  assert.equal(results[2].durationMs, 0);
  assert.equal(results[2].message, 'Scenario reset failed: mock reset');
});

test('RunGroups_LegacyAdapters_ReportNumericTiming', async t => {
  const basic = require('./basic.cjs');
  const processes = require('../lan-checks/processes.cjs');
  const {runGroups} = require('./suites.cjs');
  const root = scratch(t);
  const context = {root, directory: path.join(root, 'invocation'), revision: 'a'.repeat(40)};
  fs.mkdirSync(context.directory);
  const runs = adapter => workspace.locations(root).runs(adapter);
  t.mock.method(basic, 'executeBasic', async () => ({results: [], prepared: true}));
  t.mock.method(processes, 'run', async (executable, args) => {
    const tool = args[0];
    if (tool.endsWith('gameplay-checks/check.cjs')) {
      const directory = runs('gameplay-checks', 'recorded');
      fs.mkdirSync(directory, {recursive: true});
      const file = path.join(directory, 'summary.json');
      fs.writeFileSync(file, JSON.stringify({revision: context.revision, status: 'pass', selected: ['core.timed'],
        results: [{id: 'core.timed', status: 'pass', durationMilliseconds: 1234.4}]}));
      return {streamsClosed: true, code: 0, stdout: 'PASS 1/1 selected obligations; ' + path.relative(root, file) + '\n'};
    }
    if (tool.endsWith('asset-pipeline/check.cjs')) {
      const directory = runs('asset-pipeline', 'recorded');
      fs.mkdirSync(directory, {recursive: true});
      fs.writeFileSync(path.join(directory, 'result.json'), JSON.stringify({id: 'source.untimed', revision: context.revision,
        status: 'pass'}));
      return {streamsClosed: true, code: 0, stdout: 'PASS source.untimed ' + path.relative(root, directory) + '\n'};
    }
    const plan = JSON.parse(fs.readFileSync(args[1], 'utf8'));
    fs.writeFileSync(path.join(plan.context.directory, 'scenario.json'), '{}');
    fs.writeFileSync(path.join(plan.context.directory, 'results.json'), JSON.stringify(plan.rows.map(row => ({id: row.id,
      status: 'pass', artifacts: ['scenario.json'], durationMs: 7, launchCount: 2}))));
    return {streamsClosed: true, code: 0, stdout: ''};
  });
  const selected = [
    {id: 'gameplay-checks.core.timed', adapter: 'gameplay-checks', tier: 1, legacyId: 'core.timed'},
    {id: 'asset-pipeline.source.untimed', adapter: 'asset-pipeline', tier: 1, legacyId: 'source.untimed'},
    {id: 'mock.scenario.first', adapter: 'scenario', tier: 1, runGroup: 'mock.scenario', order: 1,
      driver: 'src/godot-game/tests/scenarios/mock.cjs'},
    {id: 'mock.scenario.second', adapter: 'scenario', tier: 1, runGroup: 'mock.scenario', order: 2,
      driver: 'src/godot-game/tests/scenarios/mock.cjs'}
  ];
  const {results, groups} = await runGroups(context, selected);
  assert.deepEqual(results.map(row => row.status), ['pass', 'pass', 'pass', 'pass']);
  for (const result of results) {
    assert(Number.isSafeInteger(result.durationMs) && result.durationMs >= 0, result.id + ' durationMs');
    assert(Number.isSafeInteger(result.launchCount) && result.launchCount >= 0, result.id + ' launchCount');
  }
  const byId = Object.fromEntries(results.map(row => [row.id, row]));
  assert.equal(byId['gameplay-checks.core.timed'].durationMs, 1234);
  assert.equal(byId['gameplay-checks.core.timed'].timing, undefined);
  assert.equal(byId['asset-pipeline.source.untimed'].durationMs, 0);
  assert.equal(byId['asset-pipeline.source.untimed'].timing, 'not-recorded');
  assert.equal(byId['asset-pipeline.source.untimed'].launchRecord, 'not-recorded');
  assert.equal(groups.find(group => group.id === 'mock.scenario').launchCount, 2);
});
