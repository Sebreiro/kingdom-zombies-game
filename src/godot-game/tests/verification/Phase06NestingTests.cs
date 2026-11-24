using System;
using System.IO;
using System.Linq;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

using static KingdomZombies.Phase1.Tests.Phase06InventoryTests;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves filled-container placement and exact carried references in the run.</summary>
[Trait("Tier", "1")]
public sealed class Phase06NestingTests
{
    /// <summary>Equal coordinates in distinct item grids are independent and adjacent cells are legal.</summary>
    [Fact]
    public void Phase06_E050_AdjacentPocketsUseTheirActualContainerIdentity()
    {
        FoundationSession session = Start();
        RunInventory.Item pack = Equip(session: session, definitionId: "IT-23", slot: "backpack");
        RunInventory.Item belt = Equip(session: session, definitionId: "IT-21", slot: "belt");
        RunInventory.Item first = Store(session: session, definitionId: "IT-08", container: pack);
        RunInventory.Item second = Store(session: session, definitionId: "IT-08", container: belt, ordinal: 1);
        RunInventory.ContainerCell target = In(session: session, item: second, container: pack, x: 1);
        Move(session: session, source: second, target: target).ShouldBeTrue();
        session.Inventory.CarriedBy(survivorId: Survivor).Count(predicate: item =>
                item.DefinitionId is "IT-08").ShouldBe(expected: 2);
        Find(session: session, id: first.Id).ShouldBeSameAs(expected: first);
        RunInventory.Item adjacent = Find(session: session, id: second.Id);
        Reject(session: session, edit: () => Move(session: session, source: adjacent,
                target: In(session: session, item: adjacent, container: pack)));
    }

    /// <summary>The default limit permits three containers and checks the deepest moved descendant.</summary>
    [Fact]
    public void Phase06_E051_FilledTreeCannotCrossTheDefaultDepthLimit()
    {
        FoundationSession session = Start();
        session.Definitions.MaximumContainerLevels.ShouldBe(expected: 3);
        RunInventory.Item root = Equip(session: session, definitionId: "IT-23", slot: "backpack");
        RunInventory.Item middle = Store(session: session, definitionId: "IT-23", container: root, ordinal: 1);
        RunInventory.Item leaf = Store(session: session, definitionId: "IT-21", container: middle);
        RunInventory.Item food = Store(session: session, definitionId: "IT-10", container: leaf);
        Path(session: session, item: food).Items.Select(selector: item => item.Id)
                .ShouldBe(expected: [food.Id, leaf.Id, middle.Id, root.Id]);
        RunInventory.Item fourth = Seed(session: session, definitionId: "IT-24");
        Reject(session: session, edit: () => Move(session: session, source: root,
                target: In(session: session, item: root, container: fourth)));
        RunInventory.Item spareBelt = Item(session: session, definitionId: "IT-21", ordinal: 1);
        Reject(session: session, edit: () => Move(session: session, source: spareBelt,
                target: In(session: session, item: spareBelt, container: leaf, x: 2, y: 1)));
        Move(session: session, source: leaf, target: In(session: session, item: leaf, container: fourth))
                .ShouldBeTrue();
        Path(session: session, item: food).Items.Select(selector: item => item.Id)
                .ShouldBe(expected: [food.Id, leaf.Id, fourth.Id]);
        Find(session: session, id: food.Id).ShouldBeSameAs(expected: food);
    }

    /// <summary>A concurrent child move cannot use its old path after the ancestor moves.</summary>
    [Fact]
    public void Phase06_E052_ContestedAncestorPreservesTheSerializedChildOutcome()
    {
        FoundationSession session = Start();
        RunInventory.Item pack = Equip(session: session, definitionId: "IT-23", slot: "backpack");
        RunInventory.Item food = Store(session: session, definitionId: "IT-08", container: pack);
        RunInventory.ItemPath ancestor = Path(session: session, item: pack);
        RunInventory.ItemPath expected = Path(session: session, item: food);
        RunInventory.ContainerCell adjacent = In(session: session, item: food, container: pack, x: 1);
        Assign(session: session, slot: 1, itemId: food.Id).ShouldBeTrue();
        int committed = Race(first: () => Drop(session: session, expected: ancestor, pileId: "pile-1"),
                second: () => MoveExpected(session: session, expected: expected, target: adjacent));
        RunInventory.Item after = Find(session: session, id: food.Id);
        (after.Quantity, after.Condition).ShouldBe(expected: (food.Quantity, food.Condition));
        after.Location.ShouldBe(expected: committed == 2 ? adjacent : food.Location);
        Path(session: session, item: food).Root.ShouldBe(expected: new RunInventory.Ground(pileId: "pile-1"));
        Resolve(session: session, slot: 1).ShouldBeNull();
        RunInventory.Location open = committed == 2 ? food.Location : adjacent;
        Reject(session: session, edit: () => MoveExpected(session: session, expected: expected, target: open));
    }

    /// <summary>A real empty grid can be moved; non-storage equipment cannot acquire an invisible pocket.</summary>
    [Fact]
    public void Phase06_E053_EmptyStorageAndMissingStorageAreDistinct()
    {
        FoundationSession session = Start();
        RunInventory.Item trousers = Item(session: session, definitionId: "IT-12");
        RunInventory.Item boots = Item(session: session, definitionId: "IT-17");
        RunInventory.Item pouch = Seed(session: session, definitionId: "IT-22");
        pouch.StorageGrid.ShouldBe(expected: new RunDefinitions.Size(width: 2, height: 2));
        Reject(session: session, edit: () => Move(session: session, source: pouch,
                target: In(session: session, item: pouch, container: boots)));
        Move(session: session, source: pouch, target: In(session: session, item: pouch, container: trousers))
                .ShouldBeTrue();
        session.Inventory.CarriedBy(survivorId: Survivor).Select(selector: item => item.Id).Order()
                .ShouldBe(expected: [1L, 2L, 3L, pouch.Id]);
        Drop(session: session, expected: Path(session: session, item: pouch), pileId: "pile-empty").ShouldBeTrue();
        session.Inventory.CarriedBy(survivorId: Survivor).Select(selector: item => item.Id)
                .ShouldBe(expected: [1L, 2L, 3L]);
        session.Inventory.Items.Any(predicate: item =>
                item.Location is RunInventory.ContainerCell cell && cell.ContainerId == pouch.Id).ShouldBeFalse();
    }

    /// <summary>Deliberate drop publishes the complete subtree once and a preview leaves no committed state.</summary>
    [Fact]
    public void Phase06_E054_ExplicitDropKeepsAllNestedRecordsAndRejectsReplay()
    {
        FoundationSession session = Start();
        RunInventory.Item pack = Equip(session: session, definitionId: "IT-23", slot: "backpack");
        RunInventory.Item club = Store(session: session, definitionId: "IT-04", container: pack);
        RunInventory.Item food = Store(session: session, definitionId: "IT-09", container: pack, x: 1);
        Assign(session: session, slot: 1, itemId: food.Id).ShouldBeTrue();
        RunInventory.ItemPath expected = Path(session: session, item: pack);
        RunInventory root = session.Inventory;
        var ground = new RunInventory.Ground(pileId: "pile-2");
        RunInventory? preview = RunItemTransfer.Drop(inventory: root, expected: expected, pileId: ground.PileId);
        preview.ShouldNotBeNull();
        session.Inventory.ShouldBeSameAs(expected: root);
        Reject(session: session, edit: () => MoveExpected(session: session, expected: expected, target: ground));
        session.TryEditInventory(edit: (current, _, _) => RunItemTransfer.Drop(inventory: current,
                expected: expected, pileId: ground.PileId), change: out RunItemTransfer.Change? change).ShouldBeTrue();
        change.ShouldNotBeNull();
        change.Before.ShouldBeSameAs(expected: root);
        change.After.PathTo(itemId: food.Id)?.Root.ShouldBe(expected: ground);
        change.Before.PathTo(itemId: food.Id)?.IsCarriedBy(survivorId: Survivor).ShouldBe(expected: true);
        change.ChangedCells().IsEmpty.ShouldBeTrue();
        Find(session: session, id: food.Id).ShouldBeSameAs(expected: food);
        Find(session: session, id: club.Id).ShouldBeSameAs(expected: club);
        Resolve(session: session, slot: 1).ShouldBeNull();
        Reject(session: session, edit: () => Drop(session: session, expected: expected, pileId: ground.PileId));
        Move(session: session, source: Find(session: session, id: pack.Id), target: pack.Location).ShouldBeTrue();
        Resolve(session: session, slot: 1).ShouldBeNull();
    }

    /// <summary>Equal-looking bags and a filled coat retain their child identities after reordering.</summary>
    [Fact]
    public void Phase06_E055_OnlyTheMovedEqualLookingBagTakesItsDescendants()
    {
        FoundationSession session = Start();
        RunInventory.Item first = Equip(session: session, definitionId: "IT-23", slot: "backpack");
        RunInventory.Item second = Item(session: session, definitionId: "IT-23", ordinal: 1);
        RunInventory.Item firstFood = Store(session: session, definitionId: "IT-09", container: first);
        RunInventory.Item secondFood = Store(session: session, definitionId: "IT-09", container: second, ordinal: 1);
        RunInventory.Item coat = Seed(session: session, definitionId: "IT-13");
        RunInventory.Item coatFood = Store(session: session, definitionId: "IT-08", container: coat);
        Reverse(session: session);
        Move(session: session, source: coat, target: In(session: session, item: coat, container: first, x: 1))
                .ShouldBeTrue();
        Assign(session: session, slot: 2, itemId: coatFood.Id).ShouldBeTrue();
        Move(session: session, source: Find(session: session, id: coat.Id),
                target: In(session: session, item: coat, container: second, x: 1)).ShouldBeTrue();
        Path(session: session, item: firstFood).Root.ShouldBe(expected: first.Location);
        Path(session: session, item: secondFood).Root.ShouldBe(expected: second.Location);
        Path(session: session, item: coatFood).Items.Select(selector: item => item.Id)
                .ShouldBe(expected: [coatFood.Id, coat.Id, second.Id]);
        Find(session: session, id: coatFood.Id).ShouldBeSameAs(expected: coatFood);
        Resolve(session: session, slot: 2).ShouldBeNull();
    }

    /// <summary>Root level one, configured limit two, cycles and duplicate identities are checked exactly.</summary>
    [Fact]
    public void Phase06_E056_ConfiguredDepthAndQuantitiesStayIntegral()
    {
        FoundationSession session = WithDepth(value: "2");
        session.Definitions.MaximumContainerLevels.ShouldBe(expected: 2);
        RunInventory.Item outer = Seed(session: session, definitionId: "IT-22");
        RunInventory.Item inner = Seed(session: session, definitionId: "IT-22");
        RunInventory.Item third = Seed(session: session, definitionId: "IT-22");
        Reject(session: session, edit: () => Move(session: session, source: outer,
                target: In(session: session, item: outer, container: outer)));
        Move(session: session, source: inner, target: In(session: session, item: inner, container: outer))
                .ShouldBeTrue();
        Reject(session: session, edit: () => Move(session: session, source: outer,
                target: In(session: session, item: outer, container: inner)));
        Reject(session: session, edit: () => Move(session: session, source: third,
                target: In(session: session, item: third, container: inner)));
        Reject(session: session, edit: () => session.TryEditInventory(edit: (current, _, _) =>
                current.WithItems(items: current.Items.Add(item: outer))));
        Path(session: session, item: inner).Items.Select(selector: item => item.Id)
                .ShouldBe(expected: [inner.Id, outer.Id]);
        foreach (string invalid in new[] { "0", "-1", "1.5", "2147483648" })
        {
            Should.Throw<InvalidDataException>(actual: () => WithDepth(value: invalid));
        }
    }

    /// <summary>Moving and splitting a stack preserve its references and exact remaining quantity.</summary>
    [Fact]
    public void Phase06_E057_MovingAndSplittingKeepsTheOriginalReference()
    {
        FoundationSession session = Start();
        RunInventory.Item shirt = Item(session: session, definitionId: "IT-11");
        RunInventory.Item trousers = Item(session: session, definitionId: "IT-12");
        RunInventory.Item food = Store(session: session, definitionId: "IT-08", container: shirt);
        Assign(session: session, slot: 1, itemId: food.Id).ShouldBeTrue();
        Assign(session: session, slot: 4, itemId: food.Id).ShouldBeTrue();
        RunInventory.ItemPath expected = Path(session: session, item: food);
        RunInventory.ContainerCell target = In(session: session, item: food, container: trousers);
        session.TryEditInventory(edit: (current, _, _) => RunItemTransfer.Move(inventory: current,
                expected: expected, destination: target), change: out RunItemTransfer.Change? change).ShouldBeTrue();
        change.ShouldNotBeNull();
        change.ChangedCells().AsEnumerable().ShouldBe(expected:
                [(RunInventory.GridKind.Container, shirt.Id, 0, 0),
                    (RunInventory.GridKind.Container, trousers.Id, 0, 0)]);
        long extracted = session.Inventory.NextItemId;
        RunInventory.Item moved = Find(session: session, id: food.Id);
        Split(session: session, source: moved, quantity: 3,
                target: In(session: session, item: moved, container: trousers, x: 1)).ShouldBeTrue();
        Resolve(session: session, slot: 1)?.Item.ShouldBe(expected: moved with { Quantity = 7 });
        Resolve(session: session, slot: 4)?.Item.Id.ShouldBe(expected: food.Id);
        Find(session: session, id: extracted).Quantity.ShouldBe(expected: 3);
    }

    /// <summary>The first and fourth indices are inclusive and each slot stores only one reference.</summary>
    [Fact]
    public void Phase06_E058_ExactlyFourQuickSlotsCanReferenceOneRealStack()
    {
        FoundationSession session = Start();
        RunInventory.Item food = Store(session: session, definitionId: "IT-08",
                container: Item(session: session, definitionId: "IT-11"));
        for (int slot = 1; slot <= 4; slot++)
        {
            Assign(session: session, slot: slot, itemId: food.Id).ShouldBeTrue();
            Resolve(session: session, slot: slot)?.Item.ShouldBeSameAs(expected: food);
        }

        foreach (int slot in new[] { int.MinValue, 0, 5, int.MaxValue })
        {
            Reject(session: session, edit: () => Assign(session: session, slot: slot, itemId: food.Id));
            Resolve(session: session, slot: slot).ShouldBeNull();
        }

        session.Inventory.QuickReferences.Count.ShouldBe(expected: 4);
        session.Inventory.Items.Count(predicate: item => item.Id == food.Id).ShouldBe(expected: 1);
        Find(session: session, id: food.Id).Quantity.ShouldBe(expected: 10);
    }

    /// <summary>Concurrent requests for one food identity cannot both spend the same expected quantity.</summary>
    [Fact]
    public void Phase06_E059_SharedShortcutsCompeteForOnlyOneActualSupply()
    {
        FoundationSession session = Start();
        RunInventory.Item food = Seed(session: session, definitionId: "IT-08", quantity: 2);
        RunInventory.Item shirt = Item(session: session, definitionId: "IT-11");
        Move(session: session, source: food, target: In(session: session, item: food, container: shirt)).ShouldBeTrue();
        Assign(session: session, slot: 1, itemId: food.Id).ShouldBeTrue();
        Assign(session: session, slot: 2, itemId: food.Id).ShouldBeTrue();
        RunInventory.ItemPath? first = Resolve(session: session, slot: 1);
        RunInventory.ItemPath? second = Resolve(session: session, slot: 2);
        int committed = Race(first: () => Spend(session: session, expected: first),
                second: () => Spend(session: session, expected: second));
        committed.ShouldBe(expected: 1);
        Resolve(session: session, slot: 1)?.Item.Quantity.ShouldBe(expected: 1);
        Resolve(session: session, slot: 2)?.Item.Id.ShouldBe(expected: food.Id);
        Spend(session: session, expected: Resolve(session: session, slot: 2)).ShouldBeTrue();
        session.Inventory.QuickReferences.IsEmpty.ShouldBeTrue();
    }

    /// <summary>Empty, external, foreign and non-food selections create no quick reference or consumption.</summary>
    [Fact]
    public void Phase06_E060_EmptyAndNonCarriedSelectionsHaveNoEffect()
    {
        FoundationSession session = Start();
        RunInventory.Item external = Item(session: session, definitionId: "IT-08");
        RunInventory.Item club = Equip(session: session, definitionId: "IT-04", slot: "left-hand");
        RunInventory.Item foreignShirt = Item(session: session, definitionId: "IT-11", ordinal: 1);
        RunInventory.Item foreign = Store(session: session, definitionId: "IT-09", container: foreignShirt);
        Reject(session: session, edit: () => Assign(session: session, slot: 1, itemId: external.Id));
        Reject(session: session, edit: () => Assign(session: session, slot: 1, itemId: club.Id));
        Reject(session: session, edit: () => Assign(session: session, slot: 1, itemId: foreign.Id));
        Reject(session: session, edit: () => Assign(session: session, slot: 1, itemId: long.MaxValue));
        Reject(session: session, edit: () => Spend(session: session, expected: Path(session: session, item: external)));
        Reject(session: session, edit: () => Spend(session: session, expected: Resolve(session: session, slot: 1)));
        session.Inventory.QuickReferences.IsEmpty.ShouldBeTrue();
    }

    /// <summary>Replaying assignment or exhaustion has no second effect and never substitutes an equal stack.</summary>
    [Fact]
    public void Phase06_E061_ConsumedIdentityCannotBeReplayedOrSubstituted()
    {
        FoundationSession session = Start();
        RunInventory.Item shirt = Item(session: session, definitionId: "IT-11");
        RunInventory.Item food = Seed(session: session, definitionId: "IT-08");
        RunInventory.Item equal = Seed(session: session, definitionId: "IT-08");
        Move(session: session, source: food, target: In(session: session, item: food, container: shirt)).ShouldBeTrue();
        Move(session: session, source: equal, target: In(session: session, item: equal, container: shirt, x: 1))
                .ShouldBeTrue();
        Assign(session: session, slot: 2, itemId: food.Id).ShouldBeTrue();
        Reject(session: session, edit: () => Assign(session: session, slot: 2, itemId: food.Id));
        RunInventory.ItemPath? prepared = Resolve(session: session, slot: 2);
        Spend(session: session, expected: prepared).ShouldBeTrue();
        Reject(session: session, edit: () => Spend(session: session, expected: prepared));
        Resolve(session: session, slot: 2).ShouldBeNull();
        session.Inventory.Find(itemId: food.Id).ShouldBeNull();
        Find(session: session, id: equal.Id).Quantity.ShouldBe(expected: 1);
    }

    /// <summary>After reordering, transferring one food stack clears only that stack's references.</summary>
    [Fact]
    public void Phase06_E062_TransferAwayClearsOnlyThatSurvivorsExactReferences()
    {
        FoundationSession session = Start();
        RunInventory.Item firstShirt = Item(session: session, definitionId: "IT-11");
        RunInventory.Item secondShirt = Item(session: session, definitionId: "IT-11", ordinal: 1);
        RunInventory.Item first = Store(session: session, definitionId: "IT-08", container: firstShirt);
        RunInventory.Item second = Store(session: session, definitionId: "IT-08", container: firstShirt,
                ordinal: 1, x: 1);
        Assign(session: session, slot: 4, itemId: first.Id).ShouldBeTrue();
        Assign(session: session, slot: 1, itemId: second.Id).ShouldBeTrue();
        Assign(session: session, slot: 2, itemId: first.Id).ShouldBeTrue();
        Reverse(session: session);
        Resolve(session: session, slot: 1)?.Item.ShouldBeSameAs(expected: second);
        Resolve(session: session, slot: 4)?.Item.ShouldBeSameAs(expected: first);
        Move(session: session, source: first, target: In(session: session, item: first, container: secondShirt))
                .ShouldBeTrue();
        Resolve(session: session, slot: 4).ShouldBeNull();
        Resolve(session: session, slot: 2).ShouldBeNull();
        Resolve(session: session, slot: 1)?.Item.ShouldBeSameAs(expected: second);
        session.Inventory.QuickReferences.Count.ShouldBe(expected: 1);
        Path(session: session, item: first).IsCarriedBy(survivorId: "survivor-02").ShouldBeTrue();
    }

    /// <summary>Partial merges retain exact references and a full merge retires only the depleted source.</summary>
    [Fact]
    public void Phase06_E063_ReferenceLifetimeFollowsExactMergeQuantities()
    {
        FoundationSession session = Start();
        RunInventory.Item shirt = Item(session: session, definitionId: "IT-11");
        RunInventory.Item first = Store(session: session, definitionId: "IT-09", container: shirt);
        RunInventory.Item second = Seed(session: session, definitionId: "IT-09", quantity: 4);
        Move(session: session, source: second, target: In(session: session, item: second, container: shirt, x: 1))
                .ShouldBeTrue();
        Assign(session: session, slot: 1, itemId: first.Id).ShouldBeTrue();
        Assign(session: session, slot: 2, itemId: second.Id).ShouldBeTrue();
        Merge(session: session, source: first, target: Find(session: session, id: second.Id)).ShouldBeTrue();
        Resolve(session: session, slot: 1)?.Item.Quantity.ShouldBe(expected: 4);
        Resolve(session: session, slot: 2)?.Item.Quantity.ShouldBe(expected: 5);
        for (int use = 0; use < 4; use++)
        {
            Spend(session: session, expected: Resolve(session: session, slot: 2)).ShouldBeTrue();
        }

        Merge(session: session, source: Find(session: session, id: first.Id),
                target: Find(session: session, id: second.Id)).ShouldBeTrue();
        Resolve(session: session, slot: 1).ShouldBeNull();
        (Resolve(session: session, slot: 2)?.Item.Id, Resolve(session: session, slot: 2)?.Item.Quantity)
                .ShouldBe(expected: (second.Id, 5));
        session.Inventory.Find(itemId: first.Id).ShouldBeNull();
    }

    private static FoundationSession WithDepth(string value)
    {
        return Configured(oldValue: "\"maximumLevels\": 3", newValue: "\"maximumLevels\": " + value,
                config: "content/config/inventory.json");
    }

    private static RunInventory.Item Equip(FoundationSession session, string definitionId, string slot)
    {
        RunInventory.Item item = Item(session: session, definitionId: definitionId);
        Move(session: session, source: item, target: Slot(slot: slot)).ShouldBeTrue();
        return Find(session: session, id: item.Id);
    }

    private static RunInventory.Item Store(FoundationSession session, string definitionId, RunInventory.Item container,
            int ordinal = 0, int x = 0, int y = 0)
    {
        RunInventory.Item item = Item(session: session, definitionId: definitionId, ordinal: ordinal);
        Move(session: session, source: item, target: In(session: session, item: item, container: container, x: x, y: y))
                .ShouldBeTrue();
        return Find(session: session, id: item.Id);
    }

    private static RunInventory.ContainerCell In(FoundationSession session, RunInventory.Item item,
            RunInventory.Item container, int x = 0, int y = 0)
    {
        return RunItemTransfer.Container(definition: session.Definitions.Items[item.DefinitionId],
                containerId: container.Id, x: x, y: y);
    }

    private static RunInventory.ItemPath Path(FoundationSession session, RunInventory.Item item)
    {
        return session.Inventory.PathTo(itemId: item.Id) ?? throw new InvalidOperationException(message: "Item path.");
    }

    private static bool MoveExpected(FoundationSession session, RunInventory.ItemPath expected,
            RunInventory.Location target)
    {
        return session.TryEditInventory(edit: (current, _, _) => RunItemTransfer.Move(inventory: current,
                expected: expected, destination: target));
    }

    private static bool Drop(FoundationSession session, RunInventory.ItemPath expected, string pileId)
    {
        return session.TryEditInventory(edit: (current, _, _) =>
                RunItemTransfer.Drop(inventory: current, expected: expected, pileId: pileId));
    }

    private static bool Assign(FoundationSession session, int slot, long? itemId)
    {
        return session.TryEditInventory(edit: (current, definitions, _) => RunQuickReferences.Assign(
                inventory: current, definitions: definitions, survivorId: Survivor, slot: slot, itemId: itemId));
    }

    private static RunInventory.ItemPath? Resolve(FoundationSession session, int slot)
    {
        return RunQuickReferences.Resolve(inventory: session.Inventory, definitions: session.Definitions,
                survivorId: Survivor, slot: slot);
    }

    private static bool Spend(FoundationSession session, RunInventory.ItemPath? expected)
    {
        return session.TryEditInventory(edit: (current, definitions, _) => RunQuickReferences.SpendCarriedFood(
                inventory: current, definitions: definitions, survivorId: Survivor, expected: expected));
    }
}
