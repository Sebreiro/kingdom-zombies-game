---
phase: 06-survivors-inventory-and-combat
plan: "05"
subsystem: gameplay-authority
tags: [godot, csharp, locomotion, stamina, inventory, recovery]
requires:
  - phase: 06-01
    provides: "Shared elapsed clock, camp borders and local world geometry"
  - phase: 06-03
    provides: "Exact carried item paths, quick references and inventory publication"
provides:
  - "Configured host walk, sprint, jumping and fractional airborne steering"
  - "Exact carried load with movement and stamina modifiers"
  - "Interval-precise stamina and continuous safe health rest"
  - "Atomic direct and quick-slot food consumption with one personal cooldown"
affects: [06-06, 06-07, 06-09, 06-12, 06-13, 06-15]
tech-stack:
  added: []
  patterns: [immutable-survivor-motion, elapsed-interval-intersection, atomic-carried-consumption]
key-files:
  created:
    - src/godot-game/core/RunFood.cs
    - src/godot-game/core/RunLocomotion.cs
    - src/godot-game/core/RunVitals.cs
    - src/godot-game/tests/verification/Phase06LoadTests.cs
    - src/godot-game/tests/verification/Phase06MovementTests.cs
    - src/godot-game/tests/verification/Phase06RecoveryTests.cs
  modified:
    - src/godot-game/core/FoundationHeldIntent.cs
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/KingdomZombies.Core.csproj
    - src/godot-game/core/RunDefinitions.cs
    - src/godot-game/core/RunExternalAccess.cs
    - src/godot-game/core/RunPopulation.cs
    - src/godot-game/core/RunWorld.cs
key-decisions:
  - "The survivor retains decimal horizontal and vertical subunits. The inherited actor projection rounds horizontal position to the nearest subunit."
  - "A fresh jump pays at acceptance. Exhausted or overloaded sprint holds resume only with a fresh input."
  - "Safe rest stores the exact onset of the current interval, including during full health, and calculates recovery only after the deadline."
  - "Food nutrition comes from DT-08 and the player's cooldown comes from DT-03."
requirements-completed: [CHAR-02, CHAR-03, CHAR-04, CHAR-05, CHAR-06, INV-07]
actuals:
  tokens: 24329
  tasks: 2
  commits: 5
commits: 5
plan_head_before: d16ee928ed3884b9408943c0fbe7704fd562aee6
plan_head_after: b7b0f0418c12b0633f38a5ed4ad82e3119e89e13
duration: 25min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 05: Survivor Movement, Load, and Recovery Summary

**The host advances configured walk, sprint and jump on the persistent survivor, derives burden from real carried items, and commits safe rest and one-unit food consumption on the same run clock.**

## Contract for dependent plans

- `FoundationSession.TryAcceptMotion` takes the trusted binding, session and control generation used by the current inventory authority. Held direction and sprint share the legacy movement sequence fence. Opposing directions cancel and a neutral hold clears them. A fresh jump checks current grounded state, actual load and stamina and spends once on acceptance. A blocked jump returns a stable reason. A disconnected survivor retains its physically committed jump and continues to land, with the retired binding's hold discarded.
- The authoritative survivor stores decimal `PositionSubunitsExact` and `HeightSubunits`, a jump start, the last stamina spending instant and the reason for a blocked sprint. The inherited Phase 3 snapshot and native probe use their existing horizontal projection and wire shape. Local access uses current survivor contact geometry. The world tick preserves the inherited fixture travel bounds.
- `RunLocomotion.Load` reads the validated carried graph and sums each equipped or nested record once at its actual quantity. It returns exact kilograms, Normal/Heavy/Overloaded, speed and action cost multipliers. The baseline keeps full movement at 20 kg. At 35 kg it preserves 55% walking speed, 150% action cost and blocks sprint and jump.
- `RunVitals.Spend` supplies the positive-cost action check and safe-rest reset for later combat integration. Sprint pays continuously through its affordable end. When exhaustion or overload stops a hold, walking can continue and stamina can recover, and a fresh input is required to sprint again. Stamina recovery accrues strictly after the configured delay, including outside camp.
- Health qualifies while grounded inside camp, out of the inclusive hostile range and without a spending action that interrupts rest. Walking samples exact camp and hostile crossing intervals, including a tangent contact. The current safe interval begins at `RestStartedAt`. At full health its timestamp remains stable and the idle survivor record retains identity. `RunVitals.RestState` uses the current elapsed clock to provide Unsafe, Resting or Recovering.
- `FoundationSession.ConsumeFood` uses one typed path for the item menu and quick slots. The expected carried item path and, when present, the current slot reference must identify the actual food. One inventory commit spends its unit, clears exhausted references, updates capped personal vitals and starts one shared cooldown. A full-stat use still commits. The latest request replays its original outcome with the current survivor state. Native input, versioned protocol and rendered HUD integration belong to their dependent plans.

## Task commits

| Task | Commit | Change |
| --- | --- | --- |
| 1 RED | `307887c` | Required the ordinary session to use authored fractional walking speed |
| 1 GREEN | `fa7a720` | Integrated configured movement, stamina and carried load |
| 2 RED | `8bebdcb` | Required a combined session-owned food and vitals action |
| 2 GREEN | `1dcaaf3` | Implemented interval safe rest and actual carried food consumption |
| Compatibility fix | `b7b0f04` | Preserved the unchanged survivor record while safely resting at full health |

## Verification

The final checks ran from the phase checkout on clean revision `b7b0f0418c12b0633f38a5ed4ad82e3119e89e13`, Windows x64, Node 24.22.1, .NET SDK 10.0.404 and Godot 4.7.2 .NET. The repository-pinned Node executable was `D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe`.

| Command | Evidence | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build` | Task 1 `ef1c5aed-cfc3-4a02-9b77-17c23e566e27`, Task 2 `bbb8c268-7e66-4ed1-a5db-3b2c67295046`, repair `1365794d-c848-4593-9466-8fd9a9ae7638` | All exited 0. The subsequent direct runs passed 11, 8 and 24 tests respectively with zero failures and skips. The latter included the inherited population class. |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `f90377d3-2c76-49c5-9321-5fd179433992` | Exit 0 with four objective passes in 40.935 seconds and zero failures or blockers. Clean revision and stable sources. Strict build passed with zero errors or warnings, four repository tests passed, 100 pure tests passed with zero skips, and the native headless solo case passed. |
| `dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06MovementTests' '*Phase06LoadTests' '*Phase06RecoveryTests'` | `.local/runs/06-05-tdd/edges-final/tests.json`, CTRF | Exit 0 with exactly one passing method for every E014–E028. Fifteen passes, zero failures or skips. |

The direct final run added `--minimum-expected-tests 15 --report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-05-tdd/edges-final --progress off --no-ansi --show-test-results all`. Run the strict build first to prepare the assembly.

| Edge | Passing method |
| --- | --- |
| E014 | `Phase06_E014_OpposingHoldsSprintAndJumpUseExactBaselines` |
| E015 | `Phase06_E015_FocusClearRacesAndDepartureCannotRepeatTheJump` |
| E016 | `Phase06_E016_ExactStaminaAndRecoveryDelayNeverQueueAPress` |
| E017 | `Phase06_E017_AuthoredFractionalMotionKeepsPartitionPrecision` |
| E018 | `Phase06_E018_SafeRestRequiresAContinuousQualifyingInterval` |
| E019 | `Phase06_E019_CompetingQuickSlotsConsumeOneUnitAndInvalidateBothReferences` |
| E020 | `Phase06_E020_FullVitalsConsumeOneActualUnitAndRejectReplay` |
| E021 | `Phase06_E021_EquivalentWalkPartitionsAndAuthoredFoodKeepExactValues` |
| E022 | `Phase06_E022_AdjacentNestedStacksCountTheirOwnWeights` |
| E023 | `Phase06_E023_ExactSoftAndSevereThresholdsUseTheCurrentLoad` |
| E024 | `Phase06_E024_StepAndDroppingALoadedBagSeeOneOwner` |
| E025 | `Phase06_E025_EmptyAndSingleStorageAreExact` |
| E026 | `Phase06_E026_RejectedCyclesAndRepeatedReadsPreserveMass` |
| E027 | `Phase06_E027_ReversingRecordsKeepsMassAndMotion` |
| E028 | `Phase06_E028_MidpointCostAndPositionRetainFractions` |

Exact measurements included 5,000 horizontal subunits during one second of baseline sprint, a 1,500-subunit apex at 400 ms, 1,800 horizontal subunits during the 800 ms jump, and 0.014 stamina on the first qualifying millisecond after the 500 ms delay. At 27.5 kg the speed and cost multipliers were 0.775 and 1.25. Whole and split walking into camp both produced 51 health from 50 after 6,000 ms, with safe qualification beginning at 500 ms. The exact food cooldown accepted a fresh command at 2,000 ms, after the same item request was rejected at 1,999 ms.

The native solo process ran with PID `31608` and nonce `7fd59182-fea0-4528-9e0f-4690222ce632`. It showed outside camp, danger near and stash blocked at position −8000, elapsed 82 ms and revision 1. It then showed inside camp, danger clear and stash reachable at −6491, elapsed 618 ms and revision 79. The ordinary Quit acknowledgment arrived at frame 151. The process exited 0 with closed streams and no cleanup kill.

### Original failures

- Movement RED build `d1f2a997-d803-4a5b-8bf5-70a34733c2dc` passed. E017's JUnit run exited 2 with one failure and zero skips. The named assertion expected position −3500 after one second using 4.5 u/s and observed −5000 using the inherited 3 u/s. The unchanged JUnit received `RED_EVIDENCE_OK`.
- Food RED build `e909ccfb-33d4-4bbd-9ae6-2d7f68ad4c09` passed. E020's JUnit run exited 2 with one failure and zero skips when the named assertion found no combined session-owned `ConsumeFood` entry point. The unchanged JUnit received `RED_EVIDENCE_OK`. GREEN exercises actual carried units, full vitals, precise cooldown and replay.
- Strict development runs `04065475-0800-4a07-9ae9-beb782902ec6`, `a345bcb0-a9e8-4df0-8e1b-ce3f4aaaffd6`, `a01b83e4-397c-486f-a810-d409e243a652`, `73f2e68c-1334-4fb5-8171-027593b2fbda` and `c5171e88-8e0e-438e-9ecf-b4307ecd001e` caught C# type-style, indentation and test cancellation issues. The successful strict builds and final shared selection include their corrections.
- Shared PR run `ea6b2609-49ca-4504-a6fa-07902b9c88dd` at `1dcaaf3` exited 1 with three of four objectives passing. The pure group ran 100 methods, with 99 passes, zero skips and one failure. E009 observed a new survivor record after an idle step following lease handover. Commit `b7b0f04` stores the safe interval onset instead of updating an accumulated counter each tick. The repaired E009, all 24 focused regression methods and all four shared objectives then passed.

Reproduce either RED from its test commit after a strict build with the direct assembly command and `--filter-method '*<method>' --report-xunit-junit --report-xunit-junit-filename red.xml --results-directory <new-directory> --progress off --no-ansi --show-test-results all`.

## Deviations from plan

- [Rule 2] Task 1 extended the existing held intent, authored definition reader and immutable survivor record to carry the configured motion and timing under the existing session gate. The world and external-access helpers use that authoritative contact while the legacy probe reads its compatible actor projection. Verified in `fa7a720`.
- [Rule 2] The food values referenced by Task 2 live in `food-wildlife.json` (DT-08). The existing Core resource pack and normal guarded reader now validate this authored table. DT-03 remains the source for the player cooldown. Verified in `1dcaaf3`.
- [Rule 1] The shared selection exposed the E009 identity regression. The onset timestamp repair in `b7b0f04` restored that contract and retained exact health timing.

The phase orchestrator owns the shared worktree, roadmap and state updates, the draft pull request and output cleanup. [Draft pull request #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16)

## Self-Check: PASSED
