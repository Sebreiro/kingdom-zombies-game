'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const baseline=require('../phase1/check.cjs');
const {decodePng}=require('../phase2/rendered.cjs');
const hash=bytes=>crypto.createHash('sha256').update(bytes).digest('hex');
function requireTrue(value,message){if(!value)throw new Error(message);}
function read(file){return JSON.parse(fs.readFileSync(file,'utf8'));}
function inside(root,relative){
  const file=baseline.inside(root,relative),canonicalRoot=fs.realpathSync(root);
  let current=file;
  while(!fs.existsSync(current)){const parent=path.dirname(current);requireTrue(parent!==current,'PATH_MISSING');current=parent;}
  const canonical=fs.realpathSync(current),relativeCanonical=path.relative(canonicalRoot,canonical);
  requireTrue(relativeCanonical!== '..'&&!relativeCanonical.startsWith('..'+path.sep)&&!path.isAbsolute(relativeCanonical),'PATH_SYMLINK_ESCAPE');
  return file;
}
function exact(expected,actual,label){
  requireTrue(Array.isArray(expected)&&expected.length>0&&Array.isArray(actual),'EXACT_EMPTY '+label);
  requireTrue(new Set(expected).size===expected.length&&new Set(actual).size===actual.length,'EXACT_DUPLICATE '+label);
  requireTrue(expected.length===actual.length&&actual.every(id=>expected.includes(id)),'EXACT_MEMBERSHIP '+label);
}
function timing(row){
  requireTrue(row.clock==='coordinator-hrtime','TIMING_CLOCK');
  requireTrue(row.actor===row.expectedActor&&row.session===row.expectedSession&&Number.isSafeInteger(row.revision)&&row.revision>=row.minimumRevision,'TIMING_SUBJECT');
  requireTrue(row.drawn===true&&row.drawFrame>0&&row.visible===true&&row.insideCamera===true&&row.imageBound===true,'TIMING_DRAW');
  requireTrue(row.eventAcknowledged===true&&row.transformChanged===true,'TIMING_EVENT_TRANSFORM');
  requireTrue(['physical-SendInput','window-message','synthetic-native-ParseInputEvent'].includes(row.provenance),'TIMING_PROVENANCE');
  const values=['deliveryBefore','deliveryAfter','observedBefore','observedAfter','uncertaintyNanoseconds'];
  requireTrue(values.every(key=>typeof row[key]==='string'&&/^\d+$/.test(row[key])),'TIMING_INTEGER');
  const [before,after,observedBefore,observedAfter,uncertainty]=values.map(key=>BigInt(row[key]));
  requireTrue(after>=before&&observedBefore>=before&&observedAfter>=observedBefore&&observedAfter>=after,'TIMING_ORDER');
  const upper=observedAfter-before+uncertainty;requireTrue(upper<=250000000n,'TIMING_OVER_LIMIT');
  return {upperNanoseconds:String(upper),physicalInputCredit:row.provenance==='physical-SendInput'};
}
function stopped(row){
  requireTrue(row.clock==='coordinator-hrtime'&&row.actor===row.expectedActor&&row.session===row.expectedSession,'STOP_SUBJECT_CLOCK');
  requireTrue([row.positionSubunits,row.hostPositionSubunits,row.revision,row.minimumRevision].every(Number.isSafeInteger)&&row.revision>=row.minimumRevision,'STOP_POSITION_REVISION');
  requireTrue(BigInt(row.observedAfter)-BigInt(row.releaseBefore)+BigInt(row.uncertaintyNanoseconds)>=0n&&BigInt(row.observedAfter)-BigInt(row.releaseBefore)+BigInt(row.uncertaintyNanoseconds)<=500000000n,'STOP_TIME');
  requireTrue(Math.abs(row.positionSubunits-row.hostPositionSubunits)<=50,'STOP_DISTANCE');return row;
}
function identity(row,context){for(const key of ['nonce','input','tool','platform','revision'])requireTrue(typeof context[key]==='string'&&row[key]===context[key],'RESULT_IDENTITY '+key);}
function artifact(context,row){
  requireTrue(typeof row.path==='string'&&/^[a-f0-9]{64}$/.test(row.sha256),'ARTIFACT_DECLARATION');
  const file=inside(context.directory,row.path);requireTrue(hash(fs.readFileSync(file))===row.sha256,'ARTIFACT_HASH '+row.path);return file;
}
function image(context,row,peer){
  const file=artifact(context,row);decodePng(file,1280,720);
  const state=read(artifact(context,{path:row.statePath,sha256:row.stateSha256}));
  requireTrue(state.nonce===context.nonce&&state.pid===peer.pid&&state.role===peer.role&&state.profile===peer.profile,'IMAGE_SUBJECT');
  requireTrue(state.drawn===true&&state.drawFrame>0&&state.checkpoint===row.captureId&&state.width===1280&&state.height===720,'IMAGE_DRAW');
  requireTrue(state.snapshot?.SessionId===peer.session&&state.bindingId===peer.binding&&state.playerId===peer.profile,'IMAGE_BINDING');
  requireTrue(state.snapshot.Revision>=peer.minimumRevision,'IMAGE_REVISION');
  const visual=state.actors?.find(actor=>actor.actorId===peer.actor);
  requireTrue(visual&&visual.visible&&visual.insideCamera&&Number.isFinite(visual.screenX)&&Number.isFinite(visual.facing),'IMAGE_ACTOR');
  return state;
}
function peers(context,item,result){
  exact(item.roles,result.peers?.map(peer=>peer.role),'peers '+item.id);
  const pids=new Set(),profiles=new Set(),actors=new Set(),bindings=new Set();
  for(const peer of result.peers){
    requireTrue(Number.isSafeInteger(peer.pid)&&peer.pid>0&&!pids.has(peer.pid),'PEER_PID');pids.add(peer.pid);
    requireTrue(typeof peer.profile==='string'&&peer.profile.length>0,'PEER_PROFILE');
    if(!item.profileRestart)requireTrue(!profiles.has(peer.profile),'PEER_PROFILE_DUPLICATE');profiles.add(peer.profile);
    const launch=read(artifact(context,peer.launch));
    requireTrue(launch.pid===peer.pid&&launch.profile===peer.profile&&launch.role===peer.role&&launch.nonce===context.nonce,'PEER_LAUNCH');
    requireTrue(launch.headless===!item.needsGameWindow,'PEER_RENDER_MODE');
    artifact(context,peer.log);
    const logs=fs.readFileSync(inside(context.directory,peer.log.path),'utf8');
    requireTrue(!/(?:SCRIPT ERROR:|ERROR:|Unhandled exception|FATAL)/.test(logs),'PEER_ERROR_LOG');
    if(item.needsGameWindow){
      requireTrue(peer.session&&peer.actor&&peer.binding&&Number.isSafeInteger(peer.minimumRevision),'PEER_BINDING');
      requireTrue(!actors.has(peer.actor)&&!bindings.has(peer.binding),'PEER_ACTOR_BINDING_DUPLICATE');actors.add(peer.actor);bindings.add(peer.binding);
      exact(item.images,peer.images?.map(row=>row.checkpoint),'images '+item.id+'/'+peer.role);
      for(const row of peer.images)image(context,row,peer);
    }else{
      requireTrue(!peer.images?.length&&result.noRenderedOrPhysicalInputCredit===true,'HEADLESS_CREDIT');
      const rows=fs.readFileSync(artifact(context,peer.frames),'utf8').trim().split('\n').map(JSON.parse);
      requireTrue(rows.length>1&&rows.every(row=>row.nonce===context.nonce&&row.pid===peer.pid),'HEADLESS_OBSERVATION');
      requireTrue(rows.some(row=>row.profile?.PlayerId===peer.profile),'HEADLESS_PROFILE');
    }
  }
}
function reconcile(selected,results,context){
  exact(selected.map(row=>row.id),results.map(row=>row.id),'cases');
  for(const result of results){
    const item=selected.find(row=>row.id===result.id);identity(result,context);requireTrue(result.status==='pass','RESULT_NOT_PASS '+result.id);
    if(item.roles?.length)peers(context,item,result);
    else{
      requireTrue(result.artifacts?.length>=2,'SUBJECT_LOG_REQUIRED');exact(result.artifacts.map(row=>row.path),result.artifacts.map(row=>row.path),'artifacts');
      for(const row of result.artifacts)artifact(context,row);
      if(item.implementation==='csharp'){
        requireTrue(result.artifacts.some(row=>row.path===result.testReport),'TEST_REPORT_REQUIRED');
        const report=read(inside(context.directory,result.testReport));
        requireTrue(report.results.summary.tests===1&&report.results.summary.passed===1&&report.results.summary.failed===0,'TEST_EXECUTION_COUNT');
      }
    }
    if(item.id==='rendered.timing'){
      const expected=[];for(const origin of item.roles)for(const observer of item.roles.filter(role=>role!==origin))for(const action of ['start','reversal','stop','focus-stop'])expected.push(origin+'/'+observer+'/'+action);
      exact(expected,result.timings?.map(row=>row.origin+'/'+row.observer+'/'+row.action),'timing matrix');
      for(const row of result.timings){requireTrue(row.provenance==='physical-SendInput','PHYSICAL_TIMING_REQUIRED');timing(row);}
      exact(item.roles.flatMap(origin=>item.roles.map(observer=>origin+'/'+observer)),result.stopped?.map(row=>row.origin+'/'+row.observer),'stopped matrix');
      for(const row of result.stopped)stopped(row);
    }
  }return results;
}
function loadReport(root,reference,cases){
  const file=inside(root,reference.path);requireTrue(hash(fs.readFileSync(file))===reference.sha256,'REPORT_HASH');const report=read(file);
  requireTrue(report.schemaVersion===2&&report.status==='pass'&&Number.isSafeInteger(report.startedAt)&&report.finishedAt>=report.startedAt,'REPORT_SCHEMA_STATUS');
  exact(report.selected,report.results.map(row=>row.id),'report');
  const selected=report.selected.map(id=>{const row=cases.find(item=>item.id===id);requireTrue(row,'REPORT_UNKNOWN');return row;});
  reconcile(selected,report.results,{...report,directory:path.dirname(file)});return {...report,path:reference.path,sha256:reference.sha256};
}
function verifyCompletion(root,index,cases,current){
  requireTrue(index.schemaVersion===1&&index.scope==='Windows 1280x720 local; no VM'&&index.selection,'COMPLETION_SCHEMA');
  const selectionFile=artifact({directory:root},index.selection),selection=read(selectionFile);
  exact(cases.map(row=>row.id),selection.selected.phase3,'COMPLETION_SELECTION');
  requireTrue(index.finalReport&&Array.isArray(index.history),'COMPLETION_FINAL_REQUIRED');
  const final=loadReport(root,index.finalReport,cases);identity(final,current);exact(selection.selected.phase3,final.selected,'final selection');
  requireTrue(final.selectionSha256===index.selection.sha256&&final.prelaunchReview?.sha256,'PRELAUNCH_BINDING');
  const review=artifact({directory:root},final.prelaunchReview);requireTrue(read(review).status==='complete','PRELAUNCH_REVIEW');
  for(const reference of index.history){const file=inside(root,reference.path);requireTrue(hash(fs.readFileSync(file))===reference.sha256,'HISTORY_HASH');const prior=read(file);requireTrue(prior.finishedAt<=final.startedAt&&prior.nonce!==final.nonce,'LATER_OR_DUPLICATE_REPORT');}
  for(const result of final.results)for(const peer of result.peers??[])for(const picture of peer.images??[]){
    const reviews=(index.reviews??[]).filter(row=>row.id===result.id&&row.role===peer.role&&row.checkpoint===picture.checkpoint&&row.imageSha256===picture.sha256&&row.reportSha256===final.sha256);
    requireTrue(reviews.length===1&&reviews[0].verdict==='pass'&&reviews[0].reviewer?.trim()&&reviews[0].notes?.trim(),'APPEARANCE_REVIEW_REQUIRED');
  }
  requireTrue(index.inherited&&index.inherited.phase2&&index.inherited.phase1,'INHERITED_EVIDENCE_REQUIRED');
  for(const family of ['phase1','phase2']){const required=selection.selected[family];exact(required,index.inherited[family].results?.map(row=>row.id),'inherited '+family);requireTrue(index.inherited[family].results.every(row=>row.status==='pass'&&row.executedNow===true&&row.report),'INHERITED_RESULT');}
  return {status:'pass',selected:final.selected,report:final.path};
}
module.exports={hash,read,inside,requireTrue,exact,timing,stopped,identity,artifact,image,peers,reconcile,loadReport,verifyCompletion};
