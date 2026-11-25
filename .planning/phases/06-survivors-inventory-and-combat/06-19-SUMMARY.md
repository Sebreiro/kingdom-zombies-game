---
phase: 06-survivors-inventory-and-combat
plan: "19"
subsystem: test-runners
tags: [godot-import, rendered-runner, lan-runner, run-report, gap-closure]
status: complete

requires:
  - phase: 06-survivors-inventory-and-combat
    provides: "06-18 left the build, export and permission proofs green; run 1 listed F1 and the report defect"
provides:
  - "The Phase 2 gameplay rendered runner launches one run-owned, editor-imported copy of the current game per invocation (F1)"
  - "Every rendered LAN role (G2-G4, extra and restarted peers) launches from one prepared copy made inside scenarios.run (F1)"
  - "Both runners require the copied KingdomZombies.dll and KingdomZombies.Core.dll to equal the built ones and record their identity"
  - "Every shared-report result has a numeric durationMs and launchCount; scenario groups report their real process count"
affects: [06-24, 06-25]

actuals:
  tokens: 3846
  tasks: 3
  commits: 3
plan_head_before: e5303ce85f52fbe38feadc732f5211e4463436da
plan_head_after: a094216f14e6c22e7c39f044b0437236e1f5a249

tech-stack:
  added: []
  patterns:
    - "Rendered runners never launch the unimported checkout. They call prepareRenderedProject, which imports through the asset-pipeline command wrapper and binds the copied assemblies to the built ones"
    - "A scenario driver exposes its launched game processes as state.peers or state.launches records with a pid"
    - "A report value an adapter did not record is 0 with timing or launchRecord set to not-recorded"

key-files:
  created: []
  modified:
    - src/godot-game/tools/gameplay-checks/rendered.cjs
    - src/godot-game/tools/gameplay-checks/check.cjs
    - src/godot-game/tools/lan-checks/scenarios.cjs
    - src/godot-game/tools/game-tests/scenario-group.cjs
    - src/godot-game/tools/game-tests/suites.cjs
    - src/godot-game/tools/game-tests/runner.test.cjs
    - src/godot-game/tests/scenarios/phase06-packaged.cjs
    - src/godot-game/tests/scenarios/phase06-inspection.cjs

key-decisions:
  - "F1 is repaired in runner preparation only. FoundationAppearanceAssets and every other production file are unchanged."
  - "The LAN copy is prepared inside scenarios.run, so the shared suite and the standalone lan-checks runner get the same preparation. A failed preparation blocks the selected cases with PROJECT_PREPARE."
  - "Scenario groups put the group's launched-process count on every case. The group total therefore takes the maximum, as rendered groups already did, instead of a sum."
  - "Launches are counted from driver records with a pid. The packaged and inspection drivers had none, so they now record theirs."

patterns-established:
  - "No rendered runner starts the source checkout directly. It starts a prepared imported copy whose assemblies match the build"

requirements-completed: [COMBAT-08, CHAR-01, CHAR-02]

coverage:
  - id: D1
    description: "The Phase 2 rendered runner prepares one imported copy and starts its session with --path set to that copy"
    requirement: "COMBAT-08"
    verification:
      - kind: integration
        ref: "src/godot-game/tools/game-tests/runner.test.cjs#PreparedProject_InheritedRendered_LoadsImportedAtlases"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case gameplay-checks.runner.integrity,gameplay-checks.runner.processes,gameplay-checks.runner.repeatability (runs 41d82a13 on 37a1488 and 403a7d44 on a094216, PASS)"
        status: pass
    human_judgment: false
  - id: D2
    description: "Host and guests of the rendered LAN suite start from one prepared copy in the invocation directory"
    requirement: "CHAR-02"
    verification:
      - kind: integration
        ref: "src/godot-game/tools/game-tests/runner.test.cjs#PreparedProject_RenderedLanSuite_UsesOneCopy"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case lan-checks.runner.tracer,lan-checks.network.tracer,lan-checks.network.four-peer,lan-checks.runner.processes (run cec82f8a on 6d3632c, PASS 4/4)"
        status: pass
    human_judgment: false
  - id: D3
    description: "Every shared-report result carries a numeric durationMs and launchCount, and scenario groups report their real launches"
    requirement: "CHAR-01"
    verification:
      - kind: unit
        ref: "src/godot-game/tools/game-tests/runner.test.cjs#RunScenarios_MockDriver_RecordsDurationsAndLaunches"
        status: pass
      - kind: unit
        ref: "src/godot-game/tools/game-tests/runner.test.cjs#RunGroups_LegacyAdapters_ReportNumericTiming"
        status: pass
    human_judgment: false
  - id: D4
    description: "The 21 inherited rendered cases pass on the desktop from the prepared copy"
    verification: []
    human_judgment: true
    rationale: "Rendered selections need a clean independent review of the exact revision. They are rerun on the desktop in 06-25."

duration: 17min
completed: 2026-11-25
---

# Phase 6 Plan 19: Imported copies for inherited rendered runners and real report timing Summary

**The Phase 2 gameplay and Phase 3 LAN rendered runners now start a run-owned, editor-imported copy of the current game whose assemblies match the build. Every result in the shared report carries a measured durationMs and launchCount.**

## Performance

- **Duration:** about 17 min
- **Started:** 2026-11-25T15:23:20Z
- **Completed:** 2026-11-25T15:40:00Z
- **Tasks:** 3
- **Files modified:** 8

## F1 reproduction (headless, before the repair)

A scratch script (since deleted) launched the game headless through the shared LAN launcher (`network.launch`, `rendered:false`). It opened the menu, activated `Menu/Start` and watched the frames. It made one attempt against the unimported checkout and one against a `prepareProject` copy. The production code was the same in both.

| Attempt | `--path` | Outcome | Ready frames | Actors | Loader errors |
| --- | --- | --- | ---: | ---: | ---: |
| source | `src/godot-game/game` | `ERROR: No loader found for resource: res://assets/common/donor-kit.png (expected type: unknown)`, then `ERROR: Error loading resource: 'res://assets/common/donor-kit.png'.` | 0 of 247 | 0 | 2 |
| prepared | `<run>/imported-game` | World entry | 202 of 224 | 1 | 0 |

The source attempt never placed an actor, the condition that produced `START_ONE_ACTOR` in run 1. The run-1 gameplay session log (`.local/runs/gameplay-checks/win32-x64/d1520520-…/session/game.log`) has the same line nine times. The prepared copy's import cache holds `donor-kit.png-c302e8d249063c739de41a9b5338a529.ctex`, and its import took about 6 s.

## Accomplishments

- `gameplay-checks/rendered.cjs` adds `prepareRenderedProject(context)`. It calls `lan-checks/network.cjs` `prepareProject`, which imports through the asset-pipeline `command(…,'game-import',…)` wrapper and so applies `editorImportPolicy`. It then requires the copy to be a direct child of the run directory and the copied `KingdomZombies.dll` and `KingdomZombies.Core.dll` to equal the built ones. `createSession` takes an injectable `spawn`.
- `gameplay-checks/check.cjs` `nativeContext` prepares one copy per invocation before the session. Every native result records `preparedProject` (path and assembly SHA-256). `validateResources` still binds the checkout's `src/godot-game/game` inputs.
- `lan-checks/scenarios.cjs` `run` prepares one copy before the first group and passes it on through the shared context, so every `create`, `extra` and restart launch uses it. The desktop lock and evidence reconciliation are unchanged.
- `game-tests/scenario-group.cjs` measures each case with `performance.now()`. A blocked case gets 0. After close, it writes the count of launched processes (peers and recorded launches with a pid) on every case.
- `game-tests/suites.cjs` `measured` gives every result a numeric `durationMs` (from `durationMs` or `durationMilliseconds`) and `launchCount`. When an adapter recorded neither, the value is 0 with `timing: 'not-recorded'` or `launchRecord: 'not-recorded'`. Scenario and rendered groups take the maximum, other groups the sum.

## Task Commits

1. **Task 1: Phase 2 rendered runner launches a prepared imported copy** - `37a1488` (fix)
2. **Task 2: Phase 3 rendered LAN suite launches every role from one prepared copy** - `6d3632c` (fix)
3. **Task 3: Run report carries real per-case durations and launch counts** - `a094216` (fix)

## Verification

| Check | Revision | Run | Outcome |
| --- | --- | --- | --- |
| `node --test src/godot-game/tools/game-tests/runner.test.cjs` | working tree for each task | n/a | 9/9 pass, including the four new tests |
| `node --test …/runner.test.cjs …/catalog.test.cjs` | a094216 content | n/a | 25/25 pass (catalog 16/16) |
| `node --test …/basic.test.cjs` | a094216 content | n/a | 3/3 pass |
| `check.cjs --case gameplay-checks.runner.integrity,.processes,.repeatability` | 37a1488 (clean) | `41d82a13-78ba-41f2-a5d9-b2b412e0d249` (gameplay `f549ddd2`) | PASS 3/3 |
| `check.cjs --case lan-checks.runner.tracer,lan-checks.network.tracer,lan-checks.network.four-peer,lan-checks.runner.processes` | 6d3632c (clean) | `cec82f8a-16d4-4ad6-88c9-88a62db23177` (lan `6bca445e`) | PASS 4/4 |
| `check.cjs --case basic.repository,gameplay-checks.runner.integrity,.processes,.repeatability` | a094216 (clean) | `403a7d44-d04b-4457-837c-b8487b44d39b` (gameplay `f96ec407`) | PASS 4/4. `basic.repository` ran the new runner tests in 33.1 s, under its 90 s limit. Every result has numeric `durationMs` and `launchCount`. The C# gameplay cases record no timing and say so |
| `git diff 09e2881..HEAD --stat -- src/godot-game/game` | a094216 | n/a | empty, so no production change |

## Decisions Made

See `key-decisions` above. The production loading through `ResourceLoader` is unchanged. Both inherited runners now use the same preparation the packaged game and the Phase 6 groups already relied on.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 2 - Missing critical] The packaged and inspection drivers now record their launched processes**
- **Found during:** Task 3
- **Issue:** The plan counts launches from the driver state's peers. `phase06-packaged.cjs` (6 processes) and `phase06-inspection.cjs` (1 process) keep no peers, so they would still report 0, which is the defect being fixed.
- **Fix:** Both drivers keep a `launches` list of `{role, pid}`. Packaged records every `portableProcess` and the `release-boot` run. Inspection records the session process when it closes. A packaged release-boot that fails inside the command wrapper exposes no pid and is not counted.
- **Files modified:** src/godot-game/tests/scenarios/phase06-packaged.cjs, src/godot-game/tests/scenarios/phase06-inspection.cjs
- **Committed in:** a094216

**2. [Rule 1 - Bug] The scenario-group total no longer multiplies launches by the case count**
- **Found during:** Task 3
- **Issue:** `runGroups` summed `launchCount` for every group not ending in `.rendered`. With the group count on every case, a six-case solo group would report six launches.
- **Fix:** `groupLaunches` takes the maximum for scenario groups as well. `RunGroups_LegacyAdapters_ReportNumericTiming` asserts 2 for a two-case group.
- **Files modified:** src/godot-game/tools/game-tests/suites.cjs
- **Committed in:** a094216

**3. [Rule 2 - Missing critical] The Phase 2 rendered runner records each scenario's wall time**
- **Found during:** Task 3
- **Issue:** Its results had no duration, so normalization could only report 0 as not-recorded.
- **Fix:** `runScenarios` in `gameplay-checks/rendered.cjs` adds a measured `durationMilliseconds`, with 0 for a blocked scenario. The shared report copies it.
- **Files modified:** src/godot-game/tools/gameplay-checks/rendered.cjs (planned for Task 1, changed in Task 3)
- **Committed in:** a094216

**4. [Rule 3 - Blocking] suites.cjs calls `processes.run` through the module**
- **Found during:** Task 3
- **Issue:** The destructured `run` could not be replaced, so `RunGroups_LegacyAdapters_ReportNumericTiming` could not drive the legacy and scenario adapters without starting real children.
- **Fix:** The module object is called at run time. Behavior is unchanged.
- **Committed in:** a094216

---

**Total deviations:** 4 auto-fixed (2 missing critical, 1 bug, 1 blocking)
**Impact on plan:** All four serve the planned "real launches and timings" truth. No production code changed.

## Issues Encountered

- The plan expected catalog.test.cjs to have 24 tests. It has 16, the same as before this plan, and all pass.
- The scratch diagnosis first failed with `OBSERVATION_OWNERSHIP` because its run root was not named after the nonce. Naming it after the nonce fixed that. This was a scratch-script error only.

## Known Stubs

None.

## Threat Flags

None. The prepared copy stays inside the invocation's run directory. The assembly identity check and the import policy mitigate T-06-33. Measured values, with explicit not-recorded markers, mitigate T-06-34.

## User Setup Required

None.

## Next Phase Readiness

- 06-20 to 06-24 continue the sequential gap plans. The 21 inherited rendered cases (`gameplay-checks.rendered.*` and `lan-checks.rendered.*`, including both Critical smoke cases) are rerun on the desktop in 06-25 after the independent review.
- Still open and owned elsewhere: `bindings.test.cjs` "ac-08 derivation…" and the F3 `REPOSITORY_SOURCE_HASH` cases (06-24). The `guardedHostTuples` `EVIDENCE_NATIVE_HOST_EXTENT` mismatch stays deferred (deferred-items.md).

---
*Phase: 06-survivors-inventory-and-combat*
*Completed: 2026-11-25*

## Self-Check: PASSED
