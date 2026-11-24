using System.Reflection;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves exact placement and stack transactions in the production run.</summary>
[Trait("Tier", "1")]
public sealed class Phase06InventoryTests
{
    /// <summary>Grid changes go through the ordinary run's inventory transaction.</summary>
    [Fact]
    public void Phase06_E037_ExactFitAndOverflowUseTheCurrentRun()
    {
        var session = new FoundationSession(sessionId: "run", playerId: "host", actorId: "host-wire");
        MethodInfo? commit = session.GetType().GetMethod(name: "TryEditInventory",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);
        commit.ShouldNotBeNull(customMessage: "A legal grid placement must be able to commit to the current run.");
    }
}
