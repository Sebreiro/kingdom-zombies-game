'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {isDeepStrictEqual:equal}=require('node:util');
const runner=require('../check.cjs');
const {ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const {pcm}=require('./audio-feasibility.cjs');
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
const PACK='src/godot-game/content/packs/audio-actions.json';
const BANK='src/godot-game/assets/audio/actions.wav';
const ZIP='production/phase4/audio-actions.zip';
const APPROVAL={id:'creator-au01-15-v1',provenance:'creator-user-chat-attestation',listener:'creator/user',
  statement:'All is approved',observedAtUtc:'2026-11-18T10:25:36Z',personalName:null,outputDevice:null,playbackMethod:null,
  hearingTimestamp:null,detailedObservations:null,assistantHearing:false,finalNativeMixApproved:false,
  scope:'Presented forty full sources and forty-four version1 adaptations for AU-01 through AU-15 source/category uses only',
  presentedManifestSha256:'826ba6acae7cf2a3b07c18903a0f18756e22893cf16748292513051f048daf69',
  presentedPacketSha256:'f37896dadfd5154a2f1c34d47f16f5714fc5bc2b349641159a893eb5eacc73c8',
  bankSha256:'fb173a9863cbc2ba1989cea83afddea3497d22bc760ba28803b9d5e8a545e144',
  sampleMatrixSha256:'940fbad18d29117fc43b1b54ca8560ee195aa3a593abe54d35d481ade8fa5f49',
  categoryMatrixSha256:'bff01047498c5af631f380437e19bab26cb805deba806f09e9e5e32f3e8a7e4a'};
const approvedListening=()=>({status:'Approved by creator attestation',listener:'creator/user',verdict:'accepted',approvalId:APPROVAL.id});
// Independent exact asset-plan anchors. A name is a candidate, never a hearing verdict.
const CANDIDATES=[
  [3294,'citizen_run01'],[4949,'citizen_run02'],[3369,'knight_newattack1'],[4872,'knight_newattack2'],
  [5123,'troll_hits_player'],[2880,'Greed_Archer_Take_Damage_Blunt_02'],[3700,'Greed_Archer_Take_Damage_Arrow_05'],
  [4523,'wall_wood_wreck'],[3574,'wall_iron_wreck01'],[3288,'arrow_shoot'],[5476,'arrow_generic_hit'],
  [3640,'arrow_hit_ground'],[4434,'arrow_hit_wall1'],[5020,'arrow_hit_wall2'],[3357,'arrow_hit_bunny'],
  [4692,'arrow_hits_portal01'],[2790,'lizard_spitexplosion_altv'],[2864,'troll_movement14'],[3150,'troll_charge01'],
  [3131,'Greed_Archer_Die_01'],[2804,'Greed_Knight_Death_02'],[3554,'sfx_greed_crusher_die_01'],
  [5736,'player_loses_crown01'],[3944,'deer_jump_v2'],[4247,'deer_is_killed'],[3811,'sfx_critter_hare_die_01'],
  [5272,'worker_hammer_wood01'],[3945,'worker_hammer_wood02'],[3804,'worker_hammer_iron01'],[5176,'worker_hammer_iron02'],
  [4611,'payment_choptree01'],[3741,'worker_chops_tree01'],[3672,'worker_chops_tree02'],[2862,'payment_build'],
  [4543,'wall_completed'],[4245,'ironbuilding_complete'],[4169,'troll_hitwallwood01'],[4041,'troll_hitwalliron01'],
  [5654,'wall_wood_wreck2'],[5154,'wall_iron_wreck02']
];
const COVERAGE=[
  ['AU-01','Survivor footsteps / walk / sprint / carrying / retreat',[3294,4949],'Footfalls rather than hooves; readable walking and sprint cadence.'],
  ['AU-02','Jump takeoff / landing',[3294,4949],'Can the short footfall trims read as human takeoff and landing without speech?'],
  ['AU-03','Fist / knife / machete / club / bat swings',[3369,4872],'Identify usable swing-only regions and any missing fist/wood/metal identity.'],
  ['AU-04','Flesh / armor impacts',[5123,2880,3700],'Identify flesh versus armor contacts and unwanted fantasy creature identity.'],
  ['AU-05','Equipment condition reaches zero',[4523,3574],'Do the 140ms crack/clink trims read as small equipment breakage, without building collapse?'],
  ['AU-06','Ordinary / tower bow release',[3288],'Does the complete short release read sharply without unrelated handling?'],
  ['AU-07','Arrow terrain / wood / metal / prey / portal contact',[5476,3640,4434,5020,3357,4692],'Classify the actual material of each first-hit contact.'],
  ['AU-08','Spit windup / release / direct hit',[2790],'Is any actual wet region usable without fire/explosion identity? Name exact seconds or reject.'],
  ['AU-09','Zombie movement / attack / hit / death',[2864,3150,3131,2804,3554,2880,3700],'Which portions fit shared undead actions; which retain an inappropriate creature identity?'],
  ['AU-10','Survivor damage / death / control succession',[5123,5736],'Can any loss-cue portion communicate restrained death/succession without music or crown loss?'],
  ['AU-11','Prey flee / hit / death',[3944,4247,3811,3357],'Confirm deer/rabbit identity and suitability of flee/hit/death variants.'],
  ['AU-12','Paid wood / metal building and repair',[5272,3945,3804,5176],'Classify wood/metal contacts; assess randomized work variants.'],
  ['AU-13','Tree mark / chop / fall',[4611,3741,3672,4523,5654],'Identify mark/chop/fall candidates; exclude coin-yield or pickup implication.'],
  ['AU-14','Build / upgrade begin and complete',[2862,4543,4245],'Confirm short noticeable start and single completion cues; identify musical content.'],
  ['AU-15','Wood / metal defense hit / collapse',[4169,4041,4523,5654,3574,5154],'Classify material impacts and collapse tails independently.']
];
const ids=()=>COVERAGE.map(x=>x[0]);
const sourceChannels=id=>[2880,3700,3131,3554,3811].includes(id)?1:2;
function decoderRecipe(){
  let source=bytes(ROOT,'tools/phase4/extract-audio.py').toString('utf8');
  source=source.replace('ROOT = pathlib.Path(__file__).resolve().parents[2]',"ROOT = pathlib.Path('D:/Work/Games/KingdomZombies')");
  const selections=Object.fromEntries(CANDIDATES.map(([id,name])=>['candidate.'+id,['resources.assets',id,name,0]]));
  source=source.replace(/SELECTIONS = \{[\s\S]*?\n\}/,'SELECTIONS = '+JSON.stringify(selections));
  source=source.replace('frames == expected_frames and channels == 2',
    '0 < frames <= 44100 * 120 and abs(frames - round(clip.m_Length * rate)) <= 1 and channels == clip.m_Channels and channels == (1 if object_id in [2880,3700,3131,3554,3811] else 2)');
  need(source.includes('0 < frames <= 44100 * 120'),'AUDIO_RECIPE_TEMPLATE');
  return source;
}
const ASSEMBLE=String.raw`import array, hashlib, io, json, math, pathlib, sys, wave, zipfile
root=pathlib.Path(sys.argv[1]).resolve()
assert root.is_relative_to(pathlib.Path('D:/Work/Games/KingdomZombies/.phase4').resolve())
sha=lambda b:hashlib.sha256(b).hexdigest()
config=json.loads((root/'selection.json').read_text())
samples=[]
members={}
bank=array.array('h')
def wav(pcm):
    out=io.BytesIO()
    with wave.open(out,'wb') as f:
        f.setnchannels(2);f.setsampwidth(2);f.setframerate(44100);f.writeframes(pcm)
    return out.getvalue()
for object_id,name in config['candidates']:
    folder=root/('candidate.'+str(object_id))
    original=(folder/(name+'.wav')).read_bytes()
    record=json.loads((folder/'sample.json').read_text())
    import base64
    prefix='sources/'+str(object_id)
    members[prefix+'/original.wav']=original
    members[prefix+'/stream.fsb']=base64.b64decode(record['bundle']['originalPayloadBase64'])
    members[prefix+'/audioclip.bin']=base64.b64decode(record['bundle']['rawAudioClipBase64'])
    del record['bundle']
    members[prefix+'/provenance.json']=(json.dumps(record,indent=2)+'\n').encode()
    with wave.open(io.BytesIO(original),'rb') as f:
        raw=f.readframes(f.getnframes())
    data=array.array('h');data.frombytes(raw)
    assert sys.byteorder=='little'
    source_channels=record['output']['channels']
    if source_channels==1:data=array.array('h',(v for value in data for v in (value,value)))
    peak=max(abs(v) for v in data)
    assert peak>0
    # Candidate headroom normalization is a technical transform, not audible approval.
    gain=min(1.0,24575/peak)
    variants=[('full',0,len(data)//2)]
    if object_id in [3294,4949]:variants.append(('jump-trim',0,min(5292,len(data)//2)))
    if object_id in [4523,3574]:variants.append(('break-trim',0,min(6174,len(data)//2)))
    for role,start,end in variants:
        adapted=array.array('h',(round(v*gain) for v in data[start*2:end*2]))
        payload=adapted.tobytes();output=wav(payload)
        identity='action.'+str(object_id)+'.'+role
        variant='variants/'+identity+'.wav';members[variant]=output
        first=len(bank)//2;bank.extend(adapted)
        samples.append({'id':identity,'version':1,'candidateId':object_id,'candidateName':name,
          'source':record['source'],'preservation':record['preservation'],'tools':record['tools'],
          'original':record['output'],'sourceMember':prefix+'/original.wav','variantMember':variant,
          'sha256':sha(output),'pcmSha256':sha(payload),'frames':end-start,'channels':2,'rate':44100,'sampleWidth':2,
          'durationSeconds':(end-start)/44100,'trimFrames':[start,end],'gainLinear':gain,'gainDb':20*math.log10(gain),
          'channelTransform':'duplicate-mono-to-stereo' if source_channels==1 else 'preserve-stereo',
          'regionFrames':[first,len(bank)//2],'loop':None,'listening':{'status':'Not auditioned','listener':None,'verdict':None},
          'technical':{'peak':max(abs(v) for v in adapted),'rms':math.sqrt(sum(v*v for v in adapted)/len(adapted)),
             'nonzeroSamples':sum(v!=0 for v in adapted),'sourceFullScaleSamples':sum(abs(v)>=32767 for v in data),
             'fullScaleSamples':sum(abs(v)>=32767 for v in adapted)}})
    bank.extend([0]*4410) # 50ms indexed separator, excluded from every region
bankbytes=wav(bank.tobytes())
pack={'schemaVersion':1,'id':'audio-actions','version':1,'status':'candidate-review-blocked',
 'scope':'AU01-15 source-only candidate review; no completion, suitability, native mix or final acceptance credit',
 'bank':{'path':'assets/audio/actions.wav','sha256':sha(bankbytes),'frames':len(bank)//2,'channels':2,'rate':44100,'sampleWidth':2},
 'categories':[],'samples':samples,'listening':{'status':'Not auditioned','listener':None,'verdicts':[],
 'blockedBy':'Actual capable listener must hear exact candidate and adapted sample versions before Task1 completion'},
 'donorPolicy':{'authority':'specs/0.1/assets-and-resources.md section10',
 'selection':'All forty exact relevant donor anchors exported independently before any replacement synthesis',
 'synthesisPerformed':False,'unsuitabilityClaims':[],
 'remaining':'Auditory suitability/material classification/wet-versus-fire/speech/music/source identity/punch remain unknown'},
 'recipes':{'decoderMember':'recipes/decode.py','decoderSha256':sha((root/'decode.py').read_bytes()),
 'assemblerMember':'recipes/assemble.py','assemblerSha256':sha(pathlib.Path(__file__).read_bytes()),
 'selectionMember':'recipes/selection.json'}}
for identity,title,anchors,question in config['coverage']:
    wanted='jump-trim' if identity=='AU-02' else 'break-trim' if identity=='AU-05' else 'full'
    selected=[s['id'] for s in samples if s['candidateId'] in anchors and s['id'].endswith('.'+wanted)]
    pack['categories'].append({'id':identity,'title':title,'candidateIds':anchors,'sampleIds':selected,
      'status':'Not auditioned','suitability':'Unknown until actual hearing','question':question})
members['recipes/decode.py']=(root/'decode.py').read_bytes()
members['recipes/assemble.py']=pathlib.Path(__file__).read_bytes()
members['recipes/selection.json']=(root/'selection.json').read_bytes()
members['review/README.txt']=('Extract this ZIP to a new folder; open review/index.html locally. Each Play is deliberate. '
 'Listen to the full source and adapted variant. Nothing autoplays. All samples are Not auditioned. '
 'Report name, actual device/player, UTC date, sample ID/version/hash, entire-sample hearing, '
 'music/speech/identity/silence/clipping/onset-tail/punch and accepted/changes-required/blocked verdict. '
 'Use exact seconds for suggested edits. No game window or mix review is performed here.').encode()
import html
rows=[]
for category in pack['categories']:
    rows.append('<h2>'+category['id']+' '+html.escape(category['title'])+'</h2><p>'+html.escape(category['question'])+'</p>')
    for identity in category['sampleIds']:
        sample=next(s for s in samples if s['id']==identity)
        rows.append('<details><summary>'+identity+' v1 | '+str(sample['frames'])+' frames | '+str(round(sample['durationSeconds'],3))+'s | Not auditioned</summary>'
        +'<p>Source: resources.assets AudioClip '+str(sample['candidateId'])+' '+html.escape(sample['candidateName'])+'</p>'
        +'<p>Full source</p><audio controls preload="none" src="../'+sample['sourceMember']+'"></audio>'
        +'<p>Adapted variant: trim '+str(sample['trimFrames'])+'; gain '+str(sample['gainDb'])+'dB; bank '+str(sample['regionFrames'])+'</p>'
        +'<audio controls preload="none" src="../'+sample['variantMember']+'"></audio><p>WAV SHA256 '+sample['sha256']+'</p></details>')
members['review/index.html']=('<!doctype html><html lang="en"><meta charset="utf-8"><title>AU01-15 candidate review</title>'
 '<style>body{max-width:950px;margin:2rem auto;font:16px sans-serif;background:#181b1e;color:#eee}details{padding:12px;border:1px solid #777;margin:8px}audio{width:100%}p{overflow-wrap:anywhere}</style>'
 '<h1>AU01-15 source-only candidate review</h1><p>All candidates: Not auditioned. Start each playback deliberately. '
 'Confirm an audible device at comfortable volume. Assess source and adapted version; no game/mix approval is requested. '
 'Some source clips may include unwanted voice, music or source identity: these are unapproved candidates.</p>'+''.join(rows)+'</html>').encode()
pack['bundleMembers']=[{'path':key,'bytes':len(value),'sha256':sha(value)} for key,value in sorted(members.items())]
members['review/manifest.json']=(json.dumps(pack,indent=2,allow_nan=False)+'\n').encode()
with zipfile.ZipFile(root/'audio-actions.zip','w',compression=zipfile.ZIP_DEFLATED,compresslevel=6) as z:
    for key,value in sorted(members.items()):
        info=zipfile.ZipInfo(key,(2026,11,18,0,0,0));info.compress_type=zipfile.ZIP_DEFLATED;z.writestr(info,value)
pack['bundleSha256']=sha((root/'audio-actions.zip').read_bytes())
(root/'actions.wav').write_bytes(bankbytes)
(root/'audio-actions.json').write_text(json.dumps(pack,indent=2,allow_nan=False)+'\n',encoding='utf-8',newline='\n')
print(json.dumps({'candidates':len(config['candidates']),'variants':len(samples),'frames':len(bank)//2,'zipBytes':(root/'audio-actions.zip').stat().st_size}))
`;
const PROMOTE=String.raw`import hashlib,json,pathlib,sys,zipfile
root=pathlib.Path(sys.argv[1]).resolve()
assert root.is_relative_to(pathlib.Path('D:/Work/Games/KingdomZombies/.phase4').resolve())
sha=lambda b:hashlib.sha256(b).hexdigest()
approval=json.loads((root/'approval.json').read_text())
original=(root/'presented-manifest.json').read_bytes()
assert sha(original)==approval['presentedManifestSha256']
assert sha((root/'presented.zip').read_bytes())==approval['presentedPacketSha256']
pack=json.loads(original)
with zipfile.ZipFile(root/'presented.zip') as z:members={n:z.read(n) for n in z.namelist()}
pack['status']='approved-production-source'
pack['scope']='AU01-15 creator-approved exact production sources; final native mix/04-32/33 acceptance pending'
pack['approval']=approval
pack['listening']={'status':'Approved by creator attestation','listener':'creator/user','verdict':'accepted','approvalId':approval['id']}
for sample in pack['samples']:sample['listening']=pack['listening'].copy()
for category in pack['categories']:
    category['status']='Selected by creator attestation'
    category['suitability']='Approved for the presented source/category use'
pack['donorPolicy']['remaining']='Final native mix/04-32/33 judgment pending; no new audio approved by this attestation'
members['approval/presented-manifest.json']=original
members['approval/creator-attestation.json']=(json.dumps(approval,indent=2)+'\n').encode()
members['approval/coordinator-record.md']=(root/'coordinator-record.md').read_bytes()
members['recipes/promote.py']=pathlib.Path(__file__).read_bytes()
pack['recipes']['promotionMember']='recipes/promote.py'
pack['recipes']['promotionSha256']=sha(members['recipes/promote.py'])
pack['retainedManifestMember']='production/manifest.json'
pack['bundleMembers']=[{'path':k,'bytes':len(v),'sha256':sha(v)} for k,v in sorted(members.items())]
del pack['bundleSha256']
members['production/manifest.json']=(json.dumps(pack,indent=2,allow_nan=False)+'\n').encode()
with zipfile.ZipFile(root/'audio-actions.zip','w',compression=zipfile.ZIP_DEFLATED,compresslevel=6) as z:
    for name,payload in sorted(members.items()):
        info=zipfile.ZipInfo(name,(2026,11,18,0,0,0));info.compress_type=zipfile.ZIP_DEFLATED;z.writestr(info,payload)
pack['bundleSha256']=sha((root/'audio-actions.zip').read_bytes())
(root/'audio-actions.json').write_text(json.dumps(pack,indent=2,allow_nan=False)+'\n',encoding='utf-8',newline='\n')
print(json.dumps({'samples':len(pack['samples']),'members':len(members),'sourceApproval':approval['id']}))
`;
async function promote(){
  const context={directory:inside(ROOT,'.phase4/audio-approved/'+crypto.randomUUID())};fs.mkdirSync(context.directory,{recursive:true});
  for(const [target,source]of [['presented-manifest.json',PACK],['presented.zip',ZIP],
    ['coordinator-record.md','.planning/phases/04-asset-production/04-21-APPROVAL-AND-RESUME.md']])fs.copyFileSync(inside(ROOT,source),inside(context.directory,target));
  write(context.directory,'approval.json',APPROVAL);fs.writeFileSync(inside(context.directory,'promote.py'),PROMOTE);
  await runner.command(context,'promote',PYTHON,['-B',inside(context.directory,'promote.py'),context.directory],{timeout:40000});
  for(const [source,target]of [['audio-actions.json',PACK],['audio-actions.zip',ZIP]])fs.copyFileSync(inside(context.directory,source),inside(ROOT,target));
  console.log(JSON.stringify({directory:context.directory,approval:APPROVAL.id}));
}
function validateApproval(pack){
  need(equal(pack.approval,APPROVAL),'AUDIO_APPROVAL_SCOPE');
  need(pack.status==='approved-production-source'&&equal(pack.listening,approvedListening()),'AUDIO_FALSE_AUDITION');
  need(pack.bank.sha256===APPROVAL.bankSha256&&hash(JSON.stringify(pack.samples.map(({listening,...sample})=>sample)))===APPROVAL.sampleMatrixSha256,
    'AUDIO_UNAPPROVED_SAMPLE');
  need(hash(JSON.stringify(pack.categories.map(({status,suitability,...category})=>category)))===APPROVAL.categoryMatrixSha256,'AUDIO_UNAPPROVED_CATEGORY');
}
function inputs(){return Object.fromEntries(['tools/phase4/cases/audio.cjs','tools/phase4/extract-audio.py',PACK,BANK,ZIP].map(x=>[x,hash(bytes(ROOT,x))]));}
async function prepare(){
  const context={directory:inside(ROOT,'.phase4/audio-candidates/'+crypto.randomUUID())};
  fs.mkdirSync(context.directory,{recursive:true});
  fs.writeFileSync(inside(context.directory,'decode.py'),decoderRecipe());
  fs.writeFileSync(inside(context.directory,'assemble.py'),ASSEMBLE);
  write(context.directory,'selection.json',{candidates:CANDIDATES,coverage:COVERAGE});
  for(const [id]of CANDIDATES){
    await runner.command(context,'decode-'+id,PYTHON,['-B',inside(context.directory,'decode.py'),'--child','candidate.'+id,inside(context.directory,'candidate.'+id)],{timeout:40000});
  }
  await runner.command(context,'assemble',PYTHON,['-B',inside(context.directory,'assemble.py'),context.directory],{timeout:40000});
  for(const [source,target]of [['actions.wav',BANK],['audio-actions.json',PACK],['audio-actions.zip',ZIP]]){
    const output=inside(ROOT,target);need(!fs.existsSync(output),'AUDIO_OUTPUT_EXISTS',target);
    fs.mkdirSync(path.dirname(output),{recursive:true});fs.copyFileSync(inside(context.directory,source),output);
  }
  console.log(JSON.stringify({directory:context.directory,files:[BANK,PACK,ZIP]}));
}
function validate(pack,bank){
  const format=pcm(bank);
  need(pack.schemaVersion===1&&pack.id==='audio-actions'&&pack.version===1,'AUDIO_PACK_ID');
  need(equal(pack.categories.map(x=>x.id),ids()),'AUDIO_CATEGORY_IDS');
  need(hash(bank)===pack.bank.sha256,'AUDIO_BANK_HASH');
  need(format.frames===pack.bank.frames&&format.rate===pack.bank.rate&&format.channels===pack.bank.channels,'AUDIO_BANK_FORMAT');
  need(pack.samples.length===44&&new Set(pack.samples.map(x=>x.id)).size===44,'AUDIO_SAMPLE_IDS');
  need(pack.donorPolicy.synthesisPerformed===false&&pack.donorPolicy.unsuitabilityClaims.length===0,'AUDIO_FALSE_DONOR_REJECTION');
  need(equal(pack.listening,approvedListening()),'AUDIO_FALSE_AUDITION');
  const audio=bank.subarray(44),intervals=[];
  for(const sample of pack.samples){
    const anchor=CANDIDATES.find(x=>x[0]===sample.candidateId);
    need(anchor&&sample.candidateName===anchor[1]&&sample.source.owner==='resources.assets'&&sample.source.type==='AudioClip'&&
      sample.source.objectId===anchor[0]&&sample.source.name===anchor[1],'AUDIO_SOURCE_IDENTITY',sample.id);
    need(sample.version===1&&sample.channels===2&&sample.rate===44100&&sample.sampleWidth===2,'AUDIO_SAMPLE_FORMAT',sample.id);
    need(sample.original.channels===sourceChannels(sample.candidateId)&&sample.original.rate===44100&&sample.original.sampleWidth===2&&
      sample.channelTransform===(sourceChannels(sample.candidateId)===1?'duplicate-mono-to-stereo':'preserve-stereo'),'AUDIO_SOURCE_FORMAT',sample.id);
    need(equal(sample.listening,approvedListening()),'AUDIO_FALSE_AUDITION',sample.id);
    need(equal(sample.preservation.originalsBefore,sample.preservation.originalsAfter)&&sample.preservation.verifiedAfter,'AUDIO_ORIGINAL_MUTATION',sample.id);
    const [start,end]=sample.regionFrames,[trimStart,trimEnd]=sample.trimFrames;
    need([start,end,trimStart,trimEnd,sample.frames].every(Number.isSafeInteger)&&start>=0&&end<=format.frames&&end>start&&
      end-start===sample.frames&&trimStart>=0&&trimEnd<=sample.original.frames&&trimEnd-trimStart===sample.frames,'AUDIO_REGION',sample.id);
    need(sample.durationSeconds===sample.frames/sample.rate&&sample.loop===null&&Number.isFinite(sample.gainDb)&&
      sample.gainLinear>0&&sample.gainLinear<=1&&Math.abs(20*Math.log10(sample.gainLinear)-sample.gainDb)<1e-12,'AUDIO_SAMPLE_METADATA',sample.id);
    const region=audio.subarray(start*4,end*4);need(hash(region)===sample.pcmSha256,'AUDIO_REGION_HASH',sample.id);
    let peak=0,nonzero=0,squares=0,clipped=0;
    for(let offset=0;offset<region.length;offset+=2){const value=region.readInt16LE(offset);peak=Math.max(peak,Math.abs(value));nonzero+=value!==0;squares+=value*value;clipped+=Math.abs(value)>=32767;}
    need(nonzero>0&&peak>0&&peak<=24575&&clipped===0,'AUDIO_PCM_DOMAIN',sample.id);
    need(sample.technical.peak===peak&&sample.technical.nonzeroSamples===nonzero&&sample.technical.fullScaleSamples===clipped&&
      Math.abs(sample.technical.rms-Math.sqrt(squares/(region.length/2)))<1e-8,'AUDIO_TECHNICAL_METADATA',sample.id);
    intervals.push([start,end]);
  }
  intervals.sort((a,b)=>a[0]-b[0]);need(intervals.every((x,i)=>i===0||x[0]>=intervals[i-1][1]),'AUDIO_REGION_OVERLAP');
  for(let i=0;i<COVERAGE.length;i++){
    const actual=pack.categories[i],expected=COVERAGE[i],role=actual.id==='AU-02'?'jump-trim':actual.id==='AU-05'?'break-trim':'full';
    need(equal(actual.candidateIds,expected[2])&&actual.title===expected[1]&&actual.question===expected[3],'AUDIO_CATEGORY_CONTRACT',actual.id);
    need(equal(actual.sampleIds,pack.samples.filter(x=>expected[2].includes(x.candidateId)&&x.id.endsWith('.'+role)).map(x=>x.id)),
      'AUDIO_CATEGORY_BINDING',actual.id);
    need(actual.status==='Selected by creator attestation'&&actual.suitability==='Approved for the presented source/category use','AUDIO_FALSE_AUDITION',actual.id);
  }
  need(new Set(pack.samples.filter(x=>x.id.endsWith('.full')).map(x=>x.pcmSha256)).size===40,'AUDIO_FALSE_REUSE');
  validateApproval(pack);
  return {categories:15,candidates:40,variants:44,frames:format.frames,seconds:format.frames/44100,clippedOutputSamples:0,listening:pack.listening,approval:pack.approval};
}
const ZIP_CHECK=String.raw`import array, hashlib, io, json, pathlib, sys, wave, zipfile
root=pathlib.Path(sys.argv[1]); pack=json.loads((root/'src/godot-game/content/packs/audio-actions.json').read_text())
sha=lambda b:hashlib.sha256(b).hexdigest()
with zipfile.ZipFile(root/'production/phase4/audio-actions.zip') as z:
    expected={x['path']:x for x in pack['bundleMembers']}
    assert len(z.infolist())==len(expected)+1 and len(set(z.namelist()))==len(z.namelist()),'AUDIO_ZIP_COUNT'
    assert set(z.namelist())==set(expected)|{'production/manifest.json'},'AUDIO_ZIP_MEMBERS'
    for item in z.infolist():
        p=pathlib.PurePosixPath(item.filename)
        assert not p.is_absolute() and '..' not in p.parts and '\\' not in item.filename and ':' not in item.filename,'AUDIO_ZIP_PATH'
        assert 0<item.file_size<=32*1024*1024,'AUDIO_ZIP_BOUND'
    assert sum(x.file_size for x in z.infolist())<=128*1024*1024,'AUDIO_ZIP_TOTAL'
    for name,record in expected.items():
        value=z.read(name);assert len(value)==record['bytes'] and sha(value)==record['sha256'],'AUDIO_ZIP_HASH'
    retained=json.loads(z.read('production/manifest.json'))
    assert retained=={k:v for k,v in pack.items() if k!='bundleSha256'},'AUDIO_REBOUND_MANIFEST'
    assert sha(z.read('recipes/decode.py'))==pack['recipes']['decoderSha256'],'AUDIO_RECIPE_HASH'
    assert sha(z.read('recipes/assemble.py'))==pack['recipes']['assemblerSha256'],'AUDIO_RECIPE_HASH'
    assert sha(z.read('recipes/promote.py'))==pack['recipes']['promotionSha256'],'AUDIO_RECIPE_HASH'
    assert sha(z.read('approval/presented-manifest.json'))==pack['approval']['presentedManifestSha256'],'AUDIO_APPROVED_BASELINE'
    assert json.loads(z.read('approval/creator-attestation.json'))==pack['approval'],'AUDIO_APPROVAL_REBOUND'
    baseline=json.loads(z.read('approval/presented-manifest.json'))
    assert len(baseline['samples'])==len(pack['samples'])==44,'AUDIO_APPROVED_COUNT'
    for old,new in zip(baseline['samples'],pack['samples']):
        assert {k:v for k,v in old.items() if k!='listening'}=={k:v for k,v in new.items() if k!='listening'},'AUDIO_UNAPPROVED_SAMPLE'
    for sample in pack['samples']:
        original=z.read(sample['sourceMember']);adapted=z.read(sample['variantMember'])
        assert sha(original)==sample['original']['sha256'] and sha(adapted)==sample['sha256'],'AUDIO_FULL_WAV_HASH'
        prefix='sources/'+str(sample['candidateId'])
        assert sha(z.read(prefix+'/stream.fsb'))==sample['source']['stream']['sha256'],'AUDIO_RAW_SOURCE_HASH'
        assert sha(z.read(prefix+'/audioclip.bin'))==sample['source']['rawObjectSha256'],'AUDIO_RAW_OBJECT_HASH'
        with wave.open(io.BytesIO(original),'rb') as f:
            assert f.getnframes()==sample['original']['frames'] and f.getnchannels()==sample['original']['channels'] and f.getframerate()==44100 and f.getsampwidth()==2,'AUDIO_SOURCE_FORMAT'
            data=f.readframes(f.getnframes()+1)
            assert len(data)==f.getnframes()*f.getnchannels()*2 and sha(data)==sample['original']['pcmSha256'],'AUDIO_SOURCE_TRUNCATED'
        original_pcm=array.array('h');original_pcm.frombytes(data)
        if sample['original']['channels']==1:original_pcm=array.array('h',(v for value in original_pcm for v in (value,value)))
        start,end=sample['trimFrames'];expected_pcm=array.array('h',(round(v*sample['gainLinear']) for v in original_pcm[start*2:end*2]))
        with wave.open(io.BytesIO(adapted),'rb') as f:
            actual=f.readframes(f.getnframes()+1)
            assert f.getnframes()==sample['frames'] and len(actual)==sample['frames']*4,'AUDIO_VARIANT_TRUNCATED'
            assert actual==expected_pcm.tobytes() and sha(actual)==sample['pcmSha256'],'AUDIO_RECIPE_REPRODUCTION'
print(json.dumps({'members':len(expected)+1,'fullSources':40,'variants':44,'reproduced':True}))
`;
async function technical(context){
  const before=inputs();write(context.directory,'inputs.json',before);
  const pack=document(ROOT,PACK),bank=bytes(ROOT,BANK);const result=validate(pack,bank);
  need(hash(bytes(ROOT,ZIP))===pack.bundleSha256,'AUDIO_ZIP_IDENTITY');
  need(hash(decoderRecipe())===pack.recipes.decoderSha256&&hash(ASSEMBLE)===pack.recipes.assemblerSha256&&hash(PROMOTE)===pack.recipes.promotionSha256,'AUDIO_RECIPE_IDENTITY');
  fs.writeFileSync(inside(context.directory,'verify-zip.py'),ZIP_CHECK);
  const bundle=await runner.command(context,'retained-bundle',PYTHON,['-B',inside(context.directory,'verify-zip.py'),ROOT],{timeout:40000});
  const controls=[];
  for(const [name,code,edit]of [
    ['wrong-category','AUDIO_CATEGORY_IDS',x=>{x.categories[0].id='AU-30';}],
    ['known-valid-wrong-source','AUDIO_SOURCE_IDENTITY',x=>{x.samples[0].source.objectId=3288;}],
    ['wrong-bank-hash','AUDIO_BANK_HASH',x=>{x.bank.sha256='0'.repeat(64);}],
    ['region-rebound','AUDIO_REGION_HASH',x=>{x.samples[0].pcmSha256=x.samples[1].pcmSha256;}],
    ['fractional-frame','AUDIO_REGION',x=>{x.samples[0].regionFrames[0]=0.5;}],
    ['wrong-rate','AUDIO_SAMPLE_FORMAT',x=>{x.samples[0].rate=48000;}],
    ['wrong-channel','AUDIO_SAMPLE_FORMAT',x=>{x.samples[0].channels=1;}],
    ['false-listener','AUDIO_FALSE_AUDITION',x=>{x.listening.listener='invented';}],
    ['unsupported-unsuitability','AUDIO_FALSE_DONOR_REJECTION',x=>{x.donorPolicy.unsuitabilityClaims.push('unsupported');}],
    ['wrong-category-binding','AUDIO_CATEGORY_BINDING',x=>{x.categories[0].sampleIds=['action.3288.full'];}],
    ['original-mutation','AUDIO_ORIGINAL_MUTATION',x=>{x.samples[0].preservation.originalsAfter[0].sha256='0'.repeat(64);}],
    ['false-peak','AUDIO_TECHNICAL_METADATA',x=>{x.samples[0].technical.peak++;}],
    ['unsupported-approval','AUDIO_APPROVAL_SCOPE',x=>{x.approval.statement='approved';}],
    ['invented-personal-name','AUDIO_APPROVAL_SCOPE',x=>{x.approval.personalName='invented';}],
    ['invented-device','AUDIO_APPROVAL_SCOPE',x=>{x.approval.outputDevice='invented';}],
    ['unapproved-wav-hash','AUDIO_UNAPPROVED_SAMPLE',x=>{x.samples[0].sha256='0'.repeat(64);}],
    ['unapproved-source-hash','AUDIO_UNAPPROVED_SAMPLE',x=>{x.samples[0].source.stream.sha256='0'.repeat(64);}],
    ['unapproved-version','AUDIO_SAMPLE_FORMAT',x=>{x.samples[0].version=2;}]
  ]){
    const subject=structuredClone(pack);edit(subject);let failure;
    try{validate(subject,bank);}catch(error){failure=error.message;}
    need(failure?.split(' ')[0]===code,'AUDIO_NEGATIVE_CONTROL',name+' '+failure);controls.push({name,expectedDiagnostic:code,actual:failure});
    validate(pack,bank);
  }
  let truncated;try{validate(pack,bank.subarray(0,100));}catch(error){truncated=error.message;}
  need(truncated?.split(' ')[0]==='WAV_HEADER','AUDIO_TRUNCATION_CONTROL');controls.push({name:'truncated-bank',actual:truncated});
  need(equal(before,inputs()),'AUDIO_INPUT_CHANGED');write(context.directory,'negative-controls.json',controls);
  const complete={...result,bundle:JSON.parse(bundle.stdout),negativeControls:controls.length,restoredPositives:18,
    inputHashes:before,scope:'Exact creator-attested source/category production approval; no assistant hearing or final native mix acceptance.'};
  write(context.directory,'technical.json',complete);return complete;
}
async function actions(context){
  return technical(context);
}
runner.registerCase('audio.actions',{group:'audio',title:'AU01-15 exact donor source/PCM/region integrity and actual listening gate',importance:'Critical',
  rationale:'False audio identity, truncated samples or invented hearing invalidates required action content',needsGameWindow:false,runGroup:'G0',order:121,
  requirements:['ASSET-01','ASSET-02','ASSET-04'],acceptance:['AC-03','AC-12','AC-14','AC-19'],
  setup:'Tracked exact candidate bank, manifest and ZIP; existing pinned Python; actual capable source-only listener required for completion',
  actions:['Verify exact AU category contracts and forty distinct donor recordings','Verify integer regions, source/variant hashes and reproduce every trim/gain',
    'Check bounded retained ZIP and thirteen exact deliberate failures plus restored positives','Require actual listener evidence; Not auditioned blocks completion'],
  expected:'Exact immutable source/sample matrix and literal creator attestation validate; invented or extended approvals reject',
  reset:'Fresh owned evidence directory only; no game window, playback, donor writes or changes to production bytes',
  evidence:['inputs.json','retained-bundle.command.json','negative-controls.json','technical.json','result.json']},actions);
const INTERFACE_PACK='src/godot-game/content/packs/audio-interface-ambience.json';
const INTERFACE_BANK='src/godot-game/assets/audio/interface-ambience.wav';
const INTERFACE_ZIP='production/phase4/audio-interface-ambience.zip';
const EVENTS='src/godot-game/content/audio/events.json';
function archiveLimit(relative){return relative===INTERFACE_ZIP?64*1024*1024:32*1024*1024;}
function archiveFile(file,destinationIdentity,expectedHash){
  need(typeof expectedHash==='string'&&/^[a-f0-9]{64}$/.test(expectedHash),'AUDIO_ARCHIVE_EXPECTED_HASH');
  const stat=fs.lstatSync(file);
  need(!stat.isSymbolicLink()&&stat.isFile()&&stat.nlink===1&&stat.size>0&&stat.size<=archiveLimit(destinationIdentity),'AUDIO_ARCHIVE_SIZE',destinationIdentity);
  const value=fs.readFileSync(file);need(value.length===stat.size,'AUDIO_ARCHIVE_CHANGED',destinationIdentity);
  need(hash(value)===expectedHash,'AUDIO_ARCHIVE_HASH',destinationIdentity);return value;
}
function archiveBytes(root,relative,expectedHash){return archiveFile(inside(root,relative),relative,expectedHash);}
function publishArchive(sourceRoot,sourceRelative,destinationRoot,destinationRelative,expectedHash){
  const source=inside(sourceRoot,sourceRelative),destination=inside(destinationRoot,destinationRelative);
  const value=archiveFile(source,destinationRelative,expectedHash);
  if(destinationRelative===INTERFACE_ZIP&&fs.existsSync(destination)){
    const existing=archiveBytes(destinationRoot,destinationRelative,expectedHash);
    need(existing.length===value.length&&existing.equals(value),'AUDIO_PUBLICATION_CONFLICT',destinationRelative);
    return {bytes:value.length,sha256:expectedHash,destination:destinationRelative,cap:archiveLimit(destinationRelative),preservedExisting:true};
  }
  need(!fs.existsSync(destination),'AUDIO_PUBLICATION_EXISTS',destinationRelative);fs.mkdirSync(path.dirname(destination),{recursive:true});
  fs.writeFileSync(destination,value,{flag:'wx'});
  need(archiveBytes(destinationRoot,destinationRelative,expectedHash).equals(value),'AUDIO_PUBLICATION_CHANGED');
  return {bytes:value.length,sha256:expectedHash,destination:destinationRelative,cap:archiveLimit(destinationRelative)};
}
const serializeEvents=value=>Buffer.from(JSON.stringify(value,null,2)+'\n','utf8');
function preflightEvents(root,expected){
  const target=inside(root,EVENTS),exists=fs.existsSync(target);
  if(exists)need(bytes(root,EVENTS).equals(expected),'AUDIO_EVENTS_CONFLICT',EVENTS);
  return {existed:exists,bytes:expected.length,sha256:hash(expected)};
}
function finishEvents(root,expected,state){
  need(state.bytes===expected.length&&state.sha256===hash(expected),'AUDIO_EVENTS_EXPECTATION_CHANGED');
  if(state.existed)need(fs.existsSync(inside(root,EVENTS))&&bytes(root,EVENTS).equals(expected),'AUDIO_EVENTS_CHANGED',EVENTS);
  else{
    const target=inside(root,EVENTS);need(!fs.existsSync(target),'AUDIO_EVENTS_APPEARED',EVENTS);
    fs.mkdirSync(path.dirname(target),{recursive:true});fs.writeFileSync(target,expected,{flag:'wx'});
  }
  need(bytes(root,EVENTS).equals(expected),'AUDIO_EVENTS_FINAL_IDENTITY',EVENTS);
  return {...state,preservedExisting:state.existed,verifiedFinal:true};
}
async function publishInterface(directory){
  const staged=path.resolve(directory);need(staged.startsWith(inside(ROOT,'.phase4')+path.sep),'INTERFACE_STAGING_ROOT');
  const context={directory:inside(ROOT,'.phase4/audio-publication/'+crypto.randomUUID())};fs.mkdirSync(context.directory,{recursive:true});
  const p=document(staged,'audio-interface-ambience.json'),e=eventCatalog(p),bank=bytes(staged,'interface-ambience.wav');
  validateInterface(p,bank,e);const expectedEvents=serializeEvents(e),eventState=preflightEvents(ROOT,expectedEvents);
  const known={
    [INTERFACE_BANK]:'e61d445d30ab28d65d163ea89fcc185484b4746f35703cb2ed088f13308ba5b9',
    [INTERFACE_PACK]:'2e107c4ef6eb57262a8f571a1226b694f552a15b806117bcb67e6be7ddfda069'};
  for(const [destination,source]of [[INTERFACE_BANK,'interface-ambience.wav'],[INTERFACE_PACK,'audio-interface-ambience.json']]){
    need(hash(bytes(ROOT,destination))===known[destination]&&hash(bytes(staged,source))===known[destination],'INTERFACE_PARTIAL_IDENTITY',destination);
  }
  const result=publishArchive(staged,'audio-interface-ambience.zip',ROOT,INTERFACE_ZIP,p.bundleSha256);
  const finalEvents=finishEvents(ROOT,expectedEvents,eventState);
  write(context.directory,'authoring-complete.json',{completed:true,archive:result,events:finalEvents,knownOwnedPartial:known,bankUnchanged:true,manifestUnchanged:true,
    scope:'Exact local producer archive only; content/provenance consumer compatibility remains04-23/31'});
  console.log(JSON.stringify({directory:context.directory,completed:true,...result,events:finalEvents}));
}
function archiveControls(context){
  const root=inside(context.directory,'archive-reader-controls');fs.mkdirSync(root);const outcomes=[];
  const zero32='83ee47245398adee79bd9c0a8bc57b821e92aba10f5f9ade8a5d1fae4d8c4302',zero64='3b6a07d0d404fab4e23b6d34bc6696a6a312dd92821332385e5af7c01c421351';
  const create=(relative,size)=>{const file=inside(root,relative);fs.mkdirSync(path.dirname(file),{recursive:true});const fd=fs.openSync(file,'wx');try{fs.ftruncateSync(fd,size);}finally{fs.closeSync(fd);}return file;};
  const resize=(file,size)=>{const fd=fs.openSync(file,'r+');try{fs.ftruncateSync(fd,size);}finally{fs.closeSync(fd);}};
  const reject=(name,code,run)=>{let diagnostic;try{run();}catch(error){diagnostic=error.message;}need(diagnostic?.split(' ')[0]===code,'ARCHIVE_CONTROL',name+' '+diagnostic);outcomes.push({name,expected:code,diagnostic});};
  const canonical=create(INTERFACE_ZIP,64*1024*1024);
  need(archiveBytes(root,INTERFACE_ZIP,zero64).length===64*1024*1024,'ARCHIVE_64_BOUNDARY');outcomes.push({name:'canonical64MiB',status:'accepted'});
  resize(canonical,64*1024*1024+1);reject('canonical64MiBplus1','AUDIO_ARCHIVE_SIZE',()=>archiveBytes(root,INTERFACE_ZIP,zero64));
  resize(canonical,64*1024*1024);need(archiveBytes(root,INTERFACE_ZIP,zero64).length===64*1024*1024,'ARCHIVE_64_RESTORED');
  const ordinary='production/phase4/audio-actions.zip',other=create(ordinary,32*1024*1024);
  need(archiveBytes(root,ordinary,zero32).length===32*1024*1024,'ARCHIVE_32_BOUNDARY');outcomes.push({name:'ordinary32MiB',status:'accepted'});
  resize(other,32*1024*1024+1);reject('ordinary32MiBplus1','AUDIO_ARCHIVE_SIZE',()=>archiveBytes(root,ordinary,zero32));
  resize(other,32*1024*1024);need(archiveBytes(root,ordinary,zero32).length===32*1024*1024,'ARCHIVE_32_RESTORED');
  reject('wrongcase','PATH_CASE',()=>archiveBytes(root,'production/phase4/Audio-interface-ambience.zip',zero64));
  for(const relative of ['production/phase4/audio-interface-ambience-copy.zip','production/other/audio-interface-ambience.zip']){
    create(relative,32*1024*1024+1);reject(relative,'AUDIO_ARCHIVE_SIZE',()=>archiveBytes(root,relative,zero32));
  }
  reject('wrong-expected-hash','AUDIO_ARCHIVE_HASH',()=>archiveBytes(root,INTERFACE_ZIP,'0'.repeat(64)));
  need(archiveBytes(root,INTERFACE_ZIP,zero64).length===64*1024*1024,'ARCHIVE_HASH_RESTORED');
  const stage='staging/arbitrary-name.zip';create(stage,64*1024*1024);
  reject('staging-basename-no-read-exception','AUDIO_ARCHIVE_SIZE',()=>archiveBytes(root,stage,zero64));
  const output=inside(context.directory,'archive-publication-controls');fs.mkdirSync(output);
  const published=publishArchive(root,stage,output,INTERFACE_ZIP,zero64);need(published.bytes===64*1024*1024,'ARCHIVE_CANONICAL_PUBLICATION');
  reject('publication-lookalike','AUDIO_ARCHIVE_SIZE',()=>publishArchive(root,stage,output,'production/phase4/audio-interface-ambience-copy.zip',zero64));
  reject('publication-otherpath','AUDIO_ARCHIVE_SIZE',()=>publishArchive(root,stage,output,'production/phase4/audio-actions.zip',zero64));
  write(context.directory,'archive-reader-controls.json',{outcomes,restoredActualBoundaries:true,capScope:INTERFACE_ZIP,
    independentExpectedZeroHashes:'Python hashlib over32/64 one-MiB zero chunks; these fixtures establish reader limits, not ZIP validity'});
  return outcomes.length;
}
function publicationControls(context){
  const fixture=Buffer.from('audio-publication-r2-fixture\n'),expectedHash=hash(fixture);
  const expectedEvents=Buffer.from('{\n  "id": "r2-events",\n  "version": 1\n}\n');
  need(serializeEvents({id:'r2-events',version:1}).equals(expectedEvents),'R2_EXPECTED_SERIALIZATION');
  const outcomes=[];
  const create=(name,events,archive)=>{
    const root=inside(context.directory,'r2-'+name);fs.mkdirSync(root);
    const put=(relative,value)=>{const f=inside(root,relative);fs.mkdirSync(path.dirname(f),{recursive:true});fs.writeFileSync(f,value,{flag:'wx'});};
    put('stage/archive.zip',fixture);if(events!==undefined)put(EVENTS,events);if(archive!==undefined)put(INTERFACE_ZIP,archive);return {root,put};
  };
  const reject=(name,code,run)=>{let diagnostic;try{run();}catch(error){diagnostic=error.message;}
    need(diagnostic?.split(' ')[0]===code,'R2_PUBLICATION_CONTROL',name+' '+diagnostic);outcomes.push({name,diagnostic,expected:code});};
  for(const [name,present]of [['exact-existing',true],['missing-events',false],['restored-positive',true]]){
    const {root}=create(name,present?expectedEvents:undefined,present?fixture:undefined),state=preflightEvents(root,expectedEvents);
    const archive=publishArchive(root,'stage/archive.zip',root,INTERFACE_ZIP,expectedHash),events=finishEvents(root,expectedEvents,state);
    need(events.verifiedFinal&&events.preservedExisting===present&&archiveBytes(root,INTERFACE_ZIP,expectedHash).equals(fixture)&&bytes(root,EVENTS).equals(expectedEvents),'R2_POSITIVE',name);
    outcomes.push({name,status:'pass',archivePreserved:archive.preservedExisting===true,eventsPreserved:events.preservedExisting});
  }
  {
    const wrong=Buffer.from('conflicting-events\n'),{root}=create('conflicting-events',wrong);
    reject('conflicting-events-before-publication','AUDIO_EVENTS_CONFLICT',()=>{
      const state=preflightEvents(root,expectedEvents);publishArchive(root,'stage/archive.zip',root,INTERFACE_ZIP,expectedHash);finishEvents(root,expectedEvents,state);
    });
    need(!fs.existsSync(inside(root,INTERFACE_ZIP))&&bytes(root,EVENTS).equals(wrong),'R2_EVENTS_CONFLICT_MUTATED');
  }
  {
    const wrong=Buffer.from('conflicting-archive\n'),{root}=create('conflicting-archive',expectedEvents,wrong),state=preflightEvents(root,expectedEvents);
    reject('conflicting-archive-preserves-catalog','AUDIO_ARCHIVE_HASH',()=>{publishArchive(root,'stage/archive.zip',root,INTERFACE_ZIP,expectedHash);finishEvents(root,expectedEvents,state);});
    need(bytes(root,EVENTS).equals(expectedEvents)&&fs.readFileSync(inside(root,INTERFACE_ZIP)).equals(wrong),'R2_ARCHIVE_CONFLICT_MUTATED');
  }
  {
    const {root,put}=create('ordinary-duplicate',expectedEvents);put('production/phase4/audio-actions.zip',fixture);
    reject('ordinary-duplicate-still-rejected','AUDIO_PUBLICATION_EXISTS',()=>publishArchive(root,'stage/archive.zip',root,'production/phase4/audio-actions.zip',expectedHash));
  }
  {
    const {root}=create('existing-mutation-fence',expectedEvents),state=preflightEvents(root,expectedEvents),wrong=Buffer.from('changed-after-preflight\n');
    fs.writeFileSync(inside(root,EVENTS),wrong);publishArchive(root,'stage/archive.zip',root,INTERFACE_ZIP,expectedHash);
    reject('existing-events-mutation-fence','AUDIO_EVENTS_CHANGED',()=>finishEvents(root,expectedEvents,state));
    need(bytes(root,EVENTS).equals(wrong),'R2_MUTATION_OVERWRITTEN');
  }
  {
    const {root,put}=create('exclusive-creation-fence'),state=preflightEvents(root,expectedEvents),wrong=Buffer.from('appeared-after-preflight\n');
    publishArchive(root,'stage/archive.zip',root,INTERFACE_ZIP,expectedHash);put(EVENTS,wrong);
    reject('missing-events-exclusive-fence','AUDIO_EVENTS_APPEARED',()=>finishEvents(root,expectedEvents,state));
    need(bytes(root,EVENTS).equals(wrong),'R2_RACE_OVERWRITTEN');
  }
  write(context.directory,'r2-publication-controls.json',{outcomes,fixtureSha256:expectedHash,expectedEventsSha256:hash(expectedEvents),productionFilesMutated:false});
  return outcomes.length;
}
const INTERFACE_CANDIDATES=[
  ['r',3254,'coin_pickup_v3'],['r',4864,'coin_drophitgroundv2'],['r',3195,'peasant_pickupbow'],['r',3863,'peasant_pickuphammer_v2'],
  ['r',4973,'soldiercloth_paid'],['r',3755,'chest_open'],['r',5586,'horn_blown_reverb'],['r',3558,'env_portalnormal'],
  ['r',2860,'sfx_ruler_heimdall_special_portal_open'],['r',5582,'sfx_ruler_heimdall_special_portal_close'],
  ['r',4364,'finalportal_collapsev3'],['r',4441,'env_homefire'],['s0',438,'menu_openv3'],['s0',485,'menu_closev2'],
  ['s0',449,'ui_select'],['s0',507,'ui_playerjoins'],['s0',437,'ui_playerleaves'],['s0',420,'menu_mouseover01'],['s0',392,'menu_mouseover02'],
  ['s2',68,'ui_daycounter_new_v2'],['s2',125,'amb_forest_123_day01_q0046'],['s2',138,'amb_forest_123_night01_q0064'],
  ['s2',152,'amblayer_forest_123_wind01_loop_q0003']
];
const ownerOf=key=>key==='r'?'resources.assets':key==='s0'?'sharedassets0.assets':'sharedassets2.assets';
function interfaceDecoder(){
  const selections=Object.fromEntries(INTERFACE_CANDIDATES.map(([key,id,name])=>['interface.'+key+id,[ownerOf(key),id,name,0]]));
  return decoderRecipe().replace(/SELECTIONS = [^\n]+/,'SELECTIONS = '+JSON.stringify(selections))
    .replace('[2880,3700,3131,3554,3811]','[2860,5582]');
}
const EVENT_IDS=['footstep','jump.land','melee.swing','melee.impact','equipment.broken','bow.release','arrow.contact','spit.release-impact',
  'zombie.move-attack-hit-death','survivor.hit-death-succession','wildlife.flee-hit-death','work.build-repair','tree.clear','structure.begin-complete',
  'structure.hit-collapse','money.payment','recruit.equip','inventory.transfer','food.use-sale','warning.day-night-retaliation-assault',
  'portal.active-spawn','portal.destroy','outcome','session.connection','save.error','ui.action','ambience.day-night','ambience.fire-wind','bloater.splash','gather.carry-handle'];
// Each named semantic operation selects its own exact recordings; shared PCM is explicit.
const ROUTES=[
  [['walk',['action.3294.full','action.4949.full']],['sprint',['action.3294.full','action.4949.full']],['npc-carry-retreat',['action.3294.full','action.4949.full']]],
  [['takeoff',['action.3294.jump-trim']],['landing',['action.4949.jump-trim']]],
  [['fist-knife-machete',['action.3369.full']],['club-bat',['action.4872.full']],['dual-contact',['action.3369.full','action.4872.full']]],
  [['flesh',['action.5123.full']],['armor-blunt',['action.2880.full']],['armor-arrow',['action.3700.full']]],
  [['wood-break',['action.4523.break-trim']],['metal-break',['action.3574.break-trim']]],
  [['ordinary-archer',['action.3288.full']],['tower-archer',['action.3288.full']]],
  [['terrain',['action.3640.full']],['wood',['action.4434.full']],['metal',['action.5020.full']],['prey',['action.3357.full']],['portal',['action.4692.full']],['generic',['action.5476.full']]],
  [['windup',['action.2790.full']],['release',['action.2790.full']],['direct-hit',['action.2790.full']]],
  [['movement',['action.2864.full']],['attack',['action.3150.full']],['hit',['action.2880.full','action.3700.full']],['death',['action.3131.full','action.2804.full','action.3554.full']]],
  [['damage',['action.5123.full']],['death',['action.5736.full']],['control-succession',['action.5736.full']]],
  [['flee',['action.3944.full']],['deer-hit-death',['action.4247.full']],['rabbit-hit-death',['action.3811.full','action.3357.full']]],
  [['wood-build-repair',['action.5272.full','action.3945.full']],['metal-build-repair',['action.3804.full','action.5176.full']]],
  [['mark',['action.4611.full']],['chop',['action.3741.full','action.3672.full']],['fall',['action.4523.full','action.5654.full']]],
  [['accepted-begin',['action.2862.full']],['wood-complete',['action.4543.full']],['metal-complete',['action.4245.full']]],
  [['wood-hit',['action.4169.full']],['metal-hit',['action.4041.full']],['wood-collapse',['action.4523.full','action.5654.full']],['metal-collapse',['action.3574.full','action.5154.full']]],
  [['pickup',['interface.r3254.full']],['drop',['interface.r4864.full']],['accepted-payment',['action.2862.full']],['food-sale-proceeds',['interface.r3254.full']]],
  [['recruit-appears',['interface.r4973.full']],['job-assigned',['interface.r3863.full']],['equipment-acquired',['interface.r3195.full','interface.r3863.full']]],
  [['open',['interface.s0438.full','interface.r3755.full']],['close',['interface.s0485.full']],['move',['interface.s0449.full']],['split',['interface.s0449.full']],['rotate',['interface.s0420.full']],['equip',['interface.r3195.full']]],
  [['consume',['interface.food-crunch.v1']],['sale',['interface.r3254.full']]],
  [['day',['interface.s268.full']],['night',['interface.s268.full']],['retaliation',['interface.r5586.full']],['assault',['interface.r5586.full']],['recall',['interface.s0485.full','interface.r5586.full']]],
  [['active-loop',['interface.r3558.loop']],['open',['interface.r2860.full']],['spawn',['interface.r5582.full']]],
  [['permanent-destruction',['interface.r4364.full']]],
  [['victory',['action.4543.full']],['defeat',['action.4523.full']]],
  [['join',['interface.s0507.full']],['leave',['interface.s0437.full']],['failure',['interface.s0485.negative']]],
  [['save-success',['interface.s0449.full']],['save-failure',['interface.s0485.negative']],['invalid-action',['interface.s0485.negative']],['content-error',['interface.s0485.negative']]],
  [['hover',['interface.s0420.full','interface.s0392.full']],['confirm',['interface.s0449.full']],['cancel',['interface.s0485.full']]],
  [['forest-day',['interface.s2125.loop']],['forest-night',['interface.s2138.loop']]],
  [['local-fire',['interface.r4441.loop']],['wind',['interface.s2152.loop']]],
  [['cosmetic-death-burst',['interface.bloater-burst.v1']]],
  [['gather',['interface.handling.v1']],['loot-pickup',['interface.handling.v1']],['carry-delivery',['interface.handling.v1']],['cloth-bag',['interface.handling.v1']]]
];
function eventCatalog(interfacePack){
  const action=document(ROOT,PACK),sampleMap=new Map([...action.samples,...interfacePack.samples].map(s=>[s.id,s]));
  const events=EVENT_IDS.map((id,i)=>{
    const category='AU-'+String(i+1).padStart(2,'0'),loopCategory=[20,26,27].includes(i),bus=loopCategory?'Ambience':'Effects';
    const gain=i===26?0.35:i===27?0.2:i===20?0.3:0.85;
    const routes=ROUTES[i].map(([operation,variants])=>{
      const looping=variants.every(v=>Boolean(sampleMap.get(v)?.loop));
      return {id:id+'.'+operation,operation,variants,selection:variants.length>1?'uniform-random-avoid-immediate-repeat':'single',bus,
        gainLinear:operation==='local-fire'?0.3:gain,attenuation:{mode:i>=15&&!loopCategory?'non-positional':'distance',referenceDistanceUnits:2,maximumDistanceUnits:40,rolloff:1},
        priority:[3,9,19,21,23,24].includes(i)?100:looping?10:60,cooldownMilliseconds:i===0?80:i===25?40:100,
        repetition:{maximumOverlap:4,overflow:'steal-oldest-lowest-priority',deduplicateCommittedEvent:true},
        trigger:looping?'reconcile current '+operation+' state':'accepted '+operation+' event only',
        lifecycle:{mode:looping?'current-state-loop':'one-shot',restore:looping?'reconcile-exact-current-owner':'do-not-replay-past-events',
          owner:'preview-generation-and-entity-id',onSelectionChange:'stop-owned-players',onStopAll:'stop-all-owned-players',onDispose:'release-owned-player-and-stream',
          crossfadeMilliseconds:looping?750:0},
        warningsVisibleWhenMuted:[19,21,22,23,24].includes(i),visualPairs:i===19?['UI-12','UI-26']:i===23?['UI-10','UI-11','UI-26']:
          i===24?['UI-22','UI-23','UI-26','UI-27']:i===21?['FX-05','UI-20','UI-26']:i===22?['UI-24','UI-26']:[]};
    });
    return {category,id,bus,gainLinear:gain,variants:[...new Set(routes.flatMap(r=>r.variants))],routes,
      semantics:i===20?'Active hostile portal only; no player teleport; destruction stops active loop permanently':
        i===28?'Cosmetic death splash only; no damage, burning, persistent hazard or stain loop':
        i===6?'Only actual first contact; passed allies and fired-arrow pickup emit nothing':
        i===11?'Actual accepted paid work; never free automatic repair':
        i===15?'Committed physical transfer/payment only; failed or duplicate transactions emit nothing':
        'Inspect resource routing; future gameplay/integration owns contextual event classification'};
  });
  return {schemaVersion:1,id:'sound-events',obligation:'DT-23',version:1,buses:['Master','Effects','Ambience'],
    defaults:{gameplayConcurrentLimit:24,repeatedEffectLimit:4,masterPercent:80,effectsPercent:80,ambiencePercent:60},
    configurableFields:{gameplayConcurrentLimit:{unit:'simultaneous-players',minimum:1,maximum:128},repeatedEffectLimit:{unit:'instances-per-effect',minimum:1,maximum:24},
      volumePercent:{unit:'percent',minimum:0,maximum:100}},
    sourcePreferences:'config/local-preferences.json',sourcePresentation:'config/presentation.json',
    samplePacks:['packs/audio-actions.json','packs/audio-interface-ambience.json'],events,
    audition:{autoPlayOnSelection:false,controls:['Play sound','Stop sound','Stop all audio','Loop preview'],states:['Playing','Stopped','Muted'],elapsedUnit:'seconds'},
    reconstruction:{currentLoopsOnly:true,replayHistoricalOneShots:false,obsoleteGenerationResults:'reject-before-cancellation'},
    acceptance:{newRecordings:'Not auditioned',creatorActionApprovalScope:APPROVAL.id,finalNativeMix:'pending-04-32',noMusicOrDialogueBank:true,
      existingContextSensitiveConsumerRoutes:'pending-04-23-classification'}};
}
const INTERFACE_ASSEMBLE=String.raw`import array,base64,hashlib,html,io,json,math,pathlib,random,sys,wave,zipfile
root=pathlib.Path(sys.argv[1]).resolve();assert root.is_relative_to(pathlib.Path('D:/Work/Games/KingdomZombies/.phase4').resolve())
sha=lambda x:hashlib.sha256(x).hexdigest()
config=json.loads((root/'selection.json').read_text());members={};sources=[];samples=[];bank=array.array('h')
def wav(pcm):
    out=io.BytesIO()
    with wave.open(out,'wb') as f:f.setnchannels(2);f.setsampwidth(2);f.setframerate(44100);f.writeframes(pcm)
    return out.getvalue()
def add(identity,data,source,trim,gain,loop=None,transform='trim-gain'):
    pcm=data.tobytes();payload=wav(pcm);first=len(bank)//2;bank.extend(data);end=len(bank)//2;bank.extend([0]*4410)
    member='variants/'+identity+'.wav';members[member]=payload
    peak=max(abs(v) for v in data);rms=math.sqrt(sum(v*v for v in data)/len(data))
    samples.append({'id':identity,'version':1,'sourceId':source,'variantMember':member,'sha256':sha(payload),'pcmSha256':sha(pcm),
      'frames':len(data)//2,'channels':2,'rate':44100,'sampleWidth':2,'durationSeconds':len(data)/2/44100,'trimFrames':trim,
      'gainLinear':gain,'gainDb':20*math.log10(gain),'regionFrames':[first,end],'loop':loop,'transform':transform,
      'listening':{'status':'Not auditioned','listener':None,'verdict':None,'finalOwner':'04-32'},
      'technical':{'peak':peak,'rms':rms,'nonzeroSamples':sum(v!=0 for v in data),'fullScaleSamples':sum(abs(v)>=32767 for v in data)}})
for key,object_id,name in config['candidates']:
    identity=key+str(object_id);folder=root/('interface.'+identity);record=json.loads((folder/'sample.json').read_text())
    original=(folder/(name+'.wav')).read_bytes();prefix='sources/'+identity
    members[prefix+'/original.wav']=original;members[prefix+'/stream.fsb']=base64.b64decode(record['bundle']['originalPayloadBase64'])
    members[prefix+'/audioclip.bin']=base64.b64decode(record['bundle']['rawAudioClipBase64']);del record['bundle']
    members[prefix+'/provenance.json']=(json.dumps(record,indent=2)+'\n').encode()
    sources.append({**record,'id':identity,'sourceMember':prefix+'/original.wav'})
    with wave.open(io.BytesIO(original),'rb') as f:raw=f.readframes(f.getnframes())
    values=array.array('h');values.frombytes(raw)
    if record['output']['channels']==1:values=array.array('h',(v for x in values for v in (x,x)))
    gain=min(1.0,24575/max(abs(v) for v in values))
    if object_id in [3558,4441,125,138,152]:
        fade=512;count=min(len(values)//2,44100*8+fade);original_trim=values[:count*2]
        out=array.array('h')
        for i in range(count-fade):
            for channel in range(2):
                value=original_trim[i*2+channel]
                if i<fade:
                    t=i/(fade-1);value=value*t+original_trim[(count-fade+i)*2+channel]*(1-t)
                out.append(round(value*gain))
        loop={'mode':'forward','startFrame':0,'endFrame':len(out)//2,'crossfadeFrames':fade,'sourceTrimFrames':[0,count],
          'method':'tail-head-overlap-add','auditorySeamVerdict':'Not auditioned'}
        add('interface.'+identity+'.loop',out,identity,[0,count],gain,loop,'trim-overlap-add-gain')
    else:
        add('interface.'+identity+'.full',array.array('h',(round(v*gain) for v in values)),identity,[0,len(values)//2],gain)
    if key=='s0' and object_id==485:
        count=min(6615,len(values)//2);one=array.array('h',(round(v*gain*0.7) for v in values[:count*2]))
        add('interface.s0485.negative',one+array.array('h',[0]*4410)+one,identity,[0,count],gain*0.7,None,'two-pulses-with-2205-frame-gap')
    if key=='r' and object_id==3755:
        count=min(6615,len(values)//2)
        add('interface.handling.v1',array.array('h',(round(v*gain*0.5) for v in values[:count*2])),identity,[0,count],gain*0.5)
# AU19 is an explicit source-backed catalog gap, not an unheard donor rejection.
# Deterministic non-tonal contact/crunch texture; no music, speech model or downloaded recording.
rng=random.Random(190021);food=array.array('h');previous=0.0
for i in range(11025):
    t=i/44100;noise=rng.uniform(-1,1);high=noise-previous;previous=noise
    envelope=sum(math.exp(-max(0,t-onset)*55) if t>=onset else 0 for onset in [0.01,0.07,0.13,0.19])
    value=round(max(-1,min(1,high*envelope*0.15))*16384);food.extend([value,value])
add('interface.food-crunch.v1',food,'generated-food-crunch',[0,11025],1,None,'seed190021-four-noise-transients')
with zipfile.ZipFile(config['actionsZip']) as z:
    a=json.loads(z.read('production/manifest.json'));s=next(x for x in a['samples'] if x['id']=='action.2790.full')
    with wave.open(io.BytesIO(z.read(s['variantMember'])),'rb') as f:burst=array.array('h');burst.frombytes(f.readframes(min(f.getnframes(),13230)))
add('interface.bloater-burst.v1',array.array('h',(round(v*0.65) for v in burst)),'action.2790.full',[0,len(burst)//2],0.65,None,'shared-approved-source-new-trim-not-auditioned')
pack={'schemaVersion':1,'id':'audio-interface-ambience','version':1,'status':'production-resources-listening-pending',
 'scope':'AU16-30 actual source/variant resources; all new variants Not auditioned; native mix and loop hearing pending04-32',
 'bank':{'path':'assets/audio/interface-ambience.wav','sha256':sha(wav(bank.tobytes())),'frames':len(bank)//2,'channels':2,'rate':44100,'sampleWidth':2},
 'sources':sources,'samples':samples,'listening':{'status':'Not auditioned','listener':None,'verdicts':[],'acceptanceOwner':'04-32'},
 'donorGap':{'category':'AU-19','authority':'specs/0.1/assets-and-resources.md section10 AU-19',
 'evidence':'Food-use detail has no verified suitable human donor in the approved donor inventory; animal-eating recordings are not selected as human food sounds',
 'synthesis':'Deterministic non-tonal contact/crunch texture seed190021; needs real audition','unsupportedDonorUnsuitabilityClaims':[]},
 'sharedSources':[{'id':'action.2790.full','pack':'packs/audio-actions.json','wavSha256':s['sha256'],'use':'New cosmetic burst trim; creator approval not extended'}],
 'recipes':{'decoderMember':'recipes/decode-interface.py','decoderSha256':sha((root/'decode-interface.py').read_bytes()),
 'assemblerMember':'recipes/assemble-interface.py','assemblerSha256':sha(pathlib.Path(__file__).read_bytes())}}
members['recipes/decode-interface.py']=(root/'decode-interface.py').read_bytes();members['recipes/assemble-interface.py']=pathlib.Path(__file__).read_bytes()
members['recipes/selection.json']=(root/'selection.json').read_bytes()
rows=[]
for s in samples:
    rows.append('<h2>'+s['id']+' v1 — Not auditioned</h2><p>Frames '+str(s['regionFrames'])+'; trim '+str(s['trimFrames'])+'; SHA256 '+s['sha256']+'</p>'
    +'<audio controls preload="none" '+('loop ' if s['loop'] else '')+'src="../'+s['variantMember']+'"></audio>')
members['review/index.html']=('<!doctype html><html lang="en"><meta charset="utf-8"><title>AU16-30 source review</title><style>body{font:16px sans-serif;max-width:900px;margin:auto}p{overflow-wrap:anywhere}audio{width:100%}</style>'
 '<h1>AU16-30 — Not auditioned</h1><p>Deliberate playback only. Check full source and variant for suitability/music/speech/identity/distortion/onset/tail. '
 'Repeat ambience across boundaries; record seam/pump/drop observations. No final game mix approval is presumed.</p>'+''.join(rows)+'</html>').encode()
members['review/questions.txt']=b'For each exact version: suitable category and material? unwanted speech/music/source identity? silence/distortion? onset/tail/punch? For five loops hear several boundaries and quiet action-overlay balance in later native mix. Record actual listener/device/time and verdict; creator action approval does not cover these new versions.'
pack['bundleMembers']=[{'path':k,'bytes':len(v),'sha256':sha(v)} for k,v in sorted(members.items())]
members['production/manifest.json']=(json.dumps(pack,indent=2,allow_nan=False)+'\n').encode()
with zipfile.ZipFile(root/'audio-interface-ambience.zip','w',compression=zipfile.ZIP_DEFLATED,compresslevel=9) as z:
    for name,payload in sorted(members.items()):
        info=zipfile.ZipInfo(name,(2026,11,18,0,0,0));info.compress_type=zipfile.ZIP_DEFLATED;z.writestr(info,payload,compresslevel=9)
pack['bundleSha256']=sha((root/'audio-interface-ambience.zip').read_bytes())
(root/'audio-interface-ambience.json').write_text(json.dumps(pack,indent=2,allow_nan=False)+'\n',encoding='utf-8',newline='\n')
(root/'interface-ambience.wav').write_bytes(wav(bank.tobytes()))
print(json.dumps({'sources':len(sources),'variants':len(samples),'zipBytes':(root/'audio-interface-ambience.zip').stat().st_size,'bankBytes':len(wav(bank.tobytes()))}))
`;
async function prepareInterface(){
  const context={directory:inside(ROOT,'.phase4/audio-interface/'+crypto.randomUUID())};fs.mkdirSync(context.directory,{recursive:true});
  fs.writeFileSync(inside(context.directory,'decode-interface.py'),interfaceDecoder());fs.writeFileSync(inside(context.directory,'assemble-interface.py'),INTERFACE_ASSEMBLE);
  write(context.directory,'selection.json',{candidates:INTERFACE_CANDIDATES,actionsZip:inside(ROOT,ZIP)});
  for(const [key,id]of INTERFACE_CANDIDATES)await runner.command(context,'decode-'+key+id,PYTHON,
    ['-B',inside(context.directory,'decode-interface.py'),'--child','interface.'+key+id,inside(context.directory,'interface.'+key+id)],{timeout:40000});
  await runner.command(context,'assemble',PYTHON,['-B',inside(context.directory,'assemble-interface.py'),context.directory],{timeout:40000});
  const stagedPack=document(context.directory,'audio-interface-ambience.json'),catalog=eventCatalog(stagedPack),expectedEvents=serializeEvents(catalog),eventState=preflightEvents(ROOT,expectedEvents);
  for(const [source,target]of [['interface-ambience.wav',INTERFACE_BANK],['audio-interface-ambience.json',INTERFACE_PACK],['audio-interface-ambience.zip',INTERFACE_ZIP]]){
    need(!fs.existsSync(inside(ROOT,target)),'AUDIO_OUTPUT_EXISTS',target);fs.mkdirSync(path.dirname(inside(ROOT,target)),{recursive:true});
    need(fs.statSync(inside(context.directory,source)).size<=archiveLimit(target),'AUDIO_EXISTING_READER_BOUND',source);fs.copyFileSync(inside(context.directory,source),inside(ROOT,target));
  }
  archiveBytes(ROOT,INTERFACE_ZIP,stagedPack.bundleSha256);finishEvents(ROOT,expectedEvents,eventState);console.log(JSON.stringify({directory:context.directory}));
}
function validateInterface(pack,bank,events){
  const f=pcm(bank);need(pack.id==='audio-interface-ambience'&&pack.version===1,'INTERFACE_PACK');
  need(hash(bank)===pack.bank.sha256&&f.frames===pack.bank.frames,'INTERFACE_BANK_HASH');
  need(pack.sources.length===23&&pack.samples.length===27,'INTERFACE_COUNTS');
  need(equal(pack.listening,{status:'Not auditioned',listener:null,verdicts:[],acceptanceOwner:'04-32'}),'INTERFACE_FALSE_AUDITION');
  for(const [key,id,name]of INTERFACE_CANDIDATES){const s=pack.sources.find(x=>x.id===key+id);
    need(s&&s.source.owner===ownerOf(key)&&s.source.objectId===id&&s.source.type==='AudioClip'&&s.source.name===name,'INTERFACE_SOURCE_IDENTITY',key+id);
    need(s.output.channels===([2860,5582].includes(id)?1:2)&&s.output.rate===44100&&s.output.sampleWidth===2,'INTERFACE_SOURCE_FORMAT',key+id);
    need(s.preservation.verifiedAfter&&equal(s.preservation.originalsBefore,s.preservation.originalsAfter),'INTERFACE_ORIGINAL_MUTATION',key+id);
  }
  const raw=bank.subarray(44);let loops=0;const ranges=[];
  for(const s of pack.samples){
    need(s.version===1&&s.channels===2&&s.rate===44100&&s.sampleWidth===2,'INTERFACE_SAMPLE_FORMAT',s.id);
    need(s.listening.status==='Not auditioned'&&s.listening.listener===null&&s.listening.verdict===null,'INTERFACE_FALSE_AUDITION',s.id);
    const [a,b]=s.regionFrames;need([a,b,s.frames,...s.trimFrames].every(Number.isSafeInteger)&&a>=0&&b>a&&b<=f.frames&&b-a===s.frames,'INTERFACE_REGION',s.id);
    need(s.durationSeconds===s.frames/44100&&Number.isFinite(s.gainDb)&&s.gainLinear>0&&s.gainLinear<=1&&
      Math.abs(s.gainDb-20*Math.log10(s.gainLinear))<1e-12,'INTERFACE_METADATA',s.id);
    const data=raw.subarray(a*4,b*4);need(hash(data)===s.pcmSha256,'INTERFACE_REGION_HASH',s.id);let peak=0,nonzero=0,squares=0;
    for(let i=0;i<data.length;i+=2){const v=data.readInt16LE(i);peak=Math.max(peak,Math.abs(v));nonzero+=v!==0;squares+=v*v;}
    need(peak>0&&peak<=24575&&nonzero>0&&peak===s.technical.peak&&nonzero===s.technical.nonzeroSamples&&s.technical.fullScaleSamples===0&&
      Math.abs(Math.sqrt(squares/(data.length/2))-s.technical.rms)<1e-8,'INTERFACE_PCM_DOMAIN',s.id);
    if(s.loop){loops++;need(s.loop.startFrame===0&&s.loop.endFrame===s.frames&&s.loop.crossfadeFrames===512&&
      s.frames===s.trimFrames[1]-s.trimFrames[0]-512&&s.loop.auditorySeamVerdict==='Not auditioned','INTERFACE_LOOP',s.id);}
    ranges.push([a,b]);
  }
  ranges.sort((a,b)=>a[0]-b[0]);need(ranges.every((x,i)=>i===0||x[0]>=ranges[i-1][1])&&loops===5,'INTERFACE_LOOP_COUNT');
  need(pack.donorGap.category==='AU-19'&&pack.donorGap.unsupportedDonorUnsuitabilityClaims.length===0,'INTERFACE_DONOR_GAP');
  need(equal(events,eventCatalog(pack)),'AUDIO_EVENT_CONTRACT');
  need(events.events[27].routes.find(r=>r.operation==='local-fire').gainLinear===0.3&&
    events.events[27].routes.find(r=>r.operation==='wind').gainLinear===0.2&&events.events[26].gainLinear===0.35&&
    equal(events.events[29].routes.find(r=>r.operation==='carry-delivery').variants,['interface.handling.v1'])&&
    events.events[24].routes.every(r=>r.visualPairs.includes('UI-27')&&!r.visualPairs.includes('UI-21'))&&
    events.events[22].routes.every(r=>r.visualPairs.includes('UI-24')&&r.warningsVisibleWhenMuted),'AUDIO_CANONICAL_ROUTING');
  const action=document(ROOT,PACK),known=new Map([...action.samples,...pack.samples].map(s=>[s.id,s]));
  need(equal(events.events.map(e=>e.id),EVENT_IDS)&&events.events.length===30,'AUDIO_EVENT_IDS');
  for(const e of events.events)for(const r of e.routes){need(r.variants.length>0&&r.variants.every(id=>known.has(id)),'AUDIO_EVENT_SAMPLE',r.id);
    need(r.lifecycle.mode!=='current-state-loop'||r.variants.every(id=>Boolean(known.get(id).loop)),'AUDIO_EVENT_LOOP',r.id);}
  const prefs=document(ROOT,'src/godot-game/content/config/local-preferences.json'),presentation=document(ROOT,'src/godot-game/content/config/presentation.json');
  need(events.defaults.masterPercent===prefs.audio.masterPercent&&events.defaults.effectsPercent===prefs.audio.effectsPercent&&events.defaults.ambiencePercent===prefs.audio.ambiencePercent&&
    events.defaults.gameplayConcurrentLimit===presentation.audio.gameplayConcurrentLimit&&events.defaults.repeatedEffectLimit===presentation.audio.repeatedEffectLimit,'AUDIO_DEFAULT_SOURCE');
  return {categories:30,sources:23,newVariants:27,loopCount:loops,events:events.events.length,routes:events.events.reduce((n,e)=>n+e.routes.length,0),listening:'New variants Not auditioned; final04-32 pending'};
}
const INTERFACE_ZIP_CHECK=String.raw`import array,hashlib,io,json,math,pathlib,random,sys,wave,zipfile
root=pathlib.Path(sys.argv[1]);pack=json.loads((root/'src/godot-game/content/packs/audio-interface-ambience.json').read_text());sha=lambda b:hashlib.sha256(b).hexdigest()
with zipfile.ZipFile(root/'production/phase4/audio-interface-ambience.zip') as z:
    expected={x['path']:x for x in pack['bundleMembers']}
    assert len(z.infolist())==len(expected)+1 and set(z.namelist())==set(expected)|{'production/manifest.json'},'INTERFACE_ZIP_MEMBERS'
    assert sum(i.file_size for i in z.infolist())<=128*1024*1024,'INTERFACE_ZIP_TOTAL'
    for i in z.infolist():
        p=pathlib.PurePosixPath(i.filename);assert not p.is_absolute() and '..' not in p.parts and '\\' not in i.filename and ':' not in i.filename and 0<i.file_size<=32*1024*1024,'INTERFACE_ZIP_PATH'
    for name,record in expected.items():
        b=z.read(name);assert sha(b)==record['sha256'] and len(b)==record['bytes'],'INTERFACE_ZIP_HASH'
    assert json.loads(z.read('production/manifest.json'))=={k:v for k,v in pack.items() if k!='bundleSha256'},'INTERFACE_ZIP_REBOUND'
    for s in pack['sources']:
        b=z.read(s['sourceMember']);assert sha(b)==s['output']['sha256'],'INTERFACE_ORIGINAL_HASH'
        prefix='sources/'+s['id'];assert sha(z.read(prefix+'/stream.fsb'))==s['source']['stream']['sha256'] and sha(z.read(prefix+'/audioclip.bin'))==s['source']['rawObjectSha256'],'INTERFACE_RAW_HASH'
        with wave.open(io.BytesIO(b),'rb') as f:
            pcm=f.readframes(f.getnframes()+1);assert f.getnframes()==s['output']['frames'] and f.getnchannels()==s['output']['channels'] and f.getframerate()==44100 and len(pcm)==f.getnframes()*f.getnchannels()*2 and sha(pcm)==s['output']['pcmSha256'],'INTERFACE_SOURCE_TRUNCATED'
    for s in pack['samples']:
        b=z.read(s['variantMember']);assert sha(b)==s['sha256'],'INTERFACE_VARIANT_HASH'
        with wave.open(io.BytesIO(b),'rb') as f:
            pcm=f.readframes(f.getnframes()+1);assert f.getnframes()==s['frames'] and f.getnchannels()==2 and f.getframerate()==44100 and len(pcm)==s['frames']*4 and sha(pcm)==s['pcmSha256'],'INTERFACE_VARIANT_TRUNCATED'
        if s['sourceId']=='generated-food-crunch':
            rng=random.Random(190021);out=array.array('h');previous=0.0
            for i in range(11025):
                t=i/44100;noise=rng.uniform(-1,1);high=noise-previous;previous=noise
                envelope=sum(math.exp(-max(0,t-onset)*55) if t>=onset else 0 for onset in [0.01,0.07,0.13,0.19])
                v=round(max(-1,min(1,high*envelope*0.15))*16384);out.extend([v,v])
        else:
            if s['sourceId']=='action.2790.full':
                with zipfile.ZipFile(root/'production/phase4/audio-actions.zip') as actions:
                    a=json.loads(actions.read('production/manifest.json'));source=next(v for v in a['samples'] if v['id']==s['sourceId'])
                    original=actions.read(source['variantMember'])
            else:
                source=next(v for v in pack['sources'] if v['id']==s['sourceId']);original=z.read(source['sourceMember'])
            with wave.open(io.BytesIO(original),'rb') as f:values=array.array('h');values.frombytes(f.readframes(f.getnframes()));channels=f.getnchannels()
            if channels==1:values=array.array('h',(v for x in values for v in (x,x)))
            start,end=s['trimFrames'];assert 0<=start<end<=len(values)//2,'INTERFACE_TRIM_BOUNDS';values=values[start*2:end*2]
            if s['loop']:
                count=len(values)//2;fade=s['loop']['crossfadeFrames'];out=array.array('h')
                for i in range(count-fade):
                    for ch in range(2):
                        value=values[i*2+ch]
                        if i<fade:
                            t=i/(fade-1);value=value*t+values[(count-fade+i)*2+ch]*(1-t)
                        out.append(round(value*s['gainLinear']))
            else:
                out=array.array('h',(round(v*s['gainLinear']) for v in values))
                if s['transform']=='two-pulses-with-2205-frame-gap':out=out+array.array('h',[0]*4410)+out
        assert out.tobytes()==pcm,'INTERFACE_RECIPE_REPRODUCTION'
print(json.dumps({'members':len(expected)+1,'completeSources':len(pack['sources']),'completeVariants':len(pack['samples']),'exactRecipeReproductions':len(pack['samples'])}))
`;
async function bank(context){
  const files=[INTERFACE_PACK,INTERFACE_BANK,INTERFACE_ZIP,EVENTS,PACK,ZIP,'tools/phase4/cases/audio.cjs','tools/phase4/extract-audio.py',
    'src/godot-game/content/config/presentation.json','src/godot-game/content/config/local-preferences.json'];
  const p=document(ROOT,INTERFACE_PACK);
  const before=Object.fromEntries(files.map(file=>[file,hash(file===INTERFACE_ZIP?archiveBytes(ROOT,file,p.bundleSha256):bytes(ROOT,file))]));write(context.directory,'inputs.json',before);
  const b=bytes(ROOT,INTERFACE_BANK),e=document(ROOT,EVENTS),observations=validateInterface(p,b,e);
  need(hash(interfaceDecoder())===p.recipes.decoderSha256&&hash(INTERFACE_ASSEMBLE)===p.recipes.assemblerSha256,'INTERFACE_RECIPE_IDENTITY');
  fs.writeFileSync(inside(context.directory,'check-interface.py'),INTERFACE_ZIP_CHECK);
  const archive=await runner.command(context,'retained-interface',PYTHON,['-B',inside(context.directory,'check-interface.py'),ROOT],{timeout:40000});
  const controls=[];
  for(const [name,code,edit]of [
    ['missing-source','INTERFACE_COUNTS',x=>x.sources.pop()],['source-rebound','INTERFACE_SOURCE_IDENTITY',x=>{x.sources[0].source.objectId=4864;}],
    ['fractional-frame','INTERFACE_REGION',x=>{x.samples[0].regionFrames[0]+=0.5;}],['hash-rebound','INTERFACE_REGION_HASH',x=>{x.samples[0].pcmSha256='0'.repeat(64);}],
    ['false-audition','INTERFACE_FALSE_AUDITION',x=>{x.samples[0].listening.listener='creator';}],
    ['wrong-loop','INTERFACE_LOOP',x=>{x.samples.find(s=>s.loop).loop.endFrame++;}],['wrong-bank','INTERFACE_BANK_HASH',x=>{x.bank.sha256='0'.repeat(64);}],
    ['source-mutation','INTERFACE_ORIGINAL_MUTATION',x=>{x.sources[0].preservation.originalsAfter[0].sha256='0'.repeat(64);}],
    ['wrong-rate','INTERFACE_SAMPLE_FORMAT',x=>{x.samples[0].rate=48000;}],['wrong-channel','INTERFACE_SAMPLE_FORMAT',x=>{x.samples[0].channels=1;}],
    ['clipping-metadata','INTERFACE_PCM_DOMAIN',x=>{x.samples[0].technical.fullScaleSamples=1;}]
  ]){const x=structuredClone(p);edit(x);let error;try{validateInterface(x,b,e);}catch(failure){error=failure.message;}need(error?.split(' ')[0]===code,'INTERFACE_NEGATIVE_CONTROL',name+' '+error);controls.push({name,diagnostic:error});validateInterface(p,b,e);}
  for(const [name,edit]of [['wrong-limit',x=>{x.defaults.gameplayConcurrentLimit=25;}],['wrong-preferences',x=>{x.defaults.masterPercent=81;}],
    ['wrong-event-category',x=>{x.events[0].category='AU-30';}],['missing-variant',x=>{x.events[0].routes[0].variants=['missing'];}],
    ['portal-teleport',x=>{x.events[20].semantics='player teleport';}],['acid-hazard',x=>{x.events[28].semantics='persistent damage loop';}],
    ['loop-replay',x=>{x.reconstruction.replayHistoricalOneShots=true;}]]){
    const x=structuredClone(e);edit(x);let error;try{validateInterface(p,b,x);}catch(failure){error=failure.message;}
    need(error?.split(' ')[0]==='AUDIO_EVENT_CONTRACT','INTERFACE_EVENT_CONTROL',name+' '+error);controls.push({name,diagnostic:error});validateInterface(p,b,e);
  }
  let truncation;try{validateInterface(p,b.subarray(0,100),e);}catch(error){truncation=error.message;}
  need(truncation?.split(' ')[0]==='WAV_HEADER','INTERFACE_TRUNCATION_CONTROL');controls.push({name:'truncated-bank',diagnostic:truncation});validateInterface(p,b,e);
  const boundaryControls=archiveControls(context);
  const r2Controls=publicationControls(context);
  need(equal(before,Object.fromEntries(files.map(file=>[file,hash(file===INTERFACE_ZIP?archiveBytes(ROOT,file,p.bundleSha256):bytes(ROOT,file))]))),'INTERFACE_INPUT_CHANGED');write(context.directory,'negative-controls.json',controls);
  need(archiveBytes(ROOT,INTERFACE_ZIP,p.bundleSha256).length<=64*1024*1024,'INTERFACE_RESTORED_ARCHIVE');
  return {...observations,archive:JSON.parse(archive.stdout),rejectedControls:controls.length,restoredPositives:controls.length,boundaryControls,r2Controls,scope:'Source/headless definitions and real recordings; no new hearing/mix credit'};
}
runner.registerCase('audio.bank',{group:'audio',title:'Thirty canonical sound categories and exact interface/ambience event resources',importance:'Critical',
  rationale:'Missing sound resources, invalid loops or false approval invalidate required production content',needsGameWindow:false,runGroup:'G0',order:122,
  requirements:['ASSET-01','ASSET-02','ASSET-04'],acceptance:['AC-03','AC-12','AC-14','AC-19'],
  setup:'Exact tracked action/interface banks and DT23 catalog; existing pinned Python; no game or automatic audition',
  actions:['Validate all canonical AU categories and explicit semantic routes','Check complete source/WAV/raw bundles and exact PCM region/loop metadata',
    'Reject false approval, wrong source/frames/loops/limits/preferences/category/portal/acid/restoration semantics','Retain Not auditioned state for every new recording'],
  expected:'Thirty categories resolve actual recordings, configurable24/4 and80/80/60 defaults, owned current loops and truthful pending hearing',
  reset:'Fresh invocation-owned evidence; no source mutation, playback, game window or native mix judgment',
  evidence:['inputs.json','retained-interface.command.json','negative-controls.json','archive-reader-controls.json','r2-publication-controls.json','result.json']},bank);
module.exports={CANDIDATES,COVERAGE,ASSEMBLE,PROMOTE,APPROVAL,decoderRecipe,prepare,promote,validate,technical,actions,
  INTERFACE_CANDIDATES,INTERFACE_ASSEMBLE,interfaceDecoder,eventCatalog,prepareInterface,publishInterface,validateInterface,archiveBytes,publishArchive,bank};
if(require.main===module){
  const command=process.argv[2];
  (command==='--prepare'?prepare():command==='--approve-actions'?promote():command==='--prepare-interface'?prepareInterface():command==='--publish-interface'?publishInterface(process.argv[3]):command==='--technical'?technical({directory:fs.mkdtempSync(inside(ROOT,'.phase4/audio-technical-'))}):
    Promise.reject(new Error('USAGE --prepare | --technical'))).then(value=>{if(value)console.log(JSON.stringify(value));}).catch(error=>{console.error(error.stack);process.exitCode=1;});
}

// Native observations below are technical playback evidence, never hearing or final mix approval.
const AUDIO_FIXTURE='src/godot-game/content/fixtures/audio-states.json';
const AUDIO_HARNESS=String.raw`using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;

namespace KingdomZombies.AudioProof;

public sealed partial class InventoryHarness : Node
{
    public override async void _Ready()
    {
        try { await Run(); }
        catch (Exception error) { Check("fixture-exception", false, error.ToString()); }
        var result = new { pid = OS.GetProcessId(), nonce = OS.GetEnvironment("INVENTORY_NONCE"),
            failed = _failed, outcomes = _outcomes, assemblySha256 = Hash(Read("subjects/KingdomZombies.dll")),
            coreSha256 = Hash(Read("subjects/KingdomZombies.Core.dll")), scope = "Source/headless actual players only; no hearing" };
        System.IO.File.WriteAllText(ProjectSettings.GlobalizePath("res://inventory-results.json"),
            JsonSerializer.Serialize(result));
        GD.Print("INVENTORY_NATIVE_COMPLETE " + result.nonce + " " + (_failed ? "FAIL" : "PASS"));
        GetTree().Quit(_failed ? 1 : 0);
    }

    private readonly List<object> _outcomes = new();
    private Type _type = null!;
    private Type _gateway = null!;
    private bool _failed;
    private static readonly string Fixture = "content/fixtures/audio-states.json";
    private static readonly string Events = "content/audio/events.json";
    private static bool Eq(string? a, string? b) => StringComparer.Ordinal.Equals(a,b);
    private static string Hash(byte[] b) => Convert.ToHexStringLower(SHA256.HashData(b));
    private static byte[] Read(string file) => Godot.FileAccess.GetFileAsBytes("res://"+file);
    private void Check(string id, bool pass, object detail)
    {
        _outcomes.Add(new { id, pass, detail }); _failed |= !pass;
    }
    private object Call(object owner, string method, params object[] args) => _type.GetMethod(method,
        BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(owner,args)!;
    private long Epoch(object owner) => (long)_type.GetProperty("Epoch",BindingFlags.NonPublic|BindingFlags.Instance)!
        .GetValue(owner)!;
    private static VBoxContainer Panel(Node root) => root.GetNode<VBoxContainer>("Audio/Content");
    private static AudioStreamPlayer[] Players(Node root) => root.GetChildren().OfType<AudioStreamPlayer>().ToArray();
    private object New(Node2D root, string category, Func<string,byte[]?>? read = null)
    {
        read ??= file => Read(file);
        return Activator.CreateInstance(_type,BindingFlags.Instance|BindingFlags.NonPublic,null,
            new object[] { root, Activator.CreateInstance(_gateway,true)!,read,category },CultureInfo.InvariantCulture)!;
    }
    private static void Press(Node root, string name) => Panel(root).GetNode<Button>(name)
        .EmitSignal(BaseButton.SignalName.Pressed);
    private static void Select(Node root, int index)
    {
        var selector = Panel(root).GetNode<OptionButton>("Event"); selector.Select(index);
        selector.EmitSignal(OptionButton.SignalName.ItemSelected,index);
    }
    private async Task Frames()
    {
        await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
    }
    private async Task Run()
    {
        var alc=AssemblyLoadContext.GetLoadContext(GetType().Assembly)!;
        var core=alc.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.Core.dll"));
        var game=alc.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.dll"));
        _type=game.GetType("KingdomZombies.Inspection.AudioPreview",true)!;
        _gateway=game.GetType("KingdomZombies.Proofs.MainThreadGateway",true)!;
        Check("actual-production-alc",AssemblyLoadContext.GetLoadContext(game)==alc
            &&AssemblyLoadContext.GetLoadContext(core)==alc&&!_type.IsPublic&&!typeof(GodotObject).IsAssignableFrom(_type),game.FullName!);
        using var catalog=JsonDocument.Parse(Read(Events));
        foreach(var category in catalog.RootElement.GetProperty("events").EnumerateArray())
        {
            string id=category.GetProperty("category").GetString()!;
            var root=new Node2D();AddChild(root);var owner=New(root,id);
            try
            {
                var selector=Panel(root).GetNode<OptionButton>("Event");
                Check(id+"-all-routes",selector.ItemCount==category.GetProperty("routes").GetArrayLength(),selector.ItemCount);
                Check(id+"-selection-silent",Players(root).Length==0,Panel(root).GetNode<Label>("Status").Text);
                for(int i=0;i<selector.ItemCount;i++)
                {
                    Select(root,i);Panel(root).GetNode<CheckButton>("Loop").ButtonPressed=true;Press(root,"Play");
                    var players=Players(root);string route=selector.GetItemText(i);
                    Check(route+"-actual-playing",players.Length==1&&players[0].Playing,players.Length);
                    if(players.Length==1) Inspect(players[0],route);
                    Call(owner,"Tick",1.0);Press(root,"Stop");
                    Check(route+"-stopped-freed",Players(root).Length==0&&!GodotObject.IsInstanceValid(players[0]),route);
                }
            }
            finally { Call(owner,"Close"); root.Free(); }
        }
        Limits();
        await Lifetime();
        await Backstops();
        await Hub();
    }
    private void Limits()
    {
        var root=new Node2D();AddChild(root);var owner=New(root,"AU-01");
        string[] routes={"footstep.walk","footstep.sprint","footstep.npc-carry-retreat",
            "jump.land.takeoff","jump.land.landing","melee.swing.club-bat"};
        int number=0;
        foreach(string route in routes)for(int i=0;i<4;i++)
        {
            Call(owner,"Tick",1.0);
            Check("H19-admit-"+number,(bool)Call(owner,"Trigger",route,"limit-"+number++,Epoch(owner),true,false),route);
        }
        Check("H19-exact-global24",Players(root).Length==24&&Players(root).All(p=>p.Playing),Players(root).Length);
        var previous=Players(root).Where(p=>Eq(p.GetMeta("route").AsString(),routes[0])).Select(p=>p.GetInstanceId()).ToArray();
        Call(owner,"Tick",1.0);Call(owner,"Trigger",routes[0],"effect-overflow",Epoch(owner),true,false);
        Check("H19-exact-effect4",Players(root).Length==24&&Players(root).Count(p=>Eq(p.GetMeta("route").AsString(),routes[0]))==4
            &&!Players(root).Any(p=>p.GetInstanceId()==previous[0]),Players(root).Length);
        Call(owner,"Trigger","warning.day-night-retaliation-assault.assault","critical-priority",Epoch(owner),true,false);
        Check("H19-priority-admitted",Players(root).Length==24&&Players(root).Any(p=>Eq(p.GetMeta("route").AsString(),
            "warning.day-night-retaliation-assault.assault")),Players(root).Length);
        Panel(root).GetNode<SpinBox>("gameplayConcurrentLimit").Value=1;
        Check("H19-lowered-limit-preserves-priority",Players(root).Length==1&&Eq(Players(root)[0].GetMeta("route").AsString(),
            "warning.day-night-retaliation-assault.assault"),Players(root).Length);
        Call(owner,"Tick",1.0);
        Check("H19-low-priority-denied",!(bool)Call(owner,"Trigger",routes[1],"low-after-critical",Epoch(owner),true,false)
            &&Players(root).Length==1,Players(root).Length);
        Call(owner,"Close");root.Free();
    }
    private void Inspect(AudioStreamPlayer player,string route)
    {
        string sample=player.GetMeta("sample").AsString();JsonElement found=default,bank=default;
        foreach(string file in new[]{"content/packs/audio-actions.json","content/packs/audio-interface-ambience.json"})
        {
            using var pack=JsonDocument.Parse(Read(file));
            foreach(var item in pack.RootElement.GetProperty("samples").EnumerateArray())
                if(Eq(item.GetProperty("id").GetString(),sample))
                { found=item.Clone();bank=pack.RootElement.GetProperty("bank").Clone(); }
        }
        var stream=(AudioStreamWav)player.Stream;int start=found.GetProperty("regionFrames")[0].GetInt32();
        int end=found.GetProperty("regionFrames")[1].GetInt32();byte[] bytes=Read(bank.GetProperty("path").GetString()!);
        byte[] expected=bytes.AsSpan(44+start*4,(end-start)*4).ToArray();
        Check(route+"-exact-pcm",expected.SequenceEqual(stream.Data)&&Eq(Hash(stream.Data),found.GetProperty("pcmSha256").GetString()),new{sample,start,end,hash=Hash(stream.Data)});
        Check(route+"-native-format",stream.MixRate==44100&&stream.Stereo&&stream.Format==AudioStreamWav.FormatEnum.Format16Bits,stream.MixRate);
        Check(route+"-native-loop-boundary",stream.LoopMode==AudioStreamWav.LoopModeEnum.Forward&&stream.LoopBegin==0&&stream.LoopEnd==end-start,new{stream.LoopBegin,stream.LoopEnd});
        Check(route+"-exact-binding",Eq(player.GetMeta("route").AsString(),route)&&player.GetMeta("epoch").AsInt64()>0,sample);
        if(Eq(sample,"interface.food-crunch.v1")||Eq(sample,"interface.bloater-burst.v1"))
        {
            string info=Panel(player.GetParent()).GetNode<TextEdit>("Details").Text;
            bool generated=Eq(sample,"interface.food-crunch.v1");
            Check(generated?"generated-current-source-info":"shared-current-source-info",
                info.Contains(generated?"seed190021":"action.2790.full",StringComparison.Ordinal)
                &&info.Contains(generated?"recipes/assemble-interface.py":"9b29b74a94784a96307282820866e3be9731670dc32e9baa2ddd3bdbcb53f4cb",StringComparison.Ordinal)
                &&info.Contains("derivedListening",StringComparison.Ordinal)
                &&info.Contains("Not auditioned",StringComparison.Ordinal),info);
        }
    }
    private async Task Lifetime()
    {
        var root=new Node2D();AddChild(root);var owner=New(root,"AU-20");
        var expected=new[]{"ambience.day-night.forest-day","ambience.fire-wind.local-fire"};
        Call(owner,"Restore",(object)expected);var original=Players(root).Select(p=>p.GetInstanceId()).Order().ToArray();
        int events=Panel(root).GetMeta("one_shots").AsInt32();
        for(int i=0;i<19;i++)
        {
            Call(owner,"Restore",(object)expected);
            Check("H25-restore-"+i,Players(root).Select(p=>p.GetInstanceId()).Order().SequenceEqual(original)
                &&Players(root).All(p=>p.Playing)&&Panel(root).GetMeta("one_shots").AsInt32()==events,
                Players(root).Select(p=>p.GetMeta("route").AsString()).ToArray());
        }
        long old=Epoch(owner);Call(owner,"Close");
        Check("H26-close-frees-players",Players(root).Length==0,Players(root).Length);
        var replacement=New(root,"AU-20");
        Check("H26-global-owner-fence",Epoch(replacement)>old&&!(bool)Call(replacement,"Trigger",
            "warning.day-night-retaliation-assault.assault","stale-A",old,true,false),Epoch(replacement));
        Check("H26-current-A-restored",(bool)Call(replacement,"Trigger","warning.day-night-retaliation-assault.assault",
            "current-A",Epoch(replacement),true,false)&&Players(root).Length==1,Players(root).Length);
        var retired=Players(root)[0];Select(root,0);
        Check("H26-select-stops-A-silent-B",Players(root).Length==0&&!GodotObject.IsInstanceValid(retired),Players(root).Length);
        var mute=Panel(root).GetNode<CheckButton>("Mute");mute.ButtonPressed=true;
        Check("H26-muted-warning",Panel(root).GetNode<Label>("Status").Text.Contains("Sound muted",StringComparison.Ordinal)
            &&Panel(root).GetNode<Label>("Status").Text.Contains("Critical visual warning",StringComparison.Ordinal),Panel(root).GetNode<Label>("Status").Text);
        Check("muted-immediate-repeat-cooldown",!(bool)Call(replacement,"Trigger",
            "warning.day-night-retaliation-assault.assault","immediate-muted-repeat",Epoch(replacement),true,false)
            &&Players(root).Length==0,Players(root).Length);
        Call(replacement,"Tick",0.101);
        Press(root,"Play");Check("muted-actual-native-volume",Players(root).Length==1&&Players(root)[0].VolumeDb<=-80,Players(root).Length);
        Call(replacement,"Close");root.Free();await Frames();
    }
    private Func<string,byte[]?> ChangedCatalog(Action<JsonObject> change)
    {
        var data=JsonNode.Parse(Read(Events))!.AsObject();change(data);byte[] bytes=Encoding.UTF8.GetBytes(data.ToJsonString());
        var fixture=JsonNode.Parse(Read(Fixture))!;
        foreach(var binding in fixture["bindings"]!.AsArray())if(Eq(binding!["file"]!.GetValue<string>(),Events))binding["sha256"]=Hash(bytes);
        byte[] fixtureBytes=Encoding.UTF8.GetBytes(fixture.ToJsonString());
        return file=>Eq(file,Events)?bytes:Eq(file,Fixture)?fixtureBytes:Read(file);
    }
    private async Task Backstops()
    {
        var root=new Node2D();AddChild(root);var owner=New(root,"AU-29");
        Check("AUDIO-AUDITION.populated",Panel(root).GetNode<OptionButton>("Event").ItemCount==1
            &&Panel(root).GetNode<TextEdit>("Details").Text.Contains("source",StringComparison.OrdinalIgnoreCase),Panel(root).GetNode<TextEdit>("Details").Text);
        await Frames();var scroll=root.GetNode<ScrollContainer>("Audio");
        Check("AUDIO-AUDITION.overflow",scroll.FollowFocus&&scroll.GetVScrollBar().MaxValue>scroll.GetVScrollBar().Page,scroll.GetVScrollBar().MaxValue);
        Call(owner,"Close");root.Free();
        root=new Node2D();AddChild(root);owner=New(root,"AU-20",ChangedCatalog(d=>d["events"]=new JsonArray()));
        Check("AUDIO-AUDITION.empty",Panel(root).GetNode<OptionButton>("Event").ItemCount==0
            &&Panel(root).GetNode<Button>("Play").Disabled&&Players(root).Length==0,Players(root).Length);
        Call(owner,"Close");root.Free();
        root=new Node2D();AddChild(root);owner=New(root,"AU-01",file=>Eq(file,"assets/audio/actions.wav")?null:Read(file));
        Check("AUDIO-AUDITION.error",Panel(root).GetNode<TextEdit>("Details").Text.Contains(
            "assets/audio/actions.wav, field bytes, value read — missing PCM bank",StringComparison.Ordinal),Panel(root).GetNode<TextEdit>("Details").Text);
        Call(owner,"Close");root.Free();
        root=new Node2D();AddChild(root);owner=New(root,"AU-20",file=>Eq(file,"assets/audio/actions.wav")?null:Read(file));
        Check("AUDIO-AUDITION.partial",Panel(root).GetNode<OptionButton>("Event").ItemCount==5&&Players(root).Length==0
            &&Panel(root).GetNode<TextEdit>("Details").Text.Contains("missing PCM bank",StringComparison.Ordinal),Panel(root).GetNode<OptionButton>("Event").ItemCount);
        Call(owner,"Close");root.Free();
        string longText="independent-long-audio-metadata-"+new string('W',1800);
        root=new Node2D();AddChild(root);owner=New(root,"AU-20",ChangedCatalog(d=>d["events"]!.AsArray()
            .First(e=>Eq(e!["category"]!.GetValue<string>(),"AU-20"))!["routes"]!.AsArray()
            .First(r=>Eq(r!["id"]!.GetValue<string>(),"warning.day-night-retaliation-assault.day"))!["trigger"]=longText));
        var daySelector=Panel(root).GetNode<OptionButton>("Event");
        int day=Enumerable.Range(0,daySelector.ItemCount).Single(i=>Eq(daySelector.GetItemText(i),
            "warning.day-night-retaliation-assault.day"));
        Select(root,day);
        var text=Panel(root).GetNode<TextEdit>("Details");
        Check("AUDIO-AUDITION.long-text",text.Text.Contains(longText,StringComparison.Ordinal)
            &&text.WrapMode==TextEdit.LineWrappingMode.Boundary&&!text.Editable,text.Text);
        Check("AUDIO-AUDITION.zero-one-many",Panel(root).GetNode<OptionButton>("Event").ItemCount==5,5);
        Call(owner,"Close");root.Free();
        await Frames();
    }
    private async Task Hub()
    {
        // The helper project compiles only its bridge. Stage the exact production script resource afterwards,
        // then construct its already-loaded production type so Godot registers its existing script identity.
        System.IO.File.WriteAllBytes(ProjectSettings.GlobalizePath("res://inspection/InspectionRoot.cs"),
            Convert.FromBase64String("AUDIO_ROOT_SOURCE_BASE64"));
        var registration=(Node)Activator.CreateInstance(_type.Assembly.GetType(
            "KingdomZombies.Inspection.InspectionRoot",true)!)!;
        registration.Free();
        var scene=ResourceLoader.Load<PackedScene>("res://inspection/Inspection.tscn");var hub=scene.Instantiate<Node>();
        AddChild(hub);var status=hub.GetNode<Label>("Summary/Status");
        Check("AUDIO-AUDITION.loading",status.Text.StartsWith("Loading",StringComparison.Ordinal),status.Text);
        for(int i=0;i<500&&status.Text.StartsWith("Loading",StringComparison.Ordinal);i++)await Frames();
        var category=hub.GetNode<OptionButton>("Shell/Workspace/Navigation/Category");category.Select(5);
        category.EmitSignal(OptionButton.SignalName.ItemSelected,5L);
        var list=hub.GetNode<ItemList>("Shell/Workspace/Controls/Browser/Resources");
        int selected=-1;for(int i=0;i<list.ItemCount;i++)if(list.GetItemText(i).StartsWith("AU-27 ·",StringComparison.Ordinal))selected=i;
        Check("hub-all30-reachable",list.ItemCount==30&&selected>=0,list.ItemCount);
        list.Select(selected);list.EmitSignal(ItemList.SignalName.ItemSelected,(long)selected);
        Check("hub-native-audio-page",hub.HasNode("Shell/Workspace/Preview/Surface/Current/AudioPreview/Audio/Content"),status.Text);
        RemoveChild(hub);hub.Free();scene.Dispose();await Frames();
    }
}
`;
function audioPreviewFiles(){
 const foundation=[];
 function walk(directory){for(const entry of fs.readdirSync(inside(ROOT,directory),{withFileTypes:true})){
  const file=directory+'/'+entry.name;if(entry.isDirectory())walk(file);
  else if(entry.isFile()&&!file.endsWith('.uid')&&!file.endsWith('.import'))foundation.push(file);
 }}
 walk('src/godot-game/foundation');
 return [...new Set([...require('./world.cjs').inspectorFiles(),...foundation,'src/godot-game/icon.svg',
  'src/godot-game/inspection/AudioPreview.cs','tests/phase1/AudioPresentationTests.cs',AUDIO_FIXTURE,
  'tools/phase4/cases/audio.cjs'])];
}
const AUDIO_HUB_DRIVER=String.raw`extends SceneTree
var outcomes = []
var failed = false
func _initialize():
 call_deferred("run")
func check_case(id, passed, detail):
 outcomes.append({"id":id,"pass":passed,"detail":detail})
 failed = failed or not passed
func run():
 var scene = load("res://inspection/Inspection.tscn")
 var hub = scene.instantiate()
 root.add_child(hub)
 var status = hub.get_node("Summary/Status")
 check_case("AUDIO-AUDITION.loading",status.text.begins_with("Loading"),status.text)
 for i in range(500):
  if not status.text.begins_with("Loading"):
   break
  await process_frame
  await process_frame
 var category = hub.get_node("Shell/Workspace/Navigation/Category")
 category.select(5)
 category.item_selected.emit(5)
 var list = hub.get_node("Shell/Workspace/Controls/Browser/Resources")
 var selected = -1
 for i in range(list.item_count):
  if list.get_item_text(i).begins_with("AU-27 ·"):
   selected = i
 check_case("hub-all30-reachable",list.item_count==30 and selected>=0,list.item_count)
 list.select(selected)
 list.item_selected.emit(selected)
 check_case("hub-native-audio-page",hub.has_node("Shell/Workspace/Preview/Surface/Current/AudioPreview/Audio/Content"),status.text)
 root.remove_child(hub)
 hub.free()
 await process_frame
 var result = {"pid":OS.get_process_id(),"nonce":OS.get_environment("AUDIO_HUB_NONCE"),"outcomes":outcomes,"failed":failed}
 var report = FileAccess.open("res://audio-hub-results.json",FileAccess.WRITE)
 report.store_string(JSON.stringify(result))
 report.close()
 print("AUDIO_HUB_COMPLETE "+result.nonce+" "+("FAIL" if failed else "PASS"))
 quit(1 if failed else 0)
`;
async function audioHub(context,sourceInputs){
 const project=inside(context.directory,'native-audio-hub');fs.mkdirSync(project);
 for(const file of ['project.godot','inspection','foundation','icon.svg','content','assets','.godot/mono/temp/bin/Debug'])
  fs.cpSync(inside(ROOT,'src/godot-game/'+file),inside(project,file),{recursive:true});
 const binaries=inside(project,'.godot/mono/temp/bin/Debug');
 const closure=fs.readdirSync(binaries).filter(file=>fs.statSync(path.join(binaries,file)).isFile())
  .map(file=>({file,sha256:hash(fs.readFileSync(path.join(binaries,file)))}));
 const controllerClosure=document(context.directory,'inventory-production-assemblies.json');
 need(equal(closure,controllerClosure),'AUDIO_HUB_CLOSURE_MISMATCH');
 write(context.directory,'audio-hub-production-assemblies.json',closure);
 write(project,'audio-hub-probe.gd',Buffer.from(AUDIO_HUB_DRIVER));
 const engine=document(context.directory,'inventory-engine.json');
 const pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'];
 need(hash(fs.readFileSync(inside(ROOT,'.phase1/cache/'+pin.file)))===pin.sha256
  &&engine.archiveSha256===pin.sha256&&hash(fs.readFileSync(engine.executable))===engine.sha256,'AUDIO_HUB_ENGINE_AUTH');
 write(context.directory,'audio-hub-engine.json',engine);
 const nonce=context.nonce+'.hub',env={...require('./defenses.cjs').isolated(),AUDIO_HUB_NONCE:nonce};
 const imported=await runner.command(context,'audio-hub-import',engine.executable,
  ['--headless','--path',project,'--editor','--import','--quit'],{env});
 need(!/ERROR:|SCRIPT ERROR:|FATAL/i.test(imported.stdout+imported.stderr),'AUDIO_HUB_IMPORT_CLEAN');
 const child=await runner.command(context,'audio-hub-native',engine.executable,
  ['--headless','--path',project,'--script','res://audio-hub-probe.gd','--quit-after','1600'],{env});
 const result=document(project,'audio-hub-results.json');
 need(child.stderr.trim()===''&&!/ERROR:|SCRIPT ERROR:|FATAL|leaked/i.test(child.stdout+child.stderr)
  &&child.stdout.includes('AUDIO_HUB_COMPLETE '+nonce+' PASS'),'AUDIO_HUB_NATIVE_CLEAN');
 need(result.pid===child.pid&&result.nonce===nonce&&!result.failed,'AUDIO_HUB_NATIVE_IDENTITY');
 need(equal(result.outcomes.map(row=>row.id),['AUDIO-AUDITION.loading','hub-all30-reachable','hub-native-audio-page'])
  &&result.outcomes.every(row=>row.pass),'AUDIO_HUB_REQUIRED_OUTCOMES');
 write(context.directory,'audio-hub-observation.json',{...result,group:'actual-project-hub',engine,
  productionAssemblies:closure,sourceInputs});
 return result;
}
function controllerOutcomeIds(){
 const catalog=document(ROOT,'src/godot-game/content/audio/events.json');
 need(catalog.events.length===30&&catalog.events.flatMap(row=>row.routes).length===91,'AUDIO_CURRENT_CATALOG_SCOPE');
 const ids=['actual-production-alc'];
 for(const category of catalog.events){
  ids.push(category.category+'-all-routes',category.category+'-selection-silent');
  for(const route of category.routes.map(row=>row.id).sort()){
   ids.push(...['actual-playing','exact-pcm','native-format','native-loop-boundary','exact-binding'].map(s=>route+'-'+s));
   if(route==='food.use-sale.consume')ids.push('generated-current-source-info');
   if(route==='bloater.splash.cosmetic-death-burst')ids.push('shared-current-source-info');
   ids.push(route+'-stopped-freed');
  }
 }
 ids.push(...Array.from({length:24},(_,i)=>'H19-admit-'+i),
  'H19-exact-global24','H19-exact-effect4','H19-priority-admitted','H19-lowered-limit-preserves-priority',
  'H19-low-priority-denied',...Array.from({length:19},(_,i)=>'H25-restore-'+i),
  'H26-close-frees-players','H26-global-owner-fence','H26-current-A-restored','H26-select-stops-A-silent-B',
  'H26-muted-warning','muted-immediate-repeat-cooldown','muted-actual-native-volume',
  ...['populated','overflow','empty','error','partial','long-text','zero-one-many'].map(s=>'AUDIO-AUDITION.'+s));
 return ids;
}
async function lifecycle(context){
 const files=audioPreviewFiles(),before=Object.fromEntries(files.map(file=>[file,hash(bytes(ROOT,file))]));
 write(context.directory,'audio-inputs.json',before);
 const env=require('./defenses.cjs').isolated();
 const build=await runner.command(context,'audio-strict-build','dotnet',['build',inside(ROOT,'KingdomZombies.slnx'),
  '--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env});
 need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'AUDIO_STRICT_BUILD');
 const compiled=inside(context.directory,'audio-compiled');fs.mkdirSync(compiled);
 await runner.command(context,'audio-compiled','dotnet',[inside(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),
  '--filter-class','*AudioPresentationTests','--report-xunit-junit','--report-xunit-junit-filename','tests.xml',
  '--results-directory',compiled,'--no-ansi','--progress','off','--parallel','none','--timeout','45s'],{env});
 const paths=files.filter(file=>file.startsWith('src/godot-game/')&&!file.endsWith('.cs')
  &&file!=='src/godot-game/project.godot'&&file!==AUDIO_FIXTURE).map(file=>file.slice(15));
 // Keep the original hub predicates and their final order in a separately qualified actual-project process.
 const harness=AUDIO_HARNESS.replace('        await Hub();','').replace('AUDIO_ROOT_SOURCE_BASE64',
  bytes(ROOT,'src/godot-game/inspection/InspectionRoot.cs').toString('base64'));
 const native=await require('./ui-inventory.cjs').native(context,{pack:AUDIO_FIXTURE,paths,harness});
 const controller=document(context.directory,'inventory-native-observation.json');
 need(equal(controller.outcomes.map(row=>row.id),controllerOutcomeIds()),'AUDIO_CONTROLLER_REQUIRED_ORDERED_OUTCOMES');
 const controllerLog=fs.readFileSync(inside(context.directory,'inventory-native.stdout.log'),'utf8')
  +fs.readFileSync(inside(context.directory,'inventory-native.stderr.log'),'utf8');
 need(!/ERROR:|SCRIPT ERROR:|FATAL|leaked/i.test(controllerLog),'AUDIO_CONTROLLER_CLEAN');
 const hub=await audioHub(context,before);
 const qualify=(result,group)=>result.outcomes.map(row=>({...row,group,pid:result.pid,nonce:result.nonce}));
 const report={groups:[{id:'controller',pid:controller.pid,nonce:controller.nonce,sourceInputs:before,
  productionAssemblies:document(context.directory,'inventory-production-assemblies.json'),
  engine:document(context.directory,'inventory-engine.json')},{id:'actual-project-hub',pid:hub.pid,nonce:hub.nonce,
  sourceInputs:before,productionAssemblies:document(context.directory,'audio-hub-production-assemblies.json'),
  engine:document(context.directory,'audio-hub-engine.json')}],
  outcomes:[...qualify(controller,'controller'),...qualify(hub,'actual-project-hub')]};
 need(new Set(report.outcomes.map(row=>row.id)).size===report.outcomes.length
  &&report.outcomes.every(row=>row.pass),'AUDIO_NATIVE_DUPLICATE_OR_FAILED_OUTCOME');
 write(context.directory,'audio-native-observation.json',report);
 for(const state of ['empty','loading','error','populated','partial','overflow','zero-one-many','long-text'])
  need(report.outcomes.filter(row=>row.id==='AUDIO-AUDITION.'+state&&row.pass).length===1,'AUDIO_BACKSTOP '+state);
 for(const file of files)need(hash(bytes(ROOT,file))===before[file],'AUDIO_SOURCE_DRIFT '+file);
 return {native,hub:{pid:hub.pid,nonce:hub.nonce,outcomes:hub.outcomes.length},
  scope:'Two separately qualified source/headless processes; no listening/native mix approval'};
}
runner.registerCase('audio.lifecycle',{group:'audio',title:'Native Audio controls exact PCM and current loop ownership',
 importance:'Critical',rationale:'Stale native players or historical cue replay breaks current inspection state',
 needsGameWindow:false,runGroup:'G0-audio-native',order:3,requirements:['ASSET-01','ASSET-02','ASSET-04'],
 acceptance:['AC-12','AC-14','AC-19'],setup:'Fresh strict production assemblies and authenticated headless native engine',
 actions:['Inspect all30categories and91routes through actual players','Compare exact native PCM and integer loop boundaries',
  'Restore current loops19times without historical events','Retire A then reject its stale callback in replacement A',
  'Observe all eight independently named AUDIO-AUDITION backstops and real hub loading/navigation'],
 expected:'Every independent native assertion passes; hearing and final native mix remain04-32/33',
 reset:'Close acceptance then stop/free all owned players and streams; retire native controls and copied project',
 evidence:['audio-inputs.json','audio-compiled/tests.xml','inventory-production-assemblies.json','inventory-engine.json',
  'inventory-native-observation.json','result.json']},lifecycle);
Object.assign(module.exports,{AUDIO_HARNESS,audioPreviewFiles,lifecycle});
