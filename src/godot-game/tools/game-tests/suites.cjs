'use strict';

const fs = require('node:fs');
const path = require('node:path');
const {randomUUID} = require('node:crypto');
const workspace = require('../workspace-paths.cjs');
const {run} = require('../lan-checks/processes.cjs');
const {exact, reconcile, relative, requiredFile, writeJson} = require('./report.cjs');

const tools = 'src/godot-game/tools';
const limits = {
  'gameplay-checks': 70 * 60_000,
  'lan-checks': 35 * 60_000,
  'asset-pipeline': 90 * 60_000,
  verification: 2 * 60 * 60_000
};

function tasks(selected) {
  const batches = new Map();
  for (const row of selected) {
    const key = row.adapter === 'basic' ? 'basic'
      : row.adapter === 'verification' ? row.id
      : row.adapter === 'scenario' ? row.runGroup
      : row.adapter + '.' + (Number(row.tier) === 3 ? 'rendered' : 'headless');
    if (!batches.has(key)) batches.set(key, []);
    batches.get(key).push(row);
  }
  return [...batches].sort(([left], [right]) => Number(right === 'basic') - Number(left === 'basic'));
}

function childArgs(adapter, rows) {
  const ids = rows.flatMap(row => row.legacyIds ?? [row.legacyId]);
  if (!ids.length || ids.some(id => !id)) throw new Error('Missing case identities for ' + adapter);
  return [tools + '/' + adapter + '/check.cjs', adapter === 'verification' ? '--cases' : '--case', ids.join(',')];
}

async function command(context, executable, args, timeout) {
  const file = 'child-' + randomUUID() + '.json';
  let record;
  try {
    record = await run(executable, args, {cwd: context.root, env: process.env, onOutput: text => process.stdout.write(text)}, timeout);
  } catch (error) {
    record = error.record ?? {executable, args, error: error.message};
  }
  const log = writeJson(context.directory, file, record);
  return {record, log: relative(context.directory, log)};
}

function readLegacy(context, adapter, file, started) {
  const checkout = workspace.locations(context.root).runs(adapter);
  const owned = requiredFile(checkout, path.resolve(context.root, file));
  if (fs.statSync(owned).mtimeMs < started) throw new Error('Legacy report predates shared run: ' + owned);
  const report = JSON.parse(fs.readFileSync(owned, 'utf8'));
  if (report.revision !== context.revision) throw new Error('Legacy report revision mismatch: ' + owned);
  return {report, path: relative(context.root, owned)};
}

function blocked(rows, error, artifacts = []) {
  return rows.map(row => ({id: row.id, status: 'blocked', message: error.message, artifacts}));
}

async function legacy(context, adapter, rows) {
  const args = childArgs(adapter, rows);
  if (adapter === 'asset-pipeline' && Number(rows[0].tier) === 3) args.splice(1, 2, '--phase5-hub-smoke');
  const started = Date.now();
  const {record, log} = await command(context, process.execPath, args, limits[adapter]);
  if (!record.streamsClosed || record.timedOut || record.spawnError || record.signal) {
    return blocked(rows, new Error(record.timedOut ? 'Child process timed out' : record.spawnError ?? 'Child did not complete'), [log]);
  }
  try {
    const output = record.stdout.split(/\r?\n/);
    if (adapter === 'asset-pipeline') return assetResults(context, rows, output, record, log, started);
    const summary = output.findLast(line => line.includes('selected obligations;') || /^(PASS|FAIL) \d+\/\d+;/.test(line));
    const filename = summary?.match(/\[summary\]\((.*)\)$/)?.[1] ?? summary?.split('; ').at(-1);
    if (!filename) throw new Error('Child did not publish a summary.');
    const {report, path: reportPath} = readLegacy(context, adapter, filename, started);
    exact(rows.flatMap(row => row.legacyIds ?? [row.legacyId]), report.selected);
    const details = [...report.results, ...(adapter === 'verification' ? report.failures.filter(row => row.id !== 'inputs') : [])];
    exact(report.selected, details.map(row => row.id));
    return rows.map(row => {
      const members = details.filter(result => (row.legacyIds ?? [row.legacyId]).includes(result.id));
      const failure = members.find(result => result.status !== 'pass');
      const status = failure?.status ?? (record.code === 0 && report.status === 'pass' ? 'pass' : 'fail');
      if (!failure && members.every(result => result.status === 'pass') && !report.failures?.some(result => result.id === 'inputs')
          && details.some(result => result.status !== 'pass')) return {id: row.id, status: 'pass', artifacts: [log], legacyReport: reportPath, members};
      return {id: row.id, status, message: status === 'pass' ? undefined : failure?.message ?? failure?.diagnostic ?? 'Suite reported non-success',
        artifacts: [log], legacyReport: reportPath, members, launchCount: report.launches?.length ?? Number(members.some(result => result.nativeLog))};
    });
  } catch (error) {
    return blocked(rows, error, [log]);
  }
}

function assetResults(context, rows, output, record, log, started) {
  if (Number(rows[0].tier) === 3) {
    exact(['rendered.hub-navigation'], rows.map(row => row.legacyId));
    const stdout = record.stdout.trim();
    const first = stdout.lastIndexOf('\n{');
    const report = JSON.parse(first < 0 ? stdout : stdout.slice(first + 1));
    if (report.id !== 'rendered.hub-smoke' || report.revision !== context.revision) throw new Error('Asset smoke identity mismatch.');
    const assetRoot = workspace.locations(context.root).runs('asset-pipeline');
    const directory = path.resolve(context.root, report.directory);
    relative(assetRoot, directory);
    const verified = readLegacy(context, 'asset-pipeline', report.report, started);
    const checkpoints = verified.report.checkpoints ?? [];
    const judgments = report.judgments ?? [];
    if (record.code === 0 && report.status === 'pass') {
      exact(['P1', 'P2', 'P3', 'P4'], judgments.map(row => row.id));
      if (checkpoints.length < 8) throw new Error('Inspection category evidence missing.');
      const imageValidator = require('../lan-checks/png.cjs').validate;
      for (const judgment of judgments) {
        if (judgment.status !== 'pending-review') throw new Error('Inspection judgment evidence missing: ' + judgment.id);
        exact(judgment.expected, judgment.images.map(image => image.checkpoint));
        for (const {image} of judgment.images) {
          const file = requiredFile(directory, path.resolve(context.root, image.path));
          imageValidator(file, 1280, 720);
        }
      }
    }
    return [{id: rows[0].id, status: record.code === 0 && report.status === 'pass' ? 'pass' : 'fail',
      message: report.message, artifacts: [log], legacyReport: verified.path, launchCount: report.launched, checkpoints, judgments}];
  }
  const receipts = output.map(line => line.match(/^(?:PASS|FAIL) ([A-Za-z0-9._-]+) (.+)$/)).filter(Boolean);
  exact(rows.map(row => row.legacyId), receipts.map(match => match[1]));
  return rows.map(row => {
    const directory = receipts.find(match => match[1] === row.legacyId)[2];
    const {report, path: reportPath} = readLegacy(context, 'asset-pipeline', path.join(directory, 'result.json'), started);
    if (report.id !== row.legacyId && report.caseId !== row.legacyId) throw new Error('Asset case identity mismatch: ' + row.legacyId);
    const status = report.status === 'pass' && record.code !== 0 && receipts.every(match => match[0].startsWith('PASS '))
      ? 'fail' : report.status;
    return {id: row.id, status, message: report.message, artifacts: [log], legacyReport: reportPath};
  });
}

async function renderedLan(context, rows) {
  const nonce = randomUUID();
  const directory = path.join(context.directory, nonce);
  fs.mkdirSync(directory);
  fs.writeFileSync(path.join(directory, 'observation.owner'), nonce, {flag: 'wx'});
  const {createIdentity, environment, select} = require('../lan-checks/check.cjs');
  const catalog = require('../lan-checks/required-cases.json');
  const {selected} = select(catalog, ['--case', rows.map(row => row.legacyId).join(',')]);
  const suite = {root: context.root, directory, nonce, ...createIdentity(context.root), buildRevision: context.revision,
    env: environment(context.root)};
  const {results, launches} = await require('../lan-checks/scenarios.cjs').run(suite, selected);
  const check = require('../lan-checks/evidence.cjs');
  exact(selected.map(row => row.id), results.map(row => row.id));
  for (const result of results.filter(row => row.status === 'pass')) {
    try {
      check.reconcile([selected.find(row => row.id === result.id)], [result], suite);
    } catch (error) {
      result.status = 'fail';
      result.message = error.message;
    }
  }
  const report = relative(context.directory, writeJson(directory, 'summary.json', {nonce, revision: context.revision, results, launches}));
  return rows.map(row => {
    const result = results.find(result => result.id === row.legacyId);
    return {id: row.id, status: result.status, message: result.message, artifacts: [report], launchCount: launches.length, observations: result};
  });
}

async function scenarioGroup(context, rows) {
  const directory = path.join(context.directory, randomUUID());
  fs.mkdirSync(directory);
  const plan = writeJson(directory, 'plan.json', {context: {...context, directory}, rows});
  const {record, log} = await command(context, process.execPath, [tools + '/game-tests/scenario-group.cjs', plan], 20 * 60_000);
  const outcome = path.join(directory, 'results.json');
  if (!record.streamsClosed || record.timedOut || !fs.existsSync(outcome)) {
    return blocked(rows, new Error(record.timedOut ? 'Scenario group timed out' : 'Scenario group did not publish results'), [log]);
  }
  const results = JSON.parse(fs.readFileSync(outcome, 'utf8'));
  exact(rows.map(row => row.id), results.map(row => row.id));
  if (record.code !== 0 && results.every(row => row.status === 'pass')) {
    return blocked(rows, new Error('Scenario group exited nonzero'), [log]);
  }
  const prefix = relative(context.directory, directory);
  return results.map(row => ({...row, artifacts: [...(row.artifacts ?? []).map(file => prefix + '/' + file), log]}));
}

async function runGroups(context, selected) {
  const results = [];
  const groups = [];
  const batches = tasks(selected);
  let preparation;
  if (batches[0]?.[0] !== 'basic') {
    try {
      const {results: bootstrap, prepared} = await require('./basic.cjs').executeBasic(context, ['basic.strict-build']);
      preparation = {prepared, result: bootstrap[0]};
    } catch (error) {
      preparation = {prepared: false, message: error.message};
    }
  }
  for (const [id, rows] of batches) {
    const started = Date.now();
    let groupResults;
    try {
      if (id === 'basic') {
        const execution = await require('./basic.cjs').executeBasic(context, rows.map(row => row.id));
        groupResults = execution.results;
        preparation = {prepared: execution.prepared};
      } else if (!preparation?.prepared) {
        groupResults = blocked(rows, new Error('The current-source build prerequisite failed.'));
      } else if (id === 'lan-checks.rendered') groupResults = await renderedLan(context, rows);
      else if (rows[0].adapter === 'scenario') groupResults = await scenarioGroup(context, rows);
      else groupResults = await legacy(context, rows[0].adapter, rows);
      reconcile(context.directory, rows, groupResults);
    } catch (error) {
      groupResults = blocked(rows, error);
    }
    results.push(...groupResults);
    groups.push({id, ids: rows.map(row => row.id), durationMilliseconds: Date.now() - started,
      launchCount: Math.max(0, ...groupResults.map(row => row.launchCount ?? 0))});
  }
  return {results, groups, preparation};
}

module.exports = {runGroups, tasks};
