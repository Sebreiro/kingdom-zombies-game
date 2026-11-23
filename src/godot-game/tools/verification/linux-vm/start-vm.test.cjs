'use strict';
// The VM launcher's QEMU arguments, built without starting a server or a VM. Requiring the launcher runs nothing.
const test = require('node:test'), assert = require('node:assert/strict');
const os = require('node:os'), path = require('node:path');
const {driveFile, qemuArguments} = require('./start-vm.cjs');

// QEMU's option grammar: a single comma separates options, a doubled comma is a literal comma.
function options(text) {
  const parts = [''];
  for (let index = 0; index < text.length; index++) {
    if (text[index] === ',' && text[index + 1] === ',') { parts[parts.length - 1] += ','; index++; }
    else if (text[index] === ',') parts.push('');
    else parts[parts.length - 1] += text[index];
  }
  return parts;
}

test('every -drive file= keeps a shared root path with commas as one option', () => {
  const root = path.join(os.tmpdir(), 'kz,vm,root', 'tools', 'linux-vm');
  assert.equal(driveFile('a,b'), 'file=a,,b');
  const args = qemuArguments(root, 'whpx', 'max');
  const drives = args.flatMap((value, index) => args[index - 1] === '-drive' ? [options(value)] : []);
  assert.deepEqual(drives.map(parts => parts.find(part => part.startsWith('file=')).slice(5)),
    [path.join(root, 'qemu', 'share', 'edk2-x86_64-code.fd'), path.join(root, 'vars.fd'), path.join(root, 'system.qcow2')]);
  assert.deepEqual(drives.map(parts => parts.map(part => part.split('=')[0]).sort()), [
    ['file', 'format', 'if', 'readonly', 'unit'], ['file', 'format', 'if', 'unit'], ['file', 'format', 'if']]);
  assert.equal(qemuArguments(root, 'tcg', 'max')[3], 'tcg,thread=multi');
});
