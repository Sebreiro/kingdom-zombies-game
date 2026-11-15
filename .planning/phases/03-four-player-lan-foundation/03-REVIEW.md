---
phase: 03-four-player-lan-foundation
reviewed: 2026-11-15T21:22:41Z
depth: standard
reviewed_revision: 7b140a27e4f5556db12caed2b1614c52a1aad18a
build_revision: 7b140a27e4f5556db12caed2b1614c52a1aad18a
diff_base: 87e0b44
scope_files: 118
files_reviewed: 115
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
  - tools/phase3/reviewed-source.targets
  - tests/phase1/GraphFixture.cs
  - docs/code/rule-matrix.md
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

**Reviewed:** 2026-11-15T21:22:41Z  
**Depth:** standard  
**Source files reviewed:** 115  
**Explicit scope:** all 118 paths from `.phase3/review-fix-union.txt`, preserving the original scope, added SourceLink target and the two newly reviewed proof/document consumers; three planning-context paths are listed separately above  
**Status:** clean  
**Reviewed source / pinned build revision:** `7b140a27e4f5556db12caed2b1614c52a1aad18a`  
**Diff base:** `87e0b44`

## Summary

This independent re-review preserves the complete original source/context union and all earlier assessments, adding the phase-only SourceLink target and the later graph-fixture/matrix consumers for a total of 118 paths / 115 source paths. The complete Phase 3 source review above is carried forward together with a direct recheck of the six-path repair diff from `a35bd6f` to `166a32ad6a7541c4f5eed1378def0a86e9bea632`: guest publication, native projection, native regression, protocol proof, independent catalog and saved scenarios. Every repair path was already inside the retained scope. Their production callers, trust boundaries and actual regression consumers were traced; the cumulative fix ledger was treated as a claim to confirm.

The final focused integration recheck additionally examines committed source `e25a500d12006e256d84b8b241d0cee9c507f278`. Its only source changes since the prior clean review are `tools/phase3/revisions.cjs` and `revisions.test.cjs`, both already in the 115-path scope. The exact five added closure-report permissions and their actual Git/SDK fixture consumers introduce no new actionable finding; the existing fourteen resolved dispositions are preserved. Intervening canonical review/gate/fix/continuation and shared-state files are metadata, not additional changed game source.

A final actual-SDK SourceLink recheck additionally examines source `cff08ebe7cc78c562fe370dcdc3718e116938905`. An actual metadata-HEAD rebuild exposed independent `SourceRoot.RevisionId` drift despite the assembly `SourceRevisionId` pin; exact tool validation stopped before any full run or window. The bounded repair adds an exact own-root SourceLink pin, passes it to Phase 3/inherited builds, and extends the real SDK fixture with a GitHub remote and an import-omitted negative control. This CR-07 follow-up is resolved. The source/game, native/public/generated permission and fourteen prior behavioral dispositions remain intact.

The prior four-file real-run repair batch at `dc523e9` remains sound on inspection. Checking its authentic passing reports exposed CR-10's inherited evidence-base mismatch; the current focused recheck independently verifies its two-file repair at `7b140a27e4f5556db12caed2b1614c52a1aad18a`. The production inherited reader now shares the corrected exact case/report/evidence contract with controls using the real nested Phase 1 layout.

**All fifteen prior findings are resolved. No new actionable defect or open finding remains.** The reviewer independently applied the shared proof reader, read-only, to all 433 authentic passing records (428 retained first-run passes and five focused repairs), with original summary/report hashes intact. The original failed aggregate still rejects. No previous failure is changed into a pass or credited as a new execution. All preceding game, permission, driver, finalization and SourceLink dispositions remain intact.

This is an independent source re-review during the post-review repair cycle, not completed rendered acceptance. No real game window, VM, desktop input, broad test suite, build, branch operation or commit was executed by this reviewer. The renewed retained no-window report was reconciled through the current production validator and its source/tool tuple independently recomputed. The earlier .NET arithmetic probe remains defect-history evidence; the repair's actual coordinator and headless native projection executions are retained separately below. The first full selected execution now has real desktop results and retained failures, as recorded below; it is not accepted final completion. No new rendered, physical-input, image-appearance, other-OS, physical mixed-computer LAN or release acceptance is granted by this source review.

Necessity was assessed using the required rubric; no necessity finding is asserted. No structural pre-pass or external reviewer lane was supplied. The agent-skills query returned no additional configured reviewer skills.

## Narrative Findings (AI reviewer)

No open Critical/BLOCKER, Warning or Info findings remain at this reviewed source revision.

## CR-10 independent repair recheck

The original failure was real: inherited Phase 1 subject references use a case-relative base, while their CTRF references use the run root. The old Phase 3 consumer resolved both at the run root and rejected actual passing evidence with ENOENT. This is now repaired without relocating, copying or rewriting retained artifacts.

`verifyInherited` calls the shared `verifyPhase1Proof` helper for every Phase 1 result. The caller first requires a known independent catalog case; the helper requires exact result/case identity, a passing result and the exact run-relative `<case-id>/tests.json` reference. It validates that report's hash and canonical run containment before deriving its parent directory. Subject references are then hash/containment checked within that independently verified case directory, and the actual CTRF report is checked against the required case/method through the existing Phase 1 verifier. Cross-case report substitution cannot supply a different evidence base; traversal, absolute/drive paths, altered or missing artifacts remain non-success. Aggregate status, exact selected membership, invocation/source/process validation and original failure retention are unchanged.

The RED commit `8466836` first changed the closure fixture to the authentic nested producer layout and failed at the actual inherited reader's ENOENT. GREEN `7b140a2` adds the shared production helper and negative controls; the complete fixture now retains case-relative subject paths and run-relative CTRF paths. Its positive completion and actual-GitHub/five-build SDK sequence remain intact. Controls reject altered hashes, missing logs, absolute/drive/traversal paths, and cross-case report/evidence substitution. `.phase3/cr10-green.log` records 3/3 passing tests; `.phase3/cr10-strict.log` records zero warnings/errors, 10.68 seconds. No build or test suite was rerun by this reviewer.

The retained `.phase3/cr10-real-reports.json` declares compatibility-only validation of 428 original passing records plus five focused repair records, with 1732 negative checks. Its ignored script was read: it uses the same production helper, verifies original hashes/bytes, preserves the first run's five Phase 1 failures, and explicitly requires the original failed aggregate to reject. The reviewer independently checked the index's original summary/full-report hashes and invoked the actual helper read-only for all 433 records across 321 authentic summaries. All passed the repaired reader; the original full aggregate still threw `INHERITED_AGGREGATE_FAILED`. This establishes producer/consumer compatibility, not new execution, evidence reuse, or final completion credit.

The actual C#-to-Node `runner.integrity` report `.phase3/runs/win32-x64/5e25ec4e-d0d2-4f2c-bed0-4b73588c0b45/summary.json` reconciles at source/build `7b140a27e4f5556db12caed2b1614c52a1aad18a`, with zero game windows. Current read-only fingerprint recomputation matches input `c7de71c81c33616f8705fb745ef575b62dd5b3dbaea31a8d77e25e90ddbe1360`, tool tuple `92b23f579ea9813146e43d1db3ca1023342588c54a0421f984ca058084e545f3`, and proof DLL SHA-256 `93f36dbe902b3a8eb1ea78bca25baa66adcda0fab9c038f668567fa4ff7ec87c`. Prior run identities below remain historical.

## Disposition of all fifteen prior findings

The first report is retained in `03-REVIEW.iter2.md` and original review commit `624f37b`; the following review is retained in `03-REVIEW.iter3.md`. The cumulative fix ledger preserves the earlier repairs without skips; CR-10 is independently resolved by the current two-file commit and controls above. CR-09 was raised in the preceding canonical review and fixed by atomic commit `166a32ad6a7541c4f5eed1378def0a86e9bea632`. These are resolved historical findings, not additional open findings or rendered acceptance claims.

| Prior ID | Disposition | Source/consumer checked |
| --- | --- | --- |
| CR-01 | Resolved | `FoundationMenu.Rows` commits its new key before focus signals, creates the complete row set before restoring focus, and `Detail` no longer rebuilds rows. `FoundationNativeRegression.Browser` exercises occupancy/reorder/focus and unchanged-node identity through the actual headless native subject. |
| CR-02 | Resolved | `FoundationDrawObserver` enumerates actual existing row children and stored exact session identities; absent profiles serialize as null with their error preserved. The native observer regression covers hidden browser, pending discovery and corrupt-profile recovery. This does not supply rendered appearance credit. |
| CR-03 | Resolved | `FoundationAdmissionHistory` bounds refusal outcomes per provider-authenticated connection to 32 / 30 seconds, separately from reservations. Connection generations fence queued/replaced connections; expired histories cannot reopen; disconnected histories retire without recreation. Actual coordinator controls include more than 128 refusals, freed-slot admission, expiry, reconnect and repeated disconnected-history cleanup. |
| CR-04 | Resolved | `FoundationActorNode.Name` injectively encodes ordinal UTF-16 units into safe hex native names. Projection and observation use it without changing model/wire identities. Protocol/native controls cover traversal-shaped IDs, sanitizer collisions and canonically distinct Unicode. |
| CR-05 | Resolved | `stopped.cjs` reconciles sampled records against actual peer draw histories, current origin profile/actor/session/binding, native release and accepted zero-direction authority. It derives positions/revisions from those records and checks fresh frames, post-release ordering, at most 1 ms uncertainty and inclusive 50-subunit/500-ms limits. The full-reconciler fixture mutates these references and scalar values. |
| CR-06 | Resolved | Every owned rendered process has a terminal record. Normal Quit requires its handler acknowledgement, zero exit and closed streams; case/role-bound deliberate loss must be declared before termination. Unexpected exits invalidate all results sharing the process set. Real-child controls cover normal/expected loss, post-capture nonzero/crash and failure during Quit through `scenarios.close`. |
| CR-07 | Resolved | `revisions.cjs` checks real commit identity/ancestry, every intervening path against a narrow metadata whitelist, both committed relevant-input trees, the live relevant fingerprint and dirty non-metadata state. Prelaunch binds the committed gate and original Markdown bytes, copies both immutably into the invocation, and completion checks those original bytes at the actual execution commit. `buildRevision` must equal the reviewed source revision while `revision` retains actual HEAD. The documented phase-scoped `SourceRevisionId` pin survives review/gate/closure commits and every rebuild/inherited build; exact DLL/tool hashes remain required. The actual-GitHub SDK follow-up below pins own-root `SourceRoot.RevisionId` after discovery as well as the assembly property; its negative omits the reviewed import while retaining the assembly pin, reproduces SourceLink/DLL drift, rejects it and restores exact DLL bytes without rewriting reports. |
| CR-08 | Resolved | The selected exact session ID travels from `FoundationMenu.JoinSelected → Submit → FoundationRoot.JoinAddress → FoundationCoordinator.Join` into the attempt's hello. Host mismatch rejection precedes reservation; guest mismatch rejection precedes baseline install/ack. Manual joining stays session-agnostic. Cancellation clears the pending target, and stale baselines/refusals cannot enter or close a new attempt. Coordinator controls cover selected A/A, retained A after rehost B, manual B, wrong offered baseline, cancel and stale/new attempts; the native row regression verifies the submitted ID and its scoped clearing. |
| WR-01 | Resolved | `FoundationCapturePublication.Publish` writes/closes a unique same-directory temporary JSON before atomic move, after PNG completion. Capture's end bracket follows complete reads/validation/hashing. Actual C# helper and delayed consumer controls exercise the publication boundary. |
| WR-02 | Resolved | Inherited keyboard procedures follow the actual seven Tab/Shift+Tab controls and Start/Host/Join/Quit arrow paths, preserve error/retry and movement regressions, and end with ordinary keyboard Space on Quit. The adjacency control checks the authored native scene. Real desktop execution remains pending. |
| WR-03 | Resolved | Saved selection includes 433 affected Phase 1 cases, all 31 body-length-family cases, all six shared generated-permission analyzer families and affected register/generated/fixture consumers. Known affected omissions are rejected despite nonblank excuses. The exact 26 linked-source inventory, per-consumer ownership and copied compiler-fixture inputs are synchronized. The 243 active omissions and eight planned declarations remain explicit. |
| WR-04 | Resolved | The shared foreground queue covers focus/click/text sequences across helpers. Final-slot contenders are prepared serially, then Enter is dispatched to their owned PIDs via native window messages with exact preparation frames, callbacks and driver records. Provenance remains separate from physical input. Actual desktop delivery feasibility is still pending. |
| WR-05 | Resolved | `inherited.run` persists each attempted child process, stdout/stderr, outcome and original child report, including failure records. It continues independent commands, names prerequisite-specific blockers, returns all accumulated references and a failing aggregate, and `check.main` retains that aggregate in the outer summary even when independent native work runs. Completion rejects any non-passing inherited aggregate. Real controlled children exercise failing assertion followed by later success, integrity/setup blockers and missing reports; a separate control exercises the actual outer coordinator and immutable summary retention. |
| CR-09 | Resolved | `RegressesActorTime` compares exact continuing actor IDs within the already-matched session and rejects decreasing elapsed values before any candidate installation, revision/readiness change, acknowledgement or liveness renewal. Fresh actors may start at zero; a fresh session has an independent baseline. `FoundationView.Project` uses local `Time.GetTicksMsec`, clamps its interpolation interval to 0–0.25 seconds, preserves the 0.15-second smoothing/initial snap and finite camera bounds, and recovers from a nonfinite prior transform. Actual coordinator controls and the nonce-scoped native projection regression exercise the repaired consumers. |
| CR-10 | Resolved | `verifyInherited` now uses shared `verifyPhase1Proof`: exact known case/pass/report path, verified run-relative CTRF hash/containment, case-relative subject hash/containment, and actual CTRF case/method validation. The fixture uses the real nested Phase 1 layout and rejects cross-case/path/hash/missing substitutions. The reviewer independently read all 433 authentic passing records through this same helper and confirmed the original failed aggregate still rejects. |

## CR-09 independent repair recheck

The production guest branch rejects a backwards elapsed value before `_remote = candidate` and `_lastResponse = Now()`. Its check also runs while an installed baseline is still unready. It uses actor identity rather than presentation slot, so an admitted replacement actor's zero origin remains valid. Session/revision/attempt validation still precedes publication, and ownership/readiness validation remains intact. No elapsed counter from the network is used as the camera's smoothing clock.

The actual protocol control checks extreme/equal/advancing counters, regressions in each continuing actor, rejection before readiness, exact snapshot/revision/installed-revision/readiness/reply preservation, successful installation of a lower valid revision after a rejected revision 100, replacement actor identity and fresh-session baselines. It sends a rejected candidate at 9999 ms, then reaches the original 10000 ms response deadline, proving rejection does not extend liveness.

The actual headless native regression invokes production `FoundationView.Project` with elapsed `long.MaxValue → 0 → 0 → long.MaxValue - 1 → long.MaxValue`, at position 1000. It requires finite player/world positions and the initial snapped world X of 624. It then supplies a NaN/infinite prior world transform and requires finite bounded recovery. The catalog and saved scenarios retain these obligations, all earlier IDs and the full affected selection. This is native numerical/projection proof, without a rendered timing or appearance claim.

## First real selected run and four-file repair recheck

**Historical reviewed source/build stamp:** `dc523e9b07375a1a630982bfe1a6cfea924bbbc0`. **Disposition:** the four-file batch remains sound; the later discovered CR-10 is now resolved above.

The immutable first full selected report `.phase3/runs/win32-x64/28bb1d55-5831-4510-b590-9f140e1689b2/summary.json` remains `fail`: 464 passing and 17 failing cases among 481 selected obligations. Phase 1 supplied 428/433 passing cases; Phase 2 supplied 16/16, including nine actual native G1 cases; Phase 3 supplied all twenty G0 cases passing and twelve native cases failing. The diagnostic `.phase3/task3-first-full-diagnosis.json` retains focus/DPI failures, cleanup/terminal records and the read-only matrix diff. Neither the overall failure nor emergency-cleanup terminations receive passing credit.

- `EngineGeneratedTests` places its genuine handwritten companion under `proofs/companion`. It still requires a successful real consumer build before demanding SCR0001 at the exact handwritten call location. No generator permission, emitted hash, policy scope or diagnostic expectation was refreshed.
- `GraphFixture` retains the freshly clean ports DLL as an explicit binary consumer in the isolated game project, matching its existing Core baseline pattern, then still creates the forbidden Core → ports → Game project edges. This prevents the fixture's setup dependency cycle/missing-contract failure while retaining successful subject compilation before the evaluated graph must reject the actual transitive boundary with source provenance.
- The R-37 matrix row now includes its already-authoritative exact `engine-generated-spans.json` permission link. It matches the unchanged register-driven `RuleMatrix` render rather than weakening the register or consumer.
- The Windows helper explicitly sets and verifies per-monitor-v2 DPI awareness on the executing thread, at boot and per request, so client/screen APIs and SendInput coordinates use physical pixels. It still requires physical 1280×720, actual PID/start-time/window ownership, point ownership, correct foreground and original SendInput/window-message provenance. Focus acquisition uses bounded 10 ms polls around the two native attempts and an owned-window title-point click fallback (100/100/150 ms bounds). Fallback mouse holds participate in the existing final cleanup. The timer uses Windows PowerShell 5.1-compatible Stopwatch APIs. Delivery acknowledgements include physical client dimensions and coordinate/DPI provenance; none replaces actual Godot callback/state observations.

Five actual sequential focused reports now pass: `cf37e352-7f5a-470e-a34e-65fcc9825347` (handwritten partial), `67681743-5e7c-4673-8a8a-06e97a20ff8e` (transitive graph), and `b97b79e8-8a42-4c6c-a28a-8b5b3e9452fb`, `21a87174-2d35-405a-b0a2-d1e1f665bb43`, `b64671cf-5b3c-4fd3-952b-8286a9b132d8` (register binding controls). The reviewer verified their exact subject-log hashes with the correct case-relative base and their actual CTRF reports through the Phase 1 verifier. They retain tested HEAD `d5d6cf0e2bab3a5ff8b7171fad32723d3cbe69c4` with working input `b2073150918d987c0831e1cfe54aa10a9edfe3e29c55929ea06616e4fe7b5c73`, which independently matched the Phase 1 input manifest at the dc recheck. They are not relabelled as post-commit executions. Earlier parallel attempts that hit `CHECK_LOCKED` retain failure history and supply no case pass.

The post-commit pinned strict log `.phase3/batch1-pinned-strict.log` records zero warnings/errors, 10.78 seconds. The hash-bound `.phase3/batch1-driver-boot.json` matches current helper SHA-256 `4f21871237d225c9e4f2f7aa90d75a246154c96771457e4edefeb20a5d0ad98a`, zero exit and empty stderr. It establishes actual Windows PowerShell boot/DPI API/close only, with no game window or input-delivery credit. No build, helper or desktop action was launched by this reviewer.

At the dc recheck, read-only recomputation matched source input `2cbf1a00fe53498dcd9b671d496d8612eab9de44fba14e709fc9e035d6510900`, tool tuple `cb5cbaf54edb8bc2a212badee487e13a9b06b6a1cce0a8343507070f390c4502`, and proof DLL SHA-256 `3a756c2b46f3d7c9f973f3212a90af4c1b59c676e09e66e1f0e7dfeb0bc97e17`. CR-10 is now repaired and independently re-reviewed. Renew the source-bound gate at the current 7b reviewed source, first execute the twelve failed Phase 3 native cases, collect/repair any remaining failures, then obtain the required fresh entire 481-case selected pass. Prior passing pieces do not replace that final full pass.

## Earlier actual SDK SourceLink correction recheck

**Historical reviewed source/build stamp:** `cff08ebe7cc78c562fe370dcdc3718e116938905`. **Disposition:** SourceLink correction remains resolved; current batch and new finding are stated above.

The actual prelaunch metadata-descendant rebuild exposed a gap in the earlier assembly-only pin. `.phase3/task3-build-drift-diagnosis.json` records matching source/build pin but changed proof DLL/tool bytes: Git `LocateRepository` populated `SourceRoot.RevisionId` from actual HEAD, independently of the pinned assembly property. SDK GitHub SourceLink consumed that item metadata. The failure was retained, no hash equivalence was accepted, and no full selected run or game window began. The earlier isolated fixture lacked an actual GitHub remote and did not prove this producer path; the new RED fixture fails its SourceLink-stamp assertion for that exact reason.

Reviewed the five-path repair: `check.cjs`, new `reviewed-source.targets`, `revisions.test.cjs`, LAN finalization documentation and the checkpoint. The imported target computes its repository root from its own file location, normalizes the trailing directory separator, and updates only `SourceRoot` items whose source control is Git and whose exact identity equals that own root. It changes only `RevisionId`, leaving foreign roots, repository URLs, mappings, SourceLink enablement and the actual Git HEAD untouched. With an empty source stamp its target does nothing; outside the Phase 3 environment/manual import, normal SDK import behavior is unchanged.

The installed SDK 10.0.404 sources were inspected directly. `Microsoft.Common.CurrentVersion.targets` imports `CustomAfterMicrosoftCommonTargets`; Git discovery/URL translation occurs before `InitializeSourceControlInformation`; the new `AfterTargets="InitializeSourceControlInformation"` hook therefore runs after the actual source-root items exist and before `GenerateSourceLinkFile` invokes its URL initializers and passes SourceLink to the compiler. The Phase 3 environment supplies the absolute reviewed import to inherited child environments, while the documented strict-build procedure requires both that import and the immutable `SourceRevisionId` pin. No SDK file, global build policy or ordinary product build is modified.

The real fixture copies the actual target into its own repository and adds an actual GitHub origin. It requires nonempty generated SourceLink documents pointing to the reviewed SHA. Five real successful SDK builds in `.phase3/revision-control-LAJhMZ/.phase3/build-records` cover source, review metadata descendant, closure metadata descendant, the negative case omitting only the import while retaining the assembly pin, and restored import. The negative case must expose actual HEAD in SourceLink, produce different DLL bytes and fail completion's exact tool check; restoration must reproduce the original DLL and validate original immutable reports. The retained raw RED `.phase3/task3-sourcelink-red.log` fails the intended SourceLink assertion. GREEN `.phase3/task3-sourcelink-green.log` records 3/3 passing controls; `.phase3/task3-sourcelink-strict.log` records zero warnings/errors, 10.75 seconds. No build or test was rerun by the reviewer.

The actual C#-to-Node `runner.integrity` report `.phase3/runs/win32-x64/77447059-c17a-4601-a2f2-ab952808dd22/summary.json` passes current `loadReport/reconcile` at cff with zero game windows. It binds eight passing integrity controls and their hash-bound process record, including the strengthened SDK fixture. Current read-only fingerprint recomputation matches input `7a8ba8b5e6ad97aa2e5cb12f0b0ee6c0a23fc5a627891da41ba72c4916a0029a`, tool tuple `e7d70caf5dec826ddd4dd1f63a5cf481b5db6d2ad77bada8e4aabe1fd9c4607c` and proof DLL SHA-256 `a3586ca6b2d3adc3856f5e5f088338cceff835d9154cd3cb2ebec4d8715c2db3`. Actual current test/Core SourceLink JSON points to cff. The original 166 full no-window report and e25 focused controls below remain historical executions, not cff full-set evidence.

The parent must refresh/commit the gate for cff, retain both build environment properties, and perform the actual mandatory metadata-descendant rebuild with exact current DLL/tool equality before the fresh `--all`. That real-repository integration check and all desktop acceptance remain pending; the isolated SDK result does not stand in for them. The gate and report continue to retain true execution HEAD separately from reviewed-source build stamp.

## Earlier closure-report integration recheck

**Historical reviewed source/build stamp:** `e25a500d12006e256d84b8b241d0cee9c507f278`. **Disposition:** clean for that focused change; subsequent real SourceLink gap/correction is recorded above.

The exact metadata whitelist now permits only these five additional phase-owned report paths under `.planning/phases/03-four-player-lan-foundation/`: `03-VERIFICATION.md`, `03-VALIDATION.md`, `03-SECURITY.md`, `03-UI-REVIEW.md` and `03-REVIEW-DISPOSITION.md`. These are closure outputs needed by the owning phase workflow. No wildcard, source/tool/selection/scenario path, directory-wide allowance or unrelated report is added. Real commit/HEAD identity, ancestry and every intervening changed path remain checked. Both committed relevant-input trees, live relevant-input equality, dirty non-metadata rejection, exact source build stamp/DLL/tool equality and the original committed review/report byte hashes remain mandatory.

The fixture creates all five named reports before its actual Git closure commit, then exercises the existing full completion validator. Its surrounding negative controls still reject changed source/tool/selection/environment/build stamps, unrelated/nonexistent commits, disallowed and reverted intervening paths, uncommitted relevant changes and altered review bytes. The real SDK control retains exact original execution/report bytes, rejects unpinned binary drift and verifies exact pinned restoration. The parent reports the focused `node --test tools/phase3/revisions.test.cjs` run passed 3/3; this reviewer inspected the test implementation and all five successful actual SDK build records in `.phase3/revision-control-IUOfzw/.phase3/build-records`. The retained strict pinned solution build log `.phase3/closure-integration-build.log` records zero warnings/errors and 10.67 seconds. Neither check was rerun by the reviewer.

The e25 recheck matched its then-current input `a1f23903316f05961192ad385d780360f3e13382d255313a8830082ec5163089` and exact tool tuple `48a74c363799e4cff1c8c87906b220dbc59649953201b436b8189d52bd73c3aa`. That e25 proof DLL SHA-256 was `088ccd4740ac56a49dda5e4fd2ef1404bc06dda4897152e29f82b464cb15b21b`. The historical source/build pin was e25; the prior full no-window run below remains an original execution at 166 and is not relabelled as an e25 execution. The upcoming fresh `--all` must use the current cff reviewed-source pin, SourceLink target import and exact gate inputs. Game code, native/public permissions, generated metadata, scene signatures and the saved case/selection membership are unchanged by this integration.

## Retained pre-integration evidence and limits

The retained repair-batch index `.phase3/iter3-final-1794770502320/summary.json` contains twelve passing jobs: eight policy cases, four Phase 2 core cases, three Phase 2 runner cases, a strict whole-solution build with zero warnings/errors, and twenty Phase 3 non-window cases. That is 35 case executions plus the strict build. The twelve original process records and referenced passing reports were inspected. This is retained execution evidence, not a newly executed reviewer suite.

The retained pre-integration Phase 3 report `.phase3/runs/win32-x64/1be5faf3-4796-4650-8cfa-6316daecff8d/summary.json` records 20/20 passing results, duration 216980 ms, zero game windows, actual source HEAD and pinned build revision `166a32ad6a7541c4f5eed1378def0a86e9bea632`. The reviewer loaded it through the actual current `evidence.loadReport/reconcile` path: all twenty records and their required artifacts reconciled. At that prior review, recomputing its relevant source manifest returned exactly `62c3866e7eb1819c7347a246bc2daab92b46b8228d60505a84139907366dd0d1`; the e25 integration fingerprint is stated separately above.

At the prior 166 source review, the exact recorded tool hash was `c91c021821910af08a06e5b8b744f8d86133fee6c057b6955ba0cbb2333fc651`. Recomputing its then-current declared tuple from Node v24.22.1, the pinned SDK 10.0.404, toolchain bytes and the then-current proof DLL matched it; that prior DLL SHA-256 was `6f35860475ad9136db561878ad7163a18e7b2b3e63a986c66f668533eff11607`. The retained report records Windows x64, Windows 10 Enterprise LTSC 2021 / 10.0.19044, OS fingerprint `4a3c7505cb92e1bb2f7412dd3207a435c55d5c78f2e7e8d11213ecfc2e8768db`. In the preceding review, a full `createIdentity` call did not complete at its nested dotnet probe (`DOTNET_MISSING`); no such execution is credited here. The present follow-up used read-only fingerprint/report reconciliation and launched no build. No DLL was rebuilt or modified by the reviewer.

The hash-bound integrity-control process log `.phase3/controls/9c31880d-e65a-42f2-bd05-5a0f802e7a12/integrity-controls.process.json` records eight successful Node controls, including actual metadata-closure and prelaunch rejection controls. The real SDK finalization fixture `.phase3/revision-control-xFECSZ` retains five successful build records: reviewed-source build, pinned rebuild after review/gate metadata commit, pinned closure rebuild, unpinned negative build, and repinned restoration. The fixture asserts exact DLL identity, rejects changed source/tool/selection/environment/build stamp, unrelated/missing commits, unapproved/reverted intervening paths, dirty relevant input and altered review bytes, and retains unchanged original report bytes. These isolated fixtures establish validator behavior, not rendered production execution.

All five changed code-policy JSON files and `EngineGeneratedSyntax.cs` remain unchanged from `624f37b`. Inspection found 220 exact registry rows and 220 unique authored diagnostic/path/hash/span tuples; the compiler-origin and adjacent-negative controls remain source/hash scoped. The existing fifteen public types include eight Phase 3 additions, and the exact Phase 3 native-method inventory remains 45, with compiler-valid positive/adjacent-negative consumers. No blanket permissions, package additions, new project/reference or generated hash refresh is introduced by the fixes.

Older auxiliary evidence with unpinned prebuild tool drift remains failure/history and receives no current credit. Earlier 66-case body/fixture results remain historical. The fresh full selected 433 Phase 1 + 16 Phase 2 + 32 Phase 3 pass is still Task 3 work.

## Required handoff before a prelaunch gate

The source review is clean at `7b140a27e4f5556db12caed2b1614c52a1aad18a`, with CR-10 and all fourteen earlier findings resolved. The parent may renew/commit the hash-bound gate for this exact reviewed source/input/tool tuple and perform the mandatory metadata-descendant rebuild with both build environment properties. Preserve all failed/passing execution identities. The parent owns gate/commit/publishing and the focused-twelve-native-then-full-481-selected execution cycle.

The parent hash-verified the four temporary iteration-2/3 review/fix scratch originals against their ignored `.phase3/review-history` archives and removed the untracked phase-folder copies. The focused recheck observed no remaining scratch paths in Git status. Preserve that ignored history and the original Git review/fix ledger; the whitelist remains limited to its explicitly enumerated phase metadata paths.

Retain this clean review's exact `SourceRevisionId` pin and `reviewed-source.targets` import through the mandatory metadata-descendant strict rebuild, focused native retry, fresh `--all`, image review and closure validation. True execution HEAD and reviewed-source stamp remain distinct. The parent owns report/gate/closure commits, shared phase state and publishing.

Phase 2 G1 now has its retained actual passing native results, while the twelve Phase 3 native cases still need a passing repaired execution. Phase 3 foreground physical input, sustained background/window-message feasibility, permitted synthetic fallback provenance, 48 conservative rendered timing rows, 16 stopped comparisons, every required image's appearance review and the entire final repaired selected-set pass remain outstanding. A future input/timing/image failure is an acceptance blocker, not a synthetic or state-only pass. No hosted/Linux, mixed-computer LAN, complete A-38/A-45 campaign, packaging, hardware, endurance or Phase 5 certification is claimed.

---

_Reviewed: 2026-11-15T21:22:41Z_  
_Reviewer: the agent (gsd-code-reviewer)_  
_Depth: standard_
