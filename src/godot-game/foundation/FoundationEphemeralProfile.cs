using System;

namespace KingdomZombies.Foundation;

/// <summary>Gives ordinary processes distinct temporary identities until the profile provider is composed.</summary>
internal static class FoundationEphemeralProfile
{
    internal static string PlayerId { get; } = Guid.NewGuid().ToString(format: "N");
}
