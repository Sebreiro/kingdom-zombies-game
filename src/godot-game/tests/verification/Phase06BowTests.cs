using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves launch, first contact and ammo ownership in the host authority.</summary>
[Trait("Tier", "1")]
public sealed class Phase06BowTests
{
    /// <summary>An ordinary attack with a held bow and empty opposite hand has the quiet outcome.</summary>
    [Fact]
    public void Phase06_E088_QuietEmptyAndExactBowBoundariesUseTheOppositeHand()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "left-hand");
        session.Attack(controller: Phase06MeleeTests.Control(), sequence: 1).Status.ToString()
                .ShouldBe(expected: "EmptyAmmo");
    }
}
