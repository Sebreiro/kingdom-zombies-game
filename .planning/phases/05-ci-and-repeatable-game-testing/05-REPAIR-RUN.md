# Phase 5 focused repair run

The first focused repair selected the 37 failed and blocked rows from the [initial Windows run](05-INITIAL-RUN.md). It tested clean commit `4b50c81d02424b1322b06585253e5fff5476d930` on Windows x64.

- Command. `node src/godot-game/tools/game-tests/check.cjs --failed .local/runs/game-tests/78becb29-763d-4cf8-93cc-a9abff498b79/summary.json --review .local/temp/phase05-repair-review.json`
- Shared invocation. `b7ed2a51-d6c3-4a4d-aa7f-92c786e9f20d`. Exit 1 with 34 passes and 3 failed catalog rows. Original reports stay under `.local/runs/`.
- Rendered solo passed 9/9 at 1280×720 in one session. The suite invocation `b0f2464f-d9f3-4e6a-924b-dc5699ec1ef6` took 225.766s. Rendered LAN passed 12/12 under `c4dc4220-385f-47ae-83ab-3932f0b16ec7`. All seven selected LAN headless cases passed under `1ca76a2c-d52c-4291-bd9e-d1ce4f4f1c54`.
- The isolated pin proofs passed 2/2 under `946e1947-3a08-4a50-87ea-c3b1cdf14683`. The targeted rules group passed 17/22. Repository authentication exceeded three inherited 45-second stage limits. Both companion checks reported no recognised current producer. The foundation permission case reached the live-producer stage and failed on the same source identity.
- The fixed audio lifecycle, held-out audio and world-map source cases passed. The hub launched separately and stopped before capturing its first category when the input acknowledgement failed. On this desktop the 300% DPI transform made the legacy driver report 426×240 for the physical 1280×720 window. The hub still needs its category captures and P1–P4 review.
- Basic Windows and Ubuntu checks passed on this same revision. [Hosted run](https://github.com/Sebreiro/kingdom-zombies-game/actions/runs/45290688042)

The next focused attempt uses per-monitor DPI awareness only for the inspection hub's existing owned input driver. Reproduce with the exact hub case and a clean review of the new source. The updated selection distinguishes an exact change to the live FoundationRoot asset stamp from a general content helper change. It retains the actual normalization, publication, and live fixture consumers.

## Further Windows repairs

The follow-up focused selection `ec271f44-59ed-4153-88ff-4d919f5f908f` on `c898bf6` passed three of seven. The exact pack reported 162 of 180 ready, the canonical text field guide expected the current UI digest, and the two identity cases still used the retired ZIP path. The repaired pack supplied all 180 resources and preserved zero acceptance credits pending the separate reviews. The retained layout sources now resolve through the repository path mapper.

The next checks used `e65b1cfac14547317e58d5f8314ae8d4e4021592`.

- `--case asset-pipeline.rendered.hub-navigation --review .local/temp/phase05-e65b1cf-review.json` produced shared run `9c228951-14fe-40e3-b8f1-b69971f624ad` and hub `3254f1f2-1537-4fc9-8018-6591f8b73028`. Exit 1 in 119.231s. All eight category captures and the source-details capture were present. The zero-height resource list blocked physical selection of FX-08 and preview contents overlapped the lower controls.
- Shared run `9b4f3dbb-0899-411f-ba83-39f9ca74e01c` ran the exact catalog cases `gameplay-checks.core.permissions,asset-pipeline.layouts.kit,asset-pipeline.layouts.identity,asset-pipeline.layouts.identity-integrity,asset-pipeline.layouts.application-icon,asset-pipeline.content.census,asset-pipeline.text.catalog,asset-pipeline.text.compatibility,asset-pipeline.text.support-native`. Exit 1 after 395.773s. The four layouts, census and text catalog passed. Both native text probes hit the same retired managed binary copy path. The permission caller rejected the stale span registry binding before the child stages, independently of the current producer replay that passed in its isolated proof.
- Pinned Windows and Ubuntu basic jobs both passed for this revision. [Hosted run](https://github.com/Sebreiro/kingdom-zombies-game/actions/runs/45298204873)

The next reviewed wide layout, `57af06f1802e5abb358a130f47e0b312ec569a31`, gave the resource list 299 pixels of height at 1280×720 and clipped preview drawing to its pane. Repeating the exact hub case with `--review .local/temp/phase05-57af06f-review.json` produced shared run `78b1cbd6-6ecd-4060-a7d7-9a58c50d9708` and hub `7cff4393-9bfc-413d-82b7-07fe6ceb172b`. All 15 objective checkpoints produced images, including the exact FX-08 death selection, 0.125s live state and retirement at 0.25s. The terminal validator then rejected the observed executable because it resolved a shared tool identifier as a checkout-relative path. The saved reference resolved to the identical process executable through the existing shared resolver. The step and retired pixels showed an empty preview pane, so P4 remains pending. The independent source replay confirmed all 32 generated file byte and text hashes still match their recorded producer and recorded the changed authored input under the current replay.

## Superseded full reduced Windows run

Invocation `21a61405-8549-4a4a-8930-1be4284c0e2e` tested clean `74a424cc070f642854afe8b1f166672692e3a3d5` on Windows 10 Enterprise LTSC 2021 x64 at 1280×720. The command was `node src/godot-game/tools/game-tests/check.cjs --phase-end --review .local/temp/phase05-74a424c-review.json`. It exited 1 in 11,758.378s with 87 passes, five failures and zero blocked rows.

The run passed all nine solo cases, 20 headless LAN cases, 34 asset source and headless cases and the 15 objective hub checkpoints. Native asset proof `33bdc476-6d37-427c-9b0e-6b2b29059ef5` passed 251 of 251 with zero skips in 3,533.525s. The five inherited verification groups measured 451/451 rules, 22/22 engine, 35/36 runner, 27/27 toolchain pins and 46/46 register. The original rendered hub invocation `330443df-ffae-4a78-94bb-6656338e960a` retained all four pending review rows.

The four G2 rendered LAN cases failed from the same cleanup. In nested run `ed9ad16c-0431-4536-9d5f-c4131440796a`, restarted browser PID 20416 had a ready menu with Quit at client point (640,520). The Windows driver refused sequence 7 before button injection because the point failed its ownership check. The process had no ordinary Quit acknowledgment and required cleanup termination. Eight G3/G4 cases passed. The cause of the desktop point’s ownership mismatch was unavailable in the original receipt.

The isolated repair `ef490ac7ec78e69d671d1fa2dcc4331b56ee160e` tested the same four exact rendered catalog IDs with a clean reviewed source. The result `4ba42168-2a08-45c9-a9b6-b74304122114` passed 4/4 in 93.229s. The browser restart used physical Tab input through fresh Godot focus draws from Start to Host, Join and Quit, followed by a paired physical Space press and release. Its PID 21024 produced the native ordinary Quit acknowledgment and exited 0 with closed streams. The original browser still uses a real mouse before the restart. [Hosted checks on this repair](https://github.com/Sebreiro/kingdom-zombies-game/actions/runs/45312684816)

The inherited `runner.selection` test still expected four timeout overrides. The three measured 90-second limits for `edge.rule-03.adjacency`, `rules.generated-callsite-origin` and `rules.suppression-duplicate` were correctly effective in the 451-case rules group. Current control tests enumerate all seven allowed declarations and inspect both the 90-second child argument and 92-second launcher allowance. Exact `b7e419c075bd0c97423142684099c9f5d588f0c9` passed the focused `node src/godot-game/tools/verification/check.cjs --cases runner.selection,runner.process.repeat` invocation `41c12bc2-0623-4904-80f2-8185476d63b1` with exit 0 and 2/2. Native deadline and selector checks still reject malformed or oversized values and require real subprocess cleanup.

## Focused Windows inspection review

Codex inspected the eight category images, the open source drawer and both FX-08 timing frames at 1280×720. The primary category, selected resource ID, pending status and browser list were readable. Preview controls and content stayed inside their pane while details and diagnostics remained separate. Characters, Items, Buildings/world, UI, Data/text, Audio, Register and Known issues reported 31, 24, 43, 27, 29, 30, 180 and 180 results respectively. Each capture reported 180/180 source-ready and 0/180 accepted.

| Judgment | Result | Observations |
| --- | --- | --- |
| P1 / AC-22 | Pass | `source-details` shows the AN-01 identity, source file ID and declared dependencies. `category-6` and `category-7` keep the pending status and cite the exact obligation manifest. The source drawer’s paired observation includes explicit shared mappings. |
| P2 / AC-23 | Pass | `category-0` and `category-3` show the KingdomZombies title and wood-and-dark inspection theme. The game name is legible. The title’s separator displays as a stray glyph in the display font. |
| P3 / AC-24 | Pass | `category-1`, `category-2`, `category-3` and `category-4` present authored inspection samples, an explicit preview step and read-only local/host data. The inspected screens show no recoverable fired arrows, lootable crowns, structural-guard loot, damaging bloater acid or gameplay-pausing settings. |
| P4 / AC-21 | Pass | `death-step` shows a tiny muted cluster near the middle of the FX-08 pane at 0.125s, with no detached anatomy. The same location is empty in `death-retired` at 0.25s. The paired live state records one owned effect node and then zero. |

Long labels in the resource list use ellipses. The selected resource name appears in the summary. The captured desktop’s hardware overlay appears above the game at the top right. The inspection review covers these exact Windows captures.

Tested revision `74a424cc070f642854afe8b1f166672692e3a3d5`. Shared invocation `27715d10-b8cf-4196-9f0e-ebef1750b873`, hub `6c5dd16d-fbc7-4a90-9c27-a2a6f8c3563b`. The invocation published 1/1 objective result and exited 1 with `needs_review` before the separate image review.

Reproduce with `node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.rendered.hub-navigation --review .local/temp/phase05-74a424c-review.json`. Original screenshots were stored under `.local/runs/asset-pipeline/6c5dd16d-fbc7-4a90-9c27-a2a6f8c3563b/hub/`.

| Checkpoint | PNG SHA-256 |
| --- | --- |
| category-0 | `3fbabcc9a30570fbd59a342630cb574e20efc4dc43ae0055f68f4fd2a78fa5b1` |
| category-1 | `2c81b439f7f8deb55526ac4d62aacff227cb6d5aaa3c3425ce694a9ed5090cbf` |
| category-2 | `ba024e534650ecf668b09b220eb0ca27e3477c5dd65004b2c0d5d43118df9932` |
| category-3 | `94a0fd4d542b3d00fc3ecc29aa5fc7b59353cc0d65ebc783c3ecbec7449094b8` |
| category-4 | `22702be9509e9301b83730818719d203319aa0b1007ce779868d8e8b21332658` |
| category-5 | `d2bd6bf22812fca1e8f7c0cbb72a996521a2375594f9b4eb3ca06ff148f1108f` |
| category-6 | `2a82371b02c2a641af97139cb2c9495447fe860c82f5737a9bd92c6c07134ede` |
| category-7 | `423967a638fcc9b4f529f34553fdde67343bd2d3070ac150dae213cdc062a29d` |
| source-details | `1de007ad400f5f59540f264f03f22bb6db1378422c5e3861d4024d2fd2cdb2ee` |
| close-source-details | `962e304efa77f3530d4ea2c732f5e5152d782c9453367d8d5b8e72436e51bb8e` |
| return-to-buildings-world | `24d7558165e24ae9a1a639abc3d752103b9d6c15746dc08b07b688fcc6449c32` |
| death-gallery | `35daa342bed60d3594b06dff24d3cbc0b2e44d297f3a9bf8c8c2a90e133abc26` |
| death-effect | `26ee55fad3593d9b56e5cce84693fbf2e1acaf2009522b01e9d6b8da7f5db6ca` |
| death-step | `eea7cd66ae78c0cda4e8d2682ea46eb86abe553556137271c910bb700bc83ae3` |
| death-retired | `f158c3bb48f08632eb48adc21d0d1d372edc92f4ae94c9c12b8cc61766a87905` |
