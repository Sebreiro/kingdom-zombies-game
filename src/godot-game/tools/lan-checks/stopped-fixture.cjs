'use strict';
// Counterfeit artifacts exercise the full reconciler's rejection paths, never production acceptance.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),assert=require('node:assert/strict');
const e=require('./evidence.cjs');
function run(){
  const root=path.resolve(__dirname,'../../../..'),directory=path.join(root,'.phase3/controls',crypto.randomUUID());fs.mkdirSync(directory,{recursive:true});
  const fixture=require('./runner-fixture.cjs').fixture(directory),context={...fixture.context,root},result=fixture.results[0];
  result.id='rendered.timing';result.timings=[];result.stopped=[];
  const roles=result.peers.map(peer=>peer.role),item={id:result.id,group:'rendered',implementation:'native-rendered',needsGameWindow:true,roles,imagesByRole:{}};
  const records=[],histories=new Map(roles.map(role=>[role,[]])),host=result.peers[0];let frame=1,sequence=0;
  const ref=file=>({path:file,sha256:e.hash(fs.readFileSync(path.join(directory,file)))});
  const write=(file,value)=>{fs.writeFileSync(path.join(directory,file),JSON.stringify(value),{flag:'wx'});return ref(file);};
  function draw(peer,input,revision){
    const state=e.read(path.join(directory,peer.role+'.json'));
    const value={...state,drawFrame:frame++,snapshot:{...state.snapshot,Revision:revision},input};histories.get(peer.role).push(value);return value;
  }
  for(const [originIndex,origin] of result.peers.entries()){
    const base=BigInt(1000000000+originIndex*2000000000);let stop;
    for(const [index,action] of ['start','reversal','stop','focus-stop'].entries()){
      const before=base+BigInt(index*300000000),ack={pid:origin.pid,driverPid:9000,sequence:++sequence,method:'physical-SendInput',action:action==='focus-stop'?'focus-sink':'key',key:65,pressed:action!=='stop',
        deliveryBefore:String(before),deliveryAfter:String(before+1000000n),nativeAcknowledgedAfter:String(before+2000000n),nativeInputEvents:sequence,nativeFocusLosses:action==='focus-stop'?1:undefined};
      const native=draw(origin,{lastInput:{key:65,pressed:ack.pressed},inputEvents:sequence,direction:action==='stop'?0:1},7);ack.nativeDrawFrame=native.drawFrame;records.push(ack);
      const intent={ActorId:origin.actor,SessionId:origin.session,PlayerId:origin.profile,BindingId:origin.binding,Direction:action==='start'?1:action==='reversal'?-1:0,Sequence:sequence};
      const authority=draw(host,{lastAcceptedIntent:intent,lastAcceptedRevision:6},7);
      for(const observer of result.peers.filter(peer=>peer!==origin)){
        let reversalWitness;
        if(action==='reversal'){
          const previous=draw(observer,{},7),actor=previous.actors.find(row=>row.actorId===origin.actor);
          actor.x=2;actor.screenX=642;previous.snapshot.Participants.find(row=>row.Actor.ActorId===origin.actor).Actor.PositionSubunits=125;
          reversalWitness={beforeFrame:previous.drawFrame,beforeX:2,afterX:0,afterFacing:-1};
        }
        const state=draw(observer,{},7),file=observer.role+'-'+origin.role+'-'+action+'.json';state.checkpoint=file;
        if(reversalWitness){
          reversalWitness.afterFrame=state.drawFrame;state.actors.find(row=>row.actorId===origin.actor).facing=-1;
          state.snapshot.Participants.find(row=>row.Actor.ActorId===origin.actor).Actor.Facing=-1;
        }
        fs.copyFileSync(path.join(directory,'image.png'),path.join(directory,file+'.png'));
        const stateReference=write(file,state),picture={...observer.images[0],...ref(file+'.png'),checkpoint:'timing-'+origin.role+'-'+action,captureId:file,statePath:stateReference.path,stateSha256:stateReference.sha256};observer.images.push(picture);
        result.timings.push({clock:'coordinator-hrtime',origin:origin.role,observer:observer.role,action,actor:origin.actor,expectedActor:origin.actor,session:origin.session,expectedSession:origin.session,
          revision:7,minimumRevision:7,deliveryBefore:ack.deliveryBefore,deliveryAfter:ack.deliveryAfter,observedBefore:String(before+3000000n),observedAfter:String(before+4000000n),uncertaintyNanoseconds:'1000000',
          drawFrame:state.drawFrame,baselineDrawFrame:1,reversalWitness,drawn:true,visible:true,insideCamera:true,imageBound:true,provenance:ack.method,eventAcknowledged:true,transformChanged:true,
          image:picture,inputAcknowledgment:ack,acceptedIntent:intent,acceptedRevision:6,authorityDrawFrame:authority.drawFrame});
      }
      if(action==='stop')stop={ack,authority,before};
    }
    for(const observer of result.peers){
      const authority=draw(host,{},7),current=draw(observer,{},7),before=String(stop.before+10000000n),after=String(stop.before+11000000n);
      const observerDraw=write('stop-'+origin.role+'-'+observer.role+'.json',{draw:current,observedBefore:before,observedAfter:after});
      const hostDraw=write('stop-host-'+origin.role+'-'+observer.role+'.json',{draw:authority,observedBefore:before,observedAfter:after});
      result.stopped.push({clock:'coordinator-hrtime',origin:origin.role,observer:observer.role,actor:origin.actor,expectedActor:origin.actor,session:origin.session,expectedSession:origin.session,
        revision:7,minimumRevision:7,positionSubunits:0,hostPositionSubunits:0,releaseBefore:stop.ack.deliveryBefore,releaseAfter:stop.ack.deliveryAfter,observedBefore:before,observedAfter:after,uncertaintyNanoseconds:'1000000',
        observerDraw,hostDraw,authorityDrawFrame:stop.authority.drawFrame,releaseAcknowledgment:stop.ack});
    }
  }
  for(const peer of result.peers){peer.draws=write(peer.role+'.draws.json',histories.get(peer.role));
    const file=peer.role+'.ndjson';fs.writeFileSync(path.join(directory,file),histories.get(peer.role).map(JSON.stringify).join('\n')+'\n');peer.draws=ref(file);
    item.imagesByRole[peer.role]=peer.images.map(picture=>picture.checkpoint);}
  result.nativeDrivers=[write('driver.json',{nonce:context.nonce,pid:9000,ended:true,stderr:'',records,scriptSha256:e.hash(fs.readFileSync(path.join(root,'src/godot-game/tools/lan-checks/windows-input.ps1')))})];
  result.ownedProcesses=result.peers.map(peer=>({pid:peer.pid,role:peer.role,terminal:peer.terminal}));
  e.reconcile([item],[result],context);
  const forgedReversal=structuredClone(result);forgedReversal.timings.find(row=>row.action==='reversal').reversalWitness.beforeX++;
  assert.throws(()=>e.reconcile([item],[forgedReversal],context),/TIMING_REVERSAL_DRAW_BINDING/);
  function reject(change){const copy=structuredClone(result);change(copy.stopped[0]);assert.throws(()=>e.reconcile([item],[copy],context));}
  for(const change of [row=>{row.actor=row.expectedActor='nonexistent';},row=>{row.session=row.expectedSession='obsolete';},row=>row.positionSubunits=1,
    row=>row.hostPositionSubunits=1,row=>row.revision=6,row=>row.authorityDrawFrame=1,row=>row.observedBefore='1',row=>row.observedAfter='1',
    row=>row.releaseBefore='2000000000',row=>row.uncertaintyNanoseconds='1000001',row=>row.observerDraw=result.stopped[1].observerDraw,
    row=>row.releaseAcknowledgment={...row.releaseAcknowledgment,nativeDrawFrame:1},row=>row.minimumRevision=0])reject(change);
  console.log('PASS full stopped reconciler current-subject/draw/authority/release/stale/clock/uncertainty rejection controls '+directory);
}
module.exports={run};
