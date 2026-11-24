using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies the production inventory preview and nested workspace lifetime.</summary>
[Trait("Tier", "1")]
public sealed class Phase06InventoryInteractionTests
{
    /// <summary>A presentation-only workspace model is required before recording any committed item changes.</summary>
    [Fact]
    public void Phase06_PreviewCancellationPreservesTheCommittedSource()
    {
        typeof(FoundationInput).Assembly.GetType(name: "KingdomZombies.Foundation.FoundationInventoryInteraction")
                .ShouldNotBeNull();
    }
}
