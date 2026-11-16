'use strict';
const fs=require('node:fs'),e=require('./evidence.cjs');
function reconcile(context,result){
  const histories=new Map(result.peers.map(peer=>[peer.role,new Map(fs.readFileSync(e.artifact(context,peer.draws),'utf8')
    .trim().split('\n').map(JSON.parse).map(draw=>[draw.drawFrame,draw]))]));
  const records=result.nativeDrivers.flatMap(reference=>e.read(e.artifact(context,reference)).records);
  const host=result.peers.find(peer=>peer.role==='host');
  function sample(reference,peer){
    const sampled=e.read(e.artifact(context,reference)),draw=sampled.draw,actual=histories.get(peer.role).get(draw?.drawFrame);
    e.requireTrue(actual&&draw.nonce===context.nonce&&draw.pid===peer.pid&&draw.role===peer.role&&draw.drawn===true&&
      JSON.stringify(draw)===JSON.stringify(actual),'STOP_ACTUAL_DRAW');
    return sampled;
  }
  for(const row of result.stopped){
    e.stopped(row);
    const origin=result.peers.find(peer=>peer.role===row.origin),observer=result.peers.find(peer=>peer.role===row.observer);
    e.requireTrue(origin&&observer&&row.actor===origin.actor&&row.session===origin.session,'STOP_CURRENT_ORIGIN');
    const transition=result.timings.find(timing=>timing.origin===origin.role&&timing.action==='stop');
    e.requireTrue(transition&&row.actor===transition.actor&&row.session===transition.session&&row.minimumRevision===transition.minimumRevision&&
      row.authorityDrawFrame===transition.authorityDrawFrame,'STOP_TRANSITION');
    const ack=row.releaseAcknowledgment,matched=records.filter(record=>record.pid===origin.pid&&record.driverPid===ack.driverPid&&record.sequence===ack.sequence);
    e.requireTrue(matched.length===1&&JSON.stringify(matched[0])===JSON.stringify(ack)&&JSON.stringify(ack)===JSON.stringify(transition.inputAcknowledgment)&&
      ack.action==='key'&&ack.pressed===false&&[65,68].includes(ack.key)&&ack.method==='physical-SendInput'&&
      row.releaseBefore===ack.deliveryBefore&&row.releaseAfter===ack.deliveryAfter,'STOP_RELEASE_ACK');
    const release=histories.get(origin.role).get(ack.nativeDrawFrame),authority=histories.get(host.role).get(row.authorityDrawFrame);
    e.requireTrue(release?.snapshot?.SessionId===origin.session&&release.input.lastInput?.pressed===false&&
      release.input.inputEvents===ack.nativeInputEvents&&release.input.direction===0,'STOP_NATIVE_RELEASE');
    const intent=authority?.input?.lastAcceptedIntent;
    e.requireTrue(authority?.nonce===context.nonce&&authority.pid===host.pid&&intent?.ActorId===origin.actor&&intent.SessionId===origin.session&&
      intent.PlayerId===origin.profile&&intent.BindingId===origin.binding&&intent.Direction===0&&intent.Sequence>0&&
      authority.input.lastAcceptedRevision===transition.acceptedRevision&&row.minimumRevision===authority.input.lastAcceptedRevision+1,'STOP_HOST_ACCEPTED');
    const current=sample(row.observerDraw,observer),reference=sample(row.hostDraw,host);
    e.requireTrue(row.observedBefore===current.observedBefore&&row.observedAfter===current.observedAfter,'STOP_OBSERVATION_CLOCK');
    const baseline=observer===origin?ack.nativeDrawFrame:result.timings.find(timing=>timing.origin===origin.role&&timing.observer===observer.role&&timing.action==='stop')?.baselineDrawFrame;
    e.requireTrue(current.draw.drawFrame>=baseline&&reference.draw.drawFrame>=authority.drawFrame,'STOP_FRESH_FRAMES');
    for(const sampled of [current,reference]){
      e.requireTrue(BigInt(sampled.observedBefore)>=BigInt(ack.nativeAcknowledgedAfter)&&BigInt(sampled.observedAfter)>=BigInt(sampled.observedBefore)&&
        BigInt(sampled.observedAfter)-BigInt(ack.deliveryBefore)+BigInt(row.uncertaintyNanoseconds)<=500000000n,'STOP_SAMPLE_ORDER');
      const draw=sampled.draw,participant=draw.snapshot?.Participants.find(participant=>participant.Ready&&participant.Actor.ActorId===origin.actor);
      const visual=draw.actors?.find(actor=>actor.actorId===origin.actor);
      e.requireTrue(draw.snapshot.SessionId===origin.session&&draw.snapshot.Revision>=row.minimumRevision&&participant?.BindingId===origin.binding&&
        participant.Actor.PlayerId===origin.profile&&visual?.visible&&visual.insideCamera&&Math.abs(visual.x-participant.Actor.PositionSubunits*16/1000)<0.001,'STOP_READY_DRAW_ACTOR');
    }
    const position=draw=>draw.snapshot.Participants.find(participant=>participant.Actor.ActorId===origin.actor).Actor.PositionSubunits;
    e.requireTrue(row.positionSubunits===position(current.draw)&&row.hostPositionSubunits===position(reference.draw)&&row.revision===current.draw.snapshot.Revision,'STOP_DERIVED_POSITION_REVISION');
  }
}
module.exports={reconcile};
