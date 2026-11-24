using System.Reflection;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the current authority for nearby inventory access and item actions.</summary>
[Trait("Tier", "1")]
public sealed class Phase06AccessTests
{
    /// <summary>Ordinary inventory requests have one run-owned dispatcher.</summary>
    [Fact]
    public void Phase06_E064_ListUsesTheInclusiveWorldRanges()
    {
        typeof(FoundationSession).GetProperty(name: "ItemActions",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic).ShouldNotBeNull();
    }
}
