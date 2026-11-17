# Plan 04-09 — Sustained structural guard checkpoint

Latest status after prospective R1/R2 `c693867`: incomplete, zero of two tasks complete. The focused bow case and unchanged frozen17-case regression selection pass, but a subsequent authenticated sustained-guard diagnostic fails the required unlimited-arrow behavior. No Task1 GREEN commit, committed-content tracer, Task2, SUMMARY or global progress exists. The passing selection does not override this later failure.

## Latest failure: intrinsic guard stops after one shot

Specification §8.1 states that structural tower archers use unlimited arrows and have no separate inventory or bow durability. The R2 preview makes this mode independent of empty/broken ordinary inventory, but represents its intrinsic ammunition using one ordinary presentation arrow. `CharacterPresentation.TryRelease` increments `_spent`; `Advance` resets `_released` at the next generation while `BowReady` still requires the ordinary count to exceed `_spent`. No explicit intrinsic-ammunition state exists in the current six-argument `ConfigureBow` contract.

Separate diagnostic invocation `33f812b1-84dd-4fc4-b636-687769db5e86` selects empty left/right inventory, zero ordinary ammunition, Broken ordinary condition and explicit Structural guard mode. Actual authenticated nonconsole Godot PID30880 exits0 with empty stderr; Core and game hashes match its copied closure in the same owning Godot ALC. The unchanged original704 melee samples, five exact source negatives and restored positive also complete. The diagnostic's own required assertion fails `GUARD_UNLIMITED_ARROWS_AFTER_FIRST_CYCLE`:

| Point | Generation | Action seconds | Interval | Nocked | Released | Spent | Fallback |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Initial | 9 | 0 | 1.2 | true | false | 0 | none |
| Advance .601 | 9 | .601 | 1.2 | false | true | 1 | none |
| Advance .6 | 10 | .001000000000000112 | .7 | false | false | 1 | melee-or-fists |
| Advance .601 | 10 | .6020000000000001 | .7 | false | false | 1 | melee-or-fists |
| Advance .6 | 11 | .502 | .7 | false | false | 1 | melee-or-fists |

Both later samples have neither GuardArrow nor SpentArrow. The diagnostic retains all five detached samples, native source, PID/nonce, actual exit, assemblies and source fingerprints in its run directory. It has no canonical saved-case or selection pass credit. Its first setup-only attempt preloaded animation.cjs before CLI discovery and failed CASE_MODULE_EMPTY before any native run; the original ignored setup script/error remain preserved separately. The corrected diagnostic bootstrap uses a separate immutable context and the unchanged actual native proof/authentication functions. No production change followed the sustained failure.

## R1/R2 implementation and attempts

| Scope / attempt | Invocation | Result |
| --- | --- | --- |
| R1 initial | `4afc4eae-d184-4e08-9a16-ceccca6ac677` | Strict production/harness0/0; nativePID29300/exit1 at the distinct ordinary arm-pixel assertion. The prior missing atlas exception is absent; no complete bow pass claimed |
| R2 initial | `f424a0d0-b053-482a-a5cc-cad4b97923b5` | Four actual K15 clips/27 frames,11 source-negative controls/restored positive and raw atlas/frame pixels pass; strict build fails only public/private test method-order SCR0008 |
| R2 correction1/2 | `ee7ff095-ff19-4c81-93e4-8b43b0ac8305` | Strict production/harness0/0; nativePID38352/exit1 at added all-lower-frame common.s0.8794 control. Static pose bypasses Configure, so the new control's absolute midpoint ticks accumulated stride |
| R2 correction2/2 | `c965bde6-eba3-4869-9c1c-85436b911052` | Reset each frame through a genuinely different live movement without changing expected source times/pixels. Strict builds0/0; actualPID16004/exit0/empty stderr;26 compiled pass/zero skip.704 original samples/448 ordinary bow/112 guard/14 boundaries/12 binding negatives/23 extra controls/five original negatives/restored positive pass |
| Subsequent sustained diagnostic | `33f812b1-84dd-4fc4-b636-687769db5e86` | Actual native process succeeds/authenticates, but required unlimited-arrow assertion fails as above; Task1 remains incomplete |

R1 fixes only AddSpentArrow's atlas owner to the actual validated fit item, retaining IT_02_master. Its proof independently checks released pixels, actual pack atlas/rectangle, timing/registration, both logical hands/directions and no-pickup/gameplay flags. R2 rounds source-arm widths to their nearest logical pixel so float coordinate subtraction cannot shorten an arm at non-cardinal angles; the oracle independently expects the specified reach. It authenticates K15 idle777/run780/shoot783/walk804 IDs/times/rectangles/original pivots/registration and all27 raw source-frame RGBA hashes against the full common atlas, pins source-inspected ordinary/guard masks/crops and verifies actual rotating arms, sleeves, gloves and held pixels. Actual bodies-a.cjs and the used extract-media.py helper now enter bow fingerprints; old omitted-helper reports remain unchanged. Pure tests include strict bow fields/events, actual30-arrow maximum and independent limit2, finite angles, detached samples, late-generation/one-shot release and usable/broken fallback.

Original Task1 corrections3/3 and decomposition behavior2/2 remain exhausted. Binding repair remains resolved1/2. R1 resolved with its initial implementation; R2 used both corrections and cannot silently receive another. The frozen-selection substep performed its full first run without source corrections, but its passing cases are insufficient after the separate sustained failure.

## Frozen regression result and exact preserved source

`.phase4/04-09-task1-selection/{selection,results,completion}.json` records all17 selected IDs, each immutable run,402 frozen inputs, exit0 and empty drift. Bow run77c7d1d7, replacement5ce9ef5f and melee d9721b57 pass with all original contracts. A art/fit, B-wide/complete-fit, discovery, foundation/items data, hub source/lifecycle, both item cases and theme/identity/integrity pass. A retains2976 tuples/5972 native/10 compiled/12 H13; all-eight retains7936/16158/12 H13; both original animation cases retain704/four jumps/five negatives/restored positive/41 replacement controls/seven UI states/35 compiled with zero skips; both hubs retain63 compiled each and25/50 native. The selection's declared omissions stay visible, with no unselected, rendered or listening pass credit.

Tested source is HEAD `c693867de94d4cca53752597cbaa21c56b5a64d6` plus the seven WIP files in `.phase4/checkpoints/04-09-33f812b1-wip/manifest.json`. Every live/backup raw SHA256 and Git-filtered blob matches; all seven also match both focused bow-inputs and the sustained diagnostic source-inputs. No tracked/source bytes changed during the17-case selection or diagnostic. Earlier8106/f4e4 backups and every failed report remain untouched.

Diagnostic source/DLL mapping, independently checked against its native process and actual copied closure:

| Subject | SHA256 |
| --- | --- |
| Core DLL | `264a2b4f4b71587efc8d97a76d9bfd3640eb3192fce09a7be2a0109e3db4660c` |
| Game DLL | `57573f309a9d097fabd46398558bc1ae8add22d9e5ca63c63db3886bb12a902f` |
| Exact emitted diagnostic harness | `49434eb9036affee486f26a87d8c106343cd7167ebc1ce464e3a5c5e45ecf8b9` |
| Exact diagnostic source-input list | `31778cd950aba38a47d2894d9e8ba9b0952da4a24c3627c0d2cbfe5de829396d` |

These are tested HEAD plus WIP identities, not a later code commit. The seven-file manifest records raw checkout and Git-filtered identities separately; the402-input selection has its own full frozen manifest.

One current changed-source integration observation `4f18a393-4055-47ef-99fc-34330f3fcea5` is FAIL547 operations:166 BuildingPreview,303 CharacterPreview and78 IdentityKit. New `.phase4/04-09-r2-checkpoint-native-policy-observation.json` preserves exact semantic tuples; `.phase4/04-09-r2-checkpoint-generated-input-observation.json` preserves49 actual source inputs/32 generated surfaces, producer package identity and the current strict-built closure. Old546/49/32 observations are not overwritten. No permission, cohort, policy pass or final integration acceptance is granted.

## Required prospective repair boundary

A coordinator must prospectively authorize an explicit intrinsic structural-ammunition contract before any further implementation. A concrete option is an additive overload/state on existing CharacterPresentation that preserves the current six-argument ConfigureBow and all ordinary bounded-stack semantics while explicitly distinguishing structural ammunition from inventory counts. CharacterPreview would select that mode without synthetic counts. Independent tests must cover repeated shots beyond the actual ordinary30-arrow maximum, both hands/directions, empty/broken/zero ordinary inventory, no consumption/refill/reload/loot/gameplay events, generation-bound one-shot release, and return to unchanged ordinary0/1/many/last/replacement behavior. Exact API/implementation choice and correction budget remain unapproved here.

Concrete **proposed, unapproved** additive signature on the existing public sealed identity:

```csharp
public void ConfigureBow(double aimRadians, int leftArrows, int rightArrows,
        bool leftUsable, bool rightUsable, string stackIdentity, bool intrinsicAmmunition)
```

Preserve the existing six-argument method as an ordinary-mode forwarding overload with `intrinsicAmmunition: false`. The explicit true mode would require zero inventory-arrow counts, permit intrinsic readiness without an opposite-hand IT02 inventory record, expose a detached intrinsic-mode flag and keep one release per generation. Preview selection of that boolean would come only from explicit Structural guard mode; the visible body stays the real K15 source. Returning to ordinary mode must invalidate stale structural release authority and preserve actual bounded stacks. The stack identity remains an identity only. This describes a reviewable repair boundary; it grants no current authority to add the overload or change the implementation.

Counter inflation, stack refill, string-identity sentinels, repeated Configure/restart or loop workarounds must not masquerade as unlimited ammunition. No extra source correction, public signature, file/path or authority is inferred from this checkpoint. After a genuine fix: focused native/compiled controls, a fresh full justified frozen selection, exact Task1 commits and fresh same-case committed-content tracer still precede Task2.

Task2 outputs remain absent; no art generation occurred. Branch feature/04-asset-production and draftPR5/dev remain unchanged; ten-path plan union/phase236 and WINDOWS29/30/31 remain pending. Foreign global/spec/Phase6/Phase12 WIP stays unstaged. Parent owns progress/PR reconciliation. No real window, physical input, listening, independent judgment, ASSET completion or phase acceptance is claimed.

## Latest checkpoint self-check: PASSED

All seven live/backup raw and filtered identities match the manifest and both actual input lists. The402 frozen inputs still match selection hashes after the diagnostic. Required historical commits remain ancestors. SUMMARY and all three Task2 outputs are absent. Both new observation documents parse, old reports/backups remain intact, the index was empty before scoped metadata staging, branch protection query returns false for the existing feature branch, and whitespace checks pass. This preservation self-check does not supply a guard behavior, task completion or acceptance pass.

## Previous bounded behavior checkpoint — preserved history

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
