using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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

    internal BuildingPreview(Node2D root, MainThreadGateway gateway, JsonElement art, JsonElement assemblies,
            JsonElement guards, string identity, Func<string, byte[]?> read)
            : this(root: root, gateway: gateway, art: art, assemblies: assemblies, access: guards, identity: identity)
    {
        _defense = true;
        ValidateDefenseDefinitions();
        ValidateDefenseMedia(read: read, atlas: _art.GetProperty(propertyName: "atlas"),
                expected: "assets/buildings/defenses.png", width: 1024, height: 512);
        ValidateDefenseMedia(read: read, atlas: _access.GetProperty(propertyName: "atlas"),
                expected: "assets/characters/structural-guard.png", width: 256, height: 64);
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
        if (_defense)
        {
            AttachDefense();
            return;
        }

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
        if (_defense)
        {
            SelectDefenseSample(index: index);
            return;
        }

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
        if (_defense)
        {
            RestoreDefense(completedTier: completedTier, requestedTier: requestedTier, progress: progress,
                    defeated: defeated);
            return;
        }

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

        _clock = _defense ? _clock + delta : (_clock + delta) % 1400;
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
        _guardAtlas = null;
    }

    internal void AimGuard(int index, float targetX, float targetY, bool firing)
    {
        _gateway.RequireThread();
        RequireAttached();
        if (!_defense || !_defenseTower || index < 0 || index >= _root.GetMeta(name: "guard_count").AsInt32()
                || !float.IsFinite(f: targetX)
                || !float.IsFinite(f: targetY) || Math.Abs(value: targetX) > 4096
                || Math.Abs(value: targetY) > 4096 || Math.Abs(value: targetX) + Math.Abs(value: targetY) < 0.001f)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(index));
        }

        _aimX[index] = targetX;
        _aimY[index] = targetY;
        _firing[index] = firing;
        if (!firing)
        {
            _releasedAt[index] = -100;
        }

        Animate();
    }

    internal void ResetDefense()
    {
        _gateway.RequireThread();
        RequireAttached();
        if (!_defense)
        {
            throw new InvalidOperationException(message: "DEFENSE_OWNER_REQUIRED");
        }

        for (int index = 0; index < 3; index++)
        {
            _aimX[index] = index % 2 == 0 ? 1 : -1;
            _aimY[index] = 0;
            _firing[index] = true;
            _guardEvents[index] = 0;
            _releasedAt[index] = -100;
        }

        _destructionEvents = 0;
        _root.GetNode<OptionButton>(path: "Controls/Tier").Select(idx: (_defenseTower ? 3 : 0) + _completedTier - 1);
        SelectSample(index: 8);
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
    private readonly bool _defense;
    private Texture2D? _guardAtlas;
    private bool _defenseTower;
    private readonly float[] _aimX = [1, -1, 1];
    private readonly float[] _aimY = [0, 0, 0];
    private readonly bool[] _firing = [true, true, true];
    private readonly long[] _lastCycle = [0, 0, 0];
    private readonly long[] _guardEvents = [0, 0, 0];
    private readonly double[] _releasedAt = [-100, -100, -100];
    private readonly float[] _flightX = [0, 0, 0];
    private readonly float[] _flightY = [0, 0, 0];
    private readonly float[] _originX = [0, 0, 0];
    private readonly float[] _originY = [0, 0, 0];

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
        if (_defense)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value: index, other: 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value: index, other: 5);
            _defenseTower = index >= 3;
            _completedTier = (int)index % 3 + 1;
            SelectSample(index: _sample);
            return;
        }

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
        if (_defense)
        {
            ProjectDefense();
            return;
        }

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
        if (_defense)
        {
            AnimateDefense();
            return;
        }

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

    private static void RequireDefense(bool valid, string diagnostic)
    {
        if (!valid)
        {
            throw new InvalidDataException(message: "DEFENSE_" + diagnostic);
        }
    }

    private static void ValidateDefenseMedia(Func<string, byte[]?> read, JsonElement atlas, string expected,
            int width, int height)
    {
        RequireDefense(valid: StringComparer.Ordinal.Equals(x: Text(value: atlas, name: "path"),
                y: "res://" + expected), diagnostic: "MEDIA_PATH");
        byte[] data = read(arg: expected) ?? throw new InvalidDataException(message: "DEFENSE_MEDIA_MISSING "
                + expected);
        RequireDefense(valid: data.Length is >= 33 and <= 8388608
                && data.AsSpan(start: 0, length: 8).SequenceEqual(other: new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })
                && BinaryPrimitives.ReadInt32BigEndian(source: data.AsSpan(start: 16, length: 4)) == width
                && BinaryPrimitives.ReadInt32BigEndian(source: data.AsSpan(start: 20, length: 4)) == height,
                diagnostic: "MEDIA_DIMENSIONS");
        RequireDefense(valid: StringComparer.Ordinal.Equals(x: Convert.ToHexStringLower(inArray: SHA256.HashData(
                source: data)), y: Text(value: atlas, name: "sha256")), diagnostic: "MEDIA_HASH");
    }

    private void ValidateDefenseDefinitions()
    {
        ValidateValue(value: _art);
        ValidateValue(value: _assemblies);
        ValidateValue(value: _access);
        RequireDefense(valid: _identity.Length is > 0 and <= 80 && _identity.IsNormalized(
                normalizationForm: NormalizationForm.FormC), diagnostic: "IDENTITY");
        foreach ((JsonElement value, string id) in new[] { (_art, "defenses"), (_assemblies, "defense-assemblies"),
                (_access, "guards") })
        {
            RequireDefense(valid: StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: id)
                    && value.GetProperty(propertyName: "schemaVersion").GetInt32() == 1
                    && value.GetProperty(propertyName: "version").GetInt32() == 1, diagnostic: "DEFINITION_ID");
        }

        RequireDefense(valid: _art.GetProperty(propertyName: "logicalPixelsPerWorldPixel").GetInt32() == 1,
                diagnostic: "DENSITY");
        ValidateDefenseParts(pack: _art, width: 1024, height: 512, count: 46);
        ValidateDefenseParts(pack: _access, width: 256, height: 64, count: 11);
        ValidateDefenseGuard();
        ValidateDefenseTiers();
        ValidateDefenseSamples();
        ValidateDefenseBindings();
    }

    private static void ValidateDefenseParts(JsonElement pack, int width, int height, int count)
    {
        JsonElement size = pack.GetProperty(propertyName: "atlas").GetProperty(propertyName: "size");
        RequireDefense(valid: size.GetArrayLength() == 2 && size[0].GetInt32() == width
                && size[1].GetInt32() == height, diagnostic: "ATLAS_SIZE");
        JsonElement parts = pack.GetProperty(propertyName: "parts");
        RequireDefense(valid: parts.GetArrayLength() == count, diagnostic: "PART_COUNT");
        var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement part in parts.EnumerateArray())
        {
            string key = Text(value: part, name: "key");
            RequireDefense(valid: key.Length is > 0 and < 80 && key.IsNormalized(
                    normalizationForm: NormalizationForm.FormC) && keys.Add(item: key), diagnostic: "PART_ID");
            JsonElement rect = part.GetProperty(propertyName: "atlasRect");
            RequireDefense(valid: rect.GetArrayLength() == 4, diagnostic: "PART_RECT");
            int x = rect[0].GetInt32();
            int y = rect[1].GetInt32();
            int w = rect[2].GetInt32();
            int h = rect[3].GetInt32();
            RequireDefense(valid: w is > 0 and <= 160 && h is > 0 and <= 88 && x >= 0 && y >= 0
                    && x <= width - w && y <= height - h, diagnostic: "PART_BOUNDS");
        }
    }

    private static JsonElement DefensePart(JsonElement pack, string key)
    {
        foreach (JsonElement part in pack.GetProperty(propertyName: "parts").EnumerateArray())
        {
            if (StringComparer.Ordinal.Equals(x: Text(value: part, name: "key"), y: key))
            {
                return part;
            }
        }

        throw new InvalidDataException(message: "DEFENSE_PART_REFERENCE " + key);
    }

    private static void DefensePair(JsonElement pair, double first, double second, string diagnostic)
    {
        RequireDefense(valid: pair.GetArrayLength() == 2 && pair[0].GetDouble() == first
                && pair[1].GetDouble() == second, diagnostic: diagnostic);
    }

    private void ValidateDefenseGuard()
    {
        JsonElement timing = _access.GetProperty(propertyName: "timing");
        foreach ((string key, double expected) in new[] { ("start", 0d), ("stop", 0.625), ("sampleRate", 8d),
                ("interval", 1.4), ("release", 0.25), ("arrowLifetime", 0.375), ("arrowSpeed", 72d) })
        {
            RequireDefense(valid: timing.GetProperty(propertyName: key).GetDouble() == expected,
                    diagnostic: "GUARD_TIMING");
        }

        JsonElement frames = _access.GetProperty(propertyName: "frames");
        RequireDefense(valid: frames.GetArrayLength() == 5, diagnostic: "GUARD_FRAMES");
        for (int index = 0; index < 5; index++)
        {
            JsonElement frame = frames[index];
            string suffix = index.ToString(provider: CultureInfo.InvariantCulture);
            RequireDefense(valid: frame.GetProperty(propertyName: "time").GetDouble() == index * 0.125
                    && StringComparer.Ordinal.Equals(x: Text(value: frame, name: "body"), y: "torso-" + suffix)
                    && StringComparer.Ordinal.Equals(x: Text(value: frame, name: "arms"), y: "arms-" + suffix),
                    diagnostic: "GUARD_FRAME_BINDING");
            int anchor = index == 0 ? 11 : index == 4 ? 9 : 10;
            DefensePair(pair: frame.GetProperty(propertyName: "anchor"), first: anchor, second: 21,
                    diagnostic: "GUARD_ANCHOR");
            DefensePair(pair: frame.GetProperty(propertyName: "shoulder"), first: anchor, second: 10,
                    diagnostic: "GUARD_SHOULDER");
            _ = DefensePart(pack: _access, key: Text(value: frame, name: "body"));
            _ = DefensePart(pack: _access, key: Text(value: frame, name: "arms"));
        }

        _ = DefensePart(pack: _access, key: "arrow");
        ValidateDefenseOwnership();
    }

    private void ValidateDefenseOwnership()
    {
        JsonElement ownership = _access.GetProperty(propertyName: "ownership");
        foreach (string name in new[] { "inventory", "corpse", "loot", "bowDurability" })
        {
            RequireDefense(valid: !ownership.GetProperty(propertyName: name).GetBoolean(),
                    diagnostic: "GUARD_OWNERSHIP");
        }

        RequireDefense(valid: ownership.GetProperty(propertyName: "populationUse").GetInt32() == 0
                && Text(value: ownership, name: "ammo") is "unlimited-intrinsic"
                && ownership.GetProperty(propertyName: "targets").GetArrayLength() == 1
                && ownership.GetProperty(propertyName: "targets")[0].GetString() is "zombie",
                diagnostic: "GUARD_OWNERSHIP");
        JsonElement aim = _access.GetProperty(propertyName: "aim");
        RequireDefense(valid: aim.GetProperty(propertyName: "minimum").GetDouble() == -1.3
                && aim.GetProperty(propertyName: "maximum").GetDouble() == 1.3
                && aim.GetProperty(propertyName: "bodyRotation").GetDouble() == 0
                && aim.GetProperty(propertyName: "sourceBowInArmLayer").GetBoolean()
                && !aim.GetProperty(propertyName: "extraBow").GetBoolean(), diagnostic: "GUARD_AIM");
        DefensePair(pair: aim.GetProperty(propertyName: "releaseOffset"), first: 12, second: 0,
                diagnostic: "GUARD_RELEASE");
    }

    private void ValidateDefenseTiers()
    {
        JsonElement tiers = _assemblies.GetProperty(propertyName: "tierDefinitions");
        JsonElement artTiers = _art.GetProperty(propertyName: "tiers");
        RequireDefense(valid: tiers.GetArrayLength() == 6 && artTiers.GetArrayLength() == 6,
                diagnostic: "TIER_COUNT");
        int[] widths = [32, 32, 48, 44, 72, 96];
        int[] heights = [35, 43, 48, 48, 64, 80];
        for (int index = 0; index < 6; index++)
        {
            JsonElement row = tiers[index];
            JsonElement art = artTiers[index];
            string kind = index < 3 ? "wall" : "tower";
            int tier = index % 3 + 1;
            string key = kind + "-" + tier.ToString(provider: CultureInfo.InvariantCulture);
            RequireDefense(valid: StringComparer.Ordinal.Equals(x: Text(value: row, name: "id"), y: key)
                    && StringComparer.Ordinal.Equals(x: Text(value: row, name: "kind"), y: kind)
                    && row.GetProperty(propertyName: "tier").GetInt32() == tier
                    && art.GetProperty(propertyName: "tier").GetInt32() == tier
                    && StringComparer.Ordinal.Equals(x: Text(value: art, name: "material"), y: tier == 1 ? "wood"
                            : tier == 2 ? "reinforced-wood" : "metal-reinforced"), diagnostic: "TIER_ID");
            DefensePair(pair: art.GetProperty(propertyName: "footprint"), first: widths[index],
                    second: heights[index], diagnostic: "FOOTPRINT");
            foreach (string state in new[] { "intact", "damage-light", "damage-heavy", "destroyed" })
            {
                JsonElement part = DefensePart(pack: _art, key: Text(value: art.GetProperty(
                        propertyName: "states"), name: state));
                DefenseRectSize(part: part, width: widths[index], height: heights[index]);
            }

            ValidateDefenseTierGeometry(row: row, index: index, width: widths[index], height: heights[index]);
            ValidateDefenseSockets(row: row, index: index, width: widths[index], height: heights[index]);
        }
    }

    private static void DefenseRectSize(JsonElement part, int width, int height)
    {
        JsonElement rect = part.GetProperty(propertyName: "atlasRect");
        RequireDefense(valid: rect[2].GetInt32() == width && rect[3].GetInt32() == height,
                diagnostic: "PART_SIZE");
    }

    private void ValidateDefenseTierGeometry(JsonElement row, int index, int width, int height)
    {
        int[] health = [250, 900, 2200, 300, 800, 1600];
        int[] price = [20, 60, 140, 40, 90, 180];
        int[] work = [20, 35, 50, 30, 45, 60];
        int[] rebuild = [20, 80, 220, 40, 130, 310];
        RequireDefense(valid: row.GetProperty(propertyName: "health").GetInt32() == health[index]
                && row.GetProperty(propertyName: "price").GetInt32() == price[index]
                && row.GetProperty(propertyName: "workSeconds").GetInt32() == work[index]
                && row.GetProperty(propertyName: "rebuildPrice").GetInt32() == rebuild[index],
                diagnostic: "BASELINE");
        JsonElement rect = row.GetProperty(propertyName: "collision").GetProperty(propertyName: "rect");
        RequireDefense(valid: rect.GetArrayLength() == 4 && rect[0].GetInt32() == width / 2 - 4
                && rect[1].GetInt32() == 8 && rect[2].GetInt32() == 8 && rect[3].GetInt32() == height - 8,
                diagnostic: "COLLISION");
        RequireDefense(valid: StringComparer.Ordinal.Equals(x: Text(value: row.GetProperty(
                propertyName: "collision"), name: "factionBlocker"), y: index < 3 ? "zombies-only" : "none"),
                diagnostic: "FACTION_BLOCKER");
        DefensePair(pair: row.GetProperty(propertyName: "workAnchor"), first: width / 2, second: height - 2,
                diagnostic: "WORK_ANCHOR");
        string key = Text(value: row, name: "id");
        DefenseRectSize(part: DefensePart(pack: _art, key: key + "-frame"), width: width, height: height);
        DefenseRectSize(part: DefensePart(pack: _art, key: key + "-frame-damaged"), width: width, height: height);
        _ = DefensePart(pack: _art, key: Text(value: row, name: "sitePart"));
    }

    private static void ValidateDefenseSockets(JsonElement row, int index, int width, int height)
    {
        JsonElement sockets = row.GetProperty(propertyName: "sockets");
        int tier = index % 3 + 1;
        RequireDefense(valid: sockets.GetArrayLength() == (index < 3 ? 0 : tier), diagnostic: "GUARD_COUNT");
        var positions = new HashSet<string>(comparer: StringComparer.Ordinal);
        int slot = 0;
        foreach (JsonElement socket in sockets.EnumerateArray())
        {
            RequireDefense(valid: StringComparer.Ordinal.Equals(x: Text(value: socket, name: "id"),
                    y: "guard-" + slot.ToString(provider: CultureInfo.InvariantCulture))
                    && socket.GetProperty(propertyName: "phase").GetDouble()
                            == slot * 0.45 && socket.GetProperty(propertyName: "facing").GetInt32()
                                    == (slot % 2 == 0 ? 1 : -1), diagnostic: "SOCKET_ID");
            JsonElement position = socket.GetProperty(propertyName: "position");
            RequireDefense(valid: position.GetArrayLength() == 2 && position[0].GetInt32() is >= 10
                    && position[0].GetInt32() <= width - 10 && position[1].GetInt32() >= 0
                    && position[1].GetInt32() < height / 2 && positions.Add(item: position[0].GetInt32().ToString(
                            provider: CultureInfo.InvariantCulture) + ":" + position[1].GetInt32().ToString(
                                    provider: CultureInfo.InvariantCulture)),
                    diagnostic: "SOCKET_POSITION");
            DefensePair(pair: socket.GetProperty(propertyName: "projectileOrigin"), first: 12, second: -11,
                    diagnostic: "PROJECTILE_ORIGIN");
            slot++;
        }

        RequireDefense(valid: row.GetProperty(propertyName: "range").GetInt32() == (index < 3 ? 0 : 16 + tier * 4)
                && row.GetProperty(propertyName: "arrowDamage").GetInt32() == (index < 3 ? 0 : 8 + tier * 6),
                diagnostic: "ATTACK_PROFILE");
    }

    private void ValidateDefenseSamples()
    {
        string[] ids = ["unbuilt", "available", "tree-blocked", "portal-blocked", "main-tier-blocked",
            "unaffordable", "new-frame", "damaged-unfinished", "intact", "damage-light", "damage-heavy",
            "upgrade-paid", "upgrade-complete", "repair-paid", "repair-complete", "unsafe-waiting",
            "no-worker-waiting", "destroyed", "rebuild-paid", "rebuild-complete", "restore-destroyed",
            "restore-upgrade"];
        JsonElement samples = _assemblies.GetProperty(propertyName: "samples");
        RequireDefense(valid: samples.GetArrayLength() == ids.Length, diagnostic: "SAMPLE_COUNT");
        for (int index = 0; index < ids.Length; index++)
        {
            JsonElement sample = samples[index];
            string id = Text(value: sample, name: "id");
            RequireDefense(valid: StringComparer.Ordinal.Equals(x: id, y: ids[index])
                    && Text(value: sample, name: "label").Length is > 0 and < 100, diagnostic: "SAMPLE_ID");
            double progress = sample.GetProperty(propertyName: "progress").GetDouble();
            double hp = sample.GetProperty(propertyName: "hpRatio").GetDouble();
            RequireDefense(valid: double.IsFinite(d: progress) && progress is >= 0 and <= 1
                    && double.IsFinite(d: hp) && hp is >= 0 and <= 1, diagnostic: "SAMPLE_NUMBER");
            RequireDefense(valid: sample.GetProperty(propertyName: "site").GetBoolean() == (index is < 6)
                    && sample.GetProperty(propertyName: "newConstruction").GetBoolean() == (index is 6 or 7 or 18)
                    && sample.GetProperty(propertyName: "defeated").GetBoolean() == (index is 17 or 20)
                    && sample.GetProperty(propertyName: "quiet").GetBoolean() == (index is 20 or 21)
                    && sample.GetProperty(propertyName: "waiting").GetBoolean() == (index is 15 or 16),
                    diagnostic: "SAMPLE_STATE");
            RequireDefense(valid: Text(value: sample, name: "visual") is "intact" or "damage-light"
                    or "damage-heavy" or "destroyed" && Text(value: sample, name: "work") is "none" or "build"
                            or "repair" or "upgrade" or "rebuild", diagnostic: "SAMPLE_KIND");
        }
    }

    private void ValidateDefenseBindings()
    {
        RequireDefense(valid: _assemblies.GetProperty(propertyName: "sampleDataOnly").GetBoolean()
                && Text(value: _assemblies, name: "artPack") is "res://content/packs/defenses.json"
                && Text(value: _assemblies, name: "guardPack") is "res://content/packs/guards.json",
                diagnostic: "PACK_REFERENCE");
        JsonElement bindings = _assemblies.GetProperty(propertyName: "resourceBindings");
        foreach ((string key, JsonElement pack) in new[] { ("defenses", _art), ("guards", _access) })
        {
            JsonElement atlas = pack.GetProperty(propertyName: "atlas");
            JsonElement bound = bindings.GetProperty(propertyName: key);
            RequireDefense(valid: StringComparer.Ordinal.Equals(x: Text(value: atlas, name: "path"),
                    y: Text(value: bound, name: "path")) && StringComparer.Ordinal.Equals(
                            x: Text(value: atlas, name: "sha256"), y: Text(value: bound, name: "sha256")),
                    diagnostic: "RESOURCE_BINDING");
        }

        JsonElement restore = _assemblies.GetProperty(propertyName: "restoration");
        RequireDefense(valid: !restore.GetProperty(propertyName: "replayAttack").GetBoolean()
                && !restore.GetProperty(propertyName: "replayDestruction").GetBoolean()
                && restore.GetProperty(propertyName: "previousCompletedTierPermanent").GetBoolean()
                && restore.GetProperty(propertyName: "newGuardsOnlyAtProgressOne").GetBoolean(),
                diagnostic: "RESTORATION");
    }

    private void AttachDefense()
    {
        _gateway.RequireThread();
        if (_attached)
        {
            throw new InvalidOperationException(message: "BUILDING_ALREADY_ATTACHED");
        }

        _atlas = ResourceLoader.Load<Texture2D>(path: "res://assets/buildings/defenses.png")
                ?? throw new InvalidDataException(message: "DEFENSE_ATLAS_MISSING");
        _guardAtlas = ResourceLoader.Load<Texture2D>(path: "res://assets/characters/structural-guard.png")
                ?? throw new InvalidDataException(message: "DEFENSE_GUARD_ATLAS_MISSING");
        RequireDefense(valid: _atlas.GetWidth() == 1024 && _atlas.GetHeight() == 512
                && _guardAtlas.GetWidth() == 256 && _guardAtlas.GetHeight() == 64, diagnostic: "IMPORTED_MEDIA_SIZE");
        _root.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        var controls = new HBoxContainer { Name = "Controls" };
        _root.AddChild(node: controls);
        AddSelector(parent: controls, name: "Tier", labels: ["Wall 1: wood", "Wall 2: reinforced wood",
            "Wall 3: metal", "Tower 1: one guard", "Tower 2: two guards", "Tower 3: three guards"]);
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
        _root.AddChild(node: new Label { Name = "Status", Position = new Vector2(x: 0, y: 52),
                Size = new Vector2(x: 740, y: 64), AutowrapMode = TextServer.AutowrapMode.WordSmart });
        AddSurface();
        _attached = true;
        SelectSample(index: 8);
    }

    private void SelectDefenseSample(int index)
    {
        _gateway.RequireThread();
        JsonElement samples = _assemblies.GetProperty(propertyName: "samples");
        ArgumentOutOfRangeException.ThrowIfNegative(value: index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value: index, other: samples.GetArrayLength());
        JsonElement sample = samples[index];
        bool defeated = sample.GetProperty(propertyName: "defeated").GetBoolean();
        if (defeated && !_defeated && !sample.GetProperty(propertyName: "quiet").GetBoolean())
        {
            _destructionEvents++;
        }

        _sample = index;
        _defeated = defeated;
        _progress = sample.GetProperty(propertyName: "progress").GetDouble();
        _requestedTier = Text(value: sample, name: "work") is "upgrade" ? Math.Min(val1: 3,
                val2: _completedTier + 1) : _completedTier;
        _root.GetNode<OptionButton>(path: "Controls/Sample").Select(idx: index);
        _clock = 0;
        Project();
    }

    private void RestoreDefense(int completedTier, int requestedTier, double progress, bool defeated)
    {
        _gateway.RequireThread();
        _completedTier = completedTier;
        _requestedTier = requestedTier;
        _progress = progress;
        _defeated = defeated;
        _sample = defeated ? 20 : requestedTier > completedTier && progress < 1 ? 21 : 8;
        _root.GetNode<OptionButton>(path: "Controls/Tier").Select(idx: (_defenseTower ? 3 : 0) + completedTier - 1);
        _root.GetNode<OptionButton>(path: "Controls/Sample").Select(idx: _sample);
        _clock = 0;
        Project();
    }

    private void ProjectDefense()
    {
        _gateway.RequireThread();
        Node2D art = _root.GetNode<Node2D>(path: "ArtScroll/Content/Art");
        foreach (Node node in art.GetChildren())
        {
            art.RemoveChild(node: node);
            node.QueueFree();
        }

        JsonElement sample = _assemblies.GetProperty(propertyName: "samples")[_sample];
        int tier = _defeated ? _completedTier : FunctionalTier(completedTier: _completedTier,
                requestedTier: _requestedTier, progress: _progress);
        JsonElement definition = _assemblies.GetProperty(propertyName: "tierDefinitions")[(_defenseTower ? 3 : 0)
                + tier - 1];
        string key = Text(value: definition, name: "id");
        bool site = sample.GetProperty(propertyName: "site").GetBoolean();
        bool frame = sample.GetProperty(propertyName: "newConstruction").GetBoolean() && _progress < 1;
        bool functional = !site && !frame && !_defeated;
        string visual = _defeated ? "ruin" : Text(value: sample, name: "visual");
        string part = site ? Text(value: definition, name: "sitePart") : frame
                ? key + (_sample == 7 ? "-frame-damaged" : "-frame") : key + (visual is "intact" ? "" : "-" + visual);
        AddDefenseSprite(parent: art, name: "Structure", key: part, guard: false);
        ProjectDefenseMetadata(tier: tier, functional: functional, frame: frame, site: site);
        AddDefenseGeometry(parent: art, definition: definition, functional: functional);
        if (functional)
        {
            AddDefenseGuards(parent: art, definition: definition);
            if (_progress < 1 && Text(value: sample, name: "work") is not "none")
            {
                AddDefenseSprite(parent: art, name: "PaidWork", key: key + "-frame", guard: false);
            }
        }

        ShowDefenseStatus(tier: tier, definition: definition, sample: sample);
        Animate();
    }

    private void ProjectDefenseMetadata(int tier, bool functional, bool frame, bool site)
    {
        _gateway.RequireThread();
        _root.SetMeta(name: "functional_tier", value: functional ? tier : 0);
        _root.SetMeta(name: "previous_completed_tier", value: site || frame && _sample != 18 ? 0
                : functional && _progress == 1 ? tier : _completedTier);
        _root.SetMeta(name: "guard_count", value: functional && _defenseTower ? tier : 0);
        _root.SetMeta(name: "faction_blocker_active", value: functional && !_defenseTower);
        _root.SetMeta(name: "destruction_events", value: _destructionEvents);
        _root.SetMeta(name: "health_identity", value: _identity);
        _root.SetMeta(name: "corpse_count", value: 0);
        _root.SetMeta(name: "loot_count", value: 0);
        _root.SetMeta(name: "inventory_count", value: 0);
        _root.SetMeta(name: "population_use", value: 0);
        _root.SetMeta(name: "sample_only", value: true);
        for (int index = 0; index < 3; index++)
        {
            _lastCycle[index] = (long)Math.Floor(d: (index * 0.45 - 0.25) / 1.4);
            _releasedAt[index] = -100;
        }
    }

    private void AddDefenseGeometry(Node2D parent, JsonElement definition, bool functional)
    {
        _gateway.RequireThread();
        JsonElement rect = definition.GetProperty(propertyName: "collision").GetProperty(propertyName: "rect");
        float width = rect[2].GetSingle();
        float height = rect[3].GetSingle();
        var body = new StaticBody2D { Name = "FactionBlocker", CollisionLayer = functional && !_defenseTower
                ? 2u : 0u, CollisionMask = 0 };
        parent.AddChild(node: body);
        var collision = new CollisionShape2D { Name = "Collision", Shape = new RectangleShape2D
                { Size = new Vector2(x: width, y: height) }, Position = new Vector2(x: rect[0].GetSingle()
                        + width / 2, y: rect[1].GetSingle() + height / 2) };
        body.AddChild(node: collision);
        JsonElement work = definition.GetProperty(propertyName: "workAnchor");
        parent.AddChild(node: new Marker2D { Name = "WorkAnchor", Position = new Vector2(x: work[0].GetSingle(),
                y: work[1].GetSingle()) });
    }

    private void AddDefenseGuards(Node2D parent, JsonElement definition)
    {
        _gateway.RequireThread();
        int index = 0;
        foreach (JsonElement socket in definition.GetProperty(propertyName: "sockets").EnumerateArray())
        {
            var guard = new Node2D { Name = Text(value: socket, name: "id"), Position = Position(value: socket) };
            parent.AddChild(node: guard);
            guard.SetMeta(name: "inventory", value: false);
            guard.SetMeta(name: "ammo", value: "unlimited-intrinsic");
            guard.SetMeta(name: "loot_count", value: 0);
            var facing = new Node2D { Name = "Facing" };
            guard.AddChild(node: facing);
            AddDefenseSprite(parent: facing, name: "Torso", key: "torso-0", guard: true);
            var aim = new Node2D { Name = "Aim", Position = new Vector2(x: 0, y: -11) };
            facing.AddChild(node: aim);
            AddDefenseSprite(parent: aim, name: "ArmBow", key: "arms-0", guard: true);
            aim.AddChild(node: new Marker2D { Name = "ReleaseOrigin", Position = new Vector2(x: 12, y: 0) });
            AddDefenseSprite(parent: guard, name: "Arrow", key: "arrow", guard: true);
            _releasedAt[index] = -100;
            index++;
        }
    }

    private void AnimateDefense()
    {
        _gateway.RequireThread();
        Node2D art = _root.GetNode<Node2D>(path: "ArtScroll/Content/Art");
        int count = _root.GetMeta(name: "guard_count").AsInt32();
        for (int index = 0; index < count; index++)
        {
            Node2D guard = art.GetNode<Node2D>(path: "guard-" + index.ToString(provider: CultureInfo.InvariantCulture));
            double local = _clock + index * 0.45;
            double time = local % 1.4;
            long cycle = (long)Math.Floor(d: (local - 0.25) / 1.4);
            float angle = (float)Math.Clamp(value: Math.Atan2(y: _aimY[index], x: Math.Abs(value: _aimX[index])),
                    min: -1.3, max: 1.3);
            int facing = _aimX[index] < 0 ? -1 : 1;
            int frame = !_firing[index] ? 1 : time >= 0.625 ? 0 : Math.Min(val1: 4, val2: (int)(time * 8));
            ProjectGuardPose(guard: guard, frame: frame, angle: angle, facing: facing);
            if (_firing[index] && cycle > _lastCycle[index])
            {
                _guardEvents[index] += cycle - _lastCycle[index];
                _releasedAt[index] = cycle * 1.4 + 0.25 - index * 0.45;
                _flightX[index] = facing * (float)Math.Cos(d: angle);
                _flightY[index] = (float)Math.Sin(a: angle);
                _originX[index] = _flightX[index] * 12;
                _originY[index] = -11 + _flightY[index] * 12;
            }

            _lastCycle[index] = cycle;
            double age = _clock - _releasedAt[index];
            Sprite2D arrow = guard.GetNode<Sprite2D>(path: "Arrow");
            arrow.Visible = _firing[index] && age is >= 0 and < 0.375;
            arrow.Position = new Vector2(x: _originX[index] + (float)(age * 72) * _flightX[index],
                    y: _originY[index] + (float)(age * 72) * _flightY[index]);
            arrow.Rotation = (float)Math.Atan2(y: _flightY[index], x: _flightX[index]);
            guard.SetMeta(name: "sample_time", value: time);
            guard.SetMeta(name: "release_count", value: _guardEvents[index]);
        }
    }

    private void ProjectGuardPose(Node2D guard, int frame, float angle, int facing)
    {
        _gateway.RequireThread();
        JsonElement selected = _access.GetProperty(propertyName: "frames")[frame];
        Node2D facingNode = guard.GetNode<Node2D>(path: "Facing");
        facingNode.Scale = new Vector2(x: facing, y: 1);
        Sprite2D torso = facingNode.GetNode<Sprite2D>(path: "Torso");
        Sprite2D arms = facingNode.GetNode<Sprite2D>(path: "Aim/ArmBow");
        SetDefenseTexture(sprite: torso, key: Text(value: selected, name: "body"), guard: true);
        SetDefenseTexture(sprite: arms, key: Text(value: selected, name: "arms"), guard: true);
        JsonElement anchor = selected.GetProperty(propertyName: "anchor");
        torso.Offset = new Vector2(x: -anchor[0].GetSingle(), y: -anchor[1].GetSingle());
        arms.Offset = new Vector2(x: -anchor[0].GetSingle(), y: -10);
        facingNode.GetNode<Node2D>(path: "Aim").Rotation = angle;
        guard.SetMeta(name: "facing", value: facing);
        guard.SetMeta(name: "aim_angle", value: angle);
    }

    private void ShowDefenseStatus(int tier, JsonElement definition, JsonElement sample)
    {
        _gateway.RequireThread();
        string work = Text(value: sample, name: "work");
        int price = DefenseWorkPrice(definition: definition, sample: sample);
        _root.GetNode<Label>(path: "Status").Text = "Inspection sample · " + Text(value: definition, name: "id")
                + " · " + Text(value: sample, name: "label") + " · " + work + " price " + price.ToString(
                        provider: CultureInfo.InvariantCulture) + " · paid " + _progress.ToString(format: "P0",
                                provider: CultureInfo.InvariantCulture) + " · HP " + (_defeated ? 0
                                        : definition.GetProperty(propertyName: "health").GetInt32()
                                                * sample.GetProperty(propertyName: "hpRatio").GetDouble()).ToString(
                                                        format: "0", provider: CultureInfo.InvariantCulture)
                + " · completed " + _completedTier.ToString(provider: CultureInfo.InvariantCulture)
                + " · displayed " + tier.ToString(provider: CultureInfo.InvariantCulture)
                + " · guards " + _root.GetMeta(name: "guard_count").AsInt32().ToString(
                        provider: CultureInfo.InvariantCulture) + " · appearance/input pending04-32";
    }

    private int DefenseWorkPrice(JsonElement definition, JsonElement sample)
    {
        string work = Text(value: sample, name: "work");
        JsonElement tiers = _assemblies.GetProperty(propertyName: "tierDefinitions");
        int start = _defenseTower ? 3 : 0;
        if (work is "upgrade")
        {
            return _requestedTier > _completedTier ? tiers[start + _requestedTier - 1]
                    .GetProperty(propertyName: "price").GetInt32() : 0;
        }

        if (work is "repair")
        {
            int value = 0;
            for (int index = 0; index < _completedTier; index++)
            {
                value += tiers[start + index].GetProperty(propertyName: "price").GetInt32();
            }

            double health = sample.GetProperty(propertyName: "hpRatio").GetDouble();
            return _progress == 1 || _defeated || health is <= 0 or >= 1 ? 0
                    : Math.Max(val1: 1, val2: (int)Math.Ceiling(a: value * 0.5));
        }

        return definition.GetProperty(propertyName: work is "rebuild" ? "rebuildPrice" : "price").GetInt32();
    }

    private void AddDefenseSprite(Node parent, string name, string key, bool guard)
    {
        _gateway.RequireThread();
        var sprite = new Sprite2D { Name = name, Centered = false,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
        SetDefenseTexture(sprite: sprite, key: key, guard: guard);
        parent.AddChild(node: sprite);
    }

    private void SetDefenseTexture(Sprite2D sprite, string key, bool guard)
    {
        _gateway.RequireThread();
        JsonElement rect = DefensePart(pack: guard ? _access : _art, key: key).GetProperty(propertyName: "atlasRect");
        sprite.Texture = new AtlasTexture { Atlas = guard ? _guardAtlas : _atlas, Region = new Rect2(
                x: rect[0].GetInt32(), y: rect[1].GetInt32(), width: rect[2].GetInt32(), height: rect[3].GetInt32()),
                FilterClip = true };
        sprite.SetMeta(name: "part_key", value: key);
    }

    private Vector2 Position(JsonElement value)
    {
        _gateway.RequireThread();
        JsonElement position = value.GetProperty(propertyName: "position");
        return new Vector2(x: position[0].GetInt32(), y: position[1].GetInt32());
    }
}
