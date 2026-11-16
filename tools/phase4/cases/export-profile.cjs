'use strict';
const fs=require('node:fs'),path=require('node:path');
const ROOT=path.resolve(__dirname,'../../..');
const runner=require('../check.cjs');
const PROFILE='production/phase4/export-profile.json',PROJECT='src/godot-game/project.godot',PRESET='src/godot-game/export_presets.cfg';
const FEATURE='asset_inspection',BASE='application/run/main_scene',QUALIFIED=BASE+'.'+FEATURE;
const FOUNDATION='res://foundation/Foundation.tscn',INSPECTION='res://inspection/Inspection.tscn';
const SOURCE_ONLY='source/headless configuration/resource proof only; actual packaged Inspection startup pending 04-02 Task 1';
const CONTRACT={ordinaryProjectSha256:'1f7d242e12521a4866512b6017f25f1868cd6160e3c9efe0074ad498008b2a9d',
  presetBeforeFeatureSha256:'7fe9294d4a160582af981a9ec00167f8bf8bb407b6e86ce8b4bbde226bed3b00',
  projectGraphSha256:'717bc7e8898553fe62a632fef80b0bbdc10425ecdc18668f8a39cb5be30a443d',
  checkpoint:'.phase4/checkpoints/04-02-entry-93386745/manifest.json',
  fullRunnerBeforeProfileSha256:'2e022d88b462d361bc1807fc54ed031245e93f167edd6c5c7b07837a49d0a021'};
const RESOURCES=['foundation/Foundation.tscn','inspection/Inspection.tscn','assets/common/bow.png','content/pack-index.json',
  'content/packs/tracer.json','icon.svg','docs/README.md'].map(file=>'src/godot-game/'+file);
function need(value,code,detail){if(!value)throw new Error(code+' '+detail);}
function read(root,file){const target=runner.inside(root,file);need(fs.existsSync(target),'PROFILE_RESOURCE_MISSING',file);
  const data=fs.readFileSync(target);need(data.length>0&&data.length<=1048576,'PROFILE_RESOURCE_SIZE',file);return data;}
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
  need(JSON.stringify(profile.sourceContract)===JSON.stringify(CONTRACT),'PROFILE_SOURCE_CONTRACT',PROFILE+' sourceContract');
  need(JSON.stringify(profile.resources)===JSON.stringify(RESOURCES),'PROFILE_RESOURCE_CONTRACT',PROFILE+' resources');
  need(project.values.get(BASE)===JSON.stringify(FOUNDATION),'PROFILE_BASE_MAIN',PROJECT+' '+BASE);
  need(project.values.get(QUALIFIED)===JSON.stringify(INSPECTION),'PROFILE_OVERRIDE',PROJECT+' '+QUALIFIED);
  need(preset.values.get('preset.0/name')===JSON.stringify(profile.preset),'PROFILE_PRESET',PRESET+' preset.0/name');
  need(preset.values.get('preset.0/custom_features')===JSON.stringify(FEATURE),'PROFILE_FEATURE',PRESET+' preset.0/custom_features');
  const original=project.text.replace('run/main_scene.asset_inspection="res://inspection/Inspection.tscn"\n','');
  need(runner.hash(original)===profile.sourceContract.ordinaryProjectSha256,'PROFILE_ORDINARY_CONFIG',PROJECT);
  const originalPreset=preset.text.replace('custom_features="asset_inspection"','custom_features=""');
  need(runner.hash(originalPreset)===profile.sourceContract.presetBeforeFeatureSha256,'PROFILE_PRESET_OPTIONS',PRESET);
  need(!fs.existsSync(runner.inside(root,'src/godot-game/override.cfg')),'PROFILE_RUNTIME_OVERRIDE','src/godot-game/override.cfg');
  for(const relative of profile.resources){need(typeof relative==='string'&&relative.startsWith('src/godot-game/'),
    'PROFILE_RESOURCE_PATH',PROFILE+' '+relative);read(root,relative);}
  const scene=read(root,'src/godot-game/inspection/Inspection.tscn').toString('utf8');
  for(const match of scene.matchAll(/path="([^"]+)"/g)){
    need(match[1].startsWith('res://'),'PROFILE_SCENE_PATH','src/godot-game/inspection/Inspection.tscn '+match[1]);
    read(root,'src/godot-game/'+match[1].slice(6));
  }
  const graph=read(root,'tools/phase1/projects.json');need(runner.hash(graph)===profile.sourceContract.projectGraphSha256,'PROFILE_GRAPH','tools/phase1/projects.json');
  return profile;
}
function snapshot(root){const profile=validate(root);return Object.fromEntries([PROFILE,PROJECT,PRESET,'tools/phase1/projects.json',
  ...profile.resources].map(file=>[file,runner.hash(read(root,file))]));}
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
        "active":OS.has_feature("${FEATURE}"), "foundationExists":FileAccess.file_exists("${FOUNDATION}")}
    print("PROFILE_SETTINGS " + JSON.stringify(observation))
    var packed = load("${INSPECTION}")
    if not packed is PackedScene:
        reject("inspection/Inspection.tscn SCENE_LOAD")
        return
    var scene = packed.instantiate()
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
    scene.free()
    quit(0)
`;}
async function observe(root,context,name='profile'){
  const pins=runner.document(ROOT,'tools/phase1/toolchain.json'),pin=pins.archives['win32-x64'];
  need(process.platform==='win32'&&process.arch==='x64'&&process.version==='v'+pins.node,'PROFILE_PLATFORM','tools/phase1/toolchain.json');
  const relative='.phase1/tools/'+pin.directory+'/'+pin.executable,executable=runner.inside(ROOT,relative);
  need(runner.hash(fs.readFileSync(executable))===pin.executableSha256,'PROFILE_ENGINE_HASH',relative);
  const version=await runner.command(context,name+'-version',executable,['--headless','--version']);
  need(version.stdout.trim()===pins.engineFullVersion,'PROFILE_ENGINE_VERSION',relative);
  const project=runner.inside(context.directory,name+'-project');fs.mkdirSync(project);
  const profile=runner.document(root,PROFILE),inputs={};
  for(const file of [PROJECT,...profile.resources]){const data=read(root,file);inputs[file]=runner.hash(data);write(project,file.slice('src/godot-game/'.length),data);}
  const script=probeScript(context.nonce);write(project,'probe.gd',script);
  // Import media in a separate asset-only fixture. The settings subject keeps the exact production bytes.
  const importer=runner.inside(context.directory,name+'-importer');fs.mkdirSync(importer);
  write(importer,'project.godot','config_version=5\n[application]\nconfig/name="Profile media import fixture"\n');
  for(const file of profile.resources.filter(file=>/\.(png|svg)$/.test(file)))write(importer,file.slice('src/godot-game/'.length),read(root,file));
  write(context.directory,name+'-inputs.json',{inputs,probeSha256:runner.hash(script),engineSha256:pin.executableSha256,
    version:pins.engineFullVersion,scope:SOURCE_ONLY});
  const imported=await runner.command(context,name+'-import',executable,['--headless','--path',importer,'--editor','--import']);
  need(!/ERROR:|SCRIPT ERROR:|Failed to load|Error importing/i.test(imported.stdout+imported.stderr),'PROFILE_NATIVE_IMPORT',name+'-import.command.json');
  fs.cpSync(path.join(importer,'.godot/imported'),path.join(project,'.godot/imported'),{recursive:true,errorOnExist:true,force:false});
  for(const file of profile.resources.filter(file=>/\.(png|svg)$/.test(file))){const relative=file.slice('src/godot-game/'.length)+'.import';
    write(project,relative,read(importer,relative));}
  const result=await runner.command(context,name+'-settings',executable,['--headless','--path',project,'--script','res://probe.gd'],{timeout:15000});
  need(!/ERROR:|SCRIPT ERROR:|Failed to load/i.test(result.stdout+result.stderr),'PROFILE_NATIVE_RESOURCE','inspection/Inspection.tscn');
  const lines=result.stdout.split(/\r?\n/).filter(line=>line.startsWith('PROFILE_SETTINGS '));
  need(lines.length===1,'PROFILE_OBSERVATION_MISSING',name);const observation=JSON.parse(lines[0].slice(17));
  need(observation.nonce===context.nonce&&observation.base===FOUNDATION&&observation.qualified===INSPECTION&&
    observation.actual===FOUNDATION&&observation.empty===FOUNDATION&&observation.ordinary===FOUNDATION&&
    observation.inspection===INSPECTION&&observation.wrongCase===FOUNDATION&&!observation.active&&observation.foundationExists,
  'PROFILE_NATIVE_SETTINGS',JSON.stringify(observation));
  need(result.stdout.includes('PROFILE_RESOURCE '+context.nonce+' inspection/Inspection.tscn assets/common/bow.png pixels=42'),
    'PROFILE_RESOURCE_OBSERVATION_MISSING','assets/common/bow.png');
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
function selfTest(){
  const {test}=require('node:test'),assert=require('node:assert/strict');
  test('inspection export feature selects its own entry while ordinary Foundation stays literal',()=>{
    const project=fs.readFileSync(path.join(ROOT,'src/godot-game/project.godot'),'utf8');
    const preset=fs.readFileSync(path.join(ROOT,'src/godot-game/export_presets.cfg'),'utf8');
    assert.match(project,/^run\/main_scene="res:\/\/foundation\/Foundation\.tscn"$/m);
    assert.match(project,/^run\/main_scene\.asset_inspection="res:\/\/inspection\/Inspection\.tscn"$/m,
      'the permanent feature-qualified inspection entry is absent');
    assert.match(preset,/^custom_features="asset_inspection"$/m);
    assert.ok(fs.existsSync(path.join(ROOT,'production/phase4/export-profile.json')));
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
module.exports={validate,snapshot,probeScript,observe,profileCase,selfTest,helperSelfTest};
if(require.main===module&&process.argv[2]==='--self-test')selfTest();
if(require.main===module&&process.argv[2]==='--self-test-helper')helperSelfTest();
