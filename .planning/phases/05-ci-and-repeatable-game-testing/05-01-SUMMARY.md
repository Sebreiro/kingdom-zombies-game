---
phase: 05-ci-and-repeatable-game-testing
plan: "01"
subsystem: verification
requirements-completed: [TEST-01, TEST-03, TEST-07, TEST-08, TEST-09, TEST-10]
---

# Plan 05-01 summary

The shared entry point validates and lists a 295-row catalog, including A-01 through A-47 and the existing Phase 1–4 suites. It provides exact case and group selections, three tiers, a basic pull-request selection, an affected phase-end selection and a full active selection. Catalog rows retain their original suite identities, phase ownership, importance, ordered actions, run groups and evidence obligations.

The runner records a clean source revision, owned invocation, selected and omitted rows, subcommand output and per-case outcomes. A failure names the expected and observed result, evidence and exact replay command. Focused repair selects failed and blocked rows from the immutable prior report. Verification groups contribute only when their declared input or case changes.

The selection controls cover unchanged Critical smoke, changed Low, an affected consumer, a planned Critical row, analyzer versus gameplay-only changes, invalid selectors and damaged declarations. The eight pending Phase 1 certification declarations move to Phase 11. The existing legacy exhaustive selector still includes all declarations accepted at or before its argument and distinguishably reports the new 678 and 686 Phase 5 and Phase 11 totals.

See [the shared runner](../../../docs/workflow/shared-game-tests.md), [the initial result](05-INITIAL-RUN.md) and [the phase evidence](05-VERIFICATION.md).
