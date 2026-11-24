---
phase: 06-survivors-inventory-and-combat
plan: "14"
subsystem: packaged-gameplay-and-independent-review
tags: [godot, csharp, windows, package, lan, review, asset-handoff]
requires:
  - phase: 04
    provides: "Original inspection procedures, source identities and asset handoff ownership"
  - phase: 06-13
    provides: "Shared gameplay scenarios and exact Phase 6 edge census"
provides:
  - "Eighteen original asset inspection cases selected by their existing identities"
  - "Pinned Release boot, gameplay resource and rejected configuration evidence"
  - "Independent whole-phase review and targeted production repairs"
affects: [06-15]
tech-stack:
  added: []
  patterns: [exact-source-asset-bindings, bounded-authorized-projections, isolated-release-proof-companion]
key-files:
  created:
    - src/godot-game/tests/scenarios/phase06-inspection.cjs
    - src/godot-game/tests/scenarios/phase06-packaged.cjs
    - src/godot-game/core/RunTrajectoryContact.cs
    - src/godot-game/game/foundation/FoundationPublicWorld.cs
  modified:
    - src/godot-game/tools/game-tests/catalog.cjs
    - src/godot-game/tools/game-tests/catalog.json
    - src/godot-game/tools/game-tests/catalog.test.cjs
    - src/godot-game/tools/asset-pipeline/content.cjs
    - src/godot-game/tools/asset-pipeline/export.cjs
    - src/godot-game/core/FoundationSession.cs
    - src/godot-game/core/RunProjections.cs
    - src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs
    - src/godot-game/game/foundation/FoundationInventoryWorkspace.cs
    - src/godot-game/tests/scenarios/phase06-headless.cjs
    - src/godot-game/tests/verification/Phase06PublicationTests.cs
key-decisions:
  - "Record untouched Release boot separately from normal Start driven by an isolated probe companion, with identical native runtime and managed assemblies."
  - "Keep complete public roster and world installation atomic while sending private control and authorization changes on their own fence."
  - "Page nearby selectors and loose items under their existing recipient authorization. Retain an installed window only while the next host capture confirms the same scope."
  - "Order time, recovery, projectile contact and lethal ownership changes by the actual sub-tick contact."
requirements-completed: [INV-01, INV-08, INV-10, INV-11, COMBAT-08, RUN-01, RUN-04]
actuals:
  tokens: 250270
  tasks: 2
  commits: 13
commits: 13
plan_head_before: d93d4cb787fb7158fa5a814326ee9c1de672ea21
plan_head_after: 17fb851c2bfd81ba050fa094527ef21d7707690c
duration: 172min
completed: 2026-11-24
status: complete
---

# Phase 6 Plan 14 Summary

The shared checker selects all 18 original Phase 4 handoffs. The pinned Release package and its gameplay resources passed headless verification, and an independent reviewer cleared the exact repaired Phase 6 source.

## Performance

- Execution started at 18:49:33 UTC and ran for 172 minutes, including independent review and exact verification.
- Two tasks completed. The ledger measures 13 commits and 70 changed files before this summary.

## Accomplishments

- Activated the original four heldout inspections and 14 interface states through their existing case IDs, fixtures and procedures. Exact owner membership, unrelated owner retention and native scenario selection are executable checks.
- Proved untouched Release startup through the normal menu. The owned package companion retained the exact Release executable, managed assemblies, main-scene selection and gameplay resource bytes. Its PCK added the probe, changed the generated project configuration to register the probe autoload, and carried the regenerated UID cache. The companion drove the ordinary Start signal and native input and returned to the menu.
- Proved eight live survivors, exact CH-01 body segment, clothing icon and crown atlas regions, and rejected a deliberately incorrect crop. The separate package variants produced the expected menu diagnostics for absent and malformed run definitions and a combat interval of 600.001 seconds. The configured 600-second bound loaded successfully.
- Repaired bounded public convergence, immediate private control fencing, exact selected-scope refresh and access loss, lobby recipient isolation, projected movement restoration, chronological projectile damage and recovery, and the rendering and dependency findings in the independent review. Native bow release, selected-world outlines and saved spectator camera procedures now have explicit evidence requirements.
- Kept reachable stash and loot discoverable under large inventories. Tests covered 17,000 distinct nearby fixed-point pile anchors, a full page of 128-character escaped identities, 4,098 loose items in one selected pile, a 4,096-cell stash pulse and 16 nested selectors per side. The actual four-process case navigated both pagers, opened a bag and pouch, retained their exact scoped controls and focus after another recipient's withdrawal, and transferred the original off-page bag. Both external windows then closed.

## Task Commits

| Task | Commits |
| --- | --- |
| Original handoff activation and pinned Release case | `d051155` |
| Historical independent review | `9a8c180` |
| Review repairs and exact source bindings | `0e163e3`, `77be6a3`, `176e129` |
| Hosted runtime pin, chronological contact, nearby pages and runtime fixture publication | `85c1d2d`, `f4963f5`, `8c948bb` |
| Compatibility hash, selected item pages and retained/revoked views | `24a06ba`, `5b41a04`, `8d0cc81`, `f642777` |
| Exact native scenario baseline | `17fb851` |

## Verification

| Bound source | Proof | Result |
| --- | --- | --- |
| `17fb851c2bfd81ba050fa094527ef21d7707690c` | Independent rereview of the exact clean whole-phase source | Clean, with no new findings |
| Same clean source | Shared `--pr`, invocation `ce2dfbb4-09d5-40a4-99fc-e54cdbc514d0` | 4/4 in 60.865 s. Strict build, repository proofs, 204 pure tests with zero skips and native headless smoke |
| Same clean source | Six native gameplay cases and `phase06.packaged`, invocation `e0efbd40-dffc-4095-ac07-885719bb58c4` | 7/7 in 128.689 s. World, normal startup, inventory, combat, lifecycle and host plus three guests, then six separate Release package subjects |
| Same clean source | Standalone host and three guest LAN repeat, invocation `a1c80cf3-c78f-415e-95a3-0e0875f2a809` | 1/1 in 43.038 s. Both pagers, private scopes, retained focus, transfer and denial |
| Same clean source | Catalog, mock runner, selection-only and edge reconciliation | All 24 tests passed. 123 required rows, with 74 no-window rows and 49 rendered rows, including all 18 exact handoffs. All 132 resolved edge methods passed, plus both P01 and P02 controls. E074 remains deferred to Phase 12 |
| `8d0cc81f271f6c0d5bee382d36eb3c66abdba8b9` | Exact clean replay of the four inherited asset failures, invocation `1dc1a810-ed21-4906-af64-6c8166508e73` | 4/4 in 432.825 s. 180/180 source tracer, tracer failure controls, complete native census cohorts and text compatibility |
| Final scoped refresh in the precommit working tree based on `8d0cc81`, then committed as `f642777` | Precommit report `ffdd9c6d-48c4-4c59-873d-4c131f0b53b3` | 2/2 in 49.908 s. Delayed and replayed actual scope frames, 204 pure tests, and actual native keyboard focus and transfer closure |

The exact game-test invocations retain their separate `selection.json` and `summary.json` under `.local/runs/game-tests/`. The final PR directory also holds the edge reconciliation, the original-owner selection and catalog test output. The final package case retains the original Release process record, probe binding, package closure and each positive or negative launch separately. The final package artifact is `cf599027-7ded-4e0d-a7cf-550f4592123a/phase06.packaged.json` in the seven-case invocation. Its untouched PCK SHA-256 is `180be1b2a37fee7c150f5e2ce0b7df93330f2636096185f10612bf91a1e0dcc0`.

## Original Failures and Repairs

- The review initially found public payload fallback, stale passive reachability, sprite and state restoration gaps, nonlinear and chronological projectile errors, configuration divergence and overly broad dependency selection. Further exact review exposed legal nearby and loose item cardinality, a full stash pulse over the wire limit, displaced nested selectors, off-page bag closure during navigation and unrelated refresh, and focus jumping between identically named controls. The final native and pure reports exercise the affected behavior with real current ownership and retained original failure reports.
- The clean `7692488d-a1aa-484f-835e-e5c037345ccc` and `9f1ec302-b114-45c5-ae36-1410bf4d0403` groups each recorded 23/27 while stale nested source, fixture and text publications were being resolved. The latter produced four exact failures from a remaining compatibility binding to the old text-data hash. The compatibility binding and its own index entry were refreshed in `24a06ba`. The follow-up replay passed all four on clean `8d0cc81`. The later changes address scoped refresh, focus and native scenario sequencing.
- Hosted Windows and Ubuntu checks on `d93d4cb` failed at real Respawn in workflow `45463690968` after Godot selected a later .NET runtime from the installed majors. The runner now isolates the approved SDK and runtime, explicitly sets the architecture-specific runtime root, and observes the exact loaded version against the manifest. Both halfway jobs passed in workflow `45484530549` at `85c1d2d`. Both combined jobs passed in workflow `45488551824` at `5b41a04`. Both hosted jobs also passed on the production-source predecessor `f642777` in workflow `45491913932` with jobs `138792177458` and `138792178072`.
- Native RED `101bebe5-e1a1-4c24-9d3d-f7456c574d98` caught the actual off-page bag control disappearance. RED `3669ed63-81fd-40aa-9c0f-ff9a670175e7` caught removal of a still-authorized view before its replacement and the exact nested grip losing focus. The prior source, native errors and results stay identifiable in their own reports.
- The exact `f642777` gameplay and package run `8b91fb79-394d-404c-a195-60f43ab92f12` passed six of seven cases. LAN frames 243–244 showed the opening workspace installing the carried scopes after the panel drew. Frame 258 showed page two with the same new owned revision and scopes. The driver now waits for the committed initial workspace and requires both the owned rows and revision to stay identical during paging. The corrected case passed in precommit report `c8f8cf2b-9519-4b24-a24f-7cbefaf30cd0` and the two final clean runs.

## Deviations and Evidence Boundaries

- [Rule 1] Fixed findings from the independent review across the owning gameplay, wire, inventory, presentation, asset and verification paths. Framework exceptions remain method-specific.
- [Rule 3] Rebound exact changed runtime fixture, text and source bytes through their existing publications. Original handoff owners, archive paths, media and previously recorded results retain their original provenance.
- The packaged proof used Godot 4.7.2 Mono, SDK 10.0.404, .NET 10.0.15 and Node 24.22.1 on the installed Windows environment. The package report records the machine's installed .NET and registry availability and the resulting dependency resolution boundary. The export proof covers the release boot and owned gameplay variants under that configuration.
- All executed evidence in this plan used headless processes. The rendered inspections and physical gameplay groups are the next plan's evidence.

## Threat Flags

| Flag | Files | Surface and proof |
| --- | --- | --- |
| `threat_flag: protocol_schema` | `FoundationProtocol.cs`, `FoundationCoordinator.RunWire.cs` | Loose-item page requests use the current ready sender, binding, control epoch and exact reachable selection token. Scoped rows retain byte bounds and atomic assembly. Missing scopes receive immediate revocation. The transmitted-bounds and stale replay tests exercise these controls. |

## Self-Check: PASSED
