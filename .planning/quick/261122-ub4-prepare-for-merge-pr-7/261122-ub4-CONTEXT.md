# Quick Task 261122-ub4: prepare for merge PR 7 - Context

**Gathered:** 2026-11-22
**Status:** Ready for planning

<domain>
## Task Boundary

Prepare https://github.com/Sebreiro/kingdom-zombies-game/pull/7 (`feature/261119-15c-agent-cleanup` into `dev`) for merge: docs, instructions and the cleanup script. Delete the Phase 1-4 artifacts. Do not merge the PR.

</domain>

<decisions>
## Implementation Decisions

### Scope
- Merge `origin/dev` without rewriting history and resolve the conflicts.
- Fix all six reproduced cleanup losses:
  1. skip-worktree edits are lost (`git ls-files -v` prints `S`, or `s` with assume-unchanged; only lowercase is detected);
  2. a repository nested deeper than one level inside an output folder is deleted;
  3. `--build` deletes a worktree or repository inside `bin/`, `obj/` or `.godot/`;
  4. removing a worktree deletes a worktree or repository nested in its ignored folders;
  5. removing a worktree ignores that worktree's `.phase1/check.lock`;
  6. `--apply` deletes a live `.phase2/desktop.lock`, which the Phase 2, 3 and 4 runners use.
- Correct the cleanup docs to match.

### Testing
- The creator's words: "You don't need to test nothing in this task. You just need to preapare docs, instructions and clean up script!!! And delete all artefacts from pase1-phase4".
- Run no tests, test suites, builds, checker cases or behavioral probes. Write no new tests. This replaces the regression-test part of the scope option. Verification is by reading only.
- Do not edit case catalogs, digests, game code or Phase 1-4 tools. Report any follow-up they need.

### Files two checks need
- Delete as written. No exception for `.phase4/checkpoints` (`export.profile`) or `.phase4/04-23-current-model` (`ContentNativePermissionTests`), although both checks will fail afterwards. Record that consequence.

### Artifact deletion
- Delete the Phase 1-4 ignored artifacts of the main checkout. The orchestrator alone does this; no subagent deletes artifacts or runs the cleanup command.
- The seven pinned `.phase1` caches are not artifacts and stay. Tracked planning records stay. Other worktrees are not touched.

### Publishing
- Normal push to the PR branch and update the PR description. The orchestrator alone does this. Never merge the PR.

### Claude's Discretion
- `.gitignore`: keep the PR's `/.phase[0-9]*/`, which covers dev's explicit rows.
- `STATE.md`: keep dev's state and both quick-task rows in date order.

</decisions>

<specifics>
## Specific Ideas

- All six losses and a 14/14 pass of the existing tests were observed at f373400 before the no-testing decision. Neither says anything about the changed script.

</specifics>

<canonical_refs>
## Canonical References

- `docs/workflow/cleanup.md`
- `docs/workflow/ai-collaboration.md`
- `.planning/quick/261119-15c-agent-cleanup-rules/261119-15c-CONTEXT.md`

</canonical_refs>
