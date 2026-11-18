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
 assert.throws(()=>bytes(root,'production/phase4/Audio-interface-ambience.zip'),/PATH_CASE/);controls.push({id:'wrongcase-canonical',rejected:true});
 for(const path of ['production/phase4/audio-interface-ambience-copy.zip','production/phase4/other.zip']){const wrong=inside(root,path);fs.writeFileSync(wrong,Buffer.alloc(32*MIB+1,0));assert.throws(()=>bytes(root,path),/FILE_INVALID/);controls.push({id:path.endsWith('-copy.zip')?'lookalike-over32MiB':'default-over32MiB',rejected:true});fs.unlinkSync(wrong);}
 const other=inside(root,'production/phase4/other.zip');fs.writeFileSync(other,Buffer.alloc(32*MIB,0));need(bytes(root,'production/phase4/other.zip').length===32*MIB,'SOURCE_BOUNDARY_DEFAULT_LIMIT');controls.push({id:'default-exact32MiB',accepted:true});
 fs.writeFileSync(file,fs.readFileSync(inside(ROOT,INTERFACE)));need(sourceRef(root,INTERFACE,before.sha256).length===before.bytes,'SOURCE_BOUNDARY_RESTORED');need(hash(fs.readFileSync(inside(ROOT,INTERFACE)))===before.sha256,'SOURCE_BOUNDARY_PRODUCTION_CHANGED');controls.push({id:'restored-real-canonical',accepted:true});write(context.directory,'boundaries.json',controls);
 return {actual:before,controls:controls.length,defaultSourceMiB:32,exactCanonicalSourceMiB:64,memberMiB:32,expandedMiB:128,aggregateMiB:256,scope:'Source byte boundary only; no all180/native-policy/acceptance credit'};
}
runner.registerCase('content.source-boundaries',{...metadata,title:'Exact canonical source archive compatibility with independent boundary controls'},sourceBoundaries);
module.exports={sourceBoundaries};
