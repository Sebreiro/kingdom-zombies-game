# Proven development baseline

Status: Accepted implementation decision; human accuracy review Pending.

## Context

Phase 1 needs strict Godot-compatible rules and small trustworthy proofs before gameplay. The donor's web/database topology and HTTP defaults do not fit independent game state. D-01–D-18 require executable definitions, exact provenance, focused references and separate qualitative review.

## Decision

Keep the observed Windows combination: Godot .NET 4.7.2 `ed1daf0bf`, SDK 10.0.404, compiler `5.9.0-1.26529.119`, C# 14.0, net10.0/runtime 10.0.15, editor runtime 8.0.16 and Node 24.22.1. [Toolchain authority](../../../tools/phase1/toolchain.json), [central analyzer pins](../../../src/godot-game/Directory.Packages.props) and five exact LF locks control acquisition/build. The actual headless renderer is `gl_compatibility`; this is not rendered gameplay acceptance. [Initial observed evidence](../../../.planning/phases/01-code-rules-and-documentation/01-01-SUMMARY.md) and subsequent [compiler evidence](../../../.planning/phases/01-code-rules-and-documentation/01-18-SUMMARY.md) establish these selections rather than starter feature tags. Stable engine/runtime passed without a downgrade or extra native test framework.

Keep exactly Core, independent multiplayer ports, the existing Godot composition, reusable analyzers and Phase 1 proofs. [Project policy](../../../tools/phase1/projects.json) and [checked module pages](../boundaries.md) define dependencies; analyzer-only metadata never grants a product reference. Twelve custom SCR analyzers plus compiler/third-party/configuration/compiled checks retain error severity and exact [registered exemptions](../../code/exemptions.json). [Rule index](../../code/rule-matrix.md) connects current source symbols/settings and distinct real cases.

Use checked integer milliseconds, bounded runtime-pinned replay randomness and exact ordinal identities. R-70 intentionally diverges from permissive HTTP JSON: internal snapshots reject unknown/null/duplicate fields, absent required state and unsupported versions before atomic replacement. Bytes/depth/records are inclusively bounded at 1,048,576/16/1,024. No production save UX is implied.

Enforce inclusive sixty body lines and 120 UTF-16 line units; forty body lines remains advice. Require four-space indentation/tab width and eight-space continuation offsets, exact counting without rounding, and nearest representable boundary proofs. Preserve the inclusive 90% line / 80% branch coverage targets for both analyzer and native-hosted managed production scopes. Under the creator's 2026-11-14 amendment, actual collection/instrumentation and threshold/report acceptance are required Phase 5 work; 01-15 now migrates explicit ownership and adds Phase 1 selection. The existing 18.12.0 tool pin is unchanged. [Numeric definitions](../../code/register.json) and [threshold proofs](../../../src/godot-game/tests/verification/ThresholdTests.cs) retain their exact operational semantics.

## Consequences

Keep tested inputs fixed during each run and preserve the existing fresh-result reconciliation. Batch related closure work with focused checks, then obtain one final complete functional Windows run in 01-16 and representative actual clean Linux checks in 01-17. Explicit Phase 5 certification obligations remain pending without Phase 1 pass/skip credit; selected Phase 1 failures stay blocking. Qualitative reviewer/date/outcome remain separately Pending. Commands, preparation or cross-builds supply no Linux pass. Planning/prose metadata updates receive separate docs and consistency validation.

[Setup](../../workflow/setup.md) and [verification](../../workflow/verification.md) own commands. [Human review](../../code/review.md) owns semantic decisions. [Interactive collaboration](../../workflow/ai-collaboration.md) remains indexed with text mode disabled. Future pin, graph, generator or snapshot changes need renewed actual evidence and bounded adaptation; neither broad suppressions nor waived coverage are authorized.
