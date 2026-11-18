'use strict';
const fs=require('node:fs'),assert=require('node:assert/strict');
const content=require('../content.cjs'),runner=require('../check.cjs');
const {ROOT,inside,bytes,hash,need,write}=content;
const INTERFACE='production/phase4/audio-interface-ambience.zip',MIB=1048576;
const metadata={group:'content',importance:'Critical',rationale:'A source archive exception applied to an alias or unchecked bytes breaks bounded publication identity',needsGameWindow:false,runGroup:'G0-content',order:60,requirements:['ASSET-01','ASSET-02','ASSET-04','ASSET-05'],acceptance:['AC-13','AC-15'],setup:'Actual retained canonical interface archive and independently constructed boundary subjects; all other source caps remain32MiB',actions:['Authenticate actual canonical source bytes and exact recorded size/hash','Exercise exact64MiB and64MiB+1 canonical reads independently','Reject wrongcase/lookalike/default-path oversize and wronghash source identities','Restore the original real archive and preserve every production byte'],expected:'Only the exact actual source identity receives the bounded64MiB exception; all invalid subjects fail visibly',reset:'Independent invocation-owned scratch subjects, never mutate retained production archive',evidence:['inputs.json','boundaries.json','result.json']};
function sourceRef(root,path,sha256){const value=bytes(root,path);need(hash(value)===sha256,'SOURCE_BOUNDARY_HASH',path);return value;}
async function sourceBoundaries(context){
 const before={path:INTERFACE,bytes:fs.statSync(inside(ROOT,INTERFACE)).size,sha256:hash(fs.readFileSync(inside(ROOT,INTERFACE)))};
 write(context.directory,'inputs.json',[before]);need(before.bytes===64820348&&before.sha256==='cfa52aeac32a22fcf4898e05fa64a6225016af404ae990632ffd07fb130fb1d2','SOURCE_BOUNDARY_AUTHORITY');
 need(sourceRef(ROOT,INTERFACE,before.sha256).length===before.bytes,'SOURCE_BOUNDARY_REAL');
 const root=inside(context.directory,'subjects');fs.mkdirSync(root);const file=inside(root,INTERFACE);fs.mkdirSync(require('node:path').dirname(file),{recursive:true});
 const controls=[];const exact=Buffer.alloc(64*MIB,0);fs.writeFileSync(file,exact);need(sourceRef(root,INTERFACE,hash(exact)).length===64*MIB,'SOURCE_BOUNDARY_EXACT_LIMIT');controls.push({id:'canonical-exact64MiB',accepted:true,bytes:exact.length});
 fs.appendFileSync(file,Buffer.from([0]));assert.throws(()=>bytes(root,INTERFACE),/FILE_INVALID/);controls.push({id:'canonical-over64MiB',rejected:true,bytes:64*MIB+1});
 fs.writeFileSync(file,exact);assert.throws(()=>sourceRef(root,INTERFACE,'0'.repeat(64)),/SOURCE_BOUNDARY_HASH/);controls.push({id:'canonical-wrong-hash',rejected:true});
 for(const [id,sha256,code]of [['publisher-wrong-hash','0'.repeat(64),'FILE_HASH_MISMATCH'],['publisher-unauthenticated',undefined,'HASH_REQUIRED']]){const candidate=content.document(ROOT,content.INDEX);Object.assign(candidate.outputs[0],{path:INTERFACE,bytes:before.bytes,sha256});const proof=content.validate(ROOT,candidate);need(proof.diagnostics.some(x=>x.code==='OUTPUT_INVALID'&&x.message.includes(code)),'SOURCE_BOUNDARY_PRODUCTION_AUTH',id);controls.push({id,rejected:true,productionDiagnostic:code});}
 assert.throws(()=>bytes(root,'production/phase4/Audio-interface-ambience.zip'),/PATH_CASE/);controls.push({id:'wrongcase-canonical',rejected:true});
 for(const path of ['production/phase4/audio-interface-ambience-copy.zip','production/phase4/other.zip']){const wrong=inside(root,path);fs.writeFileSync(wrong,Buffer.alloc(32*MIB+1,0));assert.throws(()=>bytes(root,path),/FILE_INVALID/);controls.push({id:path.endsWith('-copy.zip')?'lookalike-over32MiB':'default-over32MiB',rejected:true});fs.unlinkSync(wrong);}
 const other=inside(root,'production/phase4/other.zip');fs.writeFileSync(other,Buffer.alloc(32*MIB,0));need(bytes(root,'production/phase4/other.zip').length===32*MIB,'SOURCE_BOUNDARY_DEFAULT_LIMIT');controls.push({id:'default-exact32MiB',accepted:true});
 fs.writeFileSync(file,fs.readFileSync(inside(ROOT,INTERFACE)));need(sourceRef(root,INTERFACE,before.sha256).length===before.bytes,'SOURCE_BOUNDARY_RESTORED');need(hash(fs.readFileSync(inside(ROOT,INTERFACE)))===before.sha256,'SOURCE_BOUNDARY_PRODUCTION_CHANGED');controls.push({id:'restored-real-canonical',accepted:true});write(context.directory,'boundaries.json',controls);
 return {actual:before,controls:controls.length,defaultSourceMiB:32,exactCanonicalSourceMiB:64,memberMiB:32,expandedMiB:128,aggregateMiB:256,scope:'Source byte boundary only; no all180/native-policy/acceptance credit'};
}
runner.registerCase('content.source-boundaries',{...metadata,title:'Exact canonical source archive compatibility with independent boundary controls'},sourceBoundaries);
module.exports={sourceBoundaries};

const COMMON='production/phase4/common-selected.zip',COMMON_HASH='2f66618305221d32d503bba04be755ae38e606408a7c5d9d8f65a1db19d9e6d9';
const COMMON_MEMBER='streams/sharedassets0.assets.resS/167471920-67108864.bin',COMMON_MEMBER_HASH='dc0a7a109f52fbadf7249a714e8b5116d57c5af4a3e86f43befd4fca4545a0e4';
function storedZip(entries){
 const chunks=[],central=[];let offset=0;
 for(const entry of entries){const name=Buffer.from(entry.name),data=Buffer.from(entry.data??'123456789'),local=Buffer.alloc(30),header=Buffer.alloc(46),size=entry.bytes??data.length,crc=entry.crc??0xcbf43926;local.writeUInt32LE(0x04034b50);local.writeUInt16LE(20,4);local.writeUInt32LE(crc,14);local.writeUInt32LE(data.length,18);local.writeUInt32LE(size,22);local.writeUInt16LE(name.length,26);header.writeUInt32LE(0x02014b50);header.writeUInt16LE(20,4);header.writeUInt16LE(20,6);header.writeUInt32LE(crc,16);header.writeUInt32LE(data.length,20);header.writeUInt32LE(size,24);header.writeUInt16LE(name.length,28);header.writeUInt32LE(offset,42);chunks.push(local,name,data);central.push(header,name);offset+=local.length+name.length+data.length;}
 const directory=Buffer.concat(central),end=Buffer.alloc(22);end.writeUInt32LE(0x06054b50);end.writeUInt16LE(entries.length,8);end.writeUInt16LE(entries.length,10);end.writeUInt32LE(directory.length,12);end.writeUInt32LE(offset,16);return Buffer.concat([...chunks,directory,end]);
}
async function archiveBoundaries(context){
 need(typeof content.archive==='function'&&typeof content.authenticateArchive==='function'&&typeof content.archiveBounds==='function','ARCHIVE_READER_PENDING');
 const ref={path:COMMON,bytes:12308215,sha256:COMMON_HASH},auth=await content.authenticateArchive(ROOT,ref),controls=[],bound=(id,token,value,code)=>{if(code){assert.throws(()=>content.archiveBounds(token,value),new RegExp(code));controls.push({id,rejected:true,code});}else{content.archiveBounds(token,value);controls.push({id,accepted:true});}};
 bound('common-exact-expanded',auth,{name:'unit.bin',bytes:1,expanded:279502632});bound('common-over-expanded',auth,{name:'unit.bin',bytes:1,expanded:279502633},'ARCHIVE_EXPANDED_LIMIT');
 bound('common-exact-member',auth,{name:COMMON_MEMBER,bytes:67108864,expanded:67108864,sha256:COMMON_MEMBER_HASH});bound('common-over-member',auth,{name:COMMON_MEMBER,bytes:67108865,expanded:67108865},'ARCHIVE_MEMBER_LIMIT');
 for(const [id,name]of [['wrong-member-name',COMMON_MEMBER+'-copy'],['wrong-member-case',COMMON_MEMBER.replace('streams/','Streams/')]])bound(id,auth,{name,bytes:67108864,expanded:67108864},'ARCHIVE_MEMBER_LIMIT');
 bound('common-member-wrong-hash',auth,{name:COMMON_MEMBER,bytes:67108864,expanded:67108864,sha256:'0'.repeat(64)},'ARCHIVE_MEMBER_IDENTITY');
 bound('caller-auth-text-rejected',{path:COMMON,sha256:COMMON_HASH,common:true},{name:COMMON_MEMBER,bytes:67108864,expanded:67108864},'ARCHIVE_AUTH_REQUIRED');
 const root=inside(context.directory,'archive-subjects');fs.mkdirSync(root);const subject=(name,value)=>{const target=inside(root,name);fs.mkdirSync(require('node:path').dirname(target),{recursive:true});fs.writeFileSync(target,value);return {path:name,bytes:value.length,sha256:hash(value)};};
 const generic=subject('production/phase4/control.zip',storedZip([{name:'valid.txt'}])),ordinary=await content.authenticateArchive(root,generic);
 bound('other-exact-expanded',ordinary,{name:'valid.txt',bytes:1,expanded:128*MIB});bound('other-over-expanded',ordinary,{name:'valid.txt',bytes:1,expanded:128*MIB+1},'ARCHIVE_EXPANDED_LIMIT');bound('other-exact-member',ordinary,{name:COMMON_MEMBER,bytes:32*MIB,expanded:32*MIB});bound('other-over-member',ordinary,{name:COMMON_MEMBER,bytes:32*MIB+1,expanded:32*MIB+1},'ARCHIVE_MEMBER_LIMIT');
 const genericProof=await content.archive(root,generic);need(genericProof.members.length===1&&genericProof.members[0].crc32==='cbf43926'&&genericProof.members[0].sha256==='15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225','ARCHIVE_KNOWN_VECTOR');controls.push({id:'known-independent-crc-sha-vector',pass:true});
 for(const [id,entries,code]of [['actual-crc',[{name:'valid.txt',crc:0}],'ARCHIVE_MEMBER_CRC'],['actual-tampered-data',[{name:'valid.txt',data:'123456788'}],'ARCHIVE_MEMBER_CRC'],['actual-short-declaration',[{name:'valid.txt',bytes:8}],'ARCHIVE_MEMBER_SIZE'],['unsafe-member',[{name:'../escape.txt'}],'PATH_INVALID'],['duplicate-member',[{name:'same.txt'},{name:'same.txt'}],'ARCHIVE_MEMBER_DUPLICATE'],['case-alias-member',[{name:'same.txt'},{name:'Same.txt'}],'ARCHIVE_MEMBER_ALIAS']]){const candidate=subject('production/phase4/'+id+'.zip',storedZip(entries));await assert.rejects(()=>content.archive(root,candidate),new RegExp(code));controls.push({id,rejected:true,code});}
 await assert.rejects(()=>content.archive(ROOT,{...ref,sha256:'0'.repeat(64)}),/ARCHIVE_SOURCE_IDENTITY|FILE_HASH_MISMATCH/);controls.push({id:'canonical-full-hash-required',rejected:true});
 await assert.rejects(()=>content.archive(ROOT,{...ref,path:COMMON.replace('common-selected','Common-selected')}),/PATH_CASE/);controls.push({id:'canonical-container-case',rejected:true});
 const actual=fs.readFileSync(inside(ROOT,COMMON)),lookalike=subject('production/phase4/common-selected-copy.zip',actual);await assert.rejects(()=>content.archive(root,lookalike),/ARCHIVE_MEMBER_LIMIT|ARCHIVE_EXPANDED_LIMIT/);controls.push({id:'canonical-lookalike-no-exception',rejected:true});
 const copy=subject(COMMON,actual),tampered=Buffer.from(actual);tampered[0]^=1;fs.writeFileSync(inside(root,COMMON),tampered);await assert.rejects(()=>content.archive(root,copy),/FILE_HASH_MISMATCH/);controls.push({id:'canonical-tamper-before-parse',rejected:true});fs.writeFileSync(inside(root,COMMON),actual);
 const common=await content.archive(root,copy);need(common.members.length===2395&&common.expandedBytes===279502632&&common.members.find(x=>x.path===COMMON_MEMBER)?.sha256===COMMON_MEMBER_HASH,'ARCHIVE_ACTUAL_COMMON');controls.push({id:'restored-real-common',pass:true});
 const other=[];for(const row of ARCHIVE_SUBJECTS){const proof=await content.archive(ROOT,row);need(proof.members.length===row.members&&proof.expandedBytes===row.expandedBytes,'ARCHIVE_ACTUAL_SUBJECT',row.path);other.push({path:row.path,members:proof.members.length,expandedBytes:proof.expandedBytes,streamed:true});}
 write(context.directory,'archive-controls.json',controls);write(context.directory,'archive-members.json',common);write(context.directory,'other-archives.json',other);need(hash(fs.readFileSync(inside(ROOT,COMMON)))===COMMON_HASH,'ARCHIVE_SOURCE_CHANGED');return {common:{members:2395,expandedBytes:279502632,oversizedMember:COMMON_MEMBER},otherArchives:other.length,controls:controls.length,scope:'Bounded actual source archive authentication only; full180/native-policy/acceptance remain pending'};
}
runner.registerCase('content.archive-boundaries',{...metadata,title:'Exact authenticated retained common archive with streamed CRC/hash and independent cap controls',order:61,setup:'Actual retained common and13other archives, immutable pinned container/member identities and independent small ZIP subjects',actions:['Authenticate complete source file before parsing/inflation','Stream all real members through the production cap/CRC/hash validator','Target the same production bound enforcer with exact/over and unauthorized-member subjects','Reject actual corrupt CRC/data, unsafe/duplicate/case aliases, fullhash tamper and lookalike containers then restore'],evidence:['archive-controls.json','archive-members.json','other-archives.json','result.json']},archiveBoundaries);
Object.assign(module.exports,{archiveBoundaries});

const ARCHIVE_SUBJECTS=[
  {
    "path": "production/phase4/audio-actions.zip",
    "bytes": 15897100,
    "sha256": "7febdb1994bfde067f83545b751bc9f23763869609f7ab8e9273e54f4992da23",
    "members": 215,
    "expandedBytes": 27520973
  },
  {
    "path": "production/phase4/audio-interface-ambience.zip",
    "bytes": 64820348,
    "sha256": "cfa52aeac32a22fcf4898e05fa64a6225016af404ae990632ffd07fb130fb1d2",
    "members": 125,
    "expandedBytes": 81440487
  },
  {
    "path": "production/phase4/bodies-a.zip",
    "bytes": 5822666,
    "sha256": "69a2b92e2a41e39d74aa3a4b00ac7d6d4568cc962863613931d6fc1b103a90e6",
    "members": 232,
    "expandedBytes": 6861840
  },
  {
    "path": "production/phase4/bodies-b.zip",
    "bytes": 5200509,
    "sha256": "77960fcbc099b9c883e50aa2e91f85f446086705ffa7ee353ec0cc89e02e9491",
    "members": 242,
    "expandedBytes": 6123929
  },
  {
    "path": "production/phase4/bodies-c.zip",
    "bytes": 4168890,
    "sha256": "f0c6949b5f3e0ffedde9b4b86aec8d33d70f0478ca5ac75c1c12e8499b1f1bb6",
    "members": 176,
    "expandedBytes": 4881873
  },
  {
    "path": "production/phase4/defenses.zip",
    "bytes": 3572582,
    "sha256": "976bd0d43ff383ac9f7a0193e0755748fd1044eaa1d62309ee6f3a68a314d312",
    "members": 124,
    "expandedBytes": 8199139
  },
  {
    "path": "production/phase4/items-01-10.zip",
    "bytes": 3122690,
    "sha256": "79ea6e5de7b45c463a1f2b31917dedc9f8e596b074ef7ed4effe57b4dcdbad59",
    "members": 58,
    "expandedBytes": 4060204
  },
  {
    "path": "production/phase4/items-11-24.zip",
    "bytes": 15845426,
    "sha256": "ef4cdcb6a9736a52864d6a4e807ab172805d293bf5b9cd88c485b4280dd681a6",
    "members": 127,
    "expandedBytes": 18729203
  },
  {
    "path": "production/phase4/main-buildings.zip",
    "bytes": 6552234,
    "sha256": "a0bde48040dea1d681f78a92910e06b4060e2b03874ca64b8023626a9934458d",
    "members": 153,
    "expandedBytes": 7329916
  },
  {
    "path": "production/phase4/scale-pilot.zip",
    "bytes": 5354772,
    "sha256": "da26ee892775c11871c1614dd917695524cb2e102b338ad33611431d956ef753",
    "members": 39,
    "expandedBytes": 5954113
  },
  {
    "path": "production/phase4/ui-identity.zip",
    "bytes": 558233,
    "sha256": "718f34b2f8a3b2803b94a9bf6590faa768fb18e3a0b515d43edb5dcd0d92d315",
    "members": 27,
    "expandedBytes": 720659
  },
  {
    "path": "production/phase4/work-special.zip",
    "bytes": 5585279,
    "sha256": "db77078931722459e8300420d43fb52d7742a9c2368f6104625d6f634e1cf9db",
    "members": 56,
    "expandedBytes": 28908283
  },
  {
    "path": "production/phase4/world.zip",
    "bytes": 1441185,
    "sha256": "a00640d1e5cdc075f533f778fe863ade2bbc8255dcfaba6bb575b02fcf3bffb7",
    "members": 212,
    "expandedBytes": 4621824
  }
];
