---
phase: quick-261122-ub4
verified: 2026-11-22T23:30:00Z
status: human_needed
score: 5/9 must-haves verified
# covered_files / covered_digest omitted: verification.fingerprint needs node, which the creator's no-execution rule forbids in this task. They are not hand-written.
behavior_unverified: 4
overrides_applied: 0
behavior_unverified_items:
  - truth: "Removing a worktree is refused when git ls-files -v reports any lowercase tag or the S tag (skip-worktree, assume-unchanged or both)."
    test: "In a scratch repository, create a pushed linked worktree, run `git update-index --skip-worktree <file>` (and separately `--assume-unchanged`, and both), then dry-run `node tools/workspace-cleanup.cjs --remove-worktree <path>`."
    expected: "Each case is refused with `files marked assume-unchanged or skip-worktree`; nothing is removed with --apply."
    why_human: "The `/^[a-zS] /` test over `ls-files -v -z` is present (tools/workspace-cleanup.cjs:207), but no test exercises it and the script was not run."
  - truth: "A Git repository (.git entry of any type or bare HEAD/objects/refs layout) or registered worktree at any depth inside a phase-output child or a --build folder is kept and reported; a phase root that is itself one is kept whole; a folder that cannot be scanned is kept."
    test: "Dry-run the command with a repository at `.phase4/a/b/repo/.git`, a bare layout at `.phase4/c/d/{HEAD,objects,refs}`, a repository inside `src/x/bin/` with --build, `.phase3` itself a repository, and an unreadable folder under `.phase2/`."
    expected: "Each is listed `keep` with `contains a Git worktree or repository at <rel>` or `could not be scanned (<code>)`; the whole `.phase3` root is kept; --apply leaves all of them in place."
    why_human: "keepReason (lines 88-123) walks the tree on lstat results, including Windows reparse-point behavior; correctness is a runtime property no test covers."
  - truth: "Removing a worktree is refused when another worktree or repository is nested anywhere in it, when it cannot be scanned, or when it has .phase1/check.lock or .phase2/desktop.lock, and the lock is checked again after all scans."
    test: "Create pushed linked worktrees containing (a) a nested repository in an ignored folder, (b) another registered worktree, (c) `.phase1/check.lock`, (d) `.phase2/desktop.lock`; run --remove-worktree on each with --apply."
    expected: "Each is refused with the nested or `runner lock at ...` reason, exit 1, nothing deleted; a lock created after planning is caught by the late recheck (lines 276-283)."
    why_human: "Presence and order are readable, but refusal and the late recheck are runtime behavior with no test."
  - truth: "--apply deletes nothing while the checkout has .phase1/check.lock or .phase2/desktop.lock, and every refusal is decided before the first deletion, prune or removal."
    test: "Create `.phase2/desktop.lock` (and separately `.phase1/check.lock`) in a scratch checkout with deletable `.phase4/runs/r.txt`, run --apply."
    expected: "Exit 1, the lock message names the lock, `.phase4/runs/r.txt` still exists."
    why_human: "findLock(root) at line 287 and the refusal block at 292-297 sit above the deletion loop (298), prune (318) and removal (323) by reading; only the existing test 12 covers the .phase1 case, and it was not rerun."
human_verification:
  - test: "node --check tools/workspace-cleanup.cjs"
    expected: "No syntax error."
    why_human: "The changed script has never been parsed by Node; the creator forbade running node in this task. A full read found no syntax problem."
  - test: "node --test tools/workspace-cleanup.test.cjs"
    expected: "14 of 14 pass on 5e00b8a (they passed at f373400 only)."
    why_human: "The tests were not rerun on the changed script; RESEARCH section 3 and this verifier's reading infer they still pass (test 8 reason string, test 10 junction/ENOENT, test 12 lock order)."
  - test: "Skip-worktree / assume-unchanged refusal (loss 1)"
    expected: "See behavior_unverified_items[0]."
    why_human: "Runtime behavior, no test."
  - test: "Nested repository, bare layout, --build folder, phase-root repository and unscannable folder are kept (losses 2, 3)"
    expected: "See behavior_unverified_items[1]."
    why_human: "Runtime behavior, no test."
  - test: "Worktree removal refused for nested repo/worktree, scan error and both runner locks, with late lock recheck (losses 4, 5)"
    expected: "See behavior_unverified_items[2]."
    why_human: "Runtime behavior, no test."
  - test: "Root .phase2/desktop.lock blocks --apply (loss 6)"
    expected: "See behavior_unverified_items[3]."
    why_human: "Runtime behavior, no test."
  - test: "First real dry run on the main checkout before --apply"
    expected: "Keep/delete reasons match docs/workflow/cleanup.md; durable .phase1 caches kept; .phase3/revision-control-* repositories (or their containing child) reported as kept."
    why_human: "Unproven script on real data; SUMMARY follow-up 7 asks for the same."
---

# Quick Task 261122-ub4: Prepare PR 7 for merge - Verification Report

**Goal:** prepare PR 7 (`feature/261119-15c-agent-cleanup` into `dev`) for merge: merge dev, fix the six cleanup losses, correct the docs and instructions. Pushing, the PR update and artifact deletion are the orchestrator's and are not checked here.
**Verified:** 2026-11-22
**Status:** human_needed
**Re-verification:** No, initial verification
**Method:** read-only git, grep and file reads in `C:/Users/User/AppData/Local/Temp/kz-261122-ub4-prepare-pr-7` at `5e00b8a`. No node, test, build, checker, probe or cleanup run (creator decision). Because node was not allowed, the `verification.fingerprint` and `verification.status` self-check could not run; `covered_files`/`covered_digest` are omitted, not hand-written.

## Goal Achievement

### Observable Truths

| # | Truth | Status | Evidence |
| --- | --- | --- | --- |
| 1 | PR branch contains origin/dev 1a14684 through one normal two-parent merge on f373400; no rewrite, nothing pushed | VERIFIED | `24df729` parents are `f3734002...` and `1a146845...`; `76ac5aa` and `5e00b8a` are single-parent children in sequence; f373400 and 1a14684 are both ancestors of HEAD; `origin/feature/261119-15c-agent-cleanup` still points at f373400 and the branch is "ahead 4" (local tracking ref; the orchestrator pushes later). |
| 2 | .gitignore keeps the single `/.phase[0-9]*/` row; STATE.md is dev plus the 261119-15c row between 261114-we2 and 261120-09v | VERIFIED | `git diff origin/dev 24df729 -- .gitignore` replaces exactly the four `/.phaseN/` rows with `/.phase[0-9]*/`; STATE.md diff vs dev is one added line, rows at lines 220-222 ordered we2, 15c, 09v; no conflict markers in either file. AGENTS.md carries both cleanup links (lines 7, 12) and the engine-defects row (16); PR edits to AGENTS.md and verification.md apply unchanged on dev's text (diff of diffs is empty). Tree differs from dev in 14 files after the merge, 18 after all commits. |
| 3 | Worktree removal refused on any lowercase tag or `S` from `git ls-files -v` | PRESENT_BEHAVIOR_UNVERIFIED | `tools/workspace-cleanup.cjs:207` uses `git(worktree.path, ['ls-files', '-v', '-z']).split('\0').some(line => /^[a-zS] /.test(line))` with the existing reason; the old `/^[a-z]/` newline test is gone. Not executed. |
| 4 | Repository (`.git` any type, bare layout) or registered worktree at any depth in a phase-output child or `--build` folder is kept; phase root that is one is kept whole; unscannable folder kept | PRESENT_BEHAVIOR_UNVERIFIED | `looksLikeRepository` (79-82); `keepReason` (88-123) checks registered worktrees at or inside target, lstats target, walks an explicit stack, descends only into lstat-confirmed directories, returns `could not be scanned (<code>)[ at <rel>]` on readdir/lstat errors. `classifyRoot` (140-156) keeps the whole root when it is a worktree or repository, else calls `keepReason(absolute, worktreePaths)` per non-durable child; old one-level `.git` existsSync test removed. `buildOutputs` (158-169) passes every folder through `keepReason`; `planArtifacts` passes `worktreePaths` (180). Not executed. |
| 5 | Worktree removal refused for nested worktree/repository, scan failure, either runner lock; lock rechecked after all scans | PRESENT_BEHAVIOR_UNVERIFIED | `worktreeState` keeps the old check order, then `findLock(worktree.path)` (217-218), then `keepReason(worktree.path, others, {ownGit: true})` (219-221); `planWorktrees` passes all paths (226-230). `execute` rechecks `findLock(match.path)` for still-removable requested worktrees (276-283) after `planWorktrees` (273) and `planArtifacts` (274-275). Not executed. |
| 6 | `--apply` deletes nothing while either root lock exists; every refusal is decided before the first deletion, prune or removal | PRESENT_BEHAVIOR_UNVERIFIED | `findLock(root)` at 287 returns 1 under `--apply`; refusal block 292-297 returns 1; both precede `removeInside` (302), `worktree prune` (319) and `removeWorktree` (326). Planning functions delete nothing. `findLock` rethrows unexpected errors, which `run()` turns into exit 1 before deletion. Order is readable; behavior not executed. |
| 7 | Every git call accepts up to 256 MiB of output | VERIFIED | The only `execFileSync` is in `git()` (line 23) with `maxBuffer: 256 * 1024 * 1024`; every git invocation in the file goes through `git()`. This is a static option, so reading is sufficient. |
| 8 | cleanup.md and the six live docs state task-end deletion, keep/refuse rules, both locks, known exceptions A-D, and the 14 tests not rerun | VERIFIED | cleanup.md: line 19 known exceptions name `export.layout`, `export.profile`, `rules.export-layout-origin`/`-boundary` (row A, B), `ContentNativePermissionTests` (C) and the Phase 2 `--verify-completion` command (D); line 37 keep/scan paragraph plus setup rerun (setup.md 38-56 does recreate `.phase1/temp`); 44 removal deletes non-link caches; 47 full refusal list; 71 named-removal scope and any-depth/bare/unscannable keep; 73 both locks and "passed 14 of 14 at f373400 ... have not been rerun on the current script". ai-collaboration.md 46, verification.md 5/13/45/51, foundation-checks.md 53, lan-foundation-checks.md 24/56, production.md 55, engine-defects.md 67 all carry the planned wording with working relative links. No live doc still claims a single `.phase1` lock or "exists only on the machine". Docs agree with the script as read. |
| 9 | All of the above verified by reading only; no test, build, checker, node or cleanup run | VERIFIED (no contrary evidence) | `tools/workspace-cleanup.test.cjs` byte-identical to f373400; no tests added; `git status --porcelain --ignored` shows no ignored output in the checkout (only the untracked quick-task folder); SUMMARY states node was never used. A negative process claim cannot be proven absolutely from the tree. |

**Score:** 5/9 truths verified (4 present, behavior-unverified)

### Required Artifacts

| Artifact | Expected | Status | Details |
| --- | --- | --- | --- |
| `tools/workspace-cleanup.cjs` | keepReason, findLock, both locks, `-z` flag test, 256 MiB buffer | VERIFIED (present, substantive, wired) | 342 lines; `function keepReason(` at 88, `function findLock(` at 126, `RUNNER_LOCKS` at 10, `NESTED_REASON` byte-identical to test 8's string at 11. `parseArgs`, `parseWorktrees`, `removeInside`, `removeWorktree`, `run`, USAGE and exports unchanged (removed lines in `76ac5aa` are only the replaced ones). Runtime behavior unverified. |
| `docs/workflow/cleanup.md` | Corrected rules, known exceptions, not-rerun note | VERIFIED | Contains `Known exceptions:` (19) and the note (73). |
| `.gitignore` | Single `/.phase[0-9]*/` glob | VERIFIED | Line 6. |

### Key Link Verification

| From | To | Via | Status | Details |
| --- | --- | --- | --- | --- |
| classifyRoot | keepReason | each non-durable child | WIRED | line 152 `keepReason(absolute, worktreePaths)` |
| buildOutputs | keepReason | each `--build` folder | WIRED | line 166 |
| worktreeState | findLock, keepReason with ownGit | last two checks | WIRED | lines 217, 220 |
| execute | findLock(root) | above artifact deletion loop | WIRED | line 287 precedes 298/302 |
| live docs | cleanup.md | markdown links | WIRED | ai-collaboration 46, verification 13 (and 47), foundation-checks 53, lan-foundation-checks 24/56, engine-defects 67 use `cleanup.md`; production 55 uses `../workflow/cleanup.md` |

### Data-Flow Trace (Level 4)

Not applicable: no rendered data. Within the script, `worktreePaths` flows from `git worktree list --porcelain` (`planWorktrees`, `execute` 275) into `classifyRoot`, `buildOutputs` and `worktreeState`; no hardcoded empty value reaches a decision (the `worktreePaths = []` default in `planArtifacts` is overridden by `execute`).

### Behavioral Spot-Checks

SKIPPED by the creator's rule: no node, tests or cleanup runs in this task. Every behavioral check is listed under Human Verification.

### Probe Execution

SKIPPED: no probes declared; execution forbidden.

### Requirements Coverage

| Requirement | Source Plan | Description | Status | Evidence |
| --- | --- | --- | --- | --- |
| QUICK-261122-ub4 | 261122-ub4-PLAN.md | Merge dev, close six cleanup losses, correct docs | NEEDS HUMAN | Merge and docs satisfied by reading; the six fixes are present and wired but unexecuted. |

### Anti-Patterns Found

| File | Line | Pattern | Severity | Impact |
| --- | --- | --- | --- | --- |
| (all changed files) | - | TBD/FIXME/XXX/TODO/HACK | none found | - |
| `tools/workspace-cleanup.cjs` | 145 | `fs.readdirSync(directory)` on the output root is not caught | Info | An unreadable phase root aborts the whole run with exit 1 before any deletion; safe direction, but the root is not "kept and reported" individually. |
| `tools/workspace-cleanup.cjs` | 281-282, 314-316 | Late lock refusal only updates the `requested` copy | Info | The `Worktrees:` listing can still print `removable` for a worktree the late recheck refused; the refusal line and exit 1 are still printed. Cosmetic. |

### Out-of-scope observations (Info, not gaps)

- RESEARCH section 4 also proposed edits to `.planning/STATE.md` (lines 65, 227), `platform-proof.md` and `05-CONTEXT.md`; the PLAN treats these as historical records or orchestrator-owned and leaves them, and SUMMARY lists the stale STATE sentence and the `docs/code/review.md` / `docs/workflow/setup.md` "retained ... baseline" wording as follow-ups.
- SUMMARY follow-ups 1-4 (Phase 1 catalog rows and digests, rows A-D failing after cleanup, Phase 4 review source identity, kept `.phase3/revision-control-*` repositories) are PR-description items, not this task's gaps.

### Human Verification Required

1. **Syntax check** - `node --check tools/workspace-cleanup.cjs`; expected no error. The changed script has never been parsed.
2. **Existing tests** - `node --test tools/workspace-cleanup.test.cjs`; expected 14/14 on `5e00b8a`. They passed only at f373400.
3. **Loss 1** - skip-worktree, assume-unchanged and both on a pushed worktree; expected refusal `files marked assume-unchanged or skip-worktree`.
4. **Losses 2-3** - deep repository, bare layout, repository in a `--build` folder, a phase root that is itself a repository, an unreadable folder; expected each kept with its reason and nothing inside deleted.
5. **Losses 4-5** - worktree with a nested repository/worktree, an unscannable folder, `.phase1/check.lock`, `.phase2/desktop.lock`; expected refusal, exit 1, nothing deleted; lock created after planning is caught.
6. **Loss 6** - root `.phase2/desktop.lock` with `--apply`; expected exit 1 and no deletion.
7. **First real dry run** on the main checkout before `--apply`; inspect every keep/delete reason.

### Gaps Summary

No gap found by reading. The merge, conflict resolutions, doc corrections and buffer are verified. All six loss fixes are present, substantive and wired in the order the plan requires, and the docs describe them accurately, but none of them has been executed or covered by a test, and the script has not been syntax-checked. Under the creator's no-testing rule these stay PRESENT_BEHAVIOR_UNVERIFIED and route to human verification rather than a pass.

---

_Verified: 2026-11-22_
_Verifier: Claude (gsd-verifier)_
