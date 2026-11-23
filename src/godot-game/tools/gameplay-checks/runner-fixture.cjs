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
const {command,runScenarios,closeSession,ownedInput,validateResources}=require('./rendered.cjs');
const {acquireLock,select}=require('./check.cjs');
const workspace=require('../workspace-paths.cjs');
const delay=milliseconds=>new Promise(resolve=>setTimeout(resolve,milliseconds));
function rejects(action,pattern){assert.throws(action,pattern);}
async function sharedRoot(directory,body){
  const previous=process.env.KZ_SHARED_LOCAL;process.env.KZ_SHARED_LOCAL=directory;
  try{return await body();}
  finally{if(previous===undefined)delete process.env.KZ_SHARED_LOCAL;else process.env.KZ_SHARED_LOCAL=previous;}
}
function png() {
  function chunk(type,bytes){const head=Buffer.alloc(8),tail=Buffer.alloc(4);head.writeUInt32BE(bytes.length);head.write(type,4);return Buffer.concat([head,bytes,tail]);}
  const header=Buffer.alloc(13);header.writeUInt32BE(1280);header.writeUInt32BE(720,4);header[8]=8;header[9]=2;
  const raw=Buffer.alloc(720*(1+1280*3));
  for(let y=0;y<720;y++)for(let x=0;x<1280;x++)raw[y*(1+1280*3)+1+x*3]=x%255;
  return Buffer.concat([Buffer.from([137,80,78,71,13,10,26,10]),chunk('IHDR',header),chunk('IDAT',zlib.deflateSync(raw)),chunk('IEND',Buffer.alloc(0))]);
}
async function run(mode) {
  const root=path.resolve(__dirname,'../../../..');
  const directory=workspace.locations(root).temp('gameplay-checks','controls',crypto.randomUUID());
  fs.mkdirSync(directory,{recursive:true});
  if(mode==='resource-roles') {
    const current=validateResources({root,directory});
    assert.deepEqual(current.projectResourceRoles,{
      'run/main_scene':'foundation/Foundation.tscn',
      'run/main_scene.asset_inspection':'inspection/Inspection.tscn',
      'config/icon':'assets/ui/kingdom-zombies-icon.png'
    });
    const project=path.join(current.isolatedInputRoot,'src/godot-game/game/project.godot');
    const source=fs.readFileSync(project,'utf8');
    for(const [setting,resource] of [['config/undeclared_fixture','assets/ui/undeclared-fixture.png'],
      ['config/extra_icon','assets/ui/kingdom-zombies-icon.png']]) {
      fs.writeFileSync(project,source.replace('[application]','[application]\n'+setting+'="res://'+resource+'"'));
      const target=path.join(path.dirname(project),resource);
      if(!fs.existsSync(target))fs.writeFileSync(target,Buffer.from('existing file'),{flag:'wx'});
      try {
        rejects(()=>validateResources({root:current.isolatedInputRoot,directory:path.join(directory,'rejected')}),
          error=>error.message==='UNDECLARED_RESOURCE '+resource);
      } finally {
        fs.writeFileSync(project,source);
      }
    }
    fs.copyFileSync(path.join(root,'src/godot-game/game/icon.svg'),path.join(path.dirname(project),'icon.svg'));
    const historical=source.replace(/^run\/main_scene\.asset_inspection=.*\r?\n/m,'')
      .replace('res://assets/ui/kingdom-zombies-icon.png','res://icon.svg');
    fs.writeFileSync(project,historical);
    const foundation=validateResources({root:current.isolatedInputRoot,directory:path.join(directory,'foundation')});
    assert.deepEqual(foundation.projectResourceRoles,{'run/main_scene':'foundation/Foundation.tscn','config/icon':'icon.svg'});
    console.log('PASS current three resource roles, historical Foundation roles and two undeclared resource controls');
    return {directory,mode};
  }
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
  if(mode==='session-cleanup') {
    for(const failure of ['GAME_EXITED_BEFORE_INPUT','INPUT_ACKNOWLEDGEMENT']) {
      const calls=[];
      let closing=false;
      const owned=ownedInput(async action=>{calls.push(action);if(closing)throw new Error(failure);});
      await owned.input('down',0,0,65);await owned.input('mouse-down',400,280);
      closing=true;
      const outcomes=[{id:'earlier',status:'pass',observations:[{checkpoint:'retained'}]},
        {id:'affected',status:failure==='GAME_EXITED_BEFORE_INPUT'?'fail':'pass',observations:[{checkpoint:'partial'}],
          ...(failure==='GAME_EXITED_BEFORE_INPUT'?{message:'GAME_PROCESS_LOST'}:{})},
        {id:'later',status:'blocked',message:'GAME_PROCESS_LOST'}];
      const earlier=structuredClone(outcomes[0]),blocked=structuredClone(outcomes[2]);
      await closeSession({close:()=>owned.release()},outcomes);
      assert.deepEqual(outcomes[0],earlier);assert.deepEqual(outcomes[2],blocked);
      assert.equal(outcomes[1].status,'fail');
      assert.equal(outcomes[1].scenarioStatus,failure==='GAME_EXITED_BEFORE_INPUT'?'fail':'pass');
      assert.deepEqual(outcomes[1].observations,[{checkpoint:'partial'}]);
      assert.match(outcomes[1].message,/SESSION_CLEANUP OWNED_INPUT_RELEASE_FAILED/);
      assert.deepEqual(outcomes[1].cleanupError.causes,[failure,failure]);
      assert.deepEqual(calls,['down','mouse-down','up','mouse-up']);
    }
    console.log('PASS crash/held-key and failed release acknowledgements retain earlier passes, partial observations and blocked outcomes while surfacing cleanup failure');
  } else if(mode==='owned-input') {
    for(const press of ['mouse-down','click']) {
      const calls=[];
      const owned=ownedInput(async (...args)=>{calls.push(args);if(args[0]===press)throw new Error('ACK_FAILED_AFTER_PRESS');});
      await assert.rejects(owned.input(press,450,300),/ACK_FAILED_AFTER_PRESS/);
      await owned.release();
      assert.deepEqual(calls.map(row=>row[0]),[press,'mouse-up']);
      assert.deepEqual(calls[1].slice(1,3),[450,300]);
      await owned.release();assert.equal(calls.length,2);
    }
    const observationCalls=[];
    const observationInput=ownedInput(async action=>{observationCalls.push(action);});
    await assert.rejects(async ()=>{
      try {await observationInput.input('mouse-down',400,280);throw new Error('OBSERVATION_FAILED');}
      finally {await observationInput.release();}
    },/OBSERVATION_FAILED/);
    assert.deepEqual(observationCalls,['mouse-down','mouse-up']);
    const calls=[];
    let failRelease=true;
    const owned=ownedInput(async (...args)=>{calls.push(args);if(args[0]==='up' && args[3]===65 && failRelease)throw new Error('KEY_RELEASE_FAILED');});
    await owned.input('down',0,0,65);await owned.input('down',0,0,68);
    await assert.rejects(async ()=>{
      try {await owned.input('mouse-down',400,280);throw new Error('OBSERVATION_FAILED');}
      finally {await owned.release();}
    },/OWNED_INPUT_RELEASE_FAILED/);
    assert.deepEqual(calls.slice(3).map(row=>[row[0],row[3]]),[['up',65],['up',68],['mouse-up',0]]);
    failRelease=false;await owned.release();
    assert.deepEqual(calls.at(-1),['up',0,0,65]);
    await owned.input('click',10,20);const count=calls.length;
    await owned.release();assert.equal(calls.length,count);
    console.log('PASS failed mouse-down/click acknowledgements and observation cleanup release owned mouse; failed key release cannot block remaining releases and retries retain ownership');
  } else if(mode==='scenario-diagnostics') {
    let log='WARNING: startup belongs to first scenario\n', resets=0, serial=0;
    const session={game:{pid:123},exited:false,get log(){return log;},async input(){},
      async reset(){resets++;},async capture(checkpoint){
        const actor={ActorId:'actor',SessionId:'session',PositionSubunits:-8000,Facing:1,ElapsedMilliseconds:0};
        const state={checkpoint,actors:0,actor:null,startFocused:true,inputEvents:0};
        if(checkpoint==='initial-scene')Object.assign(state,{actors:1,actor});
        if(checkpoint==='right-facing')Object.assign(state,{actors:1,actor:{...actor,PositionSubunits:-5000,ElapsedMilliseconds:1000},inputEvents:2});
        if(checkpoint==='left-facing')Object.assign(state,{actors:1,actor:{...actor,Facing:-1},inputEvents:3});
        return {state,image:{checkpoint,path:'fake-'+serial+++'.png'}};
      }};
    const outcomes=await runScenarios(context,[{id:'rendered.tracer'},{id:'control.independent'}],session);
    assert.deepEqual(outcomes.map(row=>row.status),['fail','pass']);
    assert.equal(resets,1);
    assert.match(outcomes[0].message,/NATIVE_DIAGNOSTIC WARNING: startup/);
    assert.equal(outcomes[0].nativeLog.includesStartup,true);
    assert.equal(outcomes[1].nativeLog.start,log.length);
    assert.equal(outcomes[1].nativeLog.includesStartup,false);
    assert.equal(log,'WARNING: startup belongs to first scenario\n');
    console.log('PASS earlier diagnostic fails first scenario, resets, and healthy independent scenario passes with full log retained');
  } else if(mode==='integrity') {
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
    // The desktop lock lives in the shared root; a temporary shared root keeps this control and its child off the real one.
    await sharedRoot(path.join(lockRoot,'shared'),async()=>{
      const unlock=acquireLock(lockRoot,context.nonce);
      try {
        const modulePath=path.join(root,'src/godot-game/tools/gameplay-checks/check.cjs');
        await assert.rejects(command(process.execPath,['-e','require(process.argv[1]).acquireLock(process.argv[2],"competitor")',modulePath,lockRoot]),/DESKTOP_LOCKED/);
      } finally {unlock();}
      const release=acquireLock(lockRoot,crypto.randomUUID());release();
    });
    await run('scenario-diagnostics');
    await run('owned-input');
    await run('session-cleanup');
    console.log('PASS reordered evidence, fresh repair nonce, changed fingerprint temporary aggregation, immutable baseline and actual concurrent lock refusal');
  } else throw new Error('UNKNOWN_CONTROL');
  return {directory,mode};
}
module.exports={run};
if(require.main===module)run(process.argv[2]).then(result=>console.log(JSON.stringify(result))).catch(error=>{console.error(error.stack);process.exitCode=1;});
