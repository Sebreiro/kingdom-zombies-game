'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,need,inside,bytes,document,write,hash}=require('../content.cjs');
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
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
