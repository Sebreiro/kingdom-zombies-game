using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Projects authenticated authored world resources and explicitly requested cosmetic effects.</summary>
internal sealed class WorldPreview
{
    internal WorldPreview(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read,
            JsonElement definition)
    {
        _root = root;
        _gateway = gateway;
        _gateway.RequireThread();
        _definition = definition;
        ValidateBindings(read: read);
        _world = Read(read: read, file: "content/packs/world.json", id: "world");
        _map = Read(read: read, file: "content/world/map.json", id: "world-map");
        _effects = Read(read: read, file: "content/packs/effects-portals.json", id: "effects-portals");
        _common = Read(read: read, file: "content/packs/common.json", id: "common");
        ValidateMedia(read: read, pack: _world, expected: "assets/world/environment.png", size: 2048, height: 1024);
        ValidateMedia(read: read, pack: _effects, expected: "assets/effects/effects.png", size: 2048, height: 2048);
        ValidateWorld();
        ValidateEffects();
        ValidatePortals();
        ValidateSourceParts();
    }

    internal long Epoch => _epoch;

    internal double Time => _time;

    internal string Sample => _sample;

    internal int EventCount => _events;

    internal int TransientCount => _transients.Count;

    internal static WorldPreview Open(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read)
    {
        gateway.RequireThread();
        return new WorldPreview(root: root, gateway: gateway, read: read,
                definition: Read(read: read, file: "content/world/portal-assemblies.json", id: "portal-assemblies"));
    }

    internal void Attach()
    {
        _gateway.RequireThread();
        Require(valid: !_closed && !_attached, code: "WORLD_ATTACH_STATE");
        _worldAtlas = ResourceLoader.Load<Texture2D>(path: "res://assets/world/environment.png")
                ?? throw new InvalidDataException(message: "WORLD_ATLAS_MISSING");
        _effectAtlas = ResourceLoader.Load<Texture2D>(path: "res://assets/effects/effects.png")
                ?? throw new InvalidDataException(message: "EFFECT_ATLAS_MISSING");
        _attached = true;
        Restore(sample: "portal.closed");
    }

    internal void Restore(string sample)
    {
        _gateway.RequireThread();
        RequireActive();
        Require(valid: _definition.GetProperty(propertyName: "samples").EnumerateArray().Any(
                predicate: row => Same(left: Text(row: row, name: "id"), right: sample)), code: "WORLD_SAMPLE");
        Cancel();
        _sample = sample;
        Project();
    }

    internal bool Trigger(string effect, string source, float originX, float originY, float targetX,
            float targetY, long epoch)
    {
        _gateway.RequireThread();
        RequireActive();
        if (epoch != _epoch)
        {
            return false;
        }

        Effect(id: effect);
        Require(valid: ValidSource(source: source) && float.IsFinite(f: originX) && float.IsFinite(f: originY)
                && float.IsFinite(f: targetX) && float.IsFinite(f: targetY)
                && Math.Abs(value: originX) <= 2560 && Math.Abs(value: targetX) <= 2560
                && Math.Abs(value: originY) <= 1024 && Math.Abs(value: targetY) <= 1024,
                code: "WORLD_EFFECT_ORIGIN");
        if (_transients.Count >= 24 || _transients.Count(predicate: item => Same(left: item.Id, right: effect)) >= 4)
        {
            return false;
        }

        _transients.Add(item: new EffectState(id: effect, source: source, started: _time,
                originX: originX, originY: originY, targetX: targetX, targetY: targetY, contacted: false));
        _events++;
        Project();
        return true;
    }

    internal bool Contact(float x, float y, string material, long epoch)
    {
        _gateway.RequireThread();
        RequireActive();
        if (epoch != _epoch)
        {
            return false;
        }

        Require(valid: float.IsFinite(f: x) && float.IsFinite(f: y) && Math.Abs(value: x) <= 2560
                && Math.Abs(value: y) <= 1024 && _definition.GetProperty(propertyName: "projectileContacts")
                        .EnumerateArray().Any(predicate: item => Same(left: item.GetString(), right: material)),
                code: "WORLD_CONTACT");
        int index = _transients.FindLastIndex(match: item => !item.Contacted
                && Same(left: Text(row: Effect(id: item.Id), name: "kind"), right: "projectile"));
        if (index < 0)
        {
            return false;
        }

        EffectState shot = _transients[index];
        _transients.RemoveAt(index: index);
        string effect = Same(left: shot.Id, right: "spit.projectile") ? "spit.splash" : "arrow.contact." + material;
        _transients.Add(item: new EffectState(id: effect, source: shot.Source, started: _time,
                originX: x, originY: y, targetX: x, targetY: y, contacted: false));
        _events++;
        Project();
        return true;
    }

    internal void Advance(double elapsedSeconds)
    {
        _gateway.RequireThread();
        RequireActive();
        Require(valid: double.IsFinite(d: elapsedSeconds) && elapsedSeconds is >= 0 and <= 60,
                code: "WORLD_ELAPSED");
        _time += elapsedSeconds;
        _transients.RemoveAll(match: item => _time - item.Started >= Number(row: Effect(id: item.Id),
                name: "duration"));
        Project();
    }

    internal void Reset()
    {
        _gateway.RequireThread();
        RequireActive();
        Cancel();
        Project();
    }

    internal void Close()
    {
        _gateway.RequireThread();
        if (!_closed)
        {
            _closed = true;
            Cancel();
            Retire();
        }
    }

    private const string PortalShader = "shader_type canvas_item;\nuniform float preview_time = 0.0;\n"
            + "uniform vec4 tint : source_color = vec4(0.27,0.17,0.35,1.0);\n"
            + "void fragment(){ vec2 p=(UV-vec2(0.5))*vec2(2.0,1.4); float d=length(p); "
            + "float bands=step(0.52,fract(d*5.0-preview_time*0.35)); float a=step(d,0.75)*step(0.18,d); "
            + "COLOR=vec4(tint.rgb*(0.65+0.35*bands),a*tint.a); }\n";

    private readonly Node2D _root;
    private readonly MainThreadGateway _gateway;
    private readonly JsonElement _definition;
    private readonly JsonElement _world;
    private readonly JsonElement _map;
    private readonly JsonElement _effects;
    private readonly JsonElement _common;
    private readonly List<EffectState> _transients = [];
    private readonly List<Resource> _resources = [];
    private readonly List<Node2D> _owners = [];
    private Texture2D? _worldAtlas;
    private Texture2D? _effectAtlas;
    private bool _closed;
    private bool _attached;
    private double _time;
    private long _epoch;
    private int _events;
    private string _sample = "portal.closed";
    private static long _nextEpoch;

    private static readonly string[] _bindingFiles = ["content/packs/common.json", "content/packs/world.json",
        "content/world/map.json", "content/packs/effects-portals.json", "assets/world/environment.png",
        "assets/effects/effects.png"];

    private static JsonElement Read(Func<string, byte[]?> read, string file, string id)
    {
        byte[] source = read(arg: file) ?? throw new InvalidDataException(message: "WORLD_SOURCE_MISSING " + file);
        Require(valid: source.Length is > 0 and <= 4194304, code: "WORLD_DOCUMENT_SIZE");
        using var parsed = JsonDocument.Parse(utf8Json: source, options: new JsonDocumentOptions { MaxDepth = 24 });
        int count = 0;
        ValidateValue(row: parsed.RootElement, count: ref count);
        JsonElement root = parsed.RootElement;
        Require(valid: Same(left: Text(row: root, name: "id"), right: id)
                && Number(row: root, name: "schemaVersion") == 1
                && Number(row: root, name: "version") == 1, code: "WORLD_DOCUMENT_ID");
        return root.Clone();
    }

    private static void ValidateValue(JsonElement row, ref int count)
    {
        Require(valid: ++count <= 100000, code: "WORLD_DOCUMENT_COUNT");
        if (row.ValueKind == JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in row.EnumerateObject())
            {
                Require(valid: keys.Add(item: property.Name), code: "WORLD_DUPLICATE_PROPERTY");
                ValidateValue(row: property.Value, count: ref count);
            }
        }
        else if (row.ValueKind == JsonValueKind.Array)
        {
            Require(valid: row.GetArrayLength() <= 8192, code: "WORLD_ARRAY_SIZE");
            foreach (JsonElement item in row.EnumerateArray())
            {
                ValidateValue(row: item, count: ref count);
            }
        }
        else if (row.ValueKind == JsonValueKind.Number)
        {
            Require(valid: row.TryGetDouble(value: out double value) && double.IsFinite(d: value),
                    code: "WORLD_NUMBER");
        }
        else if (row.ValueKind == JsonValueKind.String)
        {
            string value = row.GetString() ?? string.Empty;
            Require(valid: value.Length <= 65536 && value.IsNormalized(), code: "WORLD_TEXT");
        }
    }

    private void ValidateBindings(Func<string, byte[]?> read)
    {
        JsonElement rows = _definition.GetProperty(propertyName: "sourceBindings");
        string[] actual = rows.EnumerateArray().Select(selector: row => Text(row: row, name: "file")).ToArray();
        Require(valid: rows.GetArrayLength() == 6 && actual.Order(comparer: StringComparer.Ordinal).SequenceEqual(
                second: _bindingFiles.Order(comparer: StringComparer.Ordinal), comparer: StringComparer.Ordinal),
                code: "WORLD_BINDING_SET");
        foreach (JsonElement row in rows.EnumerateArray())
        {
            string file = Text(row: row, name: "file");
            byte[] source = read(arg: file) ?? throw new InvalidDataException(message: "WORLD_BINDING_MISSING " + file);
            string actualHash = Convert.ToHexStringLower(inArray: SHA256.HashData(source: source));
            Require(valid: source.Length is > 0 and <= 16777216
                    && Same(left: actualHash, right: Text(row: row, name: "sha256")),
                    code: "WORLD_BINDING_HASH " + file);
        }
    }

    private static void ValidateMedia(Func<string, byte[]?> read, JsonElement pack, string expected, int size,
            int height)
    {
        JsonElement atlas = pack.GetProperty(propertyName: "atlas");
        Require(valid: Same(left: Text(row: atlas, name: "path"), right: "res://" + expected),
                code: "WORLD_MEDIA_PATH");
        byte[] source = read(arg: expected) ?? throw new InvalidDataException(message: "WORLD_MEDIA_MISSING");
        Require(valid: source.Length is > 32 and <= 16777216 && source.AsSpan(start: 0, length: 8).SequenceEqual(
                other: new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })
                && BinaryPrimitives.ReadInt32BigEndian(source: source.AsSpan(start: 16, length: 4)) == size
                && BinaryPrimitives.ReadInt32BigEndian(source: source.AsSpan(start: 20, length: 4)) == height
                && Same(left: Convert.ToHexStringLower(inArray: SHA256.HashData(source: source)),
                        right: Text(row: atlas, name: "sha256")), code: "WORLD_MEDIA_HASH_OR_SIZE");
        Require(valid: atlas.GetProperty(propertyName: "size")[0].GetInt32() == size
                && atlas.GetProperty(propertyName: "size")[1].GetInt32() == height, code: "WORLD_ATLAS_SIZE");
        JsonElement rows = pack.GetProperty(propertyName: "parts");
        Unique(rows: rows, key: "key", maximum: 256);
        foreach (JsonElement row in rows.EnumerateArray())
        {
            JsonElement r = row.GetProperty(propertyName: "atlasRect");
            Require(valid: r.GetArrayLength() == 4, code: "WORLD_PART_RECT");
            int x = r[0].GetInt32(), y = r[1].GetInt32(), w = r[2].GetInt32(), h = r[3].GetInt32();
            Require(valid: x >= 0 && y >= 0 && w is > 0 and <= 896 && h is > 0 and <= 256
                    && x <= size - w && y <= height - h
                    && row.GetProperty(propertyName: "size")[0].GetInt32() == w
                    && row.GetProperty(propertyName: "size")[1].GetInt32() == h, code: "WORLD_PART_BOUNDS");
        }
    }

    private void ValidateWorld()
    {
        Require(valid: Number(row: _map, name: "pixelsPerUnit") == 8
                && _map.GetProperty(propertyName: "edges")[0].GetInt32() == -320
                && _map.GetProperty(propertyName: "edges")[1].GetInt32() == 320, code: "WORLD_BASELINE");
        string[] groups = ["portals", "walls", "towers", "clearings", "wildlifeZones"];
        double[][] positions = [[100, 190, 280], [25, 75, 155, 245], [21, 71, 151, 241], [50, 125, 215],
            [45, 130, 230]];
        for (int group = 0; group < groups.Length; group++)
        {
            JsonElement rows = _map.GetProperty(propertyName: groups[group]);
            Unique(rows: rows, key: "id", maximum: 16);
            Require(valid: rows.GetArrayLength() == positions[group].Length * 2, code: "WORLD_SITE_COUNT");
            for (int index = 0; index < rows.GetArrayLength(); index++)
            {
                Require(valid: Number(row: rows[index], name: Same(left: groups[group], right: "wildlifeZones")
                        ? "center" : "x")
                        == positions[group][index % positions[group].Length]
                                * (index < positions[group].Length ? -1 : 1), code: "WORLD_SITE_POSITION");
            }
        }

        JsonElement terrain = _map.GetProperty(propertyName: "terrain");
        Require(valid: terrain.GetArrayLength() == 1 && Number(row: terrain[0], name: "from") == -320
                && Number(row: terrain[0], name: "to") == 320 && Number(row: terrain[0], name: "height") == 0,
                code: "WORLD_CONTINUOUS_GROUND");
        Pair(row: terrain[0].GetProperty(propertyName: "navigation")[0], x: -320, y: 0, code: "WORLD_NAVIGATION");
        Pair(row: terrain[0].GetProperty(propertyName: "navigation")[1], x: 320, y: 0, code: "WORLD_NAVIGATION");
        ValidateWorldStates();
        ValidateSamples();
        ValidateNodes();
    }

    private void ValidateWorldStates()
    {
        JsonElement plants = _map.GetProperty(propertyName: "plants");
        Unique(rows: plants, key: "id", maximum: 6);
        Require(valid: plants.GetArrayLength() == 6, code: "WORLD_PLANTS");
        foreach (JsonElement plant in plants.EnumerateArray())
        {
            Require(valid: !plant.GetProperty(propertyName: "clearingRemoves").GetBoolean()
                    && Number(row: plant, name: "capacity") > 0, code: "WORLD_PLANT_STOCK");
            Part(pack: _world, key: Text(row: plant, name: "base"));
            foreach (JsonElement key in plant.GetProperty(propertyName: "stockLayers").EnumerateArray())
            {
                Part(pack: _world, key: key.GetString() ?? string.Empty);
            }
        }

        double[] foodPositions = [-10, -50, -16, 10, 50, 16];
        for (int index = 0; index < 6; index++)
        {
            Require(valid: Number(row: plants[index], name: "x") == foodPositions[index], code: "WORLD_FOOD_POSITION");
        }

        foreach (JsonElement clearing in _map.GetProperty(propertyName: "clearings").EnumerateArray())
        {
            Require(valid: clearing.GetProperty(propertyName: "foodIndependent").GetBoolean()
                    && !clearing.GetProperty(propertyName: "blocksWalking").GetBoolean(),
                    code: "WORLD_FOOD_INDEPENDENCE");
        }

        JsonElement states = _map.GetProperty(propertyName: "stockStates");
        Require(valid: states.GetArrayLength() == 4, code: "WORLD_STOCK_STATES");
        double[] fractions = [1, 0.5, 0, 1];
        for (int index = 0; index < 4; index++)
        {
            Require(valid: Number(row: states[index], name: "fraction") == fractions[index],
                    code: "WORLD_STOCK_STATES");
        }

        JsonElement reveal = _map.GetProperty(propertyName: "reveal");
        Require(valid: reveal.GetProperty(propertyName: "shared").GetBoolean()
                && reveal.GetProperty(propertyName: "preserveExplored").GetBoolean()
                && !reveal.GetProperty(propertyName: "liveEnemies").GetBoolean(), code: "WORLD_REVEAL");
    }

    private void ValidateNodes()
    {
        JsonElement rows = _map.GetProperty(propertyName: "native").GetProperty(propertyName: "nodes");
        Unique(rows: rows, key: "id", maximum: 512);
        Require(valid: rows.GetArrayLength() == 275, code: "WORLD_NODE_COUNT");
        foreach (JsonElement node in rows.EnumerateArray())
        {
            Require(valid: Same(left: Text(row: node, name: "type"), right: "Sprite2D")
                    && Same(left: Text(row: node, name: "parent"), right: "."),
                    code: "WORLD_NODE_TYPE");
            Part(pack: _world, key: Text(row: node, name: "part"));
            JsonElement p = node.GetProperty(propertyName: "position");
            Require(valid: p.GetArrayLength() == 2 && Math.Abs(value: p[0].GetDouble()) <= 6000
                    && Math.Abs(value: p[1].GetDouble()) <= 500, code: "WORLD_NODE_POSITION");
            Require(valid: Number(row: node, name: "z") is >= -100 and <= 20
                    && Math.Abs(value: Number(row: node, name: "rotation")) <= Math.PI / 2,
                    code: "WORLD_NODE_TRANSFORM");
            ValidateColor(row: node.GetProperty(propertyName: "color"));
        }
    }

    private void ValidateEffects()
    {
        JsonElement effects = _effects.GetProperty(propertyName: "effects");
        Unique(rows: effects, key: "id", maximum: 64);
        Require(valid: effects.GetArrayLength() == 51, code: "WORLD_EFFECT_COUNT");
        foreach (JsonElement effect in effects.EnumerateArray())
        {
            Part(pack: _effects, key: Text(row: effect, name: "part"));
            Require(valid: Number(row: effect, name: "duration") is > 0 and <= 10
                    && Number(row: effect, name: "amount") is > 0 and <= 12
                    && Number(row: effect, name: "radiusPixels") is > 0 and <= 24
                    && Number(row: effect, name: "speedPixelsPerSecond") is >= 0 and <= 200
                    && Number(row: effect, name: "gravityPixelsPerSecondSquared") is >= 0 and <= 100
                    && Number(row: effect, name: "emissionSeconds") > 0
                    && Number(row: effect, name: "emissionSeconds") <= Number(row: effect, name: "duration"),
                    code: "WORLD_EFFECT_TIMING");
            Require(valid: effect.GetProperty(propertyName: "amount").TryGetInt32(value: out int amount)
                    && amount is > 0 and <= 12, code: "WORLD_EFFECT_AMOUNT");
            foreach (string field in new[] { "loot", "damage", "collision", "status" })
            {
                Require(valid: !effect.GetProperty(propertyName: field).GetBoolean(), code: "WORLD_EFFECT_MECHANIC");
            }

            string kind = Text(row: effect, name: "kind");
            Require(valid: kind is "projectile" or "particles" or "stain" or "marker" or "warning",
                    code: "WORLD_EFFECT_KIND");
            ValidateColor(row: effect.GetProperty(propertyName: "color"));
            if (effect.GetProperty(propertyName: "sound").ValueKind != JsonValueKind.Null)
            {
                Require(valid: Text(row: effect, name: "sound").StartsWith(value: Text(row: effect,
                        name: "soundObligation") + ":", comparisonType: StringComparison.Ordinal),
                        code: "WORLD_SOUND_REFERENCE");
            }
        }

        Require(valid: Same(left: Text(row: _effects, name: "portalShader"), right: PortalShader),
                code: "WORLD_SHADER_SOURCE");
        JsonElement limits = _effects.GetProperty(propertyName: "limits");
        Require(valid: Number(row: limits, name: "total") == 24 && Number(row: limits, name: "perEffect") == 4
                && Number(row: limits, name: "bloodParticles") == 0
                && !limits.GetProperty(propertyName: "dismemberment").GetBoolean()
                && limits.GetProperty(propertyName: "cosmeticStainOnly").GetBoolean(), code: "WORLD_EFFECT_LIMITS");
        ValidateClips();
    }

    private void ValidateSamples()
    {
        JsonElement samples = _definition.GetProperty(propertyName: "samples");
        Unique(rows: samples, key: "id", maximum: 32);
        Require(valid: samples.GetArrayLength() == 24, code: "WORLD_SAMPLE_COUNT");
        foreach (JsonElement sample in samples.EnumerateArray())
        {
            string state = Text(row: sample, name: "portalState");
            Require(valid: state is "closed" or "idle" or "warning" or "spawning" or "damaged" or "destroying"
                    or "ruins", code: "WORLD_PORTAL_STATE");
            Find(rows: _map.GetProperty(propertyName: "stockStates"), key: "id",
                    value: Text(row: sample, name: "stock"));
            Find(rows: _map.GetProperty(propertyName: "clearingStates"), key: "id",
                    value: Text(row: sample, name: "clearing"));
            Find(rows: _map.GetProperty(propertyName: "rallySamples"), key: "id",
                    value: Text(row: sample, name: "rally"));
            Find(rows: _map.GetProperty(propertyName: "reveal").GetProperty(propertyName: "samples"), key: "id",
                    value: Text(row: sample, name: "reveal"));
            foreach (JsonElement loop in sample.GetProperty(propertyName: "loops").EnumerateArray())
            {
                Effect(id: loop.GetString() ?? string.Empty);
            }
        }
    }

    private void ValidateClips()
    {
        JsonElement clips = _effects.GetProperty(propertyName: "portalClips");
        Unique(rows: clips, key: "id", maximum: 6);
        Require(valid: clips.GetArrayLength() == 6, code: "WORLD_PORTAL_CLIPS");
        foreach (JsonElement clip in clips.EnumerateArray())
        {
            JsonElement original = Find(rows: _common.GetProperty(propertyName: "clips"), key: "id",
                    value: Text(row: clip, name: "id"));
            Require(valid: Same(left: JsonSerializer.Serialize(value: clip),
                    right: JsonSerializer.Serialize(value: original))
                    && !clip.GetProperty(propertyName: "callbacksExecutable").GetBoolean()
                    && clip.GetProperty(propertyName: "events").GetArrayLength() == 0,
                    code: "WORLD_CLIP_SOURCE");
            foreach (JsonElement layer in clip.GetProperty(propertyName: "layers").EnumerateArray())
            {
                double previous = -1;
                foreach (JsonElement key in layer.GetProperty(propertyName: "keys").EnumerateArray())
                {
                    double time = Number(row: key, name: "time");
                    Require(valid: time >= previous && time <= Number(row: clip, name: "stop") + 0.000001,
                            code: "WORLD_CLIP_TIME");
                    previous = time;
                    if (key.TryGetProperty(propertyName: "sprite", value: out JsonElement sprite)
                            && sprite.ValueKind == JsonValueKind.String)
                    {
                        Part(pack: _effects, key: sprite.GetString() ?? string.Empty);
                    }
                }
            }
        }
    }

    private void ValidatePortals()
    {
        JsonElement rows = _definition.GetProperty(propertyName: "portals");
        Unique(rows: rows, key: "id", maximum: 6);
        Require(valid: rows.GetArrayLength() == 6, code: "WORLD_PORTALS");
        foreach (JsonElement row in rows.EnumerateArray())
        {
            JsonElement source = Find(rows: _map.GetProperty(propertyName: "portals"), key: "id",
                    value: Text(row: row, name: "id"));
            Require(valid: Number(row: row, name: "x") == Number(row: source, name: "x")
                    && Same(left: Text(row: row, name: "side"), right: Text(row: source, name: "side"))
                    && Number(row: row, name: "sourceIndex") == Number(row: source, name: "sourceIndex")
                    && Same(left: Text(row: row, name: "front"), right: "common.s0.8291")
                    && Same(left: Text(row: row, name: "back"), right: "common.s0.7865"),
                    code: "WORLD_PORTAL_IDENTITY");
            Require(valid: row.GetProperty(propertyName: "ruinPassable").GetBoolean()
                    && row.GetProperty(propertyName: "ruinPermanent").GetBoolean()
                    && !row.GetProperty(propertyName: "ruinActive").GetBoolean()
                    && !row.GetProperty(propertyName: "ruinRegrows").GetBoolean(), code: "WORLD_RUIN_STATE");
            JsonElement blocker = row.GetProperty(propertyName: "blocker");
            Require(valid: blocker.GetProperty(propertyName: "extendsAllJumpHeights").GetBoolean(),
                    code: "WORLD_PORTAL_JUMP");
            JsonElement rect = blocker.GetProperty(propertyName: "rect");
            Require(valid: rect.GetArrayLength() == 4 && rect[0].GetDouble() == -1 && rect[1].GetDouble() == -64
                    && rect[2].GetDouble() == 2 && rect[3].GetDouble() == 70, code: "WORLD_PORTAL_GEOMETRY");
            foreach (JsonElement debris in row.GetProperty(propertyName: "debris").EnumerateArray())
            {
                Part(pack: _effects, key: debris.GetString() ?? string.Empty);
            }
        }

        Require(valid: _definition.GetProperty(propertyName: "restoration").GetProperty(
                propertyName: "silent").GetBoolean() && !_definition.GetProperty(propertyName: "restoration")
                        .GetProperty(propertyName: "replayPastEvents").GetBoolean(), code: "WORLD_RESTORATION");
        Unique(rows: _definition.GetProperty(propertyName: "samples"), key: "id", maximum: 32);
    }

    private void ValidateSourceParts()
    {
        foreach (JsonElement part in _effects.GetProperty(propertyName: "parts").EnumerateArray())
        {
            JsonElement source = Find(rows: _common.GetProperty(propertyName: "resources"), key: "id",
                    value: Text(row: part, name: "key"));
            string hashField = source.TryGetProperty(propertyName: "pngSha256", value: out _) ? "pngSha256" : "sha256";
            Require(valid: Same(left: Text(row: part, name: "sourceMember"), right: Text(row: source, name: "member"))
                    && Same(left: Text(row: part, name: "sourceSha256"), right: Text(row: source, name: hashField))
                    && Same(left: JsonSerializer.Serialize(value: part.GetProperty(propertyName: "source")),
                            right: JsonSerializer.Serialize(value: source.GetProperty(propertyName: "source"))),
                    code: "WORLD_PART_SOURCE");
            if (part.GetProperty(propertyName: "registration").ValueKind != JsonValueKind.Null)
            {
                JsonElement registration = part.GetProperty(propertyName: "registration");
                Require(valid: Same(left: JsonSerializer.Serialize(value: registration),
                        right: JsonSerializer.Serialize(value: source.GetProperty(propertyName: "registration"))),
                        code: "WORLD_PART_REGISTRATION");
            }
        }

        JsonElement events = _effects.GetProperty(propertyName: "soundEvents");
        Unique(rows: events, key: "id", maximum: 64);
        foreach (JsonElement effect in _effects.GetProperty(propertyName: "effects").EnumerateArray())
        {
            if (effect.GetProperty(propertyName: "sound").ValueKind == JsonValueKind.String)
            {
                JsonElement binding = Find(rows: events, key: "id", value: Text(row: effect, name: "sound"));
                string obligation = Text(row: binding, name: "obligation");
                Require(valid: obligation.Length == 5 && obligation.StartsWith(value: "AU-",
                        comparisonType: StringComparison.Ordinal)
                        && int.TryParse(s: obligation.AsSpan(start: 3), provider: CultureInfo.InvariantCulture,
                                result: out int number) && number is >= 1 and <= 30, code: "WORLD_SOUND_OBLIGATION");
            }
        }
    }

    private static void Require(bool valid, string code)
    {
        if (!valid)
        {
            throw new InvalidDataException(message: code);
        }
    }

    private static void Unique(JsonElement rows, string key, int maximum)
    {
        Require(valid: rows.GetArrayLength() is > 0 && rows.GetArrayLength() <= maximum, code: "WORLD_ARRAY_COUNT");
        var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement row in rows.EnumerateArray())
        {
            string value = Text(row: row, name: key);
            Require(valid: value.Length is > 0 and <= 100 && keys.Add(item: value), code: "WORLD_DUPLICATE_ID");
        }
    }

    private static JsonElement Find(JsonElement rows, string key, string value)
    {
        foreach (JsonElement row in rows.EnumerateArray())
        {
            if (Same(left: Text(row: row, name: key), right: value))
            {
                return row;
            }
        }

        throw new InvalidDataException(message: "WORLD_REFERENCE " + value);
    }

    private static JsonElement Part(JsonElement pack, string key)
    {
        return Find(rows: pack.GetProperty(propertyName: "parts"), key: "key", value: key);
    }

    private JsonElement Effect(string id)
    {
        return Find(rows: _effects.GetProperty(propertyName: "effects"), key: "id", value: id);
    }

    private static string Text(JsonElement row, string name)
    {
        return row.GetProperty(propertyName: name).GetString() ?? throw new InvalidDataException(message: "WORLD_TEXT");
    }

    private static double Number(JsonElement row, string name)
    {
        return row.GetProperty(propertyName: name).GetDouble();
    }

    private static void Pair(JsonElement row, double x, double y, string code)
    {
        Require(valid: row.GetArrayLength() == 2 && row[0].GetDouble() == x && row[1].GetDouble() == y, code: code);
    }

    private static void ValidateColor(JsonElement row)
    {
        Require(valid: row.GetArrayLength() == 4 && row.EnumerateArray().All(predicate: item =>
                item.GetDouble() is >= 0 and <= 1), code: "WORLD_COLOR");
    }

    private bool ValidSource(string source)
    {
        return source is "friendly" or "enemy" or "tower" or "survivor" or "bloater" or "spitter"
                || _definition.GetProperty(propertyName: "portals").EnumerateArray().Any(
                        predicate: row => Same(left: Text(row: row, name: "id"), right: source));
    }

    private void Cancel()
    {
        _epoch = Interlocked.Increment(location: ref _nextEpoch);
        _time = 0;
        _events = 0;
        _transients.Clear();
    }

    private void RequireActive()
    {
        _gateway.RequireThread();
        Require(valid: !_closed && _attached && GodotObject.IsInstanceValid(instance: _root)
                && (_owners.Count == 0 || (GodotObject.IsInstanceValid(instance: _owners[0])
                        && ReferenceEquals(objA: _owners[0].GetParent(), objB: _root))),
                code: "WORLD_OWNER_CLOSED");
    }

    private void Retire()
    {
        _gateway.RequireThread();
        foreach (Node2D owner in _owners)
        {
            if (GodotObject.IsInstanceValid(instance: owner))
            {
                if (GodotObject.IsInstanceValid(instance: _root))
                {
                    _root.RemoveChild(node: owner);
                }

                owner.Free();
            }
        }

        _owners.Clear();

        foreach (Resource resource in _resources)
        {
            resource.Dispose();
        }

        _resources.Clear();
    }

    private void Project()
    {
        _gateway.RequireThread();
        RequireActive();
        Retire();
        var owner = new Node2D { Name = "WorldPreview", TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
        owner.SetMeta(name: "resource_id", value: _sample);
        owner.SetMeta(name: "epoch", value: _epoch);
        _owners.Add(item: owner);
        _root.AddChild(node: owner);
        JsonElement sample = Find(rows: _definition.GetProperty(propertyName: "samples"), key: "id", value: _sample);
        ProjectMap(sample: sample);
        ProjectPortals(state: PortalState(sample: sample));
        ProjectMinimap(sample: sample);
        ProjectLoops(sample: sample);
        foreach (EffectState effect in _transients)
        {
            ProjectEffect(effect: effect);
        }
    }

    private void ProjectMap(JsonElement sample)
    {
        _gateway.RequireThread();
        string stock = Text(row: sample, name: "stock"), clearing = Text(row: sample, name: "clearing");
        double fraction = Number(row: Find(rows: _map.GetProperty(propertyName: "stockStates"),
                key: "id", value: stock),
                name: "fraction");
        JsonElement state = Find(rows: _map.GetProperty(propertyName: "clearingStates"), key: "id", value: clearing);
        JsonElement nodes = _map.GetProperty(propertyName: "native").GetProperty(propertyName: "nodes");
        foreach (JsonElement row in nodes.EnumerateArray())
        {
            string id = Text(row: row, name: "id");
            if (id.Contains(value: ".portal.", comparisonType: StringComparison.Ordinal))
            {
                continue;
            }

            Sprite2D sprite = Sprite(key: Text(row: row, name: "part"), world: true, id: id);
            JsonElement p = row.GetProperty(propertyName: "position");
            sprite.Position = new Vector2(x: p[0].GetSingle(), y: p[1].GetSingle());
            sprite.ZIndex = row.GetProperty(propertyName: "z").GetInt32();
            sprite.Modulate = ReadColor(row: row.GetProperty(propertyName: "color"));
            if (Same(left: Text(row: sample, name: "lighting"), right: "night"))
            {
                JsonElement tint = _map.GetProperty(propertyName: "lighting")[3].GetProperty(propertyName: "tint");
                sprite.Modulate *= ReadColor(row: tint);
            }

            ProjectMapState(sprite: sprite, id: id, sample: sample, state: state, fraction: fraction);
        }

        ProjectGeometry(state: PortalState(sample: sample));
    }

    private void ProjectMapState(Sprite2D sprite, string id, JsonElement sample, JsonElement state, double fraction)
    {
        _gateway.RequireThread();
        foreach (JsonElement plant in _map.GetProperty(propertyName: "plants").EnumerateArray())
        {
            if (id.StartsWith(value: Text(row: plant, name: "id") + ".stock.",
                    comparisonType: StringComparison.Ordinal))
            {
                int layers = plant.GetProperty(propertyName: "stockLayers").GetArrayLength();
                sprite.Visible = id[^1] - '0' < Math.Ceiling(a: layers * fraction);
            }
        }

        foreach (JsonElement clearing in _map.GetProperty(propertyName: "clearings").EnumerateArray())
        {
            if (id.StartsWith(value: Text(row: clearing, name: "id") + ".", comparisonType: StringComparison.Ordinal))
            {
                sprite.Visible = !state.GetProperty(propertyName: "stump").GetBoolean();
                float angle = state.GetProperty(propertyName: "rotation").GetSingle();
                double duration = Number(row: state, name: "duration");
                angle *= duration > 0 ? (float)Math.Min(val1: 1, val2: _time / duration) : 1;
                int tree = id[^1] - '0';
                JsonElement offset = clearing.GetProperty(propertyName: "trees")[tree].GetProperty(
                        propertyName: "offset");
                float pivot = clearing.GetProperty(propertyName: "x").GetSingle() * 8 + offset[0].GetSingle() * 8;
                var origin = new Vector2(x: pivot, y: 0);
                sprite.Position = origin + (sprite.Position - origin).Rotated(angle: angle);
                sprite.Rotation = angle;
            }
        }

        for (int index = 0; index < 2; index++)
        {
            JsonElement rally = _map.GetProperty(propertyName: "rallies")[index];
            if (id.StartsWith(value: Text(row: rally, name: "id") + ".", comparisonType: StringComparison.Ordinal))
            {
                JsonElement current = Find(rows: _map.GetProperty(propertyName: "rallySamples"), key: "id",
                        value: Text(row: sample, name: "rally"));
                sprite.Position = new Vector2(x: current.GetProperty(propertyName: "rallies")[index].GetSingle() * 8,
                        y: sprite.Position.Y);
            }
        }
    }

    private void ProjectGeometry(string state)
    {
        _gateway.RequireThread();
        AddCollision(id: "ground.geometry", x: 0, y: 24, width: 5120, height: 48, layer: 1);
        AddCollision(id: "left.edge", x: -2564, y: 0, width: 8, height: 1024, layer: 4);
        AddCollision(id: "right.edge", x: 2564, y: 0, width: 8, height: 1024, layer: 4);
        if (!Same(left: state, right: "ruins"))
        {
            foreach (JsonElement portal in _definition.GetProperty(propertyName: "portals").EnumerateArray())
            {
                AddCollision(id: Text(row: portal, name: "id") + ".blocker",
                        x: portal.GetProperty(propertyName: "x").GetSingle() * 8,
                        y: -232, width: 16, height: 560, layer: 2);
            }
        }

        var navigation = new NavigationPolygon
        {
            Vertices = [new Vector2(x: -2560, y: -8), new Vector2(x: 2560, y: -8),
                new Vector2(x: 2560, y: 0), new Vector2(x: -2560, y: 0)],
            AgentRadius = 0,
        };
        navigation.AddPolygon(polygon: [0, 1, 2, 3]);
        _resources.Add(item: navigation);
        _owners[0].AddChild(node: new NavigationRegion2D { Name = "Navigation", NavigationPolygon = navigation });
        ProjectClearingMarkers();
    }

    private void AddCollision(string id, float x, float y, float width, float height, uint layer)
    {
        _gateway.RequireThread();
        var body = new StaticBody2D { Position = new Vector2(x: x, y: y), CollisionLayer = layer, CollisionMask = 1 };
        body.SetMeta(name: "resource_id", value: id);
        var shape = new RectangleShape2D { Size = new Vector2(x: width, y: height) };
        _resources.Add(item: shape);
        body.AddChild(node: new CollisionShape2D { Shape = shape });
        _owners[0].AddChild(node: body);
    }

    private void ProjectClearingMarkers()
    {
        _gateway.RequireThread();
        JsonElement sample = Find(rows: _definition.GetProperty(propertyName: "samples"), key: "id", value: _sample);
        JsonElement state = Find(rows: _map.GetProperty(propertyName: "clearingStates"), key: "id",
                value: Text(row: sample, name: "clearing"));
        bool stump = state.GetProperty(propertyName: "stump").GetBoolean();
        if (!stump && !state.GetProperty(propertyName: "mark").GetBoolean())
        {
            return;
        }

        foreach (JsonElement clearing in _map.GetProperty(propertyName: "clearings").EnumerateArray())
        {
            for (int index = 0; index < 2; index++)
            {
                Sprite2D sprite = Sprite(key: stump ? "stump" : "tree-mark", world: true,
                        id: Text(row: clearing, name: "id") + ".state."
                                + index.ToString(provider: CultureInfo.InvariantCulture));
                JsonElement offset = clearing.GetProperty(propertyName: "trees")[index].GetProperty(
                        propertyName: "offset");
                sprite.Position = new Vector2(x: clearing.GetProperty(propertyName: "x").GetSingle() * 8
                        + offset[0].GetSingle() * 8,
                        y: stump ? -20 : -60);
            }
        }
    }

    private void ProjectPortals(string state)
    {
        _gateway.RequireThread();
        foreach (JsonElement portal in _definition.GetProperty(propertyName: "portals").EnumerateArray())
        {
            string id = Text(row: portal, name: "id");
            float x = portal.GetProperty(propertyName: "x").GetSingle() * 8;
            if (Same(left: state, right: "ruins"))
            {
                ProjectRuin(portal: portal, x: x);
                continue;
            }

            Sprite2D back = Sprite(key: Text(row: portal, name: "back"), world: false, id: id + ".back");
            back.Position = new Vector2(x: x - 22, y: -70);
            back.ZIndex = 1;
            ProjectInterior(id: id, x: x, active: state is not "closed");
            Sprite2D front = Sprite(key: Text(row: portal, name: "front"), world: false, id: id + ".front");
            front.Position = new Vector2(x: x - 24, y: -70);
            front.ZIndex = 5;
            ProjectPortalClip(portal: portal, state: state, x: x);
            if (Same(left: state, right: "warning"))
            {
                ProjectWarning(effect: new EffectState(id: "portal.warning", source: id, started: 0,
                        originX: x, originY: -70, targetX: x, targetY: -70, contacted: false),
                        row: Effect(id: "portal.warning"));
            }
        }
    }

    private void ProjectRuin(JsonElement portal, float x)
    {
        _gateway.RequireThread();
        int index = 0;
        foreach (JsonElement part in portal.GetProperty(propertyName: "debris").EnumerateArray())
        {
            Sprite2D sprite = Sprite(key: part.GetString() ?? string.Empty, world: false,
                    id: Text(row: portal, name: "id") + ".ruin."
                            + index.ToString(provider: CultureInfo.InvariantCulture));
            sprite.Position = new Vector2(x: x - 24 + index * 3, y: -6 - (index % 2) * 3);
            sprite.ZIndex = 3;
            sprite.SetMeta(name: "permanent", value: true);
            sprite.SetMeta(name: "active", value: false);
            sprite.SetMeta(name: "passable", value: true);
            index++;
        }
    }

    private void ProjectInterior(string id, float x, bool active)
    {
        _gateway.RequireThread();
        var shader = new Shader { Code = PortalShader };
        var material = new ShaderMaterial { Shader = shader };
        material.SetShaderParameter(param: "preview_time", value: (float)_time);
        material.SetShaderParameter(param: "tint",
                value: ReadColor(row: _effects.GetProperty(propertyName: "portalColor")));
        _resources.Add(item: material);
        _resources.Add(item: shader);
        var interior = new Polygon2D
        {
            Polygon = [new Vector2(x: -18, y: -62), new Vector2(x: 18, y: -62), new Vector2(x: 18, y: 0),
                new Vector2(x: -18, y: 0)],
            UV = [new Vector2(x: 0, y: 0), new Vector2(x: 1, y: 0), new Vector2(x: 1, y: 1), new Vector2(x: 0, y: 1)],
            Position = new Vector2(x: x, y: 0),
            Material = material,
            Visible = active,
            ZIndex = 2,
        };
        interior.SetMeta(name: "resource_id", value: id + ".interior");
        _owners[0].AddChild(node: interior);
    }

    private void ProjectPortalClip(JsonElement portal, string state, float x)
    {
        _gateway.RequireThread();
        string clipId = Text(row: _definition.GetProperty(propertyName: "clips"), name: state);
        JsonElement clip = Find(rows: _effects.GetProperty(propertyName: "portalClips"), key: "id", value: clipId);
        double length = Number(row: clip, name: "stop");
        double time = clip.GetProperty(propertyName: "loop").GetBoolean() ? _time % length
                : Math.Min(val1: _time, val2: length);
        int index = 0;
        foreach (JsonElement layer in clip.GetProperty(propertyName: "layers").EnumerateArray())
        {
            JsonElement[] keys = layer.GetProperty(propertyName: "keys").EnumerateArray().Where(
                    predicate: key => key.TryGetProperty(propertyName: "sprite", value: out JsonElement sprite)
                            && sprite.ValueKind == JsonValueKind.String).ToArray();
            if (keys.Length == 0)
            {
                continue;
            }

            JsonElement key = keys.Last(predicate: row => Number(row: row, name: "time") <= time + 0.000001);
            string partKey = Text(row: key, name: "sprite");
            JsonElement part = Part(pack: _effects, key: partKey);
            Sprite2D sprite = Sprite(key: partKey, world: false, id: Text(row: portal, name: "id") + ".clip."
                    + index.ToString(provider: CultureInfo.InvariantCulture));
            JsonElement registration = part.GetProperty(propertyName: "registration").GetProperty(
                    propertyName: "offsetTopLeftExact");
            sprite.Position = new Vector2(x: x + registration[0].GetSingle(), y: registration[1].GetSingle()
                    - part.GetProperty(propertyName: "registration").GetProperty(propertyName: "logicalRect")
                            .GetProperty(propertyName: "height").GetSingle());
            sprite.ZIndex = 3;
            sprite.SetMeta(name: "clip_id", value: clipId);
            sprite.SetMeta(name: "sample_time", value: time);
            ProjectAnimation(layer: layer, sprite: sprite, length: length, id: clipId + "." + index.ToString(
                    provider: CultureInfo.InvariantCulture));
            index++;
        }
    }

    private void ProjectAnimation(JsonElement layer, Sprite2D sprite, double length, string id)
    {
        _gateway.RequireThread();
        var animation = new Animation { Length = (float)length };
        int track = animation.AddTrack(type: Animation.TrackType.Value);
        animation.TrackSetPath(trackIdx: track, path: ".:texture");
        animation.ValueTrackSetUpdateMode(trackIdx: track, mode: Animation.UpdateMode.Discrete);
        foreach (JsonElement key in layer.GetProperty(propertyName: "keys").EnumerateArray())
        {
            if (key.TryGetProperty(propertyName: "sprite", value: out JsonElement value)
                    && value.ValueKind == JsonValueKind.String)
            {
                animation.TrackInsertKey(trackIdx: track, time: Number(row: key, name: "time"),
                        key: Texture(key: value.GetString() ?? string.Empty, world: false));
            }
        }

        _resources.Add(item: animation);
        sprite.SetMeta(name: "native_animation", value: animation);
        sprite.SetMeta(name: "animation_id", value: id);
    }

    private void ProjectMinimap(JsonElement sample)
    {
        _gateway.RequireThread();
        JsonElement reveal = Find(rows: _map.GetProperty(propertyName: "reveal").GetProperty(propertyName: "samples"),
                key: "id", value: Text(row: sample, name: "reveal"));
        foreach (JsonElement portal in _map.GetProperty(propertyName: "portals").EnumerateArray())
        {
            double x = Number(row: portal, name: "x");
            bool known = reveal.GetProperty(propertyName: "explored").EnumerateArray().Any(
                    predicate: range => range[0].GetDouble() <= x && range[1].GetDouble() >= x);
            if (known)
            {
                bool destroyed = reveal.GetProperty(propertyName: "destroyedPortals").EnumerateArray().Any(
                        predicate: id => Same(left: id.GetString(), right: Text(row: portal, name: "id")));
                Sprite2D sprite = Sprite(key: destroyed ? "portal-map-ruin" : "portal-map", world: true,
                        id: "minimap." + Text(row: portal, name: "id"));
                sprite.Position = new Vector2(x: (float)x, y: 160);
            }
        }

        foreach (JsonElement player in reveal.GetProperty(propertyName: "players").EnumerateArray())
        {
            Sprite2D sprite = Sprite(key: "pointer-map", world: true, id: "minimap." + Text(row: player, name: "id"));
            sprite.Position = new Vector2(x: player.GetProperty(propertyName: "x").GetSingle(), y: 160);
            sprite.Modulate = ReadColor(row: player.GetProperty(propertyName: "color"));
            sprite.SetMeta(name: "player_number", value: player.GetProperty(propertyName: "number").GetInt32());
            sprite.SetMeta(name: "player_shape", value: Text(row: player, name: "shape"));
            sprite.SetMeta(name: "player_name", value: Text(row: player, name: "name"));
        }

        foreach (JsonElement id in reveal.GetProperty(propertyName: "knownDefenses").EnumerateArray())
        {
            JsonElement site = _map.GetProperty(propertyName: "walls").EnumerateArray().Concat(
                    second: _map.GetProperty(propertyName: "towers").EnumerateArray()).Single(
                            predicate: row => Same(left: Text(row: row, name: "id"), right: id.GetString()));
            Sprite2D sprite = Sprite(key: "tree-mark", world: true, id: "minimap." + id.GetString());
            sprite.Position = new Vector2(x: site.GetProperty(propertyName: "x").GetSingle(), y: 160);
        }
    }

    private void ProjectLoops(JsonElement sample)
    {
        _gateway.RequireThread();
        foreach (JsonElement loop in sample.GetProperty(propertyName: "loops").EnumerateArray())
        {
            string id = loop.GetString() ?? string.Empty;
            ProjectEffect(effect: new EffectState(id: id, source: "survivor", started: 0, originX: 0,
                    originY: -8, targetX: 0, targetY: -8, contacted: false));
        }

        if (sample.GetProperty(propertyName: "loops").EnumerateArray().Any(
                predicate: row => Same(left: row.GetString(), right: "fire.loop")))
        {
            var gradient = new Gradient();
            gradient.SetColor(point: 0, color: new Color(r: 1, g: 1, b: 1, a: 1));
            gradient.SetColor(point: 1, color: new Color(r: 0, g: 0, b: 0, a: 0));
            var texture = new GradientTexture2D { Gradient = gradient, Width = 64, Height = 64,
                Fill = GradientTexture2D.FillEnum.Radial };
            _resources.Add(item: texture);
            _resources.Add(item: gradient);
            var light = new PointLight2D { Texture = texture, Energy = 0.7f,
                Color = new Color(r: 1, g: 0.65f, b: 0.28f), Position = new Vector2(x: 0, y: -12) };
            light.SetMeta(name: "resource_id", value: "current.firelight");
            _owners[0].AddChild(node: light);
        }
    }

    private void ProjectEffect(EffectState effect)
    {
        _gateway.RequireThread();
        JsonElement row = Effect(id: effect.Id);
        double age = _time - effect.Started;
        string kind = Text(row: row, name: "kind");
        if (effect.Contacted && Same(left: kind, right: "projectile"))
        {
            return;
        }

        if (Same(left: kind, right: "particles"))
        {
            ProjectParticles(effect: effect, row: row, age: age);
        }
        else if (Same(left: kind, right: "warning") || Same(left: kind, right: "marker"))
        {
            ProjectWarning(effect: effect, row: row);
        }
        else
        {
            Sprite2D sprite = Sprite(key: Text(row: row, name: "part"), world: false,
                    id: "effect." + effect.Id + "." + _events.ToString(provider: CultureInfo.InvariantCulture));
            sprite.Modulate = ReadColor(row: row.GetProperty(propertyName: "color"));
            sprite.Position = new Vector2(x: effect.OriginX, y: effect.OriginY);
            if (Same(left: kind, right: "projectile"))
            {
                var delta = new Vector2(x: effect.TargetX - effect.OriginX, y: effect.TargetY - effect.OriginY);
                sprite.Position += delta.Normalized() * Math.Min(val1: delta.Length(), val2: (float)age
                        * row.GetProperty(propertyName: "speedPixelsPerSecond").GetSingle());
                sprite.Rotation = delta.Angle();
            }
            else if (Same(left: kind, right: "stain"))
            {
                float fade = row.GetProperty(propertyName: "fadeSeconds").GetSingle();
                float alpha = (float)Math.Min(val1: 1, val2: (Number(row: row, name: "duration") - age) / fade);
                sprite.Modulate = new Color(r: sprite.Modulate.R, g: sprite.Modulate.G, b: sprite.Modulate.B,
                        a: sprite.Modulate.A * alpha);
                sprite.SetMeta(name: "cosmetic_only", value: true);
            }

            sprite.ZIndex = 20;
        }
    }

    private void ProjectParticles(EffectState effect, JsonElement row, double age)
    {
        _gateway.RequireThread();
        bool loop = row.GetProperty(propertyName: "loop").GetBoolean();
        float lifetime = row.GetProperty(propertyName: "duration").GetSingle();
        var particles = new CpuParticles2D
        {
            Amount = row.GetProperty(propertyName: "amount").GetInt32(),
            Lifetime = lifetime,
            OneShot = !loop,
            Emitting = true,
            SpeedScale = 0,
            UseFixedSeed = true,
            Seed = 13,
            Texture = Texture(key: Text(row: row, name: "part"), world: false),
            Position = new Vector2(x: effect.OriginX, y: effect.OriginY),
            Direction = new Vector2(x: 0, y: -1),
            Spread = 45,
            InitialVelocityMin = 6,
            InitialVelocityMax = row.GetProperty(propertyName: "speedPixelsPerSecond").GetSingle(),
            Gravity = new Vector2(x: 0, y: row.GetProperty(propertyName: "gravityPixelsPerSecondSquared").GetSingle()),
            Color = ReadColor(row: row.GetProperty(propertyName: "color")),
            ZIndex = 20,
            AnimSpeedMin = 1,
            AnimSpeedMax = 1,
        };
        var material = new CanvasItemMaterial { ParticlesAnimation = true,
            ParticlesAnimHFrames = row.GetProperty(propertyName: "textureFrames")[0].GetInt32(),
            ParticlesAnimVFrames = row.GetProperty(propertyName: "textureFrames")[1].GetInt32(),
            ParticlesAnimLoop = loop };
        particles.Material = material;
        _resources.Add(item: material);
        particles.SetMeta(name: "resource_id", value: "effect." + effect.Id);
        particles.SetMeta(name: "source_id", value: effect.Source);
        particles.SetMeta(name: "preview_time", value: age);
        particles.SetMeta(name: "sound_reference",
                value: row.GetProperty(propertyName: "sound").GetString() ?? string.Empty);
        _owners[0].AddChild(node: particles);
        particles.RequestParticlesProcess(processTime: (float)(loop ? age % lifetime
                : Math.Min(val1: age, val2: lifetime)));
    }

    private void ProjectWarning(EffectState effect, JsonElement row)
    {
        _gateway.RequireThread();
        string label = row.TryGetProperty(propertyName: "label", value: out JsonElement text)
                ? text.GetString() ?? string.Empty : effect.Id;
        var canvas = new CanvasLayer { Layer = Same(left: Text(row: row, name: "kind"), right: "warning") ? 100 : 50 };
        int index = _definition.GetProperty(propertyName: "portals").EnumerateArray().TakeWhile(
                predicate: portal => !Same(left: Text(row: portal, name: "id"), right: effect.Source)).Count();
        var warning = new Label { Text = label + " / " + effect.Source,
            Position = new Vector2(x: 24, y: 64 + Math.Min(val1: index, val2: 5) * 28),
            Modulate = new Color(r: 0.96f, g: 0.95f, b: 0.87f) };
        warning.SetMeta(name: "resource_id", value: "effect." + effect.Id);
        warning.SetMeta(name: "source_id", value: effect.Source);
        canvas.AddChild(node: warning);
        _owners[0].AddChild(node: canvas);
        if (Same(left: Text(row: row, name: "kind"), right: "marker"))
        {
            Sprite2D marker = Sprite(key: Text(row: row, name: "part"), world: false,
                    id: "effect." + effect.Id + ".marker");
            marker.Position = new Vector2(x: effect.OriginX, y: effect.OriginY - 12);
            marker.Modulate = ReadColor(row: row.GetProperty(propertyName: "color"));
            var outline = new Line2D { Width = 1, Closed = true,
                Points = [new Vector2(x: -6, y: -12), new Vector2(x: 6, y: -12),
                    new Vector2(x: 6, y: 0), new Vector2(x: -6, y: 0)],
                DefaultColor = new Color(r: 0.95f, g: 0.94f, b: 0.85f),
                Position = new Vector2(x: effect.OriginX, y: effect.OriginY) };
            outline.SetMeta(name: "resource_id", value: "effect." + effect.Id + ".outline");
            _owners[0].AddChild(node: outline);
        }
    }

    private Sprite2D Sprite(string key, bool world, string id)
    {
        _gateway.RequireThread();
        var sprite = new Sprite2D { Texture = Texture(key: key, world: world), Centered = false,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
        sprite.SetMeta(name: "resource_id", value: id);
        sprite.SetMeta(name: "part", value: key);
        _owners[0].AddChild(node: sprite);
        return sprite;
    }

    private AtlasTexture Texture(string key, bool world)
    {
        _gateway.RequireThread();
        JsonElement part = Part(pack: world ? _world : _effects, key: key);
        JsonElement rect = part.GetProperty(propertyName: "atlasRect");
        var texture = new AtlasTexture { Atlas = world ? _worldAtlas : _effectAtlas,
            Region = new Rect2(x: rect[0].GetSingle(), y: rect[1].GetSingle(), width: rect[2].GetSingle(),
                    height: rect[3].GetSingle()), FilterClip = true };
        _resources.Add(item: texture);
        return texture;
    }

    private Color ReadColor(JsonElement row)
    {
        _gateway.RequireThread();
        return new Color(r: row[0].GetSingle(), g: row[1].GetSingle(), b: row[2].GetSingle(), a: row[3].GetSingle());
    }

    private static bool Same(string? left, string? right)
    {
        return StringComparer.Ordinal.Equals(x: left, y: right);
    }

    private string PortalState(JsonElement sample)
    {
        string state = Text(row: sample, name: "portalState");
        return Same(left: state, right: "destroying") && _time >= Number(row: Effect(id: "portal.destroy"),
                name: "duration") ? "ruins" : state;
    }

    private sealed record EffectState
    {
        internal EffectState(string id, string source, double started, float originX, float originY,
                float targetX, float targetY, bool contacted)
        {
            Id = id;
            Source = source;
            Started = started;
            OriginX = originX;
            OriginY = originY;
            TargetX = targetX;
            TargetY = targetY;
            Contacted = contacted;
        }

        internal string Id { get; init; }
        internal string Source { get; init; }
        internal double Started { get; init; }
        internal float OriginX { get; init; }
        internal float OriginY { get; init; }
        internal float TargetX { get; init; }
        internal float TargetY { get; init; }
        internal bool Contacted { get; init; }
    }

}
