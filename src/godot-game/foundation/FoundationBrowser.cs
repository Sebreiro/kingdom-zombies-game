using System;
using System.Collections.Generic;
using System.Linq;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

/// <summary>Keeps selection attached to exact sessions across reorder, expiry and provider replacement.</summary>
internal sealed class FoundationBrowser
{
    internal IReadOnlyList<SessionAdvertisement> Rows { get; private set; } = [];
    internal string SelectedId { get; private set; } = "";
    internal SessionAdvertisement? Selected => Rows.FirstOrDefault(predicate: row =>
            StringComparer.Ordinal.Equals(x: row.SessionId, y: SelectedId));
    internal string Feedback { get; private set; } = "Searching for LAN sessions…";

    internal void Replace(IDiscoveryService service)
    {
        _generation = service.Generation;
        SelectedId = "";
        Rows = [];
    }

    internal bool Publish(string generation, IReadOnlyList<SessionAdvertisement> rows, string error)
    {
        if (!StringComparer.Ordinal.Equals(x: generation, y: _generation))
        {
            return false;
        }

        Rows = rows.DistinctBy(keySelector: row => row.SessionId, comparer: StringComparer.Ordinal)
                .OrderBy(keySelector: row => row.SessionName, comparer: StringComparer.Ordinal)
                .ThenBy(keySelector: row => row.SessionId, comparer: StringComparer.Ordinal).ToArray();
        bool vanished = SelectedId.Length > 0 && Selected is null;
        if (vanished)
        {
            SelectedId = "";
        }

        Feedback = vanished ? "This session is no longer available. Refresh or enter a host address."
                : error.Length > 0 ? error : Rows.Count == 0
                        ? "No LAN sessions found\nRefresh to search again, or enter a host address below."
                        : Rows.Count == 1 ? "1 session" : Rows.Count + " sessions";
        return true;
    }

    internal void Select(string sessionId)
    {
        SelectedId = Rows.Any(predicate: row => StringComparer.Ordinal.Equals(x: row.SessionId, y: sessionId))
                ? sessionId : "";
    }

    private string _generation = "";
}
