---
phase: quick
plan: 261114-we2
subsystem: workflow
tags: [agents, git, gsd, pull-requests]
status: complete
completed: 2026-11-14
requirements-completed: []
key-files:
  modified:
    - AGENTS.md
    - docs/workflow/ai-collaboration.md
    - docs/workflow/gsd.md
    - .planning/config.json
key-decisions:
  - Use the zero-padded GSD phase ID as the task ID in feature/{phase}-{slug}.
  - Open a draft PR to dev at the first implementation push and promote the same PR after phase completion and required verification.
  - Configure phase branching and dev as the base, and disable default-branch commits.
---

# Quick Task 261114-we2: Phase Branch and PR Lifecycle

Phase implementation now uses `feature/<task-id>-<short-description>`, with an early draft PR targeting `dev` and ready-for-review promotion after implementation and required verification.

## Changes

- Added the canonical rules to [AI collaboration](../../../docs/workflow/ai-collaboration.md#phase-branches-and-pull-requests), including phase ID naming, the `origin/dev` start point, branch/PR reuse, continuation records and honest reporting of failed remote operations.
- Linked the lifecycle directly from the [agent guide](../../../AGENTS.md) and [phase workflow](../../../docs/workflow/gsd.md).
- Aligned [GSD settings](../../config.json): phase strategy, `feature/{phase}-{slug}`, base `dev` and default-branch commit protection. PR state transitions remain mandatory agent actions.

## Verification

- `node tools/phase1/check.cjs --case docs.navigation` passed its one selected obligation on Windows. Report: `.phase1/runs/16bbd47c-dae2-427b-a1c9-67189e19f4a0/summary.json`.
- The first restricted attempt stopped at locked restore before executing the documentation case; its preserved report is `.phase1/runs/f3265579-8f1e-49cd-bf38-b678c0ee2c5e/summary.json`. Repeating with installed-tool/cache access produced the pass above.
- JSON assertions confirmed phase strategy, base `dev`, the feature template, disabled default-branch commits and the unchanged interactive-question setting.
- Read-only `query init.execute-phase 2` resolved `feature/02-playable-foundation`; `query git.base-branch --raw` resolved `dev`.
- The first navigation check including the quick-task registry rejected GSD's generated folder link (`DOC_TARGET`), preserved at `.phase1/runs/f1382985-8f5f-4970-bcf6-294de73ebe57/summary.json`. The schema-backed append helper's `--directory` override now links directly to this summary file, satisfying the repository's exact-file navigation convention.
- Final documentation navigation passed with the planning records included: `.phase1/runs/c5d838f7-536d-46fb-ba87-f2e90dc18fb9/summary.json`, one selected obligation passed and no failures.
- `git diff --check` passed for the policy/settings change and staged planning records.

## Commit and Scope

- Policy/settings commit: `862ac06` — `docs(quick-261114-we2): require phase feature branches and draft PRs`.
- Published task branch: `feature/261114-we2-phase-pr-lifecycle`. Its two task commits were replayed onto `origin/dev` before publication after confirming that the merged development tree exactly matched the original task baseline. The tested implementation bytes and original evidence identities are preserved.
- Pull request: [#2 — Require phase feature branches and draft PR lifecycle](https://github.com/Sebreiro/kingdom-zombies-game/pull/2), targeting `dev`. Opened as a draft; its current review state is recorded on GitHub. The PR contains only this task's seven changed files.
- GitHub confirmed `dev` as the default branch. This documentation/configuration task did not start a game implementation phase or change repository administration settings.
- The quick workflow ran inline under its Codex spawn restriction. Phase 02 remains ready to plan; no roadmap, phase-completion or acceptance status changed.
