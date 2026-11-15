'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),dgram=require('node:dgram');
const {spawn}=require('node:child_process');
const {acquireLock}=require('../phase2/check.cjs');
const delay=ms=>new Promise(resolve=>setTimeout(resolve,ms));
function requireTrue(value,message){if(!value)throw new Error(message);}
async function allocatePort(){
  const socket=dgram.createSocket('udp4');
  try {await new Promise((resolve,reject)=>{socket.once('error',reject);socket.bind(0,'127.0.0.1',resolve);});return socket.address().port;}
  finally {socket.close();}
}
function frames(peer){
  const file=path.join(peer.directory,'frames.ndjson');if(!fs.existsSync(file))return [];
  const text=fs.readFileSync(file,'utf8'),lines=text.split('\n');lines.pop();
  return lines.filter(Boolean).map(line=>JSON.parse(line));
}
function launch(context,role,port){
  const directory=path.join(context.directory,role);fs.mkdirSync(directory);
  const pins=JSON.parse(fs.readFileSync(path.join(context.root,'tools/phase1/toolchain.json'),'utf8'));
  const archive=pins.archives[context.platform];requireTrue(archive,'ENGINE_PLATFORM');
  const consoleEngine=path.join(context.root,'.phase1/tools',archive.directory,archive.executable);
  requireTrue(fs.existsSync(consoleEngine),'ENGINE_MISSING');
  const digest=crypto.createHash('sha256').update(fs.readFileSync(consoleEngine)).digest('hex');
  if(archive.executableSha256)requireTrue(digest===archive.executableSha256,'ENGINE_HASH');
  const engine=process.platform==='win32'?consoleEngine.replace('_console.exe','.exe'):consoleEngine;
  if(process.platform==='win32')requireTrue(crypto.createHash('sha256').update(fs.readFileSync(engine)).digest('hex')===
    '45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab','ENGINE_NATIVE_HASH');
  const profile=crypto.randomUUID();
  const child=spawn(engine,['--headless','--path',path.join(context.root,'src/godot-game')],{cwd:context.root,windowsHide:true,
    env:{...context.env,PHASE3_NONCE:context.nonce,PHASE3_OUTPUT:directory,PHASE3_PROFILE:profile,PHASE3_PORT:String(port)},stdio:['ignore','pipe','pipe']});
  const peer={role,directory,profile,child,pid:child.pid,sequence:0,output:'',exit:null,error:null};
  child.stdout.on('data',bytes=>peer.output+=bytes);child.stderr.on('data',bytes=>peer.output+=bytes);
  child.once('error',error=>peer.error=error);child.once('exit',(code,signal)=>peer.exit={code,signal});
  fs.writeFileSync(path.join(directory,'launch.json'),JSON.stringify({role,profile,pid:child.pid,port,headless:true,mainScene:'res://foundation/Foundation.tscn',engine,engineHash:digest,nonce:context.nonce},null,2));
  return peer;
}
async function waitFor(peers,deadline,predicate,message){
  while(Date.now()<deadline){for(const peer of peers){if(peer.error)throw peer.error;if(peer.exit)throw new Error('CHILD_EXIT '+peer.role+' '+JSON.stringify(peer.exit)+' '+peer.output.slice(-1500));}
    const result=predicate();if(result)return result;await delay(25);}
  throw new Error('SUBJECT_TIMEOUT '+message);
}
async function input(peer,context,deadline,request){
  const sequence=++peer.sequence,file=path.join(peer.directory,'input.request');
  fs.writeFileSync(file+'.tmp',JSON.stringify({nonce:context.nonce,sequence,...request}));fs.renameSync(file+'.tmp',file);
  await waitFor([peer],deadline,()=>{const ack=path.join(peer.directory,'input.ack');return fs.existsSync(ack)&&fs.readFileSync(ack,'utf8')===String(sequence);},'input ack '+peer.role);
}
async function key(peer,context,deadline,code,pressed){await input(peer,context,deadline,{action:'key',key:code,pressed});}
async function activate(peer,context,deadline,control){
  await input(peer,context,deadline,{action:'focus',control});await key(peer,context,deadline,4194309,true);await key(peer,context,deadline,4194309,false);
}
function actor(row,player){return row?.snapshot?.Participants.find(p=>p.Actor.PlayerId===player)?.Actor;}
function coherent(peers,context){
  const host=frames(peers[0]),guest=frames(peers[1]);
  const byRevision=new Map(host.filter(row=>row.snapshot).map(row=>[row.snapshot.Revision,row]));
  for(const current of guest.slice().reverse()){
    const match=byRevision.get(current.snapshot?.Revision);
    if(!match||!current.ready||!match.ready||current.nativeActors!==2||match.nativeActors!==2)continue;
    if(JSON.stringify(current.snapshot)!==JSON.stringify(match.snapshot))continue;
    if(!actor(current,peers[1].profile)||actor(current,peers[1].profile).PositionSubunits<=-7700)continue;
    for(const row of [current,match])requireTrue(row.nonce===context.nonce&&row.provenance==='synthetic-native-ParseInputEvent','FRAME_IDENTITY');
    return {host:match,guest:current};
  }
  return null;
}
async function stop(peer,context,deadline){
  await key(peer,context,deadline,4194305,true);await key(peer,context,deadline,4194305,false);
  await input(peer,context,deadline,{action:'focus',control:'Menu/Quit'});
  // A press can synchronously exit before writing the driver ack; process exit is the expected acknowledgment.
  const file=path.join(peer.directory,'input.request');fs.writeFileSync(file,JSON.stringify({nonce:context.nonce,sequence:++peer.sequence,action:'key',key:4194309,pressed:true}));
}
async function cleanup(peer){
  if(!peer.exit){peer.child.kill();await Promise.race([new Promise(resolve=>peer.child.once('exit',resolve)),delay(3000)]);}
  fs.writeFileSync(path.join(peer.directory,'console.log'),peer.output);
  requireTrue(peer.exit!==null,'OWNED_PROCESS_CLEANUP '+peer.role);
}
async function tracer(context,item){
  const started=Date.now(),deadline=started+60000,peers=[];let unlock,result,error;
  try{
    unlock=acquireLock(context.root,context.nonce);const port=await allocatePort();
    const host=launch(context,'host',port);peers.push(host);
    await waitFor(peers,deadline,()=>frames(host).length>1,'host main scene');
    await activate(host,context,deadline,'Menu/Host');
    await waitFor(peers,deadline,()=>frames(host).at(-1)?.ready,'host ready');
    const guest=launch(context,'guest',port);peers.push(guest);
    await waitFor(peers,deadline,()=>frames(guest).length>1,'guest main scene');
    await activate(guest,context,deadline,'Menu/Join');
    await input(guest,context,deadline,{action:'address',address:'127.0.0.1:'+port});
    await activate(guest,context,deadline,'Browser/JoinAddress');
    const ready=await waitFor(peers,deadline,()=>{const h=frames(host).at(-1),g=frames(guest).at(-1);return h?.snapshot?.Participants.length===2&&g?.ready&&g.nativeActors===2?g:null;},'guest installed and ready');
    requireTrue(ready.installedRevision>=0,'BASELINE_INSTALL_REQUIRED');
    const install=ready.events.findIndex(value=>value.startsWith('baseline-installed:')),
      ack=ready.events.findIndex(value=>value.startsWith('baseline-acknowledged:'));
    requireTrue(install>=0&&ack>install,'BASELINE_ACK_ORDER');
    requireTrue(new Set(ready.snapshot.Participants.map(p=>p.Actor.PlayerId)).size===2&&new Set(ready.snapshot.Participants.map(p=>p.Actor.ActorId)).size===2&&new Set(ready.snapshot.Participants.map(p=>p.BindingId)).size===2,'DISTINCT_BINDINGS');
    requireTrue(host.pid!==guest.pid&&ready.pid===guest.pid,'REAL_PROCESS_IDENTITY');
    const before=ready.inputEvents;await key(guest,context,deadline,68,true);
    const agreement=await waitFor(peers,deadline,()=>coherent(peers,context),'guest movement at both peers');
    const holds=frames(guest).filter(row=>row.direction===1&&row.polledRight===true&&row.inputEvents>before);
    requireTrue(holds.length>=3,'SYNTHETIC_HOLD_MUST_SURVIVE_NATIVE_POLL');
    await key(guest,context,deadline,68,false);
    await waitFor(peers,deadline,()=>{const row=frames(guest).at(-1);return row?.direction===0&&row.polledRight===false;},'native release');
    requireTrue(actor(agreement.host,host.profile).PositionSubunits===-8000,'HOST_UNAUTHORIZED_MOVEMENT');
    result={id:item.id,status:'pass',nonce:context.nonce,input:context.input,tool:context.tool,platform:context.platform,
      port,peers:peers.map(peer=>({role:peer.role,pid:peer.pid,profile:peer.profile})),agreement,
      provenance:'synthetic-native-ParseInputEvent',noRenderedOrPhysicalInputCredit:true};
    fs.writeFileSync(path.join(context.directory,'network.tracer.agreement.json'),JSON.stringify(result,null,2),{flag:'wx'});
    await stop(guest,context,deadline);await stop(host,context,deadline);
    while(peers.some(peer=>!peer.exit)&&Date.now()<deadline)await delay(25);
    for(const peer of peers)requireTrue(peer.exit?.code===0&&!peer.exit.signal,'CLEAN_EXIT '+peer.role+' '+JSON.stringify(peer.exit));
  }catch(failure){error=failure;}
  finally{for(const peer of peers){try{await cleanup(peer);}catch(failure){error??=failure;}}if(unlock)unlock();}
  if(error)throw error;
  requireTrue(Date.now()-started<=60000,'SUBJECT_DEADLINE');
  return {...result,durationMilliseconds:Date.now()-started,cleanup:'two owned children exited zero'};
}
module.exports={tracer,frames,coherent};
