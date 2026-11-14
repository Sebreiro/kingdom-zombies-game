using System.Collections.Generic;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Exercises the actual pure Core controlled-input operation.</summary>
public sealed class StateInputTests
{
    /// <summary>Advances real state with one draw while preserving exact identities and old values.</summary>
    [Fact, Trait("Case", "state.inputs")]
    public void Advance_ControlledInputs_PreservesValuesAndIdentities()
    {
        var identity = new StableIdentity(value: "É sample ");
        var random = new ControlledRandom(value: 42);
        var before = new ExampleState(elapsedMilliseconds: 7, ordered: [identity],
                records: new Dictionary<StableIdentity, long> { [identity] = 9 });
        ExampleState after = before.Advance(elapsed: new ControlledElapsed(milliseconds: 5), random: random);
        after.ElapsedMilliseconds.ShouldBe(expected: 12);
        after.Records[identity].ShouldBe(expected: 42);
        after.Ordered[0].ShouldBeSameAs(expected: identity);
        after.Ordered[0].Value.ShouldBe(expected: "É sample ");
        before.ElapsedMilliseconds.ShouldBe(expected: 7);
        before.Records[identity].ShouldBe(expected: 9);
        random.DrawPosition.ShouldBe(expected: 1);
        ProofProtocol.Record(id: "state.inputs", diagnostic: "CONTROLLED_ADVANCE",
                location: "src/core/ExampleState.cs:Advance");
    }
}
