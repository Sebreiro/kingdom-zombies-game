using System.Collections.Generic;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

/// <summary>Supplies controlled discovery changes to the nonce-scoped native regression subject.</summary>
internal sealed class FoundationRegressionDiscovery : IDiscoveryService
{
    public string Generation => "native-regression";
    public string FailureReason => "";
    public void Advertise(SessionAdvertisement? advertisement) { }
    public void Refresh() { }
    public IReadOnlyList<SessionAdvertisement> Update(long nowMilliseconds) { return Rows; }
    public void Dispose() { }

    internal IReadOnlyList<SessionAdvertisement> Rows { get; set; } = [];
}
