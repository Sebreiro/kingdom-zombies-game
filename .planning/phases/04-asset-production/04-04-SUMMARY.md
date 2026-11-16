---
phase: 04-asset-production
plan: "04"
subsystem: donor-media-and-scale-production
tags: [UnityPy, Pillow, Godot, PNG, provenance, animation, imagegen, source-headless]
status: complete
requires:
  - phase: 04-26
    provides: Current guarded inspection foundation and saved-case discovery prerequisites
provides:
  - Portable selected common kit with725 sprites,35 clips,61 layers and594 authored keys
  - Four retained generated masters with expanded prompts and reproducible native exports
  - Scriptless day/night ScaleComparison resource scene with actual donor/generated artwork
  - Saved media.conversion and media.scale cases with independent semantic/native negative controls
affects: [04-05, 04-06, 04-11, 04-23, 04-30, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [read-only-selected-donor-export, retained-bounded-ZIP, original-registration, inert-callback-data, nearest-pixel-normalization]
key-files:
  created:
    - tools/phase4/extract-media.py
    - tools/phase4/cases/media.cjs
    - src/godot-game/assets/common/donor-kit.png
    - src/godot-game/content/packs/common.json
    - production/phase4/common-selected.zip
    - src/godot-game/assets/common/scale-pilot.png
    - src/godot-game/content/packs/scale-pilot.json
    - src/godot-game/inspection/ScaleComparison.tscn
    - production/phase4/scale-pilot.zip
  modified:
    - .planning/phases/04-asset-production/deferred-items.md
key-decisions:
  - "Select one art pixel per world pixel; survivor30px, wide zombie34px, native donor walls35/43px and tower48px; inspection zoom3 is separate."
  - "CurrentD-13 simpler donor detail overrides older richer-density sprite references; original generated masters remain intact."
  - "Preserve original fractional logical/trim/pivot registration in common records; explicitly normalize visible bottoms only in the pilot, snapping odd-width anchors to integer pixels."
  - "Keep common and scale-pilot shards pending in the current index until04-23 adopts complete current outputs; final actual visual acceptance belongs to04-32/33."
requirements-completed: []
requirements-contributed: [ASSET-01, ASSET-02, ASSET-03]
coverage:
  - id: common-conversion
    description: Exact retained donor/source registration and all selected runtime animation layers/keys
    requirement: ASSET-01
    verification:
      - kind: integration
        ref: .phase4/runs/dbb25049-0eb8-485c-bc24-91ccd1fb473c/result.json
        status: pass
    human_judgment: false
  - id: scale-source-proof
    description: Four generated inputs, actual donor artwork, reproducible density choice and native resource scene
    requirement: ASSET-03
    verification:
      - kind: integration
        ref: .phase4/runs/4cd8c68c-c1f3-475f-af0d-5bc2c8dd86a3/result.json
        status: pass
    human_judgment: true
    rationale: Source-side production choice is complete; final actual in-game appearance/readability judgment remains pending04-32.
actuals:
  tokens: 474001
  tasks: 2
  commits: 2
commits: 2
plan_head_before: 5533fff01fde4f30e75e37d581b8a002c2db1afd
plan_head_after: 1ffe80752bd35547846999168efa79329f18d4da
duration: 36min
completed: 2026-11-17
---

# Phase04 Plan04: Reusable donor kit and assembled scale proof Summary

**725 real donor sprites and35 exact runtime clips, plus four original generated pilots assembled with actual wood/tower/base donors at one reproducible native pixel density.**

## Performance and completion boundary

Both formal tasks are complete on `feature/04-asset-production`, associated with the parent's existing draft PR5 targeting dev. No branch switch, installation, VM, game window, listening, independent phase review or actual rendered acceptance occurred. The phase remains incomplete and the inherited provenance failure below remains an active phase-completion blocker.

The retained authoring window starts with `.phase4/04-04` creation at2026-11-16T22:47:47Z and closes around23:24Z, approximately36min. The final four-case source selection took52,329ms in its scenario invocations: three passes and one inherited failure. Source generation used four distinct built-in image_gen calls, with real alpha requested for each; there was no paid/API fallback or dependency installation.

The persisted ledger starts at`5533fff` and `git rev-list --count 5533fff..1ffe807` measures2 implementation commits. The realized ordinary unified diff across the exact nine implementation paths contains1,896,001 characters; `ceil(chars/4)` is474,001 on the estimate's scale. This includes the large serialized source/animation metadata and ordinary binary-diff notices, not a harness token count or base85 binary patch. Metadata close-out commits are outside the recorded before/after snapshot.

## Accomplishments

- Exported K01–K18/K20–K30 common source inputs, including725 sprites, seven texture inputs, four embedded fonts and the loose save image:737 resource records across29 donor families. K19 fantasy enemies are not selected as zombie art. The family audit retains exact selected identities and36 alternate-biome K02 rejections, keeping historical V/M/C observations distinct from source validation and pending actual judgment.
- Retained35 clips with61 bindings/components and594 timed keys, including seven consecutive repeated sprite keys. Portal close/open each retain ten layers; permanent destruction retains seven layers, including six actual constant curves. Independent H16 fixtures also exercise dense transform components, a held second layer, irregular/repeated pointer keys, explicit loops/held endpoints and event filtering. No frame order is guessed from filenames.
- Retained logical canvas, normalized bottom-left pivot, source offset, exact fractional trim/atlas offsets, packing rotation, alpha dependency and source pixels-per-unit separately from each decoded image. Two real empty hide frames (`common.s0.12582` chest_sparkles_7 and`common.s0.15397` trollcave_surface_destroy_18) remain explicit authored empties; they are not counted as completed visible content.
- Preserved exact OakAndBirch K29 raw-object identities and decoded fields, deerColorA/B gradients, assembly input and the original Deer/BaseSpriteFX source references. These are production inputs; complete wildlife recoloring/fitting still belongs to the later wildlife slice. Only `OnAnimHammerHit` becomes inert `work.contact` presentation data. Excluded callbacks, including SpitOutHermits/SpitOutGems and donor gameplay callbacks, are retained as inert source data and never invoked.
- Generated actual neutral-survivor, wide-zombie, metal-reinforcement and apocalypse-shelter artwork. Original masters, expanded prompts, inspected references, cleaned full-size images, native exports, exact recipes and day/night source proofs are retained in the tracked pilot ZIP. A scriptless actual Godot scene loads18 sprite instances across two rows, with nearest filtering, integer registration, metal over the donor wall, castle ground detail, quiet contact shapes and distinct day/night treatment.

## Task commits

1. **Task1 — Export selected common donor dependencies and runtime animation tracks:** `58872a7` — `feat(04-04): retain common donor pixels and exact runtime tracks`.
2. **Task2 — Produce production scale/composition pilots:** `1ffe807` — `feat(04-04): retain generated pilots and native scale comparison`.

Each task changed only its declared five paths; shared`media.cjs` makes the implementation union exactly nine. The parent separately authorized the documentation-only deferred-items entry and SDK ledger attempt. There are no tracked deletions or unexpected generated files; unrelated global/spec/phase06/phase12 work remains preserved.

## Verification and immutable evidence

| Command/case | Invocation | Revision | Result and duration |
| --- | --- | --- | --- |
| `node tools/phase4/check.cjs --case media.conversion` | `9ef861fb-3382-4745-8cfb-8ef4a6823c1d` | `5533fff` plus exact captured Task1 inputs | PASS,43,438ms; portable2395-member kit,13 rejected faults, native61 tracks/594 keys |
| Required post-Task1 tracer feedback, same command | `29152784-2275-4ac9-bd59-89bd38f6b306` | `58872a7` | PASS,43,431ms; gate passed before Task2 expansion |
| `node tools/phase4/check.cjs --case media.scale` | `cecdd0cb-38f6-4f34-b68f-a484fc49addb` | `58872a7` plus exact captured Task2 inputs | PASS,8,006ms; four master reproductions,12 semantic negatives and one actual native rejection |
| Final `media.conversion` | `dbb25049-0eb8-485c-bc24-91ccd1fb473c` | `1ffe807` | PASS,43,722ms |
| Final `media.scale` | `4cd8c68c-c1f3-475f-af0d-5bc2c8dd86a3` | `1ffe807` | PASS,8,103ms;18 native sprites/two day-night rows |
| Final `source.case-discovery` | `d029b2a2-f585-4831-ad91-c245d84fccab` | `1ffe807` | PASS,470ms; new saved cases discovered through current automatic module discovery |
| Additional final `source.provenance` | `adb38f27-592b-4700-880c-3fa1d65d0c15` | `1ffe807` | FAIL,34ms; inherited stale tracer scene binding, detailed below |

Every invocation lives under`.phase4/runs/<id>/` with captured current input hashes, command arguments, executable/environment identities, bounded child completion, stdout/stderr and result. The final declared selection and omissions are saved at`.phase4/04-04/selection.json`; independent cases continued after failures. The committed implementation has no compiled C# or project/policy changes, so the plan's conditional strict solution build does not apply. Unchanged audio/export/hub/LAN/full-catalog suites were not selected for this slice; actual phase-end selection remains separately owned.

Conversion controls reject malformed dense counts/rates, nonfinite constants, missing bindings, truncated streamed keys, out-of-range events, missing layers, wrong pivot, lost held keys, false alpha metadata, executable callbacks, changed loops and missing retained dependencies. Semantic ZIP mutations rebuild their outer inventories, so rejection is not merely a stale outer checksum. The saved literal H16 oracle uses nonuniform times0/.1875/.4375/.8125, sprite orderq/z/z/a, a separately held layer, three dense components and a constant. It is independent of the research happy-path data.

Scale controls reject absent master/prompt, smoothed sampling, ground/density changes, missing tower/base, mismatched world dimensions, shifted atlas region, erased D-13 precedence, fake generation and linear scene filtering. The actual native scene is separately corrupted in its owned proof project; a fresh Godot process exits1 with`MEDIA_SCALE_REJECT FILTER`, without a success marker. The passing scene is instantiated headlessly, not visually certified by that test.

Initial development evidence remains under`.phase4/04-04`: donor scans, incomplete new-owned authoring generations, initial slow mutation bundles and scale-stage prototypes. An ordinary sandbox attempt hit`spawnSync git EPERM` before a revision-bound run could start; the authorized native/Git child operation succeeded with approved execution. There was no source assertion pass inferred from that environment failure. Generated originals have partial-alpha edges; the survivor's original corner includes alpha1. The documented threshold cleanup is reproduced from originals, rather than falsely describing those masters as binary native-grid exports.

## Artifacts this phase produces

### Exact implementation files and executable symbols

| Exact path | Actual contract |
| --- | --- |
| `tools/phase4/extract-media.py` | Bounded new-owned extraction; strict portable ZIP/JSON/image verification and independent conversion controls |
| `src/godot-game/assets/common/donor-kit.png` | Actual2048×2763 lossless RGBA atlas; SHA256`bf3b0b91be9b772184f034d8e681e4a8b21210cfa5197aac2c03fc773c4b88d4` |
| `src/godot-game/content/packs/common.json` | Schema1/id`common`/version1;737 resources,35 clips,29 family audit rows, preservation, recipe, deer palette and pending acceptance data |
| `production/phase4/common-selected.zip` | 2,395 retained source/media members,12,308,215 bytes; SHA256`2f66618305221d32d503bba04be755ae38e606408a7c5d9d8f65a1db19d9e6d9` |
| `tools/phase4/cases/media.cjs` | Current automatic saved cases`media.conversion`(G0/order20) and`media.scale`(G0/order21), both Critical/no-window |
| `src/godot-game/assets/common/scale-pilot.png` | Actual512×128 separated native-art atlas; SHA256`4a91081a3ecc08a563585336a8a961165c8903696d6bebf9580d83b5f9a9f707` |
| `src/godot-game/content/packs/scale-pilot.json` | Schema1/id`scale-pilot`/version1; eight art inputs, exact dimensions/alpha/transform/generation references, assembly and D-13-pilot-1 decision |
| `src/godot-game/inspection/ScaleComparison.tscn` | Scriptless39-node native resource scene;18 actual sprites,14 contact shapes, two backgrounds and day/night parents |
| `production/phase4/scale-pilot.zip` | 39 retained master/prompt/reference/recipe/source-proof members,5,354,772 bytes; SHA256`da26ee892775c11871c1614dd917695524cb2e102b338ad33611431d956ef753` |

Python top-level call signatures are`need(ok, code)`, `sha(value)`, `digest(file)`, `relative(value)`, `contained(root, value)`, `owned(value)`, `portable(value, depth=0, budget=None)`, `json_bytes(value)`, `loads(value)`, `image_module()`, `png(image)`, `image_info(image)`, `streamed(words, count, start, stop)`, `convert_clip(tree, mapping)`, `archive(file, members)`, `read_archive(file)`, `extract(destination)`, `verify_common(bundle, pack_file, atlas_file, supplied_members=None)`, `conversion_controls(bundle, destination)` and`main()`. Extraction-local helpers are`preserve(name)`, `identity(obj)`, `retain(obj)`, `texture(pointer)` and`sprite(obj, family)`; JSON duplicate-key parsing uses`pairs(items)`. Constants are`ROOT`, `DONOR`, `DEPS`, `PYTHON`, `MAX_MEMBER`, `MAX_TOTAL`, `R`, `S`, `G`, `ANCHORS`, `CLIPS`, `FAMILIES`, `APPROVED_EVENTS` and`ANCHOR_NAMES`.

CLI modes are`--extract NEW_OWNED_DIRECTORY`, `--verify-common BUNDLE PACK ATLAS` and`--conversion-controls BUNDLE NEW_OWNED_DIRECTORY`. Extraction accepts only a fresh contained`.phase4` directory, rejects reparse ancestry and never writes recovered inputs. Selected owners/texture streams are manifest-hashed before and after export; the loose save source and retained source references have exact byte identities. Installed Python3.12.14/UnityPy1.23.0/Pillow13.0.0 are reused without installation. Individual archive members are bounded128MiB, total uncompressed bytes512MiB, member count16,384, image area16,777,216 pixels, and finite document/curve/count/depth bounds apply. Bounded owned Python/Godot children are supervised by the existing runner.

CommonJS functions/exports are`inputs(files)`, `validateCommon(pack)`, `engine()`, `conversionProbe(nonce)`, `nativeConversion(context)`, `conversion(context)`, `scaleProbe(nonce)`, `nativeScale(context)` and`scale(context)`. Exported constants additionally include`FILES`, `SCALE_VERIFY` and`SCALE_FILES`; local resource constants are`PYTHON`, `EXTRACT`, `COMMON`, `ATLAS`, `BUNDLE`, `SCALE`, `SCALE_ATLAS`, `SCALE_SCENE` and`SCALE_BUNDLE`. `SCALE_VERIFY` is trusted project-authored Python text, retaining`verify(pack, members)` and its independent semantic controls. Its exact bytes match the retained`recipes/verify-scale.py`; code from an untrusted donor/archive is never executed. Retained`recipes/build-scale.py` is the actual authoring recipe, not a newly installed package or native runtime dependency.

Emitted owned GDScript proof scripts define`reject(code)` and`_initialize()` only. There are **no new compiled C# types, members, Godot script classes, generated permissions or native-policy tuples** in this plan. Actual resource/subresource IDs are external`1_pilot` and AtlasTexture subresources`Atlas_survivor`, `Atlas_zombie`, `Atlas_metal`, `Atlas_shelter`, `Atlas_wall_wood`, `Atlas_wall_reinforced`, `Atlas_tower`, `Atlas_base_donor`. Root`ScaleComparison` has`Day` and`Night`; each has`Background`, `Artwork`, contacts`Contact16/56/100/140/180/228/320`, and sprites`survivor/zombie/wall_wood/wall_reinforced/metal_backing/metal/tower/shelter/base_donor` under Artwork. These exact39 nodes retain filter1,3× inspection zoom, ground92 and art layering. No foundation/gameplay scene is replaced.

### Exhaustive retained member identities

The **canonical exact member-name/byte/SHA256 inventories** are each bundle's`members.json`. They enumerate every other member; their own identities below close the inventory without a recursive self-hash. This is the parent's authorized machine-reconcilable inventory for all2,395 common members, rather than an abbreviated list mistaken for an exhaustive one.

| Bundle inventory | Inventory bytes/SHA256 | Exact coverage |
| --- | --- | --- |
| `common-selected.zip` → `members.json` | 322,142 bytes; `9c086cfd7ad4c36d02abc35f783b1fcafc701e34d090deb6f4ca6e8677b4aec2` | 2,394 rows plus this inventory =2,395 members |
| `scale-pilot.zip` → `members.json` | 4,665 bytes; `7568681efbf63a3617f4d292e50855773deb2fecebac68ccb68cbfc99119f56f` | 38 rows plus this inventory =39 members |

Common members comprise725`images/<owner>/<objectId>.png`,817 exact`raw/<owner>/<objectId>.bin`,761`trees/<owner>/<objectId>.json`,35`streams/<path>/<offset>-<size>.bin`,35 exact clip trees under`clips/<id>.json`, seven decoded texture inputs, four embedded font payloads, and these named records: `common.json`, `donor-kit.png`, `preserved-manifest.json`, `palettes/73492.json`, `palettes/73486.json`, `assembly/building-upgrades.json`, `reference-code/Deer.cs`, `reference-code/BaseSpriteFX.cs`, `recipes/extract-media.py`, `loose/SaveIcon.png`, `members.json`. The four font members are R6003/R6004/S0558/S0564 in`fonts/<owner>/<id>.ttf`; the complete literal paths and all hashes are in the canonical inventory. Resource IDs are immutable`common.r.<id>`/`common.s0.<id>`, `common.texture.<id>`, `common.font.<id>`, `common.save-indicator`; clips use`common.clip.<id>`. Every exact source owner/type/objectId/name and chosen transitive texture/alpha dependency is retained with the selected records.

All39 scale members are:

- `ScaleComparison.tscn`, `scale-pilot.json`, `scale-pilot.png`, `members.json`.
- `masters/metal.png`, `masters/shelter.png`, `masters/survivor.png`, `masters/zombie.png`.
- `prompts/metal.txt`, `prompts/shelter.txt`, `prompts/survivor.txt`, `prompts/zombie.txt`.
- `cleaned/metal.png`, `cleaned/shelter.png`, `cleaned/survivor.png`, `cleaned/zombie.png`.
- `native/metal.png`, `native/shelter.png`, `native/survivor.png`, `native/zombie.png`.
- `donors/base-donor.json`, `donors/base-donor.png`, `donors/tower.json`, `donors/tower.png`, `donors/wall-reinforced.json`, `donors/wall-reinforced.png`, `donors/wall-wood.json`, `donors/wall-wood.png`.
- `proofs/day-night-native.png`, `proofs/day-night-nearest-4x.png`, `recipes/build-scale.py`, `recipes/verify-scale.py`.
- `references/donor-castle.png`, `references/donor-lighthouse.png`, `references/donor-survivor.png`, `references/donor-tower.png`, `references/donor-wall.png`, `references/older-base.png`, `references/older-zombie.png`.

### Generated masters, dimensions and source production choice

All four generation calls used built-in image_gen with`transparent_background=true`, one distinct call per asset. Their retained generation session is`01a1e68a-0f44-7392-bfe8-70e3761f0132`; original output files remain under CODEX_HOME, while the tracked ZIP contains the actual original bytes and expanded prompts. Original file identities are`exec-2c6f9f79-f013-43c6-9eac-8efd070c05f8.png`(survivor), `exec-df0be24c-279b-46ff-ab9d-1e286f1c8268.png`(zombie), `exec-8dbfa84e-d3ca-41a1-8682-81e7d7eae8ef.png`(metal) and`exec-b77b82fa-79b7-4886-a2d7-a4e3344ae432.png`(shelter). Runtime/proof resolution uses the tracked assets, not those global output paths.

| Resource | Original canvas | Opaque crop size | Native export/world size | Retained original master SHA256 |
| --- | --- | --- | --- | --- |
| `scale.survivor` | 1003×1568 | 332×1271 | 8×30 | `5391c94333e798dce63e01cbf2c5f01bbdd228cd1ec6acba74b43cb1d2c03118` |
| `scale.zombie` | 1312×1199 | 996×1066 | 32×34 | `c705573b4c3fb6468568e8e68b8302e2ddd853d8ffc58279d3125f6696b99fd7` |
| `scale.metal` | 1081×1455 | 1008×1399 | 32×43 | `a3f8eb99248d9413cfde70beebfeabaf8359f03f01f018ad5405b622e4537191` |
| `scale.shelter` | 1586×992 | 1406×680 | 128×62 | `3ba2177e82fb3ce3a8a1629f23de63aebae94d3ef1389c7fb8b418dd3c3a51d8` |

Expanded prompt SHA256 values in the same row order are`8fa377301039da9f44246d78000d8aa574a02d144315f9af156db52dbbaaa768`, `ec6869a95cf6098defff93ec896b0e29ab177adc59f21406cc16baae2362e94c`, `76959fa1c7a00eac175526abe64fbb9cc03c211ce57121483ec375d8c17965ff`, `bce3473c6d3137b293d2e43b723cf02cba40152e214eeb2398193a1544c39d8f`. Each explicitly states currentD-13 low-resolution precedence and actual transparency. The seven references were visually inspected before generation; the original Inspiration directory was unavailable, so the skill's bundled original castle/lighthouse references were used. No reference was treated as a source of executable instructions.

Native donor identities are R8144 wall_1(32×88 logical,32×35 decoded), R9758 wall_2(32×88 logical,32×43 decoded), R8711 tower_2(96×124 logical,44×48 decoded) and R10583 castle_02_front(172×69 logical,97×8 decoded). Their pixels remain unchanged. Original fractional registration stays in the common/donor records; the scale fixture explicitly sets integer bottom anchors for its visible-ground comparison. In particular, the odd97px base detail uses offset-48 rather than a half-pixel offset.

`D-13-pilot-1` chooses **one native art pixel per world pixel**, with source reference32 pixels-per-unit and separate3× inspector zoom. Survivor height30 and wide-zombie height34 read beside native35/43px wood walls and48px tower; the128px shelter has an approximately35px doorway. Source proof`proofs/day-night-native.png` is400×224; the retained1600×896 proof is an exact4× nearest enlargement. These are assembled source images, not screenshots of a real game window or final supported-display evidence.

Cleanup thresholds source alpha at128, zeros RGB only outside the mask, preserves retained RGB, crops exact opaque bounds and uses nearest sampling. Metal deliberately fits32×43; other assets preserve their cropped aspect to the chosen native height/width within integer rounding. Original/generated/export/world sizes are never conflated. Native alpha is binary; preserved donor RGB underneath zero alpha is unchanged and never smoothed into visible edges. Visual source inspection supported the production choice; final named judgment remains pending.

## Decisions, deviations and deferred issues

No architectural change, new package or expanded product capability was required. The current D-13 decision supersedes the sprite skill's older richer-detail preference. End-of-phase judgment ownership supersedes that skill's optional generic test-approval pause; autonomous source production did not wait for optional approval. The required tracer feedback ran and passed before expansion.

The parent authorized keeping the two new packs standalone: `pack-index.json` still marks`common` and`scale-pilot` pending.04-23 must adopt every completed shard and validate current runtime reachability through the complete publication path before final review. Exact shared index/tracer ownership will be amended before that integration plan is dispatched. This plan does not claim current hub/census adoption, complete character/building fits, an accepted deer palette, or any completed obligation. Old bundles/provenance were preserved; new outputs were validated in owned staging before publication, and fresh-output guards prevent overwriting previous bundles.

**Active inherited phase-completion blocker:** `source.provenance` invocation`adb38f27-592b-4700-880c-3fa1d65d0c15` fails`PARTIAL_PACK_REQUIRED`. Read-only`content.validate` reports zero valid outputs/zero covered obligations and`OUTPUT_INVALID common.bow: FILE_HASH_MISMATCH src/godot-game/inspection/Inspection.tscn`. Every upstream snapshot/index/tracer input is byte-unchanged across`5533fff..1ffe807`(empty scoped Git diff). This failure is inherited from current-hub/tracer reconciliation; it is not caused by the nine paths produced here and was not repaired or repeatedly rerun outside ownership.

The parent's documentation-only amendment added the exact active entry to [deferred-items.md](deferred-items.md).04-23 source/index adoption and04-31 current-hub tracer/export reconciliation must authenticate current bindings and atomically publish their metadata **before final independent review/acceptance**. Preserve historical hashes/reports and never relabel old evidence. This is an active phase blocker, not an accepted omission or permission to ship.

The required SDK WINDOWS append was attempted with phase04/kind`deviation`/file`src/godot-game/content/pack-index.json`. It declined before writing because the existing rendered table disagrees with fenced JSON row28. The ledger had no preexisting Git diff; no hand-edit, repair or staging of that shared ledger occurred, and no new entry ID was produced. Parent reconciliation of that existing table/JSON inconsistency and the exact active deviation append remains required. The failed SDK diagnostic remains in the execution tool output.

Shared PROJECT/REQUIREMENTS/ROADMAP/STATE/spec/phase06/phase12 changes remain foreign-owned work in progress. Per parent instruction, this executor does not overwrite or stage them. After this summary commit, the parent must record the supported Phase04 progress delta **9/37→10/37**, with both source tasks complete, no requirement completion, and graph-ready plans04-05/04-11. No other phase is executed by this agent.

## Known stubs and threat surface

No implementation-blocking stubs, TODO/FIXME placeholders, skipped tests or unrun in-scope automated verifications remain in the nine paths. Pending final appearance/listening/integration ownership is explicitly declared future work, not a fabricated pass. The two empty donor frames are actual hidden-state source frames, not placeholders. New file access and parser/image/ZIP surfaces are covered by the plan's existing T-04-10/11/12 threat model, bounded data handling and semantic controls; no additional network/auth/schema boundary or compiled native permission is introduced.

## Self-Check: PASSED

All nine implementation artifacts and the supported deferred-items document exist. Both implementation commits are ancestors of the recorded current HEAD. The two canonical inventories authenticate all2,434 retained members across the bundles, including their separately recorded inventory identities. The implemented-case and tracer/native results above are actual retained invocations; the inherited provenance failure and missing final actual acceptance remain explicit.
