---
phase: 261122-ub4-prepare-for-merge-pr-7
reviewed: 2026-11-22T22:07:55Z
depth: quick
files_reviewed: 8
files_reviewed_list:
  - tools/workspace-cleanup.cjs
  - docs/workflow/cleanup.md
  - docs/workflow/ai-collaboration.md
  - docs/workflow/verification.md
  - docs/workflow/foundation-checks.md
  - docs/workflow/lan-foundation-checks.md
  - docs/workflow/engine-defects.md
  - docs/content/production.md
findings:
  critical: 1
  warning: 9
  info: 0
  total: 10
status: issues_found
---

# Quick Task 261122-ub4: Code Review Report

**Reviewed:** 2026-11-22T22:07:55Z
**Depth:** quick (a careful read of the full script and the full `24df729..5e00b8a` diff; necessity was not assessed at this depth)
**Files Reviewed:** 8
**Status:** issues_found

## Summary

Scope: commits 76ac5aa and 5e00b8a (`git diff 24df729 5e00b8a`), with the full files for context, on `feature/261119-15c-agent-cleanup`.

**Evidence basis.** Following the creator's no-testing decision, every finding below comes from reading only. I did not run the script, tests, node, git probes or fixtures. Nothing in the repository was changed except this file. I did not read inside any `.phase1`-`.phase4` folder. Each finding states how confident I am. Statements about Git and Node behaviour come from their documented semantics, not from observation.

**What the six fixes now do, by reading.** All six reproduced losses from CONTEXT look closed as written:

- `-z` with `/^[a-zS] /` catches `S`, `s` and lowercase flags.
- `keepReason` walks only directories confirmed with `lstat`, at any depth, and treats a scan error as "keep".
- `--build` entries go through `keepReason`.
- A worktree scan skips only its own top-level `.git`.
- `findLock` checks both fixed lock paths in the target worktree and in the root.

All new checks run before the first mutation (`execute` line 294 returns before line 298). Reading confirms:

- I found no path that throws past `run()`'s catch. Every Git or filesystem failure becomes `cleanup stopped` with exit 1.
- No new uncaught crash was introduced.
- `removeInside` still refuses non-relative targets.

**Remaining concerns.** These are the main paths by which the command can still delete work that is not a task output:

- The worktree "clean" test honours `core.excludesFile` and the shared `info/exclude` (CR-01). The docs claim the opposite.
- Nothing holds the runner locks during deletion (WR-01, WR-02).
- Commits held only by a worktree's reflog or per-worktree refs are deleted (WR-03).
- Every `--apply` prunes other sessions' missing, unlocked worktrees (WR-04).

On the documentation side, the "Known exceptions" list is incomplete, and one LAN doc row prescribes a command that can no longer pass (WR-05). `platform-proof.md` still presents local copies as present (WR-09). Two places can report success or a false outcome: the unconditional "pruned" message (WR-04) and a dry run that says outputs "would be deleted" while a lock forbids it (WR-06).

## Critical Issues

### CR-01: BLOCKER. A worktree counts as "clean" even when its untracked work is hidden by the user's global ignore file or the repository's shared `info/exclude`. Removal then deletes that work, and the docs explicitly claim it is protected

**File:** `tools/workspace-cleanup.cjs:205-206`, `:242-245`; `docs/workflow/cleanup.md:71`
**Classification:** BLOCKER (data loss of non-task work; false doc guarantee)
**Established by:** reading. **Confidence:** high on the mechanism, medium on how often it happens.
**Issue:** `git status --porcelain=v1 --untracked-files=all --ignore-submodules=none` overrides only `status.showUntrackedFiles` and submodule settings. Status still applies the standard exclude sources:

- `.gitignore`;
- `$GIT_COMMON_DIR/info/exclude`, which every linked worktree shares;
- `core.excludesFile`, which defaults to `$XDG_CONFIG_HOME/git/ignore` or `~/.config/git/ignore`.

Status prints nothing for a file matched by a personal or shared exclude pattern (for example `*.local`, `notes/`, `.env`, `scratch/`, `.vscode/`, `*.orig`). The worktree is reported `removable (clean and pushed)`. `git worktree remove` without `--force` deletes ignored files by design. The `fs.rmSync` fallback at :245 then removes anything left.

None of these files is a task output under the repository's own rules. Yet `cleanup.md:71` says a worktree is clean "only with ... no untracked files (whatever the Git configuration hides)". The global excludes file is exactly Git configuration that hides untracked files. The same applies to repository-ignored state that the "What to delete and keep" table (`cleanup.md:35`) says to keep, such as `.planning/state.json` and `.gsd/`. A worktree removal deletes these silently. The only nearby statement, "deletes everything ignored inside it" (`cleanup.md:44`), reads as referring to the repository's ignore rules.
**Fix:** Decide what counts as untracked using only the repository's `.gitignore` files. Refuse when anything else is present:
```js
// Untracked paths judged only by tracked .gitignore files (no info/exclude, no core.excludesFile).
const hidden = git(worktree.path, ['ls-files', '-z', '--others', '--exclude-per-directory=.gitignore', '--directory', '--no-empty-directory'])
  .split('\0').filter(Boolean);
if (hidden.length) return {removable: false, reason: `untracked files hidden by local exclude rules (${hidden[0]}...)`};
```
Also refuse when `.gsd/` or `.planning/state.json` exists in the target, or list them in the doc as deleted. If the code is not changed, at minimum change `cleanup.md:71` to say that files ignored by personal or shared exclude rules are deleted.

## Warnings

### WR-01: WARNING. The runner lock is checked once, but cleanup never takes it, so a runner started during a minutes-long deletion loses its live outputs

**File:** `tools/workspace-cleanup.cjs:287-291`, `:298-311`; `docs/workflow/cleanup.md:73`
**Established by:** reading (`tools/phase1/check.cjs:712-716`, `tools/phase2/check.cjs:18-21` take the locks with `openSync(..., 'wx')`). **Confidence:** high.
**Issue:** `findLock(root)` runs once, then the loop runs `rmSync` over children such as `.phase4/runs` (the 74 GB case) and `.phase1/runs` / `.phase1/temp`. Cleanup does not create either lock itself.

So a Phase 1 check or a native Phase 2/3/4 run can start mid-loop:

1. It acquires the lock and writes into `.phase1/runs/<nonce>` or `.phase1/temp`, or into `.phase2/...`.
2. The loop later reaches those planned children and deletes the live run's directory.

The doc sentence "`--apply` deletes nothing while this checkout has `.phase1/check.lock` or `.phase2/desktop.lock`" is only true at the moment of the check.
**Fix:** Before the first deletion, take both locks exclusively with the runners' own protocol. Release them in `finally`. Leave a lock in place if cleanup crashes, so the next runner is told to inspect it:
```js
const held = RUNNER_LOCKS.map(rel => { const file = path.join(root, rel);
  fs.mkdirSync(path.dirname(file), {recursive: true});
  const fd = fs.openSync(file, 'wx'); fs.writeFileSync(fd, JSON.stringify({pid: process.pid, owner: 'workspace-cleanup'})); fs.closeSync(fd); return file; });
try { /* deletion loop, prune, removals */ } finally { for (const file of held) fs.rmSync(file, {force: true}); }
```
`'wx'` failing with `EEXIST` then replaces the separate `findLock(root)` race window.

### WR-02: WARNING. The "late lock" recheck for requested worktrees happens before the slowest step, not immediately before removal

**File:** `tools/workspace-cleanup.cjs:276-283` (recheck), `:298-311` (artifact deletion), `:323-333` (removal)
**Established by:** reading. **Confidence:** high.
**Issue:** The comment justifies the recheck because "the scans above can take minutes". Yet the recheck runs before the artifact-deletion loop, and `removeWorktree` runs after it. Without `--worktrees-only`, that loop can run for many minutes. A runner, a new nested clone or new ignored output can appear in the target worktree during that time, and `git worktree remove` plus the `rmSync` fallback then delete it. Git's own pre-removal check covers only tracked and untracked changes, filtered by the CR-01 excludes. It does not look for repository locks under `.phase*`, nested repositories in ignored folders or the runners' lock files. The nested-repository scan (`keepReason`, :220) is also not repeated.
**Fix:** Move the `findLock(match.path)` and `keepReason(match.path, others, {ownGit: true})` recheck into the removal loop, directly before `removeWorktree(root, worktree.path)`, and count a hit as a refusal (failure). Better still, combine this with WR-01 by taking the target worktree's locks with `'wx'` before calling `git worktree remove`.

### WR-03: WARNING. "Unpushed commits" is checked only at HEAD, so commits held only by the worktree's reflog or per-worktree refs are dropped with its admin folder

**File:** `tools/workspace-cleanup.cjs:214-216`; `docs/workflow/cleanup.md:45`
**Established by:** reading (Git stores `logs/HEAD`, `refs/worktree/*` and `refs/bisect/*` per worktree in `$GIT_COMMON_DIR/worktrees/<id>/`, which `git worktree remove` deletes). **Confidence:** medium-high.
**Issue:** Consider a detached-HEAD worktree where an agent made commits and then checked out a pushed commit, for example `git checkout origin/dev`. It passes `git branch -r --contains <HEAD>`. Its earlier commits are reachable only from that worktree's `logs/HEAD` reflog, or from `refs/worktree/*` and `refs/bisect/*`. Removal deletes these, so the commits become unreachable and `git gc` later discards them. The previous review's IN-01 raised this and it is still open. The doc line "The command refuses worktrees with ... unpushed commits" is therefore not fully true.
**Fix:** Refuse when any commit named in the worktree's HEAD reflog or per-worktree refs is not reachable from a remote-tracking ref:
```js
const named = git(worktree.path, ['rev-list', '--walk-reflogs', '--no-walk', 'HEAD']).split('\n').filter(Boolean);
const refs = git(worktree.path, ['for-each-ref', '--format=%(objectname)', 'refs/worktree', 'refs/bisect']).split('\n').filter(Boolean);
if (named.length + refs.length && git(root, ['rev-list', '-n1', ...new Set([...named, ...refs]), '--not', '--remotes']).trim())
  return {removable: false, reason: 'commits only in this worktree\'s reflog or per-worktree refs'};
```
(Or use `git reflog show --format=%H HEAD` run in the worktree.)

### WR-04: WARNING. Every `--apply` prunes all missing, unlocked worktree records, including other sessions' records, and reports "pruned" whether or not Git pruned anything

**File:** `tools/workspace-cleanup.cjs:204`, `:318-321`; `docs/workflow/cleanup.md:43,64,71`
**Established by:** reading. **Confidence:** high on behaviour, medium on how often it happens.
**Issue:**
- The documented task-end command `--apply --worktrees-only --remove-worktree <path>` (`cleanup.md:42`) runs `git worktree prune` whenever any worktree is labelled `prune`. Git deletes `$GIT_COMMON_DIR/worktrees/<id>/` for every unlocked worktree whose folder is unreachable, for example on an unplugged drive, an unmounted share or a temp path that is briefly inaccessible. That folder holds the other session's index, and with it its staged-but-uncommitted content, plus its HEAD reflog and per-worktree refs.
- This contradicts `cleanup.md:43`: "Remove only worktrees created for the task ... ask the creator before removing any other".
- It also contradicts `cleanup.md:71`, which says that apart from `--remove-worktree` the command never deletes outside the checkout. When the command runs from a linked worktree, the common Git folder is outside that checkout.
- The label comes from `fs.existsSync`, which returns `false` for EACCES and EPERM as well as for a missing path. Git decides pruning by its own checks. So `  pruned missing worktree records` (:320) is printed unconditionally, even when Git pruned nothing.
**Fix:** Prune only when the user asks for it, with a new `--prune` option, and never as a side effect of `--remove-worktree`. Report the result by comparing `git worktree list --porcelain` before and after. Label entries `missing` rather than `prune` when `existsSync` is false for any reason other than `ENOENT`:
```js
let missing; try { fs.lstatSync(worktree.path); missing = false; } catch (e) { if (e.code !== 'ENOENT') return {removable: false, reason: `could not be checked (${e.code})`}; missing = true; }
```

### WR-05: WARNING. The docs' "Known exceptions" list is incomplete, and a LAN doc row still prescribes a completion command that cannot pass

**File:** `docs/workflow/cleanup.md:19`; `docs/workflow/lan-foundation-checks.md:24`
**Established by:** reading (`.planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json` contains 3,329 `.phase` occurrences; `tools/phase4/scenarios.cjs:385-404` resolves review-build evidence under `.phase4/runs/<nonce>/`; RESEARCH section 7 "already fails" table). **Confidence:** medium-high.
**Issue:** `cleanup.md:19` lists five checks as "Known exceptions" that read ignored inputs, which reads as a complete list. It omits other checks that also dereference ignored `.phase1`-`.phase4` run receipts:

- Phase 3 `--verify-completion` of `03-EVIDENCE.json`;
- Phase 4 `runner.prelaunch`, `runner.selection`, the inherited and own rendered groups, `acceptance.artifacts` and Phase 4 `--verify-completion`.

These already fail on dev for other reasons. After cleanup they also lose their inputs, so fixing the ancestry or revision problem alone will not make them pass. `lan-foundation-checks.md:24` still lists `node tools/phase3/check.cjs --verify-completion .planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json` as the command to run "before cleanup". The same sentence says that record's local reports "are no longer kept". So the table offers a command that cannot succeed on any checkout.
**Fix:** In `cleanup.md:19`, add the Phase 3 completion check and the Phase 4 prelaunch, selection, rendered, acceptance and completion checks. Alternatively, say "including" and link RESEARCH section 7. In `lan-foundation-checks.md:24`, split the row: keep the command for a *new* completion record, and mark the saved `03-EVIDENCE.json` verification as historical and no longer reproducible.

### WR-06: WARNING. A dry run with a runner lock says outputs "would be deleted" and exits 0, although `--apply` would delete nothing and exit 1

**File:** `tools/workspace-cleanup.cjs:287-291`, `:335-337`; `docs/workflow/cleanup.md:73`
**Established by:** reading. **Confidence:** high.
**Issue:** Without `--apply`, a lock only prints an error line. The run then continues, lists every child including `.phase1/check.lock` and `.phase2/desktop.lock` themselves as `delete`, prints `Dry run: N output(s) would be deleted` and returns 0 unless a requested worktree was refused. The doc says "The command exits with 1 when it refuses". An agent that gates `--apply` on a zero-exit dry run gets a false green, and the summary contradicts the lock message just above it.
**Fix:** When `lock` is set, mark the plan blocked. Print `Dry run: blocked by <lock>; --apply would delete nothing`, show lock files as `keep (runner lock)`, and return 1 in both modes.

### WR-07: WARNING. A repository anywhere below a top-level child keeps that whole child, so one test fixture can silently stop cleanup of `.phase4/runs` or `.phase1/temp`

**File:** `tools/workspace-cleanup.cjs:149-155`; `docs/workflow/cleanup.md:38`
**Established by:** reading (`classifyRoot` classifies only direct children of each phase root; `keepReason` returns on the first hit at any depth). **Confidence:** high.
**Issue:** Any `.git` entry, or the `HEAD` + `objects` + `refs` triple, deep inside a single run keeps the entire `.phase4/runs` folder, which holds every run. A frozen validation checkout under `.phase1/temp` keeps all of `.phase1/temp`; `docs/workflow/platform-proof.md:65,84` names one at `.phase1/temp/17-windows-59f032c`. The line is reported as `keep`, so this is not a false success. Still, cleanup's main purpose (reclaiming the 74 GB) fails, exit status 0 gives no hint of it, and the doc's "it recreates `.phase1/temp`" (`cleanup.md:38`) is then moot. SUMMARY follow-up 4 covers only `.phase3/revision-control-*`.
**Fix:** When `keepReason` finds a hit below a child, classify that child's own children recursively, keeping only the path segments that lead to the hit. Or at least print a closing count of bytes or folders kept for repository reasons, so the shortfall is visible.

### WR-08: WARNING. One broken or unborn linked worktree that the user did not request blocks all cleanup

**File:** `tools/workspace-cleanup.cjs:205-217`, `:226-230`, `:272-273`
**Established by:** reading. **Confidence:** medium-high.
**Issue:** `planWorktrees` runs `status`, `ls-files`, `rev-parse`, `branch -r --contains` and `findLock` for every linked worktree before anything else, and none of these is wrapped. Some failures throw:

- A worktree on an unborn branch has porcelain `HEAD 0000000000000000000000000000000000000000`, so `git branch -r --contains 000…` fails as a malformed object name.
- A worktree with a corrupt index makes `status` or `ls-files` fail.
- A worktree folder that cannot be read makes `findLock` throw EACCES or EPERM.

Any of these aborts the whole command with `cleanup stopped` (exit 1). The checkout's own outputs are then never cleaned, even with `--worktrees-only` absent and no worktree requested. This fails safe but is the still-open part of the previous review's IN-02.
**Fix:** Wrap the body of `worktreeState` after the `prune` check in try/catch, and return `{removable: false, reason: \`git failed: ${firstLine(stderr)}\`}` for that worktree only.

### WR-09: WARNING. Live docs this task did not touch still state facts that cleanup now makes false

**File:** `docs/workflow/platform-proof.md:15,27,59,84`; `docs/code/review.md:3`; `docs/workflow/setup.md` ("retained ... baseline")
**Established by:** reading. **Confidence:** high that the text is as quoted. Whether the folders survive depends on WR-07, which I could not check without reading `.phase1`.
**Issue:**
- `platform-proof.md` presents the following local copies as present ("Its original report is", "copied/extracted under", "`.phase1/temp/17-linux-smoke/amendment-inspection.json` independently verifies"):
  - `.phase1/runs/66a74d57-…/summary.json`
  - `.phase1/temp/17-linux-smoke/…`
  - `.phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-…/summary.json`
- RESEARCH section 4 asked for a one-line historical-record note there. It was not added, and SUMMARY's follow-ups do not mention `platform-proof.md`.
- `verification.md:5` now says the "recorded" baseline, while `docs/code/review.md:3` still says "the retained complete Windows baseline" in the same sentence. SUMMARY follow-up 6 lists this mismatch.
**Fix:** Add the RESEARCH wording after the `platform-proof.md` title: "This is a historical record. The Windows `.phase1/temp` and `.phase1/runs` copies it names are removed by cleanup; the recorded results and commands remain." Also add `platform-proof.md` to the follow-up list in the PR description. Change "retained" to "recorded" in `review.md:3` and `setup.md` in the follow-up that owns those files.

---

_Reviewed: 2026-11-22T22:07:55Z_
_Reviewer: Claude (gsd-code-reviewer)_
_Depth: quick (careful read; no execution per the creator's decision)_
