using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises ordinary service behavior through the production adapters.</summary>
public sealed class FoundationLanServicesTests
{
    /// <summary>Manual addresses use the same bounded numeric IPv4 grammar as transport.</summary>
    [Fact]
    public void DiscoveryAndManualEndpointContract()
    {
        FoundationEndpoint.TryParse(value: "127.0.0.1:1", address: out string address, port: out int port)
                .ShouldBeTrue();
        address.ShouldBe(expected: "127.0.0.1");
        port.ShouldBe(expected: 1);
        FoundationEndpoint.TryParse(value: "127.0.0.1", address: out _, port: out port).ShouldBeTrue();
        port.ShouldBe(expected: 47778);
        foreach (string invalid in new[] { "::1", "localhost", "127.1", "127.0.0.1:0", "127.0.0.1:65536" })
        {
            FoundationEndpoint.TryParse(value: invalid, address: out _, port: out _).ShouldBeFalse();
        }
    }
}
