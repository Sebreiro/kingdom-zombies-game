'use strict';

const assert = require('node:assert/strict');
const test = require('node:test');
const { discoveredTests, verifyTestResults } = require('./basic.cjs');
const { verifyEdges } = require('./edge-check.cjs');

const spec = [
  '## Edge Coverage',
  '| Edge | Category | Requirement | Status | Verification | Resolution |',
  '| E001 | empty | R01 | resolved | explicit | Empty map produces no result. |',
  '| E002 | bound | R02 | deferred | Phase 12 | Later behavior. |',
].join('\n');
const edge = (methodName, typeName = 'FixtureTests') => ({ type: { methodName, typeName, namespace: 'Game.Tests' } });

test('the edge check counts one discovered test method and carries deferred rows forward', () => {
  const check = edge('Phase06_E001_EmptyMap');
  assert.deepEqual(verifyEdges('06', spec, [check, check]), {
    phase: 6, expected: 1, methods: { E001: 'Game.Tests.FixtureTests.Phase06_E001_EmptyMap' },
  });
});

test('the edge check rejects a missing and a duplicate tested edge', () => {
  assert.throws(() => verifyEdges('06', spec, []), /EDGE_TEST_MISSING E001/);
  assert.throws(() => verifyEdges('06', spec, [edge('Phase06_E001_One'), edge('Phase06_E001_Two')]),
    /EDGE_TEST_DUPLICATE E001/);
  assert.throws(() => verifyEdges('06', spec, [edge('Phase06_E002_Later')]), /EDGE_TEST_UNEXPECTED E002/);
});

test('test execution is reconciled against the named discovery records', () => {
  const row = { uid: 'a'.repeat(64), displayName: 'Game.Tests.Fixture.Success' };
  const discovered = discoveredTests(JSON.stringify({ schemaVersion: 1, tests: [row] }));
  const result = (name, status = 'passed', skipped = 0) => ({
    results: {
      summary: { tests: 1, passed: 1, failed: 0, pending: 0, skipped, other: 0 },
      tests: [{ name, status }],
    },
  });
  assert.doesNotThrow(() => verifyTestResults(discovered, result(row.displayName)));
  assert.throws(() => verifyTestResults(discovered, result('Game.Tests.Fixture.Different')), /TEST_RESULT_MEMBERSHIP/);
  assert.throws(() => verifyTestResults(discovered, result(row.displayName, 'skipped', 1)), /TEST_NON_PASS/);
  assert.throws(() => discoveredTests('{"schemaVersion":1,"tests":[]}'), /TEST_DISCOVERY_EMPTY/);
});
