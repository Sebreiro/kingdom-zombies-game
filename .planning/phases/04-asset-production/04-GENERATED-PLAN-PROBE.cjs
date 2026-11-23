'use strict';
// Read-only frozen generated-authentication planning probe; run from the repository root.
const fs=require('fs'),crypto=require('crypto'),f=require(process.cwd()+'/.codex/gsd-core/bin/lib/frontmatter.cjs'),{projectProhibitions}=require(process.cwd()+'/.codex/gsd-core/bin/lib/probe-core.cjs');
const d='.planning/phases/04-asset-production/',rows=fs.readdirSync(d).filter(n=>/^04-\d+-PLAN.md$/.test(n)).map(n=>{const s=fs.readFileSync(d+n,'utf8'),fm=f.extractFrontmatter(s);return {id:n.slice(0,5),n,s,fm,wave:+fm.wave,files:fm.files_modified,tasks:[...s.matchAll(/<task [\s\S]*?<\/task>/g)].map(x=>x[0]),truths:f.parseMustHavesBlock(s,'truths'),prohibitions:f.parseMustHavesBlock(s,'prohibitions')};}),coverage=JSON.parse(fs.readFileSync(d+'04-PLAN-COVERAGE.json')),errors=[];
for(const p of rows){if(p.files.length>9||p.tasks.length<2||p.tasks.length>3)errors.push('bounds '+p.id);
 for(const t of p.tasks){const paths=t.match(/<files>([\s\S]*?)<\/files>/)[1].split(',').map(x=>x.trim());if(paths.length>5||paths.some(x=>!p.files.includes(x)))errors.push('task ownership '+p.id);
 for(const tag of ['read_first','files','action','acceptance_criteria','verify'])if(!t.includes('<'+tag+'>'))errors.push('missing '+tag+' '+p.id);}
 if(!p.s.trimEnd().endsWith('</output>')||p.s.includes('gsd:write-continue'))errors.push('tail '+p.id);
 for(const command of p.s.matchAll(/<automated>[\s\S]*?<\/automated>([\s\S]*?)(?=<automated>|<\/verify>|<\/check>)/g))if(!/^\s*<fails_when>/.test(command[1]))errors.push('fails_when '+p.id);
 for(const dep of p.fm.depends_on){const q=rows.find(x=>x.id===dep);if(!q||q.wave>=p.wave)errors.push('dependency '+p.id+' '+dep);}
 if(!p.s.includes('<threat_model>')||!p.s.includes('block_on high'))errors.push('security '+p.id);}
for(let i=0;i<rows.length;i++)for(let j=0;j<i;j++)if(rows[i].wave===rows[j].wave&&rows[i].files.some(x=>rows[j].files.includes(x)))errors.push('same-wave '+rows[i].id+' '+rows[j].id);
const backstops=rows.flatMap(x=>x.truths.filter(t=>t&&typeof t==='object'&&t.verification==='backstop')),prohibitions=rows.flatMap(x=>x.prohibitions),projected=projectProhibitions(coverage.prohibitions);
if(backstops.length!==104||prohibitions.length!==4||JSON.stringify(projected)!==JSON.stringify(prohibitions))errors.push('truth/prohibition projection');
for(const h of coverage.heldout)if(!backstops.some(t=>t.statement===h.sourceIntent))errors.push('H identity '+h.id);
for(const u of coverage.ui)if(!backstops.some(t=>t.statement===u.surface+' / '+u.category+': '+u.sourceIntent))errors.push('UI identity '+u.id);
const threats=rows.flatMap(p=>[...p.s.matchAll(/\| T-04-(\d+) \|/g)].map(x=>+x[1]));if(threats.length!==111||new Set(threats).size!==111)errors.push('threat IDs');
const files=[...new Set(rows.flatMap(p=>p.files))].sort(),artifact=fs.readFileSync(d+'04-ARTIFACTS.md','utf8');for(const path of files)if(!artifact.includes('| '+path+' |'))errors.push('artifact '+path);
const planned=coverage.counts;if(rows.length!==planned.plans||rows.reduce((n,p)=>n+p.tasks.length,0)!==planned.tasks||Math.max(...rows.map(p=>p.wave))!==planned.waves)errors.push('count map');
const expectation=fs.readFileSync('tools/phase4/expected-obligations.json'),expectedHash=crypto.createHash('sha256').update(expectation).digest('hex');if(expectedHash!=='a4d9243bda1b512b59c189a3bdc48a7cccc8171a218bbfc5ae16b00274f2fda5')errors.push('independent authority drift');
const protectedPaths=['tools/phase4/check.cjs','tools/phase4/content.cjs','tools/phase4/expected-obligations.json','src/godot-game/content/pack-index.json','src/godot-game/project.godot','src/godot-game/export_presets.cfg','src/godot-game/inspection/Inspection.tscn',...['01','25','34','35','02'].map(n=>d+'04-'+n+'-SUMMARY.md'),d+'04-PLAN-CHECK.md',d+'04-PLAN-CHECK-INSPECTION-PROFILE.md'];
const protectedHashes=Object.fromEntries(protectedPaths.map(p=>[p,crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex')]));

const revisionInputs=JSON.parse(fs.readFileSync(d+'04-INTEGRATION-REVISION-1-INPUTS.json'));
const omitOwnership=item=>Object.fromEntries(Object.entries(item).filter(([key])=>!revisionInputs.sourceIdentityProjection.excludedOwnershipKeys.includes(key)));
const identityHashes=Object.fromEntries(Object.entries(revisionInputs.sourceIdentityProjection.baselineSha256).map(([key,expected])=>{
 const actual=crypto.createHash('sha256').update(JSON.stringify(coverage[key].map(omitOwnership))).digest('hex');
 if(actual!==expected)errors.push('source identity '+key);return [key,actual];}));
const producer=rows.find(p=>p.id==='04-03'),catalog=rows.find(p=>p.id==='04-30'),consumer=rows.find(p=>p.id==='04-31');
const consumerFiles=consumer.tasks.map(task=>task.match(/<files>([\s\S]*?)<\/files>/)[1].split(',').map(x=>x.trim()));
if(consumer.files.length!==9||consumerFiles.some(x=>x.length!==5))errors.push('consumer bounds');
for(const path of revisionInputs.plannedConsumers.paths)if(!consumerFiles[0].includes(path))errors.push('consumer ownership '+path);
if(consumer.tasks[0].includes('<automated>node tools/phase4/check.cjs --group export</automated>')||!consumer.tasks[1].includes('<automated>node tools/phase4/check.cjs --group export</automated>'))errors.push('consumer group gate ordering');
for(const contract of ['natural','packaged','current','PID','180','read-only','independent','export.profile','source.tracer','export.hub-consumer'])if(!consumer.tasks[0].includes(contract))errors.push('consumer contract '+contract);
if(!producer.tasks[0].includes('Summary/ResourceId')||(!producer.tasks[0].includes('naturally')&&!producer.tasks[0].includes('Natural inspection startup'))||!producer.tasks[0].includes('common.bow'))errors.push('producer contract');
if(!catalog.tasks[0].includes('export.hub-consumer')||!catalog.tasks[0].includes('pending-before-creation'))errors.push('consumer required catalog');
if(coverage.obligations.find(x=>x.id==='DT-27').task!==2)errors.push('DT27 final task');
for(const id of ['AC-15','AC-16','AC-17'])if(!coverage.acceptance.find(x=>x.id===id).owners.includes('04-31 Task 2'))errors.push('AC final task '+id);


const generatedInputs=JSON.parse(fs.readFileSync(d+'04-GENERATED-PLANNING-INPUTS.json'));
const sha=bytes=>crypto.createHash('sha256').update(bytes).digest('hex');
for(const row of generatedInputs.subjects)if(sha(fs.readFileSync(row.path))!==row.sha256)errors.push('frozen03 '+row.path);
for(const row of generatedInputs.outputs){const bytes=fs.readFileSync(row.retainedCopy);if(sha(bytes)!==row.sha256||sha(Buffer.from(bytes.toString('utf8').replace(/^\uFEFF/,'')))!==row.textSha256)errors.push('retained generated '+row.path);}
if(sha(fs.readFileSync(generatedInputs.package))!==generatedInputs.packageSha256||generatedInputs.diagnostics!==177)errors.push('producer input');
const generated=rows.find(p=>p.id==='04-37'),inputPlan=rows.find(p=>p.id==='04-26');
const gf=generated.tasks.map(t=>t.match(/<files>([\s\S]*?)<\/files>/)[1].split(',').map(x=>x.trim()));
if(generated.files.length!==9||gf[0].length!==5||gf[1].length!==4)errors.push('37 ownership');
if(generated.fm.depends_on.join(',')!=='04-36'||producer.fm.depends_on.join(',')!=='04-37')errors.push('37 dependency');
if(producer.files.length!==7||inputPlan.files.length!==9)errors.push('03/26 budgets');
const nativeCommand='<automated>dotnet build src/godot-game/KingdomZombies.csproj';
if(generated.tasks[0].includes(nativeCommand)||!generated.tasks[1].includes(nativeCommand))errors.push('37 full native ordering');
for(const path of ['tools/code-rules/EngineGeneratedSyntax.cs','docs/code/engine-generated-spans.json','docs/code/generated-callsite-adaptations.json'])if(producer.files.includes(path)||!generated.files.includes(path)||!inputPlan.files.includes(path))errors.push('generated exact owner '+path);
const gate=inputPlan.tasks[1];if(gate.indexOf('engine.generated-origin')>gate.indexOf('engine.source-policy</automated>')||!gate.includes('engine.generated-origin'))errors.push('26 gate ordering');
for(const count of ['220','18','16','26'])if(!generated.tasks[0].includes(count))errors.push('baseline '+count);
for(const id of ['engine.generated-origin','engine.generated-boundary','rules.generated-callsite-origin','rules.generated-callsite-boundary','rules.godot-callsite-boundary','R-50.positive','ac-08.complete','rules.exemptions'])if(!generated.tasks[1].includes('--case '+id))errors.push('37 actual gate '+id);

console.log(JSON.stringify({generatedOwnership:{plan37Files:generated.files.length,taskFiles:gf.map(x=>x.length),plan03Files:producer.files.length,plan26Files:inputPlan.files.length,frozen:generatedInputs.subjects.length,observedDiagnostics:generatedInputs.diagnostics,fullNativeGateTask:2},consumerOwnership:{plan31Files:consumer.files.length,taskFiles:consumerFiles.map(x=>x.length),codePaths:consumerFiles[0],finalGroupTask:2,newCase:'export.hub-consumer'},identityHashes,plans:rows.length,tasks:rows.reduce((n,p)=>n+p.tasks.length,0),waves:Math.max(...rows.map(p=>p.wave)),maxFilesPerPlan:Math.max(...rows.map(p=>p.files.length)),new36Files:rows.find(p=>p.id==='04-36').files.length,plan03Files:rows.find(p=>p.id==='04-03').files.length,backstops:backstops.length,prohibitions:prohibitions.length,threats:threats.length,plannedFiles:files.length,coverage:coverage.counts,expectedHash,protectedHashes,errors}));
process.exitCode=errors.length?1:0;



