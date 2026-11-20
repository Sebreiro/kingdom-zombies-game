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
  const terminal = e.writeExclusive(context.directory, 'test-terminal.json', {...process,log:ref,executable:ref});
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
  if (fs.existsSync(path.join(ROOT,e.REVIEW))) {
    const gate = scenarios.prelaunch(ROOT,catalog,scenarios.currentIdentity(ROOT));
    result.currentReview = gate.review;
    result.currentSelection = gate.selection;
  }
  return result;
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

function controlRunner(context, name, run) {
  const e = require('../evidence.cjs'), controls = [];
  function reject(id, pattern, action) {assert.throws(action,pattern,id);controls.push(id);}
  const result = run(reject);
  e.writeExclusive(context.directory,name+'-controls.json',{scope:'Synthetic source-only fixture records; no real windows, reviewers, listening or acceptance.',controls});
  return {controls,...result};
}

function selectionControls(context) {
  const scenarios = require('../scenarios.cjs'), e = require('../evidence.cjs');
  const result = controlRunner(context,'selection',reject => {
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
  if (fs.existsSync(path.join(ROOT,e.REVIEW)) || fs.existsSync(path.join(ROOT,e.SELECTION))) {
    const gate = scenarios.prelaunch(ROOT,scenarios.loadCatalog(ROOT,check.cases),scenarios.currentIdentity(ROOT));
    result.currentSelection = gate.selection;
    result.actualSelected = gate.selected.length;
  }
  return result;
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
    const gate = scenarios.prelaunch(ROOT,catalog,current);
    const listening = e.document(ROOT,e.LISTENING), judgments = e.document(ROOT,e.JUDGMENTS);
    const final = scenarios.finalRendered(ROOT,gate,current,judgments.runs,judgments.campaign);
    e.validateJudgments(ROOT,listening,judgments,current,e.listeningExpectations(ROOT),e.judgmentExpectations(catalog),final);
    result.actualNamedVerdicts = listening.verdicts.length + judgments.verdicts.length;
  }
  return result;
}

async function artifactControls(context) {
  const e = require('../evidence.cjs'), scenarios = require('../scenarios.cjs');
  const result = controlRunner(context,'artifacts',reject => {
    const {current} = baseFixture();
    const source = 'function chooseExact() { return 7; }';
    const file = e.writeExclusive(context.directory,'test-source.cjs',Buffer.from(source + '\n'));
    const symbol = {path:file.path,member:'chooseExact',signature:'chooseExact()',line:1,sourceLine:source};
    const expected = {files:[file.path],cases:['sample.case'],native:['operation/exact'],archives:[]};
    const audit = {...current,schemaVersion:1,sourceSymbols:[symbol],archives:[]};
    const inventory = {...current,schemaVersion:1,files:[file],sourceSymbols:[symbol],caseExports:expected.cases,nativePermits:expected.native,archives:[]};
    const verify = (value, proof=audit) => e.validateArtifactInventory(context.directory,value,expected,proof,current);
    verify(inventory);
    reject('omitted-source-file',/EVIDENCE_SET/,() => verify({...inventory,files:[]}));
    reject('changed-source-bytes',/EVIDENCE_HASH/,() => verify({...inventory,files:[{...file,bytes:file.bytes+1}]}));
    reject('omitted-symbol',/EVIDENCE_SET/,() => verify({...inventory,sourceSymbols:[]}));
    reject('unreal-source-symbol',/EVIDENCE_SYMBOL_SOURCE/,() => {
      const fake={...symbol,member:'forged',signature:'forged()'};
      verify({...inventory,sourceSymbols:[fake]},{...audit,sourceSymbols:[fake]});
    });
    reject('omitted-native-tuple',/EVIDENCE_SET/,() => verify({...inventory,nativePermits:[]}));
    reject('missing-case-export',/EVIDENCE_SET/,() => verify({...inventory,caseExports:[]}));
    reject('older-audit-build',/EVIDENCE_IDENTITY/,() => verify(inventory,{...audit,build:'f'.repeat(64)}));
    return {verifiedFixtureFiles:inventory.files.length};
  });
  if (fs.existsSync(path.join(ROOT,e.COMPLETION))) result.completion = await scenarios.verifyCompletion(ROOT,e.COMPLETION,check.cases);
  return result;
}

if (require.main === module) {
  require('node:test').test('completion dispatch rejects missing completion evidence', rejectsMissingCompletion);
  require('node:test').test('listening evidence rejects non-WAV bytes with an exact digest', rejectsInvalidWave);
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
  const base = {group:'runner',importance:'Critical',needsGameWindow:false,runGroup:'G0-runner',
    requirements:['ASSET-01','ASSET-02','ASSET-03','ASSET-04','ASSET-05'],acceptance:['AC-17','AC-19','AC-20'],
    tags:['source-only','runner','acceptance-integrity'],setup:'Fixed current source and invocation-owned negative fixtures',
    reset:'Keep original fixtures, review records and every prior invocation immutable'};
  check.registerCase('runner.selection',{...base,order:301,title:'Exact current and inherited selected cases',
    rationale:'Omitting a Critical, new, changed or affected check invalidates the planned evidence',
    actions:['Derive phase-local impact independently from changed source, shared inputs and inherited consumers','Check exact selected and omitted cases, roles and ordering'],
    expected:'Every required current or inherited case remains selected with an exact process and cleanup declaration',
    evidence:['selection-controls.json','result.json']},selectionControls);
  check.registerCase('acceptance.judgments',{...base,order:302,title:'Named current media and separate prohibition judgments',
    rationale:'A missing or stale listening or real named verdict cannot support actual acceptance',
    actions:['Authenticate actual PCM WAV format, exact sample and version, and independent named verdict receipts','Check one distinct named result for each prohibition and exact current case images'],
    expected:'Every missing, stale, forged or failed named result rejects without mechanical listening or visual credit',
    evidence:['judgments-controls.json','result.json']},judgmentControls);
  check.registerCase('acceptance.artifacts',{...base,order:303,title:'Current artifact, scope and exact completion inventory',
    rationale:'An incomplete or stale artifact inventory can hide unverified production or selected failures',
    actions:['Compare exact tracked artifacts and reviewed symbols, case exports, native tuples and retained archive members','Reconcile the complete current run, earlier failure history, and required coverage'],
    expected:'Missing, stale or unexpected artifacts, signatures, cases and final evidence are rejected',
    evidence:['artifacts-controls.json','result.json']},artifactControls);
}

module.exports = {rejectsMissingCompletion,baseFixture,rejectsInvalidWave,qualification,selectionFixture,controlRunner,
  selectionControls,pcmFixture,judgmentControls,artifactControls};
