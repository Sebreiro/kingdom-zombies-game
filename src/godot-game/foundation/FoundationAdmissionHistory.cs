using System.Collections.Generic;

namespace KingdomZombies.Foundation;

/// <summary>Bounds replay outcomes to one finite transport connection without evicting stale-attempt fences.</summary>
internal sealed class FoundationAdmissionHistory
{
    internal FoundationAdmissionHistory(string generation, long startedAt)
    {
        Generation = generation;
        StartedAt = startedAt;
    }

    internal string Generation { get; }
    internal long StartedAt { get; }
    internal bool Expired { get; set; }
    internal Dictionary<string, FoundationEnvelope> Outcomes { get; } = new(comparer: System.StringComparer.Ordinal);
}
