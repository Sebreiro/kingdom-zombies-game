# Plan 01-09 execution checkpoint — current continuation

**0/2 complete tasks.** The direct-edge tracer increment has genuine RED/GREEN commits; full Task 1 and all Task 2 remain incomplete. Do not create a complete SUMMARY or advance STATE/ROADMAP. Phase 1 stays 9/18, all RULE requirements Pending, with independent Phase 2 annotation/artifacts preserved.

## Newly verified committed increment

- `266d495`: real direct-edge RED. Exact `GraphTests.Evaluate_CoreEngineReference_ReportsForbiddenEdge` failed planned ShouldContain after clean and changed real subjects built successfully; strict harness had zero warnings/errors. Actual exit 2, unchanged supported JUnit `.phase1/temp/09-red.xml`, console `.phase1/temp/09-red.console.log`, record `.phase1/temp/09-red.json`. Classifier returned **RED_EVIDENCE_OK** using fully qualified emitted method `KingdomZombies.Phase1.Tests.GraphTests.Evaluate_CoreEngineReference_ReportsForbiddenEdge`. Initial short-name record was rejected; report bytes unchanged.
- `92fca69`: actual evaluated five-project graph GREEN. Own policy `tools/phase1/projects.json`, `ProjectEvaluator.cs`, GraphFixture inventory copy and exact manifest binding. Canonical `node tools/phase1/check.cjs --case graph.allowed-and-direct` passed **1/1**, exit0, run `.phase1/runs/79c0915a-63da-428b-8280-2ae6e7178bb7/summary.json`. Intended `GRAPH_FORBIDDEN` identifies Core-to-game and defining Core project.
- Canonical run revision `266d495cbd6b65d27d7eb29b924e48ca2ed24c1e` plus precommit evaluated-graph changes, input `231046a122cd6ee3109a18779433fef523bdb004b55364cac8da070849ca4410`, tool `d256c2aefa92b65af9032296448fba3c01a2894eab3e7c73a7c4c0712b36cda8`, actual `win32-x64-10.0.19044`. This does not verify subsequent compiled-type WIP. Raw focused tracer also passed1/1 (~22.5s). No full09suite run.

Evaluator queries actual MSBuild ProjectReference/PackageReference plus TargetFramework/AssemblyName/TargetPath, recursively evaluates closure and defining origins, checks both analyzer metadata, exact five-project permissions, duplicate refs/packages and solution plus src/tests/tools authored inventory. This verified tracer does not yet satisfy complete graph/type/architecture actions.

## Preserved WIP and immediate next action

Modified `tests/phase1/GraphTests.cs` adds `Inspect_CoreEngineType_ReportsCompiledLeak`: creates real clean fixture, builds Core-to-game edge, writes compiled Core `typeof(KingdomZombies.Proofs.Phase1)` source, requires another successful build then TYPE_FORBIDDEN. Untracked `tests/phase1/TypeBoundaryScanner.cs` is an intentional accepting `Validate(assembly, expectedType, boundary)` scaffold pending genuine runtime RED. Neither file is included in the GREEN increment.

Current strict build has exactly two new scaffold errors, no runtime type RED credit:

```text
GraphTests.cs(40,72): SCR0001: Argument for parameter 'value' must be named
TypeBoundaryScanner.cs(7,121): SCR0007: Line is 123 characters long; limit120
```

Next: use named `value:` and explicit `comparisonType: System.StringComparison.Ordinal` in the new Contains assertion with compliant continuation; wrap XML doc below120. Strict compile, run exact new target with fresh JUnit, require successful clean and changed subject builds plus intended assertion failure, persist/classify semantic RED, commit RED before scanner implementation. The changed type subject has not run and might still have a build issue; that would never supply RED credit.

This window reached the bounded task repair limit: evaluator convention fixes (null-forgiving/member order/array/style/return type), final constant-order fix, then runner binding fix from unsupported `implementation: implemented` to actual `csharp`. All fixed/verified. The subsequent compiled-type scaffold failure is preserved for a fresh continuation instead of fixing beyond the limit.

## Remaining exact plan scope

Both entire actions in01-09-PLAN.md remain required. Strengthen inventory to tracked/new authored union across intended repository scope (current discovered directories alone insufficient); missing/null/empty/duplicate selectors must fail before set conversion. Add independent expected compiled types; both analyzer metadata mutations; real imported props/targets/wildcard/semicolon/transitive/undeclared/duplicate/reversed matrices; compiled bases/interfaces/generics/attributes/signatures/fields/IL and lambda/async/iterator leaks; isolated provider only ports; product-agnostic analyzer; service locator boundaries; concrete entity/snapshot/private mutation/hand mapping/config DTO selectors; single version and pinned enum/evolution baselines. No sixth repo project. Actual proof project has zero product edges; inspection edges explicit. Bind unique R51–56/R58–66/R74/75/77/78/81 polarities and graph AC12/13/edges/ac07. Preserve R57 inapplicability and qualitative judgments. Current exact tracer is already `csharp`/GRAPH_FORBIDDEN.

No Context7 tool or ctx7 executable available. Official fallback docs were read this continuation before metadata implementation choice:

- https://learn.microsoft.com/en-us/visualstudio/msbuild/evaluate-items-and-properties
- https://learn.microsoft.com/en-us/dotnet/api/system.reflection.metadata.metadatareader?view=net-10.0
- https://learn.microsoft.com/en-us/dotnet/api/system.reflection.portableexecutable.pereader?view=net-10.0

Built-in PEReader/MetadataReader may avoid new packages; inspect only trusted freshly compiled assemblies, never instantiate Nodes. Exact donor architecture rules were read directly from immutable ZIP; originals unchanged. Strict inherited severity and authentic Directory.Xunit.targets preserved. Explicit cwd/root pin and protected query before commits; normal hooks. Ledger `.git/gsd-plan-head-before-01-09` persists `286fb68`; original initial baseline3a5c66a, previous checkpoint286fb68. Current manifest585 (one added tracer), no full count claim. Prior08suite477pass/0failedcontrols/107pendingof584. Finish prerequisites then one immutable full suite perD10, exact SUMMARY/selfcheck, latest shared state preservingPhase2. Live LinuxVM untouched; no Linux/coverage claim.

---

# Historical prior checkpoint (superseded by current continuation above)

Task 1 RED harness is in progress. **0/2 tasks complete; no task commits and no valid runtime RED evidence.** Baseline HEAD is `3a5c66a`. Do not advance STATE/ROADMAP or mark any RULE requirement complete. The exact Phase 2 session annotation remains untouched.

## Preserved working files

- `tests/phase1/ProjectEvaluator.cs`: accepting `Validate(root)` scaffold returning no diagnostics, intentionally pending implementation until genuine RED evidence.
- `tests/phase1/GraphFixture.cs`: real isolated `BuildFixture(game: true)` clean restore/build; removes game-to-Core edge, adds Core-to-game reference, regenerates isolated locks, performs locked restore and builds violating subject. Own SDK/git child helper has explicit cwd, 60-second deadline, owned-tree timeout cleanup and logs under `.phase1/temp/graph-*.log`.
- `tests/phase1/GraphTests.cs`: intended tracer tests allowed repository graph then compiled forbidden Core-to-Godot graph and requires GRAPH_FORBIDDEN with Core and engine source identities.

These three files are intentional untracked WIP, preserved for continuation. Do not treat them as complete implementation. There is no projects.json or type scanner yet.

## Immediate blocker and next action

Strict build currently produces exactly:

```text
tests/phase1/GraphTests.cs(17,48): error CS1503: Argument 2: cannot convert from 'method group' to 'System.Linq.Expressions.Expression<System.Func<string, bool>>'
```

The call is `errors.ShouldContain(elementPredicate: IsDirectViolation);`. Shouldly requires an expression lambda. Replace with `errors.ShouldContain(elementPredicate: error => IsDirectViolation(error: error));`, then build the strict harness. Three scaffolding repair passes consumed this executor's task repair budget (named arguments/continuation, extraction of predicate, then expression binding); this checkpoint obeys the bounded-repair rule. No compile failure supplies RED credit.

Run all commands with explicit workdir `D:/Work/Games/KingdomZombies`. Before writes and every commit repeat the literal supplied-root pin from dispatch. Sequential execution is authorized on main with the recorded default-branch override; hooks remain enabled.

```powershell
$env:DOTNET_CLI_HOME = "$PWD/.phase1/cli-home"
$env:NUGET_PACKAGES = "$PWD/.phase1/packages"
$env:PHASE1_ROOT = "$PWD"
dotnet build tests/phase1/KingdomZombies.Phase1.Tests.csproj --no-restore -p:NuGetAudit=false --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -v:q
dotnet tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll --filter-method '*GraphTests.Evaluate_CoreEngineReference_ReportsForbiddenEdge' --report-xunit-junit --report-xunit-junit-filename 09-red.xml --results-directory .phase1/temp --no-ansi --parallel none
```

Require actual target assertion failure after successful clean and violating fixture builds, capture unchanged JUnit plus command/exit/target/expected/actual/semantic assessment in `.phase1/temp/09-red.json`, and require `node .codex/gsd-core/bin/gsd-tools.cjs check tdd-red-evidence .phase1/temp/09-red.json --raw` to return RED_EVIDENCE_OK before implementation. Commit genuine RED before GREEN. The runtime MVP gate is not configured (`init.execute-phase` returns tdd_mode false), but both task tdd=true contracts still apply. No supported-format report has been created yet.

## Implementation remaining

Read the full exact `01-09-PLAN.md` and inherited 01-08 SUMMARY. Implement all declared graph/type/structural/pinned-evolution actions; this checkpoint does not narrow scope. Important observations:

- The production subject inventory currently consists of Core `KingdomZombies.Core.ProofValue`, ports `KingdomZombies.Multiplayer.Abstractions.IProofService`, and game `KingdomZombies.Proofs.Phase1`; preserve minimal Phase 1 scope.
- Shared targets add authentic analyzer-only reference to every non-analyzer project. Require BOTH OutputItemType=Analyzer and ReferenceOutputAssembly=false. Actual proof project initially has no product references; inspection edges must be explicit, not inferred permissions.
- Exactly five projects in KingdomZombies.slnx; independent expected inventory must also union tracked/new authored project discovery, evaluate recursive imported ProjectReference/PackageReference/properties with FullPath and DefiningProjectFullPath, and reject duplicates/missing/empty selectors before set conversion.
- No Context7 tool is exposed and ctx7 is absent. Library API documentation lookup remains to be done before implementing metadata APIs. Built-in System.Reflection.Metadata/PEReader can avoid new packages; this is a proposal, not implemented or verified. Never instantiate Godot Nodes in plain .NET inspection.
- Donor originals reside in the immutable `docs/code/provenance/langproj-baseline.zip`. ProjectEvaluator, GeneratedCodeScanner and LayerPurityTests analogs were inspected from the archive; preserve originals.
- `tools/phase1/required-cases.json` has pending graph AC12/13 and RULE05 edge rows, pending R51–56/R58–66/R74/75/77/78/81 polarities, and pending ac07.complete. Each must have a unique actual method/binding and fresh evidence; preserve inapplicable R57 register-resolution. Add graph.allowed-and-direct exact case binding.
- Current runtime test names must retain Method_State_Expected. Source policy is strict, all twelve analyzers enabled. Use small cohesive helpers and named arguments; no severity/exemption weakening.
- Include Directory.Xunit.targets in real copied test repositories; it pins the authentic producer's logical seed and receipts. Existing BuildFixture does this.

After focused tracer and graph checks, perform prerequisites first and one immutable full currently implemented suite per D10. Baseline full run had 477 pass, zero failed controls, 107 later-owner pending of 584. No new full run has happened here. Then write SUMMARY, self-check, commit it, update current latest ROADMAP/STATE while preserving Phase 2 annotation, and leave shared RULE requirements Pending. No Phase 2 work is authorized.
