# Quick task research: one regenerable `.tmp/` root instead of `.phase1/`–`.phase4/`

**Researched:** 2026-11-23, read-only, static analysis of `origin/dev` at `18e77bc`. Node is not available, so no repository tool ran.
**Locations:** "main checkout" means the primary working tree. "Worktree" means a linked worktree. "Shared root" means the proposed `.tmp/` in the main checkout.
**Confidence:** HIGH for the inventory and counts, which come from git grep and git show. MEDIUM for the NuGet and Godot behaviour, which comes from cited documentation. Items tagged `[ASSUMED]` need confirmation.

## Summary

- On dev, `.phaseN` paths are **literals spread across about 74 live source files** (317 lines). No central helper exists. The isolation environment (`DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH`, `TEMP`) is copied into at least 12 places, in JS and C#.
- **Every write goes to the checkout root.** Bootstrap uses `root = path.resolve(__dirname,'../../../..')` and `home = path.join(root,'.phase1')`. It also refuses any path outside that root (`PATH_OUTSIDE_WORKSPACE`), so a shared toolchain needs a deliberate new exception.
- **Node is not acquired by any tracked tool.** It is a declared host prerequisite (`toolchain.json` `"node": "24.22.1"`, and no hash is pinned). Recent quick tasks placed it by hand in an ignored per-worktree folder (`.phase1/quick-261121-jmu-node/`).
- **Hash cascade:** all 25 C# test files that name `.phaseN` are ac-08 `CHECKED` rows. 19 asset-pipeline modules carry about 282 of the 423 `moduleSha256` bindings. The 5 production JSONs and the 5 `docs/code` registers that name `.phaseN` are pack-index rows or read-out pins, so they **should keep their bytes** and be resolved through a map.
- **NuGet sharing is safe only with a shared `NUGET_SCRATCH`.** Microsoft documents that processes using different temp folders but the same global-packages or http-cache folder can fail. Our tools set `TEMP` per checkout.
- **Build outputs (`bin/`, `obj/`, `.godot/`) should stay where they are.** About 1,000 register lines, the analyzer `GeneratedLayout.cs` and test hint paths depend on `.godot/mono/temp/...`.
- **GSD paths (`.planning/tmp`, `state.json`, `milestone.lock`) must stay.** They are hard-coded relative to the planning folder inside the vendored GSD.

## 1. Inventory of ignored locations (dev)

Sizes come from `du -sh` on the main checkout's subfolders. The main checkout's `.phase2/`, `.phase3/` and `.phase4/` folders are **empty** (`ls -A`), so every Phase 2–4 input listed below is already deleted locally.

| Location | Category | Writer → readers | Regenerable / cost | Must survive tasks? |
|---|---|---|---|---|
| `.phase1/cache/` Godot zip + `SHA512-SUMS.txt` (112 MB) | toolchain archive | `verification/bootstrap.cjs:239-251` → about 12 asset-pipeline cases (`hash(.phase1/cache/<pin.file>)`), `export-portability`, `hub`, `ui-kit`, `world` | yes, download plus SHA-512 pin | yes (cache) |
| `.phase1/cache/phase4-exports/` templates `.tpz` (1.2 GB) + sums | toolchain archive | **manual** (no tracked downloader) → `export.cjs:39-42`, pinned in `production/assets/export-toolchain.json` `archive.path` | yes, 1.2 GB manual download | yes |
| `.phase1/cache/game.packages.lock.json` | stray scratch | no live writer → none | n/a | no; **misfiled** under the "durable" `cache` |
| `.phase1/tools/Godot_v4.7.2-stable_mono_win64/` (258 MB) | toolchain binary | bootstrap extract (`:255-262`) → `EngineRun.cs:36`, `network.cjs:21`, `gameplay-checks/check.cjs:107`, `asset-pipeline/check.cjs:256`, `evidence.cjs:173`, `scenarios.cjs:341`, `items`, `media`, `hub`, `export-*` | yes, extract from the cached zip | yes |
| `.phase1/tools/linux-vm/` (8.3 GB) + `linux-prep/` (398 MB) | toolchain/VM state, **includes an untracked launcher `start-vm.cjs`** | manual (Phase 1 plans) → `docs/workflow/setup.md` | **no / very costly** (installed guest disk) | **yes, durable**. The launcher is code living in an ignored folder |
| `.phase1/tools/phase4-*-pre-hooks.json`, `phase4-plan-index.json`, `phase4-execute-init.json` | GSD scratch | ad-hoc → none | n/a | no; **misfiled** under the "durable" `tools` |
| `.phase1/feed/` (4 nupkg, 3.4 MB) | verified package feed | bootstrap `:282` → `NuGet.config` `value="../../.phase1/feed"`, `toolchain.json` `feeds`, `scenarios.cjs:349,432`, `ConfigurationTests.cs:343-372`, `ExemptionValidator.cs:540,628` | yes, copied from the zip | yes (cache) |
| `.phase1/packages/` (580 MB) | NuGet global packages | dotnet restore via `NUGET_PACKAGES` → `ConfigurationTests.cs:55`, `ExemptionTests.cs:426`, `export.cjs:524-527`, `export-layout.cjs:89`, `scenarios.cjs:360` | yes, restore (network for NuGet.org packages) | yes (cache) |
| `.phase1/http-cache/` (60 MB) | HTTP cache | NuGet → NuGet | yes | no (nice to keep) |
| `.phase1/cli-home/` (37 MB) | CLI state | `DOTNET_CLI_HOME` → dotnet | yes, cheap | no |
| `.phase1/dotnet-home/` (~0) | CLI state | **no live writer on dev** (only the cleanup keep list) | n/a | no |
| `.phase1/temp/` | temp | bootstrap, all isolation envs, 14 C# tests (`.phase1/temp/<purpose>-<guid>`) | yes | no |
| `.phase1/runs/<nonce>/` | run output/report | `verification/check.cjs:714-720`, bootstrap `:15` → `--failed` repair, `PinEdgeTests`, `RunnerContractTests`, `RunnerProcessFixture` | not repeatable bit-for-bit | no (records copy results) |
| `.phase1/check.lock` | lock (per checkout) | `verification/check.cjs:716` → `workspace-cleanup.cjs:10` | n/a | no |
| `.phase1/quick-*`, `phase4-pr-body.md` | task scratch | agents → agents | n/a | no |
| `.phase2/runs/<platform>/<nonce>/`, `completions/`, `controls/` | run output | `gameplay-checks/check.cjs:70,85`, `runner-fixture.cjs:23` → `--failed`, `--verify-completion 02-PLATFORM-EVIDENCE.json` | game runs, not repeatable | **evidence** for the Phase 2 completion check (already broken after cleanup) |
| `.phase2/desktop.lock` | lock (desktop ownership) | `gameplay-checks/check.cjs:19`, `scenarios.cjs:1664`, `lan-checks/runner-fixture.cjs:99` → cleanup | n/a | no |
| `.phase3/runs/<platform>/<nonce>/`, `controls/`, `profile-tests/`, `substitution-profiles/` | run output / temp | `lan-checks/check.cjs:67`, fixtures, 3 C# tests | game runs | **evidence** for `03-EVIDENCE.json` completion (already broken) |
| `.phase4/runs/<nonce>/` (98 GiB measured in Phase 5 CONTEXT) | run output, including **full source copies** `runs/<n>/source/src/godot-game` | `check.cjs:236`, `scenarios.cjs:335-453,669-695`, `evidence.cjs:782` | not repeatable | **evidence** cited by `export-layout-origin.json` (about 40 run paths) |
| `.phase4/acceptance/<invocation>/` | run output/evidence | `scenarios.cjs:530-651`, `evidence.cjs:297-690` | needs game runs and judgments | **evidence** for `acceptance.*` |
| `.phase4/checkpoints/04-02-entry-93386745/manifest.json` | **durable input** | historical plan → `cases/export-profile.cjs:14,255-257`, `export-profile.json:15` | no | **yes**: `export.profile` fails without it |
| `.phase4/04-23-current-model/` | **durable input** | historical plan → `ContentNativePermissionTests.cs:120,1689,1862,1990` | no | **yes**: model tests fail |
| `.phase4/04-14-font-inspection.json`, `04-28-art.json` | **durable input** | historical plan → `cases/ui-kit.cjs:42,115` | no | **yes** |
| `.phase4/04-34-preservation.json`, `04-13-authoring/` | evidence / recipe identity | historical → `export-layout-origin.json:210`; `world.cjs:100` uses it only as `__file__` | no | identity only |
| `.phase4/tdd/`, `audio-*/`, `lifecycle-red/`, `compiled-proof/`, `observed26/`, `hub-policy-controls/`, `temp/` | temp/scratch | cases and C# tests → same | yes | no |
| `bin/`, `obj/`, `**/.godot/` | build output | MSBuild, Godot → tests (`BoundaryFixture.cs:113` HintPath, `GraphTests.cs:39`, `ConfigurationTests.cs:54`), analyzer `GeneratedLayout.cs:52,60`, `gameplay-checks/check.cjs` test DLL | yes, a build | no |
| `.planning/tmp/` (52 KB) | GSD scratch | GSD workflows → GSD | yes | no |
| `.planning/state.json`, `.planning/milestone.lock` | GSD state/lock | GSD `state-contract.cjs:368`, `milestone-lock.cjs:65` | derived | GSD-owned |

Commands used: `git grep -n -o -E '.{0,70}\.phase[0-9]…' origin/dev -- src/godot-game/tools`, `git grep … -- src/godot-game/production src/godot-game/tests`, `git show origin/dev:src/godot-game/tools/verification/bootstrap.cjs`, and `du -sh .phase1/*` in the main checkout.

**Durable items that are not caches:** the Linux VM and its launcher, the export templates (manual acquisition), and the five Phase 4 inputs (`04-23-current-model`, the `checkpoints/04-02…` manifest, `04-14-font-inspection.json`, `04-28-art.json`, `04-34-preservation.json`). The Phase 4 inputs **no longer exist in the main checkout.** `docs/workflow/cleanup.md` already lists their checks as known failures.

## 2. How paths are built

There is **no central helper.** Each file builds its own literal. Path-only hits come from `git grep -c -P "(?<![A-Za-z0-9_)\]])\.phase[0-9](?![0-9A-Za-z_.])" origin/dev -- src docs`, which excludes property names such as `selection.selected.phase1`:

| Area | Files | Lines | Notes |
|---|---|---|---|
| `tools/asset-pipeline/` | 25 | 118 | `isolated()`/`env()`/`buildEnvironment()` copied into 7 case modules; `inside(ROOT,'.phase1/cache/'+pin.file)` in about 12 |
| `tools/lan-checks/` | 10 | 35 | isolation env in `check.cjs:22-23`; tests use fake roots |
| `tools/gameplay-checks/` | 2 | 6 | isolation env `check.cjs:144`, desktop lock `:19` |
| `tools/verification/` | 3 | 7 | `bootstrap.cjs:12` `home`, `check.cjs:126` `environment(root)` (honours `PHASE1_PACKAGE_CACHE`), `toolchain.json` feeds |
| `tools/workspace-cleanup*.cjs` | 2 | 45 | `DURABLE_PHASE1`, `RUNNER_LOCKS`, `/^\.phase\d+$/` |
| `tools/README.md`, `NuGet.config` | 2 | 3 | |
| `tests/verification/*.cs` | 25 | 49 | `Path.Combine(ProofProtocol.Root, ".phase1/temp/…")`. `DocumentationValidator.cs:109` and `ProjectEvaluator.cs:199` hold **exclusion lists** naming `.phase1` |
| `production/assets/*.json` | 5 | 54 | captured records and pins |
| `docs/code/*.json` registers | 5 | 531 | package identity `.phase1/feed/Godot.SourceGenerators.4.7.2.nupkg` (448 in `engine-generated-spans.json`) |
| `docs/workflow`, `docs/content`, `docs/architecture` | 10 | 44 | `platform-proof.md` (13) is a historical evidence narrative |
| **Live total** | **89** | **≈892** | 74 src files / 317 lines + 15 docs / 575 lines |
| History (`.planning/phases`, `quick`, `debug`, `STATE.md`) | ≈220 | ≈6,000 | keep bytes (`git grep -c` per folder) |

Partial hooks already exist: the environment variables `PHASE1_PACKAGE_CACHE`, `PHASE1_ROOT` and `PHASE1_RESULT_DIRECTORY` let Node runners pass locations to C# children.

**Recommendation:** add one JS module, `src/godot-game/tools/workspace-paths.cjs`, that resolves shared and local roots and exports `toolchain()`, `nuget()`, `isolationEnv()` and `runDir(tool)`. Add one C# `WorkspacePaths` in `tests/verification/` that reads `KZ_*` environment variables set by the runners and falls back to `<root>/.tmp/…`. Then replace the literals with calls.

## 3. Hash cascade

| Edited set | Binding | Effect |
|---|---|---|
| 25 C# test files (`AnalyzerInputIdentityTests` … `RunnerProcessFixture`) | **ac-08 `repositoryStages` `CHECKED` rows** (230 of 498 sources are checked; all 25 are among them) | regenerate ac-08 `sources[].sha256`, `sourceSetSha256` and `callerStages.repositorySha256` (5 Phase 1 callers plus gameplay `core.permissions`) |
| 18 asset-pipeline case modules + `asset-pipeline/check.cjs` | `asset-pipeline/required-cases.json` `moduleSha256`: **about 282 of 423 rows** (from `grep -o '"module": …' \| uniq -c`) | rewrite those rows, then rehash the register |
| `asset-pipeline/required-cases.json` | pack-index row with logical path `tools/phase4/required-cases.json`; `delivery.json` (2 refs) | regenerate the catalog, delivery and pack index |
| `scenarios.cjs`, `evidence.cjs`, `export.cjs`, `content.cjs` | caller/producer registries and input fingerprints (`export.cjs:26` lists `tools/phase4/export.cjs` and `check.cjs`) | rehash when the plan stages them |
| JS outside the asset pipeline, docs, `NuGet.config`, `toolchain.json` | ac-08 `outside-validator`, `sha256: null` | no hash, but `NuGet.config` and `toolchain.json` `feeds` must change **together** (`bootstrap.cjs:176` `PINS_MISMATCH`) |
| 5 `production/assets/*.json` | **pack-index rows** (`production/phase4/export-toolchain.json` etc.) | **keep their bytes**; resolve `.phase1/cache/phase4-exports/…` through a map |
| 5 `docs/code/*.json` registers | read-out pins in `EngineGeneratedTests`, `ExemptionTests`, `GeneratedCallSiteAdapterTests` (spans `980c2a80…` etc.) | **keep their bytes**; treat `.phase1/feed/X.nupkg` as a logical package identity |
| Captured recipes asserting `<main checkout>/.phase4` (`audio.cjs:66,172,607`; `check.cjs:35`; `tracer-source.json:389`) | recipe hash pins | must keep their bytes. Once outputs leave `.phase4/`, they **fail even in the main checkout**. Today they fail only outside it (fsu WR-02) |

**Rebinding order** (as in quick task 261123-fsu): (1) code: helpers plus literal replacement, committed. (2) Registers: `moduleSha256`, row-baseline pins, caller authority and producer registries. (3) Catalog, delivery and pack index. (4) ac-08, as a **separate commit**, because the fsu writer rebuilt from HEAD blobs and refused a dirty tree. **Gap:** the fsu rebinding tools (`ac08.cjs`, `gates.cjs`) lived in ignored `.phase1/quick-261123-fsu/` and were deleted. They must be rebuilt from the fsu PLAN and RESEARCH §3c. They should then be tracked under `src/godot-game/tools/verification/` (sources: fsu SUMMARY lines 99 and 229, PLAN line 363).

## 4. Sharing per repository

| Topic | Finding |
|---|---|
| Today | `root` is derived from the tool's own location, so every worktree gets its own `.phase1/`. Worktrees seed caches by **copying** from the main checkout (fsu "How to reproduce" step 1). |
| Find the main checkout | `git rev-parse --path-format=absolute --git-common-dir` returns the main `.git` from any worktree (git 2.56.1 here; verified by running it). Its parent is the main checkout. To avoid spawning git, read the worktree's `.git` file (`gitdir: …/worktrees/<n>`), then the `commondir` file in that directory. |
| Override | Add `KZ_SHARED_TMP` for CI, for several clones sharing one cache, for bare repositories, and for source copies under run folders. Those copies (`scenarios.cjs` copies the tree into `runs/<n>/source`) are not git checkouts, so runners must pass the shared root down through the environment. |
| Main checkout removed / several clones | Worktrees break anyway without the common `.git`. Each clone has its own shared root, and the cost is one re-bootstrap (about 1.4 GB download plus the VM). Use `KZ_SHARED_TMP` to share across clones. |
| Path length | Longest relative paths today: `cli-home` 164, `http-cache` 158, `packages` 157 (`find … \| awk 'length'`). Under the main checkout (about 29 characters) that is about 193. Under a temp-folder worktree (about 70 characters) it is about 235. Sharing moves these out of worktrees. Real overflow comes from `.phase4/runs/<uuid>/source/src/godot-game/…/.godot/mono/temp/obj/…` copies, so keep run folder names short. `core.longpaths` is **unset** (`git config --get core.longpaths` exits 1), and that is the "Filename too long" source on removal. |
| NuGet concurrency | Microsoft: "If different processes use different temp directories, but the same global-packages or http-cache directory, various errors might occur… NuGet uses the temp directory to coordinate inter-process access… using filesystem locks". The scratch folder can be moved with `NUGET_SCRATCH` [CITED: learn.microsoft.com …/managing-the-global-packages-and-cache-folders]. **So a shared packages folder requires a shared `NUGET_SCRATCH`.** Restoring the *same project* concurrently is unsafe, but each worktree's projects have different paths. `.nupkg.metadata` is written last and marks extraction as complete. |
| Toolchain extraction race | Bootstrap downloads to `<file>.<nonce>.part` and renames it, which is safe. Extraction into `tools/` and the feed copy are not atomic. Add a shared `.tmp/locks/toolchain.lock` around acquisition, and treat the toolchain as read-only after hash verification. Godot editor data is already redirected per run (`asset-pipeline/check.cjs:148`), so sharing the binary is safe. |
| Desktop lock | `.phase2/desktop.lock` protects the **desktop**, which is a machine-wide resource. Per checkout, two worktrees can launch rendered games at the same time. Recommend a shared lock. |
| Bootstrap guard | `resolveLocal` rejects paths outside the checkout (`bootstrap.cjs:73-76`). `export.cjs:376` rejects traces containing `\.phase1\` (`HOST_AUTHORING_RESOLUTION`). Both need an explicit allowance for the shared root. |

## 5. Build outputs: keep them

`Godot.NET.Sdk` places the game's bin and obj under `<project>/.godot/mono/temp/` [CITED: Godot SDK commit history via web search; MEDIUM]. `git grep -c '\.godot/mono/temp'` finds about **1,000 lines** that depend on it: `engine-generated-spans.json` 681, `generated-callsite-adaptations.json` 273, `GeneratedLayout.cs:52,60` (the analyzer recognizes generated files by this path), `ExemptionValidator.cs` 9, test hint paths, and about 15 asset-pipeline cases. The other projects (`core`, `tests`, `code-rules`) could move through `BaseIntermediateOutputPath`/`BaseOutputPath` in `Directory.Build.props`. That would change analyzer inputs and the paths that `gameplay-checks` and `lan-checks` use to find the test DLL (`tests/verification/bin/Debug/net10.0/…`). **Recommendation: keep `bin/`, `obj/` and `.godot/` in place**, still ignored and cleaned by `--build`. Moving them carries a high risk for no user-visible gain.

## 6. GSD paths: they must stay

`.claude/gsd-core/bin/lib/milestone-lock.cjs:65` (`'milestone.lock'`), `state-contract.cjs:368` (`path.join(paths.planning,'state.json')`) and the GSD workflows (`.planning/tmp`) resolve relative to the planning folder. They are vendored, and a GSD update overwrites any edits. **Keep them**, and keep them listed separately in the cleanup table.

## 7. Committed root folders and phase-named live paths

Tracked root folders are `.claude`, `.codex`, `.planning`, `docs`, `specs` and `src` (`git ls-tree -d origin/dev`). All have specific names.

Phase-named **live** tracked paths, excluding `.planning/phases`, quick tasks and vendored GSD:

- `docs/workflow/phase4-layout-integration.md` (a completed guide; history candidate)
- `docs/workflow/pre-phase5-testing.md`
- `src/godot-game/game/proofs/Phase1.cs` and `Phase1.tscn`
- `src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj` (1,246 references to `KingdomZombies.Phase1.Tests`)
- `.planning/debug/phase04-*.md`

Phase-named identifiers inside files:

- environment variables: `PHASE1_*` (27 names), `PHASE2_*`, `PHASE3_*`, `KZ_PHASE4_*`
- inherited case families `phase1`, `phase2` and `phase3` in the selection files
- the Godot preset `Phase4 Windows inspection`
- the Linux VM user `/home/phase1`

Renaming these changes test FQNs and case IDs throughout the registers. **Keep this out of scope** and record it as a follow-up.

## 8. Proposed layout

S = shared root (`<main checkout>/.tmp`, or `KZ_SHARED_TMP`). L = this checkout's `.tmp`. In the main checkout, S = L.

| New path | Scope | Replaces |
|---|---|---|
| `S/tools/node/` (v24.22.1), `S/tools/godot/<archive dir>/`, `S/tools/linux-vm/`, `S/tools/linux-prep/` | shared | `.phase1/tools/*`, ad-hoc `quick-*-node/` |
| `S/downloads/` (Godot zip, `SHA512-SUMS.txt`, export `.tpz` + sums, Node zip + `SHASUMS256.txt`) | shared | `.phase1/cache/` |
| `S/nuget/packages/`, `S/nuget/http/`, `S/nuget/scratch/` (`NUGET_SCRATCH`) | shared | `.phase1/packages`, `http-cache` |
| `S/locks/toolchain.lock`, `S/locks/desktop.lock` | shared | new, and `.phase2/desktop.lock` |
| `L/feed/` (4 nupkg, copied from S) | per checkout | `.phase1/feed`. Keeps `NuGet.config` relative (`../../.tmp/feed`) for direct `dotnet restore` |
| `L/cli-home/`, `L/temp/` | per checkout | `.phase1/cli-home`, `temp`, `dotnet-home` |
| `L/locks/check.lock` | per checkout | `.phase1/check.lock` |
| `L/runs/verify/`, `L/runs/game/<platform>/`, `L/runs/lan/<platform>/`, `L/runs/assets/` (plus `assets/acceptance/`) | per checkout | `.phase1/runs`, `.phase2/runs` + `completions`, `.phase3/runs`, `.phase4/runs` + `acceptance` |
| `L/scratch/<purpose>/` (controls, tdd, probes, `<task-id>/`) | per checkout | `.phase2/controls`, `.phase3/controls|profile-tests|substitution-profiles`, `.phase4/tdd|audio-*|…`, `.phase1/quick-*` |
| `src/godot-game/production/assets/inputs/…` (tracked) **or** declared lost | tracked | `.phase4/04-23-current-model`, `checkpoints/04-02…`, `04-14…`, `04-28…` (Decision 4) |
| `src/godot-game/tools/verification/linux-vm/start-vm.cjs` (tracked) | tracked | untracked launcher in `.phase1/tools/linux-vm/` |

**`.gitignore`:** replace `/.phase[0-9]*/` with `/.tmp/`. Keep `/.phase[0-9]*/` during a transition so old folders are not suddenly committable.

**Cleanup rules:**

| Location | Action |
|---|---|
| `S/tools`, `S/downloads`, `S/nuget` | keep (replaces `DURABLE_PHASE1`) |
| `L/feed`, `L/cli-home` | keep (cheap; optional delete) |
| `L/temp`, `L/runs`, `L/scratch` | delete at task end |
| `S/locks/*`, `L/locks/*` | runner locks; refuse to apply while present (replaces `RUNNER_LOCKS`) |
| Legacy `.phaseN/` | report it, then migrate (move the shared items once) or delete |
| Worktree removal | must never follow into S, which lies outside the worktree |

## 9. Decisions for the creator

| # | Decision | Options | Recommendation |
|---|---|---|---|
| D1 | Folder name | (a) `.tmp`; (b) `.local`; (c) `.work` | **(a) `.tmp`** as you proposed, with the 10 GB items kept under the clearly named `tools/` and `downloads/`, which cleanup protects |
| D2 | Shared root location | (a) `<main checkout>/.tmp` found through `--git-common-dir`, with a `KZ_SHARED_TMP` override; (b) inside `<common .git>/kz/`, in the style of git-lfs (works for bare repos) `[ASSUMED]`; (c) a user-level folder such as `%LOCALAPPDATA%` | **(a)** |
| D3 | Which subfolders are shared | (a) tools, downloads, nuget and locks shared; feed, cli-home, temp, runs and scratch per checkout; (b) also share the feed and cli-home; (c) share only tools and downloads | **(a)**. NuGet sharing requires a shared `NUGET_SCRATCH`. |
| D4 | Durable Phase 4 inputs (`04-23-current-model`, the `04-02` checkpoint, `04-14`, `04-28`) | (a) track them under `src/godot-game/production/assets/inputs/` if a copy still exists somewhere; (b) declare them lost and leave the checks blocked for Phase 5 renewal; (c) a kept "evidence" folder under `.tmp` | **(b)**. The main checkout's `.phase4/` is already empty, so check other worktrees first. Reject (c), because ignored folders are not storage between tasks. |
| D5 | Build outputs | (a) keep them in place; (b) move non-Godot bin/obj into `.tmp/build`; (c) move everything | **(a)** |
| D6 | How historical and pinned paths are read | (a) add `.phaseN → .tmp/…` rows to the repository-layout historical map, plus logical→physical mapping in `workspace-paths.cjs` for pinned in-repo data (`export-toolchain.json` `archive.path`, `docs/code` package paths); (b) rewrite the pins and accept the pack-index and register cascade | **(a)**, the PR #6 precedent |
| D7 | Captured recipes that assert `<main checkout>/.phase4` | (a) fail fast with `CAPTURED_ROOT_REQUIRED` and leave renewal to Phase 5 (SPEC line 94 / D-46); (b) keep a legacy writable `.phase4/` only for those steps; (c) a replay shim | **(a)** |
| D8 | How Node 24 reaches `PATH` (tools and GSD) | (a) tracked `src/godot-game/tools/node-setup.ps1` and `.sh` scripts that download, verify (with a new SHA-256 pin in `toolchain.json`) and print a per-session `PATH` line; (b) also add a Claude Code SessionStart hook in the tracked `.claude/settings.json` that writes `PATH` to `CLAUDE_ENV_FILE` `[ASSUMED]`; (c) a persistent user `PATH` entry, which conflicts with the cleanup.md environment rule | **(a) + (b)** |
| D9 | Desktop lock scope | (a) shared, because the desktop is machine-wide; (b) per checkout, as today | **(a)** |
| D10 | Phase-named identifiers (env vars, `Phase1.Tests`, case families) | (a) out of scope; (b) a separate quick task | **(a)**, recorded as a follow-up |
| D11 | Timing relative to Phase 5 | (a) a quick task now, before Phase 5 implementation (the Phase 5 branch has only edited planning and docs so far: `git diff --name-only origin/dev ecb423d`); (b) fold it into Phase 5 | **(a)** |
| D12 | Rebinding tools | (a) track the rebuilt ac-08 and module rebind scripts under `tools/verification/`; (b) task scratch again | **(a)** |
| D13 | `core.longpaths` | (a) document `git config core.longpaths true` as a repository-local setting in setup; (b) rely only on short names | **(a) + short names** |

**Size estimate:**
- Live files: about 74 source files (317 literal lines), about 10 Markdown docs (about 45 lines), `.gitignore`, 2–3 new helpers with tests, and the cleanup command and its test.
- Generated: about 282 `moduleSha256` rows, the asset-pipeline register, pack index, delivery and catalog, and the ac-08 block (25 rows, source set and 6 caller stages).
- Total: about 95 files and 4–6 commits.
- History: 0 bytes changed (about 220 `.planning` files, `platform-proof.md`, 5 `docs/code` registers, 5 production JSONs).
- Main risks: the hash cascade and the untracked rebinding tooling.

## Sources

- [NuGet: managing the global packages, cache and temp folders](https://learn.microsoft.com/en-us/nuget/consume-packages/managing-the-global-packages-and-cache-folders): shared-scratch requirement, `NUGET_SCRATCH`, `.nupkg.metadata` (HIGH)
- [NuGet/Home#7060](https://github.com/NuGet/Home/issues/7060): locks live in temp (MEDIUM)
- [Godot SDK commit: build output moved to .godot/mono/temp](https://remotebranch.eu/Stowage/godot/commit/c3954441f360b46546e8a8deacfc42460e93987d) (MEDIUM)
- [MSBuild: customize by directory](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory): BaseIntermediateOutputPath must be set before the SDK import (MEDIUM)
- Repository (`origin/dev` 18e77bc): `bootstrap.cjs`, `workspace-cleanup.cjs`, `docs/workflow/cleanup.md`, `setup.md`, `toolchain.json`, `export-toolchain.json`, `required-cases.json` (both), `layout.cjs`, the 261123-fsu SUMMARY, the Phase 5 SPEC on the feature branch (VERIFIED via git show and git grep)
