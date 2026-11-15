# Phase 3: Four-player LAN foundation — Pattern Map

**Mapped:** 2026-11-15
**Scope:** CONTEXT, SPEC and RESEARCH; proposed names below are planning assignments, not implemented APIs.
**Tracked-source gate:** Analog paths verified with `git ls-files`; no installed capability mirrors.

## File Classification

| New/modified file or explicitly bounded family | Role | Data flow | Closest tracked analog | Match |
|---|---|---|---|---|
| `src/core/FoundationSession.cs` | service | event-driven | same file | exact |
| `src/core/FoundationMovementCommand.cs`, `FoundationActorState.cs` | model | transform | existing same files | exact |
| `src/core/FoundationSnapshot.cs`, participant model(s) | model | transform | `src/core/ExampleStateStore.cs` | partial |
| `src/multiplayer-abstractions/IDiscoveryService.cs`, `IIdentityService.cs`, `IConnectionService.cs`, framework-only DTOs | service/model | request-response/event-driven | `src/multiplayer-abstractions/IProofService.cs` | role |
| `src/godot-game/foundation/FoundationCoordinator.cs` | service | event-driven | `src/core/FoundationSession.cs` | partial |
| `src/godot-game/foundation/FoundationProtocol.cs`, wire DTOs | utility/model | transform | `src/core/ExampleStateStore.cs` | partial |
| `src/godot-game/foundation/EnetConnectionService.cs` | service | event-driven | `src/godot-game/foundation/FoundationRoot.cs` | partial |
| `src/godot-game/foundation/LanDiscoveryService.cs` | service | pub-sub | none | none |
| `src/godot-game/foundation/LocalProfileService.cs` | service | file-I/O | none | none |
| `src/godot-game/foundation/FoundationRoot.cs`, `Foundation.tscn` | controller/component | event-driven | existing root | exact |
| `src/godot-game/foundation/FoundationInput.cs` | utility | event-driven | existing root input route | role |
| `src/godot-game/foundation/FoundationView.cs`, `FoundationWorld.tscn` | component | transform | existing view/root | exact |
| Phase3 authority, coordinator, substitution tests in `tests/phase1/` | test | event-driven | `tests/phase1/FoundationPermissionTests.cs` | role |
| `tests/phase1/ArchitecturePolicy.cs`, `EngineAccessPolicy.cs`, `FoundationPermissionTests.cs` | utility/test | batch | existing same files | exact |
| `docs/code/visibility-exemptions.json`, exact generated permission registers if build changes them | config | batch | existing exact registries | exact |
| `tests/phase1/KingdomZombies.Phase1.Tests.csproj` | config | batch | existing explicit linked-source composition | exact |
| `tools/phase3/check.cjs`, `evidence.cjs`, `required-cases.json` | utility/config | batch | `tools/phase2/evidence.cjs` | exact |
| `tools/phase3/rendered.cjs`, owned Windows input helper/scenario files | utility | event-driven/file-I/O | `tools/phase2/check.cjs` | role |

Families deliberately leave final helper/DTO names to the planner; enumerate each concrete file in PLAN.md. Preserve the existing project inventory and update relevant focused documentation indexes with the implementation.

## Pattern Assignments

### Pure authority, command and actor publications

Copy `src/core/FoundationSession.cs:1-15` for `System` imports, file-scoped namespace, XML documentation, named arguments and constructor validation. Keep trusted elapsed input outside client payloads. Preserve ordinal identity checks (`38-43`):
```csharp
if (!StringComparer.Ordinal.Equals(x: command.SessionId, y: Actor.SessionId)
        || !StringComparer.Ordinal.Equals(x: command.PlayerId, y: Actor.PlayerId)
        || !StringComparer.Ordinal.Equals(x: command.ActorId, y: Actor.ActorId))
{
    throw new ArgumentException(message: "FOUNDATION_OWNER: command ownership mismatch",
            paramName: nameof(command));
}
```
Copy `51-56` for checked integer movement, bounds and replacement of immutable actor state. Extend this one authority for roster/readiness/sequence checks; coordinator sender binding is additional validation, not a replacement for Core ownership checks. `22-25` fences closed lifetimes before teardown.

### Detached snapshots and protocol mapping

`src/core/ExampleStateStore.cs:31-42` is the publication analog:
```csharp
    cancellationToken.ThrowIfCancellationRequested();
    ExampleState candidate = SnapshotCodec.Decode(document: document);
    beforeCommit?.Invoke(obj: candidate);
    cancellationToken.ThrowIfCancellationRequested();
    Interlocked.Exchange(location1: ref _root, value: candidate);
    return candidate;
```
This excerpt is the try-body (`33-38`); the real finally releases the writer (`40-43`). Copy detached preparation and single publication, not proof DTO names. Validate complete roster uniqueness, bounds, content, session and revision before installation. There is no existing network envelope/baseline/ack implementation; use RESEARCH for those semantics.

### Independent service ports and provider composition

`src/multiplayer-abstractions/IProofService.cs:1-9` supplies the independent namespace/interface convention:
```csharp
namespace KingdomZombies.Multiplayer.Abstractions;
// Existing interface signature:
// public interface IProofService
// int Echo(int value);
```
Create three narrow ports and their own framework-value DTOs. Neither contracts nor Core may reference the other. Map explicitly in game composition. Keep engine-free coordinator/protocol/provider substitutes testable through exact linked Compile inputs in the existing test project; do not add a sixth project or broad source glob.

### Godot adapters, menu, input and actor projection

Use `src/godot-game/foundation/FoundationRoot.cs:1-9` for import order (`System`, `Godot`, project namespaces). Every native method/helper requires its authentic preceding gateway guard; `_Ready:21-33` connects button/focus events, `_ExitTree:102-114` disconnects after closing the session. Menu return (`168-177`):
```csharp
_gateway.RequireThread();
_session?.Close();
_session = null;
_input.Clear();
_remainder = 0;
Node2D? world = GetNodeOrNull<Node2D>(path: "World");
```
The following block removes and queues the world. Extend closure to transport callbacks, reservations and discovery before native teardown. Copy `Start:136-162` for recoverable entry and visible error rollback. Replace the current two-button Enter fallback (`57-70`) with explicit screen/focus dispatch. Preserve focus clearing (`195-199`), aliases and release latches.

`src/godot-game/foundation/FoundationView.cs:12-19` is the projection analog:
```csharp
gateway.RequireThread();
Node2D player = world.GetNode<Node2D>(path: "Player");
float x = actor.PositionSubunits * 16f / 1000;
player.Position = new Vector2(x: x, y: 540);
player.GetNode<Polygon2D>(path: "Facing").Scale = new Vector2(x: actor.Facing, y: 1);
```
Replace the single Player lookup with actor-keyed presentations; project the camera once from the local actor. Retain current fixture dimensions and camera smoothing (`19-25`); follow UI-SPEC for new native controls/markers. No existing raw-ENet adapter exists: reuse guards/lifetimes, not imagined transport APIs.

### Exact permissions and adjacent rejection

`tests/phase1/ArchitecturePolicy.cs:22-44` independently compares exact public registry entries and rejects `R-34: invalid or expanded exact visibility registry`. Update both exact expectations and `docs/code/visibility-exemptions.json` for necessary public types only.

`tests/phase1/EngineAccessPolicy.cs:96-119` matches exact path, namespace, type and method, then authentic preceding `MainThreadGateway.RequireThread`. Add only actual native helpers; no wildcard approval. `tests/phase1/FoundationPermissionTests.cs:27-31` provides positive/negative pairing:
```csharp
EngineAccessPolicy.Check(source: Prefix + "gateway.RequireThread(); " + Body + Suffix, path: Path,
        consumer: consumer).ShouldBeEmpty();
EngineAccessPolicy.Check(source: Prefix + Body + Suffix, path: Path, consumer: consumer)
```
The continuation asserts rejection. Retain adjacent type/method, wrong namespace, unguarded/late guard and async misuse controls (`41-90`), visibility neighbor (`93-103`) and every-method guard mutations (`106-130`). Refresh generated spans from actual strict build output only.

### Runner, scenarios and current evidence

Copy `tools/phase2/check.cjs:10-23` selection/locking and `40-120` orchestration/finalization, not old case IDs. `tools/phase2/evidence.cjs:28-35` provides set-based reconciliation:
```javascript
requireTrue(selected.length > 0 && results.length === selected.length, 'RESULT_COUNT');
const expected = new Map(selected.map(row => [row.id,row])), seen = new Set();
requireTrue(expected.size === selected.length, 'SELECTION_DUPLICATE');
```
Extend identity to exact case plus peer role; four real participants cannot be replaced by duplicate results. Preserve nonce/input/tool/platform checks, PNG decoding and state/image hashes (`35-57`), fresh report order and appearance review (`77-106`). The tracked `tools/phase2/required-cases.json` independently declares tests and checkpoint images.

`tools/phase2/rendered.cjs` is the scenario integration reference: `createSession:79-135`, `tracer:137-291`, `runScenarios:340+`. Preserve owned process cleanup and real child failures; use a persistent owned driver for overlapping four-peer input. Current local policy is Windows 1280×720 after complete implementation and review, without a VM; do not restore superseded platform/resolution execution from older phase artifacts.

## Shared Patterns and Gaps

- Validation precedes mutation; use ordinal opaque identities, explicit dependencies and immutable publication.
- Native operations stay behind exact guarded methods; close acceptance before cancellation/disconnect/free.
- No production analog exists for discovery TTL, atomic profile persistence, connection binding, network revision/readiness or multi-peer timing. Implement those from RESEARCH/SPEC and prove substitutions and failing controls; existing proof examples do not establish LAN correctness.
- Search scope: tracked `src/`, `tests/phase1/`, `tools/phase2/`, `docs/code/`; five primary excerpt families: authority, state publication, service port, Godot presentation, evidence reconciliation. Permission/scenario references are supporting integration points.
