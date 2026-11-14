---
phase: 01-code-rules-and-documentation
reviewed: 2026-11-14T17:33:08Z
revision: 6d208727543eb462f3495515f9928acd8075a0e3
depth: standard
files_reviewed: 182
files_reviewed_list:
  - .config/dotnet-tools.json
  - .editorconfig
  - .gitignore
  - AGENTS.md
  - BannedSymbols.Pure.txt
  - BannedSymbols.Tests.txt
  - BannedSymbols.txt
  - Directory.Build.props
  - Directory.Build.targets
  - Directory.Packages.props
  - Directory.Xunit.targets
  - KingdomZombies.slnx
  - NuGet.config
  - docs/architecture/README.md
  - docs/architecture/boundaries.md
  - docs/architecture/decisions/0001-development-baseline.md
  - docs/architecture/decisions/0001-enforcement.md
  - docs/architecture/decisions/0002-boundaries.md
  - docs/architecture/decisions/0003-state-and-engine.md
  - docs/code/README.md
  - docs/code/engine-adaptations.json
  - docs/code/engine-generated-spans.json
  - docs/code/exemptions.json
  - docs/code/generated-callsite-adaptations.json
  - docs/code/generated-layout-adaptations.json
  - docs/code/generated-syntax-adaptations.json
  - docs/code/provenance/analyzer-adoption.json
  - docs/code/provenance/manifest.json
  - docs/code/provenance/xunit-entrypoint-path-adaptation.json
  - docs/code/register.json
  - docs/code/review-checklist.md
  - docs/code/review.md
  - docs/code/rule-matrix.md
  - docs/code/visibility-exemptions.json
  - docs/workflow/ai-collaboration.md
  - docs/workflow/gsd.md
  - docs/workflow/platform-proof.md
  - docs/workflow/setup.md
  - docs/workflow/verification.md
  - global.json
  - src/core/ExampleState.cs
  - src/core/ExampleStateStore.cs
  - src/core/IElapsedInput.cs
  - src/core/IRandomInput.cs
  - src/core/KingdomZombies.Core.csproj
  - src/core/ProofKind.cs
  - src/core/ProofValue.cs
  - src/core/ReplayRandom.cs
  - src/core/SnapshotCodec.cs
  - src/core/SnapshotDto.cs
  - src/core/SnapshotJson.cs
  - src/core/SnapshotWriter.cs
  - src/core/StableIdentity.cs
  - src/core/docs/README.md
  - src/godot-game/GeneratedCallSiteAdapters.cs
  - src/godot-game/GeneratedReportAdapter.cs
  - src/godot-game/KingdomZombies.csproj
  - src/godot-game/docs/README.md
  - src/godot-game/proofs/InvalidFixture.tscn
  - src/godot-game/proofs/LifecycleFixture.cs
  - src/godot-game/proofs/LifetimeToken.cs
  - src/godot-game/proofs/MainThreadGateway.cs
  - src/godot-game/proofs/Phase1.cs
  - src/godot-game/proofs/Phase1.tscn
  - src/godot-game/proofs/ProofResource.cs
  - src/godot-game/proofs/ProofResource.tres
  - src/godot-game/proofs/ProofSequence.cs
  - src/godot-game/proofs/ValidFixture.tscn
  - src/multiplayer-abstractions/IProofService.cs
  - src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj
  - src/multiplayer-abstractions/docs/README.md
  - tests/phase1/AnalyzerInputIdentityTests.cs
  - tests/phase1/AnalyzerSourceConsumerTests.cs
  - tests/phase1/Analyzers/BodyCommentAnalyzerTests.cs
  - tests/phase1/Analyzers/BodyLengthAnalyzerTests.cs
  - tests/phase1/Analyzers/ContinuationIndentAnalyzerTests.cs
  - tests/phase1/Analyzers/LineLengthAnalyzerTests.cs
  - tests/phase1/Analyzers/LocalNewDeclarationAnalyzerTests.cs
  - tests/phase1/Analyzers/MemberOrderAnalyzerTests.cs
  - tests/phase1/Analyzers/NamedArgumentAnalyzerTests.cs
  - tests/phase1/Analyzers/NullForgivingAnalyzerTests.cs
  - tests/phase1/Analyzers/PrimaryConstructorAnalyzerTests.cs
  - tests/phase1/Analyzers/RegionDirectiveAnalyzerTests.cs
  - tests/phase1/Analyzers/ThisQualificationAnalyzerTests.cs
  - tests/phase1/Analyzers/UsingOrderAnalyzerTests.cs
  - tests/phase1/ArchitecturePolicy.cs
  - tests/phase1/ArchitectureRuleTests.cs
  - tests/phase1/BoundaryFixture.cs
  - tests/phase1/BuildFixture.cs
  - tests/phase1/ConfigurationTests.cs
  - tests/phase1/ControlledElapsed.cs
  - tests/phase1/ControlledInputPolicy.cs
  - tests/phase1/ControlledRandom.cs
  - tests/phase1/DocumentationAnchors.cs
  - tests/phase1/DocumentationDecisions.cs
  - tests/phase1/DocumentationFixture.cs
  - tests/phase1/DocumentationLinks.cs
  - tests/phase1/DocumentationPath.cs
  - tests/phase1/DocumentationProjectTests.cs
  - tests/phase1/DocumentationProjects.cs
  - tests/phase1/DocumentationTests.cs
  - tests/phase1/DocumentationValidator.cs
  - tests/phase1/EnforcementTests.cs
  - tests/phase1/EngineAccessPolicy.cs
  - tests/phase1/EngineBoundaryTests.cs
  - tests/phase1/EngineCaseTests.cs
  - tests/phase1/EngineEvidencePolicy.cs
  - tests/phase1/EngineEvidenceTests.cs
  - tests/phase1/EngineGeneratedTests.cs
  - tests/phase1/EngineRun.cs
  - tests/phase1/ExemptionTests.cs
  - tests/phase1/ExemptionValidator.cs
  - tests/phase1/GeneratedCallSiteAdapterTests.cs
  - tests/phase1/GeneratedCallSiteAdapters.cs
  - tests/phase1/GeneratedLayoutAdapterTests.cs
  - tests/phase1/GeneratedSurfaceScanner.cs
  - tests/phase1/GeneratedSyntaxAdapterTests.cs
  - tests/phase1/GodotCallSiteAdapterTests.cs
  - tests/phase1/GraphFixture.cs
  - tests/phase1/GraphTests.cs
  - tests/phase1/KingdomZombies.Phase1.Tests.csproj
  - tests/phase1/Models/BoundaryEntity.cs
  - tests/phase1/Models/BoundaryKind.cs
  - tests/phase1/Models/BoundaryMapper.cs
  - tests/phase1/Models/BoundaryOptions.cs
  - tests/phase1/Models/BoundarySnapshotDto.cs
  - tests/phase1/PinEdgeTests.cs
  - tests/phase1/PinsTests.cs
  - tests/phase1/ProjectEvaluator.cs
  - tests/phase1/ProofProtocol.cs
  - tests/phase1/RandomContractTests.cs
  - tests/phase1/RegisterTests.cs
  - tests/phase1/RegisterValidator.cs
  - tests/phase1/RestoreAtomicityTests.cs
  - tests/phase1/RestoreConcurrencyProof.cs
  - tests/phase1/RestoreMutationProof.cs
  - tests/phase1/RuleBindingTests.cs
  - tests/phase1/RuleBindingValidator.cs
  - tests/phase1/RuleDefinitions.cs
  - tests/phase1/RuleMatrix.cs
  - tests/phase1/RunnerContractTests.cs
  - tests/phase1/RunnerProcessFixture.cs
  - tests/phase1/SnapshotProofData.cs
  - tests/phase1/SnapshotShapePolicy.cs
  - tests/phase1/SnapshotTests.cs
  - tests/phase1/SourceFileFormat.cs
  - tests/phase1/StateInputTests.cs
  - tests/phase1/SuppressionPolicy.cs
  - tests/phase1/SuppressionTests.cs
  - tests/phase1/TestMethodNameTests.cs
  - tests/phase1/TestSourceIndex.cs
  - tests/phase1/ThresholdTests.cs
  - tests/phase1/TypeBoundaryScanner.cs
  - tests/phase1/Verification/AdapterTargets.cs
  - tests/phase1/Verification/AnalyzerVerifier.cs
  - tests/phase1/Verification/InheritedAnalyzerVerifier.cs
  - tests/phase1/docs/README.md
  - tools/code-rules/AnalyzerReleases.Shipped.md
  - tools/code-rules/AnalyzerReleases.Unshipped.md
  - tools/code-rules/BodyCommentAnalyzer.cs
  - tools/code-rules/BodyLengthAnalyzer.cs
  - tools/code-rules/ContinuationIndentAnalyzer.cs
  - tools/code-rules/EngineGeneratedSyntax.cs
  - tools/code-rules/GeneratedLayout.cs
  - tools/code-rules/GeneratedSyntax.cs
  - tools/code-rules/LineLengthAnalyzer.cs
  - tools/code-rules/LocalNewDeclarationAnalyzer.cs
  - tools/code-rules/MemberBodies.cs
  - tools/code-rules/MemberOrderAnalyzer.cs
  - tools/code-rules/NamedArgumentAnalyzer.cs
  - tools/code-rules/NullForgivingAnalyzer.cs
  - tools/code-rules/PrimaryConstructorAnalyzer.cs
  - tools/code-rules/RegionDirectiveAnalyzer.cs
  - tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj
  - tools/code-rules/ThisQualificationAnalyzer.cs
  - tools/code-rules/UsingOrderAnalyzer.cs
  - tools/code-rules/docs/README.md
  - tools/phase1/bootstrap.cjs
  - tools/phase1/check.cjs
  - tools/phase1/projects.json
  - tools/phase1/required-cases.json
  - tools/phase1/toolchain.json
findings:
  critical: 2
  warning: 2
  info: 0
  total: 4
  necessity: 0
status: issues_found
---

# Phase 1: Code Review Report

**Reviewed:** 2026-11-14T17:33:08Z
**Revision:** `6d208727543eb462f3495515f9928acd8075a0e3`
**Depth:** standard
**Files Reviewed:** 182
**Status:** issues_found

## Summary

The supplied scope contains 188 existing paths. This report covers its 182 reviewable source, configuration and documentation paths; the five dependency locks and immutable donor ZIP were inspected for dependency/provenance context and excluded from the source count. The historical deleted `src/godot-game/GeneratedLifecycleAdapters.cs` is not a current deliverable. The supplied explicit scope was used directly, rather than the narrower SDK phase evaluation.

The findings concern enforcement behavior: active conditional branches escape source-policy checks, method groups and implicit property references escape the engine operation scan, the visibility proof has an incomplete actual-source inventory, and the R-16 negative proof fails for an unrelated undefined type. These are defects in required checks, even though the current native fixture's ordinary calls are guarded. No current fixture crash, snapshot corruption, secret exposure or dependency-acquisition exploit was established.

Review followed the actual Core, Godot, analyzer, runner and policy call paths, inspected their configuration and declared fixture/permission inventories, and checked C# fixture syntax and asynchronous verification call structure. All 170 engine permission table entries were reconciled across their complete diagnostic/path/text-hash/span fields; all 85 current rule bindings, 678 case declarations and 151 captured-file metadata rows were inspected. The 670 implemented declarations map uniquely to their actual source trait/method identities, and the 290 Task-returning verification wrappers return their tasks. These inspections are review context rather than proof of overall correctness. Focused demonstrations invoked existing policy code through reflection and compiled independent subjects in memory using the actual .NET/Godot references. They changed no source or tests. No new full Windows or Linux suite was run. An apparent missing-import concern was rejected after the actual isolated locked solution restore succeeded in the permitted process context.

The creator's 2026-11-14 acceptance amendment is respected. The eight named Phase 5 aggregate/coverage obligations, nine known Linux canonical timeouts, historical missing initial RED evidence, and fourteen Pending human components are separate recorded acceptance conditions, not new source findings here. This report supplies no human qualitative approval. Necessity was assessed using the repository rubric; no evidence-backed necessity defect was found. No structural pre-pass or external reviewer evidence was supplied.

## Narrative Findings (AI reviewer)

## Critical Issues

### CR-01: Active consumer preprocessor branches bypass engine and suppression policies

**Classification:** BLOCKER
**File:** `D:/Work/Games/KingdomZombies/tests/phase1/EngineAccessPolicy.cs:17`
**Related locations:** `D:/Work/Games/KingdomZombies/tests/phase1/EngineAccessPolicy.cs:116`; `D:/Work/Games/KingdomZombies/tests/phase1/ExemptionValidator.cs:20`; `D:/Work/Games/KingdomZombies/tests/phase1/EngineBoundaryTests.cs:36`
**Issue:** Both the single-subject and repository paths reparse C# with default parse options. The real game consumer defines symbols including `DEBUG`, `GODOT_WINDOWS` and `TOOLS`; the checker supplies none of them. Consequently, code under an active consumer branch such as `#if DEBUG` becomes disabled text in the checker's tree and is never examined. A forbidden engine call can compile into the actual Debug game while the required engine source check reports no violation.

A concrete subject is:

```csharp
#if DEBUG
namespace KingdomZombies.Proofs;
internal static class Subject
{
    internal static void Run(Godot.Node node)
    {
        node.QueueFree();
    }
}
#endif
```

The actual evaluated game `DefineConstants` includes `DEBUG`. In the focused demonstration, this subject had **zero compiler errors** with matching consumer parse symbols and **zero errors from `EngineAccessPolicy.Check`**. The same unwrapped call produced `ENGINE_ACCESS`. Passing the conditional tree parsed with the matching symbols to the existing `CheckTree` produced **one `ENGINE_ACCESS`**, independently isolating the parse-option mismatch. This is a concrete false acceptance, not a hypothesis about a future platform.

The same default-parser defect exists in `ExemptionValidator.CheckSource`. Wrapping an unregistered assembly `SuppressMessage` attribute in `#if DEBUG` hides it from the required suppression scan while the actual Debug compiler still applies it. A focused subject used this exact attribute inside the branch:

```csharp
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Sebreiro.CodeRules", "SCR0001", Scope = "member",
    Target = "~M:KingdomZombies.Core.Subject.Read",
    Justification = "unregistered")]
```

Its `Subject.Read` returned `System.Math.Abs(-1)`. With `DEBUG` defined, the subject had **zero compiler errors**, **zero diagnostics from the actual NamedArgumentAnalyzer** because the suppression took effect, and **zero errors from `ExemptionValidator.CheckSource`**. Removing only the surrounding conditional directives made the same attribute produce **one unregistered-suppression error**. Thus an undeclared suppression can defeat both an enforced analyzer and its required source guard.

**Fix:** Obtain the selected consumer's evaluated parse inputs, including its complete `DefineConstants` and language version, and use them consistently in the engine policy, suppression policy and other semantic source policies when parsing every actual handwritten and generated tree and every replacement subject. Prefer inspecting the actual consumer compilation rather than reconstructing a different one. Reject unresolved compilation prerequisites instead of accepting an incomplete semantic view. Add focused controls for an active `DEBUG` branch and an active evaluated Godot/platform symbol, alongside their inactive counterparts, including a conditional unregistered suppression that the actual analyzer would otherwise honor. Preserve existing exact generated permissions and the current test deadlines.

### CR-02: Method groups and implicit native property references bypass the engine operation scan

**Classification:** BLOCKER
**File:** `D:/Work/Games/KingdomZombies/tests/phase1/EngineAccessPolicy.cs:30`
**Related location:** `D:/Work/Games/KingdomZombies/tests/phase1/EngineAccessPolicy.cs:34`
**Issue:** The checker examines only invocation, object creation and explicit member-access syntax, then explicitly skips member accesses whose symbol is a method. A Godot method converted into a delegate therefore escapes inspection. Invoking that delegate through `Task.Run` resolves to the System task API rather than to Godot, so neither the conversion nor the worker-thread native invocation is rejected.

This compilable subject is accepted with no engine policy errors:

```csharp
namespace KingdomZombies.Proofs;
internal static class Subject
{
    internal static System.Threading.Tasks.Task Run(Godot.Node node)
    {
        System.Action work = node.QueueFree;
        return System.Threading.Tasks.Task.Run(action: work);
    }
}
```

The focused demonstration compiled this subject against the actual references with **zero compiler errors** and observed **zero `EngineAccessPolicy.Check` errors**; the equivalent direct `node.QueueFree()` call was rejected. The returned task executes an unguarded native operation on a worker and never passes through `MainThreadGateway`.

The same restricted syntax inventory also misses inherited native property references written as bare identifiers. A `Godot.Node` subclass method returning `Name` compiled with **zero compiler errors** and produced **zero policy errors**, although it performs an unguarded Godot property read outside the declared adapters.

**Fix:** Inspect semantic operations, including native `IMethodReferenceOperation`, `IPropertyReferenceOperation` and `IFieldReferenceOperation`, rather than relying on this limited syntax-node list. Reject escaping native method groups; checking the thread only when a delegate is captured would not protect its later execution. Require deferred work to call a wrapper that performs the existing guard at invocation/dispatch time. Add focused negative controls for a native method group handed to `Task.Run` and an unqualified inherited native property read, while retaining the valid guarded direct-call controls.

## Warnings

### WR-01: The visibility proof checks four fixed files instead of the production subject inventory

**Classification:** WARNING
**File:** `D:/Work/Games/KingdomZombies/tests/phase1/ArchitectureRuleTests.cs:16`
**Related location:** `D:/Work/Games/KingdomZombies/tests/phase1/ArchitecturePolicy.cs:16`
**Issue:** The actual R-34 positive proof sends only `ProofValue.cs`, `IProofService.cs`, `Phase1.cs` and the test `BoundaryEntity.cs` to the visibility checker. The only other visibility call is the isolated negative fixture. No repository-wide caller supplies the remaining production files. An undeclared public type in another existing production file therefore lies outside the required visibility proof, despite the exact three-type permission registry.

For example, changing only `internal sealed class ExampleState` to `public sealed class ExampleState` is outside those four paths. An in-memory compilation of the actual Core sources with that change had **zero compiler errors**, and the existing R-63 model policy returned **zero errors**. Calling the visibility helper directly on that same source correctly returned `R-34: undeclared public type KingdomZombies.Core.ExampleState in src/core/ExampleState.cs`; the actual positive proof never makes that call. Thus the helper can identify the violation, but its repository invocation does not cover the source subject.

**Fix:** Enumerate the selected production consumers' actual authored compile inputs and apply the exact visibility policy to every relevant type declaration. Keep tooling/test-public surfaces and genuine generated outputs under their existing explicit scopes rather than granting a blanket waiver. Add a controlled public-type mutation in an existing production file outside the current list and require the actual repository scan to reject it. The separate human judgment about whether a permitted API is necessary remains Pending.

### WR-02: The implicit-usings negative proof succeeds for an unrelated missing type

**Classification:** WARNING
**File:** `D:/Work/Games/KingdomZombies/tests/phase1/EnforcementTests.cs:149`
**Related locations:** `D:/Work/Games/KingdomZombies/docs/code/register.json:816`; `D:/Work/Games/KingdomZombies/tests/phase1/ConfigurationTests.cs:630`
**Issue:** R-16 is bound to `<ImplicitUsings>disable</ImplicitUsings>`, but its independent negative proof returns the undefined type `Missing` and accepts CS0246 at that declaration. `Missing` is undefined regardless of whether SDK implicit usings are enabled. The test therefore passes after an unrelated missing-type failure and does not establish the claimed rule behavior.

A focused compilation of the exact subject returned the same `Subject.cs(6,21): error CS0246` for `Missing` both without implicit globals and with the SDK's complete implicit global-using set. The existing effective-property configuration check still requires `ImplicitUsings=disable`; the defect is specifically the reliability and intended-subject meaning of this required negative case.

**Fix:** Use an unqualified existing framework type, such as `Uri`, whose namespace is supplied by SDK implicit usings. Require that exact subject/location to fail with implicit usings disabled, and prove in an isolated consumer that enabling only implicit usings makes the same subject compile. Keep a compliant explicitly qualified/imported positive control. Do not use an invented type to supply the negative diagnostic.

---

_Reviewed: 2026-11-14T17:33:08Z_
_Reviewer: the agent (gsd-code-reviewer)_
_Depth: standard_

