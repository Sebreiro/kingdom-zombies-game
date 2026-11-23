'use strict';

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');
const {spawnSync} = require('node:child_process');

const ROOT = path.resolve(__dirname, '../../../../..');
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

async function qualification(context) {
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
  reject('focused-pass-is-not-full-rerun', /EVIDENCE_FINAL_PASS/, () => e.freshFinal([item], [
    {...report,status:'fail',outcomes:[{...report.outcomes[0],status:'blocked'}]},
    {...failed,kind:'focused',repairReason:'Control',status:'pass',outcomes:[{...failed.outcomes[0],status:'pass'}]}
  ], current));
  const ref = e.writeExclusive(context.directory, 'immutable-control.json', {value:'original fixture'});
  reject('overwrite-history', /EEXIST/, () => e.writeExclusive(context.directory, ref.path, {value:'replacement'}));
  reject('forged-file-hash', /EVIDENCE_HASH/, () => e.artifact(context.directory, {...ref,sha256:'f'.repeat(64)}));
  assert.deepEqual(e.document(context.directory, ref), {value:'original fixture'});
  reject('unnamed-listening', /EVIDENCE_NAMED_LISTENING/, () => e.validateNamed(context.directory,
    {...current,id:'sample',kind:'listening',reviewedAt:review.reviewedAt,verdict:'pass',observations:'test',files:[ref]},
    'listening',{id:'sample'},current));
  reject('missing-judgment-verdict', /EVIDENCE_NAMED_JUDGMENT/, () => e.validateNamed(context.directory,
    {...current,id:'P1',kind:'judgment',reviewer:'Test Reviewer',reviewedAt:review.reviewedAt,observations:'test',files:[ref]},
    'judgment',{id:'P1'},current));
  renderedDeclarationControls(context.directory,reject);
  authoredCoverageControls(context.directory,reject);
  sourceHistoryControls(context.directory,reject);
  inheritedReceiptControls(context.directory,reject);
  campaignHistoryControls(context.directory,reject);
  await interruptionControls(context.directory,reject);
  repairHistoryControls(reject);
  buildIdentityControls(context.directory,reject);
  await processIdentityControls(name => control.push(name));
  const pythonPin = require('../python.cjs');
  pythonPin.verifyPython();
  const substitutes = path.join(context.directory,'python-controls');
  fs.mkdirSync(path.join(substitutes,'approved-executable'),{recursive:true});
  fs.writeFileSync(path.join(substitutes,'python.exe'),'unknown interpreter bytes',{flag:'wx'});
  reject('unpinned-python-executable',/PYTHON_PIN executable/,() => pythonPin.verifyPython(path.join(substitutes,'python.exe')));
  fs.copyFileSync(pythonPin.PYTHON,path.join(substitutes,'approved-executable','python.exe'),fs.constants.COPYFILE_EXCL);
  fs.writeFileSync(path.join(substitutes,'approved-executable',pythonPin.PIN.runtimeLibrary),'wrong runtime',{flag:'wx'});
  reject('unpinned-python-runtime-library',/PYTHON_PIN runtime library/,() =>
    pythonPin.verifyPython(path.join(substitutes,'approved-executable','python.exe')));
  const desktop=desktopHelperControl(context.directory);
  const recovery=await inspectionRecoveryControls(context.directory);
  const result = {scope:'Source-only negative controls. Fixture identities supply no review or acceptance.', controls:control,desktop,recovery,
    sourceCases:catalog.cases.length, heldout:catalog.heldout.length, ui:catalog.backstops.length, judgments:catalog.judgments.length, accepted:0};
  e.writeExclusive(context.directory, 'runner-controls.json', result);
  return result;
}

async function inspectionRecoveryControls(directory) {
  const {runInspectionLane}=require('../scenarios.cjs'), e=require('../evidence.cjs');
  const items=['heldout.H12','heldout.H13'].map(id=>({id}));
  const steps=[{id:'initial',lifetime:1},{id:'screenshot',lifetime:1},
    {id:'navigate-return-items',lifetime:1,navigate:true},{id:'reset-1',lifetime:1}];
  const drivers=new Map(items.map(item=>[item.id,{steps}]));
  const records=[];
  for (const resetWorks of [true,false]) {
    const calls=[], outcomes=items.map(({id})=>({id,status:'pass',checkpoints:[]}));
    const session={usable:true,async checkpoint(item,step) {
      calls.push([item.id,step.id]);
      if (item===items[0] && step.id==='screenshot') throw new Error('Detached original screenshot failure');
      return {id:step.id};
    },async restore(item,repair) {
      calls.push([item.id,...repair.map(step=>step.id)]);
      assert.deepEqual(repair,steps.slice(2));
      if (!resetWorks) throw new Error('Detached baseline was not restored');
      return {baseline:'detached verification'};
    }};
    const attempted=await runInspectionLane(session,{items,lifetime:1},outcomes,drivers,'detached',()=>500,()=>{});
    assert.equal(outcomes[0].status,'fail');
    assert.equal(outcomes[0].observed,'Detached original screenshot failure');
    assert.equal(outcomes[0].failedCheckpoint,'screenshot');
    assert.deepEqual(outcomes[0].checkpoints,[{id:'initial'}]);
    assert.deepEqual(calls[2],[items[0].id,...steps.slice(2).map(step=>step.id)]);
    assert.equal(outcomes[1].status,resetWorks?'pass':'blocked');
    assert.equal(attempted.length,resetWorks?2:1);
    assert.equal(calls.some(([id])=>id===items[1].id),resetWorks);
    if (resetWorks) assert.equal(outcomes[0].recovery.baseline,'detached verification');
    else {
      assert.equal(outcomes[0].resetFailure,'Detached baseline was not restored');
      assert.equal(outcomes[1].execution,'not-executed');
    }
    records.push({resetWorks,calls,outcomes});
  }
  return e.writeExclusive(directory,'inspection-recovery-controls.json',records);
}

function authoredCoverageControls(directory,reject) {
  const contracts=require('../rendered-contracts.cjs'), e=require('../evidence.cjs');
  const {encode}=require('../native-observer.cjs'),{savedNativeSnapshot}=require('../scenarios.cjs');
  for (const [id,keypoints] of [
    ['UIH.CHARACTER-PREVIEW.populated',['walk-far','walk-near']],
    ['UIH.WORLD-ITEM-PREVIEW.populated',['assembled-world-day','portal-ruins-focus']],
    ['heldout.H12',['death-effect','death-step','death-retired']]
  ]) {
    const steps=contracts.template(id).map(step=>({...step,action:'click',x:100,y:200,
      ...(step.openMenu?{openMenu:{x:90,y:190}}:{})}));
    const driver={id,stimulus:'Detached case-specific checkpoints',steps};
    contracts.validateDeclaration(driver,{id});
    for (const point of keypoints) {
      const step=steps.find(row=>row.id===point);
      assert.ok(step?.checks.length>1);
      reject('resource-ID-alone-cannot-prove-'+point,/RENDERED_CASE_OBSERVATIONS/,() =>
        contracts.validateDeclaration({...driver,steps:steps.map(row=>row===step?{...row,checks:[row.checks[0]]}:row)},{id}));
    }
  }
  const put=(name,value)=>e.writeExclusive(directory,'authored-coverage/'+name,value);
  const tree=put('tree.bin',encode(['scene:scene_tree',1,[1,'root','Window',1,'',0,1,'WorldPreview','Node2D',2,'',0,
    0,'@CPUParticles2D@942','CPUParticles2D',3,'',0]]));
  const child='/root/WorldPreview/@CPUParticles2D@942';
  const packet=put('effect.bin',encode(['scene:inspect_objects',1,[[3,'CPUParticles2D',[
    ['metadata/resource_id',0,0,'',0,'effect.character.death']
  ]]]]));
  const at='2026-11-20T11:00:00.000Z', objects=[{node:child,packet}];
  const exactNative=put('effect.json',{at,tree,objects});
  const checks=[{node:'/root/WorldPreview',childClass:'CPUParticles2D',childIndex:0,property:'metadata/resource_id',
    match:'equal',value:'effect.character.death'}];
  assert.deepEqual(savedNativeSnapshot(directory,exactNative,checks,1).values,[{id:3,value:'effect.character.death'}]);
  const stray=put('wrong-node.json',{at,tree,objects:[{node:'/root/WorldPreview',packet}]});
  reject('foreign-particle-object-path',/EVIDENCE_SET/,()=>savedNativeSnapshot(directory,stray,checks,1));
  const corpse=put('corpse.png',Buffer.from('detached image reference'));
  const live=put('active-death.png',Buffer.from('detached active image reference'));
  const retired=put('retired-death.png',Buffer.from('detached retired image reference'));
  const final={outcomes:[{id:'heldout.H12',status:'pass',checkpoints:[{id:'persistent-corpse',image:corpse},
    {id:'death-step',image:live},{id:'death-retired',image:retired}]}]};
  const judgment={id:'P4',caseIds:['heldout.H12'],files:[live,retired]};
  e.validateJudgmentImages(judgment,final);
  reject('corpse-only-does-not-prove-P4',/EVIDENCE_JUDGMENT_DEATH_IMAGE death-step/,() =>
    e.validateJudgmentImages({...judgment,files:[corpse]},final));
  reject('P4-requires-effect-retirement-frame',/EVIDENCE_JUDGMENT_DEATH_IMAGE death-retired/,() =>
    e.validateJudgmentImages({...judgment,files:[live]},final));
}

function desktopHelperControl(directory) {
  const e=require('../evidence.cjs'), tool=require('../export.cjs');
  const executable=require('../rendered-process.cjs').windowsPowerShell();
  const profile=path.join(directory,'desktop-helper-profile'), trace=path.join(directory,'desktop-helper-host.log');
  const env=tool.validatePortableEnvironment({...tool.cleanEnvironment(profile),COREHOST_TRACE:'1',COREHOST_TRACEFILE:trace},profile,trace);
  const invoke=args=>spawnSync(executable,args,{cwd:ROOT,env,encoding:'utf8',windowsHide:true,timeout:12000});
  const inert=invoke(['-NoProfile','-NonInteractive','-Command','Write-Output KZ_HELPER_OK']);
  assert.ifError(inert.error);
  assert.equal(inert.status,0,inert.stderr);
  assert.equal(inert.stdout.trim(),'KZ_HELPER_OK');
  const probe=invoke(['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(ROOT,'tools/phase2/windows-input.ps1'),
    '-OwnedPid','0','-Action','probe','-X','0','-Y','0','-Key','0']);
  assert.ifError(probe.error);
  assert.equal(probe.status,1,probe.stdout);
  assert.match(probe.stderr,/OWNED_WINDOW_MISSING/);
  return e.writeExclusive(directory,'desktop-helper-control.json',{executable,path:env.PATH,inert:inert.stdout.trim(),
    inertPid:0,probe:probe.status,ownedWindow:'missing',physicalInput:false});
}

function buildIdentityControls(directory,reject) {
  const e=require('../evidence.cjs'), s=require('../scenarios.cjs'), tool=require('../export.cjs');
  const relative='detached-package', target=path.join(directory,relative);
  const packageRef=e.writeExclusive(directory,relative+'/KingdomZombies.exe',Buffer.from('Template fixture'));
  const pck=e.writeExclusive(directory,relative+'/KingdomZombies.pck',Buffer.from('Healthy PCK fixture'));
  e.writeExclusive(directory,relative+'/KingdomZombies.dll',Buffer.from('Current assembly fixture'));
  const build={portable:{files:tool.inventory(target)}}, subject={fixture:null,executable:packageRef,pck};
  s.validateProcessPackageFiles(directory,build,subject);
  fs.writeFileSync(path.join(target,'KingdomZombies.pck'),'Changed PCK fixture');
  reject('changed-pck-under-same-executable',/RENDERED_LIVE_PORTABLE_CLOSURE/,() => s.validateProcessPackageFiles(directory,build,subject));
  fs.writeFileSync(path.join(target,'KingdomZombies.pck'),'Healthy PCK fixture');
  const unapproved=path.join(target,'override.cfg');
  fs.writeFileSync(unapproved,'fixture');
  reject('unexpected-portable-companion',/RENDERED_LIVE_PORTABLE_CLOSURE/,() => s.validateProcessPackageFiles(directory,build,subject));
  fs.unlinkSync(unapproved);
  const profile=path.join(directory,'profile'),trace=path.join(directory,'host.log');
  const env={...tool.cleanEnvironment(profile),COREHOST_TRACE:'1',COREHOST_TRACEFILE:trace};
  tool.validatePortableEnvironment(env,profile,trace);
  reject('foreign-runtime-path',/RUNTIME_SEARCH_OVERRIDE/,() => tool.validatePortableEnvironment({...env,DOTNET_ROOT:'foreign'},profile,trace));
  reject('foreign-runtime-search-case',/RUNTIME_SEARCH_OVERRIDE/,() => tool.validatePortableEnvironment({...env,Path:'foreign'},profile,trace));
  const project='prepared-source', sourceFiles={files:[{path:'src/godot-game/project.godot'}]};
  e.writeExclusive(directory,project+'/project.godot',Buffer.from('config_version=5'));
  e.writeExclusive(directory,project+'/.godot/uid_cache.bin',Buffer.from('detached cache'));
  e.writeExclusive(directory,project+'/.godot/global_script_class_cache.cfg',Buffer.from('list=[]'));
  assert.equal(s.sourceImportCompanions(directory,project,sourceFiles).length,2);
  for (const alias of ['project.binary','Override.cfg','.godot/extension_list.cfg','.godot/imported/substitute.remap']) {
    const extra=e.writeExclusive(directory,project+'/'+alias,Buffer.from('detached alternate'));
    reject('alternate-source-'+alias,/RENDERED_SOURCE_RUNTIME_REDIRECT/,() => s.sourceImportCompanions(directory,project,sourceFiles));
    fs.unlinkSync(path.join(directory,extra.path));
  }
  const revision='c'.repeat(40), launch=crypto.randomUUID(), names=['KingdomZombies','KingdomZombies.Core','KingdomZombies.Multiplayer.Abstractions'];
  const source={revision,engine:{path:'engine.exe'},sourceManaged:Object.fromEntries(names.map(name=>[name,
    {file:'source-project/.godot/mono/temp/bin/Debug/'+name+'.dll',mvid:crypto.randomUUID()}]))};
  const observations=names.map(name=>({name,launch,pid:1234,executable:path.join(directory,source.engine.path),
    file:'',loadContext:'GodotPlugins.PluginLoadContext',
    version:'0.1.0+'+revision,mvid:source.sourceManaged[name].mvid}));
  const record=(name,rows) => ({role:'solo',pid:1234,launch,assemblies:e.writeExclusive(directory,name+'.ndjson',
    Buffer.from(rows.map(row=>JSON.stringify(row)).join('\n')+'\n'))});
  s.validateLoadedSource(directory,source,record('current-runtime',observations));
  reject('stale-loaded-dll',/RENDERED_SOURCE_RUNTIME_IDENTITY/,() => s.validateLoadedSource(directory,source,
    record('stale-dll',observations.map(row=>({...row,version:'0.1.0+'+'a'.repeat(40)})))));
  reject('substituted-managed-module-id',/RENDERED_SOURCE_RUNTIME_IDENTITY/,() => s.validateLoadedSource(directory,source,
    record('substituted-dll',observations.map(row=>({...row,mvid:crypto.randomUUID()})))));
  reject('substituted-managed-loader',/RENDERED_SOURCE_RUNTIME_IDENTITY/,() => s.validateLoadedSource(directory,source,
    record('substituted-loader',observations.map(row=>({...row,loadContext:'foreign-context'})))));
  reject('wrong-managed-runtime-owner',/RENDERED_SOURCE_RUNTIME_IDENTITY/,() => s.validateLoadedSource(directory,source,
    record('wrong-owner',observations.map(row=>({...row,pid:5678})))));
  reject('wrong-observed-engine',/RENDERED_SOURCE_RUNTIME_IDENTITY/,() => s.validateLoadedSource(directory,source,
    record('wrong-engine',observations.map(row=>({...row,executable:packageRef.path})))));
}

// A hard-terminated attempt keeps start.json without a terminal. The receipt is refused while its runner or a journaled
// owned launch survives, then records a failed interruption that the campaign history accepts.
async function interruptionControls(directory,reject) {
  const e = require('../evidence.cjs'), scenarios = require('../scenarios.cjs');
  const {current,report,item} = baseFixture();
  const root = path.join(directory,'hard-termination'), campaign = crypto.randomUUID(), invocation = crypto.randomUUID();
  const relative = '.phase4/acceptance/'+invocation;
  const drivers = e.writeExclusive(root,relative+'/declared-drivers.json',{campaign});
  const opening = {...current,schemaVersion:1,invocation,campaign,startedAt:report.startedAt,previousInvocation:null,kind:'initial',
    selected:[item.id],drivers,reviewSha256:'a'.repeat(64),selectionSha256:'b'.repeat(64),gateRevision:'0'.repeat(40),
    newlyAffectedCaseIds:[],runner:{pid:900,startedAt:1000}};
  e.writeExclusive(root,relative+'/start.json',opening);
  e.writeExclusive(root,relative+'/owned-processes.ndjson',Buffer.from([{pid:910,trackedAt:2000},{pid:910,trackedAt:2000,exitedAt:3000},
    {pid:920,trackedAt:40000}].map(row => JSON.stringify(row)).join('\n')+'\n'));
  e.writeExclusive(root,'.phase2/desktop.lock',{pid:900,nonce:invocation,at:report.startedAt});
  const row = (pid, parent, created) => ({pid,parent,created});
  const record = (rows, cases = new Map([[item.id,item]])) => scenarios.recordInterruption(root,invocation,{rows:cases,
    observer:{inventory:() => ({self:999,rows}),kill:() => assert.fail('An interruption receipt never kills'),now:() => 100000}});
  const rejectAsync = async (name, pattern, action) => {
    await assert.rejects(action,pattern,name);
    reject(name,pattern,() => {throw new Error(pattern.source);});
  };
  // A refusal changes nothing: the lock stays with the attempt and no receipt is written, so the call can be retried.
  const untouched = () => {
    assert.equal(fs.existsSync(path.join(root,'.phase2/desktop.lock')),true);
    assert.deepEqual(fs.readdirSync(path.join(root,relative)).sort(),['declared-drivers.json','owned-processes.ndjson','start.json']);
  };
  await rejectAsync('interruption-unresolved-case',/RENDERED_INTERRUPTION_CASE/,() => record([row(900,1,50000)],new Map()));
  untouched();
  await rejectAsync('interruption-runner-still-alive',/RENDERED_INTERRUPTION_RUNNER_ALIVE/,() => record([row(900,1,999)]));
  untouched();
  await rejectAsync('interruption-owned-launch-survives',/RENDERED_INTERRUPTION_SURVIVORS/,() => record([row(920,1,39999),row(921,920,40500)]));
  untouched();
  const recorded = await record([row(900,1,50000),row(910,4,60000),row(930,910,61000),row(920,4,70000),row(940,920,3000)]);
  assert.equal(recorded.releasedLock,true);
  assert.equal(fs.existsSync(path.join(root,'.phase2/desktop.lock')),false);
  const history = e.historyDocuments(root,[recorded.report],campaign);
  assert.equal(history[0].status,'fail');
  assert.deepEqual(history[0].outcomes.map(row => row.status),['blocked']);
  await rejectAsync('interruption-already-terminal',/RENDERED_INTERRUPTION_TERMINAL/,() => record([]));
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
  // A declaration that never wrote start.json launched nothing, even when its declared time was in the future.
  e.writeExclusive(directory,'.phase4/acceptance/'+crypto.randomUUID()+'/declared-drivers.json',
    {campaign,declaredAt:'2099-01-01T00:00:00.000Z'});
  const head = e.git(ROOT,['rev-parse','HEAD']).trim(), parent = e.git(ROOT,['rev-parse','HEAD~1']).trim();
  const scope = {head,reviewSha256:'a'.repeat(64),selectionSha256:'b'.repeat(64)};
  assert.equal(e.historyDocuments(directory,[first,end],campaign,scope).length,2);
  // A second campaign may not hide an attempt made under the same review, selection or gate lineage.
  const attempt = (base, owner, fields = {}) => {
    const relative = '.phase4/acceptance/'+crypto.randomUUID();
    const drivers = e.writeExclusive(base,relative+'/declared-drivers.json',{campaign:owner});
    const opening = {...current,...report,invocation:path.posix.basename(relative),campaign:owner,drivers,
      reviewSha256:'c'.repeat(64),selectionSha256:'d'.repeat(64),gateRevision:'0'.repeat(40),...fields};
    const start = e.writeExclusive(base,relative+'/start.json',opening);
    return e.writeExclusive(base,relative+'/report.json',{...opening,start,status:'fail'});
  };
  const concealed = [['second-campaign-unrelated',null],['second-campaign-same-review',{reviewSha256:scope.reviewSha256}],
    ['second-campaign-same-selection',{selectionSha256:scope.selectionSha256}],['second-campaign-ancestor-gate',{gateRevision:parent}]];
  for (const [name,fields] of concealed) {
    const base = path.join(directory,name);
    const own = attempt(base,campaign,{reviewSha256:scope.reviewSha256,selectionSha256:scope.selectionSha256});
    attempt(base,crypto.randomUUID(),fields ?? {});
    if (!fields) assert.equal(e.historyDocuments(base,[own],campaign,scope).length,1);
    else reject(name,/EVIDENCE_CAMPAIGN_CONCEALED/,() => e.historyDocuments(base,[own],campaign,scope));
  }

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

function repairHistoryControls(reject) {
  const e=require('../evidence.cjs'), s=require('../scenarios.cjs');
  const {current,report:base}=baseFixture();
  const items=['heldout.H12','heldout.H13'].map(id=>({id,runGroup:'G1-hub',expected:id+' expected state'}));
  const [h12,h13]=items.map(row=>row.id), campaign=crypto.randomUUID();
  function report(kind,previous,cases,affected=[]) {
    const invocation=crypto.randomUUID(), startedAt=previous?.finishedAt??base.startedAt;
    const outcomes=cases.map(([index,status])=>({...items[index],invocation,status,execution:'executed-now',observed:'Detached history control'}));
    return {...current,campaign,schemaVersion:1,invocation,kind,previousInvocation:previous?.invocation??null,startedAt,
      finishedAt:new Date(Date.parse(startedAt)+1000).toISOString(),repairReason:kind==='focused'?'Scoped repair':undefined,
      selected:outcomes.map(row=>row.id),outcomes,newlyAffectedCaseIds:affected,status:outcomes.every(row=>row.status==='pass')?'pass':'fail'};
  }
  const initial=report('initial',null,[[0,'fail'],[1,'pass']]);
  const unrelated=report('focused',initial,[[1,'pass']]);
  const invalidFinal=report('final',unrelated,[[0,'pass'],[1,'pass']]);
  reject('focused-unrelated-H13-cannot-clear-H12',/EVIDENCE_REPAIR_PROOF.*heldout\.H12/,() => e.freshFinal(items,[initial,unrelated,invalidFinal],current));
  const repaired=report('focused',initial,[[0,'pass']]);
  const final=report('final',repaired,[[0,'pass'],[1,'pass']]);
  assert.equal(e.freshFinal(items,[initial,repaired,final],current),final);
  const partial=report('focused',initial,[[0,'pass'],[1,'invalid']],[h13]);
  const remaining=report('focused',partial,[[1,'pass']]);
  const rest=report('final',remaining,[[0,'pass'],[1,'pass']]);
  assert.equal(e.freshFinal(items,[initial,partial,remaining,rest],current),rest);
  reject('final-skips-unresolved-invalid-case',/EVIDENCE_REPAIR_PROOF/,() =>
    e.freshFinal(items,[initial,partial,report('final',partial,[[0,'pass'],[1,'pass']])],current));
  const selection={...current,groups:[{id:'G1-hub',cases:[h12,h13]}],selected:[
    {id:h12,dependencies:['src/godot-game/inspection/BuildingPreview.cs']},{id:h13,dependencies:['src/godot-game/inspection/CharacterPreview.cs']}]};
  const affected=s.repairAffected(selection,selection,['src/godot-game/inspection/CharacterPreview.cs']);
  assert.deepEqual(affected,[h13]);
  const previous={path:'detached/initial.json',sha256:'a'.repeat(64),bytes:1};
  const declaration={...current,campaign,kind:'focused',declaredAt:initial.startedAt,previousRun:previous,repairReason:partial.repairReason,
    caseIds:partial.selected,newlyAffectedCaseIds:[h13]};
  assert.deepEqual(s.attemptDeclaration(partial,declaration,selection,previous,affected).map(row=>row.id),[h12,h13]);
  reject('focused-report-does-not-match-own-declaration',/EVIDENCE_SET/,() =>
    s.attemptDeclaration(partial,{...declaration,caseIds:[h13]},selection,previous,affected));
  reject('focused-declaration-omits-real-new-impact',/EVIDENCE_SET/,() =>
    s.attemptDeclaration(partial,{...declaration,newlyAffectedCaseIds:[]},selection,previous,affected));
  reject('focused-declaration-substitutes-prior-input',/EVIDENCE_ATTEMPT_DECLARATION/,() =>
    s.attemptDeclaration(partial,{...declaration,previousRun:{...previous,bytes:2}},selection,previous,affected));
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

function inheritedReceiptControls(directory,reject) {
  const e = require('../evidence.cjs'), scenarios = require('../scenarios.cjs');
  const baseline = require('../../phase1/check.cjs'), phase2 = require('../../phase2/evidence.cjs');
  const {current} = baseFixture();
  const root = path.join(directory,'inherited-receipts');
  fs.mkdirSync(root);
  const originals = new Map([
    ['phase1',{id:'R-50.positive',implementation:'csharp',callerStages:{catalog:'phase1'}}],
    ['phase2',{id:'core.permissions',implementation:'csharp',callerStages:{catalog:'phase2'}}]
  ]);
  const refs = [];
  for (const [family,item] of originals) {
    e.writeExclusive(root,'tools/'+family+'/required-cases.json',{cases:[item]});
    const nonce = crypto.randomUUID(), platform = family==='phase1'?'win32-x64-'+require('node:os').release():'win32-x64';
    const result = {id:item.id,status:'pass'};
    const prefix = family==='phase1'?'.phase1/runs/':'.phase2/runs/win32-x64/';
    const ref = e.writeExclusive(root,prefix+nonce+'/summary.json',{
      nonce,platform,revision:current.revision,startedAt:1,status:'pass',selected:[item.id],results:[result],
      input:'6'.repeat(64),tool:'7'.repeat(64)
    });
    refs.push({id:family+'/'+item.id,result:ref});
  }
  e.writeExclusive(root,'tools/phase3/required-cases.json',{cases:[]});
  const selected = scenarios.inheritedCases(root), called = [];
  const phase1Verifier = baseline.verifyCallerResult, phase2Verifier = phase2.reconcile;
  const binding = scenarios.inheritedBinding, bindingInput = binding.input;
  // The fixture revision has no commit, so the committed-input recomputation is substituted.
  let committedInput = '6'.repeat(64);
  binding.input = (_root,_family,result) => {assert.equal(result.revision,current.revision);return committedInput;};
  try {
    baseline.verifyCallerResult = (item,_result,context) => {
      assert.deepEqual(item,originals.get('phase1'));
      assert.equal(context.directory,path.join(root,path.posix.dirname(refs[0].result.path)));
      called.push('phase1');
    };
    phase2.reconcile = (items,_results,context) => {
      assert.deepEqual(items,[originals.get('phase2')]);
      assert.equal(context.directory,path.join(root,path.posix.dirname(refs[1].result.path)));
      called.push('phase2');
    };
    assert.deepEqual([...scenarios.sourceReceipts(root,selected,refs,current)],refs.map(row=>row.id));
    assert.deepEqual(called,['phase1','phase2']);
    // Same revision name, but the tested files differ from that revision's committed bytes.
    committedInput = '8'.repeat(64);
    reject('inherited-receipt-uncommitted-input',/COMPLETION_INHERITED_INPUT/,() => scenarios.sourceReceipts(root,selected,refs,current));
    committedInput = '6'.repeat(64);
    // A later rebuild restamps the shared test assembly with a newer HEAD. That must not invalidate the receipt.
    assert.equal(Object.hasOwn(binding,'tool'),false);
    called.length = 0;
    assert.deepEqual([...scenarios.sourceReceipts(root,selected,refs,current)],refs.map(row=>row.id));
  } finally {
    baseline.verifyCallerResult = phase1Verifier;
    phase2.reconcile = phase2Verifier;
    binding.input = bindingInput;
  }
  // The committed recomputation reproduces the inherited runners' working-tree snapshot for clean tracked inputs.
  const head = e.git(ROOT,['rev-parse','HEAD']).trim(), sample = ['AGENTS.md','tools/phase1/toolchain.json','tools/phase2'];
  assert.equal(e.committedSnapshot(ROOT,head,sample),baseline.snapshot(ROOT,{inputs:sample}).sha256);
  reject('committed-input-missing',/EVIDENCE_INPUT_MISSING/,() => e.committedSnapshot(ROOT,head,['tools/phase4/missing-input']));

  const original = phase2.catalog(ROOT).cases.find(row=>row.implementation==='native');
  const item = {id:'phase2/'+original.id,family:'phase2',runGroup:'G3-solo'};
  const report = {...current,revision:e.git(ROOT,['rev-parse','HEAD']).trim(),invocation:crypto.randomUUID()};
  const bindings = {nonce:report.invocation,input:report.source,tool:report.tool,revision:report.revision,platform:'win32-x64'};
  const subject = {bindings,result:{...bindings,id:original.id,status:'pass'},runtime:[]};
  const owned = '.phase4/acceptance/'+report.invocation+'/'+item.runGroup+'/';
  const fixture = e.writeExclusive(root,owned+original.id+'-result.json',subject);
  const copied = e.writeExclusive(root,owned+'copied-result.json',subject);
  reject('historical-inherited-copied-receipt',/EVIDENCE_INHERITED_RESULT_PATH/,() =>
    scenarios.verifyInheritedRendered(root,report,{inherited:copied},item,{},new Map()));
  reject('historical-inherited-passes-without-family-evidence',/RESULT_IMAGE_MISSING|NATIVE_SESSION_REQUIRED/,() =>
    scenarios.verifyInheritedRendered(root,report,{inherited:fixture},item,{},new Map()));
}

function renderedDeclarationControls(directory,reject) {
  const e = require('../evidence.cjs'), scenarios = require('../scenarios.cjs');
  const {encode} = require('../native-observer.cjs');
  const contracts=require('../rendered-contracts.cjs');
  const {current,report,process,review} = baseFixture();
  const invocation=report.invocation,id='heldout.H31',runGroup='G2-packaged';
  const root = path.join(directory,'rendered-declaration');
  fs.mkdirSync(root);
  const put = (name,value) => e.writeExclusive(root,name,value);
  const fake = put('test-executable.bin',Buffer.from('Detached parser fixture'));
  const image = put('test.png',require('../../phase3/runner-fixture.cjs').png());
  const time=milliseconds=>new Date(Date.parse(report.startedAt)+milliseconds).toISOString();
  function snapshot(prefix,checks,at,before=false) {
    const paths=new Map([['/root',{id:1,name:'root'}]]);
    for(const check of checks) {
      let parent='/root';
      for(const name of check.node.split('/').slice(2)) {
        const full=parent+'/'+name;
        if(!paths.has(full))paths.set(full,{id:paths.size+1,parent,name});
        parent=full;
      }
    }
    function walk(full) {
      const node=paths.get(full),children=[...paths].filter(([,child])=>child.parent===full);
      return [children.length,node.name,'Node',node.id,'',0,...children.flatMap(([name])=>walk(name))];
    }
    const tree=put(prefix+'-tree.bin',encode(['scene:scene_tree',1,walk('/root')]));
    const objects=[...new Set(checks.map(check=>check.node))].map((node,index)=>{
      const rows=new Map(checks.filter(check=>check.node===node).map(check=>[check.property,
        before&&check===checks[0]?'detached previous observation':check.value]));
      const properties=[...rows].map(([property,value])=>[property,0,0,'',0,value]);
      return {node,packet:put(prefix+'-object-'+index+'.bin',encode(['scene:inspect_objects',1,[[paths.get(node).id,'Node',properties]]]))};
    });
    return put(prefix+'.json',{at,tree,objects});
  }
  const searchPath='/root/Inspection/Shell/Workspace/Controls/Browser/Search';
  const searchStep={id:'navigate-search',lifetime:1,navigate:true,control:searchPath,action:'tap',key:81,focusClick:{x:110,y:210},
    checks:[{node:searchPath,property:'text',match:'equal',value:'q'}]};
  const driver={id,fixture:{id,kind:'portable-positive'},stimulus:'Observe two detached physical declaration shapes',
    steps:contracts.template(id).map(step=>({...step,action:'click',x:100,y:200}))};
  driver.steps.splice(1,0,searchStep);
  const row={id,runGroup,checkpoints:[],processes:[]};
  for(const lifetime of [1,2]) {
    const offset=(lifetime-1)*1000,pid=process.pid+lifetime;
    const runtime=put('runtime-'+lifetime+'.json',{pidPacket:put('pid-'+lifetime+'.bin',encode(['set_pid',1,[pid]]))});
    const ready=snapshot('ready-'+lifetime,contracts.readyChecks,time(offset+10));
    row.processes.push(put('process-'+lifetime+'.json',{...process,pid,role:'packaged-inspection',
      startedAt:time(offset),finishedAt:time(offset+990),runtime,ready,log:fake,executable:fake,profile:'profiles/'+lifetime}));
    for(const [index,step] of driver.steps.filter(step=>step.lifetime===lifetime).entries()) {
      const tick=offset+100+index*100,token=crypto.randomUUID();
      const flags={caseId:id,checkpoint:step.id,invocation,pid,token};
      const acknowledged={pid,focused:true,width:1280,height:720,window:71};
      const input=put(step.id+'-input.json',{...flags,at:time(tick+2),actionStartedAt:time(tick+1),focused:true,
        clientWidth:1280,clientHeight:720,method:step.action==='tap'?'physical-keyboard':'physical-mouse',
        action:step.action,x:step.x,y:step.y,key:step.key,control:step.control,
        acknowledgement:{...acknowledged,action:step.action,key:step.key},
        focusClick:step.focusClick&&{...step.focusClick,at:time(tick+1),acknowledgement:{...acknowledged,action:'click'}}});
      const draw=put(step.id+'-draw.json',{...flags,at:time(tick+6),drawn:true,frame:index+1,
        frameSource:'composited-client-between-native-observations',ready,
        before:snapshot(step.id+'-before',step.checks,time(tick),true),
        observed:snapshot(step.id+'-observed',step.checks,time(tick+3)),
        after:snapshot(step.id+'-after',step.checks,time(tick+5)),imageSha256:image.sha256,
        capture:{pid,window:71,token,at:time(tick+4),imageSha256:image.sha256}});
      row.checkpoints.push({id:step.id,input,draw,image});
    }
  }
  const gate = {review:{sha256:'7'.repeat(64)},selection:{sha256:'8'.repeat(64)},reviewedAt:review.reviewedAt};
  const declaration = {...current,schemaVersion:2,reviewSha256:gate.review.sha256,selectionSha256:gate.selection.sha256,
    campaign:crypto.randomUUID(),declaredAt:review.reviewedAt,kind:'initial',drivers:[driver]};
  assert.deepEqual([...contracts.CASES.keys()].sort(),require('../handoff.json')['4'].slice().sort());
  assert.equal(scenarios.reviewedDrivers(declaration,gate,current,[row],'initial').get(id),driver);
  const declarations=new Map();
  for (const caseId of contracts.CASES.keys()) {
    const steps=contracts.template(caseId).map(step=>({...step,action:step.key===undefined?'click':'tap',x:100,y:200,
      ...(step.openMenu?{openMenu:{x:110,y:210}}:{}),...(step.recheck?{recheck:{x:120,y:220}}:{}),
      ...(step.focusClick?{focusClick:{x:130,y:230}}:{})}));
    const authored={id:caseId,stimulus:'Detached retained procedure',steps};
    contracts.validateDeclaration(authored,{id:caseId});
    declarations.set(caseId,authored);
  }
  function alterStep(caseId,id,change) {
    const authored=declarations.get(caseId);
    const steps=authored.steps.map(step=>step.id===id?{...step,...change}:step);
    return contracts.validateDeclaration({...authored,steps},{id:caseId});
  }
  for (const [caseId,id,index] of [['heldout.H13','overlap',6],['heldout.H15','gear-away',2],['heldout.H15','gear-order',6]]) {
    assert.equal(declarations.get(caseId).steps.find(step=>step.id===id).checks[0].value,index);
    reject('missing-gear-menu-'+id,/RENDERED_CASE_MENU/,()=>alterStep(caseId,id,{openMenu:null}));
  }
  reject('search-key-without-focus-click',/RENDERED_SEARCH_FOCUS/,
    ()=>alterStep('heldout.H21','zero-results',{focusClick:undefined}));
  reject('search-click-without-key',/RENDERED_SEARCH_FOCUS/,
    ()=>alterStep('heldout.H21','zero-results',{action:'click'}));
  reject('search-with-wrong-key',/RENDERED_CASE_KEY/,()=>alterStep('heldout.H21','zero-results',{key:90}));
  const bowId='heldout.H12',category='/root/Inspection/Shell/Workspace/Navigation/Category';
  const bowSteps=contracts.template(bowId).map(step=>({...step,action:'click',x:100,y:200,
    ...(step.openMenu?{openMenu:{x:110,y:210}}:{})}));
  const navigate={id:'navigate-characters',lifetime:1,navigate:true,action:'click',x:100,y:200,
    openMenu:{x:110,y:210},control:category,checks:[{node:category,property:'selected',match:'equal',value:0}]};
  const bow=steps=>contracts.validateDeclaration({id:bowId,stimulus:'Detached bow menu controls',steps},{id:bowId});
  bow([navigate,...bowSteps]);
  reject('missing-physical-character-menu-open',/RENDERED_CASE_MENU/,()=>bow([
    {...bowSteps[0],openMenu:null},...bowSteps.slice(1)]));
  reject('missing-physical-category-menu-open',/RENDERED_CASE_MENU/,()=>bow([{...navigate,openMenu:null},...bowSteps]));
  reject('non-distinct-physical-menu-clicks',/RENDERED_CASE_MENU/,()=>bow([{...navigate,openMenu:{x:100,y:200}},...bowSteps]));
  reject('unavailable-navigation-menu-item',/RENDERED_NAVIGATION_MENU/,()=>bow([
    {...navigate,checks:[{...navigate.checks[0],value:8}]},...bowSteps]));
  e.validateRenderedOutcome(root,row,report,driver);
  const searchPoint=row.checkpoints.find(point=>point.id===searchStep.id);
  const searchInput=e.document(root,searchPoint.input);
  for (const [label,focusClick] of [['missing',undefined],['wrong-window',{...searchInput.focusClick,
    acknowledgement:{...searchInput.focusClick.acknowledgement,window:72}}]]) {
    const input=put(label+'-search-focus.json',{...searchInput,focusClick});
    reject(label+'-physical-search-focus',/EVIDENCE_PHYSICAL_SEARCH_FOCUS/,()=>e.validateRenderedOutcome(root,
      {...row,checkpoints:row.checkpoints.map(point=>point===searchPoint?{...point,input}:point)},report,driver));
  }
  reject('omitted-saved-relaunch-checkpoint',/EVIDENCE_SET/,() => e.validateRenderedOutcome(root,{...row,checkpoints:row.checkpoints.slice(0,1)},report,driver));
  reject('omitted-second-owned-launch',/EVIDENCE_RENDERED_LAUNCHES/,() => e.validateRenderedOutcome(root,{...row,processes:row.processes.slice(0,1)},report,driver));
  reject('reordered-saved-checkpoints',/EVIDENCE_SET/,() => e.validateRenderedOutcome(root,{...row,checkpoints:row.checkpoints.slice().reverse()},report,driver));
  reject('declared-physical-action-mismatch',/EVIDENCE_NATIVE_INPUT/,() => e.validateRenderedOutcome(root,row,report,
    {...driver,steps:[{...driver.steps[0],x:101},...driver.steps.slice(1)]}));
  reject('missing-rendered-png',/EVIDENCE_PATH_MISSING unwritten\.png/,() => e.validateRenderedOutcome(root,
    {...row,checkpoints:[{...row.checkpoints[0],image:{...image,path:'unwritten.png'}},...row.checkpoints.slice(1)]},report,driver));
  const preflight=steps=>scenarios.reviewedDrivers({...declaration,drivers:[{...driver,steps}]},gate,current,[row],'initial');
  reject('no-ordinary-reset',/EVIDENCE_SET/,()=>preflight(driver.steps.filter(step=>!step.id.startsWith('reset-'))));
  reject('title-only-declaration',/RENDERED_CASE_OBSERVATIONS/,()=>preflight([{...driver.steps[0],
    checks:[{node:'/root/Inspection/Summary/Title',property:'text',match:'equal',value:'Title fixture'}]},...driver.steps.slice(1)]));
  reject('tab-only-action',/RENDERED_CASE_ACTION/,()=>preflight([{...driver.steps[0],action:'tap',key:9},...driver.steps.slice(1)]));
  const draw=e.document(root,row.checkpoints[0].draw),mutate=(name,pattern,edit)=>reject(name,pattern,()=>{
    const changed=put(name+'.json',edit(structuredClone(draw)));
    e.validateRenderedOutcome(root,{...row,checkpoints:[{...row.checkpoints[0],draw:changed},...row.checkpoints.slice(1)]},report,driver);
  });
  mutate('draw-for-another-physical-input',/EVIDENCE_PHYSICAL_RECEIPT/,value=>({...value,capture:{...value.capture,token:crypto.randomUUID()}}));
  mutate('stale-before-ready-draw',/EVIDENCE_NATIVE_CAPTURE_ORDER/,value=>({...value,
    observed:snapshot('old-state',driver.steps[0].checks,time(2))}));
  mutate('absent-native-transition',/EVIDENCE_NATIVE_TRANSITION/,value=>({...value,
    before:snapshot('unchanged-before',driver.steps[0].checks,time(100))}));
  mutate('native-change-around-visible-capture',/EVIDENCE_NATIVE_CAPTURE_DRIFT/,value=>({...value,
    after:snapshot('changed-after',driver.steps[0].checks,time(105),true)}));
  reject('stale-driver-review',/RENDERED_INPUT_SCHEMA/,() => scenarios.reviewedDrivers({...declaration,reviewSha256:'a'.repeat(64)},gate,current,[row],'initial'));
  const tool = require('../export.cjs'), header = Buffer.alloc(112);
  header.write('GDPC');
  [4,4,7,2,2].forEach((value,index) => header.writeUInt32LE(value,4+index*4));
  header.writeBigUInt64LE(112n,24);
  const named = 'content/packs/items-11-24.json', sibling = 'content/packs/items-01-10.json';
  const positive = tool.packBytes(header,new Map([[named,Buffer.from('subject')],[sibling,Buffer.from('sibling')]]));
  for (const [fixture,alias] of Object.entries(tool.RENDERED_FIXTURES)) {
    const {data,...ref} = tool.renderedFixture(positive,fixture);
    const files = tool.pack(tool.verifyRenderedFixture(positive,fixture,ref));
    assert.equal(files.has(named),false);
    assert.equal(files.get(sibling).data.toString(),'sibling');
    assert.equal(files.has('content/packs/items-11-24-copy.json'),Boolean(alias));
    reject('healthy-pck-used-for-'+fixture,/PORTABLE_FIXTURE_HASH/,() =>
      tool.verifyRenderedFixture(positive,fixture,{bytes:positive.length,sha256:require('../content.cjs').hash(positive)}));
  }
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

async function processIdentityControls(pass) {
  const {observeChildren} = require('../rendered-process.cjs');
  let clock = 0, snapshot;
  const killed = [];
  const child = pid => {
    const listeners = [];
    return {pid,exitCode:null,signalCode:null,once:(event,listener) => {if (event === 'exit') listeners.push(listener);},
      exit() {this.exitCode = 0;for (const listener of listeners) listener();}};
  };
  const owner = observeChildren({inventory:() => snapshot,kill:pids => {killed.push(...pids);
    snapshot = {...snapshot,rows:snapshot.rows.filter(row => !pids.includes(row.pid))};},now:() => clock,settle:async () => {}});
  const at = (time, action) => {clock = time;return action();};
  const helper = at(10000,() => owner.track(child(100)));
  at(11000,() => helper.exit());
  const game = at(12000,() => owner.track(child(400)));
  const query = at(13000,() => owner.track(child(600)));
  at(13500,() => query.exit());
  clock = 20000;
  const row = (pid, parent, created) => ({pid,parent,created});
  snapshot = {self:600,rows:[row(100,4,15000),row(200,100,15500),row(300,100,3000),row(500,100,10500),row(400,1,11995),
    row(401,400,12500),row(402,401,12600),row(403,400,11000),row(600,1,19990),row(601,600,19995),row(700,1,5000)]};
  assert.equal(game.pid,400);
  const owned = (await owner.remaining()).sort((a,b) => a-b);
  assert.deepEqual(owned,[400,401,402,500]);
  const excluded = {'recycled-helper-pid':100,'descendant-of-recycled-pid':200,'stale-parent-before-launch':300,
    'stale-parent-before-game':403,'inventory-query-recycled-pid':600,'inventory-query-descendant':601,'unrelated-process':700};
  for (const [name,pid] of Object.entries(excluded)) {
    assert.equal(owned.includes(pid),false,name);
    pass('process-identity-'+name);
  }
  assert.deepEqual(await owner.cleanup(),[]);
  assert.deepEqual(killed.sort((a,b) => a-b),[400,401,402,500]);
  pass('process-identity-kills-only-identified');
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
    // An unchanged group folder does not prove a case unaffected: its own runner and test project are inputs too.
    for (const file of ['tools/phase1/check.cjs','tests/phase1/PinEdgeTests.cs']) {
      const runnerImpact = scenarios.impactClosure(cases,[file]);
      assert.equal(runnerImpact.get('phase1/unaffected').affected,true,file);
      reject('omit-pins-after-'+file,/SELECTION_IMPACT_MISMATCH/,() =>
        scenarios.validateSelection(structuredClone(record),cases,catalog,current,runnerImpact));
    }
    const phase2 = {id:'phase2/core.unaffected',family:'phase2',group:'core',importance:'Normal',needsGameWindow:false,
      runGroup:'G0',order:5,processRoles:['source']};
    const withPhase2 = [...cases,phase2], phase2Record = structuredClone(record);
    phase2Record.omitted.push({...structuredClone(record.omitted[0]),id:phase2.id});
    assert.deepEqual(scenarios.validateSelection(structuredClone(phase2Record),withPhase2,catalog,current,
      scenarios.impactClosure(withPhase2,['tools/phase3/network.cjs'])).selected.map(item => item.id),record.selected.map(item => item.id));
    for (const file of ['tools/phase2/rendered.cjs','tools/phase1/check.cjs']) {
      reject('omit-phase2-after-'+file,/SELECTION_IMPACT_MISMATCH/,() =>
        scenarios.validateSelection(structuredClone(phase2Record),withPhase2,catalog,current,scenarios.impactClosure(withPhase2,[file])));
    }
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
    const root = context.directory;
    const file = e.writeExclusive(root,'fixture.wav',pcmFixture());
    const picture = e.writeExclusive(root,'fixture-image-reference.bin',Buffer.from('Synthetic image reference on detached fixture'));
    const authority = {id:'P1',statement:'Synthetic procedure text',criterion:'AC-22',cases:['heldout.H01']};
    const audio = audioJudgmentFixture(root,report,picture,reject);
    const final = {...report,outcomes:[{id:'heldout.H01',status:'pass',checkpoints:[{image:picture}]},audio]};
    const subjects = e.listeningSubjects(root,final), expected = e.listeningExpectations(root);
    const time = (at,delta) => new Date(Date.parse(at)+delta).toISOString();
    const row = (kind, id, fields) => ({...current,id,kind,reviewer:'Fixture Reviewer',reviewedAt:final.finishedAt,
      observations:'Synthetic check of file identity and verdict fields.',verdict:'pass',...fields});
    const listening = {...current,schemaVersion:2,invocation:final.invocation,verdicts:expected.map(item=>{
      const subject = {method:'actual-listening',caseId:audio.id,invocation:final.invocation,version:item.version,
        catalogSha256:item.catalogSha256,mediaIndex:subjects.media,reviewedAt:time(subjects.preparedAt,2000),
        listenerCapability:'actual-hearing',player:'Fixture player',outputDevice:'Fixture output'};
      if (item.kind==='mix') {
        const checkpoints = subjects.decisions.get(item.id);
        const files = [...new Map(checkpoints.flatMap(point=>point.files).map(ref=>[ref.path,ref])).values()];
        return row('mix',item.id,{...subject,checkpoints,files,playbackMethod:'live-native-mix',heardEveryCue:true,
          listenedFrom:final.startedAt,listenedThrough:final.finishedAt,
          findings:{swing:'Fixture swing',impact:'Fixture impact',confirmation:'Fixture confirmation',dayFire:'Fixture day',
            nightWind:'Fixture night',actionAndConfirmation:'Fixture action and confirmation'}});
      }
      const {wav,loop} = subjects.samples.get(item.id);
      return row('listening',item.id,{...subject,sample:item,sampleSha256:item.sampleSha256,files:loop?[wav,loop]:[wav],
        playbackMethod:loop?'repeated-loop-extract':'exact-wav-extract',heardEntireSample:true,heardLoopBoundaries:2,
        listenedFrom:time(subjects.preparedAt,10),listenedThrough:time(subjects.preparedAt,1500),
        findings:{suitability:'Fixture suitability',sourceIdentity:'Fixture source',onsetTail:'Fixture onset',silence:'Fixture silence',
          distortion:'Fixture distortion',music:'Fixture music',speech:'Fixture speech',loopSeam:'Fixture seam'}});
    })};
    const judgments = {...current,schemaVersion:1,invocation:final.invocation,verdicts:[row('judgment',authority.id,
      {method:'actual-named-review',statement:authority.statement,criterion:authority.criterion,caseIds:authority.cases,files:[picture]})]};
    const verify = (a,b=judgments,outcome=audio) => e.validateJudgments(root,a,b,current,expected,[authority],
      {...final,outcomes:[final.outcomes[0],outcome]});
    verify(listening,judgments);
    reject('absent-listening',/EVIDENCE_SET/,() => verify({...listening,verdicts:[]},judgments));
    reject('absent-judgment',/EVIDENCE_SET/,() => verify(listening,{...judgments,verdicts:[]}));
    const change = (data, fields, id=data.verdicts[0].id) => ({...data,verdicts:data.verdicts.map(row=>row.id===id?{...row,...fields}:row)});
    reject('listener-missing',/EVIDENCE_NAMED_LISTENING/,() => verify(change(listening,{reviewer:''}),judgments));
    reject('sample-version-stale',/EVIDENCE_LISTENING_SAMPLE/,() => verify(change(listening,{version:'2'}),judgments));
    reject('audio-review-output-only',/EVIDENCE_LISTENING_RUN/,() => verify(change(listening,{method:'successful-play'}),judgments));
    reject('audible-required-failure',/EVIDENCE_LISTENING_FAILED/,() => verify(change(listening,{verdict:'fail'}),judgments));
    reject('wrong-sample-digest',/EVIDENCE_AUDIO_SAMPLE_BINDING/,() => verify(change(listening,{sampleSha256:'f'.repeat(64)})));
    const sample = expected[0], loop = expected.find(row=>row.loopFrames);
    reject('unrelated-sample-route',/EVIDENCE_AUDIO_SAMPLE_BINDING/,() =>
      verify(change(listening,{sample:{...sample,routes:['other.route']}},sample.id)));
    reject('unrelated-sample-wav',/EVIDENCE_AUDIO_SAMPLE_BINDING/,() => verify(change(listening,{files:[file]})));
    reject('missing-listened-loop-iteration',/EVIDENCE_AUDIO_HEARD_FINDINGS/,() =>
      verify(change(listening,{heardLoopBoundaries:1},loop.id)));
    reject('historical-listening-context',/EVIDENCE_LISTENING_RUN/,() =>
      verify(change(listening,{invocation:crypto.randomUUID()})));
    reject('legacy-verdict-schema',/EVIDENCE_JUDGMENT_SET/,() => verify({...listening,schemaVersion:1}));
    reject('unrelated-mix-wav',/EVIDENCE_AUDIO_DECISION_WAV/,() => verify(change(listening,{files:[file]},'D-31')));
    reject('stale-mix-version',/EVIDENCE_LISTENING_SAMPLE/,() => verify(change(listening,{version:'2'},'D-32')));
    reject('missing-mix-listening',/EVIDENCE_SET/,() =>
      verify({...listening,verdicts:listening.verdicts.filter(row=>row.id!=='D-32')}));
    reject('other-decision-checkpoints',/EVIDENCE_AUDIO_DECISION_BINDING/,() =>
      verify(change(listening,{checkpoints:subjects.decisions.get('D-31')},'D-32')));
    reject('unrelated-mix-screenshot',/EVIDENCE_AUDIO_DECISION_BINDING/,() => {
      const checkpoints=structuredClone(subjects.decisions.get('D-32'));
      checkpoints[0].image=file;
      verify(change(listening,{checkpoints},'D-32'));
    });
    reject('mix-without-independent-heard-finding',/EVIDENCE_AUDIO_HEARD_FINDINGS/,() =>
      verify(change(listening,{findings:{dayFire:'Only one condition'}},'D-32')));
    reject('listening-outside-live-play',/EVIDENCE_AUDIO_DECISION_BINDING/,() =>
      verify(change(listening,{listenedFrom:final.finishedAt,listenedThrough:time(final.finishedAt,100)},'D-32')));
    reject('stale-owned-audio-draw',/EVIDENCE_AUDIO_DECISION_CHECKPOINT/,() => {
      const checkpoints=structuredClone(audio.checkpoints);
      const draw=e.document(root,checkpoints[0].draw);
      checkpoints[0].draw=e.writeExclusive(root,'old-audio-draw.json',{...draw,invocation:crypto.randomUUID()});
      verify(listening,judgments,{...audio,checkpoints});
    });
    reject('wrong-audio-draw-step-id',/EVIDENCE_AUDIO_DECISION_CHECKPOINT/,() => {
      const checkpoints=structuredClone(audio.checkpoints);
      checkpoints[0].draw=checkpoints[1].draw;
      verify(listening,judgments,{...audio,checkpoints});
    });
    reject('missing-actual-image',/EVIDENCE_JUDGMENT_IMAGE/,() => verify(listening,change(judgments,{files:[file]})));
    reject('prohibition-failure',/EVIDENCE_JUDGMENT_FAILED/,() => verify(listening,change(judgments,{verdict:'fail'})));
    reject('old-judgment-source',/EVIDENCE_IDENTITY/,() => verify(listening,change(judgments,{source:'f'.repeat(64)})));
    reject('wrong-judgment-statement',/EVIDENCE_JUDGMENT_RUN/,() => verify(listening,change(judgments,{statement:'Different prohibition'})));
    assert.equal(subjects.samples.size,93);
    assert.equal(new Set([...subjects.samples.values()].filter(row=>row.sample.pack==='packs/audio-interface-ambience.json')
      .map(row=>row.sample.variant)).size,27);
    assert.equal(subjects.decisions.size,2);
    return {audioExpectations:expected.length,sampleAndLoopRows:subjects.samples.size,mixDecisions:subjects.decisions.size,actualNamedVerdicts:0};
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

function audioJudgmentFixture(root,report,picture,reject) {
  const e = require('../evidence.cjs');
  const {AUDIO_CASE,AUDIO_PLAYBACK,template,validateDeclaration} = require('../rendered-contracts.cjs');
  const base = 'src/godot-game/';
  for (const file of ['content/audio/events.json','content/fixtures/audio-states.json','content/packs/audio-actions.json',
    'content/packs/audio-interface-ambience.json','assets/audio/actions.wav','assets/audio/interface-ambience.wav']) {
    fs.mkdirSync(path.dirname(path.join(root,base+file)),{recursive:true});
    fs.copyFileSync(path.join(ROOT,base+file),path.join(root,base+file),fs.constants.COPYFILE_EXCL);
  }
  const authority = e.audioAuthority(root);
  const id = AUDIO_CASE, invocation = report.invocation;
  const steps = template(id).map(step=>({...step,action:'click',x:100,y:200,
    ...(step.openMenu?{openMenu:{x:110,y:210}}:{})}));
  const driver = {id,stimulus:'Fixture audio controls',steps};
  validateDeclaration(driver,{id});
  reject('missing-physical-audio-menu-open',/RENDERED_CASE_MENU/,()=>validateDeclaration({...driver,
    steps:steps.map(step=>step.id==='select-swing'?{...step,openMenu:null}:step)},{id}));
  const {masterPercent,effectsPercent,ambiencePercent}=authority.events.defaults;
  function clip(routeId,loop) {
    const route=authority.routes.get(routeId),sample=authority.sampleById.get(route.variants[0]);
    return {route:routeId,variant:sample.id,version:sample.version,wavSha256:sample.sha256,pcmSha256:sample.pcmSha256,
      bus:route.bus,gainLinear:route.gainLinear,loopFrames:loop?[0,sample.frames]:null,stateLoop:loop,playing:true,
      volumeDb:20*Math.log10(masterPercent*(loop?ambiencePercent:effectsPercent)*route.gainLinear/10000)};
  }
  let previous = null;
  const checkpoints = [...AUDIO_PLAYBACK].map(([step,{route,state}])=>{
    if (step==='play-impact'||step==='play-confirmation') previous=null;
    const play = {epoch:1,serial:(previous?.serial??0)+1,oneShots:(previous?.oneShots??0)+1,
      catalogVersion:authority.events.version,masterPercent,effectsPercent,ambiencePercent,muted:false,
      cue:clip(route,false),overlap:authority.states.get(state).slice().sort().map(route=>clip(route,true))};
    const native = (value) => ({id:62,value:value?JSON.stringify(value):null});
    const playback = e.audioPlayback(authority,step,native(previous),native(play));
    if (step==='play-day-swing') {
      const before=native(previous);
      const mutate=(name,pattern,edit)=>reject(name,pattern,()=>{
        const changed=structuredClone(play);
        edit(changed);
        e.audioPlayback(authority,step,before,native(changed));
      });
      mutate('wrong-selected-variant',/EVIDENCE_AUDIO_CLIP/,value=>{value.cue.variant='action.4872.full';});
      mutate('wrong-cue-route',/EVIDENCE_AUDIO_CLIP/,value=>{value.cue.route='melee.swing.club-bat';});
      mutate('wrong-playback-sample-version',/EVIDENCE_AUDIO_CLIP/,value=>{value.cue.version++;});
      mutate('wrong-cue-wav',/EVIDENCE_AUDIO_CLIP/,value=>{value.cue.wavSha256=value.overlap[0].wavSha256;});
      mutate('no-overlapping-action',/EVIDENCE_AUDIO_CLIP/,value=>{value.cue=null;});
      mutate('missing-active-ambience',/EVIDENCE_SET/,value=>{value.overlap.pop();});
      mutate('wrong-overlapping-ambience',/EVIDENCE_SET/,value=>{value.overlap[0]=clip('ambience.day-night.forest-night',true);});
      mutate('missing-live-loop-configuration',/EVIDENCE_AUDIO_LOOP/,value=>{value.overlap[0].loopFrames=null;});
      mutate('wrong-live-loop-configuration',/EVIDENCE_AUDIO_LOOP/,value=>{value.overlap[0].loopFrames[1]--;});
      mutate('wrong-effective-gain',/EVIDENCE_AUDIO_GAIN/,value=>{value.cue.volumeDb=-80;});
      mutate('muted-audio-playback',/EVIDENCE_AUDIO_MIX_SETTINGS/,value=>{value.muted=true;});
      reject('stale-native-audition-record',/EVIDENCE_AUDIO_PLAY_TRANSITION/,()=>e.audioPlayback(authority,step,before,before));
    }
    previous = play;
    const flags = {caseId:id,checkpoint:step,invocation,token:crypto.randomUUID()};
    const draw = e.writeExclusive(root,step+'-draw.json',{...flags,at:'2026-11-20T11:00:03.000Z',drawn:true,imageSha256:picture.sha256});
    const input = e.writeExclusive(root,step+'-input.json',{...flags,actionStartedAt:'2026-11-20T11:00:02.000Z'});
    return {id:step,playback,draw,input,image:picture};
  });
  return {id,invocation,status:'pass',checkpoints,listeningMedia:e.writeAuditionMedia(root,invocation)};
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
    repairHistoryControls(reject);
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
  sourceHistoryControls,renderedDeclarationControls,selectionControls,pcmFixture,judgmentControls,artifactControls,
  processIdentityControls,inheritedReceiptControls,campaignHistoryControls,interruptionControls};
