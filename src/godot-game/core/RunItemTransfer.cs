using System;
using System.Collections.Immutable;

namespace KingdomZombies.Core;

/// <summary>Proposes one detached item edit from exact current source and destination state.</summary>
internal static class RunItemTransfer
{
    internal static RunInventory.StashCell Stash(RunDefinitions.Item definition, int page, int x, int y,
            bool rotated = false)
    {
        return new RunInventory.StashCell(page: page, x: x, y: y, rotated: rotated,
                footprint: RunInventoryRules.Footprint(definition: definition, rotated: rotated));
    }

    internal static RunInventory? Move(RunInventory inventory, RunInventory.Item expected,
            RunInventory.Location destination)
    {
        RunInventory.Item? source = Current(inventory: inventory, expected: expected);
        return source is null || source.Location == destination ? null
                : inventory.WithItems(items: inventory.Items.Replace(oldValue: source,
                        newValue: source with { Location = destination }));
    }

    internal static RunInventory? Split(RunInventory inventory, RunInventory.Item expected, decimal quantity,
            RunInventory.Location destination)
    {
        RunInventory.Item? source = Current(inventory: inventory, expected: expected);
        if (source is null || quantity <= 0 || quantity >= source.Quantity || quantity != decimal.Truncate(d: quantity)
                || inventory.NextItemId == long.MaxValue)
        {
            return null;
        }

        int count = (int)quantity;
        var extracted = new RunInventory.Item(id: inventory.NextItemId, definitionId: source.DefinitionId,
                quantity: count, condition: source.Condition, location: destination, storageGrid: source.StorageGrid);
        ImmutableArray<RunInventory.Item> items = inventory.Items.Replace(oldValue: source,
                newValue: source with { Quantity = source.Quantity - count });
        return inventory.WithItems(items: items.Add(item: extracted), nextItemId: inventory.NextItemId + 1);
    }

    internal static RunInventory? Merge(RunInventory inventory, RunDefinitions definitions,
            RunInventory.Item expectedSource, RunInventory.Item expectedDestination)
    {
        RunInventory.Item? source = Current(inventory: inventory, expected: expectedSource);
        RunInventory.Item? destination = Current(inventory: inventory, expected: expectedDestination);
        if (source is null || destination is null || source.Id == destination.Id
                || !StringComparer.Ordinal.Equals(x: source.DefinitionId, y: destination.DefinitionId)
                || source.Condition != destination.Condition)
        {
            return null;
        }

        int transferred = Math.Min(val1: source.Quantity,
                val2: definitions.Items[source.DefinitionId].MaxStack - destination.Quantity);
        if (transferred <= 0)
        {
            return null;
        }

        ImmutableArray<RunInventory.Item> items = inventory.Items.Replace(oldValue: destination,
                newValue: destination with { Quantity = destination.Quantity + transferred });
        return inventory.WithItems(items: transferred == source.Quantity ? items.Remove(item: source)
                : items.Replace(oldValue: source, newValue: source with { Quantity = source.Quantity - transferred }));
    }

    private static RunInventory.Item? Current(RunInventory inventory, RunInventory.Item expected)
    {
        RunInventory.Item? actual = inventory.Find(itemId: expected.Id);
        return actual == expected ? actual : null;
    }
}
