# Phase 7: Camp economy and development - Pattern Map

**Mapped:** 2026-11-18
**Scope:** 23 candidate file/family assignments; paths below are design candidates, not delivered Phase 6 APIs.
**Status:** Ready for dependency-aware planning; predecessor production delivery remains an implementation prerequisite.

**Context reconciliation, 2026-11-18:** Rechecked against the current D-01–D-18 labels. Planning normalized only label punctuation from `D-NN.` to `D-NN:` for the decision parser; all decision wording and the source mappings below remain unchanged. The independent revised plan check also verifies the applicable analogs in executable actions.

## Required implementation-start read and rebind

Before writing gameplay, read the delivered Phase 6 authority, command, controlled-survivor identity, inventory ownership/placement/transfer, reachability/safety, movement, food/ammunition, damage/projectile/corpse, death/Respawn, input-context and detached snapshot APIs and their focused proofs. Record actual files, signatures and ownership in each implementation plan. Rebind every proposed Camp/Npc component below to those common operations. Current FoundationSession implements participant movement; it does not establish production inventory or combat acceptance.

Read delivered Phase 5 catalog, CLI selection, independent case discovery, review gate, evidence identity and grouping schemas before registering Phase 7 cases. Replace selector placeholders with verified executable selectors; do not assume a `--phase 7` switch. Re-read final Phase 4 packaged definitions, resources and text contracts alongside the final Phase 6 Stash workspace. Preserve D-01–D-18 when old samples disagree.

This map changes no source, branch, index or acceptance status. No tests or game windows were launched. The active Phase 4 checkout contains unrelated work; implementation must follow the phase branch/PR workflow separately.

## File Classification

Rows describe candidate files or explicitly named families. A family may split into small files after predecessor inspection; avoid a new monolithic session or scene script. Match quality describes reusable mechanics, not equivalent camp behavior.

| New/modified candidate | Role | Data flow | Closest tracked analog | Match quality |
| --- | --- | --- | --- | --- |
| `src/core/CampTransactions.cs` | service | request-response / CRUD | `src/core/FoundationSession.cs` | role-match; movement authority only |
| `src/core/CampOrders.cs` | model/service | event-driven / CRUD | `src/core/ExampleStateStore.cs` | partial; commit mechanics |
| `src/core/CampProjection.cs` | model | transform | `src/core/FoundationSnapshot.cs` | role-match |
| `src/core/CampTime.cs` | service | event-driven / batch | `src/core/FoundationSession.cs` | partial; explicit elapsed only |
| `src/core/CampTerritory.cs` | service | transform | none | no live border analog |
| `src/core/CampResources.cs` | service | event-driven / CRUD | none | no finite resource/admission service |
| `src/core/NpcJobs.cs` | service | event-driven | none | no production NPC policy |
| `src/core/NpcEquipment.cs` | service | CRUD | none | requires delivered Phase 6 operations |
| `src/core/HaulingReservations.cs` | model/service | CRUD / event-driven | none | no physical hauling implementation |
| `src/core/CampDefinitions.cs` | model/utility | file-I/O / transform | `src/core/ContentPack.cs` | role-match; typed gameplay validation new |
| `src/core/CampSnapshot*.cs` or delivered snapshot extension | model/utility | transform | `src/core/FoundationSnapshot.cs`, `src/core/ExampleStateStore.cs` | role-match; schema must extend Phase 6 |
| Godot camp command/coordinator extension | controller | request-response | `src/godot-game/foundation/FoundationCoordinator.cs` | role-match |
| Proposed Godot `CampWindow.cs` | component/controller | event-driven | `src/godot-game/inspection/CampLayouts.cs` | role-match; inspection only |
| Proposed Godot `CampPeopleView.cs` | component | event-driven / transform | `src/godot-game/inspection/CampLayouts.cs` | role-match |
| Proposed Godot `CampSalesView.cs` | component | request-response / transform | `src/godot-game/inspection/CampLayouts.cs` | partial; interaction differs |
| Proposed Godot `CampSiteView.cs` | component | event-driven / transform | `src/godot-game/inspection/CampLayouts.cs` | role-match; repair semantics differ |
| Proposed Godot `CampWorldPresenter.cs` | component | event-driven / transform | none | world work/retreat cues new; use delivered presenters |
| Delivered input-context extension | controller | event-driven | `src/godot-game/foundation/FoundationInput.cs` | role-match; movement only |
| Existing `content/config/{time,jobs,food-wildlife,map-sites,structures,towers}.json` | config | file-I/O | same tracked domain files | exact domain |
| Existing `content/ui/camp-panels.json`, `content/text/{en,expanded}.json` and field docs | config | file-I/O / transform | same tracked resources | exact domain; production bindings new |
| `tests/phase1/Camp{Transaction,Time,Npc,Structure,Snapshot}Tests.cs` | test | request-response / event-driven / transform | `tests/phase1/FoundationMovementTests.cs` | role-match |
| Phase 7 native/headless/network drivers in existing Godot/test infrastructure | test/controller | event-driven / request-response | `src/godot-game/foundation/FoundationCoordinator.cs` | partial; delivered production driver must be rebound |
| Delivered Phase 5 catalog extension plus proposed Phase 7 scenarios/selection/review artifacts | test/config | batch / file-I/O | `tools/phase3/catalog.cjs`, `tools/phase3/required-cases.json` | role-match; not final Phase 5 schema |

## Pattern Assignments

### Core transactions, orders, time and projections

**Primary analog:** `src/core/FoundationSession.cs` (189 lines). Imports at lines 1–6 use explicit BCL imports and a file-scoped `KingdomZombies.Core` namespace. Methods use named arguments; mutable authority remains private, while published state is immutable.

**Trusted identity and replay guard, lines 74–85:**

```csharp
public bool TryAccept(FoundationMovementCommand command, string trustedBinding, string trustedPlayer,
        string trustedActor)
{
    ArgumentNullException.ThrowIfNull(argument: command);
    FoundationParticipantState? participant = Find(bindingId: trustedBinding);
    if (!_active || participant is null || !participant.Ready || command.Direction is < -1 or > 1
            || command.Sequence <= 0 || !Matches(command: command, participant: participant,
                    trustedPlayer: trustedPlayer, trustedActor: trustedActor)
            || _intents.TryGetValue(key: trustedBinding, value: out FoundationHeldIntent? previous)
                    && command.Sequence <= previous.Sequence)
    {
        return false;
    }
```

Copy the trust boundary, ordinal comparisons (lines 137–145) and validate-before-mutation ordering. Do not copy movement sequence retention as a complete purchase receipt design. Camp requests must resolve the current living controlled survivor, spectator/access/range rules, displayed quote, actual carried funds, item capacity/ownership and current target together through Phase 6 authority. Duplicate purchases and retries require retained consequence identity; rejected requests preserve the entire prior root and return current values plus a reason.

**Explicit elapsed input and whole-publication replacement, lines 93–100:**

```csharp
public void Step(long elapsedMilliseconds)
{
    RequireActive();
    ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
    FoundationParticipantState[] candidate = [.._snapshot.Participants.Select(selector: participant =>
            AdvanceParticipant(participant: participant, elapsedMilliseconds: elapsedMilliseconds))];
    _snapshot = new FoundationSnapshot(sessionId: Actor.SessionId, revision: checked(_snapshot.Revision + 1),
            participants: candidate);
```

Use explicit integer elapsed time and checked arithmetic; do not copy the fixture movement clamp as a camp-time policy. CampTime must process crossed boundaries in order and retain per-event daily identity with each consequence. Recompute borders/refuge and safety before dependent work. CampOrders must distinguish paid work, old completed tier, damageable frame, retained progress, repair health and destruction; there is no existing paid-order state machine to copy.

**Atomic detached candidate analog:** `src/core/ExampleStateStore.cs`, lines 22–43:

```csharp
try
{
    cancellationToken.ThrowIfCancellationRequested();
    ExampleState candidate = SnapshotCodec.Decode(document: document);
    beforeCommit?.Invoke(obj: candidate);
    cancellationToken.ThrowIfCancellationRequested();
    Interlocked.Exchange(location1: ref _root, value: candidate);
    return candidate;
}
finally
{
    Volatile.Write(location: ref _writer, value: 0);
}
```

Lines 26–29 reject concurrent preparation; lines 16–19 acquire one root with Volatile.Read. This is a proof-store mechanism, not a new production store mandate. Preserve the delivered single-writer transaction boundary and prepare physical item effects plus camp consequence before one commit. Never debit money and subsequently discover invalid worker/site/capacity state.

**Detached model analog:** `src/core/FoundationSnapshot.cs`, lines 13–19:

```csharp
public FoundationSnapshot(string sessionId, long revision, IEnumerable<FoundationParticipantState> participants)
{
    ArgumentNullException.ThrowIfNull(argument: participants);
    Participants = [.. participants];
    SessionId = sessionId;
    Revision = revision;
    Validate();
}
```

Lines 46–56 enforce distinct binding/player/actor/slot identities with ordinal comparers and throw InvalidDataException on invalid combinations. Extend the delivered Phase 6 detached root/codec, not the four-participant movement schema. Include camp time/events, resources, NPC jobs/cargo/reservations, structures/orders and required random state; validate cross-references and impossible combinations before installation. Current feature round trips are Phase 7; save UX, reconnect and late join remain Phase 9.

### Godot command transport and lifetime

**Analog:** `src/godot-game/foundation/FoundationCoordinator.cs`, lines 1–15: explicit `System.*`, `KingdomZombies.Core` and `KingdomZombies.Multiplayer.Abstractions` imports; an internal partial coordinator accepts `IConnectionService?` and an injected `Func<long>?` clock. Keep provider dependencies in Godot and independent service contracts in multiplayer abstractions.

**Receive guard, lines 163–171:**

```csharp
long lifetime = _lifetime;
foreach (ConnectionPacket packet in _connection?.Poll(maximumPackets: 64) ?? [])
{
    if (!_active || lifetime != _lifetime
            || !StringComparer.Ordinal.Equals(x: packet.Generation, y: _generation)
            || packet.PeerGeneration.Length == 0 || !StringComparer.Ordinal.Equals(x: packet.PeerGeneration,
                    y: _connection?.PeerGeneration(peer: packet.Sender)))
    {
        continue;
    }
```

Lines 176–183 decode a detached envelope and dispatch using the actual sender; guest publications are accepted only from the host sender. Lines 186–199 catch known malformed-input/domain/overflow failures, increment rejection observations and distinguish incompatible protocol/content. Use the delivered command envelope/binding and bounded transport; do not trust a client owner, price or completion assertion. Measure realistic camp snapshots against actual bounded serialization/transfer and prove malformed, truncated, oversized, stale and duplicate-identity rejection before expanding payload bounds.

### Production camp UI and feedback

**Visual analog:** `src/godot-game/inspection/CampLayouts.cs`. This is explicitly inspection-only (line 15). Copy layout/resource/lifetime techniques into the delivered production view path; never turn sample JSON into gameplay authority.

**Thread and generation guard, lines 124–138:**

```csharp
internal void Project(byte[] snapshot, long generation)
{
    _gateway.RequireThread();
    JsonElement state = Read(bytes: snapshot);
```

After complete validation, line 136 rejects `!_accepting || generation <= _generation || !_root.IsInsideTree()`. Close at lines 160–165 revokes acceptance and advances generation before clearing owned controls. Use these fences for tab changes, pending callbacks and teardown; tie one-shot sounds/interruption effects to committed event identity, not every publication.

**Composition excerpt, lines 843–852:**

```csharp
var panel = new PanelContainer { Name = "Camp", Position = new Vector2(x: 24, y: 112),
        Size = new Vector2(x: 1232, y: 584), Theme = ResourceLoader.Load<Theme>(
                path: "res://assets/ui/InspectionTheme.tres") };
var scroll = new ScrollContainer { Name = "Scroll",
        HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
var content = new VBoxContainer { Name = "Content", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
_root.AddChild(node: panel);
_owned.Add(item: panel);
panel.AddChild(node: scroll);
scroll.AddChild(node: content);
```

Resource identity is current inspected source; re-read final approved theme/resources before production composition. Dimensions are examples, not locked production sizes. D-01–D-05 require one almost-full-screen translucent four-tab window opening on Stash, Phase 6 grids only on Stash, continuous simulation/warnings, panel-first Escape, bag closure with remembered positions on leaving, nonlethal damage retention and death/access-loss closure.

**People assignment:** lines 869–881 create HBoxContainer `PeopleColumns`, side-by-side VBoxContainer `Roster` and `Jobs`, and label each survivor with job/activity. Lines 884–902 show price/equipment and disabled assignment reasons. Adapt to D-06–D-09: roster grouped by role with headings/counts, a fixed recruit/price/fireside/total header, persistent selected details and live away/controlled explanations. The sample's flat roster and footer recruitment do not satisfy those decisions.

**Sale assignment:** lines 908–917 compose food icons/stock ledger; lines 935–943 show quantity, unit, total, resulting stash money and physical proceeds. Reuse display structure only. Lines 932–933 set `Editable = false`; lines 946–948 invoke `Modal(amount: amount, result: result)`; lines 1112–1128 create `SaleConfirmation`. These conflict with D-10–D-14 and must not be copied into production. Implement editable whole numbers, +/−, Max, pre-click stock clamp and one explicit Sell submitting the clicked quote. A rejected stale quote refreshes values/reason, preserves possessions and requires a new click. Scope quote identity to relevant sale stock/prices; unrelated movement publication is not sufficient invalidation.

**Site assignment:** lines 969–982 display tier, health, prerequisite, carried money, price and paid order. D-12 submits the priced action directly. D-15–D-18 need actual nearby paid work/wait markers, detailed reasons in the site panel, current/full health for paid repair (including downward movement after damage), and brief danger/night interruption cues with persistent People activity reasons. Sample progress must not override these semantics.

### Input context

**Analog:** `src/godot-game/foundation/FoundationInput.cs`, lines 15–23:

```csharp
internal void SetContext(bool gameplay)
{
    if (_gameplay != gameplay)
    {
        Clear();
        _gameplay = gameplay;
    }
}
```

Lines 41–45 block currently held keys, clear holds and reset FreshPress. Lines 31–36 require release before a blocked key becomes fresh again. This movement-only foundation is an analog; use delivered Phase 6 capture across movement, attack, quick food and world clicks for every camp tab. GUI event consumption alone does not clear global held polling. Closing a panel must not reactivate a still-held action or pause host simulation.

### Configuration and content ownership

**Loader analog:** `src/core/ContentPack.cs`, lines 34–52 validates exact portable `src/godot-game/` resource identities with StringComparison.Ordinal, rejects invalid segments and returns the canonical relative key. Lines 86–105 separate byte reads from validation and report known format/JSON/Unicode diagnostics. It remains an asset census/inspection loader; implement typed gameplay domain validation through the delivered configuration path rather than treating successful asset inspection as production acceptance.

Preserve existing domain ownership and identities:

| Tracked content under `src/godot-game/` | Production consumer and validation |
| --- | --- |
| `content/config/time.json` (DT-02) | CampTime; positive finite durations, ordered dawn IDs and boundary behavior. Lines 16–17 declare once-per-dawn policy and ordered `advance-day`, `update-stage`, `refill-plants`, `introduce-wildlife`, `retreat-eligible-zombies`, `autosave`; integrate later-owned consequences without claiming save delivery. |
| `content/config/jobs.json` (DT-07) | Recruitment/role prices and limits, equipment alternatives/reserves, job priority, safety, hauling/work rules. Reconcile production role IDs explicitly. |
| `content/config/food-wildlife.json` (DT-08) | Real item food/sales, finite plant stock, prey admissions and meat creation. Interpret food menu as the selected Food sales tab. |
| `content/config/map-sites.json` (DT-09) | Both-sided authored camp, stable site/plant/zone identities, representative active/destroyed portal barriers; no full Phase 8 map claim. |
| `content/config/structures.json` (DT-10) | Completed/pending tiers, prerequisites, frames, geometry, incremental costs, cumulative valuation/rebuild and paid repair targets. |
| `content/config/towers.json` (DT-11) | Structural guard count/profile, independent sockets/origins and shared-main health; no recruited inventory/population/corpse artifacts. |
| `content/ui/camp-panels.json` | Style/sample reference only; sampleDataOnly/gameplayTransactions flags exclude authority. |
| `content/text/en.json`, `content/text/expanded.json` | Production labels/reasons/field contracts and overflow coverage; keep translations and consumers consistent. |
| `content/pack-index.json` | Packaged identity/reference integration; no runtime donor paths. |

Bind final produced tier/damage/frame/ruin art, worker/gather/hunt/carry/retreat animations, warnings and audio through approved packaged resources. Gameplay collision/navigation/interaction/attack geometry remains independent of silhouettes. Update changed field documentation and consumers together. Do not copy inspection hardcoded prices or capacities as configuration authority.

### Tests, scenarios and catalog integration

**Pure proof analog:** `tests/phase1/FoundationMovementTests.cs`, lines 1–5 imports explicit Core/Foundation, Shouldly and Xunit; lines 13–14 and 35–36 use `[Fact]` methods. Lines 70–81 assert both expected exception and identical prior actor on invalid/overflow input:

```csharp
Should.Throw<ArgumentException>(actual: () => session.Advance(command: invalid, elapsedMilliseconds: 1));
session.Actor.ShouldBeSameAs(expected: before);
```

Adapt this invariant to the complete production root/possessions/orders. Proposed test basenames are CampTransactionTests, CampTimeTests, CampNpcTests, CampStructureTests and CampSnapshotTests. Cover races in both orders, retry/replay, relevant stale quote versus unrelated publication, checked overflow, interrupted reservations, actual physical visits/transfers, daily boundary replay, repair damage/destruction and detached feature round trips. Pair pure proofs with actual headless production driver/network observations; UI samples and test-only mutation cannot certify gameplay.

**Project ownership:** `tests/phase1/KingdomZombies.Phase1.Tests.csproj`, lines 1–7 is the existing executable test project. Lines 9–46 explicitly link selected engine-free adapter source and reference Core; do not add a Godot project reference or a sixth project. `tools/phase1/projects.json` is the exact five-project graph: Core and multiplayer abstractions have no project dependencies; Godot references both; tests reference Core. Keep native construction in Godot and all feature rules engine-free. Existing graph/analyzer/public-surface rules continue to apply; no broad exemption is implied.

**Catalog analog:** `tools/phase3/catalog.cjs`, lines 10–15 validates title/importance/rationale/setup/reset/runGroup, actions/expected/evidence/requirements/acceptance/tags, order and rendered roles/images. Lines 17–24 require exact coverage and selected/omitted accounting. Lines 29–44 bind prelaunch to reviewed source and immutable review artifacts. Copy the requirements of independent discovery, explicit selection and evidence identity into delivered Phase 5 registration, not Phase 3's fixed phase counts or selectors.

Save Phase 7 scenario/crosswalk and ordered groups with deterministic seeds, explicit time, ordinary production resets, process roles, bounded waits and owned cleanup. Account for all 22 global/25 local requirements, 48 AC, 168 edge resolutions and D-01–D-18 without renumbering upstream scenarios. New/changed and affected cases plus all active Critical cases are selected; omitted/planned/blocked cases receive no pass credit.

## Shared Patterns

- **Authority:** trusted connection/session/control binding plus current-state eligibility at host commit; disabled controls are feedback only.
- **Atomicity:** prepare complete physical and gameplay consequences, validate, then replace one immutable root. Preserve prior state on rejection/overflow/cancellation.
- **Determinism:** explicit integer elapsed time, checked quantities, ordinal identities and injected replayable randomness; processed event IDs travel with consequences.
- **Lifetime:** captured native-thread checks, acceptance closure before cleanup, generation fencing and one-shot event identities.
- **Validation:** bounded strict detached parse plus cross-reference/invariant validation before install; measure valid payload size before changing transport budgets.
- **Verification:** only no-window/headless work until the entire implementation and independent code review/fixes are complete. Later local rendered work uses Windows 1280×720, no VM, grouped selected scenarios. Preserve original failures, batch fixes, run focused repairs, then the full selected set on the repaired build. Hosted/Linux/mixed-OS/hardware/release proofs remain separate.

## No Analog Found

| Candidate | Missing production pattern | Planning source |
| --- | --- | --- |
| CampTerritory | Live borders/refuge fallback and portal-restricted reachability | Locked spec plus delivered Phase 6 access/safety |
| CampResources | Daily finite stock and wildlife admissions | Time/data domains and locked phase rules |
| NpcJobs | Priority/interruption/autonomous task state machine | Locked spec plus delivered common movement/combat |
| NpcEquipment | Actual NPC inventory visits and gear changes | Delivered Phase 6 ownership/transfer operations |
| HaulingReservations | Reservation ownership versus physical transfer | Locked spec plus delivered inventory API |
| CampWorldPresenter | Production paid work/repair and retreat event cues | D-15–D-18 plus delivered world presentation |

There is also no equivalent production paid-order or economic transaction implementation: the partial matches above establish mechanics only. Do not invent predecessor symbols to fill these gaps.

## Metadata

**Analog search scope:** tracked Core, Godot foundation/inspection/content, existing proof project, Phase 3 catalog and phase research/context.
**Primary pattern families:** five: authority/state, native coordinator/input, inspection composition, packaged content, proof/catalog infrastructure.
**Tracked-source gate:** named existing source/config/project analogs checked with `git ls-files -- <path>`; no installed capability mirrors are analogs.
**Coverage:** 23 classified candidate rows; 17 have exact/role/partial mechanics or domain matches; 6 explicitly have no production analog. These are planning coverage counts, not implemented files or passing tests.
**Recheck:** at Phase 5/6 delivery and immediately before implementation; final Phase 4 source is actively changing.
