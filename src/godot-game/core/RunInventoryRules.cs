using System;
using System.Collections.Generic;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Validates a complete ownership and placement candidate against the current run.</summary>
internal static class RunInventoryRules
{
    internal static bool IsValid(RunInventory inventory, RunDefinitions definitions, RunPopulation population)
    {
        if (inventory.Items.IsDefault || !RunSavedLayout.ValidSize(size: inventory.StashGrid)
                || inventory.SavedLayout is null && inventory.StashGrid != definitions.StashGrid
                || inventory.StashPageCount <= 0 || inventory.NextItemId <= 0)
        {
            return false;
        }

        var items = new Dictionary<long, RunInventory.Item>();
        foreach (RunInventory.Item item in inventory.Items)
        {
            if (item.Id <= 0 || item.Id >= inventory.NextItemId || !items.TryAdd(key: item.Id, value: item))
            {
                return false;
            }
        }

        var equipment = new Dictionary<RunInventory.Equipment, string>();
        var corpses = new Dictionary<RunInventory.Equipment, string>();
        var occupied = new HashSet<(RunInventory.GridKind kind, long id, int x, int y)>();
        foreach (RunInventory.Item item in inventory.Items)
        {
            if (!definitions.Items.TryGetValue(key: item.DefinitionId, value: out RunDefinitions.Item? current))
            {
                return false;
            }

            RunDefinitions.Item rule = inventory.SavedLayout?.RuleFor(item: item, current: current) ?? current;
            if (!MatchesDefinition(item: item, definition: rule))
            {
                return false;
            }

            bool valid = item.Location switch
            {
                RunInventory.Equipment slot => SlotAccepts(location: slot, definition: rule, population: population)
                        && equipment.TryAdd(key: slot, value: rule.Id),
                RunInventory.CorpseEquipment corpse => CorpseSlotAccepts(location: corpse, definition: rule)
                        && corpses.TryAdd(key: new RunInventory.Equipment(survivorId: corpse.CorpseId,
                                slot: corpse.Slot), value: rule.Id),
                RunInventory.GridCell cell => AddGridPlacement(cell: cell, definition: rule,
                        bounds: GridBounds(inventory: inventory, cell: cell, items: items), occupied: occupied),
                RunInventory.Ground ground => IsPileId(pileId: ground.PileId),
                _ => false,
            };
            if (!valid)
            {
                return false;
            }
        }

        return ValidHandPairs(equipment: equipment) && ValidHandPairs(equipment: corpses)
                && ValidContainerDepth(items: items, maximum: inventory.SavedLayout?.MaximumContainerLevels
                        ?? definitions.MaximumContainerLevels);
    }

    internal static RunDefinitions.Size Footprint(RunDefinitions.Item definition, bool rotated)
    {
        return rotated
                ? new RunDefinitions.Size(width: definition.Footprint.Height, height: definition.Footprint.Width)
                : definition.Footprint;
    }

    internal static bool InBounds(RunInventory.GridCell cell, RunDefinitions.Size bounds)
    {
        return cell.X >= 0 && cell.Y >= 0 && cell.X <= bounds.Width - cell.Footprint.Width
                && cell.Y <= bounds.Height - cell.Footprint.Height;
    }

    internal static bool KeepsIdentities(RunInventory current, RunInventory candidate)
    {
        if (candidate.Items.IsDefault || candidate.NextItemId < current.NextItemId)
        {
            return false;
        }

        var known = current.Items.ToDictionary(keySelector: item => item.Id);
        foreach (RunInventory.Item item in candidate.Items)
        {
            if (item.Id < current.NextItemId && (!known.TryGetValue(key: item.Id, value: out RunInventory.Item? old)
                    || !StringComparer.Ordinal.Equals(x: old.DefinitionId, y: item.DefinitionId)
                    || old.StorageGrid != item.StorageGrid))
            {
                return false;
            }
        }

        return true;
    }

    internal static bool SlotAccepts(RunInventory.Equipment location, RunDefinitions.Item definition,
            RunPopulation population)
    {
        return AcceptsDefinition(slot: location.Slot, definition: definition)
                && population.Survivors.Any(predicate: survivor => survivor.Health > 0
                        && StringComparer.Ordinal.Equals(x: survivor.Id, y: location.SurvivorId));
    }

    internal static bool CorpseSlotAccepts(RunInventory.CorpseEquipment location, RunDefinitions.Item definition)
    {
        return IsPileId(pileId: location.CorpseId) && AcceptsDefinition(slot: location.Slot, definition: definition);
    }

    private static bool AcceptsDefinition(string slot, RunDefinitions.Item definition)
    {
        return RunDefinitions.EquipmentSlots.Contains(value: slot, comparer: StringComparer.Ordinal)
                && (StringComparer.Ordinal.Equals(x: definition.Slot, y: slot)
                        || definition.Slot is "either-logical-hand" && slot is "left-hand" or "right-hand");
    }

    private static bool MatchesDefinition(RunInventory.Item item, RunDefinitions.Item definition)
    {
        return item.Quantity > 0 && item.Quantity <= definition.MaxStack && item.StorageGrid == definition.Storage
                && (definition.MaximumCondition is { } maximum
                        ? item.Condition >= 0 && item.Condition <= maximum : item.Condition is null);
    }

    private static RunDefinitions.Size? GridBounds(RunInventory inventory, RunInventory.GridCell cell,
            Dictionary<long, RunInventory.Item> items)
    {
        return cell switch
        {
            RunInventory.StashCell stash when stash.Page >= 0 && stash.Page < inventory.StashPageCount =>
                    inventory.StashGrid,
            RunInventory.ContainerCell container => items.GetValueOrDefault(key: container.ContainerId)?.StorageGrid,
            _ => null,
        };
    }

    private static bool AddGridPlacement(RunInventory.GridCell cell, RunDefinitions.Item definition,
            RunDefinitions.Size? bounds, HashSet<(RunInventory.GridKind kind, long id, int x, int y)> occupied)
    {
        RunDefinitions.Size size = cell.Footprint;
        if (bounds is not { } grid || cell.Rotated && !definition.Rotatable
                || size != Footprint(definition: definition, rotated: cell.Rotated)
                || !InBounds(cell: cell, bounds: grid))
        {
            return false;
        }

        (RunInventory.GridKind kind, long id) = cell.Grid;
        for (int y = cell.Y; y < cell.Y + size.Height; y++)
        {
            for (int x = cell.X; x < cell.X + size.Width; x++)
            {
                if (!occupied.Add(item: (kind, id, x, y)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool IsPileId(string pileId)
    {
        if (string.IsNullOrWhiteSpace(value: pileId))
        {
            return false;
        }

        try
        {
            _ = new StableIdentity(value: pileId);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static bool ValidContainerDepth(Dictionary<long, RunInventory.Item> items, int maximum)
    {
        var depths = new Dictionary<long, int>();
        foreach (RunInventory.Item item in items.Values)
        {
            if (item.StorageGrid.HasValue
                    && !SaveDepth(item: item, items: items, depths: depths, maximum: maximum))
            {
                return false;
            }
        }

        return true;
    }

    private static bool SaveDepth(RunInventory.Item item, Dictionary<long, RunInventory.Item> items,
            Dictionary<long, int> depths, int maximum)
    {
        var pending = new List<long>();
        var visited = new HashSet<long>();
        RunInventory.Item current = item;
        while (!depths.ContainsKey(key: current.Id) && current.Location is RunInventory.ContainerCell cell)
        {
            if (!visited.Add(item: current.Id) || pending.Count >= maximum)
            {
                return false;
            }

            pending.Add(item: current.Id);
            current = items[cell.ContainerId];
        }

        if (!depths.TryGetValue(key: current.Id, value: out int depth))
        {
            depth = 1;
            depths.Add(key: current.Id, value: depth);
        }

        if (pending.Count > maximum - depth)
        {
            return false;
        }

        for (int index = pending.Count - 1; index >= 0; index--)
        {
            depths.Add(key: pending[index], value: ++depth);
        }

        return true;
    }

    private static bool ValidHandPairs(Dictionary<RunInventory.Equipment, string> equipment)
    {
        foreach ((RunInventory.Equipment location, string definition) in equipment)
        {
            if (definition is "IT-07")
            {
                string opposite = location.Slot is "left-hand" ? "right-hand" : "left-hand";
                var oppositeHand = new RunInventory.Equipment(survivorId: location.SurvivorId, slot: opposite);
                if (equipment.TryGetValue(key: oppositeHand, value: out string? held) && held is not "IT-02")
                {
                    return false;
                }
            }
        }

        return true;
    }
}
