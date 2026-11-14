using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Coordinates actual writers and readers with barriers rather than timing guesses.</summary>
internal static class RestoreConcurrencyProof
{
    /// <summary>Requires old/new snapshots, rejects a writer at the barrier, and bounds every wait.</summary>
    internal static async Task Run(int run, CancellationToken cancellationToken)
    {
        ExampleState old = SnapshotProofData.State(count: 1, total: 7 + run, seed: 1 + run, position: 1);
        ExampleState candidate = SnapshotProofData.State(count: 3, total: 91 + run, seed: 117 + run, position: 3);
        var store = new ExampleStateStore(initial: old);
        using var prepared = new ManualResetEventSlim(initialState: false);
        using var release = new ManualResetEventSlim(initialState: false);
        using var started = new CountdownEvent(initialCount: 4);
        using var finished = new CancellationTokenSource();
        var observations = new ConcurrentBag<string>();
        string before = Fingerprint(state: old);
        string after = Fingerprint(state: candidate);
        Task<ExampleState> writer = Task.Run(function: () => store.Restore(document: SnapshotCodec.Encode(
                state: candidate), beforeCommit: _ =>
                {
                    prepared.Set();
                    release.Wait(timeout: TimeSpan.FromSeconds(seconds: 10), cancellationToken: cancellationToken)
                            .ShouldBeTrue();
                }, cancellationToken: cancellationToken), cancellationToken: cancellationToken);
        try
        {
            prepared.Wait(timeout: TimeSpan.FromSeconds(seconds: 10), cancellationToken: cancellationToken)
                    .ShouldBeTrue();
            store.Read().ShouldBeSameAs(expected: old);
            Should.Throw<InvalidOperationException>(actual: () => store.Restore(document: SnapshotCodec.Encode(
                    state: old), cancellationToken: cancellationToken)).Message.ShouldContain(expected: "RESTORE_BUSY");
            Task[] readers = [..Enumerable.Range(start: 0, count: 4).Select(selector: _ => Task.Run(action: () =>
                    Observe(store: store, started: started, observations: observations,
                            before: before, after: after, finished: finished.Token),
                    cancellationToken: cancellationToken))];
            started.Wait(timeout: TimeSpan.FromSeconds(seconds: 10), cancellationToken: cancellationToken)
                    .ShouldBeTrue();
            release.Set();
            ExampleState accepted = await writer.WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 10),
                    cancellationToken: cancellationToken);
            store.Read().ShouldBeSameAs(expected: accepted);
            finished.Cancel();
            await Task.WhenAll(tasks: readers).WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 10),
                    cancellationToken: cancellationToken);
            observations.ShouldContain(elementPredicate: item => StringComparer.Ordinal.Equals(x: item, y: before));
            observations.ShouldContain(elementPredicate: item => StringComparer.Ordinal.Equals(x: item, y: after));
            SnapshotProofData.Equal(actual: accepted, expected: candidate);
        }
        finally
        {
            release.Set();
            finished.Cancel();
            await writer.WaitAsync(timeout: TimeSpan.FromSeconds(seconds: 10), cancellationToken: cancellationToken);
        }
    }

    private static void Observe(ExampleStateStore store, CountdownEvent started,
            ConcurrentBag<string> observations, string before, string after, CancellationToken finished)
    {
        observations.Add(item: Fingerprint(state: store.Read()));
        started.Signal();
        do
        {
            ExampleState root = store.Read();
            string actual = Fingerprint(state: root);
            (StringComparer.Ordinal.Equals(x: actual, y: before) || StringComparer.Ordinal.Equals(x: actual, y: after))
                    .ShouldBeTrue(customMessage: "ATOMIC_COMPLETE_ROOT: " + actual);
        } while (!finished.IsCancellationRequested);
        observations.Add(item: Fingerprint(state: store.Read()));
    }

    private static string Fingerprint(ExampleState state)
    {
        string[] fields = [state.ElapsedMilliseconds.ToString(provider: CultureInfo.InvariantCulture),
            state.Seed.ToString(provider: CultureInfo.InvariantCulture),
            state.DrawPosition.ToString(provider: CultureInfo.InvariantCulture),
            state.RandomContractVersion, state.Kind.ToString(),
            string.Join(separator: ",", values: state.Ordered.Select(selector: item => item.Value)),
            string.Join(separator: ",", values: state.Records.OrderBy(keySelector: pair => pair.Key.Value,
                    comparer: StringComparer.Ordinal).Select(selector: pair => pair.Key.Value + "="
                            + pair.Value.ToString(provider: CultureInfo.InvariantCulture)))];
        return string.Join(separator: "|", value: fields);
    }
}
