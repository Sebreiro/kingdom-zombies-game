'use strict';
// Gates for rebinding work: each gate collects facts fail-closed, then a pure judge decides PASS or FAIL.
//   node src/godot-game/tools/verification/rebind-gates.cjs --base <revision> [--keep a,b] [--allow path[=N],...]
//     [--scope a,b] [--pattern <regex>] [--repo <directory>] <gate>...
// residue requires =N on every --allow entry: the exact number of matching lines each allowed path keeps.
// Gates: history newhost residue lists tests roots build assets ac08 docs. Each prints "PASS <gate> <counts>" or
// "FAIL <gate>: <reason>"; the exit code is 1 when any gate fails.
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const crypto = require('node:crypto');
const {spawnSync} = require('node:child_process');

const ROOT = path.resolve(__dirname, '../../../..');
const WORKSPACE = 'src/godot-game/';
const TOOLS = WORKSPACE + 'tools/';
const HISTORY_ROOTS = ['.planning/phases/', '.planning/quick/', '.planning/debug/', 'docs/code/provenance/'];
const DEFAULT_PATTERN = '(?<![A-Za-z0-9_)\\]])\\.phase[0-9](?![0-9A-Za-z_.])';
const RUNNERS = ['verification', 'gameplay-checks', 'lan-checks', 'asset-pipeline'];
const REPO_GATES = new Set(['history', 'newhost', 'residue']);
const VERIFICATION_CATALOG = TOOLS + 'verification/required-cases.json';
const CALLER_CATALOG = TOOLS + 'gameplay-checks/required-cases.json';
const ASSET_CATALOG = TOOLS + 'asset-pipeline/required-cases.json';
const EXPECTED_MODULES = 423;
const EXPECTED_STAGED = 9;
const EXPECTED_CALLERS = 6;
const TEST_GLOB = ':(glob)' + TOOLS + '**/*.test.cjs';
// Machine-specific path terms: a drive path, a Git Bash user or work folder, the per-user application data folder, a
// user folder under any user name, a Linux home folder, or a UNC share (two backslashes, a server, a share). The
// coarse expression selects candidate lines; the fine one decides. Only lines new since --base count, so a wider term
// cannot fail on history.
const HOST_COARSE = '[A-Za-z]:[/\\\\][A-Za-z]|/[a-z]/(Users|Work)|App[D]ata|Users[/\\\\][A-Za-z0-9]|/home/[A-Za-z0-9._-]+/'
  + '|\\\\\\\\[A-Za-z0-9][^\\\\[:space:]]*\\\\[A-Za-z0-9]';
const HOST = new RegExp('(^|[^A-Za-z0-9_])[A-Za-z]:[/\\\\][A-Za-z]|\\/[a-z]\\/(Users|Work)\\b|App[D]ata|Users[/\\\\][A-Za-z0-9]'
  + '|\\/home\\/[A-Za-z0-9._-]+\\/|\\\\\\\\[A-Za-z0-9][^\\\\\\s]*\\\\[A-Za-z0-9]');

class GateFailure extends Error {}

function sha256(bytes) {
  return crypto.createHash('sha256').update(bytes).digest('hex');
}

function fail(reason) {
  return {ok: false, reason};
}

function pass(summary, details = []) {
  return {ok: true, summary, details};
}

function git(root, args, ok = [0]) {
  const result = spawnSync('git', args, {cwd: root, encoding: 'utf8', maxBuffer: 1 << 30, windowsHide: true});
  if (result.error) throw new GateFailure('git ' + args[0] + ' could not start: ' + result.error.message);
  if (!ok.includes(result.status))
    throw new GateFailure('git ' + args.join(' ').slice(0, 200) + ' exit ' + result.status + ': ' + String(result.stderr).trim().slice(0, 300));
  return result.stdout;
}

/** Runs a child process; a child that cannot start is a collection failure, never a PASS. */
function child(executable, args, options) {
  const result = spawnSync(executable, args, {encoding: 'utf8', maxBuffer: 1 << 30, windowsHide: true, ...options});
  if (result.error) throw new GateFailure(path.basename(executable) + ' could not start: ' + result.error.message);
  return {exit: result.status, output: (result.stdout || '') + (result.stderr || ''), stdout: result.stdout || ''};
}

function tree(root, revision) {
  const map = new Map();
  for (const line of git(root, ['ls-tree', '-r', '-z', revision]).split('\0')) {
    if (!line) continue;
    const tab = line.indexOf('\t');
    map.set(line.slice(tab + 1), line.slice(0, tab).split(' ')[2]);
  }
  return map;
}

function show(root, revision, file) {
  const result = spawnSync('git', ['show', revision + ':' + file], {cwd: root, encoding: 'utf8', maxBuffer: 1 << 30, windowsHide: true});
  if (result.error) throw new GateFailure('git show could not start');
  return result.status === 0 ? result.stdout : null;
}

function readJson(root, file) {
  const target = path.join(root, file);
  if (!fs.existsSync(target)) throw new GateFailure('missing working-tree file ' + file);
  try { return JSON.parse(fs.readFileSync(target, 'utf8')); } catch (error) { throw new GateFailure('unparseable ' + file + ': ' + error.message); }
}

function parseJson(text, label) {
  try { return JSON.parse(text); } catch (error) { throw new GateFailure('unparseable ' + label + ': ' + error.message); }
}

/** A plain pathspec names a path when it equals it or is one of its parent folders. */
function matches(file, spec) {
  const plain = spec.replace(/\/+$/, '');
  return file === plain || file.startsWith(plain + '/');
}

function historical(file) {
  return HISTORY_ROOTS.some(prefix => file.startsWith(prefix));
}

function unknownSpecs(paths, specs) {
  const list = [...paths];
  return specs.filter(spec => !list.some(file => matches(file, spec)));
}

function samePath(left, right) {
  if (typeof left !== 'string' || typeof right !== 'string') return false;
  const normal = value => {
    const resolved = path.resolve(value);
    return process.platform === 'win32' ? resolved.toLowerCase() : resolved;
  };
  return normal(left) === normal(right);
}

function realOrResolved(value) {
  try { return fs.realpathSync.native(value); } catch { return path.resolve(value); }
}

function tail(text, length = 600) {
  return String(text || '').slice(-length).replace(/\s+/g, ' ').trim();
}

// ---- history ------------------------------------------------------------------------------------------------------

function collectHistory(context) {
  const base = tree(context.root, context.base), head = tree(context.root, 'HEAD');
  const unknownKeep = unknownSpecs(base.keys(), context.keep);
  const records = [...base].filter(([file]) => historical(file) || context.keep.some(spec => matches(file, spec)))
    .map(([file, blob]) => ({path: file, base: blob, head: head.get(file) ?? null}));
  return {unknownKeep, records};
}

function judgeHistory(facts) {
  if (facts.unknownKeep.length) return fail('unknown pathspec --keep ' + facts.unknownKeep.join(','));
  if (!facts.records.length) return fail('no history record or --keep path at base');
  const deleted = facts.records.filter(record => record.head === null).map(record => record.path);
  const changed = facts.records.filter(record => record.head !== null && record.head !== record.base).map(record => record.path);
  if (deleted.length || changed.length)
    return fail((deleted.length + changed.length) + ' record(s) differ from base: changed ' + (changed.join(' ') || 'none')
      + '; deleted ' + (deleted.join(' ') || 'none'));
  return pass('history ' + facts.records.length + ' records unchanged');
}

// ---- newhost ------------------------------------------------------------------------------------------------------

function hostLines(root, revision) {
  const output = git(root, ['grep', '-I', '-n', '-E', HOST_COARSE, revision], [0, 1]);
  return output.split('\n').filter(Boolean).map(line => /^[^:]+:(.+?):(\d+):(.*)$/.exec(line))
    .filter(match => match && HOST.test(match[3])).map(match => ({at: match[1] + ':' + match[2], text: match[3]}));
}

function collectNewhost(context) {
  return {baseTexts: hostLines(context.root, context.base).map(line => line.text), headLines: hostLines(context.root, 'HEAD')};
}

function judgeNewhost(facts) {
  const old = new Set(facts.baseTexts);
  const added = facts.headLines.filter(line => !old.has(line.text));
  if (added.length) return fail(added.length + ' new machine-path line(s): ' + added.slice(0, 12).map(line => line.at).join(' '));
  return pass('newhost 0 new of ' + facts.headLines.length + ' machine-path lines');
}

// ---- residue ------------------------------------------------------------------------------------------------------

function collectResidue(context) {
  const head = tree(context.root, 'HEAD').keys(), base = tree(context.root, context.base).keys();
  const headPaths = [...head];
  const unknown = [
    ...unknownSpecs(headPaths, context.scope).map(spec => '--scope ' + spec),
    ...unknownSpecs(headPaths, context.allow.map(entry => entry.spec)).map(spec => '--allow ' + spec),
    ...unknownSpecs(base, context.keep).map(spec => '--keep ' + spec)
  ];
  const specs = context.scope.length ? ['--', ...context.scope] : [];
  const output = git(context.root, ['grep', '-n', '-I', '-P', context.pattern, 'HEAD', ...specs], [0, 1]);
  const hits = output.split('\n').filter(Boolean).map(line => {
    const match = /^[^:]+:(.+?):(\d+):/.exec(line);
    if (!match) throw new GateFailure('unparseable git grep line ' + line.slice(0, 200));
    return {path: match[1], line: Number(match[2])};
  });
  return {unknown, hits};
}

function judgeResidue(facts, options) {
  if (facts.unknown.length) return fail('unknown pathspec ' + facts.unknown.join(' '));
  // An uncounted entry would let its files gain any number of matching lines; every entry names its exact count.
  const uncounted = options.allow.filter(entry => entry.count === null).map(entry => entry.spec);
  if (uncounted.length) return fail(uncounted.length + ' --allow entry without =N: ' + uncounted.join(' '));
  const counted = facts.hits.filter(hit => !historical(hit.path) && !options.keep.some(spec => matches(hit.path, spec)));
  const perFile = new Map();
  for (const hit of counted) perFile.set(hit.path, (perFile.get(hit.path) || 0) + 1);
  const details = [...perFile].map(([file, count]) => 'residue ' + file + ' ' + count);
  const outside = [...perFile.keys()].filter(file => !options.allow.some(entry => matches(file, entry.spec)));
  const counts = options.allow.filter(entry => entry.count !== null).map(entry => {
    const kept = counted.filter(hit => matches(hit.path, entry.spec)).length;
    return kept === entry.count ? null : entry.spec + ' retains ' + kept + ' line(s), expected ' + entry.count;
  }).filter(Boolean);
  const reasons = [];
  if (outside.length) reasons.push(outside.length + ' file(s) outside --allow: ' + outside.map(file => file + '=' + perFile.get(file)).join(' '));
  if (counts.length) reasons.push(counts.join('; '));
  if (reasons.length) return {...fail(reasons.join('; ')), details};
  return pass('residue ' + counted.length + ' allowed line(s) in ' + perFile.size + ' file(s)', details);
}

// ---- lists --------------------------------------------------------------------------------------------------------

function caseCount(text, label) {
  const catalog = parseJson(text, label);
  if (!Array.isArray(catalog.cases)) throw new GateFailure(label + ' has no cases array');
  return catalog.cases.length;
}

function collectLists(context) {
  return RUNNERS.map(runner => {
    const catalog = TOOLS + runner + '/required-cases.json';
    const run = child(process.execPath, [path.join(context.root, TOOLS, runner, 'check.cjs'), '--list'], {cwd: os.tmpdir()});
    const listed = run.exit === 0 ? parseJson(run.stdout, runner + ' --list output') : null;
    if (listed !== null && !Array.isArray(listed)) throw new GateFailure(runner + ' --list output is not an array');
    const baseText = show(context.root, context.base, catalog);
    return {
      runner, exit: run.exit, error: run.exit === 0 ? '' : tail(run.output, 300), listed: listed && listed.length,
      catalog: caseCount(fs.readFileSync(path.join(context.root, catalog), 'utf8'), catalog),
      base: baseText === null ? null : caseCount(baseText, context.base + ':' + catalog)
    };
  });
}

function judgeLists(facts) {
  if (!facts.length) return fail('no runner listed');
  for (const row of facts) {
    if (row.exit !== 0) return fail(row.runner + ' --list exit ' + row.exit + ' ' + row.error);
    if (row.base === null) return fail(row.runner + ' base catalog absent');
    if (row.listed !== row.catalog) return fail(row.runner + ' lists ' + row.listed + ', catalog has ' + row.catalog);
    if (row.catalog !== row.base) return fail(row.runner + ' catalog has ' + row.catalog + ', base has ' + row.base);
  }
  return pass('lists ' + facts.map(row => row.runner + '=' + row.listed).join(' '));
}

// ---- tests --------------------------------------------------------------------------------------------------------

function collectTests(context) {
  const files = git(context.root, ['ls-files', '-z', '--', TEST_GLOB]).split('\0').filter(Boolean);
  if (!files.length) return {files, exit: null, pass: null, fail: null, output: ''};
  const run = child(process.execPath, ['--test', '--test-reporter=tap', ...files], {cwd: context.root});
  const passed = /^# pass (\d+)$/m.exec(run.stdout), failed = /^# fail (\d+)$/m.exec(run.stdout);
  return {files, exit: run.exit, pass: passed ? Number(passed[1]) : null, fail: failed ? Number(failed[1]) : null, output: tail(run.output)};
}

function judgeTests(facts) {
  if (!facts.files.length) return fail('no test file found');
  if (facts.pass === null || facts.fail === null) return fail('unparseable test summary: ' + facts.output);
  if (facts.exit !== 0 || facts.fail !== 0) return fail('exit ' + facts.exit + ', fail ' + facts.fail + ': ' + facts.output);
  if (facts.pass === 0) return fail('pass 0');
  return pass('tests ' + facts.pass + ' passed in ' + facts.files.length + ' files');
}

// ---- roots --------------------------------------------------------------------------------------------------------

function printRoots(context, extra) {
  const script = path.join(context.root, TOOLS, 'workspace-paths.cjs');
  const run = child(process.execPath, [script, '--print-roots', ...extra], {cwd: os.tmpdir()});
  if (run.exit !== 0) throw new GateFailure('workspace-paths.cjs --print-roots ' + extra.join(' ') + ' exit ' + run.exit + ': ' + tail(run.output, 300));
  return parseJson(run.stdout, 'workspace-paths.cjs --print-roots output');
}

function collectRoots(context) {
  const common = git(context.root, ['rev-parse', '--path-format=absolute', '--git-common-dir']).trim();
  const gitDir = git(context.root, ['rev-parse', '--path-format=absolute', '--git-dir']).trim();
  const main = path.dirname(common);
  return {
    checkout: context.root, common, main, override: process.env.KZ_SHARED_LOCAL || null,
    linked: !samePath(realOrResolved(gitDir), realOrResolved(common)),
    printed: printRoots(context, []), printedMain: printRoots(context, ['--root', main])
  };
}

function judgeRoots(facts) {
  const expectedShared = facts.override || path.join(facts.main, '.local');
  const printed = facts.printed, main = facts.printedMain;
  if (!samePath(printed.local, path.join(facts.checkout, '.local'))) return fail('local ' + printed.local + ' is not the checkout .local');
  if (!samePath(printed.shared, expectedShared))
    return fail('shared ' + printed.shared + ' is not ' + (facts.override ? 'KZ_SHARED_LOCAL' : 'the .local beside the git common dir'));
  if (facts.linked && samePath(printed.shared, printed.local)) return fail('shared equals local from a linked worktree');
  if (!samePath(main.local, path.join(facts.main, '.local'))) return fail('main checkout local ' + main.local);
  if (!samePath(main.shared, expectedShared)) return fail('main checkout shared ' + main.shared);
  if (!facts.override && !samePath(main.shared, main.local)) return fail('main checkout shared differs from its local');
  return pass('roots source=' + printed.source + ' linked=' + facts.linked + ' override=' + Boolean(facts.override));
}

// ---- build --------------------------------------------------------------------------------------------------------

function workspacePaths(root) {
  const file = path.join(root, TOOLS, 'workspace-paths.cjs');
  if (!fs.existsSync(file)) throw new GateFailure('workspace-paths.cjs is missing');
  return require(file);
}

function collectBuild(context) {
  const paths = workspacePaths(context.root);
  const env = {...paths.isolationEnv(context.root), DOTNET_CLI_TELEMETRY_OPTOUT: '1', DOTNET_NOLOGO: '1', DOTNET_SKIP_FIRST_TIME_EXPERIENCE: '1'};
  const logs = paths.locations(context.root).temp('rebind-gates');
  fs.mkdirSync(logs, {recursive: true});
  const run = (args, name) => {
    const result = child('dotnet', args, {cwd: path.join(context.root, WORKSPACE), env, timeout: 30 * 60 * 1000});
    fs.writeFileSync(path.join(logs, name), result.output);
    return {exit: result.exit, log: result.output};
  };
  const restore = run(['restore', 'KingdomZombies.slnx', '--locked-mode', '--disable-parallel', '-p:NuGetAudit=false', '-v:n', '-tl:off'], 'restore.log');
  const build = restore.exit === 0
    ? run(['build', 'KingdomZombies.slnx', '--no-restore', '--disable-build-servers', '-p:UseSharedCompilation=false', '-nodeReuse:false',
      '-p:NuGetAudit=false', '-tl:off'], 'build.log')
    : null;
  return {restore, build, logs};
}

function judgeBuild(facts) {
  if (facts.restore.exit !== 0) return fail('restore exit ' + facts.restore.exit + ': ' + tail(facts.restore.log, 300));
  const get = /\bGET https?:\S*/.exec(facts.restore.log);
  if (get) return fail('restore used the network: ' + get[0]);
  if (!facts.build) return fail('build did not run');
  const warnings = /(\d+) Warning\(s\)/.exec(facts.build.log), errors = /(\d+) Error\(s\)/.exec(facts.build.log);
  if (!warnings || !errors) return fail('build summary line missing (exit ' + facts.build.exit + ')');
  if (facts.build.exit !== 0 || warnings[1] !== '0' || errors[1] !== '0')
    return fail('build exit ' + facts.build.exit + ', ' + warnings[1] + ' Warning(s), ' + errors[1] + ' Error(s)');
  return pass('build restore locked without network, 0 Warning(s), 0 Error(s)');
}

// ---- assets -------------------------------------------------------------------------------------------------------

function moduleCount(catalog) {
  let count = 0;
  (function walk(value) {
    if (!value || typeof value !== 'object') return;
    if (typeof value.module === 'string' && typeof value.moduleSha256 === 'string') count++;
    for (const key of Object.keys(value)) walk(value[key]);
  })(catalog);
  return count;
}

function collectAssets(context) {
  const run = child(process.execPath, [path.join(context.root, TOOLS, 'verification/asset-bindings.cjs'), '--check'], {cwd: context.root});
  const baseText = show(context.root, context.base, ASSET_CATALOG);
  return {
    exit: run.exit, output: tail(run.output), modules: moduleCount(readJson(context.root, ASSET_CATALOG)),
    baseModules: baseText === null ? null : moduleCount(parseJson(baseText, context.base + ':' + ASSET_CATALOG))
  };
}

function judgeAssets(facts) {
  if (facts.baseModules === null) return fail('base asset catalog absent');
  if (facts.exit !== 0) return fail('asset-bindings --check exit ' + facts.exit + ': ' + facts.output);
  if (facts.modules !== facts.baseModules) return fail(facts.modules + ' module bindings, base has ' + facts.baseModules);
  if (facts.modules !== EXPECTED_MODULES) return fail(facts.modules + ' module bindings, expected ' + EXPECTED_MODULES);
  return pass('assets modules=' + facts.modules + ' base=' + facts.baseModules + ' check passed');
}

// ---- ac08 ---------------------------------------------------------------------------------------------------------

function fileSha(root, file) {
  const target = path.join(root, file);
  return fs.existsSync(target) && fs.statSync(target).isFile() ? sha256(fs.readFileSync(target)) : null;
}

function collectAc08(context) {
  const run = child(process.execPath, [path.join(context.root, TOOLS, 'verification/ac08-bindings.cjs'), '--check', '--rev', 'HEAD'], {cwd: context.root});
  const catalogs = [VERIFICATION_CATALOG, CALLER_CATALOG];
  const dirtyCatalogs = catalogs.filter(file => show(context.root, 'HEAD', file) !== fs.readFileSync(path.join(context.root, file), 'utf8'));
  const [primary, caller] = catalogs.map(file => readJson(context.root, file));
  const item = (primary.cases || []).find(row => row.id === 'ac-08.complete');
  if (!item || !item.repositoryStages) throw new GateFailure('ac-08.complete has no repositoryStages');
  const stages = item.repositoryStages;
  const disk = stages.sources.filter(row => row.sha256 !== null && fileSha(context.root, row.path) !== row.sha256).map(row => row.path);
  const authority = primary.callerAuthority
    ? {path: primary.callerAuthority.registry, declared: primary.callerAuthority.sha256, actual: fileSha(context.root, primary.callerAuthority.registry)}
    : null;
  const all = [...primary.cases, ...caller.cases];
  const registries = all.filter(row => row.producerStages).flatMap(row => Object.values(row.producerStages.registries || {})
    .map(entry => ({case: row.id, path: entry.path, declared: entry.sha256, actual: fileSha(context.root, entry.path)})));
  const callers = all.filter(row => row.callerStages).map(row => ({id: row.id, declared: row.callerStages.repositorySha256}));
  const runner = require(path.join(context.root, TOOLS, 'verification/check.cjs'));
  const plans = all.filter(row => row.repositoryStages || row.callerStages || row.producerStages).map(row => {
    try { runner.producerPlan(row, context.root); return {id: row.id, ok: true}; } catch (error) {
      return {id: row.id, ok: false, error: String(error.diagnostic || error.message).slice(0, 200)};
    }
  });
  return {
    checkExit: run.exit, checkOutput: tail(run.output), dirtyCatalogs, disk, authority, registries, callers,
    expectedRepositorySha256: sha256(JSON.stringify(stages)), plans
  };
}

function judgeAc08(facts) {
  if (facts.checkExit !== 0) return fail('ac08-bindings --check --rev HEAD exit ' + facts.checkExit + ': ' + facts.checkOutput);
  if (facts.dirtyCatalogs.length) return fail('working-tree catalog differs from HEAD: ' + facts.dirtyCatalogs.join(' '));
  if (facts.disk.length) return fail('hashed rows differ from the working tree: ' + facts.disk.slice(0, 8).join(' '));
  if (!facts.authority || facts.authority.actual !== facts.authority.declared)
    return fail('stale callerAuthority ' + (facts.authority && facts.authority.path));
  const registries = facts.registries.filter(entry => entry.actual !== entry.declared);
  if (!facts.registries.length) return fail('no producer registry');
  if (registries.length) return fail('producer registry sha256 differs from the file: ' + registries.map(entry => entry.case + ' ' + entry.path).join(' '));
  const callers = facts.callers.filter(entry => entry.declared !== facts.expectedRepositorySha256);
  if (facts.callers.length !== EXPECTED_CALLERS) return fail(facts.callers.length + ' caller hashes, expected ' + EXPECTED_CALLERS);
  if (callers.length) return fail('stale caller repositorySha256: ' + callers.map(entry => entry.id).join(' '));
  const refused = facts.plans.filter(plan => !plan.ok);
  if (refused.length) return fail('producerPlan refused ' + refused.map(plan => plan.id + ' ' + plan.error).join(' | '));
  if (facts.plans.length !== EXPECTED_STAGED) return fail(facts.plans.length + ' staged plans, expected ' + EXPECTED_STAGED);
  return pass('ac08 current, callers=' + facts.callers.length + ' staged=' + facts.plans.length + ' registries=' + facts.registries.length);
}

// ---- docs ---------------------------------------------------------------------------------------------------------

function docsCount(text, label) {
  const catalog = parseJson(text, label);
  if (!Array.isArray(catalog.cases)) throw new GateFailure(label + ' has no cases array');
  return catalog.cases.filter(row => row.group === 'docs').length;
}

/** The counts that must hold before the docs runner may launch; null when it may launch. */
function docsPrecondition(facts) {
  if (facts.nBase === null) return 'base catalog absent at ' + facts.base;
  if (facts.nHead !== facts.nBase) return 'head docs count ' + facts.nHead + ' differs from base ' + facts.nBase;
  if (facts.nBase === 0) return 'base has no docs case';
  return null;
}

function collectDocs(context) {
  const baseText = show(context.root, context.base, VERIFICATION_CATALOG);
  const facts = {
    base: context.base, nBase: baseText === null ? null : docsCount(baseText, context.base + ':' + VERIFICATION_CATALOG),
    nHead: docsCount(fs.readFileSync(path.join(context.root, VERIFICATION_CATALOG), 'utf8'), VERIFICATION_CATALOG),
    launched: false, exit: null, stdout: ''
  };
  if (docsPrecondition(facts) !== null) return facts;
  const run = child(process.execPath, [path.join(context.root, TOOLS, 'verification/check.cjs'), '--group', 'docs'], {cwd: context.root});
  return {...facts, launched: true, exit: run.exit, stdout: run.stdout, output: tail(run.output)};
}

function judgeDocs(facts) {
  const blocked = docsPrecondition(facts);
  if (blocked !== null) return fail(blocked);
  if (!facts.launched) return fail('runner did not launch');
  if (facts.exit !== 0) return fail('check.cjs --group docs exit ' + facts.exit + ': ' + (facts.output || ''));
  const line = new RegExp('^PASS ' + facts.nBase + '/' + facts.nBase + ' selected obligations(.*)$', 'm').exec(facts.stdout);
  if (!line) return fail('summary line PASS ' + facts.nBase + '/' + facts.nBase + ' selected obligations missing');
  const summary = /\]\(([^)]+)\)/.exec(line[1]);
  return pass('docs N_base=' + facts.nBase + ' N_head=' + facts.nHead + ' PASS ' + facts.nBase + '/' + facts.nBase
    + ' summary=' + (summary ? summary[1] : 'unreported'));
}

// ---- driver -------------------------------------------------------------------------------------------------------

const GATES = {
  history: {collect: collectHistory, judge: judgeHistory},
  newhost: {collect: collectNewhost, judge: judgeNewhost},
  residue: {collect: collectResidue, judge: judgeResidue},
  lists: {collect: collectLists, judge: judgeLists},
  tests: {collect: collectTests, judge: judgeTests},
  roots: {collect: collectRoots, judge: judgeRoots},
  build: {collect: collectBuild, judge: judgeBuild},
  assets: {collect: collectAssets, judge: judgeAssets},
  ac08: {collect: collectAc08, judge: judgeAc08},
  docs: {collect: collectDocs, judge: judgeDocs}
};

function list(value) {
  return value.split(',').map(entry => entry.trim()).filter(Boolean);
}

function parseArgs(argv) {
  const options = {base: null, keep: [], allow: [], scope: [], pattern: DEFAULT_PATTERN, repo: null, gates: []};
  for (let index = 0; index < argv.length; index++) {
    const argument = argv[index], value = argv[index + 1];
    const takes = ['--base', '--keep', '--allow', '--scope', '--pattern', '--repo'].includes(argument);
    if (takes && value === undefined) throw new GateFailure(argument + ' needs a value');
    if (argument === '--base') options.base = argv[++index];
    else if (argument === '--keep') options.keep.push(...list(argv[++index]));
    else if (argument === '--scope') options.scope.push(...list(argv[++index]));
    else if (argument === '--pattern') options.pattern = argv[++index];
    else if (argument === '--repo') options.repo = argv[++index];
    else if (argument === '--allow') {
      for (const entry of list(argv[++index])) {
        const match = /^(.+?)(?:=(\d+))?$/.exec(entry);
        options.allow.push({spec: match[1], count: match[2] === undefined ? null : Number(match[2])});
      }
    } else if (Object.hasOwn(GATES, argument)) options.gates.push(argument);
    else throw new GateFailure('unknown argument ' + argument + '; gates: ' + Object.keys(GATES).join(' '));
  }
  if (!options.base) throw new GateFailure('--base <revision> is required');
  if (!options.gates.length) throw new GateFailure('name at least one gate: ' + Object.keys(GATES).join(' '));
  return options;
}

function runGate(name, context) {
  if (context.repoSet && !REPO_GATES.has(name)) return fail('--repo unsupported');
  try {
    const facts = GATES[name].collect(context);
    return GATES[name].judge(facts, context);
  } catch (error) {
    return fail(error instanceof GateFailure ? error.message : 'collection error: ' + error.message);
  }
}

function main(argv) {
  let options;
  try { options = parseArgs(argv); } catch (error) {
    console.error('FAIL usage: ' + error.message);
    return 1;
  }
  const root = options.repo ? path.resolve(options.repo) : ROOT;
  let base;
  try { base = git(root, ['rev-parse', '--verify', options.base + '^{commit}']).trim(); } catch (error) {
    for (const name of options.gates) console.error('FAIL ' + name + ': base ' + options.base + ' unresolved: ' + error.message);
    return 1;
  }
  const context = {...options, root, base, repoSet: Boolean(options.repo)};
  let failed = 0;
  for (const name of options.gates) {
    const verdict = runGate(name, context);
    for (const line of verdict.details || []) console.log(line);
    if (verdict.ok) console.log('PASS ' + verdict.summary);
    else { failed++; console.error('FAIL ' + name + ': ' + verdict.reason); }
  }
  return failed ? 1 : 0;
}

module.exports = {
  DEFAULT_PATTERN, HISTORY_ROOTS, EXPECTED_MODULES, EXPECTED_STAGED, EXPECTED_CALLERS, HOST, GateFailure, GATES, matches,
  parseArgs, runGate, judgeHistory, judgeNewhost, judgeResidue, judgeLists, judgeTests, judgeRoots, judgeBuild, judgeAssets,
  judgeAc08, judgeDocs, docsPrecondition, main
};

if (require.main === module) process.exitCode = main(process.argv.slice(2));
