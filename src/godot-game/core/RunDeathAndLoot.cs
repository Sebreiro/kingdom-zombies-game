using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Transfers each victim's actual roots once and removes only continuously empty loot.</summary>
internal static class RunDeathAndLoot
{
    internal static Frame ApplyDeaths(RunWorld world, RunPopulation population, RunInventory inventory, State state,
            ImmutableArray<RunDamage.Hit> hits)
    {
        var frame = new Frame(world: world, population: population, inventory: inventory, state: state);
        foreach (RunDamage.Hit hit in hits)
        {
            if (hit.Lethal && !frame.Loot.Deaths.ContainsKey(key: hit.TargetId))
            {
                ApplyDeath(frame: frame, hit: hit);
            }
        }

        return frame;
    }

    internal static (RunWorld world, State loot) Reconcile(RunWorld world, RunInventory inventory, State loot,
            long now, decimal delay)
    {
        if (loot.Targets.IsEmpty && !world.Anchors.Any(predicate: anchor => IsLoot(kind: anchor.Kind)))
        {
            return (world, loot);
        }

        var targets = loot.Targets.ToBuilder();
        foreach (RunWorld.Anchor anchor in world.Anchors.Where(predicate: anchor => IsLoot(kind: anchor.Kind)))
        {
            if (!targets.ContainsKey(key: anchor.Id))
            {
                targets.Add(key: anchor.Id, value: new Target(anchor: anchor, generated: false));
            }
        }

        ImmutableArray<RunWorld.Anchor> anchors = world.Anchors;
        foreach (Target target in targets.Values.ToArray())
        {
            decimal? since = inventory.HasLoot(id: target.Anchor.Id) ? null : target.EmptySince ?? now;
            if (since is { } empty && now - empty >= delay)
            {
                targets.Remove(key: target.Anchor.Id);
                anchors = anchors.RemoveAll(match: anchor => Same(first: target.Anchor, second: anchor));
                continue;
            }

            targets[target.Anchor.Id] = target with { EmptySince = since };
            if (target.Generated && !anchors.Contains(item: target.Anchor))
            {
                anchors = anchors.RemoveAll(match: anchor => Same(first: target.Anchor, second: anchor))
                        .Add(item: target.Anchor);
            }
        }

        return (anchors == world.Anchors ? world : world.WithAnchors(anchors: anchors),
                new State(targets: targets.ToImmutable(), deaths: loot.Deaths, lastTargetId: loot.LastTargetId));
    }

    internal static DropSite? PreparePile(FoundationSession.RunPublication current, RunPopulation.Survivor owner)
    {
        RunWorld.Point contact = current.World.ActorContact(survivor: owner);
        var ground = new RunWorld.Point(x: contact.X, y: current.World.Origin.Y + current.World.GroundContactSubunits);
        RunWorld.Anchor existing = current.World.Anchors.Where(predicate: anchor => anchor.Kind == RunWorld.Access.Pile
                && anchor.Contact == ground).OrderBy(keySelector: anchor => anchor.Id, comparer: StringComparer.Ordinal)
                .FirstOrDefault();
        if (existing.Kind == RunWorld.Access.Pile)
        {
            return new DropSite(id: existing.Id, world: current.World, loot: current.Loot);
        }

        try
        {
            return Allocate(world: current.World, loot: current.Loot, kind: RunWorld.Access.Pile, point: ground);
        }
        catch (OverflowException)
        {
            return null;
        }
    }

    private static void ApplyDeath(Frame frame, RunDamage.Hit hit)
    {
        RunPopulation.Survivor? victim = frame.Population.Survivors.FirstOrDefault(predicate: survivor =>
                StringComparer.Ordinal.Equals(x: survivor.Id, y: hit.TargetId));
        if (victim is not { Health: 0 })
        {
            throw new InvalidDataException(message: "RUN_DEATH_VICTIM");
        }

        RunWorld.Point point = frame.World.ActorContact(survivor: victim);
        DropSite corpse = Allocate(world: frame.World, loot: frame.Loot, kind: RunWorld.Access.Corpse,
                point: point);
        frame.Inventory = frame.Inventory.MoveToCorpse(survivorId: victim.Id, corpseId: corpse.Id);
        RunPopulation.Lease? lease = frame.Population.Leases.FirstOrDefault(predicate: item =>
                StringComparer.Ordinal.Equals(x: item.SurvivorId, y: victim.Id));
        if (lease is not null)
        {
            frame.Population = frame.Population.Release(bindingId: lease.BindingId);
        }

        frame.World = victim.Hostile ? corpse.World.WithHostiles(hostiles: corpse.World.Hostiles.RemoveAll(
                match: hostile => StringComparer.Ordinal.Equals(x: hostile.Id, y: victim.Id))) : corpse.World;
        var death = new Death(survivorId: victim.Id, corpseId: corpse.Id, contact: point, at: hit.At, lease: lease);
        Target target = corpse.Loot.Targets[corpse.Id];
        if (!frame.Inventory.HasLoot(id: corpse.Id))
        {
            target = target with { EmptySince = hit.At };
        }

        frame.Loot = new State(targets: corpse.Loot.Targets.SetItem(key: corpse.Id, value: target),
                deaths: corpse.Loot.Deaths.Add(key: victim.Id, value: death), lastTargetId: corpse.Loot.LastTargetId);
    }

    private static DropSite Allocate(RunWorld world, State loot, RunWorld.Access kind, RunWorld.Point point)
    {
        long next = loot.LastTargetId;
        string prefix = kind == RunWorld.Access.Pile ? "pile" : "corpse";
        string id;
        do
        {
            next = checked(next + 1);
            id = FormattableString.Invariant(formattable: $"run.{prefix}.{next}");
        }
        while (world.OccupiesId(id: id) || loot.Targets.ContainsKey(key: id));

        var anchor = new RunWorld.Anchor(id: id, contact: point, kind: kind);
        var target = new Target(anchor: anchor, generated: true);
        var state = new State(targets: loot.Targets.Add(key: id, value: target),
                deaths: loot.Deaths, lastTargetId: next);
        return new DropSite(id: id, world: world.WithAnchors(anchors: world.Anchors.Add(item: anchor)), loot: state);
    }

    private static bool IsLoot(RunWorld.Access kind)
    {
        return kind is RunWorld.Access.Corpse or RunWorld.Access.Pile;
    }

    private static bool Same(RunWorld.Anchor first, RunWorld.Anchor second)
    {
        return first.Kind == second.Kind && StringComparer.Ordinal.Equals(x: first.Id, y: second.Id);
    }

    internal sealed class Frame
    {
        internal Frame(RunWorld world, RunPopulation population, RunInventory inventory, State state)
        {
            World = world;
            Population = population;
            Inventory = inventory;
            Loot = state;
        }

        internal RunWorld World { get; set; }
        internal RunPopulation Population { get; set; }
        internal RunInventory Inventory { get; set; }
        internal State Loot { get; set; }
    }

    internal sealed class State
    {
        internal State(ImmutableDictionary<string, Target> targets, ImmutableDictionary<string, Death> deaths,
                long lastTargetId)
        {
            Targets = targets;
            Deaths = deaths;
            LastTargetId = lastTargetId;
        }

        internal static State Empty { get; } = new(
                targets: ImmutableDictionary.Create<string, Target>(keyComparer: StringComparer.Ordinal),
                deaths: ImmutableDictionary.Create<string, Death>(keyComparer: StringComparer.Ordinal),
                lastTargetId: 0);
        internal ImmutableDictionary<string, Target> Targets { get; }
        internal ImmutableDictionary<string, Death> Deaths { get; }
        internal long LastTargetId { get; }
    }

    internal sealed record Target
    {
        internal Target(RunWorld.Anchor anchor, bool generated)
        {
            Anchor = anchor;
            Generated = generated;
        }

        internal RunWorld.Anchor Anchor { get; }
        internal bool Generated { get; }
        internal decimal? EmptySince { get; init; }
    }

    internal sealed record Death
    {
        internal Death(string survivorId, string corpseId, RunWorld.Point contact, decimal at,
                RunPopulation.Lease? lease)
        {
            SurvivorId = survivorId;
            CorpseId = corpseId;
            Contact = contact;
            At = at;
            Lease = lease;
        }

        internal string SurvivorId { get; }
        internal string CorpseId { get; }
        internal RunWorld.Point Contact { get; }
        internal decimal At { get; }
        internal RunPopulation.Lease? Lease { get; }
    }

    internal sealed record DropSite
    {
        internal DropSite(string id, RunWorld world, State loot)
        {
            Id = id;
            World = world;
            Loot = loot;
        }

        internal string Id { get; }
        internal RunWorld World { get; }
        internal State Loot { get; }
    }
}
