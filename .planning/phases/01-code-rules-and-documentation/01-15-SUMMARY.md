---
phase: 01-code-rules-and-documentation
plan: "15"
subsystem: verification-selection
tags: [csharp, node, declaration-selection, rule-bindings, phase-5-handoff]
requires:
  - phase: 01-14
    provides: 666 implemented controls, current 85-rule bindings and approved certification timing amendment
provides:
  - Validated declaration-based --phase selection with separate deferred ownership
  - All 666 existing cases retained plus four focused selector controls
  - Exact eight pending certification identities assigned to Phase 5
  - Current binding validation and verification guidance aligned with Phase 5 ownership
affects: [01-16, 01-17, 05-ci-and-repeatable-game-testing]
tech-stack:
  added: []
  patterns: [independent-acceptance-phase-declarations, existing-summary-deferral]
key-files:
  created: []
  modified:
    - tools/phase1/required-cases.json
    - tools/phase1/check.cjs
    - tests/phase1/RunnerContractTests.cs
    - tests/phase1/RuleBindingValidator.cs
    - tests/phase1/RuleBindingTests.cs
    - tests/phase1/RuleMatrix.cs
    - docs/code/register.json
    - docs/code/rule-matrix.md
    - docs/workflow/verification.md
    - .planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md
key-decisions:
  - Select acceptance phases from independent executable case declarations; absent means Phase 1, never infer membership from implementation or results.
  - Preserve exhaustive and focused selector semantics, with deferred phase ownership outside the selected denominator.
  - Retain exact technical coverage scopes and targets; update only current owner and delivery-timing labels to Phase 5.
requirements-completed: []
requirements-contributed: [RULE-01, RULE-02, RULE-03]
coverage:
  - id: D1
    description: Declaration selection preserves all implemented cases and rejects unresolved or missing Phase 1 evidence
    requirement: RULE-03
    verification:
      - kind: integration
        ref: tests/phase1/RunnerContractTests.cs
        status: pass
    human_judgment: false
  - id: D2
    description: Current bindings retain strict functional proofs while pending certification belongs to Phase 5
    requirement: RULE-02
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case register.binding-precoverage
        status: pass
    human_judgment: false
actuals:
  tokens: 12379
  tasks: 2
  commits: 2
commits: 2
plan_head_before: aa7a036171d7d7cd5d615f90ffac5563f5850782
plan_head_after: 2ea0c8755a8902a1bdd3686bba6b506aec69d977
duration: 11min
completed: "2026-11-14"
status: complete
---

# Phase 1 Plan 15: Explicit acceptance phases and certification handoff Summary

**Declaration-based phase selection requires all 670 Phase 1 obligations and separately exposes eight pending Phase 5 cases without passing or skipping them.**

## Accomplishments

Task 1 extends the existing selector and reporter. Optional `acceptancePhase` defaults to 1; explicit values must be positive safe integers. `--phase N` selects declarations with phase at most N. Invalid declarations are rejected before selection, including corrupt declarations outside the requested phase. Malformed, duplicate and conflicting selectors are rejected. `--list` can accompany one selector and retains its existing array format. `--all`, exact `--case` and `--group` preserve their exhaustive membership within their respective scope.

The existing summary gains a separate `deferred` array containing ID, acceptance phase and owner, plus explicit console lines. Deferred cases are outside the selected denominator and receive no pass/skip record. Selected pending cases still block; selected missing, duplicate, failed, skipped, unrelated and stale evidence still fails the existing reconciliation. No second reporter was introduced. Inspection of existing Node report validation and C# JsonNode summary readers found no strict schema reader that needed weakening; normalized rerun comparison now includes `deferred`.

All original 666 implemented declarations match starting revision `aa7a036` exactly. Four newly declared C# controls exercise phase membership, malformed declarations, actual deferred summary publication/exhaustive pending failures, and actual unresolved/default-phase and missing-result rejection. Current membership is **670 Phase 1 / eight deferred / 678 exhaustive**. `--phase 4` still selects 670; `--phase 5` and `--all` select 678. Membership was inspected through actual `--phase 1 --list` and the exported existing selector, without executing a full suite.

Task 2 preserves all 85 source dispositions: **77 implemented bindings, two pending coverage rules and six reasoned inapplicable rules**. R-49/R-82 keep exact IDs, original planned locators, technical module inclusion, empty unimplemented methods, pending status and inclusive **90% lines / 80% branches** using exact counts without rounding. Only current binding owners, generated labels and delivery-timing suffixes change to Phase 5. Captured donor/provenance bytes remain unchanged. The validator still permits pending coverage only for R-49/R-82 and now requires their exact Phase 5 owner and numeric phase 5. Existing mutation tests additionally reject old owners and Phase 1 coverage acceptance. All other mechanisms, proof identities and scope checks remain intact.

Verification guidance now exposes the implemented command, focused closure cadence, final complete Windows run in 01-16 and representative actual Linux execution/review in 01-17. The manifest's older `acceptanceCriteria` table retains original Phase 1 implementation allocation, distinct from the eight exhaustive case wrappers; it does not drive selection. The Phase 5 handoff accurately records all eight pending identities and retains every implemented advanced runner/pin case in Phase 1.

## Task commits

| Task | Commit | Result |
| --- | --- | --- |
| 1 — Explicit phase ownership and bounded selector | `ff6c50f` | Existing runner, independent declarations and four actual selector controls |
| 2 — Current bindings and verification guidance | `2ea0c87` | Narrow binding/formatter/test updates, current labels, verification and Phase 5 handoff |

Both commits used normal hooks after the supplied-root guard and configured branch-protection check. Sequential execution on pinned main is explicitly configured and authorized. No unrelated changes were staged; no tracked file was deleted.

## Actual focused verification

The strict whole-solution command passed with **zero warnings / zero errors**:

`dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`

The restricted initial build returned a diagnostic-free failure; the same authorized build under scoped execution surfaced actual style diagnostics. New tests were corrected without changing analyzers or severity, and subsequent strict builds passed. No package acquisition was needed.

| Command | Actual result | Canonical invocation |
| --- | --- | --- |
| `--group runner` | Diagnostic run: 35/36 accepted, one failed new membership assertion | `d6c1a60b-4f6a-49aa-be70-ec0b20d0d7f5` |
| `--case runner.phase-membership` after correction | 1/1 accepted, exit 0 | `1a291dba-9fbe-4711-b7c4-26a0122e59e2` |
| `--case runner.phase-declarations` after expanded invalid-input checks | 1/1 accepted, exit 0 | `83687852-7fc4-43c5-8915-acdae09c9c38` |
| `--case register.binding-precoverage` | 1/1 accepted, exit 0 | `c0256a9f-0f9d-4557-bd25-35e9838db452` |
| `--group register` | 46 accepted / zero failed controls / one pending; exit 1 | `79094d4c-e083-4fc5-acf6-60697aecb8e7` |
| `--group docs` before task 2 commit | 36/36 accepted, exit 0 | `11895026-4d99-4ad6-863f-9f575a99ef96` |

Each invocation has its own immutable `.phase1/runs/<nonce>/summary.json`, per-case reports, command identities and logs. These runs are not combined into a fabricated full-suite verdict.

The runner diagnostic report's input is `70d94580ea1fdc663ab3568f1602b06f983d3aa375105a4fc8082422cb39d518`; tool `637d5cb07630795e74751bd1cea20d42eaa334a05a93f2664528cd64c28d1d77`. The sole failed test mistakenly looked for R-49 in `coverage` although its preserved declaration belongs to `rules`. Corrected selector runs share input `6b33fee8d51950091c9aadc836ca8da2dca7b4378ce4313e85aed4d8b32ab281` and tool `746e2b00f5fcecdf2ca425b3c31203c3fc40485ee0d5834f879061aa9712f4d7`; all other runner controls, including both new actual-process controls, passed in the diagnostic run.

The canonical binding and register runs share tested revision `ff6c50fcd4a2d0db142249a5f76c768eac879b4f`, input `9771e8f32e7a5ae8a8c529da022404a19d0f0872e9eb6a4c38383b59724f7fcf` and tool `6d0edb00be0198ccb86d16821c4a5ba8b46a9152323cf8b0fced25375440ddb3`. The register group correctly returns non-success solely because unchanged group semantics include `ac-06.complete`, blocked as `IMPLEMENTATION_PENDING`, owner `Phase 5`; it is not a failed functional control.

Precommit docs have input `69bbee63c20f6f50ca97a31dfc364833f5378a77d85508888afc48e79b51585d`, the same task-2 tool hash and tested revision `ff6c50fcd4a2d0db142249a5f76c768eac879b4f`. All recorded platform identities are `win32-x64-10.0.19044`. Source/prose changes have distinct fingerprints; earlier reports do not certify subsequent bytes.

## Deferred acceptance identities

| Exact identity | acceptancePhase | Current owner | Required meaning |
| --- | --- | --- | --- |
| `ac-02.complete` | 5 | Phase 5 | Exhaustive clean two-platform execution and reconciliation |
| `ac-06.complete` | 5 | Phase 5 | Complete register/mechanism linkage including real measured coverage |
| `ac-07.complete` | 5 | Phase 5 | Complete enforcement certification including real measured coverage |
| `ac-09.complete` | 5 | Phase 5 | All exact numeric/report thresholds including measured 90%/80% coverage |
| `R-49.positive` | 5 | Phase 5 | Real analyzer coverage satisfies both inclusive targets |
| `R-49.negative` | 5 | Phase 5 | Real analyzer coverage rejects insufficient/invalid evidence |
| `R-82.positive` | 5 | Phase 5 | Real production/native-hosted managed coverage satisfies both targets |
| `R-82.negative` | 5 | Phase 5 | Real production coverage rejects insufficient/invalid evidence |

These remain pending obligations, with no fabricated coverage, platform, pass or skip result. `register.binding-precoverage` and every paired functional rule proof remain Phase 1 obligations. The five `edge.rule-01.*` checks and all advanced implemented runner controls retain original identities and requirements.

## Deviations from Plan

No scope deviation. The narrowly scoped `RuleBindingTests.cs` change is the explicitly allowed existing test-helper wiring. The creator-approved timing correction changes only metadata suffixes in current R-49/R-82 scopes, leaving technical inclusion/exclusion and original captured fields intact.

During task 1, strict build checks caught new test collection/line formatting; a focused run caught the incorrect group expectation described above. During task 2, strict checks caught a new ordinal-comparison/style issue. These were repaired under the existing rules with passing relevant verification. No test was skipped, no verify step was omitted, no package substituted and no diagnostic, exemption, threshold or suppression scope weakened. No TDD/RED history is claimed for this non-TDD plan.

## Remaining obligations and limits

Plan 01-16 owns one final complete clean Windows Phase 1 run after closure stabilizes. Plan 01-17 owns actual representative clean Linux restore/build/bootstrap/proofs and the separate qualitative review. No full `--phase 1` or `--all` run was executed for this plan; exhaustive/pending behavior was exercised through the existing bounded real-process fixtures. The Linux VM, its tools/keys and live setup were left running and untouched. Frozen plan-14 reports were not edited or copied.

All seven RULE requirements and all fourteen human outcomes remain Pending. Phase 5 still owes measured coverage, exhaustive platform/runner certification and the eight exact identities above. There is no new production gameplay, project, coverage instrumentation, broad exemption or reporting framework. Existing historical WINDOWS item 1 remains open; item 9 stays fixed. No new implementation stub, skipped test, unrun plan verification or security-relevant trust boundary was introduced.

The initial recorded execution window begins `2026-11-14T13:55:00Z`. Actual token scale is 49,515 realized task-diff characters / 4, rounded up to 12,379; this is not a harness token count. The commit ledger records the measured two-task commit range above; subsequent summary/state evidence commits are separate closeout work.

## Self-Check: PASSED

All ten owned implementation/documentation paths exist. Both task commits are ancestors of HEAD. The persisted plan ledger begins at `aa7a036171d7d7cd5d615f90ffac5563f5850782`; its measured task-end range contains two commits ending at `2ea0c8755a8902a1bdd3686bba6b506aec69d977`. Baseline comparison proves exact preservation of all 666 earlier implemented declarations and all 85 technical rule definitions apart from the authorized coverage timing/owner fields. Canonical register binding, all 46 implemented register controls and all 36 documentation controls have actual accepted results. Initial diagnostic failures and corrected focused results are explicitly distinguished above.
