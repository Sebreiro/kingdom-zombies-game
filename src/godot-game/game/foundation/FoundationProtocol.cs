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
    internal const int MaximumWindowSelectors = 16;
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

    internal static bool ValidateWorld(JsonElement body)
    {
        Shape(value: body, names: ["Elapsed", "OriginX", "Ground", "CampLeft", "CampRight", "Actors", "Defenses",
            "Loot", "Arrows", "Corpses"]);
        foreach (string collection in new[] { "Actors", "Corpses", "Defenses", "Loot", "Arrows" })
        {
            var identities = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonElement row in body.GetProperty(propertyName: collection).EnumerateArray())
            {
                ValidatePublicRow(collection: collection, row: row);
                if (!identities.Add(item: row.GetProperty(propertyName: "Id").ToString()))
                {
                    throw new InvalidDataException(message: "WIRE_PUBLIC_ID");
                }
            }
        }

        _ = FoundationEquipmentProjection.Read(revision: 0, world: body);
        return true;
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
            Func<FoundationRunFrame, FoundationEnvelope> envelope, FoundationRunFrame template,
            int maximumParts = MaximumViewParts, int maximumRows = MaximumViewRows)
    {
        if (rows.IsDefault || rows.Length > maximumRows)
        {
            throw new InvalidDataException(message: "WIRE_VIEW_ROWS");
        }

        if (rows.IsEmpty)
        {
            return [Encode(envelope: envelope(arg: template with { Parts = 1 }))];
        }

        ImmutableArray<ImmutableArray<JsonElement>>.Builder chunks =
                ImmutableArray.CreateBuilder<ImmutableArray<JsonElement>>();
        int overhead = JsonSerializer.SerializeToUtf8Bytes(value: envelope(arg: template with
        {
            Part = maximumParts - 1,
            Parts = maximumParts,
        })).Length;
        Partition(rows: rows, maximumParts: maximumParts, available: MaximumBytes - NativeHeaderBytes - overhead,
                chunks: chunks);

        return [.. chunks.Select(selector: (chunk, index) => Encode(envelope: envelope(arg: template with
        {
            Part = index,
            Parts = chunks.Count,
            Rows = chunk,
        })))];
    }

    internal static int? ExternalLoosePageLength(JsonElement body)
    {
        JsonElement number = body.GetProperty(propertyName: "ItemPage");
        if (number.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        const int Size = FoundationSession.ExternalItemsPageSize;
        int page = number.GetInt32();
        int count = body.GetProperty(propertyName: "ItemCount").GetInt32();
        int total = count > 0 ? 1 + (count - 1) / Size : 1;
        if (count < 0 || page < 1 || page > total)
        {
            throw new InvalidDataException(message: "WIRE_EXTERNAL_PAGE");
        }

        return Math.Min(val1: Size, val2: count - (page - 1) * Size);
    }

    internal static bool IsLooseItem(JsonElement item)
    {
        return Text(value: item.GetProperty(propertyName: "Location"), name: "Kind") is "ground";
    }

    internal static byte[]? ReadStashEffectMask(JsonElement body, JsonElement effect)
    {
        int width = body.GetProperty(propertyName: "Width").GetInt32();
        int height = body.GetProperty(propertyName: "Height").GetInt32();
        if (width is < 1 or > 64 || height is < 1 or > 64)
        {
            return null;
        }

        int cells = width * height;
        byte[] mask = new byte[(cells + 7) / 8];
        string encoded = Text(value: effect, name: "Mask");
        int tail = cells % 8;
        return encoded.Length == 4 * ((mask.Length + 2) / 3)
                && Convert.TryFromBase64String(s: encoded, bytes: mask, bytesWritten: out int length)
                && length == mask.Length && (tail == 0 || mask[^1] >> tail == 0)
                && StringComparer.Ordinal.Equals(x: Convert.ToBase64String(inArray: mask), y: encoded) ? mask : null;
    }

    private const int NativeHeaderBytes = 8;
    private static readonly string[] _contentFields = ["FixtureId", "GroundId", "BuildingId", "PlayerContentId"];
    private static readonly JsonSerializerOptions _gameOptions = new()
    {
        AllowDuplicateProperties = false,
        RespectNullableAnnotations = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    private static void Partition(ImmutableArray<JsonElement> rows, int maximumParts, int available,
            ImmutableArray<ImmutableArray<JsonElement>>.Builder chunks)
    {
        int first = 0;
        int size = 0;
        for (int index = 0; index < rows.Length; index++)
        {
            int bytes = JsonSerializer.SerializeToUtf8Bytes(value: rows[index]).Length;
            if (bytes > available)
            {
                throw new InvalidDataException(message: "WIRE_VIEW_PARTS");
            }

            if (index > first && size + 1 + bytes > available)
            {
                chunks.Add(item: rows.Slice(start: first, length: index - first));
                if (chunks.Count == maximumParts)
                {
                    throw new InvalidDataException(message: "WIRE_VIEW_PARTS");
                }

                first = index;
                size = 0;
            }

            size += bytes + (index == first ? 0 : 1);
        }

        chunks.Add(item: rows.Slice(start: first, length: rows.Length - first));
    }

    private static void Validate(FoundationEnvelope envelope)
    {
        if (envelope.Kind is not ("hello" or "baseline" or "ack" or "state" or "ping" or "bye"
                or "end" or "end-ack" or "cancel" or "reject" or "action" or "action-result" or "view-part"
                or "view-revoke" or "world-part") || envelope.Sequence < 0 || envelope.Revision < 0
                || envelope.Direction is < -1 or > 1 || envelope.Label.EnumerateRunes().Count() > 256
                || new[] { envelope.SessionId, envelope.PlayerId, envelope.ActorId, envelope.BindingId,
                    envelope.AttemptId }
                        .Any(predicate: value => value.Length > 128))
        {
            throw new InvalidDataException(message: "WIRE_VALUE");
        }

        bool identity = new[] { envelope.SessionId, envelope.PlayerId, envelope.ActorId, envelope.BindingId }
                .All(predicate: value => !string.IsNullOrWhiteSpace(value: value));
        bool missingRoster = envelope.Kind is "baseline" or "state" or "world-part"
                && (envelope.Snapshot is null
                        || !StringComparer.Ordinal.Equals(x: envelope.SessionId, y: envelope.Snapshot.SessionId)
                        || envelope.Revision != envelope.Snapshot.Revision);
        if (envelope.Kind is "hello" && (string.IsNullOrWhiteSpace(value: envelope.PlayerId)
                || string.IsNullOrWhiteSpace(value: envelope.Label)) || envelope.Kind is "ack" && !identity
                || missingRoster)
        {
            throw new InvalidDataException(message: "WIRE_REQUIRED");
        }

        if (envelope.Snapshot is not null && envelope.Kind is not ("baseline" or "state" or "world-part")
                || !ValidRunFrame(envelope: envelope, identity: identity))
        {
            throw new InvalidDataException(message: "WIRE_GAMEPLAY");
        }
    }

    private static bool ValidRunFrame(FoundationEnvelope envelope, bool identity)
    {
        if (envelope.Game is not { } frame)
        {
            return envelope.Kind is not ("action" or "action-result" or "view-part" or "view-revoke" or "world-part");
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
                    && frame.Epoch > 0 && (frame.Action is "respawn" ? frame.Survivor.Length == 0
                            : frame.Survivor.Length > 0) && NoViewScope(frame: frame),
            "view-part" => identity && ValidScope(scope: frame.Scope) && frame.View > 0 && frame.Body is not null
                    && frame.Parts is >= 1 and <= MaximumViewParts && frame.Part < frame.Parts
                    && NoActionFields(frame: frame) && ValidScopeBody(frame: frame),
            "view-revoke" => identity && ValidScope(scope: frame.Scope) && frame.View > 0 && frame.Body is null
                    && frame.Rows.IsEmpty && frame.Part == 0 && frame.Parts == 0 && NoActionFields(frame: frame)
                    && frame.Epoch == 0 && frame.Token == 0,
            "state" or "baseline" => frame.Body is { } body && frame.Rows.IsEmpty && NoActionFields(frame: frame)
                    && NoViewScope(frame: frame) && frame.Epoch == 0 && ValidateWorld(body: body),
            "world-part" => ValidPublicPart(frame: frame),
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
                or "place" or "merge" or "respawn" or "window" or "assign" or "workspace" or "nearby-page"
                or "external-items-page";
    }

    private static bool ValidScope(string scope)
    {
        return scope is "status" or "nearby" or "own" or "external" || WindowScope(scope: scope, external: false)
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
                    "Teammates", "Hud"]);
                ValidateRows(body: body, member: "Teammates", fields: ["BindingId", "Slot", "SurvivorId"]);
                return frame.Rows.IsEmpty && frame.Token == 0 && frame.Parts == 1
                        && body.GetProperty(propertyName: "Generation").GetInt64() == frame.Epoch
                        && ValidHud(hud: body.GetProperty(propertyName: "Hud"), alive: frame.Epoch > 0);
            case "nearby":
                return ValidNearbyPage(frame: frame, body: body);
            case "own":
                Shape(value: body, names: ["SurvivorId", "ContainerId", "Width", "Height", "Quick"]);
                return frame.Epoch > 0 && frame.Token == 0 && ValidOwnControls(body: body)
                        && frame.Rows.All(predicate: ValidPrivateItem);
            case "external":
                return ValidExternal(frame: frame, body: body);
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

    private static bool ValidNearbyPage(FoundationRunFrame frame, JsonElement body)
    {
        Shape(value: body, names: ["Availability", "Page", "Count", "SelectedToken"]);
        const int Size = FoundationSession.NearbyPageSize;
        int count = body.GetProperty(propertyName: "Count").GetInt32();
        int page = body.GetProperty(propertyName: "Page").GetInt32();
        long token = body.GetProperty(propertyName: "SelectedToken").GetInt64();
        string availability = Text(value: body, name: "Availability");
        return frame.Epoch > 0 && frame.Token == 0 && frame.Parts == 1 && page >= 1 && count >= 0 && token >= 0
                && availability is "Ready" or "NoTargets" or "AccessLost"
                && (availability is "AccessLost" || (count == 0) == (availability is "NoTargets"))
                && (token == 0 || availability is "Ready")
                && page <= (count == 0 ? 1 : 1 + (count - 1) / Size)
                && frame.Rows.Length == Math.Min(val1: Size, val2: count - (page - 1) * Size)
                && frame.Rows.All(predicate: ValidNearbyTarget);
    }

    private static bool ValidExternal(FoundationRunFrame frame, JsonElement body)
    {
        Shape(value: body, names: ["Kind", "TargetId", "Page", "PageCount", "ItemPage", "ItemCount",
            "GearCount", "ContainerId", "Width", "Height", "Effect"]);
        string kind = Text(value: body, name: "Kind");
        string target = Text(value: body, name: "TargetId");
        if (frame.Epoch <= 0 || frame.Token <= 0 || kind is not ("Stash" or "Npc" or "Pile" or "Corpse")
                || target.Length is < 1 or > 128 || !ValidPageEffect(body: body)
                || !frame.Rows.All(predicate: ValidPrivateItem))
        {
            return false;
        }

        if (body.GetProperty(propertyName: "ContainerId").ValueKind == JsonValueKind.Null
                && kind is "Pile" or "Corpse")
        {
            return ValidExternalLoosePage(frame: frame, body: body, kind: kind, target: target);
        }

        return body.GetProperty(propertyName: "ItemPage").ValueKind == JsonValueKind.Null
                && body.GetProperty(propertyName: "ItemCount").ValueKind == JsonValueKind.Null
                && body.GetProperty(propertyName: "GearCount").ValueKind == JsonValueKind.Null;
    }

    private static bool ValidExternalLoosePage(FoundationRunFrame frame, JsonElement body, string kind,
            string target)
    {
        int held = body.GetProperty(propertyName: "GearCount").GetInt32();
        if (ExternalLoosePageLength(body: body) is not { } loose || held < 0
                || held > (kind is "Corpse" ? FoundationInventoryInteraction.Slots.Length : 0))
        {
            return false;
        }

        int items = frame.Rows.Count(predicate: IsLooseItem);
        return items <= loose && frame.Rows.Length - items <= held
                && (frame.Parts == 1 ? items == loose && frame.Rows.Length - items == held : !frame.Rows.IsEmpty)
                && frame.Rows.All(predicate: row => ExternalLooseRow(row: row, target: target));
    }

    private static bool ExternalLooseRow(JsonElement row, string target)
    {
        JsonElement location = row.GetProperty(propertyName: "Location");
        return Text(value: location, name: "Kind") is "ground" or "corpse"
                && StringComparer.Ordinal.Equals(x: Text(value: location, name: "TargetId"), y: target);
    }

    private static bool ValidPageEffect(JsonElement body)
    {
        JsonElement effect = body.GetProperty(propertyName: "Effect");
        if (effect.ValueKind == JsonValueKind.Null)
        {
            return true;
        }

        Shape(value: effect, names: ["Sequence", "PageNumber", "MorePages", "Mask"]);
        int page = body.GetProperty(propertyName: "Page").GetInt32();
        int first = effect.GetProperty(propertyName: "PageNumber").GetInt32();
        return Text(value: body, name: "Kind") is "Stash" && first == page
                && effect.GetProperty(propertyName: "Sequence").GetInt64() > 0
                && effect.GetProperty(propertyName: "MorePages").GetInt32() >= 0
                && ReadStashEffectMask(body: body, effect: effect) is not null;
    }

    private static bool ValidHud(JsonElement hud, bool alive)
    {
        if (!alive)
        {
            return hud.ValueKind == JsonValueKind.Null;
        }

        Shape(value: hud, names: ["Rest", "Weight", "Load", "Speed", "Cost", "Soft", "Severe", "Money", "Left",
            "Right", "LeftAttack", "RightAttack", "Ammo", "AttackAt", "AttackReady", "FoodAt", "FoodReady",
            "FlinchAt", "SprintBlock", "Food"]);
        JsonElement slots = hud.GetProperty(propertyName: "Food");
        return ValidHudScalars(hud: hud) && slots.ValueKind == JsonValueKind.Array && slots.GetArrayLength() == 4
                && ValidHudItem(item: hud.GetProperty(propertyName: "Left"))
                && ValidHudItem(item: hud.GetProperty(propertyName: "Right"))
                && slots.EnumerateArray().All(predicate: ValidHudItem);
    }

    private static bool ValidHudScalars(JsonElement hud)
    {
        JsonElement ammo = hud.GetProperty(propertyName: "Ammo");
        return Text(value: hud, name: "Rest") is "Unsafe" or "Resting" or "Recovering"
                && Text(value: hud, name: "Load") is "Normal" or "Heavy" or "Overloaded"
                && Text(value: hud, name: "SprintBlock") is "None" or "Overloaded" or "InsufficientStamina"
                && hud.GetProperty(propertyName: "Money").GetInt64() >= 0
                && (ammo.ValueKind == JsonValueKind.Null || ammo.GetInt32() is >= 0 and <= 10000)
                && hud.GetProperty(propertyName: "LeftAttack").ValueKind is JsonValueKind.True or JsonValueKind.False
                && hud.GetProperty(propertyName: "RightAttack").ValueKind is JsonValueKind.True or JsonValueKind.False
                && ValidHudNumbers(data: hud);
    }

    private static bool ValidHudNumbers(JsonElement data)
    {
        string[] required = ["Weight", "Speed", "Cost", "Soft", "Severe"];
        string[] optional = ["AttackAt", "AttackReady", "FoodAt", "FoodReady", "FlinchAt"];
        return required.All(predicate: name => NonnegativeNumber(value: data.GetProperty(propertyName: name)))
                && optional.All(predicate: name => OptionalNonnegative(value: data.GetProperty(propertyName: name)));
    }

    private static bool NonnegativeNumber(JsonElement value)
    {
        return value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(value: out decimal available)
                && available >= 0;
    }

    private static bool OptionalNonnegative(JsonElement value)
    {
        return value.ValueKind == JsonValueKind.Null || NonnegativeNumber(value: value);
    }

    private static bool ValidHudItem(JsonElement item)
    {
        if (item.ValueKind == JsonValueKind.Null)
        {
            return true;
        }

        Shape(value: item, names: ["Id", "DefinitionId", "CanonicalIcon", "Quantity", "Condition",
            "MaximumCondition"]);
        return item.GetProperty(propertyName: "Id").GetInt64() > 0
                && item.GetProperty(propertyName: "Quantity").GetInt32() is >= 1 and <= 10000
                && Text(value: item, name: "DefinitionId").Length is >= 1 and <= 128
                && Text(value: item, name: "CanonicalIcon").Length is >= 1 and <= 128
                && OptionalNonnegative(value: item.GetProperty(propertyName: "Condition"))
                && OptionalNonnegative(value: item.GetProperty(propertyName: "MaximumCondition"));
    }

    private static bool ValidOwnControls(JsonElement body)
    {
        ValidateRows(body: body, member: "Quick", fields: ["Slot", "ItemId", "Stamp"]);
        return body.GetProperty(propertyName: "Quick").EnumerateArray().All(predicate: quick =>
                quick.GetProperty(propertyName: "Slot").GetInt32() is >= 1 and <= 4
                        && quick.GetProperty(propertyName: "ItemId").GetInt64() > 0
                        && quick.GetProperty(propertyName: "Stamp").GetString() is null or { Length: 64 });
    }

    private static bool ValidNearbyTarget(JsonElement target)
    {
        Shape(value: target, names: ["Kind", "Id"]);
        return Text(value: target, name: "Kind") is "Stash" or "Npc" or "Pile" or "Corpse"
                && Text(value: target, name: "Id").Length is >= 1 and <= 128;
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

    private static bool ValidPublicPart(FoundationRunFrame frame)
    {
        if (frame.Body is not { } header || frame.Epoch != 0 || frame.View != 0 || frame.Token != 0
                || frame.Scope.Length != 0 || !NoActionFields(frame: frame) || frame.Rows.IsEmpty
                || frame.Parts is < 1 or > FoundationPublicWorld.MaximumParts || frame.Part >= frame.Parts)
        {
            return false;
        }

        Shape(value: header, names: ["Elapsed", "OriginX", "Ground", "CampLeft", "CampRight"]);
        foreach (JsonElement row in frame.Rows)
        {
            Shape(value: row, names: ["Collection", "Row"]);
            ValidatePublicRow(collection: Text(value: row, name: "Collection"),
                    row: row.GetProperty(propertyName: "Row"));
        }

        return true;
    }

    private static void ValidatePublicRow(string collection, JsonElement row)
    {
        string[] fields = collection switch
        {
            "Actors" => ["Id", "X", "Y", "Facing", "Hostile", "BindingId", "Equipment", "Movement",
                "StrikeAt", "StrikeUntil", "AimX", "AimY", "FlinchAt", "ArrowReady"],
            "Corpses" => ["Id", "SurvivorId", "X", "Y", "Facing", "Hostile", "At", "Equipment"],
            "Defenses" => ["Id", "X", "Y", "Living", "Completed", "IsWall"],
            "Loot" => ["Id", "X", "Y", "Kind"],
            "Arrows" => ["Id", "X", "Y", "VelocityX", "VelocityY"],
            _ => throw new InvalidDataException(message: "WIRE_PUBLIC_COLLECTION"),
        };
        Shape(value: row, names: fields);
        if (collection is "Actors" or "Corpses")
        {
            ValidateRows(body: row, member: "Equipment", fields: ["Slot", "DefinitionId", "Broken"]);
        }
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
