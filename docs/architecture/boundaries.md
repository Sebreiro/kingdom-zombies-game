# Project boundary reference

The [executable five-project policy](../../tools/phase1/projects.json) is the single allowed-edge authority. [ProjectEvaluator](../../tests/phase1/ProjectEvaluator.cs) reads actual evaluated MSBuild items and recursive closure; [TypeBoundaryScanner](../../tests/phase1/TypeBoundaryScanner.cs) inspects actual compiled metadata/IL, including generated async/lambda surfaces. Missing selectors and subjects fail. [ArchitecturePolicy](../../tests/phase1/ArchitecturePolicy.cs) owns structural type, mapping, enum and version checks.

| Responsibility | Project reference |
| --- | --- |
| Pure state/identity/time/random/snapshot contracts | [Core](../../src/core/docs/README.md) |
| Independent replaceable service contracts | [Multiplayer ports](../../src/multiplayer-abstractions/docs/README.md) |
| Engine ownership and Core/ports composition | [Godot integration](../../src/godot-game/docs/README.md) |
| Product-independent Roslyn enforcement | [Code rules](../../tools/code-rules/docs/README.md) |
| Actual compiled/native/document proof inspection | [Phase 1 tests](../../tests/phase1/docs/README.md) |

The checked module declarations separate direct production references, the test-only internal Core inspection edge, and analyzer-only build metadata. Both `OutputItemType=Analyzer` and `ReferenceOutputAssembly=false` are required. Loading analyzer bytes, inspecting built assemblies and starting native processes are test tooling operations; they create no reverse production dependency. Core and ports remain independent of Godot/concrete services; providers must consume ports rather than put implementations inside the contract assembly. No sixth project, LAN provider or gameplay system is introduced.

The [consumer input reader](../../tests/phase1/ConsumerCompilation.cs) evaluates each selected Debug consumer's complete symbols, language version, Compile inputs and resolved references once per source-policy scan. Engine checks parse authored and emitted companion trees consistently and reject incomplete compilations. Native method captures are rejected even after a capture-time guard; an authored deferred wrapper must perform the guard when invoked. The R-34 repository scan checks every authored Compile input in Core, ports and Godot against the unchanged [three-type visibility registry](../code/visibility-exemptions.json). Analyzer/test public tooling surfaces and emitted generator outputs keep their separate proof scopes. Public API necessity remains a Pending human judgment.

[Boundary decision](decisions/0002-boundaries.md), [baseline decision](decisions/0001-development-baseline.md), [current rule index](../code/rule-matrix.md) and [qualitative review](../code/review.md) provide rationale and separate judgments. Commands live in [verification](../workflow/verification.md).
