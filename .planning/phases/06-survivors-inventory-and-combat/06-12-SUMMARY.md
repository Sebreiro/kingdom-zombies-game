---
phase: 06-survivors-inventory-and-combat
plan: "12"
subsystem: native-hud-and-spectator
tags: [godot, csharp, hud, spectator, succession, lan]
requires:
  - phase: 06-07
    provides: "Committed death epochs, persistent successors, ordered requests and living teammate rights"
  - phase: 06-09
    provides: "Solo/LAN action dispatch, recipient status and bounded public world observations"
  - phase: 06-10
    provides: "Native world input, authored crowns, flinch, equipped hands and quiet bow aim"
  - phase: 06-11
    provides: "Current native browsing surfaces, carried item stamps and shared inventory input capture"
provides:
  - "Live recipient HUD for vitals, committed rest/load modifiers, money, hands, readiness and four food references"
  - "Input capture, scoped feedback, nonlethal damage cues and direct native inventory access"
  - "Explicit Respawn and waiting, authorized teammate follow, held camera and verified successor presentation"
affects: [06-13, 06-14, 06-15]
tech-stack:
  added: []
  patterns: [recipient-scoped-hud, explicit-spectator-follow, known-viewport-handover]
key-files:
  created:
    - src/godot-game/game/foundation/FoundationHud.Live.cs
    - src/godot-game/game/foundation/FoundationHudState.cs
    - src/godot-game/game/foundation/FoundationSpectator.cs
    - src/godot-game/game/foundation/FoundationSpectatorPanel.cs
    - src/godot-game/core/RunProjections.Hud.cs
    - src/godot-game/tests/verification/Phase06UiStateTests.cs
  modified:
    - src/godot-game/core/RunProjections.cs
    - src/godot-game/game/foundation/FoundationHud.cs
    - src/godot-game/game/foundation/FoundationRoot.cs
    - src/godot-game/game/foundation/FoundationView.cs
    - src/godot-game/game/foundation/FoundationInput.cs
    - src/godot-game/game/foundation/FoundationWorldInput.cs
    - src/godot-game/game/foundation/FoundationAppearance.cs
    - src/godot-game/game/foundation/FoundationInventoryArt.cs
    - src/godot-game/game/foundation/FoundationCoordinator.cs
    - src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs
    - src/godot-game/game/foundation/FoundationCoordinator.GuestPublication.cs
    - src/godot-game/game/foundation/FoundationCoordinator.Lifetime.cs
    - src/godot-game/game/foundation/FoundationProtocol.cs
    - src/godot-game/game/foundation/FoundationDiagnostics.cs
    - src/godot-game/game/foundation/FoundationAutomation.cs
    - src/godot-game/tests/verification/EngineAccessPolicy.cs
    - src/godot-game/tests/verification/FoundationLinkedSources.cs
    - src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj
    - src/godot-game/tests/verification/Phase06SuccessionTests.cs
    - src/godot-game/tools/game-tests/headless.cjs
key-decisions:
  - "Use a bounded private HUD object within the current recipient status and verify action replies against the submitted control identity."
  - "Use the exact opposite-hand arrow count and a shared personal food cooldown. Suppress the extra empty-hand cue on an unloaded bow."
  - "Center native spectating on the committed death or explicitly selected living teammate. Interpolate an authorized nearby handover within previously shown camera bounds."
requirements-completed: [CHAR-03, CHAR-04, CHAR-05, CHAR-06, INV-07, COMBAT-03, COMBAT-08, RUN-01, RUN-02, RUN-03, RUN-04]
actuals:
  tokens: 31043
  tasks: 2
  commits: 4
commits: 4
plan_head_before: 58d6838d15acd71802fc30e5e3ff2a5c25a068d0
plan_head_after: cc7a8900c1691667e12fe6fc0b16cd36dd43ff56
duration: 38min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 12: Live HUD and Spectator Summary

**The normal gameplay scene displays current recipient vitals and equipment, captures input while browsing, and presents explicit teammate follow and Respawn without replacing authoritative survivor state.**

## Production and handoff

- The top-left HUD displays current and maximum health/stamina, actual carried money and load, the committed load band and movement/stamina modifiers, and Unsafe, Resting or Recovering. Load text retains enough precision to preserve the committed band. Stamina uses the bar. Sprint and jump denial stays beside the vitals, and attack and food denial stays beside the action bar.
- The bottom row uses canonical art for logical hands, exact ready arrows and four references to real carried food. Assigned food shares the survivor's actual cooldown. The bow follows the existing cursor aim unloaded. With empty ammunition, the count reads zero and the HUD adds neither an empty-hand marker nor an empty-shot notice. Committed attack and food timers determine shading, including a pending restored attack cooldown whose visual start is unavailable.
- The HUD's **Open inventory** control uses the same workspace action as I. The live clock advances while the workspace owns input. Nonlethal damage uses a 250 ms local pulse and edge tint above panels and retains the established authored flinch. Valid existing preview identities remain current across these vitals-only changes.
- Death removes living controls and HUD and fixes the horizontal camera at the committed death contact. The spectator card enables **Respawn** at the authoritative one-second threshold and shows the pending or waiting state. The actual prior death epoch and submission sequence fence requests. Teammates were authorized in recipient status and matched against the current ready roster. Follow is explicit, uses stable slot order, and stops at the last camera view if the target disappears.
- A successor activates the living presentation when the bound public pose and private status agree. The held camera moves smoothly for 250 ms only when the new body is within one quarter of the viewport width and the full sweep remains inside previously shown horizontal ranges. Other handovers cut to the authorized successor. The HUD and current crown reflect the transferred control together, and the banner displays its actual identity for two seconds. This uses the existing view bounds.

## Task commits

| Task | Commit | Result |
| --- | --- | --- |
| 1 RED | `2830e1f` | Required the linked live HUD decision path for E125 |
| 1 GREEN | `fdb11ce` | Installed native vitals, action feedback and current recipient readback |
| 2 RED | `920948a` | Required the linked explicit teammate-follow model for E126 |
| 2 GREEN | `cc7a890` | Installed native spectator controls, view retirement and authoritative handover |

## Verification

The final clean code was `cc7a8900c1691667e12fe6fc0b16cd36dd43ff56`. Checks ran on Windows x64 with Godot 4.7.2 .NET, Node 24.22.1 and .NET SDK 10.0.404. All commands completed without opening a rendered game window.

| Check | Evidence | Result |
| --- | --- | --- |
| Shared PR selection | `9694f3d8-7cd8-45b3-8c1f-2d05d374359c` | Exit 0, 4/4 in 57.413 s. Strict build, four repository tests, 182 pure tests and the actual solo native path passed. |
| Direct `Phase06UiStateTests` | `.local/runs/06-12-tdd/final-cc7a890/tests.json` | Exit 0, 9/9 in 0.755 s with zero failures or skips. |
| Input and LAN compatibility | `8b2f6f63-c551-44e6-8ec7-eb7be6e623da`, inner report `eadd0b64-1d84-4f2f-9836-b5b60be15596` | Exit 0, 3/3 in 67.332 s. Core input, two-process tracer and native lifecycle passed. The tracer took 3.200 s and lifecycle took 27.450 s. |

The direct cases exercised E125 panel-first input while live time and committed damage advance, E127 repeated cancellation and unchanged item identity, E126 zero/one/multiple stable follow choices and lost-target hold, and E128 the exact 999/1000 ms gate, current versus prior receipt, one queue request and real succession. Additional cases covered one shared food cooldown, reserve exclusion from ready arrows, zero-ammo silence and all three camera outcomes. The queue case verified automatic completion on release of a genuinely eligible teammate.

Solo process 23940, nonce `5a282d67-f368-4b20-8359-5979d8e090f0`, exercised I, the HUD inventory button, keyboard details and live stash page 1 → 2 → 1. A scoped deterministic hostile attack retired `survivor-01` at generation 1, removed the live HUD and held camera center −122.464 logical px. D remained physically held with zero gameplay direction. The first enabled native sample arrived 1,033 ms after the death sample with no living assignment. Activating the native button installed `survivor-02` at generation 2 on the same binding and transport actor. The new HUD and transfer banner appeared, then the process exited through ordinary Quit with code 0 and no cleanup kill.

Use the pinned tools from the phase worktree to reproduce this selection.

```powershell
$node = 'D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe'
& $node src/godot-game/tools/game-tests/check.cjs --pr
$env:PHASE1_ROOT = (git rev-parse --show-toplevel)
dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-class '*Phase06UiStateTests' --minimum-expected-tests 9 --progress off --no-ansi --show-test-results all
& $node src/godot-game/tools/game-tests/check.cjs --case lan-checks.core.input,lan-checks.network.tracer,lan-checks.network.lifecycle
```

## TDD and corrections

- Task 1 strict build `b00317c7-8dd5-4afe-9bf3-758781db5791` passed. The named E125 RED discovered and failed exactly once with exit 2 for the absent production projection. GSD returned `RED_EVIDENCE_OK`. Strict, policy and headless GREEN `5ecdd000-b046-41e2-abfa-5e792feb041f` passed, and all three focused cases passed in 0.653 s.
- Task 2 strict build `cd70d5f8-6958-42ca-a0ed-72c7d3d96032` passed. The named E126 RED discovered and failed exactly once with exit 2 for the absent production model. GSD returned `RED_EVIDENCE_OK`. First integrated selection `b83c5938` passed all three objective checks and eight focused cases. The expanded nine-case selection `fc4f9b3b` also passed. The final exact commit reran the full selection after its silence and empty-state checks.
- Early HUD compile `14652669` found current member-order and source-format violations. Run `b7565829` passed the native and repository checks and exposed two focused damage fixtures that used units instead of the authority's position subunits. Correcting the fixture to −7,350 subunits allowed actual nonlethal contact.
- The first spectator build `6265df93` rejected direct native references to Core internals, plus style and fixture constructor findings. The scoped no-window proof now uses the existing authenticated dispatcher on the normal game. Its fixture action is available in debug only through the nonce-scoped local driver and is excluded from accepted LAN action kinds. The native hit, corpse, Respawn, and successor use ordinary Core paths. Builds `3e106cbc` and `59b074c1` found separate line-format findings in the queued test and quiet-arrow presentation before the exact GREEN `55be13cb-68ce-4e62-8499-9c79e878fc55`.
- The HUD additions use the private status frame. Actions echo their submitted survivor and epoch in the existing result fields, and the receiver ignores obsolete identities. No new public types or blanket engine permissions were needed. Native UI permissions are exact guarded methods.
- Phase 06-14 retains rendered review, repeated physical-input and multiparty visual scenarios, and the inherited asset support rebind. A roster-only fallback can continue holding the prior authorized camera. A new assignment waits for its matching public pose before activating the local living controls.

## Self-Check: PASSED

The six new files and four task commits were verified in the pinned checkout. The worktree stays available to the orchestrator for PR publication and the remaining reviewed phase plans.
