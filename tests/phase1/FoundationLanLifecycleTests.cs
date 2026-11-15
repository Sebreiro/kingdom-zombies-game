using System;
using System.Linq;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Closes acceptance and projections before releasing a coordinator lifetime.</summary>
public sealed class FoundationLanLifecycleTests
{
    /// <summary>Repeated closure cannot retain a projection or close native ownership more than once.</summary>
    [Fact]
    public void LifetimeCleanupAndDeadlineBoundaries()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        host.Close();
        host.Snapshot.ShouldBeNull();
        host.Close();
        port.CloseCount.ShouldBe(expected: 1);
        host.Submit(direction: 1).ShouldBeFalse();
        CheckPendingDeadlines();
        CheckReadyDeadlines();
        CheckBoundaryResponses();
        CheckReplacement();
        CheckClosedCallbacks();
        CheckEndDrain();
        CheckGuestDrain();
        CheckPendingBaselineDeadline();
        CheckCancelAndOutcome();
        ProofProtocol.Record(id: "core.lifecycle", diagnostic: "30000_10000_TRIPLETS_RESPONSE_FIRST_LIFETIME_FENCES");
    }

    private static void CheckPendingDeadlines()
    {
        foreach (long now in new long[] { 29999, 30000, 30001 })
        {
            var clock = new FoundationScenarioClock();
            var port = new FoundationHarnessConnection();
            var guest = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: port,
                    nowMilliseconds: () => clock.Now);
            guest.Join(address: "test", port: 24680);
            clock.Now = now;
            guest.Update(elapsedMilliseconds: 0);
            guest.Finished.ShouldBe(expected: now >= 30000);
            if (guest.Finished)
            {
                guest.Error.ShouldStartWith(expected: "Join timed out");
                guest.Snapshot.ShouldBeNull();
                port.CloseCount.ShouldBe(expected: 1);
            }

            (FoundationCoordinator host, FoundationHarnessConnection hostPort, FoundationEnvelope baseline,
                    FoundationScenarioClock hostClock) = Pending();
            hostClock.Now = now;
            host.Update(elapsedMilliseconds: 0);
            host.Snapshot?.Participants.Length.ShouldBe(expected: now >= 30000 ? 1 : 2);
            hostPort.Sent.Any(predicate: p => p.Envelope.Kind is "baseline").ShouldBeTrue();
            baseline.Revision.ShouldBeGreaterThanOrEqualTo(expected: 0);
        }
    }

    private static void CheckReadyDeadlines()
    {
        foreach (long now in new long[] { 9999, 10000, 10001 })
        {
            (FoundationCoordinator host, FoundationHarnessConnection port, FoundationEnvelope baseline,
                    FoundationScenarioClock clock) = Pending();
            port.Enqueue(sender: 2, envelope: Reply(kind: "ack", baseline: baseline));
            host.Update(elapsedMilliseconds: 0);
            clock.Now = now;
            port.Enqueue(sender: 2, envelope: Reply(kind: "ack", baseline: baseline));
            port.Enqueue(sender: 2, envelope: Reply(kind: "intent", baseline: baseline, sequence: 100),
                    generation: "old-adapter");
            host.Update(elapsedMilliseconds: 0);
            host.Snapshot?.Participants.Length.ShouldBe(expected: now >= 10000 ? 1 : 2);
            (FoundationCoordinator guest, FoundationHarnessConnection guestPort, FoundationScenarioClock guestClock,
                    FoundationEnvelope state) = ReadyGuest();
            guestClock.Now = now;
            guestPort.Enqueue(sender: 1, envelope: state);
            guestPort.Raw(sender: 1, payload: new byte[] { 1, 2, 3 });
            guest.Update(elapsedMilliseconds: 0);
            guest.Finished.ShouldBe(expected: now >= 10000);
            if (guest.Finished)
            {
                guest.Error.ShouldStartWith(expected: "Connection lost");
            }
        }
    }

    private static void CheckBoundaryResponses()
    {
        (FoundationCoordinator host, FoundationHarnessConnection port, FoundationEnvelope baseline,
                FoundationScenarioClock clock) = Pending();
        clock.Now = 30000;
        port.Enqueue(sender: 2, envelope: Reply(kind: "ack", baseline: baseline));
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants[1].Ready.ShouldBeTrue();
        clock.Now = 40000;
        port.Enqueue(sender: 2, envelope: Reply(kind: "ping", baseline: baseline, sequence: 1));
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 2);
        clock.Now = 50000;
        port.Enqueue(sender: 2, envelope: Reply(kind: "ping", baseline: baseline, sequence: 1));
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 1);
        (FoundationCoordinator guest, FoundationHarnessConnection guestPort, FoundationScenarioClock guestClock,
                FoundationEnvelope state) = ReadyGuest();
        guestClock.Now = 10000;
        var next = new FoundationSnapshot(sessionId: "s", revision: 3, participants:
                state.Snapshot?.Participants ?? throw new InvalidOperationException(message: "STATE"));
        guestPort.Enqueue(sender: 1, envelope: new FoundationEnvelope(kind: "state", sessionId: "s", revision: 3,
                snapshot: next, attemptId: state.AttemptId));
        guest.Update(elapsedMilliseconds: 0);
        guest.Ready.ShouldBeTrue();
        guestClock.Now = 19999;
        guest.Update(elapsedMilliseconds: 0);
        guest.Ready.ShouldBeTrue();
        guestClock.Now = 20000;
        guest.Update(elapsedMilliseconds: 0);
        guest.Finished.ShouldBeTrue();
    }

    private static void CheckReplacement()
    {
        (FoundationCoordinator host, FoundationHarnessConnection port, FoundationEnvelope old,
                FoundationScenarioClock clock) = Pending();
        port.Enqueue(sender: 2, envelope: Reply(kind: "ack", baseline: old));
        host.Update(elapsedMilliseconds: 0);
        host.Submit(direction: 1).ShouldBeTrue();
        host.Update(elapsedMilliseconds: 100);
        long before = host.LocalActor?.PositionSubunits ?? 0;
        port.Enqueue(sender: 2, envelope: Reply(kind: "bye", baseline: old));
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 1);
        host.LocalActor?.PositionSubunits.ShouldBe(expected: before);
        port.Sent.Clear();
        port.Enqueue(sender: 2, envelope: new FoundationEnvelope(kind: "hello", playerId: "guest", label: "Guest",
                attemptId: "replacement"));
        host.Update(elapsedMilliseconds: 0);
        FoundationEnvelope replacement = port.Sent.First(predicate: p => p.Envelope.Kind is "baseline").Envelope;
        StringComparer.Ordinal.Equals(x: replacement.BindingId, y: old.BindingId).ShouldBeFalse();
        StringComparer.Ordinal.Equals(x: replacement.ActorId, y: old.ActorId).ShouldBeFalse();
        replacement.Snapshot?.Participants[1].Slot.ShouldBe(expected: 2);
        foreach (string kind in new[] { "ack", "bye", "intent", "cancel" })
        {
            port.Enqueue(sender: 2, envelope: Reply(kind: kind, baseline: old, sequence: 100));
        }

        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants[1].BindingId.ShouldBe(expected: replacement.BindingId);
        host.Snapshot?.Participants[1].Ready.ShouldBeFalse();
        port.Enqueue(sender: 2, envelope: Reply(kind: "ack", baseline: replacement));
        host.Update(elapsedMilliseconds: 100);
        host.LocalActor?.PositionSubunits.ShouldBe(expected: before + 300);
        host.Snapshot?.Participants[1].Actor.PositionSubunits.ShouldBe(expected: -8000);
        clock.Now.ShouldBe(expected: 0);
    }

    private static void CheckClosedCallbacks()
    {
        (FoundationCoordinator guest, FoundationHarnessConnection port, FoundationScenarioClock clock,
                FoundationEnvelope state) = ReadyGuest();
        guest.Close();
        foreach (string kind in new[] { "state", "baseline", "end", "reject" })
        {
            port.Enqueue(sender: 1, envelope: new FoundationEnvelope(kind: kind, sessionId: "s", revision: 2,
                    snapshot: state.Snapshot, attemptId: state.AttemptId, label: "old error"));
        }

        clock.Now = 30001;
        guest.Update(elapsedMilliseconds: 100);
        guest.Close();
        guest.Snapshot.ShouldBeNull();
        guest.Error.ShouldBeEmpty();
        port.CloseCount.ShouldBe(expected: 1);
        Should.Throw<InvalidOperationException>(actual: () => guest.Join(address: "test", port: 24680));
        var freshPort = new FoundationHarnessConnection();
        var fresh = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: freshPort);
        fresh.Start(host: true);
        StringComparer.Ordinal.Equals(x: fresh.Snapshot?.SessionId, y: "s").ShouldBeFalse();
        fresh.Submit(direction: 0).ShouldBeTrue();
        fresh.Update(elapsedMilliseconds: 100);
        fresh.LocalActor?.PositionSubunits.ShouldBe(expected: -8000);
    }

    private static void CheckEndDrain()
    {
        (FoundationCoordinator host, FoundationHarnessConnection port, FoundationEnvelope baseline,
                FoundationScenarioClock clock) = Pending();
        host.RequestClose();
        host.Snapshot.ShouldBeNull();
        host.Submit(direction: 1).ShouldBeFalse();
        clock.Now = 249;
        host.Update(elapsedMilliseconds: 0);
        port.CloseCount.ShouldBe(expected: 0);
        clock.Now = 250;
        host.Update(elapsedMilliseconds: 0);
        port.CloseCount.ShouldBe(expected: 1);
        host.RequestClose();
        host.Close();
        port.CloseCount.ShouldBe(expected: 1);
        baseline.SessionId.ShouldNotBeEmpty();
    }

    private static void CheckGuestDrain()
    {
        (FoundationCoordinator guest, FoundationHarnessConnection port, FoundationScenarioClock clock,
                FoundationEnvelope state) = ReadyGuest();
        guest.RequestClose();
        guest.Snapshot.ShouldBeNull();
        guest.Submit(direction: 1).ShouldBeFalse();
        port.Sent.Last().Envelope.Kind.ShouldBe(expected: "bye");
        port.CloseCount.ShouldBe(expected: 0);
        clock.Now = 249;
        port.Enqueue(sender: 1, envelope: state);
        guest.Update(elapsedMilliseconds: 0);
        guest.Snapshot.ShouldBeNull();
        port.CloseCount.ShouldBe(expected: 0);
        clock.Now = 250;
        guest.Update(elapsedMilliseconds: 0);
        port.CloseCount.ShouldBe(expected: 1);
        guest.RequestClose();
        port.CloseCount.ShouldBe(expected: 1);
    }

    private static void CheckPendingBaselineDeadline()
    {
        var clock = new FoundationScenarioClock();
        var port = new FoundationHarnessConnection();
        var guest = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: port,
                nowMilliseconds: () => clock.Now);
        guest.Join(address: "test", port: 24680);
        guest.Update(elapsedMilliseconds: 0);
        string attempt = port.Sent.Single().Envelope.AttemptId;
        var session = new FoundationSession(sessionId: "s", playerId: "host", actorId: "a1");
        session.AddParticipant(bindingId: "b2", playerId: "guest", actorId: "a2", label: "Guest").ShouldBeTrue();
        clock.Now = 29999;
        port.Enqueue(sender: 1, envelope: new FoundationEnvelope(kind: "baseline", sessionId: "s", revision: 1,
                playerId: "guest", actorId: "a2", bindingId: "b2", snapshot: session.Snapshot, attemptId: attempt));
        guest.Update(elapsedMilliseconds: 0);
        guest.Snapshot.ShouldNotBeNull();
        guest.Ready.ShouldBeFalse();
        clock.Now = 30000;
        guest.Update(elapsedMilliseconds: 0);
        guest.Finished.ShouldBeTrue();
        guest.Error.ShouldStartWith(expected: "Join timed out");
    }

    private static void CheckCancelAndOutcome()
    {
        (FoundationCoordinator host, FoundationHarnessConnection port, FoundationEnvelope baseline,
                FoundationScenarioClock clock) = Pending();
        var cancel = new FoundationEnvelope(kind: "cancel", playerId: "guest", attemptId: baseline.AttemptId);
        port.Enqueue(sender: 2, envelope: cancel);
        port.Enqueue(sender: 2, envelope: cancel);
        host.Update(elapsedMilliseconds: 0);
        host.Snapshot?.Participants.Length.ShouldBe(expected: 1);
        port.Sent.Clear();
        port.Enqueue(sender: 2, envelope: new FoundationEnvelope(kind: "hello", playerId: "guest", label: "Guest",
                attemptId: baseline.AttemptId));
        host.Update(elapsedMilliseconds: 0);
        port.Sent.Single().Envelope.Label.ShouldStartWith(expected: "This admission attempt is closed");
        host.Snapshot?.Participants.Length.ShouldBe(expected: 1);
        clock.Now.ShouldBe(expected: 0);
    }

    private static (FoundationCoordinator, FoundationHarnessConnection, FoundationEnvelope, FoundationScenarioClock)
            Pending()
    {
        var clock = new FoundationScenarioClock();
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port,
                nowMilliseconds: () => clock.Now);
        host.Start(host: true);
        port.Enqueue(sender: 2, envelope: new FoundationEnvelope(kind: "hello", playerId: "guest", label: "Guest",
                attemptId: "attempt"));
        host.Update(elapsedMilliseconds: 0);
        return (host, port, port.Sent.First(predicate: p => p.Envelope.Kind is "baseline").Envelope, clock);
    }

    private static FoundationEnvelope Reply(string kind, FoundationEnvelope baseline, long sequence = 0)
    {
        return new FoundationEnvelope(kind: kind, sessionId: baseline.SessionId, playerId: baseline.PlayerId,
                actorId: baseline.ActorId, bindingId: baseline.BindingId, revision: baseline.Revision,
                sequence: sequence, attemptId: baseline.AttemptId);
    }

    private static (FoundationCoordinator, FoundationHarnessConnection, FoundationScenarioClock, FoundationEnvelope)
            ReadyGuest()
    {
        var clock = new FoundationScenarioClock();
        var port = new FoundationHarnessConnection();
        var guest = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: port,
                nowMilliseconds: () => clock.Now);
        guest.Join(address: "test", port: 24680);
        guest.Update(elapsedMilliseconds: 0);
        string attempt = port.Sent.Single().Envelope.AttemptId;
        var session = new FoundationSession(sessionId: "s", playerId: "host", actorId: "a1");
        session.AddParticipant(bindingId: "b2", playerId: "guest", actorId: "a2", label: "Guest").ShouldBeTrue();
        port.Enqueue(sender: 1, envelope: new FoundationEnvelope(kind: "baseline", sessionId: "s", playerId: "guest",
                actorId: "a2", bindingId: "b2", revision: 1, snapshot: session.Snapshot, attemptId: attempt));
        guest.Update(elapsedMilliseconds: 0);
        session.MarkReady(bindingId: "b2").ShouldBeTrue();
        var state = new FoundationEnvelope(kind: "state", sessionId: "s", revision: 2,
                snapshot: session.Snapshot, attemptId: attempt);
        port.Enqueue(sender: 1, envelope: state);
        guest.Update(elapsedMilliseconds: 0);
        guest.Ready.ShouldBeTrue();
        return (guest, port, clock, state);
    }
}
