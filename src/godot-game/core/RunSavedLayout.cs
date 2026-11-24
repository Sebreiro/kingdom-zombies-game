using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Preserves the rules that described each item when its saved layout was created.</summary>
internal sealed class RunSavedLayout
{
    internal RunSavedLayout(int maximumContainerLevels,
            ImmutableDictionary<long, (RunDefinitions.Item definition, bool legacy)> items)
    {
        MaximumContainerLevels = maximumContainerLevels;
        Items = items;
    }

    internal int MaximumContainerLevels { get; }
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
                || item.MaxStack is < 1 or > 10000 || item.MaximumCondition is <= 0 or > 1000000
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

    private static bool ValidDisplay(string value)
    {
        return !string.IsNullOrWhiteSpace(value: value) && value.Length <= 512;
    }
}
