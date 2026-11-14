# Architecture reference index

| Need | Definition or decision |
| --- | --- |
| Exact five-project inventory and allowed references | [Project graph](../../tools/phase1/projects.json), [evaluated graph policy](../../tests/phase1/ArchitecturePolicy.cs) |
| Compiled public/type boundaries and engine access | [Type scanner](../../tests/phase1/TypeBoundaryScanner.cs), [engine access policy](../../tests/phase1/EngineAccessPolicy.cs) |
| Composition and independent pure state | [Godot project](../../src/godot-game/KingdomZombies.csproj), [Core project](../../src/core/KingdomZombies.Core.csproj), [service ports](../../src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj) |
| Executable rules and scoped compatibility | [Code index](../code/README.md), [enforcement decision](decisions/0001-enforcement.md) |
| Why gameplay, engine and services remain separated | [Boundary decision](decisions/0002-boundaries.md) |
| Controlled state, strict snapshots and engine lifetime | [State and engine decision](decisions/0003-state-and-engine.md) |
| Qualitative decision and invariant review | [Review checklist](../code/review-checklist.md) |

The five project reference pages and final register closure belong to plan 01-14. These indexes describe the implemented foundation; they do not certify gameplay, LAN, Linux execution or measured coverage.
