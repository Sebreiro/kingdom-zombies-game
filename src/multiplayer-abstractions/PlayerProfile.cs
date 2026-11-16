namespace KingdomZombies.Multiplayer.Abstractions;

/// <summary>Publishes immutable opaque ownership independently of its editable display label.</summary>
public sealed class PlayerProfile
{
    /// <summary>Creates a detached selected local profile.</summary>
    public PlayerProfile(string playerId, string displayName, int ordinal)
    {
        PlayerId = playerId;
        DisplayName = displayName;
        Ordinal = ordinal;
    }

    /// <summary>Gets the exact case-sensitive opaque ownership identity.</summary>
    public string PlayerId { get; }
    /// <summary>Gets the full Unicode display label.</summary>
    public string DisplayName { get; }
    /// <summary>Gets a presentation ordinal, never an ownership key.</summary>
    public int Ordinal { get; }
}
