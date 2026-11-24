using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

/// <summary>Maps bounded JSON to explicit detached constructors before any authority mutation.</summary>
internal static class FoundationProtocol
{
    internal const string Content =
            "foundation.fixture.v1|foundation.ground.v1|foundation.building.v1|foundation.player.v1";
    internal const int MaximumBytes = 32768;
    internal const int Version = 2;
    internal const int MaximumViewRows = 4096;
    internal const int MaximumViewParts = 256;

    internal static byte[] Encode(FoundationEnvelope envelope)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value: envelope);
        if (bytes.Length > MaximumBytes - NativeHeaderBytes)
        {
            throw new InvalidDataException(message: "WIRE_SIZE");
        }

        return bytes;
    }

    internal static bool WindowScope(string scope, bool external)
    {
        string prefix = external ? "external." : "own.";
        return scope.StartsWith(value: prefix, comparisonType: StringComparison.Ordinal)
                && scope.AsSpan(start: prefix.Length) is "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8"
                        or "9" or "10" or "11" or "12" or "13" or "14" or "15" or "16";
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
        Compatible(value: root, version: "ProtocolVersion", expected: Version);
        Shape(value: root, names: ["Kind", "SessionId", "PlayerId", "ActorId", "BindingId", "Label", "Sequence",
            "Direction", "Revision", "Snapshot", "ProtocolVersion", "ContentId", "AttemptId", "Game"]);
        var envelope = new FoundationEnvelope(kind: Text(value: root, name: "Kind"),
                sessionId: Text(value: root, name: "SessionId"), playerId: Text(value: root, name: "PlayerId"),
                actorId: Text(value: root, name: "ActorId"), bindingId: Text(value: root, name: "BindingId"),
                label: Text(value: root, name: "Label"), sequence:
                        root.GetProperty(propertyName: "Sequence").GetInt64(),
                direction: root.GetProperty(propertyName: "Direction").GetInt32(),
                revision: root.GetProperty(propertyName: "Revision").GetInt64(),
                snapshot: root.GetProperty(propertyName: "Snapshot").ValueKind == JsonValueKind.Null ? null
                        : ReadSnapshot(value: root.GetProperty(propertyName: "Snapshot")),
                attemptId: Text(value: root, name: "AttemptId"),
                game: root.GetProperty(propertyName: "Game").Deserialize<FoundationRunFrame>(options: _gameOptions));
        Validate(envelope: envelope);
        return envelope;
    }

    internal static ImmutableArray<byte[]> EncodeView(ImmutableArray<JsonElement> rows,
            Func<FoundationRunFrame, FoundationEnvelope> envelope, FoundationRunFrame template)
    {
        if (rows.IsDefault || rows.Length > MaximumViewRows)
        {
            throw new InvalidDataException(message: "WIRE_VIEW_ROWS");
        }

        if (rows.IsEmpty)
        {
            return [Encode(envelope: envelope(arg: template with { Parts = 1 }))];
        }

        ImmutableArray<ImmutableArray<JsonElement>>.Builder chunks =
                ImmutableArray.CreateBuilder<ImmutableArray<JsonElement>>();
        for (int offset = 0; offset < rows.Length;)
        {
            int length = FittingRows(rows: rows, offset: offset, envelope: envelope, template: template);
            if (length <= 0 || chunks.Count == MaximumViewParts)
            {
                throw new InvalidDataException(message: "WIRE_VIEW_PARTS");
            }

            chunks.Add(item: rows.Slice(start: offset, length: length));
            offset += length;
        }

        return [.. chunks.Select(selector: (chunk, index) => Encode(envelope: envelope(arg: template with
        {
            Part = index,
            Parts = chunks.Count,
            Rows = chunk,
        })))];
    }

    private const int NativeHeaderBytes = 8;
    private static readonly string[] _contentFields = ["FixtureId", "GroundId", "BuildingId", "PlayerContentId"];
    private static readonly JsonSerializerOptions _gameOptions = new()
    {
        AllowDuplicateProperties = false,
        RespectNullableAnnotations = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    private static int FittingRows(ImmutableArray<JsonElement> rows, int offset,
            Func<FoundationRunFrame, FoundationEnvelope> envelope, FoundationRunFrame template)
    {
        int first = 1;
        int last = rows.Length - offset;
        while (first <= last)
        {
            int middle = first + (last - first) / 2;
            FoundationEnvelope candidate = envelope(arg: template with
            {
                Part = MaximumViewParts - 1,
                Parts = MaximumViewParts,
                Rows = rows.Slice(start: offset, length: middle),
            });
            int size = JsonSerializer.SerializeToUtf8Bytes(value: candidate).Length;
            if (size <= MaximumBytes - NativeHeaderBytes)
            {
                first = middle + 1;
            }
            else
            {
                last = middle - 1;
            }
        }

        return last;
    }

    private static void Validate(FoundationEnvelope envelope)
    {
        if (envelope.Kind is not ("hello" or "baseline" or "ack" or "state" or "ping" or "bye"
                or "end" or "end-ack" or "cancel" or "reject" or "action" or "action-result" or "view-part"
                or "view-revoke") || envelope.Sequence < 0 || envelope.Revision < 0
                || envelope.Direction is < -1 or > 1 || envelope.Label.EnumerateRunes().Count() > 256
                || new[] { envelope.SessionId, envelope.PlayerId, envelope.ActorId, envelope.BindingId,
                    envelope.AttemptId }
                        .Any(predicate: value => value.Length > 128))
        {
            throw new InvalidDataException(message: "WIRE_VALUE");
        }

        bool identity = new[] { envelope.SessionId, envelope.PlayerId, envelope.ActorId, envelope.BindingId }
                .All(predicate: value => !string.IsNullOrWhiteSpace(value: value));
        if (envelope.Kind is "hello" && (string.IsNullOrWhiteSpace(value: envelope.PlayerId)
                || string.IsNullOrWhiteSpace(value: envelope.Label)) || envelope.Kind is "ack"
                        && !identity || envelope.Kind is "baseline" or "state" && (envelope.Snapshot is null
                                || !StringComparer.Ordinal.Equals(x: envelope.SessionId,
                                        y: envelope.Snapshot.SessionId)
                                || envelope.Revision != envelope.Snapshot.Revision))
        {
            throw new InvalidDataException(message: "WIRE_REQUIRED");
        }

        if (envelope.Snapshot is not null && envelope.Kind is not ("baseline" or "state")
                || !ValidRunFrame(envelope: envelope, identity: identity))
        {
            throw new InvalidDataException(message: "WIRE_GAMEPLAY");
        }
    }

    private static bool ValidRunFrame(FoundationEnvelope envelope, bool identity)
    {
        if (envelope.Game is not { } frame)
        {
            return envelope.Kind is not ("action" or "action-result" or "view-part" or "view-revoke");
        }

        if (frame.Epoch < 0 || frame.View < 0 || frame.Token < 0 || frame.Part < 0 || frame.Parts < 0
                || frame.Survivor.Length > 128 || frame.Rows.IsDefault
                || frame.Rows.Length > MaximumViewRows || frame.Body is { ValueKind: not JsonValueKind.Object }
                || envelope.Label.Length > 0 || envelope.Direction != 0
                || frame.Rows.Any(predicate: row => row.ValueKind != JsonValueKind.Object))
        {
            return false;
        }

        return envelope.Kind switch
        {
            "action" => identity && envelope.Sequence > 0 && ValidAction(frame: frame)
                    && frame.Status.Length == 0 && !frame.Replayed && NoViewScope(frame: frame),
            "action-result" => identity && envelope.Sequence > 0 && frame.Status.Length is >= 1 and <= 32
                    && KnownAction(action: frame.Action) && frame.Body is null && frame.Rows.IsEmpty
                    && frame.Survivor.Length == 0 && frame.Epoch == 0 && NoViewScope(frame: frame),
            "view-part" => identity && ValidScope(scope: frame.Scope) && frame.View > 0 && frame.Body is not null
                    && frame.Parts is >= 1 and <= MaximumViewParts && frame.Part < frame.Parts
                    && NoActionFields(frame: frame) && ValidScopeBody(frame: frame),
            "view-revoke" => identity && ValidScope(scope: frame.Scope) && frame.View > 0 && frame.Body is null
                    && frame.Rows.IsEmpty && frame.Part == 0 && frame.Parts == 0 && NoActionFields(frame: frame)
                    && frame.Epoch == 0 && frame.Token == 0,
            "state" or "baseline" => frame.Body is { } body && frame.Rows.IsEmpty && NoActionFields(frame: frame)
                    && NoViewScope(frame: frame) && frame.Epoch == 0 && ValidWorld(body: body),
            _ => false,
        };
    }

    private static bool NoViewScope(FoundationRunFrame frame)
    {
        return frame.Scope.Length == 0 && frame.View == 0 && frame.Token == 0 && frame.Part == 0 && frame.Parts == 0;
    }

    private static bool NoActionFields(FoundationRunFrame frame)
    {
        return frame.Action.Length == 0 && frame.Status.Length == 0 && frame.Survivor.Length == 0 && !frame.Replayed;
    }

    private static bool ValidAction(FoundationRunFrame frame)
    {
        return KnownAction(action: frame.Action) && frame.Epoch > 0 && frame.Rows.IsEmpty
                && frame.Body is { } body && (frame.Action is "respawn"
                        ? frame.Survivor.Length == 0 && !body.EnumerateObject().Any()
                        : frame.Survivor.Length > 0);
    }

    private static bool KnownAction(string action)
    {
        return action is "move" or "attack" or "food" or "select" or "browse" or "close" or "drop" or "quick"
                or "place" or "merge" or "respawn" or "window" or "assign" or "workspace";
    }

    private static bool ValidScope(string scope)
    {
        return scope is "status" or "own" or "external" || WindowScope(scope: scope, external: false)
                || WindowScope(scope: scope, external: true);
    }

    private static bool ValidScopeBody(FoundationRunFrame frame)
    {
        if (frame.Body is not { } body)
        {
            return false;
        }

        switch (frame.Scope)
        {
            case "status":
                Shape(value: body, names: ["SurvivorId", "Generation", "DeathGeneration", "Status", "OpensAt",
                    "DeathX", "DeathY", "Health", "Stamina", "MaximumHealth", "MaximumStamina", "RootTime",
                    "Teammates"]);
                ValidateRows(body: body, member: "Teammates", fields: ["BindingId", "Slot", "SurvivorId"]);
                return frame.Rows.IsEmpty && frame.Token == 0 && frame.Parts == 1
                        && body.GetProperty(propertyName: "Generation").GetInt64() == frame.Epoch;
            case "own":
                Shape(value: body, names: ["SurvivorId", "ContainerId", "Width", "Height", "Nearby", "Quick",
                    "Availability"]);
                return frame.Epoch > 0 && frame.Token == 0 && ValidOwnControls(body: body)
                        && frame.Rows.All(predicate: ValidPrivateItem);
            case "external":
                Shape(value: body, names: ["Kind", "TargetId", "Page", "PageCount", "ContainerId", "Width",
                    "Height", "Effect"]);
                return frame.Epoch > 0 && frame.Token > 0 && ValidPageEffect(body: body)
                        && frame.Rows.All(predicate: ValidPrivateItem);
            default:
                Shape(value: body, names: ["ContainerId", "Width", "Height"]);
                return frame.Epoch > 0 && (WindowScope(scope: frame.Scope, external: true)
                        ? frame.Token > 0 : frame.Token == 0)
                        && body.GetProperty(propertyName: "ContainerId").GetInt64() > 0
                        && body.GetProperty(propertyName: "Width").GetInt32() is >= 1 and <= 64
                        && body.GetProperty(propertyName: "Height").GetInt32() is >= 1 and <= 64
                        && frame.Rows.All(predicate: ValidPrivateItem);
        }
    }

    private static bool ValidPageEffect(JsonElement body)
    {
        JsonElement effect = body.GetProperty(propertyName: "Effect");
        if (effect.ValueKind == JsonValueKind.Null)
        {
            return true;
        }

        Shape(value: effect, names: ["Sequence", "PageNumber", "MorePages", "Cells"]);
        ValidateRows(body: effect, member: "Cells", fields: ["X", "Y"]);
        int page = body.GetProperty(propertyName: "Page").GetInt32();
        int first = effect.GetProperty(propertyName: "PageNumber").GetInt32();
        return Text(value: body, name: "Kind") is "Stash" && first == page
                && effect.GetProperty(propertyName: "Sequence").GetInt64() > 0
                && effect.GetProperty(propertyName: "MorePages").GetInt32() >= 0;
    }

    private static bool ValidOwnControls(JsonElement body)
    {
        ValidateRows(body: body, member: "Nearby", fields: ["Kind", "Id"]);
        ValidateRows(body: body, member: "Quick", fields: ["Slot", "ItemId", "Stamp"]);
        return body.GetProperty(propertyName: "Nearby").EnumerateArray().All(predicate: target =>
                Text(value: target, name: "Kind") is "Stash" or "Npc" or "Pile" or "Corpse"
                        && Text(value: target, name: "Id").Length is >= 1 and <= 128)
                && body.GetProperty(propertyName: "Quick").EnumerateArray().All(predicate: quick =>
                        quick.GetProperty(propertyName: "Slot").GetInt32() is >= 1 and <= 4
                                && quick.GetProperty(propertyName: "ItemId").GetInt64() > 0
                                && quick.GetProperty(propertyName: "Stamp").GetString() is null or { Length: 64 });
    }

    private static bool ValidPrivateItem(JsonElement row)
    {
        Shape(value: row, names: ["Id", "DefinitionId", "Quantity", "Condition", "StorageWidth", "StorageHeight",
            "FootprintWidth", "FootprintHeight", "CanonicalIcon", "Rotatable", "MaxStack", "MaximumCondition",
            "Legacy", "Location", "Stamp", "WeightKilograms", "EquipmentSlot", "Food"]);
        JsonElement location = row.GetProperty(propertyName: "Location");
        string kind = Text(value: location, name: "Kind");
        switch (kind)
        {
            case "equipment" or "corpse":
                Shape(value: location, names: ["Kind", "TargetId", "Slot"]);
                break;
            case "ground":
                Shape(value: location, names: ["Kind", "TargetId"]);
                break;
            case "stash" or "container":
                Shape(value: location, names: ["Kind", "GridId", "X", "Y", "Rotated", "Width", "Height"]);
                break;
            default:
                return false;
        }

        return row.GetProperty(propertyName: "Id").GetInt64() > 0
                && row.GetProperty(propertyName: "Quantity").GetInt32() is >= 1 and <= 10000
                && row.GetProperty(propertyName: "FootprintWidth").GetInt32() is >= 1 and <= 64
                && row.GetProperty(propertyName: "FootprintHeight").GetInt32() is >= 1 and <= 64
                && Text(value: row, name: "DefinitionId").Length is >= 1 and <= 128
                && Text(value: row, name: "Stamp").Length == 64;
    }

    private static bool ValidWorld(JsonElement body)
    {
        Shape(value: body, names: ["Elapsed", "OriginX", "Ground", "CampLeft", "CampRight", "Actors", "Defenses",
            "Loot", "Arrows", "Corpses"]);
        foreach (JsonElement actor in body.GetProperty(propertyName: "Actors").EnumerateArray())
        {
            Shape(value: actor, names: ["Id", "X", "Y", "Facing", "Hostile", "BindingId", "Equipment",
                "Movement", "StrikeAt", "StrikeUntil", "AimX", "AimY", "FlinchAt", "ArrowReady"]);
            ValidateRows(body: actor, member: "Equipment", fields: ["Slot", "DefinitionId", "Broken"]);
        }

        foreach (JsonElement corpse in body.GetProperty(propertyName: "Corpses").EnumerateArray())
        {
            Shape(value: corpse, names: ["Id", "SurvivorId", "X", "Y", "Facing", "Hostile", "At", "Equipment"]);
            ValidateRows(body: corpse, member: "Equipment", fields: ["Slot", "DefinitionId", "Broken"]);
        }

        ValidateRows(body: body, member: "Defenses", fields: ["Id", "X", "Y", "Living", "Completed", "IsWall"]);
        ValidateRows(body: body, member: "Loot", fields: ["Id", "X", "Y", "Kind"]);
        ValidateRows(body: body, member: "Arrows", fields: ["Id", "X", "Y", "VelocityX", "VelocityY"]);
        _ = FoundationEquipmentProjection.Read(revision: 0, world: body);
        return true;
    }

    private static void ValidateRows(JsonElement body, string member, string[] fields)
    {
        foreach (JsonElement row in body.GetProperty(propertyName: member).EnumerateArray())
        {
            Shape(value: row, names: fields);
        }
    }

    private static FoundationSnapshot ReadSnapshot(JsonElement value)
    {
        Shape(value: value, names: ["SessionId", "Revision", "Participants", "SchemaVersion", "ContentId"]);
        Compatible(value: value, version: "SchemaVersion", expected: 1);
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

    private static void Compatible(JsonElement value, string version, int expected)
    {
        if (value.GetProperty(propertyName: version).GetInt32() != expected
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

        string[] actual = [.. value.EnumerateObject().Select(selector: property => property.Name)];
        if (actual.Length != names.Length || actual.Distinct(comparer: StringComparer.Ordinal).Count()
                != actual.Length || actual.Except(second: names, comparer: StringComparer.Ordinal).Any())
        {
            throw new InvalidDataException(message: "WIRE_SHAPE");
        }
    }
}
