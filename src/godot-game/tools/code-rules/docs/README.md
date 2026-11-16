# Code rules analyzer reference

Repository-configured Roslyn analyzers enforce twelve syntax/style contracts. The core rules are reusable; exact generated-source allowances also name this game's observed Godot/KingdomZombies surfaces. This configured copy belongs in the game workspace. The analyzer has no product or test assembly dependencies and excludes its own shared analyzer edge. Captured donor provenance and exact generated permissions stay repository-owned.

The project inherits shared compiler/package policy from [the workspace](../../../README.md), as the other projects do. These settings create no runtime game dependency. Run direct .NET builds from that workspace to select its SDK pin.

[Project](../Sebreiro.CodeRules.Analyzers.csproj) · [Executable project policy](../../verification/projects.json) · [Rule register](../../../../../docs/code/register.json) · [Code index](../../../../../docs/code/README.md)

## Reference declaration

Production, inspection and analyzer project references are empty. Roslyn package dependencies are tooling inputs, not product references.

```project-policy
{"project":"src/godot-game/tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj","policy":"src/godot-game/tools/verification/projects.json","productionReferences":[],"inspectionReferences":[],"analyzerReference":null}
```
