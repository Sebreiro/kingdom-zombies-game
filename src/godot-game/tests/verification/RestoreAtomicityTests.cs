using System;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
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
                location: "src/godot-game/core/ExampleStateStore.cs:Restore/Read");
    }

    /// <summary>Repeated accepts retain all state and next-draw values without append or effects.</summary>
    [Fact, Trait("Case", "ac-22.complete")]
    public void Acceptance_RepeatedRestore_PreservesAllPersistedFields()
    {
        ExampleState expected = SnapshotProofData.State(count: 4, total: 9007199254740993,
                seed: int.MinValue, position: 999999);
        var store = new ExampleStateStore(initial: SnapshotProofData.State(count: 0));
        long next = SnapshotProofData.Next(state: expected);
        for (int repeat = 0; repeat < 5; repeat++)
        {
            ExampleState accepted = store.Restore(document: SnapshotCodec.Encode(state: expected),
                    cancellationToken: TestContext.Current.CancellationToken);
            SnapshotProofData.Equal(actual: store.Read(), expected: expected);
            accepted.ShouldBeSameAs(expected: store.Read());
            SnapshotProofData.Next(state: accepted).ShouldBe(expected: next);
        }

        expected.DrawPosition.ShouldBe(expected: 999999);
        ProofProtocol.Record(id: "ac-22.complete", diagnostic: "RESTORE_WITHOUT_REPLAY",
                location: "src/godot-game/core/ExampleStateStore.cs:Restore; src/godot-game/core/"
                        + "ExampleState.cs:persisted fields");
    }

    /// <summary>Interrupted preparation preserves the old root reference and every field.</summary>
    [Fact, Trait("Case", "state.restore-interrupted")]
    public void Restore_InterruptedPreparation_PreservesExactRoot()
    {
        ExampleState old = SnapshotProofData.State(count: 1, total: 7);
        ExampleState candidate = SnapshotProofData.State(count: 3, total: 91, seed: 117, position: 3);
        var store = new ExampleStateStore(initial: old);
        Should.Throw<InvalidOperationException>(actual: () => store.Restore(
                document: SnapshotCodec.Encode(state: candidate), beforeCommit: prepared =>
                {
                    SnapshotProofData.Equal(actual: prepared, expected: candidate);
                    store.Read().ShouldBeSameAs(expected: old, customMessage: "ATOMIC_ROOT_UNCHANGED");
                    SnapshotProofData.Equal(actual: store.Read(), expected: old);
                    throw new InvalidOperationException(message: "PREPARATION_INTERRUPTED");
                }, cancellationToken: TestContext.Current.CancellationToken))
                .Message.ShouldBe(expected: "PREPARATION_INTERRUPTED");
        store.Read().ShouldBeSameAs(expected: old, customMessage: "ATOMIC_ROOT_UNCHANGED");
        SnapshotProofData.Equal(actual: store.Read(), expected: old);
        store.Restore(document: SnapshotCodec.Encode(state: candidate),
                cancellationToken: TestContext.Current.CancellationToken);
        SnapshotProofData.Equal(actual: store.Read(), expected: candidate);
        ProofProtocol.Record(id: "state.restore-interrupted", diagnostic: "INTERRUPTED_ROOT_UNCHANGED",
                location: "src/godot-game/core/ExampleStateStore.cs:Restore before Interlocked.Exchange");
    }

    /// <summary>Cancellation before preparation or immediately before commit discards the complete candidate.</summary>
    [Fact, Trait("Case", "state.restore-cancellation")]
    public void Restore_CancelledBeforeCommit_PreservesExactRoot()
    {
        ExampleState old = SnapshotProofData.State(count: 1, total: 7);
        var store = new ExampleStateStore(initial: old);
        byte[] document = SnapshotCodec.Encode(state: SnapshotProofData.State(count: 3, total: 91));
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(
                token: TestContext.Current.CancellationToken);
        Should.Throw<OperationCanceledException>(actual: () => store.Restore(document: document,
                beforeCommit: _ => cancellation.Cancel(), cancellationToken: cancellation.Token));
        store.Read().ShouldBeSameAs(expected: old);
        Should.Throw<OperationCanceledException>(actual: () => store.Restore(document: document,
                cancellationToken: cancellation.Token));
        store.Read().ShouldBeSameAs(expected: old);
        store.Restore(document: document, cancellationToken: TestContext.Current.CancellationToken);
        store.Read().ElapsedMilliseconds.ShouldBe(expected: 91);
        ProofProtocol.Record(id: "state.restore-cancellation", diagnostic: "CANCELLED_ROOT_UNCHANGED",
                location: "src/godot-game/core/ExampleStateStore.cs:Restore cancellation fence before commit");
    }

    /// <summary>Invalid input following accepted restoration retains the exact accepted root.</summary>
    [Fact, Trait("Case", "state.restore-invalid")]
    public void Restore_InvalidAfterValid_PreservesAcceptedRoot()
    {
        var store = new ExampleStateStore(initial: SnapshotProofData.State(count: 0));
        ExampleState accepted = store.Restore(document: SnapshotCodec.Encode(state: SnapshotProofData.State()),
                cancellationToken: TestContext.Current.CancellationToken);
        foreach ((string name, string value) in new[] { ("elapsedMilliseconds", "-1"),
            ("elapsedMilliseconds", "9223372036854775808"), ("drawPosition", "1000001"),
            ("seed", "2147483648"), ("records", "null"), ("kind", "\"Unknown\"") })
        {
            JsonObject invalid = SnapshotProofData.Document();
            invalid[name] = JsonNode.Parse(json: value);
            Should.Throw<Exception>(actual: () => store.Restore(document: Encoding.UTF8.GetBytes(
                    s: invalid.ToJsonString()), cancellationToken: TestContext.Current.CancellationToken));
            store.Read().ShouldBeSameAs(expected: accepted);
        }

        SnapshotProofData.Equal(actual: store.Read(), expected: SnapshotProofData.State());
        ProofProtocol.Record(id: "state.restore-invalid", diagnostic: "INVALID_ACCEPTED_ROOT_UNCHANGED",
                location: "src/godot-game/core/ExampleStateStore.cs:Restore detached validation");
    }

    /// <summary>Randomized unordered records and repeated independent restores never append or reroll.</summary>
    [Fact, Trait("Case", "edge.rule-07.idempotency")]
    public void Restore_ReorderedRepetitions_PreservesSemanticState()
    {
        JsonObject document = SnapshotProofData.Document(count: 7);
        ExampleState expected = SnapshotProofData.Decode(document: document);
        var first = new ExampleStateStore(initial: SnapshotProofData.State(count: 0));
        var second = new ExampleStateStore(initial: SnapshotProofData.State(count: 1));
        var shuffle = new Random(Seed: 119);
        for (int repeat = 0; repeat < 12; repeat++)
        {
            JsonArray records = document["records"]?.AsArray()
                    ?? throw new InvalidOperationException(message: "PROOF_RECORDS_REQUIRED");
            document["records"] = new JsonArray(items: [.. records.Select(selector: node => node?.DeepClone())
                    .OrderBy(keySelector: _ => shuffle.Next())]);
            byte[] bytes = Encoding.UTF8.GetBytes(s: document.ToJsonString());
            first.Restore(document: bytes, cancellationToken: TestContext.Current.CancellationToken);
            second.Restore(document: bytes, cancellationToken: TestContext.Current.CancellationToken);
            SnapshotProofData.Equal(actual: first.Read(), expected: expected);
            SnapshotProofData.Equal(actual: second.Read(), expected: expected);
            SnapshotProofData.Next(state: first.Read()).ShouldBe(expected: SnapshotProofData.Next(state: expected));
        }

        ProofProtocol.Record(id: "edge.rule-07.idempotency", diagnostic: "IDEMPOTENT_PERMUTED_RESTORE",
                location: "src/godot-game/core/ExampleStateStore.cs:Restore; independent stores, seed=119");
    }

    /// <summary>Competing isolated runs use controlled barriers and cross-field complete-root fingerprints.</summary>
    [Fact, Trait("Case", "edge.rule-07.concurrency")]
    public async Task Restore_ConcurrentReaders_ObservesOnlyCompleteRoots()
    {
        await Task.WhenAll(tasks: [.. Enumerable.Range(start: 0, count: 3).Select(selector: run =>
                RestoreConcurrencyProof.Run(run: run, cancellationToken: TestContext.Current.CancellationToken))]);
        ProofProtocol.Record(id: "edge.rule-07.concurrency", diagnostic: "ONLY_COMPLETE_OLD_OR_NEW",
                location: "src/godot-game/core/ExampleStateStore.cs:Read/"
                        + "Restore; three isolated stores, four readers each");
    }

    /// <summary>Detects early record publication by mutating the actual store in an isolated compiled copy.</summary>
    [Fact, Trait("Case", "state.restore-mutation")]
    public void Mutation_EarlyFirstRecord_DetectedByUnchangedRootAssertion()
    {
        RestoreMutationProof.Verify();
        ProofProtocol.Record(id: "state.restore-mutation", diagnostic: "EARLY_RECORD_MUTATION_DETECTED",
                location: "src/godot-game/core/"
                        + "ExampleStateStore.cs:Restore isolated early publication; unchanged-root target");
    }
}
