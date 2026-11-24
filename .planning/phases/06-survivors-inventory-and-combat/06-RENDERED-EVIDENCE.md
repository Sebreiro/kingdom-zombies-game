# Phase 6 rendered and phase-end evidence

This record holds the Phase 6 phase-end runs at 1280×720 on the development Windows desktop. It keeps the per-case outcomes, image identities, qualitative findings and failure history. The failure history is never rewritten. Later repair and final runs are appended below it.

## Current status

**Not accepted.** The first fresh phase-end run failed. It recorded 66 passed, 39 failed and 18 blocked of 123 selected cases, and both Critical smoke cases failed. The repairs change reviewed source. They need a fresh independent review, a focused rerun of the failed and blocked cases, then a fresh full run of the selected set. None of these has happened yet. No case below is credited as a Phase 6 pass until the final run on the repaired, reviewed source passes.

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
