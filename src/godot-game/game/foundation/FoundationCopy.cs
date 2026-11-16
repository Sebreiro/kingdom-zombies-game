using System;

namespace KingdomZombies.Foundation;

/// <summary>Maps developer protocol reasons to the approved recoverable English player copy.</summary>
internal static class FoundationCopy
{
    internal static string Error(string reason)
    {
        if (reason.StartsWith(value: "Full", comparisonType: StringComparison.Ordinal))
        {
            return "This session is full (4/4). Choose another session or try again later.";
        }
        if (reason.StartsWith(value: "Duplicate profile", comparisonType: StringComparison.Ordinal))
        {
            return "This profile is already connected. Go Back and choose or create another local profile.";
        }
        if (reason.StartsWith(value: "Incompatible", comparisonType: StringComparison.Ordinal))
        {
            return "This session uses an incompatible game version. "
                    + "Use the same game build as the host, then try again.";
        }
        if (reason.StartsWith(value: "Join timed out", comparisonType: StringComparison.Ordinal))
        {
            return "Joining timed out. Check the host address and try again.";
        }
        if (reason.StartsWith(value: "Connection lost", comparisonType: StringComparison.Ordinal))
        {
            return "Connection to the host was lost. Choose Join LAN to try again, or start a new session.";
        }
        if (reason.StartsWith(value: "The host ended", comparisonType: StringComparison.Ordinal))
        {
            return "The host ended the session. Choose Start, Host session or Join LAN to play again.";
        }

        return reason;
    }
}
