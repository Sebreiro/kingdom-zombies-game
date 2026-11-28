# Phase 6 — Survivors, inventory, and combat pattern map

**Mapped:** 2026-11-24

**Source:** Phase 06 context and locked specification, checked against the linked worktree. New filenames below describe proposed ownership and can be refined by the planner.

Paths in the tables are relative to `src/godot-game/` unless they start with `.planning/` or `docs/`.

## File Classification

| Target or proposed file family | Role | Data flow | Closest tracked analog | Match |
|---|---|---|---|---|
| `core/FoundationSession.cs` and new run/survivor state files | Service/model | Event-driven, transform | `core/FoundationSession.cs` | Same authority and tick |
| New Core settings/definitions loader and values | Utility/model | Transform | `core/ContentPack.cs` | Byte-in validation |
| New Core world safety, traversal and combat rules | Service | Event-driven, transform | `core/FoundationSession.cs` | Tick integration |
| New Core inventory state and inventory transaction rules | Model/service | CRUD, event-driven | `game/inspection/InventoryLayouts.cs` | Geometry and identity only |
| New Core corpse, loot, respawn and spectator state/rules | Model/service | Event-driven | `core/FoundationSession.cs`, `game/inspection/CorpsePreview.cs` | Lifetime and presentation only |
| `core/FoundationSnapshot.cs`, new run snapshot DTO/codec and saved-layout records | Model/utility | Transform, file I/O boundary | `core/FoundationSnapshot.cs`, `core/ExampleStateStore.cs` | Immutable validation and atomic replace |
| `game/foundation/FoundationCoordinator*.cs`, `FoundationEnvelope.cs`, `FoundationProtocol.cs` and any new gameplay message/projection files | Controller/utility | Request-response, streaming | `game/foundation/FoundationCoordinator.cs`, `game/foundation/FoundationProtocol.cs` | Same transport and authority |
| `game/foundation/FoundationRoot.cs`, `FoundationInput.cs` and scene/HUD/inventory/spectator controls | Controller/component | Event-driven | `game/foundation/FoundationRoot.cs`, `game/inspection/InventoryLayouts.cs` | Input/frame and inventory presentation |
| `game/foundation/FoundationView.cs`, `FoundationActorNode.cs` and production equipped/corpse adapters | Component | Transform, event-driven | `game/inspection/CharacterPreview.cs`, `game/inspection/CorpsePreview.cs` | Same authored composition |
| New verification tests, Phase 06 stored scenarios, game-tests catalog and scenario driver wiring | Test/config | Batch, event-driven | `tests/verification/FoundationLanOrderingTests.cs`, `tools/game-tests/scenario-group.cjs` | Same proof layers |

## Pattern Assignments

### Run, survivor, world, combat and inventory authority

- `core/FoundationSession.cs:74–101` separates accepting an owned, fresh input from advancing time. Retain the trusted binding/player/actor checks, monotonic sequence and host tick. A gameplay request additionally needs current survivor ownership and action eligibility. Movement intent can be held. Attack, item and Respawn requests need explicit one-time outcomes.
- `core/FoundationSession.cs:26–70` is the admission/readiness/departure seam. Temporary actor creation/removal here must become assignment/release against a separate persistent eight-survivor run population. Keep admission readiness separate from alive/control/spectating state. One fresh-run kit belongs to run creation. Departure invalidates control and pending intents while retaining survivor and inventory. A dead survivor's old intent must never apply to its successor.
- `core/FoundationSession.cs:93–100` is the tick transaction example. Plan one elapsed input for world time, safety, locomotion, recovery, projectiles and lifecycle. Prove refuge/outer-live-wall and hostile danger/reachability before dependent features. Use Core geometry and ordered hit rules with representative defenses and terrain. Keep explicit numeric units, stable equal-contact ordering and injected replayable RNG for successor selection.
- `core/ContentPack.cs:112–151` accepts bytes and an injected reader. Use this boundary for typed configuration, with Godot resource loading in the adapter. Tracked `game/content/config/items.json`, `inventory.json`, `survivors.json`, `combat.json`, `run.json`, `time.json` and `structures.json` supply existing identities and fields. Reconcile with locked Phase 06 baselines before expanding definitions. Gameplay validation must fail on invalid required data.
- `game/inspection/InventoryLayouts.cs:32–86` supplies eleven stable slot identities, rotated dimensions, detached saved geometry and exact target/food-instance lookup. Validation seams at lines 379–479 cover state/item/grid/slot shape. Production authority belongs to Core. Validate against current locations, fit, access, quantities, depth including descendants, cycles and compatibility before committing. Resolve partial merge, split identity, filled-bag moves, paged stash placement and quick food invalidation from the same root. Treat preview/cancel as UI state and recheck current authority at drop.
- `game/inspection/CorpsePreview.cs:70–108` provides a presentation lifecycle only. Core owns one-time death, preserved body/equipment/cargo, empty-only cleanup, the one-second Respawn gate, request order with slot ties and random eligible exclusive assignment. Keep waiting, hold-camera and selected teammate state explicit. Representative stationary debug fixtures should call production rules and keep supplies out of ordinary startup.

```csharp
// core/FoundationSession.cs:93–100
RequireActive();
ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
FoundationParticipantState[] candidate = [.._snapshot.Participants.Select(selector: participant =>
        AdvanceParticipant(participant: participant, elapsedMilliseconds: elapsedMilliseconds))];
_snapshot = new FoundationSnapshot(sessionId: Actor.SessionId, revision: checked(_snapshot.Revision + 1),
        participants: candidate);
```

### Detached snapshots and transport projection

- `core/FoundationSnapshot.cs:13–34` constructs an immutable candidate and validates its shape. `core/ExampleStateStore.cs:23–42` decodes, validates and publishes after the cancellation fence. `core/SnapshotCodec.cs`, `SnapshotDto.cs`, `SnapshotJson.cs` and `SnapshotWriter.cs` are the strict serialization family. Preserve precise values, exact slot/item identities and saved dimensions, footprint, coordinates, orientation and metadata. Validate identity cross-links and ownership across the whole candidate before install. Preserve empty-versus-required semantics and deterministic continuation.
- `game/foundation/FoundationCoordinator.cs:77–135` drains inbound work, steps once, publishes, and routes local submissions through authority or remote submissions over the existing transport. Adapt the method at this seam for trusted discrete action requests and corrected results. Review `FoundationCoordinator.GuestPublication.cs:12–80` for baseline admission and monotonic installation, and preserve admission/lifetime handling.
- `game/foundation/FoundationProtocol.cs:13–55,82–149` sets the existing 32,768-byte ceiling and validates envelope and snapshot shape. Define separate coherent public and recipient-specific projections for LAN, and a complete detached durable snapshot. Review a filled/deep inventory and many stash pages against limits before wiring. Access permission belongs in the host projection before serialization as well as in mutation authorization. Send only the authorized local external target/page and necessary metadata. Correlate corrections without disclosing another player's belongings.

```csharp
// core/ExampleStateStore.cs:33–38
cancellationToken.ThrowIfCancellationRequested();
ExampleState candidate = SnapshotCodec.Decode(document: document);
beforeCommit?.Invoke(obj: candidate);
cancellationToken.ThrowIfCancellationRequested();
Interlocked.Exchange(location1: ref _root, value: candidate);
return candidate;
```

### Godot input, UI, and world projection

- `game/foundation/FoundationRoot.cs:56–96,96–145,292–320` is the composition and frame seam. `FoundationInput.cs:9–50` has held and blocked keys, fresh press, and context transitions. Keep the coordinator update outside panel activity. Opening a panel neutralizes held movement and captures attack, quick food and world clicks. Route Escape through drag, panel and menu in that order. Nonlethal damage preserves browsing.
- `game/foundation/FoundationHud.cs:13` and `FoundationView.cs:14,65` project committed state into engine nodes. Build production panels/HUD on the foundation scene path (`Foundation.tscn`, `FoundationWorld.tscn`), using inventory inspection's `Project` and `Close` hooks (`InventoryLayouts.cs:113–150)) as adapter patterns. Authoritative equipment/stats/food and access changes should update one coherent view. Local drag, bag-window positions, page selection and panel focus stay in presentation. Distinguish corrections, loss of access, quiet empty bow, and waiting/spectating.
- `game/inspection/CharacterPreview.cs:83–156` resolves exact authored body/pose/slot/item identity and replaces fitted composition on the engine thread. `CorpsePreview.cs:234` projects corpse content. Reuse or extract their fit/atlas/layer logic for production living, corpse, bow/arrow, damage/death and crown cues. Choose logical hand orientation from gameplay and carry it through mirroring. Project the crown from current control ownership. Camera follows the current permitted target only, and death/target loss holds the current view until explicit selection or succession.

```csharp
// game/foundation/FoundationRoot.cs:104–113
double exact = delta * 1000 + _remainder;
long elapsed = checked((long)exact);
_remainder = exact - elapsed;
if (_coordinator?.Ready == true && _lastDirection != _input.Direction)
{
    _coordinator.Submit(direction: _input.Direction);
    _lastDirection = _input.Direction;
}

_coordinator?.Update(elapsedMilliseconds: elapsed);
```

### Verification and scenario wiring

- Preserve `FoundationLanAuthorityTests.cs:18` (`Accept_OwnedIntent_SucceedsBeforeElapsedStep`), `FoundationLanOrderingTests.cs:16` (`Submit_RejectedCommands_LeaveIntentAndHighWaterUnchanged`), `FoundationLanInputTests.cs:15` (`Input_ScreenFocusAndLocalProjection_StayIndependent`) and `FoundationLanLifecycleTests.cs:16` (`Lifetime_CleanupAndDeadline_RespectBoundaries`) under `tests/verification/`. Preserve admission, services, protocol, permissions, capture publication and movement coverage by migrating baseline expectations only where persistence changes the intended semantics.
- Extend the exact identity, footprint and detached geometry cases in `tests/verification/InventoryLayoutTests.cs:16–66` with pure transaction proofs for conservation, invalid moves, conflicts, carried-only food, nested load and bounds. Extend the approach in `SnapshotTests.cs:19–79,197–226,294–379` and `RestoreAtomicityTests.cs:17–165` for complete continuation, malformed candidates, stable reads, and failed restoration. Preserve the existing example codec tests.
- Use `tools/game-tests/catalog.cjs:258–319,345–423,504` and tracked `catalog.json` for scenario support, phase activation, selectors and input coverage. `tools/game-tests/scenario-group.cjs:10–46` consumes planned rows, executes the driver, and returns row results. Store Phase 06 scenario definitions in the phase directory and use the common selector/runner. Preserve transport driver behavior and prior scenario coverage. Give P01/P02 (AC-40/41) wired negative and clean controls through production command/projection paths. Record pure/build and grouped post-review solo/four-process Windows evidence under the local testing strategy. Keep acceptance state distinct from implemented cases and saved scenario definitions.

## Shared Patterns

- **Architecture:** Follow `docs/architecture/decisions/0002-boundaries.md` and `0003-state-and-engine.md`. Preserve the five-project graph. Pure domain logic sees explicit values, elapsed inputs, ordinal identities and controlled RNG. Transport ports carry messages. Godot holds callbacks, nodes, resource reads, input, camera, drawing and lifetime. Guard native access through `game/proofs/MainThreadGateway.cs` and close acceptance fences before teardown.
- **Existing C# enforcement:** Run the repository compiler/analyzers and architecture/source proofs. Use existing named argument, member and import ordering, body/line, indentation and null rules. Keep one purpose per type, narrow public APIs and preserve existing source scope. Check `docs/code/README.md` and the actual executable rules it indexes.
- **Command failure and publication:** Resolve once against trusted current state. An invalid request must leave gameplay unchanged and yield a scoped reason and current permitted state. Keep duplicate protection distinct from committed gameplay revision. Host processing order must remain meaningful for contested stock, damage and successor assignment. Give private views the same reviewed implementation path for host-local and guests.

## No Analog Found

| Target | Role / flow | Missing production pattern |
|---|---|---|
| Safety, vertical movement, melee/projectile collision, stamina/condition/recovery | Service / event-driven | Existing engine movement is horizontal. Use locked specification and representative fixtures for the new rules. |
| Persistent item transaction and large stash paging | Service / CRUD | Inspection samples cover shape and display. Production atomic ownership and bounded recipient page publication are new. |
| Persistent survivor leasing, explicit Respawn queue and teammate spectating | Service/component / event-driven | Admission and teardown cover transport lifetime. The controlled survivor and camera state machines are new. |
| Phase 06 acceptance descriptors, controls and driver events | Test / batch | Phase 05 infrastructure is present. Phase 06 must supply its own executable cases. |

## Metadata

**Analog search scope:** Core, foundation and inspection adapters, tracked production config, verification tests and the unified game-tests runner.

**Inputs:** `06-CONTEXT.md` and `06-SPEC.md`. The linked worktree had no Phase 06 research document at the time of this map.
