'use strict';
// Proves the rebinding tools on committed bytes, shows each tool and gate failing on planted inputs, and pairs every
// failing control with an input that passes. Nothing here writes to the repository; temporary repositories live in the
// system temp folder. Planted machine paths and legacy folder names are built from fragments at runtime, so this file
// itself matches neither the newhost nor the residue pattern.
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const {spawnSync} = require('node:child_process');
const ac08 = require('./ac08-bindings.cjs');
const assets = require('./asset-bindings.cjs');
const gates = require('./rebind-gates.cjs');

const ROOT = path.resolve(__dirname, '../../../..');
const GATES_CLI = path.join(__dirname, 'rebind-gates.cjs');
const LEGACY = ['.', 'phase', '1'].join('');
const machinePath = user => ['C', ':', '/', 'Users', '/', user, '/', 'work'].join('');

function git(cwd, ...args) {
  const result = spawnSync('git', ['-c', 'user.name=t', '-c', 'user.email=t@example.invalid', '-c', 'core.autocrlf=false', ...args],
    {cwd, encoding: 'utf8', windowsHide: true});
  assert.equal(result.status, 0, 'git ' + args.join(' ') + ': ' + result.stderr);
  return result.stdout.trim();
}

function reproductionRevision() {
  return git(ROOT, 'log', '-1', '--format=%H', '--', ac08.CATALOG);
}

function committedCatalogs(revision) {
  const bytes = ac08.blobs(ROOT, revision, ac08.CATALOGS);
  return {bytes, catalogs: ac08.CATALOGS.map(file => JSON.parse(bytes.get(file).toString('utf8')))};
}

let derivation;
function derivedAtReproduction() {
  if (!derivation) {
    const revision = reproductionRevision(), {bytes, catalogs} = committedCatalogs(revision);
    const declared = ac08.repositoryCase(catalogs[0]).repositoryStages;
    derivation = {revision, bytes, catalogs, declared, derived: ac08.derive(ROOT, revision, declared)};
  }
  return derivation;
}

test('ac-08 derivation reproduces both committed catalogs byte for byte at the last catalog commit', () => {
  const {revision, bytes, catalogs, declared, derived} = derivedAtReproduction();
  assert.deepEqual(ac08.compare(derived, declared), []);
  assert.equal(derived.keys.length, ac08.KEY_COUNT);
  const copy = structuredClone(catalogs);
  ac08.repositoryCase(copy[0]).repositoryStages = derived;
  const repositorySha256 = ac08.sha256(JSON.stringify(derived));
  let callers = 0;
  for (const catalog of copy) for (const item of catalog.cases) if (item.callerStages) { item.callerStages.repositorySha256 = repositorySha256; callers++; }
  assert.equal(callers, 6);
  ac08.CATALOGS.forEach((file, index) => assert.equal(ac08.serialize(copy[index]), bytes.get(file).toString('utf8'), file));
  const result = ac08.check(ROOT, revision);
  assert.deepEqual(result.differences, []);
  assert.equal(result.callers, 6);
});

test('ac-08 negative controls name the planted row, and an unchanged copy passes', () => {
  const {derived} = derivedAtReproduction();
  assert.deepEqual(ac08.compare(derived, structuredClone(derived)), []);
  const controls = {
    'flipped source sha': block => {
      const row = block.sources.find(item => item.sha256);
      row.sha256 = (row.sha256[0] === '0' ? '1' : '0') + row.sha256.slice(1);
      return 'row ' + row.path + ' sha256';
    },
    'phantom path': block => {
      block.sources.push({path: 'src/godot-game/core/Phantom.cs', kind: 'source', owners: [], sha256: '0'.repeat(64)});
      return 'phantom row src/godot-game/core/Phantom.cs';
    },
    'dropped checked row': block => {
      const [row] = block.sources.splice(block.sources.findIndex(item => item.kind === 'source'), 1);
      return 'missing row ' + row.path;
    },
    'wrong owner': block => {
      const row = block.sources.find(item => item.owners.length === 1);
      row.owners = [block.projects.find(project => project !== row.owners[0])];
      return 'row ' + row.path + ' owners';
    }
  };
  for (const [name, plant] of Object.entries(controls)) {
    const declared = structuredClone(derived), expected = plant(declared);
    assert.ok(ac08.compare(derived, declared).includes(expected), name + ' must name ' + expected);
  }
});

test('ac-08 --write refuses a dirty tree, but not the two catalogs or the excluded roots', () => {
  assert.deepEqual(ac08.dirtyEntries([' M ' + ac08.CATALOG, ' M ' + ac08.CALLER_CATALOG, '?? .planning/quick/x/', '']), []);
  assert.doesNotThrow(() => ac08.refuseDirty([' M ' + ac08.CATALOG, '?? .planning/quick/x/']));
  assert.deepEqual(ac08.dirtyEntries([' M src/godot-game/core/ExampleState.cs']), [' M src/godot-game/core/ExampleState.cs']);
  assert.throws(() => ac08.refuseDirty(['?? src/godot-game/core/New.cs']), error => error.code === 'AC08_DIRTY_TREE');
  assert.throws(() => ac08.refuseDirty(['R  docs/a.md -> ' + ac08.CATALOG]), error => error.code === 'AC08_DIRTY_TREE');
});

test('asset cascade check passes on the current tree', () => {
  const result = assets.check(ROOT);
  assert.deepEqual(result.problems, []);
  assert.equal(result.modules, gates.EXPECTED_MODULES);
  assert.equal(result.valid, assets.VALID_OUTPUTS);
});

test('delivery fixture catalog is gzip level 9 of the catalog bytes, and level 6 is detected', () => {
  const content = require(path.join(ROOT, assets.PIPELINE, 'content.cjs'));
  const textData = require(path.join(ROOT, assets.PIPELINE, 'cases/text-data.cjs'));
  const delivery = JSON.parse(fs.readFileSync(content.inside(ROOT, textData.DELIVERY), 'utf8'));
  const bytes = content.bytes(ROOT, textData.SUPPORT_CATALOG);
  assert.equal(assets.gzipPayload(bytes), delivery.fixtureCatalog.gzipBase64);
  assert.deepEqual(assets.fixtureDiffs(delivery.fixtureCatalog, assets.fixtureCatalogFor(bytes, delivery.fixtureCatalog)), []);
  const level6 = {...delivery.fixtureCatalog, gzipBase64: assets.gzipPayload(bytes, 6)};
  assert.notEqual(level6.gzipBase64, delivery.fixtureCatalog.gzipBase64);
  assert.deepEqual(assets.fixtureDiffs(level6, assets.fixtureCatalogFor(bytes, delivery.fixtureCatalog)), ['delivery fixtureCatalog gzipBase64']);
});

test('one changed module byte names that module, and the unchanged bytes pass', () => {
  const catalog = JSON.parse(fs.readFileSync(path.join(ROOT, assets.CATALOG), 'utf8'));
  const read = file => fs.readFileSync(path.join(ROOT, file));
  assert.deepEqual(assets.moduleDiffs(catalog, read), []);
  const target = assets.moduleRows(catalog)[0].module;
  const changed = file => {
    const bytes = Buffer.from(read(file));
    if (file === target) bytes[0] ^= 1;
    return bytes;
  };
  assert.deepEqual(assets.moduleDiffs(catalog, changed), [target + ' stale moduleSha256']);
});

// ---- gate judges: every gate fails on a planted input and passes on its paired input -----------------------------

function fails(verdict, ...fragments) {
  assert.equal(verdict.ok, false, 'expected FAIL, got ' + JSON.stringify(verdict));
  for (const fragment of fragments) assert.ok(verdict.reason.includes(fragment), verdict.reason + ' must name ' + fragment);
}

function passes(verdict) {
  assert.equal(verdict.ok, true, 'expected PASS, got ' + JSON.stringify(verdict));
}

test('history judge: changed blobs under each root, a changed --keep file and a deleted record fail', () => {
  const records = [...gates.HISTORY_ROOTS.map(prefix => prefix + 'r.md'), 'docs/keep.md'].map(file => ({path: file, base: 'a', head: 'a'}));
  passes(gates.judgeHistory({unknownKeep: [], records}));
  for (const record of records) {
    const changed = records.map(item => item === record ? {...item, head: 'b'} : item);
    fails(gates.judgeHistory({unknownKeep: [], records: changed}), 'changed ' + record.path);
  }
  fails(gates.judgeHistory({unknownKeep: [], records: records.map((item, index) => index ? item : {...item, head: null})}), 'deleted ' + records[0].path);
  fails(gates.judgeHistory({unknownKeep: ['docs/typo'], records}), 'unknown pathspec');
  fails(gates.judgeHistory({unknownKeep: [], records: []}), 'no history record');
});

test('newhost judge: a new machine-path line under another user name fails; the same text at base passes', () => {
  const text = 'see ' + machinePath('another');
  assert.ok(gates.HOST.test(text));
  assert.ok(gates.HOST.test(['x ', 'Users', '\\', 'zed'].join('')), 'any user name matches');
  fails(gates.judgeNewhost({baseTexts: [], headLines: [{at: 'docs/a.md:3', text}]}), 'docs/a.md:3');
  passes(gates.judgeNewhost({baseTexts: [text], headLines: [{at: 'docs/a.md:3', text}]}));
});

test('residue judge: a hit outside --allow, an =N entry off by one and a git failure fail', () => {
  const hits = [{path: 'src/a.cjs', line: 1}, {path: 'src/a.cjs', line: 2}, {path: 'src/b.cjs', line: 5},
    {path: '.planning/phases/x.md', line: 1}, {path: 'docs/kept.md', line: 1}];
  const options = allow => ({keep: ['docs/kept.md'], allow});
  passes(gates.judgeResidue({unknown: [], hits}, options([{spec: 'src/a.cjs', count: 2}, {spec: 'src/b.cjs', count: null}])));
  fails(gates.judgeResidue({unknown: [], hits}, options([{spec: 'src/a.cjs', count: 2}])), 'src/b.cjs');
  fails(gates.judgeResidue({unknown: [], hits}, options([{spec: 'src', count: null}, {spec: 'src/a.cjs', count: 1}])), 'retains 2', 'expected 1');
  fails(gates.judgeResidue({unknown: [], hits}, options([{spec: 'src', count: null}, {spec: 'src/a.cjs', count: 3}])), 'retains 2', 'expected 3');
  fails(gates.judgeResidue({unknown: ['--scope nope'], hits: []}, options([])), 'unknown pathspec');
  const collect = gates.GATES.residue.collect;
  gates.GATES.residue.collect = () => { throw new gates.GateFailure('git grep exit 2'); };
  try { fails(gates.runGate('residue', {repoSet: false}), 'exit 2'); } finally { gates.GATES.residue.collect = collect; }
});

test('lists judge: a runner count off its catalog, a catalog off base and a runner exit fail', () => {
  const rows = ['verification', 'gameplay-checks'].map(runner => ({runner, exit: 0, error: '', listed: 10, catalog: 10, base: 10}));
  passes(gates.judgeLists(rows));
  fails(gates.judgeLists(rows.map((row, index) => index ? row : {...row, listed: 9})), 'verification lists 9');
  fails(gates.judgeLists(rows.map((row, index) => index ? row : {...row, listed: 11, catalog: 11})), 'base has 10');
  fails(gates.judgeLists(rows.map((row, index) => index ? row : {...row, exit: 1, listed: null})), 'exit 1');
  fails(gates.judgeLists(rows.map((row, index) => index ? row : {...row, base: null})), 'base catalog absent');
  fails(gates.judgeLists([]), 'no runner');
});

test('tests judge: fail 1, pass 0 and no test file fail', () => {
  passes(gates.judgeTests({files: ['a.test.cjs'], exit: 0, pass: 5, fail: 0, output: ''}));
  fails(gates.judgeTests({files: ['a.test.cjs'], exit: 1, pass: 4, fail: 1, output: ''}), 'fail 1');
  fails(gates.judgeTests({files: [], exit: null, pass: null, fail: null, output: ''}), 'no test file');
  fails(gates.judgeTests({files: ['a.test.cjs'], exit: 0, pass: 0, fail: 0, output: ''}), 'pass 0');
  fails(gates.judgeTests({files: ['a.test.cjs'], exit: 0, pass: null, fail: null, output: ''}), 'unparseable');
});

test('roots judge: shared equal to local from a linked worktree, or shared away from the common dir, fails', () => {
  const at = (...parts) => path.join(os.tmpdir(), 'kz-roots', ...parts);
  const facts = {
    checkout: at('worktree'), common: at('main', '.git'), main: at('main'), override: null, linked: true,
    printed: {local: at('worktree', '.local'), shared: at('main', '.local'), source: 'git'},
    printedMain: {local: at('main', '.local'), shared: at('main', '.local'), source: 'git'}
  };
  passes(gates.judgeRoots(facts));
  fails(gates.judgeRoots({...facts, printed: {...facts.printed, shared: at('worktree', '.local')}}), 'shared');
  fails(gates.judgeRoots({...facts, override: at('worktree', '.local'), printed: {...facts.printed, shared: at('worktree', '.local')},
    printedMain: {...facts.printedMain, shared: at('worktree', '.local')}}), 'shared equals local');
  fails(gates.judgeRoots({...facts, printed: {...facts.printed, shared: at('elsewhere', '.local')}}), 'git common dir');
  fails(gates.judgeRoots({...facts, printedMain: {...facts.printedMain, shared: at('elsewhere', '.local')}}), 'main checkout shared');
});

test('build judge: a network GET, a warning, an error and a missing summary line fail', () => {
  const summary = (warnings, errors) => 'Build succeeded.\n    ' + warnings + ' Warning(s)\n    ' + errors + ' Error(s)\n';
  const facts = {restore: {exit: 0, log: 'Restored KingdomZombies.slnx'}, build: {exit: 0, log: summary(0, 0)}};
  passes(gates.judgeBuild(facts));
  fails(gates.judgeBuild({...facts, restore: {exit: 0, log: '  GET https://api.nuget.org/v3/index.json'}}), 'network');
  fails(gates.judgeBuild({...facts, build: {exit: 0, log: summary(1, 0)}}), '1 Warning(s)');
  fails(gates.judgeBuild({...facts, build: {exit: 1, log: summary(0, 1)}}), '1 Error(s)');
  fails(gates.judgeBuild({...facts, build: {exit: 0, log: 'Build succeeded.'}}), 'summary line missing');
  fails(gates.judgeBuild({restore: {exit: 1, log: 'NU1004'}, build: null}), 'restore exit 1');
});

test('assets judge: 422 module bindings against base 423 and a failed check fail', () => {
  const facts = {exit: 0, output: '', modules: 423, baseModules: 423};
  passes(gates.judgeAssets(facts));
  fails(gates.judgeAssets({...facts, modules: 422}), '422 module bindings, base has 423');
  fails(gates.judgeAssets({...facts, exit: 1, output: 'FAIL asset-bindings'}), 'exit 1');
  fails(gates.judgeAssets({...facts, baseModules: null}), 'base asset catalog absent');
});

test('ac08 judge: 8 staged plans, a registry hash off its file and a stale callerAuthority fail', () => {
  const facts = {
    checkExit: 0, checkOutput: '', dirtyCatalogs: [], disk: [], authority: {path: 'docs/code/r.json', declared: 'a', actual: 'a'},
    registries: [{case: 'engine.x', path: 'docs/code/s.json', declared: 'b', actual: 'b'}],
    callers: Array.from({length: 6}, (_, index) => ({id: 'c' + index, declared: 'h'})), expectedRepositorySha256: 'h',
    plans: Array.from({length: 9}, (_, index) => ({id: 'p' + index, ok: true}))
  };
  passes(gates.judgeAc08(facts));
  fails(gates.judgeAc08({...facts, plans: facts.plans.slice(1)}), '8 staged plans');
  fails(gates.judgeAc08({...facts, registries: [{...facts.registries[0], actual: 'c'}]}), 'docs/code/s.json');
  fails(gates.judgeAc08({...facts, authority: {...facts.authority, actual: 'z'}}), 'stale callerAuthority');
  fails(gates.judgeAc08({...facts, callers: facts.callers.map((row, index) => index ? row : {...row, declared: 'old'})}), 'stale caller', 'c0');
  fails(gates.judgeAc08({...facts, checkExit: 1}), 'exit 1');
  fails(gates.judgeAc08({...facts, disk: ['src/a.cs']}), 'src/a.cs');
});

test('docs judge: PASS 35/35 against base 36, a head count of 35, a runner exit and a missing line fail', () => {
  const facts = {base: 'b', nBase: 36, nHead: 36, launched: true, exit: 0, stdout: 'PASS 36/36 selected obligations; [summary](r/summary.json)\n'};
  passes(gates.judgeDocs(facts));
  fails(gates.judgeDocs({...facts, stdout: 'PASS 35/35 selected obligations; [summary](r/summary.json)\n'}), 'PASS 36/36');
  fails(gates.judgeDocs({...facts, nHead: 35, launched: false}), 'head docs count 35');
  assert.equal(gates.docsPrecondition({...facts, nHead: 35}), 'head docs count 35 differs from base 36');
  fails(gates.judgeDocs({...facts, exit: 1}), 'exit 1');
  fails(gates.judgeDocs({...facts, stdout: ''}), 'missing');
  fails(gates.judgeDocs({...facts, nBase: null, launched: false}), 'base catalog absent');
});

// ---- gates end to end on a temporary repository ------------------------------------------------------------------

function gate(repository, base, ...args) {
  const result = spawnSync(process.execPath, [GATES_CLI, '--repo', repository, '--base', base, '--keep', 'keep',
    '--allow', 'src/allowed.txt=2', ...args], {encoding: 'utf8', windowsHide: true});
  return {exit: result.status, output: result.stdout + result.stderr};
}

function write(root, file, text) {
  fs.mkdirSync(path.dirname(path.join(root, file)), {recursive: true});
  fs.writeFileSync(path.join(root, file), text);
}

test('history, newhost and residue fail on planted changes through the CLI and pass on a clean HEAD', t => {
  const repository = fs.mkdtempSync(path.join(os.tmpdir(), 'kz-rebind-gates-'));
  t.after(() => fs.rmSync(repository, {recursive: true, force: true, maxRetries: 3}));
  git(repository, 'init', '-q', '-b', 'main');
  write(repository, '.planning/phases/01-a/record.md', 'record\n');
  write(repository, '.planning/quick/q1/record.md', 'quick\n');
  write(repository, 'keep/file.txt', 'keep\n');
  write(repository, 'src/allowed.txt', LEGACY + '/runs one\n' + LEGACY + '/runs two\n');
  write(repository, 'src/other.txt', 'plain\n');
  git(repository, 'add', '.');
  git(repository, 'commit', '-q', '-m', 'base');
  const base = git(repository, 'rev-parse', 'HEAD');

  git(repository, 'checkout', '-q', '-b', 'planted');
  write(repository, '.planning/phases/01-a/record.md', 'record changed\n');
  write(repository, 'keep/file.txt', 'keep changed\n');
  fs.rmSync(path.join(repository, '.planning/quick/q1/record.md'));
  write(repository, 'src/other.txt', 'plain\nsee ' + machinePath('tester') + '\n' + LEGACY + '/temp\n');
  write(repository, 'src/allowed.txt', LEGACY + '/runs one\n' + LEGACY + '/runs two\n' + LEGACY + '/runs three\n');
  git(repository, 'add', '-A');
  git(repository, 'commit', '-q', '-m', 'planted');

  const history = gate(repository, base, 'history');
  assert.equal(history.exit, 1, history.output);
  for (const file of ['.planning/phases/01-a/record.md', 'keep/file.txt', '.planning/quick/q1/record.md'])
    assert.ok(history.output.includes(file), file + ' in ' + history.output);
  const newhost = gate(repository, base, 'newhost');
  assert.equal(newhost.exit, 1, newhost.output);
  assert.ok(newhost.output.includes('src/other.txt:2'), newhost.output);
  const residue = gate(repository, base, 'residue');
  assert.equal(residue.exit, 1, residue.output);
  assert.ok(residue.output.includes('src/other.txt=1') && residue.output.includes('src/allowed.txt retains 3'), residue.output);
  const pattern = gate(repository, base, '--pattern', '(', 'residue');
  assert.equal(pattern.exit, 1, pattern.output);
  assert.match(pattern.output, /FAIL residue: git grep/);
  const scope = gate(repository, base, '--scope', 'src/missing.txt', 'residue');
  assert.equal(scope.exit, 1, scope.output);
  assert.match(scope.output, /FAIL residue: unknown pathspec --scope src\/missing\.txt/);
  const unsupported = gate(repository, base, 'lists');
  assert.equal(unsupported.exit, 1, unsupported.output);
  assert.match(unsupported.output, /FAIL lists: --repo unsupported/);

  git(repository, 'checkout', '-q', '-b', 'clean', base);
  write(repository, 'src/new.txt', 'fresh\n');
  git(repository, 'add', '-A');
  git(repository, 'commit', '-q', '-m', 'clean');
  for (const name of ['history', 'newhost', 'residue']) {
    const result = gate(repository, base, name);
    assert.equal(result.exit, 0, result.output);
    assert.match(result.output, new RegExp('^PASS ' + name + ' ', 'm'));
  }
});
