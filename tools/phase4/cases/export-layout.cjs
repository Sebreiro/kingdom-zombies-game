'use strict';
const runner=require('../check.cjs');
function selfTest(){
  const {test}=require('node:test'),assert=require('node:assert/strict');
  test('export.layout registers its bounded source-only compiler procedure',()=>{
    const item=runner.cases.get('export.layout');
    assert.ok(item,'export.layout must have an executable saved case before export can resume');
    assert.equal(item.needsGameWindow,false);assert.equal(item.runGroup,'G0');
    assert.equal(item.importance,'Critical');assert.equal(typeof item.run,'function');
  });
}
module.exports={selfTest};
if(require.main===module&&process.argv[2]==='--self-test')selfTest();
