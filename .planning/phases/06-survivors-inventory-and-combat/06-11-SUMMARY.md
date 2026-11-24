---
phase: 06-survivors-inventory-and-combat
plan: "11"
subsystem: native-inventory
tags: [godot, csharp, inventory, equipment, stash, lan]
requires:
  - phase: 04-asset-production
    provides: "Packaged inventory theme, character silhouette and canonical item atlas sources"
  - phase: 06-04
    provides: "Current nearby grants, stamped paths and deterministic automatic storage transactions"
  - phase: 06-09
    provides: "Solo and LAN command dispatch, recipient scopes, readback and correction receipts"
  - phase: 06-10
    provides: "Native gameplay input and the workspace capture interface"
provides:
  - "Usable native equipment, carried grids, movable nested windows and authorized nearby inventories"
  - "Actual drag, rotation, split, drop, transfer and food reference commands with current stamps"
  - "Unfiltered stash paging, revocation and depositor-only cell feedback"
affects: [06-12, 06-13, 06-14, 06-15]
tech-stack:
  added: []
  patterns: [recipient-scoped-workspace, committed-item-preview, exact-native-guard]
key-files:
  created:
    - src/godot-game/game/foundation/FoundationInventoryWorkspace.cs
    - src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs
    - src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs
    - src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs
    - src/godot-game/game/foundation/FoundationInventoryInteraction.cs
    - src/godot-game/game/foundation/FoundationInventoryArt.cs
    - src/godot-game/game/foundation/FoundationInventoryGrid.cs
    - src/godot-game/tests/verification/Phase06InventoryInteractionTests.cs
    - src/godot-game/tests/verification/Phase06ExternalInteractionTests.cs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunItemActions.cs
    - src/godot-game/core/RunItemTransfer.cs
    - src/godot-game/core/RunProjections.cs
    - src/godot-game/core/RunStash.cs
    - src/godot-game/game/foundation/FoundationCoordinator.cs
    - src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs
    - src/godot-game/game/foundation/FoundationDiagnostics.cs
    - src/godot-game/game/foundation/FoundationPeerBinding.cs
    - src/godot-game/game/foundation/FoundationProtocol.cs
    - src/godot-game/game/foundation/FoundationRoot.cs
    - src/godot-game/game/foundation/FoundationViewInbox.cs
    - src/godot-game/tests/verification/EngineAccessPolicy.cs
    - src/godot-game/tests/verification/FoundationLinkedSources.cs
    - src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj
    - src/godot-game/tools/game-tests/headless.cjs
key-decisions:
  - "Keep the target chooser in the existing canvas with stable permitted target identities and keyboard focus."
  - "Request up to 16 validated item windows in each private scope, including worn storage. Prompt the player to close a bag when the scope is full."
  - "Apply canonical item art, carried stack references and committed transfer receipts through the shared inventory renderer."
requirements-completed: [INV-02, INV-03, INV-04, INV-05, INV-06, INV-07, INV-08, INV-09, INV-10, INV-11, CHAR-05]
actuals:
  tokens: 50457
  tasks: 2
  commits: 5
commits: 5
plan_head_before: 38cb05569461cad8697a98a2e21677c7acffa32f
plan_head_after: 1bbd70da56385e28fe9173b900ac85bd7a8106cf
duration: 60min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 11: Native Inventory and Nearby Access Summary

**Normal gameplay exposes actual equipment, carried containers and authorized nearby inventory through one native workspace and the shared transaction path.**

## Production and handoff

- I opens eleven equipment slots, the authored neutral silhouette and actual worn storage grids. Nested containers open through **Open**, remember their clamped positions for the play session and close with their descendants. Equipped items and grids share the packaged theme and canonical icons. Hover and keyboard focus show the same current item details.
- Dragging keeps the authoritative item at its stamped source until a valid action commits. Full oriented cells, condition bars and compatible free or occupied slots use the shared renderer. Shift-drag validates an exact whole quantity, shows extracted and remaining quantities and supports **Half**. Outside release cancels. **Drop**, **Consume** and quick-food assignment are explicit actions on a real item. Food consumption uses the existing authority and cooldown.
- E opens the authorized target list or selects the sole nearby target. The side-by-side workspace locates real NPC/corpse gear, actual container contents and pile items from private readback. The selected target marker reads public world geometry. Stash navigation uses the current page token. Automatic deposit follows the committed first affected page and marks only the depositor's matching cells for two seconds, with the additional page count when applicable.
- Withdrawal merges actual compatible carried stacks before legal remaining storage space. The current target and token must authorize the stamped source for stash, NPC, corpse and pile withdrawals. Access loss removes the affected panel data, bag branches and previews while preserving independent carried windows. Correction and denied transactions leave item ownership with the authority.
- The typed `FoundationInput.IWorkspace` implementation and `FoundationRoot.WorkspaceInput` remain the entry point for the 06-12 HUD. The workspace consumes gameplay input while live simulation continues. 06-12 owns the status/action bar and spectator presentation. Filtering remains scheduled for Phase 12.

## Task commits

| Task | Commit | Result |
| --- | --- | --- |
| 1 RED | `9b08308` | Required an engine-free production preview and cancellation test |
| 1 GREEN | `4a5a047` | Connected native carried and nested inventory to current authority |
| 2 RED | `e315bb2` | Required valid current-token stash navigation |
| 2 GREEN | `80d1485` | Connected authorized nearby inventory, paging, correction and exact withdrawals |
| Keyboard focus repair | `1bbd70d` | Exposed the current item details through keyboard focus and cleared them on close |

## Verification

The final clean code was `1bbd70da56385e28fe9173b900ac85bd7a8106cf`. Checks ran on Windows x64 with Node 24.22.1, .NET SDK 10.0.404 and runtime 10.0.15, and Godot 4.7.2 .NET. The shared checker verified the clean tree at that revision.

| Verification | Run | Result |
| --- | --- | --- |
| Shared PR selection | `ef886668-c980-4ed8-bfcf-604f1701a8f2` | Exit 0, 4/4 in 54.587 s. Strict build, four repository tests, 173 pure tests and native solo passed with no skips. |
| Both focused classes | `.local/runs/06-11-tdd/final-1bbd70d/tests.json` | Exit 0, 13/13 in 0.650 s with no skips. The shared pure result independently ran the same 13 cases. |
| Focus and local LAN regressions | `ec247b3b-8b95-482d-a19a-2d7b13bde166`, LAN report `e06dc57c-d33a-46e0-87bc-09c5caf7b21b` | Exit 0, 3/3 in 65.589 s. Core input, native two-process tracer and native lifecycle passed. The tracer took 2.907 s and lifecycle took 27.144 s. |

The headless solo process PID 5560 and nonce `c40115b9-57c4-43d1-be25-5408925efffa` opened the carried and nearby workspace from native keys. The committed stash page went 1 → 2 → 1 under **Ready** receipts 4, 5 and 6. Escape returned to page 0 with no active target. Keyboard focus showed the equipped item details before the workspace closed. With D still physically held, the actor stayed at −7,688 subunits, direction zero, while committed elapsed time grew from 1,148 to 1,237 ms. Escape removed the workspace and details with the old physical hold still neutral. Ordinary Quit returned code 0 with no cleanup kill. The 06-11 native checks used no rendered game window.

Run the checks from the phase worktree with the pinned Windows toolchain. Scratch report paths are useful until phase-worktree cleanup.

```powershell
$node = 'D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe'
& $node src/godot-game/tools/game-tests/check.cjs --pr
$env:PHASE1_ROOT = (git rev-parse --show-toplevel)
dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06InventoryInteractionTests' '*Phase06ExternalInteractionTests' --minimum-expected-tests 13 --progress off --no-ansi --show-test-results all
& $node src/godot-game/tools/game-tests/check.cjs --case lan-checks.core.input,lan-checks.network.tracer,lan-checks.network.lifecycle
```

## TDD Gate Compliance

- Task 1 strict build `cfd390ee-91c4-4530-9769-58d64bb15b88` passed. The single RED test `Phase06_PreviewCancellationPreservesTheCommittedSource` failed with exit 2 for the absent production preview. GSD returned `RED_EVIDENCE_OK`. GREEN strict build `d54eaed0-9ad0-4071-ba0f-60b34a1530de` and all five focused tests passed in 0.629 s. Native and policy checks passed in `914edfeb-6670-4a86-86d3-6c3cc45876c9`.
- Task 2 strict build `88f2603f-538f-4775-9ec0-c503fdaf753b` passed. The single RED test `Phase06_ExternalPagerUsesCurrentTokenAndRejectsBadInput` failed with exit 2 for the absent page-request path. GSD returned `RED_EVIDENCE_OK`. The expanded targeted RED on build `4af89492-259f-4ad4-9c14-b0e77c9ae4d5` passed five and failed the three authorized NPC/corpse/pile withdrawals with `AccessLost`. GREEN strict build `0f8f5f19-a72f-4cdc-9eed-988528451db1` and all eight external focused cases passed in 0.682 s.

## Deviations and original failures

- Worn and manually opened bags required several simultaneously authorized grids. The bound request adds up to 16 fixed per-recipient scopes each for own and external windows. It keeps the prior single-window request compatible, revalidates every requested item, revokes obsolete windows and carries no durable snapshots or other controlled survivors' private contents. Exact stamped split merges also carry the requested quantity through the existing action shape.
- Task 1 native run `eb765cde-d396-406e-8cb0-fde4726efb54` failed while opening the workspace because the body record supplied an exact `res://` atlas reference. The presenter now reads that authorized source. Early strict builds `490f53bb`, `79563e6e` and `48ffaf4d` exposed formatting and record constructor rules, which were resolved before GREEN.
- The planned external presenter lives in `FoundationInventoryWorkspace.External.cs` to follow the enforced partial-class filename rule and share the existing close and revocation lifecycle. Early strict runs `5c622cfc`, `3b0ac603`, and `fb4fc08d` exposed explicit type, partial filename and exact whitespace findings. The first expanded functional run `3a9c6e52` exposed two test fixture assumptions about the actual authored page count and a nonstackable item, both corrected.
- Run `4ebc0c76-d14e-4715-8708-9e2a9de05a58` found that the popup target chooser lost native keyboard selection and identified two unguarded engine accesses. Moving the list into the existing canvas and guarding the exact native access sites passed both checks in `cf165155-e789-483a-8fdd-1638a5ba22b8`. The later keyboard-details attempt `114372e8` hit the strict member-order rule. The corrected strict, repository and native selection passed in `f01d630d-d74a-4d84-8a20-109d8c25298d` before the clean rerun.
- The inherited Core quick-transfer handler only admitted stash sources. The authority now checks the exact current external root for all four source kinds and reuses the existing carried merge and placement routine. Stash deposit still requires the selected stash and a carried source. The public world frame adds the actual stash anchor to its existing point rows for the target marker. No new endpoint or broad engine permission was introduced.
- Visual layout, physical-input coverage and multiparty UI use retain their whole-phase Windows review in 06-14. The inherited asset support rebind and packaging are also assigned to 06-14. The final headless result covers native open, keyboard item details and paging, while the focused tests cover transactions and access races.

## Self-Check: PASSED

All claimed new files were found in the pinned worktree. The five task commits are ancestors of the tested code. The worktree stays available to the phase orchestrator for PR publication and the remaining reviewed plans.
