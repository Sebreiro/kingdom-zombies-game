'use strict';
// Derives and checks the ac-08 repository-stage bindings, and the caller hashes and repository stages that repeat them,
// from the bytes at a revision. --check also requires one ExemptionTests theory row per derived stage id.
// It regenerates the bindings from current bytes; it is not a replay of a one-shot binding utility.
//   node src/godot-game/tools/verification/ac08-bindings.cjs [--check] [--rev <revision>]   (default: --check --rev HEAD)
//   node src/godot-game/tools/verification/ac08-bindings.cjs --write                        (derives from HEAD)
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {spawnSync} = require('node:child_process');

const ROOT = path.resolve(__dirname, '../../../..');
const CATALOG = 'src/godot-game/tools/verification/required-cases.json';
const CALLER_CATALOG = 'src/godot-game/tools/gameplay-checks/required-cases.json';
const CATALOGS = [CATALOG, CALLER_CATALOG];
const CASE_ID = 'ac-08.complete';
const WORKSPACE = 'src/godot-game/';
const TEST_PROJECT = '/KingdomZombies.Phase1.Tests.csproj';
const FORMAT_FILE = 'SourceFileFormat.cs';
const EXCLUDED_ROOTS = ['.claude/', '.codex/', '.agents/', '.planning/'];
const SOURCES_ACTION = 'repository-sources';
const CHUNK = 16;
const KEY_COUNT = 42;
// A caller repeats the ac-08 auth, shape and source stages as its 'repository' stages (check.cjs callerExpected), and
// each stage id must be a data row of the xUnit theory that runs it (check.cjs producer discovery).
const CALLER_ACTION = 'repository';
const CALLER_SOURCE_ACTIONS = ['repository-auth', 'repository-shape', SOURCES_ACTION];
const CALLER_BASE_MS = 180000;
const CALLER_STAGE_MS = 48000;
const THEORY_FILE = 'src/godot-game/tests/verification/ExemptionTests.cs';

class BindingError extends Error {
  constructor(code, message) {
    super(code + ': ' + message);
    this.code = code;
  }
}

function sha256(bytes) {
  return crypto.createHash('sha256').update(bytes).digest('hex');
}

function serialize(value) {
  return JSON.stringify(value, null, 2) + '\n';
}

function git(root, args, input) {
  const result = spawnSync('git', args, {cwd: root, input, maxBuffer: 1 << 30, windowsHide: true});
  if (result.error || result.status !== 0)
    throw new BindingError('GIT', 'git ' + args.join(' ') + ' exit ' + result.status + ' ' + String(result.stderr || result.error));
  return result.stdout;
}

function resolveRevision(root, revision) {
  return git(root, ['rev-parse', '--verify', revision + '^{commit}']).toString('utf8').trim();
}

/** Lists every tracked path at a revision. */
function trackedPaths(root, revision) {
  return git(root, ['ls-tree', '-r', '-z', '--name-only', revision]).toString('utf8').split('\0').filter(Boolean);
}

/** Reads the exact blob bytes of each path at a revision; a missing or non-blob path throws. */
function blobs(root, revision, paths) {
  const map = new Map();
  if (!paths.length) return map;
  const output = git(root, ['cat-file', '--batch'], paths.map(file => revision + ':' + file + '\n').join(''));
  let index = 0;
  for (const file of paths) {
    const end = output.indexOf(10, index);
    const header = output.toString('latin1', index, end).split(' ');
    if (header[1] !== 'blob') throw new BindingError('GIT', 'no blob for ' + revision + ':' + file + ' (' + header.join(' ') + ')');
    const size = Number(header[2]);
    map.set(file, output.subarray(end + 1, end + 1 + size));
    index = end + 2 + size;
  }
  return map;
}

function extension(file) {
  const name = file.split('/').pop(), dot = name.lastIndexOf('.');
  return dot < 0 ? '' : name.slice(dot).toLowerCase();
}

function kindOf(file) {
  const value = extension(file);
  if (value === '.cs') return 'source';
  if (value === '.editorconfig') return 'configuration';
  return 'outside-validator';
}

function folder(file) {
  return file.slice(0, file.lastIndexOf('/') + 1);
}

function excluded(file) {
  return EXCLUDED_ROOTS.some(prefix => file.startsWith(prefix));
}

function repositoryCase(catalog) {
  const item = (catalog.cases || []).find(row => row.id === CASE_ID);
  if (!item || !item.repositoryStages) throw new BindingError('AC08_CASE', CASE_ID + ' has no repositoryStages');
  return item;
}

/** Rebuilds the repository-sources stages in place of the declared ones, keeping every other stage in order. */
function rebuildStages(stages, sources) {
  const first = stages.findIndex(stage => stage.action === SOURCES_ACTION);
  if (first < 0) throw new BindingError('AC08_STAGES', 'no ' + SOURCES_ACTION + ' stage to replace');
  const checked = sources.filter(row => row.kind !== 'outside-validator').map(row => row.path);
  const chunks = [];
  for (let index = 0; index < checked.length; index += CHUNK)
    chunks.push({id: SOURCES_ACTION + '-' + chunks.length, action: SOURCES_ACTION, sources: checked.slice(index, index + CHUNK)});
  const others = stages.filter(stage => stage.action !== SOURCES_ACTION);
  const before = stages.slice(0, first).filter(stage => stage.action !== SOURCES_ACTION).length;
  return [...others.slice(0, before), ...chunks, ...others.slice(before)];
}

/**
 * Derives the ac-08 repositoryStages block at a revision. Keys, cohorts, method, scope and the non-source stages are
 * copied from the declared block; the path set, kinds, owners, hashes, source stages and bindings come from the bytes.
 */
function derive(root, revision, declared) {
  const projects = declared.projects || [];
  const testProject = projects.find(project => project.endsWith(TEST_PROJECT));
  if (!testProject) throw new BindingError('AC08_PROJECTS', 'no test project in projects');
  const tree = trackedPaths(root, revision);
  const tracked = tree.filter(file => file.startsWith(WORKSPACE) && file.endsWith('.csproj')).sort();
  if (JSON.stringify([...projects].sort()) !== JSON.stringify(tracked))
    throw new BindingError('AC08_PROJECTS', 'projects ' + JSON.stringify(projects) + ' differ from tracked ' + JSON.stringify(tracked));
  const meta = blobs(root, revision, [folder(testProject) + FORMAT_FILE, testProject]);
  const format = meta.get(folder(testProject) + FORMAT_FILE).toString('utf8');
  const set = /HashSet<string> _binaryExtensions[^{]*\{([^}]*)\}/.exec(format);
  if (!set) throw new BindingError('AC08_FORMAT', 'binary extension set not found in ' + FORMAT_FILE);
  const binary = new Set([...set[1].matchAll(/"(\.[a-z0-9]+)"/g)].map(match => match[1]));
  const linked = new Set([...meta.get(testProject).toString('utf8').matchAll(/<Compile\s+Include="([^"]+)"/g)]
    .map(match => path.posix.normalize(folder(testProject) + match[1])));
  const eligible = tree.filter(file => !excluded(file) && !binary.has(extension(file))).sort();
  const content = blobs(root, revision, eligible.filter(file => kindOf(file) !== 'outside-validator'));
  const sources = eligible.map(file => {
    const kind = kindOf(file);
    const owners = kind === 'source'
      ? projects.filter(project => file.startsWith(folder(project)) || (project === testProject && linked.has(file)))
      : [];
    return {path: file, kind, owners, sha256: kind === 'outside-validator' ? null : sha256(content.get(file))};
  });
  const registers = blobs(root, revision, (declared.bindings || []).map(binding => binding.path));
  const derived = {};
  for (const key of Object.keys(declared)) {
    if (key === 'sources') derived.sources = sources;
    else if (key === 'sourceSetSha256') derived.sourceSetSha256 = sha256(JSON.stringify(sources));
    else if (key === 'bindings') derived.bindings = declared.bindings.map(binding => ({...binding, sha256: sha256(registers.get(binding.path))}));
    else if (key === 'stages') derived.stages = rebuildStages(declared.stages, sources);
    else derived[key] = declared[key];
  }
  return derived;
}

/** Names every difference between a derived and a declared block, sources first by path. */
function compare(derived, declared) {
  const differences = [];
  const want = new Map(derived.sources.map(row => [row.path, row]));
  const have = new Map((declared.sources || []).map(row => [row.path, row]));
  for (const file of have.keys()) if (!want.has(file)) differences.push('phantom row ' + file);
  for (const file of want.keys()) if (!have.has(file)) differences.push('missing row ' + file);
  for (const [file, row] of want) {
    const other = have.get(file);
    if (!other) continue;
    for (const field of ['kind', 'owners', 'sha256'])
      if (JSON.stringify(row[field]) !== JSON.stringify(other[field])) differences.push('row ' + file + ' ' + field);
  }
  if (!differences.length && JSON.stringify(derived.sources) !== JSON.stringify(declared.sources)) differences.push('source row order');
  for (const key of new Set([...Object.keys(derived), ...Object.keys(declared)])) {
    if (key === 'sources') continue;
    if (JSON.stringify(derived[key]) !== JSON.stringify(declared[key])) differences.push('field ' + key);
  }
  if (!Array.isArray(declared.keys) || declared.keys.length !== KEY_COUNT)
    differences.push('keys ' + (declared.keys && declared.keys.length) + ' expected ' + KEY_COUNT);
  return differences;
}

function callers(catalogs) {
  return catalogs.flatMap(catalog => (catalog.cases || []).filter(item => item.callerStages));
}

/**
 * Derives a caller's stage list from the ac-08 stages: its contiguous 'repository' run becomes one stage per ac-08
 * auth, shape and source stage, under the caller's own id prefix, and overallMs follows the stage count. A caller
 * with no 'repository' stage is returned unchanged.
 */
function deriveCallerStages(callerStages, repositoryStages) {
  const stages = callerStages.stages;
  const first = stages.findIndex(stage => stage.action === CALLER_ACTION);
  if (first < 0) return callerStages;
  const count = stages.filter(stage => stage.action === CALLER_ACTION).length;
  if (stages.slice(first, first + count).some(stage => stage.action !== CALLER_ACTION))
    throw new BindingError('AC08_CALLER', 'repository stages are not contiguous in ' + stages[first].id);
  const sample = stages[first], suffix = '-' + sample.sourceStageId;
  if (typeof sample.sourceStageId !== 'string' || !sample.id.endsWith(suffix) || sample.id.length === suffix.length)
    throw new BindingError('AC08_CALLER', 'no caller prefix in ' + sample.id);
  const prefix = sample.id.slice(0, -suffix.length);
  const run = repositoryStages.filter(stage => CALLER_SOURCE_ACTIONS.includes(stage.action))
    .map(stage => ({id: prefix + '-' + stage.id, action: CALLER_ACTION, sourceStageId: stage.id}));
  const next = [...stages.slice(0, first), ...run, ...stages.slice(first + count)];
  return {...callerStages, stages: next, overallMs: CALLER_BASE_MS + next.length * CALLER_STAGE_MS};
}

/** Applies the derived ac-08 block to both catalogs: the repository case, then every caller's hash and stages. */
function applyDerived(catalogs, derived) {
  repositoryCase(catalogs[0]).repositoryStages = derived;
  const repositorySha256 = sha256(JSON.stringify(derived));
  for (const row of callers(catalogs)) {
    row.callerStages = deriveCallerStages(row.callerStages, derived.stages);
    row.callerStages.repositorySha256 = repositorySha256;
  }
  return repositorySha256;
}

/** Reads the string data rows of the [InlineData] attributes directly above a theory method. */
function theoryRows(text, method) {
  const lines = text.split(/\r?\n/);
  const at = lines.findIndex(line => new RegExp('\\bvoid\\s+' + method + '\\s*\\(').test(line));
  if (at < 0) throw new BindingError('AC08_THEORY', method + ' not found in ' + THEORY_FILE);
  const rows = [];
  for (let index = at - 1; index >= 0 && lines[index].trim().startsWith('['); index--) {
    const match = /^\[InlineData\("([^"]+)"\)\]$/.exec(lines[index].trim());
    if (match) rows.unshift(match[1]);
  }
  return rows;
}

/** Names every stage id the theories lack and every theory row no stage declares. */
function theoryDifferences(text, derived, rows) {
  const differences = [];
  const pairs = [[derived.stages.map(stage => stage.id), derived.method]];
  const methods = new Set(rows.map(item => item.callerStages.method));
  for (const method of methods)
    pairs.push([rows.filter(item => item.callerStages.method === method).flatMap(item => item.callerStages.stages.map(stage => stage.id)), method]);
  for (const [ids, method] of pairs) {
    const name = method.split('.').pop(), declared = theoryRows(text, name), want = new Set(ids), have = new Set(declared);
    for (const id of ids) if (!have.has(id)) differences.push('theory ' + name + ' lacks ' + id);
    for (const id of declared) if (!want.has(id)) differences.push('theory ' + name + ' extra ' + id);
    if (have.size !== declared.length) differences.push('theory ' + name + ' duplicate row');
  }
  return differences;
}

/** Checks the catalogs committed at a revision against the derivation from that revision's bytes. */
function check(root, revision = 'HEAD') {
  const commit = resolveRevision(root, revision);
  const bytes = blobs(root, commit, CATALOGS);
  const catalogs = CATALOGS.map(file => JSON.parse(bytes.get(file).toString('utf8')));
  const declared = repositoryCase(catalogs[0]).repositoryStages;
  const derived = derive(root, commit, declared);
  const differences = compare(derived, declared);
  const repositorySha256 = sha256(JSON.stringify(derived));
  const rows = callers(catalogs), wanted = [];
  for (const item of rows) {
    if (item.callerStages.repositorySha256 !== repositorySha256) differences.push('caller ' + item.id);
    const want = deriveCallerStages(item.callerStages, derived.stages);
    if (JSON.stringify(want.stages) !== JSON.stringify(item.callerStages.stages)) differences.push('caller ' + item.id + ' stages');
    if (want.overallMs !== item.callerStages.overallMs) differences.push('caller ' + item.id + ' overallMs');
    wanted.push({...item, callerStages: want});
  }
  // The theories are compared with the derived stage ids, so a stale catalog cannot hide a missing row.
  differences.push(...theoryDifferences(blobs(root, commit, [THEORY_FILE]).get(THEORY_FILE).toString('utf8'), derived, wanted));
  return {
    revision: commit, differences, repositorySha256, callers: rows.length, rows: derived.sources.length,
    checked: derived.sources.filter(row => row.kind !== 'outside-validator').length,
    sourceStages: derived.stages.filter(stage => stage.action === SOURCES_ACTION).length
  };
}

/** Returns the status lines that make --write unsafe: anything but the two catalogs and the excluded roots. */
function dirtyEntries(lines) {
  return lines.filter(line => line.trim()).filter(line => {
    const files = line.slice(3).split(' -> ');
    return !files.every(file => CATALOGS.includes(file) || excluded(file));
  });
}

function refuseDirty(lines) {
  const dirty = dirtyEntries(lines);
  if (dirty.length) throw new BindingError('AC08_DIRTY_TREE', 'commit or discard these changes first: ' + dirty.slice(0, 8).join(' | '));
}

/** Rewrites the ac-08 block and every caller hash in both catalogs from the HEAD bytes. */
function write(root) {
  refuseDirty(git(root, ['status', '--porcelain=v1']).toString('utf8').split('\n'));
  const catalogs = CATALOGS.map(file => JSON.parse(fs.readFileSync(path.join(root, file), 'utf8')));
  const item = repositoryCase(catalogs[0]), declared = item.repositoryStages;
  const derived = derive(root, 'HEAD', declared);
  if (JSON.stringify(derived.keys) !== JSON.stringify(declared.keys) || JSON.stringify(derived.cohorts) !== JSON.stringify(declared.cohorts))
    throw new BindingError('AC08_KEYS', 'keys or cohorts changed');
  if (derived.keys.length !== KEY_COUNT) throw new BindingError('AC08_KEYS', derived.keys.length + ' keys expected ' + KEY_COUNT);
  const repositorySha256 = applyDerived(catalogs, derived);
  const missing = theoryDifferences(fs.readFileSync(path.join(root, THEORY_FILE), 'utf8'), derived, callers(catalogs));
  if (missing.length) throw new BindingError('AC08_THEORY', 'add or remove these [InlineData] rows first: ' + missing.slice(0, 8).join(' | '));
  CATALOGS.forEach((file, index) => fs.writeFileSync(path.join(root, file), serialize(catalogs[index])));
  return {repositorySha256, callers: callers(catalogs).length, rows: derived.sources.length};
}

function parseArgs(argv) {
  const options = {mode: 'check', revision: 'HEAD'};
  for (let index = 0; index < argv.length; index++) {
    const argument = argv[index];
    if (argument === '--check') options.mode = 'check';
    else if (argument === '--write') options.mode = 'write';
    else if (argument === '--rev' && index + 1 < argv.length) options.revision = argv[++index];
    else throw new BindingError('USAGE', 'node ac08-bindings.cjs [--check] [--rev <revision>] | --write');
  }
  if (options.mode === 'write' && options.revision !== 'HEAD') throw new BindingError('USAGE', '--write derives from HEAD only');
  return options;
}

function main(argv) {
  const options = parseArgs(argv);
  if (options.mode === 'write') {
    const result = write(ROOT);
    console.log('WROTE ac08-bindings rows=' + result.rows + ' callers=' + result.callers + ' repositorySha256=' + result.repositorySha256);
    return 0;
  }
  const result = check(ROOT, options.revision);
  if (result.differences.length) {
    console.error('FAIL ac08-bindings ' + result.revision.slice(0, 12) + ': ' + result.differences.length + ' difference(s): '
      + result.differences.slice(0, 12).join(' | '));
    return 1;
  }
  console.log('PASS ac08-bindings ' + result.revision.slice(0, 12) + ' rows=' + result.rows + ' checked=' + result.checked
    + ' sourceStages=' + result.sourceStages + ' callers=' + result.callers + ' repositorySha256=' + result.repositorySha256);
  return 0;
}

module.exports = {
  CATALOG, CALLER_CATALOG, CATALOGS, CASE_ID, KEY_COUNT, BindingError, sha256, serialize, blobs, trackedPaths, kindOf,
  rebuildStages, derive, compare, check, dirtyEntries, refuseDirty, write, repositoryCase, deriveCallerStages, applyDerived,
  theoryRows, theoryDifferences, THEORY_FILE
};

if (require.main === module) {
  try {
    process.exitCode = main(process.argv.slice(2));
  } catch (error) {
    console.error('FAIL ac08-bindings: ' + (error instanceof BindingError ? error.message : error.stack));
    process.exitCode = 1;
  }
}
