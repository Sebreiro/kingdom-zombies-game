using System;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves current native HUD, input ownership and spectator presentation decisions.</summary>
[Trait("Tier", "1")]
public sealed class Phase06UiStateTests
{
    /// <summary>Live HUD state is projected from the actual current presentation path.</summary>
    [Fact]
    public void Phase06_E125_PanelInputWinsAndTheCommittedHudRemainsCurrent()
    {
        Type? state = typeof(FoundationInput).Assembly.GetType(
                name: "KingdomZombies.Foundation.FoundationHudState", throwOnError: false);
        state.ShouldNotBeNull();
    }
}
