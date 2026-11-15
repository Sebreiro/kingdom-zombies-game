'use strict';
// Counterfeit records below prove rejection only. Actual tracer/process cases execute their subjects separately.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),zlib=require('node:zlib');
const assert=require('node:assert/strict'),e=require('./evidence.cjs'),p=require('./processes.cjs');
const {acquireLock}=require('../phase2/check.cjs'),{select}=require('./check.cjs');
const delay=ms=>new Promise(resolve=>setTimeout(resolve,ms));
function png(){
  const {crc}=require('./png.cjs');
  function chunk(type,bytes){const head=Buffer.alloc(8),tail=Buffer.alloc(4);head.writeUInt32BE(bytes.length);head.write(type,4);tail.writeUInt32BE(crc(Buffer.concat([Buffer.from(type),bytes])));return Buffer.concat([head,bytes,tail]);}
  const header=Buffer.alloc(13);header.writeUInt32BE(1280);header.writeUInt32BE(720,4);header[8]=8;header[9]=2;
  const raw=Buffer.alloc(720*(1+1280*3));for(let y=0;y<720;y++)for(let x=0;x<1280;x++)raw[y*(1+1280*3)+1+x*3]=x%255;
  return Buffer.concat([Buffer.from([137,80,78,71,13,10,26,10]),chunk('IHDR',header),chunk('IDAT',zlib.deflateSync(raw)),chunk('IEND',Buffer.alloc(0))]);
}
function fixture(directory){
  const context={directory,nonce:crypto.randomUUID(),input:'input',tool:'tool',platform:process.platform+'-'+process.arch,revision:'revision',osFingerprint:'os'};
  const artifact=file=>({path:file,sha256:e.hash(fs.readFileSync(path.join(directory,file)))});
  fs.writeFileSync(path.join(directory,'image.png'),png());
  const selected=[{id:'control.scene',implementation:'native-rendered',needsGameWindow:true,roles:['host','guest1'],images:['scene']},
    {id:'control.unit',implementation:'csharp'}];
  const peers=['host','guest1'].map((role,index)=>{
    const peer={role,pid:1000+index,profile:'p'+index,session:'session',actor:'a'+index,binding:'b'+index,minimumRevision:1};
    const actors=[0,1].map(index=>({actorId:'a'+index,playerId:'p'+index,bindingId:'b'+index,x:0,y:540,screenX:640,screenY:540,facing:1,visible:true,insideCamera:true}));
    const state={nonce:context.nonce,pid:peer.pid,role,profile:peer.profile,playerId:peer.profile,bindingId:peer.binding,
      checkpoint:role,drawn:true,drawFrame:1,width:1280,height:720,cameraX:0,actors,
      snapshot:{SessionId:'session',Revision:1,Participants:[0,1].map(index=>({Ready:true,BindingId:'b'+index,Actor:{ActorId:'a'+index,PlayerId:'p'+index,PositionSubunits:0,Facing:1}}))}};
    fs.writeFileSync(path.join(directory,role+'.json'),JSON.stringify(state));
    fs.writeFileSync(path.join(directory,role+'.launch.json'),JSON.stringify({...peer,nonce:context.nonce,headless:false}));
    fs.writeFileSync(path.join(directory,role+'.log'),'CONTROL_ONLY_NOT_PRODUCTION_EVIDENCE\n');
    fs.writeFileSync(path.join(directory,role+'.terminal.json'),JSON.stringify({nonce:context.nonce,pid:peer.pid,role,streamsClosed:true,exit:{code:0,signal:null,observedAt:'2'},quitRequested:{before:'1'},quitAck:{action:'ordinary-Quit',nonce:context.nonce,pid:peer.pid,role}}));
    return {...peer,launch:artifact(role+'.launch.json'),log:artifact(role+'.log'),terminal:artifact(role+'.terminal.json'),images:[{checkpoint:'scene',captureId:role,...artifact('image.png'),statePath:role+'.json',stateSha256:artifact(role+'.json').sha256}]};
  });
  fs.writeFileSync(path.join(directory,'subject.log'),'CONTROL_ONLY');fs.writeFileSync(path.join(directory,'test.json'),JSON.stringify({results:{summary:{tests:1,passed:1,failed:0}}}));
  const first={...context,directory:undefined,id:selected[0].id,status:'pass',peers};
  const second={...context,directory:undefined,id:selected[1].id,status:'pass',testReport:'test.json',artifacts:[artifact('subject.log'),artifact('test.json')]};
  return {context,selected,results:[first,second],artifact};
}
async function run(mode){
  const root=path.resolve(__dirname,'../..'),directory=path.join(root,'.phase3/controls',crypto.randomUUID());fs.mkdirSync(directory,{recursive:true});
  if(mode==='integrity'){
    await require('./processes.cjs').run(process.execPath,['--test',path.join(root,'tools/phase3/navigation.test.cjs'),path.join(root,'tools/phase3/selection.test.cjs')],{cwd:root},12000);
    const {context,selected,results}=fixture(directory),[first,second]=results;
    e.reconcile(selected,results,context);e.reconcile(selected,[second,{...first,peers:[...first.peers].reverse()}],context);
    const bad=mutation=>{const clone=structuredClone(results);mutation(clone);assert.throws(()=>e.reconcile(selected,clone,context));};
    assert.throws(()=>e.reconcile([],[],context));assert.throws(()=>e.reconcile(selected,[],context));
    bad(rows=>rows.pop());bad(rows=>rows[1]=rows[0]);bad(rows=>rows[1].id='unknown');bad(rows=>rows[0].status='blocked');
    for(const key of ['nonce','input','tool','platform','revision','osFingerprint'])bad(rows=>rows[0][key]='stale');
    bad(rows=>rows[0].peers.pop());bad(rows=>rows[0].peers[1]=rows[0].peers[0]);bad(rows=>rows[0].peers[1].pid=rows[0].peers[0].pid);
    bad(rows=>rows[0].peers[1].profile=rows[0].peers[0].profile);bad(rows=>rows[0].peers[0].images=[]);
    bad(rows=>rows[0].peers[0].images.push(rows[0].peers[0].images[0]));
    bad(rows=>rows[0].peers[0].images[0].path='../foreign.png');
    bad(rows=>rows[0].peers[0].images[0].path='C:foreign.png');
    bad(rows=>rows[0].peers[0].images[0].stateSha256='0'.repeat(64));
    const stateFile=path.join(directory,'host.json'),originalState=fs.readFileSync(stateFile),host=first.peers[0];
    for(const delta of [{nonce:'old'},{pid:7},{role:'substitute'},{profile:'other'},{drawFrame:0},{drawn:false},{width:1920}]){
      fs.writeFileSync(stateFile,JSON.stringify({...JSON.parse(originalState),...delta}));
      const picture={...host.images[0],stateSha256:e.hash(fs.readFileSync(stateFile))};assert.throws(()=>e.image(context,picture,host));
    }
    fs.writeFileSync(stateFile,originalState);
    for(const args of [['--case',''],['--case','control.scene,control.scene'],['--case','unknown'],['--group','unknown'],['--all','--group','core']])assert.throws(()=>select({cases:selected},args));
    const bytes=fs.readFileSync(path.join(directory,'image.png'));bytes[bytes.length-1]^=1;fs.writeFileSync(path.join(directory,'image.png'),bytes);
    assert.throws(()=>e.reconcile(selected,results,context));assert.throws(()=>require('./png.cjs').validate(path.join(directory,'image.png')),/PNG_CRC/);
    fs.unlinkSync(path.join(directory,'image.png'));assert.throws(()=>e.reconcile(selected,results,context));
    assert.throws(()=>e.verifyCompletion(root,{schemaVersion:1,scope:'Windows 1280x720 local; no VM'},selected,context));
    require('./catalog.cjs').validate(root,e.read(path.join(root,'tools/phase3/required-cases.json')));
    console.log('PASS exact cases/roles, duplicate substitution, stale identities, images, corruption, unsafe paths and catalog controls; fixtures earn no execution credit');
  }else if(mode==='processes'){
    await require('./terminal-fixture.cjs').run(directory);
    const positive=await p.run(process.execPath,['-e','process.stdout.write("real-positive")']);assert.equal(positive.stdout,'real-positive');
    for(const [name,args,pattern] of [['missing',[],/ENOENT/],['nonzero',['-e','process.exit(7)'],/CHILD_EXIT 7/],['crash',['-e','process.abort()'],/CHILD_EXIT/]]){
      try{await p.run(name==='missing'?path.join(directory,'absent'):process.execPath,args,{cwd:directory},1000);assert.fail('child should fail');}
      catch(error){assert.match(error.message,pattern);fs.writeFileSync(path.join(directory,name+'.json'),JSON.stringify(error.record??{error:error.message}));}
    }
    const file=path.join(directory,'descendants.json');
    const script='const fs=require("fs"),{spawn}=require("child_process");const child=spawn(process.execPath,["-e","setInterval(()=>{},1000)"],{stdio:"ignore",windowsHide:true});fs.writeFileSync(process.argv[1],JSON.stringify({parent:process.pid,child:child.pid}));setInterval(()=>{},1000);';
    await assert.rejects(p.run(process.execPath,['-e',script,file],{cwd:directory},500),/CHILD_TIMEOUT/);
    const owned=e.read(file);for(const pid of Object.values(owned)){let live=true;for(let attempt=0;attempt<15;attempt++){try{process.kill(pid,0);}catch{live=false;break;}await delay(20);}assert.equal(live,false,'owned descendant survived '+pid);}
    console.log('PASS actual nonzero/crash/hung child and descendant cleanup; no Godot process or foreign PID targeted');
  }else if(mode==='repeatability'){
    const {context,selected,results}=fixture(directory);e.reconcile(selected,[...results].reverse(),context);
    const file=path.join(directory,'original.json');fs.writeFileSync(file,JSON.stringify({nonce:context.nonce,status:'fail',results}),{flag:'wx'});
    const original=e.hash(fs.readFileSync(file));assert.throws(()=>fs.writeFileSync(file,'replacement',{flag:'wx'}),/EEXIST/);
    const second=path.join(directory,'repeat');fs.mkdirSync(second);const fresh=fixture(second);assert.notEqual(fresh.context.nonce,context.nonce);e.reconcile(fresh.selected,fresh.results,fresh.context);
    assert.equal(e.hash(fs.readFileSync(file)),original);
    const lockRoot=path.join(directory,'desktop');fs.mkdirSync(lockRoot);const unlock=acquireLock(lockRoot,context.nonce);
    try{await assert.rejects(p.run(process.execPath,['-e','require(process.argv[1]).acquireLock(process.argv[2],"competitor")',path.join(root,'tools/phase2/check.cjs'),lockRoot],{cwd:root},1000),/CHILD_EXIT/);}
    finally{unlock();}
    const release=acquireLock(lockRoot,crypto.randomUUID()),lockFile=path.join(lockRoot,'.phase2/desktop.lock');
    fs.writeFileSync(lockFile,'foreign-owner');assert.throws(release,/OWNERSHIP_CHANGED/);assert.equal(fs.readFileSync(lockFile,'utf8'),'foreign-owner');
    console.log('PASS reordered exact evidence, distinct repeat nonces, immutable original and actual concurrent lock refusal; foreign lock retained');
  }else throw new Error('UNKNOWN_CONTROL');
  console.log('CONTROL_DIRECTORY '+directory);return {mode,directory};
}
module.exports={run,png,fixture};
