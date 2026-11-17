'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
const ART_FILES=['src/godot-game/assets/buildings/defenses.png','src/godot-game/content/packs/defenses.json','production/phase4/defenses.zip','tools/phase4/cases/defenses.cjs'];
const ART_INPUTS=[...ART_FILES,'production/phase4/common-selected.zip','production/phase4/main-buildings.zip','src/godot-game/content/packs/common.json','src/godot-game/content/packs/main-buildings.json'];
function fingerprint(files){return files.map(file=>({file,sha256:hash(bytes(ROOT,file))}));}
const VERIFY=String.raw`import sys,pathlib,json,io,hashlib,copy,importlib.util
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
d=m.read_archive(root/'production/phase4/defenses.zip');p=m.loads(d['defenses.json']);atlas=Image.open(io.BytesIO(d['defenses.png'])).convert('RGBA')
m.need(d['defenses.json']==(root/'src/godot-game/content/packs/defenses.json').read_bytes(),'DEFENSE_PACK_BYTES');m.need(d['defenses.png']==(root/'src/godot-game/assets/buildings/defenses.png').read_bytes(),'DEFENSE_ATLAS_BYTES')
inventory=m.loads(d['members.json']);m.need(len(inventory)+1==len(d) and len({r['path'] for r in inventory})==len(inventory),'DEFENSE_INVENTORY')
for r in inventory:m.need(r['path'] in d and r['bytes']==len(d[r['path']]) and r['sha256']==m.sha(d[r['path']]),'DEFENSE_MEMBER_HASH')
def verify(p,d):
 m.need(p['schemaVersion']==1 and p['version']==1 and p['id']=='defenses','DEFENSE_ID')
 m.need(p['logicalPixelsPerWorldPixel']==1 and 'D-13 LOW-RESOLUTION OVERRIDES' in p['stylePrecedence'],'DEFENSE_DENSITY')
 m.need(p['acceptance']=={'visual':'pending','owner':'04-32/04-33','humanApproval':None,'completedObligations':[]},'DEFENSE_FALSE_ACCEPTANCE')
 m.need(p['atlas']['path']=='res://assets/buildings/defenses.png' and p['atlas']['size']==[1024,512] and p['atlas']['sha256']==m.sha(d['defenses.png']) and p['atlas']['rgbaSha256']==m.sha(atlas.tobytes()),'DEFENSE_ATLAS')
 expected=[('BL-04','wall',1,'wood'),('BL-05','wall',2,'reinforced-wood'),('BL-06','wall',3,'metal-reinforced'),('BL-07','tower',1,'wood'),('BL-08','tower',2,'reinforced-wood'),('BL-09','tower',3,'metal-reinforced')]
 m.need([(r['id'],r['kind'],r['tier'],r['material']) for r in p['tiers']]==expected,'DEFENSE_TIERS')
 m.need(len(p['parts'])==46 and len({r['key'] for r in p['parts']})==46,'DEFENSE_PARTS');parts={r['key']:r for r in p['parts']};images={}
 for part in p['parts']:
  m.need(part['origin'] in d and part['member'] in d and part['sha256']==m.sha(d[part['member']]),'DEFENSE_ORIGIN')
  im=Image.open(io.BytesIO(d[part['member']])).convert('RGBA');images[part['key']]=im
  m.need(im.getbbox() and part['density']==1 and part['worldSize']==list(im.size) and part['rgbaSha256']==m.sha(im.tobytes()) and part['visiblePixels']==sum(bool(a) for a in im.getchannel('A').tobytes()),'DEFENSE_IMAGE')
  m.need(set(im.getchannel('A').tobytes())<={0,255} and all(a or (r,g,b)==(0,0,0) for r,g,b,a in im.get_flattened_data()),'DEFENSE_ALPHA')
  x,y,w,h=part['atlasRect'];m.need([w,h]==list(im.size) and 0<=x<=1024-w and 0<=y<=512-h and atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),'DEFENSE_ATLAS_PIXELS')
 for r in p['tiers']:
  m.need(r['bakedGuardCount']==0 and r['previousTierRuin'] is True and list(r['states'])==['intact','damage-light','damage-heavy','destroyed'] and all(k in parts for k in r['states'].values()),'DEFENSE_LIFECYCLE')
  key=r['kind']+'-'+str(r['tier']);m.need(r['states']['intact']==key and r['states']['destroyed']==key+'-ruin' and r['footprint']==list(images[key].size),'DEFENSE_PREVIOUS_TIER')
  base=images[key];light=images[key+'-damage-light'];heavy=images[key+'-damage-heavy'];m.need(base.tobytes()!=light.tobytes()!=heavy.tobytes(),'DEFENSE_DAMAGE')
  retained=lambda other:sum(a==b and a[3]>0 for a,b in zip(base.get_flattened_data(),other.get_flattened_data()))/sum(bool(a) for a in base.getchannel('A').tobytes())
  m.need(retained(light)>retained(heavy)>0.70,'DEFENSE_LOCALIZED_DAMAGE')
  m.need(r['fragments']==['metal-fragment' if r['tier']==3 else 'wood-fragment'],'DEFENSE_MATCHED_RUBBLE')
  m.need(images[key+'-ruin'].getbbox() and images[key+'-ruin'].tobytes()!=base.tobytes(),'DEFENSE_RUIN')
 expectedFrames=[('main-'+str(i),[160,88]) for i in range(1,4)]+[(r['kind']+'-'+str(r['tier']),r['footprint']) for r in p['tiers']]
 m.need([(r['structure'],r['footprint']) for r in p['frames']]==expectedFrames,'DEFENSE_NINE_FOOTPRINTS')
 for r in p['frames']:
  a=images[r['frame']];b=images[r['damagedFrame']];m.need(list(a.size)==r['footprint'] and b.size==a.size and a.tobytes()!=b.tobytes() and r['tileScale']==1,'DEFENSE_FRAME')
  coverage=sum(bool(a) for a in a.getchannel('A').tobytes())/(a.width*a.height);m.need(coverage<=p['visibility']['workOverlayMaxCoverage']<=0.48,'DEFENSE_OVERLAY_READABILITY')
 m.need(p['siteStates']==['unbuilt','available','tree-blocked','portal-blocked','main-tier-blocked','unaffordable','new-frame','damaged-unfinished','intact','damage-light','damage-heavy','upgrade-paid','upgrade-complete','repair-paid','repair-complete','unsafe-waiting','no-worker-waiting','destroyed','rebuild-paid','rebuild-complete','restore-destroyed','restore-upgrade'],'DEFENSE_SITE_STATES')
 m.need(p['visibility']=={'intactAbove':0.75,'lightDamageAbove':0.4,'heavyDamageAbove':0,'newFrameMaxHealthFraction':0.25,'workOverlayMaxCoverage':0.48,'oldFunctionalTierUntilProgress':1,'rubblePreservesCompletedTier':True,'noGuardLoot':True,'frameTileScale':1},'DEFENSE_VISIBILITY')
 m.need(p['sources']['commonBundleSha256']==m.digest(root/'production/phase4/common-selected.zip') and p['sources']['mainBundleSha256']==m.digest(root/'production/phase4/main-buildings.zip'),'DEFENSE_LINEAGE')
 m.need(len(p['sources']['generated'])==3,'DEFENSE_GENERATED_COUNT')
 for r in p['sources']['generated']:
  m.need(r['tool']=='built-in image_gen' and r['master'] in d and r['sha256']==m.sha(d[r['master']]) and r['promptSha256']==m.sha(d[r['prompt']]),'DEFENSE_MASTER')
  master=Image.open(io.BytesIO(d[r['master']])).convert('RGBA');low,high=master.getchannel('A').getextrema();m.need(low==0 and high>=128 and b'D-13 LOW-RESOLUTION' in d[r['prompt']] and b'transparent' in d[r['prompt']],'DEFENSE_MASTER_ALPHA')
 m.need(p['recipe']['sha256']==m.sha(d['recipes/build-defenses.py']),'DEFENSE_RECIPE')
 return {'parts':46,'tiers':6,'frames':9,'members':len(d),'generatedMasters':3,'sourceSelected':p['sources']['selectedCount']}
result=verify(p,d)
# Re-execute exact retained recipe using only retained ZIP inputs; no recovery directory.
ns={'__name__':'retained_recipe'};exec(compile(d['recipes/build-defenses.py'],'retained-build-defenses.py','exec'),ns);replayed,outputs=ns['build'](d);m.need(replayed==p,'DEFENSE_REPLAY_PACK')
for n,b in outputs.items():m.need(b==d[n],'DEFENSE_REPLAY_BYTES '+n)
controls=[]
def reject(name,mutate,code):
 candidate=copy.deepcopy(p);data=dict(d);mutate(candidate,data)
 try:verify(candidate,data)
 except (KeyError,ValueError,AssertionError) as e:m.need(code in str(e),'DEFENSE_CONTROL_REASON '+name+' '+str(e));controls.append({'id':name,'rejected':True,'diagnostic':str(e)})
 else:raise ValueError('DEFENSE_CONTROL_FALSE_PASS '+name)
reject('stone-metal-substitute',lambda p,d:p['tiers'][2].update(material='stone'),'DEFENSE_TIERS')
reject('missing-wide-tower',lambda p,d:p['tiers'].pop(),'DEFENSE_TIERS')
reject('baked-guards',lambda p,d:p['tiers'][5].update(bakedGuardCount=3),'DEFENSE_LIFECYCLE')
reject('lost-previous-tier',lambda p,d:p['tiers'][1].update(previousTierRuin=False),'DEFENSE_LIFECYCLE')
reject('missing-ruin',lambda p,d:p['tiers'][2]['states'].pop('destroyed'),'DEFENSE_LIFECYCLE')
reject('wood-metal-rubble',lambda p,d:p['tiers'][5].update(fragments=['wood-fragment']),'DEFENSE_MATCHED_RUBBLE')
reject('one-arbitrary-frame',lambda p,d:p['frames'].__setitem__(0,{**p['frames'][0],'footprint':[32,16]}),'DEFENSE_NINE_FOOTPRINTS')
reject('scaled-frame',lambda p,d:p['frames'][5].update(tileScale=2),'DEFENSE_FRAME')
reject('missing-waiting',lambda p,d:p['siteStates'].remove('unsafe-waiting'),'DEFENSE_SITE_STATES')
reject('opaque-work',lambda p,d:p['visibility'].update(workOverlayMaxCoverage=1),'DEFENSE_OVERLAY_READABILITY')
reject('no-frame-damage',lambda p,d:p['frames'][3].update(damagedFrame=p['frames'][3]['frame']),'DEFENSE_FRAME')
reject('missing-master',lambda p,d:d.pop('masters/wall-3.png'),'DEFENSE_ORIGIN')
reject('fabricated-generation',lambda p,d:p['sources']['generated'][0].update(tool='code-generated stand-in'),'DEFENSE_MASTER')
reject('wrong-world-density',lambda p,d:p.update(logicalPixelsPerWorldPixel=2),'DEFENSE_DENSITY')
reject('false-approval',lambda p,d:p['acceptance'].update(visual='approved'),'DEFENSE_FALSE_ACCEPTANCE')
reject('shifted-atlas',lambda p,d:p['parts'][2]['atlasRect'].__setitem__(0,p['parts'][2]['atlasRect'][0]+1),'DEFENSE_ATLAS_PIXELS')
print(json.dumps({**result,'replayedParts':len(outputs)-1,'expectedRejected':len(controls),'controls':controls}))
`;
async function art(context){
 const before=fingerprint(ART_INPUTS);write(context.directory,'inputs.json',before);write(context.directory,'verify-defenses.py',Buffer.from(VERIFY));
 const child=await runner.command(context,'defenses-art',PYTHON,['-B',inside(context.directory,'verify-defenses.py'),ROOT]);const result=JSON.parse(child.stdout);
 need(result.tiers===6&&result.frames===9&&result.parts===46&&result.expectedRejected===16&&result.replayedParts===46,'DEFENSE_ART_RESULT');write(context.directory,'negative-controls.json',result.controls);
 need(equal(before,fingerprint(ART_INPUTS)),'DEFENSE_SOURCE_CHANGED');return {...result,scope:'Source art/replay only; native defense states are buildings.defense-state; full hub/index owner04-23; actual named appearance/input pending04-32/33'};
}
runner.registerCase('buildings.defense-art',{group:'buildings',title:'Six material-correct defense tiers, nine fitted work frames and persistent lifecycle source art',importance:'Critical',
 rationale:'Missing defense materials, previous tiers or traceable lifecycle art invalidates required production',needsGameWindow:false,runGroup:'G0-defenses',order:1,
 requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-03','AC-08','AC-09','AC-24'],setup:'Tracked defenses ZIP/atlas/shard and pinned existing Python/Pillow; no donor folder or game window',
 actions:['Authenticate complete retained members and selected donor lineage','Replay46 actual art parts from retained3 generated masters and selected source pixels','Independently validate six tiers, localized damage, matched ruins and nine exact frames','Reject16 material/lifecycle/source/alpha/frame/acceptance faults'],
 expected:'Six actual wood/reinforced-wood/metal tiers and BL10–12 components pass exact source replay and all16 independent controls',reset:'Fresh immutable owned source invocation; no real game window',
 evidence:['inputs.json','defenses-art.command.json','negative-controls.json','result.json']},art);
module.exports={ART_FILES,ART_INPUTS,VERIFY,fingerprint,art};
