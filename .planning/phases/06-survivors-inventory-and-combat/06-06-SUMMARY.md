---
phase: 06-survivors-inventory-and-combat
plan: "06"
subsystem: gameplay-authority
tags: [godot, csharp, combat, melee, bow, armor, projectiles]
requires:
  - phase: 06-04
    provides: "Current control and the exact inventory publication gate"
  - phase: 06-05
    provides: "Persistent survivor pose, stamina spending and one elapsed clock"
provides:
  - "One authoritative melee or bow action with exact stamina and cooldown"
  - "Per-hit armor reduction and condition updates on actual equipped gear"
  - "Swept arrow first contact, factions and durable defense damage"
  - "Lethal hit receipts for lifecycle ownership"
affects: [06-07, 06-08, 06-09, 06-10, 06-12, 06-13, 06-15]
tech-stack:
  added: []
  patterns: [current-control-attack, exact-gear-mutation, ordered-swept-contact]
key-files:
  created:
    - src/godot-game/core/RunCombat.cs
    - src/godot-game/core/RunCombatDefinitions.cs
    - src/godot-game/core/RunDamage.cs
    - src/godot-game/core/RunProjectiles.cs
    - src/godot-game/tests/verification/Phase06MeleeTests.cs
    - src/godot-game/tests/verification/Phase06BowTests.cs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunDefinitions.cs
    - src/godot-game/core/RunExternalAccess.cs
    - src/godot-game/core/RunPopulation.cs
    - src/godot-game/core/RunVitals.cs
    - src/godot-game/core/RunWorld.cs
    - src/godot-game/core/RunWorldGeometry.cs
key-decisions:
  - "Melee orders eligible opposing targets by distance then ordinal identity, with left hand before right hand."
  - "Arrow contacts order by elapsed time, arrow identity, terrain/defense/survivor precedence and ordinal target identity."
  - "Lethal hits append an explicit receipt and leave the existing item records available for the lifecycle handoff."
  - "Bow readback carries the current aim and actual opposite-hand arrow count, including zero."
requirements-completed: [COMBAT-01, COMBAT-02, COMBAT-03, COMBAT-05, COMBAT-06]
actuals:
  tokens: 26591
  tasks: 2
  commits: 4
commits: 4
plan_head_before: f93702cf3c576f7dfb5e695828c45d089a53742f
plan_head_after: 52567539c8395134c667a65d39c73bb6b0e09568
duration: 24min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 06: Authoritative Combat Summary

**The host resolves configured melee, per-hit armor and spent arrows using real held gear, the current survivor publication and an ordered swept first-contact query.**

## Contract for dependent plans

- `FoundationSession.Attack` validates the trusted current controller and gates the sequence against that survivor's control generation. `AttackUncontrolled` is an internal authority entry for an ordinary NPC or hostile with no control lease. An accepted action spends once through `RunVitals.Spend` and stores one cooldown. A duplicate returns the previous status, and a rejected press has no later effect. The existing movement transport continues to use its current schema.
- Fists require a free hand unless a usable melee weapon already supplies the attack. A usable weapon can swing beside broken gear, and two usable hands charge their combined cost and slower interval. Each hand sees current health and condition and can strike one opposing target. Misses leave item condition intact.
- `RunDamage.Apply` uses each armor piece's condition at hit start, applies the configured multiplicative reduction and cap, and wears protection from the same raw hit. Broken gear remains in its actual slot with its original identity. A lethal hit reaches zero health and appends one `RunDamage.Hit` to `RunPublication.Combat.FatalHits`. Lifecycle owns the next inventory and control transition.
- Bow release requires an actual usable bow, a real opposite-hand arrow, sufficient current stamina and bounded finite world aim. It removes one arrow and one configured bow condition when it publishes the shot. `ReadBow` carries the supplied valid aim and actual ready-hand count even when that count is zero. Empty shots return `EmptyAmmo` for quiet presentation. Carried reserve stacks remain available for explicit inventory transfer.
- `RunProjectiles` stores the launch origin, velocity, aim, lifetime, source and faction. It sweeps the clipped shot-time interval during `Step`, using linear interpolation between the committed poses for moving survivor colliders. The representative survivor collider extends 250 subunits either side of the contact and 1,000 above it. The defense collider extends 250 either side and 2,500 above. Defense fixtures may specify health, with a 100-health default. Terrain wins an equal-time tie ahead of defense and survivor contacts. Positive elapsed steps publish the latest impacts.
- Allied arrows pass friendly survivors and camp defenses. Hostile arrows can strike a living completed defense, and a later arrow can pass once it has broken. Terrain and the earliest eligible hit consume the projectile. The arrow remains spent through expiry or shooter death. The test fixture uses real hostile survivors and equipped items through the same attack and tick paths.

## Task commits

| Task | Commit | Change |
| --- | --- | --- |
| 1 RED | `57bfbe5` | Required an active session attack boundary |
| 1 GREEN | `7d90959` | Integrated configured melee, armor wear and both P01 controls |
| 2 RED | `aa86a06` | Required the held empty bow's quiet outcome |
| 2 GREEN | `5256753` | Released actual arrows and committed swept first contact |

## Verification

The final checks used clean revision `52567539c8395134c667a65d39c73bb6b0e09568` on Windows x64 with Node 24.22.1, .NET SDK 10.0.404 and Godot 4.7.2 .NET. Run from the phase checkout with the repository-pinned `D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe`.

| Command | Evidence | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build` | Task 1 `6fa13e18-5120-4f6a-8b1b-8962b361053f` and Task 2 `5668750a-bc06-43bd-878b-1fee9f91d21e` | Both exited 0. The melee and bow class filters passed 14 and 7 methods respectively with zero failures or skips. |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `33ead326-c0d2-4fd1-a37d-c8d52896f109` | Exit 0 with four objective passes in 39.870 seconds, zero failures or blockers, and the same clean source revision. Strict build had zero warnings or errors. Four repository tests, 121 pure tests and native headless solo passed. |
| `dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06MeleeTests' '*Phase06BowTests'` | `.local/runs/06-06-tdd/edges-final/tests.json`, CTRF | Exit 0 with exactly 21 passed methods, zero failures and zero skips. The complete E080–E098 sequence and both P01 controls matched the report. |

The direct final run added `--minimum-expected-tests 21 --report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-06-tdd/edges-final --progress off --no-ansi --show-test-results all`. Run the strict build first to prepare the assembly.

| Edge or control | Passing method |
| --- | --- |
| E080 | `Phase06_E080_DualMeleeCanHitOneAdjacentTargetTwice` |
| E081 | `Phase06_E081_ReachFacingAndExactlyEnoughStaminaAreInclusive` |
| E082 | `Phase06_E082_ConcurrentMeleeRequestsCommitOneSwing` |
| E083 | `Phase06_E083_MissesAndBrokenHandsPreserveHeldGear` |
| E084 | `Phase06_E084_CooldownReplaysDoNotQueueAtTheBoundary` |
| E085 | `Phase06_E085_EqualMeleeContactsHaveAStableWinner` |
| E086 | `Phase06_E086_FractionalReachDoesNotRoundIntoAHit` |
| E087 | `Phase06_E087_ArrowPassesAlliesAndStopsAtTheFirstOverlappingEnemy` |
| E088 | `Phase06_E088_QuietEmptyAndExactBowBoundariesUseTheOppositeHand` |
| E089 | `Phase06_E089_ConcurrentBowRequestsCommitOneRelease` |
| E090 | `Phase06_E090_AnExpiredArrowNeverCreatesLootOrRefillsTheHand` |
| E091 | `Phase06_E091_APreviouslyFiredArrowSurvivesShooterDeath` |
| E092 | `Phase06_E092_HostileDefenseTerrainAndSurvivorContactsHaveStableOrder` |
| E093 | `Phase06_E093_AimBoundsAndFractionalSweepStayPrecise` |
| E094 | `Phase06_E094_ExactArmorReductionAndRawWear` |
| E095 | `Phase06_E095_ConcurrentHitsReadTheNewArmorCondition` |
| E096 | `Phase06_E096_DeathAndWearCannotBeReplayed` |
| E097 | `Phase06_E097_DualHitsApplyFullProtectionThenNoProtection` |
| E098 | `Phase06_E098_ZeroAndFractionalRawHitsKeepExactCondition` |
| P01 clean | `P01_CleanControl_PreservesAlliesAndCosmeticCrown` |
| P01 violation | `P01_ViolationControl_DetectsAllyDamageOrCrownBenefit` |

The exact armor proof measured 17.28 health damage from raw 32 through usable plated armor and a helmet, with 6.4 and 3.2 condition wear. A second hit after both pieces broke dealt 32. The friendly bow struck its first hostile at 110 ms. The hostile bow struck the blocking defense at 134 ms, then reached a survivor after the defense broke. The lifetime proof included contact at exactly 2,000 ms, while a missed arrow expired at the same boundary with its ammunition spent. Appearance edges E099–E103 and lifecycle edge E104 have their assigned later plans.

The shared native headless solo ran with PID `29440` and nonce `31016d46-0fad-4ea6-b4ae-6d511ee0cd1c`. The Hint changed from outside camp, danger near and stash blocked at −8000, elapsed 84 ms and revision 1, to inside camp, danger clear and stash reachable at −6488, elapsed 615 ms and revision 78. Ordinary Quit was acknowledged at frame 150. The process exited 0 with closed streams and no cleanup kill. The focused Core tests supply combat evidence.

### Original failures

- Task 1 RED strict build `d1704c6b-fcc0-4bc1-bc94-13a93a1b8785` passed. E080's JUnit run exited 2 with one failure and zero skips. The named assertion found no session-owned attack method. The classifier validated the unchanged report as `RED_EVIDENCE_OK`. The GREEN method checks dual hits, combined cost and timing, and both real item conditions.
- Task 2 RED strict build `dfa20edf-df72-4fa9-96e4-74b1a0ec323d` passed. E088's JUnit run exited 2 with one failure and zero skips. The real held-bow attack returned `IncompatibleHands` where the named assertion expected `EmptyAmmo`. The classifier returned `RED_EVIDENCE_OK`.
- Task 1 development strict runs `d5b318ed-5ce7-4ddf-bd43-ce37f8b93365`, `f07e7bca-1aeb-485a-90bb-fd9268fab682`, `0b1d1607-c621-4c80-acc4-8952e892334e` and `62d180ff-99dc-4ae1-9b2d-926b4686814c` caught an immutable set argument name and source formatting. Task 2 run `b3b4619a-cef5-4a62-9b8f-8702861c571f` caught one defense validation indentation rule. Both focused builds and the final shared PR passed after correction.

To reproduce the intentional failures, check out the corresponding RED commit, run the strict build, and run the direct test assembly with `--filter-method '*<method>' --report-xunit-junit --report-xunit-junit-filename red.xml --results-directory <new-directory> --progress off --no-ansi --show-test-results all`.

## Deviations from plan

- [Rule 2] The existing `RunDefinitions` guarded reader validates the authored DT-05 values before constructing `RunCombatDefinitions`. `RunPopulation` carries faction and attack readiness, `RunExternalAccess` excludes hostile recruits from ordinary inventory access, and `RunVitals` excludes those hostiles from ordinary safe health rest. `RunWorld` tracks live hostile contacts and actual defense health for the combat updates. The narrow integration uses the existing immutable root and gate. Verified in `7d90959` and `5256753`.

The phase orchestrator owns the shared worktree, roadmap and state updates, draft pull request, and output cleanup. [Draft pull request #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16)

## Self-Check: PASSED
