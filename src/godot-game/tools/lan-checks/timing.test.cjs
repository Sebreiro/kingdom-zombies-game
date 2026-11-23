'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const fs=require('node:fs');
// Controls live in this checkout's temp folder; a temporary shared root keeps them off the real one.
const workspace=require('../workspace-paths.cjs'),controls=workspace.locations().temp('lan-checks');
fs.mkdirSync(controls,{recursive:true});process.env.KZ_SHARED_LOCAL=fs.mkdtempSync(require('node:path').join(controls,'shared-'));
test('parallel PNG validation keeps observations responsive and preserves exact validation and hashes',async()=>{
  const path=require('node:path'),crypto=require('node:crypto'),parallel=require('./png-worker.cjs');
  const directory=fs.mkdtempSync(path.join(controls,'png-worker-control-'));
  const file=path.join(directory,'actual.png'),broken=path.join(directory,'broken.png');
  fs.writeFileSync(file,require('./runner-fixture.cjs').png());
  const bytes=fs.readFileSync(file),expected={...require('./png.cjs').validate(file),sha256:crypto.createHash('sha256').update(bytes).digest('hex')};
  try{
    let responsive=false;
    const pending=Array.from({length:4},()=>parallel.validate(file));
    setImmediate(()=>responsive=true);
    for(const decoded of await Promise.all(pending))assert.deepEqual(decoded,expected);
    assert.equal(responsive,true,'PNG reads, CRC, reconstruction and content checks must not block observation polling');
    bytes[bytes.length-5]^=1;fs.writeFileSync(broken,bytes);
    await assert.rejects(parallel.validate(broken),/PNG_CRC/);
    assert.deepEqual(await parallel.validate(file),expected,'a rejected image must not contaminate later work');
  }finally{await parallel.close();}
  try{assert.deepEqual(await parallel.validate(file),expected,'closed workers can be created for a later invocation');}
  finally{await parallel.close();}
});
test('draw timing requires an inclusive conservative external-clock bound',()=>{
  const moduleFile=__dirname+'/evidence.cjs';
  const evaluate=fs.existsSync(moduleFile)?require(moduleFile).timing:null;
  assert.equal(typeof evaluate,'function','post-draw timing evaluator must exist');
  const row={clock:'coordinator-hrtime',origin:'host',observer:'guest1',actor:'a',session:'s',revision:7,
    expectedActor:'a',expectedSession:'s',minimumRevision:7,deliveryBefore:'1000000000',deliveryAfter:'1001000000',
    observedBefore:'1249000000',observedAfter:'1250000000',uncertaintyNanoseconds:'0',drawFrame:1,
    drawn:true,visible:true,insideCamera:true,imageBound:true,provenance:'physical-SendInput',
    eventAcknowledged:true,transformChanged:true};
  assert.equal(evaluate(row).upperNanoseconds,'250000000');
  for(const delta of [{observedAfter:'1250000001'},{uncertaintyNanoseconds:'1'},{clock:'peer-uptime'},
    {actor:'foreign'},{session:'old'},{revision:6},{drawn:false},{visible:false},{insideCamera:false},
    {drawFrame:0},{imageBound:false},{eventAcknowledged:false},{transformChanged:false}])assert.throws(()=>evaluate({...row,...delta}));
});
test('stopped agreement includes exact distance and time equality',()=>{
  const evaluate=require('./evidence.cjs').stopped;
  const row={clock:'coordinator-hrtime',actor:'a',expectedActor:'a',session:'s',expectedSession:'s',revision:2,minimumRevision:2,
    positionSubunits:50,hostPositionSubunits:0,releaseBefore:'1000000000',releaseAfter:'1000000000',observedBefore:'1500000000',observedAfter:'1500000000',uncertaintyNanoseconds:'0',
    observerDraw:{sha256:'fixture'},hostDraw:{sha256:'fixture'},releaseAcknowledgment:{nativeDrawFrame:1},authorityDrawFrame:1};
  assert.equal(evaluate(row).positionSubunits,50);
  for(const delta of [{positionSubunits:51},{observedAfter:'1500000001'},{uncertaintyNanoseconds:'1'},
    {observedBefore:'999999999'},{observedAfter:'999999999'},{uncertaintyNanoseconds:'1000001'},
    {observerDraw:null},{hostDraw:null},{releaseAcknowledgment:null},
    {uncertaintyNanoseconds:'-1'},{actor:'other'},{session:'old'},{revision:1},{clock:'peer-uptime'},{positionSubunits:NaN}])assert.throws(()=>evaluate({...row,...delta}));
});

test('reversal requires later leftward drawn motion while still ahead of an older baseline',()=>{
  const evaluate=require('./evidence.cjs').timing;
  const row={clock:'coordinator-hrtime',action:'reversal',actor:'a',session:'s',revision:7,
    expectedActor:'a',expectedSession:'s',minimumRevision:7,deliveryBefore:'1000000000',deliveryAfter:'1001000000',
    observedBefore:'1100000000',observedAfter:'1120000000',uncertaintyNanoseconds:'1000000',drawFrame:12,baselineDrawFrame:8,
    drawn:true,visible:true,insideCamera:true,imageBound:true,provenance:'physical-SendInput',eventAcknowledged:true,transformChanged:true,
    reversalWitness:{beforeFrame:9,afterFrame:10,beforeX:6,afterX:5,afterFacing:-1}};
  assert.equal(evaluate(row).upperNanoseconds,'121000000');
  for(const delta of [{afterX:6},{afterX:7},{afterFacing:1},{beforeFrame:7},{afterFrame:9},{afterFrame:13}])
    assert.throws(()=>evaluate({...row,reversalWitness:{...row.reversalWitness,...delta}}));
  assert.throws(()=>evaluate({...row,reversalWitness:null}));
});
test('draw stream polling retains partial Unicode and refuses truncation',()=>{
  const path=require('node:path'),crypto=require('node:crypto'),{lines}=require('./rendered.cjs');
  const directory=path.join(controls,'controls',crypto.randomUUID());fs.mkdirSync(directory,{recursive:true});
  const file=path.join(directory,'draws.ndjson'),row=Buffer.from(JSON.stringify({frame:1,label:'雪'})+'\n');
  const split=row.indexOf(Buffer.from('雪'))+1;fs.writeFileSync(file,row.subarray(0,split));assert.deepEqual(lines(file),[]);
  fs.appendFileSync(file,row.subarray(split));assert.deepEqual(lines(file),[{frame:1,label:'雪'}]);
  fs.appendFileSync(file,JSON.stringify({frame:2})+'\n');assert.equal(lines(file).at(-1).frame,2);
  fs.writeFileSync(file,'');assert.throws(()=>lines(file),/TRUNCATED/);
});

test('the full reconciler binds every stopped scalar to current release, authority and peer draws',()=>{
  require('./stopped-fixture.cjs').run();
});

test('capture waits for atomic JSON publication and brackets the complete PNG read',async()=>{
  const path=require('node:path'),crypto=require('node:crypto'),r=require('./rendered.cjs');
  const directory=path.join(controls,'controls',crypto.randomUUID());fs.mkdirSync(directory,{recursive:true});
  const context={directory,nonce:'atomic-control'},peer={directory,pid:123,role:'control',exit:null};
  const pending=r.capture(context,peer,'atomic');
  const request=JSON.parse(fs.readFileSync(path.join(directory,'draw.request'),'utf8')),file=path.join(directory,request.checkpoint+'.json');
  fs.writeFileSync(file+'.tmp','{');await new Promise(resolve=>setTimeout(resolve,100));assert.equal(fs.existsSync(file),false);
  fs.writeFileSync(path.join(directory,request.checkpoint+'.png'),require('./runner-fixture.cjs').png());
  const state={nonce:context.nonce,pid:123,role:'control',profile:'profile',playerId:'profile',bindingId:'local',drawn:true,drawFrame:1,
    checkpoint:request.checkpoint,width:1280,height:720,cameraX:0,
    snapshot:{SessionId:'session',Revision:7,Participants:[{Ready:true,BindingId:'local',Actor:{ActorId:'actor',PlayerId:'profile',PositionSubunits:0,Facing:1}}]},
    actors:[{actorId:'actor',playerId:'profile',bindingId:'local',x:0,y:540,screenX:640,facing:1,visible:true,insideCamera:true}]};
  fs.writeFileSync(file+'.tmp',JSON.stringify(state));
  fs.renameSync(file+'.tmp',file);const published=r.now(),capture=await pending;
  assert.ok(BigInt(capture.after)>=published);assert.equal(capture.state.drawFrame,1);
  assert.equal(capture.image.subject.minimumRevision,7);
  require('./evidence.cjs').image(context,capture.image,{pid:123,role:'control',profile:'profile',session:'session',actor:'actor',binding:'local',minimumRevision:999});
});
