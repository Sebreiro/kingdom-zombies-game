'use strict';

const assert = require('node:assert/strict');
const path = require('node:path');
const {spawnSync} = require('node:child_process');

const ROOT = path.resolve(__dirname, '../../..');
const check = require('../check.cjs');

function rejectsMissingCompletion() {
  const result = spawnSync(process.execPath, [
    'tools/phase4/check.cjs', '--verify-completion',
    '.phase4/missing-phase4-runner-negative-fixture/completion.json'
  ], {cwd:ROOT, encoding:'utf8', windowsHide:true, timeout:15000});
  assert.ifError(result.error);
  assert.equal(result.status, 1, result.stderr);
  assert.match(result.stderr, /EVIDENCE_PATH_MISSING/, 'Completion dispatch must reject a missing completion record');
}

if (require.main === module) {
  require('node:test').test('completion dispatch rejects missing completion evidence', rejectsMissingCompletion);
} else {
  check.registerCase('runner.prelaunch', {
    group:'runner', title:'Current Phase 4 review and prelaunch gate', importance:'Critical',
    rationale:'Running an unreviewed build invalidates all rendered acceptance',
    needsGameWindow:false, runGroup:'G0-runner', order:300,
    requirements:['ASSET-01','ASSET-02','ASSET-03','ASSET-04','ASSET-05'],
    acceptance:['AC-17','AC-19','AC-20'], tags:['source-only','review','selection'],
    setup:'Current saved case catalog and an isolated missing completion reference',
    actions:['Invoke the real completion command against a missing record'],
    expected:'Missing completion evidence is explicitly rejected',
    reset:'Preserve existing review, selection and proof files', evidence:['result.json']
  }, rejectsMissingCompletion);
}
