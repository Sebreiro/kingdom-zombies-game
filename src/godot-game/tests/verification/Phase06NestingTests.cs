using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves filled-container placement and exact carried references in the run.</summary>
[Trait("Tier", "1")]
public sealed class Phase06NestingTests
{
    /// <summary>The authored maximum depth can be changed while the default remains three.</summary>
    [Fact]
    public void Phase06_E056_ConfiguredDepthAndQuantitiesStayIntegral()
    {
        Should.NotThrow(action: () => Phase06InventoryTests.Configured(
                oldValue: "\"maximumLevels\": 3", newValue: "\"maximumLevels\": 2",
                config: "content/config/inventory.json"));
    }
}
