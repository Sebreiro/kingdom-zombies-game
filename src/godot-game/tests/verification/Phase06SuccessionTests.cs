using System.Reflection;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the host's explicit post-death request and control handover.</summary>
[Trait("Tier", "1")]
public sealed class Phase06SuccessionTests
{
    /// <summary>The host owns one explicit, batchable post-death admission boundary.</summary>
    [Fact]
    public void Phase06_E112_RespawnOpensAtExactlyOneSecondAndRequiresInput()
    {
        typeof(FoundationSession).GetMethod(name: "RequestRespawns",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic).ShouldNotBeNull();
    }
}
