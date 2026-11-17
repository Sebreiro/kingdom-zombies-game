using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Projects validated resource fixtures into reusable native identity controls.</summary>
internal sealed class IdentityKit
{
    internal IdentityKit(Control root, MainThreadGateway gateway, byte[] identity, byte[] definitions,
            byte[] weapons, byte[] clothing)
    {
        _root = root;
        _gateway = gateway;
        _identity = Read(bytes: identity);
        _definitions = Read(bytes: definitions).GetProperty(propertyName: "definitions").Clone();
        _packs = [Read(bytes: weapons), Read(bytes: clothing)];
        ValidateDefinitions();
    }

    /// <summary>Reads a detached bounded strict document without touching native state.</summary>
    internal static JsonElement Read(byte[] bytes)
    {
        if (bytes.Length is 0 or > 1048576 || bytes[0] == 239)
        {
            throw new InvalidDataException(message: "IDENTITY_DOCUMENT_SIZE");
        }

        using var document = JsonDocument.Parse(utf8Json: bytes, options: new JsonDocumentOptions { MaxDepth = 24 });
        int count = 0;
        ValidateValue(value: document.RootElement, count: ref count);
        return document.RootElement.Clone();
    }

    /// <summary>Installs a complete current snapshot only after every row is validated.</summary>
    internal void Project(byte[] snapshot)
    {
        _gateway.RequireThread();
        RequireActive();
        JsonElement state = Read(bytes: snapshot);
        ValidateSnapshot(state: state);
        Clear();
        foreach (JsonElement binding in state.GetProperty(propertyName: "bindings").EnumerateArray())
        {
            ProjectBinding(binding: binding, all: state.GetProperty(propertyName: "bindings"));
        }

        foreach (JsonElement status in state.GetProperty(propertyName: "statuses").EnumerateArray())
        {
            ProjectStatus(status: status);
        }

        foreach (JsonElement player in state.GetProperty(propertyName: "players").EnumerateArray())
        {
            ProjectPlayer(player: player);
        }

        foreach (JsonElement condition in state.GetProperty(propertyName: "conditions").EnumerateArray())
        {
            ProjectCondition(condition: condition);
        }
    }

    /// <summary>Closes acceptance before retiring owned projection rows.</summary>
    internal void Close()
    {
        _gateway.RequireThread();
        _accepting = false;
        Clear();
    }

    private static readonly string[] PlayerCues = ["number", "shape", "color"];
    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly JsonElement _identity;
    private readonly JsonElement _definitions;
    private readonly JsonElement[] _packs;
    private readonly List<AtlasTexture> _textures = [];
    private bool _accepting = true;

    private static void ValidateValue(JsonElement value, ref int count)
    {
        if (++count > 16000)
        {
            throw new InvalidDataException(message: "IDENTITY_DOCUMENT_COUNT");
        }

        if (value.ValueKind == JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                if (!keys.Add(item: property.Name) || property.Name is "__proto__" or "constructor" or "prototype")
                {
                    throw new InvalidDataException(message: "IDENTITY_DOCUMENT_KEY");
                }

                ValidateValue(value: property.Value, count: ref count);
            }
        }
        else if (value.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement child in value.EnumerateArray())
            {
                ValidateValue(value: child, count: ref count);
            }
        }
        else if (value.ValueKind == JsonValueKind.String)
        {
            string text = value.GetString() ?? throw new InvalidDataException(message: "IDENTITY_DOCUMENT_TEXT");
            if (text.Length > 4096 || !text.IsNormalized(normalizationForm: NormalizationForm.FormC)
                    || text.Any(predicate: character => char.IsControl(c: character)))
            {
                throw new InvalidDataException(message: "IDENTITY_DOCUMENT_TEXT");
            }
        }
        else if (value.ValueKind == JsonValueKind.Number && !double.IsFinite(d: value.GetDouble()))
        {
            throw new InvalidDataException(message: "IDENTITY_DOCUMENT_NUMBER");
        }
    }

    private static string Text(JsonElement value, string name)
    {
        string text = value.GetProperty(propertyName: name).GetString()
                ?? throw new InvalidDataException(message: "IDENTITY_TEXT");
        if (text.Length is 0 or > 256)
        {
            throw new InvalidDataException(message: "IDENTITY_TEXT");
        }

        return text;
    }

    private void ValidateDefinitions()
    {
        if (!Same(left: Text(value: _identity, name: "id"), right: "ui-identity")
                || _definitions.GetArrayLength() != 24)
        {
            throw new InvalidDataException(message: "IDENTITY_DEFINITIONS");
        }

        for (int index = 0; index < 24; index++)
        {
            string id = "IT-" + (index + 1).ToString(format: "D2", provider: CultureInfo.InvariantCulture);
            JsonElement definition = Find(values: _definitions, id: id);
            JsonElement row = Find(values: _identity.GetProperty(propertyName: "items"), id: id);
            JsonElement item = Find(values: _packs[index < 10 ? 0 : 1].GetProperty(propertyName: "items"), id: id);
            string icon = Text(value: item, name: "canonicalIcon");
            if (!Same(left: Text(value: row, name: "icon"), right: icon)
                    || !Same(left: Text(value: definition.GetProperty(propertyName: "appearance"),
                            name: "canonicalIcon"), right: icon)
                    || !Same(left: definition.GetProperty(propertyName: "conditionMaximum").ToString(),
                            right: row.GetProperty(propertyName: "maximum").ToString()))
            {
                throw new InvalidDataException(message: "IDENTITY_CANONICAL_ITEM " + id);
            }

            JsonElement part = _packs[index < 10 ? 0 : 1].GetProperty(propertyName: "parts").EnumerateArray()
                    .Single(predicate: value => Same(left: Text(value: value, name: "key"), right: icon));
            if (!part.GetProperty(propertyName: "atlasRect").ToString()
                    .Equals(value: row.GetProperty(propertyName: "atlasRect").ToString(),
                            comparisonType: StringComparison.Ordinal))
            {
                throw new InvalidDataException(message: "IDENTITY_ITEM_REGION " + id);
            }
        }
    }

    private static JsonElement Find(JsonElement values, string id)
    {
        return values.EnumerateArray().Single(predicate: value =>
                Same(left: Text(value: value, name: "id"), right: id));
    }

    private void ValidateSnapshot(JsonElement state)
    {
        string[] keys = ["bindings", "statuses", "players", "conditions"];
        if (!state.EnumerateObject().Select(selector: value => value.Name).Order(comparer: StringComparer.Ordinal)
                .SequenceEqual(second: keys.Order(comparer: StringComparer.Ordinal), comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "IDENTITY_SNAPSHOT_FIELDS");
        }

        foreach (string key in keys)
        {
            JsonElement rows = state.GetProperty(propertyName: key);
            int expected = key is "players" ? 4 : key is "conditions" ? 24 : key is "statuses" ? 18 : 4;
            if (rows.GetArrayLength() != expected || rows.EnumerateArray().Select(
                    selector: value => Text(value: value, name: "id")).Distinct(comparer: StringComparer.Ordinal)
                    .Count() != expected)
            {
                throw new InvalidDataException(message: "IDENTITY_SNAPSHOT_IDS " + key);
            }
        }

        ValidateBindings(bindings: state.GetProperty(propertyName: "bindings"));
        ValidatePlayers(players: state.GetProperty(propertyName: "players"));
        foreach (JsonElement status in state.GetProperty(propertyName: "statuses").EnumerateArray())
        {
            Find(values: _identity.GetProperty(propertyName: "statuses"), id: Text(value: status, name: "id"));
            Text(value: status, name: "value");
        }

        foreach (JsonElement condition in state.GetProperty(propertyName: "conditions").EnumerateArray())
        {
            JsonElement definition = Find(values: _definitions, id: Text(value: condition, name: "id"));
            JsonElement maximum = definition.GetProperty(propertyName: "conditionMaximum");
            JsonElement current = condition.GetProperty(propertyName: "current");
            if (maximum.ValueKind == JsonValueKind.Null ? current.ValueKind != JsonValueKind.Null
                    : !current.TryGetInt32(value: out int number) || number < 0 || number > maximum.GetInt32())
            {
                throw new InvalidDataException(message: "IDENTITY_CONDITION_RANGE");
            }
        }
    }

    private void ValidateBindings(JsonElement bindings)
    {
        foreach (JsonElement binding in bindings.EnumerateArray())
        {
            Find(values: _identity.GetProperty(propertyName: "bindings"), id: Text(value: binding, name: "id"));
            Text(value: binding, name: "label");
            Text(value: binding, name: "context");
            binding.GetProperty(propertyName: "remapped").GetBoolean();
            JsonElement events = binding.GetProperty(propertyName: "events");
            if (events.GetArrayLength() > 8)
            {
                throw new InvalidDataException(message: "IDENTITY_BINDING_COUNT");
            }

            foreach (JsonElement input in events.EnumerateArray())
            {
                EventText(input: input);
            }
        }
    }

    private void ValidatePlayers(JsonElement players)
    {
        foreach (JsonElement player in players.EnumerateArray())
        {
            JsonElement identity = Find(values: _identity.GetProperty(propertyName: "players"),
                    id: Text(value: player, name: "id"));
            Text(value: player, name: "name");
            if (PlayerCues.Any(predicate: key =>
                    !Same(left: Text(value: identity, name: key), right: Text(value: player, name: key)))
                    || !_identity.GetProperty(propertyName: "playerStates").EnumerateArray().Any(
                            predicate: value => Same(left: value.GetString() ?? "",
                                    right: Text(value: player, name: "state"))))
            {
                throw new InvalidDataException(message: "IDENTITY_PLAYER_CUES");
            }
        }
    }

    private static string EventText(JsonElement input)
    {
        string kind = Text(value: input, name: "kind");
        string key = Text(value: input, name: "key");
        string[] modifiers = input.GetProperty(propertyName: "modifiers").EnumerateArray()
                .Select(selector: value => value.GetString()
                        ?? throw new InvalidDataException(message: "IDENTITY_BINDING_EVENT")).ToArray();
        string[] order = ["Ctrl", "Alt", "Shift", "Meta"];
        if (kind is not ("key" or "mouse") || modifiers.Length > 4
                || modifiers.Distinct(comparer: StringComparer.Ordinal).Count() != modifiers.Length
                || modifiers.Any(predicate: value => !order.Contains(value: value, comparer: StringComparer.Ordinal)))
        {
            throw new InvalidDataException(message: "IDENTITY_BINDING_EVENT");
        }

        return string.Join(separator: " + ", values: order.Where(predicate: value => modifiers.Contains(
                value: value, comparer: StringComparer.Ordinal))
                .Append(element: kind is "mouse" ? "Mouse " + key : key));
    }

    private void RequireActive()
    {
        _gateway.RequireThread();
        if (!_accepting || !_root.IsInsideTree())
        {
            throw new InvalidOperationException(message: "IDENTITY_NOT_ACTIVE");
        }
    }

    private HBoxContainer Row(string group, string id, string label)
    {
        _gateway.RequireThread();
        var row = new HBoxContainer { Name = id };
        _root.GetNode(path: "Scroll/Content/" + group).AddChild(node: row);
        row.SetMeta(name: "resource_id", value: id);
        AddLabel(parent: row, name: "Identity", text: id + " · " + label);
        return row;
    }

    private void AddLabel(Node parent, string name, string text)
    {
        _gateway.RequireThread();
        parent.AddChild(node: new Label { Name = name, Text = text,
                AutowrapMode = TextServer.AutowrapMode.WordSmart, SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });
    }

    private void ProjectBinding(JsonElement binding, JsonElement all)
    {
        _gateway.RequireThread();
        string id = Text(value: binding, name: "id");
        string[] events = binding.GetProperty(propertyName: "events").EnumerateArray()
                .Select(selector: input => EventText(input: input)).ToArray();
        string[] conflicts = all.EnumerateArray().Where(predicate: other =>
                !Same(left: Text(value: other, name: "id"), right: id)
                        && Same(left: Text(value: other, name: "context"), right: Text(value: binding, name: "context"))
                        && other.GetProperty(propertyName: "events").EnumerateArray().Any(predicate: input =>
                                events.Contains(value: EventText(input: input), comparer: StringComparer.Ordinal)))
                .Select(selector: other => Text(value: other, name: "id"))
                .Order(comparer: StringComparer.Ordinal).ToArray();
        HBoxContainer row = Row(group: "Bindings", id: id, label: Text(value: binding, name: "label"));
        row.AddChild(node: new Button { Name = "Keycap", Text = events.Length == 0 ? "Unbound"
                : string.Join(separator: " / ", value: events), MouseFilter = Control.MouseFilterEnum.Ignore });
        AddLabel(parent: row, name: "State", text: conflicts.Length > 0 ? "Conflict · "
                + string.Join(separator: ", ", value: conflicts) : events.Length == 0 ? "Unbound"
                        : binding.GetProperty(propertyName: "remapped").GetBoolean() ? "Remapped" : "Current");
    }

    private void ProjectStatus(JsonElement status)
    {
        _gateway.RequireThread();
        string id = Text(value: status, name: "id");
        JsonElement definition = Find(values: _identity.GetProperty(propertyName: "statuses"), id: id);
        string[] glyphs = ["+", "~", "#", "$", ">", "/", "X", "L", "!", "!!", "o", "v", "...", "%", "R", "*", "S", "="];
        HBoxContainer row = Row(group: "Statuses", id: id, label: Text(value: definition, name: "label"));
        AddLabel(parent: row, name: "Glyph", text: glyphs[definition.GetProperty(propertyName: "glyph").GetInt32()]);
        AddLabel(parent: row, name: "Value", text: Text(value: status, name: "value"));
    }

    private void ProjectPlayer(JsonElement player)
    {
        _gateway.RequireThread();
        HBoxContainer row = Row(group: "Players", id: Text(value: player, name: "id"),
                label: Text(value: player, name: "number") + " · " + Text(value: player, name: "name"));
        var marker = new Control { Name = "Marker", CustomMinimumSize = new Vector2(x: 32, y: 32) };
        row.AddChild(node: marker);
        Vector2[] points = Text(value: player, name: "shape") switch
        {
            "square" => [new(x: 4, y: 4), new(x: 28, y: 4), new(x: 28, y: 28), new(x: 4, y: 28)],
            "triangle" => [new(x: 16, y: 4), new(x: 28, y: 28), new(x: 4, y: 28)],
            "diamond" => [new(x: 16, y: 2), new(x: 30, y: 16), new(x: 16, y: 30), new(x: 2, y: 16)],
            _ => [new(x: 12, y: 4), new(x: 20, y: 4), new(x: 20, y: 12), new(x: 28, y: 12),
                new(x: 28, y: 20), new(x: 20, y: 20), new(x: 20, y: 28), new(x: 12, y: 28),
                new(x: 12, y: 20), new(x: 4, y: 20), new(x: 4, y: 12), new(x: 12, y: 12)]
        };
        marker.AddChild(node: new Polygon2D { Name = "Shape", Polygon = points,
                Color = Color.FromHtml(rgba: Text(value: player, name: "color")), Antialiased = false });
        AddLabel(parent: row, name: "State", text: Text(value: player, name: "state"));
    }

    private void ProjectCondition(JsonElement condition)
    {
        _gateway.RequireThread();
        string id = Text(value: condition, name: "id");
        JsonElement definition = Find(values: _identity.GetProperty(propertyName: "items"), id: id);
        HBoxContainer row = Row(group: "Items", id: id, label: Text(value: definition, name: "name"));
        int[] rect = definition.GetProperty(propertyName: "atlasRect").EnumerateArray()
                .Select(selector: value => value.GetInt32()).ToArray();
        var texture = new AtlasTexture { Atlas = ResourceLoader.Load<Texture2D>(path: "res://"
                + Text(value: definition, name: "texture")), Region = new Rect2(x: rect[0], y: rect[1],
                        width: rect[2], height: rect[3]), FilterClip = true };
        _textures.Add(item: texture);
        row.AddChild(node: new TextureRect { Name = "Icon", Texture = texture,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest, CustomMinimumSize = new Vector2(x: 32, y: 32),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered });
        JsonElement maximum = definition.GetProperty(propertyName: "maximum");
        JsonElement current = condition.GetProperty(propertyName: "current");
        string text = maximum.ValueKind == JsonValueKind.Null ? "" : current.GetInt32()
                .ToString(provider: CultureInfo.InvariantCulture) + " / " + maximum.GetInt32()
                        .ToString(provider: CultureInfo.InvariantCulture);
        AddLabel(parent: row, name: "Condition", text: text);
        AddLabel(parent: row, name: "Broken", text: maximum.ValueKind != JsonValueKind.Null
                && current.GetInt32() == 0 ? "X · Broken" : "");
    }

    private void Clear()
    {
        _gateway.RequireThread();
        foreach (string group in new[] { "Bindings", "Statuses", "Players", "Items" })
        {
            Node container = _root.GetNode(path: "Scroll/Content/" + group);
            foreach (Node child in container.GetChildren())
            {
                container.RemoveChild(node: child);
                child.Free();
            }
        }

        foreach (AtlasTexture texture in _textures)
        {
            texture.Dispose();
        }

        _textures.Clear();
    }

    private static bool Same(string left, string right)
    {
        return StringComparer.Ordinal.Equals(x: left, y: right);
    }

}
