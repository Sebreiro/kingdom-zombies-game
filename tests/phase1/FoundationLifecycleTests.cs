using System;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises closed authority and physical release barriers at lifetime changes.</summary>
public sealed class FoundationLifecycleTests
{
    /// <summary>Old commands cannot mutate closed authority or carry held aliases into a fresh session.</summary>
    [Fact]
    public void ClosedAuthorityAndHeldKeysCannotCrossLifetimes()
    {
        var input = new FoundationInput();
        foreach (int key in new[] { 65, 4194319, 4194309 })
        {
            input.Set(key: key, pressed: true);
        }

        input.Clear();
        foreach (int key in new[] { 65, 4194319, 4194309 })
        {
            input.Set(key: key, pressed: true);
            input.FreshPress.ShouldBeFalse();
            input.Direction.ShouldBe(expected: 0);
        }

        input.Set(key: 65, pressed: false);
        input.Set(key: 65, pressed: true);
        input.FreshPress.ShouldBeTrue();
        input.Direction.ShouldBe(expected: -1);
        input.Clear();
        input.Set(key: 4194319, pressed: false);
        input.Set(key: 4194319, pressed: true);
        input.FreshPress.ShouldBeTrue();
        input.Direction.ShouldBe(expected: -1);
        CheckClosedAuthority();
        ProofProtocol.Record(id: "core.lifecycle", diagnostic: "CLOSED_AUTHORITY_AND_RELEASE_BARRIERS");
    }

    private static void CheckClosedAuthority()
    {
        var old = new FoundationSession(sessionId: "old", playerId: "local", actorId: "old-actor");
        var command = new FoundationMovementCommand(sessionId: "old", playerId: "local",
                actorId: "old-actor", direction: 1);
        FoundationActorState before = old.Actor;
        old.Close();
        old.Close();
        Should.Throw<InvalidOperationException>(actual: () => old.Advance(command: command,
                elapsedMilliseconds: 1000));
        old.Actor.ShouldBeSameAs(expected: before);
        var fresh = new FoundationSession(sessionId: "fresh", playerId: "local", actorId: "fresh-actor");
        fresh.Actor.PositionSubunits.ShouldBe(expected: -8000);
        fresh.Actor.Facing.ShouldBe(expected: 1);
        fresh.Actor.PlayerId.ShouldBe(expected: old.Actor.PlayerId);
        Should.Throw<ArgumentException>(actual: () => fresh.Advance(command: command, elapsedMilliseconds: 1));
        fresh.Actor.PositionSubunits.ShouldBe(expected: -8000);
    }
}
