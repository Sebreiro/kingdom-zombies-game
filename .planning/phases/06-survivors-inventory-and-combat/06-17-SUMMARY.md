---
phase: 06-survivors-inventory-and-combat
plan: "17"
subsystem: testing
tags: [verification, rule-suite, naming, r-48, edge-census]
status: complete

requires:
  - phase: 06-survivors-inventory-and-combat
    provides: "06-16 repaired rule-suite fixtures; run 1 listed the 43 R-48 naming violations (F5)"
provides:
  - "All 43 run-1 R-48 violations renamed to Method_State_Expected, with no rule, analyzer or exemption change"
  - "R-48.positive and R-48.negative pass on the current proof assembly"
  - "Phase 6 edge census unchanged: 132 of 132 resolved edges, E074 deferred"
affects: [06-24, 06-25, phase-11-certification]

actuals:
  tokens: 6100
  tasks: 3
  commits: 3
plan_head_before: 69447ad88d97779ba3dbecfdf97cd94b4e042274
plan_head_after: 5ebf612f8d80b660cb8a69edee4ad66fbff83e92

tech-stack:
  added: []
  patterns:
    - "Rename-only repairs are proven by a zero count of non-identifier diff lines plus per-name execution counts in the pure CTRF report"

key-files:
  created: []
  modified:
    - src/godot-game/tests/verification/CharacterBowTests.cs
    - src/godot-game/tests/verification/FoundationLanInputTests.cs
    - src/godot-game/tests/verification/FoundationLanLifecycleTests.cs
    - src/godot-game/tests/verification/Phase06BowTests.cs
    - src/godot-game/tests/verification/Phase06ExternalInteractionTests.cs
    - src/godot-game/tests/verification/Phase06FixtureTests.cs
    - src/godot-game/tests/verification/Phase06InventoryInteractionTests.cs
    - src/godot-game/tests/verification/Phase06PopulationTests.cs
    - src/godot-game/tests/verification/Phase06PublicationTests.cs
    - src/godot-game/tests/verification/Phase06SnapshotTests.cs
    - src/godot-game/tests/verification/Phase06StashTests.cs
    - src/godot-game/tests/verification/Phase06UiStateTests.cs

key-decisions:
  - "F5 closes by renaming the 43 methods. R-48 and its checker stay as they are, with no allowlist or exemption."
  - "The renames drop the descriptive Phase06_ prefix, because Phase06_E### is reserved for edge identities"
  - "Historical planning summaries (06-04, 06-10, 06-11) keep the old names they record as past evidence"

patterns-established:
  - "Phase 6 non-edge test names use Subject_State_Expected; only Phase06_E###_ names are edge identities"

requirements-completed: []

coverage:
  - id: D1
    description: "Ten bow, LAN input and lifecycle proofs renamed; still compile and run"
    requirement: "COMBAT-03"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build,basic.pure (run 299be9c1, 2/2, 0 warnings, pure 204/204 with 0 skipped)"
        status: pass
      - kind: unit
        ref: "dotnet KingdomZombies.Phase1.Tests.dll --filter-method *LastArrow_CommittedRelease_SamplesRecoveryThenQuietAim (direct, PHASE1_ROOT set, 1/1)"
        status: pass
    human_judgment: false
  - id: D2
    description: "Sixteen external-interaction, fixture, inventory-interaction and population proofs renamed"
    requirement: "INV-08"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build,basic.pure (run e0ab6667, 2/2, 0 warnings, pure 204/204, every renamed test executed)"
        status: pass
    human_judgment: false
  - id: D3
    description: "Seventeen publication, snapshot, stash and UI-state proofs renamed; R-48 and the edge census pass"
    requirement: "RUN-04"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/verification/check.cjs --cases R-48.positive,R-48.negative (run 99999d1a, 2/2: TEST_NAMES_ACCEPTED, CA1707)"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/edge-check.cjs 6 (exit 0, 132 of 132 resolved, E074 deferred)"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build,basic.pure (run 171b4a44, 2/2, 0 warnings, pure 204/204, every renamed test executed)"
        status: pass
    human_judgment: false

duration: 5min
completed: 2026-11-25
---

# Phase 6 Plan 17: R-48 test method names Summary

**All 43 Phase 6 test methods that run 1 flagged under R-48 now use the three-segment Method_State_Expected shape. Only identifiers changed. R-48 passes with no allowlist, the strict build has zero warnings, the 204 pure tests all run with no skips, and the edge census still resolves 132 of 132 edges.**

## Performance

- **Duration:** about 5 min
- **Started:** 2026-11-25T12:19:24Z
- **Completed:** 2026-11-25T12:24:00Z
- **Tasks:** 3 of 3
- **Files modified:** 12

## Accomplishments

- **F5 closed.** 10 + 16 + 17 = 43 methods were renamed. `git diff 69447ad..5ebf612 -- src` changes 43 declaration lines and 0 other lines. Bodies, attributes, traits and XML comments are byte-identical.
- **No further violations.** R-48.positive scans every production `[Fact]`/`[Theory]` and found none beyond the 43, so no new method needed renaming.
- **Execution, not just compilation.** For every renamed name in tasks 2 and 3, the pure CTRF report shows at least one executed passing row (theories show one per InlineData row). In task 1, nine of the ten methods run in basic.pure. The tenth, `CharacterBowTests`, is outside basic.pure (it runs in the asset-pipeline bow-work case), so its renamed method was run directly and passed 1/1.
- **Edge census unchanged.** No `Phase06_E###` method was renamed. `edge-check.cjs 6` lists 132 distinct edge methods, and E074 stays deferred in 06-SPEC.md.

## Renames

| File | Old | New |
| --- | --- | --- |
| CharacterBowTests | CommittedLastArrowSamplesRecoveryThenReturnsToQuietAim | LastArrow_CommittedRelease_SamplesRecoveryThenQuietAim |
| FoundationLanInputTests | Input_CursorCoordinatesResolveToExactBoundedHostAim | Input_CursorCoordinates_ResolveToBoundedHostAim |
| FoundationLanInputTests | Input_JumpAndAttackRequireFreshWorldPresses | Input_JumpAndAttack_RequireFreshWorldPresses |
| FoundationLanInputTests | Input_QuickFoodUsesItsPermittedReferenceWithoutAnOpenGrid | Input_QuickFood_UsesPermittedReferenceWithoutOpenGrid |
| FoundationLanInputTests | Input_UnloadedBowRetainsAimAndCannotRelease | Input_UnloadedBow_RetainsAimAndCannotRelease |
| FoundationLanInputTests | Input_WorkspaceTransitionClearsMovementAndPendingPresses | Input_WorkspaceTransition_ClearsMovementAndPendingPresses |
| FoundationLanLifecycleTests | Lifetime_ProviderDisconnectPreservesTheExactResponseDeadline | Lifetime_ProviderDisconnect_PreservesExactResponseDeadline |
| Phase06BowTests | AlreadyRestingSurvivorAppliesImpactAtTheSubtick | ArrowImpact_AlreadyRestingSurvivor_AppliesAtSubtick |
| Phase06BowTests | JumpAndLandingFollowTheSameTrajectoryAcrossHostSteps | JumpAndLanding_SplitHostSteps_FollowSameTrajectory |
| Phase06BowTests | LethalFlightLeavesTheCorpseAtItsCommittedImpact | LethalFlight_SplitHostSteps_LeavesCorpseAtCommittedImpact |
| Phase06ExternalInteractionTests | Phase06_DepositEffectUsesTheSelectedStashAndExactAction | DepositEffect_SelectedStashPage_UsesExactAction |
| Phase06ExternalInteractionTests | Phase06_ExternalPagerUsesCurrentTokenAndRejectsBadInput | ExternalPager_CurrentToken_RejectsBadInput |
| Phase06ExternalInteractionTests | Phase06_ExternalRevocationKeepsIndependentOwnWindows | ExternalRevocation_AccessLost_KeepsIndependentOwnWindows |
| Phase06ExternalInteractionTests | Phase06_NearbySelectionsUseStablePermittedIdentities | NearbySelection_EqualLabels_UseStablePermittedIdentities |
| Phase06ExternalInteractionTests | Phase06_QuickWithdrawUsesActualCarryWithoutEquipping | QuickWithdraw_ActualCarry_MergesWithoutEquipping |
| Phase06ExternalInteractionTests | Phase06_WindowSelectorsKeepExistingBagsAtTheProtocolLimit | WindowSelectors_ProtocolLimit_KeepExistingBags |
| Phase06FixtureTests | Phase06_FixtureDoesNotAlterNormalStartup | Fixture_NormalStartup_KeepsConfiguredKit |
| Phase06FixtureTests | Phase06_FixtureKeepsSeparateCorpseOwnership | Fixture_TwoLethalContacts_KeepSeparateCorpseOwnership |
| Phase06FixtureTests | Phase06_FixtureSuccessorRangePreservesTheSeededSurvivor | Fixture_SuccessorRange_PreservesSeededSurvivor |
| Phase06InventoryInteractionTests | Phase06_BagWindowsCloseByParentAndRememberClampedPosition | BagWindows_ParentClosed_CloseAndRememberClampedPosition |
| Phase06InventoryInteractionTests | Phase06_InventoryPreviewChecksRotatedCellsAndManualReplacement | InventoryPreview_RotatedOrOccupied_ChecksCellsAndManualReplacement |
| Phase06InventoryInteractionTests | Phase06_InventoryWindowsAndQuickAssignmentUseHostAuthority | InventoryWindows_QuickAssignment_UseHostAuthority |
| Phase06InventoryInteractionTests | Phase06_PreviewCancellationPreservesTheCommittedSource | PreviewCancellation_OutsideReleaseOrClose_PreservesCommittedSource |
| Phase06InventoryInteractionTests | Phase06_SplitPreviewBoundsAndQuantityUseTheCurrentStack | SplitPreview_CurrentStack_BoundsQuantity |
| Phase06PopulationTests | ReadsSuppliedResourcesBeforeOpeningTransport | Coordinator_SuppliedResources_ReadBeforeOpeningTransport |
| Phase06PopulationTests | RejectsInvalidRunDefinitionsBeforeBuildingInventory | RunDefinitions_InvalidBytes_RejectedBeforeBuildingInventory |
| Phase06PublicationTests | AuthorizedOffPageWindowsSurviveRefreshAndFenceOldPartsOnTransfer | OffPageWindows_RefreshThenTransfer_SurviveAndFenceOldParts |
| Phase06PublicationTests | FullStashGridPulseFitsTheWireWithoutLosingCells | StashGridPulse_FullPage_FitsWireWithoutLosingCells |
| Phase06PublicationTests | GameplayVersionIdentifiesScopedPublicationFrames | GameplayVersion_CurrentEpoch_IdentifiesScopedPublicationFrames |
| Phase06PublicationTests | GuestMotionAttackAndFoodTraverseTheGameplayWire | GuestRequests_MotionAttackAndFood_TraverseGameplayWire |
| Phase06PublicationTests | LargeScopedPageInstallsAtomicallyAndRevocationFencesDelayedFrames | ScopedPage_LargeThenRevoked_InstallsAtomicallyAndFencesDelayedFrames |
| Phase06PublicationTests | LooseLootPagesRetainAuthorizedTokensAndExactNonInitialItemReferences | LooseLootPages_ThousandsOfItems_RetainTokensAndExactItemReferences |
| Phase06PublicationTests | MaximumLengthEscapedNearbyIdsFitOneNativeFrame | NearbyIds_MaximumEscapedLength_FitOneNativeFrame |
| Phase06PublicationTests | OversizedPublicFramesInstallTogetherAndDoNotDelayPrivateFencing | PublicFrames_Oversized_InstallTogetherWithoutDelayingPrivateFencing |
| Phase06PublicationTests | SaturatedNearbyListsKeepTheStashAndEveryBoundedPageAvailable | NearbyList_Saturated_KeepsStashAndEveryBoundedPageAvailable |
| Phase06PublicationTests | SpectatorRetainsConnectionAndNewControlRejectsTheOldEpoch | Spectator_NewControl_RetainsConnectionAndRejectsOldEpoch |
| Phase06PublicationTests | TrustedGuestAndSoloUseOneDispatcher | Dispatcher_TrustedGuestAndSolo_UseOneAuthority |
| Phase06SnapshotTests | RunSnapshotPreservesCurrentAppearanceWithoutReplayingTheCue | RunSnapshot_ActiveCues_PreserveAppearanceWithoutReplay |
| Phase06StashTests | QuickWithdrawalOfTheLastStashItemPublishesOneOwner | QuickWithdrawal_ContestedLastStashItem_PublishesOneOwner |
| Phase06StashTests | QuickWithdrawalWithNoRoomRejectsWithoutAPartialMerge | QuickWithdrawal_NoRoom_RejectsWithoutPartialMerge |
| Phase06UiStateTests | Phase06_HudReadsOnlyCarriedFoodAndReadyArrows | Hud_ReservesPresent_ReadsOnlyCarriedFoodAndReadyArrows |
| Phase06UiStateTests | Phase06_SpectatorCameraCutsAcrossUnknownOrDistantViews | SpectatorCamera_UnknownOrDistantView_Cuts |
| Phase06UiStateTests | Phase06_SpectatorWaitsWithoutRepeatingTheRequest | Spectator_QueuedRequest_CompletesWithoutRepeating |

## Task Commits

1. **Task 1 (tracer): bow, LAN input and lifecycle proofs.** `7cc551d` (test)
2. **Task 2: external-interaction, fixture, inventory-interaction and population proofs.** `8344cb4` (test)
3. **Task 3: publication, snapshot, stash and UI-state proofs, then R-48 and the edge census.** `5ebf612` (test)

## Verification runs

| Case | Run | Outcome |
| --- | --- | --- |
| basic.strict-build, basic.pure (after task 1) | game-tests `299be9c1` | PASS 2/2, 0 warnings, 204/204, 0 skipped |
| CharacterBowTests renamed method, direct filtered run | n/a (scratch results) | 1/1 passed |
| basic.strict-build, basic.pure (after task 2) | game-tests `e0ab6667` | PASS 2/2, 0 warnings, 204/204, 0 skipped |
| R-48.positive, R-48.negative | verification `99999d1a` | PASS 2/2 (TEST_NAMES_ACCEPTED, CA1707) |
| basic.strict-build, basic.pure (after task 3) | game-tests `171b4a44` | PASS 2/2, 0 warnings, 204/204, 0 skipped |
| edge-check.cjs 6 | stdout only | exit 0, 132 of 132 resolved, E074 deferred |

The R-48 run's summary records revision `8344cb4`, because Task 3's renames were still uncommitted working-tree changes when it ran. The proof reads source files, and the committed `5ebf612` content is identical to what it scanned.

## Deviations from Plan

None. The plan executed as written.

Notes, not deviations:
- The plan's reference baseline was `09e2881`, and this run started at `69447ad`. At `69447ad`, `git grep` found no code reference to any old name outside its declaring file. The only other mentions are historical evidence lines in 06-04, 06-10 and 06-11 SUMMARY files and in this plan, and they were left unchanged.
- An extra direct run covered the one renamed method outside basic.pure (CharacterBowTests). This adds evidence and changes no code.

## Issues Encountered

None.

## Known Stubs

None.

## Next Phase Readiness

F5 is closed. Sequential gap plans continue with 06-18. The known open items owned by 06-24 are unchanged: the `ac-08 derivation` binding test and the F3 REPOSITORY_SOURCE_HASH cases.

## Self-Check: PASSED

- All 12 modified files exist
- Commits `7cc551d`, `8344cb4` and `5ebf612` are ancestors of HEAD
- 0 of the 43 old identifiers remain under src/godot-game
