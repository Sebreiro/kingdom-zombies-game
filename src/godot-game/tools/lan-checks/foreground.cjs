'use strict';
const {AsyncLocalStorage}=require('node:async_hooks');
const owner=new AsyncLocalStorage();let tail=Promise.resolve();
async function run(action){
  if(owner.getStore())return action();
  const previous=tail;let release;tail=new Promise(resolve=>release=resolve);await previous;
  try{return await owner.run(true,action);}finally{release();}
}
module.exports={run};
