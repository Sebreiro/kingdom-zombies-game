namespace KingdomZombies.Foundation;

/// <summary>Maps a provider-authenticated transient sender to a pending authority reservation.</summary>
internal sealed class FoundationPeerBinding
{
    internal FoundationPeerBinding(string bindingId, string playerId, string actorId, long baselineRevision,
            FoundationEnvelope baseline)
    {
        BindingId = bindingId;
        PlayerId = playerId;
        ActorId = actorId;
        BaselineRevision = baselineRevision;
        Baseline = baseline;
    }

    internal string BindingId { get; }
    internal string PlayerId { get; }
    internal string ActorId { get; }
    internal long BaselineRevision { get; }
    internal FoundationEnvelope Baseline { get; }
}
