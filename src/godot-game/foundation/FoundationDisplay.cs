using System.Linq;
using System.Text;

namespace KingdomZombies.Foundation;

/// <summary>Bounds network presentation without changing a profile's full persisted Unicode display name.</summary>
internal static class FoundationDisplay
{
    internal static string Label(string value)
    {
        Rune[] runes = [.. value.EnumerateRunes().Take(count: 257)];
        return runes.Length <= 256 ? value : string.Concat(values: runes.Take(count: 255)) + "…";
    }
}
