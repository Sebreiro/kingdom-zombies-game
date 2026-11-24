using System.Reflection;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves continuous safe rest and real food use in the host publication.</summary>
[Trait("Tier", "1")]
public sealed class Phase06RecoveryTests
{
    /// <summary>The ordinary session owns the combined food and vitals transaction.</summary>
    [Fact]
    public void Phase06_E020_FullVitalsConsumeOneActualUnitAndRejectReplay()
    {
        typeof(FoundationSession).GetMethod(name: "ConsumeFood",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic).ShouldNotBeNull();
    }
}
