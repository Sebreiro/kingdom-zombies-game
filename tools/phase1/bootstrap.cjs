'use strict';

// Development bootstrap only. These raw logs are not Phase 1 acceptance reports.
const fs = require('node:fs');
const path = require('node:path');
const os = require('node:os');
const https = require('node:https');
const crypto = require('node:crypto');
const { spawn } = require('node:child_process');
const root = path.resolve(__dirname, '../..');
const home = path.join(root, '.phase1');
const project = path.join(root, 'src/godot-game');
const nonce = crypto.randomUUID();
const runDirectory = path.join(home, 'runs', nonce);
const manifestPath = path.join(__dirname, 'toolchain.json');
const timeoutMs = 60_000;
const fail = (id, message) => { const error = new Error(message); error.diagnostic = id; throw error; };
const hash = (file, algorithm = 'sha256') => crypto.createHash(algorithm).update(fs.readFileSync(file)).digest('hex');
const readJson = file => JSON.parse(fs.readFileSync(file, 'utf8'));
const resolveLocal = relative => {
  const target = path.resolve(root, relative);
  if (!target.startsWith(root + path.sep)) fail('PATH_OUTSIDE_WORKSPACE', relative);
  return target;
};

function environment(extra = {}) {
  return {
    ...process.env,
    DOTNET_CLI_HOME: path.join(home, 'cli-home'),
    NUGET_PACKAGES: path.join(home, 'packages'),
    NUGET_HTTP_CACHE_PATH: path.join(home, 'http-cache'),
    TEMP: path.join(home, 'temp'), TMP: path.join(home, 'temp'),
    DOTNET_CLI_TELEMETRY_OPTOUT: '1', DOTNET_NOLOGO: '1',
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE: '1',
    PHASE1_NONCE: nonce,
    ...extra,
  };
}

async function command(label, executable, args, options = {}) {
  const started = new Date().toISOString();
  const result = await new Promise(resolve => {
    const child = spawn(executable, args, {
      cwd: root, env: environment(options.env), windowsHide: true,
      detached: process.platform !== 'win32', shell: false,
    });
    let output = '', error = '', timedOut = false, spawnError;
    child.stdout.on('data', chunk => { output += chunk; });
    child.stderr.on('data', chunk => { error += chunk; });
    child.on('error', value => { spawnError = value.message; });
    const timer = setTimeout(() => {
      timedOut = true;
      if (process.platform === 'win32') {
        const killer = spawn('taskkill.exe', ['/pid', String(child.pid), '/T', '/F'], { windowsHide: true });
        killer.on('error', () => child.kill());
      } else {
        try { process.kill(-child.pid, 'SIGKILL'); } catch { child.kill('SIGKILL'); }
      }
    }, options.timeoutMs ?? timeoutMs);
    child.on('close', (code, signal) => {
      clearTimeout(timer);
      resolve({ executable, args, started, code, signal, timedOut, spawnError, stdout: output, stderr: error });
    });
  });
  fs.writeFileSync(path.join(runDirectory, label + '.json'), JSON.stringify(result, null, 2));
  fs.writeFileSync(path.join(runDirectory, label + '.log'), result.stdout + result.stderr);
  if (result.timedOut) fail('CHILD_TIMEOUT', `${label}: exceeded ${options.timeoutMs ?? timeoutMs}ms; descendants terminated`);
  if (result.spawnError) fail(label === 'engine' ? 'ENGINE_MISSING' : 'TOOL_MISSING', `${label}: ${result.spawnError}`);
  if (result.code !== 0) fail(options.diagnostic ?? 'CHILD_FAILED', `${label}: exit ${result.code}; see ${runDirectory}`);
  return result.stdout.trim();
}

function validateManifest(manifest) {
  for (const key of ['engine', 'engineFullVersion', 'sdk', 'compiler', 'language', 'framework', 'node', 'runtime', 'editorRuntime', 'coverageTool']) {
    if (typeof manifest[key] !== 'string' || manifest[key].length === 0) fail('PINS_MISSING', key);
  }
  if (!Array.isArray(manifest.godotPackages) || manifest.godotPackages.length !== 4) fail('PINS_MISSING', 'four Godot packages required');
  const seen = new Set();
  for (const item of manifest.godotPackages) {
    if (seen.has(item.id)) fail('PINS_CONFLICT', `duplicate package ${item.id}`);
    seen.add(item.id);
    if (item.version !== manifest.engine) fail('PINS_MISMATCH', `${item.id} differs from engine`);
    if (!/^[a-f0-9]{64}$/.test(item.sha256)) fail('PINS_MISSING', `${item.id} SHA-256`);
  }
  const required = ['Godot.NET.Sdk', 'Godot.SourceGenerators', 'GodotSharp', 'GodotSharpEditor'];
  if (required.some(id => !seen.has(id))) fail('PINS_MISSING', 'matching SDK/bindings/generator');
  const supported = ['engine=4.7.2', 'sdk=10.0.404', 'language=14.0', 'framework=net10.0', 'node=24.22.1', 'runtime=10.0.15', 'editorRuntime=8.0.16'];
  for (const pair of supported) {
    const [key, value] = pair.split('=');
    if (manifest[key] !== value) fail('PINS_MISMATCH', `unsupported ${key}=${manifest[key]}; expected ${value}`);
  }
  return JSON.stringify(Object.fromEntries(required.sort().map(id => [id, manifest.godotPackages.find(item => item.id === id).version])));
}

// Node HTTPS uses a direct route, independent of inherited shell proxy variables.
function download(url, destination, redirects = 0) {
  if (!url.startsWith('https://') || redirects > 5) fail('ACQUIRE_SOURCE', url);
  return new Promise((resolve, reject) => {
    const request = https.get(url, { timeout: timeoutMs }, response => {
      if ([301, 302, 303, 307, 308].includes(response.statusCode)) {
        response.resume();
        download(new URL(response.headers.location, url).href, destination, redirects + 1).then(resolve, reject);
        return;
      }
      if (response.statusCode !== 200) { response.resume(); reject(new Error(`HTTP ${response.statusCode}: ${url}`)); return; }
      const output = fs.createWriteStream(destination);
      const timer = setTimeout(() => { request.destroy(new Error('Download deadline exceeded')); output.destroy(); }, timeoutMs);
      response.pipe(output);
      response.on('error', reject);
      output.on('error', reject);
      output.on('finish', () => { clearTimeout(timer); output.close(resolve); });
    });
    request.on('timeout', () => request.destroy(new Error('Connection timeout')));
    request.on('error', reject);
  });
}

async function acquire(manifest) {
  const archive = manifest.archives[`${process.platform}-${process.arch}`];
  if (!archive) fail('PLATFORM_UNAVAILABLE', `${process.platform}-${process.arch}`);
  const cache = path.join(home, 'cache', archive.file);
  if (!fs.existsSync(cache)) {
    const local = process.env.PHASE1_ARCHIVE ?? path.join(os.homedir(), 'Downloads', archive.file);
    if (process.env.PHASE1_IGNORE_DOWNLOADS !== '1' && fs.existsSync(local)) fs.copyFileSync(local, cache);
    else await download(archive.url, cache);
  }
  if (hash(cache, 'sha512') !== archive.sha512 || (archive.sha256 && hash(cache) !== archive.sha256)) fail('ARCHIVE_HASH', archive.file);
  const checksums = path.join(home, 'cache', 'SHA512-SUMS.txt');
  if (!fs.existsSync(checksums)) await download(manifest.releaseChecksums, checksums);
  const entries = fs.readFileSync(checksums, 'utf8').split(/\r?\n/).map(line => line.trim().split(/\s+/));
  if (entries.filter(row => row[1] === archive.file && row[0] === archive.sha512).length !== 1) fail('ARCHIVE_CHECKSUM_SOURCE', archive.file);
  const engineDirectory = resolveLocal('.phase1/tools/' + archive.directory);
  const executable = path.join(engineDirectory, archive.executable);
  if (!fs.existsSync(executable)) {
    if (process.platform === 'win32') {
      await command('extract', 'powershell.exe', ['-NoProfile', '-NonInteractive', '-Command', 'Expand-Archive -LiteralPath $env:PHASE1_ZIP -DestinationPath $env:PHASE1_TOOLS -Force'],
        { env: { PHASE1_ZIP: cache, PHASE1_TOOLS: path.join(home, 'tools') }, diagnostic: 'ARCHIVE_EXTRACT' });
    } else {
      await command('extract', 'unzip', ['-o', cache, '-d', path.join(home, 'tools')], { diagnostic: 'ARCHIVE_EXTRACT' });
      fs.chmodSync(executable, 0o755);
    }
  }
  const source = path.join(engineDirectory, 'GodotSharp/Tools/nupkgs');
  for (const pkg of manifest.godotPackages) {
    const file = `${pkg.id}.${pkg.version}.nupkg`;
    const input = path.join(source, file);
    if (!fs.existsSync(input) || hash(input) !== pkg.sha256) fail('PACKAGE_HASH', file);
    fs.copyFileSync(input, path.join(home, 'feed', file));
  }
  return executable;
}

async function tools(manifest, engine) {
  if (process.versions.node !== manifest.node) fail('PINS_MISMATCH', `Node ${process.versions.node}; expected ${manifest.node}`);
  const sdk = await command('sdk-version', 'dotnet', ['--version']);
  if (sdk !== manifest.sdk) fail('PINS_MISMATCH', `SDK ${sdk}; expected ${manifest.sdk}`);
  const runtimes = await command('runtimes', 'dotnet', ['--list-runtimes']);
  for (const version of [manifest.runtime, manifest.editorRuntime]) {
    if (!runtimes.includes(`Microsoft.NETCore.App ${version} [`)) fail('RUNTIME_INCOMPATIBLE', `Missing Microsoft.NETCore.App ${version}`);
  }
  const sdkList = await command('sdk-list', 'dotnet', ['--list-sdks']);
  const sdkLine = sdkList.split(/\r?\n/).find(line => line.startsWith(manifest.sdk + ' ['));
  if (!sdkLine) fail('PINS_MISMATCH', 'Selected SDK missing');
  const sdkRoot = sdkLine.slice(sdkLine.indexOf('[') + 1, -1);
  const compiler = path.join(sdkRoot, manifest.sdk, 'Roslyn/bincore/csc.dll');
  const compilerVersion = await command('compiler-version', 'dotnet', [compiler, '-version']);
  if (compilerVersion !== manifest.compiler) fail('PINS_MISMATCH', `Compiler ${compilerVersion}; expected ${manifest.compiler}`);
  const engineVersion = await command('engine-version', engine, ['--version']);
  if (engineVersion !== manifest.engineFullVersion) fail('PINS_MISMATCH', `Engine ${engineVersion}; expected ${manifest.engineFullVersion}`);
  return { sdk, compiler, compilerVersion, engine, engineVersion, node: process.execPath, platform: process.platform, architecture: process.arch, release: os.release() };
}

async function engineProbe(manifest, engine) {
  const identity = await tools(manifest, engine);
  const config = path.join(runDirectory, 'NuGet.config');
  // The archive supplies the complete four-package closure for this initial scene.
  fs.writeFileSync(config, `<configuration><packageSources><clear/><add key="verified-godot" value="${path.join(home, 'feed')}"/></packageSources></configuration>`);
  const args = ['src/godot-game/KingdomZombies.csproj', '--configfile', config, '--disable-parallel', '-p:NuGetAudit=false'];
  await command('restore', 'dotnet', ['restore', ...args], { diagnostic: 'RESTORE_FAILED' });
  await command('restore-locked', 'dotnet', ['restore', ...args, '--locked-mode'], { diagnostic: 'RESTORE_FAILED' });
  await command('build', 'dotnet', ['build', 'src/godot-game/KingdomZombies.csproj', '--no-restore', '--configuration', 'Debug', '-p:NuGetAudit=false'], { diagnostic: 'COMPILER_FAILED' });
  const output = await command('engine', engine, ['--headless', '--path', project, '--rendering-method', 'gl_compatibility', '--scene', 'res://proofs/Phase1.tscn'], { diagnostic: 'ENGINE_FAILED' });
  const assertions = output.split(/\r?\n/).filter(line => line.startsWith('PHASE1_ASSERT ')).map(line => JSON.parse(line.slice(14)));
  const expected = ['construction', 'ready', 'exit'];
  if (assertions.length !== expected.length || assertions.some((value, i) => value.nonce !== nonce || value.assertion !== expected[i])) fail('ENGINE_EVIDENCE', 'Expected exactly one fresh construction/ready/exit sequence');
  const identities = output.split(/\r?\n/).filter(line => line.startsWith('PHASE1_IDENTITY ')).map(line => JSON.parse(line.slice(16)));
  if (identities.length !== 1 || identities[0].nonce !== nonce || identities[0].runtime !== manifest.runtime) fail('RUNTIME_INCOMPATIBLE', 'Engine runtime identity missing or incompatible');
  fs.writeFileSync(path.join(runDirectory, 'compatibility.json'), JSON.stringify({ nonce, kind: 'raw-compatibility-only', identity, engineIdentity: identities[0], assertions, inputSha256: hash(manifestPath), executableSha256: hash(engine), scriptSha256: hash(path.join(project, 'proofs/Phase1.cs')) }, null, 2));
  return assertions;
}

async function boundariesProbe() {
  for (const file of ['src/core/KingdomZombies.Core.csproj', 'src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj']) {
    if (!fs.existsSync(resolveLocal(file))) fail('BOUNDARIES_MISSING', file);
    await command(path.basename(file) + '-build', 'dotnet', ['build', file], { diagnostic: 'BOUNDARIES_BUILD' });
  }
}

async function main() {
  for (const directory of ['tools', 'cache', 'cli-home', 'packages', 'http-cache', 'temp', 'feed']) fs.mkdirSync(path.join(home, directory), { recursive: true });
  fs.mkdirSync(runDirectory, { recursive: true });
  const args = process.argv.slice(2);
  if (args.length !== 2 || args[0] !== '--probe' || !['engine', 'pins', 'boundaries'].includes(args[1])) fail('USAGE', 'node tools/phase1/bootstrap.cjs --probe engine|pins|boundaries');
  const manifest = readJson(manifestPath);
  validateManifest(manifest);
  if (args[1] === 'boundaries') await boundariesProbe();
  else {
    const engine = await acquire(manifest);
    if (args[1] === 'engine') await engineProbe(manifest, engine);
    else await tools(manifest, engine);
  }
  console.log(JSON.stringify({ nonce, probe: args[1], status: 'pass', evidenceKind: 'raw-compatibility-only', logs: path.relative(root, runDirectory) }));
}

main().catch(error => {
  const result = { nonce, status: 'fail', diagnostic: error.diagnostic ?? 'BOOTSTRAP_FAILED', message: error.message, logs: path.relative(root, runDirectory) };
  if (fs.existsSync(runDirectory)) fs.writeFileSync(path.join(runDirectory, 'failure.json'), JSON.stringify(result, null, 2));
  console.error(JSON.stringify(result));
  process.exitCode = 1;
});
