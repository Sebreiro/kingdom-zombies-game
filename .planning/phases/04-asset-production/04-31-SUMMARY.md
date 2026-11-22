---
phase: 04-asset-production
plan: "31"
subsystem: windows-inspection-delivery
tags: [Windows, Godot, dotnet, export, portability, headless]
status: complete
requires:
  - phase: 04-30
    provides: Saved focused selection, evidence validation and the Windows handoff
  - phase: 04-23
    provides: Complete current resource index and production inspection hub
provides:
  - Authenticated portable Windows inspection build with exact bundle and source inventory
  - Read-only current hub startup, exact negative controls and clean natural relaunch evidence
  - Portable launch instructions and DT-19 through DT-23, DT-26 and DT-27 delivery bindings
affects: [04-24, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [Bounded read-only native observation, Exact raw and imported PCK members, Independent runtime profiles]
key-files:
  created:
    - tools/phase4/cases/export-portability.cjs
  modified:
    - tools/phase4/check.cjs
    - tools/phase4/export.cjs
    - tools/phase4/native-observer.cjs
    - tools/phase4/required-cases.json
    - .planning/phases/04-asset-production/04-SCENARIOS.md
    - src/godot-game/content/pack-index.json
    - src/godot-game/content/packs/delivery.json
    - docs/content/README.md
    - docs/content/windows-inspection.md
    - src/godot-game/docs/README.md
key-decisions:
  - Preserve both the engine's imported and remapped members and the exact raw bytes read by the ContentPack model.
  - Bind the portable package to external immutable execution evidence and keep in-pack acceptance metadata acyclic.
  - Give natural startup credit to the directly launched exported executable and use editor-mounted PCK probes for individual resource support.
requirements-completed: []
requirements-contributed: [ASSET-01, ASSET-02, ASSET-03, ASSET-04, ASSET-05]
actuals:
  tokens: 75234
  tasks: 2
  commits: 4
commits: 4
plan_head_before: b1eb2907263eae35f084d0dabe502130f7114009
plan_head_after: 155c4e0cfe03171de95bd9a4cba71b0e0d8e0c06
duration: "70 min"
completed: 2026-11-21
---

# Phase 04 Plan 31 — Portable Windows inspection delivery

The genuine Godot 4.7.2 .NET Windows inspection export passed the complete seven-case export group. The selected package opened the current hub, reached 180 of 180 source-ready entries, and exited normally in two separate headless launches. A controlled missing item produced an exact diagnostic and the neighboring valid Bow preview remained usable. Plan 24 owns independent review. The same selected build goes to Plan 32 after that review.

## Selected package

Use the complete `.phase4/runs/1b3fce9a-592e-48bf-b341-45680caf7e38/portable` folder. On Windows, open `KingdomZombies.exe` from that folder beside `KingdomZombies.pck` and `data_KingdomZombies_windows_x86_64`. The 1280×720 session follows the focused Windows procedure. For a bounded headless smoke check, run `.\KingdomZombies.exe --headless --quit-after 360` from the portable folder. [Windows inspection and launch guide](../../../docs/content/windows-inspection.md) · [Focused Windows handoff](04-HANDOFF.md)

| Selected artifact | Bytes | SHA-256 |
| --- | ---: | --- |
| `KingdomZombies.exe` | 103,418,880 | `4526aac3ac47c65de66f99f715f56b35a2e2e4446fcb23e6d280a830cd97fdea` |
| `KingdomZombies.console.exe` | 188,928 | `f78703ba3a05dd51a3464f8c90d4c72f54f95d5a8669993e7a5c3ce522b30c52` |
| `KingdomZombies.pck` | 45,857,756 | `c928c1b51c61dc4f400149fa374ef101b653b1624fa48ba1c008e5ae446e809b` |
| [Game assembly](../../../.phase4/runs/1b3fce9a-592e-48bf-b341-45680caf7e38/portable/data_KingdomZombies_windows_x86_64/KingdomZombies.dll) | 620,032 | `d71a26585c8fbd47af071b70ea4daac9aa6127630f93506699575d3a9f8017c1` |
| `portable-delivery.json` | | `a38758612141ead470dcc7d64d82a63edb9b7d773f971c75874c9f8dcc2dae6c` |
| Independent package and source inventory | | `5fa665013c8ae9732acd1c82df2f914f84a2f37a9502bea0e19fe40b410c6247` |

The independent audit matched the package's 202 files, all 235 PCK members and all 259 distinct source inputs. The package totals 237,271,132 bytes. The source inputs matched both the working files and the raw tracked blobs at `155c4e0`. The untouched official debug template supplies the executable and its shell metadata. The PCK contains the project's application icon, English catalog, fonts, audio, layouts, data and inspection documentation. [Portability receipt](../../../.phase4/runs/1b3fce9a-592e-48bf-b341-45680caf7e38/portable-delivery.json) · [Full source, package and PCK inventory](../../../.phase4/temp/04-31-final-package-audit-1b3fce9a-592e-48bf-b341-45680caf7e38.json)

The selected package uses the permanent `Phase4 Windows inspection` preset and `asset_inspection` feature. Each headless process started directly from the portable folder with a distinct clean profile and System32 on PATH. The host trace resolved the self-contained .NET 10.0.15 assemblies and native libraries inside the bundle. The trace records the tested host's installed registry and shared-store probes. Source and actual acceptance metadata remain separately auditable. [Portability receipt](../../../.phase4/runs/1b3fce9a-592e-48bf-b341-45680caf7e38/portable-delivery.json)

## Executed checks

The complete `--group export` invocation exited 0 at `155c4e0`. Its seven cases used the current source. The group log SHA-256 is `d5c7e7996bb0a9f452c93182ef153eef65b591bd50424557e6a878b98a0b6cfa`. [Group log](../../../.phase4/temp/04-31-155c4e0-export-group.log)

| Case | Run identity |
| --- | --- |
| `export.layout` | `7e56d424-ac33-464e-ad1b-4005eae3573f` |
| `export.feasibility` | `6883bf12-7919-4ff2-90a0-555e66d1a8fa` |
| `export.hub-consumer` | `2851d144-bfcd-46da-985b-1378d91aedcc` |
| `export.portability` | `1b3fce9a-592e-48bf-b341-45680caf7e38` |
| `export.profile` | `7462bae7-4568-4bfc-ae9a-442acfe596cb` |
| `export.profile-boundaries` | `2beab20b-902c-41ef-998b-2c72efbf1926` |
| `export.application-icon` | `956254a9-8392-4bfa-9784-427f602f9891` |

The standalone `--case export.portability` invocation passed in `93be5e8f-3bf3-44e9-a0a9-424758e396f9`. All 259 inputs for that output matched the subsequent Task 2 commit `e01db46`. The complete group after the compatibility repair produced and independently audited the selected current package above. [Standalone receipt](../../../.phase4/runs/93be5e8f-3bf3-44e9-a0a9-424758e396f9/portable-delivery.json)

The committed Task 1 gate passed `source.tracer` in `84c6686c-2926-4ae4-8808-05b5e0da94e0`, `source.tracer-failures` in `52dac388-eb59-4e30-801c-7e7f9c8f21d8` and `export.hub-consumer` in `7a36f3df-b924-4b0e-b302-623bbbfd6c5f`. The source case exercised all 11 expected rejections plus fresh restored and current positive controls on unchanged donor bytes. The two unchanged profile cases passed in the first committed sweep and again in the final group.

The affected saved runner and evidence checks passed. The run IDs were `cec0b8b8-72fe-4814-9d5b-a11c1e0c3acd` for `runner.prelaunch`, `abc870a9-c923-4bc1-9f51-5b0100eff393` for `runner.selection`, `559735ac-fe6e-4bc5-ab1c-50d155744fb5` for `acceptance.judgments` and `0a3763b4-d471-4497-889b-7987c5ae7726` for `acceptance.artifacts`. Case discovery and the updated catalog passed in `477e56fa-c479-4fa1-8956-d322d194d27b` and `38ef32c8-929c-4793-aa55-4f828e785b30`. Native DT-26/27 support passed in `e470635c-9016-4c15-b6d3-a64750d1c087`. The documentation navigation and project-reference cases each passed. [Docs navigation](../../../.phase1/runs/6872f042-f5d9-4796-ade1-e69442a008e4/summary.json) · [Project-reference validation](../../../.phase1/runs/640518ec-2182-4b29-8c9b-da4d85c649b9/summary.json)

### Current hub and failure evidence

- The exported processes with PIDs `28424` and `9392` each reached the settled Inspection scene and exited zero. Their distinct launch IDs were `0d822d38-881f-4cc5-8b3d-4a0aaf5bd57c` and `dda59096-91d4-4038-bfb7-734d75ef437e`. Each exposed 138 native nodes and 39 required paths. The loopback listener sent only the scene-tree and single-object inspection requests. The source-bound validation checked the exact selected ID, name, visible status labels, registration, category controls and 24 Items rows. The retained `common.bow` Sprite has the exact texture and registration and is hidden for the current unavailable legacy selection. The independently authored `IT-07` Bow entry remains present.
- The retained 16×18 Bow PNG has SHA-256 `08b53699e459d71cf73f3e7be84af19d04805d5a7a488121e39a514a81701999` and decoded RGBA SHA-256 `88b8e7062f0eb9cfb788ef3658028ba58a6f62d408d3e39ae2e1228bfa6c8905`. The editor-mounted probe checked the 42 visible pixels, pivot (0.375, 0.5), offset (−6, −9), unit scale and native nearest filter, with the actual exported assemblies. The probe also checked all 180 exact Register IDs against the independent authority. This probe supplies mounted resource evidence.
- The natural partial-package process rejected the clean register expectation. In the mounted PCK fixture at PID `33392`, `IT-11` reported `WORLD_INSPECTOR_MISSING` for the item pack. Its visible failure text named the pack index field `outputs` and the exact missing resource field `bytes`. Selecting the independent `IT-07` sibling opened the populated authored view and selected the actual `IT-07` AnimationPlayer clip. Separate nearby filename, wrong-case, reordered and changed-byte subjects preserved the same working sibling. [Missing item pack](../../../src/godot-game/content/packs/items-11-24.json) · [Current pack index](../../../src/godot-game/content/pack-index.json)
- The selected run retained eight independent package rejections and 21 observer controls per clean observation. Exact case collisions and encoded aliases failed path validation. Empty and invalid independent-register packages failed with current native observations. A deliberate missing main-scene link returned exit 1 with the exact resource path. The owned font import stopped after its real initialization marker at PID `38276`, before import completion. The final clean relaunch used its own profile and the same recorded package hashes. [Portability result](../../../.phase4/runs/1b3fce9a-592e-48bf-b341-45680caf7e38/result.json)

## Task commits

| Work | Commit |
| --- | --- |
| Task 1 current native hub, runtime PCK closure and case registration | `0c2acf2` |
| Task 1 strict source tracer dependency repair | `fb84217` |
| Task 2 provenance crosswalk, guides and exact portability case | `e01db46` |
| Full-group missing-CoreCLR compatibility repair | `155c4e0` |

## Deviations and preserved failures

- The original nine-node observer rejected the genuine exported Inspection tree after a clean exit in `777b990c-0875-458c-87aa-fd1a8f5f40ca`. GSD qualified the targeted RED assertion. The current bounded observer waits for the real validation state before it inspects the settled tree. The pinned debugger limits inspection to one requested object by default. The final harness queries one at a time and gives the engine 360 frames for real startup and natural close. [Task 1 RED receipt](../../../.phase4/temp/04-31-red-final.json) · [Godot 4.7.2 scene debugger source](https://raw.githubusercontent.com/godotengine/godot/4.7.2-stable/scene/debugger/scene_debugger.cpp)
- The stock export contained native remaps and imported resources and omitted 24 exact raw runtime files used by ContentPack, totaling 26,299,168 bytes. The bounded repack adds these independently authenticated source members alongside their stock remaps, preserves all 211 runtime-bearing stock members byte for byte and omits the authoring `packages.lock.json`. The original stock PCK stays in the selected run. Every process targeted at importing a project went through the repository's single-threaded editor import policy. [Selected PCK closure](../../../.phase4/runs/1b3fce9a-592e-48bf-b341-45680caf7e38/pck-runtime-closure.json) · [Pinned PCK reader](https://raw.githubusercontent.com/godotengine/godot/4.7.2-stable/core/io/file_access_pack.cpp) · [Editor import defect and workaround](../../../docs/workflow/engine-defects.md)
- The first committed tracer sweep found that the profile's standalone game build changed the Core MVID while the compiled test pickup held the previous Core DLL. The exact mismatch remains in `570adb46-3882-422b-9792-24f388a7751c` and `99248ac4-1a89-4af9-84d9-cff55d389d68`. The tracer now runs the existing strict solution build and repair assertions before snapshots and copies.
- GSD qualified the Task 2 RED assertion for the missing registered portability procedure. During implementation the first two full exact-case attempts exposed the editor console-wrapper PID difference and the isolated PowerShell executable's unavailable hash cmdlet. The corrected item fixture launches the directly authenticated native editor. The launch diagnostic showed a truncated package could stop before headless initialization. The final fixture removes the exact main-scene remap from an otherwise valid PCK and requires the resulting process exit and missing scene message. The interruption marker matches the engine's actual terminal output. [Task 2 RED receipt](../../../.phase4/temp/04-31-task2-red.json)
- The first complete group ended 6/7 because the deliberate missing-CoreCLR process exited at Windows 0xC0000005 before the earlier debugger-only expectation. Its one missing file and fresh host diagnostics are preserved. The repair checks exact runtime-closure difference, process and exit identity, self-contained config and the observed hostpolicy/hostfxr/crash diagnostics. It retains the authentic structured debugger failure route. The separately reproduced repair and the new complete group passed. [First group](../../../.phase4/temp/04-31-e01db46-export-group.log) · [Focused missing-runtime repair](../../../.phase4/temp/04-31-coreclr-tracer/bad-runtime-native-missing-member.json)

## Artifacts this phase produces

| Path | Actual symbols, bindings and resources |
| --- | --- |
| `tools/phase4/native-observer.cjs` | Existing `encode` and `decode`, bounded canonical `tree` and `hub`, `properties`, `validate`, `observe` and `diagnostics`. Exact Inspection root, Bow and Summary, toolbar, category and browser Controls, preview surface, transport, details and failure panel. |
| `tools/phase4/export.cjs` | `pack`, `packBytes`, `sourceContract`, `runtimeMembers`, `completePack`, `packaged`, `probe`, `mountPackage`, `packagedProbe`, `portable`, `missingCoreClr`, `negatives`, `buildPackage`, `verifyInputs`, `exportTracer` and source snapshot updates. Preserves the approved template, preset, two managed export configurations and native resource dependencies. |
| `tools/phase4/check.cjs` | `tracerAssemblies`, `runTracer` and `tracerFailures`. Reuses the actual current closure and builds the matching game and Core assemblies before snapshotting and observing. |
| `tools/phase4/cases/export-portability.cjs` | `hubConsumer`, `itemProbe`, `fixtureEngine`, `inspectItems`, `validateDelivery` and `portability`. Registers Critical no-window `export.hub-consumer` and `export.portability` in `G0-export-hub` at orders 310 and 320. |
| `tools/phase4/required-cases.json` and `.planning/phases/04-asset-production/04-SCENARIOS.md` | Exact saved case procedures, module digest bindings and current executable lifecycle for the two cases. Preserves the focus and ownership register for later rendered claims. |
| `src/godot-game/content/packs/delivery.json` and `src/godot-game/content/pack-index.json` | `supportCrosswalk`, `fixtureCatalog`, `guides`, portable preset/files/feature and external receipt declaration, exact updated file references and affected dependency rows. All seven DT support bindings and 180 source IDs remain available. |
| `docs/content/README.md`, `docs/content/windows-inspection.md`, `src/godot-game/docs/README.md` | Source and Windows navigation, exact portable launch and close/relaunch instructions, missing-resource recovery and DT-18/19–23/26/27 entry points. |

The existing `export.profile` and `export.profile-boundaries` cases reuse their current source closure. The five-project graph, exact retained source Bow bytes and standalone Foundation entry passed their existing support checks. The checkout's three transient Git status indicators were confirmed to have the committed bytes. Workstation proof storage used lossless NTFS compression during the constrained runs. Package and receipt hashes were checked after retention.

The recorded acceptance count is zero. AC-15/16/17/19 have the source and Windows no-window evidence above. Plan 24 independently reviews the current source and selected package. Plan 32 owns rendered startup/relaunch, input, appearance, readability and listening on the reviewed build. Plan 33 reconciles the resulting actual evidence and named judgments. The focused handoff assigns other rendered procedures to Phases 5–9. Linux execution remains with its platform owner.

Execution began `2026-11-21T00:10:19.623Z` and finished `2026-11-21T01:20:28.778Z`. The measured implementation patch was 300,936 characters across 11 authored paths.

## Self-Check: PASSED

All 11 scoped artifacts, the four implementation commits and the selected retained package and audit receipts were checked against the current workspace.
