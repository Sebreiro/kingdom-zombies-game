using System.Collections.Immutable;

namespace KingdomZombies.Foundation;

/// <summary>Maps a provider-authenticated transient sender to a pending authority reservation.</summary>
internal sealed class FoundationPeerBinding
{
    internal FoundationPeerBinding(string bindingId, string playerId, string actorId, long baselineRevision,
            FoundationEnvelope baseline, long startedAt, string connectionGeneration)
    {
        BindingId = bindingId;
        PlayerId = playerId;
        ActorId = actorId;
        BaselineRevision = baselineRevision;
        Baseline = baseline;
        StartedAt = startedAt;
        LastResponse = startedAt;
        ConnectionGeneration = connectionGeneration;
    }

    internal string BindingId { get; }
    internal string ConnectionGeneration { get; }
    internal string PlayerId { get; }
    internal string ActorId { get; }
    internal long BaselineRevision { get; }
    internal FoundationEnvelope Baseline { get; }
    internal long StartedAt { get; }
    internal long LastResponse { get; set; }
    internal long HeartbeatSequence { get; set; }
    internal bool Ready { get; set; }
    internal long RunSequence { get; set; }
    internal string RunAction { get; set; } = "";
    internal string RunStatus { get; set; } = "";
    internal long RunViewSerial { get; set; }
    internal long LastRunStatus { get; set; }
    internal (long epoch, long token, long inventory) RunViewFence { get; set; }
    internal long? OwnGrid { get; set; }
    internal long? ExternalGrid { get; set; }
    internal int NearbyPage { get; set; } = 1;
    internal ImmutableArray<long> OwnWindows { get; set; } = [];
    internal ImmutableArray<long> ExternalWindows { get; set; } = [];
    internal ImmutableArray<string> WindowScopes { get; set; } = [];
    internal ImmutableQueue<byte[]> PublicParts { get; set; } = ImmutableQueue<byte[]>.Empty;
}
