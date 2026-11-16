# KingdomZombies — Agent Guide

KingdomZombies is a Godot/C# side-view survival and camp-management game for solo or up to four cooperative LAN players on Windows and Linux. Version 0.1 delivers the complete game in the specification.

This is the documentation entry point. Open the documents relevant to the current task.

Before starting or finishing phase implementation, follow the [phase branch and pull request rules](docs/workflow/ai-collaboration.md#phase-branches-and-pull-requests).

| Need | Read |
| --- | --- |
| Repository layout, game workspace and file ownership | [Repository map](README.md), [layout policy](docs/architecture/repository-layout.md) |
| Pull request review entry point, rules and evidence | [Code review](docs/workflow/code-review.md) |
| Game development tools and generated output | [Game tools](src/godot-game/tools/README.md) |
| AI collaboration, phase branches/PRs, testing platforms, and decisions | [AI collaboration](docs/workflow/ai-collaboration.md) |
| C# rules, executable definitions, exemptions, and qualitative review | [Code index](docs/code/README.md) |
| Project boundaries and architecture decisions | [Architecture index](docs/architecture/README.md) |
| Toolchain prerequisites and isolated setup | [Development setup](docs/workflow/setup.md) |
| Selecting proofs and interpreting current evidence | [Verification](docs/workflow/verification.md) |
| Local test selection, saved scenarios, run grouping, and the phase repair cycle | [Local testing strategy](docs/workflow/local-testing.md) |
| Required game behavior, balance baseline, exclusions, and acceptance scenarios | [Version 0.1 specification](specs/0.1/initial-spec.md) |
| Required assets, recovered donors, and production checks | [Asset and resource plan](specs/0.1/assets-and-resources.md) |
| Project context and decisions | [PROJECT.md](.planning/PROJECT.md) |
| Approved requirements and phase ownership | [REQUIREMENTS.md](.planning/REQUIREMENTS.md) |
| Delivery phases and completion criteria | [ROADMAP.md](.planning/ROADMAP.md) |
| Current position and next action | [STATE.md](.planning/STATE.md) |
| Technical research and unresolved proofs | [Research summary](.planning/research/SUMMARY.md) |
| GSD commands and planning artifacts | [GSD workflow](docs/workflow/gsd.md) |

Phase 1 establishes the enforced code standards, architecture rules, proof tests, and focused documentation tree. Their implementation is not complete merely because initialization documents exist.
