'use strict';
// The export guards that keep authoring state out of a package and out of the portable host's resolution. Legacy
// folder names and shared roots are built at runtime, so this file holds neither a legacy folder literal nor a machine
// path.
const test = require('node:test'), assert = require('node:assert/strict');
const fs = require('node:fs'), os = require('node:os'), path = require('node:path');
const {authoringMember, authoringTrace} = require('./export.cjs');

const legacy = digit => ['.', 'phase', digit].join('');

test('a packaged member under any .local folder or a legacy folder is authoring state', () => {
  for (const name of ['.local/runs/asset-pipeline/n/a.json', '.local/temp/x.txt', '.local/locks/check.lock',
    '.local/nuget/packages/p.nupkg', '.local/tools/godot/godot.exe', '.local/downloads/godot/g.zip', '.local',
    'content/.local/runs/a.json', '.LOCAL/temp/x', `${legacy(1)}/runs/a.json`, `${legacy(4)}/checkpoint.json`,
    'override.cfg', 'packages.lock.json', 'production/assets/a.png']) {
    assert.equal(authoringMember(name), true, name);
  }
  for (const name of ['content/pack-index.json', 'docs/README.md', 'assets/common/bow.png.import', '.localx/a.json',
    'content/local/a.json', 'project.binary']) {
    assert.equal(authoringMember(name), false, name);
  }
});

test('a host trace through the resolved shared root is refused whatever the root is named', t => {
  const base = fs.realpathSync.native(fs.mkdtempSync(path.join(os.tmpdir(), 'kz-export-guards-')));
  t.after(() => fs.rmSync(base, {recursive: true, force: true}));
  const override = path.join(base, 'kz-cache'), checkout = path.join(base, 'checkout');
  const line = file => `assembly_paths: '${file}'\nprobe ${file}\n`;
  for (const folder of ['tools', 'downloads', 'nuget']) {
    const file = path.join(override, folder, 'x', 'System.Runtime.dll');
    assert.equal(authoringTrace(line(file), override), true, 'override ' + folder);
    assert.equal(authoringTrace(line(file.toUpperCase()), override), true, 'override ' + folder + ' in another case');
    assert.equal(authoringTrace(line(file.split(path.sep).join('/')), override), true, 'override ' + folder + ' with /');
  }
  const exported = path.join(checkout, '.local', 'runs', 'asset-pipeline', 'n', 'ExportDebug', 'System.Runtime.dll');
  assert.equal(authoringTrace(line(exported), path.join(checkout, '.local')), false,
    'a run folder inside the shared root of a main checkout stays allowed');
  assert.equal(authoringTrace(line(exported), override), false, 'an unrelated override allows the run folder');
  assert.equal(authoringTrace(line(path.join(base, 'kz-cache-other', 'nuget', 'a.dll')), override), false, 'sibling prefix');
  for (const trace of [line(path.join(checkout, '.local', 'tools', 'godot', 'a.dll')),
    line(path.join(checkout, legacy(1), 'packages', 'a.dll')), line(path.join(base, 'sdk', 'a.dll')), 'KindomCrown donor'])
    assert.equal(authoringTrace(trace, override), true, trace);
  fs.mkdirSync(path.join(base, 'real-cache', 'nuget'), {recursive: true});
  let linked = true;
  try { fs.symlinkSync(path.join(base, 'real-cache'), path.join(base, 'linked-cache'), 'junction'); } catch { linked = false; }
  if (linked) {
    assert.equal(authoringTrace(line(path.join(base, 'real-cache', 'nuget', 'a.dll')), path.join(base, 'linked-cache')), true,
      'the real path of a linked shared root');
  }
});
