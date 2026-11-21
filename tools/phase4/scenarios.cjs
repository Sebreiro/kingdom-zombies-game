'use strict';

const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const net = require('node:net');
const os = require('node:os');
const {spawn} = require('node:child_process');
const {setTimeout:delay} = require('node:timers/promises');
const {isDeepStrictEqual:equal} = require('node:util');
const e = require('./evidence.cjs');

const CATALOG = 'tools/phase4/required-cases.json';
const COVERAGE = e.PHASE + '04-ACCEPTANCE-COVERAGE.md';
const SCENARIOS = e.PHASE + '04-SCENARIOS.md';
const RUNS = e.PHASE + '04-RUNS.json';
const ORDER = (a, b) => a.order - b.order || (a.id < b.id ? -1 : a.id > b.id ? 1 : 0);
const IMPORTANCE = ['Critical', 'High', 'Normal', 'Low'];
const COMPOSITION = new Set(['animation', 'appearance', 'bodies', 'buildings', 'items', 'world']);
const GROUPS = ['G1-hub', 'G2-packaged', 'G3-solo', 'G3-lan'];
const HISTORICAL_TARGET = 'tools/phase3/reviewed-source.targets';
const REVIEW_PROOF_CASES = new Map([
  ['CR-01',['runner.prelaunch-source']], ['CR-02',['runner.prelaunch-source','export.portability']],
  ['CR-03',['runner.prelaunch-source']], ['CR-04',['runner.prelaunch-source','export.portability']],
  ['CR-05',['runner.prelaunch-source']], ['CR-06',['acceptance.artifacts-source']],
  ['CR-07',['acceptance.judgments','audio.lifecycle']], ['CR-08',['hub.source','export.hub-consumer','content.census']],
  ['CR-09',['source.publication','content.census']], ['WR-01',['runner.prelaunch-source']], ['WR-02',['animation.bow']]
]);
const RUNTIME_FIELDS = ['title','importance','rationale','needsGameWindow','runGroup','order','requirements',
  'acceptance','setup','actions','expected','reset','evidence','group'];

function booleanOutcomes(row, keys, label) {
  e.need(keys.every(key => typeof row[key] === 'boolean'), 'SELECTION_IMPACT', label);
}

function coverageRows(root, pattern) {
  return e.read(root, COVERAGE).data.toString('utf8').split(/\r?\n/).filter(line => pattern.test(line))
    .map(line => line.split('|').slice(1, -1).map(value => value.trim()));
}

function validateProcedure(row) {
  e.need(e.named(row.id) && IMPORTANCE.includes(row.importance) && Number.isSafeInteger(row.order) &&
    typeof row.needsGameWindow === 'boolean' && ['active-source', 'active-rendered'].includes(row.lifecycle), 'SCENARIO_IDENTITY', row.id);
  for (const field of ['title','rationale','setup','expected','reset','cleanup','boundedWaits','runGroup','group']) {
    e.need(e.named(row[field]), 'SCENARIO_FIELD', row.id + ' ' + field);
  }
  for (const field of ['actions','evidence','featureTags','requirements','preconditions','input','processRoles']) {
    e.need(Array.isArray(row[field]) && row[field].length > 0 && row[field].every(e.named), 'SCENARIO_FIELD', row.id + ' ' + field);
  }
  e.need(Array.isArray(row.acceptance) && Array.isArray(row.heldoutIds) && Array.isArray(row.uiIds), 'SCENARIO_COVERAGE', row.id);
}

function validateCatalog(root, catalog, registered = null) {
  e.need(catalog?.schemaVersion === 1 && catalog.id === 'phase4-fixture-catalog' && catalog.actualAcceptedObligations === 0 &&
    Array.isArray(catalog.cases) && catalog.cases.length > 0 && Array.isArray(catalog.renderedCases) &&
    Array.isArray(catalog.plannedCases), 'SCENARIO_CATALOG');
  const byId = new Map(catalog.cases.map(row => [row.id, row]));
  const all = [...catalog.cases, ...catalog.renderedCases, ...catalog.plannedCases];
  e.exact(all.map(row => row.id), all.map(row => row.id), 'catalog case IDs');
  const settings = [
    ['heldout', /^\| H\d\d \|/, 34, 'intent', 2], ['backstops', /^\| UIH\./, 70, 'intent', 2],
    ['acceptance', /^\| AC-\d\d \|/, 24, 'criterion', 1], ['decisions', /^\| D-\d\d \|/, 32, 'statement', 1],
    ['judgments', /^\| P[1-4] \|/, 4, 'statement', 1]
  ];
  for (const [key, pattern, count, field, column] of settings) {
    const expected = coverageRows(root, pattern);
    e.need(expected.length === count && Array.isArray(catalog[key]), 'SCENARIO_COVERAGE', key);
    e.exact(expected.map(row => row[0]), catalog[key].map(row => row.id), key, true);
    for (const [index, row] of catalog[key].entries()) {
      e.need(row[field] === expected[index][column], 'SCENARIO_INTENT', row.id);
    }
  }
  const expectedRendered = [...catalog.heldout.map(row => row.actualAcceptanceCase), ...catalog.backstops.map(row => row.id),
    'rendered.hub-navigation','rendered.packaged-relaunch','rendered.audio-audition'];
  e.exact(expectedRendered, catalog.renderedCases.map(row => row.id), 'rendered cases');
  for (const row of [...catalog.cases, ...catalog.renderedCases]) validateProcedure(row);
  for (const row of catalog.renderedCases) {
    const domain = catalog.heldout.find(item => item.actualAcceptanceCase === row.id);
    const surface = catalog.backstops.find(item => item.id === row.id);
    e.need(row.needsGameWindow && GROUPS.slice(0,2).includes(row.runGroup), 'SCENARIO_RENDERED', row.id);
    if (domain) e.need(equal(row.heldoutIds, [domain.id]) && row.expected.includes(domain.intent), 'SCENARIO_HELDOUT', row.id);
    if (surface) e.need(equal(row.uiIds, [surface.id]) && row.expected.includes(surface.intent), 'SCENARIO_UI', row.id);
  }
  for (const row of catalog.judgments) {
    e.need(row.status === 'pending-named-judgment' && row.procedure?.result === null &&
      row.procedure.actions.every(e.named) && row.procedure.requiredEvidence.includes('Named reviewer'), 'SCENARIO_JUDGMENT', row.id);
  }
  e.need(new Set(catalog.judgments.map(row => row.procedure.actions.join('\n'))).size === 4, 'SCENARIO_JUDGMENT_DISTINCT');
  const exportCase = all.find(row => row.id === 'export.hub-consumer');
  e.need(exportCase?.importance === 'Critical' && exportCase.group === 'export' && exportCase.runGroup === 'G0-export-hub' &&
    exportCase.order === 310 && equal(exportCase.requirements, ['ASSET-05']) &&
    equal(exportCase.acceptance, ['AC-15','AC-16','AC-17']), 'SCENARIO_EXPORT_HANDOFF');
  if (registered !== null) {
    e.exact([...byId.keys()], [...registered.keys()], 'executable discovery');
    for (const [id, runtime] of registered) {
      const row = byId.get(id);
      for (const field of RUNTIME_FIELDS) e.need(equal(row[field], runtime[field]), 'SCENARIO_DISCOVERY_DRIFT', id + ' ' + field);
      const binding = row.sourceBinding;
      e.need(binding.id === id && e.reference(root, binding.module).sha256 === binding.moduleSha256, 'SCENARIO_MODULE', id);
    }
  }
  const saved = e.read(root, SCENARIOS).data.toString('utf8');
  for (const item of [...catalog.cases, ...catalog.renderedCases]) {
    e.need(saved.includes('### ' + item.id + '\n') || saved.includes('`' + item.id + '`'), 'SCENARIO_DOCUMENT', item.id);
  }
  return catalog;
}

function loadCatalog(root, registered) {
  const catalog = validateCatalog(root, e.document(root, CATALOG), registered);
  const handoff = e.document(root, 'tools/phase4/handoff.json');
  e.exact(['4','5','6','7','8','9'], Object.keys(handoff), 'handoff phases');
  e.need(Object.values(handoff).every(Array.isArray), 'HANDOFF_CASE_LIST');
  e.exact(catalog.renderedCases.map(row => row.id), Object.values(handoff).flat(), 'handoff owners');
  const active = new Set(handoff[4]);
  for (const row of catalog.renderedCases) {
    if (row.id.startsWith('rendered.') || row.importance === 'Critical') e.need(active.has(row.id), 'HANDOFF_REQUIRED', row.id);
  }
  const activeRendered = catalog.renderedCases.filter(row => active.has(row.id));
  const deferred = Object.fromEntries(Object.entries(handoff).filter(([phase]) => phase !== '4'));
  const acceptance = catalog.acceptance.map(row => ({...row, actualCases:row.actualCases.filter(id => active.has(id))}));
  return {...catalog, activeRendered, deferred, acceptance};
}

function selectCases(registry, option, value) {
  e.need(registry instanceof Map && registry.size > 0, 'CASE_DISCOVERY_EMPTY');
  let selected;
  if (option === '--case') {
    const ids = value.split(',');
    e.need(ids.length <= 256 && new Set(ids).size === ids.length && ids.every(id => registry.has(id)), 'CASE_PENDING_OR_UNKNOWN', value);
    selected = ids.map(id => registry.get(id));
  } else {
    e.need(option === '--group', 'CASE_FILTER', option);
    selected = [...registry.values()].filter(row => value === 'composition' ? COMPOSITION.has(row.id.split('.')[0]) : row.group === value);
  }
  e.need(selected.length > 0, 'CASE_PENDING_OR_UNKNOWN', value);
  return selected.sort(ORDER);
}

function inheritedCases(root) {
  return [1,2,3].flatMap(phase => e.document(root, 'tools/phase' + phase + '/required-cases.json').cases
    .filter(row => row.implementation !== 'pending' && (row.acceptancePhase ?? 1) <= 4)
    .map((row, index) => {
      const rendered = row.needsGameWindow === true || phase === 2 && row.implementation === 'native';
      return {...row, originalId:row.id, inheritedGroup:row.runGroup, family:'phase' + phase, id:'phase' + phase + '/' + row.id,
        needsGameWindow:rendered, runGroup:rendered ? (phase === 2 ? 'G3-solo' : 'G3-lan') : 'G0-inherited',
        order:(phase * 10000) + (row.order ?? index), processRoles:rendered ? (phase === 2 ? ['solo'] : row.roles) : ['source']};
    }));
}

function selectionRows(root, catalog) {
  return [...catalog.cases, ...catalog.activeRendered, ...inheritedCases(root)];
}

function impactClosure(cases, files) {
  const starts = (...prefixes) => files.some(file => prefixes.some(prefix => file === prefix || file.startsWith(prefix+'/')));
  const shared = starts('Directory.Build.props','Directory.Build.targets','Directory.Packages.props','global.json','NuGet.config');
  const runtime = shared || starts('src','tools/code-rules','tests/phase1');
  const inheritedGroups = {
    engine:['src/godot-game','docs/code','tools/code-rules','tests/phase1'],
    graph:['src','docs/architecture','docs/code','tests/phase1','tools/phase1/projects.json'],
    rules:['src','docs/code','tools/code-rules','tests/phase1'],
    register:['docs/code','tools/code-rules','tests/phase1'],
    state:['src/core','src/godot-game','tests/phase1'],
    docs:['docs','specs','src','tools','tests','AGENTS.md'],
    runner:['tools/phase1','tests/phase1'],
    coverage:['src','docs/code','tests/phase1'],
    pins:['tools/phase1/toolchain.json']
  };
  return new Map(cases.map(row => [row.id, {new:!row.family, changed:false, affected:row.family==='phase1' ?
    shared || starts(...inheritedGroups[row.group]??[]) : Boolean(row.family && runtime)}]));
}

function validateSelection(record, cases, catalogRef, current, expectedImpact = null) {
  e.need(record?.schemaVersion === 1 && record.status === 'declared-before-first-window' && e.instant(record.declaredAt) &&
    record.platform === 'win32-x64' && record.vm === false && equal(record.window, {width:1280,height:720}) &&
    Array.isArray(record.selected) && Array.isArray(record.omitted) && Array.isArray(record.groups) &&
    record.catalogSha256 === catalogRef.sha256, 'SELECTION_SCHEMA');
  e.identity(current, record, 'selection');
  e.exact(cases.map(row => row.id), [...record.selected, ...record.omitted].map(row => row.id), 'selection dispositions');
  const entries = new Map([...record.selected, ...record.omitted].map(row => [row.id, row]));
  const chosen = new Set(record.selected.map(row => row.id));
  for (const row of cases) {
    const item = entries.get(row.id);
    booleanOutcomes(item, ['new','changed','affected','uncertain'], row.id);
    e.need(e.named(item.reason) && Array.isArray(item.dependencies) && item.dependencies.every(e.named) &&
      item.dependencies.length > 0 && !item.uncertain, 'SELECTION_IMPACT', row.id);
    const impact = expectedImpact?.get(row.id);
    e.need(!impact || ['new','changed','affected'].every(key => !impact[key] || item[key]), 'SELECTION_IMPACT_MISMATCH', row.id);
    const required = !row.family || row.importance === 'Critical' || item.new || item.changed || item.affected;
    e.need(!required || chosen.has(row.id), 'SELECTION_REQUIRED', row.id);
  }
  const selected = cases.filter(row => chosen.has(row.id));
  const byGroup = new Map();
  for (const row of selected.sort(ORDER)) {
    if (!byGroup.has(row.runGroup)) byGroup.set(row.runGroup, []);
    byGroup.get(row.runGroup).push(row);
  }
  e.exact([...byGroup.keys()], record.groups.map(row => row.id), 'selected run groups');
  let seenWindow = false;
  for (const group of record.groups) {
    const rows = byGroup.get(group.id);
    const native = rows[0].needsGameWindow;
    e.need(rows.every(row => row.needsGameWindow === native) && (!seenWindow || native) &&
      (!native || GROUPS.includes(group.id)) && e.named(group.reset) && e.named(group.cleanup) &&
      Number.isSafeInteger(group.timeoutMs) && group.timeoutMs > 0 && group.timeoutMs <= 900000, 'SELECTION_GROUP', group.id);
    e.exact(rows.map(row => row.id), group.cases, group.id, true);
    e.exact([...new Set(rows.flatMap(row => row.processRoles ?? []))], group.roles, group.id + ' roles');
    seenWindow ||= native;
  }
  e.exact(GROUPS.filter(group => byGroup.has(group)), record.groups.filter(group => GROUPS.includes(group.id)).map(group => group.id),
    'rendered group order', true);
  return {selected:record.groups.flatMap(group => byGroup.get(group.id)), groups:record.groups, omitted:record.omitted};
}

function currentReviewAdapter(root, revision, targetRef) {
  e.need(targetRef?.path === e.TARGETS, 'REVIEW_TARGET_PATH');
  e.committedRef(root, revision, targetRef);
  const targets = e.textArtifact(root, targetRef);
  e.need(targets.includes('SourceRevisionId') && targets.includes('SourceRoot') &&
    !targets.includes(HISTORICAL_TARGET) && !targets.includes('Phase3Reviewed'), 'REVIEW_TARGET_HISTORICAL');
  return {
    env:{...process.env, SourceRevisionId:revision, CustomAfterMicrosoftCommonTargets:path.join(root, e.TARGETS)},
    solo:require('../phase2/rendered.cjs').rendered,
    lan:require('../phase3/scenarios.cjs').run,
    physical:require('../phase2/rendered.cjs').ownedInput,
    decodePng:require('../phase2/rendered.cjs').decodePng,
    readDraws:require('../phase3/rendered.cjs').draws
  };
}

function verifyFixProof(root, catalog, current, finding, proof) {
  const {commit,cases} = proof;
  e.need(e.REVISION.test(commit) && Array.isArray(cases) && cases.length > 0, 'REVIEW_PROOF_SCHEMA', finding.id);
  const eligible = REVIEW_PROOF_CASES.get(finding.id);
  const tested = new Map(catalog.cases.map(row => [row.id,row]));
  e.exact(cases.map(row => row.id),cases.map(row => row.id),'review proof cases '+finding.id);
  const [subject,committed] = e.git(root,['show','-s','--format=%s%n%cI',commit]).trim().split(/\r?\n/);
  e.need(subject.startsWith('fix(04): '+finding.id+' ') && Number.isFinite(Date.parse(committed)), 'REVIEW_PROOF_COMMIT', finding.id);
  e.git(root,['merge-base','--is-ancestor',commit,current.revision]);
  const changed = e.git(root,['diff-tree','--no-commit-id','--name-only','-r','-z',commit]).split('\0');
  const touched = changed.filter(file => finding.paths.includes(file));
  e.need(touched.length > 0,'REVIEW_PROOF_SCOPE',finding.id);
  for (const receipt of cases) {
    const item = tested.get(receipt.id);
    e.need(item && !item.needsGameWindow && (eligible ? eligible.includes(item.id) : touched.includes(item.sourceBinding.module)),
      'REVIEW_PROOF_CASE', finding.id+' '+receipt.id);
    const {result} = phase4SourceReceipt(root,item,receipt,current);
    e.need(equal(result.testedSource,{source:current.source,media:current.media}) &&
      e.sameCommittedSource(root,result.revision,current.revision) && Date.parse(result.startedAt) >= Date.parse(committed),
      'REVIEW_PROOF_CURRENT', finding.id+' '+receipt.id);
    e.git(root,['merge-base','--is-ancestor',commit,result.revision]);
  }
}

function preWindowCases(selected) {
  // Review and selection are verified by this gate. Judgment and closure proofs follow rendered execution.
  const otherStages = new Set(['runner.prelaunch','runner.selection','acceptance.judgments','acceptance.artifacts']);
  return selected.filter(row => !row.needsGameWindow && !otherStages.has(row.id));
}

function prelaunch(root, catalog, current, {receiptRoot = null, prerequisites = true} = {}) {
  const reviewRef = e.reference(root, e.REVIEW);
  const selectionRef = e.reference(root, e.SELECTION);
  const fixesRef = e.reference(root, e.FIXES);
  const review = e.document(root, reviewRef);
  const selection = e.document(root, selectionRef);
  const fixes = e.document(root, fixesRef);
  const head = e.git(root, ['rev-parse','HEAD']).trim();
  e.need(review.report?.path === e.REVIEW_REPORT, 'REVIEW_REPORT_PATH');
  for (const ref of [reviewRef, selectionRef, fixesRef, review.report]) e.committedRef(root, head, ref);
  e.git(root, ['merge-base','--is-ancestor', current.revision, head]);
  const original = e.git(root,['show',e.ORIGINAL_REVIEW+':'+e.REVIEW_REPORT],null);
  e.validateReview(review, fixes, current, {report:e.textArtifact(root, review.report),
    originalReport:{text:new TextDecoder('utf-8',{fatal:true}).decode(original),sha256:require('./content.cjs').hash(original)},
    verifyProof:(finding,proof) => verifyFixProof(root,catalog,current,finding,proof)});
  e.need(selection.declaredAt >= review.reviewedAt, 'SELECTION_BEFORE_REVIEW');
  const adapter = currentReviewAdapter(root, head, review.targets);
  const rows = selectionRows(root,catalog), reviewChanges = e.changedFiles(root,current.revision);
  const resolved = validateSelection(selection,rows,e.reference(root,CATALOG),current,impactClosure(rows,reviewChanges));
  if (prerequisites) sourceReceipts(root,preWindowCases(resolved.selected),selection.sourceResults,current);
  if (receiptRoot !== null) {
    for (const ref of [reviewRef, selectionRef, fixesRef, review.report, review.targets])
      e.writeExclusive(root, receiptRoot + '/' + path.basename(ref.path), e.artifact(root, ref).data);
  }
  return {...resolved, review:reviewRef, selection:selectionRef, fixes:fixesRef, reviewedAt:review.reviewedAt, adapter, head};
}

function currentIdentity(root) {
  // Review binding commits may follow the tested source commit without changing its exact tracked inputs.
  const review = e.document(root, e.REVIEW);
  const source = e.identifySource(root, review.revision);
  const tools = e.identifyTool(root);
  const build = e.document(root, review.buildReport);
  e.need(build.schemaVersion === 1 && build.status === 'pass' && build.revision === source.revision &&
    build.source === source.source && build.media === source.media && build.tool === tools.tool &&
    build.sourceLinkRevision === source.revision && Array.isArray(build.artifacts) && build.artifacts.length > 0, 'REVIEW_BUILD');
  for (const ref of build.artifacts) e.artifact(root, ref);
  return {revision:source.revision, source:source.source, media:source.media, tool:tools.tool, build:review.buildReport.sha256};
}

function runId() {
  return '.phase4/acceptance/' + crypto.randomUUID();
}

function finalRendered(root, gate, current, refs, campaign) {
  const native = gate.selected.filter(row => row.needsGameWindow);
  const reports = e.historyDocuments(root,refs,campaign);
  const final = e.freshFinal(native,reports,current);
  e.need(final.reviewSha256 === gate.review.sha256 && final.selectionSha256 === gate.selection.sha256 &&
    final.startedAt >= gate.reviewedAt,'COMPLETION_CURRENT_RUN');
  e.need(final.drivers?.path === '.phase4/acceptance/'+final.invocation+'/declared-drivers.json','COMPLETION_DRIVER_SCOPE');
  const driverRecord = e.document(root,final.drivers);
  const drivers = reviewedDrivers(driverRecord,gate,current,native,final.kind);
  e.need(driverRecord.campaign === final.campaign && driverRecord.declaredAt <= final.startedAt,'COMPLETION_DRIVER_TIME');
  for (const [index,row] of final.outcomes.entries()) {
    const item = native[index];
    if (!item.family) e.validateRenderedOutcome(root,row,final,drivers.get(row.id));
    else {
      const {result,bindings} = e.document(root,row.inherited);
      const directory = path.resolve(root,path.dirname(row.inherited.path));
      e.need(bindings.nonce === final.invocation && bindings.revision === current.revision &&
        bindings.input === current.source && bindings.tool === current.tool && bindings.platform === 'win32-x64',
      'COMPLETION_INHERITED_BINDINGS',item.id);
      const original = {...item,id:item.originalId,runGroup:item.inheritedGroup};
      const context = {root,directory,...bindings};
      if (item.family === 'phase2') require('../phase2/evidence.cjs').reconcile([original],[result],context);
      else require('../phase3/evidence.cjs').reconcile([original],[result],context);
    }
  }
  return final;
}

function latestPhase4Sources(root, admitted, currentRevision, sameInputs) {
  const found = new Map(admitted);
  if (!found.size) return found;
  const base = path.join(root,'.phase4','runs');
  for (const folder of fs.readdirSync(base,{withFileTypes:true})) {
    if (!folder.isDirectory() || !e.UUID.test(folder.name)) continue;
    const prefix = '.phase4/runs/'+folder.name+'/';
    if (!fs.existsSync(path.join(root,prefix,'invocation.json'))) continue;
    const invoked = e.document(root,prefix+'invocation.json');
    const prior = found.get(invoked.caseId);
    if (!prior || invoked.nonce === prior.nonce) continue;
    e.need(invoked.nonce === folder.name && e.instant(invoked.startedAt),'COMPLETION_SOURCE_INVOCATION',invoked.caseId);
    if (invoked.startedAt < prior.startedAt || invoked.node !== prior.node || invoked.platform !== prior.platform ||
      !sameInputs(root,invoked.revision,currentRevision)) continue;
    e.need(invoked.startedAt !== prior.startedAt,'COMPLETION_SOURCE_TIME_AMBIGUOUS',invoked.caseId);
    const relative = prefix+'result.json';
    const terminal = fs.existsSync(path.join(root,relative)) ? e.document(root,relative) : null;
    if (terminal) e.need(terminal.caseId === invoked.caseId && terminal.nonce === invoked.nonce && terminal.revision === invoked.revision &&
      terminal.startedAt === invoked.startedAt && e.instant(terminal.finishedAt) && terminal.finishedAt >= invoked.startedAt,
    'COMPLETION_SOURCE_HISTORY',invoked.caseId);
    found.set(invoked.caseId,{...invoked,status:terminal?.status??'missing'});
  }
  return found;
}

function phase4SourceReceipt(root, item, receipt, current) {
  const result = e.document(root,receipt.result);
  e.need(result.status==='pass' && result.observations!==null && typeof result.observations==='object',
    'COMPLETION_SOURCE_FAILURE',item.id);
  const directory = '.phase4/runs/'+result.nonce+'/';
  e.need(e.UUID.test(result.nonce) && receipt.result.path===directory+'result.json' && result.caseId===item.id,
    'COMPLETION_SOURCE_SUBJECT',item.id);
  const invocation = e.document(root,directory+'invocation.json');
  e.need(invocation.nonce===result.nonce && invocation.caseId===item.id && invocation.revision===result.revision &&
    invocation.platform==='win32' && invocation.node==='v'+e.document(root,'tools/phase1/toolchain.json').node &&
    invocation.scope==='source/headless; zero game windows' &&
    e.instant(result.startedAt) && invocation.startedAt===result.startedAt && e.instant(result.finishedAt) && result.finishedAt>=result.startedAt,
    'COMPLETION_SOURCE_INVOCATION',item.id);
  const binding = item.sourceBinding;
  if (binding?.module.startsWith('tools/phase4/cases/')) {
    const modules = invocation.caseModules?.filter(row => row.caseIds.includes(item.id));
    e.need(modules?.length===1 && modules[0].name===path.basename(binding.module) && modules[0].sha256===binding.moduleSha256,
      'COMPLETION_SOURCE_MODULE',item.id);
  }
  e.need(!Object.hasOwn(result,'unverifiedSource') && !Object.hasOwn(invocation,'unverifiedSource') &&
    equal(result.testedSource,invocation.testedSource) && (!result.testedSource ||
      equal(result.testedSource,{source:current.source,media:current.media})), 'COMPLETION_SOURCE_DRIFT',item.id);
  return {result,invocation};
}

function inheritedSourceReceipt(root, item, receipt) {
  const result = e.document(root,receipt.result);
  const directory = path.posix.dirname(receipt.result.path);
  const familyPath = item.family==='phase1' ? '.phase1/runs/' : '.'+item.family+'/runs/win32-x64/';
  const subject = result.results?.filter(row => row.id===item.originalId);
  e.need(result.status==='pass' && e.UUID.test(result.nonce) && result.platform==='win32-x64' &&
    receipt.result.path===familyPath+result.nonce+'/summary.json' && result.selected?.includes(item.originalId) &&
    subject?.length===1 && subject[0].status==='pass','COMPLETION_INHERITED_SUBJECT',item.id);
  const expected = {...item,id:item.originalId};
  const context = {...result,root,directory:path.join(root,directory)};
  if (item.family==='phase1') {
    const baseline = require('../phase1/check.cjs'), row = subject[0];
    const readProof = (parent,ref) => {
      const actual = e.reference(root,parent+'/'+ref.path);
      e.need(actual.sha256===ref.sha256,'COMPLETION_INHERITED_HASH',item.id);
      return actual;
    };
    if (item.callerStages) baseline.verifyCallerResult(expected,row,{...context,directory:path.join(context.directory,item.originalId)});
    else {
      const report = e.document(root,readProof(directory,row.report));
      if (item.repositoryStages || item.producerStages) {
        e.need(report.caseId===item.originalId && report.status==='pass' && report.monolithicExecuted===false &&
          equal(report.stages,row.stages),'COMPLETION_INHERITED_STAGES',item.id);
      } else {
        e.need(Array.isArray(row.evidence) && row.evidence.length>0,'COMPLETION_INHERITED_EVIDENCE',item.id);
        for (const ref of row.evidence) readProof(directory+'/'+item.originalId,ref);
        baseline.verifyReport(report,[expected]);
      }
    }
  } else if (item.family==='phase2') require('../phase2/evidence.cjs').reconcile([expected],subject,context);
  else require('../phase3/evidence.cjs').reconcile([expected],subject,context);
  return result;
}

function inheritedSourceTime(root, ref, result) {
  if (Number.isSafeInteger(result.startedAt)) return result.startedAt;
  const firstCommand = Date.parse(result.commands?.[0]?.started);
  return Number.isFinite(firstCommand) ? firstCommand : fs.statSync(path.join(root,ref.path)).mtimeMs;
}

function verifyInheritedHistory(root, admitted, current, sameInputs) {
  const families = new Set([...admitted.values()].map(row => row.item.family));
  for (const family of families) {
    const directory = family==='phase1' ? '.phase1/runs/' : '.'+family+'/runs/win32-x64/';
    for (const folder of fs.readdirSync(path.join(root,directory),{withFileTypes:true})) {
      if (!folder.isDirectory() || !e.UUID.test(folder.name)) continue;
      const relative = directory+folder.name+'/summary.json';
      if (!fs.existsSync(path.join(root,relative))) continue;
      const result = e.document(root,relative);
      if (result.platform!=='win32-x64' || !Array.isArray(result.selected)) continue;
      const candidates = result.selected.map(id => admitted.get(family+'/'+id)).filter(row => row && row.result.nonce!==result.nonce);
      if (!candidates.length) continue;
      const time = inheritedSourceTime(root,{path:relative},result);
      for (const row of candidates) {
        if (time < row.time || !e.REVISION.test(result.revision) || !sameInputs(root,result.revision,current.revision)) continue;
        e.need(time !== row.time,'COMPLETION_SOURCE_TIME_AMBIGUOUS',row.item.id);
        const status = result.results?.find(proof => proof.id===row.item.originalId)?.status ?? 'missing';
        e.need(result.nonce===folder.name && result.status==='pass' && status==='pass','COMPLETION_LATER_SOURCE_FAILURE',row.item.id+' '+status);
        e.need(false,'COMPLETION_SOURCE_REFERENCE_SUPERSEDED',row.item.id);
      }
    }
  }
}

function sourceReceipts(root, selected, refs, current) {
  e.need(Array.isArray(refs),'COMPLETION_SOURCE_RECEIPTS');
  e.exact(selected.map(row => row.id),refs.map(row => row.id),'source/headless selected outcomes');
  const admitted = new Map();
  const inherited = new Map();
  const examined = new Map();
  function sameUnderReview(root,revision,currentRevision) {
    if (!examined.has(revision)) examined.set(revision,e.sameCommittedSource(root,revision,currentRevision));
    return examined.get(revision);
  }
  for (const item of selected) {
    const receipt = refs.find(row => row.id === item.id);
    let result;
    if (!item.family) {
      const source = phase4SourceReceipt(root,item,receipt,current);
      result = source.result;
      admitted.set(item.id,{...source.invocation,status:'pass'});
    } else {
      result = inheritedSourceReceipt(root,item,receipt);
      inherited.set(item.id,{item,result,time:inheritedSourceTime(root,receipt.result,result)});
    }
    e.need(sameUnderReview(root,result.revision,current.revision),'COMPLETION_SOURCE_DRIFT',item.id);
  }
  const latest = latestPhase4Sources(root,admitted,current.revision,sameUnderReview);
  for (const [id,invocation] of latest) {
    e.need(invocation.status==='pass','COMPLETION_LATER_SOURCE_FAILURE',id+' '+invocation.status);
    e.need(invocation.nonce===admitted.get(id).nonce,'COMPLETION_SOURCE_REFERENCE_SUPERSEDED',id);
  }
  verifyInheritedHistory(root,inherited,current,sameUnderReview);
  return new Set(selected.map(row => row.id));
}

function completionSourceReceipts(root, selected, refs, current) {
  // The verifier's own result is published after this check. The independent qualification has its own case ID.
  return sourceReceipts(root,selected.filter(row => !row.needsGameWindow && row.id!=='acceptance.artifacts'),refs,current);
}

function coverageExpectation(catalog, root) {
  const catalogAlpha = new Set(catalog.cases.map(row => row.id));
  const native = new Set(catalog.activeRendered.map(row => row.id));
  const resolve = rows => rows.map(row => row.id).filter(id => catalogAlpha.has(id));
  const judgment = e.judgmentExpectations(catalog);
  return {
    obligations:require('./content.cjs').expected(root).obligations.map(row => ({id:row.id,cases:['content.census']})),
    acceptance:catalog.acceptance.map(row => ({id:row.id,cases:[...new Set([...resolve(row.sourceCases),...row.actualCases,
      ...judgment.filter(item => item.criterion===row.id).map(item => item.id)])]})),
    heldout:catalog.heldout.filter(row => native.has(row.actualAcceptanceCase)).map(row => ({id:row.id,cases:[row.actualAcceptanceCase]})),
    backstops:catalog.backstops.filter(row => native.has(row.id)).map(row => ({id:row.id,cases:[row.id]})),
    decisions:catalog.decisions.map(row => ({id:row.id,cases:row.id==='D-31'||row.id==='D-32'?[row.id]:
      [...new Set([...resolve(row.sourceCases),...(row.actualCases??[]).filter(id => native.has(id))])]})),
    judgments:judgment.map(row => ({id:row.id,cases:[row.id,...row.cases]}))
  };
}

function validateDriver(row, item) {
  e.need(row.id === item.id && e.named(row.stimulus) && Array.isArray(row.steps) && row.steps.length > 0 && row.steps.length <= 24,
    'RENDERED_STIMULUS', item.id);
  e.exact(row.steps.map(step => step.id), row.steps.map(step => step.id), 'physical checkpoint IDs');
  for (const step of row.steps) {
    const click = step.action === 'click' && Number.isInteger(step.x) && step.x >= 0 && step.x < 1280 &&
      Number.isInteger(step.y) && step.y >= 0 && step.y < 720;
    const tap = step.action === 'tap' && Number.isInteger(step.key) && step.key > 0 && step.key < 256;
    e.need(/^[a-z][a-z0-9-]{0,47}$/.test(step.id) && (click || tap) && typeof step.node === 'string' &&
      /^\/root\/Inspection(?:\/[a-zA-Z0-9_.@-]+)*$/.test(step.node) &&
      ['text','visible','disabled','value','selected','item_count','playing','texture','size'].includes(step.property) &&
      (['string','number','boolean'].includes(typeof step.expected) || step.expected === null), 'RENDERED_CHECKPOINT', row.id);
  }
}

function reviewedDrivers(data, gate, current, native, kind) {
  e.identity(current,data,'rendered input');
  e.need(data.schemaVersion === 1 && data.selectionSha256 === gate.selection.sha256 && data.reviewSha256 === gate.review.sha256 &&
    e.UUID.test(data.campaign) && e.instant(data.declaredAt) && data.declaredAt>=gate.reviewedAt && data.kind===kind &&
    Array.isArray(data.drivers),'RENDERED_INPUT_SCHEMA');
  const files = native.filter(row => !row.family);
  e.exact(files.map(row => row.id),data.drivers.map(row => row.id),'saved physical drivers');
  const drivers = new Map(data.drivers.map(row => [row.id,row]));
  for (const row of files) validateDriver(drivers.get(row.id),row);
  if (drivers.has('rendered.packaged-relaunch')) e.need(drivers.get('rendered.packaged-relaunch').steps.length >= 2,'RENDERED_RELAUNCH_CHECKPOINTS');
  return drivers;
}

function treePaths(raw) {
  e.need(Array.isArray(raw) && raw.length % 6 === 0 && raw.length > 0 && raw.length <= 8190, 'RENDERED_NATIVE_TREE');
  const result = new Map(), parents = [];
  for (let offset = 0; offset < raw.length; offset += 6) {
    while (parents.at(-1)?.remaining === 0) parents.pop();
    const [children, name, , id] = raw.slice(offset, offset + 4), parent = parents.at(-1);
    e.need(Number.isInteger(children) && children >= 0 && e.named(name) && Number.isSafeInteger(id) && id > 0 &&
      (offset === 0 ? name === 'root' : parent), 'RENDERED_NATIVE_TREE');
    const node = (parent?.node ?? '') + '/' + name;
    e.need(!result.has(node), 'RENDERED_NATIVE_TREE_DUPLICATE', node);
    result.set(node, id);
    if (parent) parent.remaining--;
    parents.push({node, remaining:children});
  }
  e.need(parents.every(row => row.remaining === 0), 'RENDERED_NATIVE_TREE_INCOMPLETE');
  return result;
}

async function nativeObserver() {
  const {encode, decode} = require('./native-observer.cjs');
  const {hash} = require('./content.cjs');
  let channel, pid, thread, pending, trouble, received = 0;
  const server = net.createServer(socket => {
    if (channel || socket.remoteAddress !== '127.0.0.1') {socket.destroy();return;}
    channel = socket;
    let buffer = Buffer.alloc(0);
    socket.on('error', error => {trouble ??= error;});
    socket.on('data', chunk => {
      try {
        received += chunk.length;
        e.need(received <= 256 * 1024 * 1024, 'RENDERED_NATIVE_PACKET_TOTAL');
        buffer = Buffer.concat([buffer, chunk]);
        while (buffer.length >= 4) {
          const size = buffer.readUInt32LE();
          e.need(size > 0 && size <= 1048576, 'RENDERED_NATIVE_PACKET_SIZE');
          if (buffer.length < size + 4) break;
          const raw = buffer.subarray(4, size + 4);
          buffer = buffer.subarray(size + 4);
          const [message, sourceThread, payload] = decode(raw);
          if (message === 'set_pid') {pid = payload[0];thread = sourceThread;}
          if (['error','debug_enter'].includes(message)) throw new Error('RENDERED_NATIVE_ERROR ' + message);
          if (pending?.response === message) pending.value = {payload,raw,sha256:hash(raw)};
        }
      } catch (error) {trouble ??= error;socket.destroy();}
    });
  });
  await new Promise((resolve,reject) => {server.once('error',reject);server.listen(0,'127.0.0.1',resolve);});
  async function waitFor(test) {
    const end = Date.now() + 12000;
    for (;;) {
      if (trouble) throw trouble;
      const value = test();
      if (value) return value;
      e.need(Date.now() < end, 'RENDERED_NATIVE_DEADLINE');
      await delay(40);
    }
  }
  async function request(message, response, args) {
    e.need(!pending && channel && !channel.destroyed, 'RENDERED_NATIVE_CONNECTION');
    const body = encode([message,thread,args]), header = Buffer.alloc(4);
    header.writeUInt32LE(body.length);
    pending = {response};
    channel.write(Buffer.concat([header,body]));
    try {return await waitFor(() => pending.value);} finally {pending = null;}
  }
  async function probe(ownedPid, step) {
    await waitFor(() => pid);
    e.need(pid === ownedPid, 'RENDERED_NATIVE_PID');
    const tree = await request('scene:request_scene_tree','scene:scene_tree',[]);
    const id = treePaths(tree.payload).get(step.node);
    e.need(id, 'RENDERED_NATIVE_NODE', step.node);
    const inspected = await request('scene:inspect_objects','scene:inspect_objects',[[id],false]);
    const item = inspected.payload.find(value => value[0] === id);
    const property = item?.[2]?.find(value => value[0] === step.property);
    e.need(property && equal(property[5],step.expected), 'RENDERED_NATIVE_PROPERTY', step.node + ' ' + step.property);
    return {node:step.node,property:step.property,value:property[5],tree,inspected};
  }
  return {endpoint:'tcp://127.0.0.1:' + server.address().port, probe, close:() => {
    channel?.destroy();return new Promise(resolve => server.close(resolve));
  }};
}

const CAPTURE_CLIENT = String.raw`
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class Phase4Capture {
 [StructLayout(LayoutKind.Sequential)] public struct Point { public int X; public int Y; }
 [StructLayout(LayoutKind.Sequential)] public struct Rect { public int L; public int T; public int R; public int B; }
 [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
 [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr window, out uint pid);
 [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr window, out Rect rectangle);
 [DllImport("user32.dll")] public static extern bool ClientToScreen(IntPtr window, ref Point point);
 [DllImport("user32.dll")] public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr value);
 [DllImport("dwmapi.dll")] public static extern int DwmFlush();
}
'@
[void][Phase4Capture]::SetThreadDpiAwarenessContext([IntPtr](-4))
$window = [IntPtr]([long]$env:KZ_PHASE4_WINDOW)
$processId = [uint32]0
[void][Phase4Capture]::GetWindowThreadProcessId($window, [ref]$processId)
if ($processId -ne [uint32]$env:KZ_PHASE4_PID -or [Phase4Capture]::GetForegroundWindow() -ne $window) { throw 'CAPTURE_OWNER' }
$rectangle = New-Object Phase4Capture+Rect
$origin = New-Object Phase4Capture+Point
if (-not [Phase4Capture]::GetClientRect($window,[ref]$rectangle) -or -not [Phase4Capture]::ClientToScreen($window,[ref]$origin) -or
    $rectangle.R - $rectangle.L -ne 1280 -or $rectangle.B - $rectangle.T -ne 720 -or [Phase4Capture]::DwmFlush() -ne 0) { throw 'CAPTURE_CLIENT' }
$bitmap = New-Object Drawing.Bitmap(1280,720,[Drawing.Imaging.PixelFormat]::Format32bppArgb)
$graphics = [Drawing.Graphics]::FromImage($bitmap)
$stream = $null
try {
 $graphics.CopyFromScreen($origin.X,$origin.Y,0,0,$bitmap.Size,[Drawing.CopyPixelOperation]::SourceCopy)
 if ([Phase4Capture]::GetForegroundWindow() -ne $window) { throw 'CAPTURE_LOST_FOCUS' }
 $stream = [IO.File]::Open($env:KZ_PHASE4_PNG,[IO.FileMode]::CreateNew)
 $bitmap.Save($stream,[Drawing.Imaging.ImageFormat]::Png)
} finally { if ($stream) { $stream.Dispose() }; $graphics.Dispose(); $bitmap.Dispose() }
`;

function desktopCommand(root, script, env, limit) {
  return require('../phase2/rendered.cjs').command('powershell.exe', ['-NoProfile','-NonInteractive','-EncodedCommand',
    Buffer.from(script,'utf16le').toString('base64')], {cwd:root,env}, limit);
}

async function inspectionSession(root, relative, group, build, adapter) {
  const phase2 = require('../phase2/rendered.cjs');
  const role = group === 'G1-hub' ? 'inspection-hub' : 'packaged-inspection';
  const target = role === 'inspection-hub' ? build.engine : build.package;
  e.artifact(root,target);
  const executable = path.join(root,target.path);
  const project = role === 'inspection-hub' ? ['--path',path.join(root,'src/godot-game'),'res://inspection/Inspection.tscn'] : [];
  const directory = path.join(root,relative);
  fs.mkdirSync(directory,{recursive:true});
  const profile = path.join(directory,'profile');
  fs.mkdirSync(profile);
  const env = {...adapter.env, APPDATA:profile, LOCALAPPDATA:profile, USERPROFILE:profile};
  const native = await nativeObserver();
  const args = ['--resolution','1280x720','--remote-debug',native.endpoint,...project];
  const startedAt = new Date().toISOString();
  let log = '', error, inputReleased = false, timedOut = false, terminal, frame = 0;
  const child = spawn(executable,args,{cwd:path.dirname(executable),env,windowsHide:false,shell:false,stdio:['ignore','pipe','pipe']});
  const pid = child.pid;
  const closed = new Promise(resolve => child.once('close',(exitCode,signal) => {
    terminal = {exitCode,signal};resolve(terminal);
  }));
  child.on('error',caught => {error ??= caught;});
  for (const stream of [child.stdout,child.stderr]) stream.on('data',data => {
    if (log.length < 8 * 1024 * 1024) log += data;
    else {error ??= new Error('RENDERED_LOG_LIMIT');phase2.killTree(child);}
  });
  let acknowledgement;
  const input = adapter.physical(async (action,x,y,key) => {
    e.need(pid && !terminal && !error, 'RENDERED_PROCESS_LOST', role);
    const result = await phase2.command('powershell.exe',['-NoProfile','-ExecutionPolicy','Bypass','-File',
      path.join(root,'tools/phase2/windows-input.ps1'),'-OwnedPid',String(pid),'-Action',action,'-X',String(x),'-Y',String(y),'-Key',String(key)],{cwd:root,env},12000);
    acknowledgement = JSON.parse(result.stdout.trim());
    e.need(acknowledgement.pid === pid && acknowledgement.focused && acknowledgement.action === action &&
      acknowledgement.width === 1280 && acknowledgement.height === 720, 'RENDERED_INPUT_ACK');
  });
  async function checkpoint(item,step,invocation) {
    const prefix = relative + '/' + item.id + '-' + step.id;
    const deadline = Date.now() + 12000;
    for (;;) {
      try {await input.input('probe');break;} catch (failure) {
        if (terminal || error || Date.now() >= deadline) throw failure;
        await delay(150);
      }
    }
    await input.input(step.action,step.x ?? 0,step.y ?? 0,step.key ?? 0);
    const at = new Date().toISOString(), physical = acknowledgement;
    const observed = await native.probe(pid,step);
    const nativeTree = e.writeExclusive(root,prefix+'-native-tree.bin',observed.tree.raw);
    const nativeObject = e.writeExclusive(root,prefix+'-native-object.bin',observed.inspected.raw);
    const imagePath = prefix + '.png';
    await desktopCommand(root,CAPTURE_CLIENT,{...env,KZ_PHASE4_PID:String(pid),KZ_PHASE4_WINDOW:String(physical.window),
      KZ_PHASE4_PNG:path.join(root,imagePath)},12000);
    adapter.decodePng(path.join(root,imagePath),1280,720);
    const image = e.reference(root,imagePath,16 * 1024 * 1024);
    const flags = {caseId:item.id,checkpoint:step.id,invocation,pid};
    const inputRef = e.writeExclusive(root,prefix+'-input.json',{...flags,at,focused:true,action:step.action,
      clientWidth:physical.width,clientHeight:physical.height,method:step.action==='click'?'physical-mouse':'physical-keyboard',
      x:step.x,y:step.y,key:step.key,acknowledgement:physical});
    const draw = e.writeExclusive(root,prefix+'-draw.json',{...flags,at:new Date().toISOString(),drawn:true,frame:++frame,
      observation:JSON.stringify({node:observed.node,property:observed.property,value:observed.value}),
      frameSource:'composited-client-after-DwmFlush',nativeTree,nativeObject,imageSha256:image.sha256});
    return {id:step.id,input:inputRef,draw,image};
  }
  async function close() {
    try {
      await input.release();inputReleased = true;
      if (!terminal && pid) await desktopCommand(root,"$ErrorActionPreference='Stop'; $p=Get-Process -Id ([int]$env:KZ_PHASE4_PID); if(-not $p.CloseMainWindow()){throw 'OWNED_QUIT'}",
        {...env,KZ_PHASE4_PID:String(pid)},6000);
      if (!terminal) await Promise.race([closed,delay(12000)]);
    } catch (caught) {error ??= caught;}
    finally {
      if (!terminal) {timedOut = true;phase2.killTree(child);await Promise.race([closed,delay(3000)]);}
      await native.close();
    }
    const logRef = e.writeExclusive(root,relative+'/console.log',Buffer.from(log));
    const record = {role,pid,startedAt,finishedAt:new Date().toISOString(),timedOut,
      crashed:Boolean(error) || terminal?.exitCode !== 0 || /ERROR:|SCRIPT ERROR:|Failed to load/i.test(log),
      exitCode:terminal?.exitCode ?? null,signal:terminal?.signal ?? null,streamsClosed:Boolean(terminal),inputReleased,
      remainingPids:terminal?[]:[pid],log:logRef,executable:target};
    return {record,ref:e.writeExclusive(root,relative+'/terminal.json',record)};
  }
  return {checkpoint,close,get usable(){return Boolean(pid && !terminal && !error);}};
}

async function runInspection(root, relative, group, items, drivers, build, adapter, invocation) {
  const outcomes = [];
  const sessions = [];
  let session, currentAttempt = [];
  const deadline = Date.now() + group.timeoutMs;
  const finish = async () => {
    if (!session) return;
    try {
      const terminal = await session.close();
      for (const row of currentAttempt) row.processes.push(terminal.ref);
      e.validateProcess(terminal.record);
    } catch (error) {
      for (const row of currentAttempt.filter(row => row.status === 'pass')) {
        row.status = 'invalid';row.observed = error.message;
      }
    } finally {session = null;currentAttempt = [];}
  };
  const launch = async () => {
    const owned = relative + '/' + group.id + '-launch-' + (sessions.length + 1);
    session = await inspectionSession(root,owned,group.id,build,adapter);
    sessions.push(owned);
  };
  try {
    for (const item of items) {
      const row = {id:item.id,runGroup:item.runGroup,invocation,status:'pass',execution:'executed-now',
        expected:e.expectedOutcome(item),observed:'Current native observation and physical input recorded',checkpoints:[],processes:[]};
      outcomes.push(row);
      try {
        if (!sessions.length) await launch();
        currentAttempt.push(row);
        e.need(session?.usable && Date.now() < deadline,'RENDERED_GROUP_UNAVAILABLE',group.id);
        const driver = drivers.get(item.id);
        for (const [index,step] of driver.steps.entries()) {
          e.need(Date.now() < deadline,'RENDERED_GROUP_DEADLINE',group.id);
          row.checkpoints.push(await session.checkpoint(item,step,invocation));
          if (item.id === 'rendered.packaged-relaunch' && index === 0) {
            await finish();e.need(row.status === 'pass','RENDERED_RELAUNCH_FIRST_EXIT');
            await launch();currentAttempt.push(row);
          }
        }
      } catch (error) {
        row.status = row.checkpoints.length || session?.usable ? 'fail' : 'blocked';
        row.observed = error.message.slice(0,4096).replace(/[\r\n]/g,' ');
      }
    }
  } finally {await finish();}
  return outcomes;
}

async function runInherited(root, relative, group, items, build, adapter, identity, invocation) {
  const original = items.map(item => ({...item,id:item.originalId,runGroup:item.inheritedGroup}));
  const directory = path.join(root,relative,group.id);
  fs.mkdirSync(directory,{recursive:true});
  const engine = path.join(root,build.engine.path);
  const {revision,source:input,tool} = identity;
  const context = {root,directory,engine,env:adapter.env,revision,input,tool,nonce:invocation,platform:'win32-x64',
    osFingerprint:require('./content.cjs').hash(JSON.stringify({platform:process.platform,architecture:process.arch,release:os.release(),version:os.version()}))};
  const solo = group.id === 'G3-solo';
  const executed = solo ? {results:await adapter.solo(context,original)} : await adapter.lan(context,original);
  const bindings = {nonce:invocation,input,tool,revision,platform:context.platform,osFingerprint:context.osFingerprint};
  const outcomes = [];
  for (const [index,item] of items.entries()) {
    const result = executed.results.find(row => row.id === item.originalId);
    let status = result?.status ?? 'blocked', observed = result?.message ?? 'Inherited original case executed on current Phase 4 build';
    if (status === 'pass') try {
      if (solo) require('../phase2/evidence.cjs').reconcile([original[index]],[result],context);
      else require('../phase3/evidence.cjs').reconcile([original[index]],[result],context);
    } catch (error) {status = 'invalid';observed = error.message;}
    const inherited = e.writeExclusive(root,relative+'/'+group.id+'/'+item.originalId+'-result.json', {bindings,result:result??null});
    outcomes.push({id:item.id,runGroup:item.runGroup,invocation,status,execution:'executed-now',
      expected:e.expectedOutcome(item),observed:observed.slice(0,4096).replace(/[\r\n]/g,' '),inherited});
  }
  return outcomes;
}

async function runRendered(root, registered) {
  const catalog = loadCatalog(root,registered);
  const current = currentIdentity(root);
  const relative = runId(), invocation = path.basename(relative);
  const gate = prelaunch(root,catalog,current,{receiptRoot:relative});
  const runInput = e.reference(root,RUNS);
  const data = e.document(root,runInput);
  const allNative = gate.selected.filter(row => row.needsGameWindow);
  const previous = data.previousRun ? e.document(root,data.previousRun) : null;
  e.need(!previous || previous.campaign === data.campaign && e.UUID.test(previous.invocation) &&
    data.previousRun.path === '.phase4/acceptance/'+previous.invocation+'/report.json','RENDERED_PREVIOUS_RUN');
  e.need(previous ? ['focused','final'].includes(data.kind) : data.kind==='initial','RENDERED_ATTEMPT_KIND');
  let native = allNative;
  if (data.kind==='focused') {
    e.need(Array.isArray(data.caseIds) && data.caseIds.length>0 && e.named(data.repairReason),'RENDERED_FOCUSED_SELECTION');
    native = allNative.filter(row => data.caseIds.includes(row.id));
    e.exact(native.map(row => row.id),data.caseIds,'focused selected cases');
    e.need(previous.outcomes.filter(row => row.status!=='pass').every(row => data.caseIds.includes(row.id)), 'RENDERED_FOCUSED_MISSING');
  }
  const drivers = reviewedDrivers(data,gate,current,native,data.kind);
  const review = e.document(root,gate.review), build = e.document(root,review.buildReport);
  for (const ref of [build.engine,build.package]) {
    e.need(build.artifacts.some(artifact => equal(artifact,ref)),'RENDERED_BUILD_TARGET');
    e.artifact(root,ref);
  }
  const engineHash = '45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab';
  e.need(build.engine.sha256 === engineHash && build.package.path.startsWith('.phase4/') &&
    build.package.path.endsWith('.exe'),'RENDERED_EXECUTABLE');
  const driverRef = e.writeExclusive(root,relative+'/declared-drivers.json',e.artifact(root,runInput).data);
  const startedAt = new Date().toISOString(), outcomes = [];
  e.need(data.declaredAt <= startedAt,'RENDERED_DRIVER_DECLARATION_TIME');
  for (const group of gate.groups.filter(row => GROUPS.includes(row.id))) {
    const items = native.filter(row => row.runGroup === group.id);
    if (!items.length) continue;
    let unlock;
    try {
      if (group.id !== 'G3-lan') unlock = require('../phase2/check.cjs').acquireLock(root,invocation);
      const result = group.id.startsWith('G3') ? await runInherited(root,relative,group,items,build,gate.adapter,current,invocation) :
        await runInspection(root,relative,group,items,drivers,build,gate.adapter,invocation);
      outcomes.push(...result);
    } catch (error) {
      for (const row of items) outcomes.push({id:row.id,runGroup:row.runGroup,invocation,status:'blocked',execution:'executed-now',
        expected:e.expectedOutcome(row),observed:error.message.slice(0,4096).replace(/[\r\n]/g,' ')});
    } finally {
      try {unlock?.();} catch (error) {
        for (const row of outcomes.filter(row => row.runGroup===group.id && row.status==='pass')) {
          row.status='invalid';row.observed=error.message;
        }
      }
    }
  }
  let finishedAt = new Date().toISOString();
  if (finishedAt === startedAt) finishedAt = new Date(Date.parse(startedAt)+1).toISOString();
  let unchanged = false;
  try {e.identity(current,currentIdentity(root),'post-run');unchanged = true;} catch (error) {
    e.writeExclusive(root,relative+'/identity-failure.json',{at:finishedAt,message:error.message});
  }
  for (const row of outcomes) if (row.status === 'pass' && !unchanged) {row.status = 'invalid';row.observed = 'Current source or build drifted during this run';}
  const report = {...current,schemaVersion:1,invocation,campaign:data.campaign,startedAt,finishedAt,
    previousInvocation:previous?.invocation??null,kind:data.kind,repairReason:data.repairReason,selected:native.map(row => row.id),outcomes,
    drivers:driverRef,reviewSha256:gate.review.sha256,selectionSha256:gate.selection.sha256};
  for (const row of outcomes.filter(row => row.status==='pass' && drivers.has(row.id))) try {
    e.validateRenderedOutcome(root,row,report,drivers.get(row.id));
  } catch (error) {
    row.status = 'invalid';row.observed = error.message.slice(0,4096).replace(/[\r\n]/g,' ');
  }
  report.status = outcomes.every(row => row.status === 'pass')?'pass':'fail';
  e.reconcile(native,report,current);
  const ref = e.writeExclusive(root,relative+'/report.json',report);
  return {status:report.status,invocation,executed:outcomes.length,report:ref};
}

async function verifyCompletion(root, relative, registered) {
  const index = e.document(root, relative);
  e.need(index?.schemaVersion === 1 && index.scope === 'phase-04-windows-resource-delivery' &&
    Array.isArray(index.runs) && index.runs.length > 0, 'COMPLETION_SCHEMA');
  const catalog = loadCatalog(root, registered);
  e.need(equal(index.deferred, catalog.deferred), 'COMPLETION_DEFERRED_OWNERS');
  const current = currentIdentity(root);
  const gate = prelaunch(root, catalog, current, {prerequisites:false});
  e.identity(current, index, 'completion');
  e.need(index.reviewSha256 === gate.review.sha256 && index.selectionSha256 === gate.selection.sha256, 'COMPLETION_BINDINGS');
  const final = finalRendered(root,gate,current,index.runs,index.campaign);
  const receipts = completionSourceReceipts(root,gate.selected,index.sourceResults,current);
  for (const row of final.outcomes) receipts.add(row.id);
  e.need(index.listening?.path===e.LISTENING && index.judgments?.path===e.JUDGMENTS,'COMPLETION_NAMED_PATHS');
  for (const ref of [index.listening,index.judgments]) e.committedRef(root,gate.head,ref);
  const audio = e.listeningExpectations(root), judgments = e.judgmentExpectations(catalog);
  e.validateJudgments(root,e.document(root,index.listening),e.document(root,index.judgments),current,audio,judgments,final);
  for (const row of [...audio,...judgments]) receipts.add(row.id);
  const review = e.document(root,gate.review), build = e.document(root,review.buildReport);
  e.need(equal(build.package,index.export) && e.artifact(root,index.export).bytes>0,'COMPLETION_EXPORT');
  const artifacts = e.document(root,index.artifacts), audit = e.document(root,index.sourceAudit);
  const expected = e.inventoryExpectation(root,current.revision,catalog);
  e.validateArtifactInventory(root,artifacts,expected,audit,current);
  e.validateNativeHostProof(root,index.nativeProof,index.sourceResults.find(row => row.id==='content.native-permissions')?.result,expected.guarded);
  for (const row of artifacts.archives) {
    const result = await require('./content.cjs').archive(root,{path:row.path,sha256:row.sha256,bytes:row.bytes});
    e.need(equal(row.members,result.members),'EVIDENCE_ARCHIVE_CURRENT',row.path);
  }
  receipts.add('acceptance.artifacts');
  e.validateCoverage(index,coverageExpectation(catalog,root),receipts);
  return {status:'pass',invocation:final.invocation,executed:receipts.size,scope:'Current Windows resource delivery'};
}

module.exports = {CATALOG, COVERAGE, SCENARIOS, GROUPS, ORDER, coverageRows, validateProcedure, validateCatalog, loadCatalog,
  selectCases, inheritedCases, selectionRows, impactClosure, validateSelection, currentReviewAdapter, prelaunch, currentIdentity, runId,
  finalRendered, latestPhase4Sources, sourceReceipts, completionSourceReceipts, preWindowCases, verifyFixProof, coverageExpectation, validateDriver, reviewedDrivers,
  treePaths, runInspection, runInherited, runRendered, verifyCompletion};
