---
phase: 01-code-rules-and-documentation
plan: 05
subsystem: testing
tags: [roslyn, analyzers, named-arguments, comments, constructors, directives]
requires:
  - phase: 01-18
    provides: Exact compiler verifier, strict evaluated configuration and canonical dependency locks
provides:
  - Four real inherited error analyzers enforcing R-38, R-36, R-23, R-24 and R-25
  - Every captured applicable subject plus explicit params/ref, implicit collection construction, block XML/anonymous method, generated/nested and inactive-region boundaries
  - Ten exact generated-member call-site adaptations with pinned package/source origin and adjacent/collision proofs
  - Independent analyzer-source input identity with isolated mutation and stale-evidence rejection
affects: [01-06, 01-07, 01-08, 01-14, 01-15, 01-17]
tech-stack:
  added: []
  patterns: [Captured local Roslyn analyzers, exact markup-to-span expectations, member-scoped generated compatibility]
key-files:
  created: [tools/code-rules/NamedArgumentAnalyzer.cs, tools/code-rules/BodyCommentAnalyzer.cs, tools/code-rules/PrimaryConstructorAnalyzer.cs, tools/code-rules/RegionDirectiveAnalyzer.cs, tools/code-rules/MemberBodies.cs, tests/phase1/Verification/InheritedAnalyzerVerifier.cs, tests/phase1/Analyzers/NamedArgumentAnalyzerTests.cs, tests/phase1/Analyzers/BodyCommentAnalyzerTests.cs, tests/phase1/Analyzers/PrimaryConstructorAnalyzerTests.cs, tests/phase1/Analyzers/RegionDirectiveAnalyzerTests.cs, tests/phase1/GeneratedCallSiteAdapterTests.cs, tests/phase1/GodotCallSiteAdapterTests.cs, tests/phase1/AnalyzerInputIdentityTests.cs, docs/code/generated-callsite-adaptations.json, docs/code/provenance/analyzer-adoption.json]
  modified: [tests/phase1/Verification/AnalyzerVerifier.cs, tools/phase1/required-cases.json, src/godot-game/proofs/Phase1.cs, tests/phase1/ConfigurationTests.cs]
key-decisions:
  - Preserve exact donor descriptors and applicable syntax semantics; generated markers/settings do not exempt handwritten subjects.
  - Exempt only ten observed pinned generated call-site members, with per-member source/package provenance and collision boundaries; retain the original Godot RS0030 adaptation unchanged.
  - Include owned analyzer source in independent input identity; mutate only an isolated real-source copy to prove stale-evidence rejection.
requirements-completed: [RULE-02, RULE-03]
requirements-status: Pending shared phase acceptance; this plan contributes proofs without completing any RULE requirement
coverage:
  - id: D1
    description: NamedArgument and BodyComment inherited behavior and exact diagnostics
    verification:
      - kind: integration
        ref: .phase1/runs/844f25c7-d928-484b-8a2e-3ef59037f86d/summary.json
        status: pass
    human_judgment: false
  - id: D2
    description: PrimaryConstructor and RegionDirective inherited behavior and exact diagnostics
    verification:
      - kind: integration
        ref: tests/phase1/Analyzers/PrimaryConstructorAnalyzerTests.cs; tests/phase1/Analyzers/RegionDirectiveAnalyzerTests.cs
        status: pass
    human_judgment: false
  - id: D3
    description: Exact generated origin, adjacent/collision boundaries and analyzer input identity
    verification:
      - kind: integration
        ref: rules.generated-callsite-origin; rules.generated-callsite-boundary; rules.godot-callsite-boundary; rules.analyzer-input-identity
        status: pass
    human_judgment: false
actuals:
  tokens: 82208
  tasks: 2
  commits: 4
plan_head_before: b7383c5b551f54d4d21edf8c5dd30099a87f46eb
plan_head_after: 2b0621d1b7e360b80b9b1d57757cb0684bb9d890
duration: 25min
completed: 2026-11-13
status: complete
---

# Phase 1 Plan 5: Inherited Constructor, Directive and Call-Site Rules Summary

**Four captured Roslyn analyzers now reject real call-site, body-comment, primary-constructor, positional-record and region violations at exact error spans, while compliant subjects pass.**

## Performance

- Observed implementation window: 25min, from first on-disk task preparation at 2026-11-13T18:59:10Z to task commit at 19:24Z; context loading preceded that window.
- Tasks: 2 complete. Changed production/proof/configuration files: 30.
- Actual tokens use chars/4 of the realized Git diff, rounded upward; commits are measured from the persisted per-plan ledger. Metadata commits are separate.

## Task Commits

| Task | Name | Commit | Type |
| --- | --- | --- | --- |
| 1 | NamedArgument/BodyComment RED subjects | 03d1495 | test |
| 1 | NamedArgument/BodyComment enforcement and prerequisites | 0adbf7b | feat |
| 2 | Constructor/record/region RED subjects | 85257b9 | test |
| 2 | Constructor/region enforcement and verified integration prerequisites | 2b0621d | feat |

No tracked files were deleted. Donor originals remain unchanged in the captured archive; adoption changes are recorded in `docs/code/provenance/analyzer-adoption.json`. No new dependency was installed.

## Verification

- Final command: `node tools/phase1/check.cjs --all`.
- Final fresh run: `.phase1/runs/844f25c7-d928-484b-8a2e-3ef59037f86d/summary.json` — **140 passed, 0 failed, 203 blocked pending, 343 declared**, exit 1. This is a passing complete implemented suite and an intentionally incomplete phase aggregate, not a passing `--all` verdict.
- Final input identity: `1f02250c445745e3795137dd78d4d957f34882001052b05d0379f29cfb0da9a4`. The runner accepted fresh unique reports/evidence and found no approved source changes.
- All ten required `R-23/R-24/R-25/R-36/R-38.positive/.negative` identities executed independently. Forty-six analyzer subjects and four origin/boundary/input controls add 50 actual passes to the previous 90.
- The initial 30 captured Task 1 subjects also passed together twice; the final run includes all final boundary additions. Exact ID, Error severity, file, start/end span and zero unexpected compiler errors are enforced through genuine Roslyn compilation. Missing analyzer registration and wrong severity cannot satisfy a negative.
- Task 1 focused negatives: `8e82e2e8-7ae1-4917-a4c1-abfdb45c81c5` (SCR0001) and `931190fa-a330-46df-9530-bf703bf441cf` (SCR0002). Task 2 focused negatives: `25c0551e-55cc-4b30-958b-727df76c90f5` (R-23), `6a47ad3a-0d07-4016-a3cd-d1ff806b1b35` (R-24), `ec855abc-2ed1-451b-a057-9d38840ad9ce` (R-25). Final evidence supersedes earlier focused inputs.
- The first aggregate run (`c3729c82-fa7a-4ebd-b8e0-769b569b7b76`) exposed five integration regressions: Godot consumer/origin/boundary and two compiler fixture controls. Their demonstrated prerequisites were corrected and all five pass in the final run.
- A transient focused run rejected edits during verification with `SOURCE_CHANGED`; its result is discarded. A concurrent focused invocation was rejected with `CHECK_LOCKED`; it supplied no acceptance evidence.
- Windows execution only. Actual Linux execution, complete phase coverage, qualitative review and full phase closure remain pending. The existing local VM processes were preserved and are not acceptance evidence.

## TDD Gate Compliance

The following actual JUnit reports failed the intended diagnostic assertion after successful compilation/reference resolution, with an empty actual diagnostic set before implementation. Each record passed `check tdd-red-evidence ... --raw` with `RED_EVIDENCE_OK`; all reports and semantic assessments are retained under `.phase1/red-01-05/`.

| Rule | RED target / record directory | Expected behavior |
| --- | --- | --- |
| R-38 | InvocationPositionalArgumentReports | SCR0001 at positional method argument |
| R-36 | MethodLineAndBlockCommentsReport | Two SCR0002 body-comment spans |
| R-23 | ClassPrimaryConstructorReports | SCR0003 at class parameter list |
| R-24 | AnalyzePositionalRecordReports | SCR0003 at both positional record parameter lists |
| R-25 | AnalyzeInsideTypeReports | SCR0004 at region directive |

The initial unavailable Python launcher, compiling-harness errors and initial unqualified JUnit target-name classifier rejection were not credited as RED. Exact qualified names were taken from the unchanged JUnit reports before GREEN. Implementation adopted captured local source; cleanup made the verifier's own construction conform and kept its diagnostic ordering visible. No fabricated historical RED evidence was added.

## Deviations from Plan

1. **[Rule 3 — Selector mismatch]** Both authored `rules.family5.1` and `rules.family5.2` commands return `SELECTION_EMPTY`: those identities were absent from the independent manifest. Existing required rule IDs and individually bound captured edge IDs supplied the real proof selections; no alias or duplicate result was invented. WINDOWS entry 10 is fixed.
2. **[Rule 1 — Implicit argument bug]** The captured NamedArgument implementation reported compiler-synthesized collection construction arguments under the outer argument span. Ignoring `IArgumentOperation.IsImplicit` preserves its intended implicit-operation exception. A real Roslyn subject accepts a named collection-expression argument and still reports its explicit positional neighbor. WINDOWS entry 11 is fixed.
3. **[Rule 3 — Newly enforced prerequisites]** Actual compiler diagnostics required named arguments and removal of four in-body comments in existing proof code, a non-positional expected-diagnostic record and an ordinary verifier constructor. The Godot proof now passes its explicit named print array. Compile-only fixture subjects comply with call-site rules and own a minimal explicit entry point rather than unnecessary generated startup. No severity was lowered. WINDOWS entry 12 is fixed.
4. **[Rule 2 — Bounded generated compatibility]** Generated markers/settings remain checked. Exactly two pinned xUnit/Microsoft Testing Platform members and eight observed Godot-generated members receive SCR0001 permissions. Each has exact source/package hashes, target and proof references in `docs/code/generated-callsite-adaptations.json`; adjacent handwritten calls fail SCR0001, and every exact symbol impostor fails CS0101 or CS0111. The original Godot RS0030 adapter is unchanged. WINDOWS entry 13 is fixed.
5. **[Rule 2 — Input integrity]** The previously empty analyzer project was absent from the independent source fingerprint. `tools/code-rules` is now included. The isolated copied-source mutation changes the actual hash and rejects old evidence with INPUT_MISMATCH; approved source is unchanged and obj/cache output is excluded. WINDOWS entry 14 is fixed.

Release tracking lists only the four implemented descriptors as active, with all twelve exact donor IDs/type names enumerated. Future descriptors are comments until plans 01-06/01-07 implement them; this avoids false functionality and RS2002. Numeric body/line/continuation families remain with their explicit later owners and are not claimed here.

## Remaining Phase Work

- No goal-blocking stub, skipped test or unrun actual proof remains in this plan. Eight other analyzer families remain explicitly pending in plans 01-06/01-07, tracked by the still-open WINDOWS entry 8.
- The five distinct runner-edge identities remain pending for 01-14; WINDOWS entry 9 is unchanged. The historical bootstrap RED entry 1 remains open.
- Full rule/register activation, wider normal-build/exemption proofs, human review, graph/state/lifecycle/docs/coverage and Linux remain with their planned owners. All RULE requirements remain Pending.
- Next plan: **01-06**. Phase progress after this plan: **6/18**.

## Self-Check: PASSED

All created artifacts exist; all four listed task commits are ancestors of HEAD. Evaluation-scope resolved exactly these four commits. Final fresh reconciliation has zero failed implemented obligations, and the task commits have no unexpected deletions. Stub and threat-surface scans found no new unfinished current-plan implementation or unmodeled security boundary.
