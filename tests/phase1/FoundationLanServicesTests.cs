using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KingdomZombies.Foundation;
using KingdomZombies.Multiplayer.Abstractions;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises ordinary service behavior through the production adapters.</summary>
public sealed class FoundationLanServicesTests
{
    /// <summary>Manual addresses use the same bounded numeric IPv4 grammar as transport.</summary>
    [Fact]
    public async Task Endpoints_DiscoveryAndManualQueries_FollowContract()
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

        CheckBrowser();
        await RealMulticastFanout();
        ProofProtocol.Record(id: "adapters.discovery", diagnostic: "IPV4_SELECTION_GENERATION_EXPIRY_CODEC");
    }

    /// <summary>Restart, equal names, writer contention and failed publication preserve exact ownership.</summary>
    [Fact]
    public void Profiles_PersistedValues_AreAtomicAndExact()
    {
        string directory = Path.Combine(path1: ProofProtocol.Root, path2: ".phase3/profile-tests/"
                + Guid.NewGuid().ToString(format: "N"));
        var service = new LocalProfileService(directory: directory);
        service.Load().ShouldBeTrue();
        string first = service.Selected?.PlayerId ?? throw new InvalidOperationException(message: "PROFILE");
        service.Rename(displayName: "Élodie 生存者").ShouldBeTrue();
        var restarted = new LocalProfileService(directory: directory);
        restarted.Load().ShouldBeTrue();
        restarted.Selected?.PlayerId.ShouldBe(expected: first);
        restarted.Selected?.DisplayName.ShouldBe(expected: "Élodie 生存者");
        restarted.Create().ShouldBeTrue();
        restarted.Rename(displayName: "Élodie 生存者").ShouldBeTrue();
        string fullName = string.Concat(values: Enumerable.Repeat(element: "生存者😀", count: 128));
        restarted.Rename(displayName: fullName).ShouldBeTrue();
        var longRestart = new LocalProfileService(directory: directory);
        longRestart.Load().ShouldBeTrue();
        longRestart.Selected?.DisplayName.ShouldBe(expected: fullName);
        restarted.Selected?.PlayerId.ShouldNotBe(expected: first, comparer: StringComparer.Ordinal);
        restarted.Choose(playerId: first.ToUpperInvariant()).ShouldBeFalse();
        restarted.Choose(playerId: first).ShouldBeTrue();
        restarted.Rename(displayName: " \t").ShouldBeFalse();
        restarted.Rename(displayName: "Élodie 生存者").ShouldBeTrue();
        CheckProfileFailures(directory: directory, first: first);
        ProofProtocol.Record(id: "adapters.profiles", diagnostic: "SCHEMA1_ATOMIC_LOCKED_EXACT_RESTART_RECOVERY");
    }

    /// <summary>An unavailable selected provider cannot bind cached ownership into a new coordinator.</summary>
    [Fact]
    public void Providers_IndependentServices_UseProductionComposition()
    {
        var identity = new FoundationIdentityReplacement(profile: new PlayerProfile(
                playerId: Guid.NewGuid().ToString(format: "N"), displayName: "Equal", ordinal: 1),
                failureReason: "Identity unavailable");
        using var discovery = new LanDiscoveryService();
        var services = new FoundationServices(identity: identity, discovery: discovery,
                connection: () => new FoundationHarnessConnection());
        Should.Throw<InvalidOperationException>(actual: () => services.Create(connected: true,
                nowMilliseconds: () => 0)).Message.ShouldBe(expected: "Identity unavailable");
        FoundationServiceSubstitutionProof.Run();
    }

    internal static SessionAdvertisement Advertisement(string id)
    {
        return new SessionAdvertisement(sessionId: id, sessionName: "Equal 生存者", hostLabel: "Equal",
                address: "127.0.0.1", port: 47778, occupied: 1, availability: "Available", protocol: 1,
                content: FoundationProtocol.Content);
    }

    private static async Task RealMulticastFanout()
    {
        using var host1 = new LanDiscoveryService();
        using var host2 = new LanDiscoveryService();
        using var browser1 = new LanDiscoveryService();
        using var browser2 = new LanDiscoveryService();
        string first = Guid.NewGuid().ToString(format: "N");
        string second = Guid.NewGuid().ToString(format: "N");
        host1.Advertise(advertisement: Advertisement(id: first));
        host2.Advertise(advertisement: Advertisement(id: second));
        var clock = System.Diagnostics.Stopwatch.StartNew();
        bool both = false;
        while (clock.ElapsedMilliseconds < 5000 && !both)
        {
            host1.Update(nowMilliseconds: clock.ElapsedMilliseconds);
            host2.Update(nowMilliseconds: clock.ElapsedMilliseconds);
            SessionAdvertisement[] one = [.. browser1.Update(nowMilliseconds: clock.ElapsedMilliseconds)];
            SessionAdvertisement[] two = [.. browser2.Update(nowMilliseconds: clock.ElapsedMilliseconds)];
            both = HasBoth(rows: one, first: first, second: second)
                    && HasBoth(rows: two, first: first, second: second);
            await Task.Delay(millisecondsDelay: 10, cancellationToken: TestContext.Current.CancellationToken);
        }

        both.ShouldBeTrue(customMessage: host1.FailureReason + host2.FailureReason
                + browser1.FailureReason + browser2.FailureReason);
        host1.Advertise(advertisement: null);
        host2.Advertise(advertisement: null);
        browser1.Update(nowMilliseconds: 10000).Any(predicate: row => StringComparer.Ordinal.Equals(
                x: row.SessionId, y: first)).ShouldBeFalse();
    }

    private static bool HasBoth(SessionAdvertisement[] rows, string first, string second)
    {
        return rows.Any(predicate: row => StringComparer.Ordinal.Equals(x: row.SessionId, y: first))
                && rows.Any(predicate: row => StringComparer.Ordinal.Equals(x: row.SessionId, y: second));
    }

    private static void CheckBrowser()
    {
        using var discovery = new LanDiscoveryService();
        var browser = new FoundationBrowser();
        browser.Replace(service: discovery);
        SessionAdvertisement one = Advertisement(id: "one");
        SessionAdvertisement two = Advertisement(id: "two");
        browser.Publish(generation: discovery.Generation, rows: [one, one, two], error: "").ShouldBeTrue();
        browser.Rows.Count.ShouldBe(expected: 2);
        browser.Select(sessionId: "two");
        browser.Publish(generation: discovery.Generation, rows: [two, one], error: "");
        browser.Selected?.SessionId.ShouldBe(expected: "two");
        browser.Publish(generation: "obsolete", rows: [], error: "late").ShouldBeFalse();
        browser.Selected?.SessionId.ShouldBe(expected: "two");
        browser.Publish(generation: discovery.Generation, rows: [one], error: "");
        browser.Selected.ShouldBeNull();
        browser.Feedback.ShouldStartWith(expected: "This session is no longer available.");
        browser.Publish(generation: discovery.Generation, rows: [one], error: "");
        browser.Feedback.ShouldStartWith(expected: "This session is no longer available.");
        browser.Refresh();
        browser.Publish(generation: discovery.Generation, rows: [one], error: "");
        browser.Feedback.ShouldBe(expected: "1 session");
        DiscoveryCodec.Decode(bytes: DiscoveryCodec.Encode(advertisement: one), address: "192.168.1.2")
                ?.Address.ShouldBe(expected: "192.168.1.2");
        DiscoveryCodec.Decode(bytes: new byte[4097], address: "127.0.0.1").ShouldBeNull();
        DiscoveryCodec.Decode(bytes: "{}"u8.ToArray(), address: "127.0.0.1").ShouldBeNull();
        string longName = string.Concat(values: Enumerable.Repeat(element: "生存者😀", count: 128));
        FoundationDisplay.Label(value: longName).EndsWith(value: '…')
                .ShouldBeTrue();
        var longAdvertisement = new SessionAdvertisement(sessionId: "long", sessionName: FoundationDisplay.Label(
                value: longName), hostLabel: FoundationDisplay.Label(value: longName), address: "127.0.0.1",
                port: 47778, occupied: 4, availability: "Full", protocol: 1, content: FoundationProtocol.Content);
        DiscoveryCodec.Encode(advertisement: longAdvertisement).Length.ShouldBeLessThanOrEqualTo(expected: 4096);
    }

    private static void CheckProfileFailures(string directory, string first)
    {
        string path = Path.Combine(path1: directory, path2: "profiles.json");
        byte[] before = File.ReadAllBytes(path: path);
        var failed = new LocalProfileService(directory: directory, publish: (_, _) =>
                throw new IOException(message: "Injected atomic-publication failure"));
        failed.Load().ShouldBeTrue();
        failed.Rename(displayName: "Changed").ShouldBeFalse();
        failed.Selected?.PlayerId.ShouldBe(expected: first);
        File.ReadAllBytes(path: path).ShouldBe(expected: before);
        var bounded = new LocalProfileService(directory: directory);
        bounded.Load().ShouldBeTrue();
        bounded.Rename(displayName: new string(c: '界', count: 1048576)).ShouldBeFalse();
        File.ReadAllBytes(path: path).ShouldBe(expected: before);
        using (var writer = new FileStream(path: Path.Combine(path1: directory, path2: "profiles.lock"),
                mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.None))
        {
            failed.Create().ShouldBeFalse();
            File.ReadAllBytes(path: path).ShouldBe(expected: before);
        }

        foreach (string corrupt in new[] { "{", "{\"schema\":1,\"selected\":\"bad\",\"profiles\":[]}",
            "{\"schema\":1,\"selected\":\"bad\",\"profiles\":[{\"name\":\"Name\",\"ordinal\":1}]}" })
        {
            File.WriteAllText(path: path, contents: corrupt);
            var broken = new LocalProfileService(directory: directory);
            broken.Load().ShouldBeFalse();
            broken.Selected.ShouldBeNull();
            File.ReadAllText(path: path).ShouldBe(expected: corrupt);
            broken.Create().ShouldBeTrue();
            broken.Selected?.PlayerId.ShouldNotBe(expected: first, comparer: StringComparer.Ordinal);
            Directory.GetFiles(path: directory, searchPattern: "profiles.corrupt.*.json").Length.ShouldBeGreaterThan(
                    expected: 0);
        }
    }
}
