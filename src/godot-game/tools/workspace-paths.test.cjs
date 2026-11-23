'use strict';
// Every repository here is temporary, under the system temp folder; nothing touches the real shared root.
const test = require('node:test'), assert = require('node:assert/strict');
const fs = require('node:fs'), path = require('node:path'), os = require('node:os');
const {spawnSync} = require('node:child_process');
const paths = require('./workspace-paths.cjs');

const MODULE = path.join(__dirname, 'workspace-paths.cjs');
const real = value => fs.realpathSync.native(value);
const same = (actual, expected, label) => {
  const fold = value => process.platform === 'win32' ? path.resolve(value).toLowerCase() : path.resolve(value);
  assert.equal(fold(actual), fold(expected), label);
};
const legacy = (...parts) => ['.', 'phase', ...parts].join('');

function scratch(t) {
  const base = real(fs.mkdtempSync(path.join(os.tmpdir(), 'kz-workspace-paths-')));
  t.after(() => fs.rmSync(base, {recursive: true, force: true}));
  return base;
}
// Ceiling the git search at the scratch base, so a non-git folder never finds an unrelated enclosing repository.
const isolated = base => ({...process.env, GIT_CEILING_DIRECTORIES: base, KZ_SHARED_LOCAL: ''});
// legacyPath and insideShared read process.env; pin the override so an outer runner's value cannot leak in.
function withShared(t, value) {
  const previous = process.env.KZ_SHARED_LOCAL;
  process.env.KZ_SHARED_LOCAL = value;
  t.after(() => { if (previous === undefined) delete process.env.KZ_SHARED_LOCAL; else process.env.KZ_SHARED_LOCAL = previous; });
}
function git(cwd, ...args) {
  const result = spawnSync('git', ['-c', 'user.name=t', '-c', 'user.email=t@example.invalid', '-c', 'commit.gpgsign=false',
    ...args], {cwd, encoding: 'utf8', windowsHide: true});
  assert.equal(result.status, 0, args.join(' ') + ': ' + result.stderr);
  return result.stdout.trim();
}
function repository(base, name = 'main') {
  const root = path.join(base, name);
  fs.mkdirSync(root, {recursive: true});
  git(root, 'init', '-q', '-b', 'main');
  fs.writeFileSync(path.join(root, '.gitignore'), '/.local/\n');
  git(root, 'add', '.gitignore');
  git(root, 'commit', '-q', '-m', 'base');
  return root;
}
// Installs the helper at its tracked location in a temporary checkout, so __dirname resolution is exercised.
function install(root) {
  const target = path.join(root, 'src/godot-game/tools/workspace-paths.cjs');
  fs.mkdirSync(path.dirname(target), {recursive: true});
  fs.copyFileSync(MODULE, target);
  return target;
}
function cli(script, args, cwd, env) {
  const result = spawnSync(process.execPath, [script, ...args], {cwd, env, encoding: 'utf8', windowsHide: true});
  return {exit: result.status, stdout: result.stdout, stderr: result.stderr};
}

test('main checkout: shared equals local', t => {
  const base = scratch(t), main = repository(base);
  const result = paths.roots(main, isolated(base));
  assert.equal(result.source, 'git');
  same(result.local, path.join(main, '.local'));
  same(result.shared, result.local);
});

test('linked worktree: shared is the main .local and local is the worktree .local', t => {
  const base = scratch(t), main = repository(base), linked = path.join(base, 'linked');
  git(main, 'worktree', 'add', '-q', '-b', 'task', linked);
  const result = paths.roots(linked, isolated(base));
  assert.equal(result.source, 'git');
  same(result.shared, path.join(main, '.local'));
  same(result.local, path.join(linked, '.local'));
  const where = paths.locations(linked, isolated(base));
  same(where.nuget.packages, path.join(main, '.local/nuget/packages'));
  same(where.nuget.scratch, path.join(main, '.local/nuget/scratch'));
  same(where.nuget.feed, path.join(linked, '.local/nuget/feed'));
  same(where.locks.toolchain, path.join(main, '.local/locks/toolchain.lock'));
  same(where.locks.desktop, path.join(main, '.local/locks/desktop.lock'));
  same(where.locks.check, path.join(linked, '.local/locks/check.lock'));
  same(where.tools('godot'), path.join(main, '.local/tools/godot'));
  same(where.downloads('godot'), path.join(main, '.local/downloads/godot'));
  same(where.runs('verification', 'n'), path.join(linked, '.local/runs/verification/n'));
  same(where.temp('x'), path.join(linked, '.local/temp/x'));
  same(where.cliHome, path.join(linked, '.local/temp/cli-home'));
});

test('an unrelated working directory resolves through the installed helper location', t => {
  const base = scratch(t), main = repository(base), linked = path.join(base, 'linked');
  git(main, 'worktree', 'add', '-q', '-b', 'task', linked);
  const elsewhere = path.join(base, 'elsewhere');
  fs.mkdirSync(elsewhere);
  const run = cli(install(linked), ['--print-roots'], elsewhere, isolated(base));
  assert.equal(run.exit, 0, run.stderr);
  const printed = JSON.parse(run.stdout);
  same(printed.checkout, linked);
  same(printed.shared, path.join(main, '.local'));
  same(printed.local, path.join(linked, '.local'));
  const explicit = JSON.parse(cli(MODULE, ['--print-roots', '--root', main], elsewhere, isolated(base)).stdout);
  same(explicit.shared, path.join(main, '.local'));
  same(explicit.local, explicit.shared);
});

test('KZ_SHARED_LOCAL overrides the shared root and must be absolute', t => {
  const base = scratch(t), main = repository(base), override = path.join(base, 'override');
  const result = paths.roots(main, {...isolated(base), KZ_SHARED_LOCAL: override});
  assert.equal(result.source, 'override');
  same(result.shared, override);
  same(result.local, path.join(main, '.local'));
  assert.throws(() => paths.roots(main, {...isolated(base), KZ_SHARED_LOCAL: 'relative/folder'}),
    error => error.diagnostic === 'SHARED_ROOT_INVALID');
  const run = cli(MODULE, ['--print-roots', '--root', main], base, {...isolated(base), KZ_SHARED_LOCAL: 'relative'});
  assert.equal(run.exit, 1);
  assert.match(run.stderr, /SHARED_ROOT_INVALID/);
});

test('KZ_SHARED_LOCAL is refused inside the checkout except its .local, at a filesystem root, and in MSYS form', t => {
  const base = scratch(t), main = repository(base);
  const refused = value => assert.throws(() => paths.roots(main, {...isolated(base), KZ_SHARED_LOCAL: value}),
    error => error.diagnostic === 'SHARED_ROOT_INVALID' && error.message.includes(value), value);
  refused(main);
  refused(path.join(main, 'src'));
  refused(path.join(main, '.localx'));
  refused(path.parse(main).root);
  refused([base, '..', path.basename(base), 'shared'].join(path.sep));
  if (process.platform === 'win32') {
    refused('/' + main[0].toLowerCase() + main.slice(2).replaceAll('\\', '/'));
    refused(main.slice(2));
  }
  for (const accepted of [path.join(main, '.local'), path.join(main, '.local', 'temp', 'x'), path.join(base, 'shared')])
    same(paths.roots(main, {...isolated(base), KZ_SHARED_LOCAL: accepted}).shared, accepted, accepted);
});

test('without git on PATH the helper refuses with GIT_MISSING and never falls back to the checkout .local', t => {
  const base = scratch(t), main = repository(base), bin = path.join(base, 'empty-bin');
  fs.mkdirSync(bin);
  const env = Object.fromEntries(Object.entries(isolated(base)).filter(([key]) => key.toLowerCase() !== 'path'));
  const run = cli(MODULE, ['--print-roots', '--root', main], base, {...env, PATH: bin});
  assert.equal(run.exit, 1, run.stdout);
  assert.match(run.stderr, /^GIT_MISSING: /);
  assert.throws(() => paths.roots(main, {...env, PATH: bin}), error => error.diagnostic === 'GIT_MISSING');
  assert.equal(fs.existsSync(path.join(main, '.local')), false);
});

test('a non-git copy is standalone', t => {
  const base = scratch(t), copy = path.join(base, 'copy');
  fs.mkdirSync(copy);
  const result = paths.roots(copy, isolated(base));
  assert.equal(result.source, 'standalone');
  same(result.shared, path.join(copy, '.local'));
  same(result.local, result.shared);
});

test('a non-git folder nested under a checkout .local/temp keeps its own local and the enclosing shared root', t => {
  const base = scratch(t), main = repository(base), linked = path.join(base, 'linked');
  git(main, 'worktree', 'add', '-q', '-b', 'task', linked);
  const copy = path.join(linked, '.local/temp/source-copy');
  fs.mkdirSync(copy, {recursive: true});
  const result = paths.roots(copy, isolated(base));
  same(result.local, path.join(copy, '.local'));
  same(result.shared, path.join(main, '.local'));
});

test('a bare common dir is unresolved and names the override', t => {
  const base = scratch(t), main = repository(base), bare = path.join(base, 'bare.git'), linked = path.join(base, 'from-bare');
  git(base, 'clone', '-q', '--bare', main, bare);
  git(bare, 'worktree', 'add', '-q', linked, 'main');
  assert.throws(() => paths.roots(linked, isolated(base)),
    error => error.diagnostic === 'SHARED_ROOT_UNRESOLVED' && /KZ_SHARED_LOCAL/.test(error.message));
  const result = paths.roots(linked, {...isolated(base), KZ_SHARED_LOCAL: path.join(base, 'ci')});
  same(result.shared, path.join(base, 'ci'));
});

test('every legacy-map row resolves to its layout location; the longest prefix wins', t => {
  const base = scratch(t), main = repository(base), linked = path.join(base, 'linked');
  git(main, 'worktree', 'add', '-q', '-b', 'task', linked);
  const S = path.join(main, '.local'), L = path.join(linked, '.local');
  withShared(t, S);
  const expected = [
    [legacy('1/cache/phase4-exports/a.tpz'), S, 'downloads/export-templates/a.tpz'],
    [legacy('1/cache/Godot.zip'), S, 'downloads/godot/Godot.zip'],
    [legacy('1/tools/linux-vm/disk'), S, 'tools/linux-vm/disk'],
    [legacy('1/tools/linux-prep/a.tar.xz'), S, 'downloads/linux-prep/a.tar.xz'],
    [legacy('1/tools/Godot_dir/godot.exe'), S, 'tools/godot/Godot_dir/godot.exe'],
    [legacy('1/feed/Godot.SourceGenerators.4.7.2.nupkg'), L, 'nuget/feed/Godot.SourceGenerators.4.7.2.nupkg'],
    [legacy('1/packages/x/1.0/x.nupkg'), S, 'nuget/packages/x/1.0/x.nupkg'],
    [legacy('1/http-cache/v3'), S, 'nuget/http/v3'],
    [legacy('1/cli-home/.dotnet'), L, 'temp/cli-home/.dotnet'],
    [legacy('1/dotnet-home'), L, 'temp/cli-home'],
    [legacy('1/temp/a'), L, 'temp/a'],
    [legacy('1/runs/n/summary.json'), L, 'runs/verification/n/summary.json'],
    [legacy('1/check.lock'), L, 'locks/check.lock'],
    [legacy('1/quick-x/notes.md'), L, 'temp/verification/quick-x/notes.md'],
    [legacy('2/runs/windows/n'), L, 'runs/gameplay-checks/windows/n'],
    [legacy('2/completions/c.json'), L, 'runs/gameplay-checks/completions/c.json'],
    [legacy('2/desktop.lock'), S, 'locks/desktop.lock'],
    [legacy('2/scratch'), L, 'temp/gameplay-checks/scratch'],
    [legacy('3/runs/linux/n'), L, 'runs/lan-checks/linux/n'],
    [legacy('3/controls/x'), L, 'temp/lan-checks/controls/x'],
    [legacy('4/runs/n/a.json'), L, 'runs/asset-pipeline/n/a.json'],
    [legacy('4/acceptance/a.json'), L, 'runs/asset-pipeline/acceptance/a.json'],
    [legacy('4/04-23-current-model/normalized-index.json'), L, 'temp/asset-pipeline/04-23-current-model/normalized-index.json'],
  ];
  const longest = identity => paths.LEGACY_MAP.filter(entry => identity === entry[0] || identity.startsWith(entry[0] + '/'))
    .sort((a, b) => b[0].length - a[0].length)[0][0];
  assert.deepEqual(new Set(expected.map(row => longest(row[0]))), new Set(paths.LEGACY_MAP.map(entry => entry[0])),
    'the rows cover every legacy-map entry');
  for (const [identity, rootOf, relative] of expected)
    same(paths.legacyPath(identity, linked), path.join(rootOf, ...relative.split('/')), identity);
  same(paths.legacyPath(legacy('1\\feed\\a.nupkg'), linked), path.join(L, 'nuget/feed/a.nupkg'), 'backslash identity');
  assert.equal(paths.legacyPath('src/godot-game/tools/verification/check.cjs', linked), null);
  assert.equal(paths.legacyPath(legacy('10/x'), linked), null);
  assert.equal(paths.legacyPath(legacy('1-other/x'), linked), null);
});

test('insideShared accepts a file inside S and rejects .. escapes, a sibling prefix and outside paths', t => {
  const base = scratch(t), main = repository(base), shared = path.join(main, '.local');
  fs.mkdirSync(path.join(shared, 'nuget/packages/x'), {recursive: true});
  fs.writeFileSync(path.join(shared, 'nuget/packages/x/x.nupkg'), 'x');
  const sibling = path.join(main, '.local-other');
  fs.mkdirSync(sibling);
  withShared(t, shared);
  assert.equal(paths.insideShared(path.join(shared, 'nuget/packages/x/x.nupkg'), main), true);
  assert.equal(paths.insideShared(path.join(shared, 'tools/not-yet/created.exe'), main), true, 'a missing target under S');
  assert.equal(paths.insideShared(path.join(shared, 'nuget/../../escape.txt'), main), false, '.. escape');
  assert.equal(paths.insideShared(path.join(sibling, 'x'), main), false, 'sibling prefix');
  assert.equal(paths.insideShared(path.join(base, 'outside.txt'), main), false, 'outside');
  assert.equal(paths.insideShared(shared, main), false, 'S itself is not inside S');
  assert.equal(paths.insideShared('nuget/packages/x/x.nupkg', main), false, 'relative input');
  if (process.platform === 'win32')
    assert.equal(paths.insideShared(path.join(shared, 'nuget/packages/x/x.nupkg').toUpperCase(), main), true, 'case-insensitive');
  let link = true;
  try { fs.symlinkSync(base, path.join(shared, 'link-out'), 'junction'); } catch { link = false; }
  if (link) {
    assert.equal(paths.insideShared(path.join(shared, 'link-out', 'outside.txt'), main), false, 'link leading out');
    fs.unlinkSync(path.join(shared, 'link-out'));
  }
});

test('isolationEnv and --print-env export the shared root and per-checkout folders', t => {
  const base = scratch(t), main = repository(base), linked = path.join(base, 'linked');
  git(main, 'worktree', 'add', '-q', '-b', 'task', linked);
  const override = path.join(base, 'shared');
  const env = {...isolated(base), KZ_SHARED_LOCAL: override};
  const script = install(linked);
  const sh = cli(script, ['--print-env', 'sh'], base, env);
  assert.equal(sh.exit, 0, sh.stderr);
  const lines = sh.stdout.trim().split('\n');
  assert.deepEqual(lines.map(line => /^export ([A-Z_]+)=/.exec(line)[1]), paths.ISOLATION_KEYS);
  assert.ok(lines.includes("export KZ_SHARED_LOCAL='" + override + "'"));
  assert.ok(lines.includes("export NUGET_SCRATCH='" + path.join(override, 'nuget', 'scratch') + "'"));
  assert.ok(fs.existsSync(path.join(linked, '.local/temp/cli-home')) && fs.existsSync(path.join(override, 'nuget/scratch')));
  const powershell = cli(script, ['--print-env', 'powershell'], base, env);
  assert.ok(powershell.stdout.includes("$env:TEMP = '" + path.join(linked, '.local', 'temp') + "'"));
  assert.equal(cli(script, ['--print-env', 'fish'], base, env).exit, 2);
  assert.equal(cli(script, [], base, env).exit, 2);
});

// A main checkout on a branch older than the .local layout: its .gitignore does not ignore .local/.
function olderBranch(base) {
  const main = path.join(base, 'main');
  fs.mkdirSync(main);
  git(main, 'init', '-q', '-b', 'main');
  fs.writeFileSync(path.join(main, 'README.md'), 'older branch\n');
  git(main, 'add', 'README.md');
  git(main, 'commit', '-q', '-m', 'base');
  const linked = path.join(base, 'linked');
  git(main, 'worktree', 'add', '-q', '-b', 'task', linked);
  return {main, linked, exclude: path.join(main, '.git', 'info', 'exclude')};
}

test('nothing writes a shared root that Git does not ignore, and info/exclude is never edited', t => {
  const base = scratch(t), {main, linked, exclude} = olderBranch(base);
  const before = fs.readFileSync(exclude, 'utf8');
  const script = install(linked);
  const refused = cli(script, ['--print-env', 'sh'], base, isolated(base));
  assert.equal(refused.exit, 1, refused.stdout);
  assert.match(refused.stderr, /^SHARED_ROOT_NOT_IGNORED: .*\/\.local\/ to that checkout's \.git\/info\/exclude/);
  assert.equal(fs.existsSync(path.join(main, '.local')), false, 'the shared root was not created');
  assert.equal(fs.existsSync(path.join(linked, '.local')), false, 'nothing was created before the check');
  withShared(t, '');
  assert.throws(() => paths.isolationEnv(linked), error => error.diagnostic === 'SHARED_ROOT_NOT_IGNORED');
  assert.equal(fs.readFileSync(exclude, 'utf8'), before, 'info/exclude is never edited');

  const outside = cli(script, ['--print-env', 'sh'], base, {...isolated(base), KZ_SHARED_LOCAL: path.join(base, 'outside')});
  assert.equal(outside.exit, 0, 'a shared root outside every working tree needs no check: ' + outside.stderr);
  fs.appendFileSync(exclude, '/.local/\n');
  const allowed = cli(script, ['--print-env', 'sh'], base, isolated(base));
  assert.equal(allowed.exit, 0, allowed.stderr);
  assert.ok(fs.existsSync(path.join(main, '.local', 'nuget', 'scratch')));
});

test('bootstrap refuses before its first write when the shared root is not ignored', t => {
  const base = scratch(t), {main, linked} = olderBranch(base);
  for (const file of ['src/godot-game/tools/verification/bootstrap.cjs', 'src/godot-game/tools/dotnet.cjs']) {
    fs.mkdirSync(path.dirname(path.join(linked, file)), {recursive: true});
    fs.copyFileSync(path.join(__dirname, '../../..', file), path.join(linked, file));
  }
  install(linked);
  const run = cli(path.join(linked, 'src/godot-game/tools/verification/bootstrap.cjs'), ['--probe', 'pins'], base,
    isolated(base));
  assert.equal(run.exit, 1, run.stdout);
  assert.equal(JSON.parse(run.stderr.trim().split('\n').pop()).diagnostic, 'SHARED_ROOT_NOT_IGNORED');
  assert.equal(fs.existsSync(path.join(main, '.local')), false);
  assert.equal(fs.existsSync(path.join(linked, '.local')), false);
});

test('requiring the helper creates no folder and spawns no process', t => {
  const base = scratch(t);
  const probe = `
    const cp = require('node:child_process'), fs = require('node:fs'), calls = [];
    for (const name of ['spawn', 'spawnSync', 'exec', 'execSync', 'execFile', 'execFileSync', 'fork'])
      cp[name] = (...args) => { calls.push(name); throw new Error('spawned'); };
    for (const name of ['mkdirSync', 'mkdir', 'writeFileSync', 'mkdtempSync'])
      { const original = fs[name]; fs[name] = (...args) => { calls.push(name); return original(...args); }; }
    require(process.argv[1]);
    process.stdout.write(JSON.stringify(calls));`;
  const result = spawnSync(process.execPath, ['-e', probe, MODULE], {cwd: base, encoding: 'utf8', windowsHide: true});
  assert.equal(result.status, 0, result.stderr);
  assert.deepEqual(JSON.parse(result.stdout), []);
  assert.deepEqual(fs.readdirSync(base), []);
});
