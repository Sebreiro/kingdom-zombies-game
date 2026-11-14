using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Holds the minimal immutable non-game controlled-input proof state.</summary>
internal sealed class ExampleState
{
    /// <summary>Initializes explicit time, sequence and keyed values.</summary>
    internal ExampleState(long elapsedMilliseconds, IReadOnlyList<StableIdentity> ordered,
            IEnumerable<KeyValuePair<StableIdentity, long>> records, ProofKind kind = ProofKind.Sample,
            int seed = 0, long drawPosition = 0, string randomContractVersion = ReplayRandom.PinnedRuntimeContract)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
        ArgumentNullException.ThrowIfNull(argument: ordered);
        ArgumentNullException.ThrowIfNull(argument: records);
        ValidateRandom(drawPosition: drawPosition, randomContractVersion: randomContractVersion);
        if (kind is not ProofKind.Sample)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(kind), message: "STATE_KIND: Sample required");
        }

        StableIdentity[] sequence = [.. ordered];
        Dictionary<StableIdentity, long> values = ValidateRecords(records: records);
        ValidateOrder(ordered: sequence, records: values);
        ElapsedMilliseconds = elapsedMilliseconds;
        Ordered = Array.AsReadOnly(array: sequence);
        Records = new ReadOnlyDictionary<StableIdentity, long>(dictionary: values);
        Kind = kind;
        Seed = seed;
        DrawPosition = drawPosition;
        RandomContractVersion = randomContractVersion;
    }

    /// <summary>Gets the exact integer elapsed total.</summary>
    internal long ElapsedMilliseconds { get; }
    /// <summary>Gets meaningful identity order.</summary>
    internal IReadOnlyList<StableIdentity> Ordered { get; }
    /// <summary>Gets unordered identity/value records.</summary>
    internal IReadOnlyDictionary<StableIdentity, long> Records { get; }
    /// <summary>Gets the explicit supported proof-state kind.</summary>
    internal ProofKind Kind { get; }
    /// <summary>Gets the persisted explicit seed.</summary>
    internal int Seed { get; }
    /// <summary>Gets the accepted continuation position.</summary>
    internal long DrawPosition { get; }
    /// <summary>Gets the exact runtime operation contract.</summary>
    internal string RandomContractVersion { get; }

    /// <summary>Advances through explicit controlled inputs.</summary>
    internal ExampleState Advance(IElapsedInput elapsed, IRandomInput random)
    {
        ArgumentNullException.ThrowIfNull(argument: elapsed);
        ArgumentNullException.ThrowIfNull(argument: random);
        long delta = elapsed.Milliseconds;
        ArgumentOutOfRangeException.ThrowIfNegative(value: delta, paramName: nameof(elapsed));
        long total = checked(ElapsedMilliseconds + delta);
        if (delta == 0)
        {
            return this;
        }

        var values = new Dictionary<StableIdentity, long>(collection: Records);
        long draw = random.Draw();
        if (draw is < 0 or long.MaxValue)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(random),
                    message: "RANDOM_RANGE: expected 0 <= draw < long.MaxValue");
        }

        if (Ordered.Count > 0)
        {
            values[Ordered[0]] = draw;
        }

        return new ExampleState(elapsedMilliseconds: total, ordered: Ordered, records: values, kind: Kind,
                seed: random is ReplayRandom replay ? replay.Seed : Seed, drawPosition: random.DrawPosition,
                randomContractVersion: RandomContractVersion);
    }

    /// <summary>Parses the integer-only input boundary without floating point or culture-dependent rounding.</summary>
    internal static long ParseMilliseconds(string text)
    {
        ArgumentNullException.ThrowIfNull(argument: text);
        if (text.Length == 0 || text.Any(predicate: unit => unit is < '0' or > '9')
                || !long.TryParse(s: text, style: NumberStyles.None, provider: CultureInfo.InvariantCulture,
                        result: out long value))
        {
            throw new FormatException(message: "ELAPSED_INTEGER: expected 0..long.MaxValue milliseconds");
        }

        return value;
    }

    private static Dictionary<StableIdentity, long> ValidateRecords(
            IEnumerable<KeyValuePair<StableIdentity, long>> records)
    {
        var values = new Dictionary<StableIdentity, long>();
        foreach (KeyValuePair<StableIdentity, long> record in records)
        {
            if (values.Count == SnapshotCodec.MaxRecords)
            {
                throw new ArgumentException(message: "SNAPSHOT_RECORD_LIMIT: at most 1024 records",
                        paramName: nameof(records));
            }

            ArgumentNullException.ThrowIfNull(argument: record.Key, paramName: nameof(records));
            ArgumentOutOfRangeException.ThrowIfNegative(value: record.Value, paramName: nameof(records));
            if (!values.TryAdd(key: record.Key, value: record.Value))
            {
                throw new ArgumentException(message: "STATE_DUPLICATE: identity already present",
                        paramName: nameof(records));
            }
        }

        return values;
    }

    private static void ValidateRandom(long drawPosition, string randomContractVersion)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value: drawPosition);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value: drawPosition, other: ReplayRandom.MaxDrawPosition);
        if (!StringComparer.Ordinal.Equals(x: randomContractVersion, y: ReplayRandom.PinnedRuntimeContract))
        {
            throw new ArgumentException(message: "RANDOM_CONTRACT: pinned version required",
                    paramName: nameof(randomContractVersion));
        }
    }

    private static void ValidateOrder(StableIdentity[] ordered, Dictionary<StableIdentity, long> records)
    {
        var seen = new HashSet<StableIdentity>();
        foreach (StableIdentity identity in ordered)
        {
            ArgumentNullException.ThrowIfNull(argument: identity, paramName: nameof(ordered));
            if (!seen.Add(item: identity) || !records.ContainsKey(key: identity))
            {
                throw new ArgumentException(message: "STATE_ORDER: unique known identities required",
                        paramName: nameof(ordered));
            }
        }

        if (seen.Count != records.Count)
        {
            throw new ArgumentException(message: "STATE_ORDER: every record needs an ordered identity",
                    paramName: nameof(ordered));
        }
    }
}
