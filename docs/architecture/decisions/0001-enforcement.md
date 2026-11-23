# Executable enforcement and bounded adaptations

Status: Accepted implementation decision; qualitative accuracy review Pending.

## Context

The [phase context](../../../.planning/phases/01-code-rules-and-documentation/01-CONTEXT.md) requires executable rules, exhaustive donor traceability and small Godot-compatible examples. Duplicating code policy in prose would create competing authorities.

## Decision

Use the [shared build](../../../src/godot-game/Directory.Build.targets), twelve real Roslyn analyzers and C# structural checks indexed by the [rule register](../../code/register.json). Require distinct compliant and intended-negative proofs. Authenticate generated permissions against exact producer, source, path, member and span identities; retain only [registered exemptions](../../code/exemptions.json). Qualitative obligations stay separately Pending in the [review checklist](../../code/review-checklist.md).

## Consequences

The creator's 2026-11-14 amendment preserves all working enforcement and uses focused feedback while batching Phase 1 closure work, then one final complete Phase 1 Windows run and representative actual Linux checks. A build or raw test pass is insufficient without the existing fresh-result reconciliation. New engine output still needs bounded permissions and boundary proofs. The [current rule index](../../code/rule-matrix.md) reconciles real bindings and [human review](../../code/review.md) remains separate. Measured 90%/80% coverage and exhaustive platform/runner certification are required Phase 11 work under TEST-12 since the 2026-11-23 amendment, through the [Phase 5 handoff](../../../.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md).
