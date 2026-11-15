---
phase: 03-four-player-lan-foundation
plan: "01"
subsystem: multiplayer
tags: [godot, csharp, enet, authority, headless, permissions]
requires:
  - phase: 02-playable-foundation
    provides: Production main scene, movement fixture, physical input latch and exact native boundaries
provides:
  - Identified immutable roster authority with separate intent acceptance and host elapsed stepping
  - Framework-only replaceable connection port and guarded reliable raw ENet composition
  - Coherent baseline installation and acknowledgment before guest control
  - Ordinary solo, host and manual-join controls with local actor projection
  - Error-sensitive two-process headless tracer and exhaustive exact permission controls
affects: [03-02, 03-03, 03-04, phase-05-testing]
tech-stack:
  added: []
  patterns: [trusted-sender-binding, immutable-roster-publication, bounded-explicit-json, captured-native-gateway]
key-files:
  created:
    - src/core/FoundationSnapshot.cs
    - src/core/FoundationParticipantState.cs
    - src/multiplayer-abstractions/IConnectionService.cs
    - src/multiplayer-abstractions/ConnectionPacket.cs
    - src/godot-game/foundation/FoundationCoordinator.cs
    - src/godot-game/foundation/FoundationProtocol.cs
    - src/godot-game/foundation/EnetConnectionService.cs
    - src/godot-game/foundation/FoundationAutomation.cs
    - src/godot-game/foundation/FoundationDiagnostics.cs
    - src/godot-game/foundation/FoundationEphemeralProfile.cs
    - tests/phase1/FoundationLanAuthorityTests.cs
    - tests/phase1/FoundationLanPermissionTests.cs
    - tools/phase3/check.cjs
    - tools/phase3/network.cjs
    - tools/phase3/required-cases.json
  modified:
    - src/core/FoundationSession.cs
    - src/core/FoundationMovementCommand.cs
    - src/godot-game/foundation/FoundationRoot.cs
    - src/godot-game/foundation/FoundationView.cs
    - src/godot-game/foundation/Foundation.tscn
    - tests/phase1/EngineAccessPolicy.cs
    - tests/phase1/FoundationPermissionTests.cs
    - tools/code-rules/BodyLengthAnalyzer.cs
    - tools/code-rules/EngineGeneratedSyntax.cs
    - docs/code/engine-generated-spans.json
key-decisions:
  - "Bind trusted sender and provider generation to an opaque authority binding; never trust payload ownership."
  - "Use protocol/schema 1, exact existing fixture content, reliable packets and complete immutable snapshots."
  - "Keep synthetic native input injection separate from observation and preserve focus clearing and physical-key polling."
  - "Allow only 14 actual new native-calling tuples and four public contracts, with compiler-valid neighboring rejections."
  - "Use distinct process-local temporary profile IDs until the persisted profile provider arrives in 03-03."
requirements-completed: [LAN-01, LAN-02, LAN-03, LAN-04]
coverage:
  - id: D1
    description: Identified common authority and detached bounded protocol
    requirement: LAN-03
    verification:
      - kind: unit
        ref: "node tools/phase3/check.cjs --case core.authority"
        status: pass
    human_judgment: false
  - id: D2
    description: One real host and guest using production composition without a window
    requirement: LAN-01
    verification:
      - kind: integration
        ref: "node tools/phase3/check.cjs --case network.tracer"
        status: pass
    human_judgment: false
  - id: D3
    description: Exact new public and native permissions reject adjacent misuse
    requirement: LAN-04
    verification:
      - kind: unit
        ref: "node tools/phase3/check.cjs --case core.permissions"
        status: pass
    human_judgment: false
actuals:
  tokens: 72126
  tasks: 2
  commits: 4
plan_head_before: 2037e9240cc3a606249e1a42dbfaf15bd80440b6
plan_head_after: 4802d0511f8d07c4b2cfc5f1765815d2f4d5d4b4
duration: 39min
completed: 2026-11-15
status: complete
---

# Phase 3 Plan 1: Identified ENet Tracer and Exact Permissions Summary

**Production host/manual guest movement now crosses a bounded raw ENet connection, coherent installed baseline, common pure authority and native projection, with compiler-proven exact permissions.**

## Performance and scope

- Recorded implementation window: 2026-11-15T13:21:48Z through approximately 14:01Z, 39 minutes. The start is the first implementation file's recorded creation time; initial context loading precedes that measured window.
- Two tasks completed, with RED and GREEN commits for each. The measured ledger contains four commits before this summary commit and 39 changed files.
- Actual token estimate is `ceil(288501 diff characters / 4) = 72126`, measured over the realized committed task diff, including the two RED evidence records. It is not a harness token count.
- Existing branch `feature/03-four-player-lan-foundation` and draft [PR #4](https://github.com/Sebreiro/kingdom-zombies-game/pull/4) were reused. Implementation commits were pushed to that branch; no branch/worktree change or merge occurred.
- Shared STATE, ROADMAP and REQUIREMENTS updates belong to the parent orchestrator and were not changed by this executor. The requirement IDs above identify this plan's supporting increment; complete LAN-01 through LAN-04 acceptance still requires plans 03-02 through 03-04.

## Accomplishments

- `FoundationSession.TryAccept` checks current binding, exact session/player/actor, readiness, legal direction and increasing sequence. Acceptance changes held intent without moving an actor. `Step` alone applies trusted integer elapsed time to the ready roster. The inherited `Actor`/`Advance` facade delegates to this same authority, retaining three units per second, integer subunits, facing, bounds and overflow checks.
- Immutable `FoundationSnapshot` and participant publications contain the session, revision, schema/content and distinct ownership/readiness roster. Complete detached validation precedes publication. Protocol 1 uses the exact content tuple `foundation.fixture.v1|foundation.ground.v1|foundation.building.v1|foundation.player.v1`.
- The connection port contains framework types only and is linked explicitly into the existing proof executable, alongside engine-free coordinator/protocol files. Core has no service reference, address, transient peer or discovery dependency. No project, package or project reference was added.
- The guarded ENet adapter manually polls raw reliable packets without assignment to MultiplayerAPI, obtains the sender before consuming a packet, checks native results and bounds receives. Host transport capacity is 16; application roster capacity is four. It tracks actual connected peers to avoid sending to departed native targets.
- The coordinator offers a current baseline, installs it atomically at the guest, sends acknowledgment after installation, and enables control only after the host publishes readiness. Solo, host-local and guest submission use the same acceptance/step path.
- The ordinary native scene retains Start, Quit and direct Escape, and now exposes Host session and Join LAN/manual address controls. Ready remote actors use the existing fixture; each process projects its local camera from its assigned actor.
- Defaults remain 50 ms full-state publication, immediate changed direction and 500 ms liveness messages, pending later measurement. Bounds are 32768 wire bytes, JSON depth 16, four participants, 256 Unicode scalars per label, 128 characters per opaque identifier and 64 packet receives per update. Diagnostic event history is capped at 256 entries.
- The independent three-case catalog records importance, setup, actions, expected observations and run groups. Exact comma-separated case selection, group selection and list mode reject empty/unknown/duplicate selection. Native diagnostics use fresh nonce/profile/port ownership, two separate real processes, production native controls and key events, read-only observations, bounded waits and owned cleanup.

## Task commits

| Task | Commit | Result |
| --- | --- | --- |
| 1 RED | `8f2284c` | Owned intent expectation fails against a compile-time rejection-only API shell |
| 1 GREEN | `3f51e6d` | Common authority, bounded protocol, native ENet/menu/projection and headless tracer |
| 2 RED | `c0663d5` | Compiler-valid forged gateway is incorrectly accepted by the original scanner |
| 2 GREEN | `4802d05` | Exact permission controls, captured gateway enforcement and corrected error-free teardown |

No tracked file deletions were introduced. The plan ledger is `.git/gsd-plan-head-before-03-01`; the measured before/after hashes are recorded in frontmatter.

## Verification and immutable evidence

All checks below executed on Windows x64 with the isolated pinned toolchain, without a game window or VM. Strict build used:

`dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`

It completed with zero warnings and zero errors; the final log is `.phase3/verified-build.log`.

| Final executed selection | Result | Immutable report |
| --- | --- | --- |
| `node tools/phase3/check.cjs --case core.authority,core.permissions,network.tracer` | 3/3 pass | `.phase3/runs/win32-x64/dffe49ec-8ee1-4420-95d0-a551ce96dc89/summary.json` |
| `node tools/phase1/check.cjs --case engine.source-policy` | 1/1 pass | `.phase1/runs/d7d2709c-6785-4d8c-b609-38bce31023c8/summary.json` |
| `node tools/phase1/check.cjs --case engine.generated-origin` | 1/1 pass | `.phase1/runs/b45935ff-4680-4e5c-8f2b-028282779dec/summary.json` |
| `node tools/phase1/check.cjs --case engine.generated-boundary` | 1/1 pass | `.phase1/runs/49b925bd-101b-4bd5-8b95-982384b28b11/summary.json` |
| `node tools/phase1/check.cjs --case rules.godot-callsite-boundary` | 1/1 pass | `.phase1/runs/e7336c34-5cd6-4f2a-89f1-a6a60b93250a/summary.json` |
| `node tools/phase1/check.cjs --case docs.navigation` | 1/1 pass | `.phase1/runs/5bd00f46-f54d-4ab7-9e54-1d0f518b0241/summary.json` |
| `node tools/phase1/check.cjs --case R-37.positive` | 1/1 pass | `.phase1/runs/4ab58013-d190-4c81-af11-1dc32f0cc5c4/summary.json` |
| `node tools/phase1/check.cjs --case R-37.negative` | 1/1 pass | `.phase1/runs/19f278f0-f2a8-42f7-b901-48979a9ac5e7/summary.json` |
| `node tools/phase1/check.cjs --case rules.analyzer-source-consumer` | 1/1 pass | `.phase1/runs/71a23c1a-6def-4394-ab86-421193784dd0/summary.json` |
| `node tools/phase1/check.cjs --case rules.analyzer-source-boundary` | 1/1 pass | `.phase1/runs/26e285e3-0ac8-49e4-86d5-ac0b2afc46dc/summary.json` |
| `node tools/phase2/check.cjs --group core` | 4/4 pass | `.phase2/runs/win32-x64/d4269352-163a-441a-9c35-6813d18711c2/summary.json` |

The final Phase 3 invocation took 29872 ms; its native tracer subject took 1805 ms within its 60-second subject deadline. The report preserves each actual process/profile identity, matching session/revision/actor publications, baseline-install/ack events, held direction plus actual `IsPhysicalKeyPressed` observations, and ordinary-input exits. Both final native logs contain only the engine banner, with no native errors or exceptions.

The final run's input fingerprint is `92a85f1a575166dd87f33bcacdbba6c3e95f06e9acc1645060027f9dd313e242`; its tool fingerprint is `7b7f76415b0522c9c6f2437684d87beed9b05d4a2dc4c638d2dc01afc01cd118`. Tests ran immediately before the final feature commit, so the recorded HEAD is `c0663d5` plus the working-tree input fingerprint. A post-commit check confirmed that the same fingerprint exactly matches committed `4802d05` inputs. This is not a claim of a separately executed post-commit suite.

Fresh selector controls also accepted reordered valid IDs, `--group core` and `--list`, and rejected six empty/unknown/duplicate selections; output is `.phase3/selectors-final.log`. `git diff --check` passed.

## TDD gate compliance

- Task 1's actual JUnit assertion failed because owned nonzero intent returned false. Its rejection-only signature shell contained no intent implementation. The unchanged report and semantic assessment are preserved in `tdd/03-01-task1-red.json`; machine classification returned `RED_EVIDENCE_OK` before GREEN.
- Task 2's actual positive production subject compiled and passed, then the fresh-gateway mutation compiled and failed the explicit expected scanner-rejection assertion. The unchanged report and semantic assessment are preserved in `tdd/03-01-task2-red.json`; machine classification returned `RED_EVIDENCE_OK` before GREEN.
- An earlier Task 2 attempt omitted `PHASE1_ROOT` and failed during setup. Although the format classifier recognized a failing test, semantic review rejected that attempt; it did not authorize implementation. The valid rerun supplied the required root.
- Both RED commits precede their GREEN commits. No separate refactor commit was needed. The final strict build, named authority and permission targets, native integration and affected regression selection pass.

## Exact boundary results

The new permission case independently enumerates 14 actual native-calling tuples and four public contracts. Every tuple first passes with its real production source and rejects removal of its actual guard; it also has a positive synthetic equivalent plus missing/late/branch-only/fresh-forged guard, method-group, worker-lambda, adjacent method and wrong-path controls through real compilations. The independent complete inventory rejects untested tuple expansion. Existing namespace/type/async and authenticated generated/source/callsite controls remain active and passed.

The captured guard check accepts the readonly native gateway, an unreassigned injected gateway parameter, and the exact inherited `ProofSequence.Gateway` singleton. A newly constructed gateway cannot establish native thread authority. Unused new wrapper permissions for `Host` and `FoundationAutomation.Tick` were removed.

Actual strict-build output required 220 authenticated generated spans, replacing the old root's rows while preserving unrelated rows and package/source/hash/span authentication. The generated 91-line dispatch method has one exact SCR0006 span, using the same authenticated lookup mechanism as the other analyzers. Ordinary 61-line source still fails. Six enclosing-member metadata annotations were corrected from actual declarations; they did not alter source hashes or spans.

## Deviations and resolved issues

1. **[Rule 3 - Blocking] Exact generated body permission.** The larger real Godot dispatch method exceeded 60 lines, while BodyLengthAnalyzer did not previously consult the existing exact generated-span mechanism. Added that bounded lookup, recorded the actual emitted span, and proved ordinary body rejection, producer authentication and adjacent span rejection. Files: `BodyLengthAnalyzer.cs`, `EngineGeneratedSyntax.cs`, generated registry/count metadata and `EngineGeneratedTests.cs`. Commits: `3f51e6d`, annotation correction in `4802d05`.
2. **[Rule 1 - Bug] Fresh gateway forgery.** The scanner authenticated the method name/type but accepted a newly captured gateway. Added receiver provenance and exact inherited singleton handling, with actual positive and adjacent negative subjects. Commit: `4802d05`.
3. **[Rule 1 - Bug] Native teardown errors hidden by zero process exit.** Raw log inspection invalidated initial tracer passes as clean-teardown evidence. Added explicit native error-log rejection and paired host/client peer-event lifetimes; sends to departed native targets are suppressed. The stricter check caught an intermediate unmatched client unsubscribe, which was repaired before acceptance. Commit: `4802d05`.
4. **[Rule 2 - Correctness] Distinct ordinary temporary identities.** Replaced a shared ordinary fallback player ID with a distinct process-local identity, while preserving explicit diagnostic-owned profiles. Persisted profiles remain assigned to 03-03. Commit: `4802d05`.

Small helper files hold accepted intent, peer binding, endpoint parsing, temporary profile identity and read-only diagnostics without changing project inventory or introducing a service-to-Core reference. The exact generated callsite hashes and count metadata were updated from actual output. No analyzer was disabled and no wildcard permission or package was introduced.

## Historical failures and evidence limits

- Early tracer reports including `27d930ab-c914-408c-b889-4f1121f75644`, `553a174e-3f08-4037-ab4c-dfcee92d0b18` and `bab53205-3c33-48c3-b445-7918a819ae48` reported pass before native error-log rejection existed. Their raw logs invalidate clean-teardown acceptance; they are retained as historical attempts, not reused passes.
- The stricter tracer's intermediate failure is `.phase3/runs/win32-x64/d7db93fd-7cdc-453d-9902-6946d74e80f1/summary.json`. Error-free focused repair evidence is `9b7673e0-1246-4956-971d-cae5748f4c29`; the final full selected run is `dffe49ec-8ee1-4420-95d0-a551ce96dc89`.
- Initial generated-origin metadata failure: `.phase1/runs/c81921da-a1a0-48c8-b7c2-a105d15d2dab/summary.json`. Corresponding initial Phase 2 core failure: `.phase2/runs/win32-x64/aa1487a1-f68f-415c-9aee-40b4b2e96552/summary.json`. Both were corrected and rerun in the final selection.
- One affected analyzer boundary attempt was blocked by an overlapping proof-DLL lock: `.phase1/runs/7494545a-365b-4f96-af47-eec221e85118/summary.json`. The owning process had exited before the sequential rerun. Earlier overlapping phase1 invocations were explicitly refused by its check lock. These attempts receive no pass credit.
- All required plan checks executed successfully after repairs. No skipped test or unrun required verification remains. Stub scanning found only the functional address field's example `placeholder_text`, not unfinished behavior.
- Headless key delivery is explicitly `synthetic-native-ParseInputEvent`. It proves the native handler/latch/poll/authority path, not physical keyboard input, rendering, 250 ms responsiveness, measured four-player synchronization or physical LAN acceptance.

## Threat flags

| Flag | Files | Description |
| --- | --- | --- |
| threat_flag: local-diagnostic-file-access | `FoundationAutomation.cs`, `FoundationDiagnostics.cs` | Nonce-scoped diagnostic requests and observations use environment-selected local directories. The threat register did not separately name this local file boundary. Paths are not received from network messages; diagnostics are inactive without explicit local environment configuration. |

## Next plan readiness

03-02 can expand the same authority/coordinator into complete admission, timeout, departure and stale-lifetime semantics. Peer transport tracking currently avoids native send errors; full application reservation/actor removal and loss behavior remain 03-02 work. The process-local profile helper is intentionally temporary pending 03-03's independent persisted identity provider and discovery/browser expansion. Those are planned increments, not certified behavior here.

03-04 retains whole-phase review, full selected rendered scenarios, four-peer timing/input evidence, runner adversarial expansion and actual visual review. No real game window, VM, Linux run, hosted CI, mixed-computer LAN, packaged release or endurance acceptance was performed by this plan. Preserve the phase's existing broader evidence obligations and historical Phase 1 follow-ups.

No external service setup or authentication action was required.

## Self-Check: PASSED

The summary and all 39 committed plan paths exist. All four recorded task commits are ancestors of HEAD, the GSD evaluation scope resolves those four commits, and `git diff --check` is clean. The final evidence fingerprint matches the committed implementation, and both final native logs are error-free. Shared phase state updates remain with the parent orchestrator.
