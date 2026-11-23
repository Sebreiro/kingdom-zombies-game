using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Owns real equipped crowds, four redundant player identities and retained wildlife cycles.</summary>
internal sealed class CrowdPreview
{
    internal CrowdPreview(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read)
    {
        gateway.RequireThread();
        _root = root;
        _gateway = gateway;
        _read = file => Cached(read: read, file: file);
        _fixture = Definition(path: "content/fixtures/crowds.json", id: "crowds");
        Verify(bindings: _fixture.GetProperty(propertyName: "bindings"));
        _identities = Definition(path: "content/packs/player-identities.json", id: "player-identities");
        Verify(bindings: _identities.GetProperty(propertyName: "sourceBindings"));
        _definition = Definition(path: "content/packs/corpses-wildlife.json", id: "corpses-wildlife");
        _weapons = Definition(path: "content/packs/items-01-10.json", id: "items-01-10");
        _clothing = Definition(path: "content/packs/items-11-24.json", id: "items-11-24");
        using var common = JsonDocument.Parse(utf8Json: _read(arg: "content/packs/common.json")
                ?? throw new InvalidDataException(message: "CROWD_COMMON_MISSING"),
                options: new JsonDocumentOptions { MaxDepth = 32 });
        _common = common.RootElement.Clone();
        Validate();
    }

    internal JsonElement Snapshot => JsonSerializer.SerializeToElement(value: new
    {
        actors = _subjects.Count,
        survivors = _current.Values.Count(predicate: value => Text(row: value, name: "kind") is "alive"
                && Text(row: value, name: "body") is "CH-01"),
        zombies = _current.Values.Count(predicate: value => Text(row: value, name: "kind") is "alive"
                && Text(row: value, name: "body") is not "CH-01"),
        persistent = _current.Values.Count(predicate: value => Text(row: value, name: "kind") is "corpse" or "pile"),
        selected = _selected,
        controlled = _controlled,
        night = _night,
        gameplayEvents = 0,
        performanceCertified = false,
    });

    internal void Attach()
    {
        _gateway.RequireThread();
        if (_attached || _closed || _root.HasNode(path: "Crowd"))
        {
            throw new InvalidOperationException(message: "CROWD_ALREADY_ATTACHED_OR_CLOSED");
        }

        var surface = new Node2D { Name = "Crowd", TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
        _root.AddChild(node: surface);
        surface.AddChild(node: new Polygon2D
        {
            Name = "Background", Polygon = [Vector2.Zero, new Vector2(x: 1280, y: 0),
                new Vector2(x: 1280, y: 1280), new Vector2(x: 0, y: 1280)], ZIndex = -10
        });
        var validator = new Node2D { Name = "Validator" };
        surface.AddChild(node: validator);
        CorpsePreview validation = CorpsePreview.Open(root: validator, gateway: _gateway, read: _read);
        validation.Close();
        surface.RemoveChild(node: validator);
        validator.QueueFree();
        _attached = true;
        foreach (JsonElement row in _fixture.GetProperty(propertyName: "actors").EnumerateArray())
        {
            string id = Text(row: row, name: "id");
            JsonElement at = row.GetProperty(propertyName: "position");
            var actor = new Node2D { Name = id, Position = new Vector2(x: at[0].GetSingle(), y: at[1].GetSingle()) };
            actor.SetMeta(name: "actor_id", value: id);
            surface.AddChild(node: actor);
            _subjects.Add(key: id, value: new CorpsePreview(root: actor, gateway: _gateway, read: _read,
                    definition: _definition, common: _common, weapons: _weapons, clothing: _clothing));
            _current.Add(key: id, value: row.Clone());
            ProjectActor(id: id, selected: false);
        }

        surface.AddChild(node: new Node2D { Name = "Wildlife_rabbit", Position = new Vector2(x: 850, y: 80) });
        surface.AddChild(node: new Node2D { Name = "Wildlife_deer", Position = new Vector2(x: 900, y: 80) });
        Wildlife(body: "rabbit", action: "idle", elapsedSeconds: 0, facing: 1, selected: false);
        Wildlife(body: "deer", action: "idle", elapsedSeconds: 0, facing: -1, selected: false);
        Select(id: Text(row: _fixture, name: "selected"));
        SetNight(night: false);
    }

    internal void Select(string id)
    {
        _gateway.RequireThread();
        RequireActive();
        if (!_subjects.ContainsKey(key: id))
        {
            throw new InvalidDataException(message: "CROWD_SELECTION");
        }

        if (_selected.Length > 0 && !StringComparer.Ordinal.Equals(x: _selected, y: id))
        {
            ProjectActor(id: _selected, selected: false);
        }

        _selected = id;
        ProjectActor(id: id, selected: true);
        Publish();
    }

    internal void RestoreRemaining(string id, JsonElement equipment)
    {
        _gateway.RequireThread();
        RequireActive();
        if (!_subjects.TryGetValue(key: id, value: out CorpsePreview? subject))
        {
            throw new InvalidDataException(message: "CROWD_REMAINING_ID");
        }

        JsonElement row = _current[id];
        subject.Restore(body: Text(row: row, name: "body"), kind: Text(row: row, name: "kind"),
                equipment: equipment, selected: StringComparer.Ordinal.Equals(x: id, y: _selected),
                facing: row.GetProperty(propertyName: "facing").GetInt32());
        JsonObject candidate = JsonNode.Parse(json: row.GetRawText())?.AsObject()
                ?? throw new InvalidDataException(message: "CROWD_ROW");
        candidate["equipment"] = JsonNode.Parse(json: subject.Snapshot.GetProperty(propertyName: "equipment")
                .GetRawText());
        _current[id] = JsonSerializer.SerializeToElement(value: candidate);
        Identity(id: id);
        Publish();
    }

    internal void TransferControl(string from, string to)
    {
        _gateway.RequireThread();
        RequireActive();
        if (!StringComparer.Ordinal.Equals(x: from, y: _controlled) || StringComparer.Ordinal.Equals(x: from, y: to)
                || !_current.TryGetValue(key: to, value: out JsonElement destination)
                || destination.GetProperty(propertyName: "player").ValueKind
                        is not JsonValueKind.String)
        {
            throw new InvalidDataException(message: "CROWD_CONTROL_TRANSFER");
        }

        JsonObject previous = JsonNode.Parse(json: _current[from].GetProperty(propertyName: "equipment")
                .GetRawText())?.AsObject() ?? throw new InvalidDataException(message: "CROWD_CONTROL_GEAR");
        JsonObject next = JsonNode.Parse(json: destination.GetProperty(propertyName: "equipment").GetRawText())
                ?.AsObject() ?? throw new InvalidDataException(message: "CROWD_CONTROL_GEAR");
        _ = previous.Remove(propertyName: "crown");
        next["crown"] = "K21";
        RestoreRemaining(id: from, equipment: JsonSerializer.SerializeToElement(value: previous));
        RestoreRemaining(id: to, equipment: JsonSerializer.SerializeToElement(value: next));
        _controlled = to;
        Publish();
    }

    internal void SetNight(bool night)
    {
        _gateway.RequireThread();
        RequireActive();
        _night = night;
        JsonElement setting = _fixture.GetProperty(propertyName: night ? "night" : "day");
        _root.GetNode<Node2D>(path: "Crowd").Modulate = new Color(code: Text(row: setting, name: "modulate"));
        _root.GetNode<Polygon2D>(path: "Crowd/Background").Color = new Color(code: Text(row: setting,
                name: "background"));
        Publish();
    }

    internal void Wildlife(string body, string action, double elapsedSeconds, int facing, bool selected)
    {
        _gateway.RequireThread();
        RequireActive();
        if (body is not ("rabbit" or "deer") || !double.IsFinite(d: elapsedSeconds)
                || elapsedSeconds is < 0 or > 60 || facing is not (-1 or 1)
                || !_identities.GetProperty(propertyName: "wildlife").GetProperty(propertyName: body)
                        .TryGetProperty(propertyName: action, value: out JsonElement selection))
        {
            throw new InvalidDataException(message: "CROWD_WILDLIFE_SELECTION");
        }

        string part = selection.GetString() ?? throw new InvalidDataException(message: "CROWD_WILDLIFE_PART");
        if (action is not "corpse")
        {
            JsonElement clip = _identities.GetProperty(propertyName: "clips").EnumerateArray().Single(
                    predicate: value => StringComparer.Ordinal.Equals(x: Text(row: value, name: "name"), y: part));
            double stop = clip.GetProperty(propertyName: "stop").GetDouble();
            double time = action is "death" ? Math.Min(val1: elapsedSeconds, val2: stop)
                    : elapsedSeconds % stop;
            part = Text(row: clip.GetProperty(propertyName: "frames").EnumerateArray().Last(predicate: frame =>
                    frame.GetProperty(propertyName: "time").GetDouble() <= time), name: "part");
            if (action is "death" && elapsedSeconds >= stop)
            {
                part = Text(row: _identities.GetProperty(propertyName: "wildlife").GetProperty(propertyName: body),
                        name: "corpse");
            }
        }

        Node2D owner = _root.GetNode<Node2D>(path: "Crowd/Wildlife_" + body);
        RetireWildlife(owner: owner);
        JsonElement definition = Part(id: part);
        JsonElement offset = definition.GetProperty(propertyName: "registration").GetProperty(
                propertyName: "offsetTopLeftExact");
        var at = new Vector2(x: offset[0].GetSingle() * facing, y: offset[1].GetSingle());
        if (action is "hit")
        {
            at.X -= facing;
        }

        var sprite = new Sprite2D
        {
            Name = "Animal", Centered = false, Texture = Texture(id: part), Position = at,
            Scale = new Vector2(x: facing, y: 1), TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };
        sprite.SetMeta(name: "source", value: part);
        owner.AddChild(node: sprite);
        owner.SetMeta(name: "wildlife", value: JsonSerializer.Serialize(value: new
        {
            body, action, elapsedSeconds, facing, part, selected, gameplayEvents = 0, stunSeconds = 0,
        }));
        if (selected)
        {
            WildlifeOutline(owner: owner, sprite: sprite);
        }
    }

    internal void Close()
    {
        _gateway.RequireThread();
        if (!_closed)
        {
            _closed = true;
            _attached = false;
            foreach (CorpsePreview subject in _subjects.Values)
            {
                subject.Close();
            }

            _subjects.Clear();
            _current.Clear();
            if (_root.HasNode(path: "Crowd"))
            {
                Node surface = _root.GetNode(path: "Crowd");
                foreach (string body in new[] { "rabbit", "deer" })
                {
                    if (surface.HasNode(path: "Wildlife_" + body))
                    {
                        RetireWildlife(owner: surface.GetNode<Node2D>(path: "Wildlife_" + body));
                    }
                }

                _root.RemoveChild(node: surface);
                surface.QueueFree();
            }

            foreach (Texture2D texture in _textures.Values)
            {
                texture.Dispose();
            }

            _textures.Clear();
            _root.RemoveMeta(name: "crowd");
        }
    }

    private readonly Node2D _root;
    private readonly MainThreadGateway _gateway;
    private readonly Func<string, byte[]?> _read;
    private readonly Dictionary<string, byte[]> _bytes = new(comparer: StringComparer.Ordinal);
    private readonly JsonElement _fixture;
    private readonly JsonElement _identities;
    private readonly JsonElement _definition;
    private readonly JsonElement _common;
    private readonly JsonElement _weapons;
    private readonly JsonElement _clothing;
    private readonly Dictionary<string, CorpsePreview> _subjects = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, JsonElement> _current = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, AtlasTexture> _textures = new(comparer: StringComparer.Ordinal);
    private bool _attached;
    private bool _closed;
    private bool _night;
    private string _selected = "";
    private string _controlled = "survivor-000";

    private static string Text(JsonElement row, string name)
    {
        return row.GetProperty(propertyName: name).GetString()
                ?? throw new InvalidDataException(message: "CROWD_STRING " + name);
    }

    private byte[] Cached(Func<string, byte[]?> read, string file)
    {
        if (!_bytes.TryGetValue(key: file, value: out byte[]? data))
        {
            byte[] source = read(arg: file) ?? throw new InvalidDataException(message: "CROWD_MISSING " + file);
            if (source.Length is 0 or > 16777216 || _bytes.Count >= 32)
            {
                throw new InvalidDataException(message: "CROWD_SOURCE_BOUND");
            }

            data = (byte[])source.Clone();
            _bytes.Add(key: file, value: data);
        }

        return data;
    }

    private JsonElement Definition(string path, string id)
    {
        return CharacterPreview.ReadDefinition(bytes: _read(arg: path)
                ?? throw new InvalidDataException(message: "CROWD_MISSING " + path), expectedId: id);
    }

    private void Verify(JsonElement bindings)
    {
        if (bindings.GetArrayLength() is 0 or > 32)
        {
            throw new InvalidDataException(message: "CROWD_BINDING_COUNT");
        }

        var names = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement binding in bindings.EnumerateArray())
        {
            string file = Text(row: binding, name: "file");
            if (!names.Add(item: file) || file.Contains(value: "..", comparisonType: StringComparison.Ordinal)
                    || file.Contains(value: ':') || file.Contains(value: '\\')
                    || !(file.StartsWith(value: "content/", comparisonType: StringComparison.Ordinal)
                            || file.StartsWith(value: "assets/", comparisonType: StringComparison.Ordinal)))
            {
                throw new InvalidDataException(message: "CROWD_BINDING_PATH");
            }

            byte[] data = _read(arg: file) ?? throw new InvalidDataException(message: "CROWD_BINDING_MISSING");
            if (!StringComparer.Ordinal.Equals(x: Convert.ToHexStringLower(inArray: SHA256.HashData(source: data)),
                    y: Text(row: binding, name: "sha256")))
            {
                throw new InvalidDataException(message: "CROWD_BINDING_HASH " + file);
            }
        }
    }

    private void Validate()
    {
        JsonElement rows = _fixture.GetProperty(propertyName: "actors");
        if (rows.GetArrayLength() != 600 || _identities.GetProperty(propertyName: "players").GetArrayLength() != 4)
        {
            throw new InvalidDataException(message: "CROWD_ACTUAL_COUNTS");
        }

        JsonElement players = Definition(path: "content/packs/ui-identity.json", id: "ui-identity")
                .GetProperty(propertyName: "players");
        if (!StringComparer.Ordinal.Equals(x: JsonSerializer.Serialize(value: players),
                y: JsonSerializer.Serialize(value: _identities.GetProperty(propertyName: "players"))))
        {
            throw new InvalidDataException(message: "CROWD_EXACT_IDENTITIES");
        }

        var state = new AppearanceState(definition: System.Text.Encoding.UTF8.GetBytes(s: _definition.GetRawText()));
        var ids = new HashSet<string>(comparer: StringComparer.Ordinal);
        int index = 0;
        foreach (JsonElement row in rows.EnumerateArray())
        {
            string prefix = index < 100 ? "survivor" : index < 400 ? "zombie" : index < 500 ? "corpse" : "pile";
            int ordinal = index < 100 ? index : index < 400 ? index - 100 : index < 500 ? index - 400 : index - 500;
            string id = prefix + "-" + ordinal.ToString(format: "D3", provider: CultureInfo.InvariantCulture);
            string kind = index < 400 ? "alive" : index < 500 ? "corpse" : "pile";
            string body = Text(row: row, name: "body");
            JsonElement position = row.GetProperty(propertyName: "position");
            if (!StringComparer.Ordinal.Equals(x: Text(row: row, name: "id"), y: id) || !ids.Add(item: id)
                    || !StringComparer.Ordinal.Equals(x: Text(row: row, name: "kind"), y: kind)
                    || index < 100 && body is not "CH-01" || index is >= 100 and < 400 && body is "CH-01"
                    || position.GetArrayLength() != 2 || position.EnumerateArray().Any(predicate: value =>
                            value.GetDouble() is < 0 or > 2048) || row.GetProperty(propertyName: "equipment")
                                    .EnumerateObject().Count() < 3)
            {
                throw new InvalidDataException(message: "CROWD_ACTUAL_ROW " + id);
            }

            state.Restore(body: body, kind: kind, equipment: row.GetProperty(propertyName: "equipment"),
                    selected: false);
            if (row.GetProperty(propertyName: "facing").GetInt32() is not (-1 or 1))
            {
                throw new InvalidDataException(message: "CROWD_FACING");
            }

            index++;
        }

        ValidateIdentityDefinitions();
    }

    private void ValidateIdentityDefinitions()
    {
        JsonElement atlas = Field(row: _identities, name: "atlas", kind: JsonValueKind.Object);
        const string AtlasPath = "assets/characters/identity-wildlife.png";
        if (!StringComparer.Ordinal.Equals(x: IdentityText(row: atlas, name: "path"), y: AtlasPath))
        {
            throw new InvalidDataException(message: "CROWD_ATLAS_PATH");
        }

        byte[] pixels = _read(arg: AtlasPath)
                ?? throw new InvalidDataException(message: "CROWD_ATLAS_MISSING");
        if (!StringComparer.Ordinal.Equals(x: Convert.ToHexStringLower(inArray: SHA256.HashData(source: pixels)),
                y: IdentityText(row: atlas, name: "sha256")))
        {
            throw new InvalidDataException(message: "CROWD_ATLAS_HASH");
        }

        if (pixels.Length < 33 || !pixels.AsSpan(start: 0, length: 16).SequenceEqual(other:
                new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 }))
        {
            throw new InvalidDataException(message: "CROWD_ATLAS_PNG");
        }

        uint width = BinaryPrimitives.ReadUInt32BigEndian(source: pixels.AsSpan(start: 16, length: 4));
        uint height = BinaryPrimitives.ReadUInt32BigEndian(source: pixels.AsSpan(start: 20, length: 4));
        if (width is 0 or > 16384 || height is 0 or > 16384
                || !Field(row: atlas, name: "width", kind: JsonValueKind.Number).TryGetUInt32(
                        value: out uint declaredWidth)
                || !Field(row: atlas, name: "height", kind: JsonValueKind.Number).TryGetUInt32(
                        value: out uint declaredHeight)
                || width != declaredWidth || height != declaredHeight)
        {
            throw new InvalidDataException(message: "CROWD_ATLAS_DIMENSIONS");
        }

        HashSet<string> parts = ValidateParts(width: width, height: height);
        HashSet<string> clips = ValidateClips(parts: parts);
        ValidateWildlife(parts: parts, clips: clips);
        ValidatePolygons(bound: Math.Max(val1: width, val2: height));
    }

    private static JsonElement Field(JsonElement row, string name, JsonValueKind kind)
    {
        if (row.ValueKind is not JsonValueKind.Object || !row.TryGetProperty(propertyName: name,
                value: out JsonElement value) || value.ValueKind != kind)
        {
            throw new InvalidDataException(message: "CROWD_IDENTITY_FIELD " + name);
        }

        return value;
    }

    private static string IdentityText(JsonElement row, string name)
    {
        string value = Field(row: row, name: name, kind: JsonValueKind.String).GetString()
                ?? throw new InvalidDataException(message: "CROWD_IDENTITY_TEXT " + name);
        if (value.Length is 0 or > 160 || value.Any(predicate: char.IsControl))
        {
            throw new InvalidDataException(message: "CROWD_IDENTITY_TEXT " + name);
        }

        return value;
    }

    private static JsonElement Rows(JsonElement row, string name, int minimum)
    {
        JsonElement rows = Field(row: row, name: name, kind: JsonValueKind.Array);
        if (rows.GetArrayLength() < minimum || rows.GetArrayLength() > 4096)
        {
            throw new InvalidDataException(message: "CROWD_IDENTITY_COUNT " + name);
        }

        return rows;
    }

    private static double Number(JsonElement value, double minimum, double maximum, string diagnostic)
    {
        if (value.ValueKind is not JsonValueKind.Number || !value.TryGetDouble(value: out double number)
                || !double.IsFinite(d: number) || number < minimum || number > maximum)
        {
            throw new InvalidDataException(message: diagnostic);
        }

        return number;
    }

    // Fractional registration retains the usable span of the authenticated atlas.
    private HashSet<string> ValidateParts(uint width, uint height)
    {
        var parts = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement part in Rows(row: _identities, name: "parts", minimum: 1).EnumerateArray())
        {
            if (!parts.Add(item: IdentityText(row: part, name: "id")))
            {
                throw new InvalidDataException(message: "CROWD_PART_DUPLICATE");
            }

            JsonElement rect = Field(row: part, name: "atlasRect", kind: JsonValueKind.Array);
            if (rect.GetArrayLength() != 4 || rect.EnumerateArray().Any(predicate: coordinate =>
                    coordinate.ValueKind is not JsonValueKind.Number) || !rect[0].TryGetInt32(value: out int x)
                    || !rect[1].TryGetInt32(value: out int y) || !rect[2].TryGetInt32(value: out int w)
                    || !rect[3].TryGetInt32(value: out int h) || x < 0 || y < 0 || w <= 0 || h <= 0
                    || (long)x + w > width || (long)y + h > height)
            {
                throw new InvalidDataException(message: "CROWD_PART_RECT");
            }

            JsonElement registration = Field(row: part, name: "registration", kind: JsonValueKind.Object);
            JsonElement offset = Field(row: registration, name: "offsetTopLeftExact", kind: JsonValueKind.Array);
            if (offset.GetArrayLength() != 2)
            {
                throw new InvalidDataException(message: "CROWD_PART_OFFSET");
            }

            foreach (JsonElement coordinate in offset.EnumerateArray())
            {
                _ = Number(value: coordinate, minimum: -Math.Max(val1: width, val2: height),
                        maximum: Math.Max(val1: width, val2: height), diagnostic: "CROWD_PART_OFFSET");
            }
        }

        return parts;
    }

    // The existing preview clock accepts 0..60 seconds, including exact death endpoints.
    private HashSet<string> ValidateClips(HashSet<string> parts)
    {
        var clips = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement clip in Rows(row: _identities, name: "clips", minimum: 1).EnumerateArray())
        {
            if (!clips.Add(item: IdentityText(row: clip, name: "name")))
            {
                throw new InvalidDataException(message: "CROWD_CLIP_DUPLICATE");
            }

            double stop = Number(value: Field(row: clip, name: "stop", kind: JsonValueKind.Number),
                    minimum: double.Epsilon, maximum: 60, diagnostic: "CROWD_CLIP_STOP");
            if (!clip.TryGetProperty(propertyName: "loop", value: out JsonElement loop)
                    || loop.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
            {
                throw new InvalidDataException(message: "CROWD_CLIP_LOOP");
            }

            double previous = -1;
            foreach (JsonElement frame in Rows(row: clip, name: "frames", minimum: 1).EnumerateArray())
            {
                double time = Number(value: Field(row: frame, name: "time", kind: JsonValueKind.Number),
                        minimum: 0, maximum: stop, diagnostic: "CROWD_FRAME_TIME");
                if (previous < 0 && time != 0 || previous >= 0 && time <= previous)
                {
                    throw new InvalidDataException(message: "CROWD_FRAME_ORDER");
                }

                if (!parts.Contains(item: IdentityText(row: frame, name: "part")))
                {
                    throw new InvalidDataException(message: "CROWD_FRAME_PART");
                }

                previous = time;
            }
        }

        return clips;
    }

    private void ValidateWildlife(HashSet<string> parts, HashSet<string> clips)
    {
        JsonElement wildlife = Field(row: _identities, name: "wildlife", kind: JsonValueKind.Object);
        foreach (string body in new[] { "rabbit", "deer" })
        {
            JsonElement actions = Field(row: wildlife, name: body, kind: JsonValueKind.Object);
            string[] required = body is "rabbit" ? ["idle", "move", "flee", "hit", "death", "corpse"]
                    : ["idle", "look", "eat", "move", "flee", "hit", "death", "corpse"];
            foreach (string action in required)
            {
                _ = IdentityText(row: actions, name: action);
            }

            foreach (JsonProperty action in actions.EnumerateObject())
            {
                string reference = IdentityText(row: actions, name: action.Name);
                if (!(action.Name is "corpse" ? parts : clips).Contains(item: reference))
                {
                    throw new InvalidDataException(message: "CROWD_WILDLIFE_REFERENCE " + body + "/" + action.Name);
                }
            }
        }
    }

    private void ValidatePolygons(double bound)
    {
        JsonElement polygons = Field(row: _identities, name: "shapePolygons", kind: JsonValueKind.Object);
        foreach (string shape in new[] { "square", "triangle", "diamond", "cross" })
        {
            JsonElement polygon = Rows(row: polygons, name: shape, minimum: 3);
            var points = new List<double[]>();
            foreach (JsonElement point in polygon.EnumerateArray())
            {
                if (point.ValueKind is not JsonValueKind.Array || point.GetArrayLength() != 2)
                {
                    throw new InvalidDataException(message: "CROWD_POLYGON_POINT " + shape);
                }

                double x = Number(value: point[0], minimum: -bound, maximum: bound,
                        diagnostic: "CROWD_POLYGON_POINT " + shape);
                double y = Number(value: point[1], minimum: -bound, maximum: bound,
                        diagnostic: "CROWD_POLYGON_POINT " + shape);
                points.Add(item: [x, y]);
            }

            ValidateGeometry(points: points, shape: shape);
        }
    }

    private static void ValidateGeometry(List<double[]> points, string shape)
    {
        double area = 0;
        for (int i = 0; i < points.Count; i++)
        {
            double[] a = points[i];
            double[] b = points[(i + 1) % points.Count];
            area += a[0] * b[1] - b[0] * a[1];
            for (int j = i + 1; j < points.Count; j++)
            {
                double[] c = points[j];
                double[] d = points[(j + 1) % points.Count];
                if (a.SequenceEqual(second: c) || j != i + 1 && (i != 0 || j != points.Count - 1)
                        && Intersects(a: a, b: b, c: c, d: d))
                {
                    throw new InvalidDataException(message: "CROWD_POLYGON_SIMPLE " + shape);
                }
            }
        }

        if (area == 0)
        {
            throw new InvalidDataException(message: "CROWD_POLYGON_AREA " + shape);
        }
    }

    private static double Cross(double[] a, double[] b, double[] c)
    {
        return (b[0] - a[0]) * (c[1] - a[1]) - (b[1] - a[1]) * (c[0] - a[0]);
    }

    private static bool OnSegment(double[] a, double[] b, double[] point)
    {
        return point[0] >= Math.Min(val1: a[0], val2: b[0]) && point[0] <= Math.Max(val1: a[0], val2: b[0])
                && point[1] >= Math.Min(val1: a[1], val2: b[1]) && point[1] <= Math.Max(val1: a[1], val2: b[1]);
    }

    private static bool Intersects(double[] a, double[] b, double[] c, double[] d)
    {
        double abC = Cross(a: a, b: b, c: c);
        double abD = Cross(a: a, b: b, c: d);
        double cdA = Cross(a: c, b: d, c: a);
        double cdB = Cross(a: c, b: d, c: b);
        return abC == 0 && OnSegment(a: a, b: b, point: c) || abD == 0 && OnSegment(a: a, b: b, point: d)
                || cdA == 0 && OnSegment(a: c, b: d, point: a) || cdB == 0 && OnSegment(a: c, b: d, point: b)
                || (abC < 0 && abD > 0 || abC > 0 && abD < 0) && (cdA < 0 && cdB > 0 || cdA > 0 && cdB < 0);
    }

    private void RequireActive()
    {
        if (!_attached || _closed)
        {
            throw new InvalidOperationException(message: "CROWD_CLOSED_OR_DETACHED");
        }
    }

    private void Publish()
    {
        _gateway.RequireThread();
        _root.SetMeta(name: "crowd", value: Snapshot.GetRawText());
    }

    private void ProjectActor(string id, bool selected)
    {
        _gateway.RequireThread();
        JsonElement row = _current[id];
        _subjects[id].Restore(body: Text(row: row, name: "body"), kind: Text(row: row, name: "kind"),
                equipment: row.GetProperty(propertyName: "equipment"), selected: selected,
                facing: row.GetProperty(propertyName: "facing").GetInt32());
        Identity(id: id);
    }

    private void Identity(string id)
    {
        _gateway.RequireThread();
        JsonElement player = _current[id].GetProperty(propertyName: "player");
        if (player.ValueKind is not JsonValueKind.String)
        {
            return;
        }

        JsonElement identity = _identities.GetProperty(propertyName: "players").EnumerateArray().Single(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(row: value, name: "number"),
                        y: player.GetString()));
        Node2D visual = _root.GetNode<Node2D>(path: "Crowd/" + id + "/Visual");
        if (visual.HasNode(path: "Identity"))
        {
            Node previous = visual.GetNode(path: "Identity");
            visual.RemoveChild(node: previous);
            previous.QueueFree();
        }

        var owner = new Node2D { Name = "Identity", Position = new Vector2(x: 24, y: -12) };
        visual.AddChild(node: owner);
        var color = new Color(code: Text(row: identity, name: "color"));
        JsonElement polygon = _identities.GetProperty(propertyName: "shapePolygons").GetProperty(
                propertyName: Text(row: identity, name: "shape"));
        var points = new Vector2[polygon.GetArrayLength()];
        for (int index = 0; index < points.Length; index++)
        {
            points[index] = new Vector2(x: polygon[index][0].GetSingle(), y: polygon[index][1].GetSingle());
        }
        foreach (string form in new[] { "World", "Teammate", "Minimap" })
        {
            var marker = new Node2D { Name = form, Position = new Vector2(x: 0, y: form is "World" ? 0
                    : form is "Teammate" ? -18 : -36) };
            owner.AddChild(node: marker);
            marker.AddChild(node: new Polygon2D { Name = "Shape", Polygon = points, Color = color });
            var label = new Label { Name = "Name", Position = new Vector2(x: 6, y: -9),
                Text = Text(row: identity, name: "number") + " · " + Text(row: identity, name: "name") };
            label.AddThemeFontSizeOverride(name: "font_size", fontSize: 12);
            marker.AddChild(node: label);
        }

        owner.SetMeta(name: "identity", value: identity.GetRawText());
    }

    private JsonElement Part(string id)
    {
        return _identities.GetProperty(propertyName: "parts").EnumerateArray().Single(predicate: value =>
                StringComparer.Ordinal.Equals(x: Text(row: value, name: "id"), y: id));
    }

    private AtlasTexture Texture(string id)
    {
        _gateway.RequireThread();
        if (!_textures.TryGetValue(key: id, value: out AtlasTexture? texture))
        {
            string path = Text(row: _identities.GetProperty(propertyName: "atlas"), name: "path");
            Texture2D atlas = ResourceLoader.Load<Texture2D>(path: "res://" + path)
                    ?? throw new InvalidDataException(message: "CROWD_ATLAS_NATIVE");
            JsonElement rect = Part(id: id).GetProperty(propertyName: "atlasRect");
            texture = new AtlasTexture { Atlas = atlas, Region = new Rect2(x: rect[0].GetSingle(),
                    y: rect[1].GetSingle(), width: rect[2].GetSingle(), height: rect[3].GetSingle()),
                FilterClip = true };
            _textures.Add(key: id, value: texture);
        }

        return texture;
    }

    private void RetireWildlife(Node2D owner)
    {
        _gateway.RequireThread();
        foreach (Node child in owner.GetChildren())
        {
            owner.RemoveChild(node: child);
            if (child is Sprite2D sprite && StringComparer.Ordinal.Equals(x: child.Name.ToString(),
                    y: "SelectedOutline"))
            {
                sprite.Texture.Dispose();
            }

            child.QueueFree();
        }
    }

    private void WildlifeOutline(Node2D owner, Sprite2D sprite)
    {
        _gateway.RequireThread();
        using Image source = sprite.Texture.GetImage();
        using Image output = Image.CreateEmpty(width: source.GetWidth() + 2, height: source.GetHeight() + 2,
                useMipmaps: false, format: Image.Format.Rgba8);
        for (int y = 0; y < source.GetHeight(); y++)
        {
            for (int x = 0; x < source.GetWidth(); x++)
            {
                if (source.GetPixel(x: x, y: y).A > 0)
                {
                    foreach (Vector2I offset in new[] { Vector2I.Left, Vector2I.Right, Vector2I.Up, Vector2I.Down })
                    {
                        output.SetPixel(x: x + 1 + offset.X, y: y + 1 + offset.Y, color: new Color(code: "f2e4c9"));
                    }
                }
            }
        }

        for (int y = 0; y < source.GetHeight(); y++)
        {
            for (int x = 0; x < source.GetWidth(); x++)
            {
                if (source.GetPixel(x: x, y: y).A > 0)
                {
                    output.SetPixel(x: x + 1, y: y + 1, color: Colors.Transparent);
                }
            }
        }

        owner.AddChild(node: new Sprite2D
        {
            Name = "SelectedOutline", Centered = false, Texture = ImageTexture.CreateFromImage(image: output),
            Position = sprite.Position - new Vector2(x: sprite.Scale.X, y: 1), Scale = sprite.Scale, ZIndex = 1
        });
        owner.AddChild(node: new Polygon2D { Name = "SelectedMarker", Polygon = [new Vector2(x: -2, y: -4),
            new Vector2(x: 2, y: -4), new Vector2(x: 0, y: -1)], Color = new Color(code: "f2e4c9") });
    }
}
