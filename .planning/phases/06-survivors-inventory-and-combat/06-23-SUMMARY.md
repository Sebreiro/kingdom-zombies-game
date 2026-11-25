---
phase: 06-survivors-inventory-and-combat
plan: "23"
subsystem: testing
tags: [godot, csharp, inspection-driver, native-observer, f10, h14, h16, reset, gap-closure]
status: complete

requires:
  - phase: 06-22
    provides: "Exact receipt correlation and the current producer replay"
provides:
  - "The inspection driver hashes fixture stimuli with the content.cjs hash export (H14)"
  - "H16 expectations derived from locomotion-melee.json with the presentation's step and boundary rules"
  - "CharacterPresentation.Sample uses the same 1e-8 boundary tolerance as Remaining"
  - "nativeObserver discards the late response of a request abandoned at its deadline"
  - "An inspection reset that releases input, restores the recorded fixture cleanup, then the authored baseline, and names any failed stage"
affects: [06-24, 06-25, 06-15]

actuals:
  tokens: 7400    # chars/4 over the realized diff (29609 chars)
  tasks: 2
  commits: 2
plan_head_before: 366d9e39af690b43fdf107b7e131b1569b986d5b
plan_head_after: 987248a0e25a78196dd8dcc3fe2140f587201db0

tech-stack:
  added: []
  patterns:
    - "Driver expectations derive from authored data by the product's own sampling rules, never from observations"
    - "A native request abandoned at its deadline keeps its place in an outstanding FIFO, and its late response is discarded"
    - "Reset is staged (RELEASE, FIXTURE, BASELINE), writes a reset record and names the failing stage"

key-files:
  created:
    - .planning/phases/06-survivors-inventory-and-combat/06-23-SUMMARY.md
  modified:
    - src/godot-game/tests/scenarios/phase06-inspection.cjs
    - src/godot-game/tests/scenarios/phase06-drivers.test.cjs
    - src/godot-game/tools/asset-pipeline/scenarios.cjs
    - src/godot-game/core/CharacterPresentation.cs
    - src/godot-game/tests/verification/CharacterPresentationTests.cs
    - .planning/phases/06-survivors-inventory-and-combat/deferred-items.md

key-decisions:
  - "H16: the driver expectation matched the authored data, and the presentation was wrong. At stride 0.08 + 0.16 = 0.24 s, Sample subtracted 0.08 then 0.16 and reached -2.8e-17 < 0, so it returned airborne-up. Remaining already used a 1e-8 tolerance and treated the same instant as airborne-down. Sample now uses the same tolerance"
  - "The H16 sampling defect is Phase 4 code, not a Phase 6 regression. Sample is byte-identical to b9aaa2b, and H16 never ran on the real desktop before run 1"
  - "Reset cause: the H16 settle deadline expired with a scene-tree request in flight. Its late response answered the reset's first scene-tree request, and the reset's own response then hit the duplicate guard (RENDERED_NATIVE_RESPONSE). The fix belongs in the shared nativeObserver in scenarios.cjs"
  - "No extra process or relaunch per case. The reset reuses the single G1-hub process"

patterns-established:
  - "A Core C# change outside src/godot-game/game leaves current-replay.cjs unchanged (--check PASS), so no replay chore commit is needed"

requirements-completed: [COMBAT-08, INV-01, INV-13]

coverage:
  - id: D1
    description: "H14 builds its recipe with content.cjs hashes for the original and prepared stimulus"
    requirement: "INV-13"
    verification:
      - kind: unit
        ref: "node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs (InspectionDriver_FixtureIdentity_UsesExportedHash)"
        status: pass
    human_judgment: true
    rationale: "The H14 rendered handoff runs on the real desktop in 06-25"
  - id: D2
    description: "H16 expects the authored samples, and the presentation now shows airborne-down at the 0.24 s step"
    requirement: "COMBAT-08"
    verification:
      - kind: unit
        ref: "node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs (H16Expectations_AuthoredLocomotion_MatchSteps)"
        status: pass
      - kind: unit
        ref: "CharacterPresentationTests.Step_NextSampleSeconds_LandsOnEachAuthoredSample (jump and walk), in asset-pipeline run bea17b4e, 37/37"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.animation.melee,asset-pipeline.animation.bow,asset-pipeline.appearance.corpses,basic.pure (run 0b391ccd, PASS 4/4)"
        status: pass
    human_judgment: true
    rationale: "The H16 rendered handoff runs on the real desktop in 06-25"
  - id: D3
    description: "A failed inspection handoff resets to the authored baseline, so the other handoffs keep their own outcomes"
    requirement: "INV-01"
    verification:
      - kind: unit
        ref: "node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs (InspectionReset_AfterFailedStep_RestoresFixtureAndBaseline)"
        status: pass
      - kind: unit
        ref: "node --test src/godot-game/tools/game-tests/catalog.test.cjs (phase06.inspection selects 18)"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.hub.source (run 3ec783a7, PASS 1/1)"
        status: pass
    human_judgment: true
    rationale: "The 18 handoffs run individually on the real desktop in 06-25"

metrics:
  duration: 24min
  completed: 2026-11-25
---

# Phase 6 Plan 23: Inspection driver F10 repair Summary

**The inspection driver now uses the content.cjs hash for H14 stimuli and derives H16 expectations from locomotion-melee.json. CharacterPresentation now samples the 0.24 s jump boundary as authored. The shared native observer discards the late response of an abandoned request, which was the run-1 reset failure, so each of the 18 handoffs can reset to the authored baseline and get its own outcome.**

## Performance

- **Duration:** about 24 min (19:18Z to 19:42Z)
- **Tasks:** 2 of 2
- **Files modified:** 5 source files and 1 planning record

## Accomplishments

### Task 1: H14 hashing and H16 expectations (`6e7a425`)

**H14.** `fixture()` called `evidence.hash`, which evidence.cjs does not export (its `module.exports` at line 919 has no `hash`). The driver now binds `const {hash} = require('../../tools/asset-pipeline/content.cjs')`, the function evidence.cjs uses internally. The identity fields `original` and `prepared` keep their meaning. Edits stay inside the prepared copy under the realpath check, and `restore()` writes the original bytes back.

**H16: what failed.** The failing step was `step-held-jump`, the third step after Jump was selected. The run-1 packets (`.local/runs/game-tests/ee17949e-…/cc10164d-…/inspection/asset-pipeline.heldout.H16-step-held-jump-failed-settle-object-0.bin`) show this sample:

```
{"movement":"jump","strideSeconds":0.24,"lowerPose":"walk-far","stage":"airborne-up","lowerOffset":[0,-6]}
```

**H16: the authored values.** The jump clip in locomotion-melee.json is:

| Sample | Starts at | Duration | Pose | Stage | Offset |
| --- | --- | --- | --- | --- | --- |
| 1 | 0 s | 0.08 | run | takeoff | [0,0] |
| 2 | 0.08 s | 0.16 | walk-far | airborne-up | [0,-6] |
| 3 | 0.24 s | 0.16 | walk-near | airborne-down | [0,-3] |
| 4 | 0.40 s | 0.10 | run | landing | [0,1] |

So at 0.24 s the authored sample is walk-near, airborne-down, [0,-3]. The driver's expectation was right, and the presentation was wrong.

**H16: why the presentation was wrong.** Step advances by `nextSampleSeconds`, which is `Remaining`. `Remaining` uses a 1e-8 tolerance. `Sample` used none: 0.24 − 0.08 = 0.15999999999999998, and minus 0.16 that is −2.8e-17, which is below 0. So `Sample` returned airborne-up at the exact instant that `Remaining` already treated as airborne-down. `Sample` is unchanged since `b9aaa2b`, so this is a latent Phase 4 boundary defect, not a Phase 6 regression. H16 had never run on the real desktop before run 1. The existing `Jump_AuthoredTrack_ExposesAllFourRegisteredStages` sidestepped the boundary by advancing 0.081 s.

**H16: the fix.** `Sample` now applies the same `time < duration - 1e-8` rule as `Remaining`. The walk steps were already correct and still are: 0 → walk-near, 0.12 → walk-far, 0.24 % 0.24 = 0 → walk-near. The jump steps now give 0 → takeoff, 0.08 → airborne-up, 0.24 → airborne-down, 0.4 → landing.

**H16: driver expectations.** The driver now derives every H16 step from the authored track with the presentation's sampling, stepping, walk-loop and jump-hold rules (`h16Steps`). It reads the track from the checkout under test, never from observations. Each step checks movement, lowerPose, stage, lowerOffset, strideSeconds and Body_lower. The step IDs and their order are unchanged.

**Tests.**
- `CharacterPresentationTests.Step_NextSampleSeconds_LandsOnEachAuthoredSample` steps a jump and a walk by `nextSampleSeconds` and requires each authored stage and pose.
- `InspectionDriver_FixtureIdentity_UsesExportedHash` and `H16Expectations_AuthoredLocomotion_MatchSteps` cover the driver. The second test derives the expected values independently from the JSON's cumulative durations.

### Task 2: reset after a failed handoff (`987248a`)

**What run 1 showed.** The artifacts in `.local/runs/game-tests/ee17949e-45ed-4486-9915-dc0b7029cdf1/cc10164d-bb83-4486-8c38-928705ad33cf/` show:
- **H14** failed before any step, and its reset succeeded (`H14-recovery-reset-2-*` and `H14-recovery-2-baseline*`).
- **H16**'s `step-held-jump` input was at 22:28:22.810Z. The settle deadline is 12 s, and the last observation was at 22:28:34.773Z.
- **The reset** failed with `RENDERED_NATIVE_RESPONSE` and wrote no H16 recovery file. The hub closed at 22:28:35.758Z.

**Cause.** The cause was not a hub still mid-input, an earlier fixture edit or an ended lifetime. It was the native transport:
1. `settle` hit its deadline after writing a `scene:request_scene_tree` request, and `request()` threw `RENDERED_NATIVE_DEADLINE` with that request still in flight.
2. The reset's first `native.snapshot` sent another scene-tree request.
3. The late response of the abandoned request filled the new pending slot.
4. The reset's own response then arrived while that slot was still full, so `e.need(!pending.value, 'RENDERED_NATIVE_RESPONSE')` set `trouble` and destroyed the channel.

The new unit test reproduces this exactly: before the fix it failed with `'RENDERED_NATIVE_RESPONSE'`.

**Fix in scenarios.cjs (shared).** `nativeObserver` keeps outstanding requests in a FIFO. A request that reaches its deadline is marked abandoned but keeps its place, and the matching late response is discarded. Each request receives only its own response. A cap of 16 outstanding requests guards the backlog. The session also exposes `release()`, which is `input.release()`.

With no abandoned request, behavior is identical: one request at a time, the same thread check and the same deadlines. `asset-pipeline.hub.source` passes, and `runInspectionLane`/hub-navigation are unchanged.

**Fix in the driver.** `perform` records the case's fixture `cleanup` until it succeeds. `reset` then runs three stages in order:
1. `RELEASE` releases held keys and the mouse.
2. `FIXTURE` reruns the recorded cleanup, which restores and verifies the original bytes.
3. `BASELINE` runs the original `restore(state, item, 2)` within the 12 s native bounds.

The fixture stage runs even if release failed. A failed stage throws `INSPECTION_RESET_<STAGE> <case>: <cause>`, and only then are later cases blocked. Every reset writes `<case>-reset.json` (completed stages, failures, recovery), which `inspection-cleanup.json` lists under `resets`. No process is relaunched.

**Test.** `InspectionReset_AfterFailedStep_RestoresFixtureAndBaseline` has four subtests:
- A fake remote debugger proves the stale-response discard.
- A mocked session runs a failed `CHARACTER-PREVIEW.error` whose own cleanup fails once. The reset restores the bytes and the order is release, then baseline restore with `reset-2`/lifetime 2.
- A failed release still restores the fixture, skips the baseline and names `INSPECTION_RESET_RELEASE`.
- A baseline failure names `INSPECTION_RESET_BASELINE` and its native cause.

## Verification

| Command | Result |
| --- | --- |
| `node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs` (after Task 1) | 55/55 pass, 0 fail; both inspection tests present |
| `node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.animation.melee,asset-pipeline.animation.bow,asset-pipeline.appearance.corpses,basic.pure` | First run `ea07b2e4-4ccc-4234-bea1-3900f691b026`: FAIL 0/4. basic.pure failed its build with SCR0007, a 121-character line in the new test summary, and the 3 asset cases were blocked. After fixing the line, run `0b391ccd-158a-4a4e-b3ce-b205e3e8dc44`: **PASS 4/4**. animation.melee compiled CharacterPresentationTests 37/37, including both new rows (asset-pipeline run `bea17b4e`) |
| `node src/godot-game/tools/verification/current-replay.cjs --check` | PASS, unchanged (57 historical, 22 changed, 22 added; sourceInputsSha256 `482fcebf…f8e8`). No replay chore needed |
| `node --test …phase06-drivers.test.cjs --test-name-pattern=InspectionReset` before the Task 2 fix | RED: 4 subtests failed. The observer subtest failed with `'RENDERED_NATIVE_RESPONSE'` |
| `node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs src/godot-game/tools/game-tests/catalog.test.cjs` | 76/76 pass, 0 fail. The reset test is present, and catalog.test still selects the 18 `phase06.inspection` handoffs |
| `node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.hub.source` | Run `3ec783a7-a6ff-4d7a-b29e-1684ba4f021a`: **PASS 1/1** (asset-pipeline `c7d9a2fc`) |
| `node src/godot-game/tools/game-tests/check.cjs --case gameplay-checks.core.permissions` (after Task 1) | Run `28f3fd59-6a18-4edd-848e-80c70719119e` / gameplay-checks `229f4785`: FAIL with the same message as before (`src/godot-game/core/CharacterPresentation.cs`). This is the known deferred item; see below |

No rendered runs were made. The 18 handoffs rerun on the real desktop in 06-25.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] The H16 premise said "a regression of the accepted Phase 4 contract." It is a latent Phase 4 defect.**
- **Found during:** Task 1
- **Issue:** The plan framed a presentation-side cause as a Phase 6 regression. `git diff b9aaa2b` shows `Sample` unchanged. The boundary defect predates Phase 6, and H16 never ran before.
- **Fix:** The product fix is as the plan prescribes, in CharacterPresentation.cs. The record names it Phase 4 code.
- **Commit:** 6e7a425

**2. [Rule 2 - Missing test] A C# unit test for the boundary in CharacterPresentationTests.cs, a file outside the plan's list**
- **Found during:** Task 1
- **Issue:** The existing jump test advanced 0.081 s and never stepped exactly onto a boundary.
- **Fix:** Added `Step_NextSampleSeconds_LandsOnEachAuthoredSample`. A first build failed SCR0007 on a 121-character summary line, which was shortened.
- **Commit:** 6e7a425

**3. [Rule 1 - Bug] The reset defect was in the shared native observer (scenarios.cjs), not in the driver**
- **Found during:** Task 2
- **Fix:** The abandoned-request FIFO in `nativeObserver`. `nativeObserver` is exported for the unit test, and `release()` was added to the session object. Neither changes hub-navigation.
- **Commit:** 987248a

**Total deviations:** 3, all auto-fixed. No architectural change.

## Deferred Issues

- **`gameplay-checks.core.permissions`** fails as before. Its cause is now diagnosed: `ac-08.complete` in required-cases.json binds the SHA-256 of about 40 sources and 2 registries, last at `b9aaa2b`. Phase 6 work had already drifted 39 sources and 2 bindings, and CharacterPresentation.cs is only the first in path order. 06-23 adds one more drifted source (CharacterPresentationTests.cs). The message does not change. This is recorded in deferred-items.md under "Found during 06-23"; its owner must decide whether to rebind or rederive.
- The catalog action text of `phase06.lan.population` still says one guest moves. 06-24 owns this, and this plan does not touch the catalog.

## Known Stubs

None.

## Self-Check: PASSED

- Files present: phase06-inspection.cjs (`hash` from content.cjs, `h16Steps`, staged `reset`, `recipes` and `fixture` exported), phase06-drivers.test.cjs (three new tests), scenarios.cjs (`outstanding`, `release`, `nativeObserver` exported), CharacterPresentation.cs (`Sample` tolerance), CharacterPresentationTests.cs (new theory).
- Commits present on HEAD: `6e7a425`, `987248a`.
