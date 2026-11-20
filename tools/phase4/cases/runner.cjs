'use strict';

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');
const {spawnSync} = require('node:child_process');

const ROOT = path.resolve(__dirname, '../../..');
const check = require('../check.cjs');

function rejectsMissingCompletion() {
  const result = spawnSync(process.execPath, [
    'tools/phase4/check.cjs', '--verify-completion',
    '.phase4/missing-phase4-runner-negative-fixture/completion.json'
  ], {cwd:ROOT, encoding:'utf8', windowsHide:true, timeout:15000});
  assert.ifError(result.error);
  assert.equal(result.status, 1, result.stderr);
  assert.match(result.stderr, /EVIDENCE_PATH_MISSING/, 'Completion dispatch must reject a missing completion record');
}

function baseFixture() {
  const current = {revision:'1'.repeat(40), source:'2'.repeat(64), media:'3'.repeat(64),
    tool:'4'.repeat(64), build:'5'.repeat(64)};
  const reviewedAt = '2026-11-20T10:00:00.000Z';
  const startedAt = '2026-11-20T11:00:00.000Z';
  const finishedAt = '2026-11-20T11:01:00.000Z';
  const item = {id:'heldout.H01', runGroup:'G1-hub', expected:'Same-named source objects retain different identities'};
  const invocation = crypto.randomUUID();
  const report = {...current, schemaVersion:1, invocation, previousInvocation:null, kind:'initial', status:'pass',
    startedAt, finishedAt, selected:[item.id], outcomes:[{id:item.id, runGroup:item.runGroup, invocation,
      status:'pass', execution:'executed-now', expected:item.expected, observed:'Both observations retain separate source locators'}]};
  const review = {...current, schemaVersion:1, reviewedAt, independent:true, status:'complete',
    scope:'phase-04-whole-source', reviewedBy:'Test Reviewer', implementedBy:'Test Author', report:{sha256:'6'.repeat(64)},
    findings:[{id:'TEST-1', severity:'high', summary:'Fixture finding'}]};
  const fixes = {schemaVersion:1, reviewReport:review.report.sha256,
    findings:[{id:'TEST-1',status:'resolved',reason:'Fixture fix proof',proof:{id:'test-proof'}}]};
  const declarations = {report:[review.reviewedBy, current.revision, current.source].join('\n'),
    verifyProof:proof => assert.equal(proof.id, 'test-proof')};
  const process = {role:'hub', pid:4242, startedAt, finishedAt, timedOut:false, crashed:false, exitCode:0,
    signal:null, streamsClosed:true, inputReleased:true, remainingPids:[]};
  return {current, item, report, review, fixes, declarations, process};
}

function qualification(context) {
  const e = require('../evidence.cjs');
  const scenarios = require('../scenarios.cjs');
  const catalog = scenarios.loadCatalog(ROOT, check.cases);
  const control = [];
  function reject(name, pattern, action) {
    assert.throws(action, pattern, name);
    control.push(name);
  }
  rejectsMissingCompletion();
  assert.equal(scenarios.selectCases(check.cases, '--case', 'content.census,runner.prelaunch').length, 2);
  reject('empty-discovery', /CASE_DISCOVERY_EMPTY/, () => scenarios.selectCases(new Map(), '--group', 'runner'));
  reject('wrong-group', /CASE_PENDING_OR_UNKNOWN/, () => scenarios.selectCases(check.cases, '--group', 'nonexistent'));
  reject('missing-selected-case', /CASE_PENDING_OR_UNKNOWN/, () => scenarios.selectCases(check.cases, '--case', 'runner.prelaunch,missing'));
  reject('duplicate-selection', /CASE_PENDING_OR_UNKNOWN/, () => scenarios.selectCases(check.cases, '--case', 'runner.prelaunch,runner.prelaunch'));
  reject('missing-ui-catalog-entry', /EVIDENCE_SET/, () => scenarios.validateCatalog(ROOT,
    {...catalog,renderedCases:catalog.renderedCases.filter(row => row.id !== 'UIH.HUB-NAV.loading')}));
  reject('missing-h-catalog-entry', /EVIDENCE_SET/, () => scenarios.validateCatalog(ROOT,
    {...catalog,renderedCases:catalog.renderedCases.filter(row => row.id !== 'heldout.H01')}));
  const fixture = baseFixture();
  const {current, item, report, review, fixes, declarations, process} = fixture;
  assert.equal(e.freshFinal([item], [report], current), report);
  e.validateReview(review, fixes, current, declarations);
  e.validateProcess(process);
  const reviewMutation = (name, mutate) => reject(name, /REVIEW_|EVIDENCE_IDENTITY/, () => {
    const changed = structuredClone({review, fixes});
    mutate(changed);
    e.validateReview(changed.review, changed.fixes, current, declarations);
  });
  reviewMutation('same-author-forge', value => {value.review.reviewedBy = value.review.implementedBy;});
  reviewMutation('stale-source', value => {value.review.source = 'a'.repeat(64);});
  reviewMutation('stale-media', value => {value.review.media = 'a'.repeat(64);});
  reviewMutation('stale-tool', value => {value.review.tool = 'a'.repeat(64);});
  reviewMutation('unresolved-high', value => {value.fixes.findings[0].status = 'accepted';});
  reject('forged-review-report', /REVIEW_REPORT_ATTESTATION/, () => e.validateReview(review, fixes, current, {...declarations,report:'forged'}));
  reject('crash', /EVIDENCE_PROCESS/, () => e.validateProcess({...process,crashed:true}));
  reject('timeout', /EVIDENCE_PROCESS/, () => e.validateProcess({...process,timedOut:true}));
  reject('cleanup-leak', /EVIDENCE_PROCESS/, () => e.validateProcess({...process,remainingPids:[4243]}));
  reject('outcome-absent', /EVIDENCE_SET/, () => e.reconcile([item], {...report,outcomes:[]}, current));
  reject('wrong-expectation', /EVIDENCE_OUTCOME/, () => e.reconcile([item], {...report,
    outcomes:[{...report.outcomes[0],expected:'Changed the expected result'}]}, current));
  const later = crypto.randomUUID();
  const failed = {...report, invocation:later, previousInvocation:report.invocation, kind:'final', status:'fail',
    startedAt:report.finishedAt, finishedAt:'2026-11-20T11:02:00.000Z', outcomes:[{...report.outcomes[0],invocation:later,status:'fail'}]};
  reject('later-failure-hidden', /EVIDENCE_FINAL_PASS/, () => e.freshFinal([item], [report,failed], current));
  reject('focused-pass-is-not-full-rerun', /EVIDENCE_FINAL_PASS/, () => e.freshFinal([item], [report,{...failed,kind:'focused',repairReason:'Control'}], current));
  const ref = e.writeExclusive(context.directory, 'immutable-control.json', {value:'original fixture'});
  reject('overwrite-history', /EEXIST/, () => e.writeExclusive(context.directory, ref.path, {value:'replacement'}));
  reject('forged-file-hash', /EVIDENCE_HASH/, () => e.artifact(context.directory, {...ref,sha256:'f'.repeat(64)}));
  assert.deepEqual(e.document(context.directory, ref), {value:'original fixture'});
  const terminal = e.writeExclusive(context.directory, 'test-terminal.json', process);
  const input = e.writeExclusive(context.directory, 'test-input.json', {caseId:item.id,checkpoint:'before',invocation:report.invocation,
    pid:process.pid,focused:true,at:report.startedAt,clientWidth:1280,clientHeight:720,method:'physical-keyboard',action:'tap'});
  const image = {path:'unwritten.png',bytes:250,sha256:'a'.repeat(64)};
  const draw = e.writeExclusive(context.directory, 'test-draw.json', {caseId:item.id,checkpoint:'before',invocation:report.invocation,
    pid:process.pid,drawn:true,frame:1,at:report.finishedAt,observation:'Owned test observation',imageSha256:image.sha256});
  reject('missing-rendered-png', /EVIDENCE_PATH_MISSING unwritten\.png/, () => e.validateRenderedOutcome(context.directory,
    {id:item.id,processes:[terminal],checkpoints:[{id:'before',input,draw,image}]},report));
  reject('unnamed-listening', /EVIDENCE_NAMED_LISTENING/, () => e.validateNamed(context.directory,
    {...current,id:'sample',kind:'listening',reviewedAt:review.reviewedAt,verdict:'pass',observations:'test',files:[ref]},
    'listening',{id:'sample'},current));
  reject('missing-judgment-verdict', /EVIDENCE_NAMED_JUDGMENT/, () => e.validateNamed(context.directory,
    {...current,id:'P1',kind:'judgment',reviewer:'Test Reviewer',reviewedAt:review.reviewedAt,observations:'test',files:[ref]},
    'judgment',{id:'P1'},current));
  const result = {scope:'Source-only negative controls. Fixture identities supply no review or acceptance.', controls:control,
    sourceCases:catalog.cases.length, heldout:catalog.heldout.length, ui:catalog.backstops.length, judgments:catalog.judgments.length, accepted:0};
  e.writeExclusive(context.directory, 'runner-controls.json', result);
  return result;
}

if (require.main === module) {
  require('node:test').test('completion dispatch rejects missing completion evidence', rejectsMissingCompletion);
} else {
  check.registerCase('runner.prelaunch', {
    group:'runner', title:'Current Phase 4 review and prelaunch gate', importance:'Critical',
    rationale:'Running an unreviewed build invalidates all rendered acceptance',
    needsGameWindow:false, runGroup:'G0-runner', order:300,
    requirements:['ASSET-01','ASSET-02','ASSET-03','ASSET-04','ASSET-05'],
    acceptance:['AC-17','AC-19','AC-20'], tags:['source-only','review','selection'],
    setup:'Current saved case catalog, actual CLI dispatch and isolated evidence fixtures',
    actions:['Check fixed source and actual-only cases against executable discovery', 'Reject invalid selection and missing completion evidence',
      'Reject stale or forged review bindings, missing outcomes, later failures, process failures and rewritten history'],
    expected:'Every invalid subject fails explicitly and bounded source controls establish zero actual acceptance',
    reset:'Preserve existing review, selection and proof files', evidence:['immutable-control.json','runner-controls.json','result.json']
  }, qualification);
}

module.exports = {rejectsMissingCompletion, baseFixture, qualification};
