using System;

namespace KingdomZombies.Phase1.Tests.Models;
/// <summary>Supplies a concrete validated, privately mutable architecture proof entity.</summary>
internal sealed class BoundaryEntity
{
    /// <summary>Requires meaningful state before constructing an entity.</summary>
    internal BoundaryEntity(int value, BoundaryKind kind)
    {
        if (value < 0 || kind is not BoundaryKind.Ready)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(value));
        }

        Value = value;
        Kind = kind;
    }

    /// <summary>Gets the controlled integer state.</summary>
    internal int Value { get; private set; }
    /// <summary>Gets the explicit nonzero state kind.</summary>
    internal BoundaryKind Kind { get; }

    /// <summary>Changes state only after validating the complete candidate.</summary>
    internal void Change(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value: value);
        Value = value;
    }
}
