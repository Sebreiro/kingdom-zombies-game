using System;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Prepares one attack from current posture, held equipment and opposing targets.</summary>
internal static class RunCombat
{
    internal static (Status status, Candidate? candidate) Prepare(FoundationSession.RunPublication current,
            RunPopulation.Survivor attacker)
    {
        if (attacker.AttackAvailableAt is { } ready && current.ElapsedMilliseconds < ready)
        {
            return (Status.Cooldown, null);
        }

        RunInventory.Item? left = current.Inventory.Equipped(survivorId: attacker.Id, slot: "left-hand");
        RunInventory.Item? right = current.Inventory.Equipped(survivorId: attacker.Id, slot: "right-hand");
        if (left?.DefinitionId is "IT-07" || right?.DefinitionId is "IT-07")
        {
            return (Status.IncompatibleHands, null);
        }

        ImmutableArray<Strike> strikes = Strikes(definitions: current.Definitions.Combat, left: left,
                right: right);
        if (strikes.IsEmpty)
        {
            return (left is { Condition: 0 } && right is { Condition: 0 }
                    ? Status.BrokenWeapon : Status.HandsBusy, null);
        }

        decimal cost = strikes.Sum(selector: strike => strike.Rule.Stamina);
        decimal interval = strikes.Max(selector: strike => strike.Rule.Interval);
        RunPopulation.Survivor? paid = Pay(current: current, attacker: attacker, cost: cost, interval: interval);
        if (paid is null)
        {
            return (Status.InsufficientStamina, null);
        }

        return (Status.Accepted, StrikeTargets(current: current, original: attacker, attacker: paid, strikes: strikes));
    }

    internal static (decimal x, decimal y) Contact(RunWorld world, RunPopulation.Survivor survivor)
    {
        return (world.Origin.X + survivor.PositionSubunitsExact,
                world.Origin.Y + world.GroundContactSubunits + survivor.HeightSubunits);
    }

    private static RunPopulation.Survivor? Pay(FoundationSession.RunPublication current,
            RunPopulation.Survivor attacker, decimal cost, decimal interval)
    {
        RunLocomotion.Burden load = RunLocomotion.Load(inventory: current.Inventory, definitions: current.Definitions,
                survivorId: attacker.Id);
        RunPopulation.Survivor? paid = RunVitals.Spend(survivor: attacker, amount: cost * load.ActionCost,
                now: current.ElapsedMilliseconds);
        return paid is null ? null : paid with { AttackAvailableAt = current.ElapsedMilliseconds + interval };
    }

    private static ImmutableArray<Strike> Strikes(RunCombatDefinitions definitions, RunInventory.Item? left,
            RunInventory.Item? right)
    {
        ImmutableArray<Strike>.Builder result = ImmutableArray.CreateBuilder<Strike>();
        foreach (RunInventory.Item? hand in new[] { left, right })
        {
            if (hand is { Condition: > 0 } && definitions.Weapons.TryGetValue(key: hand.DefinitionId,
                    value: out RunCombatDefinitions.Attack? weapon) && weapon.Reach.HasValue)
            {
                result.Add(item: new Strike(rule: weapon, item: hand));
            }
        }

        if (result.Count == 0 && (left is null || right is null))
        {
            result.Add(item: new Strike(rule: definitions.Fists, item: null));
        }

        return result.ToImmutable();
    }

    private static Candidate StrikeTargets(FoundationSession.RunPublication current, RunPopulation.Survivor original,
            RunPopulation.Survivor attacker, ImmutableArray<Strike> strikes)
    {
        RunPopulation population = current.Population.Replace(previous: original, current: attacker);
        RunInventory inventory = current.Inventory;
        RunCombatDefinitions rules = current.Definitions.Combat;
        ImmutableArray<RunDamage.Hit>.Builder hits = ImmutableArray.CreateBuilder<RunDamage.Hit>();
        foreach (Strike strike in strikes)
        {
            RunPopulation.Survivor? target = PickTarget(world: current.World, population: population,
                    attacker: attacker, reach: strike.Rule.Reach.GetValueOrDefault());
            if (target is null)
            {
                continue;
            }

            (inventory, RunPopulation.Survivor wounded, RunDamage.Hit? receipt) = RunDamage.Apply(
                    inventory: inventory, definitions: rules, survivor: target, sourceId: attacker.Id,
                    sourceHostile: attacker.Hostile, raw: strike.Rule.Damage, at: current.ElapsedMilliseconds);
            if (receipt is not null)
            {
                population = population.Replace(previous: target, current: wounded);
                hits.Add(item: receipt);
                if (receipt.Raw > 0 && strike.Item is { } weapon)
                {
                    inventory = RunDamage.Wear(inventory: inventory, item: weapon, amount: rules.MeleeWear);
                }
            }
        }

        ImmutableArray<RunDamage.Hit> applied = hits.ToImmutable();
        return new Candidate(population: population, inventory: inventory,
                state: current.Combat.WithHits(hits: applied), hits: applied);
    }

    private static RunPopulation.Survivor? PickTarget(RunWorld world, RunPopulation population,
            RunPopulation.Survivor attacker, decimal reach)
    {
        (decimal x, decimal y) origin = Contact(world: world, survivor: attacker);
        RunWorld.Point start = world.ActorContact(survivor: attacker);
        return population.Survivors.Where(predicate: target => target.Health > 0 && target.Hostile != attacker.Hostile)
                .Select(selector: target =>
                {
                    (decimal x, decimal y) contact = Contact(world: world, survivor: target);
                    decimal x = contact.x - origin.x;
                    decimal y = contact.y - origin.y;
                    return (target, ahead: x * attacker.Facing >= 0, distance: x * x + y * y);
                })
                .Where(predicate: target => target.ahead && target.distance <= reach * reach
                        && !world.Blockers.Any(predicate: blocker => RunWorldGeometry.Blocks(blocker: blocker,
                                start: start, end: world.ActorContact(survivor: target.target))))
                .OrderBy(keySelector: target => target.distance)
                .ThenBy(keySelector: target => target.target.Id, comparer: StringComparer.Ordinal)
                .Select(selector: target => target.target).FirstOrDefault();
    }

    internal enum Status
    {
        Accepted,
        AccessLost,
        StaleRequest,
        Cooldown,
        InsufficientStamina,
        HandsBusy,
        BrokenWeapon,
        IncompatibleHands,
        Rejected,
    }

    internal sealed class Candidate
    {
        internal Candidate(RunPopulation population, RunInventory inventory, State state,
                ImmutableArray<RunDamage.Hit> hits)
        {
            Population = population;
            Inventory = inventory;
            Combat = state;
            Hits = hits;
        }

        internal RunPopulation Population { get; }
        internal RunInventory Inventory { get; }
        internal State Combat { get; }
        internal ImmutableArray<RunDamage.Hit> Hits { get; }
    }

    internal sealed class State
    {
        internal State(ImmutableArray<RunDamage.Hit> fatalHits)
        {
            FatalHits = fatalHits;
        }

        internal static State Empty { get; } = new(fatalHits: []);
        internal ImmutableArray<RunDamage.Hit> FatalHits { get; }

        internal State WithHits(ImmutableArray<RunDamage.Hit> hits)
        {
            return hits.Any(predicate: hit => hit.Lethal)
                    ? new State(fatalHits: FatalHits.AddRange(items: hits.Where(predicate: hit => hit.Lethal))) : this;
        }
    }

    internal sealed class Outcome
    {
        internal Outcome(Status status, ImmutableArray<RunDamage.Hit> hits = default, bool replayed = false)
        {
            Status = status;
            Hits = hits.IsDefault ? [] : hits;
            Replayed = replayed;
        }

        internal Status Status { get; }
        internal ImmutableArray<RunDamage.Hit> Hits { get; }
        internal bool Replayed { get; }
    }

    private sealed class Strike
    {
        internal Strike(RunCombatDefinitions.Attack rule, RunInventory.Item? item)
        {
            Rule = rule;
            Item = item;
        }

        internal RunCombatDefinitions.Attack Rule { get; }
        internal RunInventory.Item? Item { get; }
    }
}
