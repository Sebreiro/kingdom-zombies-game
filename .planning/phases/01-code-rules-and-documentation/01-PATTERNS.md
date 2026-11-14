# Phase 1: Code rules and documentation - Pattern Map

**Mapped:** 2026-11-13
**Files analyzed:** 20 implementation units (directory rows cover related proposed files)
**Analogs found:** 7 / 20 units; 6 selected analog files

This is a planning map, not an implementation inventory. Names beyond the existing starter and documentation are proposals from RESEARCH.md. The five-project topology is recommended within SPEC.md; it is not an existing architecture. No gameplay C# patterns exist locally.

## File Classification

| New/Modified File or implementation unit | Role | Data Flow | Closest Analog | Match Quality |
|---|---|---|---|---|
| `KingdomZombies.slnx`, five project declarations | config | transform | None | greenfield |
| `global.json`, tool manifest, engine/toolchain identity input, lock files | config | file-I/O | None locally; research donor pins | greenfield |
| `Directory.Build.props`, `Directory.Packages.props` | config | transform | None selected; adopt captured donor configuration per research | greenfield |
| `Directory.Build.targets` | config | transform | Donor `Directory.Build.targets` | exact wiring pattern |
| Root `.editorconfig`, existing game `.editorconfig`, banned-symbol inputs | config | transform | None sufficient; starter only declares UTF-8 | greenfield |
| `src/core/` state, identity, time/random interfaces, detached snapshot validation | model/service | transform | None | greenfield |
| `src/multiplayer-abstractions/` independent service ports/value contracts | service/model | request-response | None | greenfield |
| `src/godot-game/KingdomZombies.csproj`, `project.godot` | config | transform | Existing `project.godot` | role-match |
| `src/godot-game/` proof driver, fixture Node/Resource, scene/resource files | component/controller | event-driven | None | greenfield |
| `src/godot-game/` thread gateway and lifetime token | service | event-driven | None | greenfield |
| `tools/code-rules/` adopted analyzers/helpers/release manifest | utility | transform | Donor source inventory in research; capture before adaptation | greenfield locally |
| `tests/phase1/` analyzer verifier and positive/negative cases | test | transform | Donor `AnalyzerVerifier.cs` | exact proof pattern |
| `tests/phase1/` evaluated graph checker/helper | test/utility | batch | Donor `ProjectEvaluator.cs` | exact proof pattern |
| `tests/phase1/` rule/register/exemption, documentation, real-build proofs | test | batch/file-I/O | None selected; research identifies donor helpers to adopt | greenfield locally |
| `tests/phase1/` state, lifecycle/result validation and coverage checks | test | transform/event-driven | None | greenfield |
| `tools/phase1/check.cjs`, bootstrap, required-case manifest and run reports | utility/config | batch/file-I/O | None | greenfield |
| `docs/code/` index, disposition register, exemptions, review checklist and captured provenance | config/reference | file-I/O | `docs/workflow/gsd.md` for authored navigation only | role-match |
| `docs/architecture/` boundaries and ADRs | config/reference | file-I/O | None selected; research donor ADR is provenance | greenfield |
| `docs/workflow/` setup and verification pages; per-project `docs/README.md` | config/reference | file-I/O | `docs/workflow/gsd.md` | role-match |
| Canonical root `AGENTS.md` and indexed collaboration links | config/reference | file-I/O | Tracked `Agents.md` | exact navigation pattern |

Counts refer to these 20 units, not a promise of 20 physical files. Split the grouped units into concrete files in plans. Do not add a project or gameplay fixture merely to match a row.

## Pattern Assignments

### Documentation indexes and root guide

**Analogs:** tracked `Agents.md` and `docs/workflow/gsd.md`. These are repository documentation, not executable rules. Preserve the short task-to-authority index and separate command pages. `docs/workflow/ai-collaboration.md` already contains the interaction contract; preserve it while fixing links to canonical root casing.

`Agents.md`, lines 5-11:

```markdown
This is the documentation entry point. Open the documents relevant to the current task.

| Need | Read |
| --- | --- |
| AI collaboration, questions, and decision prompts | [AI collaboration](docs/workflow/ai-collaboration.md) |
| Required game behavior, balance baseline, exclusions, and acceptance scenarios | [Version 0.1 specification](specs/0.1/initial-spec.md) |
| Required assets, recovered donors, and production checks | [Asset and resource plan](specs/0.1/assets-and-resources.md) |
```

`docs/workflow/gsd.md`, lines 21-25:

```markdown
## Planning Artifacts

| Artifact | Purpose |
| --- | --- |
| [PROJECT.md](../../.planning/PROJECT.md) | Project definition, scope authority, constraints, and decisions. |
```

Apply that link-and-purpose pattern to authored indexes and each project's reference page. Do not duplicate executable rules as a second prose authority or add a consolidated walkthrough. New setup commands belong to setup; proof commands belong to verification. Actual link checking, ordinal Git casing, UTF-8 handling and register completeness are new implementation, not existing documentation capabilities.

### Existing Godot launch target

**Analog:** `src/godot-game/project.godot`, lines 11-15 and 22-24.

```ini
[application]

config/name="KingdomZombies"
config/features=PackedStringArray("4.7", "Forward Plus")
config/icon="res://icon.svg"
```

```ini
[dotnet]

project/assembly_name="KingdomZombies"
```

Reuse the path and assembly identity. Feature tags are starter declarations, not compatibility evidence or final version pins. The project has no main scene or C# script to imitate. Add one minimal engine proof under this target; runtime construction, authentic generated partials, callbacks, exports, signals, two instance cycles and late worker-result rejection all require new code and actual engine assertions.

### Analyzer-only build reference

**Analog:** donor `src/backend/lang-proj-core/Directory.Build.targets`, lines 1-6 (relative to `D:/Work/SaaS/LangProj_monorepo`).

```xml
<Project>
  <ItemGroup Condition="'$(MSBuildProjectName)' != 'Sebreiro.CodeRules.Analyzers'">
    <ProjectReference Include="$(MSBuildThisFileDirectory)packages/Sebreiro.CodeRules.Analyzers/Sebreiro.CodeRules.Analyzers.csproj"
                      OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
  </ItemGroup>
</Project>
```

Adapt the path to `tools/code-rules/`; retain analyzer-only metadata and the self-reference exclusion. Prove evaluated imports and a real bad consumer build; mutation of either metadata field must fail. Capture the source/configuration locally with provenance before relying on it. Never leave a build reference into the donor folder.

### Analyzer tests and imports

**Analog:** donor `src/backend/lang-proj-core/packages/Sebreiro.CodeRules.Analyzers.Tests/Verification/AnalyzerVerifier.cs`.

Imports, lines 1-9:

```csharp
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Sebreiro.CodeRules.Analyzers.Tests.Verification;
```

Core verification, lines 41-57:

```csharp
var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
{
    TestCode = source,
    ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
};
test.TestState.OutputKind = outputKind;
if (expected is { } diagnostic)
{
    test.ExpectedDiagnostics.Add(item: diagnostic);
}

if (editorConfig is not null)
{
    test.TestState.AnalyzerConfigFiles.Add(item: ("/.editorconfig", SourceText.From(text: editorConfig)));
}

return test.RunAsync(cancellationToken: TestContext.Current.CancellationToken);
```

Preserve exact expected diagnostic matching and failure on unexpected compiler errors (file's contract, lines 11-14). Select reference assemblies only after the integrated toolchain proof; the excerpt does not certify the proposed runtime. Use the donor's named arguments, file-scoped namespaces and explicit cancellation conventions. Copy helper dependencies and tests into the owned proof project rather than creating another analyzer test project. This helper does not replace real-build severity/import proofs.

### Evaluated project graph and actionable failure

**Analog:** donor `src/backend/lang-proj-core/LangProject.Core.ArchitectureTests/Projects/ProjectEvaluator.cs`.

MSBuild process arguments, lines 184-198:

```csharp
List<string> arguments =
[
    "msbuild",
    Path.Combine(path1: solutionRoot, path2: relativePath),
    "-nologo",
    "-p:Configuration=Release",
    "-getItem:ProjectReference",
    "-getItem:PackageReference",
    .. _propertyNames.Select(selector: name => "-getProperty:" + name),
];
ProcessResult result = await DotnetQuery.RunAsync(
        arguments: arguments,
        workingDirectory: solutionRoot,
        timeout: _evaluationTimeout,
        cancellationToken: cancellationToken);
```

Error handling, lines 199-205:

```csharp
if (result.ExitCode != 0)
{
    throw new InvalidOperationException(
            message: $"MSBuild could not evaluate {relativePath}: {result.Output}");
}

return Parse(solutionRoot: solutionRoot, relativePath: relativePath, json: result.StandardOutput);
```

The source additionally unions solution paths with tracked project discovery (lines 54-71), reads reference `FullPath` and `DefiningProjectFullPath` (lines 118-127), and checks both analyzer metadata values (lines 223-228). Adapt repository roots and remove the donor-specific frontend exclusion. Keep evaluated imports/items; csproj XML scanning is insufficient. Adopt the needed process/record helpers as a coherent set. Add independent declared subjects, recursive closure, compiled type/IL inspection and negative fixtures per research; this evaluator alone does not prove those obligations.

## Shared Patterns

- **Production boundaries:** Core and Multiplayer.Abstractions are independent framework-only assemblies; Godot composes both. Analyzer edges are tooling edges. Tests may inspect the game assembly without constructing engine objects in a plain .NET process. The architecture is new: use RESEARCH.md's boundary table, not the donor's web topology.
- **Rule provenance:** Research records clean donor commit `6836a9718ff5c939e89abf2b97763760607de192`, exact document hashes, 84 heading occurrences, 85 IDs and 24 exemption rows. Capture the complete sources and selected reusable source/tests locally; do not treat this map or research's excerpts as the captured baseline.
- **Validation before acceptance:** New register, exemption, snapshot and result parsers must reject duplicates before set conversion, preserve ordinal identity, and validate complete detached candidates. Snapshot acceptance replaces one root atomically; invalid/interrupted candidates leave live state unchanged. No existing local implementation supplies this behavior.
- **Trustworthy verification:** New required-case manifest independently defines obligations. Fresh run identity, tool/input identity, isolated logs, exact case reconciliation, real process failures and atomic result publication are required. An unrelated exception is never a successful negative proof. Collect independent failures and report blocked prerequisites explicitly.
- **Godot lifetimes:** Validate required nullable exports before use, distinguish optional values, guard main-thread engine access and reject disposed-generation results. Research code examples are proposed starting points, not tracked production analogs. Do not apply context-free `ConfigureAwait(false)` policy blindly to engine continuations.
- **No auth surface:** Authentication, HTTP response formatting and database transactions have no Phase 1 applicability. Do not import donor middleware, DI hosts or EF topology.
- **Coverage and thresholds:** Preserve research's exact donor semantics, including 60 body lines and 90% lines/80% branches. Keep qualitative review obligations explicitly human-reviewed. Actual Godot-hosted managed coverage and Windows/Linux execution remain proof work.

## No Analog Found

The 13 greenfield units in File Classification have no selected sufficient analog. Most significantly, there is no local C# state model, service port, Godot lifecycle/thread fixture, test project, bootstrap/check runner or portable documentation validator. Use SPEC.md AC-01–AC-22 and RESEARCH.md's responsibility map, examples and validation architecture. The donor source inventory supplies adoption candidates for configuration/analyzers/checks; those are external provenance inputs, not evidence of an existing local implementation.

## Metadata

**Analog search scope:** tracked root guide, `docs/workflow/`, `src/godot-game/`; exact donor files named above. Installed GSD files were excluded as game implementation patterns.

**Tracked-source gate:** `git ls-files -- <path>` returned the local analog paths. `git -C D:/Work/SaaS/LangProj_monorepo ls-files -- <path>` returned all three donor analog paths. No ignored runtime mirror is named as an analog.

**Strong analog sources:** 6 physical files: two documentation pages, Godot configuration, donor targets, donor verifier and donor evaluator. Coverage counts above refer to implementation units, not physical analog files.

**Pattern extraction date:** 2026-11-13. No build, compatibility or platform execution is claimed. No source files changed and no commit performed.
