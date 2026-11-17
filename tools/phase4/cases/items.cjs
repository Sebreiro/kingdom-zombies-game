'use strict';
const fs=require('node:fs'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,need,inside,bytes,document,write,hash}=require('../content.cjs');
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
function engine(){
  const pins=document(ROOT,'tools/phase1/toolchain.json'),pin=pins.archives['win32-x64'];
  need(process.platform==='win32'&&process.arch==='x64'&&process.version==='v'+pins.node,'ITEM_PLATFORM_PIN');
  const executable=inside(ROOT,'.phase1/tools/'+pin.directory+'/'+pin.executable),stat=fs.statSync(executable);
  need(stat.isFile()&&stat.size>0&&stat.size<=268435456&&hash(fs.readFileSync(executable))===pin.executableSha256,'ITEM_ENGINE_IDENTITY');
  return {executable,version:pins.engineFullVersion,sha256:pin.executableSha256};
}
const VERIFY=String.raw`import pathlib,sys,json,io,copy,importlib.util,math
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);kind=sys.argv[2];artifact_root=pathlib.Path(sys.argv[3]) if len(sys.argv)>3 else root
spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
members=m.read_archive(artifact_root/'production/phase4'/ (kind+'.zip'));pack=m.loads(members[kind+'.json']);atlas=Image.open(io.BytesIO(members[kind+'.png'])).convert('RGBA')
m.need((artifact_root/'src/godot-game/content/packs'/(kind+'.json')).read_bytes()==members[kind+'.json'],'ITEM_PACK_BYTES')
m.need((artifact_root/'src/godot-game/assets/equipment'/(kind+'.png')).read_bytes()==members[kind+'.png'],'ITEM_ATLAS_BYTES')
scene_name=pathlib.PurePosixPath(pack['preview']['scene']).name
m.need((artifact_root/'src/godot-game/inspection'/scene_name).read_bytes()==members[scene_name],'ITEM_SCENE_BYTES')
inventory=m.loads(members['members.json']);m.need(len(inventory)+1==len(members) and len({x['path'] for x in inventory})==len(inventory),'ITEM_MEMBERS')
for row in inventory:m.need(row['path'] in members and row['bytes']==len(members[row['path']]) and row['sha256']==m.sha(members[row['path']]),'ITEM_MEMBER_HASH')
names=['Money','Arrows','Knife','Wooden club','Machete','Metal bat','Bow','Berries','Fruit','Meat']
foot=[[1,1],[1,2],[1,2],[1,3],[1,3],[1,4],[2,4],[1,1],[1,1],[1,2]]
image_cache={};info_cache={};reproduction_cache={}
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
 for op in part['operations']:
  if op['op']=='alpha-threshold':m.need(op['value']==128,'ITEM_SAMPLING');im=binary(im)
  elif op['op']=='crop':
   x,y,r,b=op['box'];m.need(0<=x<r<=im.width and 0<=y<b<=im.height,'ITEM_CROP');im=im.crop((x,y,r,b))
  elif op['op']=='nearest':m.need(len(op['size'])==2 and all(type(x)==int and 0<x<=128 for x in op['size']),'ITEM_OP_BOUNDS');im=im.resize(op['size'],Image.Resampling.NEAREST)
  elif op['op']=='crop-compose':
   x,y,r,b=op['box'];m.need(0<=x<r<=im.width and 0<=y<b<=im.height and len(op['placements'])<=8,'ITEM_CROP');crop=im.crop((x,y,r,b));w,h=op['size'];m.need(0<w<=128 and 0<h<=128,'ITEM_OP_BOUNDS');im=Image.new('RGBA',(w,h))
   for at in op['placements']:m.need(0<=at[0]<=w-crop.width and 0<=at[1]<=h-crop.height,'ITEM_PLACEMENT');im.alpha_composite(crop,at)
  else:raise ValueError('ITEM_OPERATION')
 return im
def verify(p,data):
 m.need(p['schemaVersion']==1 and p['id']==kind and p['version']==1,'ITEM_IDENTITY')
 m.need(p['logicalPixelsPerWorldPixel']==1 and 'D-13' in p['stylePrecedence'] and 'OVERRIDES' in p['stylePrecedence'],'ITEM_STYLE_SCALE')
 m.need(p['acceptance']=={'visual':'pending','humanApproval':None,'owner':'04-32/04-33','completedObligations':[]},'ITEM_FALSE_ACCEPTANCE')
 m.need(p['commonBundleSha256']==m.digest(root/'production/phase4/common-selected.zip'),'ITEM_COMMON_SOURCE')
 m.need(p['recipe']['sha256']==m.sha(data['recipes/build-items.py']),'ITEM_RECIPE')
 m.need([x['id'] for x in p['items']]==['IT-%02d'%i for i in range(1,11)] and [x['name'] for x in p['items']]==names,'ITEM_ROSTER')
 m.need(len(p['parts'])<=256 and len({x['key'] for x in p['parts']})==len(p['parts']),'ITEM_PART_KEYS')
 parts={x['key']:x for x in p['parts']};images={}
 for part in p['parts']:
  raw=data[part['member']];im,metadata=decoded(raw);m.need(im.getbbox() is not None and part['sha256']==m.sha(raw),'ITEM_EMPTY_HASH')
  m.need(metadata==part['image'] and part['density']==1 and part['worldSize']==list(im.size),'ITEM_IMAGE_DIMENSIONS')
  m.need(set(im.getchannel('A').tobytes())<={0,255} and all(a or (r==g==b==0) for r,g,b,a in im.get_flattened_data()),'ITEM_ALPHA')
  x,y,w,h=part['atlasRect'];m.need([w,h]==list(im.size) and 0<=x<=atlas.width-w and 0<=y<=atlas.height-h and atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),'ITEM_ATLAS_PIXELS')
  replay=reproduced(part,data);m.need(replay.size==im.size and replay.tobytes()==im.tobytes(),'ITEM_REPRODUCTION');images[part['key']]=im
 for i,item in enumerate(p['items'],1):
  m.need(item['inventoryFootprint']==foot[i-1] and item['canonicalIcon'] in parts and item['iconVariants']==[item['canonicalIcon']],'ITEM_CANONICAL_ICON')
  m.need(item['balanceOwner']=='DT-04' and item['bodyFitOwner']=='04-06/04-07' and item['storage'] is None,'ITEM_SCOPE')
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
  route=item['inspection'];m.need(route['category']=='Items' and route['scene']==p['preview']['scene'] and route['animation']==item['id'] and route['button']=='Browse/List/Select_'+item['node'] and route['group']=='Groups/'+item['node'] and route['zoomAnimations']==['zoom1','zoom2','zoom4','zoom8'],'ITEM_ROUTE')
 m.need(len({images[p['items'][i-1]['master']].tobytes() for i in [3,4,5,6]})==4 and images[p['items'][7]['master']].tobytes()!=images[p['items'][8]['master']].tobytes(),'ITEM_DISTINCT')
 scene=data[scene_name].decode('utf-8');m.need('script' not in scene.lower() and 'type="Script"' not in scene and 'texture_filter = 2' not in scene and scene.count('method="play"')==len(p['items'])+sum(len(x['modes']) for x in p['items'])+4,'ITEM_SCENE_SAFETY')
 m.need(p['preview']['zooms']==[1,2,4,8] and p['preview']['currentHubReachability']=='pending-04-23','ITEM_PREVIEW')
 return {'items':len(p['items']),'parts':len(parts),'members':len(data),'generatedMasters':sum('generate' in x['route'] for x in p['items']),'components':sum(len(x['components']) for x in p['items'])}
result=verify(pack,members);faults=[('missing-master','ITEM_MASTER'),('missing-prompt','ITEM_PROMPT'),('duplicate-id','ITEM_ROSTER'),('condition-icons','ITEM_CANONICAL_ICON'),('wrong-grip','ITEM_GRIP'),('wrong-logical-hand','ITEM_GRIP'),('bundle-alias','ITEM_ARROW_BUNDLE'),('empty-ammo','ITEM_AMMUNITION'),('missing-component','ITEM_COMPONENT_SET'),('wrong-sampling','ITEM_SAMPLING'),('wrong-zoom','ITEM_PREVIEW'),('script-bearing','ITEM_SCENE_SAFETY'),('linear-filter','ITEM_SCENE_SAFETY'),('false-acceptance','ITEM_FALSE_ACCEPTANCE'),('stale-route','ITEM_ROUTE')];out=[]
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
 failure=None
 try:verify(p,data)
 except (ValueError,KeyError) as e:failure=str(e)
 m.need(failure==expected,'ITEM_NEGATIVE_'+name+'_'+str(failure));out.append({'name':name,'diagnostic':failure,'expectedRejectionObserved':True})
result['controls']=out;result['expectedRejected']=len(out);print(json.dumps(result))
`;
function files(kind,scene){return ['tools/phase4/cases/items.cjs','tools/phase4/extract-media.py','production/phase4/common-selected.zip',
  'src/godot-game/assets/equipment/'+kind+'.png','src/godot-game/content/packs/'+kind+'.json','production/phase4/'+kind+'.zip',
  'src/godot-game/inspection/'+scene];}
function fingerprint(paths){return Object.fromEntries(paths.map(file=>[file,hash(bytes(ROOT,file))]));}
function probe(nonce,kind,scene){return `extends SceneTree
func reject(code):
    printerr("ITEM_NATIVE_REJECT " + code)
    quit(1)
func _initialize():
    call_deferred("inspect")
func inspect():
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
    for item in pack.items + pack.items.duplicate():
        var button = scene.get_node(item.inspection.button)
        var links = button.get_signal_connection_list("pressed")
        if links.size() != 1 or links[0].callable.get_object() != selection or links[0].callable.get_method() != "play" or links[0].callable.get_bound_arguments() != [StringName(item.id)]:
            reject("BINDING")
            return
        button.emit_signal("pressed")
        selection.advance(0)
        for other in pack.items:
            var group = scene.get_node(other.inspection.group)
            if group.visible != (other.id == item.id):
                reject("SELECTION_RETIRE")
                return
            for mode in other.modes:
                if group.get_node("Art/" + mode.node).visible:
                    reject("OVERLAY_RETIRE")
                    return
        var group = scene.get_node(item.inspection.group)
        if group.get_node("Identity").text != item.id + " | " + item.name + " | " + item.status or group.get_node("Use").text != item.usageLabel:
            reject("IDENTITY_STATUS")
            return
        if item.id == "IT-22" and (group.has_node("State_Worn") or item.slot != null):
            reject("STORED_POUCH")
            return
        for zoom in [1,2,4,8]:
            scene.get_node("Zoom/Zoom" + str(zoom)).emit_signal("pressed")
            zooms.advance(0)
            if group.get_node("Art").scale != Vector2(zoom,zoom):
                reject("ZOOM")
                return
            transitions += 1
        for component in item.components + item.modes:
            var sprite = group.get_node("Art/" + component.node)
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
                if group.get_node("Art/" + candidate.node).visible != (candidate.node == mode.node):
                    reject("STATE_EXCLUSIVE")
                    return
        scene.get_node(item.inspection.button).emit_signal("pressed")
        selection.advance(0)
    scene.free()
    print("ITEM_NATIVE_COMPLETE ${nonce} items=" + str(pack.items.size()) + " sprites=" + str(sprites) + " zoom-transitions=" + str(transitions))
    quit(0)
`;}
async function native(context,kind,scene){
  const project=inside(context.directory,'native-items');fs.mkdirSync(project);
  write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="Item source proof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
  for(const file of files(kind,scene).filter(x=>x.startsWith('src/godot-game/')))write(project,file.replace('src/godot-game/',''),bytes(ROOT,file));
  write(project,'probe.gd',Buffer.from(probe(context.nonce,kind,scene)));const tool=engine();write(context.directory,'engine.json',tool);
  const version=await runner.command(context,'items-engine-version',tool.executable,['--headless','--version']);need(version.stdout.trim()===tool.version,'ITEM_ENGINE_VERSION');
  const imported=await runner.command(context,'items-native-import',tool.executable,['--headless','--path',project,'--editor','--import']);need(!/ERROR:|SCRIPT ERROR:|Failed to load|Error importing/.test(imported.stdout+imported.stderr),'ITEM_NATIVE_IMPORT');
  const result=await runner.command(context,'items-native',tool.executable,['--headless','--path',project,'--script','res://probe.gd']);
  need(!/ERROR:|SCRIPT ERROR:/.test(result.stdout+result.stderr)&&result.stdout.includes('ITEM_NATIVE_COMPLETE '+context.nonce+' items='),'ITEM_NATIVE_COMPLETE');
  const target=inside(project,'inspection/'+scene);fs.writeFileSync(target,bytes(ROOT,'src/godot-game/inspection/'+scene).toString().replace('texture_filter = 1','texture_filter = 2'));
  let failure;try{await runner.command(context,'items-native-negative',tool.executable,['--headless','--path',project,'--script','res://probe.gd']);}catch(error){failure=error.message;}
  const rejected=document(context.directory,'items-native-negative.command.json');need(failure?.startsWith('CHILD_FAILED')&&rejected.code===1&&rejected.stderr.includes('ITEM_NATIVE_REJECT SCENE_SAFETY')&&!rejected.stdout.includes('ITEM_NATIVE_COMPLETE'),'ITEM_NATIVE_NEGATIVE');
  return {headless:true,observed:result.stdout.trim(),expectedRejected:1};
}
async function run(context,kind,scene){
  const before=fingerprint(files(kind,scene));write(context.directory,'inputs.json',before);write(context.directory,'verify-items.py',Buffer.from(VERIFY));
  const verified=await runner.command(context,'items-portable',PYTHON,['-B',inside(context.directory,'verify-items.py'),ROOT,kind]);const result=JSON.parse(verified.stdout);need(result.items===10&&result.expectedRejected===15,'ITEM_PORTABLE_RESULT');
  write(context.directory,'negative-controls.json',result.controls);const actual=await native(context,kind,scene);need(equal(before,fingerprint(files(kind,scene))),'SOURCE_CHANGED');
  return {...result,native:actual,scope:'Portable source production and actual headless native selection/zoom; hub adoption04-23 and actual appearance04-32/33 pending'};
}
runner.registerCase('items.weapons-food',{group:'items',title:'Ten complete weapon/currency/ammunition/food identities and callable native previews',importance:'Critical',
  rationale:'Wrong grips, ammo, identity or missing sources invalidate required composition and inventory inputs',needsGameWindow:false,runGroup:'G0',order:22,
  requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-01','AC-03','AC-05','AC-06','AC-22'],
  setup:'Tracked retained item ZIP/atlas/pack/scriptless scene; pinned installed Python/Pillow and headless Godot4.7.2; no donor access',
  actions:['Reproduce every component from retained donor/generated masters and bounded crop/nearest recipes','Validate exact independent IT-01–10 roster/grips/canonical icons/conditional overlays and distinct bundle/projectile/food identities',
    'Reject15 semantic faults beyond outer archive hashes','Emit real selection/state/zoom button signals for each item twice; inspect exclusive groups, actual texture pixels and compact identity/status','Reject an actual malformed native scene'],
  expected:'Ten complete source identities and components,15 semantic rejections, all native button/zoom transitions and one native rejection; final acceptance remains pending',
  reset:'Fresh owned immutable invocation and bounded headless children; no real game window',evidence:['inputs.json','negative-controls.json','engine.json','items-portable.command.json','items-native.command.json','items-native-negative.command.json','result.json']},
  context=>run(context,'items-01-10','ItemsWeaponsPreview.tscn'));
module.exports={VERIFY,PYTHON,files,fingerprint,probe,native,run};
