# Code rules analyzer reference

Reusable Roslyn analyzers enforce twelve syntax/style contracts. The analyzer has no product or test assembly dependencies and excludes its own shared analyzer edge. Captured donor provenance and exact generated permissions stay repository-owned.

[Project](../Sebreiro.CodeRules.Analyzers.csproj) · [Executable project policy](../../phase1/projects.json) · [Rule register](../../../docs/code/register.json) · [Code index](../../../docs/code/README.md)

## Reference declaration

Production, inspection and analyzer project references are empty. Roslyn package dependencies are tooling inputs, not product references.

```project-policy
{"project":"tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj","policy":"tools/phase1/projects.json","productionReferences":[],"inspectionReferences":[],"analyzerReference":null}
```
