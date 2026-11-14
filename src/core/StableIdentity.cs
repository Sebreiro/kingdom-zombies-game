namespace KingdomZombies.Core;

/// <summary>Represents an exact identity in the minimal non-game proof.</summary>
internal sealed class StableIdentity
{
    /// <summary>Initializes the identity from explicit text.</summary>
    internal StableIdentity(string value)
    {
        Value = value;
    }

    /// <summary>Gets the exact supplied text.</summary>
    internal string Value { get; }
}
