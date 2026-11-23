using System;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises identified production movement through the pure authority.</summary>
[Trait("Tier", "1")]
public sealed class FoundationMovementTests
{
    /// <summary>Aliases cancel and release through the exact production input helper.</summary>
    [Fact]
    public void Move_PhysicalAliases_CancelAndRelease()
    {
        var input = new FoundationInput();
        input.Set(key: 65, pressed: true);
        input.Direction.ShouldBe(expected: -1);
        input.Set(key: 4194319, pressed: true);
        input.Direction.ShouldBe(expected: -1);
        input.Set(key: 65, pressed: true);
        input.Direction.ShouldBe(expected: -1);
        input.Set(key: 68, pressed: true);
        input.Direction.ShouldBe(expected: 0);
        input.Set(key: 65, pressed: false);
        input.Direction.ShouldBe(expected: 0);
        input.Set(key: 4194319, pressed: false);
        input.Direction.ShouldBe(expected: 1);
        input.Set(key: 68, pressed: false);
        input.Direction.ShouldBe(expected: 0);
        ProofProtocol.Record(id: "core.input", diagnostic: "PHYSICAL_ALIAS_CANCEL_RELEASE");
    }

    /// <summary>A rightward command advances exactly three units and preserves ownership.</summary>
    [Fact]
    public void Move_IdentifiedRightwardCommand_AdvancesThreeUnits()
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
        CheckValidationAndBounds();
        CheckSmallElapsedUpdates();
        ProofProtocol.Record(id: "core.authority", diagnostic: "IDENTIFIED_THREE_UNIT_WALK");
    }

    private static void CheckValidationAndBounds()
    {
        var session = new FoundationSession(sessionId: "s", playerId: "p", actorId: "a");
        var command = new FoundationMovementCommand(sessionId: "s", playerId: "p", actorId: "a", direction: -1);
        FoundationActorState original = session.Actor;
        session.Advance(command: command, elapsedMilliseconds: 1000);
        session.Actor.PositionSubunits.ShouldBe(expected: -11000);
        session.Actor.Facing.ShouldBe(expected: -1);
        session.Advance(command: command with { Direction = 0 }, elapsedMilliseconds: 100);
        session.Actor.PositionSubunits.ShouldBe(expected: -11000);
        session.Actor.Facing.ShouldBe(expected: -1);
        original.PositionSubunits.ShouldBe(expected: -8000);
        foreach (FoundationMovementCommand invalid in new[] { command with { SessionId = "old" },
            command with { PlayerId = "other" }, command with { ActorId = "other" }, command with { Direction = 2 },
            command with { Direction = -2 } })
        {
            FoundationActorState before = session.Actor;
            Should.Throw<ArgumentException>(actual: () => session.Advance(command: invalid, elapsedMilliseconds: 1));
            session.Actor.ShouldBeSameAs(expected: before);
        }

        Should.Throw<ArgumentOutOfRangeException>(actual: () => session.Advance(command: command,
                elapsedMilliseconds: -1));
        session.Advance(command: command, elapsedMilliseconds: long.MaxValue - session.Actor.ElapsedMilliseconds);
        session.Actor.PositionSubunits.ShouldBe(expected: -59000);
        session.Actor.ElapsedMilliseconds.ShouldBe(expected: long.MaxValue);
        FoundationActorState end = session.Actor;
        Should.Throw<OverflowException>(actual: () => session.Advance(command: command, elapsedMilliseconds: 1));
        session.Actor.ShouldBeSameAs(expected: end);
        var fresh = new FoundationSession(sessionId: "s", playerId: "p", actorId: "a");
        fresh.Advance(command: command with { Direction = 1 }, elapsedMilliseconds: 100000);
        fresh.Actor.PositionSubunits.ShouldBe(expected: 59000);
        fresh.Advance(command: command, elapsedMilliseconds: 1000);
        fresh.Actor.PositionSubunits.ShouldBe(expected: 56000);
        fresh.Actor.GroundSubunits.ShouldBe(expected: 0);
        fresh.Actor.ActorId.ShouldBe(expected: "a");
    }

    private static void CheckSmallElapsedUpdates()
    {
        var session = new FoundationSession(sessionId: "s", playerId: "p", actorId: "a");
        var command = new FoundationMovementCommand(sessionId: "s", playerId: "p", actorId: "a", direction: 1);
        for (int frame = 0; frame < 60; frame++)
        {
            session.Advance(command: command, elapsedMilliseconds: frame % 3 == 2 ? 16 : 17);
        }

        session.Actor.ElapsedMilliseconds.ShouldBe(expected: 1000);
        session.Actor.PositionSubunits.ShouldBe(expected: -5000);
        session.Actor.ActorId.ShouldBe(expected: "a");
    }
}

