namespace KingdomZombies.Core;

/// <summary>Addresses one immutable movement request to its owning local actor.</summary>
public sealed record FoundationMovementCommand
{
    /// <summary>Creates an explicit ownership and direction request.</summary>
    public FoundationMovementCommand(string sessionId, string playerId, string actorId, int direction,
            string bindingId = "local", long sequence = 1)
    {
        SessionId = sessionId;
        PlayerId = playerId;
        ActorId = actorId;
        Direction = direction;
        BindingId = bindingId;
        Sequence = sequence;
    }

    /// <summary>Gets the owning session.</summary>
    public string SessionId { get; init; }
    /// <summary>Gets the owning local player.</summary>
    public string PlayerId { get; init; }
    /// <summary>Gets the addressed actor.</summary>
    public string ActorId { get; init; }
    /// <summary>Gets the requested direction.</summary>
    public int Direction { get; init; }
    /// <summary>Gets the current opaque authority binding.</summary>
    public string BindingId { get; init; }
    /// <summary>Gets the monotonically increasing intent number.</summary>
    public long Sequence { get; init; }
}
