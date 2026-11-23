'use strict';
// Lists and removes ignored task outputs and finished Git worktrees. Nothing is deleted without --apply.
// Rules: docs/workflow/cleanup.md
const fs = require('node:fs');
const path = require('node:path');
const {execFileSync} = require('node:child_process');

const DURABLE_PHASE1 = new Set(['cache', 'cli-home', 'dotnet-home', 'feed', 'http-cache', 'packages', 'tools']);
const BUILD_NAMES = new Set(['bin', 'obj', '.godot']);
const RUNNER_LOCKS = ['.phase1/check.lock', '.phase2/desktop.lock'];
const NESTED_REASON = 'contains a Git worktree or repository';
const USAGE = `Usage: node src/godot-game/tools/workspace-cleanup.cjs [--apply] [--build] [--worktrees-only] [--remove-worktree <path>]...

Without --apply, only reports what would be deleted.
  --apply                   Delete the listed task outputs and prune missing worktrees.
  --build                   Also delete ignored bin/, obj/ and .godot/ build outputs.
  --worktrees-only          Leave this checkout's outputs alone; only handle worktrees.
  --remove-worktree <path>  Remove this linked worktree if it is clean and pushed (repeatable).`;

class UsageError extends Error {}

function git(cwd, args) {
  return execFileSync('git', args, {cwd, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'], maxBuffer: 256 * 1024 * 1024});
}

function parseArgs(argv) {
  const options = {apply: false, build: false, help: false, worktreesOnly: false, worktrees: []};
  for (let index = 0; index < argv.length; index++) {
    const argument = argv[index];
    if (argument === '--apply') options.apply = true;
    else if (argument === '--build') options.build = true;
    else if (argument === '--worktrees-only') options.worktreesOnly = true;
    else if (argument === '--help' || argument === '-h') options.help = true;
    else if (argument === '--remove-worktree') {
      const value = argv[++index];
      if (!value || value.startsWith('--')) throw new UsageError('--remove-worktree needs a path');
      options.worktrees.push(value);
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

/** Returns the first fixed-path runner lock under dir (a dangling link counts), or null; unexpected errors throw. */
function findLock(dir) {
  for (const entry of RUNNER_LOCKS) {
    const lock = path.join(dir, entry);
    try {
      fs.lstatSync(lock);
      return lock;
    } catch (failure) {
      if (failure.code !== 'ENOENT' && failure.code !== 'ENOTDIR') throw failure;
    }
  }
  return null;
}

/** Classifies children of one ignored output root; unsafe roots and children are kept whole. */
function classifyRoot(root, relative, reason, durable, worktreePaths) {
  const directory = path.join(root, relative);
  if (!isRealDirectory(directory)) return [];
  if (hasTrackedFiles(root, relative)) return [{path: relative, action: 'keep', reason: 'contains tracked files'}];
  if (!isIgnored(root, relative + '/')) return [{path: relative, action: 'keep', reason: 'not ignored by Git'}];
  const names = fs.readdirSync(directory);
  if (worktreePaths.some(worktree => samePath(worktree, directory)) || looksLikeRepository(names)) {
    return [{path: relative, action: 'keep', reason: NESTED_REASON}];
  }
  return names.sort().map(name => {
    const child = `${relative}/${name}`, absolute = path.join(root, child);
    if (durable.has(name.toLowerCase())) return {path: child, action: 'keep', reason: 'pinned toolchain cache'};
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
    .filter(entry => BUILD_NAMES.has(entry.split('/').pop()) && !/^\.phase\d+(\/|$)/.test(entry))
    .sort()
    .map(entry => {
      const kept = keepReason(path.join(root, entry), worktreePaths);
      return kept ? {path: entry, action: 'keep', reason: kept} : {path: entry, action: 'delete', reason: 'build output'};
    });
}

/** Returns keep/delete decisions for ignored outputs in the checkout at root. */
function planArtifacts(root, {build = false, worktreePaths = []} = {}) {
  const entries = [];
  const phases = fs.readdirSync(root).filter(name => /^\.phase\d+$/.test(name)).sort();
  for (const name of phases) {
    const durable = name === '.phase1' ? DURABLE_PHASE1 : new Set();
    entries.push(...classifyRoot(root, name, 'phase output', durable, worktreePaths));
  }
  entries.push(...classifyRoot(root, '.planning/tmp', 'workflow scratch', new Set(), worktreePaths));
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
  fs.rmSync(target, {recursive: true, force: true, maxRetries: 3});
}

/** Removes a worktree; links (such as junctions to shared caches) are unlinked, never followed. */
function removeWorktree(root, worktreePath) {
  git(root, ['worktree', 'remove', worktreePath]);
  // Git leaves folders it cannot fully delete; Node's rmSync unlinks junctions and symlinks without following them.
  if (fs.existsSync(worktreePath)) fs.rmSync(worktreePath, {recursive: true, force: true, maxRetries: 3});
  if (fs.existsSync(worktreePath)) throw new Error('folder still exists after removal');
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
  let failures = 0;

  log(`Workspace cleanup ${options.apply ? '(apply)' : '(dry run)'}: ${root}`);
  const lock = findLock(root);
  if (lock) {
    error(`  ${lock} exists: a runner is active or stopped uncleanly. Inspect it; nothing is deleted while it exists.`);
    if (options.apply) return 1;
  }
  const refused = requested.filter(worktree => !worktree.removable);
  for (const worktree of refused) error(`  refused   ${worktree.path} (${worktree.reason})`);
  if (options.apply && refused.length) {
    error('  nothing was deleted because a requested worktree was refused');
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

  const pending = artifacts.filter(entry => entry.action === 'delete').length;
  if (!options.apply && lock) log(`Dry run: --apply would delete nothing while ${lock} exists; ${pending} output(s) are listed for deletion after that.`);
  else if (!options.apply) log(`Dry run: ${pending} output(s) would be deleted. Rerun with --apply to delete them.`);
  return failures ? 1 : 0;
}

module.exports = {parseArgs, parseWorktrees, planArtifacts, planWorktrees, run, UsageError};

if (require.main === module) process.exitCode = run(process.argv.slice(2));
