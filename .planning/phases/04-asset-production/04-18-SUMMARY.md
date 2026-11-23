---
phase: 04-asset-production
plan: "18"
subsystem: content-data
tags: [json, baseline, strict-validation, source-crosswalk, H24]
requires:
  - phase: 04-05
    provides: All 24 canonical item identities and actual item packs, icons and appearance inputs
  - phase: 04-06
    provides: Existing survivor body and fits-a resource references
provides:
  - Six separate editable DT-01 through DT-06 baseline documents
  - Exhaustive 984-entry explicit field descriptions with defaults, units, constraints, dependencies and source references
  - Reusable bounded strict parser, field walker, reference and relationship validators
  - Two saved source/headless cases with independent malformed-input, baseline and H24 precision controls
affects: [04-19, 04-20, 04-22, 04-23, 04-31, 04-32, 04-33, survivor-gameplay, inventory-gameplay]
tech-stack:
  added: []
  patterns: [explicit-field-inventory, pinned-source-table-crosswalk, ordinal-reference-resolution, data-only-validation]
key-files:
  created:
    - src/godot-game/content/config/run.json
    - src/godot-game/content/config/time.json
    - src/godot-game/content/config/survivors.json
    - src/godot-game/content/config/items.json
    - src/godot-game/content/config/combat.json
    - src/godot-game/content/config/inventory.json
    - src/godot-game/content/schema/fields-foundation.json
    - src/godot-game/content/schema/fields-items-combat.json
    - tools/phase4/cases/data-foundation.cjs
  modified: []
key-decisions:
  - Authenticate baseline source at committed revision 0c6e959, preserving foreign future-phase specification discussion without consuming it as approved Phase 4 baseline.
  - Validate actual item, image, appearance, fit and pack references separately from declared sound obligations and item-pack translation keys awaiting their production catalogs.
  - Keep data/source validation separate from gameplay, current-hub adoption, native source-policy permission, actual rendering and final acceptance.
requirements-addressed: [ASSET-02, ASSET-04]
requirements-completed: []
actuals:
  tokens: 223342
  tasks: 2
  commits: 2
commits: 2
plan_head_before: 0c6e9590c66ad2f3fe975bb36ce09d24ea72adca
plan_head_after: 7e3b7e5de3f635cea22abeba397756c401e3eb36
coverage:
  - id: DT-01-03
    description: Complete run time and survivor data with exhaustive source field documentation
    requirement: ASSET-04
    verification:
      - kind: other
        ref: .phase4/runs/390fc7b3-29c1-46b7-836b-bda77542b8dc/result.json
        status: pass
    human_judgment: false
  - id: DT-04-06
    description: All item combat inventory baseline data and H24 source precision validation
    requirement: ASSET-04
    verification:
      - kind: other
        ref: .phase4/runs/4d5d4ed7-ac43-42f4-bc5c-0862529c5d41/result.json
        status: pass
    human_judgment: false
duration: 16min recorded production window; initialization reading precedes that measured window
completed: 2026-11-17
status: complete
---

# Phase 04 Plan 18: Complete run, survivor, item, combat and inventory baseline Summary

**Six separate authored baseline domains preserve exact source values and all 24 item identities, with 984 explicit field records, reusable strict diagnostics and independent source/precision checks.**

## Performance and task commits

The recorded production window starts with the first authored file at `2026-11-17T04:26:26.762Z`; the final production commit and identity measurement finished at `2026-11-17T04:42:30.499Z`. Initialization/context reading preceded this measured 16-minute window. Both tasks completed and all nine implementation paths were created. No other plan was executed.

| Task | Commit | Delivered scope |
| --- | --- | --- |
| 1 — Every run/time/survivor field with source-table crosswalk | `02ae8841975315d3c5733da06bae411e09474f13` | Three foundation configs, 202-entry field inventory and reusable foundation saved case |
| 2 — All 24 definitions and full combat/armor/inventory baselines | `7e3b7e5de3f635cea22abeba397756c401e3eb36` | Three additional configs, 782-entry field inventory, item/combat saved case and affected shared validation expansion |

The persisted `.git/gsd-plan-head-before-04-18` ledger records `0c6e9590c66ad2f3fe975bb36ce09d24ea72adca`. `git rev-list --count <base>..7e3b7e5` measured **2** production commits. The realized nine-path diff is **893,365 characters**, giving **223,342 estimate-scale tokens** by `ceil(chars/4)`. This is substantially above the 45,000 estimate because every structural and scalar field carries a literal default and explicit metadata; it is not a harness token count. This summary's separate documentation commit follows those measured production commits.

## Source authority and exact baseline inventory

The independent source is `specs/0.1/initial-spec.md` at committed revision `0c6e9590c66ad2f3fe975bb36ce09d24ea72adca`, SHA256 `f5aa78ee3bbb8d3b1cdd49bf5da89d687d75f164aaf975676fcf57e43c6e688e`. Each data invocation saves those exact source bytes as `source-spec.md`. Numeric item/attack/armor/storage rows are parsed from the actual source tables; foundation attributes, stash quantities, time values, caps, interaction distances, hauling and cleanup also come from their source table/paragraph. Policy assertions carry explicit source excerpts. These are proposed designer defaults where the source marks them proposed, with no claim of tested balance.

The foreign working-copy filtering discussion remains untouched. Committed Phase 4 UI/resource obligations are preserved. No inventory/stash filtering implementation or future INV-14 acceptance is claimed by these data files.

### DT-01 run setup

There are eight starting survivors, at most four players, caps of 100 recruited survivors including controlled characters, 300 active zombies and 24 living wildlife. `EN-01` is the declared handcrafted-map obligation; it is not a claim that this plan produced/resolved the map. A run starts on day 1 at dawn, main tier 1, six living portals, marked sites unbuilt and uncontrolled starters unassigned. All starters and new recruits wear IT-11/IT-12/IT-17 only. Supplies are once per shared run, independent of participants; recruitment does not replenish them.

The explicit initial stash is 200 money in 2×100, four full-durability clubs, four full-durability bows, 240 arrows in 8×30, four empty small backpacks, four empty belts with pouches, 20 berries in 2×10, ten fruit in 2×5 and ten meat in 2×5: **32 legal stacks**. Containers begin empty. The records distinguish full condition from not applicable.

Succession delay is 1 second with random eligible uncontrolled recruits, FIFO waiting players, preservation of actual state and reconnect priority for the previous available survivor. The cosmetic crown consumes no equipment slot and becomes no loot. Outcomes explicitly encode immediate six-portal victory, main-destruction/no-eligible-survivor defeat, defeat precedence in a simultaneous first resolution, continuing after victory, retention of awarded victory and cancellation of destroyed portals' pending spawns.

### DT-02 time

Daylight/night are **600/300 seconds**, opening scheduled-wave-free days **1**, escalation interval **3 days**, origin day/stage **1/0**, dawn stage boundary and first scheduled night **2**. One regular wave is configured per surviving portal per eligible night. Stage checks observe day/stage pairs 1/0, 4/1, 7/2, 10/3. Opening grace does not suppress provocation or retaliation. Active gameplay does not pause. Ordered dawn events are advance day, update stage, refill plants, introduce wildlife, retreat eligible zombies and autosave, once per dawn without replay on load/join. Phase transitions occur at boundaries and visual conversion cannot change authored durations; no invented blend duration is supplied.

### DT-03 survivors

| Attribute | Authored default and unit |
| --- | --- |
| Health / stamina maxima | 100 health / 100 stamina |
| Walk / sprint | 3 / 5 u/second |
| Jump height / level-ground airborne duration | 1.5 u / 0.8 seconds |
| Air walk control | 0.75 of ground walk control |
| Sprint / jump / walking stamina | 10 stamina/second / 12 stamina/jump / 0 |
| Stamina recovery / delay | 14 stamina/second / 0.5 seconds since spending |
| Safe health recovery / rest delay | 2 health/second / 5 seconds |
| Unsafe natural healing | 0 health/second |
| Enemy clearance / no-wall refuge | 12 u / 12 u |
| Player food cooldown | 2 seconds; immediate recovery clamped to maxima |
| Soft / severe load | 20 / 35 kg |
| Minimum speed / maximum stamina cost multiplier | 0.55 / 1.5 |

Safe rest requires inside camp, no sprint/jump/attack and sufficient enemy clearance; ordinary walking is allowed. Stamina can recover during combat; spending restarts its delay and actions require sufficient stamina. The authored load curve is linear between thresholds with endpoint clamping. Severe load prohibits sprint/jump but always permits walking; nested and equipped weights count exactly once. Only actual survivor body **CH-01** is referenced; CH-02/03 are existing zombie bodies and are excluded from the survivor list. Body pack and starter outfit references resolve against existing production documents.

The local binding template preserves A/Left, D/Right, Shift, Space, MouseLeft, mouse-position aim, E, I, R, Ctrl+click, Shift+drag with quantity choice, food keys 1–4, Tab and Escape. It explicitly declares local-preference scope, remapping and reset-to-default; it does not implement shared-host binding overrides or a settings screen.

### DT-04 all canonical item definitions

All outer footprints may rotate. Weights are per individual, retain their authored decimal precision and include no hidden unit conversion. All equipment and containers have stack maximum 1; compatible consumables retain their source maxima. A clothing pair is a single item.

| ID / label | Footprint cells | Maximum stack | kg/item | Slot | Internal grid | Maximum condition |
| --- | --- | --- | --- | --- | --- | --- |
| IT-01 Money | 1×1 | 100 | 0.001 | none | none | not applicable |
| IT-02 Arrows | 1×2 | 30 | 0.03 | either logical hand | none | not applicable |
| IT-03 Knife | 1×2 | 1 | 0.3 | either logical hand | none | 120 |
| IT-04 Wooden club | 1×3 | 1 | 1.2 | either logical hand | none | 150 |
| IT-05 Machete | 1×3 | 1 | 0.8 | either logical hand | none | 180 |
| IT-06 Metal bat | 1×4 | 1 | 2 | either logical hand | none | 220 |
| IT-07 Bow | 2×4 | 1 | 1.2 | either logical hand | none | 160 |
| IT-08 Berries | 1×1 | 10 | 0.1 | none | none | not applicable |
| IT-09 Fruit | 1×1 | 5 | 0.2 | none | none | not applicable |
| IT-10 Meat | 1×2 | 5 | 0.4 | none | none | not applicable |
| IT-11 Basic shirt | 2×2 | 1 | 0.3 | torso-clothing | 2×1 | not applicable |
| IT-12 Trousers | 2×2 | 1 | 0.6 | legs | 2×2 | not applicable |
| IT-13 Coat | 2×3 | 1 | 1 | torso-clothing | 2×2 | not applicable |
| IT-14 Cap | 2×1 | 1 | 0.1 | headwear | none | not applicable |
| IT-15 Face covering | 1×1 | 1 | 0.05 | face | none | not applicable |
| IT-16 Gloves | 1×1 | 1 | 0.1 | gloves | none | not applicable |
| IT-17 Boots | 1×2 | 1 | 0.8 | boots | none | not applicable |
| IT-18 Protective helmet | 2×2 | 1 | 1 | headwear | none | 100 |
| IT-19 Improvised body armor | 2×3 | 1 | 3 | body-armor | none | 100 |
| IT-20 Plated body armor | 3×3 | 1 | 7 | body-armor | none | 200 |
| IT-21 Belt with pouches | 2×1 | 1 | 0.3 | belt | 4×2 | not applicable |
| IT-22 Small pouch | 1×2 | 1 | 0.15 | none | 2×2 | not applicable |
| IT-23 Small backpack | 2×3 | 1 | 0.8 | backpack | 4×4 | not applicable |
| IT-24 Field backpack | 3×4 | 1 | 1.8 | backpack | 5×6 | not applicable |

Each definition contains its actual item-pack name/description key, appearance pack, item identity, canonical icon and applicable fit resource/item identity. Null storage, slot, condition or fit values are explicit legitimate non-applicability, never missing producers masquerading as completed content. D-20 keeps one canonical inventory icon; current/max condition plus Broken presentation is recorded separately.

### DT-05 combat, armor, projectiles and condition

| Profile ID | Item | Raw damage | Interval seconds | Reach / flight | Stamina |
| --- | --- | --- | --- | --- | --- |
| survivor.fists | natural | 8 | 0.7 | 1 u | 3 |
| weapon.knife | IT-03 | 10 | 0.5 | 1.1 u | 4 |
| weapon.club | IT-04 | 18 | 0.9 | 1.4 u | 7 |
| weapon.machete | IT-05 | 26 | 0.75 | 1.5 u | 8 |
| weapon.bat | IT-06 | 32 | 1.1 | 1.6 u | 10 |
| weapon.bow | IT-07 | 22 | 1.2 | arrow: 25 u/second, 2-second lifetime | 6 |
| spitter.spit | natural | 12 | 3 | 16-u targeting range; spit 8 u/second, 3-second lifetime | 6 |

Projectile IDs are exactly `arrow` and `spit`. Both stop at terrain and their first eligible hit, ignore allies, are not recoverable, have no area damage and leave no lingering gameplay effect. Bow reach is explicitly non-applicable to melee reach; spit targeting range is distinct from its speed×lifetime flight bound.

Improvised/plated/helmet armor profiles reference IT-19/20/18, reduce damage by 0.2/0.4/0.1, have 100/200/100 durability and wear by 0.2/0.2/0.1 of the same incoming raw hit. Reduction combines multiplicatively with a 0.6 cap and applies per hit. Friendly damage multiplier is zero; critical hits, headshots, penetration and bleeding are false. Melee wears one per successful strike and bows one per shot. Zero-condition weapons cannot attack, zero-condition armor cannot protect, broken items remain, ordinary clothing/bags/belts do not wear and equipment repair is false.

Both empty hands use fists. Dual one-handed weapons swing together, use maximum interval and summed stamina, and may independently hit the same target. One eligible target is allowed per weapon swing. Opposite a bow only IT-02 arrows are compatible; players do not auto-refill, while ordinary NPCs/zombies transfer actual carried reserve stacks and consume actual ammunition. Fallback is usable carried weapon or fists. Maximum simultaneous melee-zombie contacts are six per structure and three per survivor. Cosmetic acid has zero damage, durability loss and slowdown, no obstruction and no status effect. These are data definitions and legal-combination checks, not executed combat.

### DT-06 inventory, access, hauling and legacy layout

Stash pages are 10×10 cells and unlimited, independent of tier, with no remote access. Placement order is combine compatible stacks, fitting empty spaces, rotate when allowed, create page. Current nesting is three container levels, with worn/direct-stash containers at level 1 and no cycles. Four quick slots reference actual carried food stacks only; removal/consumption/loss invalidates the exact reference and never substitutes a similar stack.

Interaction ranges are 4 u for the main building and 3 u each for NPC, corpse, site and rally. Optional hauling respects safety/primary duty, daylight exterior work, reservation before pickup and removal only after successful pickup. Return occurs when no more loot fits or at 0.8 of the 20-kg soft load, **16 kg**. Priority is money, ammunition, usable combat gear, food, containers, clothing, broken gear. Unfitting items remain, haulers retain roles, tower guards never haul and collected cargo stays on the carrier's corpse on death.

Physical corpses/loot piles expire only 180 seconds after completely empty; nonempty belongings do not age out, adding an item cancels cleanup and cosmetic fading cannot delete loot. Legacy metadata preserves positions, orientation, old footprints, grid bounds and containment, marks incompatibility and permits removal. It rejects new invalid placements/deeper invalid nesting, uses current definitions at destination, retains the source after failed movement, allows a filled unit only with legal outer placement/no worse internal invalid depth, adopts current grid when empty, extracts legal substacks without truncation, prevents illegal loadouts from attacking/protecting, persists unresolved layouts through saves and errors explicitly while preserving original saves for missing definitions.

The file also records one-owner/location identity, immediate access with zero action timer, controlled-player privacy, non-destructive drag preview, authored numeric precision and numeric current/max plus Broken presentation. UI-05/06/13/14/15 remain explicit resource-inspection obligations; data records do not implement their UI or filtering transactions.

## Verification and exact invocation identities

The final selection was declared before running: new/changed `data.foundation`, new/changed `data.items-combat`, and affected Critical `source.case-discovery`. All commands are distinct invocations with exactly one `--case`. Inputs were frozen in `.phase4/04-18-frozen-selection.json`; no edits occurred during accepted runs. After Task 2 commit every one of the nine committed blobs was rehashed against that frozen manifest and matched.

| Canonical command | Final fresh invocation | Result and observations |
| --- | --- | --- |
| `node tools/phase4/check.cjs --case data.foundation` | `390fc7b3-29c1-46b7-836b-bda77542b8dc` | PASS; 202 walked/documented fields, 77 source assertions, 19 rejected invalid fixtures, two accepted independent edits |
| `node tools/phase4/check.cjs --case data.items-combat` | `4d5d4ed7-ac43-42f4-bc5c-0862529c5d41` | PASS; 782 walked/documented fields, 307 source assertions, 33 rejected invalid fixtures, five accepted legal hand combinations |
| `node tools/phase4/check.cjs --case source.case-discovery` | `cd5a840d-02b8-484b-b360-91b448e1b684` | PASS; actual 25-case catalog, original registrations preserved, 35 expected rejections, four actual CLI children, zero case bodies executed during import |

Runner metadata records HEAD `02ae884` plus the exact Task 2 WIP input hashes. Those bytes became commit `7e3b7e5` without alteration. Final case work took **7.671 seconds** in aggregate (0.347 + 6.830 + 0.494); the enclosing selection command took **8.118 seconds**. Node was the existing installed `v24.22.1`; no packages were installed. All reports are under `.phase4/runs/<invocation>/`, with `result.json`, input identities and separate evidence filenames.

The foundation case saves `inputs.json`, `source-spec.md`, `source-crosswalk.json`, `fields.json`, `negative-controls.json` and `valid-edit.json`. The items case additionally saves `H24-precision.json` and `legal-loadouts.json`. The discovery invocation retains its actual CLI-list/unknown-selection command reports and independent fixture controls. These are distinct run-owned files, not overwritten aggregate results.

Task 1 passed precommit `a013297e-0b0a-4ba9-a5af-51f2fef1b3e9`. Its mandatory same-case postcommit tracer **`c8f760cc-1d92-45d9-873f-8741cd95fd10` passed before any Task 2 expansion**. Earlier corrected foundation pass `2243399a-d453-4a96-9fdc-99b5092dc68b`, initial items pass `6059e434-d2d7-4a1e-90bf-27920a348acb`, and focused `494e2a13-b521-4c09-881f-e99affba5df1`, `53e92046-3a05-422b-99ac-2cf4ea86695f`, `64e793fc-2906-4006-af9e-e792cff684f7` remain history, not replacements for the final selection.

The parser/validator rejects duplicate JSON keys, invalid UTF-8/BOM, nonfinite numbers, wrong types, missing/unknown fields, fractional integer fields, invalid bounds, wrong ordinal IDs, duplicate supply/item/attack/projectile IDs, absent item/body/content/sound references, unsafe resource paths, absent/mismatched image bytes, wrong text keys, bad fit references, impossible stash footprints, incoherent caps/stacks/load/night relationships, armor condition mismatch, excessive nesting and illegal hands. Diagnostics carry exact **file, JSON-pointer field, offending value and reason**. H24 independently rejects rounding money's 0.001 kg default to a two-decimal display value and changing weight metadata to g/item. Serialized authored numbers remain separate from rounded display examples.

Independent valid edits prove that strict validation does not enforce hidden fallback balance: daylight 720 seconds, soft load 22 kg, money 0.002 kg/item, fists interval 0.8 seconds and stash 12×10 are accepted when validated separately from the shipped-baseline source oracle. Legal hand fixtures include fists, single knife, knife+bat, bow+arrows and arrows+bow; bow+knife, two bows, a wearable in a hand and an unknown item reject precisely.

Syntax and whitespace checks passed. No C# or compiled source changed, so the plan's conditional strict solution build does not apply. Unchanged native UI/hub/LAN/audio/export suites were not selected for this data slice; no pass/skip credit is inferred for them. Whole-phase active Critical/new/changed/affected rendered selection remains a later phase-end responsibility, after complete implementation and independent review/fixes.

## Artifacts this phase produces

### Exact implementation files and bytes

| File | Bytes | SHA256 |
| --- | --- | --- |
| src/godot-game/content/config/run.json | 2,131 | `b054a22b09553e3e6e27df8ccb235c6589145b2e945a1cd99e5d228f4b166ba2` |
| src/godot-game/content/config/time.json | 678 | `5b965846cba5fb759e19fa6d13325f314131a8dbe01a28a3a80aade7f01821b9` |
| src/godot-game/content/config/survivors.json | 1,799 | `abf2ed4abe643a1283865094f4768f0c4a1f41ad7fba7f1f135409f4cd3f77dc` |
| src/godot-game/content/schema/fields-foundation.json | 146,960 | `c3a059aeb1204a3d9cecd19acfce837cda503f6be9633b1c6a02b2de76178230` |
| src/godot-game/content/config/items.json | 15,550 | `fa9bb09bfc3a999dd21349274f5010e5a653a868d4a0d0be73a46caaef226720` |
| src/godot-game/content/config/combat.json | 4,135 | `c6610cc69731cd196490c26e381d92b7508f6487b96714bef8c4e96c2a478145` |
| src/godot-game/content/config/inventory.json | 2,551 | `4cff67d917744dfd1051ee4a827f34744ea42b8dff5ac84c695a5e0ba1cc0035` |
| src/godot-game/content/schema/fields-items-combat.json | 640,768 | `016dbd2ecf53a40cfcdbd0fdf08ead97d271c1728b106ecae93ac1cd853bbc49` |
| tools/phase4/cases/data-foundation.cjs | 52,875 | `d994597629cb53223927759b57dd70c1cd31e5353c7fc788270eaa9cca1b26c2` |

### Exhaustive field identities, metadata and constraints

The exact machine-readable field-ID inventories are the complete `files[].fields[]` arrays in the two SHA-bound schema files above, also copied in each final invocation's `fields.json`. These arrays enumerate **every actual field**, including structural objects/arrays and every authored array entry; they are not hand-picked count assertions. Each row contains exactly named `id`, `path`, `type`, `meaning`, `unit`, literal `default`, `allowed`, `dependencies`, `source` and `referenceKind` responsibilities. `source` contains document, exact section and source evidence; Task 2 item/attack/armor rows carry their actual table row, while inventory relationships carry source table/paragraph evidence. The metadata itself is checked for required fields and duplicate paths, defaults match actual data, source bytes/sections authenticate, and critical numeric units are independently checked.

The exhaustive deterministic identity rule is `<domain>` plus each exact JSON-pointer segment joined with `.`; the document root ends in `.`. Thus `items.definitions.0.weightKilograms` identifies Money's exact weight field, not a wildcard or a fallback. Array positions are explicit ordinal indices. The concrete schema inventory partition is run 101, time 25, survivors 76, items 546, combat 151 and inventory 85, **984 total**. Every concrete ID/default/unit/constraint/dependency/reference value is present verbatim in those hash-bound arrays.

The field families exhaust the actual authored trees:

- `run.` has all named setup/cap/map/day/phase/site/outfit/supply fields, explicit `startingOutfit.0`–`.2`, `initialStash.0`–`.8` and each stack's `item/quantity/stackQuantity/stacks/condition/contents`, plus all succession and outcome keys described above.
- `time.` has every timing/grace/stage/event policy key and `dawnEvents.0`–`.5` in their actual order.
- `survivors.` has every attribute/recovery/safety key, all `safeRest` and `load` keys, `appearance.pack/bodyIds.0/outfitRefs.0`–`.2/crown`, and every exact local `controls` key including alternative movement and quick-food indices.
- `items.` has `id/version/definitions`; `definitions.0`–`.23` each contains `id/name/nameKey/descriptionKey/footprint.0/footprint.1/maxStack/weightKilograms/rotatable/slot/storage/conditionMaximum/appearance/handlingSound/breakSound`. Only actual storage-bearing records have `storage.0/.1`. Every `appearance` has `pack/item/canonicalIcon/fitFile/fitItem`.
- `combat.` has every key described in DT-05: `attacks.0`–`.6` each with `attackId/itemRef/damage/intervalSeconds/reachUnits/staminaCost/projectile/releaseSound/impactSound`; `projectiles.0/.1` each with `projectileId/speedUnitsPerSecond/lifetimeSeconds/targetRangeUnits/stopAtTerrain/stopAtFirstEligibleHit/alliedPassThrough/recoverable/areaDamage/lingeringEffect`; `armor.0`–`.2` each with `itemRef/reductionFraction/maximumDurability/rawDamageWearFraction`; and all reduction/modifier/durability/hand/contact/acid fields.
- `inventory.` has every stash/placement, nesting, quick-slot, range, hauling/priority, cleanup, legacy, inspection-obligation, identity/access/privacy/drag/number/condition field described in DT-06, including actual ordinal entries of each list.

Validation requires exact documented types, safe integer count/grid fields, positive durations/weights/dimensions, finite numbers, bounded arrays, declared choices, no unknown/missing fields and explicit non-applicability. Quantities/caps have finite authoring bounds; supported players are at most four. Grid dimensions are additionally capped at 256, nesting at 16, fractions at their coherent ranges and local curve/cap/stack/condition relationships validate. No default is inserted into a missing document. Readers are bounded to 1 MiB, 100,000 parsed values and depth 16; portable canonical containment, ordinal paths and reparse/hardlink rejection use the existing content boundary. The only new subprocess is a bounded ten-second read-only `git show` of the pinned source.

### Resource references and existing dependencies

Config IDs are exactly **DT-01, DT-02, DT-03, DT-04, DT-05, DT-06**; schema IDs are **fields-foundation** and **fields-items-combat**, all version 1. Canonical item definitions are exactly **IT-01–IT-24** in source-table order.

Existing resource references are `content/packs/items-01-10.json`, `content/packs/items-11-24.json`, `content/packs/bodies-a.json` and, for applicable items, `content/animation/fits-a.json`. The two actual atlas dependencies are `assets/equipment/items-01-10.png` and `assets/equipment/items-11-24.png`; actual bytes are hashed against their pack metadata. Every item appearance points back to its identical IT ID. Canonical icon part IDs are exactly `IT_01_master` through `IT_24_master`, one per item, with actual part lookup and atlas identity checks. Exact text keys are `item.IT-01.name` through `item.IT-24.name` and corresponding `.description` keys, validated against actual item-pack records. These are text-key bindings awaiting TXT catalog production, not proof of completed translated descriptions.

Existing fit-item references are IT-02–IT-21 plus IT-23/24; IT-01 and IT-22 deliberately have no worn/held fit. Survivor appearance references only CH-01. `EN-01` and UI-05/06/13/14/15 are declared content obligations from the independent expected register. Sound references are declared AU-03/04/05/06/07/08/18 obligations for swing/hit/break/release/impact/spit/handling as appropriate. AU references are validated as exact obligation identities, not silently treated as produced sound payloads or listening evidence.

The schemas distinguish reference kinds `none`, `item`, `body`, `resource`, `image`, `text`, `content-obligation` and `sound-obligation`. Generic reference checks and item-specific image/text/appearance/fit checks are separate. Missing/wrong references report precise file/field/value/reason. No pack-index/current hub registration was edited. No new textures/scenes, retained donor/generated bundles, recipe bundle members, C# types/members, native resource/subresource IDs, generated permissions or native-policy tuples were created. Existing donor assets were read only. The nine new implementation paths were already in the formal phase inventory, whose distinct union remains **234**.

### Executable exports and complete symbol inventory

The module exports exactly these constants/metadata and reusable functions: `BASE`, `SOURCE_REVISION`, `FOUNDATION`, `fault(file,field,value,reason)`, `check(ok,file,field,value,reason)`, `strictParse(raw,file)`, `read(file)`, `source()`, `section(text,id)`, `row(text,id,label)`, `numbers(text)`, `capture(text,pattern)`, `roster(text)`, `referenceSets()`, `describeFields(schema)`, `validateDocument(file,data,definition,refs)`, `validateFoundation(docs,refs)`, `foundationSource(docs,text)`, `loadDomains(names,schemaName)`, `validateSet(docs,schema,refs)`, `schemaEvidence(schema,docs,text)`, `negative(controls,name,file,field,reason,action)`, `foundation(context)`, `inputs(names,schema)`, `metadata`, `ITEMS_COMBAT`, `legalLoadout(left,right,definitions)`, `itemReferences(item,index,refs,load=content.bytes loader)`, `validateItemsCombat(docs,refs)`, `itemsSource(docs,text)` and `itemsCombat(context)`.

Nested named helpers are strict parser `string(p)`/`value(p,depth)`, field walker `walk(v,p,depth)`, source oracle `exact(domain,p,value,expected,source)` and `policy(domain,p,value,expected,id,quote)`, and items fixture `invalid(name,domain,p,value,reason,semantic=false)`. Local arrow helpers include whitespace scanning, foundation relationship failure/eq/bad mutations, source exact comparison and immutable fingerprint construction. No helper executes a case at module import. The only import-time effects are registering `data.foundation` and `data.items-combat`; no cross-case module is eagerly imported to execute production or tests.

Both cases are Critical/no-window with requirement/acceptance identities, setup, ordered actions, expected results, reset/cleanup, evidence and persistent run-group metadata. They use **G0-data/order40** and **G0-data/order41**. H24 authored-unit/default precision and UI-TEXT-STATUS/error diagnostic backstops are explicit, with later rendered checkpoints `data-formatting` and `data-errors`; they supply no rendered acceptance. D-27 remains preview-only pause/speed/step ownership; `activeGamePauses:false` preserves the gameplay no-pause contract.

## Deviations, original failures and remaining acceptance

**[Rule 1 — Parser capacity correction, Task 1]** Initial invocation `a6f96d5e-3385-4e66-bc1e-2b7db917d4a1` failed at the existing wearable pack's `/acceptance/owner` after 10,001 values. The first limit of 10,000 was too small for that existing 232-KiB dependency. It was raised to a still-bounded 100,000 parsed values, retaining 1-MiB byte/depth/path/reference controls. The corrected original/final foundation invocations pass. That original failure remains failed, not converted into a RED or a pass. No source-policy permission/test was relaxed.

The initial local authoring subprocess encountered managed sandbox `spawnSync git EPERM`; the equivalent bounded local command ran with escalation and the environment failure supplies no assertion credit. There was no package install, auth gate, branch switch, push, stash/reset/rebase, donor write or foreign-file staging. No task needed an additional tracked implementation path.

No implementation-blocking stub, skipped test or unrun in-scope verification remains. Legitimate null/non-applicable item fields and declared future catalog/map/inspection obligations are explicitly typed, source-owned references, not completed payload claims. There is no new network/auth/trust boundary beyond the planned bounded local file/parser/evidence surface.

All ASSET requirements remain Pending. Current resource adoption and full hub/data presentation remain 04-23-owned; packaged producer/source closure remains 04-31-owned. **WINDOWS29/30/31 remain open:** inherited current-hub tracer/index/provenance binding, actual new native adapter permissions and the current producer hash/path cohort must be reconciled before independent 04-24 review and actual 04-32/33 acceptance. The existing `engine.source-policy` and generated-boundary failures are unchanged. Passing this plan's data source/crosswalk case is not an engine source-policy pass.

No actual gameplay transaction, movement/combat execution, real game window, physical input/display, sound audition/listening, appearance/readability judgment or final package acceptance was performed. Map, full body roster, complete TXT/audio production and final integration retain their scheduled owners. Current index remains 180 expected obligations, one partial bow, 28 pending shards and zero completed obligations. Actual Windows1280×720/no-VM acceptance follows full phase implementation and independent review/fixes; listener remains null.

Parent owns shared continuation/PR5/push and global progress reconciliation after this committed summary. Record Phase4 local progress **14/37→15/37**, no requirement completion, inventory234 unchanged, wave12 complete and next wave13 plans04-07/04-28. Foreign PROJECT/REQUIREMENTS/ROADMAP/STATE, initial-spec, Phase06 specification/checkpoint and untracked Phase12 work stay unstaged and preserved.

## Self-Check: PASSED

All nine implementation files exist and their working and committed SHA256 identities exactly match the frozen selection. Walking every actual data object/array/scalar produces the identical ordered 984 concrete paths and field IDs in the explicit schema inventories. Both task commits are ancestors of the recorded implementation HEAD. All three fresh final result files report pass, and the frozen-run-to-commit mapping is unchanged. The canonical summary exists, whitespace checks pass, no tracked deletion or unexpected generated untracked output remains and the index is empty before staging this summary. Foreign global/spec/Phase06/Phase12 work remains unstaged and preserved. Only this summary is committed in the immediate metadata transaction; parent reconciles shared progress afterward.
