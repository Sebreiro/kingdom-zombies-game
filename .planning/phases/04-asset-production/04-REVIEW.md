---
phase: 04-asset-production
reviewed: 2026-11-21T01:39:15.822Z
reviewer: Codex Phase 4 Independent Reviewer
revision: d9c74bee6886dec06814533cf5196d4bdee20d12
source: 258897354953cf2142df712f0d6267b6bd1a53d4620a6beecd82d79d4b804e54
media: ba19b22a70ce25873efbe7e64be1ba8d0b50d1da4ac0b4a426fcc8272e51d547
tool: baa548789f3ecaa391d12e4a1e320cbac840a2ded71058335aa4f60571dbcfce
depth: standard
files_reviewed: 233
source_identity_files: 514
files_reviewed_list:
  - .gitignore
  - .planning/phases/04-asset-production/04-SCENARIOS.md
  - AGENTS.md
  - docs/code/engine-generated-spans.json
  - docs/code/generated-callsite-adaptations.json
  - docs/code/generated-layout-adaptations.json
  - docs/code/visibility-exemptions.json
  - docs/content/README.md
  - docs/content/fields.md
  - docs/content/production.md
  - docs/content/windows-inspection.md
  - docs/workflow/engine-defects.md
  - docs/workflow/setup.md
  - docs/workflow/verification.md
  - production/phase4/audio-actions.zip
  - production/phase4/audio-feasibility.json
  - production/phase4/audio-interface-ambience.zip
  - production/phase4/bodies-a.zip
  - production/phase4/bodies-b.zip
  - production/phase4/bodies-c.zip
  - production/phase4/common-selected.zip
  - production/phase4/defenses.zip
  - production/phase4/export-layout-origin.json
  - production/phase4/export-profile.json
  - production/phase4/export-toolchain.json
  - production/phase4/items-01-10.zip
  - production/phase4/items-11-24.zip
  - production/phase4/listening-route.md
  - production/phase4/main-buildings.zip
  - production/phase4/scale-pilot.zip
  - production/phase4/tracer-source.json
  - production/phase4/ui-identity.zip
  - production/phase4/work-special.zip
  - production/phase4/world.zip
  - specs/0.1/initial-spec.md
  - src/core/AppearanceState.cs
  - src/core/CharacterPresentation.cs
  - src/core/ContentPack.cs
  - src/godot-game/GeneratedCallSiteAdapters.cs
  - src/godot-game/assets/audio/actions.wav
  - src/godot-game/assets/audio/interface-ambience.wav
  - src/godot-game/assets/buildings/defenses.png
  - src/godot-game/assets/buildings/main-tiers.png
  - src/godot-game/assets/characters/bodies-a.png
  - src/godot-game/assets/characters/bodies-b.png
  - src/godot-game/assets/characters/bodies-c.png
  - src/godot-game/assets/characters/identity-wildlife.png
  - src/godot-game/assets/characters/structural-guard.png
  - src/godot-game/assets/characters/work-special.png
  - src/godot-game/assets/common/bow.png
  - src/godot-game/assets/common/donor-kit.png
  - src/godot-game/assets/common/scale-pilot.png
  - src/godot-game/assets/effects/effects.png
  - src/godot-game/assets/equipment/items-01-10.png
  - src/godot-game/assets/equipment/items-11-24.png
  - src/godot-game/assets/ui/Fallback.ttf
  - src/godot-game/assets/ui/InspectionTheme.tres
  - src/godot-game/assets/ui/LiberationSans.ttf
  - src/godot-game/assets/ui/PerfectDOSVGA437.ttf
  - src/godot-game/assets/ui/identity-symbols.png
  - src/godot-game/assets/ui/kingdom-zombies-icon.png
  - src/godot-game/assets/ui/kingdom-zombies.ico
  - src/godot-game/assets/world/environment.png
  - src/godot-game/content/animation/bow.json
  - src/godot-game/content/animation/fits-a.json
  - src/godot-game/content/animation/fits-b.json
  - src/godot-game/content/animation/fits-c.json
  - src/godot-game/content/animation/locomotion-melee.json
  - src/godot-game/content/animation/work-special.json
  - src/godot-game/content/audio/events.json
  - src/godot-game/content/compatibility/appearance-layout.json
  - src/godot-game/content/config/combat.json
  - src/godot-game/content/config/food-wildlife.json
  - src/godot-game/content/config/inventory.json
  - src/godot-game/content/config/items.json
  - src/godot-game/content/config/jobs.json
  - src/godot-game/content/config/local-preferences.json
  - src/godot-game/content/config/map-sites.json
  - src/godot-game/content/config/portals-assaults.json
  - src/godot-game/content/config/presentation.json
  - src/godot-game/content/config/run.json
  - src/godot-game/content/config/session-save.json
  - src/godot-game/content/config/structures.json
  - src/godot-game/content/config/survivors.json
  - src/godot-game/content/config/time.json
  - src/godot-game/content/config/towers.json
  - src/godot-game/content/config/zombie-loot.json
  - src/godot-game/content/config/zombie-progression.json
  - src/godot-game/content/config/zombies.json
  - src/godot-game/content/expected-obligations.json
  - src/godot-game/content/fixtures/audio-states.json
  - src/godot-game/content/fixtures/crowds.json
  - src/godot-game/content/fixtures/world-states.json
  - src/godot-game/content/pack-index.json
  - src/godot-game/content/packs/audio-actions.json
  - src/godot-game/content/packs/audio-interface-ambience.json
  - src/godot-game/content/packs/bodies-a.json
  - src/godot-game/content/packs/bodies-b.json
  - src/godot-game/content/packs/bodies-c.json
  - src/godot-game/content/packs/camp-access.json
  - src/godot-game/content/packs/common.json
  - src/godot-game/content/packs/compatibility.json
  - src/godot-game/content/packs/corpses-wildlife.json
  - src/godot-game/content/packs/defenses.json
  - src/godot-game/content/packs/delivery.json
  - src/godot-game/content/packs/effects-portals.json
  - src/godot-game/content/packs/guards.json
  - src/godot-game/content/packs/items-01-10.json
  - src/godot-game/content/packs/items-11-24.json
  - src/godot-game/content/packs/main-buildings.json
  - src/godot-game/content/packs/player-identities.json
  - src/godot-game/content/packs/scale-pilot.json
  - src/godot-game/content/packs/text-data.json
  - src/godot-game/content/packs/tracer.json
  - src/godot-game/content/packs/ui-camp.json
  - src/godot-game/content/packs/ui-identity.json
  - src/godot-game/content/packs/ui-inventory.json
  - src/godot-game/content/packs/ui-kit.json
  - src/godot-game/content/packs/ui-legacy-stash.json
  - src/godot-game/content/packs/ui-map-warnings.json
  - src/godot-game/content/packs/ui-session.json
  - src/godot-game/content/packs/ui-settings.json
  - src/godot-game/content/packs/world.json
  - src/godot-game/content/schema/fields-economy.json
  - src/godot-game/content/schema/fields-foundation.json
  - src/godot-game/content/schema/fields-items-combat.json
  - src/godot-game/content/schema/fields-world.json
  - src/godot-game/content/schema/fields-zombies.json
  - src/godot-game/content/text/en.json
  - src/godot-game/content/text/expanded.json
  - src/godot-game/content/ui/camp-panels.json
  - src/godot-game/content/ui/inventory.json
  - src/godot-game/content/ui/legacy-stash-targets.json
  - src/godot-game/content/ui/map-spectator-warnings.json
  - src/godot-game/content/ui/session-save-outcomes.json
  - src/godot-game/content/ui/settings-errors.json
  - src/godot-game/content/world/defense-assemblies.json
  - src/godot-game/content/world/main-assemblies.json
  - src/godot-game/content/world/map.json
  - src/godot-game/content/world/portal-assemblies.json
  - src/godot-game/docs/README.md
  - src/godot-game/export_presets.cfg
  - src/godot-game/inspection/AudioPreview.cs
  - src/godot-game/inspection/BuildingPreview.cs
  - src/godot-game/inspection/CampLayouts.cs
  - src/godot-game/inspection/CharacterPreview.cs
  - src/godot-game/inspection/CorpsePreview.cs
  - src/godot-game/inspection/CrowdPreview.cs
  - src/godot-game/inspection/DataTextPreview.cs
  - src/godot-game/inspection/IdentityKit.cs
  - src/godot-game/inspection/IdentityKit.tscn
  - src/godot-game/inspection/Inspection.tscn
  - src/godot-game/inspection/InspectionInput.cs
  - src/godot-game/inspection/InspectionRoot.cs
  - src/godot-game/inspection/InspectionShell.cs
  - src/godot-game/inspection/InventoryLayouts.cs
  - src/godot-game/inspection/ItemsWeaponsPreview.tscn
  - src/godot-game/inspection/ItemsWearablesPreview.tscn
  - src/godot-game/inspection/ScaleComparison.tscn
  - src/godot-game/inspection/SessionLayouts.cs
  - src/godot-game/inspection/WorldPreview.cs
  - src/godot-game/project.godot
  - tests/phase1/AppearanceStateTests.cs
  - tests/phase1/ArchitecturePolicy.cs
  - tests/phase1/AudioPresentationTests.cs
  - tests/phase1/BuildingPresentationTests.cs
  - tests/phase1/CampLayoutTests.cs
  - tests/phase1/CharacterBowTests.cs
  - tests/phase1/CharacterFitTests.cs
  - tests/phase1/CharacterPresentationTests.cs
  - tests/phase1/ContentInspectorTests.cs
  - tests/phase1/ContentNativePermissionTests.cs
  - tests/phase1/EngineAccessPolicy.cs
  - tests/phase1/EngineGeneratedTests.cs
  - tests/phase1/ExemptionTests.cs
  - tests/phase1/ExemptionValidator.cs
  - tests/phase1/FoundationPermissionTests.cs
  - tests/phase1/GeneratedCallSiteAdapterTests.cs
  - tests/phase1/GeneratedExportLayoutTests.cs
  - tests/phase1/GodotCallSiteAdapterTests.cs
  - tests/phase1/InventoryLayoutTests.cs
  - tests/phase1/RunnerContractTests.cs
  - tests/phase1/SessionLayoutTests.cs
  - tests/phase1/ThemeLayoutTests.cs
  - tests/phase1/WorldInspectorTests.cs
  - tools/code-rules/EngineGeneratedSyntax.cs
  - tools/code-rules/GeneratedLayout.cs
  - tools/phase1/check.cjs
  - tools/phase1/required-cases.json
  - tools/phase2/check.cjs
  - tools/phase2/evidence.cjs
  - tools/phase2/required-cases.json
  - tools/phase4/case-discovery.cjs
  - tools/phase4/cases/animation.cjs
  - tools/phase4/cases/audio-feasibility.cjs
  - tools/phase4/cases/audio.cjs
  - tools/phase4/cases/bodies-a.cjs
  - tools/phase4/cases/bodies-b.cjs
  - tools/phase4/cases/bow-work.cjs
  - tools/phase4/cases/census.cjs
  - tools/phase4/cases/corpses.cjs
  - tools/phase4/cases/data-camp.cjs
  - tools/phase4/cases/data-campaign.cjs
  - tools/phase4/cases/data-foundation.cjs
  - tools/phase4/cases/defenses.cjs
  - tools/phase4/cases/export-feasibility.cjs
  - tools/phase4/cases/export-layout.cjs
  - tools/phase4/cases/export-portability.cjs
  - tools/phase4/cases/export-profile.cjs
  - tools/phase4/cases/hub.cjs
  - tools/phase4/cases/integration.cjs
  - tools/phase4/cases/items.cjs
  - tools/phase4/cases/main-buildings.cjs
  - tools/phase4/cases/media.cjs
  - tools/phase4/cases/provenance.cjs
  - tools/phase4/cases/runner.cjs
  - tools/phase4/cases/text-data.cjs
  - tools/phase4/cases/ui-camp.cjs
  - tools/phase4/cases/ui-inventory.cjs
  - tools/phase4/cases/ui-kit.cjs
  - tools/phase4/cases/ui-session.cjs
  - tools/phase4/cases/world.cjs
  - tools/phase4/check.cjs
  - tools/phase4/content.cjs
  - tools/phase4/evidence.cjs
  - tools/phase4/expected-obligations.json
  - tools/phase4/export.cjs
  - tools/phase4/extract-audio.py
  - tools/phase4/extract-media.py
  - tools/phase4/handoff.json
  - tools/phase4/native-observer.cjs
  - tools/phase4/required-cases.json
  - tools/phase4/scenarios.cjs
findings:
  critical: 9
  warning: 2
  info: 0
  total: 11
  necessity: 0
status: issues_found
---

# Phase 4 independent code, content and evidence review

## Summary

Codex Phase 4 Independent Reviewer independently reviewed the Phase 4 implementation at `d9c74bee6886dec06814533cf5196d4bdee20d12`, source SHA-256 `258897354953cf2142df712f0d6267b6bd1a53d4620a6beecd82d79d4b804e54`. I did not implement the submitted source. Nine high-severity findings require resolution before the review binding can authorize the first rendered campaign. The clearest consequences are a guaranteed failure in a selected inherited solo resource case, retained package scenarios the live runner cannot execute as specified, and gates that accept incomplete review and execution evidence.

I scoped the effective Phase 4 diff from `5428da86b6dc753d43472ea8460acb997d7e15dd`. The list records changed product, tool, case, resource, native declaration and saved procedure paths. I excluded parallel Phase 08 planning and used Phase 4 plans and summaries as requirements and evidence context. I traced unchanged dependencies where they govern current behavior, including the earlier solo/LAN drivers, Windows input helper and capture/evidence consumers. I reviewed non-text media through authored manifests, source references, retained archive validation, exact hashes, raw pack membership and available native receipts. The inspection included Core/Godot boundaries, generated permissions and main-thread ownership, data/schema/config baselines, body/equipment/corpse/building/world/FX compositions, UI/text and native audio routes, path/JSON/media parsers, publication and export, current build binding, and scenario selection and receipt reconciliation.

The pinned Windows tool verified all 514 tracked inputs byte-for-byte at the reviewed revision. No byte drift was present. Its `SOURCE_ROOTS` are identical to the selected export revision `155c4e0cfe03171de95bd9a4cba71b0e0d8e0c06`. These checks concern the existing source and retained no-window evidence. I did not open game windows, perform input or listening, rerun broad build/test groups, or issue any visual, listening or prohibition verdict.

### Retained package and source evidence

| Reviewed subject | Observed identity or result |
| --- | --- |
| Selected portable directory | `.phase4/runs/1b3fce9a-592e-48bf-b341-45680caf7e38/portable` |
| Delivery receipt SHA-256 | `a38758612141ead470dcc7d64d82a63edb9b7d773f971c75874c9f8dcc2dae6c` |
| Executable SHA-256 | `4526aac3ac47c65de66f99f715f56b35a2e2e4446fcb23e6d280a830cd97fdea` |
| PCK SHA-256 | `c928c1b51c61dc4f400149fa374ef101b653b1624fa48ba1c008e5ae446e809b` |
| Managed game assembly SHA-256 | `d71a26585c8fbd47af071b70ea4daac9aa6127630f93506699575d3a9f8017c1` |
| Exact file closure | All 202 path/length/hash rows matched. Their directly summed extent was 237,271,120 bytes, 12 bytes below the prose total in the Plan 31 summary. The exact row and file hashes agree. |
| Stock and completed PCK | 212 stock members, 211 retained byte-for-byte, 24 exact raw additions and one authoring lockfile removed. All 235 final members parsed with matching internal digests. All 102 indexed runtime raw dependencies matched their declared SHA-256. The source-support files have a separate source identity and do not require duplicate raw copies of the compiled/imported scenes and code. |
| Runtime and shell | The executable is the authenticated official debug template with stock PE metadata. The product title and authored icon are configured in the project and delivered in the PCK. The self-contained runtime config pins .NET 10.0.15 and both retained host traces resolve bundle runtime assemblies and native libraries from the portable closure. |
| Build inputs and export group | The 244 paths in the export project-input map matched current bytes. The retained full source/package audit records 259 distinct inputs. All seven cases appear as passes in the exact `.phase4/temp/04-31-155c4e0-export-group.log`, SHA-256 `d5c7e7996bb0a9f452c93182ef153eef65b591bd50424557e6a878b98a0b6cfa`. |
| Positive and negative launch evidence | PIDs 28424 and 9392 reached the settled exported Inspection tree and exited zero from distinct clean profiles. The separately mounted partial PCK reported `IT-11` with `WORLD_INSPECTOR_MISSING content/packs/items-11-24.json` and opened populated sibling `IT-07` with the native `IT-07` clip. These receipts support the no-window export checks. |
| Authored coverage and provenance | Current source validation resolved all 180 obligation IDs, with zero actual acceptance. The bounded retained-source reader validated 14 archives and the selected common/font/work/interface source relationships. The source registers preserve donor-owner/object locators, explicit shared mappings, authored master/recipe evidence and pending approvals. Plans 23, 29 and 30 distinguish native/source checks from review and listening, including all 27 later variants. |
| Native permission evidence | I matched 251 distinct guarded-host declarations against compiled case declarations and revalidated all 251 exact per-case CTRF receipts in retained invocation `149fa2d6-2840-410a-9c16-5e388edd18bc`. The permission registry grants specific production owners and members. The retained receipt belongs to its original revision. |

The November 21 focused handoff has 107 unique original rendered procedures in the ownership register. Fourteen Phase 4 procedures are in G1-hub and four are in G2-packaged. The later phases own 25, 18, 6, 11 and 29 procedures respectively. Selection computation on this revision additionally finds nine inherited solo and twelve inherited LAN rendered regressions, including all affected Critical LAN cases. This is a read-only derivation from the tracked catalogs and effective changes. The independent named listening ledger requires 93 sample/loop entries and two mix entries. Current evidence still has zero accepted obligations, zero verdicts issued here, and zero Phase 4 rendered checkpoints.

## Narrative Findings (AI reviewer)

## Critical Issues

### CR-01 — The selected inherited solo resource check necessarily rejects the current project

**Classification:** BLOCKER  
**Severity:** high  
**File:** `tools/phase2/rendered.cjs:295-315`, `src/godot-game/project.godot:13-17`, `tools/phase4/scenarios.cjs:133-141,148-164,662-684`

**Issue:** The inherited `phase2/rendered.resources` is selected because current runtime inputs changed. Its ordinary rendered tracer eventually calls `validateResources`. That function copies a fixed Foundation-only list and requires every `res://` reference it finds in `project.godot` to appear in that list. Phase 4 added the feature-specific Inspection scene and replaced the application icon with an authored PNG. Both resources are outside the fixed list. I applied the exact scanner and set to the current files read-only. The missing paths are `inspection/Inspection.tscn` and `assets/ui/kingdom-zombies-icon.png`. A successful Foundation session still ends this case with `UNDECLARED_RESOURCE`.

**Fix:** Give the current inherited consumer an explicit, exact resource graph for the current project roles. Validate the ordinary Foundation scene, shared application icon, and declared feature override according to their actual purposes. Preserve the previous Phase 2 reports and expectations for their original subject. Add a no-window regression of `validateResources` on the current source plus a genuinely undeclared resource control, then run `phase2/rendered.resources` as part of the reviewed G3-solo group.

### CR-02 — The packaged group cannot execute the retained negative subjects or its held-out relaunch

**Classification:** BLOCKER  
**Severity:** high  
**File:** `tools/phase4/scenarios.cjs:377-402,529-545,613-655`, `tools/phase4/evidence.cjs:188-202`, `.planning/phases/04-asset-production/04-SCENARIOS.md:2516-2542,2572-2612`

**Issue:** G2 starts a single executable from the immutable healthy `build.package` directory. Its sole fixture inputs are the shared clean source and physical coordinates or key codes. H27 requires a conflicting/nearby-path subject with a functioning sibling. H28 requires an actually empty or partial package/gallery failure, rather than a search box that happens to show zero matches. The runner has no reviewed artifact or fixture setup step for either, and the valid package does not contain their missing subject. Only the separate `rendered.packaged-relaunch` ID receives a new process. The receipt validator explicitly requires a single process for H31, whose procedure requires two clean lifetimes of the same portable content. The retained export negatives demonstrate viable source/headless fixtures and cannot supply these rendered receipts.

**Fix:** Predeclare hash-bound, run-owned negative PCK fixture subjects derived from the reviewed portable closure for the incompatible H27/H28 setups. Keep the reviewed positive package fixed for clean startup and relaunch. Allow H31 and the relaunch procedure to cite the same two genuine lifetimes where appropriate, with individual checkpoints for each case and exact positive source/PCK, PIDs, process role and cleanup binding. Add negative controls that reject the healthy PCK as the failure stimulus and one lifetime for H31. Affected checks are `runner.prelaunch`, `runner.selection`, `export.portability`, H27/H28/H31, `rendered.packaged-relaunch` and `acceptance.artifacts`.

### CR-03 — The physical driver accepts unrelated observations and has no ready or reset gate

**Classification:** BLOCKER  
**Severity:** high  
**File:** `tools/phase4/scenarios.cjs:377-402,423-477,564-585,633-650`, `tools/phase4/evidence.cjs:188-229`, `tools/phase4/native-observer.cjs:173-215`

**Issue:** The sole tie to the case procedure is its ID and a nonempty stimulus string. An in-memory check accepted a Tab press and unchanged title label as the entire H27 procedure. This also applies to H01 provenance, H12/H13/H15 state transitions, navigation/focus and the other retained case-specific checkpoints. The generic property set cannot express several required observations, including inspected fixture metadata and exact diagnostic and resource focus semantics. Neither the live loop nor offline verifier requires baseline reset or a real state transition. Startup waits for an HWND and the debugger PID, then immediately injects input. The genuine exported observer had to wait for the naturally settled Operation label before probing the tree. G1 likewise has no readiness barrier before the first case. Each live probe takes one immediate native sample, and the sequential DWM capture by itself does not bind that sample to a specific current game draw.

**Fix:** Make each retained declaration carry reviewed, case-specific fixture identity, actions, required pre/post observations and ordinary reset checkpoints. Reuse the genuine debugger handshake and an explicit startup/settled-state condition. Require independent native object/visibility and draw identity for relevant changes and compare the physical action acknowledgment to the captured checkpoint. Add negative controls for title-only evidence, stale/pre-ready state, absent reset and a draw for a different observation. Affected checks are `runner.prelaunch`, `runner.selection`, all retained G1/G2 cases, `acceptance.judgments` and `acceptance.artifacts`.

### CR-04 — The rendered build gate verifies declarations without authenticating the executed closure

**Classification:** BLOCKER  
**Severity:** high  
**File:** `tools/phase4/scenarios.cjs:209-222,247-257,529-545,662-672,708-716,778-779`, `tools/phase4/evidence.cjs:188-202`, `src/godot-game/inspection/InspectionRoot.cs:64-75`

**Issue:** The preflight accepts a free-text `sourceLinkRevision` equal to the review revision and hashes whatever list the build record supplies. The only package target required by launch is an `.exe`, and completion compares that one target. The executable is the stock template. Its hash cannot distinguish two PCKs or managed runtimes. Companion PCK and native/managed closure membership, actual assembly identity from the running hub, and the prepared source project's Godot import and assembly artifacts are optional under this gate. G1/G3 run the live source project. At runtime, the targets file and `SourceRevisionId` environment values do not stamp or replace already compiled DLLs. G2 copies the current full OS environment apart from selected profile paths, whereas the successful export proof sanitized runtime lookup and retained resolved paths. A stale or swapped native assembly/pack or a host resolution override can undermine the evidence even while the claimed source/build digest matches.

**Fix:** Require and recheck the exact accepted delivery receipt and full portable file/PCK/managed/runtime closure. Verify the current pinned source project import and compiled assemblies for G1/G3 and authenticate their live identity using the production runtime marker or the existing native observers. Use the proven clean runtime environment for packaged launches. Compare each carried process record to its exact expected target and role both at launch and in final reconciliation. Add stale DLL, changed PCK with unchanged executable, unexpected companion artifact, and wrong engine/managed runtime controls. Affected cases are `runner.prelaunch`, `export.portability`, G1/G2/G3 and `acceptance.artifacts`.

### CR-05 — Prelaunch can report success without the actual review and can omit open findings

**Classification:** BLOCKER  
**Severity:** high  
**File:** `tools/phase4/cases/runner.cjs:123-130,246-264,373-393`, `tools/phase4/evidence.cjs:161-177`, `tools/phase4/scenarios.cjs:225-244,688-695`

**Issue:** The documented Plan 24 `runner.prelaunch` command returns a normal pass from detached controls when `04-REVIEW.json` is absent. The rendered command does require that file. Its review verifier, however, only finds the reviewer name and two digests as substrings of the Markdown, then compares two independently declared lists. An in-memory probe passed with a report containing an unresolved high finding and empty lists in review/fix records. A resolved finding can cite any committed file without relating the proof to its finding, relevant fix or current test. The rendered prelaunch sequence also does not check selected no-window results, including already-known later failures. The check happens only at completion.

**Fix:** Give detached validator controls an explicit source-only entry point. Make the Plan 24 command require and authenticate the whole independent ledger. Derive the exact findings IDs, severities and report status from the independent report payload and bind resolved records to the appropriate implementation and executed proof. Gate the first real launch on the required selected no-window receipts or proven eligible reuse. Test absent review, a dropped report finding, an unrelated fix proof and a newer failed prerequisite. Affected checks are `runner.prelaunch`, `runner.selection`, all first-window procedures and completion.

### CR-06 — The required artifact case invalidates its own source receipt

**Classification:** BLOCKER  
**Severity:** high  
**File:** `tools/phase4/check.cjs:233-240,561-585`, `tools/phase4/cases/runner.cjs:365,399-403`, `tools/phase4/scenarios.cjs:292-313,316-355,760-770`, `.planning/phases/04-asset-production/04-33-PLAN.md:133-140`

**Issue:** `acceptance.artifacts` is automatically required as an active Phase 4 source case. The runner writes its invocation record before executing it and writes the result after it returns. Once the completion index exists, the case calls full completion validation. That validation immediately requires a passing receipt for `acceptance.artifacts` itself. An earlier detached pass is superseded by this in-progress run of the same case, which is treated as a later missing result. A reference to the current receipt also fails because the result cannot exist yet. The required Plan 33 case cannot pass on a complete index using genuine records.

**Fix:** Separate the source-only artifact qualification receipt from the final closure-verifier result, or perform the verified result publication and closure in explicit stages without making the verifier its own input. Require earlier incomplete *independent* invocations to invalidate reuse. Add a control that runs both documented Plan 33 entry points against a consistent test ledger with an older passing qualification and an active closure validation. Affected checks are `acceptance.artifacts` and `--verify-completion`.

### CR-07 — Named mix evidence can attest an unrelated sample and omits the played mix identity

**Classification:** BLOCKER  
**Severity:** high  
**File:** `tools/phase4/evidence.cjs:233-272,282-305`, `src/godot-game/inspection/AudioPreview.cs:590-635,713-735,798-816`, `production/phase4/listening-route.md:7-12`

**Issue:** The individual sample checks tie WAV hashes and versions to the production catalog. D-31/D-32 have no expected WAV or mix identity. For those rows, the validator accepts a genuine PCM WAV of the declared hash with the events version number. It does not require the current events/routes, overlapping action and ambience, gains, loop bounds or an invocation-bound record of what was heard. The linked audio outcome establishes only that a saved physical action and an arbitrary permitted property were observed. Thus the two required mix decisions can appear complete with an unrelated WAV and no actual listening evidence of the conditions. The native UI selects a route, chooses variants internally and can play short samples before the single subsequent observation reaches them. A named reviewer needs the exact cue or extract identities in addition to a live mix checkpoint.

**Fix:** Bind D-31/D-32 to the specific current production routes, variants, playback/loop configurations and signed-off listening context by exact evidence reference. Record actual heard findings for each independently. For the 93 sample/loop entries preserve immutable exact playback or extracted-sample identity so short cues and all 27 new variants are auditable. Add rejection controls for unrelated or wrong-version mix audio, missing overlap/loop settings and a named record attached to an unrelated screenshot. Affected checks are `audio.lifecycle`, `audio.heldout`, `rendered.audio-audition`, `acceptance.judgments` and completion.

### CR-08 — The Error filter returns valid resources labelled Partial

**Classification:** BLOCKER  
**Severity:** high  
**File:** `src/godot-game/inspection/InspectionRoot.cs:339-358,361-365,448-487`, `src/core/ContentPack.cs:152-161`, `.planning/phases/04-asset-production/04-UI-SPEC.md:116-123`

**Issue:** The Error and Incomplete options include every matching ID in the independent expected register without checking whether that ID has a healthy backing entry or an error. The list then labels an entry Partial whenever a validated entry exists or the ID has a world inspector. With the current complete source census, selecting Error returns valid entries labelled Partial. With a broken world dependency, a known ID can still be labelled Partial in the list while the selected inspector reports Error. Opening an unvisited category also selects `common.bow` as the fallback even when it is absent from that category's rows. The discrepancy defeats the retained error/navigation and register state checks.

**Fix:** Compute a single per-ID status from actual validation, diagnostic category and authored backing availability. Use it for filtering, row labels and selection. Retain independent missing IDs without classifying validated definitions as errors. Choose a valid row in a newly visited category or show the empty selection explicitly. Exercise the healthy Error filter, missing `IT-11`, healthy sibling, empty search and category restoration. Affected checks are `hub.source`, `content.census`, `export.hub-consumer`, UIH.HUB-NAV.error, UIH.HUB-REGISTER.partial and `rendered.hub-navigation`.

### CR-09 — Concurrent recovery can remove another publisher's lock

**Classification:** BLOCKER  
**Severity:** high  
**File:** `tools/phase4/content.cjs:476-499,502-519,529-538`, `docs/content/production.md:25-27`

**Issue:** Two recoverers can read and validate the same dead owner. Recovery performs mutation before the final ownership check, and the final read/check is separate from the unlink. One recovery can delete the old lock and a new publisher can acquire `publication.lock` between the other recovery's final read and unlink. The delayed recovery can remove the new lock. This breaks the advertised exclusive writer guarantee and lets subsequent writers race to replace the current pointer. Reading the lock contents one more time without acquiring mutation ownership does not make recovery atomic.

**Fix:** Serialize recovery and writer lock creation with a shared exclusive operation, or use an atomic claim protocol in which only the process that claims a verified dead owner can remove its files and release the publication slot. Verify ownership before mutations within that critical section. Add a two-recoverer plus new-writer test that pauses recovery at the former race point and proves both the current pointer and the new lock remain correct. Affected checks are `source.publication`, `content.census` and the interrupted publication/recovery backstops.

## Warnings

### WR-01 — Interrupted campaign groups can lose the run record and process cleanup

**Classification:** WARNING  
**Severity:** medium  
**File:** `tools/phase4/scenarios.cjs:529-610,688-756`, `tools/phase4/evidence.cjs:308-321,455-477`

**Issue:** The campaign UUID is written with drivers before execution, but history only scans folders that already have a terminal `report.json`. A stopped process or exception before that write leaves no reported attempt. Starting another attempt can conceal it from final reconciliation. Inspection sessions clean up in the normal async `finally` path, with no equivalent to the inherited solo process signal handler. Their process record infers that no PIDs remain from a single child `close` event. Group deadlines are inspected between checkpoints, with an individual checkpoint allowed to overrun the group and still pass.

**Fix:** Write an immutable, campaign-bound start receipt before launching and require a corresponding terminal or explicit interruption receipt in history. Route process signals and timeouts through owned cleanup and verify the owned process set has terminated before releasing the lock. Apply the remaining group deadline to helper calls and preserve receipts even if report reconciliation itself fails. Affected checks are `runner.prelaunch`, grouped rendered cleanup and `acceptance.artifacts`.

### WR-02 — Static character mode leaves the previous dynamic sample on the native root

**Classification:** WARNING  
**Severity:** medium  
**File:** `src/godot-game/inspection/CharacterPreview.cs:852-918,1656-1668`

**Issue:** `Compose` clears the managed sample when the character returns to Static pose with Attack None. `Project` updates native `sample` metadata only while a dynamic JSON sample exists. The earlier bow or melee sample remains inspectable on the root alongside the static visual. The properties expose contradictory action, ammunition or logical-hand evidence after an ordinary mode change.

**Fix:** Clear the native sample in the same generation when the managed sample is empty, or publish a truthful explicit static snapshot. Add a production native control that activates bow with the last arrow, changes to Static/None and proves the dynamic metadata is retired. Affected cases are the character/bow native checks, H12, H15 and UIH.CHARACTER-PREVIEW.populated.

---

The independent review is complete for the identities above. All nine blocker dispositions are open. Each subsequent implementation change requires the affected no-window checks and a renewed independent binding to the resulting source, media, tool and executable closure. The genuine headless delivery and byte checks remain evidence at their recorded scope.

