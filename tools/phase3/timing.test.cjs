'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const fs=require('node:fs');
test('draw timing requires an inclusive conservative external-clock bound',()=>{
  const moduleFile=__dirname+'/evidence.cjs';
  const evaluate=fs.existsSync(moduleFile)?require(moduleFile).timing:null;
  assert.equal(typeof evaluate,'function','post-draw timing evaluator must exist');
  const row={clock:'coordinator-hrtime',origin:'host',observer:'guest1',actor:'a',session:'s',revision:7,
    expectedActor:'a',expectedSession:'s',minimumRevision:7,deliveryBefore:'1000000000',deliveryAfter:'1001000000',
    observedBefore:'1249000000',observedAfter:'1250000000',uncertaintyNanoseconds:'0',drawFrame:1,
    drawn:true,visible:true,insideCamera:true,imageBound:true,provenance:'physical-SendInput',
    eventAcknowledged:true,transformChanged:true};
  assert.equal(evaluate(row).upperNanoseconds,'250000000');
  for(const delta of [{observedAfter:'1250000001'},{uncertaintyNanoseconds:'1'},{clock:'peer-uptime'},
    {actor:'foreign'},{session:'old'},{revision:6},{drawn:false},{visible:false},{insideCamera:false},
    {drawFrame:0},{imageBound:false},{eventAcknowledged:false},{transformChanged:false}])assert.throws(()=>evaluate({...row,...delta}));
});
test('stopped agreement includes exact distance and time equality',()=>{
  const evaluate=require('./evidence.cjs').stopped;
  const row={clock:'coordinator-hrtime',actor:'a',expectedActor:'a',session:'s',expectedSession:'s',revision:2,minimumRevision:2,
    positionSubunits:50,hostPositionSubunits:0,releaseBefore:'1000000000',observedAfter:'1500000000',uncertaintyNanoseconds:'0'};
  assert.equal(evaluate(row).positionSubunits,50);
  for(const delta of [{positionSubunits:51},{observedAfter:'1500000001'},{uncertaintyNanoseconds:'1'},
    {uncertaintyNanoseconds:'-1'},{actor:'other'},{session:'old'},{revision:1},{clock:'peer-uptime'},{positionSubunits:NaN}])assert.throws(()=>evaluate({...row,...delta}));
});
test('draw stream polling retains partial Unicode and refuses truncation',()=>{
  const path=require('node:path'),crypto=require('node:crypto'),{lines}=require('./rendered.cjs');
  const directory=path.resolve(__dirname,'../../.phase3/controls',crypto.randomUUID());fs.mkdirSync(directory,{recursive:true});
  const file=path.join(directory,'draws.ndjson'),row=Buffer.from(JSON.stringify({frame:1,label:'雪'})+'\n');
  const split=row.indexOf(Buffer.from('雪'))+1;fs.writeFileSync(file,row.subarray(0,split));assert.deepEqual(lines(file),[]);
  fs.appendFileSync(file,row.subarray(split));assert.deepEqual(lines(file),[{frame:1,label:'雪'}]);
  fs.appendFileSync(file,JSON.stringify({frame:2})+'\n');assert.equal(lines(file).at(-1).frame,2);
  fs.writeFileSync(file,'');assert.throws(()=>lines(file),/TRUNCATED/);
});
