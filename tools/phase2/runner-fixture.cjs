'use strict';
// Deliberate controls exercise the same evidence and process functions as the CLI.
// These children never start Godot or send desktop input.
const fs=require('node:fs');
const path=require('node:path');
const crypto=require('node:crypto');
const zlib=require('node:zlib');
const {spawn}=require('node:child_process');
const assert=require('node:assert/strict');
const e=require('./evidence.cjs');
const {command}=require('./rendered.cjs');
const {acquireLock,select}=require('./check.cjs');
const delay=milliseconds=>new Promise(resolve=>setTimeout(resolve,milliseconds));
function rejects(action,pattern){assert.throws(action,pattern);}
function png() {
  function chunk(type,bytes){const head=Buffer.alloc(8),tail=Buffer.alloc(4);head.writeUInt32BE(bytes.length);head.write(type,4);return Buffer.concat([head,bytes,tail]);}
  const header=Buffer.alloc(13);header.writeUInt32BE(1280);header.writeUInt32BE(720,4);header[8]=8;header[9]=2;
  const raw=Buffer.alloc(720*(1+1280*3));
  for(let y=0;y<720;y++)for(let x=0;x<1280;x++)raw[y*(1+1280*3)+1+x*3]=x%255;
  return Buffer.concat([Buffer.from([137,80,78,71,13,10,26,10]),chunk('IHDR',header),chunk('IDAT',zlib.deflateSync(raw)),chunk('IEND',Buffer.alloc(0))]);
}
async function run(mode) {
  const root=path.resolve(__dirname,'../..'),directory=path.join(root,'.phase2/controls',crypto.randomUUID());
  fs.mkdirSync(directory,{recursive:true});
  const context={directory,nonce:crypto.randomUUID(),input:'input',tool:'tool',platform:process.platform+'-'+process.arch};
  const selected=[{id:'rendered.tracer',implementation:'native',images:['launch-menu']},{id:'core.authority',implementation:'csharp'}];
  const image=png();fs.writeFileSync(path.join(directory,'image.png'),image);
  const state=JSON.stringify({nonce:context.nonce,checkpoint:'image',width:1280,height:720});
  fs.writeFileSync(path.join(directory,'image.json'),state);
  const first={...context,id:selected[0].id,status:'pass',pid:123,width:1280,height:720,images:[{checkpoint:'launch-menu',path:'image.png',sha256:e.hash(image),statePath:'image.json',stateSha256:e.hash(state)}]};
  delete first.directory;
  fs.writeFileSync(path.join(directory,'subject.log'),'ASSERTION_PASSED');
  fs.writeFileSync(path.join(directory,'tests.json'),JSON.stringify({results:{summary:{tests:1,passed:1,failed:0}}}));
  const artifacts=['subject.log','tests.json'].map(file=>({path:file,sha256:e.hash(fs.readFileSync(path.join(directory,file)))}));
  const second={...first,id:selected[1].id,images:[],testReport:'tests.json',artifacts};
  const results=[first,second];
  if(mode==='integrity') {
    e.reconcile(selected,results,context);
    rejects(()=>e.reconcile(selected,[],context),/RESULT_COUNT/);
    rejects(()=>e.reconcile(selected,[first],context),/RESULT_COUNT/);
    rejects(()=>e.reconcile(selected,[first,first],context),/DUPLICATE/);
    rejects(()=>e.reconcile(selected,[first,{...second,id:'unknown'}],context),/DUPLICATE/);
    for(const field of ['nonce','input','tool','platform'])rejects(()=>e.reconcile(selected,[{...first,[field]:'stale'},second],context),/RESULT_IDENTITY/);
    rejects(()=>e.reconcile(selected,[{...first,images:[]},second],context),/IMAGE_MISSING/);
    rejects(()=>e.reconcile(selected,[{...first,status:'fail'},second],context),/NOT_PASS/);
    rejects(()=>e.reconcile(selected,[first,{...second,artifacts:[]}],context),/SUBJECT_LOG_REQUIRED/);
    rejects(()=>e.reconcile(selected,[{...first,images:[first.images[0],first.images[0]]},second],context),/IMAGE_DUPLICATE/);
    rejects(()=>e.reconcile(selected,[{...first,images:[{...first.images[0],path:'../escape.png'}]},second],context),/PATH_ESCAPE/);
    fs.writeFileSync(path.join(directory,'image.png'),'corrupt');
    rejects(()=>e.reconcile(selected,results,context),/IMAGE_HASH/);
    fs.unlinkSync(path.join(directory,'image.png'));rejects(()=>e.reconcile(selected,results,context),/ENOENT/);
    rejects(()=>select({cases:selected},['--case','missing']),/SELECTION_EMPTY/);
    rejects(()=>select({cases:selected},['--all','--group','rendered']),/USAGE/);
    console.log('PASS empty, missing, duplicate, unknown, stale, corrupt, missing-image, traversal and invalid selection controls');
  } else if(mode==='processes') {
    await command(process.execPath,['-e','process.stdout.write("positive")']).then(row=>assert.equal(row.stdout,'positive'));
    await assert.rejects(command(path.join(directory,'missing-engine'),[]),/ENOENT/);
    await assert.rejects(command(process.execPath,['-e','process.exit(7)']),/INPUT_EXIT 7/);
    await assert.rejects(command(process.execPath,['-e','process.abort()'],{cwd:directory}),/INPUT_EXIT/);
    const descendantFile=path.join(directory,'descendant.json');
    const script='const fs=require("node:fs"),{spawn}=require("node:child_process");const child=spawn(process.execPath,["-e","setInterval(()=>{},1000)"],{stdio:"ignore",windowsHide:true});fs.writeFileSync(process.argv[1],JSON.stringify({parent:process.pid,child:child.pid}));setInterval(()=>{},1000);';
    await assert.rejects(command(process.execPath,['-e',script,descendantFile],{cwd:directory},1000),/INPUT_DEADLINE/);
    const pids=e.read(descendantFile);
    for(const pid of Object.values(pids)) {
      let live=true;
      for(let attempt=0;attempt<20;attempt++){try{process.kill(pid,0);}catch{live=false;break;}await delay(50);}
      assert.equal(live,false,'owned timeout process survived '+pid);
    }
    console.log('PASS real missing executable, nonzero, crash and timeout with descendant cleanup');
  } else if(mode==='repeatability') {
    e.reconcile(selected,[second,first],context);
    const firstFile=path.join(directory,'summary.json');
    const summary={schemaVersion:2,...context,status:'fail',selected:selected.map(row=>row.id),results:[first,{...second,status:'fail'}],failures:[],revision:'baseline',startedAt:1,finishedAt:2};
    delete summary.directory;
    fs.writeFileSync(firstFile,JSON.stringify(summary),{flag:'wx'});
    const reference={path:path.relative(root,firstFile),sha256:e.hash(fs.readFileSync(firstFile))};
    const repairFile=path.join(directory,'repair.json'),repairContext={...context,nonce:crypto.randomUUID(),input:'changed-after-repair'};
    const repaired={...second,nonce:repairContext.nonce,input:repairContext.input};
    fs.writeFileSync(repairFile,JSON.stringify({...summary,...repairContext,status:'pass',selected:[second.id],results:[repaired],repairOf:reference,revision:'repair',startedAt:3,finishedAt:4}));
    const repair={path:path.relative(root,repairFile),sha256:e.hash(fs.readFileSync(repairFile))};
    assert.equal(e.aggregate(root,[reference,repair],selected).status,'coverage-complete');
    assert.equal(e.aggregate(root,[reference],selected).status,'incomplete');
    rejects(()=>e.verifyCompletion(root,{reports:[reference,repair],reviews:[]},selected),/APPEARANCE_REVIEW_REQUIRED/);
    rejects(()=>e.aggregate(root,[repair],selected),/REPAIR_BASELINE_MISSING/);
    rejects(()=>e.aggregate(root,[reference,reference],selected),/REPORT_DUPLICATE/);
    assert.equal(e.hash(fs.readFileSync(firstFile)),reference.sha256,'baseline mutated');
    const lockRoot=path.join(directory,'lock-root');fs.mkdirSync(lockRoot);
    const unlock=acquireLock(lockRoot,context.nonce);
    try {
      const modulePath=path.join(root,'tools/phase2/check.cjs');
      await assert.rejects(command(process.execPath,['-e','require(process.argv[1]).acquireLock(process.argv[2],"competitor")',modulePath,lockRoot]),/DESKTOP_LOCKED/);
    } finally {unlock();}
    const release=acquireLock(lockRoot,crypto.randomUUID());release();
    console.log('PASS reordered evidence, fresh repair nonce, changed fingerprint temporary aggregation, immutable baseline and actual concurrent lock refusal');
  } else throw new Error('UNKNOWN_CONTROL');
  return {directory,mode};
}
module.exports={run};
if(require.main===module)run(process.argv[2]).then(result=>console.log(JSON.stringify(result))).catch(error=>{console.error(error.stack);process.exitCode=1;});
