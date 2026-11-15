---
phase: 03-four-player-lan-foundation
reviewed: 2026-11-15T16:40:08Z
depth: standard
files_reviewed: 89
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
  - src/godot-game/foundation/FoundationAutomation.cs
  - src/godot-game/foundation/FoundationBrowser.cs
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
  - src/godot-game/foundation/FoundationObservationScope.cs
  - src/godot-game/foundation/FoundationPeerBinding.cs
  - src/godot-game/foundation/FoundationPresentation.cs
  - src/godot-game/foundation/FoundationProtocol.cs
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
  - tests/phase1/EngineAccessPolicy.cs
  - tests/phase1/EngineGeneratedTests.cs
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
  - tests/phase1/FoundationPermissionTests.cs
  - tests/phase1/FoundationScenarioClock.cs
  - tests/phase1/FoundationServiceSubstitutionProof.cs
  - tests/phase1/FoundationTestConnection.cs
  - tests/phase1/KingdomZombies.Phase1.Tests.csproj
  - tools/code-rules/BodyLengthAnalyzer.cs
  - tools/code-rules/EngineGeneratedSyntax.cs
  - tools/phase2/check.cjs
  - tools/phase2/rendered.cjs
  - tools/phase3/catalog.cjs
  - tools/phase3/check.cjs
  - tools/phase3/evidence.cjs
  - tools/phase3/inherited.cjs
  - tools/phase3/network.cjs
  - tools/phase3/png.cjs
  - tools/phase3/processes.cjs
  - tools/phase3/rendered.cjs
  - tools/phase3/required-cases.json
  - tools/phase3/runner-fixture.cjs
  - tools/phase3/scenarios.cjs
  - tools/phase3/selection.test.cjs
  - tools/phase3/timing.test.cjs
  - tools/phase3/windows-input.ps1
findings:
  critical: 6
  warning: 4
  info: 0
  total: 10
  necessity: 0
status: issues_found
---

# Phase 03: Code Review Report

**Reviewed:** 2026-11-15T16:40:08Z  
**Depth:** standard  
**Files Reviewed:** 89  
**Status:** issues_found  
**Reviewed revision:** `6dd2419c040aab990ccebac734e3c9a9df6101a6`  
**Diff base:** `87e0b44`

## Summary

Reviewed the complete explicit Phase 3 scope from `.phase3/review-files.txt`, including production C#, native scenes, independent service contracts, tests, exact generated/native/public permissions, the inherited runner adaptations, PowerShell driver and all Phase 3 runner modules. Reviewed the phase plans and acceptance/UI/validation/scenario/selection context against the current Windows 1280×720/no-VM testing policy. The 89-file list takes precedence over individual plan summaries.

There are six BLOCKER findings and four WARNING findings. The principal risks are browser refresh reentrancy, admission denial of service, untrusted actor IDs reaching native node paths, failure of rendered observation on ordinary discovery updates, and acceptance of unsupported convergence or failed-process evidence. The selected inherited navigation and analyzer regression coverage also need repair before rendered testing.

This is a review at the deliberate Plan 4 checkpoint, not a completion or rendered-test certification. No source files, branches or commits were changed. No game window, VM, full test suite or physical input driver was launched. Small Node-only probes corroborated the convergence validator, partial capture publication, unexpected peer-exit handling and concurrent foreground-click race. Those probes are rejection/defect evidence only. Static native call-chain findings still need focused regression proofs after repair.

The generated permission changes were examined as exact policy changes: the current registry contains 220 diagnostic/source/hash/span rows, including the single SCR0006 body-length permission; new native/public inventories remain explicit. This review does not propose any blanket permission refresh or suppression. Necessity was assessed using the required rubric; no necessity finding is asserted.

## Narrative Findings (AI reviewer)

## Critical Issues

### CR-01: Restoring row focus recursively rebuilds the browser

**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/src/godot-game/foundation/FoundationMenu.cs:387-403`  
**Related locations:** `FoundationMenu.cs:449-454`, `FoundationMenu.cs:464-469`  
**Issue:** When an advertisement changes while a session row has keyboard focus, `Rows()` removes the old rows and calls `AddRow()` to restore the focused session. `AddRow()` calls `GrabFocus()`, whose `FocusEntered` handler calls `Detail()`; `Detail()` immediately calls `Rows()` again. The outer call does not assign the new `_rowsKey` until after all `AddRow()` calls finish. The nested call therefore sees the same outstanding change and rebuilds/restores the row again. A normal occupancy change, arriving session or reordered list can cause unbounded reentrancy/stack overflow instead of preserving browser selection.

**Evidence:** The complete production chain is `Rows → AddRow → GrabFocus → FocusEntered → Detail → Rows`. No refresh/reentrancy guard or deferred focus restoration closes that chain. The service-only browser tests never instantiate these native handlers.

**Fix:** Commit the refreshed model/key before emitting focus signals, finish creating the complete row collection before restoring focus, and prevent the focus-detail handler from reentering a rebuild. Keep selection styling/detail refresh separate from row construction. Add a native regression that focuses a row, changes its participant count and inserts/reorders another session, then verifies one rebuild and preserved selected identity.

### CR-02: Draw observation dereferences UI nodes that have never been created

**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/src/godot-game/foundation/FoundationDrawObserver.cs:115-126`  
**Related locations:** `FoundationMenu.cs:103-117`, `FoundationDrawObserver.cs:177`, `LocalProfileService.cs:92-96`  
**Issue:** The draw observer uses the number of service discovery rows to fetch `Browser/Scroll/Rows/SessionN` with mandatory `GetNode<Control>()`. Discovery updates continue while the main menu or game is visible, but `FoundationMenu.Tick()` returns before `Rows()` when the browser is hidden, and also skips row construction during joining. Thus a host's own looped-back advertisement, or another host appearing while a guest is at the menu, can produce discovery rows without corresponding native nodes. Observation then reports missing-node/native errors and fails before writing the current draw/capture. Existing headless proofs cannot expose this because the draw observer is disabled headlessly.

The same observer also assumes `profile` is always an object at line 177. A documented profile-load failure deliberately publishes `Selected = null`; trying to observe that recoverable error instead throws from `GetProperty("PlayerId")`.

**Evidence:** `LanDiscoveryService` enables multicast loopback and `FoundationServices.Update()` publishes rows independently of the visible browser. `FoundationDiagnostics.Observe()` serializes the nullable selected profile. Both states are supported production states, not hypothetical invalid input.

**Fix:** Observe actual existing native row children, with their exact session identity, rather than manufacturing paths from an unrelated service-row count. Permit an absent profile in menu/error observations and retain its failure reason; require a profile only when validating a ready participant. Keep the observer read-only. Add rendered-observer regressions for a hidden browser receiving an advertisement, discovery changes during a pending join, and profile load failure/recovery.

### CR-03: A small number of rejected hellos permanently disables session admission

**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/src/godot-game/foundation/FoundationCoordinator.Admission.cs:84-87`  
**Related locations:** `FoundationCoordinator.Admission.cs:105-108`, `FoundationCoordinator.Admission.cs:144-149`, `FoundationCoordinator.Lifetime.cs:173-185`  
**Issue:** Every Full or duplicate-profile refusal is retained in `_admissions`, and the dictionary is cleared only when the entire host coordinator closes. At 128 rows the host refuses every new attempt, even after all guest slots become free. One unadmitted LAN connection can send distinct attempt IDs against a full host, or repeatedly claim an already-active profile, and consume this global lifetime budget in two bounded receive batches. Disconnecting the attacker does not restore admission. Ordinary repeated unsuccessful joins can reach the same state.

**Evidence:** `Refuse()` unconditionally adds a sender/attempt outcome. The global count check runs before current capacity/identity admission and there is no expiry, per-sender quota, connection cleanup or bounded eviction. This turns a memory bound into persistent application-level denial of service.

**Fix:** Bound and expire refusal/tombstone history without exhausting all future admission. Separate active reservations from replay outcomes, apply per-connection/per-sender limits, and retire closed-connection outcomes using a connection-generation fence. Preserve duplicate-outcome and stale-attempt protection when evicting. Prove that more than 128 rejected attempts cannot prevent a distinct legitimate guest from joining a subsequently free slot.

### CR-04: Opaque wire actor IDs are interpreted as native node paths

**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/src/godot-game/foundation/FoundationView.cs:35-41`  
**Related locations:** `FoundationSnapshot.cs:64-77`, `FoundationProtocol.cs:103-126`, `FoundationCoordinator.GuestPublication.cs:53-57`  
**Issue:** Snapshot validation allows any nonblank actor ID up to 128 characters, including slash, colon and parent-path syntax. The projection concatenates that untrusted value into both a Godot `NodePath` lookup and `Node.Name`. A structurally valid host publication can therefore be accepted atomically and then cause invalid native names, traversal to unrelated nodes, cast failures or repeated duplicate visual construction. For example, a ready remote actor `a` creates `Actor_a`; another ready remote ID `a/../../Menu` resolves `Actor_a/../../Menu` from World to the root Menu control, which cannot be read as `Node2D`.

**Evidence:** Exact uniqueness, ownership, content and numeric checks do not reject or encode these IDs. The unsafe use occurs after the guest has installed the complete candidate, outside the packet-decoding exception boundary.

**Fix:** Keep exact opaque IDs as model/dictionary keys and map them to safe native node names, or encode them into an injective safe representation before any node-name/path operation. Do not normalize the identity itself or let labels/paths replace ownership. Reject or safely project malicious path-shaped IDs before touching unrelated native nodes; add detached-wire and projection controls for slash, colon, parent segments and sanitizer collisions.

### CR-05: Stopped-position evidence is not bound to any actual stopped actor or peer draw

**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/tools/phase3/evidence.cjs:34-40`  
**Related locations:** `evidence.cjs:138-139`, `tools/phase3/rendered.cjs:118-128`  
**Issue:** The stopped matrix checks role labels, then validates only self-declared scalar rows. `actor === expectedActor` and `session === expectedSession` compare fields supplied by that same row. Neither positions nor revision are reconciled with the required observer draw, the host draw, the origin's accepted stop, or the matching release acknowledgment. Changing all comparison rows to an obsolete/nonexistent subject and equal zero positions can retain pass status. Additionally, `STOP_TIME` adds uncertainty before checking nonnegative elapsed time, so an observation before release can be accepted when uncertainty cancels the negative interval.

**Evidence:** A Node-only call to the actual `stopped()` accepted actor `nonexistent-actor`, session `obsolete-session`, revision/minimum 0, release `2000000000`, observed `1000000000`, uncertainty `1000000000`, and both positions zero. Its observation was one second before release with one second of uncertainty. `reconcile()` adds no subject/artifact validation for stopped rows beyond the exact role matrix. The collector writes no draw references for these comparisons.

**Fix:** Record immutable observer/host draw references and frame IDs for every comparison, bind the exact origin actor/session to the accepted zero-direction intent and native release record, and derive positions/revisions from those referenced records. Require observation at or after release before adding conservative uncertainty, and keep the inclusive 50-subunit/500-ms bounds. Add full-reconciler negative controls for wrong actor/session, substituted host/observer positions, stale frame/revision, pre-release observation, and excessive uncertainty.

### CR-06: A required rendered peer can exit nonzero after capture and still pass

**Classification:** BLOCKER  
**File:** `D:/Work/Games/KingdomZombies/tools/phase3/scenarios.cjs:273-280`  
**Related locations:** `tools/phase3/network.cjs:82-85`, `tools/phase3/evidence.cjs:70-97`  
**Issue:** Rendered cleanup skips every peer that has already exited, regardless of whether the termination was expected. It then calls `network.cleanup()`, which accepts any non-null exit record without checking exit code/signal or whether a normal Quit actually completed. Consequently, a peer that silently exits 7 or is killed after its last checkpoint can leave all its rendered scenario results passing. The reconciler checks launch/log/draw artifacts but has no required terminal process record. The separate `processes.cjs` failure controls exercise another command wrapper, so they do not close this actual rendered-peer gap.

**Evidence:** A Node-only call to the actual `network.cleanup()` with an exited required peer `{code: 7, signal: null}` and an empty output log returned successfully. In `scenarios.close()` such a peer is skipped by the ordinary Quit loop. An empty or clean log does not establish a zero process exit.

**Fix:** Persist and reconcile a terminal record for every owned game process. Normal peers must acknowledge ordinary Quit and exit zero without signal within the deadline; cleanup kills and unexpected prior exits must invalidate affected results. Allow deliberate host/guest-loss termination only through an explicit, case-bound expected-termination record declared before the kill. Add actual child-failure controls through the rendered peer lifecycle, including failure immediately after the last capture and during normal Quit.

## Warnings

### WR-01: Capture acknowledgment is published before its JSON is complete

**Classification:** WARNING  
**File:** `D:/Work/Games/KingdomZombies/src/godot-game/foundation/FoundationDrawObserver.cs:168-169`  
**Related location:** `D:/Work/Games/KingdomZombies/tools/phase3/rendered.cjs:64-66`  
**Issue:** The producer writes directly to the final checkpoint JSON path, while the consumer polls for existence every 2 ms and immediately parses it. File creation precedes completion of `WriteAllText()`, so a legitimate capture can fail with partial/empty JSON or a sharing error. This introduces avoidable flakiness across the many required captures. The inherited Phase 2 capture correctly uses a temporary JSON file and move, but the new observer does not.

**Evidence:** A Node-only probe of the actual `r.capture()` published the expected file with `{` and completed the write 100 ms later. Capture immediately failed with `SyntaxError: Expected property name or '}' in JSON`, rather than waiting for publication.

**Fix:** Write the complete JSON to a unique same-directory temporary file, close it, then atomically rename/move it to the final checkpoint path after the PNG is complete. Treat the rename as acknowledgment and retain the conservative external-clock end bracket. Add a delayed-publication control that cannot observe partial JSON.

### WR-02: The selected inherited navigation tests still assume a two-button menu

**Classification:** WARNING  
**File:** `D:/Work/Games/KingdomZombies/tools/phase2/rendered.cjs:151-157`  
**Related locations:** `tools/phase2/rendered.cjs:277-279`, `src/godot-game/foundation/Foundation.tscn:184-185`  
**Issue:** `rendered.menu` and `rendered.quit` expect one Tab from Start to focus Quit; the menu test also expects Down from Start to focus Quit. The Phase 3 scene explicitly changes Start's next/down neighbor to Host and adds Host, Join and profile controls. Thus the still-required inherited cases fail against the intended current UI before checking their meaningful recoverable-entry and clean-Quit obligations. Only profile isolation was adapted in the inherited runner.

**Fix:** Update the inherited procedures and observation fields for the actual current keyboard traversal. Preserve bidirectional navigation, fresh activation, hover/press/error-retry, complete movement regressions and final ordinary keyboard Quit. Navigate to Quit through the declared current controls rather than deleting the assertion, bypassing input, or removing either inherited case from selection.

### WR-03: The saved selection omits regressions for an analyzer changed by this phase

**Classification:** WARNING  
**File:** `D:/Work/Games/KingdomZombies/tools/phase3/catalog.cjs:21-23`  
**Related locations:** `tools/code-rules/BodyLengthAnalyzer.cs:53-54`, `.planning/phases/03-four-player-lan-foundation/03-SELECTION.json`  
**Issue:** The prelaunch validator accepts the saved Phase 1 omissions merely because every ID is accounted for and has some nonblank reason. The actual selection omits `R-37.positive`, `R-37.negative` and all `rules.body*` regressions: 31 body-length-family IDs in total. Their omission reason says there were no analyzer implementation edits, although Phase 3 changes BodyLengthAnalyzer to consult generated permissions and replaces the EngineGeneratedSyntax permission data. These are affected regressions under the current testing policy, not unaffected isolated fixtures. Earlier Plan 1 executions are historical; the saved selection explicitly claims no evidence reuse.

**Fix:** Recompute affected inherited selection from the complete phase diff, including the body-length positive/negative/boundary and generated-configuration controls and affected analyzer source-consumer/boundary checks. Reconcile changed generated-callsite/register inputs with their owning proofs as well. Update scenarios/counts/rationales and require the prelaunch validator to reject the known affected omissions. Keep genuinely unaffected omissions explicit; no complete Phase 5 certification is requested.

### WR-04: Concurrent foreground clicks race for the single Windows foreground window

**Classification:** WARNING  
**File:** `D:/Work/Games/KingdomZombies/tools/phase3/scenarios.cjs:229`  
**Related locations:** `scenarios.cjs:10-13`, `tools/phase3/windows-input.ps1:129-143`  
**Issue:** The last-slot race invokes two `click()` operations concurrently using separate helper processes. Each helper first focuses its target, then sends a foreground physical click in a separate request. Another helper can take foreground between those operations, so the first click fails the helper's correct `DRIVER_MOUSE_FOCUS`/point-ownership check. This tests an uncontrolled desktop-focus race instead of reliably delivering two admission attempts.

**Evidence:** A Node-only probe of the exported actual `scenarios.click()` used two valid control observations and one shared foreground owner, with both focus acknowledgments completing before the click requests. The exact Promise.all arrangement failed with `DRIVER_MOUSE_FOCUS`. No real desktop input was sent.

**Fix:** Keep foreground focus-plus-click operations serialized. For the specifically concurrent contender test, prepare the two ordinary focused Join controls serially and use a controlled, PID-targeted native event dispatch that both helpers can deliver without foreground theft, recording its actual window-message provenance and native handler acknowledgments. Retain separate mandatory genuine foreground physical-input/focus tests. Do not weaken the helper's ownership checks or describe targeted/synthetic delivery as physical input.

---

All findings are open in this review. The orchestrator owns fixes, focused verification, re-review, review-gate creation and the eventual post-review selected rendered pass. This report supplies no rendered, Linux/hosted, mixed-computer LAN, campaign, export, hardware, endurance or Phase 5 certification credit.

_Reviewer: the agent (gsd-code-reviewer)_  
_Depth: standard_

