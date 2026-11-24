using System;
using System.Collections.Generic;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Validates a complete ownership and placement candidate against the current run.</summary>
internal static class RunInventoryRules
{
    internal static bool IsValid(RunInventory inventory, RunDefinitions definitions, RunPopulation population)
    {
        if (inventory.Items.IsDefault || inventory.StashGrid != definitions.StashGrid
                || inventory.StashPageCount <= 0 || inventory.NextItemId <= 0)
        {
            return false;
        }

        var ids = new HashSet<long>();
        var equipment = new Dictionary<RunInventory.Equipment, string>();
        var occupied = new HashSet<(int page, int x, int y)>();
        foreach (RunInventory.Item item in inventory.Items)
        {
            if (item.Id <= 0 || item.Id >= inventory.NextItemId || !ids.Add(item: item.Id)
                    || !definitions.Items.TryGetValue(key: item.DefinitionId, value: out RunDefinitions.Item? rule)
                    || !MatchesDefinition(item: item, definition: rule))
            {
                return false;
            }

            bool valid = item.Location switch
            {
                RunInventory.Equipment slot => SlotAccepts(location: slot, definition: rule, population: population)
                        && equipment.TryAdd(key: slot, value: rule.Id),
                RunInventory.StashCell cell => AddStashPlacement(inventory: inventory, cell: cell,
                        definition: rule, occupied: occupied),
                _ => false,
            };
            if (!valid)
            {
                return false;
            }
        }

        return ValidHandPairs(equipment: equipment);
    }

    internal static RunDefinitions.Size Footprint(RunDefinitions.Item definition, bool rotated)
    {
        return rotated
                ? new RunDefinitions.Size(width: definition.Footprint.Height, height: definition.Footprint.Width)
                : definition.Footprint;
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

    private static bool MatchesDefinition(RunInventory.Item item, RunDefinitions.Item definition)
    {
        return item.Quantity > 0 && item.Quantity <= definition.MaxStack && item.StorageGrid == definition.Storage
                && (definition.MaximumCondition is { } maximum
                        ? item.Condition >= 0 && item.Condition <= maximum : item.Condition is null);
    }

    private static bool SlotAccepts(RunInventory.Equipment location, RunDefinitions.Item definition,
            RunPopulation population)
    {
        return RunDefinitions.EquipmentSlots.Contains(value: location.Slot, comparer: StringComparer.Ordinal)
                && (StringComparer.Ordinal.Equals(x: definition.Slot, y: location.Slot)
                        || definition.Slot is "either-logical-hand" && location.Slot is "left-hand" or "right-hand")
                && population.Survivors.Any(predicate: survivor =>
                        StringComparer.Ordinal.Equals(x: survivor.Id, y: location.SurvivorId));
    }

    private static bool AddStashPlacement(RunInventory inventory, RunInventory.StashCell cell,
            RunDefinitions.Item definition, HashSet<(int page, int x, int y)> occupied)
    {
        RunDefinitions.Size size = cell.Footprint;
        if (cell.Page < 0 || cell.Page >= inventory.StashPageCount || cell.X < 0 || cell.Y < 0
                || cell.Rotated && !definition.Rotatable || size != Footprint(definition: definition,
                        rotated: cell.Rotated) || cell.X > inventory.StashGrid.Width - size.Width
                || cell.Y > inventory.StashGrid.Height - size.Height)
        {
            return false;
        }

        for (int y = cell.Y; y < cell.Y + size.Height; y++)
        {
            for (int x = cell.X; x < cell.X + size.Width; x++)
            {
                if (!occupied.Add(item: (cell.Page, x, y)))
                {
                    return false;
                }
            }
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
