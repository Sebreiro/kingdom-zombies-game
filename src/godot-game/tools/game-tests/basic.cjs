'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { launch } = require('../dotnet.cjs');
const { run } = require('../lan-checks/processes.cjs');
const workspace = require('../workspace-paths.cjs');

const tools = 'src/godot-game/tools';
const assemblyPath = 'src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll';
const testOptions = ['--no-ansi', '--progress', 'off', '--parallel', 'none', '--timeout', '60s'];
const repositoryMethods = [
  'GraphTests.Evaluate_DeclaredProjectsAndTypes_Accepted',
  'DocumentationTests.Check_RepositoryIndex_ResolvesAuthority',
  'DocumentationProjectTests.Check_ActualProjectPages_MatchesEvaluatedReferences',
  'ArchitectureRuleTests.Rule34_DeclaredVisibility_Accepted',
].map(method => 'KingdomZombies.Phase1.Tests.' + method);
const repositoryControls = ['basic.test.cjs', 'catalog.test.cjs', 'runner.test.cjs']
  .map(file => tools + '/game-tests/' + file);
const operations = ['basic.strict-build', 'basic.repository', 'basic.pure', 'basic.headless'];

function save(file, value) {
  fs.writeFileSync(file, JSON.stringify(value, null, 2) + '\n', { flag: 'wx' });
}

async function command(context, folder, name, executable, args, timeout) {
  const child = launch(context.root, executable, args);
  const started = performance.now();
  let record;
  try {
    record = await run(executable, child.args, {
      cwd: child.cwd,
      env: context.env,
      stdio: ['ignore', 'pipe', 'pipe'],
    }, timeout);
    return record;
  } catch (error) {
    record = error.record ?? { message: error.message };
    const detail = [record.stderr, record.stdout].filter(Boolean).join('\n').trim().slice(-1000);
    throw new Error(name + ': ' + error.message + (detail ? '\n' + detail : ''), { cause: error });
  } finally {
    save(path.join(folder, name + '.json'), {
      executable, args: child.args, cwd: child.cwd, durationMs: Math.round(performance.now() - started), ...record,
    });
  }
}

async function prepare(context, folder) {
  await command(context, folder, 'toolchain', process.execPath, [tools + '/verification/bootstrap.cjs', '--prepare'], 240_000);
  await command(context, folder, 'restore', 'dotnet', [
    'restore', 'KingdomZombies.slnx', '--locked-mode', '--disable-parallel', '-p:NuGetAudit=false',
  ], 240_000);
  await command(context, folder, 'build', 'dotnet', [
    'build', 'KingdomZombies.slnx', '--no-restore', '--disable-build-servers', '-p:UseSharedCompilation=false',
    '-nodeReuse:false', '-p:NuGetAudit=false', '-v:q',
  ], 240_000);
}

function discoveredTests(output) {
  const { schemaVersion, tests } = JSON.parse(output);
  assert.equal(schemaVersion, 1, 'TEST_DISCOVERY_SCHEMA');
  assert.ok(Array.isArray(tests) && tests.length > 0, 'TEST_DISCOVERY_EMPTY');
  const uids = new Set();
  const names = new Set();
  for (const test of tests) {
    assert.match(test.uid, /^[0-9a-f]{64}$/, 'TEST_DISCOVERY_UID');
    assert.equal(typeof test.displayName, 'string', 'TEST_DISCOVERY_NAME');
    assert.ok(test.displayName.trim() && !uids.has(test.uid) && !names.has(test.displayName), 'TEST_DISCOVERY_DUPLICATE');
    uids.add(test.uid);
    names.add(test.displayName);
  }
  return tests;
}

function verifyTestResults(tests, result) {
  const { summary, tests: results } = result.results;
  assert.equal(summary.tests, tests.length, 'TEST_RESULT_COUNT');
  assert.equal(summary.passed, tests.length, 'TEST_PASS_COUNT');
  for (const key of ['failed', 'pending', 'skipped', 'other']) {
    assert.equal(summary[key], 0, 'TEST_NON_PASS ' + key);
  }
  assert.ok(Array.isArray(results) && results.every(record => record.status === 'passed'), 'TEST_RESULT_STATUS');
  assert.deepEqual(results.map(record => record.name).sort(), tests.map(record => record.displayName).sort(),
    'TEST_RESULT_MEMBERSHIP');
}

async function xunit(context, folder, filter, expected = null) {
  const assembly = path.join(context.root, assemblyPath);
  const discovery = await command(context, folder, 'discovery', 'dotnet', [
    assembly, ...filter, '--list-tests', 'json',
  ], 30_000);
  const tests = discoveredTests(discovery.stdout);
  if (expected) {
    assert.deepEqual(tests.map(test => test.displayName).sort(), [...expected].sort(), 'REPOSITORY_TEST_MEMBERSHIP');
  }
  await command(context, folder, 'execution', 'dotnet', [
    assembly, ...filter, ...testOptions, '--report-xunit-ctrf', '--report-xunit-ctrf-filename', 'tests.json',
    '--results-directory', folder,
  ], 120_000);
  verifyTestResults(tests, JSON.parse(fs.readFileSync(path.join(folder, 'tests.json'), 'utf8')));
}

function artifacts(directory, folder) {
  return fs.readdirSync(folder, { recursive: true, withFileTypes: true })
    .filter(entry => entry.isFile() && entry.name !== 'profiles.lock')
    .map(entry => path.relative(directory, path.join(entry.parentPath, entry.name)).split(path.sep).join('/'))
    .sort();
}

async function executeBasic(context, ids) {
  assert.ok(ids.length > 0 && new Set(ids).size === ids.length && ids.every(id => operations.includes(id)),
    'BASIC_SELECTION');
  const env = workspace.isolationEnv(context.root, {
    DOTNET_CLI_TELEMETRY_OPTOUT: '1', DOTNET_NOLOGO: '1', PHASE1_ROOT: context.root,
  });
  context = { ...context, env };
  const folders = new Map(ids.map(id => [id, path.join(context.directory, id)]));
  for (const folder of folders.values()) fs.mkdirSync(folder);
  const build = 'basic.strict-build';
  const preparationFolder = folders.get(build) ?? folders.values().next().value;
  const preparing = performance.now();
  let preparationError;
  try {
    await prepare(context, preparationFolder);
  } catch (error) {
    preparationError = error;
  }
  const preparationDurationMs = Math.round(performance.now() - preparing);

  const results = [];
  for (const id of ids) {
    const folder = folders.get(id);
    const started = performance.now();
    let status = 'pass';
    let message;
    if (preparationError) {
      status = folder === preparationFolder ? 'fail' : 'blocked';
      message = preparationError.message;
    } else {
      try {
        if (id === 'basic.repository') {
          await command(context, folder, 'runner-controls', process.execPath, ['--test', ...repositoryControls], 90_000);
          await xunit(context, folder, ['--filter-method', ...repositoryMethods], repositoryMethods);
        } else if (id === 'basic.pure') {
          await xunit(context, folder, ['--filter-trait', 'Tier=1']);
        } else if (id === 'basic.headless') {
          await require('./headless.cjs').smoke(context, folder);
        }
      } catch (error) {
        status = 'fail';
        message = error.message;
      }
    }
    const timing = { durationMs: Math.round(performance.now() - started) };
    if (folder === preparationFolder) timing.preparationDurationMs = preparationDurationMs;
    const result = { id, status, message, ...timing };
    save(path.join(folder, 'result.json'), result);
    results.push({ ...result, artifacts: artifacts(context.directory, folder) });
  }
  return results;
}

module.exports = { executeBasic, discoveredTests, verifyTestResults, assemblyPath };
