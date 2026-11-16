'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {TextDecoder,isDeepStrictEqual:equal}=require('node:util');
const ROOT=path.resolve(__dirname,'../..'),INDEX='src/godot-game/content/pack-index.json';
const EXPECTED='tools/phase4/expected-obligations.json',MAX_FILE=32*1024*1024,MAX_TOTAL=256*1024*1024;
const hash=value=>crypto.createHash('sha256').update(value).digest('hex');
function need(ok,code,detail=''){if(!ok)throw new Error(code+' '+detail);}
function text(value,label){need(typeof value==='string'&&value.length>0&&value.length<=4096&&
  value===value.normalize('NFC')&&!/[\u0000-\u001f\ud800-\udfff]/u.test(value),'IDENTITY_INVALID',label);return value;}
function relative(value){
  text(value,'path');need(value.length<220&&/^[A-Za-z0-9._/-]+$/.test(value)&&
    value.split('/').every(segment=>segment!=='.'&&segment!=='..'&&segment.length>0&&
      !segment.endsWith('.')&&!/^(con|prn|aux|nul|com[0-9]|lpt[0-9])(?:\.|$)/i.test(segment)),
  'PATH_INVALID',value);return value;
}
function inside(root,name){
  relative(name);const base=path.resolve(root),target=path.resolve(base,name);
  need(target.startsWith(base+path.sep),'PATH_ESCAPE',name);
  let current=path.parse(base).root;
  for(const segment of path.relative(current,target).split(path.sep)){
    if(fs.existsSync(current)){
      need(!fs.lstatSync(current).isSymbolicLink(),'PATH_REPARSE',current);
      const names=fs.readdirSync(current);need(names.includes(segment)||!names.some(x=>x.toLowerCase()===segment.toLowerCase()),'PATH_CASE',name);
    }
    current=path.join(current,segment);
    if(fs.existsSync(current))need(!fs.lstatSync(current).isSymbolicLink(),'PATH_REPARSE',name);
  }
  return target;
}
function bytes(root,name){
  const file=inside(root,name),stat=fs.statSync(file);need(stat.isFile()&&stat.nlink===1&&stat.size>0&&stat.size<=MAX_FILE,'FILE_INVALID',name);
  const value=fs.readFileSync(file);need(value.length===stat.size,'FILE_CHANGED',name);return value;
}
function parse(value,label){
  const source=new TextDecoder('utf-8',{fatal:true,ignoreBOM:true}).decode(value);need(!source.startsWith('\ufeff'),'UTF8_BOM',label);
  const stack=[];
  for(const token of source.match(/"(?:[^"\\]|\\.)*"|[{}\[\]:,]/g)??[]){
    if(token==='{')stack.push({keys:new Set(),key:true});else if(token==='[')stack.push({});
    else if(token==='}'||token===']')stack.pop();else if(token===','&&stack.at(-1)?.keys)stack.at(-1).key=true;
    else if(token===':'&&stack.at(-1)?.keys)stack.at(-1).key=false;
    else if(token.startsWith('"')&&stack.at(-1)?.key){const key=JSON.parse(token),frame=stack.at(-1);
      need(!frame.keys.has(key),'JSON_DUPLICATE_KEY',label+' '+key);frame.keys.add(key);}
    need(stack.length<=24,'DOCUMENT_DEPTH',label);
  }
  const result=JSON.parse(source);let count=0;
  function walk(item,depth){
    need(++count<=100000&&depth<=24,'DOCUMENT_LIMIT',label);
    if(typeof item==='number')need(Number.isFinite(item),'DOCUMENT_NUMBER',label);
    if(typeof item==='string')need(item.length<=MAX_FILE&&item===item.normalize('NFC'),'DOCUMENT_ENCODING',label);
    if(item&&typeof item==='object')for(const [key,child]of Object.entries(item)){
      need(!['__proto__','constructor','prototype'].includes(key),'DOCUMENT_KEY',label);walk(child,depth+1);
    }
  }
  walk(result,0);return result;
}
const document=(root,name)=>parse(bytes(root,name),name);
const digest=(value,label)=>need(typeof value==='string'&&/^[a-f0-9]{64}$/.test(value),'HASH_REQUIRED',label);
function fileRef(root,ref,files){
  need(ref&&typeof ref==='object','FILE_REFERENCE_REQUIRED');relative(ref.path);digest(ref.sha256,ref.path);
  const value=bytes(root,ref.path);need(value.length===ref.bytes&&hash(value)===ref.sha256,'FILE_HASH_MISMATCH',ref.path);
  if(files.has(ref.path))need(files.get(ref.path).equals(value),'FILE_CHANGED',ref.path);else files.set(ref.path,value);
  need([...files.values()].reduce((sum,x)=>sum+x.length,0)<=MAX_TOTAL,'PACK_SIZE_LIMIT');return value;
}
function expected(root=ROOT){
  const register=document(root,EXPECTED),counts={CH:13,AN:18,IT:24,BL:16,EN:11,FX:12,UI:27,AU:30,DT:27,TXT:2};
  const ids=Object.entries(counts).flatMap(([prefix,count])=>Array.from({length:count},(_,i)=>prefix+'-'+String(i+1).padStart(2,'0')));
  need(register.schemaVersion===1&&register.obligations.length===180,'EXPECTED_COUNT');
  need(new Set(register.obligations.map(x=>x.id)).size===180&&ids.every(id=>register.obligations.some(x=>x.id===id)),'EXPECTED_IDS');
  for(const row of register.obligations){text(row.name,row.id);text(row.source,row.id);}
  return register;
}
function sourceKey(source){
  need(source&&Number.isSafeInteger(source.objectId)&&source.objectId!==0,'SOURCE_ID_REQUIRED');
  relative(source.owner);text(source.type,'source.type');text(source.name,'source.name');digest(source.ownerSha256,'source.ownerSha256');
  digest(source.rawSha256,'source.rawSha256');return JSON.stringify([source.owner,source.type,source.objectId]);
}
function resolveOrigin(origins,selection){
  const key=sourceKey(selection),matches=origins.filter(item=>sourceKey(item)===key);
  need(matches.length===1&&equal(matches[0],selection),'ORIGIN_NOT_EXACT',key);return matches[0];
}
function recipe(root,ref,files){
  const record=parse(fileRef(root,ref,files),ref.path);
  need(record.recipe&&typeof record.recipe.code==='string'&&record.recipe.code.length>0,'RECIPE_REQUIRED',ref.path);
  digest(record.recipe.sha256,'recipe.sha256');need(hash(record.recipe.code)===record.recipe.sha256,'RECIPE_HASH',ref.path);
  return record;
}
function validate(root,index=document(root,INDEX),register=expected(),{requireComplete=false}={}){
  need(index&&index.schemaVersion===1&&index.id==='kingdom-zombies.v0.1'&&Number.isSafeInteger(index.version)&&index.version>0,'INDEX_INVALID');
  need(Array.isArray(index.shards)&&index.shards.length>0&&index.shards.length<=128&&Array.isArray(index.outputs)&&index.outputs.length<=4096,'INDEX_COLLECTIONS');
  need(index.status==='incomplete'||index.status==='complete','INDEX_STATUS');
  const files=new Map(),origins=index.origins;need(Array.isArray(origins)&&origins.length<=8192,'ORIGINS_REQUIRED');
  const originKeys=origins.map(sourceKey);need(new Set(originKeys).size===originKeys.length,'SOURCE_DUPLICATE');
  const shards=new Map(),ids=new Set(),paths=new Map(),diagnostics=[],valid=[],coverage=new Map();
  for(const shard of index.shards){
    text(shard.id,'shard.id');text(shard.ownerPlan,'shard.ownerPlan');relative(shard.path);
    need(!shards.has(shard.id),'SHARD_DUPLICATE',shard.id);shards.set(shard.id,shard);
    if(shard.status==='pending'){diagnostics.push({code:'SHARD_PENDING',path:shard.path,id:shard.id});continue;}
    need(shard.status==='selected','SHARD_STATUS',shard.id);
    const body=parse(fileRef(root,shard,files),shard.path);need(body.id===shard.id&&body.version===shard.version&&Array.isArray(body.resources),'SHARD_IDENTITY',shard.path);
    shard.body=body;
  }
  for(const output of index.outputs){
    try{
      text(output.id,'output.id');need(/^[a-z][a-z0-9.-]*$/.test(output.id)&&!ids.has(output.id),'OUTPUT_ID_DUPLICATE_OR_INVALID',output.id);ids.add(output.id);
      need(Number.isSafeInteger(output.version)&&output.version>0,'VERSION_REQUIRED',output.id);text(output.name,output.id);
      const value=fileRef(root,output,files);need(['reuse','adapt','generate'].includes(output.route),'ROUTE_REQUIRED',output.id);
      const shard=shards.get(output.shard),binding=shard?.body?.resources.filter(row=>row.id===output.id);
      need(binding?.length===1&&binding[0].version===output.version&&binding[0].path===output.path.replace(/^src\/godot-game\//,'')&&binding[0].sha256===output.sha256,'SHARD_OUTPUT_BINDING',output.id);
      const production=recipe(root,output.production,files);
      need(Array.isArray(output.dependencies)&&Array.isArray(output.origins),'DEPENDENCIES_REQUIRED',output.id);
      const depIds=new Set();for(const dep of output.dependencies){text(dep.id,'dependency.id');need(!depIds.has(dep.id),'DEPENDENCY_DUPLICATE',dep.id);depIds.add(dep.id);fileRef(root,dep,files);}
      if(output.route!=='generate'){
        need(output.origins.length>0,'ORIGIN_REQUIRED',output.id);
        for(const selection of output.origins){const origin=resolveOrigin(origins,selection);
          need(Array.isArray(origin.streams)&&Array.isArray(origin.dependencies),'SOURCE_DEPENDENCIES_REQUIRED',output.id);
          for(const stream of origin.streams){relative(stream.path);digest(stream.sha256,stream.path);
            need(Number.isSafeInteger(stream.offset)&&stream.offset>=0&&Number.isSafeInteger(stream.bytes)&&stream.bytes>0,'STREAM_RANGE',output.id);}
          for(const dep of origin.dependencies){relative(dep.owner);text(dep.type,'source.dependency.type');text(dep.name,'source.dependency.name');need(Number.isSafeInteger(dep.objectId),'SOURCE_DEPENDENCY_ID');}
          need(production.source?.owner===origin.owner&&production.source.type===origin.type&&production.source.objectId===origin.objectId&&production.source.name===origin.name&&
            production.preservation?.rawSpriteSha256===origin.rawSha256&&production.preservation?.originals?.some(x=>x.path===origin.owner&&x.sha256===origin.ownerSha256),'PRODUCTION_ORIGIN_BINDING',output.id);
          need(origin.streams.every(x=>production.preservation.originals.some(p=>p.path===x.path&&p.sha256===x.sha256)),'PRODUCTION_STREAM_BINDING',output.id);
          need(production.bundle&&hash(Buffer.from(production.bundle.rawSpriteBase64??'','base64'))===origin.rawSha256,'RETAINED_SOURCE_HASH',output.id);
          for(const stream of origin.streams){digest(stream.rangeSha256,'stream.rangeSha256');
            need(production.preservation.streamRangeSha256===stream.rangeSha256&&production.source.stream?.path===stream.path&&
              production.source.stream.offset===stream.offset&&production.source.stream.size===stream.bytes,'PRODUCTION_STREAM_BINDING',output.id);}
        }
      }
      need(production.id===output.id&&production.version===output.version,'PRODUCTION_IDENTITY',output.id);
      need(output.metadata&&Object.keys(output.metadata).length>0&&output.applicability&&Object.keys(output.applicability).length>0,'METADATA_REQUIRED',output.id);
      if(output.metadata.kind==='image')need(value.subarray(0,8).equals(Buffer.from('89504e470d0a1a0a','hex'))&&value.readUInt32BE(16)===output.metadata.width&&value.readUInt32BE(20)===output.metadata.height&&production.image?.pngSha256===output.sha256,'IMAGE_METADATA',output.id);
      text(output.inspection.route,'inspection.route');need(depIds.has(output.inspection.dependency),'INSPECTION_DEPENDENCY',output.id);
      const evidence=output.evidence;need(evidence&&['pending','verified'].includes(evidence.source)&&['pending','verified'].includes(evidence.native)&&
        ['pending','accepted','not-applicable'].includes(evidence.listening)&&['pending','accepted'].includes(evidence.judgment),'EVIDENCE_REQUIRED',output.id);
      need(evidence.listening!=='not-applicable'||output.applicability.listening==='not-applicable','LISTENING_APPLICABILITY',output.id);
      need(Array.isArray(evidence.records),'EVIDENCE_RECORDS_REQUIRED',output.id);
      for(const proof of evidence.records){text(proof.kind,'evidence.kind');text(proof.caseId,'evidence.caseId');text(proof.invocation,'evidence.invocation');
        const record=parse(fileRef(root,proof,files),proof.path);need(record.status==='pass'&&record.nonce===proof.invocation&&record.caseId===proof.caseId&&
          record.outputs?.some(x=>x.id===output.id&&x.version===output.version&&x.sha256===output.sha256),'EVIDENCE_BINDING',output.id);}
      for(const [kind,status]of Object.entries({source:evidence.source,native:evidence.native,listening:evidence.listening,judgment:evidence.judgment}))
        if(['verified','accepted'].includes(status))need(evidence.records.some(x=>x.kind===kind),'EVIDENCE_MISSING',output.id+' '+kind);
      need(Array.isArray(output.obligations)&&output.obligations.length>0&&new Set(output.obligations.map(x=>x.id)).size===output.obligations.length,'OBLIGATIONS_REQUIRED',output.id);
      for(const obligation of output.obligations){need(register.obligations.some(x=>x.id===obligation.id)&&['partial','candidate'].includes(obligation.coverage),'OBLIGATION_INVALID',output.id);
        if(obligation.coverage==='candidate'&&evidence.source==='verified'&&evidence.native==='verified'&&evidence.judgment==='accepted'&&evidence.listening!=='pending'){
          if(!coverage.has(obligation.id))coverage.set(obligation.id,[]);coverage.get(obligation.id).push(output.id);}}
      if(paths.has(output.path)){const other=paths.get(output.path);need(output.sharedFileId&&output.sharedFileId===other.sharedFileId&&output.sha256===other.sha256,'SHARED_OUTPUT_NOT_EXPLICIT',output.id);}
      else paths.set(output.path,output);
      valid.push({id:output.id,version:output.version,path:output.path,sha256:output.sha256,origins:output.origins,obligations:output.obligations});
    }catch(error){diagnostics.push({code:'OUTPUT_INVALID',id:output.id,path:output.path,message:error.message});}
  }
  for(const shard of shards.values())delete shard.body;
  const missing=register.obligations.filter(x=>!coverage.has(x.id)).map(x=>({id:x.id,code:'OBLIGATION_INCOMPLETE'}));
  diagnostics.push(...missing);const complete=diagnostics.length===0;
  need(index.status!=='complete'||complete,'FALSE_COMPLETION');need(!requireComplete||complete,'PACK_INCOMPLETE');
  return {status:complete?'complete':'incomplete',complete,valid,diagnostics,covered:[...coverage.keys()].sort(),expected:180,files};
}
function merge(base,addition){
  const result=structuredClone(base);
  for(const field of ['origins','shards','outputs']){
    const key=item=>field==='origins'?sourceKey(item):item.id;
    for(const item of addition[field]??[]){const previous=result[field].find(x=>key(x)===key(item));
      if(previous){
        const replacing=field==='shards'&&previous.path===item.path&&previous.ownerPlan===item.ownerPlan&&
          ((previous.status==='pending'&&item.status==='selected')||(previous.status==='selected'&&item.status==='selected'&&item.version>previous.version));
        if(replacing)Object.assign(previous,structuredClone(item));else need(equal(previous,item),'IDENTITY_IMMUTABLE',key(item));
      }else result[field].push(structuredClone(item));}
    result[field].sort((a,b)=>key(a)<key(b)?-1:key(a)>key(b)?1:0);
  }
  return result;
}
function destination(root){
  const absolute=path.resolve(root),repo=fs.realpathSync.native(ROOT),rel=path.relative(repo,absolute).split(path.sep).join('/');relative(rel);
  need(rel.startsWith('.phase4/')||rel.startsWith('production/phase4/published/'),'PUBLICATION_ROOT_DENIED',rel);
  inside(repo,rel);return absolute;
}
function write(root,name,value){
  const file=inside(root,name);fs.mkdirSync(path.dirname(file),{recursive:true});const fd=fs.openSync(file,'wx');
  try{fs.writeFileSync(fd,Buffer.isBuffer(value)?value:Buffer.from(JSON.stringify(value,null,2)+'\n'));fs.fsyncSync(fd);}finally{fs.closeSync(fd);}
}
function publish({sourceRoot,index,publicationRoot,invocation=crypto.randomUUID(),observe=()=>{}}){
  need(/^[a-z0-9-]{1,80}$/.test(invocation),'INVOCATION_INVALID');const root=destination(publicationRoot);fs.mkdirSync(root,{recursive:true});
  const lock=inside(root,'publication.lock');let descriptor;
  try{descriptor=fs.openSync(lock,'wx');}catch(error){if(error.code==='EEXIST')throw new Error('PUBLICATION_LOCKED '+lock);throw error;}
  const stage='stage-'+invocation,generation='generations/'+invocation,pointer='current-'+invocation+'.json';let committed=false,moved=false;
  try{
    fs.writeFileSync(descriptor,JSON.stringify({invocation,pid:process.pid}));fs.fsyncSync(descriptor);
    observe('locked');const checked=validate(sourceRoot,structuredClone(index));need(!checked.diagnostics.some(x=>x.code==='OUTPUT_INVALID'),'PUBLICATION_INVALID_OUTPUT');
    if(fs.existsSync(inside(root,'current.json'))){const current=readPublished(root);merge(current.index,index);
      need(current.index.outputs.every(x=>index.outputs.some(y=>equal(x,y)))&&current.index.shards.filter(x=>x.status==='selected').every(x=>index.shards.some(y=>y.id===x.id&&y.status==='selected'&&y.version>=x.version)),'PUBLICATION_REGRESSION');}
    const staged=inside(root,stage);fs.mkdirSync(staged);write(staged,INDEX,index);
    for(const [name,value]of checked.files)write(staged,name,value);
    observe('staged');const proof=validate(staged,document(staged,INDEX));need(!proof.diagnostics.some(x=>x.code==='OUTPUT_INVALID'),'STAGED_INVALID');
    observe('validated');fs.mkdirSync(inside(root,'generations'),{recursive:true});need(!fs.existsSync(inside(root,generation)),'INVOCATION_REUSED');
    fs.renameSync(staged,inside(root,generation));moved=true;
    write(root,pointer,{schemaVersion:1,invocation,generation,indexSha256:hash(bytes(inside(root,generation),INDEX)),status:proof.status});
    observe('before-commit');inside(root,'current.json');fs.renameSync(inside(root,pointer),inside(root,'current.json'));committed=true;
    return {invocation,generation,status:proof.status,complete:proof.complete,outputs:proof.valid.map(({id,version,sha256})=>({id,version,sha256}))};
  }finally{
    if(fs.existsSync(inside(root,pointer)))fs.unlinkSync(inside(root,pointer));
    const cleanup=moved?generation:stage;
    if(!committed&&fs.existsSync(inside(root,cleanup)))fs.rmSync(inside(root,cleanup),{recursive:true});
    fs.closeSync(descriptor);need(document(root,'publication.lock').invocation===invocation,'LOCK_OWNERSHIP');fs.unlinkSync(lock);
  }
}
function readPublished(publicationRoot){
  const root=destination(publicationRoot),pointer=document(root,'current.json');
  need(pointer.schemaVersion===1&&pointer.generation==='generations/'+pointer.invocation,'PUBLICATION_POINTER');
  const generation=inside(root,pointer.generation),value=bytes(generation,INDEX);need(hash(value)===pointer.indexSha256,'PUBLICATION_INDEX_HASH');
  const index=parse(value,INDEX),result=validate(generation,index);need(!result.diagnostics.some(x=>x.code==='OUTPUT_INVALID')&&result.status===pointer.status,'PUBLICATION_INVALID');
  return {pointer,index,result};
}
module.exports={ROOT,INDEX,EXPECTED,hash,need,text,relative,inside,bytes,parse,document,expected,sourceKey,resolveOrigin,validate,merge,destination,write,publish,readPublished};
