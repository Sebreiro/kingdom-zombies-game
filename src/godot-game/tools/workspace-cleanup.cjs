'use strict';
// Lists and removes ignored task outputs and finished Git worktrees. Nothing is deleted without --apply.
// Rules: docs/workflow/cleanup.md
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const crypto = require('node:crypto');
const {execFileSync, spawnSync} = require('node:child_process');
const workspace = require('./workspace-paths.cjs');

// The regenerable workspace root keeps its toolchain, downloads and NuGet caches; runs, temp and anything else go.
const KEPT_LOCAL = new Set(['tools', 'downloads', 'nuget']);
const KEPT_LOCAL_REASON = 'shared toolchain, download or NuGet cache';
// Legacy per-phase folders keep today's rule until workspace-migrate.cjs retires them.
const LEGACY_DURABLE = new Set(['cache', 'cli-home', 'dotnet-home', 'feed', 'http-cache', 'packages', 'tools']);
const LEGACY_REASON = 'legacy toolchain cache; run workspace-migrate.cjs';
const LEGACY_LOCKS = ['.phase1/check.lock', '.phase2/desktop.lock'];
const BUILD_NAMES = new Set(['bin', 'obj', '.godot']);
const NESTED_REASON = 'contains a Git worktree or repository';
const IN_USE_HINT = 'a process may be using it, such as a .NET build server (dotnet build-server shutdown); stop it and rerun';
const USAGE = `Usage: node src/godot-game/tools/workspace-cleanup.cjs [--apply] [--build] [--worktrees-only] [--remove-worktree <path>]...
  [--remove-orphan <path>]...

Without --apply, only reports what would be deleted. Keeps .local/tools, .local/downloads and .local/nuget;
deletes .local/runs, .local/temp and every other .local entry. Nothing is deleted while a file exists in this
checkout's .local/locks, the shared root's locks folder or a legacy runner lock.
  --apply                   Delete the listed task outputs and prune missing worktrees.
  --build                   Also delete ignored bin/, obj/ and .godot/ build outputs.
  --worktrees-only          Leave this checkout's outputs alone; only handle worktrees.
  --remove-worktree <path>  Remove this linked worktree if it is clean and pushed (repeatable).
  --remove-orphan <path>    Remove this task worktree folder whose Git record is gone, if every remaining file is
                            ignored or equals a remote branch (repeatable).`;

class UsageError extends Error {}

function git(cwd, args) {
  return execFileSync('git', args, {cwd, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'], maxBuffer: 256 * 1024 * 1024});
}

function parseArgs(argv) {
  const options = {apply: false, build: false, help: false, worktreesOnly: false, worktrees: [], orphans: []};
  for (let index = 0; index < argv.length; index++) {
    const argument = argv[index];
    if (argument === '--apply') options.apply = true;
    else if (argument === '--build') options.build = true;
    else if (argument === '--worktrees-only') options.worktreesOnly = true;
    else if (argument === '--help' || argument === '-h') options.help = true;
    else if (argument === '--remove-worktree' || argument === '--remove-orphan') {
      const value = argv[++index];
      if (!value || value.startsWith('--')) throw new UsageError(`${argument} needs a path`);
      (argument === '--remove-worktree' ? options.worktrees : options.orphans).push(value);
    } else throw new UsageError(`unknown argument: ${argument}`);
  }
  return options;
}

function samePath(left, right) {
  const a = path.resolve(left), b = path.resolve(right);
  return process.platform === 'win32' ? a.toLowerCase() === b.toLowerCase() : a === b;
}

function isRealDirectory(target) {
  try {
    return fs.lstatSync(target).isDirectory();
  } catch {
    return false;
  }
}

function isIgnored(root, relative) {
  try {
    git(root, ['check-ignore', '-q', '--', relative]);
    return true;
  } catch {
    return false;
  }
}

function hasTrackedFiles(root, relative) {
  return git(root, ['ls-files', '-z', '--', relative]).length > 0;
}

function isWithin(parent, child) {
  const fold = value => process.platform === 'win32' ? path.resolve(value).toLowerCase() : path.resolve(value);
  const relative = path.relative(fold(parent), fold(child));
  return relative === '' || (!relative.startsWith('..') && !path.isAbsolute(relative));
}

function relativeSlashed(from, to) {
  return path.relative(from, to).split(path.sep).join('/');
}

function looksLikeRepository(names) {
  const lower = new Set(names.map(name => name.toLowerCase()));
  return lower.has('.git') || (lower.has('head') && lower.has('objects') && lower.has('refs'));
}

/**
 * Returns why target must be kept (a registered worktree or a repository at any depth, or a scan error), or null.
 * Descends only into lstat-confirmed directories; ownGit skips the target's own top-level .git entry.
 */
function keepReason(target, worktreePaths, {ownGit = false} = {}) {
  const nested = worktreePaths.find(worktree => isWithin(target, worktree));
  if (nested) return samePath(nested, target) ? NESTED_REASON : `${NESTED_REASON} at ${relativeSlashed(target, path.resolve(nested))}`;
  let stat;
  try {
    stat = fs.lstatSync(target);
  } catch (failure) {
    return `could not be scanned (${failure.code || failure.message})`;
  }
  if (!stat.isDirectory()) return null;
  const stack = [target];
  while (stack.length) {
    const directory = stack.pop();
    const at = directory === target ? '' : ` at ${relativeSlashed(target, directory)}`;
    let entries;
    try {
      entries = fs.readdirSync(directory, {withFileTypes: true});
    } catch (failure) {
      return `could not be scanned (${failure.code || failure.message})${at}`;
    }
    if (ownGit && directory === target) entries = entries.filter(entry => entry.name.toLowerCase() !== '.git');
    if (looksLikeRepository(entries.map(entry => entry.name))) return NESTED_REASON + at;
    for (const entry of entries) {
      if (entry.isFile()) continue;
      const child = path.join(directory, entry.name);
      let childStat;
      try {
        childStat = fs.lstatSync(child);
      } catch (failure) {
        return `could not be scanned (${failure.code || failure.message}) at ${relativeSlashed(target, child)}`;
      }
      if (childStat.isDirectory()) stack.push(child);
    }
  }
  return null;
}

/** Returns the first entry of a locks folder (a dangling link counts), or null when it is missing or empty. */
function lockIn(directory) {
  let names;
  try {
    names = fs.readdirSync(directory);
  } catch (failure) {
    if (failure.code === 'ENOENT' || failure.code === 'ENOTDIR') return null;
    throw failure;
  }
  return names.length ? path.join(directory, names.sort()[0]) : null;
}

/** Returns the first runner lock of the checkout at dir: legacy fixed paths, then .local/locks; unexpected errors throw. */
function findLock(dir) {
  for (const entry of LEGACY_LOCKS) {
    const lock = path.join(dir, entry);
    try {
      fs.lstatSync(lock);
      return lock;
    } catch (failure) {
      if (failure.code !== 'ENOENT' && failure.code !== 'ENOTDIR') throw failure;
    }
  }
  return lockIn(path.join(dir, '.local', 'locks'));
}

/** Returns the first lock in the shared root (toolchain, desktop), or null; in the main checkout it is .local/locks. */
function findSharedLock(root) {
  return lockIn(path.dirname(workspace.locations(root).locks.toolchain));
}

/** Classifies children of one ignored output root; unsafe roots and children are kept whole. */
function classifyRoot(root, relative, reason, durable, durableReason, worktreePaths) {
  const directory = path.join(root, relative);
  const linked = workspace.linkedComponent(root, directory);
  if (linked) return [{path: relative, action: 'keep', reason: `${linked} is a link`}];
  if (!isRealDirectory(directory)) return [];
  if (hasTrackedFiles(root, relative)) return [{path: relative, action: 'keep', reason: 'contains tracked files'}];
  if (!isIgnored(root, relative + '/')) return [{path: relative, action: 'keep', reason: 'not ignored by Git'}];
  const names = fs.readdirSync(directory);
  if (worktreePaths.some(worktree => samePath(worktree, directory)) || looksLikeRepository(names)) {
    return [{path: relative, action: 'keep', reason: NESTED_REASON}];
  }
  return names.sort().map(name => {
    const child = `${relative}/${name}`, absolute = path.join(root, child);
    if (durable.has(name.toLowerCase())) return {path: child, action: 'keep', reason: durableReason};
    if (fs.lstatSync(absolute).isSymbolicLink()) return {path: child, action: 'keep', reason: `${child} is a link`};
    const kept = keepReason(absolute, worktreePaths);
    if (kept) return {path: child, action: 'keep', reason: kept};
    return {path: child, action: 'delete', reason};
  });
}

function buildOutputs(root, worktreePaths) {
  return git(root, ['ls-files', '-z', '--others', '--ignored', '--exclude-standard', '--directory'])
    .split('\0')
    .filter(entry => entry.endsWith('/'))
    .map(entry => entry.slice(0, -1))
    .filter(entry => BUILD_NAMES.has(entry.split('/').pop()) && !/^(?:\.local|\.phase\d+)(\/|$)/.test(entry))
    .sort()
    .map(entry => {
      const linked = workspace.linkedComponent(root, path.join(root, entry));
      if (linked) return {path: entry, action: 'keep', reason: `${linked} is a link`};
      const kept = keepReason(path.join(root, entry), worktreePaths);
      return kept ? {path: entry, action: 'keep', reason: kept} : {path: entry, action: 'delete', reason: 'build output'};
    });
}

/** Returns keep/delete decisions for ignored outputs in the checkout at root. */
function planArtifacts(root, {build = false, worktreePaths = []} = {}) {
  const entries = [];
  entries.push(...classifyRoot(root, '.local', 'regenerable output', KEPT_LOCAL, KEPT_LOCAL_REASON, worktreePaths));
  const legacy = fs.readdirSync(root).filter(name => /^\.phase\d+$/.test(name)).sort();
  for (const name of legacy) {
    const durable = name === '.phase1' ? LEGACY_DURABLE : new Set();
    entries.push(...classifyRoot(root, name, 'legacy phase output', durable, LEGACY_REASON, worktreePaths));
  }
  entries.push(...classifyRoot(root, '.planning/tmp', 'workflow scratch', new Set(), '', worktreePaths));
  if (build) entries.push(...buildOutputs(root, worktreePaths));
  return entries;
}

function parseWorktrees(text) {
  return text.split(/\r?\n\r?\n/).filter(block => block.trim()).map((block, index) => {
    const worktree = {main: index === 0, path: '', head: '', branch: null, locked: false, prunable: false};
    for (const line of block.split(/\r?\n/)) {
      const [key, ...rest] = line.split(' ');
      const value = rest.join(' ');
      if (key === 'worktree') worktree.path = value;
      else if (key === 'HEAD') worktree.head = value;
      else if (key === 'branch') worktree.branch = value.replace(/^refs\/heads\//, '');
      else if (key === 'locked') worktree.locked = true;
      else if (key === 'prunable') worktree.prunable = true;
    }
    return worktree;
  });
}

function worktreeState(root, worktree, worktreePaths) {
  if (worktree.main) return {removable: false, reason: 'main checkout'};
  if (samePath(worktree.path, root)) return {removable: false, reason: 'current worktree'};
  if (worktree.locked) return {removable: false, reason: 'locked'};
  if (worktree.prunable || !fs.existsSync(worktree.path)) return {removable: false, prune: true, reason: 'directory missing'};
  const status = git(worktree.path, ['status', '--porcelain=v1', '--untracked-files=all', '--ignore-submodules=none']);
  if (status.trim()) return {removable: false, reason: 'uncommitted changes'};
  // Only the repository's own .gitignore files decide what is disposable; core.excludesFile and info/exclude can hide work.
  if (git(worktree.path, ['ls-files', '-z', '--others', '--exclude-per-directory=.gitignore']).length) {
    return {removable: false, reason: 'untracked files hidden by Git exclude settings'};
  }
  if (git(worktree.path, ['ls-files', '-v', '-z']).split('\0').some(line => /^[a-zS] /.test(line))) {
    return {removable: false, reason: 'files marked assume-unchanged or skip-worktree'};
  }
  const gitDirectory = path.resolve(worktree.path, git(worktree.path, ['rev-parse', '--git-dir']).trim());
  const operation = ['rebase-merge', 'rebase-apply', 'MERGE_HEAD', 'CHERRY_PICK_HEAD', 'REVERT_HEAD', 'BISECT_LOG']
    .find(name => fs.existsSync(path.join(gitDirectory, name)));
  if (operation) return {removable: false, reason: `operation in progress (${operation})`};
  if (!git(root, ['branch', '-r', '--contains', worktree.head]).trim()) {
    return {removable: false, reason: 'commits not on any remote branch'};
  }
  const lock = findLock(worktree.path);
  if (lock) return {removable: false, reason: `runner lock at ${relativeSlashed(worktree.path, lock)}`};
  const others = worktreePaths.filter(other => !samePath(other, worktree.path));
  const nested = keepReason(worktree.path, others, {ownGit: true});
  if (nested) return {removable: false, reason: nested};
  return {removable: true, reason: 'clean and pushed'};
}

/** Lists linked worktrees of the repository containing root with their removal state. */
function planWorktrees(root) {
  const worktrees = parseWorktrees(git(root, ['worktree', 'list', '--porcelain']));
  const paths = worktrees.map(worktree => worktree.path);
  return worktrees.map(worktree => ({...worktree, ...worktreeState(root, worktree, paths)}));
}

function removeInside(root, relative) {
  const target = path.resolve(root, relative);
  const inside = path.relative(root, target);
  if (!inside || inside.startsWith('..') || path.isAbsolute(inside)) {
    throw new Error(`refusing to delete outside the checkout: ${target}`);
  }
  const linked = workspace.linkedComponent(root, target);
  if (linked) throw new Error(`refusing to delete through the link ${linked}`);
  fs.rmSync(target, {recursive: true, force: true, maxRetries: 3});
}

function pause(milliseconds) {
  Atomics.wait(new Int32Array(new SharedArrayBuffer(4)), 0, 0, milliseconds);
}

/**
 * Deletes a worktree's ignored outputs while its Git record still exists. Build servers and test runs keep files there;
 * a file in use stops the removal here, before `git worktree remove` drops the record and leaves an orphaned folder.
 * Only the repository's .gitignore files count, as in worktreeState; links are unlinked, never followed.
 */
function removeIgnoredOutputs(worktreePath) {
  const entries = git(worktreePath, ['ls-files', '-z', '--others', '--ignored', '--exclude-per-directory=.gitignore', '--directory'])
    .split('\0').filter(Boolean).map(entry => entry.replace(/\/$/, ''));
  for (const entry of entries) {
    const target = path.join(worktreePath, entry);
    const linked = workspace.linkedComponent(worktreePath, path.dirname(target));
    if (linked) throw new Error(`refusing to delete ${entry} through the link ${linked}`);
    try {
      fs.rmSync(target, {recursive: true, force: true, maxRetries: 3});
    } catch (failure) {
      throw new Error(`could not delete ${entry} (${failure.code || failure.message}); ${IN_USE_HINT}`);
    }
  }
}

/**
 * Moves the folder aside and back. Windows refuses while a process works in it or holds a file there without sharing.
 * The worktree record is locked meanwhile, so no `git worktree prune` drops it while the folder is aside; a probe
 * that cannot move the folder back leaves the record locked.
 */
function probeMovable(root, folder) {
  const aside = `${folder}.cleanup-probe-${process.pid}`;
  git(root, ['worktree', 'lock', '--reason', `workspace-cleanup move probe; if ${path.basename(aside)} exists, ` +
    'move it back and run git worktree unlock', folder]);
  try {
    fs.renameSync(folder, aside);
  } catch (failure) {
    git(root, ['worktree', 'unlock', folder]);
    throw new Error(`the folder cannot be moved (${failure.code || failure.message}); ${IN_USE_HINT}`);
  }
  let last;
  for (let attempt = 0; attempt < 20; attempt++) {
    try {
      fs.renameSync(aside, folder);
      git(root, ['worktree', 'unlock', folder]);
      return;
    } catch (failure) {
      last = failure;
      pause(100);
    }
  }
  throw new Error(`moved the folder to ${aside} for a probe and could not move it back (${last.code || last.message}); ` +
    `the worktree record stays locked: move the folder back by hand, then run git worktree unlock ${folder}`);
}

/** Throws when a runner lock exists in folder; a runner may have started there since the plan was made. */
function assertNoLock(folder) {
  const lock = findLock(folder);
  if (lock) throw new Error(`runner lock at ${relativeSlashed(folder, lock)}; nothing was deleted there`);
}

/** Removes a worktree; links (such as junctions to the shared root) are unlinked, never followed. */
function removeWorktree(root, worktreePath) {
  assertNoLock(worktreePath);
  removeIgnoredOutputs(worktreePath);
  probeMovable(root, worktreePath);
  const orphaned = failure => {
    const listed = parseWorktrees(git(root, ['worktree', 'list', '--porcelain'])).some(worktree => samePath(worktree.path, worktreePath));
    const detail = String(failure.stderr || failure.message).trim();
    return listed || !fs.existsSync(worktreePath) ? new Error(detail)
      : new Error(`${detail}; Git dropped the worktree record, so finish with --remove-orphan ${worktreePath}`);
  };
  try {
    git(root, ['worktree', 'remove', worktreePath]);
    // Git leaves folders it cannot fully delete; Node's rmSync unlinks junctions and symlinks without following them.
    if (fs.existsSync(worktreePath)) fs.rmSync(worktreePath, {recursive: true, force: true, maxRetries: 3});
  } catch (failure) {
    throw orphaned(failure);
  }
  if (fs.existsSync(worktreePath)) throw orphaned(new Error('folder still exists after removal'));
}

/** The folder of this repository's task worktrees: <worktrees.root, else the system temp folder>/<main checkout name>. */
function worktreesHome(root) {
  let base = '';
  try {
    base = git(root, ['config', '--get', 'worktrees.root']).trim();
  } catch {
    // Unset: the system temp folder stands in, as in docs/workflow/ai-collaboration.md.
  }
  const common = git(root, ['rev-parse', '--path-format=absolute', '--git-common-dir']).trim();
  return path.join(path.resolve(base || os.tmpdir()), path.basename(path.dirname(path.resolve(common))));
}

/** Folders in the task worktree folder that no worktree record names. */
function listOrphans(root, worktrees) {
  const home = worktreesHome(root);
  let names;
  try {
    names = fs.readdirSync(home);
  } catch {
    return [];
  }
  return names.sort().map(name => path.join(home, name))
    .filter(folder => isRealDirectory(folder) && !worktrees.some(worktree => samePath(worktree.path, folder)));
}

/** Every file under folder as a slash-separated relative path; links are skipped, since unlinking one loses no data. */
function listFiles(folder) {
  const files = [], stack = [''];
  while (stack.length) {
    const relative = stack.pop();
    for (const entry of fs.readdirSync(path.join(folder, relative), {withFileTypes: true})) {
      const child = relative ? `${relative}/${entry.name}` : entry.name;
      const stat = fs.lstatSync(path.join(folder, child));
      if (stat.isDirectory()) stack.push(child);
      else if (stat.isFile()) files.push(child);
    }
  }
  return files.sort();
}

/** Paths the repository's .gitignore files ignore, judged in this checkout; info/exclude and core.excludesFile never count. */
function ignoredByGitignore(root, files) {
  if (!files.length) return new Set();
  const result = spawnSync('git', ['-c', 'core.excludesFile=', 'check-ignore', '--no-index', '-v', '-n', '-z', '--stdin'],
    {cwd: root, input: files.join('\0') + '\0', encoding: 'utf8', maxBuffer: 256 * 1024 * 1024, windowsHide: true});
  // Exit 1 means no path is ignored, the usual state once removal deleted an orphan's ignored outputs.
  if (result.error) throw result.error;
  if (result.status !== 0 && result.status !== 1) {
    throw new Error(`git check-ignore failed with exit ${result.status}: ${String(result.stderr).trim()}`);
  }
  const fields = result.stdout.split('\0'), ignored = new Set();
  for (let index = 0; index + 3 < fields.length; index += 4) {
    const [source, , pattern, file] = fields.slice(index, index + 4);
    if (source && path.basename(source) === '.gitignore' && !pattern.startsWith('!')) ignored.add(file);
  }
  return ignored;
}

/** The Git object id a file's bytes have as a blob in this repository. */
function blobId(file, format) {
  const bytes = fs.readFileSync(file);
  return crypto.createHash(format === 'sha256' ? 'sha256' : 'sha1')
    .update(`blob ${bytes.length}\0`).update(bytes).digest('hex');
}

/**
 * Decides whether an orphaned task worktree folder can go: it must sit in the task worktree folder, have no record,
 * repository, lock or scan error, and one remote branch tip must account for every remaining file. A file that tip
 * tracks must equal its blob there, even when a .gitignore pattern matches it; a file it does not track must be
 * ignored. A failed removal deletes files, so a subset of the tree is enough; nothing that was not pushed is lost.
 */
function orphanState(root, folder, worktrees) {
  const home = worktreesHome(root);
  if (worktrees.some(worktree => samePath(worktree.path, folder))) return {removable: false, reason: 'a registered worktree; use --remove-worktree'};
  if (!samePath(path.dirname(folder), home)) return {removable: false, reason: `not a task worktree folder in ${home}`};
  if (!isRealDirectory(folder)) return {removable: false, reason: 'not a folder'};
  const kept = keepReason(folder, worktrees.map(worktree => worktree.path));
  if (kept) return {removable: false, reason: kept};
  const lock = findLock(folder);
  if (lock) return {removable: false, reason: `runner lock at ${relativeSlashed(folder, lock)}`};
  const files = listFiles(folder);
  if (!files.length) return {removable: true, reason: 'no files remain'};
  const ignored = ignoredByGitignore(root, files);
  const format = git(root, ['rev-parse', '--show-object-format']).trim();
  const ids = new Map();
  const idOf = file => {
    if (!ids.has(file)) ids.set(file, blobId(path.join(folder, file), format));
    return ids.get(file);
  };
  const tips = new Map();
  for (const line of git(root, ['for-each-ref', '--format=%(objectname) %(refname)', 'refs/remotes']).split('\n')) {
    const [id, ref] = line.trim().split(' ');
    // A remote's HEAD only names another remote branch, which is listed under its own name.
    if (id && ref && !ref.endsWith('/HEAD') && !tips.has(id)) tips.set(id, ref.replace(/^refs\/remotes\//, ''));
  }
  const trees = [...tips].map(([id, name]) => {
    const tree = new Map();
    for (const row of git(root, ['ls-tree', '-r', '-z', '--full-tree', id]).split('\0').filter(Boolean)) {
      const tab = row.indexOf('\t');
      tree.set(row.slice(tab + 1), row.slice(0, tab).split(' ')[2]);
    }
    return {name, tree};
  });
  // An ignored file is a disposable output only when no remote branch tracks its path; a tracked one is source.
  const disposable = file => ignored.has(file) && !trees.some(({tree}) => tree.has(file));
  let best = null;
  for (const {name, tree} of trees) {
    let compared = 0;
    const mismatched = files.filter(file => {
      if (!tree.has(file)) return !disposable(file);
      compared++;
      return tree.get(file) !== idOf(file);
    });
    if (!mismatched.length) return {removable: true, reason: compared ? `matches ${name}` : 'only ignored outputs remain'};
    if (!best || mismatched.length < best.length) best = mismatched;
  }
  const missed = best ?? files.filter(file => !disposable(file));
  if (!missed.length) return {removable: true, reason: 'only ignored outputs remain'};
  return {removable: false, reason: `${missed.length} file(s) match no remote branch, such as ${missed[0]}`};
}

/** Deletes an orphaned folder; links inside are unlinked, never followed. */
function removeOrphan(folder) {
  assertNoLock(folder);
  try {
    fs.rmSync(folder, {recursive: true, force: true, maxRetries: 3});
  } catch (failure) {
    throw new Error(`${failure.code || failure.message}; ${IN_USE_HINT}`);
  }
  if (fs.existsSync(folder)) throw new Error(`folder still exists after removal; ${IN_USE_HINT}`);
}

/** Runs the command; returns the process exit code. */
function run(argv, {cwd = process.cwd(), log = console.log, error = console.error} = {}) {
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
  try {
    return execute(options, {cwd, log, error});
  } catch (failure) {
    error(`cleanup stopped: ${String(failure.stderr || failure.message).trim()}`);
    return 1;
  }
}

function execute(options, {cwd, log, error}) {
  const root = path.resolve(git(cwd, ['rev-parse', '--show-toplevel']).trim());
  const worktrees = planWorktrees(root);
  const artifacts = options.worktreesOnly ? []
    : planArtifacts(root, {...options, worktreePaths: worktrees.map(worktree => worktree.path)});
  const requested = options.worktrees.map(value => {
    const match = worktrees.find(worktree => samePath(worktree.path, path.resolve(cwd, value)));
    if (!match) return {path: path.resolve(cwd, value), removable: false, reason: 'not a worktree of this repository'};
    if (!match.removable) return match;
    // The scans above can take minutes, so a runner may have started in the worktree since it was planned.
    const lateLock = findLock(match.path);
    return lateLock ? {...match, removable: false, reason: `runner lock at ${relativeSlashed(match.path, lateLock)}`} : match;
  });
  const orphans = listOrphans(root, worktrees);
  const requestedOrphans = options.orphans.map(value => {
    const folder = path.resolve(cwd, value);
    return {path: folder, orphan: true, ...orphanState(root, folder, worktrees)};
  });
  let failures = 0;

  log(`Workspace cleanup ${options.apply ? '(apply)' : '(dry run)'}: ${root}`);
  const lock = findLock(root) ?? findSharedLock(root);
  if (lock) {
    error(`  ${lock} exists: a runner is active or stopped uncleanly. Inspect it; nothing is deleted while it exists.`);
    if (options.apply) return 1;
  }
  const refused = [...requested, ...requestedOrphans].filter(worktree => !worktree.removable);
  for (const worktree of refused) error(`  refused   ${worktree.path} (${worktree.reason})`);
  if (options.apply && refused.length) {
    error('  nothing was deleted because a requested worktree or orphan was refused');
    return 1;
  }
  for (const entry of artifacts) {
    let status = entry.action;
    if (options.apply && entry.action === 'delete') {
      try {
        removeInside(root, entry.path);
        status = 'deleted';
      } catch (failure) {
        failures++;
        status = 'failed';
        entry.reason = failure.message;
      }
    }
    log(`  ${status.padEnd(8)} ${entry.path}  (${entry.reason})`);
  }

  log('Worktrees:');
  for (const worktree of worktrees) {
    const label = worktree.removable ? 'removable' : worktree.prune ? 'prune' : 'keep';
    log(`  ${label.padEnd(9)} ${worktree.path}  [${worktree.branch ?? 'detached'}] (${worktree.reason})`);
  }
  for (const folder of orphans) log(`  ${'orphan'.padEnd(9)} ${folder}  (no worktree record; remove with --remove-orphan)`);
  if (options.apply && worktrees.some(worktree => worktree.prune)) {
    git(root, ['worktree', 'prune']);
    log('  ran git worktree prune for records whose folders are missing (any session\'s; locked records stay)');
  }
  failures += refused.length;
  for (const worktree of requested.filter(item => item.removable)) {
    if (options.apply) {
      try {
        removeWorktree(root, worktree.path);
        log(`  removed   ${worktree.path}`);
      } catch (failure) {
        failures++;
        error(`  failed    ${worktree.path} (${String(failure.stderr || failure.message).trim()})`);
      }
    } else log(`  would remove ${worktree.path}`);
  }
  for (const orphan of requestedOrphans.filter(item => item.removable)) {
    if (options.apply) {
      try {
        removeOrphan(orphan.path);
        log(`  removed orphan ${orphan.path} (${orphan.reason})`);
      } catch (failure) {
        failures++;
        error(`  failed    ${orphan.path} (${failure.message})`);
      }
    } else log(`  would remove orphan ${orphan.path} (${orphan.reason})`);
  }

  const pending = artifacts.filter(entry => entry.action === 'delete').length;
  if (!options.apply && lock) log(`Dry run: --apply would delete nothing while ${lock} exists; ${pending} output(s) are listed for deletion after that.`);
  else if (!options.apply) log(`Dry run: ${pending} output(s) would be deleted. Rerun with --apply to delete them.`);
  return failures ? 1 : 0;
}

module.exports = {parseArgs, parseWorktrees, planArtifacts, planWorktrees, run, UsageError, keepReason, isIgnored,
  hasTrackedFiles, isRealDirectory, findLock, findSharedLock};

if (require.main === module) process.exitCode = run(process.argv.slice(2));
