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
(root/'audio-actions.json').write_text(json.dumps(pack,indent=2,allow_nan=False)+'\n',encoding='utf-8')
print(json.dumps({'candidates':len(config['candidates']),'variants':len(samples),'frames':len(bank)//2,'zipBytes':(root/'audio-actions.zip').stat().st_size}))
`;
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
  need(pack.listening.status==='Not auditioned'&&pack.listening.listener===null&&pack.listening.verdicts.length===0,'AUDIO_FALSE_AUDITION');
  const audio=bank.subarray(44),intervals=[];
  for(const sample of pack.samples){
    const anchor=CANDIDATES.find(x=>x[0]===sample.candidateId);
    need(anchor&&sample.candidateName===anchor[1]&&sample.source.owner==='resources.assets'&&sample.source.type==='AudioClip'&&
      sample.source.objectId===anchor[0]&&sample.source.name===anchor[1],'AUDIO_SOURCE_IDENTITY',sample.id);
    need(sample.version===1&&sample.channels===2&&sample.rate===44100&&sample.sampleWidth===2,'AUDIO_SAMPLE_FORMAT',sample.id);
    need(sample.original.channels===sourceChannels(sample.candidateId)&&sample.original.rate===44100&&sample.original.sampleWidth===2&&
      sample.channelTransform===(sourceChannels(sample.candidateId)===1?'duplicate-mono-to-stereo':'preserve-stereo'),'AUDIO_SOURCE_FORMAT',sample.id);
    need(sample.listening.status==='Not auditioned'&&sample.listening.listener===null&&sample.listening.verdict===null,'AUDIO_FALSE_AUDITION',sample.id);
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
    need(actual.status==='Not auditioned'&&actual.suitability==='Unknown until actual hearing','AUDIO_FALSE_AUDITION',actual.id);
  }
  need(new Set(pack.samples.filter(x=>x.id.endsWith('.full')).map(x=>x.pcmSha256)).size===40,'AUDIO_FALSE_REUSE');
  return {categories:15,candidates:40,variants:44,frames:format.frames,seconds:format.frames/44100,clippedOutputSamples:0,listening:'Not auditioned'};
}
const ZIP_CHECK=String.raw`import array, hashlib, io, json, pathlib, sys, wave, zipfile
root=pathlib.Path(sys.argv[1]); pack=json.loads((root/'src/godot-game/content/packs/audio-actions.json').read_text())
sha=lambda b:hashlib.sha256(b).hexdigest()
with zipfile.ZipFile(root/'production/phase4/audio-actions.zip') as z:
    expected={x['path']:x for x in pack['bundleMembers']}
    assert len(z.infolist())==len(expected)+1 and len(set(z.namelist()))==len(z.namelist()),'AUDIO_ZIP_COUNT'
    assert set(z.namelist())==set(expected)|{'review/manifest.json'},'AUDIO_ZIP_MEMBERS'
    for item in z.infolist():
        p=pathlib.PurePosixPath(item.filename)
        assert not p.is_absolute() and '..' not in p.parts and '\\' not in item.filename and ':' not in item.filename,'AUDIO_ZIP_PATH'
        assert 0<item.file_size<=32*1024*1024,'AUDIO_ZIP_BOUND'
    assert sum(x.file_size for x in z.infolist())<=128*1024*1024,'AUDIO_ZIP_TOTAL'
    for name,record in expected.items():
        value=z.read(name);assert len(value)==record['bytes'] and sha(value)==record['sha256'],'AUDIO_ZIP_HASH'
    retained=json.loads(z.read('review/manifest.json'))
    assert retained=={k:v for k,v in pack.items() if k!='bundleSha256'},'AUDIO_REBOUND_MANIFEST'
    assert sha(z.read('recipes/decode.py'))==pack['recipes']['decoderSha256'],'AUDIO_RECIPE_HASH'
    assert sha(z.read('recipes/assemble.py'))==pack['recipes']['assemblerSha256'],'AUDIO_RECIPE_HASH'
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
  need(hash(decoderRecipe())===pack.recipes.decoderSha256&&hash(ASSEMBLE)===pack.recipes.assemblerSha256,'AUDIO_RECIPE_IDENTITY');
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
    ['false-peak','AUDIO_TECHNICAL_METADATA',x=>{x.samples[0].technical.peak++;}]
  ]){
    const subject=structuredClone(pack);edit(subject);let failure;
    try{validate(subject,bank);}catch(error){failure=error.message;}
    need(failure?.split(' ')[0]===code,'AUDIO_NEGATIVE_CONTROL',name+' '+failure);controls.push({name,expectedDiagnostic:code,actual:failure});
    validate(pack,bank);
  }
  let truncated;try{validate(pack,bank.subarray(0,100));}catch(error){truncated=error.message;}
  need(truncated?.split(' ')[0]==='WAV_HEADER','AUDIO_TRUNCATION_CONTROL');controls.push({name:'truncated-bank',actual:truncated});
  need(equal(before,inputs()),'AUDIO_INPUT_CHANGED');write(context.directory,'negative-controls.json',controls);
  const complete={...result,bundle:JSON.parse(bundle.stdout),negativeControls:controls.length,restoredPositives:12,
    inputHashes:before,scope:'Technical integrity only. No hearing, suitability, selected recording or Task1 completion.'};
  write(context.directory,'technical.json',complete);return complete;
}
async function actions(context){
  await technical(context);
  throw new Error('AUDIO_LISTENING_REQUIRED AU01-15 exact source and adapted candidate versions are Not auditioned; Task1 remains incomplete');
}
runner.registerCase('audio.actions',{group:'audio',title:'AU01-15 exact donor source/PCM/region integrity and actual listening gate',importance:'Critical',
  rationale:'False audio identity, truncated samples or invented hearing invalidates required action content',needsGameWindow:false,runGroup:'G0',order:121,
  requirements:['ASSET-01','ASSET-02','ASSET-04'],acceptance:['AC-03','AC-12','AC-14','AC-19'],
  setup:'Tracked exact candidate bank, manifest and ZIP; existing pinned Python; actual capable source-only listener required for completion',
  actions:['Verify exact AU category contracts and forty distinct donor recordings','Verify integer regions, source/variant hashes and reproduce every trim/gain',
    'Check bounded retained ZIP and thirteen exact deliberate failures plus restored positives','Require actual listener evidence; Not auditioned blocks completion'],
  expected:'All technical controls pass; AUDIO_LISTENING_REQUIRED remains until actual hearing records are supplied and independently validated',
  reset:'Fresh owned evidence directory only; no game window, playback, donor writes or changes to production bytes',
  evidence:['inputs.json','retained-bundle.command.json','negative-controls.json','technical.json','result.json']},actions);
module.exports={CANDIDATES,COVERAGE,decoderRecipe,prepare,validate,technical,actions};
if(require.main===module){
  const command=process.argv[2];
  (command==='--prepare'?prepare():command==='--technical'?technical({directory:fs.mkdtempSync(inside(ROOT,'.phase4/audio-technical-'))}):
    Promise.reject(new Error('USAGE --prepare | --technical'))).then(value=>{if(value)console.log(JSON.stringify(value));}).catch(error=>{console.error(error.stack);process.exitCode=1;});
}
