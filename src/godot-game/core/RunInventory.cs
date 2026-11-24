using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Owns each item instance and its one location within the run.</summary>
internal sealed class RunInventory
{
    internal RunInventory(ImmutableArray<Item> items, int stashPageCount, long nextItemId)
    {
        Items = items;
        StashPageCount = stashPageCount;
        NextItemId = nextItemId;
    }

    internal ImmutableArray<Item> Items { get; }
    internal int StashPageCount { get; }
    internal long NextItemId { get; }

    internal static RunInventory Create(RunDefinitions definitions, RunPopulation population)
    {
        ImmutableArray<Item>.Builder items = ImmutableArray.CreateBuilder<Item>();
        foreach (RunPopulation.Survivor survivor in population.Survivors)
        {
            foreach (string id in definitions.StartingOutfit)
            {
                RunDefinitions.Item definition = definitions.Items[id];
                string slot = definition.Slot ?? throw new InvalidOperationException(message: "RUN_OUTFIT_SLOT");
                items.Add(item: new Item(id: items.Count + 1L, definitionId: id, quantity: 1, condition: null,
                        location: new Equipment(survivorId: survivor.Id, slot: slot)));
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
                        quantity: stack.Quantity, condition: definition.MaximumCondition, location: cell));
            }
        }

        return new RunInventory(items: items.ToImmutable(), stashPageCount: pages.Count, nextItemId: items.Count + 1L);
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
        internal Item(long id, string definitionId, int quantity, decimal? condition, Location location)
        {
            Id = id;
            DefinitionId = definitionId;
            Quantity = quantity;
            Condition = condition;
            Location = location;
        }

        internal long Id { get; }
        internal string DefinitionId { get; }
        internal int Quantity { get; }
        internal decimal? Condition { get; }
        internal Location Location { get; }
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
    internal sealed record StashCell : Location
    {
        internal StashCell(int page, int x, int y, bool rotated, RunDefinitions.Size footprint)
        {
            Page = page;
            X = x;
            Y = y;
            Rotated = rotated;
            Footprint = footprint;
        }

        internal int Page { get; }
        internal int X { get; }
        internal int Y { get; }
        internal bool Rotated { get; }
        internal RunDefinitions.Size Footprint { get; }
    }
}
