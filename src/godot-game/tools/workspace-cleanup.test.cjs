'use strict';
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const assert = require('node:assert/strict');
const {execFileSync, spawn} = require('node:child_process');
const cleanup = require('./workspace-cleanup.cjs');

// Every fixture repository resolves its own shared root from its Git common dir, never an inherited override.
delete process.env.KZ_SHARED_LOCAL;

const IGNORE = ['/.local/', '/.phase1/', '/.phase2/', '/.phase4/', '**/bin/', '**/obj/', '**/.godot/', '/.planning/tmp/', '']
  .join('\n');

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
  assert.match(result.out, /keep\s+\.phase1\/cache\s+\(legacy toolchain cache; run workspace-migrate\.cjs\)/);
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

test('--worktrees-only removes a worktree and leaves the current checkout outputs', t => {
  const context = repository(t);
  write(context.root, '.phase4/runs/r.txt');
  const target = addWorktree(context, 'only');
  git(target, 'push', '-q', 'origin', 'feature/only');
  const result = invoke(context.root, '--apply', '--worktrees-only', '--remove-worktree', target);
  assert.equal(result.code, 0, result.err);
  assert.ok(!fs.existsSync(target));
  assert.ok(exists(context.root, '.phase4/runs/r.txt'));
});

test('the .local root keeps tools, downloads and nuget and deletes runs, temp and every other entry', t => {
  const {root} = repository(t);
  write(root, '.local/tools/node/node.exe');
  write(root, '.local/downloads/godot/godot.zip');
  write(root, '.local/nuget/packages/p.nupkg');
  write(root, '.local/nuget/feed/f.nupkg');
  write(root, '.local/runs/verification/a/summary.json');
  write(root, '.local/temp/cli-home/x.txt');
  write(root, '.local/other/y.txt');
  const dry = invoke(root);
  assert.equal(dry.code, 0, dry.err);
  for (const name of ['tools', 'downloads', 'nuget']) {
    assert.match(dry.out, new RegExp('keep\\s+\\.local/' + name + '\\s+\\(shared toolchain, download or NuGet cache\\)'));
  }
  for (const name of ['runs', 'temp', 'other']) assert.match(dry.out, new RegExp('delete\\s+\\.local/' + name + '\\s'));
  assert.match(dry.out, /3 output\(s\) would be deleted/);
  assert.ok(exists(root, '.local/runs/verification/a/summary.json'), 'a dry run deletes nothing');
  const applied = invoke(root, '--apply');
  assert.equal(applied.code, 0, applied.err);
  for (const gone of ['.local/runs', '.local/temp', '.local/other']) assert.ok(!exists(root, gone), gone);
  for (const kept of ['.local/tools/node/node.exe', '.local/downloads/godot/godot.zip', '.local/nuget/packages/p.nupkg',
    '.local/nuget/feed/f.nupkg']) assert.ok(exists(root, kept), kept);
});

test('a lock in this checkout or in the shared root stops all deletion', t => {
  const context = repository(t);
  write(context.root, '.local/runs/r.txt');
  write(context.root, '.local/locks/check.lock');
  const local = invoke(context.root, '--apply');
  assert.equal(local.code, 1);
  assert.match(local.err, /check\.lock exists/);
  assert.ok(exists(context.root, '.local/runs/r.txt'));
  fs.rmSync(path.join(context.root, '.local/locks/check.lock'));
  const target = addWorktree(context, 'shared-lock');
  write(target, '.local/runs/w.txt');
  for (const name of ['desktop.lock', 'toolchain.lock']) {
    write(context.root, '.local/locks/' + name);
    const shared = invoke(target, '--apply');
    assert.equal(shared.code, 1, name);
    assert.match(shared.err, new RegExp(name.replace('.', '\\.') + ' exists'));
    assert.ok(exists(target, '.local/runs/w.txt'), name);
    assert.ok(exists(context.root, '.local/runs/r.txt'), name);
    fs.rmSync(path.join(context.root, '.local/locks/' + name));
  }
  const released = invoke(target, '--apply');
  assert.equal(released.code, 0, released.err);
  assert.ok(!exists(target, '.local/runs'));
  assert.ok(exists(context.root, '.local/runs/r.txt'), 'cleanup in a worktree never touches the shared root');
});

test('legacy folders keep their caches, delete the rest and their runner locks still block', t => {
  const {root} = repository(t);
  write(root, '.phase1/cache/godot.zip');
  write(root, '.phase1/runs/a/summary.json');
  write(root, '.phase2/runs/b/summary.json');
  write(root, '.phase2/desktop.lock');
  const blocked = invoke(root, '--apply');
  assert.equal(blocked.code, 1);
  assert.match(blocked.err, /desktop\.lock exists/);
  assert.ok(exists(root, '.phase1/runs/a/summary.json'));
  fs.rmSync(path.join(root, '.phase2/desktop.lock'));
  const applied = invoke(root, '--apply');
  assert.equal(applied.code, 0, applied.err);
  assert.match(applied.out, /keep\s+\.phase1\/cache\s+\(legacy toolchain cache; run workspace-migrate\.cjs\)/);
  assert.ok(exists(root, '.phase1/cache/godot.zip'));
  assert.ok(!exists(root, '.phase1/runs') && !exists(root, '.phase2/runs'));
});

test('the build filter skips build-named folders inside .local', t => {
  const {root} = repository(t);
  write(root, '.local/tools/sdk/bin/tool.dll');
  write(root, '.local/nuget/packages/p/obj/x.json');
  write(root, 'src/game/obj/project.assets.json');
  const result = invoke(root, '--apply', '--build');
  assert.equal(result.code, 0, result.err);
  assert.ok(!exists(root, 'src/game/obj'));
  assert.ok(exists(root, '.local/tools/sdk/bin/tool.dll'));
  assert.ok(exists(root, '.local/nuget/packages/p/obj/x.json'));
});

test('removing a worktree never follows a link into the shared root', t => {
  const context = repository(t);
  write(context.root, '.local/tools/node/node.exe');
  const target = addWorktree(context, 'shared-link');
  git(target, 'push', '-q', 'origin', 'feature/shared-link');
  fs.mkdirSync(path.join(target, '.local'));
  fs.symlinkSync(path.join(context.root, '.local', 'tools'), path.join(target, '.local', 'tools'), 'junction');
  const result = invoke(context.root, '--apply', '--worktrees-only', '--remove-worktree', target);
  assert.equal(result.code, 0, result.err);
  assert.ok(!fs.existsSync(target));
  assert.ok(exists(context.root, '.local/tools/node/node.exe'), 'the shared root survives');
});

test('cleanup keeps every output reached through a link and never deletes through one', t => {
  const {base, root} = repository(t);
  const outside = path.join(base, 'outside');
  for (const [relative, file] of [['temp', 'temp/scratch.txt'], ['planning', 'planning/tmp/state.json'], ['local', 'local/runs/r.txt']])
    write(outside, file, relative);
  try {
    fs.mkdirSync(path.join(root, '.phase1'));
    fs.symlinkSync(path.join(outside, 'temp'), path.join(root, '.phase1', 'temp'), 'junction');
    fs.symlinkSync(path.join(outside, 'planning'), path.join(root, '.planning'), 'junction');
  } catch {
    t.skip('this OS cannot create a junction or symbolic link');
    return;
  }
  const result = invoke(root, '--apply');
  assert.equal(result.code, 0, result.err);
  assert.match(result.out, /keep\s+\.phase1\/temp\s+\(\.phase1\/temp is a link\)/);
  assert.match(result.out, /keep\s+\.planning\/tmp\s+\(\.planning is a link\)/);
  assert.ok(fs.existsSync(path.join(outside, 'temp/scratch.txt')), 'the linked legacy entry keeps its target');
  assert.ok(fs.existsSync(path.join(outside, 'planning/tmp/state.json')), 'a linked ancestor keeps its target');
  fs.symlinkSync(path.join(outside, 'local'), path.join(root, '.local'), 'junction');
  const local = invoke(root, '--apply');
  assert.equal(local.code, 0, local.err);
  assert.match(local.out, /keep\s+\.local\s+\(\.local is a link\)/);
  assert.ok(fs.existsSync(path.join(outside, 'local/runs/r.txt')), 'a linked .local keeps its target');
});

/** Starts a process whose working directory is folder, as a build server's is; the returned function stops it. */
function hold(t, folder) {
  const child = spawn(process.execPath, ['-e', 'setInterval(() => {}, 1000)'], {cwd: folder, stdio: 'ignore', windowsHide: true});
  const exited = new Promise(resolve => child.once('exit', resolve));
  const release = () => { child.kill(); return exited; };
  // A failed assertion must not leave the process holding the fixture or keeping the test run alive.
  child.unref();
  process.once('exit', () => child.kill());
  t.after(release);
  return release;
}

test('a worktree in use keeps its record and folder until the process stops', async t => {
  if (process.platform !== 'win32') {
    t.skip('only Windows refuses to delete or move a folder another process works in');
    return;
  }
  const context = repository(t);
  const listed = target => git(context.root, 'worktree', 'list', '--porcelain').includes(target.split(path.sep).join('/'));
  const outputs = addWorktree(context, 'busy-output');
  git(outputs, 'push', '-q', 'origin', 'feature/busy-output');
  write(outputs, '.local/temp/held/x.txt');
  let release = hold(t, path.join(outputs, '.local', 'temp', 'held'));
  const held = invoke(context.root, '--apply', '--worktrees-only', '--remove-worktree', outputs);
  assert.equal(held.code, 1);
  assert.match(held.err, /failed\s+.*busy-output \(could not delete \.local .*dotnet build-server shutdown/);
  assert.ok(listed(outputs), 'the worktree record stays');
  assert.ok(exists(outputs, '.git') && exists(outputs, 'README.md'), 'Git has not touched the folder');
  await release();
  assert.equal(invoke(context.root, '--apply', '--worktrees-only', '--remove-worktree', outputs).code, 0);
  assert.ok(!fs.existsSync(outputs) && !listed(outputs));

  const tracked = addWorktree(context, 'busy-source');
  write(tracked, 'src/a.txt');
  git(tracked, 'add', 'src/a.txt');
  git(tracked, 'commit', '-q', '-m', 'source');
  git(tracked, 'push', '-q', 'origin', 'feature/busy-source');
  release = hold(t, path.join(tracked, 'src'));
  const moved = invoke(context.root, '--apply', '--worktrees-only', '--remove-worktree', tracked);
  assert.equal(moved.code, 1);
  assert.match(moved.err, /failed\s+.*busy-source \(the folder cannot be moved/);
  assert.ok(listed(tracked) && exists(tracked, '.git') && exists(tracked, 'src/a.txt'), 'record and folder stay together');
  await release();
  assert.equal(invoke(context.root, '--apply', '--worktrees-only', '--remove-worktree', tracked).code, 0);
  assert.ok(!fs.existsSync(tracked) && !listed(tracked));
});

test('an orphaned task folder is removed only when every remaining file is pushed or ignored', t => {
  const context = repository(t);
  const home = path.join(context.base, 'worktrees', 'repo');
  git(context.root, 'config', 'worktrees.root', path.join(context.base, 'worktrees'));
  const target = path.join(home, 'kz-orphan');
  git(context.root, 'worktree', 'add', '-q', '-b', 'feature/orphan', target);
  write(target, 'src/a.txt', 'pushed\n');
  git(target, 'add', 'src/a.txt');
  git(target, 'commit', '-q', '-m', 'work');
  git(target, 'push', '-q', 'origin', 'feature/orphan');
  write(target, '.local/temp/VBCSCompiler/held.dll');
  const shared = path.join(context.base, 'shared-tools');
  write(shared, 'node.exe');
  let linked = true;
  try {
    fs.symlinkSync(shared, path.join(target, '.local', 'tools'), 'junction');
  } catch {
    linked = false;
  }
  // What a failed `git worktree remove` leaves: no record, no .git file, some tracked files already deleted.
  fs.rmSync(path.join(target, '.git'));
  fs.rmSync(path.join(target, '.gitignore'));
  git(context.root, 'worktree', 'prune');

  const dry = invoke(context.root);
  assert.equal(dry.code, 0, dry.err);
  assert.match(dry.out, /orphan\s+.*kz-orphan\s+\(no worktree record; remove with --remove-orphan\)/);
  const refused = (argument, pattern) => {
    const result = invoke(context.root, '--apply', '--worktrees-only', '--remove-orphan', argument);
    assert.equal(result.code, 1, argument);
    assert.match(result.err, pattern);
    assert.match(result.err, /nothing was deleted/);
    assert.ok(exists(target, 'src/a.txt'), 'a refused orphan stays');
  };
  write(target, 'src/a.txt', 'changed\n');
  refused(target, /refused\s+.*kz-orphan \(1 file\(s\) match no remote branch, such as src\/a\.txt\)/);
  write(target, 'src/a.txt', 'pushed\n');
  write(target, 'notes.txt', 'unpushed work');
  refused(target, /refused\s+.*kz-orphan \(1 file\(s\) match no remote branch, such as notes\.txt\)/);
  fs.rmSync(path.join(target, 'notes.txt'));
  git(target, 'init', '-q', 'nested');
  refused(target, /refused\s+.*kz-orphan \(contains a Git worktree or repository at nested\)/);
  fs.rmSync(path.join(target, 'nested'), {recursive: true, force: true});
  fs.mkdirSync(path.join(context.base, 'elsewhere'));
  refused(path.join(context.base, 'elsewhere'), /not a task worktree folder in /);
  const registered = addWorktree(context, 'registered');
  refused(registered, /a registered worktree; use --remove-worktree/);

  const result = invoke(context.root, '--apply', '--worktrees-only', '--remove-orphan', target);
  assert.equal(result.code, 0, result.err);
  assert.match(result.out, /removed orphan\s+.*kz-orphan \(matches origin\/feature\/orphan\)/);
  assert.ok(!fs.existsSync(target));
  if (linked) assert.ok(exists(shared, 'node.exe'), 'a junction inside the orphan is unlinked, never followed');
});

/** Makes the pushed worktree at target an orphan as a failed `git worktree remove` leaves it: no record, no .git file. */
function orphanize(context, target) {
  fs.rmSync(path.join(target, '.git'));
  git(context.root, 'worktree', 'prune');
}

/** A task worktree folder home with one pushed worktree on branch; returns its path. */
function taskWorktree(context, name, branch) {
  git(context.root, 'config', 'worktrees.root', path.join(context.base, 'worktrees'));
  const target = path.join(context.base, 'worktrees', 'repo', name);
  git(context.root, 'worktree', 'add', '-q', '-b', branch, target);
  return target;
}

/** Runs the command and calls onHeader once its plan is printed, before anything is deleted or removed. */
function invokeAfterPlan(cwd, onHeader, ...argv) {
  const lines = [], errors = [];
  const log = line => {
    if (!lines.length) onHeader();
    lines.push(line);
  };
  const code = cleanup.run(argv, {cwd, log, error: line => errors.push(line)});
  return {code, out: lines.join('\n'), err: errors.join('\n')};
}

test('an orphan with no ignored file left is judged without stopping the run', t => {
  const context = repository(t);
  git(context.root, 'remote', 'set-head', 'origin', 'dev');
  const target = taskWorktree(context, 'kz-plain', 'feature/plain');
  git(target, 'push', '-q', 'origin', 'feature/plain');
  orphanize(context, target);
  const dry = invoke(context.root, '--worktrees-only', '--remove-orphan', target);
  assert.equal(dry.code, 0, dry.err);
  assert.match(dry.out, /would remove orphan\s+.*kz-plain \(matches origin\/(dev|feature\/plain)\)/);
  const result = invoke(context.root, '--apply', '--worktrees-only', '--remove-orphan', target);
  assert.equal(result.code, 0, result.err);
  assert.ok(!fs.existsSync(target));
});

test('an ignored path that a remote branch tracks must still match that branch', t => {
  const context = repository(t);
  const target = taskWorktree(context, 'kz-tracked-bin', 'feature/tracked-bin');
  write(target, 'tools/bin/run.cjs', 'pushed\n');
  git(target, 'add', '-f', 'tools/bin/run.cjs');
  git(target, 'commit', '-q', '-m', 'tracked under an ignored pattern');
  git(target, 'push', '-q', 'origin', 'feature/tracked-bin');
  write(target, '.local/temp/out.txt');
  orphanize(context, target);
  write(target, 'tools/bin/run.cjs', 'edited, never pushed\n');
  const refused = invoke(context.root, '--apply', '--worktrees-only', '--remove-orphan', target);
  assert.equal(refused.code, 1);
  assert.match(refused.err, /kz-tracked-bin \(1 file\(s\) match no remote branch, such as tools\/bin\/run\.cjs\)/);
  assert.ok(exists(target, 'tools/bin/run.cjs'), 'the unpushed edit stays');
  write(target, 'tools/bin/run.cjs', 'pushed\n');
  const result = invoke(context.root, '--apply', '--worktrees-only', '--remove-orphan', target);
  assert.equal(result.code, 0, result.err);
  assert.match(result.out, /removed orphan\s+.*kz-tracked-bin \(matches origin\/feature\/tracked-bin\)/);
});

test('a runner lock that appears after planning stops the removal of a worktree or orphan', t => {
  const context = repository(t);
  const listed = target => git(context.root, 'worktree', 'list', '--porcelain').includes(target.split(path.sep).join('/'));
  const worktree = addWorktree(context, 'late-lock');
  git(worktree, 'push', '-q', 'origin', 'feature/late-lock');
  const late = invokeAfterPlan(context.root, () => write(worktree, '.local/locks/check.lock'),
    '--apply', '--worktrees-only', '--remove-worktree', worktree);
  assert.equal(late.code, 1);
  assert.match(late.err, /failed\s+.*late-lock \(runner lock at \.local\/locks\/check\.lock/);
  assert.ok(listed(worktree) && exists(worktree, '.local/locks/check.lock'), 'record, folder and lock stay');

  const orphan = taskWorktree(context, 'kz-late-orphan', 'feature/late-orphan');
  git(orphan, 'push', '-q', 'origin', 'feature/late-orphan');
  orphanize(context, orphan);
  const lateOrphan = invokeAfterPlan(context.root, () => write(orphan, '.local/locks/check.lock'),
    '--apply', '--worktrees-only', '--remove-orphan', orphan);
  assert.equal(lateOrphan.code, 1);
  assert.match(lateOrphan.err, /failed\s+.*kz-late-orphan \(runner lock at \.local\/locks\/check\.lock/);
  assert.ok(exists(orphan, '.local/locks/check.lock') && exists(orphan, 'README.md'), 'the orphan and its lock stay');
});

test('a probe that cannot move the folder back keeps the worktree record locked', t => {
  const context = repository(t);
  const worktree = addWorktree(context, 'stuck-probe');
  git(worktree, 'push', '-q', 'origin', 'feature/stuck-probe');
  const rename = fs.renameSync;
  t.after(() => { fs.renameSync = rename; });
  fs.renameSync = (from, to) => {
    if (String(from).includes('.cleanup-probe-')) throw Object.assign(new Error('simulated busy folder'), {code: 'EBUSY'});
    return rename(from, to);
  };
  const result = invoke(context.root, '--apply', '--worktrees-only', '--remove-worktree', worktree);
  fs.renameSync = rename;
  assert.equal(result.code, 1);
  assert.match(result.err, /could not move it back .*git worktree unlock/);
  const aside = `${worktree}.cleanup-probe-${process.pid}`;
  assert.ok(fs.existsSync(aside) && !fs.existsSync(worktree), 'the folder is still aside');
  git(context.root, 'worktree', 'prune');
  const record = git(context.root, 'worktree', 'list', '--porcelain').split(/\r?\n\r?\n/)
    .find(block => block.includes(worktree.split(path.sep).join('/')));
  assert.ok(record && /^locked/m.test(record), 'prune keeps the locked record of the folder that is aside');
  fs.renameSync(aside, worktree);
  git(context.root, 'worktree', 'unlock', worktree);
  assert.equal(invoke(context.root, '--apply', '--worktrees-only', '--remove-worktree', worktree).code, 0);
  assert.ok(!fs.existsSync(worktree));
});

test('the migration reuses the exported safety helpers', () => {
  for (const name of ['keepReason', 'isIgnored', 'hasTrackedFiles', 'isRealDirectory']) {
    assert.equal(typeof cleanup[name], 'function', name);
  }
});
