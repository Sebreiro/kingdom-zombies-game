using System;

namespace KingdomZombies.Core;

/// <summary>Represents an exact identity in the minimal non-game proof.</summary>
internal sealed class StableIdentity : IEquatable<StableIdentity>
{
    /// <summary>Compares exact UTF-16 text with ordinal semantics.</summary>
    public bool Equals(StableIdentity? other)
    {
        return other is not null && StringComparer.Ordinal.Equals(x: Value, y: other.Value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is StableIdentity identity && Equals(other: identity);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return StringComparer.Ordinal.GetHashCode(obj: Value);
    }

    /// <summary>Initializes the identity from explicit text.</summary>
    internal StableIdentity(string value)
    {
        ArgumentNullException.ThrowIfNull(argument: value);
        if (value.Length is < 1 or > 128)
        {
            throw new ArgumentException(message: "IDENTITY_LENGTH: expected 1..128 UTF-16 units",
                    paramName: nameof(value));
        }

        ValidateText(value: value);
        Value = value;
    }

    /// <summary>Gets the exact supplied text.</summary>
    internal string Value { get; }

    private static void ValidateText(string value)
    {
        for (int index = 0; index < value.Length; index++)
        {
            char unit = value[index];
            if (char.IsControl(c: unit) || char.IsLowSurrogate(c: unit))
            {
                throw new ArgumentException(message: "IDENTITY_TEXT: control or unpaired surrogate",
                        paramName: nameof(value));
            }

            if (!char.IsHighSurrogate(c: unit))
            {
                continue;
            }

            if (index + 1 >= value.Length || !char.IsLowSurrogate(c: value[index + 1]))
            {
                throw new ArgumentException(message: "IDENTITY_TEXT: unpaired surrogate", paramName: nameof(value));
            }

            index++;
        }
    }
}
