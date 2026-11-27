# Phase 6 rendered and phase-end evidence

This record holds the Phase 6 phase-end runs at 1280×720 on the development Windows desktop. It keeps the per-case outcomes, image identities, qualitative findings and failure history. The failure history is never rewritten. Later repair and final runs are appended below it.

## Current status

**Not accepted.** The first fresh phase-end run failed. It recorded 66 passed, 39 failed and 18 blocked of 123 selected cases, and both Critical smoke cases failed. The repairs change reviewed source. They need a fresh independent review, a focused rerun of the failed and blocked cases, then a fresh full run of the selected set. None of these has happened yet. No case below is credited as a Phase 6 pass until the final run on the repaired, reviewed source passes.

The focused rerun (Run 2, below) did not pass: 28 passed, 13 failed and 16 blocked of the 57 run-1 cases. The one newly affected case passed, with P1–P4 pending.

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
