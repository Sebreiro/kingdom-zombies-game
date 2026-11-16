# Controlled state and native engine ownership

Status: Accepted implementation decision; qualitative accuracy review Pending.

## Context

The foundation must demonstrate repeatable state and strict snapshots while real Godot construction, injection and teardown remain engine-owned. Compilation alone cannot prove native behavior.

## Decision

Core uses explicit integer elapsed time, bounded replayable randomness and ordinal identities. Strict snapshots validate detached complete values before one atomic immutable root replacement. The [state proofs](../../../src/godot-game/tests/verification/StateInputTests.cs) and [snapshot proofs](../../../src/godot-game/tests/verification/SnapshotTests.cs) exercise the actual code. Native [engine proofs](../../../src/godot-game/game/proofs/Phase1.cs) require two distinct instances, actual callbacks/signals/serialized values and guarded mutation on the captured engine thread. Each lifetime closes its acceptance fence before cancellation and disconnection; pure worker results return through guarded frame dispatch.

## Consequences

These examples establish contracts rather than production save UX or gameplay. Nullable pre-injection staging is checked before use. Genuine generated source receives exact authenticated span permissions; handwritten partials stay fully checked. [Current bindings](../../code/rule-matrix.md) index the actual definitions. The representative actual Linux baseline and [human judgment](../../code/review.md) remain Phase 1 requirements; measured coverage and exhaustive certification belong to Phase 5 under the creator's 2026-11-14 amendment. Preserve all already implemented state and lifetime behavior.
