'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {TextDecoder,isDeepStrictEqual:equal}=require('node:util');
const {spawnSync}=require('node:child_process');
const zlib=require('node:zlib'),{pipeline}=require('node:stream/promises'),{Readable}=require('node:stream');
const {physical,logical}=require('./layout.cjs');
const workspace=require('../workspace-paths.cjs');
const ROOT=path.resolve(__dirname,'../../../..'),INDEX='src/godot-game/content/pack-index.json';
const EXPECTED='tools/phase4/expected-obligations.json',MAX_FILE=32*1024*1024,MAX_TOTAL=256*1024*1024;
const PACKAGED_EXPECTED='src/godot-game/content/expected-obligations.json',EXPECTED_BYTES=25345;
const EXPECTED_SHA256='a4d9243bda1b512b59c189a3bdc48a7cccc8171a218bbfc5ae16b00274f2fda5';
const COMMON_ARCHIVE='production/phase4/common-selected.zip',COMMON_ARCHIVE_HASH='2f66618305221d32d503bba04be755ae38e606408a7c5d9d8f65a1db19d9e6d9';
const COMMON_STREAM='streams/sharedassets0.assets.resS/167471920-67108864.bin',COMMON_STREAM_HASH='dc0a7a109f52fbadf7249a714e8b5116d57c5af4a3e86f43befd4fca4545a0e4';
const archiveAuthentications=new WeakSet(),crcTable=Array.from({length:256},(_,value)=>{for(let bit=0;bit<8;bit++)value=value&1?0xedb88320^(value>>>1):value>>>1;return value>>>0;});
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
  relative(name);const base=path.resolve(root),target=path.resolve(base,physical(name));
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
// This checkout's asset-pipeline runs and scratch, spelled relative to a checkout root for the in-checkout path checks.
function workspacePath(absolute){return path.relative(workspace.checkoutRoot(),absolute).split(path.sep).join('/');}
function runsPath(...parts){return workspacePath(workspace.locations().runs('asset-pipeline',...parts));}
function tempPath(...parts){return workspacePath(workspace.locations().temp('asset-pipeline',...parts));}
function feedPath(...parts){return workspacePath(path.join(workspace.locations().nuget.feed,...parts));}
// Historical record identities resolve through the workspace map, still relative to the checkout.
function legacyLocal(identity){
  const absolute=workspace.legacyPath(identity);need(absolute,'LEGACY_PATH_UNKNOWN',identity);
  const rel=workspacePath(absolute);need(!rel.startsWith('..')&&!path.isAbsolute(rel),'LEGACY_PATH_SHARED',identity);return rel;
}
// The narrow shared-root allowance: Godot downloads, the Godot editor and NuGet packages, read from the
// repository's shared root by real path. Everything else stays inside the checkout.
function sharedFile(kind,...parts){
  const where=workspace.locations(),file=kind==='downloads'?where.downloads('godot',...parts):kind==='tools'?
    where.tools('godot',...parts):kind==='packages'?path.join(where.nuget.packages,...parts):null;
  need(file&&parts.every(part=>typeof part==='string'&&part.length>0&&!part.includes('..'))&&workspace.insideShared(file),
    'SHARED_PATH_DENIED',kind+' '+parts.join('/'));
  need(!workspace.linkedComponent(where.shared,file),'PATH_REPARSE',file);
  return file;
}
function legacyShared(identity){
  const absolute=workspace.legacyPath(identity);need(absolute&&workspace.insideShared(absolute),'LEGACY_PATH_NOT_SHARED',identity);
  need(!workspace.linkedComponent(workspace.locations().shared,absolute),'PATH_REPARSE',identity);return absolute;
}
// Shared files obey the same single-link rule as bytes(): no hard link to another name.
function sharedBytes(file,limit){
  const stat=fs.statSync(file);need(stat.isFile()&&stat.nlink===1&&stat.size>0&&stat.size<=limit,'FILE_INVALID',file);
  const value=fs.readFileSync(file);need(value.length===stat.size,'FILE_CHANGED',file);return value;
}
function bytes(root,name){
  const file=inside(root,name),stat=fs.statSync(file),limit=name==='production/phase4/audio-interface-ambience.zip'?64*1024*1024:MAX_FILE;
  need(stat.isFile()&&stat.nlink===1&&stat.size>0&&stat.size<=limit,'FILE_INVALID',name);
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
  const value=bytes(root,EXPECTED);need(value.length===EXPECTED_BYTES&&hash(value)===EXPECTED_SHA256,'EXPECTED_AUTHORITY_DRIFT',EXPECTED);
  const register=parse(value,EXPECTED),counts={CH:13,AN:18,IT:24,BL:16,EN:11,FX:12,UI:27,AU:30,DT:27,TXT:2};
  const ids=Object.entries(counts).flatMap(([prefix,count])=>Array.from({length:count},(_,i)=>prefix+'-'+String(i+1).padStart(2,'0')));
  need(register.schemaVersion===1&&register.obligations.length===180,'EXPECTED_COUNT');
  need(new Set(register.obligations.map(x=>x.id)).size===180&&ids.every(id=>register.obligations.some(x=>x.id===id)),'EXPECTED_IDS');
  for(const row of register.obligations){text(row.name,row.id);text(row.source,row.id);}
  return register;
}
function packagedExpected(root,index,register,files){
  try{
    const authority=expected(),source=bytes(ROOT,EXPECTED),ref=index.expectedObligations;
    need(equal(register,authority),'EXPECTED_CALLER_AUTHORITY',PACKAGED_EXPECTED);
    need(ref&&typeof ref==='object'&&!Array.isArray(ref)&&
      equal(Object.keys(ref).sort(),['bytes','path','sha256']),'EXPECTED_REFERENCE_REQUIRED',PACKAGED_EXPECTED);
    need(ref.path===PACKAGED_EXPECTED,'EXPECTED_PATH',String(ref.path));
    need(ref.bytes===EXPECTED_BYTES&&ref.sha256===EXPECTED_SHA256,'EXPECTED_REFERENCE_IDENTITY',PACKAGED_EXPECTED);
    const value=fileRef(root,ref,files),packaged=parse(value,PACKAGED_EXPECTED);
    need(value.equals(source)&&equal(packaged,authority),'EXPECTED_CONTENT_IDENTITY',PACKAGED_EXPECTED);
    return authority;
  }catch(error){throw new Error('EXPECTED_PACKAGE_INVALID '+PACKAGED_EXPECTED+' '+error.message,{cause:error});}
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
  text(record.recipe.language,'recipe.language');
  need(Array.isArray(record.recipe.transformations)&&record.recipe.transformations.length>0&&record.recipe.transformations.every(x=>typeof x==='string'&&x.length>0),'TRANSFORMATIONS_REQUIRED',ref.path);
  digest(record.recipe.sha256,'recipe.sha256');need(hash(record.recipe.code)===record.recipe.sha256,'RECIPE_HASH',ref.path);
  return record;
}
function archiveBounds(auth,{name,bytes,expanded,sha256}){
  need(archiveAuthentications.has(auth),'ARCHIVE_AUTH_REQUIRED');relative(name);
  const exact=auth.common&&name===COMMON_STREAM,limit=exact?67108864:MAX_FILE;
  need(Number.isSafeInteger(bytes)&&bytes>=0&&bytes<=limit,'ARCHIVE_MEMBER_LIMIT',name);
  need(Number.isSafeInteger(expanded)&&expanded>=0&&expanded<=(auth.common?279502632:128*1024*1024),'ARCHIVE_EXPANDED_LIMIT',auth.path);
  if(exact&&sha256!==undefined)need(bytes===67108864&&sha256===COMMON_STREAM_HASH,'ARCHIVE_MEMBER_IDENTITY',name);
}
async function archiveSource(root,ref,consume){
  need(ref&&typeof ref==='object','FILE_REFERENCE_REQUIRED');relative(ref.path);digest(ref.sha256,ref.path);
  const file=inside(root,ref.path),stat=fs.statSync(file),limit=ref.path==='production/phase4/audio-interface-ambience.zip'?64*1024*1024:MAX_FILE,common=ref.path===COMMON_ARCHIVE;
  need(stat.isFile()&&stat.nlink===1&&stat.size>0&&stat.size<=limit,'FILE_INVALID',ref.path);
  need(Number.isSafeInteger(ref.bytes)&&ref.bytes===stat.size,'FILE_HASH_MISMATCH',ref.path);
  if(common)need(ref.bytes===12308215&&ref.sha256===COMMON_ARCHIVE_HASH,'ARCHIVE_SOURCE_IDENTITY',ref.path);
  const fd=fs.openSync(file,'r');
  try{
    const same=()=>{const current=fs.fstatSync(fd);need(current.dev===stat.dev&&current.ino===stat.ino&&current.size===stat.size&&current.mtimeMs===stat.mtimeMs&&current.ctimeMs===stat.ctimeMs&&current.nlink===1,'FILE_CHANGED',ref.path);};
    const authenticate=async()=>{same();const state=crypto.createHash('sha256');let count=0;while(count<stat.size){const chunk=archiveRead(fd,count,Math.min(65536,stat.size-count),stat.size);count+=chunk.length;state.update(chunk);}need(count===stat.size&&state.digest('hex')===ref.sha256,'FILE_HASH_MISMATCH',ref.path);same();};
    await authenticate();const auth=Object.freeze({path:ref.path,sha256:ref.sha256,bytes:ref.bytes,common});archiveAuthentications.add(auth);
    const result=await consume(auth,fd,stat.size,file);await authenticate();inside(root,ref.path);return result;
  }finally{fs.closeSync(fd);}
}
async function authenticateArchive(root,ref){return archiveSource(root,ref,auth=>auth);}
function archiveRead(fd,position,length,size){
  need(Number.isSafeInteger(position)&&Number.isSafeInteger(length)&&position>=0&&length>=0&&length<=65557&&position+length<=size,'ARCHIVE_EXTENT');
  const value=Buffer.alloc(length);let count=0;while(count<length){const next=fs.readSync(fd,value,count,length-count,position+count);need(next>0,'ARCHIVE_TRUNCATED');count+=next;}return value;
}
function archiveDirectory(auth,fd,size){
  const tailStart=Math.max(0,size-65557),tail=archiveRead(fd,tailStart,size-tailStart,size);let end=-1;
  for(let index=tail.length-22;index>=0;index--)if(tail.readUInt32LE(index)===0x06054b50&&index+22+tail.readUInt16LE(index+20)===tail.length){end=index;break;}
  need(end>=0,'ARCHIVE_END');const count=tail.readUInt16LE(end+10),start=tail.readUInt32LE(end+16),length=tail.readUInt32LE(end+12),endAt=tailStart+end;
  need(tail.readUInt16LE(end+4)===0&&tail.readUInt16LE(end+6)===0&&tail.readUInt16LE(end+8)===count&&count>0&&count<=4096,'ARCHIVE_MEMBER_COUNT');
  need(!auth.common||count===2395,'ARCHIVE_SOURCE_MEMBER_COUNT');need(start+length===endAt,'ARCHIVE_DIRECTORY_EXTENT');
  const members=[],names=new Set(),aliases=new Set();let position=start,expanded=0;
  for(let index=0;index<count;index++){
    const header=archiveRead(fd,position,46,size);need(header.readUInt32LE(0)===0x02014b50,'ARCHIVE_CENTRAL_HEADER');
    const nameLength=header.readUInt16LE(28),extra=header.readUInt16LE(30),comment=header.readUInt16LE(32),flags=header.readUInt16LE(8),method=header.readUInt16LE(10);
    need(nameLength>0&&nameLength<220&&header.readUInt16LE(6)<=20&&(flags&~0x800)===0&&[0,8].includes(method)&&header.readUInt16LE(34)===0,'ARCHIVE_MEMBER_FORMAT');
    need(((header.readUInt32LE(38)>>>16)&0xf000)!==0xa000,'ARCHIVE_MEMBER_REPARSE');
    const rawName=archiveRead(fd,position+46,nameLength,size),name=new TextDecoder('utf-8',{fatal:true}).decode(rawName);relative(name);
    need((flags&0x800)!==0||rawName.every(x=>x<128),'ARCHIVE_MEMBER_ENCODING');need(!names.has(name),'ARCHIVE_MEMBER_DUPLICATE',name);need(!aliases.has(name.toLowerCase()),'ARCHIVE_MEMBER_ALIAS',name);names.add(name);aliases.add(name.toLowerCase());
    const member={path:name,bytes:header.readUInt32LE(24),compressedBytes:header.readUInt32LE(20),crc:header.readUInt32LE(16),local:header.readUInt32LE(42),method,flags};
    expanded+=member.bytes;archiveBounds(auth,{name,bytes:member.bytes,expanded});
    need(member.compressedBytes<0xffffffff&&member.bytes<0xffffffff&&member.local<start,'ARCHIVE_MEMBER_EXTENT',name);
    const local=archiveRead(fd,member.local,30,size);need(local.readUInt32LE(0)===0x04034b50&&local.readUInt16LE(4)<=20&&local.readUInt16LE(6)===flags&&local.readUInt16LE(8)===method&&local.readUInt32LE(14)===member.crc&&local.readUInt32LE(18)===member.compressedBytes&&local.readUInt32LE(22)===member.bytes&&local.readUInt16LE(26)===nameLength,'ARCHIVE_LOCAL_BINDING',name);
    need(archiveRead(fd,member.local+30,nameLength,size).equals(rawName),'ARCHIVE_LOCAL_NAME',name);member.offset=member.local+30+nameLength+local.readUInt16LE(28);member.end=member.offset+member.compressedBytes;
    need(member.end<=start,'ARCHIVE_MEMBER_EXTENT',name);members.push(member);position+=46+nameLength+extra+comment;need(position<=endAt,'ARCHIVE_DIRECTORY_EXTENT');
  }
  need(position===endAt,'ARCHIVE_DIRECTORY_EXTENT');let endOfData=0;for(const member of [...members].sort((a,b)=>a.local-b.local)){need(member.local===endOfData,'ARCHIVE_OVERLAPPING_OR_HIDDEN_MEMBER',member.path);endOfData=member.end;}need(endOfData===start,'ARCHIVE_DIRECTORY_EXTENT');
  need(!auth.common||expanded===279502632,'ARCHIVE_SOURCE_EXPANDED_IDENTITY');return members;
}
async function archive(root,ref,{capture=[]}={}){
  need(Array.isArray(capture)&&capture.length<=1024&&new Set(capture).size===capture.length,'ARCHIVE_CAPTURE_SELECTION');for(const name of capture)relative(name);
  const selected=new Set(capture),retained=new Map();let retainedBytes=0;
  return archiveSource(root,ref,async(auth,fd,size,file)=>{
    const members=archiveDirectory(auth,fd,size),observations=[];let expandedBytes=0,maxOutputChunkBytes=0;
    for(const member of members){
      const state=crypto.createHash('sha256'),chunks=[];let crc=0xffffffff,count=0;
      const accept=chunk=>{count+=chunk.length;maxOutputChunkBytes=Math.max(maxOutputChunkBytes,chunk.length);archiveBounds(auth,{name:member.path,bytes:count,expanded:expandedBytes+count});need(count<=member.bytes,'ARCHIVE_MEMBER_SIZE',member.path);for(const byte of chunk)crc=crcTable[(crc^byte)&255]^(crc>>>8);state.update(chunk);if(selected.has(member.path)){retainedBytes+=chunk.length;need(count<=MAX_FILE&&retainedBytes<=MAX_TOTAL,'ARCHIVE_CAPTURE_BOUND',member.path);chunks.push(Buffer.from(chunk));}};
      if(member.compressedBytes>0){
        const chunks=function*(){for(let position=member.offset;position<member.end;position+=16384)yield archiveRead(fd,position,Math.min(16384,member.end-position),size);};
        const source=Readable.from(chunks(),{objectMode:false,highWaterMark:16384}),inflate=member.method===8?zlib.createInflateRaw({chunkSize:16384}):null;
        try{if(inflate){await pipeline(source,inflate,async output=>{for await(const chunk of output)accept(chunk);});need(inflate.bytesWritten===member.compressedBytes,'ARCHIVE_COMPRESSED_EXTENT',member.path);}else{for await(const chunk of source)accept(chunk);}}finally{source.destroy();inflate?.destroy();}
      }
      const actualHash=state.digest('hex');need(count===member.bytes,'ARCHIVE_MEMBER_SIZE',member.path);need(((crc^0xffffffff)>>>0)===member.crc,'ARCHIVE_MEMBER_CRC',member.path);expandedBytes+=count;archiveBounds(auth,{name:member.path,bytes:count,expanded:expandedBytes,sha256:actualHash});
      observations.push({path:member.path,bytes:count,compressedBytes:member.compressedBytes,sha256:actualHash,crc32:member.crc.toString(16).padStart(8,'0')});
      if(selected.has(member.path))retained.set(member.path,Buffer.concat(chunks));
    }
    need(retained.size===selected.size,'ARCHIVE_CAPTURE_MISSING');
    return {path:ref.path,bytes:ref.bytes,sha256:ref.sha256,members:observations,expandedBytes,maxOutputChunkBytes,authenticationChunkBytes:65536,extent:'All source bytes authenticated before parse and after full member EOF; each member CRC/SHA checked without retaining unselected expanded payloads',...(selected.size?{retained}: {})};
  });
}
const retainedContracts=[
  ['common-selected','common','common.json','donor-kit.png'],
  ['bodies-a','bodies-a','bodies-a.json','bodies-a.png'],['bodies-b','bodies-b','bodies-b.json','bodies-b.png'],['bodies-c','bodies-c','bodies-c.json','bodies-c.png'],
  ['items-01-10','items-01-10','items-01-10.json','items-01-10.png'],['items-11-24','items-11-24','items-11-24.json','items-11-24.png'],
  ['main-buildings','main-buildings','main-buildings.json','main-tiers.png'],['defenses','defenses','defenses.json','defenses.png'],
  ['scale-pilot','scale-pilot','scale-pilot.json','scale-pilot.png'],['world','world','world.json','environment.png'],
  ['audio-actions','audio-actions','production/manifest.json'],['audio-interface-ambience','audio-interface-ambience','production/manifest.json'],
  ['work-special',null,'work-special.json'],['ui-identity',null,'recipes/identity.json']
];
function retainedMember(proof,name,sha256,bytes){
  relative(name);const found=proof.members.filter(x=>x.path===name);need(found.length===1,'SOURCE_MEMBER_UNAVAILABLE',proof.path+'#'+name);
  const row=found[0];if(sha256!==undefined){digest(sha256,name);need(row.sha256===sha256,'SOURCE_MEMBER_HASH',proof.path+'#'+name);}if(bytes!==undefined)need(Number.isSafeInteger(bytes)&&bytes>0&&row.bytes===bytes,'SOURCE_MEMBER_EXTENT',proof.path+'#'+name);return row;
}
function retainedInventory(proof,inventory){
  need(Array.isArray(inventory)&&inventory.length===proof.members.length-1,'SOURCE_INVENTORY_EXTENT',proof.path);const seen=new Set();
  for(const row of inventory){need(row.path!=='members.json'&&!seen.has(row.path),'SOURCE_INVENTORY_DUPLICATE',proof.path+'#'+row.path);seen.add(row.path);retainedMember(proof,row.path,row.sha256,row.bytes);}
}
function retainedRecipe(proof,body,name){
  const recipe=body.recipe??body.recipes;if(!recipe)return;
  const known={common:'recipes/extract-media.py','bodies-a':'recipes/build-bodies.py','bodies-b':'recipes/build-bodies.py','bodies-c':'recipes/build-bodies.py','items-01-10':'recipes/build-items.py','items-11-24':'recipes/build-items.py','main-buildings':'recipes/build-main.py',defenses:'recipes/build-defenses.py','scale-pilot':'recipes/build-scale.py',world:'recipes/build-world.py'};
  if(known[name]){const member=recipe.member??recipe.path;need(member===known[name],'SOURCE_RECIPE_ROLE',name);retainedMember(proof,member,recipe.sha256);}
  const actual={
    'audio-actions':[['decoder','recipes/decode.py'],['assembler','recipes/assemble.py'],['promotion','recipes/promote.py']],
    'audio-interface-ambience':[['decoder','recipes/decode-interface.py'],['assembler','recipes/assemble-interface.py']],
    'scale-pilot':[['verifier','recipes/verify-scale.py']]
  };
  for(const [role,member]of actual[name]??[]){need(recipe[role+'Member']===member,'SOURCE_RECIPE_ROLE',name+' '+role);retainedMember(proof,member,recipe[role+'Sha256']);}
}
function retainedCommon(proof,pack,manifest){
  retainedMember(proof,'preserved-manifest.json',pack.preservation?.manifestSha256);need(Array.isArray(manifest.files)&&Array.isArray(pack.preservation?.originals),'SOURCE_ORIGINAL_MANIFEST');
  const originals=new Map();for(const original of pack.preservation.originals){need(!originals.has(original.path),'SOURCE_ORIGINAL_DUPLICATE',original.path);const found=manifest.files.filter(x=>x.path===original.path);need(found.length===1&&found[0].sha256===original.sha256&&found[0].bytes===original.bytes,'SOURCE_ORIGINAL_IDENTITY',original.path);originals.set(original.path,original);}
  const ids=new Set(),sources=new Set();let edges=0;
  function raw(source,scope){
    need(source&&['Sprite','Texture2D','Font','AnimationClip','SpriteAtlas'].includes(source.type)&&Number.isSafeInteger(source.objectId)&&source.objectId>0,'SOURCE_OBJECT_IDENTITY',scope);
    relative(source.owner);const original=originals.get(source.owner);need(original&&original.sha256===source.ownerSha256,'SOURCE_OWNER_IDENTITY',scope);
    const member='raw/'+source.owner+'/'+source.objectId+'.bin';need(source.rawMember===member,'SOURCE_RAW_IDENTITY',scope);retainedMember(proof,member,source.rawSha256);edges++;
    if(source.stream){const stream=source.stream,owner=originals.get(stream.path);need(owner&&Number.isSafeInteger(stream.offset)&&stream.offset>=0&&Number.isSafeInteger(stream.bytes)&&stream.bytes>0&&stream.offset<=owner.bytes-stream.bytes,'SOURCE_STREAM_RANGE',scope);need(stream.member==='streams/'+stream.path+'/'+stream.offset+'-'+stream.bytes+'.bin','SOURCE_STREAM_IDENTITY',scope);retainedMember(proof,stream.member,stream.sha256,stream.bytes);edges++;}
  }
  for(const row of [...pack.resources,...pack.clips]){
    need(typeof row.id==='string'&&!ids.has(row.id),'SOURCE_RESOURCE_DUPLICATE',row.id);ids.add(row.id);
    if(row.id==='common.save-indicator'){need(row.source?.path==='KingdomTwoCrowns_Data/StreamingAssets/SaveIcon.png','SOURCE_SAVE_ICON_IDENTITY');retainedMember(proof,row.member,row.source.sha256);continue;}
    const source=row.source;raw(source,row.id);const identity=source.owner+'#'+source.type+'#'+source.objectId;need(!sources.has(identity),'SOURCE_OBJECT_DUPLICATE',row.id);sources.add(identity);
    const prefix=source.type==='Sprite'?(source.owner==='resources.assets'?'common.r.':source.owner==='sharedassets0.assets'?'common.s0.':'invalid.'):{Texture2D:'common.texture.',Font:'common.font.',AnimationClip:'common.clip.'}[source.type];
    need(row.id===prefix+source.objectId&&row.name===source.name,'SOURCE_RESOURCE_IDENTITY',row.id);
    if(row.member){const group={Sprite:['images','.png'],Texture2D:['textures','.png'],Font:['fonts','.ttf']}[source.type];need(group&&row.member===group[0]+'/'+source.owner+'/'+source.objectId+group[1],'SOURCE_DECODED_MEMBER_IDENTITY',row.id);retainedMember(proof,row.member,row.pngSha256??row.sha256,row.bytes);}
    const dependencies=new Set();for(const dep of row.dependencies??[]){const key=dep.owner+'#'+dep.type+'#'+dep.objectId;need(!dependencies.has(key),'SOURCE_DEPENDENCY_DUPLICATE',row.id);dependencies.add(key);raw(dep,row.id+' '+key);}
    if(source.type==='AnimationClip'){need(row.callbacksExecutable===false&&Array.isArray(row.events)&&row.events.every(x=>x.id==='work.contact'&&x.execution==='presentation-data-only'),'SOURCE_CALLBACK_EXECUTION',row.id);need(Number.isFinite(row.start)&&Number.isFinite(row.stop)&&row.stop>=row.start&&Number.isFinite(row.sampleRate)&&row.sampleRate>0&&Array.isArray(row.layers)&&row.layers.length>0,'SOURCE_CLIP_RANGE',row.id);}
  }
  return{resources:pack.resources.length,clips:pack.clips.length,rawAndStreamEdges:edges};
}
function retainedParts(proof,body,name){
  if(!Array.isArray(body.parts))return;
  const parts=new Map();for(const part of body.parts){need(typeof part.key==='string'&&part.key.length>0&&!parts.has(part.key),'SOURCE_PART_IDENTITY',name);parts.set(part.key,part);if(part.member)retainedMember(proof,part.member,part.sha256);if(part.origin&&proof.members.some(x=>x.path===part.origin))retainedMember(proof,part.origin);}
  if(name.startsWith('bodies-'))for(const character of body.bodies){retainedMember(proof,character.master,character.masterSha256);retainedMember(proof,character.prompt,character.promptSha256);const poses=new Set();for(const pose of character.poses){need(typeof pose.id==='string'&&!poses.has(pose.id),'SOURCE_POSE_DUPLICATE',character.id);poses.add(pose.id);for(const [role,key]of Object.entries(pose.parts)){need(['full','head','upper','lower'].includes(role)&&key===character.id+'.'+pose.id+'.'+role&&parts.has(key),'SOURCE_POSE_PART_ROLE',character.id+' '+pose.id+' '+role);}}}
  if(name.startsWith('items-'))for(const item of body.items){need(parts.has(item.canonicalIcon)&&parts.has(item.master)&&Array.isArray(item.iconVariants)&&item.iconVariants.every(x=>parts.has(x)),'SOURCE_ITEM_ICON_ROLE',item.id);for(const component of item.components)need(parts.has(component.part),'SOURCE_ITEM_COMPONENT',item.id+' '+component.name);}
}
function retainedPcm(value,label){
  need(Buffer.isBuffer(value)&&value.length>=44&&value.length<=16*1024*1024&&value.toString('ascii',0,4)==='RIFF'&&value.readUInt32LE(4)===value.length-8&&value.toString('ascii',8,12)==='WAVE'&&value.toString('ascii',12,16)==='fmt '&&value.readUInt32LE(16)===16&&value.readUInt16LE(20)===1&&value.readUInt16LE(22)===2&&value.readUInt32LE(24)===44100&&value.readUInt32LE(28)===176400&&value.readUInt16LE(32)===4&&value.readUInt16LE(34)===16&&value.toString('ascii',36,40)==='data'&&value.readUInt32LE(40)===value.length-44&&(value.length-44)%4===0,'SOURCE_INTERFACE_PCM_FORMAT',label);return value.subarray(44);
}
function retainedRound(value){const floor=Math.floor(value),fraction=value-floor;return fraction===0.5?(floor%2===0?floor:floor+1):Math.round(value);}
function retainedFonts(proof,common,kit,read){
  const roles={body:['assets/ui/LiberationSans.ttf','common.font.6004'],display:['assets/ui/PerfectDOSVGA437.ttf','common.font.6003'],fallback:['assets/ui/Fallback.ttf','common.font.6004']},seen=new Set();need(Array.isArray(kit.fonts)&&kit.fonts.length===3,'SOURCE_FONT_ROLES');
  for(const font of kit.fonts){const expected=roles[font.role];need(expected&&!seen.has(font.role)&&font.path===expected[0]&&font.id===expected[1],'SOURCE_FONT_ROLE',font.role);seen.add(font.role);const candidates=common.resources.filter(x=>x.id===font.id);need(candidates.length===1&&candidates[0].source.type==='Font','SOURCE_FONT_ORIGIN',font.role);const origin=candidates[0];need(font.member===origin.member&&font.sha256===origin.sha256&&font.bytes===origin.bytes,'SOURCE_FONT_MEMBER_ROLE',font.role);retainedMember(proof,origin.member,origin.sha256,origin.bytes);const actual=read(font.path);need(Buffer.isBuffer(actual)&&actual.length===origin.bytes&&hash(actual)===origin.sha256,'SOURCE_FONT_RUNTIME_BYTES',font.role);need(equal(font.fallbacks,font.role==='fallback'?[]:['assets/ui/Fallback.ttf']),'SOURCE_FONT_FALLBACK_ROLE',font.role);}
  for(const candidate of kit.fontCandidates){const origin=common.resources.find(x=>x.id===candidate.id);need(origin&&origin.source.type==='Font'&&equal(candidate.source,origin.source)&&candidate.member===origin.member&&candidate.sha256===origin.sha256&&candidate.bytes===origin.bytes,'SOURCE_FONT_CANDIDATE_ORIGIN',candidate.id);}
  return{roles:[...seen],actualRetainedFonts:3,sharedBodyFallback:'Explicit distinct runtime roles, same common.font.6004 immutable donor bytes'};
}
function retainedWork(proof,body,common,read){
  need(body.id==='work-special'&&body.clock==='explicit-elapsed-only'&&body.provenance?.master==='generated/work-effects-master.png'&&body.provenance.prompt==='prompts/work-effects.txt'&&body.provenance.recipe==='recipes/produce.py','SOURCE_WORK_GENERATION_ROLE');
  retainedMember(proof,body.provenance.master,body.provenance.masterSha256);retainedMember(proof,body.provenance.prompt,body.provenance.promptSha256);retainedMember(proof,body.provenance.recipe);
  const master=proof.retained.get(body.provenance.master);need(master&&master.subarray(0,8).equals(Buffer.from('89504e470d0a1a0a','hex')),'SOURCE_WORK_MASTER_FORMAT');const width=master.readUInt32BE(16),height=master.readUInt32BE(20),components=new Set();
  function rectangle(value,w,h,label){need(Array.isArray(value)&&value.length===4&&value.every(Number.isSafeInteger)&&value[0]>=0&&value[1]>=0&&value[2]>value[0]&&value[3]>value[1]&&value[2]<=w&&value[3]<=h,'SOURCE_WORK_SOURCE_REGION',label);}
  for(const part of body.components){need(typeof part.key==='string'&&!components.has(part.key),'SOURCE_WORK_COMPONENT_IDENTITY',part.key);components.add(part.key);need(part.filter==='nearest'&&part.rgbRetained===true,'SOURCE_WORK_COMPONENT_ROLE',part.key);if(part.key==='hammer'){const origin=common.resources.find(x=>x.id==='common.s0.6853');need(origin&&part.source==='retained/common/'+origin.member,'SOURCE_WORK_DONOR_ROLE',part.key);retainedMember(proof,part.source,origin.pngSha256);}else{need(part.source===body.provenance.master&&part.alphaThreshold===128,'SOURCE_WORK_COMPONENT_ROLE',part.key);rectangle(part.cell,width,height,part.key+' cell');rectangle(part.crop,part.cell[2]-part.cell[0],part.cell[3]-part.cell[1],part.key+' crop');}}
  const clips=new Set(common.clips.map(x=>x.id)),actions=new Set();for(const action of body.actions){need(typeof action.id==='string'&&!actions.has(action.id),'SOURCE_WORK_ACTION_IDENTITY',action.id);actions.add(action.id);need(action.inventoryMutation===false&&Number.isFinite(action.duration)&&action.duration>0&&Array.isArray(action.samples)&&action.samples.length>0&&action.samples.every(x=>Number.isFinite(x.duration)&&x.duration>0)&&Math.abs(action.samples.reduce((sum,x)=>sum+x.duration,0)-action.duration)<1e-12,'SOURCE_WORK_ACTION_DOMAIN',action.id);
    if(action.id==='Chop'){const origin=common.resources.find(x=>x.id==='common.s0.4399');need(action.obligation==='AN-12'&&action.body==='CH-01'&&action.sourceClip==='common.s0.4399'&&origin?.source.type==='Sprite','SOURCE_WORK_CLIP_REFERENCE',action.id);retainedMember(proof,'retained/common/'+origin.source.rawMember,origin.source.rawSha256);retainedMember(proof,'retained/common/'+origin.member,origin.pngSha256);}
    else need(action.sourceClip===null||clips.has(action.sourceClip),'SOURCE_WORK_CLIP_REFERENCE',action.id);
    need(action.tool===null||[...components].some(key=>key===action.tool||key.startsWith(action.tool+'.')),'SOURCE_WORK_TOOL_REFERENCE',action.id);if(action.event)need(equal(Object.keys(action.event).sort(),['sound','time'])&&Number.isFinite(action.event.time)&&action.event.time>=0&&action.event.time<=action.duration&&/^AU-\d\d$/.test(action.event.sound),'SOURCE_WORK_INERT_EVENT',action.id);}
  need(body.atlas.path==='src/godot-game/assets/characters/work-special.png'&&hash(read(body.atlas.path))===retainedMember(proof,'work-special.png',body.atlas.sha256).sha256,'SOURCE_WORK_RUNTIME_ATLAS');return{components:components.size,actions:actions.size,generatedMaster:[width,height],recipeExecution:false};
}
function retainedInterface(proof,body,actionsProof,actions,bank){
  need(body.recipes?.assemblerMember==='recipes/assemble-interface.py'&&body.recipes.assemblerSha256==='4d1b537ffe933e13c11955e9437422bddc5532a85ae5596684940ff4e2868b37','SOURCE_INTERFACE_RECIPE_IDENTITY');retainedMember(proof,body.recipes.assemblerMember,body.recipes.assemblerSha256);
  const sources=new Map();for(const source of body.sources){need(typeof source.id==='string'&&!sources.has(source.id)&&source.output&&typeof source.sourceMember==='string','SOURCE_INTERFACE_ORIGIN_IDENTITY');sources.set(source.id,source);retainedMember(proof,source.sourceMember,source.output.sha256,source.output.bytes);retainedMember(proof,'sources/'+source.id+'/audioclip.bin',source.source.rawObjectSha256);retainedMember(proof,'sources/'+source.id+'/stream.fsb',source.source.stream.sha256,source.source.stream.bytes);}
  const identities=new Map(body.samples.map(x=>[x.id,x]));need(identities.size===body.samples.length,'SOURCE_INTERFACE_VARIANT_DUPLICATE');
  const food=identities.get('interface.food-crunch.v1'),burst=identities.get('interface.bloater-burst.v1');need(food&&burst,'SOURCE_INTERFACE_SPECIAL_VARIANTS');
  for(const sample of body.samples){need(sources.has(sample.sourceId)||(sample.id===food.id&&sample.sourceId==='generated-food-crunch')||(sample.id===burst.id&&sample.sourceId==='action.2790.full'),'SOURCE_INTERFACE_VARIANT_ORIGIN',sample.id);retainedMember(proof,sample.variantMember,sample.sha256);}
  need(body.donorGap?.category==='AU-19'&&body.donorGap.authority==='specs/0.1/assets-and-resources.md section10 AU-19'&&body.donorGap.synthesis==='Deterministic non-tonal contact/crunch texture seed190021; needs real audition'&&Array.isArray(body.donorGap.unsupportedDonorUnsuitabilityClaims)&&body.donorGap.unsupportedDonorUnsuitabilityClaims.length===0,'SOURCE_INTERFACE_GENERATED_AUTHORITY');
  need(food.sourceId==='generated-food-crunch'&&food.transform==='seed190021-four-noise-transients','SOURCE_INTERFACE_GENERATED_ROLE');need(food.frames===11025&&equal(food.trimFrames,[0,11025])&&food.gainLinear===1&&food.gainDb===0,'SOURCE_INTERFACE_GENERATED_RECIPE');
  need(burst.sourceId==='action.2790.full'&&burst.transform==='shared-approved-source-new-trim-not-auditioned','SOURCE_INTERFACE_SHARED_ROLE');need(burst.frames===13230&&equal(burst.trimFrames,[0,13230])&&burst.gainLinear===0.65&&Math.abs(burst.gainDb-20*Math.log10(0.65))<1e-12,'SOURCE_INTERFACE_SHARED_RECIPE');
  need(actionsProof&&actions&&Array.isArray(body.sharedSources)&&body.sharedSources.length===1,'SOURCE_INTERFACE_SHARED_DEPENDENCY');const shared=body.sharedSources[0],matched=actions.samples.filter(x=>x.id==='action.2790.full');need(matched.length===1&&shared.id==='action.2790.full'&&shared.pack==='packs/audio-actions.json'&&shared.wavSha256===matched[0].sha256,'SOURCE_INTERFACE_SHARED_DEPENDENCY');const action=matched[0];retainedMember(actionsProof,action.variantMember,action.sha256);retainedMember(actionsProof,action.sourceMember,action.original.sha256,action.original.bytes);
  need(body.bank&&hash(bank)===body.bank.sha256,'SOURCE_INTERFACE_BANK_HASH');const actualBank=retainedPcm(bank,'actual interface runtime bank'),sourcePcm=retainedPcm(actionsProof.retained.get(action.variantMember),'actual shared action source');
  for(const sample of [food,burst]){
    need(sample.variantMember==='variants/'+sample.id+'.wav','SOURCE_INTERFACE_VARIANT_MEMBER_ROLE',sample.id);const value=proof.retained.get(sample.variantMember);need(value&&hash(value)===sample.sha256,'SOURCE_INTERFACE_VARIANT_BYTES',sample.id);const pcm=retainedPcm(value,sample.id);
    need(sample.channels===2&&sample.rate===44100&&sample.sampleWidth===2&&sample.loop===null&&sample.durationSeconds===sample.frames/44100&&pcm.length===sample.frames*4,'SOURCE_INTERFACE_VARIANT_EXTENT',sample.id);
    need(sample.listening?.status==='Not auditioned'&&sample.listening.listener===null&&sample.listening.verdict===null,'SOURCE_INTERFACE_FALSE_AUDITION',sample.id);
    need(Array.isArray(sample.regionFrames)&&sample.regionFrames.length===2&&sample.regionFrames.every(Number.isSafeInteger)&&sample.regionFrames[0]>=0&&sample.regionFrames[1]-sample.regionFrames[0]===sample.frames&&sample.regionFrames[1]<=actualBank.length/4,'SOURCE_INTERFACE_BANK_REGION',sample.id);
    need(hash(pcm)===sample.pcmSha256&&pcm.equals(actualBank.subarray(sample.regionFrames[0]*4,sample.regionFrames[1]*4)),'SOURCE_INTERFACE_PCM_REGION',sample.id);
    if(sample===burst){need(sourcePcm.length>=pcm.length,'SOURCE_INTERFACE_SHARED_SOURCE_EXTENT');for(let offset=0;offset<pcm.length;offset+=2)need(pcm.readInt16LE(offset)===retainedRound(sourcePcm.readInt16LE(offset)*0.65),'SOURCE_INTERFACE_SHARED_PCM_RECIPE',sample.id);}
  }
  return{localSources:sources.size,generated:food.id,shared:burst.id,pcmFrames:[food.frames,burst.frames],recipe:'Authenticated data only; generated PCM reconstruction has separate unchanged audio.bank proof'};
}
async function validateCompletedSources(root=ROOT,model=normalizeCurrent(root)){
  need(model.model==='completed-shards-v1'&&model.schemaVersion===1,'SOURCE_COMPLETED_MODEL');const files=new Map(),byPath=new Map();
  for(const ref of model.files){need(!byPath.has(ref.path),'SOURCE_FILE_DUPLICATE',ref.path);byPath.set(ref.path,ref);fileRef(root,ref,files);}
  const register=expected(),ids=model.outputs.map(x=>x.id);packagedExpected(root,model,register,files);need(ids.length===180&&new Set(ids).size===180&&equal([...ids].sort(),register.obligations.map(x=>x.id).sort()),'SOURCE_INDEPENDENT_OBLIGATIONS');
  const observations=[];let common,commonBody,actionsSource,interfaceSource,fontSource,workSource;
  for(const [archiveName,packName,documentMember,atlasMember]of retainedContracts){
    const archivePath='production/phase4/'+archiveName+'.zip',ref=byPath.get(archivePath);need(ref&&ref.role==='authoring','SOURCE_ARCHIVE_ROLE',archivePath);
    const capture=[documentMember];if(!['common-selected','audio-actions','audio-interface-ambience','ui-identity'].includes(archiveName))capture.push('members.json');if(archiveName==='common-selected')capture.push('preserved-manifest.json');if(archiveName==='audio-actions')capture.push('variants/action.2790.full.wav');if(archiveName==='audio-interface-ambience')capture.push('variants/interface.food-crunch.v1.wav','variants/interface.bloater-burst.v1.wav');if(archiveName==='work-special')capture.push('generated/work-effects-master.png');
    const proof=await archive(root,ref,{capture}),retained=parse(proof.retained.get(documentMember),archivePath+'#'+documentMember);
    if(proof.retained.has('members.json'))retainedInventory(proof,parse(proof.retained.get('members.json'),archivePath+'#members.json'));
    if(packName){
      const packPath='src/godot-game/content/packs/'+packName+'.json',body=document(root,packPath);retainedRecipe(proof,body,packName);retainedParts(proof,body,packName);
      if(packName==='common'){common=retainedCommon(proof,body,parse(proof.retained.get('preserved-manifest.json'),archivePath+'#preserved-manifest.json'));commonBody=body;fontSource=retainedFonts(proof,body,document(root,'src/godot-game/content/packs/ui-kit.json'),path=>bytes(root,'src/godot-game/'+path));}
      if(packName.startsWith('audio-')){
        const detached={...body};delete detached.bundleSha256;need(equal(detached,retained)&&body.bundleSha256===ref.sha256,'SOURCE_RETAINED_AUDIO_DOCUMENT',packPath);
        if(packName==='audio-actions'){for(const sample of body.samples){need(sample.original&&typeof sample.sourceMember==='string','SOURCE_ACTION_INLINE_ORIGIN',sample.id);retainedMember(proof,sample.variantMember,sample.sha256);retainedMember(proof,sample.sourceMember,sample.original.sha256,sample.original.bytes);}actionsSource={proof,body};}
        else interfaceSource=retainedInterface(proof,body,actionsSource?.proof,actionsSource?.body,bytes(root,'src/godot-game/assets/audio/interface-ambience.wav'));
      }
      else need(proof.retained.get(documentMember).equals(bytes(root,packPath)),'SOURCE_RETAINED_DOCUMENT',packPath);
      if(atlasMember){const path='src/godot-game/'+body.atlas.path.replace(/^res:\/\//,'');need(hash(bytes(root,path))===retainedMember(proof,atlasMember).sha256,'SOURCE_RETAINED_ATLAS',path);}
    }
    if(archiveName==='work-special'){const path='src/godot-game/content/animation/work-special.json',body=document(root,path);workSource=retainedWork(proof,body,commonBody,path=>bytes(root,path));need(bytes(root,path).equals(proof.retained.get(documentMember)),'SOURCE_RETAINED_WORK_DOCUMENT',path);}
    if(archiveName==='ui-identity')for(const [path,member]of [['src/godot-game/assets/ui/kingdom-zombies-icon.png','outputs/icon-256.png'],['src/godot-game/assets/ui/identity-symbols.png','outputs/identity-symbols.png'],['src/godot-game/assets/ui/kingdom-zombies.ico','outputs/kingdom-zombies.ico']])need(hash(bytes(root,path))===retainedMember(proof,member).sha256,'SOURCE_CURRENT_IDENTITY_MEMBER',path);
    observations.push({path:archivePath,members:proof.members.length,expandedBytes:proof.expandedBytes,sha256:ref.sha256,actualMemberCrcSha:true});
  }
  return{status:'retained-source-relations-validated',expectedIds:register.obligations.map(x=>x.id),archiveObservations:observations,common,interfaceSource,fontSource,workSource,files:files.size,aggregateBytes:[...files.values()].reduce((sum,x)=>sum+x.length,0),accepted:0,scope:'Actual bounded retained bytes, source identities, member/recipe/role relationships; compiled domain/native reachability and later judgments remain distinct'};
}
function normalizeCurrent(root=ROOT){
  const assert=require('node:assert/strict'),base='src/godot-game/',register=expected(),files=new Map(),documents=new Map(),definitions=new Map();
function reference(file,role){const value=bytes(root,file),id='file.'+hash(Buffer.from(file)).slice(0,20);const row={id,path:file,bytes:value.length,sha256:hash(value),version:1,role};files.set(file,row);return row;}
function walkDirectory(directory,role){for(const entry of fs.readdirSync(inside(root,directory),{withFileTypes:true})){const file=directory+'/'+entry.name;if(entry.isDirectory())walkDirectory(file,role);else if(!file.endsWith('.import')){reference(file,role);if(file.endsWith('.json'))documents.set(file,document(root,file));}}}
walkDirectory(base+'content','runtime');walkDirectory(base+'assets','runtime');
for(const file of ['inspection/ItemsWeaponsPreview.tscn','inspection/ItemsWearablesPreview.tscn'])reference(base+file,'runtime');
for(const name of fs.readdirSync(inside(root,'production/phase4'))){const file='production/phase4/'+name;if(fs.statSync(inside(root,file)).isFile())reference(file,'authoring');}
for(const file of ['tools/phase4/required-cases.json','docs/content/fields.md','docs/content/README.md','docs/content/windows-inspection.md',base+'docs/README.md',base+'project.godot',base+'inspection/Inspection.tscn'])reference(file,'source-support');
for(const [file,count,digest]of [[base+'foundation/FoundationInput.cs',3256,'55c477e859143b8c8a3429396518eb2094cece492f37df3cef6cce8fbe2d394a'],[base+'inspection/ScaleComparison.tscn',8312,'3f680a4afa2d87c68368549ae329a82baf20fcff767e1ad2f5fa48d32f753fc5']]){const actual=reference(file,'source-support');need(actual.bytes===count&&actual.sha256===digest,'SUPPORT_SOURCE_IDENTITY',file);}
for(const [file,count,digest]of [[base+'foundation/FoundationRoot.cs',23630,'5149af8375f85388877d75df564babe1a585dbda0c5ab61760a927b8db2c0193'],[base+'inspection/InspectionInput.cs',2061,'05fed457e170e39e9092fb38ce4a956990e30322c8fcb89358bf0c59c8510c0c']]){const actual=reference(file,'source-support');need(actual.bytes===count&&actual.sha256===digest,'SUPPORT_SOURCE_IDENTITY',file);}
files.delete(base+'content/pack-index.json');documents.delete(base+'content/pack-index.json');
function add(id,file,pointers,contract){assert(register.obligations.some(x=>x.id===id),'Unknown independent obligation '+id);assert(!definitions.has(id),'Duplicate definition '+id);assert(files.has(file),'Missing physical definition '+file);definitions.set(id,{file,pointers,contract});}
for(const [file,data]of documents){
 if(/^DT-\d\d$/.test(data.id)&&file.includes('/config/'))add(data.id,file,[''],'authored-domain-v1');
 if(/^TXT-\d\d$/.test(data.id)&&file.includes('/text/'))add(data.id,file,[''],'text-catalog-v1');
 if(/\/packs\/bodies-[abc]\.json$/.test(file))for(const [i,body]of data.bodies.entries())add(body.id,file,['/bodies/'+i],'body-parts-v1');
 if(/\/packs\/items-(01-10|11-24)\.json$/.test(file))for(const [i,item]of data.items.entries())add(item.id,file,['/items/'+i],'item-parts-v1');
 if(/\/packs\/(main-buildings|defenses)\.json$/.test(file))for(const [i,tier]of data.tiers.entries())add(tier.id,file,['/tiers/'+i],'structure-parts-v1');
 if(/\/packs\/audio-(actions|interface-ambience)\.json$/.test(file)){
  if(data.categories)for(const [i,category]of data.categories.entries())add(category.id,file,['/categories/'+i],'audio-bank-v1');
 }
}
const declared=['animation/locomotion-melee.json','animation/bow.json','animation/work-special.json','packs/corpses-wildlife.json','packs/guards.json','packs/world.json','packs/effects-portals.json','packs/ui-kit.json','packs/ui-identity.json','packs/ui-inventory.json','packs/ui-legacy-stash.json','packs/ui-camp.json','packs/ui-map-warnings.json','packs/ui-session.json','packs/ui-settings.json','packs/compatibility.json'];
for(const local of declared){const file=base+'content/'+local,data=documents.get(file);for(const id of data.obligations??[])if(!definitions.has(id))add(id,file,['/obligations/'+data.obligations.indexOf(id)],local.startsWith('animation/')?'animation-v1':local.startsWith('packs/ui-')?'native-layout-v1':local==='packs/compatibility.json'?'compatibility-v1':'world-parts-v1');}
const events=documents.get(base+'content/audio/events.json');for(let number=16;number<=30;number++){const id='AU-'+number;add(id,base+'content/audio/events.json',['/events'],'audio-events-v1');}
add('CH-12',base+'content/packs/player-identities.json',['/players','/shapePolygons','/controlTransfer'],'player-identities-v1');
for(const [id,file,pointers]of [['BL-10','world/map.json',['/walls','/towers']],['BL-11','packs/defenses.json',['/frames','/parts']],['BL-12','packs/defenses.json',['/frames','/parts']],['BL-13','packs/camp-access.json',['/stash']],['BL-14','packs/camp-access.json',['/layouts']]])add(id,base+'content/'+file,pointers,'world-assembly-v1');
for(const [id,file,pointers,contract]of [['DT-18','packs/text-data.json',['/documentation','/bindings'],'designer-documentation-v1'],['DT-19','packs/delivery.json',['/fixtureCatalog','/application'],'source-provenance-composition-v1'],['DT-20','animation/locomotion-melee.json',['/clips','/bindings'],'animation-library-v1'],['DT-21','world/map.json',[''],'world-assembly-library-v1'],['DT-22','packs/ui-kit.json',[''],'native-layout-library-v1'],['DT-23','audio/events.json',['/events'],'audio-event-library-v1'],['DT-26','packs/delivery.json',['/fixtureCatalog','/fixtureResources'],'fixture-catalog-v1'],['DT-27','packs/delivery.json',['/guides','/application','/platformRecords'],'desktop-source-v1']])add(id,base+'content/'+file,pointers,contract);
assert.equal(definitions.size,180);assert.deepEqual([...definitions.keys()].sort(),register.obligations.map(x=>x.id).sort());
function pointer(value,selector){for(const key of selector.split('/').slice(1))value=value[key.replaceAll('~1','/').replaceAll('~0','~')];return value;}
for(const definition of definitions.values())for(const p of definition.pointers)assert.notEqual(pointer(documents.get(definition.file),p),undefined,definition.file+p);
const direct=new Map();for(const [file,data]of documents){const refs=new Set();function scan(value){if(typeof value==='string'){let candidate=value.startsWith('res://')?base+value.slice(6):/^(content|assets|inspection)\//.test(value)?base+value:value;if(files.has(candidate))refs.add(candidate);else for(const target of files.keys())if(candidate.startsWith(target+'/')||candidate.startsWith(target+'#'))refs.add(target);}if(value&&typeof value==='object')for(const child of Object.values(value))scan(child);}scan(data);direct.set(file,refs);}
function closure(file){const seen=new Set();function follow(current){if(seen.has(current))return;seen.add(current);for(const child of direct.get(current)??[])follow(child);}follow(file);return [...seen].sort().map(x=>files.get(x).id);}
const sourceLibraries={DT19:[...files.values()].filter(x=>x.role!=='runtime').map(x=>x.id),DT20:[...files.values()].filter(x=>/\/animation\/|\/packs\/bodies-|\/packs\/corpses-wildlife|\/packs\/guards/.test(x.path)).map(x=>x.id),DT21:[...files.values()].filter(x=>/\/world\/|\/packs\/(main-buildings|defenses|guards|camp-access|world|effects-portals)/.test(x.path)).map(x=>x.id),DT22:[...files.values()].filter(x=>/\/ui\/|\/packs\/ui-/.test(x.path)).map(x=>x.id),DT23:[...files.values()].filter(x=>/\/audio\/|\/packs\/audio-/.test(x.path)).map(x=>x.id)};
const outputs=register.obligations.map(obligation=>{const d=definitions.get(obligation.id),file=files.get(d.file);return {id:obligation.id,name:obligation.name,version:1,contract:d.contract,file:file.id,definitionPointers:d.pointers,dependencies:[...new Set([...closure(d.file),...(sourceLibraries[obligation.id.replace('-','')]??[])])].sort(),sharedFileId:'shared.'+file.id,obligations:[{id:obligation.id,coverage:'candidate'}],sourceStatus:'available-unvalidated',acceptance:{native:'pending',listening:obligation.id.startsWith('AU-')?'pending':'not-applicable',judgment:'pending',accepted:false}};});
const result={schemaVersion:1,id:'kingdom-zombies.v0.1',version:1,model:'completed-shards-v1',status:'incomplete',expectedObligations:{path:PACKAGED_EXPECTED,bytes:EXPECTED_BYTES,sha256:EXPECTED_SHA256},sourceReadiness:{available:180,validated:0,accepted:0},files:[...files.values()].sort((a,b)=>a.path.localeCompare(b.path,'en')),outputs};

  const value=Buffer.from(JSON.stringify(result));need(value.length<=1048576,'NORMALIZED_DOCUMENT_BOUND');need(result.files.reduce((sum,x)=>sum+x.bytes,0)<=MAX_TOTAL,'PACK_SIZE_LIMIT');return result;
}
function validateCompleted(root,index,register,{requireComplete=false}={}){
  need(index.schemaVersion===1&&index.id==='kingdom-zombies.v0.1'&&Number.isSafeInteger(index.version)&&index.version>0&&index.version<=2147483647,'INDEX_INVALID');
  need(index.status==='incomplete','FALSE_COMPLETION');
  need(Array.isArray(index.files)&&index.files.length>0&&index.files.length<=4096&&Array.isArray(index.outputs)&&index.outputs.length<=4096,'INDEX_COLLECTIONS');
  const files=new Map(),diagnostics=[],valid=[],refs=new Map(),fileIds=new Set(),badFiles=new Set(),fileErrors=new Map();
  register=packagedExpected(root,index,register,files);
  const canonical=normalizeCurrent(ROOT),canonicalFiles=new Map(canonical.files.map(x=>[x.path,x])),canonicalOutputs=new Map(canonical.outputs.map(x=>[x.id,x]));
  for(const ref of index.files){
    try{
      text(ref.id,'file.id');need(!fileIds.has(ref.id)&&!refs.has(ref.path),'FILE_DUPLICATE',ref.path);fileIds.add(ref.id);refs.set(ref.path,ref);
      const actual=canonicalFiles.get(ref.path);need(actual&&ref.id===actual.id&&ref.role===actual.role&&ref.version===actual.version,'FILE_ROLE_IDENTITY',ref.path);
      fileRef(root,ref,files);
    }catch(error){badFiles.add(ref.id);fileErrors.set(ref.id,error.message);diagnostics.push({code:'OUTPUT_INVALID',id:ref.id,path:ref.path,message:error.message});}
  }
  const ids=new Set(),duplicates=new Set();for(const row of index.outputs){if(ids.has(row.id))duplicates.add(row.id);ids.add(row.id);}
  for(const output of index.outputs){
    try{
      const actual=canonicalOutputs.get(output.id);need(actual&&!duplicates.has(output.id),'OUTPUT_ID_DUPLICATE_OR_INVALID',output.id);
      need(equal(output,actual),'COMPLETED_OUTPUT_BINDING',output.id);
      for(const id of output.dependencies){const ref=index.files.find(x=>x.id===id);need(ref&&!badFiles.has(id),'DEPENDENCY_INVALID',output.id+' '+id+' '+(fileErrors.get(id)??''));}
      const ref=index.files.find(x=>x.id===output.file);need(ref&&!badFiles.has(ref.id),'OUTPUT_FILE_INVALID',output.id);
      valid.push({id:output.id,version:output.version,path:ref.path,sha256:ref.sha256,obligations:output.obligations});
    }catch(error){diagnostics.push({code:'OUTPUT_INVALID',id:output.id,path:index.files.find(x=>x.id===output.file)?.path,message:error.message});}
  }
  for(const ref of canonical.files)if(!refs.has(ref.path))diagnostics.push({code:'OUTPUT_INVALID',id:ref.id,path:ref.path,message:'FILE_REQUIRED '+ref.path});
  const available=new Set(valid.map(x=>x.id));for(const row of register.obligations)if(!available.has(row.id))diagnostics.push({code:'SOURCE_OBLIGATION_MISSING',id:row.id});
  need(equal(index.sourceReadiness,{available:180,validated:0,accepted:0}),'SOURCE_READINESS_UNPROVEN');
  need(!requireComplete,'PACK_INCOMPLETE');
  return{status:'incomplete',complete:false,sourceComplete:available.size===180&&!diagnostics.length,sourceReady:[...available].sort(),valid,diagnostics,covered:[],accepted:0,expected:register.obligations.length,files};
}
function validate(root,index=document(root,INDEX),register=expected(),{requireComplete=false}={}){
  if(index?.model==='completed-shards-v1')return validateCompleted(root,index,register,{requireComplete});
  need(index&&index.schemaVersion===1&&index.id==='kingdom-zombies.v0.1'&&Number.isSafeInteger(index.version)&&index.version>0,'INDEX_INVALID');
  need(Array.isArray(index.shards)&&index.shards.length>0&&index.shards.length<=128&&Array.isArray(index.outputs)&&index.outputs.length<=4096,'INDEX_COLLECTIONS');
  need(index.status==='incomplete'||index.status==='complete','INDEX_STATUS');
  const files=new Map(),origins=index.origins;need(Array.isArray(origins)&&origins.length<=8192,'ORIGINS_REQUIRED');
  register=packagedExpected(root,index,register,files);
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
          for(const [type,id,name]of [['SpriteAtlas',production.source.atlasId,production.source.atlasName],['Texture2D',production.source.textureId,production.source.textureName]])
            if(id!==undefined)need(origin.dependencies.some(x=>x.owner===origin.owner&&x.type===type&&x.objectId===id&&x.name===name),'SOURCE_DEPENDENCY_MISSING',output.id+' '+type);
          need(!production.source.stream||origin.streams.length===1,'SOURCE_STREAM_MISSING',output.id);
          for(const stream of origin.streams){digest(stream.rangeSha256,'stream.rangeSha256');
            need(production.preservation.streamRangeSha256===stream.rangeSha256&&production.source.stream?.path===stream.path&&
              production.source.stream.offset===stream.offset&&production.source.stream.size===stream.bytes,'PRODUCTION_STREAM_BINDING',output.id);}
        }
      }
      need(production.id===output.id&&production.version===output.version,'PRODUCTION_IDENTITY',output.id);
      need(output.route!=='reuse'||(production.route==='reused-native-pixels'&&Buffer.from(production.bundle?.originalExportPngBase64??'','base64').equals(value)),'REUSE_CHANGED_BYTES',output.id);
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
  const absolute=path.resolve(root),repo=fs.realpathSync.native(ROOT),rel=logical(path.relative(repo,absolute).split(path.sep).join('/'));
  need(rel.startsWith(runsPath()+'/')||rel.startsWith(tempPath()+'/')||rel.startsWith('production/phase4/published/'),
    'PUBLICATION_ROOT_DENIED',rel);
  relative(rel);
  // Write where the reparse and escape walk ran: the physical folder layout.cjs resolves `rel` to.
  return inside(repo,rel);
}
function write(root,name,value){
  const file=inside(root,name);fs.mkdirSync(path.dirname(file),{recursive:true});const fd=fs.openSync(file,'wx');
  try{fs.writeFileSync(fd,Buffer.isBuffer(value)?value:Buffer.from(JSON.stringify(value,null,2)+'\n'));fs.fsyncSync(fd);}finally{fs.closeSync(fd);}
}
function takePublicationOwnership(root,name,invocation,conflict){
  const owner={invocation,pid:process.pid,token:crypto.randomUUID()};
  const temporary='publication-owner-'+owner.token+'.json',lock=inside(root,name);
  try{
    write(root,temporary,owner);
    try{fs.linkSync(inside(root,temporary),lock);}
    catch(error){if(error.code==='EEXIST')throw new Error(conflict+' '+name);throw error;}
  }finally{
    const file=inside(root,temporary);if(fs.existsSync(file))fs.unlinkSync(file);
  }
  return owner;
}
function publicationOwner(root,name){
  const fd=fs.openSync(inside(root,name),'r');
  try{
    const stat=fs.fstatSync(fd);
    need(stat.isFile()&&stat.size>0&&stat.size<=512,'FILE_INVALID',name);
    const owner=parse(fs.readFileSync(fd),name);
    need(typeof owner?.invocation==='string'&&/^[a-z0-9-]{1,80}$/.test(owner.invocation)&&
      Number.isSafeInteger(owner.pid)&&owner.pid>0&&
      (owner.token===undefined||(typeof owner.token==='string'&&/^[a-f0-9-]{36}$/.test(owner.token))),
      'RECOVERY_OWNER',name);
    if(stat.nlink!==1){
      const temporary=inside(root,'publication-owner-'+owner.token+'.json');
      const same=fs.statSync(temporary,{throwIfNoEntry:false});
      need(stat.nlink===2&&owner.token&&(same?stat.dev===same.dev&&stat.ino===same.ino:fs.fstatSync(fd).nlink===1),
        'FILE_INVALID',name);
    }
    return owner;
  }finally{fs.closeSync(fd);}
}
function releasePublicationOwnership(root,name,owner){
  need(equal(publicationOwner(root,name),owner),'LOCK_OWNERSHIP',name);
  fs.unlinkSync(inside(root,name));
}
function publish({sourceRoot,index,publicationRoot,invocation=crypto.randomUUID(),observe=()=>{}}){
  if(index?.model==='completed-shards-v1')return publishCompleted({sourceRoot,index,publicationRoot,invocation,observe});
  need(/^[a-z0-9-]{1,80}$/.test(invocation),'INVOCATION_INVALID');const root=destination(publicationRoot);fs.mkdirSync(root,{recursive:true});
  const owner=takePublicationOwnership(root,'publication.lock',invocation,'PUBLICATION_LOCKED');
  const stage='stage-'+invocation,generation='generations/'+invocation,pointer='current-'+invocation+'.json';let committed=false,moved=false;
  try{
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
    releasePublicationOwnership(root,'publication.lock',owner);
  }
}
async function publishCompleted({sourceRoot,index,publicationRoot,invocation,observe}){
  need(/^[a-z0-9-]{1,80}$/.test(invocation),'INVOCATION_INVALID');const root=destination(publicationRoot);fs.mkdirSync(root,{recursive:true});
  const owner=takePublicationOwnership(root,'publication.lock',invocation,'PUBLICATION_LOCKED');
  const stage='stage-'+invocation,generation='generations/'+invocation,pointer='current-'+invocation+'.json';let committed=false,moved=false;
  try{
    observe('locked');
    const checked=validate(sourceRoot,structuredClone(index));need(checked.sourceComplete,'PUBLICATION_INVALID_OUTPUT');
    const sourceProof=await validateCompletedSources(sourceRoot,index);
    if(fs.existsSync(inside(root,'current.json'))){const previous=readPublished(root);need(previous.index.model==='completed-shards-v1'&&previous.index.outputs.every(x=>index.outputs.some(y=>equal(x,y))),'PUBLICATION_REGRESSION');}
    const staged=inside(root,stage);fs.mkdirSync(staged);write(staged,INDEX,index);for(const [name,value]of checked.files)write(staged,name,value);
    observe('staged');const proof=validate(staged,document(staged,INDEX));need(proof.sourceComplete,'STAGED_INVALID');
    const retained=await validateCompletedSources(staged,index);need(equal(retained,sourceProof),'STAGED_SOURCE_CHANGED');observe('validated');
    fs.mkdirSync(inside(root,'generations'),{recursive:true});need(!fs.existsSync(inside(root,generation)),'INVOCATION_REUSED');fs.renameSync(staged,inside(root,generation));moved=true;
    write(root,pointer,{schemaVersion:1,invocation,generation,indexSha256:hash(bytes(inside(root,generation),INDEX)),status:proof.status});observe('before-commit');fs.renameSync(inside(root,pointer),inside(root,'current.json'));committed=true;
    return{invocation,generation,status:proof.status,complete:false,sourceComplete:true,accepted:0,sourceProof,outputs:proof.valid.map(({id,version,sha256})=>({id,version,sha256}))};
  }finally{
    if(fs.existsSync(inside(root,pointer)))fs.unlinkSync(inside(root,pointer));const cleanup=moved?generation:stage;if(!committed&&fs.existsSync(inside(root,cleanup)))fs.rmSync(inside(root,cleanup),{recursive:true});
    releasePublicationOwnership(root,'publication.lock',owner);
  }
}
function readPublished(publicationRoot){
  const root=destination(publicationRoot),pointer=document(root,'current.json');
  need(pointer.schemaVersion===1&&pointer.generation==='generations/'+pointer.invocation,'PUBLICATION_POINTER');
  const generation=inside(root,pointer.generation),value=bytes(generation,INDEX);need(hash(value)===pointer.indexSha256,'PUBLICATION_INDEX_HASH');
  const index=parse(value,INDEX),result=validate(generation,index);need(!result.diagnostics.some(x=>x.code==='OUTPUT_INVALID')&&result.status===pointer.status&&(index.model!=='completed-shards-v1'||result.sourceComplete),'PUBLICATION_INVALID');
  return {pointer,index,result};
}
function requireDeadOwner(owner,code){
  try{process.kill(owner.pid,0);}
  catch(error){if(error.code==='ESRCH')return;throw error;}
  throw new Error(code+' '+owner.pid);
}
function claimPublicationRecovery(root,owner){
  let previous=owner;
  const interrupted=[];
  // Each claim is exclusive for a specific owner token. A dead claimant can itself be claimed.
  while(interrupted.length<16){
    const name='publication-recovery-'+(previous.token??hash(JSON.stringify(previous)))+'.json';
    try{
      return {name,claim:takePublicationOwnership(root,name,owner.invocation,'RECOVERY_LOCKED'),interrupted};
    }catch(error){if(!error.message.startsWith('RECOVERY_LOCKED '))throw error;}
    previous=publicationOwner(root,name);
    need(previous.invocation===owner.invocation,'RECOVERY_LOCK_CHANGED',name);
    requireDeadOwner(previous,'RECOVERY_LOCKED');
    interrupted.push({name,owner:previous});
  }
  throw new Error('RECOVERY_CLAIM_LIMIT');
}
function recoverPublication({publicationRoot,invocation,observe=()=>{}}){
  const root=destination(publicationRoot),owner=publicationOwner(root,'publication.lock');
  need(owner.invocation===invocation,'RECOVERY_OWNER');
  requireDeadOwner(owner,'RECOVERY_OWNER_ALIVE');
  observe('owner-read');
  const {name,claim,interrupted}=claimPublicationRecovery(root,owner);
  try{
    need(equal(publicationOwner(root,'publication.lock'),owner),'RECOVERY_LOCK_CHANGED');
    const current=fs.existsSync(inside(root,'current.json'))?readPublished(root):undefined;
    for(const name of ['stage-'+invocation,'current-'+invocation+'.json',...(current?.pointer.invocation===invocation?[]:['generations/'+invocation])]){
      const file=inside(root,name);if(fs.existsSync(file))fs.rmSync(file,{recursive:true});
    }
    need(equal(publicationOwner(root,'publication.lock'),owner),'RECOVERY_LOCK_CHANGED');
    observe('before-unlock');
    fs.unlinkSync(inside(root,'publication.lock'));
    for(const prior of interrupted.reverse())releasePublicationOwnership(root,prior.name,prior.owner);
    return {invocation,status:'interrupted',published:current?.pointer.invocation===invocation,cleanup:'owned-stage-pointer-and-unreferenced-generation-only'};
  }finally{releasePublicationOwnership(root,name,claim);}
}
async function worker(args){
  need(args.length===5&&args[0]==='--publish-worker','WORKER_USAGE');
  const [,sourceRoot,publicationRoot,invocation,pauseAt]=args;
  need(['none','locked','staged','validated','before-commit','compete'].includes(pauseAt),'WORKER_PAUSE');
  const index=document(sourceRoot,INDEX);
  const result=await publish({sourceRoot,index,publicationRoot,invocation,observe:step=>{
    if(step==='locked'&&pauseAt==='compete'){
      const competitor=spawnSync(process.execPath,[__filename,'--publish-worker',sourceRoot,publicationRoot,invocation+'-peer','none'],
        {cwd:ROOT,windowsHide:true,encoding:'utf8',timeout:5000,maxBuffer:65536,shell:false});
      need(competitor.status===1&&!competitor.error&&competitor.stderr.includes('PUBLICATION_LOCKED'),'LIVE_COMPETITOR_RESULT');
      need(document(publicationRoot,'publication.lock').pid===process.pid,'LIVE_OWNER_LOCK_CHANGED');
      write(publicationRoot,'live-competition.json',{ownerPid:process.pid,invocation,subjectStatus:'fail',code:competitor.status,
        stderr:competitor.stderr,stdout:competitor.stdout,observed:'PUBLICATION_LOCKED while owner is alive'});
    }
    if(step===pauseAt){process.stdout.write('PUBLICATION_PAUSED '+invocation+' '+step+'\n');
      Atomics.wait(new Int32Array(new SharedArrayBuffer(4)),0,0,30000);throw new Error('PAUSE_DEADLINE '+invocation);}
  }});
  console.log(JSON.stringify(result));
}
module.exports={ROOT,INDEX,runsPath,tempPath,feedPath,legacyLocal,legacyShared,sharedFile,sharedBytes,EXPECTED,PACKAGED_EXPECTED,EXPECTED_BYTES,EXPECTED_SHA256,hash,need,text,relative,inside,bytes,parse,document,expected,packagedExpected,sourceKey,resolveOrigin,validate,merge,destination,write,publish,readPublished,recoverPublication,worker,normalizeCurrent,archive,authenticateArchive,archiveBounds,validateCompletedSources,retainedCommon,retainedRecipe,retainedParts,retainedInterface,retainedFonts,retainedWork};
if(require.main===module)worker(process.argv.slice(2)).catch(error=>{console.error(error.stack);process.exitCode=1;});
