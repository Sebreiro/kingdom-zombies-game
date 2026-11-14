# GSD Workflow

GSD manages project context, scoped requirements, phase planning, execution, and verification. Its planning artifacts live in `.planning/`.

Follow the [AI collaboration instructions](ai-collaboration.md) for phase branches and PRs, testing, questions and decisions. Use interactive question controls; the project keeps `workflow.text_mode` disabled.

## Find the Current Position

Use `$gsd-progress` to inspect the project state and identify the next workflow action. [STATE.md](../../.planning/STATE.md) records current progress and session continuity; [ROADMAP.md](../../.planning/ROADMAP.md) records phase goals and completion criteria.

## Work Through a Phase

| Command | Purpose |
| --- | --- |
| `$gsd-discuss-phase N` | Clarify the approach and capture decisions for phase N. |
| `$gsd-plan-phase N` | Research as configured and produce checked execution plans for phase N. |
| `$gsd-execute-phase N` | Execute the phase's plans and record implementation results. |
| `$gsd-verify-work N` | Check delivered behavior against the phase and user acceptance expectations. |

At `$gsd-execute-phase N` start, apply the [phase branch and pull request rules](ai-collaboration.md#phase-branches-and-pull-requests): create or resume `feature/<task-id>-<short-description>` and open a draft PR to `dev` with the first implementation push. At phase completion, after required verification, mark the same PR ready for review. Preserve its branch and PR URL in continuation records.

The [GSD configuration](../../.planning/config.json) sets `git.branching_strategy` to `phase`, `git.phase_branch_template` to `feature/{phase}-{slug}` and `git.base_branch` to `dev`, with default-branch commits disabled. Agents perform the PR lifecycle actions as part of execution and completion.

[STATE.md](../../.planning/STATE.md) is the current next-action authority. Planning documents alone do not complete an implementation phase.

## Planning Artifacts

| Artifact | Purpose |
| --- | --- |
| [PROJECT.md](../../.planning/PROJECT.md) | Project definition, scope authority, constraints, and decisions. |
| [config.json](../../.planning/config.json) | Workflow settings, model inheritance, research, and verification preferences. |
| [REQUIREMENTS.md](../../.planning/REQUIREMENTS.md) | Approved version 0.1 requirements and source/phase mappings. |
| [ROADMAP.md](../../.planning/ROADMAP.md) | Ordered phases, requirement ownership, and observable completion criteria. |
| [STATE.md](../../.planning/STATE.md) | Current phase, progress, concerns, and continuation information. |
| [Research summary](../../.planning/research/SUMMARY.md) | Technical recommendations, sources, and unresolved implementation proofs. |

The configured `mvp` phase mode means working increments toward the full version 0.1 scope. `$gsd-settings` changes workflow preferences. This page describes GSD; the project specifications and focused code/architecture documents define the game and repository conventions.

The canonical [agent guide](../../AGENTS.md) selects focused references. Use [setup](setup.md) for prerequisites, [verification](verification.md) for focused/full proof commands and fresh evidence, [code](../code/README.md) for executable rules and [architecture](../architecture/README.md) for boundaries and decisions. Preserve the interactive question preferences above; `workflow.text_mode` remains `false`.
