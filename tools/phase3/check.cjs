'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {spawnSync}=require('node:child_process');
const baseline=require('../phase1/check.cjs');
const {command}=require('../phase2/rendered.cjs');
const {observed}=require('./network.cjs');
const evidence=require('./evidence.cjs');
const hash=bytes=>crypto.createHash('sha256').update(bytes).digest('hex');
function requireTrue(value,message){if(!value)throw new Error(message);}
function select(catalog,args){
  requireTrue(args.length===1&&args[0]==='--list'||args.length===2&&['--case','--group'].includes(args[0]),'USAGE: --list | --case ID[,ID] | --group GROUP');
  requireTrue(Array.isArray(catalog.cases)&&catalog.cases.length>0,'CATALOG_EMPTY');
  const known=new Map(catalog.cases.map(row=>[row.id,row]));
  requireTrue(known.size===catalog.cases.length,'CATALOG_DUPLICATE');
  if(args[0]==='--list')return {list:true,selected:catalog.cases};
  if(args[0]==='--group'){
    const selected=catalog.cases.filter(row=>row.group===args[1]);
    requireTrue(selected.length>0,'SELECTION_EMPTY_OR_UNKNOWN');return {selected};
  }
  const ids=args[1].split(',');requireTrue(ids.length>0&&new Set(ids).size===ids.length,'SELECTION_DUPLICATE');
  requireTrue(ids.every(id=>id.length>0&&known.has(id)),'SELECTION_EMPTY_OR_UNKNOWN');
  return {selected:ids.map(id=>known.get(id))};
}
async function csharp(context,item){
  const env={...context.env,PHASE1_ROOT:context.root,PHASE1_RESULT_DIRECTORY:context.directory,
    PHASE1_NONCE:context.nonce,PHASE1_INPUT:context.input,PHASE1_TOOL:context.tool,PHASE1_PLATFORM:context.platform};
  const run=await command('dotnet',[path.join(context.root,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),
    '--filter-method',item.test,'--report-xunit-ctrf','--report-xunit-ctrf-filename',item.id+'.ctrf.json',
    '--results-directory',context.directory],{cwd:context.root,env},180000);
  fs.writeFileSync(path.join(context.directory,item.id+'.console.log'),run.stdout+run.stderr);
  const report=JSON.parse(fs.readFileSync(path.join(context.directory,item.id+'.ctrf.json'),'utf8'));
  requireTrue(report.results.summary.tests===1&&report.results.summary.passed===1&&report.results.summary.failed===0,'TEST_EXECUTION_COUNT');
  const proof=JSON.parse(fs.readFileSync(path.join(context.directory,item.id+'.proof.json'),'utf8'));
  for(const key of ['nonce','input','tool','platform'])requireTrue(proof[key]===context[key],'PROOF_IDENTITY '+key);
  requireTrue(proof.id===item.id&&proof.status==='pass'&&proof.evidence.length>0,'PROOF_SUBJECT');
  return {...proof,revision:context.revision,testReport:item.id+'.ctrf.json',artifacts:[...proof.evidence,item.id+'.ctrf.json'].map(file=>({path:file,sha256:hash(fs.readFileSync(baseline.inside(context.directory,file)))}))};
}
async function main(root,args){
  const catalog=JSON.parse(fs.readFileSync(path.join(root,'tools/phase3/required-cases.json'),'utf8'));
  const selection=select(catalog,args);if(selection.list){console.log(JSON.stringify(selection.selected,null,2));return;}
  const nonce=crypto.randomUUID(),platform=process.platform+'-'+process.arch;
  const directory=baseline.inside(root,'.phase3/runs/'+platform+'/'+nonce);fs.mkdirSync(directory,{recursive:true});
  fs.writeFileSync(path.join(directory,'observation.owner'),nonce,{flag:'wx'});
  const env={...process.env,DOTNET_CLI_HOME:path.join(root,'.phase1/cli-home'),NUGET_PACKAGES:path.join(root,'.phase1/packages'),
    NUGET_HTTP_CACHE_PATH:path.join(root,'.phase1/http-cache'),TEMP:path.join(root,'.phase1/temp'),TMP:path.join(root,'.phase1/temp')};
  const inputs={inputs:['src','tests/phase1','tools/phase3','tools/phase1/toolchain.json','Directory.Build.props','Directory.Build.targets',
    'Directory.Packages.props','.editorconfig','BannedSymbols.txt','BannedSymbols.Pure.txt','docs/code','tools/code-rules']};
  const snapshot=baseline.snapshot(root,inputs);fs.writeFileSync(path.join(directory,'inputs.json'),JSON.stringify(snapshot,null,2));
  const version=spawnSync('dotnet',['--version'],{cwd:root,env,encoding:'utf8',windowsHide:true,timeout:12000});
  requireTrue(version.status===0,'DOTNET_MISSING');
  const revision=spawnSync('git',['rev-parse','HEAD'],{cwd:root,encoding:'utf8',windowsHide:true});requireTrue(revision.status===0,'REVISION_UNAVAILABLE');
  const context={root,directory,nonce,platform,input:snapshot.sha256,env,revision:revision.stdout.trim(),
    tool:hash(JSON.stringify({node:process.version,dotnet:version.stdout.trim(),pins:hash(fs.readFileSync(path.join(root,'tools/phase1/toolchain.json'))),
      testBinary:hash(fs.readFileSync(path.join(root,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll')))}))};
  const startedAt=Date.now(),results=[];
  for(const item of selection.selected){console.log('Checking '+item.id);try{
    const result=item.implementation==='csharp'?await csharp(context,item):item.id==='runner.timing'?await require('./rendered.cjs').timingControl(context,item):await observed(context,item);
    evidence.reconcile([item],[result],context);
    results.push(result);
  }catch(error){results.push({id:item.id,status:'fail',nonce,input:context.input,tool:context.tool,platform,message:error.message,stack:error.stack});}}
  const unchanged=baseline.snapshot(root,inputs).sha256===context.input;
  if(!unchanged)for(const result of results){result.status='fail';result.message='SOURCE_CHANGED';}
  const status=results.length===selection.selected.length&&results.every(row=>row.status==='pass')?'pass':'fail';
  const summary={schemaVersion:2,...context,env:undefined,root:undefined,directory:undefined,status,selected:selection.selected.map(row=>row.id),
    results,startedAt,finishedAt:Date.now(),revision:revision.stdout.trim(),gameWindows:0,scope:'headless diagnostics and pure proofs only'};
  fs.writeFileSync(path.join(directory,'summary.json'),JSON.stringify(summary,null,2),{flag:'wx'});
  console.log(status.toUpperCase()+' '+results.filter(row=>row.status==='pass').length+'/'+selection.selected.length+'; '+path.relative(root,path.join(directory,'summary.json')));
  for(const result of results.filter(row=>row.status!=='pass'))console.error(result.id+': '+result.message);
  if(status!=='pass')process.exitCode=1;return summary;
}
module.exports={main,select,csharp,requireTrue,hash};
if(require.main===module)main(path.resolve(__dirname,'../..'),process.argv.slice(2)).catch(error=>{console.error(error.stack);process.exitCode=1;});
