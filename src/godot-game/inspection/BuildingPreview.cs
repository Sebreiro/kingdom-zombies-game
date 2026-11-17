using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Projects detached main-building sample state into reusable inspection resources.</summary>
internal sealed class BuildingPreview
{
    internal BuildingPreview(Node2D root, MainThreadGateway gateway, JsonElement art, JsonElement assemblies,
            JsonElement access, string identity)
    {
        _root = root;
        _gateway = gateway;
        _art = art;
        _assemblies = assemblies;
        _access = access;
        _identity = identity;
    }

    internal static JsonElement ReadDefinition(string text, string expectedId)
    {
        if (text.Length is 0 or > 1048576 || text[0] == '\ufeff')
        {
            throw new InvalidDataException(message: "BUILDING_DEFINITION_SIZE");
        }

        using var document = JsonDocument.Parse(json: text, options: new JsonDocumentOptions { MaxDepth = 32 });
        JsonElement root = document.RootElement;
        ValidateValue(value: root);
        if (root.GetProperty(propertyName: "schemaVersion").GetInt32() != 1
                || root.GetProperty(propertyName: "version").GetInt32() != 1
                || !StringComparer.Ordinal.Equals(x: root.GetProperty(propertyName: "id").GetString(), y: expectedId))
        {
            throw new InvalidDataException(message: "BUILDING_DEFINITION_ID");
        }

        return root.Clone();
    }

    internal static int FunctionalTier(int completedTier, int requestedTier, double progress)
    {
        if (completedTier is < 1 or > 3 || requestedTier < completedTier || requestedTier > Math.Min(
                val1: 3, val2: completedTier + 1) || !double.IsFinite(d: progress) || progress is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(progress));
        }

        return progress == 1 ? requestedTier : completedTier;
    }

    internal static int FunctionalCapacity(int completedTier, int requestedTier, double progress)
    {
        return FunctionalTier(completedTier: completedTier, requestedTier: requestedTier, progress: progress)
                switch { 1 => 8, 2 => 12, _ => 16 };
    }

    internal static int FunctionalGuards(int completedTier, int requestedTier, double progress)
    {
        return FunctionalTier(completedTier: completedTier, requestedTier: requestedTier, progress: progress) == 3
                ? 2 : 0;
    }

    internal void Attach()
    {
        _gateway.RequireThread();
        ValidateDefinitions();
        if (_attached)
        {
            throw new InvalidOperationException(message: "BUILDING_ALREADY_ATTACHED");
        }

        _atlas = ResourceLoader.Load<Texture2D>(path: "res://assets/buildings/main-tiers.png")
                ?? throw new InvalidDataException(message: "BUILDING_ATLAS_MISSING");
        _root.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        var controls = new HBoxContainer { Name = "Controls" };
        _root.AddChild(node: controls);
        AddSelector(parent: controls, name: "Tier", labels: ["Main 1: fire/boxes", "Main 2: wood", "Main 3: metal"]);
        var labels = new List<string>();
        foreach (JsonElement sample in _assemblies.GetProperty(propertyName: "samples").EnumerateArray())
        {
            labels.Add(item: Text(value: sample, name: "label"));
        }

        AddSelector(parent: controls, name: "Sample", labels: labels);
        AddSelector(parent: controls, name: "Zoom", labels: ["1x", "2x", "4x", "8x"]);
        AddSelector(parent: controls, name: "Light", labels: ["Day", "Night"]);
        _root.GetNode<OptionButton>(path: "Controls/Tier").ItemSelected += TierSelected;
        _root.GetNode<OptionButton>(path: "Controls/Sample").ItemSelected += SampleSelected;
        _root.GetNode<OptionButton>(path: "Controls/Zoom").ItemSelected += ZoomSelected;
        _root.GetNode<OptionButton>(path: "Controls/Light").ItemSelected += LightSelected;
        var status = new Label { Name = "Status", Position = new Vector2(x: 0, y: 52),
                Size = new Vector2(x: 740, y: 64), AutowrapMode = TextServer.AutowrapMode.WordSmart };
        _root.AddChild(node: status);
        AddSurface();
        _attached = true;
        SelectSample(index: 0);
    }

    internal void SelectSample(int index)
    {
        _gateway.RequireThread();
        RequireAttached();
        JsonElement samples = _assemblies.GetProperty(propertyName: "samples");
        ArgumentOutOfRangeException.ThrowIfNegative(value: index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value: index, other: samples.GetArrayLength());
        _sample = index;
        _root.GetNode<OptionButton>(path: "Controls/Sample").Select(idx: index);
        JsonElement sample = samples[index];
        string work = Text(value: sample, name: "work");
        bool defeated = Text(value: sample, name: "visual") is "defeated";
        if (defeated && !_defeated && !sample.GetProperty(propertyName: "quiet").GetBoolean())
        {
            _destructionEvents++;
        }

        _defeated = defeated;
        _progress = sample.GetProperty(propertyName: "progress").GetDouble();
        _requestedTier = work is "upgrade" ? Math.Min(val1: 3, val2: _completedTier + 1) : _completedTier;
        _clock = 0;
        Project();
    }

    internal void Restore(int completedTier, int requestedTier, double progress, bool defeated)
    {
        _gateway.RequireThread();
        RequireAttached();
        _ = FunctionalTier(completedTier: completedTier, requestedTier: requestedTier, progress: progress);
        _completedTier = completedTier;
        _requestedTier = requestedTier;
        _progress = progress;
        _defeated = defeated;
        _sample = defeated ? 8 : requestedTier > completedTier && progress < 1 ? 9 : 0;
        _root.GetNode<OptionButton>(path: "Controls/Tier").Select(idx: completedTier - 1);
        _root.GetNode<OptionButton>(path: "Controls/Sample").Select(idx: _sample);
        _clock = 0;
        Project();
    }

    internal void Tick(double delta)
    {
        _gateway.RequireThread();
        RequireAttached();
        if (!double.IsFinite(d: delta) || delta is < 0 or > 60)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(delta));
        }

        _clock = (_clock + delta) % 1400;
        Animate();
    }

    internal void Detach()
    {
        _gateway.RequireThread();
        if (!_attached)
        {
            return;
        }

        _attached = false;
        _root.GetNode<OptionButton>(path: "Controls/Tier").ItemSelected -= TierSelected;
        _root.GetNode<OptionButton>(path: "Controls/Sample").ItemSelected -= SampleSelected;
        _root.GetNode<OptionButton>(path: "Controls/Zoom").ItemSelected -= ZoomSelected;
        _root.GetNode<OptionButton>(path: "Controls/Light").ItemSelected -= LightSelected;
        foreach (Node node in _root.GetChildren())
        {
            _root.RemoveChild(node: node);
            node.QueueFree();
        }

        _atlas = null;
    }

    private readonly Node2D _root;
    private readonly MainThreadGateway _gateway;
    private readonly JsonElement _art;
    private readonly JsonElement _assemblies;
    private readonly JsonElement _access;
    private readonly string _identity;
    private Texture2D? _atlas;
    private bool _attached;
    private int _completedTier = 1;
    private int _requestedTier = 1;
    private int _sample;
    private int _destructionEvents;
    private double _progress;
    private double _clock;
    private bool _defeated;

    private static string Text(JsonElement value, string name)
    {
        return value.GetProperty(propertyName: name).GetString()
                ?? throw new InvalidDataException(message: "BUILDING_NULL_TEXT " + name);
    }

    private static void ValidateValue(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                if (!names.Add(item: property.Name))
                {
                    throw new InvalidDataException(message: "BUILDING_DUPLICATE_KEY");
                }

                ValidateValue(value: property.Value);
            }
        }
        else if (value.ValueKind == JsonValueKind.Array)
        {
            if (value.GetArrayLength() > 512)
            {
                throw new InvalidDataException(message: "BUILDING_ARRAY_LIMIT");
            }

            foreach (JsonElement element in value.EnumerateArray())
            {
                ValidateValue(value: element);
            }
        }
        else if (value.ValueKind == JsonValueKind.Number && !double.IsFinite(d: value.GetDouble()))
        {
            throw new InvalidDataException(message: "BUILDING_NONFINITE");
        }
    }

    private void ValidateDefinitions()
    {
        if (_identity.Length is 0 or > 80 || _art.GetProperty(propertyName: "id").GetString() is not "main-buildings"
                || _assemblies.GetProperty(propertyName: "id").GetString() is not "main-assemblies"
                || _access.GetProperty(propertyName: "id").GetString() is not "camp-access"
                || Text(value: _art.GetProperty(propertyName: "atlas"), name: "path")
                        is not "res://assets/buildings/main-tiers.png")
        {
            throw new InvalidDataException(message: "BUILDING_RESOURCE_BINDING");
        }

        JsonElement tiers = _assemblies.GetProperty(propertyName: "tierDefinitions");
        JsonElement layouts = _access.GetProperty(propertyName: "layouts");
        if (tiers.GetArrayLength() != 3 || layouts.GetArrayLength() != 3
                || _assemblies.GetProperty(propertyName: "samples").GetArrayLength() != 12)
        {
            throw new InvalidDataException(message: "BUILDING_DEFINITION_COUNT");
        }

        for (int index = 0; index < 3; index++)
        {
            int capacity = FunctionalCapacity(completedTier: index + 1, requestedTier: index + 1, progress: 0);
            if (tiers[index].GetProperty(propertyName: "tier").GetInt32() != index + 1
                    || layouts[index].GetProperty(propertyName: "seats").GetArrayLength() != capacity
                    || tiers[index].GetProperty(propertyName: "attachments").GetArrayLength()
                            != FunctionalGuards(completedTier: index + 1, requestedTier: index + 1, progress: 0))
            {
                throw new InvalidDataException(message: "BUILDING_FUNCTIONAL_LAYOUT");
            }
        }
    }

    private void RequireAttached()
    {
        if (!_attached)
        {
            throw new InvalidOperationException(message: "BUILDING_DETACHED");
        }
    }

    private void AddSelector(Node parent, string name, IEnumerable<string> labels)
    {
        _gateway.RequireThread();
        var selector = new OptionButton { Name = name, CustomMinimumSize = new Vector2(x: 0, y: 48) };
        foreach (string label in labels)
        {
            selector.AddItem(label: label);
        }

        parent.AddChild(node: selector);
    }

    private void AddSurface()
    {
        _gateway.RequireThread();
        var scroll = new ScrollContainer { Name = "ArtScroll", Position = new Vector2(x: 0, y: 124),
                Size = new Vector2(x: 740, y: 300), ClipContents = true, FollowFocus = true };
        _root.AddChild(node: scroll);
        var content = new Control { Name = "Content", CustomMinimumSize = new Vector2(x: 280, y: 160) };
        scroll.AddChild(node: content);
        var art = new Node2D { Name = "Art", Position = new Vector2(x: 50, y: 20) };
        content.AddChild(node: art);
    }

    private void TierSelected(long index)
    {
        _gateway.RequireThread();
        if (index is < 0 or > 2)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(index));
        }

        _completedTier = (int)index + 1;
        SelectSample(index: _sample);
    }

    private void SampleSelected(long index)
    {
        _gateway.RequireThread();
        SelectSample(index: checked((int)index));
    }

    private void ZoomSelected(long index)
    {
        _gateway.RequireThread();
        if (index is < 0 or > 3)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(index));
        }

        int zoom = 1 << (int)index;
        _root.GetNode<Node2D>(path: "ArtScroll/Content/Art").Scale = new Vector2(x: zoom, y: zoom);
        _root.GetNode<Node2D>(path: "ArtScroll/Content/Art").Position = new Vector2(x: 50 * zoom, y: 20 * zoom);
        _root.GetNode<Control>(path: "ArtScroll/Content").CustomMinimumSize = new Vector2(x: 280 * zoom, y: 160 * zoom);
    }

    private void LightSelected(long index)
    {
        _gateway.RequireThread();
        if (index is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(index));
        }

        _root.GetNode<Node2D>(path: "ArtScroll/Content/Art").Modulate = index == 0 ? Colors.White
                : new Color(r: 0.55f, g: 0.65f, b: 0.82f);
    }

    private void Project()
    {
        _gateway.RequireThread();
        Node2D art = _root.GetNode<Node2D>(path: "ArtScroll/Content/Art");
        foreach (Node node in art.GetChildren())
        {
            art.RemoveChild(node: node);
            node.QueueFree();
        }

        int tier = FunctionalTier(completedTier: _completedTier, requestedTier: _requestedTier, progress: _progress);
        JsonElement sample = _assemblies.GetProperty(propertyName: "samples")[_sample];
        JsonElement definition = _assemblies.GetProperty(propertyName: "tierDefinitions")[tier - 1];
        string visual = _defeated ? "defeated" : Text(value: sample, name: "visual");
        AddSprite(parent: art, name: "Main", key: "tier-" + tier.ToString(provider: CultureInfo.InvariantCulture)
                + "-" + visual, position: Vector2.Zero);
        _root.SetMeta(name: "functional_tier", value: tier);
        _root.SetMeta(name: "health_identity", value: _identity);
        _root.SetMeta(name: "destruction_events", value: _destructionEvents);
        _root.SetMeta(name: "sample_only", value: true);
        if (!_defeated)
        {
            AddAccess(parent: art, tier: tier, sample: sample);
            AddGuards(parent: art, definition: definition);
            if (_progress < 1 && Text(value: sample, name: "work") is not "none")
            {
                AddSprite(parent: art, name: "PaidWork", key: "scaffold", position: new Vector2(x: 64, y: 50));
            }
        }

        ShowStatus(tier: tier, sample: sample, definition: definition);
        Animate();
    }

    private void AddAccess(Node2D parent, int tier, JsonElement sample)
    {
        _gateway.RequireThread();
        JsonElement stash = _access.GetProperty(propertyName: "stash");
        bool opened = sample.GetProperty(propertyName: "stashOpen").GetBoolean();
        AddSprite(parent: parent, name: "Stash", key: Text(value: stash, name: opened ? "openPart" : "closedPart"),
                position: new Vector2(x: 130, y: 68));
        parent.GetNode(path: "Stash").SetMeta(name: "stash_identity", value: Text(value: stash, name: "id"));
        parent.GetNode(path: "Stash").SetMeta(name: "concurrent_viewers", value: opened ? 2 : 0);
        var marker = new Label { Name = "StashMarker", Text = Text(value: stash, name: "marker"),
                Position = new Vector2(x: 124, y: 88), Modulate = sample.GetProperty(propertyName: "stashSelected")
                        .GetBoolean() ? new Color(r: 1, g: 0.8f, b: 0.3f) : Colors.White };
        parent.AddChild(node: marker);
        var seats = new Node2D { Name = "Seats" };
        parent.AddChild(node: seats);
        foreach (JsonElement seat in _access.GetProperty(propertyName: "layouts")[tier - 1]
                .GetProperty(propertyName: "seats").EnumerateArray())
        {
            var socket = new Marker2D { Name = Text(value: seat, name: "id"), Position = Position(value: seat)
                    + new Vector2(x: 80, y: 0) };
            seats.AddChild(node: socket);
            socket.SetMeta(name: "occupied", value: false);
            AddSeatMark(parent: socket);
        }

        var overflow = new Marker2D { Name = "Overflow", Position = new Vector2(x: 80, y: 126) };
        parent.AddChild(node: overflow);
        AddSprite(parent: parent, name: "Flame", key: "flame-0", position: new Vector2(x: 73, y: 65));
    }

    private void AddGuards(Node2D parent, JsonElement definition)
    {
        _gateway.RequireThread();
        foreach (JsonElement socket in definition.GetProperty(propertyName: "attachments").EnumerateArray())
        {
            var attachment = new Node2D { Name = Text(value: socket, name: "id"), Position = Position(value: socket) };
            parent.AddChild(node: attachment);
            attachment.SetMeta(name: "health_identity", value: _identity);
            attachment.SetMeta(name: "independent_repair", value: false);
            attachment.SetMeta(name: "independent_destruction", value: false);
            attachment.SetMeta(name: "loot_count", value: 0);
            attachment.SetMeta(name: "guard_count", value: 1);
            AddSprite(parent: attachment, name: "Platform", key: "tower", position: Vector2.Zero);
            AddSprite(parent: attachment, name: "Guard", key: "guard-shoot-0", position: new Vector2(x: 22, y: 0));
            AddSprite(parent: attachment, name: "Arrow", key: "arrow", position: new Vector2(x: 34, y: -14));
        }
    }

    private void AddSeatMark(Node parent)
    {
        _gateway.RequireThread();
        var mark = new Line2D { Name = "EmptyPlace", Width = 1,
                Points = [new Vector2(x: -2, y: 0), new Vector2(x: 2, y: 0)],
                DefaultColor = new Color(r: 0.65f, g: 0.5f, b: 0.3f) };
        parent.AddChild(node: mark);
    }

    private void Animate()
    {
        _gateway.RequireThread();
        if (_defeated)
        {
            return;
        }

        Node2D art = _root.GetNode<Node2D>(path: "ArtScroll/Content/Art");
        int tier = FunctionalTier(completedTier: _completedTier, requestedTier: _requestedTier, progress: _progress);
        JsonElement guard = _assemblies.GetProperty(propertyName: "guard");
        double interval = guard.GetProperty(propertyName: "attackProfile").GetProperty(
                propertyName: "intervalSeconds").GetDouble();
        foreach (JsonElement socket in _assemblies.GetProperty(propertyName: "tierDefinitions")[tier - 1]
                .GetProperty(propertyName: "attachments").EnumerateArray())
        {
            double time = (_clock + socket.GetProperty(propertyName: "phase").GetDouble()) % interval;
            JsonElement frames = guard.GetProperty(propertyName: "frames");
            int index = time >= guard.GetProperty(propertyName: "stop").GetDouble() ? 0
                    : Math.Min(val1: 4, val2: (int)(time * 8));
            JsonElement frame = frames[index];
            Node2D attachment = art.GetNode<Node2D>(path: Text(value: socket, name: "id"));
            Sprite2D body = attachment.GetNode<Sprite2D>(path: "Guard");
            SetTexture(sprite: body, key: Text(value: frame, name: "part"));
            JsonElement anchor = frame.GetProperty(propertyName: "anchor");
            body.Offset = new Vector2(x: -anchor[0].GetInt32(), y: -anchor[1].GetInt32());
            Sprite2D arrow = attachment.GetNode<Sprite2D>(path: "Arrow");
            arrow.Visible = time is >= 0.25 and < 0.625;
            arrow.Position = new Vector2(x: 34 + (float)(Math.Max(val1: 0, val2: time - 0.25) * 72), y: -14);
            attachment.SetMeta(name: "sample_time", value: time);
        }

        int flame = (int)(_clock * 8) % 8;
        SetTexture(sprite: art.GetNode<Sprite2D>(path: "Flame"), key: "flame-" + flame.ToString(
                provider: CultureInfo.InvariantCulture));
    }

    private void ShowStatus(int tier, JsonElement sample, JsonElement definition)
    {
        _gateway.RequireThread();
        double ratio = _defeated ? 0 : sample.GetProperty(propertyName: "hpRatio").GetDouble();
        string hp = (definition.GetProperty(propertyName: "health").GetInt32() * ratio).ToString(
                format: "0", provider: CultureInfo.InvariantCulture);
        string work = Text(value: sample, name: "work");
        int price = work is "upgrade" ? _assemblies.GetProperty(propertyName: "tierDefinitions")[_completedTier - 1]
                .GetProperty(propertyName: "upgradePrice").GetInt32() : definition.GetProperty(
                        propertyName: "repairPrice").GetInt32();
        _root.GetNode<Label>(path: "Status").Text = "Inspection sample · Main " + tier.ToString(
                provider: CultureInfo.InvariantCulture) + " · " + Text(value: sample, name: "label") + " · HP " + hp
                + "/" + definition.GetProperty(propertyName: "health").GetInt32().ToString(
                        provider: CultureInfo.InvariantCulture) + " · " + work + " price " + price.ToString(
                                provider: CultureInfo.InvariantCulture) + " · paid progress " + _progress.ToString(
                                        format: "P0", provider: CultureInfo.InvariantCulture)
                + " · seats " + (_defeated ? 0 : FunctionalCapacity(completedTier: _completedTier,
                        requestedTier: _requestedTier, progress: _progress)).ToString(
                                provider: CultureInfo.InvariantCulture)
                + " · guards " + (_defeated ? 0 : FunctionalGuards(completedTier: _completedTier,
                        requestedTier: _requestedTier, progress: _progress)).ToString(
                                provider: CultureInfo.InvariantCulture)
                + " · appearance pending04-32; full-hub adoption04-23";
    }

    private void AddSprite(Node parent, string name, string key, Vector2 position)
    {
        _gateway.RequireThread();
        var sprite = new Sprite2D { Name = name, Centered = false, Position = position,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
        SetTexture(sprite: sprite, key: key);
        parent.AddChild(node: sprite);
    }

    private void SetTexture(Sprite2D sprite, string key)
    {
        _gateway.RequireThread();
        foreach (JsonElement part in _art.GetProperty(propertyName: "parts").EnumerateArray())
        {
            if (!StringComparer.Ordinal.Equals(x: Text(value: part, name: "key"), y: key))
            {
                continue;
            }

            JsonElement rect = part.GetProperty(propertyName: "atlasRect");
            int x = rect[0].GetInt32();
            int y = rect[1].GetInt32();
            int width = rect[2].GetInt32();
            int height = rect[3].GetInt32();
            if (x < 0 || y < 0 || width <= 0 || height <= 0 || x + width > 1024 || y + height > 512)
            {
                throw new InvalidDataException(message: "BUILDING_ATLAS_BOUNDS");
            }

            sprite.Texture = new AtlasTexture { Atlas = _atlas, Region = new Rect2(x: x, y: y,
                    width: width, height: height), FilterClip = true };
            sprite.SetMeta(name: "part_key", value: key);
            return;
        }

        throw new InvalidDataException(message: "BUILDING_PART_MISSING " + key);
    }

    private Vector2 Position(JsonElement value)
    {
        _gateway.RequireThread();
        JsonElement position = value.GetProperty(propertyName: "position");
        return new Vector2(x: position[0].GetInt32(), y: position[1].GetInt32());
    }
}
