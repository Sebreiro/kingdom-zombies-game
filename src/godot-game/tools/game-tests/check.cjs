'use strict';

const fs = require('node:fs');
const path = require('node:path');
const os = require('node:os');
const {randomUUID} = require('node:crypto');
const {execFileSync} = require('node:child_process');
const workspace = require('../workspace-paths.cjs');
const {runGroups} = require('./suites.cjs');
const {loadCatalog, select} = require('./catalog.cjs');
const {reconcile, writeJson, relative} = require('./report.cjs');

const entry = 'src/godot-game/tools/game-tests/check.cjs';
const usage = 'Use --list, --pr, --full, --phase-end, --case ID[,ID], --group GROUP, --tier N or --failed REPORT. '
  + 'Use --review FILE for rendered selections. With --phase-end, use --base REF and --phase N to override the defaults.';

function parse(args) {
  const modes = new Set(['list', 'pr', 'full', 'phase-end', 'case', 'group', 'tier', 'failed']);
  const valued = new Set(['case', 'group', 'tier', 'failed', 'base', 'review', 'phase']);
  const options = {};
  for (let i = 0; i < args.length; i++) {
    const key = args[i].startsWith('--') && args[i].slice(2);
    if (!modes.has(key) && !['base', 'review', 'phase'].includes(key)) throw new Error(usage);
    const value = valued.has(key) ? args[++i] : true;
    if (!value || typeof value === 'string' && value.startsWith('-')) throw new Error(usage);
    if (modes.has(key)) {
      if (options.mode) throw new Error(usage);
      options.mode = key;
      if (value !== true) options.value = value;
    } else {
      if (options[key]) throw new Error(usage);
      options[key] = value;
    }
  }
  if (!options.mode || (options.base || options.phase) && options.mode !== 'phase-end') throw new Error(usage);
  if (options.phase && !/^[1-9][0-9]*$/.test(options.phase)) throw new Error(usage);
  if (options.mode === 'tier' && !/^[123]$/.test(options.value)) throw new Error(usage);
  return options;
}

function git(root, ...args) {
  return execFileSync('git', args, {cwd: root, encoding: 'utf8', windowsHide: true, timeout: 30000});
}

function source(root) {
  return {
    revision: git(root, 'rev-parse', 'HEAD').trim(),
    changes: git(root, 'status', '--porcelain=v1', '-z', '--untracked-files=all')
  };
}

function changedPaths(root, base) {
  const commit = git(root, 'rev-parse', '--verify', '--end-of-options', base + '^{commit}').trim();
  const baseCommit = git(root, 'merge-base', commit, 'HEAD').trim();
  const nul = text => text.split('\0').filter(Boolean);
  return {
    baseCommit,
    changes: [...new Set([
      ...nul(git(root, 'diff', '--name-only', '--no-renames', '-z', baseCommit, '--')),
      ...nul(git(root, 'ls-files', '--others', '--exclude-standard', '-z'))
    ])].sort()
  };
}

function codeReview(root, file, tested) {
  if (!file || tested.changes.length) throw new Error('Rendered groups require a clean source and --review with a completed code review.');
  const review = JSON.parse(fs.readFileSync(path.resolve(root, file), 'utf8'));
  if (review.status !== 'clean' || review.revision !== tested.revision || !review.reviewer?.trim()) {
    throw new Error('Rendered code review must be clean and identify the exact tested revision and reviewer.');
  }
  return review;
}

function repairOptions(root, filename, catalog) {
  const runs = workspace.locations(root).runs('game-tests');
  const file = fs.realpathSync(path.resolve(root, filename));
  relative(runs, file);
  const previous = JSON.parse(fs.readFileSync(file, 'utf8'));
  if (previous.schemaVersion !== 1 || path.basename(path.dirname(file)) !== previous.invocation) {
    throw new Error('This report does not identify a shared test run.');
  }
  const ids = previous.results.filter(row => row.status === 'fail' || row.status === 'blocked').map(row => row.id);
  return {selection: select(catalog, {mode: 'case', value: ids.join(',')}), repairOf: previous.invocation};
}

async function main(root, args) {
  const options = parse(args);
  const catalog = loadCatalog(root);
  if (options.mode === 'list') {
    console.log(JSON.stringify(catalog, null, 2));
    return;
  }

  const tested = source(root);
  const impact = options.mode === 'phase-end' ? changedPaths(root, options.base ?? 'origin/dev') : {};
  const {selection, repairOf} = options.mode === 'failed' ? repairOptions(root, options.value, catalog)
    : {selection: select(catalog, {...options, ...impact, value: options.phase ?? options.value})};
  const {selected, omitted} = selection;
  if (!selected.length) throw new Error('No required cases selected.');
  const review = selected.some(row => Number(row.tier) === 3) ? codeReview(root, options.review, tested) : undefined;

  const invocation = randomUUID();
  const directory = workspace.locations(root).runs('game-tests', invocation);
  fs.mkdirSync(path.dirname(directory), {recursive: true});
  fs.mkdirSync(directory);
  const context = {root, directory, invocation, revision: tested.revision, review};
  const started = Date.now();
  const plan = {schemaVersion: 1, invocation, revision: tested.revision, clean: tested.changes.length === 0,
    os: {platform: process.platform, architecture: process.arch, release: os.release(), version: os.version()},
    tools: {node: process.version, pins: require('../verification/toolchain.json')}, startedAt: new Date(started).toISOString(),
    mode: options.mode, phase: options.mode === 'phase-end' ? Number(options.phase ?? 5) : undefined, baseCommit: impact.baseCommit, changes: impact.changes, selected, omitted, repairOf, review,
    command: [process.execPath, entry, ...args]};
  writeJson(directory, 'selection.json', plan);
  console.log('Selected ' + selected.length + ' required checks. See ' + path.join(directory, 'selection.json'));
  const {results, groups, preparation} = await runGroups(context, selected);
  const actual = source(root);
  const sourceChanged = actual.revision !== tested.revision || actual.changes !== tested.changes;
  if (sourceChanged) {
    for (const result of results.filter(row => row.status === 'pass')) {
      result.status = 'fail';
      result.message = 'Source changed during execution.';
    }
  }
  reconcile(directory, selected, results);
  for (const result of results.filter(row => row.status !== 'pass')) {
    result.replay = [process.execPath, entry, '--case', result.id, ...(options.review ? ['--review', options.review] : [])];
    console.error(result.status.toUpperCase() + ' ' + result.id + ' ' + result.message);
  }
  const pendingReview = results.flatMap(row => row.judgments ?? []).filter(row => row.status === 'pending-review');
  const status = results.some(row => row.status !== 'pass') ? 'fail' : pendingReview.length ? 'needs_review' : 'pass';
  const report = {...plan, status, pendingReview, sourceChanged, preparation, groups, results, finishedAt: new Date().toISOString(),
    durationMilliseconds: Date.now() - started};
  writeJson(directory, 'summary.json', report);
  console.log(status.toUpperCase() + ' ' + results.filter(row => row.status === 'pass').length + '/' + selected.length
    + ' objective checks. Report ' + path.join(directory, 'summary.json'));
  if (status !== 'pass') process.exitCode = 1;
}

module.exports = {main, parse, changedPaths, codeReview};
if (require.main === module) {
  main(workspace.checkoutRoot(), process.argv.slice(2)).catch(error => {
    console.error(error.message);
    process.exitCode = 1;
  });
}
