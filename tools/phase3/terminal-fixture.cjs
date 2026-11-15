'use strict';
const fs=require('node:fs'),path=require('node:path'),assert=require('node:assert/strict'),{spawn}=require('node:child_process');
const terminal=require('./terminal.cjs'),scenarios=require('./scenarios.cjs');
const delay=ms=>new Promise(resolve=>setTimeout(resolve,ms));
async function run(directory){
  directory=path.resolve(directory);
  for(const mode of ['normal','after-final-capture','during-quit','crash','expected-loss']){
    const local=path.join(directory,'terminal-'+mode);fs.mkdirSync(local);
    const script='const fs=require("fs");const [dir,mode]=process.argv.slice(1);process.stdout.write("READY\\n");process.stdin.once("data",()=>{'+
      'if(mode==="crash")process.abort();if(mode!=="after-final-capture")fs.writeFileSync(dir+"/quit.ack.json",JSON.stringify({action:"ordinary-Quit",nonce:"control",pid:process.pid,role:"control"}));process.exit(mode==="normal"?0:7);});';
    const child=spawn(process.execPath,['-e',script,local,mode],{cwd:local,windowsHide:true,stdio:['pipe','pipe','pipe']});
    const peer={role:'control',pid:child.pid,child,directory:local,exit:null,error:null,output:''};terminal.track(peer,{nonce:'control'});
    child.stdout.on('data',bytes=>peer.output+=bytes);child.stderr.on('data',bytes=>peer.output+=bytes);child.once('error',error=>peer.error=error);
    for(let attempt=0;attempt<100&&!peer.output.includes('READY');attempt++)await delay(10);
    assert.match(peer.output,/READY/,'the real child must start before the intended failure');
    fs.writeFileSync(path.join(local,'draws.ndjson'),JSON.stringify({drawFrame:1,snapshot:null,controls:[{path:'Menu/Host',visible:true},{path:'Menu/Quit',visible:true,x:1,y:1}]})+'\n');
    const input={async send(subject,request){if(request.action==='click')child.stdin.write('quit');return {};},async close(){}};
    peer.driver=input;const state={peers:[peer],extra:[],input,drivers:[]};
    const captured=require('./rendered.cjs').capture({nonce:'control',directory:local},peer,'final-control');
    const checkpoint=JSON.parse(fs.readFileSync(path.join(local,'draw.request'),'utf8')).checkpoint;
    fs.writeFileSync(path.join(local,checkpoint+'.png'),require('./runner-fixture.cjs').png());
    fs.writeFileSync(path.join(local,checkpoint+'.json.tmp'),JSON.stringify({nonce:'control',pid:peer.pid,drawn:true,drawFrame:1}));
    fs.renameSync(path.join(local,checkpoint+'.json.tmp'),path.join(local,checkpoint+'.json'));
    await captured;
    if(['after-final-capture','crash'].includes(mode)){child.stdin.write('fail-after-capture');await peer.closed;}
    if(mode==='expected-loss'){await terminal.terminateExpected(peer,'control.expected-loss','Declared fixture loss before the owned kill');}
    if(['normal','expected-loss'].includes(mode))await scenarios.close(state);
    else{
      await assert.rejects(scenarios.close(state),/TERMINAL_NORMAL_QUIT/);
      const results=[{status:'pass',_state:state},{status:'pass',_state:state},{status:'pass',_state:{}}];
      terminal.invalidate(results,state,new Error('real child failed'));assert.deepEqual(results.map(result=>result.status),['fail','fail','pass']);
    }
    const record=JSON.parse(fs.readFileSync(path.join(local,'terminal.json'),'utf8'));
    assert.throws(()=>terminal.validate({...record,quitAck:null,expectedLoss:null}));
    if(mode==='after-final-capture')assert.throws(()=>terminal.expect(peer,'control.expected-loss','Too late'));
  }
  console.log('PASS actual rendered-peer lifecycle normal quit, expected loss, post-capture nonzero, crash and during-quit failure controls; no game/physical credit');
}
module.exports={run};
