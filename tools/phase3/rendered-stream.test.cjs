'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {lines}=require('./rendered.cjs');
test('large legitimate draw backlog remains bounded, current and UTF8-complete; truncation and oversized records reject',()=>{
  const directory=fs.mkdtempSync(path.resolve(__dirname,'../../.phase3/stream-control-')),file=path.join(directory,'draws.ndjson');
  const descriptor=fs.openSync(file,'wx'),name='雪🌙'.repeat(180);
  try{for(let frame=1;frame<=18000;frame++)fs.writeSync(descriptor,JSON.stringify({frame,name})+'\n');}finally{fs.closeSync(descriptor);}
  assert.ok(fs.statSync(file).size>16*1024*1024);
  const observed=lines(file);assert.equal(observed.length,256);assert.equal(observed[0].frame,17745);
  assert.deepEqual(observed.at(-1),{frame:18000,name});
  const next=Buffer.from(JSON.stringify({frame:18001,name:'雪🌙'})+'\n'),boundary=next.indexOf(Buffer.from('雪'))+1;
  fs.appendFileSync(file,next.subarray(0,boundary));assert.equal(lines(file).at(-1).frame,18000);
  fs.appendFileSync(file,next.subarray(boundary));assert.deepEqual(lines(file).at(-1),{frame:18001,name:'雪🌙'});
  fs.truncateSync(file,1);assert.throws(()=>lines(file),/DRAW_STREAM_REPLACED_OR_TRUNCATED/);
  const huge=path.join(directory,'oversized.ndjson');fs.writeFileSync(huge,Buffer.alloc(16*1024*1024+1,120));
  assert.throws(()=>lines(huge),/DRAW_RECORD_BOUNDS/);
});
