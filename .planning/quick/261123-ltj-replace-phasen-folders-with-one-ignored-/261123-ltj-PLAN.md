---
phase: quick-261123-ltj
plan: 01
type: execute
wave: 1
depends_on: []
# The live `.phaseN` inventory that authorizes edits comes from a fresh `git grep` at the start of each task
# (see "Live inventory"). Directory entries below stand for the files that inventory names inside them.
files_modified:
  - .gitignore
  - .claude/settings.json
  - .codex/hooks.json
  - .planning/STATE.md
  - .planning/ROADMAP.md
  - .planning/phases/999.1-rename-phase-named-identifiers/.gitkeep
  - docs/architecture/repository-layout.md
  - docs/content/production.md
  - docs/workflow/ai-collaboration.md
  - docs/workflow/cleanup.md
  - docs/workflow/foundation-checks.md
  - docs/workflow/lan-foundation-checks.md
  - docs/workflow/setup.md
  - docs/workflow/verification.md
  - src/godot-game/NuGet.config
  - src/godot-game/tools/README.md
  - src/godot-game/tools/workspace-paths.cjs
  - src/godot-game/tools/workspace-paths.test.cjs
  - src/godot-game/tools/workspace-cleanup.cjs
  - src/godot-game/tools/workspace-cleanup.test.cjs
  - src/godot-game/tools/workspace-migrate.cjs
  - src/godot-game/tools/workspace-migrate.test.cjs
  - src/godot-game/tools/node-setup/
  - src/godot-game/tools/verification/
  - src/godot-game/tools/gameplay-checks/
  - src/godot-game/tools/lan-checks/
  - src/godot-game/tools/asset-pipeline/
  - src/godot-game/game/content/packs/delivery.json
  - src/godot-game/game/content/pack-index.json
  - src/godot-game/tests/verification/
autonomous: true
requirements: [QUICK-261123-LTJ]

estimate:
  tokens: 750000
  raw_tokens: 750000
  tasks: 3
  confidence: low

must_haves:
  truths:
    - "Live code, the ignore rules and live docs use one ignored root `.local/` with the subfolders tools, downloads, nuget, locks, runs/<tool-purpose> and temp. The only remaining `.phaseN` names are the documented legacy entries, captured recipe bytes, logical identities compared with pinned data, and historical statements; the residue gate lists each one (CONTEXT: Folder name and layout)."
    - "tools, downloads, nuget and the shared locks resolve to the main checkout's `.local/` from the task worktree, from the main checkout and from an unrelated working directory, through `git rev-parse --path-format=absolute --git-common-dir`; `KZ_SHARED_LOCAL` overrides it; runs, temp, the verified feed and the check lock stay per checkout (CONTEXT: Sharing)."
    - "From the task worktree, the strict locked build has 0 warnings and 0 errors with no network GET, and the documentation group passes 36/36, using the shared NuGet packages folder and a shared NUGET_SCRATCH (CONTEXT: Verification)."
    - "The ac-08, asset-cascade and gate checks are tracked tools with tests. They reproduced the unchanged base bytes before any hashed file changed. Every gate was shown to FAIL on planted and real-tree inputs before any of its PASS results counted as evidence. At the final commit every binding is current: 423 module bindings, delivery, pack index, every ac-08 row, the 6 caller hashes and 9 staged plans. The four runners still list 686, 16, 32 and 75 cases, and the docs group count equals the base count (CONTEXT: Extras, Specifics: No weakening)."
    - "Node 24.22.1 is pinned by SHA-256 in toolchain.json, installed once into the shared `.local/tools/node/` by the tracked setup script, and put on PATH by a Claude Code SessionStart hook; Codex gets an advisory SessionStart line. No persistent PATH change is made (CONTEXT: Node 24, Node on PATH, Decisions after research)."
    - "The cleanup command keeps tools, downloads and nuget and deletes runs, temp and other scratch. The migration command is dry-run first, lists the legacy VM deletion and its size first, refuses while the main checkout's branch still uses `.phase1`, and was run only as a dry run (CONTEXT: Folder name and layout, Decisions after research)."
    - "Docs carry the worktree location rule in ai-collaboration.md, cleanup-only cleanup.md, `core.longpaths` in setup.md, and `.phaseN -> .local/...` rows in the layout map. The PR #11 contradictions are fixed, backlog 999.1 records the identifier rename, history and pinned bytes are unchanged, and no committed line adds a machine-specific path (CONTEXT: Extras, Phase 5 planning changes, Phase-named identifiers)."
  artifacts:
    - path: src/godot-game/tools/workspace-paths.cjs
      provides: "The one JS resolver for shared and local `.local` roots, the isolation environment and the legacy `.phaseN` map"
    - path: src/godot-game/tests/verification/WorkspacePaths.cs
      provides: "The C# twin used by the verification tests, including the narrow shared-package exception"
    - path: src/godot-game/tools/verification/ac08-bindings.cjs
      provides: "Tracked ac-08 regenerator and checker"
    - path: src/godot-game/tools/verification/asset-bindings.cjs
      provides: "Tracked asset catalog, delivery and pack-index regenerator and checker"
    - path: src/godot-game/tools/verification/rebind-gates.cjs
      provides: "Tracked gates: history, newhost, residue, lists, tests, roots, build, assets, ac08, docs; each has a fail-closed collector and an exported pure judge"
    - path: src/godot-game/tools/verification/bindings.test.cjs
      provides: "Tool reproduction, tool negative controls, and failing-direction controls for every gate (in memory and on a temporary repository)"
    - path: src/godot-game/tools/node-setup/setup.ps1
      provides: "Pinned Node download, verification and shared install on Windows"
    - path: src/godot-game/tools/node-setup/session-path.sh
      provides: "SessionStart PATH hook for Claude Code and the Codex advisory line"
    - path: src/godot-game/tools/workspace-migrate.cjs
      provides: "Dry-run-first legacy `.phase1` migration for the main checkout"
    - path: .claude/settings.json
      provides: "Project SessionStart hook, separate from the GSD-owned settings.local.json"
  key_links:
    - from: src/godot-game/tools/verification/check.cjs
      to: src/godot-game/tools/workspace-paths.cjs
      via: "environment(root) uses isolationEnv(), which exports KZ_SHARED_LOCAL and NUGET_SCRATCH to every child"
    - from: src/godot-game/tests/verification/WorkspacePaths.cs
      to: "KZ_SHARED_LOCAL set by the runners"
      via: "Shared resolves the override first, then the git common dir, then the checkout's own .local"
    - from: src/godot-game/NuGet.config
      to: src/godot-game/tools/verification/toolchain.json
      via: "feeds value ../../.local/nuget/feed must match exactly (bootstrap PINS_MISMATCH)"
    - from: src/godot-game/tools/asset-pipeline/required-cases.json
      to: src/godot-game/game/content/pack-index.json
      via: "moduleSha256 rows, then delivery fixtureCatalog, then the pack-index row tools/phase4/required-cases.json"
    - from: src/godot-game/tools/verification/required-cases.json
      to: src/godot-game/tools/gameplay-checks/required-cases.json
      via: "ac-08 repositoryStages hash = callerStages.repositorySha256 (5 cases plus core.permissions)"
    - from: .claude/settings.json
      to: src/godot-game/tools/node-setup/session-path.sh
      via: "SessionStart command appends the PATH line to CLAUDE_ENV_FILE, reading the version from toolchain.json"
    - from: src/godot-game/tools/workspace-migrate.cjs
      to: src/godot-game/tools/workspace-cleanup.cjs
      via: "reuses the exported keepReason, isIgnored, hasTrackedFiles and isRealDirectory safety helpers"
---

<objective>
Replace the per-phase ignored folders with one ignored `.local/` root. Share the toolchain, downloads and NuGet caches once per repository from the main checkout's `.local/`, keeping runs and temp per checkout. Pin and install Node 24 in the shared tools, with session PATH hooks. Track the rebinding tools, rebind every hash the edit breaks, update the docs, fix the PR #11 contradictions and record the identifier rename as backlog.

Purpose: one regenerable, specifically named workspace folder that every worktree reuses, without weakening any validator or rewriting history. All decisions are locked in `261123-ltj-CONTEXT.md`, including "Decisions after research".
Output: linear commits on `feature/261123-ltj-local-folder` in the task worktree (not pushed), plus an uncommitted `261123-ltj-SUMMARY.md`.
</objective>

<execution_context>
@.claude/gsd-core/workflows/execute-plan.md
@.claude/gsd-core/templates/summary.md
</execution_context>

<context>
@.planning/STATE.md
@AGENTS.md
@.planning/quick/261123-ltj-replace-phasen-folders-with-one-ignored-/261123-ltj-CONTEXT.md
@.planning/quick/261123-ltj-replace-phasen-folders-with-one-ignored-/261123-ltj-PRE-RESEARCH.md
@.planning/quick/261123-ltj-replace-phasen-folders-with-one-ignored-/261123-ltj-RESEARCH.md
@docs/architecture/repository-layout.md
@docs/workflow/cleanup.md
@docs/code/README.md

Reference only, never executed: `.planning/quick/261123-fsu-resolve-conflict-https-github-com-sebrei/261123-fsu-PLAN.md`. Its embedded gate script (`acRows`, `blobs`, `tree`, `hostLines`, the `bindings` gate) is the source the tracked tools are rebuilt from. Its `merge`, `moves`, `state` and `records` gates, the `MAP`/`RETIRED` tables and its `DEV`-relative comparisons are not carried over (RESEARCH section 4).
</context>

## Locations and shell rules

- **Task worktree**: the only checkout you edit, build and commit in. **Main checkout**: the primary working tree, read-only apart from its shared `.local/` subfolders. **Shared root (S)**: the main checkout's `.local/`. **Local root (L)**: a checkout's own `.local/`; in the main checkout S and L are the same folder. Your prompt gives the real paths. Never write a real path into any file, commit message or record.
- Every shell call is Git Bash, starts with `cd` to the task worktree root, and runs `export MSYS_NO_PATHCONV=1`. Prepend the Node folder your prompt gives to PATH. `node --version` must print `v24.22.1`, because the runners reject any other version. When a command needs the main checkout, derive it: `MAIN=$(dirname "$(git rev-parse --path-format=absolute --git-common-dir)")`.
- The main checkout's tracked files, its branch and its `.phase1/` contents are read-only. Copy from them and never move or delete them. Inside the main checkout you may write only to `.local/tools`, `.local/downloads`, `.local/nuget` and `.local/locks`. The common `info/exclude` already ignores `/.local/` in every checkout.
- Scratch, logs and commit-message files go in `.local/temp/quick-261123-ltj/` of the task worktree. Each task appends its results (command, revision, exit status, counts) and its residue classifications to `.local/temp/quick-261123-ltj/results.md`. Task 3 builds the SUMMARY from that file, so a fresh executor finds the earlier proofs there. Stage with explicit pathspecs only, never `git add -A`, `git add .` or `git commit -a`. Never stage or commit anything in `.planning/quick/261123-ltj-replace-phasen-folders-with-one-ignored-/`. Do not add the 261123-ltj row to STATE "Quick Tasks Completed".
- Write repository files with the Edit or Write tools or with Node (LF, no BOM), never with PowerShell `Set-Content` or `Out-File`. C# follows `docs/code/README.md`: named arguments, XML documentation and 120 columns, as in `ProofProtocol.cs`.
- Network: only the Node archive from nodejs.org (Task 3) and NuGet restore if it ever needs it (the `build` gate fails on any HTTP GET). No other downloads or installs. Never push and never touch GitHub.
- Every commit message ends with `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`. Use `git commit -F .local/temp/quick-261123-ltj/<name>.txt`.
- Do not weaken, skip or broaden a validator, pin, exemption or expected count. Rebind only what the move requires. History records, captured provenance and pinned bytes keep their bytes.
- BASE is `88953aad1e7d7b6e8855e53c3692bdeeffe1ed29`, which is origin/dev.

## Shared contracts (all three tasks use these exactly)

### Layout

| Purpose | Location | Scope | Replaces |
| --- | --- | --- | --- |
| Node 24.22.1 | `S/tools/node/<pin directory>/` | shared | hand-placed per-task Node folders |
| Godot editor | `S/tools/godot/<archive directory>/` | shared | `.phase1/tools/<directory>` |
| Linux VM, recreated by the phase that needs one | `S/tools/linux-vm/` | shared | `.phase1/tools/linux-vm/`, which the migration deletes |
| Godot archive and `SHA512-SUMS.txt` | `S/downloads/godot/` | shared | `.phase1/cache/` |
| Export templates and sums | `S/downloads/export-templates/` | shared | `.phase1/cache/phase4-exports/` |
| Node archives | `S/downloads/node/` | shared | new |
| VM preparation archives | `S/downloads/linux-prep/` | shared | `.phase1/tools/linux-prep/` |
| NuGet global packages, HTTP cache, scratch (`NUGET_SCRATCH`) | `S/nuget/packages/`, `S/nuget/http/`, `S/nuget/scratch/` | shared | `.phase1/packages`, `.phase1/http-cache`; scratch is new and required once packages are shared |
| Verified Godot package feed | `L/nuget/feed/` | per checkout | `.phase1/feed`. `NuGet.config` keeps the relative value `../../.local/nuget/feed`. |
| Toolchain and desktop locks | `S/locks/toolchain.lock`, `S/locks/desktop.lock` | shared | new, and `.phase2/desktop.lock` |
| Check lock | `L/locks/check.lock` | per checkout | `.phase1/check.lock` |
| Runs | `L/runs/verification/`, `L/runs/gameplay-checks/<platform>/` and `completions/`, `L/runs/lan-checks/<platform>/`, `L/runs/asset-pipeline/` and `acceptance/` | per checkout | `.phase1/runs`, `.phase2/runs` and `completions`, `.phase3/runs`, `.phase4/runs` and `acceptance` |
| Temp and scratch | `L/temp/` (TEMP, TMP, TMPDIR), `L/temp/cli-home/` (DOTNET_CLI_HOME), `L/temp/<tool-purpose>/<name>/` for controls, tdd, probes and task scratch | per checkout | `.phase1/temp`, `cli-home`, `dotnet-home`, the `.phase2`–`.phase4` scratch, `.phase1/quick-*` |

The feed and the CLI home sit inside the CONTEXT subfolders `nuget` and `temp`, so the layout keeps exactly the creator's six subfolders. Choosing a per-checkout feed is the CONTEXT discretion "verified feed shared or per checkout". RESEARCH section 6 explains the reasons: relative `NuGet.config`, `bootstrap.cjs` `PINS_MISMATCH`, and `ExemptionValidator.CheckHash` staying inside the root.

### Legacy map

A historical `.phaseN` identity maps to its current physical location; the longest prefix wins. `workspace-paths.cjs` `legacyPath()`, C# `WorkspacePaths.Physical()` (the rows C# reads), the migration and the layout-policy historical map all use this one table:

- `.phase1/cache/phase4-exports/` → `S/downloads/export-templates/`; `.phase1/cache/` → `S/downloads/godot/`
- `.phase1/tools/linux-vm/` → `S/tools/linux-vm/`; `.phase1/tools/linux-prep/` → `S/downloads/linux-prep/`; `.phase1/tools/` → `S/tools/godot/`
- `.phase1/feed/` → `L/nuget/feed/`; `.phase1/packages/` → `S/nuget/packages/`; `.phase1/http-cache/` → `S/nuget/http/`
- `.phase1/cli-home/` and `.phase1/dotnet-home/` → `L/temp/cli-home/`; `.phase1/temp/` → `L/temp/`; `.phase1/runs/` → `L/runs/verification/`; `.phase1/check.lock` → `L/locks/check.lock`; other `.phase1/<x>` → `L/temp/verification/<x>`
- `.phase2/runs/` → `L/runs/gameplay-checks/`; `.phase2/completions/` → `L/runs/gameplay-checks/completions/`; `.phase2/desktop.lock` → `S/locks/desktop.lock`; other `.phase2/<x>` → `L/temp/gameplay-checks/<x>`
- `.phase3/runs/` → `L/runs/lan-checks/`; other `.phase3/<x>` → `L/temp/lan-checks/<x>`
- `.phase4/runs/` → `L/runs/asset-pipeline/`; `.phase4/acceptance/` → `L/runs/asset-pipeline/acceptance/`; other `.phase4/<x>` → `L/temp/asset-pipeline/<x>`. The Phase 4 inputs the creator declared lost resolve here and stay missing.

### `src/godot-game/tools/workspace-paths.cjs` (Task 1 creates it; Tasks 2 and 3 consume it)

- `checkoutRoot()`: the repository root derived from `__dirname`.
- `roots(root = checkoutRoot(), env = process.env)` returns `{checkout, local, shared, source}`. `source` is `override`, `git` or `standalone`. Resolution order:
  1. `KZ_SHARED_LOCAL`, which must be absolute, else `SHARED_ROOT_INVALID`.
  2. Otherwise `git -C <root> rev-parse --path-format=absolute --git-common-dir`; when the common dir's basename is `.git`, its parent plus `/.local`. When git succeeds but the basename is not `.git` (a bare repository or a submodule), fail with `SHARED_ROOT_UNRESOLVED` and say to set `KZ_SHARED_LOCAL`.
  3. When git fails (a non-git copy), `<root>/.local`.
  Memoize per root and override. The module spawns no process and creates no folder at require time.
- `locations(root?, env?)` returns `local`, `shared`, `tools(name)`, `downloads(name)`, `nuget.{packages, http, scratch, feed}`, `locks.{toolchain, desktop, check}`, `runs(tool, ...parts)`, `temp(...parts)` and `cliHome`, following the layout table.
- `isolationEnv(root?, extra?)` returns `process.env` plus DOTNET_CLI_HOME, NUGET_PACKAGES (still honoring `PHASE1_PACKAGE_CACHE` first, as `verification/check.cjs` does today), NUGET_HTTP_CACHE_PATH, NUGET_SCRATCH, TEMP, TMP, TMPDIR and `KZ_SHARED_LOCAL` set to the resolved shared root, so children and source copies agree. Each caller adds its own flags, such as the telemetry opt-outs.
- `legacyPath(identity, root?)` returns the absolute physical path for a `.phaseN/...` identity per the legacy map, or null.
- `insideShared(absolute, root?)` is true only when the realpath-resolved target lies inside the realpath of S. It compares case-insensitively on Windows and rejects `..` escapes and links leading out.
- `ensure(...directories)`.
- CLI: `--print-roots` prints `roots()` as JSON. `--print-env sh|powershell` creates the folders and prints the isolation assignments for that shell. Both accept `--root <dir>`.
- `KZ_SHARED_LOCAL` is the only new environment variable (CONTEXT discretion). Existing `PHASE1_*`, `PHASE2_*` and `PHASE3_*` names stay; renaming them is backlog 999.1.

### C# `src/godot-game/tests/verification/WorkspacePaths.cs` (Task 1)

An internal static class rooted at `ProofProtocol.Root`, mirroring the JS resolution order: `KZ_SHARED_LOCAL`, then the git common dir, then the checkout's `.local`. It provides `Local`, `Shared`, `Temp(purpose)`, `Runs(tool)`, the NuGet packages, http, scratch and feed folders, `Tools(name)` and `Physical(identity)` (legacy-map rows C# reads: feed, packages, temp, runs, tools), plus one way to apply the isolation variables, including NUGET_SCRATCH and KZ_SHARED_LOCAL, to a child process. `IsSharedPackage(fullPath)` is the narrow shared-root exception: true only for a realpath inside `S/nuget/packages`.

### Literal classification rules (every live `.phaseN` hit)

1. A fresh output location (runs, temp, locks, controls, probes, scratch) becomes a helper call.
2. A toolchain, package or feed location becomes a helper call.
3. A value compared with pinned or captured bytes keeps its logical `.phaseN` string. Examples: register `package` and `surface` cells, production JSON, record identities in history, and C# `ValidateOriginCells`. Resolve it through `legacyPath()` or `Physical()` only where a file is opened.
4. Pinned recipes that assert the original checkout's absolute `.phase4` folder keep their bytes. They are in `asset-pipeline/check.cjs` (one line) and `asset-pipeline/cases/audio.cjs` (three lines). Before such a recipe launches, fail fast with the diagnostic `CAPTURED_ROOT_REQUIRED`. The message says the pinned recipe asserts the original checkout's `.phase4` folder and that Phase 5 makes it work in the relocated tree (05-CONTEXT D-46). Derive the asserted folder from the recipe bytes at runtime; never write that absolute path again.
5. Path-safety guards stay strict. These are `PATH_OUTSIDE_WORKSPACE`, `SUPPRESSION_ORIGIN_PATH`, `HOST_AUTHORING_RESOLUTION` and the export file-set filter. Add only the narrow shared-root allowance: exact S, realpath, and only for the named purpose, which is tools for bootstrap and NuGet packages for register reads. Where a guard rejected `.phase1` authoring folders, it now also rejects the `.local` authoring folders `nuget`, `tools` and `downloads`, and it still rejects `.phase1`. It must not reject a run folder under `L/runs`.
6. Exclusion lists that skip the ignored workspace folder add `.local` and keep `.phaseN` as a legacy entry until the migration retires it. These are `DocumentationValidator.IsInventory`, `ProjectEvaluator` discovery, the cleanup build filter and the export file filter.
7. Fixtures and tests emulate the new layout. Every JS test or fixture that could touch shared locks or tools sets `KZ_SHARED_LOCAL` to a temporary folder. No test writes into the real shared root, except NuGet restore into the shared packages folder.
8. Write no new comment that names a legacy `.phaseN` folder outside the files the residue gate allows.

### Live inventory (MUTABLE-SCOPE authority)

At each task start, save both outputs to `.local/temp/quick-261123-ltj/inventory-task<N>.txt`. Only files they list, plus the new files this plan names, may be edited for the move. At planning time on BASE they printed 89 files and 892 lines, and the counts in RESEARCH section 9 still held. That is guidance only.

- `git grep -n -P "(?<![A-Za-z0-9_)\]])\.phase[0-9](?![0-9A-Za-z_.])" -- src docs .gitignore .planning/STATE.md`
- `git grep -n -F -e '.phase\d' -e '.phase[0-9]' -e '.phaseN' -e "'.'+family" -e "'.'+item.family" -- src docs .gitignore`

A `residue` hit outside a task's listed `--allow` set has two possible outcomes. Either rebind it, or classify it as a legacy-map, captured, logical-identity or historical entry under the rules above, add it to that task's `--allow` (with `=N` when the count is fixed) and record the reason in `results.md`. Later tasks carry earlier additions into their own `--allow`.

### Gate invocation

Every `<automated>` check calls `node src/godot-game/tools/verification/rebind-gates.cjs` with `--base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29`. These arguments recur:

- KEEP_FINAL = `docs/code,src/godot-game/production/assets,docs/workflow/platform-proof.md,docs/workflow/phase4-layout-integration.md,src/godot-game/tools/asset-pipeline/extract-media.py,src/godot-game/tools/asset-pipeline/extract-audio.py,specs,.planning/REQUIREMENTS.md`
- PATTERN = `'(?<![A-Za-z0-9_)\]])\.phase[0-9](?![0-9A-Za-z_.])'`

<tasks>

<task type="tracer">
  <name>Task 1: (1a) Track the rebinding tools, proven on unchanged bytes and with controls showing every gate can fail; then, after a resumable seam, (1b) wire one shared .local root through the verification runner, bootstrap, NuGet and the C# tests</name>
  <precondition>The task worktree is on feature/261123-ltj-local-folder at 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29, clean apart from the untracked 261123-ltj quick-task folder; `git check-ignore -q .local/x` exits 0; the main checkout's `.phase1/packages` and `.phase1/feed` exist.</precondition>
  <reversibility rating="reversible">Commits stay local. The NuGet seed in the shared root is a copy; deleting it restores the previous state.</reversibility>
  <files>src/godot-game/tools/verification/ac08-bindings.cjs, src/godot-game/tools/verification/asset-bindings.cjs, src/godot-game/tools/verification/rebind-gates.cjs, src/godot-game/tools/verification/bindings.test.cjs, src/godot-game/tools/workspace-paths.cjs, src/godot-game/tools/workspace-paths.test.cjs, .gitignore, src/godot-game/NuGet.config, src/godot-game/tools/verification/toolchain.json, src/godot-game/tools/verification/check.cjs, src/godot-game/tools/verification/bootstrap.cjs, src/godot-game/tests/verification/WorkspacePaths.cs, src/godot-game/tests/verification/ (the 25 inventoried test files)</files>
  <action>
1. Entry check. Confirm the precondition, `node --version` = v24.22.1 and `dotnet --version` = 10.0.404. Write both inventory outputs (Live inventory). Run the BASE baseline that needs no `.local` setup: `node --test` over every tracked `src/godot-game/tools/**/*.test.cjs`, and each runner's `--list` from the system temp folder (686, 16, 32, 75). If either fails at BASE, stop and report, because the gates could not pass.

2. Write the three rebinding tools and their test as new, still untracked files. Use the fsu gate script as source (Context) and RESEARCH section 4 as the design. Each tool exports pure derivation functions and is a CLI. Tests call the functions in memory or use temporary repositories in the system temp folder, and never write to the tree. Catalog writes use `JSON.stringify(value, null, 2) + '\n'`, LF.
   - `ac08-bindings.cjs`, with `--check [--rev R]` (default) and `--write`. It derives ac-08 `repositoryStages` from R's tree by the fsu `acRows` rule:
     - the path set is every tracked path outside `.claude/`, `.codex/`, `.agents/` and `.planning/` whose extension is not in `SourceFileFormat.cs` `_binaryExtensions`, sorted ordinal;
     - the kind follows the extension;
     - the owners are the projects whose folder holds the file, plus the test project for its linked `Compile` items, in `projects` order;
     - the sha256 is that of the blob at R, or null for outside-validator rows;
     - `sourceSetSha256` = sha256 of `JSON.stringify(sources)`;
     - the `repository-sources-*` stages hold the checked rows in order, in chunks of 16, at the same position among the other stages, which keep their order;
     - `bindings` are the register bytes at R.
     It never derives `keys` or `cohorts`: it asserts they are unchanged and that there are 42 keys. It asserts that `projects` equals the tracked `*.csproj` set under `src/godot-game/`, keeping the committed order. `callerStages.repositorySha256` = sha256 of `JSON.stringify(repositoryStages)`, set in both the verification catalog and `gameplay-checks/required-cases.json`.
     - `--check` compares the derivation with the catalogs at R and names the first differing rows.
     - `--write` derives from HEAD. It refuses with `AC08_DIRTY_TREE` when `git status --porcelain` shows any change other than the two catalogs, and writes those two catalogs in place.
   - `asset-bindings.cjs`, with `--check` (default) and `--write`, working on the working tree in cascade order:
     - every `{module, moduleSha256}` in `asset-pipeline/required-cases.json` (each module under `src/godot-game/tools/asset-pipeline/` and existing; raw-byte sha256);
     - `game/content/packs/delivery.json` `fixtureCatalog`: sha256, uncompressedBytes, and gzipBase64 at gzip level 9 of the catalog bytes;
     - `game/content/pack-index.json` = `JSON.stringify(content.normalizeCurrent(ROOT), null, 2) + '\n'`;
     - finally `content.validate(ROOT).valid.length` = 180 and `cases/text-data.cjs` `validateSupport()` passes.
     Guides and `projectGraphSha256` are checked, not written.
   - `rebind-gates.cjs`: `--base REV`, `--keep a,b`, `--allow path[=N],...`, `--scope a,b`, `--pattern RE` and `--repo DIR`, then gate names. Each gate prints `PASS <gate> <counts>` or exits 1 with `FAIL <gate>: <reason>`.
     - Structure: build every gate as a collector plus an exported pure `judge(facts, options)`, so the tests can drive each verdict in memory.
     - Fail closed: collection never yields PASS by default. These are all `FAIL`:
       - a git exit above 1, including an invalid `--pattern`;
       - a child that cannot start or exits non-zero;
       - a missing base or HEAD file;
       - output the judge cannot parse.
     - Pathspecs: every `--scope` and `--allow` entry must name at least one tracked path at HEAD, and every `--keep` entry at least one at base. Otherwise the gate fails with `unknown pathspec`, so a typo cannot pass vacuously.
     - `--repo` (default: this checkout) points `history`, `newhost` and `residue` at another repository for the controls. Every other gate fails with `--repo unsupported` when it is set.
     - `history`: every tracked path at base under `.planning/phases/`, `.planning/quick/`, `.planning/debug/`, `docs/code/provenance/` or a `--keep` pathspec has the same blob at HEAD. New files are allowed.
     - `newhost`: no HEAD line matches the fsu machine-path pattern, generalized so the user-folder term matches any user name, unless the same text exists at base.
     - `residue`: `git grep -P PATTERN HEAD` inside `--scope` (default: everything), excluding the history roots and `--keep`, finds nothing outside `--allow`. An `--allow path=N` entry must retain exactly N matching lines. The gate always prints the remaining hits per file.
     - `lists`: each of the four runners' `--list`, run from the system temp folder, equals its catalog length and the base catalog length.
     - `tests`: `node --test` over every tracked `src/godot-game/tools/**/*.test.cjs`, exit 0 with `fail 0`; print the pass count. It fails when no test file is found or the pass count is 0.
     - `roots`: `workspace-paths.cjs --print-roots` from the system temp folder resolves shared = realpath of the git common dir's parent plus `/.local` (or `KZ_SHARED_LOCAL` when set) and local = `<checkout>/.local`. With `--root` set to the main checkout, shared = local. From a linked worktree, shared ≠ local.
     - `build`: with `isolationEnv()`, a locked restore of `src/godot-game/KingdomZombies.slnx` (`--locked-mode --disable-parallel -p:NuGetAudit=false -v:n -tl:off`) whose log has no `GET https?:`. Then a strict build (`--no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -tl:off`) with `0 Warning(s)` and `0 Error(s)`. Logs go to `L/temp/rebind-gates/`.
     - `assets`: `asset-bindings --check`, and 423 module bindings, equal to the base count.
     - `ac08`: `ac08-bindings --check --rev HEAD`; the working-tree bytes of every hashed row equal HEAD; `callerAuthority` and every `producerStages.registries` sha256 match the file bytes; `verification/check.cjs` `producerPlan` accepts every staged case of both catalogs, and there are 9.
     - `docs`: the expected count comes from base, never from a file this plan edits.
       - N_base is the number of `cases` entries with `group` `docs` in `git show <base>:src/godot-game/tools/verification/required-cases.json`. N_head is the same count in the working-tree catalog. Both are 36 at planning time.
       - The gate fails before it launches the runner when the base catalog is absent or N_head ≠ N_base.
       - It also fails when `verification/check.cjs --group docs` exits non-zero, or when its output lacks the exact line `PASS N_base/N_base selected obligations`.
       - Print N_base, N_head and the summary path.
   - `bindings.test.cjs`:
     - ac-08 reproduction at the commit that last changed `src/godot-game/tools/verification/required-cases.json` (`git log -1`). At planning time that was `18e77bc`; only Markdown changed between it and BASE.
     - asset `--check` passes on the current tree.
     - The delivery gzip at level 9 reproduces the committed payload, and level 6 is detected.
     - In-memory negative controls, each failing with the named row: a flipped source sha, a phantom path, a dropped checked row, a wrong owner, and one changed module byte.
     - `AC08_DIRTY_TREE` refusal, as a pure function over status lines.
     - Gate controls. Each control asserts `FAIL <gate>` with the named path, row or count, and is paired with an input that passes, so a control cannot succeed because of an unrelated error.
       - In memory, through each gate's `judge`, for all ten gates:
         - `history`: a changed blob under each history root, a changed `--keep` file, a deleted record;
         - `newhost`: a new machine-path line under another user name fails; the same text present at base passes;
         - `residue`: a hit outside `--allow`, an `=N` entry retaining N-1 or N+1 lines, a git exit of 2;
         - `lists`: a runner count different from its catalog, a catalog length different from base, a runner exit of 1;
         - `tests`: `fail 1`; `pass 0` with no test file;
         - `roots`: shared = local from a linked worktree; shared not under the git common dir's parent;
         - `build`: a `GET https://` line, `1 Warning(s)`, `1 Error(s)`, a missing summary line;
         - `assets`: 422 module bindings against base 423, a failed `--check`;
         - `ac08`: 8 staged plans, a registry sha256 that differs from the file bytes, a stale `callerAuthority`;
         - `docs`: `PASS 35/35` against base 36, a head docs count of 35 against base 36, a runner exit of 1, a missing summary line.
       - End to end, through the CLI with `--repo` on a temporary git repository in the system temp folder.
         - Setup: a base commit, then a HEAD commit that:
           - changes a `.planning/phases/` record;
           - changes a `--keep` file;
           - deletes a `.planning/quick/` record;
           - adds a machine-path line;
           - adds legacy-folder hits to an allowed `=N` file and to an unallowed file.
         - Expected: `history`, `newhost` and `residue` each exit 1 and name the planted path. `residue` also exits 1 with `--pattern '('` and with an unknown `--scope` entry.
         - Pass direction: on a clean second HEAD, all three exit 0 with PASS.
       - Build every planted machine-path and legacy-folder string from fragments at runtime, for example `['.', 'phase', '1'].join('')`. The committed test bytes then match neither pattern, and the tools commit passes `newhost` and `residue` on its own bytes.

3. Prove the tools on unchanged bytes, before any hashed file changes. HEAD is still BASE and the tools are still untracked.
   - Tool proofs:
     - `ac08-bindings.cjs --check --rev HEAD` and `--rev 18e77bc` both exit 0;
     - `asset-bindings.cjs --check` exits 0;
     - `node --test src/godot-game/tools/verification/bindings.test.cjs` passes, including every gate control.
   - Forced FAIL runs on the real tree. Use the exact PATTERN quoting the later verifies use. Each run must exit 1 and name the expected rows, and each is paired with the PASS run listed beside it:
     - `history`:
       - `history --base 18e77bc` names the three tracked `.planning/phases/05-ci-and-repeatable-game-testing/` files that PR #11 changed: `05-CONTEXT.md`, `05-PHASE1-HANDOFF.md` and `05-SPEC.md`.
       - Adding `--keep docs/workflow/verification.md` also names that file.
       - Pass direction: `history --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29` passes.
     - `newhost`:
       - `newhost --base 47efd809c5246bc1cf961f17e25c7f67e1e82110` fails with new lines. That is the root commit, which has no matching line.
       - Pass direction: `newhost --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29` passes.
     - `residue`:
       - `residue --scope src/godot-game/tools/verification/check.cjs` with no `--allow` fails with 3 lines.
       - With `--allow src/godot-game/tools/verification/check.cjs=3` it passes; with `=2` or `=4` it fails.
       - Over the Task 1 scope (`.gitignore,src/godot-game/NuGet.config,src/godot-game/tools/verification,src/godot-game/tests`) with no `--allow`, it fails. Its per-file counts equal `git grep -c -P PATTERN HEAD -- <the same paths>`, which gave 29 files at planning time.
     - `lists` and `docs`:
       - `lists --base a305b8b` and `docs --base a305b8b` fail because the base catalogs are absent there. `docs` must fail before it launches the runner.
       - Pass direction for `lists`: `lists --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29` passes with 686/16/32/75.
       - Pass direction for `docs`: first shown at the Task 1b verify, after these controls.
   - Append each command, its exit status and the named rows to `.local/temp/quick-261123-ltj/results.md` under `## Gate controls`.
   - If a forced run exits 0, its gate can pass vacuously. Fix the tool and rerun every step-3 proof before committing.
   - Commit only the 4 tool files: `feat(quick-261123-ltj): track the rebinding tools proven on unchanged bytes`.
   - At that commit, `rebind-gates.cjs --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29 --scope <the 4 tool files> --pattern PATTERN history newhost residue` passes with no `--allow`. That proves the tools' own bytes trip neither gate. If it fails, repair the tools in a fix commit and rerun this check.

**Resumable seam: Task 1a (steps 1–3) ends here, and Task 1b (step 4) starts.** Task 1a's exit commit is the tools commit, plus any tool fix commit. A fresh executor may take over at step 4.

Task 1b entry check. Run it in full before step 4. If any check fails, stop and report instead of starting step 4.
   - Branch history:
     - `git log --format=%s 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29..HEAD` lists only the tools commit and any tool fix commit.
     - `git diff --name-only 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29 HEAD` lists exactly the 4 tool files.
     - The tree is clean apart from the quick-task folder.
   - Tests and reproduction proofs:
     - `node --test src/godot-game/tools/verification/bindings.test.cjs` passes.
     - `asset-bindings.cjs --check` exits 0.
     - `ac08-bindings.cjs --check --rev 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29` and `--rev 18e77bc` both exit 0. The first is the revision that was HEAD during step 3.
     - `ac08-bindings.cjs --check --rev HEAD` exits 1, because the 4 new tracked tool files are not yet catalog rows. Record it as the real-tree ac08 failing-direction run. It stays stale until Task 3 step 5.
   - Gates: `rebind-gates.cjs --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29 history newhost lists tests` passes.
   - Records:
     - `results.md` holds the step-3 `## Gate controls` rows, and every forced run there exited 1.
     - The Task 1 inventory file exists. If it is missing, rerun the Live inventory into `inventory-task1.txt`.

4. Tracer slice (CONTEXT: Folder name and layout, Sharing, What stays where it is):
   - Add `workspace-paths.cjs` per the contract. Its `workspace-paths.test.cjs` uses temporary git repositories in the system temp folder (`git -c user.name=t -c user.email=t@example.invalid`, then `git worktree add`) and covers:
     - main checkout: shared = local;
     - linked worktree: shared = main `.local`, local = worktree `.local`;
     - an unrelated working directory;
     - the `KZ_SHARED_LOCAL` override, and its rejection when relative;
     - a non-git copy, giving standalone;
     - a non-git folder nested under a checkout's `.local/temp`, giving local = the folder's own `.local` and shared = the enclosing repository's shared root;
     - a bare common dir, giving `SHARED_ROOT_UNRESOLVED`;
     - every legacy-map row;
     - `insideShared` accepting a file inside S and rejecting `..`, a sibling prefix and an outside path;
     - no folder or process at require time.
   - `.gitignore`: replace the workspace rule with `/.local/` under a comment naming the regenerable workspace root. Keep `/.phase[0-9]*/` on its own line under a comment saying it covers legacy folders until `workspace-migrate.cjs` retires them. Keeping it prevents 9 GB of legacy folders in the main checkout from becoming untracked-and-addable after it switches to dev, and lets the migration require its sources to be ignored. Report this justification for "replaces" in the SUMMARY.
   - `NuGet.config` and `toolchain.json` `feeds`: `../../.local/nuget/feed`, changed together because bootstrap requires the match.
   - `verification/check.cjs`:
     - `environment(root)` = `isolationEnv(root)` plus its existing flags and `PHASE1_*` variables;
     - the lock is `locks.check`, with the `CHECK_LOCKED` message naming `.local/locks/check.lock`;
     - runs go to `runs('verification', nonce)`;
     - `main()` creates the local folders.
   - `verification/bootstrap.cjs`:
     - `home` and every location come from the helper;
     - Godot archive and checksums go to `downloads('godot')`, extraction to `tools('godot')`, and package copies to `nuget.feed`;
     - acquisition runs under an exclusive `locks.toolchain` (open with `wx`, refuse with `TOOLCHAIN_LOCKED`, remove in `finally`);
     - `resolveLocal` keeps `PATH_OUTSIDE_WORKSPACE` for everything except an explicit `resolveShared` that allows only `insideShared` targets;
     - the run-local `NuGet.config` rewrite replaces `../../.local/nuget/feed` and fails closed when absent.
     Do not run bootstrap acquisition, because it would download Godot.
   - C#: add `WorkspacePaths.cs` per the contract and rebind every inventoried literal in `src/godot-game/tests/verification/` by the classification rules:
     - `.phase1/temp/<purpose>` uses `Temp`;
     - runner fixtures and assertions use `.local/runs/verification` and `.local/locks/check.lock` together with the runner;
     - `EngineRun`, `BuildFixture` and `GraphFixture` use the helper's Godot and isolation locations, including NUGET_SCRATCH and KZ_SHARED_LOCAL;
     - register `package` and `surface` reads (`EngineGeneratedTests`, `GeneratedLayoutAdapterTests`, `GeneratedSyntaxAdapterTests`, `GeneratedExportLayoutTests`, `ExemptionValidator` line about the engine package) go through `Physical`;
     - `ExemptionValidator.CheckHash` keeps rejecting rooted and `..` paths, except that a `.phase1/packages/` identity whose physical path satisfies `IsSharedPackage` is allowed;
     - `ValidateOriginCells` keeps its logical `.phase1/feed/...` cell;
     - `DocumentationValidator.IsInventory` and `ProjectEvaluator` discovery add `.local` and keep `.phase1` (rule 6).
     Never add a test method or Case trait, because case counts are pinned.
   - Seed, copy only, per RESEARCH section 6 with the new layout:
     - `mkdir -p "$MAIN/.local/nuget/packages" "$MAIN/.local/nuget/http" "$MAIN/.local/nuget/scratch" .local/nuget/feed .local/temp`;
     - `cp -R -n "$MAIN/.phase1/packages/." "$MAIN/.local/nuget/packages/"`;
     - copy each `$MAIN/.phase1/feed/*.nupkg` into `.local/nuget/feed/` and `cmp` it, giving 4 files.
   - Commit: `feat(quick-261123-ltj): resolve one shared .local root for verification and the C# tests`. This is Task 1b's exit commit.
   - Then run the verify. It must run after the commit, because `history`, `newhost` and `residue` read HEAD. Repair failures with fix commits and rerun the verify.
   - A tools test outside this scope may fail because it reads the verification runner's run or lock location, for example a lan-checks inherited-family control. In that case, rebind that coupling here as well (it is in the inventory) and add its folder to `--scope`.
  </action>
  <verify>
    <automated>node --test src/godot-game/tools/workspace-paths.test.cjs src/godot-game/tools/verification/bindings.test.cjs && node src/godot-game/tools/verification/rebind-gates.cjs --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29 --keep docs/code,src/godot-game/production/assets --scope .gitignore,src/godot-game/NuGet.config,src/godot-game/tools/verification,src/godot-game/tests --allow src/godot-game/tests/verification/WorkspacePaths.cs,src/godot-game/tests/verification/ExemptionValidator.cs,src/godot-game/tests/verification/DocumentationValidator.cs,src/godot-game/tests/verification/ProjectEvaluator.cs --pattern '(?<![A-Za-z0-9_)\]])\.phase[0-9](?![0-9A-Za-z_.])' history newhost residue lists tests roots assets build docs</automated>
  </verify>
  <done>
    - Task 1a: the tools commit comes first, plus any tool fix commit.
      - Before it, the tools reproduced BASE: ac-08 at HEAD and at 18e77bc, the asset cascade, and the bindings tests.
      - Every gate control passed, and every forced real-tree run exited 1 with its expected rows, recorded in `results.md` under `## Gate controls`.
      - At that commit, the tools pass `history`, `newhost` and `residue` on their own bytes.
    - Task 1b: its entry check passed, then came the tracer commit and any fix commits.
    - The shared root resolves from the task worktree, from the main checkout (`--root`) and from the system temp folder.
    - The strict locked build is 0/0 with no network GET, and the docs group passes 36/36 from the task worktree using `S/nuget/packages` and `S/nuget/scratch`.
    - Lists are 686/16/32/75, tests pass, history and newhost pass, and no unallowed `.phaseN` literal remains in the Task 1 scope.
    - `ac08` is expected stale until Task 3 and is not run here.
  </done>
</task>

<task type="auto" tdd="true">
  <name>Task 2: Move gameplay, LAN and asset-pipeline outputs to .local, regenerate the asset cascade, and rebuild cleanup plus the dry-run-first migration</name>
  <precondition>Task 1's two commits are on the branch, the tree is clean apart from the quick-task folder, and the seeded `S/nuget/packages` and `L/nuget/feed` exist.</precondition>
  <reversibility rating="reversible">Local commits only. The migration is never applied; its dry run on the main checkout is read-only.</reversibility>
  <files>src/godot-game/tools/gameplay-checks/, src/godot-game/tools/lan-checks/, src/godot-game/tools/asset-pipeline/ (inventoried modules and required-cases.json), src/godot-game/game/content/packs/delivery.json, src/godot-game/game/content/pack-index.json, src/godot-game/tools/workspace-cleanup.cjs, src/godot-game/tools/workspace-cleanup.test.cjs, src/godot-game/tools/workspace-migrate.cjs, src/godot-game/tools/workspace-migrate.test.cjs, src/godot-game/tools/verification/linux-vm/start-vm.cjs</files>
  <behavior>
    - Cleanup in a checkout: `.local/tools`, `.local/downloads` and `.local/nuget` are kept. `.local/runs`, `.local/temp` and any other `.local` entry are deleted. `--apply` deletes nothing while `L/locks/*` or `S/locks/*` holds a file, including the shared desktop and toolchain locks. Legacy `.phaseN` folders keep today's rule: `.phase1` durable names are kept, with the reason "legacy toolchain cache; run workspace-migrate.cjs", and the rest is deleted. The legacy `.phase1/check.lock` and `.phase2/desktop.lock` still block. The build filter skips `.local` and legacy folders. Worktree removal never follows into S.
    - Migration dry run (default): no write anywhere. It prints one row per legacy item as `<action> <legacy path> -> <target> (<size>)`. The first row is `delete .phase1/tools/linux-vm (<size>)`, where `<size>` starts with a digit, such as `8.1 GB`. Refusal lines are prefixed `refuse:`. It exits 0 only when nothing would be refused.
    - Migration refuses when:
      - the main checkout's HEAD lacks `src/godot-game/tools/workspace-paths.cjs`;
      - `.local/` is not ignored there by `.gitignore`;
      - `KZ_SHARED_LOCAL` names anything other than the main checkout's `.local`;
      - any legacy or new lock file exists;
      - a live process has an executable or command line under the legacy folder, or a PID in `linux-vm/processes.json` is live;
      - a source is a link, holds tracked files, is not ignored or contains a repository;
      - a non-cache target already exists without verifying against its pin.
    - `--apply` renames each row; a rerun reports already-migrated rows and changes nothing.
  </behavior>
  <action>
1. Entry check: the precondition, then `rebind-gates.cjs --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29 lists tests assets newhost` passes. Rerun the Live inventory into `inventory-task2.txt`. Only the files it lists under `gameplay-checks`, `lan-checks`, `asset-pipeline` and `workspace-cleanup*` are in this task's rebind scope.
2. Rebind every listed literal by the classification rules, through `workspace-paths.cjs`:
   - gameplay-checks:
     - the isolation env comes from `isolationEnv()`;
     - runs go to `runs('gameplay-checks', platform, nonce)` and completions to `runs('gameplay-checks', 'completions')`;
     - the desktop lock is `locks.desktop`, which is shared (CONTEXT: Sharing; RESEARCH: machine-wide desktop);
     - the runner fixture follows.
   - lan-checks:
     - `check.cjs` isolation and runs;
     - the `network.cjs` Godot path comes from `tools('godot')`;
     - `runner-fixture.cjs` uses the shared desktop lock;
     - the test fake roots (`inherited`, `revisions`, `timing`, `foreground`, `rendered-stream` tests and `stopped-fixture`) emulate the new layout and set `KZ_SHARED_LOCAL` to a temporary folder (rule 7). Where a test builds family run folders, map each family through the legacy map: phase1 to `runs/verification`, phase2 to `runs/gameplay-checks`, phase3 to `runs/lan-checks`.
   - asset-pipeline, in all inventoried modules:
     - `inside(ROOT, '.phase1/cache/' + pin.file)` reads use `downloads('godot')`;
     - engine paths use `tools('godot')`;
     - feed and packages use `nuget.feed`, `nuget.packages` and the export-run `NUGET_PACKAGES`;
     - `export.cjs` `feedConfig` replaces `value="../../.local/nuget/feed"` with the absolute local feed and fails closed with `NUGET_FEED_REWRITE`;
     - the export file-set filter and `hostTrace` `HOST_AUTHORING_RESOLUTION` follow rule 5: also reject `.local/nuget`, `.local/tools` and `.local/downloads`, never `.local/runs`;
     - runs, acceptance, tdd, audio and scratch folders use `runs('asset-pipeline', ...)` or `temp('asset-pipeline', ...)`;
     - `scenarios.cjs` historical families (the computed `'.'+family` paths) map through the legacy map;
     - `scenarios.cjs` and `export.cjs` source copies use the copy's own `.local/nuget/feed`, passing `KZ_SHARED_LOCAL` down;
     - captured recipes follow rule 4, with `CAPTURED_ROOT_REQUIRED` naming D-46 before launch;
     - logical identities compared with `production/assets/*.json` or history keep their strings and resolve through `legacyPath()`.
     Keep `extract-media.py` and `extract-audio.py` byte-identical. Before each commit, confirm the captured recipe strings still hash to their pins.
3. Regenerate the asset cascade: `node src/godot-game/tools/verification/asset-bindings.cjs --write`, then `--check`. Commit the runner edits and regenerated files: `feat(quick-261123-ltj): move gameplay, LAN and asset-pipeline outputs to .local`. `assets` must pass at this commit.
4. Cleanup (CONTEXT: "The cleanup command keeps tools, downloads and nuget..."): rewrite `planArtifacts`, the lock list, the build filter and USAGE in `workspace-cleanup.cjs` per the behavior block. Export `keepReason`, `isIgnored`, `hasTrackedFiles` and `isRealDirectory` beside the current exports. Extend `workspace-cleanup.test.cjs` with the `.local` keep/delete rows, both lock scopes and the legacy rows, keeping every existing test.
5. Migration (CONTEXT "Decisions after research", RESEARCH section 5 without the VM rebase): write `workspace-migrate.cjs`. It operates on the main checkout derived from the git common dir, whatever the current working directory, and resolves targets through `workspace-paths.cjs` and the legacy map. Rows:
   - `.phase1/tools/linux-vm`: delete, listed first with its size. The key it contains is never copied anywhere.
   - Godot zip and `SHA512-SUMS.txt`, export templates, and the Godot editor: move, verified against the `toolchain.json` and `production/assets/export-toolchain.json` pins after the move.
   - `linux-prep`: move to `downloads/linux-prep`.
   - feed: move to `nuget/feed`, verified against the 4 pins.
   - packages and http-cache: move. When the shared target already exists (this task seeds it), delete the legacy copy as a duplicate regenerable cache, listed with its size.
   - `cli-home`, the stray `cache/game.packages.lock.json` and empty `dotnet-home`: delete.
   - Empty `.phase1`–`.phase4` folders: remove.
   Use one same-volume rename per row and stop at the first failure. Inject the process lister and the clock so tests can stub them: on Windows the lister reads `Win32_Process` executable paths and command lines through PowerShell CIM; on Linux it reads `/proc/*/exe` and `cmdline`. Write `workspace-migrate.test.cjs` with temporary git repositories and fake legacy trees, covering every behavior row and each refusal; its fake `toolchain.json` carries hashes of the fake files.
6. Launcher (CONTEXT "Durable data" and "Decisions after research"): the main checkout's untracked `.phase1/tools/linux-vm/start-vm.cjs` is generic. It reads the public key at runtime and carries no secret or machine path. Copy it, read-only from the source, to `src/godot-game/tools/verification/linux-vm/start-vm.cjs`. Change only its root to `locations().tools('linux-vm')` and its require path; keep every other byte. Never copy any other file from that folder. Check it with `node --check` only; never run it.
7. Commit: `feat(quick-261123-ltj): keep shared caches in cleanup and add the legacy migration`.
8. Run the verify. The dry run on the main checkout must refuse, because its HEAD lacks the helper, and must not write. Prove the no-write: compare `git --no-optional-locks -C "$MAIN" status --porcelain` and a name/size/mtime listing of `$MAIN/.phase1` and `$MAIN/.local`, taken before and after into `.local/temp/quick-261123-ltj/`. Never pass `--apply`.
  </action>
  <verify>
    <automated>node src/godot-game/tools/verification/rebind-gates.cjs --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29 --keep docs/code,src/godot-game/production/assets,src/godot-game/tools/asset-pipeline/extract-media.py,src/godot-game/tools/asset-pipeline/extract-audio.py --scope src/godot-game/tools/gameplay-checks,src/godot-game/tools/lan-checks,src/godot-game/tools/asset-pipeline,src/godot-game/tools/workspace-cleanup.cjs,src/godot-game/tools/workspace-cleanup.test.cjs,src/godot-game/tools/workspace-migrate.cjs,src/godot-game/tools/workspace-migrate.test.cjs,src/godot-game/tools/verification/linux-vm --allow src/godot-game/tools/asset-pipeline/check.cjs=1,src/godot-game/tools/asset-pipeline/cases/audio.cjs=3,src/godot-game/tools/workspace-cleanup.cjs,src/godot-game/tools/workspace-cleanup.test.cjs,src/godot-game/tools/workspace-migrate.cjs,src/godot-game/tools/workspace-migrate.test.cjs --pattern '(?<![A-Za-z0-9_)\]])\.phase[0-9](?![0-9A-Za-z_.])' history newhost residue lists tests assets && { node src/godot-game/tools/workspace-migrate.cjs > .local/temp/quick-261123-ltj/migrate-dry-run.txt 2>&1; test $? -eq 1; } && grep -m1 -E '^(delete|move|skip) ' .local/temp/quick-261123-ltj/migrate-dry-run.txt | grep -qE '^delete \.phase1/tools/linux-vm \([0-9]' && grep -q '^refuse:.*workspace-paths' .local/temp/quick-261123-ltj/migrate-dry-run.txt</automated>
  </verify>
  <done>
    - Two or three linear commits on top of Task 1. The asset cascade (423 modules, delivery, pack index, 180 valid) is current at HEAD. Lists are 686/16/32/75 and every tools test passes, including the cleanup and migration tests.
    - The Task 2 scope keeps only the allowed captured-recipe lines (1 plus 3) and the legacy rules in the cleanup and migration files.
    - The main-checkout dry run listed the VM deletion first with its size, refused because the main checkout's branch still uses `.phase1`, exited 1 and changed nothing.
  </done>
</task>

<task type="auto" tdd="true">
  <name>Task 3: Pin and install Node 24 with session hooks, update every live doc and STATE/ROADMAP, regenerate ac-08 last, run every gate and write the SUMMARY</name>
  <precondition>Tasks 1 and 2 commits are on the branch, the tree is clean apart from the quick-task folder, and nodejs.org is reachable for the one approved download.</precondition>
  <reversibility rating="reversible">The shared Node install is a new folder in the shared root that can be deleted; commits stay local.</reversibility>
  <files>src/godot-game/tools/verification/toolchain.json, src/godot-game/tools/node-setup/setup.ps1, src/godot-game/tools/node-setup/setup.sh, src/godot-game/tools/node-setup/session-path.sh, src/godot-game/tools/node-setup/session-path.ps1, src/godot-game/tools/node-setup/session-path.test.cjs, .claude/settings.json, .codex/hooks.json, docs/workflow/setup.md, docs/workflow/cleanup.md, docs/workflow/ai-collaboration.md, docs/workflow/verification.md, docs/workflow/foundation-checks.md, docs/workflow/lan-foundation-checks.md, docs/content/production.md, docs/architecture/repository-layout.md, src/godot-game/tools/README.md, .planning/STATE.md, .planning/ROADMAP.md, .planning/phases/999.1-rename-phase-named-identifiers/.gitkeep, src/godot-game/tools/verification/required-cases.json, src/godot-game/tools/gameplay-checks/required-cases.json, .planning/quick/261123-ltj-replace-phasen-folders-with-one-ignored-/261123-ltj-SUMMARY.md (written, never committed)</files>
  <behavior>
    - `session-path.sh`:
      - with no shared Node it is a silent no-op;
      - with a fake shared Node it appends exactly one `export PATH='<dir>':"$PATH"` line to `CLAUDE_ENV_FILE`, and two runs still leave one line;
      - it works from an unrelated working directory, with a backslash `CLAUDE_PROJECT_DIR` and with a folder name containing spaces;
      - `KZ_SHARED_LOCAL` overrides the shared root;
      - `--codex` prints the line instead of writing;
      - every failure exits 0.
    - `session-path.ps1` (Windows only) prints the PowerShell line when the shared Node exists and nothing otherwise.
    - The setup scripts:
      - verify the archive SHA-256 against `toolchain.json` before extraction;
      - download to `.part` with a nonce and rename;
      - extract into a temporary sibling and rename it into `S/tools/node/<directory>`;
      - hold `S/locks/toolchain.lock`;
      - are idempotent, so a second run downloads nothing;
      - print exactly one PATH line for their shell on stdout.
  </behavior>
  <action>
1. Entry check: the precondition, then `rebind-gates.cjs --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29 lists tests assets` passes. Rerun the Live inventory into `inventory-task3.txt`.

2. Node (CONTEXT: Node 24, Node on PATH, Decisions after research):
   - In `toolchain.json`, add `nodeArchives` after `archives`. It has one-line entries for `win32-x64` and `linux-x64`. Each entry has `file`, `url` (`provenance.nodeSource` + file), `sha256`, `directory` and `executable`:
     - win-x64 `node-v24.22.1-win-x64.zip` = `d2046d28c2c3dcd211ac5145c0bf30e2047ef9dfb306693c8c6e2eccd8b95fa5`, `node.exe`;
     - linux-x64 `node-v24.22.1-linux-x64.tar.xz` = `6c172a9ab6718f7b801fb5a7bd486af2ee5e32758086a04b84fcce2aa77095e6`, `bin/node`.
     Keep the top-level `"node": "24.22.1"` as the first `"node":` key, because the hook reads it with `sed`.
   - Write `src/godot-game/tools/node-setup/`:
     - `setup.ps1` for Windows PowerShell 5.1, with TLS 1.2, `Invoke-WebRequest -UseBasicParsing` and `System.IO.Compression.ZipFile`.
     - `setup.sh`. On Linux it uses curl or wget, `sha256sum` and `tar -xJf`, and reads the pin from the single toolchain line containing the pinned file name, failing closed unless exactly one 64-hex value parses. On Git Bash it verifies an existing install itself, or delegates installation to `setup.ps1` through `powershell.exe -NoProfile -ExecutionPolicy Bypass -File`, then prints the sh line.
     - Both setup scripts take `--archive <path>` (seed from a verified local archive) and `--check` (verify only). `setup.sh` also takes `--platform linux-x64`, which verifies and unpacks without running the binary, for VM preparation.
     - `session-path.sh` and `session-path.ps1` exactly as RESEARCH sections 1–2, with the install directory `S/tools/node/<pin directory>`.
     - `session-path.test.cjs`, covering the behavior block with temporary `KZ_SHARED_LOCAL` roots and asserting that each pin `directory` equals `node-v<node>-<platform>`.
   - Add `.claude/settings.json` exactly as RESEARCH section 1, tracked and separate from the GSD-owned `settings.local.json`, which stays untouched. Append the second SessionStart group from RESEARCH section 2 to `.codex/hooks.json`, leaving the GSD group byte-identical.
   - Run the setup once: `powershell -NoProfile -ExecutionPolicy Bypass -File src/godot-game/tools/node-setup/setup.ps1`. This downloads from nodejs.org (approved) into `S/downloads/node/` and installs into `S/tools/node/node-v24.22.1-win-x64/`. Rerun it to prove idempotence. Confirm `sh src/godot-game/tools/node-setup/setup.sh --check` and that the archive sha256 equals the pin. Run `session-path.sh` against the real shared root with a scratch `CLAUDE_ENV_FILE`, source the file, and check that `node --version` is v24.22.1.
   - If Git Bash has `xz`, verify the Linux branch with `KZ_SHARED_LOCAL` set to a system-temp scratch folder and `--platform linux-x64 --archive "$MAIN/.phase1/tools/linux-prep/node-v24.22.1-linux-x64.tar.xz"`. This confirms RESEARCH assumption A1, the `bin/node` layout. Otherwise record it as not run.
   - From here on you may run the remaining steps with the shared Node, via `eval "$(sh src/godot-game/tools/node-setup/setup.sh)"`.
   - Commit: `feat(quick-261123-ltj): pin Node 24 and put the shared Node on agent session PATH`.

3. Docs. Derive every statement from the committed code (`git diff 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29..HEAD`). Classify each inventoried doc line: instructions and current behaviour are rewritten; past-tense historical facts keep their bytes and are interpreted through the map.
   - `docs/architecture/repository-layout.md`:
     - rewrite the generated-state paragraph (C6) for `.local/`, shared versus per-checkout and `KZ_SHARED_LOCAL`;
     - add the legacy-map rows to the historical table;
     - name `workspace-paths.cjs` as the one resolver.
   - `docs/workflow/setup.md`:
     - Node setup, its PATH line, the Claude hook (a real PATH change for Bash tool calls) and the Codex hook (advisory context, one-time `/hooks` trust, or start `codex` from a prepared shell);
     - isolation via `workspace-paths.cjs --print-env sh|powershell`;
     - the bootstrap locations;
     - `KZ_SHARED_LOCAL` for CI, extra clones and bare repositories;
     - `git config core.longpaths true` in this repository;
     - the VM section, keeping the `#local-vm-hardware-acceleration` anchor: the VM is recreated under `.local/tools/linux-vm/` by the phase that needs it, and the tracked launcher lives at `src/godot-game/tools/verification/linux-vm/start-vm.cjs`;
     - the creator's post-merge migration steps: switch the main checkout to `dev` (its squash-merged branch needs `git branch -D`), `dotnet build-server shutdown`, the migration dry run, `--apply`, then the cleanup dry run.
   - `docs/workflow/cleanup.md`, cleanup only:
     - the keep/delete table for S and L, the legacy row and both lock scopes;
     - known exceptions: the creator declared the Phase 4 inputs lost on 2026-11-23, and their checks stay blocked under the owners the roadmap assigns;
     - C4, Phase 11 (TEST-12);
     - the new-output-location rule names `.local/runs/<tool>` or `.local/temp/<purpose>`;
     - setup recreates `.local/temp`;
     - the test statement reports the `tests` gate count at the tested revision;
     - remove the worktree-creation bullet and link to ai-collaboration.md.
   - `docs/workflow/ai-collaboration.md`:
     - the worktree location rule: `<git config --get worktrees.root>/<main checkout folder name>/kz-<task-id>-<short-description>`. When that per-user value is unset, the system temp folder stands in for it. Explain how to set it with no machine path;
     - fix line 51's Phase 5 reuse sentence to Phase 11 (TEST-12), keeping the precedence note.
   - `docs/workflow/verification.md`: the runs path, and C3 (the "Existing Phase 1 Commands and Evidence" paragraph owner becomes Phase 11 under TEST-12).
   - `foundation-checks.md` and `lan-foundation-checks.md`: run paths, completions and the shared desktop lock.
   - `docs/content/production.md`: publication and run locations exactly as Task 2's code enforces them. It is not a pack-index file; confirm with `git grep -n production.md src/godot-game/game/content/pack-index.json`.
   - `src/godot-game/tools/README.md`:
     - outputs under `.local/runs/<tool>`;
     - `workspace-paths.cjs`, `workspace-migrate.cjs` and `node-setup/`;
     - the rebinding tools, stating that they regenerate bindings from current bytes and are not replays of the one-shot utilities that the STATE process-history concern forbids reapplying.
   - Commit: `docs(quick-261123-ltj): document the .local layout, worktree rule, Node setup and migration`.

4. Planning records (CONTEXT: Phase 5 planning changes, Phase-named identifiers; orchestrator ROADMAP exception):
   - `.planning/STATE.md`:
     - C1: reword the `[PR #6 relocation]` bullet. The pinned recipes (export tracer, audio assembly) now fail fast with `CAPTURED_ROOT_REQUIRED` in every checkout, naming 05-CONTEXT D-46, until Phase 5 makes the Phase 4 checks it runs work in the relocated tree. The review-bound gates stay with Phase 11 under TEST-12.
     - Add to `[Cleanup 2026-11-22]` that the creator declared those inputs lost on 2026-11-23.
     - `[Local VM]`: the migration deletes the legacy VM after merge, and the phase that needs a VM recreates it under `.local/tools/`.
     - C5: in Session Continuity, drop the stale PR #5 "ready for review" sentence and the done "merges through its own planning PR" sentence; say PR #11 merged as `88953aa`; refresh "Last session" and "Stopped at".
     - Update `last_updated`, `last_activity`, `last_activity_desc` and the "Last activity" line.
     - Add a Deferred Items row: backlog 999.1, rename of the phase-named identifiers, captured and not scheduled, quick task 261123-ltj on 2026-11-23.
     - Add no Quick Tasks row.
   - `.planning/ROADMAP.md`, per `.claude/gsd-core/workflows/add-backlog.md`, without its commit step:
     - append `## Backlog` after the footer, then `### Phase 999.1: Rename phase-named identifiers (BACKLOG)` with `**Goal:** [Captured for future planning]`, a scope line, `**Requirements:** TBD`, `**Plans:** 0 plans` and `Plans:` / `- [ ] TBD (promote with /gsd-review-backlog when ready)`;
     - the scope line names `Phase1.cs`/`Phase1.tscn`, the `KingdomZombies.Phase1.Tests` project, the `PHASE1_*`/`PHASE2_*`/`PHASE3_*`/`KZ_PHASE4_*` variables, the `phase1`/`phase2`/`phase3` case families, the `Phase4 Windows inspection` preset and the VM user, and notes that renaming changes test FQNs and case IDs across the registers;
     - create `.planning/phases/999.1-rename-phase-named-identifiers/.gitkeep`.
     The one other permitted ROADMAP edit is C7. The "Delivery and Ownership Rules" sentence says "Fourteen of the current 135". That contradicts PR #11's own amendment paragraph and traceability total, which read 136 with 19 complete. Change it to "Nineteen of the current 136" only after confirming the total row still reads 136 and 19 complete; otherwise leave it and report. Make no other ROADMAP change, including the pre-existing phase-list glitches.
   - The fsu `state` gate pattern (C2) is not carried over; the SUMMARY notes the supersession.
   - Commit: `docs(quick-261123-ltj): fix PR #11 contradictions and record the identifier backlog`.

5. ac-08 last. Run `ac08-bindings.cjs --write` on the clean tree, then `--check --rev HEAD` after committing the two catalogs: `fix(quick-261123-ltj): regenerate ac-08 repository stages and caller bindings`. A later commit that adds or removes a tracked file outside `.planning`, `.claude`, `.codex` or `.agents`, or changes a hashed row, means redoing this step.

6. Final checks at the final commit: run the verify, using the background if a call would exceed the shell timeout. Repair failures with fix commits and rerun everything. Make no commit after the passing run; that HEAD is the tested revision. A residue hit outside the listed allow set is either rebound or, if it is a classified legacy, captured, logical or historical entry, added to `--allow` with its reason in the SUMMARY residue table.

7. Write `.planning/quick/261123-ltj-replace-phasen-folders-with-one-ignored-/261123-ltj-SUMMARY.md` from the summary template and `results.md`. Do not stage it. It contains:
   - frontmatter `status: complete` and the tested revision as a full SHA;
   - a results table (command | tested revision | exit status | counts) for:
     - every gate;
     - the BASE tool proofs from Task 1;
     - the `## Gate controls` forced-FAIL runs and their paired PASS runs from Task 1, with the Task 1b entry check;
     - the Node setup run and its idempotent rerun;
     - the session hook checks;
     - the migration dry run (exit 1, the refusal, the VM deletion size);
     - the per-file test counts;
   - the residue table: each remaining `.phaseN` hit with its class;
   - a not-run / expected-blocked table: export, editor import, headless launch, game windows, the full case selection, bootstrap acquisition, the Codex hook under Codex, the Linux setup branch if `xz` was missing, the C# shared-package exception (compiled, not executed), the launcher (syntax only), and `export.*`/`rules.export-layout-*`/model tests/`--verify-completion` blocked on lost inputs;
   - known gaps;
   - deviations, including the kept legacy ignore line and the per-checkout feed under `nuget/`;
   - the task-created ignored paths: the worktree's `.local/temp`, `.local/runs` and `.local/nuget/feed`; the shared `nuget/packages`, `nuget/http` and `nuget/scratch` seeds; `tools/node` and `downloads/node`; any system-temp scratch;
   - `## How to reproduce`, with commands from the checkout root.
   Use symbolic locations only.
  </action>
  <verify>
    <automated>sh src/godot-game/tools/node-setup/setup.sh --check && node src/godot-game/tools/verification/rebind-gates.cjs --base 88953aad1e7d7b6e8855e53c3692bdeeffe1ed29 --keep docs/code,src/godot-game/production/assets,docs/workflow/platform-proof.md,docs/workflow/phase4-layout-integration.md,src/godot-game/tools/asset-pipeline/extract-media.py,src/godot-game/tools/asset-pipeline/extract-audio.py,specs,.planning/REQUIREMENTS.md --allow .planning/STATE.md,src/godot-game/tools/workspace-paths.cjs,src/godot-game/tools/workspace-paths.test.cjs,src/godot-game/tools/workspace-cleanup.cjs,src/godot-game/tools/workspace-cleanup.test.cjs,src/godot-game/tools/workspace-migrate.cjs,src/godot-game/tools/workspace-migrate.test.cjs,src/godot-game/tests/verification/WorkspacePaths.cs,src/godot-game/tests/verification/ExemptionValidator.cs,src/godot-game/tests/verification/DocumentationValidator.cs,src/godot-game/tests/verification/ProjectEvaluator.cs,src/godot-game/tools/asset-pipeline/check.cjs=1,src/godot-game/tools/asset-pipeline/cases/audio.cjs=3,docs/architecture/repository-layout.md,docs/workflow/cleanup.md,docs/workflow/setup.md,docs/workflow/engine-defects.md,src/godot-game/tools/README.md --pattern '(?<![A-Za-z0-9_)\]])\.phase[0-9](?![0-9A-Za-z_.])' history newhost residue lists tests roots build assets ac08 docs</automated>
  </verify>
  <done>
    - Every gate prints PASS at the final commit:
      - history and newhost unchanged against BASE;
      - residue only in classified files;
      - lists 686/16/32/75;
      - all tools tests pass, including the hook, cleanup, migration, paths and bindings tests;
      - roots resolve from the worktree, the main checkout and the system temp folder;
      - strict build 0/0 with no network GET;
      - assets (423 modules, 180 valid);
      - ac08 current, with 6 caller hashes and 9 staged plans;
      - docs 36/36.
    - Node 24.22.1 is installed once in `S/tools/node/` from the pinned archive, and a second run downloads nothing.
    - STATE and ROADMAP carry the fixes and backlog 999.1, without the 261123-ltj row.
    - The SUMMARY exists uncommitted with status complete.
    - The tree is clean apart from the quick-task folder.
  </done>
</task>

</tasks>

## Source coverage audit

| Source | Item | Covered by |
| --- | --- | --- |
| GOAL | One ignored `.local/` root, binaries per repository, Node 24 setup with session hooks, docs | Tasks 1–3 |
| CONTEXT | Folder name and layout; the cleanup keep/delete rule | Shared contracts; T1 step 4 (`.gitignore`, runners); T2 steps 2 and 4 |
| CONTEXT | Sharing via `--git-common-dir` and an env override; NuGet scratch and the desktop lock shared; CLI homes per checkout; narrow bootstrap and export-trace exceptions | Shared contracts; T1 step 4; T2 step 2 (rule 5) |
| CONTEXT | What stays where it is (`bin`, `obj`, `.godot`, GSD paths, tracked root names) | No task moves them; the build filter keeps skipping build outputs (T2 step 4) |
| CONTEXT | Durable data: tracked launcher; Phase 4 inputs declared lost; captured recipes keep bytes and fail fast | T2 step 6; T3 steps 3–4; rule 4 in T2 step 2 |
| CONTEXT | Node 24 pin, setup scripts, run once | T3 step 2 |
| CONTEXT | Node on PATH: Claude hook, Codex advisory, no persistent PATH change | T3 step 2; setup.md in T3 step 3 |
| CONTEXT | Phase-named identifiers become backlog | T3 step 4 (999.1, `.gitkeep`, Deferred Items row) |
| CONTEXT | Extras: worktree rule; rebinding tools tracked; `core.longpaths`; publishing by the orchestrator | T3 step 3; T1a steps 2–3 (tools, gate controls, seam); T3 step 3; Locations and shell rules (never push) |
| CONTEXT | Phase 5 planning changes: review PR #11 and fix contradictions | T3 steps 3–4 (C1–C7) |
| CONTEXT | Verification scope and not-run list | Gate invocations; T3 step 7 tables |
| CONTEXT | Decisions after research: Codex advisory hook; VM deleted not moved; migration refuses on the old branch; creator switches the main checkout | T3 step 2; T2 step 5; T2 verify; setup.md in T3 step 3 |
| CONTEXT | Specifics: central helpers; no weakening; history keeps bytes; no machine paths; do not migrate the main toolchain; downloads; locations in records | Shared contracts; gates history, newhost, lists, assets, ac08; T2 step 8 (dry run only); T3 step 2 |
| RESEARCH | Sections 1–2 hook scripts and settings; section 3 backlog convention; section 4 tools and tests; section 5 migration (VM rebase superseded by the creator's delete decision); section 6 seeding; section 7 C1–C6; section 8 exclude; section 9a physical register reads | T3 step 2; T3 step 4; T1 steps 2–3; T2 step 5; T1 step 4; T3 steps 3–4; already done by the orchestrator; T1 step 4 (C#) |
| PRE-RESEARCH | Inventory, path construction, hash cascade order, sharing risks, proposed layout | Live inventory; cascade order T2 step 3 then T3 step 5; Shared contracts |

Out of scope by decision, not gaps: identifier renames (backlog), moving `bin`/`obj`/`.godot` or the GSD paths, applying the migration, the VM header rebase, pre-existing ROADMAP glitches, and the export, import and game runs.

<threat_model>
## Trust Boundaries

| Boundary | Description |
|----------|-------------|
| nodejs.org to shared tools | A downloaded archive becomes an executable on every agent's PATH |
| Shared root to every worktree | One folder outside each worktree is now trusted by bootstrap and the register reads |
| Executor edits to the committed tree | Hash-bound validators, pins, history and expected counts that a shortcut could weaken |
| Hook commands to agent sessions | Tracked scripts run automatically at session start |
| Migration and cleanup to the main checkout | Commands that rename or delete about 10 GB of durable toolchain state |

## STRIDE Threat Register

| Threat ID | Category | Component | Severity | Disposition | Mitigation Plan |
|-----------|----------|-----------|----------|-------------|-----------------|
| T-ltj-01 | Tampering | Node setup scripts | high | mitigate | SHA-256 pins from the official SHASUMS256 in `toolchain.json` are checked before extraction; HTTPS only; `.part` with a nonce, then rename; extraction into a temporary sibling, then rename; the toolchain lock; `--check` re-verifies. |
| T-ltj-02 | Elevation of privilege | Shared-root exceptions in bootstrap and `CheckHash` | high | mitigate | `insideShared` and `IsSharedPackage` allow only realpaths inside exact S, for the named purpose (tools, packages). `PATH_OUTSIDE_WORKSPACE` and `SUPPRESSION_ORIGIN_PATH` stay for everything else. `workspace-paths.test.cjs` rejects `..`, sibling-prefix and outside paths. |
| T-ltj-03 | Tampering | Validators, pins, counts, history | high | mitigate | The tools are proven on BASE bytes before any edit. Every gate is shown to fail before its PASS counts: `judge` controls in memory, CLI controls on a temporary repository, and forced real-tree runs recorded in `results.md`. Collection fails closed, and unknown pathspecs fail. Gates fix lists (equal to base), 423 modules and 180 valid, 42 keys, 9 staged plans, and the docs count equal to the base catalog's 36. `history` requires unchanged records and `--keep` files; ac-08 rows are re-derived from HEAD blobs. |
| T-ltj-04 | Information disclosure | Committed lines and records | high | mitigate | The `newhost` gate rejects any new machine-path line; hooks and docs derive paths at runtime; captured recipe roots are parsed from existing bytes, never rewritten; records use symbolic locations. |
| T-ltj-05 | Information disclosure | VM private key | high | mitigate | Only `start-vm.cjs` is copied, and it reads the public key at runtime. The migration deletes the VM folder and never copies it; no key file enters a tracked location. |
| T-ltj-06 | Denial of service | Main checkout toolchain under migration or cleanup | high | mitigate | Dry run by default; refusals for the branch, ignore rule, override, locks, live processes, links, tracked files and unverified targets; `--apply` never runs in this task; the dry run is checked for zero writes. Cleanup never deletes tools, downloads or nuget, and refuses while any lock exists. |
| T-ltj-07 | Tampering | Session hooks | medium | mitigate | No untrusted input; single-quote escaping; fail-open; Codex hooks need per-hash trust; the GSD hook groups stay byte-identical. |
| T-ltj-08 | Denial of service | Shared NuGet packages under concurrent restores | medium | mitigate | A shared `NUGET_SCRATCH` (NuGet's cross-process locks); build servers disabled in gates; the toolchain lock around acquisition. |
| T-ltj-09 | Tampering | Tests writing into the real shared root | medium | mitigate | Rule 7: tests set `KZ_SHARED_LOCAL` to temporary folders; only NuGet restore writes to the shared packages folder. |
| T-ltj-SC | Tampering | npm, pip and cargo installs | low | accept | None occur. The only download is the pinned Node archive (T-ltj-01). NuGet restore is locked mode from copied caches, and the `build` gate fails on any HTTP GET. |
</threat_model>

<verification>
- Task 1a exit (the seam):
  - the tools are proven on BASE;
  - every gate is shown to fail (in memory, on a temporary repository, and in forced real-tree runs);
  - the tools commit passes `history`, `newhost` and `residue` on its own bytes.
- Task 1b: its entry check, then the Task 1 verify (tracer): resolution, the strict build and docs from the task worktree.
- Task 2 verify: the runner rebind, the asset cascade, cleanup and migration tests, and the read-only main-checkout dry run.
- Task 3 verify (final): `sh src/godot-game/tools/node-setup/setup.sh --check`, then every gate (`history newhost residue lists tests roots build assets ac08 docs`) at the final commit.
- Not run, and reported as not run: export, editor import, headless launch, game windows, the full case selection, Godot bootstrap acquisition and the Codex hook under Codex. The orchestrator re-runs the docs gate after adding the STATE row.
</verification>

<success_criteria>
- Linear commits on `feature/261123-ltj-local-folder`, nothing pushed: tools, tracer slice, runners and cascade, cleanup and migration, Node and hooks, docs, planning records, then ac-08 last.
- Every gate in the Task 3 verify prints PASS at the tested revision. Lists are 686/16/32/75, the build is 0/0 and docs pass 36/36.
- The shared Node 24.22.1 is installed from the pin, the hooks are tested, and the migration dry run refused without writing.
- The SUMMARY is written, uncommitted, with How to reproduce.
</success_criteria>

<output>
Write `.planning/quick/261123-ltj-replace-phasen-folders-with-one-ignored-/261123-ltj-SUMMARY.md` and leave it uncommitted. The orchestrator adds the STATE row linked to that file, commits the quick-task artifacts, re-runs the docs gate at the final HEAD, and pushes.
</output>
