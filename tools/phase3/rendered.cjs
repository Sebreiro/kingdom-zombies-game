'use strict';
const fs=require('node:fs'),path=require('node:path'),{spawn}=require('node:child_process');
const {StringDecoder}=require('node:string_decoder');
const e=require('./evidence.cjs');
const {command,killTree}=require('../phase2/rendered.cjs');
const delay=ms=>new Promise(resolve=>setTimeout(resolve,ms));
const now=()=>process.hrtime.bigint();
const streams=new Map();
function lines(file){
  if(!fs.existsSync(file))return [];
  const stat=fs.statSync(file);let stream=streams.get(file);
  if(!stream){stream={offset:0,inode:stat.ino,decoder:new StringDecoder('utf8'),carry:'',rows:[]};streams.set(file,stream);}
  e.requireTrue(stat.ino===stream.inode&&stat.size>=stream.offset,'DRAW_STREAM_REPLACED_OR_TRUNCATED');
  const size=stat.size-stream.offset;e.requireTrue(size<=16*1024*1024,'DRAW_STREAM_BACKLOG');if(!size)return stream.rows;
  const descriptor=fs.openSync(file,'r'),buffer=Buffer.alloc(size);let read;
  try{read=fs.readSync(descriptor,buffer,0,size,stream.offset);}finally{fs.closeSync(descriptor);}
  stream.offset+=read;stream.carry+=stream.decoder.write(buffer.subarray(0,read));
  const complete=stream.carry.split('\n');stream.carry=complete.pop();
  for(const line of complete.filter(Boolean))stream.rows.push(JSON.parse(line));
  if(stream.rows.length>256)stream.rows.splice(0,stream.rows.length-256);return stream.rows;
}
function draws(peer){return lines(path.join(peer.directory,'draws.ndjson'));}
function actor(draw,id){return draw?.actors?.find(row=>row.actorId===id);}
async function until(peers,predicate,label,milliseconds=12000){
  const deadline=now()+BigInt(milliseconds)*1000000n;
  while(now()<deadline){for(const peer of peers)e.requireTrue(!peer.error&&!peer.exit,'RENDERED_PROCESS_LOST '+peer.role+' '+peer.output?.slice(-1000));
    const result=predicate();if(result)return result;await delay(2);}
  throw new Error('RENDERED_DEADLINE '+label);
}
function driver(context,peers){
  e.requireTrue(process.platform==='win32','WINDOWS_ONLY');
  const child=spawn('powershell.exe',['-NoProfile','-ExecutionPolicy','Bypass','-File',path.join(context.root,'tools/phase3/windows-input.ps1'),
    '-OwnedPids',peers.map(row=>row.pid).join(','),'-Nonce',context.nonce],{cwd:context.root,env:context.env,windowsHide:true,stdio:['pipe','pipe','pipe']});
  let buffer='',stderr='',sequence=0,ended=false,error,closed=false;const pending=new Map(),records=[];
  let readyResolve;const ready=new Promise(resolve=>readyResolve=resolve);
  child.stdout.on('data',bytes=>{buffer+=bytes;let boundary;while((boundary=buffer.indexOf('\n'))>=0){const line=buffer.slice(0,boundary);buffer=buffer.slice(boundary+1);try{
    const row=JSON.parse(line);if(row.ready){readyResolve(row);continue;}const request=pending.get(row.sequence);if(request){pending.delete(row.sequence);request.resolve({...row,observedAfter:String(now())});}
  }catch(failure){error=failure;}}});
  child.stderr.on('data',bytes=>stderr+=bytes);child.once('error',failure=>error=failure);child.once('exit',()=>ended=true);
  async function deliver(peer,request){
    const start=now(),index=++sequence;const ackPromise=new Promise(resolve=>pending.set(index,{resolve}));
    child.stdin.write(JSON.stringify({nonce:context.nonce,sequence:index,pid:peer.pid,...request})+'\n');
    const row=await Promise.race([ackPromise,delay(8000).then(()=>{throw new Error('DRIVER_ACK_TIMEOUT '+stderr);})]);
    e.requireTrue(!error&&!ended&&row.ok&&row.nonce===context.nonce&&row.pid===peer.pid&&row.driverPid===child.pid,'DRIVER_ACK '+JSON.stringify(row));
    const record={...row,deliveryBefore:String(start),deliveryAfter:String(now()),clock:'coordinator-hrtime'};records.push(record);return record;
  }
  async function send(peer,request){
    return request.method==='physical-SendInput'?require('./foreground.cjs').run(()=>deliver(peer,request)):deliver(peer,request);
  }
  async function start(){const row=await Promise.race([ready,delay(10000).then(()=>{throw new Error('DRIVER_START_TIMEOUT '+stderr);})]);
    e.requireTrue(row.nonce===context.nonce&&row.pid===child.pid,'DRIVER_READY');e.exact(peers.map(row=>row.pid),row.owned,'driver owned PIDs');return row;}
  async function close(){
    if(closed)return;closed=true;
    if(!ended){child.stdin.end(JSON.stringify({nonce:context.nonce,sequence:++sequence,action:'close'})+'\n');
      await Promise.race([new Promise(resolve=>child.once('exit',resolve)),delay(3000)]);if(!ended){killTree(child);await Promise.race([new Promise(resolve=>child.once('exit',resolve)),delay(1000)]);}}
    fs.writeFileSync(path.join(context.directory,'native-input-'+child.pid+'.json'),JSON.stringify({pid:child.pid,nonce:context.nonce,
      ownedPids:peers.map(peer=>peer.pid),scriptSha256:e.hash(fs.readFileSync(path.join(context.root,'tools/phase3/windows-input.ps1'))),records,stderr,ended},null,2),{flag:'wx'});
    e.requireTrue(ended&&!stderr&&!error,'DRIVER_CLEANUP '+stderr);
  }
  return {start,send,close,child,records,file:path.join(context.directory,'native-input-'+child.pid+'.json')};
}
async function capture(context,peer,checkpoint){
  peer.captureSequence=(peer.captureSequence??0)+1;
  const captureId='draw-'+checkpoint+'-'+peer.captureSequence;
  const request=path.join(peer.directory,'draw.request');e.requireTrue(!fs.existsSync(request),'DRAW_ALREADY_PENDING');
  const before=now();fs.writeFileSync(request+'.tmp',JSON.stringify({nonce:context.nonce,checkpoint:captureId}),{flag:'wx'});fs.renameSync(request+'.tmp',request);
  const stateFile=path.join(peer.directory,captureId+'.json');
  await until([peer],()=>fs.existsSync(stateFile),'capture '+peer.role+'/'+checkpoint);
  const state=e.read(stateFile),file=path.join(peer.directory,captureId+'.png');
  const decoded=require('./png.cjs').validate(file);
  e.requireTrue(state.nonce===context.nonce&&state.pid===peer.pid&&state.drawn===true&&state.drawFrame>0,'DRAW_IDENTITY');
  const image={checkpoint,captureId,path:path.relative(context.directory,file),sha256:e.hash(fs.readFileSync(file)),
    statePath:path.relative(context.directory,stateFile),stateSha256:e.hash(fs.readFileSync(stateFile)),...decoded};
  const own=state.snapshot?.Participants.find(participant=>participant.Ready&&participant.BindingId===state.bindingId);
  if(own)image.subject={session:state.snapshot.SessionId,actor:own.Actor.ActorId,binding:own.BindingId,minimumRevision:state.snapshot.Revision};
  return {state,image,before:String(before),after:String(now())};
}
async function nativeKey(context,input,peer,key,pressed,method='physical-SendInput'){
  const previous=draws(peer).at(-1)?.input.inputEvents??0;
  const ack=await input.send(peer,{action:'key',method,key,pressed});
  const native=await until([peer],()=>{const row=draws(peer).at(-1);const event=row?.input.lastInput;
    return row?.input.inputEvents>previous&&event?.key===({13:4194309,27:4194305,9:4194306,37:4194319,39:4194321}[key]??key)&&event.pressed===pressed?row:null;},'native event '+peer.role+'/'+key+'/'+pressed);
  Object.assign(ack,{nativeInputEvents:native.input.inputEvents,nativeDrawFrame:native.drawFrame,nativeAcknowledgedAfter:String(now())});return ack;
}
async function transition(context,input,peers,origin,key,pressed,action){
  const initial=draws(origin).at(-1),own=initial.snapshot.Participants.find(row=>row.BindingId===initial.bindingId).Actor;
  const host=peers.find(peer=>peer.role==='host'),acceptedBefore=draws(host).at(-1).input.acceptedIntents;
  const baselines=new Map(peers.map(peer=>[peer.role,draws(peer).at(-1)]));
  const previous=new Map();let ack;
  if(action==='focus-stop'){
    ack=await input.send(origin,{action:'focus-sink',method:'physical-SendInput'});
    await until([origin],()=>draws(origin).at(-1)?.input.focusLosses>initial.input.focusLosses,'real focus loss');
    Object.assign(ack,{nativeFocusLosses:draws(origin).at(-1).input.focusLosses,nativeAcknowledgedAfter:String(now())});
  }else if(action==='reversal'){
    const release=await nativeKey(context,input,origin,68,false);ack=await nativeKey(context,input,origin,key,pressed);ack.deliveryBefore=release.deliveryBefore;
  }else ack=await nativeKey(context,input,origin,key,pressed);
  const direction=action==='start'?1:action==='reversal'?-1:0;
  const accepted=await until([host],()=>{const draw=draws(host).at(-1),intent=draw?.input.lastAcceptedIntent;
    return draw?.input.acceptedIntents>acceptedBefore&&intent?.ActorId===own.ActorId&&intent.SessionId===own.SessionId&&intent.Direction===direction?draw:null;},'host accepted exact '+action,1000);
  const results=await Promise.all(peers.filter(peer=>peer!==origin).map(async observer=>{
    const baseline=actor(baselines.get(observer.role),own.ActorId);
    e.requireTrue(baseline?.insideCamera&&baseline.visible,'TIMING_INITIAL_CAMERA');
    await until([observer],()=>{
      const draw=draws(observer).at(-1),current=actor(draw,own.ActorId);if(!current)return false;
      const last=previous.get(observer.role);previous.set(observer.role,{x:current.x,frame:draw.drawFrame});
      if(draw.snapshot?.Revision<=accepted.input.lastAcceptedRevision)return false;
      if(action==='stop'||action==='focus-stop')return last&&draw.drawFrame>last.frame&&current.x===last.x;
      return action==='reversal'?current.facing!==baseline.facing&&current.x<baseline.x:current.x>baseline.x;
    },'visible transition '+origin.role+'/'+observer.role+'/'+action,1000);
    const picture=await capture(context,observer,'timing-'+origin.role+'-'+action);
    const current=actor(picture.state,own.ActorId);
    const sample={clock:'coordinator-hrtime',origin:origin.role,observer:observer.role,action,actor:own.ActorId,
      expectedActor:own.ActorId,session:own.SessionId,expectedSession:own.SessionId,minimumRevision:accepted.input.lastAcceptedRevision+1,
      revision:picture.state.snapshot.Revision,deliveryBefore:ack.deliveryBefore,deliveryAfter:ack.deliveryAfter,
      observedBefore:picture.before,observedAfter:picture.after,uncertaintyNanoseconds:'1000000',
      uncertaintyPolicy:'External before-delivery to after-complete-draw-PNG-read brackets include all polling, driver, draw, capture and scheduler delays; no peer clocks subtracted.',
      drawFrame:picture.state.drawFrame,drawn:picture.state.drawn,visible:current?.visible,insideCamera:current?.insideCamera,
      baselineDrawFrame:baselines.get(observer.role).drawFrame,
      imageBound:true,provenance:ack.method,eventAcknowledged:true,transformChanged:action==='stop'||action==='focus-stop'?true:current?.x!==baseline.x,
      image:picture.image,inputAcknowledgment:ack,acceptedIntent:accepted.input.lastAcceptedIntent,
      acceptedRevision:accepted.input.lastAcceptedRevision,authorityDrawFrame:accepted.drawFrame};
    fs.writeFileSync(path.join(context.directory,'timing-'+origin.role+'-'+observer.role+'-'+action+'-'+sample.drawFrame+'.json'),
      JSON.stringify(sample,null,2),{flag:'wx'});
    e.timing(sample);return sample;
  }));return {timings:results,ack,actor:{...own,minimumRevision:accepted.input.lastAcceptedRevision+1,
    authorityDrawFrame:accepted.drawFrame}};
}
async function stopAgreement(context,peers,origin,release,own){
  const rows=[];
  const host=peers.find(peer=>peer.role==='host');
  const position=draw=>draw?.snapshot?.Participants.find(row=>row.Ready&&row.Actor.ActorId===own.ActorId)?.Actor.PositionSubunits;
  await until(peers,()=>peers.every(peer=>{const draw=draws(peer).at(-1),authority=draws(host).at(-1);
    return draw?.snapshot?.SessionId===own.SessionId&&draw.snapshot.Revision>=own.minimumRevision&&
      Number.isSafeInteger(position(draw))&&Math.abs(position(draw)-position(authority))<=50;}),'current stopped actor at every peer',500);
  function sample(peer){
    const observedBefore=String(now()),draw=draws(peer).at(-1),observedAfter=String(now());
    const file=path.join(peer.directory,'stopped-'+origin.role+'-'+peer.role+'-'+(peer.stopSequence=(peer.stopSequence??0)+1)+'.json');
    fs.writeFileSync(file,JSON.stringify({draw,observedBefore,observedAfter}),{flag:'wx'});
    return {draw,observedBefore,observedAfter,reference:{path:path.relative(context.directory,file),sha256:e.hash(fs.readFileSync(file))}};
  }
  for(const observer of peers){
    const authority=sample(host),current=sample(observer),draw=current.draw;
    const row={clock:'coordinator-hrtime',origin:origin.role,observer:observer.role,actor:own.ActorId,expectedActor:own.ActorId,
      session:draw.snapshot.SessionId,expectedSession:own.SessionId,revision:draw.snapshot.Revision,minimumRevision:own.minimumRevision??0,
      positionSubunits:position(draw),hostPositionSubunits:position(authority.draw),releaseBefore:release.deliveryBefore,releaseAfter:release.deliveryAfter,
      observedBefore:current.observedBefore,observedAfter:current.observedAfter,uncertaintyNanoseconds:'1000000',
      observerDraw:current.reference,hostDraw:authority.reference,authorityDrawFrame:own.authorityDrawFrame,
      releaseAcknowledgment:release};e.stopped(row);rows.push(row);
  }return rows;
}
async function timingControl(context,item){
  const result=await command(process.execPath,['--test','--test-reporter=tap',path.join(context.root,'tools/phase3/timing.test.cjs')],{cwd:context.root,env:context.env},60000);
  const output=item.id+'.tap';fs.writeFileSync(path.join(context.directory,output),result.stdout,{flag:'wx'});
  const control=item.id+'.control.json';fs.writeFileSync(path.join(context.directory,control),JSON.stringify({status:'pass',subject:'inclusive conservative draw timing rejection controls; no rendered or physical-input credit'}),{flag:'wx'});
  return {id:item.id,status:'pass',nonce:context.nonce,input:context.input,tool:context.tool,platform:context.platform,revision:context.revision,osFingerprint:context.osFingerprint,
    artifacts:[output,control].map(file=>({path:file,sha256:e.hash(fs.readFileSync(path.join(context.directory,file)))}))};
}
module.exports={now,lines,draws,actor,until,driver,capture,nativeKey,transition,stopAgreement,timingControl};
