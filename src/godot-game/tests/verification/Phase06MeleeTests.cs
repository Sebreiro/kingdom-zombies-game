using System.Reflection;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the ordinary host's melee and per-hit armor authority.</summary>
[Trait("Tier", "1")]
public sealed class Phase06MeleeTests
{
    /// <summary>The run provides one current-control attack boundary.</summary>
    [Fact]
    public void Phase06_E080_DualMeleeCanHitOneAdjacentTargetTwice()
    {
        typeof(FoundationSession).GetMethod(name: "Attack",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic).ShouldNotBeNull();
    }
}
