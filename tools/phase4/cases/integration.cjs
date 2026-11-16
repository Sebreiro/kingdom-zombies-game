'use strict';
const fs=require('node:fs'),path=require('node:path'),assert=require('node:assert/strict');
const {spawnSync}=require('node:child_process');
const runner=require('../check.cjs'),content=require('../content.cjs');
const {ROOT,need,write,bytes,hash}=content;
const SCRIPT=path.join(ROOT,'tools/phase4/check.cjs');
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
    for(const [file,value]of Object.entries(files))write(directory,file,value);return directory;};
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
function selfTest(){
  const {test}=require('node:test');
  test('actual CLI discovers integration saved cases',()=>{
    const child=spawnSync(process.execPath,[SCRIPT,'--list'],{cwd:ROOT,windowsHide:true,encoding:'utf8',timeout:10000});
    assert.equal(child.status,0,child.stderr);
    assert.ok(JSON.parse(child.stdout).some(x=>x.id==='source.case-discovery'),'actual CLI must discover integration.cjs');
  });
}
module.exports={ORIGINAL,definition,cli,discovery,selfTest};
if(require.main===module&&process.argv[2]==='--self-test')selfTest();
