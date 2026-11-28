# Phase 6 rendered and phase-end evidence

This record holds the Phase 6 phase-end runs at 1280×720 on the development Windows desktop. It keeps the per-case outcomes, image identities, qualitative findings and failure history. The failure history is never rewritten. Later repair and final runs are appended below it.

## Current status

**Not accepted.** The first fresh phase-end run failed. It recorded 66 passed, 39 failed and 18 blocked of 123 selected cases, and both Critical smoke cases failed. The repairs change reviewed source. They need a fresh independent review, a focused rerun of the failed and blocked cases, then a fresh full run of the selected set. None of these has happened yet. No case below is credited as a Phase 6 pass until the final run on the repaired, reviewed source passes.

The focused rerun (Run 2, below) did not pass: 28 passed, 13 failed and 16 blocked of the 57 run-1 cases. The one newly affected case passed, with P1–P4 pending.

The focused attempt after repair rounds 1 and 2 (Run 2a, below) did not pass either: 27 passed, 28 failed and 2 blocked of the 57 run-1 cases. The one newly affected case passed again, with P1–P4 pending.

## Run 1: initial phase-end selection (failed)

| Field | Value |
| --- | --- |
| Invocation | `ee17949e-45ed-4486-9915-dc0b7029cdf1` |
| Tested revision | `a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d` (clean worktree) |
| Review record | `.local/temp/phase06-review.json`: revision `a9ac95f`, reviewed source `17fb851c2bfd81ba050fa094527ef21d7707690c`, reviewer "Independent Phase 6 code reviewer", status `clean` |
| Comparison base | `origin/dev` at `b9aaa2b951e6f5dc128a17c7d58e0d17ffe10cb6`, with 177 changed paths |
| Command | `node src/godot-game/tools/game-tests/check.cjs --phase-end --phase 6 --review .local/temp/phase06-review.json` |
| Environment | Windows 10 Enterprise LTSC 2021 x64, Node 24.22.1, .NET SDK 10.0.404 / runtime 10.0.15, Godot 4.7.2 Mono, 1280×720 |
| Timing | Started `2026-11-24T21:43:57.588Z` and finished `2026-11-24T23:37:17.330Z` (6799.742 s) |
| Status | `fail`: 66 passed, 39 failed, 18 blocked, with 4 image reviews (P1–P4) pending |
| Selection | 123 selected (Critical smoke 2, phase baseline 38, declared input changed 27, affected regressions 56). 188 omitted, and none of them receives credit |

The run completed. The earlier session was interrupted after it finished and before its outcome was recorded. No game, test or runner process from that session was still running when this record was written.

### Omissions

| Reason | Count |
| --- | ---: |
| Unaffected and outside the smoke set or the phase scope | 43 |
| Pending procedure owned by Phase 5 | 25 |
| Pending procedure owned by Phase 6 (`A-05`, `A-22`, `A-23`, `A-27`) | 4 |
| Pending procedures owned by Phases 7, 8, 9 and 10 | 18, 25, 44, 5 |
| Pending procedure owned by Phase 11 | 24 |

### Groups

| Group | Pass | Fail | Blocked | Reported launches | Wall time |
| --- | ---: | ---: | ---: | ---: | ---: |
| basic | 4 | 0 | 0 | 1 | 61.079 s |
| phase06.solo | 0 | 4 | 2 | 0 (one solo process actually ran) | 63.861 s |
| phase06.lan | 0 | 3 | 0 | 0 (host and three guests actually ran) | 75.732 s |
| phase06.headless | 5 | 1 | 0 | 0 | 30.480 s |
| phase06.packaged | 1 | 0 | 0 | 0 | 78.537 s |
| gameplay-checks.headless | 6 | 1 | 0 | 0 | 12.459 s |
| gameplay-checks.rendered | 0 | 9 | 0 | 1 | 35.233 s |
| lan-checks.headless | 19 | 1 | 0 | 17 | 409.327 s |
| lan-checks.rendered | 0 | 12 | 0 | 6 | 158.410 s |
| asset-pipeline.headless | 25 | 3 | 0 | 7 | 1606.015 s |
| phase06.inspection | 0 | 2 | 16 | 0 (one inspection process actually ran) | 156.492 s |
| asset-pipeline.rendered | 1 | 0 | 0 | 1 | 124.829 s |
| verification.rules | 0 | 1 (21 of 451 nested) | 0 | 0 | 2397.207 s |
| verification.graph | 1 | 0 | 0 | 0 | 882.333 s |
| verification.engine | 0 | 1 (2 of 22 nested) | 0 | 0 | 291.407 s |
| verification.runner, pins, register, docs | 4 | 0 | 0 | 0 | 70.495 / 104.375 / 79.999 / 66.890 s |
| verification.state | 0 | 1 (1 of 33 nested) | 0 | 0 | 94.419 s |

Report defect: the summary reports `launchCount` 0 for the Phase 6 scenario groups, although they launched real processes. Most per-case `durationMs` values are missing (`NaN`). The final record must take its launch and timing facts from the group logs until the runner reports them.

### Case outcomes and failure history

Passed (66): `basic.strict-build`, `basic.repository`, `basic.pure`, `basic.headless`; `phase06.headless.world`, `.start`, `.inventory`, `.combat`, `.lifecycle`; `phase06.packaged`; `gameplay-checks.core.authority`, `.core.input`, `.core.lifecycle`, `.runner.integrity`, `.runner.processes`, `.runner.repeatability`; all 19 `lan-checks` core, network, adapter and runner cases except `core.permissions`; 25 `asset-pipeline` headless cases (source, layouts, animation, appearance, application icon, content relations and census, `hub.source`, packaged expectations and the four text cases); `asset-pipeline.rendered.hub-navigation`; `verification.graph`, `.runner`, `.pins`, `.register` and `.docs`.

The failed and blocked cases are grouped below by observed cause. Each cause cites the original result. Where a cause is inferred, the entry says so.

| # | Cases | Original result | Observed cause |
| --- | --- | --- | --- |
| F1 | `gameplay-checks.rendered.*` (9, including Critical `rendered.tracer`); `lan-checks.rendered.*` (12, including Critical `rendered.walkers`) | `START_ONE_ACTOR`; `RENDERED_DEADLINE ordinary host ready` | These inherited runners launch the unimported source project. `FoundationAppearanceAssets` (added in 06-10) loads atlases through `ResourceLoader` when the world is shown. In the logs this raised `No loader found for resource: res://assets/common/donor-kit.png` (9 times in the gameplay session log). The LAN host then showed "Could not host a session". In Phase 5 both groups passed. The Phase 6 groups import their own copy first and did not hit this |
| F2 | `verification.rules` nested R-46±, R-47±, `rules.generated-callsite-boundary`, `rules.xunit-producer-relocation`, `-seed`, `-bypass`, `-shape`, `-hash` | Fixture `clean.Exit` 1 | Fixture project copies omit `game/foundation/FoundationPublicWorld.cs` (added in 06-14), producing `CS2001`. The `verification.state` nested `state.restore-mutation` fixture build also exited 1, but its error was not extracted and may have the same cause |
| F3 | `verification.rules` nested `ac-08.complete`, R-50+, `rules.godot-callsite-boundary`, `rules.godot-companion-positive`, `rules.godot-companion-negative`, `rules.exemptions`; `gameplay-checks.core.permissions` | `REPOSITORY_SOURCE_HASH src/godot-game/core/CharacterPresentation.cs` | The bound repository source identity for the changed `core/CharacterPresentation.cs` is stale |
| F4 | `verification.rules` nested `edge.rule-03.adjacency`, `rules.generated-callsite-origin`, `rules.suppression-duplicate` | Expected `6a0cf431…ea271`, observed `5ced3273…afab4` | A pinned current asset-pipeline input hash is stale |
| F5 | `verification.rules` nested R-48+ and R-67+ | R-48: 43 violating test methods. R-67: the foundation file inventory differs | New Phase 6 tests and foundation files are not reconciled with the rule inventories |
| F6 | `lan-checks.core.permissions` | `CHILD_EXIT 2` after 4 min 22 s, `CONSUMER_COMPILATION … FoundationHud.cs(12,47): CS0535 IDisposable.Dispose()` | The consumer compilation analyzed by the Phase 3 permission proof no longer matches the changed `FoundationHud` |
| F7 | `verification.engine` nested `engine.generated-origin` (stage `origin-fixed`) and `engine.generated-boundary` (stage `boundary-calls-cohort-0`) | `PRODUCER_STAGE_FAILED` | Not yet diagnosed |
| F8 | `asset-pipeline.export.hub-consumer`, `asset-pipeline.export.portability` | `CHILD_FAILED common.bow managed-publish {"code":1}` | The source snapshot compiled for the common bow export fails with `FoundationCoordinator.cs(481,38): CS0117 'FoundationSession' does not contain 'CreateObserved'` |
| F9 | `asset-pipeline.content.native-permissions` | `NATIVE_PERMISSIONS_DISCOVERY_SELECTION` | The native permission census selection no longer matches the discovered methods. Not yet diagnosed further |
| F10 | `asset-pipeline.heldout.H14`; `.H16`; then blocked `H17`, `H18` and all 14 `UIH.CHARACTER-PREVIEW.*` / `UIH.UI-COLLECTIONS.*` | `evidence.hash is not a function`; `RENDERED_NATIVE_SETTLE … expected "walk-near" observed movement "jump"`; `Scenario reset failed: RENDERED_NATIVE_RESPONSE` | Defects in the Phase 6 inspection driver (`phase06-inspection.cjs`). After H16 the reset failed, which blocked the remaining 16 handoffs. None of the 18 handoffs has an accepted outcome |
| F11 | `phase06.solo.world`, `.movement`, `.inventory`, `.life` (fail); `.stash`, `.combat` (blocked) | `dash is not defined`; `RENDERED_DEADLINE solo panel keeps jump at ground`; `RENDERED_DEADLINE solo inventory-split`; reset `RENDERED_DEADLINE native event solo/27/true` with cleanup `TERMINAL_NORMAL_QUIT solo` | Defects in the solo driver: an undefined helper, then waits that never completed and an Escape reset that never registered. This was the first rendered execution of these drivers |
| F12 | `phase06.lan.population`, `.conflicts`, `.succession` | `ENOENT … ordinary-cleanup.json`; `RENDERED_DEADLINE guest-1 one authority withdrawal and corrected source`; reset and cleanup deadlines for guests 1–3 | Defects in the LAN driver's artifact paths and waits. The four population captures and four conflict captures were taken, but their cases did not pass |
| F13 | `phase06.headless.lan-privacy` | `place: 'Ready' !== 'Applied'` | Intermittent. The same source `17fb851` passed this case in invocations `e0efbd40-dffc-4095-ac07-885719bb58c4` and `a1c80cf3-c78f-415e-95a3-0e0875f2a809` |

Replay one case with `node src/godot-game/tools/game-tests/check.cjs --case <id> --review .local/temp/phase06-review.json`. Replay all failed and blocked cases with `node src/godot-game/tools/game-tests/check.cjs --failed .local/runs/game-tests/ee17949e-45ed-4486-9915-dc0b7029cdf1/summary.json --review FILE`.

### Image identities from run 1

All of these are decoded 1280×720 PNG files. They come from failed cases, so they are diagnostic history, not acceptance evidence.

| Case | Image (relative to the invocation directory) | Bytes | SHA-256 |
| --- | --- | ---: | --- |
| phase06.solo.movement | `62f18c32-…/solo/draw-movement-sprint-1.png` | 52593 | `378440dd587735ef464995a54b4448ec6173640098581f5fe19bf8308f394152` |
| phase06.solo.movement | `62f18c32-…/solo/draw-movement-air-2.png` | 53088 | `eb42fb86c5870899ce526e2a6c75bef12864432a623240e94eb65812ade747f9` |
| phase06.solo.inventory | `62f18c32-…/solo/draw-inventory-equipped-3.png` | 89234 | `32f2a6025f9d49f9c032dc0b8940138d09762025512f1c4c99ae67abc3a81588` |
| phase06.solo.inventory | `62f18c32-…/solo/draw-inventory-nested-4.png` | 98983 | `7c8e54b4c6f0ce1083da6d9ec6feed42f65856788331fc833684796103fa7f5a` |
| phase06.lan.population | `6d406c5e-…/host/draw-population-host-1.png` | 67272 | `54d38ef52ec06fa897f314f76fd37423c3d4eda645d88b0ce3ce908586d08070` |
| phase06.lan.population | `6d406c5e-…/guest-1/draw-population-guest-1-1.png` | 58811 | `39865716b7a471222f1f950399e5b8a9970ea1c4edc80f9e202f2ed9c9be2465` |
| phase06.lan.population | `6d406c5e-…/guest-2/draw-population-guest-2-1.png` | 58634 | `bc912097698c1ba7b6bdef373ba7c5f232061cb8d515f15b73354f3b04d771f4` |
| phase06.lan.population | `6d406c5e-…/guest-3/draw-population-guest-3-1.png` | 58603 | `12c0cf28ff05c49e4a296c9387c42c1867456b65d34c0dc7dab8327f146cd679` |
| phase06.lan.conflicts | `6d406c5e-…/host/draw-conflict-original-host-2.png` | 70475 | `7653716d2c0f64a70bcf974eaa91aadd3857cc77bd3433fc15adb359fb797a9d` |
| phase06.lan.conflicts | `6d406c5e-…/guest-1/draw-conflict-original-guest-1-2.png` | 113458 | `3172913c1039802880b7113639b1d07a2e15972007c688dd6726f676b2caf106` |
| phase06.lan.conflicts | `6d406c5e-…/guest-2/draw-conflict-original-guest-2-2.png` | 112414 | `9eeb2f13df6fd58fe88a4dee1a3241c9f14627648c32b4d148618e2acc500b59` |
| phase06.lan.conflicts | `6d406c5e-…/guest-3/draw-conflict-original-guest-3-2.png` | 112522 | `f2396d334d93d40c76b67a97b7bb1f91cb6b7b831e8765bd8049b5f27bdb81dd` |

The full directory names are `62f18c32-928d-407a-b1e3-6e9d1680f8e1` (solo) and `6d406c5e-0500-4c50-9b1e-d800ddc95f38` (LAN). The P1–P4 hub captures are in asset-pipeline run `873a3da6-6d7d-47f0-8583-f0b7f512193e/hub/`, with `rendered.hub-smoke-source-details.png` SHA-256 `dc3fbb9e63de48aee0dad832b16b490ccf3fee12fa7b744d93738d39291e5ac2`. They stay pending review.

### Qualitative findings from run 1 (diagnostic, not accepted)

These are observations from two of the captures, not acceptance judgments. The final run must repeat the review on its own captures.

- `draw-inventory-equipped-3.png`: Equipment item icons cover their slot labels ("Boots", "Torso clothing", "Legs", "Body armor"). Every carried container still shows an "Opening inventory…" row. The top-right help text runs past the panel's top edge and is cut off ("Rest is unsafe with an enemy nearby"). The hotbar slots truncate "Empty" to "Em".
- `draw-population-host-1.png`: The host roster reads "Hosting · 4/4 players". All four survivors stand at the same spot, so only one body sprite is visible under the stacked P1–P4 labels. Every roster name is "Player 1". The hotbar truncation repeats.
- P1–P4 image review was not performed. The objective run did not pass, so these judgments would not be credited.

### Records not yet produced

- Focused rerun of the failed and blocked cases on repaired, re-reviewed source.
- A fresh full run of the selected set on that source, with all 18 handoff outcomes and P1–P4 judgments.

## Run 2: focused repair rerun

These focused results are diagnostic. They are not the phase's final pass, and no case below receives Phase 6 credit from them. The P1–P4 judgments, the solo and four-role image judgments and the 18 handoff judgments stay pending for 06-15. Run 1 above is unchanged. The `.local/runs` artifacts are kept; 06-15 owns their cleanup after its final record.

| Field | Value |
| --- | --- |
| Invocation | `a0e0ad44-adde-4344-b584-75e7f99bc5e3` |
| Newly affected invocation | `3c8b5fa1-1391-4c1d-9bfa-c43a1afdfe85` |
| Tested revision | `987e12029ea68964cc048e912ab70f5538b11ebe` (clean worktree, `sourceChanged` false in both reports) |
| Review record | `.local/temp/phase06-review.json`: revision `987e12029ea68964cc048e912ab70f5538b11ebe`, reviewed source `c2d24563389c22628deb301dda4f4d2a8e791997`, reviewer "gsd-code-reviewer (fresh and independent; spawned by the /gsd-code-review workflow at deep depth; did not author the repairs or any earlier review in 06-REVIEW.md)", status `clean`. Between the reviewed source and the tested revision, only `.planning/` paths changed (`.planning/ROADMAP.md`, `.planning/STATE.md`, `06-24-SUMMARY.md`, `06-REVIEW.md`) |
| Repaired range | `a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d..c2d24563389c22628deb301dda4f4d2a8e791997` |
| Commands | `node src/godot-game/tools/game-tests/check.cjs --failed .local/runs/game-tests/ee17949e-45ed-4486-9915-dc0b7029cdf1/summary.json --review .local/temp/phase06-review.json`, then `node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.rendered.hub-navigation --review .local/temp/phase06-review.json` |
| Environment | Windows 10 Enterprise LTSC 2021 x64 (10.0.19044), Node 24.22.1, Godot 4.7.2 Mono, the development desktop at 1280×720 logical (3840×2160 physical at 288 DPI); each game client measured 1280×720 physical pixels; PowerShell 7 (`pwsh`) on PATH for the run only |
| Timing | `--failed`: started `2026-11-27T16:42:44.383Z`, finished `2026-11-27T19:06:22.917Z` (8618.534 s). `--case`: started `2026-11-27T19:07:00.399Z`, finished `2026-11-27T19:09:24.462Z` (144.063 s) |
| Status | `--failed`: `fail`, 28 passed, 13 failed, 16 blocked of 57. `--case`: `needs_review`, 1 passed, 0 failed, 0 blocked of 1, with P1–P4 pending review |
| Selection | `--failed` selected all 57 run-1 failed and blocked cases (`repairOf` `ee17949e-45ed-4486-9915-dc0b7029cdf1`); the three verification groups were narrowed to their failed members (21, 2 and 1 nested). `--case` selected the newly affected case below |

### Newly affected selection

The selection used the read-only catalog `select()` in phase-end mode for phase 6, with base `a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d` and the 101 paths changed up to the tested revision. It selected 119 cases, 49 of them tier 3. Tier-3 rows whose reason is "declared input changed" or "affected by …" were kept, and the 57 run-1 failed and blocked ids were removed. One case remains:

- `asset-pipeline.rendered.hub-navigation`: affected by inspection, asset-rendered (it passed in run 1).

Newly affected rendered cases: `asset-pipeline.rendered.hub-navigation`

### Groups

| Invocation | Group | Pass | Fail | Blocked | Reported launches | Wall time |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| `a0e0ad44` | phase06.solo | 3 | 3 | 0 | 1 | 61.269 s |
| `a0e0ad44` | phase06.lan | 1 | 2 | 0 | 4 | 46.854 s |
| `a0e0ad44` | phase06.headless | 1 | 0 | 0 | 4 | 24.488 s |
| `a0e0ad44` | gameplay-checks.headless | 1 | 0 | 0 | 0 | 1015.587 s |
| `a0e0ad44` | gameplay-checks.rendered | 7 | 2 | 0 | 1 | 230.052 s |
| `a0e0ad44` | lan-checks.headless | 1 | 0 | 0 | 0 | 261.833 s |
| `a0e0ad44` | lan-checks.rendered | 8 | 4 | 0 | 14 | 138.536 s |
| `a0e0ad44` | asset-pipeline.headless | 3 | 0 | 0 | 0 | 4069.849 s |
| `a0e0ad44` | phase06.inspection | 0 | 2 | 16 | 1 | 62.510 s |
| `a0e0ad44` | verification.rules | 1 (21 of 21 nested) | 0 | 0 | 0 | 1884.573 s |
| `a0e0ad44` | verification.engine | 1 (2 of 2 nested) | 0 | 0 | 0 | 777.028 s |
| `a0e0ad44` | verification.state | 1 (1 of 1 nested) | 0 | 0 | 0 | 26.827 s |
| `3c8b5fa1` | asset-pipeline.rendered | 1 | 0 | 0 | 1 | 123.942 s |

The report now records launch counts and per-case durations for the Phase 6 scenario groups (the run-1 report defect). Cases run by an inherited suite still report `durationMs` 0, and their time is in the group wall time.

### Case outcomes

Passed in `a0e0ad44` (28): `phase06.solo.world` (1.134 s), `phase06.solo.movement` (2.442 s), `phase06.solo.life` (3.165 s), `phase06.lan.succession` (5.476 s), `phase06.headless.lan-privacy` (12.600 s), `gameplay-checks.core.permissions`, `gameplay-checks.rendered.tracer` (Critical), `gameplay-checks.rendered.menu`, `gameplay-checks.rendered.directions`, `gameplay-checks.rendered.distance`, `gameplay-checks.rendered.world`, `gameplay-checks.rendered.lifecycle`, `gameplay-checks.rendered.quit`, `lan-checks.core.permissions`, `lan-checks.rendered.entry`, `lan-checks.rendered.browser`, `lan-checks.rendered.join-errors`, `lan-checks.rendered.profiles`, `lan-checks.rendered.focus`, `lan-checks.rendered.admission`, `lan-checks.rendered.guest-lifecycle`, `lan-checks.rendered.host-loss`, `asset-pipeline.content.native-permissions` (3921.853 s), `asset-pipeline.export.hub-consumer` (46.669 s), `asset-pipeline.export.portability` (100.971 s), `verification.rules`, `verification.engine`, `verification.state`.

Passed in `3c8b5fa1` (1): `asset-pipeline.rendered.hub-navigation`, with P1–P4 pending review.

Failed and blocked in `a0e0ad44`. Evidence paths are relative to `.local/runs/game-tests/a0e0ad44-adde-4344-b584-75e7f99bc5e3/`. Where a cause is inferred rather than proven, the entry says so.

| # | Case | Outcome | Original message | Evidence | Observed cause |
| --- | --- | --- | --- | --- | --- |
| R2-1 | `phase06.solo.inventory` | fail (12.201 s) | `RENDERED_DEADLINE solo inventory-equipped` | `child-83a54bf4-cf45-4d28-89f8-73a9bdeb7899.json`; `de3bd518-7676-427b-a421-01e5f88568cb/solo/draws.ndjson` | Proven symptom: from draw 211 to 934 (about 12 s) the open workspace shows "Opening inventory…" for all three carried storage items, so `settledView` never holds. The observed readback carries the `own.1`, `own.2` and `own.3` views with epoch 1 and token 0 for the whole span. The cause inside the workspace is not yet proven |
| R2-2 | `phase06.solo.stash` | fail (13.073 s) | `RENDERED_DEADLINE solo stash-withdrawal` | as R2-1, plus `native-input-19312.json` records 53 to 55 | Proven: the Ctrl-click on the center of `Item_25` (IT-01, 100 money) at client (840, 260) submitted no request. `runAction.sequence` stays 7 ("browse", "Ready") from draw 988 until the inventory closes at draw 1709. The cause is not proven |
| R2-3 | `phase06.solo.combat` | fail (12.095 s) | `RENDERED_DEADLINE solo combat-ready` | as R2-1 | Proven: `draw.crosshair` is false in all 2619 solo draws. `FoundationDrawObserver.cs:75` reads `_root.GetNodeOrNull<Node2D>("Aim")`, but `FoundationWorldInput.cs:44-45` adds the `Aim` node under the `WorldInput` surface, not under the root, so the observer can never report it. `ArrowReady`, the IT-02 quantity and the HUD were satisfied from draw 1717 |
| R2-4 | `phase06.lan.population` | fail (1.669 s) | `DRIVER_ACK {"nonce":"9a73dc35-1f52-4795-bf90-af39b358ec06","ok":false,"error":"DRIVER_MOUSE_POINT_NOT_OWNED","sequence":13,"observedAfter":"3001889300822100"}` | `child-04863267-e9ed-4606-bbce-eb314440f8ab.json`; `9a73dc35-1f52-4795-bf90-af39b358ec06/native-input-20080.json` | A window of another process covered the guest-2 client point at sequence 13. Inferred: an overlapping window on the desktop (see R2-8) |
| R2-5 | `phase06.lan.conflicts` | fail (18.086 s) | `SUBJECT_TIMEOUT one authority withdrawal` | as R2-4, records 64 to 67 | Proven: guest-1 held Ctrl for 78 ms, moved the pointer and clicked the center of `Item_25` at (840, 260), then released Ctrl. No quick request followed. This is the same failure as R2-2, now reproduced with Ctrl held across frames, which rules out the 06-22 "ordering within one input burst" explanation |
| R2-6 | `gameplay-checks.rendered.focus` | fail | `INPUT_EXIT 1: SINK_POINT_NOT_OWNED` (windows-input.ps1:112) | `child-8f882aba-745e-4639-92ad-d9f4f2a86272.json` | The topmost focus sink did not become foreground after `Activate`, and another window covered its caption point. Inferred: an external window held the foreground (see R2-8) |
| R2-7 | `gameplay-checks.rendered.resources` | fail | `UNDECLARED_RESOURCE assets/ui/InspectionTheme.tres` | as R2-6 | Proven: `gameplay-checks/rendered.cjs:366` requires every `res://` literal in `foundation/*.cs` and `*.tscn` to name a foundation file. `FoundationInventoryArt.cs:31` loads `res://assets/ui/InspectionTheme.tres`. Run 1 hid this behind F1 |
| R2-8 | `lan-checks.rendered.moving-join`, `lan-checks.rendered.walkers` (Critical), `lan-checks.rendered.timing` | fail | `DRIVER_ACK {"nonce":"65c172ab-5d2b-405d-9d26-37b39bf07052","ok":false,"error":"DRIVER_MOUSE_POINT_NOT_OWNED","sequence":4,…}` (moving-join), `…"sequence":24,…` (walkers), `…"sequence":60,…` (timing) | `65c172ab-5d2b-405d-9d26-37b39bf07052/rendered.moving-join.failure.log`, `rendered.walkers.failure.log`, `rendered.timing.failure.log`, `summary.json` | Another process's window was on top at the owned click point. No launch passes a window position, so the peer windows probably open at the same centered position and overlap. Inferred: an external foreground holder (R2-10 proves one) blocked the driver's foreground acquisition |
| R2-9 | `lan-checks.rendered.host-end` | fail | `DRIVER_ACK {"nonce":"65c172ab-5d2b-405d-9d26-37b39bf07052","ok":false,"error":"DRIVER_FOCUS_POINT_NOT_OWNED target=5835174 actual=40308046 point=1450,617","sequence":1,"observedAfter":"3003577613937900"}` | `65c172ab-5d2b-405d-9d26-37b39bf07052/rendered.host-end.failure.log` | The host caption point (1450, 617) belonged to window 40308046, which no longer exists after the run. Same inferred cause as R2-8 |
| R2-10 | `asset-pipeline.heldout.H14` | fail (12.817 s) | `RENDERED_HELPER_EXIT 1 #< CLIXML …OWNED_FOCUS_UNACKNOWLEDGED expectedWindow=9045448 expectedPid=34188 actualWindow=58984652 actualPid=32484` (windows-input.ps1:71; CLIXML progress records elided) | `child-f8be179e-8972-4632-a34f-0dbd8e86241a.json`; `7c4a677c-5883-49e5-9cc8-9ae81f5c7451/asset-pipeline.heldout.H14-reset.json` | Proven environmental: after the run, window 58984652 is the maximized main window of the Claude desktop app (`Claude.exe`, pid 32484, class `Chrome_WidgetWin_1`, covering the whole display). It held the foreground when the inspection game expected it |
| R2-11 | `asset-pipeline.heldout.H16`, `asset-pipeline.heldout.H17`, `asset-pipeline.heldout.H18`, `asset-pipeline.UIH.CHARACTER-PREVIEW.empty`, `asset-pipeline.UIH.CHARACTER-PREVIEW.loading`, `asset-pipeline.UIH.CHARACTER-PREVIEW.error`, `asset-pipeline.UIH.CHARACTER-PREVIEW.partial`, `asset-pipeline.UIH.CHARACTER-PREVIEW.overflow`, `asset-pipeline.UIH.CHARACTER-PREVIEW.long-text`, `asset-pipeline.UIH.UI-COLLECTIONS.empty`, `asset-pipeline.UIH.UI-COLLECTIONS.loading`, `asset-pipeline.UIH.UI-COLLECTIONS.error`, `asset-pipeline.UIH.UI-COLLECTIONS.populated`, `asset-pipeline.UIH.UI-COLLECTIONS.partial`, `asset-pipeline.UIH.UI-COLLECTIONS.overflow`, `asset-pipeline.UIH.UI-COLLECTIONS.zero-one-many` | blocked (16) | `Scenario reset failed: INSPECTION_RESET_BASELINE asset-pipeline.heldout.H14: RENDERED_HELPER_EXIT 1 #< CLIXML …` | `child-f8be179e-8972-4632-a34f-0dbd8e86241a.json` | The reset after R2-10 failed, so none of these handoffs ran |
| R2-12 | `asset-pipeline.UIH.UI-COLLECTIONS.long-text` | fail | Same reset message as R2-11 | as R2-11 | Same cause as R2-11. The runner reports this last case as `fail`, not `blocked` |

What 06-24 left for this run to prove on screen:

- **The `clipped` field in a real rendered draw.** All 2619 solo draws report `clipped: false` for every observed control. No rendered draw has yet shown the field set. The stash image `draw-stash-first-6.png` shows overlapping controls ("Next" covers "Go to page", and the page field covers "Previous"). These are overlaps between controls, not text clipped inside its own control, so they do not test the field.
- **The cause of the 06-22 guest-1 Ctrl-click.** Still not proven, but narrowed. The click lands on the center of the money cell (draw positions are control centers), and Ctrl was held for 78 ms in the LAN case, so neither the coordinate nor the ordering within one burst explains it. The withdrawal also fails in solo (R2-2 and R2-5).
- **The near-successor smooth handover.** `phase06.lan.succession` passed, so its handover predicate held on gap-free spans in this run.

### Image identities from run 2

All images are decoded 1280×720 PNG files. They are diagnostic history, not acceptance evidence. Paths are relative to `.local/runs/game-tests/a0e0ad44-adde-4344-b584-75e7f99bc5e3/`.

| Image | Bytes | SHA-256 |
| --- | ---: | --- |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-world-walked-1.png` | 51349 | `f629752c2faad70f0c4a2be7c86557b1131fc3986bbb9ace43a3378fd7aa6730` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-movement-sprint-2.png` | 51922 | `e58b28e871d37d2f981dabe6e2a58b911dd292f929950a57d8e51f6ef13ddf52` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-movement-air-3.png` | 52329 | `5e64a58e67182b3ac5edcb7621eeb587d4e668aea0c9f7482bb3496dbccdb4d1` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-nearby-outline-0-4.png` | 106494 | `23e7902b95d1eb858084e71bae5dc493aa15fc85f3c26a59a4fd2e5565fd47b2` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-nearby-outline-1-5.png` | 108696 | `83a7238b5fa3f1c24aef78b4d56a6d48942fb74afe9bbdea8d188d9a9f744d10` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-stash-first-6.png` | 106038 | `aa7392ab62ce9766cd806f534f79e68c2873e10f38f439e60b4020ad3637dfeb` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-stash-next-7.png` | 94534 | `30638429b7dda35871b55c55d071570e9b46ac76783cb56bf19c20e77a9e80c5` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-life-death-8.png` | 31144 | `3b7da4b0de54a9b6e6f26275a00d88b2f5d33485771a9503b21ca0a34e77ef20` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-life-ready-9.png` | 27245 | `1294396fce925fbb3617fe97df9a2427306a82be54b14c0e1c07abfa90ee06a0` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-life-transfer-10.png` | 54697 | `6199c3552caadddcab4063b6acfeb85be2ac56ad26b3437259b8a5ac28ca020a` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-life-cleanup-11.png` | 54697 | `6199c3552caadddcab4063b6acfeb85be2ac56ad26b3437259b8a5ac28ca020a` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-life-distant-death-12.png` | 27552 | `14c9a0bdbd82c07aa15ceb3bea10355d174f8af2bc95e5c6101dfa5086ec52d1` |
| `de3bd518-7676-427b-a421-01e5f88568cb/solo/draw-life-distant-cut-13.png` | 54640 | `74d2a6190d63c4029d1d71f7cf29c36498030149c87731304d277cad2fd90708` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/host/draw-conflict-original-host-1.png` | 70393 | `2c9befbb9b1144c86b9178fd25931f22c9de57006b61b1381d79dd34615c7fdb` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/guest-1/draw-conflict-original-guest-1-1.png` | 116108 | `5ceec130acc6736aaf3d647bfad68bc779b17429c33b374c91c6fc7fd1dfcac3` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/guest-2/draw-conflict-original-guest-2-1.png` | 115505 | `d229560a4d11b6c209abe540e79d54c62475473cf2bad0d6c10fb025a8f296f2` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/guest-3/draw-conflict-original-guest-3-1.png` | 115348 | `8d3049b81700771e7137c69e737d249e0e4eaf7df909df275ca88f67453cf385` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/host/draw-succession-death-2.png` | 57305 | `587c5826720bff9bb81c1a2f014abe636a353cfb86424940ca8f67dd564f86bf` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/host/draw-succession-next-3.png` | 55693 | `34fbc58e6c0a7797cbad9b3e5ee0bdb378486cc2bf355d77bbf328f0f3b12183` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/host/draw-succession-previous-4.png` | 55938 | `37a5ece3ef9879690ebdcd4b1f7f1a8872b240622ffcc65aa68436742f482683` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/host/draw-succession-chosen-5.png` | 54792 | `700f38d736b66a55b964e755d59571196b62e8b29822248248093a1d4b931721` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/host/draw-succession-lost-6.png` | 51928 | `a9672c63a895e5a7ad550d9ecae17acaeb0f8a3dffc9ba068945cb4d3c478955` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/host/draw-succession-transfer-7.png` | 76220 | `e2088930f8a2212150ba9a71f420cf8180a4180e16ab0a150e7c4ef5ab3698eb` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/host/draw-succession-current-host-8.png` | 76318 | `ef9ab2128884f79ca00cde21c1c63d32cf26d45f75e8d27095eb2f88e6ac3686` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/guest-1/draw-succession-current-guest-1-2.png` | 63351 | `9b5f0a8dc6a9015af7e3117bc5970e0bc09364fd11b951dbb525f6841560c9c0` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/guest-2/draw-succession-current-guest-2-2.png` | 63338 | `4a7d5e055cdf7999eecdcb2f38f1fc755c439ee8d5775ddf09cc2e8dd4ac3f23` |
| `9a73dc35-1f52-4795-bf90-af39b358ec06/guest-3/draw-succession-current-guest-3-2.png` | 63410 | `0d640d5fac8506723680ecd5772ee1f7ccf221a38dfd8ce2103b3f869c4b1f48` |

The 42 inherited lan-checks captures are under `65c172ab-5d2b-405d-9d26-37b39bf07052/G2/` and `G3/`, and each case's `summary.json` lists them with their hashes. The P1–P4 hub captures of `3c8b5fa1` are in asset-pipeline run `1d94dc06-8ee4-4a02-9fd7-5817dff48c47/hub/`. Among them, `rendered.hub-smoke-source-details.png` is 145345 bytes with SHA-256 `a72acfec1066dfa579da4844b30e613db04d805a0a946c85eedb42588c4874e3`, and `rendered.hub-smoke-death-retired.png` is 79490 bytes with SHA-256 `174ece820527acabaac1cf103d19d9538f42bb9769b7c950d0d132649d74337e`. They stay pending review.

### Run 2 outcome

Run 2 did not pass: 13 failed and 16 blocked in `a0e0ad44`. No repair is made in this record. Each repair round goes to a fresh executor and a fresh independent reviewer, and is appended here as Run 2a, 2b and so on. The fresh full selected-set run stays with 06-15.

## Run 2a: focused attempt after repair rounds 1–2

These focused results are diagnostic. They are not the phase's final pass, and no case below receives Phase 6 credit from them. The P1–P4 judgments, the solo and four-role image judgments and the 18 handoff judgments stay pending for 06-15. Run 1 and Run 2 above are unchanged. The `.local/runs` artifacts are kept; 06-15 owns their cleanup after its final record.

| Field | Value |
| --- | --- |
| Invocation | `335e06c4-0187-48d8-8fd5-22b53a472f2a` |
| Newly affected invocation | `13ecb196-ae48-4628-8930-c98adb93a4d1` |
| Tested revision | `2e7e176f094f858d648b3d48c1a9532126376e48` (clean worktree, `sourceChanged` false in both reports) |
| Review record | `.local/temp/phase06-review.json`: revision `2e7e176f094f858d648b3d48c1a9532126376e48`, reviewed source `f0890cab1a02f80371a8dc52a8fc67fbb0ce8fe6`, reviewer "gsd-code-reviewer (fresh and independent; spawned by the /gsd-code-review workflow at deep depth; did not author the repairs); RR-06..08 accepted by the orchestrator", status `clean`, reviewed range `a9ac95f..f0890cab1a02f80371a8dc52a8fc67fbb0ce8fe6`. Between the reviewed source and the tested revision, only `.planning/phases/06-survivors-inventory-and-combat/06-REVIEW.md` changed (the RR repair review and the orchestrator acceptance of RR-06 to RR-08) |
| Repaired range | `a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d..f0890cab1a02f80371a8dc52a8fc67fbb0ce8fe6` (repair rounds 1 and 2) |
| Commands | `node src/godot-game/tools/game-tests/check.cjs --failed .local/runs/game-tests/ee17949e-45ed-4486-9915-dc0b7029cdf1/summary.json --review .local/temp/phase06-review.json`, then `node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.rendered.hub-navigation --review .local/temp/phase06-review.json` |
| Environment | Windows 10 Enterprise LTSC 2021 x64 (10.0.19044), Node 24.22.1, Godot 4.7.2 Mono, the development desktop at 1280×720 logical (3840×2160 physical); each game client measured 1280×720 physical pixels; PowerShell 7 (`pwsh`) on PATH for the run only. The Claude desktop app was minimized for the whole run. A read-only watcher logged every foreground change (`.local/temp/06-25-r2a-foreground.log`): the foreground passed only between the owned Godot clients, the drivers' own focus sink windows, the Windows desktop (`explorer` `Progman`/`Shell_TrayWnd`) and Idle. No other process took the foreground. MSI Afterburner with RTSS and the AMD Radeon overlay (`amdow`) were running; their performance overlay is drawn into the captures (top-right GPU/CPU/RAM/FPS block). No user window was minimized or closed |
| Timing | `--failed`: started `2026-11-28T03:29:08.560Z`, finished `2026-11-28T06:09:42.151Z` (9633.591 s). `--case`: started `2026-11-28T06:10:13.073Z`, finished `2026-11-28T06:12:38.234Z` (145.161 s) |
| Status | `--failed`: `fail`, 27 passed, 28 failed, 2 blocked of 57. `--case`: `needs_review`, 1 passed, 0 failed, 0 blocked of 1, with P1–P4 pending review |
| Selection | `--failed` selected all 57 run-1 failed and blocked cases (`repairOf` `ee17949e-45ed-4486-9915-dc0b7029cdf1`); the three verification groups were narrowed to their failed members (21, 2 and 1 nested). `--case` selected the newly affected case below |

### Newly affected selection

The selection was recomputed after both repair rounds with the read-only catalog `select()` in phase-end mode for phase 6, with base `a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d` and the 106 paths changed up to the tested revision. It selected 121 cases. Tier-3 rows whose reason is "declared input changed" or "affected by …" were kept, and the 57 run-1 failed and blocked ids were removed. One case remains, the same as in Run 2:

- `asset-pipeline.rendered.hub-navigation`: affected by inspection, asset-rendered (it passed in run 1 and in Run 2).

Newly affected rendered cases: `asset-pipeline.rendered.hub-navigation`

### Groups

| Invocation | Group | Pass | Fail | Blocked | Reported launches | Wall time |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| `335e06c4` | phase06.solo | 0 | 4 | 2 | 1 | 52.267 s |
| `335e06c4` | phase06.lan | 2 | 1 | 0 | 4 | 32.296 s |
| `335e06c4` | phase06.headless | 1 | 0 | 0 | 4 | 24.584 s |
| `335e06c4` | gameplay-checks.headless | 1 | 0 | 0 | 0 | 1025.632 s |
| `335e06c4` | gameplay-checks.rendered | 8 | 1 | 0 | 1 | 229.317 s |
| `335e06c4` | lan-checks.headless | 1 | 0 | 0 | 0 | 266.339 s |
| `335e06c4` | lan-checks.rendered | 8 | 4 | 0 | 14 | 138.652 s |
| `335e06c4` | asset-pipeline.headless | 3 | 0 | 0 | 0 | 4094.678 s |
| `335e06c4` | phase06.inspection | 0 | 18 | 0 | 1 | 1049.298 s |
| `335e06c4` | verification.rules | 1 (21 of 21 nested) | 0 | 0 | 0 | 1889.587 s |
| `335e06c4` | verification.engine | 1 (2 of 2 nested) | 0 | 0 | 0 | 783.270 s |
| `335e06c4` | verification.state | 1 (1 of 1 nested) | 0 | 0 | 0 | 28.159 s |
| `13ecb196` | asset-pipeline.rendered | 1 | 0 | 0 | 1 | 124.114 s |

Cases run by an inherited suite still report `durationMs` 0; their time is in the group wall time.

### Case outcomes

Passed in `335e06c4` (27): `phase06.lan.conflicts` (4.590 s), `phase06.lan.succession` (5.417 s), `phase06.headless.lan-privacy` (12.701 s), `gameplay-checks.core.permissions`, `gameplay-checks.rendered.tracer` (Critical), `gameplay-checks.rendered.menu`, `gameplay-checks.rendered.directions`, `gameplay-checks.rendered.distance`, `gameplay-checks.rendered.world`, `gameplay-checks.rendered.lifecycle`, `gameplay-checks.rendered.quit`, `gameplay-checks.rendered.resources`, `lan-checks.core.permissions`, `lan-checks.rendered.entry`, `lan-checks.rendered.browser`, `lan-checks.rendered.join-errors`, `lan-checks.rendered.profiles`, `lan-checks.rendered.focus`, `lan-checks.rendered.admission`, `lan-checks.rendered.guest-lifecycle`, `lan-checks.rendered.host-loss`, `asset-pipeline.content.native-permissions` (3946.133 s), `asset-pipeline.export.hub-consumer` (46.798 s), `asset-pipeline.export.portability` (101.402 s), `verification.rules`, `verification.engine`, `verification.state`.

Compared with Run 2, `phase06.lan.conflicts` and `gameplay-checks.rendered.resources` now pass. `phase06.solo.world` and `phase06.solo.movement` passed their own steps but are reported failed through the solo group cleanup (R2a-1).

Passed in `13ecb196` (1): `asset-pipeline.rendered.hub-navigation`, with P1–P4 pending review.

Failed and blocked in `335e06c4`. Evidence paths are relative to `.local/runs/game-tests/335e06c4-0187-48d8-8fd5-22b53a472f2a/`. Where a cause is inferred rather than proven, the entry says so.

| # | Case | Outcome | Original message | Evidence | Observed cause |
| --- | --- | --- | --- | --- | --- |
| R2a-1 | `phase06.solo.inventory` | fail (12.915 s) | `RENDERED_DEADLINE solo exact integral split preserves source identity` | `child-79ab3af2-d338-4c8a-bf7b-18b73aca92b8.json`; `0cef6c79-e7d9-4538-a81c-88232e65421d/solo/console.log` lines 4–16; `…/solo/draws.ndjson` | Proven: the game threw `System.InvalidOperationException: The requested operation requires an element of type 'Number', but the target element has type 'Null'` in `FoundationInventoryWorkspace.PointerRelease` (`FoundationInventoryWorkspace.Input.cs:124`). An own-inventory `place` or `merge` request carries `Token = null` (`FoundationInventoryInteraction.cs:464-467` returns `null` when the drag is not external), and `TryGetProperty("Token")` succeeds on the JSON null, so `token.GetInt64()` throws. The release after "Move quantity" therefore never sent the split and never retired the placement preview. The line dates from 80d1485, before run 1; no earlier on-screen run reached a pointer drop |
| R2a-2 | `phase06.solo.stash`, `phase06.solo.combat` | blocked | `Scenario reset failed: solo Escape acknowledged without its D-14 effect "cancel drag or split preview": RENDERED_DEADLINE solo Escape effect cancel drag or split preview` | as R2a-1 | Same cause as R2a-1: the preview left behind by the exception stayed visible, so the D-14 Escape step could not observe it cancelled. The stash Ctrl-click did not run |
| R2a-3 | `phase06.solo.life` | fail | Same reset message as R2a-2, then `Owned scenario cleanup failed: … 'TERMINAL_NORMAL_QUIT solo'` | as R2a-1 | Same cause as R2a-1. The native `ERROR:` line in the console also fails the ordinary cleanup assertion (`phase06-headless.cjs:130-131`) |
| R2a-4 | `phase06.solo.world`, `phase06.solo.movement` | fail (1.124 s, 2.396 s) | `ENOENT: no such file or directory, lstat '…\0cef6c79-e7d9-4538-a81c-88232e65421d\ordinary-cleanup.json'` | `0cef6c79-e7d9-4538-a81c-88232e65421d/results.json`, `phase06.solo.world.json`, `phase06.solo.movement.json` | Proven collateral of R2a-1: both cases wrote their evidence, but every case lists the group's `ordinary-cleanup.json` (`phase06-headless.cjs:160-161`), which is written only after an ordinary Quit (`:139-148`). The failed cleanup left it missing |
| R2a-5 | `phase06.lan.population` | fail (1.656 s) | `DRIVER_ACK {"nonce":"128e5f25-aefc-4d1b-bd23-32b781185954","ok":false,"error":"DRIVER_MOUSE_POINT_NOT_OWNED","sequence":13,"observedAfter":"3040664764485100"}` | `child-4593ca34-bd68-4448-8eb4-8be229f8d137.json`; `128e5f25-aefc-4d1b-bd23-32b781185954/native-input-3548.json` records 11–14 | Proven symptom: guest-2 (pid 32928, window 127666716) was acknowledged as foreground at sequence 12, and the click on its `Menu/Join` at sequence 13 found another process's window at the point. This is the same case and the same sequence as R2-4, while the foreground log shows no external process. Inferred: an owned peer window, or a hit-testable overlay window, lies over the foreground client (see R2a-7) |
| R2a-6 | `gameplay-checks.rendered.focus` | fail (2.839 s) | `INPUT_EXIT 1: SINK_POINT_NOT_OWNED` (`gameplay-checks/windows-input.ps1:112`) | `child-f447bccf-7d55-45b6-84e1-3ede1a302d2e.json` | The topmost focus sink was not foreground after `Activate`, and its caption point belonged to another window. Same failure as R2-6, now without any external foreground holder. Not proven which window covered the point: since the sink is topmost, only another topmost window can cover it (candidates: the taskbar or the AMD overlay window) |
| R2a-7 | `lan-checks.rendered.moving-join`, `lan-checks.rendered.walkers` (Critical), `lan-checks.rendered.timing` | fail | `DRIVER_ACK {"nonce":"ac2ede97-e39d-4474-a815-8b77df7da101","ok":false,"error":"DRIVER_MOUSE_POINT_NOT_OWNED","sequence":4,…}` (moving-join), `…"sequence":24,…` (walkers), `…"sequence":60,…` (timing) | `ac2ede97-e39d-4474-a815-8b77df7da101/rendered.moving-join.failure.log`, `rendered.walkers.failure.log`, `rendered.timing.failure.log`, `summary.json` | Same sequences as R2-8 in Run 2, so the failure is deterministic, not an external window. No peer launch passes a window position (`lan-checks/network.cjs:56`), so the four 1280×720 clients open at the same place and overlap. Inferred: an owned peer window stays above the client the driver focused |
| R2a-8 | `lan-checks.rendered.host-end` | fail | `DRIVER_ACK {"nonce":"ac2ede97-e39d-4474-a815-8b77df7da101","ok":false,"error":"DRIVER_FOCUS_POINT_NOT_OWNED target=6620682 actual=86183602 point=1450,617","sequence":1,"observedAfter":"3042352655480000"}` | `ac2ede97-e39d-4474-a815-8b77df7da101/rendered.host-end.failure.log` | Proven: the foreground log maps window 6620682 to Godot pid 13072 and window 86183602 to Godot pid 23876, both owned lan-checks clients. An owned peer window covered the host caption point (1450, 617), the same point as R2-9. This refutes Run 2's external-window inference for this case |
| R2a-9 | `asset-pipeline.heldout.H14`, `asset-pipeline.heldout.H18` | fail (82.765 s, 58.796 s) | `INSPECTION_SCROLL_REQUIRED /root/Inspection/Shell/Workspace/Preview/Surface/Current/Character/Controls/Slots/left/Select` | `child-3dc43da2-3749-4873-9f24-e0250566e744.json`; `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H14-*` | Proven symptom: 20 scrollbar track clicks (`asset-pipeline/scenarios.cjs:1273-1285`) never brought the `left` equipment-slot selector into the 320-pixel `Character/Controls` scroll view. Earlier selectors further down (`Ammunition`, `BowSubject` in H14) were reached. Not proven: the failing reveal must scroll upward, so the upward page click at track offset 12 px does not take effect, or the container's `FollowFocus = true` (`CharacterPreview.cs:188-193`) scrolls back to the still-focused selector below |
| R2a-10 | `asset-pipeline.heldout.H17` | fail (82.475 s) | `RENDERED_NATIVE_SETTLE …/ResourcePreview.metadata/resource_view expected "current death" observed "{\"id\":\"CH-13\",\"state\":\"persistent corpse\",…\"controller\":\"CorpsePreview\",…}" \| …/Art/Corpse.metadata/appearance expected 1 …` | `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H17-explicit-death-input.json`, `…-explicit-death-failed-settle.json` | Proven symptom: the State option click at (900, 645), two Down taps and Enter were all acknowledged, but the corpse stayed in `persistent corpse`. The same `choose` helper works for character selectors in the same case. Not proven: whether the popup opened, or whether the item list read from `popup/item_*` matched the rebuilt State control |
| R2a-11 | `asset-pipeline.UIH.UI-COLLECTIONS.empty`, `.loading`, `.error`, `.populated`, `.partial`, `.overflow`, `.zero-one-many` | fail | `RENDERED_NATIVE_SETTLE …/ResourcePreview.metadata/resource_view expected "InventoryLayouts" observed "{\"id\":\"UI-12\",\"state\":\"inventory.broken\",…\"controller\":\"Source-bound UI presenter\",…}"` | `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.UI-COLLECTIONS.*` | Proven scenario defect: `phase06-inspection.cjs:383,393` checks `view('controller', 'InventoryLayouts')`, and `view` reads `ResourcePreview` `metadata/resource_view` (`:102`). That metadata names a UI resource's controller `Source-bound UI presenter` (`IdentityKit.cs:1213`). `InventoryLayouts` is written only to the `Surface` `ui_library` metadata (`IdentityKit.cs:856-859`). The state (`inventory.broken`) matched |
| R2a-12 | `asset-pipeline.UIH.UI-COLLECTIONS.long-text` | fail | `RENDERED_GROUP_DEADLINE`, then `Owned scenario cleanup failed: EVIDENCE_PROCESS inspection-hub` | as R2a-11 | The group budget ran out after the earlier failures. The hub process then did not reach an ordinary end. Not separately diagnosed |
| R2a-13 | `asset-pipeline.heldout.H16`, `asset-pipeline.UIH.CHARACTER-PREVIEW.empty`, `.loading`, `.error`, `.partial`, `.overflow`, `.long-text` | fail | `ENOENT: no such file or directory, lstat '…\d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a\inspection-cleanup.json'` | `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/results.json` and the cases' own `.json` records | Proven collateral of R2a-12: these seven cases completed their own steps and images, but they list the group's `inspection-cleanup.json`, which the failed cleanup never wrote |

Shared root causes: R2a-1 to R2a-4 (6 cases) are one game defect. R2a-11 to R2a-13 (15 cases) follow one scenario defect plus the group deadline. R2a-5 to R2a-8 (6 cases) are the window-coverage family. R2a-9 (2 cases) and R2a-10 (1 case) are separate inspection-driving defects.

### ItemPress records (RR-06 comparison)

`input.phase06.ItemPress` was traced through every draw of each role (`.local/temp/06-25-r2a-press.cjs`), comparing `Sequence` before and after each click as RR-06 requires.

- **Conflicts Ctrl-click (guest-1, `128e5f25-…/guest-1/draws.ndjson`).** Before the click no press existed (`ItemPress` null through draw 441). At draw 442 `Sequence` went from none to 1 with `Item` 25 (the IT-01 money cell), `External` true, `Button` left, `CtrlFlag` false, `CtrlHeld` true, `DrawnToken` 3 = `ExternalToken` 3, `ExternalKind` `Stash` and `Branch` `quick`. The quick withdrawal followed and the case passed. This proves the Run 1 and Run 2 Ctrl-click cause: the native click's `InputEventMouseButton.CtrlPressed` flag is false while Ctrl is physically held, and only the held-key fallback in the RR-01 press branch selects `quick`. The record disappears while the inventory is closed (draws 548 and 895) and reappears with the same `Sequence` 1 when it reopens at draw 584. That is the RR-06 staleness, visible but harmless here because the sequence was compared. `loserPress` is null because the losing guest never pressed an item.
- **Stash Ctrl-click (solo).** Not reached: the stash case was blocked by R2a-2. The solo record shows four presses, all before the exception: three right-click menus (sequences 1–3, `Branch` `menu`) and the Shift-press on the berries (sequence 4, `ShiftFlag` true, `ShiftHeld` true, `Branch` `split`, draw 229). No later press was recorded, as expected, since the split drop goes through `PointerRelease`, not `ItemInput`.

### Image identities from run 2a

All images are decoded 1280×720 PNG files. They are diagnostic history, not acceptance evidence. The captures include the third-party performance overlay noted under Environment. Paths are relative to `.local/runs/game-tests/335e06c4-0187-48d8-8fd5-22b53a472f2a/`.

| Image | Bytes | SHA-256 |
| --- | ---: | --- |
| `0cef6c79-e7d9-4538-a81c-88232e65421d/solo/draw-inventory-equipped-4.png` | 78891 | `7844779a9c8d34a972502d87ffbce8ca57e57206229d4a4d2899c263f3b16d38` |
| `0cef6c79-e7d9-4538-a81c-88232e65421d/solo/draw-inventory-nested-5.png` | 94700 | `97acd1534e94ebeb153d4441bcc65cb1594a5b4ab06430896bf30c2e62dfdb05` |
| `0cef6c79-e7d9-4538-a81c-88232e65421d/solo/draw-inventory-split-6.png` | 107898 | `bb3c6949bf72e7de2928b1970e7832f797cf8072d34224a00c08616f067acd17` |
| `0cef6c79-e7d9-4538-a81c-88232e65421d/solo/draw-movement-air-3.png` | 52614 | `41bcc34506eb738a013ab2c0a97abff1013b90f9adf7bcfe0d155d815d92d271` |
| `0cef6c79-e7d9-4538-a81c-88232e65421d/solo/draw-movement-sprint-2.png` | 51964 | `265ef8de7daab48d6ffc240e5ab0be04346a5baec0c859ba101ac901e45c7df9` |
| `0cef6c79-e7d9-4538-a81c-88232e65421d/solo/draw-world-walked-1.png` | 51235 | `704980c3649c99595ef88c50319e9860a7ba6a0155243c941e0699c93b36e4ba` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-1/draw-conflict-current-guest-1-2.png` | 118357 | `9bf2e9dab4ac24044c608cf0b6932ad3bc4c666c6d557ec266c5fd46b7b6b136` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-1/draw-conflict-original-guest-1-1.png` | 116108 | `5ceec130acc6736aaf3d647bfad68bc779b17429c33b374c91c6fc7fd1dfcac3` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-1/draw-succession-current-guest-1-3.png` | 63607 | `4afc7c447e2713e3fbc49343132c6265965ecfab8f94a40c1c5867ba1894d9b3` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-2/draw-conflict-current-guest-2-3.png` | 62306 | `68fb581418bf18d7cd3da109b1492b9fa425c9051be506580f53033b3f3c9db3` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-2/draw-conflict-original-guest-2-1.png` | 115505 | `d229560a4d11b6c209abe540e79d54c62475473cf2bad0d6c10fb025a8f296f2` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-2/draw-conflict-revoked-2.png` | 62554 | `5d0184d1cae1ab75b6b7d3590c07a2f3fa703cba44703e91572f356190086050` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-2/draw-succession-current-guest-2-4.png` | 63645 | `94864d81a0cd09c7b4ee10946628cabc794a6a4ba7577b8252d9a69790a2c888` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-3/draw-conflict-current-guest-3-2.png` | 116035 | `72651bc7e79f2897bb04692fe8abce4657a1cdf0837e603fbe7289444a4ebeff` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-3/draw-conflict-original-guest-3-1.png` | 115348 | `8d3049b81700771e7137c69e737d249e0e4eaf7df909df275ca88f67453cf385` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/guest-3/draw-succession-current-guest-3-3.png` | 63823 | `f72a139ac28d144ddc2bfe61a77bf75573950eccf5587a44e459f6c46ab1bea3` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/host/draw-conflict-current-host-2.png` | 71249 | `49dd0d28e1f4a4ffad9036b185221e291902d94c766c298628b0b96d66e161c4` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/host/draw-conflict-original-host-1.png` | 70393 | `2c9befbb9b1144c86b9178fd25931f22c9de57006b61b1381d79dd34615c7fdb` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/host/draw-succession-chosen-6.png` | 54959 | `67597e8670bd59bd70866e16860a37da3286a27595f70b91b97d408a3d8342d8` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/host/draw-succession-current-host-9.png` | 76688 | `adb96cb2654972536fa9ac568bfc9a543bd0c47accb8849581ad6b4d91574275` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/host/draw-succession-death-3.png` | 57305 | `587c5826720bff9bb81c1a2f014abe636a353cfb86424940ca8f67dd564f86bf` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/host/draw-succession-lost-7.png` | 52579 | `8c13e0033e37404ce9b1be634865cfd2eac8fe52d803e434a971f411b79858f0` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/host/draw-succession-next-4.png` | 55903 | `8b943d17fc07fb8dc26ef3bb3405b629426a4ba826ccff7da62dd8a7e1d16cc0` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/host/draw-succession-previous-5.png` | 56105 | `3995698e3e839ac7e8a1d8ac98a44c99a172a0891878204a40d14512c33a1252` |
| `128e5f25-aefc-4d1b-bd23-32b781185954/host/draw-succession-transfer-8.png` | 76726 | `34570e773d5920a5136924ff65ac5110415581cdd13d01692b6890a9edac4cdb` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.empty-character.png` | 92654 | `0686a3238a1ca5eec3b433a6c2356d0e38c882a77ad2387b1421574c44efd0be` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.empty-recovery-reset-1.png` | 129529 | `d7edf192c76c622d9ad3622dd35bec85a0966f7f9f96fa2b9372d4bc41c38834` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.empty-unavailable-character.png` | 65074 | `f4bd12d3fcdcc51dc5b0be569490497bb4c9fd7a158e5fb45c646c3da34f1d2c` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.error-failed-character.png` | 77011 | `96979864638046743eb10f6fc9652f943d3963eb2aaff0dc5ed71c0a60c44340` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.error-recovery-reset-1.png` | 129440 | `c9545cccd21fe8ad5d843f379b80402f94745671b4c27a0210a64369382868c6` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.error-restored-character.png` | 91291 | `949e8843ce0bfd8c9347c3ba5a98ae4384e1ec4fe882e1e45a2efefafb6a0570` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.loading-character.png` | 92783 | `3044a9458ee6288abe1ae8a65546733f464ec6562b2fcd78eaeb089ce88db105` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.loading-current-character-loading.png` | 72382 | `4577f1587e2ad0f3d0b364fbe8a16bdcf00d79d07632fafe8ad70fc34367eac5` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.loading-recovery-reset-1.png` | 129620 | `152f5bd2f63a3445e6d06312f44412f8ebc81efb0ee6f196271c6d2d5424dc41` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.long-text-character.png` | 91178 | `1aff5da3667b183f00394ed45c8d579c391e3225ac0894475c94d2b057f652ee` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.long-text-long-character-provenance.png` | 109571 | `0556f82caeed40881be801cf60dd85b2042027c056da0947fb598890d6fe49f9` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.long-text-recovery-reset-1.png` | 129759 | `b950eed7f8dad6e15b2d0a1accf417e7d0c3503e558ab971376ded9573babc5b` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.overflow-character.png` | 91375 | `a41059f3a7a5c7275b610b13dfd56f7b6c90a385948a7dde3a5a7a0084d088c9` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.overflow-reach-overflowed-transport.png` | 87956 | `023ce9d31e493ae95b3df1817f20b533e5b0e4518dabf0af2243d062c8bde809` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.overflow-recovery-reset-1.png` | 129483 | `4cb8ef4760ad4180719bf13d6b63ffc24d48ac66a762b1fb8510301e89fd9723` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.partial-failed-character.png` | 76907 | `88024a1d90a85d2b1f4a0110dc29090815fe844d4610150cd18ac905ae850f5f` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.partial-recovery-reset-1.png` | 129305 | `a870976bfc9642c40f9a48c39fd5e302108394b4468544db2c2528e66c3f2b62` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.CHARACTER-PREVIEW.partial-restored-character.png` | 91096 | `7730a79ddaabcddaac0a67825825329208b5119d81a1eefc3aefeee0f3aa7e07` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.UI-COLLECTIONS.empty-recovery-reset-2.png` | 129536 | `84a739a0371d9c0721c991628c67fd0120ce2ffd2dd864e3794a5edd59a8cf16` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.UI-COLLECTIONS.error-recovery-reset-2.png` | 129550 | `8eb6c204161d02822c0d8b0ffa93583d983aa4b28a47898bee214212dde10437` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.UI-COLLECTIONS.loading-recovery-reset-2.png` | 129405 | `0c0d385b0b3b97cc10ee26b8abdbee2461c41548af65aea0e941ff013f5ea8a0` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.UI-COLLECTIONS.overflow-recovery-reset-2.png` | 129442 | `a46fcc8d1188c4d942df8a46866a23e325124dbe5d2f32826057cf9e1e710fcd` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.UI-COLLECTIONS.partial-recovery-reset-2.png` | 129541 | `20ef5587625ce1f027c19e6f42f2deabdae53039859dbe7a23074aac87c0a191` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.UI-COLLECTIONS.populated-recovery-reset-2.png` | 129528 | `5229503e523a1fcbaa7009a990dbf793e4159f3b9d7dd5f0fb94c1775083335e` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.UIH.UI-COLLECTIONS.zero-one-many-recovery-reset-2.png` | 129402 | `327c127c2bbcddaa72081b3f6a5756433f791e1469c91a21053204e693e8ecae` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H14-actual-empty-ammo.png` | 95131 | `c4ab017e577e7d49922fa0471dadaa6ee39db673129b19449b8731ee2f41ef85` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H14-bow-preview.png` | 98927 | `6004647c8f09e35303d677ff4e720d27d55c3dae5489f8692ce49af97ddbbf81` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H14-recovery-reset-2.png` | 129516 | `e8afff2ea2948548c235c712ba9c60de5a6187fd4fef5ff3e525d934847352e1` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H16-authored-walk.png` | 100902 | `6a55819cd952afd2b1c15c7e92f74d272d40309462570515dee37a62da875429` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H16-exact-second-frame.png` | 96954 | `5111d4639141a4f27b273af5c25a1b769cbf364168585242e0f30e3cd686d056` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H16-held-walk-endpoint.png` | 97192 | `220e1489e722d716bcd76dcae2dc516187d472ab0b6ed8640bcdde76d6a552e0` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H16-recovery-reset-1.png` | 129574 | `935d9fbab6aa5c344001ac975e1403284b974994fb51c5306cf6663835c9a063` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H16-start-jump.png` | 95681 | `ff8093b74f856995df485cefc17a31a02b557c70ab0fa1a6fd70ac9404848390` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H16-start-walk.png` | 96381 | `9422cf6312fbd48946c6944a5e4212750f7d1e915aaafe1bd604a0c2a5ee2402` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H16-step-held-jump.png` | 97471 | `5eed5d85506122b3ae17e1269732281834cbeac8d59e0d96d8cf46f3e229af7e` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H16-step-jump-landing.png` | 97525 | `ad22deccd0bdf5b9c791574c0c79c80e65c70717518e3919e907f7a1d2aa7638` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H16-step-nonuniform-jump.png` | 97428 | `591baeddbdc1d36a6fcb420549eb18de187a7053a9b286eccb4fb5b316b362d2` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H17-active-melee.png` | 97317 | `d98189a787bf060d151aecfd6f22b4298ba04a024796f655e916dcaebbbe7f7a` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H17-advance-transport.png` | 98554 | `a68eb6b6c10bc4847cccef4a7c754efeea407bc58dbe7a239d0e894e58196edd` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H17-character-preview.png` | 98037 | `4d4bb9a526d94fc94135a93009ba2dee9f8947da5b002ec0530bc9fecc6cf3fc` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H17-persistent-corpse.png` | 86209 | `53b0e39d4c9fd8b0abebb4ea1dd04a6370ecd3d7792cb1b8c773514e99f3f0fe` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H17-recovery-reset-2.png` | 129577 | `652732ec67cf2184fd1e9fc97b36948206411621062510fa60fa649c50471c2d` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H17-reset-transport.png` | 96695 | `294a4ba0ffa61b91b7149b321e7772dd89866b497a063ac225522243e8aa386b` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H17-walking.png` | 96014 | `6ca6f3c05068213a46f16d8cff585c008c780183c41308c12307f95c918311fc` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H18-recovery-reset-2.png` | 129427 | `b99af6dd14df2681f0e8c1a7c702832fc8914b72e2e9586baf83824fe17613f4` |
| `d4c4cd48-2b4e-4297-8c4d-f4611ef3f43a/inspection/asset-pipeline.heldout.H18-walking-melee-body.png` | 98333 | `11eab5448e9558637ddb4c06cceb930456e62f7d50a86d808f3049c716ea152f` |

The 42 inherited lan-checks captures are under `ac2ede97-e39d-4474-a815-8b77df7da101/G2/` and `G3/`, and each case's `summary.json` lists them with their hashes. The P1–P4 hub captures of `13ecb196` are in asset-pipeline run `5be894bf-264b-41c2-a2b3-9a03f693537f/hub/`. Among them, `rendered.hub-smoke-source-details.png` is 145670 bytes with SHA-256 `452be5e6d6cbc2f3c18851a95c50e7bcd6bc8d0c112414777d2170654396f8b6`, and `rendered.hub-smoke-death-step.png` is 79630 bytes with SHA-256 `4258aa2b0846e275a4cdc87c1182d40d4f6384d0501ae3ee0993cbf69a188c99`. They stay pending review.

### Run 2a outcome

Run 2a did not pass: 28 failed and 2 blocked in `335e06c4`, and the newly affected case passed in `13ecb196`. No repair is made in this record. The next repair round also owns the orchestrator-accepted RR-06 to RR-08 and the two known older failures recorded in 06-REVIEW.md ("Orchestrator acceptance (2026-11-28)"). Each repair round goes to a fresh executor and a fresh independent reviewer, and is appended here as Run 2b and so on. The fresh full selected-set run stays with 06-15.

### Repair round 3 scope decisions

These decisions were taken by the orchestrator for repair round 3 and are recorded here unchanged. They are not reopened by later repair rounds.

1. **`asset-pipeline.runner.prelaunch-source` stays expected-blocked.** Round 3a (`58d8216`) cleared its `SELECTION_IMPACT_MISMATCH`. The next control, `unrelated-passing-case`, stops at `EVIDENCE_GIT` merge-base, because `verifyFixProof` (`asset-pipeline/scenarios.cjs:274`) needs the CR-01 fix commit `ef9e58c`. That commit exists only on `feature/04-asset-production`; PR #5 was squash-merged as `1a14684`. Under Phase 5 CONTEXT D-46 the `runner.prelaunch` gate stays expected-blocked, without pass credit, until Phase 11 (TEST-12). It is not repaired in Phase 6.
2. **Eight pinned Phase 4 recipe cases are deferred.** They fail on pre-move paths, never ran earlier in this phase, and were selected only because the asset-bindings rederivation touched `pack-index.json` and `delivery.json`:
   - `asset-pipeline.audio.feasibility` and `asset-pipeline.media.conversion`: `CAPTURED_ROOT_REQUIRED`, documented in `src/godot-game/tools/README.md`;
   - `asset-pipeline.media.scale`: missing `tools/phase4/extract-media.py`;
   - `asset-pipeline.bodies.b-wide` and `asset-pipeline.bodies.complete-fit`: missing `production/phase4/bodies-b.zip`;
   - `asset-pipeline.buildings.defense-state`: missing `production/phase4/main-buildings.zip`;
   - `asset-pipeline.export.profile`: missing local checkpoint `04-02-entry-93386745`;
   - `asset-pipeline.export.layout`: `EXPORT_SOURCE_CASE` on `src/core/obj/…AssemblyAttributes.cs`.

   These failures predate Phase 6 and are not caused by it. They are deferred to the D-43/D-46 relocated-tree provenance work and must be named as open items in 06-25-SUMMARY and in 06-15.
3. **`asset-pipeline.export.profile-boundaries` is repaired in round 3b.** `export-profile.cjs:197` named `relative`, which 11a7ca3 had removed, so every native observation threw `ReferenceError: relative is not defined`. Round 3b (`a8588ef`) names the pinned console executable instead, as 11a7ca3 already did for the engine hash check. The case then passed on its own: game-tests `2a5841cd-9890-467c-ae5f-3a7ed99c442c` (asset-pipeline `a4dabad9-d75d-4df8-b61b-7aa5cf1df57d`), 1 passed of 1, on `a8588ef`.
