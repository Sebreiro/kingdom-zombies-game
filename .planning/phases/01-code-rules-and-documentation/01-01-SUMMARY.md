---
phase: 01-code-rules-and-documentation
plan: "01"
subsystem: infra
tags: [godot, csharp, dotnet, bootstrap, pins]
requires: []
provides:
  - Verified Windows Godot 4.7.2 and .NET 10 engine-created C# compatibility slice
  - Exact tool declarations, isolated package feed and reproducible official acquisition
affects: [01-02, toolchain, engine-proofs]
actuals:
  tokens: 9311
  tasks: 2
  commits: 2
plan_head_before: 2a68133df7aa03055f6b0ab2562e3771a5fab810
plan_head_after: 2ba3a32bf866cc2d13b6598557e6d1578e668908
tech-stack:
  added: [Godot.NET.Sdk 4.7.2, .NET SDK 10.0.404, C# 14.0, Node 24.22.1]
  patterns: [nonce-tagged engine assertions, workspace-local CLI state, checksum-verified package feed]
key-files:
  created: [tools/phase1/bootstrap.cjs, tools/phase1/toolchain.json, src/godot-game/KingdomZombies.csproj, src/godot-game/proofs/Phase1.cs, src/godot-game/proofs/Phase1.tscn, global.json, NuGet.config, .config/dotnet-tools.json, docs/workflow/setup.md]
  modified: [.gitignore, .planning/config.json]
key-decisions:
  - "Keep the successfully hosted net10.0/C#14 baseline; no net8 fallback was necessary."
  - "Authenticate unsigned Godot nupkgs through official archive SHA-512 and exact package SHA-256, without claiming signature validity."
  - "Keep early logs explicitly raw compatibility evidence until 01-02 establishes trusted acceptance reconciliation."
requirements-completed: []
requirements-addressed: [RULE-01, RULE-06]
coverage:
  - id: D1
    description: Windows native-host scene construction, ready and exit
    requirement: RULE-06
    verification:
      - kind: integration
        ref: node tools/phase1/bootstrap.cjs --probe engine
        status: pass
    human_judgment: false
  - id: D2
    description: Exact pins and missing/conflicting/mismatched declaration controls
    requirement: RULE-01
    verification:
      - kind: integration
        ref: node tools/phase1/bootstrap.cjs --probe pins
        status: pass
    human_judgment: false
duration: 9min
completed: 2026-11-13
status: complete
---

# Phase 1 Plan 01: Pinned native-host toolchain Summary

**Godot 4.7.2 constructs and runs a net10.0 C# scene on Windows using exact tool pins, verified archive packages and isolated restore/build state.**

## Performance

- Observed implementation window: 2026-11-13T17:00:31Z to 2026-11-13T17:09:33Z, approximately 9 minutes; preliminary context reads precede this window.
- Tasks: 2/2; implementation files: 10.
- Actual token scale: 37,242 realized diff characters / 4, rounded up to 9,311. Commit count is measured from the persisted per-plan base through `plan_head_after`, before metadata commits.

## Accomplishments

- Existing `src/godot-game` project restores through a complete verified four-package feed, repeats restore with `--locked-mode`, builds and launches `res://proofs/Phase1.tscn` in the real native engine. Three exact fresh assertions establish construction, ready and exit, in order.
- Child environments use `.phase1` CLI home, NuGet packages, HTTP cache and temporary directories. Native engine and .NET children use argument arrays, hidden Windows windows, bounded execution and process-tree termination on timeout.
- Exact declarations cover engine, SDK/compiler, language/framework, Node, application/editor runtimes, Godot packages and local coverage tool. Strict bounded JSON parsing rejects duplicate keys before ordinary parsing loses identity. Built-in controls reject absent, duplicate, conflicting and mismatched pins; equivalent ordering preserves selection.
- Official acquisition succeeded with Downloads disabled and a forced HTTPS archive download. Donor paths are absent from the runtime/build inputs. Setup commands and archive identities are in the focused setup reference.

## Task Commits

1. **Bootstrap one real Godot C# scene without donor runtime dependencies** — `70bf15d` (feat).
2. **Freeze coherent declarations and reproducible local setup** — `2ba3a32` (chore).

## Observed Toolchain

| Component | Observed identity |
| --- | --- |
| Windows | x64, OS release 10.0.19044 |
| Godot | `4.7.2.stable.mono.official.ed1daf0bf` |
| Native executable | `.phase1/tools/Godot_v4.7.2-stable_mono_win64/Godot_v4.7.2-stable_mono_win64_console.exe` |
| Native executable SHA-256 | `2445d009a5e0474fc7064b9767100e9d2c09521890ac5625476bfb81cf03f2d4` |
| SDK | `10.0.404`, installed beneath `C:/Program Files/dotnet` |
| Compiler | `5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf)`; `C:/Program Files/dotnet/sdk/10.0.404/Roslyn/bincore/csc.dll` |
| Scene runtime | `10.0.15`, observed inside the engine |
| Editor runtime | `Microsoft.NETCore.App 8.0.16` present; editor-tools runtime configuration targets net8.0 |
| Node | `24.22.1`, `C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/bin/node.exe` |
| Display/renderer | `headless` / `gl_compatibility`; this is no visual or performance claim |
| Bindings/generator | All four Godot packages exactly 4.7.2, source revision `ed1daf0bf001b61586d9930840f2f1394092c079` |

Roslyn package versions in the manifest are future analyzer-development pins, separate from the observed compiler. The local coverage tool is pinned, but collection and coverage thresholds are not yet proved.

## Verification and Raw Evidence

These logs establish this plan's compatibility slice. They are explicitly **not complete Phase 1 acceptance reports** before 01-02's trusted runner controls.

| Command/control | Result | Raw evidence |
| --- | --- | --- |
| `node tools/phase1/bootstrap.cjs --probe engine` | Pass; actual construction/ready/exit | [first engine log](../../../.phase1/runs/2a4cedda-4d13-46b9-9a45-8bbcc3a3c5d9/engine.log) |
| Same command with `PHASE1_IGNORE_DOWNLOADS=1` and `PHASE1_FORCE_DOWNLOAD=1` | Pass; official download, restore, locked restore, build, engine | [compatibility identity](../../../.phase1/runs/da785a9f-d9ef-403b-b56e-29b55c15d69e/compatibility.json), [engine log](../../../.phase1/runs/da785a9f-d9ef-403b-b56e-29b55c15d69e/engine.log) |
| Actual missing executable, invalid restore subject, unavailable runtime and child timeout | All four observed expected separate failures | [infrastructure controls](../../../.phase1/runs/da785a9f-d9ef-403b-b56e-29b55c15d69e/infrastructure-controls.json) |
| `node tools/phase1/bootstrap.cjs --probe pins` | Pass; 11 mutation/permutation controls and actual executable identities | [pin controls](../../../.phase1/runs/c7a794d5-c8e1-4fc0-9899-4bdd77aa4b9c/pins-controls.json) |
| `node --check tools/phase1/bootstrap.cjs` and `git diff --check` | Pass | Command exit status observed |
| Package ZIP/nuspec/build asset inspection; `dotnet nuget verify --all` for each package | Exact publisher/revision/hashes; all unsigned with expected NU3004 | [package provenance](../../../.phase1/runs/da785a9f-d9ef-403b-b56e-29b55c15d69e/package-provenance.json), adjacent per-package signature logs |
| `node tools/phase1/bootstrap.cjs --probe boundaries` | Expected non-success `BOUNDARIES_MISSING`: Core is not yet implemented | [explicit failure](../../../.phase1/runs/e056c9df-d15b-4de7-b489-dce397ebf43a/failure.json) |
| Bounded `wsl.exe --list --quiet` discovery | Exit 1, installer/usage output; no working Linux environment established | [discovery output](../../../.phase1/runs/da785a9f-d9ef-403b-b56e-29b55c15d69e/linux-discovery.stdout.log) |

All currently implemented positive bootstrap probes pass. The boundaries command deliberately remains non-success until its declared subjects arrive. All remaining AC-01–AC-22 obligations, C# runner proofs, rules, architecture, lifecycle expansion, coverage and actual Linux execution remain pending their later plans. RULE-01 and RULE-06 are not marked complete by this partial slice.

## Decisions Made

- Keep net10.0 and explicit C#14 because the actual selected engine hosted them successfully; no downgrade or feature suppression was needed.
- Treat exact official archive/package hashes and source metadata as provenance for unsigned release nupkgs. Signature verification explicitly failed with NU3004, as expected, and is not represented as passed authenticity verification.
- Keep the engine lock file and generated runtime/tool outputs isolated under ignored `.phase1`; validate the complete initial closure and repeat locked restore before compilation.
- The orchestrator recorded `git.allow_default_branch_commits:true` because this authorized run intentionally uses the pinned main checkout with `branching_strategy:none`. Every scoped commit used the protected-branch guard and normal hooks under the execution context that can query Git metadata.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Resolve the exact compiler identity and ambiguous engine type name.**
- Found during Task 1's first real compiler/build executions.
- The compiler emits the full source hash, and `Environment` conflicts with `Godot.Environment`.
- Recorded the full compiler identity and qualified `System.Environment`; subsequent actual restore/build/engine execution passed.
- Files: `toolchain.json`, `proofs/Phase1.cs`; commit `70bf15d`.

**2. [Rule 3 - Blocking] Use the authorized execution context for restricted child processes and Git metadata.**
- Default restricted child creation returned EPERM; direct curl TLS also lacked credentials. Authorized scoped escalated commands ran the unchanged official source/verification path successfully. No automatic approval review rejection occurred.
- The inherited broken shell proxy was bypassed by task-local direct HTTPS, without global changes. Exact official SHA-512 confirmed the provided archive before extraction.
- The SDK's Git guard respected the orchestrator's explicit main-branch override when executed in the context that can query repository metadata.

## TDD Gate Compliance

Task 1 carries `tdd="true"`, but owns bootstrap glue, configuration and the initial engine scene before 01-02 provides the C# proof runner. No valid assertion-based RED report or RED commit exists. Missing-tool/compile errors were not misrepresented as RED. The executor reported this conflict to the orchestrator, which confirmed documenting the deviation rather than inventing RED evidence or importing later-plan tests. Actual engine integration and diagnostic controls pass. This deviation is recorded in `.planning/WINDOWS.md`; 01-02 owns the C# mutation proofs.

## Known Stubs and Threat Review

No placeholder, TODO, FIXME or empty UI data stub was introduced. The scene is deliberately the minimal compatibility fixture; expanded lifecycle/state behavior belongs to its owning later plans. Official HTTPS acquisition, file extraction and child-process evidence are already covered by the plan's declared trust boundaries; no additional endpoint/auth/schema surface was added.

## Next Plan Readiness

01-02 can build the trusted C# runner against the observed baseline and extend pin mutations through its registered proofs. Windows compatibility is established for this initial slice. Linux environment acquisition/execution remains a future concrete gate; neither WSL discovery nor published archive metadata is a Linux pass. No gameplay/UI, LAN or coverage claim is made.

## Self-Check: PASSED

All ten implementation artifacts and this summary exist. Both task commits are ancestors of HEAD. No tracked file deletion occurred in either task commit. A stub scan found no source placeholders. Runtime/tool/cache outputs are ignored, and preexisting planning runtime files were preserved.
