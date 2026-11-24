using System.Linq;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the host's death ownership and exact empty-loot lifetime.</summary>
[Trait("Tier", "1")]
public sealed class Phase06DeathTests
{
    /// <summary>A real lethal attack leaves the former survivor with no carried roots.</summary>
    [Fact]
    public void Phase06_E104_AdjacentActualBelongingsMoveToOneCorpse()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        Phase06MeleeTests.Enemy(session: session, id: "enemy", x: -7000);
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 5, stamina: 100).ShouldBeTrue();
        RunDamage.Hit hit = session.AttackUncontrolled(survivorId: "enemy", sequence: 1).Hits.Single();
        hit.Lethal.ShouldBeTrue();
        session.Inventory.CarriedBy(survivorId: Phase06MeleeTests.Owner).ShouldBeEmpty();
    }
}
