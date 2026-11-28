---
phase: 06-survivors-inventory-and-combat
plan: "08"
subsystem: gameplay-authority
tags: [godot, csharp, snapshots, inventory, replay, authority]
requires:
  - phase: 06-06
    provides: "Authoritative attacks, projectiles and action receipts"
  - phase: 06-07
    provides: "Atomic deaths, loot clocks and ordered successor requests"
provides:
  - "Versioned detached full-run capture and validated atomic restore"
  - "Saved inventory dimensions, orientation and per-item layout descriptors"
  - "Persistent action sequences, cleanup timers and successor random continuation"
affects: [06-09, 06-15, 09]
tech-stack:
  added: []
  patterns: [detached-versioned-document, strict-candidate-validation, single-authority-restore]
key-files:
  created:
    - src/godot-game/core/RunSnapshotCodec.cs
    - src/godot-game/core/RunSnapshotDocument.cs
    - src/godot-game/core/RunSavedLayout.cs
    - src/godot-game/tests/verification/Phase06SnapshotTests.cs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunInventory.cs
    - src/godot-game/core/RunInventoryRules.cs
    - src/godot-game/core/RunItemActions.cs
    - src/godot-game/core/RunProjectiles.cs
    - src/godot-game/core/RunStash.cs
    - src/godot-game/tests/verification/BuildFixture.cs
key-decisions:
  - "Capture private action journals and the immutable run publication under the same authority lock."
  - "Use stable record keys and explicit queue time and slot for wire-independent load ordering."
  - "Keep saved layout descriptors beside item identities, with explicit legacy markers and current placement bounds."
requirements-completed: [CHAR-01, INV-05, INV-06, INV-11, INV-12, INV-13, RUN-02, RUN-04]
actuals:
  tokens: 28266
  tasks: 2
  commits: 5
commits: 5
plan_head_before: 0233c9afc4ff2d504854cce1bff69e1a870380d1
plan_head_after: ed90bf38423c4ae3239682eab9df92476fd64884
duration: 32min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 08: Full Run Snapshot and Restore Summary

**The host captures an entire run and its replay journals in one revision. Valid saves restore that state atomically, including older inventory layouts, airborne projectiles, and pending successor requests.**

## Contract for dependent plans

- Use the internal `FoundationSession.CaptureRunSnapshot()` and `TryRestoreRunSnapshot(data, out error)` methods at the host authority boundary. Capture detaches the publication and journals under one lock, then serializes the document. Restore validates the versioned document and prepares every replacement dictionary before installing the candidate under that lock. A rejected restore preserves the original publication object, replay state, and input bytes. The example store, public roster, and recipient projections keep their own contracts.
- The document keeps the run and participant identities, leases, world geometry, survivor positions and vitals, complete inventory graph, quick references, live arrows and receipts, recorded deaths, loot clocks, waiting requests, and random seed, position, and operation contract. Future simulation advances the saved clock through the original authority paths. Action replay responds from the saved sequence history.
- Stable keys identify items, survivors, world fixtures, loot targets and lease epochs. Waiting records store accepted time and player slot. The decoder validates duplicates, mandatory fields, references, containment, record bounds, lethal relationships and awaiting requests before replacing the root. Exact decimals carry condition, motion and fractional simulation timestamps.
- `RunSavedLayout` retains original stash bounds, containment limit, per-item footprint, rotation rule, storage dimensions, stack and condition bounds, display resource descriptors, and legacy markers. The older placements remain attached to the same item IDs, rows and ownership paths. Later placement uses current bounds and cannot deepen an already invalid saved nesting chain. Manual save UX and legacy extraction belong to Phase 9.

## Task commits

| Task | Commit | Change |
| --- | --- | --- |
| 1 RED | `ea2e9da` | Required an internal detached full-run capture method |
| 1 GREEN | `d6cbb60` | Added versioned capture and round trips with saved geometry |
| 2 RED | `acc3d05` | Reproduced acceptance of a duplicate snapshot field |
| 2 GREEN | `2e5d051` | Validated the full candidate and proved coherent restore and continuation |
| Verification repair | `ed90bf3` | Copied required embedded inputs into existing compiler proof fixtures |

## Verification

Final evidence ran from clean code revision `ed90bf38423c4ae3239682eab9df92476fd64884`, Windows x64, Node 24.22.1, .NET SDK 10.0.404 with runtime 10.0.15, and Godot 4.7.2 .NET. The Node executable was `D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe`.

| Command | Evidence | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build` | Task 1 `baa677bf-5a26-42f0-b722-22469e65b704`, Task 2 `9c448c03-491b-4744-954a-3e01d9a96b27`, and fixture repair `834e5f01-7355-4d4f-9189-7a9c5470d8a2` | Each exited 0. The four Task 1 tests, seven Task 2 tests and 27 original example tests executed with zero failures or skips in their corresponding runs. |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `8d942226-3cf0-418f-a44d-b1379f776770` | Exit 0 with four objective passes in 42.362 seconds. Strict build, four repository tests, 142 pure tests, and the native headless solo check passed on clean code. |
| `dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06SnapshotTests' 'KingdomZombies.Phase1.Tests.SnapshotTests' 'KingdomZombies.Phase1.Tests.RestoreAtomicityTests'` | `.local/runs/06-08-tdd/final-committed/tests.json`, CTRF | Exit 0 in 23.942 seconds with 34 passing cases and zero failures or skips. All seven Phase 6 edges and 27 original example snapshot and restore tests executed, including the isolated mutation proof. |

The final direct test command set `PHASE1_ROOT` to the phase checkout and added `--minimum-expected-tests 34 --report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-08-tdd/final-committed --progress off --no-ansi`.

| Edge | Passing method |
| --- | --- |
| E118 | `E118CapturesOneDetachedRunWithExactElapsedTime` |
| E119 | `Phase06_E119_RestorePreservesSavedExtentsAndLegalExtrema` |
| E120 | `Phase06_E120_ConcurrentCaptureMatchesExactlyOneCommittedTransferRevision` |
| E121 | `Phase06_E121_RestoresEmptyAndSingleCollections` |
| E122 | `Phase06_E122_RejectsAmbiguousRecordsWithoutChangingTheRoot` |
| E123 | `Phase06_E123_ShuffledRecordsKeepPendingSlotsAndRandomContinuation` |
| E124 | `Phase06_E124_KeepsFractionalConditionAndVitalsWithIntegralCounts` |

The restored stash kept 10 by 10 cells under a current 7 by 10 rule. It preserved a stack at (9, 9) and a rotated bow with a 4 by 2 saved footprint at (6, 8) after the current bow changed to 1 by 3. The replay proof continued a real arrow in flight, rejected repeat ammo and food effects, preserved the fractional lethal contact time, and removed one empty corpse at its original deadline. It compared continued captures with the uninterrupted run. The ordering proof reversed saved record arrays, verified the earlier player slot for two same-time waiters, and continued a second succession draw from a nonzero saved RNG position.

The native headless solo process was PID `31300`, nonce `5b25f1dc-fb27-40fd-a210-3eb05c7939fc`. Its Hint moved from outside camp, danger near and stash blocked at −8000, elapsed 87 ms and revision 1, to inside camp, danger clear and stash reachable at −6485, elapsed 621 ms and revision 78. Ordinary Quit was acknowledged at frame 146. The process exited 0 with closed streams and no cleanup kill.

### Original failures

- Task 1 RED strict build `ede5d612-3f81-45b0-8845-165dcbd27ebf` passed. The named JUnit test exited 2, because `CaptureRunSnapshot` was missing. The classifier returned `RED_EVIDENCE_OK`.
- Task 2 RED strict build `a41b28ec-0b82-40e1-9f58-cd718c72abff` passed. The named JUnit test exited 2, because a duplicated `Version` field installed a candidate. The classifier returned `RED_EVIDENCE_OK`.
- Development builds `98211b3a`, `5eeb1f9f`, `fb07ae1d`, `fcf370d1`, `81b94fff`, `df63c335`, and `d1b9e6c6` caught assertion API, declaration style, mapping, and continuation indentation issues. The first seven-case run after build `ae7a5f4b` passed six cases and failed E123 because its tied melee fixture struck an adjacent recruit. The recipe now moves the chosen successor into uniquely closest reach before the second death.
- The direct original example suite initially ran 26 of 27 successfully. The isolated mutation proof first needed an explicit `PHASE1_ROOT`. With that root set, its clean copied build failed with CS1566 because `content/config/run.json` and the other already registered Core resources were missing from the fixture. The fixture repair restored 27 of 27 passing.

Reproduce either RED from its test commit after the strict build using the direct test assembly and `--filter-method '*<method>' --report-xunit-junit --report-xunit-junit-filename red.xml --results-directory <new-directory> --progress off --no-ansi --show-test-results all`.

## Deviations from plan

- [Rule 2] `RunItemActions` detaches and restores its replay journal under the authority lock, and `RunProjectiles.Impact` accepts detached receipt values without recreating a spent arrow. `RunInventory`, its validation, and `RunStash` preserve the saved layout through ordinary immutable inventory changes. The continuation gate rejects new placements outside current limits.
- [Rule 3] The original isolated example restore mutation test required the authored files already embedded by Core. `BuildFixture` now copies the existing required resource set into its Core and test scratch trees. The original codec, mutation and assertions are preserved.

The phase orchestrator owns the shared worktree, roadmap and state updates, draft pull request, and output cleanup. [Draft pull request #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16)

## Self-Check: PASSED
