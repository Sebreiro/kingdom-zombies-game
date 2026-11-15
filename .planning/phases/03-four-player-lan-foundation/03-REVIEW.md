---
phase: 03-four-player-lan-foundation
reviewed: 2026-11-15T19:32:02Z
depth: standard
reviewed_revision: 166a32ad6a7541c4f5eed1378def0a86e9bea632
build_revision: 166a32ad6a7541c4f5eed1378def0a86e9bea632
diff_base: 87e0b44
scope_files: 115
files_reviewed: 112
files_reviewed_list:
  - .gitignore
  - docs/code/engine-generated-spans.json
  - docs/code/exemptions.json
  - docs/code/generated-callsite-adaptations.json
  - docs/code/register.json
  - docs/code/visibility-exemptions.json
  - docs/workflow/lan-foundation-checks.md
  - docs/workflow/verification.md
  - src/core/FoundationHeldIntent.cs
  - src/core/FoundationMovementCommand.cs
  - src/core/FoundationParticipantState.cs
  - src/core/FoundationSession.cs
  - src/core/FoundationSnapshot.cs
  - src/godot-game/foundation/DiscoveryCodec.cs
  - src/godot-game/foundation/EnetConnectionService.cs
  - src/godot-game/foundation/Foundation.tscn
  - src/godot-game/foundation/FoundationActorNode.cs
  - src/godot-game/foundation/FoundationAdmissionHistory.cs
  - src/godot-game/foundation/FoundationAutomation.cs
  - src/godot-game/foundation/FoundationBrowser.cs
  - src/godot-game/foundation/FoundationCapturePublication.cs
  - src/godot-game/foundation/FoundationCoordinator.Admission.cs
  - src/godot-game/foundation/FoundationCoordinator.GuestPublication.cs
  - src/godot-game/foundation/FoundationCoordinator.Lifetime.cs
  - src/godot-game/foundation/FoundationCoordinator.cs
  - src/godot-game/foundation/FoundationCopy.cs
  - src/godot-game/foundation/FoundationDiagnostics.cs
  - src/godot-game/foundation/FoundationDisplay.cs
  - src/godot-game/foundation/FoundationDrawObserver.cs
  - src/godot-game/foundation/FoundationEndpoint.cs
  - src/godot-game/foundation/FoundationEnvelope.cs
  - src/godot-game/foundation/FoundationEphemeralProfile.cs
  - src/godot-game/foundation/FoundationHud.cs
  - src/godot-game/foundation/FoundationInput.cs
  - src/godot-game/foundation/FoundationMenu.cs
  - src/godot-game/foundation/FoundationNativeRegression.cs
  - src/godot-game/foundation/FoundationObservationScope.cs
  - src/godot-game/foundation/FoundationPeerBinding.cs
  - src/godot-game/foundation/FoundationPresentation.cs
  - src/godot-game/foundation/FoundationProtocol.cs
  - src/godot-game/foundation/FoundationRegressionDiscovery.cs
  - src/godot-game/foundation/FoundationRoot.cs
  - src/godot-game/foundation/FoundationRuntimeState.cs
  - src/godot-game/foundation/FoundationServices.cs
  - src/godot-game/foundation/FoundationView.cs
  - src/godot-game/foundation/FoundationWorld.tscn
  - src/godot-game/foundation/LanDiscoveryService.cs
  - src/godot-game/foundation/LocalProfileService.cs
  - src/godot-game/foundation/LocalProfileStorage.cs
  - src/multiplayer-abstractions/ConnectionPacket.cs
  - src/multiplayer-abstractions/IConnectionService.cs
  - src/multiplayer-abstractions/IDiscoveryService.cs
  - src/multiplayer-abstractions/IIdentityService.cs
  - src/multiplayer-abstractions/PlayerProfile.cs
  - src/multiplayer-abstractions/SessionAdvertisement.cs
  - tests/phase1/ArchitecturePolicy.cs
  - tests/phase1/BuildFixture.cs
  - tests/phase1/EngineAccessPolicy.cs
  - tests/phase1/EngineGeneratedTests.cs
  - tests/phase1/ExemptionTests.cs
  - tests/phase1/ExemptionValidator.cs
  - tests/phase1/FoundationCapturePublicationTests.cs
  - tests/phase1/FoundationConnectionReplacement.cs
  - tests/phase1/FoundationDiscoveryReplacement.cs
  - tests/phase1/FoundationHarnessConnection.cs
  - tests/phase1/FoundationIdentityReplacement.cs
  - tests/phase1/FoundationLanAdmissionTests.cs
  - tests/phase1/FoundationLanAuthorityTests.cs
  - tests/phase1/FoundationLanInputTests.cs
  - tests/phase1/FoundationLanLifecycleTests.cs
  - tests/phase1/FoundationLanOrderingTests.cs
  - tests/phase1/FoundationLanPermissionTests.cs
  - tests/phase1/FoundationLanProtocolTests.cs
  - tests/phase1/FoundationLanRunnerTests.cs
  - tests/phase1/FoundationLanServicesTests.cs
  - tests/phase1/FoundationLinkedSources.cs
  - tests/phase1/FoundationPermissionTests.cs
  - tests/phase1/FoundationScenarioClock.cs
  - tests/phase1/FoundationServiceSubstitutionProof.cs
  - tests/phase1/FoundationTestConnection.cs
  - tests/phase1/KingdomZombies.Phase1.Tests.csproj
  - tests/phase1/SuppressionTests.cs
  - tools/code-rules/BodyLengthAnalyzer.cs
  - tools/code-rules/EngineGeneratedSyntax.cs
  - tools/phase2/check.cjs
  - tools/phase2/menu-navigation.cjs
  - tools/phase2/rendered.cjs
  - tools/phase3/catalog.cjs
  - tools/phase3/check.cjs
  - tools/phase3/evidence.cjs
  - tools/phase3/foreground.cjs
  - tools/phase3/foreground.test.cjs
  - tools/phase3/inherited.cjs
  - tools/phase3/navigation.test.cjs
  - tools/phase3/network.cjs
  - tools/phase3/png.cjs
  - tools/phase3/processes.cjs
  - tools/phase3/rendered.cjs
  - tools/phase3/required-cases.json
  - tools/phase3/runner-fixture.cjs
  - tools/phase3/scenarios.cjs
  - tools/phase3/selection-impact.cjs
  - tools/phase3/selection.test.cjs
  - tools/phase3/stopped-fixture.cjs
  - tools/phase3/stopped.cjs
  - tools/phase3/terminal-fixture.cjs
  - tools/phase3/terminal.cjs
  - tools/phase3/timing.test.cjs
  - tools/phase3/windows-input.ps1
  - tools/phase3/revisions.cjs
  - tools/phase3/revisions.test.cjs
  - tools/phase3/inherited.test.cjs
context_files_reviewed:
  - .planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md
  - .planning/phases/03-four-player-lan-foundation/03-SELECTION.json
  - .planning/phases/03-four-player-lan-foundation/03-04-CHECKPOINT.md
findings:
  critical: 0
  warning: 0
  info: 0
  total: 0
  necessity: 0
status: clean
---

# Phase 03: Code Review Report

**Reviewed:** 2026-11-15T19:32:02Z  
**Depth:** standard  
**Source files reviewed:** 112  
**Explicit scope:** all 115 paths from `.phase3/review-fix-union.txt`, including the three planning-context paths listed separately above  
**Status:** clean  
**Reviewed source / pinned build revision:** `166a32ad6a7541c4f5eed1378def0a86e9bea632`  
**Diff base:** `87e0b44`

## Summary

This independent re-review preserves the complete 115-path source/context union and all earlier assessments. The complete Phase 3 source review above is carried forward together with a direct recheck of the six-path repair diff from `a35bd6f` to `166a32ad6a7541c4f5eed1378def0a86e9bea632`: guest publication, native projection, native regression, protocol proof, independent catalog and saved scenarios. Every repair path was already inside the retained scope. Their production callers, trust boundaries and actual regression consumers were traced; the cumulative fix ledger was treated as a claim to confirm.

**All fourteen prior findings are resolved. No new actionable defect or open finding remains.** CR-09's repair rejects continuing-actor elapsed regressions before publication or liveness mutation and makes camera interpolation use bounded local monotonic time with finite recovery. The preceding thirteen dispositions remain valid: the repair changes none of their admission/session fences, evidence-finalization helpers, inherited continuation paths, exact permission registries or generated metadata.

This is a clean pre-window source review, not completed rendered acceptance. No real game window, VM, desktop input, broad test suite, build, branch operation or commit was executed by this reviewer. The renewed retained no-window report was reconciled through the current production validator and its source/tool tuple independently recomputed. The earlier .NET arithmetic probe remains defect-history evidence; the repair's actual coordinator and headless native projection executions are retained separately below. No rendered, physical-input, image-appearance, other-OS, physical mixed-computer LAN or release acceptance is granted.

Necessity was assessed using the required rubric; no necessity finding is asserted. No structural pre-pass or external reviewer lane was supplied. The agent-skills query returned no additional configured reviewer skills.

## Narrative Findings (AI reviewer)

No open Critical/BLOCKER, Warning or Info findings remain at the reviewed source revision.

## Disposition of all fourteen prior findings

The first report is retained in `03-REVIEW.iter2.md` and original review commit `624f37b`; the following review is retained in `03-REVIEW.iter3.md`. The cumulative `03-REVIEW-FIX.md` records all fourteen repairs without skips. CR-09 was raised in the preceding canonical review and fixed by atomic commit `166a32ad6a7541c4f5eed1378def0a86e9bea632`. These are resolved historical findings, not additional open findings or rendered acceptance claims.

| Prior ID | Disposition | Source/consumer checked |
| --- | --- | --- |
| CR-01 | Resolved | `FoundationMenu.Rows` commits its new key before focus signals, creates the complete row set before restoring focus, and `Detail` no longer rebuilds rows. `FoundationNativeRegression.Browser` exercises occupancy/reorder/focus and unchanged-node identity through the actual headless native subject. |
| CR-02 | Resolved | `FoundationDrawObserver` enumerates actual existing row children and stored exact session identities; absent profiles serialize as null with their error preserved. The native observer regression covers hidden browser, pending discovery and corrupt-profile recovery. This does not supply rendered appearance credit. |
| CR-03 | Resolved | `FoundationAdmissionHistory` bounds refusal outcomes per provider-authenticated connection to 32 / 30 seconds, separately from reservations. Connection generations fence queued/replaced connections; expired histories cannot reopen; disconnected histories retire without recreation. Actual coordinator controls include more than 128 refusals, freed-slot admission, expiry, reconnect and repeated disconnected-history cleanup. |
| CR-04 | Resolved | `FoundationActorNode.Name` injectively encodes ordinal UTF-16 units into safe hex native names. Projection and observation use it without changing model/wire identities. Protocol/native controls cover traversal-shaped IDs, sanitizer collisions and canonically distinct Unicode. |
| CR-05 | Resolved | `stopped.cjs` reconciles sampled records against actual peer draw histories, current origin profile/actor/session/binding, native release and accepted zero-direction authority. It derives positions/revisions from those records and checks fresh frames, post-release ordering, at most 1 ms uncertainty and inclusive 50-subunit/500-ms limits. The full-reconciler fixture mutates these references and scalar values. |
| CR-06 | Resolved | Every owned rendered process has a terminal record. Normal Quit requires its handler acknowledgement, zero exit and closed streams; case/role-bound deliberate loss must be declared before termination. Unexpected exits invalidate all results sharing the process set. Real-child controls cover normal/expected loss, post-capture nonzero/crash and failure during Quit through `scenarios.close`. |
| CR-07 | Resolved | `revisions.cjs` checks real commit identity/ancestry, every intervening path against a narrow metadata whitelist, both committed relevant-input trees, the live relevant fingerprint and dirty non-metadata state. Prelaunch binds the committed gate and original Markdown bytes, copies both immutably into the invocation, and completion checks those original bytes at the actual execution commit. `buildRevision` must equal the reviewed source revision while `revision` retains actual HEAD. The documented phase-scoped `SourceRevisionId` pin survives review/gate/closure commits and every rebuild/inherited build; exact DLL/tool hashes remain required. Actual isolated SDK controls reproduce unpinned drift, reject it and restore byte-identical pinned DLLs without rewriting execution/review reports. |
| CR-08 | Resolved | The selected exact session ID travels from `FoundationMenu.JoinSelected → Submit → FoundationRoot.JoinAddress → FoundationCoordinator.Join` into the attempt's hello. Host mismatch rejection precedes reservation; guest mismatch rejection precedes baseline install/ack. Manual joining stays session-agnostic. Cancellation clears the pending target, and stale baselines/refusals cannot enter or close a new attempt. Coordinator controls cover selected A/A, retained A after rehost B, manual B, wrong offered baseline, cancel and stale/new attempts; the native row regression verifies the submitted ID and its scoped clearing. |
| WR-01 | Resolved | `FoundationCapturePublication.Publish` writes/closes a unique same-directory temporary JSON before atomic move, after PNG completion. Capture's end bracket follows complete reads/validation/hashing. Actual C# helper and delayed consumer controls exercise the publication boundary. |
| WR-02 | Resolved | Inherited keyboard procedures follow the actual seven Tab/Shift+Tab controls and Start/Host/Join/Quit arrow paths, preserve error/retry and movement regressions, and end with ordinary keyboard Space on Quit. The adjacency control checks the authored native scene. Real desktop execution remains pending. |
| WR-03 | Resolved | Saved selection includes 433 affected Phase 1 cases, all 31 body-length-family cases, all six shared generated-permission analyzer families and affected register/generated/fixture consumers. Known affected omissions are rejected despite nonblank excuses. The exact 26 linked-source inventory, per-consumer ownership and copied compiler-fixture inputs are synchronized. The 243 active omissions and eight planned declarations remain explicit. |
| WR-04 | Resolved | The shared foreground queue covers focus/click/text sequences across helpers. Final-slot contenders are prepared serially, then Enter is dispatched to their owned PIDs via native window messages with exact preparation frames, callbacks and driver records. Provenance remains separate from physical input. Actual desktop delivery feasibility is still pending. |
| WR-05 | Resolved | `inherited.run` persists each attempted child process, stdout/stderr, outcome and original child report, including failure records. It continues independent commands, names prerequisite-specific blockers, returns all accumulated references and a failing aggregate, and `check.main` retains that aggregate in the outer summary even when independent native work runs. Completion rejects any non-passing inherited aggregate. Real controlled children exercise failing assertion followed by later success, integrity/setup blockers and missing reports; a separate control exercises the actual outer coordinator and immutable summary retention. |
| CR-09 | Resolved | `RegressesActorTime` compares exact continuing actor IDs within the already-matched session and rejects decreasing elapsed values before any candidate installation, revision/readiness change, acknowledgement or liveness renewal. Fresh actors may start at zero; a fresh session has an independent baseline. `FoundationView.Project` uses local `Time.GetTicksMsec`, clamps its interpolation interval to 0–0.25 seconds, preserves the 0.15-second smoothing/initial snap and finite camera bounds, and recovers from a nonfinite prior transform. Actual coordinator controls and the nonce-scoped native projection regression exercise the repaired consumers. |

## CR-09 independent repair recheck

The production guest branch rejects a backwards elapsed value before `_remote = candidate` and `_lastResponse = Now()`. Its check also runs while an installed baseline is still unready. It uses actor identity rather than presentation slot, so an admitted replacement actor's zero origin remains valid. Session/revision/attempt validation still precedes publication, and ownership/readiness validation remains intact. No elapsed counter from the network is used as the camera's smoothing clock.

The actual protocol control checks extreme/equal/advancing counters, regressions in each continuing actor, rejection before readiness, exact snapshot/revision/installed-revision/readiness/reply preservation, successful installation of a lower valid revision after a rejected revision 100, replacement actor identity and fresh-session baselines. It sends a rejected candidate at 9999 ms, then reaches the original 10000 ms response deadline, proving rejection does not extend liveness.

The actual headless native regression invokes production `FoundationView.Project` with elapsed `long.MaxValue → 0 → 0 → long.MaxValue - 1 → long.MaxValue`, at position 1000. It requires finite player/world positions and the initial snapped world X of 624. It then supplies a NaN/infinite prior world transform and requires finite bounded recovery. The catalog and saved scenarios retain these obligations, all earlier IDs and the full affected selection. This is native numerical/projection proof, without a rendered timing or appearance claim.

## Evidence inspected and limits

The retained repair-batch index `.phase3/iter3-final-1794770502320/summary.json` contains twelve passing jobs: eight policy cases, four Phase 2 core cases, three Phase 2 runner cases, a strict whole-solution build with zero warnings/errors, and twenty Phase 3 non-window cases. That is 35 case executions plus the strict build. The twelve original process records and referenced passing reports were inspected. This is retained execution evidence, not a newly executed reviewer suite.

The final Phase 3 report `.phase3/runs/win32-x64/1be5faf3-4796-4650-8cfa-6316daecff8d/summary.json` records 20/20 passing results, duration 216980 ms, zero game windows, actual source HEAD and pinned build revision `166a32ad6a7541c4f5eed1378def0a86e9bea632`. The reviewer loaded it through the actual current `evidence.loadReport/reconcile` path: all twenty records and their required artifacts reconciled. Recomputing the relevant source manifest returned exactly `62c3866e7eb1819c7347a246bc2daab92b46b8228d60505a84139907366dd0d1`.

The exact recorded tool hash is `c91c021821910af08a06e5b8b744f8d86133fee6c057b6955ba0cbb2333fc651`. Recomputing its declared tuple from current Node v24.22.1, the pinned SDK 10.0.404, toolchain bytes and current proof DLL matched it; the current DLL SHA-256 is `6f35860475ad9136db561878ad7163a18e7b2b3e63a986c66f668533eff11607`. The retained report records Windows x64, Windows 10 Enterprise LTSC 2021 / 10.0.19044, OS fingerprint `4a3c7505cb92e1bb2f7412dd3207a435c55d5c78f2e7e8d11213ecfc2e8768db`. In the preceding review, a full `createIdentity` call did not complete at its nested dotnet probe (`DOTNET_MISSING`); no such execution is credited here. The present follow-up used read-only fingerprint/report reconciliation and launched no build. No DLL was rebuilt or modified by the reviewer.

The hash-bound integrity-control process log `.phase3/controls/9c31880d-e65a-42f2-bd05-5a0f802e7a12/integrity-controls.process.json` records eight successful Node controls, including actual metadata-closure and prelaunch rejection controls. The real SDK finalization fixture `.phase3/revision-control-xFECSZ` retains five successful build records: reviewed-source build, pinned rebuild after review/gate metadata commit, pinned closure rebuild, unpinned negative build, and repinned restoration. The fixture asserts exact DLL identity, rejects changed source/tool/selection/environment/build stamp, unrelated/missing commits, unapproved/reverted intervening paths, dirty relevant input and altered review bytes, and retains unchanged original report bytes. These isolated fixtures establish validator behavior, not rendered production execution.

All five changed code-policy JSON files and `EngineGeneratedSyntax.cs` remain unchanged from `624f37b`. Inspection found 220 exact registry rows and 220 unique authored diagnostic/path/hash/span tuples; the compiler-origin and adjacent-negative controls remain source/hash scoped. The existing fifteen public types include eight Phase 3 additions, and the exact Phase 3 native-method inventory remains 45, with compiler-valid positive/adjacent-negative consumers. No blanket permissions, package additions, new project/reference or generated hash refresh is introduced by the fixes.

Older auxiliary evidence with unpinned prebuild tool drift remains failure/history and receives no current credit. Earlier 66-case body/fixture results remain historical. The fresh full selected 433 Phase 1 + 16 Phase 2 + 32 Phase 3 pass is still Task 3 work.

## Required handoff before a prelaunch gate

The source review is complete at `166a32ad6a7541c4f5eed1378def0a86e9bea632`; CR-09 and the preceding thirteen findings are resolved. The parent may now author the hash-bound review gate for this exact reviewed source/input/tool tuple, after the scratch cleanup below. Keep `SourceRevisionId` pinned to this reviewed source SHA through the gate metadata commit, mandatory strict rebuild and subsequent commands. A future source change requires renewed affected checks and independent review; old executions must retain their original identities.

The parent confirmed that untracked `03-REVIEW.iter2.md`, `03-REVIEW.iter3.md`, `03-REVIEW-FIX.iter2.md` and `03-REVIEW-FIX.iter3.md` in the phase folder are temporary review-loop backups. Before gate authoring/commit, archive the required history under ignored `.phase3/review-history` and remove those untracked phase-folder scratch copies. They must not become intervening commits after the eventual reviewed source SHA. The narrow metadata whitelist intentionally rejects undeclared scratch; do not broaden it to accommodate them. Preserve the original review in Git and the cumulative final fix ledger.

Retain this review's exact `SourceRevisionId` pin through the mandatory metadata-descendant strict rebuild, fresh `--all`, image review and closure validation. True execution HEAD and reviewed-source stamp remain distinct. The parent owns report/gate/closure commits, shared phase state and publishing.

Actual native foreground physical input, sustained background/window-message feasibility, permitted synthetic fallback provenance, 48 conservative rendered timing rows, 16 stopped comparisons, every required image's appearance review and the entire final selected-set pass remain unproved. A future input/timing/image failure is an acceptance blocker, not a synthetic or state-only pass. No hosted/Linux, mixed-computer LAN, complete A-38/A-45 campaign, packaging, hardware, endurance or Phase 5 certification is claimed.

---

_Reviewed: 2026-11-15T19:32:02Z_  
_Reviewer: the agent (gsd-code-reviewer)_  
_Depth: standard_
