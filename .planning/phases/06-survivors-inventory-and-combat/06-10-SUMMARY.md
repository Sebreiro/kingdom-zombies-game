---
phase: 06-survivors-inventory-and-combat
plan: "10"
subsystem: native-gameplay
tags: [godot, csharp, appearance, combat, input, lan]
requires:
  - phase: 04-asset-production
    provides: "Authored body fits, movement and bow clips, equipment atlases, crown and corpse compositions"
  - phase: 06-07
    provides: "Committed living control, corpses and actual retained inventory"
  - phase: 06-09
    provides: "Current solo and LAN action authority and recipient-scoped render readback"
provides:
  - "Ordinary gameplay presentation from committed equipment, condition, pose, arrow and corpse data"
  - "Fresh native movement, jump, attack, interaction and food input through current authority"
  - "Focus and workspace ownership with a typed native panel handoff"
  - "Guarded source imports and native headless input and lifetime verification"
affects: [06-11, 06-12, 06-13, 06-14, 06-15]
tech-stack:
  added: []
  patterns: [detached-appearance-frame, authored-composition-reuse, fresh-physical-input, exact-native-guard]
key-files:
  created:
    - src/godot-game/game/foundation/FoundationAppearance.cs
    - src/godot-game/game/foundation/FoundationAppearanceAssets.cs
    - src/godot-game/game/foundation/FoundationActorArt.cs
    - src/godot-game/game/foundation/FoundationEquipmentProjection.cs
    - src/godot-game/game/foundation/FoundationWorldInput.cs
    - src/godot-game/tests/verification/Phase06AppearanceTests.cs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunProjections.cs
    - src/godot-game/core/CharacterPresentation.cs
    - src/godot-game/game/foundation/FoundationRoot.cs
    - src/godot-game/game/foundation/FoundationInput.cs
    - src/godot-game/game/foundation/FoundationView.cs
    - src/godot-game/game/foundation/FoundationProtocol.cs
    - src/godot-game/game/foundation/FoundationCoordinator.Lifetime.cs
    - src/godot-game/game/inspection/CharacterPreview.cs
    - src/godot-game/tests/verification/FoundationLanInputTests.cs
    - src/godot-game/tests/verification/FoundationLanLifecycleTests.cs
    - src/godot-game/tests/verification/EngineAccessPolicy.cs
    - src/godot-game/tools/game-tests/headless.cjs
    - src/godot-game/tools/lan-checks/network.cjs
key-decisions:
  - "Retain a complete public appearance frame across roster-only fallbacks and leave that behavior visible for phase review."
  - "Compose one new native subject per frame so ENet polls continue during entry and replacement."
  - "Use the first active native GUI owner for pointer input and a single fresh physical edge for each discrete action."
  - "Keep the 250 ms graceful transport drain and 10,000 ms response deadline. Neutralize a disconnected binding immediately."
requirements-completed: [CHAR-02, COMBAT-03, COMBAT-08, INV-12, RUN-01]
actuals:
  tokens: 35879
  tasks: 2
  commits: 5
commits: 5
plan_head_before: 272020c0384e575507f02069465e9cf9f984d872
plan_head_after: 806f599c1b81cd42dd241691621ae25c08f54b67
duration: 66min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 10: Native Survivor Appearance and Controls Summary

**The normal world scene composes authored survivor and corpse appearances from current equipment and action state. The same scene sends fresh movement and mouse-aimed attacks through solo or LAN authority.**

## Production and handoff

- The public presentation carries living actors, their actual fitted equipment and broken-condition flags, accepted movement and action cues, active projectile locations, and surviving corpses with their remaining visible gear. Each subject keeps its logical hands when facing changes. A living controlled survivor can wear headgear and one cosmetic crown. Nested containers stay in inventory and create no extra worn layer.
- The native scene reuses the Phase 4 body and fit packs, authored tracks, and imported atlases. The crown uses `common.s0.7985` and committed arrows use `IT_02_master`. Unloaded bow aim keeps the small cursor crosshair and omits the nocked arrow. The presentation does not synthesize projectiles from mouse events or animation sampling.
- `FoundationRoot.WorkspaceInput` accepts the `FoundationInput.IWorkspace` consumer for 06-11. Implement `CapturesWorld` and handle the typed Interact, Inventory and Escape commands in the current panel, then the workspace. A visible editable control keeps its normal typing. Native mouse events reach the world surface after GUI hit testing. An unhandled E can select the sole currently permitted nearby target. The panel selection and inventory layouts are assigned to 06-11.
- Number keys use the recipient's own quick-reference ID and its current path stamp. The authority rechecks the exact carried item and food cooldown. The header can supply the stamp for a nested carried food item without opening or disclosing its inventory page. A stale or unowned path supplies no usable stamp.
- Phase 3 roster observations retain their original bound participant meaning. Native population and corpse nodes are separate. The view retains the last complete public appearance during an oversized roster-only fallback, while the legacy roster and camera can still advance. This can temporarily freeze guest pose, corpse and projectile changes until a complete frame arrives. Review the effect on large public populations at the phase gate.
- CH-01 uses the authored jump clip. Other bodies follow their committed airborne positions with their neutral authored pose. Motion, strike and flinch cues are transient presentation values. A durable restore preserves the gameplay state and does not replay earlier visual effects.

## Task commits

| Task | Commit | Result |
| --- | --- | --- |
| 1 RED | `1045f0c` | Required a linked production appearance mapping |
| 1 GREEN | `baddd11` | Projected current worn gear, corpses and arrows from actual game state |
| 2 RED | `c88eb51` | Required a fresh, one-shot jump consumer |
| 2 GREEN | `729d651` | Routed native keyboard, viewport mouse and quick-food input |
| Compatibility repair | `806f599` | Preserved idle identity, exact engine access and bounded LAN close |

## Verification

The final clean code was `806f599c1b81cd42dd241691621ae25c08f54b67`. Checks used Windows x64, Node 24.22.1, .NET SDK 10.0.404 and runtime 10.0.15, and Godot 4.7.2 .NET. Run the Node commands below with `D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe` from the phase worktree.

| Command or selection | Run | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `6bf62137-3010-4a66-900a-2b97aea90c1e` | Exit 0, 4/4 in 52.168 s. Strict build, all four repository tests, all 160 pure tests, and the native solo smoke passed. |
| Direct `Phase06AppearanceTests` and `FoundationLanInputTests` | `.local/runs/06-10-tdd/final-806f599/tests.json` | Exit 0, 11/11 in 0.573 s with zero failures or skips. |
| `node src/godot-game/tools/game-tests/check.cjs --case lan-checks.core.input,lan-checks.network.tracer,lan-checks.network.lifecycle` | `48d9a951-6690-4cf8-80a6-e72c37c1a585`, LAN report `16e046e8-1417-4413-b902-693a5898ce58` | Exit 0, 3/3 in 67.013 s. Original focus/alias case, native two-process tracer and native lifecycle passed. |
| Direct repository graph, E009, exact provider deadline and five native permission cases | Strict build `35976861-058a-4691-84ec-a28f3533efa3`, `.local/runs/06-10-tdd/repaired-boundaries/tests.json` | Exit 0, 8/8 in 21.751 s before the repair commit. All five adjacent signatures and missing guards rejected. Final clean PR repeated the production graph and pure regressions. |
| `node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.appearance.corpses,asset-pipeline.animation.bow,asset-pipeline.export.feasibility` | `5d947a72-527b-4bf1-b55c-eccee47fb191` | Exit 1 overall, 2/3 in 92.770 s. Authored bow and corpse source/native cases passed in `2f2b05a0-f40f-4448-92c7-baf74dcd9e48` and `32db5763-1cba-4715-a5d4-6150dd4dbdc5`. Legacy export `6dd23e34-cab7-4924-b83f-eb42895e2cb8` stopped at the known source-stamp gate. |

The focused assembly was `src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll`. After the shared build, run it with `dotnet`, `--filter-class '*Phase06AppearanceTests' '*FoundationLanInputTests' --minimum-expected-tests 11 --report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-10-tdd/final-806f599 --progress off --no-ansi --show-test-results all` and `PHASE1_ROOT` set to this checkout.

- E099 kept adjacent subjects, equal-looking separate hands, actual broken condition, headwear and one cosmetic crown distinct. E100 raced two real publication revisions. E101 proved empty visible gear and omission of a pouch inside a worn bag. E102 kept one uncrowned corpse and its broken gear after a replay and 180,001 ms. E103 preserved layer order and logical hand identity through mirroring and rejected regressed and malformed frames.
- The solo native process with PID 32384 and nonce `b0fd03ce-9f8b-4573-99f7-3cf5a4a1d413` moved from −8,000 to −6,500 subunits across the preserved reachability and rest transition. Restarting with D still held produced zero direction. Ordinary Shift+D produced `faster`. Space produced positive jump height of 139.115625 subunits at run time 337 ms. A viewport-injected left click aimed at (−7609.6076965332, 1750), committed attack sequence 7, and a repeated held press kept sequence 7. The process exited through ordinary Quit with code 0 and no cleanup kill.
- The native LAN lifecycle ran in 27.042 s. It recorded graceful guest departure in 66 ms, guest response loss in 10,021 ms and host response loss in 10,079 ms. Both current guests received the graceful host end. A new session rehosted, and guests recovered into usable solo sessions. No rendered game window opened.

## TDD Gate Compliance

Task 1 RED compiled with `610adc1f-929c-4a00-a7bf-d05aa5fa222b`. `Phase06_E099_ProjectsWornGearAndCosmeticCrown` failed because the production mapping was absent. Task 2 RED compiled with `435e7b8f-575a-4064-8998-5a16d7bf3c56`. `Input_JumpAndAttackRequireFreshWorldPresses` failed because the fresh jump consumer was absent. Each exact filtered run discovered one test, returned exit 2 and passed the GSD `RED_EVIDENCE_OK` classifier before production edits. Both fixtures became direct behavioral tests. Final GREEN evidence is listed above.

## Deviations and original failures

- **[Rule 3] Native source import.** The original solo check `8feeb2ce-9395-4839-bb9c-5b93fd4d5ea2` reached Start and reported `RUN_APPEARANCE_ATLAS assets/common/donor-kit.png` in an unimported checkout. Source-native verification now copies the production project into the invocation directory and imports it through the existing pinned single-threaded editor policy. The first imported launch succeeded, but the wrapper rejected the deliberately empty stderr file in `8c156746-6ac4-4237-811a-2f448449366b`. Its command JSON now retains both streams. A subsequent work-in-progress run `b690413a-0924-4b39-b41e-eeb301184e27` was rejected for source drift.
- **[Rule 1] Preserve identity and exact engine access.** The first clean PR run `67ce49d4-a70d-4255-9746-53793382e65d` passed strict build and solo but failed the native engine graph and historical E009 identity check. Movement presentation now expires by committed time without rewriting an idle survivor. Only exact guarded signatures gained native access. Existing generated policies and public type allowances took no changes. The earlier strict compile attempts exposed analyzer and generated-signature drift, and the final clean gate passed.
- **[Rule 1] Exercise the real native click route.** `b006a009-4280-4c56-b0a0-7fa17a5ab0ec` drove native sprint and jump but timed out waiting for the synthetic mouse to reach GUI dispatch. The nonce-scoped driver now pushes viewport input. The ordinary GUI surface dispatches the action, and the final run proved its aim and one-shot sequence.
- **[Rule 1] Preserve the Phase 3 LAN deadlines.** `d4e85900-94d7-4324-a251-e0e91ea537a2` and `164d5b47-6ee5-46e2-9f87-75ddcae7e2be` exposed replacement guest startup frames that stalled while composing all subjects and missed graceful host end. Composing one new subject per frame keeps the 250 ms drain usable. `834db4d7-a681-4d9c-ba02-e8bcdacdf768` exposed native disconnect being classified as an immediate new generation. The authority now stops held movement on provider loss, retains the exact response deadline for roster removal, and still fences a real changed provider identity. The repaired native pair passed in `e9cdcf31-c379-4707-a33b-417aa28ddbbe` and the final clean LAN selection passed again.
- **Packaged acceptance.** The inherited export stopped at `SUPPORT_SOURCE_IDENTITY src/godot-game/foundation/FoundationInput.cs` before production export. Plan 06-14 owns those source-stamp rebinds and the current packaged gameplay proof. The current authored source and imported native cases ran. Physical input and rendered appearance remain for whole-phase review.

## Self-Check: PASSED

All claimed new files were found in the pinned phase checkout. Each of the five recorded task commits is an ancestor of the tested clean code.
