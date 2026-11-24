# Shared game tests

Run these commands from the checkout root using the pinned tools in [Development setup](setup.md). The shared catalog links the acceptance scenarios, production checks and existing suites. It records the expected setup, actions, outcomes and evidence for each scenario.

| Command arguments | Selection |
| --- | --- |
| `--list` | Validate and print the catalog |
| `--pr` | Strict build, repository checks, one pure test process and one headless solo smoke |
| `--tier 1` | Pure tests in one process |
| `--tier 2` | Active headless scenarios with their declared group setup |
| `--tier 3 --review FILE` | Active rendered scenarios |
| `--phase-end --review FILE` | Required Critical cases, new or changed cases, and affected regressions against `origin/dev` |
| `--case ID[,ID]` or `--group GROUP` | An exact selection using IDs from the shared catalog |
| `--failed REPORT` | Failed and blocked cases from the named shared run |
| `--full --review FILE` | All active executable checks, across all importance levels |

For example, run `node src/godot-game/tools/game-tests/check.cjs --pr`. Use `--base REF` if the branch to compare for `--phase-end` differs from `origin/dev`. Use `--phase N` for later phases. The default is 5. For a focused rendered rerun, include `--review FILE`.

## Phase-end procedure

1. After the whole-phase code review, put its verdict in a local JSON file with `revision`, `reviewer` and `status`. Use the reviewed commit's full Git ID and set `status` to `clean` after resolving the review findings. Use the development Windows desktop at 1280×720 and keep the game in the foreground while the rendered driver runs.
2. Run `--phase-end --review FILE`. The runner records its selection, reasons and omissions before work starts. Read the resulting `summary.json` and linked logs, observations, images and suite reports under `.local/runs/`. Per-case failures include exact replay arguments.
3. Inspect the hub images and record P1–P4 using the Phase 4 procedure. Cite the tested commit, invocation, checkpoint and evidence for every judgment. The objective run reports `needs_review` while these observations are pending.
4. If a failure requires a repair, keep its original report. Run `--failed REPORT`, plus any newly affected cases, against the repaired build. When those checks pass, run the complete reduced selection once on the repaired, reviewed commit. An initial clean run needs no duplicate launch.
5. Preserve the tested revision, selection, commands, timing, outcomes, observed judgments and evidence locations in the phase record, then follow [Workspace cleanup](cleanup.md).

Planned cases and omitted checks receive no result credit. Hosted PR, rendered desktop, physical LAN, reference hardware and endurance evidence serve distinct acceptance procedures.

## Adding checks

Add a test to the existing verification project and opt pure tests into tier 1 using `[Trait("Tier", "1")]`. Name specification edge tests with the phase and edge identity, for example `Phase06_E001_Description`. Run `node src/godot-game/tools/game-tests/edge-check.cjs 6` to check the Phase 6 test inventory against the specification. Add scenario rows to the shared catalog with effective phase, requirement, impact inputs, group order, setup, production actions, expected outcomes and artifact requirements. For a new native scenario group, add a `scenario` row to `game-tests/catalog.json` and a driver file under `src/godot-game/tests/scenarios/`. Use one driver path per run group. Its exports are `open(context)`, `cases` keyed by catalog ID, `reset(state, context)` and `close(state, context)`. Each case uses the group state and returns the paths of required evidence written inside the run directory. Keep production input, fresh state and process ownership in the scenario driver. The shared runner owns selection, ordering, bounded execution and report publication.

The existing runners still expose their original entry points for individual suite diagnosis.
