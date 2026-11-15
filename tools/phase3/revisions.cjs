'use strict';
// Phase-owned finalization only: original executions and review bytes remain immutable.
const fs=require('node:fs');
const {spawnSync}=require('node:child_process'),e=require('./evidence.cjs'),baseline=require('../phase1/check.cjs');
const phase='.planning/phases/03-four-player-lan-foundation/';
const metadata=new Set(['.planning/STATE.md','.planning/ROADMAP.md','.planning/REQUIREMENTS.md',
  ...['03-REVIEW.md','03-REVIEW.json','03-REVIEW-FIX.md','03-REVIEW.iter2.md','03-REVIEW-FIX.iter2.md','03-EVIDENCE.json','03-VISUAL-REVIEW.md',
    '03-04-SUMMARY.md','03-04-CHECKPOINT.md','03-CONTINUATION.md','03-VERIFICATION.md','03-VALIDATION.md',
    '03-SECURITY.md','03-UI-REVIEW.md','03-REVIEW-DISPOSITION.md','COVERAGE.md'].map(file=>phase+file)]);
function git(root,args,encoding='utf8'){
  const result=spawnSync('git',args,{cwd:root,encoding,windowsHide:true,timeout:15000,maxBuffer:32*1024*1024});
  e.requireTrue(!result.error&&result.status===0,'GIT_VALIDATION_UNAVAILABLE '+args[0]);return result.stdout;
}
function commit(root,revision){
  e.requireTrue(typeof revision==='string'&&/^[a-f0-9]{40}$/.test(revision),'REVISION_MALFORMED');
  e.requireTrue(git(root,['rev-parse','--verify',revision+'^{commit}']).trim()===revision,'REVISION_NOT_COMMIT');
}
function committedInputs(root,revision,input){
  const snapshot=baseline.snapshot(root,require('./check.cjs').inputManifest());
  e.requireTrue(snapshot.sha256===input,'COMMITTED_INPUT_FINGERPRINT');
  e.requireTrue(snapshotAt(root,revision,require('./check.cjs').inputManifest()).sha256===input,'COMMITTED_INPUT_CHANGED');
}
function snapshotAt(root,revision,manifest){
  const included=file=>manifest.inputs.some(prefix=>file===prefix||file.startsWith(prefix+'/'))&&
    !file.split('/').some(segment=>['bin','obj','.godot'].includes(segment));
  const entries=git(root,['ls-tree','-r','-z',revision]).split('\0').filter(Boolean).map(row=>{
    const [header,file]=row.split('\t');return {file,mode:header.split(' ')[0],oid:header.split(' ')[2]};
  }).filter(row=>included(row.file));
  for(const prefix of manifest.inputs)e.requireTrue(entries.some(row=>row.file===prefix||row.file.startsWith(prefix+'/')),'COMMITTED_INPUT_MISSING '+prefix);
  const result=spawnSync('git',['cat-file','--batch'],{cwd:root,input:entries.map(row=>row.oid).join('\n')+'\n',windowsHide:true,timeout:15000,maxBuffer:64*1024*1024});
  e.requireTrue(!result.error&&result.status===0,'GIT_VALIDATION_UNAVAILABLE cat-file');
  let offset=0;const files=[];
  for(const row of entries){
    e.requireTrue(['100644','100755'].includes(row.mode),'COMMITTED_INPUT_MODE');
    const end=result.stdout.indexOf(10,offset),header=result.stdout.subarray(offset,end).toString('utf8').split(' '),size=Number(header[2]);
    e.requireTrue(header[0]===row.oid&&header[1]==='blob'&&Number.isSafeInteger(size)&&size>=0,'COMMITTED_BLOB_INVALID');
    offset=end+1;const bytes=result.stdout.subarray(offset,offset+size);offset+=size+1;
    e.requireTrue(bytes.length===size,'COMMITTED_BLOB_TRUNCATED');files.push({path:row.file,sha256:e.hash(bytes)});
  }
  files.sort((a,b)=>a.path.localeCompare(b.path,'en'));return {sha256:e.hash(JSON.stringify(files)),files};
}
function relation(root,tested,current,input){
  commit(root,tested);commit(root,current);
  e.requireTrue(git(root,['rev-parse','HEAD']).trim()===current,'REVISION_HEAD_MISMATCH');
  git(root,['merge-base','--is-ancestor',tested,current]);
  const changes=git(root,['log','-m','--format=','--name-only',tested+'..'+current]).split(/\r?\n/).filter(Boolean);
  e.requireTrue(changes.every(file=>metadata.has(file)),'REVISION_NON_METADATA '+changes.find(file=>!metadata.has(file)));
  committedInputs(root,tested,input);committedInputs(root,current,input);
  // Force a successful index/worktree query too; unknown Git state never means clean.
  const dirty=git(root,['status','--porcelain=v1','--untracked-files=all']);
  e.requireTrue(!dirty.split(/\r?\n/).some(row=>row.length>3&&!metadata.has(row.slice(3))),'REVISION_DIRTY_NON_METADATA');
  return {testedRevision:tested,validatedRevision:current,execution:'original'};
}
function committedArtifact(root,revision,reference){
  e.inside(root,reference.path);
  e.requireTrue(e.hash(git(root,['show',revision+':'+reference.path],null))===reference.sha256,'COMMITTED_ARTIFACT_CHANGED');
}
function review(root,record,reference,context){
  e.requireTrue(record.status==='complete'&&record.input===context.input&&record.tool===context.tool,'WHOLE_PHASE_REVIEW_REQUIRED');
  e.requireTrue(context.buildRevision===record.revision,'REVIEW_BUILD_REVISION_REQUIRED');
  relation(root,record.revision,context.revision,context.input);
  committedArtifact(root,context.revision,reference);committedArtifact(root,context.revision,record.report);
}
module.exports={relation,review,committedArtifact,snapshotAt,metadata};
