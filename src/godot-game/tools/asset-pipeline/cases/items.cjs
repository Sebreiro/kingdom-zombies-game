'use strict';
const fs=require('node:fs'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,need,inside,bytes,document,write,hash,sharedFile}=require('../content.cjs');
const {PYTHON}=require('../python.cjs');
function engine(){
  const pins=document(ROOT,'src/godot-game/tools/verification/toolchain.json'),pin=pins.archives['win32-x64'];
  need(process.platform==='win32'&&process.arch==='x64'&&process.version==='v'+pins.node,'ITEM_PLATFORM_PIN');
  const executable=sharedFile('tools',pin.directory,pin.executable),stat=fs.statSync(executable);
  need(stat.isFile()&&stat.size>0&&stat.size<=268435456&&hash(fs.readFileSync(executable))===pin.executableSha256,'ITEM_ENGINE_IDENTITY');
  return {executable,version:pins.engineFullVersion,sha256:pin.executableSha256};
}
const VERIFY=String.raw`import pathlib,sys,json,io,copy,importlib.util,math
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);kind=sys.argv[2];artifact_root=pathlib.Path(sys.argv[3]) if len(sys.argv)>3 else root
spec=importlib.util.spec_from_file_location('media',root/'src/godot-game/tools/asset-pipeline/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
members=m.read_archive(artifact_root/'src/godot-game/production/assets'/ (kind+'.zip'));pack=m.loads(members[kind+'.json']);atlas=Image.open(io.BytesIO(members[kind+'.png'])).convert('RGBA')
m.need((artifact_root/'src/godot-game/game/content/packs'/(kind+'.json')).read_bytes()==members[kind+'.json'],'ITEM_PACK_BYTES')
m.need((artifact_root/'src/godot-game/game/assets/equipment'/(kind+'.png')).read_bytes()==members[kind+'.png'],'ITEM_ATLAS_BYTES')
scene_name=pathlib.PurePosixPath(pack['preview']['scene']).name
m.need((artifact_root/'src/godot-game/game/inspection'/scene_name).read_bytes()==members[scene_name],'ITEM_SCENE_BYTES')
inventory=m.loads(members['members.json']);m.need(len(inventory)+1==len(members) and len({x['path'] for x in inventory})==len(inventory),'ITEM_MEMBERS')
for row in inventory:m.need(row['path'] in members and row['bytes']==len(members[row['path']]) and row['sha256']==m.sha(members[row['path']]),'ITEM_MEMBER_HASH')
if kind=='items-01-10':
 legacy=members['history/pre-scroll.tscn'];m.need(m.sha(legacy)=='e6657e1e02bb4977a8f9f23b8c5baab7a7cd9210f88db07e20ddf6b44a51a1ae','ITEM_ORIGINAL_OVERFLOW')
 target=m.contained(root,pathlib.Path(__file__).parent.relative_to(root).as_posix()+'/pre-scroll.tscn')
 if target.exists():m.need(target.read_bytes()==legacy,'ITEM_ORIGINAL_OVERFLOW_OUTPUT')
 else:
  with target.open('xb') as stream:stream.write(legacy)
names=['Money','Arrows','Knife','Wooden club','Machete','Metal bat','Bow','Berries','Fruit','Meat']
foot=[[1,1],[1,2],[1,2],[1,3],[1,3],[1,4],[2,4],[1,1],[1,1],[1,2]]
wearables=kind=='items-11-24'
if wearables:
 prior=m.loads((root/'src/godot-game/game/content/packs/items-01-10.json').read_bytes());m.need([x['id'] for x in prior['items']]+[x['id'] for x in pack['items']]==['IT-%02d'%x for x in range(1,25)],'ITEM_FULL_ROSTER')
wear_names=['Basic shirt','Trousers','Coat','Cap','Face covering','Gloves','Boots','Protective helmet','Improvised body armor','Plated body armor','Belt with pouches','Small pouch','Small backpack','Field backpack']
wear_foot=[[2,2],[2,2],[2,3],[2,1],[1,1],[1,1],[1,2],[2,2],[2,3],[3,3],[2,1],[1,2],[2,3],[3,4]]
storage={11:[2,1],12:[2,2],13:[2,2],21:[4,2],22:[2,2],23:[4,4],24:[5,6]}
slots=['torso-clothing','legs','torso-clothing','headwear','face','gloves','boots','headwear','body-armor','body-armor','belt',None,'backpack','backpack']
live_components={11:['torso-front','torso-rear','sleeve-left-front','sleeve-left-rear','sleeve-right-front','sleeve-right-rear'],12:['waist','leg-left','leg-right'],13:['torso-front','torso-rear','hem','sleeve-left-front','sleeve-left-rear','sleeve-right-front','sleeve-right-rear'],14:['head'],15:['face'],16:['hand-left-front','hand-left-rear','hand-right-front','hand-right-rear'],17:['foot-left','foot-right'],18:['head'],19:['torso-front','torso-rear','strap-left-front','strap-left-rear','strap-right-front','strap-right-rear'],20:['torso-front','torso-rear','strap-left-front','strap-left-rear','strap-right-front','strap-right-rear'],21:['belt-front','belt-rear','pouch-left','pouch-right'],22:['container-header'],23:['bag-body','strap-left-front','strap-left-rear','strap-right-front','strap-right-rear'],24:['bag-body','pocket-left','pocket-right','strap-left-front','strap-left-rear','strap-right-front','strap-right-rear']}
image_cache={};info_cache={};reproduction_cache={};operation_cache={}
def decoded(raw):
 key=m.sha(raw)
 if key not in image_cache:
  image_cache[key]=Image.open(io.BytesIO(raw)).convert('RGBA');info_cache[key]=m.image_info(image_cache[key])
 return image_cache[key],info_cache[key]
def reproduced(part,data):
 key=json.dumps(part['operations'],sort_keys=True)+'|'+part['origin']+'|'+(m.sha(data[part['origin']]) if part['origin'] in data else '')
 if key not in reproduction_cache:reproduction_cache[key]=reproduce(part,data)
 return reproduction_cache[key]
def binary(im):
 im=im.convert('RGBA');im.putalpha(im.getchannel('A').point(lambda a:255 if a>=128 else 0));im.putdata([(r,g,b,a) if a else (0,0,0,0) for r,g,b,a in im.get_flattened_data()]);return im
def reproduce(part,data):
 if part['origin']=='procedural-condition-overlay':
  from PIL import ImageDraw
  w,h=part['operations'][0]['size'];m.need(0<w<=128 and 0<h<=128,'ITEM_OP_BOUNDS');im=Image.new('RGBA',(w,h));ImageDraw.Draw(im).line([(max(0,w//2-1),h//2),(w//2,h//2+1),(max(0,w//2-1),min(h-1,h//2+2))],fill=(33,33,46,255));return im
 m.need(part['origin'] in data,'ITEM_ORIGIN');im=Image.open(io.BytesIO(data[part['origin']])).convert('RGBA')
 prefix=[];origin_hash=m.sha(data[part['origin']])
 for op in part['operations']:
  prefix.append(op);operation_key=origin_hash+'|'+json.dumps(prefix,sort_keys=True)
  if operation_key in operation_cache:im=operation_cache[operation_key];continue
  if op['op']=='alpha-threshold':m.need(op['value']==128,'ITEM_SAMPLING');im=binary(im)
  elif op['op']=='crop':
   x,y,r,b=op['box'];m.need(0<=x<r<=im.width and 0<=y<b<=im.height,'ITEM_CROP');im=im.crop((x,y,r,b))
  elif op['op']=='nearest':m.need(len(op['size'])==2 and all(type(x)==int and 0<x<=128 for x in op['size']),'ITEM_OP_BOUNDS');im=im.resize(op['size'],Image.Resampling.NEAREST)
  elif op['op']=='crop-compose':
   x,y,r,b=op['box'];m.need(0<=x<r<=im.width and 0<=y<b<=im.height and len(op['placements'])<=8,'ITEM_CROP');crop=im.crop((x,y,r,b));w,h=op['size'];m.need(0<w<=128 and 0<h<=128,'ITEM_OP_BOUNDS');im=Image.new('RGBA',(w,h))
   for at in op['placements']:m.need(0<=at[0]<=w-crop.width and 0<=at[1]<=h-crop.height,'ITEM_PLACEMENT');im.alpha_composite(crop,at)
  else:raise ValueError('ITEM_OPERATION')
  operation_cache[operation_key]=im
 return im
def verify(p,data):
 m.need(p['schemaVersion']==1 and p['id']==kind and p['version']==1,'ITEM_IDENTITY')
 m.need(p['logicalPixelsPerWorldPixel']==1 and 'D-13' in p['stylePrecedence'] and 'OVERRIDES' in p['stylePrecedence'],'ITEM_STYLE_SCALE')
 m.need(p['acceptance']=={'visual':'pending','humanApproval':None,'owner':'04-32/04-33','completedObligations':[]},'ITEM_FALSE_ACCEPTANCE')
 m.need(p['commonBundleSha256']==m.digest(root/'src/godot-game/production/assets/common-selected.zip'),'ITEM_COMMON_SOURCE')
 m.need(p['recipe']['sha256']==m.sha(data['recipes/build-items.py']),'ITEM_RECIPE')
 expected_ids=range(11,25) if wearables else range(1,11)
 m.need([x['id'] for x in p['items']]==['IT-%02d'%i for i in expected_ids] and [x['name'] for x in p['items']]==(wear_names if wearables else names),'ITEM_ROSTER')
 m.need(p['atlas']['sha256']==m.sha(data[kind+'.png']) and all(p['atlas'][k]==v for k,v in m.image_info(atlas).items()),'ITEM_ATLAS_METADATA')
 m.need(len(p['parts'])<=256 and len({x['key'] for x in p['parts']})==len(p['parts']),'ITEM_PART_KEYS')
 parts={x['key']:x for x in p['parts']};images={}
 for part in p['parts']:
  m.need(isinstance(part['operations'],list) and 1<=len(part['operations'])<=4,'ITEM_OPERATION_LIMIT')
  raw=data[part['member']];im,metadata=decoded(raw);m.need(im.getbbox() is not None and part['sha256']==m.sha(raw),'ITEM_EMPTY_HASH')
  m.need(metadata==part['image'] and part['density']==1 and part['worldSize']==list(im.size),'ITEM_IMAGE_DIMENSIONS')
  m.need(set(im.getchannel('A').tobytes())<={0,255} and all(a or (r==g==b==0) for r,g,b,a in im.get_flattened_data()),'ITEM_ALPHA')
  x,y,w,h=part['atlasRect'];m.need([w,h]==list(im.size) and 0<=x<=atlas.width-w and 0<=y<=atlas.height-h and atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),'ITEM_ATLAS_PIXELS')
  replay=reproduced(part,data);m.need(replay.size==im.size and replay.tobytes()==im.tobytes(),'ITEM_REPRODUCTION');images[part['key']]=im
 for ordinal,item in enumerate(p['items']):
  i=ordinal+(11 if wearables else 1)
  m.need(item['inventoryFootprint']==(wear_foot[ordinal] if wearables else foot[ordinal]) and item['canonicalIcon'] in parts and item['iconVariants']==[item['canonicalIcon']],'ITEM_CANONICAL_ICON')
  m.need(item['balanceOwner']=='DT-04' and item['bodyFitOwner']=='04-06/04-07','ITEM_SCOPE')
  if not wearables:m.need(item['storage'] is None and item['slot']==('either-logical-hand' if i in [2,3,4,5,6,7] else None),'ITEM_STORAGE_SLOT')
  mode_names=(['Broken'] if i in [3,4,5,6,7,18,19,20] else [])+(['Nocked'] if i==7 else [])
  m.need([x['node'] for x in item['modes']]==mode_names,'ITEM_MODE_SET')
  s=item['source'];m.need(s['master'] in data and s['masterSha256']==m.sha(data[s['master']]),'ITEM_MASTER')
  original,metadata=decoded(data[s['master']]);m.need(metadata==s['original'],'ITEM_ORIGINAL')
  if 'generate' in item['route']:
   m.need(s['tool']=='built-in image_gen' and s['prompt'] in data and m.sha(data[s['prompt']])==s['promptSha256'],'ITEM_PROMPT')
   prompt=data[s['prompt']];m.need(b'D-13' in prompt and b'LOW-RESOLUTION' in prompt and b'transparen' in prompt,'ITEM_PROMPT_DIRECTION')
   m.need(s['maskThreshold']==128 and s['resampling']=='nearest' and original.getchannel('A').getextrema()==(0,255),'ITEM_SAMPLING')
   m.need(s['originalCanvas']==list(original.size) and s['nativeExport']==parts[item['master']]['worldSize'] and len(s['referenceMembers'])>=3 and all(x in data for x in s['referenceMembers']),'ITEM_MASTER_DIMENSIONS')
  else:
   d=m.loads(data[s['record']]);m.need(d['id']==s['resource'] and d['pngSha256']==m.sha(data[s['master']]) and d['source']['rawSha256'] and d['registration']['filter']=='nearest','ITEM_DONOR')
  c={x['name']:x for x in item['components']};expected=['icon']
  if wearables:expected+=live_components[i]+(['corpse-'+x for x in live_components[i]] if i!=22 else [])
  if i in [2,3,4,5,6,7]:expected+=['held-left','corpse-left','held-right','corpse-right']
  if i==2:expected+=['nocked-arrow','flying-arrow']
  if i==7:expected+=['drawn-bow','released-bow']
  if i in [8,9,10]:expected+=['food-use','corpse-storage']
  if i==1:expected+=['stored']
  m.need(list(c)==expected,'ITEM_COMPONENT_SET')
  for comp in c.values():m.need(comp['part'] in parts and comp['scale']==[1,1] and comp['nativeSize']==parts[comp['part']]['worldSize'] and math.isfinite(comp['rotation']) and len(comp['pivot'])==2 and all(type(x)==int for x in comp['pivot']),'ITEM_COMPONENT_BINDING')
  if i in [2,3,4,5,6,7]:
   grip={2:[2,2],3:[2,9],4:[2,15],5:[2,17],6:[2,21],7:[7,9]}[i]
   for hand in ['left','right']:
    m.need(c['held-'+hand]['attachment']=='hand.'+hand and c['corpse-'+hand]['attachment']=='corpse.hand.'+hand and c['held-'+hand]['pivot']==grip and c['corpse-'+hand]['rotation']==math.pi/2,'ITEM_GRIP')
  if i==2:m.need(item['canonicalIcon']!=item['master'] and images[item['canonicalIcon']].size!=images[item['master']].size and item['recoverableProjectile'] is False,'ITEM_ARROW_BUNDLE')
  if i in [3,4,5,6,7]:m.need(item['modes'][0]['node']=='Broken' and item['modes'][0]['visibleWhen']=='condition==0','ITEM_BROKEN')
  if i==7:m.need(item['modes'][1]['node']=='Nocked' and item['modes'][1]['identity']=='IT-02' and item['modes'][1]['visibleWhen']=='opposite-hand arrows>0 and bow usable' and item['emptyArrowHand']=='no nocked layer','ITEM_AMMUNITION')
  for mode in item['modes']:m.need(mode['part'] in parts and mode['identity'] in ['IT-%02d'%x for x in range(1,25)],'ITEM_MODE_PART')
  if wearables:
   m.need(item['storage']==storage.get(i) and item['slot']==slots[ordinal],'ITEM_STORAGE_SLOT')
   m.need(item['independentlyRemovable'] is True and item['bakedEquipment']==[] and item['secondaryMotion']=='closely-attached-little-sway','ITEM_REMOVABLE')
   m.need(all(x in data for x in s['donorIngredients']) and len(s['donorIngredients'])==2 and 'baked' in s['donorSuitability'],'ITEM_K14_SUITABILITY')
   for name in live_components[i]:
    m.need(c[name]['part']!=item['master'] and len(parts[c[name]['part']]['operations'])==4,'ITEM_MODULAR_CROP')
    if i!=22:m.need(c['corpse-'+name]['part']==c[name]['part'] and c['corpse-'+name]['attachment']=='corpse.'+c[name]['attachment'] and c['corpse-'+name]['rotation']==math.pi/2,'ITEM_CORPSE_PART')
   if i in [14,18]:m.need(item['crownAnchor']=={'attachment':'head.crown','aboveHeadgear':True,'independent':True},'ITEM_CROWN')
   if i==15:m.need(item['mouthReleaseAnchor']['maskDoesNotSuppressAttack'] is True and item['mouthReleaseAnchor']['attachment']=='head.mouth','ITEM_MOUTH')
   if i==16:
    for hand in ['left','right']:
     for layer in ['front','rear']:m.need(c['hand-'+hand+'-'+layer]['attachment']=='hand.'+hand+'.'+layer,'ITEM_HAND_IDENTITY')
   if i==17:
    for hand in ['left','right']:m.need(c['foot-'+hand]['attachment']=='foot.'+hand,'ITEM_FOOT_IDENTITY')
   if i in [18,19,20]:m.need(len(item['modes'])==1 and item['modes'][0]['visibleWhen']=='condition==0','ITEM_BROKEN')
   else:m.need(item['modes']==[],'ITEM_UNINVENTED_CONDITION')
   if i==22:m.need(item['presentation']=='stored-only' and item['wornWhenNested'] is False and list(c)==['icon','container-header'],'ITEM_STORED_POUCH')
   if i in [23,24]:m.need(item['presentation']=='worn-only-in-backpack-slot' and item['wornWhenNested'] is False,'ITEM_NESTED_BAG')
  route=item['inspection'];m.need(route['category']=='Items' and route['scene']==p['preview']['scene'] and route['animation']==item['id'] and route['button']=='Browse/List/Select_'+item['node'] and route['group']=='Groups/'+item['node'] and route['zoomAnimations']==['zoom1','zoom2','zoom4','zoom8'],'ITEM_ROUTE')
 if wearables:m.need(len({images[p['items'][i-11]['master']].tobytes() for i in [19,20,23,24]})==4,'ITEM_DISTINCT')
 else:m.need(len({images[p['items'][i-1]['master']].tobytes() for i in [3,4,5,6]})==4 and images[p['items'][7]['master']].tobytes()!=images[p['items'][8]['master']].tobytes(),'ITEM_DISTINCT')
 scene=data[scene_name].decode('utf-8');m.need('script' not in scene.lower() and 'type="Script"' not in scene and 'texture_filter = 2' not in scene and scene.count('method="play"')==len(p['items'])+sum(len(x['modes']) for x in p['items'])+4,'ITEM_SCENE_SAFETY')
 m.need(p['preview']['zooms']==[1,2,4,8] and p['preview']['currentHubReachability']=='pending-04-23','ITEM_PREVIEW')
 m.need(p['preview']['sha256']==m.sha(data[scene_name]) and p['preview']['artNode']=='ArtScroll/Canvas/Art' and p['preview']['viewport']==[680,320] and p['preview']['logicalWindow']==[1280,720],'ITEM_PREVIEW_REACHABILITY')
 return {'items':len(p['items']),'parts':len(parts),'members':len(data),'generatedMasters':sum('generate' in x['route'] for x in p['items']),'components':sum(len(x['components']) for x in p['items'])}
result=verify(pack,members);faults=[('missing-master','ITEM_MASTER'),('missing-prompt','ITEM_PROMPT'),('duplicate-id','ITEM_ROSTER'),('condition-icons','ITEM_CANONICAL_ICON'),('wrong-grip','ITEM_GRIP'),('wrong-logical-hand','ITEM_GRIP'),('bundle-alias','ITEM_ARROW_BUNDLE'),('empty-ammo','ITEM_AMMUNITION'),('missing-component','ITEM_COMPONENT_SET'),('wrong-sampling','ITEM_SAMPLING'),('wrong-zoom','ITEM_PREVIEW'),('script-bearing','ITEM_SCENE_SAFETY'),('linear-filter','ITEM_SCENE_SAFETY'),('false-acceptance','ITEM_FALSE_ACCEPTANCE'),('stale-route','ITEM_ROUTE')]
if wearables:faults=[(n,e) for n,e in faults if n not in ['wrong-grip','wrong-logical-hand','bundle-alias','empty-ammo']]+[('wrong-storage','ITEM_STORAGE_SLOT'),('baked-bag','ITEM_REMOVABLE'),('worn-pouch','ITEM_STORAGE_SLOT'),('nested-bag','ITEM_NESTED_BAG'),('wrong-glove-hand','ITEM_HAND_IDENTITY'),('invented-clothing-condition','ITEM_MODE_SET'),('missing-corpse','ITEM_COMPONENT_SET'),('missing-strap','ITEM_COMPONENT_SET'),('hidden-crown','ITEM_CROWN'),('blocked-mouth','ITEM_MOUTH')]
faults += [('unbounded-operations','ITEM_OPERATION_LIMIT'),('mode-proliferation','ITEM_MODE_SET')]
out=[]
for name,expected in faults:
 p=copy.deepcopy(pack);data=dict(members)
 if name=='missing-master':p['items'][2]['source']['master']='masters/missing.png'
 elif name=='missing-prompt':p['items'][2]['source']['prompt']='prompts/missing.txt'
 elif name=='duplicate-id':p['items'][1]['id']='IT-01'
 elif name=='condition-icons':p['items'][2]['iconVariants'].append('broken-icon')
 elif name=='wrong-grip':p['items'][2]['components'][1]['pivot'][1]+=1
 elif name=='wrong-logical-hand':p['items'][2]['components'][1]['attachment']='hand.right'
 elif name=='bundle-alias':p['items'][1]['canonicalIcon']=p['items'][1]['master'];p['items'][1]['iconVariants']=[p['items'][1]['master']]
 elif name=='empty-ammo':p['items'][6]['modes'][1]['visibleWhen']='always'
 elif name=='missing-component':p['items'][3]['components'].pop()
 elif name=='wrong-sampling':p['items'][2]['source']['resampling']='bilinear'
 elif name=='wrong-zoom':p['preview']['zooms']=[1,2,3,4]
 elif name=='script-bearing':data[scene_name]+=b'\nscript = null\n'
 elif name=='linear-filter':data[scene_name]=data[scene_name].replace(b'texture_filter = 1',b'texture_filter = 2')
 elif name=='false-acceptance':p['acceptance']['visual']='approved'
 elif name=='stale-route':p['items'][0]['inspection']['scene']='inspection/Absent.tscn'
 elif name=='wrong-storage':p['items'][0]['storage']=[2,2]
 elif name=='baked-bag':p['items'][2]['bakedEquipment']=['IT-23']
 elif name=='worn-pouch':p['items'][11]['slot']='backpack'
 elif name=='nested-bag':p['items'][12]['wornWhenNested']=True
 elif name=='wrong-glove-hand':p['items'][5]['components'][1]['attachment']='hand.right.front';p['items'][5]['components'][5]['attachment']='corpse.hand.right.front'
 elif name=='invented-clothing-condition':p['items'][0]['modes']=[{'node':'Broken','part':p['items'][0]['master'],'identity':'IT-11','visibleWhen':'condition==0'}]
 elif name=='missing-corpse':p['items'][0]['components'].pop()
 elif name=='missing-strap':p['items'][12]['components'].pop(1)
 elif name=='hidden-crown':p['items'][3]['crownAnchor']['aboveHeadgear']=False
 elif name=='blocked-mouth':p['items'][4]['mouthReleaseAnchor']['maskDoesNotSuppressAttack']=False
 elif name=='unbounded-operations':p['parts'][0]['operations']*=5
 elif name=='mode-proliferation':p['items'][0]['modes']=[{'node':'Extra','part':p['items'][0]['master'],'identity':p['items'][0]['id'],'visibleWhen':'always'}]
 failure=None
 try:verify(p,data)
 except (ValueError,KeyError) as e:failure=str(e)
 m.need(failure==expected,'ITEM_NEGATIVE_'+name+'_'+str(failure));out.append({'name':name,'diagnostic':failure,'expectedRejectionObserved':True})
result['controls']=out;result['expectedRejected']=len(out);print(json.dumps(result))
`;
function files(kind,scene){return ['tools/phase4/cases/items.cjs','tools/phase4/extract-media.py','production/phase4/common-selected.zip','src/godot-game/tools/verification/toolchain.json',
  'src/godot-game/assets/equipment/'+kind+'.png','src/godot-game/content/packs/'+kind+'.json','production/phase4/'+kind+'.zip',
  'src/godot-game/inspection/'+scene,...(kind==='items-11-24'?['src/godot-game/content/packs/items-01-10.json','src/godot-game/assets/equipment/items-01-10.png','production/phase4/items-01-10.zip']:[])];}
function fingerprint(paths){return Object.fromEntries(paths.map(file=>[file,hash(bytes(ROOT,file))]));}
function probe(nonce,kind,scene){return `extends SceneTree
func reject(code):
    printerr("ITEM_NATIVE_REJECT " + code)
    quit(1)
func _initialize():
    call_deferred("inspect")
func inspect():
    # Headless DisplayServer starts at100x100; explicitly instantiate the authored logical viewport.
    root.size = Vector2i(1280,720)
    var source = FileAccess.get_file_as_string("res://inspection/${scene}")
    if source.to_lower().contains("script") or source.contains("texture_filter = 2"):
        reject("SCENE_SAFETY")
        return
    var pack = JSON.parse_string(FileAccess.get_file_as_string("res://content/packs/${kind}.json"))
    var packed = load("res://inspection/${scene}")
    if not packed is PackedScene:
        reject("SCENE")
        return
    var scene = packed.instantiate()
    root.add_child(scene)
    if scene.get_script() != null or scene.texture_filter != CanvasItem.TEXTURE_FILTER_NEAREST:
        reject("FILTER_SCRIPT")
        return
    var parts = {}
    for part in pack.parts:
        parts[part.key] = part
    var selection = scene.get_node("Selection")
    var states = scene.get_node("State")
    var zooms = scene.get_node("ZoomPlayer")
    var sprites = 0
    var transitions = 0
    var reachable = 0
    await process_frame
    if root.size != Vector2i(1280,720):
        reject("LOGICAL_VIEWPORT")
        return
    for item in pack.items + pack.items.duplicate():
        var button = scene.get_node(item.inspection.button)
        var links = button.get_signal_connection_list("pressed")
        if links.size() != 1 or links[0].callable.get_object() != selection or links[0].callable.get_method() != "play" or links[0].callable.get_bound_arguments() != [StringName(item.id)]:
            reject("BINDING")
            return
        button.emit_signal("pressed")
        selection.advance(0)
        await process_frame
        for other in pack.items:
            var group = scene.get_node(other.inspection.group)
            if group.visible != (other.id == item.id):
                reject("SELECTION_RETIRE")
                return
            var art = group.get_node_or_null(pack.preview.artNode)
            if art == null:
                reject("ART_SCROLL")
                return
            for mode in other.modes:
                if art.get_node(mode.node).visible:
                    reject("OVERLAY_RETIRE")
                    return
        var group = scene.get_node(item.inspection.group)
        var scroll = group.get_node_or_null("ArtScroll")
        if not scroll is ScrollContainer or not scroll.clip_contents or scroll.size != Vector2(680,320) or group.get_node("Identity").get_parent() != group:
            reject("ART_SCROLL")
            return
        var canvas = scroll.get_node("Canvas")
        var art = group.get_node(pack.preview.artNode)
        if group.get_node("Identity").text != item.id + " | " + item.name + " | " + item.status or group.get_node("Use").text != item.usageLabel:
            reject("IDENTITY_STATUS")
            return
        if item.id == "IT-22" and (group.has_node("State_Worn") or item.slot != null):
            reject("STORED_POUCH")
            return
        var storage = {"IT-11":[2,1],"IT-12":[2,2],"IT-13":[2,2],"IT-21":[4,2],"IT-22":[2,2],"IT-23":[4,4],"IT-24":[5,6]}
        if storage.has(item.id):
            var size = storage[item.id]
            var expected = "Storage " + str(size[0]) + "x" + str(size[1]) + "\\n"
            for row in range(size[1]):
                if row > 0:
                    expected += "\\n"
                for column in range(size[0]):
                    if column > 0:
                        expected += " "
                    expected += "[ ]"
            if group.get_node("Storage").text != expected:
                reject("STORAGE_DIAGRAM")
                return
        for zoom in [1,2,4,8]:
            scene.get_node("Zoom/Zoom" + str(zoom)).emit_signal("pressed")
            zooms.advance(0)
            await process_frame
            if art.scale != Vector2(zoom,zoom):
                reject("ZOOM")
                return
            transitions += 1
            var horizontal = scroll.get_h_scroll_bar()
            var vertical = scroll.get_v_scroll_bar()
            var page = Vector2(horizontal.page,vertical.page)
            var maximum = Vector2(max(0,horizontal.max_value-horizontal.page),max(0,vertical.max_value-vertical.page))
            if page.x <= 0 or page.y <= 0:
                reject("SCROLL_EXTENT")
                return
            for component in item.components + item.modes:
                var sprite = art.get_node(component.node)
                var bounds = art.transform * (sprite.transform * sprite.get_rect())
                if bounds.position.x < -0.001 or bounds.position.y < -0.001 or bounds.end.x > canvas.size.x+0.001 or bounds.end.y > canvas.size.y+0.001 or bounds.size.x > page.x or bounds.size.y > page.y:
                    reject("CONTENT_EXTENT")
                    return
                scroll.scroll_horizontal = int(min(bounds.position.x,maximum.x))
                scroll.scroll_vertical = int(min(bounds.position.y,maximum.y))
                var visible = bounds.position - Vector2(scroll.scroll_horizontal,scroll.scroll_vertical)
                if visible.x < -0.001 or visible.y < -0.001 or visible.x+bounds.size.x > page.x+0.001 or visible.y+bounds.size.y > page.y+0.001:
                    reject("UNREACHABLE_COMPONENT")
                    return
                reachable += 1
        for component in item.components + item.modes:
            var sprite = art.get_node(component.node)
            var part = parts[component.part]
            if not sprite is Sprite2D or sprite.get_script() != null or sprite.centered or sprite.texture_filter != CanvasItem.TEXTURE_FILTER_NEAREST or not is_equal_approx(sprite.rotation,float(component.get("rotation",0))):
                reject("COMPONENT")
                return
            if not sprite.texture is AtlasTexture or not sprite.texture.filter_clip or sprite.texture.region != Rect2(part.atlasRect[0],part.atlasRect[1],part.atlasRect[2],part.atlasRect[3]):
                reject("ATLAS")
                return
            var image = sprite.texture.get_image()
            image.convert(Image.FORMAT_RGBA8)
            var digest = HashingContext.new()
            digest.start(HashingContext.HASH_SHA256)
            var pixels = image.get_data()
            # Godot repairs RGB beneath alpha0 on import; visible RGB and every alpha remain exact.
            for pixel in range(0,pixels.size(),4):
                if pixels[pixel+3] == 0:
                    pixels[pixel] = 0
                    pixels[pixel+1] = 0
                    pixels[pixel+2] = 0
            digest.update(pixels)
            if digest.finish().hex_encode() != part.image.rgbaSha256:
                reject("PIXELS")
                return
            sprites += 1
        for mode in item.modes:
            group.get_node("State_" + mode.node).emit_signal("pressed")
            states.advance(0)
            for candidate in item.modes:
                if art.get_node(candidate.node).visible != (candidate.node == mode.node):
                    reject("STATE_EXCLUSIVE")
                    return
        scene.get_node(item.inspection.button).emit_signal("pressed")
        selection.advance(0)
    scene.free()
    print("ITEM_NATIVE_COMPLETE ${nonce} items=" + str(pack.items.size()) + " sprites=" + str(sprites) + " zoom-transitions=" + str(transitions) + " reachable=" + str(reachable))
    quit(0)
`;}
async function native(context,kind,scene){
  const project=inside(context.directory,'native-items');fs.mkdirSync(project);
  write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="Item source proof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
  for(const file of files(kind,scene).filter(x=>require('../layout.cjs').physical(x).startsWith('src/godot-game/game/')))write(project,file.replace('src/godot-game/',''),bytes(ROOT,file));
  write(project,'probe.gd',Buffer.from(probe(context.nonce,kind,scene)));const tool=engine();write(context.directory,'engine.json',tool);
  const version=await runner.command(context,'items-engine-version',tool.executable,['--headless','--version']);need(version.stdout.trim()===tool.version,'ITEM_ENGINE_VERSION');
  const imported=await runner.command(context,'items-native-import',tool.executable,['--headless','--path',project,'--editor','--import']);need(!/ERROR:|SCRIPT ERROR:|Failed to load|Error importing/.test(imported.stdout+imported.stderr),'ITEM_NATIVE_IMPORT');
  const result=await runner.command(context,'items-native',tool.executable,['--headless','--path',project,'--script','res://probe.gd']);
  need(!/ERROR:|SCRIPT ERROR:/.test(result.stdout+result.stderr)&&result.stdout.includes('ITEM_NATIVE_COMPLETE '+context.nonce+' items='),'ITEM_NATIVE_COMPLETE');
  const target=inside(project,'inspection/'+scene);fs.writeFileSync(target,bytes(ROOT,'src/godot-game/inspection/'+scene).toString().replace('texture_filter = 1','texture_filter = 2'));
  let failure;try{await runner.command(context,'items-native-negative',tool.executable,['--headless','--path',project,'--script','res://probe.gd']);}catch(error){failure=error.message;}
  const rejected=document(context.directory,'items-native-negative.command.json');need(failure?.startsWith('CHILD_FAILED')&&rejected.code===1&&rejected.stderr.includes('ITEM_NATIVE_REJECT SCENE_SAFETY')&&!rejected.stdout.includes('ITEM_NATIVE_COMPLETE'),'ITEM_NATIVE_NEGATIVE');
  const original=kind==='items-01-10'?bytes(context.directory,'pre-scroll.tscn'):
    Buffer.from(bytes(ROOT,'src/godot-game/inspection/'+scene).toString().replaceAll('name="ArtScroll" type="ScrollContainer"','name="ArtScroll" type="Control"'));
  fs.writeFileSync(target,original);failure=undefined;
  try{await runner.command(context,'items-original-overflow-negative',tool.executable,['--headless','--path',project,'--script','res://probe.gd']);}catch(error){failure=error.message;}
  const overflow=document(context.directory,'items-original-overflow-negative.command.json');need(failure?.startsWith('CHILD_FAILED')&&overflow.code===1&&overflow.stderr.includes('ITEM_NATIVE_REJECT ART_SCROLL')&&!overflow.stdout.includes('ITEM_NATIVE_COMPLETE'),'ITEM_OVERFLOW_NEGATIVE');
  return {headless:true,observed:result.stdout.trim(),expectedRejected:2,originalOverflowSha256:hash(original)};
}
async function run(context,kind,scene){
  const before=fingerprint(files(kind,scene));write(context.directory,'inputs.json',before);write(context.directory,'verify-items.py',Buffer.from(VERIFY));
  const verified=await runner.command(context,'items-portable',PYTHON,['-B',inside(context.directory,'verify-items.py'),ROOT,kind]);const result=JSON.parse(verified.stdout);need(result.items===(kind==='items-01-10'?10:14)&&result.expectedRejected===(kind==='items-01-10'?17:23),'ITEM_PORTABLE_RESULT');
  write(context.directory,'negative-controls.json',result.controls);const actual=await native(context,kind,scene);need(equal(before,fingerprint(files(kind,scene))),'SOURCE_CHANGED');
  return {...result,native:actual,scope:'Portable source production and actual headless native selection/zoom; hub adoption04-23 and actual appearance04-32/33 pending'};
}
runner.registerCase('items.weapons-food',{group:'items',title:'Ten complete weapon/currency/ammunition/food identities and callable native previews',importance:'Critical',
  rationale:'Wrong grips, ammo, identity or missing sources invalidate required composition and inventory inputs',needsGameWindow:false,runGroup:'G0',order:22,
  requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-01','AC-03','AC-05','AC-06','AC-22'],
  setup:'Tracked retained item ZIP/atlas/pack/scriptless scene; pinned installed Python/Pillow and headless Godot4.7.2; no donor access',
  actions:['Reproduce every component from retained donor/generated masters and bounded crop/nearest recipes','Validate exact independent IT-01â€“10 roster/grips/canonical icons/conditional overlays and distinct bundle/projectile/food identities',
    'Reject17 semantic faults beyond outer archive hashes','Emit real selection/state/zoom button signals for each item twice; inspect exclusive groups, actual texture pixels and compact identity/status','Reach every component through native scroll ranges at1/2/4/8x; reject malformed/current and retained original-overflow scenes'],
  expected:'Ten complete source identities and components,17 semantic rejections, all native button/zoom transitions and two native rejections; final acceptance remains pending',
  reset:'Fresh owned immutable invocation and bounded headless children; no real game window',evidence:['inputs.json','negative-controls.json','engine.json','items-portable.command.json','items-native.command.json','items-native-negative.command.json','result.json']},
  context=>run(context,'items-01-10','ItemsWeaponsPreview.tscn'));
runner.registerCase('items.wearables',{group:'items',title:'Fourteen independent wearable/container masters and native storage/selection previews',importance:'Critical',
  rationale:'Baked gear, missing limb/strap/corpse inputs or incorrect container semantics block legal equipment composition',needsGameWindow:false,runGroup:'G0',order:23,
  requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-01','AC-03','AC-05','AC-06','AC-22'],
  setup:'Tracked retained wearable ZIP/atlas/pack/scriptless scene, prior ten-item pack, pinned Python/Pillow and headless Godot4.7.2; no donor access',
  actions:['Validate independent exact24-item union and one canonical icon per item','Reproduce every cropped torso/sleeve/leg/head/hand/foot/strap/corpse input from14 original built-in masters',
    'Validate seven exact storage diagrams and11-slot membership; pouch stored-only and bags hidden when nested','Reject23 semantic missing-source/component/slot/baked/crown/mouth/condition/operation-bound faults',
    'Exercise every native item/zoom and exclusive removable states twice, including current identity/status and previous-group retirement','Reach every component through native scroll ranges at1/2/4/8x; reject malformed/current and retained original-overflow scenes'],
  expected:'Fourteen distinct masters and all modular inputs;23 semantic rejections; exact native item/storage/pixel/zoom outcomes; rendered/body-fit/hub acceptance remains pending',
  reset:'Fresh owned immutable invocation and bounded headless children; no real window',evidence:['inputs.json','negative-controls.json','engine.json','items-portable.command.json','items-native.command.json','items-native-negative.command.json','result.json']},
  context=>run(context,'items-11-24','ItemsWearablesPreview.tscn'));
module.exports={VERIFY,PYTHON,engine,files,fingerprint,probe,native,run};
