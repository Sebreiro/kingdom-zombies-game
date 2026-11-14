using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires whole-candidate restore with immutable root publication.</summary>
public sealed class RestoreAtomicityTests
{
    /// <summary>Publishes exactly the accepted candidate without changing the former root.</summary>
    [Fact, Trait("Case", "state.restore")]
    public void Restore_AcceptedCandidate_ReplacesOneCompleteRoot()
    {
        ExampleState old = SnapshotProofData.State(count: 1, total: 7, seed: 1, position: 1);
        ExampleState expected = SnapshotProofData.State(count: 3, total: 91, seed: 117, position: 3);
        var store = new ExampleStateStore(initial: old);
        store.Restore(document: SnapshotCodec.Encode(state: expected),
                cancellationToken: TestContext.Current.CancellationToken);
        ExampleState actual = store.Read();
        actual.ElapsedMilliseconds.ShouldBe(expected: 91);
        SnapshotProofData.Equal(actual: actual, expected: expected);
        old.ElapsedMilliseconds.ShouldBe(expected: 7);
        old.Records.Count.ShouldBe(expected: 1);
        ProofProtocol.Record(id: "state.restore", diagnostic: "ATOMIC_ACCEPTED_ROOT",
                location: "src/core/ExampleStateStore.cs:Restore/Read");
    }
}
