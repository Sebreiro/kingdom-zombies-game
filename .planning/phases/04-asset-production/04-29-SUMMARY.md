---
phase: 04-asset-production
plan: "29"
subsystem: native-audio-inspection
tags: [godot, csharp, pcm, ownership, source-headless, native-audio]
status: complete
requires:
  - phase: 04-21
    provides: Two current PCM banks, 71 samples, 30 categories and 91 canonical routes
  - phase: 04-26
    provides: Guarded existing inspection Root and native shell
  - phase: 04-27
    provides: Existing bounded current reads and world inspection ownership
provides:
  - Native Audio page with deliberate audition and exact current-state loop ownership
  - Configurable admission, deterministic variants, mute warnings and full source diagnostics
  - Two Critical saved cases, 22 compiled tests and fresh 49-case source/headless pass
  - Exact 129-new-tuple and current 58-input/32-surface handoff without policy grants
affects: [04-22, 04-23, 04-24, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [Globally monotonic ownership fence, Explicit audition, Reconciled current loops, Actual PCM comparison]
key-files:
  created:
    - src/godot-game/inspection/AudioPreview.cs
    - tests/phase1/AudioPresentationTests.cs
    - src/godot-game/content/fixtures/audio-states.json
  modified:
    - src/godot-game/inspection/InspectionRoot.cs
    - tools/phase4/cases/audio.cjs
key-decisions:
  - Keep AudioPreview internal and preserve all existing authored Root signatures and Read bytes.
  - Validate actual ordinary/generated/shared source forms against current authority before native attachment.
  - Keep partial-view stop controls reachable for independently valid loops.
  - Apply existing status eligibility to Audio rows without weakening original Hub regressions.
  - Record ungranted native operations and changed producer inputs for 04-23/31.
requirements-contributed: [ASSET-01, ASSET-02, ASSET-04]
requirements-completed: []
actuals:
  tokens: 29824
  tasks: 2
  commits: 22
  diff_chars: 119294
  scale: "ceil(chars/4) over the exact five-path realized Git diff; not harness tokens"
commits: 22
plan_head_before: 643d8eb5d526d62ee8e3ce0c66a4895b01d2a91b
plan_head_after: ecc20cd0e42a503eaeea7e78de0fa10d13c7b285
duration: "181.5677min elapsed"
duration_scope: "First preserved compiled RED through final selected-set completion, including coordinator waits"
selected_case_duration_ms: 1749413
selected_wall_duration_ms: 1769167
completed: 2026-11-18
---

# Phase 04 Plan 29: Native audio audition and current loop ownership summary

**The actual native Audio page deliberately auditions all 91 current routes, reconciles owned current loops without historical cues, preserves muted warnings, and passes a fresh 49/49 executable source/headless selection.**

This completes the local implementation and automated proof contract. All ASSET requirements, source/pack integration, native permission reconciliation, independent phase review, real game windows, hearing, loop-seam quality and final mix/readability judgments retain their separate owners. No automated outcome is a listening verdict.

## Delivered behavior

The Audio page exposes Play, Stop, Stop all and explicit Loop; event/source/version/exact media/mix details; Playing/Stopped/Muted, elapsed time and Master/Effects/Ambience controls. Selection alone creates no audible audition. The current owner holds actual AudioStreamPlayer and AudioStreamWav objects; selection/category/recheck/cancel/scene teardown closes acceptance, stops players, detaches and frees resources. Main-thread guards precede native entry. The process-wide Interlocked epoch cannot revive an older A after A→B→A replacement, even when a local state repeats.

Current-state restoration reconciles the exact desired loop identities once. Repeating a restore preserves the current players rather than accumulating loops; transactional/death/warning history is never replayed. Explicit auditions use current source variants, route priority/cooldown/repetition constraints and committed-event identities. Defaults remain 24 global and four per effect, with independent configured endpoints 1/7/128 and 1/2/24, constrained by each actual route's maxOverlap. Owned seed 290029 supplies bounded draws excluding the immediately preceding choice. Volume-zero buses and explicit mute retain the complete critical visual warning and “Sound muted”.

The two dependency banks remain exactly 9,994,132 and 13,107,808 bytes. Runtime uses their actual canonical 44-byte WAV headers, integer frame regions, current bank/sample hashes and native stream Data/format/loop boundaries. Existing 16MiB binary and 1MiB JSON bounds remain distinct and unchanged. Runtime does not load retained production ZIPs. Generated food provenance requires actual AU19 donor-gap authority, seed190021 and retained recipe identity; shared action.2790.full requires the canonical action pack/current source WAV hash. Ordinary/inline/donor references remain unique and coherent. Unknown, ambiguous, wrong-valid-reference/hash/authority/recipe inputs reject specifically; independent valid siblings remain usable with exact file/field/value/reason diagnostics.

R5 keeps Stop/Stop all enabled beside zero selected action events and two independently valid day-fire loops, while Play remains disabled. Native activation of the enabled Stop all frees the exact players; an empty repeated stop is harmless. Observer-owned playback handles are disposed explicitly, and captured exact 64bit native stream/playback IDs retire after normal owned teardown. The original four-object diagnostic warning remains unattributed historical evidence; current clean full-case/resource-identity evidence does not invent its cause.

R6 adds only the existing All statuses/Incomplete/Error eligibility to the Audio-ID projection in Rows. Partial no longer injects 30 Incomplete AU rows into Register; the unchanged original assertions select the sole common.bow and expose “Explicit shared mappings: IT-07”. Seven additional actual UI outcomes independently exercise All/Incomplete/Error, Audio Partial exclusion, Register Partial, genuine selected metadata and restored controls using stable option labels.

## Task commits and TDD gate compliance

| Task | Genuine compiled RED | GREEN and feedback |
| --- | --- | --- |
| 1: native page/current ownership | `d70ef24`; .phase4/audio29-red1-supported/tests.xml executes production NextEpoch twice and rejects equal zero epochs, 1 failure/0 errors/0 skips | `4f0583a`; fresh committed SAME-CASE tracer `716d7c95-a3f1-4463-84b9-7b4f3c358293` passed before Task2 |
| 1 source-form repair | `13ffce4`; actual helper used by LoadPack rejects valid generated/shared sources before correction; .phase4/audio29-r3-source-red retains genuine intended failures | `e5b2ccc` validates all 71 actual variants plus independent negative/restored controls |
| 2: independent endpoints/restores/mute | `c06fcfa`; .phase4/audio29-task2-red/tests.xml has 15 passing prior tests and one assertion rejecting actual IsMuted(false,80,0)==false | `f72e3be`; independent held-out native proof and 22 compiled tests |
| Partial stop repair | Actual native diagnostic `190066f9-d938-4124-b437-f94a2a95e1e0` proves disabled Stop all beside two playing loops | `8d318f4`; clean 680/800 outcomes before R6 and six separate stop/resource regressions |
| Existing Hub status repair | Original `hub.source 9e7f2301` / `hub.lifecycle af92e17d` fail partial-filter and dependent details-mappings without changed observers | `ecc20cd`; unchanged Hub cases and seven new native UI outcomes pass, then entirely new 49-case pass |

Saved RED records were accepted by the SDK evidence check and semantically inspected. Strict compiled targets executed actual production contracts; missing assemblies/APIs, unsupported command/setup failures and absent completion markers received no RED or GREEN credit. The first source lookup RED commit also preserves then-unaccepted controller work; it is not labeled a complete native GREEN. No behavior expansion preceded the fresh committed Task1 tracer.

## Fresh final verification

Final source HEAD is **`ecc20cd0e42a503eaeea7e78de0fa10d13c7b285`**, branch **feature/04-asset-production**, isolation none. All **484 exact inputs** remained unchanged. Initial/final HEAD match; there were no scheduling events, drift, stopped cases or reused outcomes. The 48 Phase04 cases plus graph.visibility-inventory ran freshly in declared compatible ordered groups. Their separate native processes are required for different project registries, deliberately mutated owned copies and existing compiled authorization controls; no process is claimed to observe another group's objects.

| Final Audio case | Run | Controller PID/nonce | Actual-project Hub PID/nonce | Native outcomes |
| --- | --- | --- | --- | --- |
| `audio.lifecycle` | `0d00857f-4198-4f33-be44-52eb4895d124` | 21492 / `0d00857f-4198-4f33-be44-52eb4895d124` | 26248 / `0d00857f-4198-4f33-be44-52eb4895d124.hub` | 687/687 |
| `audio.heldout` | `acd3f76e-17fb-4a5f-aa36-c73239dc7415` | 36600 / `acd3f76e-17fb-4a5f-aa36-c73239dc7415` | 4316 / `acd3f76e-17fb-4a5f-aa36-c73239dc7415.hub` | 807/807 |

Both cases record exact current source inputs, separate authenticated official nonconsole engine identities, actual production game/Core assembly closures and process-qualified outcomes. Controller and actual registered KingdomZombies scene groups complete naturally; duplicate/missing/failed outcomes reject the merge. The pinned native engine is 4.7.2.stable.mono.official.ed1daf0bf, executable SHA256 `45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab`, authenticated archive SHA256 `a2a48473a7414c5f19fab690518caebb738c09ef9601f6bd2388676a7f53b3c0`. No console surrogate, forced GC, delay, frame padding, expanded cap/deadline, hidden stderr or early quit supplies a pass.

Fresh Audio strict production/harness builds report **0 warnings/0 errors**; **22 compiled tests pass, zero skips**. All 71 variants and all 91 routes use actual players and independently compared exact PCM/source info, including AU19 two routes and AU29 one route. H19 covers priority/admission/source overlap/cooldown; H25 covers independently chosen loop sets and repeated restore without old cues; H26 covers eleven owners, category/audition/state replacement, obsolete callbacks and current bindings. Wrong current hash, rebound invalid gain and shifted PCM have specific rejection assertions plus independent sibling/restored positives.

All eight AUDIO-AUDITION backstops are separately named, saved and asserted: empty, loading, error, populated, partial, overflow, zero-one-many and long-text. The held-out case repeats all eight independently. Long metadata is tested after selecting the intended stable route; muted auditions are legally admitted under the real cooldown before testing volume/warning and immediate-repeat rejection. Reversible partial failures remain usable beside independent valid views.

The two unchanged final Hub cases pass their original 25/25 and 50/50 native outcomes and compiled authorization controls. Their historical image-file loader warning remains visible in logs; the new Audio controller and actual-project Hub logs are clean. This is source/headless playback/state/lifetime evidence, not audible quality or final native mix approval.

The full 49-case selection completed at **2026-11-18T14:42:14.110Z**, with **1749413ms case execution / 1769167ms wall**. Full phase acceptance remains false because known integration/actual judgment owners remain pending.

| Order | Case | Run | Duration ms |
| --- | --- | --- | --- |
| 1 | `export.layout` | `3048c908-b389-4583-8ac8-2f10cedd89b7` | 26005 |
| 2 | `audio.feasibility` | `8df9ffc8-2f11-4893-8dcd-fee23e73a498` | 9493 |
| 3 | `media.conversion` | `ece30e9d-301f-4869-83e8-78cf45bfbd68` | 43482 |
| 4 | `media.scale` | `3dbf89af-108a-41a6-acfd-804c7f75a79b` | 8249 |
| 5 | `items.weapons-food` | `be0d903d-fd5e-4eeb-ad87-5cb70ea8a726` | 8051 |
| 6 | `items.wearables` | `ac974961-63fc-48af-8f76-56b3818b9d73` | 13348 |
| 7 | `buildings.main-art` | `23751d3e-c1b7-4db4-8464-6b13068859ee` | 5104 |
| 8 | `buildings.main-state` | `4bf81f24-95fd-4638-9d5c-632f099f3a9f` | 26251 |
| 9 | `bodies.a-art` | `8c1e9123-c6ef-4cb8-a29a-7888bf86878e` | 8032 |
| 10 | `bodies.a-fit` | `e7917881-fe93-4e18-9cda-dedbbabcb516` | 27439 |
| 11 | `bodies.b-wide` | `5cb166eb-6f76-4eaa-8897-ef4ac25d2d1b` | 45886 |
| 12 | `bodies.complete-fit` | `be1b3216-d0cb-4dd2-9460-5efc7f4704f4` | 48962 |
| 13 | `layouts.theme` | `89f5bf66-c00a-4b36-ad3f-379365b7ba4a` | 14530 |
| 14 | `layouts.identity` | `bbe51175-058b-4977-ad6d-a6ae3ced321c` | 12554 |
| 15 | `layouts.identity-integrity` | `fb0ab2b9-1510-4296-9cab-56a1a627546b` | 14771 |
| 16 | `layouts.inventory` | `3b447fc9-71b0-4caa-a006-42493640d5bb` | 16190 |
| 17 | `layouts.legacy-target` | `8a868d15-3fc0-4515-b60f-f8d49cfe09e4` | 18329 |
| 18 | `layouts.camp` | `122a0e0d-8145-44f8-a7e8-a8b21e624890` | 14883 |
| 19 | `layouts.map-warnings` | `d2ce0e29-7850-4274-89a4-c417ef519525` | 15221 |
| 20 | `layouts.session` | `a3920b04-ebb5-416c-8444-0c74719095fd` | 14765 |
| 21 | `layouts.endpoints` | `785495e9-8a78-4d00-a8ea-d60bf102d698` | 19460 |
| 22 | `animation.melee` | `318295ce-82f1-40e5-827e-cc28944601f6` | 24960 |
| 23 | `animation.replacement` | `e23bc3a1-04de-4952-b9fb-2ea970733729` | 24813 |
| 24 | `animation.bow` | `f154565e-a73c-496e-b73e-5c88eecc798d` | 45099 |
| 25 | `animation.work-special` | `a8f49688-91af-45eb-8dd3-0daaa320900e` | 25990 |
| 26 | `appearance.corpses` | `31b4cae0-c312-4c59-9c6b-8fe7371347f1` | 25822 |
| 27 | `appearance.crowds` | `699296da-2b93-4b86-97a5-a1fff29e5b83` | 31295 |
| 28 | `audio.actions` | `f7535dfb-e62d-417a-86aa-76d41aeb8eec` | 2776 |
| 29 | `audio.bank` | `088b593b-d758-49e5-86b5-2b809d25d2cc` | 5996 |
| 30 | `audio.lifecycle` | `0d00857f-4198-4f33-be44-52eb4895d124` | 41717 |
| 31 | `audio.heldout` | `acd3f76e-17fb-4a5f-aa36-c73239dc7415` | 59789 |
| 32 | `data.foundation` | `0c1d172f-97ef-4bb3-9681-e2fc44b75dbd` | 491 |
| 33 | `data.items-combat` | `8f0c8235-2a03-43c2-801b-677f0cba104f` | 6818 |
| 34 | `data.economy` | `f83377f4-f298-41f2-b83f-7f67d6195933` | 1034 |
| 35 | `data.world` | `8a07a849-871e-4eaa-95fb-ae610b94577f` | 3089 |
| 36 | `data.zombies` | `1f041564-77c5-4bd5-a48c-8b7b52278c93` | 63887 |
| 37 | `data.remaining` | `7469d9d7-8f4d-4570-ad2a-d962637a87aa` | 3561 |
| 38 | `buildings.defense-art` | `9e76aa42-d59a-496f-88ac-93171f7abf0c` | 2077 |
| 39 | `buildings.defense-state` | `7e4b940f-868f-4f90-90b3-1537dd7638d0` | 11288 |
| 40 | `export.profile` | `e5fb486b-f749-4090-a6d1-361e240aeed1` | 9001 |
| 41 | `export.profile-boundaries` | `4a902bbe-1853-46a5-bea0-100496e0fc91` | 44481 |
| 42 | `hub.source` | `b27e80a4-a2da-418a-b35f-ccbc3362ca94` | 395071 |
| 43 | `hub.lifecycle` | `3060511f-aad6-4102-b066-2204967e758f` | 396771 |
| 44 | `source.case-discovery` | `1accced9-be1b-451a-b4f3-b1ae8008f3ee` | 744 |
| 45 | `world.map` | `6406d340-a7cb-4215-96d9-f12402cb066e` | 6217 |
| 46 | `world.effects` | `12965f19-c8d4-4420-b036-4c7c6f106e96` | 20667 |
| 47 | `world.inspector` | `ea682a2a-441c-4418-8b99-0d4787cd7c89` | 21365 |
| 48 | `world.replacement` | `4fa2b038-e826-41c0-aee9-ae68ba8bca17` | 31003 |
| 49 | `graph.visibility-inventory` | `bce8e142-04f9-47e8-851d-0a9e9eb59440` | 32586 |

## Preserved failures, deviations and correction budgets

All historical reports and prospective amendments remain immutable. The original stopped20 selection at f72e3be and the later complete47pass/2fail selection at8d318f4 remain separate; neither provides a pass in the repaired final selection. [Repair authority](04-29-REPAIR.md), [technical checkpoint](04-29-TECHNICAL-CHECKPOINT.md), [R1 native checkpoint](04-29-R1-NATIVE-CHECKPOINT.md), [R2 source checkpoint](04-29-R2-PRODUCTION-CHECKPOINT.md), [R3 owner checkpoint](04-29-R3-SOURCE-CHECKPOINT.md), [partial stop checkpoint](04-29-PARTIAL-STOPALL-CHECKPOINT.md) and [R5 selected-set checkpoint](04-29-R5-REGRESSION-CHECKPOINT.md) retain exact original source/report hashes, diagnoses and scope.

| Stage | Preserved original count | Outcome / exact bounded change |
| --- | --- | --- |
| Original Task1 | 3/3 exhausted | Durable checkpoint before fourth correction |
| R1 | Initial +2/2 exhausted | Exact adjacent fluent indentation; first native project binding setup remained blocked |
| R2 | Initial +0/2 | Actual-project Hub context; stopped at demonstrated production source-form defect |
| R3.1 | Initial +2/2 exhausted | Actual generated/shared provenance resolution and genuine compiled regressions; stopped at omitted exact donor owner |
| R4 | Initial +0/2 | Add only sharedassets2.assets to exact owner set; all source positives/negatives/restored pass |
| R3.2 | Initial +C1/2 | Legal mute cooldown, stable long-text route, exact actual-project copied inputs; Task1 native GREEN |
| Original Task2 | 3/3 exhausted | Conditional format, explicit ordinal native comparer, legal dedup/cooldown and shifted-PCM controls; held-out GREEN |
| R5 | Initial +C1/2 | Button reachability; explicit observer disposal and exact ID serialization; scoped audio GREEN; outside-scope Rows defect checkpointed |
| R6 | Initial +0/2 | Existing Rows status eligibility and seven UI regressions; all affected checks then fresh49 pass |

These are prospective coordinator technical retries, not new creator/hearing approvals. No budget was reset and no fourth original correction was applied. [Rule1 - Bug] source-form/owner compatibility, partial stop reachability and status filtering were fixed only within their committed exact scope. [Rule3 - Blocking issue] native harness/compiler/copied-input/observation setup corrections preserved original IDs/order/expected predicates. Root method/signature cohort, project/policy/public/native grants, media/catalog/source approval and global reader limits were never expanded.

## Known pending integration and acceptance

Known `source.provenance` historical `adb38f27-592b-4700-880c-3fa1d65d0c15` remains blocked by WINDOWS29 PARTIAL_PACK_REQUIRED/common.bow→Inspection.tscn hash mismatch. `source.publication` remains source-inferred blocked through the same unchanged validate call; no direct failed invocation is invented. Both were explicitly omitted without rerun/pass credit. source.packaged-expectations and export.feasibility remain future/integration omissions from the inherited justified selection. Other unselected Phase1/2/3 cases retain explicit selection reasons in selection.json; this is not full catalog/other-OS/LAN/release certification.

All 49 existing consumer comparisons remain 04-23 work: 18 repair/map, 30 canonical sound categories, one old stain8 versus20. Unclassified hit/death/regeneration/impact mappings remain pending. The exact retained interface ZIP is 64,820,348 bytes; canonical local tools-only64MiB allowance and member32MiB/expanded128MiB remain distinct from global content/provenance32MiB. Exact 04-23/31 reconciliation is unperformed here. No media or archive member changed.

Creator “All is approved” remains bound only to the original21-presented40sources/44version1 adaptations and [exact approval/resume record](04-21-APPROVAL-AND-RESUME.md). No personal identity, device, hearing time or observations were invented. All27 new variants remain **Not auditioned**, including shared derived burst and generated food. Actual listening, sample suitability, loop seams and final mix remain04-32/33. No real game window, physical input, audible hearing, native mix/readability, pack adoption or final ASSET acceptance is claimed.

WINDOWS29/30/31, index0/180, inventory236, census180, 24AC/32D/104H+UI/135requirements and exact19 public production identities remain unchanged. Global PROJECT/STATE/ROADMAP/REQUIREMENTS, other phase history and PR5 draft remain parent-owned. No global update, push or PR transition was performed by this executor.

## Final policy and producer handoff — no grants

Exactly ONE final changed policy observation ran after the fresh49 pass: **`.phase1/runs/db114983-a882-4e7f-9b11-114c86bfc8d3`**, status **FAIL**, exit1, **2259** rejected operations. Historical2130 remains preserved; all **129** additional semantic tuples are actual AudioPreview operations, listed below. No policy/producer access grant or passing policy result is implied. Root has zero rejected tuples under its existing exact guarded18-method authority; this says nothing about adding authority for new methods. Its changed source hash remains part of the current unenrolled producer closure.

Root's only added authored member is `private AudioPreview? _audio`. Existing bodies modified are `_Process(double)`, `_ExitTree()`, `Recheck()`, `Cancel()`, `ChangeCategory(long)`, `Select(long)`, `Rows()` and `Selection()`. New domain calls are AudioPreview.Tick/Close/Ids/Handles/Open/Status and the Read delegate binding. The added native status projection is the existing `GetNode<Label>(path: "Summary/Status").Text` expression at current line474 inside unchanged Selection(); it stays within the existing compiled guarded Root authority and was not presented as a new unenrolled tuple. No Root method/signature changed; Read is byte-identical to643d8eb, always reads fresh bounded current bytes, and _files remains validation-only preload data.

Current evaluated producer is **Godot.SourceGenerators/4.7.2**, exact package **`.phase1/feed/Godot.SourceGenerators.4.7.2.nupkg`**, SHA256 **`ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e`**. It has **58 source inputs /32 generated surfaces**, compared with historical57/32. Actual new/changed input hashes are AudioPreview `f64d4c89a30b5b896ca418d52731c8182f5b06799c005cc7ebf204393c382d4f` and Root `67298eed12165964954bdf251657b0b89f3b0707dbd7fe7f3a6972404c28a1cc`. Source bytes/contentBase64, exact generated hashes and actual assembly closures are retained in generated-input-observation.json. No old producer report/package was overwritten. All current inputs/surfaces are inventoried below for 04-23/31; observation is not enrollment.

## Artifacts this phase produces

These are the exact five implementation paths; no retained bundle/media resource/subresource or new public production identity was added. Existing source media, original21 producer exports and catalog bytes remain unchanged.

| Exact file | SHA256 | Bytes |
| --- | --- | --- |
| `src/godot-game/inspection/AudioPreview.cs` | `f64d4c89a30b5b896ca418d52731c8182f5b06799c005cc7ebf204393c382d4f` | 42203 |
| `src/godot-game/inspection/InspectionRoot.cs` | `67298eed12165964954bdf251657b0b89f3b0707dbd7fe7f3a6972404c28a1cc` | 23894 |
| `tests/phase1/AudioPresentationTests.cs` | `be427e84f5e632415a2d74d9c4c553ef88ef1dc4958fbeae755064073bef2ba2` | 10397 |
| `src/godot-game/content/fixtures/audio-states.json` | `9379b50244a47c16da9da1c613aad621af84be1e152d8996cbd938da63e57009` | 4956 |
| `tools/phase4/cases/audio.cjs` | `5528877886c4063f837f6fbe99df7d987a4feb7d66598c57a818ea2f890070c4` | 132786 |


Fixture `audio-states` fields are schemaVersion, id, bindings, states, evidenceScope, backstops, warnings, variantSeed and heldout. Its three bindings pin current events/action/interface manifests. States are quiet(empty), day-fire(forest-day/local-fire), night-wind(forest-night/wind), portal(active-loop); saved backstop identities and heldout setups remain distinct.

audio.cjs retains its prior producer exports and adds `AUDIO_HARNESS`, `audioPreviewFiles`, `lifecycle`; registers Critical `audio.lifecycle` order3 and `audio.heldout` order4 in G0-audio-native. Private proof helpers/constants are AUDIO_FIXTURE, AUDIO_HARNESS, AUDIO_HELDOUT_METHODS, AUDIO_HUB_DRIVER, AUDIO_HELDOUT_HUB, audioPreviewFiles, audioHub, controllerOutcomeIds and lifecycle. The new actual-project GDScript helpers select_label, row_ids and filter_regressions use real native UI signals; the standalone harness is ignored proof staging, not an added game assembly type. Original IDs/predicates/order and independent expected values stay intact.

### `src/godot-game/inspection/AudioPreview.cs`

| Line | Actual authored declaration |
| --- | --- |
| 19 | `public void Dispose()` |
| 24 | `internal AudioPreview(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read, string category)` |
| 34 | `internal long Epoch => _epoch;` |
| 36 | `internal string Status => !_visible.Any(predicate: id => _routes.ContainsKey(key: id))` |
| 40 | `internal static long NextEpoch()` |
| 45 | `internal static bool Accepts(long completion, long current, bool accepting)` |
| 50 | `internal static bool IsMuted(bool muted, double master, double channel)` |
| 55 | `internal static int VariantIndex(int count, int previous, uint draw)` |
| 67 | `internal static string[] Ids(string category)` |
| 73 | `internal static bool Handles(string id)` |
| 78 | `internal static AudioPreview Open(InspectionShell? shell, MainThreadGateway gateway, Func<string, byte[]?> read, string category)` |
| 88 | `internal static byte[] ExtractPcm(byte[] bytes, JsonElement bank, JsonElement sample)` |
| 128 | `internal static string ResolveSource(JsonElement pack, JsonElement sample, JsonElement actions)` |
| 146 | `internal void Tick(double delta)` |
| 165 | `internal void Close()` |
| 189 | `internal bool Trigger(string routeId, string eventId, long epoch, bool loop, bool restoring)` |
| 235 | `internal void Restore(string[] routes)` |
| 264 | `private const string Fixture = "content/fixtures/audio-states.json";` |
| 265 | `private const string Events = "content/audio/events.json";` |
| 266 | `private const string SourceFile = "content/packs/audio-interface-ambience.json";` |
| 267 | `private static long _nextEpoch;` |
| 268 | `private readonly Node2D _root;` |
| 269 | `private readonly MainThreadGateway _gateway;` |
| 270 | `private readonly Dictionary<string, JsonElement> _routes = new(comparer: StringComparer.Ordinal);` |
| 271 | `private readonly Dictionary<string, (JsonElement Sample, JsonElement Bank, string Source)> _samples = new( comparer: StringComparer.Ordinal);` |
| 273 | `private readonly Dictionary<string, byte[]> _banks = new(comparer: StringComparer.Ordinal);` |
| 274 | `private readonly Dictionary<long, (AudioStreamPlayer Player, AudioStreamWav Stream, string Route, string Sample, int Priority, bool State)> _players = [];` |
| 276 | `private readonly Dictionary<string, double> _last = new(comparer: StringComparer.Ordinal);` |
| 277 | `private readonly Dictionary<string, int> _variants = new(comparer: StringComparer.Ordinal);` |
| 278 | `private readonly HashSet<string> _committed = new(comparer: StringComparer.Ordinal);` |
| 279 | `private readonly List<string> _errors = [];` |
| 280 | `private readonly List<string> _visible = [];` |
| 281 | `private JsonElement _definition;` |
| 282 | `private JsonElement _catalog;` |
| 283 | `private VBoxContainer? _panel;` |
| 284 | `private OptionButton? _selector;` |
| 285 | `private Label? _status;` |
| 286 | `private TextEdit? _details;` |
| 287 | `private CheckButton? _loop;` |
| 288 | `private CheckButton? _mute;` |
| 289 | `private long _epoch;` |
| 290 | `private long _serial;` |
| 291 | `private int _oneShots;` |
| 292 | `private double _time;` |
| 293 | `private double _master = 80;` |
| 294 | `private double _effects = 80;` |
| 295 | `private double _ambience = 60;` |
| 296 | `private bool _closed;` |
| 297 | `private uint _variantState = 290029;` |
| 299 | `private static string Text(JsonElement value, string key)` |
| 304 | `private static bool Same(string left, string right)` |
| 309 | `private static JsonElement SourceNode(JsonElement owner, string field, string id)` |
| 317 | `private static string SourceText(JsonElement owner, string field, string id)` |
| 325 | `private static JsonElement SourceMatch(JsonElement owner, string field, string id)` |
| 337 | `private static void ValidateOriginal(JsonElement source, string id)` |
| 347 | `private static string DonorSource(JsonElement pack, JsonElement sample, string id, string reference)` |
| 365 | `private static void DerivedSource(JsonElement sample, string id, string expected, int frames, double gain)` |
| 379 | `private static string GeneratedSource(JsonElement pack, JsonElement sample, string id)` |
| 402 | `private static string SharedSource(JsonElement pack, JsonElement sample, JsonElement actions, string id)` |
| 422 | `private static int Number(JsonElement value, string key)` |
| 427 | `private static string Hash(byte[] bytes)` |
| 432 | `private static void Require(bool valid, string file, string field, string value, string reason)` |
| 440 | `private static JsonElement Parse(byte[]? bytes, string file)` |
| 452 | `private static void Unique(JsonElement value, string file)` |
| 475 | `private void Load(Func<string, byte[]?> read, string category)` |
| 509 | `private JsonElement Bound(Func<string, byte[]?> read, string file)` |
| 520 | `private void LoadPack(Func<string, byte[]?> read, string file)` |
| 559 | `private void LoadRoute(JsonElement route)` |
| 583 | `private void Build()` |
| 629 | `private void Button(string text, Action action)` |
| 637 | `private void Volume(string name, double value)` |
| 646 | `private void Limits()` |
| 659 | `private void SetVolume(string name, double value)` |
| 683 | `private void Muted(bool muted)` |
| 692 | `private void Select(long index)` |
| 702 | `private void Play()` |
| 716 | `private void RestoreSelection(long index)` |
| 727 | `private bool Admit(string routeId, JsonElement route)` |
| 754 | `private void ReconcileLimits()` |
| 785 | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring)` |
| 806 | `private uint DrawVariant(int choices)` |
| 819 | `private void ApplyVolume(AudioStreamPlayer player, JsonElement route)` |
| 826 | `private void Remove(long id)` |
| 837 | `private void StopAudition()` |
| 852 | `private void StopAll()` |
| 866 | `private void Refresh()` |
| 914 | `private bool CurrentMuted()` |

### `tests/phase1/AudioPresentationTests.cs`

| Line | Actual authored declaration |
| --- | --- |
| 17 | `public void ReplacementOwners_ReceiveStrictlyIncreasingEpochs()` |
| 26 | `public void Sources_ResolveGeneratedFoodAuthority()` |
| 33 | `public void Sources_ResolveCurrentSharedAction()` |
| 40 | `public void CriticalBusZero_RemainsVisiblyMuted()` |
| 53 | `public void VariantDraw_ExcludesPreviousAndPreservesChoices(int count, int previous, uint draw, int expected)` |
| 60 | `public void Sources_ResolveEveryCurrentVariant()` |
| 89 | `public void Sources_RejectWrongAuthoritiesAndRestore(string control, string field)` |
| 111 | `private static readonly Type _subject = Subject();` |
| 113 | `private static JsonObject Object(string json)` |
| 118 | `private static JsonNode Field(JsonObject owner, string field)` |
| 123 | `private static JsonObject First(JsonObject owner, string field)` |
| 129 | `private static JsonElement Element(JsonObject node)` |
| 135 | `private static void Mutate(string control, JsonObject pack, JsonObject sample, JsonObject actions)` |
| 184 | `private static string Source(string id)` |
| 202 | `private static JsonElement Pack(string file)` |
| 209 | `private static object? Call(string name, object[] arguments)` |
| 216 | `private static Type Subject()` |


### Current native tuples: `src/godot-game/inspection/AudioPreview.cs`

| Line:column | Native member | Host |
| --- | --- | --- |
| 82:20 | `Godot.Node2D.Node2D()` | `internal static AudioPreview Open(InspectionShell? shell, MainThreadGateway gateway, Func<string, byte[]?> read, string category) {` |
| 82:33 | `Godot.Node.Name` | `internal static AudioPreview Open(InspectionShell? shell, MainThreadGateway gateway, Func<string, byte[]?> read, string category) {` |
| 156:64 | `Godot.AudioStreamPlayer.Playing` | `internal void Tick(double delta) {` |
| 176:27 | `Godot.Node.GetParent()` | `internal void Close() {` |
| 177:13 | `Godot.Node.RemoveChild(Godot.Node)` | `internal void Close() {` |
| 178:13 | `Godot.GodotObject.Free()` | `internal void Close() {` |
| 585:22 | `Godot.ScrollContainer.ScrollContainer()` | `private void Build() {` |
| 585:44 | `Godot.Node.Name` | `private void Build() {` |
| 585:60 | `Godot.Control.CustomMinimumSize` | `private void Build() {` |
| 585:80 | `Godot.Vector2.Vector2(float, float)` | `private void Build() {` |
| 586:13 | `Godot.ScrollContainer.FollowFocus` | `private void Build() {` |
| 587:9 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Build() {` |
| 588:18 | `Godot.VBoxContainer.VBoxContainer()` | `private void Build() {` |
| 588:38 | `Godot.Node.Name` | `private void Build() {` |
| 588:56 | `Godot.Control.CustomMinimumSize` | `private void Build() {` |
| 588:76 | `Godot.Vector2.Vector2(float, float)` | `private void Build() {` |
| 589:9 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Build() {` |
| 590:21 | `Godot.OptionButton.OptionButton()` | `private void Build() {` |
| 590:40 | `Godot.Node.Name` | `private void Build() {` |
| 591:9 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Build() {` |
| 595:13 | `Godot.OptionButton.AddItem(string, int)` | `private void Build() {` |
| 598:9 | `Godot.OptionButton.ItemSelected` | `private void Build() {` |
| 599:17 | `Godot.CheckButton.CheckButton()` | `private void Build() {` |
| 599:35 | `Godot.Node.Name` | `private void Build() {` |
| 599:50 | `Godot.Button.Text` | `private void Build() {` |
| 600:9 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Build() {` |
| 601:17 | `Godot.CheckButton.CheckButton()` | `private void Build() {` |
| 601:35 | `Godot.Node.Name` | `private void Build() {` |
| 601:50 | `Godot.Button.Text` | `private void Build() {` |
| 602:9 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Build() {` |
| 603:9 | `Godot.BaseButton.Toggled` | `private void Build() {` |
| 607:19 | `Godot.Label.Label()` | `private void Build() {` |
| 607:31 | `Godot.Node.Name` | `private void Build() {` |
| 608:9 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Build() {` |
| 609:20 | `Godot.TextEdit.TextEdit()` | `private void Build() {` |
| 609:35 | `Godot.Node.Name` | `private void Build() {` |
| 609:53 | `Godot.TextEdit.Editable` | `private void Build() {` |
| 609:71 | `Godot.TextEdit.WrapMode` | `private void Build() {` |
| 609:82 | `Godot.TextEdit.LineWrappingMode.Boundary` | `private void Build() {` |
| 610:13 | `Godot.Control.CustomMinimumSize` | `private void Build() {` |
| 610:33 | `Godot.Vector2.Vector2(float, float)` | `private void Build() {` |
| 611:9 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Build() {` |
| 616:22 | `Godot.OptionButton.OptionButton()` | `private void Build() {` |
| 616:41 | `Godot.Node.Name` | `private void Build() {` |
| 617:9 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Build() {` |
| 622:13 | `Godot.OptionButton.AddItem(string, int)` | `private void Build() {` |
| 625:9 | `Godot.OptionButton.ItemSelected` | `private void Build() {` |
| 631:22 | `Godot.Button.Button()` | `private void Button(string text, Action action) {` |
| 631:35 | `Godot.Node.Name` | `private void Button(string text, Action action) {` |
| 631:85 | `Godot.Button.Text` | `private void Button(string text, Action action) {` |
| 632:13 | `Godot.BaseButton.Disabled` | `private void Button(string text, Action action) {` |
| 633:16 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Button(string text, Action action) {` |
| 634:9 | `Godot.BaseButton.Pressed` | `private void Button(string text, Action action) {` |
| 639:21 | `Godot.Label.Label()` | `private void Volume(string name, double value) {` |
| 639:33 | `Godot.Label.Text` | `private void Volume(string name, double value) {` |
| 640:16 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Volume(string name, double value) {` |
| 641:22 | `Godot.HSlider.HSlider()` | `private void Volume(string name, double value) {` |
| 641:36 | `Godot.Node.Name` | `private void Volume(string name, double value) {` |
| 641:49 | `Godot.Range.MinValue` | `private void Volume(string name, double value) {` |
| 641:63 | `Godot.Range.MaxValue` | `private void Volume(string name, double value) {` |
| 641:79 | `Godot.Range.Step` | `private void Volume(string name, double value) {` |
| 641:89 | `Godot.Range.Value` | `private void Volume(string name, double value) {` |
| 642:16 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Volume(string name, double value) {` |
| 643:9 | `Godot.Range.ValueChanged` | `private void Volume(string name, double value) {` |
| 650:25 | `Godot.Label.Label()` | `private void Limits() {` |
| 650:37 | `Godot.Label.Text` | `private void Limits() {` |
| 651:20 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Limits() {` |
| 652:25 | `Godot.SpinBox.SpinBox()` | `private void Limits() {` |
| 652:39 | `Godot.Node.Name` | `private void Limits() {` |
| 652:52 | `Godot.Range.MinValue` | `private void Limits() {` |
| 652:66 | `Godot.Range.MaxValue` | `private void Limits() {` |
| 653:33 | `Godot.Range.Step` | `private void Limits() {` |
| 653:43 | `Godot.Range.Value` | `private void Limits() {` |
| 654:20 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Limits() {` |
| 655:13 | `Godot.Range.ValueChanged` | `private void Limits() {` |
| 705:45 | `Godot.OptionButton.ItemCount` | `private void Play() {` |
| 711:26 | `Godot.OptionButton.GetItemText(int)` | `private void Play() {` |
| 711:53 | `Godot.OptionButton.Selected` | `private void Play() {` |
| 713:29 | `Godot.BaseButton.ButtonPressed` | `private void Play() {` |
| 729:35 | `Godot.Range.Value` | `private bool Admit(string routeId, JsonElement route) {` |
| 729:35 | `Godot.Node.GetNode<Godot.SpinBox>(Godot.NodePath)` | `private bool Admit(string routeId, JsonElement route) {` |
| 730:35 | `Godot.Range.Value` | `private bool Admit(string routeId, JsonElement route) {` |
| 730:35 | `Godot.Node.GetNode<Godot.SpinBox>(Godot.NodePath)` | `private bool Admit(string routeId, JsonElement route) {` |
| 762:27 | `Godot.Range.Value` | `private void ReconcileLimits() {` |
| 762:27 | `Godot.Node.GetNode<Godot.SpinBox>(Godot.NodePath)` | `private void ReconcileLimits() {` |
| 763:27 | `Godot.Range.Value` | `private void ReconcileLimits() {` |
| 763:27 | `Godot.Node.GetNode<Godot.SpinBox>(Godot.NodePath)` | `private void ReconcileLimits() {` |
| 789:22 | `Godot.AudioStreamWav.AudioStreamWav()` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 789:43 | `Godot.AudioStreamWav.Data` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 789:55 | `Godot.AudioStreamWav.Format` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 789:64 | `Godot.AudioStreamWav.FormatEnum.Format16Bits` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 790:13 | `Godot.AudioStreamWav.MixRate` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 790:30 | `Godot.AudioStreamWav.Stereo` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 790:45 | `Godot.AudioStreamWav.LoopMode` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 790:63 | `Godot.AudioStreamWav.LoopModeEnum.Forward` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 791:23 | `Godot.AudioStreamWav.LoopModeEnum.Disabled` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 791:61 | `Godot.AudioStreamWav.LoopBegin` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 792:13 | `Godot.AudioStreamWav.LoopEnd` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 793:22 | `Godot.AudioStreamPlayer.AudioStreamPlayer()` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 793:46 | `Godot.Node.Name` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 794:58 | `Godot.AudioStreamPlayer.Stream` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 794:75 | `Godot.AudioStreamPlayer.MaxPolyphony` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 795:9 | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 796:9 | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 797:9 | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 798:9 | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 799:9 | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 803:9 | `Godot.AudioStreamPlayer.Play(float)` | `private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring) {` |
| 821:32 | `Godot.BaseButton.ButtonPressed` | `private void ApplyVolume(AudioStreamPlayer player, JsonElement route) {` |
| 823:9 | `Godot.AudioStreamPlayer.VolumeDb` | `private void ApplyVolume(AudioStreamPlayer player, JsonElement route) {` |
| 830:9 | `Godot.AudioStreamPlayer.Stop()` | `private void Remove(long id) {` |
| 831:9 | `Godot.AudioStreamPlayer.Stream` | `private void Remove(long id) {` |
| 832:9 | `Godot.Node.RemoveChild(Godot.Node)` | `private void Remove(long id) {` |
| 833:9 | `Godot.GodotObject.Free()` | `private void Remove(long id) {` |
| 834:9 | `Godot.GodotObject.Dispose()` | `private void Remove(long id) {` |
| 879:9 | `Godot.Label.Text` | `private void Refresh() {` |
| 881:9 | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` | `private void Refresh() {` |
| 882:9 | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` | `private void Refresh() {` |
| 883:9 | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` | `private void Refresh() {` |
| 884:9 | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` | `private void Refresh() {` |
| 888:13 | `Godot.OptionButton.ItemCount` | `private void Refresh() {` |
| 890:41 | `Godot.OptionButton.GetItemText(int)` | `private void Refresh() {` |
| 890:68 | `Godot.OptionButton.Selected` | `private void Refresh() {` |
| 903:17 | `Godot.Label.Text` | `private void Refresh() {` |
| 910:9 | `Godot.TextEdit.Text` | `private void Refresh() {` |
| 916:51 | `Godot.OptionButton.ItemCount` | `private bool CurrentMuted() {` |
| 917:40 | `Godot.OptionButton.GetItemText(int)` | `private bool CurrentMuted() {` |
| 917:67 | `Godot.OptionButton.Selected` | `private bool CurrentMuted() {` |
| 919:37 | `Godot.BaseButton.ButtonPressed` | `private bool CurrentMuted() {` |


### Current evaluated producer inputs

| Current evaluated source input | SHA256 |
| --- | --- |
| `src/godot-game/GeneratedCallSiteAdapters.cs` | `a975b3d99d6fc4f0d6c3a291cc310ecd17740d05040d4a59aa25f5aa27831006` |
| `src/godot-game/GeneratedReportAdapter.cs` | `eb320d524bab395c1a774fe9837afa20f7733ab4bf806b93d7fd7460a57c0a7b` |
| `src/godot-game/KingdomZombies.csproj` | `3ca7b050cd7cd3642771e295e4229ea57da3813119d5ef7921f711859df3871b` |
| `src/godot-game/foundation/DiscoveryCodec.cs` | `72857d3b68db6e13f1c6050cf4c5d59cfa04dc8c2e5c16e894e518ef94eb68c6` |
| `src/godot-game/foundation/EnetConnectionService.cs` | `172d8aeb844ef6ce3b3877f27daccf9fc02a08399e66bd50bac7de5205135805` |
| `src/godot-game/foundation/FoundationActorNode.cs` | `1660a842a73628c3ae0ec910516442a124c689d07756b12217942d7d97069756` |
| `src/godot-game/foundation/FoundationAdmissionHistory.cs` | `8c502be32d853f84bd1a7367bd17268db32864542e1da364c971ac2a36059f25` |
| `src/godot-game/foundation/FoundationAutomation.cs` | `f08fd4ab9b7e0b5db493d1ec1ca794c559b219bf81d81ea13fe94a0e325df338` |
| `src/godot-game/foundation/FoundationBrowser.cs` | `bd2de94bf7df9924edaef2473846cfa9d318ea6e0a812f0383b3b3cc13b65117` |
| `src/godot-game/foundation/FoundationCapturePublication.cs` | `40631cd1678cea9d74e3e91d24c5cb70a48c36e190b5858090db2a4b9f00be87` |
| `src/godot-game/foundation/FoundationCoordinator.Admission.cs` | `2663a9811ebf6c58c5e063906006c3ff0ac542fba8d54861048d6527bc60ffc5` |
| `src/godot-game/foundation/FoundationCoordinator.GuestPublication.cs` | `d35c8a3178ffe95d3f1310a9f625abaacee52a30d9cd941ba88a7df139983a54` |
| `src/godot-game/foundation/FoundationCoordinator.Lifetime.cs` | `0c2338fc168043a838254c8c6dd1f4b2ff1aed4a5037fa1732957b5fe3c5fc73` |
| `src/godot-game/foundation/FoundationCoordinator.cs` | `22888110aeda742bbed897384aa4e4fef5904422706f49ad86663024e1ee9297` |
| `src/godot-game/foundation/FoundationCopy.cs` | `0044b494115a9d4eaa30dc0503c4490ae86f937e0b9999ff300e67ef869ed21e` |
| `src/godot-game/foundation/FoundationDiagnostics.cs` | `ccf43cd946e547c87ed41f39eb078c166fb4c5534e68c683d7d0778fc512cba3` |
| `src/godot-game/foundation/FoundationDisplay.cs` | `d27504cc202552f2a6075532818d8182272e3ecad885e03869c8aa0914a1e572` |
| `src/godot-game/foundation/FoundationDrawObserver.cs` | `a6dc2c920c57c1a40e0c78109fbebd565608e551199f53ff48b358a207030e73` |
| `src/godot-game/foundation/FoundationDrawRequest.cs` | `7c2eda11f400c0c91cafee1df5c4ac7101f110af9d45b43bb97b77bf755ff3d1` |
| `src/godot-game/foundation/FoundationEndpoint.cs` | `b7a198d1e12079a51456503d5d2df6efa065c76f06310989a1b8bdabd564a7fb` |
| `src/godot-game/foundation/FoundationEnvelope.cs` | `0bf12d1d94212dc30a1e8e57f665e79ca8c3eb2180d62d472860caac77e9a4a6` |
| `src/godot-game/foundation/FoundationEphemeralProfile.cs` | `cb29d363e9fab9d5887ab1675ccd7421664498da80e9f518438bcc47741b49eb` |
| `src/godot-game/foundation/FoundationHud.cs` | `f59a5e31b8aed2f7b1d267c34c90c7478ca9cb69aae08bee46c24e78090ec63d` |
| `src/godot-game/foundation/FoundationInput.cs` | `caed7f7b5cd0eca46813aa7a0909f99307cf33d155407a063263cf3fca528b63` |
| `src/godot-game/foundation/FoundationMenu.cs` | `71c5cf2f909a6849d768abc0109ae0fd34a35a57cc860ad119e782320d28328d` |
| `src/godot-game/foundation/FoundationNativeRegression.cs` | `9bfbd09eaf77d280d10c631f22a4528f00eaf19babe6a6687eb8fc7f34c60416` |
| `src/godot-game/foundation/FoundationObservationScope.cs` | `ab9972b6888c0cedce0ac374421cb0554ef45e8b5596cd0b18e4fe149dcd425b` |
| `src/godot-game/foundation/FoundationPeerBinding.cs` | `4c0749cc0221f2d5dc5f7ed479be67ffa78b660e0a3e55f86871f66cd52ef153` |
| `src/godot-game/foundation/FoundationPresentation.cs` | `df4a59a0f74055375fce7b671b8fd4d1db98f3697f397ab3cdfe2045452f6928` |
| `src/godot-game/foundation/FoundationProtocol.cs` | `185359ce9cefb0296b946665aefe35f6d37929a363497130111bb20a8e9c9f78` |
| `src/godot-game/foundation/FoundationRegressionDiscovery.cs` | `42f45ef67cd60cdc4e9e37b6c49d9a8fb6a951e8122477dc69ecac9b068f65d2` |
| `src/godot-game/foundation/FoundationRoot.cs` | `530adae080b9d0811c9f48af7d1c92f3006f6e2bbabf01bd8303ec328040f8f8` |
| `src/godot-game/foundation/FoundationRuntimeState.cs` | `783e69efc179f5dfab2906c3e53ffaa22ff6d257d7248e744e0e2627ab717442` |
| `src/godot-game/foundation/FoundationServices.cs` | `d5e6dbd4148027feacbee4bfdfb332171e8fc48ab1a3709a3b8d23a238f3a444` |
| `src/godot-game/foundation/FoundationView.cs` | `ad0bad27bee096ccbc13add9135bfd2347dc183ede6dfe325d088c85c92eb9bc` |
| `src/godot-game/foundation/FoundationWireFrame.cs` | `8fde7f8dbf7288915ff9dfcf1e606a5e4bd85f4ad937c8a0ee8db53a9a56d144` |
| `src/godot-game/foundation/LanDiscoveryService.cs` | `1b92977b0071f157cdabef27417910f6272be1347e9a59034c064690acf51fac` |
| `src/godot-game/foundation/LocalProfileService.cs` | `bcba74a53d69d7912b7af509f54e0beaa3e2fa7a9b69047794c42b125bf2b444` |
| `src/godot-game/foundation/LocalProfileStorage.cs` | `bef4ab51e6fb6f7d1bc5e4cfbe699d3df1ed04cf9d44207e49dbd6c7c04869ce` |
| `src/godot-game/inspection/AudioPreview.cs` | `f64d4c89a30b5b896ca418d52731c8182f5b06799c005cc7ebf204393c382d4f` |
| `src/godot-game/inspection/BuildingPreview.cs` | `0a97d55d3e57d9610796fc4188abdc4ae3f33f180a0fdc21685da034c5fe869e` |
| `src/godot-game/inspection/CampLayouts.cs` | `55da347cdeab232c2eee214211a9372ad69bb7121bcc72a82d9b1aa5c669a2ce` |
| `src/godot-game/inspection/CharacterPreview.cs` | `7b20d679ecb39616ed6af668209f460d90df4b96d0854409ae918b565e485712` |
| `src/godot-game/inspection/CorpsePreview.cs` | `f3739163a2920a65597e719d1021a7ea2c72344a5b415040de076c15ef3624a2` |
| `src/godot-game/inspection/CrowdPreview.cs` | `595f601e5bff1c6495e9e89fd3042f0589e47fa107cfeb5187e7b892874c940f` |
| `src/godot-game/inspection/IdentityKit.cs` | `03d3e01a3eedb4251b74f413e639f5cd9d0540c9c097400aae1b1aa7c83898a2` |
| `src/godot-game/inspection/InspectionInput.cs` | `05fed457e170e39e9092fb38ce4a956990e30322c8fcb89358bf0c59c8510c0c` |
| `src/godot-game/inspection/InspectionRoot.cs` | `67298eed12165964954bdf251657b0b89f3b0707dbd7fe7f3a6972404c28a1cc` |
| `src/godot-game/inspection/InspectionShell.cs` | `5f67a836a0f9cdafe302a76f03fb93cf3cdb5f91aab4dbc71724ba0dd55366c2` |
| `src/godot-game/inspection/InventoryLayouts.cs` | `dbe540c0eaa06f8ee1a3e5f3b9f0c861383d5e0089220af3498f3de119c99f7a` |
| `src/godot-game/inspection/SessionLayouts.cs` | `2f41f0b9aba30f43c1d4818dc31480e28d64afcda52954e2b7ae0345cb5ce12f` |
| `src/godot-game/inspection/WorldPreview.cs` | `3d4ad570cc17a8b2e619325b44592657918ce145564856d0711f9a2390a037ee` |
| `src/godot-game/proofs/LifecycleFixture.cs` | `dd78f93caa8fc443da1a87818a60bc9f0005778720f9c68b65a3c6246235aa1b` |
| `src/godot-game/proofs/LifetimeToken.cs` | `1370aa0786f4c0451402f67ac8e183676fc3b2d4bedc582c7eae44ccdd704750` |
| `src/godot-game/proofs/MainThreadGateway.cs` | `1f51d27e0b9fcec988dff88d1895cb05dd17cf9f8eeb13f68ab17743c67c182a` |
| `src/godot-game/proofs/Phase1.cs` | `24cd0f38eb493bd2909176691c05a866833797238cca57b35cdcefd4af3dbbeb` |
| `src/godot-game/proofs/ProofResource.cs` | `acb1ff36821a5300369e8b38e869ddc05f993d7c09a879ee8c19c43bf9474dde` |
| `src/godot-game/proofs/ProofSequence.cs` | `eea9be87a9f9007a818de45360e3424c734d7c5526ab83a057e2cf885045a7a8` |

### Current generated surfaces

| Current generated surface | SHA256 |
| --- | --- |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.GodotPluginsInitializerGenerator/GodotPlugins.Game.generated.cs` | `7870166b33f91817432f480e7b2707b22005838c67dfd6cec2256efa7b88975f` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptMethodsGenerator/KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs` | `b160e87046cc6442d2c56ae9ad34da1ce84de25a0762c6aaf338ffb1d77a4a9b` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptMethodsGenerator/KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs` | `90670ddae4552b2c26393d581cf4c529a81df89bff8700563891f3320089776d` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptMethodsGenerator/KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs` | `7be09e40901a9e680a0a54ec073306ee4284b1a2bafb226e3bed7ad4fc1dae23` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptMethodsGenerator/KingdomZombies.Proofs.Phase1_ScriptMethods.generated.cs` | `34079613e2ca0fa422748dd95ceb99d60682c79bdbe69d80cf16c0b00ba07383` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptMethodsGenerator/KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs` | `ef6e19a71ca243f165a8d827a3169f1398fb561e3d798ed5ef61831a5b7e1b04` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPathAttributeGenerator/AssemblyScriptTypes.generated.cs` | `5d5881e57e36f1a6f4c05b1765a3b009281a043d0c47f6c78e7c8db8b58a5010` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPathAttributeGenerator/KingdomZombies.Foundation.FoundationRoot_ScriptPath.generated.cs` | `b6fa4d73beddc8b5b4b645eb35a9e583d123c5fbeb5d85b5d7fa6accfd767964` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPathAttributeGenerator/KingdomZombies.Inspection.InspectionRoot_ScriptPath.generated.cs` | `82b806e05349bd6c149d596dd82e9e7e5610ab22801f8c1ee045347d037ecb48` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPathAttributeGenerator/KingdomZombies.Proofs.LifecycleFixture_ScriptPath.generated.cs` | `52689f3ba97ce798dec88a068b7dc3c3eafe9dc02d08eeac65a5982cafa245c9` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPathAttributeGenerator/KingdomZombies.Proofs.Phase1_ScriptPath.generated.cs` | `e8f7a88f641460846aa2d92eeed7e4ad427309ff11052d7c096d554c832a4d98` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPathAttributeGenerator/KingdomZombies.Proofs.ProofResource_ScriptPath.generated.cs` | `9c16270deeb2497ef3b3f2651a8aa955886349d31cb6526408a8776396735a6f` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertiesGenerator/KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs` | `026160471d7682ebf7467b501d9d24591b774b02777a4c26ef4a9274cc2f6bdc` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertiesGenerator/KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs` | `49987c74fe6d557f5b88d57aa9d8f78d849a85e364a1d771d0b387c75990b902` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertiesGenerator/KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs` | `d99da9ea4012a4651fa1663bf1bc45074c4a60caa5cb3cf929a97fe17a40af54` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertiesGenerator/KingdomZombies.Proofs.Phase1_ScriptProperties.generated.cs` | `b1e2255f7d871ee37c5654392a539e2ba08fb35f404a0c1d703bee763eec5a59` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertiesGenerator/KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs` | `b9d765d944be0253daca0102e8a9e6894271d70689909f638e75285709e46d8b` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertyDefValGenerator/KingdomZombies.Foundation.FoundationRoot_ScriptPropertyDefVal.generated.cs` | `085f28cc80245a249d0b8ce95a215c4ed13bcbc511f1cac16779a1598c4ff15d` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertyDefValGenerator/KingdomZombies.Inspection.InspectionRoot_ScriptPropertyDefVal.generated.cs` | `dfe0e51f1b9c8dbf3e532cbdb82045cb724b4e0f62f5d999404fe536684cbb42` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertyDefValGenerator/KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs` | `ee4987b41beea70d50ff79251c3104a7fde2e2f5e98631cdf6cf20d51ffdfe08` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertyDefValGenerator/KingdomZombies.Proofs.Phase1_ScriptPropertyDefVal.generated.cs` | `2a4ea882c5fc095285e05b515bf352f8aa4b485b7a069defe7346f81a3e65dfc` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertyDefValGenerator/KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs` | `fd162606de35b04e2133e078a52f23f66e4e4682dac4d65b671b84d358780579` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSerializationGenerator/KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs` | `68c1e274833226bb83c6fb9c4c2d127d547a7b4c43bf9d5c5c897befa2d81d90` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSerializationGenerator/KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs` | `b1ae41f931f3ddd1d83ca13f9d46ba35e310d1f0ab2eb6b8c4470c7106e0bbd6` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSerializationGenerator/KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs` | `7d9f20427246a8c2061e3b2639834b6e335f57d95a713b510d4564ffd7b2f47a` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSerializationGenerator/KingdomZombies.Proofs.Phase1_ScriptSerialization.generated.cs` | `0a33712cd9aad72d45086d840eb0d4b01e13953b9e1396514413626ca83ff93e` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSerializationGenerator/KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs` | `97dfe2f6e9a66379cbd39ee24a659961798eada8e30a7fcb71741504e73dc5bb` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSignalsGenerator/KingdomZombies.Foundation.FoundationRoot_ScriptSignals.generated.cs` | `fc5da71423173e70534fbe6c1b3322d53db563098ba9b3476ad7a4925a9ef8ce` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSignalsGenerator/KingdomZombies.Inspection.InspectionRoot_ScriptSignals.generated.cs` | `6e6614685327ae041a8cad23e31ebd37cb04162293a2880ceac50333924e3519` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSignalsGenerator/KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs` | `ec1160985562cd5eaec56d12f8b9bce8859103d612f7f1c4e30a6118b3a6ba1a` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSignalsGenerator/KingdomZombies.Proofs.Phase1_ScriptSignals.generated.cs` | `7f367c42c26637dafcf1e1dfce7a9cc3616a0a8bc631b7b665f5cad3e4063b57` |
| `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.ScriptSignalsGenerator/KingdomZombies.Proofs.ProofResource_ScriptSignals.generated.cs` | `1935ca33aae897f56066a979b291cb0b9692f366222b651c6a547f80fe3a463e` |

### Current production assembly closure

| File | SHA256 |
| --- | --- |
| `GodotSharp.dll` | `2afe2686aacfefac36da6601b59aeb05e560f06e4970cd5036100acd2ca4249d` |
| `GodotSharpEditor.dll` | `195c81c1ccb7c05559fc79b4acbb9025154a74ad99ecb9bb3747b5fbfc2ed1d1` |
| `KingdomZombies.Core.dll` | `98fde008c3ef522c4eba86aa601d2fc7538a8e59c95da7caa5e7b241095567e8` |
| `KingdomZombies.Core.pdb` | `05b2b534d36d01f48b1da7b3265aff4efe6494e05a4dcd653c19d3108ec48a1c` |
| `KingdomZombies.Core.xml` | `35dd6d282c11bf5f14bc76ded97865044324d5f07b5047543698ff643d5e12dd` |
| `KingdomZombies.deps.json` | `2697218d683aee1fe41a750aa66ab615b7f07dcf206648c78fcf8046c25c4861` |
| `KingdomZombies.dll` | `8e11c16920f0190814210e9028d76208fe00337ec6b2572f09e3041728c1299a` |
| `KingdomZombies.Multiplayer.Abstractions.dll` | `00df4530bb32bc1e01f2f14fb4c8ba7908ee9ecedc2ef6535fc507b1ed28946a` |
| `KingdomZombies.Multiplayer.Abstractions.pdb` | `73a68aceb355a4d6aa26e8175d62db1bc6a694801160208f876f5e409eb4a590` |
| `KingdomZombies.Multiplayer.Abstractions.xml` | `8653c78e02e3ff6db52e2cb7bff366305498f8d3467570b4b575948214fc6e3e` |
| `KingdomZombies.pdb` | `451c72e17ba39890d4e83d1c6731450fe2ec32b49b9803253398caffeeed753d` |
| `KingdomZombies.runtimeconfig.json` | `4ce55550bffe2e763ef3d418b5566fc16d1290811d9a8fd98f5a6a6cbd378f2b` |
| `KingdomZombies.xml` | `b5422053eac5d1e744355965402b250c9dd83276fecb2aef9b307cd843586aef` |

## Evidence integrity and self-check inputs

Ignored immutable final directory: `.phase4/04-29-final-r6/`. Its full ordered selection records all reasons/omissions and per-case actual revisions/closures. Exact SHA256 values:

| File | SHA256 |
| --- | --- |
| `selection.json` | `881a2004df55a7797ad7332202bbbf9b4db47eb1f10dc1833006aa922c69612a` |
| `results.json` | `c6b12db25cddd8be1f26b8f6a697ca7d13747269c249d95406fb16838ea8fef0` |
| `completion.json` | `11749eda32a0b9f3ce4938b8f47c59a9c6de9265ccdf991bf81bb1d3fbe1b622` |
| `authored-symbols.json` | `1145a3a4cf509725aec58df04ef5d127a2ee41f87bd44570daf2f15737cd0134` |
| `actuals.json` | `7715840cd1d8314d9fd0b40c4866b3027f11accc78b2d23c2a5c6c37e6809638` |
| `native-policy-observation.json` | `ee0322c63f00b435cd2f4a3c2eabf63313e908f983bb2fdfaf43b81f849db678` |
| `generated-input-observation.json` | `568ab216f123f587768dbcdda124109dafcb9cd8fb777de6d29520a5199062cc` |

Final combined Audio observation hashes are lifecycle `03c0357ec22f7a63c129e1a0dd7d5ed585de93ca3adbdbc1ccbed5d31c251d43`, held-out `6a9e6de954db83b0deedfa47a42bc19ce175af2ec045ea671448e20437bd23df`. Final native policy and generated input observations are separate from executable-case passes and preserve expected failure/unenrolled status.

Stub/skip scan found no task-blocking placeholders, TODO/FIXME or skipped tests. The two LINQ Skip calls are actual limit eviction, not skipped proofs. Empty quiet state and unavailable/error/partial views are intentional tested behavior with current data sources. No new unmodeled network/auth/schema boundary exists; local bounded parser/native/evidence threats remain within the plan threat model. No deferred implementation stub is concealed as complete.

Measured source-ledger commits, including preserved checkpoints/coordinator amendments, exclude the following SUMMARY metadata commit:

```text
d70ef24 test(04-29): assert replacement audio ownership epochs
3f31a8c docs(04-29): preserve exhausted Task1 technical checkpoint
8bef623 docs(04-29): bound initial native lifecycle repair after preserved checkpoint
e2de6fd docs(04-29): record R1 adjacent formatting scope checkpoint
b908d34 docs(04-29): scope adjacent selector indentation within R1 budget
835f6c9 docs(04-29): preserve exhausted R1 native binding checkpoint
4ce3803 docs(04-29): scope actual-project Hub proof context after R1 checkpoint
7d31093 docs(04-29): record native source semantics checkpoint
8aa3309 docs(04-29): scope actual generated and shared audio provenance resolution
13ffce4 test(04-29): prove generated and shared source lookup rejection
8ed9481 docs(04-29): preserve exhausted R3 source owner checkpoint
fef57b9 docs(04-29): scope exact sharedassets2 owner compatibility correction
e5b2ccc fix(04-29): resolve current audio source authority forms
4f0583a feat(04-29): add native audio audition and exact current loop ownership
c06fcfa test(04-29): require visible mute at selected bus zero
f72e3be feat(04-29): prove held-out audio limits restore ownership and muted buses
6e7edc0 docs(04-29): checkpoint partial-view Stop all and safely stopped selection
ae3e593 docs(04-29): scope partial-view stop control repair
8d318f4 fix(04-29): keep stop controls reachable for partial audio loops
f185975 docs(04-29): checkpoint complete selection hub filter regression
1c93552 docs(04-29): scope existing Hub status filter repair
ecc20cd fix(04-29): honor existing Hub status eligibility for audio rows
```

## Self-Check: PASSED

Verified all484 current input hashes,49 fresh passing invocation revisions,22 compiled tests with0skips,687/807 complete native outcomes and clean applicable logs; exact report/assembly/producer/package hashes; all task/repair commit ancestry; committed Task1 tracer before Task2; measured22 source-ledger commits; required artifacts and local checkpoint links. Root signatures/Read and fixed production scope are unchanged. Only this canonical summary remained untracked before its metadata commit. Global state and acceptance remain parent-owned.

