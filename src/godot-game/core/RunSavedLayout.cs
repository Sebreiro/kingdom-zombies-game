using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Preserves the rules that described each item when its saved layout was created.</summary>
internal sealed class RunSavedLayout
{
    internal RunSavedLayout(int maximumContainerLevels, bool legacyStash,
            ImmutableDictionary<long, (RunDefinitions.Item definition, bool legacy)> items)
    {
        MaximumContainerLevels = maximumContainerLevels;
        LegacyStash = legacyStash;
        Items = items;
    }

    internal int MaximumContainerLevels { get; }
    internal bool LegacyStash { get; }
    internal ImmutableDictionary<long, (RunDefinitions.Item definition, bool legacy)> Items { get; }

    internal RunDefinitions.Item RuleFor(RunInventory.Item item, RunDefinitions.Item current)
    {
        return Items.TryGetValue(key: item.Id, value: out (RunDefinitions.Item definition, bool legacy) saved)
                && saved.legacy
                ? saved.definition : current;
    }

    internal static bool IsDifferent(RunDefinitions.Item saved, RunDefinitions.Item current)
    {
        return saved.Footprint != current.Footprint || saved.Rotatable != current.Rotatable
                || saved.MaxStack != current.MaxStack || saved.Storage != current.Storage
                || saved.MaximumCondition != current.MaximumCondition
                || !StringComparer.Ordinal.Equals(x: saved.Slot, y: current.Slot)
                || saved.Appearance != current.Appearance;
    }

    internal static void Validate(RunDefinitions.Item item)
    {
        if (!ValidSize(size: item.Footprint) || item.Storage is { } storage && !ValidSize(size: storage)
                || item.MaxStack is < 1 or > 10000 || item.MaximumCondition is < 1 or > 10000
                || !ValidDisplay(value: item.Appearance.Pack) || !ValidDisplay(value: item.Appearance.CanonicalIcon)
                || item.Appearance.FitFile is { } file && !ValidDisplay(value: file)
                || item.Appearance.FitItem is { } fit && !ValidDisplay(value: fit)
                || item.Slot is { } slot && slot is not "either-logical-hand"
                        && !RunDefinitions.EquipmentSlots.Contains(value: slot, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "RUN_SNAPSHOT_ITEM_LAYOUT");
        }
    }

    internal static bool ValidSize(RunDefinitions.Size size)
    {
        return size.Width is >= 1 and <= 64 && size.Height is >= 1 and <= 64;
    }

    internal bool AllowsContinuation(RunInventory before, RunInventory after, RunDefinitions definitions)
    {
        if (!ReferenceEquals(objA: this, objB: after.SavedLayout) || after.StashGrid != before.StashGrid)
        {
            return false;
        }

        var oldItems = before.Items.ToDictionary(keySelector: item => item.Id);
        foreach (RunInventory.Item item in after.Items)
        {
            _ = oldItems.TryGetValue(key: item.Id, value: out RunInventory.Item? old);
            RunDefinitions.Item rule = definitions.Items[item.DefinitionId];
            if (item.Quantity > rule.MaxStack && item.Quantity > (old?.Quantity ?? 0)
                    || item.Location is RunInventory.GridCell cell && old?.Location != item.Location
                            && !FitsCurrent(inventory: after, rule: rule, cell: cell, definitions: definitions))
            {
                return false;
            }

            if (MaximumContainerLevels > definitions.MaximumContainerLevels && item.StorageGrid.HasValue
                    && after.PathTo(itemId: item.Id) is { } path
                    && path.Items.Length > definitions.MaximumContainerLevels
                    && path.Items.Length > (before.PathTo(itemId: item.Id)?.Items.Length ?? 0))
            {
                return false;
            }
        }

        return true;
    }

    private static bool FitsCurrent(RunInventory inventory, RunDefinitions.Item rule, RunInventory.GridCell cell,
            RunDefinitions definitions)
    {
        if (cell.Rotated && !rule.Rotatable
                || cell.Footprint != RunInventoryRules.Footprint(definition: rule, rotated: cell.Rotated))
        {
            return false;
        }

        if (cell is RunInventory.StashCell)
        {
            return RunInventoryRules.InBounds(cell: cell, bounds: definitions.StashGrid);
        }

        var nested = (RunInventory.ContainerCell)cell;
        RunInventory.ItemPath? parent = inventory.PathTo(itemId: nested.ContainerId);
        return parent is not null && definitions.Items[parent.Item.DefinitionId].Storage is { } storage
                && RunInventoryRules.InBounds(cell: cell, bounds: storage)
                && parent.Items.Length + (rule.Storage.HasValue ? 1 : 0) <= definitions.MaximumContainerLevels;
    }

    private static bool ValidDisplay(string value)
    {
        return !string.IsNullOrWhiteSpace(value: value) && value.Length <= 512;
    }
}
