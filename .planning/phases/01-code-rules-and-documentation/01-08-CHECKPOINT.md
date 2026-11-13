# 01-08 execution checkpoint

Status: incomplete; Task 1 tracer proven, Task 1 expansion and Task 2 pending. No SUMMARY.md exists and no plan/requirement progress was advanced.

## Latest continuation: active policy retained, eight diagnostics

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
