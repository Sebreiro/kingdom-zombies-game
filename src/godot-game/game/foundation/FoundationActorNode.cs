using System.Globalization;
using System.Linq;

namespace KingdomZombies.Foundation;

/// <summary>Encodes exact ordinal UTF-16 identities injectively before any native name or path operation.</summary>
internal static class FoundationActorNode
{
    internal static string Name(string actorId)
    {
        return "Actor_" + string.Concat(values: actorId.Select(selector: character => ((int)character)
                .ToString(format: "X4", provider: CultureInfo.InvariantCulture)));
    }
}
