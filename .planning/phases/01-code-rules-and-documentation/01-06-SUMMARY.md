---
phase: 01-code-rules-and-documentation
plan: 06
subsystem: testing
tags: [roslyn, analyzers, nullability, body-size, utf16, member-order]
requires:
  - phase: 01-05
    provides: Four inherited analyzers, exact Roslyn verifier and bounded generated-call-site proofs
provides:
  - SCR0005-SCR0008 error enforcement with captured subjects and exact diagnostic spans
  - Real independent R-37 59/60/61 proofs and UTF-16 ASCII/Cyrillic/surrogate/combining boundaries
  - Exact generated-layout origin and content/path/adjacent boundaries
  - Actual analyzer-source consumer compilation under all eight implemented analyzers
affects: [01-07, 01-08, 01-14, 01-15, 01-16, 01-17]
tech-stack:
  added: []
  patterns: [Captured Roslyn analyzers, immutable source-line permissions, parsed constant adapter targets]
key-files:
  created: [tools/code-rules/NullForgivingAnalyzer.cs, tools/code-rules/BodyLengthAnalyzer.cs, tools/code-rules/LineLengthAnalyzer.cs, tools/code-rules/MemberOrderAnalyzer.cs, tools/code-rules/GeneratedLayout.cs, tests/phase1/Analyzers/NullForgivingAnalyzerTests.cs, tests/phase1/Analyzers/BodyLengthAnalyzerTests.cs, tests/phase1/Analyzers/LineLengthAnalyzerTests.cs, tests/phase1/Analyzers/MemberOrderAnalyzerTests.cs, tests/phase1/GeneratedLayoutAdapterTests.cs, tests/phase1/AnalyzerSourceConsumerTests.cs, tests/phase1/Verification/AdapterTargets.cs, docs/code/generated-layout-adaptations.json]
  modified: [tests/phase1/Verification/AnalyzerVerifier.cs, tests/phase1/Verification/InheritedAnalyzerVerifier.cs, tests/phase1/ConfigurationTests.cs, tests/phase1/RegisterTests.cs, tools/phase1/required-cases.json, docs/code/provenance/analyzer-adoption.json]
key-decisions:
  - Preserve donor descriptors, physical-body counting, inclusive 60-line/120-UTF16 maxima and member ranking; 40 lines remains advice.
  - Generated markers grant no exemption; four immutable source/path/line entries and seven exact diagnostic/member pairs require origin and boundary proofs.
  - Correct diagnosed handwritten source and compile-only subjects; retain error severities and existing SCR0001/RS0030 scopes.
requirements-completed: [RULE-02, RULE-03]
requirements-status: Pending shared phase acceptance; no RULE requirement is completed by this plan alone
coverage:
  - id: D1
    description: Null suppression and member-body enforcement with exact physical-line boundaries
    verification:
      - kind: integration
        ref: .phase1/runs/8cfc852a-4508-4d19-a179-cf98c219aa1c/summary.json
        status: pass
    human_judgment: false
  - id: D2
    description: UTF-16 line boundaries and captured member-order semantics
    verification:
      - kind: integration
        ref: tests/phase1/Analyzers/LineLengthAnalyzerTests.cs; tests/phase1/Analyzers/MemberOrderAnalyzerTests.cs
        status: pass
    human_judgment: false
  - id: D3
    description: Generated layout origin/boundaries, strict subject decoding and actual analyzer-source consumer
    verification:
      - kind: integration
        ref: rules.generated-layout-origin; rules.generated-layout-boundary; rules.line.invalid-utf8; rules.analyzer-source-consumer
        status: pass
    human_judgment: false
actuals:
  tokens: 117258
  tasks: 2
  commits: 4
plan_head_before: 6cf7e7081a3dfcfa3b9ebeb34722234a58f6fdca
plan_head_after: be22525568dd237fe4d612b7efcf3a577ef31a65
duration: 44min
completed: 2026-11-13
status: complete
---

# Phase 1 Plan 6: Null, Body Size and Source Layout Summary

**Four captured analyzers now reject null suppression, bodies over 60 physical lines, lines over 120 UTF-16 units and members outside the inherited access/kind order at exact error spans.**

## Performance

- Observed implementation and verification window: 44min, beginning at 2026-11-13T19:32:14Z.
- Tasks: 2 complete; 41 production/proof/configuration/provenance files changed.
- Actual tokens are chars/4 of the realized Git diff, rounded upward. Four task commits are measured from the persisted ledger; metadata commits are separate.

## Task Commits

| Task | Name | Commit | Type |
| --- | --- | --- | --- |
| 1 | Null suppression and body-length RED subjects | 078a4af | test |
| 1 | Null/body enforcement and diagnosed prerequisites | 22e89c7 | feat |
| 2 | Line-length and member-order RED subjects | 501ee23 | test |
| 2 | Line/order enforcement and bounded integration prerequisites | be22525 | feat |

No tracked files were deleted. No dependency version was added. Captured donor originals remain unchanged; adoption changes are recorded in `docs/code/provenance/analyzer-adoption.json`.

## Verification

- Final command: `node tools/phase1/check.cjs --all`.
- Final run: `.phase1/runs/8cfc852a-4508-4d19-a179-cf98c219aa1c/summary.json` — **209 passed, 0 failed, 192 blocked pending, 401 declared**. Exit 1 reflects pending phase obligations; it is not a passing aggregate `--all` verdict.
- Final input identity: `4052e63511f54551402cd607d5e5b17d23f6f2b232a49004f0f50aa144732471`. Fresh unique results, required evidence and unchanged approved inputs reconcile.
- The plan adds 69 actual passes to the prior 140. Fifty captured subject patterns are retained or explicitly adapted; supplements include independent `rules.body.59/.60/.61`, zero/empty bodies, 40-line advice, and 119/120/121 ASCII, Cyrillic, surrogate-pair and combining-sequence cases. CRLF terminators are excluded. Local functions, expression/lambda/global counting, default access, explicit interfaces, partial declarations and extension blocks retain their captured semantics.
- Focused raw suites passed **19/19 LineLength** and **22/22 MemberOrder** tests. Final reconciliation independently executes their exact manifest bindings and all null/body cases. ID, Error severity, file/start/end span and zero unexpected compiler warnings/errors are required; two captured ordering cases also assert their exact invariant messages.
- Generated layout uses four exact immutable source-byte/path/line entries and seven diagnostic/member pairs across six existing generated members. Real Roslyn original/mutated-byte/wrong-path proofs, adjacent actual-build negatives, pinned SDK/package hashes and existing exact-symbol collision controls pass. Original ten SCR0001 scopes and the Godot startup RS0030 scope retain their exact values. No folder/type-wide exemption was added.
- `rules.analyzer-source-consumer` builds every owned analyzer source as a separate netstandard2.0 consumer under all eight implemented analyzers. Test compilation and the normal Godot build pass with zero warnings/errors. The origin case compiles the test project without copying over its executing DLL.
- Strict UTF-8 proof-subject decoding rejects overlong encodings, encoded surrogates and truncated scalars before parsing. This does not claim completion of the later repository-wide source-format gate.
- `git diff --check` passed. Windows execution only; Linux execution, numeric coverage gates and whole-phase acceptance remain pending.

## TDD Gate Compliance

The following genuine JUnit reports failed the intended exact-diagnostic assertion after successful build/reference resolution, with an empty actual diagnostic set before implementation. Each final record passed `check tdd-red-evidence ... --raw` with `RED_EVIDENCE_OK` and semantic inspection. Reports and records remain under `.phase1/red-01-06/`.

| Rule | RED target | Intended failure |
| --- | --- | --- |
| R-26 | AnalyzeVariableReports | Missing SCR0005 at suppression operator |
| R-37 | Method61LinesFails | Missing SCR0006 at method identifier |
| R-10 | Line121CharactersFails | Missing SCR0007 at UTF-16 unit 121 |
| R-33 | AnalyzePrivateFieldBeforePublicMethodReports | Missing SCR0008 at out-of-order method |

Initial missing-root, failed-build and zero-discovery attempts are not credited as RED. The historical bootstrap missing-RED ledger entry remains open.

## Deviations from Plan

1. **[Rule 3 — Selector mismatch]** Both authored `rules.family6.1` and `rules.family6.2` commands return `SELECTION_EMPTY`. Exact existing rule obligations and captured/boundary case IDs supply the actual proofs; no alias, duplicate result or passing selector was invented. WINDOWS entry 15 is fixed.
2. **[Rule 3 — Diagnosed integration prerequisites]** New enforcement exposed handwritten null suppression, long source and member ordering in existing proofs and the minimal Core/Godot fixtures. Explicit null checks, value-preserving string wrapping and member reordering correct those diagnostics. Compile-only subjects are formatted, and adapter targets are read as exact constant strings after wrapping. The pending-register control now requires R-37 proofs to exist while R-09 remains pending. No severity, approved requirement or source threshold was lowered. Commits: 22e89c7, be22525; WINDOWS entry 16 is fixed.
3. **[Rule 2 — Bounded generated layout]** Actual SDK/xUnit/Godot diagnostics require the precisely registered layout permissions above. The analyzer-source consumer additionally exposed the SDK netstandard attribute. Every permission has origin and boundary evidence; changed content or adjacent handwritten code remains rejected. Commit: be22525; WINDOWS entry 17 is fixed.
4. **[Rule 3 — Windows output locks]** In-memory analyzer loads now read assembly bytes so consumer builds do not hold the analyzer output open. The origin test invokes the test project's Compile target instead of overwriting its executing DLL. Commit: be22525; WINDOWS entry 18 is fixed.

Intermediate full run `7072d27d` exposed the stale R-37 pending expectation; `4485e751` passed 208 controls and exposed the self-DLL copy lock. An interrupted run and its inspected stale lock supplied no acceptance evidence. The final run above supersedes them after the demonstrated corrections.

## Remaining Phase Work

- No current-plan stub, skipped test or unrun actual proof remains. WINDOW entry 8 stays open for the four analyzer families owned by 01-07; continuation offsets/forms belong there.
- Five distinct runner-edge obligations retain their declared owner and 01-14 closure assignment. WINDOWS entry 9 remains open.
- Register activation/wider exemptions, graph/state/lifecycle/docs, actual coverage and Linux proofs retain their planned owners. All RULE requirements remain Pending. Prepared Linux VM processes were preserved; environment preparation is not acceptance evidence.
- Next plan: **01-07**. Phase progress: **7/18**.

## Self-Check: PASSED

All thirteen created artifacts exist; all four task commits are ancestors of HEAD. No unexpected tracked deletion, current-plan stub or unmodeled network/auth/schema boundary was found. Final fresh reconciliation has zero failed implemented obligations.
