'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {spawn,spawnSync}=require('node:child_process');
const {TextDecoder,isDeepStrictEqual:equal}=require('node:util');
const {createCaseRegistry,loadCaseModules}=require('./case-discovery.cjs');
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
// Godot 4.7.2 on Windows builds each DirAccessWindows by reading the process cwd with two unlocked GetCurrentDirectoryW calls (size, then fill).
// It ignores a failed fill. change_dir switches the process cwd for a moment under a lock the constructor does not take.
// When an import worker thread makes the cwd longer between the two calls, the fill writes nothing.
// The uninitialized buffer is then decoded as UTF-16, which prints the Unicode parsing errors.
// One import thread (editor/import/use_multiple_threads=false) removes the race. The imported files are byte-identical.
// The editor ignores override.cfg, so the setting goes into the run-owned project.godot of every --import command.
// The block is appended last because Godot keeps the last value it reads. Only a file that already ends with it is left unchanged.
const EDITOR_IMPORT_THREADS='editor/import/use_multiple_threads';
function editorImportPolicy(base,cwd,name,args){
  if(!args.includes('--import'))return;
  const at=args.indexOf('--path');
  need(at>=0&&args.indexOf('--path',at+1)<0&&typeof args[at+1]==='string'&&args[at+1].length>0&&!args.includes('--main-pack')&&
    !args.some(arg=>/(^|[\\/])project\.(godot|binary)$/i.test(arg)),'EDITOR_IMPORT_PROJECT',name);
  const project=path.resolve(cwd,args[at+1]);need(project.startsWith(base+path.sep),'EDITOR_IMPORT_PROJECT',name);
  let file,data,text;
  try{inside(base,path.relative(base,project).replaceAll(path.sep,'/'));if(!fs.statSync(project).isDirectory())throw new Error('not a directory');
    if(fs.existsSync(path.join(project,'project.binary')))throw new Error('project.binary would be loaded instead of project.godot');
    file=inside(project,'project.godot');const stat=fs.lstatSync(file);if(!stat.isFile()||stat.nlink!==1)throw new Error('project.godot is not a single-link regular file');
    data=bytes(project,'project.godot');text=new TextDecoder('utf-8',{fatal:true}).decode(data);}
  catch(error){need(false,'EDITOR_IMPORT_PROJECT',name+' '+error.message);}
  // Keys are read like Godot's VariantParser: a quoted key is unquoted, any other key loses all whitespace, and a header ends at its first ']'.
  // Feature-tag overrides such as use_multiple_threads.windows beat the base key, so every assignment found must be false.
  let section='';const values=[];
  for(const raw of text.split(/\r?\n/)){const line=raw.trim();if(!line||line.startsWith(';'))continue;
    const close=line.startsWith('[')?line.indexOf(']'):-1;if(close>0){section=line.slice(1,close).trim();continue;}
    const eq=line.indexOf('=');if(eq<0)continue;const written=line.slice(0,eq).trim();
    const key=/^".*"$/.test(written)?written.slice(1,-1):written.replace(/[\x00-\x20]/g,''),full=section?section+'/'+key:key;
    if(full===EDITOR_IMPORT_THREADS||full.startsWith(EDITOR_IMPORT_THREADS+'.'))values.push(line.slice(eq+1).replace(/;.*$/,'').trim());}
  need(values.every(value=>value==='false'),'EDITOR_IMPORT_THREADED',name);
  const eol=text.includes('\r\n')?'\r\n':'\n',block='[editor]'+eol+eol+'import/use_multiple_threads=false'+eol;
  if(text===block||text.endsWith(eol+block))return;
  fs.appendFileSync(file,(data.at(-1)===0x0a?'':eol)+eol+block);
}
async function command(context,name,executable,args,{timeout=60000,interruptMarker,env,cwd}={}){
  need(Number.isInteger(timeout)&&timeout>0&&timeout<=60000,'COMMAND_TIMEOUT_RANGE',name);
  const childCwd=cwd??context.directory,base=path.resolve(context.directory),resolved=path.resolve(childCwd);
  need(resolved===base||resolved.startsWith(base+path.sep),'COMMAND_CWD_OWNERSHIP',name);
  if(resolved!==base)inside(base,path.relative(base,resolved).replaceAll(path.sep,'/'));
  need(fs.statSync(resolved).isDirectory(),'COMMAND_CWD_DIRECTORY',name);
  need(env===undefined||(env!==null&&typeof env==='object'&&!Array.isArray(env)&&
    Object.values(env).every(value=>typeof value==='string'||value===undefined)),'COMMAND_ENV_INVALID',name);
  editorImportPolicy(base,resolved,name,args);
  const childEnv=env??environment(context.directory);
  const startedAt=new Date().toISOString(),child=spawn(executable,args,{cwd:childCwd,env:childEnv,
    windowsHide:true,shell:false,detached:process.platform!=='win32'});
  let stdout='',stderr='',spawnError,reason,cleanupError;
  function stop(code){if(reason)return;reason=code;try{terminate(child);}catch(error){cleanupError=error.message;}}
  child.stdout.on('data',part=>{stdout+=part.toString('utf8');if(stdout.length>1048576)stop('OUTPUT_LIMIT');
    if(interruptMarker&&stdout.includes(interruptMarker))stop('LOADER_INTERRUPTED');});
  child.stderr.on('data',part=>{stderr+=part.toString('utf8');if(stderr.length>1048576)stop('OUTPUT_LIMIT');});
  child.once('error',error=>{spawnError=error.message;});
  child.once('exit',(code,signal)=>save(context.directory,name+'.exit.json',{pid:child.pid,code,signal,exitedAt:new Date().toISOString()}));
  const timer=setTimeout(()=>stop('CHILD_TIMEOUT'),timeout);
  let emergency;
  const result=await Promise.race([
    new Promise(resolve=>child.once('close',(code,signal)=>resolve({code,signal,streamsClosed:true}))),
    new Promise(resolve=>{emergency=setTimeout(()=>resolve({code:null,signal:null,streamsClosed:false}),timeout+8000);})
  ]);
  clearTimeout(timer);clearTimeout(emergency);
  const record={name,executable,args,pid:child.pid,cwd:childCwd,environmentKeys:Object.keys(childEnv).sort(),timeout,
    startedAt,finishedAt:new Date().toISOString(),...result,
    reason,spawnError,cleanupError,stdout,stderr};
  save(context.directory,name+'.command.json',record);save(context.directory,name+'.stdout.log',stdout);save(context.directory,name+'.stderr.log',stderr);
  need(!spawnError&&!reason&&!cleanupError&&result.streamsClosed&&result.code===0&&!result.signal,
    reason??'CHILD_FAILED',name+' '+JSON.stringify({code:result.code,spawnError,cleanupError}));
  return record;
}
function fingerprint(root){
  const paths=[...Object.values(FILES),'tools/phase4/check.cjs','tools/phase1/toolchain.json','src/godot-game/project.godot','src/godot-game/content/pack-index.json','src/godot-game/content/expected-obligations.json','src/core/ContentPack.cs'];
  for(const directory of ['src/godot-game/inspection','src/godot-game/foundation','src/godot-game/.godot/mono/temp/bin/Debug'])for(const file of fs.readdirSync(inside(root,directory)))if(fs.statSync(inside(root,directory+'/'+file)).isFile()&&(file.endsWith('.cs')||directory.endsWith('/Debug')))paths.push(directory+'/'+file);
  return Object.fromEntries([...new Set(paths)].sort().map(file=>[file,hash(bytes(root,file,
    file.startsWith('src/godot-game/.godot/mono/temp/bin/Debug/')?16777216:1048576))]));
}
function createRun(caseId){
  const nonce=crypto.randomUUID(),directory=inside(ROOT,'.phase4/runs/'+nonce);fs.mkdirSync(directory,{recursive:true});
  const revision=spawnSync('git',['rev-parse','HEAD'],{cwd:ROOT,windowsHide:true,encoding:'utf8',timeout:5000});
  if(revision.status!==0){save(directory,'setup-failure.json',{caseId,nonce,status:'fail',code:'REVISION_UNAVAILABLE',error:revision.error?.message,stderr:revision.stderr});
    need(false,'REVISION_UNAVAILABLE',revision.error?.message??revision.stderr);}
  const context={directory,nonce,caseId,revision:revision.stdout.trim(),startedAt:new Date().toISOString()};
  save(directory,'invocation.json',{...context,node:process.version,platform:process.platform,caseModules,
    scope:'source/headless; zero game windows'});
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
  need(equal(entry.obligations,['IT-07'])&&entry.obligationStatus==='partial-master-only','RETAINED_OBLIGATION_MAPPING',FILES.pack);
  need(entry.sha256===hash(png)&&equal(entry.source,EXPECTED)&&
    equal(entry.registration,source.registration)&&equal(entry.image,source.image),
    'SOURCE_IDENTITY_MISMATCH',FILES.pack);
  bytes(root,FILES.scene,65536);
  return {source,pack,png};
}
function probeScript(nonce,image,interrupt){return `extends SceneTree
var stage_index = 0
func stage(name,observed):
    var file = FileAccess.open("res://tracer-stage-" + str(stage_index) + ".json",FileAccess.WRITE)
    file.store_string(JSON.stringify({"pid":OS.get_process_id(),"nonce":"${nonce}","stage":name,"observed":observed,"ticksMsec":Time.get_ticks_msec()}))
    file.close()
    stage_index += 1
func _initialize():
    stage("initialize",{"scope":"Actual bounded startup; census owns exhaustive selection"})
    call_deferred("inspect")
func reject(reason):
    push_error("TRACER_NATIVE_FAILURE ${nonce} common.bow inspection/Inspection.tscn " + reason)
    quit(1)
func inspect():
    stage("scene-load-start",{"path":"res://inspection/Inspection.tscn"})
    var packed = load("res://inspection/Inspection.tscn")
    if not packed is PackedScene:
        reject("SCENE_LOAD")
        return
    var scene = packed.instantiate()
    root.add_child(scene)
    stage("scene-started",{"script":str(scene.get_script()),"status":scene.get_node("Summary/Status").text})
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
    var raw = FileAccess.get_file_as_bytes("res://assets/common/bow.png")
    if raw.size() != 135:
        reject("ORIGINAL_BOUNDED_BYTES")
        return
    var original = Image.new()
    var decoded = original.load_png_from_buffer(raw)
    if decoded != OK:
        reject("ORIGINAL_DECODE")
        return
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
    stage("native-bow",{"visiblePixels":visible,"size":[image.get_width(),image.get_height()],"registration":[sprite.offset.x,sprite.offset.y]})
    while scene.get_node("Summary/Status").text.begins_with("Loading"):
        await process_frame
    stage("startup-ready",{"packStatus":scene.get_node("Summary/PackStatus").text,"runtime":str(scene.get_meta("inspection_runtime", "")),"diagnostics":scene.get_node("Shell/Workspace/Preview/Failures").text})
    if not scene.get_node("Summary/PackStatus").text.begins_with("180/180 source-ready · 0/180 accepted"):
        reject("CURRENT_SOURCE_READINESS " + scene.get_node("Summary/PackStatus").text)
        return
    var runtime = str(scene.get_meta("inspection_runtime", ""))
    var parts = runtime.split("|")
    if parts.size() != 5 or parts[0] != "KingdomZombies.Inspection.InspectionRoot" or parts[3] == "" or parts[3].contains("DefaultAssemblyLoadContext") or parts[4] != "True":
        reject("ACTUAL_CORE_GAME_ALC " + runtime)
        return
    var category = scene.get_node("Shell/Workspace/Navigation/Category")
    for index in range(category.item_count):
        if category.get_item_text(index) == "Register":
            category.select(index)
            category.item_selected.emit(index)
            break
    var list = scene.get_node("Shell/Workspace/Controls/Browser/Resources")
    var authority = JSON.parse_string(FileAccess.get_file_as_string("res://content/expected-obligations.json"))
    var expected_ids = authority.obligations.map(func(row): return row.id)
    var actual_ids = []
    for index in range(list.item_count):
        actual_ids.append(list.get_item_text(index).split(" · ")[0])
    actual_ids.sort()
    expected_ids.sort()
    var inspection_ids = ["world.assembled", "world.effects", "world.portals", "world.wildlife"]
    var membership = {"pid":OS.get_process_id(),"nonce":"${nonce}","runtime":runtime,"actualRegister":actual_ids,"independentExpected":expected_ids,"inspectionIds":inspection_ids,"inspectionAuthority":"WorldPreview.InspectorIds(Buildings/world); Register passes empty category and includes no authored world views","accepted":0}
    var membership_output = FileAccess.open("res://tracer-register.json",FileAccess.WRITE)
    membership_output.store_string(JSON.stringify(membership))
    membership_output.close()
    if actual_ids != expected_ids or actual_ids.size() != 180 or not expected_ids.all(func(id): return actual_ids.count(id) == 1):
        reject("CURRENT_INDEPENDENT_IDS")
        return
    stage("register-validated",membership)
    var pack_bytes = FileAccess.get_file_as_bytes("res://content/packs/tracer.json")
    stage("retained-mapping-read",{"bytes":pack_bytes.size()})
    if pack_bytes.size() <= 0 or pack_bytes.size() > 65536:
        reject("RETAINED_PACK_BOUNDS")
        return
    var pack = JSON.parse_string(pack_bytes.get_string_from_utf8())
    if pack == null or pack.id != "tracer" or pack.resources.size() != 1:
        reject("RETAINED_PACK_IDENTITY")
        return
    var retained = pack.resources[0]
    stage("retained-mapping",retained)
    if retained.id != "common.bow" or retained.path != "assets/common/bow.png" or retained.obligations != ["IT-07"] or retained.obligationStatus != "partial-master-only" or retained.source.objectId != 7008:
        reject("RETAINED_SOURCE_MAPPING")
        return
    var receipt = {"pid":OS.get_process_id(),"nonce":"${nonce}","runtime":runtime,"actualRegister":actual_ids,"independentExpected":expected_ids,"retainedMappings":retained.obligations,"bowPixels":visible,"accepted":0,"packStatus":scene.get_node("Summary/PackStatus").text,"reachabilityScope":"Startup/current180Register/retainedbow only; exhaustive180+fourviews belongs content.census"}
    var output = FileAccess.open("res://tracer-runtime.json",FileAccess.WRITE)
    output.store_string(JSON.stringify(receipt))
    output.close()
    image = null
    original = null
    print("TRACER_LOADED ${nonce} common.bow inspection/Inspection.tscn pixels=" + str(visible))
    ${interrupt?'await create_timer(30).timeout':'await process_frame'}
    stage("closing",{"sceneId":scene.get_instance_id(),"spriteId":sprite.get_instance_id()})
    scene.free()
    stage("closed",{"sceneValid":is_instance_valid(scene),"spriteValid":is_instance_valid(sprite)})
    if is_instance_valid(scene) or is_instance_valid(sprite):
        reject("OWNED_SCENE_RETIREMENT")
        return
    print("TRACER_COMPLETE ${nonce} common.bow")
    quit(0)
`;}
async function runTracer(root,context,{interrupt=false}={}){
  const initial=fingerprint(root);save(context.directory,'inputs.json',initial);
  const {source}=validate(root),tool=engine(),pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'];
  const executable=path.join(path.dirname(tool.executable),pin.executable.replace('_console.exe','.exe')),archive=inside(ROOT,'.phase1/cache/'+pin.file),entry=pin.directory+'/'+path.basename(executable);
  need(hash(fs.readFileSync(archive))===pin.sha256,'ENGINE_ARCHIVE_HASH');
  const auth=await command(context,'engine-auth',PYTHON,['-B','-c','import sys,zipfile,hashlib; z=zipfile.ZipFile(sys.argv[1]); r=[x for x in z.infolist() if x.filename==sys.argv[2]]; assert len(r)==1 and 0<r[0].file_size<=268435456; print(hashlib.sha256(z.read(r[0])).hexdigest())',archive,entry]);
  need(hash(fs.readFileSync(executable))===auth.stdout.trim(),'ENGINE_NATIVE_ENTRY_HASH');tool.executable=executable;tool.sha256=auth.stdout.trim();save(context.directory,'engine.json',{...tool,archiveSha256:pin.sha256,entry});
  const version=await command(context,'engine-version',tool.executable,['--headless','--version']);need(version.stdout.trim()===tool.version,'ENGINE_VERSION');
  const compiledDirectory=inside(context.directory,'tracer-compiled');fs.mkdirSync(compiledDirectory);const compiled=await require('./cases/data-camp.cjs').compiledRepair({...context,directory:compiledDirectory},false);
  const project=inside(context.directory,'project');fs.mkdirSync(project);
  for(const file of ['project.godot','inspection','foundation','content','assets','.godot/mono/temp/bin/Debug'])fs.cpSync(inside(root,'src/godot-game/'+file),inside(project,file),{recursive:true});
  save(project,'probe.gd',probeScript(context.nonce,source.image,interrupt));
  const imported=await command(context,'native-import',tool.executable,['--headless','--path',project,'--editor','--import','--quit']);
  need(imported.stderr.trim()===''&&!/Unicode parsing error|ERROR:|SCRIPT ERROR:|FATAL|WARNING:|leaked/i.test(imported.stdout+imported.stderr),'NATIVE_IMPORT_ERROR',FILES.scene);
  let loaded;
  try{loaded=await command(context,'native-loader',tool.executable,['--headless','--path',project,'--script','res://probe.gd'],
    {timeout:15000,interruptMarker:interrupt?'TRACER_LOADED '+context.nonce:undefined});}
  catch(error){if(error.message.startsWith('CHILD_FAILED '))throw new Error('NATIVE_LOAD_ERROR '+ID+' '+FILES.scene+' '+error.message);throw error;}
  need(loaded.stderr.trim()===''&&!/Unicode parsing error|ERROR:|SCRIPT ERROR:|FATAL|WARNING:|leaked|Failed to load/i.test(loaded.stdout+loaded.stderr),'NATIVE_LOAD_ERROR',FILES.scene);
  const receipt=document(project,'tracer-runtime.json'),parts=receipt.runtime.split('|');need(receipt.pid===loaded.pid&&receipt.nonce===context.nonce&&receipt.actualRegister.length===180&&equal(receipt.actualRegister,receipt.independentExpected)&&receipt.accepted===0&&equal(receipt.retainedMappings,['IT-07'])&&receipt.bowPixels===42&&compiled.observations.every(x=>x.gameMvid===parts[1]&&x.coreMvid===parts[2]),'NATIVE_RUNTIME_ASSEMBLY_IDENTITY');save(context.directory,'runtime-observation.json',receipt);
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
  for(const file of ['project.godot','inspection','foundation','content','assets','.godot/mono/temp/bin/Debug'])fs.cpSync(inside(ROOT,'src/godot-game/'+file),inside(root,'src/godot-game/'+file),{recursive:true});
  for(const file of ['production/phase4/tracer-source.json','tools/phase4/check.cjs','tools/phase1/toolchain.json','src/core/ContentPack.cs'])save(root,file,bytes(ROOT,file));
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
const registry=createCaseRegistry(),{cases,registerCase}=registry;
let caseModules;
const ordinal=(a,b)=>a<b?-1:a>b?1:0;
const caseOrder=(a,b)=>ordinal(a.runGroup,b.runGroup)||a.order-b.order||ordinal(a.id,b.id);
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
  caseModules??=loadCaseModules(path.join(__dirname,'cases'),registry);
  if(args.length===1&&args[0]==='--list'){console.log(JSON.stringify([...cases.values()].sort(caseOrder).map(({run,...item})=>item),null,2));return;}
  need(args.length===2&&['--case','--group'].includes(args[0]),'USAGE','--case ID | --group GROUP | --list | --export-tracer');
  const selected=args[0]==='--case'?[cases.get(args[1])]:[...cases.values()].filter(item=>item.group===args[1]);
  need(selected.length>0&&selected.every(Boolean),'CASE_PENDING_OR_UNKNOWN',args[1]);
  let failed=false;
  for(const item of selected.sort(caseOrder)){
    const context=createRun(item.id);let result;
    try{result={status:'pass',observations:await item.run(context)};}
    catch(error){result={status:'fail',message:error.message,stack:error.stack};failed=true;}
    save(context.directory,'result.json',{...context,...result,finishedAt:new Date().toISOString(),scope:'source/headless only; acceptance pending 04-32'});
    console.log(result.status.toUpperCase()+' '+item.id+' '+context.directory);if(result.message)console.error(result.message);
  }
  if(failed)process.exitCode=1;
}
function editorImportSelfTest(){
  const {test}=require('node:test'),assert=require('node:assert/strict');
  const STAND_IN="const fs=require('fs'),p=require('path'),a=process.argv,i=a.indexOf('--path'),v=i<0?'':a[i+1]||'',f=v?p.join(v,'project.godot'):'';if(f&&fs.existsSync(f))process.stdout.write(fs.readFileSync(f).toString('base64'));";
  const BASE='; Engine configuration file.\n\nconfig_version=5\n\n[application]\n\nconfig/name="EditorImportFixture"\n',BLOCK='\n[editor]\n\nimport/use_multiple_threads=false\n';
  const FALSE_EDITOR=BASE+BLOCK,FALSE_ROOT=BASE.replace('config_version=5\n','config_version=5\neditor/import/use_multiple_threads=false\n'),FALSE_SECTION=BASE+'\n[editor/import]\n\nuse_multiple_threads=false\n';
  const TRUE_EDITOR=FALSE_EDITOR.replace('=false','=true'),TRUE_ROOT=FALSE_ROOT.replace('=false','=true'),MIXED=FALSE_EDITOR+'\n[editor/import]\n\nuse_multiple_threads=true\n';
  const FALSE_COMMENT=BASE+'\n[editor]\n\nimport/use_multiple_threads=false ; one import thread\n',FALSE_OVERRIDE=BASE+'\n[editor]\n\nimport/use_multiple_threads.windows=false\n';
  const CRLF=BASE.replaceAll('\n','\r\n'),NO_FINAL=BASE.slice(0,-1);
  const importArgs=project=>['--headless','--path',project,'--editor','--import','--quit'];
  const scratch=body=>async()=>{const dir=path.join(ROOT,'.phase4/tdd/editor-import',crypto.randomUUID());fs.mkdirSync(dir,{recursive:true});
    try{await body(dir);}finally{fs.rmSync(dir,{recursive:true,force:true,maxRetries:3});}};
  const setup=(dir,scenario,text)=>{const run=path.join(dir,scenario,'run'),project=path.join(run,'project'),file=path.join(project,'project.godot');
    fs.mkdirSync(project,{recursive:true});if(text!==undefined)fs.writeFileSync(file,text);return {scenario:path.join(dir,scenario),run,project,file};};
  const start=(directory,name,args)=>command({directory},name,process.execPath,['-e',STAND_IN,'--',...args],{timeout:20000});
  const seen=record=>Buffer.from(record.stdout,'base64'),disk=file=>fs.readFileSync(file);
  const absent=(directory,name)=>{for(const item of [name+'.command.json',name+'.exit.json','home'])assert.equal(fs.existsSync(path.join(directory,item)),false,name+' left '+item);};
  test('editor import appends the single-thread setting the child reads',scratch(async dir=>{
    for(const [scenario,args]of [['absolute',project=>importArgs(project)],['relative',()=>importArgs('project')]]){
      const {run,project,file}=setup(dir,scenario,BASE),record=await start(run,'import-'+scenario,args(project));
      assert.deepEqual(seen(record),Buffer.from(BASE+BLOCK),scenario);assert.deepEqual(disk(file),seen(record),scenario);}}));
  test('editor import keeps a final single-thread block and appends after other false spellings',scratch(async dir=>{
    for(const [scenario,text,expected]of [['editor',FALSE_EDITOR,FALSE_EDITOR],['editor-crlf',FALSE_EDITOR.replaceAll('\n','\r\n'),FALSE_EDITOR.replaceAll('\n','\r\n')],
      ['root',FALSE_ROOT,FALSE_ROOT+BLOCK],['section',FALSE_SECTION,FALSE_SECTION+BLOCK],['comment',FALSE_COMMENT,FALSE_COMMENT+BLOCK],['override',FALSE_OVERRIDE,FALSE_OVERRIDE+BLOCK]]){
      const {run,project,file}=setup(dir,scenario,text),record=await start(run,'import-'+scenario,importArgs(project));
      assert.deepEqual(seen(record),Buffer.from(expected),scenario);assert.deepEqual(disk(file),Buffer.from(expected),scenario);}}));
  test('editor import is idempotent on a second import',scratch(async dir=>{
    const {run,project,file}=setup(dir,'twice',BASE),first=await start(run,'import-1',importArgs(project)),second=await start(run,'import-2',importArgs(project));
    assert.deepEqual(seen(first),Buffer.from(BASE+BLOCK));assert.deepEqual(seen(second),seen(first));assert.deepEqual(disk(file),seen(first));}));
  test('non-import commands leave project.godot untouched',scratch(async dir=>{
    for(const [scenario,text,args]of [['script',BASE,p=>['--headless','--path',p,'--script','res://probe.gd']],['export',BASE,p=>['--headless','--path',p,'--export-debug','Windows Desktop','out.exe']],
      ['script-true',TRUE_EDITOR,p=>['--headless','--path',p,'--script','res://probe.gd']],['version',BASE,()=>['--version']]]){
      const {run,project,file}=setup(dir,scenario,text);await start(run,'command-'+scenario,args(project));assert.deepEqual(disk(file),Buffer.from(text),scenario);}}));
  test('editor import rejects a threaded value before spawn',scratch(async dir=>{
    for(const [scenario,text]of [['editor-true',TRUE_EDITOR],['root-true',TRUE_ROOT],['section-true',FALSE_SECTION.replace('=false','=true')],['mixed',MIXED],
      ['quoted-key',FALSE_EDITOR+'"import/use_multiple_threads"=true\n'],['spaced-key',FALSE_EDITOR+'import/ use_multiple_threads=true\n'],
      ['header-comment',FALSE_ROOT+'\n[editor] ; note\n\nimport/use_multiple_threads=true\n'],['override-windows',BASE+'\n[editor]\n\nimport/use_multiple_threads.windows=true\n'],
      ['override-editor',FALSE_EDITOR+'import/use_multiple_threads.editor=true\n']]){
      const {run,project,file}=setup(dir,scenario,text);
      await assert.rejects(start(run,'import-'+scenario,importArgs(project)),{message:/^EDITOR_IMPORT_THREADED /},scenario);
      assert.deepEqual(disk(file),Buffer.from(text),scenario);absent(run,'import-'+scenario);}}));
  test('editor import rejects an unowned or missing project before spawn',scratch(async dir=>{
    const cases=[['no-path',s=>['--headless','--editor','--import','--quit']],['path-last',s=>['--headless','--editor','--import','--quit','--path']],
      ['outside',s=>{const other=path.join(s.scenario,'outside');fs.mkdirSync(other);fs.writeFileSync(path.join(other,'project.godot'),BASE);return importArgs(other);}],
      ['prefix',s=>{const other=s.run+'-x';fs.mkdirSync(other);fs.writeFileSync(path.join(other,'project.godot'),BASE);return importArgs(other);}],
      ['no-project',s=>importArgs(s.project)],['binary',s=>{fs.writeFileSync(path.join(s.project,'project.binary'),'binary');return importArgs(s.project);}],
      ['hard-link',s=>{fs.linkSync(s.file,path.join(s.scenario,'linked.godot'));return importArgs(s.project);}],
      ['main-pack',s=>[...importArgs(s.project),'--main-pack','game.pck']],['positional',s=>[...importArgs(s.project),path.join(s.project,'project.godot')]]];
    for(const [scenario,args]of cases){const s=setup(dir,scenario,scenario==='no-project'?undefined:BASE);
      await assert.rejects(start(s.run,'import-'+scenario,args(s)),{message:/^EDITOR_IMPORT_PROJECT /},scenario);absent(s.run,'import-'+scenario);
      if(fs.existsSync(s.file))assert.deepEqual(disk(s.file),Buffer.from(BASE),scenario);}}));
  test('editor import keeps CRLF and adds a missing final newline',scratch(async dir=>{
    for(const [scenario,text,expected]of [['crlf',CRLF,CRLF+BLOCK.replaceAll('\n','\r\n')],['no-final-newline',NO_FINAL,NO_FINAL+'\n'+BLOCK]]){
      const {run,project,file}=setup(dir,scenario,text),record=await start(run,'import-'+scenario,importArgs(project));
      assert.deepEqual(seen(record),Buffer.from(expected),scenario);assert.deepEqual(disk(file),Buffer.from(expected),scenario);}}));
}
module.exports={main,registerCase,cases,runTracer,validate,hash,inside,document,command,environment};
if(require.main===module){const argv=process.argv.slice(2);if(argv.length===1&&argv[0]==='--self-test-editor-import')editorImportSelfTest();
  else main(argv).catch(error=>{console.error(error.stack);process.exitCode=1;});}
