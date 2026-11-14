# Plan 01-09 execution history — superseded by completed SUMMARY

**Resolved 2026-11-14:** Both tasks are complete. Final immutable run `3fa47e43-8147-43bb-b88b-46529437cfc6` accepted545 controls with zero failed controls and60 later-owner pending obligations. See [01-09-SUMMARY.md](01-09-SUMMARY.md). The continuation records below are preserved historical evidence, not the current resume point.

## Historical continuation 4

**Full closure remains incomplete: 0/2 entire tasks, no SUMMARY, no immutable full09 suite and no STATE/ROADMAP advancement.** Phase 1 stays9/18; all RULE requirements Pending. Preserve the independent Phase 2 annotation/artifacts. The eight WIP paths below remain intact; never reset/reapply prior changes.

## Immediate next action

`GraphTests.Build_UndeclaredTransitiveEngineType_ReportsCs0012` now references the built game assembly explicitly, because the original ProjectReference also supplied transitive Godot compile assets and the alleged violation successfully compiled. The current explicit `HintPath` is wrong: it uses `../godot-game/bin/Debug/net10.0/KingdomZombies.dll`. The real freshly compiled assembly was read-only verified at:

```text
.phase1/temp/build-6172b59e25a14006be312ab99b796c18/src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll
```

Change only the isolated HintPath to `../godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll`, retaining clean build before intentional type use and normal inherited strict rules. Rebuild strict solution, rerun this exact target with a fresh report, require genuine CS0012 after the clean assembly-reference subject succeeds. Tighten intended source coordinates if the observed diagnostic permits. Infrastructure/restore/style failure never passes. Current clean control fails **MSB3245**, not intended CS0012; no RED or acceptance credit.

This continuation used its fresh three-repair budget: initial three convention corrections; correction of the transitive compiler fixture assumption; wrapping the added 121-character line. The remaining path prerequisite is preserved for a fresh continuation. No user decision, authorization or package installation is missing.

## New verified results

- Initial three convention errors corrected; actual `dotnet build KingdomZombies.slnx --no-restore -p:NuGetAudit=false --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -v:q` passed **0warnings/0errors**. A final strict rebuild after the transitive-fixture edit also passed0/0. The first mistaken `.sln` command only reported missing file; it supplied no evidence.
- `.phase1/temp/09-final-affected.xml`, matching `.log` and `.exit`: actual exit2, **42executed,41passed,1failed,0skipped**,2m01s. Every one of the36 RegisterTests mutation cases passed with the final register WIP; Core-to-concrete-provider R59 negative and both provider/reusable R61/R62 polarities passed. Sole failed target was the original transitive-compiler assumption (`built.Exit` was0). Results do not cover the subsequent transitive fixture edit.
- `.phase1/temp/09-final-preflight.xml`, matching `.log` and `.exit`: actual exit0, **4/4passed**,0failed/skipped,~10s. Exact targets: all unique binding/Method_State_Expected naming; current tracked-source format; five exact LF locks unchanged by actual locked restores; actual owned analyzer source consumer with all twelve strict Error descriptors. These ran before the transitive fixture edit; later full immutable run must cover final code.
- `node tools/phase1/bootstrap.cjs --probe engine` actual exit0, raw compatibility **pass**, nonce `20a330d4-1dc9-423f-9f91-f7bc28e10b5d`, logs `.phase1/runs/20a330d4-1dc9-423f-9f91-f7bc28e10b5d`; console/exit `.phase1/temp/09-final-engine.log`/`.exit`.
- `.phase1/temp/09-transitive-final.xml`, matching `.log` and `.exit`: actual exit2, **0/1passed**,20s. New explicit assembly-reference control fails MSB3245 because HintPath is wrong, before any expected compiler violation. No acceptance.
- `git diff --check` passed. SDK git protection query with normal elevated metadata returns `false` under the recorded default-branch override; restricted sandbox metadata falsely returned protected and was not used for commits. Normal hooks remain mandatory.

## Preserved WIP and closure

Modified: `docs/code/register.json`; `tests/phase1/ArchitecturePolicy.cs`, `BoundaryFixture.cs`, `GraphTests.cs`, `ProjectEvaluator.cs`, `RegisterValidator.cs`; `tools/phase1/required-cases.json`. Intentional untracked `tests/phase1/ArchitectureRuleTests.cs` retains the18 previously passing proofs. No Task2 source commit yet. Ledger remains `.git/gsd-plan-head-before-01-09` = `286fb681f356badc1c426ffca35f2770053702bb`.

After fixing/verifying the single remaining compiler fixture, finish the current continuation3 closure procedure below: final source/bindings/diff checks as affected, normal Task2 source commit, then exactly one immutable `node tools/phase1/check.cjs --all` with fingerprinted inputs fixed. Current manifest expectations605/545implemented/60later pending remain expectations until actual reconciliation. Record actual source/tool/platform, exit and report. Pending is never passing. Only then SUMMARY, self-check, latest ROADMAP then STATE10/18/next10 preserving Phase2 context; all RULE Pending. No Linux/coverage credit. Do not kill the Linux VM shell.

---

# Prior continuation 3 (superseded)

**Full plan closure remains incomplete: no SUMMARY, no immutable full09 suite and no STATE/ROADMAP advancement.**
The evaluated/compiled Task 1 foundation is now committed as `7e2a528`; final cross-rule register bindings and Task 2 proof activation are preserved WIP. Do not treat a foundation increment as both entire tasks complete. Phase 1 stays9/18; all RULE requirements Pending; independent Phase 2 annotation/artifacts remain untouched.

## Immediate next action and exact blocking diagnostics

The final strict whole-solution build currently fails with exactly three new convention errors, no runtime RED credit:

```text
tests/phase1/BoundaryFixture.cs(34,121): SCR0007: Line is123 characters long; limit120
tests/phase1/BoundaryFixture.cs(73,121): SCR0007: Line is122 characters long; limit120
tests/phase1/ProjectEvaluator.cs(298,21): SCR0009: Wrapped argument indented20 spaces; expected28
```

Wrap `ProofProtocol.Record` location at BoundaryFixture34; wrap the collection expression/ValidateProvider call at73 using compliant +8 continuation; indent the `comparisonType:` argument at ProjectEvaluator298 to28. Do not weaken any rule/severity/exemption. The source is preserved, not reset.

This executor reached the bounded task-repair limit through the consolidated convention correction, canonical-root/locator prerequisite correction, and final semantic Core-to-provider/reusable-edge correction. These last three convention errors are handed to a fresh continuation rather than repaired beyond the limit. No permission decision or package installation is needed.

## Verified progress in this continuation

- `1ccde62` genuine compiled-type RED: strict harness0warnings/errors; clean fixture, Core-to-game edge and final typeof subject all build successfully; exact `GraphTests.Inspect_CoreEngineType_ReportsCompiledLeak` fails intended ShouldContain. Actual exit2, unchanged supported report `.phase1/temp/09-type-red.xml`, record `09-type-red.json` with report/run/modification timestamps; classifier **RED_EVIDENCE_OK**. Sandbox restore denial was infrastructure only, then successful elevated execution supplied real RED.
- `c6280d4` compiled metadata GREEN: exact target1/1pass (~20.7s). Built-in PEReader/MetadataReader inspect trusted freshly compiled metadata without loading assemblies/constructing Nodes. Independent subject guards and full assembly/type token inventories catch body/generated dependencies. No package acquisition.
- `c42201d` expansion RED: actual clean graph and compilable Core OutputType=Exe plus Entry.Main both build; exact `Evaluate_CoreExecutable_ReportsOutputKind` fails intended GRAPH_OUTPUT_KIND assertion. `.phase1/temp/09-expansion-red.xml/json`, exit2, **RED_EVIDENCE_OK**. Exact focused GREEN later1/1pass (~14.4s), report `09-expansion-green.xml`.
- `7e2a528` verified evaluated/compiled foundation: independent five-project/type policy; tracked+new authored inventory; recursive real evaluated imports/origins; both analyzer metadata; output kinds; all compiled leak shapes; ports/providers/helper/locator mechanisms; concrete architecture Models and exact visibility registry. Source compiles strictly before subsequent WIP changes.
- Canonical `node tools/phase1/check.cjs --group graph` **24/24pass, exit0**, report `.phase1/runs/40a8ea85-dcb0-4d1e-8a34-3d17d4353a94/summary.json`. Revision `c42201d761043d7d7716a4a35c0b36e5f8735d41` plus then-uncommitted foundation; input `c26ae20f686da75e458d404b5aba02cc77583bd196ff813d4f992055b0d3b11d`; tool `1bc1550a3487ad6bcfa66506ba802fe26ba79f3775fa104be4c76dc8708f1e32`; actual `win32-x64-10.0.19044`. This is NOT evidence for subsequent WIP.
- Raw focused structure/evolution `.phase1/temp/09-structure-focused.xml`: **18/18pass**,0failed,~2m08s. Actual copied strict model compilations precede intentional private-mutation/mapping/enum/version/config failures.
- Raw boundary `.phase1/temp/09-boundary-focused.xml`: **23/23pass**,0failed,~4m35s. Distinct R51–62/R78 methods (R52negative is the separate exact executable test).
- Final affected direct targets **5/5pass** after adding real allowed Core+port composition, missing project file, policy+reference permutation, exact CS0246 coordinates and strict visibility registry fields. Source then strictly compiled0warnings/errors. Targets: Build_ExplicitCoreAndPortComposition_Accepted, Evaluate_MalformedPolicySelectors_ReportsRequired, Evaluate_PermutedReferences_PreservesVerdict, Rule53_CompilerViolation_Rejected, ArchitectureRuleTests.Rule34_DeclaredVisibility_Accepted.
- Earlier first broad GraphTests run was stopped after repeated common safe-write prerequisite failures; it is not complete-suite evidence. Only its identified dotnet process27948 and descendants were stopped. The Linux VM shell was not touched.

## Preserved WIP, including unverified final changes

`git status --short` at handoff:

```text
 M docs/code/register.json
 M tests/phase1/ArchitecturePolicy.cs
 M tests/phase1/BoundaryFixture.cs
 M tests/phase1/GraphTests.cs
 M tests/phase1/ProjectEvaluator.cs
 M tests/phase1/RegisterValidator.cs
 M tools/phase1/required-cases.json
?? tests/phase1/ArchitectureRuleTests.cs
```

Do not discard or redo this work. The untracked ArchitectureRuleTests file contains all18 already-passing structural/default-visibility/evolution proofs; it still needs Task2 commit. The JSON manifest currently declares **605 cases,545 csharp implementations,60 later-owner pending** (including the newly added, not-yet-run CS0012 proof). No full suite of that inventory has run.

Final register WIP now points R34/R51–56/R58–66/R74/75/77/78/81 at actual ProjectEvaluator/TypeBoundaryScanner/ArchitecturePolicy/GraphTests methods and real GraphTests versus ArchitectureRuleTests proof paths. RegisterValidator has the independent `_architecture` mapping and matching expected proof paths. All register mechanism/proof status and qualitative-review status remain Pending; R57 stays inapplicable. This binding update has not yet passed the register mutation group.

Two necessary semantic corrections also await verification after the three convention fixes:

1. `R-59.negative` now calls `BoundaryFixture.CoreProvider`, compiling an actual **Core -> concrete provider type**. The earlier passing R59 test inspected provider->Core, which correctly tested provider isolation but did not exercise the intended R59 direction. New helper compiles a clean control/provider first, then the actual Core type use and expects TYPE_FORBIDDEN naming KingdomZombies.Provider.
2. Isolated provider/reusable-helper proofs now require both actual evaluated graph verdict and compiled-type verdict. `ProjectEvaluator.ValidateProvider(..., reusable:true)` grants no product edges and evaluates actual PackageReference items, preventing an unused forbidden reference from disappearing during compilation. Product-named packages are rejected for the reusable analyzer/helper. Original five-project graph remains unchanged.

The new actual `graph.transitive-compiler` method `Build_UndeclaredTransitiveEngineType_ReportsCs0012` has **not been compiled/run yet**. It uses the already verified clean+Core-to-game control then accesses Phase1's inherited Node member from Core without a direct GodotSharp reference, requiring intended CS0012 and no MSB/NU infrastructure failure. Inspect its real outcome; adjust only within the fresh bounded repair budget if needed. The existing CS0246 direct-absence case already passed exact Subject.cs(8,23).

## Remaining closure procedure

1. Fix only the three convention diagnostics, strict whole solution, then focused actual CS0012/Core-provider/reusable cases and full RegisterTests mutation group. Preserve genuine RED records/commits; no additional accepting scaffolds are needed.
2. Verify unique manifest IDs/targets and Method_State_Expected; check tracked source/format, git diff, exact LF locks via affected restore check, native `node tools/phase1/bootstrap.cjs --probe engine`, and actual twelve-analyzer source consumer. The final strict solution prerequisite must pass. Do these prerequisites before the one full suite.
3. Commit the remaining Task2 proof/manifest/register/wiring files with normal hooks, exact supplied-root pin and protected-branch override check. Source is intentionally WIP now. Avoid fabricated complete-task/SUMMARY claims before fresh checks.
4. Run one immutable `node tools/phase1/check.cjs --all` per D10. Expected current inventory605 includes545 implemented +60 later-owner pending; counts are expectations only until executed. Collect actual console/exit/report and identities; no fingerprinted source/docs/spec/config edits during that run. Later pending obligations never count as passing. AC07 complete remains honestly later-owner pending while state/lifecycle/docs/etc mechanisms are unimplemented.
5. On actual success of implemented controls, write01-09-SUMMARY to disk, self-check, update latest ROADMAP first and STATE to10/18/next10 while preserving exact Phase2 annotation/artifacts. All RULE requirements remain Pending. Ledger `.git/gsd-plan-head-before-01-09` remains authoritative `286fb681f356badc1c426ffca35f2770053702bb`; measure commits/diff actuals, do not narrate counts. Capture final docs commit normally. No Linux/coverage pass yet.

Official API references and earlier provenance details below remain applicable. Newly authored visibility permissions are exact source/type entries in `docs/code/visibility-exemptions.json`; no original generator permission or donor archive was broadened/modified. R63/64/65/74/75/77/81 models are concrete compiled test-only Models; production ExampleState/ProofKind/SnapshotDto remain owned by10/11.

---

# Prior continuation 2 (superseded)

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
