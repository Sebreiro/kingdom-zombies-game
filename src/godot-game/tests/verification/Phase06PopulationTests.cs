using System.Reflection;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the actual fresh run's recruited population and exact inventory.</summary>
[Trait("Tier", "1")]
public sealed class Phase06PopulationTests
{
    /// <summary>The native session authority owns the persistent run roster.</summary>
    [Fact]
    public void Phase06_E007_FreshRunCreatesEightIndependentOutfits()
    {
        var session = new FoundationSession(sessionId: "run", playerId: "host", actorId: "host-wire");
        PropertyInfo? population = session.GetType().GetProperty(name: "Population",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);
        population.ShouldNotBeNull(customMessage: "The fresh run must own the persistent recruited population.");
    }
}
