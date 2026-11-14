---
phase: 01-code-rules-and-documentation
plan: "14"
subsystem: documentation-and-proof-linkage
tags: [csharp, msbuild, documentation, rule-register, provenance, frozen-verification]
requires:
  - phase: 01-13
    provides: Canonical navigation, portable documentation checks and strict inherited proof baseline
provides:
  - Five checked project reference pages reconciled with evaluated MSBuild references
  - Exact 85-rule index with 77 implemented current bindings, two pending coverage rules and six inapplicable dispositions
  - Distinct actual closure of the five original pin-runner edges and R-67 suppression controls
  - Fourteen qualitative components and a separate Pending human-check artifact
affects: [01-15, 01-16, 01-17, phase-5-certification]
tech-stack:
  added: []
  patterns: [evaluated-reference-declarations, exact-symbol-and-proof-bindings, immutable-validation-checkout]
key-files:
  created:
    - src/core/docs/README.md
    - src/multiplayer-abstractions/docs/README.md
    - src/godot-game/docs/README.md
    - tools/code-rules/docs/README.md
    - tests/phase1/docs/README.md
    - docs/code/rule-matrix.md
    - docs/code/review.md
    - docs/architecture/boundaries.md
    - docs/architecture/decisions/0001-development-baseline.md
    - tests/phase1/DocumentationProjects.cs
    - tests/phase1/DocumentationProjectTests.cs
    - tests/phase1/RuleDefinitions.cs
    - tests/phase1/RuleBindingValidator.cs
    - tests/phase1/RuleBindingTests.cs
    - tests/phase1/RuleMatrix.cs
    - tests/phase1/PinEdgeTests.cs
    - tests/phase1/SuppressionPolicy.cs
    - tests/phase1/SuppressionTests.cs
    - .planning/phases/01-code-rules-and-documentation/01-HUMAN-CHECK.md
  modified:
    - docs/code/register.json
    - tools/phase1/required-cases.json
    - tests/phase1/DocumentationValidator.cs
    - tests/phase1/RegisterValidator.cs
    - tests/phase1/RegisterTests.cs
key-decisions:
  - Keep production, test inspection and both analyzer-only metadata fields distinct in checked module declarations.
  - Resolve current rules to actual symbols/settings and unique reciprocal proof methods; do not credit historical pending locators as implemented mechanisms.
  - Preserve final acceptance and human review separately from implemented precoverage linkage.
  - Validate a committed frozen candidate when concurrent authority edits can invalidate a main-checkout run.
requirements-completed: []
requirements-contributed: [RULE-01, RULE-02, RULE-03, RULE-04, RULE-05, RULE-06, RULE-07]
coverage:
  - id: D1
    description: Five module pages match actual references and reject missing, empty, unreachable or mismatched declarations
    requirement: RULE-04
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case docs.projects
        status: pass
    human_judgment: false
  - id: D2
    description: Current source/proof linkage and distinct pin/suppression cases have accepted fresh evidence
    requirement: RULE-03
    verification:
      - kind: integration
        ref: .phase1/temp/14-frozen-a8ebbf6/.phase1/runs/45f70351-fca6-428e-bd8f-8f43ede2d665/summary.json
        status: pass
    human_judgment: false
  - id: D3
    description: Necessary APIs, invariant meaning, mapping intent, enum evolution and decision accuracy receive separate human review
    verification: []
    human_judgment: true
    rationale: Structural checks and AI review cannot establish the required qualitative human outcome; reviewer/date/outcome remain Pending.
actuals:
  tokens: 58353
  tasks: 2
  commits: 5
commits: 5
plan_head_before: e1f3e372b0ec5f6e9e9d54d47073904421682625
plan_head_after: a8ebbf6e37849919ca687a588b695b784c0dce8e
duration: 101min
completed: "2026-11-14"
status: complete
workflow-closeout: metadata-complete-focused-documentation-validation-pending
---

# Phase 1 Plan 14: Project references and real rule bindings Summary

**Five module declarations now match evaluated MSBuild references; all 85 rules have accurate current linkage/dispositions, and the frozen candidate accepted 666 implemented controls with zero failed controls.**

Both implementation tasks are complete and committed. Shared workflow metadata closeout resumed after the authorized acceptance-timing amendment was committed as `1587015`: ROADMAP was updated first, then STATE advanced to **15/18, next revised 01-15**. A separate latest-main D-11 documentation result follows the metadata commit; the frozen full report remains evidence for its original tested candidate. No coverage, Linux acceptance or human pass is claimed. All shared RULE requirements remain Pending.

## Delivered behavior

Each of the exact five projects has a short purpose/boundary/reference page. Core and ports declare no production references; Godot composes both; the analyzer has no product references; tests explicitly distinguish their internal Core inspection edge from other inspection/process tooling. The checked declarations point to the one executable `projects.json` policy. Both `OutputItemType=Analyzer` and `ReferenceOutputAssembly=false` remain mandatory and separate from assembly references. The actual Core navigation tracer reconciles all five declarations against evaluated MSBuild output; missing/empty/unreachable pages, changed edges, duplicate declarations and either altered analyzer metadata field fail independently.

The focused rule index covers all **85 exact IDs**, with **77 implemented current bindings**, **two pending coverage rules** and **six reasoned inapplicable dispositions**. Bindings resolve actual source symbols or literal settings, including the precise parameter names selecting the overloaded compiled type scanner. Positive/negative references identify separate actual methods and reciprocal exact case traits. Missing, reversed, ambiguous, invented, scope-drift, config-drift, stale-index and fabricated-coverage bindings fail. Register order does not change the verdict. Per-diagnostic links lead to exact generated/member/span permission registries; no new exemption or severity downgrade was introduced.

The index and concise baseline decision record explain Godot construction, generated partials, visibility, exports, main-thread/async/nullability boundaries, checked integer time, pinned runtime random replay, exact identities, strict snapshots and R-70's deliberate HTTP divergence. They preserve **85 IDs / 84 heading occurrences / 24 original exemptions**, including the deliberate **EX-22 numbering gap**. The six HTTP/DI/database/health-check/test-host inapplicability reasons are individual and match the captured source. Forty body lines remains advisory; sixty is the enforced inclusive limit. Measured inclusive 90% line / 80% branch coverage remains required, not credited by linkage.

R-67 now has its own actual repository and violation cases. Its checks cover semantic C# suppressions, exact editor/global configuration, MSBuild XML and response/workflow commands. The five carried pin-runner edge IDs retain their original declared owner `01-02`, add closure owner `01-14`, and execute five distinct methods with fresh actual subjects. None borrows another case's result.

`review.md` identifies all fourteen qualitative components, including necessary APIs, invariants, mapping intent, enum evolution and decision accuracy. The separate phase-end human-check artifact records reviewer, date and outcome as **Pending**. Automated results and AI review supply no human pass. Indexed interactive preferences and `workflow.text_mode=false` are preserved.

## Atomic task commits

| Task | Commit | Result |
| --- | --- | --- |
| 1: Check project purpose/references against live graph | `8f46d27` | Five pages, real navigation/evaluation tracer and eight independent module controls |
| 2: Bind real mechanisms/proofs and separate human review | `a8ebbf6` | Exact current index, ten binding controls, R-67 pair, five pin edges and Pending human artifact |

The persisted ledger measures **five** commits between the frontmatter heads: these **two owned task commits** plus three concurrent Phase 3 planning commits (`7b81cd4`, `d81d39d`, `6dff108`). The ledger was not rewritten to hide concurrency. Realized-cost actuals exclude those foreign changes: the net owned diff across **31 files** is **233411 characters / ceil(chars/4) = 58353 tokens** on the estimate's scale. The observed implementation/verification window is `2026-11-14T11:38:25Z`–`2026-11-14T13:18:58Z`, rounded to **101min**; subsequent authority-reconciliation waiting is excluded.

## Immutable full evidence

The [unaltered canonical full report](../../../.phase1/temp/14-frozen-a8ebbf6/.phase1/runs/45f70351-fca6-428e-bd8f-8f43ede2d665/summary.json) records **666 ACCEPTED, zero failed controls and eight IMPLEMENTATION_PENDING obligations out of 674**. Actual exit is **1**, solely because those declared obligations remain pending. The report's overall status is `fail`; it is not represented as complete Phase 1 acceptance.

| Identity | Actual value |
| --- | --- |
| Candidate | `a8ebbf6e37849919ca687a588b695b784c0dce8e` |
| Frozen root | `D:/Work/Games/KingdomZombies/.phase1/temp/14-frozen-a8ebbf6` |
| Invocation | `45f70351-fca6-428e-bd8f-8f43ede2d665` |
| Input | `faceec4629ccbff946e7952cde6e432216ff8204219c5e6b94b5bd25c902cc2e` |
| Tool | `208c2f610cb0cb9b577660cc45af815b3668e3eaf5569a068f53bf11a96706e4` |
| Platform | `win32-x64-10.0.19044` |
| Full interval | `2026-11-14T12:28:05Z`–`2026-11-14T13:18:58.073Z` |

Accepted groups: runner **32**, pins **27**, register **46**, rules **447**, docs **36**, graph **26**, engine **19**, state **33**. Every implemented obligation has its own exact fresh test/report/subject evidence. The coordinator verified unchanged frozen inputs through publication; the checkout remains at the tested candidate. [Derived closure evidence](01-14-EVIDENCE.json) indexes the source report, original five pin identities, R-67 diagnostics, measured costs and exact authority deltas; it does not replace or edit the immutable report.

At this frozen pre-amendment revision, pending cases are `ac-02.complete` (owner `01-17`), `ac-06.complete`, `ac-07.complete`, `ac-09.complete` and the four `R-49`/`R-82` polarity cases (owner `01-15`). These are the report's historical owner values. The subsequently authorized timing amendment moves coverage/exhaustive certification to Phase 5; revised plan 01-15 owns reconciling selectors and handoff mappings. Pending cases remain pending, never passed or waived.

## Focused and prerequisite evidence

- Strict whole `KingdomZombies.slnx` build: **zero warnings/errors**, in both main and frozen candidate. Frozen exact five-project locked restore passed.
- Actual Task 1 module controls: **8/8 passed**, no skipped tests. Canonical `docs.projects` accepted before commit (`85b75383-e2b9-4993-9495-7ee2252d6765`) and at the post-commit tracer gate (`fbe581f1-b7a5-4cbc-aa13-537a96232e46`).
- Task 2 actual pin-edge/R-67/binding feedback: **17/17 passed**; inherited register/naming/analyzer preflight **40/40 passed**. Final exact-symbol binding controls **10/10 passed**, canonical main binding nonce `b0fc3b7a-91b0-4cc2-819c-00ce31385c5b`.
- Frozen native bootstrap passed (`49a095fb-8d46-4f55-838a-4a1ad74dcc62`, raw compatibility only). Source-format/five-lock/native-companion prerequisites passed **5/5** in `.phase1/temp/14-contracts/14-contracts.xml` under the frozen root.
- Frozen canonical docs: **36/36 accepted**, nonce `6ad31132-e3cc-4284-97e6-fbbbf5eeefa6`. Register: **46 accepted, zero failed controls, only AC-06 pending**, nonce `eb8a0792-f27c-49d6-881a-1e5e3adb2b31`. Exact binding gate: **1/1 accepted**, nonce `eba0f7fa-e817-4567-93fe-87a31983c356`.
- `git diff --check`, actual method uniqueness, exact five LF locks and unchanged bounded generated permissions passed. No plan `<automated>` verification remains unrun. No new skipped test or functional stub was introduced. This prose-oriented plan has no TDD annotation; no historical RED is invented.

## Deviations and integration findings

**[Rule 2 — Required real linkage] Added cohesive C# validators/tests within the authorized helper allowance.** Existing generic planned locators could not prove actual source/settings and distinct methods. The current `binding` contract, exact rendered index, module evaluator and independent R-67/pin cases close that gap without a sixth project, new product subsystem, package, suppression or donor edit. A legitimate overloaded scanner needed an exact parameter-name selector; ambiguity validation remains strict.

**[Rule 3 — Verification isolation] Used a committed frozen validation checkout.** An earlier main graph run executed 26 controls successfully but correctly rejected final publication with `SOURCE_CHANGED` when a parallel Phase 3 session edit changed STATE. That diagnostic report (`64cbefef-c186-4bdf-bd59-04864147dcf0`) is not acceptance evidence. The orchestrator authorized a frozen local test checkout; authored edits/commits stayed in pinned main and no source was repaired in the fixture. It used 41 exact locked package payloads with archive-sidecar/normalized-lock checks, the verified official Windows engine archive and its own CLI/cache/proof paths. The Linux VM and private keys were excluded.

Fixture setup exposed `powershell.exe` Archive-module autoload failure. The same hash-verified official ZIP was installed with existing .NET `ZipFile` before bootstrap reran successfully; no bootstrap source or package name was substituted. Signed NuGet content hashes and archive-byte hashes were checked separately, as already documented by 01-04. A warm SDK cache whose bytes did not match the official archive package was not copied; the fixture resolved SDK inputs from the verified archive feed. This is setup evidence, not clean-platform certification. Early raw runner-root/selector mistakes and untracked documentation helper inventory failures received no acceptance or RED credit; final actual controls and the full suite pass.

WINDOWS item **9 is fixed** at `2026-11-14T13:24:24.221Z` from its five distinct accepted full-run controls. Historical missing-bootstrap-RED item **1 remains open**. Existing strict severities, all twelve analyzers, 170 exact native generated spans, eighteen member suppressions, authentic xUnit logical-seed production and inherited native companion repair are preserved.

## Authorized authority amendment and completed shared metadata closeout

After the frozen candidate was created, another chat edited acceptance timing. The orchestrator verified the direct user instruction to move coverage and exhaustive platform/runner checks to Phase 5 while preserving important rules and everything already working. That instruction governs remaining work. No replacement full run is required merely for this authorized timing amendment.

The main/candidate comparison observed **exactly five changed fingerprinted authority files**, with **no removed inputs** and **no implementation/docs/config drift**:

- `.planning/PROJECT.md`
- `.planning/REQUIREMENTS.md`
- `.planning/ROADMAP.md`
- `.planning/phases/01-code-rules-and-documentation/01-SPEC.md`
- `.planning/phases/01-code-rules-and-documentation/01-CONTEXT.md`

The observed main input is `bc50ad84fd6102143420f97b148a7dd0b7223fe1ad77015397f660295583dc01`; exact before/after file hashes are in the derived evidence. It differs from the frozen full input and has not been represented as tested by that report. Further WIP also includes revised plans 14–17, the human-check artifact, research/coverage/skeleton/deferred notes and the Phase 5 handoff. Those edits belong to the other chat and were not staged, overwritten or reverted by this executor.

The initial closeout deliberately left shared progress untouched while that chat was editing. The orchestrator subsequently verified the user approval and committed all 24 amendment files as `15870156eee42fa72b0f0588427e37865ab6aff2`. The approved amendment supersedes old timing for the remaining plans and preserves all working rules/checks. Its later edits also changed STATE and seven focused documentation pages; the earlier five-file comparison above is a historical point-in-time observation, not a claim about current bytes.

On authorized resumption, SDK `roadmap.update-plan-progress 1` first recorded **15 summaries / 18 plans, In Progress**. Only then did STATE advance from plan **14 to 15** and recalculate **15/18 completed**. SDK metrics record the existing actual **101min / two tasks / 31 owned files**; the previously measured token and commit-span figures remain unchanged. The complete `Phase 2 context captured (02-CONTEXT.md); Phase 3 context gathered (03-CONTEXT.md)` annotations and the new amendment decisions are preserved. REQUIREMENTS and all fourteen human outcome rows remain Pending.

This resumption changes only shared progress and owned summary/derived evidence. It performs no source/runner repair, no replacement full run, and no Linux VM/helper/transfer operation. Revised 01-15 owns phase selection and executable Phase 5 handoff reconciliation; 01-16 owns the final complete functional Windows run; 01-17 owns representative actual Linux execution and qualitative review. The separate latest-main D-11 documentation check is recorded after the metadata commit with its own input/tool identity.

All shared RULE requirements and qualitative reviewer/date/outcome remain Pending. No gameplay, Phase 2 execution, actual Linux acceptance or measured coverage is claimed. The exact fourteen human components are ready for consolidated phase-end review; automation/AI do not fill their outcome fields.

## Self-Check: PASSED

All 31 owned implementation/artifact paths plus this SUMMARY and the derived evidence file exist (**33 checked paths**). Both owned task commits are ancestors of main HEAD, and the persisted ledger measures the five-commit range recorded above. The unaltered full report reconciles 666 accepted controls, zero failed controls and exactly eight pending obligations; the frozen input snapshot remains unchanged. No functional TODO/FIXME/placeholder, new skipped test, accidental tracked deletion or new unmodeled network/auth surface was introduced. WINDOWS 9 is fixed and historical WINDOWS 1 remains open. Shared metadata now records 15/18 and next revised 01-15; latest-main D-11 evidence is recorded separately below.
