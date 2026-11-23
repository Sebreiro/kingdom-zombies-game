---
phase: quick-261123-ltj
reviewed: 2026-11-23T17:50:53Z
depth: standard
files_reviewed: 105
files_reviewed_list:
  - .claude/settings.json
  - .codex/hooks.json
  - .gitignore
  - .planning/ROADMAP.md
  - .planning/STATE.md
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
  - src/godot-game/game/content/pack-index.json
  - src/godot-game/game/content/packs/delivery.json
  - src/godot-game/tests/verification/AnalyzerInputIdentityTests.cs
  - src/godot-game/tests/verification/AnalyzerSourceConsumerTests.cs
  - src/godot-game/tests/verification/Analyzers/ContinuationIndentAnalyzerTests.cs
  - src/godot-game/tests/verification/AppearanceStateTests.cs
  - src/godot-game/tests/verification/BuildFixture.cs
  - src/godot-game/tests/verification/CharacterPresentationTests.cs
  - src/godot-game/tests/verification/ConfigurationTests.cs
  - src/godot-game/tests/verification/ContentInspectorTests.cs
  - src/godot-game/tests/verification/ContentNativePermissionTests.cs
  - src/godot-game/tests/verification/DocumentationFixture.cs
  - src/godot-game/tests/verification/DocumentationValidator.cs
  - src/godot-game/tests/verification/EngineGeneratedTests.cs
  - src/godot-game/tests/verification/EngineRun.cs
  - src/godot-game/tests/verification/ExemptionTests.cs
  - src/godot-game/tests/verification/ExemptionValidator.cs
  - src/godot-game/tests/verification/FoundationCapturePublicationTests.cs
  - src/godot-game/tests/verification/FoundationLanServicesTests.cs
  - src/godot-game/tests/verification/FoundationServiceSubstitutionProof.cs
  - src/godot-game/tests/verification/GeneratedCallSiteAdapterTests.cs
  - src/godot-game/tests/verification/GeneratedExportLayoutTests.cs
  - src/godot-game/tests/verification/GeneratedLayoutAdapterTests.cs
  - src/godot-game/tests/verification/GeneratedSyntaxAdapterTests.cs
  - src/godot-game/tests/verification/GraphFixture.cs
  - src/godot-game/tests/verification/PinEdgeTests.cs
  - src/godot-game/tests/verification/PinsTests.cs
  - src/godot-game/tests/verification/ProjectEvaluator.cs
  - src/godot-game/tests/verification/RunnerContractTests.cs
  - src/godot-game/tests/verification/RunnerProcessFixture.cs
  - src/godot-game/tests/verification/WorkspacePaths.cs
  - src/godot-game/tools/README.md
  - src/godot-game/tools/asset-pipeline/cases/audio-feasibility.cjs
  - src/godot-game/tools/asset-pipeline/cases/audio.cjs
  - src/godot-game/tools/asset-pipeline/cases/bodies-a.cjs
  - src/godot-game/tools/asset-pipeline/cases/bodies-b.cjs
  - src/godot-game/tools/asset-pipeline/cases/defenses.cjs
  - src/godot-game/tools/asset-pipeline/cases/export-layout.cjs
  - src/godot-game/tools/asset-pipeline/cases/export-portability.cjs
  - src/godot-game/tools/asset-pipeline/cases/export-profile.cjs
  - src/godot-game/tools/asset-pipeline/cases/hub.cjs
  - src/godot-game/tools/asset-pipeline/cases/items.cjs
  - src/godot-game/tools/asset-pipeline/cases/main-buildings.cjs
  - src/godot-game/tools/asset-pipeline/cases/media.cjs
  - src/godot-game/tools/asset-pipeline/cases/provenance.cjs
  - src/godot-game/tools/asset-pipeline/cases/runner.cjs
  - src/godot-game/tools/asset-pipeline/cases/text-data.cjs
  - src/godot-game/tools/asset-pipeline/cases/ui-camp.cjs
  - src/godot-game/tools/asset-pipeline/cases/ui-inventory.cjs
  - src/godot-game/tools/asset-pipeline/cases/ui-kit.cjs
  - src/godot-game/tools/asset-pipeline/cases/world.cjs
  - src/godot-game/tools/asset-pipeline/check.cjs
  - src/godot-game/tools/asset-pipeline/content.cjs
  - src/godot-game/tools/asset-pipeline/evidence.cjs
  - src/godot-game/tools/asset-pipeline/export.cjs
  - src/godot-game/tools/asset-pipeline/required-cases.json
  - src/godot-game/tools/asset-pipeline/scenarios.cjs
  - src/godot-game/tools/gameplay-checks/check.cjs
  - src/godot-game/tools/gameplay-checks/required-cases.json
  - src/godot-game/tools/gameplay-checks/runner-fixture.cjs
  - src/godot-game/tools/lan-checks/check.cjs
  - src/godot-game/tools/lan-checks/foreground.test.cjs
  - src/godot-game/tools/lan-checks/inherited.cjs
  - src/godot-game/tools/lan-checks/inherited.test.cjs
  - src/godot-game/tools/lan-checks/network.cjs
  - src/godot-game/tools/lan-checks/rendered-stream.test.cjs
  - src/godot-game/tools/lan-checks/revisions.test.cjs
  - src/godot-game/tools/lan-checks/runner-fixture.cjs
  - src/godot-game/tools/lan-checks/stopped-fixture.cjs
  - src/godot-game/tools/lan-checks/timing.test.cjs
  - src/godot-game/tools/node-setup/session-path.ps1
  - src/godot-game/tools/node-setup/session-path.sh
  - src/godot-game/tools/node-setup/session-path.test.cjs
  - src/godot-game/tools/node-setup/setup.ps1
  - src/godot-game/tools/node-setup/setup.sh
  - src/godot-game/tools/verification/ac08-bindings.cjs
  - src/godot-game/tools/verification/asset-bindings.cjs
  - src/godot-game/tools/verification/bindings.test.cjs
  - src/godot-game/tools/verification/bootstrap.cjs
  - src/godot-game/tools/verification/check.cjs
  - src/godot-game/tools/verification/linux-vm/start-vm.cjs
  - src/godot-game/tools/verification/rebind-gates.cjs
  - src/godot-game/tools/verification/required-cases.json
  - src/godot-game/tools/verification/toolchain.json
  - src/godot-game/tools/workspace-cleanup.cjs
  - src/godot-game/tools/workspace-cleanup.test.cjs
  - src/godot-game/tools/workspace-migrate.cjs
  - src/godot-game/tools/workspace-migrate.test.cjs
  - src/godot-game/tools/workspace-paths.cjs
  - src/godot-game/tools/workspace-paths.test.cjs
findings:
  critical: 0
  warning: 9
  info: 10
  total: 19
  necessity: 0
status: issues_found
fix_status: partial
fixed_at: 2026-11-23T19:10:00Z
fixed_revision: cfbbb5fc88155e964ce0498c00c005bf987bd260
fixes:
  WR-01: {status: fixed, commit: d207354}
  WR-02: {status: fixed, commit: 8b77f1c}
  WR-03: {status: fixed, commit: 5bec726}
  WR-04: {status: fixed, commit: 5bec726}
  WR-05: {status: fixed, commit: b094936}
  WR-06: {status: fixed, commit: 78c020e}
  WR-07: {status: fixed, commit: d3b00c8}
  WR-08: {status: fixed, commit: 3d06ede, note: "docs only; the catalog, register and rule matrix still say Phase 5 (recorded gap)"}
  WR-09: {status: fixed, commit: 7cc3454}
  IN-01: {status: fixed, commit: b0ee77b}
  IN-02: {status: fixed, commit: 1987c8c}
  IN-03: {status: fixed, commit: fbfc383}
  IN-04: {status: fixed, commit: 69b76fa}
  IN-05: {status: acknowledged, commit: null}
  IN-06: {status: fixed, commit: d65cd11}
  IN-07: {status: acknowledged, commit: null}
  IN-08: {status: fixed, commit: 3b479ab}
  IN-09: {status: fixed, commit: dc9960e}
  IN-10: {status: acknowledged, commit: null}
  ac08_rebind: {status: done, commit: cfbbb5f}
---

# Quick Task 261123-ltj: Code Review Report

**Reviewed:** 2026-11-23T17:50:53Z
**Depth:** standard
**Files Reviewed:** 105 (`88953aa..19f5818`, excluding `.planning/quick/`)
**Status:** issues_found

Locations are symbolic. The **main checkout** is the primary working tree. The **task worktree** is this task's linked worktree. The **shared root** is the main checkout's `.local/` (or `KZ_SHARED_LOCAL`). The **local root** is a checkout's own `.local/`.

## Summary

I reviewed every changed file through `git diff 88953aa..19f5818` and read the surrounding code. Most attention went to the seven focus areas. The core resolver, the runner rebinding and the C# twin are consistent with one another. The legacy map agrees with the physical paths the code now writes, and I found no live `.phaseN` path outside the classified residue. No added line contains a machine-specific path. ac-08 and the asset cascade regenerate from bytes correctly, and I found no vacuous pass in `history`, `lists`, `tests`, `roots`, `build`, `assets`, `ac08` or `docs`.

There are no blockers. Nine warnings remain, in four groups:

- **Migration safety:** the dry run does not verify move sources, and the migration follows links in ancestor folders.
- **The shared-root override:** it is not normalized on Windows and not bounded, so the "narrow exception" is only as narrow as the variable's value.
- **Ignore state:** the shared root is written even when the main checkout does not ignore `.local/`.
- **Weakening and docs:** two export guards were weakened against the no-weakening rule, the `residue` gate passes without limit for 15 uncounted allow entries, and the SUMMARY understates the PR #11 contradictions, including one in the executable catalog.

The necessity rubric was applied, and no finding qualifies. The `IsSharedPackage` exception is reachable: `docs/code/generated-callsite-adaptations.json` rows name `.phase1/packages/...`, and `ExemptionValidator.cs:540-542` passes them to `CheckHash`.

Out of scope and not re-raised: the known gaps the SUMMARY already lists, unless noted below as understated; the pre-existing GSD machine paths in `.codex/hooks.json` and `.claude/settings.local.json`; and the pre-existing `rules.xunit-producer-hash` separator issue.

## Warnings

### WR-01: The migration dry run verifies no move source, so `--apply` can delete the VM and then wedge on a later row

**File:** `src/godot-game/tools/workspace-migrate.cjs:152-174`, `:313-329`; `docs/workflow/setup.md:105`
**Issue:** `classify` runs `row.verify` only when the target already exists (line 168). For a plain `move` row it returns `decision: 'move'` without checking the source against its pin, and without checking that the target's parent can be created. `apply` then works row by row:

1. It deletes the 8.2 GB VM first.
2. For each moved row, it renames and verifies only after the rename (lines 320-323). On a mismatch it renames back and throws.

**Failure scenario:** the legacy `phase4-exports/…tpz` was truncated by an interrupted download, or the editor folder's console executable differs from `executableSha256`. The dry run prints `move …` and exits 0. `--apply` then deletes the VM, moves the Godot zips, and stops at the templates row. Every later row stays unmigrated: editor, `linux-prep`, feed, packages, http-cache, `cli-home`, the stray file and `dotnet-home`.

A rerun hits the same row and stops again, and no flag skips it. The migration can never finish without manual surgery. The test `apply stops at the first failure…` (`workspace-migrate.test.cjs:258-266`) encodes this: a file at `.local/downloads` passes the dry run but fails `--apply` after the VM is gone.

`setup.md:105` also says these items "move and are verified against their pins" and that the dry run "exits 0 only when nothing would be refused". That reads as a pre-verified plan, which it is not.

**Fix:** Verify sources in `classify` and refuse there. Check the target's ancestor chain in the plan as well, so that any problem is refused before the irreversible VM row runs:

```js
// in classify(), before the final move decision
if (row.verify && !row.verify(row.source)) return {...row, decision: 'refuse', line: `refuse: ${row.legacy} does not verify against its pin`};
for (let p = path.dirname(row.target); p.length > main.length; p = path.dirname(p))
  if (present(p) && !fs.lstatSync(p).isDirectory()) return {...row, decision: 'refuse', line: `refuse: ${slashed(path.relative(main, p))} is not a folder`};
```

Alternatively, run the VM deletion last.

### WR-02: The migration follows junctions and symlinks in ancestor folders and can delete or move data outside the main checkout

**File:** `src/godot-game/tools/workspace-migrate.cjs:132-149`, `:316`, `:319-320`
**Issue:** `unsafeSource` runs `lstat` only on the leaf `row.source`. The intermediate folders `.phase1`, `.phase1/tools` and `.phase1/cache` are never checked, and neither are the target ancestors `.local`, `.local/downloads`, `.local/tools`, `.local/nuget`. `git check-ignore` and `git ls-files` decide by path string, so they pass even when an ancestor is a reparse point. The leftover scanner does check `isRealDirectory` for `.phase1` (line 287), but the rows do not.

**Failure scenario:** a user moved the 8 GB VM off the system drive by making `.phase1/tools` a junction to `<other drive>/kz-tools`. Another clone or an older checkout shares that folder through the same junction. `--apply` then:

- runs `rmSync` on `<other drive>/kz-tools/linux-vm`;
- renames `<other drive>/kz-tools/Godot_…` into the main checkout if both are on one volume (otherwise EXDEV stops it after the VM is gone);
- deletes the shared `packages` and `cli-home` through a junctioned `.phase1`.

All of these are outside the intended roots.

**Fix:** Require each source and target to resolve to exactly the lexical location. Refuse otherwise:

```js
const real = fs.realpathSync.native(path.dirname(row.source));
if (!samePath(real, path.join(fs.realpathSync.native(main), ...path.posix.dirname(row.legacy).split('/'))))
  return 'has a linked ancestor folder';
```

Apply the same check to the nearest existing ancestor of `row.target`. Add a fixture in which `.phase1/tools` is a junction.

### WR-03: On Windows, a root-relative `KZ_SHARED_LOCAL` is accepted and resolved against the current drive; the scripts disagree on its meaning

**File:** `src/godot-game/tools/workspace-paths.cjs:29-31`; `src/godot-game/tools/node-setup/setup.ps1:67,75`; `src/godot-game/tools/node-setup/setup.sh:60-63,70`; `src/godot-game/tests/verification/WorkspacePaths.cs:156`
**Issue:** Each implementation decides differently what counts as absolute:

| Implementation | Check | Result for `\x` or `/d/…` |
| --- | --- | --- |
| `workspace-paths.cjs` | `path.isAbsolute`, then `path.resolve` | Accepted and resolved against the current drive |
| `setup.ps1` | `IsPathRooted`, then `GetFullPath` | Same as above |
| `WorkspacePaths.cs` | `IsPathFullyQualified` | Rejected |
| `setup.sh` | `/*` taken as an MSYS path | `/d/…` read as `<drive D>:\…` |

I ran it on the tested revision under the repository's own shell rule (`MSYS_NO_PATHCONV=1`, which stops MSYS converting the value). `KZ_SHARED_LOCAL=/d/<repo>/.local node src/godot-game/tools/workspace-paths.cjs --print-roots` reported `shared` as `<current drive>:\d\<repo>\.local`.

**Failure scenario:** a user exports the override in Git Bash in MSYS form. The results split:

- `setup.sh` and `session-path.sh` install Node under `<drive D>:\<repo>\.local`.
- Every Node tool and bootstrap use the different folder `<current drive>:\d\<repo>\.local`. They download Godot (111 MB + 257 MB) and restore all NuGet packages into it, and `isolationEnv` then exports that wrong folder to children.
- A direct `dotnet test` without the runner throws `SHARED_ROOT_INVALID` instead.

So one setting yields three different results.

**Fix:** Use a single rule. On Windows, require a drive-qualified or UNC path (`/^[A-Za-z]:[\\/]|^\\\\/`) in `workspace-paths.cjs` and `setup.ps1` (`[IO.Path]::IsPathFullyQualified` is not in Windows PowerShell 5.1, so use the regex). In `setup.sh` and `session-path.sh`, convert with `cygpath -m` before validating, and pass the converted value on. Add a `workspace-paths.test.cjs` case for `\x` and `/d/x` on win32.

### WR-04: `KZ_SHARED_LOCAL` is unbounded, so the "narrow" shared-root exception covers whatever folder it names

**File:** `src/godot-game/tools/workspace-paths.cjs:29-31`, `:146-154`; `src/godot-game/tools/asset-pipeline/content.cjs:51-58`; `src/godot-game/tools/verification/bootstrap.cjs:82-85`; `src/godot-game/tools/node-setup/session-path.sh:12-13`; `src/godot-game/tools/node-setup/session-path.ps1:7-11`
**Issue:** The only check on the override is that it is absolute. A drive root, the checkout itself, the checkout's parent or a tracked subtree all become the shared root. `insideShared` then accepts every file beneath it. I verified this on the tested revision:

- With the override set to a drive root, `insideShared('<drive>:/Windows/System32/notepad.exe')` returned `true`.
- With the override set to the worktrees parent, the task worktree's tracked `.gitignore` returned `true`.

`resolveShared`, `sharedFile` and `ENGINE_OUTSIDE_SHARED` therefore stop at whatever folder the variable names. Bootstrap, the NuGet isolation and the Node setup also write `tools/`, `downloads/` and `nuget/` there.

**Failure scenario:** a user sets `KZ_SHARED_LOCAL` to the main checkout's root instead of its `.local`. Bootstrap writes `tools/godot/…`, `downloads/godot/…` and `nuget/packages/…` (more than 1 GB) into the main checkout's tracked tree as unignored files, where `git add -A` would commit them.

The session hooks go further: they do not check absoluteness at all (`session-path.sh:13`, `session-path.ps1:7`). A relative value puts a relative `PATH` entry into `CLAUDE_ENV_FILE`, which every later Bash call resolves against its own working directory.

**Fix:** Bound the override in `roots()` and `ResolveShared()`:

- reject a filesystem root;
- reject a folder inside any checkout of this repository (compare against `git rev-parse --show-toplevel` and the common dir's parent) unless it is that checkout's `.local`;
- optionally require the basename `.local`.

Apply the same absolute check in both session hooks, and fail open (exit 0 and write nothing) on rejection.

### WR-05: The shared root is written even when the main checkout does not ignore `.local/`, so the binaries can be committed

**File:** `src/godot-game/tools/verification/bootstrap.cjs:398-399`; `src/godot-game/tools/workspace-paths.cjs:164-167`; `src/godot-game/tools/node-setup/setup.sh:129`; `src/godot-game/tools/node-setup/setup.ps1:118`; `src/godot-game/tests/verification/WorkspacePaths.cs:109-113`
**Issue:** Any linked worktree can create folders in the main checkout's `.local/` through any of these paths:

- bootstrap and `--print-env`;
- the Node setup;
- C# `Isolate` and the runners' `ensure`.

None of them checks that the main checkout ignores `/.local/`. That holds only while the main checkout is on a branch that contains this change. The task itself had to work around this with a repository-local `info/exclude` entry (SUMMARY, "Repository-local exclude").

**Failure scenario:** the main checkout is on any branch older than this change, or on an older tag for bisecting. A worktree runs `bootstrap.cjs --probe engine` or `setup.sh`. The main checkout's `git status` then shows `.local/` with gigabytes of Godot, Node and NuGet binaries as untracked, one `git add -A` away from a commit. The creator's requirement was that these binaries are never committed.

**Fix:** Make the root ignore itself. When creating the shared root (and the local root), write `.local/.gitignore` containing `*`. Then adjust `workspace-cleanup.cjs` and `workspace-migrate.cjs` (`isIgnored(root, '.local/')`, `ignoredByGitignore`) to accept a self-ignoring `.local`.

A simpler alternative: before the first write, refuse with `SHARED_ROOT_NOT_IGNORED` unless `git -C <main> check-ignore -q .local/` succeeds.

### WR-06: Two export guards were weakened, against the task's no-weakening rule

**File:** `src/godot-game/tools/asset-pipeline/export.cjs:238`, `:378`
**Issue:** Both guards lost coverage:

- **Packaged members (line 238).** Before, any `.phase1` or `.phase4` segment was rejected, and those folders held every run, temp, feed and cache output. Now only `.local/(nuget|tools|downloads)` is rejected. A packaged member under `.local/runs/…`, `.local/temp/…` or `.local/locks/…`, the direct successors of the old `.phase4/` and `.phase1/runs|temp`, is accepted.
- **`HOST_AUTHORING_RESOLUTION` (line 378).** It detects the shared root only by the literal path segment `.local`. With `KZ_SHARED_LOCAL` set to a folder with another name, which is exactly the CI, extra-clone and source-copy configuration that `setup.md` documents, a host trace that resolves from the shared NuGet packages or the Godot tools no longer trips this check. `HOST_EXTERNAL_RESOLUTION` remains, but that only covers the assembly and native search paths, not the rest of the trace.

The SUMMARY's residue table classifies `export.cjs=2` as a "legacy guard", but it does not record that the guard's coverage narrowed.

**Fix:**

```js
// line 238: reject the whole workspace root, as before
!/(^|\/)(?:override\.cfg|packages\.lock\.json|production|\.phase1|\.phase4|\.local)(?:$|\/)/i.test(name)
// line 378: also reject the resolved shared root, whatever its name
const shared = workspace.locations().shared.toLowerCase();
need(!/KindomCrown|[\\/]sdk[\\/]|[\\/]\.phase1[\\/]|[\\/]\.local[\\/]/i.test(trace) && !trace.toLowerCase().includes(shared), 'HOST_AUTHORING_RESOLUTION');
```

### WR-07: The `residue` gate passes without limit for uncounted `--allow` entries

**File:** `src/godot-game/tools/verification/rebind-gates.cjs:185-189`; the SUMMARY "How to reproduce" command
**Issue:** An `--allow` entry without `=N` sets `count: null`. `judgeResidue` checks counts only for counted entries (line 186), so an uncounted file may hold any number of `.phaseN` hits. In the recorded final command, 15 of the 22 allow entries are uncounted, including these live files:

- `ExemptionValidator.cs`, `DocumentationValidator.cs`, `ProjectEvaluator.cs`;
- `workspace-paths.cjs`, `WorkspacePaths.cs`;
- `docs/workflow/setup.md`, `docs/workflow/cleanup.md`, `src/godot-game/tools/README.md`.

**Failure scenario:** a later edit adds `path.join(root, '.phase1/feed')` to `ExemptionValidator.cs`, or a new live `.phase1/…` instruction to `setup.md`. `residue` still prints PASS. The gate is meant to be the reusable tool for "future moves", so this weakness carries forward.

**Fix:** Require `=N` on every `--allow` entry, or add an explicit `--allow-uncounted` opt-out that the PASS line reports. Record the per-file counts in the documented command (the SUMMARY table already has them: STATE 2, layout 16, cleanup.md 4, and so on).

### WR-08: Known gap understated — the PR #11 Phase 5/11 contradictions go beyond the three listed lines and include the executable catalog

**File:** `docs/workflow/foundation-checks.md:63`; `docs/workflow/pre-phase5-testing.md:7`; `src/godot-game/tools/verification/required-cases.json:1811,1848,1867,…` (8 rows); `docs/code/register.json:2498,4026`; `docs/code/rule-matrix.md:5,64,97,117`; versus the edited `docs/workflow/setup.md:5,91` and `docs/workflow/verification.md`
**Issue:** The SUMMARY's known gap names only `foundation-checks.md:3` and `lan-foundation-checks.md:34,44`. There are more:

- `foundation-checks.md:63` still says "without Phase 5 reuse certification", and that file is in this diff.
- `pre-phase5-testing.md:7` says "Phase 5 implements the framework and actual hosted Windows/Linux compatibility". `verification.md:9` and `local-testing.md:70` move full hosted runs to Phase 11.
- **More importantly**, the executable source of truth still assigns the eight coverage identities to Phase 5:
  - `required-cases.json` has 8 rows with `"acceptancePhase": 5`, and 0 with 11;
  - `register.json` says `"owner": "Phase 5"`;
  - `rule-matrix.md` says "Pending Phase 5".

This task edited `setup.md:5`/`:91` and `verification.md` to say these are "Phase 11 obligations under TEST-12". The prose now contradicts the catalog that `check.cjs --phase 5` reads. `lan-foundation-checks.md:44` ("eight future Phase 5 declarations") is consistent with the code, and the newly edited lines are not.

**Failure scenario:** Phase 5 runs `check.cjs --phase 5` for its completion. It selects the 8 coverage cases, which cannot pass in Phase 5 under the amended scope. The phase then either fails its own completion or needs an undocumented omission. Meanwhile the docs say the cases belong to Phase 11.

**Fix:** List all of these in the SUMMARY's known gaps, and state plainly that the catalog, register and rule-matrix (all hash-bound, `--keep`) still say Phase 5. Either move `acceptancePhase` to 11 with the cascade rebind in Phase 5 planning, or revert the `setup.md` and `verification.md` wording to "declared `acceptancePhase: 5`; certification ownership moved to Phase 11 by PR #11, catalog rebind pending". Correct `foundation-checks.md:63` in this PR, because the file is already being edited.

### WR-09: The concurrent Node setup is not re-checked after taking the lock and nests a second copy of the install

**File:** `src/godot-game/tools/node-setup/setup.ps1:110-155`; `src/godot-game/tools/node-setup/setup.sh:106-163`
**Issue:** Both scripts test for the install folder before taking `toolchain.lock`, and do not test again after. `Move-Item`/`mv` onto an existing directory moves the source inside it.

**Failure scenario:** two sessions or worktrees run the setup at the same time on a fresh repository:

1. Run A takes the lock and installs.
2. Run B waits behind the `[ -d "$install" ]` check, takes the lock after A releases it, extracts again, and moves `staging/node-v24.22.1-win-x64` to `tools/node/node-v24.22.1-win-x64/node-v24.22.1-win-x64`.
3. `Assert-Install` passes, so the stray nested 90 MB copy goes unnoticed.

If B instead reached the lock while A held it, B would fail with `TOOLCHAIN_LOCKED`, which is acceptable. The bug is the TOCTOU window.

**Fix:** After acquiring the lock, test again: `if (Test-Path -LiteralPath $install -PathType Container) { Assert-Install; Write-PathLine; exit 0 }` in PowerShell, with the same in sh. Before moving, refuse when the destination exists.

## Info

### IN-01: The migration takes no `toolchain.lock` while applying

**File:** `src/godot-game/tools/workspace-migrate.cjs:220-224`, `:313-329`
**Issue:** Locks are checked only at plan time. A `bootstrap.cjs` or `setup.*` started in a linked worktree during `--apply` writes the same shared targets:

- `.local/downloads/godot/<zip>` and `SHA512-SUMS.txt`, where a file rename silently replaces the target;
- `.local/tools/godot/<dir>`, where a directory rename fails mid-run.

**Fix:** Create `locks/toolchain.lock` with `wx` for the length of `apply()`, and remove it in `finally`.

### IN-02: The Codex hook wrapper does not fail open

**File:** `.codex/hooks.json:17-18`; `docs/workflow/setup.md:56`
**Issue:** Both commands evaluate `git rev-parse --show-toplevel` in the outer shell, outside the fail-open script:

- If Codex starts outside the repository, or `git` is not on `PATH`, `sh "/src/godot-game/…/session-path.sh"` exits 127.
- The Windows variant fails in `Join-Path $null`.

`setup.md:56` says the hooks "never fail a session start".

**Fix:** Wrap each command in its own guard, for example `sh -c 'top=$(git rev-parse --show-toplevel 2>/dev/null) && sh "$top/…/session-path.sh" --codex; exit 0'`, and use `try { … } catch {}; exit 0` in PowerShell. Alternatively, soften the doc sentence.

### IN-03: The `newhost` gate misses common machine-path shapes

**File:** `src/godot-game/tools/verification/rebind-gates.cjs:34-35`
**Issue:** `HOST` does not match:

- `/home/<user>/…`, `/mnt/<drive>/…` and `/Users/<name>` (macOS case);
- UNC shares (two backslashes, a server name, a backslash, a share name);
- drive paths whose next character is not a letter (`<drive>:/_x`, `<drive>:\1`).

A new line with such a path passes. Note that `docs/workflow/platform-proof.md:27` already holds lines under the Linux VM user's home folder.

**Fix:** Add alternatives such as `/home/[^/\s]+/`, `\\\\[^\\\s]+\\`, `/mnt/[a-z]/` and `[A-Za-z]:[/\\][^\s]`.

### IN-04: The shared-file readers dropped the `nlink === 1` and reparse-ancestor checks of `bytes()` and `inside()`

**File:** `src/godot-game/tools/asset-pipeline/content.cjs:51-66`; `src/godot-game/tools/asset-pipeline/check.cjs:257-258`; `src/godot-game/tools/asset-pipeline/export.cjs:41-44`
**Issue:** The engine executable, the templates archive and the checksum file were read through `bytes()`/`inside()`, which reject hard links and links in any ancestor. They are now read through `sharedFile`/`sharedBytes`, which check only the leaf `isSymbolicLink`, plus real-path containment. Hashes still pin the content, so the impact is low, but this is a narrowing that the residue table does not mention.

**Fix:** Add `stat.nlink === 1` to `sharedBytes`, and walk ancestors below the shared root for reparse points, as `WorkspacePaths.IsSharedPackage` already does.

### IN-05: The C# `Local()` helpers map any legacy identity to the shared root without the `IsSharedPackage` containment checks

**File:** `src/godot-game/tests/verification/GeneratedExportLayoutTests.cs:191`; `src/godot-game/tests/verification/GeneratedLayoutAdapterTests.cs:193`; `src/godot-game/tests/verification/GeneratedSyntaxAdapterTests.cs:140`; `src/godot-game/tests/verification/EngineGeneratedTests.cs:321,769`; `docs/architecture/repository-layout.md:30`
**Issue:** Only `ExemptionValidator.CheckHash` applies the prefix gate, the `..` rejection and the reparse walk. The generic helpers pass any `.phase1/tools|cache|packages` identity through `Physical` and read the file directly.

The reads are hash-checked, but `repository-layout.md:30` claims shared-root reads happen "only for their named purposes … and only after resolving links", and that is not true of these call sites.

**Fix:** Route the shared mappings through `IsSharedPackage` (or a general `IsShared`), or narrow the sentence in `repository-layout.md`.

### IN-06: Known gap "git missing" understated — the impact is not limited to Node

**File:** `src/godot-game/tools/workspace-paths.cjs:33-42`; `src/godot-game/tests/verification/WorkspacePaths.cs:188`
**Issue:** With no `git` on `PATH`, a linked worktree silently becomes `standalone`. The impact goes beyond a Node install:

- `bootstrap.cjs` downloads and extracts Godot again (about 370 MB) into the worktree.
- NuGet restores into a private package folder.
- The C# twin does not fall back. `Process.Start("git")` throws `Win32Exception`, which is not caught, so direct `dotnet test` runs crash while the Node runners proceed.

**Fix:** When `<checkout>/.git` is a file (`gitdir:`), treat the checkout as a linked worktree and fail with `SHARED_ROOT_UNRESOLVED` instead of falling back. Catch `Win32Exception` in the C# code and apply the same rule.

### IN-07: `isolationEnv` now honours `PHASE1_PACKAGE_CACHE` for every runner

**File:** `src/godot-game/tools/workspace-paths.cjs:79`
**Issue:** Before this change, only `verification/check.cjs` read `PHASE1_PACKAGE_CACHE`. The gameplay-checks, lan-checks, asset-pipeline and C# paths used the fixed `.phase1/packages`. Now gameplay-checks, lan-checks and the asset-pipeline `isolated()`/`env()` helpers inherit it too, while bootstrap overrides it explicitly. This silent behaviour change is not documented in `setup.md`.

**Fix:** Document it, or keep the override in `verification/check.cjs` only.

### IN-08: `asset-bindings.cjs --write` has no clean-tree guard

**File:** `src/godot-game/tools/verification/asset-bindings.cjs:130-148`
**Issue:** It rehashes every catalog module unconditionally. Unlike `ac08-bindings.cjs --write` (`refuseDirty`), it blesses any unrelated or accidental module edit in the working tree. The README says `--write` is "only for a deliberate rebind", but the tool does not enforce that.

**Fix:** Refuse when `git status --porcelain` lists anything other than the three cascade files and the modules the caller names. Alternatively, print the stale module list and require `--write --yes`.

### IN-09: `start-vm.cjs` builds QEMU `-drive` options from the shared path without escaping commas

**File:** `src/godot-game/tools/verification/linux-vm/start-vm.cjs:52-54`
**Issue:** The VM path now depends on the main checkout's location or on `KZ_SHARED_LOCAL`. A comma in that path splits the `file=` value into extra QEMU options.

**Fix:** Escape with `.replaceAll(',', ',,')` for each `file=` value, or refuse paths that contain a comma.

### IN-10: `legacyTarget` duplicates the legacy-map lookup and ignores each row's scope

**File:** `src/godot-game/tools/workspace-migrate.cjs:84-90`
**Issue:** It re-implements `legacyRow` and always targets `<main>/.local`, ignoring each row's `shared` or `local` scope. This is correct today only because the migration refuses any override (shared equals local in the main checkout). A future relaxation of that refusal would silently put shared caches in the wrong root.

**Fix:** Export `legacyRow` from `workspace-paths.cjs` and use `workspace.legacyPath(identity, main)`, with the override refusal kept as the guard.

## Fix status

The fixes are 16 commits on top of `19f5818`, ending at `cfbbb5f`, with ac-08 regenerated last. Every gate passes at `cfbbb5f`: history, newhost (stricter), residue (every `--allow` entry counted), lists 686/16/32/75, tests 120 in 16 files, roots, build (0/0, no network GET), assets 423, ac08 and docs 36/36. The SUMMARY's "Review fixes" table names the tests behind each fix; each test failed on the pre-fix code.

| Finding | Status | Commit |
| --- | --- | --- |
| WR-01 | Fixed: planning verifies sources, targets and refusals before anything changes; moves, then deletes, the VM last; reruns continue | `d207354` |
| WR-02 | Fixed: shared `linkedComponent` helper; migration and cleanup refuse links in any component | `8b77f1c` |
| WR-03, WR-04 | Fixed: one rule in JS, C#, `shared-root.sh` and `shared-root.ps1`; MSYS and root-relative forms refused, not converted | `5bec726` |
| WR-05 | Fixed: `git check-ignore` before the first write; `SHARED_ROOT_NOT_IGNORED`; documented info/exclude line, never auto-edited | `b094936` |
| WR-06 | Fixed: members under any `.local` refused; trace refuses the resolved shared root's `tools`, `downloads` and `nuget` whatever its name | `78c020e` |
| WR-07 | Fixed: residue refuses an `--allow` entry without `=N` | `d3b00c8` |
| WR-08 | Fixed in docs; the executable catalog (`acceptancePhase: 5`), register and rule matrix stay unchanged, as PR #11 leaves them to the Phase 5 implementation. The gap is recorded in the SUMMARY and the STATE `[Phase 11]` concern | `3d06ede` |
| WR-09 | Fixed: both setup scripts recheck after the lock and refuse to move onto an install | `7cc3454` |
| IN-01 | Fixed: `--apply` holds `toolchain.lock` | `b0ee77b` |
| IN-02 | Fixed: both Codex commands exit 0 on any failure | `1987c8c` |
| IN-03 | Fixed: `/home/<name>/` and UNC shares; 0 new of 1814 on BASE..HEAD | `fbfc383` |
| IN-04 | Fixed through the WR-02 helper (ancestor links below the shared root) plus `nlink === 1`, rather than left as acknowledged | `69b76fa` |
| IN-05 | Acknowledged, not fixed: outside the requested fix scope. The reads stay hash-checked; the `repository-layout.md:30` wording overstates containment | - |
| IN-06 | Fixed: `GIT_MISSING` in the resolver, both setup scripts and C# | `d65cd11` |
| IN-07 | Acknowledged, not fixed: outside the requested fix scope; behaviour unchanged and undocumented | - |
| IN-08 | Fixed: `asset-bindings --write` refuses a dirty tree like ac-08 | `3b479ab` |
| IN-09 | Fixed: commas doubled in `-drive file=` values | `dc9960e` |
| IN-10 | Acknowledged, not fixed: outside the requested fix scope; correct while the migration refuses any override | - |

---

**Ignored outputs created by this review:** none in the task worktree. `.local/temp/review/` was not needed: the probes only ran `workspace-paths.cjs --print-roots` and a read-only `insideShared` script, which create no folders. The probe script lives in the reviewer's session scratchpad, outside the repository.

_Reviewed: 2026-11-23T17:50:53Z_
_Reviewer: Claude (gsd-code-reviewer)_
_Depth: standard_
