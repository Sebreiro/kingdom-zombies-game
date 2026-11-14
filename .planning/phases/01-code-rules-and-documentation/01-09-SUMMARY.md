---
phase: "01-code-rules-and-documentation"
plan: "09"
subsystem: testing
tags: [csharp, msbuild, metadata, architecture, godot, xunit]
requires:
  - phase: "01-08"
    provides: "Strict real-build policy, bounded source/generated permissions and trusted evidence"
provides:
  - "Independent exact five-project policy with real evaluated imports, recursive edges and authored inventory"
  - "Compiled metadata and IL checks for engine/provider leakage, including generated state machines"
  - "Real graph, service-port/provider, structural model, visibility and evolution mutation proofs"
affects: ["01-10", "01-11", "01-12", "01-14", "01-15", "01-16", "01-17"]
tech-stack:
  added: []
  patterns: ["Clean compiled control before intended violation", "Evaluated graph plus compiled metadata", "Fresh exact proof reconciliation"]
key-files:
  created:
    - tools/phase1/projects.json
    - docs/code/visibility-exemptions.json
    - tests/phase1/ProjectEvaluator.cs
    - tests/phase1/TypeBoundaryScanner.cs
    - tests/phase1/GraphFixture.cs
    - tests/phase1/GraphTests.cs
    - tests/phase1/BoundaryFixture.cs
    - tests/phase1/ArchitecturePolicy.cs
    - tests/phase1/ArchitectureRuleTests.cs
    - tests/phase1/Models/BoundaryEntity.cs
    - tests/phase1/Models/BoundarySnapshotDto.cs
    - tests/phase1/Models/BoundaryMapper.cs
    - tests/phase1/Models/BoundaryOptions.cs
    - tests/phase1/Models/BoundaryKind.cs
  modified:
    - docs/code/register.json
    - tests/phase1/RegisterValidator.cs
    - tools/phase1/required-cases.json
key-decisions:
  - "Require independent expected projects/types, actual evaluated imported edges and compiled type/IL evidence; empty discovery cannot pass."
  - "Keep structural proof models in the existing test project; production state/snapshot examples remain owned by 01-10/01-11."
  - "Require successful clean assembly-reference control before exact Subject.cs(7,22) CS0012 for missing GodotSharp 4.7.2.0."
  - "Successful proofs identify the actual inspected source even when no diagnostics exist; empty subject evidence remains rejected."
requirements-completed: []
requirements-contributed: [RULE-03, RULE-05, RULE-07]
coverage:
  - id: D1
    description: "Actual allowed graph and compiled direct forbidden edge"
    requirement: RULE-05
    verification:
      - kind: integration
        ref: "node tools/phase1/check.cjs --case graph.allowed-and-direct"
        status: pass
    human_judgment: false
  - id: D2
    description: "Evaluated graph/type mutation matrix with independent expected subjects"
    requirement: RULE-05
    verification:
      - kind: integration
        ref: "node tools/phase1/check.cjs --group graph"
        status: pass
    human_judgment: false
  - id: D3
    description: "Concrete structural/default-visibility/evolution proofs and provider boundaries"
    requirement: RULE-03
    verification:
      - kind: integration
        ref: ".phase1/runs/3fa47e43-8147-43bb-b88b-46529437cfc6/summary.json"
        status: pass
    human_judgment: false
actuals:
  tokens: 45646
  tasks: 2
  commits: 11
commits: 11
plan_head_before: 286fb681f356badc1c426ffca35f2770053702bb
plan_head_after: 60ea7e270718c984f73bd5f142fd30a8c691d060
duration: 158min
completed: "2026-11-14"
status: complete
---

# Phase 1 Plan 09: Evaluated dependency and compiled-type boundaries Summary

**Actual MSBuild graphs, compiled metadata/IL and concrete architecture mutations are enforced and verified by 545 accepted fresh controls.**

Both owned tasks are complete. Phase 1 remains incomplete: 60 declared later-owner obligations remain pending, and every shared RULE requirement remains Pending. This plan contributes to RULE-03/RULE-05/RULE-07 without claiming their cross-plan completion.

## Final verification

The final immutable `node tools/phase1/check.cjs --all` run reconciled **545 accepted controls, zero failed controls, 60 IMPLEMENTATION_PENDING obligations and 605 declared cases**. Actual exit was **1 solely because the later-owner obligations remain pending**. All 26 current graph-group cases passed within this full run; pending cases never count as passing.

- Canonical report: `.phase1/runs/3fa47e43-8147-43bb-b88b-46529437cfc6/summary.json`.
- Console/actual exit: `.phase1/temp/09-final-reconciled-all.log` / `09-final-reconciled-all.exit`.
- Tested revision: `60ea7e270718c984f73bd5f142fd30a8c691d060`.
- Input identity: `b5b925b1b4b570511c3b436e66c4b95f0b31839252dfdeb20864b37d9dc45488`.
- Tool identity: `7fb6dbaaceb38ede22251c84718f7d3ae190269928dc9183d0099c434cb49dd2`.
- Actual platform: `win32-x64-10.0.19044`.
- Run interval: `2026-11-14T04:14:51.390Z` to `2026-11-14T04:55:02.718Z`; fingerprinted inputs stayed unchanged.

Final prerequisites included strict `dotnet build KingdomZombies.slnx --no-restore -p:NuGetAudit=false --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -v:q` with zero warnings/errors, exact CS0012 plus naming/bindings/source-format checks 3/3 (`09-fixed-preflight.xml`), all 36 register mutations in the affected 41/42 run, and canonical corrected R-60 evidence 1/1 (`1cfc9e65-4fd0-4206-9478-961653a7c711`). The final full run independently covers all final implementations and bindings. Five exact LF locks, twelve Error analyzers and native engine compatibility prerequisites passed; native nonce `20a330d4-1dc9-423f-9f91-f7bc28e10b5d` remains raw Windows compatibility evidence. `git diff --check` passed and the checkout was clean after final verification.

The earlier immutable run `7964c2a3-e280-4463-aea0-eb77e62bc268` is retained as failed evidence: 545 passing test reports yielded only544 accepted controls because R-60.positive had an empty subject location. It had one `SUBJECT_EVIDENCE` failure and60 pending cases, actual exit1. That failure caused the small source correction and justified the final full rerun. It is never presented as accepted evidence for the corrected source.

## Delivered behavior

Task 1 checks the independent exact five-project policy, solution plus tracked/new authored inventory, missing projects/types/selectors, duplicates before set conversion, output kinds and both analyzer-reference metadata fields. Real MSBuild evaluation retains imported item origins and recursive closure; Core and ports have no production edges, game composition uses only Core/ports, and reusable analyzer boundaries remain product-agnostic. Trusted freshly compiled assemblies are inspected with built-in PEReader/MetadataReader without loading engine Nodes.

Task 2 proves direct/imported props/targets, wildcard/semicolon/transitive/undeclared/duplicate/reordered reference mutations; compiled base/interface/generic/attribute/signature/field/body and lambda/async/iterator leaks; independent ports-only providers; actual Core-to-concrete-provider leakage; analyzer/helper product isolation; and resolved service-locator aliases. Exact compiler controls require CS0246 at Subject.cs(8,23) and CS0012 at Subject.cs(7,22) after clean infrastructure succeeds. Infrastructure/style/restore failures never satisfy a negative.

Concrete test-only entity/snapshot/mapping/configuration/enum models prove R-63–65/R-81 structure and R-66/R-74/R-75/R-77/R-78 version/evolution constraints. Default internal visibility and exactly observed public surfaces use the exact source/type registry. Independent register mappings bind real mechanisms and unique proof methods for R-34/R-51–56/R-58–66/R-74/75/77/78/81. R-57 remains inapplicable; qualitative and later-owner statuses remain Pending. No sixth repository project, gameplay feature, production state model, package acquisition, suppression widening or immutable donor edit was introduced.

## TDD evidence and task commits

- Task 1 direct-edge RED `266d495`: successful clean/changed real builds, then intended GRAPH_FORBIDDEN assertion failure; unchanged `09-red.xml` plus record `09-red.json`, classifier RED_EVIDENCE_OK. GREEN `92fca69` passes canonical tracer1/1 (`79c0915a-63da-428b-8280-2ae6e7178bb7`).
- Task 1 compiled-type RED `1ccde62`: successful clean and leaking subject builds, intended TYPE_FORBIDDEN assertion failure; `09-type-red.xml/json`, RED_EVIDENCE_OK. GREEN `c6280d4` passes the exact compiled-type proof1/1.
- Task 2 expansion RED `c42201d`: real compilable executable Core remained accepted, then intended GRAPH_OUTPUT_KIND assertion failed; `09-expansion-red.xml/json`, RED_EVIDENCE_OK. GREEN foundation `7e2a528` and final expansion/bindings `90890af` complete the evaluated/compiled/structural matrix. `60ea7e2` fixes successful locator evidence identity.
- Historical continuation metadata: `9e6ac9f`, `11fc981`, `30002af`; all committed increments and original RED evidence remain preserved.

The persisted ledger measures11 commits from `286fb681f356badc1c426ffca35f2770053702bb` through the tested source head. Actual tokens use realized committed diff characters/4 (45646), not model token usage. Duration158min covers the observed continuation/verification window from prior metadata `3a5c66a` at02:17:30Z through04:55Z; metadata closure time is additional.

## Deviations from Plan

### Auto-fixed issues

**1. [Rule 3 - Blocking] Resolved strict fixture prerequisites.** Corrected new-source naming/layout/continuation diagnostics and canonical copied-fixture root/producer prerequisites without weakening inherited policy. These bounded corrections and their actual outcomes are preserved in the historical checkpoint. No compilation failure supplied RED credit.

**2. [Rule 1 - Bug] Corrected boundary direction and undeclared compiler assumptions.** Core-to-provider now compiles the intended direction; reusable/provider checks require evaluated graph and compiled verdicts. A ProjectReference legitimately supplies transitive compile assets, so the undeclared-engine compiler fixture uses the actual built game assembly with no direct GodotSharp reference. Corrected Godot-specific HintPath and observed exact CS0012 coordinates; `90890af`.

**3. [Rule 1 - Bug] Preserved the successful locator subject location.** Joining an empty success diagnostic list produced an empty location and the trusted runner rejected it. Record `CompositionRoot.cs`, the exact filename written, compiled and semantically inspected by the fixture. Canonical1/1 and final545-control reconciliation passed; `60ea7e2`. No runner contract was weakened.

All changes remain within the owned architecture/proof scope. No authentication or package-legitimacy gate occurred. No tracked files were deleted.

## Remaining phase obligations

The60 pending obligations remain required: platform1, register1, rules24, documentation7, engine9, state13 and pins5. Plans01-10/11 own production state/snapshot examples,01-12 lifecycle/thread/export proofs,01-13 documentation,01-14 shared register/runner closure,01-15 measured coverage, and01-16/17 clean checkout plus actual Linux execution. AC-07 complete remains honestly pending. Actual Linux, managed coverage and complete phase acceptance are not claimed. The prepared Linux VM and independent Phase2 context were preserved.

No goal-blocking stub or skipped/unrun owned verification remains. Qualitative design judgments remain separate pending review. WINDOWS historical missing-RED entry1 and runner-edge entry9 are unchanged. All shared RULE IDs are blocked by unfinished sibling plans per `requirements.ready-ids`; none is marked Complete.

## Next action

Continue Phase1 with01-10. Keep all strict inherited settings, exact generated producer/member/span permissions, five LF lock boundaries and immutable provenance intact.

## Self-Check: PASSED

All14 declared created artifacts and the SUMMARY exist. All11 recorded plan commits are ancestors of HEAD. The measured ledger range is nonzero, final source changes are committed, and the final report accepts545 implemented controls with no failed controls. Historical RED reports and both immutable runs remain available under `.phase1/`.
