using System;
using System.Collections.Generic;

namespace KingdomZombies.Multiplayer.Abstractions;

/// <summary>Provides independently replaceable bounded LAN discovery.</summary>
public interface IDiscoveryService : IDisposable
{
    /// <summary>Gets the exact provider lifetime for obsolete-result fencing.</summary>
    string Generation { get; }
    /// <summary>Gets a recoverable discovery reason; manual joining remains independent.</summary>
    string FailureReason { get; }
    /// <summary>Publishes or withdraws this host's current session.</summary>
    void Advertise(SessionAdvertisement? advertisement);
    /// <summary>Queries immediately without clearing valid rows.</summary>
    void Refresh();
    /// <summary>Performs bounded nonblocking work and expires old advertisements.</summary>
    IReadOnlyList<SessionAdvertisement> Update(long nowMilliseconds);
}
