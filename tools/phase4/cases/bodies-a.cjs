'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
function engine(){return require('./items.cjs').engine();}
const ART_FILES=['src/godot-game/assets/characters/bodies-a.png','src/godot-game/content/packs/bodies-a.json','production/phase4/bodies-a.zip','tools/phase4/cases/bodies-a.cjs'];
function fingerprint(files){return files.map(file=>({file,sha256:hash(bytes(ROOT,file,67108864))}));}
const VERIFY=String.raw`import pathlib,sys,io,json,copy,importlib.util
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);artifact=pathlib.Path(sys.argv[2]) if len(sys.argv)>2 else root
spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
data=m.read_archive(artifact/'production/phase4/bodies-a.zip');pack=m.loads(data['bodies-a.json']);atlas=Image.open(io.BytesIO(data['bodies-a.png'])).convert('RGBA')
m.need((artifact/'src/godot-game/assets/characters/bodies-a.png').read_bytes()==data['bodies-a.png'] and (artifact/'src/godot-game/content/packs/bodies-a.json').read_bytes()==data['bodies-a.json'],'BODY_PUBLICATION')
inventory=m.loads(data['members.json']);m.need(len(inventory)+1==len(data) and len({x['path'] for x in inventory})==len(inventory),'BODY_INVENTORY')
for row in inventory:m.need(row['path'] in data and row['sha256']==m.sha(data[row['path']]) and row['bytes']==len(data[row['path']]),'BODY_MEMBER_HASH')
replay_cache={};decode_cache={}
def largest(im):
 pixels=im.load();seen=set();best=[]
 for y in range(im.height):
  for x in range(im.width):
   if (x,y) in seen or pixels[x,y][3]==0:continue
   group=[];queue=[(x,y)];seen.add((x,y))
   while queue:
    a,b=queue.pop();group.append((a,b))
    for dy in [-1,0,1]:
     for dx in [-1,0,1]:
      c,d=a+dx,b+dy
      if 0<=c<im.width and 0<=d<im.height and (c,d) not in seen and pixels[c,d][3]:seen.add((c,d));queue.append((c,d))
   if len(group)>len(best):best=group
 out=Image.new('RGBA',im.size);target=out.load()
 for x,y in best:target[x,y]=pixels[x,y]
 return out
def replay(part,d):
 raw=d[part['origin']];origin=m.sha(raw)
 if origin not in decode_cache:decode_cache[origin]=Image.open(io.BytesIO(raw)).convert('RGBA')
 im=decode_cache[origin].copy();prefix=[]
 for op in part['operations']:
  prefix.append(op);cache_key=origin+'|'+json.dumps(prefix,sort_keys=True)
  if cache_key in replay_cache:im=replay_cache[cache_key].copy();continue
  if op['op']=='mask':
   m.need(op['alphaAtLeast']==240 and op['removeRedMatte']==[180,2,2],'BODY_MASK');im.putdata([(r,g,b,255) if a>=240 and not(r>180 and r>g*2 and r>b*2) else (0,0,0,0) for r,g,b,a in im.get_flattened_data()])
  elif op['op']=='crop':
   x,y,r,b=op['box'];m.need(0<=x<r<=im.width and 0<=y<b<=im.height,'BODY_CROP');im=im.crop(op['box'])
  elif op['op']=='nearest':m.need(all(type(x)==int and 0<x<=48 for x in op['size']),'BODY_SIZE');im=im.resize(op['size'],Image.Resampling.NEAREST)
  elif op['op']=='largest-component':m.need(op['connectivity']==8,'BODY_COMPONENT_MASK');im=largest(im)
  elif op['op']=='place':
   m.need(op['canvas']==[48,40] and 0<=op['at'][0]<=48-im.width and 0<=op['at'][1]<=40-im.height,'BODY_REGISTRATION');out=Image.new('RGBA',(48,40));out.alpha_composite(im,op['at']);im=out
  elif op['op']=='partition':
   x,y,r,b=op['bounds'];m.need(0<=x<r<=48 and 0<=y<b<=40,'BODY_PARTITION');out=Image.new('RGBA',(48,40));out.paste(im.crop(op['bounds']),(x,y));im=out
   for box in op.get('exclude',[]):im.paste((0,0,0,0),box)
  else:raise ValueError('BODY_OPERATION')
  replay_cache[cache_key]=im.copy()
 return im
poses=['idle','walk-near','walk-far','run','windup','strike','recover','aim','hit','collapse-a','collapse-b','corpse','sit','gather','eat','carry']
def verify(p,d):
 m.need(p['schemaVersion']==p['version']==1 and p['id']=='bodies-a' and p['logicalPixelsPerWorldPixel']==1 and p['scaleDecision']=='D-13-pilot-1' and 'D-13 LOW-RESOLUTION OVERRIDES' in p['stylePrecedence'],'BODY_ID_SCALE')
 m.need(p['acceptance']=={'visual':'pending','humanApproval':None,'owner':'04-32/04-33','completedObligations':[]},'BODY_FALSE_ACCEPTANCE')
 m.need(p['commonBundleSha256']==m.digest(root/'production/phase4/common-selected.zip') and p['pilotBundleSha256']==m.digest(root/'production/phase4/scale-pilot.zip'),'BODY_LINEAGE')
 m.need(p['recipe']['sha256']==m.sha(d[p['recipe']['member']]) and p['recipe']['Pillow']=='13.0.0','BODY_RECIPE')
 m.need(p['atlas']['size']==list(atlas.size) and p['atlas']['sha256']==m.sha(d['bodies-a.png']) and p['atlas']['path']=='res://assets/characters/bodies-a.png','BODY_ATLAS')
 m.need([x['id'] for x in p['bodies']]==['CH-01','CH-02','CH-03'] and len({x['silhouette'] for x in p['bodies']})==3,'BODY_ROSTER')
 for b in p['bodies']:
  m.need(b['tool']=='built-in image_gen' and b['masterSha256']==m.sha(d[b['master']]) and b['promptSha256']==m.sha(d[b['prompt']]) and b'D-13 LOW-RESOLUTION' in d[b['prompt']],'BODY_GENERATION')
 m.need(len(p['parts'])==193 and len({x['key'] for x in p['parts']})==193,'BODY_PARTS');parts={x['key']:x for x in p['parts']};images={}
 for part in p['parts']:
  m.need(part['origin'] in d and len(part['operations'])==(5 if part['key']=='CH-02.local-head' else 6),'BODY_SOURCE');im=Image.open(io.BytesIO(d[part['member']])).convert('RGBA');m.need(im.getbbox() and part['sha256']==m.sha(d[part['member']]) and part['rgbaSha256']==m.sha(im.tobytes()),'BODY_PIXELS')
  m.need(part['density']==1 and part['worldSize']==[48,40] and part['pivot']==[24,38] and set(im.getchannel('A').tobytes())<={0,255} and all(a or(r,g,b)==(0,0,0) for r,g,b,a in im.get_flattened_data()),'BODY_ALPHA_REGISTRATION')
  x,y,w,h=part['atlasRect'];m.need([w,h]==[48,40] and 0<=x<=atlas.width-w and 0<=y<=atlas.height-h and atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),'BODY_ATLAS_PIXELS')
  replayed=replay(part,d);m.need(im.tobytes()==replayed.tobytes(),'BODY_REPRODUCTION');images[part['key']]=im
 for b in p['bodies']:
  source=Image.open(io.BytesIO(d[b['master']])).convert('RGBA');m.need(source.getchannel('A').getextrema()==(0,255) and list(source.size)==b['sourceCanvas'],'BODY_ORIGINAL_ALPHA')
  m.need([x['id'] for x in b['poses']]==poses and b['bakedEquipment']==[] and b['logicalStandingHeight'] in [28,30],'BODY_POSES')
  for pose in b['poses']:
   m.need(pose['authored'] and pose['bakedEquipment']==[] and pose['persistent']==(pose['id']=='corpse') and pose['duration']==(0 if pose['persistent'] else .12),'BODY_AUTHORED')
   m.need(list(pose['parts'])==['full','head','upper','lower'] and all(k in parts for k in pose['parts'].values()),'BODY_COMPONENTS')
   combined=Image.new('RGBA',(48,40))
   for label in ['head','upper','lower']:combined.alpha_composite(images[pose['parts'][label]])
   m.need(combined.tobytes()==images[pose['parts']['full']].tobytes(),'BODY_COMPONENT_COVERAGE')
  m.need(len({images[b['id']+'.'+pose+'.full'].tobytes() for pose in poses})==16,'BODY_DUPLICATED_POSE')
 m.need(len({images[b['id']+'.idle.full'].getchannel('A').tobytes() for b in p['bodies']})==3,'BODY_RECOLORED_SILHOUETTE')
 m.need(len(p['sourceSuitability'])==57 and all(x['suitability'] for x in p['sourceSuitability']) and all(x in d for x in ['history/rejected-safety.json','history/rejected-covered-survivor.png','history/runner-original-clipped.png']),'BODY_SUITABILITY')
 for id in ['common.s0.2361','common.s0.7052','common.s0.10365','common.s0.7060','common.s0.7246','common.s0.10669']:
  row=m.loads(d['references/'+id+'.json']);m.need(row['source']['rawSha256']==m.sha(d['references/'+id+'.bin']) and row['pngSha256']==m.sha(d['references/'+id+'.png']),'BODY_DONOR_IDENTITY')
 m.need(p['headAdaptation']['sha256']==m.sha(d['adapted/local-shambler-head.png']) and 'CH-02.local-head' in parts,'BODY_LOCAL_ADAPTATION')
 return {'bodies':3,'poses':48,'parts':193,'members':len(d),'generatedSelected':3,'visualAcceptance':'pending04-32/33'}
result=verify(pack,data);rejections=[]
for name,code in [('missing-pose','BODY_POSES'),('baked-equipment','BODY_POSES'),('wrong-density','BODY_ID_SCALE'),('false-acceptance','BODY_FALSE_ACCEPTANCE'),('missing-master','BODY_GENERATION'),('missing-component','BODY_COMPONENTS'),('recolored-silhouette','BODY_ROSTER'),('altered-transform','BODY_MASK'),('wrong-source-hash','BODY_DONOR_IDENTITY')]:
 p=copy.deepcopy(pack);d=dict(data)
 if name=='missing-pose':p['bodies'][1]['poses'].pop()
 elif name=='baked-equipment':p['bodies'][0]['bakedEquipment']=['IT-11']
 elif name=='wrong-density':p['logicalPixelsPerWorldPixel']=2
 elif name=='false-acceptance':p['acceptance']['visual']='approved'
 elif name=='missing-master':d[p['bodies'][0]['master']]=b'changed'
 elif name=='missing-component':p['bodies'][0]['poses'][0]['parts']['head']='absent'
 elif name=='recolored-silhouette':p['bodies'][2]['silhouette']=p['bodies'][1]['silhouette']
 elif name=='altered-transform':p['parts'][0]['operations'][0]['alphaAtLeast']=128
 elif name=='wrong-source-hash':d['references/common.s0.2361.bin']=b'changed'
 try:verify(p,d);raise AssertionError('accepted '+name)
 except ValueError as e:m.need(str(e)==code,'BODY_CONTROL_DIAGNOSTIC '+name+' '+str(e));rejections.append({'id':name,'diagnostic':str(e)})
print(json.dumps({'actual':result,'negativeControls':rejections}))
`;
async function art(context){
 const before=fingerprint(ART_FILES);write(context.directory,'inputs.json',before);write(context.directory,'verify-art.py',Buffer.from(VERIFY));
 const child=await runner.command(context,'bodies-art',PYTHON,[inside(context.directory,'verify-art.py'),ROOT]);
 const result=JSON.parse(child.stdout.trim());need(result.actual.bodies===3&&result.actual.poses===48&&result.actual.parts===193&&result.negativeControls.length===9,'BODY_ART_RESULTS');
 need(equal(before,fingerprint(ART_FILES)),'BODY_ART_SOURCE_CHANGED');write(context.directory,'art-observation.json',result);return result;
}
runner.registerCase('bodies.a-art',{group:'characters',title:'Three source-reproducible neutral bodies and authored registered pose/component masters',importance:'Critical',rationale:'Missing or baked body poses prevent reusable character/equipment coverage',needsGameWindow:false,runGroup:'G0',order:26,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-05','AC-18'],setup:'Retained built-in generated originals, exact K14/K18/source head references, complete production atlas and strict bounded ZIP inventory',actions:['Reproduce193 parts from exact retained originals and masks/crops/nearest transforms','Assert all48 neutral poses, disjoint reusable component coverage and three distinct alpha silhouettes','Reject nine source/pose/baked/density/component/acceptance/transform faults'],expected:'Three actual neutral body families with true alpha and exact source identity; final appearance judgement remains04-32/33',reset:'Fresh immutable source-side invocation; no game window',evidence:['inputs.json','bodies-art.command.json','art-observation.json','result.json']},art);
module.exports={ART_FILES,VERIFY,fingerprint,art};
