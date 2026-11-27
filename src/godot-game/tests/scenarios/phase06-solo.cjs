'use strict';

const assert = require('node:assert/strict');
const run = require('./phase06-rendered.cjs');
const {helpers: native} = require('./phase06-headless.cjs');
const {draws} = require('../../tools/lan-checks/rendered.cjs');
const {named, within, title, key, actor, own, nearby, external, item} = run;

const saved = (state, name, peer, start, observations, pictures) =>
  run.finish(state, 'phase06.solo.' + name, [peer], start, observations, pictures);
const at = state => state.peers[0];
// Every solo image shows a settled, unclipped view in addition to the case's own state.
const shot = (state, peer, label, predicate, minimumFrame) => run.picture(state, peer, label,
  (row, draw) => run.settledView(draw) && predicate(row, draw), minimumFrame);
const field = draw => draw.controls.find(entry => named('QuantityToMove')(entry) && entry.visible);
const contents = (row, id) => Object.entries(row.phase06.Views).find(([scope, view]) =>
  scope.startsWith('own.') && view.Header.ContainerId === id)?.[1].Rows ?? [];

async function world(state) {
  const peer = at(state), start = state.input.records.length;
  const first = await run.enter(state, peer, 'field');
  const initial = first.input;
  const camp = initial.phase06.World;
  assert(camp.CampLeft < camp.CampRight);
  const stationary = camp.Actors.filter(entry => entry.Hostile);
  assert.equal(stationary.length, 3);
  assert.equal(stationary[0].X, stationary[1].X);
  assert.notEqual(stationary[0].Id, stationary[1].Id);
  assert.deepEqual(camp.Defenses.map(({Id, Living}) => [Id, Living]),
    [['fixture.living-wall', true], ['fixture.broken-wall', false]]);
  const walked = await run.moveUntil(state, peer, key.right, row => actor(row)?.X > actor(initial).X + 500,
    'controlled native walk');
  const moved = await shot(state, peer, 'world-walked', (row, draw) => {
    const local = draw.actors.find(entry => entry.local);
    return local?.insideCamera && local.visible && actor(row)?.X > actor(initial).X + 500
      && draw.appearance.filter(entry => entry.kind === 'Actors' && entry.visible).length === camp.Actors.length
      && row.phase06.World.Actors.filter(entry => entry.Hostile)
        .every(entry => stationary.some(value => value.Id === entry.Id && value.X === entry.X));
  }, first.drawFrame);
  return saved(state, 'world', peer, start, {initial: first, walked}, [moved]);
}

async function movement(state) {
  const peer = at(state), start = state.input.records.length;
  const first = await run.enter(state, peer, 'field');
  assert.equal(first.input.hud.slots, 4);
  assert(first.input.phase06.Views.status.Header.Hud.Weight > 0);
  await run.press(state, peer, key.right, true);
  let walked, walking;
  try {
    const walkStart = await run.committed(peer, row => actor(row)?.Movement === 'walk', 'committed walk starts');
    walked = await run.committed(peer, row => !!(walking = run.motionInterval(walkStart.input, row, 'walk')),
      'walking displacement over committed time', walkStart.drawFrame);
  } finally { await run.press(state, peer, key.right, false); }
  await run.press(state, peer, key.shift, true);
  let sprinted, sprint;
  try {
    await run.press(state, peer, key.right, true);
    try {
      const sprintStart = await run.committed(peer, row => actor(row)?.Movement === 'faster',
        'committed sprint starts', walked.drawFrame);
      sprinted = await shot(state, peer, 'movement-sprint', row => {
        sprint = run.motionInterval(sprintStart.input, row, 'faster');
        return sprint?.speed > walking.speed && sprint.spent > 0;
      }, sprintStart.drawFrame);
    } finally { await run.press(state, peer, key.right, false); }
  } finally { await run.press(state, peer, key.shift, false); }
  const beforeJump = run.last(peer);
  await run.press(state, peer, key.jump, true);
  let airborne;
  try {
    airborne = await shot(state, peer, 'movement-air', row => {
      const player = actor(row);
      return player?.Movement === 'jump' && player.Y > row.phase06.World.Ground;
    }, beforeJump.drawFrame);
  } finally { await run.press(state, peer, key.jump, false); }
  await run.committed(peer, row => actor(row)?.Movement !== 'jump', 'first jump completed');
  await run.press(state, peer, key.jump, true);
  const airStart = await run.committed(peer, row => actor(row)?.Movement === 'jump', 'second jump committed');
  const airControl = await run.moveUntil(state, peer, key.left, row => actor(row)?.Movement === 'jump'
    && actor(row).X < actor(airStart.input).X, 'native aerial reversal');
  await run.press(state, peer, key.jump, false);
  const settled = await run.committed(peer, row => actor(row)?.Y === row.phase06.World.Ground, 'normal landing');
  await run.tap(state, peer, key.inventory);
  const panel = await run.committed(peer, row => row.inventoryOpen, 'inventory captures controls');
  await run.tap(state, peer, key.jump);
  const blocked = await run.committed(peer, row => row.inventoryOpen && actor(row)?.Y === row.phase06.World.Ground,
    'panel keeps jump at ground', panel.drawFrame);
  return saved(state, 'movement', peer, start, {first, walked, walking, sprint, airControl, settled, blocked},
    [sprinted, airborne]);
}

async function inventory(state) {
  const peer = at(state), start = state.input.records.length;
  await run.enter(state, peer, 'field');
  await run.tap(state, peer, key.inventory);
  const equipped = await shot(state, peer, 'inventory-equipped', row => row.inventoryOpen
    && row.inventorySlots === 11 && !!item(row, 'own', 'IT-23'));
  const bag = item(equipped.draw.input, 'own', 'IT-23');
  await run.bagWindow(state, peer, named('equipment_backpack'), bag.Id, 'own');
  const open = await run.committed(peer, row => contents(row, bag.Id).some(entry => entry.DefinitionId === 'IT-22'),
    'current carried pouch');
  const pouch = contents(open.input, bag.Id).find(entry => entry.DefinitionId === 'IT-22');
  const inBag = name => within('/Bag_' + bag.Id + '/', name);
  await run.bagWindow(state, peer, inBag('Item_' + pouch.Id), pouch.Id, 'own');
  const nested = await shot(state, peer, 'inventory-nested', row => contents(row, pouch.Id)
    .some(entry => entry.DefinitionId === 'IT-08' && entry.Quantity === 6));
  const berries = contents(nested.draw.input, pouch.Id).find(entry => entry.DefinitionId === 'IT-08');
  const inPouch = name => within('/Bag_' + pouch.Id + '/', name);
  const seedItem = inPouch('Item_' + berries.Id);

  await run.choose(state, peer, seedItem, 'right');
  await run.choose(state, peer, title('Assign to quick slot'));
  await run.choose(state, peer, title('Slot 1'));
  const assigned = await run.committed(peer, row => own(row)?.Header.Quick.some(entry =>
    entry.Slot === 1 && entry.ItemId === berries.Id), 'physical context assignment');
  // Shift-press the stack, cross the drag start threshold onto the empty pouch cell (1, 0) and see the exact split
  // field open with Half = floor(6 / 2) while the button is still held.
  const stack = await run.control(peer, seedItem);
  const pouchBoard = await run.control(peer, inPouch('ItemCells'));
  const emptyCell = {x: pouchBoard.x - pouchBoard.width / 2 + 48, y: pouchBoard.y - pouchBoard.height / 2 + 16};
  const half = String(Math.floor(berries.Quantity / 2));
  await run.press(state, peer, key.shift, true);
  let opened;
  try {
    opened = await run.drag(state, peer, {x: stack.x, y: stack.y}, emptyCell, before => run.after(peer,
      draw => field(draw)?.text === half, 'Shift-drag opens the split field', before));
  } finally { await run.press(state, peer, key.shift, false); }
  const split = await shot(state, peer, 'inventory-split', (_, draw) => field(draw)?.text === half,
    opened.drawFrame - 1);
  await run.choose(state, peer, title('Move quantity'));
  const board = await run.control(peer, inPouch('ItemCells'));
  await run.click(state, peer, {x: board.x - board.width / 2 + 48, y: board.y - board.height / 2 + 16});
  const divided = await run.committed(peer, row => {
    const stack = contents(row, pouch.Id).filter(entry => entry.DefinitionId === berries.DefinitionId);
    return stack.length === 2 && stack.every(entry => entry.Quantity === 3)
      && stack.some(entry => entry.Id === berries.Id);
  }, 'exact integral split preserves source identity');

  const ammo = contents(divided.input, bag.Id).find(entry => entry.DefinitionId === 'IT-02');
  const from = await run.control(peer, inBag('Item_' + ammo.Id));
  const bagBoard = await run.control(peer, inBag('ItemCells'));
  const destination = {x: bagBoard.x - bagBoard.width / 2 + 16, y: bagBoard.y - bagBoard.height / 2 + 80};
  const preview = draw => draw.controls.find(entry => named('PlacementPreview')(entry) && entry.visible);
  await run.drag(state, peer, from, destination, async before => {
    const upright = await run.after(peer, draw => preview(draw)?.height > preview(draw).width,
      'upright ammunition placement preview', before);
    await run.tap(state, peer, key.rotate);
    return run.after(peer, draw => preview(draw)?.width > preview(draw).height, 'rotated placement preview',
      upright.drawFrame);
  });
  const rotated = await shot(state, peer, 'inventory-rotated', row => {
    const moved = contents(row, bag.Id).find(entry => entry.Id === ammo.Id);
    return moved?.Location.Rotated === true && moved.Location.X === 0 && moved.Location.Y === 2;
  });
  await run.choose(state, peer, named('CloseInventory'));
  await run.committed(peer, row => !row.inventoryOpen, 'native Close inventory');
  await run.tap(state, peer, key.food1);
  const food = await run.committed(peer, row => row.runAction?.status === 'Consumed'
    && row.hud.Health > 0 && !row.inventoryOpen, 'physical quick food after closing');
  await run.tap(state, peer, key.food1);
  const cooldown = await run.committed(peer, row => row.runAction?.sequence > food.input.runAction.sequence
    && /cooldown/i.test(row.runAction.status), 'visible real food cooldown');
  return saved(state, 'inventory', peer, start, {assigned, divided, food, cooldown}, [equipped, nested, split, rotated]);
}

async function stash(state) {
  const peer = at(state), start = state.input.records.length;
  const joined = await run.enter(state, peer, 'field');
  const selection = await overlappingSelection(state, peer, joined);
  await run.stash(state, peer);
  const first = await shot(state, peer, 'stash-first', row => external(row)?.Header.Page === 1
    && external(row).Header.PageCount > 1 && row.nearby.kind === 'Stash');
  await run.choose(state, peer, named('NextStashPage'));
  const second = await shot(state, peer, 'stash-next', row => external(row)?.Header.Page === 2
    && row.nearby.page === 2, first.draw.drawFrame);
  await run.choose(state, peer, named('PreviousStashPage'));
  const back = await run.committed(peer, row => external(row)?.Header.Page === 1, 'returned to original stash page');
  const money = item(back.input, 'external', 'IT-01');
  await run.press(state, peer, key.control, true);
  try { await run.choose(state, peer, within('/NearbyStorage/', 'Item_' + money.Id)); }
  finally { await run.press(state, peer, key.control, false); }
  const committed = await shot(state, peer, 'stash-withdrawal', row => row.runAction?.status === 'Applied'
    && external(row)?.Rows.every(entry => entry.Id !== money.Id)
    && row.phase06.Views.status.Header.Hud.Money >= money.Quantity, back.drawFrame);
  await run.choose(state, peer, named('CloseInventory'));
  await run.moveUntil(state, peer, key.left, row => !nearby(row).some(entry => entry.Kind === 'Stash'),
    'walk out of the stash access range');
  const revoked = await shot(state, peer, 'stash-out-of-range', row => !external(row)
    && !nearby(row).some(entry => entry.Kind === 'Stash'));
  return saved(state, 'stash', peer, start, {back, money, cleared: selection.cleared},
    [...selection.pictures, first, second, committed, revoked]);
}

async function overlappingSelection(state, peer, joined) {
  const candidates = nearby(joined.input).filter(row => row.Kind === 'Npc');
  assert(candidates.length >= 2, 'the stationary fixture exposes overlapping eligible targets');
  const [first, second] = candidates.slice(0, 2).map(row => row.Id);
  assert.equal(actor(joined.input, first).X, actor(joined.input, second).X);
  assert.equal(actor(joined.input, first).Y, actor(joined.input, second).Y);
  await run.tap(state, peer, key.interact);
  const pictures = [];
  for (const [index, id] of [first, second].entries()) {
    if (index) await run.choose(state, peer, named('NearbyTarget'));
    await run.choose(state, peer, named('NearbyChoice_Npc_' + Buffer.from(id).toString('hex').toUpperCase()));
    pictures.push(await shot(state, peer, 'nearby-outline-' + index, (row, draw) => {
      const marks = draw.appearance.filter(art => art.kind === 'NearbySelection' && art.visible);
      return external(row)?.Header.TargetId === id && marks.length === 1 && marks[0].id === id && marks[0].outlined;
    }));
  }
  const before = await run.choose(state, peer, named('CloseInventory'));
  const cleared = await run.committed(peer, (row, draw) => !row.inventoryOpen && !external(row)
    && !draw.appearance.some(art => art.kind === 'NearbySelection'), 'dismissed target clears both visual cues', before);
  return {pictures, cleared};
}

async function combat(state) {
  const peer = at(state), start = state.input.records.length;
  await run.enter(state, peer, 'bow');
  const point = {x: 760, y: 370};
  await run.pointer(state, peer, point);
  const initial = await shot(state, peer, 'combat-ready', (row, draw) => actor(row)?.ArrowReady
    && item(row, 'own', 'IT-02')?.Quantity === 1 && !!row.hud && draw.crosshair);
  const bow = item(initial.draw.input, 'own', 'IT-07');
  const before = await run.click(state, peer, point);
  const fired = await shot(state, peer, 'combat-fired', (row, draw) => row.aim && !actor(row)?.ArrowReady
    && !item(row, 'own', 'IT-02') && item(row, 'own', 'IT-07')?.Condition < bow.Condition
    && draw.appearance.some(art => art.id === row.phase06.control && art.bowStage === 'recovery'), before);
  await run.committed(peer, row => actor(row)?.StrikeAt === null, 'bow ready to test the empty attempt');
  await run.click(state, peer, point);
  const empty = await shot(state, peer, 'combat-empty', (row, draw) => row.runAction?.status === 'EmptyAmmo'
    && !actor(row)?.ArrowReady && item(row, 'own', 'IT-07')?.Condition === item(fired.draw.input, 'own', 'IT-07').Condition
    && row.hud.Ammo === 0 && draw.crosshair && draw.appearance.some(art =>
      art.id === row.phase06.control && art.bowStage === 'aim') && draw.controls.some(entry =>
      named('ActionNotice')(entry) && !entry.text));
  await run.fixture(state, peer, {action: 'fixture-attack', Target: 'fixture.bow'});
  await run.fixture(state, peer, {action: 'fixture-step', Elapsed: 500});
  const defense = await shot(state, peer, 'combat-broken-defense', row => row.phase06.World.Defenses
    .find(entry => entry.Id === 'fixture.living-wall')?.Living === false);
  return saved(state, 'combat', peer, start, {point}, [initial, fired, empty, defense]);
}

async function life(state) {
  const peer = at(state), start = state.input.records.length;
  const first = await run.enter(state, peer, 'life');
  const previous = first.input.phase06.control;
  await run.moveUntil(state, peer, key.right, row => actor(row)?.X > actor(first.input).X + 300,
    'separate the death and eligible near successor camera centers');
  await run.fixture(state, peer, {action: 'fixture-attack', Target: 'fixture.melee-a'});
  const dead = await shot(state, peer, 'life-death', (row, draw) => row.spectator?.Visible && !row.hud && !own(row)
    && row.phase06.World.Corpses.some(entry => entry.SurvivorId === previous
      && entry.Equipment.some(piece => piece.DefinitionId === 'IT-23')
      && draw.appearance.some(art => art.id === entry.Id && art.kind === 'Corpses' && art.visible && !art.crown)));
  const gate = await shot(state, peer, 'life-ready', row => row.spectator?.CanRespawn);
  const nearSpan = since(peer, gate.draw);
  await run.choose(state, peer, named('Respawn'));
  const transferred = await shot(state, peer, 'life-transfer', row => nearSpan().length > 0
    && row.phase06.control !== previous && row.phase06.epoch > 0 && row.bindingId === first.bindingId
    && !!row.spectator?.TransferNotice && !row.spectator.Smoothing);
  const near = handover(nearSpan, transferred.draw);
  const origin = dead.draw.cameraX;
  const destination = transferred.draw.cameraX;
  assert(Math.abs(origin - destination) > 2, 'near camera endpoints must differ');
  const travelled = near.filter(draw => draw.input.spectator.Smoothing
    && draw.cameraX > Math.min(origin, destination) + 0.02 && draw.cameraX < Math.max(origin, destination) - 0.02);
  assert(travelled.length >= 2 && travelled.every((draw, index) => index === 0
    || Math.abs(draw.cameraX - destination) <= Math.abs(travelled[index - 1].cameraX - destination)),
  'near handover travels through successive intermediate native draws toward the successor');
  const corpse = dead.draw.input.phase06.World.Corpses.find(entry => entry.SurvivorId === previous);
  await run.fixture(state, peer, {action: 'fixture-step', Elapsed: 180000});
  const cleanup = await shot(state, peer, 'life-cleanup', row => row.phase06.World.Corpses
    .some(entry => entry.Id === corpse.Id) && row.phase06.World.Loot.some(entry => entry.Id === 'fixture.supplies')
    && !row.phase06.World.Loot.some(entry => entry.Id === 'fixture.empty'));
  await run.menu(state, peer);
  const distant = await farHandover(state, peer);
  return saved(state, 'life', peer, start, {first, corpse, travelled, cut: distant.cut},
    [dead, gate, transferred, cleanup, distant.dead, distant.live]);
}

// Every committed draw after a known draw, collected across reads without gaps (FRAME_SPAN_GAP otherwise). The
// shared reader keeps only its latest 256 draws, so the span is read once before the Respawn click and then polled
// by the picture that ends the handover.
function since(peer, previousDraw) {
  const span = native.contiguous(() => draws(peer), 'drawFrame', previousDraw.drawFrame + 1);
  span();
  return span;
}

// The transfer draws of a gap-free span, up to the picture that ends the handover.
function handover(span, lastDraw) {
  return span().filter(draw => draw.drawFrame <= lastDraw.drawFrame
    && draw.input.phase06?.epoch === lastDraw.input.phase06.epoch
    && draw.snapshot?.SessionId === lastDraw.snapshot.SessionId && draw.input.spectator?.TransferNotice);
}

async function farHandover(state, peer) {
  const first = await run.enter(state, peer, 'far');
  await run.fixture(state, peer, {action: 'fixture-attack', Target: 'fixture.melee-a'});
  const dead = await shot(state, peer, 'life-distant-death', row => row.spectator?.CanRespawn);
  const farSpan = since(peer, dead.draw);
  await run.choose(state, peer, named('Respawn'));
  const live = await shot(state, peer, 'life-distant-cut', row => farSpan().length > 0 && row.phase06.epoch > 0
    && first.input.phase06.control !== row.phase06.control && !!row.spectator?.TransferNotice);
  const cut = handover(farSpan, live.draw);
  assert(cut.length > 0 && Math.abs(cut[0].cameraX - dead.draw.cameraX) > 300
    && cut.every(draw => !draw.input.spectator.Smoothing && Math.abs(draw.cameraX - live.draw.cameraX) < 0.02),
  'distant handover cuts on the first committed draw without an intermediate position');
  return {dead, live, cut};
}

module.exports = {open: context => run.open(context, ['solo']), reset: run.reset, close: run.close, cases: {
  'phase06.solo.world': world,
  'phase06.solo.movement': movement,
  'phase06.solo.inventory': inventory,
  'phase06.solo.stash': stash,
  'phase06.solo.combat': combat,
  'phase06.solo.life': life,
}};

