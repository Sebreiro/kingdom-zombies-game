'use strict';

// Acceptance comes from the independent manifest, never the discovered test count.
const fs = require('node:fs');
const path = require('node:path');
const os = require('node:os');
const crypto = require('node:crypto');
const { spawn, spawnSync } = require('node:child_process');
const { parseJson } = require('./bootstrap.cjs');
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
  const home = path.join(root, '.phase1');
  return { ...process.env, DOTNET_CLI_HOME: path.join(home, 'cli-home'),
    NUGET_PACKAGES: process.env.PHASE1_PACKAGE_CACHE ?? path.join(home, 'packages'),
    NUGET_HTTP_CACHE_PATH: path.join(home, 'http-cache'), TEMP: path.join(home, 'temp'), TMP: path.join(home, 'temp'), TMPDIR: path.join(home, 'temp'),
    DOTNET_CLI_TELEMETRY_OPTOUT: '1', TESTINGPLATFORM_TELEMETRY_OPTOUT: '1', DOTNET_NOLOGO: '1', DOTNET_SKIP_FIRST_TIME_EXPERIENCE: '1',
    PHASE1_ROOT: root, PHASE1_NODE: process.execPath };
}
async function command(executable, args, context, label, timeoutMs = 60_000, extra = {}) {
  const log = path.join(context.directory, label + '.process.log'), fd = fs.openSync(log, 'wx');
  let child, timer, timedOut = false, spawnError;
  const started = new Date().toISOString();
  const result = await new Promise(resolve => {
    child = spawn(executable, args, { cwd: context.root, env: { ...environment(context.root), ...extra }, windowsHide: true,
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
async function main(root, args) {
  root = fs.realpathSync(root);
  const manifest = read(path.join(root, 'tools/phase1/required-cases.json'));
  const selection = select(manifest, args);
  if (selection.list) { console.log(JSON.stringify(selection.selected, null, 2)); return; }
  const nonce = crypto.randomUUID(), home = path.join(root, '.phase1');
  fs.mkdirSync(home, { recursive: true });
  const lock = path.join(home, 'check.lock');
  let lockFd;
  try { lockFd = fs.openSync(lock, 'wx'); } catch (error) {
    if (error.code === 'EEXIST') reject('CHECK_LOCKED', 'Another check owns .phase1/check.lock; interrupted locks require inspection');
    throw error;
  }
  fs.writeFileSync(lockFd, JSON.stringify({ nonce, pid: process.pid, started: new Date().toISOString() }));
  const directory = path.join(home, 'runs', nonce);
  fs.mkdirSync(directory, { recursive: true });
  for (const name of ['temp', 'cli-home', 'http-cache']) fs.mkdirSync(path.join(home, name), { recursive: true });
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
        const pins = read(path.join(root, 'tools/phase1/toolchain.json'));
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
        context.tool = digest(JSON.stringify({ node: process.versions.node, nodeHash: fileHash(process.execPath), sdk: sdk.output.trim(), compiler: csc.output.trim(), compilerHash: fileHash(compiler), assembly: fileHash(assembly), lock: manifest.project ? fileHash(path.join(root, 'tests/phase1/packages.lock.json')) : null }));
        for (const item of executableCases) {
          const caseDir = path.join(directory, item.id); fs.mkdirSync(caseDir);
          try {
            console.log(`Checking ${item.id}`);
            const childContext = { ...context, directory: caseDir };
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
module.exports = { reconcile, select, snapshot, verifyReport, inside, caseTimeout, main };
if (require.main === module) main(path.resolve(__dirname, '../..'), process.argv.slice(2)).catch(error => {
  console.error(`${error.diagnostic ?? 'CHECK_ERROR'}: ${error.message}`); process.exitCode = 1;
});
