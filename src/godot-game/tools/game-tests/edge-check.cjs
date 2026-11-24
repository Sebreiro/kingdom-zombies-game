'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { launch } = require('../dotnet.cjs');
const { run } = require('../lan-checks/processes.cjs');
const workspace = require('../workspace-paths.cjs');
const { assemblyPath, discoveredTests } = require('./basic.cjs');

function specificationEdges(markdown) {
  const lines = markdown.split(/\r?\n/);
  const section = lines.indexOf('## Edge Coverage');
  assert.notEqual(section, -1, 'EDGE_SPEC_SECTION_MISSING');
  const edges = new Map();
  for (const line of lines.slice(section + 1)) {
    if (line.startsWith('## ')) break;
    const row = /^\| (E\d{3,}) \| [^|]+ \| [^|]+ \| ([^|]+) \|/.exec(line);
    if (!row) continue;
    const [, id, disposition] = row;
    assert.ok(!edges.has(id), 'EDGE_SPEC_DUPLICATE ' + id);
    assert.ok(['resolved', 'deferred'].includes(disposition.trim()), 'EDGE_SPEC_DISPOSITION ' + id);
    edges.set(id, disposition.trim());
  }
  assert.ok([...edges.values()].includes('resolved'), 'EDGE_SPEC_EMPTY');
  return edges;
}

function verifyEdges(phase, markdown, tests) {
  const expected = specificationEdges(markdown);
  const methods = new Map();
  for (const test of tests) {
    const { namespace, typeName, methodName } = test.type ?? {};
    const match = new RegExp('^Phase' + phase + '_(E[0-9]{3,})_[A-Za-z0-9_]+$').exec(methodName ?? '');
    if (!match) continue;
    const [, id] = match;
    const method = [namespace, typeName, methodName].join('.');
    assert.ok(expected.get(id) === 'resolved', 'EDGE_TEST_UNEXPECTED ' + id);
    assert.ok(!methods.has(id) || methods.get(id) === method, 'EDGE_TEST_DUPLICATE ' + id);
    methods.set(id, method);
  }
  const missing = [...expected].filter(([id, status]) => status === 'resolved' && !methods.has(id)).map(([id]) => id);
  assert.deepEqual(missing, [], 'EDGE_TEST_MISSING ' + missing.join(', '));
  return { phase: Number(phase), expected: methods.size, methods: Object.fromEntries(methods) };
}

async function main(args) {
  assert.ok(args.length === 1 && /^(?:0?[6-9]|[1-9][0-9])$/.test(args[0]), 'USAGE: node edge-check.cjs PHASE');
  const phase = args[0].padStart(2, '0');
  const root = path.resolve(__dirname, '../../../..');
  const phases = path.join(root, '.planning/phases');
  const folders = fs.readdirSync(phases).filter(name => name.startsWith(phase + '-'));
  assert.equal(folders.length, 1, 'EDGE_PHASE_UNKNOWN ' + phase);
  const spec = fs.readFileSync(path.join(phases, folders[0], phase + '-SPEC.md'), 'utf8');
  const subject = launch(root, 'dotnet', [
    path.join(root, assemblyPath), '--filter-trait', 'Tier=1', '--list-tests', 'json',
  ]);
  const record = await run('dotnet', subject.args, {
    cwd: subject.cwd,
    env: { ...workspace.isolationEnv(root), PHASE1_ROOT: root },
    stdio: ['ignore', 'pipe', 'pipe'],
  }, 30_000);
  console.log(JSON.stringify(verifyEdges(phase, spec, discoveredTests(record.stdout)), null, 2));
}

if (require.main === module) main(process.argv.slice(2)).catch(error => {
  console.error(error.message);
  process.exitCode = 1;
});
module.exports = { specificationEdges, verifyEdges };
