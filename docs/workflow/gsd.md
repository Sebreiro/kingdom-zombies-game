# GSD Workflow

GSD manages project context, scoped requirements, phase planning, execution, and verification. Its planning artifacts live in `.planning/`.

## Find the Current Position

Use `$gsd-progress` to inspect the project state and identify the next workflow action. [STATE.md](../../.planning/STATE.md) records current progress and session continuity; [ROADMAP.md](../../.planning/ROADMAP.md) records phase goals and completion criteria.

## Work Through a Phase

| Command | Purpose |
| --- | --- |
| `$gsd-discuss-phase N` | Clarify the approach and capture decisions for phase N. |
| `$gsd-plan-phase N` | Research as configured and produce checked execution plans for phase N. |
| `$gsd-execute-phase N` | Execute the phase's plans and record implementation results. |
| `$gsd-verify-work N` | Check delivered behavior against the phase and user acceptance expectations. |

For the first phase, the next discussion command is `$gsd-discuss-phase 1`. Initialization alone creates no completed implementation phase.

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
