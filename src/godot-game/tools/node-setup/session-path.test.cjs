'use strict';
// Session PATH hooks and the Node setup script. Every shared root here is a temporary KZ_SHARED_LOCAL folder or a
// temporary repository under the system temp folder; nothing touches the real shared root.
const test = require('node:test'), assert = require('node:assert/strict');
const fs = require('node:fs'), path = require('node:path'), os = require('node:os');
const {spawnSync} = require('node:child_process');

const ROOT = path.resolve(__dirname, '../../../..');
const HOOK = path.join(__dirname, 'session-path.sh');
const HOOK_PS = path.join(__dirname, 'session-path.ps1');
const SETUP = path.join(__dirname, 'setup.sh');
const MANIFEST = path.join(ROOT, 'src/godot-game/tools/verification/toolchain.json');
const pins = JSON.parse(fs.readFileSync(MANIFEST, 'utf8'));
const WINDOWS = process.platform === 'win32';
// The hook picks the folder from uname: Git Bash on Windows, Linux elsewhere.
const NATIVE = WINDOWS ? {dir: `node-v${pins.node}-win-x64`, exe: 'node.exe'} : {dir: `node-v${pins.node}-linux-x64/bin`, exe: 'node'};

function scratch(t, label = 'kz-session-path-') {
  const base = fs.realpathSync.native(fs.mkdtempSync(path.join(os.tmpdir(), label)));
  t.after(() => fs.rmSync(base, {recursive: true, force: true}));
  return base;
}
function sh(args, {cwd = ROOT, env = {}} = {}) {
  const result = spawnSync('sh', args, {cwd, encoding: 'utf8', windowsHide: true,
    env: {...process.env, KZ_SHARED_LOCAL: '', CLAUDE_ENV_FILE: '', CLAUDE_PROJECT_DIR: '', ...env}});
  assert.equal(result.error, undefined, 'sh must start');
  return {exit: result.status, stdout: result.stdout, stderr: result.stderr};
}
function fakeNode(shared) {
  const dir = path.join(shared, 'tools', 'node', ...NATIVE.dir.split('/'));
  fs.mkdirSync(dir, {recursive: true});
  fs.writeFileSync(path.join(dir, NATIVE.exe), '');
  return dir;
}
const lines = file => fs.existsSync(file) ? fs.readFileSync(file, 'utf8').split('\n').filter(Boolean) : [];
// Sources the written line in sh and reports whether the first PATH entry holds the fake executable.
function sourced(file) {
  return sh(['-c', '. "$1"; first=${PATH%%:*}; [ -f "$first/$2" ] && printf ok', 'probe', file, NATIVE.exe]).stdout;
}
function git(cwd, ...args) {
  const result = spawnSync('git', ['-c', 'user.name=t', '-c', 'user.email=t@example.invalid', '-c', 'commit.gpgsign=false',
    ...args], {cwd, encoding: 'utf8', windowsHide: true});
  assert.equal(result.status, 0, args.join(' ') + ': ' + result.stderr);
  return result.stdout.trim();
}
// A temporary repository carrying the tracked toolchain.json and node-setup scripts at their tracked locations.
function repository(base, manifestText = fs.readFileSync(MANIFEST, 'utf8')) {
  const root = path.join(base, 'main');
  fs.mkdirSync(path.join(root, 'src/godot-game/tools/verification'), {recursive: true});
  fs.mkdirSync(path.join(root, 'src/godot-game/tools/node-setup'), {recursive: true});
  fs.writeFileSync(path.join(root, 'src/godot-game/tools/verification/toolchain.json'), manifestText);
  for (const name of ['session-path.sh', 'session-path.ps1', 'setup.sh', 'setup.ps1', 'shared-root.sh', 'shared-root.ps1'])
    fs.copyFileSync(path.join(__dirname, name), path.join(root, 'src/godot-game/tools/node-setup', name));
  fs.writeFileSync(path.join(root, '.gitignore'), '/.local/\n');
  git(root, 'init', '-q', '-b', 'main');
  git(root, 'add', '.');
  git(root, 'commit', '-q', '-m', 'base');
  return root;
}

// The one KZ_SHARED_LOCAL rule, as a case table that every implementation must answer identically. Drive and UNC
// paths are built from fragments so this file holds no machine-path text.
const drive = rest => 'Q' + ':' + rest;
const unc = (...parts) => ['', '', ...parts].join('\\');
const REASON = {
  windows: 'must be a drive-qualified or UNC path, not a root-relative or MSYS /d/... form',
  absolute: 'must be an absolute path',
  dots: 'must not contain . or .. segments',
  root: 'must not be a filesystem or drive root',
  inside: 'must not be the checkout or inside it, except its .local folder',
};
const WINDOWS_CHECKOUT = drive('/work/repo');
const OVERRIDE_CASES = [
  [drive('/shared'), WINDOWS_CHECKOUT, 1, null],
  [drive('\\shared\\kz'), WINDOWS_CHECKOUT, 1, null],
  [unc('host', 'share', 'kz'), WINDOWS_CHECKOUT, 1, null],
  ['//host/share/kz', WINDOWS_CHECKOUT, 1, null],
  [drive('/work'), WINDOWS_CHECKOUT, 1, null],
  [drive('/work/repo2'), WINDOWS_CHECKOUT, 1, null],
  [drive('/work/repo/.local'), WINDOWS_CHECKOUT, 1, null],
  [drive('/WORK/Repo/.LOCAL/temp/x'), WINDOWS_CHECKOUT, 1, null],
  [drive('\\work\\repo\\.local\\'), drive('\\work\\repo'), 1, null],
  ['\\shared', WINDOWS_CHECKOUT, 1, REASON.windows],
  ['/shared', WINDOWS_CHECKOUT, 1, REASON.windows],
  ['/q/work/repo/.local', WINDOWS_CHECKOUT, 1, REASON.windows],
  ['relative/shared', WINDOWS_CHECKOUT, 1, REASON.windows],
  ['Q' + ':' + 'shared', WINDOWS_CHECKOUT, 1, REASON.windows],
  [drive('/work/./shared'), WINDOWS_CHECKOUT, 1, REASON.dots],
  [drive('/work/repo/../repo/src'), WINDOWS_CHECKOUT, 1, REASON.dots],
  [drive('/'), WINDOWS_CHECKOUT, 1, REASON.root],
  [drive('\\'), WINDOWS_CHECKOUT, 1, REASON.root],
  [drive('//'), WINDOWS_CHECKOUT, 1, REASON.root],
  [unc('host', 'share'), WINDOWS_CHECKOUT, 1, REASON.root],
  [unc('host', 'share', ''), WINDOWS_CHECKOUT, 1, REASON.root],
  [drive('/work/repo'), WINDOWS_CHECKOUT, 1, REASON.inside],
  [drive('/work/repo/'), WINDOWS_CHECKOUT, 1, REASON.inside],
  [drive('/WORK/Repo/src'), WINDOWS_CHECKOUT, 1, REASON.inside],
  [drive('/work//repo/src'), WINDOWS_CHECKOUT, 1, REASON.inside],
  [drive('/work/repo/.localx'), WINDOWS_CHECKOUT, 1, REASON.inside],
  [drive('\\work\\repo\\src'), drive('\\work\\repo'), 1, REASON.inside],
  ['/srv/shared', '/srv/repo', 0, null],
  ['/srv/repo/.local/x', '/srv/repo', 0, null],
  ['/SRV/repo/src', '/srv/repo', 0, null],
  ['srv/shared', '/srv/repo', 0, REASON.absolute],
  ['/srv/repo/../x', '/srv/repo', 0, REASON.dots],
  ['/', '/srv/repo', 0, REASON.root],
  ['//', '/srv/repo', 0, REASON.root],
  ['/srv/repo', '/srv/repo', 0, REASON.inside],
  ['/srv//repo/src', '/srv/repo', 0, REASON.inside],
];

test('KZ_SHARED_LOCAL follows one rule in workspace-paths.cjs, shared-root.sh and shared-root.ps1', t => {
  const workspace = require('../workspace-paths.cjs');
  for (const [value, checkout, windows, expected] of OVERRIDE_CASES)
    assert.equal(workspace.overrideProblem(value, checkout, windows === 1), expected, 'workspace-paths.cjs ' + value);
  const base = scratch(t), table = path.join(base, 'cases.tsv');
  fs.writeFileSync(table, OVERRIDE_CASES.map(row => row.slice(0, 3).join('\t')).join('\n') + '\n');
  const shell = sh(['-c', '. "$1"; tab=$(printf "\\t"); while IFS="$tab" read -r v c w; do printf "%s|\\n" "$(kz_override_problem "$v" "$c" "$w")"; done < "$2"',
    'cases', path.join(__dirname, 'shared-root.sh'), table]);
  assert.equal(shell.exit, 0, shell.stderr);
  const shellAnswers = shell.stdout.split('\n').slice(0, -1).map(line => line.slice(0, -1) || null);
  OVERRIDE_CASES.forEach((row, index) => assert.equal(shellAnswers[index], row[3], 'shared-root.sh ' + row[0]));
  if (!WINDOWS) return;
  const json = path.join(base, 'cases.json');
  fs.writeFileSync(json, JSON.stringify(OVERRIDE_CASES.filter(row => row[2] === 1).map(([value, checkout]) => ({value, checkout}))));
  const powershell = spawnSync('powershell.exe', ['-NoProfile', '-ExecutionPolicy', 'Bypass', '-Command',
    `. '${path.join(__dirname, 'shared-root.ps1')}'; (Get-Content -Raw '${json}' | ConvertFrom-Json) | ForEach-Object { `
    + `[string](Get-KzOverrideProblem $_.value $_.checkout) + '|' }`], {encoding: 'utf8', windowsHide: true});
  assert.equal(powershell.status, 0, powershell.stderr);
  const answers = powershell.stdout.split(/\r?\n/).filter(Boolean).map(line => line.slice(0, -1) || null);
  OVERRIDE_CASES.filter(row => row[2] === 1).forEach((row, index) => assert.equal(answers[index], row[3], 'shared-root.ps1 ' + row[0]));
});

test('the hooks write nothing and setup refuses for a KZ_SHARED_LOCAL inside the checkout, relative or in MSYS form', t => {
  const base = scratch(t), main = repository(base), env = path.join(base, 'env.sh');
  const inside = path.join(main, 'src');
  fakeNode(inside);
  const values = [inside, 'relative/shared'];
  if (WINDOWS) {
    const outside = path.join(base, 'outside');
    fakeNode(outside);
    values.push(sh(['-c', 'cygpath -u "$1"', 'msys', outside]).stdout.trim());
  }
  for (const value of values) {
    const hook = sh([path.join(main, 'src/godot-game/tools/node-setup/session-path.sh')],
      {cwd: base, env: {KZ_SHARED_LOCAL: value, CLAUDE_ENV_FILE: env, CLAUDE_PROJECT_DIR: main, MSYS_NO_PATHCONV: '1'}});
    assert.equal(hook.exit, 0, value);
    assert.equal(hook.stdout + hook.stderr, '', value);
    assert.equal(fs.existsSync(env), false, 'session-path.sh wrote nothing for ' + value);
    const setup = sh([path.join(main, 'src/godot-game/tools/node-setup/setup.sh'), '--check'],
      {cwd: base, env: {KZ_SHARED_LOCAL: value, MSYS_NO_PATHCONV: '1'}});
    assert.equal(setup.exit, 1, value);
    assert.match(setup.stderr, /^SHARED_ROOT_INVALID: KZ_SHARED_LOCAL must /m, value);
    if (WINDOWS) {
      const run = (script, ...args) => spawnSync('powershell.exe', ['-NoProfile', '-ExecutionPolicy', 'Bypass', '-File',
        path.join(main, 'src/godot-game/tools/node-setup', script), ...args],
      {cwd: main, encoding: 'utf8', windowsHide: true, env: {...process.env, KZ_SHARED_LOCAL: value}});
      const codex = run('session-path.ps1');
      assert.equal(codex.status, 0, value);
      assert.equal(codex.stdout.trim(), '', 'session-path.ps1 printed nothing for ' + value);
      const install = run('setup.ps1', '--check');
      assert.equal(install.status, 1, value);
      assert.match(install.stderr, /^SHARED_ROOT_INVALID: KZ_SHARED_LOCAL must /m, value);
    }
  }
});

test('without git on PATH both setup scripts refuse with GIT_MISSING and write nothing', t => {
  const base = scratch(t), main = repository(base), bin = path.join(base, 'empty-bin');
  fs.mkdirSync(bin);
  const shell = sh(['-c', 'command -v sh']).stdout.trim();
  const executable = WINDOWS ? sh(['-c', 'cygpath -w "$1"', 'path', shell]).stdout.trim() : shell;
  const env = Object.fromEntries(Object.entries(process.env).filter(([key]) => key.toLowerCase() !== 'path'));
  const result = spawnSync(executable, [path.join(main, 'src/godot-game/tools/node-setup/setup.sh')],
    {cwd: base, encoding: 'utf8', windowsHide: true, env: {...env, KZ_SHARED_LOCAL: '', PATH: bin}});
  assert.equal(result.status, 1, result.stderr);
  assert.match(result.stderr, /^GIT_MISSING: /);
  assert.equal(result.stdout, '');
  assert.equal(fs.existsSync(path.join(main, '.local')), false, 'setup.sh wrote nothing');
  if (!WINDOWS) return;
  const system = path.join(process.env.SystemRoot, 'System32');
  const powershell = spawnSync(path.join(system, 'WindowsPowerShell', 'v1.0', 'powershell.exe'), ['-NoProfile',
    '-ExecutionPolicy', 'Bypass', '-File', path.join(main, 'src/godot-game/tools/node-setup/setup.ps1')],
  {cwd: base, encoding: 'utf8', windowsHide: true, env: {...env, KZ_SHARED_LOCAL: '', PATH: system}});
  assert.equal(powershell.status, 1, powershell.stderr);
  assert.match(powershell.stderr, /^GIT_MISSING: /m);
  assert.equal(powershell.stdout, '');
  assert.equal(fs.existsSync(path.join(main, '.local')), false, 'setup.ps1 wrote nothing');
});

test('each Node pin names node-v<node>-<platform> and the official release URL', () => {
  const expected = {'win32-x64': ['win-x64', '.zip', 'node.exe'], 'linux-x64': ['linux-x64', '.tar.xz', 'bin/node']};
  assert.deepEqual(Object.keys(pins.nodeArchives).sort(), Object.keys(expected).sort());
  for (const [platform, [suffix, extension, executable]] of Object.entries(expected)) {
    const pin = pins.nodeArchives[platform];
    assert.equal(pin.directory, `node-v${pins.node}-${suffix}`, platform);
    assert.equal(pin.file, pin.directory + extension, platform);
    assert.equal(pin.url, pins.provenance.nodeSource + pin.file, platform);
    assert.match(pin.sha256, /^[0-9a-f]{64}$/, platform);
    assert.equal(pin.executable, executable, platform);
  }
  // The hooks read the version with sed from the first "node": key, which must be the top-level pin.
  const first = fs.readFileSync(MANIFEST, 'utf8').split('\n').find(line => /^ *"node":/.test(line));
  assert.equal(first.trim(), `"node": "${pins.node}",`);
  // setup.sh reads each pin from the single line naming its file.
  for (const pin of Object.values(pins.nodeArchives))
    assert.equal(fs.readFileSync(MANIFEST, 'utf8').split('\n').filter(line => line.includes(`"file": "${pin.file}"`)).length, 1);
});

test('session-path.sh is a silent no-op without a shared Node', t => {
  const base = scratch(t), env = path.join(base, 'env.sh');
  const result = sh([HOOK], {env: {KZ_SHARED_LOCAL: path.join(base, 'shared'), CLAUDE_ENV_FILE: env, CLAUDE_PROJECT_DIR: ROOT}});
  assert.equal(result.exit, 0);
  assert.equal(result.stdout + result.stderr, '');
  assert.equal(fs.existsSync(env), false);
});

test('session-path.sh appends exactly one PATH line, idempotently, from an unrelated folder', t => {
  const base = scratch(t), shared = path.join(base, 'shared'), env = path.join(base, 'env.sh'), elsewhere = path.join(base, 'cwd');
  fs.mkdirSync(elsewhere);
  fakeNode(shared);
  fs.writeFileSync(env, 'export EXISTING=1\n');
  // On Windows the project folder arrives with backslashes, as Claude Code passes it.
  const project = WINDOWS ? ROOT.split('/').join('\\') : ROOT;
  for (let run = 0; run < 2; run++) {
    const result = sh([HOOK], {cwd: elsewhere, env: {KZ_SHARED_LOCAL: shared, CLAUDE_ENV_FILE: env, CLAUDE_PROJECT_DIR: project}});
    assert.equal(result.exit, 0, result.stderr);
    assert.equal(result.stdout, '');
  }
  const written = lines(env);
  assert.equal(written.length, 2, written.join('\n'));
  assert.equal(written[0], 'export EXISTING=1');
  assert.match(written[1], new RegExp(`^export PATH='[^']*/tools/node/${NATIVE.dir}':"\\$PATH"$`));
  assert.equal(sourced(env), 'ok');
});

test('session-path.sh quotes a shared folder whose name has spaces and an apostrophe', t => {
  const base = scratch(t), shared = path.join(base, "shared folder's root"), env = path.join(base, 'env.sh');
  fakeNode(shared);
  const result = sh([HOOK], {env: {KZ_SHARED_LOCAL: shared, CLAUDE_ENV_FILE: env, CLAUDE_PROJECT_DIR: ROOT}});
  assert.equal(result.exit, 0, result.stderr);
  assert.equal(lines(env).length, 1);
  assert.match(lines(env)[0], /shared folder'\\''s root/);
  assert.equal(sourced(env), 'ok');
});

test('session-path.sh resolves the main checkout through the git common dir, and KZ_SHARED_LOCAL overrides it', t => {
  const base = scratch(t), main = repository(base), linked = path.join(base, 'linked'), env = path.join(base, 'env.sh');
  git(main, 'worktree', 'add', '-q', '-b', 'task', linked);
  const run = (extra = {}) => sh([HOOK], {cwd: base, env: {CLAUDE_ENV_FILE: env, CLAUDE_PROJECT_DIR: linked, ...extra}});
  assert.equal(run().exit, 0);
  assert.equal(fs.existsSync(env), false, 'no shared Node yet: no line');
  fakeNode(path.join(main, '.local'));
  assert.equal(run().exit, 0);
  assert.equal(lines(env).length, 1);
  assert.match(lines(env)[0], /\/main\/\.local\/tools\/node\//, 'the linked worktree uses the main checkout .local');
  // Git Bash: an MSYS /c/... project folder still resolves when MSYS_NO_PATHCONV stops argument conversion.
  if (WINDOWS) {
    fs.rmSync(env);
    const msys = sh(['-c', 'cygpath -u "$1"', 'msys', linked]).stdout.trim();
    assert.equal(run({CLAUDE_PROJECT_DIR: msys, MSYS_NO_PATHCONV: '1'}).exit, 0);
    assert.match(lines(env)[0] ?? '', /\/main\/\.local\/tools\/node\//, 'MSYS project folder');
  }
  const override = path.join(base, 'override');
  fakeNode(override);
  fs.rmSync(env);
  assert.equal(run({KZ_SHARED_LOCAL: override}).exit, 0);
  assert.match(lines(env)[0], /\/override\/tools\/node\//);
});

test('session-path.sh --codex prints the line and writes nothing', t => {
  const base = scratch(t), shared = path.join(base, 'shared'), env = path.join(base, 'env.sh');
  fakeNode(shared);
  const result = sh([HOOK, '--codex'], {env: {KZ_SHARED_LOCAL: shared, CLAUDE_ENV_FILE: env, CLAUDE_PROJECT_DIR: ROOT}});
  assert.equal(result.exit, 0);
  assert.match(result.stdout, new RegExp(`^Shared Node ${pins.node.replaceAll('.', '\\.')} for this repository\\. In each shell command run first: export PATH='[^']*/tools/node/${NATIVE.dir}':"\\$PATH"\\n$`));
  assert.equal(fs.existsSync(env), false);
});

test('session-path.sh exits 0 and changes nothing on every failure', t => {
  const base = scratch(t), shared = path.join(base, 'shared'), plain = path.join(base, 'plain');
  fakeNode(shared);
  fs.mkdirSync(plain);
  const ceiling = {GIT_CEILING_DIRECTORIES: base};
  // Not a repository; an unwritable env file; no env file; no toolchain.json.
  const cases = [
    {KZ_SHARED_LOCAL: shared, CLAUDE_ENV_FILE: path.join(base, 'env.sh'), CLAUDE_PROJECT_DIR: plain, ...ceiling},
    {KZ_SHARED_LOCAL: shared, CLAUDE_ENV_FILE: path.join(base, 'missing', 'env.sh'), CLAUDE_PROJECT_DIR: ROOT},
    {KZ_SHARED_LOCAL: shared, CLAUDE_PROJECT_DIR: ROOT},
  ];
  for (const env of cases) {
    const result = sh([HOOK], {env});
    assert.equal(result.exit, 0, JSON.stringify(env));
    assert.equal(result.stdout, '');
  }
  assert.equal(fs.existsSync(path.join(base, 'env.sh')), false);
  const empty = path.join(base, 'empty');
  fs.mkdirSync(empty);
  git(empty, 'init', '-q');
  assert.equal(sh([HOOK], {env: {KZ_SHARED_LOCAL: shared, CLAUDE_ENV_FILE: path.join(base, 'env.sh'), CLAUDE_PROJECT_DIR: empty}}).exit, 0);
  assert.equal(fs.existsSync(path.join(base, 'env.sh')), false);
});

test('session-path.ps1 prints the PowerShell line only when the shared Node exists', {skip: !WINDOWS && 'Windows PowerShell hook'}, t => {
  const base = scratch(t), main = repository(base), shared = path.join(base, 'shared');
  const run = () => spawnSync('powershell.exe', ['-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', HOOK_PS],
    {cwd: main, encoding: 'utf8', windowsHide: true, env: {...process.env, KZ_SHARED_LOCAL: shared}});
  let result = run();
  assert.equal(result.status, 0);
  assert.equal(result.stdout.trim(), '');
  const dir = fakeNode(shared);
  result = run();
  assert.equal(result.status, 0);
  assert.equal(result.stdout.trim(), `Shared Node ${pins.node} for this repository. In each PowerShell command run first: `
    + `$env:Path = '${path.resolve(dir)}' + [IO.Path]::PathSeparator + $env:Path`);
});

// setup.ps1 from a linked worktree: the shared root is the main checkout's .local through the git common dir. The fake
// archive packs this test's own Node, which the runners pin, so the install check can run it.
test('setup.ps1 seeds from a verified archive into the main checkout .local, once, and refuses a mismatch',
  {skip: !WINDOWS && 'Windows PowerShell setup'}, t => {
    const base = scratch(t, 'kz-node-setup-'), directory = `node-v${pins.node}-win-x64`;
    fs.mkdirSync(path.join(base, 'zip', directory), {recursive: true});
    fs.copyFileSync(process.execPath, path.join(base, 'zip', directory, 'node.exe'));
    const archive = path.join(base, directory + '.zip');
    const packed = spawnSync('powershell.exe', ['-NoProfile', '-Command', 'Add-Type -AssemblyName System.IO.Compression.FileSystem; '
      + `[IO.Compression.ZipFile]::CreateFromDirectory('${path.join(base, 'zip')}', '${archive}', 'NoCompression', $false)`],
    {encoding: 'utf8', windowsHide: true});
    assert.equal(packed.status, 0, packed.stderr);
    const run = (root, ...args) => spawnSync('powershell.exe', ['-NoProfile', '-ExecutionPolicy', 'Bypass', '-File',
      path.join(root, 'src/godot-game/tools/node-setup/setup.ps1'), ...args],
    {cwd: base, encoding: 'utf8', windowsHide: true, env: {...process.env, KZ_SHARED_LOCAL: ''}});

    const wrong = repository(path.join(base, 'wrong'));
    let result = run(wrong, '--archive', archive);
    assert.equal(result.status, 1);
    assert.match(result.stderr, /^NODE_ARCHIVE_MISMATCH: /m);
    assert.equal(result.stdout, '');
    assert.deepEqual(fs.readdirSync(path.join(wrong, '.local/tools/node')), [], 'nothing extracted');
    assert.deepEqual(fs.readdirSync(path.join(wrong, '.local/downloads/node')), [], 'no archive or .part kept');
    assert.deepEqual(fs.readdirSync(path.join(wrong, '.local/locks')), [], 'the lock is released');

    const text = fs.readFileSync(MANIFEST, 'utf8').replace(pins.nodeArchives['win32-x64'].sha256, sha(archive));
    const main = repository(path.join(base, 'right'), text), linked = path.join(base, 'right', 'linked');
    git(main, 'worktree', 'add', '-q', '-b', 'task', linked);
    result = run(linked, '--archive', archive);
    assert.equal(result.status, 0, result.stderr);
    const install = path.join(main, '.local/tools/node', directory);
    assert.equal(result.stdout, `$env:Path = '${install}' + [IO.Path]::PathSeparator + $env:Path\r\n`);
    assert.equal(fs.existsSync(path.join(linked, '.local')), false, 'the linked worktree gets no Node');
    assert.equal(sha(path.join(main, '.local/downloads/node', directory + '.zip')), sha(archive));
    assert.deepEqual(fs.readdirSync(path.join(main, '.local/tools/node')), [directory], 'no staging sibling remains');
    const again = run(linked);
    assert.equal(again.status, 0, again.stderr);
    assert.equal(again.stdout, result.stdout);
    assert.match(again.stderr, /nothing downloaded/);
    const checked = run(linked, '--check');
    assert.equal(checked.status, 0, checked.stderr);
    assert.equal(checked.stdout, result.stdout);
  });

// setup.sh --platform linux-x64 on a fake pinned archive: verification before extraction, nonce siblings, the lock,
// idempotence and one PATH line. Needs tar with xz, which Git Bash and Linux provide.
function fakeArchive(base, version) {
  const source = path.join(base, 'archive-source'), directory = `node-v${version}-linux-x64`;
  fs.mkdirSync(path.join(source, directory, 'bin'), {recursive: true});
  fs.writeFileSync(path.join(source, directory, 'bin', 'node'), '#!/bin/sh\necho v' + version + '\n');
  const file = path.join(base, `${directory}.tar.xz`);
  // A relative archive name keeps GNU tar from reading a drive letter as a remote host.
  const packed = sh(['-c', 'cd "$1" && tar -cJf "../$2" "$3"', 'pack', source, path.basename(file), directory]);
  assert.equal(packed.exit, 0, packed.stderr);
  return file;
}
function pinnedRepository(base, archive, sha256) {
  const manifest = JSON.parse(fs.readFileSync(MANIFEST, 'utf8'));
  const text = fs.readFileSync(MANIFEST, 'utf8').replace(manifest.nodeArchives['linux-x64'].sha256, sha256);
  assert.notEqual(text, fs.readFileSync(MANIFEST, 'utf8'));
  return repository(base, text);
}
const sha = file => require('node:crypto').createHash('sha256').update(fs.readFileSync(file)).digest('hex');

// Run from a linked worktree without an override, with MSYS_NO_PATHCONV set as the repository shell rules do: the
// shared root must be the main checkout's .local.
test('setup.sh verifies the pin before extraction, installs once into the main checkout .local, and prints one PATH line', t => {
  const base = scratch(t, 'kz-node-setup-'), archive = fakeArchive(base, pins.node);
  const main = pinnedRepository(base, archive, sha(archive)), linked = path.join(base, 'linked'), shared = path.join(main, '.local');
  git(main, 'worktree', 'add', '-q', '-b', 'task', linked);
  const script = path.join(linked, 'src/godot-game/tools/node-setup/setup.sh');
  const run = (...args) => sh([script, '--platform', 'linux-x64', ...args], {cwd: base, env: {MSYS_NO_PATHCONV: '1'}});
  const install = path.join(shared, 'tools/node', `node-v${pins.node}-linux-x64`);

  let result = run('--check');
  assert.equal(result.exit, 1);
  assert.match(result.stderr, /^NODE_ARCHIVE_MISSING: /);

  result = run('--archive', archive);
  assert.equal(result.exit, 0, result.stderr);
  assert.match(result.stdout, new RegExp(`^export PATH='[^']*/tools/node/node-v${pins.node.replaceAll('.', '\\.')}-linux-x64/bin':"\\$PATH"\\n$`));
  assert.equal(sha(path.join(shared, 'downloads/node', path.basename(archive))), sha(archive));
  assert.ok(fs.statSync(path.join(install, 'bin', 'node')).isFile());
  assert.equal(fs.existsSync(path.join(linked, '.local')), false, 'the linked worktree gets no Node');
  assert.deepEqual(fs.readdirSync(path.join(shared, 'tools/node')), [path.basename(install)], 'no staging sibling remains');
  assert.deepEqual(fs.readdirSync(path.join(shared, 'downloads/node')), [path.basename(archive)], 'no .part file remains');
  assert.deepEqual(fs.readdirSync(path.join(shared, 'locks')), [], 'the lock is released');

  const again = run();
  assert.equal(again.exit, 0, again.stderr);
  assert.equal(again.stdout, result.stdout);
  assert.match(again.stderr, /nothing downloaded/);
  const checked = run('--check');
  assert.equal(checked.exit, 0, checked.stderr);
  assert.equal(checked.stdout, result.stdout);
});

test('setup.sh refuses a mismatched archive before extraction and a held toolchain lock', t => {
  const base = scratch(t, 'kz-node-setup-'), archive = fakeArchive(base, pins.node), shared = path.join(base, 'shared');
  const main = pinnedRepository(base, archive, 'f'.repeat(64));
  const script = path.join(main, 'src/godot-game/tools/node-setup/setup.sh');
  const run = (...args) => sh([script, '--platform', 'linux-x64', ...args], {cwd: base, env: {KZ_SHARED_LOCAL: shared}});
  let result = run('--archive', archive);
  assert.equal(result.exit, 1);
  assert.match(result.stderr, /^NODE_ARCHIVE_MISMATCH: /m);
  assert.equal(result.stdout, '');
  assert.deepEqual(fs.readdirSync(path.join(shared, 'tools/node')), [], 'nothing extracted');
  assert.deepEqual(fs.readdirSync(path.join(shared, 'downloads/node')), [], 'no archive or .part kept');
  assert.deepEqual(fs.readdirSync(path.join(shared, 'locks')), []);

  fs.writeFileSync(path.join(shared, 'locks', 'toolchain.lock'), '{}');
  result = run('--archive', archive);
  assert.equal(result.exit, 1);
  assert.match(result.stderr, /^TOOLCHAIN_LOCKED: /);
  assert.equal(fs.readFileSync(path.join(shared, 'locks', 'toolchain.lock'), 'utf8'), '{}', 'a foreign lock stays');

  result = sh([script, '--platform', 'linux-x64'], {cwd: base, env: {KZ_SHARED_LOCAL: 'relative/shared'}});
  assert.equal(result.exit, 1);
  assert.match(result.stderr, /^SHARED_ROOT_INVALID: /);
});
