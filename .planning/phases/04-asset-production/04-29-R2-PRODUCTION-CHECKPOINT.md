# Plan04-29 R2 native source-semantics checkpoint

Recorded2026-11-18T12:10:51UTC on `feature/04-asset-production`, source HEAD `4ce3803dd5fe50cc56523a6e1500ea6aa5bf6ee0`. Original Task1 remains exhausted3/3. R1 remains initial plus2/2 exhausted. R2 completed its initial implementation/verification, with corrective attempts **0/2 used**. Task2 remains unstarted0/3.

## Blocking production defect

The working standalone Audio native bridge completed its current report and demonstrated a real production source-semantics defect. R2 owns only proof setup in audio.cjs and explicitly requires a checkpoint before any fixed production edit. No production edit or R2 correction has been made in response.

`AudioPreview.LoadPack` resolves interface samples without an inline `source` by searching only `pack.sources` for an equal `sourceId`. The actual current interface pack has23 ordinary donor source records, plus a documented generated food source and an explicit shared action source. Those two valid source forms are not entries in the23 donor records:

| Actual sample | Current sourceId | Actual source authority | Rejected behavior |
| --- | --- | --- | --- |
| `interface.food-crunch.v1` | `generated-food-crunch` | Current pack donorGap AU-19, approved asset-plan section10, deterministic contact/crunch seed190021 and retained recipe identity | `Single` throws `Sequence contains no matching element`; route `food.use-sale.consume` is unavailable; AU-19 exposes1of2 routes |
| `interface.bloater-burst.v1` | `action.2790.full` | Current pack `sharedSources` explicitly references `packs/audio-actions.json`, WAV SHA256 `9b29b74a94784a96307282820866e3be9731670dc32e9baa2ddd3bdbcb53f4cb` | Same failed source lookup; route `bloater.splash.cosmetic-death-burst` unavailable; AU-29 exposes0of1 routes |

The current interface PCM/sample bytes and hashes are valid dependency-produced resources. This is not a reason to edit the catalog, donor/source media, recipes, approval or fixture to match the implementation. The actual preview must preserve and validate each source form and its provenance rather than assuming every source is an ordinary donor row. Shared source approval cannot be extended to the new derived bloater variant; all27new variants stay Not auditioned.

## Actual native observation

Case `audio.lifecycle`, invocation **`157bbeea-0670-47d1-85b3-114dc37d733d`**, completed controller native process **PID37612**, matching nonce `157bbeea-0670-47d1-85b3-114dc37d733d`. It saved656 individual outcomes: **651pass/5fail**, then emitted the explicit `INVENTORY_NATIVE_COMPLETE ... FAIL` marker and exited1. Stdout contains the engine banner and FAIL marker; stderr is empty. Strict production and native harness builds both passed0warnings/0errors; the genuine compiled ownership test passed1/1 with0skips.

Current native game assembly SHA256 `279aa5708e63bf62be6aa8450fe4aa2ae46d91374d545c778767dced0a7f9029`; Core SHA256 `f59a66e3be25946e693728499034f3aa1614aef4323ad9c889528883b83ff85b`. The report verifies the loaded game/Core assemblies share the controller harness's actual ALC. This is a controller-process claim only.

Unchanged full raw report: `.phase4/runs/157bbeea-0670-47d1-85b3-114dc37d733d/native-inventory/inventory-results.json`. The invocation directory preserves all source fingerprints, production assembly closure, authenticated official engine identity/origin/version, build/JUnit/native logs and process exit/command records. The helper rejects the actual child exit1 before copying the report to its success-only observation filename; read the original completed native report at the path above. No failing child was hidden or merged as success.

The five failing IDs are:

- `AU-19-all-routes` — observed1, expected actual current catalog route count2.
- `AU-29-all-routes` — observed0, expected actual current catalog route count1.
- `muted-actual-native-volume` — observed0players; separately diagnosed clock/route setup issue below.
- `AUDIO-AUDITION.populated` — no AU-29 selector item due the demonstrated production source rejection.
- `AUDIO-AUDITION.long-text` — separately diagnosed selector setup issue below; current details also preserve the production source errors.

651 individual true predicates do not make this case or task pass. All91 routes, all eight backstops, actual Hub and full acceptance remain unproved as a complete contract. No GREEN or tracer exists.

## Two independently diagnosed proof observation issues

These are observations to resolve only under a prospective continuation; no test change has been made after the failed run.

1. Lifetime admits the assault warning in the replacement owner at explicit preview time0, then selects sorted Event row0 (the same assault route), mutes and immediately presses Play without advancing the preview clock. The actual route's100ms cooldown rejects that repeat, so the test sees no player to inspect for muted volume. Keep the cooldown/actual admission semantics and the muted volume predicate; make its setup independently establish an admitted current muted audition rather than treating an intentional cooldown rejection as a volume observation. Do not remove the cooldown or replace expected volume with observed state.
2. Long metadata changes AU-20's catalog `routes[0]` (day) but the actual native Event selector sorts IDs ordinally, initially choosing assault. The long text is present in the rebound day route, while the test reads untouched assault details. Explicitly select the independently identified mutated day route before reading its TextEdit. Preserve the full long text, wrapping/noneditable predicate, original ID/order and exact current binding controls.

## R2 proof setup authored and not yet verified end-to-end

Before editing, all five files matched the835f6c9 native checkpoint and were immutably snapshotted to `.phase4/audio29-r2-checkpoint-4ce3803/`, with create-only writes, raw hashes and independent readback.

R2 modifies **only audio.cjs**. The controller bridge no longer invokes its scene-bound Hub method. The same final three hub predicates/IDs/order are authored in a separate GDScript observer using the proven world.inspector copy protocol: actual `project.godot`, complete inspection/content/assets trees and exact already-built game/Core closure. It authenticates the official nonconsole executable against the existing controller invocation's archive identity, uses the established actual-project1600frame cap, requires native completion, exact ordered outcomes and clean logs, and saves process-qualified source/closure/engine/PID/nonce evidence. Only two complete successful groups would be merged, rejecting duplicate/failing IDs.

The controller's exit1 stopped execution **before `audioHub` ran**. Therefore the actual-project Hub split, its class binding, loading/navigation assertions and combined report have no execution credit yet. No claim says one process/ALC observed both groups. No process deadline was enlarged, and no shadow Game assembly or tracked project/config was rebuilt.

## Exact current implementation bytes

All four fixed production/test/fixture files remain byte-identical to835f6c9. Only audio.cjs changed during R2.

| File | Current SHA256 |
| --- | --- |
| `src/godot-game/inspection/AudioPreview.cs` | `c0e39826edf0ec613c5a0bcb51eb77f390b665d7241ebd7f98aa3d324b032bb7` |
| `src/godot-game/inspection/InspectionRoot.cs` | `6e831506dc3ee8b58ad59b2c857d81ad885d630e9f40839c2eafc478b0539e9a` |
| `tests/phase1/AudioPresentationTests.cs` | `3f6f8265a43be327cc5123a0b4a9facbea0317f502470e0dffe1acbd9ee7d01a` |
| `src/godot-game/content/fixtures/audio-states.json` | `4adf9d6c32c089a6afeefa074ba4cf5d6dc4c1e7396fddd32212cc55fcfe732b` |
| `tools/phase4/cases/audio.cjs` | `991a560010603ab2698039582b829f1cb4c4380bb83ac8589eafbc4caef8589f` |

## Awaiting prospective scope

Authorize a concrete bounded production-source-form repair and independently diagnosed observation setup corrections before further edits. Preserve ordinary donor lookup, generated-source authority and exact recipe provenance, shared current-pack/sample/hash binding, exact PCM regions, approval separation and all intended route/backstop/limit/lifetime predicates. A fallback empty source or generic catch/accept is not a repair. This checkpoint does not itself authorize changes to fixed files.

Original Task1 and R1 counts remain exhausted; R2 has0/2 corrections used but its proof-only scope cannot absorb a production change. Any continuation must record its own exact budget/scope without resetting these counts. Complete actual Task1, exact GREEN commit and fresh same-case committed tracer must precede untouched Task2 TDD. Final fresh49selection, exactly one final changed policy/producer closure, full symbol/native tuple inventory, canonical SUMMARY/self-check and parent global updates remain pending.

All global files, media/catalog/reader limits, Root signatures, native/public/policy grants and source listening approvals remain unchanged. No new creator confirmation is needed. Whole implementation/review/fixes still precede actual game windows. AllASSET/actual32/33,WINDOWS29/30/31,index0/180 and known49consumer comparisons remain pending.

## Self-check

The five current implementation files exist; current four fixed-file hashes match the immutable R2 starting snapshot. RED commit `d70ef24` remains in ancestry. The completed raw native report exists and contains exactly656 outcomes with the five failures above. No task completion or canonical SUMMARY is claimed.
