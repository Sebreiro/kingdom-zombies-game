using System.Collections.Generic;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Executes the real strict snapshot boundary against detached Core states.</summary>
public sealed class SnapshotTests
{
    /// <summary>Restores exact values without modifying the source root.</summary>
    [Fact, Trait("Case", "state.snapshot")]
    public void Codec_DetachedState_RoundTripsExactValues()
    {
        var identity = new StableIdentity(value: "é😀");
        var root = new ExampleState(elapsedMilliseconds: 9007199254740993, ordered: [identity],
                records: new Dictionary<StableIdentity, long> { [identity] = long.MaxValue });
        ExampleState restored = SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: root));
        restored.ElapsedMilliseconds.ShouldBe(expected: 9007199254740993);
        restored.Records[identity].ShouldBe(expected: long.MaxValue);
        restored.Ordered[0].Value.ShouldBe(expected: "é😀");
        restored.ShouldNotBeSameAs(expected: root);
        root.ElapsedMilliseconds.ShouldBe(expected: 9007199254740993);
        ProofProtocol.Record(id: "state.snapshot", diagnostic: "STRICT_SNAPSHOT_ROUNDTRIP",
                location: "src/core/SnapshotCodec.cs:Decode");
    }
}
