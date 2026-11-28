---
phase: 06-survivors-inventory-and-combat
plan: "03"
subsystem: gameplay-authority
tags: [godot, csharp, inventory, equipment, containment, quick-food]
requires:
  - phase: 06-02
    provides: "Persistent run items, survivor identities, authored definitions and starting stash"
  - phase: 06-01
    provides: "One immutable session publication and serialized authority"
provides:
  - "Exact equipment, grid, split and merge transactions over the existing run inventory"
  - "Acyclic filled-container transfers and drops with stable ancestry, ownership and cell results"
  - "Four carried food references per survivor and a single-stock decrement for the food action"
affects: [06-04, 06-05, 06-06, 06-07, 06-09, 06-11, 06-13]
tech-stack:
  added: []
  patterns: [immutable-publication, expected-source-path, bounded-containment]
key-files:
  created:
    - src/godot-game/core/RunInventoryRules.cs
    - src/godot-game/core/RunItemTransfer.cs
    - src/godot-game/core/RunQuickReferences.cs
    - src/godot-game/tests/verification/Phase06InventoryTests.cs
    - src/godot-game/tests/verification/Phase06NestingTests.cs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunDefinitions.cs
    - src/godot-game/core/RunInventory.cs
key-decisions:
  - "An edit checks the complete current source path and submits one detached candidate for validation under the session lock."
  - "A container owns its grid by item identity. Grid bounds and footprints are stored as integral run state."
  - "Quick slots are one-based references keyed by persistent survivor identity. The same root commit removes references that have lost their exact carried food."
requirements-completed: [INV-01, INV-02, INV-03, INV-04, INV-05, INV-06, INV-07]
actuals:
  tokens: 24540
  tasks: 2
  commits: 4
commits: 4
plan_head_before: 9cd26975d8a3c9b114b2f27714fe7b4058b547ad
plan_head_after: 0a78ccc2554af1ee80cc21b914f49d2469845023
duration: 23min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 03: Inventory Transactions and Container Trees Summary

**The run now commits legal moves, rotations, splits and merges over real equipment and pocket grids. Filled items keep their entire subtree, and four quick food slots follow the exact carried stack through the same publication.**

## Contract for dependent plans

- `FoundationSession.TryEditInventory` runs a trusted Core edit against the current inventory, definitions and population under the session lock. The candidate publishes with a new revision after identity, slot, fit, condition and containment checks. Rejection keeps the same run publication and allocation counter. A successful item change leaves elapsed time unchanged.
- `RunInventory.PathTo` returns immutable, leaf-first item records. Its final location identifies the actual survivor, stash page or ground pile, and `CarriedBy` uses the same traversal. Capture the expected path when preparing a request. Move, split, merge and removal compare it with current state before proposing a change, including an ancestor that moved while the child's coordinates stayed in place. A binding or presentation identifier cannot be an equipment owner.
- `RunItemTransfer.Stash` and `Container` construct exact footprint and rotation proposals. Grid indices and cells are zero-based internally. `RunInventory.StashGrid`, the item's `StorageGrid` and placed `GridCell.Footprint` carry current saved geometry. Equipment checks all eleven logical slots, including the bow's opposite-hand restriction. A full slot requires a separate unequip. The reader retains the 24 definitions' icon and fit references alongside their geometry and condition.
- `RunItemTransfer.Split` accepts an integral amount from one through source quantity minus one and creates one fresh ID on commit. The original ID stays at its source with its remainder. Merge checks both expected paths, matching definition and condition, keeps the target ID, and fills only its remaining capacity. Fully merged IDs cannot be reintroduced by a later candidate.
- Container depth counts storage-bearing items from level one. The authored default is three and the reader accepts positive integral alternatives. The candidate validates cycles, missing or non-storage parents, duplicate identities, cell collisions and every descendant's final depth. `Drop` is an explicit graph action taking a pile identity. The access action will supply and validate the actual world pile and its reachability.
- The commit overload returns `RunItemTransfer.Change` with immutable `Before` and `After` roots for ownership queries and deduplicated `ChangedCells()` in grid, page or container, row and column order. Child grids keep their cells when their ancestor moves. A prepared candidate does not publish or alter its source.
- `RunQuickReferences.Assign`, `Resolve` and `SpendCarriedFood` share the current carried-path check. Slots 1–4 can share one food ID and the same supply. Partial splits, in-survivor moves and merges with a live remainder preserve references. Exhaustion, retirement and removal from the survivor clear only the affected references in the inventory commit. Plan 06-04 supplies current binding, access, replay metadata and stash actions. Plan 06-05 composes the carried graph and single-unit food decrement with survivor vitals and cooldown.

## Task commits

| Task | Commit | Change |
| --- | --- | --- |
| 1 RED | `4d75eff` | Required a legal grid placement to commit through the current session |
| 1 GREEN | `3b4b615` | Committed exact equipment, grid and stack transactions |
| 2 RED | `58e148c` | Required an authored two-level nesting limit to load |
| 2 GREEN | `0a78ccc` | Preserved complete container trees and carried food references |

## Verification

The closing checks used clean revision `0a78ccc2554af1ee80cc21b914f49d2469845023` on development Windows x64 with Node 24.22.1, .NET SDK 10.0.404 and Godot 4.7.2 .NET. Run from the phase checkout root with the repository-pinned Node. The strict build in `--pr` prepares the assembly before the direct class filters.

| Command | Evidence | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build` | `06e7d929-7615-4d6b-898a-bce71352eccc`, Task 1; `fc269f7e-4790-4d48-a7cf-672ff9030ce1`, Task 2 | Both exited 0. Task 1 then ran 21 focused passes. Task 2 then ran 35 focused passes. Both had zero failures or skips. |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `334036a3-ebb3-4940-9120-837b84237cbf` | Exit 0 with four objective passes, zero failures or blockers and an unchanged source revision. Strict build passed with zero errors or warnings, four repository checks passed, 68 pure tests passed with zero skips, and the native headless solo case passed. |
| `dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06InventoryTests' '*Phase06NestingTests'` | `.local/runs/06-03-tdd/edges-final/tests.json`, CTRF | Exit 0. The report contained exactly 35 passing methods, one for each edge E029–E063, with zero failures or skips. |

The direct run added `--minimum-expected-tests 35 --report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-03-tdd/edges-final --progress off --no-ansi --show-test-results all`. The closing census checked the passing results against the complete integer sequence 29 through 63.

| Edge | Passing method in `Phase06InventoryTests` |
| --- | --- |
| E029 | `Phase06_E029_AdjacentInstancesHaveIndependentLocations` |
| E030 | `Phase06_E030_AllAuthoredDefinitionsRejectInvalidExtrema` |
| E031 | `Phase06_E031_OneInstanceWinsOneConcurrentMove` |
| E032 | `Phase06_E032_EmptySlotsHaveNoInventoryItems` |
| E033 | `Phase06_E033_ReplayingAnEquipKeepsThePublication` |
| E034 | `Phase06_E034_ReorderedItemsRetainTheSameOwnersAndWeight` |
| E035 | `Phase06_E035_ConditionAndAuthoredWeightKeepExactPrecision` |
| E036 | `Phase06_E036_EdgeContactAcceptsAndOneCellOverlapRejects` |
| E037 | `Phase06_E037_ExactFitAndOverflowUseTheCurrentRun` |
| E038 | `Phase06_E038_ContestedEmptyEquipmentSlotAcceptsOneIdentity` |
| E039 | `Phase06_E039_EmptySlotsDoNotCreateExtraOwners` |
| E040 | `Phase06_E040_RotationCycleNeverAllocatesAnIdentity` |
| E041 | `Phase06_E041_ManualUnequipPrecedesReplacementAndBowPairing` |
| E042 | `Phase06_E042_InvalidGridGeometryCannotWrapOrShrink` |
| E043 | `Phase06_E043_SplitKeepsTheSourceAndAddsOneAdjacentStack` |
| E044 | `Phase06_E044_SplitAndPartialMergeConserveTheExactBoundary` |
| E045 | `Phase06_E045_CompetingMergesShareTheOneDestinationCapacity` |
| E046 | `Phase06_E046_FullMissingAndSelfTargetsRejectUnchanged` |
| E047 | `Phase06_E047_MergeRetiresTheExactSourceAndRejectsReplay` |
| E048 | `Phase06_E048_StackMergeHonorsIdentitiesAfterEnumerationChanges` |
| E049 | `Phase06_E049_SplitRejectsNonIntegralAndInvalidAmountsAtomically` |

| Edge | Passing method in `Phase06NestingTests` |
| --- | --- |
| E050 | `Phase06_E050_AdjacentPocketsUseTheirActualContainerIdentity` |
| E051 | `Phase06_E051_FilledTreeCannotCrossTheDefaultDepthLimit` |
| E052 | `Phase06_E052_ContestedAncestorPreservesTheSerializedChildOutcome` |
| E053 | `Phase06_E053_EmptyStorageAndMissingStorageAreDistinct` |
| E054 | `Phase06_E054_ExplicitDropKeepsAllNestedRecordsAndRejectsReplay` |
| E055 | `Phase06_E055_OnlyTheMovedEqualLookingBagTakesItsDescendants` |
| E056 | `Phase06_E056_ConfiguredDepthAndQuantitiesStayIntegral` |
| E057 | `Phase06_E057_MovingAndSplittingKeepsTheOriginalReference` |
| E058 | `Phase06_E058_ExactlyFourQuickSlotsCanReferenceOneRealStack` |
| E059 | `Phase06_E059_SharedShortcutsCompeteForOnlyOneActualSupply` |
| E060 | `Phase06_E060_EmptyAndNonCarriedSelectionsHaveNoEffect` |
| E061 | `Phase06_E061_ConsumedIdentityCannotBeReplayedOrSubstituted` |
| E062 | `Phase06_E062_TransferAwayClearsOnlyThatSurvivorsExactReferences` |
| E063 | `Phase06_E063_ReferenceLifetimeFollowsExactMergeQuantities` |

The headless solo process was PID `17504` with nonce `f4756c4f-5877-41ca-8567-3eb39c45dab2`. The visible Hint read outside camp, danger near and stash blocked at position −8000, elapsed 73 ms and revision 1. It then read inside camp, danger clear and stash reachable at −6497, elapsed 618 ms and revision 80. The process exercised a fresh ordinary Start and acknowledged ordinary Quit at frame 150. It exited 0 with closed streams and no cleanup kill. The pure authority tests supply the inventory transaction evidence. The grouped rendered acceptance belongs to the later phase acceptance plan.

## TDD and deviations

- Task 1 RED strict build `a8e45e2b-adfe-4dd0-93c9-19f82ba2781e` succeeded. The direct E037 JUnit run exited 2 with one failure and zero skips. The existing session lacked the internal committing method, so Shouldly saw a null method at the expected assertion. The GREEN proof executes legal and invalid placements through the actual session. An initial reporter invocation used the unregistered `--report-junit` option and exited 5 before discovery. The installed `--report-xunit-junit` option produced the genuine failure. The classifier initially rejected the simple target identity and then returned `RED_EVIDENCE_OK` when supplied the fully qualified JUnit class and displayed name from the unchanged report.
- Task 2 RED strict build `d09b6c5c-230d-46fe-8453-35d9557f8999` succeeded. The direct E056 JUnit run exited 2 with one failure and zero skips. The intended success assertion reported `RUN_DEFINITION_INVALID content/config/inventory.json: maximumLevels` for the configured limit two. The exact JUnit result received `RED_EVIDENCE_OK`. The GREEN proof loads that configuration and applies it to live depth, cycle and duplicate-ownership cases. Both RED commands can be reproduced at their test commits with the direct assembly call and `--filter-method '*<method>' --report-xunit-junit --report-xunit-junit-filename red.xml --results-directory <new-directory> --progress off --no-ansi`.
- [Rule 2] The plan's file list omitted `FoundationSession.cs`. Both task commits include the narrow publication change necessary to validate and publish through the existing authority. The orchestrator approved the file addition. All new implementation types and methods remain internal to Core.
- [Rule 3] Task 1 strict invocations `d169ed79-0302-4660-819b-34eefbb0abcb` and `1bbc0501-4c74-4e19-b547-06ae512077b9` failed style and test compilation checks. Task 2 strict invocations `466d81d6-f422-4232-ae2a-92cfeaac5ed9`, `e7c64a7f-78ac-48f4-beb3-32754f3011f0` and `32ee7121-a57b-4148-8ff3-b6d9f2a0cd86` failed source-rule checks for constructor visibility and order, collection initialization, explicit types, and line lengths. The listed GREEN builds and clean closing check passed after repair.

## Handoff

Implementation ran from 11:43 to 12:06 UTC on 2026-11-24. The phase orchestrator owns the shared worktree, continuity metadata, draft pull request update and output cleanup. [Draft pull request #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16)

## Self-Check: PASSED

All eight implementation and test files exist. All four task commits are reachable. The clean PR report records the tested revision, and the focused report contains one passing result for every expected edge.
