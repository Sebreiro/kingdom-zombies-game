'use strict';
const fs=require('node:fs'),path=require('node:path'),e=require('./evidence.cjs');
const now=()=>String(process.hrtime.bigint()),delay=ms=>new Promise(resolve=>setTimeout(resolve,ms));
function track(peer,context){
  peer.nonce=context.nonce;
  peer.child.once('exit',(code,signal)=>peer.exit={code,signal,observedAt:now()});
  peer.closed=new Promise(resolve=>peer.child.once('close',()=>{peer.streamsClosed=true;resolve();}));
}
function requestQuit(peer){
  e.requireTrue(!peer.exit&&!peer.quitRequested,'QUIT_ALREADY_EXITED_OR_REQUESTED '+peer.role);
  peer.quitRequested={before:now()};
}
function expect(peer,caseId,reason){
  e.requireTrue(!peer.exit&&peer.child.exitCode===null&&peer.child.signalCode===null&&!peer.expectedLoss&&typeof caseId==='string'&&reason?.trim(),'EXPECTED_TERMINATION_BEFORE_EXIT');
  peer.expectedLoss={caseId,reason,declaredBefore:now(),pid:peer.pid,nonce:peer.nonce};peer.expectedTermination=true;
}
function validate(record,production=false){
  e.requireTrue(record.exit&&/^\d+$/.test(record.exit.observedAt)&&record.streamsClosed===true&&!record.error,'TERMINAL_RECORD');
  if(record.expectedLoss){
    const expected=record.expectedLoss;
    const allowed=production?['rendered.host-loss','rendered.guest-lifecycle']:['rendered.host-loss','rendered.guest-lifecycle','network.lifecycle','control.expected-loss'];
    e.requireTrue(allowed.includes(expected.caseId)&&(!production||expected.caseId==='rendered.host-loss'&&record.role==='host'||expected.caseId==='rendered.guest-lifecycle'&&record.role==='guest2')&&
      expected.pid===record.pid&&expected.nonce===record.nonce&&expected.reason?.trim()&&BigInt(expected.declaredBefore)<=BigInt(record.exit.observedAt)&&
      record.intentionalKill===true&&!record.cleanupKill,'TERMINAL_EXPECTED_LOSS');
  }else{
    e.requireTrue(record.exit.code===0&&!record.exit.signal&&!record.cleanupKill&&record.quitRequested&&record.quitAck?.action==='ordinary-Quit'&&
      record.quitAck.pid===record.pid&&record.quitAck.nonce===record.nonce&&record.quitAck.role===record.role&&
      BigInt(record.quitRequested.before)<=BigInt(record.exit.observedAt),'TERMINAL_NORMAL_QUIT '+record.role);
  }
  return record;
}
async function cleanup(peer){
  if(!peer.exit){
    // An ordinary Quit gets its bounded opportunity to exit before owned emergency cleanup.
    if(peer.quitRequested)await Promise.race([peer.closed,delay(5000)]);
    if(!peer.exit){peer.cleanupKill=true;peer.child.kill();}
  }
  await Promise.race([peer.closed,delay(3000)]);
  const file=path.join(peer.directory,'quit.ack.json'),quitAck=fs.existsSync(file)?e.read(file):null;
  const record={nonce:peer.nonce,pid:peer.pid,role:peer.role,exit:peer.exit,streamsClosed:peer.streamsClosed===true,error:peer.error?.message,
    quitRequested:peer.quitRequested,quitAck,expectedLoss:peer.expectedLoss,intentionalKill:peer.intentionalKill===true,cleanupKill:peer.cleanupKill===true};
  fs.writeFileSync(path.join(peer.directory,'console.log'),peer.output);
  const destination=path.join(peer.directory,'terminal.json');
  if(!fs.existsSync(destination))fs.writeFileSync(destination,JSON.stringify(record,null,2),{flag:'wx'});
  else e.requireTrue(JSON.stringify(e.read(destination))===JSON.stringify(record),'TERMINAL_CHANGED');
  validate(record);return record;
}
async function terminateExpected(peer,caseId,reason){
  expect(peer,caseId,reason);peer.intentionalKill=peer.child.kill();await Promise.race([peer.closed,delay(3000)]);
  e.requireTrue(peer.exit,'EXPECTED_PROCESS_CLEANUP');
}
function invalidate(results,state,error){
  for(const result of results.filter(result=>result._state===state)){
    result.status='fail';result.cleanupError=error.message;result.message??=error.message;
  }
}
module.exports={track,requestQuit,expect,validate,cleanup,terminateExpected,invalidate};
