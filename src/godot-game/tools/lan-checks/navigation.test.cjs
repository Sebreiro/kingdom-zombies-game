'use strict';
const fs=require('node:fs'),path=require('node:path'),test=require('node:test'),assert=require('node:assert/strict');
test('the inherited keyboard procedure follows the actual native scene neighbors in both directions',async()=>{
  const scene=fs.readFileSync(path.resolve(__dirname,'../../game/foundation/Foundation.tscn'),'utf8');
  const nodes=new Map([...scene.matchAll(/\[node name="([^"]+)"[^\]]*parent="Menu"\]([\s\S]*?)(?=\[node|$)/g)].map(match=>[match[1],match[2]]));
  let current='Start',shift=false,observations=0;
  async function send(action,key){
    if(key==='Shift_L'){shift=action==='down';return;}
    assert.equal(action,'tap');const property=key==='Tab'?(shift?'focus_previous':'focus_next'):key==='Down'?'focus_neighbor_bottom':'focus_neighbor_top';
    const neighbor=nodes.get(current).match(new RegExp(property+' = NodePath\\("\\.\\./([^"/]+)"\\)'));assert.ok(neighbor,current+'/'+property);current=neighbor[1];
  }
  await require('../gameplay-checks/menu-navigation.cjs').check(send,async()=>{observations++;return {focusedControl:current};},(value,label)=>assert.ok(value,label));
  assert.equal(current,'Start');assert.equal(shift,false);assert.equal(observations,20);
});
