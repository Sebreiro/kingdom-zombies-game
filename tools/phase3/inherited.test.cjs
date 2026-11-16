'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const e=require('./evidence.cjs'),inherited=require('./inherited.cjs');
function fixture(mode){
  const root=fs.mkdtempSync(path.resolve(__dirname,'../../.phase3/inherited-control-'));
  const write=(file,value)=>{fs.mkdirSync(path.dirname(path.join(root,file)),{recursive:true});fs.writeFileSync(path.join(root,file),typeof value==='string'?value:JSON.stringify(value));};
  const phase1=[{id:'ordinary.failure',group:'rules'},{id:'ordinary.success',group:'rules'},{id:'engine.source-policy',group:'engine'}];
  const phase2=[{id:'core.control',group:'core'},{id:'runner.control',group:'runner'},{id:'rendered.control',group:'rendered'}];
  write('tools/phase1/required-cases.json',{cases:phase1});write('tools/phase2/required-cases.json',{cases:phase2});
  const script=`'use strict';
const fs=require('node:fs'),path=require('node:path'),assert=require('node:assert/strict');
const family=path.basename(__dirname),catalog=require('./required-cases.json').cases,args=process.argv.slice(2);
const selected=catalog.filter(row=>args[0]==='--group'?row.group===args[1]:row.id===args[1]).map(row=>row.id);
const mode=process.env.CONTROL_MODE;const directory='.'+family+'/runs/'+family+'-'+args[1];
fs.mkdirSync(directory,{recursive:true});fs.appendFileSync('children.log',family+'/'+args.join(' ')+'\\n');
let status='pass',results=selected.map(id=>({id,status:'pass'})),failures=[];
if(mode==='setup'&&family==='phase2'&&args[1]==='core'){status='fail';results=selected.map(id=>({id,status:'blocked',message:'CONTROL_PREREQUISITE_MISSING'}));}
if(family==='phase1'&&selected.includes('ordinary.failure')){
  try{assert.equal(1,2,'controlled assertion failure');}catch(error){status='fail';results=results.filter(row=>row.id!=='ordinary.failure');failures.push({id:'ordinary.failure',status:'fail',message:error.message});}
}
if(mode==='integrity'&&selected.includes('engine.source-policy')){status='fail';results=results.map(row=>({...row,status:'fail'}));}
if(mode==='missing-report'&&selected.includes('ordinary.failure')){console.error('CONTROL_SETUP_NO_REPORT');process.exit(2);}
fs.writeFileSync(directory+'/summary.json',JSON.stringify({status,selected,results,failures,nonce:directory}));
console.log(status.toUpperCase()+' '+directory+'/summary.json');process.exitCode=status==='pass'?0:1;
`;
  write('tools/phase1/check.cjs',script);write('tools/phase2/check.cjs',script);
  for(const input of require('./check.cjs').inputManifest().inputs){
    if(!fs.existsSync(path.join(root,input)))write(path.posix.basename(input).includes('.')?input:input+'/control.txt','CONTROL_ONLY');
  }
  write('tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll','CONTROL_IDENTITY_ONLY_NEVER_EXECUTED');
  const directory=path.join(root,'.phase3/run');fs.mkdirSync(directory,{recursive:true});
  return {root,context:{root,directory,env:{...process.env,CONTROL_MODE:mode}},selection:{selected:{phase1:phase1.map(row=>row.id),phase2:phase2.map(row=>row.id)}}};
}
test('real inherited assertion failure retains immutable failed report and later independent successes',async()=>{
  const f=fixture('ordinary'),result=await inherited.run(f.context,f.selection);
  assert.equal(result.status,'fail');assert.equal(result.nativeAllowed,true);assert.equal(result.commands.length,6);
  assert.equal(result.phase1.length,3);assert.equal(result.phase2.length,3);
  const failed=result.commands.find(row=>row.selected.includes('ordinary.failure'));
  assert.equal(failed.status,'fail');assert.match(failed.reason,/CHILD_EXIT 1/);
  assert.equal(e.read(e.artifact({directory:f.root},failed.process)).code,1);
  assert.match(fs.readFileSync(e.artifact({directory:f.root},failed.logs.stdout),'utf8'),/FAIL/);
  const file=e.artifact({directory:f.root},failed.report),bytes=fs.readFileSync(file);
  assert.equal(JSON.parse(bytes).failures[0].status,'fail');
  const later=result.commands.find(row=>row.selected.includes('ordinary.success'));assert.equal(later.status,'pass');
  assert.ok(later.startedAt>=failed.finishedAt);assert.deepEqual(fs.readFileSync(file),bytes);
  for(const row of result.commands){assert.equal(row.executed,true);e.artifact({directory:f.root},row.process);e.artifact({directory:f.root},row.record);}
  assert.throws(()=>e.verifyInherited(f.root,{inherited:result},f.selection,[]),/INHERITED_AGGREGATE_FAILED/);
});
test('actual outer coordinator preserves inherited references and fails after an independent native group',async()=>{
  const f=fixture('ordinary'),catalog=require('./catalog.cjs'),scenarios=require('./scenarios.cjs'),check=require('./check.cjs');
  const originalGate=catalog.prelaunch,originalNative=scenarios.run,originalExit=process.exitCode;
  const cases=[{id:'rendered.control',needsGameWindow:true,group:'control',implementation:'control'}];
  fs.writeFileSync(path.join(f.root,'tools/phase3/required-cases.json'),JSON.stringify({cases}));let nativeRuns=0;
  catalog.prelaunch=()=>({review:{path:'CONTROL_ONLY',sha256:'CONTROL_ONLY'},selection:{sha256:'CONTROL_ONLY'},savedSelection:f.selection});
  scenarios.run=async context=>{
    nativeRuns++;const artifacts=['one.log','two.log'].map(name=>{
      fs.writeFileSync(path.join(context.directory,name),'CONTROL_NATIVE_DEPENDENCY_ONLY');return {path:name,sha256:e.hash(fs.readFileSync(path.join(context.directory,name)))};
    });
    return {launches:[],results:[{id:'rendered.control',status:'pass',...Object.fromEntries(['nonce','input','tool','platform','revision','osFingerprint'].map(key=>[key,context[key]])),artifacts}]};
  };
  try{
    const summary=await check.main(f.root,['--all']);assert.equal(summary.status,'fail');assert.equal(nativeRuns,1);
    assert.equal(summary.results[0].status,'pass');assert.equal(summary.inherited.status,'fail');
    assert.equal(summary.inherited.phase1.length,3);assert.equal(summary.inherited.phase2.length,3);
    for(const reference of summary.inherited.phase1){e.artifact({directory:f.root},reference);e.artifact({directory:f.root},reference.process);}
    const file=path.join(f.root,'.phase3/runs',summary.platform,summary.nonce,'summary.json');
    assert.deepEqual(e.read(file).inherited,JSON.parse(JSON.stringify(summary.inherited)));
  }finally{catalog.prelaunch=originalGate;scenarios.run=originalNative;process.exitCode=originalExit;}
});
test('integrity or setup failures block dependent native selections while pure commands continue',async()=>{
  for(const mode of ['integrity','setup','missing-report']){
    const f=fixture(mode),result=await inherited.run(f.context,f.selection);
    assert.equal(result.status,'fail');assert.equal(result.commands.find(row=>row.args[1]==='runner').status,'pass');
    const native=result.commands.find(row=>row.args[1]==='rendered');
    if(mode==='missing-report'){
      assert.equal(native.status,'pass');assert.equal(result.nativeAllowed,true);
      const missing=result.commands.find(row=>row.selected.includes('ordinary.failure'));
      assert.equal(missing.status,'blocked');assert.equal(missing.report,undefined);assert.equal(e.read(e.artifact({directory:f.root},missing.process)).code,2);
      assert.match(fs.readFileSync(e.artifact({directory:f.root},missing.logs.stderr),'utf8'),/CONTROL_SETUP_NO_REPORT/);
    }else{
      assert.equal(native.status,'blocked');assert.equal(native.executed,false);assert.equal(native.process,undefined);
      assert.match(native.reason,/PREREQUISITE_FAILED/);assert.equal(result.nativeAllowed,false);
      assert.equal(fs.readFileSync(path.join(f.root,'children.log'),'utf8').includes('phase2/--group rendered'),false);
    }
    assert.throws(()=>e.verifyInherited(f.root,{inherited:result},f.selection,[]),/INHERITED_AGGREGATE_FAILED/);
  }
});
