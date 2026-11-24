---
phase: 05-ci-and-repeatable-game-testing
verified: 2026-11-24T08:26:56.175Z
status: passed
score: 92/92 selected checks and P1–P4 verified
revision: b7e419c075bd0c97423142684099c9f5d588f0c9
---

# Phase 5 verification

The fresh reduced selection passed **92 of 92 objective checks** on Windows 10 Enterprise LTSC 2021 x64 at 1280×720. Codex accepted all four named image judgments on that same run. The actual Windows and Ubuntu basic pull-request jobs passed on the same source.

## Final reduced Windows run

- Tested clean `b7e419c075bd0c97423142684099c9f5d588f0c9` after independent code review. The comparison base was `11a7ca31614f6929a1a27285511b696a07b78c39`. Tools were Node 24.22.1, .NET 10.0.404 and Godot 4.7.2 Mono.
- Invocation `62a015a9-6df3-4a66-9a16-039aa7430de0` started `2026-11-24T05:09:14.431Z` and finished `2026-11-24T08:25:02.720Z`. Wall time was 11748.289s. It published no failed or blocked cases and recorded 40 unaffected and 163 planned omissions.
- Reproduce with `node src/godot-game/tools/game-tests/check.cjs --phase-end --review .local/temp/phase05-b7e419c-review.json`. Prepare the exact reviewed revision, reviewer and clean verdict using the shared test procedure. The selection and summary were stored in `.local/runs/game-tests/62a015a9-6df3-4a66-9a16-039aa7430de0/`. [Shared test procedure](../../../docs/workflow/shared-game-tests.md)
- The immutable runner exited 1 with `needs_review` after completing all 92 objective checks. Its four outstanding rows were the image judgments. The named review below completes those rows for this same invocation.

| Group | Selected and passed | Nested proof results | Reported launches | Wall time |
| --- | ---: | ---: | ---: | ---: |
| basic | 4 | — | 1 | 36.825s |
| gameplay-checks.headless | 7 | — | 0 | 957.038s |
| gameplay-checks.rendered | 9 | — | 1 | 225.118s |
| lan-checks.headless | 20 | — | 17 | 309.966s |
| lan-checks.rendered | 12 | — | 14 | 145.172s |
| asset-pipeline.headless | 34 | — | 5 | 5105.635s |
| asset-pipeline.rendered | 1 | — | 1 | 121.156s |
| verification.rules | 1 | 451 | 0 | 3728.889s |
| verification.engine | 1 | 22 | 0 | 871.711s |
| verification.runner | 1 | 36 | 0 | 68.822s |
| verification.pins | 1 | 27 | 0 | 98.091s |
| verification.register | 1 | 46 | 0 | 79.729s |

Nested legacy logs carry their own helper process records. The shared launch column records the counts returned to the shared runner.

- Tier 1 used one pure test execution with 18/18. It took 1.386s after 15.741s strict preparation. Repository checks took 18.559s. The routine pure execution target is five seconds after preparation.
- Tier 2 headless solo passed in 1.050s. The separate 20-case LAN lane took 309.966s and included the named independent headless process groups. Native asset permissions `82fb7a03-3086-4d2a-a11e-8e3881f49350` passed 251/251 in 3,527.910s with zero skips.
- Tier 3 ran all nine solo, twelve LAN and one hub cases in 491.446s. Solo used one process. LAN retained 14 recorded launches across its browser, four-player and lifecycle groups, with reusable peers in compatible cases. The hub launched separately.
- Standalone Critical smoke `5562ace7-2ebe-494b-9f15-53a83274887c` passed both declared cases on the earlier reviewed source in 32.240s wall. The actual rendered time was 16.040s. The fixed Critical budget is two minutes of rendered execution. [Measured testing budgets](../../../docs/workflow/local-testing.md)

<details>
<summary>Exact selected catalog IDs by group</summary>

- basic · `basic.strict-build`, `basic.repository`, `basic.pure`, `basic.headless`
- gameplay-checks.headless · `gameplay-checks.core.authority`, `gameplay-checks.core.input`, `gameplay-checks.core.lifecycle`, `gameplay-checks.core.permissions`, `gameplay-checks.runner.integrity`, `gameplay-checks.runner.processes`, `gameplay-checks.runner.repeatability`
- gameplay-checks.rendered · `gameplay-checks.rendered.tracer`, `gameplay-checks.rendered.menu`, `gameplay-checks.rendered.directions`, `gameplay-checks.rendered.distance`, `gameplay-checks.rendered.world`, `gameplay-checks.rendered.lifecycle`, `gameplay-checks.rendered.focus`, `gameplay-checks.rendered.quit`, `gameplay-checks.rendered.resources`
- lan-checks.headless · `lan-checks.core.authority`, `lan-checks.network.tracer`, `lan-checks.core.permissions`, `lan-checks.core.admission`, `lan-checks.core.ordering`, `lan-checks.core.protocol`, `lan-checks.network.four-peer`, `lan-checks.core.lifecycle`, `lan-checks.network.lifecycle`, `lan-checks.network.idle-poll`, `lan-checks.adapters.discovery`, `lan-checks.adapters.profiles`, `lan-checks.network.discovery-profile`, `lan-checks.core.input`, `lan-checks.adapters.substitutions`, `lan-checks.runner.tracer`, `lan-checks.runner.timing`, `lan-checks.runner.integrity`, `lan-checks.runner.processes`, `lan-checks.runner.repeatability`
- lan-checks.rendered · `lan-checks.rendered.entry`, `lan-checks.rendered.browser`, `lan-checks.rendered.join-errors`, `lan-checks.rendered.profiles`, `lan-checks.rendered.moving-join`, `lan-checks.rendered.walkers`, `lan-checks.rendered.timing`, `lan-checks.rendered.focus`, `lan-checks.rendered.admission`, `lan-checks.rendered.guest-lifecycle`, `lan-checks.rendered.host-end`, `lan-checks.rendered.host-loss`
- asset-pipeline.headless · `asset-pipeline.source.tracer`, `asset-pipeline.source.tracer-failures`, `asset-pipeline.source.publication`, `asset-pipeline.layouts.kit`, `asset-pipeline.layouts.theme`, `asset-pipeline.layouts.identity`, `asset-pipeline.layouts.identity-integrity`, `asset-pipeline.layouts.inventory`, `asset-pipeline.layouts.legacy-target`, `asset-pipeline.layouts.camp`, `asset-pipeline.layouts.map-warnings`, `asset-pipeline.appearance.corpses`, `asset-pipeline.appearance.crowds`, `asset-pipeline.audio.actions`, `asset-pipeline.audio.bank`, `asset-pipeline.layouts.application-icon`, `asset-pipeline.audio.lifecycle`, `asset-pipeline.audio.heldout`, `asset-pipeline.content.source-relations`, `asset-pipeline.content.domain-relations`, `asset-pipeline.content.census`, `asset-pipeline.content.native-permissions`, `asset-pipeline.hub.source`, `asset-pipeline.source.packaged-expectations`, `asset-pipeline.text.catalog`, `asset-pipeline.text.compatibility`, `asset-pipeline.text.support-catalog`, `asset-pipeline.text.support-native`, `asset-pipeline.world.map`, `asset-pipeline.world.effects`, `asset-pipeline.world.inspector`, `asset-pipeline.world.replacement`, `asset-pipeline.export.hub-consumer`, `asset-pipeline.export.portability`
- asset-pipeline.rendered · `asset-pipeline.rendered.hub-navigation`
- verification.rules · `verification.rules`
- verification.engine · `verification.engine`
- verification.runner · `verification.runner`
- verification.pins · `verification.pins`
- verification.register · `verification.register`

</details>

## Acceptance coverage

| Phase 5 criteria | Verified evidence |
| --- | --- |
| AC-01–04, AC-30–32, AC-35–36, AC-38–39 and AC-42–45 | The 295-case shared catalog covers A-01 through A-47, validates exact ownership and grouping, and records planned and omitted rows. The clean and defective selection controls cover Critical, changed Low, affected consumers, planned cases and both rule-impact directions. The edge control accepts a matching unit test and rejects missing or duplicate IDs. Windows and Ubuntu exercised the strict basic selection and real headless movement. The eight certification IDs resolve to Phase 11 under the legacy exhaustive selector. |
| AC-08–10, AC-17–18 and AC-20 | Eighteen pure tests exercise production state and bounded time/random continuation. Immutable run and repair reports reconcile actual results. Seven deliberate defects exercise missing dependencies, zero discovery, a skipped result, an assertion failure, a timeout, a missing peer and missing evidence, with their clean comparisons. |
| AC-11–16 | Fresh 1280×720 solo, LAN and inspection runs used ordinary production input with owned processes and exact checks. Native peers preserve identities, timing, state and cleanup. The isolated Windows ExportRelease proof excluded test authority controls and retained its read-only observer. The catalog retains each proof’s exact evidence scope. |
| AC-06, AC-37 and AC-40–41 | Initial and focused failures remain attributable. The successor first passed all five failed rows in a focused selection and then completed the entire final reduced set on the clean reviewed build. The recorded local and hosted durations set the routine budgets. |

Phase 11 owns the transferred full hosted scope, coverage certification, evidence reuse, remaining runner certification and review-bound asset procedures under TEST-12. The catalog records physical mixed-computer LAN, reference-PC performance and endurance under their existing release owners.

## Basic pull-request results

Each hosted job ran the strict build with analyzers, repository and catalog checks, 18 pure tests in one process, and a native headless solo start, movement and natural quit. Each published four passing shared results plus its case reports. Pinned tools were Node 24.22.1, .NET 10.0.404 and Godot 4.7.2 Mono.

| Hosted revision and run | OS | Dependency cache | Job wall | Shared runner | Preparation | Pure execution | Headless |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: |
| `6f4cef61a70b657034b4f4ce05d0a968d7955d64` [cold run](https://github.com/Sebreiro/kingdom-zombies-game/actions/runs/45279546809) | Windows | Cold | 143s | 72.946s | 48.842s | 1.414s | 2.864s |
| same run | Ubuntu | Cold | 113s | 64.206s | 46.526s | 1.269s | 2.369s |
| `b7e419c075bd0c97423142684099c9f5d588f0c9` [cached run](https://github.com/Sebreiro/kingdom-zombies-game/actions/runs/45331138330) | Windows | Restored | 169s | 88.097s | 56.037s | 1.924s | 3.093s |
| same run | Ubuntu | Restored | 132s | 91.591s | 60.627s | 1.793s | 5.618s |

The cached Windows shared invocation is `7a0c9413-f432-4a07-a019-baaa2db09462`. The Ubuntu invocation is `88685b65-368f-434c-ad97-faf1b70ffe0e`. The checked workflow and reports ran with the same exact revision as the fresh local selection. Both cached jobs restored their dependency cache and finished within the ten-minute target. Preparation includes toolchain checks, locked restore and strict build. Job wall also includes checkout, caching and artifact upload.

## Inspection image review

Codex reviewed all 15 fresh 1280×720 images in hub invocation `bf5b98c8-8406-4f5e-8340-327abaa28516`, a child of the final reduced run. The owned native process passed every objective checkpoint, returned exit 0, and released all owned processes. The before and after native state packets and physical input acknowledgments are paired with the original captures.

The category control, selected ID, list, count and partial status were legible in every category. The eight result counts were Characters 31, Items 24, Buildings/world 43, UI 27, Data/text 29, Audio 30, Register 180 and Known issues 180. They consistently showed 180/180 source-ready and 0/180 accepted. The preview stays within its pane and leaves the details buttons and diagnostics unobstructed.

| Judgment | Result | Observations |
| --- | --- | --- |
| P1 / AC-22 | Pass | `source-details` shows the actual AN-01 identity, file ID and declared dependencies in its drawer. The paired observation includes the explicit shared mappings. `category-6` and `category-7` retain the pending status and exact obligation source. |
| P2 / AC-23 | Pass | `category-0` and `category-3` retain the legible KingdomZombies name and the game’s wood-and-dark inspection theme. No outside logo appears. |
| P3 / AC-24 | Pass | `category-1`, `category-2`, `category-3` and `category-4` present authored inspection samples, explicit preview time controls and read-only data. The inspected screens show no recoverable fired arrows, lootable crowns, structural-guard loot, damaging bloater acid or gameplay-pausing settings. |
| P4 / AC-21 | Pass | `death-effect` shows the actual selected `effect:character.death` route. `death-step` shows the restrained cluster near client point (838,465) at 0.125s with no detached anatomy. The same point clears in `death-retired` at 0.25s. The observed effect node count changes from one to zero. |

The title’s separator appears as a stray glyph in the display font. Long browser-row labels use ellipses, with the selected resource name available in the summary. Some content requires its local scrollbar. The desktop hardware overlay in the top right is visible in these captures. Isolated geometry at 960×720 reproduced the existing compact-layout overflow. This review covers the declared 1280×720 case.

Original images lived under `.local/runs/asset-pipeline/bf5b98c8-8406-4f5e-8340-327abaa28516/hub/`. Each filename is `rendered.hub-smoke-<checkpoint>.png`. These are the exact reviewed images.

| Checkpoint | PNG SHA-256 |
| --- | --- |
| category-0 | `f161613c1ee27be911faf9ed4d9011ba633bcebec24d91aa89680d2d7c94d53f` |
| category-1 | `cf89ffb419ab306f9f77adfbbf040a667fb626a57a4d43ac6fa31568c04785d2` |
| category-2 | `62ba3ed8fa83ecade516cdf811da5314ade6072e65857c24d58d070a4c6968f9` |
| category-3 | `b637bf82aa814020eff7097f5e000d178e6739c370116bafd19930cd97530444` |
| category-4 | `ec6349a5128fc66f935a67cf3fa58b6f46640023331e5c5262f0119d7bc4bd91` |
| category-5 | `6fb05437a64be0e9d14d9ced2a000803d92a0c4d65807904f777433d755f1390` |
| category-6 | `c9e1b5f42240682cb08c322ecc77ffb16e7c9a265681772bd5f54b8d17c2a29c` |
| category-7 | `66045c6149d0cc818d5c9c09e0bf1fcb33e52e49941e6e9dc0298ddbe434b674` |
| source-details | `a65637e336c8b3a09df0243fe199269daffb107011defdf1b19441fbbe642968` |
| close-source-details | `6876763b7cfbce6f257eff8ad37ed621de0658a3a410a91bd7d3bf93c6ea26fb` |
| return-to-buildings-world | `737439d0a45fcfb519bba2ed31993d67468defc185f4623fa4119dcbe4b9f9d4` |
| death-gallery | `d31bbfd28440ce1d6f33acef30c16272e551bf7608d0c9c371a51a3bbd8c06a2` |
| death-effect | `2e2d7da2ec5c58dc62413896f01c623a5c854d3154358a06a977ea5db52a77d7` |
| death-step | `affb48d188e20a5ddb16e9620615db5014f02bdef0ccf3f5e39024c7da2dd0b3` |
| death-retired | `a0aeb145b484f9d37a38cb828913a8ba54f2f22a11d20cef4e417a6791e26c29` |

Shared failure replay `40a009c8-0500-4427-bf60-f843455a59b5` on the final source passed all five cases with exit 0 in 96.448s. It used `node src/godot-game/tools/game-tests/check.cjs --failed .local/runs/game-tests/21a61405-8549-4a4a-8930-1be4284c0e2e/summary.json --review .local/temp/phase05-b7e419c-review.json`. The legacy runner row executed only `runner.selection` in that replay.

## Focused regression evidence

All focused Windows receipts below used clean `74a424cc070f642854afe8b1f166672692e3a3d5`, Node 24.22.1, .NET 10.0.404 and Godot 4.7.2 Mono unless another revision is named.

| Invocation | Exit and measured result | Reproduction selection |
| --- | --- | --- |
| `4ec0a827-139c-4d84-8246-f21bf810ff20` | Exit 0, 5/5 inherited rules. The five cases executed 25, 18, 7, 7 and 18 stages respectively. | `node src/godot-game/tools/verification/check.cjs --cases ac-08.complete,R-50.positive,rules.godot-companion-positive,rules.godot-companion-negative,rules.exemptions` |
| `b3bf6428-9895-4ce8-9fab-072bbaf773c2` | Exit 0, 3/3 in 1,032.166s. Foundation permission child `32937371-341b-471d-92fb-95d5bd0b62b1` passed all 72 stages in 930.384s with `EXACT_PRODUCTION_EVENT_GUARD`. Both actual native text checks passed. | Shared `--failed .local/runs/game-tests/9b4f3dbb-0899-411f-ba83-39f9ca74e01c/summary.json` or the three failed IDs in the repair record |
| `df146942-ef21-43d5-ae20-22060814c2b6` | Exit 0, 3/3 in 96.854s. Real world replacement completed under `532f259a-fd10-44ef-9511-48ab40fe4273`. | Shared `--case asset-pipeline.world.effects,asset-pipeline.world.inspector,asset-pipeline.world.replacement` |
| `b340809f-cd07-44ac-99f2-a2bcbb5e3876` | Exit 1, 8 passes and 2 failures in 593.237s. The two export cases stopped at the missing canonical shared template archive. | Shared `--case asset-pipeline.layouts.theme,asset-pipeline.layouts.inventory,asset-pipeline.layouts.legacy-target,asset-pipeline.layouts.camp,asset-pipeline.layouts.map-warnings,asset-pipeline.appearance.corpses,asset-pipeline.appearance.crowds,asset-pipeline.hub.source,asset-pipeline.export.hub-consumer,asset-pipeline.export.portability` |
| `beaa8031-baaf-47ae-b945-59df3da9860d` | Exit 0, 2/2 in 159.652s. Fresh portably exported Windows binaries and PCKs ran the clean, altered and relaunch subjects. | Shared `--failed .local/runs/game-tests/b340809f-cd07-44ac-99f2-a2bcbb5e3876/summary.json` or `--case asset-pipeline.export.hub-consumer,asset-pipeline.export.portability` |
| `5562ace7-2ebe-494b-9f15-53a83274887c` | Exit 0, 2/2 in 32.240s wall. Rendered solo took 8.113s with one process. Rendered LAN took 7.927s with a host and three guests. | Shared `--case gameplay-checks.rendered.tracer,lan-checks.rendered.walkers --review .local/temp/phase05-74a424c-review.json` |

The shared counter locates each report at `.local/runs/game-tests/<invocation>/summary.json`. Inherited rules use `.local/runs/verification/<invocation>/summary.json`. The initial and intermediate failures remain in [the initial run record](05-INITIAL-RUN.md) and [the focused repair record](05-REPAIR-RUN.md).

For the export prerequisites, use the exact archive and official checksum manifest named in [the export toolchain pin](../../../src/godot-game/production/assets/export-toolchain.json). The verified 1,202,598,411-byte retained archive and its official manifest were copied from the previous checkout cache into the shared `.local/downloads/export-templates/` directory. The runner authenticated the archive and all four Windows template members before exporting.
