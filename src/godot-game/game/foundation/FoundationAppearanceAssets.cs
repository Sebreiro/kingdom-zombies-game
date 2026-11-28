using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Inspection;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Resolves the tracked Phase 4 metadata and exact imported gameplay atlas references.</summary>
internal sealed class FoundationAppearanceAssets : IDisposable
{
    public void Dispose()
    {
        _gateway.RequireThread();
        Crown.Dispose();
        Arrow.Dispose();
        _metadata.Clear();
    }

    internal FoundationAppearanceAssets(MainThreadGateway gateway)
    {
        _gateway = gateway;
        _gateway.RequireThread();
        byte[] corpse = Required(path: "content/packs/corpses-wildlife.json");
        _death = CharacterPreview.ReadDefinition(bytes: corpse, expectedId: "corpses-wildlife");
        JsonElement common = CorpsePreview.ReadPack(read: Read, id: "common");
        JsonElement tools = CharacterPreview.ReadDefinition(bytes: Required(path: "content/packs/items-01-10.json"),
                expectedId: "items-01-10");
        Crown = SpriteTexture(pack: common, rows: "resources", identity: "id",
                key: "common.s0.7985", expectedPath: "assets/common/donor-kit.png");
        Arrow = SpriteTexture(pack: tools, rows: "parts", identity: "key",
                key: "IT_02_master", expectedPath: "assets/equipment/items-01-10.png");
    }

    internal AtlasTexture Crown { get; }
    internal AtlasTexture Arrow { get; }
    internal double CollapseMilliseconds => _death.GetProperty(propertyName: "collapseSeconds").GetDouble() * 1000;

    internal AppearanceState State()
    {
        _gateway.RequireThread();
        return new AppearanceState(definition: Required(path: "content/packs/corpses-wildlife.json"));
    }

    internal CharacterPresentation Presentation()
    {
        _gateway.RequireThread();
        return new CharacterPresentation(animation: Required(path: "content/animation/locomotion-melee.json"),
                items: Required(path: "content/config/items.json"),
                combat: Required(path: "content/config/combat.json"),
                bow: Required(path: "content/animation/bow.json"));
    }

    internal byte[]? Read(string path)
    {
        _gateway.RequireThread();
        if (!_expected.Contains(item: path))
        {
            throw new InvalidDataException(message: "RUN_APPEARANCE_RESOURCE " + path);
        }

        if (_metadata.TryGetValue(key: path, value: out byte[]? previous))
        {
            return previous;
        }

        using Godot.FileAccess? file = Godot.FileAccess.Open(path: "res://" + path,
                flags: Godot.FileAccess.ModeFlags.Read);
        ulong length = file?.GetLength() ?? 0;
        if (file is null || length is 0 or > 2097152)
        {
            throw new InvalidDataException(message: "RUN_APPEARANCE_RESOURCE_SIZE " + path);
        }

        byte[] bytes = file.GetBuffer(length: checked((long)length));
        if ((ulong)bytes.Length != length || file.GetError() is not (Error.Ok or Error.FileEof))
        {
            throw new InvalidDataException(message: "RUN_APPEARANCE_RESOURCE_READ " + path);
        }

        _metadata.Add(key: path, value: bytes);
        return bytes;
    }

    private static readonly HashSet<string> _expected = new(collection:
            ["content/animation/fits-a.json", "content/animation/fits-b.json", "content/animation/fits-c.json",
                "content/packs/bodies-a.json", "content/packs/bodies-b.json", "content/packs/bodies-c.json",
                "content/packs/items-01-10.json", "content/packs/items-11-24.json", "content/packs/common.json",
                "content/packs/corpses-wildlife.json", "content/animation/locomotion-melee.json",
                "content/config/items.json", "content/config/combat.json", "content/animation/bow.json"],
            comparer: StringComparer.Ordinal);
    private readonly MainThreadGateway _gateway;
    private readonly Dictionary<string, byte[]> _metadata = new(comparer: StringComparer.Ordinal);
    private readonly JsonElement _death;

    private byte[] Required(string path)
    {
        return Read(path: path) ?? throw new InvalidDataException(message: "RUN_APPEARANCE_RESOURCE_MISSING " + path);
    }

    private AtlasTexture SpriteTexture(JsonElement pack, string rows, string identity, string key, string expectedPath)
    {
        _gateway.RequireThread();
        string? path = pack.GetProperty(propertyName: "atlas").GetProperty(propertyName: "path").GetString();
        if (!StringComparer.Ordinal.Equals(x: path, y: expectedPath))
        {
            throw new InvalidDataException(message: "RUN_APPEARANCE_ATLAS " + key);
        }

        JsonElement resource = pack.GetProperty(propertyName: rows).EnumerateArray().Single(predicate: part =>
                StringComparer.Ordinal.Equals(x: part.GetProperty(propertyName: identity).GetString(), y: key));
        JsonElement rect = resource.GetProperty(propertyName: "atlasRect");
        Texture2D texture = ResourceLoader.Load<Texture2D>(path: "res://" + expectedPath)
                ?? throw new InvalidDataException(message: "RUN_APPEARANCE_ATLAS " + expectedPath);
        int x = rect[0].GetInt32();
        int y = rect[1].GetInt32();
        int width = rect[2].GetInt32();
        int height = rect[3].GetInt32();
        if (x < 0 || y < 0 || width <= 0 || height <= 0 || x > texture.GetWidth() - width
                || y > texture.GetHeight() - height)
        {
            throw new InvalidDataException(message: "RUN_APPEARANCE_ATLAS_REGION " + key);
        }

        return new AtlasTexture
        {
            Atlas = texture,
            Region = new Rect2(x: x, y: y, width: width, height: height),
            FilterClip = true,
        };
    }
}
