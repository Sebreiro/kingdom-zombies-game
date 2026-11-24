'use strict';

const assert = require('node:assert/strict');
const path = require('node:path');
const {test} = require('node:test');
const {changedVerificationGroups, loadCatalog, select, validateCatalog, verificationRows} = require('./catalog.cjs');

const root = path.resolve(__dirname, '../../../..');
const ids = rows => rows.map(row => row.id);
const rendered = catalog => catalog.cases.filter(row => row.tier === 3 && row.status === 'required');

test('the independent source and legacy definitions account for the exact required rendered cases', () => {
  const catalog = loadCatalog(root);
  const source = catalog.cases.filter(row => /^A-\d\d$/.test(row.id));
  assert.deepEqual(ids(source), Array.from({length: 47}, (_, index) => 'A-' + String(index + 1).padStart(2, '0')));
  assert.equal(rendered(catalog).filter(row => row.adapter === 'gameplay-checks').length, 9);
  assert.equal(rendered(catalog).filter(row => row.adapter === 'lan-checks').length, 12);
  assert.deepEqual(ids(rendered(catalog).filter(row => row.adapter === 'asset-pipeline')),
    ['asset-pipeline.rendered.hub-navigation']);
  assert.equal(catalog.cases.filter(row => row.adapter === 'asset-pipeline' && row.tier === 3 &&
    row.owner === 11 && row.status === 'planned').length, 17);
  const solo = catalog.runGroups.find(group => group.id === 'gameplay-checks.rendered');
  assert.equal(solo.cases[0], 'gameplay-checks.rendered.menu');
  assert.equal(solo.cases.at(-1), 'gameplay-checks.rendered.quit');
});

test('PR scope has exactly four checks and tier 1 runs the one pure test command', () => {
  const catalog = loadCatalog(root);
  assert.deepEqual(ids(select(catalog, {mode: 'pr'}).selected),
    ['basic.strict-build', 'basic.repository', 'basic.pure', 'basic.headless']);
  assert.deepEqual(ids(select(catalog, {mode: 'tier', value: '1'}).selected), ['basic.pure']);
});

test('Phase 10 tracks distinct planned release procedures without replacing A-47', () => {
  const catalog = loadCatalog(root);
  const procedures = catalog.cases.filter(row => row.group === 'release.environments');
  assert.deepEqual(procedures.map(row => [row.id, row.owner, row.status, row.requirements[0], row.sourceIds[0]]), [
    ['release.physical-lan', 10, 'planned', 'REL-02', 'A-38'],
    ['release.reference-pc', 10, 'planned', 'REL-04', 'A-47'],
    ['release.endurance', 10, 'planned', 'REL-05', 'A-47']
  ]);
  assert.equal(new Set(procedures.map(row => row.runGroup)).size, 3);
  assert.equal(catalog.cases.filter(row => row.id === 'A-47').length, 1);
  assert(!select(catalog, {mode: 'full'}).selected.some(row => row.group === 'release.environments'));
});

test('headless LAN process groups declare their independent roles and keep the existing suite selectors', () => {
  const catalog = loadCatalog(root);
  const native = catalog.cases.filter(row => row.adapter === 'lan-checks' && row.tier === 2);
  assert.deepEqual(native.map(row => [row.legacyId, row.roles]), [
    ['network.tracer', ['host', 'guest']],
    ['network.four-peer', ['four-host', 'four-guest1', 'four-guest2', 'challenger1', 'challenger2']],
    ['network.lifecycle', ['life-host', 'life-guest1', 'life-guest2', 'life-replacement']],
    ['network.idle-poll', ['idle-host']],
    ['network.discovery-profile', ['discovery-host', 'profile-before-restart', 'profile-after-restart']],
    ['runner.tracer', ['host', 'guest']]
  ]);
  assert.equal(new Set(native.map(row => row.runGroup)).size, native.length);
  for (const scenario of native) {
    assert.equal(scenario.needsGameWindow, false);
    assert(scenario.freshProcessReason);
    assert(scenario.evidence.some(item => item.includes('durationMilliseconds')));
    assert.deepEqual(ids(select(catalog, {mode: 'group', value: scenario.runGroup}).selected), [scenario.id]);
    assert(select(catalog, {mode: 'group', value: scenario.group}).selected.some(row => row.id === scenario.id));
  }
});

test('full scope includes active low-priority checks without selecting planned procedures', () => {
  const catalog = loadCatalog(root);
  catalog.cases.find(row => row.id === 'lan-checks.core.authority').importance = 'Low';
  const {selected, omitted} = select(catalog, {mode: 'full'});
  assert.equal(selected.length, catalog.cases.filter(row => row.status === 'required').length);
  assert(selected.some(row => row.id === 'lan-checks.core.authority' && row.importance === 'Low'));
  assert(omitted.some(row => row.id === 'TEST-12' && row.status === 'planned'));
});

test('phase-end selection includes the unchanged smoke and pending Critical stays planned', () => {
  const {selected, omitted} = select(loadCatalog(root), {mode: 'phase-end', changes: []});
  assert(selected.some(row => row.id === 'gameplay-checks.rendered.tracer' && row.reason === 'Critical smoke'));
  assert(selected.some(row => row.id === 'lan-checks.rendered.walkers' && row.reason === 'Critical smoke'));
  assert(selected.some(row => row.id === 'asset-pipeline.world.replacement' && row.reason === 'due in this phase'));
  assert(omitted.some(row => row.id === 'A-38' && row.status === 'planned'));
  assert(omitted.some(row => row.id === 'asset-pipeline.items.wearables' && row.status === 'not-selected'));
});

test('direct changed Low and transitive affected consumer both enter phase-end scope', () => {
  const catalog = loadCatalog(root);
  const low = catalog.cases.find(row => row.id === 'asset-pipeline.items.wearables');
  low.importance = 'Low';
  const {selected} = select(catalog, {mode: 'phase-end', value: 6,
    changes: [low.inputs[0], 'src/godot-game/core/Movement.cs']});
  assert(selected.some(row => row.id === low.id && row.reason === 'declared input changed'));
  assert(selected.some(row => row.id === 'lan-checks.rendered.profiles' && row.reason.startsWith('affected by')));
});

test('a rule input triggers its legacy group and gameplay-only change triggers none', () => {
  const catalog = loadCatalog(root);
  assert(select(catalog, {mode: 'phase-end', changes: ['src/godot-game/tools/code-rules/DeclarationAnalyzer.cs']})
    .selected.some(row => row.id === 'verification.rules'));
  assert(!select(catalog, {mode: 'phase-end', changes: ['src/godot-game/game/foundation/FoundationRoot.cs']})
    .selected.some(row => row.adapter === 'verification'));
});

test('pending ownership changes exercise the selector without expanding all Phase 1 proof groups', () => {
  const current = require('../verification/required-cases.json');
  const transferred = [
    'ac-02.complete', 'ac-06.complete', 'ac-07.complete', 'ac-09.complete',
    'R-49.positive', 'R-49.negative', 'R-82.positive', 'R-82.negative'
  ];
  assert.deepEqual(ids(current.cases.filter(row => transferred.includes(row.id))).sort(), transferred.sort());
  for (const row of current.cases.filter(row => transferred.includes(row.id))) {
    assert.equal(row.acceptancePhase, 11);
    assert.equal(row.implementation, 'pending');
  }
  const prior = structuredClone(current);
  for (const row of prior.cases.filter(row => transferred.includes(row.id))) row.acceptancePhase = 5;
  assert.deepEqual([...changedVerificationGroups(prior, current)], []);
  const rule = prior.cases.find(row => row.group === 'rules' && row.implementation === 'csharp');
  rule.expectedDiagnostic = 'CHANGED_EXPECTED_DIAGNOSTIC';
  assert.deepEqual([...changedVerificationGroups(prior, current)], ['rules']);
});

test('an unexpected pending Phase 1 declaration remains in required group selection', () => {
  const current = require('../verification/required-cases.json');
  const unexpected = {id: 'unexpected.required-proof', group: 'rules', implementation: 'pending'};
  const modified = {...current, cases: [...current.cases, unexpected]};
  const [rules] = verificationRows(modified).filter(row => row.legacyId === 'rules');
  assert.equal(rules.status, 'required');
  assert(rules.legacyIds.includes(unexpected.id));
  assert(!rules.deferredIds.includes(unexpected.id));
  assert.deepEqual([...changedVerificationGroups(current, modified)], ['rules']);
});

test('unknown impact and empty, duplicate, conflicting or malformed selections fail', () => {
  const catalog = loadCatalog(root);
  assert.throws(() => select(catalog, {mode: 'phase-end', changes: ['src/unknown/component.cs']}), /UNKNOWN_IMPACT/);
  assert.throws(() => select(catalog, {mode: 'case', value: 'A-47'}), /SELECTION_PLANNED/);
  assert.throws(() => select(catalog, {mode: 'case', value: 'basic.pure,basic.pure'}), /SELECTION_CASE/);
  assert.throws(() => select(catalog, {mode: 'case', value: 'basic.pure,A-47'}), /SELECTION_PLANNED/);
  assert.throws(() => select(catalog, {mode: 'full', value: '3'}), /SELECTION_CONFLICT/);
  assert.throws(() => select(catalog, {mode: 'phase-end', value: '5.0'}), /SELECTION_PHASE/);
});

test('catalog validation rejects malformed importance, duplicate order, references and missing source IDs', () => {
  for (const [change, message] of [
    [catalog => catalog.cases[0].importance = 'urgent', /CATALOG_IMPORTANCE/],
    [catalog => catalog.cases[0].order = catalog.cases[1].order, /CATALOG_RUN_ORDER/],
    [catalog => catalog.cases[0].group = 'rogue', /CATALOG_DANGLING_GROUP/],
    [catalog => catalog.cases[0].requirements = ['TEST-99'], /CATALOG_REQUIREMENT/],
    [catalog => catalog.cases[0].sourceIds = ['A-99'], /CATALOG_SOURCE_REFERENCE/],
    [catalog => catalog.cases[0].procedure = 'docs/missing.md#scope', /CATALOG_PROCEDURE/],
    [catalog => catalog.cases[0].procedure = '../reference.md#scope', /CATALOG_PROCEDURE/],
    [catalog => catalog.cases[0].procedure = 'C:/reference.md', /CATALOG_PROCEDURE/],
    [catalog => catalog.cases[0].actions = [], /CATALOG_METADATA/],
    [catalog => catalog.sources.delete('A-46'), /CATALOG_SOURCE_A01_A47/]
  ]) {
    const catalog = loadCatalog(root);
    change(catalog);
    assert.throws(() => validateCatalog(catalog), message);
  }
});

test('a source support rebind is distinct from a content helper behavior change', () => {
  const {supportStampOnly} = require('./catalog.cjs');
  const before = "[base+'foundation/FoundationRoot.cs',100,'" + 'a'.repeat(64) + "']\nreference(file)";
  const after = before.replace('100', '200').replace('a'.repeat(64), 'b'.repeat(64));
  assert.equal(supportStampOnly(before, after), true);
  assert.equal(supportStampOnly(before, before), false);
  assert.equal(supportStampOnly(before, after.replace('reference(file)', 'reference(other)')), false);
});
