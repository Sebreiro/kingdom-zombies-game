'use strict';
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const assert = require('node:assert/strict');
const {execFileSync} = require('node:child_process');
const cleanup = require('./workspace-cleanup.cjs');

const IGNORE = ['/.phase1/', '/.phase2/', '/.phase4/', '**/bin/', '**/obj/', '**/.godot/', '/.planning/tmp/', ''].join('\n');

function git(cwd, ...args) {
  return execFileSync('git', args, {cwd, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe']});
}

function write(root, relative, text = 'x') {
  fs.mkdirSync(path.dirname(path.join(root, relative)), {recursive: true});
  fs.writeFileSync(path.join(root, relative), text);
}

function exists(root, relative) {
  return fs.existsSync(path.join(root, relative));
}

/** Creates a committed repository with a bare origin; the test context deletes both afterwards. */
function repository(t) {
  const base = fs.mkdtempSync(path.join(os.tmpdir(), 'kz-cleanup-test-'));
  t.after(() => fs.rmSync(base, {recursive: true, force: true, maxRetries: 3}));
  const origin = path.join(base, 'origin.git'), root = path.join(base, 'repo');
  git(base, 'init', '-q', '--bare', origin);
  git(base, 'init', '-q', '-b', 'dev', root);
  git(root, 'config', 'user.email', 'test@example.invalid');
  git(root, 'config', 'user.name', 'Cleanup Test');
  git(root, 'config', 'core.autocrlf', 'false');
  write(root, '.gitignore', IGNORE);
  write(root, 'README.md', 'fixture\n');
  git(root, 'add', '.');
  git(root, 'commit', '-q', '-m', 'fixture');
  git(root, 'remote', 'add', 'origin', origin);
  git(root, 'push', '-q', 'origin', 'dev');
  return {base, root};
}

function invoke(cwd, ...argv) {
  const lines = [], errors = [];
  const code = cleanup.run(argv, {cwd, log: line => lines.push(line), error: line => errors.push(line)});
  return {code, out: lines.join('\n'), err: errors.join('\n')};
}

function addWorktree(context, name) {
  const target = path.join(context.base, name);
  git(context.root, 'worktree', 'add', '-q', '-b', `feature/${name}`, target);
  return target;
}

test('dry run lists task outputs, keeps pinned caches and deletes nothing', t => {
  const {root} = repository(t);
  write(root, '.phase1/cache/godot.zip');
  write(root, '.phase1/tools/linux-vm/disk.img');
  write(root, '.phase1/runs/a/summary.json');
  write(root, '.phase4/checkpoints/m.json');
  write(root, '.planning/tmp/init.json');
  const result = invoke(root);
  assert.equal(result.code, 0);
  assert.match(result.out, /delete\s+\.phase1\/runs/);
  assert.match(result.out, /delete\s+\.phase4\/checkpoints/);
  assert.match(result.out, /delete\s+\.planning\/tmp\/init\.json/);
  assert.match(result.out, /keep\s+\.phase1\/cache\s+\(pinned toolchain cache\)/);
  assert.match(result.out, /3 output\(s\) would be deleted/);
  for (const file of ['.phase1/runs/a/summary.json', '.phase4/checkpoints/m.json', '.planning/tmp/init.json']) {
    assert.ok(exists(root, file), file);
  }
});

test('apply deletes outputs and keeps pinned caches, tracked roots and roots Git does not ignore', t => {
  const {root} = repository(t);
  write(root, '.phase1/packages/p.nupkg');
  write(root, '.phase1/temp/scratch.txt');
  write(root, '.phase1/phase4-pr-body.md');
  write(root, '.phase4/runs/r/log.txt');
  write(root, '.phase3/runs/r/log.txt');
  write(root, '.phase2/kept.txt');
  fs.appendFileSync(path.join(root, '.gitignore'), '!/.phase2/\n');
  git(root, 'add', '.gitignore', '.phase2/kept.txt');
  git(root, 'commit', '-q', '-m', 'tracked output root');
  fs.writeFileSync(path.join(root, '.gitignore'), IGNORE);
  const result = invoke(root, '--apply');
  assert.equal(result.code, 0, result.err);
  assert.ok(!exists(root, '.phase1/temp'));
  assert.ok(!exists(root, '.phase1/phase4-pr-body.md'));
  assert.ok(!exists(root, '.phase4/runs'));
  assert.ok(exists(root, '.phase1/packages/p.nupkg'));
  assert.ok(exists(root, '.phase3/runs/r/log.txt'), '.phase3 is not ignored in this fixture');
  assert.match(result.out, /keep\s+\.phase3\s+\(not ignored by Git\)/);
  assert.ok(exists(root, '.phase2/kept.txt'));
  assert.match(result.out, /keep\s+\.phase2\s+\(contains tracked files\)/);
});

test('build outputs are deleted only with --build', t => {
  const {root} = repository(t);
  write(root, 'src/game/bin/Debug/a.dll');
  write(root, 'src/game/obj/project.assets.json');
  write(root, 'src/game/.godot/imported/x.ctex');
  write(root, '.phase1/tools/sdk/bin/tool.dll');
  invoke(root, '--apply');
  assert.ok(exists(root, 'src/game/bin/Debug/a.dll'));
  const result = invoke(root, '--apply', '--build');
  assert.equal(result.code, 0, result.err);
  for (const directory of ['src/game/bin', 'src/game/obj', 'src/game/.godot']) assert.ok(!exists(root, directory), directory);
  assert.ok(exists(root, '.phase1/tools/sdk/bin/tool.dll'));
});

test('a clean pushed worktree is removed only when named', t => {
  const context = repository(t);
  const target = addWorktree(context, 'pushed');
  write(target, 'change.txt');
  write(target, '.phase4/runs/r/log.txt');
  git(target, 'add', 'change.txt');
  git(target, 'commit', '-q', '-m', 'work');
  git(target, 'push', '-q', 'origin', 'feature/pushed');
  const listed = invoke(context.root, '--apply');
  assert.match(listed.out, /removable\s+.*pushed.*\(clean and pushed\)/);
  assert.ok(fs.existsSync(target), 'unnamed worktrees stay');
  const removed = invoke(context.root, '--apply', '--remove-worktree', target);
  assert.equal(removed.code, 0, removed.err);
  assert.ok(!fs.existsSync(target));
  assert.match(git(context.root, 'branch', '--list', 'feature/pushed'), /feature\/pushed/, 'the branch survives');
});

test('dirty, unpushed and current worktrees are refused', t => {
  const context = repository(t);
  const dirty = addWorktree(context, 'dirty');
  write(dirty, 'untracked.txt');
  const unpushed = addWorktree(context, 'unpushed');
  write(unpushed, 'local.txt');
  git(unpushed, 'add', 'local.txt');
  git(unpushed, 'commit', '-q', '-m', 'local only');
  const result = invoke(context.root, '--apply', '--remove-worktree', dirty, '--remove-worktree', unpushed);
  assert.equal(result.code, 1);
  assert.match(result.err, /refused.*dirty.*\(uncommitted changes\)/);
  assert.match(result.err, /refused.*unpushed.*\(commits not on any remote branch\)/);
  assert.ok(fs.existsSync(dirty) && fs.existsSync(unpushed));
  const self = invoke(dirty, '--remove-worktree', dirty);
  assert.match(self.err, /\(current worktree\)/);
  const main = invoke(dirty, '--remove-worktree', context.root);
  assert.match(main.err, /\(main checkout\)/);
});

test('apply prunes records of deleted worktree directories', t => {
  const context = repository(t);
  const target = addWorktree(context, 'missing');
  fs.rmSync(target, {recursive: true, force: true});
  const dry = invoke(context.root);
  assert.match(dry.out, /prune\s+.*missing/);
  assert.match(git(context.root, 'worktree', 'list'), /missing/);
  invoke(context.root, '--apply');
  assert.doesNotMatch(git(context.root, 'worktree', 'list'), /missing/);
});

test('arguments are validated', t => {
  const {root} = repository(t);
  assert.equal(invoke(root, '--force').code, 2);
  assert.equal(invoke(root, '--remove-worktree').code, 2);
  assert.match(invoke(root, '--remove-worktree', 'nowhere').err, /not a worktree of this repository/);
  assert.equal(invoke(root, '--help').code, 0);
  assert.throws(() => cleanup.parseArgs(['--remove-worktree', '--apply']), cleanup.UsageError);
});

test('a worktree inside an output root is kept with its work', t => {
  const context = repository(t);
  const nested = path.join(context.root, '.phase4', 'wt');
  git(context.root, 'worktree', 'add', '-q', '-b', 'feature/nested', nested);
  write(nested, 'precious.txt');
  write(context.root, '.phase4/runs/r.txt');
  const result = invoke(context.root, '--apply');
  assert.equal(result.code, 0, result.err);
  assert.match(result.out, /keep\s+\.phase4\/wt\s+\(contains a Git worktree or repository\)/);
  assert.ok(exists(nested, 'precious.txt'));
  assert.ok(!exists(context.root, '.phase4/runs'));
});

test('untracked work is detected even when Git hides untracked files', t => {
  const context = repository(t);
  const target = addWorktree(context, 'hidden');
  git(target, 'push', '-q', 'origin', 'feature/hidden');
  git(context.root, 'config', 'status.showUntrackedFiles', 'no');
  write(target, 'new-work.txt');
  const result = invoke(context.root, '--apply', '--remove-worktree', target);
  assert.equal(result.code, 1);
  assert.match(result.err, /\(uncommitted changes\)/);
  assert.ok(exists(target, 'new-work.txt'));
});

test('removing a worktree unlinks junctions without touching their targets', t => {
  const context = repository(t);
  const shared = path.join(context.base, 'shared-cache');
  write(shared, 'godot.zip');
  const target = addWorktree(context, 'linked');
  git(target, 'push', '-q', 'origin', 'feature/linked');
  fs.mkdirSync(path.join(target, '.phase1'));
  fs.symlinkSync(shared, path.join(target, '.phase1', 'cache'), 'junction');
  const result = invoke(context.root, '--apply', '--remove-worktree', target);
  assert.equal(result.code, 0, result.err);
  assert.ok(!fs.existsSync(target), 'the worktree folder is gone');
  assert.ok(exists(shared, 'godot.zip'), 'the junction target survives');
});

test('pinned cache names match case-insensitively', t => {
  const {root} = repository(t);
  write(root, '.phase1/Tools/linux-vm/disk.img');
  invoke(root, '--apply');
  assert.ok(exists(root, '.phase1/Tools/linux-vm/disk.img'));
});

test('an active checker lock or a refused worktree stops all deletion', t => {
  const context = repository(t);
  write(context.root, '.phase4/runs/r.txt');
  write(context.root, '.phase1/check.lock');
  assert.equal(invoke(context.root, '--apply').code, 1);
  assert.ok(exists(context.root, '.phase4/runs/r.txt'));
  fs.rmSync(path.join(context.root, '.phase1/check.lock'));
  const result = invoke(context.root, '--apply', '--remove-worktree', 'nowhere');
  assert.equal(result.code, 1);
  assert.match(result.err, /nothing was deleted/);
  assert.ok(exists(context.root, '.phase4/runs/r.txt'));
});

test('Git failures stop with exit code 1', t => {
  const outside = fs.mkdtempSync(path.join(os.tmpdir(), 'kz-cleanup-test-'));
  t.after(() => fs.rmSync(outside, {recursive: true, force: true}));
  const result = invoke(outside);
  assert.equal(result.code, 1);
  assert.match(result.err, /cleanup stopped/);
});
