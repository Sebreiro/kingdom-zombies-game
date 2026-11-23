'use strict';
const fs=require('node:fs'),path=require('node:path');
const ROOT=path.resolve(__dirname,'../../../../..');
const runner=require('../check.cjs');
const PROFILE='production/phase4/export-profile.json',PROJECT='src/godot-game/project.godot',PRESET='src/godot-game/export_presets.cfg';
const FEATURE='asset_inspection',BASE='application/run/main_scene',QUALIFIED=BASE+'.'+FEATURE;
const FOUNDATION='res://foundation/Foundation.tscn',INSPECTION='res://inspection/Inspection.tscn';
const THEME='src/godot-game/assets/ui/InspectionTheme.tres';
const RESOURCE_LIMIT=1048576,THEME_LIMIT=2097152,CLOSURE_COUNT=128,CLOSURE_DEPTH=16,CLOSURE_BYTES=16777216;
const SOURCE_ONLY='source/headless configuration/resource proof only; actual packaged Inspection startup pending 04-02 Task 1';
const CONTRACT={ordinaryProjectSha256:'1f7d242e12521a4866512b6017f25f1868cd6160e3c9efe0074ad498008b2a9d',
  presetBeforeFeatureSha256:'7fe9294d4a160582af981a9ec00167f8bf8bb407b6e86ce8b4bbde226bed3b00',
  projectGraphSha256:'8e7748d80497d3f4b499158be8c34ca98ae6f883b0306de7fbddd4a678880d64',
  checkpoint:'.phase4/checkpoints/04-02-entry-93386745/manifest.json',
  fullRunnerBeforeProfileSha256:'2e022d88b462d361bc1807fc54ed031245e93f167edd6c5c7b07837a49d0a021'};
const RESOURCES=['foundation/Foundation.tscn','inspection/Inspection.tscn','assets/common/bow.png','content/pack-index.json',
  'content/packs/tracer.json','assets/ui/kingdom-zombies-icon.png','docs/README.md'].map(file=>'src/godot-game/'+file);
const PRESET_OPTIONS={platform:'Windows Desktop',export_filter:'all_resources',
  include_filter:'*.json,*.md,*.txt,*.csv,*.ttf,*.otf,*.wav,*.ogg,*.png',
  'custom_template/debug':'@WINDOWS_DEBUG_TEMPLATE@','custom_template/release':'@WINDOWS_RELEASE_TEMPLATE@',
  'binary_format/architecture':'x86_64','binary_format/embed_pck':false,'dotnet/include_scripts_content':false,
  'dotnet/include_debug_symbols':true,'dotnet/embed_build_outputs':false};
const PROJECT_FILES=['src/godot-game/core/KingdomZombies.Core.csproj','src/godot-game/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj',
  'src/godot-game/KingdomZombies.csproj','src/godot-game/tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj','src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj'];
function need(value,code,detail){if(!value)throw new Error(code+' '+detail);}
function read(root,file){const target=runner.inside(root,file);need(fs.existsSync(target),'PROFILE_RESOURCE_MISSING',file);
  const limit=file===THEME?THEME_LIMIT:RESOURCE_LIMIT,fd=fs.openSync(target,'r');
  try{const stat=fs.fstatSync(fd);need(stat.isFile()&&stat.size>0&&stat.size<=limit,'PROFILE_RESOURCE_SIZE',file);
    const data=Buffer.alloc(stat.size);let offset=0;
    while(offset<data.length){const count=fs.readSync(fd,data,offset,data.length-offset,null);
      need(count>0,'PROFILE_RESOURCE_CHANGED',file);offset+=count;}
    need(fs.readSync(fd,Buffer.alloc(1),0,1,null)===0&&fs.fstatSync(fd).size===data.length,'PROFILE_RESOURCE_CHANGED',file);
    return data;
  }finally{fs.closeSync(fd);}}
function sceneClosure(root,seeds=RESOURCES){
  const found=new Map();let total=0;
  function visit(file,depth){
    // Authenticate the spelling even when a dependency has already been visited.
    runner.inside(root,file);if(found.has(file))return;
    need(depth<=CLOSURE_DEPTH,'PROFILE_CLOSURE_DEPTH',file);
    need(found.size<CLOSURE_COUNT,'PROFILE_CLOSURE_COUNT',file);
    const data=read(root,file);total+=data.length;need(total<=CLOSURE_BYTES,'PROFILE_CLOSURE_BYTES',file);
    found.set(file,runner.hash(data));
    if(!/\.(tscn|tres)$/.test(file))return;
    const text=new TextDecoder('utf-8',{fatal:true}).decode(data);
    need(!text.startsWith('\ufeff'),'PROFILE_ENCODING',file);
    for(const match of text.matchAll(/(?:^|\s)(?:metadata\/source_)?path\s*=\s*"([^"]+)"/gm)){
      need(match[1].startsWith('res://'),'PROFILE_SCENE_PATH',file+' '+match[1]);
      try{visit('src/godot-game/'+match[1].slice(6),depth+1);}
      catch(error){throw new Error(error.message+' referenced by '+file);}
    }
  }
  for(const file of seeds)visit(file,0);return Object.fromEntries(found);
}
function write(root,file,value){const target=runner.inside(root,file);fs.mkdirSync(path.dirname(target),{recursive:true});
  fs.writeFileSync(target,typeof value==='string'||Buffer.isBuffer(value)?value:JSON.stringify(value,null,2)+'\n',{flag:'wx'});}
function ini(root,file){
  const text=new TextDecoder('utf-8',{fatal:true}).decode(read(root,file)),values=new Map(),sections=new Set(),folded=new Set();let section='';
  need(!text.startsWith('\ufeff'),'PROFILE_ENCODING',file);
  for(const [index,line]of text.split(/\r?\n/).entries()){
    if(!line.trim()||line.trimStart().startsWith(';'))continue;
    const heading=line.match(/^\[([^\]]+)\]$/);
    if(heading){section=heading[1];need(!sections.has(section.toLowerCase()),'PROFILE_SECTION_CONFLICT',file+':'+(index+1)+' '+section);
      sections.add(section.toLowerCase());continue;}
    const entry=line.match(/^([^=\s]+)=(.+)$/);need(entry,'PROFILE_CONFIG_SYNTAX',file+':'+(index+1));
    const key=(section?section+'/':'')+entry[1];need(!folded.has(key.toLowerCase()),'PROFILE_KEY_CONFLICT',file+':'+(index+1)+' '+key);
    folded.add(key.toLowerCase());values.set(key,entry[2]);
  }
  return {text,values};
}
function validate(root){
  read(root,PROFILE);const profile=runner.document(root,PROFILE),project=ini(root,PROJECT),preset=ini(root,PRESET);
  need(profile.schemaVersion===1&&profile.feature===FEATURE&&profile.baseKey===BASE&&profile.overrideKey===QUALIFIED&&
    profile.baseScene===FOUNDATION&&profile.inspectionScene===INSPECTION&&profile.preset==='Phase4 Windows inspection',
  'PROFILE_IDENTITY',PROFILE);
  need(profile.scope===SOURCE_ONLY&&profile.nativeEntryComplete===false,'PROFILE_SCOPE',PROFILE);
  need(profile.currentSourceIcon?.path==='src/godot-game/assets/ui/kingdom-zombies-icon.png'
    &&profile.currentSourceIcon.sha256==='198b2c20db8b42c23e7a863a1ae39aeef600266911e12f5411ace6cf17fcfc3c'
    &&profile.currentSourceIcon.appliedToCurrentExport===false,'PROFILE_ICON_SOURCE_SCOPE',PROFILE);
  need(JSON.stringify(profile.sourceContract)===JSON.stringify(CONTRACT),'PROFILE_SOURCE_CONTRACT',PROFILE+' sourceContract');
  need(JSON.stringify(profile.resources)===JSON.stringify(RESOURCES),'PROFILE_RESOURCE_CONTRACT',PROFILE+' resources '+JSON.stringify(profile.resources));
  need(JSON.stringify(profile.requiredPreset)===JSON.stringify(PRESET_OPTIONS),'PROFILE_OPTIONS_CONTRACT',PROFILE+' requiredPreset');
  need(project.values.get(BASE)===JSON.stringify(FOUNDATION),'PROFILE_BASE_MAIN',PROJECT+' '+BASE);
  need(project.values.get(QUALIFIED)===JSON.stringify(INSPECTION),'PROFILE_OVERRIDE',PROJECT+' '+QUALIFIED);
  need(preset.values.get('preset.0/name')===JSON.stringify(profile.preset),'PROFILE_PRESET',PRESET+' preset.0/name');
  need(preset.values.get('preset.0/custom_features')===JSON.stringify(FEATURE),'PROFILE_FEATURE',PRESET+' preset.0/custom_features');
  need(project.values.get('application/config/icon')==='"res://assets/ui/kingdom-zombies-icon.png"','PROFILE_CURRENT_ICON',PROJECT);
  need(runner.hash(read(root,'src/godot-game/assets/ui/kingdom-zombies-icon.png'))==='198b2c20db8b42c23e7a863a1ae39aeef600266911e12f5411ace6cf17fcfc3c','PROFILE_CURRENT_ICON_BYTES',PROJECT);
  const original=project.text.replace('run/main_scene.asset_inspection="res://inspection/Inspection.tscn"\n','')
    .replace('config/icon="res://assets/ui/kingdom-zombies-icon.png"','config/icon="res://icon.svg"');
  need(runner.hash(original)===profile.sourceContract.ordinaryProjectSha256,'PROFILE_ORDINARY_CONFIG',PROJECT);
  const originalPreset=preset.text.replace('custom_features="asset_inspection"','custom_features=""');
  need(runner.hash(originalPreset)===profile.sourceContract.presetBeforeFeatureSha256,'PROFILE_PRESET_OPTIONS',PRESET);
  need(!fs.existsSync(runner.inside(root,'src/godot-game/override.cfg')),'PROFILE_RUNTIME_OVERRIDE','src/godot-game/override.cfg');
  for(const relative of profile.resources){need(typeof relative==='string'&&relative.startsWith('src/godot-game/'),
    'PROFILE_RESOURCE_PATH',PROFILE+' '+relative);read(root,relative);}
  sceneClosure(root,profile.resources);
  const graph=read(root,'src/godot-game/tools/verification/projects.json');need(runner.hash(graph)===profile.sourceContract.projectGraphSha256,'PROFILE_GRAPH','src/godot-game/tools/verification/projects.json');
  return profile;
}
function snapshot(root){const profile=validate(root);return Object.fromEntries([PROFILE,PROJECT,PRESET,'src/godot-game/tools/verification/projects.json',
  'src/godot-game/tools/verification/toolchain.json','tools/phase4/check.cjs','tools/phase4/cases/export-profile.cjs',...PROJECT_FILES,
  'src/godot-game/content/expected-obligations.json',
  ...Object.keys(sceneClosure(root,profile.resources))].map(file=>[file,runner.hash(read(root,file))]));}
function productionSources(){
  const files=['src/godot-game/Directory.Build.props','src/godot-game/Directory.Build.targets',
    'src/godot-game/Directory.Packages.props','src/godot-game/global.json','src/godot-game/build/BannedSymbols.txt','.editorconfig',
    ...PROJECT_FILES];
  function walk(directory){for(const entry of fs.readdirSync(runner.inside(ROOT,directory),{withFileTypes:true})){
    const file=directory+'/'+entry.name;
    if(entry.isDirectory()&&!['bin','obj','.godot'].includes(entry.name))walk(file);
    else if(entry.isFile()&&/\.cs$/.test(entry.name))files.push(file);
  }}
  for(const directory of ['src/godot-game/core','src/godot-game/multiplayer-abstractions','src/godot-game/game','src/godot-game/tools/code-rules'])walk(directory);
  return Object.fromEntries([...new Set(files)].sort().map(file=>[file,runner.hash(read(ROOT,file))]));
}
function buildEnvironment(context){const base=path.join(ROOT,'.phase1');return {...runner.environment(context.directory),
  DOTNET_CLI_HOME:path.join(base,'cli-home'),NUGET_PACKAGES:path.join(base,'packages'),
  NUGET_HTTP_CACHE_PATH:path.join(base,'http-cache'),TEMP:path.join(base,'temp'),TMP:path.join(base,'temp'),PHASE1_ROOT:ROOT};}
function copyRuntime(project){
  const relative='src/godot-game/game/.godot/mono/temp/bin/Debug',source=runner.inside(ROOT,relative),files=fs.readdirSync(source).sort();
  need(files.length>0&&files.length<=32,'PROFILE_ASSEMBLY_COUNT',relative);const closure=[];let total=0;
  for(const file of files){const from=runner.inside(source,file),stat=fs.statSync(from);
    need(stat.isFile()&&stat.size>0&&stat.size<=16777216,'PROFILE_ASSEMBLY_SIZE',relative+'/'+file);
    total+=stat.size;need(total<=67108864,'PROFILE_ASSEMBLY_BYTES',relative);
    const data=fs.readFileSync(from),sha256=runner.hash(data);need(data.length===stat.size,'PROFILE_ASSEMBLY_CHANGED',file);
    const target='.godot/mono/temp/bin/Debug/'+file;write(project,target,data);
    need(runner.hash(fs.readFileSync(runner.inside(project,target)))===sha256,'PROFILE_ASSEMBLY_COPY',file);
    closure.push({file:relative+'/'+file,target,bytes:data.length,sha256});
  }
  for(const file of ['KingdomZombies.dll','KingdomZombies.Core.dll','KingdomZombies.Multiplayer.Abstractions.dll',
    'KingdomZombies.deps.json','KingdomZombies.runtimeconfig.json'])need(files.includes(file),'PROFILE_ASSEMBLY_MISSING',file);
  return closure;
}
function probeScript(nonce){return `extends SceneTree
func reject(reason):
    push_error("PROFILE_NATIVE_FAILURE ${nonce} " + reason)
    quit(1)
func _initialize():
    call_deferred("inspect")
func inspect():
    var key = "${BASE}"
    var observation = {"nonce":"${nonce}", "scope":"source/headless only", "base":ProjectSettings.get_setting(key),
        "qualified":ProjectSettings.get_setting("${QUALIFIED}", "missing"),
        "actual":ProjectSettings.get_setting_with_override(key),
        "empty":ProjectSettings.get_setting_with_override_and_custom_features(key, PackedStringArray()),
        "ordinary":ProjectSettings.get_setting_with_override_and_custom_features(key, PackedStringArray(["windows", "editor"])),
        "inspection":ProjectSettings.get_setting_with_override_and_custom_features(key, PackedStringArray(["${FEATURE}"])),
        "wrongCase":ProjectSettings.get_setting_with_override_and_custom_features(key, PackedStringArray(["ASSET_INSPECTION"])),
        "active":OS.has_feature("${FEATURE}"), "foundationExists":FileAccess.file_exists("${FOUNDATION}"),
        "pid":OS.get_process_id(), "executable":OS.get_executable_path()}
    print("PROFILE_SETTINGS " + JSON.stringify(observation))
    var packed = load("${INSPECTION}")
    if not packed is PackedScene:
        reject("inspection/Inspection.tscn SCENE_LOAD")
        return
    var scene = packed.instantiate()
    if scene.get_script() == null or scene.get_script().resource_path != "res://inspection/InspectionRoot.cs":
        scene.free()
        reject("inspection/InspectionRoot.cs SCRIPT_IDENTITY")
        return
    root.add_child(scene)
    var bow = scene.get_node_or_null("Bow")
    if not bow is Sprite2D or bow.texture == null:
        reject("assets/common/bow.png TEXTURE_LOAD")
        return
    var image = bow.texture.get_image()
    if image == null or image.get_width() != 16 or image.get_height() != 18:
        reject("assets/common/bow.png IMAGE_LOAD")
        return
    image.convert(Image.FORMAT_RGBA8)
    var visible = 0
    for i in range(3, image.get_data().size(), 4):
        if image.get_data()[i] > 0:
            visible += 1
    if visible != 42:
        reject("assets/common/bow.png IMAGE_EMPTY")
        return
    print("PROFILE_RESOURCE ${nonce} inspection/Inspection.tscn assets/common/bow.png pixels=" + str(visible))
    print("PROFILE_SCRIPT ${nonce} " + scene.get_script().resource_path)
    image = null
    scene.free()
    quit(0)
`;}
async function observe(root,context,name='profile',expected={qualified:INSPECTION,inspection:INSPECTION,active:false,actual:FOUNDATION}){
  const pins=runner.document(ROOT,'src/godot-game/tools/verification/toolchain.json'),pin=pins.archives['win32-x64'];
  need(process.platform==='win32'&&process.arch==='x64'&&process.version==='v'+pins.node,'PROFILE_PLATFORM','src/godot-game/tools/verification/toolchain.json');
  const relative='.phase1/tools/'+pin.directory+'/'+pin.executable,console=runner.inside(ROOT,relative);
  need(runner.hash(fs.readFileSync(console))===pin.executableSha256,'PROFILE_ENGINE_HASH',relative);
  const archive=runner.inside(ROOT,'.phase1/cache/'+pin.file),nativeName=pin.executable.replace('_console.exe','.exe');
  need(runner.hash(fs.readFileSync(archive))===pin.sha256,'PROFILE_ARCHIVE_HASH',pin.file);
  const authentication=await runner.command(context,name+'-tool-origin','powershell.exe',['-NoProfile','-NonInteractive','-Command',
    "Add-Type -AssemblyName System.IO.Compression.FileSystem; $zip=[IO.Compression.ZipFile]::OpenRead('"+archive.replaceAll("'","''")+"'); try { $entry=$zip.Entries | Where-Object { $_.Name -ceq '"+nativeName+"' }; if(@($entry).Count -ne 1){throw 'ARCHIVE_ENTRY'}; $stream=$entry.Open(); try { $sha=[Security.Cryptography.SHA256]::Create(); [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-','').ToLowerInvariant() } finally { $stream.Dispose() } } finally { $zip.Dispose() }"]);
  const executable=path.join(path.dirname(console),nativeName),engineSha256=runner.hash(fs.readFileSync(executable));
  need(/^[a-f0-9]{64}$/.test(authentication.stdout.trim())&&engineSha256===authentication.stdout.trim(),
    'PROFILE_NATIVE_ENGINE_HASH',nativeName);
  const version=await runner.command(context,name+'-version',executable,['--headless','--version']);
  need(version.stdout.trim()===pins.engineFullVersion,'PROFILE_ENGINE_VERSION',relative);
  const project=runner.inside(context.directory,name+'-project');fs.mkdirSync(project);
  const profile=runner.document(root,PROFILE),inputs={},sources=productionSources();
  const closure=sceneClosure(root,profile.resources),expectations='src/godot-game/content/expected-obligations.json';
  const resources=[...new Set([PROJECT,'src/godot-game/KingdomZombies.csproj',expectations,...Object.keys(closure)])];
  for(const file of resources){const data=read(root,file);inputs[file]=runner.hash(data);write(project,file.slice('src/godot-game/'.length),data);}
  const build=await runner.command(context,name+'-strict-build','dotnet',['build',runner.inside(ROOT,'src/godot-game/KingdomZombies.csproj'),
    '--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:buildEnvironment(context)});
  need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'PROFILE_CLEAN_BUILD',name);
  need(JSON.stringify(productionSources())===JSON.stringify(sources),'PROFILE_BUILD_SOURCE_CHANGED',name);
  const assemblies=copyRuntime(project);write(context.directory,name+'-assemblies.json',assemblies);
  const script=probeScript(context.nonce);write(project,'probe.gd',script);
  // Import media in a separate asset-only fixture. The settings subject keeps the exact production bytes.
  const importer=runner.inside(context.directory,name+'-importer');fs.mkdirSync(importer);
  write(importer,'project.godot','config_version=5\n[application]\nconfig/name="Profile media import fixture"\n');
  const media=resources.filter(file=>/\.(png|svg|ttf|otf)$/.test(file));
  for(const file of media)write(importer,file.slice('src/godot-game/'.length),read(root,file));
  write(context.directory,name+'-inputs.json',{inputs,sources,assemblies,probeSha256:runner.hash(script),engineSha256,
    consoleSha256:pin.executableSha256,archiveSha256:pin.sha256,executable,version:pins.engineFullVersion,scope:SOURCE_ONLY});
  const imported=await runner.command(context,name+'-import',executable,['--headless','--path',importer,'--editor','--import']);
  need(!/ERROR:|SCRIPT ERROR:|Failed to load|Error importing/i.test(imported.stdout+imported.stderr),'PROFILE_NATIVE_IMPORT',
    'src/godot-game/assets/common/bow.png '+name+'-import.command.json');
  fs.cpSync(path.join(importer,'.godot/imported'),path.join(project,'.godot/imported'),{recursive:true,errorOnExist:true,force:false});
  for(const file of media){const relative=file.slice('src/godot-game/'.length)+'.import';
    write(project,relative,read(importer,relative));}
  const importedFiles=[];
  for(const file of fs.readdirSync(path.join(project,'.godot/imported')).sort()){
    const target=path.join(project,'.godot/imported',file),stat=fs.statSync(target);
    need(stat.isFile()&&stat.size<=16777216,'PROFILE_IMPORT_SIZE',file);
    importedFiles.push({file:'.godot/imported/'+file,bytes:stat.size,sha256:runner.hash(fs.readFileSync(target))});
  }
  write(context.directory,name+'-imported.json',importedFiles);
  const result=await runner.command(context,name+'-settings',executable,['--headless','--path',project,'--script','res://probe.gd'],
    {timeout:15000,env:buildEnvironment(context)});
  need(!/ERROR:|SCRIPT ERROR:|Failed to load|leaked|ObjectDB instances/i.test(result.stdout+result.stderr)&&result.stderr==='',
    'PROFILE_NATIVE_RESOURCE','inspection/Inspection.tscn '+name+'-settings.command.json');
  const lines=result.stdout.split(/\r?\n/).filter(line=>line.startsWith('PROFILE_SETTINGS '));
  need(lines.length===1,'PROFILE_OBSERVATION_MISSING',name);const observation=JSON.parse(lines[0].slice(17));
  need(observation.pid===result.pid&&path.resolve(observation.executable)===path.resolve(executable),
    'PROFILE_PROCESS_IDENTITY',name+' '+JSON.stringify(observation));
  need(observation.nonce===context.nonce&&observation.base===FOUNDATION&&observation.qualified===expected.qualified&&
    observation.actual===expected.actual&&observation.empty===FOUNDATION&&observation.ordinary===FOUNDATION&&
    observation.inspection===expected.inspection&&observation.wrongCase===FOUNDATION&&observation.active===expected.active&&observation.foundationExists,
  'PROFILE_NATIVE_SETTINGS',JSON.stringify(observation));
  need(result.stdout.includes('PROFILE_RESOURCE '+context.nonce+' inspection/Inspection.tscn assets/common/bow.png pixels=42'),
    'PROFILE_RESOURCE_OBSERVATION_MISSING','assets/common/bow.png');
  need(result.stdout.includes('PROFILE_SCRIPT '+context.nonce+' res://inspection/InspectionRoot.cs'),
    'PROFILE_SCRIPT_OBSERVATION_MISSING','inspection/InspectionRoot.cs');
  for(const [file,sha256]of Object.entries(inputs)){
    const target=runner.inside(project,file.slice('src/godot-game/'.length)),stat=fs.statSync(target);
    need(stat.isFile()&&stat.size>0&&stat.size<=(file===THEME?THEME_LIMIT:RESOURCE_LIMIT), 'PROFILE_SUBJECT_SIZE',file);
    need(runner.hash(fs.readFileSync(target))===sha256,'PROFILE_SUBJECT_CHANGED',file);
  }
  need(JSON.stringify(productionSources())===JSON.stringify(sources),'PROFILE_OBSERVATION_SOURCE_CHANGED',name);
  write(context.directory,name+'-observations.json',observation);return observation;
}
async function profileCase(context){const before=snapshot(ROOT);write(context.directory,'inputs.json',before);
  const checkpoint=runner.document(ROOT,CONTRACT.checkpoint);for(const row of checkpoint)
    need(runner.hash(read(ROOT,CONTRACT.checkpoint.replace('manifest.json','')+row.path))===row.sha256,'PROFILE_CHECKPOINT',row.path);
  need(checkpoint.some(row=>row.path==='tools/phase4/check.cjs'&&row.sha256===CONTRACT.fullRunnerBeforeProfileSha256),
    'PROFILE_CHECKPOINT','tools/phase4/check.cjs');
  const observation=await observe(ROOT,context);need(JSON.stringify(snapshot(ROOT))===JSON.stringify(before),'PROFILE_SOURCE_CHANGED',PROJECT);
  return {observation,scope:SOURCE_ONLY,nativeEntryComplete:false};}
const definition={group:'export',title:'Permanent inspection feature selects real source settings and resources',importance:'Critical',
  rationale:'Wrong delivery entry or accidental ordinary activation blocks inspection delivery and normal startup',requirements:['ASSET-05'],acceptance:[],
  status:'required',tags:['source-only','export-profile','settings','resources'],needsGameWindow:false,runGroup:'G0-export-profile',order:350,
  setup:'Permanent production profile and single existing preset; authenticated pinned Windows engine; fresh owned source copy',
  actions:['Validate exact canonical keys, preset feature/options, portable resources and unchanged ordinary configuration',
    'Import owned copy with authenticated pinned engine','Read genuine base/actual/explicit-feature settings and load Inspection texture'],
  expected:'Ordinary and wrong-case query resolve Foundation; canonical explicit feature resolves Inspection and loads 42 visible bow pixels; no native export credit',
  reset:'Fresh owned disposable project; no production mutation or autoload; bounded hidden children with descendant cleanup; immutable evidence retained',
  evidence:['inputs.json','profile-inputs.json','profile-import.command.json','profile-settings.command.json','profile-observations.json','result.json']};
runner.registerCase('export.profile',definition,profileCase);
function trialContext(context,name){
  const directory=runner.inside(context.directory,'trials/'+name);fs.mkdirSync(directory,{recursive:true});
  const child={...context,directory,nonce:require('node:crypto').randomUUID(),caseId:context.caseId+'/'+name,startedAt:new Date().toISOString()};
  write(directory,'invocation.json',child);return child;
}
function copySubject(context){const root=runner.inside(context.directory,'subject');fs.mkdirSync(root);
  for(const file of Object.keys(snapshot(ROOT)))write(root,file,read(ROOT,file));return root;}
function mutate(root,file,edit){const target=runner.inside(root,file),original=read(root,file);fs.writeFileSync(target,edit(original.toString('utf8')));}
function registry(){
  const rows=[...runner.cases.values()].map(({run,...item})=>item),ids=new Set(),orders=new Set();
  for(const row of rows){need(!ids.has(row.id),'PROFILE_REGISTRY_ID',row.id);ids.add(row.id);
    const position=row.runGroup+'/'+row.order;need(!orders.has(position),'PROFILE_REGISTRY_ORDER',row.id+' '+position);orders.add(position);}
  for(const [id,group,order]of [['export.layout','G0',0],['source.tracer','G0',1],['source.tracer-failures','G0',2],
    ['source.provenance','G0',3],['source.publication','G0',4],['audio.feasibility','G0',6],
    ['export.profile','G0-export-profile',350],['export.profile-boundaries','G0-export-profile',351]]){
    const row=rows.find(row=>row.id===id);need(row&&row.runGroup===group&&row.order===order&&row.needsGameWindow===false,
      'PROFILE_REGISTRY_CHANGED',id);}
  return rows;
}
async function helperControls(context){
  const script='console.log(JSON.stringify({cwd:process.cwd(),home:process.env.HOME,temp:process.env.TEMP,packages:process.env.NUGET_PACKAGES,marker:process.env.PROFILE_OWNED_MARKER??null}));console.error("HELPER_STDERR");';
  const ordinary=await runner.command(context,'helper-default',process.execPath,['-e',script]),defaults=JSON.parse(ordinary.stdout.trim());
  need(path.resolve(defaults.cwd)===path.resolve(context.directory)&&defaults.home===path.join(context.directory,'home')&&
    defaults.temp===path.join(context.directory,'temp')&&defaults.packages===path.join(context.directory,'packages')&&
    ordinary.stderr.trim()==='HELPER_STDERR'&&ordinary.code===0&&ordinary.streamsClosed&&!ordinary.cleanupError,
  'PROFILE_HELPER_DEFAULT','helper-default.command.json');
  const cwd=runner.inside(context.directory,'explicit-cwd');fs.mkdirSync(cwd);
  const envRoot=runner.inside(context.directory,'explicit-env'),env={...runner.environment(envRoot),PROFILE_OWNED_MARKER:context.nonce};
  const explicit=await runner.command(context,'helper-explicit',process.execPath,['-e',script],{cwd,env}),owned=JSON.parse(explicit.stdout.trim());
  need(path.resolve(owned.cwd)===path.resolve(cwd)&&owned.home===path.join(envRoot,'home')&&owned.temp===path.join(envRoot,'temp')&&
    owned.packages===path.join(envRoot,'packages')&&owned.marker===context.nonce&&explicit.stderr.trim()==='HELPER_STDERR'&&
    explicit.streamsClosed&&explicit.code===0&&!explicit.cleanupError,'PROFILE_HELPER_EXPLICIT','helper-explicit.command.json');
  const outcomes=[];
  async function reject(name,code,action,inspect){let failure;try{await action();}catch(error){failure=error;}
    need(failure&&failure.message.startsWith(code+' '),'PROFILE_HELPER_REJECTION',name+' '+failure?.message);
    if(inspect){const file=runner.inside(context.directory,name+'.command.json');
      need(fs.statSync(file).size<=2097152,'PROFILE_HELPER_RECORD_SIZE',name);inspect(JSON.parse(fs.readFileSync(file,'utf8')));}
    outcomes.push({name,expected:code,observed:failure.message});}
  for(const timeout of [0,-1,1.5,60001])await reject('invalid-timeout-'+String(timeout).replace('.','-'),'COMMAND_TIMEOUT_RANGE',
    ()=>runner.command(context,'invalid-timeout-'+String(timeout).replace('.','-'),process.execPath,['-e','process.exit(0)'],{timeout}));
  await reject('outside-cwd','COMMAND_CWD_OWNERSHIP',()=>runner.command(context,'outside-cwd',process.execPath,['-e','process.exit(0)'],{cwd:ROOT}));
  await reject('invalid-env','COMMAND_ENV_INVALID',()=>runner.command(context,'invalid-env',process.execPath,['-e','process.exit(0)'],{env:[]}));
  await reject('helper-nonzero','CHILD_FAILED',()=>runner.command(context,'helper-nonzero',process.execPath,
    ['-e','console.log("EXPECTED_STDOUT");console.error("EXPECTED_NONZERO");process.exit(7)']),row=>{
      need(row.code===7&&row.stdout.trim()==='EXPECTED_STDOUT'&&row.stderr.trim()==='EXPECTED_NONZERO'&&row.streamsClosed&&!row.cleanupError,
        'PROFILE_HELPER_EXIT','helper-nonzero.command.json');});
  await reject('helper-missing','CHILD_FAILED',()=>runner.command(context,'helper-missing',path.join(context.directory,'absent-child.exe'),[]),row=>{
    need(row.spawnError?.includes('ENOENT')&&row.streamsClosed&&!row.cleanupError,'PROFILE_HELPER_SPAWN','helper-missing.command.json');});
  const tree='const p=require("node:child_process").spawn(process.execPath,["-e","setInterval(()=>{},1000)"],{stdio:"ignore",windowsHide:true});console.log("OWNED_DESCENDANT "+p.pid);console.error("TIMEOUT_STDERR");setInterval(()=>{},1000);';
  await reject('helper-timeout','CHILD_TIMEOUT',()=>runner.command(context,'helper-timeout',process.execPath,['-e',tree],{timeout:750}),row=>{
    const match=row.stdout.match(/OWNED_DESCENDANT (\d+)/);need(match&&row.stderr.includes('TIMEOUT_STDERR')&&row.streamsClosed&&!row.cleanupError,
      'PROFILE_HELPER_TIMEOUT','helper-timeout.command.json');
    let alive=true;try{process.kill(Number(match[1]),0);}catch(error){need(error.code==='ESRCH','PROFILE_HELPER_PID',error.code);alive=false;}
    need(!alive,'PROFILE_HELPER_DESCENDANT','helper-timeout.command.json');});
  await reject('helper-output-limit','OUTPUT_LIMIT',()=>runner.command(context,'helper-output-limit',process.execPath,
    ['-e','process.stdout.write("x".repeat(1048577));setInterval(()=>{},1000)']),row=>{
      need(row.stdout.length>1048576&&row.streamsClosed&&!row.cleanupError,'PROFILE_HELPER_OUTPUT','helper-output-limit.command.json');});
  write(context.directory,'helper-controls.json',{defaults,owned,outcomes});return {defaults,owned,outcomes};
}
function sourceControls(context,before=snapshot(ROOT)){
  const controls=[
    ['missing-profile',PROFILE,'PROFILE_RESOURCE_MISSING',null],
    ['wrong-profile-feature',PROFILE,'PROFILE_IDENTITY',text=>text.replace('"feature": "asset_inspection"','"feature": "other"')],
    ['missing-profile-feature',PROFILE,'PROFILE_IDENTITY',text=>text.replace('  "feature": "asset_inspection",\n','')],
    ['duplicate-profile-feature',PROFILE,'JSON_DUPLICATE_KEY',text=>text.replace('"feature": "asset_inspection"','"feature": "asset_inspection", "feature": "other"')],
    ['wrong-profile-scope',PROFILE,'PROFILE_SCOPE',text=>text.replace('"nativeEntryComplete": false','"nativeEntryComplete": true')],
    ['wrong-profile-input',PROFILE,'PROFILE_SOURCE_CONTRACT',text=>text.replace(CONTRACT.ordinaryProjectSha256,'0'.repeat(64))],
    ['missing-feature',PRESET,'PROFILE_FEATURE',text=>text.replace('custom_features="asset_inspection"','custom_features=""')],
    ['wrong-feature',PRESET,'PROFILE_FEATURE',text=>text.replace('custom_features="asset_inspection"','custom_features="other"')],
    ['feature-case',PRESET,'PROFILE_FEATURE',text=>text.replace('custom_features="asset_inspection"','custom_features="ASSET_INSPECTION"')],
    ['duplicate-feature',PRESET,'PROFILE_FEATURE',text=>text.replace('custom_features="asset_inspection"','custom_features="asset_inspection,asset_inspection"')],
    ['conflicting-feature',PRESET,'PROFILE_FEATURE',text=>text.replace('custom_features="asset_inspection"','custom_features="asset_inspection,ASSET_INSPECTION"')],
    ['wrong-preset-name',PRESET,'PROFILE_PRESET',text=>text.replace('Phase4 Windows inspection','Other inspection')],
    ['duplicate-preset-name',PRESET,'PROFILE_PRESET_OPTIONS',text=>text+'\n[preset.1]\nname="Phase4 Windows inspection"\n'],
    ['missing-preset',PRESET,'PROFILE_RESOURCE_MISSING',null],
    ['duplicate-preset-key',PRESET,'PROFILE_KEY_CONFLICT',text=>text+'custom_features="asset_inspection"\ncustom_features="other"\n'],
    ['preset-section-case',PRESET,'PROFILE_SECTION_CONFLICT',text=>text+'\n[PRESET.0]\nname="Other"\n'],
    ['missing-override',PROJECT,'PROFILE_OVERRIDE',text=>text.replace(/^run\/main_scene\.asset_inspection=.*\n/m,'')],
    ['setting-case',PROJECT,'PROFILE_OVERRIDE',text=>text.replace('run/main_scene.asset_inspection','run/main_scene.ASSET_INSPECTION')],
    ['setting-key-case',PROJECT,'PROFILE_OVERRIDE',text=>text.replace('run/main_scene.asset_inspection','run/Main_scene.asset_inspection')],
    ['duplicate-setting',PROJECT,'PROFILE_KEY_CONFLICT',text=>text.replace('config/features=','run/main_scene.asset_inspection="res://inspection/Inspection.tscn"\nconfig/features=')],
    ['conflicting-setting-case',PROJECT,'PROFILE_KEY_CONFLICT',text=>text.replace('config/features=', 'run/main_scene.ASSET_INSPECTION="res://foundation/Foundation.tscn"\nconfig/features=')],
    ['base-main',PROJECT,'PROFILE_BASE_MAIN',text=>text.replace('run/main_scene="res://foundation/Foundation.tscn"','run/main_scene="res://inspection/Inspection.tscn"')],
    ['missing-base',PROJECT,'PROFILE_BASE_MAIN',text=>text.replace('run/main_scene="res://foundation/Foundation.tscn"\n','')],
    ['inspection-path-case',PROJECT,'PROFILE_OVERRIDE',text=>text.replace('res://inspection/Inspection.tscn','res://Inspection/Inspection.tscn')],
    ['absolute-scene-path',PROJECT,'PROFILE_OVERRIDE',text=>text.replace('res://inspection/Inspection.tscn','D:/inspection/Inspection.tscn')],
    ['escaped-scene-path',PROJECT,'PROFILE_OVERRIDE',text=>text.replace('res://inspection/Inspection.tscn','res://inspection/../inspection/Inspection.tscn')],
    ['aliased-scene-path',PROJECT,'PROFILE_OVERRIDE',text=>text.replace('res://inspection/Inspection.tscn','uid://inspection')],
    ['global-feature',PROJECT,'PROFILE_ORDINARY_CONFIG',text=>'_custom_features="asset_inspection"\n'+text],
    ['ordinary-config-feature',PROJECT,'PROFILE_ORDINARY_CONFIG',text=>text.replace('"Forward Plus")','"Forward Plus", "asset_inspection")')],
    ['autoload-shortcut',PROJECT,'PROFILE_ORDINARY_CONFIG',text=>text+'\n[autoload]\nInspection="*res://inspection/Inspection.tscn"\n'],
    ['ordinary-feature-override',PROJECT,'PROFILE_ORDINARY_CONFIG',text=>text.replace('[display]','run/main_scene.windows="res://inspection/Inspection.tscn"\n\n[display]')],
    ['missing-inspection','src/godot-game/inspection/Inspection.tscn','PROFILE_RESOURCE_MISSING',null],
    ['missing-foundation','src/godot-game/foundation/Foundation.tscn','PROFILE_RESOURCE_MISSING',null],
    ['missing-bow','src/godot-game/assets/common/bow.png','PROFILE_RESOURCE_MISSING',null],
    ['missing-json','src/godot-game/content/pack-index.json','PROFILE_RESOURCE_MISSING',null],
    ['missing-doc','src/godot-game/docs/README.md','PROFILE_RESOURCE_MISSING',null],
    ['resource-case','src/godot-game/inspection/Inspection.tscn','PATH_CASE',text=>text.replace('assets/common/bow.png','assets/Common/bow.png')],
    ['resource-escape','src/godot-game/inspection/Inspection.tscn','PATH_INVALID',text=>text.replace('assets/common/bow.png','assets/common/../common/bow.png')],
    ['resource-absolute','src/godot-game/inspection/Inspection.tscn','PROFILE_SCENE_PATH',text=>text.replace('res://assets/common/bow.png','D:/assets/common/bow.png')],
    ['resource-alias','src/godot-game/inspection/Inspection.tscn','PROFILE_SCENE_PATH',text=>text.replace('res://assets/common/bow.png','uid://bow')],
    ['profile-path-escape',PROFILE,'PROFILE_RESOURCE_CONTRACT',text=>text.replace('src/godot-game/assets/common/bow.png','../bow.png')],
    ['profile-path-absolute',PROFILE,'PROFILE_RESOURCE_CONTRACT',text=>text.replace('src/godot-game/assets/common/bow.png','D:/bow.png')],
    ['profile-path-case',PROFILE,'PROFILE_RESOURCE_CONTRACT',text=>text.replace('src/godot-game/assets/common/bow.png','src/godot-game/assets/Common/bow.png')],
    ['profile-options',PROFILE,'PROFILE_OPTIONS_CONTRACT',text=>text.replace('"binary_format/architecture": "x86_64"','"binary_format/architecture": "arm64"')]
  ];
  for(const key of ['include_filter','export_filter','custom_template/debug','custom_template/release',
    'binary_format/architecture','binary_format/embed_pck','dotnet/include_scripts_content','dotnet/include_debug_symbols','dotnet/embed_build_outputs'])
    controls.push(['missing-option-'+key.replaceAll('/','-'),PRESET,'PROFILE_PRESET_OPTIONS',text=>text.split('\n').filter(line=>!line.startsWith(key+'=')).join('\n')]);
  for(const extension of ['json','png','wav','ogg','ttf','otf','md'])controls.push(['missing-filter-'+extension,PRESET,'PROFILE_PRESET_OPTIONS',
    text=>text.replace('*.'+extension+',','').replace(',*.'+extension+'"','"')]);
  const outcomes=[];
  for(const [name,file,code,edit]of controls){const trial=trialContext(context,name),root=copySubject(trial);
    if(edit)mutate(root,file,edit);else fs.unlinkSync(runner.inside(root,file));
    const mutation={file,before:before[file],after:fs.existsSync(runner.inside(root,file))?runner.hash(read(root,file)):null};
    write(trial.directory,'mutation.json',mutation);let failure;try{validate(root);}catch(error){failure=error;}
    write(trial.directory,'result.json',{nonce:trial.nonce,status:failure?'fail':'pass',expected:code,observed:failure?.message,mutation});
    need(failure&&failure.message.startsWith(code+' ')&&failure.message.includes(file),'PROFILE_BOUNDARY_RESULT',name+' '+failure?.message);
    outcomes.push({name,nonce:trial.nonce,directory:trial.directory,expected:code,observed:failure.message,subjectStatus:'fail'});
  }
  const override=trialContext(context,'runtime-override'),overrideRoot=copySubject(override);
  write(overrideRoot,'src/godot-game/override.cfg','[application]\nrun/main_scene="res://inspection/Inspection.tscn"\n');
  let rejected;try{validate(overrideRoot);}catch(error){rejected=error;}
  need(rejected?.message==='PROFILE_RUNTIME_OVERRIDE src/godot-game/override.cfg','PROFILE_BOUNDARY_RESULT','runtime-override '+rejected?.message);
  write(override.directory,'result.json',{nonce:override.nonce,status:'fail',expected:'PROFILE_RUNTIME_OVERRIDE',observed:rejected.message});
  outcomes.push({name:'runtime-override',nonce:override.nonce,directory:override.directory,expected:'PROFILE_RUNTIME_OVERRIDE',observed:rejected.message,subjectStatus:'fail'});
  write(context.directory,'source-controls.json',outcomes);
  const reader=[];
  for(const [name,file,size,code]of [
    ['theme-exact-2mib',THEME,THEME_LIMIT,null],['theme-over-2mib',THEME,THEME_LIMIT+1,'PROFILE_RESOURCE_SIZE'],
    ['ordinary-exact-1mib','src/godot-game/assets/common/bow.png',RESOURCE_LIMIT,null],
    ['ordinary-over-1mib','src/godot-game/assets/common/bow.png',RESOURCE_LIMIT+1,'PROFILE_RESOURCE_SIZE'],
    ['theme-empty',THEME,0,'PROFILE_RESOURCE_SIZE'],['theme-missing',THEME,null,'PROFILE_RESOURCE_MISSING'],
    ['theme-case','src/godot-game/assets/ui/inspectiontheme.tres',null,'PATH_CASE'],
    ['theme-alias','uid://InspectionTheme',null,'PATH_INVALID'],
    ['theme-traversal','src/godot-game/assets/ui/../ui/InspectionTheme.tres',null,'PATH_INVALID'],
    ['theme-sibling','src/godot-game/assets/ui/OtherTheme.tres',THEME_LIMIT,'PROFILE_RESOURCE_SIZE']
  ]){const trial=trialContext(context,name),root=copySubject(trial);
    if(size!==null)fs.writeFileSync(runner.inside(root,file),Buffer.alloc(size,32));
    else if(name==='theme-missing')fs.unlinkSync(runner.inside(root,file));
    let failure,value;try{value=read(root,file);}catch(error){failure=error;}
    need(code?failure?.message.startsWith(code+' '):!failure&&value.length===size,'PROFILE_READER_CONTROL',name+' '+failure?.message);
    const row={name,file,bytes:size,expected:code??'accepted',observed:failure?.message??'accepted',nonce:trial.nonce};
    write(trial.directory,'result.json',row);reader.push(row);
  }
  write(context.directory,'reader-controls.json',reader);
  return {outcomes,reader};
}
async function boundaries(context){
  const before=snapshot(ROOT),rows=registry();write(context.directory,'inputs.json',before);write(context.directory,'ordered-cases.json',rows);
  const {outcomes,reader}=sourceControls(context,before);
  const native=[];
  for(const [name,edit,expected]of [
    ['native-missing-override',text=>text.replace(/^run\/main_scene\.asset_inspection=.*\n/m,''),{qualified:'missing',inspection:FOUNDATION,active:false,actual:FOUNDATION}],
    ['native-setting-case',text=>text.replace('run/main_scene.asset_inspection','run/main_scene.ASSET_INSPECTION'),{qualified:'missing',inspection:INSPECTION,active:false,actual:FOUNDATION}],
    ['native-global-activation',text=>'_custom_features="asset_inspection"\n'+text,{qualified:INSPECTION,inspection:INSPECTION,active:true,actual:INSPECTION}]
  ]){const trial=trialContext(context,name),root=copySubject(trial);mutate(root,PROJECT,edit);
    const observation=await observe(root,trial,name,expected);write(trial.directory,'result.json',{nonce:trial.nonce,status:'pass',scope:SOURCE_ONLY,observation});
    native.push({name,nonce:trial.nonce,directory:trial.directory,observation});}
  const helper=await helperControls(context),restored=trialContext(context,'fresh-valid-control'),root=copySubject(restored);
  validate(root);const observation=await observe(root,restored,'restored');write(restored.directory,'result.json',{nonce:restored.nonce,status:'pass',observation});
  need(JSON.stringify(snapshot(ROOT))===JSON.stringify(before),'PROFILE_SOURCE_CHANGED',PROJECT);
  write(context.directory,'native-controls.json',native);
  return {sourceRejected:outcomes.length,outcomes,reader,native,helper,restored:{nonce:restored.nonce,directory:restored.directory,observation},
    orderedCases:rows.map(row=>({id:row.id,runGroup:row.runGroup,order:row.order})),scope:SOURCE_ONLY,nativeEntryComplete:false};
}
runner.registerCase('export.profile-boundaries',{...definition,title:'Independent profile, ordinary activation, path and bounded helper controls',order:351,
  actions:['Mutate independent owned profile/config/preset/resource copies and require exact fresh diagnostics',
    'Observe native missing override, case normalization and global activation on separate usable copies',
    'Exercise real child default/explicit env/cwd, deadline/output/nonzero/spawn rejection and descendant cleanup',
    'Validate original case subsets and global ID/order uniqueness; finish with fresh valid native control'],
  expected:'Every deliberate damaged source is rejected independently; native API semantics match pinned behavior; helper bounds and original registry identities survive',
  evidence:['inputs.json','ordered-cases.json','source-controls.json','native-controls.json','helper-controls.json','trials/*/result.json','result.json']},boundaries);
function selfTest(){
  const {test}=require('node:test'),assert=require('node:assert/strict');
  test('inspection export feature selects its own entry while ordinary Foundation stays literal',()=>{
    const project=fs.readFileSync(path.join(ROOT,'src/godot-game/game/project.godot'),'utf8');
    const preset=fs.readFileSync(path.join(ROOT,'src/godot-game/game/export_presets.cfg'),'utf8');
    assert.match(project,/^run\/main_scene="res:\/\/foundation\/Foundation\.tscn"$/m);
    assert.match(project,/^run\/main_scene\.asset_inspection="res:\/\/inspection\/Inspection\.tscn"$/m,
      'the permanent feature-qualified inspection entry is absent');
    assert.match(preset,/^custom_features="asset_inspection"$/m);
    assert.ok(fs.existsSync(path.join(ROOT,'src/godot-game/production/assets/export-profile.json')));
  });
}
function helperSelfTest(){
  const {test}=require('node:test'),assert=require('node:assert/strict');
  test('bounded command rejects a deadline exceeding sixty seconds before spawning',async()=>{
    const parent=path.join(ROOT,'.phase4/tdd/04-35');fs.mkdirSync(parent,{recursive:true});
    const directory=fs.mkdtempSync(path.join(parent,'helper-red-'));
    await assert.rejects(runner.command({directory},'oversized-deadline',process.execPath,['-e','process.exit(0)'],{timeout:60001}),
      /COMMAND_TIMEOUT_RANGE/,'a caller cannot extend the bounded child deadline');
    assert.equal(fs.existsSync(path.join(directory,'oversized-deadline.command.json')),false);
  });
}
function applicationIconControls(context){const before=snapshot(ROOT),root=copySubject(context),icon='src/godot-game/assets/ui/kingdom-zombies-icon.png',iconPath=runner.inside(root,icon),projectPath=runner.inside(root,PROJECT),source=fs.readFileSync(iconPath),project=fs.readFileSync(projectPath),controls=[];
  for(const kind of ['missing','changed','old-project','wrongcase-project']){let caught;try{if(kind==='missing')fs.unlinkSync(iconPath);if(kind==='changed')fs.writeFileSync(iconPath,Buffer.concat([source.subarray(0,-1),Buffer.from([source.at(-1)^1])]));if(kind==='old-project')fs.writeFileSync(projectPath,project.toString('utf8').replace('res://assets/ui/kingdom-zombies-icon.png','res://icon.svg'));if(kind==='wrongcase-project')fs.writeFileSync(projectPath,project.toString('utf8').replace('kingdom-zombies-icon.png','Kingdom-zombies-icon.png'));validate(root);}catch(error){caught=error;}finally{fs.writeFileSync(iconPath,source);fs.writeFileSync(projectPath,project);}
    need(caught,'PROFILE_ICON_CONTROL_ACCEPTED',kind);controls.push({id:kind,rejected:true,diagnostic:caught.message});validate(root);controls.push({id:kind+'-restored',passed:true});}
  need(JSON.stringify(snapshot(ROOT))===JSON.stringify(before),'PROFILE_ICON_SOURCE_DRIFT',PROJECT);write(context.directory,'icon-controls.json',{before,controls});return {controls,scope:SOURCE_ONLY,nativeEntryComplete:false};}
runner.registerCase('export.application-icon',{group:'export',title:'Current source icon config and resource binding without export acceptance',importance:'High',rationale:'Missing or aliased application art invalidates current delivery identity',needsGameWindow:false,runGroup:'G0-source-icon-profile',order:1,requirements:['ASSET-01','ASSET-05'],acceptance:['AC-03','AC-15','AC-23'],setup:'Current exact icon/project/profile bytes and original historical ordinary-config contract',actions:['Validate current source icon and bounded resource closure','Independently remove corrupt and alias icon/config in owned copies','Restore each positive and preserve original sourceContract/support scope'],expected:'Current exact source icon passes; every changed/missing/aliased subject rejects; final native export remains31-owned',reset:'Restore only disposable subject files; never mutate current source or retained archive',evidence:['icon-controls.json','result.json']},applicationIconControls);
module.exports={read,sceneClosure,sourceControls,validate,snapshot,probeScript,observe,profileCase,trialContext,copySubject,mutate,registry,helperControls,boundaries,selfTest,helperSelfTest,applicationIconControls};
if(require.main===module&&process.argv[2]==='--self-test')selfTest();
if(require.main===module&&process.argv[2]==='--self-test-helper')helperSelfTest();
