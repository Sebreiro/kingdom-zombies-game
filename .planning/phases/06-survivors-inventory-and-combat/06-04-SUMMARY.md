---
phase: 06-survivors-inventory-and-combat
plan: "04"
subsystem: gameplay-authority
tags: [godot, csharp, inventory, access, stash]
requires:
  - phase: 06-01
    provides: "One authoritative clock and the exact world range and blocker query"
  - phase: 06-03
    provides: "Immutable item paths, containment, placement and the session inventory edit gate"
provides:
  - "Scoped nearby inventory listing, selection and correction under current control"
  - "Sequenced inventory transactions and transactional quick transfer"
  - "Unfiltered stash pages and deterministic stack-first placement shared with fresh runs"
affects: [06-05, 06-07, 06-09, 06-11, 06-13, 06-15]
tech-stack:
  added: []
  patterns: [current-publication, recipient-scoped-correction, deterministic-stash-placement]
key-files:
  created:
    - src/godot-game/core/RunExternalAccess.cs
    - src/godot-game/core/RunItemActions.cs
    - src/godot-game/core/RunStash.cs
    - src/godot-game/tests/verification/Phase06AccessTests.cs
    - src/godot-game/tests/verification/Phase06StashTests.cs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunInventory.cs
    - src/godot-game/core/RunInventoryRules.cs
    - src/godot-game/core/RunItemTransfer.cs
    - src/godot-game/core/RunPopulation.cs
    - src/godot-game/core/RunWorld.cs
key-decisions:
  - "Inventory reads, scope changes, sequence decisions and commits share the session lock."
  - "A request can use its own inventory and the one currently selected reachable external scope. A changed selection or browse page gets its own sequence token."
  - "Automatic placement breaks ties by earliest page, then row and column, then unrotated orientation at the same cell."
  - "Quick withdrawal places the entire source across actual carried stacks and storage or returns capacity with the source intact."
requirements-completed: [INV-08, INV-09, INV-10, INV-11]
actuals:
  tokens: 24264
  tasks: 2
  commits: 4
commits: 4
plan_head_before: ee677f80c924d9c12235ab2dac9aa345733d9737
plan_head_after: 740bf23cc335f0acb5c325bbde686e0d4b858fd9
duration: 22min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 04: Nearby Inventory Access and Paged Stash Summary

**The host checks both ends of every item transfer against current control and reachability. The shared stash browses real pages, merges compatible stacks before placing remainders, and allocates a page only within a successful transfer.**

## Contract for dependent plans

- `FoundationSession.ItemActions` owns `Read` and `Submit`. Supply the trusted binding, current session ID and control generation as a `RunItemActions.Controller`. The session lock covers the current participant, population, world and inventory publication. The existing `TryEditInventory` validates and publishes a detached candidate with one revision and zero elapsed time.
- `Read` returns the recipient's personal items and quick references, permitted target identities, and the selected target's contents when accessible. The stash view contains one selected page and its descendants. An inaccessible selection clears and preserves an access-loss reason for the current nearby list. An invalid controller receives no view. A reserved survivor becomes private immediately, including before readiness.
- Select and browse return an opaque selection token derived from the accepted sequence. External source and destination roots must match the token and current scope. Container destinations also check the supplied exact ancestry. The nearest range decisions use live NPC poses and world anchors with the same inclusive range and blocker geometry. Explicit Drop checks a real reachable pile and the controller's actual carried item. Later lifecycle work supplies live corpse and pile creation and expiry.
- Each current binding and control generation has a positive monotonic action sequence. The latest sequence repeats its original status with a fresh permitted correction. Older sequences receive a stale-request result with the current correction. Rejected and replayed requests preserve the publication, allocation and clock.
- `RunStash` merges matching top-level stash stacks in page, row, column and item order before scanning cells. At each cell it chooses the first legal orientation, preferring unrotated when both fit. A full stash grows one page for the remainder on commit. The existing fresh-run factory uses the same placer for each configured stack, preserving the authored partition, condition and identities.
- Browse accepts one-based integral page numbers and returns current page and total. Quick withdrawal uses existing carried pocket and bag stacks before fitting the remainder in storage. A capacity rejection discards its detached candidate, including any possible partial merges. The depositor receives the first affected page, its ordered changed-cell coordinates and the count of additional affected pages. An automatic deposit follows the first affected page when it differs from the displayed one.

## Task commits

| Task | Commit | Change |
| --- | --- | --- |
| 1 RED | `caa5864` | Required an ordinary session-owned item action dispatcher |
| 1 GREEN | `8a36499` | Committed current local transfer authority and recipient corrections |
| 2 RED | `f40ccda` | Required the first legal page to win during fresh setup |
| 2 GREEN | `740bf23` | Shared the stash placer and committed real page browsing and quick transfers |

## Verification

The final checks ran on clean revision `740bf23cc335f0acb5c325bbde686e0d4b858fd9` on development Windows x64 with Node 24.22.1, .NET SDK 10.0.404 and Godot 4.7.2 .NET. The repository-pinned Node executable was `D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe`. Run from the phase checkout root.

| Command | Evidence | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build` | Task 1 `a0591cb6-2f83-4f6a-952e-25947b50908b` and Task 2 `afe3ee0e-8e66-405c-85c9-8b887a66ca6e` | Both exited 0. The subsequent direct filters passed 8 and 9 methods respectively, with zero failures or skips. |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `319267b8-923b-46f2-8ec2-64dcd9890d5b` | Exit 0. Four objective passes in 39.410 seconds with zero failures or blockers and a clean, stable revision. The strict build had zero errors and warnings. Four repository tests, 85 pure tests and the native headless solo case passed. |
| `dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06AccessTests' '*Phase06StashTests'` | `.local/runs/06-04-tdd/edges-final/tests.json`, CTRF | Exit 0. Exactly 17 passing methods, zero failures or skips. Validated one case per edge E064–E073 and E075–E079, plus two focused withdrawal regressions. |

The direct run added `--minimum-expected-tests 17 --report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-04-tdd/edges-final --progress off --no-ansi --show-test-results all`. Run the strict build first to prepare the assembly.

| Edge | Passing method |
| --- | --- |
| E064 | `Phase06_E064_ListUsesTheInclusiveWorldRanges` |
| E065 | `Phase06_E065_TakeoverRevokesANearbyInventory` |
| E066 | `Phase06_E066_NoReachableTargetsPreservesThePublication` |
| E067 | `Phase06_E067_CancellingAndImplicitGroundPlacementNeverDropAnItem` |
| E068 | `Phase06_E068_OnlyTheLatestSelectedOverlappingTargetIsVisible` |
| E069 | `Phase06_E069_ReachAndSequenceDoNotRoundOrWrap` |
| E070 | `Phase06_E070_StartupUsesTheEarliestFittingOrientation` |
| E071 | `Phase06_E071_FullExistingPagesCreateOnePageOnCommit` |
| E072 | `Phase06_E072_CompetingDepositsShareOneNewPage` |
| E073 | `Phase06_E073_EmptyStashAndMissingPagesAreDistinct` |
| E075 | `Phase06_E075_QuickWithdrawalUsesPocketsAndCannotReplay` |
| E076 | `Phase06_E076_CompatibleStacksWinBeforeFreeCellsAndUseStablePageOrder` |
| E077 | `Phase06_E077_PageAndQuantityBoundsDoNotWrapOrTruncate` |
| E078 | `Phase06_E078_CompetingControllersCommitTheItemExactlyOnce` |
| E079 | `Phase06_E079_ReplayedAndRejectedSequencesCannotCommitAnotherItem` |

`QuickWithdrawalOfTheLastStashItemPublishesOneOwner` and `QuickWithdrawalWithNoRoomRejectsWithoutAPartialMerge` also passed. Phase 12 INV-14 owns E074.

The headless case ran with PID `1368` and nonce `45a81ca0-86e3-4c78-979a-c0f9dc501e10`. The visible Hint changed from outside camp, danger near and stash blocked at position −8000, elapsed 81 ms and revision 1, to inside camp, danger clear and stash reachable at −6482, elapsed 622 ms and revision 80. A fresh ordinary Start ran, and ordinary Quit was acknowledged at frame 149. The process exited 0 with closed streams and no cleanup kill. The direct Core tests provide the item transaction evidence. The native inventory UI and LAN publication proofs are scheduled in their owning plans.

### Original failures

- Task 1 RED strict build `b8735264-5913-410d-a3db-bce95cd76fd5` passed. The E064 JUnit run exited 2 with one failure and zero skips. The named assertion found no session-owned dispatcher. The classifier returned `RED_EVIDENCE_OK`. The GREEN test opens the real dispatcher and verifies inclusive ranges and distinct overlapping targets.
- Task 2 RED strict build `8558fa18-18f9-4f19-9dd8-262dd1059ffd` passed. The E070 JUnit run exited 2 with one failure and zero skips. The sword appeared on zero-based page 1 at `(2, 0)` unrotated. It fit on page 0 at `(1, 2)` rotated. The classifier returned `RED_EVIDENCE_OK`. The GREEN test also verifies the original money-stack IDs and quantities and validates the full layout.
- Strict build attempts `1e8a3986-eb4f-47e6-af13-1835d413f443` and `ceae7d7d-00c8-4395-922e-d4994940e6fc` surfaced ordering, style and test assertion signature errors during Task 1. Task 2 strict build `463bcaf6-dd75-45d1-91fb-01fa8cf0ffa0` surfaced one test line-length violation. Both task builds and the final shared selection passed after corrections.

Reproduce either RED from its commit after the strict build with the same direct assembly command and `--filter-method '*<method>' --report-xunit-junit --report-xunit-junit-filename red.xml --results-directory <new-directory> --progress off --no-ansi --show-test-results all`.

## Deviations from plan

- [Rule 2] Task 1 added the session-owned dispatcher and a narrow locked publication callback to `FoundationSession.cs`. The existing edit gate supplied inventory, definitions and population. Nearby projection also needs the exact same world's anchors, snapshot revision and ready controller. `RunWorld.cs` now shares the existing geometry predicate between anchored targets and an ordinary NPC's current pose. Verified in `8a36499`.
- [Rule 3] The production startup call entered `RunInventory.Create`. Task 2 kept that entry point and redirected it to the fresh population inventory constructor in `RunPopulation.cs`, replacing the parallel stash placer in `RunInventory.cs`. Verified in `740bf23` through the ordinary session constructor and E070.

The phase orchestrator owns the shared worktree, roadmap and state updates, draft pull request and task-output cleanup. [Draft pull request #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16)

## Self-Check: PASSED
