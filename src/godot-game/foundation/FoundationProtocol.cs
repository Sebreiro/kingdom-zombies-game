using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

/// <summary>Maps bounded JSON to explicit detached constructors before any authority mutation.</summary>
internal static class FoundationProtocol
{
    internal const string Content =
            "foundation.fixture.v1|foundation.ground.v1|foundation.building.v1|foundation.player.v1";
    internal const int MaximumBytes = 32768;

    internal static byte[] Encode(FoundationEnvelope envelope)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value: envelope);
        if (bytes.Length > MaximumBytes)
        {
            throw new InvalidDataException(message: "WIRE_SIZE");
        }

        return bytes;
    }

    internal static FoundationEnvelope Decode(ReadOnlyMemory<byte> bytes)
    {
        if (bytes.Length is < 2 or > MaximumBytes)
        {
            throw new InvalidDataException(message: "WIRE_SIZE");
        }

        using JsonDocument document = JsonDocument.Parse(utf8Json: bytes, options:
                new JsonDocumentOptions { MaxDepth = 16 });
        JsonElement root = document.RootElement;
        Shape(value: root, names: ["Kind", "SessionId", "PlayerId", "ActorId", "BindingId", "Label", "Sequence",
            "Direction", "Revision", "Snapshot", "ProtocolVersion", "ContentId"]);
        Compatible(value: root, version: "ProtocolVersion");
        var envelope = new FoundationEnvelope(kind: Text(value: root, name: "Kind"),
                sessionId: Text(value: root, name: "SessionId"), playerId: Text(value: root, name: "PlayerId"),
                actorId: Text(value: root, name: "ActorId"), bindingId: Text(value: root, name: "BindingId"),
                label: Text(value: root, name: "Label"), sequence:
                        root.GetProperty(propertyName: "Sequence").GetInt64(),
                direction: root.GetProperty(propertyName: "Direction").GetInt32(),
                revision: root.GetProperty(propertyName: "Revision").GetInt64(),
                snapshot: root.GetProperty(propertyName: "Snapshot").ValueKind == JsonValueKind.Null ? null
                        : ReadSnapshot(value: root.GetProperty(propertyName: "Snapshot")));
        Validate(envelope: envelope);
        return envelope;
    }

    private static readonly string[] _contentFields = ["FixtureId", "GroundId", "BuildingId", "PlayerContentId"];

    private static void Validate(FoundationEnvelope envelope)
    {
        if (envelope.Kind is not ("hello" or "baseline" or "ack" or "state" or "intent" or "ping" or "bye"
                or "end" or "reject") || envelope.Sequence < 0 || envelope.Revision < 0
                || envelope.Direction is < -1 or > 1 || envelope.Label.EnumerateRunes().Count() > 256
                || new[] { envelope.SessionId, envelope.PlayerId, envelope.ActorId, envelope.BindingId }
                        .Any(predicate: value => value.Length > 128))
        {
            throw new InvalidDataException(message: "WIRE_VALUE");
        }

        bool identity = new[] { envelope.SessionId, envelope.PlayerId, envelope.ActorId, envelope.BindingId }
                .All(predicate: value => !string.IsNullOrWhiteSpace(value: value));
        if (envelope.Kind is "hello" && (string.IsNullOrWhiteSpace(value: envelope.PlayerId)
                || string.IsNullOrWhiteSpace(value: envelope.Label)) || envelope.Kind is "intent" or "ack"
                        && !identity || envelope.Kind is "baseline" or "state" && (envelope.Snapshot is null
                                || !StringComparer.Ordinal.Equals(x: envelope.SessionId,
                                        y: envelope.Snapshot.SessionId)
                                || envelope.Revision != envelope.Snapshot.Revision))
        {
            throw new InvalidDataException(message: "WIRE_REQUIRED");
        }
    }

    private static FoundationSnapshot ReadSnapshot(JsonElement value)
    {
        Shape(value: value, names: ["SessionId", "Revision", "Participants", "SchemaVersion", "ContentId"]);
        Compatible(value: value, version: "SchemaVersion");
        JsonElement array = value.GetProperty(propertyName: "Participants");
        if (array.ValueKind != JsonValueKind.Array || array.GetArrayLength() is < 1 or > 4)
        {
            throw new InvalidDataException(message: "WIRE_ROSTER");
        }

        string session = Text(value: value, name: "SessionId");
        var participants = new List<FoundationParticipantState>();
        foreach (JsonElement participant in array.EnumerateArray())
        {
            participants.Add(item: ReadParticipant(value: participant, sessionId: session));
        }

        return new FoundationSnapshot(sessionId: session, revision: value.GetProperty(propertyName: "Revision")
                .GetInt64(), participants: participants);
    }

    private static FoundationParticipantState ReadParticipant(JsonElement value, string sessionId)
    {
        Shape(value: value, names: ["BindingId", "Label", "Slot", "Ready", "Actor"]);
        JsonElement actor = value.GetProperty(propertyName: "Actor");
        Shape(value: actor, names: ["SessionId", "PlayerId", "ActorId", "PositionSubunits", "Facing",
            "ElapsedMilliseconds", "SchemaVersion", "FixtureId", "GroundId", "BuildingId", "PlayerContentId",
            "GroundSubunits"]);
        ValidateActorContent(value: actor, sessionId: sessionId);
        var state = new FoundationActorState(sessionId: Text(value: actor, name: "SessionId"),
                playerId: Text(value: actor, name: "PlayerId"), actorId: Text(value: actor, name: "ActorId"),
                positionSubunits: actor.GetProperty(propertyName: "PositionSubunits").GetInt64(),
                facing: actor.GetProperty(propertyName: "Facing").GetInt32(),
                elapsedMilliseconds: actor.GetProperty(propertyName: "ElapsedMilliseconds").GetInt64());
        return new FoundationParticipantState(bindingId: Text(value: value, name: "BindingId"),
                label: Text(value: value, name: "Label"), slot: value.GetProperty(propertyName: "Slot").GetInt32(),
                ready: value.GetProperty(propertyName: "Ready").GetBoolean(), actor: state);
    }

    private static void ValidateActorContent(JsonElement value, string sessionId)
    {
        string content = string.Join(separator: "|", values: _contentFields.Select(selector: name =>
                Text(value: value, name: name)));
        if (!StringComparer.Ordinal.Equals(x: content, y: Content)
                || !StringComparer.Ordinal.Equals(x: Text(value: value, name: "SessionId"), y: sessionId)
                || value.GetProperty(propertyName: "SchemaVersion").GetInt32() != 1
                || value.GetProperty(propertyName: "GroundSubunits").GetInt64() != 0)
        {
            throw new InvalidDataException(message: "WIRE_ACTOR_CONTENT");
        }
    }

    private static void Compatible(JsonElement value, string version)
    {
        if (value.GetProperty(propertyName: version).GetInt32() != 1
                || !StringComparer.Ordinal.Equals(x: Text(value: value, name: "ContentId"), y: Content))
        {
            throw new InvalidDataException(message: "WIRE_INCOMPATIBLE");
        }
    }

    private static string Text(JsonElement value, string name)
    {
        return value.GetProperty(propertyName: name).GetString()
                ?? throw new InvalidDataException(message: "WIRE_STRING");
    }

    private static void Shape(JsonElement value, string[] names)
    {
        if (value.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException(message: "WIRE_OBJECT");
        }

        string[] actual = [..value.EnumerateObject().Select(selector: property => property.Name)];
        if (actual.Length != names.Length || actual.Distinct(comparer: StringComparer.Ordinal).Count()
                != actual.Length || actual.Except(second: names, comparer: StringComparer.Ordinal).Any())
        {
            throw new InvalidDataException(message: "WIRE_SHAPE");
        }
    }
}
