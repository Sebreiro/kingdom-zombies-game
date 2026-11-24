using System;
using System.Linq;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the current authority for nearby inventory access and item actions.</summary>
[Trait("Tier", "1")]
public sealed class Phase06AccessTests
{
    /// <summary>Nearby results include exact 3 u and 4 u boundaries and distinct equal-looking targets.</summary>
    [Fact]
    public void Phase06_E064_ListUsesTheInclusiveWorldRanges()
    {
        RunWorld.Anchor[] anchors =
        [
            Anchor(id: RunWorld.StashId, kind: RunWorld.Access.Stash, x: -4000),
            Anchor(id: "pile", kind: RunWorld.Access.Pile, x: -5000),
            Anchor(id: "corpse", kind: RunWorld.Access.Corpse, x: -5000),
            Anchor(id: "outside", kind: RunWorld.Access.Pile, x: -4999),
        ];
        FoundationSession session = Start(anchors: anchors);
        RunItemActions.Controller controller = Controller(session: session);
        RunItemActions.View view = session.ItemActions.Read(controller: controller).ShouldNotBeNull();
        view.Nearby.Any(predicate: target => StringComparer.Ordinal.Equals(x: target.Id, y: RunWorld.StashId))
                .ShouldBeTrue();
        view.Nearby.Any(predicate: target => target.Id is "pile").ShouldBeTrue();
        view.Nearby.Any(predicate: target => target.Id is "corpse").ShouldBeTrue();
        view.Nearby.Any(predicate: target => target.Id is "outside").ShouldBeFalse();
        view.Nearby.Count(predicate: target => target.Kind == RunWorld.Access.Npc).ShouldBe(expected: 7);
        RunItemActions.View selected = Select(session: session, controller: controller, sequence: 1,
                kind: RunWorld.Access.Stash, id: RunWorld.StashId);
        selected.External.ShouldNotBeEmpty();
        selected.External.All(predicate: item => ((RunInventory.StashCell)item.Location).Page == 0).ShouldBeTrue();
    }

    /// <summary>A new controller revokes an NPC grant and hides its inventory from all losing actions.</summary>
    [Fact]
    public void Phase06_E065_TakeoverRevokesANearbyInventory()
    {
        FoundationSession session = Start();
        RunItemActions.Controller local = Controller(session: session);
        RunItemActions.View before = Select(session: session, controller: local, sequence: 1,
                kind: RunWorld.Access.Npc, id: "survivor-02");
        RunInventory.Item boots = before.External.Single(predicate: item => item.DefinitionId is "IT-17");
        RunInventory.ItemPath source = Path(session: session, item: boots);
        ReserveGuest(session: session);
        FoundationSnapshot snapshot = session.Snapshot;
        var request = new RunItemActions.Place(expected: source, destination:
                new RunInventory.Equipment(survivorId: "survivor-01", slot: "boots"), token: 1);
        RunItemActions.Outcome lost = session.ItemActions.Submit(controller: local, sequence: 2, request: request);
        lost.Status.ShouldBe(expected: RunItemActions.Status.AccessLost);
        RunItemActions.View corrected = lost.View.ShouldNotBeNull();
        corrected.Selection.ShouldBeNull();
        corrected.External.ShouldBeEmpty();
        corrected.Availability.ShouldBe(expected: RunItemActions.Status.AccessLost);
        corrected.Nearby.Any(predicate: target => target.Id is "survivor-02").ShouldBeFalse();
        corrected.Nearby.Any(predicate: target => target.Id is "survivor-03").ShouldBeTrue();
        corrected.Personal.Any(predicate: item => item.Id == boots.Id).ShouldBeFalse();
        var guessedDestination = new RunItemActions.Place(expected: Path(session: session,
                item: corrected.Personal[0]), destination: new RunInventory.Equipment(
                        survivorId: "survivor-02", slot: "torso-clothing"));
        session.ItemActions.Submit(controller: local, sequence: 3, request: guessedDestination)
                .Status.ShouldBe(expected: RunItemActions.Status.AccessLost);
        session.MarkReady(bindingId: "guest").ShouldBeTrue();
        RunItemActions.View guest = session.ItemActions.Read(controller: Controller(session: session,
                binding: "guest")).ShouldNotBeNull();
        guest.Personal.Any(predicate: item => item.Id == boots.Id).ShouldBeTrue();
        (session.Snapshot.Revision - snapshot.Revision).ShouldBe(expected: 1);
    }

    /// <summary>Zero permitted targets has an explicit result and cannot create inventory or a remote scope.</summary>
    [Fact]
    public void Phase06_E066_NoReachableTargetsPreservesThePublication()
    {
        var barrier = new RunWorld.Blocker(id: "barrier", minimum: new RunWorld.Point(x: -8000, y: -1),
                maximum: new RunWorld.Point(x: -8000, y: 1));
        FoundationSession session = Start(anchors: [Anchor(id: RunWorld.StashId, kind: RunWorld.Access.Stash,
                x: -4000)], blockers: [barrier]);
        RunItemActions.Controller controller = Controller(session: session);
        RunItemActions.View view = session.ItemActions.Read(controller: controller).ShouldNotBeNull();
        view.Nearby.ShouldBeEmpty();
        view.Availability.ShouldBe(expected: RunItemActions.Status.NoTargets);
        FoundationSnapshot snapshot = session.Snapshot;
        session.ItemActions.Submit(controller: controller, sequence: 1, request: new RunItemActions.Select(target:
                new RunExternalAccess.Target(kind: RunWorld.Access.Stash, id: RunWorld.StashId))).Status
                .ShouldBe(expected: RunItemActions.Status.AccessLost);
        session.Snapshot.ShouldBeSameAs(expected: snapshot);
    }

    /// <summary>Closing and replay preserve the item. Only an explicit, reachable drop can move it to a pile.</summary>
    [Fact]
    public void Phase06_E067_CancellingAndImplicitGroundPlacementNeverDropAnItem()
    {
        FoundationSession session = Start(anchors: [Anchor(id: "pile", kind: RunWorld.Access.Pile, x: -7500)]);
        RunItemActions.Controller controller = Controller(session: session);
        Select(session: session, controller: controller, sequence: 1, kind: RunWorld.Access.Pile, id: "pile");
        var close = new RunItemActions.Close(token: 1);
        FoundationSnapshot before = session.Snapshot;
        session.ItemActions.Submit(controller: controller, sequence: 2, request: close).Status
                .ShouldBe(expected: RunItemActions.Status.Ready);
        session.ItemActions.Submit(controller: controller, sequence: 2, request: close).Replayed.ShouldBeTrue();
        RunInventory.ItemPath source = Path(session: session, item: session.Inventory.Items[0]);
        var implicitDrop = new RunItemActions.Place(expected: source,
                destination: new RunInventory.Ground(pileId: "pile"));
        session.ItemActions.Submit(controller: controller, sequence: 3, request: implicitDrop).Status
                .ShouldBe(expected: RunItemActions.Status.Incompatible);
        var fabricated = new RunItemActions.Drop(expected: source, pileId: "invented");
        session.ItemActions.Submit(controller: controller, sequence: 4, request: fabricated).Status
                .ShouldBe(expected: RunItemActions.Status.AccessLost);
        session.Snapshot.ShouldBeSameAs(expected: before);
        var explicitDrop = new RunItemActions.Drop(expected: source, pileId: "pile");
        session.ItemActions.Submit(controller: controller, sequence: 5, request: explicitDrop).Status
                .ShouldBe(expected: RunItemActions.Status.Applied);
        session.Inventory.Find(itemId: source.Item.Id)?.Location.ShouldBe(expected:
                new RunInventory.Ground(pileId: "pile"));
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 0);
    }

    /// <summary>Overlapping piles use an explicit selection, and earlier scopes cannot consume its items.</summary>
    [Fact]
    public void Phase06_E068_OnlyTheLatestSelectedOverlappingTargetIsVisible()
    {
        FoundationSession session = Start(anchors:
                [Anchor(id: "b", kind: RunWorld.Access.Pile, x: -7500),
                    Anchor(id: "a", kind: RunWorld.Access.Pile, x: -7500)]);
        RunInventory.ItemPath first = GroundMoney(session: session, pile: "a");
        RunInventory.ItemPath second = GroundMoney(session: session, pile: "b");
        RunItemActions.Controller controller = Controller(session: session);
        RunItemActions.View a = Select(session: session, controller: controller, sequence: 1,
                kind: RunWorld.Access.Pile, id: "a");
        a.Nearby.Where(predicate: target => target.Kind == RunWorld.Access.Pile)
                .Select(selector: target => target.Id).ShouldBe(expected: ["a", "b"]);
        a.External.Single().Id.ShouldBe(expected: first.Item.Id);
        RunItemActions.View b = Select(session: session, controller: controller, sequence: 2,
                kind: RunWorld.Access.Pile, id: "b");
        b.External.Single().Id.ShouldBe(expected: second.Item.Id);
        RunItemActions.Outcome stale = session.ItemActions.Submit(controller: controller, sequence: 3,
                request: PickUp(session: session, source: first, owner: "survivor-01", token: 1));
        stale.Status.ShouldBe(expected: RunItemActions.Status.AccessLost);
        stale.View.ShouldNotBeNull().External.Single().Id.ShouldBe(expected: second.Item.Id);
        RunWorld removed = World(anchors: [Anchor(id: "a", kind: RunWorld.Access.Pile, x: -7500)]);
        session.TryReplaceRunWorld(candidate: removed).ShouldBeTrue();
        session.ItemActions.Submit(controller: controller, sequence: 4,
                request: PickUp(session: session, source: second, owner: "survivor-01", token: 2))
                .View.ShouldNotBeNull().Selection.ShouldBeNull();
        session.Inventory.Find(itemId: second.Item.Id).ShouldBeSameAs(expected: second.Item);
    }

    /// <summary>Exact subunit reach and integral sequences reject adjacent invalid numbers.</summary>
    [Fact]
    public void Phase06_E069_ReachAndSequenceDoNotRoundOrWrap()
    {
        FoundationSession session = Start(anchors: [Anchor(id: "edge", kind: RunWorld.Access.Pile, x: -6200,
                y: 2400), Anchor(id: "outside", kind: RunWorld.Access.Pile, x: -6200, y: 2401)]);
        RunItemActions.Controller controller = Controller(session: session);
        var select = new RunItemActions.Select(target: new RunExternalAccess.Target(kind: RunWorld.Access.Pile,
                id: "edge"));
        var forged = new RunItemActions.Controller(sessionId: controller.SessionId,
                bindingId: controller.BindingId, generation: controller.Generation + 1);
        session.ItemActions.Submit(controller: forged, sequence: 1, request: select).View.ShouldBeNull();
        session.ItemActions.Submit(controller: controller, sequence: long.MinValue, request: select).Status
                .ShouldBe(expected: RunItemActions.Status.StaleRequest);
        session.ItemActions.Submit(controller: controller, sequence: long.MaxValue, request: select).Status
                .ShouldBe(expected: RunItemActions.Status.Ready);
        session.ItemActions.Submit(controller: controller, sequence: long.MaxValue - 1, request: select).Status
                .ShouldBe(expected: RunItemActions.Status.StaleRequest);
        session.ItemActions.Read(controller: controller).ShouldNotBeNull().Nearby
                .Any(predicate: target => target.Id is "outside").ShouldBeFalse();
    }

    /// <summary>Two valid controllers contest one real item and see only their permitted correction.</summary>
    [Fact]
    public async Task Phase06_E078_CompetingControllersCommitTheItemExactlyOnce()
    {
        FoundationSession session = Start(anchors: [Anchor(id: "pile", kind: RunWorld.Access.Pile, x: -8000)]);
        RunInventory.ItemPath source = GroundMoney(session: session, pile: "pile");
        ReserveGuest(session: session);
        session.MarkReady(bindingId: "guest").ShouldBeTrue();
        RunItemActions.Controller local = Controller(session: session);
        RunItemActions.Controller guest = Controller(session: session, binding: "guest");
        Select(session: session, controller: local, sequence: 1, kind: RunWorld.Access.Pile, id: "pile");
        Select(session: session, controller: guest, sequence: 1, kind: RunWorld.Access.Pile, id: "pile");
        RunItemActions.Place localRequest = PickUp(session: session, source: source, owner: "survivor-01", token: 1);
        RunItemActions.Place guestRequest = PickUp(session: session, source: source, owner: "survivor-02", token: 1);
        long revision = session.Snapshot.Revision;
        Task<RunItemActions.Outcome> hostResult = Task.Run(function: () => session.ItemActions.Submit(
                controller: local, sequence: 2, request: localRequest));
        Task<RunItemActions.Outcome> guestResult = Task.Run(function: () => session.ItemActions.Submit(
                controller: guest, sequence: 2, request: guestRequest));
        RunItemActions.Outcome[] results = await Task.WhenAll(tasks: [hostResult, guestResult]);
        results.Count(predicate: result => result.Status == RunItemActions.Status.Applied).ShouldBe(expected: 1);
        RunItemActions.View loser = results.Single(predicate: result => result.Status
                == RunItemActions.Status.StaleItem).View.ShouldNotBeNull();
        loser.External.ShouldBeEmpty();
        loser.Personal.Any(predicate: item => item.Id == source.Item.Id).ShouldBeFalse();
        loser.Revision.ShouldBe(expected: revision + 1);
        session.Inventory.Items.Single(predicate: item => item.Id == source.Item.Id).Quantity
                .ShouldBe(expected: source.Item.Quantity);
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
    }

    /// <summary>A recorded success or rejection cannot be replaced by changing the payload at that sequence.</summary>
    [Fact]
    public void Phase06_E079_ReplayedAndRejectedSequencesCannotCommitAnotherItem()
    {
        FoundationSession session = Start(anchors: [Anchor(id: "pile", kind: RunWorld.Access.Pile, x: -8000)]);
        RunItemActions.Controller controller = Controller(session: session);
        RunInventory.ItemPath first = Path(session: session, item: session.Inventory.Items[0]);
        RunInventory.ItemPath second = Path(session: session, item: session.Inventory.Items[1]);
        var bad = new RunItemActions.Drop(expected: first, pileId: "missing");
        var good = new RunItemActions.Drop(expected: first, pileId: "pile");
        session.ItemActions.Submit(controller: controller, sequence: 1, request: bad).Status
                .ShouldBe(expected: RunItemActions.Status.AccessLost);
        session.ItemActions.Submit(controller: controller, sequence: 1, request: good).Status
                .ShouldBe(expected: RunItemActions.Status.AccessLost);
        session.ItemActions.Submit(controller: controller, sequence: 2, request: good).Status
                .ShouldBe(expected: RunItemActions.Status.Applied);
        FoundationSnapshot committed = session.Snapshot;
        var changed = new RunItemActions.Drop(expected: second, pileId: "pile");
        RunItemActions.Outcome replay = session.ItemActions.Submit(controller: controller, sequence: 2,
                request: changed);
        replay.Replayed.ShouldBeTrue();
        replay.Status.ShouldBe(expected: RunItemActions.Status.Applied);
        session.Snapshot.ShouldBeSameAs(expected: committed);
        session.Inventory.Find(itemId: second.Item.Id).ShouldBeSameAs(expected: second.Item);
    }

    private static FoundationSession Start(RunWorld.Anchor[]? anchors = null, RunWorld.Blocker[]? blockers = null)
    {
        return new FoundationSession(sessionId: "session", playerId: "host", actorId: "actor",
                world: World(anchors: anchors ?? [], blockers: blockers));
    }

    private static RunWorld World(RunWorld.Anchor[] anchors, RunWorld.Blocker[]? blockers = null)
    {
        return new RunWorld(origin: default, groundContactSubunits: 0, mainBuilding: default,
                ranges: RunWorld.DefaultRanges, defenses: [], blockers: blockers ?? [], hostiles: [], anchors: anchors);
    }

    private static RunWorld.Anchor Anchor(string id, RunWorld.Access kind, long x, long y = 0)
    {
        return new RunWorld.Anchor(id: id, contact: new RunWorld.Point(x: x, y: y), kind: kind);
    }

    private static RunItemActions.Controller Controller(FoundationSession session, string binding = "local")
    {
        RunPopulation.Lease lease = session.Population.FindLease(bindingId: binding).ShouldNotBeNull();
        return new RunItemActions.Controller(sessionId: lease.SessionId, bindingId: binding,
                generation: lease.Generation);
    }

    private static RunItemActions.View Select(FoundationSession session, RunItemActions.Controller controller,
            long sequence, RunWorld.Access kind, string id)
    {
        var request = new RunItemActions.Select(target: new RunExternalAccess.Target(kind: kind, id: id));
        RunItemActions.Outcome result = session.ItemActions.Submit(controller: controller, sequence: sequence,
                request: request);
        result.Status.ShouldBe(expected: RunItemActions.Status.Ready);
        return result.View.ShouldNotBeNull();
    }

    private static RunInventory.ItemPath Path(FoundationSession session, RunInventory.Item item)
    {
        return session.Inventory.PathTo(itemId: item.Id).ShouldNotBeNull();
    }

    private static RunInventory.ItemPath GroundMoney(FoundationSession session, string pile)
    {
        RunInventory.Item item = session.Inventory.Items.First(predicate: candidate => candidate.DefinitionId
                is "IT-01" && candidate.Location is RunInventory.StashCell);
        RunInventory.ItemPath source = Path(session: session, item: item);
        session.TryEditInventory(edit: (inventory, _, _) => RunItemTransfer.Drop(inventory: inventory,
                expected: source, pileId: pile)).ShouldBeTrue();
        return session.Inventory.PathTo(itemId: item.Id).ShouldNotBeNull();
    }

    private static RunItemActions.Place PickUp(FoundationSession session, RunInventory.ItemPath source, string owner,
            long token)
    {
        RunInventory.Item shirt = session.Inventory.Equipped(survivorId: owner, slot: "torso-clothing")
                .ShouldNotBeNull();
        return new RunItemActions.Place(expected: source, destination: RunItemTransfer.Container(
                definition: session.Definitions.Items[source.Item.DefinitionId], containerId: shirt.Id, x: 0, y: 0),
                token: token, container: Path(session: session, item: shirt));
    }

    private static void ReserveGuest(FoundationSession session)
    {
        session.AddParticipant(bindingId: "guest", playerId: "guest-player", actorId: "guest-actor", label: "Guest")
                .ShouldBeTrue();
    }
}
