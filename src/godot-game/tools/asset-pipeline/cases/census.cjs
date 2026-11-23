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
 for(const [id,sha256,code]of [['publisher-wrong-hash','0'.repeat(64),'FILE_HASH_MISMATCH'],['publisher-unauthenticated',undefined,'HASH_REQUIRED']]){const candidate=require('./provenance.cjs').legacyFixture().index;Object.assign(candidate.outputs[0],{path:INTERFACE,bytes:before.bytes,sha256});const proof=content.validate(ROOT,candidate);need(proof.diagnostics.some(x=>x.code==='OUTPUT_INVALID'&&x.message.includes(code)),'SOURCE_BOUNDARY_PRODUCTION_AUTH',id);controls.push({id,rejected:true,productionDiagnostic:code});}
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

async function retainedSources(context){
 const model=content.normalizeCurrent(ROOT);write(context.directory,'inputs.json',{files:model.files,modelSha256:hash(Buffer.from(JSON.stringify(model))),source:['tools/phase4/content.cjs','tools/phase4/cases/census.cjs'].map(path=>({path,sha256:hash(bytes(ROOT,path))}))});
 const initial=await content.validateCompletedSources(ROOT,model);write(context.directory,'retained-positive.json',initial);
 const ref={path:COMMON,bytes:12308215,sha256:COMMON_HASH},proof=await content.archive(ROOT,ref,{capture:['preserved-manifest.json']}),manifest=content.parse(proof.retained.get('preserved-manifest.json'),'actual retained common preservation manifest'),common=content.document(ROOT,'src/godot-game/content/packs/common.json'),controls=[];
 const source=(id,mutate,reason)=>{const changed=structuredClone(common);mutate(changed);assert.throws(()=>content.retainedCommon(proof,changed,manifest),new RegExp(reason));controls.push({id,rejected:true,reason,guard:'Same production retainedCommon invoked after actual authenticated archive CRC/SHA, no test-owned validator'});};
 const sprites=common.resources.filter(x=>x.source?.type==='Sprite'),first=common.resources.indexOf(sprites[0]),second=common.resources.indexOf(sprites[1]);
 source('source-id-other-real-object',p=>{p.resources[first].source.objectId=p.resources[second].source.objectId;},'SOURCE_RAW_IDENTITY');
 source('source-raw-other-valid-member',p=>{p.resources[first].source.rawMember=p.resources[second].source.rawMember;p.resources[first].source.rawSha256=p.resources[second].source.rawSha256;},'SOURCE_RAW_IDENTITY');
 source('source-owner-other-valid-owner',p=>{p.resources[first].source.owner='sharedassets0.assets';},'SOURCE_OWNER_IDENTITY');
 source('source-identity-name',p=>{p.resources[first].source.name=p.resources[second].source.name;},'SOURCE_RESOURCE_IDENTITY');
 source('source-resource-duplicate',p=>{p.resources.push(structuredClone(p.resources[first]));},'SOURCE_RESOURCE_DUPLICATE');
 source('source-decoded-other-valid-image',p=>{p.resources[first].member=p.resources[second].member;p.resources[first].pngSha256=p.resources[second].pngSha256;},'SOURCE_DECODED_MEMBER_IDENTITY');
 source('source-stale-raw-hash',p=>{p.resources[first].source.rawSha256='0'.repeat(64);},'SOURCE_MEMBER_HASH');
 source('source-unsafe-object-id',p=>{p.resources[first].source.objectId=9007199254740992;},'SOURCE_OBJECT_IDENTITY');
 source('source-callback-executable',p=>{p.clips[0].callbacksExecutable=true;},'SOURCE_CALLBACK_EXECUTION');
 source('source-event-foreign-callback',p=>{p.clips[0].events=[{id:'damage.apply',execution:'presentation-data-only'}];},'SOURCE_CALLBACK_EXECUTION');
 source('source-clip-time-reversal',p=>{p.clips[0].stop=p.clips[0].start-1;},'SOURCE_CLIP_RANGE');
 const streamRow=common.resources.findIndex(x=>x.dependencies?.some(d=>d.stream));
 source('source-stream-outside-original',p=>{p.resources[streamRow].dependencies.find(d=>d.stream).stream.offset=9007199254740991;},'SOURCE_STREAM_RANGE');
 source('source-stream-other-member',p=>{p.resources[streamRow].dependencies.find(d=>d.stream).stream.member=COMMON_MEMBER;},'SOURCE_STREAM_IDENTITY');
 source('source-stream-wrong-hash',p=>{p.resources[streamRow].dependencies.find(d=>d.stream).stream.sha256='0'.repeat(64);},'SOURCE_MEMBER_HASH');
 content.retainedCommon(proof,common,manifest);controls.push({id:'restored-real-common-source',pass:true});
 const body=content.document(ROOT,'src/godot-game/content/packs/bodies-a.json'),bodyRef={path:'production/phase4/bodies-a.zip',bytes:fs.statSync(inside(ROOT,'production/phase4/bodies-a.zip')).size,sha256:hash(bytes(ROOT,'production/phase4/bodies-a.zip'))},bodyProof=await content.archive(ROOT,bodyRef),badRecipe=structuredClone(body),badPose=structuredClone(body),badPart=structuredClone(body);
 badRecipe.recipe.member='members.json';badRecipe.recipe.sha256=bodyProof.members.find(x=>x.path==='members.json').sha256;assert.throws(()=>content.retainedRecipe(bodyProof,badRecipe,'bodies-a'),/SOURCE_RECIPE_ROLE/);controls.push({id:'recipe-other-valid-member-reboundhash',rejected:true,reason:'SOURCE_RECIPE_ROLE'});
 badRecipe.recipe.member=body.recipe.member;badRecipe.recipe.sha256='0'.repeat(64);assert.throws(()=>content.retainedRecipe(bodyProof,badRecipe,'bodies-a'),/SOURCE_MEMBER_HASH/);controls.push({id:'recipe-stale-hash',rejected:true,reason:'SOURCE_MEMBER_HASH'});
 badPose.bodies[0].poses[0].parts.head=badPose.bodies[0].poses[0].parts.lower;assert.throws(()=>content.retainedParts(bodyProof,badPose,'bodies-a'),/SOURCE_POSE_PART_ROLE/);controls.push({id:'pose-other-valid-part-role',rejected:true,reason:'SOURCE_POSE_PART_ROLE'});
 badPart.parts.push(structuredClone(badPart.parts[0]));assert.throws(()=>content.retainedParts(bodyProof,badPart,'bodies-a'),/SOURCE_PART_IDENTITY/);controls.push({id:'duplicate-valid-production-part',rejected:true,reason:'SOURCE_PART_IDENTITY'});
 content.retainedRecipe(bodyProof,body,'bodies-a');content.retainedParts(bodyProof,body,'bodies-a');controls.push({id:'restored-real-body-recipe-parts',pass:true});
 const interfacePack=content.document(ROOT,'src/godot-game/content/packs/audio-interface-ambience.json'),actions=content.document(ROOT,'src/godot-game/content/packs/audio-actions.json'),bank=bytes(ROOT,'src/godot-game/assets/audio/interface-ambience.wav');
 const interfaceProof=await content.archive(ROOT,{path:INTERFACE,bytes:fs.statSync(inside(ROOT,INTERFACE)).size,sha256:interfacePack.bundleSha256},{capture:['variants/interface.food-crunch.v1.wav','variants/interface.bloater-burst.v1.wav']}),actionPath='production/phase4/audio-actions.zip',actionProof=await content.archive(ROOT,{path:actionPath,bytes:fs.statSync(inside(ROOT,actionPath)).size,sha256:actions.bundleSha256},{capture:['variants/action.2790.full.wav']});
 const mutateInterface=(id,mutate,reason)=>{const pack=structuredClone(interfacePack),actionPack=structuredClone(actions),subject={...interfaceProof,members:interfaceProof.members.map(x=>({...x})),retained:new Map([...interfaceProof.retained].map(([k,v])=>[k,Buffer.from(v)]))},source={...actionProof,members:actionProof.members.map(x=>({...x})),retained:new Map([...actionProof.retained].map(([k,v])=>[k,Buffer.from(v)]))},wav=Buffer.from(bank);mutate(pack,subject,actionPack,source,wav);assert.throws(()=>content.retainedInterface(subject,pack,source,actionPack,wav),new RegExp(reason));controls.push({id,rejected:true,reason,guard:'Actual production retainedInterface, controlled declaration/receipt rebinding where needed to reach specific semantic guard'});content.retainedInterface(interfaceProof,interfacePack,actionProof,actions,bank);};
 const food=p=>p.samples.find(x=>x.id==='interface.food-crunch.v1'),burst=p=>p.samples.find(x=>x.id==='interface.bloater-burst.v1');
 mutateInterface('interface-unknown-origin',p=>{p.samples[0].sourceId='unknown-source';},'SOURCE_INTERFACE_VARIANT_ORIGIN');
 mutateInterface('generated-wrong-valid-donor-category',p=>{p.donorGap.category='AU-18';},'SOURCE_INTERFACE_GENERATED_AUTHORITY');
 mutateInterface('generated-wrong-valid-local-origin',p=>{food(p).sourceId=p.sources[0].id;},'SOURCE_INTERFACE_GENERATED_ROLE');
 mutateInterface('generated-wrong-recipe-role',p=>{food(p).transform='trim-gain';},'SOURCE_INTERFACE_GENERATED_ROLE');
 mutateInterface('generated-wrong-trim',p=>{food(p).trimFrames=[1,11026];},'SOURCE_INTERFACE_GENERATED_RECIPE');
 mutateInterface('generated-wrong-gain',p=>{food(p).gainLinear=0.65;},'SOURCE_INTERFACE_GENERATED_RECIPE');
 mutateInterface('generated-other-valid-member',p=>{food(p).variantMember=p.samples[0].variantMember;food(p).sha256=p.samples[0].sha256;},'SOURCE_INTERFACE_VARIANT_MEMBER_ROLE');
 mutateInterface('generated-false-audition',p=>{food(p).listening.listener='creator';},'SOURCE_INTERFACE_FALSE_AUDITION');
 mutateInterface('generated-wrong-valid-assembler-member',(p,s)=>{p.recipes.assemblerMember='recipes/decode-interface.py';p.recipes.assemblerSha256=s.members.find(x=>x.path===p.recipes.assemblerMember).sha256;},'SOURCE_INTERFACE_RECIPE_IDENTITY');
 mutateInterface('shared-unknown-action-origin',p=>{burst(p).sourceId=actions.samples[0].id;},'SOURCE_INTERFACE_VARIANT_ORIGIN');
 mutateInterface('shared-wrong-transform-role',p=>{burst(p).transform='trim-gain';},'SOURCE_INTERFACE_SHARED_ROLE');
 mutateInterface('shared-stale-source-binding',p=>{p.sharedSources[0].wavSha256=actions.samples[0].sha256;},'SOURCE_INTERFACE_SHARED_DEPENDENCY');
 mutateInterface('shared-other-valid-pack',p=>{p.sharedSources[0].pack='packs/audio-interface-ambience.json';},'SOURCE_INTERFACE_SHARED_DEPENDENCY');
 mutateInterface('shared-missing-dependency',p=>{p.sharedSources=[];},'SOURCE_INTERFACE_SHARED_DEPENDENCY');
 mutateInterface('shared-wrong-trim',p=>{burst(p).trimFrames=[0,11025];},'SOURCE_INTERFACE_SHARED_RECIPE');
 mutateInterface('shared-wrong-gain',p=>{burst(p).gainLinear=0.5;},'SOURCE_INTERFACE_SHARED_RECIPE');
 mutateInterface('shared-other-valid-interface-member',p=>{burst(p).variantMember=p.samples[0].variantMember;burst(p).sha256=p.samples[0].sha256;},'SOURCE_INTERFACE_VARIANT_MEMBER_ROLE');
 const reboundPcm=(p,s,which,wav)=>{const sample=which(p),value=s.retained.get(sample.variantMember);value.writeInt16LE(value.readInt16LE(44)===0?1:0,44);sample.sha256=hash(value);sample.pcmSha256=hash(value.subarray(44));s.members.find(x=>x.path===sample.variantMember).sha256=sample.sha256;if(wav){value.subarray(44).copy(wav,44+sample.regionFrames[0]*4);p.bank.sha256=hash(wav);}};
 mutateInterface('generated-rebound-pcm-wrong-bank-region',(p,s)=>{reboundPcm(p,s,food);},'SOURCE_INTERFACE_PCM_REGION');
 mutateInterface('shared-rebound-pcm-and-bank-wrong-source-recipe',(p,s,a,source,wav)=>{reboundPcm(p,s,burst,wav);},'SOURCE_INTERFACE_SHARED_PCM_RECIPE');
 content.retainedInterface(interfaceProof,interfacePack,actionProof,actions,bank);controls.push({id:'restored-actual-generated-and-shared-interface',pass:true});write(context.directory,'retained-controls.json',controls);
 const restored=await content.validateCompletedSources(ROOT);write(context.directory,'retained-restored.json',restored);assert.deepEqual(restored,initial);
 return{archives:initial.archiveObservations.length,common:initial.common,actualFileRefs:initial.files,aggregateBytes:initial.aggregateBytes,controls:controls.length,scope:'Actual retained source relations and independent mutations only; compiled all180/native reachability/policy/publication and acceptance remain separate'};
}
runner.registerCase('content.source-relations',{...metadata,title:'Actual retained source objects, decoded members, generated roles and inert recipes',order:62,setup:'Current actual133file model,14retained production archives and independent mutations of authenticated source/part/recipe relationships',actions:['Authenticate and stream every retained archive through existing bounded CRC/SHA reader','Match actual retained primary JSON/atlas/recipe/generated-master/prompt/member inventories and exact source owner/raw/stream identities','Reject other valid source/member/recipe/part roles and unsafe/duplicate/callback/range mutations at the same production guards','Restore actual inputs and independently repeat complete retained source relation validation'],expected:'Real retained bytes and relationships pass; each named independent defect fails for its specific source/field reason, never a fabricated acceptance receipt',reset:'Only detached in-memory metadata mutations; every tracked producer/archive byte stays fixed',evidence:['retained-positive.json','retained-controls.json','retained-restored.json','result.json']},retainedSources);
Object.assign(module.exports,{retainedSources});

async function domainRelations(context){
 const before=content.normalizeCurrent(ROOT);write(context.directory,'inputs.json',{files:before.files,source:['tools/phase4/content.cjs','tools/phase4/cases/census.cjs'].map(path=>({path,sha256:hash(bytes(ROOT,path))}))});
 const common=content.document(ROOT,'src/godot-game/content/packs/common.json'),kit=content.document(ROOT,'src/godot-game/content/packs/ui-kit.json'),work=content.document(ROOT,'src/godot-game/content/animation/work-special.json'),commonProof=await content.archive(ROOT,{path:COMMON,bytes:12308215,sha256:COMMON_HASH}),workPath='production/phase4/work-special.zip',workProof=await content.archive(ROOT,{path:workPath,bytes:fs.statSync(inside(ROOT,workPath)).size,sha256:hash(bytes(ROOT,workPath))},{capture:['generated/work-effects-master.png']}),controls=[];
 const readFont=path=>bytes(ROOT,'src/godot-game/'+path),readWork=path=>bytes(ROOT,path),fontBaseline=content.retainedFonts(commonProof,common,kit,readFont),workBaseline=content.retainedWork(workProof,work,common,readWork);write(context.directory,'domain-positive.json',{fontBaseline,workBaseline});
 const font=(id,mutate,reason,reader=readFont)=>{const changed=structuredClone(kit);mutate(changed);assert.throws(()=>content.retainedFonts(commonProof,common,changed,reader),new RegExp(reason));controls.push({id,rejected:true,reason});assert.deepEqual(content.retainedFonts(commonProof,common,kit,readFont),fontBaseline);};
 font('font-other-valid-source-role',p=>{p.fonts[0].id=p.fonts[1].id;},'SOURCE_FONT_ROLE');
 font('font-other-valid-retained-member',p=>{p.fonts[0].member=p.fonts[1].member;p.fonts[0].sha256=p.fonts[1].sha256;p.fonts[0].bytes=p.fonts[1].bytes;},'SOURCE_FONT_MEMBER_ROLE');
 font('font-wrong-fallback-role',p=>{p.fonts[0].fallbacks=['assets/ui/PerfectDOSVGA437.ttf'];},'SOURCE_FONT_FALLBACK_ROLE');
 font('font-candidate-other-valid-object',p=>{p.fontCandidates[0].source.objectId=p.fontCandidates[1].source.objectId;},'SOURCE_FONT_CANDIDATE_ORIGIN');
 font('font-duplicate-role',p=>{p.fonts[1].role=p.fonts[0].role;},'SOURCE_FONT_ROLE');
 font('font-wrong-valid-runtime-bytes',()=>{},'SOURCE_FONT_RUNTIME_BYTES',path=>path==='assets/ui/LiberationSans.ttf'?readFont('assets/ui/PerfectDOSVGA437.ttf'):readFont(path));
 controls.push({id:'restored-three-actual-font-roles',pass:true});
 const changedWork=(id,mutate,reason)=>{const changed=structuredClone(work);mutate(changed);assert.throws(()=>content.retainedWork(workProof,changed,common,readWork),new RegExp(reason));controls.push({id,rejected:true,reason});assert.deepEqual(content.retainedWork(workProof,work,common,readWork),workBaseline);};
 changedWork('work-other-valid-generated-master',p=>{p.provenance.master='retained/bodies-a/masters/CH-01.png';},'SOURCE_WORK_GENERATION_ROLE');
 changedWork('work-other-valid-inert-recipe',p=>{p.provenance.recipe='recipes/extract-media.py';},'SOURCE_WORK_GENERATION_ROLE');
 changedWork('work-generated-part-as-donor',p=>{p.components[0].source=p.components.find(x=>x.key==='hammer').source;},'SOURCE_WORK_COMPONENT_ROLE');
 changedWork('work-donor-other-valid-source',p=>{p.components.find(x=>x.key==='hammer').source=workProof.members.find(x=>x.path.startsWith('retained/common/images/')&&!x.path.endsWith('/6853.png')).path;},'SOURCE_WORK_DONOR_ROLE');
 changedWork('work-source-cell-outside-master',p=>{p.components[0].cell[2]=9007199254740991;},'SOURCE_WORK_SOURCE_REGION');
 changedWork('work-source-crop-outside-cell',p=>{p.components[0].crop[2]=p.components[0].cell[2]+1;},'SOURCE_WORK_SOURCE_REGION');
 changedWork('work-unknown-clip-reference',p=>{p.actions.find(x=>x.sourceClip!==null).sourceClip='common.clip.999999';},'SOURCE_WORK_CLIP_REFERENCE');
 changedWork('work-historical-chop-other-valid-sprite',p=>{p.actions.find(x=>x.id==='Chop').sourceClip='common.s0.6853';},'SOURCE_WORK_CLIP_REFERENCE');
 changedWork('work-tool-without-component',p=>{p.actions[0].tool='bow';},'SOURCE_WORK_TOOL_REFERENCE');
 changedWork('work-forbidden-gameplay-mutation',p=>{p.actions[0].inventoryMutation=true;},'SOURCE_WORK_ACTION_DOMAIN');
 changedWork('work-nonpresentation-callback',p=>{p.actions.find(x=>x.event!==null).event.method='ApplyDamage';},'SOURCE_WORK_INERT_EVENT');
 changedWork('work-duplicate-component',p=>{p.components.push(structuredClone(p.components[0]));},'SOURCE_WORK_COMPONENT_IDENTITY');
 controls.push({id:'restored-actual-generated-and-donor-work-components',pass:true});write(context.directory,'domain-controls.json',controls);assert.deepEqual(content.normalizeCurrent(ROOT),before);
 return{fontBaseline,workBaseline,controls:controls.length,accepted:0,scope:'Actual retained/generated domain and role relationships; independent compiled selector and final native/runtime/publication gates remain separate'};
}
runner.registerCase('content.domain-relations',{...metadata,title:'Actual donor font and generated/donor work domain relationships',order:63,setup:'Three actual packaged donor fonts and mixed generated/donor work master, immutable common/work archives and current runtime resources',actions:['Link explicit font roles and candidates to authenticated original donor member bytes, retaining shared body/fallback identity','Check actual generated master/cell/crop/recipe/prompt and donor hammer relationships','Reject other valid sources/roles, wrong regions/references, duplicate identities and forbidden callbacks/gameplay mutation','Restore actual source/runtime bytes after each independent guard'],expected:'Exact real domain relationships and all independent failures/restored positives pass with0accepted',reset:'Detached metadata and bounded byte callbacks only, no tracked source/resource mutation',evidence:['inputs.json','domain-positive.json','domain-controls.json','result.json']},domainRelations);
Object.assign(module.exports,{domainRelations});

function nativeCohorts(){
 const authority=content.expected().obligations.map(x=>x.id),groups=[];
 for(const [prefix,category,size]of [['AN','Characters',18],['CH','Characters',8],['IT','Items',12],['BL','Buildings/world',9],['EN','Buildings/world',11],['FX','Buildings/world',12],['UI','UI',9],['AU','Audio',15],['DT','Data/text',9],['TXT','Data/text',2]]){
  const ids=authority.filter(x=>x.startsWith(prefix+'-'));for(let start=0;start<ids.length;start+=size)groups.push({id:prefix.toLowerCase()+'-'+Math.floor(start/size),category,ids:ids.slice(start,start+size),kind:'obligations',reason:'Exact finite category subset; full native traversal exceeded original15s startup tracer. Independent children preserve per-child60s and startup15s.'});
 }
 groups.push({id:'world-views',category:'Buildings/world',ids:['world.assembled','world.effects','world.portals','world.wildlife'],kind:'inspection-views',authority:'WorldPreview.InspectorIds(Buildings/world), exact existing four IDs; separate from180 obligations',reason:'Existing non-obligation inspection controls require separate native traversal'});
 const ids=groups.filter(x=>x.kind==='obligations').flatMap(x=>x.ids);assert.equal(new Set(ids).size,180);assert.deepEqual([...ids].sort(),[...authority].sort());return groups;
}
const NATIVE_CENSUS_DRIVER=String.raw`extends SceneTree
var hub: Node
var sequence = 0
var outcomes: Array = []
var failed = false
var request: Dictionary
var retirement_controls: Array = []
var retirement_control_hits = 0
var run_started = 0
const CURRENT = "Shell/Workspace/Preview/Surface/Current"
func record(stage,detail):
 var file = FileAccess.open("res://census-"+request.nonce+"-stage-"+str(sequence)+".json",FileAccess.WRITE)
 file.store_string(JSON.stringify({"pid":OS.get_process_id(),"nonce":request.nonce,"cohort":request.cohort.id,"stage":stage,"detail":detail,"ticksMsec":Time.get_ticks_msec()}))
 file.close()
 sequence += 1
func save_result(complete):
 var file = FileAccess.open("res://census-"+request.nonce+"-result.json",FileAccess.WRITE)
 file.store_string(JSON.stringify({"pid":OS.get_process_id(),"nonce":request.nonce,"cohort":request.cohort,"outcomes":outcomes,"retirementControls":retirement_controls,"failed":failed,"complete":complete,"runtime":str(hub.get_meta("inspection_runtime","")) if is_instance_valid(hub) else request.runtime,"accepted":0}))
 file.close()
func _initialize():
 request = JSON.parse_string(FileAccess.get_file_as_string("res://census-request.json"))
 record("initialize",{"requested":request.cohort.ids})
 call_deferred("run")
func owner_state(instance):
 if not is_instance_id_valid(instance):
  return {"instance":instance,"valid":false,"retired":true}
 var node = instance_from_id(instance)
 var callbacks = []
 var pending = [node]
 while not pending.is_empty():
  var child = pending.pop_front()
  pending.append_array(child.get_children())
  for signal_name in ["pressed","toggled","item_selected","value_changed"]:
   if child.has_signal(signal_name):
    for link in child.get_signal_connection_list(signal_name):
     callbacks.append({"node":child.get_instance_id(),"signal":signal_name,"callable":str(link.callable)})
 var closed = callbacks.is_empty() and (not node.has_meta("resource_view") or bool(node.get_meta("closed",false)))
 return {"instance":instance,"valid":true,"name":node.name,"inTree":node.is_inside_tree(),"parent":node.get_parent().get_instance_id() if node.get_parent()!=null else 0,"queued":node.is_queued_for_deletion(),"processing":node.is_processing(),"callbacks":callbacks,"acceptanceClosed":closed,"retired":false}
func retirement_ready(ids):
 for instance in ids:
  var state = owner_state(instance)
  if state.valid and (state.inTree or state.parent!=0 or not state.queued or state.processing or not state.acceptanceClosed):
   return false
 return true
func retirement_states(ids):
 return ids.map(func(instance):return owner_state(instance))
func await_retirement(ids,label):
 var begun = Time.get_ticks_msec()
 var frames = 0
 record("retirement-immediate",{"id":label,"states":retirement_states(ids),"ready":retirement_ready(ids),"frames":frames,"elapsedMs":0})
 if not retirement_ready(ids):
  return {"pass":false,"reason":"OLD_OWNER_ACTIVE_OR_NOT_QUEUED","frames":frames,"elapsedMs":Time.get_ticks_msec()-begun,"states":retirement_states(ids)}
 while ids.any(func(instance):return is_instance_id_valid(instance)):
  record("retirement-pending",{"id":label,"states":retirement_states(ids),"frames":frames,"elapsedMs":Time.get_ticks_msec()-begun})
  if Time.get_ticks_msec()-run_started>=60000:
   return {"pass":false,"reason":"OWNED_RETIREMENT_TIMEOUT","frames":frames,"elapsedMs":Time.get_ticks_msec()-begun,"states":retirement_states(ids)}
  await process_frame
  frames += 1
  if not retirement_ready(ids):
   return {"pass":false,"reason":"OLD_OWNER_REACTIVATED","frames":frames,"elapsedMs":Time.get_ticks_msec()-begun,"states":retirement_states(ids)}
 var result = {"pass":true,"reason":"ACTUAL_WITHIN_RUN_NATIVE_IDS_INVALID","frames":frames,"elapsedMs":Time.get_ticks_msec()-begun,"states":retirement_states(ids)}
 record("retirement-complete",{"id":label,"result":result})
 return result
func retirement_control_pressed():
 retirement_control_hits += 1
func prove_retirement():
 var control = Node2D.new()
 control.name = "RetirementControl"
 root.add_child(control)
 var button = Button.new()
 control.add_child(button)
 button.pressed.connect(retirement_control_pressed)
 var ids = [control.get_instance_id()]
 button.pressed.emit()
 var active = {"id":"actual-active-owned-node-rejected","pass":not retirement_ready(ids) and retirement_control_hits==1,"states":retirement_states(ids)}
 retirement_controls.append(active)
 record("retirement-control",active)
 root.remove_child(control)
 var detached = {"id":"actual-detached-not-queued-node-rejected","pass":not retirement_ready(ids),"states":retirement_states(ids)}
 retirement_controls.append(detached)
 record("retirement-control",detached)
 button.pressed.disconnect(retirement_control_pressed)
 button.pressed.emit()
 control.queue_free()
 var queued = {"id":"actual-disconnected-queued-owner-restored","pass":retirement_ready(ids) and retirement_control_hits==1,"states":retirement_states(ids)}
 retirement_controls.append(queued)
 record("retirement-control",queued)
 var deleted = await await_retirement(ids,"retirement-control")
 retirement_controls.append({"id":"actual-native-deletion-restored","pass":deleted.pass and not is_instance_id_valid(ids[0]),"result":deleted})
 record("retirement-control",retirement_controls[-1])
 return retirement_controls.all(func(row):return row.pass)
func filter_facts(content,foreign,unrelated):
 var owner = content.get_node_or_null("WorldPreview")
 var ground = null
 if owner!=null:
  for node in owner.find_children("*","CanvasItem",true,false):
   if str(node.get_meta("resource_id",""))==request.nonmatchingResource:
    ground = node
 var visible = []
 if owner!=null:
  for node in owner.find_children("*","CanvasItem",true,false):
   if node.is_visible_in_tree() and node.has_meta("resource_id"):
    visible.append({"id":str(node.get_meta("resource_id")),"instance":node.get_instance_id(),"class":node.get_class()})
 return {"owner":owner.get_instance_id() if owner!=null else 0,"ownerVisible":owner!=null and owner.is_visible_in_tree(),"ownerClass":owner.get_class() if owner!=null else "","ownerName":str(owner.name) if owner!=null else "","ownerResource":str(owner.get_meta("resource_id","")) if owner!=null else "","foreign":foreign.get_instance_id(),"foreignVisible":foreign.visible,"foreignClass":foreign.get_class(),"foreignName":str(foreign.name),"foreignResource":str(foreign.get_meta("resource_id")),"unrelatedVisible":unrelated.visible,"nonmatching":ground.get_instance_id() if ground!=null else 0,"nonmatchingVisible":ground.visible if ground!=null else true,"visibleResources":visible}
func prove_filter(id):
 var content = hub.get_node(CURRENT+"/WorldItems/InspectorScroll/Body/Art/Content")
 var original = content.get_node("WorldPreview")
 var original_instance = original.get_instance_id()
 var holder = Node2D.new()
 holder.name = "ForeignFilterProof"
 content.add_child(holder)
 var foreign = Node2D.new()
 foreign.name = original.name
 foreign.set_meta("resource_id",original.get_meta("resource_id"))
 holder.add_child(foreign)
 var unrelated = Node2D.new()
 unrelated.set_meta("resource_id","probe.unrelated")
 holder.add_child(unrelated)
 record("filter-control-pending",{"id":id,"originalOwner":original_instance,"facts":filter_facts(content,foreign,unrelated)})
 var choices = hub.get_node(CURRENT+"/WorldItems/InspectorScroll/Body/Choices")
 var light = choices.get_node("Light")
 light.item_selected.emit(light.selected)
 var after_light = filter_facts(content,foreign,unrelated)
 record("filter-control-after-light",{"id":id,"facts":after_light})
 choices.get_node("Step").pressed.emit()
 var after_step = filter_facts(content,foreign,unrelated)
 record("filter-control-after-step",{"id":id,"facts":after_step})
 var state = choices.get_node("CurrentState")
 state.item_selected.emit(state.selected)
 var restored = filter_facts(content,foreign,unrelated)
 record("filter-control-restored",{"id":id,"facts":restored})
 var pass_control = after_light.owner!=original_instance and after_step.owner!=after_light.owner and restored.owner!=after_step.owner
 for facts in [after_light,after_step,restored]:
  pass_control = pass_control and facts.ownerVisible and not facts.foreignVisible and not facts.unrelatedVisible and facts.nonmatching!=0 and not facts.nonmatchingVisible and facts.ownerClass==facts.foreignClass and facts.ownerName==facts.foreignName and facts.ownerResource==facts.foreignResource
 var holder_instance = holder.get_instance_id()
 holder.free()
 var result = {"pass":pass_control and not is_instance_id_valid(holder_instance),"afterLight":after_light,"afterStep":after_step,"restored":restored,"controlDisposed":not is_instance_id_valid(holder_instance)}
 record("filter-control-complete",{"id":id,"result":result})
 return result
func prove_data_library(id):
 var selector = hub.get_node(CURRENT+"/DataText/Body/Choices/Field")
 var expected = request.dataLibraries[id]
 var observations = []
 var pass_control = selector.item_count==expected.size()
 for index in range(expected.size()):
  record("data-library-pending",{"id":id,"index":index,"source":expected[index].path})
  selector.select(index)
  selector.item_selected.emit(index)
  var actual = observe(id)
  var row = {"source":actual.dataLibrary.get("source",""),"sha256":actual.displayedDocumentSha256,"pass":actual.verified and actual.dataLibrary.get("source","")==expected[index].path,"readOnly":actual.dataLibrary.get("readOnly",false)}
  observations.append(row)
  pass_control = pass_control and row.pass
  record("data-library-complete",{"id":id,"index":index,"actual":row})
 var previous = observe(id)
 selector.item_selected.emit(-1)
 var rejected = observe(id)
 var invalid_stable = rejected.dataLibrary==previous.dataLibrary and rejected.displayedDocumentSha256==previous.displayedDocumentSha256
 record("data-library-invalid-index",{"id":id,"unchanged":invalid_stable,"source":rejected.dataLibrary.get("source","")})
 selector.select(0)
 selector.item_selected.emit(0)
 var restored = observe(id)
 return {"pass":pass_control and invalid_stable and restored.verified and restored.dataLibrary.get("source","")==expected[0].path,"sources":observations,"invalidIndexUnchanged":invalid_stable,"restoredSha256":restored.displayedDocumentSha256}
func observe(id):
 var current = hub.get_node(CURRENT)
 var queue = current.get_children()
 var nodes = []
 var sprites = []
 var widgets = []
 var playing = []
 var facts = []
 var native_resources = []
 var ui_facts = []
 while not queue.is_empty():
  var node = queue.pop_front()
  if node is CanvasItem and not node.is_visible_in_tree():
   continue
  nodes.append({"path":str(current.get_path_to(node)),"class":node.get_class(),"instance":node.get_instance_id()})
  if node is Sprite2D and node.texture != null:
   sprites.append({"path":str(current.get_path_to(node)),"width":node.texture.get_width(),"height":node.texture.get_height(),"part":str(node.get_meta("part","")),"source":str(node.get_meta("source","")),"resourceId":str(node.get_meta("resource_id",""))})
  for key in ["appearance","wildlife","identity","zone","site"]:
   if node.has_meta(key):
    facts.append({"path":str(current.get_path_to(node)),"kind":key,"value":JSON.parse_string(str(node.get_meta(key)))})
  if node.has_meta("resource_id"):
   native_resources.append({"id":str(node.get_meta("resource_id")),"class":node.get_class(),"path":str(current.get_path_to(node))})
  if node.has_meta("stash_identity"):
   facts.append({"kind":"stash","value":str(node.get_meta("stash_identity")),"path":str(current.get_path_to(node))})
  if node.has_meta("occupied"):
   facts.append({"kind":"seat","value":bool(node.get_meta("occupied")),"position":[node.position.x,node.position.y],"path":str(current.get_path_to(node))})
  if node.has_meta("ui_control"):
   ui_facts.append({"kind":"control","id":str(node.get_meta("ui_control")),"class":node.get_class(),"path":str(current.get_path_to(node))})
  if node is TextureRect and node.texture!=null:
   var texture_fact = {"kind":"texture","name":str(node.name),"path":str(current.get_path_to(node)),"width":node.texture.get_width(),"height":node.texture.get_height(),"resource":node.texture.resource_path,"symbol":str(node.get_meta("ui_symbol",""))}
   if node.texture is AtlasTexture:
    texture_fact.atlas = node.texture.atlas.resource_path
    texture_fact.region = [node.texture.region.position.x,node.texture.region.position.y,node.texture.region.size.x,node.texture.region.size.y]
   ui_facts.append(texture_fact)
  if node is Label:
   ui_facts.append({"kind":"label","text":node.text,"path":str(current.get_path_to(node))})
   if node.has_meta("font_source"):
    ui_facts.append({"kind":"font","resource":node.get_theme_font("font").resource_path,"source":str(node.get_meta("font_source")),"size":node.get_theme_font_size("font_size")})
  if node is BaseButton or node is Range or node is TextEdit or node is ItemList:
   widgets.append(str(current.get_path_to(node)))
  if node is AudioStreamPlayer and node.playing:
   playing.append(node.get_instance_id())
  queue.append_array(node.get_children())
 var state = {"id":id,"summaryId":hub.get_node("Summary/ResourceId").text,"status":hub.get_node("Summary/Status").text,"epoch":int(hub.get("_generation")),"nativeNodes":nodes,"sprites":sprites,"widgets":widgets,"playing":playing,"packStatus":hub.get_node("Summary/PackStatus").text,"errors":hub.get_node("Shell/Workspace/Preview/Failures").text,"verified":false,"reason":"No actual selected preview/controller content"}
 state.nativeFacts = facts
 state.nativeResources = native_resources
 state.uiFacts = ui_facts
 var character = current.get_node_or_null("Character")
 var world = current.get_node_or_null("WorldItems/InspectorScroll/Body/Identity")
 var data = current.get_node_or_null("DataText/Body/Identity")
 var detail = current.get_node_or_null("DataText/Body/Detail")
 var audio = current.get_node_or_null("AudioPreview/Audio/Content/Event")
 var resource = current.get_node_or_null("ResourcePreview")
 if (id.begins_with("CH-") or id.begins_with("AN-")) and character != null:
  state.characterState = {"composition":str(character.get_meta("composition","")),"sample":str(character.get_meta("sample","")),"work":str(character.get_meta("work","")),"obligation":str(character.get_meta("obligation","")),"activation":str(character.get_meta("activation",""))}
  state.characterControls = {}
  for key in ["Body","Movement","Action","WorkAction","left","right","BowSubject","Ammunition"]:
   var control = character.get_node_or_null("Controls/Slots/"+key+"/Select")
   if control != null:
    state.characterControls[key] = control.get_item_text(control.selected)
  var expected_body = id if id.begins_with("CH-") and id!="CH-09" else "CH-06" if id=="AN-15" else "CH-08" if id=="AN-16" else "CH-01"
  var real_body = sprites.any(func(row):return row.part.begins_with(expected_body+"."))
  if id in ["CH-09","AN-18"]:
   real_body = sprites.any(func(row):return row.path.contains("Guard")) and state.characterControls.get("BowSubject","")=="Structural guard"
  var selected_state = true
  var required = {"AN-01":{"Movement":"Idle"},"AN-02":{"Movement":"Walk"},"AN-03":{"Movement":"Jump"},"AN-04":{"Action":"Melee","left":"Empty","right":"Empty"},"AN-05":{"Action":"Melee","left":"IT-03"},"AN-06":{"Action":"Melee","left":"IT-03","right":"IT-04"},"AN-07":{"Action":"Bow"},"AN-08":{"Action":"Bow","Ammunition":"0"},"AN-10":{"WorkAction":"Sit"},"AN-11":{"WorkAction":"Gather"},"AN-12":{"WorkAction":"Build"},"AN-13":{"WorkAction":"Pickup"},"AN-14":{"WorkAction":"Eat"},"AN-15":{"WorkAction":"Spit"},"AN-16":{"WorkAction":"Burst"},"AN-18":{"BowSubject":"Structural guard"}}
  for key in required.get(id,{}):
   selected_state = selected_state and state.characterControls.get(key,"")==required[id][key]
  state.verified = real_body and selected_state and not widgets.is_empty() and state.characterControls.get("Body","")==expected_body
  state.reason = "Actual character/animation sprites, controller controls and selected source identity"
 elif resource != null and resource.has_meta("resource_view"):
  state.resourceView = JSON.parse_string(str(resource.get_meta("resource_view")))
  var real_content = not sprites.is_empty() and not widgets.is_empty()
  if id in ["AN-09","CH-13"]:
   real_content = real_content and facts.any(func(row):return row.kind=="appearance" and row.value.persistent and row.value.gameplayEvents==0)
  elif id in ["CH-10","CH-11","AN-17"]:
   var species = "deer" if id=="CH-11" else "rabbit"
   real_content = real_content and facts.any(func(row):return row.kind=="wildlife" and row.value.body==species and row.value.selected and row.value.gameplayEvents==0)
  elif id=="CH-12":
   var players = facts.filter(func(row):return row.kind=="identity")
   real_content = real_content and players.size()==4 and players.map(func(row):return row.value.number).size()==4
  elif id=="EN-08":
   real_content = real_content and facts.filter(func(row):return row.kind=="zone" and row.value.width==15).size()==6
  elif id in request.ui:
   var authority = request.ui[id]
   var surface = resource.get_node_or_null("Art/LibraryScroll/Surface")
   real_content = surface!=null and surface.has_meta("ui_library") and not widgets.is_empty()
   if real_content:
    state.uiLibrary = JSON.parse_string(str(surface.get_meta("ui_library")))
    real_content = state.uiLibrary.id==id and state.uiLibrary.source==authority.source and state.uiLibrary.state==state.resourceView.state
    if id=="UI-01":
     var controls = ui_facts.filter(func(row):return row.kind=="control")
     real_content = real_content and controls.size()==authority.controls.size() and authority.controls.all(func(name):return controls.any(func(row):return row.id==name and row.class==authority.classes[name]))
    elif id=="UI-02":
     var fonts = ui_facts.filter(func(row):return row.kind=="font")
     real_content = real_content and fonts.size()==authority.fonts.size() and authority.fonts.all(func(expected):return fonts.any(func(row):return row.source==expected.source and row.resource=="res://"+expected.source and row.size==expected.size))
    elif id in ["UI-03","UI-04","UI-07"]:
     real_content = real_content and authority.identities.all(func(key):return native_resources.filter(func(row):return row.id==key).size()==1)
     if id=="UI-03":
      real_content = real_content and ui_facts.any(func(row):return row.kind=="texture" and row.symbol=="interaction-cursor" and row.atlas==authority.atlas and row.region==authority.region)
    elif id=="UI-08":
     real_content = real_content and ui_facts.any(func(row):return row.kind=="label" and row.text==authority.title) and ui_facts.any(func(row):return row.kind=="texture" and row.name=="CurrentIcon" and row.width==256 and row.height==256 and row.resource==authority.icon) and ui_facts.any(func(row):return row.kind=="texture" and row.symbol=="title-emblem" and row.region==authority.region)
    else:
     var sample = authority.samples.filter(func(row):return row.id==state.uiLibrary.state)
     real_content = real_content and sample.size()==1
     if real_content:
      if id in ["UI-25","UI-27"]:
       var settings = surface.get_node_or_null("Settings/Frame/Viewport/Extent/Canvas/LocalScroll/Content")
       real_content = settings!=null and settings.get_node("Identity").text==sample[0].id and settings.get_node("Title").text==sample[0].name and settings.get_node("World").text=="World continues" and not paused
      else:
       real_content = ui_facts.any(func(row):return row.kind=="label" and row.text.contains(sample[0].name)) and (state.uiLibrary.controller=="InventoryLayouts" or native_resources.any(func(row):return row.id==sample[0].id))
  state.verified = real_content and state.resourceView.id==id and not state.resourceView.closed and state.resourceView.epoch>0
  state.reason = "Actual owned source-specific presenter/native sprites/controls and live domain facts"
 elif world != null:
  state.identity = world.text
  state.verified = world.text.begins_with(id+"\nPopulated") and not sprites.is_empty() and not widgets.is_empty()
  state.reason = "Actual native authored world/item content and selected identity"
  var owner = current.get_node("WorldItems")
  if owner.has_meta("library_view"):
   state.libraryView = JSON.parse_string(str(owner.get_meta("library_view")))
   var selected = owner.get_node("InspectorScroll/Body/Choices/CurrentState")
   var coherent = state.libraryView.id==id and state.libraryView.epoch>0 and selected.get_item_text(selected.selected)==state.libraryView.state and world.text.begins_with(id+"\nPopulated") and not widgets.is_empty()
   if id.begins_with("FX-"):
    var effect = state.libraryView.state.trim_prefix("effect:")
    coherent = coherent and effect in request.effectFamilies[id] and state.libraryView.eventCount==1 and native_resources.any(func(row):return row.id=="effect."+effect or row.id.begins_with("effect."+effect+"."))
   elif id=="BL-15":
    coherent = coherent and request.rallyIds.all(func(rally):return native_resources.any(func(row):return row.id.begins_with(rally+".")))
   elif id=="BL-16":
    coherent = coherent and request.portalIds.all(func(portal):return native_resources.any(func(row):return row.id.begins_with(portal+".")))
   else:
    var building = owner.get_node("InspectorScroll/Body/Art/Content")
    var tier = building.get_node("Controls/Tier")
    state.building = {"identity":str(building.get_meta("health_identity","")),"tierSelected":tier.selected,"sourceKind":state.libraryView.kind,"sample":state.libraryView.sample}
    coherent = coherent and not sprites.is_empty() and state.building.identity==id and state.libraryView.tier>=1 and state.libraryView.tier<=3
    if id=="BL-10":
     var sites = facts.filter(func(row):return row.kind=="site").map(func(row):return row.value.id)
     sites.sort()
     coherent = coherent and sites==request.siteIds
    elif id=="BL-13":
     coherent = coherent and facts.any(func(row):return row.kind=="stash" and row.value==request.stashId)
    elif id=="BL-14":
     var seats = facts.filter(func(row):return row.kind=="seat").map(func(row):return row.position)
     coherent = coherent and seats==request.seats[state.libraryView.tier-1]
   state.verified = coherent
   state.reason = "Actual source-bound finite building/effect library with independent native component relationships"
 elif data != null and detail != null:
  state.identity = data.text
  state.detailLength = detail.text.length()
  state.verified = data.text.begins_with(id+" · ") and not detail.editable and detail.text.length()>0 and not data.text.contains("Unavailable") and not widgets.is_empty()
  state.reason = "Actual read-only selected data/text content and fields"
  if id in request.dataLibraries:
   var library = current.get_node("DataText")
   state.dataLibrary = JSON.parse_string(str(library.get_meta("data_library","{}")))
   var raw_start = detail.text.find("{")
   var raw = detail.text.substr(raw_start) if raw_start>=0 else ""
   state.displayedDocumentSha256 = raw.sha256_text()
   state.verified = state.verified and state.dataLibrary.get("id","")==id and state.dataLibrary.get("readOnly",false) and state.dataLibrary.get("sources",[])==request.dataLibraries[id].map(func(row):return row.path) and request.dataLibraries[id].any(func(row):return row.path==state.dataLibrary.get("source","") and row.sha256==state.displayedDocumentSha256)
 elif audio != null:
  state.routes = []
  for index in range(audio.item_count):
   state.routes.append(audio.get_item_text(index))
  state.routes.sort()
  var expected_routes = request.audioRoutes.get(id,[])
  expected_routes.sort()
  state.verified = state.routes==expected_routes and not expected_routes.is_empty() and playing.is_empty() and not widgets.is_empty()
  state.reason = "Actual exact native audio route controls, selected silently"
 return state
func run():
 run_started = Time.get_ticks_msec()
 record("scene-load-start",{"path":"inspection/Inspection.tscn"})
 hub = load("res://inspection/Inspection.tscn").instantiate()
 root.add_child(hub)
 var begun = Time.get_ticks_msec()
 while hub.get_node("Summary/Status").text.begins_with("Loading"):
  if Time.get_ticks_msec()-begun>15000:
   record("startup-timeout",{"status":hub.get_node("Summary/Status").text})
   failed = true
   save_result(false)
   hub.free()
   quit(1)
   return
  await process_frame
 request.runtime = str(hub.get_meta("inspection_runtime",""))
 record("startup-ready",{"runtime":request.runtime,"packStatus":hub.get_node("Summary/PackStatus").text})
 if not hub.get_node("Summary/PackStatus").text.begins_with("180/180 source-ready · 0/180 accepted"):
  failed = true
 if not await prove_retirement():
  failed = true
 var category = hub.get_node("Shell/Workspace/Navigation/Category")
 var selected_category = -1
 for index in range(category.item_count):
  if category.get_item_text(index)==request.cohort.category:
   selected_category = index
 record("category-pending",{"category":request.cohort.category,"index":selected_category})
 if selected_category<0:
  failed = true
 else:
  category.select(selected_category)
  category.item_selected.emit(selected_category)
 var list = hub.get_node("Shell/Workspace/Controls/Browser/Resources")
 for id in request.cohort.ids:
  var index = -1
  for row in range(list.item_count):
   if list.get_item_text(row).begins_with(id+" · "):
    index = row
  var previous = hub.get_node(CURRENT).get_children().map(func(node):return node.get_instance_id())
  var epoch = int(hub.get("_generation"))
  record("selection-pending",{"id":id,"category":request.cohort.category,"index":index,"epoch":epoch,"previousOwners":previous,"previousState":retirement_states(previous),"acceptance":hub.get_node("Summary/PackStatus").text})
  if index>=0:
   list.select(index)
   list.item_selected.emit(index)
  var quiet = {}
  if id in request.effectFamilies:
   quiet = observe(id)
   record("effect-quiet-restored",quiet)
   var effect_selector = hub.get_node_or_null(CURRENT+"/WorldItems/InspectorScroll/Body/Choices/CurrentState")
   if effect_selector!=null:
    record("effect-explicit-selection",{"id":id,"index":effect_selector.selected,"state":effect_selector.get_item_text(effect_selector.selected)})
    effect_selector.item_selected.emit(effect_selector.selected)
  var actual = observe(id)
  if id in request.effectFamilies:
   actual.quietBeforeExplicit = {"eventCount":quiet.get("libraryView",{}).get("eventCount",-1),"nativeEffects":quiet.nativeResources.filter(func(row):return row.id.begins_with("effect.")).size()}
   actual.verified = actual.verified and actual.quietBeforeExplicit.eventCount==0 and actual.quietBeforeExplicit.nativeEffects==0
  if id in request.effectFamilies or id in ["BL-15","BL-16"]:
   var filter_control = prove_filter(id)
   var after_filter = observe(id)
   after_filter.filterControl = filter_control
   after_filter.verified = after_filter.verified and filter_control.pass
   if actual.has("quietBeforeExplicit"):
    after_filter.quietBeforeExplicit = actual.quietBeforeExplicit
    after_filter.verified = after_filter.verified and actual.quietBeforeExplicit.eventCount==0 and actual.quietBeforeExplicit.nativeEffects==0
   actual = after_filter
  if id in request.dataLibraries:
   var library_control = prove_data_library(id)
   actual = observe(id)
   actual.libraryControl = library_control
   actual.verified = actual.verified and library_control.pass
  record("selection-content",actual)
  actual.retirement = await await_retirement(previous,id)
  actual.previousOwnersRetired = actual.retirement.pass and previous.all(func(instance):return not is_instance_id_valid(instance))
  actual.afterRetirementEpoch = int(hub.get("_generation"))
  actual.afterRetirementId = hub.get_node("Summary/ResourceId").text
  actual.previousOwnersRetired = actual.previousOwnersRetired and actual.afterRetirementEpoch==actual.epoch and actual.afterRetirementId==id
  actual["pass"] = index>=0 and actual.summaryId==id and actual.epoch>epoch and actual.verified and actual.previousOwnersRetired and not actual.errors.contains("field bytes, value read")
  record("selection-finished",actual)
  outcomes.append(actual)
  failed = failed or not actual["pass"]
  save_result(false)
 record("closing",{"owner":hub.get_instance_id(),"selected":hub.get_node("Summary/ResourceId").text})
 save_result(true)
 hub.free()
 record("closed",{"ownerValid":is_instance_valid(hub)})
 if is_instance_valid(hub):
  failed = true
 print("CENSUS_NATIVE_COMPLETE "+request.nonce+" "+("FAIL" if failed else "PASS"))
 quit(1 if failed else 0)
`;
function nativeUiAuthority(){
 const kit=content.document(ROOT,'src/godot-game/content/packs/ui-kit.json'),identity=content.document(ROOT,'src/godot-game/content/packs/ui-identity.json'),result={};
 for(const [pack,ids]of [['ui-kit',[1,2]],['ui-identity',[3,4,7,8]],['ui-inventory',[5,12,13]],['ui-legacy-stash',[6,14,15]],['ui-camp',[16,17,18,19]],['ui-map-warnings',[20,21,26]],['ui-session',[9,10,11,22,23,24]],['ui-settings',[25,27]]]){
  const source='content/packs/'+pack+'.json',definition=content.document(ROOT,'src/godot-game/'+source),data=!['ui-kit','ui-identity'].includes(pack)?content.document(ROOT,'src/godot-game/'+definition.bindings[0].path):null;
  for(const number of ids){const id='UI-'+String(number).padStart(2,'0');need(definition.obligations.includes(id),'CENSUS_UI_SOURCE_OBLIGATION',id);result[id]={source};if(data)result[id].samples=data.samples.map(x=>({id:x.id,name:x.name??x.title??x.reason}));}
 }
 result['UI-01']={...result['UI-01'],controls:kit.controls,classes:Object.fromEntries(kit.controls.map(x=>[x,['TooltipPanel','ToastPanel'].includes(x)?'PanelContainer':x]))};
 result['UI-02']={...result['UI-02'],fonts:kit.fonts.flatMap(font=>kit.typography.map(type=>({source:font.path,size:type.size})))};
 for(const [id,key]of [['UI-03','bindings'],['UI-04','statuses'],['UI-07','players']])result[id].identities=identity[key].map(x=>x.id);
 Object.assign(result['UI-03'],{atlas:'res://'+identity.atlas.path,region:identity.regions['interaction-cursor']});
 Object.assign(result['UI-08'],{title:identity.desktop.title,icon:'res://'+identity.desktop.sourceApplication.path,region:identity.regions['title-emblem']});
 need(Object.keys(result).length===27,'CENSUS_UI_INDEPENDENT_COVERAGE');return result;
}
function nativeDataAuthority(){
 const paths={
  'DT-19':['content/packs/delivery.json','content/packs/compatibility.json'],
  'DT-20':['content/animation/locomotion-melee.json','content/animation/bow.json','content/animation/work-special.json','content/animation/fits-a.json','content/animation/fits-b.json','content/animation/fits-c.json'],
  'DT-21':['content/world/map.json','content/world/main-assemblies.json','content/world/defense-assemblies.json','content/world/portal-assemblies.json'],
  'DT-22':['content/packs/ui-kit.json','content/packs/ui-identity.json','content/packs/ui-inventory.json','content/packs/ui-legacy-stash.json','content/packs/ui-camp.json','content/packs/ui-map-warnings.json','content/packs/ui-session.json','content/packs/ui-settings.json'],
  'DT-23':['content/audio/events.json','content/packs/audio-actions.json','content/packs/audio-interface-ambience.json']
 };
 return Object.fromEntries(Object.entries(paths).map(([id,files])=>[id,files.map(path=>({path,sha256:hash(Buffer.from(fs.readFileSync(inside(ROOT,'src/godot-game/'+path),'utf8').trim()))}))]));
}
async function nativeCensus(context,{startupDirectory,selectedCohorts}={}){
 const cohorts=nativeCohorts(),chosen=selectedCohorts?cohorts.filter(x=>selectedCohorts.includes(x.id)):cohorts,source=content.normalizeCurrent(ROOT),before=source.files.map(x=>({path:x.path,sha256:x.sha256,bytes:x.bytes}));need(chosen.length>0&&(!selectedCohorts||chosen.length===selectedCohorts.length),'CENSUS_NATIVE_SELECTION');
 write(context.directory,'native-cohort-selection.json',{cohorts,selected:chosen.map(x=>x.id),independent:content.expected(),expectedCompleteUnion:cohorts.filter(x=>x.kind==='obligations').flatMap(x=>x.ids),fourViews:cohorts.at(-1).ids,perChildMs:60000,startupMs:15000,reason:'Separate exact category subsets preserve original15s startup tracer and ordinary60s native child bounds; no previous timeout is reused as pass',frozen:before});
 if(!startupDirectory){startupDirectory=inside(context.directory,'census-startup');fs.mkdirSync(startupDirectory);await runner.runTracer(ROOT,{...context,directory:startupDirectory});}
 const project=inside(startupDirectory,'project'),engine=content.document(startupDirectory,'engine.json'),startup=content.document(project,'tracer-runtime.json'),audio=content.document(ROOT,'src/godot-game/content/audio/events.json'),audioRoutes=Object.fromEntries(audio.events.map(x=>[x.category,x.routes.map(y=>y.id)])),effects=content.document(ROOT,'src/godot-game/content/packs/effects-portals.json'),map=content.document(ROOT,'src/godot-game/content/world/map.json'),access=content.document(ROOT,'src/godot-game/content/packs/camp-access.json'),effectFamilies=Object.fromEntries(effects.obligations.filter(x=>x.startsWith('FX-')).map(id=>[id,effects.effects.filter(x=>x.obligation===id).map(x=>x.id)])),ui=nativeUiAuthority(),dataLibraries=nativeDataAuthority(),results=[];
 need(hash(fs.readFileSync(engine.executable))===engine.sha256,'CENSUS_NATIVE_ENGINE');
 for(const cohort of chosen){
  const nonce=require('node:crypto').randomUUID(),name='census-native-'+cohort.id,request={nonce,cohort,audioRoutes,effectFamilies,ui,dataLibraries,rallyIds:map.rallies.map(x=>x.id),portalIds:map.portals.map(x=>x.id),siteIds:[...map.walls,...map.towers].map(x=>x.id).sort(),stashId:access.stash.id,seats:access.layouts.map(x=>x.seats.map(y=>[y.position[0]+80,y.position[1]])),nonmatchingResource:'ground_0'};need(map.native.nodes.some(x=>x.id===request.nonmatchingResource),'CENSUS_FILTER_SOURCE_CONTROL');fs.writeFileSync(inside(project,'census-request.json'),JSON.stringify(request));const driver='census-driver-'+cohort.id+'.gd';write(project,driver,Buffer.from(NATIVE_CENSUS_DRIVER));let command,error;
  try{command=await runner.command(context,name,engine.executable,['--headless','--path',project,'--script','res://'+driver],{timeout:60000});}catch(actual){error=actual;command=content.document(context.directory,name+'.command.json');}
  const file='census-'+nonce+'-result.json',actual=fs.existsSync(inside(project,file))?content.document(project,file):null,stages=fs.readdirSync(project).filter(x=>x.startsWith('census-'+nonce+'-stage-')).map(x=>({path:x,sha256:hash(fs.readFileSync(inside(project,x)))}));
  write(context.directory,name+'-observation.json',{cohort,nonce,command:command?{pid:command.pid,code:command.code,reason:command.reason,start:command.startedAt,finish:command.finishedAt}:null,actual,stages,error:error?.message??null});
  need(actual&&actual.pid===command.pid&&actual.nonce===nonce&&actual.runtime===startup.runtime&&actual.accepted===0,'CENSUS_NATIVE_IDENTITY',cohort.id);
  need(!error&&!actual.failed&&actual.complete&&actual.retirementControls.length===4&&actual.retirementControls.every(x=>x.pass)&&actual.outcomes.length===cohort.ids.length&&actual.outcomes.every(x=>x.pass)&&command.stderr.trim()===''&&!/Unicode parsing error|ERROR:|SCRIPT ERROR:|WARNING:|FATAL|leaked/i.test(command.stdout+command.stderr)&&command.stdout.includes('CENSUS_NATIVE_COMPLETE '+nonce+' PASS'),'CENSUS_NATIVE_CONTENT',cohort.id+' '+(error?.message??JSON.stringify(actual.outcomes.filter(x=>!x.pass).map(x=>({id:x.id,reason:x.reason})))));
  results.push({id:cohort.id,nonce,pid:command.pid,ids:actual.outcomes.map(x=>x.id),kind:cohort.kind,durationMs:Date.parse(command.finishedAt)-Date.parse(command.startedAt)});
 }
 const ids=results.filter(x=>x.kind==='obligations').flatMap(x=>x.ids),full=chosen.length===cohorts.length;if(full){assert.deepEqual([...ids].sort(),content.expected().obligations.map(x=>x.id).sort());assert.equal(new Set(ids).size,180);assert.deepEqual(results.filter(x=>x.kind==='inspection-views').flatMap(x=>x.ids),cohorts.at(-1).ids);}
 assert.deepEqual(content.normalizeCurrent(ROOT).files.map(x=>({path:x.path,sha256:x.sha256,bytes:x.bytes})),before);write(context.directory,'native-cohort-union.json',{full,actualObligations:ids,results,accepted:0});return{full,results,actualObligations:ids.length,accepted:0};
}
async function proveCensusSources(context){
 const model=content.normalizeCurrent(ROOT),before=model.files.map(x=>({path:x.path,bytes:x.bytes,sha256:x.sha256})),positive=content.validate(ROOT,structuredClone(model)),controls=[];
 write(context.directory,'census-inputs.json',{files:before,independent:content.expected(),model});
 need(positive.sourceComplete&&positive.valid.length===180&&positive.expected===180&&positive.covered.length===0&&!positive.complete,'CENSUS_REAL_CURRENT_POSITIVE');
 const expected=content.expected().obligations.map(x=>x.id).sort();assert.deepEqual(positive.sourceReady,expected);
 const trial=(id,mutate,assertion)=>{const subject=structuredClone(model);mutate(subject);const result=content.validate(ROOT,subject);need(!result.sourceComplete&&!result.complete&&result.covered.length===0,'CENSUS_INVALID_ACCEPTED',id);assertion(result);controls.push({id,valid:result.valid.map(x=>x.id),diagnostics:result.diagnostics,accepted:0});};
 trial('H06-duplicate-inflated180',x=>{x.outputs[179]=structuredClone(x.outputs[0]);},r=>need(r.valid.length===178&&r.diagnostics.some(x=>x.code==='SOURCE_OBLIGATION_MISSING'),'CENSUS_DUPLICATE_CONTROL'));
 trial('H08-missing-unique179',x=>{x.outputs.pop();},r=>need(r.valid.length===179,'CENSUS_MISSING_CONTROL'));
 trial('H08-empty-dependency',x=>{x.outputs[0].dependencies=[];},r=>need(r.valid.length===179,'CENSUS_DEPENDENCY_CONTROL'));
 trial('H08-absent-dependency',x=>{x.outputs[0].dependencies.push('file.absent');},r=>need(r.valid.length===179,'CENSUS_ABSENT_DEPENDENCY_CONTROL'));
 trial('H10-fractional-version',x=>{x.outputs[0].version=1.0000000001;},r=>need(r.valid.length===179,'CENSUS_PRECISION_CONTROL'));
 trial('H07-incoherent-shared-identity',x=>{x.outputs[0].sharedFileId=x.outputs.find(y=>y.file!==x.outputs[0].file).sharedFileId;},r=>need(r.valid.length===179,'CENSUS_SHARED_CONTROL'));
 trial('H27-wrong-valid-selector',x=>{x.outputs[0].definitionPointers=x.outputs[1].definitionPointers;},r=>need(r.valid.length===179,'CENSUS_SELECTOR_CONTROL'));
 trial('H28-empty-pack',x=>{x.outputs=[];},r=>need(r.valid.length===0&&r.diagnostics.filter(x=>x.code==='SOURCE_OBLIGATION_MISSING').length===180,'CENSUS_EMPTY_CONTROL'));
 trial('H29-wrong-case-file',x=>{x.files.find(f=>f.role==='runtime').path=x.files.find(f=>f.role==='runtime').path.replace('src/','Src/');},r=>need(r.diagnostics.some(x=>x.code==='OUTPUT_INVALID'),'CENSUS_CASE_CONTROL'));
 trial('H29-lookalike-file',x=>{x.files[0].path=x.files[0].path.replace('.md','-copy.md');},r=>need(r.diagnostics.some(x=>x.code==='OUTPUT_INVALID'),'CENSUS_ALIAS_CONTROL'));
 trial('H30-invalid-sibling',x=>{x.files.find(f=>f.id===x.outputs[0].file).sha256='0'.repeat(64);},r=>need(r.valid.length>0&&r.valid.length<180,'CENSUS_SIBLING_CONTROL'));
 trial('H32-false-acceptance',x=>{x.outputs[0].acceptance.accepted=true;},r=>need(r.valid.length===179,'CENSUS_ACCEPTANCE_CONTROL'));
 const shuffled=structuredClone(model);shuffled.outputs.reverse();shuffled.files.reverse();const reordered=content.validate(ROOT,shuffled);assert.deepEqual(reordered.sourceReady,positive.sourceReady);need(reordered.sourceComplete,'CENSUS_ORDER_CONTROL');controls.push({id:'H09-H34-shuffled-repeated-identities',sourceReady:reordered.sourceReady,accepted:0});
 const source=await content.validateCompletedSources(ROOT,model);need(source.archiveObservations.length===14&&source.files===model.files.length,'CENSUS_RETAINED_SOURCE_CLOSURE');
 const publicationDirectory=inside(context.directory,'current-publication-controls');fs.mkdirSync(publicationDirectory);const publication=await require('./provenance.cjs').currentPublication({...context,directory:publicationDirectory});
 assert.deepEqual(content.normalizeCurrent(ROOT).files.map(x=>({path:x.path,bytes:x.bytes,sha256:x.sha256})),before);
 write(context.directory,'census-controls.json',controls);write(context.directory,'census-source-proof.json',source);
 return {model,before,positive,expected,source,publication,controls};
}

async function census(context){
 const {model,before,positive,expected,source,publication,controls}=await proveCensusSources(context);
 const nativeDirectory=inside(context.directory,'current-native-controls');fs.mkdirSync(nativeDirectory);await runner.runTracer(ROOT,{...context,directory:nativeDirectory});const native=await nativeCensus(context,{startupDirectory:nativeDirectory});need(native.full&&native.actualObligations===180,'CENSUS_NATIVE_COMPLETE_UNION');
 assert.deepEqual(content.normalizeCurrent(ROOT).files.map(x=>({path:x.path,bytes:x.bytes,sha256:x.sha256})),before);
 return{sourceReady:positive.valid.length,independentExpected:expected.length,runtimePaths:model.files.filter(x=>x.role==='runtime').length,accepted:0,source,publication,native,controls:controls.length,scope:'Actual source/compiled/native production reachability; final exact policy and current producer gates are separately mandatory, all later appearance/listening/platform judgments pending'};
}
runner.registerCase('content.census',{...metadata,title:'Actual complete independently held-out source census and atomic current native reachability',order:64,setup:'All actual current production shards/resources and independently fixed180 authority; bounded retained source archives, completed Core decoder and actual current Godot Root',actions:['Decode and authenticate actual distinct resources/dependencies/shared mappings against independent180 IDs','Reject duplicate-inflated totals,missing unique IDs,partial/empty dependencies,precision and conflicting paths','Preserve valid sibling visibility and order-independent identities without accepted judgment credit','Publish actual full retained source generations atomically and prove interruption/recovery/restored identity','Reach each actual current resource through the genuine compiled native Root and current source/assembly/ALC/PID/nonce closure'],expected:'All180 actual source resources and exact independent controls pass; accepted remains0 and later judgment owners stay pending',reset:'Owned disposable subjects only; no production mutation; original legacy fixtures preserved',evidence:['census-inputs.json','census-controls.json','census-source-proof.json','result.json']},census);

function proveHeldoutPackagedPaths(model,positive){
 const canonical='src/godot-game/content/packs/world.json';
 const file=model.files.find(row=>row.path===canonical);
 need(file,'NATIVE_PERMISSIONS_PACKAGED_CONTROL_FILE',canonical);
 const encoded=canonical.replace('world.json','world%2ejson');
 assert.throws(()=>inside(ROOT,encoded),error=>error.message===`PATH_INVALID ${encoded}`);
 const encodedTrial=structuredClone(model);
 encodedTrial.files.find(row=>row.id===file.id).path=encoded;
 const encodedResult=content.validate(ROOT,encodedTrial);
 const encodedError=encodedResult.diagnostics.find(row=>row.id===file.id&&row.path===encoded);
 need(!encodedResult.sourceComplete&&encodedError?.code==='OUTPUT_INVALID'&&encodedError.message===`FILE_ROLE_IDENTITY ${encoded}`,
  'NATIVE_PERMISSIONS_ENCODED_PATH',encoded);

 const conflict=structuredClone(model);
 const conflictingId='file.heldout-conflicting-world';
 conflict.files.push({...file,id:conflictingId,sha256:'0'.repeat(64)});
 const conflictResult=content.validate(ROOT,conflict);
 const conflictError=conflictResult.diagnostics.find(row=>row.id===conflictingId);
 need(!conflictResult.sourceComplete&&conflictError?.code==='OUTPUT_INVALID'&&conflictError.path===canonical&&
  conflictError.message===`FILE_DUPLICATE ${canonical}`,'NATIVE_PERMISSIONS_CONFLICTING_PATH',canonical);
 assert.deepEqual(conflictResult.sourceReady,positive.sourceReady);

 const badBinding=structuredClone(model);
 badBinding.files.find(row=>row.id===file.id).sha256='0'.repeat(64);
 const first=content.validate(ROOT,badBinding);
 badBinding.files.reverse();badBinding.outputs.reverse();
 const reordered=content.validate(ROOT,badBinding);
 const healthy=model.outputs.filter(row=>row.file!==file.id&&!row.dependencies.includes(file.id)).map(row=>row.id).sort();
 const diagnostics=result=>result.diagnostics.map(row=>JSON.stringify(row)).sort();
 need(!first.sourceComplete&&!reordered.sourceComplete&&healthy.length>0&&healthy.length<180&&
  first.diagnostics.some(row=>row.id===file.id&&row.path===canonical&&row.message===`FILE_HASH_MISMATCH ${canonical}`),
  'NATIVE_PERMISSIONS_BAD_PATH_VISIBLE',canonical);
 assert.deepEqual(first.sourceReady,healthy);
 assert.deepEqual(reordered.sourceReady,healthy);
 assert.deepEqual(diagnostics(first),diagnostics(reordered));

 const shared=new Map();
 for(const row of model.outputs){
  if(!shared.has(row.sharedFileId))shared.set(row.sharedFileId,[]);
  shared.get(row.sharedFileId).push({id:row.id,file:row.file,selectors:row.definitionPointers});
 }
 const sharedGroups=[...shared].filter(([,rows])=>rows.length>1).map(([id,rows])=>({id,rows}));
 need(sharedGroups.length>0&&sharedGroups.every(({rows})=>rows.every(row=>row.file===rows[0].file&&
  positive.sourceReady.includes(row.id))&&new Set(rows.map(row=>row.id)).size===rows.length),
  'NATIVE_PERMISSIONS_SHARED_IDENTITIES');
 return [
  {id:'H07-valid-distinct-shared-file-outputs',sharedGroups},
  {id:'H27-conflicting-exact-packaged-path',diagnostic:conflictError,valid:conflictResult.sourceReady},
  {id:'H29-percent-encoded-packaged-path',diagnostic:encodedError,valid:encodedResult.sourceReady},
  {id:'H30-bad-packaged-binding-permuted',diagnostics:first.diagnostics,valid:healthy},
 ];
}

const PERMISSION_DISCOVERY=String.raw`
$ErrorActionPreference = 'Stop'
$output = & $env:KZ_DOTNET $env:KZ_TEST_ASSEMBLY --filter-method $env:KZ_PERMISSION_METHOD --list-tests json
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
$discovery = ($output -join [Environment]::NewLine) | ConvertFrom-Json
$bridge = [Reflection.Assembly]::LoadFrom($env:KZ_XUNIT_COMMON).GetType('Xunit.Sdk.UniqueIDGenerator', $true).GetMethod('ForTest', [type[]]@([string], [int]))
foreach ($test in $discovery.tests) {
    $testUid = $bridge.Invoke($null, [object[]]@($test.uid, 0))
    $test | Add-Member -NotePropertyName testUid -NotePropertyValue $testUid
}
$discovery | ConvertTo-Json -Depth 12 -EscapeHandling EscapeNonAscii
`;

async function proveCompiledNativePermissions(context){
 const path=require('node:path'),phase1=require('../../verification/check.cjs');
 const manifest=content.document(ROOT,'src/godot-game/tools/verification/required-cases.json');
 const pins=content.document(ROOT,'src/godot-game/tools/verification/toolchain.json');
 const sourceInputs=()=>phase1.snapshot(ROOT,{inputs:[...manifest.inputs]});
 const source=sourceInputs();
 need(process.versions.node===pins.node,'NATIVE_PERMISSIONS_NODE_PIN');
 const dotnet='C:/Program Files/dotnet/dotnet.exe',env=require('./defenses.cjs').isolated();
 const version=await runner.command(context,'native-permissions-sdk',dotnet,['--version'],{env});
 need(version.stdout.trim()===pins.sdk&&version.stderr==='','NATIVE_PERMISSIONS_SDK_PIN');
 const build=await runner.command(context,'native-permissions-strict-build',dotnet,
  ['build',inside(ROOT,'src/godot-game/KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false',
   '-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env});
 need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout)&&build.stderr==='','NATIVE_PERMISSIONS_STRICT_BUILD');
 const bin='src/godot-game/tests/verification/bin/Debug/net10.0',assembly=inside(ROOT,`${bin}/KingdomZombies.Phase1.Tests.dll`);
 const xunit=inside(ROOT,`${bin}/xunit.v3.common.dll`);
 const pinnedBinaries=[...fs.readdirSync(inside(ROOT,bin),{withFileTypes:true}).filter(row=>row.isFile())
  .map(row=>inside(ROOT,`${bin}/${row.name}`)),inside(ROOT,'src/godot-game/core/bin/Debug/net10.0/KingdomZombies.Core.dll'),
  inside(ROOT,'src/godot-game/game/.godot/mono/temp/bin/Debug/KingdomZombies.dll'),
  inside(ROOT,'src/godot-game/tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll'),dotnet];
 const ref=file=>({file,sha256:hash(fs.readFileSync(file))});
 const binaryInputs=pinnedBinaries.map(ref);
 write(context.directory,'native-permissions-inputs.json',{source,binaryInputs,sdk:pins.sdk});
 const testMethod='Check_PresenterNative_RequiresExactGuardedMember';
 const typeName='ContentNativePermissionTests',namespace='KingdomZombies.Phase1.Tests';
 const method=`${namespace}.${typeName}.${testMethod}`;
 const discoveryCommand=await runner.command(context,'native-permissions-discovery','pwsh',
  ['-NoLogo','-NoProfile','-NonInteractive','-EncodedCommand',Buffer.from(PERMISSION_DISCOVERY,'utf16le').toString('base64')],
  {env:{...env,KZ_DOTNET:dotnet,KZ_TEST_ASSEMBLY:assembly,KZ_PERMISSION_METHOD:method,KZ_XUNIT_COMMON:xunit}});
 need(discoveryCommand.stderr==='','NATIVE_PERMISSIONS_DISCOVERY_STDERR');
 const discovery=content.parse(Buffer.from(discoveryCommand.stdout),'native permission discovery');
 const tests=discovery.tests;
 need(Array.isArray(tests)&&tests.length===251&&new Set(tests.map(test=>test.uid)).size===251&&
  new Set(tests.map(test=>test.testUid)).size===251&&tests.every(test=>/^[a-f0-9]{64}$/.test(test.uid)&&
   /^[a-f0-9]{64}$/.test(test.testUid)&&test.type.namespace===namespace&&test.type.typeName===typeName&&
   test.type.methodName===testMethod&&test.displayName.startsWith(`${method}(`)),
  'NATIVE_PERMISSIONS_DISCOVERY_SELECTION');
 const child='45s',launcherMs=60000;
 write(context.directory,'native-permissions-selection.json',{method,child,launcherMs,bridge:'Xunit.Sdk.UniqueIDGenerator.ForTest',
  xunit:ref(xunit),tests});
 console.log(`Native permissions selected ${tests.length} cases`);
 const results=[];
 for(const [ordinal,test] of tests.entries()){
  const name=`native-permissions-${String(ordinal+1).padStart(3,'0')}-${test.uid.slice(0,16)}`;
  const directory=inside(context.directory,name);fs.mkdirSync(directory);
  const command=await runner.command({...context,directory},'xunit',dotnet,
   [assembly,'--filter-uid',test.uid,'--report-xunit-ctrf','--report-xunit-ctrf-filename','tests.json',
    '--results-directory',directory,'--no-ansi','--progress','off','--parallel','none','--explicit','on','--timeout',child],
   {timeout:launcherMs,env:{...env,PHASE1_RESULT_DIRECTORY:directory}});
  const report=content.document(directory,'tests.json').results,[result]=report.tests;
  const summary=report.summary;
  need(command.stderr===''&&report.tool.version.split('+')[0]==='4.0.2'&&report.tests.length===1&&
   result.status==='passed'&&result.name===test.displayName&&result.extra.method===testMethod&&
   result.extra.id===test.testUid&&summary.tests===1&&summary.passed===1&&summary.failed===0&&summary.skipped===0&&
   !summary.pending&&!summary.other&&!report.extra?.suites?.some(suite=>suite.errors?.length),
   'NATIVE_PERMISSIONS_EXECUTION',test.uid);
  results.push({uid:test.uid,testUid:test.testUid,pid:command.pid,
   records:['xunit.command.json','xunit.exit.json','xunit.stdout.log','xunit.stderr.log','tests.json']
    .map(record=>ref(path.join(directory,record)))});
  if(results.length%25===0||results.length===tests.length)console.log(`Native permissions passed ${results.length}/${tests.length}`);
 }
 assert.deepEqual(results.map(row=>row.uid),tests.map(row=>row.uid));
 assert.deepEqual(sourceInputs(),source);
 assert.deepEqual(pinnedBinaries.map(ref),binaryInputs);
 write(context.directory,'native-permissions-execution.json',{selected:tests.length,passed:results.length,failed:0,skipped:0,results});
 return {selected:tests.length,passed:results.length,skipped:0};
}

async function nativePermissions(context){
 const {model,before,positive,source,publication,controls}=await proveCensusSources(context);
 const paths=proveHeldoutPackagedPaths(model,positive);
 write(context.directory,'native-permissions-packaged-controls.json',paths);
 const compiled=await proveCompiledNativePermissions(context);
 assert.deepEqual(content.normalizeCurrent(ROOT).files.map(x=>({path:x.path,bytes:x.bytes,sha256:x.sha256})),before);
 return {sourceReady:positive.valid.length,authenticatedArchives:source.archiveObservations.length,
  heldoutCensusControls:controls.length,packagedControls:paths.map(row=>row.id),publication,compiled,accepted:0};
}

runner.registerCase('content.native-permissions',{...metadata,title:'Exact final native guarded permissions and held-out content boundaries',
 order:65,requirements:['ASSET-01','ASSET-02','ASSET-03','ASSET-04','ASSET-05'],acceptance:['AC-01','AC-16','AC-19'],
 rationale:'Every permitted native operation must belong to an actual exact guarded member, and invalid content must stay visible',
 setup:'Current authored source, pinned SDK and xUnit assembly, exact completed package and independent 180-obligation authority',
 actions:['Authenticate actual independently defined obligations and all retained current sources',
  'Exercise independent shared, conflicting, encoded and reordered packaged bindings and interrupted publication',
  'Build the current solution, discover every exact guarded-member test UID and execute each one with the existing limits',
  'Require authentic passing guarded positives and rejected wrong-path, sibling, unguarded, deferred and worker callsites'],
 expected:'Every exact current native permission case and held-out content control passes with zero skips and zero accepted obligations',
 reset:'Use independent invocation-owned directories and leave current source and executable evidence fixed',
 evidence:['census-controls.json','census-source-proof.json','native-permissions-packaged-controls.json','native-permissions-inputs.json',
  'native-permissions-selection.json','native-permissions-execution.json','result.json']},nativePermissions);

Object.assign(module.exports,{census,nativeCohorts,nativeCensus,nativePermissions});

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
