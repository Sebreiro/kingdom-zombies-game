'use strict';

// The one resolver for the ignored, regenerable workspace root `.local/`.
// Shared (once per repository, in the main checkout): tools, downloads, nuget packages/http/scratch, toolchain and
// desktop locks. Per checkout: runs, temp, the verified package feed and the check lock.
// KZ_SHARED_LOCAL overrides the shared root for CI, extra clones, bare repositories and source copies; overrideProblem
// holds the one rule it must meet. Requiring this module spawns no process and creates no folder.
const fs = require('node:fs');
const path = require('node:path');
const {spawnSync} = require('node:child_process');

const OVERRIDE = 'KZ_SHARED_LOCAL';
const memo = new Map();

const fail = (diagnostic, message) => { throw Object.assign(new Error(diagnostic + ': ' + message), {diagnostic}); };

function checkoutRoot() {
  return path.resolve(__dirname, '../../..');
}

// A path for comparison only: on Windows backslashes become '/'; repeated separators collapse except a leading '//';
// trailing separators are dropped.
function normalOverride(value, windows) {
  let text = windows ? value.replaceAll('\\', '/') : value;
  const unc = text.startsWith('//');
  text = text.replace(/\/{2,}/g, '/');
  if (unc) text = '/' + text;
  return text.replace(/\/+$/, '');
}

// The one KZ_SHARED_LOCAL rule. node-setup/shared-root.sh, node-setup/shared-root.ps1 and WorkspacePaths.cs hold the
// same rule with the same messages, and node-setup/session-path.test.cjs runs one case table through each script.
// On Windows the value is drive-qualified or UNC (a root-relative or MSYS /d/... form is refused, never converted);
// elsewhere it starts with '/'. It has no '.' or '..' segment, is not a filesystem or drive root, and is neither the
// checkout nor inside it, except the checkout's own .local folder. Returns the reason it is refused, or null.
function overrideProblem(value, checkout, windows = process.platform === 'win32') {
  const absolute = windows ? /^([A-Za-z]:[\\/]|[\\/]{2}[^\\/]+[\\/][^\\/]+)/.test(value) : value.startsWith('/');
  if (!absolute) {
    return windows ? 'must be a drive-qualified or UNC path, not a root-relative or MSYS /d/... form'
      : 'must be an absolute path';
  }
  const normal = normalOverride(value, windows);
  if (/(^|\/)\.\.?(\/|$)/.test(normal)) return 'must not contain . or .. segments';
  const root = windows ? /^[A-Za-z]:$|^\/\/[^/]+\/[^/]+$/.test(normal) : normal === '';
  if (root) return 'must not be a filesystem or drive root';
  const fold = text => windows ? text.toLowerCase() : text;
  const target = fold(normal) + '/', base = fold(normalOverride(checkout, windows)) + '/';
  if (target.startsWith(base + '.local/')) return null;
  return target.startsWith(base) ? 'must not be the checkout or inside it, except its .local folder' : null;
}

// Resolution order: KZ_SHARED_LOCAL, then the git common dir's parent, then the checkout's own `.local`.
function roots(root = checkoutRoot(), env = process.env) {
  const checkout = path.resolve(root);
  const override = env[OVERRIDE] || '';
  const key = checkout + '\0' + override;
  if (memo.has(key)) return memo.get(key);
  const local = path.join(checkout, '.local');
  let result;
  if (override) {
    const problem = overrideProblem(override, checkout);
    if (problem) fail('SHARED_ROOT_INVALID', OVERRIDE + ' ' + problem + ': ' + override);
    result = {checkout, local, shared: path.resolve(override), source: 'override'};
  } else {
    const git = spawnSync('git', ['-C', checkout, 'rev-parse', '--path-format=absolute', '--git-common-dir'],
      {encoding: 'utf8', windowsHide: true, env});
    // Without Git a linked worktree would silently become standalone and download its own toolchain; refuse instead.
    if (git.error) {
      fail('GIT_MISSING', 'git could not start (' + git.error.code + '), so the shared .local root cannot be resolved; '
        + 'put Git on PATH or set ' + OVERRIDE);
    }
    const common = git.status === 0 ? String(git.stdout).trim() : '';
    if (common) {
      if (path.basename(common) !== '.git')
        fail('SHARED_ROOT_UNRESOLVED', 'git common dir ' + common + ' is not a .git folder; set ' + OVERRIDE);
      result = {checkout, local, shared: path.join(path.dirname(path.resolve(common)), '.local'), source: 'git'};
    } else {
      result = {checkout, local, shared: local, source: 'standalone'};
    }
  }
  memo.set(key, Object.freeze(result));
  return memo.get(key);
}

function locations(root, env) {
  const {checkout, local, shared, source} = roots(root, env);
  return {
    checkout, local, shared, source,
    tools: (...parts) => path.join(shared, 'tools', ...parts),
    downloads: (...parts) => path.join(shared, 'downloads', ...parts),
    nuget: {
      packages: path.join(shared, 'nuget', 'packages'),
      http: path.join(shared, 'nuget', 'http'),
      scratch: path.join(shared, 'nuget', 'scratch'),
      feed: path.join(local, 'nuget', 'feed'),
    },
    locks: {
      toolchain: path.join(shared, 'locks', 'toolchain.lock'),
      desktop: path.join(shared, 'locks', 'desktop.lock'),
      check: path.join(local, 'locks', 'check.lock'),
    },
    runs: (tool, ...parts) => path.join(local, 'runs', tool, ...parts),
    temp: (...parts) => path.join(local, 'temp', ...parts),
    cliHome: path.join(local, 'temp', 'cli-home'),
  };
}

// Child-process isolation: CLI home, NuGet folders, temp and the resolved shared root, so children and source
// copies agree. Callers add their own flags (telemetry opt-outs, nonces). Children write the shared NuGet folders,
// so the shared root must be ignored first.
function isolationEnv(root, extra = {}) {
  assertSharedIgnored(root);
  const where = locations(root);
  const temp = where.temp();
  return {
    ...process.env,
    DOTNET_CLI_HOME: where.cliHome,
    NUGET_PACKAGES: process.env.PHASE1_PACKAGE_CACHE ?? where.nuget.packages,
    NUGET_HTTP_CACHE_PATH: where.nuget.http,
    NUGET_SCRATCH: where.nuget.scratch,
    TEMP: temp, TMP: temp, TMPDIR: temp,
    [OVERRIDE]: where.shared,
    ...extra,
  };
}

// Historical `.phaseN` identities and their current physical locations; the longest prefix wins.
// Scope `shared` resolves under the shared root, `local` under the checkout's `.local`.
const LEGACY_MAP = Object.freeze([
  ['.phase1/cache/phase4-exports', 'shared', 'downloads/export-templates'],
  ['.phase1/cache', 'shared', 'downloads/godot'],
  ['.phase1/tools/linux-vm', 'shared', 'tools/linux-vm'],
  ['.phase1/tools/linux-prep', 'shared', 'downloads/linux-prep'],
  ['.phase1/tools', 'shared', 'tools/godot'],
  ['.phase1/feed', 'local', 'nuget/feed'],
  ['.phase1/packages', 'shared', 'nuget/packages'],
  ['.phase1/http-cache', 'shared', 'nuget/http'],
  ['.phase1/cli-home', 'local', 'temp/cli-home'],
  ['.phase1/dotnet-home', 'local', 'temp/cli-home'],
  ['.phase1/temp', 'local', 'temp'],
  ['.phase1/runs', 'local', 'runs/verification'],
  ['.phase1/check.lock', 'local', 'locks/check.lock'],
  ['.phase1', 'local', 'temp/verification'],
  ['.phase2/runs', 'local', 'runs/gameplay-checks'],
  ['.phase2/completions', 'local', 'runs/gameplay-checks/completions'],
  ['.phase2/desktop.lock', 'shared', 'locks/desktop.lock'],
  ['.phase2', 'local', 'temp/gameplay-checks'],
  ['.phase3/runs', 'local', 'runs/lan-checks'],
  ['.phase3', 'local', 'temp/lan-checks'],
  ['.phase4/runs', 'local', 'runs/asset-pipeline'],
  ['.phase4/acceptance', 'local', 'runs/asset-pipeline/acceptance'],
  ['.phase4', 'local', 'temp/asset-pipeline'],
].map(row => Object.freeze(row)));

function legacyRow(identity) {
  const logical = String(identity).replaceAll('\\', '/').replace(/^\.\//, '');
  let best = null;
  for (const row of LEGACY_MAP) {
    if ((logical === row[0] || logical.startsWith(row[0] + '/')) && (!best || row[0].length > best[0].length)) best = row;
  }
  return best ? {row: best, rest: logical.slice(best[0].length).replace(/^\//, '')} : null;
}

function legacyPath(identity, root) {
  const match = legacyRow(identity);
  if (!match) return null;
  const where = roots(root);
  const base = match.row[1] === 'shared' ? where.shared : where.local;
  return path.join(base, ...match.row[2].split('/'), ...(match.rest ? match.rest.split('/') : []));
}

// Realpath of the nearest existing ancestor plus the remaining segments, so links leading out are followed.
function realish(target) {
  let existing = path.resolve(target);
  const rest = [];
  while (!fs.existsSync(existing)) {
    const parent = path.dirname(existing);
    if (parent === existing) break;
    rest.unshift(path.basename(existing));
    existing = parent;
  }
  return path.join(fs.realpathSync.native(existing), ...rest);
}

function insideShared(absolute, root) {
  if (typeof absolute !== 'string' || !path.isAbsolute(absolute)) return false;
  const shared = roots(root).shared;
  if (!fs.existsSync(shared)) return false;
  const fold = value => process.platform === 'win32' ? value.toLowerCase() : value;
  const base = fold(fs.realpathSync.native(shared)), target = fold(realish(absolute));
  const relative = path.relative(base, target);
  return relative !== '' && !path.isAbsolute(relative) && relative.split(path.sep)[0] !== '..';
}

// The first existing component of target, from just below base down to target itself, that is a symbolic link or a
// junction, as a base-relative path with forward slashes; null when there is none. A missing component ends the walk.
function linkedComponent(base, target) {
  const relative = path.relative(base, target);
  if (relative === '') return null;
  if (relative.split(path.sep)[0] === '..' || path.isAbsolute(relative)) {
    throw new Error(target + ' is not inside ' + base);
  }
  const parts = relative.split(path.sep);
  let current = base;
  for (let index = 0; index < parts.length; index++) {
    current = path.join(current, parts[index]);
    let stat;
    try { stat = fs.lstatSync(current); } catch { return null; }
    if (stat.isSymbolicLink()) return parts.slice(0, index + 1).join('/');
  }
  return null;
}

const ignoredShared = new Set();

// Before anything writes the shared root, Git must ignore it in the checkout that holds it, so its binaries can never
// be committed there, even while that checkout is on a branch older than the .local layout. A shared root outside
// every Git working tree needs no check. This never edits info/exclude; the refusal names the line to add.
function assertSharedIgnored(root, env = process.env) {
  const {shared} = roots(root, env);
  if (ignoredShared.has(shared)) return shared;
  let existing = shared;
  while (!fs.existsSync(existing) && path.dirname(existing) !== existing) existing = path.dirname(existing);
  const git = args => {
    const result = spawnSync('git', args, {encoding: 'utf8', windowsHide: true, env});
    if (result.error) {
      fail('GIT_MISSING', 'git could not start (' + result.error.code + '), so the shared root cannot be checked');
    }
    return result;
  };
  const top = git(['-C', existing, 'rev-parse', '--show-toplevel']);
  if (top.status === 0) {
    const checkout = String(top.stdout).trim();
    if (git(['-C', checkout, 'check-ignore', '-q', '--', shared + '/']).status !== 0) {
      fail('SHARED_ROOT_NOT_IGNORED', shared + ' is not ignored by Git in ' + checkout + '; on a branch older than the '
        + '.local layout, add the line /.local/ to that checkout\'s .git/info/exclude (docs/workflow/setup.md)');
    }
  }
  ignoredShared.add(shared);
  return shared;
}

function ensure(...directories) {
  for (const directory of directories) fs.mkdirSync(directory, {recursive: true});
  return directories;
}

const ISOLATION_KEYS = ['DOTNET_CLI_HOME', 'NUGET_PACKAGES', 'NUGET_HTTP_CACHE_PATH', 'NUGET_SCRATCH', 'TEMP', 'TMP',
  'TMPDIR', OVERRIDE];

function printEnv(shell, root) {
  assertSharedIgnored(root);
  const where = locations(root);
  ensure(where.temp(), where.cliHome, where.nuget.packages, where.nuget.http, where.nuget.scratch, where.nuget.feed,
    path.dirname(where.locks.check), path.dirname(where.locks.toolchain));
  const env = isolationEnv(root);
  const quote = value => "'" + String(value).replaceAll("'", shell === 'sh' ? "'\\''" : "''") + "'";
  return ISOLATION_KEYS.map(key => shell === 'sh' ? 'export ' + key + '=' + quote(env[key]) : '$env:' + key + ' = '
    + quote(env[key])).join('\n') + '\n';
}

const USAGE = 'usage: node src/godot-game/tools/workspace-paths.cjs --print-roots [--root <dir>]\n'
  + '       node src/godot-game/tools/workspace-paths.cjs --print-env sh|powershell [--root <dir>]';

function main(args) {
  let mode = null, shell = null, root;
  for (let index = 0; index < args.length; index++) {
    const arg = args[index];
    if (arg === '--print-roots' && !mode) mode = 'roots';
    else if (arg === '--print-env' && !mode && ['sh', 'powershell'].includes(args[index + 1])) { mode = 'env'; shell = args[++index]; }
    else if (arg === '--root' && args[index + 1] && root === undefined) root = path.resolve(args[++index]);
    else { process.stderr.write(USAGE + '\n'); return 2; }
  }
  if (!mode) { process.stderr.write(USAGE + '\n'); return 2; }
  if (mode === 'roots') process.stdout.write(JSON.stringify(roots(root)) + '\n');
  else process.stdout.write(printEnv(shell, root));
  return 0;
}

module.exports = {
  OVERRIDE, LEGACY_MAP, ISOLATION_KEYS, checkoutRoot, overrideProblem, roots, locations, isolationEnv, legacyPath,
  insideShared, linkedComponent, assertSharedIgnored, ensure, printEnv,
};

if (require.main === module) {
  try { process.exitCode = main(process.argv.slice(2)); }
  catch (error) { process.stderr.write((error.diagnostic ? '' : 'WORKSPACE_PATHS: ') + error.message + '\n'); process.exitCode = 1; }
}
