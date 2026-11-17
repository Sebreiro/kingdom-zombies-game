using System.Text.Json;

namespace KingdomZombies.Inspection;

/// <summary>Owns reusable inventory inspection projections, separate from gameplay transactions.</summary>
internal sealed class InventoryLayouts
{
    /// <summary>Returns the logical equipment identities exposed by this projection.</summary>
    internal static string[] SlotIds()
    {
        return ["headwear"];
    }

    /// <summary>Returns the displayed cell footprint for an inspection item.</summary>
    internal static int[] Footprint(int width, int height, bool rotated)
    {
        return [width, height];
    }

    /// <summary>Resolves a food shortcut to its actual carried instance.</summary>
    internal static JsonElement? ResolveQuick(JsonElement state, int index)
    {
        return null;
    }
}
