---
phase: 03-four-player-lan-foundation
fixed_at: 2026-11-15T19:28:15.447Z
review_path: .planning/phases/03-four-player-lan-foundation/03-REVIEW.md
iteration: 3
findings_in_scope: 14
fixed: 15
skipped: 0
status: all_fixed
iteration_findings: 1
iteration_fixed: 1
---

# Phase 3: Code Review Fix Report

**Fixed at:** 2026-11-15T19:28:15.447Z  
**Source review:** [03-REVIEW.md](03-REVIEW.md)  
**Iteration:** 3; `critical_warning`; cumulative nine Critical/BLOCKER and five Warning findings. This iteration attempts only CR-09; all thirteen earlier dispositions, commits and verification history remain below.

All fourteen findings have implementation fixes and executed regression checks. None was skipped. The latest independent review rechecked all thirteen earlier findings as resolved. Their original fourteen atomic fix/test commits and four iteration-2 commits remain preserved. One coherent atomic commit addresses CR-09. Earlier report/review backups and `.phase3/review-second.md` retain the prior checkpoints; the parent owns archiving temporary phase-folder backups under ignored review history before gate authoring. This cumulative fixer report does not assert an independent clean re-review, authorize the rendered gate, or complete Plan 03-04 Task 3. The canonical independent review remains the parent's CR-09 finding until its separate recheck.

The `fixed: requires human verification` disposition below is the fixer's required flag for logic/state/evidence changes. Behavioral regressions have executed; syntax/build success alone does not establish semantic correctness. Independent re-review remains the next workflow step; no additional creator approval is requested by this report.

## Fixed Issues

### CR-01: Restoring row focus recursively rebuilds the browser

**Status:** fixed: requires human verification  
**Commit:** `a29602d`  
**Files modified:** `src/godot-game/foundation/FoundationMenu.cs`, `FoundationAutomation.cs`, new `FoundationNativeRegression.cs`, new `FoundationRegressionDiscovery.cs`; `tests/phase1/EngineAccessPolicy.cs`, `FoundationLanPermissionTests.cs`; `tools/phase3/network.cjs`.

The menu commits its refreshed key before constructing rows, creates the complete collection before restoring focus, and keeps the focus-detail callback out of `Rows()`. Selection styling is applied by the ordinary tick. The actual native regression focuses a real row, changes occupancy, inserts a row sorting before it, verifies exact selected identity/focus, then verifies an unchanged tick retains the same node instance. It runs inside `network.discovery-profile` through the nonce-scoped headless automation route and production menu handlers.

### CR-02: Draw observation dereferences UI nodes that have never been created

**Status:** fixed: requires human verification  
**Commit:** `494a197`  
**Files modified:** `src/godot-game/foundation/FoundationDrawObserver.cs`, `FoundationNativeRegression.cs`; `tests/phase1/EngineAccessPolicy.cs`, `FoundationLanPermissionTests.cs`.

Observation enumerates actual native row children and records each row's stored session identity. Service discovery counts no longer manufacture mandatory node paths. Serialization accepts an absent profile and preserves the profile failure reason in the observed input sample. The regression invokes the production observer/control serializer against hidden and actual pending-join menu states whose native and service row sets differ. It also loads corrupt owned profile bytes, observes the null/error state, and explicitly recovers a profile. Headless inspection records `drawn: false`; it supplies no completed-render evidence or UI mutation through the observer.

### CR-03: Rejected hellos permanently disable session admission

**Status:** fixed: requires human verification  
**Commits:** `10b83d6`, `bf94821`  
**Files modified:** `src/multiplayer-abstractions/IConnectionService.cs`, `ConnectionPacket.cs`; `src/godot-game/foundation/EnetConnectionService.cs`, `FoundationCoordinator.cs`, `FoundationCoordinator.Admission.cs`, `FoundationCoordinator.Lifetime.cs`, `FoundationPeerBinding.cs`, new `FoundationAdmissionHistory.cs`; `tests/phase1/FoundationHarnessConnection.cs`, `FoundationTestConnection.cs`, `FoundationConnectionReplacement.cs`, `FoundationLanAdmissionTests.cs`, `KingdomZombies.Phase1.Tests.csproj`.

Active reservations remain separate from replay history. Each authenticated transport connection has at most 32 retained outcomes and a 30-second admission window. Expiry clears outcome payloads and closes that connection's admission window until reconnect, preserving a stale-attempt fence. Disconnect/reconnect tokens are created by the provider and copied into packet metadata; old queued generations are ignored. History from a disconnected connection is retired without recreation during timeout removal. The provider's existing 16-connection transport bound also bounds live history owners.

Actual coordinator regressions send 160 duplicate-profile refusals and 160 Full refusals, release a reservation, and admit a distinct legitimate guest. They verify duplicate/closed outcomes, expiry and same-sender reconnect fencing. Thirty-two repeated admission/disconnect/timeout cycles assert that history returns to zero. Real headless ENet tracer, four-peer and lifecycle cases exercise the adapted port.

### CR-04: Opaque actor IDs are interpreted as native node paths

**Status:** fixed: requires human verification  
**Commit:** `3a82df1`  
**Files modified:** new `src/godot-game/foundation/FoundationActorNode.cs`, `FoundationView.cs`, `FoundationDrawObserver.cs`, `FoundationNativeRegression.cs`, `FoundationAutomation.cs`; `tests/phase1/FoundationLanProtocolTests.cs`, `FoundationLanPermissionTests.cs`, `EngineAccessPolicy.cs`, `KingdomZombies.Phase1.Tests.csproj`.

Exact ordinal UTF-16 identity is encoded injectively into an ASCII hexadecimal internal node name before any native name/path operation. The model and wire identity remain unchanged. Projection and observation use the same mapping. Wire round trips and actual native projection cover slash, colon, parent segments, sanitizer-collision candidates and canonically distinct Unicode. Two projections retain three distinct remote nodes and the untouched Menu node.

### CR-05: Stopped-position evidence is detached from actual stopped actors and draws

**Status:** fixed: requires human verification  
**Commits:** `7e9664e`, `7f9bfd0`  
**Files modified:** `tools/phase3/evidence.cjs`, `rendered.cjs`, new `stopped.cjs`, new `stopped-fixture.cjs`, `timing.test.cjs`.

Each comparison references immutable sampled observer/host records that must match the actual peer NDJSON draw frames. The full reconciler binds the current origin actor/session/profile/binding, exact native release acknowledgment and host-accepted zero-direction intent. It derives and checks positions/revisions against those records, checks frame/revision freshness, requires observation after release/native acknowledgment, and bounds uncertainty to at most 1 ms before evaluating the inclusive 50-subunit/500-ms limits. Every capture also stores its own actual local subject and revision, so later final-scene progress cannot invalidate an earlier valid timing image.

The full-reconciler control rejects wrong actors/sessions, substituted observer/host positions or draws, stale authority/frame/revision, altered release acknowledgments, pre-release observations and excessive uncertainty. Scalar equality/one-nanosecond/one-subunit boundary controls remain. The actual capture consumer is tested with an earlier revision and a later final-scene minimum. Counterfeit fixture images/records prove rejection only and earn no rendered credit.

### CR-06: A required rendered peer can exit nonzero after capture and still pass

**Status:** fixed: requires human verification  
**Commits:** `ab4cb05`, `640e1c3`  
**Files modified:** `src/godot-game/foundation/FoundationDiagnostics.cs`, `FoundationRoot.cs`; `tools/phase3/network.cjs`, `scenarios.cjs`, `evidence.cjs`, new `terminal.cjs`, new `terminal-fixture.cjs`, `runner-fixture.cjs`, `stopped-fixture.cjs`.

Every owned peer receives a terminal record after its output streams close. Ordinary exit requires the production Quit-handler acknowledgment, a prior ordinary Quit request and zero exit without signal. Deliberate guest/host loss has an explicit case/PID/nonce declaration before the owned kill; emergency cleanup is never an expected-success termination. Unexpected exit invalidates every result using the affected state/process, including earlier completed captures. Retired guest/restart processes remain in owned-process bookkeeping.

Real Node children traverse the actual rendered-peer close lifecycle. A completed call to the production capture consumer precedes the post-capture/crash failure. Normal Quit and predeclared expected loss establish valid controls; post-capture nonzero, crash, failure during Quit and late expected-loss declaration must fail. Full reconciliation also rejects missing/tampered terminal records. These are real child-lifecycle rejection controls, without game-render or physical-input credit.

### WR-01: Capture acknowledgment precedes complete JSON publication

**Status:** fixed: requires human verification  
**Commit:** `df4b90d`  
**Files modified:** `src/godot-game/foundation/FoundationDrawObserver.cs`, new `FoundationCapturePublication.cs`; `tests/phase1/FoundationLanRunnerTests.cs`, new `FoundationCapturePublicationTests.cs`, `KingdomZombies.Phase1.Tests.csproj`; `tools/phase3/rendered.cjs`, `timing.test.cjs`.

The producer closes a complete unique same-directory temporary JSON file and atomically moves it to the final checkpoint path after PNG completion. The move is the acknowledgment. The consumer's end bracket follows state/image reads, validation and hashing. The actual C# helper is checked before move, after move and against overwrite; delayed publication exercises the actual polling consumer without exposing partial final JSON.

### WR-02: Inherited navigation assumes a two-button menu

**Status:** fixed: requires human verification  
**Commit:** `28ab751`  
**Files modified:** `src/godot-game/foundation/FoundationRoot.cs`, `Foundation.tscn`; `tools/phase2/rendered.cjs`, new `menu-navigation.cjs`; `tools/phase3/runner-fixture.cjs`, new `navigation.test.cjs`.

The observer exposes the actual focused control name. Explicit current scene neighbors and the inherited procedure cover the complete seven-control Tab/Shift+Tab cycle and Start/Host/Join/Quit Down/Up traversal. Hover/press, recoverable entry failure, fresh retry, all movement checks and final ordinary keyboard Space on Quit remain. The procedure is checked against actual authored scene neighbors. All sixteen inherited IDs remain selected; genuine native rendered keyboard execution remains Task 3 work.

### WR-03: Saved selection omits affected analyzer and generated/register consumers

**Status:** fixed: requires human verification  
**Commits:** `51b6718`, `9f72121`  
**Files modified:** `tools/phase3/catalog.cjs`, new `selection-impact.cjs`, `selection.test.cjs`, `runner-fixture.cjs`; `03-SELECTION.json`, `03-SCENARIOS.md`; `docs/workflow/lan-foundation-checks.md`; `tests/phase1/BuildFixture.cs`, `ExemptionValidator.cs`, `ExemptionTests.cs`, `SuppressionTests.cs`, new `FoundationLinkedSources.cs`.

Selection now includes 433 active Phase 1 cases: the original 124 plus 309 affected cases. It includes all 31 body/R-37 cases, all six shared generated-permission analyzer families, analyzer input/source/boundary, generated/callsite/companion, register/exemption and actual shared-source/test-fixture/xUnit consumers. There are 243 individually explained active omissions and eight unchanged planned Phase 5 declarations. All 32 Phase 3 and all 16 inherited Phase 2 IDs remain selected. Every known affected omission is rejected before launch, even with a nonblank excuse.

Executing the newly selected checks exposed stale consumers. Exact generated-origin assertions now require the sixteen existing origin-record IDs and all file hashes; the existing suppression permission count is 26. An independent immutable inventory lists exactly 26 linked engine-free source paths. Ownership requires the exact original project plus proof project for those paths, and rejects missing, duplicate, adjacent or extra owners. Isolated test fixtures copy those real links and the unchanged local Godot editor policy, so intended rejection controls begin with compiler-valid setup. No permission definition, diagnostic severity or generated hash was expanded/refreshed.

### WR-04: Concurrent foreground clicks race for the single foreground window

**Status:** fixed: requires human verification  
**Commit:** `e9bc36a`  
**Files modified:** new `tools/phase3/foreground.cjs`, new `foreground.test.cjs`, `scenarios.cjs`, `rendered.cjs`, `evidence.cjs`, `runner-fixture.cjs`; `03-SCENARIOS.md`; `docs/workflow/lan-foundation-checks.md`.

Physical operations share one coordinator queue across helpers. Focus-plus-click and text sequences keep ownership through completion. Final-slot contenders are prepared serially through ordinary physical click/Tab, then Enter is dispatched concurrently to the owned PIDs using native window messages. Exact driver/native callback acknowledgments and focused preparation draw frames are required. Provenance remains `window-message` with no physical-input credit; separate genuine foreground input/focus obligations remain mandatory. The Windows helper's ownership/foreground/point checks are unchanged. Actual exported procedures are regression-tested with controlled helpers; real desktop feasibility remains Task 3.

### CR-07: Completion validation rejects the metadata commit needed to preserve its own evidence

**Status:** fixed: requires human verification  
**Commits:** `ba56010`, `a35bd6f` (WR-05 commit `6b24056` also adapts its fixture to the required inherited command records).  
**Files modified:** new `tools/phase3/revisions.cjs`, new `revisions.test.cjs`; `catalog.cjs`, `check.cjs`, `evidence.cjs`, `runner-fixture.cjs`; `tests/phase1/FoundationLanRunnerTests.cs`; `docs/workflow/lan-foundation-checks.md`; `03-04-CHECKPOINT.md`.

Finalization preserves the actual tested revision, original nonce and original report/review bytes. Actual Git commits must exist with the tested/reviewed commits in current HEAD's ancestry. Every intervening path, including changes later reverted, must belong to the explicit phase-metadata list. Current input/tool/platform/OS equality remains mandatory, relevant current bytes must match the tested and closure Git trees, and Git query/state failures or dirty nonmetadata inputs fail closed. Original inherited fingerprints are checked against the tested tree, including their broader documentation inputs, so later permitted shared-state closure changes do not rewrite historical inputs.

Prelaunch requires the reviewed source and exact Markdown/gate bytes to be committed before execution. It makes exclusive byte-for-byte invocation copies of the original review JSON and Markdown; completion verifies those hashes against the execution commit, allowing later canonical review edits without replacing the original binding. The explicit whitelist also includes the two already-created `iter2` history artifacts, with no wildcard permission. No earlier execution is relabeled or broadly reused.

The real build exposed a second cycle: the SDK appends current HEAD to `AssemblyInformationalVersion`, changing the raw proof-DLL/tool hash after a metadata commit. The exact solution pins the phase's MSBuild `SourceRevisionId` to the reviewed source commit before its first strict build, then retains that pin through the review/gate commit, mandatory strict rebuild, inherited builds, execution and closure rebuild/validation. `createIdentity()` records `buildRevision`; prelaunch requires it to equal the original reviewed revision and completion requires the same original pin. Exact raw DLL/tool equality remains mandatory. Actual execution `revision` remains the real HEAD, independent of the source stamp. The SDK targets' conditional `SourceRevisionId` assignment and generated version attribute were inspected locally; no global build property, hash normalization or binary equivalence was added.

The workable authoring flow is source commit → export its immutable `SourceRevisionId` → strict build → independent review and exact identity capture → review/gate metadata commit → strict rebuild and fresh `--all` with the same pin → actual image review/index/summary metadata commit → pinned rebuild if needed and completion validation from that closure checkout. Do not reset the pin to newer metadata HEADs. The checkpoint and LAN checks now state this exact sequence.

Three actual isolated-Git controls exercise the production review gate and complete completion validator. They reject changed source/tool/selection, wrong tool/platform/OS/input/build identities, unknown/malformed/unavailable Git revisions, unrelated/nonancestor commits, undeclared paths and a reverted undeclared change, while asserting unchanged original report/review bytes. The third control performs five real SDK builds across source/review/closure commits: pinned builds retain identical DLL bytes, the unpinned negative produces different bytes and fails, and restoring the original pin restores the exact DLL and validates the unchanged report. Actual process/build records and hashes are retained in the owned fixture and hash-bound control process log. Constructed evidence fixtures prove validator behavior only and receive no game execution credit.

### CR-08: Join session can silently join a replacement session at the selected endpoint

**Status:** fixed: requires human verification  
**Commit:** `3b43287`  
**Files modified:** `src/godot-game/foundation/FoundationCoordinator.cs`, `FoundationCoordinator.Admission.cs`, `FoundationCoordinator.GuestPublication.cs`, `FoundationCoordinator.Lifetime.cs`, `FoundationMenu.cs`, `FoundationRoot.cs`, `FoundationNativeRegression.cs`; `tests/phase1/FoundationLanAdmissionTests.cs`; `03-SCENARIOS.md`.

Browser submission carries the exact selected session ID through the existing root handler into one coordinator lifetime and the hello's existing session field. The host rejects mismatches before reserving; the guest rejects a mismatched baseline before installation/readiness/acknowledgment and sends cancellation to release any offered reservation. The expected ID clears on close and menu request completion/cancellation. Manual address submission supplies an empty expected ID. The existing attempt/generation fences also reject stale nonempty-attempt refusals before they can close a new pending join. No generated Root signature or identity/Unicode normalization changed.

The production coordinator control retains A, closes/rehosts B at the same endpoint and checks selected A→A success, A→B visible refusal/no actor/control/slot/resource leak, intentional manual→B success, wrong-baseline rejection without acknowledgment, reservation cancellation, and old baseline/refusal delivery after cancel/new attempt. The native browser regression invokes the production selected-row handler, inspects its exact synchronous session/address request and confirms clearing. The actual headless multicast/profile-restart selected-row route also executes. These combined controls supply no rendered stale-host acceptance credit; actual desktop recovery remains Task 3.

### WR-05: First inherited failure aborts independent checks and drops accumulated report linkage

**Status:** fixed: requires human verification  
**Commit:** `6b24056`  
**Files modified:** `tools/phase3/inherited.cjs`, new `inherited.test.cjs`, `processes.cjs`, `check.cjs`, `evidence.cjs`, `revisions.test.cjs`, `runner-fixture.cjs`; `tests/phase1/FoundationLanRunnerTests.cs`; `docs/workflow/lan-foundation-checks.md`; `03-SCENARIOS.md`.

Every attempted inherited command now persists its real process/outcome/stdout/stderr records and original child-report hash, including nonzero exits. Ordinary assertion failures retain partial passing/failing outcomes and continue independent commands. Missing reports produce blocked cases with actual process/setup diagnostics. Phase 2 core and runner run independently before its one shared nine-case rendered process. That rendered group and Phase 3 native groups have explicit selected integrity/Phase-2-core dependencies; failed prerequisites create named unexecuted blockers without process/pass credit. The outer report retains all accumulated references and fails for any inherited failure/blocker even when independent native scenarios pass. Completion rejects failed/blocked aggregates and command records.

Three controls execute actual tiny Node children through the exported inherited runner. A valid early assertion failure is followed by independent success and complete immutable report/process/log linkage. Setup, missing-report and integrity cases distinguish executed failure from unexecuted dependent blockers. The actual outer `main()` is exercised with only its review/native dependencies isolated; its published summary retains the failed inherited reports and returns failure after the independent native dependency succeeds. These controls receive no rendered credit. The scoped Phase 3 C# fixture wrapper allows a bounded 40 seconds for actual Git/SDK/child controls and kills only its owned tree; shared `ProofProtocol`, the inherited 45-second case deadline and all unrelated deadlines remain unchanged.

### CR-09: Accepted backwards actor time poisons the native camera with NaN

**Status:** fixed: requires human verification  
**Commit:** `166a32ad6a7541c4f5eed1378def0a86e9bea632`  
**Files modified:** `src/godot-game/foundation/FoundationCoordinator.GuestPublication.cs`, `FoundationView.cs`, `FoundationNativeRegression.cs`; `tests/phase1/FoundationLanProtocolTests.cs`; `tools/phase3/required-cases.json`; `03-SCENARIOS.md`.

Guest publication compares every continuing ordinal actor identity against the installed snapshot before baseline/state handling. Any elapsed-time regression rejects the entire candidate before publication, readiness, installed revision, acknowledgment or response-liveness changes. Equal counters and arbitrarily large advancing nonnegative counters remain valid. A genuinely new actor identity may start from zero; a fresh coordinator/session accepts its own elapsed origin. The state revision remains the publication high-water mark, so a rejected revision 100 cannot suppress the next valid revision 1 or 2.

The existing guarded native `FoundationView.Project` reads local monotonic `Time.GetTicksMsec()` and its own `projection_tick` metadata. It bounds the render interval to 0–0.25 seconds, retains the 0.15-second exponential smoothing constant, the ±320 camera clamp and first-projection snap, and never derives render time from network actor counters. Explicit finite-result guards precede player/world transform writes; a nonfinite prior camera recovers to its finite target. Server three-units/second stepping, input, facing and detached wire shape remain unchanged. No new native method tuple, Root generated signature, public type, package, project/reference, suppression or generated permission refresh was needed.

Production coordinator controls install huge late and zero-origin fresh baselines, then reject higher-revision regressions independently for host/local continuing actors, including rejection before readiness. They assert the identical installed snapshot, unchanged publication/installed revision, readiness and reply/ack count, plus exactly one rejection. Subsequent lower valid revisions, equal/advancing huge counters and a replacement actor at zero install successfully. A rejected packet at 9999 ms cannot extend the original 10000 ms response deadline. The controls run again in a fresh session lifetime.

The existing nonce-scoped native regression invokes actual projection at position 1000 through `long.MaxValue → 0 → 0 → long.MaxValue - 1 → long.MaxValue`, checking finite world/camera/player transforms and the exact initial snapped target. It also injects a nonfinite prior world transform and requires finite bounded recovery. This runs through the actual headless `network.discovery-profile` process and existing automation action. Original browser/opaque-ID regressions remain in the same native helper. Scenario/catalog prose records these added obligations; every selected ID and omission remains unchanged. These checks provide no rendered, physical-input or timing acceptance credit.

## Verification

All iteration-3 gates ran sequentially in the **main checkout** `D:/Work/Games/KingdomZombies`, branch `feature/03-four-player-lan-foundation`, with `workflow.use_worktrees=false`. The root/branch pin ran before the first edit and the atomic fix commit. No worktree, branch change, remote push/merge, real game window, VM or external input driver was used. Source stayed fixed throughout each report snapshot.

Development builds used the then-current source commit `a35bd6f88aa64c62d48f7af69134a56ad3a028e1`. After committing the repair, the source SHA and immutable `SourceRevisionId` build stamp were both changed to **`166a32ad6a7541c4f5eed1378def0a86e9bea632`** before the first strict rebuild. All final strict/inherited builds retained that new pin. Do not restore the old a35 stamp or advance this stamp to later metadata commits. The actual final execution `revision` is also the new source SHA.

| Iteration-3 check | Result | Duration / immutable evidence |
| --- | --- | --- |
| Focused coordinator, exact permissions and actual headless projection/discovery | 3/3 pass, zero game windows | 84100 ms; `.phase3/runs/win32-x64/a3be3257-a412-4dc0-8359-269cb043b835/summary.json`; development source stamp, committed tested source bytes |
| Initial post-commit strict solution rebuild with the new source pin | Zero warnings, zero errors | 10.98 s compiler; `.phase3/iter3-pinned-initial-build.log` |
| Eight affected Phase 1 policy cases | 8/8 pass: source, generated origin/boundary, callsite, visibility/composition, format and docs | Every original report/process/log indexed in `.phase3/iter3-final-1794770502320/summary.json` |
| Inherited Phase 2 core | 4/4 pass | 43866 ms; `.phase2/runs/win32-x64/f5324eab-94ad-4e6b-910a-cc90f57330ae/summary.json` |
| Inherited Phase 2 runner | 3/3 pass | 9290 ms; `.phase2/runs/win32-x64/7b0bf0de-eae6-43a7-960f-a1a5b3edd81c/summary.json` |
| Final strict solution build | Zero warnings, zero errors | 6.49 s compiler / 6727 ms process; batch `strict-build.console.log` |
| Entire current Phase 3 non-window set, executed last | 20/20 pass; zero game windows | 216980 ms report interval / 217475 ms process; `.phase3/runs/win32-x64/1be5faf3-4796-4650-8cfa-6316daecff8d/summary.json` |
| Exact original-report/current-identity reconciliation | Pass at 19:28:15.447Z; original report SHA-256 `0bed5b07b1fd680a7315a6a5ddbf29e47d4c742a871e1a9936fbf0f52e30d15c` | `.phase3/iter3-final-1794770502320/identity-validation.json`; no new execution credit |

The final sequential batch passed all twelve jobs, representing **35 case executions plus the strict build**, in **381585 ms**. Its before/after input, tool, revision, build stamp, platform and OS identities match exactly; the unchanged final report was loaded through production `evidence.loadReport` and reconciled against the current checkout. Current input fingerprint: `62c3866e7eb1819c7347a246bc2daab92b46b8228d60505a84139907366dd0d1`. Tool fingerprint: `c91c021821910af08a06e5b8b744f8d86133fee6c057b6955ba0cbb2333fc651`. Actual OS: Windows 10 Enterprise LTSC 2021, `10.0.19044`, x64; OS fingerprint `4a3c7505cb92e1bb2f7412dd3207a435c55d5c78f2e7e8d11213ecfc2e8768db`. Toolchain remains Node 24.22.1, .NET SDK 10.0.404 and pinned Godot .NET 4.7.2.

All 220 exact generated permissions and the native/public registries remain unchanged from iteration 2. Exact native proof remains `45_TUPLES_8_PUBLIC_TYPES_WITH_VALID_NEGATIVES`; the inherited `FoundationView.Project` source/guard controls also pass in Phase 2 core. No tracked file was created. All six committed repair paths are already members of the unchanged **115-path** full review union `.phase3/review-fix-union.txt`. The unchanged 66-case body/analyzer/fixture suite was not repeated; the affected current-source, permission, consumer and strict-build checks above executed afresh. The final full 433 Phase 1 + 16 Phase 2 + 32 Phase 3 saved selection and actual desktop/image review remain Task 3.

### Retained iteration-2 verification

The following checkpoint is historical. Its references to final/current/matching describe iteration 2 and are superseded by the new source-stamped iteration-3 verification above.

All iteration-2 gates ran sequentially in the **main checkout** `D:/Work/Games/KingdomZombies`, branch `feature/03-four-player-lan-foundation`, with worktrees disabled. The root/branch pin ran before the first edit and every fix commit. No worktree, branch change, remote push/merge, game window, VM or external input driver was used.

The final tested revision and immutable build stamp are both `a35bd6f88aa64c62d48f7af69134a56ad3a028e1`. The initial strict build established that exact pin before the final batch; every inherited build retained it. All twelve jobs passed (35 case executions plus strict build), in 379290 ms. Before/after source/tool/revision/platform/OS/build-stamp identities agree, and the unchanged final 20-case report was reconciled again against the current identity at 18:59:14.560Z. This validation supplies no new execution.

| Iteration-2 final check | Result | Duration / immutable evidence |
| --- | --- | --- |
| Eight affected Phase 1 policy cases | 8/8 pass: source, generated origin/boundary, callsite, visibility/composition, format and docs | All individual report/process/log references indexed in `.phase3/iter2-final-1794768701111/summary.json` |
| Inherited Phase 2 core | 4/4 pass | 42606 ms; `.phase2/runs/win32-x64/cf70d487-ab60-4e87-85cc-f54a01ed6ab4/summary.json` |
| Inherited Phase 2 runner | 3/3 pass | 9229 ms; `.phase2/runs/win32-x64/494c3686-7f5a-4052-8648-fc17dbf1db0e/summary.json` |
| Strict solution build | Zero warnings, zero errors | 6.56 s compiler / 6796 ms process; batch `strict-build.console.log` |
| Entire current Phase 3 non-window set, executed last | 20/20 pass; zero game windows | 216589 ms report interval; `.phase3/runs/win32-x64/0a4dfcdd-c6d3-4d94-a9c8-98b99aeeef85/summary.json` |
| Exact original-report/current-identity validation | Pass; original report SHA-256 `22caaca1d50fd7dc2d7807cbccc51faf165ec8bd94f8ff62b2046bf22e9ea9ed` | `.phase3/iter2-final-1794768701111/identity-validation.json` |

Current input fingerprint: `6f1900ae9fc8837d85b51d6db8629cb3f2f720024cb80877805caa4e64757849`. Tool fingerprint: `adfb30a7635567dd80fc31910bb2b8e2b80a2ceb1ab08bfe59b452e9b538d1da`. OS fingerprint: `4a3c7505cb92e1bb2f7412dd3207a435c55d5c78f2e7e8d11213ecfc2e8768db` (Windows 10 Enterprise LTSC 2021, `10.0.19044`, x64).

The final `runner.integrity` proof names the actual SDK fixture `.phase3/revision-control-n5JBbF`: source `4acbfa2ae18bb3221e1431372e5aab19221c0e64`, execution `fb23c8f7facd9270c8f4132955310459c199ebe8`, closure `76a2e2c55b0eaf74cba05570436350c91e606933`, original exact DLL SHA-256 `8d9f75acd598e906a4709caa08360639dec5f6c360292171aad1f905610a3b35`. Its five successful real build records are in that fixture's `.phase3/build-records/`. The enclosing actual Node process record is `.phase3/controls/eac4ad55-41f6-487b-bf4a-4e6a162ec1d5/integrity-controls.process.json`, hash `9b14ce027e26069571b27fd1600bb9a94be1ad2cb9df7cc927992d12b89dba9b`. This is reproducible validation/control evidence, without game execution credit.

All 220 generated permissions, package pins, five-project boundaries and public/native permission registries remain unchanged. Exact native proof remains `45_TUPLES_8_PUBLIC_TYPES_WITH_VALID_NEGATIVES`. Every changed tracked file uses LF UTF-8 without BOM. The earlier 66-case body/analyzer/fixture batch was not repeated: those implementations/policy inputs were unchanged; the current affected production/source/permission/consumer checks above were executed afresh.

### Retained iteration-1 verification

The following section preserves the prior checkpoint. Its references to final/current/matching describe the iteration-1 tree and are historical, superseded by the iteration-2 evidence above.

All gates ran sequentially in the **main checkout** `D:/Work/Games/KingdomZombies`, on `feature/03-four-player-lan-foundation`, because `workflow.use_worktrees=false`. No worktree, temporary branch or recovery sentinel was created. The root/branch pin ran before the first edit and every commit.

Final committed/tested revision: `9f7212183134744f90b03767b1c1e6afffe08a22`. Windows 10 Enterprise LTSC 2021, `10.0.19044`, x64. No real game window, VM or external input driver was launched.

| Final check | Result | Duration / evidence |
| --- | --- | --- |
| Focused Phase 1 current-source controls | 66/66 pass: 31 body, 21 policy/analyzer/generated/register, 14 fixture/ownership/restore | 579264 ms; every individual report is indexed in `.phase3/reviewfix-final-1794764396971/summary.json` |
| Inherited Phase 2 core | 4/4 pass | 42488 ms; `.phase2/runs/win32-x64/d8278577-31a5-4743-8d9a-e86b1bfc74ab/summary.json` |
| Inherited Phase 2 runner | 3/3 pass | 8997 ms; `.phase2/runs/win32-x64/639ea2a8-46fd-482e-9f85-447a1bbf3de6/summary.json` |
| Strict whole-solution build | Zero warnings, zero errors | 6.32 s compiler time / 6538 ms process; `.phase3/reviewfix-final-1794764396971/strict-build.console.log` |
| Entire current Phase 3 non-window set, executed last | 20/20 pass; zero game windows | 189264 ms subject/report interval; `.phase3/runs/win32-x64/57ee4949-87df-4551-8bdc-37a6686a2d89/summary.json` |

The final batch contains 70 successful jobs, representing 93 case executions plus the strict build, and took 827334 ms. Its immutable per-command logs/process records and individual report references are in `.phase3/reviewfix-final-1794764396971/`. This is focused repair verification, not the eventual entire saved rendered selection.

The final Phase 3 report's source/tool/revision/OS identity was recomputed after completion and matches the checkout:

- Input: `bfa3594d742dd8dabc968b85dab8269fd50a2cc4373a39937d90a8060aebe268`.
- Tool: `1d109333ce0d2c433e908e70cf0c86fb6eb9eeafcc1728ee52fa6bb24a44a715`.
- OS: `4a3c7505cb92e1bb2f7412dd3207a435c55d5c78f2e7e8d11213ecfc2e8768db`.
- Exact native proof: `45_TUPLES_8_PUBLIC_TYPES_WITH_VALID_NEGATIVES`. The three additional native tuples are `FoundationNativeRegression.Browser`, `FoundationNativeRegression.Projection` and `FoundationDrawObserver.ObserveControls`.

All 220 generated hash/span permissions and the generated-callsite/exemption/EngineGeneratedSyntax data are unchanged from review commit `624f37b`. FoundationRoot's generated signatures remain stable. The five-project graph, package pins and public type inventory are preserved. No suppression, wildcard permission or alternate diagnostic bypass was added.

## Retained failures and repair history

Original reports remain intact. They are not substituted for the final current-source run:

- Iteration 3's first focused report `.phase3/runs/win32-x64/2f10dccb-603d-4b1c-8a31-299ff51bc205/summary.json` retains the actual 1/3 outcome. The coordinator control passed; native projection rejected the new hyphenated metadata key as an invalid Godot identifier, while the exact permission control correctly rejected native property accesses introduced inside an unguarded LINQ lambda. The key now uses `projection_tick`, and the original direct guarded child enumeration is preserved. Renewed focused 3/3 and final 20/20 runs above pass without permission expansion. The initial restricted-sandbox build logged `Build FAILED` with no compiler diagnostics (`.phase3/iter3-initial-build.log`) and receives no pass credit. The first actual strict build retained the new continuation-indentation diagnostic in `.phase3/iter3-initial-build2.log`; its corrected build and `.phase3/iter3-native-repair-build.log` pass with zero warnings/errors. All failures remain visible and no original report was rewritten.

- Iteration 2's initial `core.admission` expectation assumed a fixed revision after multiple valid zero-elapsed host updates. The actual baseline was newer. `.phase3/runs/win32-x64/59bad5fb-3f0d-455d-8c5d-b6451b069c14/summary.json` preserves the 1/2 outcome; the corrected assertion checks installation/readiness without imposing an unrelated revision constant. Its actual native selected-row case already passed. The subsequent focused four-case report is `6ebd7023-366b-4ecf-bf75-5e39cc301671`.
- The expanded real Git/child controls first exceeded the old five-second shared wrapper: `.phase3/runs/win32-x64/b70a3a05-d21b-4612-a8b8-d6e249f2cc7c/summary.json` retains both failures. Only the Phase 3 fixture wrapper was adapted, finally bounded at 40 seconds for the real SDK control; shared proof helpers and inherited case deadlines remain unchanged. Passing focused reports `a29c239b-5892-4b02-a42e-8e20759aff2f`, `642a1fe4-06d0-4e3b-b41f-5f31511d5501` and `c1d7f358-f1ec-40df-bdc5-1bd45e8444ec` remain history.
- `.phase3/iter2-cr08-build.log` retains the member-order diagnostic; `.phase3/iter2-wr05-build.log` retains the temporary using-placement syntax diagnostics. Both were corrected with normal strict checks, without suppressions. Isolated SDK-control setup initially rejected invalid marker NuGet/package inputs (`.phase3/revision-control-yTxvdz`, `.phase3/revision-control-hQdYhm`); those setup failures earned no intended-rejection credit. Correct complete SDK fixture inputs subsequently build successfully before each positive/negative control.
- `.phase3/iter2-final-1794767582941/summary.json` preserves all twelve passing jobs (35 case executions plus strict build, 358600 ms), and its auxiliary guard's pre-/post-build tool difference. The initial identity preceded the first canonical rebuild after commit `6b24056`, whose SDK version attribute changed the proof DLL hash. Its last actual 20/20 report `33477dd8-ef20-42e4-a1b2-d084718d66dd` matched that then-current source/tool/revision, as separately recorded in that directory's `identity-validation.json`; neither report was rewritten. This exposed the real build-stamp cycle and prompted CR-07 follow-up `a35bd6f`. The renewed final batch below uses the explicit pin and supersedes that earlier source state.

- The first broad verification batch retained 52 passing and three failing jobs in `.phase3/reviewfix-final-1794763275754/progress.json`; all 31 body cases passed there. Companion-positive failed at `.phase1/runs/ffce8485-6596-4b71-836c-86255b47822c/summary.json`, companion-negative at `.phase1/runs/7ad5dd1f-644c-42ca-9517-14054613e9da/summary.json`, and ownership at `.phase1/runs/d66fb6c3-f0b8-481e-8479-27db0bca65d0/summary.json`. A path-format error in that auxiliary batch indexer prevented its aggregate summary; the individual reports/progress remained intact. The final indexer uses the canonical root and completed successfully.
- After ownership was corrected, the stale 18-versus-26 count failed separately at `.phase1/runs/287040ad-27ab-44b0-89a4-6c414ae8438d/summary.json`. The final exact count and generated-origin checks pass.
- The isolated test-consumer setup initially lacked the production local editor policy. Both R-46 controls correctly failed setup and earned no intended-rejection credit; one retained failure is `.phase1/runs/5cb70d62-1f21-4e7f-acc1-e6f7b64218e7/summary.json`. Complete production-equivalent inputs now compile before the intended controls; all related xUnit producer and restore controls pass in the final batch.
- The initial terminal-control fixture had a relative child path setup error; `.phase3/terminal-control-gRYFwk/terminal-normal/` retains that log/terminal record. Corrected canonical fixtures establish successful real-child setup before deliberate failures. Counterfeit image/record fixtures always earn rejection-only credit.
- Intermediate strict formatting/API diagnostics were corrected without disabling checks. The history-retirement formatting failure is retained in `.phase3/reviewfix-history-retirement-build-failure.log`; the final strict build is green.
- Earlier passing 20-case reports `93ff2fcb-763f-41fd-bf8d-b5eb8fdf4a23` and `fc44396f-3456-4fe6-bca9-44a79e49ffbc` remain history. Later current capture/terminal/consumer changes are covered by the final `57ee4949-87df-4551-8bdc-37a6686a2d89` run.

## New files and handoff

The original sixteen tracked helpers/regression files are preserved: `FoundationActorNode.cs`, `FoundationAdmissionHistory.cs`, `FoundationCapturePublication.cs`, `FoundationNativeRegression.cs`, `FoundationRegressionDiscovery.cs`, `FoundationCapturePublicationTests.cs`, `FoundationLinkedSources.cs`, `tools/phase2/menu-navigation.cjs`, and Phase 3 `foreground.cjs`, `foreground.test.cjs`, `navigation.test.cjs`, `selection-impact.cjs`, `stopped.cjs`, `stopped-fixture.cjs`, `terminal.cjs`, `terminal-fixture.cjs`. Iteration 2 created three additional tracked control/implementation files, `tools/phase3/revisions.cjs`, `revisions.test.cjs` and `inherited.test.cjs`, as explicitly required by its findings. Iteration 3 creates no tracked file and commits only the six CR-09 paths above. No source fix remains uncommitted; this cumulative report is intentionally left uncommitted for the orchestrator.

The full retained review union is `.phase3/review-fix-union.txt`: **115 unique paths**, preserving the original 111 plus those three new files and the updated `03-04-CHECKPOINT.md` workflow artifact. Independent re-review must inspect that complete union and current phase context before Task 3. The source-build stamp rule is part of that review. Keep `SourceRevisionId` pinned to the final reviewed source revision when authoring/committing the gate and running later strict/inherited/full/closure commands; see the updated checkpoint for the exact sequence.

No findings were skipped. Independent re-review must now inspect the repaired original scope plus new fix files. The review gate `03-REVIEW.json` and `03-04-SUMMARY.md` were not created. Shared STATE/ROADMAP/REQUIREMENTS, publishing and PR transitions remain with the parent; no merge occurred. Owned Phase 1/desktop locks were released.

Actual native completed-draw/physical-input/window-message feasibility, visual review and the fresh entire saved selection of 32 Phase 3 + 16 Phase 2 + 433 Phase 1 cases remain Task 3 work after independent re-review. There is no rendered, physical LAN, hosted/Linux, supported-display, campaign, packaging, hardware, endurance or Phase 5 certification credit in this repair report.

---

_Fixed: 2026-11-15T19:28:15.447Z_  
_Fixer: the agent (gsd-code-fixer)_  
_Iteration: 3_

## Independent verification and final disposition

The independent final code recheck is complete and clean at source/build revision `166a32ad6a7541c4f5eed1378def0a86e9bea632`. All fourteen findings are resolved. This subsequent verification closes the earlier fixer-only independent-check flags; desktop input, completed-render timing, actual image review and the fresh entire selected-set pass remain Plan 03-04 Task 3. Historical Phase 1 human-review follow-ups retain their original status.

| Finding | Severity | Final disposition | Independent verification |
| --- | --- | --- | --- |
| CR-01 | critical | fixed | complete |
| CR-02 | critical | fixed | complete |
| CR-03 | critical | fixed | complete |
| CR-04 | critical | fixed | complete |
| CR-05 | critical | fixed | complete |
| CR-06 | critical | fixed | complete |
| CR-07 | critical | fixed | complete |
| CR-08 | critical | fixed | complete |
| CR-09 | critical | fixed | complete |
| WR-01 | medium | fixed | complete |
| WR-02 | medium | fixed | complete |
| WR-03 | medium | fixed | complete |
| WR-04 | medium | fixed | complete |
| WR-05 | medium | fixed | complete |

The advisory per-finding ledger is mirrored in `.phase3/review-history/03-REVIEW-DISPOSITION.json` with the exact review/fix hashes. Review-loop scratch copies are archived there before removal from the phase folder; original review commit `624f37b` and the cumulative commit/evidence ledger above preserve the history. The phase-owned immutable-metadata contract keeps this complete tracked disposition inside this report.

## Final closure integration recheck

Source `e25a500d12006e256d84b8b241d0cee9c507f278` adds only five exact phase-owned final verification/report paths to the closure contract and writes them in the existing real Git/SDK fixture. The three focused controls pass, including the real five-build SDK cycle; strict build has zero warnings/errors (`.phase3/closure-integration-build.log`). Independent reviewer `/root/phase3_re_review_3` rechecked this integration and retained clean complete-scope review with all fourteen findings resolved. Prior 166 proof remains historical; the fresh full selection uses this new source/build pin.

## Actual GitHub SourceLink rebuild correction

The mandatory e25 metadata-descendant rebuild passed strict compilation but failed exact tool identity before any full run or window: SDK Git discovery independently stamped `SourceRoot.RevisionId` with metadata HEAD. Original mismatch/diagnosis remain `.phase3/task3-prelaunch-mismatch.json` and `.phase3/task3-build-drift-diagnosis.json`. RED `bc6b252` reproduces that real GitHub SourceLink failure; GREEN `cff08eb` supplies a phase-only target that pins only this repository source root while preserving real SourceLink. Three controls pass with five real SDK builds and an import-omitted/assembly-pinned negative that reproduces exact DLL drift. Actual `runner.integrity` passes (`77447059-c17a-4601-a2f2-ab952808dd22`); strict build passes zero warnings/errors. Independent complete-scope review is clean at source/build `cff08ebe7cc78c562fe370dcdc3718e116938905`, 116 paths / 113 source paths, with all fourteen findings resolved. All future builds must preserve both the source stamp and target import.

## First rendered-run batch and CR-10 closure

The first complete selected execution remains immutable and failed (`28bb1d55-5831-4510-b590-9f140e1689b2`, 464 pass / 17 fail). Repair `dc523e9` corrects two isolated compiler fixtures, one stale R-37 matrix cell and the physical desktop helper without relaxing generated permissions, resolution, ownership or focus requirements. All five inherited failed cases pass their focused reruns; driver boot/DPI APIs pass without input credit. RED `8466836` then reproduces CR-10 using genuine nested Phase 1 report shape; GREEN `7b140a2` verifies exact per-case report/hash/containment before resolving subject artifacts. Three closure/GitHub SDK controls and actual `runner.integrity` pass (`5e25ec4e-d0d2-4f2c-bed0-4b73588c0b45`); strict build has zero warnings/errors (`.phase3/cr10-strict.log`). Shared production-reader compatibility checks cover 428 retained passing and five focused repaired records with 1732 negatives; original reports and failed aggregate remain unchanged (`.phase3/cr10-real-reports.json`). Independent review is clean at source/build `7b140a27e4f5556db12caed2b1614c52a1aad18a`, 118 paths / 115 source paths, all fifteen findings resolved. Native focused retry and fresh whole selected-set acceptance remain required.

## Native repair batch 2

Focused `72f2d327-0ff1-40bf-83fd-72fbf90a4ab9` remains failed without acceptance substitution. Native-only sources `af8d4f2` and `d2d5a6` repair paced Unicode delivery/UTF8, actual empty-field clearing, owned-window focus reachability and pre-initialized focus observation; every paced character retains a foreground guard. Raw timing rows are now retained before the unchanged inclusive 250 ms check can reject them. Existing foreground/navigation controls pass 3/3 and af8 helper boot passes; these are not native input acceptance. Strict build at `d2d5a6106990aaf16b0b02a5f296fc7b11aa0f3e` passes zero warnings/errors (`.phase3/batch2-guard-strict.log`). Independent focused recheck is clean, 118/115 scope and fifteen resolved findings. Focused native retry and fresh entire selected pass remain required.

## Native repair batch 3

Focused `268a2e82-bb76-4138-8e79-6088cee21315` remains failed (0/12 accepted). Long text/empty clearing now works without Godot error logs; overlapping owned windows and expensive sequential PNG decoding still blocked acceptance. Raw start timing bounds remain 283.3227/435.9760/589.3014 ms. Read-only diagnostic runs `d30c16a4-3690-44d7-a8f0-18c15f54a1fb` and `2a2b232b-118e-4586-8aec-129bfc6ccaab` preserve owned geometry, original failures and cleanup without acceptance credit. `2902f7a` replaces collision-free RGB string keys with equivalent packed byte triples and skips irrelevant Paeth arithmetic, preserving all decoding/validation and the original timing bracket/250 ms limit. The three original hash-bound PNGs retain exactly the same decoded results while validation improves from 145–163 ms to 23–38 ms (`.phase3/png-before.json`, `png-after.json`). The helper temporarily raises only the owned target and restores its prior non-topmost status in finally; `4ea54ca` places the physical caption click clear of system-menu controls. The exact point/foreground/resolution fences remain. Phase 2 runner passes 3/3 (`a1fb0045-e63a-4deb-8f2c-876515615be2`), Phase 3 integrity passes at 2902 (`f8c44633-9198-49a5-8c81-3a2e0b3a4d86`), and strict build at final source 4ea54ca731c90de700480033f58d17fc5ae8e00b passes zero warnings/errors. Independent review is clean, 118 paths / 115 source paths, all fifteen findings resolved. Next retry twelve native cases from `268a2e82...`; only after focused acceptance start the fresh full 481 selected set and actual image review.
