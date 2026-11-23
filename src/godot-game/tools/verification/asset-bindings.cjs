'use strict';
// Checks and regenerates the asset binding cascade in the working tree, in order: every catalog module hash,
// the delivery fixture catalog, then the pack index. It regenerates the bindings from current bytes; it is not a
// replay of a one-shot binding utility.
//   node src/godot-game/tools/verification/asset-bindings.cjs [--check]
//   node src/godot-game/tools/verification/asset-bindings.cjs --write   (refuses a tree with other uncommitted changes)
const fs = require('node:fs');
const path = require('node:path');
const zlib = require('node:zlib');
const crypto = require('node:crypto');
const {spawnSync} = require('node:child_process');

const ROOT = path.resolve(__dirname, '../../../..');
const PIPELINE = 'src/godot-game/tools/asset-pipeline/';
const CATALOG = PIPELINE + 'required-cases.json';
const CASCADE = [CATALOG, 'src/godot-game/game/content/packs/delivery.json', 'src/godot-game/game/content/pack-index.json'];
const EXCLUDED_ROOTS = ['.claude/', '.codex/', '.agents/', '.planning/'];
const PROJECT_GRAPH = 'src/godot-game/tools/verification/projects.json';
const EXPORT_PROFILE = 'src/godot-game/production/assets/export-profile.json';
const EXPORT_PROFILE_CASE = PIPELINE + 'cases/export-profile.cjs';
const VALID_OUTPUTS = 180;
// The committed payload carries the header OS byte of the Windows zlib build; pinning it makes the derivation identical
// on every platform without changing the compressed data.
const GZIP_OS_BYTE = 10;

class AssetBindingError extends Error {
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

function pipeline(root) {
  return {
    content: require(path.join(root, PIPELINE, 'content.cjs')),
    textData: require(path.join(root, PIPELINE, 'cases/text-data.cjs'))
  };
}

/** Gzip level 9 with the pinned header OS byte, base64 encoded. */
function gzipPayload(bytes, level = 9) {
  const packed = zlib.gzipSync(bytes, {level});
  packed[9] = GZIP_OS_BYTE;
  return packed.toString('base64');
}

/** Collects every object in the catalog that binds a module to its hash. */
function moduleRows(catalog) {
  const rows = [];
  (function walk(value) {
    if (!value || typeof value !== 'object') return;
    if (typeof value.module === 'string' && typeof value.moduleSha256 === 'string') rows.push(value);
    for (const key of Object.keys(value)) walk(value[key]);
  })(catalog);
  return rows;
}

/** Names each module whose binding is outside the pipeline, missing or stale; read returns bytes or null. */
function moduleDiffs(catalog, read) {
  const problems = new Map();
  for (const row of moduleRows(catalog)) {
    if (!row.module.startsWith(PIPELINE)) { problems.set(row.module, 'outside ' + PIPELINE); continue; }
    const bytes = read(row.module);
    if (!bytes) problems.set(row.module, 'missing');
    else if (sha256(bytes) !== row.moduleSha256) problems.set(row.module, 'stale moduleSha256');
  }
  return [...problems].map(([module, reason]) => module + ' ' + reason);
}

function fileReader(root) {
  return file => {
    const target = path.join(root, file);
    return fs.existsSync(target) && fs.statSync(target).isFile() ? fs.readFileSync(target) : null;
  };
}

/** The fixture catalog entry that the delivery pack must carry for the catalog bytes. */
function fixtureCatalogFor(bytes, existing) {
  return {...existing, sha256: sha256(bytes), uncompressedBytes: bytes.length, gzipBase64: gzipPayload(bytes)};
}

function fixtureDiffs(actual, expected) {
  return ['sha256', 'uncompressedBytes', 'gzipBase64'].filter(field => actual[field] !== expected[field])
    .map(field => 'delivery fixtureCatalog ' + field);
}

function readText(file) {
  return fs.readFileSync(file, 'utf8');
}

/** Checks the whole cascade against the working tree and returns the module count and every problem found. */
function check(root = ROOT) {
  const {content, textData} = pipeline(root);
  const problems = [];
  const catalogText = readText(path.join(root, CATALOG)), catalog = JSON.parse(catalogText);
  if (serialize(catalog) !== catalogText) problems.push('catalog format');
  const modules = moduleRows(catalog).length;
  problems.push(...moduleDiffs(catalog, fileReader(root)));
  if (path.resolve(content.inside(root, textData.SUPPORT_CATALOG)) !== path.resolve(root, CATALOG))
    problems.push('support catalog does not resolve to ' + CATALOG);
  const deliveryFile = content.inside(root, textData.DELIVERY), deliveryText = readText(deliveryFile);
  const delivery = JSON.parse(deliveryText);
  if (serialize(delivery) !== deliveryText) problems.push('delivery format');
  const catalogBytes = content.bytes(root, textData.SUPPORT_CATALOG);
  problems.push(...fixtureDiffs(delivery.fixtureCatalog, fixtureCatalogFor(catalogBytes, delivery.fixtureCatalog)));
  for (const guide of delivery.guides || []) {
    const bytes = content.bytes(root, guide.path);
    if (guide.bytes !== bytes.length || guide.sha256 !== sha256(bytes) || guide.text !== bytes.toString('utf8'))
      problems.push('delivery guide ' + guide.path);
  }
  const indexFile = content.inside(root, content.INDEX);
  if (serialize(content.normalizeCurrent(root)) !== readText(indexFile)) problems.push('pack-index differs from normalizeCurrent');
  const result = content.validate(root);
  const valid = Array.isArray(result.valid) ? result.valid.length : null;
  if (valid !== VALID_OUTPUTS) problems.push('content.validate ' + valid + ' expected ' + VALID_OUTPUTS);
  try { textData.validateSupport(); } catch (error) { problems.push('validateSupport ' + (error.diagnostic || error.message)); }
  const graph = sha256(fs.readFileSync(path.join(root, PROJECT_GRAPH)));
  const profile = JSON.parse(readText(path.join(root, EXPORT_PROFILE)));
  if (profile.sourceContract.projectGraphSha256 !== graph || !readText(path.join(root, EXPORT_PROFILE_CASE)).includes(graph))
    problems.push('projectGraphSha256');
  return {modules, valid, problems};
}

/** Returns the status lines that make --write unsafe: anything but the three cascade files and the excluded roots. */
function dirtyEntries(lines) {
  return lines.filter(line => line.trim()).filter(line => {
    const files = line.slice(3).split(' -> ');
    return !files.every(file => CASCADE.includes(file) || EXCLUDED_ROOTS.some(prefix => file.startsWith(prefix)));
  });
}

/** Refuses a dirty tree, as ac08-bindings.cjs does, so --write rebinds only committed module bytes. */
function refuseDirty(lines) {
  const dirty = dirtyEntries(lines);
  if (dirty.length) {
    throw new AssetBindingError('ASSET_DIRTY_TREE', 'commit or discard these changes first: ' + dirty.slice(0, 8).join(' | '));
  }
}

function status(root) {
  const result = spawnSync('git', ['status', '--porcelain=v1', '--untracked-files=all'],
    {cwd: root, encoding: 'utf8', windowsHide: true});
  if (result.error || result.status !== 0) {
    throw new AssetBindingError('ASSET_GIT', 'git status failed: ' + (result.error ? result.error.message : result.stderr));
  }
  return result.stdout.split('\n');
}

/**
 * Rewrites stale module hashes, then the delivery fixture catalog, then the pack index, and checks the result. Commit
 * the module edits first: a tree with any change but the three cascade files is refused.
 */
function write(root = ROOT) {
  refuseDirty(status(root));
  const {content, textData} = pipeline(root);
  const read = fileReader(root), written = [];
  const catalog = JSON.parse(readText(path.join(root, CATALOG)));
  for (const row of moduleRows(catalog)) {
    if (!row.module.startsWith(PIPELINE)) throw new AssetBindingError('ASSET_MODULE', row.module + ' is outside ' + PIPELINE);
    const bytes = read(row.module);
    if (!bytes) throw new AssetBindingError('ASSET_MODULE', row.module + ' is missing');
    row.moduleSha256 = sha256(bytes);
  }
  const replace = (file, text) => {
    if (readText(file) !== text) { fs.writeFileSync(file, text); written.push(path.relative(root, file).replaceAll('\\', '/')); }
  };
  replace(path.join(root, CATALOG), serialize(catalog));
  const deliveryFile = content.inside(root, textData.DELIVERY), delivery = JSON.parse(readText(deliveryFile));
  delivery.fixtureCatalog = fixtureCatalogFor(content.bytes(root, textData.SUPPORT_CATALOG), delivery.fixtureCatalog);
  replace(deliveryFile, serialize(delivery));
  replace(content.inside(root, content.INDEX), serialize(content.normalizeCurrent(root)));
  return {written, ...check(root)};
}

function main(argv) {
  const mode = argv.length === 0 || (argv.length === 1 && argv[0] === '--check') ? 'check'
    : argv.length === 1 && argv[0] === '--write' ? 'write' : null;
  if (!mode) throw new AssetBindingError('USAGE', 'node asset-bindings.cjs [--check | --write]');
  const result = mode === 'write' ? write(ROOT) : check(ROOT);
  if (mode === 'write') console.log('WROTE ' + (result.written.join(' ') || 'nothing'));
  if (result.problems.length) {
    console.error('FAIL asset-bindings: ' + result.problems.length + ' problem(s): ' + result.problems.slice(0, 12).join(' | '));
    return 1;
  }
  console.log('PASS asset-bindings modules=' + result.modules + ' valid=' + result.valid);
  return 0;
}

module.exports = {
  CATALOG, CASCADE, PIPELINE, VALID_OUTPUTS, AssetBindingError, dirtyEntries, refuseDirty, sha256, serialize, gzipPayload, moduleRows, moduleDiffs,
  fixtureCatalogFor, fixtureDiffs, check, write
};

if (require.main === module) {
  try {
    process.exitCode = main(process.argv.slice(2));
  } catch (error) {
    console.error('FAIL asset-bindings: ' + (error instanceof AssetBindingError ? error.message : error.stack));
    process.exitCode = 1;
  }
}
