---
phase: quick-261123-fsu
plan: 01
subsystem: repository-layout
tags: [merge, repository-layout, asset-pipeline, phase4, pr7, rebinding]
status: complete
requires:
  - phase: 04-asset-production
    provides: "Phase 4 squash 1a14684 on dev (PR #5)"
  - phase: quick-261122-ub4
    provides: "PR #7 task-end cleanup rules and tools (a305b8b on dev)"
provides:
  - "PR #6 branch integrates origin/dev with one merge commit and linear fix commits"
  - "Every Phase 4 and PR #7 file at its current layout location"
  - "src/godot-game/tools/asset-pipeline/layout.cjs logical-to-physical map for Phase 4 record identities"
  - "Rebound registers, pins, asset catalog, delivery, pack index and ac-08 repository stages"
  - "Live docs pointing at the relocated tree; STATE.md with the Phase 5 renewal handoff"
affects: [phase-05, asset-pipeline, verification, gameplay-checks, lan-checks, workspace-cleanup]
tested_revision: e7ec53d36846c845480c509fe4ec098aff974c33
plan_head_before: c5be170a4df039ffb54a5a2f903c5c9b305a56cf
plan_head_after: 6bd6179b8e08520f2216119db8052e36ccb38e5d
review_fix_head: e7ec53d36846c845480c509fe4ec098aff974c33
review_fixed: [CR-01, CR-02, WR-01]
actuals:
  tokens: 276000
  tasks: 3
  commits: 13
key-files:
  created:
    - src/godot-game/tools/asset-pipeline/layout.cjs
    - src/godot-game/tests/verification/AssetPipelinePaths.cs
    - src/godot-game/tools/asset-pipeline/run-extractor.py
  modified:
    - .planning/STATE.md
    - docs/architecture/repository-layout.md
    - docs/content/production.md
    - docs/workflow/cleanup.md
    - docs/workflow/engine-defects.md
    - docs/workflow/setup.md
    - src/godot-game/tools/README.md
    - src/godot-game/tools/verification/required-cases.json
    - src/godot-game/tools/asset-pipeline/required-cases.json
    - src/godot-game/game/content/pack-index.json
    - src/godot-game/game/content/packs/delivery.json
key-decisions:
  - "Phase 4 content and pack records keep their original paths as logical identities; one map (layout.cjs) resolves them, and runner bindings use physical paths"
  - "Build and check in place in the PR worktree (CONTEXT discretion) instead of a detached verification worktree"
  - "Phase 4 review-bound gates are expected-blocked by the relocation; their renewal is handed to Phase 5"
requirements-completed: [QUICK-261123-FSU]
completed: 2026-11-23
---

# Quick Task 261123-fsu: Integrate dev (Phase 4 and PR #7) into PR #6 Summary

**One merge commit (parents `c5be170` and `a305b8b`) plus eight linear fix and docs commits move all 206 dev-added Phase 4 and PR #7 files to the current layout. Phase 4 record identities resolve through one asset-pipeline map, and every hash binding the move broke is regenerated. Four review-fix commits then repair CR-01, CR-02 and WR-01 from the code review. The strict build is 0/0, the layout tests pass 6/6 and the documentation group passes 36/36 at the tested revision.**

Locations are named symbolically: the **PR worktree** (a linked worktree on `feature/quick-repository-layout`, where all edits, builds and checks ran) and the **main checkout** (read-only seed source for the ignored caches).

## Tested revision

`e7ec53d36846c845480c509fe4ec098aff974c33` (short `e7ec53d`), on `feature/quick-repository-layout` in the PR worktree. This is the head after the review fixes. The plan's own head `6bd6179` passed the same gate set before the review. No commit followed the passing run. Nothing was pushed.

## Commits

| Commit | Subject |
| --- | --- |
| 75d9b8c | Merge origin/dev (Phase 4 and PR #7) into feature/quick-repository-layout |
| 6e9f260 | fix(quick-261123-fsu): resolve asset-pipeline paths through one layout map |
| 53635a9 | fix(quick-261123-fsu): rebind live code to the relocated tree |
| b929cc3 | fix(quick-261123-fsu): rebind register rows and row-baseline pins |
| 95ad3be | fix(quick-261123-fsu): regenerate asset catalog, delivery and pack index |
| 7e9d4ac | fix(quick-261123-fsu): regenerate ac-08 repository stages and caller bindings |
| 0b99549 | fix(quick-261123-fsu): name the corpses replay archive handle and rehash |
| 52594d6 | docs(quick-261123-fsu): point live docs at the relocated tree |
| 6bd6179 | docs(quick-261123-fsu): record the dev integration in STATE |
| c5ea61e | fix(quick-261123-fsu): CR-01 point the resource-roles control at the game folder |
| bfb5355 | fix(quick-261123-fsu): CR-02 map every game-root file in both layout mirrors |
| 219c875 | fix(quick-261123-fsu): CR-02 regenerate ac-08 repository stages |
| e7ec53d | fix(quick-261123-fsu): WR-01 publish to the checked physical destination |

## Results

Gate script: `.phase1/quick-261123-fsu/gates.cjs`, extracted byte-exact from the PLAN (sha256 `daa62c2f34409450785496ee65ce8eb0c1ab1adbe257ef837fa60ca1f63e0892`), run from the PR worktree root with the pinned Node v24.22.1.

| Command | Tested revision | Exit status | Counts |
| --- | --- | --- | --- |
| `gates.cjs merge` | e7ec53d | 0 (PASS) | merge commit `75d9b8c12`; 0 unmerged entries; 0 conflict markers; fix commits linear |
| `gates.cjs moves` | e7ec53d | 0 (PASS) | 206 dev-added mapped files byte-identical in the merge commit; 0 retired paths at HEAD |
| `gates.cjs history` | e7ec53d | 0 (PASS) | 418 history records equal their source side |
| `gates.cjs newhost` | e7ec53d | 0 (PASS) | 0 new machine-path lines |
| `gates.cjs lists` (from an unrelated working directory) | e7ec53d | 0 (PASS) | verification 686, gameplay-checks 16, lan-checks 32, asset-pipeline 75 |
| `gates.cjs layout` (`node --test src/godot-game/tools/repository-layout.test.cjs`) | e7ec53d | 0 (PASS) | 6 pass / 0 fail |
| `gates.cjs build` (locked restore, strict build of `KingdomZombies.slnx`) | e7ec53d | 0 (PASS) | restore locked with no HTTP GET; 0 Warning(s), 0 Error(s) |
| `gates.cjs bindings` | e7ec53d | 0 (PASS) | 423 module bindings; 9 staged plans accepted; content.validate 180; 133 pack-index rows; ac-08 rule reproduces dev's block, then every HEAD row matches |
| `gates.cjs state` | e7ec53d | 0 (PASS) | Phase 05 position, 7 quick-task rows, ub4 SUMMARY link, renewal bullet, no uby/fsu row |
| `gates.cjs docs` (`node src/godot-game/tools/verification/check.cjs --group docs`) | e7ec53d | 0 (PASS) | PASS 36/36 selected obligations; report summary `.phase1/runs/26a5bd67-8fe6-459d-9347-f3c87070f07d/summary.json` (local, deleted at cleanup) |
| `gates.cjs records` | e7ec53d | 0 (PASS) | SUMMARY has status complete, How to reproduce and the tested revision; no machine path in PLAN or SUMMARY; no quick-task record committed; tree clean apart from the two untracked quick-task folders |
| Full set `gates.cjs merge moves history newhost lists layout build bindings state docs records` (after this SUMMARY was written) | e7ec53d | 0 | all 11 gates PASS; docs again PASS 36/36, report summary `.phase1/runs/fb49a759-3408-43ed-8c77-8a1c01799358/summary.json` |
| `node .phase1/quick-261123-fsu/ac08.cjs check` (after the last C# edit was committed) | e7ec53d | 0 | committed and rebuilt `repositoryStages` sha256 `4a795aab…` BYTE-EQUAL |

Before the review, the plan head `6bd6179` passed the same full set (docs report `.phase1/runs/5ea8fdba-7a99-468f-8b0e-e303d15656be/summary.json`). The first ten gates were also run at `e7ec53d` before this SUMMARY was updated, and all passed (docs report `26a5bd67…`, above).

Earlier docs pre-run at `0b99549` (before Task 3): FAIL 32/36, all six failures `DOC_TARGET` (dev's ub4 directory link in STATE.md, `docs/content/production.md:21`, `docs/workflow/engine-defects.md:24,44,57`, `docs/workflow/setup.md:17`). Task 3 fixed exactly these and the related commands.

Read-out pins (values read out from hashed rows, not test passes): engine-generated spans `980c2a80…`, generated call sites `1900d6a9…`, layout members `7a761b17…`, engine adaptations local `392edce1…`, MTP cohort `sourceInputsSha256` `8c78e497…`.

## Conflict resolutions (live merge index)

The live index after `git merge --no-ff --no-commit origin/dev` held 81 unmerged paths, matching the research: 16 content conflicts and 65 file-location conflicts (46 under `src/godot-game/tools/phase4/`, 14 under `src/godot-game/tests/verification/`, 3 under `src/godot-game/core/`, 2 under `src/godot-game/tools/`).

| Path | Conflict | Resolution (both intents kept) |
| --- | --- | --- |
| `.planning/STATE.md` | content | dev frontmatter, position and continuity; PR's 6-column quick table with dev rows (Status `—`) plus k3u, m0w, jmu ordered by ID; PR's superseded "Review PR #4" block dropped. Task 3 finished it (below) |
| `docs/code/visibility-exemptions.json` | content | union; dev's 4 rows at `src/godot-game/core/…` and `src/godot-game/game/inspection/InspectionRoot.cs` |
| `docs/workflow/lan-foundation-checks.md` | content | PR's lan-checks commands plus dev's cleanup wording in the last row |
| `docs/workflow/verification.md` | content | PR's verification paths plus dev's "recorded" |
| `src/godot-game/tests/verification/ArchitecturePolicy.cs` | content | PR list plus dev's 4 types at their current paths |
| `src/godot-game/tests/verification/BoundaryFixture.cs` | content | dev's binary Reference; HintPath `../game/.godot/mono/temp/bin/Debug/KingdomZombies.dll` |
| `src/godot-game/tests/verification/EngineAccessPolicy.cs` | content | dev's inspection branch; tuples at `src/godot-game/game/inspection/…`, in step with `ContentNativePermissionTests` |
| `src/godot-game/tests/verification/ExemptionTests.cs` | content | dev's companion refactor; companion filter `src/godot-game/game/`; fixture `src/godot-game/game/proofs/Phase1.cs`; bypass subject `src/godot-game/core/` |
| `src/godot-game/tests/verification/ExemptionValidator.cs` | content | dev's AuthenticateRecord/MergeOrigin; generated prefix `src/godot-game/game/.godot/mono/temp/obj/Debug/generated/`; InspectionRoot under `src/godot-game/game/inspection/` |
| `src/godot-game/tests/verification/FoundationPermissionTests.cs` | content | dev's staged CheckEvent/VerifyNativeStage; path `src/godot-game/game/foundation/` |
| `src/godot-game/tools/gameplay-checks/check.cjs` | content | dev's inputManifest/environment/toolIdentity; inputs exactly `src`, `.editorconfig`, `docs/code`; dotnetLaunch cwd; current toolchain and test DLL paths; root `../../../..` |
| `src/godot-game/tools/gameplay-checks/rendered.cjs` | content | dev logic with the `src/godot-game/game` project and gameplay-checks drivers |
| `src/godot-game/tools/lan-checks/check.cjs` | content | dev's per-item timeout with the current test DLL and dotnetLaunch cwd |
| `src/godot-game/tools/lan-checks/network.cjs` | content | dev's context.project and beforeGame with the `src/godot-game/game` project |
| `src/godot-game/tools/lan-checks/rendered.cjs` | content | dev's budget call with the lan-checks input driver |
| `src/godot-game/tools/verification/check.cjs` | content | dev's full export list (21 names) with the PR's root depth `../../../..` |
| `src/godot-game/core/AppearanceState.cs`, `CharacterPresentation.cs`, `ContentPack.cs` (3) | file location | staged at git's placement with dev bytes; final location, bytes unchanged |
| `src/godot-game/tests/verification/` `AppearanceStateTests.cs`, `AudioPresentationTests.cs`, `BuildingPresentationTests.cs`, `CampLayoutTests.cs`, `CharacterBowTests.cs`, `CharacterFitTests.cs`, `CharacterPresentationTests.cs`, `ContentInspectorTests.cs`, `ContentNativePermissionTests.cs`, `GeneratedExportLayoutTests.cs`, `InventoryLayoutTests.cs`, `SessionLayoutTests.cs`, `ThemeLayoutTests.cs`, `WorldInspectorTests.cs` (14) | file location | staged at git's placement with dev bytes (final location); physical literals rebound later in `53635a9` |
| `src/godot-game/tools/workspace-cleanup.cjs`, `workspace-cleanup.test.cjs` (2) | file location | staged at git's placement with dev bytes (final location); USAGE text rebound in `53635a9` |
| `src/godot-game/tools/phase4/` `case-discovery.cjs`, `check.cjs`, `content.cjs`, `evidence.cjs`, `expected-obligations.json`, `export.cjs`, `extract-audio.py`, `extract-media.py`, `handoff.json`, `native-observer.cjs`, `python.cjs`, `python.json`, `rendered-contracts.cjs`, `rendered-process.cjs`, `required-cases.json`, `reviewed-source.targets`, `scenarios.cjs` (17) | file location | staged at git's placement with dev bytes, then moved in the merge to `src/godot-game/tools/asset-pipeline/` (same names) |
| `src/godot-game/tools/phase4/cases/` `animation`, `audio-feasibility`, `audio`, `bodies-a`, `bodies-b`, `bow-work`, `census`, `corpses`, `data-camp`, `data-campaign`, `data-foundation`, `defenses`, `export-feasibility`, `export-layout`, `export-portability`, `export-profile`, `hub`, `integration`, `items`, `main-buildings`, `media`, `provenance`, `runner`, `text-data`, `ui-camp`, `ui-inventory`, `ui-kit`, `ui-session`, `world` (29, all `.cjs`) | file location | staged at git's placement with dev bytes, then moved in the merge to `src/godot-game/tools/asset-pipeline/cases/` (same names) |

Moves in the merge commit: 187 (`src/godot-game/tools/phase4/` 46 and `production/phase4/` 20 to the asset pipeline and production assets; `src/godot-game/{assets,content,inspection}/` 24, 77 and 19 plus `export_presets.cfg` under `src/godot-game/game/`). With the 19 final git placements, all 206 dev-added files sit at their current locations.

## Task 3 work

- **Live docs (`52594d6`), path-only.** None of these files is a pack-index row or a delivery guide: `docs/content/production.md` appears in `delivery.json` only as an authoring path reference, not as a hashed guide, and the docs are `outside-validator` ac-08 rows (`sha256: null`), so no cascade applied.
  - `docs/workflow/engine-defects.md`: toolchain link to `src/godot-game/tools/verification/toolchain.json`; harness link and self-test command to `src/godot-game/tools/asset-pipeline/check.cjs`; project link to `src/godot-game/game/project.godot`.
  - `docs/workflow/setup.md`: python.json link to `src/godot-game/tools/asset-pipeline/python.json`.
  - `docs/workflow/cleanup.md`: commands only (`node src/godot-game/tools/workspace-cleanup.cjs …`, `node --test src/godot-game/tools/workspace-cleanup.test.cjs`, and the Phase 2 completion command through `src/godot-game/tools/gameplay-checks/check.cjs`). No rule moved.
  - `docs/content/production.md`: content-module link and the two `--case` commands; the harness and case-folder locations in the discovery paragraphs. The logical identities `production/phase4/published/`, `tools/phase4/expected-obligations.json` and `src/godot-game/content/expected-obligations.json` stay, as the layout rule now explains.
  - `src/godot-game/tools/README.md`: the asset-pipeline sentence is present tense.
  - `docs/architecture/repository-layout.md`: "after integration" dropped from the `tools/phase4/` and `production/phase4/` rows; `tools/workspace-cleanup.cjs` and `.test.cjs` added to the same-names row; the logical-identity rule with a link to `layout.cjs`; the integration-guide sentence in past tense.
- **STATE.md (`6bd6179`).** `last_updated`, `last_activity`, `last_activity_desc` and the "Last activity" line name 261123-fsu; dev's Phase 5 position and Phase 4 closure are unchanged; the ub4 row links `./quick/261122-ub4-prepare-for-merge-pr-7/261122-ub4-SUMMARY.md`; the `[PR #6 relocation]` concern bullet hands renewal to Phase 5, with a matching Next action clause; the PR's two 2026-11-16 paragraphs are condensed into one sentence. No 261123-fsu or 261122-uby row (the orchestrator adds the fsu row).

## Review fixes (`261123-fsu-REVIEW.md`)

The code review of `75d9b8c..6bd6179` found 2 critical, 3 warning and 3 info findings. CR-01, CR-02 and WR-01 are fixed. The others are recorded below under known gaps and follow-ups. No full case ran for the fixes. Each one was proved by calling the changed code path directly.

| Finding | Status | Commit | Fix | Proof |
| --- | --- | --- | --- | --- |
| CR-01 | Fixed | `c5ea61e` | `gameplay-checks/runner-fixture.cjs` `resource-roles` now reads the isolated project at `src/godot-game/game/project.godot` and copies the icon from `src/godot-game/game/icon.svg`. These are the paths `validateResources` uses. The fixture is an `outside-validator` ac-08 row (`sha256: null`), and no catalog, registry or module binding hashes it, so nothing was rebound. | `node src/godot-game/tools/gameplay-checks/runner-fixture.cjs resource-roles` (the child that `runner.prelaunch-source` spawns; it starts no Godot) printed `PASS current three resource roles, historical Foundation roles and two undeclared resource controls` with exit 0. The `6bd6179` bytes, run in the same module location, threw `ENOENT … isolated-input/src/godot-game/project.godot`. The `.phase2/controls/` output of both runs was deleted. |
| CR-02 | Fixed | `bfb5355`, `219c875` | `git ls-tree` showed that the a305b8b `src/godot-game/` top level and the HEAD `src/godot-game/game/` top level hold the same 15 entries. The map lacked 6 of them: `icon.svg`, `icon.svg.import`, `.editorconfig`, `packages.lock.json`, `GeneratedCallSiteAdapters.cs` and `GeneratedReportAdapter.cs`. `layout.cjs` `GAME_ENTRY` and `AssetPipelinePaths.cs` `_gameFiles` now list the same explicit names. `override.cfg` stays, as before. The asset cascade (catalog, delivery, index) regenerated byte-unchanged. After the build, `ac08.cjs write` rebound the `AssetPipelinePaths.cs` row, `sourceSetSha256` and the six `callerStages.repositorySha256` values. Caller authority and producer registries were unchanged. | Over all 2777 tracked files: `physical(f) === f`; `physical(logical(f)) === f`; `logical` and `physical` are idempotent; no logical form collides with a tracked file; every top-level entry under `src/godot-game/game/` maps. The six names resolve to existing files and map back. A direct run of the `audio.lifecycle` input steps (`audioPreviewFiles()`, `hash(bytes(ROOT,file))` for each, then the hub filter at `audio.cjs:1630`) hashed all 283 files. It hashed `src/godot-game/icon.svg` to `6c803843…`, equal to the a305b8b blob, and kept `icon.svg` among the 108 hub paths. With the `6bd6179` `layout.cjs` swapped in, the same steps threw `ENOENT … src/godot-game/icon.svg`. `ac08.cjs check` reported BYTE-EQUAL. |
| WR-01 | Fixed | `e7ec53d` | `content.cjs` `destination()` returns `inside(repo,rel)`, the physical path its escape and reparse walk checked, instead of the caller's unmapped path. Every publication write goes through that return value. Records keep their logical identities: the index paths are unchanged and no record stores the destination. `docs/content/production.md` says that `production/phase4/published/` is a logical identity that resolves to `src/godot-game/production/assets/published/`. That doc is neither a pack-index row nor a delivery guide, and its ac-08 row has `sha256: null`. The asset cascade regenerated byte-unchanged. | `destination()` was called directly, with no write, under both the `6bd6179` bytes and the fixed bytes. For `production/phase4/published/x`, the old code returned the retired `production/phase4/published/x` and the fixed code returns `src/godot-game/production/assets/published/x`. A physical input still returns itself. `.phase4/runs/x` still returns `.phase4/runs/x`. `production/phase4/x` still throws `PUBLICATION_ROOT_DENIED`. |

## Rebound, statically consistent, not executed

These bindings were regenerated and checked statically by the gates. None of the cases or behaviours below was executed in this task.

- ac-08 `repositoryStages` (498 sources, 230 checked, 15 source stages, 5 projects, 42 keys) and the 5 Phase 1 callers plus gameplay `core.permissions` (`callerStages.repositorySha256` and catalog strings).
- The producer and caller stages: `callerAuthority.sha256`, every `producerStages.registries.*.sha256`; 9 staged plans accepted by `producerPlan` without running them.
- Row-baseline pins and the MTP cohort `sourceInputsSha256` (read out, see Results).
- Every asset-pipeline case: 423 module bindings and `moduleSha256` values, `projectGraphSha256`, delivery fixture catalog and guides, and pack-index rows.
- Export: the restricted `git ls-files` pathspec, the fail-closed NuGet feed rewrite and the export-layout origin comparison through the map.
- The extractor entry point `src/godot-game/tools/asset-pipeline/run-extractor.py` and the inline Python rebinds.
- C# behaviour of the rebound path literals and of `AssetPipelinePaths`; only compilation and analyzers ran.
- Asset-pipeline `runner.prelaunch-source` (CR-01). Only its `resource-roles` child was exercised, not the case itself.
- Asset-pipeline `audio.lifecycle` and `audio.heldout` (CR-02). Only their input-hashing and hub-filter steps were exercised. The build, the tests and the native hub did not run.
- Publication through `content.publish` (WR-01). Only `destination()` was exercised.

## Known gaps (fail closed; for Phase 5 renewal)

- Captured recipes such as `SCALE_VERIFY` still name `tools/phase4/…` and `production/phase4/…`. They keep their bytes and need a replay root in the captured layout.
- Inline Python that opens logical paths read from pack data is not mapped through `layout.cjs`.
- Phase 4 dotnet child processes lack the workspace SDK working directory.
- **Captured recipes run only in the original checkout (review WR-02, not fixed by decision).** Pinned recipe bytes hard-assert the original checkout's `.phase4` folder: `target.is_relative_to(Path('<original checkout>/.phase4'))`. The audio decoder recipe also rewrites `ROOT` to the original checkout. This task derives ROOT from each tool's location, so in any other checkout (this PR worktree, CI, a second developer) these steps fail inside the recipe. Some fail after writing run-owned output. The affected steps are the export tracer (`--export-tracer` / `source.tracer`), the `audio.actions` assembly and the interface-ambience assembly. Before this task they failed fast on `PROJECT_ROOT_PIN`. The recipes keep their bytes by locked decision. Renewal belongs to Phase 5: either a replay root shim like `run-extractor.py`, or a `CAPTURED_ROOT_REQUIRED` fail-fast check before these recipes launch.

## Follow-ups (review findings not fixed in code)

- **WR-03: JS/C# map agreement test.** `layout.cjs` and `AssetPipelinePaths.cs` are hand-kept copies, and no test checks that they agree. CR-02 made them identical by hand. A node or xUnit test should run both over every `files[].path` in `pack-index.json` and every tracked file under `src/godot-game/game/`, and assert identical output.
- **IN-01: `PROJECT_ROOT_PIN` can no longer fail.** `check.cjs` derives ROOT and then compares it with the same expression. Remove the pin, or turn it into the captured-root precondition from WR-02.
- **IN-02: duplicate fingerprint keys.** The `ui-camp`, `ui-inventory`, `ui-session` and `export-profile` input fingerprints list some files under both their logical and physical names. Before/after equality still holds. Normalising keys before the `Set` would fix it.
- **IN-03: broader inherited selection.** In `scenarios.cjs` `impactClosure`, the `runtime` prefix `src` now covers every tool. A Phase 4 case-module change therefore selects every inherited Phase 2 and Phase 3 case. This is conservative. Either exclude the asset pipeline and production, or record the broadening in the selection rationale.

## Not run (by decision)

| Check | Status |
| --- | --- |
| Windows export | Not run |
| Editor-import self-test (`node src/godot-game/tools/asset-pipeline/check.cjs --self-test-editor-import`) | Not run |
| Headless launch | Not run |
| Game windows | Not run |
| Full 776-case selection | Not run |
| `node --test src/godot-game/tools/workspace-cleanup.test.cjs` (research recommendation, not in the plan) | Not run |

## Expected-blocked (renewal owned by Phase 5)

| Gate | Reason |
| --- | --- |
| Phase 4 `runner.prelaunch` | Review-bound to `5c637da`; the relocation changes its review identity and inputs |
| Phase 4 `runner.selection` | Same |
| `acceptance.judgments`, `acceptance.artifacts` (`acceptance.*`) | Same |
| Phase 4 `--verify-completion` | Review identity and `REVIEW_TARGET_PATH` change with the relocation |

Phase 4 review records keep their bytes. STATE.md records the handoff in the `[PR #6 relocation]` concern.

## Blocked by PR #7 cleanup (not by this task)

| Gate | Reason |
| --- | --- |
| Phase 4 `export.layout` | Reads ignored `.phase4` inputs deleted by the PR #7 cleanup |
| Phase 4 `export.profile` | Same |
| Phase 1 `rules.export-layout-origin` | Same |
| Phase 1 `rules.export-layout-boundary` | Same |
| `ContentNativePermissionTests` model tests | Same |

## Deviations from Plan and Research

1. **Export list count (Task 1).** The plan and research said dev's `verification/check.cjs` exports 22 names; dev's actual list has 21. The resolution keeps dev's full list unchanged with the PR's root depth.
2. **Repair commit `0b99549` (Task 2).** A rebound inline Python line in `cases/corpses.cjs` opened the archive as `as z:`, which the `newhost` check reads as a drive letter. The handle was renamed to `bundle` with identical behaviour, and the step C tail was re-run (catalog `moduleSha256`, delivery fixture catalog, pack index). ac-08 was unaffected (outside-validator row).
3. **Verification location (all tasks).** Builds and checks ran in place in the PR worktree, as the plan chose under the CONTEXT discretion. No detached verification worktree was created.
4. **Task 1 intermediate state.** Before Task 2, `normalizeCurrent` reproduced 132 of 133 index rows (all IDs and all 180 outputs); the remaining row, the game docs README, was refreshed in step C.
5. **Docs pre-run (Task 2 to Task 3).** The docs group failed 32/36 at `0b99549`; all six failures were Task 3 items and were fixed in `52594d6` and `6bd6179`.
6. **`docs/architecture/repository-layout.md` (Task 3).** Besides the two rows, "after integration" was also dropped from the purpose-folder sentence in Ownership, because it carried the same stale statement. The rule sentence names the logical prefixes as "paths under …" rather than with an ellipsis character.
7. **`docs/content/production.md` (Task 3).** Besides the link and the two commands, the two prose locations of the harness and the case folder were updated, because they name physical tool locations. Logical record identities in the same file were kept.
8. **Scratch output location (Task 3).** One generated scratch table was briefly written to the system temp folder next to the PR worktree; it was moved into `.phase1/quick-261123-fsu/` and the stray file deleted.
9. **CR-02 in two commits (review fixes).** `ac08.cjs write` rebuilds from HEAD blobs and refuses a working tree that differs from HEAD in a hashed row. The `AssetPipelinePaths.cs` edit was therefore committed first (`bfb5355`) and the regenerated ac-08 bindings second (`219c875`). `ac08.cjs check` then reported BYTE-EQUAL.
10. **CR-02 name list (review fixes).** The review proposed `Generated[A-Za-z]+\.cs`. Both mirrors instead list the two real generated files by name, so the JS regex and the C# list have the same, exact contents.
11. **WR-01 rejection rule (review fixes).** The review's sketch also rejected inputs whose physical form differs from the input. That would deny the logical `production/phase4/published/` root, which `production.md` allows. The fix only returns and writes the checked physical path, and the doc sentence it suggested was added.

## Task-created ignored paths (for end-of-task cleanup)

Per `docs/workflow/cleanup.md`, in the PR worktree:

- Delete: `.phase1/quick-261123-fsu/` (gate script, scratch generators, logs, commit messages; the orchestrator needs it for its final docs run first); this task's `.phase1/runs/` reports, including the docs pre-run `90565b4e-742a-4033-9b4e-11c93698a872` and the final docs runs `f69385a7-5a88-41ad-b9ce-a86d84eb96ce` and `5ea8fdba-7a99-468f-8b0e-e303d15656be`, the review-fix docs runs `26a5bd67-8fe6-459d-9347-f3c87070f07d` and `fb49a759-3408-43ed-8c77-8a1c01799358` (plus any earlier Task 1-2 runs in `.phase1/runs/`); `.phase1/temp/`. The review-fix proof scripts and logs are in `.phase1/quick-261123-fsu/` (`cr02-proof.cjs`, `wr01-proof.cjs`, `head-*.cjs`, `msg-cr01.txt`, `msg-cr02a.txt`, `msg-cr02b.txt`, `msg-wr01.txt`, `fix-*.log`).
- Optional delete: `bin/`, `obj/` and `.godot/` build and import caches under `src/godot-game/`.
- Keep (cleanup.md keep list): the seeded `.phase1/feed/` (4 nupkgs) and the 3 Godot package folders `.phase1/packages/{godot.sourcegenerators,godotsharp,godotsharpeditor}/4.7.2/`; `.phase1/cli-home/` and `.phase1/http-cache/`.
- The pinned Node folder `.phase1/quick-261121-jmu-node/` predates this task and is needed for the orchestrator's final docs run.
- The untracked `.planning/quick/261122-uby-…` folder is not this task's output; it holds machine paths and must stay uncommitted.

## How to reproduce

All commands run from the PR worktree root (the checkout root) in Git Bash.

1. Seed the ignored caches by copying (never moving or linking) from the main checkout, setting `MAIN_CHECKOUT` only in that shell:
   ```bash
   mkdir -p .phase1/feed
   for f in "$MAIN_CHECKOUT"/.phase1/feed/*.nupkg; do test -e ".phase1/feed/${f##*/}" || cp "$f" .phase1/feed/; cmp "$f" ".phase1/feed/${f##*/}" || exit 1; done
   for p in godot.sourcegenerators godotsharp godotsharpeditor; do test -e ".phase1/packages/$p/4.7.2" || { mkdir -p ".phase1/packages/$p" && cp -r "$MAIN_CHECKOUT/.phase1/packages/$p/4.7.2" ".phase1/packages/$p/"; }; done
   ```
2. Put the pinned Node first on `PATH` for this shell only and confirm the version:
   ```bash
   export PATH="$PWD/.phase1/quick-261121-jmu-node:$PATH" MSYS_NO_PATHCONV=1
   node --version   # v24.22.1
   ```
3. Set the isolation environment from `docs/workflow/setup.md` (`DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH`, `TEMP`, `TMP` under `.phase1/`), then run the strict build from `docs/workflow/verification.md`:
   ```bash
   cd src/godot-game
   dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false
   dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false
   cd ../..
   ```
   Expect `0 Warning(s)` and `0 Error(s)`.
4. Layout tests: `node --test src/godot-game/tools/repository-layout.test.cjs` (expect 6 pass, 0 fail).
5. Documentation group: `node src/godot-game/tools/verification/check.cjs --group docs` (expect `PASS 36/36 selected obligations`).
6. Gate script: extract it from the PLAN with Session setup S3 (sha256 `daa62c2f34409450785496ee65ce8eb0c1ab1adbe257ef837fa60ca1f63e0892`), then run:
   ```bash
   node .phase1/quick-261123-fsu/gates.cjs merge moves history newhost lists layout build bindings state docs records
   ```
   Every gate prints `PASS`. The `build` gate sets the isolation environment itself; `records` needs this SUMMARY and a tree that is clean apart from the two untracked quick-task folders.

## Self-Check: PASSED

- Files present at the tested revision: `layout.cjs`, `AssetPipelinePaths.cs`, `run-extractor.py`, `docs/architecture/repository-layout.md`, `.planning/STATE.md`.
- All thirteen commits listed above are ancestors of the tested revision.
- This SUMMARY and the PLAN, CONTEXT and RESEARCH files are untracked and uncommitted.
