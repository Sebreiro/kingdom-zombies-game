---
phase: 04-asset-production
plan: "05"
subsystem: item-master-and-component-production
tags: [PNG, Godot, imagegen, provenance, reusable-components, source-headless]
status: complete
requires:
  - phase: 04-04
    provides: SHA-bound common donor kit and selected one-art-pixel-per-world-pixel scale
  - phase: 04-26
    provides: Existing inspection lifecycle and saved-case discovery foundation
provides:
  - All24 canonical item identities with88 actual art parts and166 reusable component inputs
  - Twenty original built-in generated masters, exact prompts, reference inputs and reproducible pixel transformations
  - Two callable scriptless native selection/state/zoom/scroll preview scenes
  - Saved items.weapons-food and items.wearables cases with independent semantic and native negative controls
affects: [04-06, 04-07, 04-09, 04-14, 04-15, 04-18, 04-23, 04-27, 04-30, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [retained-generated-originals, canonical-item-icons, independent-modular-crops, native-scriptless-selection, physical-scroll-reachability]
key-files:
  created:
    - src/godot-game/assets/equipment/items-01-10.png
    - src/godot-game/content/packs/items-01-10.json
    - production/phase4/items-01-10.zip
    - tools/phase4/cases/items.cjs
    - src/godot-game/inspection/ItemsWeaponsPreview.tscn
    - src/godot-game/assets/equipment/items-11-24.png
    - src/godot-game/content/packs/items-11-24.json
    - production/phase4/items-11-24.zip
    - src/godot-game/inspection/ItemsWearablesPreview.tscn
    - .planning/phases/04-asset-production/04-05-AMENDMENTS.md
  modified: []
key-decisions:
  - "Keep one canonical icon per item; held/corpse/wearable inputs and conditional overlays remain separate components."
  - "CurrentD-13 simple donor-like logical detail overrides the older sprite skill's rich-detail preference; one art pixel remains one world pixel."
  - "Use scriptless Button-to-AnimationPlayer selection/zoom/state resources with independently verified physical scroll reachability."
  - "Keep the small pouch stored-only, bags hidden when nested, and clothing/armor/backpack independently removable."
requirements-completed: []
requirements-contributed: [ASSET-01, ASSET-02, ASSET-03]
coverage:
  - id: item-source-masters
    description: Exact24-item source roster, original masters, canonical icons and applicable component inputs
    requirement: ASSET-02
    verification:
      - kind: integration
        ref: .phase4/runs/11bfbdd3-91a5-4afa-afb9-8bfa481b3525/result.json
        status: pass
      - kind: integration
        ref: .phase4/runs/92975263-3670-44e9-8afb-2bfe4e25a486/result.json
        status: pass
    human_judgment: true
    rationale: Source/native inputs are complete; actual appearance, later body fits and final acceptance remain separately owned.
  - id: item-native-preview
    description: Actual scriptless item/state selection, compact identity/status, exact visible pixels and1/2/4/8x scroll reachability
    requirement: ASSET-03
    verification:
      - kind: integration
        ref: .phase4/runs/11bfbdd3-91a5-4afa-afb9-8bfa481b3525/result.json
        status: pass
      - kind: integration
        ref: .phase4/runs/92975263-3670-44e9-8afb-2bfe4e25a486/result.json
        status: pass
    human_judgment: true
    rationale: Callable/headless scenes do not establish current full-hub dispatch or actual rendered appearance acceptance.
actuals:
  tokens: 166014
  tasks: 2
  commits: 3
commits: 3
plan_head_before: 1271c2f306d8d3dc694babb367b4c08b23e22491
plan_head_after: 23fe7cef373db0a3b5455c5b101ed258a7e2b98b
duration: 48min
completed: 2026-11-17
---

# Phase04 Plan05: All item identities and reusable equipment masters Summary

**Twenty original generated masters plus selected donor pixels deliver all24 canonical items,88 concrete art parts,166 reusable component inputs and two actual native item previews.**

## Completion boundary and measured cost

Both tasks are complete on`feature/04-asset-production`, under the parent's existing draftPR5 targeting dev. No installation, branch switch, stash/reset/rebase/merge, real game window, VM, listening or final appearance judgment occurred. No C# or policy source changed. This source/headless completion does not complete an ASSET requirement or any of the180 final obligations.

The measured production/verification window is2026-11-17T00:01:07Z–00:48:46Z,47min39sec, rounded48min. Initial context loading and this documentation close-out lie outside that measured interval. `.phase4/04-05/completion-metrics.json` retains the measurement and fresh dependency frontier. The persisted Git ledger measures exactly3 implementation commits from1271c2f to23fe7ce. Ordinary unified diff text across the nine implementation paths plus the approved amendment contains664,053 characters;`ceil(chars/4)` is166,014. Binary notices, not binary patch expansion or harness tokens, are included on that same scale. The summary commit is outside the before/after snapshot.

## Accomplishments

- IT-01–IT-10 have exact money/arrow/weapon/food identities. Reuse K20 coin7856, arrow6885 and bow7008; compose a compact purple berry cluster from K04 sprite11340. The bundle arrow icon is a10×7 composition, separate from the8×4 single nocked/flying/held arrow input. The four melee weapons have different actual silhouettes and grips. Food-use inputs reuse their actual item masters; meat has one definition, with no invented cooking system.
- Generated knife, club, machete, metal bat, meat and a readable complete fruit. Inspected K05 sprite8203 contains flattened13×3/13×4 orchard fragments; its exact PNG/record remains retained as the fruit adaptation reference. Generated material is not labeled unchanged donor reuse.
- IT-11–IT-24 have distinct canonical icons and actual removable garment/head/face/hand/foot/armor/belt/container inputs. Shirt/coat torso and sleeve segments, trouser waist/legs, glove hands, boot feet, armor torso/straps and backpack body/straps/pockets are separately cropped and anchored. Every applicable live component has its corresponding reusable corpse component. Armor and bags are not baked into coats. These are pose-fitting inputs, not a separately baked equipped-character atlas.
- K14 sprites7848 and7052 were inspected at enlarged native pixels and retained with exact records. Their cloth is baked into tiny actors; they are source/palette ingredients rather than falsely claimed removable garment masters. Fourteen missing complete wearable/container masters were generated. Front/rear variants can intentionally reuse the same cropped art where appropriate; their attachment identities remain distinct.
- Storage diagrams are actual native Label resources generated from exact grids: shirt2×1, trousers2×2, coat2×2, belt4×2, pouch2×2, small backpack4×4, field backpack5×6. Small pouch has no worn slot or worn component. Nested bags do not acquire an extra worn appearance. Cap/helmet retain a separate crown anchor; face covering preserves the later mouth/release anchor. No ordinary clothing durability mechanic is invented. Numeric weights/combat/balance remain DT-04 ownership.
- Both scriptless scenes expose real item Buttons connected to native AnimationPlayer selection, separate state examples, zoom1/2/4/8, a clipped680×320 art scroll and compact exact identity/name/source status outside that scroll. Selection hides previous groups and conditional overlays. The native source fixture explicitly sets its logical root to1280×720; it does not open or certify a real game window.

## Task and repair commits

1. Task1:`dbfff50` — `feat(04-05): produce ten item masters and callable weapon previews`.
2. Separately authorized same-plan repair:`827ac6d` — `fix(04-05): prove every item component reachable through native zoom scroll`.
3. Task2:`23fe7ce` — `feat(04-05): produce fourteen removable wearable and container masters`.

Task1 and Task2 each have five implementation paths under the prospective two-scene amendment. The repair touches four existing Task1 implementation paths and its authority note. No implementation path was added beyond the approved nine-path union. The original pre-scroll scene and original failing/passing reports remain intact. No tracked deletion occurred.

## Verification and immutable evidence

All invocations below live under`.phase4/runs/<invocation>/`, retaining actual revision, input hashes, bounded process/PID/argument/environment identities, command stdout/stderr and final result. No source edit occurred during an accepted canonical invocation.

| Case/gate | Invocation | Tested revision | Result/duration |
| --- | --- | --- | --- |
| Task1 final precommit | 31b2b03f-5f26-4311-933c-fdd4e53d3b81 | 1271c2f plus captured Task1 inputs | PASS,6,680ms;15 semantic controls and actual selection/zoom/pixels; later scroll gap explicitly not covered |
| Required post-Task1 tracer | 2e45ae7a-89f0-4f53-92b3-977512e3815e | dbfff50 | PASS,6,704ms before Task2 expansion |
| Bounded scroll repair first attempt | d8bcf6ab-4669-4d42-9d3f-c45a9113f8d2 | dbfff50 plus repair inputs | FAIL,`LOGICAL_VIEWPORT`; preserved |
| Bounded scroll repair final attempt | 6abe66e2-b5d1-4518-ba7d-f9a6a3a75598 | dbfff50 plus captured repair inputs | PASS,7,764ms;408 physical component reachability checks |
| Required post-repair tracer | a5ca6a21-9f25-4c0c-acfe-2e6c05572551 | 827ac6d | PASS,7,805ms on frozen repair inputs |
| Task2 final precommit | 4c9feb52-4888-4682-90da-3199c39d19b3 | 827ac6d plus captured Task2 inputs | PASS,13,227ms;23 semantic and2 native rejections |
| Final items.weapons-food | 11bfbdd3-91a5-4afa-afb9-8bfa481b3525 | 23fe7ce | PASS,8,340ms |
| Final items.wearables | 92975263-3670-44e9-8afb-2bfe4e25a486 | 23fe7ce | PASS,13,614ms |
| Final source.case-discovery | fed3ec47-da1d-4b10-b983-d087a4e422c6 | 23fe7ce | PASS,529ms;17 current cases,35 rejected discovery faults,4 actual CLI children, zero case bodies executed during import |

The final three-case selection was declared at`.phase4/04-05/selection.json`. Independent cases ran against the same finished23fe7ce source, with no owned implementation WIP. Sum of per-case durations is22,483ms; those operations overlapped and are not claimed as elapsed wall time. The final weapon input manifest has8 entries/SHA256`ebf21870724b419872f5dd6e97ffe2e7c9a95a3fea3bd25384924a500e321f56`; wearable11 entries/`24e00f1e3c06899d5839908a14dcbbb630b4eb1213c1d58f550d789b79bd5cd9`; discovery3 entries/`6cdf6abf2fc14f6572af6bf0593d9122b75bae8d2db1902dcf932796027ef2f4`. These identify the exact retained`inputs.json` bytes, rather than inventing a global WIP fingerprint.

Final weapon observations:10 items,17 parts,45 component inputs,6 generated originals,58 ZIP members;102 actual native sprite observations including overlays,80 zoom transitions and408 physical reachability checks. Final wearable observations:14 items,71 parts,121 inputs,14 originals,127 members;248 native sprite observations,112 zoom transitions and992 reachability checks. Every item is selected twice. Every component is physically reachable through valid native scrollbar ranges at every zoom. Identity, source status, storage diagram text, exact alpha/visible RGB, separate AtlasTexture regions, overlay exclusivity and previous-selection retirement are observed. Godot's RGB repair underneath alpha0 is normalized only for native comparison; every alpha and visible RGB remains exact.

Seventeen weapon semantic controls reject missing masters/prompts, duplicate roster identities, condition-icon proliferation, wrong grip/logical hand, bundle/projectile alias, invalid ammo predicate, absent component, smoothing, wrong zoom, script-bearing/linear scenes, false acceptance, stale route, excess operations and excess modes. Twenty-three wearable controls additionally cover wrong grids/slots, baked gear, worn pouch, nested bag, wrong glove hand, invented clothing condition, absent corpse/strap, hidden crown and blocked mouth. Pixel/alpha/crop/nearest reproduction runs from the actual retained sources, with source-byte/operation-keyed caches only; semantic authorization is rechecked on every mutation.

Both suites reject a real scene-filter mutation through a fresh native process. Weapon reachability additionally rejects the exact original pre-scroll scene, SHA256`e6657e1e02bb4977a8f9f23b8c5baab7a7cd9210f88db07e20ddf6b44a51a1ae`. Wearable reachability rejects an actual non-scroll-container shape, SHA256`23b045d3089c8bd4dfb74a3812d887276e82c214be8cb38f2976838d2e0b12d6`. These are physical resource/observer failures, not metadata flags.

No compiled source changed, so the plan's conditional strict solution build does not apply. Unchanged audio/LAN/export/hub/full-catalog wrappers were not selected for this slice. No pass/skip credit is assigned to actual rendering/listening/judgment. The known inherited `source.provenance` failure was not repeatedly rerun or repaired outside ownership.

## Artifacts this phase produces

### Exact files and current byte identities

| Path | Actual bytes / SHA256 |
| --- | --- |
| src/godot-game/assets/equipment/items-01-10.png | 2,296;`3d8b8e3d6aa0a2e94dc3fba8b64cc54573b62031be257db76c79fa2cac53cfe9` |
| src/godot-game/content/packs/items-01-10.json | 84,190;`9402295cc6457d42d53a7b4a8ada19bc06711d6db8f600386f49b05a238686b7` |
| production/phase4/items-01-10.zip | 3,122,690;`79ea6e5de7b45c463a1f2b31917dedc9f8e596b074ef7ed4effe57b4dcdbad59` |
| src/godot-game/inspection/ItemsWeaponsPreview.tscn | 109,764;`a0b6e6306cd461efbecebfe5e715763b916a6988f1558451f98407f0ed8c79e4` |
| src/godot-game/assets/equipment/items-11-24.png | 6,603;`b5dafe0a20348a9b667e9eda74c4250fdcbf2ce2fffac38ab7193b19059a4219` |
| src/godot-game/content/packs/items-11-24.json | 232,442;`78761a40c0e0b7a22ef9885ee847338fd7315ce69014756608a0bc965fe0a826` |
| production/phase4/items-11-24.zip | 15,845,426;`ef4cdcb6a9736a52864d6a4e807ab172805d293bf5b9cd88c485b4280dd681a6` |
| src/godot-game/inspection/ItemsWearablesPreview.tscn | 173,593;`402f04ae90dbad7817809f5ed79eaa2e49d0aa6e1d955f0d918f93fe167e3331` |
| tools/phase4/cases/items.cjs | Actual saved-case implementation; exact bytes are authenticated by both final invocation input manifests above |

Both lossless RGBA atlases are512×256. Visible pixels are446 and2,245 respectively; native alpha is binary and transparent RGB is zero. Atlas size is packaging, not world size. All item/part world sizes equal their native exports at density1; inspection zoom is separate.

### Executable symbols and complete native resource identities

CommonJS functions/signatures are`engine()`, `files(kind,scene)`, `fingerprint(paths)`, `probe(nonce,kind,scene)`, `native(context,kind,scene)` and`run(context,kind,scene)`. Exports are`VERIFY`, `PYTHON` and those six functions. Registered case IDs are exactly`items.weapons-food`(G0/order22) and`items.wearables`(G0/order23), both Critical/no-window. The embedded trusted Python verifier defines`decoded(raw)`, `reproduced(part,data)`, `binary(im)`, `reproduce(part,data)` and`verify(p,data)` with exact independent names, footprints, slots/storage, component names and fault oracles. Generated owned native proof scripts define only`reject(code)`, `_initialize()` and`inspect()`.

The retained authoring recipe defines`sha(b)`, `js(x)`, `png(im)`, `info(im)`, `clean(im)`, `quoted(v)`, `track(index,target,value)`, `scene(pack)`, `build(which)`, nested`library(id,rows)`, `node(name,type,parent=None,props='')`, `part(key,im,origin,ops)` and`component(name,key=masterkey,rotation=0,attachment=None,pivot=None)`. It retains the actual authoring process and crop/slot/storage constants. Authoring source locations are provenance; fresh saved checks reproduce the pixel transformations from retained masters/pack operations without the generated-image cache or donor root. The authoring workspace's generation-input manifests are not a runtime dependency.

There are **zero new C# types/members, generated permissions or production native-policy tuples**. Static scenes instantiate existing native resource/node classes. No tuple metadata authorizes engine access, and no donor callback is executed.

The exhaustive scene inventory is defined by the two SHA-bound scene files plus the actual ordered`items`, `parts`, `components` and`modes` arrays in the SHA-bound pack documents. The following exact construction rules enumerate every identifier without substituting a count-only claim:

- External resource in each scene:`1_items`. Each actual`parts[].key` creates exactly one AtlasTexture ID`A_<key>` with its exact atlasRect;17 in weapons,71 in wearables. Weapons part keys are`IT_01_master`, `IT_02_master`, `IT_02_bundle`, `IT_03_master`, `IT_03_broken`, `IT_04_master`, `IT_04_broken`, `IT_05_master`, `IT_05_broken`, `IT_06_master`, `IT_06_broken`, `IT_07_master`, `IT_07_broken`, `IT_07_nocked`, `IT_08_master`, `IT_09_master`, `IT_10_master`. Wearable keys are each`IT_nn_master`, every exact live component name with hyphens replaced by underscores, and`IT_18_broken`/`IT_19_broken`/`IT_20_broken`; all literal names/bytes are present in the127-member inventory.
- Selection Animation IDs:`Select_IT_01`–`Select_IT_10` and`Select_IT_11`–`Select_IT_24`; their default-library animation names are the exact corresponding`IT-01`–`IT-24`. State IDs are`State_IT_03_Broken`–`State_IT_07_Broken`, `State_IT_07_Nocked`, and`State_IT_18_Broken`/`State_IT_19_Broken`/`State_IT_20_Broken`; public animation keys are`<IT-ID>.<mode.node>`. Both scenes have`Zoom_1`, `Zoom_2`, `Zoom_4`, `Zoom_8` with keys`zoom1/zoom2/zoom4/zoom8`, and AnimationLibrary IDs`Selections`, `States`, `Zooms`.
- Exact roots:`ItemsWeaponsPreview`/`ItemsWearablesPreview`. Shared root children:`Title`, `Browse`, `Zoom`, `Modes`, `Groups`, `Selection`, `State`, `ZoomPlayer`. `Browse/List` contains exactly`Select_<item.node>` for each pack item. `Zoom` contains`Zoom1/Zoom2/Zoom4/Zoom8`.
- `Groups/<item.node>` contains`Identity`, `Use`, `ArtScroll`, `Components`; only storage-bearing items also have`Storage`, and only declared modes create`State_<mode.node>` Buttons. `ArtScroll/Canvas/Art` contains exactly each actual`components[].node`, each declared`modes[].node`, and`Caption_0` through`Caption_<componentCount-1>`. Those records exhaust all paths, including exact logical hand/foot and corpse identities. Weapons have196 nodes/40 subresources; wearables381/95. Identity/status and state controls remain outside art scrolling.

The native API/animation design was checked against official Godot [Animation](https://docs.godotengine.org/en/stable/classes/class_animation.html) and [AnimationPlayer](https://docs.godotengine.org/en/stable/classes/class_animationplayer.html) documentation after Context7/ctx7 were unavailable. The actual pinned4.7.2 process supplies implementation evidence.

### Exhaustive retained ZIP inventory

Canonical exact member-name/byte/SHA256 inventories are each ZIP's`members.json`, authenticating every other member. Their own identities close the inventory without recursive self-hashing:

| ZIP | Inventory bytes/SHA256 | Exact member count |
| --- | --- | --- |
| items-01-10.zip | 8,322;`91e172c5a0561b0fa74778f04d4530c772f99b316463da3638f600d26305dff5` | 57 rows plus inventory =58 |
| items-11-24.zip | 18,727;`cfeffb64a21f82a4e29d9eab6b8273ba4127db7174dcd69e7187afe2c29f1e96` | 126 rows plus inventory =127 |

Together the185 retained members comprise20 original`masters/<name>.png`,20 exact`prompts/<name>.txt`,20 full-size cleaned`cleaned/<name>.png`,88 actual`parts/<key>.png`,16 donor PNG/JSON records,8 reference PNGs,6 top-level current pack/atlas/scene members,2 authoring recipes,2 source proof boards,1 original historical scene and2 inventories. This inventory explicitly distinguishes retained sources from runtime atlases.

Master/prompt/cleaned names are exactly`knife`, `club`, `machete`, `bat`, `fruit`, `meat` in the first bundle and`shirt`, `trousers`, `coat`, `cap`, `face`, `gloves`, `boots`, `helmet`, `improvised`, `plated`, `belt`, `pouch`, `smallpack`, `fieldpack` in the second. Every parts filename is the corresponding literal`parts[].member`, with no invented alias outside that array. Both bundles contain`recipes/build-items.py`, `proofs/icons-nearest3x.png`, `references/references.png`, `references/survivor.png`, `references/donor-survivor.png`, `references/donor-wall.png`. First top-level members are`items-01-10.json`, `items-01-10.png`, `ItemsWeaponsPreview.tscn`, plus`history/pre-scroll.tscn`; second are`items-11-24.json`, `items-11-24.png`, `ItemsWearablesPreview.tscn`.

First donor pairs are`donors/common.r.11340.{json,png}`, `common.r.8203.{json,png}`, `common.s0.6885.{json,png}`, `common.s0.7008.{json,png}`, `common.s0.7848.{json,png}`, `common.s0.7856.{json,png}` under that same donors directory. Second pairs are`donors/common.s0.7052.{json,png}` and`donors/common.s0.7848.{json,png}`. Original fractional donor registration is preserved in their records; selected raw objects/texture dependencies remain in the exact SHA-bound existing`common-selected.zip`. This summary does not claim those raw objects were duplicated into the item bundles.

### Generation, cleanup and separate dimensions

All20 original masters came from20 distinct built-in image_gen calls with`transparent_background=true`; no API-key/CLI fallback or package installation was used. Session identity is`01a1e6c7-acce-7781-a415-1d58fa0e2f3a`. Exact original output filename, original PNG SHA256, full prompt SHA256, original alpha metadata, source crop bounds and reference-member identities are each item's`source` record and the canonical inventory. The tracked ZIPs contain the original bytes; runtime does not resolve the global CODEX_HOME output location. All prompts explicitly preserve D-13's simple low-resolution precedence.

| Item/master | Original canvas | Native export/world dimensions |
| --- | --- | --- |
| IT-03 knife | 724×2172 | 4×12 |
| IT-04 club | 1088×1445 | 5×18 |
| IT-05 machete | 1983×793 | 5×20 |
| IT-06 bat | 1086×1448 | 4×24 |
| IT-09 fruit | 1263×1245 | 6×7 |
| IT-10 meat | 1484×1060 | 7×5 |
| IT-11 shirt | 1415×1111 | 14×11 whole garment; separate torso/sleeve crops |
| IT-12 trousers | 984×1599 | 8×13 whole pair; separate waist/leg crops |
| IT-13 coat | 1322×1190 | 16×18 whole garment; separate torso/hem/sleeves |
| IT-14 cap | 1659×948 | 7×4 |
| IT-15 face | 1619×971 | 5×3 |
| IT-16 gloves | 1586×992 | 8×5 whole pair; separate hands |
| IT-17 boots | 1619×971 | 10×6 whole pair; separate feet |
| IT-18 helmet | 1354×1161 | 7×6 |
| IT-19 improvised | 1070×1470 | 8×11 whole master; separate torso/straps |
| IT-20 plated | 1145×1374 | 10×12 whole master; separate torso/straps |
| IT-21 belt | 1983×793 | 10×4 whole assembly; separate strap/pouches |
| IT-22 pouch | 1060×1484 | 5×7 |
| IT-23 smallpack | 1070×1470 | 8×11 whole master; separate body/straps |
| IT-24 fieldpack | 1075×1464 | 11×15 whole master; separate body/pockets/straps |

Original partial-alpha masters are preserved. Cleanup thresholds alpha at128, zeros RGB only outside the binary mask, crops the exact recorded opaque bounds and uses nearest sampling to each explicitly recorded native size. Original canvas, opaque crop, native export, world size and inspection zoom are separate fields. The saved verifier reproduces every actual part from these operations; generated high-resolution originals are not represented as native-resolution exports. Source proof boards are assembled artwork, not screenshots or actual appearance approval.

## Deviations, failures and remaining ownership

1. **[Rule2 - Missing actual preview adapter]** The original seven-path plan required callable selection/zoom/status but supplied no item consumer resource. The prospective coordinator amendment added only the two exact scriptless scenes, making nine implementation paths and actual phase inventory232→234 after reconciliation. Authority is recorded in`04-05-AMENDMENTS.md`; it grants no Root/index/C# policy permission.
2. **[Rule1 - Saved-module discovery]** An initial direct require of`media.cjs` registered its cases before automatic discovery later loaded it, producing`CASE_MODULE_EMPTY`. The item module now authenticates its pinned engine directly, preserving automatic discovery. The initial process-sandbox`spawnSync git EPERM` was environment setup, not an assertion pass or RED.
3. **[Rule1 - Native pixel comparison]** Actual native invocation`4fade79c-8d84-480f-ad5b-ec8b0aaaea3c` failed`PIXELS` because Godot fills RGB under transparent alpha. Native comparison now clears only those invisible RGB bytes before hashing, while exact alpha/visible RGB and source zero-background checks remain required. Later original Task1 and final full-selection passes establish the corrected behavior.
4. **[Rule2 - Missing reachability coverage]** Original Task1/tracer checked selection/zoom values but did not prove large layouts reachable. The parent prospectively authorized the four-path same-plan repair with two attempts. Attempt1 failed`LOGICAL_VIEWPORT`: the headless root was100×100 despite authored/content scale1280×720. The owned diagnostic records actual100×100 and actual1280×720 after explicit sizing. Attempt2 passed, was committed, and its postcommit tracer passed. Both repair attempts were consumed; there is no remaining failure or unspent reset. Original overflow art/scene/proof remains preserved as a rejected regression fixture.
5. **[Rule2 - Bounded operation/mode inputs]** Task2 adds exact per-part operation and per-item mode bounds with independent controls; the final whole item/discovery selection passed on the finished source. Empty mode/storage/hidden-equipment arrays are intentional legal states, not data stubs.

All ASSET requirements remain Pending. Current-resource index adoption and full in-hub selection/status projection remain explicitly04-23-owned; these two callable scenes do not pretend the current hardcoded bow-only hub already dispatches all items. Body fits, articulated action rigs and exact bow draw/release/string behavior remain their04-06/07/09 source owners; reusable bow/arrow/grip inputs and action ownership are explicit. Actual Windows1280×720/no-VM rendering, appearance/provenance judgment, listening and final package acceptance remain04-32/33 after whole-phase implementation and independent review/fixes.

Existing WINDOWS29 and immutable inherited`source.provenance` failure`adb38f27-592b-4700-880c-3fa1d65d0c15` remain active04-23/31 integration ownership. This plan neither duplicates that entry nor refreshes stale tracer hashes. Pack-index still has one declared invalid partial bow,28 pending shards/zero completed obligations; these standalone outputs do not silently change that census. No new unresolved stub/skipped test/unrun in-scope verification or unmodeled network/auth boundary is introduced. The existing file/parser/media/native/evidence threat boundaries cover these bounded local resources.

Parent explicitly owns shared progress reconciliation. Foreign PROJECT/REQUIREMENTS/ROADMAP/STATE/spec/phase06/phase12 work remains unstaged and preserved; no global planning/spec write occurred here. After this immediately committed summary, record Phase04 progress10/37→11/37, no requirement completion, actual inventory234, and next earliest ready04-11(wave11). Wave12's04-06/14/18 have dependency prerequisites satisfied but wait for the parent's wave policy and04-11 serialization. No other plan or phase was executed.

## Self-Check: PASSED

All nine implementation artifacts, exact amendment and this canonical summary exist. dbfff50,827ac6d and23fe7ce are ancestors of recorded implementation HEAD. Both inventories close all185 retained members, original/master/prompt hashes and source/native transformations reconcile, and the fresh three-case final selection passed on23fe7ce. No tracked deletion, unexpected generated untracked output, implementation-blocking stub or unresolved in-scope automated failure remains. Global foreign work remains outside staging. Summary is committed immediately before progress narration or parent state reconciliation.
