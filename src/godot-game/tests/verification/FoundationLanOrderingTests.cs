using System;
using System.Linq;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Rejects unauthorized work before mutation, including sequence high-water changes.</summary>
[Trait("Tier", "1")]
public sealed class FoundationLanOrderingTests
{
    /// <summary>Invalid high sequences cannot suppress the next valid stop or reverse.</summary>
    [Fact]
    public void Submit_RejectedCommands_LeaveIntentAndHighWaterUnchanged()
    {
        foreach (bool local in new[] { true, false })
        {
            CheckAuthority(local: local);
        }

        CheckSender();
        ProofProtocol.Record(id: "core.ordering", diagnostic: "IMMUTABLE_REJECTION_BEFORE_SEQUENCE_MUTATION");
    }

    private static void CheckAuthority(bool local)
    {
        var session = new FoundationSession(sessionId: "s", playerId: "host", actorId: "a1");
        session.AddParticipant(bindingId: "b2", playerId: "guest", actorId: "a2", label: "Guest").ShouldBeTrue();
        var command = new FoundationMovementCommand(sessionId: "s", playerId: local ? "host" : "guest",
                actorId: local ? "a1" : "a2", direction: 1, bindingId: local ? "local" : "b2");
        if (!local)
        {
            Try(session: session, command: command).ShouldBeFalse();
            session.MarkReady(bindingId: "b2").ShouldBeTrue();
        }

        Try(session: session, command: command).ShouldBeTrue();
        FoundationSnapshot before = session.Snapshot;
        foreach (FoundationMovementCommand invalid in new[] { command, command with { Sequence = 0 },
            command with { Sequence = 100, SessionId = "old" }, command with { Sequence = 100, ActorId = "foreign" },
            command with { Sequence = 100, PlayerId = "foreign" }, command with { Sequence = 100, BindingId = "old" },
            command with { Sequence = 100, Direction = 2 } })
        {
            Try(session: session, command: invalid, original: command).ShouldBeFalse();
            ReferenceEquals(objA: before, objB: session.Snapshot).ShouldBeTrue();
        }

        session.Step(elapsedMilliseconds: 100);
        long moved = session.Snapshot.Participants.First(predicate: p => p.BindingId.Equals(
                value: command.BindingId, comparisonType: StringComparison.Ordinal)).Actor.PositionSubunits;
        Try(session: session, command: command with { Sequence = 2, Direction = 0 }).ShouldBeTrue();
        Try(session: session, command: command).ShouldBeFalse();
        session.Step(elapsedMilliseconds: 100);
        session.Snapshot.Participants.First(predicate: p => p.BindingId.Equals(value: command.BindingId,
                comparisonType: StringComparison.Ordinal)).Actor.PositionSubunits.ShouldBe(expected: moved);
        Try(session: session, command: command with { Sequence = 3, Direction = -1 }).ShouldBeTrue();
        session.Step(elapsedMilliseconds: 100);
        session.Snapshot.Participants.First(predicate: p => p.BindingId.Equals(value: command.BindingId,
                comparisonType: StringComparison.Ordinal)).Actor.PositionSubunits.ShouldBe(expected: -8000);
    }

    private static bool Try(FoundationSession session, FoundationMovementCommand command,
            FoundationMovementCommand? original = null)
    {
        FoundationMovementCommand owner = original ?? command;
        return session.TryAccept(command: command, trustedBinding: owner.BindingId, trustedPlayer: owner.PlayerId,
                trustedActor: owner.ActorId);
    }

    private static void CheckSender()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        port.Enqueue(sender: 2, envelope: new FoundationEnvelope(kind: "hello", playerId: "guest", label: "Guest"));
        host.Update(elapsedMilliseconds: 0);
        FoundationEnvelope baseline = port.Sent[0].Envelope;
        var intent = new FoundationEnvelope(kind: "intent", sessionId: baseline.SessionId,
                bindingId: baseline.BindingId, playerId: baseline.PlayerId, actorId: baseline.ActorId, sequence: 1,
                direction: 1);
        port.Enqueue(sender: 2, envelope: intent);
        host.Update(elapsedMilliseconds: 0);
        host.AcceptedIntents.ShouldBe(expected: 0);
        port.Enqueue(sender: 2, envelope: new FoundationEnvelope(kind: "ack", sessionId: baseline.SessionId,
                bindingId: baseline.BindingId, playerId: baseline.PlayerId, actorId: baseline.ActorId,
                revision: baseline.Revision));
        host.Update(elapsedMilliseconds: 0);
        port.Enqueue(sender: 3, envelope: intent);
        port.Enqueue(sender: 2, envelope: intent, generation: "obsolete-adapter");
        host.Update(elapsedMilliseconds: 0);
        host.AcceptedIntents.ShouldBe(expected: 0);
        port.Enqueue(sender: 2, envelope: intent);
        host.Update(elapsedMilliseconds: 100);
        host.AcceptedIntents.ShouldBe(expected: 1);
        host.Snapshot?.Participants[1].Actor.PositionSubunits.ShouldBe(expected: -7700);
    }
}
