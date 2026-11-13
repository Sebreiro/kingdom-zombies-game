# 01-08 execution checkpoint

Status: incomplete; Task 1 tracer proven, Task 1 expansion and Task 2 pending. No SUMMARY.md exists and no plan/requirement progress was advanced.

## Latest continuation: 21 real-build rule pairs verified

| Increment | Commit | Verified result |
| --- | --- | --- |
| Active inherited policy and compliant proof sources | `dd59a77` | Complete run: 375 passed, zero failed controls, 184 pending |
| 42 unique actual-build proofs for 21 rules | `f4d4d79` | Complete run: 417 passed, zero failed controls, 142 pending |

The new separately bound compliant/negative cases cover R-01, R-11, R-12, R-13, R-15, R-16, R-17, R-19, R-20, R-21, R-22, R-27, R-29, R-31, R-32, R-35, R-39, R-40, R-41, R-42 and R-45. Each restores the actual copied Core consumer with its canonical lock, requires a clean build, then checks its own source. Negatives require the intended compiler/analyzer ID, `Subject.cs`, and exact line/column. Case IDs remain stable; all 42 new bindings are distinct C# methods. Raw child compiler logs and source remain under their unique ignored `.phase1/temp/build-*` directories.

Focused class feedback genuinely executed 43 cases: 41 passed and two failed. R-22 expected IDE0022 at the wrong column; actual arrow location is `(6,32)`. R-19's non-apparent return expression produced no IDE0090, so its paired fixtures now use an explicitly typed field initialized with `new()` versus `new object()`; actual negative is IDE0090 at `(11,45)`. Canonical focused corrected reports: `.phase1/runs/3235c7b4-9d68-4e62-91cc-a16c5a0a9e67/summary.json`, `.phase1/runs/5111130c-5d0c-444d-bf53-2b71a2b611d5/summary.json`, and `.phase1/runs/da83118e-1762-4c7d-ab67-9af6ff349a99/summary.json`. Earlier unsupported reporter and missing-root launcher attempts were infrastructure failures, never RED evidence or passing rule proofs.

Final strict solution build has zero warnings/errors: `.phase1/temp/08-expand-final-build.log`. Complete immutable-source `node tools/phase1/check.cjs --all` finished with **417 passed, zero failed controls, 142 explicitly pending, 559 declared**, actual exit **1** solely for pending obligations. Canonical report: `.phase1/runs/0d359120-54ec-4e77-a1d1-c5e3587c7f1a/summary.json`; raw console: `.phase1/temp/08-expand-all.log`; tested revision `dd59a7746808af2a83f89b8766bc5ad96c260682`; input identity `7a6089c8b8fd11cb22603acd8dd92e61754449aea33fe2ec33fb153fbdb4ee24`. `git diff --check` passes. Do not redundantly rerun this passing increment without a new change or concern.

The original ignored `08-expand.cjs` remains unrun. Its inspected copy `08-expand-stage.cjs` ran exactly once to create these cases and was then corrected through actual compiler feedback; do not rerun it over the existing methods.

Task 1 still requires source-format/formatting and permission/test-project/configuration controls, including retained R-48 syntax scanning plus meaningful test-method and manifest/proof-reference renames without changing stable case IDs. Task 2's global suppression/exemption/generated provenance and numeric contracts remain incomplete. Graph 01-09 and actual coverage 01-15 stay pending. No SUMMARY, task completion, requirement completion or advancement beyond 8/18 is claimed.

A read-only preliminary format scan found CRLF in exactly the five canonical lock files. The remaining source-format work must enforce LF and restrict the final-newline exception to observed registered exact paths. It must prove clean locked restore preserves normalized canonical bytes rather than silently extending the exception to CRLF. Captured donor `SourceFileFormat.cs`, `TestSourceIndex.cs`, and `TestMethodNameTests.cs` were read from the immutable ZIP; they are reference inputs, not new implemented proofs.

## Previous continuation: inherited policy activation verified

The complete preserved 27-file policy/source increment is now verified under the active inherited policy. All three earlier activation regressions are fixed without changing rule severities, generated permissions, or captured sample content:

- `pins.async-permission`: the clean `Read()` fixture now uses a block body; its intended MA0137 configuration mutation still fails. Fresh focused report: `.phase1/runs/c56e69a3-3aaf-412c-bb16-0f02bc6f9a15/summary.json`.
- `rules.analyzer-source-consumer`: a tuple alias shortens the generated-syntax table declaration so ordinary collection layout satisfies both formatting and SCR0009. Every exact path/hash/line/column/length value remains intact. Focused report: `.phase1/runs/c0a093c2-fe03-497e-80ff-4229d48c9be8/summary.json`.
- `rules.continuationindent.AgreedSamples_FormatterLeavesUnchanged_AndNotReported`: standalone emission appends an absent LF and compares the complete emitted file exactly. The captured `AgreedSamples` constant is unchanged. Focused report: `.phase1/runs/9d6b18ff-85b4-473a-aed4-444dcab8ef52/summary.json`.

Final strict five-project solution build: `dotnet build KingdomZombies.slnx --no-restore -p:NuGetAudit=false -p:UseSharedCompilation=false -nodeReuse:false`, **zero warnings and zero errors**, `.phase1/temp/08-reconcile-final-baseline.log`. Actual twelve-analyzer source consumer passes. Native `node tools/phase1/bootstrap.cjs --probe engine` passes with fresh construction/ready/exit assertions; compatibility-only evidence is `.phase1/runs/c8fb6afc-9219-4a9b-8fa4-53debc8dd993/compatibility.json`. This is bootstrap evidence, not the later lifecycle/exports/thread acceptance.

Exact generated syntax origin and boundary controls pass separately: `.phase1/runs/86d4acf5-14d3-45fd-8a00-dd523b7f2003/summary.json` and `.phase1/runs/cf4d56bd-0abb-48e2-897e-8b3ddddb53c3/summary.json`.

The complete `node tools/phase1/check.cjs --all` run finished with **375 passed, zero failed controls, 184 explicitly pending, 559 declared**, actual exit **1** solely for pending obligations. Canonical report: `.phase1/runs/9ab3b049-7ae7-4ef5-9e84-9e4587ad491b/summary.json`; raw console: `.phase1/temp/08-reconcile-all.log`. Tested revision: `559b0c0e4951a01b0799b60929111bfae120796e`. All source inputs stayed unchanged throughout the run. `git diff --check` passes.

This is an accepted activation increment, not Task 1 or Task 2 completion. Continue the required real-build/source-format/configuration expansion and exact exemption/generated-surface/numeric controls below. The ignored `08-expand.cjs` remains an unrun, unverified draft and must not be assumed valid. Do not rerun the one-shot `Continue08.cs`. No SUMMARY, requirement completion, or advancement beyond 8/18 is authorized by these results.

## Previous continuation: strict solution green; three activation regressions

All eight inherited prerequisite diagnostics are corrected. The complete five-project solution now builds with **zero warnings and zero errors** under the fully active inherited policy. Actual command: `dotnet build KingdomZombies.slnx --no-restore -p:NuGetAudit=false -p:UseSharedCompilation=false -nodeReuse:false`; raw output: `.phase1/temp/08-finish-baseline3.log`. Three bounded feedback passes produced eight → three → two → zero errors. No severity, analyzer edge, generated identity or exemption scope was weakened.

The corrections preserve `source:` named-argument spelling and the PascalCase `Source` constant; extract the generator suffix without changing it; split `RunnerProcessFixture` setup into `CopyFixtureInputs` and `WriteFixtureManifest`, leaving constructor-only get-property assignment intact; and construct the register heading/source-ID array with compatible formatting. All 85 IDs, 84 headings and 24 source exemptions remain unchanged.

The complete existing suite **finished**, actual command `node tools/phase1/check.cjs --all`, exit **1**. Final totals: **372 passed, 3 failed, 184 pending, 559 declared**. Canonical report: `.phase1/runs/66b8b9b1-e366-4dd3-a7ad-ebbe96025e23/summary.json`; console output: `.phase1/temp/08-finish-prerequisite-all.log`. Revision `7f58ecb38b106598b567329ca88c5ed89de926fc`; input fingerprint `ce613dda91f95ad5c715793166a98060c9ba867b28077ee5866a533497c940a1`. No source-changed failure occurred; the run is intact. The new real-build tracer adds one declared/implemented case to the historical 558-case baseline: `rules.build-wiring` **passes**, diagnostic `REAL_BUILD_SCR0006`, exact scratch `src/core/Subject.cs(6,25)`. Its successful locked restore and clean build precede the intended rejection.

`pins.async-permission` / `ConfigurationTests.AsyncSuffixPermissionRejectsConfigurationDrift` fails its **clean control** because its expression-bodied `Read()` now triggers `Subject.cs(6,54): error IDE0022`. This is not the intended MA0137 negative and cannot count as a pass. The exact fresh child report/log are under that run's `pins.async-permission/` directory.

The next executor must change this isolated clean fixture to a block-bodied method returning `System.Threading.Tasks.Task.CompletedTask`, preserving the no-Async-suffix behavior, then rerun that exact case. Do not change the rule or globally reformat fixtures. The current executor reached its three-repair limit while fixing the inherited prerequisites and cannot perform a fourth repair in Task 1.

Two further independent failures were collected in the same complete run:

| Case | Actual failure | Bounded continuation work |
| --- | --- | --- |
| `rules.analyzer-source-consumer` | `tools/code-rules/GeneratedSyntax.cs(37,5): error SCR0009`: collection bracket indented 4 spaces, expected 12 | Reconcile the `_spans` collection expression layout with the active formatter and custom continuation rule. Preserve every exact suffix, hash, line, column and length value. The ordinary analyzer project excludes its own analyzer edge, so the green whole-solution build does not replace this proof. |
| `rules.continuationindent.AgreedSamples_FormatterLeavesUnchanged_AndNotReported` | formatter exit 2; scratch `Sample.cs(103,2): error FINALNEWLINE`: insert LF | Preserve the captured `AgreedSamples` constant. Adapt only emission/comparison of a standalone scratch source file to its now-active final-newline contract, then prove formatter leaves that complete file unchanged and the analyzer accepts it. |

These are clean-control prerequisites, not valid intended-negative rule evidence. No further repair was attempted after the three-pass limit.

`git diff --check` passes. No native Godot/lifecycle or Linux acceptance is claimed by this continuation. No package installation, generated permission change or donor-file mutation occurred.

The GeneratedSyntax bracket is ordinary formatter-compatible layout in the current green solution, but its actual self-consumer proves the inherited continuation check rejects that C# collection-expression shape. A general language incompatibility is **not established** by this single failure. Prefer a source structure accepted by both rules. If investigation proves a real formatter/language conflict, any narrow adaptation needs its own corpus and regression proofs; no blanket skip or severity change is authorized.

All source WIP remains uncommitted and preserved. `.phase1/temp/08-finish-preserved.patch` contains the exact current binary diff; do **not** apply it over the already edited checkout. The ignored `.phase1/temp/08-expand.cjs` is an **UNRUN draft** generator for 21 separate positive/negative actual-compiler rule pairs. It writes tracked sources/manifest if run, and is not verified or accepted work. Inspect it before use; some diagnostic/line expectations may need real-build feedback. It does not address all Task 1/Task 2 obligations. Never rerun the older one-shot `Continue08.cs` utility.

The captured donor R-48 also requires `Method_State_Expected` syntax scanning; many existing authored methods currently lack that shape. Its retained disposition must be honored through deliberate source/manifest binding changes and proofs, not weakened. Broader compiler/configuration/exemption/generated-origin/threshold work remains required below. No SUMMARY, task completion, plan advancement or requirement completion is claimed.

## Previous continuation: active policy retained, eight diagnostics

The approved fresh continuation restored `08-policy-activation.patch` exactly once after `git apply --check`. The inherited policy is **currently active in the working tree**. Do not reapply that original patch, restore these sources, or mistake the earlier restored-baseline build for current acceptance.

Three bounded strict-build feedback cycles reduced the saved 170 diagnostics to eight. Latest command: `dotnet build KingdomZombies.slnx --no-restore -p:NuGetAudit=false -p:UseSharedCompilation=false -nodeReuse:false`; actual exit 1. The analyzer project passes; the proof project is reached. Exact raw output: `.phase1/temp/08-continue-third.log`; unique diagnostic lines: `.phase1/temp/08-continue-third-errors.txt`; machine-readable counts and 27-file inventory: `.phase1/temp/08-continue-remaining.json`.

| Diagnostic | Count | Exact remaining work |
| --- | --- | --- |
| CS1739 | 4 | `ConfigurationTests.cs` lines 74, 102, 106, 109: restore the external named argument spelling `source:`. The renamed local constant `Source` remains PascalCase. |
| SCR0006 | 1 | `RunnerProcessFixture.cs(145,13)`: the private `RunnerProcessFixture(string mode)` constructor now occupies 79 body lines. Extract independent fixture setup work without changing mode/result semantics. |
| SCR0007 | 1 | `ConfigurationTests.cs(382,121)`: the generator DLL suffix assertion is 176 UTF-16 units. Extract its suffix or predicate into a short local/helper and preserve the exact string value. |
| SCR0009 | 2 | `RegisterValidator.cs(650,25)` and `(652,33)`: wrapped arguments in `CaptureHeadings` both require 28 spaces. Prefer a local JSON source-ID value over further nested wrapping. |

Owned source WIP is deliberately **uncommitted and preserved**. A complete binary diff against committed HEAD is `.phase1/temp/08-continue-preserved.patch`; it was captured before this documentation update. `git diff --check` passes. Do not apply the saved diff over the already edited checkout. The ignored `.phase1/refactor06/Continue08.cs` helper records the syntax-based reflow/extraction approach, but it is a one-shot refactoring utility, **not safe to rerun**: mappings were already extracted and fixed-index extraction must not run twice.

Changes include private-field symbol/reference renames, collection-expression fixes, literal-preserving syntax reflow, explicit ordinal-comparer calls, and extraction of `ValidateEffectiveProperties`, `VerifyChangedDependencyLock`, `ValidateNumericDescription`, `ValidateDispositionInventories`, `ValidateLocalExemptions`, and `ValidateRuleDisposition`. The exact generated layout/syntax hash/path/span values were retained. No diagnostic severity was weakened and no package, gameplay, branch, or later-plan capability was added. The default restricted branch query cannot verify metadata and returns fail-closed; use the authorized elevated guard with the existing default-branch override before the documentation commit.

The repair limit requires a fresh executor before further fixes. No selected proof, native Godot, or full `--all` run was rerun against this failing WIP. Task 1 expansion and all Task 2 work below remain pending. Phase progress remains 8/18 and all RULE requirements remain Pending. A completed SUMMARY must wait for both tasks and their actual proof runs.

## Committed work

| Increment | Commit | Result |
| --- | --- | --- |
| Task 1 RED | b139948 | Compiling isolated real Core build test; actual expected recognizer assertion fails after successful locked restore and clean build |
| Task 1 tracer GREEN | 3b45318 | Exact error ID, Subject.cs and start coordinates required; infrastructure diagnostics cannot satisfy the recognizer |

The fixture copies the actual Core project, owned analyzer inputs, shared policy and canonical locks under `.phase1/temp/build-*`. It uses argument arrays, a controlled working directory, redirected output, no shared compiler/build servers/node reuse, stripped MSBuild/profiler settings, a 60-second child deadline and process-tree cleanup. Scratch inputs and raw process logs remain ignored local outputs.

## Proven evidence

- Genuine RED record: `.phase1/temp/08-red.json`; original JUnit `.phase1/temp/08-red.xml`. The final correctly configured invocation executed the intended test and failed its Shouldly assertion despite actual `Subject.cs(6,25): error SCR0006`. The classifier returned `RED_EVIDENCE_OK`; semantic inspection confirms successful locked restore and clean compilation preceded the intended assertion. Earlier incorrect reporter/root invocations are infrastructure failures and provide no RED credit.
- First reconciled GREEN: `.phase1/runs/4b887617-de74-4cb2-a83c-830bd63e85c5/summary.json`, 1/1 pass.
- Mandatory tracer repeat: `.phase1/runs/86e1c908-f3fc-4df8-aa98-1b651bb127e0/summary.json`, 1/1 pass.
- Final restored-baseline repeat: `.phase1/runs/bcb53ffd-3858-4f35-b774-7d34c1e1e368/summary.json`, 1/1 pass.
- Final whole five-project solution: `.phase1/temp/08-safe-solution.log`, zero warnings/errors. `git diff --check` passes.
- Full `--all` was not rerun for this incomplete checkpoint. Prior 374/558 results are historical; current complete-suite acceptance is not claimed. WINDOWS entry 23 records the unfinished verification.

## Why execution stopped

Activating the captured donor `.editorconfig` exposes real prerequisites in previously authored proof code. Three bounded repair passes did not reconcile the inherited formatter with all twelve custom rules. The executor's three-attempt limit requires stopping instead of another repair/build cycle.

Initial analyzer-project count: 15 distinct errors, IDE1006=2, IDE0011=3, IDE0008=4, IDE0055=6. Once the analyzer project passed, the proof project exposed 236 distinct errors. The final attempted activation leaves **170 distinct errors**: IDE0055=63, IDE1006=10, IDE0300=2, IDE0305=6, SCR0009=80, SCR0006=5, SCR0007=4.

Exact final diagnostics: `.phase1/temp/08-remaining.json`; raw log `.phase1/temp/08-policy-third.log`. No warning/severity was lowered to pass these checks.

## Safe continuation inputs

- `.phase1/temp/08-policy-activation.patch` preserves all uncommitted activation/formatter changes; `.phase1/temp/08-policy-activation-files.json` lists its 27 owned files.
- Only these uncommitted edits were restored to committed `3b45318`, leaving the checkout buildable with the preceding enforced policy. Donor originals, locks, adapter scopes and exact generated identities remain unchanged. The activation patch is a failed candidate, not an accepted implementation.
- The captured donor settings are also in `.phase1/temp/donor.editorconfig` and immutable `docs/code/provenance/langproj-baseline.zip`.
- Generic `dotnet format` can conflict with SCR0009's exact eight-space continuation rule. Apply layout changes deliberately. Do not reuse old line-number diagnostic fixes after wrapping changes line numbers.
- Remaining oversized methods in the candidate: `ConfigurationTests.ValidateConfiguration`, `RegisterValidator.ValidateCapture`, `ValidateMappings`, `ValidateSourceRule`, and `ValidateProof`; use the exact source locations in the saved JSON to identify their current names before refactoring. Private static field names also need `_camelCase` without changing literals or register identities.

## Required remaining work

Task 1 still needs actual compiler/built-in/Meziantou/Roslynator/banned-API/test-policy and import/analyzer-edge controls, complete R-01 through R-85 mechanism binding within this plan's ownership, source encoding/layout/format checks, and a clean strict activated-policy solution. Graph/state/docs/coverage mechanisms keep their later owners and pending status.

Task 2 is not started: suppression/exemption validators, exact generated provenance/scopes and collision controls, donor exemption dispositions, numeric boundary contracts and genuine Godot handwritten/generated controls still require implementation and executed proofs. No coverage measurement or Linux acceptance is claimed.

Resume 01-08 Task 1 expansion with a fresh bounded correction window. Keep phase progress at 8/18 until both tasks pass; then create the actual complete SUMMARY and advance to 01-09.
