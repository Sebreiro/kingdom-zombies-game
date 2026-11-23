'use strict';

// Acceptance comes from the independent manifest, never the discovered test count.
const fs = require('node:fs');
const path = require('node:path');
const os = require('node:os');
const crypto = require('node:crypto');
const { spawn, spawnSync } = require('node:child_process');
const { parseJson } = require('./bootstrap.cjs');
const {launch}=require('../dotnet.cjs');
const workspace = require('../workspace-paths.cjs');
const groups = ['pins', 'runner', 'register', 'rules', 'graph', 'state', 'engine', 'docs', 'coverage', 'platform'];
const digest = data => crypto.createHash('sha256').update(data).digest('hex');
const fileHash = file => digest(fs.readFileSync(file));
function reject(diagnostic, message) { throw Object.assign(new Error(message), { diagnostic }); }
function read(file) {
  let text;
  try { text = new TextDecoder('utf-8', { fatal: true }).decode(fs.readFileSync(file)); }
  catch (error) { reject('JSON_INVALID', `${file}: ${error.message}`); }
  return parseJson(text);
}
function inside(root, relative) {
  if (typeof relative !== 'string' || !relative || path.isAbsolute(relative)) reject('PATH_ESCAPE', String(relative));
  const target = path.resolve(root, relative);
  if (!target.startsWith(root + path.sep)) reject('PATH_ESCAPE', relative);
  let ancestor = target;
  while (!fs.existsSync(ancestor)) ancestor = path.dirname(ancestor);
  const realRoot = fs.realpathSync(root), real = fs.realpathSync(ancestor);
  if (real !== realRoot && !real.startsWith(realRoot + path.sep)) reject('PATH_ESCAPE', relative);
  return target;
}
function snapshot(root, manifest) {
  const rows = [];
  function visit(relative) {
    const target = inside(root, relative);
    if (!fs.existsSync(target)) reject('INPUT_MISSING', relative);
    const stat = fs.lstatSync(target);
    if (stat.isSymbolicLink()) reject('INPUT_SYMLINK', relative);
    if (stat.isDirectory()) {
      for (const name of fs.readdirSync(target).sort()) {
        if (['bin', 'obj', '.godot'].includes(name)) continue;
        visit(relative + '/' + name);
      }
    } else rows.push({ path: relative.replaceAll('\\', '/'), sha256: fileHash(target) });
  }
  for (const input of manifest.inputs) visit(input);
  rows.sort((a, b) => a.path.localeCompare(b.path, 'en'));
  if (!rows.length || new Set(rows.map(row => row.path)).size !== rows.length) reject('INPUT_DUPLICATE', 'Empty or overlapping input scope');
  return { sha256: digest(JSON.stringify(rows)), files: rows };
}
function select(manifest, args) {
  if (!Array.isArray(manifest.cases) || !manifest.cases.length) reject('MANIFEST_EMPTY', 'No declared obligations');
  const ids = new Set();
  for (const row of manifest.cases) {
    if (!row || typeof row !== 'object' || Array.isArray(row)) reject('MANIFEST_INVALID', 'Case must be an object');
    if (typeof row.id !== 'string' || !/^[A-Za-z0-9][A-Za-z0-9._-]+$/.test(row.id) || ids.has(row.id)) reject('MANIFEST_DUPLICATE', String(row.id));
    ids.add(row.id);
    const strings = values => Array.isArray(values) && values.length > 0 && values.every(value => typeof value === 'string' && value.trim());
    if (!groups.includes(row.group) || typeof row.owner !== 'string' || !row.owner.trim()
        || !strings(row.prerequisites) || !strings(row.evidence) || typeof row.rule !== 'string' || !row.rule.trim()
        || !['csharp', 'pending'].includes(row.implementation)
        || (row.implementation === 'csharp' && (typeof row.test !== 'string' || !row.test.trim()))) reject('MANIFEST_INVALID', row.id);
    if (Object.hasOwn(row, 'acceptancePhase') && (!Number.isSafeInteger(row.acceptancePhase) || row.acceptancePhase < 1)) reject('MANIFEST_PHASE', row.id);
    caseTimeout(row, manifest);
  }
  let mode, value, list = false;
  for (let i = 0; i < args.length; i++) {
    const arg = args[i];
    if (arg === '--list' && !list) { list = true; continue; }
    if (mode || !['--all', '--group', '--case', '--phase'].includes(arg)) reject('USAGE', `Unknown/conflicting option ${arg}`);
    mode = arg;
    if (arg !== '--all') {
      value = args[++i];
      if (!value || value.startsWith('--')) reject('USAGE', `Missing value for ${arg}`);
    }
  }
  if (!mode && !list) reject('USAGE', 'Use --all, --phase N, --group name, --case exact-id or --list');
  if (mode === '--phase' && (!/^[1-9][0-9]*$/.test(value) || !Number.isSafeInteger(Number(value)))) reject('USAGE', 'Phase must be a positive integer');
  if (mode === '--group' && !groups.includes(value)) reject('SELECTION_UNKNOWN', value);
  const deferred = mode === '--phase' ? manifest.cases.filter(row => (row.acceptancePhase ?? 1) > Number(value)) : [];
  const selected = manifest.cases.filter(row => !mode || mode === '--all' || (mode === '--phase'
    ? (row.acceptancePhase ?? 1) <= Number(value) : mode === '--group' ? row.group === value : row.id === value));
  if (!selected.length) reject('SELECTION_EMPTY', String(value));
  return { selected, deferred, list };
}
function caseTimeout(item, manifest) {
  if (!Object.hasOwn(item, 'childTimeoutMs')) return { argument: '45s', outerMs: manifest.childTimeoutMs ?? 45_000 };
  const value = item.childTimeoutMs;
  if (!Number.isSafeInteger(value) || value < 1 || value > 120_000) reject('MANIFEST_TIMEOUT', item.id);
  const seconds = Math.ceil(value / 1000);
  return { argument: `${seconds}s`, outerMs: seconds * 1000 + 2000 };
}
function evidence(root, name) {
  const file = inside(root, name);
  if (!fs.existsSync(file) || !fs.statSync(file).isFile() || fs.statSync(file).size === 0) reject('EVIDENCE_MISSING', name);
  return { path: name, sha256: fileHash(file) };
}
function reconcile(selected, results, context) {
  const seen = new Set(), expected = new Map(selected.map(row => [row.id, row]));
  for (const row of results) {
    if (seen.has(row.id)) reject('RESULT_DUPLICATE', row.id);
    seen.add(row.id);
    if (!expected.has(row.id)) reject('RESULT_UNKNOWN', row.id);
  }
  for (const item of selected) if (!seen.has(item.id)) reject('RESULT_MISSING', item.id);
  const accepted = [];
  for (const row of results) {
    const item = expected.get(row.id);
    if (row.status !== 'pass') reject('RESULT_NOT_PASS', row.id + ': ' + row.status);
    for (const [key, diagnostic] of [['nonce', 'NONCE_MISMATCH'], ['input', 'INPUT_MISMATCH'], ['tool', 'TOOL_MISMATCH'], ['platform', 'PLATFORM_MISMATCH']]) {
      if (!context[key] || row[key] !== context[key]) reject(diagnostic, row.id);
    }
    if (!row.location || !row.diagnostic || !Array.isArray(row.prerequisites) || ['build', 'discovery', 'execution'].some(p => !row.prerequisites.includes(p))) reject('SUBJECT_EVIDENCE', row.id);
    if (item.expectedDiagnostic && row.diagnostic !== item.expectedDiagnostic) reject('SUBJECT_DIAGNOSTIC', row.id + ': ' + row.diagnostic);
    if (!Array.isArray(row.evidence) || !row.evidence.length) reject('EVIDENCE_MISSING', row.id);
    const logs = row.evidence.map(name => evidence(context.directory, name));
    accepted.push({ id: row.id, status: 'pass', diagnostic: row.diagnostic, location: row.location, evidence: logs });
  }
  return accepted.sort((a, b) => a.id.localeCompare(b.id, 'en'));
}
function killTree(child) {
  if (!child?.pid) return;
  if (process.platform === 'win32') spawnSync('taskkill.exe', ['/pid', String(child.pid), '/T', '/F'], { windowsHide: true, timeout: 3000 });
  else { try { process.kill(-child.pid, 'SIGKILL'); } catch {} }
}
function environment(root) {
  return { ...workspace.isolationEnv(root),
    DOTNET_CLI_TELEMETRY_OPTOUT: '1', TESTINGPLATFORM_TELEMETRY_OPTOUT: '1', DOTNET_NOLOGO: '1', DOTNET_SKIP_FIRST_TIME_EXPERIENCE: '1',
    PHASE1_ROOT: root, PHASE1_NODE: process.execPath };
}
async function command(executable, args, context, label, timeoutMs = 60_000, extra = {}) {
  const log = path.join(context.directory, label + '.process.log'), fd = fs.openSync(log, 'wx');
  let child, timer, timedOut = false, spawnError;
  const started = new Date().toISOString();
  const result = await new Promise(resolve => {
    const configured=launch(context.root,executable,args);
    args=configured.args;
    child = spawn(executable, args, { cwd: configured.cwd, env: { ...environment(context.root), ...extra }, windowsHide: true,
      detached: process.platform !== 'win32', stdio: ['ignore', fd, fd], shell: false });
    context.children.add(child);
    child.once('error', error => { spawnError = error.code; });
    timer = setTimeout(() => { timedOut = true; killTree(child); }, timeoutMs);
    child.once('close', (code, signal) => resolve({ executable, args, pid: child.pid, code, signal, timedOut, timeoutMs, spawnError, started, ended: new Date().toISOString() }));
  });
  clearTimeout(timer); context.children.delete(child); fs.closeSync(fd);
  fs.writeFileSync(path.join(context.directory, label + '.command.json'), JSON.stringify(result, null, 2));
  context.commands.push({ ...result, log: path.basename(log), logSha256: fileHash(log) });
  if (spawnError) reject('TOOL_MISSING', label + ': ' + spawnError);
  if (timedOut) reject('CHILD_TIMEOUT', label);
  return { ...result, output: fs.readFileSync(log, 'utf8') };
}
function verifyReport(report, items) {
  const tests = report.results?.tests;
  if (!Array.isArray(tests) || !tests.length) reject('DISCOVERY_EMPTY', 'No executed test records');
  const seen = new Set();
  for (const test of tests) {
    const id = test.labels?.Case;
    if (seen.has(id)) reject('DISCOVERY_DUPLICATE', String(id));
    seen.add(id);
    const item = items.find(row => row.id === id);
    if (!item || test.name !== item.test) reject('DISCOVERY_WRONG', String(id));
    if (test.status === 'skipped') reject('TEST_SKIPPED', id);
    if (test.status !== 'passed') reject('TEST_FAILED', id + ': ' + test.message);
  }
  if (items.some(item => !seen.has(item.id))) reject('DISCOVERY_MISSING', items.filter(item => !seen.has(item.id)).map(item => item.id).join(', '));
  const s = report.results.summary;
  if (s.tests !== items.length || s.passed !== items.length || s.failed || s.skipped || s.pending || s.other
      || report.results.extra?.suites?.some(suite => suite.errors?.length)) reject('REPORT_INCONSISTENT', 'Counts/errors disagree');
  if (report.results.tool?.version?.split('+')[0] !== '4.0.2') reject('TOOL_MISMATCH', 'xUnit identity');
}
function repositoryPlan(item, root) {
  const spec = item.repositoryStages;
  if (item.id !== 'ac-08.complete' || item.test !== 'KingdomZombies.Phase1.Tests.ExemptionTests.Validate_BoundedExemptions_RequiresActualEngineBoundary'
      || spec?.method !== 'KingdomZombies.Phase1.Tests.ExemptionTests.Check_RepositoryStage_RequiresBoundedCompleteOriginalConjunction'
      || !Array.isArray(spec.sources) || !spec.sources.length || !Array.isArray(spec.projects) || spec.projects.length !== 5
      || digest(JSON.stringify(spec.sources)) !== spec.sourceSetSha256) reject('REPOSITORY_PLAN', item.id);
  const paths = new Set(), applicable = new Set();
  for (const row of spec.sources) {
    if (paths.has(row.path) || !['source', 'configuration', 'outside-validator'].includes(row.kind)
        || !Array.isArray(row.owners) || new Set(row.owners).size !== row.owners.length
        || row.owners.some(owner => !spec.projects.includes(owner))) reject('REPOSITORY_SOURCE_DECLARATION', row.path);
    paths.add(row.path); inside(root, row.path);
    if (row.kind !== 'outside-validator') {
      applicable.add(row.path);
      if (fileHash(inside(root, row.path)) !== row.sha256) reject('REPOSITORY_SOURCE_HASH', row.path);
    }
  }
  for (const binding of spec.bindings) if (fileHash(inside(root, binding.path)) !== binding.sha256) reject('REPOSITORY_BINDING', binding.path);
  const expected = ['repository-auth', 'repository-shape', 'repository-newline', 'repository-companion-shape'],
    ids = new Set(), actions = new Set(), covered = new Set(), cohorts = new Set();
  for (const stage of spec.stages) {
    if (!/^[a-z0-9-]+$/.test(stage.id) || ids.has(stage.id)) reject('REPOSITORY_STAGE_ID', stage.id);
    ids.add(stage.id);
    if (stage.action === 'repository-sources') {
      if (!Array.isArray(stage.sources) || !stage.sources.length) reject('REPOSITORY_SOURCE_EMPTY', stage.id);
      for (const source of stage.sources) {
        if (!applicable.has(source) || covered.has(source)) reject('REPOSITORY_SOURCE_UNION', source);
        covered.add(source);
      }
    } else if (stage.action === 'repository-companion-cohort') {
      const cohort = spec.cohorts.find(row => row.id === stage.cohort);
      if (!cohort || cohorts.has(stage.cohort) || JSON.stringify(stage.rowIds) !== JSON.stringify(cohort.rowIds)) reject('REPOSITORY_COHORT', stage.id);
      cohorts.add(stage.cohort);
    } else {
      if (!expected.includes(stage.action) || actions.has(stage.action)) reject('REPOSITORY_STAGE_ACTION', stage.id);
      actions.add(stage.action);
    }
  }
  if (covered.size !== applicable.size || cohorts.size !== spec.cohorts.length || actions.size !== expected.length
      || expected.some(action => !actions.has(action)) || spec.keys.length !== 42 || new Set(spec.keys).size !== 42) reject('REPOSITORY_COMPLETE_PLAN', item.id);
  return spec;
}
function verifyRepositoryPayload(item, data, context) {
  const spec = item.repositoryStages;
  if (data?.caseId !== item.id) reject('REPOSITORY_PAYLOAD_FOREIGN', item.id);
  for (const key of ['nonce', 'input', 'tool', 'platform']) if (!context[key] || data[key] !== context[key]) reject('REPOSITORY_PAYLOAD_STALE', key);
  for (const key of ['keys', 'bindings', 'cohorts']) if (JSON.stringify(data[key]) !== JSON.stringify(spec[key])) reject('REPOSITORY_PAYLOAD_ALTERED', key);
  return true;
}
function verifyRepositorySources(item, rows) {
  const expected = new Map(item.repositoryStages.sources.filter(row => row.kind !== 'outside-validator').map(row => [row.path, row])), seen = new Set();
  if (!Array.isArray(rows) || rows.length !== expected.size) reject('REPOSITORY_OWNER_COUNT', item.id);
  for (const row of rows) {
    const source = expected.get(row.path);
    if (!source || seen.has(row.path)) reject('REPOSITORY_OWNER_UNKNOWN', row.path);
    seen.add(row.path);
    if (row.sha256 !== source.sha256 || JSON.stringify(row.owners) !== JSON.stringify(source.owners)
        || !Array.isArray(row.errors) || row.errors.length) reject('REPOSITORY_OWNER_MISMATCH', row.path);
  }
  return true;
}
function repositoryControls(item, context) {
  const spec = item.repositoryStages, original = { caseId: item.id, nonce: context.nonce, input: context.input,
    tool: context.tool, platform: context.platform, keys: spec.keys, bindings: spec.bindings, cohorts: spec.cohorts }, results = [];
  verifyRepositoryPayload(item, original, context);
  for (const kind of ['missing', 'foreign', 'stale', 'key', 'binding', 'cohort']) {
    let changed = structuredClone(original);
    if (kind === 'missing') changed = null;
    else if (kind === 'foreign') changed.caseId = 'other';
    else if (kind === 'stale') changed.input = 'old';
    else if (kind === 'key') changed.keys[0] = 'other';
    else if (kind === 'binding') changed.bindings[0].sha256 = 'other';
    else changed.cohorts[0].sourceInputsSha256 = 'other';
    let error; try { verifyRepositoryPayload(item, changed, context); } catch (failure) { error = failure.diagnostic; }
    if (!error) reject('REPOSITORY_CONTROL_ACCEPTED', kind);
    verifyRepositoryPayload(item, original, context); results.push({ kind, rejected: error, restored: true });
  }
  const sources = spec.sources.filter(row => row.kind !== 'outside-validator').map(row => ({ path: row.path,
    sha256: row.sha256, owners: row.owners, errors: [] }));
  verifyRepositorySources(item, sources);
  for (const kind of ['missing-source', 'duplicate-source', 'unknown-source', 'foreign-owner', 'missing-owner', 'changed-source']) {
    const changed = structuredClone(sources);
    if (kind === 'missing-source') changed.pop();
    else if (kind === 'duplicate-source') changed[changed.length - 1] = structuredClone(changed[0]);
    else if (kind === 'unknown-source') changed[0].path = 'unknown';
    else if (kind === 'changed-source') changed[0].sha256 = 'other';
    else { const row = changed.find(source => source.owners.length); row.owners = kind === 'missing-owner' ? [] : ['foreign']; }
    let error; try { verifyRepositorySources(item, changed); } catch (failure) { error = failure.diagnostic; }
    if (!error) reject('REPOSITORY_CONTROL_ACCEPTED', kind);
    verifyRepositorySources(item, sources); results.push({ kind, rejected: error, restored: true });
  }
  return { scope: 'Actual production payload/source-union validators; synthetic controls confer no execution credit', positive: true, results };
}
function verifyRepositoryExecution(item, observations, context) {
  const spec = item.repositoryStages, sources = observations.filter(row => row.stageId.startsWith('repository-sources-'))
    .flatMap(row => row.details?.checkedSources ?? []);
  verifyRepositorySources(item, sources);
  const shape = observations.find(row => row.stageId === 'repository-shape')?.details;
  if (shape?.tracked !== spec.sources.length || shape?.projects !== 5) reject('REPOSITORY_SHAPE_EVIDENCE', item.id);
  const companion = observations.filter(row => spec.stages.find(stage => stage.id === row.stageId)?.action === 'repository-companion-cohort');
  if (companion.length !== spec.cohorts.length || companion.reduce((sum, row) => sum + (row.details?.count ?? 0), 0) < 16
      || companion.reduce((sum, row) => sum + (row.details?.current ?? 0), 0) <= 0) reject('REPOSITORY_COMPANION_UNION', item.id);
  return { sources: sources.length, ownerContexts: sources.reduce((sum, row) => sum + row.owners.length, 0),
    tracked: shape.tracked, cohorts: companion.length, execution: 'Fresh complete original AC08 conjunction' };
}

function callerRepository(root) {
  const manifest = read(inside(root, 'src/godot-game/tools/verification/required-cases.json'));
  const owner = manifest.cases.find(row => row.id === 'ac-08.complete');
  if (!owner) reject('CALLER_REPOSITORY_REQUIRED', 'ac-08.complete');
  return repositoryPlan(owner, root);
}
function callerExpected(item, root) {
  const repository = callerRepository(root), stage = (prefix, key, action, extra = {}) => ({ id: prefix + '-' + key, action, ...extra });
  const sources = prefix => repository.stages.filter(row => ['repository-auth', 'repository-shape', 'repository-sources'].includes(row.action))
    .map(row => stage(prefix, row.id, 'repository', { sourceStageId: row.id }));
  const authority = read(inside(root, 'src/godot-game/tools/verification/required-cases.json')).callerAuthority;
  if (authority?.registry !== 'docs/code/generated-callsite-adaptations.json'
      || fileHash(inside(root, authority.registry)) !== authority.sha256 || !Array.isArray(authority.collisionRowIds)
      || !authority.collisionRowIds.length || new Set(authority.collisionRowIds).size !== authority.collisionRowIds.length) reject('CALLER_COLLISION_AUTHORITY', item.id);
  const collisionRows = authority.collisionRowIds;
  const godot = prefix => [stage(prefix, 'game-build', 'game-build'), stage(prefix, 'adjacent-restored', 'godot-adjacent'),
    ...repository.cohorts.map((row, index) => stage(prefix, 'calls-' + index, 'godot-cohort', { cohort: row.id, rowIds: row.rowIds })),
    stage(prefix, 'collision-restored', 'godot-collision', { rowIds: collisionRows })];
  switch (item.id) {
    case 'R-50.positive': return [stage('r50', 'game-build', 'game-build'), ...sources('r50')];
    case 'rules.exemptions': return [...sources('exempt'), stage('exempt', 'bypass-restored', 'suppression-bypass')];
    case 'rules.godot-companion-positive': return [stage('companion', 'baseline', 'companion-shape'),
      ...repository.cohorts.map((row, index) => stage('companion', 'calls-' + index, 'companion-positive', { cohort: row.id, rowIds: row.rowIds }))];
    case 'rules.godot-companion-negative': return [stage('companion-negative', 'baseline', 'companion-shape'),
      ...repository.cohorts.map((row, index) => stage('companion-negative', 'calls-' + index, 'companion-negative', { cohort: row.id, rowIds: row.rowIds }))];
    case 'rules.godot-callsite-boundary': return godot('godot');
    case 'core.permissions': {
      const manifest = read(inside(root, 'src/godot-game/tools/verification/required-cases.json'));
      const aliases = { 'boundary-warm-spans-missing-observations': 'warm-spans-missing-observations',
        'boundary-warm-calls-missing-observations': 'warm-calls-missing-observations' };
      const producers = ['engine.generated-origin', 'engine.generated-boundary'].flatMap(id => {
        const owner = manifest.cases.find(row => row.id === id); producerPlan(owner, root);
        return owner.producerStages.stages.map(row => stage('foundation', aliases[row.id] ?? row.id, 'producer', { sourceCaseId: id,
          sourceStageId: row.id, cohort: row.cohort ?? null, rowIds: row.rowIds ?? [] }));
      });
      return [...['event', 'neighbors', 'methods', 'visibility'].map(kind => stage('foundation', 'native-' + kind, 'foundation-native', { kind })),
        ...producers, ...godot('foundation-godot')];
    }
    default: reject('CALLER_CASE_UNKNOWN', item.id);
  }
}
function callerPlan(item, root) {
  const identities = {
    'R-50.positive': ['KingdomZombies.Phase1.Tests.ExemptionTests.Generate_ExactObservedPermissions_AcceptsOwnedSources', 'EXEMPTIONS_EXACT'],
    'rules.exemptions': ['KingdomZombies.Phase1.Tests.ExemptionTests.Validate_RealSuppressionBypass_RequiresSourceGuard', 'UNREGISTERED_REAL_BUILD_SUPPRESSION'],
    'rules.godot-companion-positive': ['KingdomZombies.Phase1.Tests.ExemptionTests.Build_GenuineGodotAndHandwrittenCompanion_AcceptsExactOutput', 'GODOT_COMPANION_COMPILED'],
    'rules.godot-companion-negative': ['KingdomZombies.Phase1.Tests.ExemptionTests.Build_HandwrittenGodotBody_RetainsExactDiagnostic', 'GODOT_HANDWRITTEN_SCR0001'],
    'rules.godot-callsite-boundary': ['KingdomZombies.Phase1.Tests.GodotCallSiteAdapterTests.Analyze_GodotCallSitePermissions_RejectsOutsideMembers', 'GODOT_CALLSITE_BOUNDARY_REJECTED'],
    'core.permissions': ['*FoundationPermissionTests.EventAccess_ProductionCalls_RequireAuthenticPrecedingGuard', 'EXACT_PRODUCTION_EVENT_GUARD']
  };
  const identity = identities[item.id], spec = item.callerStages;
  if (!identity || item.test !== identity[0] || spec?.diagnostic !== identity[1]
      || spec.method !== 'KingdomZombies.Phase1.Tests.ExemptionTests.Check_CallerStage_RequiresCompleteOriginalConjunction'
      || spec.catalog !== (item.id === 'core.permissions' ? 'src/godot-game/tools/gameplay-checks/required-cases.json' : 'src/godot-game/tools/verification/required-cases.json')
      || spec.childMs !== 45000 || spec.setupMs !== 60000 || !Array.isArray(spec.stages) || !spec.stages.length
      || spec.overallMs !== 180000 + spec.stages.length * 48000
      || spec.repositorySha256 !== digest(JSON.stringify(callerRepository(root)))) reject('CALLER_PLAN', item.id);
  const published = read(inside(root, spec.catalog)).cases.filter(row => row.id === item.id);
  if (published.length !== 1 || JSON.stringify(published[0]) !== JSON.stringify(item)) reject('CALLER_CATALOG_BINDING', item.id);
  if (JSON.stringify(spec.stages) !== JSON.stringify(callerExpected(item, root))
      || new Set(spec.stages.map(row => row.id)).size !== spec.stages.length) reject('CALLER_RESPONSIBILITY_UNION', item.id);
  return spec;
}
function callerBindings(item, root, assembly) {
  const directory = path.dirname(assembly), binaries = fs.readdirSync(directory).filter(name => /\.(dll|deps\.json|runtimeconfig\.json)$/.test(name))
    .sort().map(name => ({ path: path.relative(root, path.join(directory, name)).replaceAll('\\', '/'), sha256: fileHash(path.join(directory, name)) }));
  const spec = callerPlan(item, root);
  return { catalog: spec.catalog, catalogSha256: fileHash(inside(root, spec.catalog)), planSha256: digest(JSON.stringify(spec)),
    runnerSha256: fileHash(inside(root, 'src/godot-game/tools/verification/check.cjs')), phase1CatalogSha256: fileHash(inside(root, 'src/godot-game/tools/verification/required-cases.json')), binaries };
}
function callerAction(stage, repository) {
  return stage.action === 'repository' ? repository.stages.find(row => row.id === stage.sourceStageId)?.action : stage.action;
}
function verifyCallerExecution(item, observations, root) {
  const repository = callerRepository(root), expected = item.callerStages.stages;
  if (['R-50.positive', 'rules.exemptions'].includes(item.id)) {
    const sources = observations.filter(row => callerAction(expected.find(stage => stage.id === row.stageId), repository) === 'repository-sources')
      .flatMap(row => row.details?.checkedSources ?? []);
    verifyRepositorySources({ ...item, repositoryStages: repository }, sources);
    const shape = observations.find(row => callerAction(expected.find(stage => stage.id === row.stageId), repository) === 'repository-shape')?.details;
    if (shape?.tracked !== repository.sources.length || shape.projects !== repository.projects.length) reject('CALLER_SOURCE_SHAPE', item.id);
  }
  if (['rules.godot-companion-positive', 'rules.godot-companion-negative'].includes(item.id)) {
    const kind = item.id === 'rules.godot-companion-negative' ? 'companion-negative' : 'companion-positive';
    const cohorts = observations.filter(row => expected.find(stage => stage.id === row.stageId).action === kind);
    if (cohorts.length !== repository.cohorts.length || cohorts.reduce((n, row) => n + (row.details?.count ?? 0), 0) < 16
        || cohorts.reduce((n, row) => n + (row.details?.current ?? 0), 0) <= 0) reject('CALLER_COMPANION_UNION', item.id);
  }
  for (const row of observations) {
    const stage = expected.find(value => value.id === row.stageId);
    if (['godot-cohort', 'godot-collision', 'companion-negative'].includes(stage.action) && JSON.stringify(row.details?.rowIds) !== JSON.stringify(stage.rowIds)) reject('CALLER_COLLISION_UNION', stage.id);
  }
  return true;
}
function verifyCallerPermissionDescriptor(item, descriptor, auth, expectedHash) {
  const stage = item.callerStages.stages.find(row => row.action === 'repository' && row.sourceStageId === 'repository-auth');
  if (!stage) {
    if (descriptor !== undefined) reject('CALLER_UNEXPECTED_PAYLOAD', item.id);
    return true;
  }
  if (!descriptor || typeof descriptor !== 'object' || Array.isArray(descriptor)
      || JSON.stringify(Object.keys(descriptor).sort()) !== JSON.stringify(['caseUid', 'path', 'sha256', 'testUid'])
      || descriptor.path !== stage.id + '/permission-keys.json' || descriptor.sha256 !== expectedHash
      || !auth || descriptor.caseUid !== auth.caseUid || descriptor.testUid !== auth.testUid) reject('CALLER_PERMISSION_DESCRIPTOR', item.id);
  return true;
}
function verifyCallerResult(item, result, context) {
  const spec = callerPlan(item, context.root);
  const fields = ['id', 'status', 'diagnostic', 'execution', 'location', 'report', 'stages', 'monolithicExecuted',
    'aggregatedExecution', 'callerCatalog', 'planSha256', 'nonce', 'input', 'tool', 'platform', 'artifacts'];
  if (Object.keys(result).some(key => !fields.includes(key)) || result.aggregatedExecution !== true
      || result.id !== item.id || result.status !== 'pass' || result.execution !== 'aggregated-caller-stages'
      || result.monolithicExecuted !== false || result.callerCatalog !== spec.catalog
      || result.planSha256 !== digest(JSON.stringify(spec))) reject('CALLER_RESULT_SCHEMA', item.id);
  for (const key of ['nonce', 'input', 'tool', 'platform']) if (!context[key] || result[key] !== context[key]) reject('CALLER_RESULT_CONTEXT', key);
  const artifactPaths = new Set();
  if (!Array.isArray(result.artifacts) || !result.artifacts.length) reject('CALLER_ARTIFACTS_REQUIRED', item.id);
  for (const artifact of result.artifacts) {
    if (artifactPaths.has(artifact.path) || fileHash(inside(context.directory, artifact.path)) !== artifact.sha256) reject('CALLER_ARTIFACT_HASH', artifact.path);
    artifactPaths.add(artifact.path);
  }
  const prefix = item.id + '/', required = name => {
    const relative = prefix + name;
    if (!artifactPaths.has(relative)) reject('CALLER_ARTIFACT_MISSING', relative);
    return read(inside(context.directory, relative));
  };
  const union = required('producer-union.json'), selection = required('producer-selection.json'), exact = required('producer-exact-uids.json');
  const unionFields = ['caseId', 'execution', 'aggregatedExecution', 'monolithicExecuted', 'status', 'selected', 'executed',
    'stages', 'permissionPayload', 'bindings', 'aggregateMs', 'overallBudgetMs'];
  if (Object.keys(union).some(key => !unionFields.includes(key)) || union.aggregatedExecution !== true
      || result.report?.path !== prefix + 'producer-union.json'
      || result.report.sha256 !== fileHash(inside(context.directory, result.report.path))) reject('CALLER_UNION_REFERENCE', item.id);
  const assembly = inside(context.root, 'src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll');
  if (JSON.stringify(union.bindings) !== JSON.stringify(callerBindings(item, context.root, assembly))
      || JSON.stringify(selection.plan) !== JSON.stringify(spec) || selection.caseId !== item.id || selection.monolithicExecuted !== false
      || union.execution !== 'aggregated-caller-stages' || union.monolithicExecuted !== false || union.caseId !== item.id || union.status !== 'pass'
      || union.selected !== spec.stages.length || union.executed !== spec.stages.length
      || JSON.stringify(result.stages) !== JSON.stringify(union.stages)) reject('CALLER_EVIDENCE_BINDING', item.id);
  verifyProducerUnion(item, union.stages, context); verifyCallerExecution(item, union.stages, context.root);
  if (exact.length !== spec.stages.length || JSON.stringify(exact.map(row => row.stage)) !== JSON.stringify(spec.stages)) reject('CALLER_EXACT_SELECTION', item.id);
  for (const row of union.stages) {
    const stage = spec.stages.find(value => value.id === row.stageId), chosen = exact.find(value => value.stage.id === row.stageId)?.selected;
    if (!chosen || chosen.uid !== row.caseUid) reject('CALLER_DISCOVERY_UID', row.stageId);
    const command = required(row.stageId + '/test.command.json'), report = required(row.report.path);
    const timeout = producerStageTimeout(item, stage, context.platform);
    if (command.code !== 0 || command.timedOut || !(command.pid > 0) || command.timeoutMs !== timeout
        || command.args[command.args.indexOf('--timeout') + 1] !== `${timeout / 1000}s`
        || command.args[command.args.indexOf('--filter-uid') + 1] !== chosen.uid
        || Date.parse(command.ended) - Date.parse(command.started) !== row.elapsedMs) reject('CALLER_COMMAND', row.stageId);
    const receipts = row.progress.map(reference => {
      const value = required(reference.path);
      if (fileHash(inside(context.directory, prefix + reference.path)) !== reference.sha256) reject('CALLER_PROGRESS_HASH', reference.path);
      return value;
    });
    const receipt = receipts.at(-1);
    if (receipt?.caseId !== item.id || receipt.catalog !== spec.catalog || JSON.stringify(receipt.data) !== JSON.stringify(row.details)
        || fileHash(inside(context.directory, prefix + row.report.path)) !== row.report.sha256) reject('CALLER_RECEIPT', row.stageId);
    verifyProducerStage(report, chosen, stage, receipt, context);
    if (!Array.isArray(row.proofs) || !row.proofs.length) reject('CALLER_PROOF_REQUIRED', row.stageId);
    const child = { ...context, directory: inside(context.directory, prefix + row.stageId) };
    for (const proof of row.proofs) {
      const record = required(proof.path);
      if (fileHash(inside(context.directory, prefix + proof.path)) !== proof.sha256) reject('CALLER_PROOF_HASH', proof.path);
      reconcile([item], [record], child);
    }
  }
  const payloadStage = spec.stages.find(stage => stage.action === 'repository' && stage.sourceStageId === 'repository-auth');
  if (payloadStage) {
    const payload = required(payloadStage.id + '/permission-keys.json');
    verifyCallerPermissionDescriptor(item, union.permissionPayload, union.stages.find(row => row.stageId === payloadStage.id),
      fileHash(inside(context.directory, prefix + payloadStage.id + '/permission-keys.json')));
    verifyRepositoryPayload({ ...item, repositoryStages: callerRepository(context.root) }, payload, context);
  } else verifyCallerPermissionDescriptor(item, union.permissionPayload);
  return true;
}
function callerEvidenceControls(item, result, context) {
  verifyCallerResult(item, result, context); const outcomes = [];
  for (const kind of ['forged-monolithic', 'forged-one-test', 'wrong-report-reference', 'missing', 'duplicate', 'unknown', 'failed', 'stale', 'wrongcase', 'wrongcatalog', 'context', 'artifact', 'payload']) {
    const changed = structuredClone(result);
    if (kind === 'forged-monolithic') changed.monolithicExecuted = true;
    else if (kind === 'forged-one-test') changed.testReport = 'forged-one-passed-test.ctrf.json';
    else if (kind === 'wrong-report-reference') changed.report.path = 'forged-union.json';
    else if (kind === 'missing') changed.stages.pop();
    else if (kind === 'duplicate') changed.stages[changed.stages.length - 1] = structuredClone(changed.stages[0]);
    else if (kind === 'unknown') changed.stages[0].stageId = 'unknown';
    else if (kind === 'failed') changed.stages[0].status = 'fail';
    else if (kind === 'stale') changed.input = 'old';
    else if (kind === 'wrongcase') changed.id = 'core.other';
    else if (kind === 'wrongcatalog') changed.callerCatalog = 'src/godot-game/tools/lan-checks/required-cases.json';
    else if (kind === 'context') changed.nonce = 'foreign';
    else if (kind === 'artifact') changed.artifacts[0].sha256 = '0'.repeat(64);
    else changed.artifacts = changed.artifacts.filter(row => !row.path.endsWith('/permission-keys.json') && !row.path.endsWith('/producer-union.json'));
    let diagnostic; try { verifyCallerResult(item, changed, context); } catch (error) { diagnostic = error.diagnostic; }
    if (!diagnostic) reject('CALLER_CONTROL_ACCEPTED', kind);
    verifyCallerResult(item, result, context); outcomes.push({ kind, diagnostic, restored: true });
  }
  const union = read(inside(context.directory, item.id + '/producer-union.json'));
  const payloadStage = item.callerStages.stages.find(row => row.action === 'repository' && row.sourceStageId === 'repository-auth');
  const auth = payloadStage ? union.stages.find(row => row.stageId === payloadStage.id) : undefined;
  const expectedHash = payloadStage ? fileHash(inside(context.directory, item.id + '/' + payloadStage.id + '/permission-keys.json')) : undefined;
  verifyCallerPermissionDescriptor(item, union.permissionPayload, auth, expectedHash);
  for (const kind of payloadStage ? ['path', 'caseUid', 'testUid', 'sha256', 'extra', 'missing'] : ['unexpected']) {
    let changed = structuredClone(union.permissionPayload ?? {});
    if (kind === 'path') changed.path = 'foreign/permission-keys.json';
    else if (['caseUid', 'testUid', 'sha256'].includes(kind)) changed[kind] = '0'.repeat(64);
    else if (kind === 'missing') changed = undefined;
    else changed.foreign = true;
    let diagnostic; try { verifyCallerPermissionDescriptor(item, changed, auth, expectedHash); } catch (error) { diagnostic = error.diagnostic; }
    if (!diagnostic) reject('CALLER_CONTROL_ACCEPTED', 'descriptor-' + kind);
    verifyCallerPermissionDescriptor(item, union.permissionPayload, auth, expectedHash);
    outcomes.push({ kind: 'descriptor-' + kind, diagnostic, restored: true });
  }
  return { scope: 'Same canonical evidence and descriptor validators using this fresh actual union; controls confer no execution credit', outcomes };
}
function finishCallerResult(item, bounded, context) {
  const parent = { ...context, directory: path.dirname(context.directory) }, files = [];
  function visit(directory) {
    for (const name of fs.readdirSync(directory).sort()) {
      const file = path.join(directory, name), stat = fs.lstatSync(file);
      if (stat.isSymbolicLink()) reject('CALLER_ARTIFACT_SYMLINK', file);
      if (stat.isDirectory()) visit(file);
      else files.push({ path: path.relative(parent.directory, file).replaceAll('\\', '/'), sha256: fileHash(file) });
    }
  }
  visit(context.directory);
  const result = { ...bounded, aggregatedExecution: true, monolithicExecuted: false, callerCatalog: item.callerStages.catalog,
    planSha256: digest(JSON.stringify(item.callerStages)), nonce: context.nonce, input: context.input,
    tool: context.tool, platform: context.platform, artifacts: files };
  verifyCallerResult(item, result, parent);
  const controls = callerEvidenceControls(item, result, parent), controlPath = path.join(context.directory, 'caller-evidence-controls.json');
  fs.writeFileSync(controlPath, JSON.stringify(controls, null, 2), { flag: 'wx' });
  result.artifacts.push({ path: item.id + '/caller-evidence-controls.json', sha256: fileHash(controlPath) });
  verifyCallerResult(item, result, parent);
  return result;
}
function producerPlan(item, root) {
  if (item.callerStages) return callerPlan(item, root);
  if (item.repositoryStages) return repositoryPlan(item, root);
  const identities = { 'engine.generated-origin': 'Check_EmittedEngineSpans_RequiresAuthenticatedOrigin',
    'engine.generated-boundary': 'Check_EngineSpanPermissions_RejectsAdjacentImpostors' };
  const spec = item.producerStages, prefix = 'KingdomZombies.Phase1.Tests.EngineGeneratedTests.';
  if (!Object.hasOwn(identities, item.id) || item.test !== prefix + identities[item.id]
      || spec?.method !== prefix + 'Check_ProducerStage_RequiresCompleteOriginalPredicate'
      || !Array.isArray(spec.stages) || !spec.stages.length) reject('PRODUCER_PLAN', item.id);
  const registries = {}, required = [], key = stage => [stage.action, stage.registry ?? '', stage.cohort ?? '', stage.field ?? ''].join('|');
  for (const registry of ['spans', 'calls']) {
    const expected = registry === 'spans' ? 'docs/code/engine-generated-spans.json' : 'docs/code/generated-callsite-adaptations.json';
    const binding = spec.registries?.[registry];
    if (binding?.path !== expected || fileHash(inside(root, expected)) !== binding.sha256) reject('PRODUCER_REGISTRY_BINDING', registry);
    if (!Array.isArray(binding.cohorts) || !binding.cohorts.length
        || new Set(binding.cohorts.map(row => row.id)).size !== binding.cohorts.length
        || binding.cohorts.some(row => !Array.isArray(row.rowIds) || !row.rowIds.length)) reject('PRODUCER_COHORT_DECLARATION', registry);
    registries[registry] = { producerInventories: binding.cohorts };
  }
  const origin = item.id === 'engine.generated-origin';
  if (origin) required.push('build|||', 'shape|spans||', 'fixed|spans||');
  else required.push('shape|spans||', 'shape|calls||', 'sealing|||',
    'extra-call|calls|' + registries.calls.producerInventories.at(-1).id + '|',
    ...['live', 'changed', 'historical'].map(field => 'reproduction|spans||' + field));
  for (const registry of origin ? ['spans'] : ['spans', 'calls']) {
    const action = origin ? 'origin' : registry === 'spans' ? 'boundary' : 'cohort';
    for (const cohort of registries[registry].producerInventories) required.push([action, registry, cohort.id, ''].join('|'));
  }
  if (!origin) {
    for (const field of ['missing-observations', 'extra-row', 'sha256', 'packageSha256', 'surface', 'member', 'diagnostic',
      'line', 'column', 'length', 'missing-row', 'source-input', 'missing-source', 'package', 'missing-package', 'borrowed-row', 'wrong-case'])
      required.push(['inventory-negative', 'spans', registries.spans.producerInventories.at(-1).id, field].join('|'));
    for (const registry of ['spans', 'calls']) for (const field of ['snapshot-bytes', 'snapshot-missing', 'snapshot-borrowed',
      'source-input', 'missing-source', 'package', 'missing-package', 'borrowed-row', 'missing-observations'])
      required.push(['warm-negative', registry, 'inspection03-08035ba', field].join('|'));
  }
  const ids = new Set(), responsibilities = new Set();
  for (const stage of spec.stages) {
    if (!/^[A-Za-z0-9-]+$/.test(stage.id) || ids.has(stage.id)) reject('PRODUCER_STAGE_DUPLICATE', stage.id);
    ids.add(stage.id); const responsibility = key(stage);
    if (!required.includes(responsibility) || responsibilities.has(responsibility)) reject('PRODUCER_STAGE_RESPONSIBILITY', stage.id);
    responsibilities.add(responsibility);
    if (['origin', 'boundary', 'cohort'].includes(stage.action)) {
      const cohort = registries[stage.registry].producerInventories.find(row => row.id === stage.cohort);
      if (JSON.stringify(cohort.rowIds) !== JSON.stringify(stage.rowIds)) reject('PRODUCER_STAGE_ROWS', stage.id);
    } else if (Object.hasOwn(stage, 'rowIds')) reject('PRODUCER_STAGE_EXTRA_ROWS', stage.id);
  }
  if (responsibilities.size !== required.length || required.some(value => !responsibilities.has(value))) reject('PRODUCER_STAGE_MISSING', item.id);
  return spec;
}
function verifyProducerUnion(item, rows, context) {
  const expected = new Map((item.producerStages ?? item.repositoryStages ?? item.callerStages).stages.map(stage => [stage.id, stage])), seen = new Set(), uids = new Set();
  if (!Array.isArray(rows) || rows.length !== expected.size || !rows.length) reject('PRODUCER_UNION_COUNT', item.id);
  for (const row of rows) {
    if (seen.has(row.stageId)) reject('PRODUCER_UNION_DUPLICATE', row.stageId);
    seen.add(row.stageId); const stage = expected.get(row.stageId);
    if (!stage || row.caseId !== item.id) reject('PRODUCER_UNION_UNKNOWN', row.stageId);
    if (row.status !== 'pass') reject('PRODUCER_UNION_FAILED', row.stageId);
    for (const field of ['nonce', 'input', 'tool', 'platform']) if (!context[field] || row[field] !== context[field]) reject('PRODUCER_UNION_STALE', field);
    if (row.cohort !== (stage.cohort ?? null) || JSON.stringify(row.rowIds) !== JSON.stringify(stage.rowIds ?? [])) reject('PRODUCER_UNION_ROWS', row.stageId);
    if (!/^[a-f0-9]{64}$/.test(row.caseUid ?? '') || !/^[a-f0-9]{64}$/.test(row.testUid ?? '')
        || uids.has(row.caseUid)) reject('PRODUCER_UNION_UID', row.stageId);
    uids.add(row.caseUid);
  }
  if ([...expected.keys()].some(id => !seen.has(id))) reject('PRODUCER_UNION_MISSING', item.id);
  return true;
}
function producerUnionControls(item, context) {
  const original = (item.producerStages ?? item.repositoryStages ?? item.callerStages).stages.map(stage => ({ stageId: stage.id, caseId: item.id, status: 'pass',
    caseUid: digest('control-case-' + stage.id), testUid: digest('control-test-' + stage.id), cohort: stage.cohort ?? null,
    rowIds: stage.rowIds ?? [], nonce: context.nonce, input: context.input, tool: context.tool, platform: context.platform }));
  verifyProducerUnion(item, original, context); const results = [];
  for (const kind of ['missing', 'duplicate', 'unknown', 'empty', 'stale', 'failed', 'wrongcase', 'wrongcohort', 'wrongrows', 'wronguid']) {
    const changed = structuredClone(original);
    if (kind === 'missing') changed.pop();
    else if (kind === 'empty') changed.length = 0;
    else if (kind === 'duplicate') changed[changed.length - 1] = structuredClone(changed[0]);
    else if (kind === 'unknown') changed[0].stageId = 'unknown';
    else if (kind === 'stale') changed[0].nonce = 'old';
    else if (kind === 'failed') changed[0].status = 'fail';
    else if (kind === 'wrongcase') changed[0].caseId = 'engine.unknown';
    else if (kind === 'wrongcohort') changed[0].cohort = 'unknown';
    else if (kind === 'wrongrows') changed[0].rowIds = ['unknown'];
    else changed[0].caseUid = changed[1].caseUid;
    let failure; try { verifyProducerUnion(item, changed, context); } catch (error) { failure = error.diagnostic; }
    if (!failure) reject('PRODUCER_CONTROL_ACCEPTED', kind);
    verifyProducerUnion(item, original, context); results.push({ kind, rejected: failure, restored: true });
  }
  return { scope: 'Actual production union validator protocol controls; synthetic records are not execution evidence', positive: true, results };
}
function verifyProducerStage(report, selected, stage, receipt, context) {
  const tests = report.results?.tests, summary = report.results?.summary;
  if (!Array.isArray(tests) || tests.length !== 1 || tests[0].status !== 'passed' || tests[0].name !== selected.displayName
      || tests[0].extra?.method !== selected.type.methodName || summary?.tests !== 1 || summary.passed !== 1
      || summary.failed || summary.skipped || summary.pending || summary.other
      || report.results.extra?.suites?.some(suite => suite.errors?.length)
      || report.results.tool?.version?.split('+')[0] !== '4.0.2') reject('PRODUCER_STAGE_CTRF', stage.id);
  if (receipt.stageId !== stage.id || receipt.status !== 'pass' || receipt.caseUid !== selected.uid
      || receipt.testUid !== tests[0].extra?.id) reject('PRODUCER_STAGE_IDENTITY', stage.id);
  for (const key of ['nonce', 'input', 'tool', 'platform']) if (!context[key] || receipt[key] !== context[key]) reject('PRODUCER_STAGE_STALE', key);
}
function producerStageTimeout(item, stage, platform) {
  const windows = /^win32-x64(?:-|$)/.test(platform);
  const fixedReplay = item.id === 'engine.generated-origin' && stage.id === 'origin-fixed'
    || item.id === 'core.permissions' && stage.id === 'foundation-origin-fixed';
  return windows && fixedReplay ? 90000 : 45000;
}

async function runProducerStages(item, context, executable, assembly) {
  const spec = producerPlan(item, context.root), observations = [], log = path.join(context.directory, 'test.process.log');
  const caller = Boolean(item.callerStages), repository = caller ? callerRepository(context.root) : item.repositoryStages;
  const payloadOwner = repository ? { ...item, repositoryStages: repository } : item;
  const action = stage => caller ? callerAction(stage, repository) : stage.action;
  const bindings = caller ? callerBindings(item, context.root, assembly) : undefined;
  const started = Date.now(), withinBudget = () => { if (caller && Date.now() - started > spec.overallMs) reject('CALLER_AGGREGATE_DEADLINE', item.id); };
  let permissionPayload;
  fs.writeFileSync(log, 'Explicit aggregate of executed bounded stages; original monolithic UID is not executed.\n', { flag: 'wx' });
  fs.writeFileSync(path.join(context.directory, 'producer-selection.json'), JSON.stringify({ caseId: item.id, originalMethod: item.test,
    monolithicExecuted: false, childMs: 45000, setupMs: 60000, plan: spec }, null, 2), { flag: 'wx' });
  fs.writeFileSync(path.join(context.directory, 'producer-union-controls.json'), JSON.stringify(producerUnionControls(item, context), null, 2), { flag: 'wx' });
  if (item.repositoryStages || caller) fs.writeFileSync(path.join(context.directory, "repository-controls.json"), JSON.stringify(repositoryControls(payloadOwner, context), null, 2), { flag: "wx" });
  const discovered = await command(executable, [assembly, '--filter-method', spec.method, '--list-tests', 'json'], context, 'producer-discovery');
  if (discovered.code !== 0) reject('PRODUCER_DISCOVERY_FAILED', item.id);
  const discovery = JSON.parse(discovered.output), chosen = [];
  for (const stage of spec.stages) {
    const name = spec.method + '(stageId: ' + JSON.stringify(stage.id) + ')';
    const matches = discovery.tests?.filter(row => row.displayName === name);
    if (matches?.length !== 1) reject('PRODUCER_DISCOVERY_IDENTITY', stage.id);
    chosen.push({ stage, selected: matches[0] });
  }
  if (new Set(chosen.map(row => row.selected.uid)).size !== spec.stages.length) reject('PRODUCER_DISCOVERY_DUPLICATE', item.id);
  fs.writeFileSync(path.join(context.directory, 'producer-exact-uids.json'), JSON.stringify(chosen, null, 2), { flag: 'wx' });
  for (const { stage, selected } of chosen) {
    withinBudget();
    if (caller && JSON.stringify(callerBindings(item, context.root, assembly)) !== JSON.stringify(bindings)) reject('CALLER_BINARY_SOURCE_DRIFT', stage.id);
    const directory = path.join(context.directory, stage.id); fs.mkdirSync(directory);
    const child = { ...context, directory }, extra = { PHASE1_NONCE: context.nonce, PHASE1_INPUT: context.input,
      PHASE1_TOOL: context.tool, PHASE1_PLATFORM: context.platform, PHASE1_RESULT_DIRECTORY: directory,
      PHASE1_AGGREGATE_DIRECTORY: context.directory };
    if (caller) { extra.PHASE1_CASE_ID = item.id; extra.PHASE1_STAGE_CATALOG = spec.catalog; }
    if ((item.repositoryStages || caller) && ['repository-sources', 'suppression-bypass'].includes(action(stage))) {
      if (!permissionPayload || fileHash(permissionPayload.path) !== permissionPayload.sha256) reject('REPOSITORY_PAYLOAD_CHANGED', stage.id);
      verifyRepositoryPayload(payloadOwner, read(permissionPayload.path), context);
      extra.PHASE1_PERMISSION_DATA = permissionPayload.path; extra.PHASE1_PERMISSION_HASH = permissionPayload.sha256;
    }
    fs.appendFileSync(log, 'Starting ' + stage.id + ' UID ' + selected.uid + '\n');
    const timeout = producerStageTimeout(item, stage, context.platform);
    const result = await command(executable, [assembly, '--filter-uid', selected.uid, '--report-xunit-ctrf',
      '--report-xunit-ctrf-filename', 'tests.json', '--results-directory', directory, '--no-ansi', '--progress', 'off',
      '--parallel', 'none', '--explicit', 'on', '--timeout', `${timeout / 1000}s`], child, 'test', timeout, extra);
    if (result.code !== 0) reject('PRODUCER_STAGE_FAILED', stage.id);
    const receipts = fs.readdirSync(directory).filter(name => (item.repositoryStages || caller ? /^repository-progress-[0-9]+\.json$/ : /^producer-progress-[0-9]+\.json$/).test(name)).sort();
    if (!receipts.length) reject('PRODUCER_PROGRESS_MISSING', stage.id);
    const receipt = read(path.join(directory, receipts.at(-1))), report = read(path.join(directory, 'tests.json'));
    verifyProducerStage(report, selected, stage, receipt, context);
    if (caller && (receipt.caseId !== item.id || receipt.catalog !== spec.catalog)) reject('CALLER_RECEIPT_IDENTITY', stage.id);
    if ((item.repositoryStages || caller) && action(stage) === 'repository-auth') {
      const payloadPath = path.join(context.directory, stage.id, 'permission-keys.json');
      verifyRepositoryPayload(payloadOwner, read(payloadPath), context);
      permissionPayload = { path: payloadPath, sha256: fileHash(payloadPath), caseUid: selected.uid, testUid: receipt.testUid };
    }
    reconcile([item], fs.readdirSync(directory).filter(name => name.endsWith('.proof.json')).map(name => read(path.join(directory, name))), child);
    observations.push({ caseId: item.id, stageId: stage.id, status: 'pass', caseUid: selected.uid, testUid: receipt.testUid,
      cohort: stage.cohort ?? null, rowIds: stage.rowIds ?? [], details: receipt.data, nonce: receipt.nonce, input: receipt.input,
      tool: receipt.tool, platform: receipt.platform, elapsedMs: Date.parse(result.ended) - Date.parse(result.started),
      report: { path: stage.id + '/tests.json', sha256: fileHash(path.join(directory, 'tests.json')) },
      proofs: fs.readdirSync(directory).filter(name => name.endsWith('.proof.json')).map(name => ({ path: stage.id + '/' + name, sha256: fileHash(path.join(directory, name)) })),
      progress: receipts.map(name => ({ path: stage.id + '/' + name, sha256: fileHash(path.join(directory, name)) })) });
    fs.writeFileSync(path.join(context.directory, 'producer-progress.json'), JSON.stringify(observations, null, 2));
    fs.appendFileSync(log, 'Passed ' + stage.id + '\n');
    console.log('Producer stage ' + item.id + ' ' + observations.length + '/' + spec.stages.length + ': ' + stage.id);
  }
  producerPlan(item, context.root); verifyProducerUnion(item, observations, context);
  withinBudget();
  if (caller) {
    if (JSON.stringify(callerBindings(item, context.root, assembly)) !== JSON.stringify(bindings)) reject('CALLER_BINARY_SOURCE_DRIFT', item.id);
    verifyCallerExecution(item, observations, context.root);
    if (permissionPayload && fileHash(permissionPayload.path) !== permissionPayload.sha256) reject('REPOSITORY_PAYLOAD_CHANGED', item.id);
  }
  if (item.repositoryStages) {
    if (!permissionPayload || fileHash(permissionPayload.path) !== permissionPayload.sha256) reject('REPOSITORY_PAYLOAD_CHANGED', item.id);
    verifyRepositoryExecution(item, observations, context);
  }
  const persistedPayload = caller && permissionPayload ? { ...permissionPayload, path: path.relative(context.directory, permissionPayload.path).replaceAll(path.sep, '/') } : permissionPayload;
  const union = { caseId: item.id, aggregatedExecution: caller ? true : undefined, execution: caller ? 'aggregated-caller-stages' : item.repositoryStages ? 'aggregated-repository-stages' : 'aggregated-producer-stages', monolithicExecuted: false,
    status: 'pass', selected: spec.stages.length, executed: observations.length, stages: observations, permissionPayload: persistedPayload, bindings,
    aggregateMs: Date.now() - started, overallBudgetMs: caller ? spec.overallMs : undefined };
  const unionFile = path.join(context.directory, 'producer-union.json');
  fs.writeFileSync(unionFile, JSON.stringify(union, null, 2), { flag: 'wx' });
  const bounded = { id: item.id, status: 'pass', diagnostic: item.expectedDiagnostic ?? spec.diagnostic, execution: union.execution,
    location: 'Aggregated complete original predicate conjunction from ' + observations.length + ' executed stages',
    report: { path: item.id + '/producer-union.json', sha256: fileHash(unionFile) }, stages: observations };
  return caller ? finishCallerResult(item, bounded, context) : bounded;
}

async function main(root, args) {
  root = fs.realpathSync(root);
  const manifest = read(path.join(root, 'src/godot-game/tools/verification/required-cases.json'));
  const selection = select(manifest, args);
  if (selection.list) { console.log(JSON.stringify(selection.selected, null, 2)); return; }
  const nonce = crypto.randomUUID(), where = workspace.locations(root);
  workspace.assertSharedIgnored(root);
  const lock = where.locks.check;
  fs.mkdirSync(path.dirname(lock), { recursive: true });
  let lockFd;
  try { lockFd = fs.openSync(lock, 'wx'); } catch (error) {
    if (error.code === 'EEXIST') reject('CHECK_LOCKED', 'Another check owns .local/locks/check.lock; interrupted locks require inspection');
    throw error;
  }
  fs.writeFileSync(lockFd, JSON.stringify({ nonce, pid: process.pid, started: new Date().toISOString() }));
  const directory = where.runs('verification', nonce);
  workspace.ensure(directory, where.temp(), where.cliHome, where.nuget.http, where.nuget.scratch);
  const context = { root, directory, nonce, children: new Set(), commands: [], platform: `${process.platform}-${process.arch}-${os.release()}` };
  const failures = [], accepted = [];
  let before;
  const interrupted = () => {
    for (const child of context.children) killTree(child);
    fs.writeFileSync(path.join(directory, 'interrupted.json'), JSON.stringify({ nonce, status: 'interrupted' }));
    fs.closeSync(lockFd); fs.unlinkSync(lock); process.exit(130);
  };
  process.once('SIGINT', interrupted); process.once('SIGTERM', interrupted);
  try {
    before = snapshot(root, manifest); context.input = before.sha256;
    fs.writeFileSync(path.join(directory, 'inputs.json'), JSON.stringify(before, null, 2));
    for (const item of selection.selected.filter(item => item.implementation !== 'csharp')) failures.push({ id: item.id, status: 'blocked', diagnostic: 'IMPLEMENTATION_PENDING', owner: item.owner });
    const executableCases = selection.selected.filter(item => item.implementation === 'csharp');
    if (executableCases.length) {
      try {
        const pins = read(path.join(root, 'src/godot-game/tools/verification/toolchain.json'));
        if (process.versions.node !== pins.node) reject('TOOL_MISMATCH', 'Node version');
        const sdk = await command('dotnet', ['--version'], context, 'sdk');
        if (sdk.code !== 0 || sdk.output.trim() !== pins.sdk) reject('TOOL_MISMATCH', 'SDK identity');
        const sdks = await command('dotnet', ['--list-sdks'], context, 'sdk-list');
        const sdkLine = sdks.output.split(/\r?\n/).find(line => line.startsWith(pins.sdk + ' ['));
        if (!sdkLine) reject('TOOL_MISMATCH', 'SDK location');
        const compiler = path.join(sdkLine.slice(sdkLine.indexOf('[') + 1, -1), pins.sdk, 'Roslyn/bincore/csc.dll');
        const csc = await command('dotnet', [compiler, '-version'], context, 'compiler');
        if (csc.code !== 0 || csc.output.trim() !== pins.compiler) reject('TOOL_MISMATCH', 'Compiler identity');
        if (manifest.project) {
          const restore = await command('dotnet', ['restore', manifest.project, '--locked-mode', '--disable-parallel', '-p:NuGetAudit=false'], context, 'restore');
          if (restore.code !== 0) reject('RESTORE_FAILED', 'Exact locked restore');
          const build = await command('dotnet', ['build', manifest.project, '--no-restore', '-p:NuGetAudit=false'], context, 'build');
          if (build.code !== 0) reject('BUILD_FAILED', 'C# proof project');
        }
        const assembly = inside(root, manifest.assembly);
        if (!fs.existsSync(assembly)) reject('TOOL_MISSING', manifest.assembly);
        const executable = manifest.executable ? inside(root, manifest.executable) : 'dotnet';
        context.tool = digest(JSON.stringify({ node: process.versions.node, nodeHash: fileHash(process.execPath), sdk: sdk.output.trim(), compiler: csc.output.trim(), compilerHash: fileHash(compiler), assembly: fileHash(assembly), lock: manifest.project ? fileHash(path.join(root, 'src/godot-game/tests/verification/packages.lock.json')) : null }));
        for (const item of executableCases) {
          const caseDir = path.join(directory, item.id); fs.mkdirSync(caseDir);
          try {
            console.log(`Checking ${item.id}`);
            const childContext = { ...context, directory: caseDir };
            if (item.producerStages || item.repositoryStages || item.callerStages) { accepted.push(await runProducerStages(item, childContext, executable, assembly)); continue; }
            const timeout = caseTimeout(item, manifest);
            const result = await command(executable, [assembly, '--filter-method', item.test, '--report-xunit-ctrf', '--report-xunit-ctrf-filename', 'tests.json', '--results-directory', caseDir,
              '--no-ansi', '--progress', 'off', '--parallel', 'none', '--explicit', 'on', '--timeout', timeout.argument], childContext, 'test', timeout.outerMs,
            { PHASE1_NONCE: nonce, PHASE1_INPUT: context.input, PHASE1_TOOL: context.tool, PHASE1_PLATFORM: context.platform, PHASE1_RESULT_DIRECTORY: caseDir });
            const reportFile = path.join(caseDir, 'tests.json');
            if (!fs.existsSync(reportFile)) reject('REPORT_MISSING', item.id + ': child exit ' + result.code);
            verifyReport(read(reportFile), [item]);
            if (result.code !== 0) reject('CHILD_FAILED', item.id + ': exit ' + result.code);
            const results = fs.readdirSync(caseDir).filter(name => name.endsWith('.proof.json')).map(name => read(path.join(caseDir, name)));
            accepted.push(...reconcile([item], results, { ...context, directory: caseDir }).map(row => ({ ...row, report: { path: item.id + '/tests.json', sha256: fileHash(reportFile) } })));
          } catch (error) { failures.push({ id: item.id, status: 'fail', diagnostic: error.diagnostic ?? 'CHECK_ERROR', message: error.message, log: `${item.id}/test.process.log` }); }
        }
      } catch (error) {
        for (const item of executableCases) failures.push({ id: item.id, status: 'blocked', diagnostic: error.diagnostic ?? 'INFRASTRUCTURE_FAILED', message: error.message });
      }
    }
    if (snapshot(root, manifest).sha256 !== before.sha256) failures.push({ id: 'inputs', status: 'fail', diagnostic: 'SOURCE_CHANGED', message: 'Approved source/documents changed during verification' });
  } catch (error) { failures.push({ id: 'inputs', status: 'fail', diagnostic: error.diagnostic ?? 'INPUT_ERROR', message: error.message }); }
  finally {
    for (const child of context.children) killTree(child);
    process.removeListener('SIGINT', interrupted); process.removeListener('SIGTERM', interrupted);
    // Keep the lock through atomic publication; a competing invocation cannot race it.
  }
  const status = failures.length || accepted.length !== selection.selected.length ? 'fail' : 'pass';
  const summary = { schemaVersion: 1, nonce, status, selected: selection.selected.map(row => row.id), input: context.input, tool: context.tool, platform: context.platform,
    revision: spawnSync('git', ['rev-parse', 'HEAD'], { cwd: root, encoding: 'utf8', windowsHide: true }).stdout?.trim() || 'fixture-without-git',
    deferred: selection.deferred.map(row => ({ id: row.id, acceptancePhase: row.acceptancePhase, owner: row.owner })),
    results: accepted, failures, commands: context.commands };
  const temporary = path.join(directory, 'summary.json.tmp');
  fs.writeFileSync(temporary, JSON.stringify(summary, null, 2));
  fs.renameSync(temporary, path.join(directory, 'summary.json'));
  fs.closeSync(lockFd); fs.unlinkSync(lock);
  for (const failure of failures) console.error(`${failure.status.toUpperCase()} ${failure.id}: ${failure.diagnostic}${failure.message ? ' — ' + failure.message : ''}${failure.log ? ' [log](' + path.relative(root, path.join(directory, failure.log)).replaceAll('\\', '/') + ')' : ''}`);
  for (const item of selection.deferred) console.log(`DEFERRED ${item.id}: Phase ${item.acceptancePhase}; owner ${item.owner}`);
  console.log(`${status.toUpperCase()} ${accepted.length}/${selection.selected.length} selected obligations; [summary](${path.relative(root, path.join(directory, 'summary.json')).replaceAll('\\', '/')})`);
  if (status !== 'pass') process.exitCode = 1;
}
module.exports = { reconcile, select, snapshot, verifyReport, inside, caseTimeout, producerPlan, verifyProducerUnion, producerUnionControls, repositoryPlan, repositoryControls, verifyRepositoryPayload, verifyRepositorySources,
  callerPlan, callerBindings, verifyCallerResult, callerEvidenceControls, runProducerStages, command, killTree, main };
if (require.main === module) main(path.resolve(__dirname, '../../../..'), process.argv.slice(2)).catch(error => {
  console.error(`${error.diagnostic ?? 'CHECK_ERROR'}: ${error.message}`); process.exitCode = 1;
});
