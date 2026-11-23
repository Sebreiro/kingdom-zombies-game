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
const content = require('./content.cjs');
const workspace = require('../workspace-paths.cjs');

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

function inheritedCatalog(root, family, revision = null) {
  e.need(['phase1','phase2','phase3'].includes(family),'INHERITED_FAMILY',family);
  const path = 'tools/' + family + '/required-cases.json';
  return (revision ? JSON.parse(e.git(root,['show',revision+':'+path])) : e.document(root,path)).cases;
}

function inheritedDeclaration(root, item, catalogs, revision = null) {
  const family = item.family;
  const [prefix,id] = item.id.split('/');
  e.need(prefix===family && id && item.id===family+'/'+id,'INHERITED_CASE_ID',item.id);
  if (!catalogs.has(family)) {
    const rows = inheritedCatalog(root,family,revision);
    e.exact(rows.map(row=>row.id),rows.map(row=>row.id),'inherited catalog '+family);
    catalogs.set(family,new Map(rows.map(row=>[row.id,row])));
  }
  const declared = catalogs.get(family).get(id);
  e.need(declared && declared.implementation!=='pending' && (declared.acceptancePhase??1)<=4,
    'INHERITED_CASE_DECLARATION',item.id);
  return declared;
}

function inheritedCases(root) {
  return [1,2,3].flatMap(phase => inheritedCatalog(root,'phase'+phase)
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
  const shared = starts('src/godot-game/Directory.Build.props','src/godot-game/Directory.Build.targets',
    'src/godot-game/Directory.Packages.props','src/godot-game/global.json','src/godot-game/NuGet.config');
  const runtime = shared || starts('src','src/godot-game/tools/code-rules','src/godot-game/tests/verification');
  const inheritedGroups = {
    engine:['src/godot-game/game','docs/code','src/godot-game/tools/code-rules','src/godot-game/tests/verification'],
    graph:['src','docs/architecture','docs/code','src/godot-game/tests/verification','src/godot-game/tools/verification/projects.json'],
    rules:['src','docs/code','src/godot-game/tools/code-rules','src/godot-game/tests/verification'],
    register:['docs/code','src/godot-game/tools/code-rules','src/godot-game/tests/verification'],
    state:['src/godot-game/core','src/godot-game/game','src/godot-game/tests/verification'],
    docs:['docs','specs','src','tools','tests','AGENTS.md'],
    runner:['src/godot-game/tools/verification','src/godot-game/tests/verification'],
    coverage:['src','docs/code','src/godot-game/tests/verification'],
    pins:['src/godot-game/tools/verification/toolchain.json']
  };
  // Each inherited case also executes through its own coordinator, manifest and compiled test project. Phase 2 and 3
  // coordinators load the Phase 1 runner, and Phase 3 loads Phase 2 evidence, locking and rendering modules.
  const runners = {phase1:['src/godot-game/tools/verification','src/godot-game/tests/verification'], phase2:['src/godot-game/tools/verification','src/godot-game/tools/gameplay-checks'],
    phase3:['src/godot-game/tools/verification','src/godot-game/tools/gameplay-checks','src/godot-game/tools/lan-checks']};
  return new Map(cases.map(row => [row.id, {new:!row.family, changed:false, affected:Boolean(row.family) &&
    (shared || starts(...runners[row.family]) || (row.family === 'phase1' ? starts(...inheritedGroups[row.group]??[]) : runtime))}]));
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
    solo:require('../gameplay-checks/rendered.cjs').rendered,
    lan:require('../lan-checks/scenarios.cjs').run,
    physical:require('../gameplay-checks/rendered.cjs').ownedInput,
    decodePng:require('../gameplay-checks/rendered.cjs').decodePng,
    readDraws:require('../lan-checks/rendered.cjs').draws
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
  const nonce=crypto.randomUUID(), ownership=content.runsPath(nonce)+'/', directory=path.resolve(root,ownership);
  const copy=ownership+'source/', relative=copy+'src/godot-game/game', project=path.resolve(root,relative);
  const {command,environment}=require('./check.cjs');
  const context={directory,nonce,revision,caseId:'rendered-source-preparation'};
  const pin=e.document(root,'src/godot-game/tools/verification/toolchain.json');
  const win=pin.archives['win32-x64'];
  const engine=e.reference(root,e.SHARED_TOOLS+win.directory+'/'+win.executable.replace('_console.exe','.exe'),256*1024*1024);
  e.need(engine.sha256===NATIVE_ENGINE_HASH,'RENDERED_EXECUTABLE');
  e.committedRef(root,e.git(root,['rev-parse','HEAD']).trim(),e.reference(root,e.TARGETS));
  for (const ref of source.files.filter(row=>/^(?:src\/godot-game\/(?:game\/|core\/|multiplayer-abstractions\/|tools\/code-rules\/|build\/|Directory\.|global\.json$|NuGet\.config$)|\.(?:editorconfig|gitattributes)$)/.test(row.path))) {
    const stored=e.writeExclusive(root,copy+ref.path,e.artifact(root,ref).data,ref.bytes);
    e.need(stored.bytes===ref.bytes&&stored.sha256===ref.sha256,'RENDERED_SOURCE_COPY',ref.path);
  }
  const godotFeed=pin.godotPackages.map(pkg=>{
    const file=content.feedPath(pkg.id+'.'+pkg.version+'.nupkg');
    const packageFile=e.read(root,file);
    e.need(packageFile.sha256===pkg.sha256,'RENDERED_SOURCE_GODOT_FEED',file);
    return e.writeExclusive(root,copy+file,packageFile.data,packageFile.bytes);
  });
  const env={...environment(directory),KZ_SHARED_LOCAL:workspace.locations().shared};
  const sdk=await command(context,'rendered-source-sdk',DOTNET,['--version'],{env});
  e.need(sdk.stdout.trim()===pin.sdk,'RENDERED_SOURCE_SDK');
  const importArgs=['--headless','--path',project,'--editor','--import'];
  await command(context,'rendered-source-import',e.located(root,engine.path),importArgs,{env});
  const isolated=['--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false',
    '-p:RestorePackagesPath='+content.sharedFile('packages'),
    '-p:RestoreSources='+path.join(root,copy,content.feedPath()),'-v:minimal'];
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
  const pin = e.document(root,'src/godot-game/tools/verification/toolchain.json').archives['win32-x64'];
  e.need(build.engine?.path === e.SHARED_TOOLS+pin.directory+'/'+pin.executable.replace('_console.exe','.exe') &&
    build.engine.sha256 === NATIVE_ENGINE_HASH,'RENDERED_EXECUTABLE');
  e.artifact(root,build.engine);
  const item = loadCatalog(root).cases.find(row => row.id === 'export.portability');
  const {result} = phase4SourceReceipt(root,item,{result:build.deliveryResult},source);
  const receipt = e.document(root,build.delivery);
  const directory = content.runsPath(result.nonce)+'/portable/';
  e.need(build.delivery.path === content.runsPath(result.nonce,'portable-delivery.json') &&
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

// A prepared source project lives in an asset-pipeline run folder: <runs>/<nonce>/source/src/godot-game/game.
function preparedOwner(relative) {
  const prefix = content.runsPath()+'/';
  return typeof relative === 'string' && relative.startsWith(prefix) ?
    /^([a-f0-9-]+)\/source\/src\/godot-game\/game$/.exec(relative.slice(prefix.length)) : null;
}

// Inherited families keep their historical names; their run folders come from the workspace map.
function familyRuns(family) {
  return content.legacyLocal('.'+family+'/runs')+'/'+(family==='phase1' ? '' : 'win32-x64/');
}

function validateSourceProject(root,build,source) {
  const prepared = build.sourceProject;
  const match = preparedOwner(prepared?.path);
  e.need(match && e.UUID.test(match[1]),'RENDERED_SOURCE_PROJECT');
  const ownership = content.runsPath(match[1])+'/', copy = ownership+'source/';
  const project = path.resolve(root,prepared.path);
  const tracked = source.files.filter(ref => ref.path.startsWith('src/godot-game/game/'));
  e.need(equal(prepared.importCompanions,sourceImportCompanions(root,prepared.path,source)),
    'RENDERED_SOURCE_IMPORT_COMPANIONS');
  for (const ref of tracked) {
    if (ref.path !== 'src/godot-game/game/project.godot') e.artifact(root,{...ref,path:copy+ref.path});
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
  const pin=e.document(root,'src/godot-game/tools/verification/toolchain.json');
  const expectedFeed=pin.godotPackages.map(pkg=>({path:copy+content.feedPath(pkg.id+'.'+pkg.version+'.nupkg'),sha256:pkg.sha256}));
  e.need(Array.isArray(prepared.godotFeed)&&equal(prepared.godotFeed.map(({path,sha256})=>({path,sha256})),expectedFeed),
    'RENDERED_SOURCE_GODOT_FEED');
  for (const ref of prepared.godotFeed) e.artifact(root,ref);
  e.need(sdk.stdout.trim()===pin.sdk && equal(sdk.args,['--version']) && [sdk,compiled].every(row =>
    path.resolve(row.executable)===path.resolve(DOTNET)),
    'RENDERED_SOURCE_SDK');
  e.need(equal(imported.args,['--headless','--path',project,'--editor','--import']) &&
    path.resolve(imported.executable)===path.resolve(e.located(root,build.engine.path)),'RENDERED_SOURCE_IMPORT_ENGINE');
  e.need(compiled.args?.includes(path.join(project,'KingdomZombies.csproj')) &&
    compiled.args.includes('-p:SourceRevisionId='+source.revision) && compiled.args.includes('-p:IncludeSourceRevisionInInformationalVersion=true') &&
    compiled.args.includes('-p:CustomAfterMicrosoftCommonTargets='+path.join(root,e.TARGETS)) &&
    compiled.args.includes('-p:RestoreSources='+path.join(root,copy,content.feedPath())),
    'RENDERED_SOURCE_COMPILED_REVISION');
  const imports=delivery.inventory(path.join(project,'.godot','imported'));
  e.need(imports.length>0 && equal(imports,prepared.imports),'RENDERED_SOURCE_IMPORT_CLOSURE');
  return validatePreparedManaged(root,build);
}

function validatePreparedManaged(root,build) {
  const prepared=build.sourceProject;
  const owner=preparedOwner(prepared?.path);
  e.need(owner && e.UUID.test(owner[1]),'RENDERED_SOURCE_PROJECT');
  const managedPath=prepared.path+'/.godot/mono/temp/bin/Debug';
  const managed=delivery.inventory(require('./content.cjs').inside(root,managedPath));
  e.need(equal(managed,prepared.managed),'RENDERED_SOURCE_MANAGED_CLOSURE');
  return Object.fromEntries(MANAGED_NAMES.map(name => {
    const file=managedPath+'/'+name+'.dll';
    const artifact=e.read(root,file,64*1024*1024);
    e.need(name==='GodotSharp' || artifact.data.includes(Buffer.from('+'+build.revision)),'RENDERED_SOURCE_STALE_DLL',name);
    return [name,{sha256:artifact.sha256,mvid:owned.managedMvid(artifact.data),file}];
  }));
}

function sourceImportCompanions(root,relative,source) {
  const authored=new Set(source.files.filter(ref=>ref.path.startsWith('src/godot-game/game/'))
    .map(ref=>ref.path.slice('src/godot-game/game/'.length)));
  const project=path.resolve(root,relative);
  e.need(fs.lstatSync(project).isDirectory()&&!fs.lstatSync(project).isSymbolicLink(),'RENDERED_SOURCE_REPARSE',relative);
  const generated=[];
  function visit(directory,prefix,cache=false) {
    for (const entry of fs.readdirSync(directory,{withFileTypes:true})) {
      const name=prefix+entry.name;
      e.need(!entry.isSymbolicLink(),'RENDERED_SOURCE_REPARSE',name);
      e.need(!/^(?:project\.binary|override\.cfg|extension_list\.cfg)$|\.(?:remap|gdextension)$/i.test(entry.name),
        'RENDERED_SOURCE_RUNTIME_REDIRECT',name);
      if (entry.isDirectory()) visit(path.join(directory,entry.name),name+'/',cache||name==='.godot');
      else if (!cache && !authored.has(name)) {
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
  visit(project,'');
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
      path.resolve(row.executable)===path.resolve(e.located(root,build.engine.path)) &&
      (row.file==='' || path.resolve(row.file)===path.resolve(root,declared.file)) &&
      row.loadContext===SOURCE_LOAD_CONTEXT &&
      row.mvid===declared.mvid &&
      (row.name==='GodotSharp'||row.version?.endsWith('+'+build.revision)),'RENDERED_SOURCE_RUNTIME_IDENTITY',String(row.name));
  }
  return observations;
}

function runId() {
  return content.runsPath('acceptance',crypto.randomUUID());
}

function nativeSelection(selection) {
  return selection.groups.filter(group=>GROUPS.includes(group.id))
    .flatMap(group=>group.cases.map(id=>({id,runGroup:group.id,family:id.startsWith('phase2/')?'phase2':id.startsWith('phase3/')?'phase3':null})));
}

function repairAffected(before,after,changed) {
  const prior=new Map(before.selected.map(row=>[row.id,row]));
  const entries=new Map(after.selected.map(row=>[row.id,row]));
  return nativeSelection(after).map(row=>row.id).filter(id=>{
    const was=prior.get(id), is=entries.get(id);
    e.need(is && Array.isArray(is.dependencies),'EVIDENCE_REPAIR_DEPENDENCIES',id);
    return !was || ['changed','affected'].some(flag=>is[flag]&&!was[flag]) ||
      changed.some(file=>is.dependencies.some(dependency=>file===dependency||file.startsWith(dependency+'/')));
  });
}

function newlyAffected(root,previous,revision,prior,selection) {
  if (!previous) return [];
  e.git(root,['merge-base','--is-ancestor',previous.revision,revision]);
  const changed=e.git(root,['diff','--name-only','-z',previous.revision,revision]).split('\0').filter(Boolean);
  return repairAffected(prior,selection,changed);
}

function attemptDeclaration(report,data,selection,previousRef,affected) {
  e.identity(report,data,'campaign attempt declaration');
  e.identity(report,selection,'campaign attempt selection');
  e.need(data.campaign===report.campaign && data.kind===report.kind &&
    data.reviewSha256===report.reviewSha256 && data.selectionSha256===report.selectionSha256 &&
    equal(data.previousRun??null,previousRef??null) && data.repairReason===report.repairReason &&
    e.instant(data.declaredAt) && data.declaredAt<=report.startedAt,'EVIDENCE_ATTEMPT_DECLARATION');
  const all=nativeSelection(selection);
  const rows=report.kind==='focused'?all.filter(row=>data.caseIds?.includes(row.id)):all;
  e.exact(rows.map(row=>row.id),report.selected,'attempt selection',true);
  e.need(report.outcomes.every((row,index)=>row.runGroup===rows[index]?.runGroup &&
    (row.status!=='pass'||row.execution==='executed-now')),'EVIDENCE_ATTEMPT_OUTCOMES');
  if (report.kind==='focused') {
    e.exact(report.selected,data.caseIds,'declared focused cases',true);
    e.exact(affected,report.newlyAffectedCaseIds,'derived newly affected cases',true);
    e.exact(affected,data.newlyAffectedCaseIds,'declared newly affected cases',true);
  } else e.need(affected.length===0 && (data.newlyAffectedCaseIds?.length??0)===0,
    'EVIDENCE_REPAIR_PROOF','newly affected cases must receive a focused run first');
  return rows;
}

function savedAttempt(root,report) {
  e.need(e.REVISION.test(report.gateRevision),'EVIDENCE_ATTEMPT_GATE');
  const directory=content.runsPath('acceptance',report.invocation)+'/';
  function committedCopy(canonical,digest) {
    const record=e.read(root,directory+path.posix.basename(canonical));
    const committed=e.git(root,['show',report.gateRevision+':'+canonical],null);
    e.need(record.sha256===digest && record.data.equals(committed),'EVIDENCE_ATTEMPT_COMMITTED_FILE',canonical);
    return e.document(root,record);
  }
  e.git(root,['merge-base','--is-ancestor',report.revision,report.gateRevision]);
  const review=committedCopy(e.REVIEW,report.reviewSha256);
  const selection=committedCopy(e.SELECTION,report.selectionSha256);
  e.identity(report,review,'campaign attempt review');
  e.need(review.buildReport?.sha256===report.build && e.instant(review.reviewedAt) && report.startedAt>=review.reviewedAt,
    'EVIDENCE_ATTEMPT_BUILD');
  const build=e.document(root,review.buildReport);
  e.identity(report,{...build,build:review.buildReport.sha256},'campaign attempt build');
  return {review,selection,build,data:e.document(root,report.drivers)};
}

function verifyInheritedRendered(root, report, outcome, item, build, catalogs) {
  const original = inheritedDeclaration(root,item,catalogs,report.revision);
  const owned = content.runsPath('acceptance',report.invocation,item.runGroup)+'/';
  e.need(outcome.inherited?.path===owned+original.id+'-result.json','EVIDENCE_INHERITED_RESULT_PATH',item.id);
  const {result,bindings,runtime} = e.document(root,outcome.inherited);
  e.need(bindings?.nonce===report.invocation && bindings.revision===report.revision &&
    bindings.input===report.source && bindings.tool===report.tool && bindings.platform==='win32-x64',
    'COMPLETION_INHERITED_BINDINGS',item.id);
  const context = {...bindings,root,directory:path.resolve(root,owned)};
  const verifier = item.family==='phase2'?require('../gameplay-checks/evidence.cjs'):require('../lan-checks/evidence.cjs');
  verifier.reconcile([original],[result],context);
  validateInheritedRuntime(root,build,item.family,result,runtime);
}

function validateAttemptHistory(root,attempts) {
  let prior=null;
  for (const {report,terminal} of attempts) {
    const {review,selection,build,data}=savedAttempt(root,report);
    const affected=newlyAffected(root,prior?.report,report.revision,prior?.selection,selection);
    const rows=attemptDeclaration(report,data,selection,prior?.terminal,affected);
    const gate={review:{sha256:report.reviewSha256},selection:{sha256:report.selectionSha256},reviewedAt:review.reviewedAt};
    const drivers=reviewedDrivers(data,gate,report,rows,report.kind);
    for (const driver of drivers.values()) {
      const fixture=driver.fixture;
      const expected=fixture.kind==='portable-negative'?build.renderedFixtures?.[driver.id]?.sha256:build.pck?.sha256;
      e.need(fixture.kind==='inspection-source'?fixture.sourceSha256===report.source:fixture.pckSha256===expected,
        'EVIDENCE_ATTEMPT_FIXTURE',driver.id);
    }
    if (report.kind==='focused' && report.outcomes.some(row=>row.status==='pass')) {
      const runtimeBuild={...build,sourceManaged:validatePreparedManaged(root,build),
        portable:delivery.validatePortableReceipt(root,build,e.document(root,build.delivery))};
      const catalogs = new Map();
      for (const [index,row] of report.outcomes.entries()) {
        if (row.status!=='pass') continue;
        if (!rows[index].family) e.validateRenderedOutcome(root,row,report,drivers.get(row.id),runtimeBuild);
        else verifyInheritedRendered(root,report,row,rows[index],runtimeBuild,catalogs);
      }
    }
    prior={report,terminal,selection};
  }
  return prior;
}

function campaignScope(gate) {
  return {head:gate.head,reviewSha256:gate.review.sha256,selectionSha256:gate.selection.sha256};
}

function finalRendered(root, gate, current, refs, campaign) {
  const native = gate.selected.filter(row => row.needsGameWindow);
  const reports = e.historyDocuments(root,refs,campaign,campaignScope(gate));
  const final = e.freshFinal(native,reports,current);
  validateAttemptHistory(root,reports.map((report,index)=>({report,terminal:refs[index]})));
  e.need(final.reviewSha256 === gate.review.sha256 && final.selectionSha256 === gate.selection.sha256 &&
    final.startedAt >= gate.reviewedAt,'COMPLETION_CURRENT_RUN');
  e.need(final.drivers?.path === content.runsPath('acceptance',final.invocation,'declared-drivers.json'),'COMPLETION_DRIVER_SCOPE');
  const driverRecord = e.document(root,final.drivers);
  const drivers = reviewedDrivers(driverRecord,gate,current,native,final.kind);
  const build = validateBuild(root,e.document(root,e.document(root,gate.review).buildReport),e.identifySource(root,current.revision),current.tool);
  validateDriverFixtures(root,drivers,build,gate,current);
  e.need(driverRecord.campaign === final.campaign && driverRecord.declaredAt <= final.startedAt,'COMPLETION_DRIVER_TIME');
  const catalogs = new Map();
  for (const [index,row] of final.outcomes.entries()) {
    const item = native[index];
    if (!item.family) e.validateRenderedOutcome(root,row,final,drivers.get(row.id),build);
    else verifyInheritedRendered(root,final,row,item,build,catalogs);
  }
  return final;
}

function latestPhase4Sources(root, admitted, currentRevision, sameInputs) {
  const found = new Map(admitted);
  if (!found.size) return found;
  const base = path.join(root,content.runsPath());
  for (const folder of fs.readdirSync(base,{withFileTypes:true})) {
    if (!folder.isDirectory() || !e.UUID.test(folder.name)) continue;
    const prefix = content.runsPath(folder.name)+'/';
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
  const directory = content.runsPath(result.nonce)+'/';
  e.need(e.UUID.test(result.nonce) && receipt.result.path===directory+'result.json' && result.caseId===item.id,
    'COMPLETION_SOURCE_SUBJECT',item.id);
  const invocation = e.document(root,directory+'invocation.json');
  e.need(invocation.nonce===result.nonce && invocation.caseId===item.id && invocation.revision===result.revision &&
    invocation.platform==='win32' && invocation.node==='v'+e.document(root,'src/godot-game/tools/verification/toolchain.json').node &&
    invocation.scope==='source/headless; zero game windows' &&
    e.instant(result.startedAt) && invocation.startedAt===result.startedAt && e.instant(result.finishedAt) && result.finishedAt>=result.startedAt,
    'COMPLETION_SOURCE_INVOCATION',item.id);
  const binding = item.sourceBinding;
  if (binding?.module.startsWith('src/godot-game/tools/asset-pipeline/cases/')) {
    const modules = invocation.caseModules?.filter(row => row.caseIds.includes(item.id));
    e.need(modules?.length===1 && modules[0].name===path.basename(binding.module) && modules[0].sha256===binding.moduleSha256,
      'COMPLETION_SOURCE_MODULE',item.id);
  }
  e.need(!Object.hasOwn(result,'unverifiedSource') && !Object.hasOwn(invocation,'unverifiedSource') &&
    equal(result.testedSource,invocation.testedSource) && (!result.testedSource ||
      equal(result.testedSource,{source:current.source,media:current.media})), 'COMPLETION_SOURCE_DRIFT',item.id);
  return {result,invocation};
}

// Inherited runners fingerprint their working tree but never refuse uncommitted changes. A receipt is therefore bound to
// the committed bytes of its recorded revision, recomputed here independently. Its recorded tool digest is not compared
// with the current machine: it hashes the shared test assembly, which every build stamps with the HEAD commit, so a
// rebuild after any later commit changes it without saying anything about the receipt's own run. The Phase 1 runner
// enforces the pinned Node, SDK and compiler itself, and the current Phase 4 tool identity pins Node and the SDK.
// Revisions before the repository-layout move keep the Phase 1 catalog at its original path.
function phase1Catalog(root, revision) {
  const current = 'src/godot-game/tools/verification/required-cases.json';
  return e.git(root,['ls-tree','--name-only',revision,'--',current]).trim() === current ? current : 'tools/phase1/required-cases.json';
}
const inheritedBinding = {
  inputs:new Map(),
  input(root, family, result, catalogs) {
    const key = result.revision + '\0' + family + '\0' + JSON.stringify(result.selected);
    if (!this.inputs.has(key)) {
      let inputs;
      if (family === 'phase1') inputs = JSON.parse(e.git(root,['show',result.revision+':'+phase1Catalog(root,result.revision)])).inputs;
      else if (family === 'phase2') inputs = require('../gameplay-checks/check.cjs').inputManifest(result.selected.map(id => {
        const row = catalogs.get('phase2')?.get(id);
        e.need(row,'COMPLETION_INHERITED_SUBJECT','phase2/'+id);
        return row;
      })).inputs;
      else inputs = require('../lan-checks/check.cjs').inputManifest().inputs;
      this.inputs.set(key,e.committedSnapshot(root,result.revision,inputs));
    }
    return this.inputs.get(key);
  }
};

function inheritedSourceReceipt(root, item, receipt, catalogs) {
  const result = e.document(root,receipt.result);
  const directory = path.posix.dirname(receipt.result.path);
  const familyPath = familyRuns(item.family);
  const subject = result.results?.filter(row => row.id===item.originalId);
  e.need(result.status==='pass' && e.UUID.test(result.nonce) && result.platform===inheritedWindowsPlatform(item.family) &&
    receipt.result.path===familyPath+result.nonce+'/summary.json' && result.selected?.includes(item.originalId) &&
    subject?.length===1 && subject[0].status==='pass' && e.REVISION.test(result.revision),'COMPLETION_INHERITED_SUBJECT',item.id);
  const expected = inheritedDeclaration(root,item,catalogs);
  e.need(e.HEX.test(result.input) && result.input===inheritedBinding.input(root,item.family,result,catalogs),
    'COMPLETION_INHERITED_INPUT',item.id);
  e.need(e.HEX.test(result.tool),'COMPLETION_INHERITED_SUBJECT',item.id);
  const context = {...result,root,directory:path.join(root,directory)};
  if (item.family==='phase1') {
    const baseline = require('../verification/check.cjs'), row = subject[0];
    const readProof = (parent,ref) => {
      const actual = e.reference(parent,ref.path);
      e.need(actual.sha256===ref.sha256,'COMPLETION_INHERITED_HASH',item.id);
      return actual;
    };
    if (expected.callerStages) baseline.verifyCallerResult(expected,row,context);
    else {
      const report = e.document(context.directory,readProof(context.directory,row.report));
      if (expected.repositoryStages || expected.producerStages) {
        e.need(report.caseId===item.originalId && report.status==='pass' && report.monolithicExecuted===false &&
          equal(report.stages,row.stages),'COMPLETION_INHERITED_STAGES',item.id);
      } else {
        e.need(Array.isArray(row.evidence) && row.evidence.length>0,'COMPLETION_INHERITED_EVIDENCE',item.id);
        const caseDirectory = require('./content.cjs').inside(context.directory,expected.id);
        for (const ref of row.evidence) readProof(caseDirectory,ref);
        baseline.verifyReport(report,[expected]);
      }
    }
  } else if (item.family==='phase2') require('../gameplay-checks/evidence.cjs').reconcile([expected],subject,context);
  else require('../lan-checks/evidence.cjs').reconcile([expected],subject,context);
  return result;
}

function inheritedWindowsPlatform(family) {
  return family==='phase1' ? 'win32-x64-'+os.release() : 'win32-x64';
}

function inheritedSourceTime(root, ref, result) {
  if (Number.isSafeInteger(result.startedAt)) return result.startedAt;
  const firstCommand = Date.parse(result.commands?.[0]?.started);
  return Number.isFinite(firstCommand) ? firstCommand : fs.statSync(path.join(root,ref.path)).mtimeMs;
}

function verifyInheritedHistory(root, admitted, current, sameInputs) {
  const families = new Set([...admitted.values()].map(row => row.item.family));
  for (const family of families) {
    const directory = familyRuns(family);
    for (const folder of fs.readdirSync(path.join(root,directory),{withFileTypes:true})) {
      if (!folder.isDirectory() || !e.UUID.test(folder.name)) continue;
      const relative = directory+folder.name+'/summary.json';
      if (!fs.existsSync(path.join(root,relative))) continue;
      const result = e.document(root,relative);
      if (result.platform!==inheritedWindowsPlatform(family) || !Array.isArray(result.selected)) continue;
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
  const catalogs = new Map();
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
      result = inheritedSourceReceipt(root,item,receipt,catalogs);
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

function treePaths(raw, types = null) {
  e.need(Array.isArray(raw) && raw.length % 6 === 0 && raw.length > 0 && raw.length <= 8190, 'RENDERED_NATIVE_TREE');
  const result = new Map(), parents = [], ids=new Set();
  for (let offset = 0; offset < raw.length; offset += 6) {
    while (parents.at(-1)?.remaining === 0) parents.pop();
    const [children, name, type, id] = raw.slice(offset, offset + 4), parent = parents.at(-1);
    e.need(Number.isInteger(children) && children >= 0 && e.named(name) && Number.isSafeInteger(id) && id > 0 &&
      (offset === 0 ? name === 'root' : parent), 'RENDERED_NATIVE_TREE');
    const node = (parent?.node ?? '') + '/' + name;
    e.need(!result.has(node) && !ids.has(id), 'RENDERED_NATIVE_TREE_DUPLICATE', node);
    result.set(node, id);
    types?.set(node,type);
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
    const types=new Map(),nodes=treePaths(tree.payload,types),objects=new Map();
    const targets=nativeTargets(nodes,types,checks);
    for(const node of observationNodes(targets))
      objects.set(node,await request('scene:inspect_objects','scene:inspect_objects',[[nodes.get(node)],false],deadline));
    const values=nativeValues(nodes,objects,targets);
    return {tree,objects,values,at:new Date().toISOString()};
  }
  return {endpoint:'tcp://127.0.0.1:' + server.address().port, snapshot, get pidPacket(){return pidPacket;}, close:() => {
    channel?.destroy();return new Promise(resolve => server.close(resolve));
  }};
}

function nativeTargets(nodes,types,checks) {
  return checks.map(check=>{
    if (!check.childClass) return {...check,target:nodes.has(check.node)?check.node:null};
    const children=[...types].filter(([node,type])=>type===check.childClass&&path.posix.dirname(node)===check.node);
    if (check.match==='child-count') return {...check,target:nodes.has(check.node)?check.node:null,childCount:children.length};
    return {...check,target:children[check.childIndex]?.[0]??null};
  });
}

function observationNodes(targets) {
  return [...new Set(targets.filter(row=>row.target&&row.property!==null).map(row=>row.target))];
}

function nativeValues(nodes,objects,targets) {
  return targets.map(check => {
    const id = nodes.get(check.target) ?? null;
    const object=objects.get(check.target);
    const properties=object?.payload.find(row=>row[0]===id)?.[2];
    const values=properties?.filter(row=>row[0]===check.property);
    const value=check.match==='child-count' ? (id===null?null:check.childCount) :
      check.property===null ? id!==null : values?.length===1 ? values[0][5] : null;
    return {id,value};
  });
}

function savedNativeSnapshot(root,ref,checks,thread) {
  const snapshot=e.document(root,ref), decode=require('./native-observer.cjs').decode;
  e.need(e.instant(snapshot.at) && Array.isArray(snapshot.objects),'EVIDENCE_NATIVE_SNAPSHOT');
  const tree=decode(e.artifact(root,snapshot.tree,1048576).data), types=new Map(),nodes=treePaths(tree[2],types);
  e.need(tree[0]==='scene:scene_tree' && tree[1]===thread,'EVIDENCE_NATIVE_SNAPSHOT_THREAD');
  const targets=nativeTargets(nodes,types,checks);
  const required=observationNodes(targets);
  e.exact(required,snapshot.objects.map(row=>row.node),'native snapshot subjects');
  const objects=new Map(snapshot.objects.map(row=>{
    const object=decode(e.artifact(root,row.packet,1048576).data);
    e.need(object[0]==='scene:inspect_objects' && object[1]===thread && object[2]?.length===1 &&
      object[2][0][0]===nodes.get(row.node),'EVIDENCE_NATIVE_SNAPSHOT_OBJECT',row.node);
    return [row.node,{payload:object[2]}];
  }));
  return {at:snapshot.at,values:nativeValues(nodes,objects,targets)};
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
  return owned.command(owner,owned.windowsPowerShell(), ['-NoProfile','-NonInteractive','-EncodedCommand',
    Buffer.from(script,'utf16le').toString('base64')], {cwd:root,env}, limit,signal);
}

async function verifiedCleanup(owner) {
  try {return await owner.cleanup();}
  catch(error) {throw Object.assign(error,{unconfirmedCleanup:true});}
}

async function inspectionSession(root, relative, group, build, adapter, budget, signal, subject = null) {
  const phase2 = require('../gameplay-checks/rendered.cjs');
  const owner = owned.observeChildren();
  const role = group === 'G1-hub' ? 'inspection-hub' : 'packaged-inspection';
  const target = role === 'inspection-hub' ? build.engine : subject.executable;
  e.artifact(root,target);
  const executable = e.located(root,target.path);
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
  const powerShell=owned.windowsPowerShell();
  budget();
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
    const result = await owned.command(owner,powerShell,['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',
      path.join(root,'src/godot-game/tools/gameplay-checks/windows-input.ps1'),'-OwnedPid',String(pid),'-Action',action,'-X',String(x),'-Y',String(y),'-Key',String(key)],
      {cwd:root,env},closing?3000:budget(12000),closing?undefined:signal);
    acknowledgement = JSON.parse(result.stdout.trim());
    e.need(acknowledgement.pid === pid && acknowledgement.focused && acknowledgement.action === action &&
      acknowledgement.width === 1280 && acknowledgement.height === 720, 'RENDERED_INPUT_ACK');
  });
  async function settle(checks,{match=true,previous=null,onFailure=null}={}) {
    const end=Date.now()+budget(12000);
    for (;;) {
      const observed=await native.snapshot(pid,checks,end);
      const valid=checks.every((check,index)=>contracts.matches(check,observed.values[index].value));
      if ((!match || valid) && previous && observed.at>previous.at && equal(previous.values,observed.values)) return observed;
      previous=(!match || valid)?observed:null;
      if (terminal || error || Date.now()>=end) onFailure?.(observed);
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
  // Phase 5 resolves controls from the owned live scene instead of a Phase 4 driver declaration.
  async function clickControl(node, {firstRow=false}={}) {
    const position = {node,property:'global_position'};
    const size = {node,property:'size'};
    const ancestors = [];
    for (let current=node; current!=='/root'; current=path.posix.dirname(current)) {
      ancestors.push({node:current,property:'position'});
    }
    const layout = await native.snapshot(pid,[position,size,...ancestors]);
    const [global,extent,...locals] = layout.values.map(row => row.value);
    const vector = value => value?.type===5 && value.values.length===2;
    e.need(vector(extent) && (vector(global) || locals.every(vector)), 'HUB_SMOKE_CONTROL_GEOMETRY',node);
    const origin = vector(global) ? global.values : locals.reduce(
      ([x,y],{values}) => [x+values[0],y+values[1]], [0,0]);
    const [width,height] = extent.values;
    const x = Math.floor(origin[0] + (firstRow ? Math.min(width/2,24) : width/2));
    const y = Math.floor(origin[1] + (firstRow ? Math.min(height/2,12) : height/2));
    e.need(width>0 && height>0 && x>=0 && x<1280 && y>=0 && y<720, 'HUB_SMOKE_CONTROL_OUTSIDE_CLIENT',node);
    return {x,y};
  }
  async function checkpoint(item,step,invocation,recovery=false) {
    const prefix = relative + '/' + item.id + (recovery?'-recovery-':'-') + step.id;
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
    const beforeRef=snapshotRef(prefix+'-before',before);
    const token=crypto.randomUUID(), actionStartedAt=new Date().toISOString();
    let recheck,openMenu,focusClick;
    if(step.recheck) {
      await input.input('click',step.recheck.x,step.recheck.y,0);
      recheck={...step.recheck,at:new Date().toISOString(),acknowledgement};
    }
    if (step.openMenu && !step.drive) {
      await input.input('click',step.openMenu.x,step.openMenu.y,0);
      openMenu={...step.openMenu,at:new Date().toISOString(),acknowledgement};
    }
    if (step.focusClick) {
      await input.input('click',step.focusClick.x,step.focusClick.y,0);
      focusClick={...step.focusClick,at:new Date().toISOString(),acknowledgement};
    }
    const actions=[];
    if (step.drive) {
      const act = async (action,x,y,key) => {
        await input.input(action,x,y,key);
        actions.push({action,x,y,key,acknowledgement});
      };
      await step.drive({
        async click(node, options) {
          const {x,y}=await clickControl(node,options);
          await act('click',x,y,0);
        },
        tap(key) { return act('tap',0,0,key); }
      });
      e.need(actions.length>0,'HUB_SMOKE_INPUT_EMPTY',step.id);
    } else {
      await input.input(step.action,step.x ?? 0,step.y ?? 0,step.key ?? 0);
    }
    const at = new Date().toISOString(), physical = acknowledgement;
    const flags = {caseId:item.id,checkpoint:step.id,invocation,pid,token};
    const inputRef = e.writeExclusive(root,prefix+'-input.json',{...flags,at,actionStartedAt,focused:true,action:step.action,
      clientWidth:physical.width,clientHeight:physical.height,method:step.drive?'physical-sequence':
        step.action==='click'?'physical-mouse':'physical-keyboard',
      control:step.control,x:step.x,y:step.y,key:step.key,acknowledgement:physical,recheck,openMenu,focusClick,
      ...(step.drive ? {actions} : {})});
    const observed = await settle(step.checks,{onFailure:state=>snapshotRef(prefix+'-failed-settle',state)});
    const observedRef=snapshotRef(prefix+'-observed',observed);
    e.need(!equal(observed.values,before.values),'RENDERED_NATIVE_TRANSITION',item.id+' '+step.id);
    const imagePath = prefix + '.png';
    const capture=await desktopCommand(root,CAPTURE_CLIENT,{...env,KZ_PHASE4_PID:String(pid),KZ_PHASE4_WINDOW:String(physical.window),
      KZ_PHASE4_DRAW_TOKEN:token,KZ_PHASE4_PNG:path.join(root,imagePath)},budget(12000),owner,signal);
    const visible=JSON.parse(capture.stdout.trim());
    const after=await native.snapshot(pid,step.checks);
    const afterRef=snapshotRef(prefix+'-after',after);
    e.need(equal(observed.values,after.values),'RENDERED_NATIVE_DRAW_DRIFT',step.id);
    budget();
    adapter.decodePng(path.join(root,imagePath),1280,720);
    const image = e.reference(root,imagePath,16 * 1024 * 1024);
    const draw = e.writeExclusive(root,prefix+'-draw.json',{...flags,at:new Date().toISOString(),drawn:true,frame:++frame,
      before:beforeRef,observed:observedRef,after:afterRef,capture:visible,ready,frameSource:'composited-client-between-native-observations',
      imageSha256:image.sha256});
    const point = {id:step.id,input:inputRef,draw,image};
    if (item.id===contracts.AUDIO_CASE && contracts.AUDIO_PLAYBACK.has(step.id))
      point.playback=e.audioPlayback(e.audioAuthority(root),step.id,before.values[0],observed.values[0]);
    return point;
  }
  async function restore(item,steps,invocation,lifetime) {
    const checkpoints=[];
    for (const step of steps) {
      const state=await native.snapshot(pid,step.checks);
      if (step.checks.every((check,index)=>contracts.matches(check,state.values[index].value))) continue;
      checkpoints.push(await checkpoint(item,step,invocation,true));
    }
    const baseline=await settle(contracts.baseline);
    const prefix=relative+'/'+item.id+'-recovery-'+lifetime;
    return {checkpoints,baseline:snapshotRef(prefix+'-baseline',baseline)};
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
  return {checkpoint,restore,close,get usable(){return Boolean(pid && !terminal && !error);}};
}

function resetSequence(steps,lifetime) {
  const reset=steps.at(-1);
  e.need(reset?.id==='reset-'+lifetime,'RENDERED_RECOVERY_DECLARATION');
  let start=steps.length-1;
  while (steps[start-1]?.navigate) start--;
  return steps.slice(start);
}

async function runInspectionLane(session,lane,outcomes,drivers,invocation,budget,onPass) {
  const attempted=[];
  let recoveryFailure=null;
  for (const item of lane.items) {
    const row=outcomes.find(outcome=>outcome.id===item.id);
    if (row.status!=='pass') continue;
    const steps=drivers.get(item.id).steps.filter(step=>step.lifetime===lane.lifetime);
    try {
      budget();
      e.need(session.usable&&!recoveryFailure,'RENDERED_GROUP_UNAVAILABLE',recoveryFailure??item.id);
    } catch (error) {
      row.status='blocked';row.execution='not-executed';row.observed=error.message;
      continue;
    }
    attempted.push(row);
    let resetVerified=false;
    let activeCheckpoint=null;
    try {
      for (const step of steps) {
        activeCheckpoint=step.id;
        row.checkpoints.push(await session.checkpoint(item,step,invocation));
      }
      resetVerified=true;
      activeCheckpoint=null;
      onPass(item,row);
    } catch (error) {
      row.status='fail';
      row.observed=error.message.slice(0,4096).replace(/[\r\n]/g,' ');
      row.failedCheckpoint=activeCheckpoint;
      if (!resetVerified && session.usable) {
        try {row.recovery=await session.restore(item,resetSequence(steps,lane.lifetime),invocation,lane.lifetime);}
        catch (failure) {
          recoveryFailure=failure.message.slice(0,4096).replace(/[\r\n]/g,' ');
          row.resetFailure=recoveryFailure;
        }
      }
    }
  }
  return attempted;
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
      currentAttempt=await runInspectionLane(session,lane,outcomes,drivers,invocation,budget,(item,row)=>{
        if (item.id===contracts.AUDIO_CASE) row.listeningMedia=e.writeAuditionMedia(root,invocation);
      });
    } catch (error) {
      for (const row of outcomes.filter(row => lane.items.some(item => item.id === row.id) && row.status === 'pass')) {
        row.status = 'blocked';
        row.observed = error.message.slice(0,4096).replace(/[\r\n]/g,' ');
      }
    } finally {await finish();}
  }
  try {budget();}
  catch (error) {
    for (const row of outcomes.filter(row=>row.status==='pass')) {
      row.status='invalid';row.observed=error.message;
    }
  }
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
  const catalogs = new Map();
  const original = items.map(item => inheritedDeclaration(root,item,catalogs,identity.revision));
  const directory = path.join(root,relative,group.id);
  fs.mkdirSync(directory,{recursive:true});
  const engine = e.located(root,build.engine.path);
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
      if (solo) require('../gameplay-checks/evidence.cjs').reconcile([original[index]],[result],context);
      else require('../lan-checks/evidence.cjs').reconcile([original[index]],[result],context);
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
  const attempts = e.campaignAttempts(root,data.campaign,campaignScope(gate));
  const {previous,pending}=e.repairHistory(attempts.map(row=>row.report));
  const last=validateAttemptHistory(root,attempts);
  const selection=e.document(root,gate.selection);
  const affected=newlyAffected(root,previous,current.revision,last?.selection,selection);
  e.need(equal(data.previousRun ?? null,last?.terminal ?? null),'RENDERED_PREVIOUS_RUN');
  e.need(previous ? ['focused','final'].includes(data.kind) : data.kind==='initial','RENDERED_ATTEMPT_KIND');
  let native = allNative;
  if (data.kind==='focused') {
    e.need(Array.isArray(data.caseIds) && data.caseIds.length>0 && e.named(data.repairReason),'RENDERED_FOCUSED_SELECTION');
    native = allNative.filter(row => data.caseIds.includes(row.id));
    e.exact(native.map(row => row.id),data.caseIds,'focused selected cases');
    e.exact(affected,data.newlyAffectedCaseIds,'declared newly affected cases',true);
    e.exact([...new Set([...pending,...affected])],data.caseIds,'focused repair denominator');
  } else e.need(!pending.size && affected.length===0 && (data.newlyAffectedCaseIds?.length??0)===0,
    'EVIDENCE_REPAIR_PROOF','focused repair still pending');
  const drivers = reviewedDrivers(data,gate,current,native,data.kind);
  const review = e.document(root,gate.review);
  const build = validateBuild(root,e.document(root,review.buildReport),e.identifySource(root,current.revision),current.tool);
  validateDriverFixtures(root,drivers,build,gate,current);
  // Reject a future declaration before this attempt writes anything that a later attempt must reconcile.
  e.need(e.instant(data.declaredAt) && data.declaredAt <= new Date().toISOString(),'RENDERED_DRIVER_DECLARATION_TIME');
  const driverRef = e.writeExclusive(root,relative+'/declared-drivers.json',e.artifact(root,runInput).data);
  const startedAt = new Date().toISOString(), outcomes = [];
  e.need(data.declaredAt <= startedAt,'RENDERED_DRIVER_DECLARATION_TIME');
  const opening = {...current,schemaVersion:1,invocation,campaign:data.campaign,startedAt,
    previousInvocation:previous?.invocation??null,kind:data.kind,repairReason:data.repairReason,selected:native.map(row => row.id),
    drivers:driverRef,reviewSha256:gate.review.sha256,selectionSha256:gate.selection.sha256,gateRevision:gate.head,
    newlyAffectedCaseIds:affected,runner:{pid:process.pid,startedAt:Math.floor(Date.now()-process.uptime()*1000)}};
  const start = e.writeExclusive(root,relative+'/start.json',opening);
  owned.journal(path.join(root,relative,'owned-processes.ndjson'));
  const controller = new AbortController();
  const interrupt = name => controller.abort(new Error('RENDERED_INTERRUPTED '+name));
  // Closing the console delivers SIGHUP on Windows. Harder terminations leave start.json for recordInterruption.
  const signals = ['SIGINT','SIGTERM','SIGBREAK','SIGHUP'].map(name => [name,() => interrupt(name)]);
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
      unlock = require('../gameplay-checks/check.cjs').acquireLock(root,invocation);
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
    owned.journal(null);
    for (const [name,handle] of signals) process.removeListener(name,handle);
  }
}

// The interrupted attempt's outcome rows come from its own committed selection and the catalogs at its gate revision,
// not from the current catalog, which can be mid-repair.
function attemptRows(root, opening) {
  const {selection} = savedAttempt(root,opening);
  const phase4 = new Map(JSON.parse(e.git(root,['show',opening.gateRevision+':'+CATALOG])).renderedCases.map(row => [row.id,row]));
  const families = new Map();
  return new Map(nativeSelection(selection).map(row => {
    let declared;
    if (row.family) {
      if (!families.has(row.family))
        families.set(row.family,new Map(inheritedCatalog(root,row.family,opening.gateRevision).map(item => [item.id,item])));
      const original = row.id.slice(row.family.length + 1), item = families.get(row.family).get(original);
      declared = item && {...item,originalId:original};
    } else declared = phase4.get(row.id);
    e.need(declared,'RENDERED_INTERRUPTION_CASE',row.id);
    return [row.id,{...declared,id:row.id,runGroup:row.runGroup}];
  }));
}

// A runner that was terminated without its own terminal receipt (crash, forced kill, power loss) leaves start.json
// alone. Record the attempt as a failed interruption only after its runner and every journaled owned process are gone.
// Every check and every outcome row is resolved before the lock is released or anything is written, and the single
// interrupted.json write carries the cleanup record, so a refused or failed attempt can simply be retried.
async function recordInterruption(root, invocation, {observer:observerOptions = {}, rows:fixtureRows = null} = {}) {
  e.need(e.UUID.test(invocation),'RENDERED_INTERRUPTION_USAGE');
  const relative = content.runsPath('acceptance',invocation);
  const start = e.reference(root,relative+'/start.json'), opening = e.document(root,start);
  e.need(opening.schemaVersion===1 && opening.invocation===invocation && e.UUID.test(opening.campaign) &&
    e.instant(opening.startedAt) && Array.isArray(opening.selected) && Number.isSafeInteger(opening.runner?.pid) &&
    Number.isSafeInteger(opening.runner?.startedAt),'RENDERED_INTERRUPTION_START',invocation);
  for (const name of ['report.json','interrupted.json'])
    e.need(!fs.existsSync(path.join(root,relative,name)),'RENDERED_INTERRUPTION_TERMINAL',name);
  const rows = fixtureRows ?? attemptRows(root,opening);
  const interruption = 'The runner ended without a terminal receipt; recorded afterwards once no owned process survived.';
  const outcomes = opening.selected.map(id => {
    const row = rows.get(id);
    e.need(row,'RENDERED_INTERRUPTION_CASE',id);
    return {id,runGroup:row.runGroup,invocation,status:'blocked',execution:'executed-now',expected:e.expectedOutcome(row),
      observed:interruption};
  });
  const placeholder = (pid, exited) => ({pid,exitCode:exited ? 0 : null,signalCode:null});
  // The runner itself must be gone. Its own untracked helpers are not inferred from a parent PID that may be recycled.
  const runner = owned.observeChildren(observerOptions);
  runner.track(placeholder(opening.runner.pid,false),opening.runner.startedAt);
  const runnerAlive = await runner.remaining({descendants:false});
  e.need(runnerAlive.length===0,'RENDERED_INTERRUPTION_RUNNER_ALIVE',String(opening.runner.pid));
  const journalPath = path.join(root,relative,'owned-processes.ndjson');
  const launches = new Map();
  if (fs.existsSync(journalPath)) for (const line of fs.readFileSync(journalPath,'utf8').split(/\r?\n/).filter(Boolean)) {
    const row = JSON.parse(line);
    e.need(Number.isSafeInteger(row.pid) && Number.isSafeInteger(row.trackedAt) &&
      (row.exitedAt === undefined || Number.isSafeInteger(row.exitedAt)),'RENDERED_INTERRUPTION_JOURNAL');
    const key = row.pid+' '+row.trackedAt;
    launches.set(key,{...launches.get(key),...row});
  }
  const observer = owned.observeChildren(observerOptions);
  for (const row of launches.values())
    observer.track(placeholder(row.pid,row.exitedAt !== undefined),row.trackedAt,row.exitedAt ?? null);
  const remainingPids = await observer.remaining();
  e.need(remainingPids.length===0,'RENDERED_INTERRUPTION_SURVIVORS',remainingPids.join(','));
  const lock = workspace.locations(root).locks.desktop;
  const ownsLock = fs.existsSync(lock) && JSON.parse(fs.readFileSync(lock,'utf8')).nonce===invocation;
  const finishedAt = new Date(Math.max(Date.now(),Date.parse(opening.startedAt)+1)).toISOString();
  const cleanup = {runner:opening.runner,launches:launches.size,remainingPids,releasedLock:ownsLock};
  if (ownsLock) fs.unlinkSync(lock);
  const ref = e.writeExclusive(root,relative+'/interrupted.json',{...opening,start,finishedAt,interruption,status:'fail',
    cleanup,outcomes});
  return {status:'fail',invocation,report:ref,releasedLock:ownsLock};
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

module.exports = {CATALOG, COVERAGE, SCENARIOS, GROUPS, ORDER, familyRuns, coverageRows, validateProcedure, validateCatalog, loadCatalog,
  selectCases, inheritedCases, selectionRows, impactClosure, validateSelection, currentReviewAdapter, prelaunch, currentIdentity, runId,
  finalRendered, latestPhase4Sources, sourceReceipts, inheritedBinding, completionSourceReceipts, recordInterruption, preWindowCases, verifyFixProof, coverageExpectation, validateDriver, reviewedDrivers,
  treePaths, savedNativeSnapshot, relaunchCase, packagedSubject, prepareSource, validateBuild, validateSourceProject,
  repairAffected, attemptDeclaration, validateAttemptHistory, verifyInheritedRendered,
  runInspectionLane, inspectionSession,
  sourceImportCompanions, validateLoadedSource, validateProcessRuntime, validateProcessPackageFiles, runInspection, runInherited,
  runRendered, verifyCompletion};
