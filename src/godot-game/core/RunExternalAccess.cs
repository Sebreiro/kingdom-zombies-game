using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;

namespace KingdomZombies.Core;

/// <summary>Resolves local inventory permission from the current authority publication.</summary>
internal static class RunExternalAccess
{
    internal static FoundationParticipantState? Controller(FoundationSession.RunPublication current,
            RunPopulation.Lease lease)
    {
        return current.Snapshot.Participants.FirstOrDefault(predicate: participant => participant.Ready
                && StringComparer.Ordinal.Equals(x: participant.BindingId, y: lease.BindingId)
                && StringComparer.Ordinal.Equals(x: participant.Actor.PlayerId, y: lease.PlayerId));
    }

    internal static ImmutableArray<Target> Nearby(FoundationSession.RunPublication current,
            FoundationParticipantState participant)
    {
        IEnumerable<Target> anchored = current.World.Anchors.Where(predicate: anchor => anchor.Kind
                != RunWorld.Access.Npc).Select(selector: anchor => new Target(kind: anchor.Kind, id: anchor.Id));
        IEnumerable<Target> recruits = current.Population.Survivors.Select(selector: survivor =>
                new Target(kind: RunWorld.Access.Npc, id: survivor.Id));
        return [.. anchored.Concat(second: recruits)
                .Where(predicate: target => Allows(current: current, participant: participant, target: target))
                .OrderBy(keySelector: target => target.Kind)
                .ThenBy(keySelector: target => target.Id, comparer: StringComparer.Ordinal)];
    }

    internal static bool Allows(FoundationSession.RunPublication current, FoundationParticipantState participant,
            Target target)
    {
        RunWorld world = current.World;
        RunWorld.Point actor = current.Population.FindLivingControl(bindingId: participant.BindingId) is { } lease
                ? world.ActorContact(survivor: current.Population.ForLease(lease: lease))
                : world.ActorContact(actor: participant.Actor);
        if (target.Kind == RunWorld.Access.Npc)
        {
            return ReachRecruit(current: current, actor: actor, id: target.Id);
        }

        return target.Kind switch
        {
            RunWorld.Access.Stash => StringComparer.Ordinal.Equals(x: target.Id, y: RunWorld.StashId)
                    && world.Reachable(actor: actor, kind: target.Kind, id: target.Id),
            RunWorld.Access.Corpse => !current.Population.Survivors.Any(predicate: survivor =>
                    StringComparer.Ordinal.Equals(x: survivor.Id, y: target.Id) && survivor.Health > 0)
                    && world.Reachable(actor: actor, kind: target.Kind, id: target.Id),
            RunWorld.Access.Pile => world.Reachable(actor: actor, kind: target.Kind, id: target.Id),
            _ => false,
        };
    }

    internal static bool Contains(Selection selection, RunInventory.Location location)
    {
        return location switch
        {
            RunInventory.Equipment equipment => selection.Target.Kind is RunWorld.Access.Npc or RunWorld.Access.Corpse
                    && StringComparer.Ordinal.Equals(x: equipment.SurvivorId, y: selection.Target.Id),
            RunInventory.StashCell cell => selection.Target.Kind == RunWorld.Access.Stash
                    && cell.Page == selection.PageIndex,
            RunInventory.Ground ground => selection.Target.Kind is RunWorld.Access.Pile or RunWorld.Access.Corpse
                    && StringComparer.Ordinal.Equals(x: ground.PileId, y: selection.Target.Id),
            _ => false,
        };
    }

    internal static ImmutableArray<RunInventory.Item> Contents(RunInventory inventory, Selection selection)
    {
        ILookup<long, RunInventory.Item> children = inventory.Items
                .Where(predicate: item => item.Location is RunInventory.ContainerCell)
                .ToLookup(keySelector: item => ((RunInventory.ContainerCell)item.Location).ContainerId);
        var visible = new List<RunInventory.Item>(collection: inventory.Items.Where(predicate: item =>
                Contains(selection: selection, location: item.Location)));
        for (int index = 0; index < visible.Count; index++)
        {
            visible.AddRange(collection: children[visible[index].Id]);
        }

        return [.. visible.OrderBy(keySelector: item => item.Id)];
    }

    private static bool ReachRecruit(FoundationSession.RunPublication current, RunWorld.Point actor, string id)
    {
        RunPopulation population = current.Population;
        RunPopulation.Survivor? survivor = population.Survivors.FirstOrDefault(predicate: item =>
                StringComparer.Ordinal.Equals(x: item.Id, y: id));
        if (survivor is null || survivor.Health <= 0 || population.Leases.Any(predicate: lease =>
                StringComparer.Ordinal.Equals(x: lease.SurvivorId, y: id)))
        {
            return false;
        }

        RunWorld world = current.World;
        RunWorld.Point contact = world.ActorContact(survivor: survivor);
        return world.Reachable(actor: actor, kind: RunWorld.Access.Npc, contact: contact);
    }

    [StructLayout(layoutKind: LayoutKind.Auto)]
    internal readonly record struct Target
    {
        internal Target(RunWorld.Access kind, string id)
        {
            Kind = kind;
            Id = id;
        }

        internal RunWorld.Access Kind { get; }
        internal string Id { get; }
    }

    internal sealed record Selection
    {
        internal Selection(Target target, long token, int pageIndex = 0)
        {
            Target = target;
            Token = token;
            PageIndex = pageIndex;
        }

        internal Target Target { get; }
        internal long Token { get; }
        internal int PageIndex { get; }
    }
}
