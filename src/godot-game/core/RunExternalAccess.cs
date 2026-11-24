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
        RunWorld.Point actor = Contact(current: current, participant: participant);
        IEnumerable<Target> anchored = current.World.Anchors.Where(predicate: anchor =>
                ReachAnchor(current: current, actor: actor, anchor: anchor)).Select(selector: anchor =>
                        new Target(kind: anchor.Kind, id: anchor.Id));
        IEnumerable<Target> recruits = current.Population.Survivors.Where(predicate: survivor =>
                ReachRecruit(current: current, actor: actor, id: survivor.Id)).Select(selector: survivor =>
                        new Target(kind: RunWorld.Access.Npc, id: survivor.Id));
        return [.. anchored.Concat(second: recruits)
                .OrderBy(keySelector: target => target.Kind)
                .ThenBy(keySelector: target => target.Id, comparer: StringComparer.Ordinal)];
    }

    internal static bool Allows(FoundationSession.RunPublication current, FoundationParticipantState participant,
            Target target)
    {
        RunWorld world = current.World;
        RunWorld.Point actor = Contact(current: current, participant: participant);
        if (target.Kind == RunWorld.Access.Npc)
        {
            return ReachRecruit(current: current, actor: actor, id: target.Id);
        }

        return world.Anchors.Any(predicate: anchor => anchor.Kind == target.Kind
                && StringComparer.Ordinal.Equals(x: anchor.Id, y: target.Id)
                && ReachAnchor(current: current, actor: actor, anchor: anchor));
    }

    internal static bool Contains(Selection selection, RunInventory.Location location)
    {
        return location switch
        {
            RunInventory.Equipment equipment => selection.Target.Kind is RunWorld.Access.Npc or RunWorld.Access.Corpse
                    && StringComparer.Ordinal.Equals(x: equipment.SurvivorId, y: selection.Target.Id),
            RunInventory.CorpseEquipment corpse => selection.Target.Kind == RunWorld.Access.Corpse
                    && StringComparer.Ordinal.Equals(x: corpse.CorpseId, y: selection.Target.Id),
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

    private static RunWorld.Point Contact(FoundationSession.RunPublication current,
            FoundationParticipantState participant)
    {
        return current.Population.FindLivingControl(bindingId: participant.BindingId) is { } lease
                ? current.World.ActorContact(survivor: current.Population.ForLease(lease: lease))
                : current.World.ActorContact(actor: participant.Actor);
    }

    private static bool ReachAnchor(FoundationSession.RunPublication current, RunWorld.Point actor,
            RunWorld.Anchor anchor)
    {
        return anchor.Kind switch
        {
            RunWorld.Access.Stash => StringComparer.Ordinal.Equals(x: anchor.Id, y: RunWorld.StashId)
                    && current.World.Reachable(actor: actor, kind: anchor.Kind, contact: anchor.Contact),
            RunWorld.Access.Corpse => !current.Population.Survivors.Any(predicate: survivor => survivor.Health > 0
                    && StringComparer.Ordinal.Equals(x: survivor.Id, y: anchor.Id))
                    && ReachKnownLoot(current: current, actor: actor, anchor: anchor),
            RunWorld.Access.Pile => ReachKnownLoot(current: current, actor: actor, anchor: anchor),
            _ => false,
        };
    }

    private static bool ReachRecruit(FoundationSession.RunPublication current, RunWorld.Point actor, string id)
    {
        RunPopulation population = current.Population;
        RunPopulation.Survivor? survivor = population.Survivors.FirstOrDefault(predicate: item =>
                StringComparer.Ordinal.Equals(x: item.Id, y: id));
        if (survivor is null || survivor.Hostile || survivor.Health <= 0 || population.Leases.Any(predicate: lease =>
                StringComparer.Ordinal.Equals(x: lease.SurvivorId, y: id)))
        {
            return false;
        }

        RunWorld world = current.World;
        RunWorld.Point contact = world.ActorContact(survivor: survivor);
        return world.Reachable(actor: actor, kind: RunWorld.Access.Npc, contact: contact);
    }

    private static bool ReachKnownLoot(FoundationSession.RunPublication current, RunWorld.Point actor,
            RunWorld.Anchor anchor)
    {
        return current.Loot.Targets.TryGetValue(key: anchor.Id, value: out RunDeathAndLoot.Target? target)
                && target.Anchor.Kind == anchor.Kind
                && current.World.Reachable(actor: actor, kind: anchor.Kind, contact: anchor.Contact);
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
