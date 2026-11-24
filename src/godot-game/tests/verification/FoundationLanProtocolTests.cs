using System;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Publishes only completely validated current detached snapshots.</summary>
[Trait("Tier", "1")]
public sealed class FoundationLanProtocolTests
{
    /// <summary>Malformed content, identities, shape and bounds cannot replace the installed publication.</summary>
    [Fact]
    public void Publish_DetachedSnapshot_ValidatesBeforePublication()
    {
        FoundationWireFrameTests.Check();
        var port = new FoundationHarnessConnection();
        var guest = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: port);
        guest.Join(address: "test", port: 24680);
        guest.Update(elapsedMilliseconds: 0);
        string attempt = port.Sent.Single().Envelope.AttemptId;
        var session = new FoundationSession(sessionId: "s", playerId: "host", actorId: "a1");
        session.AddParticipant(bindingId: "b2", playerId: "guest", actorId: "a2", label: "Guest").ShouldBeTrue();
        var baseline = new FoundationEnvelope(kind: "baseline", sessionId: "s", playerId: "guest", actorId: "a2",
                bindingId: "b2", revision: session.Snapshot.Revision, snapshot: session.Snapshot, attemptId: attempt);
        port.Enqueue(sender: 1, envelope: baseline);
        guest.Update(elapsedMilliseconds: 0);
        guest.Ready.ShouldBeFalse();
        session.MarkReady(bindingId: "b2").ShouldBeTrue();
        var state = new FoundationEnvelope(kind: "state", sessionId: "s", revision: session.Snapshot.Revision,
                snapshot: session.Snapshot, attemptId: attempt);
        port.Enqueue(sender: 1, envelope: state);
        guest.Update(elapsedMilliseconds: 0);
        guest.Ready.ShouldBeTrue();
        FoundationSnapshot? before = guest.Snapshot;
        string json = Encoding.UTF8.GetString(bytes: FoundationProtocol.Encode(envelope: state));
        foreach (string invalid in InvalidDocuments(json: json))
        {
            port.Raw(sender: 1, payload: Encoding.UTF8.GetBytes(s: invalid));
            guest.Update(elapsedMilliseconds: 0);
            ReferenceEquals(objA: before, objB: guest.Snapshot).ShouldBeTrue();
        }

        port.Enqueue(sender: 1, envelope: baseline);
        port.Enqueue(sender: 1, envelope: state);
        guest.Update(elapsedMilliseconds: 0);
        ReferenceEquals(objA: before, objB: guest.Snapshot).ShouldBeTrue();
        guest.RejectedPackets.ShouldBeGreaterThanOrEqualTo(expected: 10);
        CheckOpaqueActors();
        CheckActorTime();
        ProofProtocol.Record(id: "core.protocol", diagnostic: "DETACHED_SHAPE_CONTENT_OWNER_REVISION_ATOMICITY");
    }

    private static void CheckActorTime()
    {
        foreach (string sessionId in new[] { "clock-session", "fresh-session" })
        {
            var clock = new FoundationScenarioClock();
            var port = new FoundationHarnessConnection();
            var guest = new FoundationCoordinator(playerId: "guest", label: "Guest", connection: port,
                    nowMilliseconds: () => clock.Now);
            guest.Join(address: "test", port: 24680);
            guest.Update(elapsedMilliseconds: 0);
            string attempt = port.Sent.Single().Envelope.AttemptId;
            long initial = sessionId is "fresh-session" ? 0 : long.MaxValue - 1;
            FoundationSnapshot baseline = TimedSnapshot(sessionId: sessionId, revision: 0, host: initial,
                    local: initial, ready: false);
            port.Enqueue(sender: 1, envelope: new FoundationEnvelope(kind: "baseline", sessionId: sessionId,
                    playerId: "guest", actorId: "a2", bindingId: "b2", revision: 0, snapshot: baseline,
                    attemptId: attempt));
            guest.Update(elapsedMilliseconds: 0);
            guest.InstalledRevision.ShouldBe(expected: 0);
            guest.Ready.ShouldBeFalse();
            port.Sent.Count(predicate: row => row.Envelope.Kind is "ack").ShouldBe(expected: 1);
            if (initial > 0)
            {
                RejectTime(guest: guest, port: port, attempt: attempt, host: 0, local: initial);
            }

            PublishTime(guest: guest, port: port, attempt: attempt, snapshot: TimedSnapshot(sessionId: sessionId,
                    revision: 1, host: long.MaxValue, local: long.MaxValue, ready: true));
            guest.Ready.ShouldBeTrue();
            guest.Snapshot?.Revision.ShouldBe(expected: 1);
            RejectTime(guest: guest, port: port, attempt: attempt, host: 0, local: long.MaxValue);
            RejectTime(guest: guest, port: port, attempt: attempt, host: long.MaxValue, local: 0);
            PublishTime(guest: guest, port: port, attempt: attempt, snapshot: TimedSnapshot(sessionId: sessionId,
                    revision: 2, host: long.MaxValue, local: long.MaxValue, ready: true));
            guest.Snapshot?.Revision.ShouldBe(expected: 2);
            PublishTime(guest: guest, port: port, attempt: attempt, snapshot: TimedSnapshot(sessionId: sessionId,
                    revision: 3, host: 0, local: long.MaxValue, ready: true, hostActor: "replacement"));
            guest.Snapshot?.Participants[0].Actor.ActorId.ShouldBe(expected: "replacement");
            clock.Now = 9999;
            RejectTime(guest: guest, port: port, attempt: attempt, host: 0, local: 0);
            clock.Now = 10000;
            guest.Update(elapsedMilliseconds: 0);
            guest.Error.ShouldStartWith(expected: "Connection lost:");
            guest.Snapshot.ShouldBeNull();
        }
    }

    private static void RejectTime(FoundationCoordinator guest, FoundationHarnessConnection port, string attempt,
            long host, long local)
    {
        FoundationSnapshot before = guest.Snapshot ?? throw new InvalidOperationException(message: "BASELINE");
        long installed = guest.InstalledRevision;
        bool ready = guest.Ready;
        int replies = port.Sent.Count;
        long rejected = guest.RejectedPackets;
        PublishTime(guest: guest, port: port, attempt: attempt, snapshot: TimedSnapshot(sessionId: before.SessionId,
                revision: 100, host: host, local: local, ready: true,
                hostActor: before.Participants[0].Actor.ActorId));
        guest.Snapshot.ShouldBeSameAs(expected: before);
        guest.Snapshot?.Revision.ShouldBe(expected: before.Revision);
        guest.InstalledRevision.ShouldBe(expected: installed);
        guest.Ready.ShouldBe(expected: ready);
        port.Sent.Count.ShouldBe(expected: replies);
        guest.RejectedPackets.ShouldBe(expected: rejected + 1);
    }

    private static void PublishTime(FoundationCoordinator guest, FoundationHarnessConnection port, string attempt,
            FoundationSnapshot snapshot)
    {
        port.Enqueue(sender: 1, envelope: new FoundationEnvelope(kind: "state", sessionId: snapshot.SessionId,
                revision: snapshot.Revision, snapshot: snapshot, attemptId: attempt));
        guest.Update(elapsedMilliseconds: 0);
    }

    private static FoundationSnapshot TimedSnapshot(string sessionId, long revision, long host, long local,
            bool ready, string hostActor = "a1")
    {
        return new FoundationSnapshot(sessionId: sessionId, revision: revision, participants:
                [new FoundationParticipantState(bindingId: "local", label: "Host", slot: 1, ready: true,
                        actor: new FoundationActorState(sessionId: sessionId, playerId: "host", actorId: hostActor,
                                positionSubunits: 1000, facing: 1, elapsedMilliseconds: host)),
                    new FoundationParticipantState(bindingId: "b2", label: "Guest", slot: 2, ready: ready,
                            actor: new FoundationActorState(sessionId: sessionId, playerId: "guest", actorId: "a2",
                                    positionSubunits: 1000, facing: 1, elapsedMilliseconds: local))]);
    }

    private static void CheckOpaqueActors()
    {
        string[] ids = ["a", "a/../../Menu", "a:b", "a/b", "a?b", "..", "é", "e\u0301"];
        ids.Select(selector: FoundationActorNode.Name).Distinct(comparer: StringComparer.Ordinal).Count()
                .ShouldBe(expected: ids.Length);
        foreach (string id in ids)
        {
            var session = new FoundationSession(sessionId: "s", playerId: "host", actorId: "host-actor");
            session.AddParticipant(bindingId: "guest", playerId: "guest", actorId: id, label: "Guest").ShouldBeTrue();
            var state = new FoundationEnvelope(kind: "state", sessionId: "s", revision: session.Snapshot.Revision,
                    snapshot: session.Snapshot);
            FoundationProtocol.Decode(bytes: FoundationProtocol.Encode(envelope: state)).Snapshot?.Participants[1]
                    .Actor.ActorId.ShouldBe(expected: id);
            FoundationActorNode.Name(actorId: id).Skip(count: 6).All(predicate: character =>
                    character is >= '0' and <= '9' or >= 'A' and <= 'F').ShouldBeTrue();
        }
    }

    private static string[] InvalidDocuments(string json)
    {
        JsonNode root = JsonNode.Parse(json: json) ?? throw new InvalidOperationException(message: "JSON_ROOT");
        JsonNode duplicate = root.DeepClone();
        JsonNode roster = duplicate["Snapshot"]?["Participants"]
                ?? throw new InvalidOperationException(message: "JSON_ROSTER");
        roster[1] = roster[0]?.DeepClone();
        string duplicatePlayer = DuplicateField(json: json, field: "PlayerId");
        string duplicateActor = DuplicateField(json: json, field: "ActorId");
        JsonNode missingHost = root.DeepClone();
        (missingHost["Snapshot"]?["Participants"] as JsonArray)?.RemoveAt(index: 0);
        return [json[..^1], new string(c: ' ', count: 32769), duplicate.ToJsonString(),
            duplicatePlayer, duplicateActor, missingHost.ToJsonString(),
            json.Replace(oldValue: "foundation.fixture.v1", newValue: "other",
                    comparisonType: StringComparison.Ordinal),
            json.Replace(oldValue: "-8000", newValue: "NaN", comparisonType: StringComparison.Ordinal),
            json.Replace(oldValue: "-8000", newValue: "59001", comparisonType: StringComparison.Ordinal),
            json.Replace(oldValue: "\"ProtocolVersion\":2", newValue: "\"ProtocolVersion\":1",
                    comparisonType: StringComparison.Ordinal),
            json.Replace(oldValue: "\"Kind\":\"state\"", newValue: "\"Kind\":\"state\",\"extra\":0",
                    comparisonType: StringComparison.Ordinal),
            json.Replace(oldValue: "\"SessionId\":\"s\"", newValue: "\"SessionId\":\"other\"",
                    comparisonType: StringComparison.Ordinal),
            json.Replace(oldValue: "\"Revision\":2", newValue: "\"Revision\":1",
                    comparisonType: StringComparison.Ordinal),
            "{\"nested\":" + new string(c: '[', count: 20) + "0" + new string(c: ']', count: 20) + "}"];
    }

    private static string DuplicateField(string json, string field)
    {
        JsonNode root = JsonNode.Parse(json: json) ?? throw new InvalidOperationException(message: "JSON_ROOT");
        JsonNode roster = root["Snapshot"]?["Participants"]
                ?? throw new InvalidOperationException(message: "JSON_ROSTER");
        JsonNode actor = roster[1]?["Actor"] ?? throw new InvalidOperationException(message: "JSON_ACTOR");
        actor[field] = roster[0]?["Actor"]?[field]?.DeepClone();
        return root.ToJsonString();
    }
}
