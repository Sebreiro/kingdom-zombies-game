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
const owned = require('./rendered-process.cjs');
const delivery = require('./export.cjs');
const contracts = require('./rendered-contracts.cjs');

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
  ['CR-07',['acceptance.judgments','audio.lifecycle','audio.heldout']], ['CR-08',['hub.source','export.hub-consumer','content.census']],
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
  validateBuild(root,build,source,tools.tool);
  return {revision:source.revision, source:source.source, media:source.media, tool:tools.tool, build:review.buildReport.sha256};
}

const MANAGED_NAMES = ['KingdomZombies','KingdomZombies.Core','KingdomZombies.Multiplayer.Abstractions','GodotSharp'];
const NATIVE_ENGINE_HASH = '45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab';
const SOURCE_LOAD_CONTEXT = 'GodotPlugins.PluginLoadContext';
const DOTNET='C:/Program Files/dotnet/dotnet.exe';

async function prepareSource(root,revision) {
  const source=e.identifySource(root,revision),tool=e.identifyTool(root);
  const nonce=crypto.randomUUID(), ownership='.phase4/runs/'+nonce+'/', directory=path.resolve(root,ownership);
  const copy=ownership+'source/', relative=copy+'src/godot-game', project=path.resolve(root,relative);
  const {command,environment}=require('./check.cjs');
  const context={directory,nonce,revision,caseId:'rendered-source-preparation'};
  const pin=e.document(root,'tools/phase1/toolchain.json');
  const win=pin.archives['win32-x64'];
  const engine=e.reference(root,'.phase1/tools/'+win.directory+'/'+win.executable.replace('_console.exe','.exe'),256*1024*1024);
  e.need(engine.sha256===NATIVE_ENGINE_HASH,'RENDERED_EXECUTABLE');
  e.committedRef(root,e.git(root,['rev-parse','HEAD']).trim(),e.reference(root,e.TARGETS));
  for (const ref of source.files.filter(row=>/^(?:src\/|tools\/code-rules\/|Directory\.|BannedSymbols|\.(?:editorconfig|gitattributes)$|global\.json$|NuGet\.config$|KingdomZombies\.slnx$)/.test(row.path))) {
    const stored=e.writeExclusive(root,copy+ref.path,e.artifact(root,ref).data,ref.bytes);
    e.need(stored.bytes===ref.bytes&&stored.sha256===ref.sha256,'RENDERED_SOURCE_COPY',ref.path);
  }
  const godotFeed=pin.godotPackages.map(pkg=>{
    const file='.phase1/feed/'+pkg.id+'.'+pkg.version+'.nupkg';
    const packageFile=e.read(root,file);
    e.need(packageFile.sha256===pkg.sha256,'RENDERED_SOURCE_GODOT_FEED',file);
    return e.writeExclusive(root,copy+file,packageFile.data,packageFile.bytes);
  });
  const env=environment(directory);
  const sdk=await command(context,'rendered-source-sdk',DOTNET,['--version'],{env});
  e.need(sdk.stdout.trim()===pin.sdk,'RENDERED_SOURCE_SDK');
  const importArgs=['--headless','--path',project,'--editor','--import'];
  await command(context,'rendered-source-import',path.resolve(root,engine.path),importArgs,{env});
  const isolated=['--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false',
    '-p:RestorePackagesPath='+path.join(root,'.phase1/packages'),
    '-p:RestoreSources='+path.join(root,copy,'.phase1/feed'),'-v:minimal'];
  await command(context,'rendered-source-build',DOTNET,['build',path.join(project,'KingdomZombies.csproj'),'-c','Debug',...isolated,
    '-p:SourceRevisionId='+revision,'-p:IncludeSourceRevisionInInformationalVersion=true',
    '-p:CustomAfterMicrosoftCommonTargets='+path.join(root,e.TARGETS)],{env});
  const build={revision,source:source.source,media:source.media,tool:tool.tool,engine,sourceProject:{path:relative,
    import:e.reference(root,ownership+'rendered-source-import.command.json'),
    build:e.reference(root,ownership+'rendered-source-build.command.json'),
    sdk:e.reference(root,ownership+'rendered-source-sdk.command.json'),godotFeed,
    imports:delivery.inventory(path.join(project,'.godot','imported')),
    importCompanions:sourceImportCompanions(root,relative,source),
    managed:delivery.inventory(path.join(project,'.godot','mono','temp','bin','Debug'))}};
  validateSourceProject(root,build,source);
  return e.writeExclusive(root,ownership+'rendered-source.json',build);
}

function validateBuild(root,build,source,tool) {
  e.need(build.schemaVersion === 2 && build.status === 'pass' && build.revision === source.revision &&
    build.source === source.source && build.media === source.media && build.tool === tool,'REVIEW_BUILD');
  const pin = e.document(root,'tools/phase1/toolchain.json').archives['win32-x64'];
  e.need(build.engine?.path === '.phase1/tools/'+pin.directory+'/'+pin.executable.replace('_console.exe','.exe') &&
    build.engine.sha256 === NATIVE_ENGINE_HASH,'RENDERED_EXECUTABLE');
  e.artifact(root,build.engine);
  const item = loadCatalog(root).cases.find(row => row.id === 'export.portability');
  const {result} = phase4SourceReceipt(root,item,{result:build.deliveryResult},source);
  const receipt = e.document(root,build.delivery);
  const directory = '.phase4/runs/'+result.nonce+'/portable/';
  e.need(build.delivery.path === '.phase4/runs/'+result.nonce+'/portable-delivery.json' &&
    build.package?.path === directory+'KingdomZombies.exe' && build.pck?.path === directory+'KingdomZombies.pck' &&
    equal(receipt,result.observations) && receipt.revision === result.revision && receipt.nonce === result.nonce &&
    equal(result.testedSource,{source:source.source,media:source.media}) && e.sameCommittedSource(root,result.revision,source.revision),
    'REVIEW_BUILD_DELIVERY_RECEIPT');
  const portable = delivery.validatePortableReceipt(root,build,receipt);
  const pack = e.artifact(root,build.pck,64*1024*1024);
  e.exact(Object.keys(delivery.RENDERED_FIXTURES),Object.keys(build.renderedFixtures ?? {}),'rendered PCK fixtures');
  for (const id of Object.keys(delivery.RENDERED_FIXTURES)) delivery.verifyRenderedFixture(pack.data,id,build.renderedFixtures[id]);
  delivery.packaged(path.dirname(path.join(root,build.package.path)));
  const sourceManaged=validateSourceProject(root,build,source);
  return {...build,portable,sourceManaged};
}

function validateSourceProject(root,build,source) {
  const prepared = build.sourceProject;
  const match = /^\.phase4\/runs\/([a-f0-9-]+)\/source\/src\/godot-game$/.exec(prepared?.path ?? '');
  e.need(match && e.UUID.test(match[1]),'RENDERED_SOURCE_PROJECT');
  const ownership = '.phase4/runs/'+match[1]+'/', copy = ownership+'source/';
  const project = path.resolve(root,prepared.path);
  const tracked = source.files.filter(ref => ref.path.startsWith('src/godot-game/'));
  e.need(equal(prepared.importCompanions,sourceImportCompanions(root,prepared.path,source)),
    'RENDERED_SOURCE_IMPORT_COMPANIONS');
  for (const ref of tracked) {
    if (ref.path !== 'src/godot-game/project.godot') e.artifact(root,{...ref,path:copy+ref.path});
    else {
      const original=e.artifact(root,ref).data.toString('utf8'), actual=e.read(root,copy+ref.path).data.toString('utf8');
      const newline=original.includes('\r\n')?'\r\n':'\n';
      const block=['[editor]','','import/use_multiple_threads=false',''].join(newline);
      const expected=original.endsWith(newline+block)?original:original+(original.endsWith('\n')?'':newline)+newline+block;
      e.need(actual===expected,'RENDERED_SOURCE_PROJECT_SETTINGS');
    }
  }
  function execution(ref, name) {
    e.need(ref?.path === ownership+name+'.command.json','RENDERED_SOURCE_PREPARATION',name);
    const row=e.document(root,ref);
    e.need(row.name===name && row.code===0 && row.signal===null && row.streamsClosed===true && !row.reason &&
      !row.spawnError && !row.cleanupError && !/ERROR:|SCRIPT ERROR:|Unpaired surrogate/i.test(row.stdout+row.stderr),
      'RENDERED_SOURCE_PREPARATION',name);
    return row;
  }
  const imported=execution(prepared.import,'rendered-source-import');
  const compiled=execution(prepared.build,'rendered-source-build');
  const sdk=execution(prepared.sdk,'rendered-source-sdk');
  const pin=e.document(root,'tools/phase1/toolchain.json');
  const expectedFeed=pin.godotPackages.map(pkg=>({path:copy+'.phase1/feed/'+pkg.id+'.'+pkg.version+'.nupkg',sha256:pkg.sha256}));
  e.need(Array.isArray(prepared.godotFeed)&&equal(prepared.godotFeed.map(({path,sha256})=>({path,sha256})),expectedFeed),
    'RENDERED_SOURCE_GODOT_FEED');
  for (const ref of prepared.godotFeed) e.artifact(root,ref);
  e.need(sdk.stdout.trim()===pin.sdk && equal(sdk.args,['--version']) && [sdk,compiled].every(row =>
    path.resolve(row.executable)===path.resolve(DOTNET)),
    'RENDERED_SOURCE_SDK');
  e.need(equal(imported.args,['--headless','--path',project,'--editor','--import']) &&
    path.resolve(imported.executable)===path.resolve(root,build.engine.path),'RENDERED_SOURCE_IMPORT_ENGINE');
  e.need(compiled.args?.includes(path.join(project,'KingdomZombies.csproj')) &&
    compiled.args.includes('-p:SourceRevisionId='+source.revision) && compiled.args.includes('-p:IncludeSourceRevisionInInformationalVersion=true') &&
    compiled.args.includes('-p:CustomAfterMicrosoftCommonTargets='+path.join(root,e.TARGETS)) &&
    compiled.args.includes('-p:RestoreSources='+path.join(root,copy,'.phase1/feed')),
    'RENDERED_SOURCE_COMPILED_REVISION');
  const imports=delivery.inventory(path.join(project,'.godot','imported'));
  e.need(imports.length>0 && equal(imports,prepared.imports),'RENDERED_SOURCE_IMPORT_CLOSURE');
  const managed=delivery.inventory(path.join(project,'.godot','mono','temp','bin','Debug'));
  e.need(equal(managed,prepared.managed),'RENDERED_SOURCE_MANAGED_CLOSURE');
  return Object.fromEntries(MANAGED_NAMES.map(name => {
    const file=prepared.path+'/.godot/mono/temp/bin/Debug/'+name+'.dll';
    const artifact=e.read(root,file,64*1024*1024);
    e.need(name==='GodotSharp' || artifact.data.includes(Buffer.from('+'+source.revision)),'RENDERED_SOURCE_STALE_DLL',name);
    return [name,{sha256:artifact.sha256,mvid:owned.managedMvid(artifact.data),file}];
  }));
}

function sourceImportCompanions(root,relative,source) {
  const authored=new Set(source.files.filter(ref=>ref.path.startsWith('src/godot-game/'))
    .map(ref=>ref.path.slice('src/godot-game/'.length)));
  const generated=[];
  function visit(directory,prefix) {
    for (const entry of fs.readdirSync(directory,{withFileTypes:true})) {
      const name=prefix+entry.name;
      e.need(!entry.isSymbolicLink(),'RENDERED_SOURCE_REPARSE',name);
      if (name==='.godot') continue;
      if (entry.isDirectory()) visit(path.join(directory,entry.name),name+'/');
      else if (!authored.has(name)) {
        const base=name.replace(/\.(?:uid|import)$/,'');
        e.need(base!==name&&authored.has(base),'RENDERED_SOURCE_UNEXPECTED_FILE',name);
        const {data,...ref}=e.read(root,relative+'/'+name);
        if (name.endsWith('.uid')) e.need(/^uid:\/\/[a-z0-9]+\r?\n?$/.test(data.toString('utf8')),'RENDERED_SOURCE_SCRIPT_UID',name);
        else {
          const text=data.toString('utf8');
          e.need(text.includes('source_file="res://'+base+'"'),'RENDERED_SOURCE_IMPORT_OWNER',name);
          const targets=[...text.matchAll(/^(?:path(?:\.[\w]+)?)="res:\/\/(\.godot\/imported\/[^"\r\n]+)"/gm)];
          e.need(targets.length>0,'RENDERED_SOURCE_IMPORT_TARGET',name);
          for(const target of targets)e.reference(root,relative+'/'+target[1],64*1024*1024);
        }
        generated.push(ref);
      }
    }
  }
  visit(path.resolve(root,relative),'');
  for(const name of ['uid_cache.bin','global_script_class_cache.cfg'])generated.push(e.reference(root,relative+'/.godot/'+name));
  return generated.sort((a,b)=>a.path.localeCompare(b.path));
}

function sourceLaunch(root,relative,build,role) {
  const launch = crypto.randomUUID(), stream = relative+'/loaded-assemblies.ndjson';
  fs.mkdirSync(path.resolve(root,relative),{recursive:true});
  e.writeExclusive(root,relative+'/owner',Buffer.from(launch));
  const env = {...delivery.cleanEnvironment(path.resolve(root,relative,'profile')),
    DOTNET_ROOT:'C:\\Program Files\\dotnet', DOTNET_ROOT_X64:'C:\\Program Files\\dotnet',
    KZ_PHASE4_ATTEST_ID:launch};
  return {launch,stream,env,role};
}

function validateLoadedSource(root,build,run) {
  const data=e.artifact(root,run.assemblies,1024*1024).data.toString('utf8');
  e.need(data.endsWith('\n'),'RENDERED_SOURCE_RUNTIME_STREAM');
  const observations=data.trimEnd().split('\n').map(line => JSON.parse(line));
  const required=MANAGED_NAMES.filter(name=>name!=='GodotSharp');
  e.exact(observations.map(row=>row.name),observations.map(row=>row.name),'distinct loaded assemblies');
  e.need(required.every(name=>observations.some(row=>row.name===name)),'RENDERED_SOURCE_RUNTIME_ASSEMBLIES',run.role);
  for (const row of observations) {
    const declared=build.sourceManaged[row.name];
    e.need(declared && row.launch===run.launch && row.pid===run.pid &&
      path.resolve(row.executable)===path.resolve(root,build.engine.path) &&
      (row.file==='' || path.resolve(row.file)===path.resolve(root,declared.file)) &&
      row.loadContext===SOURCE_LOAD_CONTEXT &&
      row.mvid===declared.mvid &&
      (row.name==='GodotSharp'||row.version?.endsWith('+'+build.revision)),'RENDERED_SOURCE_RUNTIME_IDENTITY',String(row.name));
  }
  return observations;
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
  const build = validateBuild(root,e.document(root,e.document(root,gate.review).buildReport),e.identifySource(root,current.revision),current.tool);
  validateDriverFixtures(root,drivers,build,gate,current);
  e.need(driverRecord.campaign === final.campaign && driverRecord.declaredAt <= final.startedAt,'COMPLETION_DRIVER_TIME');
  for (const [index,row] of final.outcomes.entries()) {
    const item = native[index];
    if (!item.family) e.validateRenderedOutcome(root,row,final,drivers.get(row.id),build);
    else {
      const {result,bindings,runtime} = e.document(root,row.inherited);
      const directory = path.resolve(root,path.dirname(row.inherited.path));
      e.need(bindings.nonce === final.invocation && bindings.revision === current.revision &&
        bindings.input === current.source && bindings.tool === current.tool && bindings.platform === 'win32-x64',
      'COMPLETION_INHERITED_BINDINGS',item.id);
      const original = {...item,id:item.originalId,runGroup:item.inheritedGroup};
      const context = {root,directory,...bindings};
      validateInheritedRuntime(root,build,item.family,result,runtime);
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
  return contracts.validateDeclaration(row,item);
}

function relaunchCase(id) {
  return id === 'heldout.H31' || id === 'rendered.packaged-relaunch';
}

function reviewedDrivers(data, gate, current, native, kind) {
  e.identity(current,data,'rendered input');
  e.need(data.schemaVersion === 2 && data.selectionSha256 === gate.selection.sha256 && data.reviewSha256 === gate.review.sha256 &&
    e.UUID.test(data.campaign) && e.instant(data.declaredAt) && data.declaredAt>=gate.reviewedAt && data.kind===kind &&
    Array.isArray(data.drivers),'RENDERED_INPUT_SCHEMA');
  const files = native.filter(row => !row.family);
  e.exact(files.map(row => row.id),data.drivers.map(row => row.id),'saved physical drivers');
  const drivers = new Map(data.drivers.map(row => [row.id,row]));
  for (const row of files) {
    const driver = drivers.get(row.id);
    validateDriver(driver,row);
    const kind = Object.hasOwn(delivery.RENDERED_FIXTURES,row.id)?'portable-negative':
      row.runGroup==='G2-packaged'?'portable-positive':'inspection-source';
    e.need(driver.fixture?.id===row.id && driver.fixture.kind===kind,'RENDERED_DRIVER_FIXTURE',row.id);
    if (kind==='portable-negative') {
      e.need(driver.fixture?.id === row.id && e.HEX.test(driver.fixture.pckSha256),'RENDERED_DRIVER_FIXTURE',row.id);
    }
    if (relaunchCase(row.id)) {
      const lifetimes = driver.steps.map(step => step.lifetime);
      e.need(lifetimes[0] === 1 && lifetimes.at(-1) === 2 &&
        lifetimes.every((value,index) => [1,2].includes(value) && (index === 0 || value >= lifetimes[index-1])),
        'RENDERED_RELAUNCH_CHECKPOINTS',row.id);
    }
  }
  return drivers;
}

function validateDriverFixtures(root,drivers,build,gate,current) {
  for (const [id,driver] of drivers) {
    const fixture=driver.fixture;
    if (fixture.kind==='inspection-source') {
      e.need(fixture.sourceSha256===current.source,'RENDERED_DRIVER_FIXTURE',id);
      if (id==='heldout.H01') {
        const declared=e.document(root,gate.selection).sourceResults.find(row=>row.id==='source.provenance')?.result;
        e.need(equal(fixture.provenance,declared) && declared,'RENDERED_H01_PROVENANCE');
        const item=loadCatalog(root).cases.find(row=>row.id==='source.provenance');
        const {result}=phase4SourceReceipt(root,item,{result:declared},current);
        e.need(result.observations.heldout?.H01==='pass-source','RENDERED_H01_PROVENANCE');
      }
    } else {
      const expected=fixture.kind==='portable-negative'?build.renderedFixtures[id]:build.pck;
      e.need(fixture.pckSha256===expected.sha256,'RENDERED_DRIVER_FIXTURE',id);
    }
  }
}

function treePaths(raw) {
  e.need(Array.isArray(raw) && raw.length % 6 === 0 && raw.length > 0 && raw.length <= 8190, 'RENDERED_NATIVE_TREE');
  const result = new Map(), parents = [], ids=new Set();
  for (let offset = 0; offset < raw.length; offset += 6) {
    while (parents.at(-1)?.remaining === 0) parents.pop();
    const [children, name, , id] = raw.slice(offset, offset + 4), parent = parents.at(-1);
    e.need(Number.isInteger(children) && children >= 0 && e.named(name) && Number.isSafeInteger(id) && id > 0 &&
      (offset === 0 ? name === 'root' : parent), 'RENDERED_NATIVE_TREE');
    const node = (parent?.node ?? '') + '/' + name;
    e.need(!result.has(node) && !ids.has(id), 'RENDERED_NATIVE_TREE_DUPLICATE', node);
    result.set(node, id);
    ids.add(id);
    if (parent) parent.remaining--;
    parents.push({node, remaining:children});
  }
  e.need(parents.every(row => row.remaining === 0), 'RENDERED_NATIVE_TREE_INCOMPLETE');
  return result;
}

async function nativeObserver(budget, signal) {
  const {encode, decode} = require('./native-observer.cjs');
  const {hash} = require('./content.cjs');
  let channel, pid, pidPacket, thread, pending, trouble, received = 0;
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
          if (message === 'set_pid') {
            e.need(pid===undefined && Array.isArray(payload) && payload.length===1 && Number.isSafeInteger(payload[0]) &&
              payload[0]>0 && Number.isSafeInteger(sourceThread) && sourceThread>0,'RENDERED_NATIVE_PID_PACKET');
            [pid]=payload;thread=sourceThread;pidPacket=raw;
          }
          if (['error','debug_enter'].includes(message)) throw new Error('RENDERED_NATIVE_ERROR ' + message);
          if (pending?.response === message) {
            e.need(sourceThread===thread && !pending.value,'RENDERED_NATIVE_RESPONSE');
            pending.value = {payload,raw,sha256:hash(raw)};
          }
        }
      } catch (error) {trouble ??= error;socket.destroy();}
    });
  });
  await new Promise((resolve,reject) => {server.once('error',reject);server.listen(0,'127.0.0.1',resolve);});
  async function waitFor(test,deadline) {
    const end = Math.min(deadline,Date.now() + budget(12000));
    for (;;) {
      if (trouble) throw trouble;
      const value = test();
      if (value) return value;
      e.need(Date.now() < end, 'RENDERED_NATIVE_DEADLINE');
      await delay(Math.min(40,budget()),undefined,{signal});
    }
  }
  async function request(message, response, args, deadline) {
    e.need(!pending && channel && !channel.destroyed, 'RENDERED_NATIVE_CONNECTION');
    const body = encode([message,thread,args]), header = Buffer.alloc(4);
    header.writeUInt32LE(body.length);
    pending = {response};
    channel.write(Buffer.concat([header,body]));
    try {return await waitFor(() => pending.value,deadline);} finally {pending = null;}
  }
  async function snapshot(ownedPid, checks, deadline=Date.now()+budget(12000)) {
    await waitFor(() => pid,deadline);
    e.need(pid === ownedPid, 'RENDERED_NATIVE_PID');
    const tree = await request('scene:request_scene_tree','scene:scene_tree',[],deadline);
    const nodes=treePaths(tree.payload),objects=new Map();
    for(const node of new Set(checks.filter(check=>check.property!==null).map(check=>check.node))) {
      const id=nodes.get(node);
      if (id) objects.set(node,await request('scene:inspect_objects','scene:inspect_objects',[[id],false],deadline));
    }
    const values=nativeValues(nodes,objects,checks);
    return {tree,objects,values,at:new Date().toISOString()};
  }
  return {endpoint:'tcp://127.0.0.1:' + server.address().port, snapshot, get pidPacket(){return pidPacket;}, close:() => {
    channel?.destroy();return new Promise(resolve => server.close(resolve));
  }};
}

function nativeValues(nodes,objects,checks) {
  return checks.map(check => {
    const id = nodes.get(check.node) ?? null;
    const object=objects.get(check.node);
    const properties=object?.payload.find(row=>row[0]===id)?.[2];
    const values=properties?.filter(row=>row[0]===check.property);
    const value=check.property===null ? id!==null : values?.length===1 ? values[0][5] : null;
    return {id,value};
  });
}

function savedNativeSnapshot(root,ref,checks,thread) {
  const snapshot=e.document(root,ref), decode=require('./native-observer.cjs').decode;
  e.need(e.instant(snapshot.at) && Array.isArray(snapshot.objects),'EVIDENCE_NATIVE_SNAPSHOT');
  const tree=decode(e.artifact(root,snapshot.tree,1048576).data), nodes=treePaths(tree[2]);
  e.need(tree[0]==='scene:scene_tree' && tree[1]===thread,'EVIDENCE_NATIVE_SNAPSHOT_THREAD');
  const required=[...new Set(checks.filter(check=>check.property!==null&&nodes.has(check.node)).map(check=>check.node))];
  e.exact(required,snapshot.objects.map(row=>row.node),'native snapshot subjects');
  const objects=new Map(snapshot.objects.map(row=>{
    const object=decode(e.artifact(root,row.packet,1048576).data);
    e.need(object[0]==='scene:inspect_objects' && object[1]===thread && object[2]?.length===1 &&
      object[2][0][0]===nodes.get(row.node),'EVIDENCE_NATIVE_SNAPSHOT_OBJECT',row.node);
    return [row.node,{payload:object[2]}];
  }));
  return {at:snapshot.at,values:nativeValues(nodes,objects,checks)};
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
@{ pid = $processId; window = $window.ToInt64(); token = $env:KZ_PHASE4_DRAW_TOKEN;
   at = [DateTime]::UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
   imageSha256 = (Get-FileHash -LiteralPath $env:KZ_PHASE4_PNG -Algorithm SHA256).Hash.ToLowerInvariant() } |
   ConvertTo-Json -Compress
`;

function desktopCommand(root, script, env, limit, owner, signal) {
  return owned.command(owner,'powershell.exe', ['-NoProfile','-NonInteractive','-EncodedCommand',
    Buffer.from(script,'utf16le').toString('base64')], {cwd:root,env}, limit,signal);
}

async function verifiedCleanup(owner) {
  try {return await owner.cleanup();}
  catch(error) {throw Object.assign(error,{unconfirmedCleanup:true});}
}

async function inspectionSession(root, relative, group, build, adapter, budget, signal, subject = null) {
  const phase2 = require('../phase2/rendered.cjs');
  const owner = owned.observeChildren();
  const role = group === 'G1-hub' ? 'inspection-hub' : 'packaged-inspection';
  const target = role === 'inspection-hub' ? build.engine : subject.executable;
  e.artifact(root,target);
  const executable = path.join(root,target.path);
  const project = role === 'inspection-hub' ? ['--path',path.join(root,build.sourceProject.path),'res://inspection/Inspection.tscn'] : [];
  const directory = path.join(root,relative);
  fs.mkdirSync(directory,{recursive:true});
  const profile = path.join(directory,'profile');
  fs.mkdirSync(profile);
  const loaded = role === 'inspection-hub' ? sourceLaunch(root,relative,build,role) : null;
  const trace = relative+'/runtime-host.log';
  const tracePath=path.join(root,trace);
  const env = loaded?.env ?? delivery.validatePortableEnvironment({...delivery.cleanEnvironment(profile),COREHOST_TRACE:'1',
    COREHOST_TRACEFILE:tracePath},profile,tracePath);
  const environment = e.writeExclusive(root,relative+'/environment.json',env);
  const native = await nativeObserver(budget,signal);
  const args = ['--resolution','1280x720','--remote-debug',native.endpoint,...project];
  const startedAt = new Date().toISOString();
  let log = '', error, inputReleased = false, timedOut = false, terminal, frame = 0;
  const child = owner.track(spawn(executable,args,{cwd:path.dirname(executable),env,windowsHide:false,shell:false,stdio:['ignore','pipe','pipe']}));
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
  let closing = false;
  let runtime, observedImage, ready;
  const input = adapter.physical(async (action,x,y,key) => {
    e.need(pid && !terminal && !error, 'RENDERED_PROCESS_LOST', role);
    const result = await owned.command(owner,'powershell.exe',['-NoProfile','-ExecutionPolicy','Bypass','-File',
      path.join(root,'tools/phase2/windows-input.ps1'),'-OwnedPid',String(pid),'-Action',action,'-X',String(x),'-Y',String(y),'-Key',String(key)],
      {cwd:root,env},closing?3000:budget(12000),closing?undefined:signal);
    acknowledgement = JSON.parse(result.stdout.trim());
    e.need(acknowledgement.pid === pid && acknowledgement.focused && acknowledgement.action === action &&
      acknowledgement.width === 1280 && acknowledgement.height === 720, 'RENDERED_INPUT_ACK');
  });
  async function settle(checks,{match=true,previous=null}={}) {
    const end=Date.now()+budget(12000);
    for (;;) {
      const observed=await native.snapshot(pid,checks,end);
      const valid=checks.every((check,index)=>contracts.matches(check,observed.values[index].value));
      if ((!match || valid) && previous && observed.at>previous.at && equal(previous.values,observed.values)) return observed;
      previous=(!match || valid)?observed:null;
      e.need(!terminal && !error && Date.now()<end,'RENDERED_NATIVE_SETTLE',checks.map(check=>check.node+' '+check.property).join(', '));
      await delay(Math.min(50,budget()),undefined,{signal});
    }
  }
  function snapshotRef(prefix,snapshot) {
    const tree=e.writeExclusive(root,prefix+'-tree.bin',snapshot.tree.raw);
    const objects=[...snapshot.objects].map(([node,packet],index)=>({node,
      packet:e.writeExclusive(root,prefix+'-object-'+index+'.bin',packet.raw)}));
    return e.writeExclusive(root,prefix+'.json',{at:snapshot.at,tree,objects});
  }
  async function checkpoint(item,step,invocation) {
    const prefix = relative + '/' + item.id + '-' + step.id;
    const deadline = Date.now() + budget(12000);
    for (;;) {
      try {await input.input('probe');break;} catch (failure) {
        if (terminal || error || Date.now() >= deadline) throw failure;
        await delay(Math.min(150,budget()),undefined,{signal});
      }
    }
    if (!runtime) {
      const observer = require('./native-observer.cjs');
      const settled=await settle(contracts.readyChecks);
      ready=snapshotRef(relative+'/ready',settled);
      const state = await native.snapshot(pid,[{node:observer.HUB,property:observer.RUNTIME_PROPERTY}]);
      const marker = observer.runtimeMarker(state.values[0].value,loaded?SOURCE_LOAD_CONTEXT:undefined);
      e.need(loaded || marker === build.portable.managedRuntime,'RENDERED_PORTABLE_LIVE_ASSEMBLY');
      const imageResult = await desktopCommand(root,"$ErrorActionPreference='Stop'; (Get-Process -Id ([int]$env:KZ_PHASE4_PID)).Path",
        {...env,KZ_PHASE4_PID:String(pid)},budget(6000),owner,signal);
      observedImage = imageResult.stdout.trim();
      e.need(path.resolve(observedImage)===path.resolve(executable),'RENDERED_LIVE_IMAGE');
      runtime = e.writeExclusive(root,relative+'/runtime.json',{pid,marker,observedImage,
        pidPacket:e.writeExclusive(root,relative+'/runtime-pid.bin',native.pidPacket),
        tree:e.writeExclusive(root,relative+'/runtime-tree.bin',state.tree.raw),
        object:e.writeExclusive(root,relative+'/runtime-object.bin',state.objects.get(observer.HUB).raw)});
    }
    const before=await native.snapshot(pid,step.checks);
    const token=crypto.randomUUID(), actionStartedAt=new Date().toISOString();
    let recheck,openMenu;
    if(step.recheck) {
      await input.input('click',step.recheck.x,step.recheck.y,0);
      recheck={...step.recheck,at:new Date().toISOString(),acknowledgement};
    }
    if (step.openMenu) {
      await input.input('click',step.openMenu.x,step.openMenu.y,0);
      openMenu={...step.openMenu,at:new Date().toISOString(),acknowledgement};
    }
    await input.input(step.action,step.x ?? 0,step.y ?? 0,step.key ?? 0);
    const at = new Date().toISOString(), physical = acknowledgement;
    const observed = await settle(step.checks);
    e.need(!equal(observed.values,before.values),'RENDERED_NATIVE_TRANSITION',item.id+' '+step.id);
    const imagePath = prefix + '.png';
    const capture=await desktopCommand(root,CAPTURE_CLIENT,{...env,KZ_PHASE4_PID:String(pid),KZ_PHASE4_WINDOW:String(physical.window),
      KZ_PHASE4_DRAW_TOKEN:token,KZ_PHASE4_PNG:path.join(root,imagePath)},budget(12000),owner,signal);
    const visible=JSON.parse(capture.stdout.trim());
    const after=await native.snapshot(pid,step.checks);
    e.need(equal(observed.values,after.values),'RENDERED_NATIVE_DRAW_DRIFT',step.id);
    budget();
    adapter.decodePng(path.join(root,imagePath),1280,720);
    const image = e.reference(root,imagePath,16 * 1024 * 1024);
    const flags = {caseId:item.id,checkpoint:step.id,invocation,pid,token};
    const inputRef = e.writeExclusive(root,prefix+'-input.json',{...flags,at,actionStartedAt,focused:true,action:step.action,
      clientWidth:physical.width,clientHeight:physical.height,method:step.action==='click'?'physical-mouse':'physical-keyboard',
      control:step.control,x:step.x,y:step.y,key:step.key,acknowledgement:physical,recheck,openMenu});
    const draw = e.writeExclusive(root,prefix+'-draw.json',{...flags,at:new Date().toISOString(),drawn:true,frame:++frame,
      before:snapshotRef(prefix+'-before',before),observed:snapshotRef(prefix+'-observed',observed),
      after:snapshotRef(prefix+'-after',after),capture:visible,ready,frameSource:'composited-client-between-native-observations',
      imageSha256:image.sha256});
    const point = {id:step.id,input:inputRef,draw,image};
    if (item.id===contracts.AUDIO_CASE && contracts.AUDIO_PLAYBACK.has(step.id))
      point.playback=e.audioPlayback(e.audioAuthority(root),step.id,before.values[0],observed.values[0]);
    return point;
  }
  async function close() {
    closing = true;
    try {
      await input.release();inputReleased = true;
      if (!terminal && pid) await desktopCommand(root,"$ErrorActionPreference='Stop'; $p=Get-Process -Id ([int]$env:KZ_PHASE4_PID); if(-not $p.CloseMainWindow()){throw 'OWNED_QUIT'}",
        {...env,KZ_PHASE4_PID:String(pid)},signal.aborted?3000:budget(4000),owner);
      if (!terminal) await Promise.race([closed,delay(signal.aborted?2000:budget(10000))]);
    } catch (caught) {error ??= caught;}
    finally {
      if (!terminal) {timedOut = true;phase2.killTree(child);await Promise.race([closed,delay(3000)]);}
      await native.close();
    }
    const remainingPids = await verifiedCleanup(owner);
    const logRef = e.writeExclusive(root,relative+'/console.log',Buffer.from(log));
    const record = {role,pid,startedAt,finishedAt:new Date().toISOString(),timedOut,environment,runtime,ready,
      crashed:Boolean(error) || terminal?.exitCode !== 0 || /ERROR:|SCRIPT ERROR:|Failed to load/i.test(log),
      exitCode:terminal?.exitCode ?? null,signal:terminal?.signal ?? null,streamsClosed:Boolean(terminal),inputReleased,
      ownedPids:owner.pids(),remainingPids,log:logRef,executable:target,package:subject?.pck ?? null,
      fixture:subject?.fixture ?? null,profile:path.posix.join(relative,'profile')};
    if (runtime && subject && fs.existsSync(path.join(root,trace))) record.hostTrace=e.reference(root,trace,16*1024*1024);
    return {record,ref:e.writeExclusive(root,relative+'/terminal.json',record)};
  }
  return {checkpoint,close,get usable(){return Boolean(pid && !terminal && !error);}};
}

function packagedSubject(root, relative, build, fixture) {
  const source = path.posix.dirname(build.package.path);
  const positive = e.artifact(root,build.pck,64*1024*1024).data;
  if (!fixture) return {fixture:null,executable:build.package,pck:build.pck};
  const data = delivery.verifyRenderedFixture(positive,fixture,build.renderedFixtures[fixture]);
  const sibling = relative+'/'+fixture+'-subject';
  const rows = delivery.inventory(path.join(root,source)).filter(row => row.path !== 'KingdomZombies.pck');
  delivery.cloneOutput({directory:path.join(root,relative)},path.posix.basename(sibling),path.join(root,source),rows);
  const pck = sibling+'/KingdomZombies.pck';
  fs.writeFileSync(path.join(root,pck),data,{flag:'wx'});
  return {fixture,executable:e.reference(root,sibling+'/KingdomZombies.exe',256*1024*1024),
    pck:e.reference(root,pck,64*1024*1024)};
}

async function runInspection(root, relative, group, items, drivers, build, adapter, invocation, signal) {
  const outcomes = items.map(item => ({id:item.id,runGroup:item.runGroup,invocation,status:'pass',execution:'executed-now',
    expected:e.expectedOutcome(item),observed:'Current native observation and physical input recorded',checkpoints:[],processes:[]}));
  const packaged = group.id === 'G2-packaged';
  const negative = items.filter(item => Object.hasOwn(delivery.RENDERED_FIXTURES,item.id));
  const positive = items.filter(item => !negative.includes(item));
  const schedule = [...negative.map(item => ({fixture:item.id,lifetime:1,items:[item]}))];
  if (positive.length) schedule.push({fixture:null,lifetime:1,items:positive});
  const repeated = positive.filter(item => relaunchCase(item.id));
  if (repeated.length) schedule.push({fixture:null,lifetime:2,items:repeated});
  let launchNumber = 0;
  let session, currentAttempt = [];
  const budget = owned.timeBudget(group.timeoutMs,signal);
  const finish = async () => {
    if (!session) return;
    try {
      const terminal = await session.close();
      for (const row of currentAttempt) row.processes.push(terminal.ref);
      if(terminal.record.remainingPids.length) throw Object.assign(new Error('RENDERED_PROCESS_REMAINDER'),
        {unconfirmedCleanup:true});
      e.validateProcess(terminal.record);
    } catch (error) {
      for (const row of currentAttempt.filter(row => row.status === 'pass')) {
        row.status = 'invalid';row.observed = error.message;
      }
      if (error.unconfirmedCleanup) throw error;
    } finally {session = null;currentAttempt = [];}
  };
  for (const lane of schedule) {
    const owned = relative + '/' + group.id + '-launch-' + (++launchNumber);
    try {
      budget();
      const subject = packaged ? packagedSubject(root,relative,build,lane.fixture) : null;
      if (packaged) validateProcessPackageFiles(root,build,subject);
      else validateSourceProject(root,build,e.identifySource(root,build.revision));
      budget();
      session = await inspectionSession(root,owned,group.id,build,adapter,budget,signal,subject);
      let reset = true;
      for (const item of lane.items) {
        const row = outcomes.find(outcome => outcome.id === item.id);
        if (row.status !== 'pass') continue;
        currentAttempt.push(row);
        try {
          budget();
          e.need(session?.usable && reset,'RENDERED_GROUP_UNAVAILABLE',group.id);
          reset = false;
          const steps = drivers.get(item.id).steps.filter(step => (step.lifetime ?? 1) === lane.lifetime);
          for (const step of steps) row.checkpoints.push(await session.checkpoint(item,step,invocation));
          reset = true;
          if (item.id===contracts.AUDIO_CASE) row.listeningMedia=e.writeAuditionMedia(root,invocation);
        } catch (error) {
          row.status = row.checkpoints.length || session?.usable ? 'fail' : 'blocked';
          row.observed = error.message.slice(0,4096).replace(/[\r\n]/g,' ');
        }
      }
    } catch (error) {
      for (const row of outcomes.filter(row => lane.items.some(item => item.id === row.id) && row.status === 'pass')) {
        row.status = 'blocked';
        row.observed = error.message.slice(0,4096).replace(/[\r\n]/g,' ');
      }
    } finally {await finish();}
  }
  budget();
  return outcomes;
}

function validateProcessPackageFiles(root,build,subject) {
  const prefix=path.posix.dirname(subject.executable.path);
  const current=delivery.inventory(path.join(root,prefix));
  const expected=build.portable.files.map(row => row.path==='KingdomZombies.pck'
    ? {path:row.path,bytes:subject.pck.bytes,sha256:subject.pck.sha256} : row);
  e.need(equal(current,expected),'RENDERED_LIVE_PORTABLE_CLOSURE',subject.fixture ?? 'positive');
}

function validateProcessRuntime(root,build,owner) {
  const observer=require('./native-observer.cjs'), runtime=e.document(root,owner.runtime);
  const tree=observer.decode(e.artifact(root,runtime.tree,1048576).data);
  const object=observer.decode(e.artifact(root,runtime.object,1048576).data);
  const handshake=observer.decode(e.artifact(root,runtime.pidPacket,1048576).data);
  const id=treePaths(tree[2]).get(observer.HUB);
  e.need(owner.pid===runtime.pid && path.resolve(runtime.observedImage)===path.resolve(root,owner.executable.path) &&
    handshake[0]==='set_pid' && equal(handshake[2],[owner.pid]) && tree[1]===handshake[1] &&
    tree[0]==='scene:scene_tree' && object[0]==='scene:inspect_objects' && tree[1]===object[1] && id &&
    object[2].find(row=>row[0]===id)?.[2]?.some(row=>row[0]===observer.RUNTIME_PROPERTY&&row[5]===runtime.marker),'RENDERED_LIVE_RUNTIME_OWNER');
  observer.runtimeMarker(runtime.marker,owner.role==='inspection-hub'?SOURCE_LOAD_CONTEXT:undefined);
  if (owner.role==='inspection-hub') {
    e.need(equal(owner.executable,build.engine),'RENDERED_SOURCE_ENGINE');
    const [game,core]=runtime.marker.split('|').slice(1,3);
    e.need(build.sourceManaged.KingdomZombies.mvid===game && build.sourceManaged['KingdomZombies.Core'].mvid===core,
      'RENDERED_SOURCE_RUNTIME_MARKER');
  } else {
    e.need(owner.role==='packaged-inspection' && runtime.marker===build.portable.managedRuntime,'RENDERED_PORTABLE_LIVE_ASSEMBLY');
    e.artifact(root,owner.hostTrace,16*1024*1024);
    delivery.validatePortableEnvironment(e.document(root,owner.environment),path.resolve(root,owner.profile),path.resolve(root,owner.hostTrace.path));
    const directory=path.dirname(path.resolve(root,owner.hostTrace.path));
    const subject=path.dirname(path.resolve(root,owner.executable.path));
    e.need(path.basename(owner.hostTrace.path)==='runtime-host.log','RENDERED_HOST_TRACE_SCOPE');
    delivery.hostTrace({directory},'runtime',subject);
    validateProcessPackageFiles(root,build,{fixture:owner.fixture,executable:owner.executable,pck:owner.package});
  }
}

function validateInheritedRuntime(root,build,family,result,runtime) {
  const expected=family==='phase2' ? [{role:'solo',pid:result.pid}] :
    result.ownedProcesses?.map(({role,pid})=>({role,pid}));
  e.need(expected?.length>0 && Array.isArray(runtime),'RENDERED_INHERITED_RUNTIME');
  e.exact(expected.map(row=>row.role+'@'+row.pid),runtime.map(row=>row.role+'@'+row.pid),'inherited runtime owner roles');
  for (const row of runtime) validateLoadedSource(root,build,row);
}

async function runInherited(root, relative, group, items, build, adapter, identity, invocation, signal) {
  const original = items.map(item => ({...item,id:item.originalId,runGroup:item.inheritedGroup}));
  const directory = path.join(root,relative,group.id);
  fs.mkdirSync(directory,{recursive:true});
  const engine = path.join(root,build.engine.path);
  const {revision,source:input,tool} = identity;
  const children = owned.observeChildren(), launches = new Map();
  const groupSignal = AbortSignal.any([signal,AbortSignal.timeout(group.timeoutMs)]);
  const budget = owned.timeBudget(group.timeoutMs,groupSignal), currentSource=e.identifySource(root,revision);
  const cancel=()=>{void children.cleanup().catch(() => {});};
  groupSignal.addEventListener('abort',cancel,{once:true});
  const context = {root,directory,engine,env:adapter.env,revision,input,tool,nonce:invocation,platform:'win32-x64',
    ownsDesktopLock:true,project:path.resolve(root,build.sourceProject.path),runRoot:path.resolve(root,relative),budget,
    beforeGame(role,where,target) {
      budget();
      e.need(path.resolve(target)===path.resolve(engine),'RENDERED_INHERITED_ENGINE',role);
      e.artifact(root,build.engine);
      validateSourceProject(root,build,currentSource);
      const destination=path.relative(root,where).replaceAll(path.sep,'/')+'/phase4-runtime';
      return sourceLaunch(root,destination,build,role);
    },
    watchChild(child,launch) {
      children.track(child);
      if (launch && child.pid) launches.set(child.pid,launch);
    },
    osFingerprint:require('./content.cjs').hash(JSON.stringify({platform:process.platform,architecture:process.arch,release:os.release(),version:os.version()}))};
  const solo = group.id === 'G3-solo';
  if (!solo) e.writeExclusive(root,relative+'/observation.owner',Buffer.from(invocation));
  let executed;
  try {
    executed = solo ? {results:await adapter.solo(context,original)} : await adapter.lan(context,original);
  } finally {
    groupSignal.removeEventListener('abort',cancel);
    const remainingPids=await verifiedCleanup(children);
    e.writeExclusive(root,relative+'/'+group.id+'/phase4-owned-cleanup.json',{ownedPids:children.pids(),remainingPids});
    if(remainingPids.length) throw Object.assign(new Error('RENDERED_INHERITED_PROCESS_REMAINDER '+group.id),
      {unconfirmedCleanup:true});
  }
  budget();
  const bindings = {nonce:invocation,input,tool,revision,platform:context.platform,osFingerprint:context.osFingerprint};
  const outcomes = [];
  for (const [index,item] of items.entries()) {
    const result = executed.results.find(row => row.id === item.originalId);
    let status = result?.status ?? 'blocked', observed = result?.message ?? 'Inherited original case executed on current Phase 4 build';
    const runtime=[];
    if (status === 'pass') try {
      if (solo) require('../phase2/evidence.cjs').reconcile([original[index]],[result],context);
      else require('../phase3/evidence.cjs').reconcile([original[index]],[result],context);
      for (const pid of solo?[result.pid]:result.ownedProcesses.map(row=>row.pid)) {
        const subject=launches.get(pid);
        e.need(subject,'RENDERED_INHERITED_RUNTIME_OWNER',String(pid));
        runtime.push({pid,role:subject.role,launch:subject.launch,assemblies:e.reference(root,subject.stream,1024*1024)});
      }
      validateInheritedRuntime(root,build,item.family,result,runtime);
    } catch (error) {status = 'invalid';observed = error.message;}
    const inherited = e.writeExclusive(root,relative+'/'+group.id+'/'+item.originalId+'-result.json', {bindings,runtime,result:result??null});
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
  const attempts = e.campaignAttempts(root,data.campaign);
  const last = attempts.at(-1);
  const previous = last?.report ?? null;
  e.need(equal(data.previousRun ?? null,last?.terminal ?? null),'RENDERED_PREVIOUS_RUN');
  e.need(previous ? ['focused','final'].includes(data.kind) : data.kind==='initial','RENDERED_ATTEMPT_KIND');
  let native = allNative;
  if (data.kind==='focused') {
    e.need(Array.isArray(data.caseIds) && data.caseIds.length>0 && e.named(data.repairReason),'RENDERED_FOCUSED_SELECTION');
    native = allNative.filter(row => data.caseIds.includes(row.id));
    e.exact(native.map(row => row.id),data.caseIds,'focused selected cases');
    e.need(previous.outcomes.filter(row => row.status!=='pass').every(row => data.caseIds.includes(row.id)), 'RENDERED_FOCUSED_MISSING');
  }
  const drivers = reviewedDrivers(data,gate,current,native,data.kind);
  const review = e.document(root,gate.review);
  const build = validateBuild(root,e.document(root,review.buildReport),e.identifySource(root,current.revision),current.tool);
  validateDriverFixtures(root,drivers,build,gate,current);
  const driverRef = e.writeExclusive(root,relative+'/declared-drivers.json',e.artifact(root,runInput).data);
  const startedAt = new Date().toISOString(), outcomes = [];
  e.need(data.declaredAt <= startedAt,'RENDERED_DRIVER_DECLARATION_TIME');
  const opening = {...current,schemaVersion:1,invocation,campaign:data.campaign,startedAt,
    previousInvocation:previous?.invocation??null,kind:data.kind,repairReason:data.repairReason,selected:native.map(row => row.id),
    drivers:driverRef,reviewSha256:gate.review.sha256,selectionSha256:gate.selection.sha256};
  const start = e.writeExclusive(root,relative+'/start.json',opening);
  const controller = new AbortController();
  const interrupt = name => controller.abort(new Error('RENDERED_INTERRUPTED '+name));
  const signals = ['SIGINT','SIGTERM','SIGBREAK'].map(name => [name,() => interrupt(name)]);
  for (const [name,handle] of signals) process.on(name,handle);
  const blocked = (row, error) => ({id:row.id,runGroup:row.runGroup,invocation,status:'blocked',
    execution:'executed-now',expected:e.expectedOutcome(row),observed:error.message.slice(0,4096).replace(/[\r\n]/g,' ')});
  try {
    for (const group of gate.groups.filter(row => GROUPS.includes(row.id))) {
    const items = native.filter(row => row.runGroup === group.id);
    if (!items.length) continue;
    let unlock, lockRetained=false;
    try {
      controller.signal.throwIfAborted();
      unlock = require('../phase2/check.cjs').acquireLock(root,invocation);
      const result = group.id.startsWith('G3') ? await runInherited(root,relative,group,items,build,gate.adapter,current,invocation,controller.signal) :
        await runInspection(root,relative,group,items,drivers,build,gate.adapter,invocation,controller.signal);
      outcomes.push(...result);
    } catch (error) {
      lockRetained=Boolean(error.unconfirmedCleanup);
      if (lockRetained) e.writeExclusive(root,relative+'/'+group.id+'-cleanup-unconfirmed.json',{invocation,group:group.id,error:error.message});
      outcomes.push(...items.map(row => blocked(row,error)));
    } finally {
      try {if(!lockRetained)unlock?.();} catch (error) {
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
  const report = {...opening,start,finishedAt,outcomes};
  for (const row of outcomes.filter(row => row.status==='pass' && drivers.has(row.id))) try {
    e.validateRenderedOutcome(root,row,report,drivers.get(row.id),build);
  } catch (error) {
    row.status = 'invalid';row.observed = error.message.slice(0,4096).replace(/[\r\n]/g,' ');
  }
  if (controller.signal.aborted) for (const row of outcomes.filter(row => row.status === 'pass')) {
    row.status = 'invalid';row.observed = controller.signal.reason.message;
  }
  report.status = outcomes.every(row => row.status === 'pass')?'pass':'fail';
  e.reconcile(native,report,current);
  const ref = e.writeExclusive(root,relative+'/report.json',report);
  return {status:report.status,invocation,executed:outcomes.length,report:ref};
  } catch (error) {
    const interruption = error.message.slice(0,4096).replace(/[\r\n]/g,' ');
    const finishedAt = new Date(Math.max(Date.now(),Date.parse(startedAt)+1)).toISOString();
    const ref = e.writeExclusive(root,relative+'/interrupted.json',{...opening,start,finishedAt,interruption,status:'fail',
      outcomes:native.map(row => blocked(row,error))});
    return {status:'fail',invocation,executed:outcomes.length,report:ref};
  } finally {
    for (const [name,handle] of signals) process.removeListener(name,handle);
  }
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
  e.need(equal(build.delivery,index.export) && e.artifact(root,index.export).bytes>0,'COMPLETION_EXPORT');
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
  treePaths, savedNativeSnapshot, relaunchCase, packagedSubject, prepareSource, validateBuild, validateSourceProject,
  sourceImportCompanions, validateLoadedSource, validateProcessRuntime, validateProcessPackageFiles, runInspection, runInherited,
  runRendered, verifyCompletion};
