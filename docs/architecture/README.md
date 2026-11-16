# Architecture reference index

| Need | Definition or decision |
| --- | --- |
| Repository layout, file ownership and migration map | [Layout policy](repository-layout.md), [game workspace](../../src/godot-game/README.md) |
| Pull request review process and linked authorities | [Code review](../workflow/code-review.md) |
| Exact five-project inventory and allowed references | [Project graph](../../src/godot-game/tools/verification/projects.json), [evaluated graph policy](../../src/godot-game/tests/verification/ArchitecturePolicy.cs) |
| Boundary rationale and proven baseline | [Boundaries](boundaries.md), [development baseline](decisions/0001-development-baseline.md) |
| Each project's purpose and checked reference declaration | [Core](../../src/godot-game/core/docs/README.md), [service ports](../../src/godot-game/multiplayer-abstractions/docs/README.md), [Godot](../../src/godot-game/game/docs/README.md), [analyzer](../../tools/code-rules/docs/README.md), [proofs](../../src/godot-game/tests/verification/docs/README.md) |
| Compiled public/type boundaries and engine access | [Type scanner](../../src/godot-game/tests/verification/TypeBoundaryScanner.cs), [engine access policy](../../src/godot-game/tests/verification/EngineAccessPolicy.cs) |
| Composition and independent pure state | [Godot project](../../src/godot-game/game/KingdomZombies.csproj), [Core project](../../src/godot-game/core/KingdomZombies.Core.csproj), [service ports](../../src/godot-game/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj) |
| Executable rules and scoped compatibility | [Code index](../code/README.md), [enforcement decision](decisions/0001-enforcement.md) |
| Why gameplay, engine and services remain separated | [Boundary decision](decisions/0002-boundaries.md) |
| Controlled state, strict snapshots and engine lifetime | [State and engine decision](decisions/0003-state-and-engine.md) |
| Qualitative decision and invariant review | [Review checklist](../code/review-checklist.md) |

Each module page selects the shared executable policy and declares its checked reference categories. These indexes describe the implemented foundation; they do not certify gameplay, LAN, Linux execution or measured coverage.
