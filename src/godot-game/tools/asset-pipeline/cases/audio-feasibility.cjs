'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,need,inside,bytes,document,write,hash}=require('../content.cjs');
const {PYTHON}=require('../python.cjs');
const FILE='production/phase4/audio-feasibility.json';
const HISTORICAL_FIXTURE_SHA='d7d5c4e53410b1e5f9aa9c0e5d4119b9e53d08e13b6768586e7d55abcc39cae0';
const HISTORICAL_REPLAY={
  revision:1,
  executableSha256:'41981a311167214a5cbffeb90998ed25568b937e7aa5b6bcadeaa1faa4a45418',
};
const CURRENT_REPLAY={
  revision:2,
  executableSha256:'5699bdfd4b6924c69d0a1f6b1d75d7760698567ce81b92d4fcc8bb2af4eeba84',
  librarySha256:'091cfaffba5da38cea075b310399799bc2025a365f6ed266d94c92e6fc91f3c3',
};
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
function fileIdentity(file){
  const stat=fs.statSync(file);
  need(stat.isFile()&&stat.size>0&&stat.size<=16*1024*1024,'AUDIO_REPLAY_FILE',file);
  const data=fs.readFileSync(file);
  need(data.length===stat.size,'AUDIO_REPLAY_FILE_CHANGED',file);
  return {path:file,bytes:data.length,sha256:hash(data)};
}
function replayRuntime(executable){
  const launcher=fileIdentity(fs.realpathSync.native(executable));
  const pin=[HISTORICAL_REPLAY,CURRENT_REPLAY].find(candidate=>candidate.executableSha256===launcher.sha256);
  need(pin,'AUDIO_REPLAY_EXECUTABLE_PIN',launcher.sha256);
  const library=pin.librarySha256?fileIdentity(path.join(path.dirname(launcher.path),'python312.dll')):null;
  need(!library||library.sha256===pin.librarySha256,'AUDIO_REPLAY_LIBRARY_PIN',library?.sha256);
  return {revision:pin.revision,python:'3.12.14',launcher,library};
}
function replayExpectation(samples,revision){
  if(revision===HISTORICAL_REPLAY.revision)return structuredClone(samples);
  need(revision===CURRENT_REPLAY.revision,'AUDIO_REPLAY_REVISION',revision);
  return samples.map(sample=>({...structuredClone(sample),tools:{...sample.tools,pythonExeSha256:CURRENT_REPLAY.executableSha256}}));
}
function requireExactReplay(expected,actual){
  need(equal(expected,actual),'AUDIO_REPRODUCTION_MISMATCH');
}
function exactHistoricalPayload(original,sample,wav){
  const mediaBytes=pcm(wav);
  need(mediaBytes.pcmBytes===original.output.pcmBytes&&mediaBytes.pcmSha256===original.output.pcmSha256,
    'AUDIO_REPLAY_PCM_MISMATCH',original.id);
  need(wav.equals(Buffer.from(original.bundle.decodedWavBase64,'base64'))&&
    wav.equals(Buffer.from(sample.bundle.decodedWavBase64,'base64')),'AUDIO_REPLAY_WAV_MISMATCH',original.id);
  const sourceFields=['originalPayloadBase64','rawAudioClipBase64'];
  need(equal(sample.source,original.source)&&equal(sample.preservation,original.preservation)&&
    sourceFields.every(key=>Buffer.from(sample.bundle[key],'base64').equals(Buffer.from(original.bundle[key],'base64'))),
    'AUDIO_REPLAY_SOURCE_MISMATCH',original.id);
  return {id:original.id,source:original.source,frames:mediaBytes.frames,pcmSha256:mediaBytes.pcmSha256,wavSha256:hash(wav)};
}
async function extract(context,executable=PYTHON){
  const results=[];
  for(const id of IDS){
    const destination=inside(context.directory,id);
    const child=await runner.command(context,id,executable,['-B',inside(ROOT,'src/godot-game/tools/asset-pipeline/run-extractor.py'),'extract-audio.py','--child',id,destination],{timeout:40000});
    const complete=JSON.parse(child.stdout),sample=document(destination,'sample.json');
    need(complete.complete&&complete.id===id&&complete.frames===sample.output.frames&&complete.sha256===sample.output.sha256,'AUDIO_CHILD_INCOMPLETE',id);
    need(hash(bytes(destination,sample.output.name))===sample.output.sha256,'AUDIO_OUTPUT_HASH',id);
    results.push(sample);
  }
  return results;
}
function replayControls(context,record,runtime,expected,actual){
  const folder=inside(context.directory,'runtime-controls');
  fs.mkdirSync(folder);
  const launcher=inside(folder,'python.exe');
  fs.writeFileSync(launcher,'unknown executable bytes',{flag:'wx'});
  const outcomes=[];
  function rejected(name,code,action){
    let failure;
    try{action();}catch(error){failure=error.message;}
    need(failure?.startsWith(code),'AUDIO_REPLAY_NEGATIVE_CONTROL',name+' '+failure);
    outcomes.push({name,expectedRejectionObserved:true,diagnostic:failure});
  }
  rejected('unknown-executable','AUDIO_REPLAY_EXECUTABLE_PIN',()=>replayRuntime(launcher));
  if(runtime.revision===CURRENT_REPLAY.revision){
    const rebound=inside(folder,'approved-python.exe');
    fs.copyFileSync(runtime.launcher.path,rebound,fs.constants.COPYFILE_EXCL);
    fs.writeFileSync(inside(folder,'python312.dll'),'wrong runtime library',{flag:'wx'});
    rejected('wrong-library-for-approved-executable','AUDIO_REPLAY_LIBRARY_PIN',()=>replayRuntime(rebound));
  }
  for(const [name,pin]of [['changed-interpreter-record','0'.repeat(64)],
    ['forged-approved-interpreter-record',runtime.revision===CURRENT_REPLAY.revision?
      HISTORICAL_REPLAY.executableSha256:CURRENT_REPLAY.executableSha256]]){
    const changed=structuredClone(actual);
    changed[0].tools.pythonExeSha256=pin;
    rejected(name,'AUDIO_REPRODUCTION_MISMATCH',()=>requireExactReplay(expected,changed));
  }
  const original=record.samples[0],sample=actual[0];
  const wav=Buffer.from(sample.bundle.decodedWavBase64,'base64');
  const changedPcm=Buffer.from(wav);
  changedPcm[changedPcm.length-1]^=1;
  rejected('changed-pcm-bytes','AUDIO_REPLAY_PCM_MISMATCH',()=>exactHistoricalPayload(original,sample,changedPcm));
  const changedWav=Buffer.concat([wav,Buffer.from('JUNK'),Buffer.alloc(4)]);
  changedWav.writeUInt32LE(changedWav.length-8,4);
  need(pcm(changedWav).pcmSha256===original.output.pcmSha256,'AUDIO_REPLAY_WAV_CONTROL_PCM');
  rejected('changed-wav-with-original-pcm','AUDIO_REPLAY_WAV_MISMATCH',()=>exactHistoricalPayload(original,sample,changedWav));
  requireExactReplay(record.samples,replayExpectation(record.samples,HISTORICAL_REPLAY.revision));
  requireExactReplay(expected,actual);
  write(context.directory,'runtime-controls.json',{outcomes,historicalExpectationUnchanged:true,currentReplayRestored:true});
  return outcomes.length;
}
async function feasibility(context){
  const files=[FILE,'tools/phase4/extract-audio.py','tools/phase4/cases/audio-feasibility.cjs','production/phase4/listening-route.md','tools/phase4/check.cjs'];
  const initial=Object.fromEntries(files.map(file=>[file,hash(bytes(ROOT,file))]));write(context.directory,'inputs.json',initial);
  need(initial[FILE]===HISTORICAL_FIXTURE_SHA,'AUDIO_HISTORICAL_FIXTURE_PIN');
  const record=validate(document(ROOT,FILE));
  const runtime=replayRuntime(PYTHON);
  const expected=replayExpectation(record.samples,runtime.revision);
  write(context.directory,'runtime-before.json',{runtime,historicalFixtureSha256:HISTORICAL_FIXTURE_SHA,
    decoder:expected[0].tools});
  write(context.directory,'expected-samples.json',expected);
  const actual=await extract(context,runtime.launcher.path);
  const after=replayRuntime(runtime.launcher.path);
  write(context.directory,'runtime-after.json',after);
  need(equal(runtime,after),'AUDIO_REPLAY_RUNTIME_CHANGED');
  requireExactReplay(expected,actual);
  const exactPayloads=actual.map((sample,index)=>{
    const destination=inside(context.directory,sample.id),wav=bytes(destination,sample.output.name);
    return {...exactHistoricalPayload(record.samples[index],sample,wav),
      sampleRecord:fileIdentity(inside(destination,'sample.json'))};
  });
  write(context.directory,'exact-reproduction.json',{runtimeRevision:runtime.revision,completeSampleEquality:true,exactPayloads});
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
  const runtimeNegatives=replayControls(context,record,runtime,expected,actual);
  need(equal(initial,Object.fromEntries(files.map(file=>[file,hash(bytes(ROOT,file))]))),'SOURCE_CHANGED');
  write(context.directory,'negative-controls.json',outcomes);
  return {samples:actual.map(x=>({id:x.id,...x.output})),isolatedChildren:3,completeChildren:3,originalsUnchanged:true,
    listening:record.listening,expectedRejected:outcomes.length,runtimeRevision:runtime.revision,runtimeNegatives,
    exactReproduction:'exact-reproduction.json',scope:'format/extraction only; no hearing or production selection'};
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
