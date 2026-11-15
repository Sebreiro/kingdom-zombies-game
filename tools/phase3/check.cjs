'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),os=require('node:os');
const {spawnSync}=require('node:child_process');
const baseline=require('../phase1/check.cjs'),e=require('./evidence.cjs');
const {run:command}=require('./processes.cjs');
const {observed}=require('./network.cjs');
const {hash,requireTrue}=e;
function select(catalog,args){
  requireTrue(args.length===1&&['--list','--all'].includes(args[0])||args.length===2&&['--case','--group','--failed'].includes(args[0]),'USAGE: --list | --all | --case ID[,ID] | --group GROUP | --failed REPORT');
  requireTrue(Array.isArray(catalog.cases)&&catalog.cases.length>0,'CATALOG_EMPTY');
  const known=new Map(catalog.cases.map(row=>[row.id,row]));requireTrue(known.size===catalog.cases.length,'CATALOG_DUPLICATE');
  if(args[0]==='--list')return {list:true,selected:catalog.cases};
  if(args[0]==='--all')return {all:true,selected:catalog.cases};
  if(args[0]==='--failed')return {repair:args[1]};
  if(args[0]==='--group'){const selected=catalog.cases.filter(row=>row.group===args[1]);requireTrue(selected.length>0,'SELECTION_EMPTY_OR_UNKNOWN');return {selected};}
  const ids=args[1].split(',');requireTrue(new Set(ids).size===ids.length,'SELECTION_DUPLICATE');
  requireTrue(ids.every(id=>id.length>0&&known.has(id)),'SELECTION_EMPTY_OR_UNKNOWN');return {selected:ids.map(id=>known.get(id))};
}
function inputManifest(){return {inputs:['src','tests/phase1','tools/phase3','tools/phase2','tools/phase1/toolchain.json','Directory.Build.props','Directory.Build.targets',
  'Directory.Packages.props','.editorconfig','BannedSymbols.txt','BannedSymbols.Pure.txt','docs/code','docs/architecture','tools/code-rules',
  'docs/workflow/local-testing.md','docs/workflow/ai-collaboration.md',
  '.planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md','.planning/phases/03-four-player-lan-foundation/03-SELECTION.json']};}
function environment(root){return {...process.env,DOTNET_CLI_HOME:path.join(root,'.phase1/cli-home'),NUGET_PACKAGES:path.join(root,'.phase1/packages'),
  NUGET_HTTP_CACHE_PATH:path.join(root,'.phase1/http-cache'),TEMP:path.join(root,'.phase1/temp'),TMP:path.join(root,'.phase1/temp')};}
function createIdentity(root){
  const env=environment(root),version=spawnSync('dotnet',['--version'],{cwd:root,env,encoding:'utf8',windowsHide:true,timeout:12000});requireTrue(version.status===0,'DOTNET_MISSING');
  const revision=spawnSync('git',['rev-parse','HEAD'],{cwd:root,encoding:'utf8',windowsHide:true});requireTrue(revision.status===0,'REVISION_UNAVAILABLE');
  const operatingSystem={platform:process.platform,architecture:process.arch,release:os.release(),version:os.version()};
  return {platform:process.platform+'-'+process.arch,os:operatingSystem,osFingerprint:hash(JSON.stringify(operatingSystem)),
    input:baseline.snapshot(root,inputManifest()).sha256,revision:revision.stdout.trim(),
    tool:hash(JSON.stringify({node:process.version,dotnet:version.stdout.trim(),pins:hash(fs.readFileSync(path.join(root,'tools/phase1/toolchain.json'))),
      testBinary:hash(fs.readFileSync(path.join(root,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll')))}))};
}
async function csharp(context,item){
  const env={...context.env,PHASE1_ROOT:context.root,PHASE1_RESULT_DIRECTORY:context.directory,
    PHASE1_NONCE:context.nonce,PHASE1_INPUT:context.input,PHASE1_TOOL:context.tool,PHASE1_PLATFORM:context.platform};
  let run;
  try{run=await command('dotnet',[path.join(context.root,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),
    '--filter-method',item.test,'--report-xunit-ctrf','--report-xunit-ctrf-filename',item.id+'.ctrf.json','--results-directory',context.directory],{cwd:context.root,env},180000);}
  catch(error){fs.writeFileSync(path.join(context.directory,item.id+'.console.log'),JSON.stringify(error.record??{error:error.stack},null,2),{flag:'wx'});throw error;}
  fs.writeFileSync(path.join(context.directory,item.id+'.console.log'),run.stdout+run.stderr,{flag:'wx'});
  const report=e.read(path.join(context.directory,item.id+'.ctrf.json'));
  requireTrue(report.results.summary.tests===1&&report.results.summary.passed===1&&report.results.summary.failed===0,'TEST_EXECUTION_COUNT');
  const proof=e.read(path.join(context.directory,item.id+'.proof.json'));
  for(const key of ['nonce','input','tool','platform'])requireTrue(proof[key]===context[key],'PROOF_IDENTITY '+key);
  requireTrue(proof.id===item.id&&proof.status==='pass'&&proof.evidence.length>0,'PROOF_SUBJECT');
  return {...proof,revision:context.revision,osFingerprint:context.osFingerprint,testReport:item.id+'.ctrf.json',artifacts:[...proof.evidence,item.id+'.ctrf.json'].map(file=>({path:file,sha256:hash(fs.readFileSync(e.inside(context.directory,file)))}))};
}
async function main(root,args){
  const catalog=e.read(path.join(root,'tools/phase3/required-cases.json'));
  if(args[0]==='--verify-completion'){
    requireTrue(args.length===2,'USAGE_COMPLETION');const index=e.read(e.inside(root,args[1]));
    const current=createIdentity(root);const result=e.verifyCompletion(root,index,catalog.cases,current);console.log(JSON.stringify(result));return result;
  }
  const selection=select(catalog,args);
  if(selection.list){require('./catalog.cjs').validate(root,catalog);console.log(JSON.stringify(selection.selected,null,2));return;}
  let repairOf;
  if(selection.repair){
    const file=e.inside(root,selection.repair),previous=e.read(file);requireTrue(previous.schemaVersion===2&&Array.isArray(previous.results),'REPAIR_REPORT_SCHEMA');
    e.exact(previous.selected,previous.results.map(row=>row.id),'repair baseline');
    const ids=previous.results.filter(row=>row.status!=='pass').map(row=>row.id);requireTrue(ids.length>0,'NO_FAILED_CASES');
    selection.selected=select(catalog,['--case',ids.join(',')]).selected;repairOf={path:selection.repair,sha256:hash(fs.readFileSync(file)),nonce:previous.nonce};
  }
  const nonce=crypto.randomUUID(),identity=createIdentity(root),directory=e.inside(root,'.phase3/runs/'+identity.platform+'/'+nonce);
  fs.mkdirSync(directory,{recursive:true});fs.writeFileSync(path.join(directory,'observation.owner'),nonce,{flag:'wx'});
  const context={root,directory,nonce,...identity,env:environment(root)};
  fs.writeFileSync(path.join(directory,'inputs.json'),JSON.stringify(baseline.snapshot(root,inputManifest()),null,2),{flag:'wx'});
  const startedAt=Date.now(),results=[];let prelaunchReview,selectionSha256,inherited,launches=[];
  const native=selection.selected.filter(row=>row.needsGameWindow);
  for(const item of selection.selected.filter(row=>!row.needsGameWindow)){
    console.log('Checking '+item.id);
    try{const result=item.implementation==='csharp'?await csharp(context,item):item.id==='runner.timing'?await require('./rendered.cjs').timingControl(context,item):await observed(context,item);
      e.reconcile([item],[result],context);results.push(result);
    }catch(error){fs.writeFileSync(path.join(directory,item.id+'.failure.log'),error.stack,{flag:'wx'});results.push({id:item.id,status:'fail',...identity,nonce,message:error.message,stack:error.stack});}
  }
  if(native.length)try{
    requireTrue(results.every(row=>row.status==='pass'),'NONWINDOW_PREREQUISITE_FAILED');
    const gate=require('./catalog.cjs').prelaunch(root,context);prelaunchReview=gate.review;selectionSha256=gate.selection.sha256;
    if(selection.all)inherited=await require('./inherited.cjs').run(context,gate.savedSelection);
    const rendered=await require('./scenarios.cjs').run(context,native);results.push(...rendered.results);launches=rendered.launches;
  }catch(error){for(const item of native)if(!results.some(row=>row.id===item.id))results.push({id:item.id,status:'blocked',...identity,nonce,message:error.message});}
  for(const result of results.filter(row=>row.status==='pass'))try{e.reconcile([catalog.cases.find(item=>item.id===result.id)],[result],context);}catch(error){result.status='fail';result.message=error.message;}
  if(baseline.snapshot(root,inputManifest()).sha256!==context.input)for(const result of results){result.status='fail';result.message='SOURCE_CHANGED';}
  const status=results.length===selection.selected.length&&results.every(row=>row.status==='pass')?'pass':'fail';
  const summary={schemaVersion:2,...identity,nonce,status,selected:selection.selected.map(row=>row.id),results,repairOf,prelaunchReview,selectionSha256,inherited,
    startedAt,finishedAt:Date.now(),launches,gameWindows:launches.length,scope:native.length?'Windows 1280x720 local; no VM':'headless diagnostics and pure proofs only'};
  for(const result of results)fs.writeFileSync(path.join(directory,result.id+'.result.json'),JSON.stringify(result,null,2),{flag:'wx'});
  fs.writeFileSync(path.join(directory,'summary.json'),JSON.stringify(summary,null,2),{flag:'wx'});
  console.log(status.toUpperCase()+' '+results.filter(row=>row.status==='pass').length+'/'+selection.selected.length+'; '+path.relative(root,path.join(directory,'summary.json')));
  for(const result of results.filter(row=>row.status!=='pass'))console.error(result.id+': '+result.message);
  if(status!=='pass')process.exitCode=1;return summary;
}
module.exports={main,select,csharp,requireTrue,hash,inputManifest,createIdentity,environment};
if(require.main===module)main(path.resolve(__dirname,'../..'),process.argv.slice(2)).catch(error=>{console.error(error.stack);process.exitCode=1;});
