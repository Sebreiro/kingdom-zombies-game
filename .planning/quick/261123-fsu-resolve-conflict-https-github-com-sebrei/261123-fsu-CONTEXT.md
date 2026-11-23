# Quick Task 261123-fsu: Resolve PR #6 merge conflicts with dev - Context

**Gathered:** 2026-11-23
**Status:** Ready for planning

<domain>
## Task Boundary

> resolve conflict https://github.com/Sebreiro/kingdom-zombies-game/pull/6

PR #6 (`feature/quick-repository-layout`, head `c5be170`) targets `dev` (`a305b8b`). Since the merge base `5428da8`, dev gained the Phase 4 squash `1a14684` (PR #5) and the task-end cleanup change `a305b8b` (PR #7, merged). GitHub reports the PR as conflicting.

Earlier quick task 261122-uby discussed, researched and planned the same integration. Its plan was never executed and the branch is unchanged. Its seed folder is now empty. Its files are untracked in the PR worktree at `.planning/quick/261122-uby-resolve-pr-6-merge-conflicts-and-check-d/`. They contain machine-specific paths and must not be committed.

</domain>

<decisions>
## Implementation Decisions

### Use of task 261122-uby
- Creator chose: reuse its decisions and research as inputs, refresh the research against today's branches, and write a new, tighter plan. Do not execute the old plan.
- The decisions below restate what carries over, so this file stands alone.

### Scope
- Creator chose: conflicts plus the rebinding the move requires.
- In scope: merge `origin/dev` into the PR branch; place every Phase 4 and PR #7 file at its current layout location; fix every binding the move breaks (paths, registers, catalogs, hash bindings, generated indexes, live doc links and commands); keep both sides' intent in every conflicted file.
- Out of scope: the wider live-doc sweep from 261122-uby. That means no move of worktree-creation rules out of `docs/workflow/cleanup.md`, no unrelated policy-contradiction fixes, and no clean-up of machine-specific paths in files this task only moves.

### Integration method and publishing
- Integrate `origin/dev` with one merge commit, followed by fix commits as needed. No rebase, no `git stash`, no force-push. Do not merge the PR.
- Creator chose: after the checks pass and the review is clean, push to `feature/quick-repository-layout` and update the live PR #6 description.
- The orchestrator does the push and the description edit. Subagents never push or change GitHub.

### Verification
- Creator chose: strict locked build with analyzers, the layout tests and the documentation checks.
- Not run, and reported as not run: Windows export, editor-import self-test, headless launch, game windows and the full 776-case selection.
- Phase 4 review-bound gates (`runner.prelaunch`, `runner.selection`, `acceptance.*`, `--verify-completion`) are expected-blocked by the relocation. Phase 4 review records stay byte-identical. Their renewal is handed to Phase 5 and recorded in STATE.md and the PR description. No new Phase 4 review here.

### Carried from 261122-uby
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

</decisions>

<specifics>
## Specific Ideas

- Do not weaken, skip or broaden a validator, pin, exemption or expected count to get a pass. Rebind only what a move requires.
- Use tools and caches already on this machine. No downloads or installs; `git fetch origin` is the only network step besides the final push.
- The main checkout, other worktrees and other branches are read-only. Copy from them; never move or link.
- Quick-task records name locations symbolically (PR worktree, main checkout, verification worktree). No real machine paths in anything committed.
- This task's artifacts and its STATE.md row live on the PR branch, committed after the merge.
- Commit messages end with `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`.

</specifics>

<canonical_refs>
## Canonical References

- `docs/workflow/phase4-layout-integration.md` (PR side)
- `docs/architecture/repository-layout.md` (PR side)
- `docs/workflow/cleanup.md` (dev side, PR #7)
- `docs/workflow/ai-collaboration.md`
- `docs/workflow/local-testing.md`
- `.planning/phases/04-asset-production/04-CLOSURE-EXCEPTION.md` (dev side)
- Untracked inputs: `.planning/quick/261122-uby-resolve-pr-6-merge-conflicts-and-check-d/261122-uby-{CONTEXT,RESEARCH,PLAN}.md` in the PR worktree

</canonical_refs>
