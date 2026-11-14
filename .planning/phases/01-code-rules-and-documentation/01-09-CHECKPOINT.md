# Plan 01-09 execution checkpoint

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
