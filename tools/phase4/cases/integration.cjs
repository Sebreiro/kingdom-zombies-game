'use strict';
const fs=require('node:fs'),path=require('node:path'),assert=require('node:assert/strict');
const {spawnSync}=require('node:child_process');
const runner=require('../check.cjs'),content=require('../content.cjs');
const {ROOT,need,write,bytes,hash}=content;
const SCRIPT=path.join(ROOT,'tools/phase4/check.cjs');
const PACKAGED='src/godot-game/content/expected-obligations.json';
const EXPECTED_SHA256='a4d9243bda1b512b59c189a3bdc48a7cccc8171a218bbfc5ae16b00274f2fda5';
const ORIGINAL=[['source.tracer','G0',1],['source.tracer-failures','G0',2],['source.provenance','G0',3],
  ['source.publication','G0',4],['audio.feasibility','G0',6],['export.layout','G0',0],
  ['export.profile','G0-export-profile',350],['export.profile-boundaries','G0-export-profile',351],['export.feasibility','G0',5]];
const definition={group:'source',title:'Strict bounded saved case discovery through the actual CLI',importance:'Critical',
  rationale:'Missing or ambiguous executable scenarios invalidate verification selection',needsGameWindow:false,
  runGroup:'G0-integration',order:360,requirements:['ASSET-02','ASSET-05'],acceptance:[],
  setup:'Tracked canonical CommonJS cases and fresh owned real fixture modules; no game window',
  actions:['List the actual CLI catalog and reject unknown selections','Load real fixtures without running case bodies',
    'Reverse directory enumeration','Reject each independent invalid module and registration'],
  expected:'Stable complete catalog; canonical future module discovered; exact invalid inputs fail visibly',
  reset:'Fresh run-owned fixture directories and isolated registries; no production fixture edits',
  evidence:['inputs.json','cli-list.command.json','unknown-case.command.json','unknown-group.command.json','controls.json','result.json']};
async function cli(context,name,args,expectedCode){
  let error,record;try{record=await runner.command(context,name,process.execPath,[SCRIPT,...args],{timeout:10000});}catch(caught){error=caught;}
  record??=content.document(context.directory,name+'.command.json');
  if(expectedCode)need(error&&record.code===1&&record.streamsClosed&&!record.reason&&!record.cleanupError&&
    record.stderr.includes(expectedCode),'CLI_REJECTION_REQUIRED',name+' '+record.stderr);
  else need(!error,'CLI_SUCCESS_REQUIRED',name+' '+error?.message);
  return record;
}
async function discovery(context){
  const listing=await cli(context,'cli-list',['--list']),catalog=JSON.parse(listing.stdout);
  need(catalog.some(x=>x.id==='source.case-discovery'),'CASE_DISCOVERY_MISSING','integration.cjs');
  for(const [id,runGroup,order]of ORIGINAL)need(catalog.some(x=>x.id===id&&x.runGroup===runGroup&&x.order===order),'ORIGINAL_CASE_CHANGED',id);
  await cli(context,'unknown-case',['--case','unknown.case'],'CASE_PENDING_OR_UNKNOWN');
  await cli(context,'unknown-group',['--group','unknown'],'CASE_PENDING_OR_UNKNOWN');
  await cli(context,'empty-group',['--group',''],'CASE_PENDING_OR_UNKNOWN');
  const helper=require('../case-discovery.cjs'),controls=[];
  const inputs=Object.fromEntries(['tools/phase4/check.cjs','tools/phase4/case-discovery.cjs',
    'tools/phase4/cases/integration.cjs'].map(file=>[file,hash(bytes(ROOT,file))]));write(context.directory,'inputs.json',inputs);
  function reject(name,code,action){
    let caught;try{action();}catch(error){caught=error;}
    const row={name,expectedCode:code,subjectStatus:caught?'fail':'pass',diagnostic:caught?.message??null};
    write(context.directory,name+'.json',row);need(caught?.message.startsWith(code),'DISCOVERY_CONTROL',name+' '+caught?.message);controls.push(row);
  }
  const meta={...definition,runGroup:'G-fixture',order:1};
  const registry=()=>helper.createCaseRegistry();
  const fixture=(name,files)=>{const directory=path.join(context.directory,name);fs.mkdirSync(directory);
    for(const [file,value]of Object.entries(files))fs.writeFileSync(path.join(directory,file),value,{flag:'wx'});return directory;};
  const body=(id,order)=>`module.exports=register=>register(${JSON.stringify(id)},${JSON.stringify({...meta,order})},()=>{throw new Error('CASE_BODY_EXECUTED')});`;
  const valid=fixture('valid',{'zeta.cjs':body('fixture.zeta',2),'alpha.cjs':body('fixture.alpha',1)});
  const load=(file,r)=>require(file)(r.registerCase);
  const first=registry(),descriptors=helper.loadCaseModules(valid,first,load);
  need(first.cases.size===2&&descriptors.map(x=>x.name).join(',')==='alpha.cjs,zeta.cjs'&&
    descriptors.every(x=>x.sha256===hash(fs.readFileSync(x.path))&&x.caseIds.length===1),'FIXTURE_DISCOVERY');
  const originalRead=fs.readdirSync;let reversed;
  try{fs.readdirSync=function(...args){const result=originalRead.apply(this,args);return path.resolve(args[0])===valid?result.reverse():result;};
    reversed=helper.loadCaseModules(valid,registry(),load);}finally{fs.readdirSync=originalRead;}
  assert.deepEqual(reversed,descriptors);
  reject('missing-directory','CASE_DIRECTORY_MISSING',()=>helper.loadCaseModules(path.join(context.directory,'absent'),registry(),load));
  reject('empty-directory','CASE_MODULES_EMPTY',()=>helper.loadCaseModules(fixture('empty',{}),registry(),load));
  for(const name of ['Upper.cjs','wrong.js','two..cjs',' space.cjs'])reject('filename-'+controls.length,'CASE_MODULE_NAME',
    ()=>helper.loadCaseModules(fixture('bad-name-'+controls.length,{[name]:'module.exports=()=>{}'}),registry(),load));
  const nested=fixture('nested',{});fs.mkdirSync(path.join(nested,'folder.cjs'));
  reject('directory-entry','CASE_MODULE_FILE',()=>helper.loadCaseModules(nested,registry(),load));
  reject('zero-registration','CASE_MODULE_EMPTY',()=>helper.loadCaseModules(fixture('zero',{'valid.cjs':body('fixture.valid',1),'zero.cjs':'module.exports=()=>{}'}),registry(),load));
  reject('throwing-module','CASE_MODULE_LOAD',()=>helper.loadCaseModules(fixture('throwing',{'throw.cjs':"throw new Error('OWNED_LOAD_FAILURE')"}),registry(),load));
  const many={};for(let i=0;i<129;i++)many['case-'+i+'.cjs']=body('fixture.case-'+i,i);
  reject('too-many','CASE_MODULE_LIMIT',()=>helper.loadCaseModules(fixture('many',many),registry(),load));
  const linked=path.join(context.directory,'linked');fs.symlinkSync(valid,linked,process.platform==='win32'?'junction':'dir');
  try{reject('reparse-directory','CASE_PATH_REPARSE',()=>helper.loadCaseModules(linked,registry(),load));}finally{fs.unlinkSync(linked);}
  const hard=fixture('hardlink',{});fs.linkSync(path.join(valid,'alpha.cjs'),path.join(hard,'alias.cjs'));
  try{reject('file-alias','CASE_MODULE_FILE',()=>helper.loadCaseModules(hard,registry(),load));}finally{fs.unlinkSync(path.join(hard,'alias.cjs'));}
  const duplicate=registry();duplicate.registerCase('fixture.same',meta,()=>{});
  reject('duplicate-id','CASE_ID_DUPLICATE',()=>duplicate.registerCase('fixture.same',{...meta,order:2},()=>{}));
  reject('duplicate-position','CASE_ORDER_DUPLICATE',()=>duplicate.registerCase('fixture.other',meta,()=>{}));
  for(const [name,edit]of [
    ['title',x=>{x.title='';}],['rationale',x=>{delete x.rationale;}],['setup',x=>{x.setup=' ';}],
    ['expected',x=>{x.expected='';}],['reset',x=>{x.reset='';}],['actions',x=>{x.actions=[];}],
    ['evidence',x=>{x.evidence=[''];}],['importance',x=>{x.importance='urgent';}],
    ['window',x=>{x.needsGameWindow='false';}],['group',x=>{x.group='Bad Group';}],
    ['run-group',x=>{x.runGroup='../escape';}],['order',x=>{x.order=0.5;}],
    ['requirements',x=>{x.requirements=['ASSET-99'];}],['acceptance',x=>{x.acceptance=['AC-25'];}],
    ['reserved-id',x=>{x.id='fixture.override';}],['reserved-run',x=>{x.run=()=>{};}]
  ]){const item={...meta};edit(item);reject('metadata-'+name,'CASE_METADATA',()=>registry().registerCase('fixture.metadata',item,()=>{}));}
  reject('nonfunction','CASE_RUN',()=>registry().registerCase('fixture.run',meta,null));
  for(const id of ['Fixture.bad','fixture..bad','fixture.e\u0301',''])reject('invalid-id-'+controls.length,'CASE_ID_INVALID',()=>registry().registerCase(id,meta,()=>{}));
  write(context.directory,'controls.json',{controls,descriptors,catalog});
  return {catalog:catalog.map(({id,runGroup,order})=>({id,runGroup,order})),descriptors,expectedRejected:controls.length,
    realCliChildren:4,caseBodiesExecutedDuringImport:0};
}
runner.registerCase('source.case-discovery',definition,discovery);
async function packaged(context){
  const legacy=require('./provenance.cjs').subject(context,'packaged-legacy'),sourceRoot=legacy.root;
  const source=bytes(ROOT,content.EXPECTED),index=legacy.index,before=structuredClone(index);
  need(source.length===25345&&hash(source)===EXPECTED_SHA256,'SOURCE_AUTHORITY_DRIFT',content.EXPECTED);
  const checked=content.validate(sourceRoot,structuredClone(index));
  need(checked.files.has(PACKAGED)&&checked.files.get(PACKAGED).equals(source),'PACKAGED_EXPECTATIONS_MISSING',PACKAGED);
  const snapshot=()=>Object.fromEntries([content.EXPECTED,PACKAGED,content.INDEX,'tools/phase4/content.cjs',
    'tools/phase4/cases/integration.cjs','src/godot-game/assets/common/bow.png'].map(file=>[file,hash(bytes(ROOT,file))]));
  const initial=snapshot();write(context.directory,'inputs.json',initial);
  const partial=proof=>need(!proof.complete&&proof.expected===180&&proof.covered.length===0&&
    proof.diagnostics.filter(x=>x.code==='OBLIGATION_INCOMPLETE').length===180,'INDEPENDENT_EXPECTATIONS_LOST');
  partial(checked);need(checked.valid.length===1&&checked.valid[0].id==='common.bow'&&
    index.shards.filter(x=>x.status==='pending').length===28,'CURRENT_PARTIAL_STATE_CHANGED');
  const controls=[];
  function subject(name){const root=path.join(context.directory,name);fs.mkdirSync(root);
    for(const [file,value]of checked.files)write(root,file,value);return {root,index:structuredClone(index)};}
  function reject(name,edit){const copy=subject(name);edit(copy);let caught;
    try{content.validate(copy.root,copy.index,copy.register);}catch(error){caught=error;}
    const outcome={name,subjectStatus:caught?'fail':'pass',diagnostic:caught?.message??null,
      indexSha256:hash(Buffer.from(JSON.stringify(copy.index))),registerSha256:fs.existsSync(path.join(copy.root,PACKAGED))?
        hash(fs.readFileSync(path.join(copy.root,PACKAGED))):null};
    write(context.directory,name+'.json',outcome);
    need(caught?.message.startsWith('EXPECTED_PACKAGE_INVALID '+PACKAGED+' '),'EXPECTATION_CONTROL',name+' '+caught?.message);
    controls.push(outcome);
  }
  const replace=(copy,value,refresh=true)=>{fs.writeFileSync(path.join(copy.root,PACKAGED),value);
    if(refresh)copy.index.expectedObligations={path:PACKAGED,bytes:value.length,sha256:hash(value)};};
  for(const [name,edit]of [
    ['missing-reference',x=>{delete x.index.expectedObligations;}],
    ['reference-null',x=>{x.index.expectedObligations=null;}],
    ['reference-extra',x=>{x.index.expectedObligations.alias=PACKAGED;}],
    ['reference-case',x=>{x.index.expectedObligations.path=PACKAGED.replace('expected','Expected');}],
    ['reference-alias',x=>{x.index.expectedObligations.path='src/godot-game/content/./expected-obligations.json';}],
    ['reference-escape',x=>{x.index.expectedObligations.path='../expected-obligations.json';}],
    ['reference-absolute',x=>{x.index.expectedObligations.path=path.join(ROOT,PACKAGED);}],
    ['reference-bytes',x=>{x.index.expectedObligations.bytes++;}],
    ['reference-hash',x=>{x.index.expectedObligations.sha256='0'.repeat(64);}],
    ['reference-missing-hash',x=>{delete x.index.expectedObligations.sha256;}],
    ['reference-negative-size',x=>{x.index.expectedObligations.bytes=-1;}],
    ['missing-register',x=>{fs.unlinkSync(path.join(x.root,PACKAGED));}],
    ['truncated-register',x=>replace(x,source.subarray(0,100))],
    ['unrefreshed-tamper',x=>replace(x,Buffer.concat([source,Buffer.from(' ')]),false)],
    ['invalid-utf8',x=>replace(x,Buffer.from([0xff]))],
    ['bom',x=>replace(x,Buffer.concat([Buffer.from([0xef,0xbb,0xbf]),source]))],
    ['duplicate-json-key',x=>replace(x,Buffer.from(source.toString().replace('"schemaVersion": 1','"schemaVersion": 1, "schemaVersion": 1')))]
  ])reject(name,edit);
  for(const [name,edit]of [
    ['wrong-schema',x=>{x.schemaVersion=2;}],['wrong-authority',x=>{x.authority='output-derived';}],
    ['wrong-scope',x=>{x.scope='selected-only';}],['missing-row',x=>{x.obligations.pop();}],
    ['duplicate-row',x=>{x.obligations[1]=x.obligations[0];}],['unknown-id',x=>{x.obligations[0].id='CH-99';}],
    ['case-id',x=>{x.obligations[0].id='ch-01';}],['nfd-id',x=>{x.obligations[0].id='CH-e\u0301';}],
    ['changed-name',x=>{x.obligations[0].name+=' changed';}],['changed-source',x=>{x.obligations[0].source+=' changed';}],
    ['reordered-rows',x=>{x.obligations.reverse();}],['output-derived',x=>{x.obligations=x.obligations.slice(0,1);}]
  ])reject(name,copy=>{const register=JSON.parse(source);edit(register);replace(copy,Buffer.from(JSON.stringify(register,null,2)+'\n'));copy.index.outputs=[];});
  reject('caller-register',copy=>{copy.register=content.expected();copy.register.obligations.pop();});
  const linked=subject('reparse-register'),file=path.join(linked.root,PACKAGED),target=path.join(context.directory,'external-register');
  fs.writeFileSync(target,source);fs.unlinkSync(file);fs.linkSync(target,file);
  try{let caught;try{content.validate(linked.root,linked.index);}catch(error){caught=error;}
    need(caught?.message.startsWith('EXPECTED_PACKAGE_INVALID '+PACKAGED+' '),'EXPECTATION_ALIAS_CONTROL');
    controls.push({name:'hardlink-register',subjectStatus:'fail',diagnostic:caught.message});}finally{fs.unlinkSync(file);}
  const reparse=subject('reparse-parent'),parent=path.join(reparse.root,'src/godot-game/content'),held=path.join(reparse.root,'retained-content');
  fs.renameSync(parent,held);fs.symlinkSync(held,parent,process.platform==='win32'?'junction':'dir');
  try{let caught;try{content.validate(reparse.root,reparse.index);}catch(error){caught=error;}
    need(caught?.message.startsWith('EXPECTED_PACKAGE_INVALID '+PACKAGED+' ')&&caught.message.includes('PATH_REPARSE'),'EXPECTATION_REPARSE_CONTROL');
    controls.push({name:'reparse-parent',subjectStatus:'fail',diagnostic:caught.message});}finally{fs.unlinkSync(parent);}
  const missingMedia=subject('missing-media');fs.unlinkSync(path.join(missingMedia.root,index.outputs[0].path));
  const missingProof=content.validate(missingMedia.root,missingMedia.index);partial(missingProof);
  need(missingProof.valid.length===0&&missingProof.diagnostics.some(x=>x.code==='OUTPUT_INVALID'),'MISSING_MEDIA_NOT_REPORTED');
  const shared=require('./provenance.cjs').sharedSubject(context);fs.unlinkSync(path.join(shared.root,shared.index.outputs[0].production.path));
  const independent=content.validate(shared.root,shared.index);partial(independent);
  need(independent.valid.length===1&&independent.valid[0].id==='heldout.second-bow','INDEPENDENT_VALID_OUTPUT_HIDDEN');
  const publicationRoot=path.join(context.directory,'publication');
  const published=content.publish({sourceRoot,index:structuredClone(index),publicationRoot,invocation:context.nonce+'-register'});
  const imported=content.readPublished(publicationRoot),generation=path.join(publicationRoot,imported.pointer.generation);
  partial(imported.result);assert.deepEqual(imported.index.expectedObligations,index.expectedObligations);
  assert.deepEqual(imported.index.shards,index.shards);assert.deepEqual(imported.index.outputs,index.outputs);
  need(bytes(generation,PACKAGED).equals(source)&&!fs.existsSync(path.join(generation,content.EXPECTED)),'PUBLISHED_EXPECTATIONS_NOT_PORTABLE');
  const generationRegister=path.join(generation,PACKAGED);fs.writeFileSync(generationRegister,Buffer.concat([source,Buffer.from(' ')]));
  let publishedFailure;try{content.readPublished(publicationRoot);}catch(error){publishedFailure=error;}
  need(publishedFailure?.message.startsWith('EXPECTED_PACKAGE_INVALID '+PACKAGED+' '),'PUBLISHED_EXPECTATIONS_TAMPER_ACCEPTED');
  write(context.directory,'published-tamper.json',{subjectStatus:'fail',diagnostic:publishedFailure.message});
  controls.push({name:'published-tamper',subjectStatus:'fail',diagnostic:publishedFailure.message});
  fs.writeFileSync(generationRegister,source);partial(content.readPublished(publicationRoot).result);
  const restoredRoot=path.join(context.directory,'missing-register');write(restoredRoot,PACKAGED,source);
  const restoredCopy=content.validate(restoredRoot,structuredClone(index));partial(restoredCopy);
  need(restoredCopy.valid.length===1,'RESTORED_EXPECTATIONS_OUTPUT_LOST');
  const restored=content.validate(sourceRoot,structuredClone(index));partial(restored);assert.deepEqual(snapshot(),initial);assert.deepEqual(index,before);
  write(context.directory,'controls.json',controls);write(context.directory,'publication.json',{published,pointer:imported.pointer,
    indexSha256:hash(bytes(generation,content.INDEX)),registerSha256:hash(bytes(generation,PACKAGED)),
    rows:JSON.parse(bytes(generation,PACKAGED)).obligations,diagnostics:imported.result.diagnostics});
  const actual=content.normalizeCurrent(ROOT),currentRoot=path.join(context.directory,'current-publication'),current=await content.publish({sourceRoot:ROOT,index:actual,publicationRoot:currentRoot,invocation:context.nonce+'-current-register'}),currentRead=content.readPublished(currentRoot),currentGeneration=path.join(currentRoot,currentRead.pointer.generation);
  need(current.sourceComplete&&current.accepted===0&&currentRead.result.sourceReady.length===180&&!current.complete,'CURRENT_EXPECTATIONS_SOURCE_READINESS');
  assert.deepEqual(currentRead.result.sourceReady,content.expected().obligations.map(x=>x.id).sort());
  need(bytes(currentGeneration,PACKAGED).equals(source)&&!fs.existsSync(path.join(currentGeneration,content.EXPECTED)),'CURRENT_EXPECTATIONS_PORTABLE');
  const currentRegister=path.join(currentGeneration,PACKAGED);fs.writeFileSync(currentRegister,Buffer.concat([source,Buffer.from(' ')]));let actualFailure;try{content.readPublished(currentRoot);}catch(error){actualFailure=error;}
  need(actualFailure?.message.startsWith('EXPECTED_PACKAGE_INVALID '+PACKAGED+' '),'CURRENT_EXPECTATIONS_TAMPER_ACCEPTED');fs.writeFileSync(currentRegister,source);
  const currentRestored=content.readPublished(currentRoot);need(currentRestored.result.sourceComplete&&currentRestored.result.valid.length===180,'CURRENT_EXPECTATIONS_RESTORED');
  write(context.directory,'current-portable-expectations.json',{current,portablePackageSha256:hash(source),sourceOnlyRegisterAbsent:true,tamperRejected:actualFailure.message,restoredSourceIds:currentRestored.result.sourceReady,runtimePaths:actual.files.filter(x=>x.role==='runtime').length,accepted:0});
  return {current:{sourceReady:180,accepted:0,portable:true,tamperRejected:true,restored:true},expected:180,completedObligations:0,validOutputs:1,pendingShards:28,status:'incomplete',expectedRejected:controls.length,
    controls,published,restored:true,sourceAuthorityUnchanged:true,independentValidOutputRetained:true,
    finalNativeListeningJudgment:'pending-04-32/04-33'};
}
runner.registerCase('source.packaged-expectations',{...definition,title:'Independent expected register survives authenticated atomic publication',order:361,
  rationale:'A missing or output-derived expectation set can conceal missing required content',
  actions:['Authenticate all independent source and package bytes','Reject each isolated reference/header/row/alias/tamper control',
    'Publish and read an isolated generation with all 180 exact rows','Keep valid independent output and all missing obligations visible','Revalidate unchanged production'],
  expected:'Byte-identical 180 rows; one partial bow, 28 pending shards, zero completed obligations; every invalid expectation fails',
  evidence:['inputs.json','controls.json','publication.json','result.json']},packaged);
function selfTest(){
  const {test}=require('node:test');
  test('actual CLI discovers integration saved cases',()=>{
    const child=spawnSync(process.execPath,[SCRIPT,'--list'],{cwd:ROOT,windowsHide:true,encoding:'utf8',timeout:10000});
    assert.equal(child.status,0,child.stderr);
    assert.ok(JSON.parse(child.stdout).some(x=>x.id==='source.case-discovery'),'actual CLI must discover integration.cjs');
  });
}
function packagedSelfTest(){
  const {test}=require('node:test');
  test('independent expectations are authenticated packaged publication inputs',()=>{
    const result=content.validate(ROOT);
    assert.ok(result.files.has(PACKAGED),'validation must include the independent packaged register');
    assert.equal(hash(result.files.get(PACKAGED)),EXPECTED_SHA256);
  });
}
module.exports={ORIGINAL,definition,cli,discovery,packaged,selfTest,packagedSelfTest};
if(require.main===module&&process.argv[2]==='--self-test')selfTest();
if(require.main===module&&process.argv[2]==='--self-test-packaged')packagedSelfTest();
