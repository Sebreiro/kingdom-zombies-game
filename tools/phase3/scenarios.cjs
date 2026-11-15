'use strict';
// Ordinary menu/native-input scenarios. Importing this module never launches a process or sends input.
const fs=require('node:fs'),path=require('node:path'),e=require('./evidence.cjs'),net=require('./network.cjs'),r=require('./rendered.cjs');
const {acquireLock}=require('../phase2/check.cjs');
const delay=ms=>new Promise(resolve=>setTimeout(resolve,ms));
const latest=peer=>r.draws(peer).at(-1);
function participant(peer,profile=peer.profile){return latest(peer)?.snapshot?.Participants.find(row=>row.Actor.PlayerId===profile);}
async function focus(input,peer){return require('./foreground.cjs').run(()=>input.send(peer,{action:'focus',method:'physical-SendInput'}));}
async function tap(input,peer,key){return require('./foreground.cjs').run(async()=>{await input.send(peer,{action:'key',method:'physical-SendInput',key,pressed:true});await delay(30);return input.send(peer,{action:'key',method:'physical-SendInput',key,pressed:false});});}
async function click(input,peer,control){
  return require('./foreground.cjs').run(async()=>{
  await focus(input,peer);const node=latest(peer)?.controls.find(row=>row.path===control);
  e.requireTrue(node?.visible,'CONTROL_NOT_VISIBLE '+peer.role+'/'+control);
  if(control==='Menu/Quit')require('./terminal.cjs').requestQuit(peer);
  await input.send(peer,{action:'click',method:'physical-SendInput',x:Math.round(node.x),y:Math.round(node.y)});await delay(60);
  });
}
async function text(input,peer,control,value){
  return require('./foreground.cjs').run(async()=>{
  await click(input,peer,control);await input.send(peer,{action:'key',method:'physical-SendInput',key:17,pressed:true});
  await tap(input,peer,65);await input.send(peer,{action:'key',method:'physical-SendInput',key:17,pressed:false});
  await tap(input,peer,8);
  await r.until([peer],()=>latest(peer)?.controls.find(row=>row.path===control)?.text==='','native clear '+control);
  await input.send(peer,{action:'text',method:'physical-SendInput',text:value});
  await r.until([peer],()=>latest(peer)?.controls.find(row=>row.path===control)?.text===value,'native text '+control);
  });
}
async function menu(input,peer){
  if(latest(peer)?.snapshot){await focus(input,peer);await tap(input,peer,27);}
  else if(latest(peer)?.controls.find(row=>row.path==='Browser/Footer/Cancel')?.visible)await click(input,peer,'Browser/Footer/Cancel');
  else if(latest(peer)?.controls.find(row=>row.path==='Browser/Footer/Back')?.visible)await click(input,peer,'Browser/Footer/Back');
  await r.until([peer],()=>!latest(peer)?.snapshot&&latest(peer)?.controls.find(row=>row.path==='Menu/Host')?.visible,'ordinary menu');
}
async function host(input,peer){await menu(input,peer);await click(input,peer,'Menu/Host');await r.until([peer],()=>latest(peer)?.input.ready,'ordinary host ready');}
async function browse(input,peer){await menu(input,peer);await click(input,peer,'Menu/Join');await r.until([peer],()=>latest(peer)?.controls.find(row=>row.path==='Browser/Address')?.visible,'ordinary browser');}
async function join(input,peer,port){
  await browse(input,peer);await text(input,peer,'Browser/Address','127.0.0.1:'+port);await click(input,peer,'Browser/JoinAddress');
  await r.until([peer],()=>latest(peer)?.input.ready,'ordinary manual join '+peer.role);
}
async function roster(peers,count){
  await r.until(peers,()=>peers.every(peer=>latest(peer)?.snapshot?.Participants.filter(row=>row.Ready).length===count&&latest(peer)?.actors.length===count),'complete ready roster '+count);
  const state=latest(peers[0]).snapshot;
  e.requireTrue(new Set(state.Participants.map(row=>row.Actor.ActorId)).size===count&&new Set(state.Participants.map(row=>row.BindingId)).size===count,'ROSTER_DISTINCT');
}
async function resetFour(state){
  for(const peer of state.peers.filter(peer=>peer!==state.host&&!peer.exit))await menu(state.input,peer);
  await menu(state.input,state.host);await delay(300);await host(state.input,state.host);
  for(const peer of state.peers.filter(peer=>peer!==state.host&&!peer.exit))await join(state.input,peer,state.port);
  await roster(state.peers,4);
}
async function create(context,group,roles,launches,retained={}){
  const directory=path.join(context.directory,group);fs.mkdirSync(directory,{recursive:true});
  const local={...context,directory,runRoot:context.directory,rendered:true},port=await net.allocatePort();
  const peers=[];let input;
  try{
    for(const role of roles){const peer=net.launch(local,role,port,retained[role]);peers.push(peer);
      launches.push({group,role:peer.role,pid:peer.pid,profile:peer.profile,port,reason:'Shared '+group+' scenario process set',directory:path.relative(context.directory,peer.directory)});}
    await r.until(peers,()=>peers.every(peer=>latest(peer)?.controls.length),'first completed draws');
    input=r.driver(local,peers);await input.start();for(const peer of peers)peer.driver=input;
    return {context:local,outer:context,group,port,peers,host:peers[0],browser:peers.at(-1),input,extra:[],drivers:[],allDrivers:[input],launches};
  }catch(error){if(input)try{await input.close();}catch{}for(const peer of peers)try{await net.cleanup(peer);}catch{}throw error;}
}
async function extra(state,role,profileRoot=null,reason='specified challenger',ownedPort=null){
  let context=state.context;
  if(fs.existsSync(path.join(context.directory,role))){const directory=path.join(context.directory,'additional-'+state.extra.length);fs.mkdirSync(directory);context={...context,directory};}
  const port=ownedPort??state.port,peer=net.launch(context,role,port,profileRoot);state.extra.push(peer);
  state.launches.push({group:state.group,role,pid:peer.pid,profile:peer.profile,port,reason,directory:path.relative(state.outer.directory,peer.directory)});
  await r.until([peer],()=>latest(peer)?.controls.length,'additional completed draw');
  const input=r.driver(state.context,[peer]);peer.driver=input;state.drivers.push(input);state.allDrivers.push(input);await input.start();return {peer,input};
}
async function take(state,item,peers=state.peers){
  const observations=[];
  for(const role of item.roles){
    const peer=peers.find(row=>row.role===role);e.requireTrue(peer&&!peer.exit,'REQUIRED_PEER_LOST '+role);
    const picture=await r.capture(state.outer,peer,item.id.replace('rendered.','')+'-scene');picture.image.checkpoint='scene';picture.image.screen=item.screen??'game';
    const sample=picture.state,own=sample.snapshot?.Participants.find(row=>row.BindingId===sample.bindingId);
    observations.push({role,pid:peer.pid,profile:peer.profile,session:sample.snapshot?.SessionId??null,
      actor:own?.Actor.ActorId??null,binding:sample.bindingId??null,minimumRevision:sample.snapshot?.Revision??0,
      images:[picture.image],_peer:peer});
  }return observations;
}
async function checkpoint(state,peer,name,screen){
  const picture=await r.capture(state.outer,peer,name);picture.image.checkpoint=name;picture.image.screen=screen;
  const own=picture.state.snapshot?.Participants.find(row=>row.BindingId===picture.state.bindingId);
  if(own)picture.image.subject={session:picture.state.snapshot.SessionId,actor:own.Actor.ActorId,binding:own.BindingId,minimumRevision:picture.state.snapshot.Revision};
  return picture.image;
}
async function g2(state,item){
  const browser=state.browser,input=state.input;
  const images=[];
  if(item.id!=='rendered.browser'&&!latest(state.host)?.snapshot)await host(input,state.host);
  if(item.id==='rendered.entry'){
    await browse(input,browser);await text(input,browser,'Browser/Address','127.0.0.1:'+await net.allocatePort());
    await click(input,browser,'Browser/JoinAddress');images.push(await checkpoint(state,browser,'pending','browser'));
    await click(input,browser,'Browser/Footer/Cancel');images.push(await checkpoint(state,browser,'cancelled','browser'));
    await menu(input,browser);await browse(input,browser);
    e.requireTrue(!latest(browser).snapshot&&latest(browser).input.nativeActors===0,'CANCEL_LEAK');
  }else if(item.id==='rendered.browser'){
    await menu(input,state.host);await browse(input,browser);await click(input,browser,'Browser/Refresh');
    await r.until([browser],()=>latest(browser).input.discoveryRows.length===0,'empty real browser',6000);
    images.push(await checkpoint(state,browser,'empty','browser'));
    const longName='Étoile 雪 🌙 '+ 'Long camp name '.repeat(30);
    await text(input,state.host,'Menu/Name',longName);await tap(input,state.host,13);await host(input,state.host);
    for(let index=2;index<=4;index++){
      const additional=await extra(state,'browser-host'+index,null,'Four real equally named hosts provide Unicode, same-label session identity and scrolling overflow',await net.allocatePort());
      await text(additional.input,additional.peer,'Menu/Name',longName);await tap(additional.input,additional.peer,13);await host(additional.input,additional.peer);
    }
    await browse(input,browser);
    await r.until([browser],()=>latest(browser).input.discoveryRows.length>=4,'actual discovery and overflow');
    await click(input,browser,'Browser/Refresh');
    const rows=await r.until([browser],()=>latest(browser).input.discoveryRows.length?latest(browser).input.discoveryRows:null,'refreshed rows');
    e.requireTrue(new Set(rows.map(row=>row.SessionId)).size===rows.length,'DISCOVERY_DUPLICATE');
    e.requireTrue(rows.length>=4&&new Set(rows.map(row=>row.SessionName)).size<rows.length&&rows.some(row=>row.SessionName.includes('雪')),'EQUAL_UNICODE_DISTINCT_SESSIONS');
    const index=rows.findIndex(row=>row.SessionId===latest(state.host).snapshot.SessionId);e.requireTrue(index>=0,'HOST_ROW');
    images.push(await checkpoint(state,browser,'populated','browser'));
    await click(input,browser,'Browser/Scroll/Rows/Session'+index);images.push(await checkpoint(state,browser,'focused-detail','browser'));
    await click(input,browser,'Browser/Footer/JoinSession');await roster([state.host,browser],2);
    await menu(input,browser);await join(input,browser,state.port);images.push(await checkpoint(state,browser,'manual-ready','game'));
    await menu(input,browser);await browse(input,browser);
  }else if(item.id==='rendered.join-errors'){
    await browse(input,browser);
    for(const [value,name] of [['','invalid-empty'],['bad-address','invalid-malformed'],['127.0.0.1:0','invalid-port']]){
      await text(input,browser,'Browser/Address',value);await click(input,browser,'Browser/JoinAddress');
      e.requireTrue(!latest(browser).snapshot&&!latest(browser).input.ready,'INVALID_ADDRESS_READY');
      images.push(await checkpoint(state,browser,name,'browser'));
    }
    await text(input,browser,'Browser/Address','127.0.0.1:'+await net.allocatePort());
    const started=r.now();await click(input,browser,'Browser/JoinAddress');images.push(await checkpoint(state,browser,'pending','browser'));
    await r.until([browser],()=>latest(browser)?.input.error?.length&&!latest(browser)?.input.ready,'unreachable timeout',33000);
    e.requireTrue(r.now()-started<=33000000000n,'JOIN_TIMEOUT_BOUND');images.push(await checkpoint(state,browser,'timeout','browser'));await menu(input,browser);await browse(input,browser);
  }else if(item.id==='rendered.profiles'){
    await menu(input,browser);const original=latest(browser).input.profile.PlayerId;
    const unicode='Étoile 雪 🌙 '+ 'Long profile '.repeat(35);await text(input,browser,'Menu/Name',unicode);await tap(input,browser,13);
    await r.until([browser],()=>latest(browser).input.profile.DisplayName===unicode,'persist full Unicode name');
    images.push(await checkpoint(state,browser,'unicode','menu'));
    await click(input,browser,'Menu/NewProfile');e.requireTrue(latest(browser).input.profile.PlayerId!==original,'NEW_PROFILE_ID');
    await text(input,browser,'Menu/Name',unicode);await tap(input,browser,13);
    await click(input,browser,'Menu/Profiles');await tap(input,browser,36);await tap(input,browser,13);
    await r.until([browser],()=>latest(browser).input.profile.PlayerId===original,'select persisted original');
    images.push(await checkpoint(state,browser,'equal-name','menu'));
    const originalEvidence=(await take(state,{...item,roles:['browser']},[browser]))[0];originalEvidence.images.push(...images);
    images.length=0;
    const root=browser.profileRoot;await click(input,browser,'Menu/Quit');
    await Promise.race([new Promise(resolve=>browser.child.once('exit',resolve)),delay(5000)]);e.requireTrue(browser.exit?.code===0,'PROFILE_EXIT');await state.input.close();
    const restartDirectory=path.join(state.context.directory,'restart');fs.mkdirSync(restartDirectory);
    const restarted=net.launch({...state.context,directory:restartDirectory},'browser-restart',state.port,root);
    state.extra.push(browser);state.peers[state.peers.indexOf(browser)]=restarted;state.browser=restarted;
    state.launches.push({group:'G2',role:'browser-restart',pid:restarted.pid,profile:restarted.profile,reason:'Actual persisted-profile restart requires a distinct process ID',directory:path.relative(state.outer.directory,restarted.directory)});
    await r.until([restarted],()=>latest(restarted)?.input.profile?.PlayerId===original,'actual restart identity');
    e.requireTrue(restarted.pid!==browser.pid&&latest(restarted).input.profile.DisplayName===unicode,'PROFILE_RESTART');
    state.input=r.driver(state.context,[state.host,restarted]);await state.input.start();
    state.allDrivers.push(state.input);
    state.host.driver=state.input;restarted.driver=state.input;
    images.push(await checkpoint(state,restarted,'restart','menu'));
    const restartedEvidence=(await take(state,{...item,roles:['browser-restart']},[restarted]))[0];restartedEvidence.images.push(...images);
    return {peers:[originalEvidence,restartedEvidence]};
  }else throw new Error('UNKNOWN_G2_CASE');
  const peers=await take(state,item,[state.browser]);peers[0].images.push(...images);return {peers};
}
async function synthetic(state,peer,key,pressed){
  await net.key(peer,state.context,Date.now()+5000,key,pressed);
  await r.until([peer],()=>latest(peer)?.input.lastInput?.key===key&&latest(peer).input.lastInput.pressed===pressed,'SYNTHETIC_NATIVE_ACK');
}
async function walkers(state,item){
  const initial=state.peers.map(peer=>participant(peer).Actor),provenance=[];
  await focus(state.input,state.peers.at(-1));
  let messages=true;
  try{
    for(const peer of state.peers)await r.nativeKey(state.outer,state.input,peer,68,true,'window-message');
    await delay(100);
    e.requireTrue(state.peers.every(peer=>latest(peer).input.direction===1&&latest(peer).input.polledRight===true),'BACKGROUND_MESSAGE_HOLD');
  }catch(error){messages=false;provenance.push({method:'window-message',status:'unsupported',reason:error.message});}
  for(const peer of state.peers)await state.input.send(peer,{action:'key',method:'window-message',key:68,pressed:false});
  if(!messages)for(const peer of state.peers)await synthetic(state,peer,68,true);
  else for(const peer of state.peers)await r.nativeKey(state.outer,state.input,peer,68,true,'window-message');
  const start=latest(state.host).snapshot;
  await r.until(state.peers,()=>latest(state.host).snapshot.Participants.every(row=>row.Actor.ElapsedMilliseconds-start.Participants.find(old=>old.Actor.ActorId===row.Actor.ActorId).Actor.ElapsedMilliseconds>=1000),'one second simultaneous movement');
  const end=latest(state.host).snapshot;
  for(const row of end.Participants){const before=start.Participants.find(old=>old.Actor.ActorId===row.Actor.ActorId).Actor;
    const elapsed=row.Actor.ElapsedMilliseconds-before.ElapsedMilliseconds,delta=row.Actor.PositionSubunits-before.PositionSubunits;
    e.requireTrue(delta>=elapsed*2.85&&delta<=elapsed*3.15,'FOUR_WALK_SPEED');}
  const observations=await take(state,item);
  for(const peer of state.peers){if(!messages)await synthetic(state,peer,68,false);else await state.input.send(peer,{action:'key',method:'window-message',key:68,pressed:false});}
  provenance.push({method:messages?'window-message':'synthetic-native-ParseInputEvent',status:'executed',physicalInputCredit:false});
  e.requireTrue(initial.length===4,'FOUR_ACTUAL_PEERS');return {peers:observations,provenance,simultaneousPhysicalInputCredit:false};
}
async function timed(state,item){
  await resetFour(state);const timings=[],stopped=[];
  for(const origin of state.peers){
    await focus(state.input,origin);
    for(const [action,key,pressed] of [['start',68,true],['reversal',65,true],['stop',65,false]]){
      const result=await r.transition(state.outer,state.input,state.peers,origin,key,pressed,action);timings.push(...result.timings);
      if(action==='stop')stopped.push(...await r.stopAgreement(state.outer,state.peers,origin,result.ack,result.actor));
      await delay(100);
    }
    await r.nativeKey(state.outer,state.input,origin,68,true);await delay(100);
    const loss=await r.transition(state.outer,state.input,state.peers,origin,68,false,'focus-stop');timings.push(...loss.timings);
    await state.input.send(origin,{action:'key',method:'physical-SendInput',key:68,pressed:false});await focus(state.input,origin);
    await r.nativeKey(state.outer,state.input,origin,68,true);await r.nativeKey(state.outer,state.input,origin,68,false);
  }
  const peers=await take(state,item);
  for(const row of timings)peers.find(peer=>peer.role===row.observer).images.push({...row.image,checkpoint:'timing-'+row.origin+'-'+row.action,screen:'game'});
  return {peers,timings,stopped};
}
async function raceJoins(context,values){
  const prepared=[];
  for(const value of values){
    await click(value.input,value.peer,'Browser/Address');await tap(value.input,value.peer,9);
    const draw=await r.until([value.peer],()=>latest(value.peer)?.controls.some(control=>control.path==='Browser/JoinAddress'&&control.focused)?latest(value.peer):null,'ordinary focused Join control');
    prepared.push({value,frame:draw.drawFrame});
  }
  const acknowledgments=await Promise.all(prepared.map(async({value,frame})=>{
    const ack=await r.nativeKey(context,value.input,value.peer,13,true,'window-message');
    Object.assign(ack,{preparedControl:'Browser/JoinAddress',preparedControlDrawFrame:frame});return ack;
  }));
  await Promise.all(prepared.map(({value})=>r.nativeKey(context,value.input,value.peer,13,false,'window-message')));
  return {method:'window-message',physicalInputCredit:false,acknowledgments};
}
async function g3(state,item){
  if(item.id==='rendered.moving-join'){
    await host(state.input,state.host);
    for(const peer of state.peers.slice(1)){
      await browse(state.input,peer);await text(state.input,peer,'Browser/Address','127.0.0.1:'+state.port);
      await synthetic(state,state.host,68,false);await synthetic(state,state.host,68,true);
      const before=participant(state.host).Actor.PositionSubunits;
      await click(state.input,peer,'Browser/JoinAddress');await r.until([peer],()=>latest(peer).input.ready,'moving admission');
      e.requireTrue(participant(state.host).Actor.PositionSubunits>before,'EXISTING_ACTOR_STALLED');
    }
    await synthetic(state,state.host,68,false);await roster(state.peers,4);return {peers:await take(state,item),movingJoinDriver:'synthetic-native-ParseInputEvent; separate physical timing remains required'};
  }
  if(!state.peers.every(peer=>latest(peer)?.input.ready))await resetFour(state);
  if(item.id==='rendered.walkers')return walkers(state,item);
  if(item.id==='rendered.timing')return timed(state,item);
  if(item.id==='rendered.focus'){
    for(const peer of state.peers){await focus(state.input,peer);await r.nativeKey(state.outer,state.input,peer,68,true);await delay(80);
      const before=latest(peer).input.focusLosses;await state.input.send(peer,{action:'focus-sink',method:'physical-SendInput'});
      await r.until([peer],()=>latest(peer).input.focusLosses>before&&latest(peer).input.direction===0,'focus cleared native latch');
      await state.input.send(peer,{action:'key',method:'physical-SendInput',key:68,pressed:false});await focus(state.input,peer);
      e.requireTrue(latest(peer).input.direction===0,'FOCUS_RETAINED_HOLD');await r.nativeKey(state.outer,state.input,peer,68,true);await r.nativeKey(state.outer,state.input,peer,68,false);}
    return {peers:await take(state,item)};
  }
  if(item.id==='rendered.admission'){
    const fifth=await extra(state,'capacity-challenger',null,'Fifth capacity refusal requires a separate process beyond the four participants');
    await browse(fifth.input,fifth.peer);await text(fifth.input,fifth.peer,'Browser/Address','127.0.0.1:'+state.port);await click(fifth.input,fifth.peer,'Browser/JoinAddress');
    await r.until([fifth.peer],()=>latest(fifth.peer).input.error?.startsWith('Full'),'visible Full refusal');await roster(state.peers,4);
    const full=await checkpoint(state,fifth.peer,'full','browser');
    const leaving=state.peers.at(-1);await menu(state.input,leaving);await roster(state.peers.slice(0,3),3);
    const sixth=await extra(state,'race-challenger',null,'Two actual final-slot contenders prove serialized capacity');
    for(const value of [fifth,sixth]){await browse(value.input,value.peer);await text(value.input,value.peer,'Browser/Address','127.0.0.1:'+state.port);}
    const raceDelivery=await raceJoins(state.outer,[fifth,sixth]);
    await r.until([fifth.peer,sixth.peer],()=>[fifth.peer,sixth.peer].filter(peer=>latest(peer).input.ready).length===1&&[fifth.peer,sixth.peer].some(peer=>latest(peer).input.error?.startsWith('Full')),'last-slot race');
    const racePictures=[];
    for(const value of [fifth,sixth])racePictures.push(await checkpoint(state,value.peer,'race-result',latest(value.peer).snapshot?'game':'browser'));
    const winner=[fifth,sixth].find(value=>latest(value.peer).input.ready);await menu(winner.input,winner.peer);await join(state.input,leaving,state.port);
    await menu(state.input,leaving);await roster(state.peers.slice(0,3),3);
    const duplicate=await extra(state,'duplicate-identity',state.peers[1].profileRoot,'Actual duplicate stable-identity refusal requires one challenger sharing that owned profile root');
    await browse(duplicate.input,duplicate.peer);await text(duplicate.input,duplicate.peer,'Browser/Address','127.0.0.1:'+state.port);await click(duplicate.input,duplicate.peer,'Browser/JoinAddress');
    await r.until([duplicate.peer],()=>latest(duplicate.peer).input.error?.startsWith('Duplicate profile')&&!latest(duplicate.peer).input.ready,'duplicate identity refusal');
    const duplicatePicture=await checkpoint(state,duplicate.peer,'duplicate','browser');await join(state.input,leaving,state.port);await roster(state.peers,4);
    const additionalPeers=[fifth,sixth,duplicate].map((value,index)=>({role:value.peer.role,pid:value.peer.pid,profile:value.peer.profile,
      session:null,actor:null,binding:null,minimumRevision:0,images:index===0?[full,racePictures[0]]:index===1?[racePictures[1]]:[duplicatePicture],_peer:value.peer}));
    return {peers:await take(state,item),additionalPeers,raceDelivery};
  }
  if(item.id==='rendered.guest-lifecycle'){
    const guest=state.peers[1],before=participant(guest).Actor.ActorId;
    await menu(state.input,guest);await roster(state.peers.filter(peer=>peer!==guest),3);await join(state.input,guest,state.port);await roster(state.peers,4);
    e.requireTrue(participant(guest).Actor.ActorId!==before,'REPLACEMENT_ACTOR_REUSED');
    const lost=state.peers[2];await require('./terminal.cjs').terminateExpected(lost,item.id,'Deliberate guest loss before replacement');state.extra.push(lost);
    await r.until(state.peers.filter(peer=>peer!==lost),()=>latest(state.host).snapshot.Participants.length===3,'guest loss default',13000);
    await state.input.close();const replacement=await extra(state,'guest2',null,'Deliberate guest process loss requires a fresh replacement process');
    state.peers[2]=replacement.peer;
    await join(replacement.input,replacement.peer,state.port);await roster(state.peers,4);
    state.input=r.driver(state.context,state.peers);await state.input.start();
    state.allDrivers.push(state.input);
    for(const peer of state.peers)peer.driver=state.input;
    return {peers:await take(state,item)};
  }
  if(item.id==='rendered.host-end'){
    const previous=latest(state.host).snapshot.SessionId;await menu(state.input,state.host);
    for(const peer of state.peers.slice(1))await r.until([peer],()=>!latest(peer)?.snapshot&&latest(peer)?.input.error?.startsWith('The host ended'),'host-ended copy');
    const pictures=await take(state,item);await resetFour(state);e.requireTrue(latest(state.host).snapshot.SessionId!==previous,'REHOST_SESSION_REUSED');return {peers:pictures};
  }
  if(item.id==='rendered.host-loss'){
    if(!state.peers.every(peer=>latest(peer)?.input.ready))await resetFour(state);
    const before=await take(state,{...item,screen:'game'});
    const host=state.host;await require('./terminal.cjs').terminateExpected(host,item.id,'Deliberate final host-loss scenario');
    for(const peer of state.peers.slice(1))await r.until([peer],()=>!latest(peer)?.snapshot&&latest(peer)?.input.error?.startsWith('Connection lost'),'unexpected host-loss menu',13000);
    const guests=await take(state,{...item,roles:item.roles.filter(role=>role!=='host'),screen:'menu'});
    for(const peer of state.peers.slice(1)){await click(state.input,peer,'Menu/Start');await r.until([peer],()=>latest(peer).input.ready,'usable solo after host loss');await menu(state.input,peer);}
    return {peers:[before.find(peer=>peer.role==='host'),...guests],hostCheckpoint:'pre-loss actual completed draw; exited host cannot produce a post-loss image'};
  }
  throw new Error('UNKNOWN_G3_CASE '+item.id);
}
async function close(state){
  const errors=[];
  for(const peer of [...new Set([...state.peers,...state.extra])].filter(peer=>!peer.exit))try{
    await menu(peer.driver,peer);await click(peer.driver,peer,'Menu/Quit');
  }catch(error){errors.push('normal quit '+peer.role+': '+error.message);}
  for(const input of [state.input,...state.drivers])try{await input.close();}catch(error){errors.push(error.message);}
  for(const peer of [...new Set([...state.peers,...state.extra])])try{await net.cleanup(peer);}catch(error){errors.push(error.message);}
  if(errors.length)throw new Error('OWNED_SCENARIO_CLEANUP '+errors.join('; '));
}
async function run(context,items){
  const unlock=acquireLock(context.root,context.nonce),results=[],launches=[],states=[];
  const identity={nonce:context.nonce,input:context.input,tool:context.tool,platform:context.platform,revision:context.revision,osFingerprint:context.osFingerprint};
  try{
    for(const group of ['G2','G3','G4']){
      const selected=items.filter(row=>row.runGroup===group).sort((a,b)=>a.order-b.order);if(!selected.length)continue;
      let state;
      if(group==='G4')state=states.find(row=>row.group==='G3'&&row.peers.every(peer=>!peer.exit));
      if(!state)try{state=await create(context,group,group==='G2'?['discovery-host','browser']:['host','guest1','guest2','guest3'],launches);states.push(state);}
      catch(error){for(const item of selected)results.push({id:item.id,status:'blocked',...identity,message:'GROUP_START '+error.message});continue;}
      for(const item of selected){console.log('Checking '+item.id);
        try{const observed=group==='G2'?await g2(state,item):await g3(state,item);results.push({id:item.id,status:'pass',...identity,...observed,_state:state});}
        catch(error){results.push({id:item.id,status:'fail',...identity,message:error.message,stack:error.stack});fs.writeFileSync(path.join(context.directory,item.id+'.failure.log'),error.stack,{flag:'wx'});}
      }
      if(group==='G2')try{await close(state);}catch(error){require('./terminal.cjs').invalidate(results,state,error);}
    }
  }finally{
    for(const state of states.filter(row=>row.group!=='G2'))try{await close(state);}catch(error){require('./terminal.cjs').invalidate(results,state,error);}
    unlock();
  }
  const artifact=file=>({path:path.relative(context.directory,file),sha256:e.hash(fs.readFileSync(file))});
  for(const result of results)if(result._state){result.nativeDrivers=result._state.allDrivers.map(input=>artifact(input.file));
    result.ownedProcesses=[...new Set([...result._state.peers,...result._state.extra])].map(peer=>({role:peer.role,pid:peer.pid,terminal:artifact(path.join(peer.directory,'terminal.json'))}));delete result._state;}
  for(const result of results)for(const peer of [...result.peers??[],...result.additionalPeers??[]]){peer.launch=artifact(path.join(peer._peer.directory,'launch.json'));peer.log=artifact(path.join(peer._peer.directory,'console.log'));peer.draws=artifact(path.join(peer._peer.directory,'draws.ndjson'));peer.terminal=artifact(path.join(peer._peer.directory,'terminal.json'));delete peer._peer;}
  return {results,launches};
}
module.exports={run,click,text,menu,host,browse,join,roster,resetFour,create,close,raceJoins};
