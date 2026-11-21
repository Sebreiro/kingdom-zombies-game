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
  const review = {...current, schemaVersion:1, reviewedAt, independent:true, status:'complete', reportStatus:'issues_found',
    scope:'phase-04-whole-source', reviewedBy:'Test Reviewer', implementedBy:'Test Author', report:{sha256:'6'.repeat(64)},
    findings:[{id:'CR-01', severity:'high', summary:'Fixture finding'}]};
  const reportText = ['---',`reviewer: ${review.reviewedBy}`,`reviewed: ${reviewedAt}`,`revision: ${current.revision}`,
    `source: ${current.source}`,`media: ${current.media}`,`tool: ${current.tool}`,'findings:','  critical: 1','  warning: 0',
    '  info: 0','  total: 1','status: issues_found','---','','### CR-01 — Fixture finding','',
    '**Severity:** high  ','**File:** `tools/phase2/rendered.cjs:10-12`',''].join('\n');
  const originalReport = {text:reportText,sha256:crypto.createHash('sha256').update(reportText).digest('hex')};
  const fixes = {schemaVersion:1, reviewReport:review.report.sha256, originalReport:originalReport.sha256,
    findings:[{id:'CR-01',status:'resolved',reason:'Fixture fix proof',proof:{id:'test-proof'}}]};
  const declarations = {report:reportText, originalReport,
    verifyProof:(finding,proof) => {assert.equal(finding.id,'CR-01');assert.equal(proof.id,'test-proof');}};
  const process = {role:'hub', pid:4242, startedAt, finishedAt, timedOut:false, crashed:false, exitCode:0,
    signal:null, streamsClosed:true, inputReleased:true, remainingPids:[]};
  return {current, item, report, review, fixes, declarations, process};
}

function rejectsInvalidWave() {
  const e = require('../evidence.cjs');
  const {current,review} = baseFixture();
  const relative = '.phase4/tdd/04-30-wave-' + crypto.randomUUID();
  const ref = e.writeExclusive(ROOT,relative+'/sample.wav',Buffer.from('An exact digest does not make text into a WAV.'));
  const sample = {id:'AU-01/test-wave', version:'1', sampleSha256:ref.sha256};
  const verdict = {...current,...sample,kind:'listening',reviewer:'Test Reviewer',reviewedAt:review.reviewedAt,
    verdict:'pass',observations:'Synthetic fixture claim',files:[ref]};
  assert.throws(() => e.validateNamed(ROOT,verdict,'listening',sample,current), /EVIDENCE_WAV_FORMAT/,
    'Exact listening bytes must be a genuine WAV and must not gain a verdict from their extension');
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
  const absent = path.join(context.directory,'missing-actual-review');
  fs.mkdirSync(absent);
  reject('missing-actual-review',/EVIDENCE_PATH_MISSING .*04-REVIEW\.json/,() => scenarios.currentIdentity(absent));
  const resourceControl = spawnSync(process.execPath,['tools/phase2/runner-fixture.cjs','resource-roles'],
    {cwd:ROOT,encoding:'utf8',windowsHide:true,timeout:15000});
  assert.ifError(resourceControl.error);
  assert.equal(resourceControl.status,0,resourceControl.stderr);
  assert.match(resourceControl.stdout,/PASS current three resource roles/);
  e.writeExclusive(context.directory,'inherited-resource-control.log',Buffer.from(resourceControl.stdout));
  selectionControls(context);
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
  const {current, item, report, review, fixes, declarations, process:processFixture} = fixture;
  const originalReport = e.reviewReport(e.git(ROOT,['show',e.ORIGINAL_REVIEW+':'+e.REVIEW_REPORT]));
  assert.deepEqual(originalReport.findings.map(row => row.id),
    ['CR-01','CR-02','CR-03','CR-04','CR-05','CR-06','CR-07','CR-08','CR-09','WR-01','WR-02']);
  assert.equal(e.freshFinal([item], [report], current), report);
  e.validateReview(review, fixes, current, declarations);
  e.validateProcess(processFixture);
  const reviewMutation = (name, mutate) => reject(name, /REVIEW_|EVIDENCE_IDENTITY|EVIDENCE_SET/, () => {
    const changed = structuredClone({review, fixes});
    mutate(changed);
    e.validateReview(changed.review, changed.fixes, current, declarations);
  });
  reviewMutation('same-author-forge', value => {value.review.reviewedBy = value.review.implementedBy;});
  reviewMutation('stale-source', value => {value.review.source = 'a'.repeat(64);});
  reviewMutation('stale-media', value => {value.review.media = 'a'.repeat(64);});
  reviewMutation('stale-tool', value => {value.review.tool = 'a'.repeat(64);});
  reviewMutation('unresolved-high', value => {value.fixes.findings[0].status = 'accepted';});
  reviewMutation('omitted-independent-finding', value => {value.fixes.findings=[];value.review.findings=[];});
  reviewMutation('omitted-finding-disposition', value => {value.fixes.findings=[];});
  reviewMutation('downgraded-independent-finding', value => {value.review.findings[0].severity='medium';});
  reviewMutation('wrong-report-status', value => {value.review.reportStatus='clean';});
  reject('forged-review-report', /REVIEW_REPORT_ATTESTATION/, () => e.validateReview(review, fixes, current, {...declarations,report:'forged'}));
  const currentRevision=e.git(ROOT,['rev-parse','HEAD']).trim();
  const wrongCommit=e.git(ROOT,['rev-parse','5597c6d']).trim();
  const actualCommit=e.git(ROOT,['rev-parse','ef9e58c']).trim();
  const finding={id:'CR-01',paths:['tools/phase2/rendered.cjs']};
  reject('unrelated-fix-commit',/REVIEW_PROOF_COMMIT/,() => scenarios.verifyFixProof(ROOT,catalog,{revision:currentRevision},finding,
    {commit:wrongCommit,cases:[{id:'runner.prelaunch-source'}]}));
  reject('unrelated-passing-case',/REVIEW_PROOF_CASE/,() => scenarios.verifyFixProof(ROOT,catalog,{revision:currentRevision},finding,
    {commit:actualCommit,cases:[{id:'content.census'}]}));
  reject('crash', /EVIDENCE_PROCESS/, () => e.validateProcess({...processFixture,crashed:true}));
  reject('timeout', /EVIDENCE_PROCESS/, () => e.validateProcess({...processFixture,timedOut:true}));
  reject('cleanup-leak', /EVIDENCE_PROCESS/, () => e.validateProcess({...processFixture,remainingPids:[4243]}));
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
  const terminal = e.writeExclusive(context.directory, 'test-terminal.json', {...processFixture,log:ref,executable:ref});
  const input = e.writeExclusive(context.directory, 'test-input.json', {caseId:item.id,checkpoint:'before',invocation:report.invocation,
    pid:processFixture.pid,focused:true,at:report.startedAt,clientWidth:1280,clientHeight:720,method:'physical-keyboard',action:'tap',key:9});
  const image = {path:'unwritten.png',bytes:250,sha256:'a'.repeat(64)};
  const draw = e.writeExclusive(context.directory, 'test-draw.json', {caseId:item.id,checkpoint:'before',invocation:report.invocation,
    pid:processFixture.pid,drawn:true,frame:1,at:report.finishedAt,observation:'Owned test observation',imageSha256:image.sha256});
  reject('missing-rendered-png', /EVIDENCE_PATH_MISSING unwritten\.png/, () => e.validateRenderedOutcome(context.directory,
    {id:item.id,processes:[terminal],checkpoints:[{id:'before',input,draw,image}]},report,{id:item.id,steps:[{id:'before',action:'tap',key:9}]}));
  reject('unnamed-listening', /EVIDENCE_NAMED_LISTENING/, () => e.validateNamed(context.directory,
    {...current,id:'sample',kind:'listening',reviewedAt:review.reviewedAt,verdict:'pass',observations:'test',files:[ref]},
    'listening',{id:'sample'},current));
  reject('missing-judgment-verdict', /EVIDENCE_NAMED_JUDGMENT/, () => e.validateNamed(context.directory,
    {...current,id:'P1',kind:'judgment',reviewer:'Test Reviewer',reviewedAt:review.reviewedAt,observations:'test',files:[ref]},
    'judgment',{id:'P1'},current));
  renderedDeclarationControls(context.directory,reject);
  sourceHistoryControls(context.directory,reject);
  campaignHistoryControls(context.directory,reject);
  const result = {scope:'Source-only negative controls. Fixture identities supply no review or acceptance.', controls:control,
    sourceCases:catalog.cases.length, heldout:catalog.heldout.length, ui:catalog.backstops.length, judgments:catalog.judgments.length, accepted:0};
  e.writeExclusive(context.directory, 'runner-controls.json', result);
  return result;
}

function campaignHistoryControls(directory,reject) {
  const e = require('../evidence.cjs');
  const {timeBudget} = require('../rendered-process.cjs');
  const {current,report} = baseFixture();
  const campaign = crypto.randomUUID();
  const create = (row,terminal) => {
    const relative = '.phase4/acceptance/'+row.invocation;
    const drivers = e.writeExclusive(directory,relative+'/declared-drivers.json',{campaign});
    const identity = {...current,...row,campaign,drivers};
    const start = e.writeExclusive(directory,relative+'/start.json',identity);
    return terminal && (() => e.writeExclusive(directory,relative+'/'+terminal+'.json',{...identity,start,
      ...(terminal === 'interrupted' ? {status:'fail',interruption:'Detached interruption fixture'} : {})}));
  };
  const first = create(report,'report')();
  assert.equal(e.historyDocuments(directory,[first],campaign)[0].invocation,report.invocation);
  const later = {...report,invocation:crypto.randomUUID(),previousInvocation:report.invocation,startedAt:report.finishedAt};
  const finish = create(later,'interrupted');
  reject('opened-campaign-without-terminal',/EVIDENCE_CAMPAIGN_TERMINAL/,() => e.historyDocuments(directory,[first],campaign));
  const end = finish();
  reject('interrupted-campaign-omitted',/EVIDENCE_SET/,() => e.historyDocuments(directory,[first],campaign));
  assert.equal(e.historyDocuments(directory,[first,end],campaign)[1].status,'fail');
  reject('submitted-campaign-terminal-mismatch',/EVIDENCE_CAMPAIGN_REFERENCE/,() =>
    e.historyDocuments(directory,[first,{...end,sha256:'f'.repeat(64)}],campaign));
  let now = 100;
  const controller = new AbortController();
  const budget = timeBudget(50,controller.signal,() => now);
  assert.equal(budget(100),50);
  now = 149;
  assert.equal(budget(100),1);
  now = 150;
  reject('group-deadline-cannot-start-late-helper',/RENDERED_GROUP_DEADLINE/,() => budget());
  controller.abort(new Error('Detached signal control'));
  reject('cancelled-group-cannot-start-helper',/Detached signal control/,() => budget());
}

function sourceHistoryControls(directory, reject) {
  const e = require('../evidence.cjs'), scenarios = require('../scenarios.cjs');
  const {current,item,report} = baseFixture();
  const other = 'a'.repeat(40), equivalent = 'b'.repeat(40);
  const same = (_root, revision) => [current.revision,equivalent].includes(revision);
  const root = path.join(directory,'source-history');
  fs.mkdirSync(root);
  function invocation(revision, at, status, node = 'v24.22.1') {
    const nonce = crypto.randomUUID(), prefix = '.phase4/runs/'+nonce+'/';
    const opened = {caseId:item.id,nonce,revision,startedAt:at,platform:'win32',node,scope:'source/headless; zero game windows'};
    e.writeExclusive(root,prefix+'invocation.json',opened);
    if (status !== null) e.writeExclusive(root,prefix+'result.json',{...opened,status,finishedAt:at,observations:{fixture:true}});
    return {...opened,status};
  }
  const old = invocation(current.revision,report.startedAt,'pass');
  const admitted = new Map([[item.id,old]]);
  e.writeExclusive(root,'tools/phase1/toolchain.json',{node:'24.22.1'});
  const priorRef = e.reference(root,'.phase4/runs/'+old.nonce+'/result.json');
  const latest = () => scenarios.latestPhase4Sources(root,admitted,current.revision,same).get(item.id);
  invocation(other,'2026-11-20T12:00:00.000Z','fail');
  invocation(current.revision,'2026-11-20T12:30:00.000Z','fail','v24.1.0');
  assert.equal(latest().nonce,old.nonce,'proven unrelated failure');
  const failed = invocation(equivalent,'2026-11-20T11:01:00.000Z','fail');
  assert.deepEqual(latest(),failed,'same source equivalent revision supersedes old pass');
  const originalIdentify = e.identifySource, originalSame = e.sameCommittedSource;
  try {
    e.identifySource = () => current;
    e.sameCommittedSource = same;
    const selected = [item,{id:'runner.prelaunch'},{id:'runner.selection'},{id:'acceptance.judgments'},{id:'acceptance.artifacts'},
      {id:'rendered-case',needsGameWindow:true}];
    assert.deepEqual(scenarios.preWindowCases(selected),[item]);
    const verify = ref => scenarios.sourceReceipts(root,scenarios.preWindowCases(selected),[{id:item.id,result:ref}],current);
    reject('unselected-prerequisite-receipt',/EVIDENCE_SET/,() => scenarios.sourceReceipts(root,[item],[],current));
    reject('old-pass-hides-new-exact-fail',/COMPLETION_LATER_SOURCE_FAILURE/,() => verify(priorRef));
    const blocked = invocation(current.revision,'2026-11-20T11:02:00.000Z','blocked');
    assert.equal(latest().nonce,blocked.nonce);
    reject('old-pass-hides-new-exact-block',/COMPLETION_LATER_SOURCE_FAILURE/,() => verify(priorRef));
    const missing = invocation(current.revision,'2026-11-20T11:03:00.000Z',null);
    assert.equal(latest().nonce,missing.nonce);
    reject('old-pass-hides-missing-current-result',/COMPLETION_LATER_SOURCE_FAILURE/,() => verify(priorRef));
    const repaired = invocation(current.revision,'2026-11-20T11:04:00.000Z','pass');
    reject('old-pass-cannot-claim-new-pass',/COMPLETION_SOURCE_REFERENCE_SUPERSEDED/,() => verify(priorRef));
    const repairedRef = e.reference(root,'.phase4/runs/'+repaired.nonce+'/result.json');
    assert(verify(repairedRef).has(item.id),'fresh selected result can resolve prior failures');
  } finally {e.identifySource=originalIdentify;e.sameCommittedSource=originalSame;}
}

function renderedDeclarationControls(directory,reject) {
  const e = require('../evidence.cjs'), scenarios = require('../scenarios.cjs');
  const {encode} = require('../native-observer.cjs');
  const {current,report,process,review} = baseFixture();
  const invocation = report.invocation, id = 'rendered.packaged-relaunch';
  const root = path.join(directory,'rendered-declaration');
  fs.mkdirSync(root);
  const put = (name,value) => e.writeExclusive(root,name,value);
  const fake = put('test-executable.bin',Buffer.from('Detached parser fixture'));
  const image = put('test.png',require('../../phase3/runner-fixture.cjs').png());
  const tree = put('tree.bin',encode(['scene:scene_tree',1,[1,'root','Window',1,'',0,0,'Inspection','Node',2,'',0]]));
  const nativeObject = put('object.bin',encode(['scene:inspect_objects',1,[[2,'Node',[['visible',0,0,0,0,true]]]]]));
  const driver = {id,stimulus:'Observe two detached physical declaration shapes',steps:['first','second'].map(id =>
    ({id,action:'tap',key:9,node:'/root/Inspection',property:'visible',expected:true}))};
  const row = {id,checkpoints:[],processes:[]};
  for (const [index,step] of driver.steps.entries()) {
    const pid = process.pid + index;
    const start = new Date(Date.parse(report.startedAt)+index*1000).toISOString();
    const end = new Date(Date.parse(start)+999).toISOString();
    row.processes.push(put(step.id+'-process.json',{...process,pid,role:'packaged-inspection',startedAt:start,finishedAt:end,log:fake,executable:fake}));
    const flags = {caseId:id,checkpoint:step.id,invocation,pid,at:start};
    const input = put(step.id+'-input.json',{...flags,focused:true,clientWidth:1280,clientHeight:720,method:'physical-keyboard',action:'tap',key:9,
      acknowledgement:{pid,focused:true,width:1280,height:720,action:'tap'}});
    const draw = put(step.id+'-draw.json',{...flags,drawn:true,frame:index+1,frameSource:'composited-client-after-DwmFlush',
      observation:JSON.stringify({node:step.node,property:step.property,value:true}),nativeTree:tree,nativeObject,imageSha256:image.sha256});
    row.checkpoints.push({id:step.id,input,draw,image});
  }
  const gate = {review:{sha256:'7'.repeat(64)},selection:{sha256:'8'.repeat(64)},reviewedAt:review.reviewedAt};
  const declaration = {...current,schemaVersion:1,reviewSha256:gate.review.sha256,selectionSha256:gate.selection.sha256,
    campaign:crypto.randomUUID(),declaredAt:review.reviewedAt,kind:'initial',drivers:[driver]};
  assert.equal(scenarios.reviewedDrivers(declaration,gate,current,[row],'initial').get(id),driver);
  e.validateRenderedOutcome(root,row,report,driver);
  reject('omitted-saved-relaunch-checkpoint',/EVIDENCE_SET/,() => e.validateRenderedOutcome(root,{...row,checkpoints:row.checkpoints.slice(0,1)},report,driver));
  reject('omitted-second-owned-launch',/EVIDENCE_RENDERED_LAUNCHES/,() => e.validateRenderedOutcome(root,{...row,processes:row.processes.slice(0,1)},report,driver));
  reject('reordered-saved-checkpoints',/EVIDENCE_SET/,() => e.validateRenderedOutcome(root,{...row,checkpoints:row.checkpoints.slice().reverse()},report,driver));
  reject('declared-physical-action-mismatch',/EVIDENCE_NATIVE_INPUT/,() => e.validateRenderedOutcome(root,row,report,
    {...driver,steps:[{...driver.steps[0],key:13},driver.steps[1]]}));
  reject('same-png-wrong-declared-state',/EVIDENCE_DECLARED_OBSERVATION/,() => e.validateRenderedOutcome(root,row,report,
    {...driver,steps:[{...driver.steps[0],expected:false},driver.steps[1]]}));
  reject('stale-driver-review',/RENDERED_INPUT_SCHEMA/,() => scenarios.reviewedDrivers({...declaration,reviewSha256:'a'.repeat(64)},gate,current,[row],'initial'));
}

function selectionFixture() {
  const {current,review} = baseFixture();
  const catalog = {sha256:'a'.repeat(64)};
  const cases = [
    {id:'phase1/critical',family:'phase1',group:'engine',importance:'Critical',needsGameWindow:false,runGroup:'G0',order:1,processRoles:['source']},
    {id:'phase1/affected',family:'phase1',group:'graph',importance:'Low',needsGameWindow:false,runGroup:'G0',order:2,processRoles:['source']},
    {id:'heldout.H01',importance:'High',needsGameWindow:true,runGroup:'G1-hub',order:3,processRoles:['inspection-hub']},
    {id:'phase1/unaffected',family:'phase1',group:'pins',importance:'Normal',needsGameWindow:false,runGroup:'G0',order:4,processRoles:['source']}
  ];
  const row = (id,add={}) => ({id,reason:'Independent fixture assessment',dependencies:['phase source fixture'],
    new:false,changed:false,affected:false,uncertain:false,...add});
  const group = (id, ids, roles) => ({id,cases:ids,roles,reset:'Reset fixture',cleanup:'Close fixture',timeoutMs:12000});
  const selected = [row('phase1/critical',{affected:true}),row('phase1/affected',{affected:true}),row('heldout.H01',{new:true})];
  const record = {...current,schemaVersion:1,status:'declared-before-first-window',declaredAt:review.reviewedAt,platform:'win32-x64',
    window:{width:1280,height:720},vm:false,catalogSha256:catalog.sha256,selected,omitted:[row('phase1/unaffected')],
    groups:[group('G0',selected.slice(0,2).map(item => item.id),['source']),group('G1-hub',['heldout.H01'],['inspection-hub'])]};
  return {current,catalog,cases,record};
}

function controlRunner(context, name, run, controls = []) {
  const e = require('../evidence.cjs');
  function reject(id, pattern, action) {assert.throws(action,pattern,id);controls.push(id);}
  const result = run(reject);
  e.writeExclusive(context.directory,name+'-controls.json',{scope:'Synthetic source-only fixture records; no real windows, reviewers, listening or acceptance.',controls});
  return {controls,...result};
}

function selectionControls(context) {
  const scenarios = require('../scenarios.cjs');
  return controlRunner(context,'selection',reject => {
    const {current,catalog,cases,record} = selectionFixture();
    const impact = scenarios.impactClosure(cases,['src/godot-game/inspection/InspectionRoot.cs']);
    const verify = value => scenarios.validateSelection(value,cases,catalog,current,impact);
    assert.deepEqual(verify(record).selected.map(row => row.id),record.selected.map(row => row.id));
    const alteration = (label,pattern,action) => reject(label,pattern,() => {const data=structuredClone(record);action(data);verify(data);});
    alteration('missing-disposition',/EVIDENCE_SET/,data => {data.omitted=[];});
    alteration('omit-critical',/SELECTION_REQUIRED/,data => {data.omitted.push(data.selected.shift());});
    alteration('omit-affected',/SELECTION_REQUIRED/,data => {data.omitted.push(data.selected.splice(1,1)[0]);});
    alteration('deny-affected-impact',/SELECTION_IMPACT_MISMATCH/,data => {data.selected[1].affected=false;});
    alteration('omit-new',/SELECTION_REQUIRED/,data => {data.omitted.push(data.selected.pop());});
    alteration('omit-changed',/SELECTION_REQUIRED/,data => {data.omitted[0].changed=true;});
    alteration('uncertain-impact',/SELECTION_IMPACT/,data => {data.omitted[0].uncertain=true;});
    alteration('stale-catalog',/SELECTION_SCHEMA/,data => {data.catalogSha256='b'.repeat(64);});
    alteration('stale-reviewed-source',/EVIDENCE_IDENTITY/,data => {data.source='b'.repeat(64);});
    alteration('wrong-window-size',/SELECTION_SCHEMA/,data => {data.window.width=1024;});
    alteration('role-substitution',/EVIDENCE_SET/,data => {data.groups[1].roles=['lan-guest'];});
    alteration('group-order',/SELECTION_GROUP/,data => {data.groups.reverse();});
    alteration('unbounded-launch',/SELECTION_GROUP/,data => {data.groups[1].timeoutMs=900001;});
    const first = [1,'root','Window',1,'',0,0,'Inspection','Node',2,'res://inspection/Inspection.tscn',0];
    assert.equal(scenarios.treePaths(first).get('/root/Inspection'),2);
    reject('incomplete-native-tree',/RENDERED_NATIVE_TREE_INCOMPLETE/,() => scenarios.treePaths([2,...first.slice(1)]));
    return {selected:record.selected.length,omitted:record.omitted.length};
  });
}

function actualPrelaunch(prerequisites) {
  const scenarios = require('../scenarios.cjs');
  const current = scenarios.currentIdentity(ROOT);
  const gate = scenarios.prelaunch(ROOT,scenarios.loadCatalog(ROOT,check.cases),current,{prerequisites});
  return {currentReview:gate.review,currentSelection:gate.selection,actualSelected:gate.selected.length,
    checkedSourcePrerequisites:prerequisites ? scenarios.preWindowCases(gate.selected).length : 0};
}

function pcmFixture() {
  const wav = Buffer.alloc(48);
  wav.write('RIFF',0);wav.writeUInt32LE(40,4);wav.write('WAVEfmt ',8);wav.writeUInt32LE(16,16);
  wav.writeUInt16LE(1,20);wav.writeUInt16LE(2,22);wav.writeUInt32LE(44100,24);wav.writeUInt32LE(176400,28);
  wav.writeUInt16LE(4,32);wav.writeUInt16LE(16,34);wav.write('data',36);wav.writeUInt32LE(4,40);wav.writeInt16LE(9,44);
  return wav;
}

function judgmentControls(context) {
  const e = require('../evidence.cjs'), scenarios = require('../scenarios.cjs');
  const result = controlRunner(context,'judgments',reject => {
    rejectsInvalidWave();
    const {current,report} = baseFixture();
    const file = e.writeExclusive(context.directory,'fixture.wav',pcmFixture());
    const picture = e.writeExclusive(context.directory,'fixture-image-reference.bin',Buffer.from('Synthetic image reference on detached fixture'));
    const item = {id:'AU-01/fixture',kind:'listening',version:'1',sampleSha256:file.sha256};
    const authority = {id:'P1',statement:'Synthetic procedure text',criterion:'AC-22',cases:['heldout.H01']};
    const final = {...report,outcomes:[{id:'heldout.H01',status:'pass',checkpoints:[{image:picture}]},{id:'rendered.audio-audition',status:'pass'}]};
    const row = (kind, id, fields) => ({...current,id,kind,reviewer:'Fixture Reviewer',reviewedAt:final.finishedAt,
      observations:'Synthetic check of file identity and verdict fields.',verdict:'pass',...fields});
    const listening = {...current,schemaVersion:1,invocation:final.invocation,verdicts:[row('listening',item.id,
      {method:'actual-listening',caseId:'rendered.audio-audition',version:'1',sampleSha256:file.sha256,files:[file]})]};
    const judgments = {...current,schemaVersion:1,invocation:final.invocation,verdicts:[row('judgment',authority.id,
      {method:'actual-named-review',statement:authority.statement,criterion:authority.criterion,caseIds:authority.cases,files:[picture]})]};
    const verify = (a,b) => e.validateJudgments(context.directory,a,b,current,[item],[authority],final);
    verify(listening,judgments);
    reject('absent-listening',/EVIDENCE_SET/,() => verify({...listening,verdicts:[]},judgments));
    reject('absent-judgment',/EVIDENCE_SET/,() => verify(listening,{...judgments,verdicts:[]}));
    const change = (data, fields) => ({...data,verdicts:[{...data.verdicts[0],...fields}]});
    reject('listener-missing',/EVIDENCE_NAMED_LISTENING/,() => verify(change(listening,{reviewer:''}),judgments));
    reject('sample-version-stale',/EVIDENCE_LISTENING_SAMPLE/,() => verify(change(listening,{version:'2'}),judgments));
    reject('audio-review-output-only',/EVIDENCE_LISTENING_RUN/,() => verify(change(listening,{method:'successful-play'}),judgments));
    reject('audible-required-failure',/EVIDENCE_LISTENING_FAILED/,() => verify(change(listening,{verdict:'fail'}),judgments));
    reject('wrong-sample-digest',/EVIDENCE_LISTENING_SAMPLE/,() => verify(change(listening,{sampleSha256:'f'.repeat(64)}),judgments));
    reject('missing-actual-image',/EVIDENCE_JUDGMENT_IMAGE/,() => verify(listening,change(judgments,{files:[file]})));
    reject('prohibition-failure',/EVIDENCE_JUDGMENT_FAILED/,() => verify(listening,change(judgments,{verdict:'fail'})));
    reject('old-judgment-source',/EVIDENCE_IDENTITY/,() => verify(listening,change(judgments,{source:'f'.repeat(64)})));
    reject('wrong-judgment-statement',/EVIDENCE_JUDGMENT_RUN/,() => verify(listening,change(judgments,{statement:'Different prohibition'})));
    const expected = e.listeningExpectations(ROOT);
    assert(expected.length>30 && expected.some(item=>item.id==='D-31') && expected.some(item=>item.id==='D-32'));
    return {audioExpectations:expected.length,actualNamedVerdicts:0};
  });
  const listeningPresent = fs.existsSync(path.join(ROOT,e.LISTENING));
  const judgmentsPresent = fs.existsSync(path.join(ROOT,e.JUDGMENTS));
  if (listeningPresent || judgmentsPresent || fs.existsSync(path.join(ROOT,e.PHASE+'04-RUNS.json'))) {
    const current = scenarios.currentIdentity(ROOT), catalog = scenarios.loadCatalog(ROOT,check.cases);
    const gate = scenarios.prelaunch(ROOT,catalog,current,{prerequisites:false});
    const listening = e.document(ROOT,e.LISTENING), judgments = e.document(ROOT,e.JUDGMENTS);
    const final = scenarios.finalRendered(ROOT,gate,current,judgments.runs,judgments.campaign);
    e.validateJudgments(ROOT,listening,judgments,current,e.listeningExpectations(ROOT),e.judgmentExpectations(catalog),final);
    result.actualNamedVerdicts = listening.verdicts.length + judgments.verdicts.length;
  }
  return result;
}

async function completionReceiptControls(context) {
  const e = require('../evidence.cjs'), scenarios = require('../scenarios.cjs');
  const {current} = baseFixture();
  const root = path.join(context.directory,'completion-history');
  fs.mkdirSync(root);
  e.writeExclusive(root,'tools/phase1/toolchain.json',{node:'24.22.1'});
  const qualification = {id:'acceptance.artifacts-source'}, prerequisite = {id:'source.publication'}, closure = {id:'acceptance.artifacts'};
  function invoke(item, minute, status) {
    const startedAt = `2026-11-20T11:${minute}:00.000Z`, nonce = crypto.randomUUID();
    const directory = '.phase4/runs/'+nonce+'/';
    const opened = {caseId:item.id,nonce,revision:current.revision,startedAt,platform:'win32',node:'v24.22.1',
      scope:'source/headless; zero game windows'};
    e.writeExclusive(root,directory+'invocation.json',opened);
    if (status) return {id:item.id,result:e.writeExclusive(root,directory+'result.json',
      {...opened,finishedAt:startedAt,status,observations:{fixture:true}})};
    return null;
  }
  const sources = [invoke(qualification,'00','pass'),invoke(prerequisite,'01','pass')];
  const oldClosure = invoke(closure,'02','pass');
  const selected = [qualification,prerequisite,closure];
  invoke(closure,'03',null);
  assert.throws(() => scenarios.sourceReceipts(root,selected,[...sources,oldClosure],current),
    /COMPLETION_LATER_SOURCE_FAILURE acceptance\.artifacts missing/);
  const controls = ['legacy-closure-self-receipt-is-missing'];
  const verify = scenarios.verifyCompletion;
  const commands = [
    ['acceptance.artifacts',async () => (await check.cases.get(closure.id).run(context)).completion],
    ['--verify-completion',() => check.main(['--verify-completion',e.COMPLETION],() => {})]
  ];
  // Exercise both entry points against the same detached ledger without publishing acceptance results.
  scenarios.verifyCompletion = async (project, relative, cases) => {
    assert.equal(path.resolve(project),ROOT);
    assert.equal(relative,e.COMPLETION);
    assert.equal(cases,check.cases);
    return [...scenarios.completionSourceReceipts(root,selected,sources,current)];
  };
  async function reject(name, diagnostic) {
    for (const [command,run] of commands) {
      await assert.rejects(run,diagnostic);
      controls.push(command+' '+name);
    }
  }
  try {
    for (const [command,run] of commands) {
      assert.deepEqual(await run(),[qualification.id,prerequisite.id]);
      controls.push(command+' ignores its own active closure receipt');
    }
    invoke(prerequisite,'04',null);
    await reject('rejects another in-flight prerequisite',/COMPLETION_LATER_SOURCE_FAILURE source\.publication missing/);
    invoke(prerequisite,'05','fail');
    await reject('rejects another failed prerequisite',/COMPLETION_LATER_SOURCE_FAILURE source\.publication fail/);
    sources[1] = invoke(prerequisite,'06','pass');
    invoke(qualification,'07',null);
    await reject('rejects a newer missing qualification',/COMPLETION_LATER_SOURCE_FAILURE acceptance\.artifacts-source missing/);
  } finally {scenarios.verifyCompletion = verify;}
  return controls;
}

async function artifactControls(context) {
  const e = require('../evidence.cjs');
  const receiptControls = await completionReceiptControls(context);
  return controlRunner(context,'artifacts',reject => {
    const {current} = baseFixture();
    const source = 'function chooseExact() { return 7; }';
    const file = e.writeExclusive(context.directory,'test-source.cjs',Buffer.from(source + '\n'));
    const symbol = {path:file.path,member:'chooseExact',signature:'chooseExact()',line:1,sourceLine:source};
    const guarded = e.guardedHostTuples(ROOT);
    assert.equal(guarded.length,251);
    const expected = {files:[file.path],cases:['sample.case'],native:['operation/exact',...guarded],guarded,archives:[]};
    const audit = {...current,schemaVersion:1,sourceSymbols:[symbol],nativeHostTuples:guarded,archives:[]};
    const inventory = {...current,schemaVersion:1,files:[file],sourceSymbols:[symbol],caseExports:expected.cases,nativePermits:expected.native,archives:[]};
    const verify = (value, proof=audit) => e.validateArtifactInventory(context.directory,value,expected,proof,current);
    verify(inventory);
    verify({...inventory,nativePermits:inventory.nativePermits.slice().reverse()},{...audit,nativeHostTuples:guarded.slice().reverse()});
    reject('omitted-source-file',/EVIDENCE_SET/,() => verify({...inventory,files:[]}));
    reject('changed-source-bytes',/EVIDENCE_HASH/,() => verify({...inventory,files:[{...file,bytes:file.bytes+1}]}));
    reject('omitted-symbol',/EVIDENCE_SET/,() => verify({...inventory,sourceSymbols:[]}));
    reject('unreal-source-symbol',/EVIDENCE_SYMBOL_SOURCE/,() => {
      const fake={...symbol,member:'forged',signature:'forged()'};
      verify({...inventory,sourceSymbols:[fake]},{...audit,sourceSymbols:[fake]});
    });
    reject('omitted-native-tuple',/EVIDENCE_SET/,() => verify({...inventory,nativePermits:[]}));
    reject('omitted-guarded-host',/EVIDENCE_SET/,() => verify(inventory,{...audit,nativeHostTuples:guarded.slice(1)}));
    reject('duplicate-guarded-host',/EVIDENCE_SET/,() => verify(inventory,{...audit,nativeHostTuples:[guarded[1],...guarded.slice(1)]}));
    reject('changed-guarded-host',/EVIDENCE_SET/,() => verify(inventory,{...audit,nativeHostTuples:[guarded[0]+' changed',...guarded.slice(1)]}));
    reject('missing-case-export',/EVIDENCE_SET/,() => verify({...inventory,caseExports:[]}));
    reject('older-audit-build',/EVIDENCE_IDENTITY/,() => verify(inventory,{...audit,build:'f'.repeat(64)}));
    return {verifiedFixtureFiles:inventory.files.length};
  },receiptControls);
}

if (require.main === module) {
  require('node:test').test('completion dispatch rejects missing completion evidence', rejectsMissingCompletion);
  require('node:test').test('listening evidence rejects non-WAV bytes with an exact digest', rejectsInvalidWave);
} else {
  const base = {group:'runner',importance:'Critical',needsGameWindow:false,runGroup:'G0-runner',
    requirements:['ASSET-01','ASSET-02','ASSET-03','ASSET-04','ASSET-05'],acceptance:['AC-17','AC-19','AC-20'],
    tags:['source-only','runner','acceptance-integrity'],setup:'Fixed current source and invocation-owned negative fixtures',
    reset:'Keep original fixtures, review records and every prior invocation immutable'};
  check.registerCase('runner.prelaunch',{...base,order:300,title:'Current Phase 4 review and prelaunch gate',
    rationale:'Running an unreviewed build invalidates all rendered acceptance',
    setup:'Current independent review, exact saved selection and selected no-window receipts',
    actions:['Authenticate the committed independent review and each finding disposition','Verify every selected pre-window source receipt and later outcome'],
    expected:'Current independent review, selected groups and eligible no-window prerequisites authorize the prelaunch gate',
    evidence:['result.json']},() => actualPrelaunch(true));
  check.registerCase('runner.selection',{...base,order:301,title:'Exact current and inherited selected cases',
    rationale:'Omitting a Critical, new, changed or affected check invalidates the planned evidence',
    setup:'Current independent review and exact committed selection',
    actions:['Derive phase-local impact independently from changed source, shared inputs and inherited consumers','Check exact selected and omitted cases, roles and ordering'],
    expected:'Every required current or inherited case remains selected with an exact process and cleanup declaration',
    evidence:['result.json']},() => actualPrelaunch(false));
  check.registerCase('runner.prelaunch-source',{...base,order:304,title:'Detached review, selection and runner evidence controls',
    rationale:'Missing, misbound or superseded evidence must fail before a game window opens',
    actions:['Exercise detached review, selection and actual CLI dispatch failure controls','Reject invalid results, process failures and rewritten history',
      'Exercise current inherited project resource roles without a game window'],
    expected:'Every invalid subject fails explicitly and bounded source controls establish zero actual acceptance',
    evidence:['immutable-control.json','runner-controls.json','selection-controls.json','inherited-resource-control.log','result.json']},qualification);
  check.registerCase('acceptance.judgments',{...base,order:302,title:'Named current media and separate prohibition judgments',
    rationale:'A missing or stale listening or real named verdict cannot support actual acceptance',
    actions:['Authenticate actual PCM WAV format, exact sample and version, and independent named verdict receipts','Check one distinct named result for each prohibition and exact current case images'],
    expected:'Every missing, stale, forged or failed named result rejects without mechanical listening or visual credit',
    evidence:['judgments-controls.json','result.json']},judgmentControls);
  check.registerCase('acceptance.artifacts',{...base,order:303,title:'Current artifact, scope and exact completion inventory',
    rationale:'An incomplete or stale artifact inventory can hide unverified production or selected failures',
    setup:'Current completion index, eligible source receipts and the actual rendered and named outcomes',
    actions:['Compare exact tracked artifacts and reviewed symbols, case exports, native tuples and retained archive members','Reconcile the complete current run, earlier failure history, and required coverage'],
    expected:'Missing, stale or unexpected artifacts, signatures, cases and final evidence are rejected',
    evidence:['result.json']},async () => ({completion:await require('../scenarios.cjs').verifyCompletion(ROOT,require('../evidence.cjs').COMPLETION,check.cases)}));
  check.registerCase('acceptance.artifacts-source',{...base,order:305,title:'Detached artifact inventory and receipt-history controls',
    rationale:'Source qualification must expose invalid evidence independently of the final closure result',
    actions:['Validate detached artifact, source signature and native permission controls','Require independent prerequisites while a completion check is in progress'],
    expected:'An active completion check does not require its own receipt and every independent missing or failed receipt blocks closure',
    evidence:['artifacts-controls.json','result.json']},artifactControls);
}

module.exports = {rejectsMissingCompletion,baseFixture,rejectsInvalidWave,qualification,selectionFixture,controlRunner,
  sourceHistoryControls,renderedDeclarationControls,selectionControls,pcmFixture,judgmentControls,artifactControls};
