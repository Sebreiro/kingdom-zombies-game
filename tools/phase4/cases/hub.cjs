'use strict';
const fs=require('node:fs'),path=require('node:path'),assert=require('node:assert/strict');
const {spawnSync}=require('node:child_process');
const runner=require('../check.cjs');
const ROOT='D:/Work/Games/KingdomZombies';
const assembly=path.join(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll');
function env(){const base=path.join(ROOT,'.phase1');return {...process.env,DOTNET_CLI_HOME:path.join(base,'cli-home'),
  NUGET_PACKAGES:path.join(base,'packages'),NUGET_HTTP_CACHE_PATH:path.join(base,'http-cache'),
  TEMP:path.join(base,'temp'),TMP:path.join(base,'temp'),PHASE1_ROOT:ROOT,DOTNET_NOLOGO:'1'};}
function compiled(){return spawnSync('dotnet',[assembly,'--filter-class','*ContentInspectorTests'],
  {cwd:ROOT,env:env(),encoding:'utf8',windowsHide:true,timeout:60000});}
// The driver is a test client of the unchanged real compiled scene. Signals invoke its C# callbacks;
// it neither replaces the script nor manufactures projected labels/results.
const DRIVER=String.raw`extends SceneTree
var hub: Node
var outcomes: Array = []
var failed := false
const BROWSER = "Shell/Workspace/Browser/"
const PREVIEW = "Shell/Workspace/Preview/"
func _initialize() -> void:
	call_deferred("run")
func check_case(id: String, ok: bool, observed: Variant) -> void:
	outcomes.append({"id": id, "pass": ok, "observed": observed})
	if not ok:
		failed = true
func category(index: int) -> void:
	var control = hub.get_node("Shell/Workspace/Navigation/Category")
	control.select(index)
	control.item_selected.emit(index)
func query(value: String) -> void:
	var control = hub.get_node(BROWSER + "Search")
	control.text = value
	control.text_changed.emit(value)
func status(index: int) -> void:
	var control = hub.get_node(BROWSER + "Status")
	control.select(index)
	control.item_selected.emit(index)
func button(node_path: String) -> void:
	hub.get_node(node_path).pressed.emit()
func rows() -> Array:
	var values: Array = []
	var list = hub.get_node(BROWSER + "Resources")
	for i in list.item_count:
		values.append(list.get_item_text(i))
	return values
func choose(id: String) -> bool:
	var list = hub.get_node(BROWSER + "Resources")
	for i in list.item_count:
		if list.get_item_text(i).begins_with(id + " · "):
			list.select(i)
			list.item_selected.emit(i)
			return true
	return false
func settle() -> void:
	for i in range(120):
		await process_frame
		if hub.get_node("Shell/Toolbar/Cancel").disabled:
			return
	check_case("bounded-load", false, "120 frames exhausted")
func restore(file: String, data: PackedByteArray) -> void:
	var handle = FileAccess.open(file, FileAccess.WRITE)
	handle.store_buffer(data)
	handle.close()
func run() -> void:
	var scene = load("res://inspection/Inspection.tscn")
	hub = scene.instantiate()
	root.add_child(hub)
	check_case("real-csharp-script", hub.get_script() is CSharpScript, str(hub.get_script()))
	check_case("loading", hub.get_node("Summary/Status").text.contains("Loading") and not hub.get_node("Bow").visible, hub.get_node("Summary/Status").text)
	await settle()
	var bow = hub.get_node("Bow")
	check_case("natural-bow", hub.get_node("Summary/ResourceId").text == "common.bow" and bow.visible and hub.get_node("Summary/Status").text.begins_with("Partial"), hub.get_node("Summary/Status").text)
	var image = bow.texture.get_image()
	image.convert(Image.FORMAT_RGBA8)
	var original = Image.load_from_file("res://assets/common/bow.png")
	original.convert(Image.FORMAT_RGBA8)
	var pixels = image.get_data()
	var source_pixels = original.get_data()
	var exact: bool = pixels.size() == source_pixels.size()
	var visible := 0
	for i in range(0, pixels.size(), 4):
		exact = exact and pixels[i+3] == source_pixels[i+3]
		if pixels[i+3] > 0:
			visible += 1
			exact = exact and pixels[i] == source_pixels[i] and pixels[i+1] == source_pixels[i+1] and pixels[i+2] == source_pixels[i+2]
	var digest = HashingContext.new()
	digest.start(HashingContext.HASH_SHA256)
	digest.update(source_pixels)
	check_case("native-bow-registration", exact and visible == 42 and bow.texture.resource_path == "res://assets/common/bow.png" and image.get_size() == Vector2i(16,18) and not bow.centered and bow.offset == Vector2(-6,-9) and bow.scale == Vector2.ONE and bow.texture_filter == 1, {"texture": bow.texture.resource_path, "offset": str(bow.offset), "size": str(image.get_size()), "sourceRgbaSha256": digest.finish().hex_encode(), "visiblePixels": visible, "importedVisiblePixelsAndAlphaExact": exact})
	check_case("independent-summary", hub.get_node("Summary/PackStatus").text.contains("0/180 obligations") and hub.get_node("Summary/PackStatus").text.contains("Pack incomplete") and not hub.get_node(PREVIEW + "Metadata").visible, hub.get_node("Summary/PackStatus").text)
	var navigation = hub.get_node("Shell/Workspace/Navigation/Category")
	var categories: Array = []
	for i in navigation.item_count:
		category(i)
		categories.append({"name":navigation.get_item_text(i),"rows":rows()})
	check_case("categories", categories.size() == 8 and categories[0].rows.size() == 31 and categories[5].rows.size() == 30, categories)
	category(6)
	check_case("register", rows().size() == 181, rows())
	query("IT-07")
	check_case("one-result", rows().size() == 1 and hub.get_node(BROWSER + "Count").text == "1 result" and choose("IT-07"), rows())
	check_case("exact-obligation-diagnostic", hub.get_node(PREVIEW + "Failures").text.contains("IT-07: res://content/expected-obligations.json, field acceptance, value IT-07") and hub.get_node(PREVIEW + "Failures").visible, hub.get_node(PREVIEW + "Failures").text)
	query("NO-SUCH-RESOURCE")
	check_case("zero-result", rows().is_empty() and hub.get_node(BROWSER + "Count").text.begins_with("0 results"), hub.get_node(BROWSER + "Count").text)
	query("bow")
	check_case("name-search", rows().size() >= 2, rows())
	query("")
	check_case("clear-search", rows().size() == 181, rows().size())
	status(1)
	check_case("partial-filter", rows().size() == 1 and choose("common.bow"), rows())
	button(PREVIEW + "Actions/Details")
	check_case("details-mappings", hub.get_node(PREVIEW + "Metadata").visible and hub.get_node(PREVIEW + "Metadata").text.contains("Explicit shared mappings: IT-07"), hub.get_node(PREVIEW + "Metadata").text)
	button(PREVIEW + "Actions/Copy")
	check_case("copy-action", hub.get_node("Shell/Toolbar/Operation").text == "Copied identity, metadata and exact diagnostics", hub.get_node("Shell/Toolbar/Operation").text)
	button(PREVIEW + "Actions/Details")
	check_case("details-close", not hub.get_node(PREVIEW + "Metadata").visible and hub.get_node(PREVIEW + "Failures").visible, hub.get_node(PREVIEW + "Actions/Details").text)
	status(2)
	check_case("incomplete-filter", rows().size() == 180, rows().size())
	status(0)
	category(7)
	check_case("known-issues", hub.get_node(PREVIEW + "Failures").text.contains("content/packs/") and hub.get_node(PREVIEW + "Failures").text.contains("missing"), hub.get_node(PREVIEW + "Failures").text)
	category(1)
	check_case("category-selection", hub.get_node("Summary/ResourceId").text == "common.bow" and bow.visible, hub.get_node("Summary/ResourceId").text)
	button("Shell/Toolbar/Recheck")
	button("Shell/Toolbar/Cancel")
	for i in range(5):
		await process_frame
	check_case("cancel-fence", not bow.visible and hub.get_node("Summary/PackStatus").text.contains("Cancelled") and hub.get_node("Shell/Toolbar/Cancel").disabled, hub.get_node("Summary/PackStatus").text)
	button("Shell/Toolbar/Recheck")
	await settle()
	check_case("fresh-recheck", bow.visible and hub.get_node("Summary/Status").text.begins_with("Partial"), hub.get_node("Summary/Status").text)
	var expected_path = "res://content/expected-obligations.json"
	var expected = FileAccess.get_file_as_bytes(expected_path)
	DirAccess.remove_absolute(ProjectSettings.globalize_path(expected_path))
	button("Shell/Toolbar/Recheck")
	await settle()
	check_case("missing-register-valid-bow", bow.visible and hub.get_node("Summary/PackStatus").text.contains("0/180") and hub.get_node(PREVIEW + "Failures").text.contains(expected_path), hub.get_node(PREVIEW + "Failures").text)
	category(6)
	query("IT-07")
	check_case("missing-register-census", rows().size() == 1 and rows()[0].contains("Unavailable metadata"), rows())
	restore(expected_path, expected)
	query("")
	category(1)
	var media_path = "res://assets/common/bow.png"
	var media = FileAccess.get_file_as_bytes(media_path)
	DirAccess.remove_absolute(ProjectSettings.globalize_path(media_path))
	button("Shell/Toolbar/Recheck")
	await settle()
	category(7)
	check_case("missing-media", not bow.visible and hub.get_node("Summary/PackStatus").text.contains("0 inspectable outputs") and hub.get_node(PREVIEW + "Failures").text.contains(media_path), hub.get_node(PREVIEW + "Failures").text)
	restore(media_path, media)
	category(1)
	button("Shell/Toolbar/Recheck")
	await settle()
	check_case("restored-native-bow", bow.visible and hub.get_node("Summary/ResourceId").text == "common.bow", hub.get_node("Summary/PackStatus").text)
	var result = {"pid":OS.get_process_id(),"script":hub.get_script().resource_path,"outcomes":outcomes,"failed":failed,"scope":"actual compiled C# scene and native controls; headless, no rendered or physical input credit"}
	restore("res://native-results.json", JSON.stringify(result, "\t").to_utf8_buffer())
	root.remove_child(hub)
	hub.free()
	await process_frame
	quit(1 if failed else 0)
`;
const LIFECYCLE=String.raw`
	check_case("preview-transport-exists", hub.has_node("Shell/Workspace/Preview/Transport/Play"), "The current preview owns explicit play/pause, speed, step and reset Controls")
`;
async function native(context,lifecycle=false){
  const game=path.join(ROOT,'src/godot-game'),project=path.join(context.directory,'native-project');
  fs.mkdirSync(project,{recursive:true});
  const build=await runner.command(context,'native-build','dotnet',['build',path.join(game,'KingdomZombies.csproj'),
    '--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:env()});
  assert.match(build.stdout,/0 Warning\(s\)[\s\S]*0 Error\(s\)/);
  for(const item of ['project.godot','inspection','content','assets/common'])
    fs.cpSync(path.join(game,item),path.join(project,item),{recursive:true});
  const binaries='.godot/mono/temp/bin/Debug';
  fs.cpSync(path.join(game,binaries),path.join(project,binaries),{recursive:true});
  const closure=Object.fromEntries(fs.readdirSync(path.join(game,binaries)).filter(x=>/\.(dll|json)$/.test(x))
    .map(x=>[x,runner.hash(fs.readFileSync(path.join(game,binaries,x)))]));
  fs.writeFileSync(path.join(context.directory,'native-assemblies.json'),JSON.stringify(closure,null,2));
  fs.writeFileSync(path.join(project,'probe.gd'),lifecycle?DRIVER.replace('\tvar result = {',LIFECYCLE+'\tvar result = {'):DRIVER);
  const pins=JSON.parse(fs.readFileSync(path.join(ROOT,'tools/phase1/toolchain.json'),'utf8'));
  const pin=pins.archives[process.platform+'-'+process.arch];
  const console=path.join(ROOT,'.phase1/tools',pin.directory,pin.executable);
  assert.equal(runner.hash(fs.readFileSync(console)),pin.executableSha256,'GODOT_CONSOLE_PIN');
  // Run the actual engine rather than its Windows console wrapper, authenticating the companion
  // directly against its entry in the already pinned installed distribution archive.
  const archive=path.join(ROOT,'.phase1/cache',pin.file),name=pin.executable.replace('_console.exe','.exe');
  assert.equal(runner.hash(fs.readFileSync(archive)),pin.sha256,'GODOT_ARCHIVE_PIN');
  const authentication=await runner.command(context,'native-tool-origin','powershell.exe',['-NoProfile','-NonInteractive','-Command',
    "Add-Type -AssemblyName System.IO.Compression.FileSystem; $zip=[IO.Compression.ZipFile]::OpenRead('D:/Work/Games/KingdomZombies/.phase1/cache/Godot_v4.7.2-stable_mono_win64.zip'); try { $entry=$zip.Entries | Where-Object { $_.Name -ceq 'Godot_v4.7.2-stable_mono_win64.exe' }; if(@($entry).Count -ne 1){throw 'ARCHIVE_ENTRY'}; $stream=$entry.Open(); try { $sha=[Security.Cryptography.SHA256]::Create(); [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-','').ToLowerInvariant() } finally { $stream.Dispose() } } finally { $zip.Dispose() }"]);
  const executable=path.join(path.dirname(console),name);
  assert.equal(runner.hash(fs.readFileSync(executable)),authentication.stdout.trim(),'GODOT_NATIVE_ENTRY_PIN');
  const isolated={...env(),...runner.environment(context.directory),NUGET_PACKAGES:path.join(ROOT,'.phase1/packages')};
  await runner.command(context,'native-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env:isolated});
  const run=await runner.command(context,'native-hub',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env:isolated});
  assert.doesNotMatch(run.stdout+run.stderr,/ERROR:|SCRIPT ERROR:|Failed to load/i);
  const result=JSON.parse(fs.readFileSync(path.join(project,'native-results.json'),'utf8'));
  assert.equal(result.pid,run.pid,'ACTUAL_NATIVE_PID');assert.equal(result.script,'res://inspection/InspectionRoot.cs');
  assert.equal(result.failed,false);assert.ok(result.outcomes.length>=24&&result.outcomes.every(x=>x.pass));
  const image=result.outcomes.find(x=>x.id==='native-bow-registration').observed;
  assert.equal(image.sourceRgbaSha256,'88b8e7062f0eb9cfb788ef3658028ba58a6f62d408d3e39ae2e1228bfa6c8905');
  fs.writeFileSync(path.join(context.directory,'native-observation.json'),JSON.stringify(result,null,2));
  return result;
}
runner.registerCase('hub.source',{group:'hub',title:'Independent portable register and valid partial hub resources',importance:'Critical',
  rationale:'One invalid resource must not hide valid content or erase required obligations',needsGameWindow:false,
  runGroup:'G0-hub',order:1,requirements:['ASSET-02','ASSET-03','ASSET-04','ASSET-05'],acceptance:['AC-16','AC-20'],
  setup:'Strict actual C# production build, compiled loader controls and isolated byte-identical native hub with real assembly bindings',
  actions:['Run compiled loader boundary controls','Drive actual native category/search/filter/details/copy signals',
    'Observe frame-driven loading/cancel/recheck, missing register/media and restored native bow'],
  expected:'Nonzero compiled discovery and actual native observations; valid bow survives independent failures with complete180 census',
  reset:'Owned isolated resources; restore every mutated file and destroy the scene; no game window',
  evidence:['compiled.command.json','inputs.json','native-build.command.json','native-assemblies.json','native-hub.command.json','native-observation.json','result.json']},async context=>{
    const result=compiled();fs.writeFileSync(path.join(context.directory,'compiled.command.json'),JSON.stringify(result,null,2));
    assert.equal(result.status,0,result.stdout+'\n'+result.stderr);assert.match(result.stdout,/summary: Passed!/);
    const discovered=/total:\s*(\d+)/.exec(result.stdout);assert.ok(discovered&&Number(discovered[1])>=26,'NONZERO_REQUIRED_DISCOVERY');
    const inputs=Object.fromEntries(['src/core/ContentPack.cs','src/godot-game/inspection/InspectionRoot.cs',
      'src/godot-game/inspection/Inspection.tscn','src/godot-game/content/pack-index.json',
      'src/godot-game/content/expected-obligations.json','src/godot-game/content/packs/tracer.json',
      'src/godot-game/assets/common/bow.png','tests/phase1/ContentInspectorTests.cs',
      'tools/phase4/cases/hub.cjs'].map(file=>[file,runner.hash(fs.readFileSync(path.join(ROOT,file)))]));
    fs.writeFileSync(path.join(context.directory,'inputs.json'),JSON.stringify(inputs,null,2));
    const observed=await native(context);
    return {compiled:true,discovered:Number(discovered[1]),native:observed.outcomes.length,pid:observed.pid,
      scope:'actual compiled pure loader and C# native hub behavior; exact source policy Task2; no rendered acceptance'};
  });
runner.registerCase('hub.lifecycle',{group:'hub',title:'Inspection input, transport and owned generation lifecycle',importance:'Critical',
  rationale:'Stale callbacks or duplicated input must not replace the selected preview or accumulate native owners',needsGameWindow:false,
  runGroup:'G0-hub',order:2,requirements:['ASSET-02','ASSET-03','ASSET-04','ASSET-05'],acceptance:['AC-04','AC-16','AC-20'],
  setup:'Actual compiled production CSharp scene and native Controls in an isolated headless project',
  actions:['Inspect explicit preview transport','Drive owned keys, category restoration and repeated reset','Observe late generation rejection and teardown'],
  expected:'One current preview; text and popups own input; paused stepping uses actual authored samples; obsolete results are rejected',
  reset:'Close acceptance, cancel work, disconnect handlers and retire every owned preview; no game window',
  evidence:['native-observation.json','native-build.command.json','native-assemblies.json','native-hub.command.json','result.json']},async context=>{
    const result=await native(context,true);return {native:result.outcomes.length,pid:result.pid,scope:result.scope};
  });
if(require.main===module&&process.argv.includes('--self-test-lifecycle')){
  require('node:test').test('inspection preview owns explicit transport and lifetime',async()=>{
    const directory=path.join(ROOT,'.phase4/lifecycle-red',require('node:crypto').randomUUID());
    fs.mkdirSync(directory,{recursive:true});
    try{await native({directory},true);}catch(error){
      if(!error.message.includes('CHILD_FAILED common.bow native-hub {"code":1}'))throw error;
    }
    const result=JSON.parse(fs.readFileSync(path.join(directory,'native-project/native-results.json'),'utf8'));
    for(const outcome of result.outcomes)assert.equal(outcome.pass,true,outcome.id+': '+JSON.stringify(outcome.observed));
  });
}
if(require.main===module&&process.argv.includes('--self-test')){
  require('node:test').test('valid bow survives incomplete independent register',()=>{
    const result=compiled();assert.equal(result.status,0,result.stdout+'\n'+result.stderr);
  });
}
