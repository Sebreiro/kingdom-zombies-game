'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,need,inside,bytes,document,write,hash}=require('../content.cjs');
const {PYTHON}=require('../python.cjs');
const EXTRACT='tools/phase4/extract-media.py',COMMON='src/godot-game/content/packs/common.json';
const ATLAS='src/godot-game/assets/common/donor-kit.png',BUNDLE='production/phase4/common-selected.zip';
const FILES=[EXTRACT,COMMON,ATLAS,BUNDLE,'tools/phase4/cases/media.cjs'];
function inputs(files){return Object.fromEntries(files.map(file=>[file,hash(bytes(ROOT,file))]));}
function validateCommon(pack){
  need(pack.schemaVersion===1&&pack.id==='common'&&pack.version===1&&pack.resources.length>100&&pack.clips.length===35,'COMMON_IDENTITY');
  need(pack.recipe.sha256===hash(bytes(ROOT,EXTRACT)),'COMMON_RECIPE_CHANGED');
  need(pack.atlas.path==='assets/common/donor-kit.png'&&pack.atlas.sha256===hash(bytes(ROOT,ATLAS)),'COMMON_ATLAS_IDENTITY');
  need(pack.preservation.verifiedAfter&&pack.acceptance.owner==='04-32/04-33'&&pack.acceptance.visual==='pending'&&
    equal(pack.acceptance.completedObligations,[]),'COMMON_FALSE_ACCEPTANCE');
  for(const clip of pack.clips){
    need(clip.callbacksExecutable===false&&clip.layers.length>0&&clip.layers.every(x=>x.keys.length>0),'COMMON_CLIP_LAYERS');
    need(clip.events.every(x=>x.id==='work.contact'&&x.execution==='presentation-data-only'),'COMMON_CALLBACK_EXECUTION');
  }
  const portal=pack.clips.find(x=>x.id==='common.clip.2568');
  need(portal.layers.length===10&&portal.layers.every(x=>x.keys.length===12),'COMMON_PORTAL_LAYERS');
  return pack;
}
function engine(){
  const pins=document(ROOT,'tools/phase1/toolchain.json'),pin=pins.archives['win32-x64'];
  need(process.platform==='win32'&&process.arch==='x64'&&process.version==='v'+pins.node,'MEDIA_PLATFORM_PIN');
  const executable=inside(ROOT,'.phase1/tools/'+pin.directory+'/'+pin.executable),stat=fs.statSync(executable);
  need(stat.isFile()&&stat.size>0&&stat.size<=268435456,'MEDIA_ENGINE_FILE');
  need(hash(fs.readFileSync(executable))===pin.executableSha256,'MEDIA_ENGINE_HASH');
  return {executable,version:pins.engineFullVersion,sha256:pin.executableSha256};
}
function conversionProbe(nonce){return `extends SceneTree
func reject(code):
    printerr("MEDIA_CONVERSION_REJECT " + code)
    quit(1)
func _initialize():
    var pack = JSON.parse_string(FileAccess.get_file_as_string("res://common.json"))
    var atlas = load("res://donor-kit.png")
    if atlas == null or atlas.get_width() != int(pack.atlas.width) or atlas.get_height() != int(pack.atlas.height):
        reject("ATLAS_NATIVE")
        return
    var resources = {}
    for item in pack.resources:
        if item.has("atlasRect"):
            resources[item.id] = item
    var total_tracks = 0
    var total_keys = 0
    var held = 0
    for clip in pack.clips:
        var animation = Animation.new()
        animation.length = clip.stop - clip.start
        animation.loop_mode = Animation.LOOP_LINEAR if clip.loop else Animation.LOOP_NONE
        for layer in clip.layers:
            var track = animation.add_track(Animation.TYPE_VALUE)
            animation.track_set_path(track, NodePath("Layer" + str(layer.curve) + ":texture"))
            animation.value_track_set_update_mode(track, Animation.UPDATE_DISCRETE)
            var last = ""
            for key in layer.keys:
                if key.has("sprite") and key.sprite != null:
                    var item = resources[key.sprite]
                    var frame = AtlasTexture.new()
                    frame.atlas = atlas
                    frame.region = Rect2(item.atlasRect[0], item.atlasRect[1], item.atlasRect[2], item.atlasRect[3])
                    frame.filter_clip = true
                    animation.track_insert_key(track, key.time - clip.start, frame)
                    if last == key.sprite:
                        held += 1
                    last = key.sprite
                else:
                    animation.track_insert_key(track, key.time - clip.start, key.value)
                total_keys += 1
            if animation.track_get_key_count(track) != layer.keys.size():
                reject("KEY_LOSS")
                return
            for index in range(layer.keys.size()):
                if abs(animation.track_get_key_time(track,index) - (layer.keys[index].time - clip.start)) > 0.000001:
                    reject("TIME_LOSS")
                    return
            total_tracks += 1
        if animation.loop_mode != (Animation.LOOP_LINEAR if clip.loop else Animation.LOOP_NONE):
            reject("LOOP_LOSS")
            return
    if total_tracks < 50 or held < 1:
        reject("COVERAGE")
        return
    print("MEDIA_CONVERSION_COMPLETE ${nonce} tracks=" + str(total_tracks) + " keys=" + str(total_keys) + " repeated=" + str(held))
    quit(0)
`;}
async function nativeConversion(context){
  const project=inside(context.directory,'native-conversion');fs.mkdirSync(project);
  write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="Media conversion proof"\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
  write(project,'common.json',bytes(ROOT,COMMON));write(project,'donor-kit.png',bytes(ROOT,ATLAS));
  write(project,'probe.gd',Buffer.from(conversionProbe(context.nonce)));
  const tool=engine();write(context.directory,'engine.json',tool);
  const version=await runner.command(context,'engine-version',tool.executable,['--headless','--version']);
  need(version.stdout.trim()===tool.version,'MEDIA_ENGINE_VERSION');
  const imported=await runner.command(context,'native-import',tool.executable,['--headless','--path',project,'--editor','--import']);
  need(!/ERROR:|SCRIPT ERROR:|Failed to load|Error importing/.test(imported.stdout+imported.stderr),'MEDIA_NATIVE_IMPORT');
  const run=await runner.command(context,'native-conversion',tool.executable,['--headless','--path',project,'--script','res://probe.gd']);
  need(!/ERROR:|SCRIPT ERROR:/.test(run.stdout+run.stderr)&&run.stdout.includes('MEDIA_CONVERSION_COMPLETE '+context.nonce),'MEDIA_NATIVE_CONVERSION');
  return {observed:run.stdout.trim(),headless:true};
}
async function conversion(context){
  const initial=inputs(FILES);write(context.directory,'inputs.json',initial);
  const pack=validateCommon(document(ROOT,COMMON));
  const verified=await runner.command(context,'portable-common',PYTHON,['-B',inside(ROOT,EXTRACT),'--verify-common',
    inside(ROOT,BUNDLE),inside(ROOT,COMMON),inside(ROOT,ATLAS)],{timeout:60000});
  const controls=await runner.command(context,'conversion-controls',PYTHON,['-B',inside(ROOT,EXTRACT),'--conversion-controls',
    inside(ROOT,BUNDLE),inside(context.directory,'controls')],{timeout:60000});
  const actual=JSON.parse(verified.stdout),negative=JSON.parse(controls.stdout);
  need(actual.portable&&actual.sprites===pack.resources.filter(x=>x.atlasRect).length&&negative.expectedRejected===13,'MEDIA_COMMON_PROOF');
  const native=await nativeConversion(context);
  need(equal(initial,inputs(FILES)),'SOURCE_CHANGED');
  return {...actual,...negative,native,scope:'Retained/source/headless conversion; visual acceptance pending04-32'};
}
runner.registerCase('media.conversion',{group:'media',title:'Portable exact donor pixels and multi-layer runtime conversion',importance:'Critical',
  rationale:'Lost layers, trim registration, held frames or executable donor callbacks corrupt every later resource consumer',
  needsGameWindow:false,runGroup:'G0',order:20,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-02','AC-03','AC-05','AC-08','AC-19'],
  setup:'Tracked selected ZIP, pack and atlas; pinned installed Python/Pillow; no donor root is read by this case; headless Godot4.7.2',
  actions:['Validate every retained member/source/stream/image/registration identity and all35 runtime clips',
    'Exercise independent H16 nonuniform/repeated/held, dense, constant, loop/endpoint and event oracles',
    'Reject semantic corruptions after rebuilding outer ZIP hashes','Import actual atlas and construct native Animation tracks with exact key times'],
  expected:'Complete portable kit and all native tracks/keys retained;13 deliberate corruptions rejected; actual rendered approval remains pending',
  reset:'Fresh owned immutable invocation, bounded children; no real game window',
  evidence:['inputs.json','portable-common.command.json','conversion-controls.command.json','controls/held-out.json','controls/negative-controls.json',
    'engine.json','native-conversion.command.json','result.json']},conversion);
module.exports={FILES,inputs,validateCommon,engine,conversionProbe,nativeConversion,conversion};
const SCALE_VERIFY=String.raw`import importlib.util, pathlib, sys, io, json, copy
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py')
media=importlib.util.module_from_spec(spec);spec.loader.exec_module(media);Image=media.image_module()
bundle=root/'production/phase4/scale-pilot.zip';members=media.read_archive(bundle)
pack=media.loads(members['scale-pilot.json'])
media.need((root/'src/godot-game/content/packs/scale-pilot.json').read_bytes()==members['scale-pilot.json'],'SCALE_PACK_IDENTITY')
media.need((root/'src/godot-game/assets/common/scale-pilot.png').read_bytes()==members['scale-pilot.png'],'SCALE_ATLAS_IDENTITY')
media.need((root/'src/godot-game/inspection/ScaleComparison.tscn').read_bytes()==members['ScaleComparison.tscn'],'SCALE_SCENE_IDENTITY')
inventory=media.loads(members['members.json'])
media.need(len(inventory)+1==len(members),'SCALE_MEMBER_COUNT')
for row in inventory:
 media.need(row['path'] in members and len(members[row['path']])==row['bytes'] and media.sha(members[row['path']])==row['sha256'],'SCALE_MEMBER_HASH')
media.need(pack['recipe']['verifierSha256']==media.sha(pathlib.Path(__file__).read_bytes()) and
           pack['recipe']['sha256']==media.sha(members['recipes/build-scale.py']),'SCALE_RECIPE_IDENTITY')

def verify(pack,members):
 media.need(pack['schemaVersion']==1 and pack['id']=='scale-pilot' and pack['version']==1,'SCALE_IDENTITY')
 media.need([x['name'] for x in pack['resources']]==['survivor','zombie','metal','shelter','wall-wood','wall-reinforced','tower','base-donor'],'SCALE_PART_IDS')
 media.need(pack['scaleDecision']['logicalPixelsPerWorldPixel']==1 and
            pack['scaleDecision']['status']=='selected-source-production-choice','SCALE_DENSITY')
 media.need('D-13' in pack['stylePrecedence'] and 'OVERRIDES' in pack['stylePrecedence'],'SCALE_STYLE')
 media.need(pack['acceptance']['visual']=='pending' and pack['acceptance']['humanApproval'] is None and
            pack['acceptance']['completedObligations']==[],'SCALE_FALSE_ACCEPTANCE')
 media.need(pack['commonBundleSha256']==media.digest(root/'production/phase4/common-selected.zip'),'SCALE_DONOR_BUNDLE')
 media.need(pack['assembly']['groundLine']==92 and all(x['anchor'][1]==92 for x in pack['assembly']['layers']),'SCALE_GROUND')
 scene=members['ScaleComparison.tscn'].decode('utf-8')
 media.need('texture_filter = 1' in scene and 'filter_clip = true' in scene and 'texture_filter = 2' not in scene,'SCALE_SCENE_FILTER')
 atlas=Image.open(io.BytesIO(members['scale-pilot.png'])).convert('RGBA')
 media.need(media.image_info(atlas)=={key:pack['atlas'][key] for key in media.image_info(atlas)},'SCALE_ATLAS_METADATA')
 for part in pack['resources']:
  native=Image.open(io.BytesIO(members[part['member']])).convert('RGBA')
  media.need(part['dimensions']['worldSize']==part['dimensions']['nativeExport']==list(native.size) and
             part['dimensions']['logicalPixelsPerWorldPixel']==1,'SCALE_DIMENSION')
  media.need(set(native.getchannel('A').tobytes())=={0,255} or set(native.getchannel('A').tobytes())=={255},'SCALE_ALPHA')
  if part['route']=='generate':
   media.need(all(a or (r==0 and g==0 and b==0) for r,g,b,a in native.get_flattened_data()),'SCALE_ALPHA_RGB')
  media.need(media.image_info(native)==part['image'] and media.sha(members[part['member']])==part['sha256'],'SCALE_NATIVE_IDENTITY')
  x,y,w,h=part['atlasRect']
  media.need([w,h]==list(native.size) and atlas.crop((x,y,x+w,y+h)).tobytes()==native.tobytes(),'SCALE_ATLAS_PIXELS')
  if part['route']=='generate':
   source=part['source'];media.need(source['tool']=='built-in image_gen' and source['transparentBackgroundRequested'],'SCALE_GENERATION')
   media.need(source['masterMember'] in members,'SCALE_MASTER_MISSING')
   media.need(source['promptMember'] in members,'SCALE_PROMPT_MISSING')
   raw=members[source['masterMember']];prompt=members[source['promptMember']]
   media.need(media.sha(raw)==source['masterSha256'] and media.sha(prompt)==source['promptSha256'],'SCALE_MASTER_PROMPT_HASH')
   media.need(b'D-13' in prompt and b'LOW-RESOLUTION' in prompt and b'transparen' in prompt,'SCALE_PROMPT_DIRECTION')
   transformation=part['transformation'];media.need(transformation['resampling']=='nearest' and transformation['maskThreshold']==128,'SCALE_SAMPLING')
   original=Image.open(io.BytesIO(raw)).convert('RGBA');alpha=original.getchannel('A')
   media.need(alpha.getextrema()==(0,255) and alpha.getpixel((0,0))<128 and alpha.getpixel((original.width-1,original.height-1))<128 and
              sum(a<128 for a in alpha.tobytes())/(original.width*original.height)>0.30,'SCALE_MASTER_ALPHA')
   media.need(media.image_info(original)==source['originalAlpha'],'SCALE_ORIGINAL_METADATA')
   clean=original.copy();clean.putalpha(alpha.point(lambda x:255 if x>=128 else 0))
   clean.putdata([(r,g,b,a) if a else (0,0,0,0) for r,g,b,a in clean.get_flattened_data()])
   transformation=part['transformation'];media.need(transformation['resampling']=='nearest' and transformation['maskThreshold']==128,'SCALE_SAMPLING')
   media.need(list(clean.getbbox())==transformation['crop'],'SCALE_CROP')
   media.need(media.png(clean)==members['cleaned/'+part['name']+'.png'],'SCALE_MASK_REPRODUCTION')
   expected=clean.crop(tuple(transformation['crop'])).resize(tuple(transformation['targetSize']),Image.Resampling.NEAREST)
   media.need(expected.tobytes()==native.tobytes(),'SCALE_NATIVE_REPRODUCTION')
  else:
   donor=media.loads(members[part['recordMember']])
   media.need(donor['id']==part['commonId'] and donor['source']==part['source'] and
              donor['pngSha256']==media.sha(members[part['member']]) and donor['image']==part['image'],'SCALE_DONOR_IDENTITY')
 for reference in pack['references']:
  media.need(reference['member'] in members and media.sha(members[reference['member']])==reference['sha256'],'SCALE_REFERENCE')
 media.need(pack['assembly']['groundLine']==92 and all(x['anchor'][1]==92 for x in pack['assembly']['layers']),'SCALE_GROUND')
 media.need(len(pack['assembly']['layers'])==9 and pack['assembly']['rows']==['Day','Night'] and
            pack['assembly']['nightTint']==[0.63,0.72,0.88] and pack['assembly']['viewZoom']==3,'SCALE_ASSEMBLY')
 scene=members['ScaleComparison.tscn'].decode('utf-8')
 media.need('texture_filter = 1' in scene and 'filter_clip = true' in scene and 'texture_filter = 2' not in scene,'SCALE_SCENE_FILTER')
 media.need(media.sha(members['ScaleComparison.tscn'])==pack['scene']['sha256'],'SCALE_SCENE_HASH')
 return {'parts':len(pack['resources']),'generatedMasters':4,'references':len(pack['references']),'members':len(members),
         'atlasSha256':media.sha(members['scale-pilot.png']),'scaleDecision':pack['scaleDecision'],'acceptance':pack['acceptance']}

result=verify(pack,members);outcomes=[]
for name,code in [('missing-master','SCALE_MASTER_MISSING'),('missing-prompt','SCALE_PROMPT_MISSING'),
                  ('smooth-sampling','SCALE_SAMPLING'),('wrong-ground','SCALE_GROUND'),
                  ('density-change','SCALE_DENSITY'),('missing-tower','SCALE_PART_IDS'),
                  ('missing-base','SCALE_PART_IDS'),('wrong-world-size','SCALE_DIMENSION'),
                  ('atlas-region','SCALE_ATLAS_PIXELS'),('erased-precedence','SCALE_STYLE'),
                  ('fake-generation','SCALE_GENERATION'),('scene-linear-filter','SCALE_SCENE_FILTER')]:
 subject=copy.deepcopy(pack);data=dict(members)
 if name=='missing-master':data.pop(subject['resources'][0]['source']['masterMember'])
 elif name=='missing-prompt':data.pop(subject['resources'][0]['source']['promptMember'])
 elif name=='smooth-sampling':subject['resources'][0]['transformation']['resampling']='bilinear'
 elif name=='wrong-ground':subject['assembly']['layers'][0]['anchor'][1]+=1
 elif name=='density-change':subject['scaleDecision']['logicalPixelsPerWorldPixel']=2
 elif name=='missing-tower':subject['resources'].pop(6)
 elif name=='missing-base':subject['resources'].pop(7)
 elif name=='wrong-world-size':subject['resources'][0]['dimensions']['worldSize'][1]+=1
 elif name=='atlas-region':subject['resources'][0]['atlasRect'][0]+=1
 elif name=='erased-precedence':subject['stylePrecedence']='Older richer art'
 elif name=='fake-generation':subject['resources'][0]['source']['tool']='programmatic stand-in'
 elif name=='scene-linear-filter':data['ScaleComparison.tscn']=data['ScaleComparison.tscn'].replace(b'texture_filter = 1',b'texture_filter = 2')
 failure=None
 try:verify(subject,data)
 except ValueError as error:failure=str(error)
 media.need(failure==code,'SCALE_CONTROL_NOT_REJECTED_'+name+'_'+str(failure))
 outcomes.append({'name':name,'diagnostic':failure,'expectedRejectionObserved':True})
result['controls']=outcomes;result['expectedRejected']=len(outcomes)
print(json.dumps(result))
`;
const SCALE='src/godot-game/content/packs/scale-pilot.json',SCALE_ATLAS='src/godot-game/assets/common/scale-pilot.png';
const SCALE_SCENE='src/godot-game/inspection/ScaleComparison.tscn',SCALE_BUNDLE='production/phase4/scale-pilot.zip';
const SCALE_FILES=[...FILES,SCALE,SCALE_ATLAS,SCALE_SCENE,SCALE_BUNDLE];
function scaleProbe(nonce){return `extends SceneTree
func reject(code):
    printerr("MEDIA_SCALE_REJECT " + code)
    quit(1)
func _initialize():
    var pack = JSON.parse_string(FileAccess.get_file_as_string("res://content/packs/scale-pilot.json"))
    var loaded = load("res://inspection/ScaleComparison.tscn")
    if loaded == null:
        reject("SCENE")
        return
    var scene = loaded.instantiate()
    root.add_child(scene)
    if scene.texture_filter != CanvasItem.TEXTURE_FILTER_NEAREST:
        reject("FILTER")
        return
    var parts = {}
    for part in pack.resources:
        parts[part.name] = part
    var observed = 0
    for row_name in ["Day", "Night"]:
        var row = scene.get_node(row_name)
        if row.scale != Vector2(3,3):
            reject("ZOOM")
            return
        var artwork = row.get_node("Artwork")
        var expected_tint = Color(1,1,1) if row_name == "Day" else Color(0.63,0.72,0.88)
        if not artwork.modulate.is_equal_approx(expected_tint):
            reject("DAY_NIGHT")
            return
        for layer in pack.assembly.layers:
            var name = layer.name.replace("-", "_")
            var sprite = artwork.get_node_or_null(name)
            if sprite == null:
                reject("SPRITE " + name)
                return
            var part = parts[layer.part]
            var rectangle = Rect2(part.atlasRect[0],part.atlasRect[1],part.atlasRect[2],part.atlasRect[3])
            if not sprite is Sprite2D or sprite.centered or sprite.position != Vector2(layer.anchor[0],92) or sprite.scale != Vector2.ONE:
                reject("REGISTRATION " + name)
                return
            if sprite.offset != Vector2(-floor(part.image.width/2.0),-part.image.height) or sprite.z_index != int(layer.layer):
                reject("PIVOT_LAYER " + name)
                return
            if not sprite.texture is AtlasTexture or sprite.texture.region != rectangle or not sprite.texture.filter_clip:
                reject("ATLAS " + name)
                return
            if sprite.texture.atlas.get_width() != int(pack.atlas.width) or sprite.texture.atlas.get_height() != int(pack.atlas.height):
                reject("ATLAS_SIZE " + name)
                return
            var pixels = sprite.texture.get_image().get_data()
            var visible = 0
            for index in range(3,pixels.size(),4):
                if pixels[index] > 0:
                    visible += 1
            if visible != int(part.image.visiblePixels):
                reject("PIXELS " + name)
                return
            observed += 1
        for anchor in [16,56,100,140,180,228,320]:
            if not artwork.get_node("Contact" + str(anchor)) is Polygon2D:
                reject("CONTACT")
                return
    scene.free()
    print("MEDIA_SCALE_COMPLETE ${nonce} sprites=" + str(observed) + " logical-density=1 day-night=2")
    quit(0)
`;}
async function nativeScale(context){
  const project=inside(context.directory,'native-scale');fs.mkdirSync(project);
  write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="Scale comparison proof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
  for(const file of [SCALE,SCALE_ATLAS,SCALE_SCENE])write(project,file.replace('src/godot-game/',''),bytes(ROOT,file));
  write(project,'probe.gd',Buffer.from(scaleProbe(context.nonce)));
  const tool=engine();write(context.directory,'engine.json',tool);
  const version=await runner.command(context,'scale-engine-version',tool.executable,['--headless','--version']);
  need(version.stdout.trim()===tool.version,'MEDIA_ENGINE_VERSION');
  const imported=await runner.command(context,'scale-native-import',tool.executable,['--headless','--path',project,'--editor','--import']);
  need(!/ERROR:|SCRIPT ERROR:|Failed to load|Error importing/.test(imported.stdout+imported.stderr),'SCALE_NATIVE_IMPORT');
  const run=await runner.command(context,'scale-native',tool.executable,['--headless','--path',project,'--script','res://probe.gd']);
  need(!/ERROR:|SCRIPT ERROR:/.test(run.stdout+run.stderr)&&run.stdout.includes('MEDIA_SCALE_COMPLETE '+context.nonce+' sprites=18'),'SCALE_NATIVE_PROOF');
  // Corrupt the actual scene in the owned proof project, then observe native rejection.
  const subject=inside(project,'inspection/ScaleComparison.tscn');
  fs.writeFileSync(subject,bytes(ROOT,SCALE_SCENE).toString('utf8').replace('texture_filter = 1','texture_filter = 2'));
  let failure;
  try{await runner.command(context,'scale-native-linear-negative',tool.executable,['--headless','--path',project,'--script','res://probe.gd']);}
  catch(error){failure=error.message;}
  const rejected=document(context.directory,'scale-native-linear-negative.command.json');
  need(failure?.startsWith('CHILD_FAILED')&&rejected.code===1&&rejected.stderr.includes('MEDIA_SCALE_REJECT FILTER')&&
    !rejected.stdout.includes('MEDIA_SCALE_COMPLETE'),'SCALE_NATIVE_NEGATIVE');
  write(context.directory,'native-negative.json',{name:'native-linear-filter',diagnostic:'MEDIA_SCALE_REJECT FILTER',expectedRejectionObserved:true});
  return {observed:run.stdout.trim(),sprites:18,headless:true,expectedRejected:1};
}
async function scale(context){
  const initial=inputs(SCALE_FILES);write(context.directory,'inputs.json',initial);
  write(context.directory,'verify-scale.py',Buffer.from(SCALE_VERIFY));
  const verified=await runner.command(context,'portable-scale',PYTHON,['-B',inside(context.directory,'verify-scale.py'),ROOT]);
  const actual=JSON.parse(verified.stdout);need(actual.parts===8&&actual.generatedMasters===4&&actual.expectedRejected===12,'SCALE_PORTABLE_PROOF');
  write(context.directory,'negative-controls.json',actual.controls);
  const native=await nativeScale(context);
  need(equal(initial,inputs(SCALE_FILES)),'SOURCE_CHANGED');
  return {...actual,native,expectedRejected:actual.expectedRejected+native.expectedRejected,
    scope:'Source production choice and real headless resource/scene loading; no actual rendered appearance judgment'};
}
runner.registerCase('media.scale',{group:'media',title:'Retained generated masters and donor-scale day/night native assembly',importance:'Critical',
  rationale:'Incorrect density, registration, alpha or missing original generation sources invalidates every later body/building fit',
  needsGameWindow:false,runGroup:'G0',order:21,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-02','AC-03','AC-05','AC-08','AC-19'],
  setup:'Tracked39-member pilot ZIP, actual atlas/scene/pack and common donor ZIP; installed pinned Python/Pillow and headless Godot4.7.2',
  actions:['Reproduce alpha cleanup/crop/nearest exports from all4 original generated masters and exact expanded prompts',
    'Validate actual donor wall/tower/base pixels and inspected reference identities','Reject missing masters/prompts/parts, density/ground/alpha/sampling/source faults',
    'Import/instantiate actual day/night scene and observe18 sprites, exact anchors/layers/nearest resources/contact shadows',
    'Corrupt actual native scene filtering and observe native rejection'],
  expected:'Reproducible common logical density with8 actual art parts,4 original masters and7 references;13 faults rejected; visual approval remains pending04-32',
  reset:'Fresh owned project and evidence; bounded children; no game window, user judgment or donor access',
  evidence:['inputs.json','portable-scale.command.json','negative-controls.json','scale-native-import.command.json','scale-native.command.json',
    'scale-native-linear-negative.command.json','native-negative.json','result.json']},scale);
Object.assign(module.exports,{SCALE_VERIFY,SCALE_FILES,scaleProbe,nativeScale,scale});
