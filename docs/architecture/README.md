# Architecture reference index

| Need | Definition or decision |
| --- | --- |
| Exact five-project inventory and allowed references | [Project graph](../../tools/phase1/projects.json), [evaluated graph policy](../../tests/phase1/ArchitecturePolicy.cs) |
| Each project's purpose and checked reference declaration | [Core](../../src/core/docs/README.md), [service ports](../../src/multiplayer-abstractions/docs/README.md), [Godot](../../src/godot-game/docs/README.md), [analyzer](../../tools/code-rules/docs/README.md), [proofs](../../tests/phase1/docs/README.md) |
| Compiled public/type boundaries and engine access | [Type scanner](../../tests/phase1/TypeBoundaryScanner.cs), [engine access policy](../../tests/phase1/EngineAccessPolicy.cs) |
| Composition and independent pure state | [Godot project](../../src/godot-game/KingdomZombies.csproj), [Core project](../../src/core/KingdomZombies.Core.csproj), [service ports](../../src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj) |
| Executable rules and scoped compatibility | [Code index](../code/README.md), [enforcement decision](decisions/0001-enforcement.md) |
| Why gameplay, engine and services remain separated | [Boundary decision](decisions/0002-boundaries.md) |
| Controlled state, strict snapshots and engine lifetime | [State and engine decision](decisions/0003-state-and-engine.md) |
| Qualitative decision and invariant review | [Review checklist](../code/review-checklist.md) |

Each module page selects the shared executable policy and declares its checked reference categories. These indexes describe the implemented foundation; they do not certify gameplay, LAN, Linux execution or measured coverage.
