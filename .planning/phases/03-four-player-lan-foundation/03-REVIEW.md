---
phase: 03-four-player-lan-foundation
reviewed: 2026-11-16T01:54:34Z
depth: standard
reviewed_revision: 2f20e72fec4156322f2b3646d08977b5eb52d675
build_revision: 2f20e72fec4156322f2b3646d08977b5eb52d675
diff_base: 87e0b44
scope_files: 120
files_reviewed: 117
files_reviewed_list:
  - .gitignore
  - docs/code/engine-generated-spans.json
  - docs/code/exemptions.json
  - docs/code/generated-callsite-adaptations.json
  - docs/code/register.json
  - docs/code/rule-matrix.md
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
  - src/godot-game/foundation/FoundationCoordinator.cs
  - src/godot-game/foundation/FoundationCoordinator.GuestPublication.cs
  - src/godot-game/foundation/FoundationCoordinator.Lifetime.cs
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
  - tests/phase1/GraphFixture.cs
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
  - tools/phase3/inherited.test.cjs
  - tools/phase3/navigation.test.cjs
  - tools/phase3/network.cjs
  - tools/phase3/png-worker.cjs
  - tools/phase3/png.cjs
  - tools/phase3/processes.cjs
  - tools/phase3/rendered-stream.test.cjs
  - tools/phase3/rendered.cjs
  - tools/phase3/required-cases.json
  - tools/phase3/reviewed-source.targets
  - tools/phase3/revisions.cjs
  - tools/phase3/revisions.test.cjs
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

**Reviewed:** 2026-11-16T01:54:34Z  
**Depth:** standard  
**Source files reviewed:** 117  
**Explicit scope:** all 120 paths from `.phase3/review-fix-union.txt`, preserving the complete original scope and subsequent helpers/consumers, including the new PNG worker; three planning-context paths are listed separately above  
**Status:** clean  
**Reviewed source / pinned build revision:** `2f20e72fec4156322f2b3646d08977b5eb52d675`  
**Diff base:** `87e0b44`

## Summary

This independent re-review preserves the complete original source/context union and all earlier assessments, adding the phase-only SourceLink target and the later graph-fixture/matrix consumers for a total of 120 paths / 117 source paths, including the draw-stream control and PNG worker. The complete Phase 3 source review above is carried forward together with a direct recheck of the six-path repair diff from `a35bd6f` to `166a32ad6a7541c4f5eed1378def0a86e9bea632`: guest publication, native projection, native regression, protocol proof, independent catalog and saved scenarios. Every repair path was already inside the retained scope. Their production callers, trust boundaries and actual regression consumers were traced; the cumulative fix ledger was treated as a claim to confirm.

The final focused integration recheck additionally examines committed source `e25a500d12006e256d84b8b241d0cee9c507f278`. Its only source changes since the prior clean review are `tools/phase3/revisions.cjs` and `revisions.test.cjs`, both already in the 115-path scope. The exact five added closure-report permissions and their actual Git/SDK fixture consumers introduce no new actionable finding; the existing fourteen resolved dispositions are preserved. Intervening canonical review/gate/fix/continuation and shared-state files are metadata, not additional changed game source.

A final actual-SDK SourceLink recheck additionally examines source `cff08ebe7cc78c562fe370dcdc3718e116938905`. An actual metadata-HEAD rebuild exposed independent `SourceRoot.RevisionId` drift despite the assembly `SourceRevisionId` pin; exact tool validation stopped before any full run or window. The bounded repair adds an exact own-root SourceLink pin, passes it to Phase 3/inherited builds, and extends the real SDK fixture with a GitHub remote and an import-omitted negative control. This CR-07 follow-up is resolved. The source/game, native/public/generated permission and fourteen prior behavioral dispositions remain intact.

The prior four-file real-run repair batch at `dc523e9` remains sound on inspection. Checking its authentic passing reports exposed CR-10's inherited evidence-base mismatch; the current focused recheck independently verifies its two-file repair at `7b140a27e4f5556db12caed2b1614c52a1aad18a`. The production inherited reader now shares the corrected exact case/report/evidence contract with controls using the real nested Phase 1 layout.

The preceding bounded recheck covered the three-file native batch-2 repair at `af8d4f26fcb220f1c18b1dc5b749b6bb219958a2`: physical input helper, ordinary text-entry procedure and immutable raw timing capture. The current recheck additionally verifies the single per-character foreground guard at `d2d5a6106990aaf16b0b02a5f296fc7b11aa0f3e`, described below; no new actionable defect remains. All fifteen prior resolutions and the complete 118-path / 115-source scope are retained. The previous focused native run remains failed; source-review clearance does not certify its repaired behavior before another actual execution.

The latest bounded recheck covers native batch 3 at `4ea54ca731c90de700480033f58d17fc5ae8e00b` (commits `2902f7a` and `4ea54ca`): the existing PNG decoder and owned-window focus helper. Their changes preserve evidence semantics, all timing thresholds and strict ownership rejection. No new actionable defect was found; the scope remains 118 paths / 115 source paths and all fifteen earlier findings remain resolved. Historical failed native runs still require a successful focused retry followed by the entire selected pass.

The preceding batch-4 recheck found CR-11 in ordinary browser cancellation. The current four-file repair at `f32e0bd8911e0c185a6fa10b24c251f1d69494cd` transfers cancellation to the Root-owned runtime retirement helper, retains older unfinished drains during rapid replacement and polls/disposes every retiring coordinator through its proper lifetime. The production Menu control uses that same retirement owner and passes its pending-connection, repeated-cancel and 249/250 ms assertions.

The current bounded source recheck covers batch 9 at `2f20e72fec4156322f2b3646d08977b5eb52d675`: explicit framework-only delivery semantics, reliable control traffic and replaceable ordered complete-state publication, their provider/substitute consumers and authority controls. No new actionable defect was found. All seventeen prior findings and the complete 120-path / 117-source scope remain resolved/preserved. The latest full-selection result remains 480/481 with a real delivered-state timing failure; current headless proofs do not establish repaired desktop acceptance.

**All seventeen prior findings are resolved. No new actionable defect or open finding remains.** The source scope is 120 paths / 117 source paths. Previous passed/failed records keep their actual revision/tool identities; no failed native run or pending full selection is upgraded by this review. Actual no-window and rendered acceptance retain the limits below.

This is an independent source re-review during the post-review repair cycle, not completed rendered acceptance. No real game window, VM, desktop input, broad test suite, build, branch operation or commit was executed by this reviewer. The renewed retained no-window report was reconciled through the current production validator and its source/tool tuple independently recomputed. The earlier .NET arithmetic probe remains defect-history evidence; the repair's actual coordinator and headless native projection executions are retained separately below. The first full selected execution now has real desktop results and retained failures, as recorded below; it is not accepted final completion. No new rendered, physical-input, image-appearance, other-OS, physical mixed-computer LAN or release acceptance is granted by this source review.

Necessity was assessed using the required rubric; no necessity finding is asserted. No structural pre-pass or external reviewer lane was supplied. The agent-skills query returned no additional configured reviewer skills.

## Narrative Findings (AI reviewer)

No open Critical/BLOCKER, Warning or Info findings remain at this reviewed source revision.

## Native repair batch 9 bounded production-delivery recheck

**Current reviewed source/build stamp:** `2f20e72fec4156322f2b3646d08977b5eb52d675`. **Disposition:** clean on source review; actual repaired native delivery/timing and full-selection acceptance remain separate.

The latest full-selection invocation `f736803d-5062-450b-a5b7-b29ca0d36f4a` remains failed at 480/481 after about 53.6032 minutes. All 449 inherited cases passed. Its sole failure is rendered timing: the first host-to-guest2 start conservative upper interval is 487.0861 ms, versus about 116.24/135.31 ms for the other two guests. Native acknowledgement is about 29.31 ms; guest2 capture request/completion are about 416.9253/486.0861 ms. The immutable report retains actual execution HEAD `9226cd5e8e41ebdb878f4ee69396010803cb1ab2` and prior 551d build stamp/tool, with fourteen game windows. It is not a pass at the current source.

The retained `.phase3/batch9-start-trace.json` uses actual per-peer draw/revision/position streams. Guest2 continues drawing near 60 Hz while receiving newer host snapshots slowly, eventually reaching the host's first changed revision 112 much later. This identifies a delivered-state delay rather than attributing all latency to PNG CPU work. It does not subtract cross-process clocks or change the timing predicate/threshold.

The eleven changed paths were reviewed through their provider, coordinator, baseline installation, readiness and lifetime consumers. `IConnectionService.Send` gains an optional framework-only reliable flag defaulting to true. Every implementation accepts the same default; the replacement forwards it, and the harness records it. Existing callers preserve reliable delivery. The coordinator explicitly selects false only for complete `state` envelopes. Baseline, admission acknowledgements/refusals, intent, liveness, departure and end acknowledgement remain reliable. No protocol schema or core movement model changes.

The authentic ENet provider sets Reliable or UnreliableOrdered immediately before its existing target, PutPacket and flush operations under the same native-thread/connection/size/peer fences. The default channel remains zero. Godot documents that this channel separates the transfer modes; ordered non-retransmitted state therefore does not share reliable control ordering. This matches the provider's intended use of replaceable movement snapshots. [Godot MultiplayerPeer delivery/channel contract](https://docs.godotengine.org/en/latest/classes/class_multiplayerpeer.html#class-multiplayerpeer-property-transfer-channel).

The guest does not assume relative order between the two delivery modes. A state received before the reliable baseline is ignored while no remote binding exists. After the baseline establishes ownership and sends its reliable acknowledgement, a later complete matching state can establish readiness; dropping an intermediate state does not require delta recovery. A candidate must retain exact session/attempt/local binding/player/actor identity, increasing revision and non-regressing actor elapsed counters, and cannot remove an already-ready local readiness flag. Late older states reject without rewinding the installed snapshot or refreshing liveness. Reliable baseline/end ordering is retained within the control stream, and closed/draining lifetime fences prevent late state from reviving a retired coordinator. Loss of all later publications remains subject to the existing bounded join/response timeouts.

The new authority control executes the production handshake, asserts state uses the replaceable mode and every observed control envelope remains reliable, drops an intermediate complete publication, installs a later one and rejects the late older one. It also asserts reliable bye/end cleanup. Admission exchange tuple consumers were updated for the recorded delivery flag without changing their envelope semantics. The intended RED report `b7055e81-cb67-4b47-8396-2d37ef42af18` captures the pre-repair reliable-state assertion failure; the earlier analyzer-failing attempt remains separate history. The seven-core working-tree repair report `17612f9f-03a4-4bd5-94bc-0823d5c409a1` passes at actual HEAD `fb506c04763df75ae461fa53b751543ca7cd0339`, prior 551d build stamp, input `1cad92445525c7117baac34bbf0c1ca1bcc0761a27579805d738c49a8f87c602` and tool `8543e1e2d679aa2689774ea01816c98523190bf381643b9e780832670bff3765`. Those identities remain unchanged and are not relabelled as a committed current run.

The final pinned strict log `.phase3/batch9-pinned-strict.log` records zero warnings/errors and 11.28 seconds. The committed affected report `.phase3/runs/win32-x64/64d4beaf-a878-42d8-bf3c-86a701639244/summary.json` passes six checks: network tracer, four-peer, lifecycle, idle polling, discovery/profile and adapter substitution. It records zero game windows, exact current source/build stamp above, input `244f027f2093de29cd07a2646a2eac4a3256dd15dad949354b95a2bf6497cd47` and tool `fa2bcb23a87728914fa1e26ddd9e5efd346b1fb980f1d63ac2eef9a9f58c27dd`. Retained reports and source were inspected read-only; this reviewer ran no test, build, native window, gate or desktop input.

No new native/public permission, generated-span refresh, Root signature/field, package/project reference, timing threshold, clock subtraction, PNG validation or acceptance reconciliation change appears in this diff. The exact existing 45 native permissions, eight added public types and 220 generated entries remain within the carried-forward scope. Renew the exact gate and matching metadata-descendant rebuild, obtain affected native acceptance, then rerun the fresh entire 481-case selection. Existing failed full aggregates remain failed.

## Native repair batch 8 bounded production-publication recheck

**Historical reviewed source/build stamp:** `551d7c6f379f701c5271508bf68ca2730efe9057`. **Disposition:** clean for that bounded review; the subsequent full-selection failure and delivery repair are recorded above.

The prior focused native invocation `12951d61…` passed twelve of twelve cases, all 48 timing samples (maximum conservative upper interval 212.5111 ms) and sixteen stopped comparisons. The subsequent fresh full selection `30ebcea8-34d2-4659-bc94-1ac112b82622` ran about 53.7243 minutes and retained a failed aggregate: 480/481 selected cases passed, including all 433 Phase 1 and sixteen Phase 2 cases; only Phase 3 rendered timing failed. Its host-to-guest2 stop upper interval was 296.5992 ms. Fourteen terminal records comprise twelve normal exits and two declared losses, with no emergency cleanup; eleven helpers ended and the lock was absent. Checkpoint `ae0aae6` preserves this failure. Neither the focused success nor this source review converts the later full-selection failure into a pass.

The retained raw stop trace records about 2.0168 ms physical delivery, 42.2695 ms native acknowledgement, capture request at 228.3797 ms and completion at 295.5992 ms (67.2195 ms capture duration before uncertainty). `.phase3/batch8-stop-trace.json` preserves actual per-peer frame/position observations. The host's earlier update/step/publication at revision 144 occurred before local stop acceptance at that same revision; guest2 could already repeat the final position there, while the required newer authoritative publication arrived at revision 150 under the old cadence. This diagnosis uses each stream's own ordering and positions, without assuming synchronized cross-process clocks.

The three changed paths were read with their immediate input, readiness, admission, expiry and lifetime consumers. `FoundationRoot._Process` still polls released physical keys first, but now submits a changed direction while the coordinator is already ready before its `Update`. Thus local host intent is accepted before that frame's simulation step instead of immediately after it. A joining guest does not submit before readiness; a newly installed world still clears held input, and subsequent frames use the existing direction-change path. Focus loss, menu/scene transitions and cancellation continue clearing input and retiring the current coordinator through their existing owners. The moved block adds no Root field or method signature and leaves completed/inactive handling fenced.

`FoundationCoordinator.Accept` marks one pending publication only after the existing ownership, binding, readiness and sequence validation succeeds. `Update` still receives and expires first, returns if the lifetime became inactive, then invokes the actual session Step before consuming that flag. A pending accepted intent therefore causes one full post-Step broadcast in that update, including a revision newer than the acceptance revision, without advancing position inside Submit. Multiple accepted commands before one step coalesce; rejected commands do not request publication. Ordinary publication is due after 16 accumulated simulation milliseconds and broadcasts at most once per update, preserving the remainder with modulo 16 rather than running an unbounded catch-up loop. Guest coordinators do not broadcast host state. Admission and departure publications retain their separate existing paths; close/retirement does not reactivate pending simulation.

The authority control proves no ordinary broadcast after 15 ms, one after the next 1 ms, and prompt 1 ms post-Step publication for local-host and remote-guest start, reverse and stop. It checks unchanged position immediately after Submit, exact published position/elapsed increments and a full revision newer than `LastAcceptedRevision`. The intended RED report `c5d2272b-61c1-4e1c-baef-edf399f740d3` fails core.authority before the repair. The repaired working-tree seven-core report `d25f9886-0ef7-4545-906c-f9205edf8508` passes all seven, including permissions, but its immutable actual HEAD is `ae0aae6aebbf843cb7c045f0c6fe932e4712123a`, build stamp is the previous 93e source and tool is `29eb1dc1263e061b42e0b6b85892aaec0644d9a23902b803f65b9e35dcad05cb`. Its input matches the current repaired tree; it is working-tree repair evidence and is not relabelled as a committed 551d execution.

The final pinned strict build `.phase3/batch8-pinned-strict.log` records zero warnings/errors and 11.07 seconds. The affected network selection completed during review: `.phase3/runs/win32-x64/c4759f76-68fe-4663-af75-b139832664e4/summary.json` passes all five checks (tracer, four-peer, lifecycle, idle polling and discovery/profile), with zero game windows and exact current source/build stamp above, input `a3062a7df72114a81c5494e37e6b97fe42a12cbb2844b463ae8cfac92c157714` and tool `56d2627f5e0e2d917b35acf98200210b0695918a2db1ff702c65f756bd426026`. Its lifecycle record retains graceful guest leave at 69 ms and unexpected loss timeouts around ten seconds. These retained results were inspected read-only; the reviewer ran no build, proof, game window, gate or desktop input.

This repair changes production response/publication order and cadence, leaving the timing clocks, conservative 250 ms threshold, PNG/state validation, stopped proof, native provenance and acceptance reconciler unchanged. It adds no core-model change, native/public permission, generated-span refresh, Root signature, project or package reference. The previous 45 native permissions, eight added public types and 220 exact generated registry entries remain within the carried-forward review scope. Actual desktop margin still requires a successful affected native execution and fresh entire selected-set pass.

## Native repair batch 7 bounded recheck

**Historical reviewed source/build stamp:** `93e8552c1518a738af24a120ece8d99c5f0bfcab`. **Disposition:** clean for that bounded review; subsequent native/full-selection evidence and the production publication repair are recorded above.

The retained batch-6 native report `1d9430ed-2a69-4609-95c2-1eb66e94a625` accepts eleven of twelve cases. Its sole remaining host-start timing failure records conservative upper intervals of 254.4477, 280.1131 and 301.4942 ms. Profiles/restart passed; fourteen terminals comprise twelve normal exits and two declared losses, with no emergency cleanup. That failed aggregate and its original timing rows remain unchanged. Checkpoint `472b392` retains this failure.

The four changed source paths were read through the capture, transition and scenario cleanup callers. The new `png-worker.cjs` owns three persistent Node worker threads. Each job carries a distinct ID and one assigned slot; unexpected replies fail the pool rather than being credited to another capture. Active jobs reference their workers, have an 8000 ms rejection timer, and release the reference on completion. Worker errors, unplanned exits, identity failures or timeout reject queued/in-flight jobs; late replies cannot resolve rejected work. An ordinary PNG validation error rejects its capture and leaves subsequent valid jobs independently checkable. Scenario cleanup awaits termination of all owned workers before releasing the shared scenario lock, and queued/in-flight jobs are rejected on close. A later invocation creates its own fresh pool.

Each worker calls the unchanged full `png.validate` implementation and hashes the exact PNG bytes with SHA-256. CRC/structure, format, reconstruction, dimensions and content validation remain mandatory. The main capture awaits that result, validates capture/state identity and computes the state-file hash before recording its final timestamp. Job queueing, inter-thread communication, PNG validation/hash work and completion handling therefore remain inside the measured delivery-to-completion interval. Ordinary earlier captures warm the pool; no measured timing stage is subtracted or replaced with worker-reported time.

Each observer now starts capture as soon as its own actual transform/stability/reversal predicate supplies its required witness. Worker CPU work does not synchronously block the other observer pollers, so the previous all-peer pre-capture barrier is unnecessary and removed. The original physical `deliveryBefore`, accepted host intent, immutable raw timing samples, inclusive 250 ms threshold, 1 ms uncertainty, exact draw/image identity binding, reversal witnesses and full stopped reconciliation remain unchanged. There is no hash normalization, validation bypass or synthetic timing credit.

The retained intended RED `.phase3/batch7-png-red.log` fails because the newly required worker module was absent. GREEN `.phase3/batch7-controls-green.log` records ten passing controls. The added control compares four concurrent worker results against unchanged synchronous PNG metadata and exact SHA-256, checks main-loop progress, rejects a corrupt CRC, verifies later valid work, closes the workers and checks recreation. The real three-image benchmark `.phase3/batch7-png-benchmark.json` retains equal hashes/metadata and records about 68.65 ms serial versus 26.05 ms parallel, with a main-loop tick at about 6.76 ms; this is diagnostic evidence, not rendered acceptance. The strict pinned build log `.phase3/batch7-pinned-strict.log` records zero warnings/errors and 10.54 seconds.

The current terminal `runner.timing` report `.phase3/runs/win32-x64/36d6cfa2-c716-4f71-bbc6-27a2452eed2f/summary.json` records pass with zero game windows, exact source/build stamp above, input `993436c8b6cba62d604754317b956e93a61ae007259d772cf6d7b041cc98ab40` and tool `ea0d2105693e044d40265f4bfe9f1e0f1f9f541bd44a64f2e762247c18573f99`. These are retained parent-owned executions inspected read-only; this reviewer ran no build, test, worker benchmark, gate, game window or desktop input during the follow-up.

Renew the exact 93e gate and perform the matching metadata-descendant rebuild with both build properties, then obtain affected native acceptance before the fresh complete 481-case selection. Eleven-of-twelve history does not establish the repaired timing result.

## Native repair batch 6 bounded recheck

**Historical reviewed source/build stamp:** `ba0b1b79499a1f9a17055bd3af61fedf8473d613`. **Disposition:** clean for that bounded review; its subsequent actual native failure and current timing repair are recorded above.

The retained native report `7aaba10c…` accepts ten of twelve cases. Its remaining profile interaction did not change the selected second profile after Up/Enter; its host-to-guest1 stop interval was 299.0516 ms while the other two observers were about 174/197 ms. Native input acknowledgement took about 28 ms, but image work for another observer began while guest1 was still collecting its stability predicate. The original over-limit row remains a failure.

The three changed paths were read through their immediate callers. `FoundationDrawObserver.ObserveControl` adds `popupFocusedIndex` via the read-only native `GetPopup().GetFocusedItem()` getter inside the existing guarded permission tuple. No action/selection setter, new native tuple or model mutation is introduced. The profile procedure requires the actual popup to remain open, sends physical Down when nothing is highlighted or physical Up while above the original index, waits for the observed highlight to change and permits at most eight attempts. It requires highlight zero before physical Enter, then still requires the exact original profile identity. Missing/closed/wrong selection remains a failure.

`rendered.transition` now first awaits the existing actual transform/stability predicates for every observer and retains their baselines/witnesses. Only after all predicates have completed does it start the required image captures/decodes. This prevents one observer's synchronous PNG work from postponing another observer's predicate polling. It does not subtract or exclude that delay: every sample retains the original before-all-delivery `ack.deliveryBefore`, and its end remains after complete capture/state/image validation. Waiting for the slowest peer, all capture scheduling/read/decode work and the 1 ms uncertainty remain included. The original inclusive 250 ms validator, draw/image/identity binding, reversal witnesses, accepted host intent and terminal/cleanup rules are unchanged.

`.phase3/batch6-pinned-strict.log` records zero warnings/errors, 10.85 seconds. `.phase3/batch6-controls.log` records nine passing existing timing, full stopped reconciliation, capture, navigation and foreground controls. The current parent-provided identity is input `39cca8844ef6ac39d761fc47f23ce4aadd85e2d5b8d1bc8de1b5761b25dfcf6e`, tool `be97ef6731b533dc578706678d5990dc806cad9f990e0cac6584ed56b9e51926`, source/build ba0 above. The parent-owned affected selection finished during review: `.phase3/runs/win32-x64/a5a5c564-5d85-4514-919e-bb842f3419af/summary.json` passes all three cases (`runner.timing`, `network.discovery-profile`, `core.permissions`), with zero game windows and exact ba0 source/build/input/tool identity. The reviewer loaded that terminal report through the current actual reconciler. The reviewer ran no build, test, gate, game window or desktop input.

Require the affected selection's successful terminal result, renew the exact ba0 gate and matching metadata-descendant rebuild, then retry native cases and obtain the fresh entire selected-set pass only after focused acceptance. Older ten-of-twelve results and timing overruns remain immutable history, not final acceptance.

## WR-06 bounded exact-session interaction recheck

**Historical reviewed source/build stamp:** `df07eba7e978401ce9e8d83b42d03bc4b3848447`. **Disposition:** resolved; the exact-session guards remain in the current source.

The one-file follow-up returns the actual target produced by the awaited exact-session/scroll lookup, replacing the pre-wait captured path. The browser caller passes its expected exact session ID into the physical click helper. After foreground acquisition and current-node lookup, that helper rejects any changed session metadata before sending the click. The browser then requires the observed focused row to carry the expected exact session before recording focused detail and submitting Join session. Vanished/reused/clipped paths remain non-success. Physical Tab/follow-focus scrolling and mouse interaction are preserved.

The reviewer inspected the bounded `scenarios.cjs` diff and the three guards through their caller. The previous six-path reversal/clip/popup review is retained unchanged. `.phase3/wr06-strict.log` records zero warnings/errors, 11.30 seconds; `.phase3/wr06-controls.log` records all three foreground/navigation controls passing. The current parent-provided identity is input `d49dd2398af1e1d89291605ef268cee464789baf1a33f2c2e17739339a2547d8`, exact tool `fde3f87242dcb3f2c4de0fcd0cc729095d9091439d6dbf4ccd38f457112caf77`, source/build df07 above. This follow-up launched no reviewer build, test, gate, game window or desktop action.

The parent may renew the exact df07 gate and matching metadata-descendant rebuild, then retry all twelve native cases. Actual interaction/timing/appearance acceptance and the final full 481-case selected pass remain outstanding; existing failed reports and timing rows retain their original identities.

## Disposition of all seventeen prior findings

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
| CR-11 | Resolved | Ordinary Menu Cancel no longer closes/discards its coordinator. Root cancellation replacement and ReturnToMenu share runtime-owned RequestClose retirement; older unfinished drains are queued during rapid replacement, polled to deadline, and disposed at teardown. The actual production Menu/native regression uses the same owner and verifies no premature transport close, repeated cancellation, two rapid attempts and close-once at 250 ms. |
| WR-06 | Resolved | `focusSession` returns the awaited current exact-session target path. The caller supplies the expected session to `click`, which rejects changed metadata after foreground/current-node lookup and before physical delivery; the browser also requires the post-click observed focused row to retain that session before Join submission. No physical interaction, visibility or identity check was removed. |

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

The source review is clean at `2f20e72fec4156322f2b3646d08977b5eb52d675`, with all seventeen findings resolved and the eleven-path delivery-contract/provider repair reviewed. The exact current six-check headless proof passes with zero game windows; the earlier seven-core working-tree proof retains its actual HEAD/build/input/tool identity. The parent may renew the exact source/input/tool gate and perform the matching metadata-descendant rebuild with both build properties. Then obtain actual affected native acceptance before the required fresh full 481-case selected-set pass. Preserve every failed 480/481 aggregate, prior focused successes, all raw timing rows and original invocation identities. This source review grants no repaired native delivery/timing, image-appearance or completed selected-set acceptance. The parent owns gate/rebuild/publishing and actual desktop execution.

The parent hash-verified the four temporary iteration-2/3 review/fix scratch originals against their ignored `.phase3/review-history` archives and removed the untracked phase-folder copies. The focused recheck observed no remaining scratch paths in Git status. Preserve that ignored history and the original Git review/fix ledger; the whitelist remains limited to its explicitly enumerated phase metadata paths.

Retain this clean review's exact `SourceRevisionId` pin and `reviewed-source.targets` import through the mandatory metadata-descendant strict rebuild, focused native retry, fresh `--all`, image review and closure validation. True execution HEAD and reviewed-source stamp remain distinct. The parent owns report/gate/closure commits, shared phase state and publishing.

Phase 2 G1 now has its retained actual passing native results, while the twelve Phase 3 native cases still need a passing repaired execution. Phase 3 foreground physical input, sustained background/window-message feasibility, permitted synthetic fallback provenance, 48 conservative rendered timing rows, 16 stopped comparisons, every required image's appearance review and the entire final repaired selected-set pass remain outstanding. A future input/timing/image failure is an acceptance blocker, not a synthetic or state-only pass. No hosted/Linux, mixed-computer LAN, complete A-38/A-45 campaign, packaging, hardware, endurance or Phase 5 certification is claimed.

---

_Reviewed: 2026-11-16T01:54:34Z_  
_Reviewer: the agent (gsd-code-reviewer)_  
_Depth: standard_
