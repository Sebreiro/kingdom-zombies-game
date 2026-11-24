using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

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
            return true;
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
            if (!_active || participant is null || !participant.Ready || lease is null
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
            FoundationParticipantState[] candidate = [.. current.Snapshot.Participants.Select(selector: participant =>
                    AdvanceParticipant(participant: participant, population: population,
                            elapsedMilliseconds: elapsedMilliseconds))];
            var snapshot = new FoundationSnapshot(sessionId: current.Snapshot.SessionId,
                    revision: checked(current.Snapshot.Revision + 1), participants: candidate);
            _publication = new RunPublication(snapshot: snapshot, world: current.World, elapsedMilliseconds: elapsed,
                    definitions: current.Definitions, population: population, inventory: current.Inventory);
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
        _publication = new RunPublication(snapshot: snapshot, world: world, elapsedMilliseconds: 0,
                definitions: definitions, population: population, inventory: inventory);
        ItemActions = new RunItemActions(session: this);
    }

    internal RunPopulation Population => _publication.Population;
    internal RunInventory Inventory => _publication.Inventory;
    internal RunDefinitions Definitions => _publication.Definitions;
    internal RunItemActions ItemActions { get; }

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
            out RunItemTransfer.Change? change)
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
            if (!_active || !ReferenceEquals(objA: current, objB: _publication) || candidate is null
                    || ReferenceEquals(objA: candidate, objB: current.Inventory)
                    || !RunInventoryRules.KeepsIdentities(current: current.Inventory, candidate: candidate)
                    || !RunInventoryRules.IsValid(inventory: candidate, definitions: current.Definitions,
                            population: current.Population))
            {
                return false;
            }

            RunInventory updated = RunQuickReferences.Invalidate(inventory: candidate,
                    definitions: current.Definitions);
            if (!TryPublish(participants: current.Snapshot.Participants, inventory: updated))
            {
                return false;
            }

            change = new RunItemTransfer.Change(before: current.Inventory, after: updated);
            return true;
        }
    }

    private readonly Lock _gate = new();
    private readonly Dictionary<string, (long generation, FoundationHeldIntent intent)> _intents =
            new(comparer: StringComparer.Ordinal);
    private volatile RunPublication _publication;
    private volatile bool _active = true;

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
                    inventory: current.Inventory, intent: intent, generation: lease?.Generation ?? 0,
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
            RunPopulation? population = null, RunInventory? inventory = null)
    {
        try
        {
            world?.Validate();
            RunPublication current = _publication;
            var snapshot = new FoundationSnapshot(sessionId: current.Snapshot.SessionId,
                    revision: checked(current.Snapshot.Revision + 1), participants: participants);
            _publication = new RunPublication(snapshot: snapshot, world: world ?? current.World,
                    elapsedMilliseconds: current.ElapsedMilliseconds, definitions: current.Definitions,
                    population: population ?? current.Population, inventory: inventory ?? current.Inventory);
            return true;
        }
        catch (Exception error) when (error is InvalidDataException or OverflowException)
        {
            return false;
        }
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
                RunDefinitions definitions, RunPopulation population, RunInventory inventory)
        {
            Snapshot = snapshot;
            World = world;
            ElapsedMilliseconds = elapsedMilliseconds;
            Definitions = definitions;
            Population = population;
            Inventory = inventory;
        }

        internal FoundationSnapshot Snapshot { get; }
        internal RunWorld World { get; }
        internal long ElapsedMilliseconds { get; }
        internal RunDefinitions Definitions { get; }
        internal RunPopulation Population { get; }
        internal RunInventory Inventory { get; }
    }
}
