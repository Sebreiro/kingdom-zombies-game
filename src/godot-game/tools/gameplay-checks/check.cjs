'use strict';
const {launch:dotnetLaunch}=require('../dotnet.cjs');
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const { spawnSync } = require('node:child_process');
const baseline = require('../verification/check.cjs');
const { rendered, command } = require('./rendered.cjs');
const evidence = require('./evidence.cjs');
const workspace = require('../workspace-paths.cjs');
const {hash,requireTrue,inside,read,reconcile,loadReport,aggregate,verifyCompletion} = evidence;
function select(catalog, args) {
  requireTrue(args.length === 1 && ['--all','--list'].includes(args[0]) || args.length === 2 && ['--case','--group','--failed'].includes(args[0]), 'USAGE: --case ID | --group GROUP | --all | --failed REPORT | --list');
  if(args[0]==='--failed') return {repair:args[1]};
  const ids = args[0] === '--case' ? args[1].split(',') : [];
  requireTrue(new Set(ids).size === ids.length, 'SELECTION_DUPLICATE');
  const selected = catalog.cases.filter(row => ['--all','--list'].includes(args[0])
    || (args[0] === '--case' ? ids.includes(row.id) : row.group === args[1]));
  requireTrue(args[0] !== '--case' || selected.length === ids.length, 'SELECTION_UNKNOWN');
  requireTrue(selected.length > 0, 'SELECTION_EMPTY');
  return {list:args[0]==='--list',selected};
}
function acquireLock(root,nonce) {
  // One machine has one desktop, so every checkout of the repository shares this lock.
  const file=workspace.locations(root).locks.desktop;
  workspace.assertSharedIgnored(root);
  fs.mkdirSync(path.dirname(file),{recursive:true});
  let fd;
  try { fd=fs.openSync(file,'wx'); } catch(error) {if(error.code==='EEXIST')throw new Error('DESKTOP_LOCKED: another run or interrupted-run lock; inspect recorded owner before removing');throw error;}
  fs.writeFileSync(fd,JSON.stringify({pid:process.pid,nonce,at:new Date().toISOString()}));
  const ownership=fs.readFileSync(file),identity=fs.fstatSync(fd);let released=false;
  return ()=>{
    if(released)return;released=true;fs.closeSync(fd);
    requireTrue(fs.existsSync(file)&&fs.readFileSync(file).equals(ownership)&&fs.statSync(file).ino===identity.ino,'DESKTOP_LOCK_OWNERSHIP_CHANGED');
    fs.unlinkSync(file);
  };
}
async function csharp(context,item) {
  if (item.callerStages) {
    requireTrue(item.id==='core.permissions','STAGED_CASE_UNKNOWN');
    baseline.callerPlan(item,context.root);
    const directory=inside(context.directory,item.id);fs.mkdirSync(directory);
    const child={...context,directory,children:new Set(),commands:[]},assembly=inside(context.root,'src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll');
    const beforeBinary=hash(fs.readFileSync(assembly));
    try {
      const build=await baseline.command('dotnet',['build','src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj','--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],child,'caller-setup-build',60000);
      requireTrue(build.code===0,'STAGED_BUILD_FAILED');
      requireTrue(hash(fs.readFileSync(assembly))===beforeBinary,'STAGED_BINARY_CHANGED');
      return await baseline.runProducerStages(item,child,'dotnet',assembly);
    } finally {for(const process of child.children)baseline.killTree(process);}
  }
  const env={...context.env,PHASE1_ROOT:context.root,PHASE1_RESULT_DIRECTORY:context.directory,PHASE1_NONCE:context.nonce,PHASE1_INPUT:context.input,PHASE1_TOOL:context.tool,PHASE1_PLATFORM:context.platform};
  let run;
  try {
    run=await command('dotnet',[path.join(context.root,'src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),'--filter-method',item.test,'--report-xunit-ctrf','--report-xunit-ctrf-filename',item.id+'.ctrf.json','--results-directory',context.directory],{cwd:dotnetLaunch(context.root,'dotnet',[]).cwd,env},180000);
  } catch(error) {fs.writeFileSync(path.join(context.directory,item.id+'.console.log'),error.stack);throw error;}
  fs.writeFileSync(path.join(context.directory,item.id+'.console.log'),run.stdout+run.stderr);
  const report=read(path.join(context.directory,item.id+'.ctrf.json'));
  requireTrue(report.results.summary.tests===1 && report.results.summary.failed===0 && report.results.summary.passed===1,'TEST_EXECUTION_COUNT');
  const result=read(path.join(context.directory,item.id+'.proof.json'));
  requireTrue(Array.isArray(result.evidence) && result.evidence.length>0,'SUBJECT_LOG_REQUIRED');
  result.testReport=item.id+'.ctrf.json';
  result.artifacts=[...result.evidence,result.testReport].map(file=>({path:file,sha256:hash(fs.readFileSync(inside(context.directory,file)))}));
  return result;
}
async function main(root,args) {
  const catalog=evidence.catalog(root);
  if(args[0]==='--verify-completion' || args[0]==='--verify-platforms') {
    requireTrue(args.length===2,'USAGE_COMPLETION');
    const completion=verifyCompletion(root,read(inside(root,args[1])),catalog.cases);
    console.log('PASS temporary development-OS completion; Phase5 platform/resolution acceptance deferred');return completion;
  }
  if(args[0]==='--complete') {
    requireTrue(args.length>1,'COMPLETION_EMPTY');
    const references=args.slice(1).map(relative=>({path:relative,sha256:hash(fs.readFileSync(inside(root,relative)))}));
    const completion={schemaVersion:1,...aggregate(root,references,catalog.cases),reviews:[]};
    const file=workspace.locations(root).runs('gameplay-checks','completions',crypto.randomUUID()+'.json');
    const relative=path.relative(root,file).replaceAll('\\','/');
    fs.mkdirSync(path.dirname(file),{recursive:true});fs.writeFileSync(file,JSON.stringify(completion,null,2),{flag:'wx'});
    console.log(completion.status.toUpperCase()+' '+relative+'; appearance review required separately');
    if(completion.missing.length)process.exitCode=1;return completion;
  }
  const selection=select(catalog,args);
  if(selection.list){console.log(JSON.stringify(selection.selected,null,2));return;}
  let repairOf=null;
  if(selection.repair) {
    const original=loadReport(root,selection.repair,catalog.cases);
    selection.selected=catalog.cases.filter(row=>original.summary.selected.includes(row.id) && !original.summary.results.some(result=>result.id===row.id && result.status==='pass'));
    requireTrue(selection.selected.length>0,'NO_FAILED_CASES');
    repairOf={path:original.path,sha256:original.sha256,revision:original.summary.revision};
  }
  const nonce=crypto.randomUUID(),platform=process.platform+'-'+process.arch;
  const directory=workspace.locations(root).runs('gameplay-checks',platform,nonce);
  fs.mkdirSync(directory,{recursive:true});
  const started=Date.now(),results=[],failures=[];
  const manifest=inputManifest(selection.selected);
  const env=environment(root);
  const context={root,directory,nonce,platform,input:null,tool:null,env};
  let unlock;
  try {
    const snapshot=baseline.snapshot(root,manifest);context.input=snapshot.sha256;
    fs.writeFileSync(path.join(directory,'inputs.json'),JSON.stringify(snapshot,null,2));
    // Pure checks need no native executable, display, VM or renderer probe.
    context.tool=toolIdentity(root,env);
    for(const item of selection.selected.filter(row=>row.implementation==='csharp')) {
      console.log('Checking '+item.id);
      try {const result=await csharp(context,item);reconcile([item],[result],context);results.push(result);}
      catch(error){results.push({id:item.id,status:'fail',nonce,input:context.input,tool:context.tool,platform,message:error.message});}
    }
    const native=selection.selected.filter(row=>row.implementation==='native');
    if(native.length) {
      unlock=acquireLock(root,nonce);
      const pins=read(path.join(root,'src/godot-game/tools/verification/toolchain.json')),archive=pins.archives[platform];
      requireTrue(archive,'PLATFORM_UNSUPPORTED');
      const engine=workspace.locations(root).tools('godot',archive.directory,archive.executable);
      requireTrue(fs.existsSync(engine),'ENGINE_MISSING');requireTrue(workspace.insideShared(engine,root),'ENGINE_OUTSIDE_SHARED');
      const executableHash=hash(fs.readFileSync(engine));
      if(archive.executableSha256)requireTrue(executableHash===archive.executableSha256,'ENGINE_HASH');
      context.engine=process.platform==='win32'?engine.replace('_console.exe','.exe'):engine;
      if(process.platform==='win32')requireTrue(hash(fs.readFileSync(context.engine))==='45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab','NATIVE_ENGINE_HASH');
      requireTrue((await command(engine,['--version'],{cwd:root,env})).stdout.trim()===pins.engineFullVersion,'ENGINE_VERSION');
      const nativeDirectory=path.join(directory,'session');fs.mkdirSync(nativeDirectory);
      const nativeResults=await rendered({...context,directory:nativeDirectory},native);
      for(const result of nativeResults) {
        if(result.images)for(const image of result.images){image.path='session/'+image.path;image.statePath='session/'+image.statePath;}
        if(result.status==='pass')try{reconcile([catalog.cases.find(row=>row.id===result.id)],[result],context);}catch(error){result.status='fail';result.message=error.message;}
        results.push(result);
      }
    }
    requireTrue(baseline.snapshot(root,manifest).sha256===context.input,'SOURCE_CHANGED');
  } catch(error){failures.push({message:error.message,stack:error.stack});}
  finally {if(unlock)unlock();}
  if(failures.length)for(const result of results)if(result.status==='pass'){result.status='fail';result.message='RUN_INVALID '+failures.at(-1).message;}
  for(const item of selection.selected)if(!results.some(row=>row.id===item.id))results.push({id:item.id,status:'blocked',nonce,input:context.input,tool:context.tool,platform,message:failures.at(-1)?.message??'NOT_EXECUTED'});
  const status=failures.length || results.some(row=>row.status!=='pass')?'fail':'pass';
  const summary={schemaVersion:2,nonce,platform,input:context.input,tool:context.tool,status,selected:selection.selected.map(row=>row.id),executed:results.filter(row=>row.status!=='blocked').length,results,failures,repairOf,
    policy:'creator-authorized-temporary-pre-phase5',resolution:[1280,720],dependencyAwareReuseCertified:false,startedAt:started,finishedAt:Date.now(),durationMilliseconds:Date.now()-started,revision:spawnSync('git',['rev-parse','HEAD'],{cwd:root,encoding:'utf8',windowsHide:true}).stdout.trim()};
  for(const result of results)fs.writeFileSync(path.join(directory,result.id+'.result.json'),JSON.stringify(result,null,2),{flag:'wx'});
  fs.writeFileSync(path.join(directory,'summary.json'),JSON.stringify(summary,null,2),{flag:'wx'});
  console.log(`${status.toUpperCase()} ${results.filter(row=>row.status==='pass').length}/${selection.selected.length} selected obligations; ${path.relative(root,path.join(directory,'summary.json'))}`);
  for(const result of results.filter(row=>row.status!=='pass'))console.error(result.status.toUpperCase()+' '+result.id+': '+result.message);
  for(const failure of failures)console.error(failure.message);
  if(status!=='pass')process.exitCode=1;return summary;
}
// The input scope and tool identity are recorded in every summary; later consumers recompute them from these definitions.
// Caller-stage inputs (the verification runner, its catalog and bootstrap) already sit under src, so every selection
// uses the same three roots; listing them again would overlap and fail INPUT_DUPLICATE.
function inputManifest(selected){
  return {inputs:['src','.editorconfig','docs/code']};
}
function environment(root){
  return workspace.isolationEnv(root);
}
function toolIdentity(root,env=environment(root)){
  const version=spawnSync('dotnet',['--version'],{cwd:dotnetLaunch(root,'dotnet',[]).cwd,env,encoding:'utf8',windowsHide:true,timeout:12000});
  requireTrue(version.status===0,'DOTNET_MISSING');
  return hash(JSON.stringify({node:process.version,dotnet:version.stdout.trim(),
    pins:hash(fs.readFileSync(path.join(root,'src/godot-game/tools/verification/toolchain.json'))),
    testBinary:hash(fs.readFileSync(path.join(root,'src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll')))}));
}
module.exports={main,select,reconcile,acquireLock,inputManifest,environment,toolIdentity};
if(require.main===module)main(path.resolve(__dirname,'../../../..'),process.argv.slice(2)).catch(error=>{console.error(error.stack);process.exitCode=1;});
