---
phase: quick
plan: 261122-ub4
subsystem: workspace-cleanup
tags: [cleanup, merge, docs, data-loss]
status: complete
requires:
  - origin/dev 1a14684 (Phase 4 squash merge)
  - PR 7 head f373400
provides:
  - PR branch merged with origin/dev through a two-parent merge commit
  - Cleanup command that keeps nested repositories, worktrees, unscannable folders and both runner locks out of deletion
  - Cleanup and live workflow/content docs matching the command
affects:
  - tools/workspace-cleanup.cjs
  - docs/workflow/cleanup.md
  - docs/workflow/ai-collaboration.md
  - docs/workflow/verification.md
  - docs/workflow/foundation-checks.md
  - docs/workflow/lan-foundation-checks.md
  - docs/workflow/engine-defects.md
  - docs/content/production.md
tech-stack:
  added: []
  patterns:
    - "Explicit-stack directory scan that descends only into lstat-confirmed directories and keeps on any scan error"
key-files:
  created: []
  modified:
    - .gitignore (merge resolution)
    - .planning/STATE.md (merge resolution)
    - tools/workspace-cleanup.cjs
    - docs/workflow/cleanup.md
    - docs/workflow/ai-collaboration.md
    - docs/workflow/verification.md
    - docs/workflow/foundation-checks.md
    - docs/workflow/lan-foundation-checks.md
    - docs/workflow/engine-defects.md
    - docs/content/production.md
decisions:
  - "Merge conflict .gitignore: kept the PR's single /.phase[0-9]*/ row, dropped dev's four explicit rows"
  - "Merge conflict STATE.md: kept dev's file plus the 261119-15c row between 261114-we2 and 261120-09v"
  - "The requested-worktree late lock recheck uses a separate variable name (lateLock) so it does not shadow the root lock in execute"
metrics:
  duration: "about 5 min"
  completed: 2026-11-22
estimate:
  tokens: 80000
  tasks: 3
actuals:
  tokens: 10200    # chars/4 over this task's own diff (merge resolution + script + docs), 40,948 chars
  tasks: 3
  commits: 4       # MEASURED rev-list count; 3 authored here plus dev's 1a14684 brought in by the merge (first-parent count is 3)
plan_head_before: f3734002f1c0d0c6d1b992aeb4feab950d6de6e9
plan_head_after: 5e00b8a247ddfdb3a4702c7af0b4e1274f3caa7e
---

# Quick Task 261122-ub4: Prepare PR 7 for merge Summary

PR 7 now contains origin/dev through one two-parent merge commit. The cleanup command now keeps nested repositories and worktrees at any depth, keeps folders it cannot scan, honors both runner locks, refuses skip-worktree flags, and reads up to 256 MiB of Git output. The cleanup and live workflow/content docs now say the same thing as the command.

**All verification was by reading only.** The changed script was not run, its syntax was not checked (`node --check` was not run), and the 14 existing tests in `tools/workspace-cleanup.test.cjs` were not rerun on it. No test, build, checker case, probe, fixture or cleanup command was run, as the creator decided. Node was not used.

## Commits

| Task | Commit | Subject |
| --- | --- | --- |
| 1 | `24df729` | Merge origin/dev into feature/261119-15c-agent-cleanup (parents f373400, 1a14684) |
| 2 | `76ac5aa` | fix(quick-261122-ub4): keep nested repositories and runner locks out of cleanup |
| 3 | `5e00b8a` | docs(quick-261122-ub4): align cleanup and workflow docs with the cleanup command |

Nothing was pushed or fetched. The branch is ahead of `origin/feature/261119-15c-agent-cleanup` by 4 commits: the three above plus dev's `1a14684`, which the merge brought in.

## Task 1: Merge

- Preconditions held: HEAD was f3734002, origin/dev was 1a146845, and the tracked tree was clean.
- `git merge --no-ff --no-commit origin/dev`. The live list of unmerged files was exactly `.gitignore` and `.planning/STATE.md`, as predicted. `AGENTS.md` and `docs/workflow/verification.md` merged without conflicts.
- `.gitignore`: kept `/.phase[0-9]*/` and dropped dev's `/.phase1/` to `/.phase4/` rows. The file has no diff3 base section.
- `.planning/STATE.md`: rows are ordered 261114-we2, 261119-15c, 261120-09v, and each matches its side byte for byte. Nothing else changed, so the stale "after PR #5 merges" sentence is still there.
- **Gate: PASS.** The commit has two parents. No unmerged paths or conflict markers remain. Numstat against dev is `1 4` for .gitignore and `1 0` for STATE.md. The rows are in the right order. The tree differs from dev in 14 files. AGENTS.md has the engine-defects row and two cleanup links. The PR's AGENTS.md and verification.md edits apply to dev's text unchanged. The quick-task folder was not committed.

## Task 2: Cleanup script (`tools/workspace-cleanup.cjs`)

- A. `git()` gets `maxBuffer: 256 * 1024 * 1024`.
- B. New constants: `RUNNER_LOCKS` (`.phase1/check.lock`, `.phase2/desktop.lock`) and `NESTED_REASON` (byte-identical to the string test 8 expects).
- C. `looksLikeRepository(names)` matches a `.git` entry of any type, or `HEAD` plus `objects` plus `refs` (bare layout), ignoring case.
- D. `keepReason(target, worktreePaths, {ownGit})` checks for a registered worktree at or inside the target, then lstats the target (non-directories are not scanned), then walks an explicit stack using `readdirSync({withFileTypes})`. It descends only into directories that lstat confirms. Any readdir or lstat error returns `could not be scanned (<code>)[ at <rel>]`. With `ownGit`, it ignores only the target's own top-level `.git`.
- E. `findLock(dir)` runs lstat on each lock path. ENOENT and ENOTDIR count as absent, a dangling link counts as a lock, and any other error is thrown.
- F (loss 2). `classifyRoot` keeps the whole root when it is a registered worktree or a repository. Otherwise each non-durable child goes through `keepReason`. The old shallow `.git` check is gone, and durable caches are never scanned.
- G (loss 3). `buildOutputs(root, worktreePaths)` passes each `bin`/`obj`/`.godot` folder through `keepReason`.
- H (losses 1, 4, 5). `worktreeState` uses `ls-files -v -z` with `/^[a-zS] /`. After the remote-branch check, it refuses `runner lock at <rel>`. Last, it runs `keepReason(..., others, {ownGit: true})`. `planWorktrees` passes every worktree path.
- I (losses 5, 6). In the `requested` mapping, a worktree that is still removable gets a second `findLock` check after the scans. The root uses `findLock(root)` with the new message. The refusal block stays above the deletion loop.
- J. `parseArgs`, `parseWorktrees`, `removeInside`, `removeWorktree`, `run`, USAGE and exports are unchanged.
- **Gate: PASS.** All 8 positive literals are present and both negative literals are gone. `keepReason(`, `findLock(` and `ownGit` each appear 4 times in non-comment lines, against required minimums of 4, 4 and 2. The awk order check passes: the `findLock(root)` line and the `refused.length)` line both come before `removeInside(root, entry.path)`. The test file is byte-identical to f373400, and the commit touches only the script.
- I read the RESEARCH section 3 test list against the new code. Test 8: `.phase4/wt` equals a registered worktree, so its reason is exactly `NESTED_REASON`. Test 10: lstat reports the junction as not a directory, so the scan does not enter it, and both lock paths return ENOENT. Test 12: the root lock still makes `--apply` exit 1 before any deletion. I found no conflicts in the others. **This is inferred from reading, not executed.**

## Task 3: Docs

- `docs/workflow/cleanup.md`: new "Known exceptions" paragraph (rows A-D); a keep/scan paragraph under the table that also points to the setup rerun; removal deletes caches that are not links; the full refusal list; the named-removal scope sentence; both locks; and a note that the 14 tests passed at f373400 and have not been rerun on the current script.
- `ai-collaboration.md`: reports stay immutable until task-end cleanup, and reuse in a later task needs durable evidence.
- `verification.md`: lines 5 and 51 say "retained" → "recorded"; line 13 adds the task-end limit; line 45 says "never rewrite historical records".
- `foundation-checks.md`: runs are deleted at task end, so run repair, completion and validation commands before cleanup.
- `lan-foundation-checks.md`: the `--verify-completion` row cell, and failed reports are recorded before cleanup.
- `docs/content/production.md`: run folders are deleted at task end.
- `engine-defects.md`: rewrote the raw-traces bullet.
- **Gate: PASS** on all 25 literals and the exact list of seven files. My first run of this gate typed the expected file list wrong (an executor typo, not a repository problem). Rerunning the plan's exact gate passed.

## Overall verification (read-only)

- `git log` shows docs, script, merge, then 1a14684/f373400 as the merge parents.
- `git status --porcelain --untracked-files=all` lists only `.planning/quick/261122-ub4-prepare-for-merge-pr-7/` entries.
- `git diff --name-only origin/dev HEAD | wc -l` = 18.
- `git diff --quiet origin/dev HEAD -- tools/phase1 tools/phase2 tools/phase3 tools/phase4 tests src production docs/code .planning/phases .planning/debug` exits 0.
- `git diff --quiet f373400 HEAD -- tools/workspace-cleanup.test.cjs` exits 0.
- No commit deleted a file.

## Deviations from Plan

None. The plan was executed as written. One small implementation choice: the late lock recheck in the `requested` mapping uses a variable named `lateLock` so it does not shadow the root `lock` binding in `execute`.

## Follow-ups (for the PR description and the orchestrator)

1. **Catalog rows and digests (RESEARCH section 6, not edited here).** `tools/phase1/required-cases.json` needs three new `outside-validator` rows (`owners: []`, `sha256: null`):
   - `docs/workflow/cleanup.md`, between ai-collaboration and engine-defects
   - `tools/workspace-cleanup.cjs`, after `tools/phase4/scenarios.cjs`
   - `tools/workspace-cleanup.test.cjs`, last

   Digests to refresh:
   - `sourceSetSha256` at `required-cases.json:5284` (currently `8038db86...c0d1`)
   - `repositorySha256` (currently `69a44bbd...3a5f`) at `required-cases.json` :8676, :11920, :16335, :16453 and :16606
   - `repositorySha256` at `tools/phase2/required-cases.json:43`

   Until then `ac-08.complete`, `R-50.positive` and `rules.exemptions` fail on the merged tree.
2. **Checks that fail after cleanup (RESEARCH section 7, rows A-D):**
   - A (**new**; the creator's no-exception decision did not know about it): run `.phase4/runs/81c4c6a6-8981-43b7-ae89-0d557dd8a54c`. Without it, Phase 4 `export.layout` and Phase 1 `rules.export-layout-origin` / `rules.export-layout-boundary` fail.
   - B: `.phase4/checkpoints/04-02-entry-93386745`. Without it, `export.profile` fails.
   - C: `.phase4/04-23-current-model/normalized-index.json`. Without it, 16 `ContentNativePermissionTests` model methods fail; no cataloged case selects them.
   - D: four `.phase2/runs/win32-x64/<id>/summary.json`. Without them, `node tools/phase2/check.cjs --verify-completion .planning/phases/02-playable-foundation/02-PLATFORM-EVIDENCE.json` fails.

   With section 6, Phase 1 `--phase 4` loses five cases.
3. **Phase 4 review source identity.** The merged docs and AGENTS edits change Phase 4's source identity (`tools/phase4/evidence.cjs:26-27,125-129`), so its saved review cannot validate on the merged tree. In addition, `runner.prelaunch`/`runner.selection` already fail on dev because `5c637da` is not an ancestor of the squash commit `1a14684`.
4. **Kept `.phase3/revision-control-*` repositories.** `tools/phase3/revisions.test.cjs:7,11` creates repositories there and never removes them, so cleanup will always keep and report them (or the whole child that contains them). Do not bypass this. Delete them by hand only after inspection.
5. **Stale STATE.md sentence.** `.planning/STATE.md` still says "after PR #5 merges". PR #5 is merged as `1a14684`. The orchestrator owns STATE.md.
6. **Wording left out of scope.** `docs/code/review.md` and `docs/workflow/setup.md` still say "retained ... baseline". They were outside this task's authorized files.
7. **Unproven behavior.** None of the 14 existing tests covers the six fixes or the buffer, and the changed script has not been executed. The first real dry run should be read carefully before `--apply`.

## How to reproduce

From the PR worktree root (or any checkout of `feature/261119-15c-agent-cleanup` at `5e00b8a`):

```sh
# Merge shape and conflict resolution
git log -1 --format=%P 24df729            # f373400... 1a14684...
git diff --numstat origin/dev 24df729 -- .gitignore .planning/STATE.md   # 1 4 / 1 0
grep -E '^\| 2611' .planning/STATE.md | cut -d'|' -f2                    # 261114-we2, 261119-15c, 261120-09v
git diff --name-only origin/dev HEAD | wc -l                              # 18

# Script changes (read-only)
git show 76ac5aa -- tools/workspace-cleanup.cjs
git diff --quiet f3734002f1c0d0c6d1b992aeb4feab950d6de6e9 HEAD -- tools/workspace-cleanup.test.cjs && echo test-file-unchanged

# Doc changes
git show 5e00b8a --stat

# Not run in this task (creator decision); run these to obtain execution evidence:
node --check tools/workspace-cleanup.cjs
node --test tools/workspace-cleanup.test.cjs     # 14 tests; none covers the new fixes
node tools/workspace-cleanup.cjs                 # dry run; inspect keep/delete reasons before --apply
```

## Self-Check: PASSED

- FOUND: tools/workspace-cleanup.cjs, docs/workflow/cleanup.md (and the six other docs)
- FOUND: 24df729, 76ac5aa, 5e00b8a (all ancestors of HEAD)

## Orchestrator addendum: review, artifact deletion and publication

**Code review** ([261122-ub4-REVIEW.md](261122-ub4-REVIEW.md), read-only): 1 blocker and 9 warnings.
- Fixed in `6421770`, by reading only:
  - **CR-01:** worktrees whose untracked files are hidden by `core.excludesFile` or `info/exclude` are now refused.
  - **WR-06:** a dry run with a runner lock now says `--apply` would delete nothing.
  - **WR-04:** the prune message and docs now state that every unlocked missing record is pruned, including other sessions'.
  - **WR-05:** the known-exceptions list adds the Phase 3 and Phase 4 checks that need deleted runs.
- Still open:
  - WR-01: no lock is taken during deletion.
  - WR-02: the late lock check comes before the slow deletion loop.
  - WR-03: commits reachable only from a worktree's reflog or per-worktree refs.
  - WR-07: one nested repository keeps its whole top-level folder, so `.phase3/revision-control-*` stay.
  - WR-08: a broken worktree stops the whole run.
  - WR-09: `platform-proof.md` and `docs/code/review.md` wording.

**Verification** ([261122-ub4-VERIFICATION.md](261122-ub4-VERIFICATION.md)): `human_needed`. 5 of 9 must-haves were verified by reading; the 4 behavioral truths need someone to run the script.

**Artifact deletion (creator-confirmed, irreversible):** on 2026-11-22, from 21:55:38Z to 21:59:35Z, the orchestrator deleted the 944 listed top-level entries in the main checkout's `.phase1`–`.phase4`: about 98.6 GB in 1.06 million files. It used an exact-manifest script, not the cleanup command.
- Result: 944 deleted, 0 skipped, 0 failed, nothing new unlisted.
- Kept: the seven `.phase1` caches (`cache`, `cli-home`, `dotnet-home`, `feed`, `http-cache`, `packages`, `tools`), about 11 GB, plus tracked files and every other worktree.
- The creator confirmed after seeing the consequences. These checks lose their inputs:
  - `export.profile`;
  - `export.layout` and `rules.export-layout-origin`/`-boundary` (run `81c4c6a6-…`);
  - 16 `ContentNativePermissionTests` model tests;
  - Phase 2 `--verify-completion`.
- Also lost: the raw local runs cited by tracked records, and 33 Phase 3 test repositories.
- Session `game-kingdom-1` (PR #6) confirmed beforehand that it no longer needed anything in the set; it had copied its inputs into its own worktree.

**Not run, by the creator's decision:** `node --check`, the 14 existing tests, any dry run of the changed script, and every checker case.
