'use strict';

// One pinned interpreter runs every Phase 4 Python script. The pin covers the executable, its runtime library, the
// reported version and the site libraries the scripts import. UnityPy is loaded from the donor resource folder and
// each script asserts its version separately.
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {spawnSync} = require('node:child_process');

const PIN_FILE = path.resolve(__dirname, 'python.json');
const PIN = JSON.parse(fs.readFileSync(PIN_FILE, 'utf8'));
const PYTHON = (process.env.KZ_PHASE4_PYTHON || PIN.executable).replaceAll('\\', '/');
const PROBE = 'import sys,json,importlib.metadata as m; names=json.loads(sys.argv[1]); ' +
  "print(json.dumps({'version':sys.version.split()[0],'libraries':{n:m.version(n) for n in names}}))";

const sha256 = file => crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex');
let probed = null;

function fail(detail) {
  throw new Error('PYTHON_PIN ' + detail);
}

// Verifies the interpreter before each use. The version and library probe runs once per process.
function verifyPython(executable = PYTHON) {
  if (!fs.existsSync(executable) || !fs.statSync(executable).isFile()) fail('missing ' + executable);
  if (sha256(executable) !== PIN.executableSha256) fail('executable ' + executable);
  const library = path.join(path.dirname(executable), PIN.runtimeLibrary);
  if (!fs.existsSync(library) || sha256(library) !== PIN.runtimeLibrarySha256) fail('runtime library ' + library);
  if (!probed) {
    const result = spawnSync(executable, ['-I', '-B', '-c', PROBE, JSON.stringify(Object.keys(PIN.libraries))],
      {encoding:'utf8', windowsHide:true, timeout:30000});
    if (result.error || result.status !== 0) fail('probe ' + (result.error?.message ?? String(result.stderr).slice(0, 500)));
    const reported = JSON.parse(result.stdout);
    if (reported.version !== PIN.version || JSON.stringify(reported.libraries) !== JSON.stringify(PIN.libraries))
      fail('reported ' + result.stdout.trim());
    probed = reported;
  }
  return {executableSha256:PIN.executableSha256, runtimeLibrarySha256:PIN.runtimeLibrarySha256, version:PIN.version,
    libraries:PIN.libraries};
}

function isPython(executable) {
  return typeof executable === 'string' && path.resolve(executable).toLowerCase() === path.resolve(PYTHON).toLowerCase();
}

module.exports = {PYTHON, PIN, verifyPython, isPython};
