# Pure rules, service ports and engine composition

Status: Accepted implementation decision; qualitative accuracy review Pending.

## Context

The game requires replaceable multiplayer services and rules independent of Godot objects. The donor web/database topology does not apply.

## Decision

Keep exactly the [five declared projects](../../../src/godot-game/tools/verification/projects.json). Core holds pure state contracts; Multiplayer.Abstractions holds service ports; the existing Godot project composes them. The analyzer and Phase1.Tests projects provide tooling and proof. Enforce evaluated direct/transitive references and compiled type surfaces with the [architecture policy](../../../src/godot-game/tests/verification/ArchitecturePolicy.cs) and [type scanner](../../../src/godot-game/tests/verification/TypeBoundaryScanner.cs). The test-only Core inspection edge does not grant product dependencies.

## Consequences

No LAN provider, account, HTTP host or gameplay is delivered here. Future adapters must respect the same directions and remain outside Core. [Checked project pages](../boundaries.md) reconcile actual references and both analyzer-only metadata fields; qualitative review remains Pending.
