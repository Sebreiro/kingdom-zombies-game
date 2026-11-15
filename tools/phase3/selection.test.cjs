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
