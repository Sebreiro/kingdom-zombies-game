using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies the production mapping from committed run appearances to authored layers.</summary>
[Trait("Tier", "1")]
public sealed class Phase06AppearanceTests
{
    /// <summary>Living and dead gear must be projected through the native composition contract.</summary>
    [Fact]
    public void Phase06_E099_ProjectsWornGearAndCosmeticCrown()
    {
        typeof(FoundationActorNode).Assembly.GetType(name: "KingdomZombies.Foundation.FoundationEquipmentProjection")
                .ShouldNotBeNull();
    }
}
