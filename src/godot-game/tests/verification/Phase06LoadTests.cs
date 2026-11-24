using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves weight from actual equipment and nested inventory during host movement.</summary>
[Trait("Tier", "1")]
public sealed class Phase06LoadTests
{
    /// <summary>Two adjacent identical stacks inside a nested pouch each contribute their own quantity.</summary>
    [Fact]
    public void Phase06_E022_AdjacentNestedStacksCountTheirOwnWeights()
    {
        FoundationSession session = Start();
        RunInventory.Item bag = Bag(session: session);
        RunInventory.Item pouch = Seed(session: session, id: "IT-22", location:
                Pocket(session: session, id: "IT-22", container: bag));
        RunInventory.Item first = Seed(session: session, id: "IT-01", quantity: 100, location:
                Pocket(session: session, id: "IT-01", container: pouch));
        RunInventory.Item second = Seed(session: session, id: "IT-01", quantity: 80, location:
                Pocket(session: session, id: "IT-01", container: pouch, x: 1));
        (first.Id != second.Id).ShouldBeTrue();
        Load(session: session).Weight.ShouldBe(expected: 2.83m);
        RunLocomotion.Load(inventory: session.Inventory, definitions: session.Definitions, survivorId: "survivor-02")
                .Weight.ShouldBe(expected: 1.7m);
    }

    /// <summary>Exact soft load is unpenalized. Severe allows walking and rejects sprint and jump.</summary>
    [Fact]
    public void Phase06_E023_ExactSoftAndSevereThresholdsUseTheCurrentLoad()
    {
        FoundationSession soft = Loaded(quantity: 183);
        CheckBurden(session: soft, weight: 20, level: RunLocomotion.LoadLevel.Normal, speed: 1, cost: 1);
        Hold(session: soft).ShouldBe(expected: (true, RunLocomotion.Block.None));
        soft.Step(elapsedMilliseconds: 1000);
        soft.Actor.PositionSubunits.ShouldBe(expected: -3000);
        FoundationSession severe = Loaded(quantity: 333);
        CheckBurden(session: severe, weight: 35, level: RunLocomotion.LoadLevel.Overloaded, speed: 0.55m, cost: 1.5m);
        Hold(session: severe, jump: true).ShouldBe(expected: (true, RunLocomotion.Block.Overloaded));
        severe.Step(elapsedMilliseconds: 1000);
        severe.Actor.PositionSubunits.ShouldBe(expected: -6350);
        severe.Population.Survivors[0].SprintBlock.ShouldBe(expected: RunLocomotion.Block.Overloaded);
        severe.Population.Survivors[0].Stamina.ShouldBe(expected: 100);
    }

    /// <summary>Moving a loaded subtree and advancing use one serialized ownership order.</summary>
    [Fact]
    public async Task Phase06_E024_StepAndDroppingALoadedBagSeeOneOwner()
    {
        FoundationSession session = Start(heavyMoney: true);
        RunInventory.Item bag = Bag(session: session);
        _ = Seed(session: session, id: "IT-01", quantity: 400, location:
                Pocket(session: session, id: "IT-01", container: bag));
        RunInventory.ItemPath original = session.Inventory.PathTo(itemId: bag.Id).ShouldNotBeNull();
        Hold(session: session, sprint: false).accepted.ShouldBeTrue();
        var step = Task.Run(action: () => session.Step(elapsedMilliseconds: 1000),
                cancellationToken: TestContext.Current.CancellationToken);
        Task<bool> drop = Task.Run(function: () => session.TryEditInventory(edit: (inventory, _, _) =>
                RunItemTransfer.Drop(inventory: inventory, expected: original, pileId: "load-test")),
                cancellationToken: TestContext.Current.CancellationToken);
        await Task.WhenAll(tasks: [step, drop]);
        (await drop).ShouldBeTrue();
        long position = session.Actor.PositionSubunits;
        (position is -6350 or -5000).ShouldBeTrue();
        Load(session: session).Weight.ShouldBe(expected: 1.7m);
        session.Inventory.PathTo(itemId: bag.Id)?.Root.ShouldBe(expected:
                new RunInventory.Ground(pileId: "load-test"));
    }

    /// <summary>Empty storage adds its own mass alone, then a single item adds its actual quantity.</summary>
    [Fact]
    public void Phase06_E025_EmptyAndSingleStorageAreExact()
    {
        FoundationSession session = Start();
        RunInventory.Item bag = Bag(session: session);
        Load(session: session).Weight.ShouldBe(expected: 2.5m);
        _ = Seed(session: session, id: "IT-02", quantity: 30, location:
                Pocket(session: session, id: "IT-02", container: bag));
        Load(session: session).Weight.ShouldBe(expected: 3.4m);
    }

    /// <summary>Replaying a cyclic candidate or reading the same load preserves the live publication.</summary>
    [Fact]
    public void Phase06_E026_RejectedCyclesAndRepeatedReadsPreserveMass()
    {
        FoundationSession session = Start();
        RunInventory.Item bag = Bag(session: session);
        RunInventory.ItemPath expected = session.Inventory.PathTo(itemId: bag.Id).ShouldNotBeNull();
        RunInventory.ContainerCell self = Pocket(session: session, id: bag.DefinitionId, container: bag);
        FoundationSnapshot before = session.Snapshot;
        RunLocomotion.Burden load = Load(session: session);
        for (int attempt = 0; attempt < 2; attempt++)
        {
            session.TryEditInventory(edit: (inventory, _, _) => RunItemTransfer.Move(inventory: inventory,
                    expected: expected, destination: self)).ShouldBeFalse();
            Load(session: session).ShouldBe(expected: load);
        }

        session.Snapshot.ShouldBeSameAs(expected: before);
    }

    /// <summary>Backing enumeration order cannot alter exact load or the next simulated movement.</summary>
    [Fact]
    public void Phase06_E027_ReversingRecordsKeepsMassAndMotion()
    {
        FoundationSession baseline = Loaded(quantity: 258);
        FoundationSession reordered = Loaded(quantity: 258);
        reordered.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(items:
                [.. inventory.Items.Reverse()])).ShouldBeTrue();
        Load(session: reordered).ShouldBe(expected: Load(session: baseline));
        Hold(session: baseline).accepted.ShouldBeTrue();
        Hold(session: reordered).accepted.ShouldBeTrue();
        baseline.Step(elapsedMilliseconds: 333);
        reordered.Step(elapsedMilliseconds: 333);
        reordered.Population.Survivors[0].PositionSubunitsExact.ShouldBe(expected:
                baseline.Population.Survivors[0].PositionSubunitsExact);
        reordered.Population.Survivors[0].Stamina.ShouldBe(expected: baseline.Population.Survivors[0].Stamina);
    }

    /// <summary>Interior load retains fractional multipliers and movement after exact boundaries.</summary>
    [Fact]
    public void Phase06_E028_MidpointCostAndPositionRetainFractions()
    {
        FoundationSession session = Loaded(quantity: 258);
        CheckBurden(session: session, weight: 27.5m, level: RunLocomotion.LoadLevel.Heavy, speed: 0.775m,
                cost: 1.25m);
        Hold(session: session).accepted.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 1);
        RunPopulation.Survivor survivor = session.Population.Survivors[0];
        survivor.PositionSubunitsExact.ShouldBe(expected: -7996.125m);
        survivor.Stamina.ShouldBe(expected: 99.9875m);
        Load(session: Loaded(quantity: 184)).Level.ShouldBe(expected: RunLocomotion.LoadLevel.Heavy);
        Load(session: Loaded(quantity: 332)).Level.ShouldBe(expected: RunLocomotion.LoadLevel.Heavy);
        Load(session: Loaded(quantity: 334)).Level.ShouldBe(expected: RunLocomotion.LoadLevel.Overloaded);
    }

    internal static byte[] Resource(string path, int limit)
    {
        using Stream stream = typeof(FoundationSession).Assembly.GetManifestResourceStream(name: path)
                ?? throw new InvalidOperationException(message: path);
        stream.Length.ShouldBeLessThanOrEqualTo(expected: limit);
        byte[] bytes = new byte[checked((int)stream.Length)];
        stream.ReadExactly(buffer: bytes);
        return bytes;
    }

    private static FoundationSession Start(bool heavyMoney = false)
    {
        if (heavyMoney)
        {
            return FoundationSession.CreateFromResources(sessionId: "load", playerId: "host",
                    actorId: "actor", read: HeavyMoney);
        }

        return new FoundationSession(sessionId: "load", playerId: "host", actorId: "actor");
    }

    private static FoundationSession Loaded(int quantity)
    {
        FoundationSession session = Start(heavyMoney: true);
        RunInventory.Item shirt = session.Inventory.Equipped(survivorId: "survivor-01", slot: "torso-clothing")
                .ShouldNotBeNull();
        _ = Seed(session: session, id: "IT-01", quantity: quantity, location:
                Pocket(session: session, id: "IT-01", container: shirt));
        return session;
    }

    private static byte[] HeavyMoney(string path, int limit)
    {
        byte[] bytes = Resource(path: path, limit: limit);
        if (path is not "content/config/items.json")
        {
            return bytes;
        }

        string text = Encoding.UTF8.GetString(bytes: bytes);
        string heavier = text.Replace(oldValue: "\"weightKilograms\": 0.001", newValue: "\"weightKilograms\": 0.1",
                comparisonType: StringComparison.Ordinal);
        string larger = heavier.Replace(oldValue: "\"maxStack\": 100,", newValue: "\"maxStack\": 1000,",
                comparisonType: StringComparison.Ordinal);
        return Encoding.UTF8.GetBytes(s: larger);
    }

    private static RunLocomotion.Burden Load(FoundationSession session)
    {
        return RunLocomotion.Load(inventory: session.Inventory, definitions: session.Definitions,
                survivorId: "survivor-01");
    }

    private static void CheckBurden(FoundationSession session, decimal weight, RunLocomotion.LoadLevel level,
            decimal speed, decimal cost)
    {
        Load(session: session).ShouldBe(expected: new RunLocomotion.Burden(weight: weight, level: level, speed: speed,
                actionCost: cost));
    }

    private static (bool accepted, RunLocomotion.Block reason) Hold(FoundationSession session, bool sprint = true,
            bool jump = false)
    {
        var controller = new RunItemActions.Controller(sessionId: "load", bindingId: "local", generation: 1);
        return session.TryAcceptMotion(controller: controller, sequence: 1, input:
                new RunLocomotion.Input(right: true, sprint: sprint, jump: jump));
    }

    private static RunInventory.Item Bag(FoundationSession session)
    {
        return Seed(session: session, id: "IT-23", location: new RunInventory.Equipment(survivorId: "survivor-01",
                slot: "backpack"));
    }

    private static RunInventory.Item Seed(FoundationSession session, string id, RunInventory.Location location,
            int quantity = 1)
    {
        RunDefinitions.Item rule = session.Definitions.Items[id];
        var item = new RunInventory.Item(id: session.Inventory.NextItemId, definitionId: id, quantity: quantity,
                condition: rule.MaximumCondition, location: location, storageGrid: rule.Storage);
        session.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(items:
                inventory.Items.Add(item: item), nextItemId: checked(item.Id + 1))).ShouldBeTrue();
        return item;
    }

    private static RunInventory.ContainerCell Pocket(FoundationSession session, string id, RunInventory.Item container,
            int x = 0)
    {
        return RunItemTransfer.Container(definition: session.Definitions.Items[id], containerId: container.Id,
                x: x, y: 0);
    }
}
