using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Closes acceptance and projections before releasing a coordinator lifetime.</summary>
public sealed class FoundationLanLifecycleTests
{
    /// <summary>Repeated closure cannot retain a projection or close native ownership more than once.</summary>
    [Fact]
    public void LifetimeCleanupAndDeadlineBoundaries()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        host.Close();
        host.Snapshot.ShouldBeNull();
        host.Close();
        port.CloseCount.ShouldBe(expected: 1);
        host.Submit(direction: 1).ShouldBeFalse();
    }
}
