using System.Collections.Immutable;

namespace KingdomZombies.Core;

/// <summary>Assigns and invalidates exact references to a survivor's carried food.</summary>
internal static class RunQuickReferences
{
    internal static RunInventory? Assign(RunInventory inventory, RunDefinitions definitions, string survivorId,
            int slot, long? itemId)
    {
        if (slot < 1 || slot > definitions.QuickSlotCount || (itemId is { } id
                && FoodPath(inventory: inventory, definitions: definitions, survivorId: survivorId, itemId: id)
                        is null))
        {
            return null;
        }

        ImmutableDictionary<(string survivor, int slot), long> references = inventory.QuickReferences;
        (string, int) key = (survivorId, slot);
        ImmutableDictionary<(string survivor, int slot), long> updated = itemId is { } target
                ? references.SetItem(key: key, value: target) : references.Remove(key: key);
        return ReferenceEquals(objA: references, objB: updated) ? null : inventory.WithReferences(references: updated);
    }

    internal static RunInventory.ItemPath? Resolve(RunInventory inventory, RunDefinitions definitions,
            string survivorId, int slot)
    {
        return slot >= 1 && slot <= definitions.QuickSlotCount
                && inventory.QuickReferences.TryGetValue(key: (survivorId, slot), value: out long itemId)
                ? FoodPath(inventory: inventory, definitions: definitions, survivorId: survivorId, itemId: itemId)
                : null;
    }

    internal static RunInventory? SpendCarriedFood(RunInventory inventory, RunDefinitions definitions,
            string survivorId, RunInventory.ItemPath? expected)
    {
        return expected is not null && !expected.Items.IsDefaultOrEmpty && expected.IsCarriedBy(survivorId: survivorId)
                && definitions.Items.TryGetValue(key: expected.Item.DefinitionId, value: out RunDefinitions.Item? rule)
                && rule.IsFood ? RunItemTransfer.RemoveOne(inventory: inventory, expected: expected) : null;
    }

    internal static RunInventory Invalidate(RunInventory inventory, RunDefinitions definitions)
    {
        ImmutableDictionary<(string survivor, int slot), long> references = inventory.QuickReferences;
        foreach (((string survivor, int slot) key, long id) in inventory.QuickReferences)
        {
            if (key.slot < 1 || key.slot > definitions.QuickSlotCount
                    || FoodPath(inventory: inventory, definitions: definitions, survivorId: key.survivor, itemId: id)
                            is null)
            {
                references = references.Remove(key: key);
            }
        }

        return ReferenceEquals(objA: references, objB: inventory.QuickReferences)
                ? inventory : inventory.WithReferences(references: references);
    }

    private static RunInventory.ItemPath? FoodPath(RunInventory inventory, RunDefinitions definitions,
            string survivorId, long itemId)
    {
        RunInventory.ItemPath? path = inventory.PathTo(itemId: itemId);
        return path is not null && path.IsCarriedBy(survivorId: survivorId)
                && definitions.Items.TryGetValue(key: path.Item.DefinitionId, value: out RunDefinitions.Item? rule)
                && rule.IsFood ? path : null;
    }
}
