using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Prepares one attack from current posture, held equipment and opposing targets.</summary>
internal static class RunCombat
{
    internal static (Status status, Candidate? candidate) Prepare(FoundationSession.RunPublication current,
            RunPopulation.Survivor attacker, Aim? aim)
    {
        RunInventory.Item? left = current.Inventory.Equipped(survivorId: attacker.Id, slot: "left-hand");
        RunInventory.Item? right = current.Inventory.Equipped(survivorId: attacker.Id, slot: "right-hand");
        if (left is { DefinitionId: "IT-07" })
        {
            return PrepareBow(current: current, attacker: attacker, bow: left, ammo: right, aim: aim);
        }

        if (right is { DefinitionId: "IT-07" })
        {
            return PrepareBow(current: current, attacker: attacker, bow: right, ammo: left, aim: aim);
        }

        if (aim is { Valid: false })
        {
            return (Status.InvalidAim, null);
        }

        if (CoolingDown(current: current, attacker: attacker))
        {
            return (Status.Cooldown, null);
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

    internal static BowView? ViewBow(RunInventory inventory, RunPopulation.Survivor survivor, Aim aim)
    {
        if (!aim.Valid)
        {
            return null;
        }

        RunInventory.Item? left = inventory.Equipped(survivorId: survivor.Id, slot: "left-hand");
        RunInventory.Item? right = inventory.Equipped(survivorId: survivor.Id, slot: "right-hand");
        RunInventory.Item? bow = left is { DefinitionId: "IT-07" } ? left
                : right is { DefinitionId: "IT-07" } ? right : null;
        RunInventory.Item? ammo = ReferenceEquals(objA: left, objB: bow) ? right : left;
        return bow is null ? null : new BowView(aim: aim, bowId: bow.Id,
                readyArrows: ammo is { DefinitionId: "IT-02" } ? ammo.Quantity : 0, usable: bow.Condition > 0);
    }

    private static (Status status, Candidate? candidate) PrepareBow(FoundationSession.RunPublication current,
            RunPopulation.Survivor attacker, RunInventory.Item bow, RunInventory.Item? ammo, Aim? aim)
    {
        if (bow.Condition is not > 0)
        {
            return (Status.BrokenWeapon, null);
        }

        if (ammo is null)
        {
            return (Status.EmptyAmmo, null);
        }

        if (ammo.DefinitionId is not "IT-02")
        {
            return (Status.IncompatibleHands, null);
        }

        if (aim is not { Valid: true })
        {
            return (Status.InvalidAim, null);
        }

        if (CoolingDown(current: current, attacker: attacker))
        {
            return (Status.Cooldown, null);
        }

        if (current.Combat.LastArrowId == long.MaxValue)
        {
            return (Status.Rejected, null);
        }

        RunCombatDefinitions.Attack rule = current.Definitions.Combat.Weapons[bow.DefinitionId];
        RunProjectiles.Arrow? arrow = RunProjectiles.Launch(current: current, shooter: attacker, aim: aim,
                damage: rule.Damage);
        if (arrow is null)
        {
            return (Status.InvalidAim, null);
        }

        RunPopulation.Survivor? paid = Pay(current: current, attacker: attacker, cost: rule.Stamina,
                interval: rule.Interval);
        if (paid is null)
        {
            return (Status.InsufficientStamina, null);
        }

        RunInventory? spent = RunItemTransfer.RemoveOne(inventory: current.Inventory,
                expected: current.Inventory.PathTo(itemId: ammo.Id));
        return spent is null ? (Status.Rejected, null) : (Status.Accepted, new Candidate(
                population: current.Population.Replace(previous: attacker, current: paid),
                inventory: RunDamage.Wear(inventory: spent, item: bow, amount: current.Definitions.Combat.BowWear),
                state: current.Combat.WithArrow(arrow: arrow), hits: [], arrowId: arrow.Id));
    }

    private static bool CoolingDown(FoundationSession.RunPublication current, RunPopulation.Survivor attacker)
    {
        return attacker.AttackAvailableAt is { } ready && current.ElapsedMilliseconds < ready;
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
        return population.Survivors.Where(predicate: target => target.Health > 0 && target.Hostile != attacker.Hostile)
                .Select(selector: target =>
                {
                    (decimal x, decimal y) contact = Contact(world: world, survivor: target);
                    decimal x = contact.x - origin.x;
                    decimal y = contact.y - origin.y;
                    return (target, contact, ahead: x * attacker.Facing >= 0, distance: x * x + y * y);
                })
                .Where(predicate: target => target.ahead && target.distance <= reach * reach
                        && !world.Blockers.Any(predicate: blocker => RunWorldGeometry.FirstContact(start: origin,
                                end: target.contact, bounds: (blocker.Minimum.X, blocker.Minimum.Y,
                                        blocker.Maximum.X, blocker.Maximum.Y)).HasValue))
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
        EmptyAmmo,
        InvalidAim,
        Rejected,
    }

    internal sealed record Aim
    {
        internal Aim(double x, double y)
        {
            X = x;
            Y = y;
        }

        internal double X { get; }
        internal double Y { get; }
        internal bool Valid => double.IsFinite(d: X) && double.IsFinite(d: Y)
                && Math.Abs(value: X) <= 1_000_000_000 && Math.Abs(value: Y) <= 1_000_000_000;
    }

    internal sealed record BowView
    {
        internal BowView(Aim aim, long bowId, int readyArrows, bool usable)
        {
            Aim = aim;
            BowId = bowId;
            ReadyArrows = readyArrows;
            Usable = usable;
        }

        internal Aim Aim { get; }
        internal long BowId { get; }
        internal int ReadyArrows { get; }
        internal bool Usable { get; }
    }

    internal sealed class Candidate
    {
        internal Candidate(RunPopulation population, RunInventory inventory, State state,
                ImmutableArray<RunDamage.Hit> hits, long? arrowId = null)
        {
            Population = population;
            Inventory = inventory;
            Combat = state;
            Hits = hits;
            ArrowId = arrowId;
        }

        internal RunPopulation Population { get; }
        internal RunInventory Inventory { get; }
        internal State Combat { get; }
        internal ImmutableArray<RunDamage.Hit> Hits { get; }
        internal long? ArrowId { get; }
    }

    internal sealed class State
    {
        internal State(ImmutableArray<RunDamage.Hit> fatalHits, ImmutableArray<RunProjectiles.Arrow> arrows,
                ImmutableArray<RunProjectiles.Impact> impacts, long lastArrowId)
        {
            FatalHits = fatalHits;
            Arrows = arrows;
            Impacts = impacts;
            LastArrowId = lastArrowId;
        }

        internal static State Empty { get; } = new(fatalHits: [], arrows: [], impacts: [], lastArrowId: 0);
        internal ImmutableArray<RunDamage.Hit> FatalHits { get; }
        internal ImmutableArray<RunProjectiles.Arrow> Arrows { get; }
        internal ImmutableArray<RunProjectiles.Impact> Impacts { get; }
        internal long LastArrowId { get; }

        internal State WithHits(ImmutableArray<RunDamage.Hit> hits)
        {
            return hits.Any(predicate: hit => hit.Lethal)
                    ? new State(fatalHits: FatalHits.AddRange(items: hits.Where(predicate: hit => hit.Lethal)),
                            arrows: Arrows, impacts: Impacts, lastArrowId: LastArrowId) : this;
        }

        internal State WithArrow(RunProjectiles.Arrow arrow)
        {
            return new State(fatalHits: FatalHits, arrows: Arrows.Add(item: arrow), impacts: Impacts,
                    lastArrowId: arrow.Id);
        }

        internal State WithFlight(ImmutableArray<RunProjectiles.Arrow> arrows,
                ImmutableArray<RunProjectiles.Impact> impacts)
        {
            IEnumerable<RunDamage.Hit> fatalities = impacts.Select(selector: impact => impact.Hit)
                    .OfType<RunDamage.Hit>().Where(predicate: hit => hit.Lethal);
            return new State(fatalHits: FatalHits.AddRange(items: fatalities), arrows: arrows, impacts: impacts,
                    lastArrowId: LastArrowId);
        }
    }

    internal sealed class Outcome
    {
        internal Outcome(Status status, ImmutableArray<RunDamage.Hit> hits = default, bool replayed = false,
                long? arrowId = null)
        {
            Status = status;
            Hits = hits.IsDefault ? [] : hits;
            Replayed = replayed;
            ArrowId = arrowId;
        }

        internal Status Status { get; }
        internal ImmutableArray<RunDamage.Hit> Hits { get; }
        internal bool Replayed { get; }
        internal long? ArrowId { get; }
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
