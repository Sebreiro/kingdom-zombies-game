using System.Collections.Generic;

namespace KingdomZombies.Multiplayer.Abstractions;

/// <summary>Owns selected stable local identities without engine or gameplay dependencies.</summary>
public interface IIdentityService
{
    /// <summary>Gets profiles published by the last successful storage operation.</summary>
    IReadOnlyList<PlayerProfile> Profiles { get; }
    /// <summary>Gets the selected usable profile or no identity after a load failure.</summary>
    PlayerProfile? Selected { get; }
    /// <summary>Gets a recoverable player-facing reason.</summary>
    string FailureReason { get; }
    /// <summary>Loads identities without silently replacing unreadable existing bytes.</summary>
    bool Load();
    /// <summary>Creates and selects a new independent identity explicitly.</summary>
    bool Create();
    /// <summary>Persists selection of an exact identity.</summary>
    bool Choose(string playerId);
    /// <summary>Commits a nonblank Unicode label without changing ownership.</summary>
    bool Rename(string displayName);
}
