'use strict';

const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {spawnSync} = require('node:child_process');
const {isDeepStrictEqual:equal, TextDecoder} = require('node:util');
const {inside, parse, hash} = require('./content.cjs');

const PHASE = '.planning/phases/04-asset-production/';
const REVIEW = PHASE + '04-REVIEW.json';
const REVIEW_REPORT = PHASE + '04-REVIEW.md';
const ORIGINAL_REVIEW = 'e9a1bc4ff9d66f3c79aaf4d572e7f2a78db73be2';
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

function writeExclusive(root, relative, value, limit = 8 * 1024 * 1024) {
  const data = Buffer.isBuffer(value) ? value : Buffer.from(JSON.stringify(value, null, 2) + '\n');
  need(Number.isSafeInteger(limit) && limit >= 0 && limit <= MAX_BYTES && data.length <= limit, 'EVIDENCE_WRITE_SIZE', relative);
  const file = inside(root, relative);
  fs.mkdirSync(path.dirname(file), {recursive:true});
  fs.writeFileSync(inside(root, relative), data, {flag:'wx'});
  return reference(root, relative, limit);
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

function sameCommittedSource(root, revision, current) {
  need(REVISION.test(revision) && REVISION.test(current),'EVIDENCE_HISTORY_REVISION');
  if (revision === current) return true;
  return git(root,['diff','--name-only','-z',revision,current,'--',...SOURCE_ROOTS]).split('\0')
    .filter(file => file && file !== TARGETS).length === 0;
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

function reviewReport(text) {
  const match = /^---\r?\n([\s\S]*?)\r?\n---\r?\n([\s\S]*)$/.exec(text);
  need(match, 'REVIEW_REPORT_ATTESTATION');
  const [, header, body] = match;
  function field(text, key, indentation = '') {
    const rows = [...text.matchAll(new RegExp('^' + indentation + key + ': ([^\\r\\n]+)$', 'gm'))];
    need(rows.length === 1 && named(rows[0][1]), 'REVIEW_REPORT_ATTESTATION', key);
    return rows[0][1];
  }
  // Headings inside fenced examples are data rather than finding declarations.
  let fence = null;
  const narrative = body.split(/\r?\n/).filter(line => {
    const marker = /^ {0,3}(`{3,}|~{3,})/.exec(line)?.[1];
    if (marker && !fence) fence = marker;
    else if (marker && marker[0] === fence?.[0] && marker.length >= fence.length) fence = null;
    else return !fence;
    return false;
  }).join('\n');
  need(!fence, 'REVIEW_REPORT_ATTESTATION', 'unclosed fence');
  const findings = narrative.split(/^### /m).slice(1).flatMap(section => {
    const heading = /^((?:CR|BL|WR|IN)-\d+) (?:— |- )([^\n]+)\n|^((?:CR|BL|WR|IN)-\d+): ([^\n]+)\n/.exec(section);
    if (!heading) {
      need(!/^(?:CR|BL|WR|IN)-/.test(section), 'REVIEW_REPORT_ATTESTATION', 'invalid finding heading');
      return [];
    }
    const id = heading[1] ?? heading[3], summary = heading[2] ?? heading[4];
    const severityFields = [...section.matchAll(/^\*\*Severity:\*\* (critical|high|medium|low|info)[ \t]*$/gm)];
    const severity = severityFields.length === 1 ? severityFields[0][1] : null;
    const classification = /^\*\*Classification:\*\* ([A-Z]+)[ \t]*$/m.exec(section)?.[1];
    const file = /^\*\*File:\*\* ([^\n]+)$/m.exec(section)?.[1];
    const paths = [...(file ?? '').matchAll(/`([^`:]+?)(?::[^`]*)?`/g)].map(row => row[1]);
    need(severity && named(summary) && paths.length > 0 &&
      (!/^(CR|BL)-/.test(id) || ['critical','high'].includes(severity)), 'REVIEW_REPORT_ATTESTATION', id);
    return [{id, summary, severity, classification, paths}];
  });
  const status = field(header, 'status');
  need(status === (findings.length ? 'issues_found' : 'clean'), 'REVIEW_REPORT_STATUS', status);
  const counts = {critical:findings.filter(row => /^(CR|BL)-/.test(row.id)).length,
    warning:findings.filter(row => row.id.startsWith('WR-')).length,
    info:findings.filter(row => row.id.startsWith('IN-')).length, total:findings.length};
  for (const [key, count] of Object.entries(counts)) {
    need(field(header, key, '  ') === String(count), 'REVIEW_REPORT_COUNT', key);
  }
  exact(findings.map(row => row.id),findings.map(row => row.id),'independent report finding IDs');
  return {status, findings, reviewedBy:field(header, 'reviewer'), reviewedAt:field(header, 'reviewed'),
    revision:field(header, 'revision'), source:field(header, 'source'), media:field(header, 'media'), tool:field(header, 'tool')};
}

function validateReview(review, fixes, current, declarations) {
  need(review?.schemaVersion === 1 && review.status === 'complete' && review.scope === 'phase-04-whole-source' &&
    review.independent === true && named(review.reviewedBy) && named(review.implementedBy) &&
    review.reviewedBy.toLowerCase() !== review.implementedBy.toLowerCase() && instant(review.reviewedAt), 'REVIEW_SCHEMA');
  identity(current, review, 'review');
  const report = reviewReport(declarations.report);
  need(['reviewedBy','reviewedAt','revision','source','media','tool'].every(key => report[key] === review[key]) &&
    report.status === review.reportStatus, 'REVIEW_REPORT_ATTESTATION');
  need(fixes?.schemaVersion === 1 && fixes.reviewReport === review.report.sha256 &&
    fixes.originalReport === declarations.originalReport.sha256, 'REVIEW_FIX_BINDING');
  need(Array.isArray(review.findings) && Array.isArray(fixes.findings), 'REVIEW_FINDINGS');
  exact(report.findings.map(row => row.id), review.findings.map(row => row.id), 'current review findings');
  const required = new Map(reviewReport(declarations.originalReport.text).findings.map(row => [row.id,row]));
  for (const finding of report.findings) {
    const declared = review.findings.find(row => row.id === finding.id);
    need(declared.severity === finding.severity && declared.summary === finding.summary, 'REVIEW_FINDING_IDENTITY', finding.id);
    const original = required.get(finding.id);
    need(!original || original.severity === finding.severity && original.summary === finding.summary,
      'REVIEW_FINDING_ID_COLLISION', finding.id);
    if (!original) required.set(finding.id, finding);
  }
  exact([...required.keys()], fixes.findings.map(row => row.id), 'review dispositions');
  for (const finding of required.values()) {
    const disposition = fixes.findings.find(row => row.id === finding.id);
    const cosmetic = ['low','info'].includes(finding.severity) && finding.classification === 'COSMETIC';
    need(named(disposition.reason) && (disposition.status === 'resolved' && disposition.proof ||
      cosmetic && disposition.status === 'accepted'), 'REVIEW_FINDINGS_UNRESOLVED', finding.id);
    if (disposition.status === 'resolved') declarations.verifyProof(finding, disposition.proof);
  }
}

function validateProcess(row) {
  need(row && named(row.role) && Number.isSafeInteger(row.pid) && row.pid > 0 && instant(row.startedAt) &&
    instant(row.finishedAt) && row.finishedAt > row.startedAt && row.timedOut === false && row.crashed === false &&
    row.exitCode === 0 && row.signal === null && row.streamsClosed === true && row.inputReleased === true &&
    Array.isArray(row.remainingPids) && row.remainingPids.length === 0, 'EVIDENCE_PROCESS', row?.role);
}

function validateRenderedOutcome(root, row, report, driver, build = null) {
  need(Array.isArray(row.checkpoints) && row.checkpoints.length > 0 && Array.isArray(row.processes) &&
    row.processes.length > 0 && driver?.id === row.id && Array.isArray(driver.steps),'EVIDENCE_RENDERED_MISSING',row.id);
  exact(driver.steps.map(item => item.id),row.checkpoints.map(item => item.id),'declared physical checkpoint IDs',true);
  const {relaunchCase,validateProcessRuntime,savedNativeSnapshot} = require('./scenarios.cjs');
  const contracts=require('./rendered-contracts.cjs');
  const audition = row.id === contracts.AUDIO_CASE ? audioAuthority(root) : null;
  const relaunch = relaunchCase(row.id);
  need(row.processes.length === (relaunch ? 2 : 1) && (!relaunch || driver.steps.length >= 2),'EVIDENCE_RENDERED_LAUNCHES',row.id);
  const processes = row.processes.map(ref => document(root, ref));
  for (const process of processes) {
    validateProcess(process);
    need(process.startedAt >= report.startedAt && process.finishedAt <= report.finishedAt,'EVIDENCE_PROCESS_TIME',row.id);
    artifact(root,process.log,8*1024*1024);artifact(root,process.executable);
    if (build) {
      const prefix='.phase4/acceptance/'+report.invocation+'/'+row.runGroup+'-launch-';
      const relative=path.posix.dirname(process.log.path);
      need(relative.startsWith(prefix) && /^\d+$/.test(relative.slice(prefix.length)) &&
        process.profile===relative+'/profile' && process.runtime?.path===relative+'/runtime.json' &&
        Array.isArray(process.ownedPids) && process.ownedPids.includes(process.pid) &&
        process.role===(row.runGroup==='G1-hub'?'inspection-hub':'packaged-inspection'),
        'EVIDENCE_PROCESS_ROLE',row.id);
      validateProcessRuntime(root,build,process);
    }
    if (build && row.runGroup === 'G2-packaged') {
      const id = Object.hasOwn(require('./export.cjs').RENDERED_FIXTURES,row.id) ? row.id : null;
      need(process.role === 'packaged-inspection' && process.fixture === id,'EVIDENCE_PACKAGE_SUBJECT',row.id);
      artifact(root,process.package,64*1024*1024);
      if (id) {
        const subject = '.phase4/acceptance/'+report.invocation+'/'+id+'-subject/';
        const approved = build.renderedFixtures[id];
        need(driver.fixture?.pckSha256 === approved.sha256 && process.package.path === subject+'KingdomZombies.pck' &&
          process.package.sha256 === approved.sha256 && process.package.bytes === approved.bytes &&
          process.executable.path === subject+'KingdomZombies.exe' && process.executable.sha256 === build.package.sha256,
          'EVIDENCE_PACKAGE_FIXTURE',row.id);
      } else need(equal(process.package,build.pck) && equal(process.executable,build.package),
        'EVIDENCE_PACKAGE_IDENTITY',row.id);
    }
  }
  exact(processes.map(process => process.pid), processes.map(process => process.pid), 'rendered process IDs');
  if (relaunch) need(processes[0].finishedAt <= processes[1].startedAt && equal(processes[0].executable,processes[1].executable) &&
    equal(processes[0].package,processes[1].package) && processes[0].profile !== processes[1].profile &&
    processes.every(process => process.role === 'packaged-inspection'),'EVIDENCE_RELAUNCH_OWNERSHIP');
  const frames=new Map(), tokens=new Set();
  for (const [index,item] of row.checkpoints.entries()) {
    const step = driver.steps[index], owner = processes[(step.lifetime ?? 1)-1];
    need(owner,'EVIDENCE_RENDERED_LIFETIME',row.id);
    const draw = document(root, item.draw);
    const input = document(root, item.input);
    if (build) {
      const prefix=path.posix.dirname(owner.log.path)+'/'+row.id+'-'+step.id;
      need(item.draw.path===prefix+'-draw.json' && item.input.path===prefix+'-input.json' && item.image.path===prefix+'.png' &&
        draw.before?.path===prefix+'-before.json' && draw.observed?.path===prefix+'-observed.json' &&
        draw.after?.path===prefix+'-after.json','EVIDENCE_RENDERED_CHECKPOINT_SCOPE',row.id+' '+step.id);
    }
    const runtime=document(root,owner.runtime);
    const handshake=require('./native-observer.cjs').decode(artifact(root,runtime.pidPacket,1048576).data);
    need(named(item.id) && draw.caseId === row.id && draw.checkpoint === item.id && draw.invocation === report.invocation &&
      draw.drawn === true && Number.isSafeInteger(draw.frame) && draw.frame > (frames.get(owner.pid)??0) &&
      instant(draw.at) && draw.at >= report.startedAt && draw.at <= owner.finishedAt && owner.pid === draw.pid &&
      handshake[0]==='set_pid' && equal(handshake[2],[owner.pid]) && UUID.test(draw.token) && !tokens.has(draw.token),
      'EVIDENCE_NATIVE_DRAW', row.id);
    frames.set(owner.pid,draw.frame);tokens.add(draw.token);
    need(input.caseId === row.id && input.checkpoint === item.id && input.invocation === report.invocation &&
      input.pid === draw.pid && input.token===draw.token && input.control===step.control && input.focused === true &&
      instant(input.actionStartedAt) && instant(input.at) && input.at >= input.actionStartedAt && input.actionStartedAt >= owner.startedAt &&
      input.at <= draw.at && input.clientWidth === 1280 && input.clientHeight === 720 &&
      input.action === step.action && (step.action === 'click' ? input.method === 'physical-mouse' && input.x === step.x && input.y === step.y :
        input.method === 'physical-keyboard' && input.key === step.key),'EVIDENCE_NATIVE_INPUT',row.id);
    const image = artifact(root, item.image, 16 * 1024 * 1024);
    need(draw.imageSha256 === image.sha256 && image.path.endsWith('.png'), 'EVIDENCE_DRAW_PNG', row.id);
    require('../phase2/rendered.cjs').decodePng(inside(root, image.path), 1280, 720);
    need(draw.frameSource==='composited-client-between-native-observations' && input.acknowledgement?.pid===draw.pid &&
      input.acknowledgement.focused===true && input.acknowledgement.width===1280 && input.acknowledgement.height===720 &&
      input.acknowledgement.action===input.action && (input.action!=='tap'||input.acknowledgement.key===input.key) &&
      draw.capture?.token===draw.token && draw.capture.pid===draw.pid && draw.capture.window===input.acknowledgement.window &&
      draw.capture.imageSha256===image.sha256 && instant(draw.capture.at),'EVIDENCE_PHYSICAL_RECEIPT',row.id);
    if(step.recheck) need(input.recheck?.x===step.recheck.x && input.recheck.y===step.recheck.y && instant(input.recheck.at) &&
      input.recheck.at>=input.actionStartedAt && input.recheck.at<input.at && input.recheck.acknowledgement?.pid===owner.pid &&
      input.recheck.acknowledgement.action==='click' && input.recheck.acknowledgement.focused===true &&
      input.recheck.acknowledgement.window===input.acknowledgement.window,'EVIDENCE_PHYSICAL_RECHECK',row.id);
    if(step.openMenu) need(input.openMenu?.x===step.openMenu.x && input.openMenu.y===step.openMenu.y &&
      instant(input.openMenu.at) && input.openMenu.at>=input.actionStartedAt && input.openMenu.at<input.at &&
      input.openMenu.acknowledgement?.pid===owner.pid && input.openMenu.acknowledgement.action==='click' &&
      input.openMenu.acknowledgement.focused===true && input.openMenu.acknowledgement.window===input.acknowledgement.window,
      'EVIDENCE_PHYSICAL_MENU',step.id);
    need(equal(draw.ready,owner.ready),'EVIDENCE_NATIVE_READY_OWNER',row.id);
    const ready=savedNativeSnapshot(root,draw.ready,contracts.readyChecks,handshake[1]);
    const before=savedNativeSnapshot(root,draw.before,step.checks,handshake[1]);
    const observed=savedNativeSnapshot(root,draw.observed,step.checks,handshake[1]);
    const after=savedNativeSnapshot(root,draw.after,step.checks,handshake[1]);
    need(owner.startedAt<=ready.at && ready.at<before.at && before.at<=input.actionStartedAt && input.at<observed.at &&
      observed.at<=draw.capture.at && draw.capture.at<=after.at && after.at<=draw.at,
      'EVIDENCE_NATIVE_CAPTURE_ORDER',row.id+' '+step.id);
    need(contracts.readyChecks.every((check,index)=>contracts.matches(check,ready.values[index].value)),
      'EVIDENCE_NATIVE_READY',row.id);
    need(step.checks.every((check,index)=>contracts.matches(check,observed.values[index].value)) &&
      before.values.some((row,index)=>!equal(row.value,observed.values[index].value)),
      'EVIDENCE_NATIVE_TRANSITION',row.id+' '+step.id);
    need(equal(observed.values,after.values),'EVIDENCE_NATIVE_CAPTURE_DRIFT',row.id+' '+step.id);
    const playback = audition && contracts.AUDIO_PLAYBACK.has(step.id);
    if (playback) {
      const exactPlayback = audioPlayback(audition,step.id,before.values[0],observed.values[0]);
      need(equal(item.playback,exactPlayback),'EVIDENCE_AUDIO_CHECKPOINT',step.id);
    } else need(item.playback === undefined,'EVIDENCE_AUDIO_CHECKPOINT',step.id);
  }
  if (audition) {
    const media = validateAudioMedia(root,row,report);
    need(media.preparedAt>=document(root,row.checkpoints.at(-1).draw).at &&
      media.preparedAt<=report.finishedAt,'EVIDENCE_AUDIO_MEDIA_TIME',row.id);
  }
}

function validateNamed(root, row, kind, expected, current) {
  need(row && row.id === expected.id && row.kind === kind && named(row.reviewer) && instant(row.reviewedAt) &&
    ['pass','fail'].includes(row.verdict) && named(row.observations) && Array.isArray(row.files) && row.files.length > 0,
  'EVIDENCE_NAMED_' + kind.toUpperCase(), expected.id);
  identity(current, row, kind + ' ' + row.id);
  for (const file of row.files) artifact(root, file);
  if (kind === 'listening') {
    const wave = row.files.find(file => file.sha256 === row.sampleSha256 && file.path.endsWith('.wav'));
    need(named(row.version) && row.version === expected.version && wave &&
      (!expected.sampleSha256 || row.sampleSha256 === expected.sampleSha256), 'EVIDENCE_LISTENING_SAMPLE', row.id);
    const data = artifact(root,wave,64*1024*1024).data;
    try {require('./cases/audio-feasibility.cjs').pcm(data);} catch (error) {throw new Error('EVIDENCE_WAV_FORMAT ' + row.id,{cause:error});}
  }
  return row.verdict === 'pass';
}

const AUDIO_ROOT = 'src/godot-game/';
const LOOP_REPETITIONS = 3;

function audioAuthority(root) {
  const base = AUDIO_ROOT+'content/';
  const catalog = read(root,base+'audio/events.json');
  const events = parse(catalog.data,catalog.path);
  const fixture = document(root,base+'fixtures/audio-states.json');
  const samplePacks = ['packs/audio-actions.json','packs/audio-interface-ambience.json'];
  exact(samplePacks,events.samplePacks,'audio sample packs');
  const samples = samplePacks.flatMap(file => {
    const pack = document(root,base+file);
    return pack.samples.map(sample => ({...sample,pack:file,bank:pack.bank}));
  });
  const sampleById = new Map(samples.map(row => [row.id,row]));
  exact(samples.map(row => row.id),[...sampleById.keys()],'authored sample IDs');
  const routes = new Map(events.events.flatMap(event => event.routes.map(route => [route.id,route])));
  exact(events.events.flatMap(event => event.routes.map(route => route.id)),[...routes.keys()],'audio routes');
  const categories = [...new Set(events.events.map(event => event.category))];
  exact(Array.from({length:30},(_,index) => 'AU-'+String(index+1).padStart(2,'0')),categories,'listening category coverage');
  return {events,catalogSha256:catalog.sha256,routes,states:new Map(fixture.states.map(state=>[state.id,state.loops])),sampleById};
}

function listeningExpectations(root) {
  const {events,catalogSha256,sampleById} = audioAuthority(root);
  const {AUDIO_DECISIONS} = require('./rendered-contracts.cjs');
  const expected = new Map();
  function assign(id, variant, routes, loop = false) {
    const sample = sampleById.get(variant);
    need(sample && HEX.test(sample.sha256) && HEX.test(sample.pcmSha256) && Number.isSafeInteger(sample.version) &&
      /^[a-z\d][a-z\d.-]+$/.test(variant) && routes.length > 0,'EVIDENCE_AUDIO_AUTHORITY',variant);
    const loopFrames = loop ? [sample.loop?.startFrame,sample.loop?.endFrame] : null;
    if (loop) need(sample.loop?.mode==='forward' && equal(loopFrames,[0,sample.frames]),'EVIDENCE_AUDIO_LOOP_AUTHORITY',id);
    expected.set(id,{id,kind:'listening',version:String(sample.version),sampleSha256:sample.sha256,pcmSha256:sample.pcmSha256,
      catalogSha256,variant,routes,pack:sample.pack,bank:sample.bank.path,bankSha256:sample.bank.sha256,
      regionFrames:sample.regionFrames,frames:sample.frames,loopFrames});
  }
  for (const event of events.events) {
    // Two categories can use the same wave, but each exact category / variant must be heard in context.
    for (const variant of new Set([...event.variants,...event.routes.flatMap(row => row.variants)]))
      assign(event.category+'/'+variant,variant,event.routes.filter(route=>route.variants.includes(variant)).map(route=>route.id));
    for (const route of event.routes.filter(row => row.lifecycle.mode === 'current-state-loop')) {
      for (const variant of route.variants) assign(event.category+'/'+route.id+'/'+variant,variant,[route.id],true);
    }
  }
  exact([...sampleById.keys()],[...new Set([...expected.values()].map(row=>row.variant))],'all production audio variants');
  for (const [id,steps] of AUDIO_DECISIONS) expected.set(id,{id,kind:'mix',version:String(events.version),catalogSha256,steps});
  return [...expected.values()];
}

function audioPlayback(authority, step, previous, observed) {
  need(observed?.id && observed.id===previous?.id,'EVIDENCE_AUDIO_CHECKPOINT',step);
  const decode = item => item.value === null ? null : parse(Buffer.from(item.value),step);
  const before = decode(previous), play = decode(observed);
  need(play && Number.isSafeInteger(play.epoch) && play.epoch>0 && Number.isSafeInteger(play.serial) &&
    Number.isSafeInteger(play.oneShots) && play.oneShots===(before?.oneShots??0)+1 &&
    (before ? play.epoch===before.epoch && play.serial>before.serial : play.serial>0),'EVIDENCE_AUDIO_PLAY_TRANSITION',step);
  validateAudioMix(authority,step,play);
  return play;
}

function validateAudioMix(authority, step, play) {
  const required = require('./rendered-contracts.cjs').AUDIO_PLAYBACK.get(step);
  need(required,'EVIDENCE_AUDIO_CHECKPOINT',step);
  const defaults = authority.events.defaults;
  need(play.catalogVersion===authority.events.version && play.masterPercent===defaults.masterPercent &&
    play.effectsPercent===defaults.effectsPercent && play.ambiencePercent===defaults.ambiencePercent &&
    play.muted===false,'EVIDENCE_AUDIO_MIX_SETTINGS',step);
  const ambient = authority.states.get(required.state);
  need(Array.isArray(ambient) && Array.isArray(play.overlap) &&
    (required.state==='quiet' ? ambient.length===0 : ambient.length===2),'EVIDENCE_AUDIO_OVERLAP',step);
  exact([...ambient].sort(),play.overlap.map(clip=>clip.route),step+' overlapping routes',true);
  checkClip(play.cue,required.route,false);
  for (const clip of play.overlap) checkClip(clip,clip.route,true);

  function checkClip(clip,id,loop) {
    const route = authority.routes.get(id);
    const sample = authority.sampleById.get(clip?.variant);
    need(clip?.route===id && route?.variants.length===1 && route.variants[0]===clip.variant &&
      clip.version===sample?.version && clip.wavSha256===sample.sha256 &&
      clip.pcmSha256===sample.pcmSha256,'EVIDENCE_AUDIO_CLIP',step+' '+id);
    if (loop) need(route.lifecycle.mode==='current-state-loop' && route.bus==='Ambience' &&
      sample.loop?.mode==='forward' && equal(clip.loopFrames,[sample.loop.startFrame,sample.loop.endFrame]) &&
      equal(clip.loopFrames,[0,sample.frames]),'EVIDENCE_AUDIO_LOOP',step+' '+id);
    else need(route.lifecycle.mode==='one-shot' && route.bus==='Effects' && clip.loopFrames===null,
      'EVIDENCE_AUDIO_LOOP',step+' '+id);
    const channel = loop ? play.ambiencePercent : play.effectsPercent;
    const volumeDb = 20*Math.log10(play.masterPercent*channel*route.gainLinear/10000);
    need(clip.playing===true && clip.stateLoop===loop && clip.bus===route.bus &&
      clip.gainLinear===route.gainLinear && Number.isFinite(clip.volumeDb) &&
      Math.abs(clip.volumeDb-volumeDb)<0.00001,'EVIDENCE_AUDIO_GAIN',step+' '+id);
  }
}

function auditionMedia(root, invocation, preparedAt, emit = false) {
  const {AUDIO_CASE} = require('./rendered-contracts.cjs');
  const expected = listeningExpectations(root).filter(row=>row.kind==='listening');
  need(UUID.test(invocation) && instant(preparedAt),'EVIDENCE_AUDIO_MEDIA_INVOCATION');
  const prefix = '.phase4/acceptance/'+invocation+'/audio-media/';
  const banks = new Map(), samples = new Map(), loops = new Map();
  const readPcm = require('./cases/audio-feasibility.cjs').pcm;
  function wave(header, pcm, repeats = 1) {
    const data = Buffer.concat([header,...Array(repeats).fill(pcm)]);
    data.writeUInt32LE(data.length-8,4);
    data.writeUInt32LE(data.length-44,40);
    return data;
  }
  function retain(name, data) {
    const ref = {path:prefix+name,sha256:hash(data),bytes:data.length};
    if (emit) writeExclusive(root,ref.path,data);
    else artifact(root,ref,16*1024*1024);
    return ref;
  }
  const rows = expected.map(item => {
    if (!banks.has(item.bank)) {
      const bank = read(root,AUDIO_ROOT+item.bank,16*1024*1024);
      need(bank.sha256===item.bankSha256 && bank.data.toString('ascii',8,16)==='WAVEfmt ' &&
        bank.data.toString('ascii',36,40)==='data' && readPcm(bank.data).pcmBytes===bank.bytes-44,
      'EVIDENCE_AUDIO_BANK',item.bank);
      banks.set(item.bank,bank.data);
    }
    const bank = banks.get(item.bank);
    if (!samples.has(item.variant)) {
      const [first,last] = item.regionFrames;
      const pcm = bank.subarray(44+first*4,44+last*4), header = bank.subarray(0,44);
      const data = wave(header,pcm);
      need(first>=0 && last>first && pcm.length===item.frames*4 && hash(pcm)===item.pcmSha256 &&
        hash(data)===item.sampleSha256,'EVIDENCE_AUDIO_EXTRACT',item.id);
      samples.set(item.variant,retain(item.variant+'.wav',data));
    }
    if (item.loopFrames && !loops.has(item.variant)) {
      const [first,last] = item.regionFrames;
      const pcm = bank.subarray(44+first*4,44+last*4);
      loops.set(item.variant,retain(item.variant+'-three-iterations.wav',wave(bank.subarray(0,44),pcm,LOOP_REPETITIONS)));
    }
    return {sample:item,wav:samples.get(item.variant),loop:item.loopFrames?loops.get(item.variant):null};
  });
  return {schemaVersion:1,invocation,caseId:AUDIO_CASE,preparedAt,rows};
}

function writeAuditionMedia(root, invocation) {
  const data = auditionMedia(root,invocation,new Date().toISOString(),true);
  return writeExclusive(root,'.phase4/acceptance/'+invocation+'/audio-media/index.json',data);
}

function validateAudioMedia(root, outcome, final) {
  const ref = outcome.listeningMedia;
  need(ref?.path==='.phase4/acceptance/'+final.invocation+'/audio-media/index.json','EVIDENCE_AUDIO_MEDIA_SCOPE');
  const data = document(root,ref);
  need(equal(data,auditionMedia(root,final.invocation,data.preparedAt)),'EVIDENCE_AUDIO_MEDIA_CATALOG');
  return data;
}

function listeningSubjects(root, final) {
  const {AUDIO_CASE,AUDIO_DECISIONS} = require('./rendered-contracts.cjs');
  const outcome = final.outcomes.find(row=>row.id===AUDIO_CASE && row.status==='pass');
  need(outcome?.invocation===final.invocation && Array.isArray(outcome.checkpoints),'EVIDENCE_LISTENING_RUN',AUDIO_CASE);
  const media = validateAudioMedia(root,outcome,final);
  const samples = new Map(media.rows.map(row=>[row.sample.id,row]));
  const waves = new Map(media.rows.map(row=>[row.sample.variant,row.wav]));
  const authority = audioAuthority(root);
  const decisions = new Map();
  for (const [id,steps] of AUDIO_DECISIONS) {
    const checkpoints = steps.map(stepId=>{
      const point = outcome.checkpoints.find(row=>row.id===stepId);
      need(point?.playback,'EVIDENCE_AUDIO_DECISION_CHECKPOINT',stepId);
      const draw = document(root,point.draw), input = document(root,point.input);
      need(draw.caseId===AUDIO_CASE && input.caseId===AUDIO_CASE && draw.checkpoint===stepId &&
        input.checkpoint===stepId && draw.invocation===final.invocation && input.invocation===final.invocation &&
        draw.token===input.token && draw.imageSha256===point.image.sha256 && draw.drawn===true,
      'EVIDENCE_AUDIO_DECISION_CHECKPOINT',stepId);
      validateAudioMix(authority,stepId,point.playback);
      const files = [point.playback.cue,...point.playback.overlap].map(binding=>{
        const ref = waves.get(binding.variant);
        need(ref?.sha256===binding.wavSha256,'EVIDENCE_AUDIO_DECISION_WAV',stepId);
        return ref;
      });
      return {id:stepId,draw:point.draw,input:point.input,image:point.image,playback:point.playback,files,
        actionStartedAt:input.actionStartedAt,drawnAt:draw.at};
    });
    decisions.set(id,checkpoints);
  }
  return {media:outcome.listeningMedia,preparedAt:media.preparedAt,samples,decisions};
}

function validateListeningRow(root, row, expected, current, final, subjects) {
  const mix = expected.kind==='mix';
  need(row.method==='actual-listening' && row.invocation===final.invocation && row.caseId===require('./rendered-contracts.cjs').AUDIO_CASE &&
    row.listenerCapability==='actual-hearing' && named(row.outputDevice) && named(row.player) &&
    instant(row.listenedFrom) && instant(row.listenedThrough) && row.listenedFrom>=final.startedAt &&
    row.listenedFrom<row.listenedThrough && row.listenedThrough<=row.reviewedAt &&
    equal(row.mediaIndex,subjects.media),'EVIDENCE_LISTENING_RUN',row.id);
  need(row.version===expected.version && row.catalogSha256===expected.catalogSha256,'EVIDENCE_LISTENING_SAMPLE',row.id);
  if (mix) {
    const checkpoints = subjects.decisions.get(row.id);
    need(row.playbackMethod==='live-native-mix' && row.heardEveryCue===true && equal(row.checkpoints,checkpoints) &&
      checkpoints.every(point=>row.listenedFrom<=point.actionStartedAt && point.drawnAt<=row.listenedThrough),
    'EVIDENCE_AUDIO_DECISION_BINDING',row.id);
    const findings = row.id==='D-31' ? ['swing','impact','confirmation'] : ['dayFire','nightWind','actionAndConfirmation'];
    need(findings.every(field=>named(row.findings?.[field])),'EVIDENCE_AUDIO_HEARD_FINDINGS',row.id);
    const files = [...new Map(checkpoints.flatMap(point=>point.files).map(ref=>[ref.path,ref])).values()];
    need(equal(files,row.files),'EVIDENCE_AUDIO_DECISION_WAV',row.id);
  } else {
    const item = subjects.samples.get(row.id);
    const files = item.loop ? [item.wav,item.loop] : [item.wav];
    need(equal(row.sample,item.sample) && equal(files,row.files) && row.sampleSha256===expected.sampleSha256 &&
      row.playbackMethod===(item.loop?'repeated-loop-extract':'exact-wav-extract') && row.heardEntireSample===true &&
      row.listenedFrom>=subjects.preparedAt,'EVIDENCE_AUDIO_SAMPLE_BINDING',row.id);
    if (item.loop) need(row.heardLoopBoundaries===LOOP_REPETITIONS-1 &&
      named(row.findings?.loopSeam),'EVIDENCE_AUDIO_HEARD_FINDINGS',row.id);
    need(['suitability','sourceIdentity','onsetTail','silence','distortion','music','speech']
      .every(field=>named(row.findings?.[field])),'EVIDENCE_AUDIO_HEARD_FINDINGS',row.id);
  }
  need(validateNamed(root,row,expected.kind,expected,current),'EVIDENCE_LISTENING_FAILED',row.id);
}

function judgmentExpectations(catalog) {
  const ids = {'P1':'AC-22','P2':'AC-23','P3':'AC-24','P4':'AC-21'};
  return catalog.judgments.map(row => ({id:row.id,statement:row.statement,criterion:ids[row.id],
    cases:catalog.acceptance.find(item => item.id === ids[row.id]).actualCases}));
}

function validateJudgments(root, listening, judgments, current, expectedAudio, expectedJudgments, final) {
  const outcomeIds = new Set(final.outcomes.filter(row => row.status === 'pass').map(row => row.id));
  const images = new Map(final.outcomes.map(row => [row.id,(row.checkpoints??[]).map(point => point.image)]));
  for (const [data,schemaVersion] of [[listening,2],[judgments,1]]) {
    need(data?.schemaVersion === schemaVersion && data.invocation === final.invocation && Array.isArray(data.verdicts),'EVIDENCE_JUDGMENT_SET');
    identity(current,data,'named evidence index');
  }
  exact(expectedAudio.map(row => row.id),listening.verdicts.map(row => row.id),'required listening cases');
  exact(expectedJudgments.map(row => row.id),judgments.verdicts.map(row => row.id),'required prohibition judgments');
  const subjects = listeningSubjects(root,final);
  for (const expected of expectedAudio) validateListeningRow(root,listening.verdicts.find(row=>row.id===expected.id),expected,current,final,subjects);
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

function campaignAttempts(root, campaign) {
  need(UUID.test(campaign),'EVIDENCE_CAMPAIGN');
  const base = inside(root,'.phase4/acceptance');
  if (!fs.existsSync(base)) return [];
  const attempts = [];
  for (const row of fs.readdirSync(base,{withFileTypes:true})) {
    if (!row.isDirectory() || !UUID.test(row.name)) continue;
    const directory = '.phase4/acceptance/'+row.name;
    const startPath = directory+'/start.json', reportPath = directory+'/report.json', interruptionPath = directory+'/interrupted.json';
    const exists = relative => fs.existsSync(inside(root,relative));
    if (!exists(startPath)) {
      // A legacy run cannot be adopted by a current campaign.
      for (const relative of [reportPath,directory+'/declared-drivers.json'])
        if (exists(relative)) need(document(root,relative).campaign !== campaign,'EVIDENCE_CAMPAIGN_START_MISSING',row.name);
      continue;
    }
    const start = reference(root,startPath), opening = document(root,start);
    if (opening.campaign !== campaign) continue;
    need(opening.schemaVersion === 1 && opening.invocation === row.name && instant(opening.startedAt) &&
      opening.drivers?.path === directory+'/declared-drivers.json','EVIDENCE_CAMPAIGN_START',row.name);
    artifact(root,opening.drivers,8*1024*1024);
    need(exists(reportPath) !== exists(interruptionPath),'EVIDENCE_CAMPAIGN_TERMINAL',row.name);
    const terminal = reference(root,exists(reportPath)?reportPath:interruptionPath), report = document(root,terminal);
    identity(opening,report,'campaign start');
    const startFields = ['invocation','campaign','startedAt','kind','selected','previousInvocation','repairReason',
      'drivers','reviewSha256','selectionSha256'];
    need(equal(start,report.start) && startFields.every(key => equal(opening[key],report[key])) &&
      (terminal.path === reportPath || (report.status === 'fail' && named(report.interruption))),
      'EVIDENCE_CAMPAIGN_REPORT',terminal.path);
    attempts.push({terminal,report});
  }
  return attempts.sort((a,b) => a.report.startedAt.localeCompare(b.report.startedAt));
}

function historyDocuments(root, refs, campaign) {
  need(Array.isArray(refs) && refs.length > 0,'EVIDENCE_CAMPAIGN');
  const attempts = campaignAttempts(root,campaign);
  exact(attempts.map(row => row.terminal.path),refs.map(ref => ref.path),'immutable campaign history',true);
  const reports = attempts.map(({terminal,report},index) => {
    need(equal(terminal,refs[index]),'EVIDENCE_CAMPAIGN_REFERENCE',terminal.path);
    return report;
  });
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

function guardedHostTuples(root) {
  const policy = read(root,'tests/phase1/EngineAccessPolicy.cs').data.toString('utf8');
  const start = policy.indexOf('private static readonly HashSet<string> _presenters');
  const end = policy.indexOf('};',start);
  need(start >= 0 && end > start,'EVIDENCE_NATIVE_HOST_POLICY');
  const definitions = policy.slice(policy.indexOf('{',start),end);
  const literals = expression => (expression.match(/"(?:[^"\\]|\\.)*"/g) ?? []).map(value => JSON.parse(value)).join('');
  const tuples = [];
  let value = '';
  for (const token of definitions.match(/"(?:[^"\\]|\\.)*"|,|\+/g) ?? []) {
    if (token.startsWith('"')) value += JSON.parse(token);
    if (token === ',' && value) {tuples.push(value);value = '';}
  }
  if (value) tuples.push(value);
  const test = read(root,'tests/phase1/ContentNativePermissionTests.cs').data.toString('utf8');
  const first = test.indexOf('Actual final native hosts require captured first guards');
  const last = test.indexOf('public void Check_PresenterNative_RequiresExactGuardedMember(');
  need(first >= 0 && last > first && tuples.length === 251,'EVIDENCE_NATIVE_HOST_EXTENT');
  const compiledInputs = [...test.slice(first,last).matchAll(/\[InlineData\(([\s\S]*?)\)\]/g)].map(match => literals(match[1]));
  exact(tuples,compiledInputs,'guarded policy and compiled case declarations',true);
  return tuples;
}

function validateNativeHostProof(root, record, result, expectedTuples) {
  need(record && result && result.path.endsWith('/result.json'),'EVIDENCE_NATIVE_HOST_RESULT');
  const subject = document(root,result);
  const prefix = '.phase4/runs/'+subject.nonce+'/';
  need(result.path === prefix+'result.json' && subject.caseId === 'content.native-permissions' && subject.status === 'pass' &&
    subject.observations?.compiled?.selected === expectedTuples.length && subject.observations.compiled.passed === expectedTuples.length &&
    subject.observations.compiled.skipped === 0,'EVIDENCE_NATIVE_HOST_RESULT');
  need(record.selection?.path === prefix+'native-permissions-selection.json' &&
    record.execution?.path === prefix+'native-permissions-execution.json','EVIDENCE_NATIVE_HOST_PROOF_SCOPE');
  const selection = document(root,record.selection), execution = document(root,record.execution);
  need(selection.method === 'KingdomZombies.Phase1.Tests.ContentNativePermissionTests.Check_PresenterNative_RequiresExactGuardedMember' &&
    Array.isArray(selection.tests) && Array.isArray(execution.results) && execution.selected === expectedTuples.length &&
    execution.passed === expectedTuples.length && execution.failed === 0 && execution.skipped === 0 &&
    selection.tests.length === expectedTuples.length,'EVIDENCE_NATIVE_HOST_EXECUTION');
  exact(selection.tests.map(test => test.uid+'/'+test.testUid),execution.results.map(test => test.uid+'/'+test.testUid),
    'compiled guarded case outcomes',true);
  for (const test of execution.results) {
    need(HEX.test(test.uid) && HEX.test(test.testUid) && Array.isArray(test.records) && test.records.length === 5,'EVIDENCE_NATIVE_HOST_CASE');
    const retained = test.records.find(row => row.file.endsWith('/tests.json') || row.file.endsWith('\\tests.json'));
    need(retained,'EVIDENCE_NATIVE_HOST_CTRF');
    const relative = path.relative(root,retained.file).replaceAll('\\','/');
    need(relative.startsWith(prefix) && reference(root,relative).sha256 === retained.sha256,'EVIDENCE_NATIVE_HOST_CTRF',test.uid);
    const report = document(root,relative).results;
    need(report.summary.tests === 1 && report.summary.passed === 1 && report.summary.failed === 0 && report.summary.skipped === 0 &&
      report.tests.length === 1 && report.tests[0].extra.id === test.testUid,'EVIDENCE_NATIVE_HOST_CTRF',test.uid);
  }
}

function inventoryExpectation(root, revision, catalog) {
  const files = changedFiles(root,revision);
  const exemption = document(root,'docs/code/exemptions.json');
  const generated = document(root,'docs/code/generated-callsite-adaptations.json');
  const spans = document(root,'docs/code/engine-generated-spans.json');
  const tracked = files.filter(file => file.startsWith('production/phase4/') && file.endsWith('.zip'));
  const guarded = guardedHostTuples(root);
  return {files, cases:catalog.cases.map(row => row.id), archives:tracked, guarded,
    native:[...exemption.local,...generated.local,...spans.local].map(row => row.id).concat(guarded).sort()};
}

function validateArtifactInventory(root, record, expected, sourceAudit, current) {
  need(record?.schemaVersion === 1 && sourceAudit?.schemaVersion === 1 && Array.isArray(record.files) &&
    Array.isArray(record.sourceSymbols) && Array.isArray(sourceAudit.sourceSymbols) && Array.isArray(record.archives) &&
    Array.isArray(sourceAudit.archives) && Array.isArray(record.caseExports) && Array.isArray(record.nativePermits) &&
    Array.isArray(sourceAudit.nativeHostTuples),'EVIDENCE_INVENTORY');
  identity(current,record,'artifact inventory');identity(current,sourceAudit,'independent resource and symbol audit');
  exact(expected.files,record.files.map(row => row.path),'changed tracked artifacts');
  exact(expected.cases,record.caseExports,'registered case exports');
  exact(expected.native,record.nativePermits,'native permission tuples');
  exact(expected.guarded,sourceAudit.nativeHostTuples,'independent compiled guarded-host tuples');
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

module.exports = {PHASE, REVIEW, REVIEW_REPORT, ORIGINAL_REVIEW, SELECTION, FIXES, TARGETS, PHASE_BASE, LISTENING, JUDGMENTS, COMPLETION, IDENTITY, REVISION, HEX, UUID, need, named, instant, exact,
  read, reference, artifact, document, writeExclusive, git, committedRef, sameCommittedSource, identifySource, identifyTool, identity,
  textArtifact, reviewReport, validateReview, validateProcess, validateRenderedOutcome, validateNamed, listeningExpectations, judgmentExpectations,
  audioAuthority, audioPlayback, writeAuditionMedia, listeningSubjects,
  validateJudgments, campaignAttempts, historyDocuments, validateCoverage, changedFiles, guardedHostTuples, validateNativeHostProof, inventoryExpectation, validateArtifactInventory,
  expectedOutcome, reconcile, freshFinal};
