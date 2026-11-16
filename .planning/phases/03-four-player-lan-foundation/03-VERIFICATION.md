---
phase: 03-four-player-lan-foundation
verified: 2026-11-16T05:53:14Z
status: passed
score: 21/21 must-haves verified
covered_files: [
    ".gitignore",
    ".phase3/final-regression-audit.json",
    ".phase3/runs/win32-x64/841e10b1-f825-4527-865b-4703382f51eb/summary.json",
    ".phase3/task3-fifth-full-evidence-audit.json",
    ".planning/phases/03-four-player-lan-foundation/03-01-PLAN.md",
    ".planning/phases/03-four-player-lan-foundation/03-01-SUMMARY.md",
    ".planning/phases/03-four-player-lan-foundation/03-02-PLAN.md",
    ".planning/phases/03-four-player-lan-foundation/03-02-SUMMARY.md",
    ".planning/phases/03-four-player-lan-foundation/03-03-PLAN.md",
    ".planning/phases/03-four-player-lan-foundation/03-03-SUMMARY.md",
    ".planning/phases/03-four-player-lan-foundation/03-04-PLAN.md",
    ".planning/phases/03-four-player-lan-foundation/03-04-SUMMARY.md",
    ".planning/phases/03-four-player-lan-foundation/03-CONTEXT.md",
    ".planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json",
    ".planning/phases/03-four-player-lan-foundation/03-RESEARCH.md",
    ".planning/phases/03-four-player-lan-foundation/03-REVIEW-DISPOSITION.md",
    ".planning/phases/03-four-player-lan-foundation/03-REVIEW.json",
    ".planning/phases/03-four-player-lan-foundation/03-REVIEW.md",
    ".planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md",
    ".planning/phases/03-four-player-lan-foundation/03-SECURITY.md",
    ".planning/phases/03-four-player-lan-foundation/03-SELECTION.json",
    ".planning/phases/03-four-player-lan-foundation/03-SPEC.md",
    ".planning/phases/03-four-player-lan-foundation/03-UI-REVIEW.md",
    ".planning/phases/03-four-player-lan-foundation/03-UI-SPEC.md",
    ".planning/phases/03-four-player-lan-foundation/03-VALIDATION.md",
    ".planning/phases/03-four-player-lan-foundation/03-VISUAL-REVIEW.md",
    "AGENTS.md",
    "docs/code/engine-generated-spans.json",
    "docs/code/exemptions.json",
    "docs/code/generated-callsite-adaptations.json",
    "docs/code/register.json",
    "docs/code/rule-matrix.md",
    "docs/code/visibility-exemptions.json",
    "docs/workflow/ai-collaboration.md",
    "docs/workflow/lan-foundation-checks.md",
    "docs/workflow/local-testing.md",
    "docs/workflow/verification.md",
    "src/core/FoundationHeldIntent.cs",
    "src/core/FoundationMovementCommand.cs",
    "src/core/FoundationParticipantState.cs",
    "src/core/FoundationSession.cs",
    "src/core/FoundationSnapshot.cs",
    "src/godot-game/foundation/DiscoveryCodec.cs",
    "src/godot-game/foundation/EnetConnectionService.cs",
    "src/godot-game/foundation/Foundation.tscn",
    "src/godot-game/foundation/FoundationActorNode.cs",
    "src/godot-game/foundation/FoundationAdmissionHistory.cs",
    "src/godot-game/foundation/FoundationAutomation.cs",
    "src/godot-game/foundation/FoundationBrowser.cs",
    "src/godot-game/foundation/FoundationCapturePublication.cs",
    "src/godot-game/foundation/FoundationCoordinator.Admission.cs",
    "src/godot-game/foundation/FoundationCoordinator.GuestPublication.cs",
    "src/godot-game/foundation/FoundationCoordinator.Lifetime.cs",
    "src/godot-game/foundation/FoundationCoordinator.cs",
    "src/godot-game/foundation/FoundationCopy.cs",
    "src/godot-game/foundation/FoundationDiagnostics.cs",
    "src/godot-game/foundation/FoundationDisplay.cs",
    "src/godot-game/foundation/FoundationDrawObserver.cs",
    "src/godot-game/foundation/FoundationDrawRequest.cs",
    "src/godot-game/foundation/FoundationEndpoint.cs",
    "src/godot-game/foundation/FoundationEnvelope.cs",
    "src/godot-game/foundation/FoundationEphemeralProfile.cs",
    "src/godot-game/foundation/FoundationHud.cs",
    "src/godot-game/foundation/FoundationInput.cs",
    "src/godot-game/foundation/FoundationMenu.cs",
    "src/godot-game/foundation/FoundationNativeRegression.cs",
    "src/godot-game/foundation/FoundationObservationScope.cs",
    "src/godot-game/foundation/FoundationPeerBinding.cs",
    "src/godot-game/foundation/FoundationPresentation.cs",
    "src/godot-game/foundation/FoundationProtocol.cs",
    "src/godot-game/foundation/FoundationRegressionDiscovery.cs",
    "src/godot-game/foundation/FoundationRoot.cs",
    "src/godot-game/foundation/FoundationRuntimeState.cs",
    "src/godot-game/foundation/FoundationServices.cs",
    "src/godot-game/foundation/FoundationView.cs",
    "src/godot-game/foundation/FoundationWireFrame.cs",
    "src/godot-game/foundation/FoundationWorld.tscn",
    "src/godot-game/foundation/LanDiscoveryService.cs",
    "src/godot-game/foundation/LocalProfileService.cs",
    "src/godot-game/foundation/LocalProfileStorage.cs",
    "src/multiplayer-abstractions/ConnectionPacket.cs",
    "src/multiplayer-abstractions/IConnectionService.cs",
    "src/multiplayer-abstractions/IDiscoveryService.cs",
    "src/multiplayer-abstractions/IIdentityService.cs",
    "src/multiplayer-abstractions/PlayerProfile.cs",
    "src/multiplayer-abstractions/SessionAdvertisement.cs",
    "tests/phase1/ArchitecturePolicy.cs",
    "tests/phase1/BuildFixture.cs",
    "tests/phase1/EngineAccessPolicy.cs",
    "tests/phase1/EngineGeneratedTests.cs",
    "tests/phase1/ExemptionTests.cs",
    "tests/phase1/ExemptionValidator.cs",
    "tests/phase1/FoundationCapturePublicationTests.cs",
    "tests/phase1/FoundationConnectionReplacement.cs",
    "tests/phase1/FoundationDiscoveryReplacement.cs",
    "tests/phase1/FoundationHarnessConnection.cs",
    "tests/phase1/FoundationIdentityReplacement.cs",
    "tests/phase1/FoundationLanAdmissionTests.cs",
    "tests/phase1/FoundationLanAuthorityTests.cs",
    "tests/phase1/FoundationLanInputTests.cs",
    "tests/phase1/FoundationLanLifecycleTests.cs",
    "tests/phase1/FoundationLanOrderingTests.cs",
    "tests/phase1/FoundationLanPermissionTests.cs",
    "tests/phase1/FoundationLanProtocolTests.cs",
    "tests/phase1/FoundationLanRunnerTests.cs",
    "tests/phase1/FoundationLanServicesTests.cs",
    "tests/phase1/FoundationLinkedSources.cs",
    "tests/phase1/FoundationPermissionTests.cs",
    "tests/phase1/FoundationScenarioClock.cs",
    "tests/phase1/FoundationServiceSubstitutionProof.cs",
    "tests/phase1/FoundationTestConnection.cs",
    "tests/phase1/FoundationWireFrameTests.cs",
    "tests/phase1/GraphFixture.cs",
    "tests/phase1/KingdomZombies.Phase1.Tests.csproj",
    "tests/phase1/SuppressionTests.cs",
    "tools/code-rules/BodyLengthAnalyzer.cs",
    "tools/code-rules/EngineGeneratedSyntax.cs",
    "tools/phase2/check.cjs",
    "tools/phase2/menu-navigation.cjs",
    "tools/phase2/rendered.cjs",
    "tools/phase3/catalog.cjs",
    "tools/phase3/check.cjs",
    "tools/phase3/evidence.cjs",
    "tools/phase3/foreground.cjs",
    "tools/phase3/foreground.test.cjs",
    "tools/phase3/inherited.cjs",
    "tools/phase3/inherited.test.cjs",
    "tools/phase3/navigation.test.cjs",
    "tools/phase3/network.cjs",
    "tools/phase3/png-worker.cjs",
    "tools/phase3/png.cjs",
    "tools/phase3/processes.cjs",
    "tools/phase3/rendered-stream.test.cjs",
    "tools/phase3/rendered.cjs",
    "tools/phase3/required-cases.json",
    "tools/phase3/reviewed-source.targets",
    "tools/phase3/revisions.cjs",
    "tools/phase3/revisions.test.cjs",
    "tools/phase3/runner-fixture.cjs",
    "tools/phase3/scenarios.cjs",
    "tools/phase3/selection-impact.cjs",
    "tools/phase3/selection.test.cjs",
    "tools/phase3/stopped-fixture.cjs",
    "tools/phase3/stopped.cjs",
    "tools/phase3/terminal-fixture.cjs",
    "tools/phase3/terminal.cjs",
    "tools/phase3/timing.test.cjs",
    "tools/phase3/windows-input.ps1"
  ]
covered_digest: "v4:sha256:523151d03780f35bf5475678ff9ac8a93cc3550b314de1ab46869f644697ae8b"
behavior_unverified: 0
overrides_applied: 0
human_verification: []
decision_coverage:
  honored: 4
  total: 4
  not_honored: []
requirements_verified: [LAN-01, LAN-02, LAN-03, LAN-04]
---

# Phase 3: Four-player LAN foundation Verification Report

**Phase Goal:** As a LAN player, I want to host or join a foundation scene for one host and three guests and independently control my character, so that all four players observe consistent authoritative movement.
**Verified:** 2026-11-16T05:53:14Z
**Status:** passed
**Re-verification:** No — initial verification; no previous Phase 3 VERIFICATION.md existed.
**Scope:** Windows development OS, 1280×720, four local game processes, no VM. This is the current phase-local contract under `docs/workflow/local-testing.md`.

The goal is achieved in production source and the source-bound execution evidence. This verdict includes eight real, nonblocking UI recommendations from the independent UI audit; it does not certify exact conformity with every styling/detail choice in UI-SPEC. No code, tests, shared phase status or PR was changed by this verification.

## User Flow Coverage

User story: «As a LAN player, I want to host or join a foundation scene for one host and three guests and independently control my character, so that all four players observe consistent authoritative movement.»

The literal roadmap goal was read through `roadmap.get-phase 3` and accepted by `user-story.validate --story <literal goal> --pick valid` (true). The earlier non-story wording was corrected in metadata commit `25ef9c8` without changing scope.

| Step | Expected | Evidence in code and actual execution | Status |
|---|---|---|---|
| Launch and select a local profile | Visible menu; persisted opaque identity independent of display name | `FoundationRoot._Ready` composes `LocalProfileService`; `LocalProfileService.Execute` publishes identity only after atomic storage. Actual `adapters.profiles`, `network.discovery-profile`, `rendered.profiles` and retained restart with distinct PIDs. | VERIFIED |
| Choose Host session | Enter the existing scene immediately as P1 | `FoundationRoot.Enter` → `FoundationServices.Create` → `FoundationCoordinator.Start`; actual `rendered.entry` and four-peer native runs. | VERIFIED |
| Find or manually join the host | Three distinct guests join; errors/cancel remain recoverable | Real `LanDiscoveryService` and raw `EnetConnectionService`; `FoundationMenu` routes selected session and manual address to `JoinAddress`. Actual browser/manual, admission, moving-join and error cases. | VERIFIED |
| Walk using ordinary movement controls | Only own actor moves; own camera follows; other peers see current movement | `_Input`/physical polling → `FoundationInput` → coordinator intent → sole Core acceptance/step → snapshots → `FoundationView.ProjectRoster`. Actual four-peer walking and physical timing evidence. | VERIFIED |
| Observe all four players consistently | Remote starts/reversals/stops/focus-stops within 250 ms; stopped position agreement within 50 subunits by 500 ms | 48 exact origin/observer/action rows, all physical SendInput, maximum conservative upper 197.8586 ms. All 16 stopped rows have zero subunit disagreement, maximum observation upper 171.4658 ms. Actual draw/PNG/authority identities reconciled. | VERIFIED |
| Leave, replace a guest, end/rehost or lose host | Exact placeholder/slot cleanup; useful menu and fresh lifetime | `FoundationCoordinator.Lifetime.cs`, `FoundationRuntimeState` and root menu cleanup; actual guest-lifecycle, host-end, host-loss and deterministic boundary/old-work tests. | VERIFIED |
| Outcome | All four players observe consistent authoritative movement | Common host-owned authority, four distinct native peers, bound current snapshots/draws and actual appearance review all support the outcome. | VERIFIED |

## Evidence Basis and Method

The independent audit read the actual authority, admission, publication, lifetime, services, native root/input/view, protocol, profile storage and evidence-validator paths. SUMMARY claims were used to locate evidence, never as proof.

- Reviewed production/build revision: `c69442148c975de974ebbb1d8ef409380940ecb0`.
- Original executed metadata revision: `6ea600953be031bf2b467a50d35090a94e44f802`.
- Current validated metadata revision: `25ef9c8b693a015fbc9f7a9e2434d7926c115d44`.
- Final immutable report **R**: `.phase3/runs/win32-x64/841e10b1-f825-4527-865b-4703382f51eb/summary.json`.
- R SHA-256: `b7bdf55d74034a5c50527f0e8391e7a7be01d1e46867dd103b9d381e6b2e6bae`.
- Actual selected execution: **481/481** (433 Phase 1, all 16 Phase 2, all 32 Phase 3), 3,363,558 ms / 56.06 minutes. Parsed result sets and inherited command outcomes agree.
- Current input fingerprint: `9e98ad7b705fbc417b371da7a48ad3c3713f635fa9672c4de2afe6f7221c5d32`; tool fingerprint: `9e911416e13c7c81e38f0f9883062d1310cfaa085e9b2e571c8b64336b5621b9`.
- This verifier reran the **read-only completion validator**, which reconciled the original execution, current source/tool/OS identities, metadata ancestry, exact cases/roles/images, immutable reports and inherited proofs. It returned PASS with `execution: original`. No build, full test rerun or game process was launched here.
- Initial restricted validator attempts stopped at `DOTNET_MISSING` before evaluating evidence because nested SDK probing was restricted. The identical scoped elevated read-only command with isolated SDK environment succeeded. Those setup failures receive no behavioral credit.
- Actual named CTRF files were independently parsed: all thirteen C# case reports contain exactly one passing named test, zero failed/skipped/pending tests. Their subject/source assertions were inspected.
- Independent image/hash check matched all **319** current review bindings and **255** distinct contents plus index/selection/visual/audit references. The retained comprehensive review records 221 actual current inspections and 34 exact-byte prior inspections with explicit current bindings. This verifier additionally opened the current idle browser, four-player guest scene and host-loss menu; it does not claim to have newly opened all images.
- A source diff from the reviewed build through current HEAD has no changes in `src`, `tests`, `tools` or `docs/code`. Source/build, original execution HEAD and current metadata HEAD remain distinct.

## Goal Achievement

### Observable Truths

The contract is the four roadmap success criteria **R1–R4**, plus seventeen plan-specific truths **P1.1–P4.5**. Related plan detail is retained where it adds a readiness, lifetime, permission, presentation or evidence obligation. Every runtime truth has an exercised named test or actual native scenario in R; none is credited from presence alone.

| ID | Truth | Status | Evidence |
|---|---|---|---|
| R1 | A host and up to three guest game processes connect to the same scene with distinct player profiles and distinct controlled characters. | VERIFIED | Raw ENet production composition and pending-inclusive roster; R `network.four-peer`, `rendered.admission`, `rendered.walkers` show four distinct PIDs/profiles/actors/bindings and one session. |
| R2 | Each participant's normal movement input affects only their own character, while every participant observes consistent resulting positions through their own camera. | VERIFIED | Root input → coordinator → `FoundationSession.TryAccept`/Step; actor-keyed projection follows one local binding. R `core.input`, `rendered.walkers`, `rendered.timing`, `rendered.focus`; 48 physical timing rows and 16 stopped comparisons. |
| R3 | Solo, host-local, and guest actions produce one authoritative result per accepted action; repeated input/command delivery cannot execute the host action twice. | VERIFIED | `FoundationSession.cs:74,93`, `FoundationCoordinator.cs:116,206`; named `OwnedIntentIsAcceptedBeforeAnyElapsedStep` and `RejectedCommandsLeaveIntentAndHighWaterUnchanged` pass with actual before/after positions and accepted counts. |
| R4 | A developer can repeat the connection/movement smoke checks and substitute the discovery, identity, or connection adapter without changing gameplay rules; player identity survives independently of a LAN address or transient peer number. | VERIFIED | Executable saved catalog/commands; independent framework-only ports; actual provider substitutions and profile restart tests. `IndependentProvidersUseProductionComposition` passes; Core before/after hashes unchanged; real LAN processes separately exercised. |
| P1.1 | Solo and a host plus one guest enter the same production fixture and use the same identified intent acceptance and host stepping path. | VERIFIED | `FoundationRoot.Enter/JoinAddress`, `FoundationServices.Create`, common coordinator Submit/Accept/Update; R `core.authority`, `network.tracer` and all inherited solo cases. |
| P1.2 | A joining guest installs coherent current state before controlling its own actor; host input has no bypass. | VERIFIED | `GuestPublication.InstallBaseline:80` installs then acknowledges; only ready publication exposes LocalActor. Host-local Submit calls the same Accept method. R authority/admission/ordering/protocol and moving-join cases. |
| P1.3 | The tracer crosses real raw ENet transport in separate headless game processes without a real game window. | VERIFIED | `EnetConnectionService` implements actual CreateServer/CreateClient/Poll/GetPacket/PutPacket. R `network.tracer` has real separate peer launch/frame/log records and explicitly no rendered/physical credit. |
| P1.4 | New public/native permissions are exact and adjacent unauthorized access still fails. | VERIFIED | Actual `FoundationLanPermissionTests.ExactLanPermissionsRejectAdjacentMisuse` pass; exact inventory 45 native/eight public additions, real compiler-valid positives and scanner-negative neighbors. Inherited source/generated/callsite proofs pass; 220 authenticated generated spans retained. |
| P2.1 | Host plus three guests join a moving scene with one coherent ready actor each and never more than four admitted/pending participants. | VERIFIED | `FoundationSession.AddParticipant` cap and immutable uniqueness; serialized coordinator Reserve. Named `PendingAdmissionIsIdempotentAndBounded`, actual moving join, fifth refusal and final-slot race pass. |
| P2.2 | Unready, foreign, duplicate, reordered and obsolete work cannot alter current movement, readiness or ownership. | VERIFIED | Core validation precedes intent/high-water mutation; actual sender/provider/peer generation bound in Receive; guest rejects stale session/revision/actor time. Named ordering/protocol tests and native admission/lifecycle checks pass. |
| P2.3 | Cancel, guest loss, host end and replacement release only the owning lifetime and allow a fresh session. | VERIFIED | `Lifetime.BeginClose:93` closes acceptance before teardown; RemovePeer uses exact object/binding/generation. Named `LifetimeCleanupAndDeadlineBoundaries`, actual guest-lifecycle/host-end/host-loss pass; old work cannot alter replacement. |
| P2.4 | Join and response-loss thresholds are evaluated at the first update at or after 30000 and 10000 milliseconds respectively. | VERIFIED | `Lifetime.cs:17,18,50,57` uses >=; Receive precedes Expire. Named lifecycle test executes below/equal/above triplets and response-at-boundary ordering. Native timeout/loss cases separately pass. |
| P3.1 | Players discover or manually join a host through usable native controls, including empty/error/full/vanished and cancellation states. | VERIFIED | Real multicast + sender-derived endpoints, identity-keyed browser, ordinary selected/manual Join callbacks and recoverable menus. R discovery/profile/browser/join-errors/admission cases and actual images; minor UI recommendations below do not prevent these flows. |
| P3.2 | Selected profiles retain opaque identities across restarts while identical display names remain separate. | VERIFIED | `LocalProfileService.Execute:51` and atomic `LocalProfileStorage` retain exact IDs; labels are independent data. Named profile/substitution tests plus real restarted process using same owned profile root and distinct PID pass. |
| P3.3 | Each ready participant sees distinct P1–P4 actors, a You marker and only its own following camera, with inherited input/focus behavior. | VERIFIED | `FoundationView.ProjectRoster:14` projects exact local actor once, keys/removes remote visuals by actor, assigns markers; root clears holds at focus/lifetime transitions. R input/walkers/focus and actual 4-player imagery support distinct labels/local You and camera behavior. |
| P3.4 | Discovery, identity and connection can each be substituted independently through actual composition without changing Core rules. | VERIFIED | `FoundationServices.Create:22/ReplaceDiscovery:36` and injected connection factory; actual `FoundationServiceSubstitutionProof` performs each swap through admission/readiness/movement, checks unavailable/obsolete providers and byte-identical Core hashes. |
| P4.1 | The complete phase is implemented and reviewed, with findings fixed, before any real game window opens. | VERIFIED | Prelaunch code requires complete source/tool/revision-bound independent review and resolved high findings. R binds immutable prelaunch review copies and reviewed source before its launches. Recorded whole-phase review resolves 18 findings; subsequent repaired builds pass the review gate before native runs. Historical runtime defects/failed attempts remain visible. |
| P4.2 | Four actual rendered peers use distinct profiles and report actor/session/revision, independent cameras and honestly identified input provenance. | VERIFIED | R role/PID/profile/actor/binding identities, actual draw streams and PNG state reconcile. Timing PIDs 34924/13612/26056/28316; physical, window-message and synthetic modes remain distinct. Simultaneous targeted-message walking earns no physical credit. |
| P4.3 | Start, reversal, stop and focus-stop rendered response have conservative upper bounds at most 250 ms; every stopped peer converges within 0.05 u by 0.5 s. | VERIFIED | Independent recomputation from actual integer clock brackets: 48 physical rows, max 197.8586 ms; 16 stopped rows, max error 0 subunits and max upper 171.4658 ms. `evidence.cjs` and stopped reconciler bind native acknowledgments, host accepted intents, completed draws and images; boundary/overrun controls pass. |
| P4.4 | Every selected current case/peer/image is required; stale, missing, duplicate, crash and timeout controls cannot produce a pass. | VERIFIED | Independent catalog exact-set reconciliation, actual C# runner controls plus real child failure/process controls. R 481/481 and current read-only completion PASS; negative fixtures prove rejection, not production success. |
| P4.5 | Reusable scenarios, selection/omissions and ordered run groups survive in the repository; historical failures remain inspectable. | VERIFIED | Saved 32 case scenarios, 18 AC/20 edge mappings, G0–G4 and exact 481 selection; 243 active omissions plus 8 planned declarations have no pass credit. Index retains 18 historical primary reports, including fourth-full behavior PASS/visual FAIL; final fresh execution follows repairs. |

**Score: 21/21 truths verified; 0 present-but-behavior-unverified; 0 overrides; 0 failed/uncertain truths.**

No backstop truths or additional must-NOT prohibitions are declared in the plans/specification. The reliance check found no specific undeclared production precondition, incidental ordering or fixture-only condition supporting the verified phase truths: production establishes identities/readiness, sequencing and generation fences; native scenarios independently cover the real provider/input paths.

### Required Artifacts

All fifteen declared artifacts passed the authoritative `verify.artifacts` existence/substance query (4 + 3 + 4 + 4). Source inspection and actual invocation evidence establish the additional wiring levels below.

| Artifact | Expected | Status | Substantive implementation and wiring |
|---|---|---|---|
| `src/core/FoundationSession.cs` | Identified roster and sole intent/step authority | VERIFIED | Validates ready owner/session/binding/sequence, retains intent and steps once; production coordinator calls it. |
| `src/godot-game/foundation/FoundationCoordinator.cs` | Sender binding, baseline and role composition | VERIFIED | Production root/services compose it; admission/publication/lifetime partials enforce real protocol and ordering. |
| `src/godot-game/foundation/EnetConnectionService.cs` | Guarded raw transport | VERIFIED | Real native calls, sender before packet consumption, generation tracking, bounded polling/framing and owned close; root factory instantiates it. |
| `tools/phase3/check.cjs` | Independent no-window and full entry point | VERIFIED | Exact selectors dispatch real named tests/native subjects; --all and completion path executed. |
| `tests/phase1/FoundationLanLifecycleTests.cs` | Admission/loss/generation boundaries | VERIFIED | Active named test asserts closure, exact deadlines, canceled/old/replacement lifetimes; explicit project compilation and catalog binding. |
| `tests/phase1/FoundationLanProtocolTests.cs` | Detached malformed/obsolete-state controls | VERIFIED | Active named test maps actual bounded protocol and immutable state; exact pass in R. |
| `tools/phase3/network.cjs` | Actual four-peer no-window integration | VERIFIED | Launches ordinary headless main scene with isolated profiles/ports and owned teardown; R includes real peer frames/logs. |
| `src/godot-game/foundation/LanDiscoveryService.cs` | Multicast discovery, expiry and selection | VERIFIED | Real multicast socket and bounded sender-derived advertisements; composed by root, consumed by browser; socket and native discovery cases pass. |
| `src/godot-game/foundation/LocalProfileService.cs` | Persisted exact profile identity | VERIFIED | Exclusive lock, validated full book and atomic publication before selected identity; production services consume selected profile. |
| `src/godot-game/foundation/Foundation.tscn` | Native menu/profile/browser feedback | VERIFIED | Production main scene with wired buttons/fields; actual usable flow and current images. Eight minor contract recommendations are retained below. |
| `tests/phase1/FoundationLanServicesTests.cs` | Actual substitution and provider failures | VERIFIED | Three active named tests exercise socket/profile/composition paths with value/state assertions; production linked sources compiled explicitly. |
| `tools/phase3/evidence.cjs` | Exact per-case/per-peer immutable reconciliation | VERIFIED | Bound hashes/nonces/provenance/PNG/terminal/clock validation, inherited proof checks; called by executed checker and completion validator. |
| `tools/phase3/rendered.cjs` | Owned native scenarios and timed draws | VERIFIED | Used by scenario runner, real native helper/draw observations and timing records in R; no state-only rendering credit. |
| `03-SCENARIOS.md` | Reusable 18-AC/20-edge cases and ordered groups | VERIFIED | All 32 stable cases have metadata, setup/actions/expectations/evidence/reset; catalog validates coverage and selects matching groups. |
| `03-EVIDENCE.json` | Final exact results and actual appearance bindings | VERIFIED | Referenced immutable final/history/selection/visual artifacts and 319 current review rows; all relevant hashes and completion validation pass. |

### Key Link Verification

The CLI returned a negative heuristic result for all twelve declarations because each PLAN writes a descriptive component phrase in `from`, where that checker requires a root-relative filename. This is a **plan metadata/checker compatibility warning**, not evidence of twelve missing production connections. Every declared connection was therefore resolved manually to real source and runtime evidence.

| Plan/link | Actual from → to | Wiring and observed proof | Status |
|---|---|---|---|
| 01/1 | Root native menu/input → coordinator Submit | _Ready signals, _Input/polling, _Process Submit; solo/host/guest authority and native tracer. | WIRED |
| 01/2 | ENet sender/provider generation → session acceptance | Poll yields actual sender/generation; Receive checks both generations; ReceiveHost resolves trusted binding; Accept calls TryAccept. | WIRED |
| 01/3 | Installed baseline → local projection | InstallBaseline sets detached state then ack; ready publication gates LocalActor/ShowWorld; ProjectRoster selects exact local binding. | WIRED |
| 02/1 | Actual sender/current binding → held intent | Ready/session/actor/sequence validation occurs before intent/high-water mutation; ordering tests inspect rejection invariance. | WIRED |
| 02/2 | Baseline acknowledgment → host promotion | Current peer/attempt/session/revision checks precede MarkReady and latest complete BroadcastState; moving join does not freeze existing actors. | WIRED |
| 02/3 | Closed generation → reservation/actor cleanup | BeginClose fences acceptance; exact peer removal and provider generation prevent old cleanup reaching replacement; deterministic/native lifecycle proof. | WIRED |
| 03/1 | Discovery session identity → join attempt | Browser retains selected session ID; menu passes RequestedSessionId; coordinator revalidates hello and offered baseline against current host. | WIRED |
| 03/2 | Persisted selected profile → coordinator ownership | Root uses LocalProfileService; services Create maps Selected.PlayerId; exact identity survives restart/address/peer changes. | WIRED |
| 03/3 | Current local actor ID → camera | ProjectRoster resolves local ready binding and calls Project once before remote iteration; input/projection tests and per-peer draw/camera records. | WIRED |
| 04/1 | Native input acknowledgment → first changed draw | Owned helper, monotonic external brackets, read-only FramePostDraw, PNG/state and host accepted-intent references are reconciled. | WIRED |
| 04/2 | Independent catalog/selection → peer results/images/reviews | Exact set equality for case/role/checkpoint, immutable hashes and explicit appearance verdicts; current completion PASS. | WIRED |
| 04/3 | Review/fixes → grouped native execution | Source/tool/revision-bound prelaunch gate and immutable review copies precede real process launches; original review remains bound through metadata closure. | WIRED |

### Data-Flow Trace (Level 4)

This game uses authoritative in-memory state and persisted profile files rather than a database. Authored fixture geometry/palette is intentional static content; live participant values trace to actual state.

| Rendered artifact/value | Real source | Flow to output | Status |
|---|---|---|---|
| Browser rows/count/availability | Real bounded multicast advertisements with received sender endpoint | Discovery.Update → FoundationBrowser exact session rows → FoundationMenu native rows; live host revalidates admission | FLOWING |
| Profile selector/name | Validated schema-1 profile book at owned user-data root | LocalProfileStorage.Read → selected opaque profile → native selector/name and coordinator identity | FLOWING |
| P1–P4 actor positions/facing | Core host Step, accepted identified intents | Immutable snapshot → wire decode/monotonic installation → ProjectRoster actor-keyed transforms | FLOWING |
| Local You and camera | Current ready binding/player/actor | FoundationPresentation.Local → Project(local actor) once → camera projection and local marker | FLOWING |
| Roster/count/host-loss feedback | Current accepted snapshot/coordinator role/lifetime/error | FoundationHud/Copy → native labels; teardown removes old world/roster | FLOWING |

No required dynamic value ends in a hardcoded empty collection, mock provider or discarded response. Test replacement adapters remain explicitly test evidence; native production composition uses the real providers.

### Behavioral Spot-Checks

The original selected-set execution was already completed on the repaired source. To preserve it and avoid redundant launches, this verification inspected the actual named reports and reran only read-only validation; the rows below distinguish retained execution from verifier work.

| Behavior/check | Command or actual report checked | Result | Status |
|---|---|---|---|
| Actual common authority and replay rejection | R/core.authority.ctrf.json; R/core.ordering.ctrf.json, bound named tests | One passed test each; actual acceptance-count, position, readiness and high-water assertions | PASS — retained execution |
| Deadline/cleanup/replacement invariants | R/core.lifecycle.ctrf.json, LifetimeCleanupAndDeadlineBoundaries | One passed test; 29999/30000/30001 and 9999/10000/10001 plus response ordering, old work and exact close | PASS — retained execution |
| Detached publication, profile persistence and service substitutions | R/core.protocol, adapters.profiles, adapters.substitutions named CTRF files | One passed test each; real value/state assertions, atomic failure checks and unchanged Core hashes | PASS — retained execution |
| Real transport/discovery/native input and four-peer lifecycle | R network.* and rendered.* case/peer/draw/log records | All 32 Phase 3 cases pass with distinct real native subjects | PASS — retained execution |
| Physical response/position agreement | Independently recomputed R/rendered.timing brackets and stopped rows | 48 physical rows ≤197.8586 ms; 16 stopped rows error 0, ≤171.4658 ms | PASS |
| Exact full completion on current metadata descendant | `node tools/phase3/check.cjs --verify-completion .planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json` | Exit 0, status pass, original tested revision retained, current validated revision 25ef9c8… | PASS — executed by verifier |
| Image and reference identities | Independent SHA-256 calculation over all index review PNGs and main referenced artifacts | 319/319 bindings, 255 distinct contents; final/selection/visual/audit hashes match | PASS — executed by verifier |

The completion command used the task-local SDK isolation from setup, `SourceRevisionId=c69442148c975de974ebbb1d8ef409380940ecb0`, and `CustomAfterMicrosoftCommonTargets=<root>/tools/phase3/reviewed-source.targets`. It performs no build or game launch.

### Probe Execution

No `scripts/*/tests/probe-*.sh` directory or phase-declared shell probe exists. Step 7c is not applicable; the actual phase check/completion entry points above were evaluated instead. No executor PASS-marker narration substituted for a declared probe.

### Requirements Coverage

| Requirement | Source plans | Approved behavior | Status | Evidence |
|---|---|---|---|---|
| LAN-01 | 01, 02, 03, 04 | One host and up to three guests connect in separate game processes | SATISFIED | R1/P2.1/P3.1; actual host+three, discovery/manual, capacity/race/cleanup and coherent moving join |
| LAN-02 | 01, 02, 03, 04 | Only own actor moves; all observe consistent movement | SATISFIED | R2/P3.3/P4.3; common ownership validation, independent cameras, real controls/focus and exact measured timing/agreement |
| LAN-03 | 01, 02, 04 | Same authority for solo/host-local/guest; no duplicate execution | SATISFIED | R3/P2.2/P2.3; passing named role/replay/sequence/lifetime tests and production native path |
| LAN-04 | 01, 02, 03, 04 | Independently replace services and preserve identity independently of addresses | SATISFIED | R4/P3.2/P3.4; each actual port swap, persisted restart, exact opaque identity and unchanged Core rules |

All four requirement IDs in PLAN frontmatter exist in REQUIREMENTS.md and map solely to Phase 3. The reverse traceability check finds exactly LAN-01–LAN-04 assigned to Phase 3, all claimed by at least one plan: **0 orphaned requirements**. Source A-38 links additional SESSION/DATA/REL obligations with later owners; this report does not claim the full A-38 campaign/release scenario.

### Decision Coverage

All trackable CONTEXT.md decisions are honored by shipped artifacts.

Authoritative decision coverage result: **4/4**, no missing entries, nonblocking gate. English native copy, inherited primitive scene direction and agent-discretion choices are visible; no new creator preference is inferred from agent styling defaults.

### Test Quality Audit

All thirteen C# report bindings are active named tests with zero skipped/pending results. The requirement-linked files were scanned for disabled tests; no actual disabled declaration was found. A broad substring match on `WaitForExit(` is not a skipped-test construct.

| Test file/family | Linked requirements | Active named cases in R | Skipped | Assertion level / circularity | Verdict |
|---|---|---:|---:|---|---|
| FoundationLanAuthorityTests.cs | LAN-02/03 | 1 | 0 | Production acceptance before Step, role movement values/counts and post-step publication; not circular | PASS |
| FoundationLanAdmissionTests.cs | LAN-01/03/04 | 1 | 0 | Pending-inclusive counts, race/refusal, selected-session identity and before/after ownership | PASS |
| FoundationLanOrderingTests.cs | LAN-02/03 | 1 | 0 | Exact rejected-state reference/high-water and stop/reversal position values for local/remote routes | PASS |
| FoundationLanProtocolTests.cs | LAN-01/03/04 | 1 | 0 | Detached actual codec/state rejection with preserved current publication | PASS |
| FoundationLanLifecycleTests.cs | LAN-01/02/03/04 | 1 | 0 | Boundary triples, ordering, replacement/close counts and old callbacks | PASS |
| FoundationLanServicesTests.cs / FoundationServiceSubstitutionProof.cs | LAN-01/04 | 3 | 0 | Real socket/file/provider composition; exact identity/bytes/positions and unavailable services | PASS |
| FoundationLanInputTests.cs | LAN-02 | 1 | 0 | Context/focus/alias/fresh-press and local projection values; actual native checks complement pure assertions | PASS |
| FoundationLanPermissionTests.cs | LAN-03/04 | 1 | 0 | Compiler-valid positive subjects then intended scanner rejection, exact independent inventory | PASS |
| FoundationLanRunnerTests.cs plus phase3 JS controls | LAN-01–04 evidence | 3 C# plus actual runner controls | 0 | Real child failures, exact identity/hash sets and over-limit rejection; fabricated negatives earn only rejection credit | PASS |

**Disabled requirement tests: 0. Circular production-pass patterns found: 0. Insufficient behavior assertions on the must-have proofs: 0.**

The disconfirmation pass explicitly checked three plausible false passes: host-local mutation bypass, stale high sequence suppressing a current stop, and old connection cleanup deleting a replacement. Production code prevents each and named passing tests assert the affected state transitions. A passing synthetic/headless test alone cannot prove physical rendered responsiveness; that misleading shortcut is excluded by separate required native timing and provenance reconciliation. Actual mixed-computer LAN failure modes remain unexercised in this local evidence, with explicit later ownership.

### Anti-Patterns and Advisory UI Findings

No unreferenced TBD/FIXME/XXX marker, unfinished runtime stub or orphaned required implementation was found in the phase production/test/tool scope. Empty collections in profile failure, disposed discovery and closed lifetimes are functional states with real population/teardown paths. Address placeholder text is an input example.

The independent `03-UI-REVIEW.md` records **19/24**, eight WARNING recommendations and zero BLOCKERs. Source/image checks support retaining these findings; the earlier broad visual PASS must not be read as exact UI-contract conformity.

| Finding | Actual file/location | Severity | Phase-goal assessment |
|---|---|---|---|
| UI-01: fields/session rows lack the common gold outer focus | Foundation.tscn theme and dynamic FoundationMenu rows | Advisory WARNING | Focus remains visibly indicated; native keyboard/mouse join/control flows pass. Exact accent treatment needs polish. |
| UI-02: roster effective 24px text produces 34px lines | Foundation.tscn Roster child Labels | Advisory WARNING | All four P-number/You entries remain readable in current 1280×720 images; compact 16px/24px role is not fully applied. |
| UI-03: browser heading lacks shared bold role | FoundationRoot heading assignment / Browser Title | Advisory WARNING | Heading is visible/readable; typography drift does not prevent hosting/joining or movement. |
| UI-04: profile-to-action gap is 32px rather than 16px | Foundation.tscn menu geometry | Advisory WARNING | Complete menu and Quit remain visible; no blocked flow. |
| UI-05: native Label/LineEdit fallback colors differ from palette | Foundation.tscn Theme | Advisory WARNING | Current text/controls are readable; exact palette is incomplete. |
| UI-06: remote body lacks dark inset detail | FoundationWorld.tscn / FoundationView | Advisory WARNING | P1–P4 and local You distinguish actors; required synchronized placeholder/control behavior remains achieved. |
| UI-07: long summary/roster full-text disclosure incomplete | FoundationMenu Summary; FoundationHud/roster mouse filter | Advisory WARNING | Real optional display-detail limitation. Fixed P-number/You and profile selection retain ownership distinction; full opaque identity remains persisted/validated. No false exact-disclosure pass claimed. |
| UI-08: address edit retains obsolete error until next attempt/navigation | FoundationMenu event wiring / FoundationRoot invalid address feedback | Advisory WARNING | Error is recoverable and actual retry/cancel/navigation remain usable; automatic edit-clearing is a real UI refinement. |
| Prose key-link metadata cannot be evaluated by file-path CLI heuristic | All four PLAN key_links.from fields | Metadata WARNING | All twelve actual links were manually proved wired; no missing source/wiring was concealed. |

These recommendations do not correspond to a FAILED or UNCERTAIN roadmap/plan observable truth: the stated controls are usable, identities distinct, actors/markers readable, local camera/control correct, and the authoritative movement outcome behaviorally demonstrated. They are recorded without inventing a human checkpoint or changing the mandatory behavior scope. No recommendation is silently marked fixed.

### Human Verification Required

**None remaining for this phase-local contract.** The plan's deferred actual-PNG/timed-frame check explicitly permits agent-operated inspection and is fulfilled by the hash-bound actual review plus separate timed native evidence. This is a user-facing phase; the absence of open human items follows completed observation, not an infrastructure auto-pass shortcut.

No future environment obligation receives a pass. Hosted Windows/Linux workflow execution belongs to Phase 5; full campaign/reconnect/save behavior to its gameplay/Phase 9 owners; actual physical mixed-computer/mixed-OS LAN, supported-display release checks, hardware/performance and endurance to Phase 10; nightly full active-catalog runs to Phase 11. Original Phase 1 qualitative/Linux follow-ups remain explicit under their own accepted closure exception. The current policy supersedes older Phase 3 two-local-OS/VM/extra-resolution wording. None is recast as a newly blocking local check.

### Selection, History and Cleanup

The independent catalog and saved selection account for every new Phase 3 case, all inherited Phase 2 cases and affected/Critical Phase 1 integrity controls. The 243 active omitted cases are **not selected for this phase run**; eight Phase 5 cases remain planned. Neither category receives pass/skip/reuse credit. The full current 481 execution follows the repaired source and retains original failures, including the fourth full behavior pass with failed browser/edge-marker appearance.

R and the retained audit record 14 native and 17 headless terminal outcomes, normal or deliberately declared losses, zero emergency kills, eleven helpers ended, no desktop lock, and 688 console logs without the recorded error/warning matches. The completion validator independently enforces bound terminal ownership and native error-log constraints. These are local process/cleanup facts, not physical LAN or release certification.

### Fingerprint and Status Self-Check

The report fingerprint was produced by the authoritative CLI, not hand-computed:

```powershell
$phase3Covered = @(git diff --name-only 87e0b44 c69442148c975de974ebbb1d8ef409380940ecb0 -- .gitignore src tests tools docs)
# Append the explicit evidence/policy paths listed in covered_files above;
# PLAN/SUMMARY entries are automatically included by the owner.
node .codex/gsd-core/bin/gsd-tools.cjs query verification.fingerprint .planning/phases/03-four-player-lan-foundation @phase3Covered
```

The executed command appended AGENTS.md, current collaboration/local-testing policy, phase CONTEXT/SPEC/UI-SPEC/RESEARCH/SCENARIOS/SELECTION/VALIDATION/SECURITY/UI-REVIEW/REVIEW JSON+Markdown/disposition/EVIDENCE/VISUAL-REVIEW, the final immutable full report and the two ignored evidence/regression audits. The returned **148 covered_files** and **covered_digest** are copied verbatim in frontmatter. Planning-root lifecycle docs and this report are inert under the fingerprint owner's rules.

After writing, `node .codex/gsd-core/bin/gsd-tools.cjs query verification.status .planning/phases/03-four-player-lan-foundation --pick status` returned `passed` with exit code 0. The report's frontmatter status and current evidence fingerprint are accepted by their owner. The worktree contains only this new verification report from the verifier; no commit was created.

### Gaps Summary

**No blocking goal-achievement gaps.** All four roadmap criteria, seventeen additional plan truths, fifteen declared artifacts and twelve manually resolved key links are supported by substantive production source, actual data flow and source-bound behavior evidence. Eight presentation warnings and one key-link metadata warning remain transparent recommendations. No override, deferred-gap substitution or unverified behavior was needed to reach the passed verdict.

---

_Verified: 2026-11-16T05:53:14Z_
_Verifier: independent goal-backward agent (gsd-verifier)_

