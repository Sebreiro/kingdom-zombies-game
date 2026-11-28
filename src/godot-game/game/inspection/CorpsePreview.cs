using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Owns one silent current corpse or pile composition and explicit transient preview state.</summary>
internal sealed class CorpsePreview
{
    internal CorpsePreview(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read,
            JsonElement definition, JsonElement common, JsonElement weapons, JsonElement clothing)
    {
        _root = root;
        _gateway = gateway;
        _read = read;
        _definition = definition;
        _common = common;
        _packs = [weapons, clothing];
        _state = new AppearanceState(definition: System.Text.Encoding.UTF8.GetBytes(s: definition.GetRawText()));
    }

    internal JsonElement Snapshot => _state.Snapshot;

    internal static CorpsePreview Open(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read)
    {
        gateway.RequireThread();
        JsonElement definition = CharacterPreview.ReadDefinition(bytes: read(arg: "content/packs/corpses-wildlife.json")
                ?? throw new InvalidDataException(message: "CORPSE_DEFINITION_MISSING"),
                expectedId: "corpses-wildlife");
        string?[] declared = definition.GetProperty(propertyName: "sourceBindings").EnumerateArray().Select(
                selector: value => value.GetProperty(propertyName: "file").GetString()).ToArray();
        if (!declared.Order(comparer: StringComparer.Ordinal).SequenceEqual(
                second: _requiredSources.Order(comparer: StringComparer.Ordinal), comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "CORPSE_BINDING_SET");
        }

        foreach (JsonElement binding in definition.GetProperty(propertyName: "sourceBindings").EnumerateArray())
        {
            string file = binding.GetProperty(propertyName: "file").GetString() ?? MissingText();
            if (file.Length > 160 || file.Contains(value: "..", comparisonType: StringComparison.Ordinal)
                    || file.Contains(value: ':')
                    || file.Contains(value: '\\')
                    || !(file.StartsWith(value: "content/", comparisonType: StringComparison.Ordinal)
                            || file.StartsWith(value: "assets/", comparisonType: StringComparison.Ordinal)))
            {
                throw new InvalidDataException(message: "CORPSE_BINDING_PATH");
            }

            byte[] bytes = read(arg: file) ?? throw new InvalidDataException(message: "CORPSE_BINDING_MISSING " + file);
            if (bytes.Length > 16777216 || !StringComparer.Ordinal.Equals(x: Convert.ToHexStringLower(
                    inArray: SHA256.HashData(source: bytes)),
                    y: binding.GetProperty(propertyName: "sha256").GetString()))
            {
                throw new InvalidDataException(message: "CORPSE_BINDING_HASH " + file);
            }
        }

        return new CorpsePreview(root: root, gateway: gateway, read: read, definition: definition,
                common: ReadPack(read: read, id: "common"), weapons: ReadPack(read: read, id: "items-01-10"),
                clothing: ReadPack(read: read, id: "items-11-24"));
    }

    internal void Restore(string body, string kind, JsonElement equipment, bool selected, int facing = 1)
    {
        _gateway.RequireThread();
        RequireActive();
        if (facing is not (-1 or 1))
        {
            throw new InvalidDataException(message: "CORPSE_FACING");
        }

        _state.Restore(body: body, kind: kind, equipment: equipment, selected: selected);
        _facing = facing;
        Project();
    }

    internal void TriggerDeath()
    {
        _gateway.RequireThread();
        RequireActive();
        _state.TriggerDeath();
        Project();
    }

    internal void Advance(double elapsedSeconds)
    {
        _gateway.RequireThread();
        RequireActive();
        _state.Advance(elapsedSeconds: elapsedSeconds);
        Project();
    }

    internal void Reset()
    {
        _gateway.RequireThread();
        RequireActive();
        _state.Reset();
        Project();
    }

    internal void Close()
    {
        _gateway.RequireThread();
        if (!_closed)
        {
            _closed = true;
            _state.Reset();
            Retire();
            _root.RemoveMeta(name: "appearance");
        }
    }

    internal static JsonElement ReadPack(Func<string, byte[]?> read, string id)
    {
        byte[] bytes = read(arg: "content/packs/" + id + ".json")
                ?? throw new InvalidDataException(message: "CORPSE_PACK_MISSING " + id);
        if (id is not "common")
        {
            return CharacterPreview.ReadDefinition(bytes: bytes, expectedId: id);
        }

        if (bytes.Length is 0 or > 4194304 || bytes[0] == 239)
        {
            throw new InvalidDataException(message: "CORPSE_COMMON_SIZE");
        }

        _ = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: bytes);
        using var document = JsonDocument.Parse(utf8Json: bytes, options: new JsonDocumentOptions { MaxDepth = 32 });
        ValidateCommon(value: document.RootElement);
        JsonElement result = document.RootElement;
        if (result.GetProperty(propertyName: "id").GetString() is not "common"
                || result.GetProperty(propertyName: "schemaVersion").GetInt32() != 1
                || result.GetProperty(propertyName: "version").GetInt32() != 1)
        {
            throw new InvalidDataException(message: "CORPSE_COMMON_ID");
        }

        return result.Clone();
    }

    private static readonly string[] _requiredSources = ["content/packs/common.json", "assets/common/donor-kit.png",
        "content/animation/fits-a.json", "content/animation/fits-b.json", "content/animation/fits-c.json",
        "content/packs/bodies-a.json", "content/packs/bodies-b.json", "content/packs/bodies-c.json",
        "assets/characters/bodies-a.png", "assets/characters/bodies-b.png", "assets/characters/bodies-c.png",
        "content/packs/items-01-10.json", "content/packs/items-11-24.json", "assets/equipment/items-01-10.png",
        "assets/equipment/items-11-24.png"];
    private readonly Node2D _root;
    private readonly MainThreadGateway _gateway;
    private readonly Func<string, byte[]?> _read;
    private readonly JsonElement _definition;
    private readonly JsonElement _common;
    private readonly JsonElement[] _packs;
    private readonly AppearanceState _state;
    private readonly List<Texture2D> _owned = [];
    private CharacterPreview? _character;
    private bool _closed;
    private int _facing = 1;

    private static string MissingText()
    {
        throw new InvalidDataException(message: "CORPSE_STRING");
    }

    private static void ValidateCommon(JsonElement value)
    {
        if (value.ValueKind is JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                if (keys.Count >= 256 || !keys.Add(item: property.Name))
                {
                    throw new InvalidDataException(message: "CORPSE_COMMON_KEYS");
                }

                ValidateCommon(value: property.Value);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            if (value.GetArrayLength() > 4096)
            {
                throw new InvalidDataException(message: "CORPSE_COMMON_COUNT");
            }

            foreach (JsonElement element in value.EnumerateArray())
            {
                ValidateCommon(value: element);
            }
        }
        else if (value.ValueKind is JsonValueKind.Number && !double.IsFinite(d: value.GetDouble())
                || value.ValueKind is JsonValueKind.String && (value.GetString() ?? "").Length > 16384)
        {
            throw new InvalidDataException(message: "CORPSE_COMMON_VALUE");
        }
    }

    private void RequireActive()
    {
        if (_closed)
        {
            throw new InvalidOperationException(message: "CORPSE_CLOSED");
        }
    }

    private void Retire()
    {
        _gateway.RequireThread();
        _character?.CloseComposition();
        _character = null;
        if (_root.HasNode(path: "Visual"))
        {
            Node visual = _root.GetNode(path: "Visual");
            _root.RemoveChild(node: visual);
            visual.QueueFree();
        }

        foreach (Texture2D texture in _owned)
        {
            texture.Dispose();
        }

        _owned.Clear();
    }

    private void Project()
    {
        _gateway.RequireThread();
        Retire();
        JsonElement sample = _state.Snapshot;
        string kind = sample.GetProperty(propertyName: "kind").GetString() ?? MissingText();
        string body = sample.GetProperty(propertyName: "body").GetString() ?? MissingText();
        var visual = new Node2D { Name = "Visual", TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
        _root.AddChild(node: visual);
        JsonElement equipment = sample.GetProperty(propertyName: "equipment");
        if (kind is "alive" or "corpse")
        {
            var owner = new Node2D { Name = "Composition" };
            visual.AddChild(node: owner);
            _character = CharacterPreview.OpenComposition(root: owner, gateway: _gateway, read: _read, body: body);
            JsonElement fitted = JsonSerializer.SerializeToElement(value: equipment.EnumerateObject()
                    .Where(predicate: value => value.Name is not "crown").ToDictionary(
                            keySelector: value => value.Name, elementSelector: value => value.Value.GetString(),
                            comparer: StringComparer.Ordinal));
            _character.RestoreComposition(body: body,
                    pose: sample.GetProperty(propertyName: "pose").GetString() ?? MissingText(),
                    facing: _facing, equipment: fitted);
            if (equipment.TryGetProperty(propertyName: "crown", value: out _))
            {
                _ = CommonSprite(parent: visual, id: "common.s0.7985", name: "Crown", at: new Vector2(x: 24, y: 5));
            }
        }
        else if (kind is "wildlife" or "wildlife-alive")
        {
            Wildlife(visual: visual, body: body, sample: sample);
            Pile(visual: visual, equipment: equipment, meatOnly: true);
        }
        else if (kind is "pile")
        {
            Pile(visual: visual, equipment: equipment, meatOnly: false);
        }

        if (sample.GetProperty(propertyName: "selectable").GetBoolean())
        {
            visual.AddChild(node: new Polygon2D
            {
                Name = "ContactShadow",
                Polygon = [new Vector2(x: 14, y: 46), new Vector2(x: 34, y: 46), new Vector2(x: 36, y: 48),
                    new Vector2(x: 12, y: 48)],
                Color = new Color(r: 0.05f, g: 0.04f, b: 0.06f, a: 0.2f),
                ZIndex = -1
            });
        }

        if (sample.GetProperty(propertyName: "selected").GetBoolean()
                && sample.GetProperty(propertyName: "selectable").GetBoolean())
        {
            Outline(visual: visual);
        }

        _root.SetMeta(name: "appearance", value: sample.GetRawText());
    }

    private void Wildlife(Node2D visual, string body, JsonElement sample)
    {
        _gateway.RequireThread();
        string id = _definition.GetProperty(propertyName: "endpoints").GetProperty(propertyName: body).GetString()
                ?? MissingText();
        if (sample.GetProperty(propertyName: "kind").GetString() is "wildlife-alive")
        {
            id = body is "rabbit" ? "common.r.9243" : "common.r.9140";
        }
        else if (sample.GetProperty(propertyName: "pose").GetString() is not "corpse" && body is "deer")
        {
            JsonElement clip = _definition.GetProperty(propertyName: "wildlifeClips").EnumerateArray().Single(
                    predicate: value => value.GetProperty(propertyName: "name").GetString() is "deer_die");
            id = clip.GetProperty(propertyName: "layers")[0].GetProperty(propertyName: "keys")[
                    sample.GetProperty(propertyName: "pose").GetString() is "collapse-a" ? 0 : 2]
                    .GetProperty(propertyName: "sprite").GetString() ?? MissingText();
        }

        Sprite2D sprite = CommonSprite(parent: visual, id: id, name: "Wildlife", at: new Vector2(x: 24, y: 0));
        JsonElement resource = _common.GetProperty(propertyName: "resources").EnumerateArray().Single(
                predicate: value => StringComparer.Ordinal.Equals(x: value.GetProperty(propertyName: "id")
                        .GetString(), y: id));
        JsonElement offset = resource.GetProperty(propertyName: "registration").GetProperty(
                propertyName: "offsetTopLeftExact");
        sprite.Position += new Vector2(x: _facing * offset[0].GetSingle(), y: offset[1].GetSingle());
        sprite.Scale = new Vector2(x: _facing, y: 1);
    }

    private void Pile(Node2D visual, JsonElement equipment, bool meatOnly)
    {
        _gateway.RequireThread();
        int index = 0;
        foreach (JsonProperty entry in equipment.EnumerateObject())
        {
            if (meatOnly && entry.Name is not "meat")
            {
                continue;
            }

            string id = entry.Value.GetString() ?? MissingText();
            JsonElement pack = _packs.First(predicate: value => value.GetProperty(propertyName: "items")
                    .EnumerateArray().Any(predicate: value => StringComparer.Ordinal.Equals(
                            x: value.GetProperty(propertyName: "id").GetString(), y: id)));
            JsonElement item = pack.GetProperty(propertyName: "items").EnumerateArray().Single(
                    predicate: value => StringComparer.Ordinal.Equals(x: value.GetProperty(propertyName: "id")
                            .GetString(), y: id));
            string key = item.GetProperty(propertyName: "canonicalIcon").GetString() ?? MissingText();
            JsonElement part = pack.GetProperty(propertyName: "parts").EnumerateArray().Single(
                    predicate: value => StringComparer.Ordinal.Equals(x: value.GetProperty(propertyName: "key")
                            .GetString(), y: key));
            Sprite2D sprite = Sprite(parent: visual, pack: pack, part: part, name: "Loot_" + entry.Name,
                    at: new Vector2(x: 16 + index * 3, y: 38 - index % 2 * 3));
            sprite.SetMeta(name: "item", value: id);
            sprite.SetMeta(name: "slot", value: entry.Name);
            index++;
        }
    }

    private Sprite2D CommonSprite(Node2D parent, string id, string name, Vector2 at)
    {
        _gateway.RequireThread();
        JsonElement part = _common.GetProperty(propertyName: "resources").EnumerateArray().Single(
                predicate: value => StringComparer.Ordinal.Equals(x: value.GetProperty(propertyName: "id")
                        .GetString(), y: id));
        Sprite2D result = Sprite(parent: parent, pack: _common, part: part, name: name, at: at);
        result.SetMeta(name: "source", value: id);
        return result;
    }

    private Sprite2D Sprite(Node2D parent, JsonElement pack, JsonElement part, string name, Vector2 at)
    {
        _gateway.RequireThread();
        string path = pack.GetProperty(propertyName: "atlas").GetProperty(propertyName: "path").GetString()
                ?? MissingText();
        Texture2D atlas = ResourceLoader.Load<Texture2D>(path: "res://" + path)
                ?? throw new InvalidDataException(message: "CORPSE_ATLAS_MISSING " + path);
        JsonElement rect = part.GetProperty(propertyName: "atlasRect");
        var texture = new AtlasTexture
        {
            Atlas = atlas,
            Region = new Rect2(x: rect[0].GetSingle(), y: rect[1].GetSingle(), width: rect[2].GetSingle(),
                    height: rect[3].GetSingle()),
            FilterClip = true
        };
        _owned.Add(item: texture);
        var sprite = new Sprite2D { Name = name, Centered = false, Texture = texture, Position = at, ZIndex = 40 };
        parent.AddChild(node: sprite);
        return sprite;
    }

    private void Outline(Node2D visual)
    {
        _gateway.RequireThread();
        using Image mask = Image.CreateEmpty(width: 96, height: 80, useMipmaps: false, format: Image.Format.Rgba8);
        foreach (Sprite2D sprite in visual.FindChildren(pattern: "*", type: "Sprite2D", recursive: true,
                owned: false).OfType<Sprite2D>())
        {
            using Image source = sprite.Texture.GetImage();
            for (int y = 0; y < source.GetHeight(); y++)
            {
                for (int x = 0; x < source.GetWidth(); x++)
                {
                    Vector2 at = sprite.Transform * (new Vector2(x: x + 0.5f, y: y + 0.5f) + sprite.Offset)
                            + new Vector2(x: 24, y: 8);
                    if (source.GetPixel(x: x, y: y).A > 0 && at.X is > 0 and < 95 && at.Y is > 0 and < 79)
                    {
                        mask.SetPixel(x: (int)at.X, y: (int)at.Y, color: Colors.White);
                    }
                }
            }
        }

        using Image outline = Image.CreateEmpty(width: 96, height: 80, useMipmaps: false, format: Image.Format.Rgba8);
        for (int y = 1; y < 79; y++)
        {
            for (int x = 1; x < 95; x++)
            {
                if (mask.GetPixel(x: x, y: y).A == 0 && (mask.GetPixel(x: x - 1, y: y).A > 0
                        || mask.GetPixel(x: x + 1, y: y).A > 0 || mask.GetPixel(x: x, y: y - 1).A > 0
                        || mask.GetPixel(x: x, y: y + 1).A > 0))
                {
                    outline.SetPixel(x: x, y: y, color: new Color(code: "f2e4c9"));
                }
            }
        }

        ImageTexture texture = ImageTexture.CreateFromImage(image: outline);
        _owned.Add(item: texture);
        visual.AddChild(node: new Sprite2D
        {
            Name = "SelectedOutline", Centered = false, Texture = texture, Position = new Vector2(x: -24, y: -8),
            ZIndex = 60
        });
        visual.AddChild(node: new Polygon2D
        {
            Name = "SelectedMarker", Polygon = [new Vector2(x: 22, y: -5), new Vector2(x: 26, y: -5),
                new Vector2(x: 24, y: -2)], Color = new Color(code: "f2e4c9"), ZIndex = 61
        });
    }
}
