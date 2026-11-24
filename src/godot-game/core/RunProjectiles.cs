using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Sweeps spent arrows through the committed elapsed interval in first-contact order.</summary>
internal static class RunProjectiles
{
    internal static Arrow? Launch(FoundationSession.RunPublication current, RunPopulation.Survivor shooter,
            RunCombat.Aim aim, decimal damage)
    {
        (decimal x, decimal y) origin = RunCombat.Contact(world: current.World, survivor: shooter);
        decimal dx = (decimal)aim.X - origin.x;
        decimal dy = (decimal)aim.Y - origin.y;
        decimal scale = Math.Max(val1: Math.Abs(value: dx), val2: Math.Abs(value: dy));
        if (scale == 0)
        {
            return null;
        }

        dx /= scale;
        dy /= scale;
        decimal square = dx * dx + dy * dy;
        decimal distance = (decimal)Math.Sqrt(d: (double)square);
        distance = (distance + square / distance) / 2;
        RunCombatDefinitions.Projectile rule = current.Definitions.Combat.Arrow;
        return new Arrow(id: checked(current.Combat.LastArrowId + 1), sourceId: shooter.Id, hostile: shooter.Hostile,
                damage: damage, launchedAt: current.ElapsedMilliseconds,
                expiresAt: current.ElapsedMilliseconds + rule.Lifetime, position: origin,
                velocity: (dx * rule.Speed / distance, dy * rule.Speed / distance), aim: aim);
    }

    internal static Frame Advance(FoundationSession.RunPublication current, RunPopulation population, long now)
    {
        var frame = new Frame(world: current.World, population: population, inventory: current.Inventory,
                combat: current.Combat);
        if (now == current.ElapsedMilliseconds || (current.Combat.Arrows.IsEmpty && current.Combat.Impacts.IsEmpty))
        {
            return frame;
        }

        var spent = new HashSet<long>();
        ImmutableArray<Impact>.Builder impacts = ImmutableArray.CreateBuilder<Impact>();
        foreach (Contact contact in Contacts(current: current, population: population, now: now))
        {
            if (!spent.Contains(item: contact.Arrow.Id) && Apply(frame: frame, contact: contact,
                    rules: current.Definitions.Combat) is { } impact)
            {
                spent.Add(item: contact.Arrow.Id);
                impacts.Add(item: impact);
            }
        }

        frame.Combat = current.Combat.WithFlight(arrows: [.. current.Combat.Arrows.Where(predicate: arrow =>
                !spent.Contains(item: arrow.Id) && now < arrow.ExpiresAt)], impacts: impacts.ToImmutable());
        return frame;
    }

    private static IEnumerable<Contact> Contacts(FoundationSession.RunPublication current, RunPopulation population,
            long now)
    {
        RunCombat.State state = current.Combat;
        if (state.Arrows.IsEmpty)
        {
            return [];
        }

        var old = current.Population.Survivors.ToDictionary(keySelector: survivor => survivor.Id,
                comparer: StringComparer.Ordinal);
        return state.Arrows.SelectMany(selector: arrow => Trace(arrow: arrow, world: current.World,
                population: population, old: old, previous: current.ElapsedMilliseconds, now: now))
                .OrderBy(keySelector: contact => contact.At)
                .ThenBy(keySelector: contact => contact.Arrow.Id)
                .ThenBy(keySelector: contact => contact.Kind)
                .ThenBy(keySelector: contact => contact.TargetId, comparer: StringComparer.Ordinal);
    }

    private static IEnumerable<Contact> Trace(Arrow arrow, RunWorld world, RunPopulation population,
            Dictionary<string, RunPopulation.Survivor> old, long previous, long now)
    {
        decimal from = Math.Max(val1: previous, val2: arrow.LaunchedAt);
        decimal to = Math.Min(val1: now, val2: arrow.ExpiresAt);
        if (to < from)
        {
            yield break;
        }

        var span = new Sweep(arrow: arrow, from: from, to: to);
        foreach (RunWorld.Blocker terrain in world.Blockers)
        {
            if (span.Against(bounds: (terrain.Minimum.X, terrain.Minimum.Y, terrain.Maximum.X, terrain.Maximum.Y),
                    id: terrain.Id, kind: Kind.Terrain) is { } contact)
            {
                yield return contact;
            }
        }

        if (arrow.Hostile)
        {
            foreach (RunWorld.Defense defense in world.Defenses)
            {
                if (defense is { Completed: true, Living: true } && span.Against(
                        bounds: (defense.Contact.X - 250, defense.Contact.Y, defense.Contact.X + 250,
                                defense.Contact.Y + 2500), id: defense.Id, kind: Kind.Defense) is { } contact)
                {
                    yield return contact;
                }
            }
        }

        foreach (RunPopulation.Survivor target in population.Survivors)
        {
            if (target.Health > 0 && target.Hostile != arrow.Hostile
                    && AgainstSurvivor(span: span, world: world, target: target,
                            original: old.GetValueOrDefault(key: target.Id) ?? target, previous: previous,
                            now: now) is { } contact)
            {
                yield return contact;
            }
        }
    }

    private static Contact? AgainstSurvivor(Sweep span, RunWorld world, RunPopulation.Survivor target,
            RunPopulation.Survivor original, long previous, long now)
    {
        (decimal x, decimal y) start = RunCombat.Contact(world: world, survivor: original);
        (decimal x, decimal y) end = RunCombat.Contact(world: world, survivor: target);
        (decimal x, decimal y) first = Between(first: start, last: end, fraction: (span.From - previous) /
                (now - previous));
        (decimal x, decimal y) last = Between(first: start, last: end, fraction: (span.To - previous) /
                (now - previous));
        decimal? offset = RunWorldGeometry.FirstContact(
                start: (span.Start.x - first.x, span.Start.y - first.y),
                end: (span.End.x - last.x, span.End.y - last.y), bounds: (-250, 0, 250, 1000),
                duration: span.To - span.From);
        return span.ContactAt(offset: offset, id: target.Id, kind: Kind.Survivor);
    }

    private static (decimal x, decimal y) Between((decimal x, decimal y) first, (decimal x, decimal y) last,
            decimal fraction)
    {
        return (first.x + (last.x - first.x) * fraction, first.y + (last.y - first.y) * fraction);
    }

    private static Impact? Apply(Frame frame, Contact contact, RunCombatDefinitions rules)
    {
        if (contact.Kind == Kind.Terrain)
        {
            return new Impact(contact: contact, damage: 0);
        }

        if (contact.Kind == Kind.Defense)
        {
            RunWorld.Defense defense = frame.World.Defenses.FirstOrDefault(predicate: item =>
                    StringComparer.Ordinal.Equals(x: item.Id, y: contact.TargetId));
            if (!defense.Living)
            {
                return null;
            }

            decimal health = Math.Max(val1: 0, val2: defense.Health - contact.Arrow.Damage);
            frame.World = frame.World.WithDefenses(defenses: frame.World.Defenses.Replace(oldValue: defense,
                    newValue: defense with { Health = health, Living = health > 0 }));
            return new Impact(contact: contact, damage: defense.Health - health);
        }

        RunPopulation.Survivor? target = frame.Population.Survivors.FirstOrDefault(predicate: item =>
                StringComparer.Ordinal.Equals(x: item.Id, y: contact.TargetId));
        if (target is null)
        {
            return null;
        }

        (RunInventory inventory, RunPopulation.Survivor survivor, RunDamage.Hit? receipt) = RunDamage.Apply(
                inventory: frame.Inventory, definitions: rules, survivor: target, sourceId: contact.Arrow.SourceId,
                sourceHostile: contact.Arrow.Hostile, raw: contact.Arrow.Damage, at: contact.At);
        if (receipt is null)
        {
            return null;
        }

        frame.Inventory = inventory;
        frame.Population = frame.Population.Replace(previous: target, current: survivor);
        return new Impact(contact: contact, damage: receipt.Damage, hit: receipt);
    }

    internal enum Kind { Terrain, Defense, Survivor }

    internal sealed record Arrow
    {
        internal Arrow(long id, string sourceId, bool hostile, decimal damage, decimal launchedAt,
                decimal expiresAt, (decimal x, decimal y) position, (decimal x, decimal y) velocity, RunCombat.Aim aim)
        {
            Id = id;
            SourceId = sourceId;
            Hostile = hostile;
            Damage = damage;
            LaunchedAt = launchedAt;
            ExpiresAt = expiresAt;
            Position = position;
            Velocity = velocity;
            Aim = aim;
        }

        internal long Id { get; }
        internal string SourceId { get; }
        internal bool Hostile { get; }
        internal decimal Damage { get; }
        internal decimal LaunchedAt { get; }
        internal decimal ExpiresAt { get; }
        internal (decimal x, decimal y) Position { get; }
        internal (decimal x, decimal y) Velocity { get; }
        internal RunCombat.Aim Aim { get; }

        internal (decimal x, decimal y) At(decimal now)
        {
            decimal elapsed = now - LaunchedAt;
            return (Position.x + elapsed * Velocity.x, Position.y + elapsed * Velocity.y);
        }
    }

    internal sealed class Frame
    {
        internal Frame(RunWorld world, RunPopulation population, RunInventory inventory, RunCombat.State combat)
        {
            World = world;
            Population = population;
            Inventory = inventory;
            Combat = combat;
        }

        internal RunWorld World { get; set; }
        internal RunPopulation Population { get; set; }
        internal RunInventory Inventory { get; set; }
        internal RunCombat.State Combat { get; set; }
    }

    internal sealed record Impact
    {
        internal Impact(Contact contact, decimal damage, RunDamage.Hit? hit = null)
                : this(arrowId: contact.Arrow.Id, targetId: contact.TargetId, kind: contact.Kind, at: contact.At,
                        damage: damage, hit: hit)
        {
        }

        internal Impact(long arrowId, string targetId, Kind kind, decimal at, decimal damage, RunDamage.Hit? hit)
        {
            ArrowId = arrowId;
            TargetId = targetId;
            Kind = kind;
            At = at;
            Damage = damage;
            Hit = hit;
        }

        internal long ArrowId { get; }
        internal string TargetId { get; }
        internal Kind Kind { get; }
        internal decimal At { get; }
        internal decimal Damage { get; }
        internal RunDamage.Hit? Hit { get; }
    }

    internal sealed record Contact
    {
        internal Contact(Arrow arrow, string targetId, Kind kind, decimal at)
        {
            Arrow = arrow;
            TargetId = targetId;
            Kind = kind;
            At = at;
        }

        internal Arrow Arrow { get; }
        internal string TargetId { get; }
        internal Kind Kind { get; }
        internal decimal At { get; }
    }

    private sealed class Sweep
    {
        internal Sweep(Arrow arrow, decimal from, decimal to)
        {
            _arrow = arrow;
            From = from;
            To = to;
            Start = arrow.At(now: from);
            End = arrow.At(now: to);
        }

        internal decimal From { get; }
        internal decimal To { get; }
        internal (decimal x, decimal y) Start { get; }
        internal (decimal x, decimal y) End { get; }

        internal Contact? Against((decimal left, decimal bottom, decimal right, decimal top) bounds,
                string id, Kind kind)
        {
            decimal? offset = RunWorldGeometry.FirstContact(start: Start, end: End, bounds: bounds,
                    duration: To - From);
            return ContactAt(offset: offset, id: id, kind: kind);
        }

        internal Contact? ContactAt(decimal? offset, string id, Kind kind)
        {
            return offset is { } first ? new Contact(arrow: _arrow, targetId: id, kind: kind,
                    at: From + first) : null;
        }

        private readonly Arrow _arrow;
    }
}
