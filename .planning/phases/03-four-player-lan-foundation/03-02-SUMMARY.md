---
phase: 03-four-player-lan-foundation
plan: "02"
subsystem: multiplayer
tags: [godot, csharp, enet, admission, ordering, lifetime, headless]
requires:
  - phase: 03-01
    provides: Identified common authority, detached snapshots, guarded raw ENet and native tracer
provides:
  - Four-participant serialized admission including pending reservations and retained attempt outcomes
  - Detached complete roster validation and baseline acknowledgment before ready control
  - Sender/session/attempt/actor/sequence fencing before accepted intent mutation
  - Exact monotonic join and response-loss deadlines with response-first update ordering
  - Guest departure/loss, fresh replacement, bounded host end and fresh rehosting
  - Actual no-window four-peer race, lifecycle and idle-provider checks
affects: [03-03, 03-04, phase-05-testing]
tech-stack:
  added: []
  patterns: [bounded-admission-outcomes, monotonic-deadlines, closed-lifetime-fence, bounded-end-drain]
key-files:
  created:
    - src/godot-game/foundation/FoundationCoordinator.Admission.cs
    - src/godot-game/foundation/FoundationCoordinator.GuestPublication.cs
    - src/godot-game/foundation/FoundationCoordinator.Lifetime.cs
    - src/godot-game/foundation/FoundationRuntimeState.cs
    - tests/phase1/FoundationHarnessConnection.cs
    - tests/phase1/FoundationLanAdmissionTests.cs
    - tests/phase1/FoundationLanOrderingTests.cs
    - tests/phase1/FoundationLanProtocolTests.cs
    - tests/phase1/FoundationLanLifecycleTests.cs
    - tests/phase1/FoundationScenarioClock.cs
  modified:
    - src/core/FoundationSession.cs
    - src/core/FoundationSnapshot.cs
    - src/godot-game/foundation/FoundationCoordinator.cs
    - src/godot-game/foundation/FoundationProtocol.cs
    - src/godot-game/foundation/FoundationEnvelope.cs
    - src/godot-game/foundation/FoundationPeerBinding.cs
    - src/godot-game/foundation/EnetConnectionService.cs
    - src/godot-game/foundation/FoundationRoot.cs
    - src/godot-game/foundation/FoundationView.cs
    - src/godot-game/foundation/FoundationAutomation.cs
    - src/godot-game/foundation/FoundationDiagnostics.cs
    - tests/phase1/KingdomZombies.Phase1.Tests.csproj
    - tools/phase3/network.cjs
    - tools/phase3/required-cases.json
key-decisions:
  - "Retain at most 128 sender/attempt outcomes; pending reservations consume the four-participant capacity."
  - "Validate detached complete rosters and current ownership before publication, readiness or sequence mutation."
  - "Use injected monotonic milliseconds: 30000 for complete joining and 10000 for valid-response loss."
  - "Process valid responses before expiration; duplicate/stale/malformed work cannot refresh liveness."
  - "Close acceptance and increment lifetime before native release; graceful host end drains for at most 250 milliseconds."
  - "Keep runtime clock/feedback in a framework-only helper, preserving exact existing generated/native permissions."
requirements-completed: [LAN-01, LAN-02, LAN-03, LAN-04]
coverage:
  - id: D1
    description: Bounded admission and immutable ordered authority/publication controls
    requirement: LAN-03
    verification:
      - kind: unit
        ref: "node tools/phase3/check.cjs --case core.admission,core.ordering,core.protocol"
        status: pass
    human_judgment: false
  - id: D2
    description: Actual four ready peers and simultaneous final-slot challengers
    requirement: LAN-01
    verification:
      - kind: integration
        ref: "node tools/phase3/check.cjs --case network.four-peer"
        status: pass
    human_judgment: false
  - id: D3
    description: Exact deadlines, teardown, replacement and rehosting
    requirement: LAN-01
    verification:
      - kind: unit
        ref: "node tools/phase3/check.cjs --case core.lifecycle"
        status: pass
      - kind: integration
        ref: "node tools/phase3/check.cjs --case network.lifecycle,network.idle-poll"
        status: pass
    human_judgment: false
actuals:
  tokens: 28656
  tasks: 2
  commits: 5
plan_head_before: ac63e96a68303a581ffc6a3e87096da6d1739d6e
plan_head_after: cba5cf311d78c73b3a63afa318b95c7349435871
duration: 29min
completed: 2026-11-15
status: complete
---

# Phase 3 Plan 2: Admission, Ordering and Session Lifetimes Summary

**Four ready participants now share bounded ENet admission and coherent authoritative publications, with exact timeout boundaries, fresh replacement ownership and closed-session teardown fences.**

## Performance and scope

- Recorded implementation window: first task file creation at 2026-11-15T14:07:16Z through summary preparation at approximately 14:36Z, 29 minutes. Initial context loading precedes this measured window.
- Two tasks completed. The persisted `.git/gsd-plan-head-before-03-02` ledger measures five task commits and 26 changed files before this summary commit.
- Actual token estimate is `ceil(114624 realized diff characters / 4) = 28656`, including the two preserved RED evidence records. This uses the estimate's diff-character scale, not harness token usage.
- Reused `feature/03-four-player-lan-foundation` and draft [PR #4](https://github.com/Sebreiro/kingdom-zombies-game/pull/4). Task checkpoints were pushed to that branch. No branch/worktree change, merge, VM or real game window occurred.
- The requirement IDs above identify this supporting increment. Complete LAN-01 through LAN-04 acceptance still requires 03-03/03-04; these fields do not certify discovery, persisted profiles, rendered behavior or synchronization timing.
- Shared STATE, ROADMAP and REQUIREMENTS updates remain with the parent orchestrator and were not edited by this executor.

## Accomplishments

- Application capacity is four including pending participants, independently of transport capacity. A repeated sender/attempt returns its original baseline or refusal. Duplicate profiles and full sessions have distinct reasons, incompatible protocol/content is explicit, and up to 128 retained outcomes prevent unbounded admission-history growth. A cancelled/expired attempt becomes a closed outcome; a new attempt gets new binding/actor IDs.
- Baselines capture immutable complete state. Detached protocol mapping checks bounded bytes/depth, exact shape/content/schema/session/revision, unique bindings/players/actors/slots, an existing ready host and legal actor values. The guest installs ownership atomically and acknowledges the intended baseline. Matching duplicate acknowledgment is idempotent; the host publishes its newest complete ready roster after acknowledgment while existing actors continue stepping.
- Actual sender, provider generation, attempt, session, binding, player, actor and readiness precede accepted intent. Invalid high sequences cannot advance the high-water mark. Solo/host-local/guest retain the same pure authority and held-intent/elapsed-step separation. Tests prove rejection before publication changes, repeated delivery without extra movement, and old movement failing to undo stop/reversal.
- Coordinator deadlines use injected monotonic milliseconds, sampled in production from guarded `Time.GetTicksMsec`. Joining lasts at most 30000 milliseconds from the original attempt through readiness; baseline installation does not restart the deadline. A ready binding's loss threshold is 10000 milliseconds since a valid current response. Received valid work runs before expiration at the same timestamp. Stale snapshots, repeated heartbeat sequences, duplicate acknowledgments, malformed data and obsolete adapters cannot refresh liveness.
- Teardown closes acceptance and advances the lifetime fence before cancellation/native release. It clears authoritative held input, pending ownership, remote projections and local binding. Host end sends session/binding/attempt-specific messages and accepts only matching end acknowledgments during an at-most-250-millisecond drain. Unexpected loss has a distinct production explanation. Fresh coordinators rehost with fresh session and actor identities; closed coordinators cannot restart.
- Graceful/lost guest removal deletes exactly the owning authority binding and held input, publishes a new roster and releases one slot. Native projection now removes departed remote nodes as well. Remaining actor state/input survives; slot reuse receives a fresh actor/binding. ENet sends flush queued packets, suppress sends to departed peers, and stop polling an internally disconnected provider.
- The catalog now contains ten exact no-window cases. New saved cases cover admission, ordering, protocol, lifecycle, actual four-peer race, actual lifecycle and idle polling. The five-process race uses ordinary host/manual-join native controls, two simultaneous final-slot challengers, current complete matching revisions and explicit loser refusal. Cleanup targets only invocation-owned children.

## Task commits and TDD gate compliance

| Task | Commit | Result |
| --- | --- | --- |
| 1 RED | `00db2b0` | Repeated hello fails the expected original-baseline count assertion |
| 1 GREEN | `46a99a6` | Four-participant admission, current baseline/ack, ordering/protocol controls and native race |
| 1 repair | `e516cb2` | Retry transient nonce-local diagnostic reads without native error logs |
| 2 RED | `2b8e270` | Closed coordinator fails the expected no-projection assertion |
| 2 GREEN | `cba5cf3` | Monotonic deadlines, lifetime fencing, cleanup, replacement, end/loss/rehost and native checks |

Both actual named RED targets compiled, executed and failed on the planned assertion. Their unmodified JUnit documents, actual exit code 2, timestamps, expected/actual results and semantic assessments are preserved in `tdd/03-02-task1-red.json` and `tdd/03-02-task2-red.json`. Each returned `RED_EVIDENCE_OK` before GREEN. An initial unsupported `--report-junit` reporter invocation authorized no implementation; the installed `--report-xunit-junit` reporter supplied the valid evidence. No package was installed.

The first Task 1 GREEN commit was pushed while its final rerun had exposed an intermittent diagnostic read error. It is an intermediate checkpoint, not accepted clean-native evidence. The error was treated as blocking, repaired in `e516cb2`, and the complete automated tracer gate passed in `d02c4a34-fb20-4ad7-b726-612319957118` before Task 2 expansion. No separate refactor commit was needed. No tracked deletions were introduced.

## Final verification and immutable evidence

All checks executed on Windows x64 with the isolated pinned toolchain, without a real game window or VM. The strict solution build completed with zero warnings and zero errors using:

`dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`

The saved final log is `.phase3/03-02-strict-build.log`. Builds and checks ran sequentially to avoid the inherited proof-DLL lock.

| Executed selection | Result | Immutable report |
| --- | --- | --- |
| All ten current Phase 3 no-window cases | 10/10 pass | `.phase3/runs/win32-x64/9e97aa05-8868-4f80-8141-72d3f5adcc33/summary.json` |
| `engine.source-policy` | 1/1 pass | `.phase1/runs/bf5bf731-1e39-4a5c-ad88-fa5a5ef995ec/summary.json` |
| `engine.generated-origin` | 1/1 pass | `.phase1/runs/631fb83e-bccb-4a13-aab6-e2f19b418f1b/summary.json` |
| `engine.generated-boundary` | 1/1 pass | `.phase1/runs/b21279c6-d200-485b-a4c1-4aa69f5f31e8/summary.json` |
| `rules.godot-callsite-boundary` | 1/1 pass | `.phase1/runs/bab93386-6835-4499-8553-8933463e013c/summary.json` |
| `graph.visibility-inventory` | 1/1 pass | `.phase1/runs/6e3bcb15-4ee0-4d8f-9d63-3fba0eba0ae8/summary.json` |
| `graph.allowed-composition` | 1/1 pass | `.phase1/runs/dc799303-9440-4bd6-a0cc-ef0521f68ee8/summary.json` |
| `rules.source-format` after repair | 1/1 pass | `.phase1/runs/353b137b-102a-4fa7-9a68-adbcbd4c7822/summary.json` |
| `R-37.positive` | 1/1 pass | `.phase1/runs/c579bd02-494f-4e3f-8e7c-c6e6cecb21d2/summary.json` |
| `R-37.negative` | 1/1 pass | `.phase1/runs/efc5f6c9-3d2d-4222-b0ec-8a38740e95e5/summary.json` |
| `docs.navigation` | 1/1 pass | `.phase1/runs/3fb7bb01-4c02-4a20-bb72-1fed35c61038/summary.json` |
| Phase 2 `--group core` after repair | 4/4 pass | `.phase2/runs/win32-x64/79d607d2-d741-421a-81d7-5b9631f78d59/summary.json` |

The exact final Phase 3 command was:

`node tools/phase3/check.cjs --case core.authority,core.permissions,core.admission,core.ordering,core.protocol,core.lifecycle,network.tracer,network.four-peer,network.lifecycle,network.idle-poll`

It took 78196 milliseconds overall. Individual native subjects stayed within their 60-second subject budgets: tracer 1726 ms, four-peer race 4010 ms, lifecycle 34436 ms, and idle polling 2812 ms. Native logs were checked for errors/exceptions and are clean; ordinary exits were zero, while the specifically owned guest/host termination subjects record their intentional termination separately.

The lifecycle subject observed graceful leave, same-profile replacement with fresh actor and reused slot, guest response loss, fresh replacement, graceful host end, fresh host session, host response loss, and usable solo controls afterward. Its observed guest/host loss waits were 10047/10053 ms; exact threshold behavior comes from the controlled 29999/30000/30001 and 9999/10000/10001 assertions, including valid response processing at the threshold. Idle polling produced 291 updates across 2001 engine milliseconds, with a maximum observed update gap of 7 ms. These are headless provider observations, not rendered synchronization acceptance.

Final report revision is committed `cba5cf311d78c73b3a63afa318b95c7349435871`. Input fingerprint `7986a337c486d5d32da5539204b01aa6874b7f001d864d1ced2fa8d581da6b40` was recomputed after the run and matches the current committed inputs. Tool fingerprint is `abdd18df108edd532d5f320bd0ebead0ab03956dab69f7b2469814219f09c4df`. Exact permission checks passed on these final inputs: the existing fourteen new native tuples and four public contracts remain unchanged, and no generated span or permission expansion was needed.

## Deviations and resolved issues

1. **[Rule 1 - Bug] Diagnostic request race.** Native error-sensitive runs caught transient sharing errors while reading the nonce-local automation request. Atomic driver writes plus retry of a transient read failure repaired the path. Failed reports `27cca1ca-6539-4a18-ae5d-4ff7585dfb3e` and `465f776e-5dab-464b-94cd-a71c8a190107` remain under `.phase3/runs/win32-x64/`; the latter followed an earlier pass and is not erased or reused as passing evidence. Commit: `e516cb2`, with the atomic request change in `46a99a6`.
2. **[Rule 2 - Correctness] Native departed placeholders.** Inspection found that correct authority removal alone left old remote scene nodes. Added removal inside the existing guarded `FoundationView.ProjectRoster` tuple and required the real lifecycle subject to observe the reduced native actor count on remaining peers. Commit: `cba5cf3`.
3. **[Rule 3 - Blocking] Intentional termination classified as a crash.** The first lifecycle aggregate (`b560bc89-f49d-4476-a375-1480e7e0aebc`) passed controlled lifecycle and idle polling, but its process guard incorrectly treated the deliberately terminated guest as an unexpected exit. The repaired guard recognizes only invocation-owned expected terminations; other early exits still fail. Focused repair report `abb8a03b-4cb2-4417-8127-c8e16c5e713c` passed. Commit: `cba5cf3`.
4. **[Rule 3 - Blocking, parent-assigned] Existing working-copy line ending.** Source-format report `.phase1/runs/b0a98c26-51b4-4683-8e81-5218aa72467c/summary.json` found one CR byte at position 5332 in untouched `tests/phase1/FoundationMovementTests.cs`. The parent explicitly assigned its narrow repair. Removed only that byte to meet `.editorconfig`/`SourceFileFormat` LF rules. Raw normalized file hash `3a339ffb277bb4eeb868a0881a95b1de79c135cc` equals the existing HEAD blob; refreshing the index produced no tracked content change or extra commit. Reran source-format, strict build, Phase 2 core and the entire Phase 3 selection. Earlier full Phase 3 pass `d300d5f4-c8e0-4525-9297-ae7b78e4e556` and Phase 2 pass `93e236c8-2426-428e-9c10-aee82192f8d4` remain earlier-input evidence, superseded by the final reports above.

Runtime coordinator responsibilities use cohesive partial files and small framework-only test/runtime helpers rather than enlarging one source body. An intermediate strict build correctly rejected native-generated spans after primitive root fields were introduced; moving clock/feedback into the framework-only runtime helper preserved the authenticated existing generated output instead of expanding permissions. No package, project, project reference, analyzer suppression or broad permission was introduced.

## Remaining scope and evidence limits

- No required plan verification remains unrun, no test is skipped, and stub scanning found no unfinished behavior in the changed production path. Empty pre-admission fields and queue initialization represent real lifecycle states. No new trust-boundary surface beyond the plan's network/admission/lifetime scope was added; the inherited nonce-local diagnostic file boundary remains described in 03-01.
- 03-03 still owns persisted local profiles, basic discovery/browser completion and separate service substitutions. Temporary process-local/diagnostic profile IDs remain intentional until that plan.
- 03-04 still owns whole-phase review/fixes, expanded runner integrity controls, rendered Windows 1280x720 scenarios and actual input/timing/camera inspection. Headless synthetic native events do not prove physical keyboard behavior, rendered responsiveness, the 250 ms remote-change limit or stopped-position timing acceptance.
- No Linux, hosted CI, mixed-computer LAN, packaged release or endurance acceptance is claimed. No host migration, campaign recovery or persistence was introduced. Broader release and historical Phase 1 follow-ups retain their existing ownership.
- No external configuration or authentication action was required. The parent owns shared progress-state updates and keeping the existing draft PR aligned with the remaining phase work.

## Self-Check: PASSED

All 26 task-diff paths and this summary exist. All five recorded task commits are ancestors of HEAD; the SDK evaluation scope resolves those same five commits. The ledger measures five commits from the stated base to the stated final task HEAD, `git diff --check` is clean, the exact phase branch guard passes and the branch is not protected. Final input fingerprint matches the checked committed implementation; final native logs are error-free. No shared STATE/ROADMAP/REQUIREMENTS edit was made.
