using System;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Stores the ordinary recruits independently of transport participation.</summary>
internal sealed class RunPopulation
{
    internal const long StartingPosition = -8000;

    internal RunPopulation(ImmutableArray<Survivor> survivors, ImmutableArray<Lease> leases,
            ImmutableHashSet<string> issuedBindings, long lastControlGeneration)
    {
        Survivors = survivors;
        Leases = leases;
        IssuedBindings = issuedBindings;
        LastControlGeneration = lastControlGeneration;
    }

    internal ImmutableArray<Survivor> Survivors { get; }
    internal ImmutableArray<Lease> Leases { get; }
    internal ImmutableHashSet<string> IssuedBindings { get; }
    internal long LastControlGeneration { get; }

    internal static RunPopulation Create(RunDefinitions definitions, string sessionId, string playerId)
    {
        ImmutableArray<Survivor> survivors = [.. Enumerable.Range(start: 1, count: definitions.SurvivorCount)
                .Select(selector: ordinal => new Survivor(
                        id: FormattableString.Invariant(formattable: $"survivor-{ordinal:D2}"),
                        positionSubunits: StartingPosition, facing: 1, health: definitions.MaximumHealth,
                        stamina: definitions.MaximumStamina, role: definitions.InitialRole))];
        return new RunPopulation(survivors: survivors, leases: [new Lease(sessionId: sessionId,
                playerId: playerId, bindingId: "local", slot: 1, survivorId: survivors[0].Id, generation: 1)],
                issuedBindings: ImmutableHashSet.Create(equalityComparer: StringComparer.Ordinal, item: "local"),
                lastControlGeneration: 1);
    }

    internal RunPopulation? Reserve(string sessionId, string playerId, string bindingId, int slot)
    {
        if (slot is < 2 or > 4 || !StringComparer.Ordinal.Equals(x: sessionId, y: Leases[0].SessionId)
                || string.IsNullOrWhiteSpace(value: bindingId) || IssuedBindings.Contains(item: bindingId)
                || Leases.Any(predicate: lease => lease.Slot == slot
                        || StringComparer.Ordinal.Equals(x: lease.PlayerId, y: playerId)))
        {
            return null;
        }

        Survivor? available = Survivors.FirstOrDefault(predicate: survivor => survivor.Health > 0
                && !Leases.Any(predicate: lease => StringComparer.Ordinal.Equals(x: lease.SurvivorId, y: survivor.Id)));
        if (available is null || LastControlGeneration == long.MaxValue)
        {
            return null;
        }

        long generation = LastControlGeneration + 1;
        var reserved = new Lease(sessionId: sessionId, playerId: playerId, bindingId: bindingId, slot: slot,
                survivorId: available.Id, generation: generation);
        return new RunPopulation(survivors: Survivors, leases: Leases.Add(item: reserved),
                issuedBindings: IssuedBindings.Add(item: bindingId), lastControlGeneration: generation);
    }

    internal RunPopulation Release(string bindingId)
    {
        return new RunPopulation(survivors: Survivors, leases: Leases.RemoveAll(match: lease =>
                StringComparer.Ordinal.Equals(x: lease.BindingId, y: bindingId)),
                issuedBindings: IssuedBindings, lastControlGeneration: LastControlGeneration);
    }

    internal Lease? FindLease(string bindingId)
    {
        return Leases.FirstOrDefault(predicate: lease => StringComparer.Ordinal.Equals(x: lease.BindingId,
                y: bindingId));
    }

    internal Survivor ForLease(Lease lease)
    {
        return Survivors.First(predicate: survivor => StringComparer.Ordinal.Equals(x: survivor.Id,
                y: lease.SurvivorId));
    }

    internal Lease? FindLivingControl(string bindingId)
    {
        Lease? lease = FindLease(bindingId: bindingId);
        return lease is not null && ForLease(lease: lease).Health > 0 ? lease : null;
    }

    internal RunPopulation ApplyPose(FoundationParticipantState participant)
    {
        if (!participant.Ready || FindLivingControl(bindingId: participant.BindingId) is not { } lease)
        {
            return this;
        }

        Survivor survivor = ForLease(lease: lease);
        FoundationActorState actor = participant.Actor;
        if (actor.PositionSubunits == survivor.PositionSubunits && actor.Facing == survivor.Facing)
        {
            return this;
        }

        return new RunPopulation(survivors: Survivors.Replace(oldValue: survivor, newValue: survivor with
        {
            PositionSubunits = actor.PositionSubunits,
            Facing = actor.Facing,
        }), leases: Leases, issuedBindings: IssuedBindings, lastControlGeneration: LastControlGeneration);
    }

    internal sealed record Survivor
    {
        internal Survivor(string id, long positionSubunits, int facing, decimal health, decimal stamina,
                string role)
        {
            Id = id;
            PositionSubunits = positionSubunits;
            Facing = facing;
            Health = health;
            Stamina = stamina;
            Role = role;
        }

        internal string Id { get; init; }
        internal long PositionSubunits { get; init; }
        internal int Facing { get; init; }
        internal decimal Health { get; init; }
        internal decimal Stamina { get; init; }
        internal string Role { get; init; }
    }

    internal sealed record Lease
    {
        internal Lease(string sessionId, string playerId, string bindingId, int slot, string survivorId,
                long generation)
        {
            SessionId = sessionId;
            PlayerId = playerId;
            BindingId = bindingId;
            Slot = slot;
            SurvivorId = survivorId;
            Generation = generation;
        }

        internal string SessionId { get; }
        internal string PlayerId { get; }
        internal string BindingId { get; }
        internal int Slot { get; }
        internal string SurvivorId { get; }
        internal long Generation { get; }
    }
}
