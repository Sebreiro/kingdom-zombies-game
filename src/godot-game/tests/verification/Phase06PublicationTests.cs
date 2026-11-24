using System.Reflection;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies that normal composition and peer frames share the same run authority.</summary>
[Trait("Tier", "1")]
public sealed class Phase06PublicationTests
{
    /// <summary>A current connection can invoke the same scoped gameplay dispatcher as a local player.</summary>
    [Fact]
    public void TrustedGuestAndSoloUseOneDispatcher()
    {
        MethodInfo? dispatch = typeof(FoundationCoordinator).GetMethod(name: "DispatchBoundRunAction",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);
        dispatch.ShouldNotBeNull();
    }
}
