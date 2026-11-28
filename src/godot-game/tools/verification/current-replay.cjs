'use strict';
// Derives docs/code/engine-generated-spans.json currentProducerReplay from the current game sources.
//   node src/godot-game/tools/verification/current-replay.cjs [--check] | --write
// The replay keeps its cohort's historical inputs first, by exact recorded path and order, with the current bytes'
// SHA-256. Game C# sources the cohort does not list follow in ordinal order of their recorded (logical) path. Every
// changed historical input and every added input is carried as a snapshot. EngineGeneratedTests.VerifyCurrentReplay
// enforces the same shape; this tool never edits the cohort, its rows or its observations.
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {spawnSync} = require('node:child_process');
const layout = require('../asset-pipeline/layout.cjs');

const ROOT = path.resolve(__dirname, '../../../..');
const REGISTRY = 'docs/code/engine-generated-spans.json';
const GAME = 'src/godot-game/game';
const ADAPTERS = new Set([GAME + '/GeneratedCallSiteAdapters.cs', GAME + '/GeneratedReportAdapter.cs']);

class ReplayError extends Error {}

function sha256(bytes) {
  return crypto.createHash('sha256').update(bytes).digest('hex');
}

/** Mirrors EngineGeneratedTests.CurrentSourcePaths: game C# files outside bin, obj and .godot, minus adapters. */
function currentSources(root) {
  const found = [];
  const walk = relative => {
    for (const entry of fs.readdirSync(path.join(root, relative), {withFileTypes: true})) {
      const child = relative + '/' + entry.name;
      if (entry.isDirectory()) {
        if (!['bin', 'obj', '.godot'].includes(entry.name)) walk(child);
      } else if (entry.name.endsWith('.cs') && !ADAPTERS.has(child)) found.push(child);
    }
  };
  walk(GAME);
  return found.sort();
}

/** Refuses sources that are untracked or differ from HEAD, so the replay always names committed bytes. */
function requireCommitted(root, files) {
  const result = spawnSync('git', ['status', '--porcelain=v1', '--untracked-files=all', '--', ...files],
    {cwd: root, encoding: 'utf8', windowsHide: true, maxBuffer: 1 << 26});
  if (result.status !== 0) throw new ReplayError('git status failed: ' + String(result.stderr).trim());
  const dirty = result.stdout.split('\n').filter(line => line.trim());
  if (dirty.length) throw new ReplayError('commit these sources first: ' + dirty.slice(0, 8).join(' | '));
}

function derive(root, registry) {
  const replay = registry.currentProducerReplay;
  if (!replay) throw new ReplayError('no currentProducerReplay in ' + REGISTRY);
  const cohorts = registry.producerInventories.filter(row => row.id === replay.cohort);
  if (cohorts.length !== 1) throw new ReplayError('cohort ' + replay.cohort + ' must exist exactly once');
  const historical = cohorts[0].sourceInputs;
  const bytes = new Map();
  const read = recorded => {
    const file = layout.physical(recorded);
    if (!fs.existsSync(path.join(root, file))) throw new ReplayError('historical input missing: ' + recorded);
    bytes.set(recorded, fs.readFileSync(path.join(root, file)));
    return {path: recorded, sha256: sha256(bytes.get(recorded))};
  };
  const originals = historical.map(row => read(row.path));
  const known = new Set(historical.map(row => layout.physical(row.path)));
  const sources = currentSources(root);
  requireCommitted(root, [...historical.map(row => layout.physical(row.path)), ...sources]);
  const added = sources.filter(file => !known.has(file)).map(file => layout.logical(file))
    .sort((left, right) => (left < right ? -1 : left > right ? 1 : 0)).map(read);
  const inputs = [...originals, ...added];
  const snapshots = [...originals.filter((row, index) => row.sha256 !== historical[index].sha256), ...added]
    .map(row => ({path: row.path, sha256: row.sha256, contentBase64: bytes.get(row.path).toString('base64')}));
  return {...replay, sourceInputs: inputs, sourceInputsSha256: sha256(JSON.stringify(inputs)),
    sourceSnapshots: snapshots, summary: {historical: historical.length, changed: snapshots.length - added.length,
      added: added.map(row => row.path)}};
}

function run(argv, root = ROOT) {
  const mode = argv.length === 0 || (argv.length === 1 && argv[0] === '--check') ? 'check'
    : argv.length === 1 && argv[0] === '--write' ? 'write' : null;
  if (!mode) throw new ReplayError('usage: node current-replay.cjs [--check] | --write');
  const file = path.join(root, REGISTRY);
  const text = fs.readFileSync(file, 'utf8');
  const registry = JSON.parse(text);
  if (JSON.stringify(registry, null, 2) + '\n' !== text) throw new ReplayError(REGISTRY + ' is not canonical JSON');
  const {summary, ...derived} = derive(root, registry);
  const counts = summary.historical + ' historical, ' + summary.changed + ' changed, ' + summary.added.length
    + ' added; sourceInputsSha256 ' + derived.sourceInputsSha256;
  if (JSON.stringify(derived) === JSON.stringify(registry.currentProducerReplay)) return 'PASS current-replay ' + counts;
  if (mode === 'check') throw new ReplayError('currentProducerReplay differs from current sources (' + counts + ')');
  registry.currentProducerReplay = derived;
  fs.writeFileSync(file, JSON.stringify(registry, null, 2) + '\n');
  return 'WROTE current-replay ' + counts + '\nadded: ' + summary.added.join(', ');
}

if (require.main === module) {
  try {
    console.log(run(process.argv.slice(2)));
  } catch (error) {
    if (!(error instanceof ReplayError)) throw error;
    console.error('FAIL current-replay: ' + error.message);
    process.exitCode = 1;
  }
}

module.exports = {run, derive, currentSources, ReplayError};
