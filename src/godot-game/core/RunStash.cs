using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Places exact item identities in real stash pages or existing carried storage.</summary>
internal static class RunStash
{
    internal static bool TryPageIndex(decimal pageNumber, int pageCount, out int index)
    {
        index = 0;
        if (pageNumber < 1 || pageNumber > pageCount || pageNumber != decimal.Truncate(d: pageNumber))
        {
            return false;
        }

        index = (int)pageNumber - 1;
        return true;
    }

    internal static RunInventory AddStartingStack(RunInventory inventory, RunDefinitions.Item definition, int quantity)
    {
        RunInventory.StashCell cell = FindStashCell(inventory: inventory, definition: definition)
                ?? throw new InvalidOperationException(message: "RUN_STARTING_STASH_FIT");
        var item = new RunInventory.Item(id: inventory.NextItemId, definitionId: definition.Id, quantity: quantity,
                condition: definition.MaximumCondition, location: cell, storageGrid: definition.Storage);
        return Store(inventory: inventory, items: inventory.Items.Add(item: item), cell: cell,
                nextItemId: checked(inventory.NextItemId + 1));
    }

    internal static RunInventory? Deposit(RunInventory inventory, RunDefinitions definitions,
            RunInventory.ItemPath expected)
    {
        RunInventory.Item? source = RunItemTransfer.Current(inventory: inventory, expected: expected);
        if (source is null || expected.Root is RunInventory.StashCell)
        {
            return null;
        }

        IEnumerable<RunInventory.Item> placements = inventory.Items
                .Where(predicate: item => item.Location is RunInventory.StashCell)
                .OrderBy(keySelector: item =>
                {
                    var cell = (RunInventory.StashCell)item.Location;
                    return (cell.Page, cell.Y, cell.X, item.Id);
                });
        RunInventory merged = MergeInto(inventory: inventory, definitions: definitions, source: source,
                targets: placements);
        RunInventory.Item? remainder = merged.Find(itemId: source.Id);
        if (remainder is null)
        {
            return merged;
        }

        RunInventory.StashCell? cell = FindStashCell(inventory: merged,
                definition: definitions.Items[source.DefinitionId]);
        return cell is null ? null : Store(inventory: merged, items: merged.Items.Replace(oldValue: remainder,
                newValue: remainder with { Location = cell }), cell: cell, nextItemId: merged.NextItemId);
    }

    internal static RunInventory? Withdraw(RunInventory inventory, RunDefinitions definitions,
            RunPopulation population, RunInventory.ItemPath expected, string survivorId)
    {
        RunInventory.Item? source = RunItemTransfer.Current(inventory: inventory, expected: expected);
        if (source is null || expected.Root is not RunInventory.StashCell)
        {
            return null;
        }

        RunInventory.Item[] carried = [.. inventory.CarriedBy(survivorId: survivorId)
                .OrderBy(keySelector: item => item.Id)];
        RunInventory merged = MergeInto(inventory: inventory, definitions: definitions, source: source,
                targets: carried.Where(predicate: item => item.Location is RunInventory.ContainerCell));
        RunInventory.ItemPath? remainder = merged.PathTo(itemId: source.Id);
        return remainder is null ? merged : PlaceInStorage(inventory: merged, definitions: definitions,
                population: population, expected: remainder, carried: carried);
    }

    internal static PageEffect? ChangedPages(RunItemTransfer.Change change)
    {
        (RunInventory.GridKind kind, long id, int x, int y)[] cells = [.. change.ChangedCells()
                .Where(predicate: cell => cell.kind == RunInventory.GridKind.Stash)];
        if (cells.Length == 0)
        {
            return null;
        }

        long first = cells[0].id;
        return new PageEffect(pageNumber: checked((int)first + 1),
                changedCells: [.. cells.TakeWhile(predicate: cell => cell.id == first)
                        .Select(selector: cell => (cell.x, cell.y))],
                morePages: cells.Select(selector: cell => cell.id).Distinct().Count() - 1);
    }

    private static RunInventory MergeInto(RunInventory inventory, RunDefinitions definitions,
            RunInventory.Item source, IEnumerable<RunInventory.Item> targets)
    {
        RunDefinitions.Item rule = definitions.Items[source.DefinitionId];
        if (source.StorageGrid.HasValue || rule.MaxStack <= 1)
        {
            return inventory;
        }

        foreach (RunInventory.Item target in targets)
        {
            if (target.Id == source.Id || target.StorageGrid.HasValue || target.Condition != source.Condition
                    || !StringComparer.Ordinal.Equals(x: target.DefinitionId, y: source.DefinitionId)
                    || target.Quantity >= rule.MaxStack)
            {
                continue;
            }

            RunInventory.ItemPath? remainder = inventory.PathTo(itemId: source.Id);
            if (remainder is null)
            {
                break;
            }

            inventory = RunItemTransfer.Merge(inventory: inventory, definitions: definitions,
                    expectedSource: remainder, expectedDestination: inventory.PathTo(itemId: target.Id))
                    ?? inventory;
        }

        return inventory;
    }

    private static RunInventory? PlaceInStorage(RunInventory inventory, RunDefinitions definitions,
            RunPopulation population, RunInventory.ItemPath expected, RunInventory.Item[] carried)
    {
        ILookup<long, RunInventory.ContainerCell> grids = inventory.Items
                .Select(selector: item => item.Location).OfType<RunInventory.ContainerCell>()
                .ToLookup(keySelector: cell => cell.ContainerId);
        RunDefinitions.Item definition = definitions.Items[expected.Item.DefinitionId];
        foreach (RunInventory.Item container in carried)
        {
            if (container.StorageGrid is not { } bounds)
            {
                continue;
            }

            var cell = (RunInventory.ContainerCell?)FindCell(bounds: bounds, occupied: grids[container.Id],
                    definition: definition, cellAt: (x, y, rotated) => RunItemTransfer.Container(
                            definition: definition, containerId: container.Id, x: x, y: y, rotated: rotated));
            RunInventory? moved = cell is null ? null : RunItemTransfer.Move(inventory: inventory,
                    expected: expected, destination: cell);
            if (moved is not null && RunInventoryRules.IsValid(inventory: moved, definitions: definitions,
                    population: population))
            {
                return moved;
            }
        }

        return null;
    }

    private static RunInventory.StashCell? FindStashCell(RunInventory inventory, RunDefinitions.Item definition)
    {
        ILookup<int, RunInventory.StashCell> pages = inventory.Items
                .Select(selector: item => item.Location).OfType<RunInventory.StashCell>()
                .ToLookup(keySelector: cell => cell.Page);
        for (int page = 0; page < inventory.StashPageCount; page++)
        {
            RunInventory.StashCell? cell = FindOnPage(bounds: inventory.StashGrid, occupied: pages[page],
                    definition: definition, page: page);
            if (cell is not null)
            {
                return cell;
            }
        }

        return inventory.StashPageCount < int.MaxValue ? FindOnPage(bounds: inventory.StashGrid, occupied: [],
                definition: definition, page: inventory.StashPageCount) : null;
    }

    private static RunInventory.StashCell? FindOnPage(RunDefinitions.Size bounds,
            IEnumerable<RunInventory.StashCell> occupied, RunDefinitions.Item definition, int page)
    {
        return (RunInventory.StashCell?)FindCell(bounds: bounds, occupied: occupied, definition: definition,
                cellAt: (x, y, rotated) => RunItemTransfer.Stash(definition: definition, page: page,
                        x: x, y: y, rotated: rotated));
    }

    private static RunInventory.GridCell? FindCell(RunDefinitions.Size bounds,
            IEnumerable<RunInventory.GridCell> occupied, RunDefinitions.Item definition,
            Func<int, int, bool, RunInventory.GridCell> cellAt)
    {
        bool tryRotation = definition.Rotatable && definition.Footprint.Width != definition.Footprint.Height;
        for (int y = 0; y < bounds.Height; y++)
        {
            for (int x = 0; x < bounds.Width; x++)
            {
                RunInventory.GridCell cell = cellAt(arg1: x, arg2: y, arg3: false);
                if (Fits(cell: cell, bounds: bounds, occupied: occupied))
                {
                    return cell;
                }

                if (tryRotation)
                {
                    RunInventory.GridCell rotated = cellAt(arg1: x, arg2: y, arg3: true);
                    if (Fits(cell: rotated, bounds: bounds, occupied: occupied))
                    {
                        return rotated;
                    }
                }
            }
        }

        return null;
    }

    private static bool Fits(RunInventory.GridCell cell, RunDefinitions.Size bounds,
            IEnumerable<RunInventory.GridCell> occupied)
    {
        return RunInventoryRules.InBounds(cell: cell, bounds: bounds) && !occupied.Any(predicate: other =>
                RunItemTransfer.Overlaps(first: cell, second: other));
    }

    private static RunInventory Store(RunInventory inventory, ImmutableArray<RunInventory.Item> items,
            RunInventory.StashCell cell, long nextItemId)
    {
        return new RunInventory(items: items, stashGrid: inventory.StashGrid,
                stashPageCount: Math.Max(val1: inventory.StashPageCount, val2: checked(cell.Page + 1)),
                nextItemId: nextItemId, quickReferences: inventory.QuickReferences, savedLayout: inventory.SavedLayout);
    }

    internal sealed class PageEffect
    {
        internal PageEffect(int pageNumber, ImmutableArray<(int x, int y)> changedCells, int morePages)
        {
            PageNumber = pageNumber;
            ChangedCells = changedCells;
            MorePages = morePages;
        }

        internal int PageNumber { get; }
        internal ImmutableArray<(int x, int y)> ChangedCells { get; }
        internal int MorePages { get; }
    }
}
