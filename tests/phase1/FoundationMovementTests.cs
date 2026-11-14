using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises identified production movement through the pure authority.</summary>
public sealed class FoundationMovementTests
{
    /// <summary>A rightward command advances exactly three units and preserves ownership.</summary>
    [Fact]
    public void IdentifiedRightwardCommandAdvancesThreeUnits()
    {
        var session = new FoundationSession(sessionId: "session-1", playerId: "local", actorId: "actor-1");
        var command = new FoundationMovementCommand(sessionId: "session-1", playerId: "local",
                actorId: "actor-1", direction: 1);
        session.Advance(command: command, elapsedMilliseconds: 1000);
        session.Actor.PositionSubunits.ShouldBe(expected: -5000);
        session.Actor.Facing.ShouldBe(expected: 1);
        session.Actor.ActorId.ShouldBe(expected: "actor-1");
        session.Advance(command: command with { Direction = 0 }, elapsedMilliseconds: 1000);
        session.Actor.PositionSubunits.ShouldBe(expected: -5000);
        session.Actor.Facing.ShouldBe(expected: 1);
        ProofProtocol.Record(id: "core.authority", diagnostic: "IDENTIFIED_THREE_UNIT_WALK");
    }
}
