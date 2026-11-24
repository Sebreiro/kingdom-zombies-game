using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Owns each item instance and its one location within the run.</summary>
internal sealed class RunInventory
{
    internal RunInventory(ImmutableArray<Item> items, RunDefinitions.Size stashGrid, int stashPageCount,
            long nextItemId, ImmutableDictionary<(string survivor, int slot), long>? quickReferences = null)
    {
        Items = items;
        StashGrid = stashGrid;
        StashPageCount = stashPageCount;
        NextItemId = nextItemId;
        QuickReferences = quickReferences ?? [];
    }

    internal ImmutableArray<Item> Items { get; }
    internal RunDefinitions.Size StashGrid { get; }
    internal int StashPageCount { get; }
    internal long NextItemId { get; }
    internal ImmutableDictionary<(string survivor, int slot), long> QuickReferences { get; }

    internal static RunInventory Create(RunDefinitions definitions, RunPopulation population)
    {
        ImmutableArray<Item>.Builder items = ImmutableArray.CreateBuilder<Item>();
        foreach (RunPopulation.Survivor survivor in population.Survivors)
        {
            foreach (string id in definitions.StartingOutfit)
            {
                RunDefinitions.Item definition = definitions.Items[id];
                string slot = definition.Slot ?? throw new InvalidOperationException(message: "RUN_OUTFIT_SLOT");
                items.Add(item: new Item(id: items.Count + 1L, definitionId: id, quantity: 1,
                        condition: definition.MaximumCondition,
                        location: new Equipment(survivorId: survivor.Id, slot: slot),
                        storageGrid: definition.Storage));
            }
        }

        var pages = new List<bool[]> { new bool[definitions.StashGrid.Width * definitions.StashGrid.Height] };
        foreach (RunDefinitions.StartingStack stack in definitions.StartingStash)
        {
            RunDefinitions.Item definition = definitions.Items[stack.ItemId];
            for (int index = 0; index < stack.Count; index++)
            {
                StashCell cell = Place(pages: pages, grid: definitions.StashGrid, item: definition);
                items.Add(item: new Item(id: items.Count + 1L, definitionId: stack.ItemId,
                        quantity: stack.Quantity, condition: definition.MaximumCondition, location: cell,
                        storageGrid: definition.Storage));
            }
        }

        return new RunInventory(items: items.ToImmutable(), stashGrid: definitions.StashGrid,
                stashPageCount: pages.Count, nextItemId: items.Count + 1L);
    }

    internal Item? Find(long itemId)
    {
        return Items.FirstOrDefault(predicate: item => item.Id == itemId);
    }

    internal Item? Equipped(string survivorId, string slot)
    {
        var location = new Equipment(survivorId: survivorId, slot: slot);
        return Items.FirstOrDefault(predicate: item => item.Location == location);
    }

    internal ItemPath? PathTo(long itemId)
    {
        Item? item = Find(itemId: itemId);
        return item is null ? null : Trace(item: item, lookup: Find);
    }

    internal ImmutableArray<Item> CarriedBy(string survivorId)
    {
        var itemsById = Items.ToDictionary(keySelector: item => item.Id);
        return [.. Items.Where(predicate: item => Trace(item: item,
                lookup: id => itemsById.GetValueOrDefault(key: id))?.IsCarriedBy(survivorId: survivorId) == true)];
    }

    internal RunInventory WithItems(ImmutableArray<Item> items, long? nextItemId = null)
    {
        return new RunInventory(items: items, stashGrid: StashGrid, stashPageCount: StashPageCount,
                nextItemId: nextItemId ?? NextItemId, quickReferences: QuickReferences);
    }

    internal RunInventory WithReferences(ImmutableDictionary<(string survivor, int slot), long> references)
    {
        return new RunInventory(items: Items, stashGrid: StashGrid, stashPageCount: StashPageCount,
                nextItemId: NextItemId, quickReferences: references);
    }

    private static ItemPath? Trace(Item item, Func<long, Item?> lookup)
    {
        ImmutableArray<Item>.Builder ancestors = ImmutableArray.CreateBuilder<Item>();
        var seen = new HashSet<long>();
        Item? current = item;
        while (current is not null && seen.Add(item: current.Id))
        {
            ancestors.Add(item: current);
            if (current.Location is not ContainerCell cell)
            {
                return new ItemPath(items: ancestors.ToImmutable());
            }

            current = lookup(arg: cell.ContainerId);
        }

        return null;
    }

    private static StashCell Place(List<bool[]> pages, RunDefinitions.Size grid, RunDefinitions.Item item)
    {
        StashCell? cell = FindCell(pages: pages, grid: grid, size: item.Footprint, rotated: false);
        if (cell is null && item.Rotatable && item.Footprint.Width != item.Footprint.Height)
        {
            cell = FindCell(pages: pages, grid: grid,
                    size: new RunDefinitions.Size(width: item.Footprint.Height, height: item.Footprint.Width),
                    rotated: true);
        }

        if (cell is null)
        {
            bool rotated = item.Footprint.Width > grid.Width || item.Footprint.Height > grid.Height;
            RunDefinitions.Size footprint = rotated
                    ? new RunDefinitions.Size(width: item.Footprint.Height, height: item.Footprint.Width)
                    : item.Footprint;
            cell = new StashCell(page: pages.Count, x: 0, y: 0, rotated: rotated, footprint: footprint);
            pages.Add(item: new bool[grid.Width * grid.Height]);
        }

        bool[] occupied = pages[cell.Page];
        for (int row = cell.Y; row < cell.Y + cell.Footprint.Height; row++)
        {
            for (int column = cell.X; column < cell.X + cell.Footprint.Width; column++)
            {
                occupied[row * grid.Width + column] = true;
            }
        }

        return cell;
    }

    private static StashCell? FindCell(List<bool[]> pages, RunDefinitions.Size grid, RunDefinitions.Size size,
            bool rotated)
    {
        for (int page = 0; page < pages.Count; page++)
        {
            for (int y = 0; y <= grid.Height - size.Height; y++)
            {
                for (int x = 0; x <= grid.Width - size.Width; x++)
                {
                    if (Fits(occupied: pages[page], width: grid.Width, x: x, y: y, size: size))
                    {
                        return new StashCell(page: page, x: x, y: y, rotated: rotated, footprint: size);
                    }
                }
            }
        }

        return null;
    }

    private static bool Fits(bool[] occupied, int width, int x, int y, RunDefinitions.Size size)
    {
        return Enumerable.Range(start: y, count: size.Height).All(predicate: row =>
                Enumerable.Range(start: x, count: size.Width)
                        .All(predicate: column => !occupied[row * width + column]));
    }

    internal sealed record Item
    {
        internal Item(long id, string definitionId, int quantity, decimal? condition, Location location,
                RunDefinitions.Size? storageGrid)
        {
            Id = id;
            DefinitionId = definitionId;
            Quantity = quantity;
            Condition = condition;
            Location = location;
            StorageGrid = storageGrid;
        }

        internal long Id { get; }
        internal string DefinitionId { get; }
        internal int Quantity { get; init; }
        internal decimal? Condition { get; init; }
        internal Location Location { get; init; }
        internal RunDefinitions.Size? StorageGrid { get; }
    }
    internal sealed class ItemPath
    {
        internal ItemPath(ImmutableArray<Item> items)
        {
            Items = items;
        }

        internal ImmutableArray<Item> Items { get; }
        internal Item Item => Items[0];
        internal Location Root => Items[^1].Location;

        internal bool IsCarriedBy(string survivorId)
        {
            return Root is Equipment equipment
                    && StringComparer.Ordinal.Equals(x: equipment.SurvivorId, y: survivorId);
        }
    }

    internal abstract record Location;
    internal sealed record Equipment : Location
    {
        internal Equipment(string survivorId, string slot)
        {
            SurvivorId = survivorId;
            Slot = slot;
        }

        internal string SurvivorId { get; }
        internal string Slot { get; }
    }
    internal enum GridKind
    {
        Stash,
        Container,
    }

    internal abstract record GridCell : Location
    {
        internal int X { get; }
        internal int Y { get; }
        internal bool Rotated { get; }
        internal RunDefinitions.Size Footprint { get; }
        internal abstract (GridKind kind, long id) Grid { get; }

        protected GridCell(int x, int y, bool rotated, RunDefinitions.Size footprint)
        {
            X = x;
            Y = y;
            Rotated = rotated;
            Footprint = footprint;
        }
    }

    internal sealed record StashCell : GridCell
    {
        internal StashCell(int page, int x, int y, bool rotated, RunDefinitions.Size footprint)
                : base(x: x, y: y, rotated: rotated, footprint: footprint)
        {
            Page = page;
        }

        internal int Page { get; }
        internal override (GridKind kind, long id) Grid => (GridKind.Stash, Page);
    }

    internal sealed record ContainerCell : GridCell
    {
        internal ContainerCell(long containerId, int x, int y, bool rotated, RunDefinitions.Size footprint)
                : base(x: x, y: y, rotated: rotated, footprint: footprint)
        {
            ContainerId = containerId;
        }

        internal long ContainerId { get; }
        internal override (GridKind kind, long id) Grid => (GridKind.Container, ContainerId);
    }

    internal sealed record Ground : Location
    {
        internal Ground(string pileId)
        {
            PileId = pileId;
        }

        internal string PileId { get; }
    }
}
