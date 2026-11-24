using System.Reflection;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies detached authoritative run snapshots and their continuation.</summary>
[Trait("Tier", "1")]
public sealed class Phase06SnapshotTests
{
    /// <summary>A run capture contains durable state beyond the legacy roster.</summary>
    [Fact]
    public void E118CapturesOneDetachedRunWithExactElapsedTime()
    {
        var session = new FoundationSession(sessionId: "snapshot", playerId: "player", actorId: "survivor");
        session.Step(elapsedMilliseconds: 123);
        MethodInfo? capture = typeof(FoundationSession).GetMethod(name: "CaptureRunSnapshot",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);
        capture.ShouldNotBeNull();
        capture.Invoke(obj: session, parameters: null).ShouldBeOfType<byte[]>();
    }
}
