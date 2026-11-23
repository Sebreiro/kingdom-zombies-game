# Plan04-29 partial-view Stop all checkpoint

Recorded2026-11-18T13:02:42UTC on `feature/04-asset-production`, HEAD `f72e3beefa24077dd93ca6286275dc087bcbfae9`. Both original tasks have committed tested outputs, but newly demonstrated missing Stop all reachability blocks plan completion. No canonical SUMMARY is written. No fourth Task2 correction or frozen tracked-source edit has been made.

## Demonstrated missing behavior

An independently diagnosed partial-view flow has zero selected event routes but can still restore valid current ambience from the independent interface bank. The generic `AudioPreview.Button` helper disables **all three** buttons when the selected event list is empty. Therefore the native Stop all button is disabled while two actual current loops are playing.

Relevant current production lines:

- `AudioPreview.cs:235`, `Restore(string[] routes)`: reconciles current state using all independently valid loop routes.
- `AudioPreview.cs:616`, CurrentState selector is created and remains enabled.
- `AudioPreview.cs:625`, its signal is connected to `RestoreSelection`.
- `AudioPreview.cs:629–632`, `Button(string text, Action action)` sets every button's Disabled property from selected `_visible` route availability.
- `AudioPreview.cs:716`, `RestoreSelection(long index)` invokes actual Restore with the selected state's loop IDs.

The actual native diagnostic selects AU-01 while `assets/audio/actions.wav` is unavailable; valid independent interface/ambience bytes remain readable. It selects the real enabled CurrentState `day-fire` option through its signal. Observations:

| ID | Actual observation | Outcome |
| --- | --- | --- |
| `partial-independent-current-loops` | Enabled native state selector;2actual Playing players bound exactly to `ambience.day-night.forest-day` and `ambience.fire-wind.local-fire` | true |
| `partial-current-loops-stopall-enabled` | Actual native `Stopall.Disabled=true`,2players,0selected Event items | false |

This is a production control-state failure, not merely an absent fixture, source lookup error or unreadable bridge. The disabled property prevents ordinary native button activation even though directly emitting its signal would bypass the disabled state. Do not use signal emission through a disabled button as a positive UI-reachability proof.

## Exact isolated diagnostic and limits

The observer is ignored `.phase4/04-29-partial-stopall-probe.cjs`. It creates its own directory and copies immutable passed case `431216ea-8456-4e51-8e44-fc7c948d6670/native-inventory`, verifies every copied production assembly against that case's saved closure, changes **only its test-owned InventoryHarness.Run**, and compiles the helper project without rebuilding Game/Core. It stages no tracked-source change.

Diagnostic directory: `.phase4/audio29-partial-stopall-190066f9-d938-4124-b437-f94a2a95e1e0`.

Actual helper build PID192 succeeds0warnings/0errors. Authenticated official nonconsole engine import PID33412 exits0. Native PID**29084**, nonce **`190066f9-d938-4124-b437-f94a2a95e1e0`**, writes its new matching report, prints `INVENTORY_NATIVE_COMPLETE ... FAIL`, and exits1. The observer uses the existing800frame cap; no active proof was killed. Game SHA256 **`9aea2bcefe803b762035c349e6c09e7fd6be72d10937e7a80065ca4ffa5e6710`**, Core SHA256 **`9069b999ab18911da90b8891292d2d49da0f90360fa2a7834fa7ea427fb3e62d`**. These are the immutable passed431216ea closure, not a relabeling as the later f72e3be build. Its recorded AudioPreview source bytes match the current frozen implementation.

The native report's two observations are materialized before owned controls/resources are closed and freed. **Stderr additionally reports4ObjectDB instances leaked at exit.** This observation is preserved without attributing it to production or observer ownership. The diagnostic wrapper consequently rejects its clean-log guard with `PROBE_IDENTITY_OR_LOG_FAILED`; it creates no success observation.json. No clean diagnostic pass is claimed. The raw report still contains the exact PID/nonce and specific disabled-control failure above. Any future positive must resolve/attribute the cleanup observation using actual owned lifetime evidence, without hidden stderr, forced GC, sleeps or an early quit.

| Exact report/log | SHA256 |
| --- | --- |
| `native/inventory-results.json` | `db7020ab02322d1b8626f14ca9c0e9ffb3c7db2baa16a6c895b2188af37cab28` |
| `commands.json` | `fff1762c2ebc1129c599b3971bd6d9320988981e1a0bfa78cf4cd2f31b5b75d9` |
| `native.stdout.log` | `d1a00656c48bcbce350d7ec2167a5105f13fef99d88f7485088bf5f92fc453c4` |
| `native.stderr.log` | `bf8149e7a4c220bbef9c5eac13cd97f17ae895e5fff5a7befa152e30319f4d9d` |
| `native/InventoryHarness.cs` | `7b7018b3ecd6eb668422458ec5f8bfb75bd9fc88fc9d6ded59b38b2790dfbbfc` |

Paths in this table are relative to the diagnostic directory above. The helper source and all build/import/native logs remain available there; the original passed431216ea directory was not modified.

## Safe stopped full selection

The frozen49-case selection began at f72e3be with **484exact input files**. The first20cases passed,0failed, with no relevant byte drift or HEAD changes. After this new missing flow was demonstrated, the permitted `.phase4/04-29-final/STOP-AFTER-CURRENT` scheduler file was created. The active `layouts.session` case finished naturally, passed, and the runner stopped **before `layouts.endpoints`**. No active process was terminated.

Completion:20/49executed,20pass; stopped=true,exitCode2;392371ms sum of case durations,400646ms wall; finished2026-11-18T12:59:56.772UTC. This is preserved partial history, **not** a full selected-set pass. The remaining29cases have no new result. The one required final changed policy/producer observation has **not** been run and must wait for actual final source and the repaired fresh whole selection.

| Exact stopped-selection file | SHA256 |
| --- | --- |
| `.phase4/04-29-final/selection.json` | `823d17995cae595360578069a45f52da6ca4b712b81dbb7a953694b9c179893c` |
| `.phase4/04-29-final/results.json` | `7ea900da82e761697ce729a60f2548a81590a85ba9a1cef343c88f02a87656d4` |
| `.phase4/04-29-final/completion.json` | `87261cd0a220041c71d342934f54767e9ffca09140427798ba8522e5ea29b8e5` |

Each executed case retains its actual invocation revision, immutable run UUID, stdout/stderr and any observed assembly closure. Known source.provenance historicaladb38f27 and source.publication source-inferred blockers remain explicit and were not rerun or given pass credit.

## Committed work and preserved budgets

| Stage | Commit / actual evidence |
| --- | --- |
| Original epoch RED | `d70ef24`; genuine compiled equal0epochs failure and supported fresh JUnit RED_EVIDENCE_OK |
| Generated/shared source RED | `13ffce4`; two genuine compiled intended source rejections, both RED_EVIDENCE_OK |
| Source-form/owner repair | `e5b2ccc`; all15source/ownership tests pass, including all71variants and eleven negative/restored controls |
| Task1 GREEN | `4f0583a`; lifecycle0ac39b4a passes674native observations |
| Fresh committed same-case tracer | `716d7c95-a3f1-4463-84b9-7b4f3c358293` PASS before any Task2 edits |
| Task2 RED | `c06fcfa`; actual zero selected-bus mute predicate false, intended assertion fails;15prior tests pass;RED_EVIDENCE_OK |
| Task2 GREEN for original selected contract | `f72e3be`; heldout431216ea passes794native observations(786controller+8actual-projectHub),22compiled tests0skip,strictproduction/harness0/0 |

Original Task1 corrections3/3 exhausted. R1 initial+2/2 exhausted. R2 initial+0/2 stopped at a production defect. R3.1 initial+2/2 exhausted. R4 initial passes,0/2 corrections used. R3.2 initial+C1/2 passes after adding exact configured foundation copy. **Original Task2 corrections3/3 exhausted:** C1initial continuation indentation before genuine RED;C2explicit ordinal comparer in native harness;C3independently isolate committed-event dedup from cooldown and add rebound shifted-PCM/valid-sibling controls. All prior technical checkpoints and reports remain immutable; no count is reset here.

## Exact proposed prospective repair scope

Production path proposed: **only `src/godot-game/inspection/AudioPreview.cs`, Button helper Disabled condition**. Keep Play unavailable without a valid selected event, while keeping Stop/Stop all reachable for independently valid current loops; empty stop operations can remain harmless. No route/source/mix/loop/limit/ownership semantics or other production members need changing for this demonstrated defect.

Proof path proposed: **only `tools/phase4/cases/audio.cjs`**, add a distinct independently named partial-view current-loop/Stopall reachability and deterministic stop/free regression using real native state controls and players, with specific expected loop IDs and no historical cue replay. Preserve every original lifecycle/heldout assertion,ID/order and all eight backstops. Diagnose/attribute the isolated4ObjectDB cleanup observation through correct owned observation/resource lifecycle before accepting a clean positive. This is a proposal, not authorization or an applied fix.

Root, AudioPresentationTests, fixture, media/catalog/approvals, reader limits, native/public/policy grants, project config and all other tracked paths should remain byte-fixed. A prospective bounded coordinator continuation is required **before** the next production/proof correction. Preserve all old budgets and failure records. On actual repair, run fresh affected audio cases, then a new immutable full49selection rather than resuming/renaming these20passes. Observe exactly one final changed native-policy/producer closure after actual final source, recording new tuples for23/31 without grants. Canonical SUMMARY/self-check follows actual completion only.

## Current five exact source hashes

| File | SHA256 |
| --- | --- |
| `src/godot-game/inspection/AudioPreview.cs` | `0faf0d7b6c370b5e6ede142ac72517c019a69a4e02bdc13cec65e03aeec4500e` |
| `src/godot-game/inspection/InspectionRoot.cs` | `6e831506dc3ee8b58ad59b2c857d81ad885d630e9f40839c2eafc478b0539e9a` |
| `tests/phase1/AudioPresentationTests.cs` | `be427e84f5e632415a2d74d9c4c553ef88ef1dc4958fbeae755064073bef2ba2` |
| `src/godot-game/content/fixtures/audio-states.json` | `9379b50244a47c16da9da1c613aad621af84be1e152d8996cbd938da63e57009` |
| `tools/phase4/cases/audio.cjs` | `62df07389d9823be4fb9a532bee1d71dfc480939b8bac42fb7a53def10df101f` |

No tracked source changed during selection or the isolated diagnostic. Parent owns globals/wave24post/PR5/push. Source approval remains exactly40sources/44presentedversions;27new variants stay Not auditioned. No game window, hearing, final mix, allASSET, source adoption/policy enrollment or final32/33 acceptance is claimed. WINDOWS29/30/31,index0/180 and49existing consumer comparisons remain pending.

## Self-check

All five source files, original794native report, the two-observation diagnostic and stopped-selection records exist. Relevant source hashes remain exact; current implementation commits are in ancestry. The partial control failure and cleanup warning are explicit, and no plan-complete or full-selection claim is made.
