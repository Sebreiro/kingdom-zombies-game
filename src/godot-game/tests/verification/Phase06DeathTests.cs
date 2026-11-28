using System.Linq;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the host's death ownership and exact empty-loot lifetime.</summary>
[Trait("Tier", "1")]
public sealed class Phase06DeathTests
{
    /// <summary>A real lethal attack leaves the former survivor with no carried roots.</summary>
    [Fact]
    public void Phase06_E104_AdjacentActualBelongingsMoveToOneCorpse()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunItemActions.Controller guest = Join(session: session);
        RunInventory.Item bow = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "left-hand");
        RunInventory.Item ammo = Phase06MeleeTests.Equip(session: session, id: "IT-02", slot: "right-hand",
                quantity: 3);
        RunInventory.Item bag = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack");
        RunInventory.Item pouch = Store(session: session, id: "IT-22", container: bag, quantity: 1, x: 0);
        RunInventory.Item money = Store(session: session, id: "IT-01", container: pouch, quantity: 47, x: 0);
        RunInventory.Item berries = Store(session: session, id: "IT-08", container: pouch, quantity: 4, x: 1);
        session.TryEditInventory(edit: (inventory, definitions, _) => RunQuickReferences.Assign(inventory: inventory,
                definitions: definitions, survivorId: Phase06MeleeTests.Owner, slot: 1, itemId: berries.Id))
                .ShouldBeTrue();
        session.Attack(controller: Phase06MeleeTests.Control(), sequence: 1,
                aim: new RunCombat.Aim(x: 10000, y: 0)).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        Phase06MeleeTests.Enemy(session: session, id: "enemy", x: -7000);
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 5, stamina: 100).ShouldBeTrue();
        long[] union = [.. session.Inventory.Items.Select(selector: item => item.Id)];
        long[] owned = [.. session.Inventory.CarriedBy(survivorId: Phase06MeleeTests.Owner)
                .Select(selector: item => item.Id).Order()];
        long revision = session.Snapshot.Revision;
        RunDamage.Hit hit = session.AttackUncontrolled(survivorId: "enemy", sequence: 1).Hits.Single();
        hit.Lethal.ShouldBeTrue();
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
        session.Inventory.CarriedBy(survivorId: Phase06MeleeTests.Owner).ShouldBeEmpty();
        session.Population.FindLease(bindingId: "local").ShouldBeNull();
        session.ItemActions.Read(controller: Phase06MeleeTests.Control()).ShouldBeNull();
        session.Inventory.QuickReferences.ShouldBeEmpty();
        RunDeathAndLoot.Death death = Loot(session: session).Deaths[Phase06MeleeTests.Owner];
        RunItemActions.View corpse = Open(session: session, controller: guest, id: death.CorpseId);
        corpse.External.Select(selector: item => item.Id).ShouldBe(expected: owned);
        session.Inventory.Items.Select(selector: item => item.Id).ShouldBe(expected: union);
        session.Inventory.Find(itemId: bow.Id)?.Condition.ShouldBe(expected: 159);
        session.Inventory.Find(itemId: ammo.Id)?.Quantity.ShouldBe(expected: 2);
        session.Inventory.Find(itemId: money.Id).ShouldBeSameAs(expected: money);
        session.Inventory.Find(itemId: berries.Id).ShouldBeSameAs(expected: berries);
        session.Inventory.PathTo(itemId: money.Id)?.Root.ShouldBe(expected:
                new RunInventory.CorpseEquipment(corpseId: death.CorpseId, slot: "backpack"));
    }

    /// <summary>Empty corpses expire at the deadline, while a broken belonging prevents expiry.</summary>
    [Fact]
    public void Phase06_E105_EmptyAndNonemptyCorpsesHaveDifferentLifetimes()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        Phase06MeleeTests.Enemy(session: session, id: "empty", x: -7500, health: 1);
        Phase06MeleeTests.Enemy(session: session, id: "full", x: -7400, health: 1);
        RunInventory.Item broken = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand",
                owner: "full", condition: 0);
        Kill(session: session, victim: "empty");
        session.Step(elapsedMilliseconds: 700);
        Kill(session: session, victim: "full", sequence: 2);
        string empty = Loot(session: session).Deaths["empty"].CorpseId;
        string full = Loot(session: session).Deaths["full"].CorpseId;
        session.Step(elapsedMilliseconds: 179299);
        Loot(session: session).Targets[empty].EmptySince.ShouldBe(expected: 0);
        Loot(session: session).Targets[full].EmptySince.ShouldBeNull();
        session.Step(elapsedMilliseconds: 1);
        Loot(session: session).Targets.ContainsKey(key: empty).ShouldBeFalse();
        session.Step(elapsedMilliseconds: 360000);
        RunItemActions.View contents = Open(session: session, controller: Phase06MeleeTests.Control(), id: full);
        contents.External.Single().Id.ShouldBe(expected: broken.Id);
        contents.External.Single().Condition.ShouldBe(expected: 0);
    }

    /// <summary>Two controllers contest the last hostile belonging and only one receives it.</summary>
    [Fact]
    public async Task Phase06_E106_LastCorpseTransferCommitsOnce()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunItemActions.Controller guest = Join(session: session);
        RunItemActions.Controller host = Phase06MeleeTests.Control();
        Phase06MeleeTests.Enemy(session: session, id: "enemy", x: -7500, health: 1);
        RunInventory.Item item = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand",
                owner: "enemy", condition: 12.5m);
        Kill(session: session, victim: "enemy");
        string corpse = Loot(session: session).Deaths["enemy"].CorpseId;
        _ = Open(session: session, controller: host, id: corpse);
        _ = Open(session: session, controller: guest, id: corpse);
        RunInventory.ItemPath source = Path(session: session, itemId: item.Id);
        RunItemActions.Place first = HandPlacement(source: source, owner: Phase06MeleeTests.Owner);
        RunItemActions.Place second = HandPlacement(source: source, owner: "survivor-02");
        Task<RunItemActions.Outcome> left = Task.Run(function: () => session.ItemActions.Submit(controller: host,
                sequence: 2, request: first), cancellationToken: TestContext.Current.CancellationToken);
        Task<RunItemActions.Outcome> right = Task.Run(function: () => session.ItemActions.Submit(controller: guest,
                sequence: 2, request: second), cancellationToken: TestContext.Current.CancellationToken);
        RunItemActions.Outcome[] results = await Task.WhenAll(tasks: [left, right]);
        results.Count(predicate: result => result.Status == RunItemActions.Status.Applied).ShouldBe(expected: 1);
        results.Single(predicate: result => result.Status == RunItemActions.Status.StaleItem).View
                .ShouldNotBeNull().External.ShouldBeEmpty();
        session.Inventory.Items.Single(predicate: record => record.Id == item.Id).Condition
                .ShouldBe(expected: 12.5m);
        Loot(session: session).Targets[corpse].EmptySince.ShouldBe(expected: 0);
    }

    /// <summary>A real explicit drop creates and reuses one physical pile at the same contact.</summary>
    [Fact]
    public void Phase06_E107_ExplicitDropCreatesARealPileWithActualRoots()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunItemActions.Controller controller = Phase06MeleeTests.Control();
        Loot(session: session).Targets.ShouldBeEmpty();
        RunInventory.Item shirt = session.Inventory.Items[0];
        RunInventory.Item pants = session.Inventory.Items[1];
        Drop(session: session, source: Path(session: session, itemId: shirt.Id), sequence: 1);
        string pile = Loot(session: session).Targets.Single().Key;
        Loot(session: session).Targets[pile].EmptySince.ShouldBeNull();
        Drop(session: session, source: Path(session: session, itemId: pants.Id), sequence: 2);
        Loot(session: session).Targets.Single().Key.ShouldBe(expected: pile);
        RunItemActions.View contents = Open(session: session, controller: controller, id: pile, sequence: 3,
                kind: RunWorld.Access.Pile);
        contents.External.Select(selector: item => item.Id).ShouldBe(expected: [shirt.Id, pants.Id]);
        session.Step(elapsedMilliseconds: 180000);
        Loot(session: session).Targets.Single().Key.ShouldBe(expected: pile);
    }

    /// <summary>A repeated lethal command cannot resurrect an expired corpse or duplicate its death receipt.</summary>
    [Fact]
    public void Phase06_E108_ExpiredCorpseDoesNotReappearFromItsLethalReceipt()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        Phase06MeleeTests.Enemy(session: session, id: "enemy", x: -7500, health: 1);
        Kill(session: session, victim: "enemy");
        RunDeathAndLoot.Death receipt = Loot(session: session).Deaths.Single().Value;
        session.Step(elapsedMilliseconds: 180000);
        session.Attack(controller: Phase06MeleeTests.Control(), sequence: 1).Replayed.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 0);
        Loot(session: session).Targets.ShouldBeEmpty();
        Loot(session: session).Deaths.Single().Value.ShouldBeSameAs(expected: receipt);
        session.Inventory.Items.Any(predicate: item => item.Location is RunInventory.CorpseEquipment)
                .ShouldBeFalse();
    }

    /// <summary>Refilling before expiry cancels the clock, and re-emptying starts a new interval.</summary>
    [Fact]
    public void Phase06_E109_RefillingAndReEmptyingRespectTheHostOrder()
    {
        var anchor = new RunWorld.Anchor(id: "old-pile", contact: new RunWorld.Point(x: -8000, y: 0),
                kind: RunWorld.Access.Pile);
        var world = new RunWorld(origin: default, groundContactSubunits: 0, mainBuilding: default,
                ranges: RunWorld.DefaultRanges, defenses: [], blockers: [], hostiles: [], anchors: [anchor]);
        FoundationSession session = Phase06MeleeTests.Start(world: world);
        RunItemActions.Controller host = Phase06MeleeTests.Control();
        RunInventory.Item shirt = session.Inventory.Items[0];
        session.Step(elapsedMilliseconds: 179999);
        Drop(session: session, source: Path(session: session, itemId: shirt.Id), sequence: 1, pile: "old-pile");
        session.Step(elapsedMilliseconds: 1);
        Loot(session: session).Targets["old-pile"].EmptySince.ShouldBeNull();
        _ = Open(session: session, controller: host, id: "old-pile", sequence: 2, kind: RunWorld.Access.Pile);
        RunInventory.ItemPath dropped = Path(session: session, itemId: shirt.Id);
        var take = new RunItemActions.Place(expected: dropped, token: 2,
                destination: new RunInventory.Equipment(survivorId: Phase06MeleeTests.Owner, slot: "torso-clothing"));
        session.ItemActions.Submit(controller: host, sequence: 3, request: take).Status
                .ShouldBe(expected: RunItemActions.Status.Applied);
        Loot(session: session).Targets["old-pile"].EmptySince.ShouldBe(expected: 180000);
        session.Step(elapsedMilliseconds: 179999);
        Loot(session: session).Targets.ContainsKey(key: "old-pile").ShouldBeTrue();
        session.Step(elapsedMilliseconds: 1);
        RunItemActions.Outcome stale = session.ItemActions.Submit(controller: host, sequence: 4, request: take);
        stale.Status.ShouldBe(expected: RunItemActions.Status.AccessLost);
        stale.View.ShouldNotBeNull().Selection.ShouldBeNull();
        Loot(session: session).Targets.ShouldBeEmpty();
    }

    /// <summary>A swept lethal impact and its empty expiry preserve their fractional authoritative time.</summary>
    [Fact]
    public void Phase06_E110_FractionalProjectileDeathAndExpiryUseOnePublication()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        Phase06MeleeTests.Enemy(session: session, id: "enemy", x: -5499.5m, health: 1);
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "left-hand");
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-02", slot: "right-hand");
        session.Attack(controller: Phase06MeleeTests.Control(), sequence: 1,
                aim: new RunCombat.Aim(x: 10000, y: 0)).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        long revision = session.Snapshot.Revision;
        session.Step(elapsedMilliseconds: 91);
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
        RunDeathAndLoot.Death death = Loot(session: session).Deaths.Single().Value;
        death.At.ShouldBe(expected: 90.02m);
        Loot(session: session).Targets[death.CorpseId].EmptySince.ShouldBe(expected: 90.02m);
        Phase06MeleeTests.Survivor(session: session, id: "enemy").Health.ShouldBe(expected: 0);
        session.Step(elapsedMilliseconds: 179999);
        Loot(session: session).Targets.ContainsKey(key: death.CorpseId).ShouldBeTrue();
        session.Step(elapsedMilliseconds: 1);
        Loot(session: session).Targets.ShouldBeEmpty();
    }

    internal static RunDeathAndLoot.State Loot(FoundationSession session)
    {
        return session.UnderInventoryAuthority(action: current => current?.Loot).ShouldNotBeNull();
    }

    internal static RunItemActions.Controller Join(FoundationSession session, string binding = "guest")
    {
        session.AddParticipant(bindingId: binding, playerId: binding + "-player", actorId: binding + "-actor",
                label: binding).ShouldBeTrue();
        session.MarkReady(bindingId: binding).ShouldBeTrue();
        return new RunItemActions.Controller(sessionId: "combat", bindingId: binding,
                generation: session.Population.FindLease(bindingId: binding).ShouldNotBeNull().Generation);
    }

    private static void Kill(FoundationSession session, string victim, long sequence = 1)
    {
        RunDamage.Hit hit = session.Attack(controller: Phase06MeleeTests.Control(), sequence: sequence).Hits.Single();
        hit.TargetId.ShouldBe(expected: victim);
        hit.Lethal.ShouldBeTrue();
    }

    private static RunItemActions.View Open(FoundationSession session, RunItemActions.Controller controller, string id,
            long sequence = 1, RunWorld.Access kind = RunWorld.Access.Corpse)
    {
        var target = new RunExternalAccess.Target(kind: kind, id: id);
        RunItemActions.Outcome result = session.ItemActions.Submit(controller: controller, sequence: sequence,
                request: new RunItemActions.Select(target: target));
        result.Status.ShouldBe(expected: RunItemActions.Status.Ready);
        return result.View.ShouldNotBeNull();
    }

    private static RunInventory.ItemPath Path(FoundationSession session, long itemId)
    {
        return session.Inventory.PathTo(itemId: itemId).ShouldNotBeNull();
    }

    private static RunItemActions.Place HandPlacement(RunInventory.ItemPath source, string owner)
    {
        return new RunItemActions.Place(expected: source, token: 1,
                destination: new RunInventory.Equipment(survivorId: owner, slot: "left-hand"));
    }

    private static void Drop(FoundationSession session, RunInventory.ItemPath source, long sequence,
            string? pile = null)
    {
        session.ItemActions.Submit(controller: Phase06MeleeTests.Control(), sequence: sequence,
                request: new RunItemActions.Drop(expected: source, pileId: pile)).Status
                .ShouldBe(expected: RunItemActions.Status.Applied);
    }

    private static RunInventory.Item Store(FoundationSession session, string id, RunInventory.Item container,
            int quantity, int x)
    {
        RunDefinitions.Item rule = session.Definitions.Items[id];
        RunInventory.ContainerCell pocket = RunItemTransfer.Container(definition: rule,
                containerId: container.Id, x: x, y: 0);
        var item = new RunInventory.Item(id: session.Inventory.NextItemId, definitionId: id, quantity: quantity,
                condition: rule.MaximumCondition, location: pocket, storageGrid: rule.Storage);
        session.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(items: inventory.Items.Add(item: item),
                nextItemId: checked(item.Id + 1))).ShouldBeTrue();
        return item;
    }
}
