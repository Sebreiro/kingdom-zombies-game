---
phase: 06-survivors-inventory-and-combat
plan: "01"
subsystem: gameplay-authority
tags: [godot, csharp, elapsed-time, camp-safety, local-access, headless]
requires:
  - phase: 03-four-player-lan-foundation
    provides: "Identified host authority, native Start and Host flows, and the legacy snapshot"
  - phase: 05-ci-and-repeatable-game-testing
    provides: "Pinned strict build, shared PR checks, and owned native observations"
provides:
  - "A checked integral run clock committed with the current roster and geometry"
  - "Authoritative camp safety and reachable interaction checks by ready actor"
  - "Six named world edges and a one-process native safety, movement, input cleanup, and Quit proof"
affects: [06-02, 06-03, 06-04, 06-05, 06-06, 06-07, 06-08, 06-14]
tech-stack:
  added: []
  patterns: [atomic-publication, fixed-point-world-geometry, post-step-readback]
key-files:
  created:
    - src/godot-game/core/RunWorld.cs
    - src/godot-game/core/RunWorldGeometry.cs
    - src/godot-game/tests/verification/Phase06WorldTests.cs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/game/foundation/FoundationCoordinator.cs
    - src/godot-game/game/foundation/FoundationRoot.cs
    - src/godot-game/tools/game-tests/headless.cjs
key-decisions:
  - "FoundationSession publishes the run clock and world with the immutable roster. The network projection keeps its existing schema and fixture IDs."
  - "World geometry uses signed subunits and exact inclusive Euclidean range and closed obstruction tests."
  - "The native smoke selects its DEBUG fixture only for a nonce-owned first headless solo Start, then proves an ordinary second Start in the same process."
patterns-established:
  - "Check current reachability by binding, target kind, and stable ID at the point of an authoritative interaction."
  - "Treat a tick as the interval (previous, current]. An eligibility deadline is inclusive, while recovery uses only elapsed time after the deadline."
requirements-completed: [CHAR-04, INV-08, INV-09, INV-10, INV-11]
actuals:
  tokens: 15016
  tasks: 3
  commits: 6
commits: 6
plan_head_before: 53c1a63ca0fbae8739e7197e140209340d7ce593
plan_head_after: beb22578ea8b649cf7a1442f5b86fb866ae06296
duration: 20min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 01: World Time, Safety, and Local Access Summary

**Solo and host-local sessions now commit one elapsed clock and use the actor's current position to determine camp safety and stash access. The native Hint displays the result after each tick.**

## Contract for Dependent Plans

- `FoundationSession.Step(long elapsedMilliseconds)` accepts nonnegative integer milliseconds. The run clock starts at zero, uses checked signed 64-bit addition, and supports `0` through `long.MaxValue` milliseconds. A zero step may publish a revision but adds no time. Intent acceptance, repeat rejection, roster changes, and geometry replacement add no run time. Actor-ready elapsed time and the coordinator's transport clocks are separate.
- The run interval is `(previous, current]`. An eligibility deadline reached at `current` may enable an action there. Recovery that requires qualification uses only time after the deadline. The world tests verify exact timestamp progression just before, at, and after 500, 1000, 2000, 5000, and 180000 ms, with zero difference between equivalent whole and split advances. Later gameplay plans own recovery, cooldown, respawn, and cleanup effects.
- A unit is 1000 subunits and 16 authored logical pixels. The inherited actor horizontal position stays within ±59000 subunits, with speed 3 subunits per millisecond. Core accepts signed world points within ±1000000000 subunits, with the origin constrained to keep the full actor travel inside this range. Fixture positions are expressed in world coordinates. `RunWorld.ActorContact` adds the explicit origin and ground/contact offset to the inherited actor position. The authored building and the ground elevation anchor the ordinary refuge and stash at the origin.
- `RunWorld.Ranges` takes validated positive whole world units. The authored defaults are 12 u refuge, 12 u danger, 3 u for NPCs, corpses, and piles, and 4 u for the stash. A completed living outer wall sets the border independently on its side. Without one, that side uses the refuge. Camp and distance boundaries are inclusive. Terrain contact includes touching the closed blocker. Distinct ordinal IDs may share coordinates, while duplicate IDs anywhere in one detached world reject.
- `FoundationSession.LocalWorld` is the read-only host presentation tuple `(elapsedMilliseconds, insideCamp, safeDistance, stashReachable)`. Internal `SafetyFor(bindingId)` and `CanReach(bindingId, kind, targetId)` use the current committed actor for a ready binding. `TryReplaceRunWorld(candidate)` validates the immutable detached fixture collections, then commits the geometry at the next revision with the current actor and elapsed time. Invalid geometry preserves the original publication. `RunWorldGeometry` provides the range and closed segment-blocker checks for later combat consumers.

## Task Commits

| Task | Commit | Change |
| --- | --- | --- |
| 1 | `6bd6750` | Published host elapsed time and world geometry, projected the live Hint, and proved both native transitions |
| 2 RED | `8012bc7` | Proved the held-direction mutation after a rejected clock overflow |
| 2 GREEN | `e50fa8f` | Made invalid elapsed and geometry atomic and completed the six world edges |
| 3 RED | `71efa47` | Required native Hint visibility evidence |
| 3 GREEN | `beb2257` | Proved visible Hint, neutral input after release, unseeded restart, and ordinary Quit |

The Git ledger also counts `4792858`, the orchestrator's draft PR continuity commit. The realized diff measurement covers the seven implementation and test files. [Phase 6 draft PR #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16)

## Verification

Executed from the phase checkout on Windows x64 with Node 24.22.1, .NET SDK 10.0.404, and Godot 4.7.2 .NET. The closing checks used a clean `beb22578ea8b649cf7a1442f5b86fb866ae06296`.

| Command | Invocation or report | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `e8806fb9-0566-4667-887d-38da0fc0e3ea` | Exit 0, four passes, zero failures or blocked cases. Strict build, repository checks, 24 pure tests, and native headless passed. |
| `node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build` | `d953f90d-ab8d-4e09-a218-112e4a69597f` | Exit 0, one pass, zero failures or blocked cases. Rebuilt the assembly before the class filter. |
| `dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06WorldTests'` | `.local/runs/06-01-tdd/edges-final/tests.json`, CTRF | Exit 0, six passes, zero failures or skips. |

The direct filter used `--report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-01-tdd/edges-final --progress off --no-ansi`. The passing methods in `KingdomZombies.Phase1.Tests.Phase06WorldTests` were:

- `Phase06_E001_InclusiveBordersDangerAndReachability`
- `Phase06_E002_AccessIsRecheckedAfterMovementAndBlockerChanges`
- `Phase06_E003_TowersAndUnfinishedWallsLeaveTheRefuge`
- `Phase06_E004_OnlyAnAuthoritativeStepAdvancesTime`
- `Phase06_E005_OuterWallLossUsesTheNextLivingWallOnThatSide`
- `Phase06_E006_ElapsedIsExactAndInvalidStepsAreAtomic`

Run `node src/godot-game/tools/game-tests/check.cjs --case basic.headless` to repeat only the native case. The successful first tracer invocation `0e5cc844-c96a-4764-9b63-810b83e1aeb3` ran on the pending Task 1 tree based on `53c1a63`, then committed as `6bd6750` and handed off for the draft PR. It exited 0 with one pass. Nonce `d3ce996b-224e-491f-a8bd-806790766239`, PID `31912`, read the unsafe and blocked Hint at position -8000, 37 ms and revision 1. It read the safe and reachable Hint at -6488, 564 ms and revision 77.

On the clean closing revision the fresh native process was PID `34624`, nonce `8aad97ea-17be-4876-a059-69f0c70a6187`. Its two exact ordered states were:

| State | Position in subunits | Run time in ms | Session revision | Native frame |
| --- | ---: | ---: | ---: | ---: |
| Outside camp, enemy nearby, stash blocked | -8000 | 45 | 1 | 13 |
| Inside camp, danger clear, stash reachable | -6500 | 577 | 79 | 91 |

The fixture held a living completed wall at x = -6500, a hostile at (-18501, 0), a stash at (-6000, 3000), and a barrier from (-6750, 1) to (-6500, 3500), leaving the ground route clear. Both Hint states were visible in the native scene tree. Releasing right accepted a neutral direction and held position at -6437. Escape returned to the empty menu while right was still physically held. The next ordinary session stayed idle at -8000 with the default safe and stash-blocked Hint. The same process acknowledged ordinary Quit at frame 152, exited 0, and closed its streams without a cleanup kill. These are headless observations. Rendered acceptance stays in the later phase plan.

## TDD and Repairs

- Task 2 RED used the installed xUnit runner with the E006 method filter and JUnit reporting. Exit 2, one named failure, no skips. E006 expected facing 1 after a rejected overflow and legal zero step, but observed -1. The exact qualified JUnit identity received `RED_EVIDENCE_OK`. The compatibility entry now checks the proposed elapsed and revision before replacing intent. [Rule 1]
- Task 3 RED wrapped the same `basic.headless` command in the Node TAP reporter. Invocation `e9654b10-7191-49c4-af81-e7d333e4de73` executed the native transitions and failed the visibility assertion. TAP exit 1 with one failure received `RED_EVIDENCE_OK`. GREEN sampled the actual Hint's visibility at projection and re-ran the native case.
- Initial strict tries `cdb966e6-89ad-41fa-ab34-1d87061688b8`, `7309d0a0-a4c2-4dfc-ad59-7750c6046129`, and `d2c19756-51e7-40d0-bf55-51668c350e0e` caught source rules for constructors, member order, indentation, and documentation. Keeping helper methods outside the Godot script also preserved its approved generated method surface. All were resolved in the commits above. [Rule 3]
- The inherited HUD overwrites its Hint during roster projection. The world Hint now appends afterward and the process observer asserts the resulting text and visibility. [Rule 1]

## Handoff

Plan 06-02 can compose authored inputs into `RunWorld.Ranges` and keep the existing internal world mutation seam. Later recovery, inventory, and combat plans can consume the elapsed interval and recheck per-binding access against the current publication. The phase-wide named-edge census, rendered scenarios, and applicable source rebind remain with their planned owners.

Execution started at 10:53 UTC and completed at 11:13 UTC on 2026-11-24. The orchestrator owns the shared phase worktree, run evidence, branch push, and phase continuity updates.

## Self-Check: PASSED

The created files exist, all task commits are reachable, and the closing report matches the recorded clean revision.
