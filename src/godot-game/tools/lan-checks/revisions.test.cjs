'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {spawnSync}=require('node:child_process'),e=require('./evidence.cjs'),r=require('./revisions.cjs');
const catalog=require('./catalog.cjs'),baseline=require('../verification/check.cjs'),check=require('./check.cjs');
const project=path.resolve(__dirname,'../../../..'),workspace=require('../workspace-paths.cjs');
// Control repositories live in this checkout's temp folder; a temporary shared root keeps them off the real one.
// Each control is removed after its test; the shared root, an emptied controls folder and the variable after the file.
const controls=workspace.locations(project).temp('lan-checks'),remove=directory=>fs.rmSync(directory,{recursive:true,force:true,maxRetries:3});
function control(prefix){for(;;){fs.mkdirSync(controls,{recursive:true});try{return fs.mkdtempSync(path.join(controls,prefix));}catch(error){if(error.code!=='ENOENT')throw error;}}}
const shared=control('shared-'),priorShared=process.env.KZ_SHARED_LOCAL;process.env.KZ_SHARED_LOCAL=shared;
test.after(()=>{
  remove(shared);if(priorShared===undefined)delete process.env.KZ_SHARED_LOCAL;else process.env.KZ_SHARED_LOCAL=priorShared;
  try{fs.rmdirSync(controls);}catch(error){if(!['ENOENT','ENOTEMPTY','EEXIST','EBUSY','EPERM'].includes(error.code))throw error;}
});
// Families map to their runners' run folders inside a control repository.
const runs={phase1:'.local/runs/verification',phase2:'.local/runs/gameplay-checks',phase3:'.local/runs/lan-checks'};
const BUILD='.local/temp/lan-checks/build';
function fixture(t,build=false){
  const root=control('revision-control-');t.after(()=>remove(root));
  const write=(file,value)=>{fs.mkdirSync(path.dirname(path.join(root,file)),{recursive:true});fs.writeFileSync(path.join(root,file),typeof value==='string'?value:JSON.stringify(value));};
  const git=(...args)=>{const result=spawnSync('git',args,{cwd:root,encoding:'utf8',windowsHide:true});assert.equal(result.status,0,result.stderr);return result.stdout.trim();};
  const commit=()=>{git('add','.');git('commit','-qm','control');return git('rev-parse','HEAD');};
  git('init','-q');git('config','user.email','control@example.invalid');git('config','user.name','Control');git('config','core.autocrlf','false');
  write('.gitignore','.local/\n**/bin/\n**/obj/\n');
  for(const input of check.inputManifest().inputs){
    const isFile=path.posix.basename(input).includes('.')||input.endsWith('.editorconfig');
    write(isFile?input:input+'/control.txt','CONTROL_ONLY');
  }
  for(const file of ['src/godot-game/tools/verification/required-cases.json','src/godot-game/tools/gameplay-checks/required-cases.json','src/godot-game/tools/lan-checks/required-cases.json',catalog.selectionPath,
    '.planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md'])write(file,fs.readFileSync(path.join(project,file),'utf8'));
  for(const input of e.read(path.join(root,'src/godot-game/tools/verification/required-cases.json')).inputs){
    if(!fs.existsSync(path.join(root,input)))write(path.posix.basename(input).includes('.')?input:input+'/control.txt','CONTROL_ONLY');
  }
  write('src/godot-game/tools/lan-checks/control.txt','CONTROL_ONLY');
  write('src/godot-game/tools/verification/toolchain.json',fs.readFileSync(path.join(project,'src/godot-game/tools/verification/toolchain.json'),'utf8'));
  if(build){
    git('remote','add','origin','https://github.com/Sebreiro/kingdom-zombies-game');
    write('src/godot-game/tools/lan-checks/reviewed-source.targets',fs.readFileSync(path.join(project,'src/godot-game/tools/lan-checks/reviewed-source.targets'),'utf8'));
    write('src/godot-game/global.json',fs.readFileSync(path.join(project,'src/godot-game/global.json'),'utf8'));
    write('src/godot-game/NuGet.config','<configuration><packageSources><clear /></packageSources></configuration>');
    write('src/Subject.cs','internal sealed class Subject { internal int Value => 1; }\n');
    write(BUILD+'/Directory.Build.props','<Project />');write(BUILD+'/Directory.Build.targets','<Project />');
    write(BUILD+'/Directory.Packages.props','<Project />');
    write(BUILD+'/NuGet.Config','<configuration><packageSources><clear /></packageSources></configuration>');
    write(BUILD+'/Subject.csproj','<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><VersionPrefix>0.1.0</VersionPrefix></PropertyGroup><ItemGroup><Compile Include="../../../../src/Subject.cs" /></ItemGroup></Project>');
  }
  // These records exercise validators, never claim game/test execution credit.
  const cases=e.read(path.join(root,'src/godot-game/tools/lan-checks/required-cases.json')).cases.map(row=>({...row,id:row.id==='rendered.timing'?'runner.control-timing':row.id,needsGameWindow:false,roles:undefined,additionalRoles:undefined,group:'runner',implementation:'control'}));
  const savedSelection=e.read(path.join(root,catalog.selectionPath));savedSelection.selected.phase3=cases.map(row=>row.id);write(catalog.selectionPath,savedSelection);
  fs.appendFileSync(path.join(root,'.planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md'),'\n`runner.control-timing` control only\n');
  const phase2=e.read(path.join(root,'src/godot-game/tools/gameplay-checks/required-cases.json'));phase2.cases=phase2.cases.map(row=>({...row,implementation:'control',images:undefined}));write('src/godot-game/tools/gameplay-checks/required-cases.json',phase2);
  write('src/godot-game/tools/lan-checks/required-cases.json',{schemaVersion:1,requiredEnvironment:'development-os',cases});
  const reviewed=commit(),input=baseline.snapshot(root,check.inputManifest()).sha256;
  if(build)compile(root,reviewed);
  const identity=build?pinnedIdentity(root,reviewed):{input,tool:'control-tool',platform:process.platform+'-'+process.arch,osFingerprint:'control-os',buildRevision:reviewed};
  write(catalog.reviewPath.replace('.json','.md'),'Immutable original review.');
  const ref=file=>({path:file,sha256:e.hash(fs.readFileSync(path.join(root,file)))});
  const review={status:'complete',...identity,revision:reviewed,reviewedBy:'Control',findings:[],report:ref(catalog.reviewPath.replace('.json','.md'))};
  write(catalog.reviewPath,review);const tested=commit();
  if(build){compile(root,reviewed);assert.equal(pinnedIdentity(root,reviewed).tool,identity.tool,'metadata commit and strict rebuild must preserve the exact tool bytes');}
  const context={...identity,revision:tested,root,directory:path.join(root,runs.phase3,'control/final')};fs.mkdirSync(context.directory,{recursive:true});
  const gate=catalog.prelaunch(root,context),selection=gate.savedSelection;
  const record=(family,summary)=>{const prefix=runs[family]+'/control';write(prefix+'/summary.json',summary);write(prefix+'/process.json',{code:0,signal:null,timedOut:false,startedAt:10,finishedAt:20});return {...ref(prefix+'/summary.json'),process:ref(prefix+'/process.json')};};
  const inherited={phase1:[],phase2:[],status:'pass',nativeAllowed:true};
  const p1=e.read(path.join(root,'src/godot-game/tools/verification/required-cases.json'));
  const p1Results=selection.selected.phase1.map(id=>{
    const item=p1.cases.find(row=>row.id===id),prefix=runs.phase1+'/control/'+id;
    write(prefix+'/'+id+'.subject.log','CONTROL');write(prefix+'/tests.json',{results:{tool:{version:'4.0.2'},summary:{tests:1,passed:1,failed:0},tests:[{name:item.test,status:'passed',labels:{Case:id}}]}});
    return {id,status:'pass',evidence:[{path:id+'.subject.log',sha256:ref(prefix+'/'+id+'.subject.log').sha256}],report:{path:id+'/tests.json',sha256:ref(prefix+'/tests.json').sha256}};
  });
  inherited.phase1.push(record('phase1',{...identity,revision:tested,nonce:'p1',status:'pass',input:baseline.snapshot(root,p1).sha256,selected:selection.selected.phase1,results:p1Results}));
  const p2Manifest={inputs:['src','.editorconfig','docs/code']};
  const p2Identity={...identity,input:baseline.snapshot(root,p2Manifest).sha256,revision:tested,nonce:'p2'};
  inherited.phase2.push(record('phase2',{schemaVersion:2,...p2Identity,status:'pass',startedAt:10,finishedAt:20,failures:[],selected:selection.selected.phase2,results:selection.selected.phase2.map(id=>({...p2Identity,id,status:'pass'}))}));
  inherited.commands=['phase1','phase2'].map(family=>{
    const reference=inherited[family][0],prefix=runs[family]+'/control/command';write(prefix+'.json','{}');write(prefix+'.log','CONTROL');
    return {family,executed:true,status:'pass',caseOutcomes:selection.selected[family].map(id=>({id,status:'pass'})),
      report:reference,process:reference.process,record:ref(prefix+'.json'),logs:{stdout:ref(prefix+'.log'),stderr:ref(prefix+'.log')}};
  });
  const finalRun=runs.phase3+'/control/final';write(finalRun+'/a.log','CONTROL');write(finalRun+'/b.log','CONTROL');
  const artifacts=['a.log','b.log'].map(file=>({path:file,sha256:ref(finalRun+'/'+file).sha256}));
  const final={schemaVersion:2,...identity,revision:tested,nonce:'final',status:'pass',startedAt:1,finishedAt:30,selected:selection.selected.phase3,
    results:cases.map(row=>({...identity,revision:tested,nonce:'final',id:row.id,status:'pass',artifacts})),prelaunchReview:gate.review,selectionSha256:gate.selection.sha256,inherited};
  write(finalRun+'/summary.json',final);
  write('.planning/phases/03-four-player-lan-foundation/03-VISUAL-REVIEW.md','CONTROL: no rendered credit');
  const index={schemaVersion:1,scope:'Windows 1280x720 local; no VM',selection:gate.selection,finalReport:ref(finalRun+'/summary.json'),history:[],reviews:[],visualReview:ref('.planning/phases/03-four-player-lan-foundation/03-VISUAL-REVIEW.md')};
  write('.planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json',index);
  write(catalog.reviewPath.replace('.json','.md'),'Later canonical review metadata; immutable original still bound.');
  for(const file of ['03-VERIFICATION.md','03-VALIDATION.md','03-SECURITY.md','03-UI-REVIEW.md','03-REVIEW-DISPOSITION.md'])
    write('.planning/phases/03-four-player-lan-foundation/'+file,'CONTROL: closure metadata; no additional execution credit');
  const closure=commit();return {root,write,git,commit,reviewed,tested,closure,context,review,gate,identity,index,cases,ref};
}
function compile(root,stamp,pinSourceRoot=true){
  // The SDK's temporary files and CLI home stay inside the control, so removing the control removes them.
  const local=workspace.locations(root),temp=local.temp();fs.mkdirSync(temp,{recursive:true});
  const env={...check.environment(project),DOTNET_CLI_HOME:local.cliHome,TEMP:temp,TMP:temp,TMPDIR:temp,SourceRevisionId:stamp??'',CustomAfterMicrosoftCommonTargets:pinSourceRoot?path.join(root,'src/godot-game/tools/lan-checks/reviewed-source.targets'):''};
  const args=['build',BUILD+'/Subject.csproj','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'];
  const configured=require('../dotnet.cjs').launch(root,'dotnet',args);
  const startedAt=Date.now(),result=spawnSync('dotnet',configured.args,
    {cwd:configured.cwd,env,encoding:'utf8',windowsHide:true,timeout:20000});
  const records=path.join(root,'.local/temp/lan-checks/build-records');fs.mkdirSync(records,{recursive:true});
  fs.writeFileSync(path.join(records,'build-'+fs.readdirSync(records).length+'.json'),JSON.stringify({executable:'dotnet',args,
    sourceRevisionId:stamp,pinSourceRoot,startedAt,finishedAt:Date.now(),code:result.status,signal:result.signal,stdout:result.stdout,stderr:result.stderr,error:result.error?.message}),{flag:'wx'});
  assert.equal(result.status,0,result.stdout+result.stderr);
  const info=fs.readFileSync(path.join(root,BUILD,'obj/Debug/net10.0/Subject.AssemblyInfo.cs'),'utf8');
  const actual=stamp??spawnSync('git',['rev-parse','HEAD'],{cwd:root,encoding:'utf8',windowsHide:true}).stdout.trim();
  assert.ok(info.includes('0.1.0+'+actual),'actual SDK build must record the requested immutable source stamp');
  const links=e.read(path.join(root,BUILD,'obj/Debug/net10.0/Subject.sourcelink.json'));
  assert.ok(Object.values(links.documents).length>0,'real GitHub remote must produce source links');
  const linked=pinSourceRoot?actual:spawnSync('git',['rev-parse','HEAD'],{cwd:root,encoding:'utf8',windowsHide:true}).stdout.trim();
  assert.ok(Object.values(links.documents).every(url=>url.includes('/'+linked+'/')),'SourceLink must retain the reviewed source stamp independently of metadata HEAD');
  const file=path.join(root,'src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll');fs.mkdirSync(path.dirname(file),{recursive:true});
  fs.copyFileSync(path.join(root,BUILD,'bin/Debug/net10.0/Subject.dll'),file);return e.hash(fs.readFileSync(file));
}
function pinnedIdentity(root,stamp){
  const prior=process.env.SourceRevisionId;
  try{if(stamp)process.env.SourceRevisionId=stamp;else delete process.env.SourceRevisionId;return check.createIdentity(root);}
  finally{if(prior===undefined)delete process.env.SourceRevisionId;else process.env.SourceRevisionId=prior;}
}
test('committed metadata closure validates original review and completion without rewriting bytes',t=>{
  const f=fixture(t),original=fs.readFileSync(path.join(f.root,f.index.finalReport.path)),review=fs.readFileSync(path.join(f.root,f.gate.review.report.path));
  const current={...f.identity,revision:f.closure};
  assert.equal(e.verifyCompletion(f.root,f.index,f.cases,current).testedRevision,f.tested);
  assert.equal(r.relation(f.root,f.reviewed,f.closure,f.identity.input).execution,'original');
  assert.deepEqual(fs.readFileSync(path.join(f.root,f.index.finalReport.path)),original);
  assert.deepEqual(fs.readFileSync(path.join(f.root,f.gate.review.report.path)),review);
  const inherited=e.read(path.join(f.root,f.index.finalReport.path)).inherited.phase1[0];
  const inheritedFile=path.join(f.root,inherited.path),subject=e.read(inheritedFile).results[0];
  const directory=path.dirname(inheritedFile),item=e.read(path.join(f.root,'src/godot-game/tools/verification/required-cases.json')).cases.find(row=>row.id===subject.id);
  const verify=value=>e.verifyPhase1Proof(directory,value,item),record=subject.evidence[0];
  verify(subject);
  assert.throws(()=>verify({...subject,evidence:[{...record,sha256:'0'.repeat(64)}]}),/ARTIFACT_HASH/);
  for(const name of ['../foreign.log','C:foreign.log','/foreign.log'])assert.throws(()=>verify({...subject,evidence:[{...record,path:name}]}));
  const other=e.read(inheritedFile).results[1];
  assert.throws(()=>verify({...subject,report:other.report}),/INHERITED_CASE_REPORT/);
  assert.throws(()=>verify({...subject,evidence:[{...other.evidence[0],path:'../'+other.id+'/'+other.evidence[0].path}]}));
  const subjectFile=path.join(directory,subject.id,record.path),subjectBytes=fs.readFileSync(subjectFile);
  fs.unlinkSync(subjectFile);assert.throws(()=>verify(subject),/ENOENT/);fs.writeFileSync(subjectFile,subjectBytes);
  for(const key of ['input','tool','platform','osFingerprint','buildRevision'])assert.throws(()=>e.verifyCompletion(f.root,f.index,f.cases,{...current,[key]:'changed'}));
  for(const revision of ['unknown','0'.repeat(40),f.closure.slice(0,8)])assert.throws(()=>e.verifyCompletion(f.root,f.index,f.cases,{...current,revision}));
  assert.throws(()=>r.relation(f.root,f.closure,f.tested,f.identity.input));
  const unrelated=f.git('commit-tree',f.git('rev-parse','HEAD^{tree}'),'-m','unrelated');
  assert.throws(()=>r.relation(f.root,unrelated,f.closure,f.identity.input));
  for(const file of ['src/control.txt','src/godot-game/tools/lan-checks/control.txt',catalog.selectionPath]){
    const bytes=fs.readFileSync(path.join(f.root,file));f.write(file,'changed');
    assert.throws(()=>e.verifyCompletion(f.root,f.index,f.cases,current));
    fs.writeFileSync(path.join(f.root,file),bytes);
  }
  for(const file of ['src/control.txt','src/godot-game/tools/lan-checks/control.txt',catalog.selectionPath]){
    f.git('checkout','--detach',f.closure);fs.appendFileSync(path.join(f.root,file),'\n');const changed=f.commit();
    assert.throws(()=>r.relation(f.root,f.tested,changed,f.identity.input),/REVISION_NON_METADATA/);
    assert.throws(()=>catalog.prelaunch(f.root,{...f.context,revision:changed}));
  }
  f.git('checkout','--detach',f.closure);
  f.write('unapproved.txt','changed');const disallowed=f.commit();
  assert.throws(()=>r.relation(f.root,f.tested,disallowed,f.identity.input),/REVISION_NON_METADATA/);
  f.git('revert','--no-edit',disallowed);assert.throws(()=>r.relation(f.root,f.tested,f.git('rev-parse','HEAD'),f.identity.input),/REVISION_NON_METADATA/);
  const originalPath=process.env.PATH;try{process.env.PATH='';assert.throws(()=>r.relation(f.root,f.tested,f.closure,f.identity.input),/GIT_VALIDATION_UNAVAILABLE/);}finally{process.env.PATH=originalPath;}
});
test('prelaunch requires committed matching review inputs and exact original report hash',t=>{
  const f=fixture(t);f.git('checkout','--detach',f.tested);
  const context={...f.context,directory:path.join(f.root,'.local/temp/lan-checks/other')};fs.mkdirSync(path.dirname(context.directory),{recursive:true});fs.mkdirSync(context.directory);
  assert.throws(()=>catalog.prelaunch(f.root,{...context,buildRevision:null}),/REVIEW_BUILD_REVISION_REQUIRED/);
  assert.throws(()=>catalog.prelaunch(f.root,{...context,buildRevision:f.tested}),/REVIEW_BUILD_REVISION_REQUIRED/);
  catalog.prelaunch(f.root,context);
  const file=catalog.reviewPath;f.write(file,{...f.review,revision:'0'.repeat(40)});assert.throws(()=>catalog.prelaunch(f.root,context));
  f.git('checkout','--',file);f.write(f.review.report.path,'tampered');assert.throws(()=>catalog.prelaunch(f.root,context));
  f.git('checkout','--',f.review.report.path);f.write('src/control.txt','dirty');assert.throws(()=>catalog.prelaunch(f.root,context));
});
test('real SDK commit-build-review-gate cycle preserves exact DLL bytes only with the reviewed source stamp',t=>{
  const f=fixture(t,true),file=path.join(f.root,f.index.finalReport.path),original=fs.readFileSync(file);
  const before=e.hash(fs.readFileSync(path.join(f.root,'src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll')));
  console.log('SDK_FINALIZATION_CONTROL '+f.root+' source='+f.reviewed+' execution='+f.tested+' closure='+f.closure+' dll='+before);
  assert.equal(compile(f.root,f.reviewed),before);
  assert.equal(e.verifyCompletion(f.root,f.index,f.cases,pinnedIdentity(f.root,f.reviewed)).testedRevision,f.tested);
  const unpinned=compile(f.root,f.reviewed,false);assert.notEqual(unpinned,before,'negative control must reproduce SourceLink HEAD drift even while the assembly stamp stays pinned');
  assert.throws(()=>e.verifyCompletion(f.root,f.index,f.cases,pinnedIdentity(f.root,f.reviewed)),/COMPLETION_CURRENT tool/);
  assert.equal(compile(f.root,f.reviewed),before,'pinned closure rebuild must restore the exact original DLL, without hash equivalence');
  e.verifyCompletion(f.root,f.index,f.cases,pinnedIdentity(f.root,f.reviewed));assert.deepEqual(fs.readFileSync(file),original);
});
