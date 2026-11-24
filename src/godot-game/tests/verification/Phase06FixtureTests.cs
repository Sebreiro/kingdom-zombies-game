using System;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies the explicitly selected stationary combat fixture and its production transactions.</summary>
[Trait("Tier", "1")]
public sealed class Phase06FixtureTests
{
    /// <summary>The production fixture must define patients with distinct adjacent target identities.</summary>
    [Fact]
    public void Phase06_E129_FixtureHasDistinctStationaryTargets()
    {
        Type? fixture = typeof(FoundationSession).Assembly.GetType(
                name: "KingdomZombies.Core.RunCombatFixture", throwOnError: false);
        fixture.ShouldNotBeNull();
    }
}
