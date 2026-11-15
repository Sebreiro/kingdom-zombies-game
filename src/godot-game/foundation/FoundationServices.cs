using System;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

/// <summary>Composes three independent service ports into the existing production coordinator.</summary>
internal sealed class FoundationServices
{
    internal FoundationServices(IIdentityService identity, IDiscoveryService discovery,
            Func<IConnectionService> connection)
    {
        Identity = identity;
        Discovery = discovery;
        _connection = connection;
        Browser.Replace(service: discovery);
    }

    internal IIdentityService Identity { get; }
    internal IDiscoveryService Discovery { get; private set; }
    internal FoundationBrowser Browser { get; } = new();

    internal FoundationCoordinator Create(bool connected, Func<long> nowMilliseconds)
    {
        if (Identity.FailureReason.Length > 0)
        {
            throw new InvalidOperationException(message: Identity.FailureReason);
        }

        PlayerProfile profile = Identity.Selected
                ?? throw new InvalidOperationException(message: Identity.FailureReason);
        return new FoundationCoordinator(playerId: profile.PlayerId, label: FoundationDisplay.Label(
                value: profile.DisplayName),
                connection: connected ? _connection() : null, nowMilliseconds: nowMilliseconds);
    }

    internal void ReplaceDiscovery(IDiscoveryService discovery)
    {
        Discovery.Dispose();
        Discovery = discovery;
        Browser.Replace(service: discovery);
    }

    internal void Update(long nowMilliseconds)
    {
        Browser.Publish(generation: Discovery.Generation, rows: Discovery.Update(nowMilliseconds: nowMilliseconds),
                error: Discovery.FailureReason);
    }

    private readonly Func<IConnectionService> _connection;
}
