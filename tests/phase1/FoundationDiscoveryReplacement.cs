using System;
using System.Collections.Generic;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Replaces only discovery publications while the production browser owns selection and fencing.</summary>
internal sealed class FoundationDiscoveryReplacement : IDiscoveryService
{
    public string Generation { get; } = Guid.NewGuid().ToString(format: "N");
    public string FailureReason { get; set; } = "";
    public void Advertise(SessionAdvertisement? advertisement) { Advertisement = advertisement; }
    public void Refresh() { Refreshes++; }
    public IReadOnlyList<SessionAdvertisement> Update(long nowMilliseconds) { return Rows; }
    public void Dispose() { Disposed = true; }
    internal IReadOnlyList<SessionAdvertisement> Rows { get; set; } = [];
    internal SessionAdvertisement? Advertisement { get; private set; }
    internal int Refreshes { get; private set; }
    internal bool Disposed { get; private set; }
}
