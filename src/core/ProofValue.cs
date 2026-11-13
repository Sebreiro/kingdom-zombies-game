namespace KingdomZombies.Core;

/// <summary>Provides an identity-free immutable value for boundary composition proofs.</summary>
public readonly struct ProofValue
{
    /// <summary>Gets the proof's integer value.</summary>
    public int Value { get; }

    /// <summary>Initializes a boundary proof value.</summary>
    /// <param name="value">The value to preserve across the boundary.</param>
    public ProofValue(int value)
    {
        this.Value = value;
    }
}
