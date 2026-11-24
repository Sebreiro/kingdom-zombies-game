using System;
using System.Collections.Generic;
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

    /// <summary>Gets the local host actor's current safety and shared stash access at the committed run time.</summary>
    public (long elapsedMilliseconds, bool insideCamp, bool safeDistance, bool stashReachable) LocalWorld
    {
        get
        {
            RunPublication current = _publication;
            RunWorld.Point contact = current.World.ActorContact(actor: current.Snapshot.Participants[0].Actor);
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
                    || command.Direction is < -1 or > 1 || command.Sequence <= 0
                    || !Matches(command: command, participant: participant, trustedPlayer: trustedPlayer,
                            trustedActor: trustedActor)
                    || _intents.TryGetValue(key: trustedBinding,
                            value: out (long generation, FoundationHeldIntent intent) previous)
                            && previous.generation == lease.Generation && command.Sequence <= previous.intent.Sequence)
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
            FoundationParticipantState[] candidate = [.. current.Snapshot.Participants.Select(selector: participant =>
                    AdvanceParticipant(participant: participant, population: current.Population,
                            elapsedMilliseconds: elapsedMilliseconds))];
            var snapshot = new FoundationSnapshot(sessionId: current.Snapshot.SessionId,
                    revision: checked(current.Snapshot.Revision + 1), participants: candidate);
            RunPopulation population = current.Population;
            foreach (FoundationParticipantState participant in candidate)
            {
                population = population.ApplyPose(participant: participant);
            }

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
        var snapshot = new FoundationSnapshot(sessionId: sessionId, revision: 0, participants:
                [new FoundationParticipantState(bindingId: "local", label: "Player 1", slot: 1, ready: true,
                        actor: new FoundationActorState(sessionId: sessionId, playerId: playerId, actorId: actorId,
                                positionSubunits: RunPopulation.StartingPosition, facing: 1,
                                elapsedMilliseconds: 0))]);
        _publication = new RunPublication(snapshot: snapshot, world: world, elapsedMilliseconds: 0,
                definitions: definitions, population: population, inventory: inventory);
    }

    internal RunPopulation Population => _publication.Population;
    internal RunInventory Inventory => _publication.Inventory;
    internal RunDefinitions Definitions => _publication.Definitions;

    internal bool CanReach(string bindingId, RunWorld.Access kind, string targetId)
    {
        RunPublication current = _publication;
        FoundationActorState? actor = ReadyActor(current: current, bindingId: bindingId);
        return actor is not null && current.World.Reachable(
                actor: current.World.ActorContact(actor: actor), kind: kind, id: targetId);
    }

    internal (bool insideCamp, bool safeDistance)? SafetyFor(string bindingId)
    {
        RunPublication current = _publication;
        FoundationActorState? actor = ReadyActor(current: current, bindingId: bindingId);
        return actor is null ? null : current.World.Safety(actor: current.World.ActorContact(actor: actor));
    }

    internal bool TryReplaceRunWorld(RunWorld? candidate)
    {
        lock (_gate)
        {
            return _active && candidate is not null && TryPublish(participants: Snapshot.Participants,
                    world: candidate);
        }
    }

    private readonly Lock _gate = new();
    private readonly Dictionary<string, (long generation, FoundationHeldIntent intent)> _intents =
            new(comparer: StringComparer.Ordinal);
    private volatile RunPublication _publication;
    private volatile bool _active = true;

    private FoundationActorState? ReadyActor(RunPublication current, string bindingId)
    {
        return _active && Find(snapshot: current.Snapshot, bindingId: bindingId) is { Ready: true } participant
                && current.Population.FindLivingControl(bindingId: bindingId) is not null
                ? participant.Actor : null;
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

    private FoundationParticipantState AdvanceParticipant(FoundationParticipantState participant,
            RunPopulation population, long elapsedMilliseconds)
    {
        if (!participant.Ready || population.FindLivingControl(bindingId: participant.BindingId) is not { } lease)
        {
            return participant;
        }

        int direction = _intents.TryGetValue(key: participant.BindingId,
                value: out (long generation, FoundationHeldIntent intent) held)
                && held.generation == lease.Generation ? held.intent.Direction : 0;
        RunPopulation.Survivor survivor = population.ForLease(lease: lease);
        FoundationActorState actor = participant.Actor;
        long position = Math.Clamp(value: survivor.PositionSubunits + Math.Min(val1: elapsedMilliseconds, val2: 40000)
                * 3 * direction, min: -59000, max: 59000);
        var moved = new FoundationActorState(sessionId: actor.SessionId, playerId: actor.PlayerId,
                actorId: actor.ActorId, positionSubunits: position,
                facing: direction == 0 ? survivor.Facing : direction,
                elapsedMilliseconds: checked(actor.ElapsedMilliseconds + elapsedMilliseconds));
        return new FoundationParticipantState(bindingId: participant.BindingId, label: participant.Label,
                slot: participant.Slot, ready: participant.Ready, actor: moved);
    }

    private bool TryPublish(IEnumerable<FoundationParticipantState> participants, RunWorld? world = null,
            RunPopulation? population = null)
    {
        try
        {
            world?.Validate();
            RunPublication current = _publication;
            var snapshot = new FoundationSnapshot(sessionId: current.Snapshot.SessionId,
                    revision: checked(current.Snapshot.Revision + 1), participants: participants);
            _publication = new RunPublication(snapshot: snapshot, world: world ?? current.World,
                    elapsedMilliseconds: current.ElapsedMilliseconds, definitions: current.Definitions,
                    population: population ?? current.Population, inventory: current.Inventory);
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

    private sealed class RunPublication
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
