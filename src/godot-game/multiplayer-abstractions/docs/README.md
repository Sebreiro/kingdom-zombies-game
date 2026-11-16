# Multiplayer service ports reference

Independent pure C# service contracts. The minimal `IProofService` demonstrates a replaceable port without implementing LAN discovery, accounts or connections. Ports cannot depend on Core, Godot or concrete providers.

[Project](../KingdomZombies.Multiplayer.Abstractions.csproj) · [Executable project policy](../../tools/verification/projects.json) · [Boundary checks](../../tests/verification/TypeBoundaryScanner.cs) · [Code index](../../../../docs/code/README.md)

## Reference declaration

Production and inspection references are empty. The shared analyzer-only build edge requires both metadata values.

```project-policy
{"project":"src/godot-game/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj","policy":"src/godot-game/tools/verification/projects.json","productionReferences":[],"inspectionReferences":[],"analyzerReference":{"path":"src/godot-game/tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj","OutputItemType":"Analyzer","ReferenceOutputAssembly":"false"}}
```
