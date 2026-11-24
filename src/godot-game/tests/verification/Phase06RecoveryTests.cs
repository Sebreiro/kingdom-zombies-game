using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves continuous safe rest and real food use in the host publication.</summary>
[Trait("Tier", "1")]
public sealed class Phase06RecoveryTests
{
    /// <summary>Danger at the inclusive boundary, jumping and sprinting reset safe qualification.</summary>
    [Fact]
    public void Phase06_E018_SafeRestRequiresAContinuousQualifyingInterval()
    {
        FoundationSession session = Start();
        Injure(session: session);
        session.Step(elapsedMilliseconds: 4999);
        Current(session: session).Health.ShouldBe(expected: 50);
        Mode(session: session).ShouldBe(expected: RunVitals.RestMode.Resting);
        session.Step(elapsedMilliseconds: 1);
        Current(session: session).Health.ShouldBe(expected: 50);
        session.Step(elapsedMilliseconds: 1);
        Current(session: session).Health.ShouldBe(expected: 50.002m);
        Mode(session: session).ShouldBe(expected: RunVitals.RestMode.Recovering);

        var hostile = new RunWorld.Hostile(id: "hostile", contact: new RunWorld.Point(x: -20000, y: 0));
        session.TryReplaceRunWorld(candidate: World(hostiles: [hostile])).ShouldBeTrue();
        Mode(session: session).ShouldBe(expected: RunVitals.RestMode.Unsafe);
        session.Step(elapsedMilliseconds: 1);
        Current(session: session).Health.ShouldBe(expected: 50.002m);
        session.TryReplaceRunWorld(candidate: World()).ShouldBeTrue();
        session.Step(elapsedMilliseconds: 5000);
        Current(session: session).Health.ShouldBe(expected: 50.002m);

        var jump = new RunLocomotion.Input(jump: true);
        session.TryAcceptMotion(controller: Controller(), sequence: 1, input: jump).reason
                .ShouldBe(expected: RunLocomotion.Block.None);
        Mode(session: session).ShouldBe(expected: RunVitals.RestMode.Unsafe);
        session.Step(elapsedMilliseconds: 800);
        Mode(session: session).ShouldBe(expected: RunVitals.RestMode.Resting);
        session.Step(elapsedMilliseconds: 5001);
        Current(session: session).Health.ShouldBe(expected: 50.004m);

        var sprint = new RunLocomotion.Input(right: true, sprint: true);
        session.TryAcceptMotion(controller: Controller(), sequence: 2, input: sprint).accepted.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 100);
        Current(session: session).Health.ShouldBe(expected: 50.004m);
        Mode(session: session).ShouldBe(expected: RunVitals.RestMode.Unsafe);
    }

    /// <summary>Two quick slots contest one item and one survivor cooldown under the same commit.</summary>
    [Fact]
    public async Task Phase06_E019_CompetingQuickSlotsConsumeOneUnitAndInvalidateBothReferences()
    {
        FoundationSession session = Start();
        Injure(session: session, health: 31.375m, stamina: 40.125m);
        RunInventory.ItemPath source = AddFood(session: session, id: "IT-09", quantity: 1);
        Assign(session: session, source: source, slot: 1);
        Assign(session: session, source: source, slot: 2);
        long revision = session.Snapshot.Revision;
        Task<RunFood.Result> first = Task.Run(function: () => Consume(session: session, source: source,
                sequence: 1, slot: 1), cancellationToken: TestContext.Current.CancellationToken);
        Task<RunFood.Result> second = Task.Run(function: () => Consume(session: session, source: source,
                sequence: 2, slot: 2), cancellationToken: TestContext.Current.CancellationToken);
        RunFood.Result[] results = await Task.WhenAll(tasks: [first, second]);
        results.Count(predicate: result => result.Status == RunFood.Status.Consumed).ShouldBe(expected: 1);
        results.Count(predicate: result => result.Status is RunFood.Status.StaleFood or RunFood.Status.StaleRequest)
                .ShouldBe(expected: 1);
        session.Inventory.Find(itemId: source.Item.Id).ShouldBeNull();
        session.Inventory.QuickReferences.ShouldBeEmpty();
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
        RunPopulation.Survivor survivor = Current(session: session);
        (survivor.Health, survivor.Stamina).ShouldBe(expected: (46.375m, 65.125m));
        survivor.FoodAvailableAt.ShouldBe(expected: 2000);
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 0);
    }

    /// <summary>Full vitals still consume a real carried item, and the personal timer governs fresh requests.</summary>
    [Fact]
    public void Phase06_E020_FullVitalsConsumeOneActualUnitAndRejectReplay()
    {
        FoundationSession session = Start();
        RunInventory.ItemPath source = AddFood(session: session, id: "IT-08", quantity: 2);
        RunInventory.Item external = session.Inventory.Items.First(predicate: item =>
                item.DefinitionId is "IT-09" && item.Location is RunInventory.StashCell);
        FoundationSnapshot before = session.Snapshot;
        Consume(session: session, source: Path(session: session, id: external.Id), sequence: 1).Status
                .ShouldBe(expected: RunFood.Status.InvalidFood);
        session.Snapshot.ShouldBeSameAs(expected: before);
        Current(session: session).FoodAvailableAt.ShouldBeNull();

        Consume(session: session, source: source, sequence: 2).Status.ShouldBe(expected: RunFood.Status.Consumed);
        (Current(session: session).Health, Current(session: session).Stamina).ShouldBe(expected: (100m, 100m));
        FoundationSnapshot first = session.Snapshot;
        RunInventory.ItemPath remaining = Path(session: session, id: source.Item.Id);
        remaining.Item.Quantity.ShouldBe(expected: 1);
        Consume(session: session, source: remaining, sequence: 2).Replayed.ShouldBeTrue();
        session.Snapshot.ShouldBeSameAs(expected: first);
        session.Step(elapsedMilliseconds: 1999);
        Consume(session: session, source: remaining, sequence: 3).Status.ShouldBe(expected: RunFood.Status.Cooldown);
        Current(session: session).FoodAvailableAt.ShouldBe(expected: 2000);
        session.Step(elapsedMilliseconds: 1);
        Consume(session: session, source: remaining, sequence: 3).Status.ShouldBe(expected: RunFood.Status.Cooldown);
        Consume(session: session, source: source, sequence: 4).Status.ShouldBe(expected: RunFood.Status.StaleFood);
        Current(session: session).FoodAvailableAt.ShouldBe(expected: 2000);
        Consume(session: session, source: remaining, sequence: 5).Status.ShouldBe(expected: RunFood.Status.Consumed);
        session.Inventory.Find(itemId: remaining.Item.Id).ShouldBeNull();
        Current(session: session).FoodAvailableAt.ShouldBe(expected: 4000);
    }

    /// <summary>Walked qualification and configured food gains retain exact values.</summary>
    [Fact]
    public void Phase06_E021_EquivalentWalkPartitionsAndAuthoredFoodKeepExactValues()
    {
        RunWorld world = World(mainX: 5500);
        FoundationSession whole = Start(world: world);
        FoundationSession split = Start(world: world);
        Injure(session: whole);
        Injure(session: split);
        var walk = new RunLocomotion.Input(right: true);
        whole.TryAcceptMotion(controller: Controller(), sequence: 1, input: walk).accepted.ShouldBeTrue();
        split.TryAcceptMotion(controller: Controller(), sequence: 1, input: walk).accepted.ShouldBeTrue();
        whole.Step(elapsedMilliseconds: 6000);
        split.Step(elapsedMilliseconds: 499);
        Mode(session: split).ShouldBe(expected: RunVitals.RestMode.Unsafe);
        split.Step(elapsedMilliseconds: 1);
        Mode(session: split).ShouldBe(expected: RunVitals.RestMode.Resting);
        split.Step(elapsedMilliseconds: 5000);
        Current(session: split).Health.ShouldBe(expected: 50);
        split.Step(elapsedMilliseconds: 500);
        Current(session: whole).Health.ShouldBe(expected: 51);
        Current(session: split).ShouldBe(expected: Current(session: whole));
        CheckFleetingDanger();
        CheckFoodGains();
    }

    private static void CheckFleetingDanger()
    {
        var tangent = new RunWorld.Hostile(id: "tangent", contact: new RunWorld.Point(x: -5000, y: 12000));
        FoundationSession session = Start(world: World(hostiles: [tangent]));
        Injure(session: session);
        session.Step(elapsedMilliseconds: 4000);
        var walk = new RunLocomotion.Input(right: true);
        session.TryAcceptMotion(controller: Controller(), sequence: 1, input: walk).accepted.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 2000);
        Current(session: session).RestMilliseconds.ShouldBe(expected: 1000);
        Current(session: session).Health.ShouldBe(expected: 50);
        session.TryAcceptMotion(controller: Controller(), sequence: 2, input: new RunLocomotion.Input())
                .accepted.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 4000);
        Current(session: session).Health.ShouldBe(expected: 50);
        session.Step(elapsedMilliseconds: 1);
        Current(session: session).Health.ShouldBe(expected: 50.002m);
    }

    private static void CheckFoodGains()
    {
        (string id, decimal health, decimal stamina)[] cases = [("IT-08", 8, 15), ("IT-09", 15, 25),
            ("IT-10", 30, 40)];
        foreach ((string id, decimal health, decimal stamina) in cases)
        {
            FoundationSession session = Start();
            Injure(session: session, health: 12.75m, stamina: 9.125m);
            RunInventory.ItemPath source = AddFood(session: session, id: id, quantity: 1);
            Consume(session: session, source: source, sequence: 1).Status.ShouldBe(expected: RunFood.Status.Consumed);
            RunPopulation.Survivor survivor = Current(session: session);
            (survivor.Health, survivor.Stamina).ShouldBe(expected: (12.75m + health, 9.125m + stamina));
        }

        var configured = FoundationSession.CreateFromResources(sessionId: "recovery", playerId: "host",
                actorId: "actor", read: ReadWithFood);
        Injure(session: configured, health: 92, stamina: 99);
        RunInventory.ItemPath food = AddFood(session: configured, id: "IT-08", quantity: 1);
        Consume(session: configured, source: food, sequence: 1).Status.ShouldBe(expected: RunFood.Status.Consumed);
        (Current(session: configured).Health, Current(session: configured).Stamina)
                .ShouldBe(expected: (92.125m, 100m));
    }

    private static byte[] ReadWithFood(string path, int limit)
    {
        byte[] bytes = Phase06LoadTests.Resource(path: path, limit: limit);
        return path is "content/config/food-wildlife.json" ? Encoding.UTF8.GetBytes(s: Encoding.UTF8.GetString(
                bytes: bytes).Replace(oldValue: "\"health\": 8,", newValue: "\"health\": 0.125,",
                        comparisonType: StringComparison.Ordinal)) : bytes;
    }

    private static FoundationSession Start(RunWorld? world = null)
    {
        return new FoundationSession(sessionId: "recovery", playerId: "host", actorId: "actor",
                world: world ?? World());
    }

    private static RunWorld World(long mainX = 0, RunWorld.Hostile[]? hostiles = null)
    {
        return new RunWorld(origin: default, groundContactSubunits: 0,
                mainBuilding: new RunWorld.Point(x: mainX, y: 0), ranges: RunWorld.DefaultRanges,
                defenses: [], blockers: [], hostiles: hostiles ?? [], anchors: []);
    }

    private static void Injure(FoundationSession session, decimal health = 50, decimal stamina = 100)
    {
        session.TryUpdateVitals(survivorId: "survivor-01", health: health, stamina: stamina).ShouldBeTrue();
    }

    private static RunPopulation.Survivor Current(FoundationSession session)
    {
        return session.Population.Survivors[0];
    }

    private static RunVitals.RestMode Mode(FoundationSession session)
    {
        return RunVitals.RestState(survivor: Current(session: session), definitions: session.Definitions);
    }

    private static RunItemActions.Controller Controller()
    {
        return new RunItemActions.Controller(sessionId: "recovery", bindingId: "local", generation: 1);
    }

    private static RunFood.Result Consume(FoundationSession session, RunInventory.ItemPath source, long sequence,
            int? slot = null)
    {
        return session.ConsumeFood(controller: Controller(), sequence: sequence,
                request: new RunFood.Request(expected: source, quickSlot: slot));
    }

    private static RunInventory.ItemPath AddFood(FoundationSession session, string id, int quantity)
    {
        RunInventory.Item pocket = session.Inventory.Equipped(survivorId: "survivor-01", slot: "legs")
                .ShouldNotBeNull();
        RunDefinitions.Item definition = session.Definitions.Items[id];
        RunInventory.ContainerCell location = RunItemTransfer.Container(definition: definition, containerId:
                pocket.Id, x: 0, y: 0);
        var food = new RunInventory.Item(id: session.Inventory.NextItemId, definitionId: id, quantity: quantity,
                condition: null, location: location, storageGrid: null);
        session.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(items:
                inventory.Items.Add(item: food), nextItemId: food.Id + 1)).ShouldBeTrue();
        return Path(session: session, id: food.Id);
    }

    private static RunInventory.ItemPath Path(FoundationSession session, long id)
    {
        return session.Inventory.PathTo(itemId: id).ShouldNotBeNull();
    }

    private static void Assign(FoundationSession session, RunInventory.ItemPath source, int slot)
    {
        session.TryEditInventory(edit: (inventory, definitions, _) => RunQuickReferences.Assign(
                inventory: inventory, definitions: definitions, survivorId: "survivor-01", slot: slot,
                itemId: source.Item.Id)).ShouldBeTrue();
    }
}
