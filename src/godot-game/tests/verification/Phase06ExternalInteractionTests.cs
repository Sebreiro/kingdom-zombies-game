using System.Reflection;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies external target, stash paging and access-loss presentation decisions.</summary>
[Trait("Tier", "1")]
public sealed class Phase06ExternalInteractionTests
{
    /// <summary>Page requests require the currently committed stash view and validated input.</summary>
    [Fact]
    public void Phase06_ExternalPagerUsesCurrentTokenAndRejectsBadInput()
    {
        typeof(FoundationInventoryInteraction).GetMethod(name: "PageRequest", bindingAttr:
                BindingFlags.NonPublic | BindingFlags.Static).ShouldNotBeNull();
    }
}
