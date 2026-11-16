# Code reference index

The [agent guide](../../AGENTS.md) selects focused references. Executable definitions and the exact inheritance register are the rule authority (D-01 through D-04); these links do not restate their rules.

| Need | Operational definition |
| --- | --- |
| Reviewing a pull request, including external review tools | [PR review entry point](../workflow/code-review.md) |
| Workspace placement and compiler-policy file purposes | [Layout policy](../architecture/repository-layout.md), [build policy guide](../../src/godot-game/build/README.md) |
| Source format, naming and severity | [.editorconfig](../../.editorconfig), [source checker](../../src/godot-game/tests/verification/SourceFileFormat.cs) |
| Compiler policy and analyzer integration | [Shared properties](../../src/godot-game/Directory.Build.props), [shared targets](../../src/godot-game/Directory.Build.targets), [central pins](../../src/godot-game/Directory.Packages.props), [banned symbols](../../src/godot-game/build/BannedSymbols.txt) |
| Twelve inherited syntax analyzers | [Analyzer project](../../tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj), [named arguments](../../tools/code-rules/NamedArgumentAnalyzer.cs), [body length](../../tools/code-rules/BodyLengthAnalyzer.cs) |
| All 85 source rule dispositions and exact positive/negative bindings | [Rule register](register.json) |
| Current source symbols/settings, scopes and distinct proof methods | [Rule mechanism index](rule-matrix.md) |
| Exact compatibility permissions | [Exemptions](exemptions.json), [permission checker](../../src/godot-game/tests/verification/ExemptionValidator.cs), [engine spans](engine-generated-spans.json) |
| Evaluated and compiled dependency boundaries | [Architecture index](../architecture/README.md), [graph checker](../../src/godot-game/tests/verification/ArchitecturePolicy.cs), [type checker](../../src/godot-game/tests/verification/TypeBoundaryScanner.cs) |
| Portable path, encoding, heading and reference convention | [Documentation checker](../../src/godot-game/tests/verification/DocumentationValidator.cs), [URL resolver](../../src/godot-game/tests/verification/DocumentationPath.cs), [anchor resolver](../../src/godot-game/tests/verification/DocumentationAnchors.cs) |
| Qualitative obligations, separately Pending | [Review checklist](review-checklist.md), [exact human components](review.md) |
| Why these mechanisms were selected | [Enforcement decision](../architecture/decisions/0001-enforcement.md) |
| Selecting proofs and interpreting results | [Verification](../workflow/verification.md) |

Local Markdown URLs use exact ordinal Git path segments and exact Unicode identity, relative to the source page or starting at the repository root with `/`. Encode spaces as `%20` and non-ASCII UTF-8 bytes when needed. Headings use lowercase letter/mark/digit slugs, retain `_` and `-`, remove punctuation and replace each whitespace character with `-`. Duplicate implicit slugs are ambiguous: link distinct explicit `<a id="unique-id"></a>` anchors instead; automatic numeric suffixes are not inferred. Explicit IDs are case-sensitive and unique. Full and collapsed reference labels use exact ordinal identity and must have one definition. Code fences are examples. Installed tooling, vendor/generated inputs and provenance archives are outside the authored navigation scope.
