---
phase: 06-survivors-inventory-and-combat
reviewed: 2026-11-27T01:55:15Z
depth: deep
revision: 860bb9348054077ac7a647cfb5661ed71046dbbf
base_revision: a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d
files_reviewed: 73
files_reviewed_list:
  - docs/code/engine-generated-spans.json
  - docs/code/generated-callsite-adaptations.json
  - docs/content/fields.md
  - src/godot-game/core/CharacterPresentation.cs
  - src/godot-game/core/KingdomZombies.Core.csproj
  - src/godot-game/game/content/fixtures/crowds.json
  - src/godot-game/game/content/pack-index.json
  - src/godot-game/game/content/packs/compatibility.json
  - src/godot-game/game/content/packs/delivery.json
  - src/godot-game/game/content/packs/player-identities.json
  - src/godot-game/game/content/packs/text-data.json
  - src/godot-game/game/content/packs/ui-camp.json
  - src/godot-game/game/content/packs/ui-identity.json
  - src/godot-game/game/content/packs/ui-inventory.json
  - src/godot-game/game/content/packs/ui-legacy-stash.json
  - src/godot-game/game/content/packs/ui-map-warnings.json
  - src/godot-game/game/foundation/FoundationCoordinator.GuestPublication.cs
  - src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs
  - src/godot-game/game/foundation/FoundationCoordinator.cs
  - src/godot-game/game/foundation/FoundationDiagnostics.cs
  - src/godot-game/game/foundation/FoundationDrawObserver.cs
  - src/godot-game/game/foundation/FoundationHud.Live.cs
  - src/godot-game/game/foundation/FoundationInventoryGrid.cs
  - src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs
  - src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs
  - src/godot-game/game/foundation/FoundationInventoryWorkspace.cs
  - src/godot-game/game/foundation/FoundationRoot.cs
  - src/godot-game/tests/scenarios/phase06-drivers.test.cjs
  - src/godot-game/tests/scenarios/phase06-headless.cjs
  - src/godot-game/tests/scenarios/phase06-inspection.cjs
  - src/godot-game/tests/scenarios/phase06-lan.cjs
  - src/godot-game/tests/scenarios/phase06-packaged.cjs
  - src/godot-game/tests/scenarios/phase06-rendered.cjs
  - src/godot-game/tests/scenarios/phase06-solo.cjs
  - src/godot-game/tests/verification/CharacterBowTests.cs
  - src/godot-game/tests/verification/CharacterPresentationTests.cs
  - src/godot-game/tests/verification/ConsumerCompilation.cs
  - src/godot-game/tests/verification/CurrentReplayTests.cs
  - src/godot-game/tests/verification/EngineAccessPolicy.cs
  - src/godot-game/tests/verification/EngineGeneratedTests.cs
  - src/godot-game/tests/verification/ExemptionTests.cs
  - src/godot-game/tests/verification/ExemptionValidator.cs
  - src/godot-game/tests/verification/FoundationLanInputTests.cs
  - src/godot-game/tests/verification/FoundationLanLifecycleTests.cs
  - src/godot-game/tests/verification/FoundationLanPermissionTests.cs
  - src/godot-game/tests/verification/FoundationLinkedSources.cs
  - src/godot-game/tests/verification/FoundationPermissionTests.cs
  - src/godot-game/tests/verification/Phase06BowTests.cs
  - src/godot-game/tests/verification/Phase06ExternalInteractionTests.cs
  - src/godot-game/tests/verification/Phase06FixtureTests.cs
  - src/godot-game/tests/verification/Phase06InventoryInteractionTests.cs
  - src/godot-game/tests/verification/Phase06PopulationTests.cs
  - src/godot-game/tests/verification/Phase06PublicationTests.cs
  - src/godot-game/tests/verification/Phase06SnapshotTests.cs
  - src/godot-game/tests/verification/Phase06StashTests.cs
  - src/godot-game/tests/verification/Phase06UiStateTests.cs
  - src/godot-game/tests/verification/RestoreMutationProof.cs
  - src/godot-game/tools/README.md
  - src/godot-game/tools/asset-pipeline/cases/census.cjs
  - src/godot-game/tools/asset-pipeline/content.cjs
  - src/godot-game/tools/asset-pipeline/required-cases.json
  - src/godot-game/tools/asset-pipeline/scenarios.cjs
  - src/godot-game/tools/game-tests/runner.test.cjs
  - src/godot-game/tools/game-tests/scenario-group.cjs
  - src/godot-game/tools/game-tests/suites.cjs
  - src/godot-game/tools/gameplay-checks/check.cjs
  - src/godot-game/tools/gameplay-checks/rendered.cjs
  - src/godot-game/tools/gameplay-checks/required-cases.json
  - src/godot-game/tools/lan-checks/scenarios.cjs
  - src/godot-game/tools/verification/ac08-bindings.cjs
  - src/godot-game/tools/verification/bindings.test.cjs
  - src/godot-game/tools/verification/current-replay.cjs
  - src/godot-game/tools/verification/required-cases.json
findings:
  critical: 0
  warning: 0
  info: 2
  total: 2
  necessity: 0
status: issues_found
rereview_files_reviewed: 28
rereview_delta: 25470d0..860bb93
prior_review:
  reviewed: 2026-11-24T21:42:39Z
  depth: deep
  revision: 17fb851c2bfd81ba050fa094527ef21d7707690c
  initial_review_revision: d05115541a751c574b1dde3b70bc42a8a5a347fe
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
    critical: 0
    warning: 0
    info: 0
    total: 0
    necessity: 0
  status: clean
---

# Phase 06 Code Review

Latest review: see [Gap-repair re-review (2026-11-27)](#gap-repair-re-review-2026-11-27) at the end of this file.

**Reviewed revision** `17fb851c2bfd81ba050fa094527ef21d7707690c`

**Target revision** `b9aaa2b951e6f5dc128a17c7d58e0d17ffe10cb6` (`origin/dev`)

**Depth** Deep

**Status** Clean independent source review. The phase still requires rendered evidence.

## Final disposition

The independent reviewer cleared every original blocker and warning on the final source revision. The review covers the cumulative Phase 6 changes against `origin/dev`, with the original 121-file review inventory retained above and subsequent repair revisions listed below.

| Reviewed source | Result |
| --- | --- |
| `d051155` | Seven blockers and four warnings in the initial whole-phase review. |
| `176e1298` | The first repair kept post-impact recovery ahead of early hits and could hide every nearby target in a sufficiently crowded run. |
| `8c948bb` | Impact chronology and nearby target paging fixed. Oversized loose loot and stash-effect messages could still revoke valid recipient views. Open bag capacity needed explicit handling. |
| `5b41a04` | Bounded loose-item pages, exact stash changed-cell encoding, maximum-size escaped target IDs and explicit window capacity verified. An off-page authorized bag window still closed. |
| `8d0cc81` | A selected bag and its descendants survived page navigation and closed on real transfer. A later review reproduced a gap on an unrelated multipart inventory refresh and a focus change to the wrong bag. |
| `f642777` | Clean game code. The host retains the previous coherent view when its new capture confirms the same recipient scope. A changed token, container, target, ground placement or control epoch revokes the affected view. A delayed revoked part cannot reopen it. The workspace restores focus to the exact control path. |
| `17fb851` | Clean. The saved LAN scenario waits for the first equipment-window response and confirms paging preserves the exact owned rows and revision. This source change is confined to the scenario driver. |

The original public view, movement predicate, saved cues, native appearance selection, combat range, bow release, camera evidence, exact package crops and nearby outline findings remain closed. The inventory refresh and focus repair changed six files. The reviewer checked the host/guest publication boundary, the exact focus path and the focused native and wire receipts. The staged RED receipts reproduced both final findings. The staged GREEN `ffdd9c6d-48c4-4c59-873d-4c131f0b53b3` passed 204 pure tests and the native host-plus-three-guest case. The earlier clean asset replay `1dc1a810-ed21-4906-af64-6c8166508e73` passed all four repaired rows on `8d0cc81`, including all 180 source obligations across 19 native cohorts. Rendered outcomes belong to the phase evidence.

## Original review record

The original observations below refer to `d05115541a751c574b1dde3b70bc42a8a5a347fe`. Their closed dispositions are recorded above.

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

## Gap-repair review (2026-11-26)

**Reviewed revision** `0e8b8596307fe909f2851c3af4dc679409fad4d8` (clean tree)

**Base revision** `a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d` (run-1 revision; the workflow diff base `70723ba` differs from it only in `.planning/`)

**Depth** Deep. The reviewer followed the changed C# to its call sites (`FoundationRoot` input and world projection, coordinator receipts, HUD, workspace and draw observer) and the changed tools to the runners and catalogs that load them (`game-tests/suites.cjs`, `scenario-group.cjs`, `gameplay-checks/check.cjs`, `lan-checks/scenarios.cjs`, `lan-checks/network.cjs`, `lan-checks/rendered.cjs`, `verification/check.cjs` caller stages and `ExemptionTests` theory rows).

**Status** Issues found: 0 P0/P1, 2 P2, 7 P3. All findings are open for the orchestrator to route to fixes. Summaries were treated as narration only.

**Reviewer** gsd-code-reviewer, independent of the repair executor.

### Files reviewed (73)

The files are exactly `git diff --name-only a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d..0e8b8596307fe909f2851c3af4dc679409fad4d8`, excluding `.planning/`.

- `docs/code/engine-generated-spans.json`
- `docs/code/generated-callsite-adaptations.json`
- `docs/content/fields.md`
- `src/godot-game/core/CharacterPresentation.cs`
- `src/godot-game/core/KingdomZombies.Core.csproj`
- `src/godot-game/game/content/fixtures/crowds.json`
- `src/godot-game/game/content/pack-index.json`
- `src/godot-game/game/content/packs/compatibility.json`
- `src/godot-game/game/content/packs/delivery.json`
- `src/godot-game/game/content/packs/player-identities.json`
- `src/godot-game/game/content/packs/text-data.json`
- `src/godot-game/game/content/packs/ui-camp.json`
- `src/godot-game/game/content/packs/ui-identity.json`
- `src/godot-game/game/content/packs/ui-inventory.json`
- `src/godot-game/game/content/packs/ui-legacy-stash.json`
- `src/godot-game/game/content/packs/ui-map-warnings.json`
- `src/godot-game/game/foundation/FoundationCoordinator.GuestPublication.cs`
- `src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs`
- `src/godot-game/game/foundation/FoundationCoordinator.cs`
- `src/godot-game/game/foundation/FoundationDiagnostics.cs`
- `src/godot-game/game/foundation/FoundationDrawObserver.cs`
- `src/godot-game/game/foundation/FoundationHud.Live.cs`
- `src/godot-game/game/foundation/FoundationInventoryGrid.cs`
- `src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs`
- `src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs`
- `src/godot-game/game/foundation/FoundationInventoryWorkspace.cs`
- `src/godot-game/game/foundation/FoundationRoot.cs`
- `src/godot-game/tests/scenarios/phase06-drivers.test.cjs`
- `src/godot-game/tests/scenarios/phase06-headless.cjs`
- `src/godot-game/tests/scenarios/phase06-inspection.cjs`
- `src/godot-game/tests/scenarios/phase06-lan.cjs`
- `src/godot-game/tests/scenarios/phase06-packaged.cjs`
- `src/godot-game/tests/scenarios/phase06-rendered.cjs`
- `src/godot-game/tests/scenarios/phase06-solo.cjs`
- `src/godot-game/tests/verification/CharacterBowTests.cs`
- `src/godot-game/tests/verification/CharacterPresentationTests.cs`
- `src/godot-game/tests/verification/ConsumerCompilation.cs`
- `src/godot-game/tests/verification/CurrentReplayTests.cs`
- `src/godot-game/tests/verification/EngineAccessPolicy.cs`
- `src/godot-game/tests/verification/EngineGeneratedTests.cs`
- `src/godot-game/tests/verification/ExemptionTests.cs`
- `src/godot-game/tests/verification/ExemptionValidator.cs`
- `src/godot-game/tests/verification/FoundationLanInputTests.cs`
- `src/godot-game/tests/verification/FoundationLanLifecycleTests.cs`
- `src/godot-game/tests/verification/FoundationLanPermissionTests.cs`
- `src/godot-game/tests/verification/FoundationLinkedSources.cs`
- `src/godot-game/tests/verification/FoundationPermissionTests.cs`
- `src/godot-game/tests/verification/Phase06BowTests.cs`
- `src/godot-game/tests/verification/Phase06ExternalInteractionTests.cs`
- `src/godot-game/tests/verification/Phase06FixtureTests.cs`
- `src/godot-game/tests/verification/Phase06InventoryInteractionTests.cs`
- `src/godot-game/tests/verification/Phase06PopulationTests.cs`
- `src/godot-game/tests/verification/Phase06PublicationTests.cs`
- `src/godot-game/tests/verification/Phase06SnapshotTests.cs`
- `src/godot-game/tests/verification/Phase06StashTests.cs`
- `src/godot-game/tests/verification/Phase06UiStateTests.cs`
- `src/godot-game/tests/verification/RestoreMutationProof.cs`
- `src/godot-game/tools/README.md`
- `src/godot-game/tools/asset-pipeline/cases/census.cjs`
- `src/godot-game/tools/asset-pipeline/content.cjs`
- `src/godot-game/tools/asset-pipeline/required-cases.json`
- `src/godot-game/tools/asset-pipeline/scenarios.cjs`
- `src/godot-game/tools/game-tests/runner.test.cjs`
- `src/godot-game/tools/game-tests/scenario-group.cjs`
- `src/godot-game/tools/game-tests/suites.cjs`
- `src/godot-game/tools/gameplay-checks/check.cjs`
- `src/godot-game/tools/gameplay-checks/rendered.cjs`
- `src/godot-game/tools/gameplay-checks/required-cases.json`
- `src/godot-game/tools/lan-checks/scenarios.cjs`
- `src/godot-game/tools/verification/ac08-bindings.cjs`
- `src/godot-game/tools/verification/bindings.test.cjs`
- `src/godot-game/tools/verification/current-replay.cjs`
- `src/godot-game/tools/verification/required-cases.json`

### Results of the eight required checks

| # | Check | Result |
| --- | --- | --- |
| 1 | F1 preparation keeps evidence bound to current source | **Pass, with a stated limit.** `lan-checks/network.cjs:30-48` copies `project.godot`, `assets`, `content`, `foundation`, `inspection`, `proofs` and the built `.godot/mono/temp/bin/Debug` into the run directory. It filters every `.import` file and copies no `.godot/imported` cache, then runs a fresh headless editor import with its own user data and fails on import errors. `gameplay-checks/rendered.cjs` `prepareRenderedProject` requires the copy to be owned by the invocation directory and its two game assemblies to equal the checkout build. `phase06-rendered.cjs` `open` repeats that check for the scenario groups. One copy serves each invocation. In the phase-end path, `game-tests/suites.cjs:224-255` runs `basic.strict-build` first and rebuilds whenever `runtimeBuildIdentity` changes before a tier-2+ group. That binds the copied assemblies to the current source. The copy-versus-checkout comparison on its own proves only the copy step (Question Q5). |
| 2 | Receipt observation does not change input routing | **Pass.** `FoundationRoot._Input` (`FoundationRoot.cs:75-84`) now increments `_inputEvents` and calls `FoundationDiagnostics.RecordInput` before `_inventory.Input`. `RecordInput` only assigns `_lastInput` (`FoundationDiagnostics.cs:28-31`). No branch of `_Input`, `_Process`, the workspace or world input reads either value. The inventory-first order, the popup, world-key, Escape and menu-activation branches and every `SetInputAsHandled` call are unchanged. The four consumers of the counter (`lan-checks/rendered.cjs` `nativeKey`, `lan-checks/evidence.cjs:148`, `game-tests/headless.cjs:264` and `gameplay-checks/rendered.cjs:285`) compare monotonic increases or the same counter's acknowledgement, so counting workspace-consumed keys keeps them valid. |
| 3 | F13 receipts matched exactly, without retries | **Pass for the reproduced cause, with P3 gaps.** `RunActionFeedback` now carries the action kind on every producer: host dispatch, guest `action-result`, respawn and the DEBUG fixture path. Diagnostics publish `kind` and `replayed`. `phase06-headless.cjs` `receipt()` reads every retained frame since submission and accepts only the first non-replayed receipt of the submitted kind with a newer sequence. It records other receipts as `interleaved` and never resubmits. The drivers test reproduces the 45 ms `place` Applied receipt followed by workspace Ready. Gaps: the match is not bound to the submitted sequence (GR-04), interleaved receipts leak across cases (GR-05), and the frame reader can drop rows (GR-03). |
| 4 | ExportDebug does not leak DEBUG into Release | **Pass.** `KingdomZombies.Core.csproj:11-13` appends `DEBUG` only when `'$(Configuration)' == 'ExportDebug'`. `Debug` gets `DEBUG` from the SDK. `Release` and `ExportRelease` do not. The game project removes `FoundationAutomation.cs` and `FoundationNativeRegression.cs` outside Debug and ExportDebug (`game/KingdomZombies.csproj:14-16`). The gameplay (release) export publishes `ExportRelease` (`asset-pipeline/export.cjs:541`). |
| 5 | Rebound pins derived, not hand-edited | **Pass.** `ac08-bindings.cjs --check` re-derived and matched the ac-08 repository stages, all six callers' hashes, stages and `overallMs`, and the `ExemptionTests` theory rows. `current-replay.cjs --check` re-derived `currentProducerReplay` in `engine-generated-spans.json` exactly. Recomputed SHA-256 values matched every pin in the cascade: `player-identities.json`, `ui-identity.json`, `text-data.json`, `crowds.json`, `fields.md`, the test project and `FoundationRoot.cs` (23601 bytes, `bab43117…5f21`). Their pins appear in `pack-index.json`, `delivery.json`, `compatibility.json`, `crowds.json`, `player-identities.json`, the `ui-*.json` packs, `asset-pipeline/content.cjs`, `generated-callsite-adaptations.json` and `fields.md`. The 34 `moduleSha256` rows in `asset-pipeline/required-cases.json` equal the SHA-256 of `cases/census.cjs`. `asset-bindings.cjs --check` was not run. |
| 6 | Census count matches the reviewed tuples | **Pass.** `ContentNativePermissionTests.cs` has exactly 252 `[InlineData]` rows above `Check_PresenterNative_RequiresExactGuardedMember`, and the file is unchanged in the range. `census.cjs:747-749` requires 252 tests, 252 distinct uids and 252 distinct testUids. The new `CharacterPreview.DrawComposition(JsonElement,JsonElement,JsonElement,int,JsonElement)` tuple is present (line 857) and matches `CharacterPreview.cs:479`. |
| 7 | UI fixes against 06-UI-SPEC, D-08/D-14/D-37 and E068/E126/E128 | **Mostly consistent. GR-07 and GR-08 are open.** The equipment columns sit at x 16 and 208 around the 80 px silhouette at x 120, which matches the UI-SPEC 96/80/8 px allocation. With Liberation Sans advance widths at 16 px, only "Torso clothing" (≈100.5 px) exceeds the 88 px label width. The left column then ends at y 448, above the quick slots moved to y 464, and the two-line label (≈41 px) fits its 46 px region. The world help wraps to four lines (≈87 px) in its 91 px interior. D-08: default focus now lands on the headwear slot, whose button has no `Pressed` action. Space therefore no longer activates *Close inventory*, consistent with UI-SPEC line 132 ("Enter and Space activate the focused control"). The headless case asserts every committed frame stays open and grounded for 250 ms. D-14: the driver's Escape order (drag/split, item menu, nearby list, bag window, workspace, menu) matches `FoundationInventoryWorkspace.Escape()` at `Input.cs:423-453`. D-37/E068/E128: panel-first routing is unchanged. E126: empty equipment slots show an explicit *Empty* marker and tooltip, and empty HUD tiles read "{title} / empty". |
| 8 | New predicates reject unsettled, clipped or single-draw evidence | **Partial. GR-01 and GR-02 are P2, GR-03 is P3.** Every solo picture goes through `shot()` and `settledView`, and the drivers test enforces this. `cameraHold` needs at least two distinct committed draws, a constant camera and a minimum teammate displacement. `respawnGate` checks the exact one-second gate over committed frames. `drag` and `withdraw` frame-separate press, threshold and click. Five LAN picture sites skip `settledView`, and the clipped gate ignores four observed text layers. |

### Findings

#### GR-01 — LAN pictures bypass the settled-view gate

**Severity** WARNING · P2 · **Disposition** open

**File** `src/godot-game/tests/scenarios/phase06-lan.cjs:36-39,123-124,164-167,255-260,267-270`

**Trigger** `allPictures` passes the case condition straight to `run.picture`. The `conflict-original` (each guest's stash plus own inventory), `conflict-revoked`, `conflict-current`, `succession-transfer` and `succession-current` pictures never apply `run.settledView`. Only `population` and five succession pictures use it.

**Consequence** A LAN image can be accepted while carried containers still read "Opening inventory…" or "Opening stash page…", or while HUD or workspace text is clipped. The `conflict-original` pictures show exactly the inventory and stash surfaces where run 1 captured the pending "Opening inventory…" rows (06-RENDERED-EVIDENCE qualitative findings). The 06-24 Task 2 requirement that the drivers reject unsettled evidence is not met for these cases.

**Rule** 06-24-PLAN Task 2 check 8. UI-SPEC lines 57 and 180. `docs/workflow/code-review.md` step 3 (false-positive passes).

**Fix** Apply the settled gate inside `allPictures` and at the two direct `run.picture` calls, for example `(row, draw) => run.settledView(draw) && condition(row, draw, peer)`. Extend the drivers test the way it already asserts solo coverage, so every `run.picture` call in `phase06-lan.cjs` goes through the settled wrapper.

#### GR-02 — The clipped-text gate ignores the world help, roster and spectator layers

**Severity** WARNING · P2 · **Disposition** open

**File** `src/godot-game/tests/scenarios/phase06-rendered.cjs:24,33`. The observer side is `src/godot-game/game/foundation/FoundationDrawObserver.cs:60-64,183-200`.

**Trigger** `textLayers = /^(?:RunHud|InventoryWorkspace)\//`. The observer also reports `clipped` for `Hint`, `Roster/Header`, `Roster/Player1-4`, `Roster/Waiting` and every Label or Button under `SpectatorCard` and `ControlTransferred`. These are visible in the solo world and life pictures and the LAN population and succession pictures. `settledView` never examines them.

**Consequence** A picture with cut-off world help still counts as settled. Run 1 recorded exactly that defect: help text cut off at "Rest is unsafe with an enemy nearby". The same applies to a clipped roster row or a truncated "Control transferred · {id}" notice. The repair moved and restyled the help (`FoundationHud.Live.cs:128-136`), but no objective predicate guards it.

**Rule** 06-24-PLAN Task 2 check 8. UI-SPEC line 57 ("Wrap help, errors and item details on a stable backplate").

**Fix** Gate every visible observed text control, or at least extend the pattern to `^(?:RunHud|InventoryWorkspace|SpectatorCard|ControlTransferred)\/|^(?:Hint|Roster\/[^/]+)$`. Add drivers-test rows for a clipped `Hint` and a clipped `SpectatorCard` label.

#### GR-03 — Frame and draw spans can silently skip rows

**Severity** INFO · P3 · **Disposition** open

**File** `src/godot-game/tests/scenarios/phase06-headless.cjs:178-214,327-342`, `src/godot-game/tests/scenarios/phase06-lan.cjs:111-114,278-285,304-311`. The shared input is `src/godot-game/tools/lan-checks/rendered.cjs:22`, which is unchanged in the range.

**Trigger** `lines()` keeps only the latest 256 rows per stream and splices older rows on each read. `receipt()`/`receiptsAfter()`, `jumpWhileBrowsing()`, `recorder()` (death, transfer and loss spans) and `quick()` read whatever rows are retained. Their comments claim "every frame written since submission" and "every row seen". Headless peers are not frame-capped (`project.godot` sets no `max_fps`), and lan-privacy rows exceed 32 KB. More than 256 rows can therefore pass between two reads, for example during an `inject` acknowledgement or a guest-side wait.

**Consequence** A dropped receipt row turns into a 15 s timeout, which is the F13 symptom shape. A dropped span can let `jumpWhileBrowsing`, `cameraHold` or the `laterReceipts` denial check judge an incomplete interval and pass.

**Rule** 06-24-PLAN Task 2 checks 3 and 8.

**Fix** Fail closed on gaps. Diagnostics write `frame = ++_frame` and the observer writes `drawFrame = ++_frame`, so require consecutive values from the reference row through the last judged row and throw `FRAME_SPAN_GAP` otherwise. Alternatively, read from a recorded byte offset rather than the shared rolling buffer.

#### GR-04 — The receipt match is not bound to the submitted request's sequence

**Severity** INFO · P3 · **Disposition** open

**File** `src/godot-game/tests/scenarios/phase06-headless.cjs:178-199`, as used by `action()`, the inventory and combat receipts and `phase06-lan.cjs:136,147`

**Trigger** A receipt is accepted when `kind` matches, `replayed === false` and its sequence exceeds the last *received* receipt in `before`. It is not compared with the sequence of the request the driver submitted. A same-kind request still in flight from earlier in the case would satisfy the wait.

**Consequence** Today's cases submit distinct kinds or wait for each receipt before the next submission, so this is latent. The check is still weaker than the "exact receipt of each submitted request" the F13 repair claims.

**Rule** 06-24-PLAN Task 2 check 3.

**Fix** Have the DEBUG `phase06-action` handler write the sequence it allocated (`FoundationCoordinator.cs:249` or the guest `SubmitRunAction` result) into `input.ack`, and match `entry.sequence === submitted`.

#### GR-05 — Interleaved receipts from a failed case are attributed to the next case

**Severity** INFO · P3 · **Disposition** open

**File** `src/godot-game/tests/scenarios/phase06-headless.cjs:155-157,165-171,180`

**Trigger** `receipt()` appends to `state.interleaved`, and only `evidence()` drains it. A case that fails before writing its evidence leaves its entries in place. Neither the headless `reset` nor `run.reset` clears them, and the entries carry no case id.

**Consequence** The next passing case's evidence file lists receipts that belong to a different case. That misattributes the evidence the F13 repair added for diagnosis.

**Rule** `docs/workflow/code-review.md` step 5 (accurate evidence). 06-24-PLAN check 3.

**Fix** Clear `state.interleaved` at the start of each case or in both resets, and record the case id with each entry.

#### GR-06 — The contested withdrawal decides "stale" from a draw sampled before the pointer moves

**Severity** INFO · P3 · **Disposition** open

**File** `src/godot-game/tests/scenarios/phase06-lan.cjs:142-151,99-108`

**Trigger** `staleCell` is computed from `before = run.last(second)` before `withdraw()` moves the pointer. If the host's correction reaches guest 2 between that draw and the click, the click lands on an empty cell. No quick request is sent, but `native.receipt(... 'quick' ...)` still waits and fails with `SUBJECT_TIMEOUT contested withdrawal denial`.

**Consequence** The rendered conflicts case can fail with a deadline, which was its run-1 failure shape (F12), even though the game behaved correctly.

**Rule** `docs/workflow/local-testing.md` (no flaky saved scenarios). 06-24-PLAN check 8.

**Fix** Return the `resting` draw from `withdraw()` and decide `staleCell` from it. Alternatively, wait for either a quick receipt newer than `before` or a corrected view with no quick receipt, then branch on which one occurred.

#### GR-07 — World-help visibility has two writers through a re-entrant signal

**Severity** INFO · P3 · **Disposition** open

**File** `src/godot-game/game/foundation/FoundationHud.Live.cs:128-136,255-277`, `src/godot-game/game/foundation/FoundationRoot.cs:415-418`

**Trigger** On every frame `ShowWorld` sets `hint.Visible = CanDrive`. While browsing, the HUD's `VisibilityChanged` handler immediately sets it back to false from inside the engine's visibility notification. `_browsing` is refreshed only in `UpdateActions`, which is skipped when the status view is momentarily absent and `Current` is unchanged (`Live.cs:44-53`).

**Consequence** Two components own one property. The help toggles visible and hidden twice per browsing frame through nested notifications, and a stale `_browsing` can hide the help after the workspace closes. The D-08 intent ("hide help while any inventory surface captures the world") belongs to the single writer.

**Rule** `docs/code/review-checklist.md` (cohesion and ownership). UI-SPEC line 114.

**Fix** Compute the value once in the root: `hint.Visible = _spectator.State.CanDrive && !_inventory.CapturesWorld;`. Then remove `KeepHintClosed`, its subscription and unsubscription, `_browsing`, and its `EngineAccessPolicy` row.

#### GR-08 — Equipment slot buttons lost their text name

**Severity** INFO · P3 · **Disposition** open

**File** `src/godot-game/game/foundation/FoundationInventoryWorkspace.cs:312-330`

**Trigger** The slot label moved from `Button.Text` into a mouse-ignored child `Label`. The button now has no text, and an occupied slot's tooltip carries only item details, not the slot name. The project sets no `AccessibilityName` anywhere.

**Consequence** Focus traversal and assistive technology now meet an unnamed button for each of the eleven own slots and the nearby/corpse slots. UI-SPEC line 132 requires accessible names for icon-only controls.

**Rule** UI-SPEC lines 118 and 132.

**Fix** Give each slot button an explicit name, for example `AccessibilityName = label + (item is null ? " empty" : " · " + itemName)`, or keep the label in `TooltipText` with the item details.

#### GR-09 — New blank line at end of file

**Severity** INFO · P3 · **Disposition** open

**File** `src/godot-game/tests/scenarios/phase06-drivers.test.cjs:715`

**Trigger and consequence** `git diff --check` reports a new blank line at EOF.

**Fix** Remove the trailing blank line.

### Questions (not defect findings)

- **Q1** `FoundationDrawObserver` flags a Button whose full text exceeds its room as clipped, including deliberate `TrimEllipsis` rows such as `NearbyChoice_*`. UI-SPEC line 57 permits ellipsis in compact rows. Conversely, an autowrapping Label that grows past its allotted rect through its minimum size is never flagged, so overlap with neighbouring controls goes undetected. Is that the intended clip semantics?
- **Q2** In `asset-pipeline/scenarios.cjs:996-1001,1018-1025`, if the native side never answers an abandoned request, each later response of the same type is consumed by the older abandoned entry and discarded. Every following request of that type then times out until `RENDERED_NATIVE_BACKLOG`. Can the inspection handler drop a reply? If so, correlate responses with a request id.
- **Q3** The population targets are 1500 subunits apart, but `spread` requires 1000 subunits (16 px). A relative key-release overshoot above 500 subunits, about 167 ms at 3 u/s, would fail the picture. Is that margin enough for guest round trips?
- **Q4** The solo `stash-withdrawal` picture (`phase06-solo.cjs:182`) still matches `runAction.status === 'Applied'` without `kind === 'quick'`. Its state predicates guard it, but the kind is now available.
- **Q5** `prepareRenderedProject` compares the copied assemblies with the checkout build, not with a build of the current source. Direct standalone invocations of `gameplay-checks/check.cjs` or `lan-checks` therefore rely on whatever the checkout last built. The phase-end path through `game-tests` is bound by its strict build.

### Checks actually run

| Command | Result |
| --- | --- |
| `node --test --test-isolation=none src/godot-game/tests/scenarios/phase06-drivers.test.cjs` | 60 pass, 0 fail, 0 skip |
| `node --test --test-isolation=none src/godot-game/tools/verification/bindings.test.cjs` | 21 pass, 0 fail |
| `node --test --test-isolation=none src/godot-game/tools/game-tests/runner.test.cjs` | 9 pass, 0 fail |
| `node src/godot-game/tools/verification/ac08-bindings.cjs --check` | `PASS ac08-bindings 0e8b8596307f rows=612 checked=301 sourceStages=19 callers=6 repositorySha256=ee8b61987aae5b0277f9049b507dc10dd9524a1f755aa78b500e886a8d74230a` |
| `node src/godot-game/tools/verification/current-replay.cjs --check` (read-only, beyond the listed checks) | `PASS current-replay 57 historical, 22 changed, 22 added; sourceInputsSha256 482fcebf635c8103c0582dcd79b33cfb8f095f61f172b475ea98ee2d53f6f8e8` |
| `git diff --check a9ac95f..HEAD -- . ':!.planning'` | One new blank line at EOF (GR-09) |
| `sha256sum` and `wc -c` over the clean tree | Every pin listed in check 5 matches |
| Manual count of `[InlineData]` rows above `Check_PresenterNative_RequiresExactGuardedMember` | 252 |

### Evidence limits

- No Godot process, build, xUnit run, rendered, LAN, inspection or packaged run was launched. C# behavior was judged from source and its callers. The new and renamed xUnit tests were read but not executed.
- `asset-bindings.cjs --check` was not run. The asset cascade was checked by recomputing the individual hashes.
- The text-fit numbers in check 7 are estimates from Liberation Sans advance widths and the theme's line spacing, not rendered measurements. Final layout, overlap and readability judgments belong to the rendered run and the P1–P4 human review.
- The Task 1 no-window results were not re-run here. This review does not replace the phase-end rendered evidence or the human qualitative components.

_Reviewer: gsd-code-reviewer (independent; spawned fresh by the /gsd-code-review workflow; did not author the repairs)_

## Gap-repair re-review (2026-11-27)

**Reviewed revision** `860bb9348054077ac7a647cfb5661ed71046dbbf` (clean tree)

**Fix-delta range** `25470d0..860bb93`. The cumulative base stays `a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d`. `25470d0` differs from the earlier reviewed source `0e8b859` only in `.planning/`.

**Depth** Deep. The reviewer followed every changed symbol to its callers and consumers, including some outside the 28 files:

- the `input.ack` writer and its only reader (`lan-checks/network.cjs`), and every driver that goes through it (`game-tests/headless.cjs`, `phase06-headless/-lan/-packaged`, `lan-checks/scenarios.cjs`)
- the coordinator's host and guest sequence allocation and receipt echo (`FoundationCoordinator.RunWire.cs`, `.GuestPublication.cs`, `.cs`)
- the diagnostics and draw-observer frame counters and the shared 256-row reader (`lan-checks/rendered.cjs` `lines()`)
- the observer's control paths and clip rule, and every `settledView` and `run.picture` caller (solo and LAN)
- the `EngineAccessPolicy` `_presenters` set against `ContentNativePermissionTests` and `asset-pipeline/evidence.cjs`
- the `FoundationLanPermissionTests` native tuple list
- the scenario-group case invocation and the `catalog.test.cjs` key checks
- the GodotSharp 4.7.2 API docs and the banned-symbol lists

**Status** Issues found: 0 P0/P1/P2 and 2 new P3 findings, both open. All nine GR findings are resolved. Deviations D1–D4 are acceptable. Fixer commit messages were read as narration only. Every claim was checked against the current source.

**Reviewer** gsd-code-reviewer (fresh and independent). This reviewer did not author the repairs and did not perform the review of `0e8b859`.

### Files reviewed (28)

These are exactly the files from `git diff --name-only 25470d0..860bb93`, excluding `.planning/`:

- `docs/code/engine-generated-spans.json`
- `docs/content/fields.md`
- `src/godot-game/game/content/fixtures/crowds.json`
- `src/godot-game/game/content/pack-index.json`
- `src/godot-game/game/content/packs/compatibility.json`
- `src/godot-game/game/content/packs/delivery.json`
- `src/godot-game/game/content/packs/player-identities.json`
- `src/godot-game/game/content/packs/text-data.json`
- `src/godot-game/game/content/packs/ui-camp.json`
- `src/godot-game/game/content/packs/ui-identity.json`
- `src/godot-game/game/content/packs/ui-inventory.json`
- `src/godot-game/game/content/packs/ui-legacy-stash.json`
- `src/godot-game/game/content/packs/ui-map-warnings.json`
- `src/godot-game/game/foundation/FoundationAutomation.cs`
- `src/godot-game/game/foundation/FoundationHud.Live.cs`
- `src/godot-game/game/foundation/FoundationInventoryWorkspace.cs`
- `src/godot-game/game/foundation/FoundationRoot.cs`
- `src/godot-game/tests/scenarios/phase06-drivers.test.cjs`
- `src/godot-game/tests/scenarios/phase06-headless.cjs`
- `src/godot-game/tests/scenarios/phase06-lan.cjs`
- `src/godot-game/tests/scenarios/phase06-rendered.cjs`
- `src/godot-game/tests/verification/EngineAccessPolicy.cs`
- `src/godot-game/tests/verification/FoundationLanPermissionTests.cs`
- `src/godot-game/tools/asset-pipeline/content.cjs`
- `src/godot-game/tools/asset-pipeline/required-cases.json`
- `src/godot-game/tools/gameplay-checks/required-cases.json`
- `src/godot-game/tools/lan-checks/network.cjs`
- `src/godot-game/tools/verification/required-cases.json`

### Dispositions of GR-01..GR-09

| ID | Disposition | Fix commit(s) | Evidence at `860bb93` |
| --- | --- | --- | --- |
| GR-01 | **Resolved** | 760e71f | `phase06-lan.cjs:37-43` adds the `shot` wrapper, which calls `run.settledView(draw)` before the case predicate. `allPictures` maps every peer through `shot`. `run.picture(` now appears once, inside the wrapper. `conflict-original` (:131), `conflict-revoked` (:177), `conflict-current` (:179), `succession-transfer` (:267) and `succession-current` (:279) now pass through the gate, along with the five succession pictures that already used it. `phase06-drivers.test.cjs:276-285` pins the single `run.picture` call, the wrapper shape, `allPictures` and the seven direct labels. |
| GR-02 | **Resolved** | f9e61fd | `phase06-rendered.cjs:26` changes `textLayers` to `^(?:(?:RunHud\|InventoryWorkspace\|SpectatorCard\|ControlTransferred)(?:\/\|$)\|Hint$\|Roster\/[^/]+$)`. That pattern covers every run text path the observer emits: the fixed paths `Hint` and `Roster/*` (`FoundationDrawObserver.cs:60-64`) and every Label, Button or LineEdit under the `SpectatorCard` and `ControlTransferred` layers (:183-200). `DamageTint` carries no text. `phase06-drivers.test.cjs:254-268` rejects a clipped `Hint`, `Roster/Header`, `Roster/Player1`, `SpectatorCard/FollowStatus`, `SpectatorCard/Respawn` and `ControlTransferred`, and it accepts the unclipped set. |
| GR-03 | **Resolved** (listed sites). One site of the same class remained outside the reviewed range (NR-01). | 1ed3aa5, af69841 | `phase06-headless.cjs:189-204` adds `contiguous()` and `framesAfter()`, which collect rows across reads and throw `FRAME_SPAN_GAP` on any missing number, including a missing first row. `receipt()` reads through it (:214-218). The food receipt (:349-353) and the empty-ammo receipt (:394-397) are collected from their reference frames, and so is `jumpWhileBrowsing` (:368-376). In LAN, `withdraw` polls the reader at every drawn step (:105-114). The winner and loser receipts and `quick()` use gap-free readers (:143-146, :154-170). The death, loss and transfer spans use `native.contiguous` (:192-196, :205, :263). The counters are per process and consecutive: `frame = ++_frame` (`FoundationDiagnostics.cs:73`) and `_frame++` with an unconditional `Write` (`FoundationDrawObserver.cs:71,259`). They never reset in a scenario. The root scene is never reloaded. `ReturnToMenu`, rejoin and reset keep the process. `network.launch` creates the role directory without `recursive` (`network.cjs:50`), so a restarted process cannot append to an old stream. A gap check therefore cannot fire on a legitimate counter reset. |
| GR-04 | **Resolved**. The physical-input residual is accepted as D1. | 1ed3aa5, bc1fce6, ec42745 | `FoundationAutomation.cs:91-97` writes `"<request>[ <allocated>]\n"`. `Deliver` returns the sequence that `SubmitPhase06` allocated (:162-167, :178-192). The host allocates the sequence and stores the same number in `RunActionFeedback` (`FoundationCoordinator.RunWire.cs:32,52`). The guest echoes the host's `action-result` under the guest's own envelope sequence (`FoundationCoordinator.GuestPublication.cs:146-149`). `network.cjs:83-85` returns that number. `action()` requires a safe integer above the baseline and passes it as `submitted` (`phase06-headless.cjs:252-260`). `matches()` then accepts only `entry.sequence === submitted` (:235-236). `phase06-drivers.test.cjs:406-416` shows that the earlier same-kind receipt is rejected and that an unmatched sequence times out. `FoundationLanPermissionTests.cs:141-147` stubs `long` with `return 0;`. |
| GR-05 | **Resolved** | 1ed3aa5 | `begin()` resets `state.interleaved` and sets `state.case` (`phase06-headless.cjs:167-174`). Every headless and LAN case runs through `tracked()`, at :637 and `phase06-lan.cjs:342`. Both resets clear the list (`phase06-headless.cjs:177`, `phase06-rendered.cjs:219`). Each entry carries `case` (:223), and `evidence()` writes only entries whose `case` equals its id (:156). Every headless and LAN `evidence`/`run.finish` id equals its case key. `catalog.test.cjs:41` still sees the same ordered keys. Solo cases are not wrapped and call no `receipt()`, so they record nothing. `phase06-drivers.test.cjs:447-471` proves the isolation. |
| GR-06 | **Resolved**. A residual of about one frame is accepted as D2. | 1ed3aa5 | `withdraw` returns the `resting` draw, the one drawn with the pointer on the cell (`phase06-lan.cjs:105-115`). `staleCell` is decided from that draw (:158), after the pointer moves, not from `before`. `phase06-drivers.test.cjs:425-430` pins this. |
| GR-07 | **Resolved** | 798ee3e | `FoundationRoot.cs:417` is now the only per-frame writer: `hint.Visible = _spectator.State.CanDrive && !_inventory.CapturesWorld`. `FoundationHud.Live.cs` no longer has `_hint`, `_browsing`, `KeepHintClosed` or any `VisibilityChanged` subscription; `Build` only styles and places the label (:120-127). The other remaining writer is the menu teardown at `FoundationRoot.cs:473`, which hides the hint. The `KeepHintClosed()` row left `EngineAccessPolicy._presenters` (373 → 372 rows). That row was never one of the 252 `ContentNativePermissionTests` `[InlineData]` tuples, so the census and its subset relation still hold. |
| GR-08 | **Resolved** | 27cc231 | `FoundationInventoryWorkspace.cs:316-322` gives every slot button an `AccessibilityName`: "{slot} empty", or "{slot} · {item name}". Occupied slots also put the slot name ahead of the item details in the tooltip (:357). `DrawSlot` serves own, nearby and corpse slots. |
| GR-09 | **Resolved** | cab791f | `phase06-drivers.test.cjs` ends with a single `});\n` (810 lines). `git diff --check 25470d0..HEAD -- . ':!.planning'` prints nothing. |

### Verdicts on fixer deviations

| ID | Verdict | Reason |
| --- | --- | --- |
| D1: exact binding only for injected `phase06-action` | **Acceptable** | A physical Ctrl+click, food key or attack click allocates its sequence inside the game, and no observation exposes that number, so the driver cannot bind it exactly. The remaining match is still strict. `receiptsAfter` needs a frame and a sequence after the baseline (`phase06-headless.cjs:240-249`) and excludes replays. The span is gap-free. Each physical wait follows exactly one request of its kind: one food tap per wait (:352-356), one second shot (:396-397) and one Ctrl+click per peer (`phase06-lan.cjs:144-146,155-160`). A stray same-kind receipt fails the status assertion that follows (`Consumed`, `EmptyAmmo`, `Applied`, or `AccessLost`/`StaleItem`). It cannot produce a false pass. Q1 describes how to close this gap. |
| D2: GR-06 race window of about one frame | **Acceptable residual** | The window now lies between the resting draw and the game's handling of the click. Before the fix it ran from a draw sampled before the focus switch and the pointer move. Guest 2's correction comes from the host publication that the winner has already shown (`phase06-lan.cjs:149-150`), so it normally lands before guest 2 is even focused. If the race does occur, the case fails loudly with `SUBJECT_TIMEOUT contested withdrawal denial`. It does not pass falsely: `laterReceipts` still rejects any `Applied`, and `observedStale \|\| !staleCell` still holds. Q1 names a deterministic alternative. |
| D3: the project's first `AccessibilityName` | **Acceptable** | The project pins `Godot.NET.Sdk/4.7.2` (`game/KingdomZombies.csproj:1`, `project.godot` features `4.7`). GodotSharp 4.7.2 documents `P:Godot.Control.AccessibilityName` as "The human-readable node name that is reported to assistive apps" (`GodotSharp.xml:84165`). `docs/workflow/engine-defects.md` has no accessibility entry. `build/BannedSymbols*.txt` bans no `Control` or accessibility member. `DrawSlot` is already a guarded presenter row (`EngineAccessPolicy.cs:226`), so the property set adds no new engine-access surface. The `_art?.Name(item) ?? item.Definition` fallback matches existing uses (`FoundationInventoryWorkspace.External.cs:389`, `.Input.cs:255,266`). |
| D4: `input.ack` = `"<request>[ <allocated>]\n"` | **Acceptable** | A repository-wide search finds one reader: `network.cjs:83-85`. All drivers reach it through `network.input`/`key`/`activate`, including `game-tests/headless.cjs:9,119,139,196`, `phase06-headless.cjs:33` and `lan-checks/scenarios.cjs` through `net`. Callers other than `action()` ignore the new return value. The writer always ends the line with `\n` (`FoundationAutomation.cs:95-97`), and the reader's regular expression `^(\d+)(?: (\d+))?\n$` requires it, so the reader never accepts a partial write. `WriteAllText` truncates and then writes. A stale earlier line cannot match the new request sequence. `stop()` still treats process exit as the acknowledgement (`network.cjs:104-112`). The DEBUG-only writer is not built into Release or ExportRelease. No evidence validator or copied artifact parses `input.ack`. |

### New findings

#### NR-01: The solo handover spans still read the rolling draw buffer

**Severity** INFO · P3 · **Disposition** open

**File** `src/godot-game/tests/scenarios/phase06-solo.cjs:278-282`, used at `:258` (near handover, `travelled`) and `:291-293` (distant handover, `cut`). This code is pre-existing and unchanged in `25470d0..HEAD`, but it belongs to the class of defect the GR-03 repair claims to close.

**Trigger** `handover()` filters the shared 256-row `draws(peer)` buffer once, after the `life-transfer` or `life-distant-cut` picture returns, from `gate.draw.drawFrame` or `dead.draw.drawFrame`. Nothing polls the stream while the Respawn click, the handover and the picture capture run. Rendered draws follow the monitor refresh rate. At 144–240 Hz, that interval plus the capture can exceed 256 draws. The function has no contiguity check.

**Consequence** The earliest transfer draws can be missing without any error. `farHandover` asserts "distant handover cuts on the first committed draw without an intermediate position" through `cut[0]` and `cut.every(...)`. If the first transfer draws, including any smoothed intermediate one, were dropped, the check passes over the remaining tail. That is the false pass GR-03 described, and `travelled` can likewise be judged over a partial span.

**Rule** 06-24-PLAN Task 2 check 8 (predicates must reject incomplete evidence). This is the same rule as GR-03.

**Fix** Before `run.choose(state, peer, named('Respawn'))`, create `const after = native.contiguous(() => draws(peer), 'drawFrame', previous.drawFrame + 1)`, using the `phase06-headless.cjs` helpers. Poll it in the `life-transfer` and `life-distant-cut` `shot` predicates, as LAN `transferring` does (`phase06-lan.cjs:263-267`). Build `handover` from `after().filter(draw => draw.drawFrame <= last.drawFrame)`. Add a drivers-test source assertion like the LAN one.

#### NR-02: `SubmitPhase06` and `Tick` misdescribe the guest workspace acknowledgement

**Severity** INFO · P3 · **Disposition** open

**File** `src/godot-game/game/foundation/FoundationAutomation.cs:178` (also the `Tick` summary at `:58-61`)

**Trigger** The summary says "otherwise the allocated run-action sequence, or 0 for a host workspace". The `Tick` summary says the acknowledgement names "the run-action sequence a DEBUG phase06 action submitted, if any". On a guest, `RequestRunWorkspace` submits a `workspace` run action and allocates a sequence (`FoundationCoordinator.RunWire.cs:138-142`). `SubmitPhase06` turns the bool into `0` (`:183-185`), so the acknowledgement omits a sequence the game did allocate.

**Consequence** The documented contract differs from the actual behavior. A future driver that relies on it to bind a guest workspace receipt would read `null`. It would then either fall back to kind matching or fail its `Number.isSafeInteger` precondition. No current caller is affected, because `phase06-headless.cjs:339,520` ignores the workspace result.

**Rule** `docs/code/review-checklist.md` (comments and contracts describe actual behavior). `docs/workflow/code-review.md` step 5 (accurate evidence).

**Fix** Either reword both summaries ("0 for any workspace request; its window sequence is not reported"), or have `RequestRunWorkspace` return the guest's allocated sequence as `long?` and pass it through.

### Re-confirmed 06-24 Task 2 checks touched by the delta

| # | Check | Result |
| --- | --- | --- |
| 3 | F13 receipts matched exactly, without retries | **Pass.** Injected actions bind to the exact allocated sequence. Physical inputs bind to the first non-replayed receipt of their kind after the baseline, read over a gap-free span (D1). `receipt()` never resubmits. Interleaved receipts are recorded per case. |
| 5 | Rebound pins derived, not hand-edited | **Pass.** `ac08-bindings.cjs --check`, `current-replay.cjs --check` and `asset-bindings.cjs --check` all pass on HEAD. Recomputed SHA-256 values match every pin of the changed files: `FoundationRoot.cs` `5149af83…0193` (23630 bytes, in `pack-index.json`, `ui-identity.json`, `content.cjs:348`, verification `required-cases.json` and `engine-generated-spans.json`), plus `FoundationAutomation.cs`, `FoundationHud.Live.cs`, `FoundationInventoryWorkspace.cs`, `player-identities.json`, `ui-identity.json`, `text-data.json`, `crowds.json`, `fields.md`, `delivery.json` and `compatibility.json`. No previous hash of the four changed game sources remains anywhere under `src/` or `docs/`. The spans registry SHA-256 `21e51b9b…01b1` appears three times in verification `required-cases.json`. The 34 `moduleSha256` rows still equal `cases/census.cjs`. The `content.cjs:348` literal is a checked constant, enforced by `SUPPORT_SOURCE_IDENTITY` and `asset-bindings --check`. |
| 8 | Predicates reject unsettled, clipped or single-draw evidence | **Pass for the delta, with NR-01 open.** Every solo and LAN picture goes through `settledView`. The clip gate covers every observed run text layer. Headless and LAN spans fail closed on gaps. The solo handover span still reads the rolling buffer (NR-01). |

### Questions (not defect findings)

- **Q1** D1 and D2 could both be closed if diagnostics published the local run-action sequence the game last allocated, per kind, as a read-only observation. A physical Ctrl+click, food key or attack would then bind exactly. The contested withdrawal could also branch on whether guest 2 actually submitted a request instead of inferring it from the resting draw. Is that worth adding?
- **Q2** The guest installs an `action-result` only when its sequence is at least the current feedback's (`FoundationCoordinator.GuestPublication.cs:146-149`). If the result of a later workspace request arrived before a submitted action's result, the exact receipt would never appear, and the driver would time out rather than mismatch. Does the host guarantee that results arrive in submission order on one connection?
- **Q3** GR-08 names the equipment slots. Grid item buttons, quick slots and HUD tiles are also icon-only and still have only tooltips. Does UI-SPEC line 132 require an `AccessibilityName` on them too, or does Godot 4.7's tooltip fallback meet it?
- **Q4** Gap margins: the retained headless streams in `.local/runs/game-tests/069f23d0…` and `045430ad…` show about 115–160 rows/s and no frame gaps. The 256-row buffer therefore holds about 1.6 s headless, or about 1 s at a 240 Hz rendered refresh. The longest unpolled waits in the delta are a few input acknowledgements, so a `FRAME_SPAN_GAP` should not fire spuriously. If one ever does, read from recorded byte offsets instead of enlarging the timeouts.

### Checks actually run

| Command | Result |
| --- | --- |
| `node --test --test-isolation=none src/godot-game/tests/scenarios/phase06-drivers.test.cjs` | 70 pass, 0 fail, 0 skip |
| `node --test --test-isolation=none src/godot-game/tools/verification/bindings.test.cjs` | 21 pass, 0 fail |
| `node --test --test-isolation=none src/godot-game/tools/game-tests/runner.test.cjs` | 9 pass, 0 fail |
| `node src/godot-game/tools/verification/ac08-bindings.cjs --check` | `PASS ac08-bindings 860bb9348054 rows=612 checked=301 sourceStages=19 callers=6 repositorySha256=88a5db9885effea21a17273beff4b69cec535d06577145c2526ba7d63c5c6c29` |
| `node src/godot-game/tools/verification/current-replay.cjs --check` | `PASS current-replay 57 historical, 22 changed, 22 added; sourceInputsSha256 d58431d3d7716635892c30c54af80000f87cd3cb4d2268f57c249e7672e46eab` |
| `node src/godot-game/tools/verification/asset-bindings.cjs --check` | `PASS asset-bindings modules=423 valid=180` |
| `git diff --check 25470d0..HEAD -- . ':!.planning'` | No output |
| SHA-256 and byte counts (Node `crypto`) over the clean tree, plus a pin-location search under `src/` and `docs/` | Every pin listed in check 5 matches. No stale previous hash of the changed game sources remains. |
| `_presenters` count against `ContentNativePermissionTests` `[InlineData]` (Node parse of base and HEAD) | 373 → 372 presenter rows (only `KeepHintClosed()` removed). 252 inline rows, all still present in the policy set. |
| Retained headless frame streams (read-only) | About 115–160 rows/s, no frame-number gaps |

### Evidence limits

- No Godot process, build, xUnit, rendered, LAN, inspection or packaged run was launched. The C# changes were judged from source, their callers and the GodotSharp 4.7.2 XML docs. That the code compiles is inferred from the fixer's game-tests history in the commit messages, which the reviewer did not re-run. The `FoundationLanPermissionTests` switch and the `AccessibilityName` setter were not executed.
- The frame-rate margins in Q4 come from retained earlier runs on this machine. The rendered refresh rate depends on the display.
- The D2 likelihood is a judgment from the scenario order, not a measured distribution.
- This re-review does not replace the phase-end rendered evidence or the human qualitative components.

_Reviewer: gsd-code-reviewer (fresh and independent; spawned by the /gsd-code-review workflow; did not author the repairs or the review of 0e8b859)_
