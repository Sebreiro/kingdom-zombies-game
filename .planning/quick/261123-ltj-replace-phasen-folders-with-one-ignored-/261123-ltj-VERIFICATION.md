---
phase: quick-261123-ltj
verified: 2026-11-23T19:20:00Z
status: passed
score: 7/7 must-haves verified
tested_revision: cfbbb5fc88155e964ce0498c00c005bf987bd260
covered_files: [".claude/settings.json",".codex/hooks.json",".gitignore",".planning/ROADMAP.md",".planning/STATE.md",".planning/phases/999.1-rename-phase-named-identifiers/.gitkeep",".planning/quick/261123-ltj-replace-phasen-folders-with-one-ignored-/261123-ltj-PLAN.md",".planning/quick/261123-ltj-replace-phasen-folders-with-one-ignored-/261123-ltj-SUMMARY.md","docs/architecture/decisions/0001-development-baseline.md","docs/architecture/decisions/0001-enforcement.md","docs/architecture/decisions/0003-state-and-engine.md","docs/architecture/repository-layout.md","docs/content/production.md","docs/workflow/ai-collaboration.md","docs/workflow/cleanup.md","docs/workflow/foundation-checks.md","docs/workflow/lan-foundation-checks.md","docs/workflow/pre-phase5-testing.md","docs/workflow/setup.md","docs/workflow/verification.md","src/godot-game/NuGet.config","src/godot-game/game/content/pack-index.json","src/godot-game/game/content/packs/delivery.json","src/godot-game/tests/verification/AnalyzerInputIdentityTests.cs","src/godot-game/tests/verification/AnalyzerSourceConsumerTests.cs","src/godot-game/tests/verification/Analyzers/ContinuationIndentAnalyzerTests.cs","src/godot-game/tests/verification/AppearanceStateTests.cs","src/godot-game/tests/verification/BuildFixture.cs","src/godot-game/tests/verification/CharacterPresentationTests.cs","src/godot-game/tests/verification/ConfigurationTests.cs","src/godot-game/tests/verification/ContentInspectorTests.cs","src/godot-game/tests/verification/ContentNativePermissionTests.cs","src/godot-game/tests/verification/DocumentationFixture.cs","src/godot-game/tests/verification/DocumentationValidator.cs","src/godot-game/tests/verification/EngineGeneratedTests.cs","src/godot-game/tests/verification/EngineRun.cs","src/godot-game/tests/verification/ExemptionTests.cs","src/godot-game/tests/verification/ExemptionValidator.cs","src/godot-game/tests/verification/FoundationCapturePublicationTests.cs","src/godot-game/tests/verification/FoundationLanServicesTests.cs","src/godot-game/tests/verification/FoundationServiceSubstitutionProof.cs","src/godot-game/tests/verification/GeneratedCallSiteAdapterTests.cs","src/godot-game/tests/verification/GeneratedExportLayoutTests.cs","src/godot-game/tests/verification/GeneratedLayoutAdapterTests.cs","src/godot-game/tests/verification/GeneratedSyntaxAdapterTests.cs","src/godot-game/tests/verification/GraphFixture.cs","src/godot-game/tests/verification/PinEdgeTests.cs","src/godot-game/tests/verification/PinsTests.cs","src/godot-game/tests/verification/ProjectEvaluator.cs","src/godot-game/tests/verification/RunnerContractTests.cs","src/godot-game/tests/verification/RunnerProcessFixture.cs","src/godot-game/tests/verification/WorkspacePaths.cs","src/godot-game/tools/README.md","src/godot-game/tools/asset-pipeline/cases/audio-feasibility.cjs","src/godot-game/tools/asset-pipeline/cases/audio.cjs","src/godot-game/tools/asset-pipeline/cases/bodies-a.cjs","src/godot-game/tools/asset-pipeline/cases/bodies-b.cjs","src/godot-game/tools/asset-pipeline/cases/defenses.cjs","src/godot-game/tools/asset-pipeline/cases/export-layout.cjs","src/godot-game/tools/asset-pipeline/cases/export-portability.cjs","src/godot-game/tools/asset-pipeline/cases/export-profile.cjs","src/godot-game/tools/asset-pipeline/cases/hub.cjs","src/godot-game/tools/asset-pipeline/cases/items.cjs","src/godot-game/tools/asset-pipeline/cases/main-buildings.cjs","src/godot-game/tools/asset-pipeline/cases/media.cjs","src/godot-game/tools/asset-pipeline/cases/provenance.cjs","src/godot-game/tools/asset-pipeline/cases/runner.cjs","src/godot-game/tools/asset-pipeline/cases/text-data.cjs","src/godot-game/tools/asset-pipeline/cases/ui-camp.cjs","src/godot-game/tools/asset-pipeline/cases/ui-inventory.cjs","src/godot-game/tools/asset-pipeline/cases/ui-kit.cjs","src/godot-game/tools/asset-pipeline/cases/world.cjs","src/godot-game/tools/asset-pipeline/check.cjs","src/godot-game/tools/asset-pipeline/content.cjs","src/godot-game/tools/asset-pipeline/evidence.cjs","src/godot-game/tools/asset-pipeline/export-guards.test.cjs","src/godot-game/tools/asset-pipeline/export.cjs","src/godot-game/tools/asset-pipeline/required-cases.json","src/godot-game/tools/asset-pipeline/scenarios.cjs","src/godot-game/tools/asset-pipeline/shared-files.test.cjs","src/godot-game/tools/gameplay-checks/check.cjs","src/godot-game/tools/gameplay-checks/required-cases.json","src/godot-game/tools/gameplay-checks/runner-fixture.cjs","src/godot-game/tools/lan-checks/check.cjs","src/godot-game/tools/lan-checks/foreground.test.cjs","src/godot-game/tools/lan-checks/inherited.cjs","src/godot-game/tools/lan-checks/inherited.test.cjs","src/godot-game/tools/lan-checks/network.cjs","src/godot-game/tools/lan-checks/rendered-stream.test.cjs","src/godot-game/tools/lan-checks/revisions.test.cjs","src/godot-game/tools/lan-checks/runner-fixture.cjs","src/godot-game/tools/lan-checks/stopped-fixture.cjs","src/godot-game/tools/lan-checks/timing.test.cjs","src/godot-game/tools/node-setup/session-path.ps1","src/godot-game/tools/node-setup/session-path.sh","src/godot-game/tools/node-setup/session-path.test.cjs","src/godot-game/tools/node-setup/setup.ps1","src/godot-game/tools/node-setup/setup.sh","src/godot-game/tools/node-setup/shared-root.ps1","src/godot-game/tools/node-setup/shared-root.sh","src/godot-game/tools/verification/ac08-bindings.cjs","src/godot-game/tools/verification/asset-bindings.cjs","src/godot-game/tools/verification/bindings.test.cjs","src/godot-game/tools/verification/bootstrap.cjs","src/godot-game/tools/verification/check.cjs","src/godot-game/tools/verification/linux-vm/start-vm.cjs","src/godot-game/tools/verification/linux-vm/start-vm.test.cjs","src/godot-game/tools/verification/rebind-gates.cjs","src/godot-game/tools/verification/required-cases.json","src/godot-game/tools/verification/toolchain.json","src/godot-game/tools/workspace-cleanup.cjs","src/godot-game/tools/workspace-cleanup.test.cjs","src/godot-game/tools/workspace-migrate.cjs","src/godot-game/tools/workspace-migrate.test.cjs","src/godot-game/tools/workspace-paths.cjs","src/godot-game/tools/workspace-paths.test.cjs"]
covered_digest: "v4:sha256:f2a192e7099536fe3128db59548913cb292b9ba389f000555350758373e3dace"
behavior_unverified: 0
overrides_applied: 0
---

# Quick Task 261123-ltj: One ignored .local folder - Verification Report

**Task goal:** Replace the `.phaseN` folders with one git-ignored, regenerable root `.local/` (tools, downloads, nuget, locks, runs/<tool-purpose>, temp); share tools/downloads/nuget per repository through the main checkout's `.local/` (git common dir, `KZ_SHARED_LOCAL` override); pin Node 24.22.1 by SHA-256 with a tracked setup script and agent session hooks; update docs, track the rebinding tools, add the worktree location rule and the `core.longpaths` note; provide a dry-run-first migration that deletes the legacy Linux VM; fix PR #11 contradictions; record backlog 999.1. No binaries committed.

**Verified:** 2026-11-23, at `cfbbb5fc88155e964ce0498c00c005bf987bd260` (task worktree, tree clean apart from the quick-task folder)
**Status:** passed
**Re-verification:** No, initial verification

Locations are symbolic: **main checkout**, **task worktree**, **shared root** (the main checkout's `.local/`), **local root** (a checkout's own `.local/`).

## Goal Achievement

### Observable Truths

| # | Truth (PLAN must_haves) | Status | Evidence |
| --- | --- | --- | --- |
| 1 | One ignored root `.local/` with tools, downloads, nuget, locks, runs/<tool>, temp in live code, ignore rules and live docs; remaining `.phaseN` names are only legacy, captured, logical-identity or historical entries, each listed by the residue gate | VERIFIED | `.gitignore:5-8` adds `/.local/` and keeps `/.phase[0-9]*/` under a legacy comment (declared deviation). `workspace-paths.cjs` `locations()` builds exactly those six subfolders. Re-ran `residue` with the 22-entry exact-`=N` allow set: `PASS residue 234 allowed line(s) in 21 file(s)`. Negative probes: dropping the `check.cjs` allow gives `FAIL ... outside --allow`; `=2` gives `retains 1 line(s), expected 2`. Computed constructions (`git grep -F '.phase\d'` etc.) remaining are the legacy regexes in cleanup, the `LEGACY_MAP` comment, and `scenarios.cjs:412`, which routes through `content.legacyLocal` (legacy map) |
| 2 | tools/downloads/nuget/shared locks resolve to the main checkout's `.local/` from the task worktree, the main checkout and an unrelated cwd via `--git-common-dir`; `KZ_SHARED_LOCAL` overrides; runs, temp, feed and check lock stay per checkout | VERIFIED | Ran `workspace-paths.cjs --print-roots`: from the task worktree shared = main checkout `.local`, local = worktree `.local`, source `git`; with `--root` = main checkout shared = local; from the system temp cwd same as worktree; absolute override gives source `override`; relative override exits 1 with `SHARED_ROOT_INVALID`. `locations()` puts `nuget.feed`, `locks.check`, `runs()`, `temp()`, `cliHome` under local and tools/downloads/packages/http/scratch/toolchain+desktop locks under shared. C# `WorkspacePaths.cs:242-295` follows the same order (override, git common dir, `GIT_MISSING`). `workspace-paths.test.cjs` 15/15 pass |
| 3 | From the task worktree the strict locked build is 0 warnings / 0 errors with no network GET, docs group 36/36, using shared NuGet packages and shared NUGET_SCRATCH | VERIFIED | Orchestrator log at the tested revision (written after the HEAD commit): `PASS build restore locked without network, 0 Warning(s), 0 Error(s)` and `PASS docs N_base=36 N_head=36 PASS 36/36`. Independent: the gate's `restore.log` has 0 `GET http` lines; `build.log` shows `0 Warning(s)` / `0 Error(s)`; worktree `obj/project.assets.json` `packageFolders` points at the shared root's `nuget/packages`. The build gate uses `isolationEnv()` (`rebind-gates.cjs:300`), which sets `NUGET_SCRATCH` to the shared `nuget/scratch` |
| 4 | ac-08, asset-cascade and gate tools are tracked with tests; reproduced base bytes before any hashed change; every gate shown to FAIL before PASS counted; all bindings current at the final commit (423 modules, delivery, pack index, ac-08 rows, 6 caller hashes, 9 staged plans); lists 686/16/32/75; docs count equals base | VERIFIED | First branch commit `0c16511` contains exactly the 4 tool files. Re-ran `ac08-bindings --check --rev BASE`: PASS rows 498, callers 6 (tool reproduces the unchanged base catalog). `bindings.test.cjs` has paired fail/pass controls for all ten gates plus CLI controls on a temporary repository (lines 160-297); included in the `tests` gate (120 pass, 16 files). Re-ran `assets` (`modules=423 base=423`) and `ac08` (`current, callers=6 staged=9 registries=4`): PASS. `lists` 686/16/32/75 and `docs` 36/36 in the orchestrator log. The forced real-tree FAIL runs before the first PASS are history, recorded in results; not re-observable now, but consistent with commit order |
| 5 | Node 24.22.1 pinned by SHA-256 in toolchain.json, installed once into shared `tools/node/` by the tracked script, put on PATH by a Claude SessionStart hook; Codex advisory line; no persistent PATH change | VERIFIED | `toolchain.json:9` `"node": "24.22.1"` first; `:34-36` `nodeArchives` with both pinned hashes matching CONTEXT. `setup.sh --check` exit 0, `verified in the shared tools`. The shared archive's sha256 equals the win-x64 pin. `.claude/settings.json` SessionStart runs `session-path.sh`; `.codex/hooks.json` adds a fail-open second group calling `session-path.sh --codex` / `session-path.ps1`, with the GSD group unchanged. Ran the hook twice with a scratch `CLAUDE_ENV_FILE`: one line; sourced in a bare `PATH=/usr/bin:/bin` shell, `node --version` = v24.22.1 from the shared root. `--codex` prints the advisory line. No `setx`, `SetEnvironmentVariable` or shell-profile write in `node-setup/` or the hook configs |
| 6 | Cleanup keeps tools/downloads/nuget and deletes runs/temp/other scratch; migration is dry-run first, lists the VM deletion and size first, refuses while the main checkout's branch uses `.phase1`, and was run only as a dry run | VERIFIED | `workspace-cleanup.cjs:10` `KEPT_LOCAL = {tools, downloads, nuget}`; legacy rule kept (`:13-14`); exports the four safety helpers that `workspace-migrate.cjs:147-150,312-331` reuses. `workspace-cleanup.test.cjs` 21/21 and `workspace-migrate.test.cjs` 17/17 pass. Re-ran the migration dry run: exit 1; first row `delete .phase1/tools/linux-vm (8.2 GB; ...)`; two `refuse:` lines (HEAD lacks `workspace-paths.cjs`; `.local/` not ignored by the main checkout's `.gitignore`); `nothing would be changed`. No-write check: the main checkout's porcelain status (0 lines), HEAD and a recursive listing digest of its `.local` were identical before and after |
| 7 | Docs carry the worktree rule in ai-collaboration.md, cleanup-only cleanup.md, `core.longpaths` in setup.md, `.phaseN -> .local/...` rows in the layout map; PR #11 contradictions fixed; backlog 999.1; history and pinned bytes unchanged; no committed machine path | VERIFIED | `ai-collaboration.md:24-32` worktree rule with `worktrees.root` and the temp fallback; `cleanup.md:47` only links to it. `setup.md:51` `git config core.longpaths true`. `repository-layout.md:55-72` legacy rows and `workspace-paths.cjs` named as the one resolver. ROADMAP C7 (`Nineteen of the current 136`), `## Backlog` / `Phase 999.1` and the tracked `.gitkeep`; STATE deferred row 999.1, C1 (`:213`), C5 (session continuity, PR #11 as `88953aa`), no 261123-ltj Quick Tasks row. Re-ran `history` (509 unchanged) and `newhost` (0 new of 1814): PASS. `git diff --stat` over `docs/code`, `production/assets`, `specs`, `REQUIREMENTS.md` and the two extractors is empty. Added lines contain no drive-qualified, user-folder or worktree path. No binary file in the diff (`--numstat` has no `-` rows), and nothing under `.local/` or `.phaseN/` is tracked |

**Score:** 7/7 truths verified (0 present, behavior-unverified)

### Required Artifacts

| Artifact | Status | Details |
| --- | --- | --- |
| `src/godot-game/tools/workspace-paths.cjs` | VERIFIED | Resolver, `isolationEnv`, `LEGACY_MAP`, `insideShared`, CLI; required by `verification/check.cjs:11`, `rebind-gates.cjs`, `workspace-migrate.cjs:12`, `start-vm.cjs:26` and others |
| `src/godot-game/tests/verification/WorkspacePaths.cs` | VERIFIED | Same resolution order; `Isolate` sets `NUGET_SCRATCH` and `KZ_SHARED_LOCAL`; 47 call sites in the test project; `IsSharedPackage` used by `ExemptionValidator.cs:673` |
| `src/godot-game/tools/verification/ac08-bindings.cjs` | VERIFIED | `--check --rev BASE` and current HEAD pass |
| `src/godot-game/tools/verification/asset-bindings.cjs` | VERIFIED | Used by the `assets` gate, which passes |
| `src/godot-game/tools/verification/rebind-gates.cjs` | VERIFIED | Ten gates; re-ran five, with negative residue probes failing as expected |
| `src/godot-game/tools/verification/bindings.test.cjs` | VERIFIED | Controls for all ten gate judges plus the CLI end-to-end test |
| `src/godot-game/tools/node-setup/setup.ps1` | VERIFIED | Pin check before extraction, `.part` with nonce, `CreateNew` toolchain lock, recheck under the lock |
| `src/godot-game/tools/node-setup/session-path.sh` | VERIFIED | Exercised directly (see truth 5) |
| `src/godot-game/tools/workspace-migrate.cjs` | VERIFIED | Dry run exercised (see truth 6) |
| `.claude/settings.json` | VERIFIED | Tracked, separate from `settings.local.json` |

### Key Link Verification

| From | To | Status | Details |
| --- | --- | --- | --- |
| `verification/check.cjs` | `workspace-paths.cjs` | WIRED | `environment()` returns `workspace.isolationEnv(root)` (`:127`); lock `where.locks.check` (`:714`); runs `where.runs('verification', nonce)` (`:722`) |
| `WorkspacePaths.cs` | `KZ_SHARED_LOCAL` | WIRED | Override read first (`:242`), then git common dir (`:266`) |
| `NuGet.config` | `toolchain.json` | WIRED | Both carry `../../.local/nuget/feed` |
| asset `required-cases.json` | `pack-index.json` | WIRED | `assets` gate PASS (423 modules, check passed) |
| verification `required-cases.json` | gameplay `required-cases.json` | WIRED | `ac08` gate PASS, 6 callers current |
| `.claude/settings.json` | `session-path.sh` | WIRED | Hook command runs the script, which reads the version from `toolchain.json` with `sed` and appends to `CLAUDE_ENV_FILE` |
| `workspace-migrate.cjs` | `workspace-cleanup.cjs` | WIRED | Reuses `keepReason`, `isIgnored`, `hasTrackedFiles`, `isRealDirectory` |

### Behavioral Spot-Checks

| Behavior | Result | Status |
| --- | --- | --- |
| `--print-roots` from the worktree, the main checkout (`--root`), the system temp cwd, with an absolute and a relative override | as in truth 2 | PASS |
| `session-path.sh` twice with a scratch env file, then sourced in a bare shell | 1 line; `node --version` v24.22.1 | PASS |
| `setup.sh --check`; shared archive sha256 | exit 0; equals the pin | PASS |
| `node --test` on `workspace-cleanup`, `workspace-migrate`, `workspace-paths` | 21, 17, 15 pass, 0 fail, 0 skipped | PASS |
| `rebind-gates history newhost residue assets ac08` with the final allow set | all PASS, same counts as the orchestrator log | PASS |
| residue negative probes (missing allow, wrong `=N`) | exit 1 with the named file | PASS |
| `ac08-bindings --check --rev BASE` | PASS rows 498 | PASS |
| migration dry run on the main checkout, with a no-write check | exit 1, VM row first with size, two refusals, nothing changed | PASS |

Full gate set (lists, tests, roots, build, docs) was not re-run; the orchestrator's run at the tested revision (exit 0) is used for those, cross-checked against the build logs and `project.assets.json`.

### Anti-Patterns Found

| File | Pattern | Severity | Impact |
| --- | --- | --- | --- |
| `.codex/hooks.json` (GSD group) | machine-specific path | Info | Present at BASE, GSD-owned, not added by this task; declared in SUMMARY Known gaps |

No unreferenced `TBD`/`FIXME`/`XXX` debt markers were introduced; the `newhost` and `residue` gates pass.

### Declared not-run items (judged for honesty)

The SUMMARY's "Not run and expected-blocked" table states each item and its reason: export, editor import, headless launch, game windows, the full case selection, bootstrap acquisition, the Codex hook under Codex, the Claude hook in a new session, native Linux execution, `--apply`, the C# rules that have no xunit case (spot runs only), and the launcher (syntax only). These match the plan's not-run list and the code. None is a hidden gap.

Creator-accepted open items (not task gaps; none contradicts a must-have): the catalog still marks the 8 coverage cases `acceptancePhase: 5` (documented in STATE `[Phase 11]` and three docs); IN-05, IN-07 and IN-10 are unfixed.

### Human Verification Required

None beyond the items already declared not-run.

### Gaps Summary

None. Every must-have was checked against code and against runs made during this verification. Where a claim is historical (forced FAIL runs before the first PASS, the BASE reproduction before any hashed change), the commit order and the re-run of the ac-08 tool against BASE are consistent with it.

---

_Verified: 2026-11-23_
_Verifier: Claude (gsd-verifier)_
