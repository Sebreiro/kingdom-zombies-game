---
phase: 01-code-rules-and-documentation
verified: 2026-11-14T21:39:24Z
status: passed
score: 16/16 current amended must-haves verified
covered_files: [".config/dotnet-tools.json",".editorconfig",".phase1/runs/1aba94e6-5790-4409-a1b9-a2a2e0f1f722/summary.json",".phase1/runs/5ee9d083-b818-4600-a7b3-4227112af20b/summary.json",".phase1/runs/66a74d57-ecc3-4d79-b1b8-0f27bf41e6d9/engine.consumer-branches/engine.consumer-branches.subject.log",".phase1/runs/66a74d57-ecc3-4d79-b1b8-0f27bf41e6d9/engine.consumer-branches/tests.json",".phase1/runs/66a74d57-ecc3-4d79-b1b8-0f27bf41e6d9/inputs.json",".phase1/runs/66a74d57-ecc3-4d79-b1b8-0f27bf41e6d9/summary.json",".phase1/runs/c8038d4d-2cc1-4f0c-82dc-f6b6c541df65/ac-10.complete/ac-10.complete.subject.log",".phase1/runs/c8038d4d-2cc1-4f0c-82dc-f6b6c541df65/ac-10.complete/tests.json",".phase1/runs/c8038d4d-2cc1-4f0c-82dc-f6b6c541df65/inputs.json",".phase1/runs/c8038d4d-2cc1-4f0c-82dc-f6b6c541df65/summary.json",".phase1/temp/17-final-preparation/docs-reconciliation.json",".phase1/temp/17-final-preparation/final-reconciliation.json",".phase1/temp/17-final-preparation/linux-prepared-inputs.json",".phase1/temp/17-final-preparation/linux-prepared.json",".phase1/temp/17-final-preparation/vm-stopped.json",".phase1/temp/17-linux-smoke/17-final-smoke-evidence.tar.gz",".phase1/temp/17-linux-smoke/17-focused-amendment-evidence.tar.gz",".phase1/temp/17-linux-smoke/amendment-evidence/.phase1/amendment/b4ccec5f-255f-466f-bea2-f807b260ccf8/amendment-receipt.json",".phase1/temp/17-linux-smoke/amendment-evidence/.phase1/amendment/b4ccec5f-255f-466f-bea2-f807b260ccf8/engine.consumer-branches/tests.json",".phase1/temp/17-linux-smoke/amendment-evidence/.phase1/direct-graph/6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365/allowed.json",".phase1/temp/17-linux-smoke/amendment-evidence/.phase1/direct-graph/6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365/controlled-mutations.json",".phase1/temp/17-linux-smoke/amendment-evidence/.phase1/direct-graph/6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365/copied-source-inventory.json",".phase1/temp/17-linux-smoke/amendment-evidence/.phase1/direct-graph/6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365/forbidden.json",".phase1/temp/17-linux-smoke/amendment-evidence/.phase1/direct-graph/6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365/graph-host-source.cs",".phase1/temp/17-linux-smoke/amendment-evidence/.phase1/direct-graph/6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365/graph-mechanism.json",".phase1/temp/17-linux-smoke/amendment-evidence/.phase1/direct-graph/6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365/host-identity.json",".phase1/temp/17-linux-smoke/amendment-inspection.json",".phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/R-38.negative/tests.json",".phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/R-38.positive/tests.json",".phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/actual-linux-defines.json",".phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/engine.consumer-branches/tests.json",".phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/graph.allowed-and-direct/tests.json",".phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/rules.suppression-conditional/tests.json",".phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/smoke-receipt.json",".phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/state.restore/tests.json",".phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/state.snapshot/tests.json",".phase1/temp/17-linux-smoke/graph-host.cs",".phase1/temp/17-linux-smoke/original-inspection.json",".phase1/temp/17-linux-smoke/vm-shutdown-ssh.exit",".phase1/temp/17-linux-smoke/vm-stopped.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/ac-10.complete/ac-10.complete.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/ac-10.complete/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/docs.projects/docs.projects.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/docs.projects/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/edge.rule-07.concurrency/edge.rule-07.concurrency.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/edge.rule-07.concurrency/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.consumer-branches/engine.consumer-branches.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.consumer-branches/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.exports/engine-invalid-null-7683b268-8759-4ac6-a06c-233223dc5f15.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.exports/engine-missing-reference-85cc3c29-23f8-464b-b20e-7f99fafeafc2.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.exports/engine-missing-value-c1022a6d-8455-4de2-a502-61b47acc86f8.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.exports/engine-valid-5b9bf4d3-f29d-4265-bd45-7ef26813988e.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.exports/engine.exports.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.exports/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.inherited-members/engine.inherited-members.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.inherited-members/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.lifecycle/engine-valid-7cd7cc12-4c9d-4276-ba6a-594e019e4686.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.lifecycle/engine.lifecycle.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.lifecycle/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.method-escape/engine.method-escape.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/engine.method-escape/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/graph.visibility-inventory/graph.visibility-inventory.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/graph.visibility-inventory/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/inputs.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/register.binding-precoverage/register.binding-precoverage.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/register.binding-precoverage/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/register.capture/register.capture.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/register.capture/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/rules.build-wiring/rules.build-wiring.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/rules.build-wiring/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/rules.implicit-usings-control/rules.implicit-usings-control.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/rules.implicit-usings-control/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/rules.suppression-conditional/rules.suppression-conditional.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/rules.suppression-conditional/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/runner.phase-membership/runner.phase-membership.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/runner.phase-membership/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/runner.process.concurrent/runner.process.concurrent.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/runner.process.concurrent/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/runner.process.interrupted/runner.process.interrupted.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/runner.process.interrupted/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/runner.process.timeout/runner.process.timeout.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/runner.process.timeout/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/state.golden-vectors/state.golden-vectors.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/state.golden-vectors/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/state.restore-cancellation/state.restore-cancellation.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/state.restore-cancellation/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/state.restore-mutation/state.restore-mutation.subject.log",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/state.restore-mutation/tests.json",".phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/summary.json",".phase1/temp/17-windows-59f032c/.phase1/temp/17-bootstrap.exit",".phase1/temp/17-windows-59f032c/.phase1/temp/17-bootstrap.log",".phase1/temp/17-windows-59f032c/.phase1/temp/17-build.exit",".phase1/temp/17-windows-59f032c/.phase1/temp/17-build.log",".phase1/temp/17-windows-59f032c/.phase1/temp/17-full.exit",".phase1/temp/17-windows-59f032c/.phase1/temp/17-full.log",".phase1/temp/17-windows-59f032c/.phase1/temp/17-restore.exit",".phase1/temp/17-windows-59f032c/.phase1/temp/17-restore.log",".planning/PROJECT.md",".planning/REQUIREMENTS.md",".planning/ROADMAP.md",".planning/STATE.md",".planning/config.json",".planning/phases/01-code-rules-and-documentation/01-01-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-01-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-02-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-02-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-03-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-03-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-04-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-04-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-05-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-05-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-06-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-06-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-07-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-07-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-08-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-08-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-09-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-09-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-10-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-10-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-11-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-11-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-12-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-12-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-13-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-13-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-14-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-14-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-15-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-15-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-16-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-16-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-17-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-17-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-18-PLAN.md",".planning/phases/01-code-rules-and-documentation/01-18-SUMMARY.md",".planning/phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md",".planning/phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md",".planning/phases/01-code-rules-and-documentation/01-CONTEXT.md",".planning/phases/01-code-rules-and-documentation/01-ENVIRONMENT.md",".planning/phases/01-code-rules-and-documentation/01-HUMAN-CHECK.md",".planning/phases/01-code-rules-and-documentation/01-RESTART-CHECKPOINT.md",".planning/phases/01-code-rules-and-documentation/01-REVIEW-DISPOSITION.md",".planning/phases/01-code-rules-and-documentation/01-REVIEW-FIX.md",".planning/phases/01-code-rules-and-documentation/01-REVIEW.md",".planning/phases/01-code-rules-and-documentation/01-SECURITY.md",".planning/phases/01-code-rules-and-documentation/01-SPEC.md",".planning/phases/01-code-rules-and-documentation/01-UI-REVIEW.md",".planning/phases/01-code-rules-and-documentation/01-UI-SPEC.md",".planning/phases/01-code-rules-and-documentation/01-VALIDATION.md",".planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md",".planning/research/SUMMARY.md","AGENTS.md","BannedSymbols.Pure.txt","BannedSymbols.Tests.txt","BannedSymbols.txt","Directory.Build.props","Directory.Build.targets","Directory.Packages.props","Directory.Xunit.targets","NuGet.config","docs/architecture/README.md","docs/architecture/boundaries.md","docs/architecture/decisions/0001-development-baseline.md","docs/architecture/decisions/0001-enforcement.md","docs/architecture/decisions/0002-boundaries.md","docs/architecture/decisions/0003-state-and-engine.md","docs/code/README.md","docs/code/engine-adaptations.json","docs/code/engine-generated-spans.json","docs/code/exemptions.json","docs/code/generated-callsite-adaptations.json","docs/code/generated-layout-adaptations.json","docs/code/generated-syntax-adaptations.json","docs/code/provenance/analyzer-adoption.json","docs/code/provenance/langproj-baseline.zip","docs/code/provenance/manifest.json","docs/code/provenance/xunit-entrypoint-path-adaptation.json","docs/code/register.json","docs/code/review-checklist.md","docs/code/review.md","docs/code/rule-matrix.md","docs/code/visibility-exemptions.json","docs/workflow/ai-collaboration.md","docs/workflow/gsd.md","docs/workflow/platform-proof.md","docs/workflow/setup.md","docs/workflow/verification.md","global.json","specs/0.1/assets-and-resources.md","specs/0.1/initial-spec.md","src/core/ExampleState.cs","src/core/ExampleStateStore.cs","src/core/IElapsedInput.cs","src/core/IRandomInput.cs","src/core/KingdomZombies.Core.csproj","src/core/ProofKind.cs","src/core/ProofValue.cs","src/core/ReplayRandom.cs","src/core/SnapshotCodec.cs","src/core/SnapshotDto.cs","src/core/SnapshotJson.cs","src/core/SnapshotWriter.cs","src/core/StableIdentity.cs","src/core/docs/README.md","src/core/packages.lock.json","src/godot-game/.editorconfig","src/godot-game/GeneratedCallSiteAdapters.cs","src/godot-game/GeneratedReportAdapter.cs","src/godot-game/KingdomZombies.csproj","src/godot-game/docs/README.md","src/godot-game/icon.svg","src/godot-game/icon.svg.import","src/godot-game/packages.lock.json","src/godot-game/project.godot","src/godot-game/proofs/InvalidFixture.tscn","src/godot-game/proofs/LifecycleFixture.cs","src/godot-game/proofs/LifetimeToken.cs","src/godot-game/proofs/MainThreadGateway.cs","src/godot-game/proofs/Phase1.cs","src/godot-game/proofs/Phase1.tscn","src/godot-game/proofs/ProofResource.cs","src/godot-game/proofs/ProofResource.tres","src/godot-game/proofs/ProofSequence.cs","src/godot-game/proofs/ValidFixture.tscn","src/multiplayer-abstractions/IProofService.cs","src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj","src/multiplayer-abstractions/docs/README.md","src/multiplayer-abstractions/packages.lock.json","tests/phase1/AnalyzerInputIdentityTests.cs","tests/phase1/AnalyzerSourceConsumerTests.cs","tests/phase1/Analyzers/BodyCommentAnalyzerTests.cs","tests/phase1/Analyzers/BodyLengthAnalyzerTests.cs","tests/phase1/Analyzers/ContinuationIndentAnalyzerTests.cs","tests/phase1/Analyzers/LineLengthAnalyzerTests.cs","tests/phase1/Analyzers/LocalNewDeclarationAnalyzerTests.cs","tests/phase1/Analyzers/MemberOrderAnalyzerTests.cs","tests/phase1/Analyzers/NamedArgumentAnalyzerTests.cs","tests/phase1/Analyzers/NullForgivingAnalyzerTests.cs","tests/phase1/Analyzers/PrimaryConstructorAnalyzerTests.cs","tests/phase1/Analyzers/RegionDirectiveAnalyzerTests.cs","tests/phase1/Analyzers/ThisQualificationAnalyzerTests.cs","tests/phase1/Analyzers/UsingOrderAnalyzerTests.cs","tests/phase1/ArchitecturePolicy.cs","tests/phase1/ArchitectureRuleTests.cs","tests/phase1/BoundaryFixture.cs","tests/phase1/BuildFixture.cs","tests/phase1/ConfigurationTests.cs","tests/phase1/ConsumerCompilation.cs","tests/phase1/ConsumerPolicyTests.cs","tests/phase1/ControlledElapsed.cs","tests/phase1/ControlledInputPolicy.cs","tests/phase1/ControlledRandom.cs","tests/phase1/DocumentationAnchors.cs","tests/phase1/DocumentationDecisions.cs","tests/phase1/DocumentationFixture.cs","tests/phase1/DocumentationLinks.cs","tests/phase1/DocumentationPath.cs","tests/phase1/DocumentationProjectTests.cs","tests/phase1/DocumentationProjects.cs","tests/phase1/DocumentationTests.cs","tests/phase1/DocumentationValidator.cs","tests/phase1/EnforcementTests.cs","tests/phase1/EngineAccessPolicy.cs","tests/phase1/EngineBoundaryTests.cs","tests/phase1/EngineCaseTests.cs","tests/phase1/EngineEvidencePolicy.cs","tests/phase1/EngineEvidenceTests.cs","tests/phase1/EngineGeneratedTests.cs","tests/phase1/EngineOperationTests.cs","tests/phase1/EngineRun.cs","tests/phase1/ExemptionTests.cs","tests/phase1/ExemptionValidator.cs","tests/phase1/GeneratedCallSiteAdapterTests.cs","tests/phase1/GeneratedCallSiteAdapters.cs","tests/phase1/GeneratedLayoutAdapterTests.cs","tests/phase1/GeneratedSurfaceScanner.cs","tests/phase1/GeneratedSyntaxAdapterTests.cs","tests/phase1/GodotCallSiteAdapterTests.cs","tests/phase1/GraphFixture.cs","tests/phase1/GraphTests.cs","tests/phase1/KingdomZombies.Phase1.Tests.csproj","tests/phase1/Models/BoundaryEntity.cs","tests/phase1/Models/BoundaryKind.cs","tests/phase1/Models/BoundaryMapper.cs","tests/phase1/Models/BoundaryOptions.cs","tests/phase1/Models/BoundarySnapshotDto.cs","tests/phase1/PinEdgeTests.cs","tests/phase1/PinsTests.cs","tests/phase1/ProjectEvaluator.cs","tests/phase1/ProofProtocol.cs","tests/phase1/RandomContractTests.cs","tests/phase1/RegisterTests.cs","tests/phase1/RegisterValidator.cs","tests/phase1/RestoreAtomicityTests.cs","tests/phase1/RestoreConcurrencyProof.cs","tests/phase1/RestoreMutationProof.cs","tests/phase1/RuleBindingTests.cs","tests/phase1/RuleBindingValidator.cs","tests/phase1/RuleDefinitions.cs","tests/phase1/RuleMatrix.cs","tests/phase1/RunnerContractTests.cs","tests/phase1/RunnerProcessFixture.cs","tests/phase1/SnapshotProofData.cs","tests/phase1/SnapshotShapePolicy.cs","tests/phase1/SnapshotTests.cs","tests/phase1/SourceFileFormat.cs","tests/phase1/StateInputTests.cs","tests/phase1/SuppressionPolicy.cs","tests/phase1/SuppressionTests.cs","tests/phase1/TestMethodNameTests.cs","tests/phase1/TestSourceIndex.cs","tests/phase1/ThresholdTests.cs","tests/phase1/TypeBoundaryScanner.cs","tests/phase1/Verification/AdapterTargets.cs","tests/phase1/Verification/AnalyzerVerifier.cs","tests/phase1/Verification/InheritedAnalyzerVerifier.cs","tests/phase1/docs/README.md","tests/phase1/packages.lock.json","tools/code-rules/AnalyzerReleases.Shipped.md","tools/code-rules/AnalyzerReleases.Unshipped.md","tools/code-rules/BodyCommentAnalyzer.cs","tools/code-rules/BodyLengthAnalyzer.cs","tools/code-rules/ContinuationIndentAnalyzer.cs","tools/code-rules/EngineGeneratedSyntax.cs","tools/code-rules/GeneratedLayout.cs","tools/code-rules/GeneratedSyntax.cs","tools/code-rules/LineLengthAnalyzer.cs","tools/code-rules/LocalNewDeclarationAnalyzer.cs","tools/code-rules/MemberBodies.cs","tools/code-rules/MemberOrderAnalyzer.cs","tools/code-rules/NamedArgumentAnalyzer.cs","tools/code-rules/NullForgivingAnalyzer.cs","tools/code-rules/PrimaryConstructorAnalyzer.cs","tools/code-rules/RegionDirectiveAnalyzer.cs","tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj","tools/code-rules/ThisQualificationAnalyzer.cs","tools/code-rules/UsingOrderAnalyzer.cs","tools/code-rules/docs/README.md","tools/code-rules/packages.lock.json","tools/phase1/bootstrap.cjs","tools/phase1/check.cjs","tools/phase1/projects.json","tools/phase1/required-cases.json","tools/phase1/toolchain.json"]
covered_digest: "v4:sha256:f11850678fe6fceb23333710249d974d56447abf215858ae93bcc86045e2b8d9"
behavior_unverified: 0
overrides_applied: 0
acceptance_basis: creator_authorized_phase1_focused_linux_exception
human_verification: []
human_review_pending: 14
human_review_rules: [R-24, R-34, R-62, R-63, R-65, R-72, R-73, R-74, R-77, R-78, R-79, R-81, R-83, R-84]
decision_coverage:
  honored: 18
  total: 18
  not_honored: []
re_verification:
  previous_status: gaps_found
  previous_score: 14/16
  gaps_closed:
    - "Creator-required focused Linux assurance is now actually proven. The former complete-Linux/restart closure gate is superseded for Phase 1, not falsely reported as executed."
    - "The current creator-acceptance and transparent-follow-up criterion is met. Fourteen individual human judgments remain unperformed/Pending and receive no review pass."
  gaps_remaining: []
  regressions: []
must_haves:
  truths:
    - "A developer can reproduce a pinned engine/SDK/compiler/analyzer build and run a representative Godot integration scene through construction, loading, callbacks, and teardown. The recorded proof covers handwritten/generated partials, exports, signals, nullability, and main-thread access."
    - "A developer can find every inherited LangProj rule classified as retained, adapted, or inapplicable, with a reason and narrowly scoped compatibility decision; normal Godot code passes without blanket suppression."
    - "Running the documented checks accepts compliant examples and rejects deliberate code, architecture, and exemption violations with actionable diagnostics."
    - "A developer or agent can navigate a short root AGENTS.md and focused indexes with valid, consistently cased links, and locate enforced gameplay/Godot/service boundaries plus rules for time, randomness, identities, and saveable authoritative state."
    - "Only a fresh unique complete selected case set with real evidence can succeed."
    - "Interruptions, concurrent runs and stale results cannot manufacture a pass or change approved inputs; progress identifies failed and blocked obligations."
    - "The exact five-project inventory, evaluated references and compiled types enforce pure Core/service and permitted Godot composition boundaries."
    - "All twelve inherited analyzers and applicable numeric/syntax/configuration boundaries preserve exact donor semantics and reject intended violations."
    - "All donor exemption dispositions and exact generated permissions are traceable; handwritten companions, adjacent scopes and unregistered suppression stay checked."
    - "Controlled state/time/random inputs reproduce exact values and continuation; invalid or overflowing input cannot change the prior state."
    - "Ordinal identities and strict detached snapshots preserve valid values and meaningful order while rejecting missing, duplicate, malformed or unsupported input."
    - "Repeated, invalid, interrupted or competing restores never expose partial state or replay effects."
    - "Two distinct native lifecycle cycles enforce callback order, exactly-once handlers, required-export validation, old-generation fencing and main-thread access."
    - "Phase selection follows declared ownership, retains all existing checks, preserves exhaustive all semantics and separately leaves eight Phase 5 cases pending."
    - "The retained complete Windows baseline, targeted both-OS portability amendment and actual focused Linux assurance satisfy the creator-authorized closure exception."
    - "Overall phase acceptance follows the creator instruction; fourteen unperformed component reviews and complete Linux certification remain explicit follow-ups without pass credit."
followups:
  - item: "Complete accelerated Linux Phase 1 scope"
    status: not_executed
    owner: "Platform/testing when the creator permits a restart; retain Phase 5 full-certification ownership"
  - item: "Original graph.allowed-and-direct Linux case"
    status: failed_unaccepted
    reason: "Actual clean fixture build exceeded its unchanged internal 60-second deadline. Separately authorized direct graph mechanism passes with no original-case credit."
    owner: "Rerun original case on an adequately accelerated Linux environment"
  - item: "Fourteen qualitative human components"
    status: individually_unreviewed_pending
    owner: "Creator/design review through 01-HUMAN-CHECK.md"
  - item: "Original bootstrap RED evidence, historical WINDOWS item 1"
    status: open_historical_process_deviation
    owner: "Preserve original record; no retrospective RED"
deferred:
  - truth: "Measured analyzer and native-hosted production line/branch coverage meets inclusive 90%/80% targets, with real collection and rejection controls."
    addressed_in: "Phase 5"
    evidence: "Creator-approved specification amendment and 05-PHASE1-HANDOFF.md explicitly retain R-49/R-82 measurement and coverage certification under TEST-02/07/08/09."
  - truth: "The eight aggregate/coverage declarations and advanced platform/report certification are complete."
    addressed_in: "Phase 5"
    evidence: "ac-02.complete, ac-06.complete, ac-07.complete, ac-09.complete, R-49.positive/negative and R-82.positive/negative remain acceptancePhase 5, owner Phase 5, with no pass/skip credit."
---

# Phase 1: Code rules and documentation Verification Report

**Phase Goal:** Developers can build and extend the Godot game under strict, documented C# rules that permit correct engine integration and demonstrably reject violations.

**Verified:** 2026-11-14T21:39:24Z
**Status:** passed — current creator-authorized Phase 1 acceptance boundary
**Re-verification:** Yes — prior 14/16 gaps_found report; changed criteria and new focused evidence checked, prior passing source/evidence assessment retained.

The phase goal is achieved **under the creator's one-time focused Linux closure exception**. The technical condition is met by the original complete Windows baseline, actual Linux strict/native checks and five unchanged passing cases, the narrow platform-symbol correction passing separately on both OSes, and the separately authorized compiled graph-mechanism proof.

This verdict does **not** certify a full Linux run, a new full Windows run at the corrected revision, a passing original Linux graph case, or fourteen individual human reviews. Those remain explicit follow-ups. There are no unmet must-haves or required human-verification items under the current exception. Shared completion checkboxes remain for the root phase-completion owner; this verifier changes no tracker.

## Current Acceptance Authority

The latest creator instruction recorded in [01-CLOSURE-EXCEPTION.md](01-CLOSURE-EXCEPTION.md) is:

> I don't want to restart machine now.
> Could you just make a few important tests on Linux and we will close the phase

The amended Plan 17 frontmatter/tasks, SPEC, CONTEXT, roadmap/requirements opening and indexed AI collaboration all point to that current exception. It authorizes this Phase 1 composed assurance and explicitly retains unperformed full Linux execution and individual human judgments as follow-ups. It also predeclares the narrow observed platform-name correction and direct graph-mechanism fallback. The general complete-phase/development-OS/other-OS-VM policy remains unchanged for later phases.

The prior report's full-Linux/restart BLOCKER and required-component-review WARNING belonged to the former acceptance boundary. They are not carried forward as present closure gates after the explicit creator amendment. They are not silently passed: their actual unperformed/unaccepted status is preserved in [01-CLOSURE-FOLLOWUPS.md](01-CLOSURE-FOLLOWUPS.md), the frontmatter above and the evidence limits below. No verifier override or fabricated human approval is used; the current authoritative criteria themselves changed.

## User Flow Coverage

The project's `mvp` increment is a developer baseline. No player-facing menu, movement, gameplay, production asset or LAN flow is claimed.

| Developer step | Expected under current acceptance | Actual evidence | Status |
| --- | --- | --- | --- |
| Reproduce the pinned build | Complete retained Windows baseline and actual focused Linux strict setup | Five locked projects restore; strict solution builds have zero warnings/errors; exact observed tools and source identities | VERIFIED |
| Use normal Godot integration under rules | Real native construction, callbacks, serialized exports/resources, generated/handwritten partials, signals, null validation and teardown/thread boundaries | Complete Windows engine cases plus actual new Linux native bootstrap with twenty-one records/two distinct instances | VERIFIED |
| Apply inherited coding rules | Compliant code accepts and deliberate violations reject with exact intended evidence | Complete Windows rules/graph/exemption records; fresh Linux R-38 positive/negative, suppression guard, corrected platform branches and direct compiled graph proof | VERIFIED |
| Use controlled independent state | Exact detached values/identities/order and complete-root restoration | Existing complete Windows state evidence plus new unchanged Linux snapshot/restore passes | VERIFIED |
| Find operational references | Short canonical guide and focused checked indexes | Retained docs 36/36; fresh current navigation 1/1 at 7d9fa3f | VERIFIED |
| Close with accurate limits | Apply actual creator condition and retain all unperformed tests/reviews visibly | Current exception, Plan 17, follow-up register and unchanged Pending component-review fields | VERIFIED |

## Goal Achievement

### Observable Truths

The first four rows preserve the roadmap success-criterion wording. Supplemental rows retain plan-specific obligations without narrowing the current contract. The sixteen-truth score is a count of current amended must-haves, **not** a Linux test count or a count of completed human components.

| # | Truth | Status | Actual source and behavioral evidence |
| --- | --- | --- | --- |
| T1 | A developer can reproduce a pinned engine/SDK/compiler/analyzer build and run a representative Godot integration scene through construction, loading, callbacks, and teardown. The recorded proof covers handwritten/generated partials, exports, signals, nullability, and main-thread access. | VERIFIED | Original clean Windows setup/full proof; new genuine Linux locked five-project restore, strict 0-warning/0-error build and unchanged native bootstrap c9059bcc. Actual twenty-one Linux surface/cycle records cover these integration surfaces. Current platform assurance is limited by the explicit exception. |
| T2 | A developer can find every inherited LangProj rule classified as retained, adapted, or inapplicable, with a reason and narrowly scoped compatibility decision; normal Godot code passes without blanket suppression. | VERIFIED | Prior direct register/donor/source assessment retained: 85 IDs, 84 headings, 24 exemption dispositions; 48 retained/31 adapted/6 inapplicable. 77 current implemented bindings and two explicitly Phase 5-pending coverage bindings. No register, rule, exemption or production change in this round. |
| T3 | Running the documented checks accepts compliant examples and rejects deliberate code, architecture, and exemption violations with actionable diagnostics. | VERIFIED | Original Windows 676 scope includes 449 rules, 27 graph and 46 register cases and all six repaired regressions. New Linux positive/negative SCR0001, actual conditional suppression, corrected active/inverse/unresolved branch proof, and existing compiled graph validator reject the intended violations. |
| T4 | A developer or agent can navigate a short root `AGENTS.md` and focused indexes with valid, consistently cased links, and locate enforced gameplay/Godot/service boundaries plus rules for time, randomness, identities, and saveable authoritative state. | VERIFIED | Retained canonical guide/index/module sources and ordinal path/anchor/UTF-8 mechanism. Complete Windows docs 36/36; current navigation c8038d4d accepts exact ac-10.complete, 1/1, after current authority/prose changes. Human usefulness judgment remains expressly unperformed follow-up. |
| T5 | Only a fresh unique complete selected case set with real evidence can succeed. | VERIFIED | Existing `check.select/verifyReport/reconcile/snapshot`, exact manifest and executed runner controls retained. New successful direct smoke records were revalidated with the same report/proof validators; original failed records remain failed rather than accepted by exit/count manipulation. |
| T6 | Interruptions, concurrent runs and stale results cannot manufacture a pass or change approved inputs; progress identifies failed and blocked obligations. | VERIFIED | Exclusive lock, child cleanup, before/after source hashes and atomic publication; prior actual timeout/concurrency/interruption/source/document/stale/repeat controls pass. No implementation/deadline changes; new compositions retain separate nonce/input/tool/source identities. |
| T7 | The exact five-project inventory, evaluated references and compiled types enforce pure Core/service and permitted Godot composition boundaries. | VERIFIED | Original evaluated/compiled graph source and 27-case Windows evidence retained. Direct Linux smoke invokes existing compiled ProjectEvaluator.Validate and GraphTests.IsDirectViolation against actually built allowed/forbidden subjects; errors are exactly [] then the expected Core-to-game GRAPH_FORBIDDEN. |
| T8 | All twelve inherited analyzers and applicable numeric/syntax/configuration boundaries preserve exact donor semantics and reject intended violations. | VERIFIED | Actual SCR0001–SCR0012, Error/span verifier, 59/60/61 body, 119/120/121 UTF-16 line and continuation-boundary proofs retained. New Linux R-38 pair passes. The only changed C# bytes add an observed platform-name alternative, not an assertion or operational rule. Coverage remains Phase 5. |
| T9 | All donor exemption dispositions and exact generated permissions are traceable; handwritten companions, adjacent scopes and unregistered suppression stay checked. | VERIFIED | Exact registries, 170 native spans/eighteen member suppressions and existing boundary proofs unchanged. New Linux rules.suppression-conditional genuinely suppresses the actual analyzer, then requires the source guard's exact rejection; inverse/unsuppressed controls remain. |
| T10 | Controlled state/time/random inputs reproduce exact values and continuation; invalid or overflowing input cannot change the prior state. | VERIFIED | Prior actual Windows controlled-input/independent-vector/range/zero/overflow/banned-API tests retained. New unchanged Linux snapshot confirms exact large integers and random continuation. No state, random contract or expected vector changed. |
| T11 | Ordinal identities and strict detached snapshots preserve valid values and meaningful order while rejecting missing, duplicate, malformed or unsupported input. | VERIFIED | StableIdentity and strict bounded codec/source behavior plus prior complete Windows values/invalid-input tests. Fresh Linux state.snapshot passes its unchanged exact-value/Unicode/order/continuation assertions. |
| T12 | Repeated, invalid, interrupted or competing restores never expose partial state or replay effects. | VERIFIED | Prior executed cancellation/interruption/reader-barrier/competing-writer/mutation-sensitive Windows tests retained; actual immutable-root implementation unchanged. New Linux state.restore proves complete candidate replacement and unchanged former root. |
| T13 | Two distinct native lifecycle cycles enforce callback order, exactly-once handlers, required-export validation, old-generation fencing and main-thread access. | VERIFIED | Existing real-frame Windows lifecycle/invalid-export cases and source guards retained. Actual Linux engine log contains two distinct instances, twenty-one fresh surfaces, one captured thread, old-generation fencing and worker rejection. Raw native evidence is not called a canonical full engine-group pass. |
| T14 | Phase selection follows declared ownership, retains all existing checks, preserves exhaustive `--all` semantics and separately leaves eight Phase 5 cases pending. | VERIFIED | Independent 684 declarations/676 Phase 1 IDs/eight Phase 5 IDs remain unchanged. All original 678 declarations plus six repair controls preserved. No new smoke changes canonical selection, assertions, severities, exemptions, pins, locks or deadlines. |
| T15 | The retained complete Windows baseline, targeted both-OS portability amendment and actual focused Linux assurance satisfy the creator-authorized closure exception. | VERIFIED | All exception technical conditions have actual bounded source-bound evidence, detailed below. The original seven-case attempt stays 5 pass/2 fail; corrected case and direct graph proof are separately labeled. No host restart, new full run or original graph-case pass is invented. |
| T16 | Overall phase acceptance follows the creator instruction; fourteen unperformed component reviews and complete Linux certification remain explicit follow-ups without pass credit. | VERIFIED | Exact latest instruction and conditional technical evidence recorded; all fourteen human outcomes still Pending. Full Linux/original graph capacity/coverage/process-history follow-ups are explicit. This verifies honest current acceptance accounting, not individual human judgments. |

**Score:** 16/16 current amended must-haves verified. **Behavior-unverified:** 0 required current truths. **Required current human verification:** none under the creator exception. Fourteen individual reviews remain Pending follow-ups; they are not counted as human passes or as executed behavior.

### Plan Must-Have Reconciliation

All eighteen plans/summaries were loaded in the initial verification. This round reuses their unchanged implemented assessment and loads the amended Plan 17 and changed authority/evidence; no completed plan is repeated.

| Plans | Truths retaining their distinct must-have detail |
| --- | --- |
| 01-01 | T1, T15 — workspace-local observed pinned engine/scene |
| 01-02 | T5, T6 — fresh exact results, integrity/interruptions/concurrency and output |
| 01-03 | T2, T9 — complete donor IDs/headings/exemptions and exact bindings |
| 01-04 | T7, T8 — exact projects and strict evaluated analyzer-only configuration |
| 01-05–07 | T3, T8 — real inherited analyzer semantics, boundaries and exact errors |
| 01-08 | T3, T8, T9 — actual mechanism/build/exemption boundary wiring |
| 01-09 | T7 — real allowed/direct/imported/transitive/type/empty-subject graph behavior |
| 01-10 | T10, T11 — exact controlled time/random and ordinal identities |
| 01-11 | T11, T12 — strict detached snapshots and atomic restoration |
| 01-12 | T1, T13 — actual native integration/lifetime/export/thread behavior |
| 01-13 | T4 — canonical guide and host-independent documentation identities |
| 01-14 | T2, T4, T16 — five module references, current bindings and separate honest review accounting |
| 01-15 | T14 — preserved ownership-based selection and explicit later certification |
| 01-16 | T15 — retained clean complete Windows baseline |
| 01-17 amended | T15, T14, T16 — creator-approved composed assurance, preserved checks and explicit unperformed follow-ups |
| 01-18 | T1, T5, T7, T8 — real composition/compiler verification, exact locks and bypass controls |

### Required Artifacts

The initial verifier ran all eighteen `verify.artifacts` owner queries: **93/93 declarations / 90 unique artifact paths passed** existence/substance checks. Previously passing artifacts remain present with basic sanity checked. The single changed test and new proof/authority records received full source/behavior/identity checks in this round.

| Artifact / set | Existence and substance | Wiring / behavior verdict |
| --- | --- | --- |
| Toolchain/bootstrap/global/NuGet/tool pins and five locks | Present, substantive and unchanged | Real pinned Windows and Linux setup/native execution; no donor runtime dependency |
| Canonical runner/independent manifest/ProofProtocol | Present, substantive and unchanged | Exact method/report/proof reconciliation and fresh identity controls; no direct smoke promoted into canonical completeness |
| Captured donor/register/matrix/RuleBindingValidator | Present, substantive and unchanged | Prior actual capture/current-binding/exemption tests retained; two coverage rows remain pending |
| Twelve analyzers/shared build/banned policy | Present, substantive and unchanged | Actual normal consumers and precise diagnostics are exercised |
| Exact projects/project policy/evaluated and compiled scanners | Present, substantive and unchanged | Original graph proof plus new real compiled Linux allowed/forbidden mechanism |
| Core identity/random/codec/store/state proofs | Present, substantive and unchanged | Value, validation, atomicity and cancellation/reader invariants have actual tests; focused Linux round trips/root replacement pass |
| Native scenes/resources/scripts/gateway/token | Present, substantive and unchanged | Actual Windows cases and Linux bootstrap callback/export/signal/thread/lifetime evidence |
| Guide/indexes/five module pages/docs validators | Present, substantive; current prose revised | Existing docs proof and current actual navigation pass |
| ConsumerPolicyTests.cs | Present; only one platform-string alternative added | Same active/inverse/unresolved assertions pass separately on Windows and Linux |
| Exception/follow-up/platform/summary/validation records | Present and detailed | Actual results retain original failures and distinct source/run limits; current exception governs inherited historical sections |
| Human checklist | Present; fourteen reviewer/date/outcomes Pending | Explicit unperformed follow-up; no invented approval |

### Key Link Verification

The initial owner link CLI could not interpret the plans' narrative-string `key_links` (exit 1, total 0). Manual concrete wiring was verified then and retained; this is not a missing production link or a structural CLI pass.

| From | To | Actual connection | Status |
| --- | --- | --- | --- |
| Pinned declarations | Actual SDK/native engine/packages and scene | Bootstrap strict manifest/hash checks → actual restore/build/launch | WIRED; new Linux record inspected |
| Independent manifest | Named C# executable/report/proof/final summary | select → actual process → verifyReport/reconcile → atomic publication | WIRED; existing complete proof retained and focused records revalidated |
| Shared policy | Normal production/test consumers | Analyzer-only project edges and AdditionalFiles/central pins | WIRED; no delta |
| Donor/register | Exact local source definitions/proofs/permissions | Captured byte inventory → validators → actual methods/scopes | WIRED; no delta |
| Actual consumer compiler inputs | Engine/suppression/random/service/visibility scans | ResolveReferences/DefineConstants/LangVersion/Compile/ReferencePath and fail-closed semantic subject | WIRED; narrow Linux platform selector now chooses actual GODOT_LINUXBSD |
| Allowed/forbidden graph | Existing compiled validator | Actual strict subjects → reflection host invokes existing Validate/predicate | WIRED; [] then exact intended edge rejection |
| Serialized candidate | Live independent root | Full decode/validation → one immutable root exchange | WIRED; unchanged Linux root proof passes |
| Real native scene | Unique callback/export/lifetime/thread records | Engine-created instances/frames → existing guarded source → actual log assertions | WIRED; twenty-one Linux records/two instances |
| Current guide/indexes | Operational source and project graph | Exact path/anchor inventory and evaluated declarations | WIRED; fresh navigation 1/1 |
| Creator closure condition | Current technical evidence and honest follow-ups | Exception/Plan 17 → distinct original/corrected/direct records → follow-up registry | WIRED; no synthetic full/seven-case/human pass |

### Data-Flow Trace (Level 4)

This phase has no rendered product-data/database surface. Real CLI, native and state values were traced instead; no empty UI/static return stands in for implementation.

| Artifact | Data source and flow | Status |
| --- | --- | --- |
| Complete Windows summary | Independent required IDs plus actual per-case test/proof/command records | FLOWING; original 676/676 report retained |
| Focused Linux receipts | Real selected MTP reports, nonempty subject logs, exact run/input/tool/platform identity and actual exits | FLOWING; initial five pass/two fail preserved, corrected case separate |
| Native surface evidence | Actual scene/resource construction and frame callbacks on Linux | FLOWING; 21 fresh surfaces, 2 distinct instances, 1 captured thread |
| Direct graph verdict | Real source-hash-matched fixture, actual strict builds and existing compiled validator | FLOWING; allowed [] and exactly one intended forbidden edge; original case not credited |
| Snapshot/live root | Caller bytes and actual immutable state/store operations | FLOWING; independent values and complete replacement tested |
| Full Linux / component review outcomes | No full run or human component decision exists | Explicit unperformed follow-up; never substituted with fabricated data |

## Behavioral Evidence

### Retained Full Windows Baseline

The initial independent verification already reconciled every one of **676 exact test reports, 692 nonempty subject-evidence files, 681 successful command/log hashes and 203 frozen inputs**, with zero mismatches. This re-verification reuses that completed assessment and confirms the baseline summary hash and narrow source delta. It does not rerun all 676 checks or label them a new full pass.

| Identity | Retained value |
| --- | --- |
| Revision | 59f032cf23340058eec91f12c5d240f585dfabdc |
| Nonce | b8b5eacc-c46a-42d7-a898-01ad33c1b0a8 |
| Input | fe2bf549f20329633fd62cfeab796ca8fb2c5ce02da432346e8ee4c314873426 / 203 files |
| Tool | a23d7bacf589ac13e65eda9e329b0218ebef3a34ba90ad2630f1607349e6aab1 |
| Platform | win32-x64-10.0.19044 |
| Result | 676/676 accepted, zero failed/blocked/missing/skipped; 8 separately deferred Phase 5 declarations |
| Actual interval | 2026-11-14T18:40:20.004Z–19:31:55.692Z |
| Summary SHA-256 | 042c7e2b280940860c7e1304ee85bcfb756d4ae58c2d192b51c713dbbd37aa4a |

[Original Windows report](../../../.phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/summary.json). Clean native bootstrap, five locked restores and strict solution build all actually exited zero with 0 warnings/errors. Source revisions and subsequent changed-case evidence remain distinct.

### Actual Linux Setup and Original Focused Attempt

Actual Ubuntu 24.04.5 LTS / kernel 6.8.0-147-generic / x86_64 under explicitly selected TCG; no host restart or acceleration pass is inferred. The seven cases were predeclared under the creator exception and use unchanged original method filters/assertions.

| Setup | Actual result |
| --- | --- |
| Locked five-project restore, 1200s ordinary outer bound | Exit 0 |
| Strict whole-solution build with original flags and -m:1 | Exit 0, 0 warnings/errors, elapsed 00:06:07.66 |
| Existing native bootstrap, unchanged internal child limits | Exit 0, nonce c9059bcc-b8fa-499d-82e3-6612c20dfbe7 |
| Actual native evidence | Construction/ready/exit plus 21 surfaces; instances 27279754708 and 27581744596; managed thread 1 |

Observed pins are Node 24.22.1, SDK 10.0.404, compiler 5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf), Godot 4.7.2.stable.mono.official.ed1daf0bf, editor runtime 8.0.16 and hosted runtime 10.0.15. Native records cover resource/exports/binding/null validation, callbacks/process/signals/teardown, late-generation fence, worker rejection and main-thread delivery.

**Original smoke identity:** nonce 55960e73-29ec-4d63-ad88-028f588e5e6d; source 59f032c; input fe2bf549...873426, exactly matching the original Windows input; tool 1b607aea3c49c937008cfeca23d8e9a7c8082516cd844155c5c2291ced86c8cc; platform linux-x64-6.8.0-147-generic. Actual interval 20:21:29.529Z–20:38:23.523Z on 2026-11-14.

| Original predeclared case | Exact observed result | Current accounting |
| --- | --- | --- |
| R-38.positive | 1 pass / 0 fail / 0 skip; ANALYZER_ACCEPTED, exit 0 | Passing focused compliant-code evidence |
| R-38.negative | 1 pass / 0 fail / 0 skip; SCR0001, exit 0 | Passing intended violation evidence |
| graph.allowed-and-direct | 1 fail / 0 pass / 0 skip, exit 2; “Isolated compiler exceeded 60 seconds” | Remains failed/unaccepted; no original-case pass credit |
| state.snapshot | 1 pass / 0 fail / 0 skip; STRICT_SNAPSHOT_ROUNDTRIP, exit 0 | Passing unchanged exact-value/identity/order/continuation evidence |
| state.restore | 1 pass / 0 fail / 0 skip; ATOMIC_ACCEPTED_ROOT, exit 0 | Passing unchanged complete-root evidence |
| engine.consumer-branches | 1 fail / 0 pass / 0 skip, exit 2; “Sequence contains no matching element” at old platform selector | Original failure retained; narrow correction has separate fresh passes below |
| rules.suppression-conditional | 1 pass / 0 fail / 0 skip; CONDITIONAL_SUPPRESSION_REJECTED, exit 0 | Passing actual analyzer/source-guard evidence |

**Original outcome remains 5 passed / 2 failed / 0 skipped.** Direct smoke case bounds are 8-minute MTP / 10-minute outer; canonical 45-second and internal 60-second source limits are unchanged. This is not canonical `--phase 1` acceptance.

[Original receipt](../../../.phase1/temp/17-linux-smoke/evidence/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/smoke-receipt.json), SHA-256 419cfed0bd80d579415317eef1407057098cad2cced33565c89716f2e4ffc5a8. Archive SHA-256 ed71610d06a12d0aafa5d0c292b9bb20c5d14af9886098fad4c3b6696e76ada9.

### Narrow Platform-Symbol Correction on Both OSes

Actual SDK evaluation records target `linuxbsd`, DefineConstants `GODOT_LINUXBSD`, `DEBUG` and `TOOLS`. It lacks the formerly assumed platform alternatives. Commit **70918fdb77cff309527b407418083e52db89378e** adds only `or "GODOT_LINUXBSD"` in ConsumerPolicyTests.cs:23. Git comparison confirms this is the **only C# change** from the full Windows baseline through current HEAD. Production, compiler-input helpers, shared policies, manifests, pins, locks, assertions and deadlines are unchanged.

The test still requires actual active forbidden calls to reject, inverse inactive branches to accept and an unresolved semantic subject to fail closed. Selecting the observed name repairs test setup rather than weakening an assertion.

| Actual changed-case verification | Result / identity |
| --- | --- |
| Windows canonical `--case engine.consumer-branches` | 1/1 accepted, actual exit 0; nonce 66a74d57-ecc3-4d79-b1b8-0f27bf41e6d9 |
| Windows genuine recorded revision | 266a6d87c72477d77dee8beaa8283ae0bce4d441 precommit workspace; same actual input as committed 70918fd |
| Windows tool | 301718a41128e6df235378cea6b54c123baef1343a283ade9c21cbfaf870269a |
| Linux strict test-project rebuild | Exit 0, 0 warnings/errors, elapsed 00:04:07.43 |
| Linux direct changed case | 1 pass / 0 fail / 0 skip, exit 0; nonce b4ccec5f-255f-466f-bea2-f807b260ccf8 |
| New source / common input | 70918fd / 2e16dd414d42f53cba37707e629c9110fe64164ddb8e96636c878c014419c15f |
| Linux tool | b8578556d768b15bb41bfc8a2b5b67e5b63a1be7b0dc2b278edcf658217b6f75 |
| Linux proof | CONSUMER_BRANCHES_CHECKED; actual evaluated DEBUG/TOOLS/GODOT_LINUXBSD; inverse branches and unresolved subject |
| Linux actual interval | 2026-11-14T20:49:51.631Z–20:55:53.044Z |

[Windows amendment](../../../.phase1/runs/66a74d57-ecc3-4d79-b1b8-0f27bf41e6d9/summary.json); [Linux amendment receipt](../../../.phase1/temp/17-linux-smoke/amendment-evidence/.phase1/amendment/b4ccec5f-255f-466f-bea2-f807b260ccf8/amendment-receipt.json), SHA-256 95d57c2c9f183e530382dcf8e64f255f32e483423e16473ae502f5a45f4e6733.

### Separately Authorized Direct Graph-Mechanism Proof

The unchanged original graph case failed during its clean fixture build at its internal 60-second deadline. The authorized fallback performs ordinary bounded strict setup outside that test helper and invokes the same actual compiled validator/predicate against real subjects. It does not rewrite the validator, grep for expected text, seed old build outputs, or grant the original case a pass.

Nonce **6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365**, source 70918fd, input 2e16dd...9c15f, platform linux-x64-6.8.0-147-generic; actual interval 20:57:02.986Z–21:05:58.060Z on 2026-11-14.

- All **72 copied source hashes** match the candidate production/configuration inputs. The host also checks the actual compiled BuildFixture.CleanSource constant against the fixture subject.
- The compliant graph strictly builds with **0 warnings/errors**, elapsed 00:03:27.44; existing compiled `ProjectEvaluator.Validate` returns **[]**.
- The actual AddDirectEngineEdge-equivalent XML delta removes game-to-Core and adds Core-to-game. Force-evaluate restore, locked restore and changed-Core strict build all exit 0; changed build has **0 warnings/errors**, elapsed 00:01:56.88.
- The same compiled validator returns exactly one error, and existing `GraphTests.IsDirectViolation` returns true:

```text
GRAPH_FORBIDDEN: src/core/KingdomZombies.Core.csproj -> src/godot-game/KingdomZombies.csproj defined by src/core/KingdomZombies.Core.csproj
```

The reflection host calls the existing assembly's methods via reflection; source inspection confirms no replacement validation algorithm. Validator assembly SHA-256 de05b6a138780912c7c103c69aea3f53442088215d54db48fbc6656f455de83f matches the amended Linux test assembly. Host source SHA-256 11f06df4604092c3b858c1901257503af0e43781a99f6251fdda2ce82c435c31 and host binary 9babaf4a9bc31d3eabdded0f2d34a0323a4e7bb9286493d23216b0ea3ff85a15 match the retained artifacts.

All eight direct commands exit zero with no timeout/signal. Ordinary setup/build outer bounds are 1200s; validator-host bounds 600s. Existing source/helper deadlines are unchanged. [Direct graph receipt](../../../.phase1/temp/17-linux-smoke/amendment-evidence/.phase1/direct-graph/6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365/graph-mechanism.json) SHA-256 3c512d37162931e4f14666b130eaa21534046f592f256e60674a6e26899dfaaf explicitly records **originalCaseCredited: false**.

This proves the seventh important behavior under the amended exception. It does not make the original seven-case attempt a 7/7 pass.

### Independent Re-Verification Performed Here

This verifier performed read-only evidence/source checks and no new tests, native process, VM boot, host change or coverage work.

| Check | Actual independent result |
| --- | --- |
| Original seven Linux reports plus corrected Linux report | Eight exact method/trait/count/status records checked; original five passes/two failures and corrected one pass preserved |
| Successful Linux proof records | Six proofs revalidated with current verifyReport/reconcile, exact fresh nonce/input/tool/platform/diagnostic and nonempty evidence; six subject hashes match |
| Linux commands/logs | All 30 log hashes match: 16 original commands retain two genuine case exit-2 failures, six amendment and eight direct commands all succeed |
| Strict build records | Original solution, amended tests, direct compliant and direct mutated subject logs each show zero warnings/errors |
| Native records | Twenty-one actual surfaces, two distinct instances, one captured thread; construction/ready/exit and required lifetime/thread/export surfaces inspected |
| Graph subject/host | 72 source hashes, actual before/after project bytes, compiled host source/binary identity, [] allowed result and exact forbidden result/predicate checked |
| Original/amendment archives | Both SHA-256 values match; amendment archive ceeeb9e22e5dfb83d4752f178bd7f8eb385028a3b2d00b1115156adc6506c765 |
| Windows changed case | Exact 1/1 plus report/evidence identities/hashes and six successful command/log hashes verified |
| Fresh current navigation | ac-10.complete 1/1, nonce c8038d4d-2cc1-4f0c-82dc-f6b6c541df65 at 7d9fa3f; exact report/proof and six command/log hashes verified |
| Current source delta | Only C# change is observed GODOT_LINUXBSD test alternative; all production/policy/helper/pin/lock/manifest/deadline bytes unchanged |
| Result | No identity/hash/source/wiring mismatch or new evidence-backed implementation defect found |

The current main source has additional explicit authority/proof/navigation prose differences from the frozen baseline. These are not mixed into the original full run. The creator exception explicitly accepts the retained baseline plus narrowly targeted feedback, and current navigation has its own identity.

### Probe Execution

| Probe | Actual retained/new record | Status and scope |
| --- | --- | --- |
| Original complete Windows setup/native/bootstrap | Frozen 59f032c actual exit-zero records | PASS retained baseline |
| New Linux existing native bootstrap | c9059bcc, actual exit 0 and inspected native assertions | PASS focused Linux raw native proof |
| Exact Linux MTP smoke cases | Original and amendment receipts above | Five unchanged passes plus separate corrected pass; two original failures remain |
| Existing compiled graph validator | Separate direct-graph receipt/actual strict subjects | PASS mechanism, originalCaseCredited false |
| Conventional probe-*.sh | No scripts directory or phase-declared shell probe | N/A |
| New verifier execution | None; this round validates actual retained artifacts | No new runtime pass invented |

## Requirements Coverage

All seven IDs retain Phase 1 primary ownership; no orphaned RULE requirement exists. The phase goal verifier recommends their completion **under the current creator acceptance boundary**. At write time the shared requirement checkboxes and 17/18 tracking still await the root phase-completion owner; this report does not change them or advance Phase 2.

| Requirement | Source plans | Current actual evidence | Verdict under exception |
| --- | --- | --- | --- |
| RULE-01 | 01, 02, 04, 15, 16, 17, 18 | Exact observed pins, five locks, donor-independent complete Windows setup/scope, actual Linux strict build/native and focused proof identities | SATISFIED current scope; full Linux certification remains follow-up |
| RULE-02 | 03, 05–08, 14, 15, 17 | All donor dispositions/reasons/scopes and current bindings; no operational weakening or fabricated coverage | SATISFIED current scope; component judgments remain individually Pending follow-up |
| RULE-03 | 02–08, 11–18 where declared | Full Windows intended positive/negative compiler/analyzer/architecture/exemption evidence, six repairs, fresh Linux SCR0001/suppression/branch/direct-graph proof | SATISFIED current scope; original graph case remains failed/unaccepted |
| RULE-04 | 02, 13, 14, 16, 17 | Canonical guide/focused indexes/five modules, exact case/encoding/anchors and current actual navigation pass | SATISFIED technical scope; usefulness/accuracy review is unperformed follow-up |
| RULE-05 | 04, 09, 14, 17, 18 | Five-project evaluated/compiled boundaries, existing full Windows graph tests and real compiled allowed/forbidden Linux validator outcomes | SATISFIED current scope; adequate-capacity original Linux case rerun remains follow-up |
| RULE-06 | 01, 08, 12, 14, 16, 17 | Actual native construction/resources/exports/generated/partial/signals/null/thread/teardown records on both platforms, exact unchanged permissions | SATISFIED current representative integration scope; no full Linux engine-group certificate claimed |
| RULE-07 | 02, 04, 10–12, 14, 17, 18 | Controlled input/identity/state/snapshot/atomic/thread implementation and actual Windows behavior plus fresh Linux snapshot/root/native evidence | SATISFIED current scope; detailed human semantics and broader Linux certification remain follow-up |

## Deferred Work and Accepted Follow-Ups

| Item | Actual status | Owner / boundary |
| --- | --- | --- |
| Complete accelerated Linux Phase 1 run | Unexecuted; focused composed evidence only | Platform/testing when creator permits restart; preserve Phase 5 certification ownership |
| Original graph.allowed-and-direct Linux case | Failed/unaccepted at internal clean fixture build deadline | Rerun original case on adequate acceleration; direct mechanism pass never overwrites it |
| Fourteen individual qualitative components | All reviewer/date/outcomes Pending/unperformed | Existing human checklist/creator-design review; no AI or test fills human fields |
| Eight Phase 5 declarations | Pending: ac-02/06/07/09.complete, R-49 positive/negative, R-82 positive/negative | Phase 5; no pass/skip credit |
| Measured analyzer/native-hosted production coverage | No accepted measured report; inclusive 90% line/80% branch targets unchanged | Phase 5 TEST-02/07/08/09 and handoff |
| Historical WINDOWS item 1 | Missing original bootstrap RED evidence retained open | Process-history follow-up; no retrospective RED |
| Gameplay/menu/movement/LAN/assets/release | Outside this phase; no implementation/acceptance claimed here | Existing later-phase owners; no Phase 2 execution |

[Closure follow-up register](01-CLOSURE-FOLLOWUPS.md) and [Phase 5 handoff](../05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) preserve the exact work and ownership. These are current explicitly accepted follow-ups, not silently lost gaps or fabricated successes. The general full-other-OS-VM rule remains binding for later phases.

### Decision Coverage

Owner query: **All trackable CONTEXT.md decisions are honored by shipped artifacts.** 18/18 honored, no not_honored entries, nonblocking. The later preservation/current-closure decisions are additionally checked against actual unchanged rules/tests and explicit exception/follow-up artifacts.

### Test Quality, Review, Security and UI

The initial actual-source audit and 185-file independent code-review scope are retained. It found no requirement supported only by skipped/existence/status assertions, circular expected-value oracle or disconnected stub. Intentional skipped/assertion/crash/hang child fixtures remain negative subjects; their enclosing controls prove rejection.

This round specifically checked the changed test's assertions and new exact-value/semantic/compiled subject records. The original wrong-symbol assumption failed visibly, the one-line observed-name repair keeps active/inverse/unresolved checks intact, and both-OS changed-case results are exact. The original graph timeout is never used as the intended rejection; the separate proof first builds a compliant subject, then actually compiles the mutated subject before invoking the existing validator. This avoids unrelated setup failure masquerading as graph proof.

The code-review amendment at 7d9fa3f / recorded in current 4a1b0c9 is **clean**, retaining the 185-file assessment and zero new findings. All four earlier findings remain fixed. Security remains 31/31 declared ASVS L1 mitigations closed, threats_open 0, no accepted security risks/transfers; the narrow test alternative changes no production security surface or closure logic. Official unsigned Godot packages retain archive/payload provenance, not an invented signature pass.

Current Nyquist validation is compliant for the creator-amended technical scope with zero required gaps. This does not declare the full Linux suite executed. UI remains N/A because no phase-owned player presentation exists; no numeric visual pass, screenshot gate or artificial graphical UAT is introduced. No new gate or gap-closure plan is needed.

### Anti-Patterns and Advisory Findings

| Area | Finding | Classification / effect |
| --- | --- | --- |
| ConsumerPolicyTests.cs delta | No TBD/FIXME/XXX/TODO/HACK/PLACEHOLDER; all existing assertions remain | No blocker/warning |
| Original Linux failures | Both reports retained; one corrected separately, graph original unaccepted | Explicit current accepted follow-up, not suppressed evidence |
| Historical plan/spec/validation/summary passages | Older full-scope/restart/human-gate prose remains beneath explicit current exception/amendment | Info; latest creator authority is unambiguous and governs this phase |
| Human checklist | All fourteen fields remain Pending, with corrected baseline/amendment references | Correct honest follow-up accounting, not a human pass |
| Historical bootstrap RED | Missing initial assertion RED remains open | Info/process deviation; no invented retrospective evidence |

### Advisory (New Scope, Unevidenced)

None. No new evidence-backed implementation defect or unresolvable current criterion was found. Retained historical prose does not override the latest explicit creator instruction.

### Human Verification Required

**None for current Phase 1 closure under the creator exception.** The fourteen individual rule judgments remain **unreviewed/Pending follow-ups**, not required current closure items and not completed component reviews.

The retained exact items are R-24 initialization semantics, R-34 public API necessity, R-62 reusable tooling intent, R-63 domain invariant meaning, R-65 mapping intent, R-72 codec tradeoff, R-73 time/random fitness, R-74 unknown sentinel meaning, R-77 enum meaning/evolution, R-78 independent DTO identities, R-79 health-check applicability, R-81 model/options cohesion, R-83 documentation usefulness and R-84 decision/adaptation accuracy. A future real reviewer must record reviewer/date/outcome before claiming any of those individual reviews complete. Overall conditional phase acceptance is the recorded creator instruction whose technical condition is now met.

## Verdict and Completion Handoff

**passed** is valid for the current amended contract: all sixteen current merged must-haves have source/wiring and applicable behavioral evidence; no blocker, new regression, required behavior-unverified truth or current human-verification item remains. The original failed graph case, unexecuted complete Linux scope, fourteen unreviewed components, eight Phase 5 declarations and historical RED debt remain explicitly visible.

The root may now use the phase-completion owner for Phase 1 and RULE-01–RULE-07, preserving these limits and follow-ups, and stop under the original `--only 1` request. This verifier does not update trackers or execute Phase 2. If completion changes fingerprinted metadata, refresh the report fingerprint through its owner before treating it as current.

The owned guest is already cleanly shut down: actual poweroff at 2026-11-14T21:08:59Z, verified at 21:09:10.1604557Z; QEMU 31028/helper 28040 exited, no owned listeners, exclusive disk read succeeded. The retained record explicitly says hostRestartPerformed false and hypervisorPresent false. No restart or VM action was performed by this verifier.

### Fingerprint and Scope

The verifier recomputed the required owner command over the previous 314-file covered set plus current authority and decisive focused evidence:

`node .codex/gsd-core/bin/gsd-tools.cjs query verification.fingerprint .planning/phases/01-code-rules-and-documentation <covered-files...>`

The returned **349-file covered set and v4:sha256:f11850678fe6fceb23333710249d974d56447abf215858ae93bcc86045e2b8d9 digest are copied verbatim** into frontmatter. This includes source/configuration/policy and phase PLAN/SUMMARY records, current exception/follow-ups, aggregate/full-proof records, both hashed Linux evidence archives and decisive raw smoke/graph/report records. The report excludes itself. No covered input was edited after fingerprinting; only this authorized verification report was rewritten. Unrelated Phase 5 discussion checkpoint and captured Phase 2/3/4 context are preserved. No tests, VM/host action, implementation edit, other report/tracker change or commit was made.

---

_Verified: 2026-11-14T21:39:24Z_
_Verifier: the agent (gsd-verifier)_

