---
phase: 06-survivors-inventory-and-combat
plan: "09"
subsystem: multiplayer-authority
tags: [godot, csharp, lan, protocol, privacy, authority]
requires:
  - phase: 06-04
    provides: "Current reachable inventory views and owned transfer authority"
  - phase: 06-07
    provides: "Retired control, ordered Respawn batches, and continued transport roster"
  - phase: 06-08
    provides: "Private durable run snapshots and action sequence continuation"
provides:
  - "One authoritative gameplay dispatcher for solo, host-local and authenticated guest actions"
  - "Version-two LAN gameplay frames, bounded recipient windows and reliable scope revocation"
  - "Atomic guest render readback with correlated private results and current control"
  - "Actual encoded four-role privacy, denial and compatibility evidence"
affects: [06-10, 06-11, 06-12, 06-13, 06-15]
tech-stack:
  added: []
  patterns: [stable-transport-binding, host-validated-control-epoch, bounded-recipient-windows]
key-files:
  created:
    - src/godot-game/core/RunProjections.cs
    - src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs
    - src/godot-game/game/foundation/FoundationRunFrame.cs
    - src/godot-game/game/foundation/FoundationRunReadback.cs
    - src/godot-game/game/foundation/FoundationRunView.cs
    - src/godot-game/game/foundation/FoundationViewInbox.cs
    - src/godot-game/tests/verification/Phase06PublicationTests.cs
  modified:
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunItemActions.cs
    - src/godot-game/game/foundation/FoundationCoordinator.cs
    - src/godot-game/game/foundation/FoundationCoordinator.Admission.cs
    - src/godot-game/game/foundation/FoundationCoordinator.GuestPublication.cs
    - src/godot-game/game/foundation/FoundationCoordinator.Lifetime.cs
    - src/godot-game/game/foundation/FoundationEnvelope.cs
    - src/godot-game/game/foundation/FoundationMenu.cs
    - src/godot-game/game/foundation/FoundationPeerBinding.cs
    - src/godot-game/game/foundation/FoundationProtocol.cs
    - src/godot-game/tests/verification/FoundationHarnessConnection.cs
    - src/godot-game/tests/verification/FoundationLanAuthorityTests.cs
    - src/godot-game/tests/verification/FoundationLanOrderingTests.cs
    - src/godot-game/tests/verification/FoundationLanProtocolTests.cs
    - src/godot-game/tests/verification/FoundationLinkedSources.cs
    - src/godot-game/tests/verification/FoundationServiceSubstitutionProof.cs
    - src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj
key-decisions:
  - "Derive peer lifetime and ready binding from the provider and check the current survivor or death epoch before the action sequence advances."
  - "Keep durable run snapshots private and publish curated world facts and separate recipient windows."
  - "Measure real UTF-8 envelope length with space reserved for the native frame header."
  - "Keep new action, projection and receive models internal under the existing R-34 inventory."
requirements-completed: [INV-08, INV-09, INV-10, INV-11, RUN-01, RUN-02, RUN-03, RUN-04, COMBAT-03]
actuals:
  tokens: 39219
  tasks: 3
  commits: 6
commits: 6
plan_head_before: 7d5dfa542edca4681be2f5153f8ca899693a477c
plan_head_after: c10cb0310d715028fc310638f834f3de43cadfe5
duration: 48min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 09: LAN Authority and Recipient Publication Summary

**Solo and LAN players submit the same host-validated gameplay actions. A living survivor’s control epoch fences commands, while a spectator keeps the ready transport binding and can request a successor. Private inventory windows cross the wire as scoped, bounded parts.**

## Contract for dependent plans

- Use `FoundationCoordinator.SubmitRunMotion(direction, sprint, jump)` for normal movement and `SubmitRunAction(kind, request)` for other gameplay. A local dispatch returns the committed reason. A guest receives `Pending` and a sequence, then reads the authoritative reason and replay status from `RunActionFeedback`. The host derives the ready binding and player from the provider, validates the expected survivor or death epoch, and rejects stale high sequences before they can suppress a current input.
- For attacks send the `attack` action with `AimX` and `AimY`. Both null requests melee. Food uses `food` with `Item` containing the projected `Id` and `Stamp`, plus a nullable `QuickSlot`. Inventory selection takes `TargetKind` and `TargetId`. Transfers use the same projected item stamp and live selection token. The authority rechecks exact current path, contents, reachability and ownership when it commits a transfer.
- Call `ReadRunRender()` for an immutable roster, curated world projection, recipient views and action feedback. The world includes elapsed time, ground, camp edges, living actors and their visible equipment slots, defenses, physical loot anchors, and arrows. `LocalRunControl` exposes the current survivor, living generation and most recent death generation. The existing roster and heartbeat keep the same opaque transport actor token when a successor gets a new living generation.
- `RequestRunWindows(ownGrid, externalGrid)` asks for independent authorized nested windows. Null requests the scope root. The `status`, `own` and `external` views carry their own revision, epoch, selection token, serial, action correlation, header and complete rows. A readback installs complete windows. The public cadence can advance while an otherwise current private window is receiving parts. Reliable revocations, actor changes, supersession and part expiry fence late data.
- Submit `respawn` with an empty request. The coordinator batches ready local and peer requests from the same update at the same host run timestamp. The existing authority orders waiting time, then player slot, and returns results in input order. The full durable snapshot codec stays in Core.
- Protocol version 2 retains the version-one detached roster schema and the established content tuple. The current browser advertisement names version 2. An incompatible gameplay protocol is rejected before admission. The public world and roster share a commit when the world fits the frame budget. An oversized world publication retains the roster and reliable scope updates for that tick.

## Task commits

| Task | Commit | Change |
| --- | --- | --- |
| 1 RED | `9743026` | Required one ready-binding gameplay dispatcher |
| 1 GREEN | `706d713` | Routed a real transfer and scoped correction through the unchanged pre-wire admission |
| 2 RED | `f12b33f` | Required the version-two compatibility epoch |
| 2 GREEN | `0e0709a` | Added bounded gameplay parts, scope revocation and native command ingress |
| 3 RED | `f38d81b` | Reproduced lost action correlation on installed recipient readback |
| 3 GREEN | `c10cb03` | Published atomic readback and proved real four-role privacy, correction and succession |

## Verification

Final evidence ran on clean revision `c10cb0310d715028fc310638f834f3de43cadfe5`, Windows x64, Node 24.22.1, .NET SDK 10.0.404 with runtime 10.0.15, and Godot 4.7.2 .NET. The Node executable was `D:/Work/Games/KingdomZombies/.local/tools/node/node-v24.22.1-win-x64/node.exe`.

| Command or selection | Evidence | Result |
| --- | --- | --- |
| `node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build` and direct pre-wire filter of `Phase06PublicationTests`, `FoundationLanAdmissionTests` and `FoundationLanProtocolTests` | Strict build `6eabc69e-8f40-41b4-80f3-cf04776e21ca`, direct report `.local/runs/06-09-tdd/task1-committed/tests.json` | Three tests on clean pre-wire commit `706d713` passed with zero failures or skips. |
| `node src/godot-game/tools/game-tests/check.cjs --pr` | `98f83b64-4967-469e-829b-463093f0989e` during Task 2 and `f9f7d9d3-d641-4d7e-a5ab-d31a9e5efea2` on final clean code | Both exited 0 with four objective passes. Final run took 44.954 seconds and passed strict build, four repository checks, 149 pure tests and native headless solo. |
| Direct final filter of `Phase06PublicationTests`, `FoundationLanAdmissionTests` and `FoundationLanProtocolTests` | `.local/runs/06-09-tdd/final-committed/tests.json` and named cases under `.local/runs/06-09-tdd/final-committed/cases/` | Nine passed in 2.431 seconds with zero failures or skips, including both exact P02 controls. |
| Direct filter of `FoundationLanAuthorityTests`, `FoundationLanLifecycleTests`, `FoundationLanOrderingTests`, `FoundationLanInputTests`, and `FoundationLanServicesTests` | `.local/runs/06-09-tdd/final-compatibility/tests.json` | Seven passed in 1.089 seconds with zero failures or skips. |
| Exact R-34 and linked original snapshot mutation proofs | `.local/runs/06-09-tdd/final-boundaries/tests.json` | Four passed in 52.864 seconds. Exact visibility, adjacent-type denial, the actual production inventory scan, and the isolated original snapshot mutation all passed. |

The focused test assembly is `src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll`. The final direct run used `--filter-class '*Phase06PublicationTests' '*FoundationLanAdmissionTests' '*FoundationLanProtocolTests' --minimum-expected-tests 9 --report-xunit-ctrf --report-xunit-ctrf-filename tests.json --results-directory .local/runs/06-09-tdd/final-committed --progress off --no-ansi --show-test-results all` with `PHASE1_ROOT` pinned to the phase checkout.

The actual 64 by 64 stash page yielded 4,096 distinct rows in 54 parts. Its largest application packet was 32,723 bytes and its largest native encoded frame was 4,380 bytes. The proof delivered reordered and duplicate parts, a malformed part, a newer public tick, a timed-out series and delayed data after revocation. The same P02 byte and permission comparator admitted each actual owner’s scoped view and detected both a misrouted private payload and an unauthorized stash observation. The real remote selection and contested losing transfer returned `AccessLost` and current allowed readback. A ready guest retained heartbeat after lethal damage, entered the waiting queue, took the later released recruit, and rejected the original epoch without suppressing the next current move.

The native solo process was PID `25356`, nonce `a56dd290-bfc0-47d5-85ca-506a52c2556b`. The Hint moved from outside camp, danger near and stash blocked at −8000, elapsed 90 ms and revision 1, to inside camp, danger clear and stash reachable at −6488, elapsed 649 ms and revision 78. Ordinary Quit was acknowledged at frame 150. It exited 0 with closed streams and no cleanup kill. Multi-role transport proof used the actual codec and coordinators in memory. The native multi-peer scenarios belong to 06-13 onward.

### Original failures

- Task 1 strict RED build `f85d3da0-5eb6-4280-a58c-d4c527daa6ce` passed and the dispatcher test exited 2 because `DispatchBoundRunAction` was absent. Task 2 strict RED `752f2c1e-3504-4b66-9246-60c4cab681ec` passed and its codec test exited 2 after observing protocol 1. Task 3 strict RED `fa99d5cb-cd5f-406c-85bf-4afde22ec71d` passed and its real admitted view exposed no correlation. All three JUnit reports returned `RED_EVIDENCE_OK`.
- Strict development builds caught declaration order, anonymous object formatting, indentation, and name or method size rules. Shared PR run `3e052112-b9ea-4591-ab79-63562b105466` passed three of four checks and found one inherited delivery fixture forwarding only the public ready state. The fixture now also delivers the real reliable control status before sending a typed guest command. Its prior reliable/unreliable and stale-state assertions still execute.
- Task 3 build `f6f504b4-eb4f-48d2-9be9-f9b7fdc51deb` caught four line lengths. Build `39806f3c-b35c-4c6d-a782-3cce33b15dd3` passed, and its focused run exposed a missing proof artifact directory and a spectator fixture that tried to set the locked eight-survivor start to four. The final proof creates the evidence directory and uses genuine lethal transitions to leave four controlled survivors before testing the queue.

## Deviations from plan

- [Rule 2] The transport implementation uses a separate coordinator partial and internal frame, window, readback and inbox types. Linked verification sources and the isolated build inventory include those exact files. Core exposes framework-value methods only on the already approved `FoundationSession` type. The R-34 registry and assembly access policy are unchanged.
- [Rule 2] The existing discovery advertisement now uses gameplay protocol version 2. The existing inventory selection helper supports a cheap current reachability check before private publication, so world ticks need no whole-grid scan. Packet rejection events use fixed reasons.
- [Rule 3] The inherited ordering, delivery, service substitution and wrong-version controls received the matching v2 fixtures while retaining their original assertions. The connection harness records the actual outbound bytes in addition to its decoded view.

The phase orchestrator owns the shared worktree, roadmap and state updates, draft pull request, and output cleanup. [Draft pull request #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16)

## Self-Check: PASSED
