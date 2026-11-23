# AI Collaboration

This document records the creator's preferences for AI agents working on KingdomZombies. The root [agent guide](../../AGENTS.md) indexes these instructions.

## Phase Branches and Pull Requests

- Before implementing a phase, create its branch from the latest `origin/dev` using `feature/<task-id>-<short-description>`. For GSD phases, `<task-id>` is the zero-padded phase identifier and `<short-description>` is a concise lowercase, hyphenated description; for example, `feature/02-playable-foundation`. Keep the phase's implementation commits on that branch.
- At implementation start, push the branch and open a **draft pull request targeting `dev`**, the project's default integration branch, as soon as the first implementation commit is available. Create the draft during the initial implementation work; do not postpone it until phase completion or `$gsd-ship`.
- When resuming a phase, reuse its existing feature branch and open PR. Keep that PR updated throughout development, with the phase scope and completed/pending verification in its description. Record the branch and PR URL in the phase's continuation information so another agent can resume the same work.
- Once the phase's implementation and required verification are complete, mark the **same PR ready for review**. Apply the phase-end platform and evidence requirements below, including any explicitly approved exception. Keep the PR in draft while required work or completion checks remain outstanding.
- Marking a PR ready for review does not authorize merging it. Leave the merge to the creator unless separately instructed.
- If `dev`, remote access or PR permissions are unavailable, report the specific blocker and preserve the current branch/PR state. Do not silently target another branch or claim a PR was created or made ready when the action did not succeed.

The [GSD configuration](../../.planning/config.json) uses phase branching, `feature/{phase}-{slug}` and base branch `dev`. Agents must perform the draft/ready transitions at the lifecycle points above; branch configuration alone does not create or update a PR.

## Workspace Cleanup

- At the end of every task, delete its ignored outputs and remove its worktree as described in [workspace cleanup](cleanup.md). Do not wait for phase completion or merge.
- When a phase or quick task is finished and its changes are committed and pushed, remove its worktree before reporting completion.
- First record the results the task relies on and the steps to reproduce them. Every PR description includes a **How to reproduce** section so the creator can rerun the checks.

## Task Worktrees

- Create each task worktree outside the checkout, on the task's branch, at `<worktrees root>/<main checkout folder name>/kz-<task-id>-<short-description>`. Record its path in the continuation record.
- The worktrees root is the per-user Git value `git config --get worktrees.root`. When that value is unset, the system temp folder stands in for it.
- Set the value once per user, outside the repository, so no tracked file holds a machine path: `git config --global worktrees.root <folder>`. Choose a folder on a local drive with room for build outputs and a short path, because deep worktree paths can exceed the Windows path limit (see [setup](setup.md) for `core.longpaths`).
- From any checkout of the repository, in Git Bash or another POSIX shell:

```sh
base=$(git config --get worktrees.root || node -p "require('node:os').tmpdir()")
main=$(dirname "$(git rev-parse --path-format=absolute --git-common-dir)")
git worktree add -b feature/<task-id>-<short-description> "$base/$(basename "$main")/kz-<task-id>-<short-description>" origin/dev
```

- A worktree shares the main checkout's `.local/tools`, `.local/downloads` and `.local/nuget` caches and keeps its own runs and temp; see [workspace cleanup](cleanup.md#what-to-delete-and-keep). Remove it at task end as [workspace cleanup](cleanup.md#worktrees) describes.

## Interactive Questions

- Use the available interactive question controls when asking the creator for information, preferences, or decisions. Do not substitute a numbered chat list when interactive controls are available.
- Offer clear, concise choices when useful, and allow a free-text answer. If the control supports only one selection, allow the creator to name several choices in the free-text answer when the question permits multiple selections.
- Use ordinary chat questions when interactive controls are unavailable or the creator explicitly requests a text response.
- Keep GSD's `workflow.text_mode` set to `false` for this preference. A workflow's text-mode default does not override the creator's explicit request for interactive questions.

## Development and Phase-End Testing

**Current rule for every phase, effective immediately on 2026-11-15:** Follow the [local testing strategy](local-testing.md). It supersedes the earlier two-local-OS/VM, extra-resolution, per-task rendered gates and failed-only closure rules.

- Implement the whole phase using checks without a real game window, then review its code and fix findings before rendered testing.
- Run routine local tests only on the development OS, without a VM, at 1280×720. Group compatible scenarios into shared game runs; explain necessary separate launches or multiplayer process sets.
- Select every active Critical case, every new/changed case and every affected regression, regardless of importance. This reduced selection starts now, before nightly testing exists.
- Log failures and continue independent scenarios, batch repairs, rerun failed/blocked and newly affected cases, then rerun the entire selected set on the repaired build. Use one game run where practical; otherwise retain explicit ordered run groups.
- Every phase saves reusable AI-readable scenarios with importance, setup, actions, expected results, evidence and run-group order. Keep selected omissions and historical failures visible.
- Hosted Windows/Linux CI and distinct physical LAN, supported-display, hardware and endurance release procedures keep their own evidence. Routine phase completion no longer requires a local other-OS VM. Phase 11 introduces nightly full testing across all active importance levels.

**Amendment, creator-approved on 2026-11-23:** The [local testing strategy](local-testing.md) carries five rule changes for Phases 6–10 and a reduced Phase 5 scope, with certification moved to Phase 11. Where they differ from the bullets above or the Phase 5 section below, the strategy governs.

See [verification](verification.md) for delivered commands. The older [pre-Phase-5 entry point](pre-phase5-testing.md) now redirects to the permanent strategy.

## Phase 5 Execution and Evidence Reuse Amendment

- Ordinary Windows/Linux PRs target about ten minutes per OS with dependencies cached. Run the strict build with analyzers, current project/code boundary checks and documentation/catalog validation. Do not launch the game or include exhaustive deliberate-defect proofs, full LAN scenarios, coverage certification or AI screenshot review in this basic selection.
- During feature work, run checks without a real game window. At the post-review game-test stage, select all active Critical cases plus new/changed cases and affected regressions, including consumers, transitive dependencies and shared inputs. An unchanged folder alone does not establish unaffected behavior. Uncertain impact is an explicit selection blocker.
- Reuse eligible passing evidence only when relevant inputs and the required environment still match. Account for the declared selection and explicit omissions. Reuse cannot replace the fresh final selected-set rendered pass after repairs; the complete catalog is available manually and becomes scheduled in Phase 11.
- Keep original reports, tested revisions and invocation identities immutable until task-end [cleanup](cleanup.md); reuse in later tasks needs durable evidence. Completion reports distinguish executed-now, explicitly reused and missing/invalidated evidence. Reuse validates an earlier result; it never supplies a newly executed pass or cross-platform substitution. An unresolved later failure prevents acceptance through an older pass.
- Before/after screenshots are checkpoints within a selected scenario on the current build, not obligatory runs of old/new builds for every edit. Keep real input, fresh-run evidence and declared appearance/readability review.
- Phase 5 must implement and prove dependency-aware selection. Evidence reuse validation moved to Phase 11 under TEST-12; until then, the current Phase 1 runner still executes selected cases afresh. This policy invents no reuse command or retroactive certification. Actual full/manual workflow compatibility still needs execution evidence.

See [Phase 5 context](../../.planning/phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md) and its [amended specification](../../.planning/phases/05-ci-and-repeatable-game-testing/05-SPEC.md). Ten minutes is an optimization target, not permission to omit selected checks. Report measured warm/cold, per-group and total times.

**One-time Phase 1 exception, later on 2026-11-14:** The creator requested no Windows restart, a few important actual Linux tests, then Phase 1 closure. The [closure exception](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) predeclares the focused checks and evidence limits. Preserve the full Windows result; keep the unexecuted complete Linux suite and unreviewed detailed human checklist visible as follow-ups. This exception does not change the rule for later phases or fabricate test/review passes.

See [GSD workflow](gsd.md) for phase commands and planning artifacts.
