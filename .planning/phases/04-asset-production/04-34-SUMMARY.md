---
phase: 04-asset-production
plan: "34"
subsystem: compiler-verification
tags: [dotnet, roslyn, godot, export, source-only, windows]
requires:
  - phase: 04-25
    provides: Existing authenticated content source and publication baseline
provides:
  - Six exact authenticated Windows SDK export source-line permissions
  - Independent export origin and negative-boundary proof identities
  - Saved source-only ExportDebug and ExportRelease managed publish procedure
affects: [04-02, 04-31, ASSET-05]
tech-stack:
  added: []
  patterns: [Exact ordinal source-path and UTF-8 hash permissions, bounded owned compiler copies]
key-files:
  created:
    - tests/phase1/GeneratedExportLayoutTests.cs
    - production/phase4/export-layout-origin.json
    - tools/phase4/cases/export-layout.cjs
  modified:
    - tools/code-rules/GeneratedLayout.cs
    - docs/code/generated-layout-adaptations.json
    - tools/phase1/required-cases.json
    - tools/phase4/check.cjs
key-decisions:
  - Keep export provenance separate from the original four sources and seven member permissions.
  - Permit only six observed Windows owning-project/configuration/path/hash/line tuples; no Linux variant is inferred.
  - Run the SDK layout prerequisite at G0 order 0 before the preserved export-feasibility order 5.
requirements-completed: []
requirements-supported: [ASSET-05]
actuals:
  tokens: 12747
  tasks: 2
  commits: 5
commits: 5
plan_head_before: 142dcc8172161105a7a065a8ff8a30c2eb18a997
plan_head_after: e3ae7c04a1aae7c1ff7b882115865714326397ed
coverage:
  - id: SDK-EXPORT-ORIGIN
    description: Six actual Windows SDK export sources authenticate and receive only exact line compatibility
    requirement: ASSET-05
    verification:
      - kind: integration
        ref: rules.export-layout-origin
        status: pass
    human_judgment: false
  - id: SDK-EXPORT-BOUNDARY
    description: Independent owner/path/configuration/case/bytes/line/producer subjects remain rejected
    requirement: ASSET-05
    verification:
      - kind: integration
        ref: rules.export-layout-boundary
        status: pass
    human_judgment: false
  - id: SDK-EXPORT-COMPILE
    description: Both genuine Windows managed export configurations publish with analyzers and errors enforced
    requirement: ASSET-05
    verification:
      - kind: integration
        ref: export.layout
        status: pass
    human_judgment: false
duration: 20min
started: 2026-11-16T14:02:48Z
completed: 2026-11-16
status: complete
---

# Phase 04 Plan 34: Exact SDK Export Layout Prerequisite Summary

**Authenticated SDK 10.0.404 assembly-attribute lines now compile in both genuine Windows export configurations through six exact path/hash/line permissions, with independent origin and rejection proofs.**

## Performance

- Implementation/evidence window: 20 minutes, beginning with the first capture helper at 14:02:48 UTC; context initialization preceded that measured window.
- Tasks: 2/2. Implementation files: 7. Production/test commits: 5.
- Actual estimate-scale cost: 50,986 characters of realized Git diff / 4, rounded upward to 12,747 tokens. This is not harness token usage.
- The persisted `.git/gsd-plan-head-before-04-34` ledger measures five commits from `142dcc8` through `e3ae7c0`. The earlier `142dcc8` planning commit is the base, not an implementation commit. SUMMARY/progress commits follow that measured range.

## Accomplishments

- Reauthenticated the actual installed producer and observed fresh outputs for Core, Multiplayer.Abstractions and Godot under both `ExportDebug` and `ExportRelease` before granting permissions.
- Preserved `GeneratedLayout.IsPermitted` and its normalization/hash/line algorithm. The original eight tuples remain byte-identical after removing the six new tuples. No global, member, directory or configuration-wide suppression was added.
- Added `rules.export-layout-origin`, `rules.export-layout-boundary` and the source-only `export.layout` saved case. Each new Phase 1 declaration binds a real named C# method, RULE-03, owner 04-34, acceptancePhase 4 and Windows x64 source-only metadata.
- Real self-contained net10.0/win-x64 managed publishes succeed in both genuine export configurations. Actual compiler command lines retain `/analyzer:` with `Sebreiro.CodeRules.Analyzers.dll` and `/warnaserror+`; evaluated policy and the exact analyzer project edge are saved independently.
- Original four source records, seven member records and all 684 previous required-case declarations compare equal to the base. Existing Debug/layout/source controls pass under their original identities.

## Task Commits

| Task | Commit | Result |
| --- | --- | --- |
| 1 RED | `6a484ee` | Failing real origin proof, separate authenticated provenance and declaration |
| 1 GREEN | `d410a48` | Six exact observed export path/hash/line tuples |
| 2 RED | `c679da7` | Exact boundary proof and genuinely failing missing executable saved-case assertion |
| 2 GREEN | `353172c` | Both real publishes, saved compiler case, narrow module registration and evidence inventory |
| 2 ordering refinement | `e3ae7c0` | Prerequisite G0 order 0 precedes preserved exporter order 5; self-test enforces ordering |

Normal commit hooks and analyzers remained enabled. No branch, PR, stash, reset, merge or partial-exporter mutation occurred.

## TDD Gate Compliance

**Task 1 RED:** the new origin target authenticated actual SDK/source bytes, then failed its planned empty-diagnostics assertion for all six real sources. Every diagnostic was SCR0007, Error, line 4, column 121, end line 4, end column 136. The failing C# target was discovered and executed once; this was not a restore/build/fixture failure. Complete unchanged JUnit output and the qualified target record are retained at `.phase4/runs/1909dff3-09ff-40df-b242-fd5e0f31c774/red-evidence-qualified.json`. The classifier returned `RED_EVIDENCE_OK` before the RED commit and production permission edit.

The first evidence record used the short target name and was rejected as `no_target_test_failure`; its report and record remain unchanged. A second record uses the actual fully qualified JUnit name, without changing the report. The first compiler/style failure is retained separately and was never counted as RED.

**Task 1 GREEN:** the real origin case passed after only the six exact tuples were appended. The required automated tracer recheck also passed before expansion.

**Task 2 RED:** a TAP self-test asserted that `export.layout` has an executable Critical G0 source-only procedure. It failed because that case was absent. `.phase4/runs/79a8e1de-9f09-4210-a374-f4ee4c5e6a2d/red-evidence.json` passed `RED_EVIDENCE_OK` before the saved compiler procedure was implemented. The initial default Node reporter was unsupported; that separate unchanged attempt remains at `762b7873-fb4e-4233-99c3-9d8d02c0a31c`. The analyzer boundary controls were independently green because Task 1's exact permission model already rejects those subjects; no artificial negative failure was introduced.

**Task 2 GREEN:** the saved-case TAP test passes, including order 0; both real publish configurations and all required regressions pass. No refactor was necessary. RED commits precede their respective GREEN commits by ancestry.

## Actual Source and Producer Identities

Installed producer: `C:/Program Files/dotnet/sdk/10.0.404/Microsoft.Common.CurrentVersion.targets`, target `GenerateTargetFrameworkMonikerAttribute`, SHA256 `428d17faaf0737fba66feb5063f9a8c0d06a2ece7c42e6cca4a8a65220f80fbe`.

All six actual Windows sources are 200 bytes, strict UTF-8, with raw and decoded-text SHA256 `93d67476196ebfc80d1d7d8844b91146cfd678d105f5ce635eb900ca40610c9b`. Only line 4, length 135 UTF-16 units, is permitted. The fresh source-generation run is `.phase4/runs/81c4c6a6-8981-43b7-ae89-0d557dd8a54c`; source generation alone received no compile credit.

| Stable new row ID | Exact project-relative path |
| --- | --- |
| KZ-EX-SDK-EXPORT-ExportDebug-core | `src/core/obj/ExportDebug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` |
| KZ-EX-SDK-EXPORT-ExportDebug-godot-game | `src/godot-game/.godot/mono/temp/obj/ExportDebug/win-x64/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` |
| KZ-EX-SDK-EXPORT-ExportDebug-multiplayer-abstractions | `src/multiplayer-abstractions/obj/ExportDebug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` |
| KZ-EX-SDK-EXPORT-ExportRelease-core | `src/core/obj/ExportRelease/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` |
| KZ-EX-SDK-EXPORT-ExportRelease-godot-game | `src/godot-game/.godot/mono/temp/obj/ExportRelease/win-x64/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` |
| KZ-EX-SDK-EXPORT-ExportRelease-multiplayer-abstractions | `src/multiplayer-abstractions/obj/ExportRelease/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` |

The analyzer's netstandard2.0 export attribute file received no permission merely because it existed. No Windows source was relabeled as Linux and no Linux hash row was added.

## Verification

| Actual saved case or command | Result and immutable evidence |
| --- | --- |
| `rules.export-layout-origin` | PASS 1/1; `.phase1/runs/249b039e-477c-468e-bccb-ea1231669b92/summary.json` |
| Origin tracer recheck | PASS 1/1; `.phase1/runs/e833a30d-48bb-40de-97ad-3f6f31616662/summary.json` |
| `rules.export-layout-boundary` | PASS 1/1; `.phase1/runs/d7fc7c1e-447c-418f-b41c-c97fcf62357a/summary.json` |
| `rules.generated-layout-origin` | PASS 1/1; `.phase1/runs/f1de3c61-ea4a-4135-b81b-30acdc0d8251/summary.json` |
| `rules.generated-layout-boundary` | PASS 1/1; `.phase1/runs/78ae539a-fbe4-4e81-bd29-7a3cf0ba4c00/summary.json` |
| `export.layout`, final committed source | PASS; `.phase4/runs/d8504bcf-69ab-4a7a-ab5b-5a490143b334/result.json`, revision `e3ae7c04a1aae7c1ff7b882115865714326397ed` |
| Strict normal Debug solution build | PASS, zero warnings/errors; `.phase4/runs/f5cca9e8-3872-4dae-8860-937b807353f5/strict-debug.command.json` |
| `source.tracer` | PASS; `.phase4/runs/d8a06990-e04c-4432-94bf-389f639e3ccc/result.json` |
| `source.tracer-failures` | PASS, eleven deliberate rejected subjects plus restored/production positives; `.phase4/runs/2621f650-4328-4811-adb9-6a6e4d0980f2/result.json` |
| `source.provenance` | PASS; `.phase4/runs/5c76d545-56ef-47f0-9efc-b5129f8becec/result.json` |
| `source.publication` | PASS; `.phase4/runs/2e8b30f2-126f-42a8-bb4f-4462c1eb51ed/result.json` |

The final export invocation saves separate configuration policy, compiler command/stdout/stderr, source identity, analyzer DLL hash, published dependency inventory and runtimeconfig/deps evidence. Earlier successful runs `75e8c8b9-6684-42ad-a95c-bc35a9faee1f` and `4767183a-9a06-4de8-b9f2-3e6fad6491ee` remain independently identified. The provenance record links the former; the final ordering-only source revision is the latter table's `d8504bcf` run. No report or historical result was overwritten.

Boundary coverage independently rejects wrong owning project/root/intermediate path/filename, configuration suffixes, `Exportdebug`/`exportrelease` and other case changes, changed/appended bytes, handwritten lookalikes, adjacent handwritten long lines, moved line positions and wrong producer hash/symbol. Actual analyzer diagnostics require column 121 through the real line end. Reflection separately exercises every actual TextLine and permits only line 4; no arbitrary TextSpan permission API was invented.

## Artifacts This Phase Produces

- `GeneratedLayout.cs`: six entries in existing `_entries`; `IsPermitted` and every original tuple/normalization step are unchanged.
- `GeneratedExportLayoutTests.cs`: class `GeneratedExportLayoutTests`; proof methods `Check_ExportLayouts_HavePinnedOrigin` and `Check_ExportLayoutPermissions_StayBounded`; private helpers `RejectAsync`, `RejectOtherLines`, `RejectProducer`, `Authenticate`, `Analyzer`, `Registry`, `Text`, `Local`, `Hash`.
- `generated-layout-adaptations.json`: separate `exportSources` containing the six row IDs above. New row fields are `id`, `platform`, `sdk`, `owner`, `configuration`, `surface`, `path`, `bytes`, `sha256`, `textSha256`, `line`, `lineLength`, `suffix`, `lines`, `origin`, `originSha256`, `originSymbol`, `diagnostic`, `proofs`, `generation`, `compilerAttempt`. Original `sources`, `members`, IDs and proof labels stay unchanged.
- `required-cases.json`: `rules.export-layout-origin` / `SDK_EXPORT_LAYOUT_ORIGIN_EXACT` and `rules.export-layout-boundary` / `SDK_EXPORT_LAYOUT_BOUNDARY_REJECTED`; each names its actual method, Windows source-only `environment`, and ASSET-05 `supports` metadata without transferring acceptance ownership.
- `export-layout-origin.json`: `schemaVersion`, `scope`, `status`, `originalFailedRun`, `capture`, `producer`, `exportSources`, `originalCollections`, `preservedPartialFiles`, `evidence`, `limits`, `retainedDevelopmentAttempts`, `preservation`. Producer fields bind path/hash/target/SDK. Evidence fields link RED, initial/current GREEN, tracer, boundary, old layout cases, actual configurations, strict Debug, four source regressions and preservation. Development attempts retain all failed compiler/proof/reporter runs, initial sandbox EPERM and the failed pre-write inline quoting attempt. Preservation fields bind base, exact old source/member/case counts, original adapter equality, four partial hashes and check time.
- `export-layout.cjs`: constants `DOTNET`, `REGISTRY`, `ORIGIN`; exported functions `snapshot`, `sourceFile`, `authenticate`, `policy`, `dependencies`, `compile`, `selfTest`; saved `export.layout` case, Critical/G0/order 0/source-only, ASSET-05 prerequisite tags; TAP test `export.layout registers its bounded source-only compiler procedure`.
- `check.cjs`: only the new `require('./cases/export-layout.cjs')` registry seam. Existing source/audio registrations and unfinished export ownership remain unchanged.
- Ignored run-owned evidence includes `invocation.json`, input fingerprints, project-input inventories, producer snapshot, retained/fresh source records, effective policy, bounded command/log records, `compile.json`, `configurations.json`, dependency closure and final result. Children have bounded deadlines and owned descendant cleanup; installed SDK/feed policy and original failed evidence are never rewritten.

## Preserved Partial Export Work

The original failed run `.phase4/runs/a9a37eb6-93a2-4891-b3f2-d442ea7b7a8b` and committed audio Task 2 `ac1de1e` remain historical and unchanged. Initial/final SHA256 comparisons match:

| Untracked parent-owned file | SHA256 |
| --- | --- |
| `tools/phase4/export.cjs` | `f96f77eb05c6f8210cdb527a9c7c6649d1c066e14a33846678c234989dd1a33d` |
| `src/godot-game/export_presets.cfg` | `7fe9294d4a160582af981a9ec00167f8bf8bb407b6e86ce8b4bbde226bed3b00` |
| `production/phase4/export-toolchain.json` | `8bf60b00b0d92dca0ec42b5c73051ab21a4608b24af20db03014f2097eefe114` |
| `tools/phase4/cases/export-feasibility.cjs` | `a05ade543ddb26306c1ed57b97d9a3bbecf30c7bf8b5792db851df42642ceed2` |

`.phase4/04-34-preservation.json` records exact equality of the original four source rows, seven member rows, 684 case declarations and the original analyzer file after removing only the six added tuples.

## Deviations from Plan

None. The small saved-case self-test resides in the owned module. G0 order 0 was selected to preserve the unfinished exporter's existing order 5. Compiler verbosity is normal solely to retain actual analyzer/error-policy command lines; engine, SDK, framework, configurations and compiler policy are unchanged. Development compiler/style/report-format failures were corrected and retained, never converted into RED or acceptance evidence. No additional generated syntax/callsite adaptation was needed.

## Evidence Limits and Next Action

This completes only the existing ASSET-05 SDK layout implementation prerequisite. All ASSET requirements remain pending. Genuine native export and clean portable execution still belong to unfinished 04-02 Task 1; complete resources, native release export, rendered inspection, actual hearing/listening, Linux execution and final phase acceptance remain with their original owners. No game window or audio playback/listening was performed here.

Parent can resume the preserved 04-02 Task 1 on a fresh invocation. Do not redo its committed independent audio task. Branch remains `feature/04-asset-production`; parent retains draft PR #5 to `dev` and phase acceptance/checkpoint reconciliation.

## Self-Check: PASSED

All seven implementation files exist. All five listed implementation/test commits are ancestors of HEAD. The SDK evaluation-scope query resolves those commits and the separate earlier planning commit; measured actuals exclude that planning base. Every required automated plan command ran successfully. No required verification was skipped, no tracked file was deleted and no unfinished UI/data stub was introduced. Security-relevant local SDK/source/file boundaries remain within the plan's T-04-100/101/102/SC model.
