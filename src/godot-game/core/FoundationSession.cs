using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using static KingdomZombies.Core.RunSnapshotDocument;

namespace KingdomZombies.Core;

/// <summary>Owns identified participants and the sole intent and elapsed movement authority.</summary>
public sealed class FoundationSession
{
    /// <summary>Creates a fresh configured run with one ready local participant and a legacy actor token.</summary>
    public FoundationSession(string sessionId, string playerId, string actorId)
            : this(sessionId: sessionId, playerId: playerId, actorId: actorId, world: RunWorld.Foundation)
    {
    }

    /// <summary>Gets the inherited view of the host actor from the common publication.</summary>
    public FoundationActorState Actor => Snapshot.Participants[0].Actor;
    /// <summary>Gets one complete immutable publication.</summary>
    public FoundationSnapshot Snapshot => _publication.Snapshot;
    /// <summary>Gets the exact resource paths and per-file size limit used by the guarded native reader.</summary>
    public static (ImmutableArray<string> paths, int maximumBytes) RequiredRunResources =>
            (RunDefinitions.DocumentPaths, RunDefinitions.MaximumDocumentBytes);

    /// <summary>Gets the local host actor's current safety and shared stash access at the committed run time.</summary>
    public (long elapsedMilliseconds, bool insideCamp, bool safeDistance, bool stashReachable) LocalWorld
    {
        get
        {
            RunPublication current = _publication;
            RunWorld.Point contact = ActorContact(current: current, participant: current.Snapshot.Participants[0]);
            (bool insideCamp, bool safeDistance) = current.World.Safety(actor: contact);
            bool stashReachable = current.World.Reachable(actor: contact, kind: RunWorld.Access.Stash,
                    id: RunWorld.StashId);
            return (current.ElapsedMilliseconds, insideCamp, safeDistance, stashReachable);
        }
    }

    /// <summary>Reads a ready binding's current survivor or most recent death epoch.</summary>
    public (string survivorId, long generation, long deathGeneration)? ReadRunControl(
            (string session, string binding, string player) identity)
    {
        return UnderInventoryAuthority(action: current => RunProjections.Control(root: current, identity: identity));
    }

    /// <summary>Captures the public gameplay data and matching roster from one authority revision.</summary>
    public (FoundationSnapshot roster, JsonElement world)? ReadRunPublic()
    {
        return UnderInventoryAuthority(action: RunProjections.Public);
    }

    /// <summary>Captures the bound recipient status and requested permitted inventory windows.</summary>
    public (long revision, ImmutableArray<(string scope, long generation, long token, JsonElement header,
            ImmutableArray<JsonElement> rows)> views)? ReadRunViews(
                    (string session, string binding, string player) identity, long? ownGrid = null,
                    long? externalGrid = null, bool includeItems = true, ImmutableArray<long> ownWindows = default,
                    ImmutableArray<long> externalWindows = default)
    {
        return UnderInventoryAuthority(action: current => RunProjections.Private(session: this, root: current,
                identity: identity, ownGrid: ownGrid, externalGrid: externalGrid, includeItems: includeItems,
                ownWindows: ownWindows, externalWindows: externalWindows));
    }

    /// <summary>Reads the access fence and identifies immutable inventory changes for publication.</summary>
    public (long revision, long epoch, long token, long inventoryVersion)? ReadRunViewFence(
            (string session, string binding, string player) identity)
    {
        lock (_gate)
        {
            RunPublication root = _publication;
            if (RunProjections.Control(root: _active ? root : null, identity: identity) is not { } control)
            {
                return null;
            }

            if (!ReferenceEquals(objA: _announcedInventory, objB: root.Inventory))
            {
                _announcedInventoryVersion = checked(_announcedInventoryVersion + 1);
                _announcedInventory = root.Inventory;
            }

            long token = control.generation == 0 ? 0 : ItemActions.SelectionFor(controller:
                    new RunItemActions.Controller(sessionId: identity.session, bindingId: identity.binding,
                            generation: control.generation))?.Token ?? 0;
            return (root.Snapshot.Revision, control.generation, token, _announcedInventoryVersion);
        }
    }

    /// <summary>Executes an authenticated living-control action at the current authority revision.</summary>
    public (string status, bool replayed, long revision) DispatchRunAction(
            (string session, string binding, string player) identity, (string survivor, long epoch, long sequence) key,
            string kind, JsonElement request)
    {
        lock (_gate)
        {
            RunPublication root = _publication;
            (string survivorId, long generation, long deathGeneration)? control = RunProjections.Control(
                    root: _active ? root : null, identity: identity);
            if (control is not { generation: > 0 } lease || lease.generation != key.epoch
                    || !StringComparer.Ordinal.Equals(x: lease.survivorId, y: key.survivor))
            {
                return ("AccessLost", false, root.Snapshot.Revision);
            }

            var controller = new RunItemActions.Controller(sessionId: identity.session, bindingId: identity.binding,
                    generation: lease.generation);
            (string status, bool replayed) = RunProjections.Dispatch(session: this, root: root, controller: controller,
                    sequence: key.sequence, kind: kind, request: request);
            return (status, replayed, _publication.Snapshot.Revision);
        }
    }

    /// <summary>Submits ready bindings' Respawn requests together at the current host timestamp.</summary>
    public ImmutableArray<(string status, bool replayed)> DispatchRunRespawns(string session,
            ImmutableArray<(string binding, string player, long epoch, long sequence)> requests)
    {
        lock (_gate)
        {
            RunPublication? root = _active ? _publication : null;
            ImmutableArray<RunSuccession.Request> batch = [.. requests.Select(selector: row =>
            {
                (string, string, string) identity = (session, row.binding, row.player);
                bool ready = RunProjections.Control(root: root, identity: identity).HasValue;
                return new RunSuccession.Request(sessionId: session, bindingId: ready ? row.binding : "",
                        deathGeneration: row.epoch, sequence: row.sequence);
            })];
            return [.. RequestRespawns(requests: batch).Select(selector: outcome =>
                    (outcome.Status.ToString(), outcome.Replayed))];
        }
    }

    /// <summary>Reserves a distinct participant without making its actor controllable.</summary>
    public bool AddParticipant(string bindingId, string playerId, string actorId, string label)
    {
        lock (_gate)
        {
            RunPublication current = _publication;
            if (!_active || current.Snapshot.Participants.Length >= current.Definitions.PlayerLimit)
            {
                return false;
            }

            int slot = Enumerable.Range(start: 1, count: current.Definitions.PlayerLimit)
                    .First(predicate: candidate => !current.Snapshot.Participants.Any(
                            predicate: participant => participant.Slot == candidate));
            string sessionId = current.Snapshot.SessionId;
            RunPopulation? population = current.Population.Reserve(sessionId: sessionId, playerId: playerId,
                    bindingId: bindingId, slot: slot);
            if (population is null)
            {
                return false;
            }

            RunPopulation.Survivor survivor = population.ForLease(lease: population.Leases[^1]);
            var actor = new FoundationActorState(sessionId: sessionId, playerId: playerId, actorId: actorId,
                    positionSubunits: survivor.PositionSubunits, facing: survivor.Facing, elapsedMilliseconds: 0);
            var participant = new FoundationParticipantState(bindingId: bindingId, label: label, slot: slot,
                    ready: false, actor: actor);
            return TryPublish(participants: current.Snapshot.Participants.Add(item: participant),
                    population: population);
        }
    }

    /// <summary>Enables a reserved actor after its current binding installed a baseline.</summary>
    public bool MarkReady(string bindingId)
    {
        lock (_gate)
        {
            FoundationParticipantState? participant = Find(snapshot: Snapshot, bindingId: bindingId);
            if (!_active || participant is null || participant.Ready)
            {
                return false;
            }

            return TryPublish(participants: Snapshot.Participants.Select(selector: current => current == participant
                    ? new FoundationParticipantState(bindingId: current.BindingId, label: current.Label,
                            slot: current.Slot, ready: true, actor: current.Actor) : current));
        }
    }

    /// <summary>Removes exactly one remote binding and its held intent, preserving remaining actors.</summary>
    public bool RemoveParticipant(string bindingId)
    {
        lock (_gate)
        {
            FoundationParticipantState? participant = Find(snapshot: Snapshot, bindingId: bindingId);
            if (!_active || participant is null || bindingId is "local")
            {
                return false;
            }

            if (!TryPublish(participants: Snapshot.Participants.Where(predicate: p => p != participant),
                    population: Population.Release(bindingId: bindingId)))
            {
                return false;
            }

            _intents.Remove(key: bindingId);
            _foodRequests.Remove(key: bindingId);
            _respawnReceipts = _respawnReceipts.RemoveRange(keys: _respawnReceipts.Keys.Where(
                    predicate: key => StringComparer.Ordinal.Equals(x: key.binding, y: bindingId)));
            return true;
        }
    }

    /// <summary>Stops a disconnected binding's held movement while retaining its last accepted sequence.</summary>
    public void StopHeldMovement(string bindingId)
    {
        lock (_gate)
        {
            if (_intents.TryGetValue(key: bindingId, value: out (long generation, FoundationHeldIntent intent) held)
                    && (held.intent.Direction != 0 || held.intent.Sprint))
            {
                _intents[bindingId] = (held.generation, new FoundationHeldIntent(sequence: held.intent.Sequence,
                        direction: 0));
            }
        }
    }

    /// <summary>Accepts a unique owned intent without advancing time or position.</summary>
    public bool TryAccept(FoundationMovementCommand command, string trustedBinding, string trustedPlayer,
            string trustedActor)
    {
        ArgumentNullException.ThrowIfNull(argument: command);
        lock (_gate)
        {
            FoundationParticipantState? participant = Find(snapshot: Snapshot, bindingId: trustedBinding);
            RunPopulation.Lease? lease = Population.FindLivingControl(bindingId: trustedBinding);
            if (!_active || participant is null || !participant.Ready || lease is null || lease.Reassigned
                    || command.Direction is < -1 or > 1
                    || !Matches(command: command, participant: participant, trustedPlayer: trustedPlayer,
                            trustedActor: trustedActor)
                    || !FreshIntent(lease: lease, sequence: command.Sequence))
            {
                return false;
            }

            _intents[trustedBinding] = (lease.Generation, new FoundationHeldIntent(sequence: command.Sequence,
                    direction: command.Direction));
            return true;
        }
    }

    /// <summary>Moves all ready actors once over the host's interval (previous time, current time].</summary>
    /// <remarks>Eligibility can begin at its deadline. Recovery accrues only over elapsed time after it.</remarks>
    public void Step(long elapsedMilliseconds)
    {
        lock (_gate)
        {
            RequireActive();
            ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
            RunPublication current = _publication;
            long elapsed = checked(current.ElapsedMilliseconds + elapsedMilliseconds);
            RunPopulation population = AdvancePopulation(current: current, elapsed: elapsed);
            RunProjectiles.Frame flight = RunProjectiles.Advance(current: current, population: population,
                    now: elapsed);
            FoundationParticipantState[] candidate = [.. current.Snapshot.Participants.Select(selector: participant =>
                    AdvanceParticipant(participant: participant, population: population,
                            elapsedMilliseconds: elapsedMilliseconds))];
            var snapshot = new FoundationSnapshot(sessionId: current.Snapshot.SessionId,
                    revision: checked(current.Snapshot.Revision + 1), participants: candidate);
            _publication = FinishStep(current: current, snapshot: snapshot, flight: flight, elapsed: elapsed);
        }
    }

    /// <summary>Validates the complete legacy command and tick before replacing the held intent.</summary>
    public void Advance(FoundationMovementCommand command, long elapsedMilliseconds)
    {
        lock (_gate)
        {
            RequireActive();
            ArgumentNullException.ThrowIfNull(argument: command);
            ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
            _ = checked(_publication.ElapsedMilliseconds + elapsedMilliseconds);
            _ = checked(Snapshot.Revision + 1);
            long sequence = _intents.TryGetValue(key: "local", value: out (long generation,
                    FoundationHeldIntent intent) held)
                    ? checked(held.intent.Sequence + 1) : 1;
            if (!TryAccept(command: command with { Sequence = sequence }, trustedBinding: "local",
                    trustedPlayer: Actor.PlayerId, trustedActor: Actor.ActorId))
            {
                throw new ArgumentException(message: "FOUNDATION_OWNER_OR_DIRECTION", paramName: nameof(command));
            }

            Step(elapsedMilliseconds: elapsedMilliseconds);
        }
    }

    /// <summary>Closes acceptance before transport and native scene teardown.</summary>
    public void Close()
    {
        lock (_gate)
        {
            _active = false;
            _intents.Clear();
            _foodRequests.Clear();
            _attackRequests.Clear();
            _respawnReceipts = _respawnReceipts.Clear();
        }
    }

    /// <summary>Starts the ordinary run from bounded authored bytes supplied by the native composition.</summary>
    public static FoundationSession CreateFromResources(string sessionId, string playerId, string actorId,
            Func<string, int, byte[]?> read, bool observeWorld = false)
    {
        var definitions = RunDefinitions.Load(read: read);
        RunWorld world = RunWorld.Foundation;
#if DEBUG
        if (observeWorld)
        {
            world = RunWorld.Observed;
        }
#endif
        var configuredWorld = new RunWorld(origin: world.Origin, groundContactSubunits: world.GroundContactSubunits,
                mainBuilding: world.MainBuilding, ranges: definitions.Ranges, defenses: world.Defenses,
                blockers: world.Blockers, hostiles: world.Hostiles, anchors: world.Anchors);
        return new FoundationSession(sessionId: sessionId, playerId: playerId, actorId: actorId,
                world: configuredWorld, definitions: definitions);
    }

#if DEBUG
    /// <summary>Creates the fixed world used by the owned native observation run.</summary>
    public static FoundationSession CreateObserved(string sessionId, string playerId, string actorId)
    {
        return new FoundationSession(sessionId: sessionId, playerId: playerId, actorId: actorId,
                world: RunWorld.Observed);
    }
#endif

    internal FoundationSession(string sessionId, string playerId, string actorId, RunWorld world,
            RunDefinitions? definitions = null)
    {
        ArgumentNullException.ThrowIfNull(argument: world);
        world.Validate();
        definitions ??= RunDefinitions.Default;
        var population = RunPopulation.Create(definitions: definitions, sessionId: sessionId, playerId: playerId);
        var inventory = RunInventory.Create(definitions: definitions, population: population);
        if (!RunInventoryRules.IsValid(inventory: inventory, definitions: definitions, population: population))
        {
            throw new InvalidDataException(message: "RUN_INVENTORY_INVALID");
        }

        var snapshot = new FoundationSnapshot(sessionId: sessionId, revision: 0, participants:
                [new FoundationParticipantState(bindingId: "local", label: "Player 1", slot: 1, ready: true,
                        actor: new FoundationActorState(sessionId: sessionId, playerId: playerId, actorId: actorId,
                                positionSubunits: RunPopulation.StartingPosition, facing: 1,
                                elapsedMilliseconds: 0))]);
        (world, RunDeathAndLoot.State loot) = RunDeathAndLoot.Reconcile(world: world, inventory: inventory,
                loot: RunDeathAndLoot.State.Empty, now: 0, delay: definitions.LootEmptyDelay);
        _publication = new RunPublication(snapshot: snapshot, world: world, elapsedMilliseconds: 0,
                definitions: definitions, population: population, inventory: inventory, combat: RunCombat.State.Empty,
                loot: loot, succession: RunSuccession.Start(sessionId: sessionId));
        ItemActions = new RunItemActions(session: this);
    }

    internal RunPopulation Population => _publication.Population;
    internal RunInventory Inventory => _publication.Inventory;
    internal RunDefinitions Definitions => _publication.Definitions;
    internal RunItemActions ItemActions { get; }

    internal byte[] CaptureRunSnapshot()
    {
        RunSnapshotDocument document;
        lock (_gate)
        {
            RequireActive();
            document = CaptureRunDocument();
        }

        return RunSnapshotCodec.Encode(document: document);
    }

    internal bool TryRestoreRunSnapshot(ReadOnlySpan<byte> data, out string? error)
    {
        lock (_gate)
        {
            if (!_active)
            {
                error = "FOUNDATION_CLOSED";
                return false;
            }

            try
            {
                (RunSnapshotDocument document, RunPublication candidate) =
                        RunSnapshotCodec.Decode(data: data, definitions: _publication.Definitions);
                InstallRunDocument(document: document, candidate: candidate);
                error = null;
                return true;
            }
            catch (Exception failure) when (failure is InvalidDataException or JsonException or ArgumentException
                    or OverflowException)
            {
                error = failure.Message;
                return false;
            }
        }
    }

    internal RunSuccession.View? ReadSuccession(string bindingId)
    {
        return UnderInventoryAuthority(action: current => current is null ? null : RunSuccession.Read(
                current: current, binding: bindingId));
    }

    internal ImmutableArray<RunSuccession.Outcome> RequestRespawns(ImmutableArray<RunSuccession.Request> requests)
    {
        lock (_gate)
        {
            if (requests.IsDefaultOrEmpty)
            {
                return [];
            }

            if (!_active)
            {
                return [.. requests.Select(selector: _ => new RunSuccession.Outcome(
                        status: RunSuccession.Status.AccessLost, view: null))];
            }

            RunPublication current = _publication;
            RunSuccession.Batch batch = RunSuccession.Prepare(current: current, requests: requests,
                    acknowledgments: _respawnReceipts);
            bool published = ReferenceEquals(objA: current.Succession, objB: batch.State)
                    || TryPublish(participants: current.Snapshot.Participants, succession: batch.State);
            return RunSuccession.Complete(current: _publication, requests: requests, batch: batch,
                    published: published, receipts: ref _respawnReceipts);
        }
    }

    internal RunCombat.Outcome Attack(RunItemActions.Controller controller, long sequence, RunCombat.Aim? aim = null)
    {
        lock (_gate)
        {
            RunPublication current = _publication;
            RunPopulation.Lease? lease = CurrentControl(current: current, controller: controller);
            return lease is null ? new RunCombat.Outcome(status: RunCombat.Status.AccessLost)
                    : AcceptAttack(current: current, attacker: current.Population.ForLease(lease: lease),
                            generation: lease.Generation, sequence: sequence, aim: aim);
        }
    }

    internal RunCombat.Outcome AttackUncontrolled(string survivorId, long sequence, RunCombat.Aim? aim = null)
    {
        lock (_gate)
        {
            RunPublication current = _publication;
            RunPopulation.Survivor? attacker = current.Population.Survivors.FirstOrDefault(predicate: survivor =>
                    StringComparer.Ordinal.Equals(x: survivor.Id, y: survivorId));
            if (!_active || attacker is not { Health: > 0 } || current.Population.Leases.Any(predicate: lease =>
                    StringComparer.Ordinal.Equals(x: lease.SurvivorId, y: survivorId)))
            {
                return new RunCombat.Outcome(status: RunCombat.Status.AccessLost);
            }

            return AcceptAttack(current: current, attacker: attacker, generation: 0, sequence: sequence, aim: aim);
        }
    }

    internal RunCombat.BowView? ReadBow(RunItemActions.Controller controller, RunCombat.Aim aim)
    {
        lock (_gate)
        {
            RunPublication current = _publication;
            RunPopulation.Lease? lease = CurrentControl(current: current, controller: controller);
            return lease is null ? null : RunCombat.ViewBow(inventory: current.Inventory,
                    survivor: current.Population.ForLease(lease: lease), aim: aim);
        }
    }

    internal bool TryAddHostile(string id, decimal position, int facing, decimal health)
    {
        lock (_gate)
        {
            RunPublication current = _publication;
            if (!_active || position is < -59000 or > 59000 || facing is not (-1 or 1)
                    || health <= 0 || health > current.Definitions.MaximumHealth
                    || current.Population.Survivors.Any(predicate: item => StringComparer.Ordinal.Equals(
                            x: item.Id, y: id)))
            {
                return false;
            }

            var hostile = new RunPopulation.Survivor(id: id, positionSubunits: 0, facing: facing, health: health,
                    stamina: current.Definitions.MaximumStamina, role: "hostile")
            {
                Hostile = true,
                PositionSubunitsExact = position,
            };
            RunWorld world = current.World.WithHostiles(hostiles: current.World.Hostiles.Add(item:
                    new RunWorld.Hostile(id: id, contact: current.World.ActorContact(survivor: hostile))));
            return TryPublish(participants: current.Snapshot.Participants, world: world,
                    population: current.Population.Add(survivor: hostile));
        }
    }

    internal (bool accepted, RunLocomotion.Block reason) TryAcceptMotion(RunItemActions.Controller controller,
            long sequence, RunLocomotion.Input input)
    {
        lock (_gate)
        {
            RunPublication current = _publication;
            RunPopulation.Lease? lease = CurrentControl(current: current, controller: controller);
            if (lease is null || !FreshIntent(lease: lease, sequence: sequence))
            {
                return (false, RunLocomotion.Block.None);
            }

            RunLocomotion.Block reason = RunLocomotion.Block.None;
            if (input.Jump)
            {
                RunPopulation.Survivor survivor = current.Population.ForLease(lease: lease);
                RunLocomotion.Burden load = RunLocomotion.Load(inventory: current.Inventory,
                        definitions: current.Definitions, survivorId: survivor.Id);
                (RunPopulation.Survivor? jumped, reason) = RunLocomotion.Jump(survivor: survivor,
                        definitions: current.Definitions, load: load, now: current.ElapsedMilliseconds);
                if (jumped is not null && !TryPublish(participants: current.Snapshot.Participants,
                        population: current.Population.Replace(previous: survivor, current: jumped)))
                {
                    return (false, RunLocomotion.Block.None);
                }
            }

            _intents[lease.BindingId] = (lease.Generation, new FoundationHeldIntent(sequence: sequence,
                    direction: input.Direction, sprint: input.Sprint));
            return (true, reason);
        }
    }

    internal T UnderInventoryAuthority<T>(Func<RunPublication?, T> action)
    {
        lock (_gate)
        {
            return action(arg: _active ? _publication : null);
        }
    }

    internal RunFood.Result ConsumeFood(RunItemActions.Controller controller, long sequence, RunFood.Request request)
    {
        lock (_gate)
        {
            RunPublication current = _publication;
            RunPopulation.Lease? lease = CurrentControl(current: current, controller: controller);
            if (lease is null)
            {
                return new RunFood.Result(status: RunFood.Status.AccessLost, survivor: null);
            }

            RunPopulation.Survivor survivor = current.Population.ForLease(lease: lease);
            if (sequence <= 0)
            {
                return new RunFood.Result(status: RunFood.Status.StaleRequest, survivor: survivor);
            }

            if (_foodRequests.TryGetValue(key: lease.BindingId,
                    value: out (long generation, long sequence, RunFood.Status status) previous)
                    && previous.generation == lease.Generation && sequence <= previous.sequence)
            {
                bool replayed = sequence == previous.sequence;
                return new RunFood.Result(status: replayed ? previous.status : RunFood.Status.StaleRequest,
                        survivor: survivor, replayed: replayed);
            }

            RunFood.Status status = RunFood.Prepare(current: current, survivor: survivor, request: request,
                    inventory: out RunInventory? candidate, fed: out RunPopulation.Survivor? fed);
            if (candidate is not null && fed is not null)
            {
                RunPopulation population = current.Population.Replace(previous: survivor, current: fed);
                if (PublishInventory(current: current, candidate: candidate, population: population, change: out _))
                {
                    survivor = fed;
                }
                else
                {
                    status = RunFood.Status.Rejected;
                }
            }

            _foodRequests[lease.BindingId] = (lease.Generation, sequence, status);
            return new RunFood.Result(status: status, survivor: survivor);
        }
    }

    internal bool TryUpdateVitals(string survivorId, decimal health, decimal stamina)
    {
        lock (_gate)
        {
            RunPublication current = _publication;
            RunPopulation.Survivor? survivor = current.Population.Survivors.FirstOrDefault(predicate: item =>
                    StringComparer.Ordinal.Equals(x: item.Id, y: survivorId));
            if (!_active || survivor is null || survivor.Health <= 0 || health <= 0
                    || health > current.Definitions.MaximumHealth || stamina < 0
                    || stamina > current.Definitions.MaximumStamina
                    || health == survivor.Health && stamina == survivor.Stamina)
            {
                return false;
            }

            RunPopulation updated = current.Population.Replace(previous: survivor, current: survivor with
            {
                Health = health,
                Stamina = stamina,
            });
            return TryPublish(participants: current.Snapshot.Participants, population: updated);
        }
    }

    internal bool CanReach(string bindingId, RunWorld.Access kind, string targetId)
    {
        RunPublication current = _publication;
        FoundationParticipantState? participant = ReadyActor(current: current, bindingId: bindingId);
        return participant is not null && current.World.Reachable(
                actor: ActorContact(current: current, participant: participant), kind: kind, id: targetId);
    }

    internal (bool insideCamp, bool safeDistance)? SafetyFor(string bindingId)
    {
        RunPublication current = _publication;
        FoundationParticipantState? participant = ReadyActor(current: current, bindingId: bindingId);
        return participant is null ? null : current.World.Safety(actor: ActorContact(current: current,
                participant: participant));
    }

    internal bool TryReplaceRunWorld(RunWorld? candidate)
    {
        lock (_gate)
        {
            return _active && candidate is not null && TryPublish(participants: Snapshot.Participants,
                    world: candidate);
        }
    }

    internal bool TryEditInventory(Func<RunInventory, RunDefinitions, RunPopulation, RunInventory?> edit)
    {
        return TryEditInventory(edit: edit, change: out _);
    }

    internal bool TryEditInventory(Func<RunInventory, RunDefinitions, RunPopulation, RunInventory?> edit,
            out RunItemTransfer.Change? change, RunDeathAndLoot.DropSite? drop = null)
    {
        lock (_gate)
        {
            change = null;
            if (!_active)
            {
                return false;
            }

            RunPublication current = _publication;
            RunInventory? candidate = edit(arg1: current.Inventory, arg2: current.Definitions,
                    arg3: current.Population);
            return candidate is not null && PublishInventory(current: current, candidate: candidate,
                    population: current.Population, change: out change, drop: drop);
        }
    }

    private readonly Lock _gate = new();
    private Dictionary<string, (long generation, FoundationHeldIntent intent)> _intents =
            new(comparer: StringComparer.Ordinal);
    private Dictionary<string, (long generation, long sequence, RunFood.Status status)> _foodRequests =
            new(comparer: StringComparer.Ordinal);
    private Dictionary<(string id, long generation), (long sequence, RunCombat.Status status)>
            _attackRequests = [];
    private ImmutableDictionary<(string binding, long epoch), RunSuccession.Ack> _respawnReceipts = [];
    private volatile RunPublication _publication;
    private volatile bool _active = true;
    private RunInventory? _announcedInventory;
    private long _announcedInventoryVersion;

    private RunSnapshotDocument CaptureRunDocument()
    {
        IntentData[] intents = [.. _intents.OrderBy(keySelector: entry => entry.Key, comparer: StringComparer.Ordinal)
                .Select(selector: entry => new IntentData
                {
                    BindingId = entry.Key, Generation = entry.Value.generation, Sequence = entry.Value.intent.Sequence,
                    Direction = entry.Value.intent.Direction, Sprint = entry.Value.intent.Sprint,
                })];
        FoodReceiptData[] food = [.. _foodRequests.OrderBy(keySelector: entry => entry.Key,
                comparer: StringComparer.Ordinal).Select(selector: entry => new FoodReceiptData
                {
                    BindingId = entry.Key, Generation = entry.Value.generation, Sequence = entry.Value.sequence,
                    Status = entry.Value.status,
                })];
        AttackReceiptData[] attacks = [.. _attackRequests.OrderBy(keySelector: entry => entry.Key.id,
                comparer: StringComparer.Ordinal).ThenBy(keySelector: entry => entry.Key.generation)
                .Select(selector: entry => new AttackReceiptData
                {
                    Id = entry.Key.id, Generation = entry.Key.generation, Sequence = entry.Value.sequence,
                    Status = entry.Value.status,
                })];
        RespawnReceiptData[] respawns = [.. _respawnReceipts.OrderBy(keySelector: entry => entry.Key.binding,
                comparer: StringComparer.Ordinal).ThenBy(keySelector: entry => entry.Key.epoch)
                .Select(selector: entry => new RespawnReceiptData
                {
                    BindingId = entry.Key.binding, Epoch = entry.Key.epoch, Sequence = entry.Value.Sequence,
                    Status = entry.Value.Status,
                })];
        return RunSnapshotCodec.Capture(root: _publication, intents: intents, foodReceipts: food,
                attackReceipts: attacks, respawnReceipts: respawns, itemReceipts: ItemActions.CaptureReceipts());
    }

    private void InstallRunDocument(RunSnapshotDocument document, RunPublication candidate)
    {
        Dictionary<string, (long generation, FoundationHeldIntent intent)> intents = document.Intents.ToDictionary(
                keySelector: row => row.BindingId,
                elementSelector: row => (row.Generation, new FoundationHeldIntent(sequence: row.Sequence,
                        direction: row.Direction, sprint: row.Sprint)), comparer: StringComparer.Ordinal);
        Dictionary<string, (long generation, long sequence, RunFood.Status status)> food =
                document.FoodReceipts.ToDictionary(keySelector: row => row.BindingId,
                        elementSelector: row => (row.Generation, row.Sequence, row.Status),
                        comparer: StringComparer.Ordinal);
        Dictionary<(string id, long generation), (long sequence, RunCombat.Status status)> attacks =
                document.AttackReceipts.ToDictionary(keySelector: row => (row.Id, row.Generation),
                        elementSelector: row => (row.Sequence, row.Status));
        ImmutableDictionary<(string binding, long epoch), RunSuccession.Ack> respawns =
                document.RespawnReceipts.ToImmutableDictionary(keySelector: row => (row.BindingId, row.Epoch),
                        elementSelector: row => new RunSuccession.Ack(sequence: row.Sequence, status: row.Status));
        ItemActions.RestoreReceipts(receipts: document.ItemReceipts);
        _intents = intents;
        _foodRequests = food;
        _attackRequests = attacks;
        _respawnReceipts = respawns;
        _publication = candidate;
    }

    private FoundationParticipantState? ReadyActor(RunPublication current, string bindingId)
    {
        return _active && Find(snapshot: current.Snapshot, bindingId: bindingId) is { Ready: true } participant
                && current.Population.FindLivingControl(bindingId: bindingId) is not null
                ? participant : null;
    }

    private static RunWorld.Point ActorContact(RunPublication current, FoundationParticipantState participant)
    {
        return current.Population.FindLease(bindingId: participant.BindingId) is { } lease
                ? current.World.ActorContact(survivor: current.Population.ForLease(lease: lease))
                : current.World.ActorContact(actor: participant.Actor);
    }

    private RunPopulation.Lease? CurrentControl(RunPublication current, RunItemActions.Controller controller)
    {
        return _active && StringComparer.Ordinal.Equals(x: current.Snapshot.SessionId, y: controller.SessionId)
                && current.Population.FindLivingControl(bindingId: controller.BindingId) is { } lease
                && lease.Generation == controller.Generation
                && RunExternalAccess.Controller(current: current, lease: lease) is not null ? lease : null;
    }

    private bool FreshIntent(RunPopulation.Lease lease, long sequence)
    {
        return sequence > 0 && (!_intents.TryGetValue(key: lease.BindingId,
                value: out (long generation, FoundationHeldIntent intent) previous)
                || previous.generation != lease.Generation || sequence > previous.intent.Sequence);
    }

    private RunCombat.Outcome AcceptAttack(RunPublication current, RunPopulation.Survivor attacker, long generation,
            long sequence, RunCombat.Aim? aim)
    {
        if (sequence <= 0)
        {
            return new RunCombat.Outcome(status: RunCombat.Status.StaleRequest);
        }

        (string id, long generation) key = (attacker.Id, generation);
        if (_attackRequests.TryGetValue(key: key, value: out (long sequence, RunCombat.Status status) previous)
                && sequence <= previous.sequence)
        {
            bool replayed = sequence == previous.sequence;
            return new RunCombat.Outcome(status: replayed ? previous.status : RunCombat.Status.StaleRequest,
                    replayed: replayed);
        }

        (RunCombat.Status status, RunCombat.Candidate? candidate) = RunCombat.Prepare(current: current,
                attacker: attacker, aim: aim);
        if (candidate is not null && !PublishAttack(current: current, candidate: candidate))
        {
            status = RunCombat.Status.Rejected;
        }

        _attackRequests[key] = (sequence, status);
        return status == RunCombat.Status.Accepted && candidate is not null
                ? new RunCombat.Outcome(status: status, hits: candidate.Hits, arrowId: candidate.ArrowId)
                : new RunCombat.Outcome(status: status);
    }

    private bool PublishAttack(RunPublication current, RunCombat.Candidate candidate)
    {
        RunDeathAndLoot.Frame frame = RunDeathAndLoot.ApplyDeaths(world: current.World,
                population: candidate.Population, inventory: candidate.Inventory, state: current.Loot,
                hits: candidate.Hits);
        if (!ReferenceEquals(objA: current.Inventory, objB: frame.Inventory)
                && !ValidInventory(current: current, candidate: frame.Inventory, population: frame.Population))
        {
            return false;
        }

        RunInventory inventory = RunQuickReferences.Invalidate(inventory: frame.Inventory,
                definitions: current.Definitions);
        return TryPublish(participants: current.Snapshot.Participants, population: frame.Population,
                inventory: inventory, world: frame.World, combat: candidate.Combat, loot: frame.Loot);
    }

    private static RunPublication FinishStep(RunPublication current, FoundationSnapshot snapshot,
            RunProjectiles.Frame flight, long elapsed)
    {
        ImmutableArray<RunDamage.Hit> hits = elapsed == current.ElapsedMilliseconds ? []
                : [.. flight.Combat.Impacts.Select(selector: impact => impact.Hit).OfType<RunDamage.Hit>()];
        RunDeathAndLoot.Frame frame = RunDeathAndLoot.ApplyDeaths(world: flight.World,
                population: flight.Population, inventory: flight.Inventory, state: current.Loot, hits: hits);
        RunInventory inventory = frame.Inventory;
        if (!ReferenceEquals(objA: inventory, objB: current.Inventory))
        {
            if (!ValidInventory(current: current, candidate: inventory, population: frame.Population))
            {
                throw new InvalidDataException(message: "RUN_PROJECTILE_INVENTORY");
            }

            inventory = RunQuickReferences.Invalidate(inventory: inventory, definitions: current.Definitions);
        }

        (RunWorld world, RunDeathAndLoot.State loot) = RunDeathAndLoot.Reconcile(world: frame.World,
                inventory: inventory, loot: frame.Loot, now: elapsed, delay: current.Definitions.LootEmptyDelay);
        RunPopulation population = ReferenceEquals(objA: world, objB: current.World) ? frame.Population
                : RefreshRest(population: frame.Population, world: world);
        RunSuccession.Frame assigned = RunSuccession.Settle(population: population, participants: snapshot.Participants,
                loot: loot, state: current.Succession);
        if (assigned.Participants != snapshot.Participants)
        {
            snapshot = new FoundationSnapshot(sessionId: snapshot.SessionId, revision: snapshot.Revision,
                    participants: assigned.Participants);
        }

        return new RunPublication(snapshot: snapshot, world: world, elapsedMilliseconds: elapsed,
                definitions: current.Definitions, population: assigned.Population, inventory: inventory,
                combat: flight.Combat, loot: loot, succession: assigned.State);
    }

    private static RunPopulation RefreshRest(RunPopulation population, RunWorld world)
    {
        foreach (RunPopulation.Survivor survivor in population.Survivors)
        {
            population = population.Replace(previous: survivor, current:
                    RunVitals.RecheckRest(survivor: survivor, world: world));
        }

        return population;
    }

    private static FoundationParticipantState? Find(FoundationSnapshot snapshot, string bindingId)
    {
        return snapshot.Participants.FirstOrDefault(predicate: participant => StringComparer.Ordinal.Equals(
                x: participant.BindingId, y: bindingId));
    }

    private static bool Matches(FoundationMovementCommand command, FoundationParticipantState participant,
            string trustedPlayer, string trustedActor)
    {
        return StringComparer.Ordinal.Equals(x: command.SessionId, y: participant.Actor.SessionId)
                && StringComparer.Ordinal.Equals(x: command.BindingId, y: participant.BindingId)
                && StringComparer.Ordinal.Equals(x: command.PlayerId, y: trustedPlayer)
                && StringComparer.Ordinal.Equals(x: command.ActorId, y: trustedActor)
                && StringComparer.Ordinal.Equals(x: trustedPlayer, y: participant.Actor.PlayerId)
                && StringComparer.Ordinal.Equals(x: trustedActor, y: participant.Actor.ActorId);
    }

    private RunPopulation AdvancePopulation(RunPublication current, long elapsed)
    {
        RunPopulation population = current.Population;
        foreach (RunPopulation.Survivor survivor in current.Population.Survivors)
        {
            if (survivor.Health <= 0)
            {
                continue;
            }

            RunPopulation.Lease? lease = population.Leases.FirstOrDefault(predicate: live =>
                    StringComparer.Ordinal.Equals(x: live.SurvivorId, y: survivor.Id));
            FoundationHeldIntent? intent = lease is not null
                    && RunExternalAccess.Controller(current: current, lease: lease) is not null
                    && _intents.TryGetValue(key: lease.BindingId,
                            value: out (long generation, FoundationHeldIntent intent) held)
                    && held.generation == lease.Generation ? held.intent : null;
            RunPopulation.Survivor next = RunLocomotion.Advance(survivor: survivor, definitions: current.Definitions,
                    inventory: current.Inventory, world: current.World, intent: intent,
                    generation: lease?.Generation ?? 0,
                    previous: current.ElapsedMilliseconds, current: elapsed);
            population = population.Replace(previous: survivor, current: next);
        }

        return population;
    }

    private static FoundationParticipantState AdvanceParticipant(FoundationParticipantState participant,
            RunPopulation population, long elapsedMilliseconds)
    {
        if (!participant.Ready || population.FindLivingControl(bindingId: participant.BindingId) is not { } lease)
        {
            return participant;
        }

        RunPopulation.Survivor survivor = population.ForLease(lease: lease);
        FoundationActorState actor = participant.Actor;
        var moved = new FoundationActorState(sessionId: actor.SessionId, playerId: actor.PlayerId,
                actorId: actor.ActorId, positionSubunits: survivor.PositionSubunits, facing: survivor.Facing,
                elapsedMilliseconds: checked(actor.ElapsedMilliseconds + elapsedMilliseconds));
        return new FoundationParticipantState(bindingId: participant.BindingId, label: participant.Label,
                slot: participant.Slot, ready: participant.Ready, actor: moved);
    }

    private bool TryPublish(IEnumerable<FoundationParticipantState> participants, RunWorld? world = null,
            RunPopulation? population = null, RunInventory? inventory = null, RunCombat.State? combat = null,
            RunDeathAndLoot.State? loot = null, RunSuccession.State? succession = null)
    {
        try
        {
            world?.Validate();
            RunPublication current = _publication;
            RunInventory items = inventory ?? current.Inventory;
            (RunWorld settledWorld, RunDeathAndLoot.State settledLoot) = RunDeathAndLoot.Reconcile(
                    world: world ?? current.World, inventory: items, loot: loot ?? current.Loot,
                    now: current.ElapsedMilliseconds, delay: current.Definitions.LootEmptyDelay);
            if (!ReferenceEquals(objA: settledWorld, objB: world ?? current.World))
            {
                settledWorld.Validate();
            }

            RunPopulation updated = population ?? current.Population;
            updated = ReferenceEquals(objA: settledWorld, objB: current.World) ? updated
                    : RefreshRest(population: updated, world: settledWorld);
            RunSuccession.Frame assigned = RunSuccession.Settle(population: updated, participants: [.. participants],
                    loot: settledLoot, state: succession ?? current.Succession);

            var snapshot = new FoundationSnapshot(sessionId: current.Snapshot.SessionId,
                    revision: checked(current.Snapshot.Revision + 1), participants: assigned.Participants);
            _publication = new RunPublication(snapshot: snapshot, world: settledWorld,
                    elapsedMilliseconds: current.ElapsedMilliseconds, definitions: current.Definitions,
                    population: assigned.Population, inventory: items, combat: combat ?? current.Combat,
                    loot: settledLoot, succession: assigned.State);
            return true;
        }
        catch (Exception error) when (error is InvalidDataException or OverflowException)
        {
            return false;
        }
    }

    private bool PublishInventory(RunPublication current, RunInventory candidate, RunPopulation population,
            out RunItemTransfer.Change? change, RunDeathAndLoot.DropSite? drop = null)
    {
        change = null;
        if (!_active || !ReferenceEquals(objA: current, objB: _publication)
                || ReferenceEquals(objA: candidate, objB: current.Inventory)
                || !ValidInventory(current: current, candidate: candidate, population: population))
        {
            return false;
        }

        RunInventory updated = RunQuickReferences.Invalidate(inventory: candidate, definitions: current.Definitions);
        if (!TryPublish(participants: current.Snapshot.Participants, population: population, inventory: updated,
                world: drop?.World, loot: drop?.Loot))
        {
            return false;
        }

        change = new RunItemTransfer.Change(before: current.Inventory, after: updated);
        return true;
    }

    private static bool ValidInventory(RunPublication current, RunInventory candidate, RunPopulation population)
    {
        return RunInventoryRules.KeepsIdentities(current: current.Inventory, candidate: candidate)
                && RunInventoryRules.IsValid(inventory: candidate, definitions: current.Definitions,
                        population: population)
                && (current.Inventory.SavedLayout?.AllowsContinuation(before: current.Inventory, after: candidate,
                        definitions: current.Definitions) ?? true);
    }

    private void RequireActive()
    {
        if (!_active)
        {
            throw new InvalidOperationException(message: "FOUNDATION_CLOSED");
        }
    }

    internal sealed class RunPublication
    {
        internal RunPublication(FoundationSnapshot snapshot, RunWorld world, long elapsedMilliseconds,
                RunDefinitions definitions, RunPopulation population, RunInventory inventory, RunCombat.State combat,
                RunDeathAndLoot.State loot, RunSuccession.State succession)
        {
            Snapshot = snapshot;
            World = world;
            ElapsedMilliseconds = elapsedMilliseconds;
            Definitions = definitions;
            Population = population;
            Inventory = inventory;
            Combat = combat;
            Loot = loot;
            Succession = succession;
        }

        internal FoundationSnapshot Snapshot { get; }
        internal RunWorld World { get; }
        internal long ElapsedMilliseconds { get; }
        internal RunDefinitions Definitions { get; }
        internal RunPopulation Population { get; }
        internal RunInventory Inventory { get; }
        internal RunCombat.State Combat { get; }
        internal RunDeathAndLoot.State Loot { get; }
        internal RunSuccession.State Succession { get; }
    }
}
