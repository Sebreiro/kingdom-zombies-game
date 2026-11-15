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
public sealed class FoundationLanProtocolTests
{
    /// <summary>Malformed content, identities, shape and bounds cannot replace the installed publication.</summary>
    [Fact]
    public void DetachedSnapshotValidationPrecedesPublication()
    {
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
        ProofProtocol.Record(id: "core.protocol", diagnostic: "DETACHED_SHAPE_CONTENT_OWNER_REVISION_ATOMICITY");
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
            json.Replace(oldValue: "\"ProtocolVersion\":1", newValue: "\"ProtocolVersion\":2",
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
