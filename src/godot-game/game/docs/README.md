# Godot integration reference

The existing Godot launch target composes Core and service ports. Its small native proof scene exercises engine-owned construction, resources, exported values, generated partials, signals, main-thread access and repeated teardown. These are development proofs; menus, movement and LAN remain later-phase work.

[Project](../KingdomZombies.csproj) · [Executable project policy](../../tools/verification/projects.json) · [Native fixture](../proofs/Phase1.cs) · [Code index](../../../../docs/code/README.md)

## Reference declaration

The two direct production references implement composition. Godot SDK packages supply engine APIs. The separate analyzer build edge must retain both metadata values.

```project-policy
{"project":"src/godot-game/game/KingdomZombies.csproj","policy":"src/godot-game/tools/verification/projects.json","productionReferences":["src/godot-game/core/KingdomZombies.Core.csproj","src/godot-game/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj"],"inspectionReferences":[],"analyzerReference":{"path":"src/godot-game/tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj","OutputItemType":"Analyzer","ReferenceOutputAssembly":"false"}}
```
