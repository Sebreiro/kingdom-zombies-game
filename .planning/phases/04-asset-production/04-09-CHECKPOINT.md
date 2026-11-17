# Plan 04-09 — Bounded behavior repair checkpoint

Current status after prospective decomposition `ea972a8`: incomplete, zero of two tasks complete. Original Task1 corrections remain exhausted3/3. Binding substep1 resolved with one of two corrections. Behavior substep2 exhausted both of its two corrections and stops here. Frozen-selection substep3 and Task2 are unstarted. No Task1 GREEN commit, committed-content tracer, SUMMARY or global progress is claimed.

## Latest immutable failure and source

Canonical invocation `8106c944-2765-44df-83e6-983539a39d45` passes strict production and harness builds with zero warnings/errors. The official authenticated nonconsole native process PID14516 exits1 on the first ordinary bow release:

```text
System.Collections.Generic.KeyNotFoundException: The given key was not present in the dictionary.
CharacterPreview.Text:513 -> CharacterPreview.AddSpentArrow:936
-> CharacterPreview.ProjectGear:903 -> CharacterPreview.Project:651 -> Tick:206
```

`AddSpentArrow` selects the real IT-02 record from the equipment pack's `items` array, then requests its nonexistent `atlas` property. Actual IT-02 pack item data has no such property; the existing fit item owns the atlas path. This is a distinct production failure after the permitted correction2. No further source correction or retry occurred. Fatal stderr also retains native cleanup warnings; neither a partial result nor successful build overrides the exit1 failure.

Exact tested source is HEAD `ea972a86f55f50739681bfe4e2b0a0b861d06f98` plus all seven WIP files in `.phase4/checkpoints/04-09-8106c944-wip/manifest.json`. Every live and backup raw SHA256/length was checked, and each Git-filtered blob identity is recorded separately. The earlier seven-file `f4e4172c` backup remains unchanged. Both RED commits and the original checkpoint/decomposition commits are reachable. No SUMMARY exists.

## Bounded repair execution ledger

| Scope / attempt | Invocation | Actual result |
| --- | --- | --- |
| Binding initial environment attempt | `e9337275-43de-4b41-af8b-873f907321d8` | Managed sandbox subprocess `spawn EPERM`; no behavior credit or correction consumed |
| Binding initial implementation | `85d57095-b1b2-47cc-b138-8ced88ca0d40` | Strict production/harness0/0; authenticated nativePID14948/exit0;11 exact bow controls, restored actual Open/Attach, original704 melee/four jumps/five negatives/restored positive |
| Binding correction1/2 | `087063d9-eb26-45be-97a5-57a411d72ac2` | Strict production/harness0/0; authenticated nativePID31048/exit0;12 exact bow controls/restored actual Open/Attach and original704/four jumps/five negatives/restored positive. Complete translated bow path set now validates before referenced reads; missing/duplicate/reordered/unexpected canonical sets have empty read traces |
| Behavior initial implementation | `a4f5189e-af2a-4ce0-976f-c4c4743a5140` | Strict build fails new Core CA1854 dictionary double lookup; no native/compiled GREEN execution |
| Behavior correction1/2 | `b0193137-ebb2-4787-9afb-d11941d7f613` | TryGetValue removes CA1854; strict build then finds one binary-operator indentation and three test null-forgiving diagnostics; no native/compiled GREEN execution |
| Behavior correction2/2 | `8106c944-2765-44df-83e6-983539a39d45` | Only those four diagnostics corrected. Strict production/harness0/0; nativePID14516/exit1 at AddSpentArrow missing atlas.12 exact binding controls/restored positive persist in native project's `bow-binding-results.json`; no completed animation observation or compiled GREEN report |

All commands use the unchanged canonical process gate, actual installed tools and no real game window. The diagnostic binding harness is explicitly separate from canonical `animation.bow`; it does not substitute for bow behavior or the committed tracer. Expected ResourcePath exception types/messages and the original locomotion five controls remain unchanged. The successful first tuple's aim checks are reachable before the failing release in the emitted harness, but no completed bow matrix result is claimed or reconstructed from that ordering.

## Current implementation drafts and remaining proof

The retained seven-file WIP includes the original Core bow API and selector work plus the bounded repair drafts. Bow paths are exact repository-prefixed paths; the reader compares translated ordinal keys before hashing resources. Binding controls independently exercise wrong prefix/traversal, missing definition/resource, empty/oversized resource, bad hash, missing/duplicate/reordered/unexpected canonical binding sets, duplicate JSON key and restored actual Open/Attach.

Actual retained five K15 shoot crops and all eight ordinary idle/aim upper-body crops were inspected as source images. Source screenshots are `.phase4/04-09-bindings/guard-frames-inspection.png`, `ordinary-aim-inspection.png` and `ordinary-idle-inspection.png`; these are source inspection, not actual appearance acceptance. The old shoulder-span torso crop clipped bulky ordinary bodies. The draft now keeps the full normal upper image outside explicit retired-arm masks and composes copied actual aim-arm strips with actual fitted sleeves/gloves and held gear. No new raster/path, CH alias or source-media edit was introduced.

The draft K15 composition retains exact source offsets and actual idle777/run780/walk804 frame/time/rectangle selections alongside shoot783. It masks extracted arm/bow spans and samples lower frames independently; two actual source sleeve strips and the actual bow crop rotate about registered shoulders. Nocked/spent arrows use actual retained IT-02 pixels. The authored native proof compares source pixels, removed-pixel masks, torso/head stability, source grips, non-cardinal rotation, attached sleeves/gloves and lower registration. These are **unproved drafts**: the448 ordinary rows,112 guard rows and14 boundaries have no completed native result. Guard inactive/broken behavior and full definition/source consistency also remain to be assessed.

Pure test drafts cover zero/one/many ammunition, finite angle rejection, actual configured IT-02 maxStack30 (independent authored limit2 fixture), immediate replacement/stale generation, detached snapshots and usable/broken melee/fist fallback. They have not executed GREEN because canonical execution stops at native failure. Original explicit Melee still expects the correct .7-second fist fallback. Native rows must be validated as actual outcomes, not merely by counts.

The source oracle currently authenticates only its original five shoot-frame rectangles/times against the full common pack/hash. It still needs independent complete controls for the newly retained guard locomotion/registration selections and accepted ordinary/guard masks/crops, plus required definition/event/finite/numeric/stack malformed controls. No full affected selection or original regression pass on the changed behavior source is claimed. Preserve all original04-08 matrices and counts; do not run unchanged known integration failures repeatedly.

## Changed integration observations and continuation boundary

One changed-source policy observation `a742804e-73b7-4b0b-8de9-81bf1b93cdab` remains FAIL546 operations:166 BuildingPreview,302 CharacterPreview and78 IdentityKit. Exact source/line/column/member/host facts are in `.phase4/04-09-checkpoint-native-policy-observation.json`. The separate `.phase4/04-09-checkpoint-generated-input-observation.json` records49 current source inputs,32 emitted surfaces, authenticated producer package identity and the actual `8106c944` production closure. No permission, cohort enrollment or policy pass was granted.

Stop because substep2's prospective two-correction budget is exhausted. A fresh coordinator continuation must establish an exact prospective repair scope; this technical checkpoint is not a package/auth/manual-human prerequisite. Original3/3 and the bounded ledger above must never reset silently. Task2 remains entirely unstarted and the mandatory fresh committed-content Task1 tracer remains ahead of it. No work-special JSON/PNG/ZIP was created.

Only this checkpoint and the repair ledger are eligible for the current metadata commit. Production WIP and foreign PROJECT/REQUIREMENTS/ROADMAP/STATE/spec/Phase6/Phase12 work stay unstaged. Branch `feature/04-asset-production`/draft PR5/dev, ten-path union, inventory236, WINDOWS29/30/31 and all actual acceptance limits remain unchanged. Parent owns progress/PR reconciliation. No switch, push, merge, install, listening, physical input, game window, requirement completion or phase acceptance occurred.

## Latest preservation self-check

All seven current raw hashes match their backup copies. Six also match the canonical run's `bow-inputs.json`; the unchanged `tools/phase4/cases/bodies-a.cjs` matches its original f4e4172c checkpoint hash but is absent from the bow case's current fingerprint list. That missing fingerprint coverage is explicit and needs closure before the frozen selection/tracer; the preservation audit does not fabricate an in-run fingerprint. The twelve actual persisted binding controls/restored positive parse successfully. SUMMARY and all three Task2 outputs are absent, the Git index is empty before the scoped metadata commit, HEAD stays on the existing feature branch, and whitespace checks pass. No preservation assertion supplies behavior or full-selection acceptance.

## Earlier exhausted checkpoint (preserved history)

Status: incomplete; zero of two tasks complete. Original Task 1 correction budget is exhausted at three of three. No Task 1 GREEN commit, committed-content tracer, Task 2 implementation, SUMMARY, global progress, ASSET completion or actual acceptance is claimed.

## Current failure and immutable evidence

Canonical invocation `f4e4172c-e764-435a-b9d2-e8f37792c012` in `.phase4/runs/` passes the current strict solution and native harness builds with zero warnings and zero errors. Authenticated official nonconsole Godot 4.7.2 native process PID37728 exits1 during the real production `CharacterPreview.Open`, before the bow matrix or compiled GREEN tests execute:

```text
System.FormatException: path requires exact src/godot-game/ prefix: content/config/items.json
ContentPack.ResourcePath -> CharacterPreview.ReadPresentation:263 -> CharacterPreview.Open
```

The new bow bindings use project-relative paths, while the existing canonical ResourcePath contract requires the exact repository prefix. Existing locomotion bindings and their precise traversal exception contract are unchanged. The proposed bounded next repair is to correct the new bow binding paths and their exact expected set, without weakening ResourcePath or accepting a different exception. No further source correction was performed after this failure.

The tested source is HEAD `0f499f9d71bafeac936e4273313e6f9f45e1bbb4` plus the exact seven WIP files backed up at `.phase4/checkpoints/04-09-f4e4172c-wip/manifest.json`. That manifest records raw SHA256, byte length and Git-filtered blob identity per file. Copies retain the original path beneath that directory. Raw checkout and filtered Git identities are distinct; no later commit is represented as tested.

## TDD history

- `7f9ad2a`: historical semantically invalid RED. Explicit Melee with a selected bow correctly uses the .7-second fist fallback; expecting1.2 there was the wrong action. Its original unchanged JUnit and SDK result remain in `.phase4/04-09-red/`, with the semantic rejection recorded in the approved amendment and corrected evidence. The current test preserves the correct explicit Melee expectation; it is neither wrong nor skipped.
- `651c553`: accepted genuine native RED. Invocation `cdb525f6-8bf7-49de-b456-e22fcce9ea1d` has strict solution/harness0/0, authenticated actual native PID12776/exit0, all704 original melee samples and five source negative controls. Actual selected IT-07 left, IT-02 right and Usable condition expose Action options None/Melee. `Native_UsableBowOffersBowAction` executes and fails `offered=false` versus true; the independent explicit Melee fallback passes. Exact qualified JUnit target receives `RED_EVIDENCE_OK`; semantic assessment is valid. No production bow behavior preceded this RED.
- Initial RED setup failures (unused using; later an IDE0007 declaration) have no behavior credit. The short JUnit target classifier rejection is preserved and was corrected to the exact emitted fully qualified target without altering the original report.

## Original Task 1 correction ledger

1. Initial GREEN strict build: three authored Core formatting diagnostics. Correction1 fixes indentation/comment lengths; the next build reports three named-argument errors (`Math.Cos` and Godot `Image.GetRegion`).
2. Correction2 uses actual pinned GodotSharp XML and .NET reference parameter definitions (`region`, `d`); strict solution build passes0/0.
3. After the complete new native matrix and continuous slider are authored, canonical `29940f01-22c2-46d8-89c2-acaf5d1d53aa` fails strict build with five diagnostics: Project64 lines, slider field CA1001, test line length and wrapping. Correction3 borrows the engine-owned slider through guarded root lookup, moves preset reconciliation into guarded UpdateAim, and fixes test formatting. Canonical `f4e4172c` passes both strict builds but fails the distinct new binding prefix at native Open.

Budget remains3/3 exhausted. No node-repair decomposition or reset is assumed. This checkpoint requires a prospective coordinator repair scope or other explicit continuation decision.

## Preserved implementation and remaining proof

Exactly seven WIP paths are preserved:

| Path | Current responsibility |
| --- | --- |
| `src/core/CharacterPresentation.cs` | Existing sealed identity; additive four-definition constructor, ConfigureBow and TryRelease; detached aim/ammunition/stack/release/fallback fields; original constructor/Configure/Melee compatibility retained |
| `src/godot-game/inspection/CharacterPreview.cs` | Required bow loading/binding validation; Bow action, count/stack/preset and continuous elevation controls; normal torso/head with mechanically cropped actual aim-arm art; fitted gear; spent-arrow visual; explicit retained K15 guard mode |
| `src/godot-game/content/animation/bow.json` | AN07/08 timing/events, source bindings and exact K15 source clip783/frame rectangles/times; current canonical-path error remains unfixed |
| `tests/phase1/CharacterBowTests.cs` | Actual Bow availability; original explicit Melee fallback; zero/one/many ammunition; one-shot release, detached samples and immediate replacement; expected native matrix/boundaries |
| `tools/phase4/cases/bow-work.cjs` | Actual source-bound animation.bow case, source guard authentication, authored448 bow aim/release rows and14 boundary outcomes; these expanded outcomes have not executed successfully |
| `tools/phase4/cases/animation.cjs` | Approved resource staging/fingerprint closure only: bow definition and actual common pack/atlas |
| `tools/phase4/cases/bodies-a.cjs` | Approved FIT_FILES staging/fingerprint closure only: same required resources |

The prospective source/native proof still must establish every new actual grip, source crop, normal head/torso, lower stride, continuous angle, both logical hands/directions, release recovery, zero/one/many/replacement/broken/empty/fallback boundary, actual retained structural guard and no pickup claim. Existing full704/four-jump/five-negative/restored-positive,41 replacement/control and seven UI-state regressions remain required and unchanged. No expanded native pass is inferred from compilation. Further strict definition/event/finite-boundary and generation controls, detached sample assertions, exact native tuple observation and current producer impact need verification before Task1 completion. Guard currently samples retained source frames; required continuous guard aiming must be assessed, not silently inferred.

Task2 has not started. No work-special JSON/PNG/ZIP exists from this execution, and no raster generation was performed. Imagegen and kingdom-zombies-sprites skills were read in preparation; D13 remains authoritative over the older richer-style default. The mandatory fresh same-case committed-byte Task1 tracer must pass before any Task2 expansion.

## Scope and continuation constraints

Prospective amendments `f41ea36` and `0f499f9` authorize the two existing consumer staging/fingerprint lists and the existing Core API only. Plan union10 and phase inventory236 remain unchanged. No Root, generated/native permission, new public identity, new Godot script, tool installation, capacity/deadline or common-media edit was made. Native access remains observation for23; no policy cohort or authorization was granted.

Branch remains `feature/04-asset-production`, existing draft PR5 targeting dev. No branch switch, merge or push. Foreign PROJECT/REQUIREMENTS/ROADMAP/STATE/spec/Phase6/Phase12 edits remain unstaged. All WINDOWS29/30/31, full policy/pack integration, actual Windows1280x720 appearance, physical input, listening, independent judgment, all ASSET and32/33 acceptance remain pending under their original owners.

## Self-check

The seven backup copies and current source hashes were compared at checkpoint creation. Both RED commits are reachable from HEAD. No task was committed complete and no SUMMARY was created.
