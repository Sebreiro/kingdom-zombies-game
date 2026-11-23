# Plan04-29 R5 completed audio repair, failed hub status-filter regression

Recorded2026-11-18T13:49:43UTC on `feature/04-asset-production`, HEAD **`8d318f42fd37af0501ecbea0722b06ae3b0fab03`**. R5's scoped partial-view stop repair is committed and its complete affected audio cases pass. The subsequent NEW immutable full49selection completed with **47pass/2fail**, exposing a Root.Rows status-filter defect outside the R5 Button-only production scope. No Root fix, further source edit, policy observation or canonical SUMMARY has been made.

## Complete frozen selection

All49selected executable cases ran freshly in declared order on exactly **484unchanged inputs**. Initial/final HEAD both8d318f4;no scheduling events, no source drift, no stopped case, no reused outcome. Duration **1749355ms caseexecution**, **1769083ms wall**. Finished2026-11-18T13:47:38.519UTC. Exit1;executableSelectionPass=false;fullPhaseSelectedSetPass=false.

Every remaining independent case completed after the first failure, as required. Both original audio-source/bank cases, repaired audio.lifecycle/audio.heldout, all animation/appearance/data/layout/export/defense/world cases and graph.visibility-inventory passed. Exactly these two cases failed:

| Case | Actual immutable run | Native PID | Native outcomes | Failures |
| --- | --- | --- | --- | --- |
| `hub.source` | `9e7f2301-9b33-4c61-bf5d-0198e1c0bca5` |29236 |25:23true/2false |partial-filter; dependent details-mappings |
| `hub.lifecycle` | `af92e17d-f4aa-4647-b0a5-a58924048de0` |30552 |50:48true/2false |same two failures, no independent lifecycle failure |

Both failures occurred in the actual compiled production C# scene under authenticated native processes after their existing compiled authorization/discovery controls completed. Their recorded revision is8d318f4. No native setup failure, deadline/cap change or missing API is being accepted as a behavior assertion. The existing headless raw-PNG image comparison warning remains in their logs and is not hidden; their specific assertion failures are below.

## Exact production diagnosis

Current `src/godot-game/inspection/InspectionRoot.cs`, Rows, lines420–425:

```csharp
_rows = outputs.Concat(second: expected).Concat(second: authored).Concat(second: AudioPreview.Ids(
        category: category).Where(predicate: id => id.Contains(value: _query,
                comparisonType: StringComparison.OrdinalIgnoreCase))).Distinct(comparer: StringComparer.Ordinal)
        .Order(comparer: StringComparer.Ordinal).ToArray();
```

The added Audio IDs apply the query predicate but **omit the existing status predicate**. In Register/category-empty view with Partial selected and empty query, all30AU rows are injected despite their current row label being Incomplete. The actual `partial-filter` observation is31rows: AU-01 throughAU-30 explicitly labeled Incomplete, plus common.bow labeled Partial. The unchanged test requires one Partial row and then `choose("common.bow")`.

The following `details-mappings` failure is dependent: its preceding `rows().size()==1 && choose("common.bow")` short-circuits at31rows, so it never selects common.bow. Metadata remains the earlier IT-07 text `Bow, 2×4\nSource: specs/0.1/assets-and-resources.md#IT-07`, rather than the common.bow entry containing `Explicit shared mappings: IT-07`. Do not weaken either original assertion or merely change its count/expected metadata.

Audio rows currently remain Incomplete because actual pack/index adoption and acceptance are23/31/32/33-owned. The existing expected-row path already handles All statuses/Incomplete/Error. A precise prospective repair should apply that same status eligibility to the added Audio ID projection, while preserving its current query/category predicates and identity ordering. The world-authored path has its own established All statuses/Partial eligibility; do not alter it.

## Required prospective path scope

Proposed production path: **only `src/godot-game/inspection/InspectionRoot.cs`, existing Rows Audio-ID Where predicate**. No new or changed Root method/signature, no Read/cache behavior change, no root ownership/generation or new native API. Keep existing `_files` validation-only snapshot and fresh bounded Read byte-identical; the prepared read-only authored inventory confirms Read remains byte-identical to643d8eb.

Proposed proof path: **only `tools/phase4/cases/audio.cjs`**, add independently named actual-project hub Audio/Register status-filter regressions using stable option labels and existing real UI signals. Prove Partial excludes Incomplete Audio rows, normal All/Incomplete eligibility keeps all30IDs, and filtering never substitutes old metadata; restore ordinary native controls before following existing assertions. Preserve every existing hub.source/hub.lifecycle and audio lifecycle/heldout predicate/ID/order. Keep `tools/phase4/cases/hub.cjs` and its original expectations fixed.

Do not change AudioPreview, AudioPresentationTests, fixture, media/catalog/approval/limits, project config, policy/native/public grants or other implementation paths for this defect. A prospective coordinator record must bound the actual repair before any edit. **R5 does not own Root.Rows**; its unused correction capacity is not permission to expand scope or reset an older budget.

After an authorized repair, run actual affected failed/new checks, then an entirely NEW immutable full49selection on repaired source. Neither these47passes nor the prior stopped20 replace that final pass. Preserve all old records and actual per-case revisions/closures. Only after the repaired whole selection passes, run exactlyONE current changed policy/producer observation and hand off complete new Audio/modifiedRoot tuples and actual generated inputs to23/31 without grants. Canonical SUMMARY/self-check follows actual completion.

## R5 implemented and verified scope

Commit **`8d318f4`**, `fix(04-29): keep stop controls reachable for partial audio loops`, changes exactly two paths: AudioPreview.Button Disabled condition now disables only Play when no selected event is valid; audio.cjs adds six distinct native partial-view/owned-resource regressions. Root/test/fixture stayed byte-identical throughoutR5. Snapshot `.phase4/audio29-r5-checkpoint-ae3e593/` retains all five pre-edit bytes and independently verified hashes.

Actual partial scenario uses enabled native CurrentState `day-fire` with missing selected action bank,0event items, exact2Playing forest-day/local-fire state-loop IDs. It proves Play disabled, Stop/Stopall reachable, activation of the **enabled** Stopall stops/frees every owned player, historical one-shot count stays0, and repeated empty stop operations are harmless. Original IDs/predicates/order/eightbackstops/22compiled tests remain.

The observer captures real stream/playback identities while objects are alive and explicitly disposes its acquired playback observation handles. At the existing normal post-backstop teardown point, exact4captured native IDs(twoAudioStreamWav/twoAudioStreamPlaybackResampled) are invalid. IDs are invariant decimalstrings to preserve full64bit identity through JavaScript JSON. No extra wait, sleep, GC, frame padding, process cap or deadline was added. Full current cases complete naturally with clean logs.

The original tiny diagnostic's four ObjectDB warnings remain **unattributed historical evidence**. A new verbose immutable copy reproduced the disabled-stop assertion but not that warning, which is not proof of its cause. Current named-object retirement and clean full-case logs are actual current evidence, not retroactive attribution or a reclassification of the failed diagnostic.

| R5 affected proof | Run | Result |
| --- | --- | --- |
| Initial lifecycle |3df0b167-e136-4e82-ae06-876d9b805ca4 |680/680native;22compiled0skip;strictproduction/harness0/0;clean |
| Initial heldout |7e87f1ab-1812-408c-927d-b0ab5a2770e2 |800/800native;22compiled0skip;clean |
| C1 exactID lifecycle |0e53c18d-784f-4360-a0ef-7d8ff79e3fe9 |680/680native;exactstringIDs;clean |
| C1 exactID heldout |5266594e-2c45-44a4-b740-cf6050433bfb |800/800native;exactstringIDs;clean |
| Fresh full-selection lifecycle |12926a82-ca2c-40ef-a252-39aaf593ef03 |680/680;controllerPID11848,actualHubPID22180;clean |
| Fresh full-selection heldout |a2f064d5-0b13-4d90-b240-6d5c198f20c8 |800/800;controllerPID3536,actualHubPID17084;clean |

## Exact current five implementation hashes

| File | SHA256 |
| --- | --- |
| `src/godot-game/inspection/AudioPreview.cs` | `f64d4c89a30b5b896ca418d52731c8182f5b06799c005cc7ebf204393c382d4f` |
| `src/godot-game/inspection/InspectionRoot.cs` | `6e831506dc3ee8b58ad59b2c857d81ad885d630e9f40839c2eafc478b0539e9a` |
| `tests/phase1/AudioPresentationTests.cs` | `be427e84f5e632415a2d74d9c4c553ef88ef1dc4958fbeae755064073bef2ba2` |
| `src/godot-game/content/fixtures/audio-states.json` | `9379b50244a47c16da9da1c613aad621af84be1e152d8996cbd938da63e57009` |
| `tools/phase4/cases/audio.cjs` | `d2d3f922e2a3c996956fa993ce99e899867cf475d7168cd3212157bfbf21edc8` |

## Immutable report hashes

| Exact path | SHA256 |
| --- | --- |
| `.phase4/04-29-final-r5/selection.json` | `2af9efd92a36e43e1c58d0b63351427c9bece2e7febbac48142a713d8228ad28` |
| `.phase4/04-29-final-r5/results.json` | `1efd6b1ce143087656fccc69a7abd436c43ee23651dcf97c71b165ead7ad095b` |
| `.phase4/04-29-final-r5/completion.json` | `32faa26a0b395b643cee11584fbf2b59a45049753834e46e79ef03e0a8d09677` |
| `.phase4/runs/9e7f2301-9b33-4c61-bf5d-0198e1c0bca5/result.json` | `9ad2147b8641ed5f78d34303e62369b0834126743aa1e07166820a5bb5365fe9` |
| `.phase4/runs/9e7f2301-9b33-4c61-bf5d-0198e1c0bca5/native-project/native-results.json` | `92e4ce68ee2cdd857abf6af8ca62a726b01e05680be1e06a5ea0a7ac849bea9e` |
| `.phase4/runs/9e7f2301-9b33-4c61-bf5d-0198e1c0bca5/compiled.command.json` | `18174d1d940e1e27d3900ff5798c2bb4c9f93302e9aa83cec0864753325da234` |
| `.phase4/runs/af92e17d-f4aa-4647-b0a5-a58924048de0/result.json` | `065cedc9bc25eba444844b85c47c52c5144f782a725059c64822535303a40649` |
| `.phase4/runs/af92e17d-f4aa-4647-b0a5-a58924048de0/native-project/native-results.json` | `df10201492fe5737fa8855aa8712eb41db0fefe3a6542095b976d0e0daa208f6` |
| `.phase4/runs/af92e17d-f4aa-4647-b0a5-a58924048de0/compiled.command.json` | `3bea5133537944ca7ff5f585d76b6b5d8d4cb29abef587184ed702060f3f8aca` |

## Budget/history and remaining obligations

OriginalTask1=3/3 exhausted;R1 initial+2/2 exhausted;R2initial+0/2 stopped at source defect;R3.1initial+2/2 exhausted;R4initial+0/2 passed;R3.2initial+C1/2 passed;originalTask2=3/3 exhausted. **R5initial+C1/2 passed its scoped audio repair**;C1 only preserved exact64bit observer IDs and scenario description. No second R5 correction or Root change was made. Preserve every original RED/failed setup/source/native report, source fix, originalTask1GREEN4f0583a/freshcommittedtracer716d7c95 and originalTask2GREENf72e3be.

Known source.provenance adb38f27 and source.publication source-inferred blockers remain explicit omissions with23/31 ownership; they were not rerun, changed or given pass credit. The previous `.phase4/04-29-final/` stopped20run remains separate immutable history. The prepared `.phase4/04-29-final-r5/observe-final.cjs` has not been executed; its full49pass guard would reject this failed selection. No current policy/producer result or new tuple count is fabricated. Read-only authored-symbol inventory exists but is not policy enrollment.

No global planning state, media/catalog, source approval, project/policy/reader bound, public identity or grant was changed. Creator source approval remains exactly40sources/44presentedversions;27new variants stay Not auditioned. No actual game window, hearing, native mix, allASSET, pack adoption or final32/33 acceptance is claimed. Parent owns globals/wave24post/PR5/push. WINDOWS29/30/31,index0/180,49existing consumer comparisons and exact64MiBarchive consumer reconciliation remain pending.

## Self-check

All49results and both raw native failure reports exist, match the recorded hashes and actual source revision, and contain only the two dependent failure IDs above. Current five source hashes are unchanged from committed8d318f4. No active selected proof remains. The plan is explicitly incomplete pending prospective Root predicate repair, repaired fresh whole selection, final current policy/producer inventory and canonical SUMMARY/self-check.
