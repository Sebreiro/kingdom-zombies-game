# Code reference index

The [agent guide](../../AGENTS.md) selects focused references. Executable definitions and the exact inheritance register are the rule authority (D-01 through D-04); these links do not restate their rules.

| Need | Operational definition |
| --- | --- |
| Source format, naming and severity | [.editorconfig](../../.editorconfig), [source checker](../../tests/phase1/SourceFileFormat.cs) |
| Compiler policy and analyzer integration | [Shared properties](../../Directory.Build.props), [shared targets](../../Directory.Build.targets), [central pins](../../Directory.Packages.props), [banned symbols](../../BannedSymbols.txt) |
| Twelve inherited syntax analyzers | [Analyzer project](../../tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj), [named arguments](../../tools/code-rules/NamedArgumentAnalyzer.cs), [body length](../../tools/code-rules/BodyLengthAnalyzer.cs) |
| All 85 source rule dispositions and exact positive/negative bindings | [Rule register](register.json) |
| Exact compatibility permissions | [Exemptions](exemptions.json), [permission checker](../../tests/phase1/ExemptionValidator.cs), [engine spans](engine-generated-spans.json) |
| Evaluated and compiled dependency boundaries | [Architecture index](../architecture/README.md), [graph checker](../../tests/phase1/ArchitecturePolicy.cs), [type checker](../../tests/phase1/TypeBoundaryScanner.cs) |
| Portable path, encoding, heading and reference convention | [Documentation checker](../../tests/phase1/DocumentationValidator.cs), [URL resolver](../../tests/phase1/DocumentationPath.cs), [anchor resolver](../../tests/phase1/DocumentationAnchors.cs) |
| Qualitative obligations, separately Pending | [Review checklist](review-checklist.md) |
| Why these mechanisms were selected | [Enforcement decision](../architecture/decisions/0001-enforcement.md) |
| Selecting proofs and interpreting results | [Verification](../workflow/verification.md) |

Local Markdown URLs use exact ordinal Git path segments and exact Unicode identity, relative to the source page or starting at the repository root with `/`. Encode spaces as `%20` and non-ASCII UTF-8 bytes when needed. Headings use lowercase letter/mark/digit slugs, retain `_` and `-`, remove punctuation and replace each whitespace character with `-`. Duplicate implicit slugs are ambiguous: link distinct explicit `<a id="unique-id"></a>` anchors instead; automatic numeric suffixes are not inferred. Explicit IDs are case-sensitive and unique. Full and collapsed reference labels use exact ordinal identity and must have one definition. Code fences are examples. Installed tooling, vendor/generated inputs and provenance archives are outside the authored navigation scope.
