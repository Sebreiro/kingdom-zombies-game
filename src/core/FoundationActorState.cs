namespace KingdomZombies.Core;

/// <summary>Publishes the immutable, versioned actor state in fixed-point world coordinates.</summary>
public sealed record FoundationActorState
{
    /// <summary>Creates one immutable state publication.</summary>
    public FoundationActorState(string sessionId, string playerId, string actorId, long positionSubunits,
            int facing, long elapsedMilliseconds)
    {
        SessionId = sessionId;
        PlayerId = playerId;
        ActorId = actorId;
        PositionSubunits = positionSubunits;
        Facing = facing;
        ElapsedMilliseconds = elapsedMilliseconds;
    }

    /// <summary>Gets the owning session.</summary>
    public string SessionId { get; }
    /// <summary>Gets the owning player.</summary>
    public string PlayerId { get; }
    /// <summary>Gets the actor identity.</summary>
    public string ActorId { get; }
    /// <summary>Gets horizontal fixed-point position.</summary>
    public long PositionSubunits { get; }
    /// <summary>Gets the last nonzero direction.</summary>
    public int Facing { get; }
    /// <summary>Gets accumulated explicit game time.</summary>
    public long ElapsedMilliseconds { get; }
    /// <summary>Gets the explicit state schema.</summary>
    public int SchemaVersion { get; } = 1;
    /// <summary>Gets the authored fixture identity.</summary>
    public string FixtureId { get; } = "foundation.fixture.v1";
    /// <summary>Gets the authored ground identity.</summary>
    public string GroundId { get; } = "foundation.ground.v1";
    /// <summary>Gets the authored decorative building identity.</summary>
    public string BuildingId { get; } = "foundation.building.v1";
    /// <summary>Gets the authored player primitive identity.</summary>
    public string PlayerContentId { get; } = "foundation.player.v1";
    /// <summary>Gets the ground feet coordinate.</summary>
    public long GroundSubunits { get; }
}
