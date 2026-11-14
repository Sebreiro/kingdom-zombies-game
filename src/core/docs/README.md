# Core reference

Pure C# proof state, ordinal identities, controlled elapsed time/randomness and detached snapshots. This assembly contains no Godot objects or service providers. Production gameplay remains later work.

[Project](../KingdomZombies.Core.csproj) · [Executable project policy](../../../tools/phase1/projects.json) · [Boundary checks](../../../tests/phase1/ProjectEvaluator.cs) · [Code index](../../../docs/code/README.md)

## Reference declaration

The checked declaration selects this project's row in the shared policy. Production references are empty. The analyzer is a build tool with both metadata fields required; it is not an assembly dependency. The scoped friend test assembly grants inspection access without a Core-to-tests edge.

```project-policy
{"project":"src/core/KingdomZombies.Core.csproj","policy":"tools/phase1/projects.json","productionReferences":[],"inspectionReferences":[],"analyzerReference":{"path":"tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj","OutputItemType":"Analyzer","ReferenceOutputAssembly":"false"}}
```
