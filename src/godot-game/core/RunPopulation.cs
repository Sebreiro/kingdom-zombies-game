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

    internal RunInventory CreateInitialInventory(RunDefinitions definitions)
    {
        ImmutableArray<RunInventory.Item>.Builder items = ImmutableArray.CreateBuilder<RunInventory.Item>();
        foreach (Survivor survivor in Survivors)
        {
            foreach (string id in definitions.StartingOutfit)
            {
                RunDefinitions.Item definition = definitions.Items[id];
                string slot = definition.Slot ?? throw new InvalidOperationException(message: "RUN_OUTFIT_SLOT");
                items.Add(item: new RunInventory.Item(id: items.Count + 1L, definitionId: id, quantity: 1,
                        condition: definition.MaximumCondition, location: new RunInventory.Equipment(
                                survivorId: survivor.Id, slot: slot), storageGrid: definition.Storage));
            }
        }

        var inventory = new RunInventory(items: items.ToImmutable(), stashGrid: definitions.StashGrid,
                stashPageCount: 1, nextItemId: items.Count + 1L);
        foreach (RunDefinitions.StartingStack stack in definitions.StartingStash)
        {
            RunDefinitions.Item definition = definitions.Items[stack.ItemId];
            for (int index = 0; index < stack.Count; index++)
            {
                inventory = RunStash.AddStartingStack(inventory: inventory, definition: definition,
                        quantity: stack.Quantity);
            }
        }

        return inventory;
    }

    internal RunPopulation? Reserve(string sessionId, string playerId, string bindingId, int slot)
    {
        if (slot is < 2 or > 4 || Leases.Any(predicate: lease =>
                !StringComparer.Ordinal.Equals(x: sessionId, y: lease.SessionId))
                || string.IsNullOrWhiteSpace(value: bindingId) || IssuedBindings.Contains(item: bindingId)
                || Leases.Any(predicate: lease => lease.Slot == slot
                        || StringComparer.Ordinal.Equals(x: lease.PlayerId, y: playerId)))
        {
            return null;
        }

        Survivor? available = Candidates().FirstOrDefault();
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

    internal RunPopulation Replace(Survivor previous, Survivor current)
    {
        return previous == current ? this : new RunPopulation(
                survivors: Survivors.Replace(oldValue: previous, newValue: current), leases: Leases,
                issuedBindings: IssuedBindings, lastControlGeneration: LastControlGeneration);
    }

    internal RunPopulation Add(Survivor survivor)
    {
        return new RunPopulation(survivors: Survivors.Add(item: survivor), leases: Leases,
                issuedBindings: IssuedBindings, lastControlGeneration: LastControlGeneration);
    }

    internal ImmutableArray<Survivor> Candidates()
    {
        return [.. Survivors.Where(predicate: survivor => survivor.EligibleForControl && !Leases.Any(
                predicate: lease => StringComparer.Ordinal.Equals(x: lease.SurvivorId, y: survivor.Id)))
                .OrderBy(keySelector: survivor => survivor.Id, comparer: StringComparer.Ordinal)];
    }

    internal RunPopulation AssignSuccessor(Lease retired, Survivor successor)
    {
        long generation = checked(LastControlGeneration + 1);
        var lease = new Lease(sessionId: retired.SessionId, playerId: retired.PlayerId,
                bindingId: retired.BindingId, slot: retired.Slot, survivorId: successor.Id, generation: generation,
                reassigned: true);
        return new RunPopulation(survivors: Survivors, leases: Leases.Add(item: lease),
                issuedBindings: IssuedBindings, lastControlGeneration: generation);
    }

    internal sealed record Survivor
    {
        internal Survivor(string id, long positionSubunits, int facing, decimal health, decimal stamina,
                string role)
        {
            Id = id;
            PositionSubunitsExact = positionSubunits;
            Facing = facing;
            Health = health;
            Stamina = stamina;
            Role = role;
        }

        internal string Id { get; init; }
        internal long PositionSubunits => decimal.ToInt64(d: decimal.Round(d: PositionSubunitsExact));
        internal decimal PositionSubunitsExact { get; init; }
        internal decimal HeightSubunits { get; init; }
        internal int Facing { get; init; }
        internal decimal Health { get; init; }
        internal decimal Stamina { get; init; }
        internal string Role { get; init; }
        internal decimal? LastStaminaSpend { get; init; }
        internal long? JumpStartedAt { get; init; }
        internal (long generation, long sequence)? SprintFence { get; init; }
        internal RunLocomotion.Block SprintBlock { get; init; }
        internal decimal? RestStartedAt { get; init; }
        internal decimal? FoodAvailableAt { get; init; }
        internal decimal? AttackAvailableAt { get; init; }
        internal string MovementPose { get; init; } = "idle";
        internal decimal? StrikeAt { get; init; }
        internal RunCombat.Aim? StrikeAim { get; init; }
        internal decimal? FlinchAt { get; init; }
        internal bool Hostile { get; init; }
        internal bool EligibleForControl => !Hostile && Health > 0;
    }

    internal sealed record Lease
    {
        internal Lease(string sessionId, string playerId, string bindingId, int slot, string survivorId,
                long generation, bool reassigned = false)
        {
            SessionId = sessionId;
            PlayerId = playerId;
            BindingId = bindingId;
            Slot = slot;
            SurvivorId = survivorId;
            Generation = generation;
            Reassigned = reassigned;
        }

        internal string SessionId { get; }
        internal string PlayerId { get; }
        internal string BindingId { get; }
        internal int Slot { get; }
        internal string SurvivorId { get; }
        internal long Generation { get; }
        internal bool Reassigned { get; }
    }
}
