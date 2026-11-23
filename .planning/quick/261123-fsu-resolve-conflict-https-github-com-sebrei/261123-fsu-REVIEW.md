---
phase: quick-261123-fsu
reviewed: 2026-11-23T12:14:34Z
depth: quick
files_reviewed: 89
files_reviewed_list:
  - .planning/STATE.md
  - docs/architecture/repository-layout.md
  - docs/code/engine-generated-spans.json
  - docs/code/generated-callsite-adaptations.json
  - docs/code/generated-layout-adaptations.json
  - docs/code/visibility-exemptions.json
  - docs/content/production.md
  - docs/content/windows-inspection.md
  - docs/workflow/cleanup.md
  - docs/workflow/engine-defects.md
  - docs/workflow/lan-foundation-checks.md
  - docs/workflow/setup.md
  - docs/workflow/verification.md
  - src/godot-game/game/content/pack-index.json
  - src/godot-game/game/content/packs/delivery.json
  - src/godot-game/production/assets/export-profile.json
  - src/godot-game/tests/verification/AppearanceStateTests.cs
  - src/godot-game/tests/verification/ArchitecturePolicy.cs
  - src/godot-game/tests/verification/AssetPipelinePaths.cs
  - src/godot-game/tests/verification/AudioPresentationTests.cs
  - src/godot-game/tests/verification/BoundaryFixture.cs
  - src/godot-game/tests/verification/BuildingPresentationTests.cs
  - src/godot-game/tests/verification/CampLayoutTests.cs
  - src/godot-game/tests/verification/CharacterBowTests.cs
  - src/godot-game/tests/verification/CharacterFitTests.cs
  - src/godot-game/tests/verification/CharacterPresentationTests.cs
  - src/godot-game/tests/verification/ContentInspectorTests.cs
  - src/godot-game/tests/verification/ContentNativePermissionTests.cs
  - src/godot-game/tests/verification/EngineAccessPolicy.cs
  - src/godot-game/tests/verification/EngineGeneratedTests.cs
  - src/godot-game/tests/verification/ExemptionTests.cs
  - src/godot-game/tests/verification/ExemptionValidator.cs
  - src/godot-game/tests/verification/FoundationPermissionTests.cs
  - src/godot-game/tests/verification/GeneratedCallSiteAdapterTests.cs
  - src/godot-game/tests/verification/GeneratedExportLayoutTests.cs
  - src/godot-game/tests/verification/GodotCallSiteAdapterTests.cs
  - src/godot-game/tests/verification/InventoryLayoutTests.cs
  - src/godot-game/tests/verification/RunnerContractTests.cs
  - src/godot-game/tests/verification/SessionLayoutTests.cs
  - src/godot-game/tests/verification/ThemeLayoutTests.cs
  - src/godot-game/tests/verification/WorldInspectorTests.cs
  - src/godot-game/tools/README.md
  - src/godot-game/tools/asset-pipeline/cases/animation.cjs
  - src/godot-game/tools/asset-pipeline/cases/audio-feasibility.cjs
  - src/godot-game/tools/asset-pipeline/cases/audio.cjs
  - src/godot-game/tools/asset-pipeline/cases/bodies-a.cjs
  - src/godot-game/tools/asset-pipeline/cases/bodies-b.cjs
  - src/godot-game/tools/asset-pipeline/cases/bow-work.cjs
  - src/godot-game/tools/asset-pipeline/cases/census.cjs
  - src/godot-game/tools/asset-pipeline/cases/corpses.cjs
  - src/godot-game/tools/asset-pipeline/cases/data-camp.cjs
  - src/godot-game/tools/asset-pipeline/cases/defenses.cjs
  - src/godot-game/tools/asset-pipeline/cases/export-layout.cjs
  - src/godot-game/tools/asset-pipeline/cases/export-portability.cjs
  - src/godot-game/tools/asset-pipeline/cases/export-profile.cjs
  - src/godot-game/tools/asset-pipeline/cases/hub.cjs
  - src/godot-game/tools/asset-pipeline/cases/integration.cjs
  - src/godot-game/tools/asset-pipeline/cases/items.cjs
  - src/godot-game/tools/asset-pipeline/cases/main-buildings.cjs
  - src/godot-game/tools/asset-pipeline/cases/media.cjs
  - src/godot-game/tools/asset-pipeline/cases/provenance.cjs
  - src/godot-game/tools/asset-pipeline/cases/runner.cjs
  - src/godot-game/tools/asset-pipeline/cases/text-data.cjs
  - src/godot-game/tools/asset-pipeline/cases/ui-camp.cjs
  - src/godot-game/tools/asset-pipeline/cases/ui-inventory.cjs
  - src/godot-game/tools/asset-pipeline/cases/ui-kit.cjs
  - src/godot-game/tools/asset-pipeline/cases/ui-session.cjs
  - src/godot-game/tools/asset-pipeline/cases/world.cjs
  - src/godot-game/tools/asset-pipeline/check.cjs
  - src/godot-game/tools/asset-pipeline/content.cjs
  - src/godot-game/tools/asset-pipeline/evidence.cjs
  - src/godot-game/tools/asset-pipeline/export.cjs
  - src/godot-game/tools/asset-pipeline/layout.cjs
  - src/godot-game/tools/asset-pipeline/required-cases.json
  - src/godot-game/tools/asset-pipeline/reviewed-source.targets
  - src/godot-game/tools/asset-pipeline/run-extractor.py
  - src/godot-game/tools/asset-pipeline/scenarios.cjs
  - src/godot-game/tools/code-rules/GeneratedLayout.cs
  - src/godot-game/tools/gameplay-checks/check.cjs
  - src/godot-game/tools/gameplay-checks/rendered.cjs
  - src/godot-game/tools/gameplay-checks/required-cases.json
  - src/godot-game/tools/gameplay-checks/runner-fixture.cjs
  - src/godot-game/tools/lan-checks/check.cjs
  - src/godot-game/tools/lan-checks/network.cjs
  - src/godot-game/tools/lan-checks/rendered.cjs
  - src/godot-game/tools/repository-layout.test.cjs
  - src/godot-game/tools/verification/check.cjs
  - src/godot-game/tools/verification/required-cases.json
  - src/godot-game/tools/workspace-cleanup.cjs
findings:
  critical: 2
  warning: 3
  info: 3
  total: 8
status: partially_fixed
fix_status:
  fixed_at: 2026-11-23
  tested_revision: e7ec53d36846c845480c509fe4ec098aff974c33
  fixed:
    CR-01: c5ea61e
    CR-02: bfb5355, 219c875
    WR-01: e7ec53d
  deferred:
    WR-02: known gap in the SUMMARY (pinned recipe bytes keep their bytes; Phase 5 renewal)
    WR-03: follow-up (JS/C# layout map agreement test)
    IN-01: follow-up
    IN-02: follow-up
    IN-03: follow-up
---

# Quick Task 261123-fsu: Code Review Report

**Reviewed:** 2026-11-23T12:14:34Z
**Depth:** quick (targeted: diff `75d9b8c..6bd6179` of the 76 fix-commit files, plus merge-resolution comparison of the 16 conflicted files; one extra auto-merged file, `gameplay-checks/runner-fixture.cjs`, was pulled in because a fix-commit file calls it)
**Files Reviewed:** 89
**Status:** partially_fixed. CR-01 was fixed in `c5ea61e`, CR-02 in `bfb5355` and `219c875`, and WR-01 in `e7ec53d`. The full gate passed at `e7ec53d`. WR-02 is recorded as a known gap in the SUMMARY. WR-03 and IN-01 to IN-03 are recorded there as follow-ups.

Locations are named symbolically: the **PR worktree** is the checkout of `feature/quick-repository-layout` at `6bd6179`; the **original checkout** is the machine-specific checkout that Phase 4 captured recipes hard-code.

## Summary

The overall rebinding is consistent. The review checked the following:

- **Root depths.** Every `__dirname` / `parents[N]` root derivation in the relocated tools is at the correct depth. That covers `asset-pipeline/{check,content}.cjs` `../../../..`, the `cases/*.cjs` files `../../../../..`, `run-extractor.py` `parents[3]` and `reviewed-source.targets` four `..`.
- **Merge resolutions.** I normalised paths on both sides and compared them. The 16 conflicted files keep both intents: dev's logic with the PR's paths and launch changes. The only residual non-path deltas are the PR's own `dotnetLaunch` cwd changes and the collapsed `inputManifest` roots. Both are legitimate because every former root input now sits under `src`.
- **Catalogs, pins and counts.** Nothing was weakened:
  - Case counts are unchanged: verification 686, gameplay 16, lan 32, asset-pipeline 75.
  - `repositoryStages` grew from 485 to 498 sources, and checked `source` rows went from 226 to 228.
  - Every dev source maps to a HEAD row with the same kind and mapped owners. The only exceptions are `.config/dotnet-tools.json` and the two cleanup tools, which moved as intended.
  - Baseline counts (220/18) are unchanged. Only the hashes were rebound.
- **Machine paths.** No line added by this task contains a machine-specific path.

Two live defects remain, both in code that was "rebound, statically consistent, not executed":

- **A retired path survives an auto-merged dev hunk** (CR-01). The resource-roles control in `gameplay-checks/runner-fixture.cjs` still uses `src/godot-game/project.godot` and `src/godot-game/icon.svg`. This deterministically breaks the `runner.prelaunch-source` case, which is not on the expected-blocked list.
- **The logical-to-physical map is incomplete** (CR-02). `layout.cjs` does not map `src/godot-game/icon.svg`, which `audio.cjs` still lists as a logical input. As a result, `audio.lifecycle` and `audio.heldout` fail before doing any work.

The SUMMARY's known gaps are acknowledged and not re-raised. One exception: WR-02 records that the SUMMARY understates the effect of captured recipes that hard-assert the original checkout root. Now that ROOT is derived, those recipes fail in every other checkout.

## Critical Issues

### CR-01: Auto-merged resource-roles control still uses retired physical game paths

**File:** `src/godot-game/tools/gameplay-checks/runner-fixture.cjs:32`, `:46`
**Issue:** Dev's `resource-roles` hunk arrived through a clean auto-merge, so it is not among the 16 conflicted files. It never got the rebinding:
```js
const project=path.join(current.isolatedInputRoot,'src/godot-game/project.godot');   // :32
fs.copyFileSync(path.join(root,'src/godot-game/icon.svg'),path.join(path.dirname(project),'icon.svg')); // :46
```
The merged `validateResources` in `gameplay-checks/rendered.cjs:303` now copies the project to `isolated-input/src/godot-game/game/project.godot`. The icon lives at `src/godot-game/game/icon.svg`. Line 33 (`fs.readFileSync(project)`) therefore throws `ENOENT` every time.

Failure scenario: `node src/godot-game/tools/asset-pipeline/check.cjs --case runner.prelaunch-source` runs `qualification()`. At `cases/runner.cjs:75-79` that function spawns `runner-fixture.cjs resource-roles`, then runs `assert.equal(resourceControl.status,0,…)`. The assertion fails on the ENOENT stack. `runner.prelaunch-source` (order 304, "Detached review, selection and runner evidence controls") is not among the SUMMARY's expected-blocked gates (`runner.prelaunch`, `runner.selection`, `acceptance.*`, `--verify-completion`), so this is an unacknowledged regression. The `moves` gate (no retired roots at HEAD) cannot see it, because `src/godot-game/` is still a live prefix.
**Fix:**
```js
const project=path.join(current.isolatedInputRoot,'src/godot-game/game/project.godot');
...
fs.copyFileSync(path.join(root,'src/godot-game/game/icon.svg'),path.join(path.dirname(project),'icon.svg'));
```
Then grep all auto-merged (non-conflicted) dev hunks for `src/godot-game/<game entry>` literals that do not go through `layout.cjs`.

### CR-02: `layout.cjs` omits game-root files that live records still use (`icon.svg`), so audio cases fail

**File:** `src/godot-game/tools/asset-pipeline/layout.cjs:12`; consumer `src/godot-game/tools/asset-pipeline/cases/audio.cjs:1387`, `:1620`, `:1630`
**Issue:** `GAME_ENTRY` maps only `assets|content|inspection|foundation|proofs|docs` and four files (`project.godot`, `export_presets.cfg`, `KingdomZombies.csproj`, `override.cfg`). The physical game folder also holds:

- `icon.svg` and `icon.svg.import`
- `.editorconfig`
- `packages.lock.json`
- `GeneratedCallSiteAdapters.cs` and `GeneratedReportAdapter.cs`

`audioPreviewFiles()` still lists the logical `'src/godot-game/icon.svg'`. `physical()` returns it unchanged, but no `src/godot-game/icon.svg` exists at HEAD. The call `lifecycle()` → `hash(bytes(ROOT,file))` at `:1620` therefore throws before the build starts. On the same list, the rebound filter at `:1630` (`physical(file).startsWith('src/godot-game/game/')`) would also silently drop the icon from the copied hub project, which changes the hub input set compared with dev.

Failure scenario: `--case audio.lifecycle` or `--case audio.heldout` fails immediately with a missing-path error. The `bindings` gate checked module hashes statically and could not catch this. `AssetPipelinePaths.Physical` (`AssetPipelinePaths.cs:73-76`) mirrors the same incomplete list, so C# callers would inherit the gap.
**Fix:** Make the map match the real game folder in both mirrors:
```js
const GAME_ENTRY = /^(?:(?:assets|content|inspection|foundation|proofs|docs)(?:\/|$)|(?:project\.godot|export_presets\.cfg|KingdomZombies\.csproj|override\.cfg|icon\.svg(?:\.import)?|packages\.lock\.json|\.editorconfig|Generated[A-Za-z]+\.cs)$)/;
```
Also update `_gameFiles` in `AssetPipelinePaths.cs`. Add a layout test that runs `logical()` over every tracked file under `src/godot-game/game/` and asserts `physical(logical(f)) === f`.

## Warnings

### WR-01: `destination()` checks the mapped physical path but returns the unmapped one

**File:** `src/godot-game/tools/asset-pipeline/content.cjs:467-471`
**Issue:** `rel` is converted with `logical()`, and the reparse/escape walk runs `inside(repo, rel)`, which maps through `physical()`. The function then returns the caller's original `absolute`.

Consider a caller that passes `<checkout>/production/phase4/published/x`. That is the logical form, and `docs/content/production.md:23` still names `production/phase4/published/` as an allowed destination. The prefix test passes, and the reparse check runs against `src/godot-game/production/assets/published/x`. `publish()` then `mkdirSync`s and writes under the retired root `<checkout>/production/phase4/`. The result is a resurrected retired folder whose reparse safety was never checked.
**Fix:** Return the checked path, and reject inputs whose physical form differs from what they name:
```js
const checked=inside(repo,rel);
need(path.resolve(checked)===path.resolve(absolute)||rel.startsWith('.phase4/'),'PUBLICATION_ROOT_DENIED',rel);
return checked;
```
Also add a sentence to `production.md:23` saying that the path is a logical identity resolved by `layout.cjs`.

### WR-02: Known gap understated. Captured recipes hard-assert the original checkout's `.phase4`, so derived roots fail in every other checkout

**File:** `src/godot-game/tools/asset-pipeline/check.cjs:28-35` (`RECIPE`, `EXPORT_PATH`); `src/godot-game/tools/asset-pipeline/cases/audio.cjs:56`, `:66`, `:172`, `:607`
**Issue:** This task replaced the hard-coded ROOT in `check.cjs:7` and `hub.cjs:5` with `path.resolve(__dirname,…)` so that "checks test the tree they live in". That made `PROJECT_ROOT_PIN` vacuous (IN-01). Several recipes that are still launched live keep pinned bytes asserting `target.is_relative_to(Path('<original checkout>/.phase4'))`. They also rewrite `ROOT` in the audio decoder to the original checkout.

Before this task, every other checkout failed fast on `PROJECT_ROOT_PIN`. Now the PR worktree, CI or a second developer gets past the pin and fails later, inside the recipe. Affected: `--export-tracer` / `source.tracer`, `audio.actions` assembly and interface-ambience assembly. Some of these runs leave run-owned output first. The SUMMARY's known gaps mention only logical `tools/phase4/…` names in captured recipes (`SCALE_VERIFY`). They do not say that these cases cannot run outside the original checkout. That statement matters for the Phase 5 handoff and for the creator's multi-user constraint.
**Fix:** Do not change the recipe bytes. Add an explicit known-gap bullet to the SUMMARY and the STATE renewal concern, and fail fast with a clear diagnostic before launching these recipes when `fs.realpathSync(ROOT)` differs from the captured root. One way is a `CAPTURED_ROOT_REQUIRED` check in the affected cases. A replay root shim, like `run-extractor.py`, is the Phase 5 fix.

### WR-03: Two hand-maintained copies of the layout map with no parity check

**File:** `src/godot-game/tools/asset-pipeline/layout.cjs:7-12`, `src/godot-game/tests/verification/AssetPipelinePaths.cs:63-76`
**Issue:** The C# summary says `Physical` "mirrors" `layout.cjs`, but no test enforces that. CR-02 shows the risk: a fix applied to one map leaves JS callers (case runners, content validation) and C# callers (`ContentNativePermissionTests`, `EngineGeneratedTests`) resolving the same logical record to different files. Hash bindings would then pass on one side and fail on the other.
**Fix:** Add a node test or xUnit test that loads both tables, or that runs both implementations over all `files[].path` in `pack-index.json` plus every tracked file under `src/godot-game/game/`, and asserts identical output.

## Info

### IN-01: `PROJECT_ROOT_PIN` is now a tautology

**File:** `src/godot-game/tools/asset-pipeline/check.cjs:572`
**Issue:** `ROOT` is defined as `path.resolve(__dirname,'../../../..')` on line 7 and then compared with the same expression. The pin can no longer fail. Before this task it guaranteed that the hard-coded root was this tool's own checkout, and that invariant is what WR-02 depended on.
**Fix:** Remove the pin, or repurpose it as the captured-root precondition described in WR-02.

### IN-02: Source-input fingerprints record the same files under logical and physical names

**File:** `src/godot-game/tools/asset-pipeline/cases/ui-camp.cjs:8` (same pattern in `ui-inventory.cjs`, `ui-session.cjs`, `export-profile.cjs` `PROJECT_FILES` plus walk)
**Issue:** `CODE='src/godot-game/inspection/CampLayouts.cs'` is logical. The walk over `src/godot-game/game` adds `src/godot-game/game/inspection/CampLayouts.cs` as a physical path. `new Set()` no longer removes the duplicate, so evidence lists the same file twice under two names. The before/after equality still holds, so this is noise, not a correctness bug. It does make `camp-inputs.json` differ structurally from dev-era evidence.
**Fix:** Normalise keys with `logical()` (or `physical()`) before the `Set`.

### IN-03: `impactClosure` runtime prefix `src` now covers every tool, so inherited selection is broader

**File:** `src/godot-game/tools/asset-pipeline/scenarios.cjs:181-183`
**Issue:** `runtime = shared || starts('src', …)`. Because `src` now contains the asset-pipeline, verification, gameplay and LAN tools, a change to any Phase 4 case module marks every Phase 2 and Phase 3 inherited case as affected. This is conservative and does not weaken anything, but it raises selection cost and diverges from dev's semantics, where `tools/phase4` did not affect inherited runtime.
**Fix:** If the dev semantics are intended, exclude `src/godot-game/tools/asset-pipeline` and `src/godot-game/production` from `runtime`. Otherwise, record the broadening in the selection rationale.

## Acknowledged (from the SUMMARY; not re-raised)

- `SCALE_VERIFY` and other captured recipes naming `tools/phase4/…` and `production/phase4/…` (`cases/media.cjs:130-152`).
- Inline Python that opens logical paths read from pack data is not mapped through `layout.cjs`.
- Phase 4 dotnet child processes lack the workspace SDK working directory (for example `cases/export-layout.cjs` and `export.cjs` `dotnet publish`/`build`, and the hard-coded default .NET install SDK paths moved from dev).
- Pre-existing machine-specific donor and checkout paths in moved, unedited lines (`check.cjs:9`, `extract-*.py`, `provenance.cjs:244`, `docs/content/fields.md`). These are out of scope per CONTEXT, except as noted in WR-02.
- Expected-blocked gates (`runner.prelaunch`, `runner.selection`, `acceptance.*`, `--verify-completion`) and gates blocked by the PR #7 cleanup.

---

_Reviewed: 2026-11-23T12:14:34Z_
_Reviewer: Claude (gsd-code-reviewer)_
_Depth: quick_
