# Phase 1 proof reference

C# tests inspect actual compiler/analyzer behavior, evaluated MSBuild references, compiled metadata/IL, strict state, native evidence and documentation. Node coordinates bounded processes and fresh reports. No test-only authority or inspection API is a gameplay feature.

[Project](../KingdomZombies.Phase1.Tests.csproj) · [Executable project policy](../../../tools/phase1/projects.json) · [Verification](../../../docs/workflow/verification.md) · [Code index](../../../docs/code/README.md)

## Reference declaration

The sole assembly reference is an internal Core inspection edge through the scoped friend assembly. Tests also load analyzer bytes, inspect built game/ports metadata and launch the real engine through bounded child processes; these are inspection/tool operations, not additional production ProjectReference edges. xUnit, Shouldly and Roslyn packages are test tools. The inherited analyzer edge retains both metadata values.

```project-policy
{"project":"tests/phase1/KingdomZombies.Phase1.Tests.csproj","policy":"tools/phase1/projects.json","productionReferences":[],"inspectionReferences":["src/core/KingdomZombies.Core.csproj"],"analyzerReference":{"path":"tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj","OutputItemType":"Analyzer","ReferenceOutputAssembly":"false"}}
```
