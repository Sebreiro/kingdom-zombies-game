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
const PHASE_BASE = '5428da86b6dc753d43472ea8460acb997d7e15dd';
const LISTENING = PHASE + '04-LISTENING.json';
const JUDGMENTS = PHASE + '04-JUDGMENTS.json';
const COMPLETION = PHASE + '04-COMPLETION.json';
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
  for (const process of processes) {
    validateProcess(process);
    need(process.startedAt >= report.startedAt && process.finishedAt <= report.finishedAt,'EVIDENCE_PROCESS_TIME',row.id);
    artifact(root,process.log,8*1024*1024);artifact(root,process.executable);
  }
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
    need(draw.frameSource==='composited-client-after-DwmFlush' && input.acknowledgement?.pid===draw.pid &&
      input.acknowledgement.focused===true && input.acknowledgement.width===1280 && input.acknowledgement.height===720 &&
      input.acknowledgement.action===input.action,'EVIDENCE_PHYSICAL_RECEIPT',row.id);
    const {decode} = require('./native-observer.cjs');
    const tree = decode(artifact(root,draw.nativeTree,1048576).data), object = decode(artifact(root,draw.nativeObject,1048576).data);
    const observation = JSON.parse(draw.observation);
    const nativeId = require('./scenarios.cjs').treePaths(tree[2]).get(observation.node);
    need(tree[0]==='scene:scene_tree' && object[0]==='scene:inspect_objects' && tree[1]===object[1] && nativeId &&
      object[2].find(item => item[0]===nativeId)?.[2]?.some(prop => prop[0]===observation.property && equal(prop[5],observation.value)),
    'EVIDENCE_NATIVE_PACKET',row.id);
  }
}

function validateNamed(root, row, kind, expected, current) {
  need(row && row.id === expected.id && row.kind === kind && named(row.reviewer) && instant(row.reviewedAt) &&
    ['pass','fail'].includes(row.verdict) && named(row.observations) && Array.isArray(row.files) && row.files.length > 0,
  'EVIDENCE_NAMED_' + kind.toUpperCase(), expected.id);
  identity(current, row, kind + ' ' + row.id);
  for (const file of row.files) artifact(root, file);
  if (kind === 'listening' || kind === 'mix') {
    const wave = row.files.find(file => file.sha256 === row.sampleSha256 && file.path.endsWith('.wav'));
    need(named(row.version) && row.version === expected.version && wave &&
      (!expected.sampleSha256 || row.sampleSha256 === expected.sampleSha256), 'EVIDENCE_LISTENING_SAMPLE', row.id);
    const data = artifact(root,wave,64*1024*1024).data;
    try {require('./cases/audio-feasibility.cjs').pcm(data);} catch (error) {throw new Error('EVIDENCE_WAV_FORMAT ' + row.id,{cause:error});}
  }
  return row.verdict === 'pass';
}

function listeningExpectations(root) {
  const base = 'src/godot-game/content/';
  const events = document(root,base+'audio/events.json');
  const samplePacks = ['packs/audio-actions.json','packs/audio-interface-ambience.json'];
  exact(samplePacks,events.samplePacks,'audio sample packs');
  const samples = samplePacks.flatMap(file => document(root,base+file).samples);
  const sampleById = new Map(samples.map(row => [row.id,row]));
  exact(samples.map(row => row.id),[...sampleById.keys()],'authored sample IDs');
  const categories = [...new Set(events.events.map(event => event.category))];
  exact(Array.from({length:30},(_,index) => 'AU-'+String(index+1).padStart(2,'0')),categories,'listening category coverage');
  const expected = new Map();
  function assign(id, variant, mode = 'listening') {
    const sample = sampleById.get(variant);
    need(sample && HEX.test(sample.sha256) && Number.isSafeInteger(sample.version),'EVIDENCE_AUDIO_AUTHORITY',variant);
    expected.set(id,{id,kind:mode,version:String(sample.version),sampleSha256:sample.sha256,variant});
  }
  for (const event of events.events) {
    // Routes and the event union are intentionally both checked. Two categories may use the same bytes.
    for (const id of new Set([...event.variants,...event.routes.flatMap(row => row.variants)])) assign(event.category+'/'+id,id);
    for (const route of event.routes.filter(row => row.lifecycle.mode === 'current-state-loop')) {
      for (const id of route.variants) assign(event.category+'/'+route.id+'/'+id,id);
    }
  }
  for (const id of ['D-31','D-32']) expected.set(id,{id,kind:'mix',version:String(events.version)});
  return [...expected.values()];
}

function judgmentExpectations(catalog) {
  const ids = {'P1':'AC-22','P2':'AC-23','P3':'AC-24','P4':'AC-21'};
  return catalog.judgments.map(row => ({id:row.id,statement:row.statement,criterion:ids[row.id],
    cases:catalog.acceptance.find(item => item.id === ids[row.id]).actualCases}));
}

function validateJudgments(root, listening, judgments, current, expectedAudio, expectedJudgments, final) {
  const outcomeIds = new Set(final.outcomes.filter(row => row.status === 'pass').map(row => row.id));
  const images = new Map(final.outcomes.map(row => [row.id,(row.checkpoints??[]).map(point => point.image)]));
  for (const data of [listening,judgments]) {
    need(data?.schemaVersion === 1 && data.invocation === final.invocation && Array.isArray(data.verdicts),'EVIDENCE_JUDGMENT_SET');
    identity(current,data,'named evidence index');
  }
  exact(expectedAudio.map(row => row.id),listening.verdicts.map(row => row.id),'required listening cases');
  exact(expectedJudgments.map(row => row.id),judgments.verdicts.map(row => row.id),'required prohibition judgments');
  for (const expected of expectedAudio) {
    const row = listening.verdicts.find(item => item.id === expected.id);
    need(row.method === 'actual-listening' && row.reviewedAt >= final.startedAt &&
      row.caseId === 'rendered.audio-audition' && outcomeIds.has(row.caseId), 'EVIDENCE_LISTENING_RUN', row.id);
    need(validateNamed(root,row,expected.kind,expected,current),'EVIDENCE_LISTENING_FAILED',row.id);
  }
  for (const expected of expectedJudgments) {
    const row = judgments.verdicts.find(item => item.id === expected.id);
    need(row.method === 'actual-named-review' && row.reviewedAt >= final.startedAt && row.statement === expected.statement &&
      row.criterion === expected.criterion && expected.cases.length > 0,'EVIDENCE_JUDGMENT_RUN',row.id);
    exact(expected.cases,row.caseIds,row.id+' individual actual checks');
    for (const caseId of row.caseIds) need(outcomeIds.has(caseId) && images.get(caseId)?.some(image =>
      row.files?.some(file => equal(image,file))), 'EVIDENCE_JUDGMENT_IMAGE', row.id+' '+caseId);
    need(validateNamed(root,row,'judgment',expected,current),'EVIDENCE_JUDGMENT_FAILED',row.id);
  }
}

function historyDocuments(root, refs, campaign) {
  need(UUID.test(campaign) && Array.isArray(refs) && refs.length > 0,'EVIDENCE_CAMPAIGN');
  const reports = refs.map(ref => {
    const report = document(root,ref);
    need(ref.path === '.phase4/acceptance/'+report.invocation+'/report.json' && report.campaign === campaign,'EVIDENCE_CAMPAIGN_REPORT',ref.path);
    return report;
  });
  const base = inside(root,'.phase4/acceptance');
  const recorded = fs.readdirSync(base,{withFileTypes:true}).filter(row => row.isDirectory() && UUID.test(row.name)).flatMap(row => {
    const relative = '.phase4/acceptance/'+row.name+'/report.json';
    return fs.existsSync(inside(root,relative)) && document(root,relative).campaign === campaign ? [row.name] : [];
  });
  exact(recorded,reports.map(report => report.invocation),'immutable campaign history');
  return reports;
}

function validateCoverage(index, expected, receipts) {
  for (const key of ['obligations','acceptance','heldout','backstops','decisions','judgments']) {
    const supplied = index[key];
    need(Array.isArray(supplied) && expected[key].length > 0,'EVIDENCE_COVERAGE',key);
    exact(expected[key].map(row => row.id),supplied.map(row => row.id),key);
    for (const row of expected[key]) {
      const found = supplied.find(item => item.id === row.id);
      exact(row.cases,found.cases,row.id+' coverage receipts');
      need(found.cases.length > 0 && found.cases.every(id => receipts.has(id)),'EVIDENCE_COVERAGE_RECEIPT',row.id);
    }
  }
}

function changedFiles(root, revision) {
  need(REVISION.test(revision),'EVIDENCE_REVISION');
  git(root,['merge-base','--is-ancestor',PHASE_BASE,revision]);
  return git(root,['diff','--name-only','--diff-filter=ACMRT','-z',PHASE_BASE,revision]).split('\0').filter(Boolean);
}

function inventoryExpectation(root, revision, catalog) {
  const files = changedFiles(root,revision);
  const exemption = document(root,'docs/code/exemptions.json');
  const generated = document(root,'docs/code/generated-callsite-adaptations.json');
  const spans = document(root,'docs/code/engine-generated-spans.json');
  const tracked = files.filter(file => file.startsWith('production/phase4/') && file.endsWith('.zip'));
  return {files, cases:catalog.cases.map(row => row.id), archives:tracked,
    native:[...exemption.local,...generated.local,...spans.local].map(row => row.id).sort()};
}

function validateArtifactInventory(root, record, expected, sourceAudit, current) {
  need(record?.schemaVersion === 1 && sourceAudit?.schemaVersion === 1 && Array.isArray(record.files) &&
    Array.isArray(record.sourceSymbols) && Array.isArray(sourceAudit.sourceSymbols) && Array.isArray(record.archives) &&
    Array.isArray(sourceAudit.archives) && Array.isArray(record.caseExports) && Array.isArray(record.nativePermits),'EVIDENCE_INVENTORY');
  identity(current,record,'artifact inventory');identity(current,sourceAudit,'independent resource and symbol audit');
  exact(expected.files,record.files.map(row => row.path),'changed tracked artifacts');
  exact(expected.cases,record.caseExports,'registered case exports');
  exact(expected.native,record.nativePermits,'native permission tuples');
  exact(expected.archives,record.archives.map(row => row.path),'retained source archive paths');
  for (const ref of record.files) artifact(root,ref);
  const signature = row => [row.path,row.member,row.signature].join('\0');
  exact(sourceAudit.sourceSymbols.map(signature),record.sourceSymbols.map(signature),'exact emitted source and compiled symbols');
  const code = expected.files.filter(file => /\.(cs|cjs|py|ps1)$/.test(file));
  const currentSymbols = new Set(record.sourceSymbols.map(row => row.path));
  for (const file of code) need(currentSymbols.has(file),'EVIDENCE_SYMBOL_FILE_MISSING',file);
  for (const row of record.sourceSymbols) {
    need(expected.files.includes(row.path) && named(row.member) && named(row.signature) &&
      Number.isSafeInteger(row.line) && row.line > 0 && named(row.sourceLine),'EVIDENCE_SYMBOL',row.path);
    const source = read(root,row.path).data.toString('utf8').split(/\r?\n/);
    need(source[row.line-1]?.trim() === row.sourceLine && row.sourceLine.includes(row.member),'EVIDENCE_SYMBOL_SOURCE',row.path+' '+row.member);
  }
  exact(sourceAudit.archives.map(row => row.path),record.archives.map(row => row.path),'audited retained archives');
  for (const row of record.archives) {
    const other = sourceAudit.archives.find(archive => archive.path === row.path);
    need(Array.isArray(row.members) && row.members.length > 0 && equal(row,other),'EVIDENCE_ARCHIVE_MEMBERS',row.path);
  }
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
  let previous, repairPending = false;
  for (const [index, report] of reports.entries()) {
    need(UUID.test(report.invocation) && !ids.has(report.invocation) && instant(report.startedAt) && instant(report.finishedAt) &&
      report.startedAt < report.finishedAt && (!previous || report.startedAt >= previous.finishedAt) &&
      report.previousInvocation === (previous?.invocation ?? null), 'EVIDENCE_HISTORY_ORDER');
    need(index === 0 ? report.kind === 'initial' : ['focused','final'].includes(report.kind), 'EVIDENCE_HISTORY_KIND');
    need(report.schemaVersion === 1 && Array.isArray(report.selected) && report.selected.length > 0 && Array.isArray(report.outcomes),
      'EVIDENCE_HISTORY_OUTCOMES');
    exact(report.selected,report.outcomes.map(row => row.id),'historical selected and reported cases',true);
    need(report.outcomes.every(row => row.invocation === report.invocation && ['pass','fail','blocked','invalid'].includes(row.status)) &&
      report.status === (report.outcomes.every(row => row.status === 'pass') ? 'pass' : 'fail'),'EVIDENCE_HISTORY_OUTCOMES');
    if (report.kind === 'focused') need(named(report.repairReason) && Array.isArray(report.selected) && report.selected.length > 0,
      'EVIDENCE_REPAIR_PROOF');
    if (report.kind === 'final') need(!repairPending,'EVIDENCE_REPAIR_PROOF');
    if (report.status === 'fail') repairPending = true;
    if (report.kind === 'focused' && report.status === 'pass') repairPending = false;
    ids.add(report.invocation);
    previous = report;
  }
  need(previous.kind !== 'focused' && reconcile(selected, previous, expected), 'EVIDENCE_FINAL_PASS');
  return previous;
}

module.exports = {PHASE, REVIEW, SELECTION, FIXES, TARGETS, PHASE_BASE, LISTENING, JUDGMENTS, COMPLETION, IDENTITY, REVISION, HEX, UUID, need, named, instant, exact,
  read, reference, artifact, document, writeExclusive, git, committedRef, identifySource, identifyTool, identity,
  textArtifact, validateReview, validateProcess, validateRenderedOutcome, validateNamed, listeningExpectations, judgmentExpectations,
  validateJudgments, historyDocuments, validateCoverage, changedFiles, inventoryExpectation, validateArtifactInventory,
  expectedOutcome, reconcile, freshFinal};
