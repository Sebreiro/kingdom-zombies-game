'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {select}=require('./check.cjs');
test('exact all-selection preserves the independent required case set',()=>{
  const catalog={cases:[{id:'one',group:'core'},{id:'two',group:'runner'}]};
  let selection;
  try {selection=select(catalog,['--all']);}catch(error){assert.match(error.message,/USAGE/);}
  assert.equal(selection!==undefined,true,'exhaustive selection must be supported');
  assert.deepEqual(selection.selected.map(row=>row.id),['one','two']);
  for(const args of [['--case',''],['--case','one,one'],['--case','unknown'],['--group','unknown'],['--all','--case','one']])assert.throws(()=>select(catalog,args));
});

test('prelaunch rejects every known affected analyzer, generated and register omission with a nonblank excuse',()=>{
  const path=require('node:path'),root=path.resolve(__dirname,'../../../..'),e=require('./evidence.cjs'),catalog=require('./catalog.cjs');
  const cases=e.read(path.join(root,'src/godot-game/tools/verification/required-cases.json')).cases,required=require('./selection-impact.cjs').affected(cases);
  const definition=e.read(path.join(root,'src/godot-game/tools/lan-checks/required-cases.json')),saved=catalog.validate(root,definition);
  assert.ok(required.length>=31);
  for(const id of required){const changed=structuredClone(saved);changed.selected.phase1=changed.selected.phase1.filter(value=>value!==id);
    changed.omitted.phase1.push({id,reason:'This nonblank excuse must not bypass known changed consumers.'});
    assert.throws(()=>catalog.validate(root,definition,changed),new RegExp('AFFECTED_PHASE1_OMITTED '+id.replaceAll('.','\\.')));
  }
});
