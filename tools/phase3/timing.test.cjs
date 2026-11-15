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
