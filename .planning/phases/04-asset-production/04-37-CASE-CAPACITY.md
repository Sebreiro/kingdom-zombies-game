# Coordinator proposal C2 — explicit bounded proof capacity

Status: independently approved; one bounded implementation attempt authorized.

## Observed reason

The expanded `engine.generated-boundary` target passes all assertions in the unchanged compiled diagnostic run: 1 passed, zero failed/skipped, target 68,631 ms and outer 69,394 ms. Diagnostic identity is `.phase4/diagnostics/04-37-c60922a3-ef11-4611-934f-ac194cbdc961/`, revision `f17be05`, input `a4a23713d6805215d646406e1c53bb55b284b05ab7ce57e9e05089543defd917`. It archived nine child logs and left zero owned processes. The canonical 45-second run `ebf178ee` remains failed and receives no pass credit.

The new boundary workload includes all original/current span and registry negatives plus real producer snapshots, nonproducer-change validation and historical reproduction. `tools/phase1/check.cjs` currently hardcodes MTP `--timeout 45s` and a manifest-wide outer fallback. No game performance criterion requires this source proof to finish in 45 seconds.

## Proposed bounded change

Add an optional validated **per-case** `childTimeoutMs` declaration to the existing runner. A declared override must be a safe positive integer no greater than 120,000 ms. Derive a bounded MTP timeout from that same override, reserving sensible outer-process/reporting headroom; keep the existing behavior when the per-case field is absent. Never pass unchecked values to a process argument or timer.

Declare `childTimeoutMs: 120000` only for the existing `engine.generated-boundary` case. Do not add a case, alter its selector/requirements/expected diagnostic, delete a negative, change a game performance limit or introduce a global timeout increase. The old diagnostic stays diagnostic-only; only a fresh canonical run with the new explicitly fingerprinted declaration can supply acceptance.

## Exact scope amendment before edits

This verification-capacity correction may touch three additional existing paths:

- `tools/phase1/check.cjs`: validated case budget and corresponding MTP/owned-child deadline.
- `tools/phase1/required-cases.json`: the single case declaration above.
- `tests/phase1/RunnerContractTests.cs`: meaningful existing-runner checks for valid/default/invalid per-case budgets, actual selected execution and bounded cleanup as appropriate.

The substep remains at three implementation files. The cohesive plan's total scope becomes twelve existing paths rather than the earlier nine-file planning heuristic; this explicit coordinator exception is based on the measured workload and preserves all proof content. Record the amendment and actual APIs in the final inventory. Node-repair decomposition stays in memory: do not rewrite PLAN.md or add a phase/plan/task solely for this capacity adjustment. No frozen03 or producer/policy source edit is authorized by C2.

## Required proof and budgets

R2's behavioral repair budget remains zero. C2 permits no further producer-proof design change. After C2 is independently approved, allow one implementation attempt and focused verification; on failure, return the exact outcome without silently retrying or extending a deadline again.

Verify valid/default/invalid timeout declarations and retained runner timeout/cleanup behavior, then run the **canonical** `engine.generated-boundary`. If it passes, R3 remains authorized to run the fresh entire 18-case selection, all newly affected runner controls, restored native/solution/Core positives and exact frozen hashes. A diagnostic pass, missing report, zero discovery or timeout supplies no acceptance. Preserve all earlier outcomes and disclose the measured before/after capacity change.

All ASSET and visual/listening/judgment acceptance remain pending. No real game window, new dependency or concurrent-work modification is authorized here.

## Independent approval

[Capacity review](04-37-CAPACITY-CHECK.md) passes with zero blockers or warnings. The reviewer inspected the actual hardcoded MTP/global fallback and measured diagnostic; it accepted the three-path correction and explicit twelve-file scope exception. Valid/default/invalid selected execution, nonleakage between cases, coherent MTP/outer headroom and owned cleanup remain implementation duties. No assertion, timeout or canonical pass was executed by that review.

The coordinator authorizes C2 under the user's existing autonomous Phase 4 request. R2 remains zero; C2 supplies only bounded verification capacity. On a failed C2 implementation/focused gate, preserve the outcome and return without silently spending a second attempt or enlarging the cap. R3 remains at its recorded budget after a successful C2 canonical check.
