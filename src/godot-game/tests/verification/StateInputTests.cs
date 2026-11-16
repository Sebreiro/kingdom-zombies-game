using System;
using System.Collections.Generic;
using System.Linq;
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
                location: "src/godot-game/core/ExampleState.cs:Advance");
    }

    /// <summary>Rejects negative and checked overflow before consuming random input or replacing the root.</summary>
    [Fact, Trait("Case", "state.numeric")]
    public void Advance_NumericNeighbors_PreservesOldRoot()
    {
        var random = new ControlledRandom(value: 1);
        ExampleState zero = Empty(total: 0);
        zero.Advance(elapsed: new ControlledElapsed(milliseconds: 0), random: random).ShouldBeSameAs(expected: zero);
        random.DrawPosition.ShouldBe(expected: 0);
        zero.Advance(elapsed: new ControlledElapsed(milliseconds: 1), random: random)
                .ElapsedMilliseconds.ShouldBe(expected: 1);
        ExampleState maximum = zero.Advance(elapsed: new ControlledElapsed(milliseconds: long.MaxValue),
                random: random);
        maximum.ElapsedMilliseconds.ShouldBe(expected: long.MaxValue);
        Should.Throw<OverflowException>(actual: () => maximum.Advance(
                elapsed: new ControlledElapsed(milliseconds: 1), random: random));
        Should.Throw<ArgumentOutOfRangeException>(actual: () => zero.Advance(
                elapsed: new ControlledElapsed(milliseconds: -1), random: random));
        Should.Throw<ArgumentOutOfRangeException>(actual: () => Empty(total: -1));
        maximum.Advance(elapsed: new ControlledElapsed(milliseconds: 0), random: random)
                .ShouldBeSameAs(expected: maximum);
        random.DrawPosition.ShouldBe(expected: 2);
        zero.ElapsedMilliseconds.ShouldBe(expected: 0);
        maximum.ElapsedMilliseconds.ShouldBe(expected: long.MaxValue);
        ProofProtocol.Record(id: "state.numeric", diagnostic: "CHECKED_MILLISECONDS",
                location: "src/godot-game/core/ExampleState.cs:Advance");
    }

    /// <summary>Retains whitespace, case, canonical Unicode distinctions and ordinal equality.</summary>
    [Fact, Trait("Case", "state.identities")]
    public void Identity_ExactUnicode_PreservesDistinctValues()
    {
        string[] text = ["x", "X", " x", "x ", " ", "é", "e\u0301", "😀", "\u00a0"];
        StableIdentity[] identities = [.. text.Select(selector: value => new StableIdentity(value: value))];
        identities.Distinct().Count().ShouldBe(expected: text.Length);
        for (int index = 0; index < text.Length; index++)
        {
            var equal = new StableIdentity(value: text[index]);
            identities[index].Equals(other: equal).ShouldBeTrue();
            identities[index].Equals(obj: (object)equal).ShouldBeTrue();
            identities[index].GetHashCode().ShouldBe(expected: equal.GetHashCode());
            identities[index].Value.ShouldBe(expected: text[index]);
        }

        identities[0].Equals(other: null).ShouldBeFalse();
        identities[0].Equals(obj: text[0]).ShouldBeFalse();
        foreach (string invalid in new[] { "", "\0", "\n", "\u007f", "\u0085", "\ud800", "\udc00",
            "x\ud800y", "😀\udc00", new string(c: 'x', count: 129) })
        {
            Should.Throw<ArgumentException>(actual: () => new StableIdentity(value: invalid));
        }

        new StableIdentity(value: new string(c: 'x', count: 127)).Value.Length.ShouldBe(expected: 127);
        new StableIdentity(value: new string(c: 'x', count: 128)).Value.Length.ShouldBe(expected: 128);
        new StableIdentity(value: string.Concat(values: Enumerable.Repeat(element: "😀", count: 64)))
                .Value.Length.ShouldBe(expected: 128);
        Should.Throw<ArgumentException>(actual: () => new StableIdentity(value:
                string.Concat(values: Enumerable.Repeat(element: "😀", count: 64)) + "x"));
        ProofProtocol.Record(id: "state.identities", diagnostic: "ORDINAL_UTF16",
                location: "src/godot-game/core/StableIdentity.cs:ValidateText");
    }

    /// <summary>Copies mutable inputs and rejects duplicates and incomplete candidate collections.</summary>
    [Fact, Trait("Case", "state.ownership")]
    public void Construct_CandidateCollections_ValidatesAndDetaches()
    {
        var identity = new StableIdentity(value: "one");
        var equal = new StableIdentity(value: "one");
        StableIdentity[] ordered = [identity];
        var values = new Dictionary<StableIdentity, long> { [identity] = 9 };
        var state = new ExampleState(elapsedMilliseconds: 0, ordered: ordered, records: values);
        ordered[0] = new StableIdentity(value: "replacement");
        values[identity] = 99;
        state.Ordered[0].ShouldBeSameAs(expected: identity);
        state.Records[equal].ShouldBe(expected: 9);
        Should.Throw<NotSupportedException>(actual: () => ((IList<StableIdentity>)state.Ordered)[0] = equal);
        Should.Throw<NotSupportedException>(actual: () => ((IDictionary<StableIdentity, long>)state.Records)
                .Add(key: equal, value: 1));
        Should.Throw<ArgumentException>(actual: () => new ExampleState(elapsedMilliseconds: 0,
                ordered: [identity], records: [new(key: identity, value: 1), new(key: equal, value: 2)]));
        Should.Throw<ArgumentException>(actual: () => new ExampleState(elapsedMilliseconds: 0,
                ordered: [identity, equal], records: values));
        Should.Throw<ArgumentException>(actual: () => new ExampleState(elapsedMilliseconds: 0,
                ordered: [], records: values));
        Should.Throw<ArgumentException>(actual: () => new ExampleState(elapsedMilliseconds: 0,
                ordered: [identity], records: []));
        Should.Throw<ArgumentOutOfRangeException>(actual: () => new ExampleState(elapsedMilliseconds: 0,
                ordered: [identity], records: [new(key: identity, value: -1)]));
        state.Records[identity].ShouldBe(expected: 9);
        ProofProtocol.Record(id: "state.ownership", diagnostic: "DETACHED_VALIDATED_STATE",
                location: "src/godot-game/core/ExampleState.cs:ValidateRecords/ValidateOrder");
    }

    /// <summary>Equal independent operations preserve meaningful order and unordered key semantics.</summary>
    [Fact, Trait("Case", "state.repeat")]
    public void Advance_IndependentRuns_ProducesExactState()
    {
        var first = new StableIdentity(value: "first");
        var second = new StableIdentity(value: "Second");
        var before = new ExampleState(elapsedMilliseconds: 4, ordered: [second, first],
                records: [new(key: first, value: 1), new(key: second, value: 2)]);
        var equivalent = new ExampleState(elapsedMilliseconds: 4, ordered: [second, first],
                records: [new(key: second, value: 2), new(key: first, value: 1)]);
        ExampleState left = before.Advance(elapsed: new ControlledElapsed(milliseconds: 1),
                random: new ControlledRandom(value: 33));
        ExampleState right = equivalent.Advance(elapsed: new ControlledElapsed(milliseconds: 1),
                random: new ControlledRandom(value: 33));
        left.ElapsedMilliseconds.ShouldBe(expected: right.ElapsedMilliseconds);
        left.Ordered.ShouldBe(expected: right.Ordered);
        left.Ordered[0].ShouldBeSameAs(expected: second);
        left.Ordered[1].ShouldBeSameAs(expected: first);
        left.Records[first].ShouldBe(expected: 1);
        left.Records[second].ShouldBe(expected: 33);
        left.Records.All(predicate: pair => right.Records[pair.Key] == pair.Value).ShouldBeTrue();
        ProofProtocol.Record(id: "state.repeat", diagnostic: "IDENTICAL_EXPLICIT_INPUTS",
                location: "src/godot-game/core/ExampleState.cs:Advance");
    }

    private static ExampleState Empty(long total)
    {
        return new ExampleState(elapsedMilliseconds: total, ordered: [], records: []);
    }
}
