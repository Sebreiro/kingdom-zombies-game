using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Inspection;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Loads only the tracked inventory theme, item definitions and authored icon atlases.</summary>
internal sealed class FoundationInventoryArt : IDisposable
{
    public void Dispose()
    {
        _gateway.RequireThread();
        foreach (AtlasTexture texture in _icons.Values)
        {
            texture.Dispose();
        }

        _icons.Clear();
    }

    internal FoundationInventoryArt(MainThreadGateway gateway)
    {
        gateway.RequireThread();
        _gateway = gateway;
        Theme = ResourceLoader.Load<Theme>(path: "res://assets/ui/InspectionTheme.tres")
                ?? throw new InvalidDataException(message: "INVENTORY_THEME");
        JsonElement definitions = Read(path: "content/config/items.json").GetProperty(propertyName: "definitions");
        _names = definitions.EnumerateArray().ToDictionary(keySelector: item =>
                item.GetProperty(propertyName: "id").GetString() ?? "", elementSelector: item =>
                        item.GetProperty(propertyName: "name").GetString() ?? "", comparer: StringComparer.Ordinal);
    }

    internal Theme Theme { get; }

    internal string Name(FoundationInventoryInteraction.Item item)
    {
        return _names.GetValueOrDefault(key: item.Definition, defaultValue: item.Definition);
    }

    internal Texture2D Icon(FoundationInventoryInteraction.Item item)
    {
        _gateway.RequireThread();
        if (item.Definition.Length != 5 || !item.Definition.StartsWith(value: "IT-", comparisonType:
                StringComparison.Ordinal) || !int.TryParse(s: item.Definition.AsSpan(start: 3),
                        provider: CultureInfo.InvariantCulture, result: out int id) || id is < 1 or > 24)
        {
            throw new InvalidDataException(message: "INVENTORY_ICON_ID");
        }

        string group = id <= 10 ? "items-01-10" : "items-11-24";
        return Atlas(pack: group, key: item.Icon, expected: "assets/equipment/" + group + ".png");
    }

    internal Texture2D Silhouette()
    {
        _gateway.RequireThread();
        return Atlas(pack: "bodies-a", key: "CH-01.idle.full", expected: "assets/characters/bodies-a.png");
    }

    private readonly MainThreadGateway _gateway;
    private readonly Dictionary<string, string> _names;
    private readonly Dictionary<string, AtlasTexture> _icons = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, JsonElement> _packs = new(comparer: StringComparer.Ordinal);

    private AtlasTexture Atlas(string pack, string key, string expected)
    {
        _gateway.RequireThread();
        if (_icons.TryGetValue(key: key, value: out AtlasTexture? existing))
        {
            return existing;
        }

        if (!_packs.TryGetValue(key: pack, value: out JsonElement definition))
        {
            definition = Read(path: "content/packs/" + pack + ".json");
            _packs.Add(key: pack, value: definition);
        }

        JsonElement atlas = definition.GetProperty(propertyName: "atlas");
        string expectedReference = pack is "bodies-a" ? "res://" + expected : expected;
        if (!StringComparer.Ordinal.Equals(x: atlas.GetProperty(propertyName: "path").GetString(),
                y: expectedReference))
        {
            throw new InvalidDataException(message: "INVENTORY_ICON_ATLAS");
        }

        JsonElement row = definition.GetProperty(propertyName: "parts").EnumerateArray().Single(predicate: item =>
                StringComparer.Ordinal.Equals(x: item.GetProperty(propertyName: "key").GetString(), y: key));
        JsonElement layout = row.GetProperty(propertyName: "atlasRect");
        Texture2D sheet = ResourceLoader.Load<Texture2D>(path: "res://" + expected)
                ?? throw new InvalidDataException(message: "INVENTORY_ICON_TEXTURE");
        var rectangle = new Rect2(x: layout[0].GetSingle(), y: layout[1].GetSingle(), width: layout[2].GetSingle(),
                height: layout[3].GetSingle());
        if (rectangle.Position.X < 0 || rectangle.Position.Y < 0 || rectangle.Size.X <= 0 || rectangle.Size.Y <= 0
                || rectangle.End.X > sheet.GetWidth() || rectangle.End.Y > sheet.GetHeight())
        {
            throw new InvalidDataException(message: "INVENTORY_ICON_REGION");
        }

        var icon = new AtlasTexture { Atlas = sheet, Region = rectangle, FilterClip = true };
        _icons.Add(key: key, value: icon);
        return icon;
    }

    private JsonElement Read(string path)
    {
        _gateway.RequireThread();
        using Godot.FileAccess? file = Godot.FileAccess.Open(path: "res://" + path,
                flags: Godot.FileAccess.ModeFlags.Read);
        ulong length = file?.GetLength() ?? 0;
        if (file is null || length is 0 or > 1048576)
        {
            throw new InvalidDataException(message: "INVENTORY_METADATA " + path);
        }

        byte[] bytes = file.GetBuffer(length: checked((long)length));
        if ((ulong)bytes.Length != length)
        {
            throw new InvalidDataException(message: "INVENTORY_METADATA_READ " + path);
        }

        return path is "content/config/items.json" ? JsonSerializer.Deserialize<JsonElement>(utf8Json: bytes)
                : CharacterPreview.ReadDefinition(bytes: bytes, expectedId: Path.GetFileNameWithoutExtension(
                        path: path));
    }
}
