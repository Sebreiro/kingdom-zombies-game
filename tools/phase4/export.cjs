'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const runner={command:(...args)=>require('./check.cjs').command(...args),
  environment:(...args)=>require('./check.cjs').environment(...args),validate:(...args)=>require('./check.cjs').validate(...args)};
const {ROOT,need,inside,bytes,parse,document,write,hash}=require('./content.cjs');
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
const PIN='production/phase4/export-toolchain.json';
async function digest(file,algorithm='sha256'){
  const value=crypto.createHash(algorithm);for await(const part of fs.createReadStream(file))value.update(part);return value.digest('hex');
}
function snapshot(){
  return Object.fromEntries([PIN,'tools/phase4/export.cjs','tools/phase4/check.cjs','tools/phase4/cases/export-feasibility.cjs',
    'src/godot-game/export_presets.cfg','src/godot-game/project.godot','src/godot-game/KingdomZombies.csproj',
    'src/godot-game/content/pack-index.json','src/godot-game/content/packs/tracer.json',
    'src/godot-game/inspection/Inspection.tscn','src/godot-game/assets/common/bow.png','tools/phase1/projects.json',
    'tools/phase4/cases/export-layout.cjs','tools/code-rules/GeneratedLayout.cs','docs/code/generated-layout-adaptations.json',
    'production/phase4/export-layout-origin.json','tools/phase1/toolchain.json','tools/phase4/native-observer.cjs',
    'production/phase4/export-profile.json','tools/phase4/cases/export-profile.cjs']
    .map(file=>[file,hash(bytes(ROOT,file))]));
}
async function templates(context,pin){
  need(pin.engine==='4.7.2.stable.mono'&&Array.isArray(pin.templates)&&pin.templates.join(',')===
    'windows_debug_x86_64.exe,windows_debug_x86_64_console.exe,windows_release_x86_64.exe,windows_release_x86_64_console.exe','TEMPLATE_SELECTION');
  const archive=inside(ROOT,pin.archive.path),stat=fs.statSync(archive);
  need(stat.isFile()&&stat.size===pin.archive.bytes,'TEMPLATE_SIZE');
  need(await digest(archive,'sha512')===pin.archive.sha512,'TEMPLATE_SHA512');
  const sums=bytes(ROOT,pin.archive.checksums).toString('utf8');
  need(sums.split(/\r?\n/).some(line=>line.trim()===pin.archive.sha512+'  '+path.basename(archive)),'OFFICIAL_CHECKSUM');
  const directory=inside(context.directory,'templates');fs.mkdirSync(directory);
  const code=`import hashlib,json,pathlib,sys,zipfile\na=pathlib.Path(sys.argv[1]); out=pathlib.Path(sys.argv[2]); names=json.loads(sys.argv[3]); rows=[]\nwith zipfile.ZipFile(a) as z:\n assert z.read('templates/version.txt').decode().strip()=='4.7.2.stable.mono', 'TEMPLATE_VERSION'\n for name in names:\n  i=z.getinfo('templates/'+name)\n  assert 0<i.file_size<200000000 and not i.is_dir(), 'TEMPLATE_ENTRY'\n  b=z.read(i); (out/name).write_bytes(b); rows.append({'name':name,'bytes':len(b),'sha256':hashlib.sha256(b).hexdigest()})\nprint(json.dumps(rows))`;
  const result=await runner.command(context,'authenticate-extract',PYTHON,['-B','-c',code,archive,directory,JSON.stringify(pin.templates)],{timeout:60000});
  const rows=JSON.parse(result.stdout);need(rows.length===4,'TEMPLATE_COUNT');
  for(const row of rows)need(pin.templateMembers[row.name]?.bytes===row.bytes&&pin.templateMembers[row.name]?.sha256===row.sha256,'TEMPLATE_MEMBER_IDENTITY',row.name);
  write(context.directory,'templates.json',{archive:pin.archive,version:pin.engine,files:rows});
  return {directory,rows};
}
function cleanEnvironment(directory){
  const isolated=runner.environment(directory),env={};
  for(const key of ['SystemRoot','WINDIR','COMSPEC','PATHEXT','TEMP','TMP','HOME','USERPROFILE','APPDATA','LOCALAPPDATA']){
    const actual=Object.keys(isolated).find(x=>x.toUpperCase()===key.toUpperCase());if(actual)env[key]=isolated[actual];
  }
  env.PATH=path.join(process.env.SystemRoot,'System32');
  return env;
}
function runtimeEnvironment(env){
  need(!Object.keys(env).some(key=>/^(DOTNET|GODOT|NUGET|MSBUILD|COMPlus|CORECLR|MONO)_/i.test(key))&&
    env.PATH===path.join(process.env.SystemRoot,'System32'),'RUNTIME_SEARCH_OVERRIDE');
  return env;
}
async function cleanCommand(context,name,executable,args,extra={}){
  const env=runtimeEnvironment({...cleanEnvironment(context.directory),...extra});
  write(context.directory,name+'-environment.json',env);
  return runner.command(context,name,executable,args,{timeout:20000,env,cwd:path.dirname(executable)});
}
function inventory(directory,prefix='',state={files:0,bytes:0},depth=0){
  need(depth<=12,'EXPORT_DEPTH');
  const rows=[];
  for(const entry of fs.readdirSync(directory,{withFileTypes:true})){
    const name=prefix+entry.name,file=inside(directory,entry.name);
    need(!entry.isSymbolicLink(),'EXPORT_REPARSE',name);
    if(entry.isDirectory())rows.push(...inventory(file,name+'/',state,depth+1));
    else{const stat=fs.statSync(file);need(stat.isFile()&&stat.size>0&&stat.size<=268435456,'EXPORT_FILE_SIZE',name);
      need(++state.files<=2048&&(state.bytes+=stat.size)<=536870912,'EXPORT_TOTAL_SIZE');
      const data=fs.readFileSync(file);need(data.length===stat.size,'EXPORT_FILE_CHANGED',name);rows.push({path:name,bytes:data.length,sha256:hash(data)});}
  }
  need(rows.length<=2048,'EXPORT_COUNT');return rows;
}
function dependencies(directory,pin){
  const rows=inventory(directory),find=name=>rows.find(row=>row.path.endsWith('/'+name)||row.path===name);
  for(const name of pin.requiredDependencies)need(find(name),'DEPENDENCY_MISSING',name);
  const configRow=find('KingdomZombies.runtimeconfig.json'),depsRow=find('KingdomZombies.deps.json');
  const config=parse(fs.readFileSync(inside(directory,configRow.path)),'runtimeconfig'),deps=parse(fs.readFileSync(inside(directory,depsRow.path)),'deps');
  need(config.runtimeOptions.tfm===pin.framework&&!config.runtimeOptions.framework&&!config.runtimeOptions.frameworks&&
    config.runtimeOptions.includedFrameworks?.some(x=>x.name==='Microsoft.NETCore.App'&&x.version===pin.runtime),'SELF_CONTAINED_RUNTIME');
  need(deps.runtimeTarget.name.endsWith('/win-x64'),'RUNTIME_IDENTIFIER');
  for(const target of Object.values(deps.targets))for(const library of Object.values(target)){
    for(const category of ['runtime','native'])for(const name of Object.keys(library[category]??{}))
      need(find(path.posix.basename(name)),'DEPENDENCY_MISSING',name);
    for(const [name,metadata]of Object.entries(library.runtimeTargets??{}))if(metadata.rid==='win-x64')
      need(find(path.posix.basename(name)),'DEPENDENCY_MISSING',name);
  }
  return {rows,config,deps};
}
function pack(value){
  need(value.length>=108&&value.length<=67108864&&value.toString('ascii',0,4)==='GDPC'&&value.readUInt32LE(4)===4&&
    value.readUInt32LE(8)===4&&value.readUInt32LE(12)===7&&value.readUInt32LE(16)===2&&value.readUInt32LE(20)===2,'PCK_HEADER');
  const base=Number(value.readBigUInt64LE(24)),directory=Number(value.readBigUInt64LE(32));
  need(Number.isSafeInteger(base)&&Number.isSafeInteger(directory)&&base>=104&&directory>base&&directory+4<=value.length,'PCK_BOUNDS');
  let cursor=directory;const count=value.readUInt32LE(cursor),files=new Map();cursor+=4;
  need(count>0&&count<=4096,'PCK_COUNT');
  for(let index=0;index<count;index++){
    need(cursor+4<=value.length,'PCK_TRUNCATED');const length=value.readUInt32LE(cursor);cursor+=4;
    need(length>0&&length<=512&&cursor+length+36<=value.length,'PCK_PATH_SIZE');
    const name=new TextDecoder('utf-8',{fatal:true}).decode(value.subarray(cursor,cursor+length)).replace(/\0+$/,'');cursor+=length;
    need(name.length>0&&!name.includes('\0')&&!files.has(name)&&!name.startsWith('/')&&!name.includes('\\')&&
      !name.includes(':')&&name.split('/').every(x=>x&&x!=='.'&&x!=='..'),'PCK_PATH',name);
    const offset=base+Number(value.readBigUInt64LE(cursor)),size=Number(value.readBigUInt64LE(cursor+8));cursor+=16;
    const md5=value.subarray(cursor,cursor+16).toString('hex');cursor+=16;const flags=value.readUInt32LE(cursor);cursor+=4;
    need(flags===0&&Number.isSafeInteger(offset)&&Number.isSafeInteger(size)&&size>0&&offset>=base&&offset+size<=directory,'PCK_MEMBER_BOUNDS',name);
    const data=value.subarray(offset,offset+size);
    need(crypto.createHash('md5').update(data).digest('hex')===md5,'PCK_MEMBER_HASH',name);
    files.set(name,{path:name,offset,bytes:size,sha256:hash(data),data});
  }
  need(cursor===value.length,'PCK_TRAILING_DATA');return files;
}
function packaged(directory){
  const file=inside(directory,'KingdomZombies.pck');need(fs.existsSync(file),'EXPORT_FILES_MISSING','KingdomZombies.pck');
  const files=pack(fs.readFileSync(file)),required=['content/pack-index.json','content/packs/tracer.json','docs/README.md',
    'inspection/Inspection.tscn.remap','assets/common/bow.png.import','foundation/Foundation.tscn.remap','project.binary'];
  for(const name of required)need(files.has(name),'PACKAGED_RESOURCE_MISSING',name);
  for(const name of ['content/pack-index.json','content/packs/tracer.json','docs/README.md'])
    need(files.get(name).sha256===hash(bytes(ROOT,'src/godot-game/'+name)),'PACKAGED_SOURCE_MISMATCH',name);
  const remap=files.get('inspection/Inspection.tscn.remap').data.toString('utf8').match(/path="res:\/\/([^"\r\n]+)"/);
  const texture=files.get('assets/common/bow.png.import').data.toString('utf8').match(/path="res:\/\/([^"\r\n]+)"/);
  need(remap&&files.has(remap[1])&&texture&&files.has(texture[1]),'PACKAGED_IMPORT_CLOSURE');
  return {files,inspection:remap[1],texture:texture[1]};
}
function probe(nonce,pck){return `extends SceneTree
func _initialize():
    call_deferred("inspect")
func inspect():
    if not ProjectSettings.load_resource_pack("${pck.replaceAll('\\','/')}"):
        quit(11)
        return
    var packed = load("res://inspection/Inspection.tscn")
    if not packed is PackedScene:
        quit(12)
        return
    var scene = packed.instantiate()
    root.add_child(scene)
    var sprite = scene.get_node_or_null("Bow")
    if not sprite is Sprite2D or sprite.texture == null or sprite.texture.get_width() != 16 or sprite.texture.get_height() != 18:
        quit(12)
        return
    var data = JSON.parse_string(FileAccess.get_file_as_string("res://content/pack-index.json"))
    if not data is Dictionary or data.get("id") != "kingdom-zombies.v0.1" or data.get("status") != "incomplete":
        quit(13)
        return
    if not FileAccess.file_exists("res://docs/README.md") or not ResourceLoader.exists("res://foundation/Foundation.tscn"):
        quit(14)
        return
    if sprite.centered or sprite.offset != Vector2(-6, -9):
        quit(15)
        return
    var image = sprite.texture.get_image()
    image.convert(Image.FORMAT_RGBA8)
    var pixels = image.get_data()
    var visible = 0
    for index in range(3, pixels.size(), 4):
        if pixels[index] > 0:
            visible += 1
    if visible != 42 or scene.get_node("Summary/ResourceId").text != "common.bow":
        quit(16)
        return
    scene.free()
    print("PACKAGED_RESOURCE_COMPLETE ${nonce} common.bow visible=42")
    quit(0)
`;}
async function packagedProbe(context,name,engine,pck){
  const project=inside(context.directory,name+'-probe');fs.mkdirSync(project);
  write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="Pack resource headless probe"\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
  write(project,'probe.gd',Buffer.from(probe(context.nonce,pck)));
  const result=await runner.command(context,name,engine,['--headless','--path',project,'--script','res://probe.gd'],{timeout:15000});
  need(result.stdout.includes('PACKAGED_RESOURCE_COMPLETE '+context.nonce+' common.bow visible=42')&&
    !/ERROR:|SCRIPT ERROR:|Failed to load/i.test(result.stdout+result.stderr),'PACKAGED_NATIVE_RESOURCE');
  return {scope:'pinned editor resource loader of exact exported PCK; separate from executable startup',pckSha256:await digest(pck)};
}
async function portable(context,name,directory){
  const env=runtimeEnvironment({...cleanEnvironment(context.directory),COREHOST_TRACE:'1',COREHOST_TRACEFILE:inside(context.directory,name+'-host.log')});
  write(context.directory,name+'-environment.json',env);
  return require('./native-observer.cjs').observe(context,name,inside(directory,'KingdomZombies.exe'),env);
}
function hostTrace(context,name,directory){
  const trace=fs.readFileSync(inside(context.directory,name+'-host.log'),'utf8');need(trace.length<16777216,'HOST_TRACE_SIZE');
  need(trace.includes('Executing as a self-contained app as per config file'),'HOST_SELF_CONTAINED');
  const assembly=[...trace.matchAll(/assembly_paths: '([^']*)'/g)].flatMap(x=>x[1].split(';').filter(Boolean));
  const native=[...trace.matchAll(/native_search_paths: '([^']*)'/g)].flatMap(x=>x[1].split(';').filter(Boolean));
  const base=path.resolve(directory).toLowerCase()+path.sep;
  need(assembly.length>0&&native.length>0&&[...assembly,...native].every(x=>path.resolve(x).toLowerCase().startsWith(base)),'HOST_EXTERNAL_RESOLUTION');
  need(!/KindomCrown|[\\/]sdk[\\/]|[\\/]\.phase1[\\/]/i.test(trace),'HOST_AUTHORING_RESOLUTION');
  return {assemblyPaths:[...new Set(assembly)],nativeSearchPaths:[...new Set(native)],
    registeredInstallationProbe:trace.includes('Found registered install location'),sharedStoreDiscovery:trace.includes('Shared store (global)'),
    limit:'Installed SDK/registry exists; resolved local dependencies do not claim a machine without those installations'};
}
function cloneOutput(context,name,directory,rows){
  const target=inside(context.directory,name);fs.mkdirSync(target);
  for(const row of rows)write(target,row.path,fs.readFileSync(inside(directory,row.path)));
  return target;
}
async function negatives(context,output,closure,pin,engine,selected){
  const outcomes=[];
  async function rejected(name,code,action){
    let diagnostic;try{await action();}catch(error){diagnostic=error.message;}
    need(diagnostic?.startsWith(code),'EXPORT_NEGATIVE_CONTROL',name+' '+diagnostic);
    const row={name,status:'fail',expectedRejectionObserved:true,diagnostic};write(context.directory,name+'.json',row);outcomes.push(row);
  }
  await rejected('wrong-template-sha512','TEMPLATE_SHA512',()=>templates(context,{...pin,archive:{...pin.archive,sha512:'0'.repeat(128)}}));
  await rejected('wrong-template-member','TEMPLATE_MEMBER_IDENTITY',async()=>{
    const subject=structuredClone(pin);subject.templateMembers['windows_debug_x86_64.exe'].sha256='0'.repeat(64);
    const child={...context,directory:inside(context.directory,'bad-member')};fs.mkdirSync(child.directory);await templates(child,subject);
  });
  const original=fs.readFileSync(inside(output,'KingdomZombies.pck'));
  await rejected('truncated-pck','PCK_HEADER',()=>pack(original.subarray(0,60)));
  const badHash=Buffer.from(original);badHash[112]^=1;
  write(context.directory,'bad-member-hash.pck',badHash);
  await rejected('pck-member-hash','PCK_MEMBER_HASH',()=>pack(badHash));
  const noPack=inside(context.directory,'no-pck');fs.mkdirSync(noPack);
  await rejected('missing-pck','EXPORT_FILES_MISSING',()=>packaged(noPack));
  const badImports=cloneOutput(context,'bad-imports',output,closure.rows),importBytes=Buffer.from(original),texture=packaged(output).texture;
  const nameOffset=importBytes.lastIndexOf(Buffer.from(texture));need(nameOffset>0,'NEGATIVE_PCK_DIRECTORY');
  Buffer.from(texture.replace(/\.ctex$/,'.xtex')).copy(importBytes,nameOffset);fs.writeFileSync(inside(badImports,'KingdomZombies.pck'),importBytes);
  await rejected('missing-imported-texture','PACKAGED_IMPORT_CLOSURE',()=>packaged(badImports));
  await rejected('native-missing-imported-texture','CHILD_FAILED',()=>packagedProbe(context,'bad-resource-native',engine,inside(badImports,'KingdomZombies.pck')));
  const missingRuntime=cloneOutput(context,'missing-runtime',output,closure.rows);
  const coreclr=closure.rows.find(row=>row.path.endsWith('/coreclr.dll'));need(coreclr,'NEGATIVE_RUNTIME_TARGET');
  fs.unlinkSync(inside(missingRuntime,coreclr.path));
  await rejected('missing-coreclr','DEPENDENCY_MISSING',()=>dependencies(missingRuntime,pin));
  await rejected('actual-missing-coreclr','DEBUG_ENGINE_ERROR',()=>portable(context,'bad-runtime-native',missingRuntime));
  const config=closure.rows.find(row=>row.path.endsWith('/KingdomZombies.runtimeconfig.json'));
  const dependent=structuredClone(closure.config);dependent.runtimeOptions.frameworks=dependent.runtimeOptions.includedFrameworks;delete dependent.runtimeOptions.includedFrameworks;
  fs.writeFileSync(inside(badImports,config.path),JSON.stringify(dependent));
  await rejected('framework-dependent','SELF_CONTAINED_RUNTIME',()=>dependencies(badImports,pin));
  const editorHash=await digest(engine),templateHash=selected.rows.find(x=>x.name==='windows_debug_x86_64.exe').sha256;
  await rejected('editor-as-template','TEMPLATE_BINARY_MISMATCH',()=>need(editorHash===templateHash,'TEMPLATE_BINARY_MISMATCH'));
  await rejected('runtime-search-override','RUNTIME_SEARCH_OVERRIDE',()=>runtimeEnvironment({...cleanEnvironment(context.directory),DOTNET_ROOT:'unavailable-runtime-root'}));
  write(context.directory,'negative-controls.json',outcomes);return outcomes;
}
async function exportTracer(context){
  const initial=snapshot();write(context.directory,'inputs.json',initial);
  runner.validate(ROOT);require('./cases/export-profile.cjs').validate(ROOT);
  const pin=document(ROOT,PIN),tools=document(ROOT,'tools/phase1/toolchain.json');
  need(process.platform==='win32'&&process.arch==='x64'&&process.version==='v'+tools.node,'PLATFORM_PIN');
  const engine=inside(ROOT,'.phase1/tools/'+tools.archives['win32-x64'].directory+'/'+tools.archives['win32-x64'].executable);
  need(await digest(engine)===tools.archives['win32-x64'].executableSha256,'ENGINE_HASH');
  const selected=await templates(context,pin);
  const version=await runner.command(context,'engine-version',engine,['--headless','--version']);need(version.stdout.trim()===tools.engineFullVersion,'ENGINE_VERSION');
  const sdk=await runner.command(context,'sdk-version','C:/Program Files/dotnet/dotnet.exe',['--version']);need(sdk.stdout.trim()===tools.sdk,'SDK_PIN');
  const inputs=await runner.command(context,'tracked-inputs','git',['-C',ROOT,'ls-files','--','src','tools/code-rules','Directory.*','BannedSymbols*','.editorconfig','global.json','NuGet.config','KingdomZombies.slnx']);
  const source=inside(context.directory,'source');fs.mkdirSync(source);
  const inputHashes={};
  for(const name of inputs.stdout.trim().split(/\r?\n/)){
    const data=bytes(ROOT,name,16777216);inputHashes[name]=hash(data);write(source,name,data);
  }
  // New uncommitted task artifacts are copied explicitly before the first task commit.
  if(!fs.existsSync(inside(source,'src/godot-game/export_presets.cfg')))write(source,'src/godot-game/export_presets.cfg',bytes(ROOT,'src/godot-game/export_presets.cfg'));
  write(context.directory,'project-inputs.json',inputHashes);
  const project=inside(source,'src/godot-game'),presetFile=inside(project,'export_presets.cfg');
  let preset=fs.readFileSync(presetFile,'utf8');
  for(const mode of ['debug','release'])preset=preset.replace('@WINDOWS_'+mode.toUpperCase()+'_TEMPLATE@',inside(selected.directory,'windows_'+mode+'_x86_64.exe').replaceAll('\\','/'));
  fs.writeFileSync(presetFile,preset);
  // Godot's C# exporter requires a solution beside the project. This generated authoring-only wrapper preserves project references.
  write(project,'KingdomZombies.sln',Buffer.from('Microsoft Visual Studio Solution File, Format Version 12.00\nProject("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "KingdomZombies", "KingdomZombies.csproj", "{78E97099-6384-493C-9F15-58D8332E0958}"\nEndProject\nGlobal\nEndGlobal\n'));
  // Source checkout copy uses the authenticated local feed and existing cache; no global installation/configuration.
  fs.writeFileSync(inside(source,'NuGet.config'),bytes(ROOT,'NuGet.config').toString('utf8').replace('value=".phase1/feed"','value="'+ROOT+'/.phase1/feed"'));
  const output=inside(context.directory,'export');fs.mkdirSync(output);
  // Publish explicitly to expose all self-contained/runtime failures before native export.
  const publish=inside(context.directory,'publish');
  await runner.command(context,'managed-publish','C:/Program Files/dotnet/dotnet.exe',[
    'publish',inside(project,'KingdomZombies.csproj'),'-c','ExportDebug','-r','win-x64','--self-contained','true','-o',publish,
    '--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false',
    '-p:RestoreLockedMode=false','-p:NuGetPackageRoot='+ROOT+'/.phase1/packages/',
    '-p:RestorePackagesPath='+ROOT+'/.phase1/packages','-v:minimal'],{timeout:60000});
  dependencies(publish,pin);
  const authoring={...runner.environment(context.directory),NUGET_PACKAGES:inside(ROOT,'.phase1/packages'),
    DOTNET_CLI_TELEMETRY_OPTOUT:'1',DOTNET_GENERATE_ASPNET_CERTIFICATE:'false',DOTNET_NOLOGO:'1',
    MSBUILDDISABLENODEREUSE:'1',DOTNET_CLI_USE_MSBUILD_SERVER:'0'};
  const imported=await runner.command(context,'export-import',engine,['--headless','--path',project,'--editor','--import'],{env:authoring});
  need(!/ERROR:|SCRIPT ERROR:|Failed to load/i.test(imported.stdout+imported.stderr),'EXPORT_IMPORT');
  const binary=inside(output,'KingdomZombies.exe');
  const exported=await runner.command(context,'native-export',engine,['--headless','--path',project,'--export-debug',pin.preset,binary,'--quit-after','2'],{timeout:60000,env:authoring});
  need(!/ERROR:|SCRIPT ERROR:|Failed to load|Export failed/i.test(exported.stdout+exported.stderr),'NATIVE_EXPORT');
  need(fs.existsSync(binary)&&fs.existsSync(inside(output,'KingdomZombies.pck')),'EXPORT_FILES_MISSING');
  const closure=dependencies(output,pin),packageData=packaged(output);
  need(await digest(binary)===pin.templateMembers['windows_debug_x86_64.exe'].sha256,'TEMPLATE_BINARY_MISMATCH');
  need(await digest(binary)!==await digest(engine),'EDITOR_AS_OUTPUT');
  const clean=inside(context.directory,'portable');fs.mkdirSync(clean);
  for(const row of closure.rows)write(clean,row.path,fs.readFileSync(inside(output,row.path)));
  const executed=await portable(context,'portable-headless',clean);
  const runtimeResolution=hostTrace(context,'portable-headless',clean);
  const debuggerRecord=document(context.directory,'portable-headless-debugger.json');
  const observerControls=require('./native-observer.cjs').diagnostics(debuggerRecord,executed.pid);
  write(context.directory,'observer-controls.json',observerControls);
  const resources=await packagedProbe(context,'packaged-resource',engine,inside(clean,'KingdomZombies.pck'));
  const rejected=await negatives(context,output,closure,pin,engine,selected);
  need(Object.entries(inputHashes).every(([file,digest])=>hash(bytes(ROOT,file))===digest),'BUILD_SOURCE_CHANGED');
  need(JSON.stringify(snapshot())===JSON.stringify(initial),'SOURCE_CHANGED');
  write(context.directory,'closure.json',{engineSha256:await digest(engine),pythonSha256:await digest(PYTHON),
    dotnetSha256:await digest('C:/Program Files/dotnet/dotnet.exe'),templates:selected.rows,files:closure.rows,
    runtimeconfig:closure.config,runtimeTarget:closure.deps.runtimeTarget,portable:inventory(clean),inputHashes,
    pck:[...packageData.files.values()].map(({data,...row})=>row),resources,executed,expectedRejected:rejected.length,
    inspectionEntryComplete:true,observerControls,runtimeResolution});
  return {scope:'source/headless Windows tracer only',runtime:pin.runtime,framework:pin.framework,files:closure.rows.length,
    genuineTemplate:true,nativeInspection:executed,packagedResources:resources,portableCompletion:true,acceptance:'pending-04-32/04-33'};
}
module.exports={digest,snapshot,templates,cleanEnvironment,runtimeEnvironment,cleanCommand,inventory,dependencies,pack,packaged,probe,packagedProbe,portable,cloneOutput,negatives,exportTracer};
