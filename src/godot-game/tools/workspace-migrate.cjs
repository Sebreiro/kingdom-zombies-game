'use strict';
// One-time migration of the main checkout's legacy per-phase folders into the shared `.local/` root.
// A dry run is the default and writes nothing. Planning checks every row before anything changes: each move source
// against its pin, each target's parent folders, and every refusal rule; the dry run runs exactly these checks. --apply
// then moves (one same-volume rename per row), then deletes, and deletes the legacy VM last. A rerun after an
// interrupted --apply skips the rows already done and continues.
// Rules: docs/workflow/setup.md and docs/workflow/cleanup.md
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {execFileSync} = require('node:child_process');
const workspace = require('./workspace-paths.cjs');
const cleanup = require('./workspace-cleanup.cjs');

const LEGACY_FOLDERS = ['.phase1', '.phase2', '.phase3', '.phase4'];
const HELPER = 'src/godot-game/tools/workspace-paths.cjs';
const TOOLCHAIN = 'src/godot-game/tools/verification/toolchain.json';
const EXPORT_TOOLCHAIN = 'src/godot-game/production/assets/export-toolchain.json';
const USAGE = `Usage: node src/godot-game/tools/workspace-migrate.cjs [--apply]

Moves the main checkout's legacy .phase1 caches into its .local/ folder, found through the Git common dir from any
working directory. Without --apply, only lists each row and its size after checking every move source against its pin,
every target and every refusal rule; it exits 0 only when --apply would not be refused. --apply moves first, then
deletes, and deletes the legacy VM last.
Stop build servers first (dotnet build-server shutdown); they can hold files inside the package cache.`;

class UsageError extends Error {}

function git(cwd, args) {
  return execFileSync('git', args, {cwd, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'], windowsHide: true,
    env: {...process.env, GIT_OPTIONAL_LOCKS: '0'}, maxBuffer: 64 * 1024 * 1024});
}

function slashed(value) {
  return value.split(path.sep).join('/');
}

function hash(file, algorithm = 'sha256') {
  const digest = crypto.createHash(algorithm), descriptor = fs.openSync(file, 'r'), buffer = Buffer.alloc(1024 * 1024);
  try {
    for (let read; (read = fs.readSync(descriptor, buffer, 0, buffer.length, null)) > 0;) digest.update(buffer.subarray(0, read));
  } finally {
    fs.closeSync(descriptor);
  }
  return digest.digest('hex');
}

function isFile(target) {
  try {
    return fs.lstatSync(target).isFile();
  } catch {
    return false;
  }
}

function present(target) {
  try {
    fs.lstatSync(target);
    return true;
  } catch {
    return false;
  }
}

/** Total bytes below target, counting links themselves and never following them. */
function size(target) {
  let total = 0;
  const stack = [target];
  while (stack.length) {
    const current = stack.pop(), stat = fs.lstatSync(current);
    total += stat.isDirectory() ? 0 : stat.size;
    if (stat.isDirectory()) for (const name of fs.readdirSync(current)) stack.push(path.join(current, name));
  }
  return total;
}

function formatSize(bytes) {
  const units = ['B', 'KB', 'MB', 'GB', 'TB'];
  let value = bytes, unit = 0;
  while (value >= 1024 && unit < units.length - 1) {
    value /= 1024;
    unit++;
  }
  return unit === 0 ? `${bytes} B` : `${value.toFixed(1)} ${units[unit]}`;
}

/** The main checkout's `.local` location of a legacy identity, from the one legacy map (S = L in the main checkout). */
function legacyTarget(main, identity) {
  const match = workspace.LEGACY_MAP.filter(row => identity === row[0] || identity.startsWith(row[0] + '/'))
    .sort((left, right) => right[0].length - left[0].length)[0];
  if (!match) throw new Error(`no legacy map row for ${identity}`);
  const rest = identity.slice(match[0].length).replace(/^\//, '');
  return path.join(main, '.local', ...match[2].split('/'), ...(rest ? rest.split('/') : []));
}

function sumsList(file, sha512, name) {
  return fs.readFileSync(file, 'utf8').split(/\r?\n/).map(line => line.trim().split(/\s+/))
    .filter(row => row[0] === sha512 && row[1] === name).length === 1;
}

/** Rows in order: the legacy VM deletion first, then moves with their pin checks, then deletions of stray state. */
function rowsFor(main, toolchain, exportToolchain) {
  const rows = [{legacy: '.phase1/tools/linux-vm', action: 'delete', last: true,
    note: 'the phase that needs a VM recreates it under .local/tools/linux-vm'}];
  for (const archive of Object.values(toolchain.archives)) {
    rows.push({legacy: `.phase1/cache/${archive.file}`, action: 'move', kind: 'file',
      verify: file => hash(file, 'sha512') === archive.sha512 && (!archive.sha256 || hash(file) === archive.sha256)});
  }
  const win = toolchain.archives['win32-x64'];
  rows.push({legacy: '.phase1/cache/SHA512-SUMS.txt', action: 'move', kind: 'file',
    verify: file => sumsList(file, win.sha512, win.file)});
  const templates = exportToolchain.archive;
  rows.push({legacy: path.posix.dirname(templates.path), action: 'move', kind: 'directory', verify: directory => {
    const archive = path.join(directory, path.posix.basename(templates.path));
    const sums = path.join(directory, path.posix.basename(templates.checksums));
    return isFile(archive) && fs.statSync(archive).size === templates.bytes && hash(archive, 'sha512') === templates.sha512 &&
      isFile(sums) && sumsList(sums, templates.sha512, path.posix.basename(templates.path));
  }});
  for (const archive of Object.values(toolchain.archives)) {
    rows.push({legacy: `.phase1/tools/${archive.directory}`, action: 'move', kind: 'directory', verify: directory => {
      const executable = path.join(directory, archive.executable);
      return isFile(executable) && (!archive.executableSha256 || hash(executable) === archive.executableSha256);
    }});
  }
  rows.push({legacy: '.phase1/tools/linux-prep', action: 'move', kind: 'directory', verify: null});
  rows.push({legacy: '.phase1/feed', action: 'move', kind: 'directory', verify: directory => toolchain.godotPackages
    .every(item => {
      const file = path.join(directory, `${item.id}.${item.version}.nupkg`);
      return isFile(file) && hash(file) === item.sha256;
    })});
  rows.push({legacy: '.phase1/packages', action: 'move', kind: 'directory', cache: true});
  rows.push({legacy: '.phase1/http-cache', action: 'move', kind: 'directory', cache: true});
  rows.push({legacy: '.phase1/cli-home', action: 'delete', note: 'per-checkout CLI home, recreated by the runners'});
  rows.push({legacy: '.phase1/cache/game.packages.lock.json', action: 'delete', note: 'stray file'});
  rows.push({legacy: '.phase1/dotnet-home', action: 'delete', empty: true, note: 'empty: only zero-byte first-run sentinels'});
  return rows.map(row => ({...row, source: path.join(main, ...row.legacy.split('/')),
    target: row.action === 'move' ? legacyTarget(main, row.legacy) : null}));
}

/** Why the source cannot be migrated, or null. */
function unsafeSource(main, row, worktreePaths) {
  const linked = workspace.linkedComponent(main, row.source);
  if (linked && linked !== row.legacy) return `has a linked folder ${linked}`;
  const stat = fs.lstatSync(row.source);
  if (stat.isSymbolicLink() || (row.kind === 'file' ? !stat.isFile() : row.kind === 'directory' && !stat.isDirectory()) ||
    (!row.kind && !stat.isDirectory() && !stat.isFile())) return 'is a link or not a real file or folder';
  if (cleanup.hasTrackedFiles(main, row.legacy)) return 'contains tracked files';
  if (!cleanup.isIgnored(main, row.legacy + (stat.isDirectory() ? '/' : ''))) return 'is not ignored by Git';
  if (stat.isDirectory()) {
    const kept = cleanup.keepReason(row.source, worktreePaths);
    if (kept) return kept;
  }
  if (row.empty && size(row.source) > 0) return 'is not empty: it holds data';
  return null;
}

/** Why a target cannot be used: a link from the main checkout down, or an existing parent that is not a folder; or null. */
function blockedTarget(main, target) {
  const linked = workspace.linkedComponent(main, target);
  if (linked) return `${linked} is a link`;
  const parents = [];
  for (let parent = path.dirname(target); parent.length > main.length; parent = path.dirname(parent)) parents.unshift(parent);
  for (const parent of parents) {
    if (present(parent) && !fs.lstatSync(parent).isDirectory()) return `${slashed(path.relative(main, parent))} is not a folder`;
  }
  return null;
}

/** Classifies one row: delete, move, skip or refuse, with the line to print. */
function classify(main, row, worktreePaths) {
  const relativeTarget = row.target && slashed(path.relative(main, row.target));
  const arrow = row.target ? `${row.legacy} -> ${relativeTarget}` : row.legacy;
  if (!present(row.source)) {
    const state = row.target && present(row.target) ? 'already migrated' : row.target ? 'absent' : 'already deleted';
    return {...row, decision: 'skip', line: `skip ${arrow} (${state})`};
  }
  const unsafe = unsafeSource(main, row, worktreePaths);
  if (unsafe) return {...row, decision: 'refuse', line: `refuse: ${row.legacy} ${unsafe}`};
  const blocked = row.target && blockedTarget(main, row.target);
  if (blocked) return {...row, decision: 'refuse', line: `refuse: ${row.legacy}: ${blocked}`};
  const bytes = formatSize(size(row.source));
  if (row.action === 'delete') return {...row, decision: 'delete', line: `delete ${row.legacy} (${bytes}; ${row.note})`};
  if (present(row.target)) {
    if (row.cache) {
      return {...row, decision: 'delete',
        line: `delete ${row.legacy} (${bytes}; duplicate regenerable cache, ${relativeTarget} already exists)`};
    }
    if (!row.verify || !row.verify(row.target)) {
      return {...row, decision: 'refuse', line: `refuse: ${relativeTarget} already exists and does not verify against its pin`};
    }
    return {...row, decision: 'delete', line: `delete ${row.legacy} (${bytes}; duplicate of the verified ${relativeTarget})`};
  }
  if (row.verify && !row.verify(row.source)) {
    return {...row, decision: 'refuse', line: `refuse: ${row.legacy} does not verify against its pin`};
  }
  return {...row, decision: 'move', line: `move ${arrow} (${bytes})`};
}

function underLegacy(main, value) {
  const fold = text => process.platform === 'win32' ? text.replaceAll('/', '\\').toLowerCase() : text;
  const text = fold(String(value || ''));
  return LEGACY_FOLDERS.some(name => text.includes(fold(path.join(main, name) + path.sep)) ||
    text === fold(path.join(main, name)));
}

/** Live processes with their executable and command line; injected by the tests. */
function listProcesses() {
  if (process.platform === 'win32') {
    const script = 'Get-CimInstance Win32_Process | Select-Object ProcessId,ExecutablePath,CommandLine | ConvertTo-Json -Compress';
    const output = execFileSync('powershell.exe', ['-NoProfile', '-NonInteractive', '-Command', script],
      {encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'], windowsHide: true, maxBuffer: 64 * 1024 * 1024});
    const rows = JSON.parse(output);
    return (Array.isArray(rows) ? rows : [rows]).map(row =>
      ({pid: row.ProcessId, executable: row.ExecutablePath || '', commandLine: row.CommandLine || ''}));
  }
  return fs.readdirSync('/proc').filter(name => /^\d+$/.test(name)).map(name => {
    const read = action => {
      try {
        return action();
      } catch {
        return '';
      }
    };
    return {pid: Number(name), executable: read(() => fs.readlinkSync(`/proc/${name}/exe`)),
      commandLine: read(() => fs.readFileSync(`/proc/${name}/cmdline`, 'utf8').split('\0').join(' ').trim())};
  });
}

/** Refusals that hold for the whole migration. */
function globalRefusals(main, {env, list, worktreePaths}) {
  const refusals = [];
  try {
    git(main, ['cat-file', '-e', `HEAD:${HELPER}`]);
  } catch {
    refusals.push(`the main checkout's HEAD lacks ${HELPER}, so its branch still uses the legacy folders; ` +
      'switch it to a branch that has the .local layout first');
  }
  if (!ignoredByGitignore(main)) refusals.push('.local/ is not ignored by the main checkout\'s .gitignore');
  const linkedLocks = workspace.linkedComponent(main, path.join(main, '.local', 'locks'));
  if (linkedLocks) refusals.push(`${linkedLocks} is a link, so the toolchain lock cannot be taken in the main checkout`);
  const override = env[workspace.OVERRIDE];
  if (override && !samePath(override, path.join(main, '.local'))) {
    refusals.push(`${workspace.OVERRIDE} names a folder other than the main checkout's .local`);
  }
  for (const worktree of worktreePaths) {
    const lock = cleanup.findLock(worktree);
    if (lock) refusals.push(`runner lock ${path.basename(lock)} exists in ${samePath(worktree, main) ? 'the main checkout' :
      'worktree ' + path.basename(worktree)}; nothing moves while a runner may be active`);
  }
  let processes;
  try {
    processes = list();
  } catch (failure) {
    refusals.push(`could not list processes (${failure.message.split('\n')[0]})`);
    processes = [];
  }
  for (const row of processes) {
    if (row.pid === process.pid) continue;
    if (underLegacy(main, row.executable) || underLegacy(main, row.commandLine)) {
      refusals.push(`process ${row.pid} (${path.basename(String(row.executable || 'unknown'))}) runs from a legacy folder`);
    }
  }
  const record = path.join(main, '.phase1', 'tools', 'linux-vm', 'processes.json');
  if (isFile(record)) {
    let recorded = {};
    try {
      recorded = JSON.parse(fs.readFileSync(record, 'utf8'));
    } catch {
      refusals.push('the legacy VM process record cannot be read');
    }
    const live = new Set(processes.map(row => row.pid));
    for (const key of ['launcher', 'qemu']) {
      if (Number.isInteger(recorded[key]) && live.has(recorded[key])) {
        refusals.push(`the legacy VM ${key} process ${recorded[key]} is live`);
      }
    }
  }
  return refusals;
}

/** True when the deciding ignore rule for .local/ is a non-negated pattern of the root .gitignore. */
function ignoredByGitignore(main) {
  let output;
  try {
    output = git(main, ['check-ignore', '-v', '--', '.local/']);
  } catch {
    return false;
  }
  const location = output.split('\t')[0], first = location.indexOf(':'), second = location.indexOf(':', first + 1);
  return first > 0 && second > first && location.slice(0, first) === '.gitignore' && !location.slice(second + 1).startsWith('!');
}

function samePath(left, right) {
  const fold = value => process.platform === 'win32' ? path.resolve(value).toLowerCase() : path.resolve(value);
  return fold(left) === fold(right);
}

/** Plans the migration of the main checkout of the repository that contains root. */
function plan({root = workspace.checkoutRoot(), env = process.env, list = listProcesses} = {}) {
  const common = path.resolve(git(root, ['rev-parse', '--path-format=absolute', '--git-common-dir']).trim());
  if (path.basename(common) !== '.git') throw new Error('the Git common dir is not a .git folder; no main checkout');
  const main = path.dirname(common);
  const worktreePaths = cleanup.parseWorktrees(git(main, ['worktree', 'list', '--porcelain'])).map(item => item.path);
  const toolchain = JSON.parse(fs.readFileSync(path.join(root, TOOLCHAIN), 'utf8'));
  const exportToolchain = JSON.parse(fs.readFileSync(path.join(root, EXPORT_TOOLCHAIN), 'utf8'));
  const rows = rowsFor(main, toolchain, exportToolchain).map(row => classify(main, row, worktreePaths));
  const known = new Set(rows.map(row => row.legacy));
  const parents = new Set(['.phase1/cache', '.phase1/tools']);
  const leftovers = [];
  for (const name of LEGACY_FOLDERS) {
    const folder = path.join(main, name);
    if (!cleanup.isRealDirectory(folder)) continue;
    const queue = [name];
    while (queue.length) {
      const relative = queue.shift();
      for (const child of fs.readdirSync(path.join(main, ...relative.split('/'))).sort()) {
        const item = `${relative}/${child}`;
        if (known.has(item)) continue;
        if (parents.has(item) && cleanup.isRealDirectory(path.join(main, ...item.split('/')))) queue.push(item);
        else leftovers.push(item);
      }
    }
  }
  return {main, rows, leftovers, refusals: globalRefusals(main, {env, list, worktreePaths})};
}

/** Removes the legacy folders that the rows left empty, deepest first. */
function pruneEmpty(main, log) {
  for (const relative of ['.phase1/cache', '.phase1/tools', ...LEGACY_FOLDERS]) {
    const folder = path.join(main, ...relative.split('/'));
    if (cleanup.isRealDirectory(folder) && fs.readdirSync(folder).length === 0) {
      fs.rmdirSync(folder);
      log(`  deleted  ${relative} (empty)`);
    }
  }
}

/**
 * Runs a fully checked plan under the shared toolchain lock, so no bootstrap or Node setup writes the same targets:
 * every move, then every deletion, with the irreversible VM deletion last.
 */
function apply(planned, log) {
  if (!planned.rows.some(row => row.decision === 'move' || row.decision === 'delete')) {
    pruneEmpty(planned.main, log);
    return;
  }
  const lock = path.join(planned.main, '.local', 'locks', 'toolchain.lock');
  fs.mkdirSync(path.dirname(lock), {recursive: true});
  let descriptor;
  try {
    descriptor = fs.openSync(lock, 'wx');
  } catch (failure) {
    if (failure.code === 'EEXIST') throw new Error('TOOLCHAIN_LOCKED: another acquisition owns .local/locks/toolchain.lock');
    throw failure;
  }
  try {
    fs.writeFileSync(descriptor, JSON.stringify({tool: 'workspace-migrate', pid: process.pid, at: new Date().toISOString()}));
    applyLocked(planned, log);
  } finally {
    fs.closeSync(descriptor);
    fs.rmSync(lock, {force: true});
  }
}

function applyLocked(planned, log) {
  const deletes = planned.rows.filter(row => row.decision === 'delete');
  for (const row of planned.rows.filter(item => item.decision === 'move')) {
    fs.mkdirSync(path.dirname(row.target), {recursive: true});
    fs.renameSync(row.source, row.target);
    if (row.verify && !row.verify(row.target)) {
      fs.renameSync(row.target, row.source);
      throw new Error(`${row.legacy} does not verify against its pin after the move; it was moved back`);
    }
    log(`  moved    ${row.legacy} -> ${slashed(path.relative(planned.main, row.target))}`);
  }
  for (const row of [...deletes.filter(item => !item.last), ...deletes.filter(item => item.last)]) {
    fs.rmSync(row.source, {recursive: true, force: false, maxRetries: 3});
    log(`  deleted  ${row.legacy}`);
  }
  pruneEmpty(planned.main, log);
}

function parseArgs(argv) {
  const options = {apply: false, help: false};
  for (const argument of argv) {
    if (argument === '--apply') options.apply = true;
    else if (argument === '--help' || argument === '-h') options.help = true;
    else throw new UsageError(`unknown argument: ${argument}`);
  }
  return options;
}

/** Runs the command; returns the process exit code. */
function run(argv, {root, env = process.env, list = listProcesses, now = () => new Date(), log = console.log,
  error = console.error} = {}) {
  let options;
  try {
    options = parseArgs(argv);
  } catch (failure) {
    if (!(failure instanceof UsageError)) throw failure;
    error(`${failure.message}\n${USAGE}`);
    return 2;
  }
  if (options.help) {
    log(USAGE);
    return 0;
  }
  let planned;
  try {
    planned = plan({root, env, list});
  } catch (failure) {
    error(`refuse: ${String(failure.stderr || failure.message).trim().split('\n')[0]}`);
    return 1;
  }
  log(`Workspace migration ${options.apply ? '(apply)' : '(dry run)'} of the main checkout at ${now().toISOString()}`);
  for (const row of planned.rows) log(row.line);
  for (const item of planned.leftovers) log(`skip ${item} (not part of the migration; left in place)`);
  const refusals = [...planned.rows.filter(row => row.decision === 'refuse').map(row => row.line),
    ...planned.refusals.map(reason => `refuse: ${reason}`)];
  for (const line of planned.refusals.map(reason => `refuse: ${reason}`)) log(line);
  if (refusals.length) {
    log(`${refusals.length} refusal(s): nothing ${options.apply ? 'was' : 'would be'} changed.`);
    return 1;
  }
  if (!options.apply) {
    log('Dry run: nothing was changed. Rerun with --apply to migrate.');
    return 0;
  }
  try {
    apply(planned, log);
  } catch (failure) {
    error(`migration stopped: ${failure.message}`);
    return 1;
  }
  log('Migration complete. Run the cleanup dry run next: node src/godot-game/tools/workspace-cleanup.cjs');
  return 0;
}

module.exports = {formatSize, legacyTarget, listProcesses, parseArgs, plan, run, UsageError};

if (require.main === module) process.exitCode = run(process.argv.slice(2));
