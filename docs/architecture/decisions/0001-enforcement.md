# Executable enforcement and bounded adaptations

Status: Accepted implementation decision; qualitative accuracy review Pending.

## Context

The [phase context](../../../.planning/phases/01-code-rules-and-documentation/01-CONTEXT.md) requires executable rules, exhaustive donor traceability and small Godot-compatible examples. Duplicating code policy in prose would create competing authorities.

## Decision

Use the [shared build](../../../Directory.Build.targets), twelve real Roslyn analyzers and C# structural checks indexed by the [rule register](../../code/register.json). Require distinct compliant and intended-negative proofs. Authenticate generated permissions against exact producer, source, path, member and span identities; retain only [registered exemptions](../../code/exemptions.json). Qualitative obligations stay separately Pending in the [review checklist](../../code/review-checklist.md).

## Consequences

Rule, command and contract changes require the full currently implemented suite. A build or raw test pass is insufficient without fresh exact result reconciliation. New engine output may need observed bounded permissions and adjacent negative proofs. The [current rule index](../../code/rule-matrix.md) reconciles real source/proof bindings; [human review](../../code/review.md), measured coverage and actual Linux acceptance remain required separate gates.
