using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

    internal static RunInventory.ContainerCell Container(RunDefinitions.Item definition, long containerId,
            int x, int y, bool rotated = false)
    {
        return new RunInventory.ContainerCell(containerId: containerId, x: x, y: y, rotated: rotated,
                footprint: RunInventoryRules.Footprint(definition: definition, rotated: rotated));
    }

    internal static RunInventory? Move(RunInventory inventory, RunInventory.ItemPath? expected,
            RunInventory.Location destination)
    {
        return destination is RunInventory.Ground ? null
                : MoveTo(inventory: inventory, expected: expected, destination: destination);
    }

    internal static RunInventory? Drop(RunInventory inventory, RunInventory.ItemPath? expected, string pileId)
    {
        return MoveTo(inventory: inventory, expected: expected, destination: new RunInventory.Ground(pileId: pileId));
    }

    internal static RunInventory? Split(RunInventory inventory, RunInventory.ItemPath? expected, decimal quantity,
            RunInventory.Location destination)
    {
        RunInventory.Item? source = Current(inventory: inventory, expected: expected);
        if (source is null || destination is RunInventory.Ground || quantity <= 0 || quantity >= source.Quantity
                || quantity != decimal.Truncate(d: quantity) || inventory.NextItemId == long.MaxValue)
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
            RunInventory.ItemPath? expectedSource, RunInventory.ItemPath? expectedDestination)
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

    internal static RunInventory? RemoveOne(RunInventory inventory, RunInventory.ItemPath? expected)
    {
        RunInventory.Item? source = Current(inventory: inventory, expected: expected);
        if (source is null)
        {
            return null;
        }

        return inventory.WithItems(items: source.Quantity == 1 ? inventory.Items.Remove(item: source)
                : inventory.Items.Replace(oldValue: source, newValue: source with { Quantity = source.Quantity - 1 }));
    }

    internal static RunItemActions.Status CheckPlacement(RunInventory inventory, RunDefinitions.Item definition,
            RunPopulation population, RunInventory.Item source, RunInventory.Location destination, bool split)
    {
        if (destination is RunInventory.Equipment equipment)
        {
            if (!RunInventoryRules.SlotAccepts(location: equipment, definition: definition, population: population))
            {
                return RunItemActions.Status.Incompatible;
            }

            return inventory.Equipped(survivorId: equipment.SurvivorId, slot: equipment.Slot) is not null
                    ? RunItemActions.Status.Capacity : RunItemActions.Status.Ready;
        }

        if (destination is not RunInventory.GridCell cell || cell.X < 0 || cell.Y < 0
                || cell.Rotated && !definition.Rotatable || cell.Footprint != RunInventoryRules.Footprint(
                        definition: definition, rotated: cell.Rotated))
        {
            return RunItemActions.Status.Incompatible;
        }

        RunDefinitions.Size? bounds = cell switch
        {
            RunInventory.StashCell stash when stash.Page >= 0 && stash.Page < inventory.StashPageCount =>
                    inventory.StashGrid,
            RunInventory.ContainerCell container => inventory.Find(itemId: container.ContainerId)?.StorageGrid,
            _ => null,
        };
        if (bounds is not { } grid)
        {
            return RunItemActions.Status.Incompatible;
        }

        return !RunInventoryRules.InBounds(cell: cell, bounds: grid)
                || inventory.Items.Any(predicate: item => (split || item.Id != source.Id)
                        && item.Location is RunInventory.GridCell occupied && Overlaps(first: cell, second: occupied))
                ? RunItemActions.Status.Capacity : RunItemActions.Status.Ready;
    }

    internal static bool Overlaps(RunInventory.GridCell first, RunInventory.GridCell second)
    {
        return first.Grid == second.Grid && first.X < second.X + second.Footprint.Width
                && second.X < first.X + first.Footprint.Width && first.Y < second.Y + second.Footprint.Height
                && second.Y < first.Y + first.Footprint.Height;
    }

    internal static RunInventory.Item? Current(RunInventory inventory, RunInventory.ItemPath? expected)
    {
        if (expected is null || expected.Items.IsDefaultOrEmpty)
        {
            return null;
        }

        RunInventory.ItemPath? actual = inventory.PathTo(itemId: expected.Item.Id);
        return actual is not null && Enumerable.SequenceEqual(first: actual.Items, second: expected.Items)
                ? actual.Item : null;
    }

    private static RunInventory? MoveTo(RunInventory inventory, RunInventory.ItemPath? expected,
            RunInventory.Location destination)
    {
        RunInventory.Item? source = Current(inventory: inventory, expected: expected);
        return source is null || source.Location == destination ? null
                : inventory.WithItems(items: inventory.Items.Replace(oldValue: source,
                        newValue: source with { Location = destination }));
    }

    internal sealed class Change
    {
        internal Change(RunInventory before, RunInventory after)
        {
            Before = before;
            After = after;
        }

        internal RunInventory Before { get; }
        internal RunInventory After { get; }

        internal ImmutableArray<(RunInventory.GridKind kind, long id, int x, int y)> ChangedCells()
        {
            var cells = new HashSet<(RunInventory.GridKind kind, long id, int x, int y)>();
            var live = After.Items.ToDictionary(keySelector: item => item.Id);
            foreach (RunInventory.Item previous in Before.Items)
            {
                _ = live.Remove(key: previous.Id, value: out RunInventory.Item? next);
                if (previous == next)
                {
                    continue;
                }

                AddCells(location: previous.Location, cells: cells);
                AddCells(location: next?.Location, cells: cells);
            }

            foreach (RunInventory.Item added in live.Values)
            {
                AddCells(location: added.Location, cells: cells);
            }

            return [.. cells.OrderBy(keySelector: cell => (cell.kind, cell.id, cell.y, cell.x))];
        }

        private static void AddCells(RunInventory.Location? location,
                HashSet<(RunInventory.GridKind kind, long id, int x, int y)> cells)
        {
            if (location is not RunInventory.GridCell cell)
            {
                return;
            }

            (RunInventory.GridKind kind, long id) = cell.Grid;
            for (int y = cell.Y; y < cell.Y + cell.Footprint.Height; y++)
            {
                for (int x = cell.X; x < cell.X + cell.Footprint.Width; x++)
                {
                    cells.Add(item: (kind, id, x, y));
                }
            }
        }
    }
}
