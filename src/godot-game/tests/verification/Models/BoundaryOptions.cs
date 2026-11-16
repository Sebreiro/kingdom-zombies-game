using System;

namespace KingdomZombies.Phase1.Tests.Models;
/// <summary>Supplies a concrete validated configuration DTO without introducing a binder.</summary>
internal sealed class BoundaryOptions
{
    /// <summary>Requires a positive configured limit.</summary>
    internal BoundaryOptions(int limit)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value: limit);
        Limit = limit;
    }

    /// <summary>Gets the constructor-bound configuration value.</summary>
    internal int Limit { get; private set; }
}
