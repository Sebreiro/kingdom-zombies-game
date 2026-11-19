'use strict';
// Lists and removes ignored task outputs and finished Git worktrees. Nothing is deleted without --apply.
// Rules: docs/workflow/cleanup.md
const fs = require('node:fs');
const path = require('node:path');
const {execFileSync} = require('node:child_process');

const DURABLE_PHASE1 = new Set(['cache', 'cli-home', 'dotnet-home', 'feed', 'http-cache', 'packages', 'tools']);
const BUILD_NAMES = new Set(['bin', 'obj', '.godot']);
const USAGE = `Usage: node tools/workspace-cleanup.cjs [--apply] [--build] [--remove-worktree <path>]...

Without --apply, only reports what would be deleted.
  --apply                   Delete the listed task outputs and prune missing worktrees.
  --build                   Also delete ignored bin/, obj/ and .godot/ build outputs.
  --remove-worktree <path>  Remove this linked worktree if it is clean and pushed (repeatable).`;

class UsageError extends Error {}

function git(cwd, args) {
  return execFileSync('git', args, {cwd, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe']});
}

function parseArgs(argv) {
  const options = {apply: false, build: false, help: false, worktrees: []};
  for (let index = 0; index < argv.length; index++) {
    const argument = argv[index];
    if (argument === '--apply') options.apply = true;
    else if (argument === '--build') options.build = true;
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

/** Classifies children of one ignored output root; unsafe roots and children are kept whole. */
function classifyRoot(root, relative, reason, durable, worktreePaths) {
  if (!isRealDirectory(path.join(root, relative))) return [];
  if (hasTrackedFiles(root, relative)) return [{path: relative, action: 'keep', reason: 'contains tracked files'}];
  if (!isIgnored(root, relative + '/')) return [{path: relative, action: 'keep', reason: 'not ignored by Git'}];
  return fs.readdirSync(path.join(root, relative)).sort().map(name => {
    const child = `${relative}/${name}`, absolute = path.join(root, child);
    if (durable.has(name.toLowerCase())) return {path: child, action: 'keep', reason: 'pinned toolchain cache'};
    if (worktreePaths.some(worktree => isWithin(absolute, worktree)) || fs.existsSync(path.join(absolute, '.git'))) {
      return {path: child, action: 'keep', reason: 'contains a Git worktree or repository'};
    }
    return {path: child, action: 'delete', reason};
  });
}

function buildOutputs(root) {
  return git(root, ['ls-files', '-z', '--others', '--ignored', '--exclude-standard', '--directory'])
    .split('\0')
    .filter(entry => entry.endsWith('/'))
    .map(entry => entry.slice(0, -1))
    .filter(entry => BUILD_NAMES.has(entry.split('/').pop()) && !/^\.phase\d+(\/|$)/.test(entry))
    .sort()
    .map(entry => ({path: entry, action: 'delete', reason: 'build output'}));
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
  if (build) entries.push(...buildOutputs(root));
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

function worktreeState(root, worktree) {
  if (worktree.main) return {removable: false, reason: 'main checkout'};
  if (samePath(worktree.path, root)) return {removable: false, reason: 'current worktree'};
  if (worktree.locked) return {removable: false, reason: 'locked'};
  if (worktree.prunable || !fs.existsSync(worktree.path)) return {removable: false, prune: true, reason: 'directory missing'};
  const status = git(worktree.path, ['status', '--porcelain=v1', '--untracked-files=all', '--ignore-submodules=none']);
  if (status.trim()) return {removable: false, reason: 'uncommitted changes'};
  if (git(worktree.path, ['ls-files', '-v']).split(/\r?\n/).some(line => /^[a-z]/.test(line))) {
    return {removable: false, reason: 'files marked assume-unchanged or skip-worktree'};
  }
  const gitDirectory = path.resolve(worktree.path, git(worktree.path, ['rev-parse', '--git-dir']).trim());
  const operation = ['rebase-merge', 'rebase-apply', 'MERGE_HEAD', 'CHERRY_PICK_HEAD', 'REVERT_HEAD', 'BISECT_LOG']
    .find(name => fs.existsSync(path.join(gitDirectory, name)));
  if (operation) return {removable: false, reason: `operation in progress (${operation})`};
  if (!git(root, ['branch', '-r', '--contains', worktree.head]).trim()) {
    return {removable: false, reason: 'commits not on any remote branch'};
  }
  return {removable: true, reason: 'clean and pushed'};
}

/** Lists linked worktrees of the repository containing root with their removal state. */
function planWorktrees(root) {
  return parseWorktrees(git(root, ['worktree', 'list', '--porcelain']))
    .map(worktree => ({...worktree, ...worktreeState(root, worktree)}));
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
  const artifacts = planArtifacts(root, {...options, worktreePaths: worktrees.map(worktree => worktree.path)});
  const requested = options.worktrees.map(value => {
    const match = worktrees.find(worktree => samePath(worktree.path, path.resolve(cwd, value)));
    return match ?? {path: path.resolve(cwd, value), removable: false, reason: 'not a worktree of this repository'};
  });
  let failures = 0;

  log(`Workspace cleanup ${options.apply ? '(apply)' : '(dry run)'}: ${root}`);
  const lock = path.join(root, '.phase1', 'check.lock');
  if (fs.existsSync(lock)) {
    error(`  ${lock} exists: a checker run is active or stopped uncleanly. Inspect it; nothing is deleted while it exists.`);
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
    log('  pruned missing worktree records');
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
  if (!options.apply) log(`Dry run: ${pending} output(s) would be deleted. Rerun with --apply to delete them.`);
  return failures ? 1 : 0;
}

module.exports = {parseArgs, parseWorktrees, planArtifacts, planWorktrees, run, UsageError};

if (require.main === module) process.exitCode = run(process.argv.slice(2));
