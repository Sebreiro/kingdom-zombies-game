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
        return population.CreateInitialInventory(definitions: definitions);
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

    internal RunInventory MoveToCorpse(string survivorId, string corpseId)
    {
        ImmutableArray<Item> items = [.. Items.Select(selector: item =>
                item.Location is Equipment equipment && StringComparer.Ordinal.Equals(
                        x: equipment.SurvivorId, y: survivorId)
                        ? item with { Location = new CorpseEquipment(corpseId: corpseId, slot: equipment.Slot) }
                        : item)];
        return Enumerable.SequenceEqual(first: items, second: Items) ? this : WithItems(items: items);
    }

    internal bool HasLoot(string id)
    {
        return Items.Any(predicate: item => item.Location switch
        {
            CorpseEquipment corpse => StringComparer.Ordinal.Equals(x: corpse.CorpseId, y: id),
            Ground ground => StringComparer.Ordinal.Equals(x: ground.PileId, y: id),
            _ => false,
        });
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

    internal sealed record CorpseEquipment : Location
    {
        internal CorpseEquipment(string corpseId, string slot)
        {
            CorpseId = corpseId;
            Slot = slot;
        }

        internal string CorpseId { get; }
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
