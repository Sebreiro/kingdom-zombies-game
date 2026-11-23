'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),test=require('node:test'),assert=require('node:assert/strict');
const scenarios=require('./scenarios.cjs'),delay=ms=>new Promise(resolve=>setTimeout(resolve,ms));
function fixture(){
  const directory=path.resolve(__dirname,'../../../../.phase3/controls',crypto.randomUUID());fs.mkdirSync(directory,{recursive:true});
  let foreground=null;const trace=[],pending=[];
  const values=[1,2].map(pid=>{
    const local=path.join(directory,String(pid));fs.mkdirSync(local);
    const peer={pid,role:'control'+pid,directory:local,exit:null},draw={drawFrame:1,input:{inputEvents:0},controls:[
      {path:'Browser/Address',visible:true,x:10,y:10,focused:false},{path:'Browser/JoinAddress',visible:true,x:20,y:20,focused:false}]};
    const publish=()=>{draw.drawFrame++;fs.appendFileSync(path.join(local,'draws.ndjson'),JSON.stringify(draw)+'\n');};publish();
    const input={async send(subject,request){
      trace.push([pid,request.action,request.method,request.key]);
      if(request.action==='focus'){foreground=pid;await delay(10);}
      if(request.method==='physical-SendInput'&&request.action!=='focus')assert.equal(foreground,pid,'foreground ownership must remain strict');
      if(request.action==='key'&&request.key===9&&request.pressed){draw.controls[1].focused=true;publish();}
      if(request.method==='window-message'&&request.key===13){
        assert.equal(draw.controls[1].focused,true);draw.input.inputEvents++;draw.input.lastInput={key:4194309,pressed:request.pressed};publish();
        if(request.pressed)await new Promise(resolve=>{pending.push(resolve);if(pending.length===2)for(const release of pending)release();});
      }
      return {...request,pid,driverPid:100+pid,sequence:trace.length};
    }};return {peer,input};
  });return {values,trace};
}
test('actual exported focus-plus-click operations serialize across independent helpers',async()=>{
  const {values,trace}=fixture();await Promise.all(values.map(value=>scenarios.click(value.input,value.peer,'Browser/JoinAddress')));
  assert.deepEqual(trace.map(row=>row.slice(0,2)),[[1,'focus'],[1,'click'],[2,'focus'],[2,'click']]);
});
test('final-slot procedure prepares serially then concurrently delivers PID-targeted native events honestly',async()=>{
  const {values,trace}=fixture(),delivery=await scenarios.raceJoins({},values);
  assert.equal(delivery.method,'window-message');assert.equal(delivery.physicalInputCredit,false);
  assert.deepEqual(delivery.acknowledgments.map(ack=>ack.pid),[1,2]);
  assert.ok(delivery.acknowledgments.every(ack=>ack.nativeInputEvents>0&&ack.nativeDrawFrame>0&&ack.preparedControlDrawFrame>0));
  const first=trace.findIndex(row=>row[2]==='window-message');assert.ok(first>0);
  assert.ok(trace.slice(first).every(row=>row[2]==='window-message'));
});
