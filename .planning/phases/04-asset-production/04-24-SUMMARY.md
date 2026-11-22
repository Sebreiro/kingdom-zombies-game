---
phase: 04-asset-production
plan: "24"
subsystem: review-and-prelaunch-selection
tags: [Windows, review, selection, evidence, runner]
status: complete
requires:
  - phase: 04-30
    provides: Saved focused selection, evidence validation and the Windows handoff
  - phase: 04-31
    provides: Portable Windows inspection build and its delivery bindings
provides:
  - Clean independent whole-phase review of the final source, with every original and renewed finding resolved
  - Current-source qualification of all 776 selected pre-window results plus the detached judgment control
  - Declared 819-case selection with 39 rendered cases in ordered process groups, validated before any window
affects: [04-32, 04-33]
tech-stack:
  added: [CPython 3.12.14 pin, PowerShell 7 test-discovery prerequisite]
  patterns: [Committed-input inherited receipts, Campaign-scoped rendered attempts, PID and creation-time process identity, Retryable interruption receipts]
key-files:
  created:
    - tools/phase4/python.cjs
    - tools/phase4/python.json
    - tools/phase4/rendered-contracts.cjs
    - tools/phase4/rendered-process.cjs
    - tools/phase4/reviewed-source.targets
    - .planning/phases/04-asset-production/04-REVIEW.json
    - .planning/phases/04-asset-production/04-REVIEW-FIXES.json
    - .planning/phases/04-asset-production/04-SELECTION.json
  modified:
    - tools/phase4/scenarios.cjs
    - tools/phase4/evidence.cjs
    - tools/phase4/check.cjs
    - tools/phase4/cases/runner.cjs
    - tools/phase4/required-cases.json
    - tools/phase1/required-cases.json
    - tools/phase2/required-cases.json
    - tools/phase2/check.cjs
    - docs/workflow/setup.md
    - .planning/phases/04-asset-production/04-REVIEW.md
    - .planning/phases/04-asset-production/04-SCENARIOS.md
key-decisions:
  - Bind inherited Phase 1-3 receipts to their committed inputs at the recorded revision; the rebuilt test DLL embeds HEAD, so a tool digest cannot be compared across commits.
  - Keep every rendered attempt for one review and selection in one campaign history, and record a crashed attempt only after its runner and owned processes are gone.
  - The creator accepted the passing reruns of world.replacement after one intermittent Godot .NET handle error and asked to finish Phase 4.
requirements-completed: []
requirements-contributed: [ASSET-01, ASSET-02, ASSET-03, ASSET-04, ASSET-05]
actuals:
  tasks: 2
commits: 51
plan_head_before: 3ad71c9
plan_head_after: 1e0fe27
completed: 2026-11-22
---

# Phase 04 Plan 24 — Independent review and prelaunch selection

The final Phase 4 source has a clean independent review. All 776 selected pre-window results and the detached judgment control pass on it. The 819-case selection and its process groups were declared and validated before any game window. The prelaunch and selection gates pass at `1e0fe27`.

## Reviewed source

| Identity | Value |
| --- | --- |
| Revision | `5c637da23437ea09501a9b167449425a824fe1c1` |
| Source | `29578ddb9b6fe12f4a37188cb008faf5dda68988ebb32298534f9f9c317b7ae9` |
| Media | `056324ea9e8255b2e1afe7b7596ac21906ef25364424d4057c2e70547e0e59fd` |
| Tool | `081ee3aa048f6f5e8e6b0bea902fe1911d9255a9c3f1721fc3444fb17fdcae6e` |
| Review build | `.phase4/runs/eb05f596-d4a0-421b-954a-ab17f2929ccd/review-build.json`, `cb7411a80166a952fd5e862806461a61d9e35cb8c6e9f4ac63ca594545312aad` |
| Portable package | `.phase4/runs/b0111601-0bf8-43e3-bcb7-d12ca022f111/portable/`: `KingdomZombies.exe` `4526aac3…`, `KingdomZombies.pck` `f35c3d76…` |
| Qualification | `.phase4/temp/04-24-5c637da-current-source-qualification.json`, `71fa77472220c86747ed0099e7468dcce136475e1ab5036ca719bbabfce09b4a` (local, ignored) |
| Review report | `.phase4/temp/04-24-5c637da-independent-review.md`, `b62211f5278e5a7214e08446302b6fbf27bed02f3b9068f82ad9da5ce2fb04ac`; committed as `04-REVIEW.md` |

## Review history

The original whole-source review at `e9a1bc4` reported CR-01 to CR-09, WR-01 and WR-02. A focused interim review reported RR-01 to RR-05. Each was repaired in a commit named after its ID, and `04-REVIEW-FIXES.json` binds each original finding to its repair commit and the cases that prove it.

The renewed reviews then ran on each repaired revision:

| Revision | Status | Findings |
| --- | --- | --- |
| `fee2a0e` | issues found | CR-10 (inherited Phase 1 cases omitted from the impact selection), WR-03, WR-04, WR-05, IN-01 |
| `84df4a8` | issues found | WR-06 (inherited tool check broke after any rebuild), IN-02 (interruption side effects before validation) |
| `8889f5a` | clean | none |
| `5c637da` | clean | none |

The creator approved each repair batch. `5c637da` adds no code: it declares the two Python pin files in the Phase 1 repository source set (see Deviations).

## Executed checks

The full Windows batch ran on `5c637da` with PowerShell 7 on PATH.

| Check | Result | Evidence |
| --- | --- | --- |
| Strict solution build | 0 warnings, 0 errors | `.phase4/temp/04-24-5c637da-strict.log` |
| `export.portability` | pass | `.phase4/runs/b0111601-0bf8-43e3-bcb7-d12ca022f111` |
| Source preparation and review build | pass | `review-build.json` above |
| 71 selected Phase 4 pre-window cases | 70 pass in the batch; `world.replacement` passed on rerun | per-case logs `.phase4/temp/04-24-5c637da-*.log` |
| `acceptance.judgments` control | pass | included in the qualification |
| Windows Phase 1 `--phase 4` | 678/678 | `.phase1/runs/efe8d62c-44ad-4296-b648-bf7fddfb8610/summary.json` |
| Windows Phase 2 core and runner | 4/4 and 3/3 | `.phase2/runs/win32-x64/9d97fe12-…` and `7766f96c-…` |
| Windows Phase 3 | 20/20 | `.phase3/runs/win32-x64/1a7c35d2-364f-40e6-9368-cec3bcccd615/summary.json` |
| `runner.prelaunch` at `1e0fe27` | pass | `.phase4/runs/43296212-9914-465f-a6c1-606d25b318ee` |
| `runner.selection` at `1e0fe27` | pass | `.phase4/runs/6a3ffe9a-90f3-493b-9489-110cd9659120` |

The selection declares 819 cases with none omitted: 93 Phase 4 (75 no-window, 18 rendered), 678 Phase 1, 16 Phase 2 (7 and 9) and 32 Phase 3 (20 and 12). Qualification requires 776 pre-window results (71 Phase 4, 678, 7 and 20). The 39 rendered cases run in G1-hub (14), G2-packaged (4), G3-solo (9) and G3-lan (12). No game window opened in this plan.

## Deviations and preserved failures

- **Phase 1 source set (8889f5a).** The batch on `8889f5a` passed every Phase 4, 2 and 3 case but Phase 1 failed 675/678 (`.phase1/runs/f32bd493-1291-4d54-8b97-c12d16d36852`). The IN-01 repair added `tools/phase4/python.cjs` and `python.json` as tracked checked paths without declaring them in the Phase 1 repository source set. `5c637da` declares both as outside-validator rows and rebinds the five Phase 1 and one Phase 2 caller digests. The three cases passed in isolation, then 678/678 in the full batch.
- **Intermittent engine error (5c637da).** `world.replacement` failed once in the batch (`.phase4/runs/88bdbc40-e7a8-407d-b375-ae018140847b`). The game printed its PASS marker, but the Godot .NET bridge logged `InvalidOperationException: Handle is not initialized` from `ScriptManagerBridge.SwapGCHandleForType` inside `ResourceLoader.Load<PackedScene>` at `src/godot-game/inspection/WorldPreview.cs:769`, so the empty-stderr requirement failed. `src/` was identical to `8889f5a`, where the case passed. Five reruns at `5c637da` passed (`e6f56745-…`, `550fc847-…`, `4da1e4b7-…`, `b947e7c4-…`, `e1523f09-…`). The suspected cause is a race between the .NET finalizer and the main thread reviving the cached item preview scene, whose managed wrapper is a discarded local. The creator accepted the passing rerun. A workaround that keeps both preview scenes in fields would need the generated-span governance rebind, a renewed review and a full batch; it is recorded as a follow-up and is not applied.
- **Missing PowerShell 7.** `content.native-permissions` first failed with `spawn pwsh ENOENT`. `docs/workflow/setup.md` now lists PowerShell 7 and CPython 3.12.14.
- Superseded batches on `fee2a0e`, `84df4a8` and `8889f5a` remain in `.phase4/temp` and their run folders. The final batch logs are UTF-16 because Windows PowerShell 5.1 hosted it; the local qualification script decodes them.

## Artifacts this phase produces

| Path | Actual symbols, bindings and resources |
| --- | --- |
| `tools/phase4/rendered-process.cjs` (new) | `managedMvid`, `timeBudget`, `observeChildren` (with `track`, `remaining`, `cleanup`), `command`, `windowsPowerShell`, `journal`. Owned processes are identified by PID and creation time, journaled with launch and exit, and cleaned up in at most three rounds. |
| `tools/phase4/rendered-contracts.cjs` (new) | `CASES`, `template`, `validateDeclaration`, `matches`, `readyChecks`, `baseline`, `P4_DEATH_CHECKPOINTS`, `AUDIO_CASE`, `AUDITION_PROPERTY`, `AUDIO_PLAYBACK`, `AUDIO_DECISIONS`: the physical checkpoint contracts for the 18 rendered Phase 4 IDs. |
| `tools/phase4/python.cjs`, `tools/phase4/python.json` (new) | `PYTHON`, `PIN`, `verifyPython`, `isPython`. CPython 3.12.14 executable and `python312.dll` hashes and Pillow 13.0.0; `KZ_PHASE4_PYTHON` overrides the location. |
| `tools/phase4/reviewed-source.targets` (new) | MSBuild binding of Phase 4 proof builds to the reviewed source. |
| `tools/phase4/scenarios.cjs` | Adds `inheritedBinding`, `completionSourceReceipts`, `recordInterruption`, `preWindowCases`, `verifyFixProof`, `savedNativeSnapshot`, `relaunchCase`, `packagedSubject`, `prepareSource`, `validateBuild`, `validateSourceProject`, `repairAffected`, `attemptDeclaration`, `validateAttemptHistory`, `verifyInheritedRendered`, `runInspectionLane`, `sourceImportCompanions`, `validateLoadedSource`, `validateProcessRuntime`, `validateProcessPackageFiles`; internal `campaignScope`, `attemptRows`. `impactClosure` adds each family's own runner and tests. |
| `tools/phase4/evidence.cjs` | Adds `REVIEW_REPORT`, `ORIGINAL_REVIEW`, `committedSnapshot`, `reviewReport`, `audioAuthority`, `audioPlayback`, `writeAuditionMedia`, `listeningSubjects`, `validateJudgmentImages`, `campaignAttempts`, `repairHistory`. `identifyTool` includes the Python pin. |
| `tools/phase4/check.cjs` | Verifies the pinned Python before every Python command; adds `--record-interruption INVOCATION`. |
| `tools/phase4/export.cjs`, `tools/phase4/native-observer.cjs` | Adds `RENDERED_FIXTURES`, `renderedFixture`, `verifyRenderedFixture`, `hostTrace`, `validatePortableReceipt`, `validatePortableEnvironment`; `RUNTIME_PROPERTY`, `runtimeMarker`, `READY`, `HUB`, `TOOLBAR`. |
| `tools/phase4/cases/runner.cjs` | Adds `processIdentityControls`, `inheritedReceiptControls`, `campaignHistoryControls`, `interruptionControls` and their negative controls. |
| `tools/phase4/cases/*.cjs`, `tools/phase4/content.cjs` | Repairs for CR-01 to CR-09, WR-01, WR-02 and RR-01 to RR-05; `provenance.cjs` adds publication-race controls. |
| `tools/phase4/required-cases.json` and `src/godot-game/content/packs/delivery.json`, `pack-index.json` | Case module SHA-256 bindings, the embedded gzip catalog and its pack-index entry. |
| `tools/phase1/required-cases.json`, `tools/phase2/required-cases.json` | 482-path repository source set (`sourceSetSha256` `8038db86…`) and caller repository digest `69a44bbd…`. |
| `tools/phase2/check.cjs` | Exports `inputManifest`, `environment`, `toolIdentity` for committed inherited inputs. |
| `src/godot-game/inspection/*.cs`, `src/godot-game/foundation/FoundationDiagnostics.cs` | `InspectionRoot` resource browser status and selection reconciliation (CR-08); `CharacterPreview` static-mode retirement (WR-02); `AudioPreview.PlaybackBinding` (CR-07); `RecordLoadedAssemblies`. |
| `tests/phase1/*.cs` | Inherited proof and regression tests listed in the commits above, with the generated-span and call-site governance rebinds. |
| `docs/workflow/setup.md` | PowerShell 7 and CPython 3.12.14 prerequisites. |
| `.planning/phases/04-asset-production/04-REVIEW.json`, `04-REVIEW-FIXES.json`, `04-SELECTION.json`, `04-REVIEW.md` | The bound clean review, the eleven original dispositions, and the declared selection with its process groups. |

No acceptance credit is recorded. Plan 32 owns the rendered run of the 39 selected cases, the named listening verdicts and the four judgments on this reviewed build. Plan 33 reconciles the resulting evidence.

## Self-Check: PASSED
