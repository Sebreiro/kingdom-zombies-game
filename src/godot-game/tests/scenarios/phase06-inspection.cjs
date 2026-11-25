'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const handoff = require('../../tools/asset-pipeline/handoff.json')['6'];
const scenarios = require('../../tools/asset-pipeline/scenarios.cjs');
const evidence = require('../../tools/asset-pipeline/evidence.cjs');
// evidence.cjs has no hash export; it uses this content.cjs function internally.
const {hash} = require('../../tools/asset-pipeline/content.cjs');
const contracts = require('../../tools/asset-pipeline/rendered-contracts.cjs');
const processes = require('../../tools/asset-pipeline/rendered-process.cjs');
const desktop = require('../../tools/gameplay-checks/rendered.cjs');

const recipes = new Map();
const recordName = 'inspection-cleanup.json';

async function open(context) {
  assert.equal(process.platform + '-' + process.arch, 'win32-x64');
  assert.equal(evidence.git(context.root, ['rev-parse', 'HEAD']).trim(), context.revision);
  assert.equal(evidence.git(context.root, ['status', '--porcelain']).trim(), '');
  const options = {restoreProject: true};
  const preparation = await scenarios.prepareSource(context.root, context.revision, options);
  const build = evidence.document(context.root, preparation);
  build.sourceManaged = scenarios.validateSourceProject(context.root, build,
    evidence.identifySource(context.root, context.revision), options);
  const controller = new AbortController();
  const budget = processes.timeBudget(17 * 60_000, controller.signal);
  const relative = path.relative(context.root, context.directory).replaceAll(path.sep, '/');
  const session = await scenarios.inspectionSession(context.root, relative + '/inspection', 'G1-hub', build,
    {physical: desktop.ownedInput, decodePng: desktop.decodePng, dpiAwareInput: true}, budget, controller.signal);
  return {...context, relative, build, preparation, session, controller, budget, evidence: [], launches: []};
}

async function close(state) {
  const finished = await state.session.close();
  state.launches.push({role: finished.record.role, pid: finished.record.pid});
  evidence.validateProcess(finished.record);
  scenarios.validateProcessRuntime(state.root, state.build, finished.record);
  assert.equal(evidence.git(state.root, ['rev-parse', 'HEAD']).trim(), state.revision);
  assert.equal(evidence.git(state.root, ['status', '--porcelain']).trim(), '');
  evidence.writeExclusive(state.root, state.relative + '/' + recordName,
    {revision: state.revision, process: finished.ref, preparation: state.preparation, cases: state.evidence});
}

async function perform(state, legacyId) {
  assert(state.session.usable, 'original inspection session unavailable');
  const item = {id: 'asset-pipeline.' + legacyId};
  state.currentItem = item;
  const checkpoints = [];
  const {steps, stimulus, cleanup} = await recipes.get(legacyId)(state);
  try {
    for (const step of steps) {
      state.budget();
      checkpoints.push(await state.session.checkpoint(item, {...step, owner: state.session}, state.invocation));
    }
  } finally { await cleanup?.(); }
  assert.deepEqual(checkpoints.map(point => point.id), steps.map(step => step.id));
  assert(checkpoints.length);
  const reset = await restore(state, item);
  state.currentItem = null;
  const result = evidence.writeExclusive(state.root, state.relative + '/' + item.id + '.json',
    {id: item.id, revision: state.revision, stimulus, expected: steps.map(({id, checks}) => ({id, checks})),
      checkpoints, reset});
  state.evidence.push(result);
  return [result, ...checkpoints.flatMap(point => [point.input, point.draw, point.image])].map(file =>
    path.join(state.root, file.path)).concat(recordName, 'inspection/terminal.json');
}

const hub = '/root/Inspection';
const summary = hub + '/Summary';
const work = hub + '/Shell/Workspace';
const toolbar = hub + '/Shell/Toolbar';
const browser = work + '/Controls/Browser';
const resources = browser + '/Resources';
const search = browser + '/Search';
const category = work + '/Navigation/Category';
const preview = work + '/Preview';
const current = preview + '/Surface/Current';
const character = current + '/Character';
const characterScroll = character + '/Controls';
const slots = characterScroll + '/Slots';
const resource = current + '/ResourcePreview';
const corpse = resource + '/Art/Corpse';
const choices = resource + '/Controls/State';
const collection = resource + '/Art/LibraryScroll/Surface';
const contents = collection + '/Inventory/Scroll/Content';
const check = (node, property, value) => ({node, property, value, match: 'equal'});
const includes = (node, property, value) => ({node, property, value, match: 'contains'});
const exists = node => ({node, property: null, match: 'present'});
const absent = node => ({node, property: null, match: 'absent'});
const text = (node, value) => check(node, 'text', value);
const subject = id => text(summary + '/ResourceId', id);
const sample = (key, value, node = character, property = 'metadata/sample') =>
  ({node, property, key, value, match: 'field'});
const view = (key, value) => sample(key, value, resource, 'metadata/resource_view');
const appearance = (key, value) => sample(key, value, corpse, 'metadata/appearance');
const oneBody = {node: corpse, property: null, childClass: 'Node2D', value: 1, match: 'child-count'};
const positive = (node, property) => ({node, property, match: 'positive-count'});
const click = (id, control, checks, scroll) => ({id, control, checks, drive: driver => driver.click(control, {scroll})});
const signature = id => [subject(id), check(character, 'metadata/body', id), exists(character + '/Art/Body_upper')];
const simple = (stimulus, steps) => async () => ({stimulus, steps});
const option = (id, node, label, checks = [], scroll) => ({
  id, control: node, checks, async drive(driver) {
    await choose(driver, this.owner, node, label, scroll);
  },
});

async function choose(driver, owner, node, label, scroll) {
  const items = await owner.inspect(Array.from({length: 128}, (_, index) =>
    ({node, property: 'popup/item_' + index + '/text'})));
  const index = items.values.findIndex(entry => entry.value === label);
  assert(index >= 0, node + ' does not offer ' + label);
  await driver.click(node, {scroll});
  for (let row = 0; row <= index; row++) await driver.tap(40);
  await driver.tap(13);
}

async function typeSearch(driver, value) {
  await driver.click(search);
  await driver.hold(17);
  try { await driver.tap(65); }
  finally { await driver.release(17); }
  await driver.tap(8);
  for (const character of value) {
    const key = character === '-' ? 0xbd : character === ' ' ? 32 : character.toUpperCase().charCodeAt(0);
    assert((key >= 48 && key <= 90) || key === 0xbd || key === 32);
    await driver.tap(key);
  }
}

function resourceStep(id, kind, identity, checks = []) {
  return {id, control: resources, checks: [subject(identity), ...checks], async drive(driver) {
    await choose(driver, this.owner, category, kind);
    await typeSearch(driver, identity);
    await driver.click(resources, {firstRow: true});
  }};
}

const characterStep = (id = 'character', body = 'CH-06') => resourceStep(id, 'Characters', body, signature(body).slice(1));
const select = (id, name, label, checks = []) =>
  option(id, slots + '/' + name + '/Select', label, checks, characterScroll);
const transport = (id, name, checks) => click(id, slots + '/' + name, checks, characterScroll);
const paces = (pose, seconds) => [sample('movement', 'walk'), sample('lowerPose', pose),
  sample('strideSeconds', seconds), sample('lowerOffset', [0, 0]), exists(character + '/Art/Body_lower')];

// CharacterPresentation sampling: a sample covers [start, end) less a 1e-8 tolerance, Step advances by
// nextSampleSeconds, walk loops and jump holds just before its end. The same double arithmetic as the C# source.
const tolerance = 0.00000001;
const authoredClip = (definition, clip) => definition.clips[clip];

function authoredSample(definition, clip, seconds) {
  const samples = authoredClip(definition, clip);
  let time = seconds;
  for (const row of samples) {
    if (time < row.duration - tolerance) return row;
    time -= row.duration;
  }
  return samples.at(-1);
}

function remaining(definition, clip, seconds) {
  let time = seconds;
  for (const row of authoredClip(definition, clip)) {
    if (time < row.duration - tolerance) return row.duration - time;
    time -= row.duration;
  }
  return 0.0000001;
}

function steppedTimes(definition, clip, steps) {
  const length = authoredClip(definition, clip).reduce((sum, row) => sum + row.duration, 0);
  const times = [0];
  for (let index = 0; index < steps; index++) {
    const next = times.at(-1) + remaining(definition, clip, times.at(-1));
    times.push(clip === 'jump' ? Math.min(length - 0.0000001, next) : next % length);
  }
  return times;
}

function authoredChecks(definition, clip, seconds) {
  const row = authoredSample(definition, clip, seconds);
  return [sample('movement', clip), sample('lowerPose', row.pose), sample('stage', row.stage),
    sample('lowerOffset', row.offset), sample('strideSeconds', seconds), exists(character + '/Art/Body_lower')];
}

function h16Steps(definition) {
  const walk = steppedTimes(definition, 'walk', 2);
  const jump = steppedTimes(definition, 'jump', 3);
  return [
    characterStep('authored-walk', 'CH-01'),
    select('start-walk', 'Movement', 'Walk', authoredChecks(definition, 'walk', walk[0])),
    transport('exact-second-frame', 'Step', authoredChecks(definition, 'walk', walk[1])),
    transport('held-walk-endpoint', 'Step', authoredChecks(definition, 'walk', walk[2])),
    select('start-jump', 'Movement', 'Jump', authoredChecks(definition, 'jump', jump[0])),
    transport('step-nonuniform-jump', 'Step', authoredChecks(definition, 'jump', jump[1])),
    transport('step-held-jump', 'Step', authoredChecks(definition, 'jump', jump[2])),
    transport('step-jump-landing', 'Step', authoredChecks(definition, 'jump', jump[3])),
  ];
}

const locomotionFile = root => path.join(root, 'src/godot-game/game/content/animation/locomotion-melee.json');

async function restore(state, item, lifetime = 1) {
  const step = {id: 'reset-' + lifetime, control: resources, checks: contracts.baseline, async drive(driver) {
    await choose(driver, state.session, category, 'Items');
    await typeSearch(driver, 'IT-07');
    await driver.click(resources, {firstRow: true});
    await typeSearch(driver, '');
    const shown = (await state.session.inspect([{node: preview + '/Metadata', property: 'visible'}])).values[0].value;
    if (shown) await driver.click(preview + '/Actions/Details');
  }};
  return state.session.restore(item, [step], state.invocation, lifetime);
}

function fixture(state, relative, transform) {
  const root = fs.realpathSync.native(path.resolve(state.root, state.build.sourceProject.path));
  const file = fs.realpathSync.native(path.join(root, relative));
  assert(file.startsWith(root + path.sep), 'fixture write escapes the prepared project');
  const source = fs.readFileSync(file);
  const changed = transform(source);
  return {
    identity: {path: relative, original: hash(source), prepared: changed ? hash(changed) : null},
    edit() {
      assert.deepEqual(fs.readFileSync(file), source, relative + ' already changed');
      if (changed) fs.writeFileSync(file, changed);
      else fs.unlinkSync(file);
    },
    restore() {
      fs.writeFileSync(file, source);
      assert.deepEqual(fs.readFileSync(file), source);
    },
  };
}

recipes.set('heldout.H14', async state => {
  const variant = fixture(state, 'content/fixtures/crowds.json', original => {
    const authored = JSON.parse(original);
    authored.actors.find(row => row.id === 'corpse-000').equipment = {};
    return Buffer.from(JSON.stringify(authored));
  });
  const gear = {torso: 'IT-13', headwear: 'IT-18', backpack: 'IT-24', left: 'IT-03', right: 'IT-04'};
  const inspectBody = (id, equipment) => resourceStep(id, 'Characters', 'CH-13',
    [view('state', 'persistent corpse'), appearance('equipment', equipment), exists(corpse + '/Visual')]);
  const empty = inspectBody('empty-corpse', {});
  const emptyDrive = empty.drive;
  empty.drive = async function (driver) {
    await typeSearch(driver, 'CH-06');
    await driver.click(resources, {firstRow: true});
    variant.edit();
    await emptyDrive.call(this, driver);
  };
  return {stimulus: variant.identity, cleanup: variant.restore, steps: [
    resourceStep('bow-preview', 'Characters', 'AN-07', [sample('action', 'bow'), sample('nocked', true)]),
    select('actual-empty-ammo', 'Ammunition', '0', [sample('nocked', false)]),
    select('empty-left', 'left', 'Empty', [sample('leftItem', 'Empty'), absent(character + '/Art/left_held')]),
    select('empty-right', 'right', 'Empty', [sample('rightItem', 'Empty'), absent(character + '/Art/right_held')]),
    select('broken-equipped-preview', 'Condition', 'Broken', [sample('nocked', false)]),
    select('intrinsic-guard', 'BowSubject', 'Structural guard', [sample('nocked', true),
      exists(character + '/Art/GuardBow'), exists(character + '/Art/GuardArrow'),
      check(character + '/Art/StructuralGuard', 'metadata/loot', false),
      absent(character + '/Art/left_held'), absent(character + '/Art/right_held')]),
    inspectBody('nonempty-corpse', gear), empty,
  ]};
});

// Expectations derive from the authored track of the checkout under test, never from observations.
recipes.set('heldout.H16', async state => ({
  stimulus: 'Authored body registration, frame step and loop boundaries',
  steps: h16Steps(JSON.parse(fs.readFileSync(locomotionFile(state.root), 'utf8'))),
}));

recipes.set('heldout.H17', simple('Current transport reset and silent persistent corpse restore', [
  characterStep('character-preview', 'CH-04'),
  select('walking', 'Movement', 'Walk', paces('walk-near', 0)),
  select('active-melee', 'Action', 'Melee', [sample('action', 'melee')]),
  transport('advance-transport', 'Step', [sample('action', 'melee'), sample('movement', 'walk')]),
  transport('reset-transport', 'Reset', [sample('action', 'none'), sample('movement', 'idle'),
    sample('actionSeconds', 0), absent(character + '/Art/Contact')]),
  resourceStep('persistent-corpse', 'Characters', 'CH-13', [appearance('eventCount', 0),
    appearance('currentLoops', 0), appearance('transientVisible', false), oneBody]),
  option('explicit-death', choices, 'current death', [view('state', 'current death'), appearance('eventCount', 1)]),
  option('restore-silent-corpse', choices, 'persistent corpse', [view('state', 'persistent corpse'),
    appearance('eventCount', 0), appearance('currentLoops', 0), appearance('transientVisible', false), oneBody]),
  click('step-restored-corpse', resource + '/Controls/Step', [view('time', 0.125),
    appearance('eventCount', 0), appearance('currentLoops', 0), appearance('transientVisible', false), oneBody]),
]));

recipes.set('heldout.H18', simple('Replace a body and its actual gear during combined locomotion and a strike', [
  characterStep('walking-melee-body', 'CH-04'),
  select('held-left', 'left', 'IT-05', [sample('leftItem', 'IT-05')]),
  select('held-right', 'right', 'IT-06', [sample('rightItem', 'IT-06')]),
  select('start-locomotion', 'Movement', 'Walk', paces('walk-near', 0)),
  select('start-melee', 'Action', 'Melee', [sample('action', 'melee'), sample('movement', 'walk')]),
  ...[1, 2, 3].map(index => transport('step-active-melee-' + index, 'Step',
    [sample('action', 'melee'), sample('movement', 'walk'), exists(character + '/Art/Body_lower')])),
  select('replace-body', 'Body', 'CH-07', [sample('body', 'CH-07'), absent(character + '/Art/Contact')]),
  select('replace-left-gear', 'left', 'IT-03', [sample('leftItem', 'IT-03'),
    check(character + '/Art/left_held', 'metadata/item', 'IT-03'), absent(character + '/Art/Contact')]),
  select('replace-right-gear', 'right', 'IT-04', [sample('rightItem', 'IT-04'),
    check(character + '/Art/right_held', 'metadata/item', 'IT-04'), absent(character + '/Art/Contact')]),
  select('clear-old-action', 'Action', 'None', [sample('action', 'none'), absent(character + '/Art/Contact')]),
  select('restart-new-action', 'Action', 'Melee', [sample('body', 'CH-07'), sample('action', 'melee'),
    sample('movement', 'walk'), sample('leftItem', 'IT-03'), sample('rightItem', 'IT-04')]),
  transport('advance-concurrent-restart', 'Step', [sample('body', 'CH-07'), sample('action', 'melee'),
    sample('movement', 'walk'), exists(character + '/Art/Body_lower')]),
]));

recipes.set('UIH.CHARACTER-PREVIEW.empty', simple('Absent character identity through the native resource search', [
  characterStep(),
  {id: 'unavailable-character', control: search, checks: [absent(character),
    includes(browser + '/Count', 'text', '0 results'), includes(browser + '/Count', 'text', 'No resources match')],
    drive: driver => typeSearch(driver, 'CH-NO-DATA')},
]));

recipes.set('UIH.CHARACTER-PREVIEW.loading', simple('Recheck the declared resources from an installed character view', [
  characterStep(),
  click('current-character-loading', toolbar + '/Recheck', [
    text(toolbar + '/Operation', 'Reading declared packaged resources'),
    check(toolbar + '/Cancel', 'disabled', false), includes(summary + '/PackStatus', 'text', 'Pack incomplete — Loading'),
    absent(character)]),
]));

function malformedCharacter(state, file, transform, diagnostic) {
  const variant = fixture(state, file, transform);
  const error = resourceStep('failed-character', 'Characters', 'CH-06', [
    absent(character), includes(preview + '/Failures', 'text', file),
    includes(preview + '/Failures', 'text', diagnostic), check(toolbar + '/Recheck', 'disabled', false)]);
  const selectIt = error.drive;
  error.drive = async function (driver) {
    variant.edit();
    await selectIt.call(this, driver);
  };
  const recovered = characterStep('restored-character');
  const selectAgain = recovered.drive;
  recovered.drive = async function (driver) {
    variant.restore();
    await typeSearch(driver, 'CH-01');
    await selectAgain.call(this, driver);
  };
  return {stimulus: variant.identity, cleanup: variant.restore, steps: [error, recovered]};
}

recipes.set('UIH.CHARACTER-PREVIEW.error', async state => malformedCharacter(state,
  'content/animation/locomotion-melee.json', () => Buffer.from('{"id":"locomotion-melee","id":"locomotion-melee"}'),
  'CHARACTER_DUPLICATE_KEY'));
recipes.set('UIH.CHARACTER-PREVIEW.partial', async state => malformedCharacter(state,
  'content/animation/fits-c.json', () => null, 'CHARACTER_FITS_MISSING'));

recipes.set('UIH.CHARACTER-PREVIEW.overflow', simple('Scroll the bounded authored character control column', [
  characterStep(),
  {id: 'reach-overflowed-transport', control: characterScroll, checks: [positive(characterScroll, 'scroll_vertical'),
    exists(slots + '/Reset'), check(characterScroll, 'clip_contents', true)],
  drive: driver => driver.click(characterScroll + '/_v_scroll', {scrollPage: 1})},
]));

recipes.set('UIH.CHARACTER-PREVIEW.long-text', simple('Long character provenance in the original Details surface', [
  characterStep(),
  click('long-character-provenance', preview + '/Actions/Details', [subject('CH-06'),
    check(preview + '/Metadata', 'visible', true), check(preview + '/Metadata', 'wrap_mode', 1),
    includes(preview + '/Metadata', 'text', 'Explicit shared mappings: '),
    includes(preview + '/Metadata', 'text', 'CH-06')]),
]));

function collectionStep(state, suffix) {
  const file = path.join(state.root, 'src/godot-game/game/content/ui/inventory.json');
  const id = 'inventory.' + suffix;
  const {samples} = JSON.parse(fs.readFileSync(file, 'utf8'));
  const sample = samples.find(entry => entry.id === id);
  assert(sample, id);
  const declaredRows = sample.slots.map(slot => {
    const row = contents + '/Slots/' + slot.id;
    return slot.item ? exists(row + '/Item') : text(row + '/Empty', 'Empty');
  });
  const gridCount = {node: contents + '/Grids', property: null, childClass: 'VBoxContainer',
    value: sample.grids.length, match: 'child-count'};
  const checks = [view('controller', 'InventoryLayouts'), view('state', id), ...declaredRows, gridCount,
    text(contents + '/Title', sample.name + ' · ' + id),
    text(collection + '/Warning', 'World continues. ' + sample.warning),
    text(contents + '/Scope', 'Layout example — gameplay actions are not active.')];
  if (suffix === 'partial') checks.push(text(contents + '/Quick/Quick4', 'Food reference 4 · Unbound'));
  if (suffix === 'long-text') checks.push(check(collection + '/Warning', 'autowrap_mode', 3));
  return option('show-' + suffix, choices, id, checks);
}

const openCollection = () => resourceStep('source-inventory-fixture', 'UI', 'UI-12',
  [view('controller', 'InventoryLayouts'), view('state', 'inventory.broken'), exists(contents + '/Slots')]);
for (const [scope, fixtureName] of Object.entries({empty: 'empty', loading: 'loading', error: 'error',
  populated: 'normal', partial: 'partial', overflow: 'overflow', 'long-text': 'long-text'})) {
  recipes.set('UIH.UI-COLLECTIONS.' + scope, async state => {
    const steps = [openCollection(), collectionStep(state, fixtureName)];
    if (scope === 'overflow') steps.push({
      id: 'scroll-overflowed-inventory', control: collection + '/Inventory/Scroll',
      checks: [view('state', 'inventory.overflow'), positive(collection + '/Inventory/Scroll', 'scroll_vertical')],
      drive: driver => driver.click(collection + '/Inventory/Scroll/_v_scroll', {scrollPage: 1,
        scroll: resource + '/Art/LibraryScroll'}),
    });
    return {stimulus: 'Original UI-12 authored sample ' + fixtureName, steps};
  });
}
recipes.set('UIH.UI-COLLECTIONS.zero-one-many', async state => ({
  stimulus: 'Original UI-12 inventory layout with zero, one and many source items',
  steps: [openCollection(), ...['empty', 'one', 'normal'].map(suffix => collectionStep(state, suffix))],
}));

assert.deepEqual([...recipes.keys()].sort(), [...handoff].sort(), 'exact Phase 6 asset handoff recipes');

async function reset(state) {
  assert(state.session.usable);
  if (state.currentItem) await restore(state, state.currentItem, 2);
  state.currentItem = null;
}

module.exports = {open, close, reset, recipes, fixture,
  cases:Object.fromEntries(handoff.map(id => ['asset-pipeline.' + id, state => perform(state, id)]))};

