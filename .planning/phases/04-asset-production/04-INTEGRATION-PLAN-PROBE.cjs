'use strict';
// Read-only planning consistency probe. Run from the repository root; no production scenario is executed.
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
const threats=rows.flatMap(p=>[...p.s.matchAll(/\| T-04-(\d+) \|/g)].map(x=>+x[1]));if(threats.length!==108||new Set(threats).size!==108)errors.push('threat IDs');
const files=[...new Set(rows.flatMap(p=>p.files))].sort(),artifact=fs.readFileSync(d+'04-ARTIFACTS.md','utf8');for(const path of files)if(!artifact.includes('| '+path+' |'))errors.push('artifact '+path);
const planned=coverage.counts;if(rows.length!==planned.plans||rows.reduce((n,p)=>n+p.tasks.length,0)!==planned.tasks||Math.max(...rows.map(p=>p.wave))!==planned.waves)errors.push('count map');
const expectation=fs.readFileSync('tools/phase4/expected-obligations.json'),expectedHash=crypto.createHash('sha256').update(expectation).digest('hex');if(expectedHash!=='a4d9243bda1b512b59c189a3bdc48a7cccc8171a218bbfc5ae16b00274f2fda5')errors.push('independent authority drift');
const protectedPaths=['tools/phase4/check.cjs','tools/phase4/content.cjs','tools/phase4/expected-obligations.json','src/godot-game/content/pack-index.json','src/godot-game/project.godot','src/godot-game/export_presets.cfg','src/godot-game/inspection/Inspection.tscn',...['01','25','34','35','02'].map(n=>d+'04-'+n+'-SUMMARY.md'),d+'04-PLAN-CHECK.md',d+'04-PLAN-CHECK-INSPECTION-PROFILE.md'];
const protectedHashes=Object.fromEntries(protectedPaths.map(p=>[p,crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex')]));
console.log(JSON.stringify({plans:rows.length,tasks:rows.reduce((n,p)=>n+p.tasks.length,0),waves:Math.max(...rows.map(p=>p.wave)),maxFilesPerPlan:Math.max(...rows.map(p=>p.files.length)),new36Files:rows.find(p=>p.id==='04-36').files.length,plan03Files:rows.find(p=>p.id==='04-03').files.length,backstops:backstops.length,prohibitions:prohibitions.length,threats:threats.length,plannedFiles:files.length,coverage:coverage.counts,expectedHash,protectedHashes,errors}));

process.exitCode=errors.length?1:0;

