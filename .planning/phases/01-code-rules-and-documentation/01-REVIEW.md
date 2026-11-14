---
phase: 01-code-rules-and-documentation
reviewed: 2026-11-14T18:22:19Z
amended: 2026-11-14T21:28:56Z
revision: 7d9fa3f11d3e7e6df335b85b085dd339c5e9f6c9
prior_review_revision: 5bb1a8d784f652fc05f0c768c93aa93645bcc3d4
depth: standard
iteration: 1
files_reviewed: 185
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
  - tests/phase1/ConsumerCompilation.cs
  - tests/phase1/ConsumerPolicyTests.cs
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
  - tests/phase1/EngineOperationTests.cs
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
  critical: 0
  warning: 0
  info: 0
  total: 0
  necessity: 0
status: clean
---

# Phase 1: Code Review Report

**Reviewed:** 2026-11-14T18:22:19Z  
**Original followup revision:** `5bb1a8d784f652fc05f0c768c93aa93645bcc3d4`

**Amended:** 2026-11-14T21:28:56Z at `7d9fa3f11d3e7e6df335b85b085dd339c5e9f6c9`

**Depth:** standard  
**Files Reviewed:** 185  
**Status:** clean

## Summary

All four findings from the initial review are resolved. The independent followup inspected the actual four repair commits and the complete changes from `0276ea8` through the reviewed revision, read the three new source files, and traced the new compiler-input reader through its engine, suppression, random, service-locator and visibility callers. No remaining or new evidence-backed defect was found. All reviewed files meet quality standards. No issues found.

The 182-file initial standard review is retained for unchanged content. The exact scope above adds `ConsumerCompilation.cs`, `ConsumerPolicyTests.cs` and `EngineOperationTests.cs`, giving 185 reviewed source/configuration/documentation files. The original supplied 188 existing paths also included five dependency locks and an immutable donor ZIP, inspected as dependency/provenance context rather than counted as source. The historical deleted `src/godot-game/GeneratedLifecycleAdapters.cs` remains outside current deliverables. This followup reused the completed review of unchanged files instead of rereading unchanged large tables.

The original findings, exact locations and independent false-acceptance demonstrations are preserved in the committed initial report at `0276ea8:.planning/phases/01-code-rules-and-documentation/01-REVIEW.md`. The [repair report](01-REVIEW-FIX.md) preserves the applied fixes and their earlier focused RED/GREEN records. This report replaces the active finding counts after independent verification; it does not erase the initial evidence.

## Narrative Findings (AI reviewer)

No active findings. Necessity was assessed with the repository rubric; no evidence-backed necessity defect was found. No structural pre-pass or external reviewer lane was supplied.

## Verified dispositions of initial findings

| Original finding | Fix commit | Independent disposition |
| --- | --- | --- |
| CR-01: Active consumer branches bypass engine and suppression policies | `330a42750499090303c566eb10ed5152b981d725` | Resolved. Selected consumer evaluation supplies actual Debug symbols, language version, Compile inventory and resolved references. |
| CR-02: Native method groups and implicit inherited member references escape the engine scan | `8ebbc1d030a7da1ba8d02583047b7cd5163ec373` | Resolved. Semantic operations include method references, bare properties and fields; native captures are rejected even after a capture-time guard. |
| WR-01: Visibility proof checks only four fixed files | `3da7114f28875b6db9bcc0ab8528916ad409deee` | Resolved. R-34's actual positive invokes the repository scan over all three production consumers' evaluated authored Compile inputs. |
| WR-02: Implicit-usings negative fails for an unrelated missing type | `5c52a20c9fdabe9fad085e97333ca65d1ec8b219` | Resolved. The negative uses real unqualified `Uri`; identical bytes change from rejected to compiled to rejected solely with the consumer's disable/enable/disable setting. |

For CR-01, `ConsumerCompilation.Read` evaluates `ResolveReferences`, `DefineConstants`, `LangVersion`, `Compile` and `ReferencePath` in the selected consumer and explicit root. Context is local to one scan, with no global evaluation cache; source contents are read afresh by `Create`. Replacement and emitted companion trees use the same parse options. Compiler errors fail the engine scan closed before unresolved operations can disappear. The repository suppression path requires exactly one evaluated owner for each tracked checked C# file, and unresolved attribute identities fail closed. The new controls require the actual `DEBUG`, `TOOLS` and platform symbols, reject active forbidden calls, accept their inverse inactive branches, and demonstrate an active unregistered suppression silencing the actual analyzer while the source policy rejects it.

For CR-02, inspection confirmed the policy obtains native method/property/field identities from resolved Roslyn operations. A native method reference is rejected regardless of adapter permission, while a deferred authored wrapper remains subject to its own invocation-time guard. Direct-call permissions retain the exact path, namespace, type and method identities, preceding direct guard, and existing lambda/local-function/async exclusions. Fresh compiling subjects cover both captured and directly supplied `Task.Run` method groups, a capture inside the actual guarded adapter, a valid authored wrapper, a valid guarded direct call, bare inherited `Name` and `NotificationReady` references.

For WR-01, the repository mechanism reads actual authored Compile items with each owner's explicit parse options; the exact three-type visibility registry remains unchanged. The fresh copied real consumer first compiles cleanly, then successfully compiles the existing `ExampleState.cs` made public under active `DEBUG` and receives the exact R-34 rejection. Its inverse inactive public declaration compiles semantically and is accepted. The register, matrix and independent binding definition point to `VisibilityRepository`.

For WR-02, the same subject bytes declare `Uri` at `Subject.cs(6,21)`. The new proof establishes the real copied Core consumer and subject-only inventory before its baseline; its analyzer bootstrap and severity settings remain enabled. It requires that exact CS0246 with implicit usings disabled, requires a successful build when only the Core XML setting enables them, and requires the same error after restoring disable. The positive explicitly qualifies `System.Uri`. The earlier unrelated IDE0005 attempt is not credited as behavior RED.

## Independent focused verification

All six existing regression cases were rerun through the canonical runner on Windows at revision `5bb1a8d784f652fc05f0c768c93aa93645bcc3d4`, input fingerprint `14ee4dbb6a303ab968e343bae54378606d2a951aaead8714b5f0ba42fae23359`. Each invocation accepted 1/1 selected obligation, reported no failure or skip, and had no nonzero/timed-out recorded command. The actual test reports and nonempty subject logs were inspected; their twelve SHA-256 entries were independently verified.

| Exact case selector | Result | Canonical report |
| --- | --- | --- |
| `engine.consumer-branches` | Pass 1/1 | [de249278-a196-44f0-8c6d-3ac1d5a65127](../../../.phase1/runs/de249278-a196-44f0-8c6d-3ac1d5a65127/summary.json) |
| `rules.suppression-conditional` | Pass 1/1 | [c1312a58-1e13-4749-91bd-5e83d723c6c9](../../../.phase1/runs/c1312a58-1e13-4749-91bd-5e83d723c6c9/summary.json) |
| `engine.method-escape` | Pass 1/1 | [1c9ba8f3-0013-4a5d-a141-01295bf12970](../../../.phase1/runs/1c9ba8f3-0013-4a5d-a141-01295bf12970/summary.json) |
| `engine.inherited-members` | Pass 1/1 | [9a7cf307-d56e-4841-9810-77e4f8937dd8](../../../.phase1/runs/9a7cf307-d56e-4841-9810-77e4f8937dd8/summary.json) |
| `graph.visibility-inventory` | Pass 1/1 | [88042de4-9bb7-473b-9722-b7156dbcb680](../../../.phase1/runs/88042de4-9bb7-473b-9722-b7156dbcb680/summary.json) |
| `rules.implicit-usings-control` | Pass 1/1 | [fb0cbb27-570e-4dbf-9736-0e3ea51bbf24](../../../.phase1/runs/fb0cbb27-570e-4dbf-9736-0e3ea51bbf24/summary.json) |

An independent complete-object comparison with `0276ea8` confirmed all 678 original manifest declarations are unchanged, with exactly these six additions. The current manifest therefore retains 676 required Phase 1 obligations and eight explicitly deferred Phase 5 cases, for 684 declarations. No permission registry, suppression allowance, deadline, threshold or previous case declaration was weakened by the repair diff.

At the original followup timestamp, no new complete Windows or Linux suite had been run by this reviewer; the full `f4658bb` reports were historical after the four repairs. Its then-current full-platform acceptance boundary is superseded for Phase 1 by the creator's later exception recorded below. This code review provides no human qualitative sign-off and advances no phase-completion flag.

## Narrow Linux platform-symbol amendment

**Verdict:** clean; zero new critical, warning or info findings. The existing 185-file scope and completed assessment are retained. This amendment independently inspected the complete C# diff from `59f032c` to `70918fd` and confirmed that the only C# change through current `7d9fa3f` is `tests/phase1/ConsumerPolicyTests.cs:23`: adding `GODOT_LINUXBSD` alongside the existing Windows/Linux names in the platform-symbol selector. Production implementation, compiler-input helpers, policy rules, manifests, locks, assertions and deadlines are unchanged. The active forbidden-call, inverse inactive-branch and unresolved-subject controls still execute against the selected consumer; this correction makes the Linux control select the platform symbol actually supplied by the pinned SDK.

The original actual SDK evaluation at `.phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/actual-linux-defines.json` explicitly supplies `GODOT_LINUXBSD`, `DEBUG` and `TOOLS`, target `linuxbsd`, Debug configuration and Unix OS. It supplies neither of the formerly assumed platform names. Thus the added alternative repairs the demonstrated test setup failure without relaxing the required engine-operation rejection.

The actual changed-case reports were inspected on both OSes. Windows canonical invocation `66a74d57-ecc3-4d79-b1b8-0f27bf41e6d9` accepts exactly `engine.consumer-branches`, with one passed test and zero failed/pending/skipped tests. Its immutable report retains genuine precommit workspace revision `266a6d87c72477d77dee8beaa8283ae0bce4d441`. Linux direct amendment `b4ccec5f-255f-466f-bea2-f807b260ccf8` records source `70918fdb77cff309527b407418083e52db89378e`, the same exact test passing with zero failed/pending/skipped tests, process exit zero, and the explicit 8-minute test/10-minute outer bounds. Both use input `2e16dd414d42f53cba37707e629c9110fe64164ddb8e96636c878c014419c15f`; their proofs identify actual `DEBUG/TOOLS/GODOT_WINDOWS` and `DEBUG/TOOLS/GODOT_LINUXBSD` respectively, including inverse branches and the unresolved subject. The changed C# file's current SHA-256 matches the Linux input inventory. Independent checks reverified the two Windows report/evidence hashes, the two Linux report/evidence hashes and all six Linux amendment command-log hashes; all recorded amendment commands exited zero without timeout. Linux receipt SHA-256 is `95d57c2c9f183e530382dcf8e64f255f32e483423e16473ae502f5a45f4e6733`. The copied Linux originals are under `.phase1/temp/17-linux-smoke/amendment-evidence/.phase1/amendment/b4ccec5f-255f-466f-bea2-f807b260ccf8/`.

**Evidence and acceptance limits:** The complete Windows baseline remains 676/676 at `59f032cf23340058eec91f12c5d240f585dfabdc`, nonce `b8b5eacc-c46a-42d7-a898-01ad33c1b0a8`, plus the explicit one-case amendment above. No new complete Windows run at `70918fd` or current HEAD is claimed. The creator's later [Phase 1 closure exception](01-CLOSURE-EXCEPTION.md) authorizes important actual Linux checks without a host restart and preserves the general full-other-OS-VM rule for later phases. The recorded Linux assurance combines strict setup/native checks, five unchanged case passes, this corrected case, and separately labeled direct graph-mechanism evidence. The original graph case remains failed/unaccepted; the direct mechanism proof is not a replacement canonical case pass. The complete Linux suite and fourteen individually unreviewed human outcomes remain visible [closure follow-ups](01-CLOSURE-FOLLOWUPS.md), with no test or human pass fabricated. Eight Phase 5 coverage/aggregate cases, inclusive 90/80 targets and advanced certification retain their ownership. No tests, full suites, host/VM actions or human judgments were performed by this bounded reviewer amendment. The [platform record](../../../docs/workflow/platform-proof.md) preserves the composed evidence identities and limits.

---

_Reviewed: 2026-11-14T18:22:19Z_  
_Reviewer: the agent (gsd-code-reviewer)_  
_Depth: standard_

