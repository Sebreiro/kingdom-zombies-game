# Phase 1: Code rules and documentation - Research

**Researched:** 2026-11-13 (Europe/Paris)
**Domain:** Godot/C# development rules, reproducible builds, executable architecture and lifecycle/state proofs
**Confidence:** MEDIUM overall; HIGH for inspected donor contents; integrated compatibility remains unproven.

<user_constraints>
## User Constraints (from CONTEXT.md)

The following decisions and discretion/deferred sections are copied verbatim. [CITED: 01-CONTEXT.md]

<!-- DATA_7E16F2AC_START -->
## Implementation Decisions

### Executable rules and focused documentation

- **D-01:** Make machine-checkable rules primarily executable lint/analyzer/configuration/architecture-check definitions. Documentation indexes and links to these definitions instead of maintaining duplicate prose descriptions as a competing source of truth. The locked requirements and approved rule dispositions still constrain what that code must enforce.
- **D-02:** Explain non-obvious rules beside their code or configuration where the format permits it, consistent with the adopted comment conventions. Separate documents explain architecture decisions, Godot adaptations, and review-only rules.
- **D-03:** Preserve the required inheritance and rule register: stable IDs, retained/adapted/inapplicable dispositions, reasons, exact code/configuration references, scopes/exemptions, and positive/negative proof mappings. Making the rules executable does not remove this traceability.
- **D-04:** Put genuinely qualitative obligations in a short, explicit review checklist linked to the relevant code and architecture decisions. Identify them as human-review obligations; do not present them as automated passes.
- **D-05:** Use focused reference indexes only. Setup commands and verification commands live in their respective documents; do not add a consolidated “Start here” walkthrough. Keep the root agent guide short.
- **D-06:** Consistent code for both people and agents is the purpose of the rules. Small functions are an explicit preference to enforce. Resolve exact thresholds through the LangProj inheritance register and required proofs; the discussion did not select a new numeric limit.

### Minimal examples and language boundary

- **D-07:** Reuse `src/godot-game/project.godot` as the launch target. Add only the minimal example code/files/resources needed to exercise the specified code/architecture rules, Godot lifecycle surfaces, and state contracts.
- **D-08:** Do not build gameplay, a game-themed demonstration, or a presentation feature in Phase 1. Minimal proof fixtures must not become a second game project or a substitute playable foundation. Launching the empty project alone does not replace the specified executable assertions.
- **D-09:** Game code, example game/engine code, and tests use C#. Development tools may use other languages. Do not interpret “C# only” as a prohibition on all non-C# tooling or ordinary configuration formats.

### Everyday verification workflow

- **D-10:** Use focused checks while editing, then run the complete local suite before finishing a code change. Partial checks are useful feedback and are not complete verification. Phase completion still requires the actual Windows and Linux evidence defined by the specification.
- **D-11:** For prose-only documentation edits, run documentation checks. Changes to rules, commands, or contracts require the complete local suite.
- **D-12:** Show concise progress and a final summary, with clear failure explanations and links to detailed logs.
- **D-13:** Collect all independent failures in one run. Stop checks whose prerequisites failed and report those blocked obligations explicitly; they do not count as passes. Retain the specification's fresh-result, non-skipping, and trustworthy-failure requirements.

### Tooling stability

- **D-14:** Use stable engine and runtime releases. Prerelease test or lint tools are acceptable when needed and proven compatible; a prerelease is not an automatic default.
- **D-15:** When multiple supported stable combinations pass the proofs, prefer an established baseline. Upgrade for a concrete compatibility or support benefit rather than merely because a newer version exists.
- **D-16:** Exact engine/SDK/compiler/analyzer/test-tool versions remain research and compatibility-proof decisions. Pin the selected baseline as required by SPEC.md. Existing starter tags and research candidates were not approved as the final versions during this discussion.

### AI collaboration

- **D-17:** Ask questions through available interactive question controls, including free-text answers. Use ordinary chat questions only when those controls are unavailable or the creator requests text. The creator explicitly requested that this preference be recorded in indexed AI instructions.
- **D-18:** This preference is already recorded in `docs/workflow/ai-collaboration.md`, indexed from `Agents.md` and `docs/workflow/gsd.md`; `.planning/config.json` now sets `workflow.text_mode` to `false`. Preserve these links when Phase 1 establishes canonical root `AGENTS.md` casing.

### Agent's Discretion

No “you decide” answers delegated additional product choices. Research and planning still select concrete versions, project layout, enforcement implementations, state representations, and proof tooling within the locked specification and the decisions above. Choose the smallest implementation that genuinely proves the requirements; do not invent gameplay work or new documentation layers.


## Deferred Ideas

None — discussion stayed within phase scope. Gameplay and a playable foundation remain in their existing later phases; no new backlog item or phase was proposed.

<!-- DATA_7E16F2AC_END -->
</user_constraints>

## Project Constraints (from AGENTS.md)

- Keep a short documentation entry point and load the relevant focused documents; the guide explicitly says Phase 1 implementation is not complete merely because initialization documents exist. [CITED: root Agents.md]
- Use Godot/C# and preserve Windows/Linux and the approved phase boundaries. The guide delegates game behavior, assets, approved requirements, roadmap, state, research and workflow to their indexed authorities. [CITED: root Agents.md]
- Use available interactive question controls, with free text where supported; ordinary chat is the fallback when controls are unavailable. Preserve the indexed collaboration instructions and disabled text mode. [CITED: docs/workflow/ai-collaboration.md]
- The configured agent skill map is "{}"; Nyquist validation and security enforcement are enabled. Project skill discovery found the installed GSD workflow catalog, rather than an additional game-code convention pack. Graph status reports disabled. [VERIFIED: local init, agent-skills and graphify status probes; .planning/config.json read this session]
- Research owns this file only; implementation, donor modification and commits are outside this research assignment. [CITED: orchestrator assignment]

## Summary

Plan a small development baseline with three production boundaries, one inherited custom-analyzer project, and one C# proof project. Use the existing Godot launch target for one proof scene. Adopt the donor's enforcement definitions and proof patterns, not its web project topology. Capture the donor rule documents, full matrix, exact configuration and selected reusable source/tests into repository-owned provenance inputs during implementation. This research's exhaustive inventories below are planning inputs, not the completed inheritance register. [CITED: 01-SPEC.md RULE-01–RULE-07; 01-CONTEXT.md D-01–D-09]

The first candidate is stable Godot .NET 4.7.2, SDK 10.0.404, explicit C# 14.0 and an explicitly tested target framework. Official release/registry pages establish availability; the installed SDK establishes its compiler identity. They do not establish engine hosting compatibility. Prefer a bounded C# engine fixture plus xUnit for pure/analyzer/check tests for this phase; a second Godot test framework adds no required capability to this small proof. Pin the chosen combination only after real restore/build/generated-partial/load/unload proofs. [CITED: https://godotengine.org/download/archive/4.7.2-stable/; https://dotnet.microsoft.com/en-us/download/dotnet/10.0; https://xunit.net/docs/getting-started/v3/microsoft-testing-platform]

Windows has the SDK and an existing matching Godot ZIP. Shell downloads currently fail through a local proxy; .NET's first-run default user write is denied. WSL did not provide a usable Linux runtime and no Docker/VM/SSH command was found in the checked PATH. Put workspace-local bootstrap and actual Linux provisioning/evidence ahead of closure. Neither unavailable tools nor successful Windows cross-builds waive Linux execution. [VERIFIED: environment probes recorded in Environment Availability] [CITED: 01-SPEC.md AC-02]

**Primary recommendation:** Execute a toolchain/engine compatibility slice first, then capture and adapt the complete rule inventory, wire real enforcement, and close all 22 acceptance criteria with positive/negative evidence on both operating systems.

## Architectural Responsibility Map

This is the recommended responsibility allocation within the locked outcomes, not a claim about existing implementation. [CITED: 01-SPEC.md RULE-05–RULE-07]

| Capability | Primary tier | Secondary tier | Rationale |
|---|---|---|---|
| Time, repeatable random input, stable identity and authoritative example state | Pure C# core | Proof tests | Independent of Godot and concrete services |
| Discovery/identity/connection contracts | Pure C# multiplayer port assembly | Godot composition | Ports are replaceable; do not implement LAN |
| Engine construction, resources, exports, signals, callbacks, main-thread gateway and teardown | Godot assembly | Pure core | The engine owns lifecycle |
| Code rules and diagnostics | Roslyn analyzer/build | Rule register | Compiler executes machine-checkable rules |
| Evaluated dependency graph, type purity, suppression/register/docs checks | C# proof/check project | MSBuild and built assemblies | Inspect actual evaluated inputs; reject empty subjects |
| Fresh-case reconciliation, timeout and result publication | Small local development entry point | C# tests + Godot process | Process completion alone is insufficient |
| Persistence proof | Pure snapshot codec + atomic in-memory replacement | Tests | No production save system or file promotion needed |

<phase_requirements>
## Phase Requirements

Descriptions below are verbatim requirement statements. [CITED: .planning/REQUIREMENTS.md:22-28]

<!-- DATA_B4D079C2_START -->

| ID | Description | Research support |
|---|---|---|
| RULE-01 | Developers can build with a pinned Godot/.NET/compiler/analyzer combination proven by a representative engine integration check. | Candidate/version matrix, isolated toolchain bootstrap, Windows/Linux gate |
| RULE-02 | Developers inherit applicable LangProj C# conventions through a retained/adapted/inapplicable rule register with reasons for every Godot adaptation. | 85-ID inventory, 84 heading occurrences, hashes, exemption inventory |
| RULE-03 | Developers receive build or check failures for rule violations, with positive and deliberately failing proofs and only documented, narrowly scoped exemptions. | Analyzer packaging, real-build mutations, exact exemptions, numeric edges |
| RULE-04 | Developers can navigate a short root AGENTS.md and focused documentation indexes with valid links and consistent Windows/Linux filename casing. | Git-index casing and explicit documentation scope; portable validator cases |
| RULE-05 | Developers have documented, enforced dependency boundaries separating gameplay rules, Godot integration, and multiplayer services. | Five-project topology, evaluated references, generated IL/type checks |
| RULE-06 | Developers can use normal Godot construction, callbacks, partial/generated code, exports, signals, nullability, and teardown under the rules without blanket suppression. | Engine-created fixture, required export validation, two lifecycle cycles and late-work guard |
| RULE-07 | Developers have explicit rules for main-thread engine access, testable time/randomness, stable identities, and saveable authoritative state. | Integer elapsed units, replayable random input, strict detached snapshot validation/replacement |

<!-- DATA_B4D079C2_END -->
</phase_requirements>

## Standard Stack

### Core and supporting candidates

Versions below are **first compatibility candidates**, not certified integrations. A newer registry version alone is not a reason to upgrade an established donor-compatible baseline. The source of a local version is quoted where used; registry publication and source-publisher evidence are separate. [CITED: 01-CONTEXT.md D-14–D-16]

| Component | Candidate | Evidence, purpose and pinning |
|---|---|---|
| Godot .NET engine + bindings/SDK | 4.7.2 stable, published 2026-08-18 | Official archive and reserved Godot NuGet owner agree. Record OS archive hashes and full engine version; use matching SDK/bindings. [CITED: https://godotengine.org/download/archive/4.7.2-stable/; https://www.nuget.org/packages/Godot.NET.Sdk/4.7.2] |
| .NET SDK / runtime / compiler | SDK 10.0.404; installed runtime 10.0.15; csc 5.9.0-1.26529.119 | Download page dates SDK release 2026-11-10; local dotnet --info and csc -version match. Pin SDK with prereleases disabled and exact resolution; record compiler output. [VERIFIED: local dotnet --info and csc -version] [CITED: https://dotnet.microsoft.com/en-us/download/dotnet/10.0] |
| Target framework / language | First probe net10.0 / explicit 14.0 | These are recommendations requiring engine hosting proof, not inferred support from NuGet computed framework lists. Keep net8.0 as a fallback compatibility probe only if net10.0 actually fails. [ASSUMED] |
| Editor/tool runtime | .NET 8 runtime alongside SDK 10 | Inspected Godot ZIP: GodotTools runtimeconfig declares "tfm": "net8.0", "rollForward": "LatestMinor"; GodotPlugins declares "rollForward": "LatestMajor". Do not assume SDK 10 alone runs editor tooling. Windows has 8.0.16. [VERIFIED: inspected ZIP runtimeconfig entries; local dotnet --info] |
| Custom analyzer | In-repo adopted donor source, target "netstandard2.0" | Compile and package as analyzer-only, with source provenance and own positive/negative tests. [VERIFIED: donor packages/Sebreiro.CodeRules.Analyzers/Sebreiro.CodeRules.Analyzers.csproj:3-17, "<TargetFramework>netstandard2.0</TargetFramework>", "<IncludeBuildOutput>false</IncludeBuildOutput>"] |
| Roslyn packages | Microsoft.CodeAnalysis.CSharp 5.0.0; Microsoft.CodeAnalysis.Analyzers 5.9.0 | Exact donor pins are "5.0.0"/"5.9.0"; registry dates 2025-11-18/2026-08-17. Do not confuse analyzer-development package version with csc compiler version. [VERIFIED: donor Directory.Packages.props:24-25] [CITED: https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp/5.0.0; https://www.nuget.org/packages/Microsoft.CodeAnalysis.Analyzers/5.9.0] |
| Style analyzers | Meziantou.Analyzer 3.0.312; Roslynator.Analyzers 5.0.1; Microsoft.CodeAnalysis.BannedApiAnalyzers 5.6.0 | Donor pins "3.0.312", "5.0.1", "5.6.0". Official publisher repositories identify the tools; NuGet reports MA 3.0.312 on 2026-10-18, Roslynator 2026-10-04, BannedApi 2026-07-02. MA exact package fetch failed; no direct restore observed. [VERIFIED: donor Directory.Packages.props:43-45] [CITED: https://github.com/meziantou/Meziantou.Analyzer; https://www.nuget.org/packages/Meziantou.Analyzer/3.0.15; https://github.com/dotnet/roslynator; https://www.nuget.org/packages/Roslynator.Analyzers/5.0.1; https://www.nuget.org/packages/Microsoft.CodeAnalysis.BannedApiAnalyzers/5.6.0] |
| Pure/check/analyzer tests | xunit.v3 4.0.2; Shouldly 4.3.0 | Donor pins "4.0.2"/"4.3.0". xUnit release dated 2026-10-09; registry now also lists 4.0.3. Keep 4.0.2 first for inherited proofs; run its executable directly with an explicitly selected MTP mode. Shouldly registry confirms 4.3.0; publication date was not resolved. [VERIFIED: donor Directory.Packages.props:32-33] [CITED: https://xunit.net/releases/v3/4.0.2; https://www.nuget.org/packages/xunit.v3; https://www.nuget.org/packages/shouldly] |
| Analyzer test helpers | Microsoft.CodeAnalysis.CSharp.Analyzer.Testing 1.1.4; Microsoft.CodeAnalysis.CSharp.Workspaces 5.0.0 | Donor pins "1.1.4"/"5.0.0"; publication 2026-05-22/2025-11-18; preserve helper's reference-assembly/compiler-error checks. [VERIFIED: donor Directory.Packages.props:37-38] [CITED: https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Analyzer.Testing/1.1.4; https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Workspaces/5.0.0] |
| Architecture/type inspection | TngTech.ArchUnitNET 0.13.4 plus Mono.Cecil 0.11.6 where directly used | ArchUnit donor pin "0.13.4"; NuGet dates 2026-08-20 and also lists newer 0.13.5. Declare Cecil directly instead of relying on accidental transitive availability. Inspect locking/assembly identity with coverage tooling. [VERIFIED: donor Directory.Packages.props:36, 'Version="0.13.4"'] [CITED: https://archunitnet.readthedocs.io/en/latest/; https://www.nuget.org/packages/TngTech.ArchUnitNET/0.13.4; https://www.nuget.org/packages/Mono.Cecil] |
| Coverage | Local dotnet-coverage 18.12.0 | Official tool supports cross-platform process/subprocess collection, managed-module instrumentation and merging; registry confirms exact candidate. Probe native Godot hosting collection before declaring coverage proven. Keep coverage tool in a local tool manifest; not a production dependency. [CITED: https://learn.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-coverage; https://www.nuget.org/packages/dotnet-coverage] |
| Serialization | System.Text.Json from selected runtime | No additional JSON package required for the proposed baseline. Runtime null annotations alone do not validate root/collection element nulls or missing values; use explicit validation. [CITED: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/nullable-annotations] |

**Renderer:** Keep this proof headless; record the renderer actually selected. Compatibility is a reasonable minimal 2D fixture candidate, but this phase does not prove rendered gameplay. Do not silently preserve starter feature tags as a toolchain constraint. [CITED: 01-SPEC.md Constraints; 01-CONTEXT.md D-07–D-08]

**Installation contract:** Pin NuGet versions centrally, commit lock files, restore in locked mode, pin SDK and local tool versions, and record engine checksums. Implement bootstrap using explicit workspace paths for CLI home/cache. Restore sources must be declared; never require the donor folder or the researcher's Downloads folder in a clean checkout. [CITED: 01-SPEC.md AC-01–AC-02]

### Alternatives considered

| Recommended | Alternative | Decision |
|---|---|---|
| Bounded actual Godot fixture + pure xUnit checks | gdUnit adapter/test framework | No adapter needed for these acceptance surfaces; keep the fixture small. Reassess when later scene/input testing needs justify a framework. [CITED: 01-CONTEXT.md D-07–D-09; 01-SPEC.md scope] |
| Three production boundary projects | Donor fifteen-project web topology | Web/database topology is explicitly excluded; a service-port assembly supplies a demonstrable dependency boundary with little code. [CITED: 01-SPEC.md RULE-05 and Boundaries] |
| Exact installed SDK 10.0.404 first | Older supported stable SDK | Preserve the newer security-patched SDK if the candidate passes; downgrade only with a documented compatibility reason and renewed proofs. [CITED: https://dotnet.microsoft.com/en-us/download/dotnet/10.0; 01-CONTEXT.md D-15] |

## Package Legitimacy Audit

The supplied seam was executed with NuGet and returned: "Usage: gsd-tools package-legitimacy check --ecosystem <npm|pypi|crates> <pkg1> ...". It cannot issue a NuGet verdict. Do not fabricate OK/SUS/SLOP, npm verification, ages or download rates. Use official project links, reserved owners where present, registry metadata, exact nupkg hashes/signature checks and a successful isolated locked restore as the equivalent NuGet audit. Network failures here mean **no restore observation**, not that packages are absent or incompatible. [VERIFIED: package-legitimacy probe]

| Package group | Registry | Publisher/source evidence | Verdict | Disposition |
|---|---|---|---|---|
| Godot.NET.Sdk and implicit GodotSharp/GodotSharpEditor/Godot.SourceGenerators | NuGet | Official Godot archive, reserved owner, included local nupkg manifests | Seam unsupported | Candidate; verify downloaded archive and package hashes |
| Microsoft.CodeAnalysis.*; dotnet-coverage | NuGet | Microsoft project documentation and registry/source links above | Seam unsupported | Candidate; lock restore graph and inspect build assets |
| xunit.v3; Shouldly | NuGet | Official xUnit release and Shouldly repository | Seam unsupported | Candidate; local runner/diagnostic proofs required |
| Meziantou.Analyzer; Roslynator.Analyzers | NuGet | Official publisher repositories above | Seam unsupported | Candidate; MA exact-version direct fetch still unresolved |
| TngTech.ArchUnitNET; Mono.Cecil | NuGet | Project docs/source links from registries | Seam unsupported | Candidate; declare directly when source consumes them |

Each row inherits the cited publisher/registry evidence in Standard Stack. Ages and package-level download rates were not audited; no package is classified suspicious merely because this seam lacks NuGet support. Inspect nuspec/build/targets/analyzer payloads rather than npm postinstall metadata. No external package install was performed by this research. [CITED: Standard Stack sources; VERIFIED: research tool-call record]

## Architecture Patterns

### System architecture diagram

Recommended data flow; every named component below is proposed implementation within the locked scope. [CITED: 01-SPEC.md AC-03–AC-22]

~~~mermaid
flowchart TD
    A[Developer invokes local check] --> B[Resolve pins and isolated run directory]
    B --> C{Required tools and inputs present?}
    C -->|No| X[Record failed and blocked obligations]
    C -->|Yes| D[Restore and build with analyzers]
    D --> E[Pure rule, graph, docs and state proofs]
    D --> F[Real Godot process loads proof scene]
    F --> G[Engine constructs fixture and loads resource]
    G --> H[Validate exports then callbacks and signals]
    H --> I[Unload, reject late work, repeat cycle]
    E --> J[Reconcile every required case and evidence]
    I --> J
    X --> J
    J --> K{Fresh, unique, complete and passed?}
    K -->|Yes| L[Atomically publish complete result]
    K -->|No| M[Nonzero exit and diagnostic logs]
~~~

### Recommended exact small topology

The following paths and names are **proposals**, not existing files. The existing launch-target path is quoted from CONTEXT.md: "src/godot-game/project.godot". Do not create a second engine project. [CITED: 01-CONTEXT.md D-07; 01-SPEC.md RULE-05]

~~~text
KingdomZombies.slnx
global.json
Directory.Build.props / Directory.Build.targets / Directory.Packages.props
.editorconfig / BannedSymbols.txt / BannedSymbols.Tests.txt
src/core/KingdomZombies.Core.csproj
src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj
src/godot-game/KingdomZombies.csproj + existing project.godot
tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj
tests/phase1/KingdomZombies.Phase1.Tests.csproj
tools/phase1/                  small cross-platform entry point and required-case input
docs/code/                    focused indexes/register/exemptions/review checklist
docs/architecture/            boundaries and decisions
docs/workflow/                separate setup and proof commands
(each csproj directory)/docs/README.md
~~~

| Project | Allowed direct production references | Boundary proof |
|---|---|---|
| Core | None; System/framework only | Reject Godot and service/provider types, including bodies and generated state machines |
| Multiplayer.Abstractions | None; independent service interfaces/value contracts | Reject Core/Godot/concrete provider dependencies |
| Godot game | Core + Multiplayer.Abstractions | Compose boundaries explicitly; engine-facing code remains analyzed |
| Custom analyzer | No product assembly | Analyzer-only edge to other projects; own source checked through a separate throwaway consumer |
| Phase1.Tests | All projects needed to inspect/test; game assembly inspected without constructing Nodes in plain .NET | Explicit test/tool edge exceptions, never production exemptions |

The analyzer edge is build tooling, not a runtime product dependency. Donor targets demonstrate the exact metadata "OutputItemType=\"Analyzer\" ReferenceOutputAssembly=\"false\""; preserve this contract and test a deliberately changed metadata value. [VERIFIED: donor Directory.Build.targets:2-5]

Evaluate each csproj with MSBuild's actual ProjectReference/PackageReference/property output. Compare declared project set, evaluated graph, reference-defining source, recursive closure, and loaded subject set. Check types in bases/interfaces/generics/attributes/signatures/fields and method bodies, including lambda/async/iterator-generated code. A list of ProjectReference XML nodes alone misses imported edges and type leakage. The donor already supplies an evaluator plus additional generated-IL scan. [VERIFIED: donor ProjectEvaluator.cs:11-17 and 104-133; donor GeneratedCodeScanner.cs:10-17 and 63-102]

### Rule authority and inheritance model

Use an immutable captured source inventory plus a separate local disposition/mapping register. Use executable definitions as authority for operational checking, with focused documentation linking to them. Enforce bidirectional mapping: source heading identity → source IDs → one disposition per ID → local definition/docs → mechanism/scope → positive/negative case IDs. Repeated source headings and a heading shared by multiple IDs are intentional relationships, not duplicate source IDs. Compare sets with ordinal identity and reject duplicates before set conversion. [CITED: 01-CONTEXT.md D-01–D-04; 01-SPEC.md AC-05–AC-06]

Do not copy donor Markdown as a competing local rule specification. Capture it as provenance, with original path, commit, file hash and source heading/line metadata. Keep imported material outside normal authored-rule/doc navigation validation, but validate its capture identity. The phase implementation must store the full source matrix/configs and adopted source/tests; this research's compact appendix cannot replace that capture. [CITED: 01-SPEC.md RULE-02 and Constraints]

### Godot compatibility boundaries

| Surface | Prescribed minimal policy and proof | Evidence |
|---|---|---|
| Construction | Keep validated constructors for pure state. Engine scripts permit engine-compatible parameterless construction and initialize scene-dependent references after load. Prove engine instantiation; never call callbacks directly as evidence. | [CITED: 01-SPEC.md AC-14–AC-16] |
| Partial/generated code | Compile actual generated partials. Emit/inspect compiler-generated source in an isolated output directory when establishing exemption boundaries. Exempt genuine selected generator surfaces only; a source file renamed to a generated suffix or carrying an auto-generated marker is rejected. | [CITED: 01-SPEC.md AC-08; https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_basics.html] |
| Exports/nullability | A nullable backing reference explicitly represents not-yet-injected state. Validate every required export at readiness before use; keep optional nullable values optional. Use private exports where possible and explicit guards; no null-forgiving workaround. | Godot allows exported fields/properties with any access modifier. [CITED: https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_exports.html] |
| Signals | Use a genuine engine signal and generated binding, explicit subscription/unsubscription, and per-cycle counts. A custom signal delegate is public and ends in EventHandler. Avoid relying on automatic cleanup for captured lambdas/custom signals. | [CITED: https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_signals.html] |
| Callbacks | Assert parent enter before child enter, child ready before parent ready, and child exit before parent exit across at least two new instances. Observe process callback without using frame timing as authoritative example time. | [CITED: https://docs.godotengine.org/en/4.7/classes/class_node.html] |
| Main thread | Capture main-thread identity in engine startup; a narrow gateway checks thread and live-generation before engine access. Queue pure results to a main-thread-owned drain. Worker-negative fixture must fail in the guard before an unsafe engine call. | Active scene-tree access is not thread safe; deferred calls are documented. [CITED: https://docs.godotengine.org/en/4.7/tutorials/performance/thread_safe_apis.html] |
| Teardown | On exit: mark lifetime closed, cancel work, disconnect subscriptions and invalidate queued-generation tokens. Deliberately release a pre-teardown worker result after unload and prove no mutation of old or replacement state. Cancellation alone does not prove the late-result boundary. | [CITED: 01-SPEC.md AC-15–AC-17] |
| Async rules | Keep context-free library ConfigureAwait policy separate from engine continuation policy. Do not use ConfigureAwait(false) as a global engine default or assume await returns to Godot's thread. Use the explicit gateway and actual thread assertions. | [CITED: donor code-style.md:528-533; Godot thread-safe APIs above] |
| Visibility/naming | Engine callback overrides and signal delegates may have engine-required visibility/names. First try unmodified analyzers; register only observed incompatibilities, with exact diagnostic/symbol surface, accepted engine build and immediately-outside violating handwritten case. | [CITED: 01-SPEC.md AC-07–AC-08] |

A guard rejects prohibited access at runtime; it does not by itself forbid all source bypasses. Add semantic/type-level checks for engine access outside the allowed adapter/gateway and forbidden global time/random APIs in Core. A negative fixture that merely crashes Godot is not a passing thread proof. [CITED: 01-SPEC.md AC-03, AC-17–AC-18]

### State examples with explicit contracts

Use a tiny non-game model: an identity-bearing ordered record sequence plus an unordered keyed record collection, elapsed integer units, and a replayable random input position. No inventory, movement, networking or save UX. Proposed concrete time contract: nonnegative signed 64-bit elapsed ticks with one declared integer unit; accepted delta zero leaves state untouched; checked addition rejects overflow before replacement. Avoid introducing floating point merely to manufacture non-finite cases; test those only at any actual floating-point boundary. [CITED: 01-SPEC.md AC-18–AC-22 and phase exclusions]

Use injected random input backed by an established generator. A minimal System.Random wrapper may persist a seed plus a bounded draw position and replay exactly the same operation to reconstruct the next draw; explicitly version that contract and verify common golden vectors on both pinned platforms. Do not claim runtime-independent algorithm stability. This representation is a proposed compatibility option needing its own continuation proof. [ASSUMED]

Use explicit ordinal identity equality and a declared lexical contract. Preserve distinct valid case/non-ASCII forms if allowed; reject malformed/empty IDs before insertion. Snapshot carries schema version, values, identities, ordering and random position. Deserialize into detached candidate objects, validate the entire candidate, then replace one immutable state root in one commit. Repeated restore does not call creation/effect/time/random APIs. Readers retain one root snapshot; interruption before the commit leaves the old root. A lock or atomic reference swap is sufficient here; do not build the full save subsystem. [CITED: 01-SPEC.md AC-20–AC-22]

## Don't Hand-Roll

| Problem | Do not build | Use instead | Why |
|---|---|---|---|
| Exact C# rule semantics | Regex C# linter | Adapt donor Roslyn analyzers and tests | Syntax/semantic cases, generated surfaces and named-argument exceptions are already encoded. [CITED: donor enforcement decision; inspected analyzer sources] |
| Effective dependency graph | csproj text-only dependency scanner | MSBuild evaluated items/properties and adopted type/IL checks | Imported/wildcard/transitive edges must count. [VERIFIED: donor ProjectEvaluator.cs:11-17] |
| C# parsing/diagnostic test harness | New compiler frontend | Roslyn analyzer testing helper | Fails unexpected compiler errors rather than crediting them as intended diagnostics. [VERIFIED: donor AnalyzerVerifier.cs:11-15 and 44-66] |
| Pseudorandom algorithm | A new PRNG for the example | Injected established generator/replay adapter | Algorithm design is outside scope; save/continue its declared input state. [CITED: 01-SPEC.md RULE-07] |
| JSON parser | String splitting or loose dictionaries | System.Text.Json + complete candidate validation | Null/missing/identity semantics still require explicit validation. [CITED: Microsoft nullable JSON documentation above] |
| Scene test framework | General-purpose game automation framework | One bounded C# fixture scene | Phase 5 owns the comprehensive framework. [CITED: 01-CONTEXT.md D-07–D-09] |
| Build/coverage framework | CI workflow copied from donor | Small local entry point + pinned coverage tool | Preserve numeric obligations locally while CI stays in Phase 5. [CITED: 01-SPEC.md Constraints] |

## Donor Baseline and Exhaustive Inventory

The donor was read at clean Git HEAD "6836a9718ff5c939e89abf2b97763760607de192". This captures 49 style heading occurrences and 35 architecture heading occurrences, mapping to 85 unique matrix IDs. Its exemptions table contains 24 rows; numbering includes EX-21 then EX-23, so do not synthesize EX-22 or assume IDs are contiguous. [VERIFIED: git revision/status and full source reads of donor code-style.md, code-architecture.md, rule-matrix.md]

| Source relative to donor backend docs | SHA-256 of inspected bytes |
|---|---|
| CodeRules/code-style.md | 87324D433E5295CD63D40657B7405BDA7CEB50B800F357FE065B4DBE2DEBB6F8 |
| CodeRules/code-architecture.md | 242885B8A03995EFFED7F26F9016CA5F4A0909AA390E92E5664111F903E76900 |
| rule-matrix.md | 3F01311132E2547CA65400BC0593A84643E25D0178234581C5B7BDCCDD54AA51 |
| decisions/0003-enforcement-approach.md | CE4A7C7DDF8D7406A8A44A5E05C9FB50C58E57DE7CC0B9D41DDFE4BDAE238E10 |

Hashes above are computed observations, not source declarations. [VERIFIED: Get-FileHash SHA256 probes]

### Exact numeric semantics to preserve

<!-- DATA_8C419E03_START -->

- R-07/R-08: "Each indentation level is 4 spaces." and "The tab width is set to 4". [VERIFIED: donor code-style.md:82-92]
- R-09: "A wrapped line that continues the previous line is indented 8 spaces past the line it continues." Ten continuation forms are explicitly enumerated; block layouts use four-space contents and multiline-string contents are not checked. [VERIFIED: donor code-style.md:94-220]
- R-10: "A C# line holds at most 120 characters. The count is in UTF-16 characters and excludes the line terminator". [VERIFIED: donor code-style.md:222-227]
- R-37: "A member body of more than 60 lines fails the build; 40 lines is the advised length." "Every line between the body's braces counts, blank lines included." Local functions count both independently and within the parent; expression/global-body counting differs. [VERIFIED: donor code-style.md:469-479; BodyLengthAnalyzer.cs:10-15 and 67-79]
- R-49: "at least 90% of its lines and 80% of its branches"; R-82: "at least 90% of the lines and 80% of the branches". [VERIFIED: donor code-style.md:558-573]
- R-74: "Every enum declares Unknown = 0." Treat the literal "Unknown = 0" as the sentinel policy when enums are selected, not a valid saved state. [VERIFIED: donor code-architecture.md:350-356]

<!-- DATA_8C419E03_END -->

For every retained numeric rule, plan below/equal/above tests: 59/60/61 body lines; 119/120/121 UTF-16 code units (ASCII, Cyrillic, surrogate-pair and combining-character cases); 3/4/5 indentation spaces and 7/8/9 continuation offsets in each recognized form. Include zero/empty bodies. Coverage gates compare exact counts by integer cross-multiplication, not rounded percentages; test 89/90/91 covered of 100 lines and 79/80/81 of 100 branches, empty/missing report and zero-denominator cases. Forty advised lines are not an extra failing limit. These are recommended proof cases derived from the quoted semantics. [CITED: 01-SPEC.md AC-09]

### Complete matrix-ID planning inventory

The title/diagnostic tokens in quotes are verbatim source values, read this session. "donor rule-matrix.md" means the hashed source above, not a runtime dependency. Proposed dispositions and implementation notes are research recommendations for the local register; the implementation must also copy full donor scopes/exemptions/proof names and map new concrete proof IDs. [CITED: 01-SPEC.md AC-05–AC-08]

<!-- DATA_D5F8362B_START -->

| Source ID | Exact source title(s) | Proposed disposition | Diagnostic tokens / kind | Local treatment and source |
|---|---|---|---|---|
| "R-01" | "Warnings are errors" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:33-33] |
| "R-02" | "UTF-8 encoding" | adapted | configuration / structural / review | Scope project text; installed GSD/vendor files are not game source. [VERIFIED: donor rule-matrix.md:34-34] |
| "R-03" | "LF line endings" | adapted | configuration / structural / review | Project text scope; preserve vendor boundary. [VERIFIED: donor rule-matrix.md:35-35] |
| "R-04" | "Final newline" | adapted | configuration / structural / review | NuGet lock-file exception remains exact. [VERIFIED: donor rule-matrix.md:36-36] |
| "R-05" | "No trailing whitespace" | adapted | "IDE0055" | Project text scope; strict UTF-8 decode precedes formatting. [VERIFIED: donor rule-matrix.md:37-37] |
| "R-06" | "Spaces only, no tab characters" | adapted | "IDE0055" | Prove Godot text scene/resource serialization before applying no-tabs globally. [VERIFIED: donor rule-matrix.md:38-38] |
| "R-07" | "Indentation of 4 spaces" | retained | "IDE0055" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:39-39] |
| "R-08" | "Tab width of 4" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:40-40] |
| "R-09" | "Continuation indent of 8 spaces" | retained | "SCR0009" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:41-41] |
| "R-10" | "Line length of 120 characters" | retained | "SCR0007" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:42-42] |
| "R-11" | "File-scoped namespaces" | retained | "IDE0161" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:43-43] |
| "R-12" | "Braces always" | retained | "IDE0011" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:44-44] |
| "R-13" | "Usings outside the namespace" | retained | "IDE0065" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:45-45] |
| "R-14" | "Using order" | retained | "SCR0010" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:46-46] |
| "R-15" | "No unused usings" | retained | "IDE0005" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:47-47] |
| "R-16" | "Implicit usings disabled" | retained | "CS0246" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:48-48] |
| "R-17" | "Var only when the type is apparent" | retained | "IDE0007", "IDE0008" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:49-49] |
| "R-18" | "Locals created with new use var" | retained | "IDE0007", "SCR0012" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:50-50] |
| "R-19" | "Target-typed new only where var cannot be used" | retained | "IDE0090" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:51-51] |
| "R-20" | "Collection expressions preferred" | retained | "IDE0028", "IDE0300", "IDE0301", "IDE0302", "IDE0303", "IDE0304", "IDE0305" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:52-52] |
| "R-21" | "Null checks use is null" | retained | "IDE0041", "RCS1248" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:53-53] |
| "R-22" | "Expression bodies only for properties and accessors" | retained | "IDE0021", "IDE0022", "IDE0023", "IDE0024", "IDE0061" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:54-54] |
| "R-23" | "No primary constructors" | retained | "SCR0003" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:55-55] |
| "R-24" | "Records only in non-positional form" | adapted | "SCR0003" | Keep non-positional records; entity/DTO restrictions have explicit subject sets. [VERIFIED: donor rule-matrix.md:56-56] |
| "R-25" | "No region directives" | retained | "SCR0004" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:57-57] |
| "R-26" | "No null-forgiving operator" | retained | "SCR0005" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:58-58] |
| "R-27" | "Private fields use _camelCase" | retained | "IDE1006" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:59-59] |
| "R-28" | "No this qualification" | retained | "IDE0003", "IDE0009", "SCR0011" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:60-60] |
| "R-29" | "Constants use PascalCase" | retained | "IDE1006" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:61-61] |
| "R-30" | "No Async suffix required" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:62-62] |
| "R-31" | "Classes sealed by default" | retained | "CA1852", "MA0053" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:63-63] |
| "R-32" | "One type per file" | retained | "MA0048" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:64-64] |
| "R-33" | "Member order" | retained | "SCR0008" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:65-65] |
| "R-34" | "Default visibility internal" | adapted | configuration / structural / review | Engine-facing script/signal/export visibility differs from web registration visibility. [VERIFIED: donor rule-matrix.md:66-66] |
| "R-35" | "XML docs on public types and members"; "XML docs and comments" | retained | "CS1591" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:67-67] |
| "R-36" | "No comments inside member bodies"; "XML docs and comments" | retained | "SCR0002" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:68-68] |
| "R-37" | "Member body of 60 lines or fewer"; "Member body length" | retained | "MA0051", "SCR0006" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:69-69] |
| "R-38" | "Named arguments"; "Named arguments at call sites" | retained | "SCR0001" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:70-70] |
| "R-39" | "Banned APIs" | retained | "RS0030" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:71-71] |
| "R-40" | "Banned APIs" | adapted | "RS0030" | Use an engine log adapter; narrowly account for proof/report output. [VERIFIED: donor rule-matrix.md:72-72] |
| "R-41" | "Banned APIs" | retained | "RS0030" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:73-73] |
| "R-42" | "Banned APIs" | retained | "RS0030" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:74-74] |
| "R-43" | "Banned APIs" | inapplicable | "RS0030" | HTTP/database-only mechanism; no corresponding Phase 1 capability. [VERIFIED: donor rule-matrix.md:75-75] |
| "R-44" | "Logging through LoggerMessage" | adapted | "CA1848", "CA1873" | Engine logging boundary; keep generated ILogger rules where ILogger is used. [VERIFIED: donor rule-matrix.md:76-76] |
| "R-45" | "ConfigureAwait in Sebreiro packages" | adapted | "MA0004" | Pure library continuations differ from main-thread engine continuations. [VERIFIED: donor rule-matrix.md:77-77] |
| "R-46" | "Tests follow every rule" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:78-78] |
| "R-47" | "Shouldly assertions" | retained | "RS0030" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:79-79] |
| "R-48" | "Test method names use Method_State_Expected" | retained | "CA1707" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:80-80] |
| "R-49" | "Analyzer coverage of 90% lines and 80% branches" | adapted | configuration / structural / review | Keep 90%/80% local analyzer coverage; CI integration belongs to Phase 5. [VERIFIED: donor rule-matrix.md:81-81] |
| "R-50" | "Generated code exempt only under Migrations" | adapted | configuration / structural / review | Replace migration exemption with genuine SDK-generated outputs only. [VERIFIED: donor rule-matrix.md:82-82] |
| "R-51" | "Exactly the listed projects" | adapted | configuration / structural / review | Exact local five-project graph replaces fifteen web projects. [VERIFIED: donor rule-matrix.md:83-83] |
| "R-52" | "CompositionRoot is the only host" | adapted | configuration / structural / review | Godot is engine host; C# game assembly may be a library; proof executable is declared. [VERIFIED: donor rule-matrix.md:84-84] |
| "R-53" | "Transitive project references disabled" | retained | "CS0012", "CS0246" | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:85-85] |
| "R-54" | "Allowed project references" | adapted | configuration / structural / review | Use the minimal game graph plus analyzer-only tooling edge. [VERIFIED: donor rule-matrix.md:86-86] |
| "R-55" | "Interface-only abstractions" | adapted | configuration / structural / review | Independent multiplayer port assembly; no per-web-layer sibling proliferation. [VERIFIED: donor rule-matrix.md:87-87] |
| "R-56" | "No entities in outward contracts" | adapted | configuration / structural / review | Service contracts stay engine/domain-free; no HTTP contract assemblies. [VERIFIED: donor rule-matrix.md:88-88] |
| "R-57" | "Per-layer registration" | inapplicable | configuration / structural / review | HTTP/database-only mechanism; no corresponding Phase 1 capability. [VERIFIED: donor rule-matrix.md:89-89] |
| "R-58" | "Domain depends only on System" | adapted | configuration / structural / review | Core depends only on System and its own types. [VERIFIED: donor rule-matrix.md:90-90] |
| "R-59" | "Application has no infrastructure types" | adapted | configuration / structural / review | Gameplay avoids engine and concrete service infrastructure. [VERIFIED: donor rule-matrix.md:91-91] |
| "R-60" | "No IServiceProvider outside CompositionRoot" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:92-92] |
| "R-61" | "Integration projects are isolated" | adapted | configuration / structural / review | Future providers must use only service ports; fixture proves isolation now. [VERIFIED: donor rule-matrix.md:93-93] |
| "R-62" | "Reusable packages are product-agnostic" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:94-94] |
| "R-63" | "Domain entities" | adapted | configuration / structural / review | Private mutation/validated construction retained; ORM constructor is inapplicable. [VERIFIED: donor rule-matrix.md:95-95] |
| "R-64" | "DTOs" | adapted | configuration / structural / review | Snapshot DTOs use explicit constructors; HTTP-specific suffix rules inapplicable. [VERIFIED: donor rule-matrix.md:96-96] |
| "R-65" | "Hand-written mapping" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:97-97] |
| "R-66" | "Single version value" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:98-98] |
| "R-67" | "No suppressions outside the rule matrix" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:99-99] |
| "R-68" | "Property names in camelCase and enums as their pinned names" | adapted | configuration / structural / review | One snapshot serialization contract; no HTTP options. [VERIFIED: donor rule-matrix.md:100-100] |
| "R-69" | "Exact-match enum input" | adapted | configuration / structural / review | Exact pinned enum parsing when enums are present in example. [VERIFIED: donor rule-matrix.md:101-101] |
| "R-70" | "Nulls omitted and unknown properties ignored" | adapted | configuration / structural / review | Declare snapshot unknown/null policy; document any divergence from permissive HTTP defaults. [VERIFIED: donor rule-matrix.md:102-102] |
| "R-71" | "Missing and null required values rejected" | adapted | configuration / structural / review | Validate required snapshot values/collections before commit. [VERIFIED: donor rule-matrix.md:103-103] |
| "R-72" | "Shared JSON settings configured once" | adapted | configuration / structural / review | One snapshot codec replaces host HTTP options. [VERIFIED: donor rule-matrix.md:104-104] |
| "R-73" | "Times and durations" | adapted | "RS0030" | Controlled elapsed integer units replace persisted wall-clock/database times. [VERIFIED: donor rule-matrix.md:105-105] |
| "R-74" | "Unknown is the zero value and never valid" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:106-106] |
| "R-75" | "Every enum member is pinned" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:107-107] |
| "R-76" | "Enums stored as their pinned names" | inapplicable | configuration / structural / review | HTTP/database-only mechanism; no corresponding Phase 1 capability. [VERIFIED: donor rule-matrix.md:108-108] |
| "R-77" | "Enum values only added" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:109-109] |
| "R-78" | "Domain enums never leave the backend" | adapted | configuration / structural / review | Keep service contracts free of core enum/entity dependencies. [VERIFIED: donor rule-matrix.md:110-110] |
| "R-79" | "Every external dependency has a health check" | inapplicable | configuration / structural / review | HTTP/database-only mechanism; no corresponding Phase 1 capability. [VERIFIED: donor rule-matrix.md:111-111] |
| "R-80" | "One tag per health check, matching its failure status" | inapplicable | configuration / structural / review | HTTP/database-only mechanism; no corresponding Phase 1 capability. [VERIFIED: donor rule-matrix.md:112-112] |
| "R-81" | "Options classes" | adapted | configuration / structural / review | Configuration DTO discipline retained as applicable; Microsoft binder is not introduced. [VERIFIED: donor rule-matrix.md:113-113] |
| "R-82" | "Production coverage of 90% lines and 80% branches" | adapted | configuration / structural / review | Keep 90%/80% local production coverage; instrument engine assembly meaningfully. [VERIFIED: donor rule-matrix.md:114-114] |
| "R-83" | "Every project has its own docs" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:115-115] |
| "R-84" | "Decision records" | retained | configuration / structural / review | Preserve rule and equivalent local positive/negative proofs. [VERIFIED: donor rule-matrix.md:116-116] |
| "R-85" | "In-process hosts only through LangApiFactory" | inapplicable | "RS0030" | HTTP/database-only mechanism; no corresponding Phase 1 capability. [VERIFIED: donor rule-matrix.md:117-117] |

<!-- DATA_D5F8362B_END -->

### Complete source-heading occurrence inventory

Every row records one heading occurrence, including shared/repeated subjects; line numbers belong to the original hashed source. The quoted headings/IDs are verbatim definitions. [VERIFIED: donor code-style.md:13-591; code-architecture.md:42-413, full reads this session]

<!-- DATA_92DB6A57_START -->

| Source file under CodeRules | Heading line | Exact heading | Linked source ID(s) |
|---|---|---|---|
| code-style.md | 13 | "Warnings are errors" | "R-01" |
| code-style.md | 21 | "No suppressions outside the rule matrix" | "R-67" |
| code-style.md | 52 | "UTF-8 encoding" | "R-02" |
| code-style.md | 58 | "LF line endings" | "R-03" |
| code-style.md | 64 | "Final newline" | "R-04" |
| code-style.md | 70 | "No trailing whitespace" | "R-05" |
| code-style.md | 76 | "Spaces only, no tab characters" | "R-06" |
| code-style.md | 82 | "Indentation of 4 spaces" | "R-07" |
| code-style.md | 88 | "Tab width of 4" | "R-08" |
| code-style.md | 94 | "Continuation indent of 8 spaces" | "R-09" |
| code-style.md | 222 | "Line length of 120 characters" | "R-10" |
| code-style.md | 231 | "File-scoped namespaces" | "R-11" |
| code-style.md | 237 | "Braces always" | "R-12" |
| code-style.md | 244 | "Usings outside the namespace" | "R-13" |
| code-style.md | 250 | "Using order" | "R-14" |
| code-style.md | 262 | "No unused usings" | "R-15" |
| code-style.md | 268 | "Implicit usings disabled" | "R-16" |
| code-style.md | 274 | "Var only when the type is apparent" | "R-17" |
| code-style.md | 281 | "Locals created with new use var" | "R-18" |
| code-style.md | 298 | "Target-typed new only where var cannot be used" | "R-19" |
| code-style.md | 305 | "Collection expressions preferred" | "R-20" |
| code-style.md | 312 | "Null checks use is null" | "R-21" |
| code-style.md | 318 | "Expression bodies only for properties and accessors" | "R-22" |
| code-style.md | 325 | "No primary constructors" | "R-23" |
| code-style.md | 353 | "Records only in non-positional form" | "R-24" |
| code-style.md | 360 | "No region directives" | "R-25" |
| code-style.md | 366 | "No null-forgiving operator" | "R-26" |
| code-style.md | 374 | "Private fields use _camelCase" | "R-27" |
| code-style.md | 380 | "No this qualification" | "R-28" |
| code-style.md | 388 | "Constants use PascalCase" | "R-29" |
| code-style.md | 394 | "No Async suffix required" | "R-30" |
| code-style.md | 402 | "Classes sealed by default" | "R-31" |
| code-style.md | 409 | "One type per file" | "R-32" |
| code-style.md | 415 | "Member order" | "R-33" |
| code-style.md | 437 | "Default visibility internal" | "R-34" |
| code-style.md | 446 | "XML docs on public types and members" | "R-35" |
| code-style.md | 453 | "No comments inside member bodies" | "R-36" |
| code-style.md | 469 | "Member body of 60 lines or fewer" | "R-37" |
| code-style.md | 483 | "Named arguments" | "R-38" |
| code-style.md | 506 | "Banned APIs" | "R-39", "R-40", "R-41", "R-42", "R-43" |
| code-style.md | 521 | "Logging through LoggerMessage" | "R-44" |
| code-style.md | 528 | "ConfigureAwait in Sebreiro packages" | "R-45" |
| code-style.md | 537 | "Tests follow every rule" | "R-46" |
| code-style.md | 544 | "Shouldly assertions" | "R-47" |
| code-style.md | 550 | "Test method names use Method_State_Expected" | "R-48" |
| code-style.md | 558 | "Analyzer coverage of 90% lines and 80% branches" | "R-49" |
| code-style.md | 566 | "Production coverage of 90% lines and 80% branches" | "R-82" |
| code-style.md | 575 | "In-process hosts only through LangApiFactory" | "R-85" |
| code-style.md | 586 | "Generated code exempt only under Migrations" | "R-50" |
| code-architecture.md | 42 | "Exactly the listed projects" | "R-51" |
| code-architecture.md | 52 | "CompositionRoot is the only host" | "R-52" |
| code-architecture.md | 59 | "Transitive project references disabled" | "R-53" |
| code-architecture.md | 68 | "Allowed project references" | "R-54" |
| code-architecture.md | 105 | "Interface-only abstractions" | "R-55" |
| code-architecture.md | 119 | "No entities in outward contracts" | "R-56" |
| code-architecture.md | 126 | "Per-layer registration" | "R-57" |
| code-architecture.md | 135 | "Domain depends only on System" | "R-58" |
| code-architecture.md | 141 | "Application has no infrastructure types" | "R-59" |
| code-architecture.md | 147 | "No IServiceProvider outside CompositionRoot" | "R-60" |
| code-architecture.md | 156 | "Integration projects are isolated" | "R-61" |
| code-architecture.md | 163 | "Reusable packages are product-agnostic" | "R-62" |
| code-architecture.md | 172 | "Domain entities" | "R-63" |
| code-architecture.md | 181 | "DTOs" | "R-64" |
| code-architecture.md | 193 | "Options classes" | "R-81" |
| code-architecture.md | 204 | "Hand-written mapping" | "R-65" |
| code-architecture.md | 211 | "Single version value" | "R-66" |
| code-architecture.md | 221 | "Member body length" | "R-37" |
| code-architecture.md | 230 | "XML docs and comments" | "R-35", "R-36" |
| code-architecture.md | 237 | "Every project has its own docs" | "R-83" |
| code-architecture.md | 253 | "Decision records" | "R-84" |
| code-architecture.md | 265 | "Named arguments at call sites" | "R-38" |
| code-architecture.md | 298 | "Property names in camelCase and enums as their pinned names" | "R-68" |
| code-architecture.md | 306 | "Exact-match enum input" | "R-69" |
| code-architecture.md | 315 | "Nulls omitted and unknown properties ignored" | "R-70" |
| code-architecture.md | 322 | "Missing and null required values rejected" | "R-71" |
| code-architecture.md | 330 | "Shared JSON settings configured once" | "R-72" |
| code-architecture.md | 337 | "Times and durations" | "R-73" |
| code-architecture.md | 350 | "Unknown is the zero value and never valid" | "R-74" |
| code-architecture.md | 358 | "Every enum member is pinned" | "R-75" |
| code-architecture.md | 366 | "Enums stored as their pinned names" | "R-76" |
| code-architecture.md | 375 | "Enum values only added" | "R-77" |
| code-architecture.md | 383 | "Domain enums never leave the backend" | "R-78" |
| code-architecture.md | 396 | "Every external dependency has a health check" | "R-79" |
| code-architecture.md | 408 | "One tag per health check, matching its failure status" | "R-80" |

<!-- DATA_92DB6A57_END -->

### Capture and exemption adaptation work

Capture the root/shared editorconfigs, props, targets, central packages, global SDK selection, both banned-symbol inputs, all twelve custom analyzers and their support helpers/release manifests, their tests/verifier, and the architecture-check source/helpers/fixtures actually adopted. The donor analyzer release list is "SCR0001" through "SCR0012"; all are errors. Preserve provenance/licensing when adopting reusable source; record modifications. [VERIFIED: donor AnalyzerReleases.Unshipped.md:8-20 and inspected project/config/source files]

Donor exemption dispositions must be explicit: EX-01 superseded body analyzer; EX-02 package async policy; EX-03 test naming; EX-04–EX-10 migration generation/BOM; EX-11 lock final newline; EX-12 audit-warning policy; EX-13 test executable; EX-14–EX-18 web visibility/subject rules; EX-19 analyzer self-edge; EX-20 analyzer coverage scope; EX-21 generated xUnit entry point; EX-23/EX-24 production coverage scopes; EX-25 binary fonts. Replace only those justified by local Godot/tooling inputs; do not import migration or web exceptions. Every local exemption needs an exact diagnostic/rule, path or symbol, reason, and boundary case; duplicate/overlapping same-diagnostic scopes must fail. [VERIFIED: donor rule-matrix.md:140-163] [CITED: 01-SPEC.md AC-08]

Retained machine rules need positive and deliberate negative proofs. Permission-only wording such as no required Async suffix needs a positive permission control and a negative configuration-drift proof; it does not imply banning Async-suffixed methods. Review-only obligations include the semantic need for public APIs, meaningful constructor invariants, mapping intent, enum evolution and ADR accuracy. Do not invent automation passes for these. [VERIFIED: donor rule-matrix.md:62-66, 95-98, 109-116] [CITED: 01-CONTEXT.md D-04]


## Common Pitfalls

1. **A partial source inventory is mistaken for inheritance.** The donor has shared headings and repeated headings, plus matrix-only subdivisions. Validate the immutable heading-ID relation and every matrix ID, not a guessed numeric range or first link per heading. Mutation proofs must delete one heading/row, duplicate an ID, change case, empty a required field and reorder rows. [CITED: 01-SPEC.md AC-05–AC-06; exact donor inventory above]
2. **Build settings exist but do not execute.** A project can bypass root imports, an analyzer edge can become a runtime reference, an SDK can add items, and a generated marker can hide handwritten violations. Inspect evaluated imports/items and run actual compiler, built-in, third-party, custom-analyzer and banned-API failures in isolated copies, with clean controls. Match diagnostic ID, file and subject; restore failure is not an analyzer pass. [CITED: 01-SPEC.md AC-07–AC-08; donor enforcement decision]
3. **Generator output changes the exemption scope.** Discover actual generated file identities from the selected compiler/generator, not a hand-authored .g.cs path. Prove authentic output compiles, handwritten companion stays enforced, fake markers fail, and an adjacent violation fails. Check duplicate/overlapping exemptions before matching. [CITED: 01-SPEC.md AC-08]
4. **A scene that quits is counted as a lifecycle proof.** The driver must load the fixture/resource through the engine, let callbacks occur, assert exported values and signal counts, observe teardown, and publish each surface's fresh result. Use at least two distinct instances and a delayed worker release after disposal. [CITED: 01-SPEC.md AC-14–AC-17]
5. **Only discovered results define what was required.** Required IDs come from an independent checked manifest. Compare exact selected obligations against fresh results and logs, reject skips/duplicates/stale output, and never let an empty discovery set become success. [CITED: 01-SPEC.md AC-03–AC-04]
6. **Snapshot deserialization mutates live objects.** Parse and validate all records/IDs/schema/ranges first. A fully accepted detached root replaces the prior root once; an invalid or interrupted candidate is discarded. Include ordered versus unordered semantics and repeated restore assertions. [CITED: 01-SPEC.md AC-18–AC-22]
7. **Coverage is weakened because CI is later.** Preserve both donor numeric gates as local checks. Instrument managed game code under actual Godot hosting and prove reports contain the expected assembly/files and nonzero counters. A .NET test report covering only Core does not establish game-assembly coverage. [CITED: donor R-49/R-82; 01-SPEC.md Constraints]
8. **Windows path resolution hides Linux failures.** Use Git's exact path inventory, inspect every local path segment and reject case-colliding entries independently of the host filesystem. Handle spaces, Unicode, percent encoding, fragments, duplicate headings/anchors and invalid UTF-8 explicitly. Do not scan installed GSD/plugin/vendor documentation as authored game docs. [CITED: 01-SPEC.md AC-10–AC-11]
9. **Parent tool state contaminates negative builds.** Pin working directory for global.json resolution; use argument arrays, disable node reuse/shared compiler/build servers for spawned proof builds, strip inherited build/profiler variables where appropriate, and bound process waits/cleanup. Keep coverage collection in its intended process; do not remove instrumentation from the engine run being measured. [VERIFIED: donor DotnetCli.cs:12-22] [CITED: https://learn.microsoft.com/en-us/dotnet/core/tools/global-json]

## Code Examples

Examples below are proposed fragments and command templates, not claims that the files or helpers exist yet.

### Nullable required export without a suppression

Godot documents private exported members. Keep a nullable staging property and reject missing required values before use; an optional property has its own separate contract. This fragment's local names are illustrative. [CITED: https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_exports.html; 01-SPEC.md AC-16]

~~~csharp
[Export]
private Resource? RequiredResource { get; set; }

private Resource ValidateResource()
{
    return RequiredResource
            ?? throw new InvalidOperationException(message: "RequiredResource is missing.");
}
~~~

This is not a complete source file or a substitute callback proof. Apply normal namespace/class/member/XML rules to the enclosing script; the actual engine-ready path calls the validator. [CITED: donor code-style.md; 01-SPEC.md AC-14–AC-16]

### Focused pure-test execution

Enable the documented "UseMicrosoftTestingPlatformRunner" property with value "true" in the chosen test project, then verify the exact pinned runner's help and reports. The MTP filter/report switches below are documented; paths, class selectors and result names are proposed. [CITED: https://xunit.net/docs/getting-started/v3/microsoft-testing-platform]

~~~text
dotnet run --project tests/phase1/KingdomZombies.Phase1.Tests.csproj --no-build -- --filter-class "*RegisterTests" --fail-skips on --results-directory <fresh-run-dir> --report-xunit --report-xunit-filename pure.xml
~~~

The normal complete entry point must validate the expected IDs, even when the framework returns zero. A focused command is editing feedback only. [CITED: 01-CONTEXT.md D-10–D-13; 01-SPEC.md AC-03]

### Actual engine path

The selected engine supports --headless, --path and --scene; unknown engine arguments may have no effect, so put project-specific proof parameters after -- and reject unknown user options yourself. The scene path and proof argument names below are proposed. [CITED: https://docs.godotengine.org/en/4.7/tutorials/editor/command_line_tutorial.html]

~~~text
<godot-mono> --headless --path src/godot-game --editor --quit
dotnet build src/godot-game/KingdomZombies.csproj --no-restore
<godot-mono> --headless --path src/godot-game --scene res://proofs/Phase1.tscn -- --proof-run-id <nonce> --proof-results <fresh-path>
~~~

Bound the engine process externally; the driver quits only after required assertions and cleanup, with explicit failure status otherwise. The initial editor/import command may need the separate .NET 8 runtime shown in the inspected package configuration. [CITED: 01-SPEC.md AC-03 and AC-14–AC-17; inspected ZIP runtimeconfigs]

### Coverage without a new game framework

Use the pinned local tool to collect managed process/subprocess coverage and merge test/engine contributions. The tool documents both dynamic and static instrumentation; whether dynamic collection sees the native-hosted game assembly remains an execution proof, not a documentation guarantee. [CITED: https://learn.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-coverage]

~~~text
dotnet tool restore
dotnet tool run dotnet-coverage collect "<godot-mono> --headless --path <isolated-game-copy> --scene res://proofs/Phase1.tscn -- <proof-args>" -f cobertura -o <fresh-engine-coverage>
~~~

Use isolated build output for any static instrumentation fallback and never rewrite approved source/rule inputs. If collection is unavailable on one target, leave the obligation failed/blocked and resolve tooling; do not lower coverage or omit the game assembly. [CITED: 01-SPEC.md AC-03–AC-04; donor coverage rules]

## Runtime State Inventory

The only rename in scope is the root guide's filename casing, not game identifiers or external system names. Inventory scope is therefore this documentation rename and the new local proof inputs. No live-game migration is claimed. [CITED: 01-SPEC.md RULE-04; 01-CONTEXT.md D-18]

| Category | Observation | Required plan action |
|---|---|---|
| Stored data | No game datastore is present in the inspected tracked starter inputs; no game implementation has been accepted. This is a repository observation, not an audit of arbitrary external databases. [CITED: 01-SPEC.md Background; inspected git ls-files] | No game data migration for filename casing |
| Live service config | No service identifier changes are proposed; this phase has no configured game service to rename in its declared scope. [CITED: 01-SPEC.md Boundaries] | None for this rename; do not claim unrelated external UIs audited |
| OS-registered state | No OS registration is renamed by canonicalizing a Markdown filename. Existing WSL capability is an environment issue, not renamed runtime state. [CITED: 01-SPEC.md RULE-04; environment probes] | Use an intermediate Git rename if needed on Windows; assert one exact index entry |
| Secrets/env vars | The filename correction does not change a secret/env key; bootstrap introduces explicit non-secret tool paths only. No secret values were read. [CITED: proposed topology and research probe record] | Document new path variables; never persist credentials/log environment dumps |
| Build artifacts | Existing imported Godot cache is not evidence; no C# build baseline currently exists. New proof runs must isolate outputs/caches and reject stale reports. [CITED: 01-SPEC.md Background, AC-03–AC-04] | Fresh-run directories and clean-checkout proofs; no data migration |

## Environment Availability

All observations are from this research session; a command lookup failing is narrower evidence than a system-wide absence. [VERIFIED: local command and filesystem probes]

| Dependency | Needed for | Observation | Executable next step / fallback |
|---|---|---|---|
| Windows host | Windows evidence | dotnet reports Windows 10.0.19044, win-x64 | Record complete OS/arch in run report |
| dotnet | Build/pure proofs | SDK 10.0.404, MSBuild 18.9.15; runtimes 8.0.16 and 10.0.15 installed | Use exact executable path and workspace CLI home |
| csc | Compiler identity | 5.9.0-1.26529.119, commit 2ab2854867733112c84ac12c4273d7970ac3afdf | Record separately from package version |
| Godot command | Real engine proof | No godot/godot4 command found in PATH | Workspace extraction of supplied stable ZIP, preserving the whole GodotSharp directory |
| Godot archive | Reversible Windows bootstrap | ZIP in Downloads contains matching engine, console launcher, GodotSharp and four SDK/binding/generator nupkgs | Verify official archive hash, extract into ignored tool cache; clean checkout must declare official source/hash, not depend on Downloads |
| NuGet package cache | Restore | Checked default user package directory did not provide a usable cache | Use declared workspace NUGET_PACKAGES and an authorized feed/network path |
| .NET first-run writes | All CLI operations | dotnet nuget locals failed UnauthorizedAccessException for user .dotnet/10.0.404.toolpath.sentinel | Set DOTNET_CLI_HOME under ignored workspace directory before invocation; also set writable package/cache/temp paths |
| Shell network | Package/tool downloads | curl NuGet/GitHub requests fail connecting to 127.0.0.1 port 9 | Approved network-capable execution or verified offline package archive/feed; no --ignore-failed-sources substitute pass |
| Web research | Docs/version verification | Built-in web lookup reaches official sources | Supplies docs evidence, not proof of local restore |
| WSL | Linux execution | wsl --status exit 50; --list --verbose printed install usage; no Lxss distro entries were returned | No Linux execution established. Provision a real distro/VM or execute the same checkout on a Linux machine |
| Docker/VM/SSH command | Alternate Linux execution | docker, qemu-system-x86_64, VBoxManage and ssh not found in checked PATH; checked Program Files names showed no matching host | No working alternative established; do not claim impossible to install or use a separate Linux host |
| Node | Optional small cross-platform entry point | v24.22.1 installed | Recommend dependency-free CJS process coordinator; pin/document Node as a dev tool if chosen |
| Git | Exact tracked casing/inventory | 2.56.1.windows.1 | Use git ls-files -z and ordinal paths; fixture snapshots must include newly authored inputs before clean-checkout acceptance |
| Python | Optional tooling | WindowsApps aliases found; interpreter execution was not established | Do not depend on Python for these plans |

Inspected Windows ZIP SHA-256: "A2A48473A7414C5F19FAB690518CAEBB738C09EF9601F6BD2388676A7F53B3C0". This is a measured local hash, **not yet compared with an official checksum**. Record that distinction. [VERIFIED: Get-FileHash probe]

A dependency-free Node coordinator is the recommended small launcher because Node is present and C# remains the language of all engine/example/test code. Official Node release notes confirm 24.22.1 LTS/security release dated 2026-10-21. It adds a declared Linux bootstrap requirement if selected; no npm dependency is needed. Use argument-array child creation with hidden Windows child windows and no shell interpolation; keep all rule logic in the C# proof project. [CITED: https://nodejs.org/en/blog/release/v24.22.1; https://nodejs.org/api/child_process.html; 01-CONTEXT.md D-09]

**Reversible workspace bootstrap:** create ignored tool/CLI/package/result directories; extract the official engine distribution; install a pinned SDK/runtime locally when not already present; use its included Godot nupkgs as a declared local source where appropriate, plus verified declared NuGet dependency inputs. Microsoft documents versioned install-directory SDK bootstrap. No global machine mutation is needed for Windows extraction or SDK-local setup. [CITED: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script; inspected ZIP manifests]

**External environment requirement:** enabling WSL may require administrator action and restart according to Microsoft's install guide. This research did not attempt OS-feature changes. A genuine Linux host/VM/WSL distro with the selected SDK, editor runtime, engine and declared development tools can execute the same headless proof. Record Linux distribution/kernel/arch, not simply "Linux" inferred from a cross-target build. A Linux-native clone or case-sensitive worktree is preferable to relying only on a mounted Windows directory for casing evidence. [CITED: https://learn.microsoft.com/en-us/windows/wsl/install; 01-SPEC.md AC-02, AC-11]

**Missing dependencies with no currently established execution route:** real Linux runtime; authorized network or complete verified offline restore feed. These block phase completion until supplied, not planning or the independent Windows/bootstrap work. [VERIFIED: probes above] [CITED: 01-SPEC.md RULE-01]

## Validation Architecture

Nyquist is enabled. No project-owned C# test infrastructure exists in the inspected starter; the test project, framework setup and all cases below are Wave 0/implementation work. All names/paths/commands in this section are proposed, not existing passing tests. [CITED: .planning/config.json; 01-SPEC.md Background]

### Test framework and entry points

| Property | Proposed value |
|---|---|
| Pure/check/analyzer framework | xUnit v3 4.0.2 + Shouldly 4.3.0, one test project |
| Runner mode | Explicit UseMicrosoftTestingPlatformRunner=true; invoke built test executable through dotnet run --no-build |
| Engine fixture | One actual C# driver scene in existing game project |
| Launcher | Dependency-free node tools/phase1/check.cjs; focused --group docs, pins, register, rules, graph, state, engine, runner; default/all runs all |
| Required-case input | Checked exact case manifest; per-data-row identity for threshold/mutation cases |
| Quick command | node tools/phase1/check.cjs --group <changed-group>; pure checks target <30 seconds after build |
| Full suite | node tools/phase1/check.cjs --all; includes restore/build, rules, engine, runner self-proofs and local coverage gates |
| Documentation-only | node tools/phase1/check.cjs --group docs |
| Raw focused xUnit | dotnet run --project tests/phase1/KingdomZombies.Phase1.Tests.csproj --no-build -- --filter-class "*<Group>Tests" --fail-skips on |
| Engine raw | <godot-mono> --headless --path src/godot-game --scene res://proofs/Phase1.tscn -- <run identity/output args> |

The launcher CLI above is a proposed contract the planner must implement and document. Raw framework commands alone cannot satisfy required-case reconciliation. Validate the pinned framework's actual help before freezing invocation; avoid root test-runner selection that silently changes future adapters' behavior. [CITED: official xUnit MTP docs; 01-SPEC.md AC-03]

### Phase requirements → test map (all 22 acceptance criteria)

Each row's coverage obligation comes from the identically numbered acceptance criterion in 01-SPEC.md. Commands use the proposed launcher groups; "No" means the files/cases must be implemented, not skipped. [CITED: 01-SPEC.md AC-01–AC-22]

| AC / requirement | Cases and assertions | Type / proposed file | Automated command | Exists? |
|---|---|---|---|---|
| AC-01 / RULE-01 | Coherent positive pins; missing pin; conflicting duplicate declarations; unsupported/mismatched executable; reordered equivalent declarations; wrong SDK/compiler/engine/generator | Integration + mutation / PinsTests.cs | check --group pins | No |
| AC-02 / RULE-01,06 | Clean checkout on Windows and separately Linux: locked restore, build, all rules, actual engine surfaces. Donor path unavailable. Capture revision/inputs/toolchain/runtime/platform/command/logs | Real processes / PlatformEvidenceTests.cs + external OS execution | check --all on each OS | No |
| AC-03 / RULE-01,03 | Required fresh unique results/evidence; deliberately fail; missing tool; zero discovery/subjects; wrong filter/selection; skip; crash; timeout; deleted report/log; stale report; negative case whose failure is unrelated infrastructure | End-to-end launcher self-proofs / RunnerContractTests.cs | check --group runner | No |
| AC-04 / RULE-01,03,04,07 | Repeat same inputs; interrupt before publication; stale output from prior run; two concurrent invocations; verify source/doc hashes unchanged. Reject second invocation or use isolated complete snapshots | Process/concurrency / RunnerContractTests.cs | check --group runner | No |
| AC-05 / RULE-02 | All source headings and 85 captured IDs; delete source relation/row; duplicate/conflicting IDs; empty inventory; empty classification/reason; invalid disposition; adaptation scope/reason absent | Unit + repository / RegisterTests.cs | check --group register | No |
| AC-06 / RULE-02,03 | Bidirectional exact ID/docs/mechanism/exemption/proof resolution; unknown/case-changed ID; missing mechanism/proof; ambiguous ref; weakened mapping; reorder rows/files; separate review-only component | Unit + repository / RegisterTests.cs | check --group register | No |
| AC-07 / RULE-03 | For every machine rule: compliant/violating subject and exact diagnostic/location; build-level compiler/built-in/third-party/custom/banned API examples; architecture/exemption real-build failures; duplicate evidence cannot cover another case | Analyzer + actual build / EnforcementTests.cs and adopted analyzer cases | check --group rules | No |
| AC-08 / RULE-03,06 | Exact exemption positive; immediately adjacent negative; duplicate/overlap rejected; unregistered suppression/severity weakening/fake-generated filename and marker rejected; actual generated partial + analyzed handwritten companion build | Unit + actual build / ExemptionTests.cs | check --group rules | No |
| AC-09 / RULE-03 | All retained numeric limits: below/equal/above; non-ASCII UTF-16; invalid encoding; overflow/rounding; ordering; coverage exact ratios and report presence; continuation/block distinction | Analyzer/unit/coverage / ThresholdTests.cs | check --group rules | No |
| AC-10 / RULE-04 | One exact Git-index AGENTS.md; no case alternative; concise navigation; required indexes and per-project docs; declared dependencies equal evaluated graph | Repository / DocumentationTests.cs | check --group docs | No |
| AC-11 / RULE-04 | Nonempty required docs; missing target/anchor; ambiguous anchors; case mismatch/collision; singleton index; link reorder; spaces/Unicode; invalid text; project/vendor scope boundary | Unit + repository on each OS / DocumentationTests.cs | check --group docs | No |
| AC-12 / RULE-05 | Exact declared project set and allowed directions; Core/service contract purity; representative permitted boundary use | Evaluated MSBuild + compiled types / GraphTests.cs | check --group graph | No |
| AC-13 / RULE-05 | Forbidden direct/imported/wildcard/semicolon references; transitive reference; type/signature/generic/body/generated-state-machine leak; undeclared dependency; missing project/type/selector; declared dependency-free component positive; duplicates/reorder | Synthetic + real compilation / GraphTests.cs | check --group graph | No |
| AC-14 / RULE-06 | Engine-created scene/resource instance; construction; enter/ready/process/exit callbacks; authentic generated partial; exports; signal emit/handler; nullable contract; clean exit. One result identifies every surface | Real engine / Phase1 driver + fixture | check --group engine | No |
| AC-15 / RULE-06 | At least two complete new-instance cycles; documented callback order; one intended handler per signal; no stale subscription; release queued result after first unload and prove old/new state untouched | Real engine lifecycle | check --group engine | No |
| AC-16 / RULE-06 | Valid required exports; missing required resource/value; invalid required null rejected before use; deliberately optional null accepted | Real engine loading different minimal fixture inputs | check --group engine | No |
| AC-17 / RULE-06,07 | Main-thread positive; worker-negative rejected before engine call; supported pure worker result returns via main-thread gateway; disposed-generation result discarded; Core runs with no Godot references | Guard + real engine + pure check | check --group engine, then state | No |
| AC-18 / RULE-07 | Same initial state/time/random seed-state gives same values/IDs in independent runs; save/restore next draws exactly; static/semantic checks reject uncontrolled clock/global random bypass; common vectors across both OS | Pure / StateContractTests.cs | check --group state | No |
| AC-19 / RULE-07 | Unit/precision/range declarations; zero no advance; min/max and nearest outside; negative delta; checked accumulation overflow; precision-sensitive serialization; non-finite only where actually representable; rejected input leaves live root unchanged | Pure / StateContractTests.cs | check --group state | No |
| AC-20 / RULE-07 | Identity/value exact round-trip; ordered sequence retained; unordered permutation semantic equality; distinct valid Unicode/case identities remain distinct; reject transient peer/address/Godot references in snapshot representation | Pure + type checks / StateContractTests.cs | check --group state | No |
| AC-21 / RULE-07 | Empty/single/multiple positive; absent/empty required ID; duplicate/conflicting identity; malformed/truncated/absent state; unsupported schema; null collection/element; error leaves prior root intact | Pure / StateContractTests.cs | check --group state | No |
| AC-22 / RULE-07 | Repeat accepted restore without append/new ID/random/time/effect; interrupt during candidate preparation; compete or reject explicitly; reader observes complete old/new root; no partial application | Pure concurrency / StateContractTests.cs | check --group state | No |

"check" in the command column abbreviates the proposed "node tools/phase1/check.cjs" command; comma-separated prose in AC-17 denotes two invocations, not an undocumented CLI feature. Group tests can be parameterized to cover many cases without manufacturing one source file per row. Record every parameterized case's stable unique identity. [CITED: 01-SPEC.md AC-03–AC-09]

### Truthful result and failure architecture

Implement only the Phase 1 required set. Before launch, allocate a unique run directory and either acquire one exclusive repo check lock or snapshot inputs/build outputs per run. The simplest policy is to reject a second complete invocation clearly, while recursive self-proof children use separate explicitly isolated fixture roots. Do not mix run logs or mutate shared generated/coverage outputs concurrently. [CITED: 01-SPEC.md AC-04; 01-CONTEXT.md D-13]

Each result needs exact case ID plus data-row identity, run nonce, input revision/content identity, tool/platform identity, status and evidence paths. Verify evidence files exist under that run root, belong to that run, are nonempty where required, and match the selected obligation. Complete publication occurs only after process exits and every required ID reconciles; write a temporary summary and publish it once after validation. Never accept a prior summary based solely on its filename/mtime. [CITED: 01-SPEC.md AC-03–AC-04]

Collect independent failures; mark downstream obligations blocked when restore/build/engine prerequisites fail. A blocked or skipped case is non-success. Normalize diagnostic aggregation order for repeatable presentation while retaining exact case identity. A negative test requires the intended rule/subject/location and a successful infrastructure prerequisite; an unrelated exception or package restore error cannot satisfy it. [CITED: 01-CONTEXT.md D-12–D-13; 01-SPEC.md AC-03, AC-07]

Runner self-proofs must exercise child processes and real entry-point behavior: missing executable, zero discovery, wrong filter, declared skip, deliberate assertion failure, crash, timeout/kill, missing log/result, duplicate result, wrong run nonce, stale prior pass, interrupted publication, concurrent starts and source mutation. Do not implement these as tests that merely pass fabricated lists to the same summarizer. Keep data-level parser/unit tests too, but process-level failures establish actual propagation/cleanup. [CITED: 01-SPEC.md AC-03–AC-04]

### Sampling and Wave 0 gaps

- Per edit: appropriate group; pure group target below 30 seconds after build. Slow actual-build/engine/coverage cases remain explicit, bounded integration checks, not silently omitted for speed.
- Before finishing a code/rule/command/contract change: complete suite; per wave: complete relevant implementation suite.
- Prose-only documentation changes: documentation checks.
- Phase gate: complete required fresh non-skipped result set plus Windows **and** Linux evidence and both adopted coverage gates before verification closure. [CITED: 01-CONTEXT.md D-10–D-13; 01-SPEC.md AC-02–AC-04]

Wave 0 tasks to include:

- [ ] Establish ignored writable tool/cache/run directories and a declared reproducible toolchain input manifest.
- [ ] Prove candidate restore/build + authentic engine creation/generation/load/unload on Windows; schedule and secure Linux execution now.
- [ ] Capture full donor source identity, all 84 heading occurrences/85 IDs/24 exemptions/configurations and the selected adopted analyzer/check source/tests.
- [ ] Create the exact small project graph, independent subject inventory and one proof project.
- [ ] Add the checked required-case manifest and fresh evidence reconciliation before relying on test results.
- [ ] Add documentation/register/exemption fixtures and adopted analyzer positive/negative cases, including threshold neighbors.
- [ ] Add minimal real-build bad-source fixtures and exact expected diagnostic parsing.
- [ ] Add real engine lifecycle fixture/resources and pure state contract tests.
- [ ] Prove code coverage collection includes actual managed engine code; local gates retain donor 90%/80% limits.
- [ ] Execute the same source revision in a genuine Linux environment and reconcile both reports. [CITED: 01-SPEC.md AC-01–AC-22]

## Security Domain

Security enforcement is enabled; the phase needs input/file/process and tooling integrity controls, not speculative game account/session systems. ASVS is primarily a web application standard. Current ASVS 5.0.0 was published 2025-05-30 and renumbered categories; do not label authentication "V2" or validation "V5" as if those were current identifiers. [CITED: .planning/config.json; https://asvs.owasp.org/; https://github.com/OWASP/ASVS/tree/v5.0.0/5.0/en]

### Applicable ASVS categories (5.0.0 names)

Applicability judgments below are research recommendations scoped to Phase 1, not an ASVS certification claim. [CITED: 01-SPEC.md Boundaries; official category index above]

| Category | Applies | Standard control adapted to this stack |
|---|---|---|
| V1 Encoding and Sanitization | Yes, tooling inputs | Strict UTF-8 decode; exact ordinal identity/casing; no shell interpolation |
| V2 Validation and Business Logic | Yes | Validate pins/registers/exemptions/case reports/snapshot candidates before acceptance |
| V5 File Handling | Yes | Resolve outputs under isolated run root; reject escaping paths/ambiguous links; bounded file reads |
| V6 Authentication | No Phase 1 account surface | No account implementation introduced |
| V7 Session Management | No network session implementation | Node lifetime is an engine correctness contract, not an auth session |
| V8 Authorization | No external access-control system | Process/check ownership and approved filesystem bounds only |
| V11 Cryptography | Only standard hashes for provenance | System SHA-256/tool-provided checksums; no custom cryptography |
| V13 Configuration | Yes | Exact tool pins, evaluated build settings, locked dependencies and narrow exemptions |
| V15 Secure Coding and Architecture | Yes | Pure core/service boundaries; main-thread/lifetime guard; negative proofs |
| V16 Security Logging and Error Handling | Yes, proportionately | Actionable non-secret error/log paths; nonzero exit on incomplete proof |

### Known threat patterns and mitigations

| Pattern | STRIDE | Required mitigation/proof |
|---|---|---|
| Forged/stale/duplicated pass report | Spoofing / Repudiation | Run nonce + input identity + independently required IDs + evidence existence; stale/duplicate injection fails |
| Malicious path/command input | Tampering / Elevation of privilege | Argument arrays, no shell execution, canonical workspace/run-root containment; escaping path fixture fails |
| Crafted snapshot or register input | Tampering / Denial of service | Bounded parse, strict schema/range/identity checks; complete detached validation then atomic replacement |
| Fake generated code / severity weakening | Tampering | Syntax-aware suppression scan and exact non-overlapping exemption registry; handwritten boundary proof |
| Run hangs / orphaned workers | Denial of service | Bounded child lifetime, cancellation and confirmed cleanup; interrupted pass cannot publish |
| Tool/dependency substitution | Spoofing / Tampering | Exact executable version/path/hash + locked package graph + official provenance; wrong-tool fixture fails |

These threat controls derive from the locked failure/input boundaries; they do not add authentication, encryption-at-rest or online services to the phase. [CITED: 01-SPEC.md AC-01–AC-09, AC-21–AC-22]

## State of the Art

| Prior/reference approach | Selected phase approach | Evidence / impact |
|---|---|---|
| Donor LangVersion "latest" with latestPatch SDK roll-forward | Exact SDK plus explicit language version and recorded csc identity | Donor declarations are "<LangVersion>latest</LangVersion>" and '"rollForward": "latestPatch"'; reproducibility here requires a coherent selected baseline. [VERIFIED: donor Directory.Build.props:6; global.json:3-4] |
| A research candidate is treated as support proof | Positive restore/build/native-host engine compatibility test | Registry existence/computed TFM metadata does not satisfy actual two-OS lifecycle evidence. [CITED: 01-SPEC.md AC-01–AC-02, AC-14] |
| Root global test runner selection copied into all projects | Explicit xUnit executable mode; Godot process separately invoked | MTP and native xUnit switches differ; keep the actual selected runner explicit. [CITED: official xUnit MTP docs] |
| Web migration generated-code exemption | Actual Godot/compiler generated-surface exemption | All handwritten partial code remains checked; fake generation markers fail. [CITED: 01-SPEC.md AC-08] |
| CI-only donor coverage gate | Same numeric local gate now; CI wiring later | No scope reduction or weakened thresholds. [CITED: donor R-49/R-82; 01-SPEC.md Boundaries] |

## Assumptions Log

| # | Claim | Section | Risk if wrong / resolution |
|---|---|---|---|
| A1 | Godot 4.7.2 with net10.0/C#14 is an integrated compatible first target candidate | Standard Stack | Native runtime/generator/lifecycle failure; execute compatibility slice before locking; documented stable fallback only after actual failing evidence |
| A2 | Seed + bounded draw position is an adequate minimal System.Random continuation representation for both selected platforms | Architecture Patterns | Different draw results or expensive replay; test fixed operation/golden vectors and restored next draw on both OS, or choose an established explicit-state generator and audit its package |

No assumption asserts platform execution has occurred. Proposed project names, file names, fixture layouts, integer-unit choice and CLI group names are design recommendations within the discretion explicitly left to planning, not findings about existing repository values. Dependency publication dates that could not be retrieved remain unresolved observations, not assumed releases. [CITED: 01-CONTEXT.md Agent's Discretion]

## Open Questions

1. **Integrated candidate target and engine-generated treatment:** Availability of Godot/SDK and generator packages is established; Windows/Linux native hosting, generated output, exact diagnostic exceptions and coverage under the selected target are not. Put an executable compatibility slice before bulk rule adaptation; record genuine failing diagnostics if a fallback is required. [CITED: 01-SPEC.md AC-01–AC-02, AC-08, AC-14]
2. **Linux execution route:** No working runtime was established locally. Plan WSL/VM/real Linux execution with the pinned toolchain now; if OS installation requires external action, retain a concrete environment checkpoint. Continue independent implementation, but phase closure must wait for actual Linux results. [VERIFIED: environment probes] [CITED: 01-SPEC.md AC-02]
3. **Dependency acquisition:** Shell NuGet/GitHub requests failed, and no usable default package cache was observed. Resolve authorized network access or a verified offline feed. A feed must include all transitive pinned inputs, not just the Godot nupkgs. No restore/build compatibility claim can be made until that succeeds. [VERIFIED: curl and cache probes]
4. **Coverage of native-hosted managed code:** Official tooling supports managed process collection, but this engine hosting path was not instrumented. Test the expected assembly and symbols; use isolated static instrumentation when needed. Keep exact donor thresholds rather than excluding unmeasured code. [CITED: official dotnet-coverage docs; donor coverage rules]
5. **Donor licensing/provenance packaging:** Record reusable source license/notice requirements during capture; the full licensing chain was not audited here. This is a capture task, not permission to modify donor originals. [CITED: 01-SPEC.md provenance constraints]

## Sources

### Primary inspected local authority (HIGH for contents)

- Root Agents.md; docs/workflow/ai-collaboration.md and gsd.md; phase SPEC/CONTEXT; REQUIREMENTS/ROADMAP/PROJECT/STATE/config; research SUMMARY. These define scope; SUMMARY is not version authority.
- Clean donor commit and hashes listed above; full code-style/code-architecture/rule-matrix/enforcement ADR; shared props/targets/editorconfig/central packages/global.json/banned symbols.
- Donor custom analyzer project/release manifest; BodyLengthAnalyzer, NamedArgumentAnalyzer; AnalyzerVerifier; architecture ProjectEvaluator, GeneratedCodeScanner, LayerPurityTests, RuleMatrix, SourceFileFormat, real-build BuildProofTests and DotnetCli.
- Local dotnet/csc/node/git/tool lookups, WSL status, checked registry/paths, curl failures and inspected Godot archive manifests/runtimeconfigs. No Linux build or full integration was executed by research.

### Official technical documentation (MEDIUM via classifier)

- [Godot 4.7.2 archive](https://godotengine.org/download/archive/4.7.2-stable/) and [SDK registry](https://www.nuget.org/packages/Godot.NET.Sdk/4.7.2): release/platform/package identity.
- [Godot C# basics](https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_basics.html), [exports](https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_exports.html), [signals](https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_signals.html), [Node](https://docs.godotengine.org/en/4.7/classes/class_node.html), [thread-safe APIs](https://docs.godotengine.org/en/4.7/tutorials/performance/thread_safe_apis.html) and [CLI](https://docs.godotengine.org/en/4.7/tutorials/editor/command_line_tutorial.html): engine proof boundaries.
- [.NET downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), [global.json](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json), [local SDK install](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script), [JSON nullable annotations](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/nullable-annotations), [coverage utility](https://learn.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-coverage): pins/bootstrap/state/coverage.
- [xUnit release](https://xunit.net/releases/v3/4.0.2), [runner modes](https://xunit.net/docs/getting-started/v3/microsoft-testing-platform), [Shouldly source](https://github.com/shouldly/shouldly), [Meziantou source](https://github.com/meziantou/Meziantou.Analyzer), [Roslynator source](https://github.com/dotnet/roslynator), [ArchUnitNET docs](https://archunitnet.readthedocs.io/en/latest/), package pages inline in Standard Stack.
- [WSL installation](https://learn.microsoft.com/en-us/windows/wsl/install), [Node LTS release](https://nodejs.org/en/blog/release/v24.22.1), [ASVS category index](https://github.com/OWASP/ASVS/tree/v5.0.0/5.0/en): environment and scoped security taxonomy.

### Research method and confidence provenance

Init.phase-op and agent-skills --skills-only were executed; configured skills are empty; graphify status reports disabled. The research-plan seam returned Context7 for three Godot/.NET questions. No Context7 MCP or ctx7 executable was available, so official web sources were used as fallback. classify-confidence returned LOW for websearch alone and MEDIUM with --verified; official fetched material is therefore CITED/MEDIUM here. Direct inspected local source/probe observations are distinguished explicitly. NuGet is unsupported by the legitimacy seam; no OK verdict is claimed. Research cache was not written outside this assigned artifact ownership. [VERIFIED: seam and tool discovery probes]

## Metadata

| Area | Confidence | Reason |
|---|---|---|
| Scope and donor inventory | HIGH | Locked source read; 85 IDs, all 84 heading occurrences, exact numeric text and content hashes captured |
| Candidate stack | MEDIUM | Official releases/registry and local executable versions checked; restore/native integration unproven |
| Architecture | MEDIUM | Locked requirements + inspected reusable enforcement patterns; proposed small structure still needs execution |
| Pitfalls and validation design | MEDIUM | Every AC has a concrete positive/negative strategy; runtime proof not yet run |
| Two-platform availability | HIGH for limited observations | Windows observed; no working Linux route established by these probes; absence is not a support claim |

**Research date:** 2026-11-13.
**Valid until:** Revalidate tool releases/feeds/environment before execution, or within 7 days; captured donor identity remains valid only for the recorded commit/hashes.
**Commit:** Not performed; root orchestrator owns artifact commits.

