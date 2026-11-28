using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace KingdomZombies.Foundation;

/// <summary>Encodes exact ordinal UTF-16 identities injectively before any native name or path operation.</summary>
internal static class FoundationActorNode
{
    internal static string Name(string actorId)
    {
        return "Actor_" + string.Concat(values: actorId.Select(selector: character => ((int)character)
                .ToString(format: "X4", provider: CultureInfo.InvariantCulture)));
    }

    internal sealed class AppearanceCursor
    {
        internal FoundationEquipmentProjection.Frame? Current => _current;

        internal bool Install(long revision, JsonElement world)
        {
            lock (_gate)
            {
                if (revision < 0 || revision <= (_current?.Revision ?? -1))
                {
                    return false;
                }

                FoundationEquipmentProjection.Frame candidate = FoundationEquipmentProjection.Read(
                        revision: revision, world: world);
                if (candidate.Elapsed < (_current?.Elapsed ?? 0))
                {
                    return false;
                }

                _current = candidate;
                return true;
            }
        }

        private readonly Lock _gate = new();
        private volatile FoundationEquipmentProjection.Frame? _current;
    }
}
