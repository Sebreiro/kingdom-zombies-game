---
phase: 06-survivors-inventory-and-combat
plan: "13"
subsystem: scenario-fixtures-and-evidence
tags: [godot, csharp, fixtures, headless, rendered, lan]
requires:
  - phase: 06-07
    provides: "Actual corpse ownership, clean expiry and explicit successor selection"
  - phase: 06-09
    provides: "Production solo and LAN authority with current public and recipient-private scopes"
  - phase: 06-10
    provides: "Native movement, world input and authored appearance nodes"
  - phase: 06-11
    provides: "Native equipment, nested inventory and scoped nearby access"
  - phase: 06-12
    provides: "Native vitals, action feedback, spectator and explicit Respawn"
provides:
  - "Scoped stationary combat fixtures that use production rules and preserve ordinary startup"
  - "Six runnable native headless cases with scoped current state and recorded process cleanup"
  - "Six saved solo and three saved four-peer Windows procedures, plus exact source and ownership mappings"
  - "Complete discovered and executed census of 132 resolved Phase 6 edge identities"
affects: [06-14, 06-15]
tech-stack:
  added: []
  patterns: [nonce-owned-debug-seed, recipient-scoped-native-observation, shared-scenario-driver]
key-files:
  created:
    - .planning/phases/06-survivors-inventory-and-combat/06-SCENARIOS.md
    - src/godot-game/core/RunCombatFixture.cs
    - src/godot-game/tests/scenarios/phase06-headless.cjs
    - src/godot-game/tests/scenarios/phase06-solo.cjs
    - src/godot-game/tests/scenarios/phase06-lan.cjs
    - src/godot-game/tests/scenarios/phase06-rendered.cjs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunProjections.cs
    - src/godot-game/game/foundation/FoundationAutomation.cs
    - src/godot-game/game/foundation/FoundationCoordinator.cs
    - src/godot-game/game/foundation/FoundationDiagnostics.cs
    - src/godot-game/game/foundation/FoundationDrawObserver.cs
    - src/godot-game/game/foundation/FoundationRoot.cs
    - src/godot-game/tests/verification/EngineAccessPolicy.cs
    - src/godot-game/tests/verification/Phase06FixtureTests.cs
    - src/godot-game/tests/verification/Phase06SnapshotTests.cs
    - src/godot-game/tools/game-tests/catalog.json
    - src/godot-game/tools/game-tests/catalog.test.cjs
    - src/godot-game/tools/lan-checks/rendered.cjs
    - src/godot-game/tools/lan-checks/windows-input.ps1
key-decisions:
  - "Derive repeatable successor selection from the selected fixture mode while ordinary and fixture run IDs remain unique."
  - "Reuse one solo process for five native headless cases and a separate actual host plus three guest processes for LAN."
  - "Use headless exact requests for competing stamps. The saved physical case requires one withdrawal and either a native denial or a visibly corrected second target."
  - "Bind every scenario to current observations, owned process identity and a group cleanup record. Physical procedures also require native input receipts and verified post-draw PNGs."
requirements-completed: [CHAR-01, CHAR-02, CHAR-03, CHAR-04, CHAR-05, CHAR-06, INV-01, INV-02, INV-03, INV-04, INV-05, INV-06, INV-07, INV-08, INV-09, INV-10, INV-11, INV-12, INV-13, COMBAT-01, COMBAT-02, COMBAT-03, COMBAT-05, COMBAT-06, COMBAT-08, RUN-01, RUN-02, RUN-03, RUN-04]
actuals:
  tokens: 35662
  tasks: 3
  commits: 5
commits: 5
plan_head_before: 6ec99a99c66ab648b1784507894182e03c3eced1
plan_head_after: 03150e3620cf0c325d6e278e0b47c247da4cdeae
duration: 39min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 13: Playable Fixture and Saved Scenarios Summary

**Normal Start and Host can enter an explicitly owned combat fixture through the production scene. The shared runner now executes six native no-window cases and stores nine physical Windows procedures for review.**

## Delivered behavior

- The debug fixture requires an owned nonce directory and a one-use, role-matched seed request before Start or Host. It supplies three stationary hostile survivors, two adjacent with the same contact, real melee and bow gear, live and destroyed walls, a raised terrain blocker, food, nested carried storage and seeded loot. Triggers use the authoritative combat action fence and normal time step. The four modes cover normal field play, one-arrow bow play, one-health death and a distant eligible successor. Fresh ordinary starts retain eight survivors, the configured outfit and the exact independent stash stacks.
- The fixture tests exercise accepted and duplicate actions, ineligible targets, actual hostile ammunition and wall damage, independent reset, separate armed and empty corpses, precise empty expiry and genuine near/far succession. Both fixture and ordinary setup go through the existing session composition.
- Headless drivers use the shared checker, normal game menus and native key/mouse routes. A nonce-owned, headless-only test route submits exact item requests through the current production coordinator when the proof needs a retained stamp or two simultaneous guest requests. The four real ENet recipients each expose their own permitted views and the shared world. The full withdrawal made the contested stack private to its winner. The other receipt was `AccessLost`, both recipients saw the corrected stash, and only the winner's carried scope included the original item ID.
- The saved Windows drivers use the existing physical input, draw and process helpers. They bind current role, session, binding, revision and imported assembly hashes to native input and decoded 1280×720 draw artifacts. The input helper adds owned pointer move and matched left/right transitions for actual drag and context menus. The observer reads authored survivor, corpse, crown, arrow and control visibility on the main thread. Each group requires successful normal shutdown before its rows retain pass evidence.

## Task commits

| Task | Commit | Result |
| --- | --- | --- |
| 1 RED | `cb78d4d` | Discovered the exact fixture E129 test and required the real implementation |
| 1 GREEN | `9f3c923` | Added the independent, nonce-owned stationary fixture and production authority path |
| 2 | `0914bb8` | Added six saved headless cases, current recipient diagnostics and catalog selection |
| 3 | `97cacc6` | Saved ordered physical solo and four-peer cases, added actual two-corpse proof and terminal evidence |
| Verification repair | `03150e3` | Registered one exact guarded appearance method and corrected the existing E118 snapshot test name |

## Verification

The final checked code was `03150e3620cf0c325d6e278e0b47c247da4cdeae` on Windows x64, with Godot 4.7.2 .NET, Node 24.22.1 and .NET SDK 10.0.404.

| Check | Evidence | Result |
| --- | --- | --- |
| Shared `--pr` | `868b0d8d-6513-4635-8d7b-789e14d88b00` | Clean exact branch, exit 0, 4/4 in 56.433 s. Strict build, four repository tests, 191 pure tests and actual no-window solo gameplay passed. |
| `edge-check.cjs 6` | `.local/runs/06-13-tdd/final-edge-census.json` plus the exact shared `basic.pure/tests.json` | All 132 required methods resolved and matched individual passed results. E074 remains deferred. All four P01/P02 clean and violation controls passed. |
| Exact `--group phase06.headless` | `11b95a72-fbb6-4dea-bb53-72e5079b0c11` | Clean exact branch, exit 0, 6/6 in 45.573 s. All five children acknowledged ordinary Quit and exited zero, with zero emergency kills. |
| Focused `Phase06FixtureTests` | `.local/runs/06-13-tdd/fixture-complete/tests.json` and the final shared pure results | All 9 passed. The focused test report measured 0.385 s. |
| `--list`, catalog and timing controls, JS/PowerShell parser checks | Pinned Node saved-catalog tests and native timing tests | Fifteen catalog tests and seven existing timing/evidence controls passed. All six headless, six solo and three LAN cases resolved in declared order, with exact roles. |
| Physical Windows runtime | Saved `phase06.solo` and `phase06.lan` groups | Pending the 06-14 whole-phase review and subsequent rendered evidence. |

In the final headless invocation, solo PID 36652 ran the five compatible cases. The four LAN PIDs were 31876 (host), 18396, 15516 and 38184. The LAN nonce was `4c72829a-5f02-4e62-b5f5-a05d45f03190`. Each participant installed a distinct control binding and scoped item identities on the same current session. The exact withdrawal receipts were `AccessLost` for guest 1 and `Applied` for guest 2.

To reproduce the validated selections from the pinned phase worktree:

```powershell
$node = 'D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe'
& $node src/godot-game/tools/game-tests/check.cjs --pr
& $node src/godot-game/tools/game-tests/edge-check.cjs 6
& $node src/godot-game/tools/game-tests/check.cjs --group phase06.headless
& $node src/godot-game/tools/game-tests/check.cjs --list
```

## TDD and corrections

- Strict preparation `5b6297eb-b9c5-404c-9706-128b41817f51` passed. The named E129 RED failed exactly once for the absent fixture with exit 2. The GSD check returned `RED_EVIDENCE_OK`. Initial GREEN build corrections removed unused imports and fixed exact analyzers for named arguments, ordinal comparison and source line length. Strict GREEN `f773acb6-59ee-42d2-b576-fcf65433871d` and the initial eight fixture tests passed.
- The first headless runs exposed incorrect expected statuses, shutdown callback binding and the backpack slot name in the driver. The full withdrawal produces `AccessLost` for the other guest when the instance moves into another survivor's scope. A later run exposed that automatic placement selected an earlier equipped pocket rather than the backpack. The corrected proof reads all permitted carried storage and verifies the exact winner-owned item, with clean shutdown for every case. The committed predecessor run `9ec5731d-ffae-4b68-b85d-132b9880d044` passed 6/6 in 45.752 s.
- The first physical-procedure strict build `47cd3cd0-dd59-4630-90d0-88912e2fc1ff` found a 122-character diagnostics line. The first clean shared run `27005834-8cb8-464a-ae85-ba46e413a5e3` then rejected nine appearance reads in the unregistered observer method. The final repair isolates them in the exact path and signature `FoundationDrawObserver.ObserveAppearance()`, which starts with the captured main-thread guard. The code retains the existing engine boundary and derived-root policy.
- The first exact census reported `EDGE_TEST_MISSING E118`. The existing passing test was named `E118CapturesOneDetachedRunWithExactElapsedTime`. Naming it `Phase06_E118_CapturesOneDetachedRunWithExactElapsedTime` made the original assertions discoverable without changing snapshot logic.
- 06-14 retains the inherited asset source rebind, package and inspection verification, and the gate for physical review. 06-15 owns fresh phase-wide evidence. Aggregate source owners and later inventory filtering, AI, structures, campaign and persistence workflows retain their existing scheduled scope.

## Self-Check: PASSED

The six created files and five task commits were found in the pinned checkout. All final headless processes exited normally and Git reported a clean source worktree. The orchestrator retains the worktree and evidence for the remaining phase review and PR publication.

