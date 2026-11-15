'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const baseline=require('../phase1/check.cjs');
const {validate:decodePng}=require('./png.cjs');
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
  requireTrue(['observedBefore','observedAfter','releaseBefore','releaseAfter','uncertaintyNanoseconds'].every(key=>typeof row[key]==='string'&&/^\d+$/.test(row[key])),'STOP_CLOCK_INTEGER');
  requireTrue(Math.abs(row.positionSubunits)<=59000&&Math.abs(row.hostPositionSubunits)<=59000,'STOP_POSITION_BOUNDS');
  requireTrue(BigInt(row.releaseAfter)>=BigInt(row.releaseBefore)&&BigInt(row.observedBefore)>=BigInt(row.releaseAfter)&&BigInt(row.observedAfter)>=BigInt(row.observedBefore),'STOP_ORDER');
  requireTrue(BigInt(row.uncertaintyNanoseconds)<=1000000n,'STOP_UNCERTAINTY');
  requireTrue(row.observerDraw?.sha256&&row.hostDraw?.sha256&&row.releaseAcknowledgment?.nativeDrawFrame>0&&row.authorityDrawFrame>0,'STOP_REFERENCES');
  requireTrue(BigInt(row.observedAfter)-BigInt(row.releaseBefore)+BigInt(row.uncertaintyNanoseconds)<=500000000n,'STOP_TIME');
  requireTrue(Math.abs(row.positionSubunits-row.hostPositionSubunits)<=50,'STOP_DISTANCE');return row;
}
function identity(row,context){for(const key of ['nonce','input','tool','platform','revision','osFingerprint'])requireTrue(typeof context[key]==='string'&&row[key]===context[key],'RESULT_IDENTITY '+key);}
function artifact(context,row){
  requireTrue(typeof row.path==='string'&&/^[a-f0-9]{64}$/.test(row.sha256),'ARTIFACT_DECLARATION');
  const file=inside(context.directory,row.path);requireTrue(hash(fs.readFileSync(file))===row.sha256,'ARTIFACT_HASH '+row.path);return file;
}
function image(context,row,peer){
  const file=artifact(context,row);decodePng(file,1280,720);
  const state=read(artifact(context,{path:row.statePath,sha256:row.stateSha256}));
  requireTrue(state.nonce===context.nonce&&state.pid===peer.pid&&state.role===peer.role&&state.profile===peer.profile,'IMAGE_SUBJECT');
  requireTrue(state.drawn===true&&state.drawFrame>0&&state.checkpoint===row.captureId&&state.width===1280&&state.height===720,'IMAGE_DRAW');
  if(row.screen==='menu'||row.screen==='browser'){
    requireTrue(state.snapshot===null,'IMAGE_SCREEN_LIFETIME');return state;
  }
  const subject=row.subject??peer;
  requireTrue(state.snapshot?.SessionId===subject.session&&state.bindingId===subject.binding&&state.playerId===peer.profile,'IMAGE_BINDING');
  requireTrue(state.snapshot.Revision>=subject.minimumRevision,'IMAGE_REVISION');
  const visual=state.actors?.find(actor=>actor.actorId===subject.actor);
  requireTrue(visual&&visual.visible&&visual.insideCamera&&Number.isFinite(visual.screenX)&&Number.isFinite(visual.facing),'IMAGE_ACTOR');
  for(const participant of state.snapshot.Participants.filter(row=>row.Ready)){
    const actor=state.actors.find(row=>row.actorId===participant.Actor.ActorId);
    requireTrue(actor&&actor.playerId===participant.Actor.PlayerId&&actor.bindingId===participant.BindingId,'IMAGE_ROSTER');
    requireTrue(Math.abs(actor.x-participant.Actor.PositionSubunits*16/1000)<0.001&&actor.facing===participant.Actor.Facing,'IMAGE_TRANSFORM');
    requireTrue(Math.abs(actor.screenX-(actor.x+640-state.cameraX))<0.001&&actor.y===540&&Number.isFinite(state.cameraX),'IMAGE_CAMERA');
  }
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
      const terminal=read(artifact(context,peer.terminal));
      requireTrue(terminal.nonce===context.nonce&&terminal.pid===peer.pid&&terminal.role===peer.role,'PEER_TERMINAL_IDENTITY');
      require('./terminal.cjs').validate(terminal,true);
      if(item.group==='rendered')artifact(context,peer.draws);
      if(peer.session){requireTrue(peer.actor&&peer.binding&&Number.isSafeInteger(peer.minimumRevision),'PEER_BINDING');
        requireTrue(!actors.has(peer.actor)&&!bindings.has(peer.binding),'PEER_ACTOR_BINDING_DUPLICATE');actors.add(peer.actor);bindings.add(peer.binding);}
      exact(item.imagesByRole?.[peer.role]??item.images,peer.images?.map(row=>row.checkpoint),'images '+item.id+'/'+peer.role);
      for(const row of peer.images){const expected=item.screensByRole?.[peer.role]??item.imageScreens?.[row.checkpoint]??item.screen??'game';
        requireTrue(Array.isArray(expected)?expected.includes(row.screen??'game'):(row.screen??'game')===expected,'IMAGE_EXPECTED_SCREEN');image(context,row,peer);}
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
    if(item.group==='rendered'){
      requireTrue(result.ownedProcesses?.length>=result.peers.length,'OWNED_TERMINALS_REQUIRED');
      exact(result.ownedProcesses.map(peer=>peer.pid),result.ownedProcesses.map(peer=>peer.pid),'owned process PIDs');
      for(const peer of [...result.peers,...result.additionalPeers??[]])requireTrue(result.ownedProcesses.some(owned=>owned.pid===peer.pid&&owned.terminal.sha256===peer.terminal.sha256),'OWNED_TERMINAL_MEMBERSHIP');
      for(const owned of result.ownedProcesses){const terminal=read(artifact(context,owned.terminal));
        requireTrue(terminal.nonce===context.nonce&&terminal.pid===owned.pid&&terminal.role===owned.role,'OWNED_TERMINAL_IDENTITY');require('./terminal.cjs').validate(terminal,true);}
      requireTrue(result.nativeDrivers?.length>0,'NATIVE_DRIVER_LOG_REQUIRED');
      for(const reference of result.nativeDrivers){const driver=read(artifact(context,reference));
        requireTrue(driver.nonce===context.nonce&&driver.ended===true&&!driver.stderr&&driver.records.length>0&&driver.pid>0,'NATIVE_DRIVER_LOG');
        requireTrue(driver.scriptSha256===hash(fs.readFileSync(path.join(context.root,'tools/phase3/windows-input.ps1'))),'NATIVE_DRIVER_TOOL');}
    }
    if(item.additionalRoles){
      requireTrue(result.additionalPeers?.every(peer=>!result.peers.some(required=>required.pid===peer.pid)),'ADDITIONAL_PID_SUBSTITUTION');
      peers(context,{...item,roles:item.additionalRoles,imagesByRole:item.additionalImagesByRole,screensByRole:item.additionalScreensByRole,profileRestart:true},{...result,peers:result.additionalPeers});
      const raced=result.additionalPeers.filter(peer=>['capacity-challenger','race-challenger'].includes(peer.role)).map(peer=>peer.images.find(image=>image.checkpoint==='race-result'));
      requireTrue(raced.length===2&&raced.filter(image=>image?.screen==='game').length===1,'RACE_IMAGE_MEMBERSHIP');
      const delivery=result.raceDelivery,contenders=result.additionalPeers.filter(peer=>['capacity-challenger','race-challenger'].includes(peer.role));
      requireTrue(delivery?.method==='window-message'&&delivery.physicalInputCredit===false,'RACE_TARGETED_PROVENANCE');
      exact(contenders.map(peer=>peer.pid),delivery.acknowledgments?.map(ack=>ack.pid),'race native acknowledgments');
      const records=result.nativeDrivers.flatMap(reference=>read(artifact(context,reference)).records);
      for(const ack of delivery.acknowledgments){
        const peer=contenders.find(peer=>peer.pid===ack.pid),draws=fs.readFileSync(artifact(context,peer.draws),'utf8').trim().split('\n').map(JSON.parse);
        const preparation=draws.find(draw=>draw.drawFrame===ack.preparedControlDrawFrame),native=draws.find(draw=>draw.drawFrame===ack.nativeDrawFrame);
        requireTrue(ack.method==='window-message'&&ack.action==='key'&&ack.key===13&&ack.pressed===true&&ack.nativeInputEvents>0&&
          ack.preparedControl==='Browser/JoinAddress'&&preparation?.controls.some(control=>control.path===ack.preparedControl&&control.focused)&&
          native?.input.lastInput?.key===4194309&&native.input.lastInput.pressed===true&&native.input.inputEvents===ack.nativeInputEvents&&
          records.filter(record=>record.driverPid===ack.driverPid&&record.sequence===ack.sequence&&JSON.stringify(record)===JSON.stringify(ack)).length===1,'RACE_NATIVE_HANDLER_ACK');
      }
    }
    if(item.id==='rendered.timing'){
      const expected=[];for(const origin of item.roles)for(const observer of item.roles.filter(role=>role!==origin))for(const action of ['start','reversal','stop','focus-stop'])expected.push(origin+'/'+observer+'/'+action);
      exact(expected,result.timings?.map(row=>row.origin+'/'+row.observer+'/'+row.action),'timing matrix');
      for(const row of result.timings){
        requireTrue(row.provenance==='physical-SendInput','PHYSICAL_TIMING_REQUIRED');timing(row);
        const peer=result.peers.find(peer=>peer.role===row.observer),bound=peer.images.find(image=>image.sha256===row.image?.sha256&&image.path===row.image.path);
        requireTrue(bound&&row.image.stateSha256===bound.stateSha256,'TIMING_IMAGE_BINDING');
        const state=image(context,bound,peer),visible=state.actors.find(actor=>actor.actorId===row.actor);
        requireTrue(state.snapshot.SessionId===row.session&&state.snapshot.Revision===row.revision&&state.drawFrame===row.drawFrame&&visible.insideCamera&&visible.visible,'TIMING_IMAGE_SUBJECT');
        const ack=row.inputAcknowledgment;requireTrue(ack?.pid===result.peers.find(peer=>peer.role===row.origin).pid&&ack.method===row.provenance&&ack.deliveryBefore===row.deliveryBefore,'TIMING_DRIVER_SUBJECT');
        const matched=result.nativeDrivers.flatMap(reference=>read(artifact(context,reference)).records).filter(record=>record.driverPid===ack.driverPid&&record.sequence===ack.sequence&&record.pid===ack.pid&&record.deliveryBefore===ack.deliveryBefore&&record.method===ack.method);
        requireTrue(matched.length===1&&(row.action==='focus-stop'?matched[0].nativeFocusLosses>0:matched[0].nativeInputEvents>0),'TIMING_NATIVE_ACK_BINDING');
        const host=result.peers.find(peer=>peer.role==='host'),authority=fs.readFileSync(artifact(context,host.draws),'utf8').trim().split('\n').map(JSON.parse).find(draw=>draw.drawFrame===row.authorityDrawFrame);
        requireTrue(authority?.pid===host.pid&&authority.nonce===context.nonce&&authority.input.lastAcceptedRevision===row.acceptedRevision&&JSON.stringify(authority.input.lastAcceptedIntent)===JSON.stringify(row.acceptedIntent),'TIMING_AUTHORITY_BINDING');
        requireTrue(row.acceptedIntent.ActorId===row.actor&&row.acceptedIntent.SessionId===row.session&&row.acceptedIntent.Direction===(row.action==='start'?1:row.action==='reversal'?-1:0)&&row.minimumRevision>row.acceptedRevision,'TIMING_ACCEPTED_INTENT');
      }
      exact(item.roles.flatMap(origin=>item.roles.map(observer=>origin+'/'+observer)),result.stopped?.map(row=>row.origin+'/'+row.observer),'stopped matrix');
      require('./stopped.cjs').reconcile(context,result);
    }
  }return results;
}
function loadReport(root,reference,cases){
  const file=inside(root,reference.path);requireTrue(hash(fs.readFileSync(file))===reference.sha256,'REPORT_HASH');const report=read(file);
  requireTrue(report.schemaVersion===2&&report.status==='pass'&&Number.isSafeInteger(report.startedAt)&&report.finishedAt>=report.startedAt,'REPORT_SCHEMA_STATUS');
  exact(report.selected,report.results.map(row=>row.id),'report');
  const selected=report.selected.map(id=>{const row=cases.find(item=>item.id===id);requireTrue(row,'REPORT_UNKNOWN');return row;});
  reconcile(selected,report.results,{...report,root,directory:path.dirname(file)});return {...report,path:reference.path,sha256:reference.sha256};
}
function verifyCompletion(root,index,cases,current){
  requireTrue(index.schemaVersion===1&&index.scope==='Windows 1280x720 local; no VM'&&index.selection,'COMPLETION_SCHEMA');
  requireTrue(index.selection.path===require('./catalog.cjs').selectionPath,'COMPLETION_SELECTION_PATH');
  const selectionFile=artifact({directory:root},index.selection),selection=read(selectionFile);
  require('./catalog.cjs').validate(root,{schemaVersion:1,requiredEnvironment:'development-os',cases});
  exact(cases.map(row=>row.id),selection.selected.phase3,'COMPLETION_SELECTION');
  requireTrue(index.finalReport&&Array.isArray(index.history),'COMPLETION_FINAL_REQUIRED');
  const final=loadReport(root,index.finalReport,cases);
  for(const key of ['input','tool','platform','osFingerprint','buildRevision'])requireTrue(final[key]===current[key],'COMPLETION_CURRENT '+key);
  const revisionValidation=require('./revisions.cjs').relation(root,final.revision,current.revision,final.input);
  exact(selection.selected.phase3,final.selected,'final selection');
  requireTrue(final.selectionSha256===index.selection.sha256&&final.prelaunchReview?.sha256,'PRELAUNCH_BINDING');
  const runDirectory=path.dirname(index.finalReport.path).replaceAll('\\','/');
  requireTrue(final.prelaunchReview.path===runDirectory+'/prelaunch-review.json'&&
    final.prelaunchReview.report?.path===runDirectory+'/prelaunch-review.md','PRELAUNCH_REVIEW_PATH');
  const reviewRecord=read(artifact({directory:root},final.prelaunchReview));
  requireTrue(reviewRecord.status==='complete'&&reviewRecord.input===final.input&&reviewRecord.tool===final.tool,'PRELAUNCH_REVIEW');
  requireTrue(final.buildRevision===reviewRecord.revision,'PRELAUNCH_BUILD_REVISION');
  require('./revisions.cjs').relation(root,reviewRecord.revision,current.revision,final.input);
  require('./revisions.cjs').committedArtifact(root,final.revision,{path:require('./catalog.cjs').reviewPath,sha256:final.prelaunchReview.sha256});
  require('./revisions.cjs').committedArtifact(root,final.revision,reviewRecord.report);
  requireTrue(final.prelaunchReview.report.sha256===reviewRecord.report.sha256,'PRELAUNCH_REPORT_BINDING');
  artifact({directory:root},final.prelaunchReview.report);
  for(const reference of index.history){const file=inside(root,reference.path);requireTrue(hash(fs.readFileSync(file))===reference.sha256,'HISTORY_HASH');const prior=read(file);requireTrue(prior.finishedAt<=final.startedAt&&prior.nonce!==final.nonce,'LATER_OR_DUPLICATE_REPORT');}
  const visualReport=fs.readFileSync(artifact({directory:root},index.visualReview),'utf8');
  for(const result of final.results)for(const peer of [...result.peers??[],...result.additionalPeers??[]])for(const picture of peer.images??[]){
    const reviews=(index.reviews??[]).filter(row=>row.id===result.id&&row.role===peer.role&&row.checkpoint===picture.checkpoint&&row.imageSha256===picture.sha256&&row.reportSha256===final.sha256);
    requireTrue(reviews.length===1&&reviews[0].verdict==='pass'&&reviews[0].reviewer?.trim()&&reviews[0].notes?.trim(),'APPEARANCE_REVIEW_REQUIRED');
    requireTrue(visualReport.includes(picture.sha256),'VISUAL_REPORT_IMAGE_BINDING');
  }
  verifyInherited(root,final,selection,index.reviews??[]);
  return {status:'pass',selected:final.selected,report:final.path,...revisionValidation};
}
function verifyInherited(root,final,selection,reviews){
  requireTrue(final.inherited?.status==='pass'&&final.inherited.nativeAllowed===true,'INHERITED_AGGREGATE_FAILED');
  requireTrue(final.inherited.commands?.length&&final.inherited.commands.every(row=>row.executed===true&&row.status==='pass'
    &&row.caseOutcomes?.length&&row.caseOutcomes.every(item=>item.status==='pass')),'INHERITED_COMMAND_FAILED');
  for(const command of final.inherited.commands){
    requireTrue(final.inherited[command.family]?.some(row=>row.path===command.report?.path&&row.sha256===command.report.sha256&&
      row.process.sha256===command.process?.sha256),'INHERITED_COMMAND_REPORT');
    artifact({directory:root},command.record);artifact({directory:root},command.logs.stdout);artifact({directory:root},command.logs.stderr);
  }
  requireTrue(final.inherited?.phase1?.length&&final.inherited?.phase2?.length,'INHERITED_EVIDENCE_REQUIRED');
  for(const family of ['phase1','phase2']){
    const catalog=read(path.join(root,'tools',family,'required-cases.json')),seen=[],nonces=new Set();
    for(const reference of final.inherited[family]){
      const file=artifact({directory:root},reference),summary=read(file),directory=path.dirname(file);
      requireTrue(summary.status==='pass'&&summary.platform.startsWith(final.platform)&&summary.revision===final.revision&&!nonces.has(summary.nonce),'INHERITED_IDENTITY');nonces.add(summary.nonce);
      const process=read(artifact({directory:root},reference.process));
      requireTrue(process.code===0&&!process.signal&&!process.timedOut&&process.startedAt>=final.startedAt&&process.finishedAt<=final.finishedAt,'INHERITED_EXECUTION');
      const manifest=family==='phase1'?catalog:{inputs:['src','tests/phase1','tools/phase2','tools/phase1/toolchain.json','Directory.Build.props','Directory.Build.targets','Directory.Packages.props','.editorconfig','BannedSymbols.txt','BannedSymbols.Pure.txt','docs/code']};
      requireTrue(require('./revisions.cjs').snapshotAt(root,final.revision,manifest).sha256===summary.input,'INHERITED_SOURCE_CHANGED');
      exact(summary.selected,summary.results.map(row=>row.id),'inherited report');seen.push(...summary.selected);
      if(family==='phase2'){
        require('../phase2/evidence.cjs').loadReport(root,reference.path,catalog.cases);
        for(const result of summary.results)for(const picture of result.images??[]){const matches=reviews.filter(row=>row.family==='phase2'&&row.id===result.id&&row.checkpoint===picture.checkpoint&&row.imageSha256===picture.sha256&&row.reportSha256===reference.sha256);requireTrue(matches.length===1&&matches[0].verdict==='pass'&&matches[0].reviewer?.trim()&&matches[0].notes?.trim(),'INHERITED_VISUAL_REVIEW');}
      }else for(const result of summary.results){
        const item=catalog.cases.find(row=>row.id===result.id);requireTrue(item,'INHERITED_UNKNOWN_CASE');
        verifyPhase1Proof(directory,result,item);
      }
    }exact(selection.selected[family],seen,'inherited exact '+family);
  }
}
function verifyPhase1Proof(directory,result,item){
  requireTrue(result.id===item.id&&result.status==='pass'&&result.report?.path===item.id+'/tests.json','INHERITED_CASE_REPORT');
  const report=artifact({directory},result.report),caseDirectory=path.dirname(report);
  requireTrue(result.evidence?.length>0,'INHERITED_PROOF_REQUIRED');
  for(const record of result.evidence)artifact({directory:caseDirectory},record);
  baseline.verifyReport(read(report),[item]);
}
module.exports={hash,read,inside,requireTrue,exact,timing,stopped,identity,artifact,image,peers,reconcile,loadReport,verifyCompletion,verifyInherited,verifyPhase1Proof};
