using System;
using System.IO;
using System.Linq;
using System.Text;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves that receiving identified intent is distinct from elapsed host simulation.</summary>
public sealed class FoundationLanAuthorityTests
{
    /// <summary>An owned intent is accepted without advancing simulation time or position.</summary>
    [Fact]
    public void OwnedIntentIsAcceptedBeforeAnyElapsedStep()
    {
        var session = new FoundationSession(sessionId: "s", playerId: "host", actorId: "host-actor");
        var command = new FoundationMovementCommand(sessionId: "s", playerId: "host",
                actorId: "host-actor", direction: 1);
        session.TryAccept(command: command, trustedBinding: "local", trustedPlayer: "host",
                trustedActor: "host-actor").ShouldBeTrue();
        session.Actor.PositionSubunits.ShouldBe(expected: -8000);
        session.Actor.ElapsedMilliseconds.ShouldBe(expected: 0);
        CheckSequencesAndReadiness();
        CheckCoordinatorRoles();
        CheckPromptPublication();
        CheckProtocol();
        ProofProtocol.Record(id: "core.authority", diagnostic: "OWNED_INTENT_BEFORE_HOST_STEP");
    }

    private static void CheckSequencesAndReadiness()
    {
        var session = new FoundationSession(sessionId: "s", playerId: "host", actorId: "a1");
        session.AddParticipant(bindingId: "guest-binding", playerId: "guest", actorId: "a2", label: "Guest")
                .ShouldBeTrue();
        var command = new FoundationMovementCommand(sessionId: "s", playerId: "guest", actorId: "a2",
                direction: 1, bindingId: "guest-binding", sequence: 1);
        session.TryAccept(command: command, trustedBinding: "guest-binding", trustedPlayer: "guest",
                trustedActor: "a2").ShouldBeFalse();
        session.MarkReady(bindingId: "guest-binding").ShouldBeTrue();
        session.TryAccept(command: command, trustedBinding: "local", trustedPlayer: "host", trustedActor: "a1")
                .ShouldBeFalse();
        session.TryAccept(command: command, trustedBinding: "guest-binding", trustedPlayer: "guest",
                trustedActor: "a2").ShouldBeTrue();
        session.TryAccept(command: command, trustedBinding: "guest-binding", trustedPlayer: "guest",
                trustedActor: "a2").ShouldBeFalse();
        session.Step(elapsedMilliseconds: 1000);
        session.Snapshot.Participants[1].Actor.PositionSubunits.ShouldBe(expected: -5000);
        session.Actor.PositionSubunits.ShouldBe(expected: -8000);
        session.TryAccept(command: command with { Direction = 0, Sequence = 2 }, trustedBinding: "guest-binding",
                trustedPlayer: "guest", trustedActor: "a2").ShouldBeTrue();
        session.TryAccept(command: command, trustedBinding: "guest-binding", trustedPlayer: "guest",
                trustedActor: "a2").ShouldBeFalse();
        session.Step(elapsedMilliseconds: 1000);
        session.Snapshot.Participants[1].Actor.PositionSubunits.ShouldBe(expected: -5000);
    }

    private static void CheckCoordinatorRoles()
    {
        var hostPort = new FoundationTestConnection(sender: 1);
        var guestPort = new FoundationTestConnection(sender: 2);
        hostPort.Partner = guestPort;
        guestPort.Partner = hostPort;
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: hostPort);
        var guest = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: guestPort);
        host.Start(host: true);
        guest.Join(address: "test-provider", port: 24680);
        guest.Submit(direction: 1).ShouldBeFalse();
        guest.Update(elapsedMilliseconds: 0);
        host.Update(elapsedMilliseconds: 0);
        guest.Update(elapsedMilliseconds: 0);
        guest.InstalledRevision.ShouldBeGreaterThanOrEqualTo(expected: 0);
        guest.Ready.ShouldBeFalse();
        host.Update(elapsedMilliseconds: 0);
        guest.Update(elapsedMilliseconds: 0);
        guest.Ready.ShouldBeTrue();
        host.Submit(direction: -1).ShouldBeTrue();
        guest.Submit(direction: 1).ShouldBeTrue();
        host.Update(elapsedMilliseconds: 1000);
        guest.Update(elapsedMilliseconds: 0);
        RequireActor(coordinator: host).PositionSubunits.ShouldBe(expected: -11000);
        RequireActor(coordinator: guest).PositionSubunits.ShouldBe(expected: -5000);
        FoundationSnapshot state = host.Snapshot ?? throw new InvalidOperationException(message: "HOST_SNAPSHOT");
        state.Participants.Select(selector: p => p.Actor.ActorId).Distinct(comparer: StringComparer.Ordinal)
                .Count().ShouldBe(expected: 2);
        host.AcceptedIntents.ShouldBe(expected: 2);
        var solo = new FoundationCoordinator(playerId: "solo", label: "Solo");
        solo.Start(host: false);
        solo.Submit(direction: 1).ShouldBeTrue();
        solo.Update(elapsedMilliseconds: 1000);
        RequireActor(coordinator: solo).PositionSubunits.ShouldBe(expected:
                RequireActor(coordinator: guest).PositionSubunits);
        solo.Close();
        host.Close();
        guest.Close();
    }

    private static void CheckPromptPublication()
    {
        var hostPort = new FoundationTestConnection(sender: 1);
        var guestPort = new FoundationTestConnection(sender: 2);
        hostPort.Partner = guestPort;
        guestPort.Partner = hostPort;
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: hostPort);
        var guest = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: guestPort);
        host.Start(host: true);
        guest.Join(address: "test-provider", port: 24680);
        guest.Update(elapsedMilliseconds: 0);
        host.Update(elapsedMilliseconds: 0);
        guest.Update(elapsedMilliseconds: 0);
        host.Update(elapsedMilliseconds: 0);
        guest.Update(elapsedMilliseconds: 0);
        long revision = guest.Snapshot?.Revision ?? throw new InvalidOperationException(message: "READY_SNAPSHOT");
        host.Update(elapsedMilliseconds: 15);
        guest.Update(elapsedMilliseconds: 0);
        guest.Snapshot?.Revision.ShouldBe(expected: revision);
        host.Update(elapsedMilliseconds: 1);
        guest.Update(elapsedMilliseconds: 0);
        guest.Snapshot?.Revision.ShouldBeGreaterThan(expected: revision);
        foreach (bool local in new[] { true, false })
        {
            foreach (int direction in new[] { 1, -1, 0 })
            {
                CheckPromptIntent(host: host, guest: guest, local: local, direction: direction);
            }
        }

        host.Close();
        guest.Close();
    }

    private static void CheckPromptIntent(FoundationCoordinator host, FoundationCoordinator guest,
            bool local, int direction)
    {
        FoundationCoordinator origin = local ? host : guest;
        FoundationActorState before = RequireActor(coordinator: origin);
        origin.Submit(direction: direction).ShouldBeTrue();
        RequireActor(coordinator: origin).PositionSubunits.ShouldBe(expected: before.PositionSubunits);
        host.Update(elapsedMilliseconds: 1);
        guest.Update(elapsedMilliseconds: 0);
        FoundationSnapshot state = guest.Snapshot ?? throw new InvalidOperationException(message: "GUEST_SNAPSHOT");
        FoundationActorState published = state.Participants.First(predicate: participant => participant.Actor.ActorId
                .Equals(value: before.ActorId, comparisonType: StringComparison.Ordinal)).Actor;
        published.PositionSubunits.ShouldBe(expected: before.PositionSubunits + 3 * direction);
        published.ElapsedMilliseconds.ShouldBe(expected: before.ElapsedMilliseconds + 1);
        state.Revision.ShouldBeGreaterThan(expected: host.LastAcceptedRevision);
    }

    private static void CheckProtocol()
    {
        var session = new FoundationSession(sessionId: "s", playerId: "p", actorId: "a");
        byte[] encoded = FoundationProtocol.Encode(envelope: new FoundationEnvelope(kind: "state", sessionId: "s",
                revision: session.Snapshot.Revision, snapshot: session.Snapshot));
        FoundationEnvelope decoded = FoundationProtocol.Decode(bytes: encoded);
        FoundationSnapshot state = decoded.Snapshot ?? throw new InvalidOperationException(message: "DECODED_SNAPSHOT");
        state.Participants[0].Actor.PositionSubunits.ShouldBe(expected: -8000);
        string json = Encoding.UTF8.GetString(bytes: encoded);
        foreach (string invalid in new[] { json.Replace(oldValue: "foundation.fixture.v1", newValue: "other",
                comparisonType: StringComparison.Ordinal), json.Replace(oldValue: "state", newValue: "unknown",
                        comparisonType: StringComparison.Ordinal), json.Replace(oldValue: "-8000", newValue: "NaN",
                                comparisonType: StringComparison.Ordinal), json[..^1] })
        {
            Should.Throw<Exception>(actual: () => FoundationProtocol.Decode(bytes: Encoding.UTF8.GetBytes(
                    s: invalid)));
        }

        Should.Throw<InvalidDataException>(actual: () => FoundationProtocol.Decode(bytes: new byte[32769]));
        session.Actor.PositionSubunits.ShouldBe(expected: -8000);
    }
    private static FoundationActorState RequireActor(FoundationCoordinator coordinator)
    {
        return coordinator.LocalActor ?? throw new InvalidOperationException(message: "ACTOR_REQUIRED");
    }
}
