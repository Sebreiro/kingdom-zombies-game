using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves that receiving identified intent is distinct from elapsed host simulation.</summary>
public sealed class FoundationLanAuthorityTests
{
    /// <summary>An owned intent is accepted without advancing simulation time or position.</summary>
    [Fact]
    public void OwnedIntentIsAcceptedBeforeAnyElapsedStep()
    {
        var session = new FoundationSession(sessionId: "s", playerId: "host", actorId: "host-actor");
        var command = new FoundationMovementCommand(sessionId: "s", playerId: "host",
                actorId: "host-actor", direction: 1);
        session.TryAccept(command: command, trustedBinding: "local", trustedPlayer: "host",
                trustedActor: "host-actor").ShouldBeTrue();
        session.Actor.PositionSubunits.ShouldBe(expected: -8000);
        session.Actor.ElapsedMilliseconds.ShouldBe(expected: 0);
        ProofProtocol.Record(id: "core.authority", diagnostic: "OWNED_INTENT_BEFORE_HOST_STEP");
    }
}
