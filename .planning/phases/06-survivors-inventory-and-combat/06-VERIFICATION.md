---
phase: 06-survivors-inventory-and-combat
verified: 2026-11-25
status: gaps_found
score: 66/123 selected checks passed in the initial phase-end run, with 39 failed and 18 blocked
revision: a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d
---

# Phase 6 verification

**Phase 6 is not verified.** The first fresh phase-end run on the reviewed revision failed. Both Critical smoke cases failed, no Phase 6 native solo or four-peer case passed, and none of the 18 asset handoffs has an accepted outcome. The repairs change reviewed source, so they need a fresh independent review before the focused and final runs. The pull request stays a draft.

## What is established

| Evidence | Source | Result |
| --- | --- | --- |
| Independent whole-phase review | `17fb851c2bfd81ba050fa094527ef21d7707690c`, recorded for revision `a9ac95f` in `.local/temp/phase06-review.json` | Clean ([06-REVIEW](06-REVIEW.md)) |
| Hosted basic jobs | `a9ac95f`, workflow run [45494079823](https://github.com/Sebreiro/kingdom-zombies-game/actions/runs/45494079823) | Both passed: Windows (job `138799796572`) and Ubuntu (job `138799796944`) |
| Local basic checks in the phase-end run | `a9ac95f`, invocation `ee17949e-45ed-4486-9915-dc0b7029cdf1` | 4/4: strict build, repository, pure tests and native headless smoke |
| Phase 6 native headless gameplay and Release package | Same invocation | 6 of 7 passed. `phase06.headless.lan-privacy` failed intermittently (F13) |
| Edge census | `node src/godot-game/tools/game-tests/edge-check.cjs 6` at `a9ac95f`, exit 0 | 132 of 132 expected edge methods resolved, with no E074 (deferred to Phase 12). The 06-14 record shows all 132, plus the P01 and P02 controls, passing at `17fb851` |

## Initial phase-end run

The command was `node src/godot-game/tools/game-tests/check.cjs --phase-end --phase 6 --review .local/temp/phase06-review.json`. Base was `origin/dev` `b9aaa2b`. The run took 6799.742 s, from `2026-11-24T21:43:57.588Z` to `23:37:17.330Z`. The [rendered evidence record](06-RENDERED-EVIDENCE.md) has the selection, omissions, per-group outcomes, image identities and each failure's original result and observed cause (F1–F13).

## Blocks

| Block | Affected evidence | Required action | Owner |
| --- | --- | --- | --- |
| Inherited rendered runners and fixtures do not handle Phase 6 additions (F1–F6, F8, F9) | Critical `gameplay-checks.rendered.tracer` and `lan-checks.rendered.walkers`, 19 other inherited rendered cases, Phase 1 rule/state fixtures, Phase 3 permission consumer, Phase 4 export snapshot and native permission census | Repair runner preparation and source bindings, or load the atlases without an import cache. Rerun the affected cases | Phase 6 repair plan |
| Phase 6 rendered drivers have never passed (F10–F12) | 6 solo, 3 LAN and all 18 handoff inspections | Fix the solo, LAN and inspection driver defects, then rerun on the real desktop | Phase 6 repair plan |
| Unexplained failures (F7, F13) | `engine.generated-origin`, `engine.generated-boundary`, `phase06.headless.lan-privacy` | Diagnose and repair, or show the failure is unrelated | Phase 6 repair plan |
| Changed source needs review | Every repair | Get a fresh independent review of the changed source. Then run `--failed` against the run 1 report, followed by a fresh full run of the selected set | Phase 6 orchestrator and reviewer |
| Image judgments | P1–P4, native solo and four-role captures, 18 handoff checkpoints | Review the final run's own captures | Phase 6, after the final run |

## Acceptance reconciliation

The 29 source IDs, 41 acceptance criteria, 48 locked decisions and 18 handoffs are not reconciled. Their rendered and native evidence comes from the final run of the selected set, which has not been produced. The 132 resolved edges, P01 and P02 are established only as listed above.

## Separate procedures

These keep their own owners and are not part of this phase's local run: actual mixed-OS physical LAN, supported-display coverage, reference-hardware 1920×1080 stress, real-duration endurance and release procedures, plus Phase 11 certification under TEST-12 (full hosted rendered and LAN runs, coverage, evidence reuse and the renewal of the Phase 4 review gates). Phase 6's own pending procedures `A-05`, `A-22`, `A-23` and `A-27` were omitted from the selection as pending. They receive no credit.

## How to reproduce

1. Prepare the pinned tools as described in [setup](../../../docs/workflow/setup.md). Write the review record `.local/temp/phase06-review.json` with the reviewed revision's full ID, the reviewer and `status: clean`.
2. Run `node src/godot-game/tools/game-tests/check.cjs --pr` and `node src/godot-game/tools/game-tests/edge-check.cjs 6`.
3. Run `node src/godot-game/tools/game-tests/check.cjs --phase-end --phase 6 --review .local/temp/phase06-review.json` on the development Windows desktop at 1280×720, keeping the game in the foreground.
4. After repairs, run `--failed <run report> --review FILE`, then the full `--phase-end` command once more, following the [shared test procedure](../../../docs/workflow/shared-game-tests.md).
