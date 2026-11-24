'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const {spawn} = require('node:child_process');
const {randomUUID} = require('node:crypto');
const delivery = require('../../tools/asset-pipeline/export.cjs');
const content = require('../../tools/asset-pipeline/content.cjs');
const network = require('../../tools/lan-checks/network.cjs');
const terminal = require('../../tools/lan-checks/terminal.cjs');
const {lines} = require('../../tools/lan-checks/rendered.cjs');

const id = 'phase06.packaged';
const packName = 'KingdomZombies.pck';
const runPath = 'content/config/run.json';
const combatPath = 'content/config/combat.json';
const fixtureName = 'fixture.request';
const expectedConfig = {
  [runPath]: 'DT-01', 'content/config/items.json': 'DT-04',
  'content/config/survivors.json': 'DT-03', 'content/config/combat.json': 'DT-05',
  'content/config/food-wildlife.json': 'DT-08', 'content/config/inventory.json': 'DT-06',
};
const assetPaths = ['content/animation/locomotion-melee.json', 'content/animation/bow.json',
  'content/animation/fits-a.json', 'content/packs/bodies-a.json', 'content/packs/corpses-wildlife.json',
  'content/packs/items-01-10.json', 'content/packs/items-11-24.json', 'content/packs/common.json',
  'assets/characters/bodies-a.png.import', 'assets/common/donor-kit.png.import',
  'assets/equipment/items-01-10.png.import', 'assets/equipment/items-11-24.png.import',
  'assets/ui/InspectionTheme.tres.remap', 'foundation/Foundation.tscn.remap'];

const gameplayProbe = `extends Node

func _ready() -> void:
    run_probe.call_deferred()

func press(key: Key, down: bool) -> void:
    var event := InputEventKey.new()
    event.keycode = key
    event.physical_keycode = key
    event.pressed = down
    Input.parse_input_event(event)

func fail(reason: String) -> void:
    print("PHASE06_PROBE_FAILED " + reason)
    get_tree().quit(3)

func crop(texture: AtlasTexture) -> Dictionary:
    if not texture or not texture.atlas or not texture.region.has_area():
        return {}
    var box := texture.region
    return {"atlas": texture.atlas.resource_path, "rect": [box.position.x, box.position.y, box.size.x, box.size.y]}

func observe_gameplay_assets(scene: Node) -> bool:
    var appearance := scene.get_node_or_null("World/RunAppearance") as Node2D
    var workspace := scene.get_node_or_null("InventoryWorkspace") as Control
    if not appearance or appearance.get_child_count() != 8 or not workspace or not workspace.theme:
        return false
    var icons: Dictionary = {}
    for slot in ["torso-clothing", "legs", "boots"]:
        var node := workspace.get_node_or_null("InventoryPanels/equipment_" + slot + "/Icon") as TextureRect
        var icon := node.texture as AtlasTexture if node else null
        icons[slot] = crop(icon)
        if icons[slot].is_empty():
            return false
    var crowns: Array[Dictionary] = []
    var body: Dictionary = {}
    for survivor in appearance.get_children():
        var node := survivor.get_node_or_null("Crown") as Sprite2D
        if node and node.visible:
            var crown := node.texture as AtlasTexture
            var crown_crop := crop(crown)
            if crown_crop.is_empty():
                return false
            crowns.append(crown_crop)
            for part in ["lower", "upper", "head"]:
                var sprite := survivor.get_node_or_null("Art/Body_" + part) as Sprite2D
                if not sprite or not sprite.visible:
                    return false
                body[part] = {"key": sprite.get_meta("part", ""), "crop": crop(sprite.texture as AtlasTexture)}
    print("PHASE06_GAMEPLAY_RESOURCES " + JSON.stringify({"actors": appearance.get_child_count(),
        "theme": workspace.theme.resource_path, "icons": icons, "crowns": crowns, "body": body}))
    return true

func run_probe() -> void:
    var expected := "res://foundation/Foundation.tscn"
    if DisplayServer.get_name() != "headless" or ProjectSettings.get_setting("application/run/main_scene") != expected:
        fail("release scene or provider")
        return
    var tree := get_tree()
    var scene := tree.current_scene
    for frame in range(60):
        if scene:
            break
        await tree.process_frame
        scene = tree.current_scene
    if not scene or scene.scene_file_path != expected:
        fail("production main scene")
        return
    await tree.process_frame
    var menu := scene.get_node("Menu")
    menu.get_node("Start").pressed.emit()
    if OS.get_environment("KZ_PHASE06_PACKAGE_VARIANT") in ["normal", "upper"]:
        await tree.create_timer(0.25).timeout
        press(KEY_D, true)
        await tree.create_timer(0.40).timeout
        press(KEY_D, false)
        press(KEY_I, true)
        await tree.process_frame
        press(KEY_I, false)
        await tree.create_timer(0.15).timeout
        if not observe_gameplay_assets(scene):
            fail("live native gameplay art")
            return
        press(KEY_I, true)
        await tree.process_frame
        press(KEY_I, false)
        press(KEY_ESCAPE, true)
        await tree.process_frame
        press(KEY_ESCAPE, false)
    elif not menu.get_node("Error").visible:
        fail("required resource rejection was not visible")
        return
    var output := OS.get_environment("PHASE3_OUTPUT")
    var nonce := OS.get_environment("PHASE3_NONCE")
    var next := Time.get_ticks_msec() + 10_000
    while Time.get_ticks_msec() < next:
        var file := FileAccess.open(output.path_join("quit.request"), FileAccess.READ)
        if file and file.get_as_text() == nonce:
            if not menu.get_node("Quit").is_visible_in_tree():
                fail("ordinary Quit unavailable")
                return
            menu.get_node("Quit").pressed.emit()
            return
        await tree.process_frame
    fail("owned quit request deadline")
`;

const source = (state, file) => path.join(state.root, 'src/godot-game/game', file);
const write = (state, file, data) => {
  const destination = path.join(state.directory, file);
  fs.writeFileSync(destination, JSON.stringify(data, null, 2), {flag: 'wx'});
  return destination;
};

function json(files, name) {
  const member = files.get(name);
  assert(member, name + ' missing from the release package');
  return JSON.parse(member.data);
}

function verifyResources(state, files) {
  const receipts = [];
  for (const [name, identity] of Object.entries(expectedConfig)) {
    const packaged = json(files, name);
    assert.equal(packaged.id, identity, name);
    assert.equal(packaged.version, 1, name);
  }
  for (const name of [...Object.keys(expectedConfig), ...assetPaths]) {
    const member = files.get(name);
    assert(member, name + ' missing from the release package');
    if (name.endsWith('.json')) assert.deepEqual(member.data, fs.readFileSync(source(state, name)), name);
    receipts.push({path: name, sha256: member.sha256, bytes: member.bytes});
  }
  const configs = json(files, 'content/config/items.json').definitions;
  assert.deepEqual(configs.map(row => row.id), Array.from({length: 24}, (_, index) =>
    'IT-' + String(index + 1).padStart(2, '0')));
  const common = json(files, 'content/packs/common.json');
  assert(common.resources.some(row => row.id === 'common.s0.7985'), 'authored crown resource');
  const weapons = json(files, 'content/packs/items-01-10.json');
  assert(weapons.parts.some(row => row.key === 'IT_02_master'), 'authored arrow resource');
  const gear = json(files, 'content/packs/items-11-24.json');
  for (const definition of json(files, runPath).startingOutfit) {
    const item = configs.find(row => row.id === definition);
    assert(gear.parts.some(part => part.key === item.appearance.canonicalIcon), definition + ' canonical icon');
  }
  return receipts;
}

function expectedCrops(state) {
  const read = name => JSON.parse(fs.readFileSync(source(state, 'content/' + name + '.json')));
  const body = read('packs/bodies-a'), gear = read('packs/items-11-24'), common = read('packs/common');
  const starting = read('config/run').startingOutfit;
  const icons = {};
  for (const {id, slot, appearance} of read('config/items').definitions) {
    if (starting.includes(id)) {
      icons[slot] = {atlas: 'res://' + gear.atlas.path,
        rect: gear.parts.find(part => part.key === appearance.canonicalIcon).atlasRect};
    }
  }
  const idle = body.bodies.find(row => row.id === 'CH-01').poses.find(pose => pose.id === 'idle');
  const segments = Object.fromEntries(['lower', 'upper', 'head'].map(name => {
    const key = idle.parts[name];
    return [name, {key, crop: {atlas: body.atlas.path, rect: body.parts.find(part => part.key === key).atlasRect}}];
  }));
  const crown = common.resources.find(row => row.id === 'common.s0.7985');
  return {actors: 8, theme: 'res://assets/ui/InspectionTheme.tres', icons,
    crowns: [{atlas: 'res://' + common.atlas.path, rect: crown.atlasRect}], body: segments};
}

function verifyCrops(state, assets) {
  const expected = expectedCrops(state);
  const exact = observed => assert.deepEqual(observed, expected, 'exact authored live gameplay atlas regions');
  exact(assets);
  const unrelated = structuredClone(assets);
  unrelated.body.lower.crop.rect = assets.body.head.crop.rect;
  assert.throws(() => exact(unrelated), {code: 'ERR_ASSERTION'});
}

function portableEnvironment(state, role) {
  const out = path.join(state.directory, role);
  fs.mkdirSync(out);
  const startsGame = role === 'normal' || role === 'upper';
  if (startsGame) fs.writeFileSync(path.join(out, fixtureName), 'field', {flag: 'wx'});
  const attest = randomUUID();
  const marker = path.join(out, 'phase4-runtime');
  fs.mkdirSync(marker);
  fs.writeFileSync(path.join(marker, 'owner'), attest);
  const nativeRole = startsGame ? 'solo' : role;
  const env = delivery.runtimeEnvironment({...delivery.cleanEnvironment(out),
    COREHOST_TRACE: '1', COREHOST_TRACEFILE: path.join(state.directory, role + '-host.log'),
    PHASE3_NONCE: state.nonce, PHASE3_OUTPUT: out, PHASE3_RUN_ROOT: state.directory, PHASE3_ROLE: nativeRole,
    PHASE3_INPUT_DRIVER: role === 'release-boot' ? 'no-input' : 'owned-headless-package-probe',
    KZ_PHASE4_ATTEST_ID: attest, KZ_PHASE06_OWNED: state.nonce, KZ_PHASE06_PACKAGE_VARIANT: role});
  return {env, out, nativeRole, attest};
}

function portableProcess(state, role, directory) {
  const {env, out, nativeRole, attest} = portableEnvironment(state, role);
  const executable = path.join(directory, 'KingdomZombies.exe');
  const child = spawn(executable, ['--headless'], {cwd: directory, env, windowsHide: true,
    stdio: ['ignore', 'pipe', 'pipe']});
  const peer = {role: nativeRole, directory: out, pid: child.pid, child, exit: null, output: '', attest};
  for (const stream of [child.stdout, child.stderr]) stream.on('data', bytes => { peer.output += bytes; });
  child.once('error', error => { peer.error = error; });
  terminal.track(peer, state);
  return peer;
}

const wait = (peer, check, label) => network.waitFor([peer], Date.now() + 8000,
  () => lines(path.join(peer.directory, 'frames.ndjson')).find(check), peer.role + ' ' + label);

function validateLoaded(peer, directory) {
  const loaded = lines(path.join(peer.directory, 'phase4-runtime', 'loaded-assemblies.ndjson'));
  assert.equal(loaded.length, 3);
  const runtime = delivery.inventory(directory);
  for (const row of loaded) {
    assert.equal(row.pid, peer.pid);
    assert.equal(row.launch, peer.attest);
    const member = runtime.find(file => path.basename(file.path) === row.name + '.dll');
    assert(member, row.name);
    assert.equal(path.resolve(row.file).toLowerCase(), path.resolve(directory, member.path).toLowerCase());
    assert.equal(row.mvid, require('../../tools/asset-pipeline/rendered-process.cjs')
      .managedMvid(fs.readFileSync(row.file)));
  }
  return loaded;
}

async function bootUnmodified(state, directory) {
  const role = 'release-boot';
  const {env, out, attest} = portableEnvironment(state, role);
  const command = await require('../../tools/asset-pipeline/check.cjs').command(state, role,
    path.join(directory, 'KingdomZombies.exe'), ['--headless', '--quit-after', '60'], {env, cwd: directory});
  assert(!/ERROR:|SCRIPT ERROR:|Unhandled exception|FATAL/.test(command.stdout + command.stderr));
  const frames = lines(path.join(out, 'frames.ndjson'));
  assert(frames.length >= 8);
  assert(frames.every(row => row.pid === command.pid && row.nonce === state.nonce && row.profile?.PlayerId
    && !row.ready && row.snapshot === null && row.phase06 === null), 'unmodified package booted into Foundation menu');
  const loaded = validateLoaded({pid: command.pid, attest, directory: out, nonce: state.nonce}, directory);
  return {pid: command.pid, menu: frames.at(-1), loaded, resolution: delivery.hostTrace(state, role, directory)};
}

async function launchScenario(state, role, directory, diagnostic) {
  const peer = portableProcess(state, role, directory);
  const fixture = path.join(peer.directory, fixtureName);
  let failure, report;
  try {
    const menu = await wait(peer, row => row.profile?.PlayerId && !row.ready, 'actual packaged menu');
    assert.equal(menu.pid, peer.pid);
    assert.equal(menu.nonce, state.nonce);
    let started, inventory, moved, assets;
    if (diagnostic) {
      started = await wait(peer, row => row.frame > menu.frame && peer.output.split(/\r?\n/).includes(diagnostic),
        'explicit packaged resource diagnostic');
      assert.equal(started.ready, false);
      assert.equal(started.snapshot, null);
      assert.equal(started.runPose, null);
    } else {
      started = await wait(peer, row => row.ready && row.runPose && row.hud?.slots === 4,
        'current gameplay authority and composed appearance');
      assert.equal(started.phase06, null, 'release cannot enable debug fixture state');
      assert.equal(fs.readFileSync(fixture, 'utf8'), 'field', 'release never consumed the debug request');
      const position = network.actor(started, menu.profile.PlayerId).PositionSubunits;
      moved = await wait(peer, row => row.ready && network.actor(row, menu.profile.PlayerId)?.PositionSubunits
        > position + 300, 'actual packaged movement');
      inventory = await wait(peer, row => row.inventoryOpen && row.inventorySlots === 11,
        'authored theme and actual starting outfit icons');
      await wait(peer, row => row.frame > inventory.frame && !row.ready, 'ordinary menu return');
      const marker = peer.output.split(/\r?\n/).find(line => line.startsWith('PHASE06_GAMEPLAY_RESOURCES '));
      assert(marker, 'missing native asset observation');
      assets = JSON.parse(marker.slice('PHASE06_GAMEPLAY_RESOURCES '.length));
      verifyCrops(state, assets);
    }
    const loaded = validateLoaded(peer, directory);
    report = {role, pid: peer.pid, menu, started, inventory, moved, loaded, assets, diagnostic};
    terminal.requestQuit(peer);
    fs.writeFileSync(path.join(peer.directory, 'quit.request'), state.nonce, {flag: 'wx'});
  } catch (error) { failure = error; }
  finally {
    try { await network.cleanup(peer); } catch (error) { failure ??= error; }
  }
  if (failure) throw failure;
  assert(!/ERROR:|SCRIPT ERROR:|Unhandled exception|FATAL/.test(peer.output), peer.output.slice(-1500));
  report.resolution = delivery.hostTrace(state, role, directory);
  return report;
}

function altered(state, built, original, role, replacements) {
  const destination = delivery.cloneOutput(state, role + '-package', built.output, built.closure.rows);
  const members = new Map([...delivery.pack(original)].map(([name, row]) => [name, row.data]));
  for (const [name, data] of replacements) {
    if (data) members.set(name, data);
    else members.delete(name);
  }
  const changed = delivery.packBytes(original, members);
  const verified = delivery.pack(changed);
  const originalFiles = delivery.pack(original);
  const differences = [...originalFiles].filter(([name, row]) => verified.get(name)?.sha256 !== row.sha256)
    .map(([name]) => name);
  assert.deepEqual(differences.sort(), [...replacements.keys()].sort());
  fs.writeFileSync(path.join(destination, packName), changed);
  return {destination, differences, sha256: content.hash(changed)};
}

function combatVariant(files, seconds) {
  const combat = json(files, combatPath);
  combat.attacks.find(attack => attack.itemRef === 'IT-07').intervalSeconds = seconds;
  const data = Buffer.from(JSON.stringify(combat));
  const replacements = new Map([[combatPath, data]]);
  for (const name of ['content/animation/locomotion-melee.json', 'content/animation/bow.json']) {
    const manifest = json(files, name);
    const binding = manifest.bindings.find(row => row.path === 'src/godot-game/' + combatPath);
    assert.equal(binding.sha256, content.hash(files.get(combatPath).data), name + ' original source binding');
    binding.sha256 = content.hash(data);
    replacements.set(name, Buffer.from(JSON.stringify(manifest)));
  }
  return replacements;
}

async function open(context) {
  assert.equal(process.platform + '-' + process.arch, 'win32-x64');
  const nonce = path.basename(context.directory);
  assert.match(nonce, /^[a-f0-9-]{36}$/);
  fs.writeFileSync(path.join(context.directory, 'observation.owner'), nonce, {flag: 'wx'});
  return {...context, nonce, caseId: id};
}

async function packaged(state) {
  const built = await delivery.buildPackage(state, {gameplay: true});
  const original = fs.readFileSync(path.join(built.clean, packName));
  const files = delivery.pack(original);
  const resources = verifyResources(state, files);
  const boot = await bootUnmodified(state, built.clean);
  const diagnostic = 'Recoverable scene entry: RUN_DEFINITION_INVALID ' + runPath + ': ';
  const probe = path.join(state.directory, 'portable-probe.gd');
  fs.writeFileSync(probe, gameplayProbe, {flag: 'wx'});
  const proof = await delivery.buildGameplayProbe(state, built, gameplayProbe);
  const probePack = fs.readFileSync(path.join(proof.output, packName));
  const missing = altered(state, proof, probePack, 'missing', new Map([[runPath, null]]));
  const malformed = altered(state, proof, probePack, 'invalid',
    new Map([[runPath, Buffer.from('{"id":"DT-01","id":"DT-01","version":1}')]]));
  const upper = altered(state, proof, probePack, 'upper', combatVariant(files, 600));
  const outside = altered(state, proof, probePack, 'above-upper', combatVariant(files, 600.001));
  const normal = await launchScenario(state, 'normal', proof.output);
  const absent = await launchScenario(state, 'missing', missing.destination, diagnostic + 'bytes');
  const invalid = await launchScenario(state, 'invalid', malformed.destination, diagnostic + 'id');
  const maximum = await launchScenario(state, 'upper', upper.destination);
  const rejected = await launchScenario(state, 'above-upper', outside.destination,
    'Recoverable scene entry: RUN_DEFINITION_INVALID ' + combatPath + ': intervalSeconds');
  assert.equal(new Set([boot.pid, normal.pid, absent.pid, invalid.pid, maximum.pid, rejected.pid]).size, 6);
  assert.deepEqual(fs.readFileSync(path.join(built.clean, packName)), original);
  delivery.verifyInputs(built);
  const report = write(state, id + '.json', {id, revision: state.revision, nonce: state.nonce,
    packagedSha256: content.hash(original), releaseTemplate: await delivery.digest(built.binary),
    resources, boot, missing, malformed, upper, outside, cases: [normal, absent, invalid, maximum, rejected]});
  return [report, probe, 'pck-runtime-closure.json', 'project-inputs.json', 'templates.json',
    'managed-publish.command.json', 'native-export.command.json', 'proof-build/probe-binding.json',
    'proof-build/native-proof-export.command.json', 'release-boot.command.json', 'release-boot-host.log',
    'release-boot/frames.ndjson', 'release-boot/phase4-runtime/loaded-assemblies.ndjson',
    ...['normal', 'missing', 'invalid', 'upper', 'above-upper'].flatMap(role => [role + '/frames.ndjson', role + '/console.log',
      role + '/terminal.json', role + '-host.log', role + '/phase4-runtime/loaded-assemblies.ndjson'])];
}

module.exports = {open, reset() {}, close() {}, cases: {[id]: packaged}};

