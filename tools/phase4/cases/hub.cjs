'use strict';
const fs=require('node:fs'),path=require('node:path'),assert=require('node:assert/strict');
const {spawnSync}=require('node:child_process');
const runner=require('../check.cjs');
const ROOT='D:/Work/Games/KingdomZombies';
const assembly=path.join(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll');
function env(){const base=path.join(ROOT,'.phase1');return {...process.env,DOTNET_CLI_HOME:path.join(base,'cli-home'),
  NUGET_PACKAGES:path.join(base,'packages'),NUGET_HTTP_CACHE_PATH:path.join(base,'http-cache'),
  TEMP:path.join(base,'temp'),TMP:path.join(base,'temp'),PHASE1_ROOT:ROOT,DOTNET_NOLOGO:'1'};}
const COMPILED_SUBJECTS=[
  ['Observe_Inspection_RecordsActualGeneratedDiagnostics',1],
  ['Authorize_Inspection_RequiresExactInvocationTuples',1],
  ['Authorize_InternalInspection_RequiresExactInvocation',2],
  ['Authorize_PublicContracts_RejectsAdjacentTypesAndPaths',1],
  ['Load_PartialPack_PreservesBowAndCensus',1],['Load_ReorderedShards_PreservesIndependentDiagnostics',1],
  ['Load_BadExpectations_PreservesCensusAndBow',6],['Load_MalformedIndex_ReturnsExactFailure',4],
  ['ResourcePath_Aliases_Reject',8],['Load_MissingMediaAndRecheck_DetachesPublishedState',1],
  ['Load_DuplicateNames_PreservesIdsAndSearchCounts',1],['Load_InvalidIndependentOutput_RetainsValidBow',4],
  ...[['Root',18],['Input',2],['Shell',12]].flatMap(([type,count])=>Array.from({length:count},(_,i)=>
    [`Authorize_${type}${String(i+1).padStart(2,'0')}_RequiresExactInvocation`,1]))];
function compiled(directory){
  const declared=COMPILED_SUBJECTS.map(([method,count])=>({method,count}));
  assert.equal(new Set(declared.map(x=>x.method)).size,declared.length,'NO_DUPLICATE_COMPILED_SUBJECT');
  fs.writeFileSync(path.join(directory,'compiled-selection.json'),JSON.stringify(declared,null,2));
  const discovery=spawnSync('dotnet',[assembly,'--list-tests','--filter-class','*ContentInspectorTests','--no-ansi'],
    {cwd:ROOT,env:env(),encoding:'utf8',windowsHide:true,timeout:60000});
  fs.writeFileSync(path.join(directory,'compiled-discovery.command.json'),JSON.stringify(discovery,null,2));
  assert.equal(discovery.status,0,discovery.stdout+'\n'+discovery.stderr);
  const names=[...discovery.stdout.matchAll(/ContentInspectorTests\.([A-Za-z0-9_]+)/g)].map(x=>x[1]);
  assert.deepEqual([...new Set(names)].sort(),declared.map(x=>x.method).sort(),'FULL_INDEPENDENT_DISCOVERY');
  assert.equal(names.length,declared.reduce((sum,x)=>sum+x.count,0),'FULL_DISCOVERY_COUNT');
  const children=[];
  for(const row of declared){
    const child=path.join(directory,'compiled-'+row.method);fs.mkdirSync(child,{recursive:true});
    const args=[assembly,'--filter-method','*ContentInspectorTests.'+row.method,'--report-xunit-ctrf',
      '--report-xunit-ctrf-filename','tests.json','--results-directory',child,'--no-ansi','--progress','off',
      '--parallel','none','--explicit','on','--timeout','45s'];
    const result=spawnSync('dotnet',args,{cwd:ROOT,env:env(),encoding:'utf8',windowsHide:true,timeout:60000});
    fs.writeFileSync(path.join(child,'command.json'),JSON.stringify({args,...result},null,2));
    assert.equal(result.status,0,result.stdout+'\n'+result.stderr);
    const report=JSON.parse(fs.readFileSync(path.join(child,'tests.json'),'utf8'));
    const tests=report.results.tests,summary=report.results.summary;
    assert.equal(summary.tests,row.count,row.method);assert.equal(summary.passed,row.count,row.method);
    assert.equal(summary.failed,0,row.method);assert.equal(summary.skipped,0,row.method);
    assert.equal(tests.length,row.count,row.method);
    for(const test of tests){assert.equal(test.status,'passed');assert.ok(test.name.includes(row.method),test.name);}
    children.push({method:row.method,count:row.count,report:path.relative(directory,path.join(child,'tests.json')),
      reportSha256:runner.hash(fs.readFileSync(path.join(child,'tests.json')))});
    console.log('COMPILED '+row.method+' '+row.count+'/'+row.count);
  }
  assert.deepEqual(children.map(x=>[x.method,x.count]),COMPILED_SUBJECTS,'COMPLETE_DECLARED_UNION');
  const union={children,discovered:names.length,members:32,restoredPositives:32,compilerValidNegatives:352};
  fs.writeFileSync(path.join(directory,'compiled.command.json'),JSON.stringify(union,null,2));
  return union;
}
// The driver is a test client of the unchanged real compiled scene. Signals invoke its C# callbacks;
// it neither replaces the script nor manufactures projected labels/results.
const DRIVER=String.raw`extends SceneTree
var hub: Node
var outcomes: Array = []
var failed := false
const BROWSER = "Shell/Workspace/Controls/Browser/"
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
	var source_file = FileAccess.open("res://assets/common/bow.png", FileAccess.READ)
	if source_file == null or source_file.get_length() != 135:
		source_file = null
		image = null
		root.remove_child(hub)
		hub.free()
		printerr("HUB_SOURCE_PNG_EXTENT")
		quit(1)
		return
	var source_png = source_file.get_buffer(135)
	var source_complete = source_png.size() == 135 and source_file.get_error() in [OK, ERR_FILE_EOF]
	source_file.close()
	source_file = null
	var source_png_digest = HashingContext.new()
	source_png_digest.start(HashingContext.HASH_SHA256)
	source_png_digest.update(source_png)
	var source_png_sha = source_png_digest.finish().hex_encode()
	source_png_digest = null
	if not source_complete or source_png_sha != "08b53699e459d71cf73f3e7be84af19d04805d5a7a488121e39a514a81701999":
		image = null
		root.remove_child(hub)
		hub.free()
		printerr("HUB_SOURCE_PNG_IDENTITY")
		quit(1)
		return
	var original = Image.new()
	if original.load_png_from_buffer(source_png) != OK:
		original = null
		image = null
		root.remove_child(hub)
		hub.free()
		printerr("HUB_SOURCE_PNG_DECODE")
		quit(1)
		return
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
	original = null
	image = null
	digest = null
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
const STATUS_CONTROLS=String.raw`
func run() -> void:
	hub = load("res://inspection/Inspection.tscn").instantiate()
	root.add_child(hub)
	await settle()
	check_case("current-startup-selection", hub.get_node("Summary/ResourceId").text == "common.bow", hub.get_node("Summary/ResourceId").text)
	category(1)
	status(3)
	check_case("healthy-error-empty", rows().is_empty() and hub.get_node("Summary/ResourceId").text.is_empty()
		and hub.get_node("Summary/Status").text.begins_with("Unavailable"), rows())
	status(1)
	query("IT-07")
	check_case("healthy-sibling-partial", rows().size() == 1 and rows()[0].ends_with(" · Partial")
		and choose("IT-07") and hub.get_node("Summary/Status").text.begins_with("Partial"), rows())
	query("NO-SUCH-RESOURCE")
	check_case("empty-search-selection", rows().is_empty() and hub.get_node("Summary/ResourceId").text.is_empty()
		and hub.get_node("Summary/ResourceName").text == "No resource selected", rows())
	query("IT-07")
	category(0)
	var list = hub.get_node(BROWSER + "Resources")
	var character_rows = list.get_selected_items()
	check_case("new-category-valid-selection", character_rows.size() == 1
		and list.get_item_text(character_rows[0]).begins_with(hub.get_node("Summary/ResourceId").text + " · ")
		and hub.get_node("Summary/ResourceId").text.begins_with("AN-"), hub.get_node("Summary/ResourceId").text)
	category(1)
	check_case("restored-category-selection", hub.get_node(BROWSER + "Search").text == "IT-07"
		and hub.get_node(BROWSER + "Status").selected == 1 and hub.get_node("Summary/ResourceId").text == "IT-07", rows())
	var missing_path = "res://content/packs/items-11-24.json"
	var original = FileAccess.get_file_as_bytes(missing_path)
	check_case("actual-missing-fixture", not original.is_empty()
		and DirAccess.remove_absolute(ProjectSettings.globalize_path(missing_path)) == OK, missing_path)
	button("Shell/Toolbar/Recheck")
	await settle()
	check_case("broken-shard-sibling-partial", rows().size() == 1 and rows()[0].ends_with(" · Partial")
		and hub.get_node("Summary/ResourceId").text == "IT-07", rows())
	status(3)
	check_case("error-excludes-healthy-sibling", rows().is_empty() and hub.get_node("Summary/ResourceId").text.is_empty(), rows())
	query("IT-11")
	check_case("broken-definition-error", rows().size() == 1 and rows()[0].ends_with(" · Error")
		and choose("IT-11") and hub.get_node("Summary/Status").text.begins_with("Error — IT-11")
		and hub.get_node(PREVIEW + "Failures").text.contains(missing_path), hub.get_node(PREVIEW + "Failures").text)
	status(2)
	check_case("incomplete-excludes-actual-error", rows().is_empty(), rows())
	restore(missing_path, original)
	button("Shell/Toolbar/Recheck")
	await settle()
	status(1)
	check_case("restored-definition-partial", rows().size() == 1 and rows()[0].ends_with(" · Partial")
		and hub.get_node("Summary/ResourceId").text == "IT-11"
		and hub.get_node("Summary/Status").text.begins_with("Partial"), rows())
	var result = {"pid":OS.get_process_id(),"outcomes":outcomes,"failed":failed}
	restore("res://status-results.json", JSON.stringify(result, "\t").to_utf8_buffer())
	root.remove_child(hub)
	hub.free()
	await process_frame
	quit(1 if failed else 0)
`;
const LIFECYCLE=String.raw`
	check_case("preview-transport-exists", hub.has_node("Shell/Workspace/Preview/Transport/Play"), "The current preview owns explicit play/pause, speed, step and reset Controls")
	if hub.has_node(PREVIEW + "Transport/Play"):
		var transport = hub.get_node(PREVIEW + "Transport")
		check_case("static-resource-unavailable", transport.get_node("Play").disabled and transport.get_node("Step").disabled and hub.get_node(PREVIEW + "Sample").text.contains("No authored animation"), hub.get_node(PREVIEW + "Sample").text)
		var fixture = Node2D.new()
		fixture.name = "Fixture"
		var marker = Node2D.new()
		marker.name = "Marker"
		fixture.add_child(marker)
		var player = AnimationPlayer.new()
		player.name = "Authored"
		fixture.add_child(player)
		var clip = Animation.new()
		clip.length = 1.0
		var track = clip.add_track(Animation.TYPE_VALUE)
		clip.track_set_path(track, NodePath("Marker:position"))
		clip.track_set_interpolation_type(track, Animation.INTERPOLATION_NEAREST)
		for sample in [[0.0, 0.0], [0.1, 10.0], [0.4, 40.0], [1.0, 100.0]]:
			clip.track_insert_key(track, sample[0], Vector2(sample[1],0))
		var library = AnimationLibrary.new()
		library.add_animation("nonuniform",clip)
		player.add_animation_library("",library)
		var generation: int = hub.get("_generation")
		hub.call("AcceptPreview",generation,fixture)
		var current = hub.get_node(PREVIEW + "Surface/Current")
		check_case("authored-player-installed", current.get_child_count() == 1 and not transport.get_node("Play").disabled and marker.position == Vector2.ZERO, current.get_children().map(func(n):return n.name))
		button(PREVIEW + "Transport/Step")
		check_case("next-authored-sample-first", marker.position == Vector2(10,0) and hub.get_node(PREVIEW + "Sample").text.contains("0.100s"), str(marker.position))
		button(PREVIEW + "Transport/Step")
		check_case("next-authored-sample-nonuniform", marker.position == Vector2(40,0) and hub.get_node(PREVIEW + "Sample").text.contains("0.400s"), str(marker.position))
		button(PREVIEW + "Transport/Reset")
		var descendants: int = current.find_children("*","",true,false).size()
		for i in range(12):
			button(PREVIEW + "Transport/Reset")
		check_case("reset-fixed-owned-layers", current.get_child_count() == 1 and descendants == current.find_children("*","",true,false).size() and marker.position == Vector2.ZERO and player.get_signal_connection_list("animation_finished").is_empty(), {"children":current.get_child_count(),"descendants":descendants})
		var speeds = transport.get_node("Speed")
		check_case("exact-playback-speeds", speeds.item_count == 4 and speeds.get_item_text(0) == "0.25×" and speeds.get_item_text(1) == "0.5×" and speeds.get_item_text(2) == "1×" and speeds.get_item_text(3) == "2×" and speeds.selected == 2, speeds.item_count)
		for i in range(4):
			button(PREVIEW + "Transport/Reset")
			speeds.select(i)
			speeds.item_selected.emit(i)
			button(PREVIEW + "Transport/Play")
			hub.call("_Process",0.1)
			button(PREVIEW + "Transport/Play")
			var authored_time: float = [0.025,0.05,0.1,0.2][i]
			check_case("clock-speed-"+str(i), abs(player.current_animation_position-authored_time)<0.000001 and not paused, player.current_animation_position)
		var held: float = player.current_animation_position
		for i in range(3):
			await process_frame
		check_case("pause-only-preview-clock", player.current_animation_position == held and not paused, player.current_animation_position)
		var search = hub.get_node(BROWSER + "Search")
		search.grab_focus()
		var space = InputEventKey.new()
		space.keycode = KEY_SPACE
		space.unicode = 32
		space.pressed = true
		root.push_input(space)
		space = space.duplicate()
		space.pressed = false
		root.push_input(space)
		await process_frame
		check_case("text-owned-space", transport.get_node("Play").text == "Play preview" and search.text.ends_with(" ") and player.current_animation_position == held, search.text)
		query("")
		button(PREVIEW + "Actions/Details")
		var details = hub.get_node(PREVIEW + "Metadata")
		var selector = hub.get_node("Shell/Workspace/Navigation/Category")
		selector.show_popup()
		var escape = InputEventKey.new()
		escape.keycode = KEY_ESCAPE
		escape.pressed = true
		hub.call("_Input",escape)
		check_case("escape-popup-first", not selector.get_popup().visible and details.visible and selector.has_focus(), details.visible)
		hub.call("_Input",escape)
		check_case("escape-drawer-second", not details.visible and hub.get_node(PREVIEW + "Actions/Details").has_focus(), details.visible)
		hub.call("_Input",escape)
		check_case("escape-return-category", search.has_focus() and is_instance_valid(hub), search.has_focus())
		transport.get_node("Step").grab_focus()
		button(PREVIEW + "Transport/Play")
		await process_frame
		check_case("disabled-focus-removed", transport.get_node("Step").disabled and not transport.get_node("Step").has_focus(), str(root.gui_get_focus_owner()))
		button(PREVIEW + "Transport/Play")
		query("bow")
		status(1)
		category(6)
		query("IT-07")
		choose("IT-07")
		category(1)
		check_case("per-category-query-filter", search.text == "bow" and hub.get_node(BROWSER+"Status").selected == 1 and hub.get_node("Summary/ResourceId").text == "common.bow", {"query":search.text,"selection":hub.get_node("Summary/ResourceId").text})
		category(6)
		check_case("per-category-selection", search.text == "IT-07" and hub.get_node("Summary/ResourceId").text == "IT-07", hub.get_node("Summary/ResourceId").text)
		var late = Node2D.new()
		late.name = "ObsoleteA"
		category(1)
		hub.call_deferred("AcceptPreview",generation,late)
		await process_frame
		await process_frame
		check_case("late-generation-rejected", current.get_child_count() == 0 and not is_instance_valid(late) and hub.get_node("Summary/ResourceId").text == "common.bow", current.get_child_count())
		var retired = Node2D.new()
		retired.name = "AuditionOwner"
		var audio = AudioStreamPlayer.new()
		audio.stream = AudioStreamWAV.new()
		retired.add_child(audio)
		hub.call("AcceptPreview",hub.get("_generation"),retired)
		button("Shell/Toolbar/Recheck")
		button("Shell/Toolbar/Cancel")
		var after_cancel = Node2D.new()
		hub.call_deferred("AcceptPreview",hub.get("_generation"),after_cancel)
		await process_frame
		await process_frame
		check_case("cancel-retires-and-fences", current.get_child_count() == 0 and not is_instance_valid(retired) and not is_instance_valid(audio) and not is_instance_valid(after_cancel), current.get_child_count())
		button("Shell/Toolbar/Recheck")
		await settle()
		check_case("local-scroll-and-single-preview", hub.get_node("Shell/Workspace/Controls") is ScrollContainer and hub.get_node("Shell/Workspace/Controls").custom_minimum_size.x == 320 and hub.find_children("Current","",true,false).size() == 1, hub.get_node("Shell/Workspace/Controls").size)
		root.content_scale_size = Vector2i.ZERO
		root.size = Vector2i(1280,720)
		for i in range(4):
			await process_frame
		var workspace = hub.get_node("Shell/Workspace")
		var controls = hub.get_node("Shell/Workspace/Controls")
		check_case("desktop-layout-bounds", not workspace.vertical and controls.size.x == 320 and controls.get_global_rect().end.y <= 696.1, {"viewport":root.get_visible_rect().size,"controls":controls.get_global_rect(),"workspace":workspace.get_global_rect()})
		root.size = Vector2i(1039,720)
		for i in range(4):
			await process_frame
		check_case("responsive-category-below1040", workspace.vertical and not hub.get_node("Shell/Workspace/Navigation/Scope").visible and controls.get_index() > hub.get_node(PREVIEW.trim_suffix("/")).get_index(), {"vertical":workspace.vertical,"viewport":root.get_visible_rect().size})
		root.size = Vector2i(1040,720)
		for i in range(4):
			await process_frame
		check_case("responsive-boundary1040", not workspace.vertical, root.get_visible_rect().size)
		root.size = Vector2i(1280,720)
		for i in range(4):
			await process_frame
`;
async function native(context,lifecycle=false){
  const themeInputs=Object.fromEntries(['src/godot-game/assets/ui/InspectionTheme.tres',
    'src/godot-game/assets/ui/LiberationSans.ttf','src/godot-game/assets/ui/PerfectDOSVGA437.ttf',
    'src/godot-game/assets/ui/Fallback.ttf','src/godot-game/assets/common/donor-kit.png']
    .map(file=>[file,runner.hash(fs.readFileSync(path.join(ROOT,file)))]));
  fs.writeFileSync(path.join(context.directory,'theme-inputs.json'),JSON.stringify(themeInputs,null,2));
  const game=path.join(ROOT,'src/godot-game'),project=path.join(context.directory,'native-project');
  fs.mkdirSync(project,{recursive:true});
  const build=await runner.command(context,'native-build','dotnet',['build',path.join(game,'KingdomZombies.csproj'),
    '--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:env()});
  assert.match(build.stdout,/0 Warning\(s\)[\s\S]*0 Error\(s\)/);
  for(const item of ['project.godot','foundation','inspection','content',...(lifecycle?['assets/common','assets/ui']:['assets'])])
    fs.cpSync(path.join(game,item),path.join(project,item),{recursive:true});
  const legacy=require('./provenance.cjs').legacyFixture(),inspection=fs.readFileSync(path.join(project,'inspection/Inspection.tscn'));
  for(const output of legacy.index.outputs)for(const dependency of output.dependencies){if(dependency.path==='src/godot-game/inspection/Inspection.tscn'){dependency.bytes=inspection.length;dependency.sha256=runner.hash(inspection);}}
  fs.writeFileSync(path.join(project,'content/pack-index.json'),JSON.stringify(legacy.index,null,2)+'\n');
  fs.writeFileSync(path.join(project,'content/packs/tracer.json'),legacy.files.get('src/godot-game/content/packs/tracer.json'));
  fs.writeFileSync(path.join(context.directory,'legacy-fixture.json'),JSON.stringify({scope:'Independent retained partial bow fixture, current actual production assemblies/UI; original predicates unchanged',indexSha256:runner.hash(fs.readFileSync(path.join(project,'content/pack-index.json'))),inspection:{bytes:inspection.length,sha256:runner.hash(inspection)},historicalDependencySha256:'66702efb9bf9390f6454316ce0ec5bc7808cb5f439addf0430d46abccb589739'},null,2));
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
  const imported=await runner.command(context,'native-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env:isolated});
  assert.equal(imported.stderr.trim(),'','HUB_IMPORT_STDERR');assert.doesNotMatch(imported.stdout+imported.stderr,/ERROR:|SCRIPT ERROR:|WARNING:|FATAL|leaked/i);
  const run=await runner.command(context,'native-hub',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env:isolated});
  assert.equal(run.stderr.trim(),'','HUB_NATIVE_STDERR');assert.doesNotMatch(run.stdout+run.stderr,/ERROR:|SCRIPT ERROR:|WARNING:|FATAL|leaked|Failed to load/i);
  const result=JSON.parse(fs.readFileSync(path.join(project,'native-results.json'),'utf8'));
  assert.equal(result.pid,run.pid,'ACTUAL_NATIVE_PID');assert.equal(result.script,'res://inspection/InspectionRoot.cs');
  assert.equal(result.failed,false);assert.ok(result.outcomes.length>=24&&result.outcomes.every(x=>x.pass));
  const image=result.outcomes.find(x=>x.id==='native-bow-registration').observed;
  assert.equal(image.sourceRgbaSha256,'88b8e7062f0eb9cfb788ef3658028ba58a6f62d408d3e39ae2e1228bfa6c8905');
  if(!lifecycle){
    fs.copyFileSync(path.join(game,'content/pack-index.json'),path.join(project,'content/pack-index.json'));
    fs.writeFileSync(path.join(project,'status-probe.gd'),DRIVER.slice(0,DRIVER.indexOf('func run()'))+STATUS_CONTROLS);
    const current=await runner.command(context,'native-current-status',executable,
      ['--headless','--path',project,'--script','res://status-probe.gd','--quit-after','800'],{env:isolated});
    assert.equal(current.stderr.trim(),'','HUB_CURRENT_STATUS_STDERR');
    assert.doesNotMatch(current.stdout,/ERROR:|SCRIPT ERROR:|WARNING:|FATAL|leaked|Failed to load/i);
    result.current=JSON.parse(fs.readFileSync(path.join(project,'status-results.json'),'utf8'));
    assert.equal(result.current.pid,current.pid,'ACTUAL_CURRENT_NATIVE_PID');
    assert.equal(result.current.failed,false);
    assert.deepEqual(result.current.outcomes.map(row=>row.id),[
      'current-startup-selection','healthy-error-empty','healthy-sibling-partial','empty-search-selection',
      'new-category-valid-selection','restored-category-selection','actual-missing-fixture','broken-shard-sibling-partial',
      'error-excludes-healthy-sibling','broken-definition-error','incomplete-excludes-actual-error','restored-definition-partial'
    ]);
    assert.ok(result.current.outcomes.every(row=>row.pass));
  }
  fs.writeFileSync(path.join(context.directory,'native-observation.json'),JSON.stringify(result,null,2));
  for(const [file,digest]of Object.entries(themeInputs)){
    assert.equal(runner.hash(fs.readFileSync(path.join(ROOT,file))),digest,'HUB_THEME_SOURCE_CHANGED');
    assert.equal(runner.hash(fs.readFileSync(path.join(project,file.replace('src/godot-game/','')))),digest,
      'HUB_THEME_SUBJECT_CHANGED');
  }
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
    const result=compiled(context.directory);
    const inputs=Object.fromEntries(['src/core/ContentPack.cs','src/godot-game/inspection/InspectionRoot.cs',
      'src/godot-game/inspection/Inspection.tscn','src/godot-game/content/pack-index.json',
      'src/godot-game/content/expected-obligations.json','src/godot-game/content/packs/tracer.json',
      'src/godot-game/assets/common/bow.png','tests/phase1/ContentInspectorTests.cs',
      'tools/phase4/cases/hub.cjs'].map(file=>[file,runner.hash(fs.readFileSync(path.join(ROOT,file)))]));
    fs.writeFileSync(path.join(context.directory,'inputs.json'),JSON.stringify(inputs,null,2));
    const observed=await native(context);
    return {compiled:true,discovered:result.discovered,native:observed.outcomes.length,pid:observed.pid,
      currentNative:observed.current.outcomes.length,currentPid:observed.current.pid,
      scope:'actual compiled pure loader and C# native hub behavior; exact source policy Task2; no rendered acceptance'};
  });
runner.registerCase('hub.lifecycle',{group:'hub',title:'Inspection input, transport and owned generation lifecycle',importance:'Critical',
  rationale:'Stale callbacks or duplicated input must not replace the selected preview or accumulate native owners',needsGameWindow:false,
  runGroup:'G0-hub',order:2,requirements:['ASSET-02','ASSET-03','ASSET-04','ASSET-05'],acceptance:['AC-04','AC-16','AC-20'],
  setup:'Actual compiled production CSharp scene and native Controls in an isolated headless project',
  actions:['Inspect explicit preview transport','Drive owned keys, category restoration and repeated reset','Observe late generation rejection and teardown'],
  expected:'One current preview; text and popups own input; paused stepping uses actual authored samples; obsolete results are rejected',
  backstops:[
    {id:'HUB-NAV/loading',observation:'loading',renderedCheckpoint:'hub-loading'},
    {id:'HUB-NAV/error',observation:'missing-register-valid-bow',renderedCheckpoint:'hub-independent-error'},
    {id:'HUB-NAV/overflow',observation:'responsive-category-below1040',renderedCheckpoint:'hub-scaled-navigation'},
    {id:'HUB-NAV/long-text',observation:'exact-obligation-diagnostic',renderedCheckpoint:'hub-long-navigation'},
    {id:'HUB-REGISTER/empty',observation:'missing-media',renderedCheckpoint:'register-no-outputs'},
    {id:'HUB-REGISTER/loading',observation:'loading',renderedCheckpoint:'register-loading'},
    {id:'HUB-REGISTER/error',observation:'missing-register-census',renderedCheckpoint:'register-invalid'},
    {id:'HUB-REGISTER/populated',observation:'register',renderedCheckpoint:'register180'},
    {id:'HUB-REGISTER/partial',observation:'partial-filter',renderedCheckpoint:'register-partial'},
    {id:'HUB-REGISTER/overflow',observation:'local-scroll-and-single-preview',renderedCheckpoint:'register-local-scroll'},
    {id:'HUB-REGISTER/zero-one-many',observation:'one-result',renderedCheckpoint:'register-zero-one-many'},
    {id:'HUB-REGISTER/long-text',observation:'exact-obligation-diagnostic',renderedCheckpoint:'register-long-identity'}],
  reset:'Close acceptance, cancel work, disconnect handlers and retire every owned preview; no game window',
  evidence:['native-observation.json','native-build.command.json','native-assemblies.json','native-hub.command.json','result.json']},async context=>{
    const coverage=compiled(context.directory);
    const result=await native(context,true);
    const definition=runner.cases.get('hub.lifecycle');
    assert.equal(definition.backstops.length,12);
    for(const row of definition.backstops)assert.equal(result.outcomes.find(x=>x.id===row.observation)?.pass,true,row.id);
    return {compiled:coverage.discovered,native:result.outcomes.length,pid:result.pid,scope:result.scope,
      backstops:definition.backstops.map(x=>({...x,headless:true,rendered:'pending04-32'}))};
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
    const directory=path.join(ROOT,'.phase4/compiled-proof',require('node:crypto').randomUUID());
    fs.mkdirSync(directory,{recursive:true});compiled(directory);
  });
}
