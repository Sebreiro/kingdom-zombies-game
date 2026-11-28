using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using KingdomZombies.Multiplayer.Abstractions;
using Shouldly;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Runs separate provider substitutions through actual production composition and wire admission.</summary>
internal static class FoundationServiceSubstitutionProof
{
    internal static void Run()
    {
        Dictionary<string, string> before = CoreHashes();
        string root = Path.Combine(path1: WorkspacePaths.Temp(purpose: "lan-checks"),
                path2: "substitution-profiles", path3: Guid.NewGuid().ToString(format: "N"));
        var persisted = new LocalProfileService(directory: root);
        persisted.Load().ShouldBeTrue();
        persisted.Rename(displayName: "Equal 生存者").ShouldBeTrue();
        PlayerProfile profile = persisted.Selected ?? throw new InvalidOperationException(message: "PROFILE");
        CheckDiscovery(identity: persisted, profile: profile);
        CheckIdentity(identity: persisted, profile: profile);
        CheckConnection(identity: persisted, profile: profile);
        CoreHashes().ShouldBe(expected: before);
        string? directory = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY");
        if (directory is not null)
        {
            string artifact = Path.Combine(path1: directory, path2: "adapters.substitutions.core-hashes.json");
            File.WriteAllText(path: artifact, contents: JsonSerializer.Serialize(value: new
            {
                before,
                after = CoreHashes(),
                substitutions = _substitutions,
                productionComposition = "FoundationServices.Create → FoundationCoordinator",
                realLanCredit = false,
            }));
            ProofProtocol.Record(id: "adapters.substitutions", diagnostic: "THREE_INDEPENDENT_PRODUCTION_PORT_SWAPS",
                    artifacts: [artifact]);
        }
    }

    private static readonly string[] _substitutions = ["discovery", "identity", "connection"];

    private static void CheckDiscovery(IIdentityService identity, PlayerProfile profile)
    {
        using var original = new LanDiscoveryService();
        var port = new FoundationHarnessConnection();
        var services = new FoundationServices(identity: identity, discovery: original, connection: () => port);
        using var replacement = new FoundationDiscoveryReplacement
        {
            Rows = [FoundationLanServicesTests.Advertisement(id: "replacement-session")],
        };
        string oldGeneration = original.Generation;
        services.ReplaceDiscovery(discovery: replacement);
        services.Update(nowMilliseconds: 0);
        services.Browser.Select(sessionId: "replacement-session");
        SessionAdvertisement selected = services.Browser.Selected
                ?? throw new InvalidOperationException(message: "SELECTION");
        FoundationCoordinator guest = services.Create(connected: true, nowMilliseconds: () => 0);
        guest.Join(address: selected.Address, port: selected.Port);
        ReadyAndMove(guest: guest, guestPort: port, profile: profile);
        services.Browser.Publish(generation: oldGeneration, rows: [], error: "old provider error").ShouldBeFalse();
        services.Browser.Selected?.SessionId.ShouldBe(expected: "replacement-session");
        replacement.FailureReason = "Discovery unavailable";
        replacement.Rows = [];
        services.Update(nowMilliseconds: 1);
        services.Browser.Feedback.ShouldNotBeEmpty();
        FoundationEndpoint.TryParse(value: "127.0.0.1:47778", address: out _, port: out _).ShouldBeTrue();
        guest.Close();
        FoundationCoordinator manual = services.Create(connected: true, nowMilliseconds: () => 0);
        port.Sent.Clear();
        manual.Join(address: "127.0.0.1", port: 47778);
        ReadyAndMove(guest: manual, guestPort: port, profile: profile);
        manual.Ready.ShouldBeTrue();
        manual.Close();
        var unavailable = new FoundationServices(identity: identity, discovery: replacement,
                connection: () => throw new InvalidOperationException(message: "Connection unavailable"));
        Should.Throw<InvalidOperationException>(actual: () => unavailable.Create(connected: true,
                nowMilliseconds: () => 0)).Message.ShouldBe(expected: "Connection unavailable");
    }

    private static void CheckIdentity(LocalProfileService identity, PlayerProfile profile)
    {
        using var discovery = new LanDiscoveryService();
        var port = new FoundationHarnessConnection();
        var replacement = new FoundationIdentityReplacement(profile: profile);
        var services = new FoundationServices(identity: replacement, discovery: discovery, connection: () => port);
        FoundationCoordinator guest = services.Create(connected: true, nowMilliseconds: () => 0);
        guest.Join(address: "127.0.0.1", port: 50001);
        ReadyAndMove(guest: guest, guestPort: port, profile: profile);
        guest.PlayerId.ShouldBe(expected: identity.Selected?.PlayerId);
        guest.Close();
        CheckEqualNames(profile: profile);
    }

    private static void CheckConnection(IIdentityService identity, PlayerProfile profile)
    {
        using var discovery = new LanDiscoveryService();
        var replacement = new FoundationConnectionReplacement();
        var services = new FoundationServices(identity: identity, discovery: discovery, connection: () => replacement);
        FoundationCoordinator guest = services.Create(connected: true, nowMilliseconds: () => 0);
        guest.Join(address: "127.0.0.1", port: 50002);
        ReadyAndMove(guest: guest, guestPort: replacement.Subject, profile: profile);
        FoundationSnapshot before = guest.Snapshot ?? throw new InvalidOperationException(message: "SNAPSHOT");
        replacement.Subject.Enqueue(sender: 1, envelope: new FoundationEnvelope(kind: "reject", label: "obsolete"),
                generation: "old-adapter");
        guest.Update(elapsedMilliseconds: 0);
        guest.Snapshot.ShouldBeSameAs(expected: before);
        guest.Error.ShouldBeEmpty();
        guest.Close();
        replacement.Subject.CloseCount.ShouldBe(expected: 1);
    }

    private static void ReadyAndMove(FoundationCoordinator guest, FoundationHarnessConnection guestPort,
            PlayerProfile profile)
    {
        var hostPort = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: hostPort);
        host.Start(host: true);
        guest.Update(elapsedMilliseconds: 0);
        FoundationEnvelope hello = guestPort.Sent.Single(predicate: row => row.Envelope.Kind is "hello").Envelope;
        hello.PlayerId.ShouldBe(expected: profile.PlayerId);
        hello.Label.ShouldBe(expected: profile.DisplayName);
        hostPort.Enqueue(sender: 7, envelope: hello);
        host.Update(elapsedMilliseconds: 0);
        FoundationEnvelope baseline = hostPort.Sent.Last(predicate: row => row.Envelope.Kind is "baseline").Envelope;
        guestPort.Enqueue(sender: 1, envelope: baseline);
        guest.Update(elapsedMilliseconds: 0);
        FoundationEnvelope ack = guestPort.Sent.Last(predicate: row => row.Envelope.Kind is "ack").Envelope;
        hostPort.Enqueue(sender: 7, envelope: ack);
        host.Update(elapsedMilliseconds: 0);
        foreach (FoundationEnvelope part in hostPort.Sent.Where(predicate: row =>
                row.Envelope.Kind is "view-part" && row.Envelope.Game?.Scope is "status")
                .Select(selector: row => row.Envelope))
        {
            guestPort.Enqueue(sender: 1, envelope: part);
        }

        FoundationEnvelope state = hostPort.Sent.Last(predicate: row => row.Envelope.Kind is "state").Envelope;
        guestPort.Enqueue(sender: 1, envelope: state);
        guest.Update(elapsedMilliseconds: 0);
        guest.Ready.ShouldBeTrue();
        guest.LocalActor?.PlayerId.ShouldBe(expected: profile.PlayerId);
        guest.Submit(direction: 1).ShouldBeTrue();
        FoundationEnvelope intent = guestPort.Sent.Last(predicate: row => row.Envelope.Game?.Action is "move")
                .Envelope;
        hostPort.Enqueue(sender: 7, envelope: intent);
        host.Update(elapsedMilliseconds: 1000);
        FoundationEnvelope moved = hostPort.Sent.Last(predicate: row => row.Envelope.Kind is "state").Envelope;
        guestPort.Enqueue(sender: 1, envelope: moved);
        guest.Update(elapsedMilliseconds: 0);
        guest.LocalActor?.PositionSubunits.ShouldBe(expected: -5000);
        host.Close();
    }

    private static void CheckEqualNames(PlayerProfile profile)
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: profile.DisplayName, connection: port);
        host.Start(host: true);
        port.Enqueue(sender: 2, envelope: new FoundationEnvelope(kind: "hello", playerId: profile.PlayerId,
                label: profile.DisplayName, attemptId: "first"));
        port.Enqueue(sender: 3, envelope: new FoundationEnvelope(kind: "hello", playerId: "distinct",
                label: profile.DisplayName, attemptId: "second"));
        port.Enqueue(sender: 4, envelope: new FoundationEnvelope(kind: "hello", playerId: profile.PlayerId,
                label: "Different name", attemptId: "duplicate"));
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 3);
        host.Snapshot?.Participants[0].Label.ShouldBe(expected: profile.DisplayName);
        port.Sent.Last().Envelope.Label.ShouldStartWith(expected: "Duplicate profile");
        host.Close();
    }

    private static Dictionary<string, string> CoreHashes()
    {
        return Directory.GetFiles(path: Path.Combine(path1: ProofProtocol.Root, path2: "src/godot-game/core"),
                searchPattern: "*.cs")
                .Order(comparer: StringComparer.Ordinal).ToDictionary(keySelector: file => Path.GetFileName(path: file),
                        elementSelector: file => Convert.ToHexStringLower(bytes: SHA256.HashData(
                                source: File.ReadAllBytes(path: file))), comparer: StringComparer.Ordinal);
    }
}
