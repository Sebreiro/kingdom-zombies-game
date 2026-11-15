'use strict';
const fs=require('node:fs'),path=require('node:path'),e=require('./evidence.cjs');
const selectionPath='.planning/phases/03-four-player-lan-foundation/03-SELECTION.json';
const reviewPath='.planning/phases/03-four-player-lan-foundation/03-REVIEW.json';
function validate(root,catalog){
  e.requireTrue(catalog.schemaVersion===1&&catalog.requiredEnvironment==='development-os','CATALOG_SCHEMA');
  e.exact(catalog.cases.map(row=>row.id),catalog.cases.map(row=>row.id),'catalog');
  const acceptance=new Set(),edges=new Set();
  for(const row of catalog.cases){
    for(const key of ['title','importance','rationale','setup','reset','runGroup'])e.requireTrue(typeof row[key]==='string'&&row[key].trim(),'CATALOG_METADATA '+row.id+'/'+key);
    for(const key of ['actions','expected','evidence','requirements','acceptance','tags'])e.requireTrue(Array.isArray(row[key])&&row[key].length>0,'CATALOG_STEPS '+row.id+'/'+key);
    e.requireTrue(['Critical','High','Normal','Low'].includes(row.importance)&&row.status==='required'&&Number.isInteger(row.order),'CATALOG_IMPORTANCE_ORDER');
    e.requireTrue(['core','adapters','network','runner','rendered'].includes(row.group),'CATALOG_GROUP');
    if(row.needsGameWindow)e.requireTrue(row.roles?.length&&row.images?.length&&row.runGroup!=='G0','CATALOG_RENDERED_ROLES_IMAGES');
    for(const id of row.acceptance)acceptance.add(id);for(const id of row.edges??[])edges.add(id);
  }
  e.exact(Array.from({length:18},(_,index)=>'AC-'+String(index+1).padStart(2,'0')),[...acceptance],'18 acceptance criteria');
  e.exact(Array.from({length:20},(_,index)=>'E'+String(index+1).padStart(2,'0')),[...edges],'20 edges');
  const selection=e.read(e.inside(root,selectionPath));e.exact(catalog.cases.map(row=>row.id),selection.selected.phase3,'saved phase3 selection');
  const inherited=e.read(path.join(root,'tools/phase2/required-cases.json'));e.exact(inherited.cases.map(row=>row.id),selection.selected.phase2,'all 16 inherited');
  const phase1=e.read(path.join(root,'tools/phase1/required-cases.json'));
  const active=phase1.cases.filter(row=>(row.acceptancePhase??1)<=1);e.exact(active.map(row=>row.id),[...selection.selected.phase1,...selection.omitted.phase1.map(row=>row.id)],'phase1 accounted');
  e.requireTrue(selection.omitted.phase1.every(row=>row.reason?.trim())&&selection.omitted.phase2.length===0&&selection.omitted.phase3.length===0,'OMISSION_RATIONALE');
  const scenarios=fs.readFileSync(path.join(root,'.planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md'),'utf8');
  for(const row of catalog.cases)e.requireTrue(scenarios.includes('`'+row.id+'`'),'SAVED_SCENARIO_MISSING '+row.id);
  return selection;
}
function prelaunch(root,context){
  e.requireTrue(process.platform==='win32','DEVELOPMENT_WINDOWS_REQUIRED');
  const catalog=e.read(path.join(root,'tools/phase3/required-cases.json')),savedSelection=validate(root,catalog);
  const review=e.read(e.inside(root,reviewPath));
  e.requireTrue(review.status==='complete'&&review.input===context.input&&review.tool===context.tool&&review.revision===context.revision,'WHOLE_PHASE_REVIEW_REQUIRED');
  e.requireTrue(Array.isArray(review.findings)&&review.findings.every(row=>['critical','high','medium','low','info'].includes(row.severity)
    &&(row.disposition==='resolved'||!['critical','high'].includes(row.severity)&&row.disposition==='accepted')),'REVIEW_FINDINGS_UNRESOLVED');
  e.requireTrue(review.reviewedBy?.trim()&&review.report?.path&&review.report.sha256,'REVIEW_AUTHOR_REPORT');e.artifact({directory:root},review.report);
  const reference=file=>({path:file,sha256:e.hash(fs.readFileSync(e.inside(root,file)))});
  return {review:reference(reviewPath),selection:reference(selectionPath),savedSelection};
}
module.exports={validate,prelaunch,selectionPath,reviewPath};
