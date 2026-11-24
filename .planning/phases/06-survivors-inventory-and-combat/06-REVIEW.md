---
phase: 06-survivors-inventory-and-combat
reviewed: 2026-11-24T19:25:19Z
depth: deep
revision: d05115541a751c574b1dde3b70bc42a8a5a347fe
base_revision: b9aaa2b951e6f5dc128a17c7d58e0d17ffe10cb6
files_reviewed: 121
files_reviewed_list:
  - src/godot-game/core/CharacterPresentation.cs
  - src/godot-game/core/FoundationHeldIntent.cs
  - src/godot-game/core/FoundationParticipantState.cs
  - src/godot-game/core/FoundationSession.cs
  - src/godot-game/core/KingdomZombies.Core.csproj
  - src/godot-game/core/RunCombat.cs
  - src/godot-game/core/RunCombatDefinitions.cs
  - src/godot-game/core/RunCombatFixture.cs
  - src/godot-game/core/RunDamage.cs
  - src/godot-game/core/RunDeathAndLoot.cs
  - src/godot-game/core/RunDefinitions.cs
  - src/godot-game/core/RunExternalAccess.cs
  - src/godot-game/core/RunFood.cs
  - src/godot-game/core/RunInventory.cs
  - src/godot-game/core/RunInventoryRules.cs
  - src/godot-game/core/RunItemActions.cs
  - src/godot-game/core/RunItemTransfer.cs
  - src/godot-game/core/RunLocomotion.cs
  - src/godot-game/core/RunPopulation.cs
  - src/godot-game/core/RunProjectiles.cs
  - src/godot-game/core/RunProjections.Hud.cs
  - src/godot-game/core/RunProjections.cs
  - src/godot-game/core/RunQuickReferences.cs
  - src/godot-game/core/RunSavedLayout.cs
  - src/godot-game/core/RunSnapshotCodec.cs
  - src/godot-game/core/RunSnapshotDocument.cs
  - src/godot-game/core/RunStash.cs
  - src/godot-game/core/RunSuccession.cs
  - src/godot-game/core/RunVitals.cs
  - src/godot-game/core/RunWorld.cs
  - src/godot-game/core/RunWorldGeometry.cs
  - src/godot-game/game/content/pack-index.json
  - src/godot-game/game/foundation/FoundationActorArt.cs
  - src/godot-game/game/foundation/FoundationActorNode.cs
  - src/godot-game/game/foundation/FoundationAppearance.cs
  - src/godot-game/game/foundation/FoundationAppearanceAssets.cs
  - src/godot-game/game/foundation/FoundationAutomation.cs
  - src/godot-game/game/foundation/FoundationCoordinator.Admission.cs
  - src/godot-game/game/foundation/FoundationCoordinator.GuestPublication.cs
  - src/godot-game/game/foundation/FoundationCoordinator.Lifetime.cs
  - src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs
  - src/godot-game/game/foundation/FoundationCoordinator.cs
  - src/godot-game/game/foundation/FoundationDiagnostics.cs
  - src/godot-game/game/foundation/FoundationDrawObserver.cs
  - src/godot-game/game/foundation/FoundationEnvelope.cs
  - src/godot-game/game/foundation/FoundationEquipmentProjection.cs
  - src/godot-game/game/foundation/FoundationHud.Live.cs
  - src/godot-game/game/foundation/FoundationHud.cs
  - src/godot-game/game/foundation/FoundationHudState.cs
  - src/godot-game/game/foundation/FoundationInput.cs
  - src/godot-game/game/foundation/FoundationInventoryArt.cs
  - src/godot-game/game/foundation/FoundationInventoryGrid.cs
  - src/godot-game/game/foundation/FoundationInventoryInteraction.cs
  - src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs
  - src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs
  - src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs
  - src/godot-game/game/foundation/FoundationInventoryWorkspace.cs
  - src/godot-game/game/foundation/FoundationMenu.cs
  - src/godot-game/game/foundation/FoundationPeerBinding.cs
  - src/godot-game/game/foundation/FoundationProtocol.cs
  - src/godot-game/game/foundation/FoundationRoot.cs
  - src/godot-game/game/foundation/FoundationRunFrame.cs
  - src/godot-game/game/foundation/FoundationRunReadback.cs
  - src/godot-game/game/foundation/FoundationRunView.cs
  - src/godot-game/game/foundation/FoundationSpectator.cs
  - src/godot-game/game/foundation/FoundationSpectatorPanel.cs
  - src/godot-game/game/foundation/FoundationView.cs
  - src/godot-game/game/foundation/FoundationViewInbox.cs
  - src/godot-game/game/foundation/FoundationWorldInput.cs
  - src/godot-game/game/inspection/CharacterPreview.cs
  - src/godot-game/game/inspection/CorpsePreview.cs
  - src/godot-game/tests/scenarios/phase06-headless.cjs
  - src/godot-game/tests/scenarios/phase06-inspection.cjs
  - src/godot-game/tests/scenarios/phase06-lan.cjs
  - src/godot-game/tests/scenarios/phase06-packaged.cjs
  - src/godot-game/tests/scenarios/phase06-rendered.cjs
  - src/godot-game/tests/scenarios/phase06-solo.cjs
  - src/godot-game/tests/verification/BuildFixture.cs
  - src/godot-game/tests/verification/ContentNativePermissionTests.cs
  - src/godot-game/tests/verification/EngineAccessPolicy.cs
  - src/godot-game/tests/verification/FoundationHarnessConnection.cs
  - src/godot-game/tests/verification/FoundationLanAuthorityTests.cs
  - src/godot-game/tests/verification/FoundationLanInputTests.cs
  - src/godot-game/tests/verification/FoundationLanLifecycleTests.cs
  - src/godot-game/tests/verification/FoundationLanOrderingTests.cs
  - src/godot-game/tests/verification/FoundationLanPermissionTests.cs
  - src/godot-game/tests/verification/FoundationLanProtocolTests.cs
  - src/godot-game/tests/verification/FoundationLinkedSources.cs
  - src/godot-game/tests/verification/FoundationServiceSubstitutionProof.cs
  - src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj
  - src/godot-game/tests/verification/Phase06AccessTests.cs
  - src/godot-game/tests/verification/Phase06AppearanceTests.cs
  - src/godot-game/tests/verification/Phase06BowTests.cs
  - src/godot-game/tests/verification/Phase06DeathTests.cs
  - src/godot-game/tests/verification/Phase06ExternalInteractionTests.cs
  - src/godot-game/tests/verification/Phase06FixtureTests.cs
  - src/godot-game/tests/verification/Phase06InventoryInteractionTests.cs
  - src/godot-game/tests/verification/Phase06InventoryTests.cs
  - src/godot-game/tests/verification/Phase06LoadTests.cs
  - src/godot-game/tests/verification/Phase06MeleeTests.cs
  - src/godot-game/tests/verification/Phase06MovementTests.cs
  - src/godot-game/tests/verification/Phase06NestingTests.cs
  - src/godot-game/tests/verification/Phase06PopulationTests.cs
  - src/godot-game/tests/verification/Phase06PublicationTests.cs
  - src/godot-game/tests/verification/Phase06RecoveryTests.cs
  - src/godot-game/tests/verification/Phase06SnapshotTests.cs
  - src/godot-game/tests/verification/Phase06StashTests.cs
  - src/godot-game/tests/verification/Phase06SuccessionTests.cs
  - src/godot-game/tests/verification/Phase06UiStateTests.cs
  - src/godot-game/tests/verification/Phase06WorldTests.cs
  - src/godot-game/tools/asset-pipeline/content.cjs
  - src/godot-game/tools/asset-pipeline/export.cjs
  - src/godot-game/tools/asset-pipeline/scenarios.cjs
  - src/godot-game/tools/game-tests/basic.cjs
  - src/godot-game/tools/game-tests/catalog.cjs
  - src/godot-game/tools/game-tests/catalog.json
  - src/godot-game/tools/game-tests/catalog.test.cjs
  - src/godot-game/tools/game-tests/headless.cjs
  - src/godot-game/tools/lan-checks/network.cjs
  - src/godot-game/tools/lan-checks/rendered.cjs
  - src/godot-game/tools/lan-checks/windows-input.ps1
findings:
  critical: 7
  warning: 4
  info: 0
  total: 11
  necessity: 0
status: issues_found
---

# Phase 06 Code Review

**Reviewed revision** `d05115541a751c574b1dde3b70bc42a8a5a347fe`

**Target revision** `b9aaa2b951e6f5dc128a17c7d58e0d17ffe10cb6` (`origin/dev`)

**Depth** Deep

**Status** Issues found. This revision has unresolved first-render blockers.

## Summary

The review compared the complete changed source and verification scope against the target revision and traced the boundaries between run authority, inventory and transport, native input and appearance, snapshots, and the verification drivers. The normal start kit, production transactions, scoped fixture, and local Windows package have supporting source or no-window evidence. The public LAN feed, passive nearby discovery, snapshot action continuity, projectile timing and test selection have concrete defects. A required rendered movement case also contains an impossible predicate.

## Narrative Findings (AI reviewer)

### Critical Issues

#### CR-01 — Oversized public state freezes guest world updates

**Severity** BLOCKER · P1

**File** `src/godot-game/game/foundation/FoundationCoordinator.cs:390-402`

A valid public world that exceeds the application byte budget is sent as a roster-only state. The public projection includes all current actors, corpses, loot anchors and arrows (`core/RunProjections.cs:48-74`). Explicit drops allocate a pile at each distinct ground position and nonempty piles remain indefinitely (`core/RunDeathAndLoot.cs:46-49,66-80,121-137`). Neither the projection nor pile lifetime limits the accumulated public state to one frame. The available starting quantities can be split and dropped into separate piles. Larger configurations accepted by the same authority make the boundary easier to reach.

The guest accepts the new roster revision, sets its world to null and skips the public control observation (`FoundationCoordinator.GuestPublication.cs:85-93`). Existing appearance keeps the last frame and stops following committed world changes (`FoundationAppearance.cs:43-59`). A fresh peer has no authored world projection, while an existing peer can see old positions, corpses, arrow sprites and equipment indefinitely. Successor presentation requires the current survivor's public pose. Reliable private status and scope revocation can still arrive, so those paths cannot supply the missing world state. The 06-09 summary records the fallback. AC-25, AC-28 and AC-33 still require a live guest world.

**Fix** Transmit byte-bounded public parts or another complete bounded world representation and install the world and matching roster coherently. Preserve the last complete public state while a newer publication is incomplete, and fence control changes independently from oversized world data. Exercise actual encoded guest state with a legal public world over the frame cap and prove both current world convergence and private isolation.

#### CR-02 — Guest nearby inventories remain stale after crossing range

**Severity** BLOCKER · P1

**File** `src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs:427-444`

The host includes the `own` view only when the control epoch, selected target token or inventory identity changes, or a correction forces a refresh. The fence has no nearby-set input (`core/FoundationSession.cs:80-89`), and Nearby and Availability appear only in the own header (`core/RunProjections.cs:303-325`). After a fresh guest walks into an unselected stash, NPC or loot target's range, ordinary movement leaves all three values unchanged. The periodic status publications contain no new selection list. The production selector consumes only the cached own header and refuses to select a target absent from it (`FoundationInventoryWorkspace.External.cs:62-105`). A legal interaction can stay unavailable until an unrelated inventory action refreshes the projection. The commit path still validates range, so a stale list does not grant remote inventory mutation.

**Fix** Publish scoped nearby metadata when authoritative reachability changes, including the empty-to-nonempty and nonempty-to-empty transitions without a selection. Keep unrelated private rows independently fenced. Add a guest scenario that approaches and leaves a target before selecting anything, observing both the visible list and the permission result. AC-01, AC-15, AC-33.

#### CR-03 — A successful sprint always times out the required rendered case

**Severity** BLOCKER · P1

**File** `src/godot-game/tests/scenarios/phase06-solo.cjs:49-53`

The movement driver waits for `Movement === 'sprint'`. The sprint transition sets `MovementPose = "faster"` (`core/RunLocomotion.cs:93-101`), the public projection sends that pose unchanged (`core/RunProjections.cs:248-249`), and appearance accepts `idle`, `walk`, `faster` and `jump` (`game/foundation/FoundationEquipmentProjection.cs:91`). A healthy production sprint cannot meet the saved predicate, so its screenshot and remaining movement steps cannot pass. AC-05, AC-37.

**Fix** Match the production pose and also assert motion over an elapsed interval and a committed stamina change. Exercise the predicate against real-shaped pre-render state so an invalid pose cannot recur.

#### CR-04 — Run snapshot drops active motion and combat cue state

**Severity** BLOCKER · P2

**File** `src/godot-game/core/RunSnapshotCodec.cs:846-884`

The survivor snapshot captures jump, rest and cooldown timestamps but leaves out `MovementPose`, `MovementAt`, `StrikeAt`, `StrikeAim` and `FlinchAt` from `RunPopulation.Survivor` (lines 176–180). The public view uses those fields directly (`RunProjections.cs:239-254`). Capturing during a walk/sprint, accepted bow or melee action, or live flinch and then restoring to the same authoritative elapsed value changes what the world publishes immediately. The cooldown and arrow can survive while the associated action pose and aim disappear. AC-30 explicitly requires motion/action state continuity.

**Fix** Encode and validate the missing active state or derive equivalent values from complete saved authoritative facts. Compare pre-capture and immediate restored projections and confirm that restoring cues never repeats damage, ammo spending or other accepted actions.

#### CR-05 — Projectile hits change with elapsed-step grouping during jumps

**Severity** BLOCKER · P1

**File** `src/godot-game/core/RunProjectiles.cs:125-138`

The survivor collision code linearly interpolates between the two positions surrounding an entire authority step. Locomotion uses a parabola during jumps and can switch from air motion to ground motion within that same step (`RunLocomotion.cs:48-76`). For a step spanning a full jump, the survivor begins and ends at ground height. A near-ground arrow crossing the survivor at the jump apex can hit that interpolated collider. Dividing the same elapsed interval around the apex produces airborne endpoints and misses. The normal native update passes its elapsed interval directly through the coordinator to `FoundationSession.Step`, which advances the whole population before tracing arrows once (`FoundationSession.cs:250-266`, `game/foundation/FoundationRoot.cs:129-143`). Hit and death results can change with frame timing. AC-05, AC-22, AC-39.

**Fix** Sweep against the authoritative movement trajectory or use a deterministic simulation partition independent of render frames, including jump and landing transitions. Compare hit identity, timestamp, health and spent-arrow outcomes for equivalent whole and partitioned intervals with airborne and landing targets.

#### CR-06 — Phase-end impact selection skips affected native appearance cases

**Severity** BLOCKER · P2

**File** `src/godot-game/tools/game-tests/catalog.cjs:15-25,240-258`

The affected source appearance cases lack input declarations for the Core and Godot code they execute. `animation.bow` uses `bow-work.cjs` and the `animation.cjs` source set and native harness. `appearance.corpses` directly loads and exercises `CorpsePreview` and its character composition. This revision changes `CharacterPresentation.cs`, `CharacterPreview.cs` and `CorpsePreview.cs`. The case rows are active-source and their asset-source/group tags do not match a changed inspection or Core path. Catalog tests cover the 18 relocated rendered cases and distinguish the exact support-stamp edit but never exercise these native source dependencies. The exact branch-versus-target selection omits both cases. The focused runs retained in the 06-10 summary identify an earlier revision. AC-36 and the current local testing policy require affected active rows regardless of ownership or importance.

**Fix** Declare the actual live inputs for the affected native cases and inspect adjacent animation and corpse consumers for the same dependency gap. Prefer exact input edges to broad asset tags. Add a selector regression covering both omissions, keep the original owners, and run the selected checks on the repaired candidate.

#### CR-07 — A valid combat interval can fail production scene presentation

**Severity** BLOCKER · P2

**File** `src/godot-game/core/RunDefinitions.cs:451-456`

The typed combat loader accepts an attack interval up to 600 seconds through `Milliseconds` (lines 253–255). `CharacterPresentation` rejects the same authored attack above 60 seconds (`CharacterPresentation.cs:41-45`). Production uses both consumers on the same `DT-05` bytes (`game/foundation/FoundationAppearanceAssets.cs:49-55`). A present, syntactically valid 61-second interval can enter authority and then fail character construction. This is a disagreement inside Phase 06's authored configuration path.

**Fix** Unify the supported range in the two consumers or make the earlier typed load reject unsupported presentation data with the ordinary startup diagnostic. Test both sides of the shared upper bound through normal scene entry.

### Warnings

#### WR-01 — A fired bow never enters the authored release and recovery presentation

**Severity** WARNING · P2

**File** `src/godot-game/game/foundation/FoundationActorArt.cs:108-135`

Accepted shots have an authoritative strike timestamp and aim. On a fresh shot this consumer resets the visual action, sets the presentation arrow count to zero while the strike aim is present, and advances the track. There is no production call to `CharacterPresentation.TryRelease` or equivalent transition. `BowStage` returns the unloaded aim branch when the bow has no visual arrow and no release flag (`core/CharacterPresentation.cs:525-539`). The projectile still flies and inventory spends the arrow, but loaded-to-fired art switches directly to the same visual state as an empty held aim. The saved combat row checks expenditure and crosshair but would also pass with this missing firing cue.

**Fix** Drive the non-authoritative release/recovery animation from the accepted strike identity once without spending inventory again. Prove the release cue on the first shot and the quiet unchanged unloaded aim on the subsequent empty press.

#### WR-02 — Rendered succession predicates do not prove a camera hold

**Severity** WARNING · P2

**File** `src/godot-game/tests/scenarios/phase06-lan.cjs:114-137`

The saved case requires the death and follow-loss camera holds. The predicates only require a finite camera value and no followed teammate. The follow checks compare the teammate identity with no deliberate movement separating the views. A camera snap at loss, drift after loss, or a followed camera that never moves with its subject can meet those predicates. The pure camera tests check model transitions, which leaves the actual native camera path unproved.

**Fix** Separate camera poses through actual teammate movement and compare the current draw camera before and after death/follow loss over distinct committed draws. Check that the selected teammate subsequently moves the camera. Exercise explicit near-smooth and distant-cut handover observations where required by the approved UI spec.

#### WR-03 — Package gameplay marker does not identify exact loaded artwork

**Severity** WARNING · P2

**File** `src/godot-game/tests/scenarios/phase06-packaged.cjs:46-67,253-257`

The marker checks eight child nodes and three nonempty icon rectangles from one atlas path, plus a crown atlas path. It does not verify any expected body texture or exact icon/crown rectangles. A wrong valid crop in the correct atlas can pass this probe, as can a body node lacking its intended texture. The manifest and PCK byte comparisons establish that the shipped resources are present. They do not establish which exact regions the native consumer selected.

**Fix** Assert each applicable composed node's expected pack identity and atlas rectangle against independently read authored definitions. Add a negative control for a wrong but otherwise valid rectangle or identity. Label the release-versus-companion observations distinctly.

#### WR-04 — Game target selection supplies a marker without the required outline

**Severity** WARNING · P2

**File** `src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs:19-45`

The target update creates and moves the gold `NearbySelection` polygon. The appearance projection has no selected-subject input and the old roster outline is unrelated to nearby selection. This implements the selection marker portion of the UI contract. The steady target-local outline requested by the UI spec at line 124 is absent when the player picks an overlapping NPC, corpse or pile.

**Fix** Project a steady outline onto the same currently permitted subject as the marker, and clear both on deselection, lost access or loss of control. Capture the production selection design on overlapping targets.

## Verification and evidence limits

- Independently ran `node --test --test-isolation=none src/godot-game/tools/game-tests/catalog.test.cjs` with 15 passes and zero failures or skips. The initial default isolation attempt encountered sandbox `spawn EPERM`. The regular exact selector's Git child was likewise denied in this review session. The two concrete missing input edges were verified against the manifest and harness sources, and the orchestrator independently reported those same omissions from the exact selector.
- `git diff --check origin/dev HEAD` reported eight new blank lines at EOF in the phase documents/scenario drivers. These were outside the actionable findings.
- Inspected the existing exact-revision Windows shared PR report `537f7c8a-bdc2-4324-bdb8-c75f8d6e1cc6` with four passes, including strict build, repository checks, 191 pure tests with zero failures or skips, and the PR no-window solo smoke. Its successful pure and repository proof does not cover the missing public/nearby scenarios. Inspected the 06-13 edge census and phase summaries as supporting historical evidence. Those summaries do not replace current source review or a newly selected result.
- Inspected exact-revision package invocation `7b3faf5f-5aca-43d4-9e35-d510a26e91a3` and inner proof `8b0a990d-0f4e-434a-9c42-7a53df42540b`. The clean Windows package case passed. Untouched release PCK `1e3c1868e770ea9e1fc4e0ef1b7dd4f0334ba71f723e88491c80cc79a0571513` booted the Foundation menu in PID 7328. Partner package PID 4900 exercised production Start, movement, starting inventory theme/icons/crown and ordinary Quit. Its differing members were `project.binary`, the UID cache and added probe resources. Managed/runtime members and gameplay content matched the release build. The missing and duplicate-key configuration variants, PIDs 7352 and 38204, produced the `bytes` and `id` diagnostics. This is a headless companion gameplay observation and an unmodified release boot.
- The source review traced immutable inventory candidates, stable path stamps, access and control checks at commit, recipient-only view publication, combat spending and corpse retention, snapshot validation before replacement, guarded engine integration, debug fixture ownership, and scenario transport and screenshot checks. Positive atomicity and privacy evidence exists for named tested paths. The exact public oversized case, passive nearby change, animated active snapshot restore and nonlinear collision cross-checks are missing.
- The 18 handoff IDs match the Phase 06 ownership list and have driver recipes. This review did not launch any rendered game or inspection window. Current production visual judgments, actual four-role rendered input and images, all inspection checkpoints, human qualitative review, hosted/platform acceptance and the final complete phase-end result remain open.

_Reviewer: independent GSD code reviewer_
