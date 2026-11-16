'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {spawn,spawnSync}=require('node:child_process');
const {TextDecoder,isDeepStrictEqual:equal}=require('node:util');
const ROOT='D:/Work/Games/KingdomZombies';
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
const DONOR='D:/Work/Games/KindomCrown/recovered/resources';
const ID='common.bow';
const FILES={media:'src/godot-game/assets/common/bow.png',pack:'src/godot-game/content/packs/tracer.json',
  scene:'src/godot-game/inspection/Inspection.tscn',source:'production/phase4/tracer-source.json'};
const ORIGINALS=[
  {path:'sharedassets0.assets',bytes:58725896,sha256:'3648658d475170661517c4907e50977a4ab932441b404a681cd0ce485592f3ff'},
  {path:'sharedassets0.assets.resS',bytes:303786800,sha256:'2efccae88002a02ca85f050a6d6dc2e5fe894c32de80e36f30cd12f8c7ffb5c2'}];
const EXPECTED={owner:'sharedassets0.assets',type:'Sprite',objectId:7008,name:'tools_bow',
  atlasId:25376,atlasName:'objects',textureId:178,textureName:'sactx-0-1024x1024-Uncompressed-objects-07654c7b',
  stream:{path:'sharedassets0.assets.resS',offset:280718128,size:4194304}};
const IMAGE={width:16,height:18,mode:'RGBA',alphaBBox:[6,1,10,17],visiblePixels:42,
  rgbaSha256:'88b8e7062f0eb9cfb788ef3658028ba58a6f62d408d3e39ae2e1228bfa6c8905',
  pngSha256:'08b53699e459d71cf73f3e7be84af19d04805d5a7a488121e39a514a81701999',pngBytes:135};
const REGISTRATION={logicalRect:{x:0,y:0,width:16,height:18},pivotNormalizedBottomLeft:{x:0.375,y:0.5},
  sourcePixelsPerUnit:32,sourceOffset:{x:-2,y:0},trimOffset:{x:0,y:0},atlasRectOffset:{x:576,y:526},
  atlasTextureRect:{x:578,y:526,width:16,height:18},atlasSettingsRaw:67,alphaTextureId:0,
  display:{centered:false,offsetTopLeft:[-6,-9],scale:[1,1],filter:'nearest'}};
const RECIPE=String.raw`import base64, hashlib, io, json, pathlib, sys
sys.dont_write_bytecode = True
sys.path.insert(0, 'D:/Work/Games/KindomCrown/.tools/resource-python')
import UnityPy
from PIL import Image
assert UnityPy.__version__ == '1.23.0' and Image.__version__ == '13.0.0', 'AUTHORING_TOOL_PIN'
root = pathlib.Path('D:/Work/Games/KindomCrown/recovered/resources')
data = root / 'KingdomTwoCrowns_Data'
target = pathlib.Path(sys.argv[1]).resolve()
assert target.is_relative_to(pathlib.Path('D:/Work/Games/KingdomZombies/.phase4').resolve()), 'EXPORT_PATH'
sha = lambda b: hashlib.sha256(b).hexdigest()
manifest_bytes = (root / 'manifest.json').read_bytes()
manifest = json.loads(manifest_bytes)
originals = []
for name in ['sharedassets0.assets', 'sharedassets0.assets.resS']:
    entry = next(x for x in manifest['files'] if x['path'] == name)
    with (data / name).open('rb') as stream:
        actual = hashlib.file_digest(stream, 'sha256').hexdigest()
    assert actual == entry['sha256'] and (data / name).stat().st_size == entry['bytes'], 'PRESERVATION_MISMATCH'
    originals.append(entry)
env = UnityPy.load(str(data / 'sharedassets0.assets'))
owner = next(x for x in env.files.values() if pathlib.Path(x.name).name == 'sharedassets0.assets')
obj = owner.objects[7008]
assert obj.type.name == 'Sprite'
sprite = obj.read()
assert sprite.m_Name == 'tools_bow'
atlas = sprite.m_SpriteAtlas.read()
assert atlas.object_reader.path_id == 25376 and atlas.m_Name == 'objects'
render = next(v for k,v in atlas.m_RenderDataMap if k == sprite.m_RenderDataKey)
texture = render.texture.read()
assert texture.object_reader.path_id == 178 and texture.m_Name == 'sactx-0-1024x1024-Uncompressed-objects-07654c7b'
stream = texture.object_reader.read_typetree()['m_StreamData']
assert stream == {'offset':280718128, 'size':4194304, 'path':'sharedassets0.assets.resS'}
with (data / stream['path']).open('rb') as handle:
    handle.seek(stream['offset'])
    stream_bytes = handle.read(stream['size'])
image = sprite.image.convert('RGBA')
assert image.size == (16,18) and image.getbbox() == (6,1,10,17)
output = io.BytesIO()
image.save(output, format='PNG', optimize=False, compress_level=9)
png = output.getvalue()
raw = obj.get_raw_data()
tree = obj.read_typetree()
portable = lambda x: json.loads(json.dumps(x,default=lambda b:{'base64':base64.b64encode(b).decode('ascii')}))
rgba = image.tobytes()
result = {'schemaVersion':1, 'id':'common.bow', 'version':1, 'route':'reused-native-pixels',
 'source':{'owner':'sharedassets0.assets','type':'Sprite','objectId':7008,'name':'tools_bow',
 'atlasId':25376,'atlasName':'objects','textureId':178,'textureName':texture.m_Name,'stream':stream},
 'preservation':{'locator':str(root), 'manifestSha256':sha(manifest_bytes),'originals':originals,
 'streamRangeSha256':sha(stream_bytes),'rawSpriteSha256':sha(raw)},
 'registration':{'logicalRect':tree['m_Rect'],'pivotNormalizedBottomLeft':tree['m_Pivot'],
 'sourcePixelsPerUnit':tree['m_PixelsToUnits'],'sourceOffset':tree['m_Offset'],
 'trimOffset':tree['m_RD']['textureRectOffset'],'atlasRectOffset':{'x':render.atlasRectOffset.x,'y':render.atlasRectOffset.y},
 'atlasTextureRect':{'x':render.textureRect.x,'y':render.textureRect.y,'width':render.textureRect.width,'height':render.textureRect.height},
 'atlasSettingsRaw':render.settingsRaw,'alphaTextureId':render.alphaTexture.path_id,
 'display':{'centered':False,'offsetTopLeft':[-6,-9],'scale':[1,1],'filter':'nearest'}},
 'image':{'width':16,'height':18,'mode':'RGBA','alphaBBox':list(image.getbbox()),
 'visiblePixels':sum(1 for a in rgba[3::4] if a),'rgbaSha256':sha(rgba),'pngSha256':sha(png),'pngBytes':len(png)},
 'tools':{'UnityPy':UnityPy.__version__,'Pillow':Image.__version__,'python':sys.version.split()[0]},
 'bundle':{'originalExportPngBase64':base64.b64encode(png).decode('ascii'),
 'rawSpriteBase64':base64.b64encode(raw).decode('ascii'),'rawSpriteTree':portable(tree),
 'textureTree':portable(texture.object_reader.read_typetree())},
 'validation':{'sourceSelection':'exact-owner-type-id-name-and-dependencies-verified',
 'production':'source-headless-only','visualReview':'pending-04-32','listening':'not-applicable',
 'obligationCompletion':'not-claimed'}}
(target / 'bow.png').write_bytes(png)
(target / 'source.json').write_text(json.dumps(result,indent=2,allow_nan=False)+'\n',encoding='utf-8')
print(json.dumps({'id':'common.bow','pngSha256':sha(png),'rgbaSha256':sha(rgba)}))
`;
function hash(value){return crypto.createHash('sha256').update(value).digest('hex');}
function need(condition,code,detail=''){if(!condition)throw new Error(code+' '+ID+' '+detail);}
function inside(root,relative){
  need(typeof relative==='string'&&relative.length<240&&!relative.includes('\\')&&
    relative.split('/').every(x=>x.length>0&&x!=='.'&&x!=='..')&&!path.isAbsolute(relative)&&!relative.includes(':'),'PATH_INVALID',relative);
  const resolved=path.resolve(root,relative);need(resolved.startsWith(path.resolve(root)+path.sep),'PATH_ESCAPE',relative);
  let current=root;
  for(const segment of relative.split('/')){
    if(fs.existsSync(current))need(!fs.lstatSync(current).isSymbolicLink(),'PATH_REPARSE',relative);
    if(fs.existsSync(current))need(fs.readdirSync(current).includes(segment)||!fs.existsSync(path.join(current,segment)),'PATH_CASE',relative);
    current=path.join(current,segment);
  }
  if(fs.existsSync(resolved))need(!fs.lstatSync(resolved).isSymbolicLink(),'PATH_REPARSE',relative);
  return resolved;
}
function bytes(root,relative,limit=1048576){
  const file=inside(root,relative);need(fs.existsSync(file),'RESOURCE_MISSING',relative);
  const stat=fs.statSync(file);need(stat.isFile()&&stat.size>0&&stat.size<=limit,'RESOURCE_SIZE',relative);
  return fs.readFileSync(file);
}
function document(root,relative){
  const text=new TextDecoder('utf-8',{fatal:true}).decode(bytes(root,relative));
  need(!text.startsWith('\ufeff'),'UTF8_BOM',relative);
  let result;try{result=JSON.parse(text);}catch{throw new Error('JSON_INVALID '+ID+' '+relative);}
  const stack=[];
  for(const token of text.match(/"(?:[^"\\]|\\.)*"|[{}\[\]:,]/g)??[]){
    if(token==='{')stack.push({keys:new Set(),key:true});
    else if(token==='[')stack.push({});
    else if(token==='}'||token===']')stack.pop();
    else if(token===','&&stack.at(-1)?.keys)stack.at(-1).key=true;
    else if(token===':'&&stack.at(-1)?.keys)stack.at(-1).key=false;
    else if(token.startsWith('"')&&stack.at(-1)?.key){
      const key=JSON.parse(token),frame=stack.at(-1);need(!frame.keys.has(key),'JSON_DUPLICATE_KEY',relative+' '+key);frame.keys.add(key);
    }
  }
  function walk(value,depth){
    need(depth<=24,'DOCUMENT_DEPTH',relative);
    if(typeof value==='number')need(Number.isFinite(value),'DOCUMENT_NUMBER',relative);
    if(Array.isArray(value))need(value.length<=4096,'DOCUMENT_COUNT',relative);
    if(value&&typeof value==='object')for(const [key,item]of Object.entries(value)){
      need(!['__proto__','constructor','prototype'].includes(key),'DOCUMENT_KEY',relative);walk(item,depth+1);
    }
  }
  walk(result,0);return result;
}
function save(root,relative,value){
  const file=inside(root,relative);fs.mkdirSync(path.dirname(file),{recursive:true});
  fs.writeFileSync(file,typeof value==='string'||Buffer.isBuffer(value)?value:JSON.stringify(value,null,2)+'\n',{flag:'wx'});
}
function environment(directory){
  const env={...process.env};
  for(const key of Object.keys(env))if(/^(GODOT|DOTNET|NUGET|MSBUILD|COMPlus|CORECLR|XDG)_/i.test(key))delete env[key];
  for(const name of ['home','temp','data','config','cache','packages','http-cache'])fs.mkdirSync(path.join(directory,name),{recursive:true});
  return {...env,HOME:path.join(directory,'home'),USERPROFILE:path.join(directory,'home'),APPDATA:path.join(directory,'data'),
    LOCALAPPDATA:path.join(directory,'data'),XDG_DATA_HOME:path.join(directory,'data'),XDG_CONFIG_HOME:path.join(directory,'config'),
    XDG_CACHE_HOME:path.join(directory,'cache'),DOTNET_CLI_HOME:path.join(directory,'home'),NUGET_PACKAGES:path.join(directory,'packages'),
    NUGET_HTTP_CACHE_PATH:path.join(directory,'http-cache'),TEMP:path.join(directory,'temp'),TMP:path.join(directory,'temp'),
    PYTHONDONTWRITEBYTECODE:'1',GODOT_SILENCE_ROOT_WARNING:'1'};
}
function terminate(child){
  if(!child.pid||child.exitCode!==null||child.signalCode!==null)return;
  if(process.platform==='win32'){
    const killed=spawnSync('taskkill.exe',['/PID',String(child.pid),'/T','/F'],{windowsHide:true,encoding:'utf8',timeout:5000});
    need(!killed.error&&killed.status===0,'OWNED_CLEANUP_FAILED',killed.stderr);
  }else try{process.kill(-child.pid,'SIGKILL');}catch(error){if(error.code!=='ESRCH')throw error;}
}
async function command(context,name,executable,args,{timeout=60000,interruptMarker,env,cwd}={}){
  const startedAt=new Date().toISOString(),child=spawn(executable,args,{cwd:cwd??context.directory,env:env??environment(context.directory),
    windowsHide:true,shell:false,detached:process.platform!=='win32'});
  let stdout='',stderr='',spawnError,reason,cleanupError;
  function stop(code){if(reason)return;reason=code;try{terminate(child);}catch(error){cleanupError=error.message;}}
  child.stdout.on('data',part=>{stdout+=part.toString('utf8');if(stdout.length>1048576)stop('OUTPUT_LIMIT');
    if(interruptMarker&&stdout.includes(interruptMarker))stop('LOADER_INTERRUPTED');});
  child.stderr.on('data',part=>{stderr+=part.toString('utf8');if(stderr.length>1048576)stop('OUTPUT_LIMIT');});
  child.once('error',error=>{spawnError=error.message;});
  const timer=setTimeout(()=>stop('CHILD_TIMEOUT'),timeout);
  let emergency;
  const result=await Promise.race([
    new Promise(resolve=>child.once('close',(code,signal)=>resolve({code,signal,streamsClosed:true}))),
    new Promise(resolve=>{emergency=setTimeout(()=>resolve({code:null,signal:null,streamsClosed:false}),timeout+8000);})
  ]);
  clearTimeout(timer);clearTimeout(emergency);
  const record={name,executable,args,pid:child.pid,startedAt,finishedAt:new Date().toISOString(),...result,
    reason,spawnError,cleanupError,stdout,stderr};
  save(context.directory,name+'.command.json',record);save(context.directory,name+'.stdout.log',stdout);save(context.directory,name+'.stderr.log',stderr);
  need(!spawnError&&!reason&&!cleanupError&&result.streamsClosed&&result.code===0&&!result.signal,
    reason??'CHILD_FAILED',name+' '+JSON.stringify({code:result.code,spawnError,cleanupError}));
  return record;
}
function fingerprint(root){
  return Object.fromEntries([...Object.values(FILES),'tools/phase4/check.cjs','tools/phase1/toolchain.json','src/godot-game/project.godot']
    .map(file=>[file,hash(bytes(root,file))]));
}
function createRun(caseId){
  const nonce=crypto.randomUUID(),directory=inside(ROOT,'.phase4/runs/'+nonce);fs.mkdirSync(directory,{recursive:true});
  const revision=spawnSync('git',['rev-parse','HEAD'],{cwd:ROOT,windowsHide:true,encoding:'utf8',timeout:5000});
  if(revision.status!==0){save(directory,'setup-failure.json',{caseId,nonce,status:'fail',code:'REVISION_UNAVAILABLE',error:revision.error?.message,stderr:revision.stderr});
    need(false,'REVISION_UNAVAILABLE',revision.error?.message??revision.stderr);}
  const context={directory,nonce,caseId,revision:revision.stdout.trim(),startedAt:new Date().toISOString()};
  save(directory,'invocation.json',{...context,node:process.version,platform:process.platform,scope:'source/headless; zero game windows'});
  return context;
}
function engine(){
  const pins=document(ROOT,'tools/phase1/toolchain.json');need(process.platform==='win32'&&process.arch==='x64','PINNED_PLATFORM_REQUIRED');
  need(process.version==='v'+pins.node,'NODE_PIN');
  const pin=pins.archives['win32-x64'],relative='.phase1/tools/'+pin.directory+'/'+pin.executable;
  const executable=inside(ROOT,relative);need(hash(bytes(ROOT,relative,268435456))===pin.executableSha256,'ENGINE_HASH',relative);
  return {executable,version:pins.engineFullVersion,sha256:pin.executableSha256};
}
function validate(root){
  const source=document(root,FILES.source),pack=document(root,FILES.pack);
  need(source&&source.preservation&&source.recipe&&source.bundle&&source.image&&source.registration,
    'SOURCE_RECORD_INVALID',FILES.source);
  need(source.schemaVersion===1&&source.id===ID&&source.version===1&&equal(source.source,EXPECTED),
    'SOURCE_IDENTITY_MISMATCH',FILES.source);
  need(equal(source.preservation.originals,ORIGINALS),'SOURCE_IDENTITY_MISMATCH','originals');
  need(source.preservation.manifestSha256==='38a914a53ba9e3a618e6b97942ee3ae382ade702d6ce1f04485e583b4923d31d'&&
    source.preservation.streamRangeSha256==='620d3a6fd3f441ea4e02f07bef8479f118d451076e29917d292797babb3bad53'&&
    source.preservation.rawSpriteSha256==='12a7303fa0815fb8cd0ac10630e934dd91b87777c041838ada7cfa8259fa4023',
    'SOURCE_IDENTITY_MISMATCH','retained-source-hashes');
  need(equal(source.image,IMAGE),'MEDIA_IDENTITY_MISMATCH',FILES.media);
  need(equal(source.registration,REGISTRATION),'REGISTRATION_MISMATCH',FILES.source);
  need(source.recipe.code===RECIPE&&source.recipe.sha256===hash(RECIPE),'SOURCE_RECIPE_MISMATCH',FILES.source);
  const png=bytes(root,FILES.media,65536),retained=Buffer.from(source.bundle.originalExportPngBase64,'base64');
  need(hash(png)===source.image.pngSha256&&png.equals(retained),'MEDIA_HASH_MISMATCH',FILES.media);
  need(hash(Buffer.from(source.bundle.rawSpriteBase64,'base64'))===source.preservation.rawSpriteSha256,'SOURCE_IDENTITY_MISMATCH','rawSprite');
  need(pack&&pack.schemaVersion===1&&pack.id==='tracer'&&pack.version===1&&Array.isArray(pack.resources)&&
    pack.resources.length===1&&pack.resources[0],'PACK_INVALID',FILES.pack);
  const entry=pack.resources[0];
  need(entry.id===ID&&entry.path==='assets/common/bow.png'&&entry.scene==='inspection/Inspection.tscn'&&entry.version===1,
    'PACK_BINDING_MISMATCH',FILES.pack);
  need(entry.sha256===hash(png)&&equal(entry.source,EXPECTED)&&
    equal(entry.registration,source.registration)&&equal(entry.image,source.image),
    'SOURCE_IDENTITY_MISMATCH',FILES.pack);
  bytes(root,FILES.scene,65536);
  return {source,pack,png};
}
function probeScript(nonce,image,interrupt){return `extends SceneTree
func _initialize():
    call_deferred("inspect")
func reject(reason):
    push_error("TRACER_NATIVE_FAILURE ${nonce} common.bow inspection/Inspection.tscn " + reason)
    quit(1)
func inspect():
    var packed = load("res://inspection/Inspection.tscn")
    if not packed is PackedScene:
        reject("SCENE_LOAD")
        return
    var scene = packed.instantiate()
    root.add_child(scene)
    var sprite = scene.get_node_or_null("Bow")
    if not sprite is Sprite2D or sprite.texture == null:
        reject("TEXTURE_MISSING")
        return
    var image = sprite.texture.get_image()
    if image == null or image.get_width() != 16 or image.get_height() != 18:
        reject("IMAGE_DIMENSIONS")
        return
    image.convert(Image.FORMAT_RGBA8)
    var pixels = image.get_data()
    var original = Image.load_from_file("res://assets/common/bow.png")
    if original == null or original.get_width() != 16 or original.get_height() != 18:
        reject("ORIGINAL_DECODE")
        return
    original.convert(Image.FORMAT_RGBA8)
    var original_pixels = original.get_data()
    var digest = HashingContext.new()
    digest.start(HashingContext.HASH_SHA256)
    digest.update(original_pixels)
    if digest.finish().hex_encode() != "${image.rgbaSha256}":
        reject("IMAGE_PIXELS")
        return
    for i in range(0, pixels.size(), 4):
        if pixels[i+3] != original_pixels[i+3]:
            reject("IMPORTED_ALPHA")
            return
        if pixels[i+3] > 0 and (pixels[i] != original_pixels[i] or pixels[i+1] != original_pixels[i+1] or pixels[i+2] != original_pixels[i+2]):
            reject("IMPORTED_VISIBLE_PIXELS")
            return
    var visible = 0
    for i in range(3, pixels.size(), 4):
        if pixels[i] > 0:
            visible += 1
    if visible != ${image.visiblePixels} or visible == 0:
        reject("IMAGE_EMPTY")
        return
    if sprite.centered or sprite.offset != Vector2(-6, -9) or sprite.scale != Vector2.ONE or sprite.texture_filter != CanvasItem.TEXTURE_FILTER_NEAREST:
        reject("REGISTRATION")
        return
    if scene.get_node("Summary/ResourceId").text != "common.bow" or scene.get_node("Summary/ResourceName").text != "Bow" or scene.get_node("Summary/Status").text != "Source/headless verified; visual review pending":
        reject("SUMMARY_BINDING")
        return
    print("TRACER_LOADED ${nonce} common.bow inspection/Inspection.tscn pixels=" + str(visible))
    ${interrupt?'await create_timer(30).timeout':'await process_frame'}
    scene.free()
    print("TRACER_COMPLETE ${nonce} common.bow")
    quit(0)
`;}
async function runTracer(root,context,{interrupt=false}={}){
  const initial=fingerprint(root);save(context.directory,'inputs.json',initial);
  const {source}=validate(root),tool=engine();save(context.directory,'engine.json',tool);
  const version=await command(context,'engine-version',tool.executable,['--headless','--version']);
  need(version.stdout.trim()===tool.version,'ENGINE_VERSION');
  const project=inside(context.directory,'project');fs.mkdirSync(project);
  for(const [key,file]of Object.entries(FILES))if(key!=='source')save(project,file.replace('src/godot-game/',''),bytes(root,file));
  save(project,'project.godot','config_version=5\n[application]\nconfig/name="KingdomZombies tracer proof"\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n');
  save(project,'probe.gd',probeScript(context.nonce,source.image,interrupt));
  const imported=await command(context,'native-import',tool.executable,['--headless','--path',project,'--editor','--import']);
  need(!/ERROR:|SCRIPT ERROR:|Failed to load|Error importing/i.test(imported.stdout+imported.stderr),'NATIVE_IMPORT_ERROR',FILES.scene);
  let loaded;
  try{loaded=await command(context,'native-loader',tool.executable,['--headless','--path',project,'--script','res://probe.gd'],
    {timeout:15000,interruptMarker:interrupt?'TRACER_LOADED '+context.nonce:undefined});}
  catch(error){if(error.message.startsWith('CHILD_FAILED '))throw new Error('NATIVE_LOAD_ERROR '+ID+' '+FILES.scene+' '+error.message);throw error;}
  need(!/ERROR:|SCRIPT ERROR:|Failed to load/i.test(loaded.stdout+loaded.stderr),'NATIVE_LOAD_ERROR',FILES.scene);
  need(loaded.stdout.includes('TRACER_COMPLETE '+context.nonce+' '+ID),'NATIVE_COMPLETION_MISSING',FILES.scene);
  need(JSON.stringify(initial)===JSON.stringify(fingerprint(root)),'SOURCE_CHANGED');
  return {id:ID,mediaSha256:source.image.pngSha256,rgbaSha256:source.image.rgbaSha256,
    sceneSha256:initial[FILES.scene],engineSha256:tool.sha256,nativeCompletion:true,visiblePixels:source.image.visiblePixels};
}
async function exportTracer(){
  const context=createRun('authoring.export-tracer'),staging=inside(context.directory,'staging');fs.mkdirSync(staging);
  try{
    save(context.directory,'export.py',RECIPE);
    await command(context,'donor-export',PYTHON,['-B',path.join(context.directory,'export.py'),staging]);
    const source=document(staging,'source.json');
    need(equal(source.source,EXPECTED)&&equal(source.preservation.originals,ORIGINALS),
      'SOURCE_IDENTITY_MISMATCH','export');
    source.recipe={language:'Python 3.12',code:RECIPE,sha256:hash(RECIPE),transformations:[
      'UnityPy Sprite.image resolves atlas/tight packing/rotation/alpha; convert RGBA',
      'Preserve all 16x18 native pixels; lossless PNG compress_level=9; no resizing/recoloring',
      'Top-left Godot offset(-6,-9) preserves normalized bottom-left donor pivot(.375,.5) on full untrimmed canvas']};
    save(ROOT,FILES.media,bytes(staging,'bow.png',65536));save(ROOT,FILES.source,source);
    save(ROOT,FILES.pack,{schemaVersion:1,id:'tracer',version:1,status:'incomplete-phase-pack',resources:[{
      id:ID,name:'Bow',version:1,path:'assets/common/bow.png',scene:'inspection/Inspection.tscn',sha256:source.image.pngSha256,
      source:source.source,registration:source.registration,image:source.image,route:source.route,
      validation:{source:'verified-export',headless:'run-source.tracer-for-current-evidence',visual:'pending-04-32'},
      obligations:['IT-07'],obligationStatus:'partial-master-only'}]});
    save(context.directory,'result.json',{status:'pass',nonce:context.nonce,mediaSha256:source.image.pngSha256});
    console.log('EXPORTED '+source.image.pngSha256+' '+context.directory);
  }catch(error){save(context.directory,'result.json',{status:'fail',nonce:context.nonce,message:error.message});throw error;}
}
async function originalIdentities(){
  const rows=[];
  for(const expected of ORIGINALS){
    const file=inside(DONOR,'KingdomTwoCrowns_Data/'+expected.path),stat=fs.statSync(file);
    need(stat.isFile()&&stat.size===expected.bytes,'ORIGINAL_SIZE',expected.path);
    const digest=crypto.createHash('sha256');
    for await(const part of fs.createReadStream(file))digest.update(part);
    const row={path:expected.path,bytes:stat.size,sha256:digest.digest('hex')};
    need(equal(row,expected),'ORIGINAL_CHANGED',expected.path);rows.push(row);
  }
  return rows;
}
function copySubject(directory){
  const root=inside(directory,'subject');fs.mkdirSync(root);
  for(const file of [...Object.values(FILES),'tools/phase4/check.cjs','tools/phase1/toolchain.json','src/godot-game/project.godot'])
    save(root,file,bytes(ROOT,file));
  return root;
}
async function tracerFailures(context){
  const initial=fingerprint(ROOT),originalsBefore=await originalIdentities(),outcomes=[];
  save(context.directory,'inputs.json',initial);save(context.directory,'originals-before.json',originalsBefore);
  const trials=[
    {name:'missing-media',file:FILES.media,code:'RESOURCE_MISSING',remove:true},
    {name:'corrupt-media',file:FILES.media,code:'MEDIA_HASH_MISMATCH',payload:Buffer.from('not a PNG')},
    {name:'missing-manifest',file:FILES.pack,code:'RESOURCE_MISSING',remove:true},
    {name:'corrupt-manifest',file:FILES.pack,code:'JSON_INVALID',payload:Buffer.from('{"invalid":')},
    {name:'duplicate-manifest-key',file:FILES.pack,code:'JSON_DUPLICATE_KEY',payload:Buffer.from('{"id":"tracer","id":"other"}')},
    {name:'origin-id',file:FILES.pack,code:'SOURCE_IDENTITY_MISMATCH',edit:value=>{value.resources[0].source.objectId=7009;}},
    {name:'origin-hash',file:FILES.source,code:'SOURCE_IDENTITY_MISMATCH',edit:value=>{value.preservation.originals[0].sha256='0'.repeat(64);}},
    {name:'missing-scene',file:FILES.scene,code:'RESOURCE_MISSING',remove:true},
    {name:'corrupt-scene',file:FILES.scene,code:'NATIVE_IMPORT_ERROR',payload:Buffer.from('[gd_scene format=3]\n[node name="broken" type=\n')},
    {name:'native-missing-texture',file:FILES.scene,code:'NATIVE_LOAD_ERROR',
      payload:Buffer.from(bytes(ROOT,FILES.scene).toString('utf8').replace('res://assets/common/bow.png','res://assets/common/absent-bow.png'))},
    {name:'interrupted-loader',file:FILES.scene,code:'LOADER_INTERRUPTED',interrupt:true}
  ];
  let missingSubject;
  for(const trial of trials){
    const invocation=createRun('source.tracer-failures/'+trial.name),subject=copySubject(invocation.directory),file=inside(subject,trial.file);
    if(trial.remove)fs.unlinkSync(file);
    if(trial.payload)fs.writeFileSync(file,trial.payload);
    if(trial.edit){const value=document(subject,trial.file);trial.edit(value);fs.writeFileSync(file,JSON.stringify(value,null,2)+'\n');}
    const mutation={id:ID,trial:trial.name,path:trial.file,expectedCode:trial.code,
      originalSha256:initial[trial.file],mutatedSha256:fs.existsSync(file)?hash(fs.readFileSync(file)):null};
    if(fs.existsSync(file))save(invocation.directory,'mutation.bin',fs.readFileSync(file));
    save(invocation.directory,'mutation.json',mutation);
    let failure;
    try{await runTracer(subject,invocation,{interrupt:trial.interrupt});}
    catch(error){failure=error;}
    save(invocation.directory,'result.json',{...invocation,status:failure?'fail':'pass',
      message:failure?.message,stack:failure?.stack,mutation,finishedAt:new Date().toISOString()});
    need(failure&&failure.message.startsWith(trial.code+' '+ID+' '),'NEGATIVE_CONTROL_WRONG_RESULT',trial.name+' '+failure?.message);
    if(trial.interrupt){
      const native=document(invocation.directory,'native-loader.command.json');
      need(native.reason==='LOADER_INTERRUPTED'&&native.streamsClosed&&!native.cleanupError&&
        native.stdout.includes('TRACER_LOADED '+invocation.nonce+' '+ID)&&
        !native.stdout.includes('TRACER_COMPLETE '+invocation.nonce),'INTERRUPTION_NOT_OBSERVED',trial.name);
    }else need(failure.message.includes(trial.file)||trial.name==='origin-hash','DIAGNOSTIC_PATH_MISSING',trial.name);
    outcomes.push({trial:trial.name,nonce:invocation.nonce,directory:invocation.directory,
      subjectStatus:'fail',expectedRejectionObserved:true,diagnostic:failure.message,mutation});
    if(trial.name==='missing-media')missingSubject=subject;
  }
  need(missingSubject,'RESTORE_SUBJECT_MISSING');
  save(missingSubject,FILES.media,bytes(ROOT,FILES.media));
  need(equal(fingerprint(missingSubject),initial),'RESTORE_INPUT_MISMATCH');
  const restored=createRun('source.tracer-failures/restored-control'),restoredResult=await runTracer(missingSubject,restored);
  save(restored.directory,'result.json',{...restored,status:'pass',observations:restoredResult,restoredFrom:outcomes[0].nonce});
  const production=createRun('source.tracer-failures/production-control'),productionResult=await runTracer(ROOT,production);
  save(production.directory,'result.json',{...production,status:'pass',observations:productionResult});
  const originalsAfter=await originalIdentities();save(context.directory,'originals-after.json',originalsAfter);
  need(equal(originalsBefore,originalsAfter)&&equal(fingerprint(ROOT),initial),'ORIGINAL_CHANGED','production-or-donor');
  return {outcomes,expectedRejected:trials.length,restored:{nonce:restored.nonce,...restoredResult},
    production:{nonce:production.nonce,...productionResult},originalsUnchanged:true};
}
const cases=new Map();
function registerCase(id,definition,run){
  need(typeof id==='string'&&!cases.has(id)&&typeof run==='function','CASE_REGISTRATION',id);
  cases.set(id,{id,...definition,run});
}
registerCase('source.tracer',{group:'source',title:'Exact recovered bow reaches native scene',importance:'High',
  rationale:'Incorrect media/origin/native binding invalidates the production slice',needsGameWindow:false,runGroup:'G0',order:1,
  requirements:['ASSET-01','ASSET-02','ASSET-05'],acceptance:['AC-02','AC-03','AC-16','AC-20'],
  setup:'Tracked bow, source record, shard and scene; authenticated pinned Windows engine; no donor folder required',
  actions:['Validate exact retained identity/media/recipe','Copy production subject to fresh owned project','Import and instantiate the exact native scene','Observe decoded pixels, registration, labels and unique completion'],
  expected:'Current nonempty source-bound texture loads; native errors/timeout/missing completion fail',
  reset:'Fresh owned no-window project per invocation; bounded descendant cleanup',evidence:['inputs.json','engine.json','native-import.command.json','native-loader.command.json','result.json']},
  context=>runTracer(ROOT,context));
registerCase('source.tracer-failures',{group:'source',title:'Bad real tracer subjects fail with fresh exact diagnostics',importance:'High',
  rationale:'Missing/corrupt/origin-mismatched/interrupted content must never inherit success',needsGameWindow:false,runGroup:'G0',order:2,
  requirements:['ASSET-01','ASSET-02','ASSET-05'],acceptance:['AC-02','AC-03','AC-16','AC-20'],
  setup:'Passing source.tracer prerequisites plus read-only preserved donor archive/stream for before/after identity observation',
  actions:['Hash original donor and production inputs','Mutate owned media/manifest/origin/scene copies independently',
    'Run the same production validator/importer/loader and require exact failed invocations','Interrupt only after native texture load is observed',
    'Restore the owned missing-media subject and load it afresh','Load unchanged production and rehash originals'],
  expected:'Eleven fresh deliberate failures; fresh restored and production passes; original bytes unchanged',
  reset:'Copies are run-owned; restore only missing media in its disposable copy; preserve all failure records; bounded descendant cleanup',
  evidence:['inputs.json','originals-before.json','originals-after.json','mutation.json','mutation.bin','native-loader.command.json','result.json']},tracerFailures);
async function main(args){
  need(fs.realpathSync.native(ROOT).toLowerCase()===fs.realpathSync.native(path.resolve(__dirname,'../..')).toLowerCase(),'PROJECT_ROOT_PIN');
  if(args.length===1&&args[0]==='--export-tracer')return exportTracer();
  if(args.length===1&&args[0]==='--list'){console.log(JSON.stringify([...cases.values()].map(({run,...item})=>item),null,2));return;}
  need(args.length===2&&['--case','--group'].includes(args[0]),'USAGE','--case ID | --group GROUP | --list | --export-tracer');
  const selected=args[0]==='--case'?[cases.get(args[1])]:[...cases.values()].filter(item=>item.group===args[1]);
  need(selected.length>0&&selected.every(Boolean),'CASE_PENDING_OR_UNKNOWN',args[1]);
  let failed=false;
  for(const item of selected){
    const context=createRun(item.id);let result;
    try{result={status:'pass',observations:await item.run(context)};}
    catch(error){result={status:'fail',message:error.message,stack:error.stack};failed=true;}
    save(context.directory,'result.json',{...context,...result,finishedAt:new Date().toISOString(),scope:'source/headless only; acceptance pending 04-32'});
    console.log(result.status.toUpperCase()+' '+item.id+' '+context.directory);if(result.message)console.error(result.message);
  }
  if(failed)process.exitCode=1;
}
module.exports={main,registerCase,cases,runTracer,validate,hash,inside,document,command,environment};
require('./cases/provenance.cjs');
require('./cases/audio-feasibility.cjs');
require('./cases/export-layout.cjs');
require('./cases/export-profile.cjs');
if(require.main===module)main(process.argv.slice(2)).catch(error=>{console.error(error.stack);process.exitCode=1;});
