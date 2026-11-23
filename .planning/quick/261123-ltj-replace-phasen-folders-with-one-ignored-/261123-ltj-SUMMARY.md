---
phase: quick-261123-ltj
plan: 01
subsystem: repository-layout
tags: [workspace, local-folder, nuget, node, session-hooks, rebinding, migration, cleanup]
status: complete
requires:
  - phase: quick-261123-fsu
    provides: "PR #6 relocated layout, the asset-pipeline layout map and the rebinding method (squash 18e77bc on dev)"
  - phase: 05-ci-and-repeatable-game-testing
    provides: "PR #11 test-scope amendment (squash 88953aa on dev), including 05-CONTEXT D-46"
provides:
  - "One ignored, regenerable workspace root .local/ (tools, downloads, nuget, locks, runs/<tool>, temp) replacing the .phaseN folders in live code, ignore rules and live docs"
  - "src/godot-game/tools/workspace-paths.cjs and tests/verification/WorkspacePaths.cs: shared root through the Git common dir, KZ_SHARED_LOCAL override, isolation environment and the legacy .phaseN map"
  - "Tracked rebinding tools: verification/ac08-bindings.cjs, asset-bindings.cjs, rebind-gates.cjs with bindings.test.cjs"
  - "Node 24.22.1 pinned by SHA-256 in toolchain.json, installed once into the shared .local/tools/node/ by node-setup/setup.ps1 and setup.sh, put on PATH by a Claude Code SessionStart hook, with an advisory Codex hook"
  - "workspace-cleanup.cjs keeps tools, downloads and nuget; workspace-migrate.cjs moves a main checkout's legacy .phase1 caches, dry run first"
  - "Docs for the layout, the worktree location rule, core.longpaths, the Node setup and the migration; PR #11 contradictions fixed; backlog 999.1"
affects: [phase-05, verification, gameplay-checks, lan-checks, asset-pipeline, workspace-cleanup, setup, agent-sessions]
tested_revision: cfbbb5fc88155e964ce0498c00c005bf987bd260
plan_head_before: 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29
plan_head_after: cfbbb5fc88155e964ce0498c00c005bf987bd260
review_fix_head_before: 19f5818870f2d0ffb65338bea70a4c95decdf0b3
actuals:
  tokens: 218000
  tasks: 3
  commits: 26
tech-stack:
  added: ["Node 24.22.1 (pinned archive, shared install)"]
  patterns:
    - "One resolver (workspace-paths.cjs / WorkspacePaths.cs) for every .local location; no scattered literals"
    - "Shared toolchain caches per repository through git rev-parse --git-common-dir; runs and temp per checkout"
    - "Fail-closed gates with pure judges, each shown to fail before its PASS counted"
key-files:
  created:
    - src/godot-game/tools/workspace-paths.cjs
    - src/godot-game/tools/workspace-paths.test.cjs
    - src/godot-game/tests/verification/WorkspacePaths.cs
    - src/godot-game/tools/verification/ac08-bindings.cjs
    - src/godot-game/tools/verification/asset-bindings.cjs
    - src/godot-game/tools/verification/rebind-gates.cjs
    - src/godot-game/tools/verification/bindings.test.cjs
    - src/godot-game/tools/workspace-migrate.cjs
    - src/godot-game/tools/workspace-migrate.test.cjs
    - src/godot-game/tools/verification/linux-vm/start-vm.cjs
    - src/godot-game/tools/node-setup/setup.ps1
    - src/godot-game/tools/node-setup/setup.sh
    - src/godot-game/tools/node-setup/session-path.sh
    - src/godot-game/tools/node-setup/session-path.ps1
    - src/godot-game/tools/node-setup/session-path.test.cjs
    - src/godot-game/tools/node-setup/shared-root.sh
    - src/godot-game/tools/node-setup/shared-root.ps1
    - src/godot-game/tools/asset-pipeline/export-guards.test.cjs
    - src/godot-game/tools/asset-pipeline/shared-files.test.cjs
    - src/godot-game/tools/verification/linux-vm/start-vm.test.cjs
    - .claude/settings.json
    - .planning/phases/999.1-rename-phase-named-identifiers/.gitkeep
  modified:
    - .gitignore
    - .codex/hooks.json
    - src/godot-game/NuGet.config
    - src/godot-game/tools/verification/toolchain.json
    - src/godot-game/tools/verification/check.cjs
    - src/godot-game/tools/verification/bootstrap.cjs
    - src/godot-game/tools/workspace-cleanup.cjs
    - src/godot-game/tools/verification/required-cases.json
    - src/godot-game/tools/gameplay-checks/required-cases.json
    - src/godot-game/tools/asset-pipeline/required-cases.json
    - src/godot-game/game/content/packs/delivery.json
    - src/godot-game/game/content/pack-index.json
    - docs/architecture/repository-layout.md
    - docs/workflow/setup.md
    - docs/workflow/cleanup.md
    - docs/workflow/ai-collaboration.md
    - docs/workflow/verification.md
    - .planning/STATE.md
    - .planning/ROADMAP.md
key-decisions:
  - "The verified Godot package feed stays per checkout at .local/nuget/feed (relative NuGet.config value, bootstrap PINS_MISMATCH, CheckHash stays inside the root)"
  - "The legacy /.phase[0-9]*/ ignore line is kept beside /.local/ until workspace-migrate.cjs retires the folders"
  - "KZ_SHARED_LOCAL is the only new environment variable; PHASE1_*/PHASE2_*/PHASE3_* names stay (backlog 999.1)"
  - "Pinned .phase4 recipes keep their bytes and fail fast with CAPTURED_ROOT_REQUIRED naming 05-CONTEXT D-46"
requirements-completed: [QUICK-261123-LTJ]
duration: "Task 3 about 75 min; Tasks 1 and 2 not timed"
completed: 2026-11-23
---

# Quick Task 261123-ltj: One ignored .local folder Summary

**The per-phase `.phaseN` folders are replaced by one ignored `.local/` root. Toolchains, downloads and NuGet caches are shared once per repository from the main checkout, and runs and temp stay per checkout. Node 24.22.1 is pinned and installed once in the shared tools and reaches agent sessions through hooks. Every hash binding was regenerated by tracked tools. After the code review, 16 fix commits closed all nine warnings and seven of the ten Info findings, and every gate passes at `cfbbb5f`.**

All locations here are symbolic. The **main checkout** is the primary working tree. The **task worktree** is this task's linked worktree. The **shared root** is the main checkout's `.local/`. The **local root** is a checkout's own `.local/`. BASE is `88953aad1e7d7b6e8855e53c3692bdeeffe1ed29` (origin/dev).

## Performance

- **Duration:** Task 3 took about 75 minutes. Tasks 1 and 2 were not timed.
- **Completed:** 2026-11-23
- **Tasks:** 3 of 3
- **Commits:** 26, measured with `git rev-list --count BASE..HEAD`: 10 for the plan, 16 for the review fixes
- **Files changed vs BASE:** 114 (5797 insertions, 750 deletions); the review fixes changed 39 files (1357 insertions, 183 deletions)

## Accomplishments

- One resolver handles every location. [`workspace-paths.cjs`](../../../src/godot-game/tools/workspace-paths.cjs) and its C# twin `WorkspacePaths.cs` resolve the shared root in this order:
  1. `KZ_SHARED_LOCAL`;
  2. otherwise, the parent of the Git common dir;
  3. otherwise, the checkout's own `.local/`.
  The `isolationEnv()` it returns includes `NUGET_SCRATCH` and `KZ_SHARED_LOCAL`. Its `LEGACY_MAP` interprets historical `.phaseN` identities.
- Every live output moved under `.local/`:
  - the verification, gameplay, LAN and asset-pipeline runners;
  - bootstrap and NuGet;
  - the C# tests;
  - cleanup.

  The desktop and toolchain locks are shared. The check lock is per checkout.
- Three binding tools are now tracked and tested: ac-08, the asset cascade, and the gates. Before any hashed file changed, they were proven on the unchanged BASE bytes, and every gate was shown to fail.
- The asset cascade was regenerated once in Task 2, and ac-08 was regenerated last in Task 3.
- Node is pinned in `toolchain.json`:
  - `nodeArchives` holds the win32-x64 and linux-x64 SHA-256 values.
  - `node-setup/` contains the setup scripts, the session hooks and their tests.
  - The new tracked `.claude/settings.json` has a SessionStart hook.
  - `.codex/hooks.json` has an advisory SessionStart group.
- Cleanup keeps `tools`, `downloads` and `nuget`. The new dry-run-first migration was run only as a dry run, and it refused as designed.
- Docs were rewritten for the layout, sharing, `KZ_SHARED_LOCAL`, the Node setup and hooks, `core.longpaths`, the migration and cleanup. The worktree location rule moved from `cleanup.md` to `ai-collaboration.md`.
- PR #11 contradictions were fixed. Backlog 999.1 was recorded in ROADMAP and STATE. STATE has no 261123-ltj row in "Quick Tasks Completed"; the orchestrator adds it.

## Commits

| Commit | Subject |
| --- | --- |
| `0c16511` | feat(quick-261123-ltj): track the rebinding tools proven on unchanged bytes |
| `53b1d3b` | feat(quick-261123-ltj): resolve one shared .local root for verification and the C# tests |
| `58b39cc` | docs(quick-261123-ltj): link the Phase 5 specification without an unreadable anchor (orchestrator) |
| `887ed3d` | feat(quick-261123-ltj): move gameplay, LAN and asset-pipeline outputs to .local |
| `817a11b` | feat(quick-261123-ltj): keep shared caches in cleanup and add the legacy migration |
| `22d56c2` | fix(quick-261123-ltj): treat a zero-byte legacy dotnet-home as empty in the migration |
| `9f30b27` | feat(quick-261123-ltj): pin Node 24 and put the shared Node on agent session PATH |
| `2fecccb` | docs(quick-261123-ltj): document the .local layout, worktree rule, Node setup and migration |
| `c896b18` | docs(quick-261123-ltj): fix PR #11 contradictions and record the identifier backlog |
| `19f5818` | fix(quick-261123-ltj): regenerate ac-08 repository stages and caller bindings |

Review fixes, in order, on top of `19f5818`:

| Commit | Subject |
| --- | --- |
| `d207354` | fix(quick-261123-ltj): verify every migration row before any change and delete the VM last |
| `8b77f1c` | fix(quick-261123-ltj): refuse links in any folder of a migration or cleanup path |
| `b0ee77b` | fix(quick-261123-ltj): hold the shared toolchain lock while the migration applies |
| `5bec726` | fix(quick-261123-ltj): validate KZ_SHARED_LOCAL by one rule in every implementation |
| `d65cd11` | fix(quick-261123-ltj): refuse with GIT_MISSING instead of falling back without git |
| `b094936` | fix(quick-261123-ltj): refuse to write a shared root that Git does not ignore |
| `7cc3454` | fix(quick-261123-ltj): recheck the Node install after taking the toolchain lock |
| `1987c8c` | fix(quick-261123-ltj): make the Codex session hook commands fail open |
| `78c020e` | fix(quick-261123-ltj): restore the export guards' coverage of every workspace folder |
| `3b479ab` | fix(quick-261123-ltj): refuse asset-bindings --write on a dirty tree |
| `d3b00c8` | fix(quick-261123-ltj): require an exact =N on every residue --allow entry |
| `fbfc383` | fix(quick-261123-ltj): match Linux home folders and UNC shares in the newhost gate |
| `dc9960e` | fix(quick-261123-ltj): double commas in the VM launcher's QEMU drive paths |
| `69b76fa` | fix(quick-261123-ltj): refuse linked folders and hard links in the shared-file readers |
| `3d06ede` | docs(quick-261123-ltj): align the remaining Phase 5 lines with PR #11 |
| `cfbbb5f` | fix(quick-261123-ltj): regenerate ac-08 repository stages and caller bindings after the review fixes |

## Review fixes

Every finding of [the review](261123-ltj-REVIEW.md) was handled. Each new or changed test failed on the pre-fix code and passes after the fix; `results.md` records each before run.

| Finding | Commit | Fix | Proof (tests) |
| --- | --- | --- | --- |
| WR-01 migration order and dry-run strength | `d207354` | Planning verifies every move source against its pin and every target's parent folders before anything changes; the dry run runs the same checks. `--apply` moves first, then deletes, the VM last. A rerun skips completed rows. `setup.md` corrected | `workspace-migrate.test.cjs`: target parent not a folder; move source failing its pin (5 rows); apply order with the VM last; rerun after an interrupted apply (a simulated rename failure). The old "apply stops at the first failure" test was replaced |
| WR-02 linked ancestors | `8b77f1c` | Shared helper `linkedComponent` walks every component from the checkout root down. The migration refuses a source or target below any link, including `.phase1`, `.phase1/tools`, `.phase1/cache` and the `.local` targets. Cleanup keeps any output reached through a link and refuses to delete through one | migrate: junction `.phase1`, `.phase1/tools`, `.phase1/cache`, `.local`, `.local/downloads`, `.local/nuget`, nothing outside changes; cleanup: linked legacy entry, linked `.planning`, linked `.local`. Junctions work on Windows; a test skips with a named reason only where the OS cannot create one |
| WR-03, WR-04 the `KZ_SHARED_LOCAL` rule | `5bec726` | One rule with the same messages in `workspace-paths.cjs`, `WorkspacePaths.cs` and the new `node-setup/shared-root.sh` and `shared-root.ps1`, sourced by both setup scripts and both hooks. Windows: drive-qualified or UNC only; root-relative and MSYS `/d/...` forms are refused, never converted. No `.` or `..` segment, no filesystem or drive root, not the checkout or inside it except its own `.local`. Hooks write nothing and exit 0 on an invalid value; setup, bootstrap and the runners stop with `SHARED_ROOT_INVALID` | `session-path.test.cjs`: one 36-case table through JS, sh and PowerShell, identical answers; hooks and both setups for an override inside the checkout, relative and in MSYS form. `workspace-paths.test.cjs`: `roots()` refusals. C# spot runs: MSYS form and inside-checkout refused with the same messages |
| WR-05 unignored shared root | `b094936` | `assertSharedIgnored` (JS, also in `isolationEnv` and `--print-env`), bootstrap, the verification runner, the shared desktop lock, both setup scripts and C# `Isolate` run `git check-ignore` from the checkout that holds the shared root before the first write; otherwise `SHARED_ROOT_NOT_IGNORED`, naming the `/.local/` info/exclude line that `setup.md` documents. Nothing edits info/exclude | `workspace-paths.test.cjs`: `--print-env`, `isolationEnv`, bootstrap `--probe pins` refuse on a branch without `/.local/`, info/exclude unchanged, passes after the documented line; `session-path.test.cjs`: `setup.sh` and `setup.ps1` refuse, then install after the line |
| WR-06 export guards | `78c020e` | The packaged-member guard refuses every member under any `.local` folder and the legacy folders. The host-trace guard also refuses the `tools`, `downloads` and `nuget` folders of the resolved shared root, whatever its name, and of its real path; run folders stay allowed because the exported build lives there | new `export-guards.test.cjs`, both directions, an override named `kz-cache`, the main-checkout case where the shared root holds the run folder, and a linked shared root |
| WR-07 residue `=N` | `d3b00c8` | The residue gate fails, naming them, on `--allow` entries without `=N`. The final command carries `=N` on all 22 entries | `bindings.test.cjs` residue judge (the old passing control with an uncounted entry now fails); the earlier final command now exits 1 with `16 --allow entry without =N` |
| WR-08 Phase 5 vs Phase 11 | `3d06ede` | Docs only: `foundation-checks.md`, `lan-foundation-checks.md`, `pre-phase5-testing.md`, `verification.md` (including the reuse sentence at line 17) and three architecture decisions no longer give Phase 5 the work PR #11 moved. `setup.md`, `verification.md` and `lan-foundation-checks.md` say the catalog still declares the 8 cases `acceptancePhase: 5`. Catalog, register and rule matrix unchanged | `rebind-gates docs` PASS 36/36 |
| WR-09 concurrent Node setup | `7cc3454` | Both scripts check the install again after taking the lock and refuse to move onto an existing install. The PowerShell ignore check now captures git output before taking its first line (found by this test: a `.cmd` git wrapper lost its exit code in the cut pipeline, so the check was skipped) | `session-path.test.cjs`: a git shim creates the install between the pre-lock check and the lock; neither script nests a copy |
| IN-01 lock during `--apply` | `b0ee77b` | `--apply` holds `.local/locks/toolchain.lock`; a run with nothing to do takes no lock | migrate: lock held during every move and deletion; a lock taken after planning stops it with `TOOLCHAIN_LOCKED` |
| IN-02 Codex hook | `1987c8c` | Both Codex commands guard the checkout lookup and exit 0 on any failure | `session-path.test.cjs` runs the tracked commands through the platform shell inside and outside a repository |
| IN-03 `newhost` terms | `fbfc383` | Also matches `/home/<name>/` and UNC shares. Still compares with base: 0 new of 1814 | judge and CLI controls in `bindings.test.cjs` |
| IN-04 shared-file readers | `69b76fa` | Covered by the WR-02 helper: `sharedFile` and `legacyShared` refuse a link below the shared root; `sharedBytes` requires a single link | new `shared-files.test.cjs` |
| IN-06 git missing | `d65cd11` | `workspace-paths.cjs`, both setup scripts and C# stop with `GIT_MISSING`; none falls back to the checkout's own `.local` | `--print-roots` and both setup scripts with an empty `PATH`; C# spot run without git |
| IN-08 `asset-bindings --write` | `3b479ab` | Refuses a dirty tree like ac-08, except the three cascade files and `.planning`, `.claude`, `.codex`, `.agents` | `bindings.test.cjs` |
| IN-09 QEMU commas | `dc9960e` | Commas in `-drive file=` paths are doubled; the launcher only runs when executed directly | new `start-vm.test.cjs` |
| IN-05, IN-07, IN-10 | - | Not fixed: outside the requested fix scope (see Known gaps) | - |

## Results

Unless noted, every command runs from the task worktree root with Node v24.22.1 and dotnet 10.0.404. PATTERN is `'(?<![A-Za-z0-9_)\]])\.phase[0-9](?![0-9A-Za-z_.])'`.

### Final gates at the tested revision

The tested revision is `cfbbb5fc88155e964ce0498c00c005bf987bd260`. The tree was clean apart from the quick-task folder, and nothing was committed after the run. The command is the one in How to reproduce; every `--allow` entry carries `=N`.

| Command | Tested revision | Exit | Counts |
| --- | --- | --- | --- |
| `sh src/godot-game/tools/node-setup/setup.sh --check` | cfbbb5f | 0 | Node 24.22.1 (win32-x64) verified in the shared tools |
| gate `history` | cfbbb5f | 0 | 509 records unchanged |
| gate `newhost` | cfbbb5f | 0 | 0 new of 1814 machine-path lines (stricter terms) |
| gate `residue` | cfbbb5f | 0 | 234 allowed lines in 21 files, every entry counted |
| gate `lists` | cfbbb5f | 0 | verification 686, gameplay-checks 16, lan-checks 32, asset-pipeline 75 |
| gate `tests` | cfbbb5f | 0 | 120 passed in 16 files |
| gate `roots` | cfbbb5f | 0 | source=git, linked=true, override=false |
| gate `build` | cfbbb5f | 0 | locked restore without network GET; strict build 0 Warning(s), 0 Error(s) |
| gate `assets` | cfbbb5f | 0 | modules 423 = base 423; 180 valid; check passed |
| gate `ac08` | cfbbb5f | 0 | current; 6 callers; 9 staged plans; 4 registries |
| gate `docs` | cfbbb5f | 0 | N_base 36 = N_head 36; `PASS 36/36` |
| overall | cfbbb5f | 0 | `VERIFY EXIT 0`, 168 s |
| `setup.ps1 --check` | cfbbb5f | 0 | verified; the shared Node folders' listing identical before and after; nothing downloaded |

Per-file tools tests at `cfbbb5f`, all fail 0 and skipped 0 on Windows: asset-pipeline `export-guards` 2 and `shared-files` 2; lan-checks foreground 2, inherited 3, navigation 1, rendered-stream 1, revisions 3, selection 2, timing 7; `node-setup/session-path` 18; `repository-layout` 6; `verification/bindings` 19; `verification/linux-vm/start-vm` 1; `workspace-cleanup` 21; `workspace-migrate` 17; `workspace-paths` 15. Total 120.

Cascade hashes of the review fixes:

- Asset cascade: `asset-bindings --write` on the clean tree after the `export.cjs` and `content.cjs` commits wrote nothing; catalog `4389332f…`, delivery `efbdc8d6…` and pack index `e6dfdaf7…` are unchanged.
- ac-08, regenerated last at `3d06ede` and committed alone in `cfbbb5f`: verification `required-cases.json` `bff1cdb1c36e4b491fe798c54ca7297ae1f6a5b44fc8899bdb66975283dfbb7a` → `f4c121f005dbf616e5e72f1cb218475f9fb26c0eedcf973b918b122182489b33`; gameplay `required-cases.json` `1b3dbf94d55b5865e11c1c5a38b47689e8edf3f12fbafc0489303d5d676f009e` → `523ee9c1d03e2d89d0ee3e57b97f7a4242ce3095950e44b7ce6250c19002797d`; repositorySha256 → `0fd6bc36c75c001ad9e7a4ecaf268c1cc160176bd201fb59f9fe313f4aa06f1e`; rows 518. No `acceptancePhase` changed.

### Final gates before the review fixes (19f5818)

The tested revision was `19f5818870f2d0ffb65338bea70a4c95decdf0b3`.

The command was the plan's Task 3 verify, with the four entries Task 2 classified added to `--allow`: `cases/export-profile.cjs=1`, `cases/world.cjs=1`, `export.cjs=2` and `lan-checks/evidence.cjs=1`. Sixteen of its entries had no `=N`; the residue gate now refuses that command.

| Command | Tested revision | Exit | Counts |
| --- | --- | --- | --- |
| `sh src/godot-game/tools/node-setup/setup.sh --check` | 19f5818 | 0 | Node 24.22.1 (win32-x64) verified in the shared tools |
| gate `history` | 19f5818 | 0 | 509 records unchanged |
| gate `newhost` | 19f5818 | 0 | 0 new of 1746 machine-path lines |
| gate `residue` | 19f5818 | 0 | 214 allowed lines in 21 files (table below) |
| gate `lists` | 19f5818 | 0 | verification 686, gameplay-checks 16, lan-checks 32, asset-pipeline 75 |
| gate `tests` | 19f5818 | 0 | 96 passed in 13 files |
| gate `roots` | 19f5818 | 0 | source=git, linked=true, override=false; shared = local with `--root` = main checkout |
| gate `build` | 19f5818 | 0 | locked restore without network GET; strict build 0 Warning(s), 0 Error(s) |
| gate `assets` | 19f5818 | 0 | modules 423 = base 423; 180 valid; check passed |
| gate `ac08` | 19f5818 | 0 | current; 6 callers; 9 staged plans; 4 registries |
| gate `docs` | 19f5818 | 0 | N_base 36 = N_head 36; `PASS 36/36` |
| overall | 19f5818 | 0 | `VERIFY EXIT 0`, 2 min 9 s |

### Per-file tools tests at 19f5818

Every file reported fail 0 and skipped 0 on Windows.

| File | Pass |
| --- | --- |
| `lan-checks/foreground.test.cjs` | 2 |
| `lan-checks/inherited.test.cjs` | 3 |
| `lan-checks/navigation.test.cjs` | 1 |
| `lan-checks/rendered-stream.test.cjs` | 1 |
| `lan-checks/revisions.test.cjs` | 3 |
| `lan-checks/selection.test.cjs` | 2 |
| `lan-checks/timing.test.cjs` | 7 |
| `node-setup/session-path.test.cjs` | 11 |
| `repository-layout.test.cjs` | 6 |
| `verification/bindings.test.cjs` | 17 |
| `workspace-cleanup.test.cjs` | 20 |
| `workspace-migrate.test.cjs` | 12 |
| `workspace-paths.test.cjs` | 11 |
| Total | 96 |

### BASE tool proofs (Task 1a)

HEAD was BASE, and the tools were still untracked.

| Command | Tested revision | Exit | Counts |
| --- | --- | --- | --- |
| `node --test` over the 9 tracked tools tests | BASE | 0 | 39 pass, 0 fail |
| each runner `--list` from the system temp folder | BASE | 0 ×4 | 686 / 16 / 32 / 75 |
| `ac08-bindings.cjs --check --rev HEAD` | BASE | 0 | rows 498, checked 230, sourceStages 15, callers 6, repositorySha256 `4a795aab…` |
| `ac08-bindings.cjs --check --rev 18e77bc` | 18e77bc | 0 | same as BASE |
| `asset-bindings.cjs --check` | BASE | 0 | modules 423, valid 180 |
| `asset-bindings.cjs --write` on the unchanged tree | BASE | 0 | wrote nothing; the 5 catalog files were byte-identical before and after |
| `node --test src/godot-game/tools/verification/bindings.test.cjs` | BASE | 0 | 17 pass. Coverage: byte-for-byte ac-08 reproduction; gzip 9 vs 6; the negative controls; `AC08_DIRTY_TREE`; ten gate judges with paired passes; CLI controls on a temporary repository |

### Gate controls (forced runs on the real tree, Task 1a)

HEAD was BASE, and the tools were still untracked. Each command is `rebind-gates.cjs <args> --pattern PATTERN <gate>`.

| # | Arguments and gate | Tested revision | Exit | Named rows |
| --- | --- | --- | --- | --- |
| F1 | `--base 18e77bc history` | BASE | 1 | the three PR #11 records `05-CONTEXT.md`, `05-PHASE1-HANDOFF.md`, `05-SPEC.md` |
| F2 | `--base 18e77bc --keep docs/workflow/verification.md history` | BASE | 1 | the same three plus `docs/workflow/verification.md` |
| P1 | `--base BASE history` | BASE | 0 | 470 records unchanged |
| F3 | `--base 47efd80… newhost` (root commit) | BASE | 1 | 1746 new machine-path lines |
| P2 | `--base BASE newhost` | BASE | 0 | 0 new of 1746 |
| F4 | `--scope …/verification/check.cjs residue`, no `--allow` | BASE | 1 | `check.cjs=3` outside allow |
| P3 | the same with `--allow …/check.cjs=3` | BASE | 0 | 3 allowed lines |
| F5, F6 | the same with `=2`, `=4` | BASE | 1, 1 | retains 3, expected 2 / 4 |
| F7 | Task 1 scope `residue`, no `--allow` | BASE | 1 | 29 files; counts equal to `git grep -c` |
| F8 | `--base a305b8b lists` | BASE | 1 | base catalog absent |
| F9 | `--base a305b8b docs` | BASE | 1 | base catalog absent; failed before launching the runner |
| P4 | `--base BASE lists` | BASE | 0 | 686 / 16 / 32 / 75 |
| self-check | `--scope <4 tool files> history newhost residue` | 0c16511 | 0 | 470 unchanged; 0 new; 0 residue lines |

### Task 1b entry check and Task 1 verify

| Command | Tested revision | Exit | Counts |
| --- | --- | --- | --- |
| `git log`, `git diff --name-only`, `git status` (entry check) | 0c16511 | 0 | only the tools commit, exactly 4 files, clean |
| `bindings.test.cjs`; `asset-bindings --check`; `ac08 --check --rev BASE` and `--rev 18e77bc` | 0c16511 | 0 | 17 pass; 423 / 180; both current |
| `ac08-bindings.cjs --check --rev HEAD` (failing direction) | 0c16511 | 1 | 11 differences: the 4 tool rows, `sourceSetSha256` and 6 caller hashes |
| `rebind-gates --base BASE history newhost lists tests` | 0c16511 | 0 | 470; 0 of 1746; 686/16/32/75; 56 in 10 files |
| `workspace-paths.test.cjs` + `bindings.test.cjs` | 53b1d3b | 0 | 28 pass |
| Task 1 gates `history newhost residue lists tests roots assets build` | 53b1d3b | 0 | 502; 0 of 1746; 28 lines in 4 files; 686/16/32/75; 67 in 11 files; linked; 423; 0/0 without GET |
| gate `docs` | 53b1d3b | 1 | runner FAIL 32/36: two `DOC_ANCHOR` links from PR #11 into `05-SPEC.md#acceptance-disposition`, also present at BASE |
| gate `docs` after the orchestrator fix | 58b39cc | 0 | `PASS 36/36` |

### Task 2

| Command | Tested revision | Exit | Counts |
| --- | --- | --- | --- |
| entry `rebind-gates --base BASE lists tests assets newhost` | 58b39cc | 0 | 686/16/32/75; 67 in 11 files; 423; 0 of 1746 |
| `asset-bindings.cjs --check` before `--write` | working tree | 1 | 21 stale rows |
| `asset-bindings.cjs --write`, then `--check` | working tree | 0, 0 | modules 423, valid 180. Catalog `02e88278…` → `4389332f…`, delivery `d86f07d3…` → `efbdc8d6…`, pack index `d0d7032b…` → `e6dfdaf7…` |
| commit-A gate check (plan allow set only) | 887ed3d | 1 | residue named exactly the 4 later-classified files |
| Task 2 verify (gates plus the migration dry run) | 22d56c2 | 0 | history 504; newhost 0 of 1746; residue 135 lines in 10 files; lists 686/16/32/75; tests 85 in 12 files; assets 423 |
| informational `roots build docs` | 22d56c2 | 0 | linked; 0/0 without GET; 36/36 |
| `ac08-bindings --check --rev HEAD` | 22d56c2 | 1 | 46 differences (stale until Task 3) |

### Node setup and session hooks (Task 3)

| Command | Tested revision | Exit | Counts |
| --- | --- | --- | --- |
| `node --test src/godot-game/tools/node-setup/session-path.test.cjs` | working tree, then 9f30b27 | 0 | 11 pass, 0 fail, 0 skipped |
| mutation: drop the hook's `grep -qxF` guard; skip `assert_archive "$part"` | scratch edit, restored | 1 | the idempotence and mismatch tests fail (8 / 2) |
| mutation: drop the `cygpath -m` conversion in both scripts | scratch edit, restored | 1 | the linked-worktree setup.sh test and the hook git-common-dir test fail (9 / 2) |
| first `setup.ps1` run | working tree | 0 | Downloaded the pinned zip from nodejs.org (verified), but installed it into the task worktree's own `.local/` because of a recursion bug. See Deviations |
| `setup.ps1 --archive <that verified zip>` | working tree | 0 | Installed into the shared root's `tools/node/node-v24.22.1-win-x64/` with no second download; printed one PowerShell PATH line |
| `setup.ps1` rerun | working tree | 0 | `already installed … nothing downloaded`; identical line; archive mtime unchanged |
| `setup.ps1 --check`; `setup.sh --check`; `setup.sh` rerun | working tree | 0, 0, 0 | verified; `nothing downloaded`; the sh line names the shared root |
| shared archive sha256 / shared `node.exe --version` | - | 0 | `d2046d28c2c3dcd211ac5145c0bf30e2047ef9dfb306693c8c6e2eccd8b95fa5` = pin / v24.22.1 |
| `session-path.sh` with a scratch `CLAUDE_ENV_FILE`, using a backslash and an MSYS `CLAUDE_PROJECT_DIR` | working tree | 0, 0 | 1 line, byte-equal to the setup.sh line. Sourced in a bare `PATH=/usr/bin:/bin` shell, `node` resolves to the shared root and reports v24.22.1 |
| `session-path.sh --codex`; `session-path.ps1` | working tree | 0, 0 | each prints the advisory line naming the shared root |
| Linux branch: `KZ_SHARED_LOCAL=<system-temp scratch> setup.sh --platform linux-x64 --archive <main checkout legacy linux-prep tar.xz>` | working tree | 0 | sha256 `6c172a9a…` = pin; `bin/node` is ELF x86-64, which confirms RESEARCH A1. `--check` 0; rerun `nothing downloaded`; source unchanged; scratch deleted |
| documented PowerShell and sh isolation commands (`--print-env`) | working tree | 0 | isolation variables set, including `NUGET_SCRATCH` and `KZ_SHARED_LOCAL` |

### Docs, planning records and ac-08 (Task 3)

| Command | Tested revision | Exit | Counts |
| --- | --- | --- | --- |
| gate `docs` on the edited docs | working tree on 9f30b27 | 1 | `DOC_TARGET` for links into `.claude/settings.json` and `.codex/hooks.json`; replaced by code spans |
| gate `docs` rerun; again after the STATE/ROADMAP edits | working tree | 0, 0 | 36/36, 36/36 |
| C7 condition: ROADMAP Coverage total row | c896b18 | - | 136 and 19 complete still held, so the sentence was changed |
| `history newhost residue lists tests roots` before ac-08 | c896b18 | 0 | 509; 0 of 1746; 214 in 21; 686/16/32/75; 96 in 13; linked |
| `ac08-bindings.cjs --check --rev HEAD` before writing | c896b18 | 1 | 51 differences |
| `ac08-bindings.cjs --write` | c896b18 | 0 | rows 513, callers 6, repositorySha256 `b190c355e9e9bd750aed1bd18a108c0902a72017e71742d757e7b670ff249c21` |
| `ac08-bindings.cjs --check --rev HEAD` | 19f5818 | 0 | rows 513, checked 231, sourceStages 15, callers 6 |

ac-08 catalog hashes:

- verification `required-cases.json`: `91480d7d76fe5ac708a070ab430d0dd5830f27b42b9fa3682b79bfab5780c68b` → `bff1cdb1c36e4b491fe798c54ca7297ae1f6a5b44fc8899bdb66975283dfbb7a`;
- gameplay `required-cases.json`: `877e732c08776fdb96adddb363cb78796910a5eda371e96d4d5eb50ff009dfef` → `1b3dbf94d55b5865e11c1c5a38b47689e8edf3f12fbafc0489303d5d676f009e`.

### Migration dry run on the main checkout

| Command | Tested revision | Exit | Counts |
| --- | --- | --- | --- |
| `node src/godot-game/tools/workspace-migrate.cjs` | 22d56c2, 19f5818 and cfbbb5f | 1, 1, 1 | Refused as required, and nothing was changed (details below) |

Details of each dry run:

- The first row was `delete .phase1/tools/linux-vm (8.2 GB; …)`.
- Two `refuse:` lines named the missing `workspace-paths.cjs` and the main checkout's `.gitignore` not ignoring `.local/`.
- The moves would cover:
  - the Godot zip (111.2 MB) and `SHA512-SUMS.txt`;
  - the export templates (1.1 GB);
  - the editor (257.3 MB);
  - `linux-prep` (397.7 MB);
  - the feed (3.3 MB).
- The deletions would cover:
  - the duplicate packages (575.8 MB) and http-cache (59.6 MB);
  - `cli-home` (36.0 MB);
  - the stray lock file;
  - `dotnet-home` (0 B).

No-write proof for the first two runs: the main checkout's `status --porcelain` had 0 lines. The name/size/mtime listing of its `.phase1` and `.local` was identical before and after: 9417 entries at 22d56c2 and 11881 at 19f5818. `--apply` was never passed.

The run at `cfbbb5f` lists the same rows in the same order, with the VM deletion first (8.2 GB). Since WR-01 it also verified every move source against its pin before listing it, and none was refused for a pin. The two refusals are unchanged. No-write proof: `git --no-optional-locks -C <main checkout> status --porcelain` had 0 lines before and after; the name/size/mtime listing of its `.phase1` and `.local` (11881 entries), its HEAD and its branch were identical before and after. `--apply` was never passed.

## Residue table (remaining `.phaseN` hits at cfbbb5f)

Every row is an `--allow` entry with its exact `=N`. Since `19f5818` only the fixtures grew: `workspace-cleanup.test.cjs` 52 → 55 and `workspace-migrate.test.cjs` 56 → 73, for the new link, pin and lock tests. `workspace-paths.test.cjs=0` asserts that file builds every legacy name from fragments.

| File | Lines | Class | Reason |
| --- | --- | --- | --- |
| `.planning/STATE.md` | 2 | historical | Decisions and concern text describing the pinned `.phase4` recipes |
| `docs/architecture/repository-layout.md` | 16 | legacy map | The historical-path rows `.phaseN → .local/…` |
| `docs/workflow/cleanup.md` | 4 | legacy map | The legacy keep/delete rows and legacy locks |
| `docs/workflow/setup.md` | 2 | legacy map / historical | The former VM folder and the migration's source folder |
| `docs/workflow/engine-defects.md` | 1 | historical | Past-tense record of a deleted trace folder |
| `src/godot-game/tools/README.md` | 3 | legacy map / captured | The legacy report folders and the recipes that assert `.phase4/` |
| `src/godot-game/tools/workspace-paths.cjs` | 23 | legacy map | `LEGACY_MAP` |
| `src/godot-game/tests/verification/WorkspacePaths.cs` | 23 | legacy map | The C# rows of the same map |
| `src/godot-game/tests/verification/ExemptionValidator.cs` | 3 | logical identity / guard | Logical `.phase1/feed` identities (rule 3) and the `.phase1/packages/` prefix that gates the shared-package exception (rule 5) |
| `src/godot-game/tests/verification/DocumentationValidator.cs`, `ProjectEvaluator.cs` | 1, 1 | legacy exclusion | Kept beside `.local` (rule 6) |
| `src/godot-game/tools/asset-pipeline/check.cjs` | 1 | captured | The tracer recipe `is_relative_to(...)` (rule 4) |
| `src/godot-game/tools/asset-pipeline/cases/audio.cjs` | 3 | captured | The ASSEMBLE, PROMOTE and INTERFACE_ASSEMBLE asserts (rule 4) |
| `src/godot-game/tools/asset-pipeline/cases/export-profile.cjs` | 1 | logical identity | `CONTRACT.checkpoint`, compared with the pinned `export-profile.json` |
| `src/godot-game/tools/asset-pipeline/cases/world.cjs` | 1 | captured | The emulated `__file__` of the hash-pinned retained world recipe |
| `src/godot-game/tools/asset-pipeline/export.cjs` | 2 | legacy guard | `authoringMember` and `authoringTrace` still reject the legacy folders (rules 5 and 6). Since WR-06 the member guard rejects all of `.local`, and the trace guard also rejects the resolved shared root's `tools`, `downloads` and `nuget` |
| `src/godot-game/tools/lan-checks/evidence.cjs` | 1 | logical identity | The JSON property `inherited?.phase1`, not a folder |
| `src/godot-game/tools/workspace-cleanup.cjs`, `.test.cjs` | 2, 55 | legacy map | The legacy rules and their fixtures |
| `src/godot-game/tools/workspace-migrate.cjs`, `.test.cjs` | 16, 73 | legacy map | The migration rows and their fixtures |
| `src/godot-game/tools/workspace-paths.test.cjs` | 0 | - | Allowed with `=0`: legacy names are built from fragments |

`--keep` files are excluded from `residue` and keep their bytes:

- `docs/code`;
- `production/assets`;
- `platform-proof.md`;
- `phase4-layout-integration.md`;
- `extract-media.py` and `extract-audio.py`;
- `specs`;
- `REQUIREMENTS.md`.

## Not run and expected-blocked

| Item | Status | Reason |
| --- | --- | --- |
| Export, editor import, headless launch, game windows | Not run | Out of scope (CONTEXT: Verification) |
| The full case selection (686 / 16 / 32 / 75) | Not run | Only the lists, the docs group and the strict build ran |
| Bootstrap acquisition (`bootstrap.cjs --probe engine`) | Not run | It would download Godot. The shared-root exception and `TOOLCHAIN_LOCKED` path are covered by code review and `workspace-paths.test.cjs` only. Its new `SHARED_ROOT_NOT_IGNORED` refusal ran in a temporary repository with `--probe pins`, which stops before any download |
| The C# twin's new rules | Spot runs only | `OverrideProblem`, `GIT_MISSING` and the `Isolate` ignore check have no xunit case: a new case would need a catalog row and change the 686 list count. Four direct `pins.consumer` runs showed the MSYS and inside-checkout refusals, `GIT_MISSING` without git, and a pass with the valid shared root |
| The Codex hook under Codex | Not run | Codex is not installed here. `session-path.sh --codex` and `session-path.ps1` were run directly; the `commandWindows` quoting under Codex is RESEARCH assumption A2 |
| The Claude Code hook inside a new Claude Code session | Not run | The script was run with a scratch `CLAUDE_ENV_FILE`; first-session hook acceptance is assumption A6 |
| Linux setup branch | Run | Under Git Bash with `xz`, `--platform linux-x64` only; native Linux execution of `bin/node` was not run |
| The C# shared-package exception (`IsSharedPackage` in `CheckHash`) | Compiled; reached by R-50.positive, not proven executed for a package row | Whether a `.phase1/packages/` row reaches `CheckHash` is not traced (RESEARCH A5) |
| `verification/linux-vm/start-vm.cjs` | Syntax only (`node --check`) | Never run, per plan |
| `workspace-migrate.cjs --apply` | Not run | The creator runs it after merge. The temporary-repository tests cover the order, the lock and the rerun after an interruption |
| `export.layout`, `export.profile`, `rules.export-layout-origin` / `-boundary`, the `ContentNativePermissionTests` model tests, `GeneratedExportLayoutTests` (2), Phase 2 `--verify-completion` | Expected-blocked | They read inputs the creator declared lost on 2026-11-23 |
| Phase 4 `runner.prelaunch`, `runner.selection`, `acceptance.judgments`, `acceptance.artifacts`, `--verify-completion` | Expected-blocked | Phase 11 under TEST-12 (05-CONTEXT D-46) |
| Export tracer, audio `--prepare` / `--approve-actions` / `--prepare-interface`, media conversion controls, audio-feasibility extraction | Expected-blocked with `CAPTURED_ROOT_REQUIRED` | Pinned recipes assert the original checkout's `.phase4/` folder; Phase 5 makes them work (D-46) |
| `rules.xunit-producer-hash` | Failed in a Task 1 spot run; not fixed | A pre-existing Windows path-separator mismatch in `BuildFixture.HasDiagnostic`, independent of this move |

## Known gaps

- **The executable catalog still assigns the eight coverage cases to Phase 5.** `src/godot-game/tools/verification/required-cases.json` declares them with `acceptancePhase: 5` (0 rows say 11), `docs/code/register.json` names owner Phase 5 and `docs/code/rule-matrix.md` says "Pending Phase 5", so `check.cjs --phase 5` still selects them. PR #11 moved their certification to Phase 11 under TEST-12 but left the catalog change to the Phase 5 implementation. This task changed prose only: `setup.md`, `verification.md` and `lan-foundation-checks.md` now say the catalog still marks them `acceptancePhase: 5` until Phase 5 reassigns them, and the STATE `[Phase 11]` concern records the gap. `docs/workflow/platform-proof.md:131` (a `--keep` history record) says the same as the catalog. The catalog, register and rule matrix are hash-bound and were not changed.
- The other live Phase 5 lines that gave Phase 5 the work PR #11 moved (`foundation-checks.md:3,63`, `lan-foundation-checks.md:34,44`, `pre-phase5-testing.md:7`, `verification.md:17,38` and three architecture decisions) were corrected in `3d06ede`. STATE's 2026-11-15 decision-log line ("Phase 5 retains hosted Windows/Linux compatibility…") is a dated decision record and was left as written.
- "Git missing" is no longer a silent fallback. With no `git` on `PATH`, `workspace-paths.cjs`, both Node setup scripts, bootstrap (through the resolver), the runners and the C# tests stop with `GIT_MISSING`. None of them falls back to the checkout's own `.local/` or downloads there. A folder with `git` available but no repository still uses its own `.local/`, as the contract requires. Before this fix, two stray Node downloads happened this way (deviations 8 and 10).
- Not fixed, outside the requested fix scope:
  - IN-05: the C# `Local()` helpers map legacy identities to the shared root without the `IsSharedPackage` checks. The reads are hash-checked, but `repository-layout.md:30` overstates the containment.
  - IN-07: `isolationEnv` honours `PHASE1_PACKAGE_CACHE` for every runner, which is undocumented.
  - IN-10: `legacyTarget` duplicates the legacy-map lookup. It is correct while the migration refuses any override.
- The `KZ_SHARED_LOCAL` rule refuses a folder inside the checkout but not a folder that contains it. Source copies run from the main checkout live under its `.local/temp/`, inside the shared root, so refusing containment would break them. An override that contains another checkout's tracked tree is still refused for writes by `SHARED_ROOT_NOT_IGNORED`.
- `tr`-based case folding in `shared-root.sh` folds ASCII letters only, while the JS, PowerShell and C# rules fold Unicode. The rules disagree only for a checkout path with non-ASCII capitals that differ only in case from the override.
- The handoff counted seven `CAPTURED_ROOT_REQUIRED` entry points; the code has six call sites. The docs list those six: the export tracer; the three audio steps; the media conversion controls; the audio-feasibility extraction.
- Pre-existing issue: `.claude/settings.local.json` and the GSD group in `.codex/hooks.json` embed a machine-specific path. Both are GSD-owned and left unchanged.
- On Linux, the two Windows-only tests in `session-path.test.cjs` are skipped with a named reason: the `session-path.ps1` hook and `setup.ps1`.
- The shared root's `locks/` folder exists and is empty. Its presence is harmless.
- Supersession note (C2): the fsu `state` gate pattern (`…runner\.prelaunch.*Phase 5`) is not carried into the tracked tools, because it encodes the superseded "Phase 5 renews" handoff. The fsu records keep their bytes.

## PR #11 contradictions

| # | Where | Disposition |
| --- | --- | --- |
| Anchor | STATE.md:192, ROADMAP.md:593 `05-SPEC.md#acceptance-disposition` | Fixed by the orchestrator in `58b39cc` (anchor dropped); not re-added |
| C1 | STATE `[PR #6 relocation]` | The pinned recipes now fail fast with `CAPTURED_ROOT_REQUIRED` in every checkout, naming 05-CONTEXT D-46, until Phase 5 makes them work; the review-bound gates stay with Phase 11 (TEST-12) |
| C2 | fsu `state` gate pattern | Not carried over; noted above |
| C3 | `verification.md` "Existing Phase 1 Commands and Evidence" | Coverage and reuse ownership changed to Phase 11 under TEST-12. The same fix was applied to `verification.md:49` and `setup.md:5` / `:91` (deviation 9) |
| C4 | `cleanup.md` reuse mechanism | Now "evidence reuse validation in Phase 11 (TEST-12)" |
| C5 | STATE Session Continuity | Dropped the PR #5 "ready for review" and "merges through its own planning PR" sentences; PR #11 recorded as merged in `88953aa`; Last session and Stopped at refreshed |
| C6 | `repository-layout.md` generated-state paragraph | Rewritten for `.local/`, sharing, `KZ_SHARED_LOCAL` and the resolver; legacy rows added |
| C7 | ROADMAP "Fourteen of the current 135" | The condition held (136 total, 19 complete), so it now reads "Nineteen of the current 136" |
| `ai-collaboration.md:51` | Phase 5 reuse sentence | Reuse validation moved to Phase 11 (TEST-12); Phase 5 keeps dependency-aware selection; the precedence note is kept |
| Review WR-08 | `foundation-checks.md:3,63`, `lan-foundation-checks.md:34,44`, `pre-phase5-testing.md:7`, `verification.md:17,38`, architecture decisions 0001 (two files) and 0003 | Fixed in `3d06ede`; the catalog gap stays open (Known gaps) |

## Deviations from Plan

### Task 1

1. **[Rule 1 - Bug] SCR0007 line length.** The first `build` gate failed with 3 errors on edited test lines. The lines were wrapped before commit, and the rerun gave 0/0.
2. **[Orchestrator fix] PR #11 anchors.** The docs gate failed 32/36 at `53b1d3b`. The cause was two `#acceptance-disposition` anchors added by PR #11, present at BASE. The validator reads `.planning/phases/**` as empty, so these anchors can never resolve. The orchestrator dropped the anchor from STATE.md:192 and ROADMAP.md:593 in `58b39cc`, and docs then passed 36/36.
3. **Inventory count.** The Task 1 inventory showed 90 files and 894 lines against planning guidance of 89 and 892, because STATE.md is included. This was guidance only.
4. **Not fixed: `rules.xunit-producer-hash`.** A spot run showed a pre-existing Windows path-separator mismatch in `BuildFixture.HasDiagnostic`. It is out of scope.

### Task 2

5. **[Classified residue] Four `--allow` entries were added**, each with its reason in the residue table: `cases/export-profile.cjs=1`, `cases/world.cjs=1`, `export.cjs=2` and `lan-checks/evidence.cjs=1`.
6. **[Rule 1 - Bug] `22d56c2`.** The migration refused a `dotnet-home` folder that holds only zero-byte .NET sentinels. That folder now counts as empty.
7. **Design detail.** `content.cjs` gained the helpers `runsPath`, `tempPath`, `feedPath`, `legacyLocal`, `legacyShared` and `sharedFile`. `evidence.cjs` records the Godot editor as `shared:tools/godot/…`.

### Task 3

8. **[Rule 1 - Bug] `setup.ps1` recursion.**
   - The helper function `Git` shadowed the `git` command (PowerShell names are case-insensitive) and recursed. The shared root then fell back to the checkout.
   - So the first real run downloaded the pinned zip from nodejs.org (verified against the pin) into the task worktree's own `.local/`.
   - Fix: the helper was renamed to `Read-Git`, and a Windows test now proves resolution from a linked worktree.
   - The shared install was then seeded with `--archive` from that verified download, so there was no second download for the shared install. The stray worktree copy was deleted.
9. **[Rule 1 - Bug] `MSYS_NO_PATHCONV`.**
   - Under the repository shell rules, native Git for Windows cannot read the MSYS `/d/…` folder that `pwd` returns.
   - `setup.sh` therefore resolved the checkout's own `.local/`, and `session-path.sh` silently did nothing for an MSYS `CLAUDE_PROJECT_DIR`.
   - Both now convert with `cygpath -m` before calling Git. This departs from the RESEARCH hook script ("exactly as RESEARCH"). Tests cover both scripts.
10. **Second stray Node download.**
    - While checking the documented PowerShell command after the docs commit, a scratch run of `setup.ps1` without `git` on `PATH` used the non-git fallback.
    - It downloaded the pinned zip from nodejs.org a second time into the task worktree's `.local/`, verified it and unpacked it.
    - It was found and deleted after the final verify. No tracked file was involved, and the shared install was untouched.
    - The download was the same approved, pinned archive. This is recorded as a known gap.
11. **Tests beyond the behavior block.** `session-path.test.cjs` also covers:
    - `setup.ps1`, Windows only: seed, mismatch refusal, idempotence and linked-worktree resolution;
    - `setup.sh` from a linked worktree under `MSYS_NO_PATHCONV=1`;
    - the hook with an MSYS project folder.
12. **Docs links.** The docs checker rejects links into `.claude/` and `.codex/` (`DOC_TARGET`), so `setup.md` names those files in code spans.
13. **Contradictions beyond C1–C7.**
    - `setup.md:5` and `:91`, and `verification.md:49`, still assigned coverage and certification to Phase 5. They now name Phase 11 under TEST-12, the same class as C3 (CONTEXT: fix contradictions with PR #11).
    - The remaining ones are listed under Known gaps.
14. **STATE frontmatter.** `stopped_at` was refreshed together with the body's "Stopped at", for consistency.
15. **`.codex/hooks.json`.** The GSD group's entries are unchanged. Only its closing brace gained the JSON separator comma before the new group.

### Review fixes

16. **[Choice] MSYS form refused, not converted (WR-03).** Every implementation refuses a root-relative or `/d/...` override on Windows with the same message. Converting would have needed `cygpath` in the Node, PowerShell and C# paths. `setup.md` tells Git Bash users to write `<drive>:/…`.
17. **[Choice] Shared rule scripts.** The sh and PowerShell rule lives once in `node-setup/shared-root.sh` and `shared-root.ps1`. Each is sourced by its setup script and hook, so four scripts hold no copy. A missing rule file makes a hook exit 0.
18. **[Choice] Ignore check scope (WR-05).** The check runs `git check-ignore` on the shared root itself, from the working tree that contains it. A shared root outside every working tree, such as a CI or temporary override, needs no check. `isolationEnv` asserts too, because children write the shared NuGet folders.
19. **[Choice] Trace guard (WR-06).** The trace guard does not reject all of `.local`: the exported build under test lives in `.local/runs/asset-pipeline/`, so it rejects only the shared root's `tools`, `downloads` and `nuget` folders, as plan rule 5 requires.
20. **[Rule 1 - Bug] PowerShell ignore check skipped behind a git wrapper.** Found by the WR-09 race test. `Get-KzIgnoreProblem` piped `git` into `Select-Object -First 1`, which stopped a `.cmd` wrapper early and lost its exit code, so the check returned "no problem". It now captures the output first. Fixed in `7cc3454`, in the same commit as the test that found it.
21. **[Rule 1 - Bug] Build failure in the GIT_MISSING commit.** The first `d65cd11` attempt was committed although the build gate had failed (SCR0009, collection-element indentation): the shell chain ignored the gate's exit code. The commit was amended with the fix before any later commit, and nothing was pushed. Later commits checked the gate exit code first.
22. **[Amend] `fbfc383`** was amended once: its new comment named a UNC example, which the stricter `newhost` then flagged. The comment now describes the shape in words.
23. **[Commit prefix] `3d06ede`** uses `docs(quick-261123-ltj):` because it changes docs only. The fix scope asked for `fix(...)`. Correcting it would need a rebase, which is not allowed.
24. **[Scope] STATE concern.** The catalog gap is recorded in the STATE `[Phase 11]` concern, the one that lists the eight cases. That concern was not among those edited earlier.
25. **[Scope] Architecture decisions.** Three decision records stated Phase 5 ownership of the moved certification in the present tense. Each keeps its original amendment history and gains the 2026-11-23 move.
26. **[Tests] C# twin.** No xunit case was added, because a new case would need a catalog row and change the 686 count. The twin was proven by direct spot runs (see Not run).
27. **[Info] Uncounted entries.** The earlier final command had 16 uncounted `--allow` entries, not 15 as the review counted.

### Kept by decision (reported as the plan requires)

- **Legacy ignore line kept.** `.gitignore` adds `/.local/` and keeps `/.phase[0-9]*/` on its own line. Without it, about 9 GB of legacy folders in the main checkout would become untracked and addable after it switches to `dev`. The line also lets the migration require its sources to be ignored. So "replaces" is implemented as "adds `.local/` and keeps the legacy line until `workspace-migrate.cjs` retires the folders".
- **Per-checkout feed under `nuget/`.** The verified Godot feed is `L/nuget/feed`, not shared, for three reasons:
  - `NuGet.config` keeps the relative `../../.local/nuget/feed`;
  - bootstrap's `PINS_MISMATCH` requires that exact value;
  - `ExemptionValidator.CheckHash` keeps feed reads inside the root.

  The CLI home is `L/temp/cli-home`. The layout keeps the creator's six subfolders.

## Task-created ignored paths

These are for task-end cleanup; never use `git clean`.

- Task worktree:
  - `.local/temp/`: scratch, logs and this task's records in `.local/temp/quick-261123-ltj/`, plus `cli-home`, `lan-checks`, `gameplay-checks`, `rebind-gates`, the build and docs folders, and MSBuild and .NET temp;
  - `.local/runs/verification/`: the docs-gate runs;
  - `.local/nuget/feed/`: 4 verified nupkgs, copied from the main checkout's legacy feed;
  - `.local/locks/`: empty;
  - the legacy `.phase1/` and `.phase3/` folders, created by the BASE tests in Task 1;
  - from the review fixes: `.local/temp/quick-261123-ltj/` gained `verify-review-fix.txt`, `per-file-tests-review-fix.txt`, `rf-migrate-dry-run.txt`, the `rf-main-*` snapshots and `csharp-spot/` (the C# spot-run results); `.local/temp/rebind-gates/` and `.local/runs/verification/` gained build logs and docs-gate runs.
- Shared root:
  - `nuget/packages/`: 580 MB, copied from the main checkout's legacy packages;
  - `nuget/http/`, `nuget/scratch/`;
  - `tools/node/node-v24.22.1-win-x64/`;
  - `downloads/node/node-v24.22.1-win-x64.zip`;
  - `locks/`: empty.
- Repository-local exclude: the common `info/exclude` holds the `/.local/` line the orchestrator added before the first install.
- System temp folder: the temporary repositories of the tests are removed by the tests, including the new link, race and git-shim fixtures. The hook and Linux-branch scratch folders were deleted. The orchestrator's session Node download folder in its scratchpad is outside this task's cleanup.

## How to reproduce

From the task worktree root (or any checkout of the branch), in Git Bash, with Git and .NET SDK 10.0.404 on `PATH`:

```sh
export MSYS_NO_PATHCONV=1
# Node: install or verify the shared Node, then put it on PATH for this shell.
eval "$(sh src/godot-game/tools/node-setup/setup.sh)"
node --version   # v24.22.1
sh src/godot-game/tools/node-setup/setup.sh --check

# Roots and isolation.
node src/godot-game/tools/workspace-paths.cjs --print-roots
eval "$(node src/godot-game/tools/workspace-paths.cjs --print-env sh)"

# Session hook (scratch env file).
CLAUDE_ENV_FILE=$(mktemp) CLAUDE_PROJECT_DIR="$PWD" sh src/godot-game/tools/node-setup/session-path.sh

# Bindings.
node src/godot-game/tools/verification/ac08-bindings.cjs --check --rev HEAD
node src/godot-game/tools/verification/asset-bindings.cjs --check

# Every gate (about 3 minutes on Windows). Every --allow entry carries its exact =N.
node src/godot-game/tools/verification/rebind-gates.cjs --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29 \
  --keep docs/code,src/godot-game/production/assets,docs/workflow/platform-proof.md,docs/workflow/phase4-layout-integration.md,src/godot-game/tools/asset-pipeline/extract-media.py,src/godot-game/tools/asset-pipeline/extract-audio.py,specs,.planning/REQUIREMENTS.md \
  --allow .planning/STATE.md=2,src/godot-game/tools/workspace-paths.cjs=23,src/godot-game/tools/workspace-paths.test.cjs=0,src/godot-game/tools/workspace-cleanup.cjs=2,src/godot-game/tools/workspace-cleanup.test.cjs=55,src/godot-game/tools/workspace-migrate.cjs=16,src/godot-game/tools/workspace-migrate.test.cjs=73,src/godot-game/tests/verification/WorkspacePaths.cs=23,src/godot-game/tests/verification/ExemptionValidator.cs=3,src/godot-game/tests/verification/DocumentationValidator.cs=1,src/godot-game/tests/verification/ProjectEvaluator.cs=1,src/godot-game/tools/asset-pipeline/check.cjs=1,src/godot-game/tools/asset-pipeline/cases/audio.cjs=3,docs/architecture/repository-layout.md=16,docs/workflow/cleanup.md=4,docs/workflow/setup.md=2,docs/workflow/engine-defects.md=1,src/godot-game/tools/README.md=3,src/godot-game/tools/asset-pipeline/cases/export-profile.cjs=1,src/godot-game/tools/asset-pipeline/cases/world.cjs=1,src/godot-game/tools/asset-pipeline/export.cjs=2,src/godot-game/tools/lan-checks/evidence.cjs=1 \
  --pattern '(?<![A-Za-z0-9_)\]])\.phase[0-9](?![0-9A-Za-z_.])' \
  history newhost residue lists tests roots build assets ac08 docs

# Migration dry run on the main checkout (read-only; refuses until the main checkout is on a branch with .local).
node src/godot-game/tools/workspace-migrate.cjs; echo "exit $?"
```

Expected results:

- Every gate prints `PASS`, with the counts in the final table above.
- The migration dry run exits 1. Its first row is the VM deletion with its size, and it prints two `refuse:` lines.
- The PowerShell equivalents are `powershell -NoProfile -ExecutionPolicy Bypass -File src/godot-game/tools/node-setup/setup.ps1 | Out-String | Invoke-Expression` and `node src/godot-game/tools/workspace-paths.cjs --print-env powershell | Out-String | Invoke-Expression`.

After this branch merges, the creator migrates the main checkout as [setup](../../../docs/workflow/setup.md#moving-a-checkout-to-the-local-folder) describes:

1. Switch to `dev`; the squash-merged branch needs `git branch -D`.
2. Run `dotnet build-server shutdown`.
3. Run the migration dry run, then `--apply`.
4. Run the cleanup dry run.

## Self-Check: PASSED

- Every created file named in `key-files` exists at HEAD `cfbbb5f`, including the node-setup files and rule scripts, the three new test files, `.claude/settings.json` and the 999.1 `.gitkeep`.
- All 26 commits listed above are ancestors of HEAD; `git rev-list --count BASE..HEAD` = 26.
- Tree clean apart from the quick-task folder; this SUMMARY and the REVIEW are uncommitted.
- The main checkout: 0 tracked-status lines, branch and HEAD unchanged. During the review fixes its shared `.local/` was only read, apart from the documented `setup.sh --check` and `setup.ps1 --check`, which write nothing; its `.phase1/` was only read.
