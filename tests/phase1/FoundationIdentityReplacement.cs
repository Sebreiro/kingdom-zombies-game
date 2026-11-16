using System.Collections.Generic;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Supplies a detached identity result independently from discovery and transport providers.</summary>
internal sealed class FoundationIdentityReplacement : IIdentityService
{
    public FoundationIdentityReplacement(PlayerProfile profile, string failureReason = "")
    {
        Selected = profile;
        FailureReason = failureReason;
    }

    public IReadOnlyList<PlayerProfile> Profiles => [Selected];
    public PlayerProfile Selected { get; }
    public string FailureReason { get; }
    public bool Load() { return FailureReason.Length == 0; }
    public bool Create() { return false; }
    public bool Choose(string playerId) { return false; }
    public bool Rename(string displayName) { return false; }
}
