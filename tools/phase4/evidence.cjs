'use strict';

const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {spawnSync} = require('node:child_process');
const {isDeepStrictEqual:equal, TextDecoder} = require('node:util');
const {inside, parse, hash} = require('./content.cjs');

const PHASE = '.planning/phases/04-asset-production/';
const REVIEW = PHASE + '04-REVIEW.json';
const SELECTION = PHASE + '04-SELECTION.json';
const FIXES = PHASE + '04-REVIEW-FIXES.json';
const TARGETS = 'tools/phase4/reviewed-source.targets';
const IDENTITY = ['revision', 'source', 'media', 'tool', 'build'];
const HEX = /^[a-f0-9]{64}$/;
const REVISION = /^[a-f0-9]{40}$/;
const UUID = /^[a-f0-9]{8}-[a-f0-9]{4}-4[a-f0-9]{3}-[89ab][a-f0-9]{3}-[a-f0-9]{12}$/;
const MAX_BYTES = 256 * 1024 * 1024;
const SOURCE_ROOTS = ['src', 'tools/phase1', 'tools/phase2', 'tools/phase3', 'tools/phase4',
  'tools/code-rules', 'tests', 'docs', 'specs', 'production', 'AGENTS.md', 'KingdomZombies.slnx',
  'global.json', 'NuGet.config', 'Directory.Build.props', 'Directory.Build.targets', 'Directory.Xunit.targets',
  'Directory.Packages.props', 'BannedSymbols.txt', 'BannedSymbols.Pure.txt', 'BannedSymbols.Tests.txt',
  '.editorconfig', '.gitattributes', PHASE + '04-SCENARIOS.md'];

function need(ok, code, detail = '') {
  if (!ok) throw new Error(code + (detail ? ' ' + detail : ''));
}

function named(value) {
  return typeof value === 'string' && value.trim() === value && value.length > 0 && value.length <= 4096 &&
    value.normalize('NFC') === value && !/[\p{Cc}\p{Cs}]/u.test(value);
}

function instant(value) {
  return typeof value === 'string' && /^\d{4}-\d\d-\d\dT\d\d:\d\d:\d\d\.\d{3}Z$/.test(value) &&
    Number.isFinite(Date.parse(value)) && new Date(value).toISOString() === value;
}

function exact(expected, actual, label, ordered = false) {
  need(Array.isArray(expected) && Array.isArray(actual) && expected.length === actual.length &&
    new Set(expected).size === expected.length && new Set(actual).size === actual.length &&
    (ordered ? equal(expected, actual) : expected.every(value => actual.includes(value))), 'EVIDENCE_SET', label);
}

function safeFile(root, relative, limit = MAX_BYTES) {
  const file = inside(root, relative);
  need(fs.existsSync(file), 'EVIDENCE_PATH_MISSING', relative);
  const stat = fs.lstatSync(file);
  need(stat.isFile() && !stat.isSymbolicLink() && stat.nlink === 1 && stat.size <= limit, 'EVIDENCE_FILE', relative);
  return {file, stat};
}

function read(root, relative, limit = 8 * 1024 * 1024) {
  const {file, stat} = safeFile(root, relative, limit);
  const data = fs.readFileSync(file);
  need(data.length === stat.size, 'EVIDENCE_CHANGED', relative);
  return {path:relative, bytes:data.length, sha256:hash(data), data};
}

function reference(root, relative, limit) {
  const {data, ...ref} = read(root, relative, limit);
  return ref;
}

function artifact(root, ref, limit = MAX_BYTES) {
  need(ref && HEX.test(ref.sha256) && Number.isSafeInteger(ref.bytes) && ref.bytes >= 0, 'EVIDENCE_REF');
  const record = read(root, ref.path, limit);
  need(record.bytes === ref.bytes && record.sha256 === ref.sha256, 'EVIDENCE_HASH', ref.path);
  return record;
}

function document(root, value) {
  const row = typeof value === 'string' ? read(root, value) : artifact(root, value, 8 * 1024 * 1024);
  return parse(row.data, row.path);
}

function writeExclusive(root, relative, value) {
  const file = inside(root, relative);
  fs.mkdirSync(path.dirname(file), {recursive:true});
  fs.writeFileSync(inside(root, relative), Buffer.isBuffer(value) ? value : JSON.stringify(value, null, 2) + '\n', {flag:'wx'});
  return reference(root, relative);
}

function git(root, args, encoding = 'utf8') {
  const result = spawnSync('git', args, {cwd:root, encoding, windowsHide:true, timeout:15000, maxBuffer:16 * 1024 * 1024});
  need(!result.error && result.status === 0 && !result.signal, 'EVIDENCE_GIT', args[0] + ' ' +
    (result.error?.message ?? String(result.stderr).trim()));
  return result.stdout;
}

function committedRef(root, revision, ref) {
  need(REVISION.test(revision), 'EVIDENCE_REVISION');
  const working = artifact(root, ref);
  const committed = git(root, ['show', revision + ':' + working.path], null);
  need(committed.length === working.bytes && hash(committed) === working.sha256, 'EVIDENCE_COMMIT_FILE', working.path);
}

function identifySource(root, revision = git(root, ['rev-parse', 'HEAD']).trim()) {
  need(REVISION.test(revision), 'EVIDENCE_REVISION');
  const list = git(root, ['ls-tree', '-r', '-z', revision, '--', ...SOURCE_ROOTS]).split('\0').filter(Boolean);
  const committed = new Map(list.map(line => {
    const match = /^(100644|100755) blob ([a-f0-9]{40})\t(.+)$/u.exec(line);
    need(match, 'EVIDENCE_SOURCE_MODE', line);
    return [match[3], match[2]];
  }).filter(([file]) => file !== TARGETS));
  const working = git(root, ['ls-files', '-z', '--cached', '--others', '--exclude-standard', '--', ...SOURCE_ROOTS])
    .split('\0').filter(file => file && file !== TARGETS);
  need(working.length > 0 && working.length <= 10000, 'EVIDENCE_SOURCE_COUNT');
  exact([...committed.keys()], working, 'tracked source closure');
  const files = working.sort().map(file => {
    const record = read(root, file, MAX_BYTES);
    const oid = crypto.createHash('sha1').update('blob ' + record.bytes + '\0').update(record.data).digest('hex');
    need(oid === committed.get(file), 'EVIDENCE_SOURCE_DRIFT', file);
    const {data, ...ref} = record;
    return ref;
  });
  const media = files.filter(row => /^(src\/godot-game\/(assets|content)\/|production\/phase4\/)/.test(row.path));
  need(media.length > 0, 'EVIDENCE_MEDIA_EMPTY');
  return {revision, source:hash(JSON.stringify(files)), media:hash(JSON.stringify(media)), files};
}

function identifyTool(root) {
  const platform = process.platform + '-' + process.arch;
  const pinFile = 'tools/phase1/toolchain.json';
  const pins = document(root, pinFile);
  need(platform === 'win32-x64' && process.version === 'v' + pins.node, 'EVIDENCE_ENVIRONMENT');
  const archive = pins.archives[platform];
  const engine = reference(root, '.phase1/tools/' + archive.directory + '/' + archive.executable, MAX_BYTES);
  need(engine.sha256 === archive.executableSha256, 'EVIDENCE_ENGINE');
  const sdk = spawnSync('dotnet', ['--version'], {cwd:root, encoding:'utf8', windowsHide:true, timeout:10000});
  need(!sdk.error && sdk.status === 0 && sdk.stdout.trim() === pins.sdk, 'EVIDENCE_SDK');
  const node = hash(fs.readFileSync(process.execPath));
  const tool = {platform, node, nodeVersion:process.version, godot:engine, sdk:pins.sdk, pins:reference(root, pinFile)};
  return {tool:hash(JSON.stringify(tool)), tools:tool};
}

function identity(expected, actual, label) {
  need(expected && actual && REVISION.test(expected.revision) && IDENTITY.slice(1).every(key => HEX.test(expected[key])) &&
    IDENTITY.every(key => actual[key] === expected[key]), 'EVIDENCE_IDENTITY', label);
}

function textArtifact(root, ref) {
  const record = artifact(root, ref, 2 * 1024 * 1024);
  const text = new TextDecoder('utf-8', {fatal:true}).decode(record.data);
  need(named(text.trim().split(/\r?\n/)[0]), 'EVIDENCE_TEXT', ref.path);
  return text;
}

function validateReview(review, fixes, current, declarations) {
  need(review?.schemaVersion === 1 && review.status === 'complete' && review.scope === 'phase-04-whole-source' &&
    review.independent === true && named(review.reviewedBy) && named(review.implementedBy) &&
    review.reviewedBy.toLowerCase() !== review.implementedBy.toLowerCase() && instant(review.reviewedAt), 'REVIEW_SCHEMA');
  identity(current, review, 'review');
  need(declarations.report.includes(review.reviewedBy) && declarations.report.includes(review.revision) &&
    declarations.report.includes(review.source), 'REVIEW_REPORT_ATTESTATION');
  need(fixes?.schemaVersion === 1 && fixes.reviewReport === review.report.sha256, 'REVIEW_FIX_BINDING');
  need(Array.isArray(review.findings) && Array.isArray(fixes.findings), 'REVIEW_FINDINGS');
  exact(review.findings.map(row => row.id), fixes.findings.map(row => row.id), 'review dispositions');
  for (const finding of review.findings) {
    const disposition = fixes.findings.find(row => row.id === finding.id);
    const mandatory = ['critical', 'high'].includes(finding.severity);
    need(named(finding.id) && named(finding.summary) && ['critical','high','medium','low','info'].includes(finding.severity) &&
      named(disposition.reason) && (disposition.status === 'resolved' && disposition.proof ||
        !mandatory && disposition.status === 'accepted'), 'REVIEW_FINDINGS_UNRESOLVED', finding.id);
    if (disposition.status === 'resolved') declarations.verifyProof(disposition.proof);
  }
}

function validateProcess(row) {
  need(row && named(row.role) && Number.isSafeInteger(row.pid) && row.pid > 0 && instant(row.startedAt) &&
    instant(row.finishedAt) && row.finishedAt > row.startedAt && row.timedOut === false && row.crashed === false &&
    row.exitCode === 0 && row.signal === null && row.streamsClosed === true && row.inputReleased === true &&
    Array.isArray(row.remainingPids) && row.remainingPids.length === 0, 'EVIDENCE_PROCESS', row?.role);
}

function validateRenderedOutcome(root, row, report) {
  need(Array.isArray(row.checkpoints) && row.checkpoints.length > 0 && Array.isArray(row.processes) &&
    row.processes.length > 0, 'EVIDENCE_RENDERED_MISSING', row.id);
  exact(row.checkpoints.map(item => item.id), row.checkpoints.map(item => item.id), 'checkpoint IDs');
  const processes = row.processes.map(ref => document(root, ref));
  for (const process of processes) validateProcess(process);
  exact(processes.map(process => process.pid), processes.map(process => process.pid), 'rendered process IDs');
  for (const item of row.checkpoints) {
    const draw = document(root, item.draw);
    const input = document(root, item.input);
    need(named(item.id) && draw.caseId === row.id && draw.checkpoint === item.id && draw.invocation === report.invocation &&
      draw.drawn === true && Number.isSafeInteger(draw.frame) && draw.frame > 0 && instant(draw.at) && draw.at >= report.startedAt &&
      draw.at <= report.finishedAt && named(draw.observation) && processes.some(process => process.pid === draw.pid), 'EVIDENCE_NATIVE_DRAW', row.id);
    need(input.caseId === row.id && input.checkpoint === item.id && input.invocation === report.invocation &&
      input.pid === draw.pid && input.focused === true && instant(input.at) && input.at >= report.startedAt &&
      input.at <= draw.at && input.clientWidth === 1280 && input.clientHeight === 720 &&
      ['physical-keyboard','physical-mouse'].includes(input.method) && named(input.action), 'EVIDENCE_NATIVE_INPUT', row.id);
    const image = artifact(root, item.image, 16 * 1024 * 1024);
    need(draw.imageSha256 === image.sha256 && image.path.endsWith('.png'), 'EVIDENCE_DRAW_PNG', row.id);
    require('../phase2/rendered.cjs').decodePng(inside(root, image.path), 1280, 720);
  }
}

function validateNamed(root, row, kind, expected, current) {
  need(row && row.id === expected.id && row.kind === kind && named(row.reviewer) && instant(row.reviewedAt) &&
    ['pass','fail'].includes(row.verdict) && named(row.observations) && Array.isArray(row.files) && row.files.length > 0,
  'EVIDENCE_NAMED_' + kind.toUpperCase(), expected.id);
  identity(current, row, kind + ' ' + row.id);
  for (const file of row.files) artifact(root, file);
  if (kind === 'listening') {
    need(named(row.version) && row.version === expected.version && row.sampleSha256 === expected.sampleSha256 &&
      row.files.some(file => file.sha256 === expected.sampleSha256 && file.path.endsWith('.wav')), 'EVIDENCE_LISTENING_SAMPLE', row.id);
  }
  return row.verdict === 'pass';
}

function expectedOutcome(item) {
  return Array.isArray(item.expected) ? item.expected.join(' | ') : item.expected ?? item.rule ?? item.originalId;
}

function reconcile(selected, report, expected) {
  identity(expected, report, 'run');
  need(report.schemaVersion === 1 && UUID.test(report.invocation) && instant(report.startedAt) &&
    instant(report.finishedAt) && report.finishedAt > report.startedAt && Array.isArray(report.outcomes), 'EVIDENCE_RUN');
  exact(selected.map(row => row.id), report.selected, 'declared run IDs', true);
  exact(report.selected, report.outcomes.map(row => row.id), 'run outcomes', true);
  for (const [index, row] of report.outcomes.entries()) {
    need(row.invocation === report.invocation && row.runGroup === selected[index].runGroup &&
      ['pass','fail','blocked','invalid'].includes(row.status) && row.expected === expectedOutcome(selected[index]) && named(row.observed) &&
      (row.status !== 'pass' || row.execution === 'executed-now'), 'EVIDENCE_OUTCOME', row.id);
  }
  const allPass = report.outcomes.every(row => row.status === 'pass');
  need(report.status === (allPass ? 'pass' : 'fail'), 'EVIDENCE_RUN_STATUS');
  return allPass;
}

function freshFinal(selected, reports, expected) {
  need(Array.isArray(reports) && reports.length > 0, 'EVIDENCE_HISTORY_EMPTY');
  const ids = new Set();
  let previous;
  for (const [index, report] of reports.entries()) {
    need(UUID.test(report.invocation) && !ids.has(report.invocation) && instant(report.startedAt) && instant(report.finishedAt) &&
      report.startedAt < report.finishedAt && (!previous || report.startedAt >= previous.finishedAt) &&
      report.previousInvocation === (previous?.invocation ?? null), 'EVIDENCE_HISTORY_ORDER');
    need(index === 0 ? report.kind === 'initial' : ['focused','final'].includes(report.kind), 'EVIDENCE_HISTORY_KIND');
    if (report.kind === 'focused') need(named(report.repairReason) && Array.isArray(report.selected) && report.selected.length > 0,
      'EVIDENCE_REPAIR_PROOF');
    ids.add(report.invocation);
    previous = report;
  }
  need(previous.kind !== 'focused' && reconcile(selected, previous, expected), 'EVIDENCE_FINAL_PASS');
  return previous;
}

module.exports = {PHASE, REVIEW, SELECTION, FIXES, TARGETS, IDENTITY, REVISION, HEX, UUID, need, named, instant, exact,
  read, reference, artifact, document, writeExclusive, git, committedRef, identifySource, identifyTool, identity,
  textArtifact, validateReview, validateProcess, validateRenderedOutcome, validateNamed, expectedOutcome, reconcile, freshFinal};
