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
module.exports={snapshot,subject,failure,invalid,reference,sharedSubject,provenance,definition};
