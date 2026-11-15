'use strict';
const {spawn,spawnSync}=require('node:child_process');
const {requireTrue}=require('./evidence.cjs');
const delay=ms=>new Promise(resolve=>{setTimeout(resolve,ms).unref();});
function terminate(child){
  if(!child?.pid||child.exitCode!==null||child.signalCode!==null)return;
  if(process.platform==='win32'){
    const result=spawnSync('taskkill.exe',['/PID',String(child.pid),'/T','/F'],{windowsHide:true,encoding:'utf8',timeout:4000});
    requireTrue(!result.error&&result.status===0,'OWNED_TREE_KILL_FAILED '+result.stderr);
  }else{try{process.kill(-child.pid,'SIGKILL');}catch(error){if(error.code!=='ESRCH')throw error;}}
}
async function run(executable,args,options={},milliseconds=12000){
  const {onOutput,...spawnOptions}=options;
  const child=spawn(executable,args,{...spawnOptions,windowsHide:true,shell:false,detached:process.platform!=='win32'});
  let stdout='',stderr='',spawnError,timedOut=false,cleanupError;
  const closed=new Promise(resolve=>{child.once('error',error=>spawnError=error);child.once('close',(code,signal)=>resolve({code,signal}));});
  child.stdout?.on('data',bytes=>{stdout+=bytes;if(onOutput)onOutput(bytes.toString());});child.stderr?.on('data',bytes=>{stderr+=bytes;if(onOutput)onOutput(bytes.toString());});
  const timer=setTimeout(()=>{timedOut=true;try{terminate(child);}catch(error){cleanupError=error;}},milliseconds);
  let result;
  try{result=await Promise.race([closed,delay(milliseconds+6000).then(()=>{throw new Error('OWNED_CLEANUP_DEADLINE');})]);}
  finally{clearTimeout(timer);}
  const record={executable,args,pid:child.pid,...result,timedOut,stdout,stderr,cleanupError:cleanupError?.message};
  if(spawnError||timedOut||cleanupError||result.code!==0||result.signal)throw Object.assign(new Error(spawnError?.code??(timedOut?'CHILD_TIMEOUT':'CHILD_EXIT '+result.code+'/'+result.signal)),{record});
  return record;
}
module.exports={run,terminate};
