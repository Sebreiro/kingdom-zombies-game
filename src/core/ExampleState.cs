using System.Collections.Generic;

namespace KingdomZombies.Core;

/// <summary>Holds the minimal immutable non-game controlled-input proof state.</summary>
internal sealed class ExampleState
{
    /// <summary>Initializes explicit time, sequence and keyed values.</summary>
    internal ExampleState(long elapsedMilliseconds, IReadOnlyList<StableIdentity> ordered,
            IReadOnlyDictionary<StableIdentity, long> records)
    {
        ElapsedMilliseconds = elapsedMilliseconds;
        Ordered = ordered;
        Records = records;
    }

    /// <summary>Gets the exact integer elapsed total.</summary>
    internal long ElapsedMilliseconds { get; }
    /// <summary>Gets meaningful identity order.</summary>
    internal IReadOnlyList<StableIdentity> Ordered { get; }
    /// <summary>Gets unordered identity/value records.</summary>
    internal IReadOnlyDictionary<StableIdentity, long> Records { get; }

    /// <summary>Advances through explicit controlled inputs.</summary>
    internal ExampleState Advance(IElapsedInput elapsed, IRandomInput random)
    {
        return this;
    }
}
