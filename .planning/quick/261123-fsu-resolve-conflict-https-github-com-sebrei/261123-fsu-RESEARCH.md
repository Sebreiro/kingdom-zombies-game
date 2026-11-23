# Quick Task 261123-fsu: Resolve PR #6 merge conflicts with dev - Research

**Researched:** 2026-11-23
**Domain:** Git merge of a repository-layout move into a hash-bound asset pipeline (Node 24.22.1, .NET SDK 10.0.404, Godot 4.7.2 .NET)
**Confidence:** HIGH for the conflict map, move map, binding inventory and check recipe (all re-derived today from `origin/dev` a305b8b and PR head c5be170). MEDIUM for the content-index and Python items carried from 261122-uby (prototyped there, not re-run today).

Location names: **PR worktree** (the checkout of `feature/quick-repository-layout`), **main checkout** (read-only, its kept `.phase1` caches), **verification worktree** (optional, under `git config --get worktrees.root`), **pinned node** (Node 24.22.1 under the PR worktree's ignored `.phase1/quick-261121-jmu-node/`), **scratch** (system temp folder, deleted at task end).

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions

#### Use of task 261122-uby
- Creator chose: reuse its decisions and research as inputs, refresh the research against today's branches, and write a new, tighter plan. Do not execute the old plan.
- The decisions below restate what carries over, so this file stands alone.

#### Scope
- Creator chose: conflicts plus the rebinding the move requires.
- In scope: merge `origin/dev` into the PR branch; place every Phase 4 and PR #7 file at its current layout location; fix every binding the move breaks (paths, registers, catalogs, hash bindings, generated indexes, live doc links and commands); keep both sides' intent in every conflicted file.
- Out of scope: the wider live-doc sweep from 261122-uby. That means no move of worktree-creation rules out of `docs/workflow/cleanup.md`, no unrelated policy-contradiction fixes, and no clean-up of machine-specific paths in files this task only moves.

#### Integration method and publishing
- Integrate `origin/dev` with one merge commit, followed by fix commits as needed. No rebase, no `git stash`, no force-push. Do not merge the PR.
- Creator chose: after the checks pass and the review is clean, push to `feature/quick-repository-layout` and update the live PR #6 description.
- The orchestrator does the push and the description edit. Subagents never push or change GitHub.

#### Verification
- Creator chose: strict locked build with analyzers, the layout tests and the documentation checks.
- Not run, and reported as not run: Windows export, editor-import self-test, headless launch, game windows and the full 776-case selection.
- Phase 4 review-bound gates (`runner.prelaunch`, `runner.selection`, `acceptance.*`, `--verify-completion`) are expected-blocked by the relocation. Phase 4 review records stay byte-identical. Their renewal is handed to Phase 5 and recorded in STATE.md and the PR description. No new Phase 4 review here.

#### Carried from 261122-uby
- **Phase 4 path identities:** content and pack records keep `src/godot-game/…`, `production/phase4/…` and `tools/phase4/…` as stable logical identities. One asset-pipeline path map resolves them to the physical folders. The layout policy states this rule. Resource IDs, archive members and pinned recipe bytes do not change.
- **Checkout root:** Phase 4 tools hard-code the main checkout's absolute root. Where it is live code, derive the root from the tool's own location, so checks test the tree they live in. Absolute paths inside captured recipes and provenance are history and keep their bytes.
- **No machine paths (creator, 2026-11-23):** "I don't want to have path hardcoded in the repository, because repository will be used by multiple users." No line this task writes may add a machine-specific path. This binds new and edited lines only; it is not a sweep of moved files.
- **History versus live docs:** Phase 1–4 records, other quick-task records and captured provenance keep their bytes, including links that dangle after the move. The layout policy's historical-path map interprets them.
- **PR #7:** dev now carries it. Its tools, docs, catalog rows, `.gitignore` rule and STATE rows are merged with both intents kept, and its task-end cleanup rule in `docs/workflow/cleanup.md` binds this task.
- **Where work happens:** edit and commit in the PR worktree. Build and check in a detached verification worktree placed under `git config --get worktrees.root` (system temp folder if unset), checked out at the commit under test, and removed at the end. Seed its ignored caches by copying from the main checkout's kept `.phase1/{cache,feed,packages,tools,…}` folders. The 261122-uby seed folder is empty and is not a source.

### Claude's Discretion
- Content of each conflict resolution, provided both sides' intent survives.
- Commit granularity after the merge commit.
- Whether the build needs a verification worktree at all, if a cheaper isolated route meets the same check list without touching the main checkout.

### Specific Ideas (binding)
- Do not weaken, skip or broaden a validator, pin, exemption or expected count to get a pass. Rebind only what a move requires.
- Use tools and caches already on this machine. No downloads or installs; `git fetch origin` is the only network step besides the final push.
- The main checkout, other worktrees and other branches are read-only. Copy from them; never move or link.
- Quick-task records name locations symbolically. No real machine paths in anything committed.
- This task's artifacts and its STATE.md row live on the PR branch, committed after the merge.
- Commit messages end with `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`.

### Deferred Ideas (OUT OF SCOPE)
- None listed in CONTEXT.md beyond the out-of-scope list above.
</user_constraints>

## Project Constraints (from AGENTS.md and the docs it indexes)

- Follow the phase branch and PR rules in `docs/workflow/ai-collaboration.md`, and the task-end rules in `docs/workflow/cleanup.md` (dev side): record evidence in committed records before deleting ignored outputs; PR description gets a **How to reproduce** section; stop processes the task started; no persistent `PATH` changes; remove task worktrees in the same session.
- `cleanup.md` forbids `git worktree remove --force` without creator approval; the supported route is `node <cleanup tool> --apply --worktrees-only --remove-worktree <path>`, which refuses worktrees whose HEAD is not on a remote branch.
- Node runners start from the repository root; direct .NET commands start from `src/godot-game` (layout policy, "Commands and generated state").
- `.gitattributes` is `* text=auto eol=lf`; C# analyzers run with `TreatWarningsAsErrors` and `EnforceCodeStyleInBuild` (`src/godot-game/Directory.Build.props:6-8`), including a 120-column rule.

## Summary

Today's merge differs from the 261122-uby picture in one structural way. PR #7 added files directly under `tools/` (`tools/workspace-cleanup.cjs` and its test). Git now detects a whole-directory rename `tools/` → `src/godot-game/tools/`, so the 46 Phase 4 tool files become **file-location conflicts placed at `src/godot-game/tools/phase4/`**, a wrong target that still needs a second move to `asset-pipeline/`. The prior figures were "31 conflicts plus 187 silently misplaced". Today they are 81 conflicted paths (16 content + 65 file-location) plus 141 silently misplaced files. Either way, the same 187 Phase 4 files need a final move.

The merge itself is mechanical. The cost lies in the rebinding cascade, and almost all of it comes from **dev's Phase 4 additions written against old paths**. The PR's own catalog never had `producerStages`, `callerStages`, `repositoryStages` or `callerAuthority`, so the whole ac-08 block (485 source rows, 42 keys, 5 projects) arrives with old paths and must be regenerated, not path-mapped. The same holds for 222 span rows and 24 call-site rows in two registers, the asset catalog, the content index, two export bindings and about 255 permission tuples in each of two C# test files.

The narrowed check list (strict build, layout tests, docs group) proves only that the code compiles, that the layout holds and that live docs resolve. It does **not** execute ac-08, the producer and caller stages, or any asset-pipeline case. The plan should therefore confirm each regenerated binding with the **existing exported validator functions**: `repositoryPlan`, `callerPlan`, `producerPlan`, `content.validate`/`normalizeCurrent`, `scenarios.loadCatalog` and `text-data.validateSupport`. These run in seconds from Node, and the report should label them "statically validated, not executed".

**Primary recommendation:** Task 1 is the mechanical merge commit (16 resolutions plus 187 moves). Task 2 rebinds in cascade order: code and roots → registers → C# pins → **strict build** → asset catalog → delivery → pack-index → Phase 1 ac-08 → caller hashes → docs. Task 3 covers STATE and records, then the docs check, then the summary. Build in place in the PR worktree after copying 4 feed packages and 3 Godot package folders from the main checkout (about 30 MB). No verification worktree is needed.

## 1. Conflict map

Command: `git merge-tree --write-tree origin/feature/quick-repository-layout origin/dev` gives result tree `7b2a9f4`, 81 conflicted paths and 81 `CONFLICT` lines: `content` 16, `file location` 65. Against `1a14684` (before PR #7) the same command gives 31, which matches 261122-uby. PR #7 did not touch any conflicted code file: `git diff --quiet 1a14684 a305b8b -- tests src docs/code tools/phase1/check.cjs tools/phase2/check.cjs tools/phase2/rendered.cjs tools/phase3 tools/phase4 production` exits 0. The 261122-uby per-file code resolutions therefore still hold [VERIFIED: merge-tree + git diff this session].

### 1a. Content conflicts (16)

| Path (merged location) | PR side | dev side | Resolution keeping both |
|---|---|---|---|
| `.planning/STATE.md` (4 hunks) | Phase 4 "planning" frontmatter; 6-column quick table (we2, k3u, m0w, jmu); cleanup continuation paragraphs; stale "Review PR #4" | Phase 5 `ready_to_plan` frontmatter with `state_head`; 5-column table (we2, 15c, 09v, ub4); Phase 4 closure and "plan Phase 5" | See §6 |
| `docs/code/visibility-exemptions.json` | path rebinds | +4 rows (AppearanceState, CharacterPresentation, ContentPack, InspectionRoot) | Union; dev rows → `src/godot-game/core/…` and `src/godot-game/game/inspection/InspectionRoot.cs` |
| `docs/workflow/lan-foundation-checks.md` **(new, PR #7)** | 10 command rows → `node src/godot-game/tools/lan-checks/check.cjs …` | last row reworded "…before [cleanup](cleanup.md); the saved Phase 3 record's local reports are no longer kept" | PR commands + dev's last-row wording |
| `docs/workflow/verification.md` **(new, PR #7)** | one line with `src/godot-game/tools/verification/check.cjs` (×2) | same line, "retained" → "recorded" | PR paths + dev's "recorded" (word-diff shows exactly these 3 tokens) |
| `tests/verification/ArchitecturePolicy.cs`, `FoundationPermissionTests.cs` | path rebinds | +4 Phase 4 types / staged `CheckEvent`/`VerifyNativeStage` | dev structure + PR paths; dev's 4 types at `src/godot-game/core/…`, `…/game/inspection/InspectionRoot.cs` |
| `tests/verification/BoundaryFixture.cs` | fixture targets `../game/KingdomZombies.csproj` | binary `<Reference>` HintPath `../godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll` | keep dev's reference approach; HintPath → `../game/.godot/mono/temp/bin/Debug/KingdomZombies.dll` |
| `tests/verification/EngineAccessPolicy.cs` | `…/game/proofs/` | new `inspection` branch, 255 `src/godot-game/inspection/…` tuples | dev logic; tuples → `src/godot-game/game/inspection/…` **in step with** `ContentNativePermissionTests` (same tuples; `evidence.cjs` `guardedHostTuples` compares them) |
| `tests/verification/ExemptionTests.cs` (4 hunks) | path rebinds, JsonNode API | `CompanionBaseline`/`CompanionBody` refactor | dev structure + PR prefixes; companion filter must stay `src/godot-game/game/` (merged line 1278 has it, line 1281 has bare `src/godot-game/`; check that `.Take(count: 16)` at 1316 selects game rows); fixture `src/godot-game/proofs/Phase1.cs` (lines 1012, 1352) → `…/game/proofs/Phase1.cs` |
| `tests/verification/ExemptionValidator.cs` | inline loop, `game/.godot` prefix | `AuthenticateRecord`/`MergeOrigin` refactor | dev structure; generated prefix `src/godot-game/game/.godot/mono/temp/obj/Debug/generated/` (dev literals at 272–335, 561); `InspectionRoot` path (648) → `…/game/inspection/` |
| `tools/gameplay-checks/check.cjs` (3 hunks) | inline `inputs:['src','.editorconfig','docs/code']`, `dotnetLaunch` cwd, root `../../../..` | `inputManifest()`, `environment()`, `toolIdentity()` with old paths | keep dev functions; `inputManifest` returns `['src','.editorconfig','docs/code']` (do **not** append dev's `tools/phase1/*` caller inputs; they now sit under `src` and `snapshot()` rejects overlap with `INPUT_DUPLICATE`); `toolIdentity` uses `dotnetLaunch(root,'dotnet',[]).cwd` + new toolchain/test-dll paths; root `../../../..` |
| `tools/gameplay-checks/rendered.cjs` (4), `tools/lan-checks/{check,network,rendered}.cjs` | `game` project path, driver paths | `beforeGame`, `context.project`, per-item timeout, `context.budget?.()` | dev logic with `src/godot-game/game` project and `src/godot-game/tools/…` drivers (unchanged from 261122-uby §1) |
| `tools/verification/check.cjs` (1 hunk, exports + root) | 6 exports, root `../../../..` | 22 exports, root `../..` | dev export list + PR root depth. Also rebind dev's **non-conflicted** literals in the same file: `tools/phase1/required-cases.json` in `callerRepository`/`callerExpected`/`callerPlan`, the `spec.catalog` comparisons (`tools/phase1/…`, `tools/phase2/required-cases.json`) — together with the catalog rows (§3) |

### 1b. File-location conflicts (65): accept git's placement, then move

| Group | Count | Git places at | Correct? |
|---|---|---|---|
| `src/core/{AppearanceState,CharacterPresentation,ContentPack}.cs` | 3 | `src/godot-game/core/` | yes, keep **bytes unchanged** (ContentPack checks logical prefixes) |
| dev-added tests `tests/phase1/*.cs` | 14 | `src/godot-game/tests/verification/` | yes; edit only physical file reads (§3) |
| `tools/workspace-cleanup.cjs`, `.test.cjs` (PR #7) | 2 | `src/godot-game/tools/` | yes, matches the layout map row "`tools/dotnet.cjs`, `tools/repository-layout.test.cjs` → same names under `src/godot-game/tools/`" and "All current tools belong in the game workspace" [VERIFIED: docs/architecture/repository-layout.md] |
| `tools/phase4/**` | 46 | `src/godot-game/tools/phase4/` | **no** → `src/godot-game/tools/asset-pipeline/` |

All 65 are stage-3-only entries (dev bytes). Resolve with `git add <path>`, then `git mv src/godot-game/tools/phase4 src/godot-game/tools/asset-pipeline`. An unmerged entry cannot be `git mv`'d.

### 1c. Silently misplaced (clean merge, wrong place): 141

Retired-root files in tree `7b2a9f4`: `production/phase4/` 20, `src/godot-game/assets/` 24, `src/godot-game/content/` 77, `src/godot-game/inspection/` 19, `src/godot-game/export_presets.cfg` 1. With the 46 from §1b, that gives 187 moves. The post-move tree has 2,774 files and no collision (scratch `virt.cjs` over `git ls-tree -r 7b2a9f4`) [VERIFIED].

Files dev modified that the PR only moved merge cleanly at the new paths with dev bytes. 261122-uby lists them (`GeneratedLayout.cs` suffixes need a rebind, §3). `.gitignore` auto-merges to PR #7's `/.phase[0-9]*/`, so nothing needs to be done there.

## 2. Move map (confirmed against the PR's layout policy and integration guide)

| Retired (dev) | Current | Source of rule |
|---|---|---|
| `tools/phase4/…` | `src/godot-game/tools/asset-pipeline/…` | layout policy table + integration guide [VERIFIED: read both this session] |
| `production/phase4/…` | `src/godot-game/production/assets/…` | same |
| `src/godot-game/{assets,content,inspection}/…`, `src/godot-game/export_presets.cfg` | `src/godot-game/game/…` | same ("Corresponding paths under `src/godot-game/game/`") |
| `src/core/…` | `src/godot-game/core/…` | same |
| `tests/phase1/…` | `src/godot-game/tests/verification/…` | same |
| `tools/workspace-cleanup{,.test}.cjs` | `src/godot-game/tools/…` | layout policy (git's suggestion agrees) |

Post-move audit: `git ls-files tools production src/core src/multiplayer-abstractions tests src/godot-game/tools/phase4 src/godot-game/assets src/godot-game/content src/godot-game/inspection src/godot-game/export_presets.cfg` must print nothing.

## 3. Rebinding cascade

### 3a. What breaks (counts from tree `7b2a9f4`)

| Binding | What dev brings with old paths | Treatment | Regenerate with | Statically validate with (seconds, Node) |
|---|---|---|---|---|
| Asset-pipeline roots and depths | `check.cjs:6` and `cases/hub.cjs:5` absolute root; `content.cjs:6` `'../..'`; `cases/{export-profile,runner}.cjs` `'../../..'`; `check.cjs:571` `PROJECT_ROOT_PIN` `'../..'` | §4 | edit | `node src/godot-game/tools/asset-pipeline/check.cjs --list` from an unrelated cwd → 75 |
| Logical-identity map | content/pack records use `src/godot-game/…`, `production/phase4/…`, `tools/phase4/…` | add one map module (e.g. `asset-pipeline/layout.cjs`) used by `content.cjs inside()` and `check.cjs inside()` | edit | `content.normalizeCurrent(root)` vs committed `pack-index.json`, and `content.validate(root)` (expect 180 valid after the cascade) [CITED: 261122-uby prototype: 132/133 hashes, all 133 IDs, 180 outputs unchanged] |
| `docs/code/engine-generated-spans.json` | 222 `.local[].surface` at `src/godot-game/.godot/mono/…` (PR rows: 220 at `…/game/.godot`) | rebind **local** rows only; `producerInventories`/`producerSources` are captured and keep bytes | edit | `producerPlan` (below) |
| `docs/code/generated-callsite-adaptations.json` | 24 `.local[].path/surface` | same | edit | same |
| `docs/code/generated-layout-adaptations.json` | 6 `exportSources[].path` | live path/suffix/owner → new; captured `surface` (`.phase4/runs/…`) keeps bytes | edit | — |
| `docs/code/visibility-exemptions.json` | 4 dev rows | §1a | edit | — |
| `src/godot-game/tools/code-rules/GeneratedLayout.cs:50-60` | 6 export suffixes `/src/core/obj/Export…`, `/src/godot-game/.godot/…`, `/src/multiplayer-abstractions/obj/…` | → `/src/godot-game/{core,game/.godot/mono/temp,multiplayer-abstractions}/obj/…`; wrap to 120 columns | edit | strict build (analyzer project compiles); export not run |
| C# row-baseline pins | `de69b0e4…` (EngineGeneratedTests ×5), `ef8ea1e0…` (EngineGeneratedTests ×3, ExemptionTests:1261, ExemptionValidator:249, GeneratedCallSiteAdapterTests:22, GodotCallSiteAdapterTests:31), `a83746c3…`/`8cce39bc…` (ExemptionValidator:243/245) | recompute only where the PR's 220/16-row rebind changed the hashed rows | after the build, run each pin's test method directly and read the actual value from the Shouldly message (below) | that test passes |
| C# tests reading files (14 moved + conflicted) | ~255 tuples each in EngineAccessPolicy and ContentNativePermissionTests; physical reads like `"src/godot-game/" + path` (ContentInspectorTests:969, ContentNativePermissionTests:2162), the DLL at ContentNativePermissionTests:76, `src/godot-game/content` in CharacterPresentationTests:320/331 | **classify each literal**: physical (file reads, analyzer keys, tuples) → new path; logical (passed to `ContentPack.ResourcePath`, compared with pack records, e.g. ContentNativePermissionTests:163/169) → keep. Paths read *from* pack JSON and then opened (ContentInspectorTests:73/110/146, ContentNativePermissionTests:1364/1730) need the same map as layout.cjs (one small C# helper) | edit | strict build compiles; behavior not executed |
| `asset-pipeline/required-cases.json` (asset catalog, 452 retired refs) | `sourceBinding.module` = `tools/phase4/cases/…`; `validateSupport` checks the `tools/phase4/cases/` prefix | modules → physical `src/godot-game/tools/asset-pipeline/cases/…` + `moduleSha256` (recompute **after** all case-module edits); fixture `resources[].path`/`delivery.fixtureCatalog.sourcePath` stay logical | script | `require('…/scenarios.cjs').loadCatalog(root, registered)` raises `SCENARIO_MODULE`/`SCENARIO_DISCOVERY_DRIFT` if stale; `text-data.validateSupport` |
| Export profile | `production/phase4/export-profile.json:14` and `cases/export-profile.cjs:13` pin `projectGraphSha256` `717bc7e8…` = sha256(dev `tools/phase1/projects.json`); PR's `src/godot-game/tools/verification/projects.json` hashes to `8e7748d8…` | recompute both from the PR's projects.json; `resources[]` stay logical; `export-profile.cjs:98` reads `tools/phase1/projects.json` → new path | edit | export not run |
| Live docs embedded in `delivery.json` | `docs/content/windows-inspection.md` (commands :22–23), `docs/content/README.md`, `src/godot-game/docs/README.md` (logical; physical `game/docs/README.md` merged hash `0088543a…` ≠ indexed `ee69ab8e…`) | edit docs first | — | — |
| `content/packs/delivery.json` → `content/pack-index.json` (133 files) | rows for the 3 docs above, `production/phase4/{listening-route.md,export-profile.json}`, `tools/phase4/required-cases.json`, `delivery.json` | keep logical paths and IDs (`file.` + sha256(path)[:20]) | `content.normalizeCurrent(ROOT)` (byte-reproducible on dev per 261122-uby); delivery: pattern of the main checkout's `.phase4/temp/04-24-refresh-live-catalog.cjs` is **gone** (main `.phase4` deleted), so rebuild with gzip level 9 in a scratch script | `content.validate` = 180 valid; `docs/content/fields.md` regenerated by `text-data` `authorDocumentation` must be **byte-unchanged** |
| `src/godot-game/tools/verification/required-cases.json` (Phase 1 catalog) | ac-08 `repositoryStages`: 398 `sources[].path`, 239 `owners[]`, 210 `stages[].sources[]`, 42 `keys[]`, 4 `projects[]`; 5 `callerStages.catalog` = `tools/phase1/required-cases.json` | **regenerate** ac-08 (§3c), then `callerStages.repositorySha256` ×5 here + ×1 in gameplay-checks, `catalog` strings, `callerAuthority.sha256`, `producerStages.registries.*.sha256` | scratch script | `const c=require('./src/godot-game/tools/verification/check.cjs'); c.repositoryPlan(item,root); c.callerPlan(item,root); c.producerPlan(item,root)` on each relevant case (all exported on dev) |
| gameplay-checks catalog | `core.permissions.callerStages` (`repositorySha256`, `catalog` = `tools/phase2/required-cases.json`) | update both | script | `callerPlan` with gameplay item; `node …/gameplay-checks/check.cjs --list` |
| Live docs (links, commands) | 6 broken live links (file-only audit, §7 P3): `.planning/STATE.md` ub4 row (dev latent), `docs/content/production.md:21`, `docs/workflow/engine-defects.md:24,44,57`, `docs/workflow/setup.md:17`; retired commands in `cleanup.md:19,45,59,60,74`, `engine-defects.md:53`, `production.md:23,33,43–49`, `windows-inspection.md:22–23`; `repository-layout.md:40–41` "after integration"; `tools/README.md:37` "Integrate it as `asset-pipeline/`"; `workspace-cleanup.cjs:12` USAGE | path-only edits; state the logical-identity rule in `repository-layout.md` (locked) | edit | docs group |
| `asset-pipeline/evidence.cjs` `TARGETS`/`SOURCE_ROOTS`, `reviewed-source.targets` depth (`'..','..'` → 4 levels), Python `parents[2]` extractors, PowerShell archive literals, export `git ls-files src` pathspec, NuGet feed rewrite in `export.cjs`/`export-layout.cjs` | live, but used only by Phase 4 cases outside the narrowed list | per 261122-uby §2 and §4 (still accurate; dev code unchanged) | edit | not executable in the narrowed list → report as "rebound, unexecuted" |

**Pin readout technique** (no new tool): after a green build, run `dotnet src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-method <FullyQualifiedMethod> --no-ansi` from the workspace with the isolation env. Take the actual hash from the failure text, then rebuild. This changes C# bytes, so it must happen **before** ac-08 regeneration.

### 3b. Order (each later step hashes earlier outputs)

1. Merge commit (moves plus resolutions). Do not edit content in it beyond conflict resolution.
2. Code: asset-pipeline map and roots, depths, the C# path literals, `GeneratedLayout.cs`, `check.cjs` literals, `workspace-cleanup` USAGE.
3. Registers: local rows, visibility union. These feed ac-08 `bindings`, `callerAuthority`, `producerStages.registries` and the C# pins.
4. C# pins (readout), then **strict locked build green**. From here on, any C# or `.editorconfig` edit restarts step 7.
5. Export-profile `projectGraphSha256` (JSON + module).
6. Asset side: every `.cjs` edit final → catalog `moduleSha256` → docs embedded in delivery → `delivery.json` → `pack-index.json` → confirm `fields.md` unchanged → `content.validate` 180.
7. Phase 1 ac-08 regeneration → `repositorySha256` (5 + 1) → `callerAuthority`/`producerStages` hashes → static `repositoryPlan`/`callerPlan`/`producerPlan` pass.
8. Live docs (`.md` edits do not cascade into ac-08: docs are `outside-validator` rows with `sha256: null`, only their path set counts) → STATE.md → docs group.

`.planning/**` is skipped by the ac-08 source rule, so STATE and SUMMARY edits never touch ac-08. **But** `.planning/STATE.md` is a Phase 1 runner input (`required-cases.json` `inputs`) and an owned doc. Finish it before the docs run.

### 3c. ac-08 regeneration rule (verified against dev)

`repositoryPlan` (dev `tools/phase1/check.cjs:173-213`) and C# `RepositoryShape` require:
- `sources[].path` = `git ls-files` filtered by `SourceFileFormat.IsChecked` (skip `.claude/`, `.codex/`, `.agents/`, `.planning/`; drop binary extensions **but keep `.import`**), sorted ordinal. Kind: `.cs` → `source`; `.editorconfig` → `configuration`; all else `outside-validator`. `sha256` = raw bytes for non-outside rows, else `null`. Owners: project-directory prefix plus the test csproj's linked `Compile Include` items (12 `game/foundation` and 6 `multiplayer-abstractions` links visible in the PR csproj).
- `sourceSetSha256 = sha256(JSON.stringify(sources))`. `stages[]` are the non-outside sources in order, in chunks of 16. `projects` = the 5 new csproj paths (must equal C# `ProjectEvaluator.Expected`). `bindings` = 7 register files with current sha256. `keys` = 42 sorted `ExemptionValidator.ReadPermissions()` keys. These are dev keys with the path map applied, re-sorted ordinal, and must still be exactly 42.
- `callerStages.repositorySha256 = sha256(JSON.stringify(repositoryStages))`.

A scratch check reproduced dev's set: 485 declared, 484 by the rule without `.import` (missing only `src/godot-game/icon.svg.import`), 0 sha mismatches, `sourceSetSha256` `22ad7a50…` matches, and sha256(JSON.stringify(spec)) = `810bd84a…` = dev's `repositorySha256` [VERIFIED: scratch `ac08set.cjs` against `origin/dev`]. Cohort `phase1-baseline-mtp` `sourceInputsSha256` reads the live test csproj, which the PR changed. Rebind it per 261122-uby §3 [CITED: 261122-uby, not re-run].

## 4. Checkout-root repair (asset pipeline, after the move)

| Location (dev line) | Kind | Minimal change |
|---|---|---|
| `check.cjs:6` `ROOT='<main-checkout-root>'`, `:571` `PROJECT_ROOT_PIN` vs `'../..'` | live | `const ROOT=path.resolve(__dirname,'../../../..')`; pin compares against the same expression (keeps the realpath check) |
| `cases/hub.cjs:5` `ROOT='<main-checkout-root>'`, `:6` assembly `tests/phase1/bin/…` | live | `path.resolve(__dirname,'../../../../..')`; assembly → `src/godot-game/tests/verification/bin/Debug/net10.0/…` |
| `content.cjs:6` `'../..'`; `cases/export-profile.cjs:3`, `cases/runner.cjs:9` `'../../..'` | live depth | `'../../../..'` / `'../../../../..'` |
| PowerShell `OpenRead('<main-checkout-root>/.phase1/cache/Godot_v4.7.2-stable_mono_win64.zip')` in `bodies-a:167`, `bodies-b:126`, `hub:465`, `main-buildings:235`, `ui-inventory:149`, `ui-kit:239,419` | live harness command | build from `runner.inside(ROOT,'.phase1/cache/'+file)` with `'`→`''` escaping, exactly as `export-profile.cjs:189-192` already does |
| `check.cjs:25-34` tracer RECIPE (donor and `.phase4` asserts); `audio.cjs:56,66,172,607`; `extract-media.py`, `extract-audio.py` (bytes pinned by `common.json`/`audio-feasibility.json` recipe sha) | **captured** | keep bytes; authoring paths fail closed elsewhere (acceptable) |
| `check.cjs:8` DONOR, `provenance.cjs:244` (negative test input), `python.json` `executable` (host interpreter pin under the user profile) | external, not a checkout root | keep; out of scope (no sweep) |

Every edited line must contain no machine path (locked).

## 5. Verification recipe (narrowed list)

### Where: build in place in the PR worktree (discretion; recommended)

The PR worktree already has `.phase1/{packages,cli-home,http-cache}`. Of the 42 lock and tool packages it lacks only `godot.sourcegenerators/4.7.2`, `godotsharp/4.7.2`, `godotsharpeditor/4.7.2` (and `dotnet-coverage/18.12.0`, a tool the build and docs runner never restore). The main checkout lacks only `dotnet-coverage` [VERIFIED: scratch scan of all 5 `packages.lock.json` + `.config/dotnet-tools.json` against both caches]. The runner inputs are fixed paths (`src`, `docs`, `specs`, `AGENTS.md`, named `.planning` files, `.editorconfig`, `README.md`), and the docs validator reads `git ls-files` only. The untracked `261122-uby` folder therefore does not affect results. This route meets the same check list without touching the main checkout, and it avoids the worktree-removal conflict (§7 P9). Disk: 9.9 GB free on the PR worktree's drive (`df -h`).

Seed (copy, never link; byte copies, since restore re-verifies hashes):
- main checkout `.phase1/feed/*` (4 nupkgs, 3.4 MB) → PR worktree `.phase1/feed/` (NuGet.config maps Godot IDs to `../../.phase1/feed`)
- main checkout `.phase1/packages/{godot.sourcegenerators,godotsharp,godotsharpeditor}/4.7.2/` (≈27 MB) → PR worktree `.phase1/packages/`

If the creator prefers the locked CONTEXT route, use a detached verification worktree at `<worktrees.root>/kz-261123-fsu-verify` at the tested commit. Seed `.phase1/{packages,feed,cli-home,http-cache}` from the main checkout (≈680 MB) and remove it after push with `node src/godot-game/tools/workspace-cleanup.cjs --apply --worktrees-only --remove-worktree <path>`. Before push the tool refuses, because the detached HEAD is not on a remote branch.

### Pinned node on PATH (process-only, never persistent)

- PowerShell: `$env:PATH = (Resolve-Path '.phase1/quick-261121-jmu-node').Path + ';' + $env:PATH; node --version` → `v24.22.1`
- Git Bash: `export PATH="$PWD/.phase1/quick-261121-jmu-node:$PATH"`

The runner rejects any other version (`process.versions.node !== pins.node` → `TOOL_MISMATCH`). The pinned node lives in a non-durable `.phase1` subfolder, so the task-end cleanup deletes it. Finish all checks first.

### Commands (repository root, PowerShell; isolation block from `docs/workflow/setup.md`)

| Check | Command | Expect |
|---|---|---|
| Isolation | setup.md PowerShell block (`DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH`, `TEMP`, `TMP` → `.phase1/*`) | — |
| Strict locked restore | `cd src/godot-game; dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false` | exit 0, no `GET https` lines |
| Strict build with analyzers | `dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false; cd ../..` (verification.md) | `0 Warning(s)`, `0 Error(s)` |
| Layout tests | `node --test src/godot-game/tools/repository-layout.test.cjs` (6 tests) | all pass. Required test edits, which strengthen and do not weaken: add `asset-pipeline` to the purpose-folder list and the unrelated-cwd `--list` loop; assert no `tools/phase4/check.cjs` and no root `tools/workspace-cleanup.cjs` |
| Moved tool self-test (recommended) | `node --test src/godot-game/tools/workspace-cleanup.test.cjs` | dev's cleanup.md says these 14 tests were not rerun on the current script; a failure that dev's bytes share is pre-existing, so report it and do not fix it |
| Documentation checks | `node src/godot-game/tools/verification/check.cjs --group docs` | `PASS 36/36` (dev and PR both 36 docs cases); the runner does its own locked restore and test-project build |

Phase 4 checks expected-blocked (report, do not run): `runner.prelaunch`, `runner.selection` (bound to `5c637da`, which is not an ancestor of squash `1a14684`; they already fail on dev per 261122-ub4 SUMMARY), `acceptance.judgments`/`acceptance.artifacts`, `--verify-completion` (review identity and `REVIEW_TARGET_PATH` change with the relocation). Also blocked by PR #7's cleanup rather than by this task (cleanup.md known exceptions): `export.layout`, `export.profile`, `rules.export-layout-origin/boundary`, `ContentNativePermissionTests` model tests. Not run by decision: export, editor-import self-test, headless launch, game windows, 776-case selection.

## 6. STATE.md merge

| Part | Take |
|---|---|
| Frontmatter | dev's (Phase 05, `ready_to_plan`, `state_head`); update only `last_updated`, `last_activity`, `last_activity_desc` for 261123-fsu |
| "Last activity" line | rewrite for 261123-fsu |
| Quick-task table | union in one format (recommend PR's 6-column with Status; `—` where unknown): we2, k3u, m0w, 15c, 09v, jmu, ub4, **fsu**. No row for 261122-uby (never executed). **Fix dev's ub4 link** `./quick/261122-ub4-prepare-for-merge-pr-7/` → `…/261122-ub4-SUMMARY.md`: the docs validator's inventory is tracked *files* only (`SourceFileFormat.TrackedPaths` = `git ls-files`; `DocumentationLinks.CheckTarget` → `DOC_TARGET`), so a directory link fails [VERIFIED: read both files]. The fsu row must link to a file tracked **before** the docs run (e.g. the committed PLAN, or the SUMMARY if it is committed before the run) |
| Session Continuity | dev's Phase 4 closure + "Next action: plan Phase 5" paragraphs; drop PR's stale "Review PR #4" and "--only 3" lines; condense PR's two cleanup-continuation paragraphs to one sentence: "PR #6 integrated `dev` (Phase 4 + PR #7) by 261123-fsu" |
| Phase 5 renewal handoff | one bullet under `### Blockers/Concerns`: after the relocation, Phase 4 `runner.prelaunch`, `runner.selection`, `acceptance.*` and `--verify-completion` are expected-blocked; renewal against the relocated tree is owned by Phase 5; Phase 4 records keep their bytes. Also one clause in the Session Continuity "Next action" paragraph. Same text in the PR description |

## 7. Pitfalls

| # | Pitfall | Avoid |
|---|---|---|
| P1 | Taking git's `src/godot-game/tools/phase4/` as final | second move to `asset-pipeline/`; layout test asserts it |
| P2 | Hash cascade invalidated by a late edit (C# fix after ac-08; module edit after catalog; doc edit after delivery) | follow §3b; re-run the static validators after the last edit of each layer |
| P3 | Directory links in owned docs (dev's STATE ub4 row) fail `DOC_TARGET` | link to files |
| P4 | STATE.md is a runner input: editing it after the docs run makes the run's input hash stale | commit STATE before the final docs run; SUMMARY (not an input, not in ac-08) after |
| P5 | Line endings / BOM (`* text=auto eol=lf`, system `core.autocrlf=input`; JSON parsers reject BOM) | write with Node or the Edit tool; never PowerShell `Set-Content`/`Out-File` for repo files |
| P6 | Git Bash path conversion | `MSYS_NO_PATHCONV=1` for `git show REV:path`; for env vars passed to `dotnet`, use the PowerShell isolation block (MSYS may rewrite `/c/...` values) |
| P7 | 120-column analyzer rule after lengthening literals (≈12 lines estimated by scratch scan; plus fluent-chain indent SCR0009 seen in the PR's own history) | wrap by hand; rebuild |
| P8 | Logical vs physical literal confusion (rewriting pack identities changes 133 resource IDs; archive-bound packs cannot change) | per-literal classification (§3a); never edit ZIP-bound packs or pinned extractors |
| P9 | Worktree removal: `cleanup.md` forbids `--force` without approval; the cleanup tool refuses unpushed detached HEADs | prefer in-place build (§5); otherwise remove after push with the cleanup tool |
| P10 | `gameplay-checks` inputs overlap (`INPUT_DUPLICATE`) if dev's caller extras are appended | §1a row |
| P11 | Machine paths in new lines | derive roots; name locations symbolically in records |
| P12 | Untracked `261122-uby` folder holds machine paths | never `git add -A`; stage explicit paths |
| P13 | `.gitignore` | nothing to do; PR #7's `/.phase[0-9]*/` merged cleanly and covers `.phase4` |

## Code Examples

```js
// Asset-pipeline path map (one module; idempotent). Shape carried from the 261122-uby prototype.
const PREFIX=[['tools/phase4/','src/godot-game/tools/asset-pipeline/'],
  ['production/phase4/','src/godot-game/production/assets/']];
const GAME=/^src\/godot-game\/(?:(?:assets|content|inspection|foundation|proofs|docs)\/|(?:project\.godot|export_presets\.cfg|KingdomZombies\.csproj|override\.cfg)$)/;
function physical(p){for(const [a,b] of PREFIX)if(p.startsWith(a))return b+p.slice(a.length);
  return GAME.test(p)?'src/godot-game/game/'+p.slice('src/godot-game/'.length):p;}
```

```js
// ac-08 sources (scratch only; reproduces dev's 485 rows when '.import' is NOT treated as binary)
const tracked=git(['ls-files','-z']).split('\0').filter(Boolean);
const checked=tracked.filter(p=>!/^\.(claude|codex|agents|planning)\//.test(p)&&!BINARY.has(ext(p))).sort(ordinal);
// kind/sha/owners per §3c; sourceSetSha256=sha256(JSON.stringify(sources)); repositorySha256=sha256(JSON.stringify(repositoryStages))
```

## Environment Availability

| Dependency | Available | Version | Note |
|---|---|---|---|
| Pinned node | ✓ (not on PATH) | 24.22.1 | PR worktree ignored `.phase1/quick-261121-jmu-node/` |
| .NET SDK | ✓ on PATH | 10.0.404 only | matches `global.json` |
| Git | ✓ | 2.56.1 (`merge-tree --write-tree`) | |
| Godot feed + 3 Godot packages | main checkout only | 4.7.2 | copy (§5) |
| `pwsh` | ✗ | — | not needed for the narrowed list |
| Long paths | ✓ | `LongPathsEnabled=1` | |
| Disk | PR worktree drive 9.9 GB free; worktrees-root drive 114 GB | | |

## Validation Architecture

| Req | Behavior | Type | Command |
|---|---|---|---|
| Merge integrity | no retired roots, no markers | static | `git ls-files <retired roots>` empty; `git diff --check`; `git grep -n '^<<<<<<<\|^>>>>>>>'` empty |
| Compile + analyzers | 5 projects, 0/0 | build | §5 build row |
| Layout | placement, catalogs from unrelated cwd incl. asset-pipeline | unit | `node --test src/godot-game/tools/repository-layout.test.cjs` |
| Docs | 36 cases | runner | `node src/godot-game/tools/verification/check.cjs --group docs` |
| Binding regeneration | self-consistent hashes | static (not a pass claim) | `repositoryPlan`/`callerPlan`/`producerPlan`, `content.validate`, `scenarios.loadCatalog`, `text-data.validateSupport`, `--list` ×4 |

## Security Domain

No authentication, sessions or crypto. The relevant threats: (1) tampering through weakened validators or pins. Mitigation: rebind values only, and diff validators for logic changes. (2) Information disclosure through machine paths or user-profile paths in committed lines. Mitigation: grep new lines for drive letters and user-profile paths before each commit. (3) Committing the untracked 261122-uby files. Mitigation: explicit staging.

## Assumptions Log

| # | Claim | Section | Risk if wrong |
|---|---|---|---|
| A1 | PR #7's direct `tools/` files trigger the `tools/` → `src/godot-game/tools/` directory-rename detection (cause inferred; the 31 → 81 effect is verified) | Summary | none for the plan |
| A2 | Seeding feed + 3 Godot packages makes the in-place locked restore fully offline | §5 | a network attempt; abort and seed full `.phase1/packages` |
| A3 | MTP cohort is the only producer cohort whose live input the PR changed | §3c | another `PRODUCER` mismatch surfaced only by unrun cases |
| A4 | `normalizeCurrent` reproduces 132/133 index rows under the map (261122-uby prototype; dev content unchanged since) | §3a | extra pack-index rows change; investigate before committing |
| A5 | Docs group passes 36/36 after the fixes; it needs no Godot editor or `.phase4` inputs | §5 | a blocked case; report it |

## Open Questions (creator)

1. **Verification location:** in place in the PR worktree (recommended, about 30 MB of copies) or the locked verification-worktree route (about 680 MB, removed after push with the cleanup tool). CONTEXT leaves this to discretion; confirm only if you want the worktree route.
2. **Worktree location wording:** CONTEXT says `worktrees.root`; `cleanup.md` (dev) says the system temp folder, named `kz-<task-id>-…`. This only matters if a worktree is used.
3. **Residual risk:** many rebound bindings (ac-08 execution, producer/caller stages, every asset-pipeline case, export, Python extractors) are only statically validated by the narrowed list. Accept a report of "rebound, statically consistent, not executed", or add `--case ac-08.complete` (about 25 stages, tens of minutes)?

## Sources

- Today: `git merge-tree` on `c5be170` × `a305b8b` (tree `7b2a9f4`) and × `1a14684`; `git show`/`git grep` of both sides; read `docs/workflow/phase4-layout-integration.md`, `docs/architecture/repository-layout.md`, dev `docs/workflow/cleanup.md`, dev `04-CLOSURE-EXCEPTION.md`, dev 261122-ub4 SUMMARY, PR `DocumentationValidator.cs`/`DocumentationLinks.cs`/`DocumentationPath.cs`/`SourceFileFormat.cs`, `repository-layout.test.cjs`, dev `tools/phase1/check.cjs`, dev `ExemptionTests.cs` repository stages, dev `tools/phase4/{check,content,scenarios,cases/*}.cjs`.
- Scratch scripts this session (deleted at end): `virt.cjs` (move map, link audit), `ac08set.cjs` (ac-08 rule vs dev), package-coverage scan.
- 261122-uby RESEARCH (refreshed; differences: conflict counts, tools/phase4 placement, PR #7 docs/tools, STATE directory link, main `.phase4` precedent scripts no longer exist).

## Metadata

**Confidence:** conflict and move map HIGH; cascade inventory HIGH; regeneration rules HIGH for ac-08 (reproduced), MEDIUM for content index (carried); verification recipe HIGH.
**Valid until:** any new commit on `dev` or the PR branch.
