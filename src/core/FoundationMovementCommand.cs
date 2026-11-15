namespace KingdomZombies.Core;

/// <summary>Addresses one immutable movement request to its owning local actor.</summary>
public sealed record FoundationMovementCommand
{
    /// <summary>Creates an explicit ownership and direction request.</summary>
    public FoundationMovementCommand(string sessionId, string playerId, string actorId, int direction)
    {
        SessionId = sessionId;
        PlayerId = playerId;
        ActorId = actorId;
        Direction = direction;
    }

    /// <summary>Gets the owning session.</summary>
    public string SessionId { get; init; }
    /// <summary>Gets the owning local player.</summary>
    public string PlayerId { get; init; }
    /// <summary>Gets the addressed actor.</summary>
    public string ActorId { get; init; }
    /// <summary>Gets the requested direction.</summary>
    public int Direction { get; init; }
}
