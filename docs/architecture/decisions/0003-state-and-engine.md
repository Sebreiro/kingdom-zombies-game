# Controlled state and native engine ownership

Status: Accepted implementation decision; qualitative accuracy review Pending.

## Context

The foundation must demonstrate repeatable state and strict snapshots while real Godot construction, injection and teardown remain engine-owned. Compilation alone cannot prove native behavior.

## Decision

Core uses explicit integer elapsed time, bounded replayable randomness and ordinal identities. Strict snapshots validate detached complete values before one atomic immutable root replacement. The [state proofs](../../../tests/phase1/StateInputTests.cs) and [snapshot proofs](../../../tests/phase1/SnapshotTests.cs) exercise the actual code. Native [engine proofs](../../../src/godot-game/proofs/Phase1.cs) require two distinct instances, actual callbacks/signals/serialized values and guarded mutation on the captured engine thread. Each lifetime closes its acceptance fence before cancellation and disconnection; pure worker results return through guarded frame dispatch.

## Consequences

These examples establish contracts rather than production save UX or gameplay. Nullable pre-injection staging is checked before use. Genuine generated source receives exact authenticated span permissions; handwritten partials stay fully checked. Linux and measured coverage remain separate required gates. Plan 01-14 refines the final project/rule/decision mapping and retains qualitative review as Pending.
