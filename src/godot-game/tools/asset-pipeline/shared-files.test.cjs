'use strict';
// The narrow shared-root readers: a file reached through a link in any folder below the shared root, or a hard-linked
// file, is refused, as the checkout readers refuse them. The shared root is a temporary KZ_SHARED_LOCAL folder.
const test = require('node:test'), assert = require('node:assert/strict');
const fs = require('node:fs'), os = require('node:os'), path = require('node:path');
const content = require('./content.cjs');

function sharedRoot(t) {
  const base = fs.realpathSync.native(fs.mkdtempSync(path.join(os.tmpdir(), 'kz-shared-files-')));
  const previous = process.env.KZ_SHARED_LOCAL;
  process.env.KZ_SHARED_LOCAL = path.join(base, 'shared');
  t.after(() => {
    if (previous === undefined) delete process.env.KZ_SHARED_LOCAL; else process.env.KZ_SHARED_LOCAL = previous;
    fs.rmSync(base, {recursive: true, force: true});
  });
  return path.join(base, 'shared');
}

test('a shared file below a linked folder of the shared root is refused', t => {
  const shared = sharedRoot(t);
  fs.mkdirSync(path.join(shared, 'elsewhere', 'packages', 'p'), {recursive: true});
  fs.writeFileSync(path.join(shared, 'elsewhere', 'packages', 'p', 'p.nupkg'), 'package');
  fs.mkdirSync(path.join(shared, 'downloads', 'godot'), {recursive: true});
  fs.writeFileSync(path.join(shared, 'downloads', 'godot', 'g.zip'), 'archive');
  assert.equal(content.sharedFile('downloads', 'g.zip'), path.join(shared, 'downloads', 'godot', 'g.zip'));
  try {
    fs.symlinkSync(path.join(shared, 'elsewhere'), path.join(shared, 'nuget'), 'junction');
  } catch {
    t.skip('this OS cannot create a junction or symbolic link');
    return;
  }
  assert.throws(() => content.sharedFile('packages', 'p', 'p.nupkg'), /PATH_REPARSE/);
});

test('a hard-linked shared file is refused and a single-link file is read', t => {
  const shared = sharedRoot(t), folder = path.join(shared, 'downloads', 'godot');
  fs.mkdirSync(folder, {recursive: true});
  fs.writeFileSync(path.join(folder, 'g.zip'), 'archive');
  assert.equal(content.sharedBytes(path.join(folder, 'g.zip'), 1024).toString(), 'archive');
  fs.linkSync(path.join(folder, 'g.zip'), path.join(folder, 'second-name.zip'));
  assert.throws(() => content.sharedBytes(path.join(folder, 'g.zip'), 1024), /FILE_INVALID/);
});
