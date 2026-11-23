'use strict';
// Every repository here is temporary, under the system temp folder; no real checkout or shared root is touched.
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const crypto = require('node:crypto');
const {execFileSync} = require('node:child_process');
const migrate = require('./workspace-migrate.cjs');

// Fixtures resolve their own main checkout; an outer runner's override must not leak into them.
delete process.env.KZ_SHARED_LOCAL;

const digest = (data, algorithm = 'sha256') => crypto.createHash(algorithm).update(data).digest('hex');
const ZIP = Buffer.from('fixture engine archive'), EXE = Buffer.from('fixture editor'), TPZ = Buffer.from('fixture templates');
const PACKAGE = Buffer.from('fixture package');
const PINS = {
  archives: {'win32-x64': {file: 'Godot_fixture_win64.zip', sha256: digest(ZIP), sha512: digest(ZIP, 'sha512'),
    directory: 'Godot_fixture_win64', executable: 'godot_console.exe', executableSha256: digest(EXE)}},
  godotPackages: [{id: 'Fixture.Package', version: '1.0.0', sha256: digest(PACKAGE)}],
};
const EXPORT = {archive: {path: '.phase1/cache/phase4-exports/templates.tpz', bytes: TPZ.length, sha512: digest(TPZ, 'sha512'),
  checksums: '.phase1/cache/phase4-exports/SHA512-SUMS.txt'}};
const IGNORE = ['/.local/', '/.phase[0-9]*/'];
const NOW = () => new Date('2026-11-23T00:00:00.000Z');

function git(cwd, ...args) {
  return execFileSync('git', ['-c', 'user.name=t', '-c', 'user.email=t@example.invalid', '-c', 'commit.gpgsign=false', ...args],
    {cwd, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe']});
}

function write(root, relative, data = 'x') {
  const file = path.join(root, ...relative.split('/'));
  fs.mkdirSync(path.dirname(file), {recursive: true});
  fs.writeFileSync(file, data);
  return file;
}

function exists(root, relative) {
  return fs.existsSync(path.join(root, ...relative.split('/')));
}

function legacyTree(root) {
  write(root, '.phase1/tools/linux-vm/system.qcow2', Buffer.alloc(2048, 1));
  write(root, '.phase1/tools/linux-vm/id_ed25519', 'fixture private key');
  write(root, '.phase1/tools/linux-vm/processes.json', JSON.stringify({launcher: null, qemu: null, lastQemu: 4242}));
  write(root, '.phase1/cache/Godot_fixture_win64.zip', ZIP);
  write(root, '.phase1/cache/SHA512-SUMS.txt', `${digest(ZIP, 'sha512')}  Godot_fixture_win64.zip\n`);
  write(root, '.phase1/cache/game.packages.lock.json', '{}');
  write(root, '.phase1/cache/phase4-exports/templates.tpz', TPZ);
  write(root, '.phase1/cache/phase4-exports/SHA512-SUMS.txt', `${digest(TPZ, 'sha512')}  templates.tpz\n`);
  write(root, '.phase1/tools/Godot_fixture_win64/godot_console.exe', EXE);
  write(root, '.phase1/tools/linux-prep/node.tar.xz', 'fixture node archive');
  write(root, '.phase1/feed/Fixture.Package.1.0.0.nupkg', PACKAGE);
  write(root, '.phase1/packages/fixture.package/1.0.0/fixture.package.1.0.0.nupkg', PACKAGE);
  write(root, '.phase1/http-cache/v3/index.json', '{}');
  write(root, '.phase1/cli-home/.dotnet/state', 'x');
  write(root, '.phase1/dotnet-home/.dotnet/10.0.404.dotnetFirstUseSentinel', '');
  fs.mkdirSync(path.join(root, '.phase2'));
}

/** A committed main checkout with a fake legacy tree; the test context deletes it afterwards. */
function repository(t, {helper = true, ignore = IGNORE} = {}) {
  const base = fs.realpathSync.native(fs.mkdtempSync(path.join(os.tmpdir(), 'kz-migrate-test-')));
  t.after(() => fs.rmSync(base, {recursive: true, force: true, maxRetries: 3}));
  const root = path.join(base, 'repo');
  fs.mkdirSync(root);
  git(root, 'init', '-q', '-b', 'dev');
  git(root, 'config', 'core.autocrlf', 'false');
  write(root, '.gitignore', ignore.join('\n') + '\n');
  write(root, 'src/godot-game/tools/verification/toolchain.json', JSON.stringify(PINS));
  write(root, 'src/godot-game/production/assets/export-toolchain.json', JSON.stringify(EXPORT));
  if (helper) write(root, 'src/godot-game/tools/workspace-paths.cjs', '// fixture\n');
  git(root, 'add', '.');
  git(root, 'commit', '-q', '-m', 'fixture');
  legacyTree(root);
  return {base, root};
}

/** Names, sizes and modification times of everything below root except .git: the no-write witness. */
function snapshot(root) {
  const rows = [];
  const stack = [root];
  while (stack.length) {
    const current = stack.pop();
    for (const name of fs.readdirSync(current)) {
      if (current === root && name === '.git') continue;
      const child = path.join(current, name), stat = fs.lstatSync(child);
      rows.push(`${path.relative(root, child)} ${stat.isDirectory() ? 'dir' : stat.size} ${stat.mtimeMs}`);
      if (stat.isDirectory()) stack.push(child);
    }
  }
  return rows.sort();
}

function invoke(root, argv = [], options = {}) {
  const lines = [], errors = [];
  const code = migrate.run(argv, {root, env: {}, list: () => [], now: NOW, log: line => lines.push(line),
    error: line => errors.push(line), ...options});
  return {code, lines, out: lines.join('\n'), err: errors.join('\n')};
}

const firstRow = result => result.lines.find(line => /^(delete|move|skip) /.test(line));

test('the dry run lists the VM deletion first with its size, every row, and writes nothing', t => {
  const {root} = repository(t);
  const before = snapshot(root), status = git(root, 'status', '--porcelain');
  const result = invoke(root);
  assert.equal(result.code, 0, result.out + result.err);
  assert.match(firstRow(result), /^delete \.phase1\/tools\/linux-vm \([0-9][0-9.]* KB; /);
  for (const row of [
    'move .phase1/cache/Godot_fixture_win64.zip -> .local/downloads/godot/Godot_fixture_win64.zip (',
    'move .phase1/cache/SHA512-SUMS.txt -> .local/downloads/godot/SHA512-SUMS.txt (',
    'move .phase1/cache/phase4-exports -> .local/downloads/export-templates (',
    'move .phase1/tools/Godot_fixture_win64 -> .local/tools/godot/Godot_fixture_win64 (',
    'move .phase1/tools/linux-prep -> .local/downloads/linux-prep (',
    'move .phase1/feed -> .local/nuget/feed (',
    'move .phase1/packages -> .local/nuget/packages (',
    'move .phase1/http-cache -> .local/nuget/http (',
    'delete .phase1/cli-home (',
    'delete .phase1/cache/game.packages.lock.json (',
    'delete .phase1/dotnet-home (',
  ]) assert.ok(result.lines.some(line => line.startsWith(row)), row);
  assert.doesNotMatch(result.out, /^refuse:/m);
  assert.match(result.out, /Dry run: nothing was changed/);
  assert.deepEqual(snapshot(root), before);
  assert.equal(git(root, 'status', '--porcelain'), status);
});

test('apply moves every row, verifies the pins, deletes the VM and a rerun changes nothing', t => {
  const {root} = repository(t);
  const result = invoke(root, ['--apply']);
  assert.equal(result.code, 0, result.out + result.err);
  for (const gone of ['.phase1', '.phase2']) assert.ok(!exists(root, gone), gone);
  assert.deepEqual(fs.readFileSync(path.join(root, '.local/downloads/godot/Godot_fixture_win64.zip')), ZIP);
  for (const kept of ['.local/downloads/godot/SHA512-SUMS.txt', '.local/downloads/export-templates/templates.tpz',
    '.local/tools/godot/Godot_fixture_win64/godot_console.exe', '.local/downloads/linux-prep/node.tar.xz',
    '.local/nuget/feed/Fixture.Package.1.0.0.nupkg', '.local/nuget/packages/fixture.package/1.0.0/fixture.package.1.0.0.nupkg',
    '.local/nuget/http/v3/index.json']) assert.ok(exists(root, kept), kept);
  assert.ok(!exists(root, '.local/tools/linux-vm'), 'the VM is deleted, never moved');
  assert.ok(!snapshot(root).some(row => row.includes('id_ed25519')), 'the VM key is copied nowhere');
  const before = snapshot(root);
  const rerun = invoke(root, ['--apply']);
  assert.equal(rerun.code, 0, rerun.out + rerun.err);
  assert.match(firstRow(rerun), /^skip \.phase1\/tools\/linux-vm \(already deleted\)/);
  assert.match(rerun.out, /^skip \.phase1\/feed -> \.local\/nuget\/feed \(already migrated\)$/m);
  assert.deepEqual(snapshot(root), before);
});

test('the main checkout is found through the Git common dir from a linked worktree', t => {
  const {base, root} = repository(t);
  const worktree = path.join(base, 'linked');
  git(root, 'worktree', 'add', '-q', '-b', 'feature/linked', worktree);
  const planned = migrate.plan({root: worktree, env: {}, list: () => []});
  assert.equal(planned.main.toLowerCase(), root.toLowerCase());
  assert.ok(planned.rows.find(row => row.legacy === '.phase1/feed').target.startsWith(path.join(root, '.local')));
});

test('the migration refuses while the main checkout HEAD lacks the workspace helper', t => {
  const {root} = repository(t, {helper: false});
  const before = snapshot(root);
  const result = invoke(root, ['--apply']);
  assert.equal(result.code, 1);
  assert.match(result.out, /^refuse:.*workspace-paths\.cjs/m);
  assert.match(firstRow(result), /^delete \.phase1\/tools\/linux-vm \([0-9]/);
  assert.deepEqual(snapshot(root), before);
});

test('the migration refuses unless the main checkout .gitignore ignores .local/', t => {
  const {root} = repository(t, {ignore: ['/.phase[0-9]*/']});
  fs.appendFileSync(path.join(root, '.git', 'info', 'exclude'), '/.local/\n');
  const result = invoke(root);
  assert.equal(result.code, 1);
  assert.match(result.out, /^refuse: \.local\/ is not ignored by the main checkout's \.gitignore/m);
});

test('the migration refuses an override other than the main checkout .local', t => {
  const {base, root} = repository(t);
  const other = invoke(root, [], {env: {KZ_SHARED_LOCAL: path.join(base, 'elsewhere')}});
  assert.equal(other.code, 1);
  assert.match(other.out, /^refuse: KZ_SHARED_LOCAL names a folder other than/m);
  assert.equal(invoke(root, [], {env: {KZ_SHARED_LOCAL: path.join(root, '.local')}}).code, 0);
});

test('the migration refuses while a legacy or new lock exists in any worktree', t => {
  const {base, root} = repository(t);
  const worktree = path.join(base, 'locked');
  git(root, 'worktree', 'add', '-q', '-b', 'feature/locked', worktree);
  for (const [where, lock] of [[root, '.phase1/check.lock'], [root, '.phase2/desktop.lock'], [root, '.local/locks/toolchain.lock'],
    [worktree, '.local/locks/check.lock']]) {
    const file = write(where, lock);
    const before = snapshot(root);
    const result = invoke(root, ['--apply']);
    assert.equal(result.code, 1, lock);
    assert.match(result.out, new RegExp(`^refuse: runner lock ${path.basename(lock).replace('.', '\\.')} exists`, 'm'), lock);
    assert.deepEqual(snapshot(root), before, lock);
    fs.rmSync(file);
  }
});

test('the migration refuses while a process runs from the legacy folder or the legacy VM is live', t => {
  const {root} = repository(t);
  const editor = path.join(root, '.phase1', 'tools', 'Godot_fixture_win64', 'godot_console.exe');
  const byPath = invoke(root, [], {list: () => [{pid: 99001, executable: editor, commandLine: ''}]});
  assert.equal(byPath.code, 1);
  assert.match(byPath.out, /^refuse: process 99001 \(godot_console\.exe\) runs from a legacy folder/m);
  const byCommand = invoke(root, [], {list: () => [{pid: 99002, executable: 'dotnet', commandLine: `dotnet "${path.join(root,
    '.phase1', 'packages', 'x.dll')}"`}]});
  assert.match(byCommand.out, /^refuse: process 99002 \(dotnet\) runs from a legacy folder/m);
  write(root, '.phase1/tools/linux-vm/processes.json', JSON.stringify({launcher: null, qemu: 4243}));
  const vm = invoke(root, [], {list: () => [{pid: 4243, executable: 'other', commandLine: ''}]});
  assert.match(vm.out, /^refuse: the legacy VM qemu process 4243 is live/m);
  assert.equal(invoke(root, [], {list: () => [{pid: 4242, executable: 'other', commandLine: ''}]}).code, 0,
    'a stale last-known PID is not a live recorded process');
  const unavailable = invoke(root, [], {list: () => { throw new Error('lister unavailable'); }});
  assert.match(unavailable.out, /^refuse: could not list processes \(lister unavailable\)/m);
});

test('the migration refuses a linked, tracked, unignored or repository-holding source', t => {
  const linked = repository(t);
  const outside = path.join(linked.base, 'outside');
  write(outside, 'v3/index.json', '{}');
  fs.rmSync(path.join(linked.root, '.phase1', 'http-cache'), {recursive: true});
  fs.symlinkSync(outside, path.join(linked.root, '.phase1', 'http-cache'), 'junction');
  assert.match(invoke(linked.root).out, /^refuse: \.phase1\/http-cache is a link or not a real file or folder/m);
  const tracked = repository(t);
  git(tracked.root, 'add', '-f', '.phase1/feed/Fixture.Package.1.0.0.nupkg');
  git(tracked.root, 'commit', '-q', '-m', 'tracked legacy file');
  assert.match(invoke(tracked.root).out, /^refuse: \.phase1\/feed contains tracked files/m);
  const unignored = repository(t, {ignore: ['/.local/', '/.phase1/*', '!/.phase1/feed/', '/.phase2/']});
  assert.match(invoke(unignored.root).out, /^refuse: \.phase1\/feed is not ignored by Git/m);
  const nested = repository(t);
  fs.mkdirSync(path.join(nested.root, '.phase1', 'tools', 'linux-prep', '.git'));
  assert.match(invoke(nested.root).out, /^refuse: \.phase1\/tools\/linux-prep contains a Git worktree or repository/m);
  write(nested.root, '.phase1/dotnet-home/notes.txt', 'data');
  assert.match(invoke(nested.root).out, /^refuse: \.phase1\/dotnet-home is not empty: it holds data/m);
});

test('an existing target must verify against its pin; an existing cache target makes the legacy copy a duplicate', t => {
  const {root} = repository(t);
  write(root, '.local/downloads/godot/Godot_fixture_win64.zip', 'not the pinned bytes');
  const wrong = invoke(root, ['--apply']);
  assert.equal(wrong.code, 1);
  assert.match(wrong.out, /^refuse: \.local\/downloads\/godot\/Godot_fixture_win64\.zip already exists and does not verify/m);
  assert.ok(exists(root, '.phase1/tools/linux-vm/system.qcow2'), 'a refusal deletes nothing');
  write(root, '.local/downloads/godot/Godot_fixture_win64.zip', ZIP);
  write(root, '.local/nuget/packages/other/1.0.0/other.nupkg', 'shared');
  const verified = invoke(root, ['--apply']);
  assert.equal(verified.code, 0, verified.out + verified.err);
  assert.match(verified.out, /^delete \.phase1\/cache\/Godot_fixture_win64\.zip \([^)]*duplicate of the verified/m);
  assert.match(verified.out, /^delete \.phase1\/packages \([^)]*duplicate regenerable cache/m);
  assert.ok(exists(root, '.local/nuget/packages/other/1.0.0/other.nupkg'), 'the shared cache is kept');
  assert.ok(!exists(root, '.local/nuget/packages/fixture.package'), 'the legacy duplicate is not merged in');
  assert.ok(!exists(root, '.phase1'));
});

test('a target parent that is not a folder is refused by the dry run, and --apply changes nothing', t => {
  const {root} = repository(t);
  write(root, '.local/downloads', 'a file where a folder belongs');
  const dry = invoke(root);
  assert.equal(dry.code, 1, dry.out);
  assert.match(dry.out, /^refuse: \.phase1\/cache\/Godot_fixture_win64\.zip: \.local\/downloads is not a folder/m);
  const before = snapshot(root);
  const result = invoke(root, ['--apply']);
  assert.equal(result.code, 1);
  assert.deepEqual(snapshot(root), before, 'a refusal deletes and moves nothing');
  assert.ok(exists(root, '.phase1/tools/linux-vm/system.qcow2'), 'the VM is kept');
});

test('a move source that fails its pin is refused by the dry run, before the VM deletion can run', t => {
  for (const [legacy, file, data] of [
    ['.phase1/cache/phase4-exports', '.phase1/cache/phase4-exports/templates.tpz', 'truncated templates'],
    ['.phase1/tools/Godot_fixture_win64', '.phase1/tools/Godot_fixture_win64/godot_console.exe', 'other editor'],
    ['.phase1/cache/Godot_fixture_win64.zip', '.phase1/cache/Godot_fixture_win64.zip', 'partial download'],
    ['.phase1/cache/SHA512-SUMS.txt', '.phase1/cache/SHA512-SUMS.txt', 'no matching line\n'],
    ['.phase1/feed', '.phase1/feed/Fixture.Package.1.0.0.nupkg', 'other package'],
  ]) {
    const {root} = repository(t);
    write(root, file, data);
    const dry = invoke(root);
    assert.equal(dry.code, 1, legacy);
    assert.match(dry.out, new RegExp(`^refuse: ${legacy.replaceAll('.', '\\.')} does not verify against its pin`, 'm'), legacy);
    const before = snapshot(root);
    assert.equal(invoke(root, ['--apply']).code, 1, legacy);
    assert.deepEqual(snapshot(root), before, legacy);
  }
});

test('apply verifies every row first, then moves, then deletes, and deletes the VM last', t => {
  const {root} = repository(t);
  const result = invoke(root, ['--apply']);
  assert.equal(result.code, 0, result.out + result.err);
  const steps = result.lines.filter(line => /^ {2}(moved|deleted) /.test(line) && !line.endsWith('(empty)'));
  const lastMove = steps.findLastIndex(line => line.startsWith('  moved'));
  const firstDelete = steps.findIndex(line => line.startsWith('  deleted'));
  assert.ok(lastMove >= 0 && firstDelete > lastMove, steps.join('\n'));
  assert.equal(steps.at(-1), '  deleted  .phase1/tools/linux-vm');
});

test('a rerun after an interrupted apply skips the completed rows and finishes', t => {
  const {root} = repository(t);
  const rename = fs.renameSync;
  let calls = 0;
  fs.renameSync = (...args) => {
    if (++calls === 3) throw Object.assign(new Error('simulated interruption'), {code: 'EIO'});
    return rename(...args);
  };
  let interrupted;
  try {
    interrupted = invoke(root, ['--apply']);
  } finally {
    fs.renameSync = rename;
  }
  assert.equal(interrupted.code, 1);
  assert.match(interrupted.err, /migration stopped: simulated interruption/);
  assert.ok(exists(root, '.phase1/tools/linux-vm/system.qcow2'), 'the VM deletion has not run');
  assert.ok(exists(root, '.local/downloads/godot/Godot_fixture_win64.zip'), 'the first move completed');
  const rerun = invoke(root, ['--apply']);
  assert.equal(rerun.code, 0, rerun.out + rerun.err);
  assert.match(rerun.out, /^skip \.phase1\/cache\/Godot_fixture_win64\.zip -> \.local\/downloads\/godot\/Godot_fixture_win64\.zip \(already migrated\)$/m);
  for (const gone of ['.phase1', '.phase2']) assert.ok(!exists(root, gone), gone);
  for (const kept of ['.local/downloads/export-templates/templates.tpz', '.local/tools/godot/Godot_fixture_win64/godot_console.exe',
    '.local/nuget/feed/Fixture.Package.1.0.0.nupkg', '.local/nuget/http/v3/index.json']) assert.ok(exists(root, kept), kept);
});

test('the process lister sees this process, sizes read as numbers, and arguments are validated', () => {
  assert.ok(migrate.listProcesses().some(row => row.pid === process.pid));
  assert.equal(migrate.formatSize(0), '0 B');
  assert.equal(migrate.formatSize(1536), '1.5 KB');
  assert.equal(migrate.formatSize(8.1 * 1024 ** 3), '8.1 GB');
  assert.throws(() => migrate.parseArgs(['--force']), migrate.UsageError);
  assert.equal(migrate.run(['--force'], {log: () => {}, error: () => {}}), 2);
});
