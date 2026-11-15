---
phase: 02-playable-foundation
reviewed: 2026-11-15T12:08:11Z
depth: standard
files_reviewed: 44
files_reviewed_list:
  - .gitignore
  - docs/code/engine-generated-spans.json
  - docs/code/exemptions.json
  - docs/code/generated-callsite-adaptations.json
  - docs/code/register.json
  - docs/code/visibility-exemptions.json
  - docs/workflow/ai-collaboration.md
  - docs/workflow/foundation-checks.md
  - docs/workflow/pre-phase5-testing.md
  - docs/workflow/verification.md
  - src/core/FoundationActorState.cs
  - src/core/FoundationMovementCommand.cs
  - src/core/FoundationSession.cs
  - src/godot-game/GeneratedCallSiteAdapters.cs
  - src/godot-game/foundation/Foundation.tscn
  - src/godot-game/foundation/FoundationInput.cs
  - src/godot-game/foundation/FoundationRoot.cs
  - src/godot-game/foundation/FoundationView.cs
  - src/godot-game/foundation/FoundationWorld.tscn
  - src/godot-game/project.godot
  - tests/phase1/ArchitecturePolicy.cs
  - tests/phase1/EngineAccessPolicy.cs
  - tests/phase1/EngineGeneratedTests.cs
  - tests/phase1/ExemptionValidator.cs
  - tests/phase1/FoundationLifecycleTests.cs
  - tests/phase1/FoundationMovementTests.cs
  - tests/phase1/FoundationPermissionTests.cs
  - tests/phase1/FoundationRunnerTests.cs
  - tests/phase1/GeneratedCallSiteAdapterTests.cs
  - tests/phase1/GodotCallSiteAdapterTests.cs
  - tests/phase1/GraphFixture.cs
  - tests/phase1/GraphTests.cs
  - tests/phase1/KingdomZombies.Phase1.Tests.csproj
  - tools/code-rules/EngineGeneratedSyntax.cs
  - tools/phase2/check.cjs
  - tools/phase2/evidence.cjs
  - tools/phase2/linux-input.sh
  - tools/phase2/rendered.cjs
  - tools/phase2/required-cases.json
  - tools/phase2/runner-fixture.cjs
  - tools/phase2/windows-input.ps1
  - tests/phase1/ConsumerCompilation.cs
  - tests/phase1/BuildFixture.cs
  - tests/phase1/SuppressionTests.cs
findings:
  critical: 0
  warning: 0
  info: 0
  total: 0
  necessity: 0
status: clean
---

# Phase 2: Code Review Report

**Reviewed:** 2026-11-15T12:08:11Z  
**Depth:** standard  
**Files Reviewed:** 44 (41 original scope, 3 additional repair files)  
**Status:** clean

## Summary

Reviewed the complete authored change set from `d1bed40` through `c3cb2d8`, with called infrastructure inspected where necessary. Planning artifacts supplied requirements but were excluded from the source count. The creator's testing amendment and `docs/workflow/pre-phase5-testing.md` govern this review: development OS only, rendered acceptance at 1280×720 after review/fixes, shared sessions and focused repairs. Deferred Linux/additional-resolution acceptance is not a defect or a pass.

Four confirmed blockers affect existing rule-test reliability and shared rendered-session recovery. No source was modified, committed, built, or launched. A Node-only in-memory control reproduced the cumulative-log defect through the production `tracer`. A read-only comparison checked all 204 generated-span bindings and the recorded generated-callsite source/package hashes without mismatches. No necessity findings were established.

The earlier post-draw/observation-stall hypothesis remains unproven in this static pass and is not a finding. This review does not claim rendered execution or completion approval.

The original finding history below is preserved. Frontmatter counts describe currently open findings: all four original findings and the intermediate cleanup regression are resolved through `d087f15`. No open issues remain from this review. The re-review was limited to these repairs and does not supply rendered acceptance evidence.

## Resolution Ledger

| Finding | Repair | Re-review outcome |
| --- | --- | --- |
| CR-01 | `96c9380` | Resolved. The scanner requires the exact declared consumer set and scans each consumer's actual preprocessor context. Missing, duplicate and undeclared owners remain rejected. The new control exercises distinct conditional branches in the game/test contexts. Parent supplied passing focused suppression evidence, 3/3. |
| CR-02 | `f8dc717` | Resolved. Test fixtures now copy the exact linked production source to its expected relative location. The shared copy helper retains all existing copied paths. Parent supplied passing evidence for four isolated controls, 72.506 seconds. |
| CR-03 | `e29f425` | Resolved. Scenario log ranges isolate later diagnostics while the first scenario owns startup output and the complete log is retained. Re-review executed the actual `scenario-diagnostics` pure control successfully at `.phase2/controls/cc5a96c9-e672-4592-8ac0-390b739ec5d7`. |
| CR-04 | `a710572`, `d087f15` | Resolved. Mouse ownership is tracked before dispatch and all owned releases are attempted; failed releases retain ownership for retry. Final cleanup annotates the last executed scenario with its original scenario status, cleanup error and nested causes while retaining earlier passes, partial observations and blocked outcomes. The actual `owned-input` control passed at `.phase2/controls/c71099f3-75c3-4090-b9db-627b1e662139`. The production-boundary `session-cleanup` control passed at `.phase2/controls/325e72d8-ee07-426a-b496-ca63ab6379e5`, covering a crashed process with held input and release-acknowledgement failure. |

Re-review read the actual repair diffs and ran only the three Node pure controls above. No .NET build, game window or VM was launched by the reviewer.

## Narrative Findings (AI reviewer)

## Original Critical Issues — Resolved History

### CR-01: Linked production source violates the suppression scanner's sole-consumer assumption

**Resolution:** Resolved by `96c9380`; original finding retained below.  
**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/tests/phase1/KingdomZombies.Phase1.Tests.csproj:10`  
**Related:** `tests/phase1/ExemptionValidator.cs:64-68`; `tests/phase1/ConsumerCompilation.cs:98-99`.

**Issue:** The new `Compile Include` adds `FoundationInput.cs` to the test assembly while the game still compiles it normally. `ConsumerCompilation` resolves both inputs to the same repository-relative authored path. `ExemptionValidator.CheckRepository()` requires exactly one consumer and throws `SUPPRESSION_SOURCE_OWNERSHIP src/godot-game/foundation/FoundationInput.cs` when it reaches this file. Existing `ExemptionTests.Generate_ExactObservedPermissions_AcceptsOwnedSources()` and `SuppressionTests` call this scanner, so the phase breaks required rule checks even though ordinary compilation can succeed.

**Fix:** Support this explicit shared source in the scanner and check its suppression syntax in every actual consumer context. Continue rejecting zero owners and undeclared ownership changes; do not skip the source or choose an arbitrary first consumer. Alternatively, replace the link with an appropriate production test seam that preserves the intended assembly boundaries. Verify the affected suppression/exemption checks.

### CR-02: Isolated test-consumer fixtures omit their new linked compiler input

**Resolution:** Resolved by `f8dc717`; original finding retained below.  
**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/tests/phase1/KingdomZombies.Phase1.Tests.csproj:10`  
**Related:** `tests/phase1/BuildFixture.cs:35-45`; `tests/phase1/EnforcementTests.cs:786-811`; `tests/phase1/ExemptionTests.cs:354-355,528`; `tests/phase1/RestoreMutationProof.cs:17`.

**Issue:** `BuildFixture(tests: true)` copies Core, the analyzer and the test project, but copies `src/godot-game` only for the mutually exclusive `game: true` mode. The copied test project now explicitly compiles `../../src/godot-game/foundation/FoundationInput.cs`, which is absent from that isolated fixture. Existing test-consumer clean controls therefore fail with a missing-source compiler error before their intended positive or negative subject can be tested. This affects test policy, exemption and restore controls, independently of CR-01.

**Fix:** Include the exact linked production input and its directory in the test fixture's copied dependency closure, or change the source-sharing seam and fixture together. Preserve the real strict build policy and require a successful clean isolated test-consumer build before the affected negative controls.

### CR-03: One scenario's native diagnostic fails every later healthy scenario

**Resolution:** Resolved by `e29f425`; original finding retained below.  
**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/tools/phase2/rendered.cjs:257-269`  
**Related:** `tools/phase2/rendered.cjs:67-70,300-313`.

**Issue:** Each scenario tests `/ERROR:|WARNING:|Unhandled exception/` against `session.log`, which accumulates output for the entire shared game process. After one scenario emits a diagnostic and fails, a successful reset does not remove that diagnostic. Every subsequent scenario reaches its own final check, sees the old message and fails too, including the special resources branch. This incorrectly expands the failed selection and defeats the amended requirement to collect independent outcomes and repair only failed/unexecuted scenarios.

**Evidence:** Called the production `tracer` with an in-memory session providing valid launch, movement, facing and returned-menu observations. Its log contained only `ERROR: previous independent scenario`. The otherwise successful later scenario was rejected as `NATIVE_DIAGNOSTIC ERROR: previous independent scenario`. No game, desktop input, build or output file was used.

**Fix:** Record the native-log offset at each scenario's start and assess diagnostics produced during that scenario. Attribute startup diagnostics explicitly, and retain the complete immutable shared `game.log` for diagnosis. Add a pure control where a failed first scenario resets successfully and an independent second scenario passes without erasing the first failure.

### CR-04: A failed pressed-button observation leaves owned mouse input held

**Resolution:** Resolved by `a710572` and `d087f15`; original finding and intermediate repair regression retained below.  
**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/tools/phase2/rendered.cjs:76-84`  
**Related:** `tools/phase2/rendered.cjs:105-115,140`.

**Issue:** The session records possible held keyboard keys before dispatch and releases them during reset/close, but it never records mouse-button holds. The menu scenario sends `mouse-down`, then awaits the `start-pressed` capture before sending `mouse-up`. If that observation fails, or the mouse helper injects the press and fails its acknowledgement, control jumps to scenario recovery with the left mouse button still held. Neither `reset()` nor `close()` releases it. Later menu scenarios run with contaminated physical input, and final process termination can leave the user's mouse button held after the game closes.

**Fix:** Track a possible owned mouse-button press before dispatch, including the press/release pair inside `click`. Clear it only after an acknowledged release, and release it alongside keyboard holds during both reset and final cleanup. Ensure cleanup attempts all owned releases even if one fails. Add a pure helper/session control that fails after mouse-down and observes a cleanup mouse-up.

**Intermediate repair regression (resolved by `d087f15`):** At `a710572`, `tools/phase2/rendered.cjs:126-133,322-324` allowed `close()`'s `release()` exception to escape the outer `finally`. For example, a game crash while a movement key was held left the key in the ownership set; the final release dispatch rejected `GAME_EXITED_BEFORE_INPUT`, producing `OWNED_INPUT_RELEASE_FAILED`. That exception overrode the completed `runScenarios()` return. `check.cjs` therefore never received `nativeResults` and recorded all selected native cases as blocked, losing earlier passes and the collected failed scenario's partial result from the summary. An acknowledgement failure during final release had the same effect.

**Verified correction:** `rendered()` retains its results array and calls the production `closeSession()` boundary. That boundary records cleanup failure on the last executed case without throwing away the results; termination, complete logs and signal-handler removal retain their guaranteed cleanup paths. The pure production-boundary control returns an earlier pass, affected partial result and later blocked result, forces both crash-related and acknowledgement-related release errors, and verifies the outcomes and cleanup causes survive.

---

_Reviewed: 2026-11-15T11:51:43Z_  
_Reviewer: the agent (gsd-code-reviewer)_  
_Depth: standard_
