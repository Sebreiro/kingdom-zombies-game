'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const baseline = require('../verification/check.cjs');
const { decodePng } = require('./rendered.cjs');
const hash = bytes => crypto.createHash('sha256').update(bytes).digest('hex');
function requireTrue(value, message) { if (!value) throw new Error(message); }
function read(file) { return JSON.parse(fs.readFileSync(file, 'utf8')); }
function inside(root, relative) {
  try { return baseline.inside(root,relative); }
  catch(error) {throw new Error((error.diagnostic??'PATH_INVALID')+': '+error.message);}
}
function catalog(root) {
  const value = read(path.join(root, 'src/godot-game/tools/gameplay-checks/required-cases.json'));
  requireTrue(value.requiredEnvironment==='development-os' && JSON.stringify(value.resolutions)==='[[1280,720]]','CATALOG_ENVIRONMENT');
  requireTrue(Array.isArray(value.cases) && value.cases.length === 16, 'CATALOG_REQUIRED');
  const ids = new Set();
  for (const row of value.cases) {
    requireTrue(/^(core|rendered|runner)\.[a-z]+$/.test(row.id) && !ids.has(row.id)
      && ['core','rendered','runner'].includes(row.group) && ['csharp','native'].includes(row.implementation), 'CATALOG_INVALID');
    requireTrue(row.implementation !== 'csharp' || typeof row.test === 'string', 'CATALOG_TEST');
    if (row.group === 'rendered') requireTrue(Array.isArray(row.images) && row.images.length > 0 && new Set(row.images).size === row.images.length, 'CATALOG_IMAGES');
    ids.add(row.id);
  }
  return value;
}
function reconcile(selected, results, context) {
  requireTrue(selected.length > 0 && results.length === selected.length, 'RESULT_COUNT');
  const expected = new Map(selected.map(row => [row.id,row])), seen = new Set();
  requireTrue(expected.size === selected.length, 'SELECTION_DUPLICATE');
  for (const result of results) {
    requireTrue(expected.has(result.id) && !seen.has(result.id), 'RESULT_UNKNOWN_DUPLICATE'); seen.add(result.id);
    requireTrue(result.status === 'pass', 'RESULT_NOT_PASS ' + result.id);
    for (const key of ['nonce','input','tool','platform']) requireTrue(typeof context[key] === 'string' && result[key] === context[key], 'RESULT_IDENTITY ' + key);
    if(expected.get(result.id).implementation==='csharp') {
      if(expected.get(result.id).callerStages) {
        requireTrue(result.id==='core.permissions','STAGED_CASE_UNKNOWN');
        baseline.verifyCallerResult(expected.get(result.id),result,context);
      } else {
      requireTrue(result.execution!=='aggregated-caller-stages' && result.monolithicExecuted!==false,'UNDECLARED_STAGED_RESULT');
      requireTrue(Array.isArray(result.artifacts) && result.artifacts.length>=2 && new Set(result.artifacts.map(row=>row.path)).size===result.artifacts.length,'SUBJECT_LOG_REQUIRED');
      for(const artifact of result.artifacts) requireTrue(hash(fs.readFileSync(inside(context.directory,artifact.path)))===artifact.sha256,'ARTIFACT_HASH');
      requireTrue(result.artifacts.some(row=>row.path===result.testReport),'TEST_REPORT_REQUIRED');
      const report=read(inside(context.directory,result.testReport));
      requireTrue(report.results.summary.tests===1 && report.results.summary.passed===1 && report.results.summary.failed===0,'TEST_EXECUTION_COUNT');
      }
    }
    const images = result.images ?? [], imageIds = new Set(), imagePaths = new Set();
    for (const image of images) {
      requireTrue(!imageIds.has(image.checkpoint), 'IMAGE_DUPLICATE'); imageIds.add(image.checkpoint);
      requireTrue(!imagePaths.has(image.path),'IMAGE_PATH_DUPLICATE'); imagePaths.add(image.path);
      const file = inside(context.directory, image.path);
      requireTrue(hash(fs.readFileSync(file)) === image.sha256, 'IMAGE_HASH');
      decodePng(file, 1280, 720);
      const stateFile=inside(context.directory,image.statePath);
      requireTrue(hash(fs.readFileSync(stateFile))===image.stateSha256,'IMAGE_STATE_HASH');
      const state=read(stateFile);
      requireTrue(state.nonce===context.nonce && state.checkpoint===path.basename(image.path,'.png')
        && state.width===1280 && state.height===720,'IMAGE_STATE_IDENTITY');
    }
    for (const name of expected.get(result.id).images ?? []) requireTrue(imageIds.has(name), 'RESULT_IMAGE_MISSING ' + name);
    if (expected.get(result.id).implementation === 'native') requireTrue(result.pid > 0 && result.width === 1280 && result.height === 720, 'NATIVE_SESSION_REQUIRED');
  }
  return results;
}
function loadReport(root, relative, cases) {
  const file = inside(root, relative), summary = read(file);
  requireTrue(summary.schemaVersion === 2 && Array.isArray(summary.selected) && summary.selected.length > 0, 'REPORT_SCHEMA');
  requireTrue(Number.isSafeInteger(summary.startedAt) && Number.isSafeInteger(summary.finishedAt) && summary.finishedAt>=summary.startedAt,'REPORT_TIME');
  const ids = new Set(summary.selected), allowed = new Map(cases.map(row => [row.id,row]));
  requireTrue(ids.size === summary.selected.length && summary.selected.every(id => allowed.has(id)), 'REPORT_SELECTION');
  requireTrue(Array.isArray(summary.results) && summary.results.length === ids.size, 'REPORT_RESULTS');
  const seen = new Set();
  for (const result of summary.results) {
    requireTrue(ids.has(result.id) && !seen.has(result.id) && ['pass','fail','blocked'].includes(result.status), 'REPORT_RESULT_ID'); seen.add(result.id);
    for (const key of ['nonce','input','tool','platform']) requireTrue(result[key] === summary[key], 'REPORT_IDENTITY');
    if (result.status === 'pass') reconcile([allowed.get(result.id)], [result], {...summary,root,directory:path.dirname(file)});
  }
  requireTrue(summary.status === (summary.results.every(row => row.status === 'pass') && !summary.failures.length ? 'pass' : 'fail'), 'REPORT_VERDICT');
  return { summary, path:relative, sha256:hash(fs.readFileSync(file)) };
}
function aggregate(root, references, cases) {
  requireTrue(references.length > 0, 'COMPLETION_EMPTY');
  const latest = new Map(), reports = [], nonces = new Set();
  for (const reference of references) {
    const report = loadReport(root, reference.path, cases);
    requireTrue(report.sha256 === reference.sha256, 'REPORT_HASH');
    requireTrue(!nonces.has(report.summary.nonce), 'REPORT_DUPLICATE'); nonces.add(report.summary.nonce);
    if (reports.length) {
      requireTrue(report.summary.platform === reports[0].summary.platform, 'COMPLETION_PLATFORM');
      requireTrue(report.summary.startedAt>=reports.at(-1).summary.finishedAt,'REPORT_ORDER');
    }
    if (report.summary.repairOf) {
      const prior = reports.find(row => row.sha256 === report.summary.repairOf.sha256 && row.path === report.summary.repairOf.path);
      requireTrue(prior, 'REPAIR_BASELINE_MISSING');
      const unresolved = prior.summary.selected.filter(id => !prior.summary.results.some(row => row.id === id && row.status === 'pass'));
      requireTrue(report.summary.selected.length === unresolved.length && report.summary.selected.every(id => unresolved.includes(id)), 'REPAIR_SELECTION');
    }
    for (const result of report.summary.results) latest.set(result.id, { ...result, report:report.path, reportSha256:report.sha256 });
    reports.push(report);
  }
  const missing = cases.filter(row => latest.get(row.id)?.status !== 'pass').map(row => row.id);
  return { status:missing.length ? 'incomplete' : 'coverage-complete', missing, results:[...latest.values()],
    reports:references, policy:'creator-authorized-temporary-pre-phase5', dependencyAwareReuseCertified:false };
}
function verifyCompletion(root, index, cases) {
  const result = aggregate(root,index.reports,cases);
  requireTrue(result.status === 'coverage-complete', 'COMPLETION_MISSING ' + result.missing.join(','));
  for (const row of result.results) for (const image of (row.images ?? []).filter(image=>cases.find(item=>item.id===row.id).images?.includes(image.checkpoint))) {
    const reviews = (index.reviews ?? []).filter(review => review.reportSha256 === row.reportSha256 && review.id === row.id && review.checkpoint===image.checkpoint && review.imageSha256 === image.sha256);
    requireTrue(reviews.length === 1 && reviews[0].verdict === 'pass' && typeof reviews[0].reviewer === 'string' && reviews[0].reviewer.trim() && typeof reviews[0].notes === 'string' && reviews[0].notes.trim(), 'APPEARANCE_REVIEW_REQUIRED ' + row.id + '/' + image.checkpoint);
  }
  return result;
}
module.exports = {hash,read,inside,requireTrue,catalog,reconcile,loadReport,aggregate,verifyCompletion};
