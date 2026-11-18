using System;

namespace KingdomZombies.Inspection;

/// <summary>Projects session inspection resources without running session transactions.</summary>
internal sealed class SessionLayouts
{
    /// <summary>Returns the authored scope statement for a session departure example.</summary>
    internal static string Departure(bool host)
    {
        return host ? "End session?" : "Leave session?";
    }
}
