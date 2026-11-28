---
phase: 06-survivors-inventory-and-combat
plan: "02"
subsystem: gameplay-authority
tags: [godot, csharp, survivors, inventory, admission, headless]
requires:
  - phase: 06-01
    provides: "One immutable session publication with authoritative elapsed time and world geometry"
  - phase: 03-four-player-lan-foundation
    provides: "Normal solo and host entry, guest reservations, baseline readiness and peer retirement"
provides:
  - "Eight persistent recruits, independently identified starter outfits and one configured stash"
  - "Validated bounded native definitions and deterministic initial stash placement"
  - "Exclusive survivor leases that retain recruit state across transport departure and admission"
affects: [06-03, 06-04, 06-05, 06-06, 06-07, 06-08, 06-09, 06-14]
tech-stack:
  added: []
  patterns: [immutable-publication, authored-bootstrap, generation-scoped-control]
key-files:
  created:
    - src/godot-game/core/RunDefinitions.cs
    - src/godot-game/core/RunInventory.cs
    - src/godot-game/core/RunPopulation.cs
    - src/godot-game/tests/verification/Phase06PopulationTests.cs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/KingdomZombies.Core.csproj
    - src/godot-game/game/foundation/FoundationCoordinator.cs
    - src/godot-game/game/foundation/FoundationRoot.cs
key-decisions:
  - "Start and Host use bounded packaged resource bytes. Pure inherited callers read embedded copies of the same tracked JSON."
  - "A ready wire participant can act only through its uniquely bound living survivor lease. Movement stores the current pose in the persistent recruit."
  - "Fresh residents and item identities are scoped to the run. Legacy wire actor tokens remain scoped to transport bindings until plan 06-09."
requirements-completed: [CHAR-01, INV-01, INV-02, RUN-01]
actuals:
  tokens: 22125
  tasks: 2
  commits: 5
commits: 5
plan_head_before: f8e036ca139909ce5a031325e141c083b7f8f46e
plan_head_after: a9f4c232d5e445ba0e71a63ba4445c3ceeafedcb
duration: 25min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 02: Starting Survivors and Stash Summary

**Each fresh run now starts with eight persistent recruits and one authored stash. Guests reserve existing recruits and release them with their current pose, role, vitals, and possessions.**

## Contract for dependent plans

- `FoundationSession` commits the persistent `RunPopulation`, `RunInventory`, and validated `RunDefinitions` with the existing snapshot, world and elapsed time. Reads return immutable values. Session mutation serializes admission, readiness, departure, held input and simulation. A rejected candidate keeps the earlier publication and identity allocation.
- `RunPopulation.Survivors` is ordered by run-scoped `survivor-01` through `survivor-08`. Each survivor has its own pose, decimal health and stamina, and remembered role. `RunPopulation.Leases` identifies the session, player, binding, player slot, survivor and generation. Reserved survivors are exclusive before readiness. A released binding cannot be issued again during that run. A new binding has a newer generation and projects the existing chosen recruit's pose. Its transport movement clock starts at zero.
- `RunInventory.Items` gives each instance one location in the same graph. Equipment locations carry a survivor and one of eleven slot identities. Shirt and trouser storage is supplied by the item's authored grid. The default outfit instance IDs occupy 1–24, the 32 stash stacks occupy 25–56, and the next available ID is 57. There is one stash with two initial 10×10 pages, 102 occupied cells and exact quantities and condition. Placement uses authored stack order, scans page and row order for unrotated fits before rotated fits, then opens a page as needed.
- The authored headers, 24-item census, slot compatibility, stack counts and caps, footprint and storage shape, weight, condition and startup-related combat references validate before creating a run. The reader accepts up to 64 KiB of strict UTF-8 JSON per file and depth 16. Starter rows name distinct known items and their exact legal integral counts. Seed intake is bounded at 512 stacks. The capacity of the shared stash is paged independently of that seed bound. Alternate valid inputs preserve authored numeric precision and can supply a stash grid that requires rotation.
- For 06-09, the guarded native root uses `FoundationSession.RequiredRunResources` and supplies detached bytes through `FoundationCoordinator.Start`. `FoundationSession.CreateFromResources` is the framework-only cross-assembly composition seam. The transport snapshot and schema remain those of Phase 3. The session still checks its current lease generation before applying held legacy movement. Plan 06-09 owns carrying survivor and control identities through the gameplay wire. Plan 06-07 owns death, waiting and successor control. Plan 06-03 owns inventory mutation and container traversal.
- The R-34 public boundary uses the approved `FoundationSession` type and framework values. Population, inventory and definition models are internal. The full repository boundary check passed with the existing native permission and visibility inventories.

## Task commits

| Task | Commit | Change |
| --- | --- | --- |
| 1 RED | `6825951` | Required the normal session to own a persistent roster |
| 1 GREEN | `980a8bb` | Created authored recruits, unique outfits and a deterministic starting stash in the session publication |
| 2 RED | `1a748ab` | Proved departure lost the authoritative moved position |
| 2 GREEN | `0db66b7` | Bound persistent survivors to ready and reserved participants and connected native startup |
| 2 repair | `a9f4c23` | Kept Godot resource operations inside the audited, synchronous native entry method |

## Verification

Executed on the development Windows x64 checkout with Node 24.22.1, .NET SDK 10.0.404 and Godot 4.7.2 .NET. Closing runs used a clean `a9f4c232d5e445ba0e71a63ba4445c3ceeafedcb`.

| Command | Invocation or evidence | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `cce49031-1987-4cf9-ae7c-e114f5658060` | Exit 0. Four objective passes with zero failures or blockers. Strict build, four repository checks, 33 pure tests and the headless solo case passed. |
| `dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06PopulationTests'` | `.local/runs/06-02-tdd/edges-final/tests.json`, CTRF | Exit 0. Nine tests passed, with zero failures or skips. |
| `node src/godot-game/tools/lan-checks/check.cjs --group network` | `bedfb9b2-ce35-475b-80ed-9917ba957d18` | Exit 0. Five cases passed, with zero failures, blockers or rendered windows. |

The direct filter added `--report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-02-tdd/edges-final --progress off --no-ansi --show-test-results all`. The seven exact edge methods in `KingdomZombies.Phase1.Tests.Phase06PopulationTests` were:

- `Phase06_E007_FreshRunCreatesEightIndependentOutfits`
- `Phase06_E008_OneThroughFourControllersShareTheOneRunKit`
- `Phase06_E009_ReservationsReleaseTheSameRecruitAndRejectOldInput`
- `Phase06_E010_NewSurvivorsAndStorageHaveNoPersonalSupplies`
- `Phase06_E011_RepeatedAdmissionDoesNotMultiplyBelongings`
- `Phase06_E012_AdmissionOrderingPreservesPublishedIdentityAndKitOrder`
- `Phase06_E013_ConfiguredStacksAndConditionAreExact`

The two accompanying tests rejected malformed or contradictory authored inputs and proved that the coordinator rejects missing content before opening the host port. E013 also ran a valid alternate authored kit with 180 money, rotated-only bow fits, and fractional maximum vitals. E009 retained the same moved survivor and outfit across departure and replacement, rejected retired input, and admitted one of two callers competing for the last slot.

The headless solo process was PID `27304` and nonce `3e1f2363-f9ca-4865-a363-4ee2857ee2c1`. It reached the unsafe and blocked native Hint at position -8000, elapsed 64 ms and revision 1. It reached the safe and reachable Hint at -6497, elapsed 609 ms and revision 80. The same process exercised an ordinary fresh Start after returning to the menu and acknowledged ordinary Quit at frame 153. It exited 0 with closed streams.

The native LAN group used ordinary Host and Join through local ENet. It passed `network.tracer`, `network.four-peer`, `network.lifecycle`, `network.idle-poll` and `network.discovery-profile`. The four-peer case observed one winner in a simultaneous final-slot contest. The lifecycle case exercised guest departure, unexpected peer loss, replacement and host loss. The group's 40.5-second invocation reports each owned process and its terminal evidence. These observations verify the legacy transport. The exact private inventory assertions come from the pure authority tests.

## TDD and deviations

- Task 1 RED ran the installed xUnit JUnit reporter with the exact E007 method filter. Exit 2, one failing assertion, zero skips. The prior production session had no persistent population. The fully qualified JUnit identity received `RED_EVIDENCE_OK`. After the task changes, strict invocation `26cfb4f5-62be-43fd-ac40-c271b3e174c7` passed and the direct class filter passed its seven then-present tests.
- Task 2 RED used the exact E009 filter and JUnit reporter. Exit 2, one failing assertion, zero skips. Movement and departure succeeded while the recruit's persistent position remained -8000 instead of -7700. This intended failure received `RED_EVIDENCE_OK`. Strict invocation `6649a8ed-dd67-4438-98b6-0f915baf61ed` and all nine focused tests passed after the lease and startup changes.
- [Rule 2] Added `KingdomZombies.Core.csproj` to Task 1 so inherited constructors and pure callers can load the same authored bytes independent of their working directory. The file embeds only the five tracked JSON documents under logical names `content/config/inventory.json`, `content/config/survivors.json`, `content/config/items.json`, `content/config/run.json` and `content/config/combat.json`. The native path requires its actual resource reads. The closing strict and repository checks verified the resulting project and type boundaries.
- [Rule 3] Shared PR invocation `afb7692d-e784-47fd-8d9a-ea53686baae7` on clean `0db66b7ff6ba021845927ea8d5b1c6541031c4eb` exited 1 with three passes, one failure and zero blockers. Its repository graph case found seven `ENGINE_ACCESS` observations because an anonymous function held the Godot resource calls. The repair keeps the native operations in the existing guarded `FoundationRoot.Enter`, with its reader delegate serving detached bytes. The closing PR run passed all four checks.
- [Rule 3] Strict tries `c39e74d6-729b-4347-bea5-b8eea7433390`, `fd744f33-9632-4a2d-b7e5-3a02b853e229`, `0370c4d2-d4b5-47c0-8748-1005818f8eb4`, `5d4a456a-c12d-42d0-848f-db242cde4587`, `91815eff-0364-4b3a-a59b-956171f895b7` and `c150e6ac-9333-4dfc-8477-6d4462e0a3ba` failed authored source rules for constructors, formatting, method length and explicit types. The repaired revision passed strict invocation `ad01ea8d-68c9-4fe6-92a3-5427d7c62021` before the closing build.

## Handoff

Execution started at 11:15 UTC and closing verification finished at 11:40 UTC on 2026-11-24. The phase orchestrator owns the shared phase worktree, draft PR update, evidence cleanup and roadmap and state updates. Plan 06-14 owns packaged asset acceptance and historical source binding updates. Plan 06-15 owns grouped rendered acceptance. [Phase 6 draft PR #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16)

## Self-Check: PASSED

The new files exist, the five task and repair commits are reachable, and both closing runner reports identify the recorded clean code revision. The focused test report contains all seven named edge methods and nine passing results.
