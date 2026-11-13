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
// JSON.parse accepts repeated keys. Pins must reject them before parsing loses identity.
function parseJson(text) {
  if (Buffer.byteLength(text) > 1_048_576) fail('PINS_CONFLICT', 'Declaration exceeds 1 MiB');
  let offset = 0;
  const whitespace = () => { while (/\s/.test(text[offset] ?? '') && offset < text.length) offset++; };
  const string = () => {
    const start = offset++;
    while (offset < text.length) {
      if (text[offset++] === '"') return JSON.parse(text.slice(start, offset));
      if (text[offset - 1] === '\\') offset++;
    }
    fail('PINS_CONFLICT', 'Unterminated JSON string');
  };
  function value(depth = 0) {
    if (depth > 32) fail('PINS_CONFLICT', 'Declaration nesting exceeds 32');
    whitespace();
    if (text[offset] === '{') {
      offset++; whitespace();
      const seen = new Set();
      if (text[offset] === '}') { offset++; return; }
      for (;;) {
        whitespace();
        if (text[offset] !== '"') fail('PINS_CONFLICT', 'Expected JSON key');
        const key = string();
        if (seen.has(key)) fail('PINS_CONFLICT', `Duplicate JSON declaration ${key}`);
        seen.add(key); whitespace();
        if (text[offset++] !== ':') fail('PINS_CONFLICT', 'Expected colon');
        value(depth + 1); whitespace();
        const separator = text[offset++];
        if (separator === '}') return;
        if (separator !== ',') fail('PINS_CONFLICT', 'Expected comma');
      }
    }
    if (text[offset] === '[') {
      offset++; whitespace();
      if (text[offset] === ']') { offset++; return; }
      for (;;) {
        value(depth + 1); whitespace();
        const separator = text[offset++];
        if (separator === ']') return;
        if (separator !== ',') fail('PINS_CONFLICT', 'Expected array comma');
      }
    }
    if (text[offset] === '"') { string(); return; }
    const primitive = text.slice(offset).match(/^(?:true|false|null|-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?)/);
    if (!primitive) fail('PINS_CONFLICT', 'Invalid JSON value');
    offset += primitive[0].length;
  }
  value(); whitespace();
  if (offset !== text.length) fail('PINS_CONFLICT', 'Trailing JSON content');
  return JSON.parse(text);
}
const readJson = file => parseJson(fs.readFileSync(file, 'utf8'));
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
    TEMP: path.join(home, 'temp'), TMP: path.join(home, 'temp'), TMPDIR: path.join(home, 'temp'),
    DOTNET_CLI_TELEMETRY_OPTOUT: '1', DOTNET_NOLOGO: '1',
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE: '1',
    PHASE1_NONCE: nonce,
    ...extra,
  };
}

async function command(label, executable, args, options = {}) {
  const started = new Date().toISOString();
  const result = await new Promise(resolve => {
    let child;
    try {
      child = spawn(executable, args, {
        cwd: root, env: environment(options.env), windowsHide: true,
        detached: process.platform !== 'win32', shell: false,
      });
    } catch (error) {
      resolve({ executable, args, started, code: null, spawnError: error.message, stdout: '', stderr: '' });
      return;
    }
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
  if (result.spawnError) fail(label.startsWith('engine') ? 'ENGINE_MISSING' : 'TOOL_MISSING', `${label}: ${result.spawnError}`);
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
  const supported = ['engine=4.7.2', 'engineFullVersion=4.7.2.stable.mono.official.ed1daf0bf', 'sdk=10.0.404', 'compiler=5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf)', 'language=14.0', 'framework=net10.0', 'node=24.22.1', 'runtime=10.0.15', 'editorRuntime=8.0.16', 'coverageTool=18.12.0'];
  for (const pair of supported) {
    const [key, value] = pair.split('=');
    if (manifest[key] !== value) fail('PINS_MISMATCH', `unsupported ${key}=${manifest[key]}; expected ${value}`);
  }
  return JSON.stringify(Object.fromEntries(required.sort().map(id => [id, manifest.godotPackages.find(item => item.id === id).version])));
}

function declarations(manifest, inputs) {
  const sdk = parseJson(inputs.global).sdk;
  if (!sdk?.version) fail('PINS_MISSING', 'global.json SDK');
  if (sdk.version !== manifest.sdk || sdk.rollForward !== 'disable' || sdk.allowPrerelease !== false) fail('PINS_MISMATCH', 'global.json exact SDK policy');
  const tool = parseJson(inputs.tools).tools?.['dotnet-coverage'];
  if (!tool?.version) fail('PINS_MISSING', 'dotnet-coverage local tool');
  if (tool.version !== manifest.coverageTool || tool.rollForward !== false) fail('PINS_MISMATCH', 'dotnet-coverage version/rollForward');
  const one = (pattern, description) => {
    const matches = [...inputs.project.matchAll(pattern)];
    if (matches.length === 0) fail('PINS_MISSING', description);
    if (matches.length !== 1) fail('PINS_CONFLICT', description);
    return matches[0][1];
  };
  if (one(/\bSdk="Godot.NET.Sdk\/([^"]+)"/g, 'Godot SDK') !== manifest.engine
      || one(/<TargetFramework>([^<]+)<\/TargetFramework>/g, 'framework') !== manifest.framework
      || one(/<LangVersion>([^<]+)<\/LangVersion>/g, 'language') !== manifest.language) fail('PINS_MISMATCH', 'project engine/framework/language');
  const sources = [...inputs.nuget.matchAll(/<add\s+key="([^"]+)"\s+value="([^"]+)"[^>]*\/>/g)];
  if (new Set(sources.map(row => row[1])).size !== sources.length || new Set(sources.map(row => row[2])).size !== sources.length) fail('PINS_CONFLICT', 'NuGet sources');
  if (sources.length !== 2 || manifest.feeds.some(feed => !sources.some(row => row[2] === feed))) fail('PINS_MISMATCH', 'NuGet official/local feeds');
}

function pinControls(manifest) {
  const inputs = {
    global: fs.readFileSync(path.join(root, 'global.json'), 'utf8'),
    tools: fs.readFileSync(path.join(root, '.config/dotnet-tools.json'), 'utf8'),
    project: fs.readFileSync(path.join(project, 'KingdomZombies.csproj'), 'utf8'),
    nuget: fs.readFileSync(path.join(root, 'NuGet.config'), 'utf8'),
  };
  declarations(manifest, inputs);
  const controls = [];
  const reject = (id, diagnostic, operation) => {
    try { operation(); } catch (error) {
      if (error.diagnostic !== diagnostic) throw error;
      controls.push({ id, diagnostic, status: 'pass' }); return;
    }
    fail('PINS_CONTROL_FAILED', `${id} was accepted`);
  };
  reject('missing-sdk', 'PINS_MISSING', () => validateManifest({ ...manifest, sdk: undefined }));
  reject('duplicate-package', 'PINS_CONFLICT', () => validateManifest({ ...manifest, godotPackages: [manifest.godotPackages[0], manifest.godotPackages[0], ...manifest.godotPackages.slice(2)] }));
  reject('generator-mismatch', 'PINS_MISMATCH', () => validateManifest({ ...manifest, godotPackages: manifest.godotPackages.map(item => item.id === 'Godot.SourceGenerators' ? { ...item, version: '4.7.1' } : item) }));
  reject('compiler-mismatch', 'PINS_MISMATCH', () => validateManifest({ ...manifest, compiler: '5.0.0' }));
  reject('engine-mismatch', 'PINS_MISMATCH', () => validateManifest({ ...manifest, engineFullVersion: '4.7.1.stable' }));
  reject('duplicate-json-same', 'PINS_CONFLICT', () => parseJson('{"sdk":"10.0.404","sdk":"10.0.404"}'));
  reject('duplicate-json-conflict', 'PINS_CONFLICT', () => parseJson('{"sdk":"10.0.404","sdk":"8.0.100"}'));
  reject('missing-global-sdk', 'PINS_MISSING', () => declarations(manifest, { ...inputs, global: '{"sdk":{}}' }));
  reject('conflicting-global-sdk', 'PINS_MISMATCH', () => declarations(manifest, { ...inputs, global: inputs.global.replace('10.0.404', '8.0.100') }));
  reject('duplicate-framework', 'PINS_CONFLICT', () => declarations(manifest, { ...inputs, project: inputs.project.replace('</PropertyGroup>', '<TargetFramework>net10.0</TargetFramework></PropertyGroup>') }));
  const permutation = Object.fromEntries(Object.entries(manifest).reverse());
  permutation.godotPackages = [...manifest.godotPackages].reverse();
  if (validateManifest(permutation) !== validateManifest(manifest)) fail('PINS_CONFLICT', 'Permutation changed selection');
  declarations(permutation, { ...inputs, global: JSON.stringify({ sdk: { allowPrerelease: false, rollForward: 'disable', version: manifest.sdk } }) });
  controls.push({ id: 'equivalent-permutation', status: 'pass' });
  fs.writeFileSync(path.join(runDirectory, 'pins-controls.json'), JSON.stringify({ nonce, controls }, null, 2));
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
  if (!fs.existsSync(cache) || process.env.PHASE1_FORCE_DOWNLOAD === '1') {
    const local = process.env.PHASE1_ARCHIVE ?? path.join(os.homedir(), 'Downloads', archive.file);
    if (process.env.PHASE1_IGNORE_DOWNLOADS !== '1' && fs.existsSync(local)) fs.copyFileSync(local, cache);
    else {
      const temporary = cache + '.' + nonce + '.part';
      await download(archive.url, temporary);
      if (hash(temporary, 'sha512') !== archive.sha512) fail('ARCHIVE_HASH', archive.file);
      fs.renameSync(temporary, cache);
    }
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
  if (archive.executableSha256 && hash(executable) !== archive.executableSha256) fail('PINS_MISMATCH', 'Engine executable payload hash');
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
  return { sdk, dotnet: path.join(path.dirname(sdkRoot), process.platform === 'win32' ? 'dotnet.exe' : 'dotnet'), compiler, compilerSha256: hash(compiler), compilerVersion, engine, engineVersion, node: process.execPath, nodeSha256: hash(process.execPath), platform: process.platform, architecture: process.arch, release: os.release() };
}

async function infrastructureControls(manifest) {
  const controls = [];
  const reject = async (id, diagnostic, operation) => {
    try { await operation(); } catch (error) {
      if (error.diagnostic !== diagnostic) throw error;
      controls.push({ id, diagnostic, status: 'pass' }); return;
    }
    fail('INFRASTRUCTURE_CONTROL_FAILED', `${id} was accepted`);
  };
  await reject('missing-engine', 'ENGINE_MISSING', () => command('engine-missing-control', path.join(runDirectory, 'nonexistent-engine'), []));
  await reject('failed-restore', 'RESTORE_FAILED', () => command('restore-failed-control', 'dotnet', ['restore', path.join(runDirectory, 'nonexistent.csproj')], { diagnostic: 'RESTORE_FAILED' }));
  const sdkList = fs.readFileSync(path.join(runDirectory, 'sdk-list.log'), 'utf8');
  const sdkLine = sdkList.split(/\r?\n/).find(line => line.startsWith(manifest.sdk + ' ['));
  const sdkRoot = sdkLine.slice(sdkLine.indexOf('[') + 1, -1);
  const runtimeConfig = path.join(runDirectory, 'incompatible.runtimeconfig.json');
  fs.writeFileSync(runtimeConfig, JSON.stringify({ runtimeOptions: { tfm: 'net99.0', rollForward: 'Disable', framework: { name: 'Microsoft.NETCore.App', version: '99.0.0' } } }));
  await reject('incompatible-runtime', 'RUNTIME_INCOMPATIBLE', () => command('runtime-incompatible-control', 'dotnet', ['exec', '--runtimeconfig', runtimeConfig, path.join(sdkRoot, manifest.sdk, 'Roslyn/bincore/csc.dll'), '-version'], { diagnostic: 'RUNTIME_INCOMPATIBLE' }));
  await reject('child-timeout', 'CHILD_TIMEOUT', () => command('timeout-control', process.execPath, ['-e', 'setInterval(() => {}, 1000)'], { timeoutMs: 200 }));
  fs.writeFileSync(path.join(runDirectory, 'infrastructure-controls.json'), JSON.stringify({ nonce, controls }, null, 2));
}

async function engineProbe(manifest, engine) {
  const identity = await tools(manifest, engine);
  await infrastructureControls(manifest);
  const config = path.join(runDirectory, 'NuGet.config');
  // The archive supplies the complete four-package closure for this initial scene.
  fs.writeFileSync(config, `<configuration><packageSources><clear/><add key="verified-godot" value="${path.join(home, 'feed')}"/></packageSources></configuration>`);
  const args = ['src/godot-game/KingdomZombies.csproj', '--configfile', config, '--disable-parallel', '-p:NuGetAudit=false'];
  await command('restore', 'dotnet', ['restore', ...args], { diagnostic: 'RESTORE_FAILED' });
  const locked = readJson(path.join(home, 'cache/game.packages.lock.json')).dependencies[manifest.framework];
  const expectedPackages = manifest.godotPackages.filter(item => item.id !== 'Godot.NET.Sdk');
  if (!locked || Object.keys(locked).length !== expectedPackages.length || expectedPackages.some(item => locked[item.id]?.resolved !== item.version)) fail('RESTORE_FAILED', 'Unexpected package closure');
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
    else { pinControls(manifest); await tools(manifest, engine); }
  }
  console.log(JSON.stringify({ nonce, probe: args[1], status: 'pass', evidenceKind: 'raw-compatibility-only', logs: path.relative(root, runDirectory) }));
}

main().catch(error => {
  const result = { nonce, status: 'fail', diagnostic: error.diagnostic ?? 'BOOTSTRAP_FAILED', message: error.message, logs: path.relative(root, runDirectory) };
  if (fs.existsSync(runDirectory)) fs.writeFileSync(path.join(runDirectory, 'failure.json'), JSON.stringify(result, null, 2));
  console.error(JSON.stringify(result));
  process.exitCode = 1;
});
