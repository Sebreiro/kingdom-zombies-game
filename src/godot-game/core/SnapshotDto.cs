using System.Collections.Generic;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Carries detached immutable transport values without engine or transient connection references.</summary>
internal sealed class SnapshotDto
{
    /// <summary>Copies a complete validated state into transport values.</summary>
    internal SnapshotDto(ExampleState state)
    {
        ElapsedMilliseconds = state.ElapsedMilliseconds;
        Seed = state.Seed;
        DrawPosition = state.DrawPosition;
        RandomContractVersion = state.RandomContractVersion;
        Kind = SnapshotCodec.SampleWireName;
        Ordered = System.Array.AsReadOnly(array: state.Ordered.Select(selector: identity => identity.Value).ToArray());
        Records = System.Array.AsReadOnly(array: state.Records.Select(selector: pair =>
                new KeyValuePair<string, long>(key: pair.Key.Value, value: pair.Value)).ToArray());
    }

    /// <summary>Gets exact elapsed milliseconds.</summary>
    internal long ElapsedMilliseconds { get; }
    /// <summary>Gets the persisted seed.</summary>
    internal int Seed { get; }
    /// <summary>Gets the persisted position.</summary>
    internal long DrawPosition { get; }
    /// <summary>Gets the exact operation contract.</summary>
    internal string RandomContractVersion { get; }
    /// <summary>Gets the exact enum name.</summary>
    internal string Kind { get; }
    /// <summary>Gets meaningful ordered identities.</summary>
    internal IReadOnlyList<string> Ordered { get; }
    /// <summary>Gets unordered independent identity/value pairs.</summary>
    internal IReadOnlyList<KeyValuePair<string, long>> Records { get; }
}
