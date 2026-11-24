using System.Linq;
using System.Text;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies detached authoritative run snapshots and their continuation.</summary>
[Trait("Tier", "1")]
public sealed class Phase06SnapshotTests
{
    /// <summary>Neighboring equal stacks keep their individual identities and clock revision.</summary>
    [Fact]
    public void E118CapturesOneDetachedRunWithExactElapsedTime()
    {
        FoundationSession session = Phase06InventoryTests.Start();
        session.Step(elapsedMilliseconds: 123);
        RunInventory.Item left = Phase06InventoryTests.Item(session: session, definitionId: "IT-01");
        RunInventory.Item right = Phase06InventoryTests.Item(session: session, definitionId: "IT-01", ordinal: 1);
        byte[] data = session.CaptureRunSnapshot();
        session.Step(elapsedMilliseconds: 1);
        FoundationSession restored = Load(data: data);
        restored.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 123);
        Phase06InventoryTests.Find(session: restored, id: left.Id).ShouldBe(expected: left);
        Phase06InventoryTests.Find(session: restored, id: right.Id).ShouldBe(expected: right);
        restored.CaptureRunSnapshot().ShouldBe(expected: data);
    }

    /// <summary>Saved grid extents keep an outlying placement after the configured grid narrows.</summary>
    [Fact]
    public void Phase06_E119_RestorePreservesSavedExtentsAndLegalExtrema()
    {
        FoundationSession source = Phase06InventoryTests.Start();
        RunInventory.Item sourceItem = Phase06InventoryTests.Seed(session: source, definitionId: "IT-01");
        var cell = (RunInventory.StashCell)sourceItem.Location;
        RunInventory.StashCell outer = Phase06InventoryTests.Cell(session: source, item: sourceItem, page: cell.Page,
                x: 9, y: 9);
        Phase06InventoryTests.Move(session: source, source: sourceItem, target: outer).ShouldBeTrue();
        source.TryAddHostile(id: "limit", position: 59000, facing: -1, health: 100).ShouldBeTrue();
        FoundationSession current = Phase06InventoryTests.Configured(oldValue: "      10,",
                newValue: "      7,", config: "content/config/inventory.json");
        current.Definitions.StashGrid.Width.ShouldBe(expected: 7);
        current.TryRestoreRunSnapshot(data: source.CaptureRunSnapshot(), error: out string? error)
                .ShouldBeTrue(customMessage: error);
        current.Inventory.StashGrid.ShouldBe(expected: new RunDefinitions.Size(width: 10, height: 10));
        Phase06InventoryTests.Find(session: current, id: sourceItem.Id).Location.ShouldBe(expected: outer);
        current.Population.Survivors.Single(predicate: survivor => survivor.Hostile).PositionSubunitsExact
                .ShouldBe(expected: 59000);
        Load(data: current.CaptureRunSnapshot()).Inventory.StashGrid.ShouldBe(expected: current.Inventory.StashGrid);
    }

    /// <summary>Empty and single-item inventories keep their exact records and next identity.</summary>
    [Fact]
    public void Phase06_E121_RestoresEmptyAndSingleCollections()
    {
        FoundationSession source = Phase06InventoryTests.Start();
        RunInventory.Item item = Phase06InventoryTests.Item(session: source, definitionId: "IT-01");
        long next = source.Inventory.NextItemId;
        source.TryEditInventory(edit: (current, _, _) => current.WithItems(items: [item])).ShouldBeTrue();
        FoundationSession single = Load(data: source.CaptureRunSnapshot());
        single.Inventory.Items.Single().ShouldBe(expected: item);
        single.Inventory.NextItemId.ShouldBe(expected: next);
        source.TryEditInventory(edit: (current, _, _) => current.WithItems(items: [])).ShouldBeTrue();
        FoundationSession empty = Load(data: source.CaptureRunSnapshot());
        empty.Inventory.Items.IsEmpty.ShouldBeTrue();
        empty.Inventory.NextItemId.ShouldBe(expected: next);
        empty.UnderInventoryAuthority(action: root => root?.Combat.Arrows.IsEmpty).ShouldBe(expected: true);
    }

    /// <summary>Duplicate fields cannot replace the active root, even with the same value.</summary>
    [Fact]
    public void Phase06_E122_RejectsAmbiguousRecordsWithoutChangingTheRoot()
    {
        FoundationSession session = Phase06InventoryTests.Start();
        byte[] baseline = session.CaptureRunSnapshot();
        byte[] duplicate = Encoding.UTF8.GetBytes(s:
                Encoding.UTF8.GetString(bytes: baseline).Insert(startIndex: 1, value: "\"Version\":1,"));
        FoundationSession.RunPublication? before = session.UnderInventoryAuthority(action: root => root);
        session.TryRestoreRunSnapshot(data: duplicate, error: out string? error).ShouldBeFalse();
        error.ShouldNotBeNullOrEmpty();
        session.UnderInventoryAuthority(action: root => root).ShouldBeSameAs(expected: before);
        session.CaptureRunSnapshot().ShouldBe(expected: baseline);
    }

    /// <summary>Precise state and whole quantities survive through detached bytes.</summary>
    [Fact]
    public void Phase06_E124_KeepsFractionalConditionAndVitalsWithIntegralCounts()
    {
        FoundationSession source = Phase06InventoryTests.Start();
        RunInventory.Item weapon = Phase06InventoryTests.Seed(session: source, definitionId: "IT-07",
                condition: 17.123456789m);
        RunInventory.Item coins = Phase06InventoryTests.Item(session: source, definitionId: "IT-01");
        source.TryUpdateVitals(survivorId: Phase06InventoryTests.Survivor, health: 88.123456789m,
                stamina: 62.123456789m).ShouldBeTrue();
        FoundationSession restored = Load(data: source.CaptureRunSnapshot());
        Phase06InventoryTests.Find(session: restored, id: weapon.Id).Condition.ShouldBe(expected: 17.123456789m);
        Phase06InventoryTests.Find(session: restored, id: coins.Id).Quantity.ShouldBe(expected: coins.Quantity);
        restored.Population.Survivors[0].Health.ShouldBe(expected: 88.123456789m);
        restored.Population.Survivors[0].Stamina.ShouldBe(expected: 62.123456789m);
    }

    private static FoundationSession Load(byte[] data)
    {
        FoundationSession restored = Phase06InventoryTests.Start();
        restored.TryRestoreRunSnapshot(data: data, error: out string? error).ShouldBeTrue(customMessage: error);
        return restored;
    }
}
