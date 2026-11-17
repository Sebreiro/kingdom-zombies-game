using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Owns fitted body and equipment layers without becoming another public Godot script.</summary>
internal sealed class CharacterPreview
{
    internal CharacterPreview(Node2D root, MainThreadGateway gateway, JsonElement fits, JsonElement bodies,
            JsonElement weapons, JsonElement clothing, string body)
    {
        _root = root;
        _gateway = gateway;
        _fits = fits;
        _bodies = bodies;
        _packs = [weapons, clothing];
        _body = body;
    }

    internal static JsonElement ReadDefinition(byte[] bytes, string expectedId)
    {
        if (bytes.Length is 0 or > 1048576 || bytes[0] == 239)
        {
            throw new InvalidDataException(message: "CHARACTER_DEFINITION_SIZE");
        }

        using var document = JsonDocument.Parse(utf8Json: bytes, options: new JsonDocumentOptions { MaxDepth = 24 });
        JsonElement root = document.RootElement;
        ValidateValue(value: root);
        if (root.GetProperty(propertyName: "schemaVersion").GetInt32() != 1
                || root.GetProperty(propertyName: "version").GetInt32() != 1
                || !StringComparer.Ordinal.Equals(x: Text(value: root, name: "id"), y: expectedId))
        {
            throw new InvalidDataException(message: "CHARACTER_DEFINITION_ID");
        }

        if (expectedId.StartsWith(value: "fits-", comparisonType: StringComparison.Ordinal))
        {
            ValidateFitting(root: root);
        }

        return root.Clone();
    }



    internal static CharacterPreview Open(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read,
            string body)
    {
        gateway.RequireThread();
        JsonElement[] profiles = [ReadProfile(read: read, profile: "a"), ReadProfile(read: read, profile: "b"),
            ReadProfile(read: read, profile: "c")];
        JsonElement fits = MergeFits(profiles: profiles);
        JsonElement bodies = MergeBodies(profiles: profiles);
        JsonElement weapons = ReadDefinition(bytes: read(arg: "content/packs/items-01-10.json")
                ?? throw new InvalidDataException(message: "CHARACTER_ITEMS_MISSING"), expectedId: "items-01-10");
        JsonElement clothing = ReadDefinition(bytes: read(arg: "content/packs/items-11-24.json")
                ?? throw new InvalidDataException(message: "CHARACTER_ITEMS_MISSING"), expectedId: "items-11-24");
        return new CharacterPreview(root: root, gateway: gateway, fits: fits, bodies: bodies, weapons: weapons,
                clothing: clothing, body: body);
    }







    internal static JsonElement FitItem(JsonElement fits, string body, string item, string pose, int facing)
    {
        bool hasPose = fits.GetProperty(propertyName: "bodies").EnumerateArray().Any(predicate: candidate =>
                StringComparer.Ordinal.Equals(x: Text(value: candidate, name: "id"), y: body)
                        && candidate.GetProperty(propertyName: "poses").EnumerateArray().Any(predicate: value =>
                                StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: pose)));
        JsonElement result = fits.GetProperty(propertyName: "items").EnumerateArray().FirstOrDefault(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: item));
        if (!hasPose || facing is not (-1 or 1) || result.ValueKind is JsonValueKind.Undefined)
        {
            throw new InvalidDataException(message: "FIT_INCOMPLETE body=" + body + " item=" + item + " pose="
                    + pose + " facing=" + facing.ToString(provider: System.Globalization.CultureInfo.InvariantCulture));
        }

        return result;
    }

    internal void Attach()
    {
        _gateway.RequireThread();
        if (_attached)
        {
            throw new InvalidOperationException(message: "CHARACTER_ALREADY_ATTACHED");
        }

        _root.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        var controls = new ScrollContainer { Name = "Controls", Position = new Vector2(x: 390, y: 0),
                Size = new Vector2(x: 260, y: 320), FollowFocus = true };
        _root.AddChild(node: controls);
        var slots = new VBoxContainer { Name = "Slots", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        controls.AddChild(node: slots);
        int selected = BodyChoice(slots: slots);
        AddSelector(parent: slots, id: "Pose", label: "Authored pose", values: _fits.GetProperty(
                propertyName: "bodies")[0].GetProperty(propertyName: "poses").EnumerateArray()
                .Select(selector: pose => Text(value: pose, name: "id")));
        AddSelector(parent: slots, id: "Facing", label: "Facing", values: ["Right", "Left"]);
        AddSelector(parent: slots, id: "Condition", label: "Equipment condition", values: ["Usable", "Broken"]);
        AddSelector(parent: slots, id: "Zoom", label: "Inspection zoom", values: ["1x", "2x", "4x", "8x"]);
        foreach (JsonElement slot in _fits.GetProperty(propertyName: "slots").EnumerateArray())
        {
            AddSelector(parent: slots, id: Text(value: slot, name: "id"), label: Text(value: slot, name: "label"),
                    values: Empty.Concat(second: slot.GetProperty(propertyName: "items")
                            .EnumerateArray().Select(selector: item => item.GetString()
                                    ?? throw new InvalidDataException(message: "CHARACTER_ITEM_ID"))));
        }

        _root.AddChild(node: new Node2D { Name = "Art", Position = new Vector2(x: 0, y: 0) });
        _root.AddChild(node: new Label { Name = "Status", Position = new Vector2(x: 0, y: 322),
                Size = new Vector2(x: 650, y: 80), AutowrapMode = TextServer.AutowrapMode.WordSmart });
        _controls["Body"].Select(idx: selected);
        _controls["Zoom"].Select(idx: 3);
        _attached = true;
        Project();
    }



    internal void Detach()
    {
        _gateway.RequireThread();
        foreach (OptionButton control in _controls.Values)
        {
            control.ItemSelected -= Changed;
        }

        _attached = false;
    }

    private readonly Node2D _root;
    private static readonly string[] Empty = ["Empty"];
    private readonly MainThreadGateway _gateway;
    private readonly JsonElement _fits;
    private readonly JsonElement _bodies;
    private readonly JsonElement[] _packs;
    private readonly Dictionary<string, OptionButton> _controls = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, Texture2D> _textures = new(comparer: StringComparer.Ordinal);
    private string _body;
    private bool _attached;

    private static void ValidateFitting(JsonElement root)
    {
        foreach (JsonElement body in root.GetProperty(propertyName: "bodies").EnumerateArray())
        {
            JsonElement bodyWidth = body.GetProperty(propertyName: "width");
            if (bodyWidth.ValueKind is not JsonValueKind.Number || !bodyWidth.TryGetInt32(value: out int count)
                    || count is < 2 or > 16)
            {
                throw new InvalidDataException(message: "CHARACTER_FIT_WIDTH body");
            }

            if (body.TryGetProperty(propertyName: "componentWidths", value: out JsonElement widths))
            {
                foreach (JsonProperty width in widths.EnumerateObject())
                {
                    if (width.Value.ValueKind is not JsonValueKind.Number
                            || !width.Value.TryGetInt32(value: out int value) || value is < 1 or > 16)
                    {
                        throw new InvalidDataException(message: "CHARACTER_FIT_WIDTH " + width.Name);
                    }
                }
            }

            foreach (JsonElement pose in body.GetProperty(propertyName: "poses").EnumerateArray())
            {
                if (pose.TryGetProperty(propertyName: "unlinkedRotation", value: out JsonElement rotation))
                {
                    double angle = rotation.GetDouble();
                    double turns = angle / (Math.PI / 2);
                    if (!double.IsFinite(d: angle) || Math.Abs(value: turns) > 2
                            || Math.Abs(value: turns - Math.Round(a: turns)) > 0.000001)
                    {
                        throw new InvalidDataException(message: "CHARACTER_FIT_ROTATION "
                                + Text(value: body, name: "id") + " " + Text(value: pose, name: "id"));
                    }
                }
            }
        }
    }

    private static JsonElement ReadProfile(Func<string, byte[]?> read, string profile)
    {
        JsonElement fits = ReadDefinition(bytes: read(arg: "content/animation/fits-" + profile + ".json")
                ?? throw new InvalidDataException(message: "CHARACTER_FITS_MISSING " + profile),
                expectedId: "fits-" + profile);
        JsonElement bodies = ReadDefinition(bytes: read(arg: "content/packs/bodies-" + profile + ".json")
                ?? throw new InvalidDataException(message: "CHARACTER_BODIES_MISSING " + profile),
                expectedId: "bodies-" + profile);
        string atlas = "res://assets/characters/bodies-" + profile + ".png";
        if (!StringComparer.Ordinal.Equals(x: Text(value: bodies.GetProperty(propertyName: "atlas"),
                name: "path"), y: atlas))
        {
            throw new InvalidDataException(message: "CHARACTER_PROFILE_ATLAS " + profile);
        }

        string[] expected = profile is "a" ? ["CH-01", "CH-02", "CH-03"]
                : profile is "b" ? ["CH-04", "CH-05", "CH-08"] : ["CH-06", "CH-07"];
        string[] actual = bodies.GetProperty(propertyName: "bodies").EnumerateArray()
                .Select(selector: value => Text(value: value, name: "id")).ToArray();
        string[] fitted = fits.GetProperty(propertyName: "bodies").EnumerateArray()
                .Select(selector: value => Text(value: value, name: "id")).ToArray();
        if (!actual.SequenceEqual(second: expected, comparer: StringComparer.Ordinal)
                || !fitted.SequenceEqual(second: expected, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "CHARACTER_PROFILE_BODIES " + profile);
        }

        return JsonSerializer.SerializeToElement(value: new { fits, bodies, atlas });
    }

    private static JsonElement MergeFits(JsonElement[] profiles)
    {
        JsonElement first = profiles[0].GetProperty(propertyName: "fits");
        return JsonSerializer.SerializeToElement(value: new
        {
            bodies = profiles.SelectMany(selector: value => value.GetProperty(propertyName: "fits")
                    .GetProperty(propertyName: "bodies").EnumerateArray()).OrderBy(
                            keySelector: value => Text(value: value, name: "id"), comparer: StringComparer.Ordinal),
            slots = first.GetProperty(propertyName: "slots"), items = first.GetProperty(propertyName: "items"),
        });
    }

    private static JsonElement MergeBodies(JsonElement[] profiles)
    {
        return JsonSerializer.SerializeToElement(value: new
        {
            parts = profiles.SelectMany(selector: value => value.GetProperty(propertyName: "bodies")
                    .GetProperty(propertyName: "parts").EnumerateArray()),
            atlases = profiles.SelectMany(selector: profile => profile.GetProperty(propertyName: "bodies")
                    .GetProperty(propertyName: "bodies").EnumerateArray().Select(selector: body =>
                            new KeyValuePair<string, string>(key: Text(value: body, name: "id"),
                                    value: Text(value: profile, name: "atlas"))))
                    .ToDictionary(keySelector: value => value.Key, elementSelector: value => value.Value,
                            comparer: StringComparer.Ordinal),
        });
    }

    private int BodyChoice(Node slots)
    {
        _gateway.RequireThread();
        string[] bodies = _fits.GetProperty(propertyName: "bodies").EnumerateArray()
                .Select(selector: value => Text(value: value, name: "id")).ToArray();
        int selected = Array.FindIndex(array: bodies, match: value => StringComparer.Ordinal.Equals(
                x: value, y: _body));
        if (selected < 0 || bodies.Distinct(comparer: StringComparer.Ordinal).Count() != bodies.Length)
        {
            throw new InvalidDataException(message: "CHARACTER_BODY_SELECTION " + _body);
        }

        AddSelector(parent: slots, id: "Body", label: "Body", values: bodies);
        return selected;
    }

    private static string Text(JsonElement value, string name)
    {
        return value.GetProperty(propertyName: name).GetString()
                ?? throw new InvalidDataException(message: "CHARACTER_TEXT " + name);
    }

    private static void ValidateValue(JsonElement value)
    {
        if (value.ValueKind is JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                if (!keys.Add(item: property.Name))
                {
                    throw new InvalidDataException(message: "CHARACTER_DUPLICATE_KEY");
                }

                ValidateValue(value: property.Value);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            foreach (JsonElement element in value.EnumerateArray())
            {
                ValidateValue(value: element);
            }
        }
        else if (value.ValueKind is JsonValueKind.Number && !double.IsFinite(d: value.GetDouble()))
        {
            throw new InvalidDataException(message: "CHARACTER_NONFINITE");
        }
    }

    private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)
    {
        _gateway.RequireThread();
        var row = new VBoxContainer { Name = id };
        parent.AddChild(node: row);
        row.AddChild(node: new Label { Name = "Label", Text = label });
        var control = new OptionButton { Name = "Select", CustomMinimumSize = new Vector2(x: 220, y: 48) };
        row.AddChild(node: control);
        foreach (string value in values)
        {
            control.AddItem(label: value);
        }

        control.ItemSelected += Changed;
        _controls.Add(key: id, value: control);
    }

    private void Changed(long index)
    {
        _gateway.RequireThread();
        ArgumentOutOfRangeException.ThrowIfNegative(value: index);
        if (!_attached)
        {
            throw new InvalidOperationException(message: "CHARACTER_DETACHED");
        }

        Project();
    }

    private string Selected(string id)
    {
        _gateway.RequireThread();
        OptionButton control = _controls[id];
        return control.GetItemText(idx: control.Selected);
    }

    private void Project()
    {
        _gateway.RequireThread();
        Node2D art = _root.GetNode<Node2D>(path: "Art");
        foreach (Node child in art.GetChildren())
        {
            art.RemoveChild(node: child);
            child.QueueFree();
        }

        _body = Selected(id: "Body");
        ReconcileHands();
        string poseId = Selected(id: "Pose");
        int facing = Selected(id: "Facing") is "Right" ? 1 : -1;
        int zoom = 1 << _controls["Zoom"].Selected;
        art.Scale = new Vector2(x: zoom, y: zoom);
        JsonElement body = _fits.GetProperty(propertyName: "bodies").EnumerateArray().First(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: _body));
        JsonElement pose = body.GetProperty(propertyName: "poses").EnumerateArray().First(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: poseId));
        AddBody(art: art, pose: pose, facing: facing);
        var diagnostics = new List<string>();
        foreach (JsonElement slot in _fits.GetProperty(propertyName: "slots").EnumerateArray())
        {
            string id = Text(value: slot, name: "id");
            string item = Selected(id: id);
            if (item is not "Empty")
            {
                AddItem(art: art, body: body, pose: pose, slot: id, itemId: item, facing: facing,
                        diagnostics: diagnostics);
            }
        }

        string state = diagnostics.Count == 0 ? "Fitted — visual acceptance pending" : "Incomplete — "
                + string.Join(separator: "; ", values: diagnostics);
        _root.GetNode<Label>(path: "Status").Text = _body + " · " + poseId + " · " + state;
        _root.SetMeta(name: "body", value: _body);
        _root.SetMeta(name: "pose", value: poseId);
        _root.SetMeta(name: "facing", value: facing);
        _root.SetMeta(name: "diagnostics", value: string.Join(separator: "\n", values: diagnostics));
    }

    private void AddBody(Node2D art, JsonElement pose, int facing)
    {
        _gateway.RequireThread();
        foreach (string part in new[] { "lower", "upper", "head" })
        {
            string key = Text(value: pose.GetProperty(propertyName: "bodyParts"), name: part);
            JsonElement definition = _bodies.GetProperty(propertyName: "parts").EnumerateArray()
                    .First(predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "key"),
                            y: key));
            var sprite = new Sprite2D { Name = "Body_" + part, Centered = false,
                    Texture = Texture(pack: _bodies, part: definition, atlas: BodyAtlas()),
                    Position = new Vector2(x: facing == 1 ? 0 : 48, y: 0),
                    Scale = new Vector2(x: facing, y: 1), ZIndex = 10,
                    TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
            sprite.SetMeta(name: "part", value: key);
            art.AddChild(node: sprite);
        }
    }

    private string BodyAtlas()
    {
        _gateway.RequireThread();
        string atlas = _bodies.TryGetProperty(propertyName: "atlases", value: out JsonElement atlases)
                ? Text(value: atlases, name: _body)
                : Text(value: _bodies.GetProperty(propertyName: "atlas"), name: "path");
        string[] allowed = ["res://assets/characters/bodies-a.png", "res://assets/characters/bodies-b.png",
            "res://assets/characters/bodies-c.png"];
        if (!allowed.Contains(value: atlas, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "CHARACTER_PROFILE_ATLAS " + _body);
        }

        return atlas[6..];
    }

    private void AddItem(Node2D art, JsonElement body, JsonElement pose, string slot, string itemId, int facing,
            List<string> diagnostics)
    {
        _gateway.RequireThread();
        JsonElement item;
        try
        {
            item = FitItem(fits: _fits, body: _body, item: itemId, pose: Text(value: pose, name: "id"),
                    facing: facing);
        }
        catch (InvalidDataException error)
        {
            diagnostics.Add(item: error.Message);
            return;
        }

        foreach (JsonElement layer in item.GetProperty(propertyName: "layers").EnumerateArray())
        {
            string surface = Text(value: layer, name: "surface");
            if (surface is "both" || StringComparer.Ordinal.Equals(x: surface,
                    y: facing == 1 ? "front" : "rear"))
            {
                AddLayer(art: art, body: body, pose: pose, item: item, layer: layer, slot: slot, facing: facing);
            }
        }
    }

    private void ReconcileHands()
    {
        _gateway.RequireThread();
        foreach (string slot in new[] { "left", "right" })
        {
            string opposite = Selected(id: slot is "left" ? "right" : "left");
            OptionButton control = _controls[slot];
            for (int index = 0; index < control.ItemCount; index++)
            {
                string item = control.GetItemText(idx: index);
                bool disabled = item is not "Empty" && (opposite is "IT-07" && item is not "IT-02"
                        || item is "IT-07" && opposite is not ("Empty" or "IT-02"));
                control.SetItemDisabled(idx: index, disabled: disabled);
                control.SetItemTooltip(idx: index, tooltip: disabled
                        ? "Bow permits only arrows in the other hand" : "");
            }
        }
    }

    private void AddLayer(Node2D art, JsonElement body, JsonElement pose, JsonElement item, JsonElement layer,
            string slot, int facing)
    {
        _gateway.RequireThread();
        string anchor = Text(value: layer, name: "anchor");
        anchor = anchor is "hand" ? slot + "Hand" : anchor;
        JsonElement anchors = pose.GetProperty(propertyName: "anchors");
        Vector2 at = Point(value: anchors.GetProperty(propertyName: anchor));
        (Vector2 size, float angle) = FitGeometry(body: body, pose: pose, layer: layer, slot: slot,
                anchor: anchor, anchors: anchors, at: at);
        JsonElement pack = _packs.First(predicate: value => value.GetProperty(propertyName: "items")
                .EnumerateArray().Any(predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value,
                        name: "id"), y: Text(value: item, name: "id"))));
        string key = Text(value: layer, name: "part");
        bool broken = Selected(id: "Condition") is "Broken"
                && item.GetProperty(propertyName: "broken").ValueKind is JsonValueKind.String;
        if (broken && (slot is "left" or "right" or "headwear" || anchor is "torso"))
        {
            key = Text(value: item, name: "broken");
        }

        JsonElement definition = pack.GetProperty(propertyName: "parts").EnumerateArray().First(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "key"), y: key));
        AddFittedSprite(art: art, item: item, layer: layer, slot: slot, facing: facing, at: at, size: size,
                angle: angle, pack: pack, definition: definition, key: key);
    }

    private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,
            string slot, string anchor, JsonElement anchors, Vector2 at)
    {
        _gateway.RequireThread();
        Vector2 size = Point(value: layer.GetProperty(propertyName: "size"));
        float angle = 0;
        if (body.TryGetProperty(propertyName: "componentWidths", value: out JsonElement widths)
                && widths.TryGetProperty(propertyName: Text(value: layer, name: "component"),
                        value: out JsonElement width))
        {
            size.X = width.GetInt32();
        }

        if (layer.GetProperty(propertyName: "end").ValueKind is JsonValueKind.String)
        {
            Vector2 delta = Point(value: anchors.GetProperty(propertyName: Text(value: layer, name: "end"))) - at;
            size.Y = Math.Max(val1: 1, val2: MathF.Round(x: delta.Length()));
            angle = delta.Angle() - MathF.PI / 2;
        }
        else if (anchor is "torso")
        {
            size.X = body.GetProperty(propertyName: "width").GetInt32() + (slot is "armor" ? 2 : 0);
            size.Y = pose.GetProperty(propertyName: "torsoLength").GetInt32();
            angle = (Point(value: anchors.GetProperty(propertyName: "hips")) - at).Angle() - MathF.PI / 2;
        }
        else if (slot is not ("left" or "right") && pose.TryGetProperty(propertyName: "unlinkedRotation",
                value: out JsonElement rotation))
        {
            angle = rotation.GetSingle();
        }

        return (size, angle);
    }

    private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,
            Vector2 at, Vector2 size, float angle, JsonElement pack, JsonElement definition, string key)
    {
        _gateway.RequireThread();
        Texture2D source = Texture(pack: pack, part: definition, atlas: Text(value: item, name: "atlas"));
        Image image = source.GetImage();
        image.Resize(width: (int)size.X, height: (int)size.Y, interpolation: Image.Interpolation.Nearest);
        bool hand = slot is "left" or "right";
        Vector2 pivot = hand ? Point(value: layer.GetProperty(propertyName: "pivot"))
                : new Vector2(x: size.X / 2, y: slot is "headwear" or "face" ? size.Y / 2 : 0);
        var sprite = new Sprite2D { Name = slot + "_" + Text(value: layer, name: "component"), Centered = false,
                Texture = ImageTexture.CreateFromImage(image: image), Offset = -pivot.Round(),
                Position = new Vector2(x: facing == 1 ? at.X : 48 - at.X, y: at.Y),
                Scale = new Vector2(x: facing, y: 1), Rotation = facing * angle,
                ZIndex = LayerOrder(layer: layer, slot: slot, facing: facing),
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
        sprite.SetMeta(name: "slot", value: slot);
        sprite.SetMeta(name: "item", value: Text(value: item, name: "id"));
        sprite.SetMeta(name: "part", value: key);
        sprite.SetMeta(name: "logical_hand", value: hand ? slot : "none");
        art.AddChild(node: sprite);
    }

    private int LayerOrder(JsonElement layer, string slot, int facing)
    {
        _gateway.RequireThread();
        string side = slot is "left" or "right" ? slot : Text(value: layer, name: "side");
        bool far = side is "left" or "right" && !StringComparer.Ordinal.Equals(x: side,
                y: facing == 1 ? "right" : "left");
        return far && (slot is "left" or "right" or "gloves" || Text(value: layer,
                name: "component").StartsWith(value: "sleeve", comparisonType: StringComparison.Ordinal))
                ? 8 : layer.GetProperty(propertyName: "z").GetInt32();
    }

    private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)
    {
        _gateway.RequireThread();
        _ = pack;
        if (!_textures.TryGetValue(key: atlas, value: out Texture2D? texture))
        {
            texture = ResourceLoader.Load<Texture2D>(path: "res://" + atlas)
                    ?? throw new InvalidDataException(message: "CHARACTER_ATLAS_MISSING " + atlas);
            _textures.Add(key: atlas, value: texture);
        }

        JsonElement rect = part.GetProperty(propertyName: "atlasRect");
        int x = rect[0].GetInt32();
        int y = rect[1].GetInt32();
        int width = rect[2].GetInt32();
        int height = rect[3].GetInt32();
        if (x < 0 || y < 0 || width <= 0 || height <= 0 || x + width > texture.GetWidth()
                || y + height > texture.GetHeight())
        {
            throw new InvalidDataException(message: "CHARACTER_ATLAS_BOUNDS");
        }

        return new AtlasTexture { Atlas = texture, Region = new Rect2(x: x, y: y, width: width, height: height),
                FilterClip = true };
    }

    private Vector2 Point(JsonElement value)
    {
        _gateway.RequireThread();
        return new Vector2(x: value[0].GetSingle(), y: value[1].GetSingle());
    }
}
