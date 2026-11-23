'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
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
