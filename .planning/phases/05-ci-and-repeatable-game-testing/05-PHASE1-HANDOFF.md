# Phase 5 — Certification transferred from Phase 1

**Latest local execution amendment, 2026-11-15:** Follow [local testing strategy](../../../docs/workflow/local-testing.md): whole-phase implementation and review/fixes before game windows; routine local execution on the development OS only, no VM, 1280×720; all active Critical/new/changed/affected cases; shared ordered game-run groups, collected failures, batched repair, failed-case checks and a fresh entire-selected-set pass after repairs. The eight identities and 90%/80% targets below remain Phase 5 obligations. Their full Windows/Linux certification can use actual hosted execution; the earlier additional local-VM requirement is superseded. Phase 5 proves full-workflow compatibility, not a full local catalog run at every future phase closure. Every phase saves scenario importance/run groups; Phase 11 adds nightly full testing under TEST-11. Older execution text below is interpreted through this amendment and the current SPEC/CONTEXT.

**Later creator exception, 2026-11-14:** Phase 1 may close under its [focused no-restart scope](../01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md). Preserve all eight pending declarations and unchanged 90%/80% targets. Its [complete-Linux/canonical-graph and human follow-ups](../01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md) remain transparent; no full Linux certification or individual human pass is supplied.

**Current Phase 5 discussion amendment, 2026-11-14:** Account for the complete required scope/environments, reusing unchanged passing group evidence only after validating relevant source/assets, dependencies, test/case definitions, tools and required environment. Rerun missing/invalidated groups and dependents; uncertain impact blocks acceptance. Basic Windows/Linux PRs target about ten minutes per OS and do not run the full proof suite. See [05-CONTEXT.md](05-CONTEXT.md) and [05-SPEC.md](05-SPEC.md). This supersedes the repeat-everything phase-end timing rule, not the eight pending IDs, coverage targets or environment obligations. Scout-time counts remain 684 declarations / 676 Phase 1 obligations.

**Approved:** Creator instruction, 2026-11-14 (Europe/Paris).
**Status:** Required Phase 5 planning input; implementation and acceptance pending. This handoff is not an executed plan or a passing report.
**Owners:** TEST-02, TEST-07, TEST-08 and TEST-09. RULE-01–RULE-07 retain their primary Phase 1 ownership for the usable development baseline.

## Preserve the delivered foundation

Keep the selected pins/locks, all twelve analyzers, exact code-rule thresholds, five-project graph, source register, bounded exemptions/generated permissions, every implemented runner/documentation/architecture/state/lifecycle test and the real Godot fixtures. Do not delete completed checks, widen suppressions, lower severity or replace working infrastructure just to implement this handoff. Already implemented advanced cases remain required regressions in Phase 1 and later.

The revised [Phase 1 specification](../01-code-rules-and-documentation/01-SPEC.md) retains actual clean Windows/Linux baseline execution. Phase 1 completion supplies no measured coverage or exhaustive two-platform certification credit.

## Required work packages

| Package | Phase 5 acceptance | Source / requirement |
| --- | --- | --- |
| Measured analyzer coverage | Real covered/total line and branch counts for the analyzer scope; at least 90% lines and 80% branches | R-49; TEST-02/07/09; former 01-15 coverage task |
| Measured production coverage | The same unchanged targets for applicable handwritten Core, ports and native-hosted managed Godot code; prove an actual engine callback is measured | R-82; TEST-02/07/09; former 01-15 coverage task |
| Coverage failure/report controls | Reject below-limit, absent, empty, malformed, duplicate, stale and wrong-source reports; exact denominators, legitimate generated exclusions and order-independent totals | AC-06/07/09 aggregate certification; TEST-07/08/09 |
| Complete Phase 5 certification execution | Complete valid Windows/Linux evidence, including pending aggregate/coverage cases, certification variants and native/state evidence; fresh or explicitly validated unchanged-group results covering final relevant inputs | Exhaustive AC-02 with the later execution/reuse amendment; TEST-02 |
| Platform/runner certification | Reconcile discovery, actual exits, case counts, source/tool/platform/run identities and required evidence; exercise remaining advanced failure, interruption and concurrent-run scenarios | TEST-07/08/09 |

Coverage targets are inclusive and unchanged. Compare exact counts using overflow-safe integer arithmetic, not rounded percentages. Report the analyzer and production scopes separately. Classify modules with no executable sequence points explicitly; absent expected handwritten modules are failures, not exclusions. Do not omit unmeasured engine code to meet the targets.

Use the pinned local coverage tool selected by the existing research only after proving compatibility. First collect a genuine managed Godot callback and nonzero branch evidence. If native hosting prevents dynamic collection, instrument isolated copied assemblies/PDBs and launch the existing scene against the copy; retain original binaries and source identity. Do not change production sources to improve the measurement. Add meaningful behavior tests where necessary. Coverage orchestration must be acyclic: functional execution, collection, validation and publication; coverage tests must not recursively call the full suite.

Complete clean execution remains donor-independent and identifies actual OS/kernel/architecture, tools, revision and command results. A Windows cross-build or prepared Linux VM never proves Linux execution. A source fix invalidates affected groups and dependents on each required environment; rerun those and validate any unchanged evidence against final relevant inputs. Keep each reused report's original tested revision/invocation instead of claiming a fresh full rerun. Coverage reuse requires unchanged complete module/scope inputs. Actual full hosted workflow execution still needs proof, not configuration alone. Native/headless results do not replace rendered-input, physical mixed-OS LAN, reference-PC or endurance evidence.

## Transferred case identities

At amendment time, the independently declared manifest contained 674 cases: 666 implemented cases and the eight pending identities below. Plan 01-15 retains those 666 and adds four focused selector controls, for 678 declarations and 670 Phase 1 obligations. Each of the eight pending declarations has `acceptancePhase: 5` and owner `Phase 5`. Preserve their IDs and original exhaustive meaning; this assignment supplies no passing result.

| Existing ID | Phase 5 closure |
| --- | --- |
| `ac-02.complete` | Exhaustive clean two-platform execution and reconciliation |
| `ac-06.complete` | Complete register/mechanism bindings including measured coverage |
| `ac-07.complete` | Complete enforcement certification including measured coverage |
| `ac-09.complete` | All numeric/report thresholds including measured 90%/80% coverage |
| `R-49.positive` | Actual analyzer coverage meets both targets |
| `R-49.negative` | Actual analyzer coverage gate rejects insufficient/invalid evidence |
| `R-82.positive` | Actual production/engine coverage meets both targets |
| `R-82.negative` | Actual production/engine coverage gate rejects insufficient/invalid evidence |

The five `edge.rule-01.*` identities were implemented during 01-14 and stay required; their older pending handoff is historical. Do not move implemented tests out of Phase 1 merely because they cover advanced runner behavior. Further cases discovered during Phase 5 must be declared and owned before acceptance; the list above is not permission to omit actual failures.

The implemented `--phase 1` selector uses explicit declared phase ownership, never an `implementation == csharp` filter. An absent phase means 1; `--phase N` requires every declaration whose acceptance phase is at most N. `--all` remains exhaustive, including pending Phase 5 cases; `--phase 5` also activates all currently declared cases. The existing summary lists deferred IDs/phases/owners outside the selected denominator without pass/skip credit. Missing or failed Phase 1 cases remain failures. Phase 5 requires actual evidence for every activated case: fresh execution when missing/invalidated, or explicitly validated unchanged evidence under the amended policy. Current precoverage binding and paired proofs remain Phase 1 obligations; aggregate wrappers still require real coverage/exhaustive certification. Reuse validation is new Phase 5 work, not an existing runner capability.

## Planning and completion instructions

Read this handoff before discussing or planning Phase 5. Allocate each package and every transferred ID to a concrete executable Phase 5 plan with artifact paths, commands and meaningful positive/negative validation. Reuse the existing runner/register/tests and optimize expensive execution only when needed; no wholesale replacement is required. The original implementation guidance is preserved in Git history for 01-15/16/17 before the approved amendment.

Catalog these obligations as required Phase 5 work under the existing TEST IDs, alongside the delivered menu/LAN/content foundations. Update current register/case ownership during implementation; leave historical reports and completed summaries unchanged. Do not mark Phase 5 complete while coverage, either platform or any activated case is missing, failed, skipped or merely planned. Qualitative human judgment remains separate from automated results.
