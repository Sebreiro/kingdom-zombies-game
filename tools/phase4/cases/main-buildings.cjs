'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const {PYTHON,engine}=require('./items.cjs');
const ART_FILES=['src/godot-game/assets/buildings/main-tiers.png','src/godot-game/content/packs/main-buildings.json','production/phase4/main-buildings.zip','tools/phase4/cases/main-buildings.cjs'];
function fingerprint(files){return files.map(file=>({file,sha256:hash(bytes(ROOT,file,67108864))}));}
const VERIFY=String.raw`import sys,pathlib,json,zipfile,io,hashlib,copy,importlib.util
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
data=m.read_archive(root/'production/phase4/main-buildings.zip');pack=m.loads(data['main-buildings.json']);atlas=Image.open(io.BytesIO(data['main-tiers.png'])).convert('RGBA')
m.need(data['main-buildings.json']==(root/'src/godot-game/content/packs/main-buildings.json').read_bytes(),'BUILDING_PACK_BYTES');m.need(data['main-tiers.png']==(root/'src/godot-game/assets/buildings/main-tiers.png').read_bytes(),'BUILDING_ATLAS_BYTES')
inventory=m.loads(data['members.json']);m.need(len(inventory)+1==len(data) and len({x['path'] for x in inventory})==len(inventory),'BUILDING_INVENTORY')
for row in inventory:m.need(row['path'] in data and row['bytes']==len(data[row['path']]) and row['sha256']==m.sha(data[row['path']]),'BUILDING_MEMBER_HASH')
def binary(im):
 im=im.convert('RGBA');im.putalpha(im.getchannel('A').point(lambda a:255 if a>=128 else 0));im.putdata([(r,g,b,a) if a else (0,0,0,0) for r,g,b,a in im.get_flattened_data()]);return im
def replay(part,d):
 m.need(part['origin'] in d,'BUILDING_ORIGIN');im=Image.open(io.BytesIO(d[part['origin']])).convert('RGBA')
 for op in part['operations']:
  if op['op']=='alpha-threshold':m.need(op['value']==128,'BUILDING_THRESHOLD');im=binary(im)
  elif op['op']=='crop':
   x,y,r,b=op['box'];m.need(0<=x<r<=im.width and 0<=y<b<=im.height,'BUILDING_CROP');im=im.crop((x,y,r,b))
  elif op['op']=='nearest':m.need(len(op['size'])==2 and all(type(x)==int and 0<x<=256 for x in op['size']),'BUILDING_NEAREST');im=im.resize(op['size'],Image.Resampling.NEAREST)
  elif op['op']=='damage':
   m.need(len(op['cuts'])<=8 and len(op['bends'])<=2,'BUILDING_DAMAGE_LIMIT')
   for box in op['cuts']:m.need(0<=box[0]<box[2]<=im.width and 0<=box[1]<box[3]<=im.height,'BUILDING_DAMAGE_BOUNDS');im.paste((0,0,0,0),box)
   for box,at in op['bends']:patch=im.crop(box);im.paste((0,0,0,0),box);im.alpha_composite(patch,at)
  elif op['op']=='rubble':
   original=im;m.need(op['size']==[im.width,16] and len(op['pieces'])==3,'BUILDING_RUBBLE');im=Image.new('RGBA',tuple(op['size']))
   for box,at in op['pieces']:im.alpha_composite(original.crop(box),at)
  else:raise ValueError('BUILDING_OPERATION')
 return im
def verify(p,d):
 m.need(p['id']=='main-buildings' and p['schemaVersion']==1 and p['version']==1,'BUILDING_ID')
 m.need(p['logicalPixelsPerWorldPixel']==1 and 'D-13 LOW-RESOLUTION OVERRIDES' in p['stylePrecedence'],'BUILDING_STYLE')
 m.need(p['acceptance']=={'visual':'pending','owner':'04-32/04-33','humanApproval':None,'completedObligations':[]},'BUILDING_ACCEPTANCE')
 m.need(p['commonBundleSha256']==m.digest(root/'production/phase4/common-selected.zip') and p['pilotBundleSha256']==m.digest(root/'production/phase4/scale-pilot.zip'),'BUILDING_LINEAGE')
 m.need(p['atlas']['sha256']==m.sha(d['main-tiers.png']) and p['atlas']['size']==list(atlas.size),'BUILDING_ATLAS')
 m.need([x['tier'] for x in p['tiers']]==[1,2,3] and [x['id'] for x in p['tiers']]==['BL-01','BL-02','BL-03'],'BUILDING_TIERS')
 m.need([x['material'] for x in p['tiers']]==['campfire/rough-boxes','wood','metal-reinforced-wood'],'BUILDING_MATERIALS')
 m.need(len(p['parts'])==47 and len({x['key'] for x in p['parts']})==47,'BUILDING_PARTS');parts={x['key']:x for x in p['parts']};images={}
 for part in p['parts']:
  m.need(part['member'] in d and part['sha256']==m.sha(d[part['member']]),'BUILDING_PART_HASH');im=Image.open(io.BytesIO(d[part['member']])).convert('RGBA')
  m.need(im.getbbox() and part['density']==1 and part['worldSize']==list(im.size) and part['image']['rgbaSha256']==m.sha(im.tobytes()),'BUILDING_PART_IMAGE')
  m.need(set(im.getchannel('A').tobytes())<={0,255} and all(a or (r,g,b)==(0,0,0) for r,g,b,a in im.get_flattened_data()),'BUILDING_ALPHA')
  x,y,w,h=part['atlasRect'];m.need([w,h]==list(im.size) and 0<=x<=atlas.width-w and 0<=y<=atlas.height-h and atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),'BUILDING_ATLAS_PIXELS')
  m.need(replay(part,d).tobytes()==im.tobytes(),'BUILDING_REPLAY');images[part['key']]=im
 for tier in p['tiers']:
  m.need(tier['bakedGuardCount']==0 and tier['repairOverlay']=='scaffold' and tier['upgradeOverlay']=='scaffold','BUILDING_COMPONENTS')
  m.need(list(tier['states'])==['intact','damage-light','damage-heavy','defeated'] and all(k in parts for k in tier['states'].values()),'BUILDING_STATES')
  material=['boxes','wood','metal'][tier['tier']-1];m.need(tier['sourceComposition']['facade']==material,'BUILDING_SILHOUETTE')
  for state,key in tier['states'].items():
   canvas=Image.new('RGBA',(160,88));asset=material if state=='intact' else material+'-'+('rubble' if state=='defeated' else state);pos=(4,68) if tier['tier']==1 else (16,16)
   if state=='defeated':pos=(16,72)
   canvas.alpha_composite(images[asset],pos)
   if state=='defeated':canvas.alpha_composite(images['wood-fragment'],(65,65))
   else:
    canvas.alpha_composite(images['fire'],(63,72));canvas.alpha_composite(images['chest-closed'],(130,68))
    if tier['tier']>1:canvas.alpha_composite(images['boxes'],(2,68))
   m.need(canvas.tobytes()==images[key].tobytes(),'BUILDING_COMPOSITION')
  a=images[material];b=images[material+'-damage-light'];c=images[material+'-damage-heavy'];m.need(a.tobytes()!=b.tobytes()!=c.tobytes() and sum(bool(x) for x in a.getchannel('A').tobytes())>sum(bool(x) for x in b.getchannel('A').tobytes())>sum(bool(x) for x in c.getchannel('A').tobytes()),'BUILDING_DAMAGE_PROGRESSION')
  m.need(tier['fragments']==['wood-fragment']+(['metal-fragment'] if tier['tier']==3 else []),'BUILDING_FRAGMENTS')
 m.need(p['recipe']['sha256']==m.sha(d['recipes/build-main.py']),'BUILDING_RECIPE')
 for generated in p['sources']['generated']:
  m.need(generated['tool']=='built-in image_gen' and generated['master'] in d and generated['sha256']==m.sha(d[generated['master']]) and generated['promptSha256']==m.sha(d[generated['prompt']]),'BUILDING_GENERATION')
  m.need(b'D-13 LOW-RESOLUTION' in d[generated['prompt']] and b'transparent' in d[generated['prompt']],'BUILDING_PROMPT')
  original=Image.open(io.BytesIO(d[generated['master']])).convert('RGBA');m.need(original.getchannel('A').getextrema()==(0,255),'BUILDING_MASTER_ALPHA')
 guard=m.loads(d['sources/guard-shoot.json']);m.need(guard['id']=='common.clip.783' and [x['time'] for x in guard['layers'][0]['keys']]==[0,0.125,0.25,0.375,0.5] and guard['stop']==0.625,'BUILDING_GUARD_TIMING')
 return {'parts':len(parts),'tiers':3,'members':len(d),'guardFrames':len(guard['mapping'])}
result=verify(pack,data);controls=[]
def reject(name,mutate,code):
 candidate=copy.deepcopy(pack);d=dict(data);mutate(candidate,d)
 try:verify(candidate,d)
 except (ValueError,KeyError,AssertionError) as error:m.need(code in str(error),'BUILDING_CONTROL_REASON '+name+' '+str(error));controls.append({'id':name,'rejected':True,'diagnostic':str(error)})
 else:raise ValueError('BUILDING_CONTROL_FALSE_PASS '+name)
reject('first-tier-house',lambda p,d:p['tiers'][0].update(material='wood'),'BUILDING_MATERIALS')
reject('third-tier-stone',lambda p,d:p['tiers'][2].update(material='stone'),'BUILDING_MATERIALS')
reject('baked-guard',lambda p,d:p['tiers'][2].update(bakedGuardCount=2),'BUILDING_COMPONENTS')
reject('missing-defeat',lambda p,d:p['tiers'][1]['states'].pop('defeated'),'BUILDING_STATES')
reject('missing-metal-fragment',lambda p,d:p['tiers'][2].update(fragments=['wood-fragment']),'BUILDING_FRAGMENTS')
reject('premature-house-silhouette',lambda p,d:p['tiers'][0]['sourceComposition'].update(facade='wood'),'BUILDING_SILHOUETTE')
reject('missing-master',lambda p,d:d.pop('masters/metal.png'),'BUILDING_ORIGIN')
reject('fake-generation',lambda p,d:p['sources']['generated'][0].update(tool='programmatic stand-in'),'BUILDING_GENERATION')
reject('bad-density',lambda p,d:p.update(logicalPixelsPerWorldPixel=2),'BUILDING_STYLE')
reject('false-approval',lambda p,d:p['acceptance'].update(visual='approved'),'BUILDING_ACCEPTANCE')
reject('atlas-shift',lambda p,d:p['parts'][0]['atlasRect'].__setitem__(0,p['parts'][0]['atlasRect'][0]+1),'BUILDING_ATLAS_PIXELS')
reject('smoothing-operation',lambda p,d:p['parts'][0]['operations'][-1].update(op='bilinear'),'BUILDING_OPERATION')
print(json.dumps({**result,'expectedRejected':len(controls),'controls':controls}))
`;
async function art(context){
 const before=fingerprint(ART_FILES);write(context.directory,'inputs.json',before);write(context.directory,'verify-buildings.py',Buffer.from(VERIFY));
 const child=await runner.command(context,'buildings-art',PYTHON,['-B',inside(context.directory,'verify-buildings.py'),ROOT]);const result=JSON.parse(child.stdout);
 need(result.tiers===3&&result.parts===47&&result.expectedRejected===12,'BUILDING_ART_RESULT');write(context.directory,'negative-controls.json',result.controls);
 need(equal(before,fingerprint(ART_FILES)),'BUILDING_SOURCE_CHANGED');return {...result,scope:'Source art/provenance only; guarded native state in buildings.main-state, hub adoption04-23, actual appearance04-32/33 pending'};
}
runner.registerCase('buildings.main-art',{group:'buildings',title:'Three actual campfire, wooden and metal main tiers with retained lifecycle art',importance:'Critical',
 rationale:'Wrong main silhouettes, missing lifecycle material or false source lineage invalidates camp resource production',needsGameWindow:false,runGroup:'G0',order:24,
 requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-03','AC-08'],setup:'Tracked portable main-building ZIP/atlas/shard, installed pinned Python/Pillow; recovered roots are unnecessary',
 actions:['Authenticate exact bundle/member/project bytes','Reproduce47 actual material/lifecycle/guard inputs from retained selected/generated art','Independently reconstruct three main silhouettes and12 lifecycle compositions','Reject12 independent semantic faults'],
 expected:'Three exact materials, no baked guards, binary alpha, localized increasing damage, reusable repair/upgrade/ruin inputs and12 rejected controls',reset:'Fresh owned immutable source invocation; no real game window',
 evidence:['inputs.json','buildings-art.command.json','negative-controls.json','result.json']},art);
module.exports={ART_FILES,VERIFY,fingerprint,art};
