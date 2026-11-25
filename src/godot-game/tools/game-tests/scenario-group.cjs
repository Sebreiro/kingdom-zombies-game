'use strict';

const fs = require('node:fs');
const path = require('node:path');
const {exact, relative, requiredFile, writeJson} = require('./report.cjs');

const rootPath = 'src/godot-game/tests/scenarios/';
const driverPath = /^src\/godot-game\/tests\/scenarios\/(?:[\w.-]+\/)*[\w.-]+\.cjs$/;

// Every launched game process is one record in the driver state's peers or launches. A relaunch is a new record.
function launchedProcesses(state) {
  const records = new Set([...(state?.peers ?? []), ...(state?.launches ?? [])]);
  return [...records].filter(record => Number.isSafeInteger(record?.pid) && record.pid > 0).length;
}

const elapsed = started => Math.max(0, Math.round(performance.now() - started));

async function runScenarios(context, rows, driver) {
  exact(rows.map(row => row.id), Object.keys(driver.cases ?? {}).filter(id => rows.some(row => row.id === id)));
  const ordered = [...rows].sort((a, b) => a.order - b.order);
  const results = [];
  const state = await driver.open(context);
  let blocked;
  try {
    for (const row of ordered) {
      if (blocked) {
        results.push({id: row.id, status: 'blocked', message: blocked, durationMs: 0});
        continue;
      }
      const started = performance.now();
      try {
        const evidence = await driver.cases[row.id](state, context);
        results.push({id: row.id, status: 'pass', artifacts: evidence, durationMs: elapsed(started)});
      } catch (error) {
        results.push({id: row.id, status: 'fail', message: error.message, durationMs: elapsed(started)});
      }
      if (row !== ordered.at(-1)) {
        try {
          await driver.reset(state, context);
        } catch (error) {
          blocked = 'Scenario reset failed: ' + error.message;
        }
      }
    }
  } finally {
    try {
      await driver.close(state, context);
    } catch (error) {
      const final = results.at(-1);
      const cleanup = 'Owned scenario cleanup failed: ' + error.message;
      final.status = 'fail';
      final.message = final.message ? final.message + '\n' + cleanup : cleanup;
    }
    const launchCount = launchedProcesses(state);
    for (const result of results) result.launchCount = launchCount;
  }
  return results;
}

async function main(planFile) {
  const {context, rows} = JSON.parse(fs.readFileSync(planFile, 'utf8'));
  const entry = rows[0]?.driver;
  if (!driverPath.test(entry) || rows.some(row => row.driver !== entry)) throw new Error('Invalid scenario driver path.');
  const driver = require(requiredFile(path.join(context.root, rootPath), path.resolve(context.root, entry)));
  const unlock = rows.some(row => row.tier === 3)
    ? require('../gameplay-checks/check.cjs').acquireLock(context.root, context.invocation) : () => {};
  let results;
  try {
    results = await runScenarios(context, rows, driver);
  } finally {
    unlock();
  }
  // Recheck every evidence file against the physical group root before serving it.
  for (const result of results.filter(row => row.status === 'pass')) {
    try {
      if (!Array.isArray(result.artifacts) || !result.artifacts.length) throw new Error('Scenario evidence missing.');
      result.artifacts = result.artifacts.map(file =>
        relative(context.directory, requiredFile(context.directory, path.resolve(context.directory, file))));
    } catch (error) {
      result.status = 'fail';
      result.message = error.message;
    }
  }
  writeJson(context.directory, 'results.json', results);
  if (results.some(row => row.status !== 'pass')) process.exitCode = 1;
}

module.exports = {runScenarios, launchedProcesses, driverPath};
if (require.main === module) {
  main(process.argv[2]).catch(error => {
    console.error(error.message);
    process.exitCode = 1;
  });
}
