using System;
using System.Linq;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises reservation capacity and idempotence through actual coordinator wire dispatch.</summary>
public sealed class FoundationLanAdmissionTests
{
    /// <summary>A repeated hello returns its reservation; refusal reasons remain distinct.</summary>
    [Fact]
    public void PendingAdmissionIsIdempotentAndBounded()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        var hello = new FoundationEnvelope(kind: "hello", playerId: "p2", label: "Guest");
        port.Enqueue(sender: 2, envelope: hello);
        host.Update(elapsedMilliseconds: 0);
        FoundationEnvelope original = port.Sent.Single(predicate: row => row.Envelope.Kind is "baseline").Envelope;
        port.Sent.Clear();
        port.Enqueue(sender: 2, envelope: hello);
        host.Update(elapsedMilliseconds: 0);
        port.Sent.Count(predicate: row => row.Envelope.Kind is "baseline").ShouldBe(expected: 1);
        port.Sent.First(predicate: row => row.Envelope.Kind is "baseline").Envelope.BindingId
                .ShouldBe(expected: original.BindingId);
        port.Enqueue(sender: 3, envelope: new FoundationEnvelope(kind: "hello", playerId: "p2", label: "Other"));
        host.Update(elapsedMilliseconds: 0);
        port.Sent.Last().Envelope.Label.ShouldStartWith(expected: "Duplicate profile");
        foreach (int sender in new[] { 4, 5, 6 })
        {
            port.Enqueue(sender: sender, envelope: new FoundationEnvelope(kind: "hello",
                    playerId: "p" + sender, label: "Guest"));
        }

        host.Update(elapsedMilliseconds: 0);
        (host.Snapshot?.Participants.Length ?? 0).ShouldBe(expected: 4);
        port.Sent.Last().Envelope.Label.ShouldStartWith(expected: "Full");
        CheckMovingBaseline();
        CheckRefusalFlood();
        CheckConnectionRetirement();
        CheckDisconnectedHistoryCleanup();
        ProofProtocol.Record(id: "core.admission", diagnostic: "PENDING_CAPACITY_REPEAT_HELLO_DISTINCT_REASONS");
    }

    private static void CheckMovingBaseline()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        host.Submit(direction: 1).ShouldBeTrue();
        port.Enqueue(sender: 2, envelope: new FoundationEnvelope(kind: "hello", playerId: "p2", label: "Guest"));
        host.Update(elapsedMilliseconds: 1000);
        FoundationEnvelope baseline = port.Sent.First(predicate: p => p.Envelope.Kind is "baseline").Envelope;
        host.LocalActor?.PositionSubunits.ShouldBe(expected: -5000);
        var ack = new FoundationEnvelope(kind: "ack", sessionId: baseline.SessionId, playerId: baseline.PlayerId,
                bindingId: baseline.BindingId, actorId: baseline.ActorId, revision: baseline.Revision);
        port.Enqueue(sender: 2, envelope: new FoundationEnvelope(kind: "ack", sessionId: baseline.SessionId,
                playerId: baseline.PlayerId, bindingId: baseline.BindingId, actorId: baseline.ActorId,
                revision: baseline.Revision + 1));
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants[1].Ready.ShouldBeFalse();
        port.Enqueue(sender: 2, envelope: ack);
        host.Update(elapsedMilliseconds: 0);
        FoundationEnvelope ready = port.Sent.Last(predicate: p => p.Envelope.Kind is "state").Envelope;
        ready.Snapshot?.Participants[0].Actor.PositionSubunits.ShouldBe(expected: -5000);
        ready.Snapshot?.Participants[1].Ready.ShouldBeTrue();
        port.Enqueue(sender: 2, envelope: ack);
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 2);
        FoundationSnapshot detached = baseline.Snapshot ?? throw new InvalidOperationException(message: "BASELINE");
        detached.Participants[0].Actor.PositionSubunits.ShouldBe(expected: -8000);
        detached.Participants[1].Ready.ShouldBeFalse();
        CheckSelectedSession();
    }

    private static void CheckSelectedSession()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        string selected = host.Snapshot?.SessionId ?? throw new InvalidOperationException(message: "HOST");
        CheckSelectionAttempt(host: host, port: port, expected: selected, pass: true);
        host.Close();
        port = new FoundationHarnessConnection();
        host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        CheckSelectionAttempt(host: host, port: port, expected: selected, pass: false);
        CheckSelectionAttempt(host: host, port: port, expected: "", pass: true);
        CheckWrongBaseline(host: host, port: port, selected: selected);
    }

    private static void CheckSelectionAttempt(FoundationCoordinator host, FoundationHarnessConnection port,
            string expected, bool pass)
    {
        var guestPort = new FoundationHarnessConnection();
        var guest = new FoundationCoordinator(playerId: "é/guest", label: "e\u0301 camp", connection: guestPort);
        guest.Join(address: "127.0.0.1", port: 47778, expectedSessionId: expected);
        for (int step = 0; step < 4; step++)
        {
            Exchange(host: host, port: port, guest: guest, guestPort: guestPort);
        }

        guest.Ready.ShouldBe(expected: pass);
        (guest.InstalledRevision >= 0).ShouldBe(expected: pass);
        guest.Submit(direction: 1).ShouldBe(expected: pass);
        if (!pass)
        {
            guest.Error.ShouldStartWith(expected: "The selected session is no longer available.");
            guest.LocalActor.ShouldBeNull();
            guest.Snapshot.ShouldBeNull();
            guestPort.CloseCount.ShouldBe(expected: 1);
        }

        guest.Close();
        Exchange(host: host, port: port, guest: guest, guestPort: guestPort);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 1);
    }

    private static void Exchange(FoundationCoordinator host, FoundationHarnessConnection port,
            FoundationCoordinator guest, FoundationHarnessConnection guestPort)
    {
        guest.Update(elapsedMilliseconds: 0);
        foreach ((int _, FoundationEnvelope envelope) in guestPort.Sent)
        {
            port.Enqueue(sender: 2, envelope: envelope);
        }

        guestPort.Sent.Clear();
        host.Update(elapsedMilliseconds: 0);
        foreach ((int _, FoundationEnvelope envelope) in port.Sent)
        {
            guestPort.Enqueue(sender: 1, envelope: envelope);
        }

        port.Sent.Clear();
    }

    private static void CheckWrongBaseline(FoundationCoordinator host, FoundationHarnessConnection port,
            string selected)
    {
        var guestPort = new FoundationHarnessConnection();
        var guest = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: guestPort);
        guest.Join(address: "127.0.0.1", port: 47778, expectedSessionId: selected);
        guest.Update(elapsedMilliseconds: 0);
        FoundationEnvelope hello = guestPort.Sent.Single().Envelope;
        port.Enqueue(sender: 2, envelope: new FoundationEnvelope(kind: "hello", playerId: hello.PlayerId,
                label: hello.Label, attemptId: hello.AttemptId));
        host.Update(elapsedMilliseconds: 0);
        FoundationEnvelope baseline = port.Sent.Last(predicate: row => row.Envelope.Kind is "baseline").Envelope;
        guestPort.Sent.Clear();
        guestPort.Enqueue(sender: 1, envelope: baseline);
        guest.Update(elapsedMilliseconds: 0);
        guest.Snapshot.ShouldBeNull();
        guest.InstalledRevision.ShouldBe(expected: -1);
        guestPort.Sent.Any(predicate: row => row.Envelope.Kind is "ack").ShouldBeFalse();
        guestPort.CloseCount.ShouldBe(expected: 1);
        Exchange(host: host, port: port, guest: guest, guestPort: guestPort);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 1);
        CheckStaleSelection(host: host, port: port, baseline: baseline, hello: hello);
    }

    private static void CheckStaleSelection(FoundationCoordinator host, FoundationHarnessConnection port,
            FoundationEnvelope baseline, FoundationEnvelope hello)
    {
        port.Sent.Clear();
        var guestPort = new FoundationHarnessConnection();
        var guest = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: guestPort);
        guest.Join(address: "127.0.0.1", port: 47778);
        guestPort.Enqueue(sender: 1, envelope: baseline);
        guestPort.Enqueue(sender: 1, envelope: new FoundationEnvelope(kind: "reject", label: "old failure",
                attemptId: hello.AttemptId));
        guest.Update(elapsedMilliseconds: 0);
        guest.Error.ShouldBeEmpty();
        guest.Snapshot.ShouldBeNull();
        for (int step = 0; step < 4; step++)
        {
            Exchange(host: host, port: port, guest: guest, guestPort: guestPort);
        }

        guest.Ready.ShouldBeTrue();
        guest.Close();
        guestPort.Enqueue(sender: 1, envelope: baseline);
        guest.Update(elapsedMilliseconds: 0);
        guest.Snapshot.ShouldBeNull();
        guestPort.CloseCount.ShouldBe(expected: 1);
    }

    private static void CheckRefusalFlood()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        for (int attempt = 0; attempt < 160; attempt++)
        {
            port.Enqueue(sender: 9, envelope: new FoundationEnvelope(kind: "hello", playerId: "host", label: "Flood",
                    attemptId: "duplicate-" + attempt));
            host.Update(elapsedMilliseconds: 0);
        }

        for (int sender = 2; sender <= 4; sender++)
        {
            port.Enqueue(sender: sender, envelope: new FoundationEnvelope(kind: "hello", playerId: "p" + sender,
                    label: "Guest", attemptId: "join-" + sender));
        }

        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 4);
        FoundationEnvelope baseline = port.Sent.Last(predicate: row => row.Peer == 4
                && row.Envelope.Kind is "baseline").Envelope;
        for (int attempt = 0; attempt < 160; attempt++)
        {
            port.Enqueue(sender: 10, envelope: new FoundationEnvelope(kind: "hello", playerId: "flood" + attempt,
                    label: "Flood", attemptId: "full-" + attempt));
            host.Update(elapsedMilliseconds: 0);
        }

        port.Enqueue(sender: 4, envelope: new FoundationEnvelope(kind: "cancel", playerId: baseline.PlayerId,
                attemptId: baseline.AttemptId));
        port.Enqueue(sender: 11, envelope: new FoundationEnvelope(kind: "hello", playerId: "legitimate",
                label: "Guest", attemptId: "legitimate"));
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Any(predicate: p => p.Actor.PlayerId is "legitimate").ShouldBeTrue();
        port.Enqueue(sender: 4, envelope: new FoundationEnvelope(kind: "hello", playerId: baseline.PlayerId,
                label: "Guest", attemptId: baseline.AttemptId));
        host.Update(elapsedMilliseconds: 0);
        port.Sent.Last().Envelope.Label.ShouldStartWith(expected: "This admission attempt is closed");
    }

    private static void CheckDisconnectedHistoryCleanup()
    {
        var clock = new FoundationScenarioClock();
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port,
                nowMilliseconds: () => clock.Now);
        host.Start(host: true);
        for (int sender = 20; sender < 52; sender++)
        {
            port.Enqueue(sender: sender, envelope: new FoundationEnvelope(kind: "hello", playerId: "p" + sender,
                    label: "Guest", attemptId: "join" + sender));
            host.Update(elapsedMilliseconds: 0);
            host.Snapshot?.Participants.Length.ShouldBe(expected: 2);
            port.PeerGenerations[key: sender] = "";
            clock.Now += 30000;
            host.Update(elapsedMilliseconds: 0);
            host.Snapshot?.Participants.Length.ShouldBe(expected: 1);
            host.AdmissionHistoryCount.ShouldBe(expected: 0);
        }
    }

    private static void CheckConnectionRetirement()
    {
        var clock = new FoundationScenarioClock();
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port,
                nowMilliseconds: () => clock.Now);
        host.Start(host: true);
        var refused = new FoundationEnvelope(kind: "hello", playerId: "host", label: "Flood", attemptId: "old");
        port.Enqueue(sender: 9, envelope: refused);
        host.Update(elapsedMilliseconds: 0);
        clock.Now = 30000;
        host.Update(elapsedMilliseconds: 0);
        port.Enqueue(sender: 9, envelope: new FoundationEnvelope(kind: "hello", playerId: "new",
                label: "Guest", attemptId: "new"));
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 1);
        port.Enqueue(sender: 9, envelope: refused);
        port.PeerGenerations[key: 9] = "reconnected";
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 1);
        port.Enqueue(sender: 9, envelope: new FoundationEnvelope(kind: "hello", playerId: "new",
                label: "Guest", attemptId: "new"));
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 2);
    }
}
