# Required qualitative rule review

**Current Phase 1 closure boundary (creator instruction, 2026-11-14):** The [one-time exception](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) accepts the retained complete Windows baseline plus actual focused Linux strict/native/rule/state/graph evidence without a host restart. The narrow GODOT_LINUXBSD test correction passed separately on both OSes. A complete Linux suite, its original graph capacity failure and fourteen individually unreviewed human components remain explicit [follow-ups](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md), not passes. The complete-other-OS-VM rule continues for later phases.

Status: Pending. This is the phase-end human checklist required by D-04 and plan 01-14. Automated structure, accepted proof reports and AI review cannot supply the human outcome. [Current rule index](rule-matrix.md) links actual definitions and distinct machine cases; [register](register.json) owns the qualitative components.

| Exact rule | Human component | Inspect |
| --- | --- | --- |
| R-24 | Class-based DTO/engine initialization expresses required values and suitable semantics | [Snapshot DTO](../../src/godot-game/core/SnapshotDto.cs), [engine scene](../../src/godot-game/game/proofs/Phase1.cs) |
| R-34 | Every public API is necessary and minimal | [Exact visibility permissions](visibility-exemptions.json), public C# surfaces |
| R-62 | Reusable tooling/package content is product-independent in intent | [Analyzer project](../../src/godot-game/tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj), [compiled isolation](../../src/godot-game/tests/verification/GraphTests.cs) |
| R-63 | Invariants and state-changing methods preserve intended domain meaning | [Example state](../../src/godot-game/core/ExampleState.cs), [structural model policy](../../src/godot-game/tests/verification/ArchitecturePolicy.cs) |
| R-65 | Handwritten mappings preserve semantic intent | [Mapping policy/proofs](../../src/godot-game/tests/verification/ArchitectureRuleTests.cs), changed mapping methods |
| R-72 | Strict snapshot contract covers actual call sites and justified HTTP divergence | [Snapshot codec](../../src/godot-game/core/SnapshotCodec.cs), [baseline decision](../architecture/decisions/0001-development-baseline.md) |
| R-73 | Controlled time/random choices fit game semantics and replay compatibility | [State proofs](../../src/godot-game/tests/verification/StateInputTests.cs), [random contract](../../src/godot-game/tests/verification/RandomContractTests.cs) |
| R-74 | Unknown sentinel rejection matches intended valid state | [ProofKind](../../src/godot-game/core/ProofKind.cs), actual state validation |
| R-77 | Enum evolution preserves meanings as well as pinned names/numbers | [Enum policy](../../src/godot-game/tests/verification/ArchitecturePolicy.cs), [snapshot proofs](../../src/godot-game/tests/verification/SnapshotTests.cs) |
| R-78 | DTOs preserve independent values and exclude transient native/process/service identity | [Snapshot shape policy](../../src/godot-game/tests/verification/SnapshotShapePolicy.cs), actual DTO members |
| R-79 | Remote health checks are reasonedly inapplicable to this phase | [Exact inapplicable disposition](register.json), five actual project responsibilities |
| R-81 | Model/options naming, cohesion and initialization intent fit responsibilities | [Model policy](../../src/godot-game/tests/verification/ArchitecturePolicy.cs), actual selected types |
| R-83 | Focused names/indexes are accurate, concise and useful | [Agent guide](../../AGENTS.md), [code](README.md), [boundaries](../architecture/boundaries.md), five module pages |
| R-84 | Decisions/adaptation reasons/consequences accurately describe actual evidence | [Architecture decisions](../architecture/README.md), [baseline](../architecture/decisions/0001-development-baseline.md), original register/provenance |

At phase end, inspect all rows against the frozen tested revision, source and fresh proof evidence. Record each row's reviewer, date, evidence and explicit outcome in the [human-check artifact](../../.planning/phases/01-code-rules-and-documentation/01-HUMAN-CHECK.md). Identify any required corrections before granting the human pass. Do not fill the reviewer/date/outcome with an automated result or an inferred approval. The final functional Windows and representative actual Linux baseline precede this review. Measured coverage and exhaustive certification are Phase 5 work under the creator's 2026-11-14 amendment and do not postpone the Phase 1 qualitative review.

The [short checklist](review-checklist.md) remains the focused entry. [AI collaboration](../workflow/ai-collaboration.md) and [GSD workflow](../workflow/gsd.md) retain the indexed interactive preference and text mode disabled. This checklist does not authorize a scope reduction or waive any machine gate.
