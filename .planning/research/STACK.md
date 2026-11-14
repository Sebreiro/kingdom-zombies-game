# Stack Research

**Domain:** Godot/C# desktop survival and camp-management game; solo and four-player LAN
**Researched:** 2026-11-13
**Confidence:** MEDIUM

## Recommended Stack

Use **Godot .NET 4.7.2, Godot.NET.Sdk 4.7.2, .NET SDK 10.0.404, explicit C# 14.0, and a proposed net10.0 target**, with engine-independent gameplay assemblies and Godot adapters. Pin the engine, SDK, packages, export templates, and asset tools together. This is a researched candidate baseline, not a claim that this repository already builds or exports successfully. Phase 1 must prove the complete combination on Windows and Linux before treating it as validated.

The repository contains a starter `project.godot`, not an established C# build: its `4.7` feature tag, Forward Plus renderer, Windows D3D12 setting, and assembly name do not identify the engine executable, SDK, generated bindings, or target framework. Local inspection found SDK **10.0.404** installed. No game project, analyzer integration, engine test suite, or export proof was performed by this research.

### Core Technologies

| Technology | Version | Purpose | Why Recommended |
|------------|---------|---------|-----------------|
| Godot .NET editor/runtime | **4.7.2-stable**, Windows/Linux x64 initial targets | Scene tree, 2D rendering, input, UI, audio, desktop exports | Current stable release on the official archive, dated 2026-08-18. Matches the starter's 4.7 family. Use the .NET distribution, including its supporting files. [S1, S2] |
| Godot.NET.Sdk | **4.7.2** | MSBuild integration, Godot bindings/source-generation integration | Published exact package; keep synchronized with the engine instead of independently advancing it. [S3] |
| .NET SDK / runtime / language | **10.0.404 / 10.0.15 / C# 14.0** | Compiler, MSBuild, managed runtime, pure gameplay/tooling | Microsoft's release dated 2026-11-10 includes this runtime and language. Use an explicit SDK/language pin instead of inheriting `latest`. [S4] |
| Game/domain target framework | **net10.0**, pending Phase 1 proof | Consistent runtime API surface for game, rules, tools, tests | A newer target than Godot's minimum is permitted; SDK selection and runtime targeting are distinct. Confirm generated code, native hosting, editor reload, and exported execution, not merely NuGet compatibility. [S5, S6] |
| Godot export templates | **4.7.2 .NET**, exact matching release | Windows/Linux runnable delivery | Acquire the .NET templates with the editor; resource-only PCK export is not a playable deliverable. [S2, S7] |
| Godot Compatibility renderer | **4.7.2**, `gl_compatibility` candidate | Pixel-art 2D and broad desktop hardware support | Official guidance identifies Compatibility as a good starting point for 2D. Prefer it over the starter's unproven Forward Plus/D3D12 default; validate intended lighting, particles, palette effects, and crowd scenes before finalizing. [S8] |
| Godot ENetMultiplayerPeer + MultiplayerAPI | Engine **4.7.2** built-ins | LAN host/client transport and RPC infrastructure | Avoid a second networking dependency for four LAN players. Keep authority, command validation, identity, and replication rules in project code. [S9] |
| PacketPeerUDP | Engine **4.7.2** built-in | LAN discovery advertisements and discovery requests | Supports broadcast; implement the session advertisement protocol separately from gameplay. Manual address joining remains required when discovery is unavailable. [S10] |
| System.Text.Json + file storage | Included in **.NET 10**, no extra JSON package | Designer configuration, explicit save DTOs, manifests and test results | Adequate for documented editable data and versioned snapshots; avoids a database service. Stable IDs, migration, validation, backup/replacement, and coherent capture remain game responsibilities. [S11] |

All external capability and version findings above have **MEDIUM research confidence**, with official sources cross-checked; final project compatibility remains unproven. Architectural choices are recommendations inferred from this project's scope, not benchmark findings. Windows/Linux x64 is a proposed delivery baseline; additional CPU architectures need an explicit requirement and validation matrix.

### Supporting Libraries

Keep gameplay dependencies small. Use the inherited analyzer/testing approach without importing LangProj's ASP.NET, HTTP, EF, PostgreSQL, dependency-injection graph, or integration-test containers.

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| `xunit.v3` | **4.0.2** | Pure C# rules, inventory/economy invariants, save/config contracts, architecture and analyzer tests | Existing LangProj pin; independently confirmed release with MTP 2.5.1. Run these tests without Godot. This is a deliberate reusable pin, not a claim of latest available release. [S12] |
| `Shouldly` | **4.3.0** donor pin | Readable pure-test assertions | Retain if copied tests benefit; observed locally, package restore still needs Phase 1 proof. |
| `TngTech.ArchUnitNET` | **0.13.4** donor pin | Assembly/dependency boundary checks | Useful for proving gameplay has no Godot or transport dependencies; do not copy web-layer expectations. Observed locally; restore/proof pending. |
| `Sebreiro.CodeRules.Analyzers` | Repository-owned source snapshot, revision/hash to record in Phase 1 | Exact inherited custom rules | Copy/adapt analyzer sources and relevant positive/negative tests into this repository; no absolute project reference to LangProj. Keep its analyzer target `netstandard2.0`. |
| Roslyn packages for inherited analyzer | `Microsoft.CodeAnalysis.CSharp` **5.0.0**; `.Workspaces` **5.0.0**; `Microsoft.CodeAnalysis.Analyzers` **5.9.0**; `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing` **1.1.4** | Analyzer implementation and compilation fixtures | Exact locally observed donor pins. Prove load and diagnostics under SDK 10.0.404's compiler; package existence alone does not prove analyzer host compatibility. |
| Global analyzer packages | `Meziantou.Analyzer` **3.0.312**; `Roslynator.Analyzers` **5.0.1**; `Microsoft.CodeAnalysis.BannedApiAnalyzers` **5.6.0** | Inherited style, correctness, banned API enforcement | Candidate retained pins from donor `Directory.Packages.props`. Scope application to authored code and prove generated-code treatment. Do not claim these are current upstream latest versions. |
| `gdUnit4.api` | **5.1.0-rc7** | Godot runtime, scene and input tests | Recommended engine-test candidate; prerelease published 2026-11-07. Supports net10/C#14 and Godot 4.7+. Isolate test dependencies from exported game. [S13] |
| `gdUnit4.test.adapter` + `gdUnit4.analyzers` | **3.2.0** + **1.1.0** | VSTest execution and test-specific compile checks | Published versions matching the maintainer's compatibility table. Engine suite only. [S14, S15, S16] |
| `Microsoft.NET.Test.Sdk` | **18.0.1** candidate pin | VSTest host for engine tests | Published version consistent with adapter 3.2.0's TestPlatform dependency minimum. Exact restore/discovery/execution remains a Phase 1 proof item; do not copy the older 18.0.0 setup example blindly. [S14, S17] |

LangProj also pins `Microsoft.Testing.Extensions.CodeCoverage` **18.12.0** and `Microsoft.Testing.Extensions.HangDump` **2.5.1**. Treat these as optional **pure-test tooling candidates**, not game or engine-suite dependencies. Verify their MTP pairing when enabling them; release acceptance is scenario/evidence coverage, not a coverage-percentage target.

### Development Tools

| Tool | Purpose | Notes |
|------|---------|-------|
| Central NuGet management + lock files | Reproducible restore and reviewable dependency changes | Adapt donor `Directory.Packages.props`, `Directory.Build.props/targets`, `.editorconfig`, and banned API files. Use locked restore in CI. Store explicit SDK, engine and template versions in one documented toolchain manifest. |
| .NET CLI + an external C# IDE | Build, debug, format, inspect diagnostics | CLI is the reproducibility contract; IDE choice need not become a game dependency. Godot's own C# editor support is limited. [S18] |
| GitHub Actions | Windows/Linux build, tests, exports and retained diagnostics | Start with explicit `windows-2025` and `ubuntu-24.04` runner labels, not moving `*-latest` aliases. Install the same pinned toolchain on both. Record runner image versions because labels do not freeze VM contents. [S19] |
| Repository-owned .NET process orchestrator | Host plus three guest E2E scenarios | Launch four actual game processes with unique user-data/log directories, session/port assignments, readiness handshakes, controlled actions, deadlines, assertions and cleanup. Reuse for local and CI runs. A unit-test adapter does not supply this application-specific protocol. |
| Godot headless CLI and separate rendered runs | Simulation/network tests and visible acceptance evidence | Headless tests are suitable for logic and many scene/input checks; retain rendered menu, asset, focus, layout, shader and viewport checks. [S13, S20] |
| UnityPy | **1.25.4**, optional offline extraction tool | Published 2026-10-03; scriptable Unity asset decoding. First reuse any working recovered extraction tooling. If needed, pin UnityPy and all Python dependencies, verify sample sprites/audio/fonts/animation metadata, and freeze the working Python version in Phase 4. It is not a shipped runtime dependency. [S21, S22] |
| Godot profiler + project performance recording | Real target-PC measurements and endurance diagnostics | Record hardware, OS, rendering backend, build mode, actual frame-time samples, population, network load, save/join timing and memory. No selected framework proves the 30-FPS requirement. |

### C# rule adaptation contract

Local donor evidence: root `global.json` specifies **10.0.402**, `latestPatch`, and the **Microsoft.Testing.Platform** runner. Backend properties specify **net10.0**, nullable enabled, implicit usings disabled, `LangVersion=latest`, warnings as errors, documentation generation, style-in-build, `AnalysisLevel=latest-recommended`, disabled transitive project references and package lock files. The custom analyzer is added as an analyzer-only project reference.

Retain strict enforcement, but replace moving language/analysis choices with documented values matched to the selected compiler. Produce a rule matrix covering ordinary domain classes, Godot partial node/resource classes, generated files, tests and tools. Preserve positive and deliberately failing real-build checks. Specifically prove engine constructors and callbacks, export/serialization initialization, partial/source-generated code, documentation rules, disposal/lifetime and main-thread constraints. Exemptions must name their rule, scope, reason and proof; blanket `NoWarn`, disabling analyzers for all Godot code, or suppressing `NU1605` is not the adaptation strategy.

## Installation

This section is the **planned acquisition and validation sequence**, not a claim that the files or commands already exist. Complete it in Phase 1, preserve early smoke checks in Phases 2–3, and expand automation in Phase 5.

1. Acquire the official **Godot 4.7.2 .NET** archive for each OS and the matching **4.7.2 .NET export templates** from the versioned official archive. Preserve the full extracted editor directory. Record download URLs, SHA-256 values, and `--version` output; point a local `GODOT_BIN` variable at the selected executable. Resolve that variable during CI setup rather than checking in a machine path.
2. Install **SDK 10.0.404** from Microsoft's versioned download/installer. Check in a `global.json` pin with prereleases disabled and a deliberate roll-forward policy; exact `disable` is recommended for CI reproducibility. Updates change the pin and rerun the proof matrix. Do not mistake the SDK version for the target framework or assume the Godot download supplies a compiler. [S4, S6, S18]
3. Create the Godot C# project with `Godot.NET.Sdk/4.7.2`; set the proposed `net10.0` and C# `14.0` explicitly. Add pure domain/test projects outside the Godot resource tree. Vendor the selected analyzer source/tests and preserve source revision and provenance.
4. Restore exact NuGet versions centrally; commit lock files. Run compiler/analyzer positive and negative proof tests, a generated Godot partial-node test, a Resource/scene load, and Windows/Linux development and release-export smoke checks. Verify the exported game on a clean machine without an SDK or donor checkout.
5. Add the candidate gdUnit4Net suite separately. Verify test discovery count, an intentional assertion failure, engine startup failure, timeout, cleanup and resource loading. If this prerelease cannot pass the proof, use a small repository-owned Godot scene harness while keeping the same acceptance catalog; do not downgrade the entire game invisibly or leave required cases skipped.

Illustrative CLI sequence after project/preset creation, with `godot` resolving to the verified .NET binary and the shown test paths being proposed names:

```text
dotnet --version
godot --version
godot --headless --path src/godot-game --editor --import
dotnet restore KingdomZombies.sln --locked-mode
dotnet build KingdomZombies.sln --no-restore
dotnet run --project tests/KingdomZombies.Domain.Tests --no-build
dotnet test tests/KingdomZombies.Godot.Tests --no-build --settings tests/godot.runsettings
godot --headless --path src/godot-game --export-release "Windows Desktop" <existing-output-dir>/KingdomZombies.exe
godot --headless --path src/godot-game --export-release "Linux" <existing-output-dir>/KingdomZombies.x86_64
```

Preset names must match the checked-in `export_presets.cfg`; create output directories first. Use editor builds for import/export. Read the pinned binary's `--help` and verify command results: Godot can silently ignore unknown switches. These are separate invocations, not a claim of a single universal test-runner command. [S7, S20]

**Test runner boundary:** Leave the repository's default `dotnet test` mode as VSTest for the engine suite. Invoke xUnit's MTP-capable test executable directly (`dotnet run --project ...`), or use a separately scoped MTP working directory with the same SDK pin. Do not copy LangProj's root `test.runner=Microsoft.Testing.Platform` into a mixed solution: .NET 10 MTP mode only supports MTP applications, whereas the gdUnit adapter implements VSTest. `global.json` resolution follows the invocation's working directory; wrappers must control it. [S6, S14, S23, S24]

For asset production, use a dedicated locked Python environment only if the extraction tool requires it. Acquisition/generation happens offline during development. Commit or reproducibly acquire selected normalized outputs into the project content tree, with stable IDs, original owning-file/object identifiers, transformation recipes and hashes. `D:/Work/Games/KindomCrown/recovered` and `D:/Work/SaaS/LangProj_monorepo` remain provenance/source locations, never runtime, CI, build-import or absolute project-reference dependencies. Export filters must explicitly include required editable configuration/catalog files and exclude source archives and test-only content.

## Alternatives Considered

| Recommended | Alternative | When to Use Alternative |
|-------------|-------------|-------------------------|
| Godot .NET 4.7.2 stable | Godot 4.8 prerelease | Only for a documented blocking fix after proving the entire toolchain again. The archive currently lists 4.8 beta3, not a stable production baseline. [S1] |
| Compatibility renderer candidate | Forward Plus or Mobile | If representative art/shader requirements or measured target-PC results justify it. Compare real scenes; do not infer performance from renderer labels. [S8] |
| Pure xUnit plus gdUnit4Net engine suite | All tests in gdUnit4Net | Only when reducing frameworks outweighs engine/test-runtime coupling. Here inherited analyzer proof tests and large deterministic rule suites favor ordinary .NET execution. |
| gdUnit4Net candidate | Small custom Godot harness | Use if the release-candidate integration cannot meet proof requirements. The custom harness still needs production input/scene execution, assertions, diagnostics, timeout handling and separate four-process orchestration. |
| ENet built into Godot | Third-party transport or platform SDK | Revisit when Steam/Epic services are actually implemented or measurements expose a limitation. Preserve independent discovery, identity and connection interfaces now. |
| JSON snapshots/configuration | SQLite or custom binary saves | Revisit only if measured snapshot size/time requires it. Neither solves coherent capture, legacy inventory preservation or migration automatically. |
| Minimal collections/state machines | ECS framework, behavior-tree add-on, full navigation middleware | Add only after profiling or NPC behavior complexity demonstrates a need. The side-view authored map and 400-actor target alone do not prove such a dependency is necessary. |

## What NOT to Use

| Avoid | Why | Use Instead |
|-------|-----|-------------|
| Standard/non-.NET Godot editor or templates | Wrong distribution for C# builds/exports | Exact .NET distribution and templates |
| Unity runtime assemblies or direct recovered MonoBehaviour ports | Donor runtime, serialization, lifecycle and original rules differ | Port useful algorithms/data into tested C# and recreate Godot presentation |
| Absolute paths into either donor repository | A clean checkout or Linux CI cannot reproduce the build | Repository-owned source/assets plus provenance and locked acquisition |
| Global MTP selection with an assumed VSTest-compatible suite | Can prevent engine tests from running through the intended adapter | Explicit runner boundary and expected-test-count checks |
| Stable gdUnit4.api 5.0.0 assumed compatible with Godot 4.7 | Maintainer table lists an older supported engine range | Verify the 5.1.0-rc7 line or use the bounded custom-harness fallback [S16] |
| Website DOM testing as the sole game E2E strategy | Native Godot scenes have no browser DOM | Godot input/scene checks plus native rendered inspection |
| Headless screenshots or software-renderer FPS as release evidence | No headless rendering; software/hosted environment differs from reference hardware | Rendered image evidence and measured target-PC sessions |
| EF/PostgreSQL/ASP.NET/CQRS infrastructure copied from LangProj | Adds unrelated service architecture to a desktop authoritative game | Small domain/application boundaries and file-backed snapshots |
| Peer-controlled inventory outcomes or NodePath as durable identity | Transfer/succession/reconnect/save correctness requires explicit state ownership | Host-validated operations and stable game/content/player identifiers |

## Stack Patterns by Variant

**For solo:** run the same authoritative simulation and commands locally. Use an offline session adapter; keep saves and inventory rules identical to multiplayer.

**For LAN:** one playable host plus up to three guests. ENet carries session/game traffic; a separate discovery adapter advertises sessions. Use host-validated commands, versioned snapshots/deltas and stable identities. Reliability/order channels, limits, content handshake, late-join transfer and reconnect protocol require Phase 3/feature-specific design, not another backend service.

**For fast feedback:** use pure .NET tests for state transitions and invariants; use headless Godot for engine-dependent cases that do not need rendering. The gdUnit rc7 notes describe input through a SubViewport and warn that rendered output still needs a display. Keep screenshot and native window-focus cases in the rendered lane. [S13]

**For repeatable CI:** run build/analyzer tests, engine import/tests, and four-process loopback cases on Windows/Linux; archive case IDs, logs, test reports, seed, toolchain/content versions and failure evidence. A missing engine, zero tests, skipped required test, crash, timeout or absent artifact is a failure/blocked result, never a pass. Hosted Linux rendered checks may use a virtual display and software GL after proving the environment; classify them as functional visual evidence, not GPU benchmarks.

**For release evidence:** add a real mixed Windows/Linux LAN session, rendered UI/asset checks at specified resolutions, full campaign verification, reference-PC crowd/frame-time measurements and three-hour save/resume endurance with persistent loot. Store the case catalog in Phase 5, mark future feature cases planned with owners, and activate them as the accepted later phases deliver features. Neither four processes on one VM nor a framework's input simulation replaces these environments.

## Version Compatibility

| Package A | Compatible With / Proposed Pair | Notes |
|-----------|--------------------------------|-------|
| Godot .NET **4.7.2** | Godot.NET.Sdk **4.7.2** + .NET export templates **4.7.2** | Release/package availability verified; clean project/export execution still required. |
| SDK **10.0.404** | runtime **10.0.15**, C# **14.0**, proposed **net10.0** | SDK/runtime/language bundle verified; Godot-specific combination needs Phase 1 editor/generated-code/export proof. |
| Analyzer **netstandard2.0** target, Roslyn **5.0.0** reference | SDK **10.0.404** compiler host | Donor pins observed; loading, rule behavior, incremental builds and source-generated code must be tested. |
| `xunit.v3` **4.0.2** | MTP **2.5.1** | Release notes verify pairing. Keep execution isolated from VSTest engine invocation. |
| `gdUnit4.api` **5.1.0-rc7** | adapter **3.2.0**, analyzers **1.1.0**, Godot **4.7/4.7.1/4.7.2**, net10 | Maintainer table and published packages align. API remains prerelease; repeatable project resource loading, input, cleanup and exports need proof. |
| Adapter **3.2.0** | VSTest; TestPlatform dependencies **>=18.0.1**; proposed Test.Sdk **18.0.1** | Confirm lock graph and actual test execution; no blanket downgrade-warning suppression. |
| UnityPy **1.25.4** | Selected Python interpreter and locked dependencies | Tool release verified; exact Python patch and donor decoding fidelity deliberately deferred to Phase 4 sample extraction. |

### Required proof gates and remaining uncertainty

- **Phase 1:** fetch/hash the exact engine/templates; prove SDK/compiler/analyzers, generated partials, export/resource construction and Windows/Linux clean exports. Record explicit differences from LangProj. The toolchain is selected for evaluation, not yet certified by this research.
- **Phase 1 then Phase 5:** prove runner isolation and engine-test candidate. The latest setup webpage still shows Godot 4.5/rc1 and a `NU1605` suppression; use current package metadata and positive/negative proof instead. [S25]
- **Phase 2/4:** choose the renderer using the game's actual layered sprites, lighting, particles and UI. The reference PC specification is not supplied; performance cannot yet select a final backend.
- **Phase 3/5:** prove real host plus three independent processes and a mixed-OS LAN procedure. Discoverability across firewalls/interfaces and actual LAN behavior cannot be inferred from loopback.
- **Phase 4:** establish extraction Python/tool pins and asset fidelity from source samples. UnityPy can decode data; it does not automatically recreate Unity animation controllers, shaders or gameplay in Godot.

## Sources

**Method:** GSD `research-plan` selected Context7 for API questions and websearch for ecosystem questions. Neither Context7 MCP nor the `ctx7` CLI was available, so official websites/package records were searched and cross-checked. `query classify-confidence --provider websearch --verified` returned **MEDIUM**; this document uses that tier. Digests were cached with the research-store seam. Local donor observations are explicitly separated from upstream compatibility claims. Sources accessed 2026-11-13; dates below are publication/release dates where available.

- **S1 — [Godot archive](https://godotengine.org/download/archive/)**: stable 4.7.2 (2026-08-18), 4.8 beta3 (2026-11-09).
- **S2 — [Godot desktop downloads](https://godotengine.org/download/)**: .NET editor builds and separate .NET export templates.
- **S3 — [Godot.NET.Sdk 4.7.2](https://www.nuget.org/packages/Godot.NET.Sdk/4.7.2)**: exact published SDK package (2026-08-18).
- **S4 — [.NET 10 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)**: SDK 10.0.404/runtime 10.0.15/C#14 (2026-11-10).
- **S5 — [Godot C# package targeting policy](https://godotengine.org/article/godotsharp-packages-net8/)**: binding target is a minimum; newer game target frameworks are permitted. Older policy source, cross-checked with current packages.
- **S6 — [.NET version selection](https://learn.microsoft.com/en-us/dotnet/core/versions/selection)**: SDK selection, global.json lookup and target/runtime distinction.
- **S7 — [Godot 4.7 exports](https://docs.godotengine.org/en/4.7/tutorials/export/exporting_projects.html)**: templates, presets, resource filters and playable export distinction.
- **S8 — [Godot 4.7 renderer comparison](https://docs.godotengine.org/en/4.7/tutorials/rendering/renderers.html)**: Compatibility/Forward Plus/Mobile tradeoffs.
- **S9 — [Godot 4.7 multiplayer](https://docs.godotengine.org/en/4.7/tutorials/networking/high_level_multiplayer.html)**: ENet, UDP and MultiplayerAPI.
- **S10 — [PacketPeerUDP](https://docs.godotengine.org/en/4.7/classes/class_packetpeerudp.html)**: binding and broadcast.
- **S11 — [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview)**: built-in serializer and source-generation option.
- **S12 — [xUnit v3 4.0.2 release notes](https://xunit.net/releases/v3/4.0.2)**: selected package and MTP 2.5.1 pairing.
- **S13 — [gdUnit4.api 5.1.0-rc7 package/release notes](https://www.nuget.org/packages/gdUnit4.api/5.1.0-rc7)**: net10/C#14, Godot 4.7+, multi-project support and headless limitations (2026-11-07).
- **S14 — [gdUnit4.test.adapter 3.2.0](https://feed.nuget.org/packages/gdUnit4.test.adapter/3.2.0)**: VSTest, package dependency minimums, GODOT_BIN, timeouts, headless parameters and failure handling.
- **S15 — [gdUnit4.analyzers 1.1.0](https://www.nuget.org/packages/gdUnit4.analyzers/1.1.0)**: published analyzer version (2026-10-25).
- **S16 — [gdUnit4Net compatibility table](https://github.com/godot-gdunit-labs/gdUnit4Net)**: version pairing; distinguish master table from independently published packages.
- **S17 — [Microsoft.NET.Test.Sdk 18.0.1](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/18.0.1)**: published VSTest host candidate.
- **S18 — [Godot 4.7 C# basics](https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_basics.html)**: separate .NET SDK requirement and external editor guidance; prerequisite paragraph still references Godot 4.5.
- **S19 — [GitHub runner selection](https://docs.github.com/en/actions/how-tos/write-workflows/choose-where-workflows-run/choose-the-runner-for-a-job)**: currently available Windows/Linux labels.
- **S20 — [Godot 4.7 CLI](https://docs.godotengine.org/en/4.7/tutorials/editor/command_line_tutorial.html)**: headless, import/export, rendered execution and unknown-argument behavior.
- **S21 — [UnityPy releases](https://pypi.org/project/UnityPy/)**: 1.25.4 release (2026-10-03).
- **S22 — [UnityPy maintained source/docs](https://github.com/K0lb3/UnityPy)**: extraction capabilities and need to pin versions/dependencies.
- **S23 — [.NET test runner modes](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test)**: MTP-only mode versus VSTest.
- **S24 — [xUnit MTP execution](https://xunit.net/docs/getting-started/v3/microsoft-testing-platform)**: executable tests and MTP support.
- **S25 — [gdUnit C# setup](https://godot-gdunit-labs.github.io/gdUnit4/latest/csharp_project_setup/csharp-setup/)**: identifies stale example settings that must not override current package metadata.

**Local evidence:** `.planning/PROJECT.md`; `.planning/config.json`; `specs/0.1/initial-spec.md`; `specs/0.1/assets-and-resources.md`; `src/godot-game/project.godot` and `.editorconfig`; LangProj root `global.json`; backend `Directory.Build.props`, `Directory.Build.targets`, `Directory.Packages.props`; `Sebreiro.CodeRules.Analyzers` and analyzer-test project files; architecture-test project references. Paths are research evidence, not proposed delivered-build dependencies.

---
*Stack research for: KingdomZombies full version 0.1*
*Researched: 2026-11-13*
