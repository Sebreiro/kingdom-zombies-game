'use strict';

const fs = require('node:fs');
const path = require('node:path');
const {execFileSync} = require('node:child_process');

const tools = 'src/godot-game/tools/';
const tests = 'src/godot-game/tests/verification/';
const requirementsPath = '.planning/REQUIREMENTS.md';
const specificationPath = 'specs/0.1/initial-spec.md';
const catalogPath = tools + 'game-tests/catalog.json';
const verificationPath = tools + 'verification/required-cases.json';
const assetContentPath = tools + 'asset-pipeline/content.cjs';
const gameContent = 'src/godot-game/game/content/';
const liveAssetInputs = {
  'ui-camp.cjs': [gameContent + 'packs/ui-camp.json', gameContent + 'packs/ui-map-warnings.json'],
  'ui-inventory.cjs': [gameContent + 'packs/ui-inventory.json', gameContent + 'packs/ui-legacy-stash.json'],
  'corpses.cjs': [gameContent + 'packs/player-identities.json', gameContent + 'fixtures/crowds.json'],
  'text-data.cjs': [gameContent + 'packs/text-data.json', gameContent + 'packs/compatibility.json',
    gameContent + 'packs/delivery.json', 'docs/content/fields.md']
};
const normalizedSupportConsumers = new Set([
  'content.source-relations', 'content.domain-relations', 'content.census', 'content.native-permissions',
  'source.publication', 'source.packaged-expectations', 'export.hub-consumer', 'export.portability', 'hub.source'
]);
const importanceLevels = ['Critical', 'High', 'Normal', 'Low'];
const scope = ['verification', 'gameplay-checks', 'lan-checks', 'asset-pipeline'];
const idPattern = /^[A-Za-z][A-Za-z0-9._-]*$/;
const deferredAssetGates = new Set([
  'runner.prelaunch', 'runner.selection', 'acceptance.judgments', 'acceptance.artifacts'
]);
const deferredCertification = new Set([
  'ac-02.complete', 'ac-06.complete', 'ac-07.complete', 'ac-09.complete',
  'R-49.positive', 'R-49.negative', 'R-82.positive', 'R-82.negative'
]);
const lanHeadlessIsolation = {
  'network.tracer': 'A fresh host and guest prove independent native lifetimes and baseline installation.',
  'network.four-peer': 'A fresh session and two simultaneous challengers isolate the final-slot admission race.',
  'network.lifecycle': 'The scenario deliberately ends and replaces owned guest and host processes.',
  'network.idle-poll': 'An idle host without any guests isolates native ENet polling and update progress.',
  'runner.tracer': 'Fresh host and guest observations establish the proof runner’s independent evidence.'
};
const criticalSmoke = new Set([
  'gameplay-checks.rendered.tracer',
  'lan-checks.rendered.walkers'
]);
const phase2Scenarios = '.planning/phases/02-playable-foundation/02-SCENARIOS.md';
const phase3Scenarios = '.planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md';
const phase4Scenarios = '.planning/phases/04-asset-production/04-SCENARIOS.md';
const pinnedRecipeConsumers = new Set(['source.tracer', 'source.tracer-failures', 'audio.actions', 'audio.bank']);

const ruleInputs = {
  rules: ['docs/code/', tools + 'code-rules/', tests + 'Analyzers/'],
  graph: ['docs/architecture/', tests + 'ArchitectureRuleTests.cs', tests + 'GraphFixture.cs',
    'src/godot-game/KingdomZombies.slnx'],
  engine: ['docs/code/engine-generated-spans.json', 'docs/code/generated-callsite-adaptations.json',
    tests + 'EngineGeneratedTests.cs', tests + 'EngineRun.cs', 'src/godot-game/game/proofs/'],
  runner: [tools + 'verification/', tests + 'RunnerContractTests.cs', tests + 'RunnerProcessFixture.cs'],
  pins: [tools + 'verification/toolchain.json', tools + 'verification/bootstrap.cjs',
    'src/godot-game/global.json', 'src/godot-game/NuGet.config', 'src/godot-game/Directory.Packages.props'],
  platform: [tools + 'verification/toolchain.json'],
  register: ['docs/code/exemptions.json', 'docs/code/register.json', 'docs/code/engine-generated-spans.json',
    tests + 'ExemptionTests.cs', tests + 'ExemptionValidator.cs', tests + 'RegisterTests.cs', tests + 'RegisterValidator.cs'],
  docs: [tests + 'DocumentationFixture.cs', tests + 'DocumentationValidator.cs',
    'docs/code/README.md', 'docs/architecture/README.md'],
  state: [tests + 'StateInputTests.cs', 'src/godot-game/game/proofs/']
};
const sharedRuleInputs = ['.editorconfig', 'src/godot-game/Directory.Build.props',
  'src/godot-game/Directory.Build.targets', tests + 'KingdomZombies.Phase1.Tests.csproj',
  tests + 'packages.lock.json'];

function requireValue(condition, message) {
  if (!condition) throw new Error(message);
}

function tableRows(text, pattern) {
  const rows = new Map();
  for (const [, id, ...cells] of text.matchAll(pattern)) {
    requireValue(!rows.has(id), 'CATALOG_DUPLICATE_SOURCE ' + id);
    rows.set(id, cells);
  }
  return rows;
}

function readSources(read) {
  const specification = read(specificationPath);
  const crosswalk = read(requirementsPath);
  const sources = tableRows(specification, /^\| (A-\d\d) \| ([^|\r\n]+) \| ([^|\r\n]+) \|\s*$/gm);
  const mappings = tableRows(crosswalk, /^\| (A-\d\d) \| ([^|\r\n]+) \|\s*$/gm);
  const owners = tableRows(crosswalk, /^\| ([A-Z]+-\d\d) \| Phase (\d+) \| [^|\r\n]+ \|\s*$/gm);
  const expected = Array.from({length: 47}, (_, index) => 'A-' + String(index + 1).padStart(2, '0'));
  requireValue(sources.size === expected.length && mappings.size === expected.length &&
    expected.every(id => sources.has(id) && mappings.has(id)), 'CATALOG_SOURCE_A01_A47');
  const requirementOwners = new Map([...owners].map(([id, [phase]]) => [id, Number(phase)]));
  for (const id of requirementOwners.keys()) requireValue(idPattern.test(id), 'CATALOG_REQUIREMENT_ID ' + id);
  return {sources, mappings, requirementOwners};
}

function row(data) {
  return {status: 'required', requirements: [], needsGameWindow: false, tier: 1, ...data};
}

function legacy(adapter, phase, item, data) {
  return row({
    id: adapter + '.' + item.id,
    adapter,
    legacyId: item.id,
    group: adapter + '.' + item.group,
    runGroup: adapter + '.' + (item.runGroup ?? item.group),
    owner: phase,
    title: item.title ?? item.id,
    requirements: item.requirements,
    procedure: tools + adapter + '/check.cjs',
    setup: item.setup,
    actions: item.actions,
    expected: item.expected,
    reset: item.reset,
    roles: item.roles ?? item.processRoles ?? ['test process'],
    evidence: item.evidence,
    ...data
  });
}

function verificationRows(manifest) {
  return Object.entries(ruleInputs).map(([name, inputs], order) => {
    const entries = manifest.cases.filter(item => item.group === name);
    requireValue(entries.length > 0, 'CATALOG_EMPTY_VERIFICATION_GROUP ' + name);
    const active = entries.filter(item => !isDeferredCertification(item));
    return row({
      id: 'verification.' + name, adapter: 'verification', legacyId: name,
      legacyIds: active.map(item => item.id), deferredIds: entries.filter(item => !active.includes(item)).map(item => item.id),
      group: 'verification.' + name, runGroup: 'verification.' + name, order: order + 1,
      owner: active.length ? 1 : 11, status: active.length ? 'required' : 'planned',
      title: 'Run verification suite group ' + name,
      requirements: active.length ? ['RULE-01', 'RULE-02', 'RULE-03', 'RULE-04', 'RULE-05', 'RULE-06', 'RULE-07'] : ['TEST-12'],
      tier: ['engine', 'platform'].includes(name) ? 2 : 1,
      importance: 'High', rationale: 'Detects regressions in the enforcement or proof inputs for ' + name + '.',
      tags: ['rule-suite'], inputs: [...new Set([...sharedRuleInputs, ...inputs])],
      procedure: tools + 'verification/check.cjs',
      setup: 'Use pinned tools, the verified feed and a checkout with a clean strict build.',
      actions: ['Run each active declared check in the legacy suite group.'],
      expected: 'Every selected declaration produces one successful, reconciled proof.',
      reset: 'The verification suite owns and isolates each proof and releases its lock on exit.',
      roles: ['verification process'], evidence: ['verification summary', 'test reports', 'subject logs']
    });
  });
}

function isDeferredCertification(item) {
  return deferredCertification.has(item.id) && item.implementation === 'pending';
}

function gameplayRows(manifest) {
  return manifest.cases.map((item, index) => {
    const rendered = item.implementation === 'native';
    const id = 'gameplay-checks.' + item.id;
    const critical = criticalSmoke.has(id);
    return legacy('gameplay-checks', 2, item, {
      title: rendered ? 'Rendered solo ' + item.id.split('.').at(-1) : 'Solo foundation ' + item.id,
      order: item.id === 'rendered.menu' ? 1 :
        item.id === 'rendered.quit' ? manifest.cases.length + 2 : index + 2,
      requirements: rendered ? ['BASE-01', 'BASE-02', 'BASE-03'] : ['BASE-01', 'BASE-02'],
      acceptancePhase: rendered ? 5 : undefined,
      tier: rendered ? 3 : 1, needsGameWindow: rendered,
      importance: critical ? 'Critical' : rendered ? 'High' : 'Normal',
      rationale: critical ? 'Launch, menu, entry or character movement failing stops play.' :
        'Verifies a separate supported solo foundation behavior.',
      tags: rendered ? ['foundation', 'solo-rendered'] : ['foundation', 'solo-proof'],
      setup: rendered ? 'Use the reviewed strict build and one owned 1280×720 Windows game session.' :
        'Use the restored verification test assembly.',
      actions: rendered ? ['Follow the saved production input steps for ' + item.id + '.'] :
        ['Run the named proof method against production foundation behavior.'],
      expected: rendered ? 'All declared scene, control, state and image assertions pass.' :
        'The named test executes exactly once and publishes its required proof.',
      reset: rendered ? 'Return to the menu and release owned keys between scenarios. Quit runs last.' :
        'Use fresh test inputs and release owned resources.',
      roles: rendered ? ['solo'] : ['test process'],
      evidence: rendered ? ['native input', 'observed state', 'component logs', ...item.images] : ['proof', 'test report', 'log'],
      scenario: phase2Scenarios,
      freshProcessReason: rendered ? 'The solo group isolates its profile and desktop from LAN and inspection.' : undefined
    });
  });
}

function lanRows(manifest) {
  return manifest.cases.map((item, index) => {
    const rendered = item.needsGameWindow;
    const native = item.implementation === 'native-headless';
    const critical = criticalSmoke.has('lan-checks.' + item.id);
    return legacy('lan-checks', 3, item, {
      runGroup: native ? 'lan-checks.headless.' + item.id : 'lan-checks.' + item.runGroup,
      order: native ? 1 : rendered ? item.order : index + 1,
      acceptancePhase: rendered ? 5 : undefined,
      tier: rendered ? 3 : native ? 2 : 1,
      needsGameWindow: rendered,
      importance: critical ? 'Critical' : rendered || native ? 'High' : 'Normal',
      rationale: critical ? 'A host and three guests must be able to play and observe the same state.' : item.rationale,
      tags: rendered ? ['foundation', 'lan', 'lan-rendered'] : ['foundation', 'lan', 'lan-proof'],
      scenario: phase3Scenarios,
      setup: item.id === 'network.discovery-profile' ?
        'Use an owned pinned headless host and two successive profile processes sharing the same isolated profile root.' : item.setup,
      evidence: native ? [...item.evidence, 'subject durationMilliseconds and per-role launch records'] : item.evidence,
      freshProcessReason: native ? item.freshProcessReason ?? lanHeadlessIsolation[item.id] :
        !rendered ? undefined : item.runGroup === 'G2' ?
        'Discovery and browser errors use roles and state distinct from live four-player play.' :
        item.runGroup === 'G4' ? 'Reuse live G3 peers if present. An isolated G4 replay needs four fresh processes.' :
        'The full play group uses an owned host and three guest processes.'
    });
  });
}

function assetRows(manifest, handoff) {
  const handoffPhases = new Map(Object.entries(handoff).flatMap(([phase, ids]) =>
    ids.map(id => [id, Number(phase)])));
  const rendered = manifest.renderedCases.map(item => {
    const originalOwner = handoffPhases.get(item.id);
    requireValue(originalOwner, 'CATALOG_MISSING_ASSET_HANDOFF ' + item.id);
    const smoke = item.id === 'rendered.hub-navigation';
    const phase06 = originalOwner === 6;
    const owner = smoke ? 5 : originalOwner === 4 ? 11 : originalOwner;
    return legacy('asset-pipeline', owner, item, {
      status: smoke || phase06 ? 'required' : 'planned',
      acceptancePhase: smoke ? 5 : undefined,
      tier: 3, needsGameWindow: true, order: item.order,
      importance: smoke ? 'High' : item.importance === 'Critical' ? 'High' : item.importance,
      rationale: item.rationale,
      tags: ['inspection', 'asset-rendered'],
      scenario: phase4Scenarios,
      setup: smoke ? 'Launch the current reviewed, imported source inspection hub in a 1280×720 owned window.' : item.setup,
      actions: smoke ? ['Visit all eight categories using real input.', 'Read resource and category state and capture each category.'] : item.actions,
      expected: smoke ? 'Every category is reachable, has current observed state and supplies a decoded screenshot for P1–P4 review.' : item.expected,
      freshProcessReason: 'The inspection hub or packaged subject needs its own scene, profile and process.',
      ...(phase06 ? {
        adapter: 'scenario', group: 'phase06.inspection', runGroup: 'phase06.inspection',
        driver: 'src/godot-game/tests/scenarios/phase06-inspection.cjs', phaseEnd: true,
        tags: ['phase06', 'inspection', 'asset-rendered']
      } : {})
    });
  });
  const source = manifest.cases.map(item => {
    const phase11 = deferredAssetGates.has(item.id);
    const module = item.sourceBinding?.module;
    const inputs = module ? [module, ...(liveAssetInputs[path.posix.basename(module)] ?? [])] : [];
    if (['layouts.identity', 'layouts.identity-integrity', 'layouts.application-icon', 'text.catalog',
      'text.support-catalog'].includes(item.id)) {
      inputs.push('src/godot-game/game/content/packs/ui-identity.json');
    }
    if (pinnedRecipeConsumers.has(item.id)) inputs.push(tools + 'asset-pipeline/run-pinned-recipe.py');
    return legacy('asset-pipeline', phase11 ? 11 : 4, item, {
      status: phase11 ? 'planned' : 'required',
      acceptancePhase: item.id === 'world.replacement' ? 5 : undefined,
      tier: 2, order: item.order + 1,
      importance: item.importance === 'Critical' ? 'High' : item.importance,
      rationale: item.rationale,
      tags: [...new Set(['asset-source', 'asset-' + item.group,
        ...(normalizedSupportConsumers.has(item.id) ? ['asset-live-support'] : [])])],
      inputs,
      scenario: phase4Scenarios
    });
  });
  return [...source, ...rendered];
}

function acceptanceRows(sources, mappings, owners) {
  return [...sources].map(([id, [setup, expected]], index) => {
    const requirements = mappings.get(id)[0].trim().split(/,\s*/);
    requireValue(requirements.every(requirement => owners.has(requirement)), 'CATALOG_UNKNOWN_REQUIREMENT ' + id);
    return row({
      id, title: setup.trim(), requirements, owner: Math.max(...requirements.map(key => owners.get(key))),
      adapter: 'procedure', tier: 3, status: 'planned', needsGameWindow: true,
      group: 'release.acceptance', runGroup: 'release.acceptance', order: index + 1,
      importance: ['A-01', 'A-38', 'A-47'].includes(id) ? 'Critical' : 'High',
      rationale: 'Full product acceptance needs the completed feature set and its release environment.',
      tags: ['release'], procedure: specificationPath + '#143-observable-acceptance-scenarios',
      setup: 'Use the owning phase’s approved environment and the saved acceptance scenario.',
      actions: [setup.trim()], expected: expected.trim(),
      reset: 'Restore the starting fixture or use a fresh run as the source procedure requires.',
      roles: id === 'A-38' || id === 'A-47' ? ['host', 'guest1', 'guest2', 'guest3'] : ['player'],
      evidence: ['observations and logs from the actual owning-phase procedure'],
      freshProcessReason: 'Release procedure uses its declared environment and starting campaign.'
    });
  });
}

function loadCatalog(root) {
  const read = file => fs.readFileSync(path.join(root, file), 'utf8');
  const data = JSON.parse(read(catalogPath));
  requireValue(data.schemaVersion === 1 && Array.isArray(data.cases) && data.cases.length > 0 &&
    validList(data.groups) && validList(data.runGroups), 'CATALOG_SCHEMA');
  const {sources, mappings, requirementOwners} = readSources(read);
  const manifests = Object.fromEntries(scope.map(name =>
    [name, JSON.parse(read(tools + name + '/required-cases.json'))]));
  const cases = [
    ...data.cases.map(item => {
      requireValue(data.groups.includes(item.group) && data.runGroups.includes(item.runGroup),
        'CATALOG_DANGLING_GROUP ' + item.id);
      return row(item);
    }),
    ...verificationRows(manifests.verification),
    ...gameplayRows(manifests['gameplay-checks']),
    ...lanRows(manifests['lan-checks']),
    ...assetRows(manifests['asset-pipeline'], JSON.parse(read(tools + 'asset-pipeline/handoff.json'))),
    ...acceptanceRows(sources, mappings, requirementOwners)
  ];
  const groups = new Map();
  const runGroups = new Map();
  for (const item of cases) {
    if (!groups.has(item.group)) groups.set(item.group, {id: item.group, adapter: item.adapter, cases: []});
    groups.get(item.group).cases.push(item.id);
    if (!runGroups.has(item.runGroup)) runGroups.set(item.runGroup, []);
    runGroups.get(item.runGroup).push(item);
  }
  const runs = [...runGroups].map(([id, members]) => ({id,
    cases: members.sort((left, right) => left.order - right.order).map(item => item.id),
    roles: [...new Set(members.flatMap(item => item.roles))]
  }));
  const catalog = {cases, groups: [...groups.values()], runGroups: runs, requirementOwners, sources, impact: data.impact,
    impactsByTag: data.impactsByTag, neutralPaths: data.neutralPaths};
  Object.defineProperties(catalog, {
    root: {value: root},
    verificationManifest: {value: manifests.verification}, manifests: {value: manifests}, declared: {value: data}
  });
  validateCatalog(catalog);
  return catalog;
}

function validText(value) {
  return typeof value === 'string' && value.trim().length > 0;
}

function validList(value) {
  return Array.isArray(value) && value.length > 0 && value.every(validText) && new Set(value).size === value.length;
}

function existingProcedure(repository, procedure) {
  const [file] = procedure.split('#', 1);
  if (/[:\\\u0000]/.test(file) || file.split('/').some(segment => !segment || segment === '.' || segment === '..')) {
    return false;
  }
  try {
    const target = fs.realpathSync(path.join(repository, file));
    const relative = path.relative(repository, target);
    return relative !== '..' && !relative.startsWith('..' + path.sep) &&
      !path.isAbsolute(relative) && fs.statSync(target).isFile();
  } catch (error) {
    if (!['ENOENT', 'ENOTDIR'].includes(error.code)) throw error;
    return false;
  }
}

function validateCatalog(catalog) {
  const ids = new Set(), runOrders = new Set(), groupIds = new Set();
  const repository = fs.realpathSync(catalog.root);
  const checkedProcedures = new Set();
  requireValue(catalog.impact && catalog.impactsByTag && validList(catalog.neutralPaths) &&
    Object.values(catalog.impact).every(validList) && Object.values(catalog.impactsByTag).every(validList), 'CATALOG_IMPACT');
  for (const group of catalog.groups) {
    requireValue(idPattern.test(group.id) && !groupIds.has(group.id) && validList(group.cases),
      'CATALOG_GROUP ' + group.id);
    groupIds.add(group.id);
  }
  const runGroupIds = new Set();
  for (const group of catalog.runGroups) {
    requireValue(idPattern.test(group.id) && !runGroupIds.has(group.id) && validList(group.cases) && validList(group.roles),
      'CATALOG_RUN_GROUP ' + group.id);
    runGroupIds.add(group.id);
  }
  for (const item of catalog.cases) {
    requireValue(typeof item.id === 'string' && idPattern.test(item.id) && !ids.has(item.id), 'CATALOG_CASE_ID ' + item.id);
    ids.add(item.id);
    for (const field of ['title', 'rationale', 'adapter', 'procedure', 'setup', 'reset']) {
      requireValue(validText(item[field]), 'CATALOG_METADATA ' + item.id + ' ' + field);
    }
    if (!checkedProcedures.has(item.procedure)) {
      requireValue(existingProcedure(repository, item.procedure), 'CATALOG_PROCEDURE ' + item.id);
      checkedProcedures.add(item.procedure);
    }
    for (const field of ['requirements', 'tags', 'actions', 'roles', 'evidence']) {
      requireValue(validList(item[field]), 'CATALOG_METADATA ' + item.id + ' ' + field);
    }
    requireValue(validText(item.expected) || validList(item.expected), 'CATALOG_METADATA ' + item.id + ' expected');
    requireValue(item.requirements.every(id => catalog.requirementOwners.has(id)), 'CATALOG_REQUIREMENT ' + item.id);
    requireValue(item.sourceIds === undefined || validList(item.sourceIds) &&
      item.sourceIds.every(id => catalog.sources.has(id)), 'CATALOG_SOURCE_REFERENCE ' + item.id);
    requireValue(Number.isSafeInteger(item.owner) && item.owner > 0 &&
      (item.acceptancePhase === undefined || Number.isSafeInteger(item.acceptancePhase) && item.acceptancePhase >= item.owner),
    'CATALOG_OWNER ' + item.id);
    if (item.adapter === 'scenario') {
      const {driverPath} = require('./scenario-group.cjs');
      requireValue(driverPath.test(item.driver) && fs.existsSync(path.join(catalog.root, item.driver)) &&
        item.tier >= 2, 'CATALOG_SCENARIO_DRIVER ' + item.id);
    } else {
      requireValue([...scope, 'basic', 'procedure'].includes(item.adapter), 'CATALOG_ADAPTER ' + item.id);
    }
    requireValue(item.pr !== true || item.adapter === 'basic' && item.tier < 3, 'CATALOG_PR_WINDOW ' + item.id);
    requireValue(importanceLevels.includes(item.importance), 'CATALOG_IMPORTANCE ' + item.id);
    requireValue(['required', 'planned'].includes(item.status) &&
      (item.status !== 'required' || item.importance !== 'Critical' || criticalSmoke.has(item.id) || item.smoke === true),
    'CATALOG_STATUS ' + item.id);
    requireValue([1, 2, 3].includes(item.tier) && item.needsGameWindow === (item.tier === 3) &&
      (!item.needsGameWindow || validText(item.freshProcessReason)), 'CATALOG_TIER ' + item.id);
    if (item.adapter === 'lan-checks' && item.tier === 2) {
      requireValue(item.runGroup === 'lan-checks.headless.' + item.legacyId && validText(item.freshProcessReason),
        'CATALOG_LAN_HEADLESS_GROUP ' + item.id);
    }
    const group = catalog.groups.find(candidate => candidate.id === item.group);
    requireValue(group?.adapter === item.adapter && group.cases.includes(item.id) &&
      catalog.runGroups.some(entry => entry.id === item.runGroup && entry.cases.includes(item.id)),
    'CATALOG_DANGLING_GROUP ' + item.id);
    const position = item.runGroup + '/' + item.order;
    requireValue(Number.isSafeInteger(item.order) && item.order > 0 && !runOrders.has(position), 'CATALOG_RUN_ORDER ' + item.id);
    runOrders.add(position);
  }
  for (const group of catalog.groups) {
    requireValue(group.cases.every(id => catalog.cases.find(item => item.id === id)?.group === group.id),
      'CATALOG_DANGLING_CASE ' + group.id);
  }
  for (const group of catalog.runGroups) {
    requireValue(group.cases.every(id => catalog.cases.find(item => item.id === id)?.runGroup === group.id),
      'CATALOG_DANGLING_CASE ' + group.id);
  }
  requireValue(catalog.sources.size === 47 &&
    catalog.cases.filter(item => /^A-\d\d$/.test(item.id)).length === 47 &&
    [...catalog.sources.keys()].every(id => ids.has(id)), 'CATALOG_SOURCE_A01_A47');
  requireValue([...criticalSmoke].every(id => catalog.cases.some(item =>
    item.id === id && item.status === 'required' && item.importance === 'Critical')), 'CATALOG_CRITICAL_SMOKE');
  return catalog;
}

function covers(pattern, file) {
  return pattern.endsWith('/') ? file.startsWith(pattern) : file === pattern;
}

function supportStampOnly(before, after) {
  const stamp = /(\[base\+'foundation\/(FoundationRoot|FoundationInput)\.cs',)\d+,'[a-f0-9]{64}'\]/g;
  const baseline = [...before.matchAll(stamp)], current = [...after.matchAll(stamp)];
  return baseline.length > 0 && baseline.length === current.length
    && new Set(baseline.map(row => row[2])).size === baseline.length
    && baseline.some((row, index) => row[0] !== current[index][0])
    && before.replace(stamp, '$1<version>]') === after.replace(stamp, '$1<version>]');
}

function liveSupportOnly(catalog, baseCommit) {
  if (!baseCommit) return false;
  requireValue(/^[a-f0-9]{40}$/.test(baseCommit), 'SELECTION_BASE_COMMIT');
  const before = execFileSync('git', ['show', baseCommit + ':' + assetContentPath],
    {cwd: catalog.root, encoding: 'utf8', windowsHide: true, timeout: 30000});
  return supportStampOnly(before, fs.readFileSync(path.join(catalog.root, assetContentPath), 'utf8'));
}

function affected(catalog, changes, baseCommit) {
  const tags = new Set();
  const unknown = [];
  for (const file of changes) {
    // A source stamp can change without changing any other content helper behavior.
    if (file === assetContentPath && liveSupportOnly(catalog, baseCommit)) {
      tags.add('asset-live-support');
      continue;
    }
    const matched = Object.entries(catalog.impact).filter(([, paths]) => paths.some(pattern => covers(pattern, file)));
    for (const [tag] of matched) tags.add(tag);
    if (!matched.length && !catalog.neutralPaths.some(pattern => covers(pattern, file)) &&
      !catalog.cases.some(item => item.inputs?.some(pattern => covers(pattern, file)))) unknown.push(file);
  }
  requireValue(!unknown.length, 'SELECTION_UNKNOWN_IMPACT ' + unknown.join(', '));
  for (const tag of tags) for (const consumer of catalog.impactsByTag[tag] ?? []) tags.add(consumer);
  return tags;
}

function changedVerificationGroups(before, after) {
  const runtime = ['assembly', 'project', 'executable', 'childTimeoutMs', 'inputs'];
  if (runtime.some(field => JSON.stringify(before[field]) !== JSON.stringify(after[field]))) {
    return new Set(Object.keys(ruleInputs));
  }
  const active = (manifest, group) => manifest.cases.filter(row =>
    row.group === group && !isDeferredCertification(row));
  return new Set(Object.keys(ruleInputs).filter(group =>
    JSON.stringify(active(before, group)) !== JSON.stringify(active(after, group))));
}

function verificationImpact(catalog, changes, baseCommit) {
  if (!changes.includes(verificationPath)) return new Set();
  if (baseCommit === undefined) return new Set(Object.keys(ruleInputs));
  requireValue(/^[a-f0-9]{40}$/.test(baseCommit), 'SELECTION_BASE_COMMIT');
  const before = JSON.parse(execFileSync('git', ['show', baseCommit + ':' + verificationPath],
    {cwd: catalog.root, encoding: 'utf8', windowsHide: true, timeout: 30_000}));
  return changedVerificationGroups(before, catalog.verificationManifest);
}

function changedCaseIds(catalog, changes, baseCommit) {
  const changed = new Set();
  const declarations = [['basic', catalogPath, catalog.declared], ...scope.filter(name => name !== 'verification')
    .map(name => [name, tools + name + '/required-cases.json', catalog.manifests[name]])];
  const readAtBase = file => {
    const git = args => execFileSync('git', args, {cwd: catalog.root, encoding: 'utf8', windowsHide: true, timeout: 30000});
    if (!git(['ls-tree', '--name-only', baseCommit, '--', file]).trim()) return {};
    return JSON.parse(git(['show', baseCommit + ':' + file]));
  };
  const shape = row => JSON.stringify(row, (key, value) => key === 'moduleSha256' ? undefined : value);
  for (const [adapter, file, after] of declarations) {
    if (!changes.includes(file)) continue;
    const before = baseCommit === undefined ? {} : readAtBase(file);
    const rows = manifest => [...manifest.cases ?? [], ...manifest.renderedCases ?? []];
    const old = new Map(rows(before).map(row => [row.id, shape(row)]));
    for (const row of rows(after)) {
      if (old.get(row.id) !== shape(row)) changed.add(adapter === 'basic' ? row.id : adapter + '.' + row.id);
    }
  }
  return changed;
}

function select(catalog, {mode, value, changes = [], baseCommit}) {
  const modes = ['pr', 'phase-end', 'full', 'case', 'group', 'tier'];
  requireValue(modes.includes(mode), 'SELECTION_MODE ' + mode);
  requireValue(Array.isArray(changes) && changes.every(validText), 'SELECTION_CHANGES');
  const phase = mode === 'phase-end' && value !== undefined ? Number(value) : 5;
  requireValue(Number.isSafeInteger(phase) && phase > 0 &&
    (mode !== 'phase-end' || value === undefined || /^[1-9]\d*$/.test(String(value))), 'SELECTION_PHASE');
  requireValue(mode !== 'tier' || /^[123]$/.test(String(value)), 'SELECTION_TIER');
  requireValue(!['pr', 'full'].includes(mode) || value === undefined, 'SELECTION_CONFLICT');
  const ids = new Set();
  if (mode === 'case') {
    const requested = Array.isArray(value) ? value : String(value ?? '').split(',');
    requireValue(validList(requested) && requested.every(id => catalog.cases.some(item => item.id === id)), 'SELECTION_CASE');
    for (const id of requested) {
      requireValue(catalog.cases.some(item => item.id === id && item.status === 'required'), 'SELECTION_PLANNED ' + id);
      ids.add(id);
    }
  }
  if (mode === 'group') requireValue(catalog.groups.some(group => group.id === value) ||
    catalog.cases.some(item => item.runGroup === value), 'SELECTION_GROUP ' + value);
  const affectedTags = mode === 'phase-end' ? affected(catalog, changes, baseCommit) : new Set();
  const verificationGroups = mode === 'phase-end' ? verificationImpact(catalog, changes, baseCommit) : new Set();
  const changedDeclarations = mode === 'phase-end' ? changedCaseIds(catalog, changes, baseCommit) : new Set();
  const selected = [], omitted = [];
  for (const item of catalog.cases) {
    const changed = changedDeclarations.has(item.id) || changes.includes(item.driver) || changes.some(file => item.inputs?.some(pattern => covers(pattern, file))) ||
      item.adapter === 'verification' && verificationGroups.has(item.legacyId);
    const impact = item.tags.filter(tag => affectedTags.has(tag));
    const picked = {
      pr: item.pr === true,
      full: true,
      case: ids.has(item.id),
      group: item.group === value || item.runGroup === value,
      tier: item.tier === Number(value) && (Number(value) !== 1 || item.id === 'basic.pure'),
      'phase-end': item.adapter === 'verification' ? changed :
        item.phaseEnd === true || item.acceptancePhase === phase || item.owner === phase ||
        item.importance === 'Critical' || changed || impact.length > 0
    }[mode];
    if (item.status === 'required' && picked) {
      const reason = mode !== 'phase-end' ? mode : item.phaseEnd ? 'phase baseline' :
        item.importance === 'Critical' ? 'Critical smoke' :
        item.owner === phase || item.acceptancePhase === phase ? 'due in this phase' :
        changed ? 'declared input changed' : 'affected by ' + impact.join(', ');
      selected.push({...item, reason});
    } else {
      const planned = item.status === 'planned';
      omitted.push({id: item.id, status: planned ? 'planned' : 'not-selected',
        reason: planned ? 'Phase ' + item.owner + ' owns the pending procedure' :
          mode === 'phase-end' ? 'unaffected and outside the selected smoke or phase scope' : 'outside the ' + mode + ' selection'});
    }
  }
  requireValue(selected.length > 0, 'SELECTION_EMPTY');
  return {selected, omitted};
}

module.exports = {changedVerificationGroups, loadCatalog, select, validateCatalog, verificationRows, supportStampOnly};
