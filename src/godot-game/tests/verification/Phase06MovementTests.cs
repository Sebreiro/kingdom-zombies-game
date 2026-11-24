using System;
using System.IO;
using System.Text;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves configured movement and stamina on the production authority.</summary>
[Trait("Tier", "1")]
public sealed class Phase06MovementTests
{
    /// <summary>The ordinary session uses an authored fractional walk rate.</summary>
    [Fact]
    public void Phase06_E017_AuthoredFractionalMotionKeepsPartitionPrecision()
    {
        FoundationSession session = ConfiguredWalk();
        var command = new FoundationMovementCommand(sessionId: "movement", playerId: "host",
                actorId: "actor", direction: 1);
        session.Advance(command: command, elapsedMilliseconds: 1000);
        session.Actor.PositionSubunits.ShouldBe(expected: -3500);
    }

    private static FoundationSession ConfiguredWalk()
    {
        return FoundationSession.CreateFromResources(sessionId: "movement", playerId: "host", actorId: "actor",
                read: ReadWithWalk);
    }

    private static byte[] ReadWithWalk(string path, int limit)
    {
        using Stream stream = typeof(FoundationSession).Assembly.GetManifestResourceStream(name: path)
                ?? throw new InvalidOperationException(message: path);
        stream.Length.ShouldBeLessThanOrEqualTo(expected: limit);
        byte[] bytes = new byte[checked((int)stream.Length)];
        stream.ReadExactly(buffer: bytes);
        return path is "content/config/survivors.json" ? Encoding.UTF8.GetBytes(s: Encoding.UTF8.GetString(
                bytes: bytes).Replace(oldValue: "\"walkSpeedUnitsPerSecond\": 3",
                        newValue: "\"walkSpeedUnitsPerSecond\": 4.5", comparisonType: StringComparison.Ordinal))
                : bytes;
    }
}
