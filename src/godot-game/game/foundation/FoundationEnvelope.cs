using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

/// <summary>Holds a completely parsed, detached protocol message before dispatch.</summary>
internal sealed class FoundationEnvelope
{
    public FoundationEnvelope(string kind, string sessionId = "", string playerId = "", string actorId = "",
            string bindingId = "", string label = "", long sequence = 0, int direction = 0, long revision = 0,
            FoundationSnapshot? snapshot = null, string attemptId = "", FoundationRunFrame? game = null)
    {
        Kind = kind;
        SessionId = sessionId;
        PlayerId = playerId;
        ActorId = actorId;
        BindingId = bindingId;
        Label = label;
        Sequence = sequence;
        Direction = direction;
        Revision = revision;
        Snapshot = snapshot;
        AttemptId = attemptId;
        Game = game;
    }

    public string Kind { get; }
    public string SessionId { get; }
    public string PlayerId { get; }
    public string ActorId { get; }
    public string BindingId { get; }
    public string Label { get; }
    public long Sequence { get; }
    public int Direction { get; }
    public long Revision { get; }
    public FoundationSnapshot? Snapshot { get; }
    public string AttemptId { get; }
    public FoundationRunFrame? Game { get; }
    public int ProtocolVersion { get; } = FoundationProtocol.Version;
    public string ContentId { get; } = FoundationProtocol.Content;
}
