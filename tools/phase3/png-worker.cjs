'use strict';
const {Worker,isMainThread,parentPort}=require('node:worker_threads');
if(!isMainThread){
  const fs=require('node:fs'),crypto=require('node:crypto');
  parentPort.on('message',({id,file})=>{
    try{parentPort.postMessage({id,value:{...require('./png.cjs').validate(file),
      sha256:crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex')}});}
    catch(error){parentPort.postMessage({id,error:error.message});}
  });
}else{
  let pool,sequence=0;
  function create(){
    const state={slots:[],queue:[],closed:false,error:null};
    function fail(error){
      if(state.closed||state.error)return;state.error=error;
      for(const job of state.queue.splice(0))job.reject(error);
      for(const slot of state.slots)if(slot.job){clearTimeout(slot.job.timer);slot.job.reject(error);slot.job=null;slot.worker.unref();}
    }
    function dispatch(){
      if(state.closed||state.error)return;
      for(const slot of state.slots)if(!slot.job&&state.queue.length){
        const job=state.queue.shift();slot.job=job;slot.worker.ref();
        job.timer=setTimeout(()=>fail(new Error('PNG_WORKER_DEADLINE')),8000);
        slot.worker.postMessage({id:job.id,file:job.file});
      }
    }
    for(let index=0;index<3;index++){
      const slot={worker:new Worker(__filename),job:null};state.slots.push(slot);
      slot.worker.on('message',row=>{
        if(state.closed||state.error)return;
        const job=slot.job;if(!job||row.id!==job.id){fail(new Error('PNG_WORKER_IDENTITY'));return;}
        clearTimeout(job.timer);slot.job=null;slot.worker.unref();
        if(row.error)job.reject(new Error(row.error));else job.resolve(row.value);
        dispatch();
      });
      slot.worker.on('error',fail);
      slot.worker.on('exit',code=>{if(!state.closed)fail(new Error('PNG_WORKER_EXIT '+code));});
      slot.worker.unref();
    }
    state.dispatch=dispatch;return state;
  }
  function validate(file){
    pool??=create();if(pool.error)return Promise.reject(pool.error);
    return new Promise((resolve,reject)=>{pool.queue.push({id:++sequence,file,resolve,reject});pool.dispatch();});
  }
  async function close(){
    const state=pool;if(!state)return;pool=null;state.closed=true;
    for(const job of state.queue.splice(0))job.reject(new Error('PNG_WORKER_CLOSED'));
    for(const slot of state.slots)if(slot.job){clearTimeout(slot.job.timer);slot.job.reject(new Error('PNG_WORKER_CLOSED'));slot.job=null;}
    await Promise.all(state.slots.map(slot=>slot.worker.terminate()));
  }
  module.exports={validate,close};
}
