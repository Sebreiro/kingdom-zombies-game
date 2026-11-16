'use strict';
const fs=require('node:fs'),path=require('node:path');
const {isDeepStrictEqual:equal}=require('node:util');
const content=require('../content.cjs'),runner=require('../check.cjs');
const {ROOT,INDEX,EXPECTED,hash,need,inside,bytes,document,expected,validate,write,publish,readPublished}=content;
function snapshot(){return Object.fromEntries([INDEX,EXPECTED,'tools/phase4/content.cjs','tools/phase4/cases/provenance.cjs',
  'tools/phase4/check.cjs','src/godot-game/content/packs/tracer.json','src/godot-game/assets/common/bow.png',
  'production/phase4/tracer-source.json','src/godot-game/inspection/Inspection.tscn'].map(name=>[name,hash(bytes(ROOT,name))]));}
function subject(context,name){
  const root=inside(context.directory,name);fs.mkdirSync(root);const index=document(ROOT,INDEX),proof=validate(ROOT,index);
  for(const [file,value]of proof.files)write(root,file,value);write(root,INDEX,index);return {root,index};
}
function failure(context,name,code,action){
  let caught;try{action();}catch(error){caught=error;}
  write(context.directory,name+'.json',{caseId:context.caseId,nonce:context.nonce,subjectStatus:caught?'fail':'pass',
    expectedCode:code,diagnostic:caught?.message??null,expectedRejectionObserved:!!caught?.message.includes(code)});
  need(caught?.message.includes(code),'CONTROL_WRONG_RESULT',name+' '+caught?.message);return {name,code,subjectStatus:'fail'};
}
function invalid(context,name,code,edit){
  const copy=subject(context,name),candidate=structuredClone(copy.index);edit(candidate,copy.root);
  const result=validate(copy.root,candidate);const failed=result.diagnostics.find(x=>x.code==='OUTPUT_INVALID'&&x.message.includes(code));
  write(context.directory,name+'-outcome.json',{nonce:context.nonce,subjectStatus:'fail',expectedCode:code,result:{...result,files:undefined}});
  need(failed,'CONTROL_WRONG_RESULT',name+' '+JSON.stringify(result.diagnostics));return {name,code,subjectStatus:'fail'};
}
function reference(root,name){const value=bytes(root,name);return {path:name,bytes:value.length,sha256:hash(value)};}
function sharedSubject(context){
  const copy=subject(context,'shared'),other=structuredClone(copy.index.outputs[0]);
  other.id='heldout.second-bow';other.name='Independent shared obligation';other.sharedFileId='shared.bow-pixels';
  other.obligations=[{id:'FX-01',coverage:'partial'}];copy.index.outputs[0].sharedFileId=other.sharedFileId;
  const production=document(copy.root,other.production.path);production.id=other.id;
  write(copy.root,'production/phase4/heldout-second.json',production);other.production=reference(copy.root,'production/phase4/heldout-second.json');
  copy.index.outputs.push(other);const shard=copy.index.shards[0],body=document(copy.root,shard.path),binding=structuredClone(body.resources[0]);
  binding.id=other.id;body.resources.push(binding);fs.writeFileSync(inside(copy.root,shard.path),JSON.stringify(body));
  Object.assign(shard,reference(copy.root,shard.path));return copy;
}
function provenance(context){
  const before=snapshot(),register=expected(),index=document(ROOT,INDEX),production=validate(ROOT,structuredClone(index));runner.validate(ROOT);
  need(register.obligations.length===180&&register.obligations.some(x=>x.id==='TXT-01')&&register.obligations.some(x=>x.id==='TXT-02'),'CENSUS_REQUIRED');
  need(production.valid.length===1&&production.covered.length===0&&!production.complete&&production.diagnostics.filter(x=>x.code==='OBLIGATION_INCOMPLETE').length===180,'PARTIAL_PACK_REQUIRED');
  write(context.directory,'inputs.json',before);write(context.directory,'pack-diagnostics.json',{...production,files:undefined});const controls=[];
  const donors=[
    {owner:'heldout-a.assets',type:'Sprite',objectId:48,name:'same_name',ownerSha256:'a'.repeat(64),rawSha256:'1'.repeat(64)},
    {owner:'heldout-b.assets',type:'Sprite',objectId:48,name:'same_name',ownerSha256:'b'.repeat(64),rawSha256:'2'.repeat(64)},
    {owner:'heldout-a.assets',type:'Texture2D',objectId:49,name:'same_name',ownerSha256:'a'.repeat(64),rawSha256:'3'.repeat(64)}];
  for(const selected of donors)need(equal(content.resolveOrigin([...donors].reverse(),selected),selected),'H01_ORIGIN_RESOLUTION');
  controls.push(failure(context,'h01-wrong-owner','ORIGIN_NOT_EXACT',()=>content.resolveOrigin(donors,{...donors[0],owner:'heldout-b.assets'})));
  const shared=sharedSubject(context),sharedProof=validate(shared.root,structuredClone(shared.index));
  need(sharedProof.valid.length===2&&sharedProof.valid[0].id!==sharedProof.valid[1].id&&sharedProof.valid[1].obligations[0].id==='FX-01','H01_SHARED_IDENTITY');
  const notShared=structuredClone(shared.index);delete notShared.outputs[1].sharedFileId;
  need(validate(shared.root,notShared).diagnostics.some(x=>x.message?.includes('SHARED_OUTPUT_NOT_EXPLICIT')),'H01_IMPLICIT_SHARING');
  for(const [name,code,edit]of [
    ['missing-origin','ORIGIN_REQUIRED',x=>{x.outputs[0].origins=[];}],
    ['missing-version','VERSION_REQUIRED',x=>{delete x.outputs[0].version;}],
    ['missing-recipe','FILE_REFERENCE_REQUIRED',x=>{delete x.outputs[0].production;}],
    ['missing-hash','HASH_REQUIRED',x=>{delete x.outputs[0].sha256;}],
    ['missing-dependencies','DEPENDENCIES_REQUIRED',x=>{delete x.outputs[0].dependencies;}],
    ['missing-source-dependencies','SOURCE_DEPENDENCY_MISSING',x=>{x.origins[0].dependencies=[];x.outputs[0].origins[0].dependencies=[];}],
    ['missing-source-stream','SOURCE_STREAM_MISSING',x=>{x.origins[0].streams=[];x.outputs[0].origins[0].streams=[];}],
    ['missing-evidence','EVIDENCE_REQUIRED',x=>{delete x.outputs[0].evidence;}],
    ['false-source-evidence','EVIDENCE_MISSING',x=>{x.outputs[0].evidence.source='verified';}],
    ['borrowed-source-evidence','EVIDENCE_BINDING',(x,root)=>{
      write(root,'stale.json',{status:'pass',nonce:'old-run',caseId:'source.provenance',outputs:[{id:'common.bow',version:1,sha256:'0'.repeat(64)}]});
      x.outputs[0].evidence.source='verified';x.outputs[0].evidence.records=[{kind:'source',caseId:'source.provenance',invocation:'current-run',...reference(root,'stale.json')}];}],
    ['missing-file','ENOENT',(_,root)=>fs.unlinkSync(inside(root,'src/godot-game/assets/common/bow.png'))],
    ['hash-drift','FILE_HASH_MISMATCH',(_,root)=>fs.writeFileSync(inside(root,'src/godot-game/assets/common/bow.png'),Buffer.from('changed'))],
    ['path-escape','PATH_INVALID',x=>{x.outputs[0].path='../bow.png';}],
    ['path-case','PATH_CASE',x=>{x.outputs[0].path='src/godot-game/assets/common/Bow.png';}],
    ['id-case','OUTPUT_ID_DUPLICATE_OR_INVALID',x=>{x.outputs[0].id='Common.bow';}],
    ['id-nfd','IDENTITY_INVALID',x=>{x.outputs[0].id='bow.e\u0301';}]
  ])controls.push(invalid(context,'h02-h03-'+name,code,edit));
  controls.push(failure(context,'h02-false-pack-completion','FALSE_COMPLETION',()=>validate(ROOT,{...structuredClone(index),status:'complete'})));
  controls.push(failure(context,'h03-invalid-utf8','encoded data',()=>content.parse(Buffer.from([0xff]),'bad-utf8')));
  controls.push(failure(context,'h03-duplicate-json-key','JSON_DUPLICATE_KEY',()=>content.parse(Buffer.from('{"id":1,"id":2}'),'duplicate')));
  controls.push(failure(context,'h03-case-alias-source','ORIGIN_NOT_EXACT',()=>content.resolveOrigin(donors,{...donors[0],owner:'Heldout-a.assets'})));
  const shuffled=structuredClone(shared.index);shuffled.outputs.reverse();shuffled.shards.reverse();shuffled.origins.reverse();
  need(equal(validate(shared.root,shuffled).valid.sort((a,b)=>a.id<b.id?-1:1),sharedProof.valid.sort((a,b)=>a.id<b.id?-1:1)),'H04_ENUMERATION');
  const publicationRoot=path.join(context.directory,'publication'),first=publish({sourceRoot:ROOT,index,publicationRoot,invocation:context.nonce+'-first'});
  const pointer=hash(bytes(publicationRoot,'current.json'));
  controls.push(failure(context,'h05-interrupted-before-validation','HELDOUT_INTERRUPT',()=>publish({sourceRoot:ROOT,index,publicationRoot,invocation:context.nonce+'-interrupt',observe:step=>{if(step==='staged')throw new Error('HELDOUT_INTERRUPT');}})));
  need(hash(bytes(publicationRoot,'current.json'))===pointer,'H05_POINTER_CHANGED');
  let competing=false;publish({sourceRoot:ROOT,index,publicationRoot,invocation:context.nonce+'-lock',observe:step=>{if(step==='locked'){
    controls.push(failure(context,'h05-competing-lock','PUBLICATION_LOCKED',()=>publish({sourceRoot:ROOT,index,publicationRoot,invocation:context.nonce+'-competitor'})));competing=true;}}});
  need(competing,'H05_COMPETITOR_NOT_RUN');const replay=publish({sourceRoot:ROOT,index,publicationRoot,invocation:context.nonce+'-replay'});
  need(equal(first.outputs,replay.outputs)&&readPublished(publicationRoot).index.outputs.length===1,'H33_REPLAY_IDENTITIES');
  const merge1=content.merge(index,{outputs:index.outputs,origins:index.origins,shards:index.shards});
  need(merge1.outputs.length===1&&equal(content.merge(merge1,index),merge1),'H33_MERGE_IDEMPOTENCY');
  need(equal(snapshot(),before),'PRODUCTION_CHANGED');write(context.directory,'inputs-after.json',snapshot());
  return {expected:180,validOutputs:production.valid.length,completedObligations:0,status:'incomplete',controls,
    heldout:{H01:'pass-source',H02:'pass-source',H03:'pass-source',H04:'pass-source',H05:'pass-source-basic',H33:'pass-source-basic'},
    finalNativeListeningJudgment:'pending-04-32/04-33',originals:'No authoring donor access; tracked originals/source bundle unchanged'};
}
const definition={group:'source',title:'Exact independent census, provenance and staged publication',importance:'Critical',
  rationale:'False identity/coverage or publication can corrupt production provenance',needsGameWindow:false,runGroup:'G0',order:3,
  requirements:['ASSET-01','ASSET-02','ASSET-05'],acceptance:['AC-01','AC-02','AC-03','AC-16','AC-19','AC-22'],
  setup:'Tracked partial bow shard/source and independent 180-ID register; fresh owned evidence directory; no donor root',
  actions:['Validate actual selected bytes, dependencies and exact origins','Run independently selected duplicate-owner/type and shared-output controls',
    'Reject missing provenance/evidence and ordinal/UTF-8/path errors','Shuffle enumeration','Interrupt and compete before commit','Repeat publication without duplicate stable identities'],
  expected:'180 expected, zero completed obligations, one usable partial output; every deliberate bad subject fails; unaffected output remains inspectable',
  reset:'Run-owned copies and generations only; no game/audio; preserve failure evidence and originals',
  evidence:['inputs.json','pack-diagnostics.json','h01-wrong-owner.json','h05-interrupted-before-validation.json','h05-competing-lock.json','inputs-after.json','result.json']};
runner.registerCase('source.provenance',definition,provenance);
async function publication(context){
  const before=snapshot(),index=document(ROOT,INDEX),publicationRoot=path.join(context.directory,'publication'),script=path.join(ROOT,'tools/phase4/content.cjs');
  write(context.directory,'inputs.json',before);const baseline=publish({sourceRoot:ROOT,index,publicationRoot,invocation:context.nonce+'-baseline'});
  const observations=[];let observedCompetitor=false;
  for(const step of ['locked','staged','validated','before-commit']){
    const invocation=context.nonce+'-'+step,name='interrupted-'+step,pointer=hash(bytes(publicationRoot,'current.json'));let caught;
    try{await runner.command(context,name,process.execPath,[script,'--publish-worker',ROOT,publicationRoot,invocation,step],
      {timeout:10000,interruptMarker:'PUBLICATION_PAUSED '+invocation+' '+step});}catch(error){caught=error;}
    const command=document(context.directory,name+'.command.json');
    need(caught?.message.includes('LOADER_INTERRUPTED')&&command.reason==='LOADER_INTERRUPTED'&&command.streamsClosed&&
      !command.cleanupError&&command.stdout.includes('PUBLICATION_PAUSED '+invocation+' '+step),'H05_ACTUAL_INTERRUPTION',step);
    need(hash(bytes(publicationRoot,'current.json'))===pointer&&readPublished(publicationRoot).pointer.invocation===baseline.invocation,'H05_PARTIAL_PUBLICATION',step);
    failure(context,name+'-retry','PUBLICATION_LOCKED',()=>publish({sourceRoot:ROOT,index,publicationRoot,invocation:context.nonce+'-retry-'+step}));
    if(step==='locked'){
      let competitor;try{await runner.command(context,'competitor',process.execPath,[script,'--publish-worker',ROOT,publicationRoot,context.nonce+'-competitor','none'],{timeout:10000});}catch(error){competitor=error;}
      const record=document(context.directory,'competitor.command.json');
      need(competitor&&record.code===1&&record.stderr.includes('PUBLICATION_LOCKED')&&record.streamsClosed&&!record.cleanupError,'H05_PROCESS_COMPETITION');
      write(context.directory,'competing-subject.json',{nonce:context.nonce,subjectStatus:'fail',diagnostic:'PUBLICATION_LOCKED',pid:record.pid});observedCompetitor=true;
    }
    failure(context,name+'-wrong-recovery','RECOVERY_OWNER',()=>content.recoverPublication({publicationRoot,invocation:'wrong-owner'}));
    const recovered=content.recoverPublication({publicationRoot,invocation});
    need(!recovered.published&&!fs.existsSync(inside(publicationRoot,'publication.lock'))&&hash(bytes(publicationRoot,'current.json'))===pointer,'H05_RECOVERY');
    need(!fs.existsSync(inside(publicationRoot,'stage-'+invocation))&&!fs.existsSync(inside(publicationRoot,'generations/'+invocation)),'H05_ORPHAN_CLEANUP');
    write(context.directory,name+'-result.json',{nonce:invocation,caseId:'heldout.H05',status:'interrupted',verified:false,command:name+'.command.json',recovered});
    observations.push({step,invocation,status:'interrupted',verified:false,pointerUnchanged:true,recovered});
  }
  need(observedCompetitor,'H05_COMPETITOR_NOT_RUN');
  await runner.command(context,'live-competition',process.execPath,[script,'--publish-worker',ROOT,publicationRoot,context.nonce+'-alive','compete'],{timeout:10000});
  const liveCompetition=document(publicationRoot,'live-competition.json');
  need(liveCompetition.subjectStatus==='fail'&&liveCompetition.observed==='PUBLICATION_LOCKED while owner is alive','H05_LIVE_COMPETITION');
  let liveRecovery=false;publish({sourceRoot:ROOT,index,publicationRoot,invocation:context.nonce+'-live',observe:step=>{
    if(step==='locked'){failure(context,'live-owner-recovery','RECOVERY_OWNER_ALIVE',()=>content.recoverPublication({publicationRoot,invocation:context.nonce+'-live'}));liveRecovery=true;}}});
  need(liveRecovery,'RECOVERY_LIVE_NOT_RUN');
  const pointer=hash(bytes(publicationRoot,'current.json'));
  const empty={...structuredClone(index),outputs:[]};
  failure(context,'no-regression-to-empty','PUBLICATION_REGRESSION',()=>publish({sourceRoot:ROOT,index:empty,publicationRoot,invocation:context.nonce+'-empty'}));
  const altered=structuredClone(index);altered.outputs[0].metadata.note='changed stable version';
  failure(context,'no-overwrite-stable-identity','IDENTITY_IMMUTABLE',()=>publish({sourceRoot:ROOT,index:altered,publicationRoot,invocation:context.nonce+'-changed'}));
  need(hash(bytes(publicationRoot,'current.json'))===pointer,'FAILED_RUN_INHERITED_PUBLICATION');
  const broken=subject(context,'broken');fs.unlinkSync(inside(broken.root,'src/godot-game/assets/common/bow.png'));
  failure(context,'metadata-only-cannot-publish','PUBLICATION_INVALID_OUTPUT',()=>publish({sourceRoot:broken.root,index:broken.index,publicationRoot,invocation:context.nonce+'-missing'}));
  need(hash(bytes(publicationRoot,'current.json'))===pointer,'MISSING_BYTES_PUBLISHED');
  failure(context,'original-root-denied','PUBLICATION_ROOT_DENIED',()=>content.destination('D:/Work/Games/KindomCrown/recovered'));
  failure(context,'root-escape-denied','PUBLICATION_ROOT_DENIED',()=>content.destination(path.join(ROOT,'../outside')));
  failure(context,'encoding-escape-denied','PATH_INVALID',()=>content.inside(ROOT,'.phase4/%2e%2e/outside'));
  failure(context,'windows-device-denied','PATH_INVALID',()=>content.inside(ROOT,'.phase4/con.txt'));
  const junction=inside(context.directory,'reparse');fs.symlinkSync(publicationRoot,junction,process.platform==='win32'?'junction':'dir');
  try{failure(context,'reparse-denied','PATH_REPARSE',()=>content.destination(junction));}finally{fs.unlinkSync(junction);}
  const runs=[];
  for(let i=0;i<3;i++){
    const shuffled=structuredClone(index);if(i%2)shuffled.shards.reverse();
    const fresh=publish({sourceRoot:ROOT,index:shuffled,publicationRoot,invocation:context.nonce+'-repeat-'+i});
    need(equal(fresh.outputs,baseline.outputs),'H33_EXPORT_IDENTITIES');const imported=readPublished(publicationRoot);
    need(imported.index.outputs.length===1&&imported.result.valid.length===1&&!imported.result.complete,'H33_IMPORT_IDENTITIES');runs.push(fresh);
  }
  const combined=sharedSubject(context);combined.index.shards.reverse();combined.index.outputs.reverse();
  const compositionRoot=path.join(context.directory,'composition');
  publish({sourceRoot:combined.root,index:combined.index,publicationRoot:compositionRoot,invocation:context.nonce+'-shared'});
  const imported=readPublished(compositionRoot);need(imported.result.valid.length===2&&imported.result.covered.length===0,'H33_SHARED_COMPOSITION');
  const forward=content.merge(index,{outputs:[],origins:[],shards:[{...index.shards.find(x=>x.status==='pending'),status:'selected',version:1,bytes:15,sha256:'a'.repeat(64)}]});
  need(forward.shards.filter(x=>x.status==='selected').length===2,'PENDING_SHARD_TRANSITION');
  need(equal(snapshot(),before),'PRODUCTION_CHANGED');write(context.directory,'inputs-after.json',snapshot());
  write(context.directory,'publication-trials.json',{observations,runs,unchangedOriginals:true});
  return {heldout:{H05:'pass-source-actual-process-interruption-and-contention',H33:'pass-source-repeat-export-import'},
    observations,runs,liveCompetition,finalNativeListeningJudgment:'pending-04-32/04-33',wholePack:'incomplete'};
}
runner.registerCase('source.publication',{...definition,title:'Actual interrupted, competing and repeated publication',order:4,
  acceptance:['AC-02','AC-03','AC-16','AC-19','AC-22'],
  actions:['Publish actual tracked bow to a validated immutable generation','Interrupt owned worker at four observed stages',
    'Run competing separate process against its exclusive lock','Recover only dead exact invocation ownership',
    'Reject regressed, stale, missing-byte, reparse and escaped publications','Repeat export/import and compose explicit shared identities'],
  expected:'Four actual interrupted failed workers and actual locked competitor preserve current validated output; fresh repeated runs preserve one stable selected identity; no whole-pack completion',
  evidence:['inputs.json','interrupted-locked.command.json','interrupted-staged.command.json','interrupted-validated.command.json',
    'interrupted-before-commit.command.json','competitor.command.json','publication-trials.json','inputs-after.json','result.json']},publication);
module.exports={snapshot,subject,failure,invalid,reference,sharedSubject,provenance,definition,publication};
