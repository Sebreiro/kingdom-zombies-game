using System.Text;
using System.Text.Json;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises stable corpse state independently from transient event lifetime.</summary>
public sealed class AppearanceStateTests
{
    /// <summary>Explicit death fades its effect while current corpse and gear remain.</summary>
    [Fact]
    public void Death_AfterTransientExpiryPreservesCurrentEquippedCorpse()
    {
        var subject = new AppearanceState(definition: Encoding.UTF8.GetBytes(s: Definition));
        JsonElement equipment = JsonSerializer.SerializeToElement(value: new { backpack = "IT-23" });
        subject.Restore(body: "CH-01", kind: "alive", equipment: equipment, selected: true);
        subject.TriggerDeath();
        subject.Advance(elapsedSeconds: 5);
        subject.Snapshot.GetProperty(propertyName: "persistent").GetBoolean().ShouldBeTrue();
        subject.Snapshot.GetProperty(propertyName: "kind").GetString().ShouldBe(expected: "corpse");
        subject.Snapshot.GetProperty(propertyName: "equipment").GetProperty(propertyName: "backpack")
                .GetString().ShouldBe(expected: "IT-23");
        subject.Snapshot.GetProperty(propertyName: "transientVisible").GetBoolean().ShouldBeFalse();
    }

    private const string Definition = "{\"schemaVersion\":1,\"id\":\"corpses-wildlife\",\"version\":1,"
            + "\"bodies\":[\"CH-01\",\"CH-02\",\"CH-03\",\"CH-04\",\"CH-05\",\"CH-06\",\"CH-07\",\"CH-08\"],"
            + "\"wildlife\":[\"rabbit\",\"deer\"],\"slots\":{\"backpack\":[\"IT-23\",\"IT-24\"]},"
            + "\"collapseSeconds\":0.32,\"effectSeconds\":0.8}";
}
