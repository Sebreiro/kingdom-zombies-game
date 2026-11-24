'use strict';

const crypto = require('node:crypto');
const path = require('node:path');
const content = require('./content.cjs');
const evidence = require('./evidence.cjs');
const contracts = require('./rendered-contracts.cjs');
const processes = require('./rendered-process.cjs');

const ID = 'rendered.hub-smoke';
const HUB = '/root/Inspection';
const SUMMARY = HUB + '/Summary';
const WORKSPACE = HUB + '/Shell/Workspace';
const CATEGORY = WORKSPACE + '/Navigation/Category';
const BROWSER = WORKSPACE + '/Controls/Browser';
const SEARCH = BROWSER + '/Search';
const RESOURCES = BROWSER + '/Resources';
const DETAILS = WORKSPACE + '/Preview/Actions/Details';
const METADATA = WORKSPACE + '/Preview/Metadata';
const TITLE = 'KingdomZombies — Resource inspection';
const PARTIAL = 'Partial — validated bytes; native/visual/listening acceptance pending';
const categories = [
  'Characters', 'Items', 'Buildings/world', 'UI', 'Data/text', 'Audio', 'Register', 'Known issues'
];
const equal = (node, property, value) => ({node, property, value, match:'equal'});
const contains = (node, property, value) => ({node, property, value, match:'contains'});
const ENTER = 0x0d;
const initialResources = ['AN-01','IT-01','BL-01','UI-01','DT-01','AU-01','AN-01','AN-01'];
const resourceCounts = [31,24,43,27,29,30,180,180];

async function choose(driver, node, key) {
  await driver.click(node);
  await driver.tap(key);
  await driver.tap(ENTER);
}

function category(index, id = 'category-' + index) {
  const name = categories[index];
  return {
    id, control:CATEGORY,
    checks:[
      equal(CATEGORY, 'selected', index),
      equal(CATEGORY, 'popup/item_' + index + '/text', name),
      equal(RESOURCES, 'visible', true),
      equal(RESOURCES, 'item_count', resourceCounts[index]),
      equal(SUMMARY + '/ResourceId', 'text', initialResources[index]),
      contains(SUMMARY + '/PackStatus', 'text', '180/180 source-ready · 0/180 accepted'),
      equal(SUMMARY + '/Title', 'text', TITLE),
      contains(SUMMARY + '/Status', 'text', PARTIAL)
    ],
    drive:driver => choose(driver, CATEGORY, name.charCodeAt(0))
  };
}

function checkpoints() {
  const death = contracts.CASES.get('heldout.H12').filter(row => row.id.startsWith('death-'));
  return [
    ...categories.map((_, index) => category(index)),
    {
      id:'source-details', control:DETAILS,
      checks:[equal(METADATA, 'visible', true), contains(METADATA, 'text', 'Explicit shared mappings: ')],
      drive:driver => driver.click(DETAILS)
    },
    {
      id:'close-source-details', control:DETAILS,
      checks:[equal(METADATA, 'visible', false)],
      drive:driver => driver.click(DETAILS)
    },
    category(2, 'return-to-buildings-world'),
    ...death.map(step => ({
      ...step,
      async drive(driver) {
        if (step.id === 'death-gallery') {
          await driver.click(SEARCH);
          for (const key of [0x46, 0x58, 0xbd, 0x30, 0x38]) await driver.tap(key);
          await driver.click(RESOURCES, {firstRow:true});
        } else if (step.openMenu) {
          await choose(driver, step.control, 0x45);
        } else {
          await driver.click(step.control);
        }
      }
    }))
  ];
}

const judgmentEvidence = [
  ['P1', 'Show the actual source route and pending status', ['source-details', 'category-6', 'category-7']],
  ['P2', 'Retain the KingdomZombies title and visual identity', ['category-0', 'category-3']],
  ['P3', 'Keep the inspection controls and mechanic presentation truthful', ['category-1', 'category-2', 'category-3', 'category-4']],
  ['P4', 'Keep death and blood effects restrained and free of dismemberment', ['death-step', 'death-retired']]
];

async function run() {
  const root = content.ROOT;
  const invocation = crypto.randomUUID();
  const relative = content.runsPath(invocation);
  const directory = content.inside(root, relative);
  const revision = evidence.git(root, ['rev-parse', 'HEAD']).trim();
  const planned = checkpoints();
  const result = {
    id:ID, status:'blocked', invocation, directory, revision, resolution:[1280, 720], launched:0,
    startedAt:new Date().toISOString(), expected:planned.map(step => step.id), checkpoints:[], judgments:[]
  };
  evidence.writeExclusive(root, relative + '/start.json', result);
  const scenarios = require('./scenarios.cjs');
  const gameplay = require('../gameplay-checks/rendered.cjs');
  const controller = new AbortController();
  const budget = processes.timeBudget(180000, controller.signal);
  const interrupt = signals => controller.abort(new Error('HUB_SMOKE_INTERRUPTED ' + signals));
  const signals = ['SIGINT','SIGTERM','SIGBREAK'].map(signal => [signal, () => interrupt(signal)]);
  for (const [signal, handler] of signals) process.on(signal, handler);
  let session, unlock, retainLock = false;
  try {
    evidence.need(process.platform === 'win32' && process.arch === 'x64', 'HUB_SMOKE_WINDOWS_X64');
    const clean = () => evidence.git(root, ['status', '--porcelain']).trim() === '' &&
      evidence.git(root, ['rev-parse', 'HEAD']).trim() === revision;
    evidence.need(clean(), 'HUB_SMOKE_CLEAN_COMMIT');
    const sourcePreparation = {restoreProject:true};
    const prepared = await scenarios.prepareSource(root, revision, sourcePreparation);
    const build = evidence.document(root, prepared);
    build.sourceManaged = scenarios.validateSourceProject(root, build, evidence.identifySource(root, revision),
      sourcePreparation);
    result.preparation = prepared;
    budget();
    unlock = require('../gameplay-checks/check.cjs').acquireLock(root, invocation);
    session = await scenarios.inspectionSession(root, relative + '/hub', 'G1-hub', build,
      {physical:gameplay.ownedInput, decodePng:gameplay.decodePng, dpiAwareInput:true}, budget, controller.signal);
    result.launched = 1;
    result.status = 'fail';
    for (const step of planned) {
      result.failedCheckpoint = step.id;
      result.checkpoints.push(await session.checkpoint({id:ID}, step, invocation));
    }
    delete result.failedCheckpoint;
    evidence.exact(result.expected, result.checkpoints.map(point => point.id), 'hub smoke checkpoints', true);
    const terminal = await session.close();
    session = null;
    result.process = terminal.ref;
    retainLock = terminal.record.remainingPids.length > 0;
    evidence.validateProcess(terminal.record);
    scenarios.validateProcessRuntime(root, build, terminal.record);
    evidence.need(clean(), 'HUB_SMOKE_SOURCE_CHANGED');
    result.status = 'pass';
  } catch (error) {
    retainLock ||= error.unconfirmedCleanup === true;
    result.message = error.message;
  } finally {
    if (session) {
      try {
        const terminal = await session.close();
        result.process = terminal.ref;
        retainLock = terminal.record.remainingPids.length > 0;
      } catch (error) {
        retainLock = true;
        result.cleanupError = error.message;
      }
    }
    try { if (!retainLock) unlock?.(); }
    catch (error) {
      result.status = 'fail';
      result.cleanupError = error.message;
    }
    for (const [signal, handler] of signals) process.removeListener(signal, handler);
  }
  const images = new Map(result.checkpoints.map(point => [point.id, point.image]));
  result.judgments = judgmentEvidence.map(([id, expectation, expected]) => ({
    id, expectation, expected, status:expected.every(point => images.has(point)) ? 'pending-review' : 'blocked',
    images:expected.filter(point => images.has(point)).map(checkpoint => ({checkpoint, image:images.get(checkpoint)}))
  }));
  result.finishedAt = new Date().toISOString();
  result.report = relative + '/result.json';
  evidence.writeExclusive(root, result.report, result);
  return result;
}

module.exports = {ID, checkpoints, run};
