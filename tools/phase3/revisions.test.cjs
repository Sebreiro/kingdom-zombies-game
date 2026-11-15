'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const {spawnSync}=require('node:child_process'),e=require('./evidence.cjs'),r=require('./revisions.cjs');
const catalog=require('./catalog.cjs'),baseline=require('../phase1/check.cjs'),check=require('./check.cjs');
const project=path.resolve(__dirname,'../..');
function fixture(){
  const root=fs.mkdtempSync(path.join(project,'.phase3/revision-control-'));
  const write=(file,value)=>{fs.mkdirSync(path.dirname(path.join(root,file)),{recursive:true});fs.writeFileSync(path.join(root,file),typeof value==='string'?value:JSON.stringify(value));};
  const git=(...args)=>{const result=spawnSync('git',args,{cwd:root,encoding:'utf8',windowsHide:true});assert.equal(result.status,0,result.stderr);return result.stdout.trim();};
  const commit=()=>{git('add','.');git('commit','-qm','control');return git('rev-parse','HEAD');};
  git('init','-q');git('config','user.email','control@example.invalid');git('config','user.name','Control');git('config','core.autocrlf','false');
  write('.gitignore','.phase1/\n.phase2/\n.phase3/\n');
  for(const input of check.inputManifest().inputs){
    const isFile=path.posix.basename(input).includes('.')||input.endsWith('.editorconfig');
    write(isFile?input:input+'/control.txt','CONTROL_ONLY');
  }
  for(const file of ['tools/phase1/required-cases.json','tools/phase2/required-cases.json','tools/phase3/required-cases.json',catalog.selectionPath,
    '.planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md'])write(file,fs.readFileSync(path.join(project,file),'utf8'));
  for(const input of e.read(path.join(root,'tools/phase1/required-cases.json')).inputs){
    if(!fs.existsSync(path.join(root,input)))write(path.posix.basename(input).includes('.')?input:input+'/control.txt','CONTROL_ONLY');
  }
  // These records exercise validators, never claim game/test execution credit.
  const cases=e.read(path.join(root,'tools/phase3/required-cases.json')).cases.map(row=>({...row,id:row.id==='rendered.timing'?'runner.control-timing':row.id,needsGameWindow:false,roles:undefined,additionalRoles:undefined,group:'runner',implementation:'control'}));
  const savedSelection=e.read(path.join(root,catalog.selectionPath));savedSelection.selected.phase3=cases.map(row=>row.id);write(catalog.selectionPath,savedSelection);
  fs.appendFileSync(path.join(root,'.planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md'),'\n`runner.control-timing` control only\n');
  const phase2=e.read(path.join(root,'tools/phase2/required-cases.json'));phase2.cases=phase2.cases.map(row=>({...row,implementation:'control',images:undefined}));write('tools/phase2/required-cases.json',phase2);
  write('tools/phase3/required-cases.json',{schemaVersion:1,requiredEnvironment:'development-os',cases});
  const reviewed=commit(),input=baseline.snapshot(root,check.inputManifest()).sha256;
  const identity={input,tool:'control-tool',platform:process.platform+'-'+process.arch,osFingerprint:'control-os'};
  write(catalog.reviewPath.replace('.json','.md'),'Immutable original review.');
  const ref=file=>({path:file,sha256:e.hash(fs.readFileSync(path.join(root,file)))});
  const review={status:'complete',...identity,revision:reviewed,reviewedBy:'Control',findings:[],report:ref(catalog.reviewPath.replace('.json','.md'))};
  write(catalog.reviewPath,review);const tested=commit(),context={...identity,revision:tested,root,directory:path.join(root,'.phase3/runs/control/final')};fs.mkdirSync(context.directory,{recursive:true});
  const gate=catalog.prelaunch(root,context),selection=gate.savedSelection;
  const record=(family,summary)=>{const prefix='.'+family+'/runs/control';write(prefix+'/summary.json',summary);write(prefix+'/process.json',{code:0,signal:null,timedOut:false,startedAt:10,finishedAt:20});return {...ref(prefix+'/summary.json'),process:ref(prefix+'/process.json')};};
  const inherited={phase1:[],phase2:[]};
  const p1=e.read(path.join(root,'tools/phase1/required-cases.json'));
  const p1Results=selection.selected.phase1.map(id=>{
    const item=p1.cases.find(row=>row.id===id),prefix='.phase1/runs/control/'+id;
    write(prefix+'.log','CONTROL');write(prefix+'.json',{results:{tool:{version:'4.0.2'},summary:{tests:1,passed:1,failed:0},tests:[{name:item.test,status:'passed',labels:{Case:id}}]}});
    return {id,status:'pass',evidence:[{path:id+'.log',sha256:ref(prefix+'.log').sha256}],report:{path:id+'.json',sha256:ref(prefix+'.json').sha256}};
  });
  inherited.phase1.push(record('phase1',{...identity,revision:tested,nonce:'p1',status:'pass',input:baseline.snapshot(root,p1).sha256,selected:selection.selected.phase1,results:p1Results}));
  const p2Manifest={inputs:['src','tests/phase1','tools/phase2','tools/phase1/toolchain.json','Directory.Build.props','Directory.Build.targets','Directory.Packages.props','.editorconfig','BannedSymbols.txt','BannedSymbols.Pure.txt','docs/code']};
  const p2Identity={...identity,input:baseline.snapshot(root,p2Manifest).sha256,revision:tested,nonce:'p2'};
  inherited.phase2.push(record('phase2',{schemaVersion:2,...p2Identity,status:'pass',startedAt:10,finishedAt:20,failures:[],selected:selection.selected.phase2,results:selection.selected.phase2.map(id=>({...p2Identity,id,status:'pass'}))}));
  write('.phase3/runs/control/final/a.log','CONTROL');write('.phase3/runs/control/final/b.log','CONTROL');
  const artifacts=['a.log','b.log'].map(file=>({path:file,sha256:ref('.phase3/runs/control/final/'+file).sha256}));
  const final={schemaVersion:2,...identity,revision:tested,nonce:'final',status:'pass',startedAt:1,finishedAt:30,selected:selection.selected.phase3,
    results:cases.map(row=>({...identity,revision:tested,nonce:'final',id:row.id,status:'pass',artifacts})),prelaunchReview:gate.review,selectionSha256:gate.selection.sha256,inherited};
  write('.phase3/runs/control/final/summary.json',final);
  write('.planning/phases/03-four-player-lan-foundation/03-VISUAL-REVIEW.md','CONTROL: no rendered credit');
  const index={schemaVersion:1,scope:'Windows 1280x720 local; no VM',selection:gate.selection,finalReport:ref('.phase3/runs/control/final/summary.json'),history:[],reviews:[],visualReview:ref('.planning/phases/03-four-player-lan-foundation/03-VISUAL-REVIEW.md')};
  write('.planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json',index);
  write(catalog.reviewPath.replace('.json','.md'),'Later canonical review metadata; immutable original still bound.');
  const closure=commit();return {root,write,git,commit,reviewed,tested,closure,context,review,gate,identity,index,cases,ref};
}
test('committed metadata closure validates original review and completion without rewriting bytes',()=>{
  const f=fixture(),original=fs.readFileSync(path.join(f.root,f.index.finalReport.path)),review=fs.readFileSync(path.join(f.root,f.gate.review.report.path));
  const current={...f.identity,revision:f.closure};
  assert.equal(e.verifyCompletion(f.root,f.index,f.cases,current).testedRevision,f.tested);
  assert.equal(r.relation(f.root,f.reviewed,f.closure,f.identity.input).execution,'original');
  assert.deepEqual(fs.readFileSync(path.join(f.root,f.index.finalReport.path)),original);
  assert.deepEqual(fs.readFileSync(path.join(f.root,f.gate.review.report.path)),review);
  for(const key of ['input','tool','platform','osFingerprint'])assert.throws(()=>e.verifyCompletion(f.root,f.index,f.cases,{...current,[key]:'changed'}));
  for(const revision of ['unknown','0'.repeat(40),f.closure.slice(0,8)])assert.throws(()=>e.verifyCompletion(f.root,f.index,f.cases,{...current,revision}));
  assert.throws(()=>r.relation(f.root,f.closure,f.tested,f.identity.input));
  const unrelated=f.git('commit-tree',f.git('rev-parse','HEAD^{tree}'),'-m','unrelated');
  assert.throws(()=>r.relation(f.root,unrelated,f.closure,f.identity.input));
  for(const file of ['src/control.txt','tools/phase3/control.txt',catalog.selectionPath]){
    const bytes=fs.readFileSync(path.join(f.root,file));f.write(file,'changed');
    assert.throws(()=>e.verifyCompletion(f.root,f.index,f.cases,current));
    fs.writeFileSync(path.join(f.root,file),bytes);
  }
  for(const file of ['src/control.txt','tools/phase3/control.txt',catalog.selectionPath]){
    f.git('checkout','--detach',f.closure);fs.appendFileSync(path.join(f.root,file),'\n');const changed=f.commit();
    assert.throws(()=>r.relation(f.root,f.tested,changed,f.identity.input),/REVISION_NON_METADATA/);
    assert.throws(()=>catalog.prelaunch(f.root,{...f.context,revision:changed}));
  }
  f.git('checkout','--detach',f.closure);
  f.write('unapproved.txt','changed');const disallowed=f.commit();
  assert.throws(()=>r.relation(f.root,f.tested,disallowed,f.identity.input),/REVISION_NON_METADATA/);
  f.git('revert','--no-edit',disallowed);assert.throws(()=>r.relation(f.root,f.tested,f.git('rev-parse','HEAD'),f.identity.input),/REVISION_NON_METADATA/);
  const originalPath=process.env.PATH;try{process.env.PATH='';assert.throws(()=>r.relation(f.root,f.tested,f.closure,f.identity.input),/GIT_VALIDATION_UNAVAILABLE/);}finally{process.env.PATH=originalPath;}
});
test('prelaunch requires committed matching review inputs and exact original report hash',()=>{
  const f=fixture();f.git('checkout','--detach',f.tested);
  const context={...f.context,directory:path.join(f.root,'.phase3/other')};fs.mkdirSync(context.directory);
  catalog.prelaunch(f.root,context);
  const file=catalog.reviewPath;f.write(file,{...f.review,revision:'0'.repeat(40)});assert.throws(()=>catalog.prelaunch(f.root,context));
  f.git('checkout','--',file);f.write(f.review.report.path,'tampered');assert.throws(()=>catalog.prelaunch(f.root,context));
  f.git('checkout','--',f.review.report.path);f.write('src/control.txt','dirty');assert.throws(()=>catalog.prelaunch(f.root,context));
});
