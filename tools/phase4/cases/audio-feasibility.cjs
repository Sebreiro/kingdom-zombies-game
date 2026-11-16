'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,need,inside,bytes,document,write,hash}=require('../content.cjs');
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
const FILE='production/phase4/audio-feasibility.json';
const IDS=['feasibility.action','feasibility.confirmation','feasibility.ambience'];
function pcm(value){
  need(value.length>44&&value.toString('ascii',0,4)==='RIFF'&&value.readUInt32LE(4)+8===value.length&&
    value.toString('ascii',8,12)==='WAVE','WAV_HEADER');
  let offset=12,format,data;
  while(offset+8<=value.length){
    const id=value.toString('ascii',offset,offset+4),size=value.readUInt32LE(offset+4),start=offset+8;
    need(start+size<=value.length,'WAV_TRUNCATED');
    if(id==='fmt '){need(!format&&size>=16,'WAV_FORMAT');format={encoding:value.readUInt16LE(start),channels:value.readUInt16LE(start+2),
      rate:value.readUInt32LE(start+4),byteRate:value.readUInt32LE(start+8),align:value.readUInt16LE(start+12),bits:value.readUInt16LE(start+14)};}
    if(id==='data'){need(!data,'WAV_DATA_DUPLICATE');data=value.subarray(start,start+size);}
    offset=start+size+(size%2);
  }
  need(format&&data&&format.encoding===1&&format.channels===2&&format.rate===44100&&format.bits===16&&
    format.align===4&&format.byteRate===176400&&data.length>0&&data.length%4===0,'WAV_PCM');
  return {...format,frames:data.length/4,pcmBytes:data.length,pcmSha256:hash(data)};
}
function validate(record){
  need(record.recipe.sha256===hash(bytes(ROOT,'tools/phase4/extract-audio.py')),'AUDIO_RECIPE_CHANGED');
  need(record.schemaVersion===1&&record.listening.status==='blocked-needs-capable-human'&&record.listening.listener===null&&
    record.listening.verdicts.length===0&&record.listening.acceptanceBlocked.includes('AC-12'),'LISTENING_CREDIT_INVALID');
  need(record.samples.length===3&&equal(record.samples.map(x=>x.id),IDS),'AUDIO_EXACT_IDS');
  for(const sample of record.samples){
    const wav=Buffer.from(sample.bundle.decodedWavBase64,'base64'),format=pcm(wav),raw=Buffer.from(sample.bundle.originalPayloadBase64,'base64');
    need(wav.length===sample.output.bytes&&hash(wav)===sample.output.sha256&&hash(raw)===sample.source.stream.sha256&&
      raw.length===sample.source.stream.bytes&&hash(Buffer.from(sample.bundle.rawAudioClipBase64,'base64'))===sample.source.rawObjectSha256,'AUDIO_BUNDLE_HASH',sample.id);
    for(const name of ['frames','channels','rate','pcmBytes','pcmSha256'])need(format[name]===sample.output[name],'AUDIO_METADATA',sample.id+' '+name);
    need(sample.preservation.verifiedAfter&&equal(sample.preservation.originalsBefore,sample.preservation.originalsAfter),'ORIGINAL_MUTATION',sample.id);
    need(sample.status.listening==='blocked-needs-capable-human'&&sample.status.productionSelection==='candidate-only','AUDIO_FALSE_ACCEPTANCE',sample.id);
  }
  return record;
}
async function extract(context){
  const results=[];
  for(const id of IDS){
    const destination=inside(context.directory,id);
    const child=await runner.command(context,id,PYTHON,['-B',inside(ROOT,'tools/phase4/extract-audio.py'),'--child',id,destination],{timeout:40000});
    const complete=JSON.parse(child.stdout),sample=document(destination,'sample.json');
    need(complete.complete&&complete.id===id&&complete.frames===sample.output.frames&&complete.sha256===sample.output.sha256,'AUDIO_CHILD_INCOMPLETE',id);
    need(hash(bytes(destination,sample.output.name))===sample.output.sha256,'AUDIO_OUTPUT_HASH',id);
    results.push(sample);
  }
  return results;
}
async function feasibility(context){
  const files=[FILE,'tools/phase4/extract-audio.py','tools/phase4/cases/audio-feasibility.cjs','production/phase4/listening-route.md','tools/phase4/check.cjs'];
  const initial=Object.fromEntries(files.map(file=>[file,hash(bytes(ROOT,file))]));write(context.directory,'inputs.json',initial);
  const record=validate(document(ROOT,FILE)),actual=await extract(context);
  need(equal(record.samples,actual),'AUDIO_REPRODUCTION_MISMATCH');
  const outcomes=[];
  for(const [name,code,edit]of [
    ['truncated-wav','WAV_HEADER',x=>{x.samples[0].bundle.decodedWavBase64=Buffer.from(x.samples[0].bundle.decodedWavBase64,'base64').subarray(0,100).toString('base64');}],
    ['wrong-owner-hash','AUDIO_BUNDLE_HASH',x=>{x.samples[0].source.stream.sha256='0'.repeat(64);}],
    ['wrong-frame-count','AUDIO_METADATA',x=>{x.samples[0].output.frames++;}],
    ['original-mutation','ORIGINAL_MUTATION',x=>{x.samples[0].preservation.originalsAfter[0].sha256='0'.repeat(64);}],
    ['fabricated-listener','LISTENING_CREDIT_INVALID',x=>{x.listening.status='approved';x.listening.listener='invented';}],
    ['batch-missing-child','AUDIO_EXACT_IDS',x=>{x.samples.pop();}]
  ]){
    const subject=structuredClone(record);edit(subject);let failure;
    try{validate(subject);}catch(error){failure=error.message;}
    need(failure?.startsWith(code),'AUDIO_NEGATIVE_CONTROL',name+' '+failure);
    const outcome={name,status:'fail',expectedRejectionObserved:true,diagnostic:failure};write(context.directory,name+'.json',outcome);outcomes.push(outcome);
  }
  need(equal(initial,Object.fromEntries(files.map(file=>[file,hash(bytes(ROOT,file))]))),'SOURCE_CHANGED');
  write(context.directory,'negative-controls.json',outcomes);
  return {samples:actual.map(x=>({id:x.id,...x.output})),isolatedChildren:3,completeChildren:3,originalsUnchanged:true,
    listening:record.listening,expectedRejected:outcomes.length,scope:'format/extraction only; no hearing or production selection'};
}
runner.registerCase('audio.feasibility',{group:'audio',title:'Isolated exact-owner audio decoding and honest listening block',importance:'Critical',
  rationale:'Truncated PCM, decoder cross-contamination or fabricated listening invalidates sound production',needsGameWindow:false,runGroup:'G0',order:6,
  requirements:['ASSET-01','ASSET-04'],acceptance:['AC-12'],
  setup:'Explicit installed Python 3.12.14, UnityPy/Pillow/pyfmodex; read-only preserved exact donor owners/streams',
  actions:['Validate retained candidate source/WAV/PCM bundles','Reproduce one action, confirmation and ambience in separate bounded native decoder children',
    'Rehash owning originals before/after','Reject missing children, truncated PCM, bad source/metadata and fabricated listening'],
  expected:'Three complete independent PCM reproductions; exact hashes; listening visibly blocked pending capable human verdicts',
  reset:'One fresh owned child output per exact selection; preserve all command/output evidence; no playback/window',
  evidence:['inputs.json','feasibility.action.command.json','feasibility.confirmation.command.json','feasibility.ambience.command.json','negative-controls.json','result.json']},feasibility);
module.exports={IDS,pcm,validate,extract,feasibility};
