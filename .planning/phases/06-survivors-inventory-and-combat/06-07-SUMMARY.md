---
phase: 06-survivors-inventory-and-combat
plan: "07"
subsystem: gameplay-authority
tags: [godot, csharp, lifecycle, loot, succession, authority]
requires:
  - phase: 06-04
    provides: "Current external inventory scopes and exact-ownership transfers"
  - phase: 06-06
    provides: "Direct and projectile lethal receipts from actual item combat"
provides:
  - "Atomic lethal ownership, one corpse and control retirement"
  - "One continuous empty-interval cleanup clock for real corpses and piles"
  - "Explicit Respawn admission, replayable random successor selection and ordered waiting"
  - "Current control and living teammate readback for transport and spectator presentation"
affects: [06-08, 06-09, 06-12, 06-13, 06-15]
tech-stack:
  added: []
  patterns: [corpse-slot-roots, authoritative-empty-interval, same-time-respawn-batch]
key-files:
  created:
    - src/godot-game/core/RunDeathAndLoot.cs
    - src/godot-game/core/RunSuccession.cs
    - src/godot-game/tests/verification/Phase06DeathTests.cs
    - src/godot-game/tests/verification/Phase06SuccessionTests.cs
  modified:
    - src/godot-game/core/FoundationParticipantState.cs
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunDefinitions.cs
    - src/godot-game/core/RunExternalAccess.cs
    - src/godot-game/core/RunInventory.cs
    - src/godot-game/core/RunInventoryRules.cs
    - src/godot-game/core/RunItemActions.cs
    - src/godot-game/core/RunItemTransfer.cs
    - src/godot-game/core/RunPopulation.cs
    - src/godot-game/core/RunWorld.cs
key-decisions:
  - "Corpse equipment roots preserve worn slots and keep every nested child record untouched."
  - "A player can explicitly drop to an authorized pile or to a host-resolved pile at their current ground contact."
  - "The host admits one batch of same-timestamp Respawn commands and settles earlier time, then player slot."
  - "A successor gets a new control generation and the same persistent survivor record and inventory."
requirements-completed: [INV-11, INV-12, INV-13, RUN-01, RUN-02, RUN-03, RUN-04]
actuals:
  tokens: 25551
  tasks: 2
  commits: 4
commits: 4
plan_head_before: f7f52abb178b33c2d1345db63334b80c5fc9d108
plan_head_after: 203de8bce50867a309d0a14a4be227dd2c2904f8
duration: 23min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 07: Death, Loot, and Succession Summary

**The host commits lethal damage, one corpse with the victim's real belongings, and retired control together. Explicit Respawn requests can lease an existing survivor or wait in authoritative time and player-slot order.**

## Contract for dependent plans

- Direct attacks and projectile ticks use `RunDeathAndLoot.ApplyDeaths` before publishing the lethal candidate. `RunPublication.Loot.Deaths` records the victim, one stable corpse ID, exact hit time, death contact and any retired lease. Each top-level worn item moves to `RunInventory.CorpseEquipment` with its original slot. Descendants keep their records, locations and counts. The same commit retires ownership, clears invalid quick references and preserves the connection's legacy roster entry.
- `RunPublication.Loot.Targets` tracks real corpse and pile contacts and the nullable `EmptySince` timestamp. The guarded DT-06 reader supplies the empty delay. The inventory commit cancels an interval when a target acquires an item and starts a fresh interval after its last item leaves. Expiry removes the physical anchor and selects no new target. Empty loot from a projectile death uses the fractional impact time as its clock origin. A recorded death keeps its identity after the corpse expires.
- The ordinary item dispatcher takes a typed `Drop(expected, pileId)`. An omitted pile ID asks the authority to create or reuse a pile at the current owner's ground contact. Supplying an ID requires the existing current reachable pile. Either path commits its inventory, anchor and cleanup state together. Corpse selection and transfer continue through the same current access and exact-path checks.
- **06-09 ingress:** Build `RunSuccession.Request(sessionId, bindingId, deathGeneration, sequence)` from the trusted current connection and the proposal's expected death generation and sequence. Submit commands observed together at one host time in a single `FoundationSession.RequestRespawns(ImmutableArray<RunSuccession.Request>)` call. Its replies keep input order. Settlement orders queued time first, then player slot. A singleton request resolves immediately when a candidate is available. A later independent call cannot revise a committed assignment.
- Read `FoundationSession.ReadSuccession(bindingId)` for the current state, exact button deadline, death contact, active survivor and control generation, and living ready teammates in slot order. The host starts waiting only after an enabled request. Replays cannot add a ticket or a second lease. When a guest releases an eligible survivor, the same commit gives the earliest waiting player that real survivor if applicable. With every ordinary survivor dead, the succession view reports defeat.
- Candidate ordering is ordinal survivor ID. The root stores the session-derived replay seed and draw position. One available survivor needs no draw, while multiple candidates use the existing bounded `ReplayRandom`. Taking control changes the lease and legacy pose projection without editing the persistent survivor or inventory. The new generation authorizes typed actions. Because the inherited movement envelope has no epoch, its input stays available for initial leases and rejects after handover until 06-09 routes the generation-bearing command. The public transport shape is preserved.

## Task commits

| Task | Commit | Change |
| --- | --- | --- |
| 1 RED | `b18a43a` | Required a lethal hit to retire actual carried ownership |
| 1 GREEN | `8ac6088` | Integrated atomic corpse ownership, explicit piles and exact cleanup |
| 2 RED | `c4f7bf0` | Required a batchable explicit Respawn authority entry |
| 2 GREEN | `203de8b` | Added request gating, ordered waiting and persistent successor leases |

## Verification

Final evidence ran from the phase checkout on clean revision `203de8bce50867a309d0a14a4be227dd2c2904f8`, Windows x64, Node 24.22.1, .NET SDK 10.0.404 and Godot 4.7.2 .NET. The Node executable was `D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe`.

| Command | Evidence | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build` | Task 1 `ebfa34f9-14bb-4b25-b637-265431d75c7c` and Task 2 `c138824b-d404-41e1-8e5b-2d3f70d1a174` | Both exited 0. The individual class filters passed seven methods each, with zero failures or skips. |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `a2ae295b-a2ad-4d2f-9758-16e73ab88e90` | Exit 0 with four objective passes in 41.560 seconds, zero failures and blockers, and no source change. Strict build, four repository tests, 135 pure tests and the native headless solo case passed. |
| `dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06DeathTests' '*Phase06SuccessionTests'` | `.local/runs/06-07-tdd/edges-final/tests.json`, CTRF | Exit 0 with exactly 14 passing methods and zero failures or skips. Verified one case for every E104–E117. |
| Same test assembly with `--filter-class '*FoundationLifecycleTests' '*FoundationLanOrderingTests' '*Phase06PopulationTests' --minimum-expected-tests 9 --progress off --no-ansi --show-test-results all` | Clean-revision focused regression run | Exit 0 with 11 passes, zero failures or skips. Closed authority and held keys, rejected command ordering, fresh startup, duplicate admission and persistent recruit release passed. |

The exact edge run added `--minimum-expected-tests 14 --report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-07-tdd/edges-final --progress off --no-ansi --show-test-results all`. Run the strict build first to prepare the assembly.

| Edge | Passing method |
| --- | --- |
| E104 | `Phase06_E104_AdjacentActualBelongingsMoveToOneCorpse` |
| E105 | `Phase06_E105_EmptyAndNonemptyCorpsesHaveDifferentLifetimes` |
| E106 | `Phase06_E106_LastCorpseTransferCommitsOnce` |
| E107 | `Phase06_E107_ExplicitDropCreatesARealPileWithActualRoots` |
| E108 | `Phase06_E108_ExpiredCorpseDoesNotReappearFromItsLethalReceipt` |
| E109 | `Phase06_E109_RefillingAndReEmptyingRespectTheHostOrder` |
| E110 | `Phase06_E110_FractionalProjectileDeathAndExpiryUseOnePublication` |
| E111 | `Phase06_E111_RespawnChoosesOneRealUncontestedSurvivor` |
| E112 | `Phase06_E112_RespawnOpensAtExactlyOneSecondAndRequiresInput` |
| E113 | `Phase06_E113_SameTimeBatchUsesPlayerSlotTieBreak` |
| E114 | `Phase06_E114_ZeroCandidatesAndNoLivingOrdinarySurvivorsAreDistinct` |
| E115 | `Phase06_E115_RespawnReplayKeepsOneQueueEntryAndOneLease` |
| E116 | `Phase06_E116_EarlierWaiterReceivesTheReleasedPersistentState` |
| E117 | `Phase06_E117_FractionalDeathTimeAndControlEpochRemainExact` |

The projectile death proof recorded 90.02 ms and one new run revision. The empty corpse was present at 180,090 elapsed milliseconds and gone at 180,091. A separate controlled projectile death had a Respawn deadline of 1,090.02 ms. The first available host millisecond was 1,091. The refill test also took an item at the old deadline after a deposit one millisecond before it, then required another full 180,000 ms of emptiness.

The native headless solo process was PID `17572`, nonce `15590edb-39ae-4844-ae1f-c90e34cf158b`. The Hint changed from outside camp, danger near and stash blocked at −8000, elapsed 87 ms and revision 1, to inside camp, danger clear and stash reachable at −6491, elapsed 619 ms and revision 78. Ordinary Quit was acknowledged at frame 149. The process exited 0 with closed streams and no cleanup kill. The focused Core tests supply lethal and Respawn evidence.

### Original failures

- Task 1 RED strict build `85b80974-749c-41a5-b1bd-8ff845e7a8ae` passed. The E104 JUnit run exited 2 with one failure and zero skips. The real lethal hit left three carried outfit roots on the dead survivor, and the named ownership assertion required an empty carried graph. The classifier returned `RED_EVIDENCE_OK`.
- Task 2 RED strict build `dca29faa-378e-49c6-b6db-94eb75bed462` passed. The E112 JUnit run exited 2 with one failure and zero skips. The assertion found no `RequestRespawns` method. The classifier returned `RED_EVIDENCE_OK`. The GREEN case proves the button boundary, fresh request and old-control rejection through production calls.
- Development strict builds `2b0d656d-9888-41cf-a610-f40eea005891` and `65a1cfec-a3c1-49b3-abfb-c3c82d0bf358` caught type-style, continuation indentation and collection initialization issues. Both task builds and the final selection passed after the corrections.

Reproduce either RED from its test commit after the strict build with the direct test assembly and `--filter-method '*<method>' --report-xunit-junit --report-xunit-junit-filename red.xml --results-directory <new-directory> --progress off --no-ansi --show-test-results all`.

## Deviations from plan

- [Rule 2] Task 1 carried corpse slots through the existing inventory placement and validation helpers, and allowed an explicit drop to create a host-resolved pile through the same edit gate. `RunWorld` supplies the immutable anchor replacement and `RunDefinitions` reads DT-06's cleanup interval. This preserves authoritative scope and items through the same publication. Verified in `8ac6088`.
- [Rule 3] Task 1 removed the assumption that a previously living host lease always exists during reservation. A dead player remains connected while the old lease is absent. Task 2 extended the existing definition reader to use DT-01's Respawn delay and kept the current roster wire shape while updating the persistent control generation. Verified in `8ac6088` and `203de8b`.

The phase orchestrator owns the shared worktree, roadmap and state updates, draft pull request, and output cleanup. [Draft pull request #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16)

## Self-Check: PASSED
