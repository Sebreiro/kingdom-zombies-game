using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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


    /// <summary>Owns exact existing character, corpse, identity and environment inspection presenters.</summary>
    internal sealed class ResourceView : IDisposable
    {
        /// <summary>Revokes the current presenter before its shell retires owned native nodes.</summary>
        public void Dispose()
        {
            Close();
        }

        internal ResourceView(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read, string id)
        {
            _root = root;
            _gateway = gateway;
            _read = read;
            _id = id;
        }

        internal string Status => _status;

        internal static bool Handles(string id)
        {
            return UiHandles(id: id) || id is "CH-10" or "CH-11" or "CH-12" or "CH-13" or "AN-09" or "AN-17"
                    or "EN-01" or "EN-02" or "EN-03" or "EN-04" or "EN-05" or "EN-06" or "EN-07"
                    or "EN-08" or "EN-09" or "EN-10" or "EN-11";
        }

        internal static ResourceView Open(InspectionShell? shell, MainThreadGateway gateway,
                Func<string, byte[]?> read, string id)
        {
            gateway.RequireThread();
            if (shell is null || !Handles(id: id))
            {
                throw new InvalidDataException(message: "RESOURCE_VIEW_ID " + id);
            }

            var root = new Node2D { Name = "ResourcePreview", TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
            shell.Install(preview: root);
            var result = new ResourceView(root: root, gateway: gateway, read: read, id: id);
            try
            {
                result.Build();
            }
            catch (Exception error) when (error is InvalidDataException or JsonException or FormatException
                    or IOException or KeyNotFoundException)
            {
                result.Close();
                shell.Retire();
                var failure = new Node2D { Name = "ResourceError" };
                failure.AddChild(node: new Label { Text = id + " — " + error.Message });
                shell.Install(preview: failure);
                result._status = "Error — " + id + " — " + error.Message;
            }

            return result;
        }

        internal void Tick(double delta)
        {
            _gateway.RequireThread();
            if (!_closed && _playing && _time is not null)
            {
                Clock.Value = Math.Min(val1: 60, val2: Clock.Value + delta);
            }
        }

        internal void Close()
        {
            _gateway.RequireThread();
            if (_closed)
            {
                return;
            }

            _closed = true;
            if (_signalsConnected && _choices is not null)
            {
                Choices.ItemSelected -= Changed;
            }
            if (_signalsConnected && _time is not null)
            {
                Clock.ValueChanged -= Seek;
            }
            if (_signalsConnected && _play is not null)
            {
                _play.Toggled -= Play;
            }
            if (_signalsConnected && _step is not null)
            {
                _step.Pressed -= Step;
            }
            _signalsConnected = false;
            _character?.Detach();
            _corpse?.Close();
            _pile?.Close();
            _crowd?.Close();
            _world?.Close();
            _inventory?.Close();
            _camp?.Close();
            _session?.Close();
            _gallery?.Close();
            foreach (AtlasTexture texture in _uiTextures)
            {
                texture.Dispose();
            }
            _uiTextures.Clear();
            _root.SetMeta(name: "closed", value: true);
        }

        private readonly Node2D _root;
        private readonly MainThreadGateway _gateway;
        private readonly Func<string, byte[]?> _read;
        private readonly string _id;
        private InventoryLayouts? _inventory;
        private CampLayouts? _camp;
        private SessionLayouts? _session;
        private IdentityKit? _gallery;
        private Control? _surface;
        private JsonElement _uiDefinition;
        private JsonElement _uiData;
        private string _uiSource = "";
        private readonly List<AtlasTexture> _uiTextures = [];
        private string _status = "Partial — actual source-bound preview; acceptance pending";
        private bool _closed;
        private bool _playing;
        private bool _signalsConnected;
        private CharacterPreview? _character;
        private CorpsePreview? _corpse;
        private CorpsePreview? _pile;
        private CrowdPreview? _crowd;
        private WorldPreview? _world;
        private Node2D? _art;
        private OptionButton? _choices;
        private HSlider? _time;
        private CheckButton? _play;
        private Button? _step;
        private JsonElement _fixture;
        private JsonElement _map;
        private string _state = "";
        private long _epoch;
        private readonly List<string> _states = [];

        private Node2D Art => _art ?? throw new InvalidOperationException(message: "RESOURCE_ART_MISSING");
        private OptionButton Choices => _choices
                ?? throw new InvalidOperationException(message: "RESOURCE_CHOICES_MISSING");
        private HSlider Clock => _time ?? throw new InvalidOperationException(message: "RESOURCE_CLOCK_MISSING");
        private CrowdPreview Crowd => _crowd ?? throw new InvalidOperationException(message: "RESOURCE_CROWD_MISSING");
        private CorpsePreview Corpse => _corpse
                ?? throw new InvalidOperationException(message: "RESOURCE_CORPSE_MISSING");
        private Control Surface => _surface ?? throw new InvalidOperationException(message: "RESOURCE_UI_SURFACE");

        private void RequireActive()
        {
            _gateway.RequireThread();
            if (_closed || !_root.IsInsideTree())
            {
                throw new InvalidOperationException(message: "RESOURCE_VIEW_CLOSED " + _id);
            }
        }

        private JsonElement Read(string path)
        {
            byte[] value = _read(arg: path) ?? throw new InvalidDataException(message: "RESOURCE_VIEW_MISSING " + path);
            return IdentityKit.Read(bytes: value);
        }

        private void Build()
        {
            _gateway.RequireThread();
            RequireActive();
            _art = new Node2D { Name = "Art", Position = new Vector2(x: 280, y: 150) };
            _root.AddChild(node: _art);
            var controls = new VBoxContainer { Name = "Controls", Position = new Vector2(x: 8, y: 300) };
            _root.AddChild(node: controls);
            var identity = new Label { Name = "Identity", Text = _id + " · Source-bound existing presentation" };
            controls.AddChild(node: identity);
            _choices = new OptionButton { Name = "State", CustomMinimumSize = new Vector2(x: 280, y: 48) };
            controls.AddChild(node: _choices);
            _time = new HSlider { Name = "Time", MinValue = 0, MaxValue = 60, Step = 0.125,
                CustomMinimumSize = new Vector2(x: 280, y: 32) };
            controls.AddChild(node: _time);
            _play = new CheckButton { Name = "Play", Text = "Play explicit preview" };
            controls.AddChild(node: _play);
            _step = new Button { Name = "Step", Text = "Step 0.125s" };
            controls.AddChild(node: _step);
            if (UiHandles(id: _id))
            {
                BuildUi();
            }
            else if (_id.StartsWith(value: "EN-", comparisonType: StringComparison.Ordinal))
            {
                BuildWorld();
            }
            else if (_id is "CH-10" or "CH-11" or "CH-12" or "AN-17")
            {
                BuildCrowd();
            }
            else
            {
                BuildCorpse();
            }

            if (_states.Count == 0)
            {
                throw new InvalidDataException(message: "RESOURCE_VIEW_EMPTY " + _id);
            }
            Choices.Select(idx: 0);
            Choices.ItemSelected += Changed;
            Clock.ValueChanged += Seek;
            _play.Toggled += Play;
            _step.Pressed += Step;
            _signalsConnected = true;
            SelectState();
        }

        private static bool UiHandles(string id)
        {
            return Enumerable.Range(start: 1, count: 27).Any(predicate: number => Same(left: id,
                    right: "UI-" + number.ToString(format: "00", provider: CultureInfo.InvariantCulture)));
        }

        private void BuildUi()
        {
            _gateway.RequireThread();
            _uiSource = "content/packs/" + (_id switch
            {
                "UI-01" or "UI-02" => "ui-kit",
                "UI-03" or "UI-04" or "UI-07" or "UI-08" => "ui-identity",
                "UI-05" or "UI-12" or "UI-13" => "ui-inventory",
                "UI-06" or "UI-14" or "UI-15" => "ui-legacy-stash",
                "UI-16" or "UI-17" or "UI-18" or "UI-19" => "ui-camp",
                "UI-20" or "UI-21" or "UI-26" => "ui-map-warnings",
                "UI-09" or "UI-10" or "UI-11" or "UI-22" or "UI-23" or "UI-24" => "ui-session",
                "UI-25" or "UI-27" => "ui-settings",
                _ => throw new InvalidDataException(message: "RESOURCE_UI_ID " + _id),
            }) + ".json";
            _uiDefinition = _uiSource is "content/packs/ui-kit.json"
                    ? DataTextPreview.UiKitDocument(bytes: UiBytes(path: _uiSource)) : Read(path: _uiSource);
            _ = UiBytes(path: "assets/ui/InspectionTheme.tres");
            Art.Position = new Vector2(x: 8, y: 8);
            var scroll = new ScrollContainer { Name = "LibraryScroll", Size = new Vector2(x: 700, y: 280) };
            Art.AddChild(node: scroll);
            _surface = new Control { Name = "Surface", CustomMinimumSize = new Vector2(x: 1280, y: 720),
                Theme = ResourceLoader.Load<Theme>(path: "res://assets/ui/InspectionTheme.tres") };
            scroll.AddChild(node: _surface);
            if (_id is "UI-01" or "UI-02")
            {
                foreach (JsonElement state in _uiDefinition.GetProperty(propertyName: "states").EnumerateArray())
                {
                    AddState(state: state.GetString() ?? throw new InvalidDataException(message: "RESOURCE_UI_STATE"));
                }
            }
            else if (_id is "UI-03" or "UI-04" or "UI-07")
            {
                BuildIdentityGallery();
            }
            else if (_id is "UI-08")
            {
                AddState(state: Text(value: _uiDefinition.GetProperty(propertyName: "desktop"), name: "title"));
            }
            else
            {
                BuildUiLayout();
            }
        }

        private byte[] UiBytes(string path)
        {
            byte[] value = _read(arg: path)
                    ?? throw new InvalidDataException(message: "RESOURCE_UI_MISSING " + path);
            if (value.Length is 0 or > 16777216)
            {
                throw new InvalidDataException(message: "RESOURCE_UI_BYTES " + path);
            }
            return value;
        }

        private byte[] UiBinding(JsonElement binding)
        {
            string path = Text(value: binding, name: "path");
            byte[] value = UiBytes(path: path);
            if (!Same(left: Convert.ToHexStringLower(inArray: SHA256.HashData(source: value)),
                    right: Text(value: binding, name: "sha256")))
            {
                throw new InvalidDataException(message: "RESOURCE_UI_BINDING " + path);
            }
            return value;
        }

        private void BuildUiLayout()
        {
            _gateway.RequireThread();
            byte[][] inputs = _uiDefinition.GetProperty(propertyName: "bindings").EnumerateArray()
                    .Select(selector: binding => UiBinding(binding: binding)).ToArray();
            byte[] pack = UiBytes(path: _uiSource);
            if (_uiSource.EndsWith(value: "/ui-inventory.json", comparisonType: StringComparison.Ordinal)
                    || _uiSource.EndsWith(value: "/ui-legacy-stash.json", comparisonType: StringComparison.Ordinal))
            {
                _uiData = InventoryLayouts.Read(bytes: inputs[0]);
                _inventory = new InventoryLayouts(root: Surface, gateway: _gateway, pack: pack, inputs: inputs);
            }
            else if (_uiSource.EndsWith(value: "/ui-camp.json", comparisonType: StringComparison.Ordinal))
            {
                _uiData = CampLayouts.Read(bytes: inputs[0]);
                _camp = new CampLayouts(root: Surface, gateway: _gateway, pack: pack, inputs: inputs);
            }
            else if (_uiSource.EndsWith(value: "/ui-map-warnings.json", comparisonType: StringComparison.Ordinal))
            {
                _uiData = CampLayouts.Read(bytes: inputs[0]);
                _camp = new CampLayouts(root: Surface, gateway: _gateway, pack: pack,
                        inputs: inputs, mapWarnings: true);
            }
            else
            {
                _uiData = SessionLayouts.Read(bytes: inputs[0]);
                _session = new SessionLayouts(root: Surface, gateway: _gateway, pack: pack, inputs: inputs);
            }
            foreach (JsonElement sample in _uiData.GetProperty(propertyName: "samples").EnumerateArray()
                    .Where(predicate: sample => UiSampleMatches(sample: sample)).OrderByDescending(
                            keySelector: sample => _id is "UI-12" && Text(value: sample, name: "id")
                                    is "inventory.broken"))
            {
                AddState(state: Text(value: sample, name: "id"));
            }
        }

        private bool UiSampleMatches(JsonElement sample)
        {
            string id = Text(value: sample, name: "id");
            string surface = sample.TryGetProperty(propertyName: "surface", value: out JsonElement value)
                    ? value.GetString() ?? "" : "";
            return _id switch
            {
                "UI-06" => id.StartsWith(value: "legacy.", comparisonType: StringComparison.Ordinal),
                "UI-14" => id.StartsWith(value: "stash.", comparisonType: StringComparison.Ordinal),
                "UI-15" => id.StartsWith(value: "target.", comparisonType: StringComparison.Ordinal),
                "UI-16" => surface is "people",
                "UI-17" => surface is "sale",
                "UI-18" => surface is "site",
                "UI-19" => surface is "rally",
                "UI-20" => id.StartsWith(value: "map.", comparisonType: StringComparison.Ordinal),
                "UI-21" => id.StartsWith(value: "spectator.", comparisonType: StringComparison.Ordinal),
                "UI-26" => id.StartsWith(value: "warning.", comparisonType: StringComparison.Ordinal),
                "UI-09" => surface is "main" or "solo",
                "UI-10" => surface is "host" or "browser" or "manual" or "lobby",
                "UI-11" => surface is "join",
                "UI-22" => surface is "saves",
                "UI-23" => surface is "host-exit" or "guest-leave",
                "UI-24" => surface is "victory" or "defeat",
                "UI-25" => surface is "settings",
                "UI-27" => surface is "diagnostics",
                _ => _id is "UI-05" or "UI-12" or "UI-13",
            };
        }

        private void BuildIdentityGallery()
        {
            _gateway.RequireThread();
            var scroll = new ScrollContainer { Name = "Scroll", Size = new Vector2(x: 1280, y: 720) };
            Surface.AddChild(node: scroll);
            var content = new VBoxContainer { Name = "Content", CustomMinimumSize = new Vector2(x: 1100, y: 0) };
            scroll.AddChild(node: content);
            foreach (string name in new[] { "Bindings", "Statuses", "Players", "Items" })
            {
                content.AddChild(node: new VBoxContainer { Name = name });
            }
            _gallery = new IdentityKit(root: Surface, gateway: _gateway, identity: UiBytes(path: _uiSource),
                    definitions: UiBytes(path: "content/config/items.json"),
                    weapons: UiBytes(path: "content/packs/items-01-10.json"),
                    clothing: UiBytes(path: "content/packs/items-11-24.json"));
            AddState(state: "Authored snapshot");
            if (_id is "UI-07")
            {
                foreach (JsonElement state in _uiDefinition.GetProperty(propertyName: "playerStates").EnumerateArray())
                {
                    AddState(state: state.GetString() ?? throw new InvalidDataException(message: "RESOURCE_UI_STATE"));
                }
            }
        }

        private byte[] IdentitySnapshot()
        {
            return JsonSerializer.SerializeToUtf8Bytes(value: new
            {
                bindings = _uiDefinition.GetProperty(propertyName: "bindings"),
                statuses = _uiDefinition.GetProperty(propertyName: "statuses").EnumerateArray().Select(
                        selector: row => new { id = Text(value: row, name: "id"),
                            value = Text(value: row, name: "label") }),
                players = _uiDefinition.GetProperty(propertyName: "players").EnumerateArray().Select(
                        selector: row => new
                        {
                            id = Text(value: row, name: "id"), number = Text(value: row, name: "number"),
                            name = Text(value: row, name: "name"), shape = Text(value: row, name: "shape"),
                            color = Text(value: row, name: "color"),
                            state = _id is "UI-07" && _state is not "Authored snapshot"
                                    ? _state : Text(value: row, name: "state"),
                        }),
                conditions = _uiDefinition.GetProperty(propertyName: "items").EnumerateArray().Select(
                        selector: row => new { id = Text(value: row, name: "id"),
                            current = row.GetProperty(propertyName: "maximum") }),
            });
        }

        private void SelectUi()
        {
            _gateway.RequireThread();
            RequireActive();
            if (_inventory is not null)
            {
                _inventory.Open(id: _state);
            }
            else if (_camp is not null)
            {
                _camp.Open(id: _state);
            }
            else if (_session is not null)
            {
                _session.Open(id: _state);
            }
            else if (_gallery is not null)
            {
                _gallery.Project(snapshot: IdentitySnapshot());
                foreach (string name in new[] { "Bindings", "Statuses", "Players", "Items" })
                {
                    Surface.GetNode<Control>(path: "Scroll/Content/" + name).Visible = _id switch
                    {
                        "UI-03" => name is "Bindings",
                        "UI-04" => name is "Statuses",
                        "UI-07" => name is "Players",
                        _ => false,
                    };
                }
                if (_id is "UI-03")
                {
                    ProjectUiSymbol(parent: Surface, key: "interaction-cursor");
                }
            }
            else
            {
                SelectUiGallery();
            }
            Surface.SetMeta(name: "ui_library", value: JsonSerializer.Serialize(value: new
            {
                id = _id, state = _state, source = _uiSource, epoch = _epoch,
                controller = _inventory is not null ? "InventoryLayouts" : _camp is not null ? "CampLayouts"
                        : _session is not null ? "SessionLayouts"
                                : _gallery is not null ? "IdentityKit" : "Native gallery",
                fixture = _uiData.ValueKind is JsonValueKind.Object ? Text(value: _uiData, name: "id")
                        : Text(value: _uiDefinition, name: "id"),
            }));
        }

        private void SelectUiGallery()
        {
            _gateway.RequireThread();
            foreach (Node child in Surface.GetChildren())
            {
                Surface.RemoveChild(node: child);
                child.QueueFree();
            }
            var rows = new VBoxContainer { Name = "Gallery", CustomMinimumSize = new Vector2(x: 680, y: 0) };
            Surface.AddChild(node: rows);
            if (_id is "UI-08")
            {
                JsonElement desktop = _uiDefinition.GetProperty(propertyName: "desktop");
                string path = Text(value: desktop.GetProperty(propertyName: "sourceApplication"), name: "path");
                _ = UiBytes(path: path);
                rows.AddChild(node: new Label { Text = Text(value: desktop, name: "title") });
                ProjectUiSymbol(parent: rows, key: "title-emblem");
                rows.AddChild(node: new TextureRect { Name = "CurrentIcon",
                    Texture = ResourceLoader.Load<Texture2D>(path: "res://" + path),
                    CustomMinimumSize = new Vector2(x: 256, y: 256) });
            }
            else if (_id is "UI-02")
            {
                ProjectTypography(parent: rows);
            }
            else
            {
                foreach (JsonElement control in _uiDefinition.GetProperty(propertyName: "controls").EnumerateArray())
                {
                    string name = control.GetString() ?? throw new InvalidDataException(message: "RESOURCE_UI_CONTROL");
                    Node widget = UiWidget(name: name);
                    widget.Name = name;
                    widget.SetMeta(name: "ui_control", value: name);
                    rows.AddChild(node: widget);
                    if (widget is Control item)
                    {
                        item.CustomMinimumSize = new Vector2(x: 360, y: 40);
                        item.TooltipText = name + " · " + _state;
                    }
                }
            }
        }

        private Node UiWidget(string name)
        {
            _gateway.RequireThread();
            return name switch
            {
                "Button" => new Button { Text = name + " · " + _state, ToggleMode = true,
                    ButtonPressed = _state is "pressed", Disabled = _state is "disabled" },
                "PanelContainer" or "TooltipPanel" or "ToastPanel" => new PanelContainer
                {
                    ThemeTypeVariation = name,
                },
                "TabBar" => UiTabs(),
                "ItemList" => UiItems(),
                "HScrollBar" => new HScrollBar { MinValue = 0, MaxValue = 100, Page = 20, Value = 25 },
                "VScrollBar" => new VScrollBar { MinValue = 0, MaxValue = 100, Page = 20, Value = 25 },
                "LineEdit" => new LineEdit { Text = _state is "empty" ? "" : "KingdomZombies · " + _state,
                    PlaceholderText = "Empty input", Editable = _state is not "disabled" },
                "TextEdit" => new TextEdit { Text = "KingdomZombies · " + _state,
                    Editable = _state is not "disabled" },
                "HSlider" => new HSlider { MinValue = 0, MaxValue = 100, Value = 50,
                    Editable = _state is not "disabled" },
                "VSlider" => new VSlider { MinValue = 0, MaxValue = 100, Value = 50,
                    Editable = _state is not "disabled" },
                "CheckBox" => new CheckBox { Text = name + " · " + _state,
                    ButtonPressed = _state is "selected", Disabled = _state is "disabled" },
                "PopupPanel" => new PopupPanel(),
                _ => throw new InvalidDataException(message: "RESOURCE_UI_CONTROL " + name),
            };
        }

        private TabBar UiTabs()
        {
            _gateway.RequireThread();
            var tabs = new TabBar();
            tabs.AddTab(title: "Resources");
            tabs.AddTab(title: "Details");
            tabs.CurrentTab = _state is "selected" ? 1 : 0;
            return tabs;
        }

        private void ProjectUiSymbol(Node parent, string key)
        {
            _gateway.RequireThread();
            Node? old = parent.GetNodeOrNull<Node>(path: "SourceSymbol");
            old?.Free();
            foreach (AtlasTexture previous in _uiTextures)
            {
                previous.Dispose();
            }
            _uiTextures.Clear();
            string path = Text(value: _uiDefinition.GetProperty(propertyName: "atlas"), name: "path");
            _ = UiBytes(path: path);
            int[] region = _uiDefinition.GetProperty(propertyName: "regions").GetProperty(propertyName: key)
                    .EnumerateArray().Select(selector: value => value.GetInt32()).ToArray();
            var texture = new AtlasTexture { Atlas = ResourceLoader.Load<Texture2D>(path: "res://" + path),
                Region = new Rect2(x: region[0], y: region[1], width: region[2], height: region[3]),
                FilterClip = true };
            _uiTextures.Add(item: texture);
            var symbol = new TextureRect { Name = "SourceSymbol", Texture = texture,
                CustomMinimumSize = new Vector2(x: region[2], y: region[3]) };
            symbol.SetMeta(name: "ui_symbol", value: key);
            parent.AddChild(node: symbol);
        }

        private ItemList UiItems()
        {
            _gateway.RequireThread();
            var items = new ItemList();
            foreach (JsonElement fixture in _uiDefinition.GetProperty(propertyName: "fixtures").EnumerateArray())
            {
                items.AddItem(text: Text(value: fixture, name: "label"));
            }
            return items;
        }

        private void ProjectTypography(Node parent)
        {
            _gateway.RequireThread();
            foreach (JsonElement font in _uiDefinition.GetProperty(propertyName: "fonts").EnumerateArray())
            {
                string path = Text(value: font, name: "path");
                byte[] value = UiBytes(path: path);
                if (!Same(left: Convert.ToHexStringLower(inArray: SHA256.HashData(source: value)),
                        right: Text(value: font, name: "sha256")))
                {
                    throw new InvalidDataException(message: "RESOURCE_UI_FONT " + path);
                }
                foreach (JsonElement type in _uiDefinition.GetProperty(propertyName: "typography").EnumerateArray())
                {
                    var label = new Label { Text = Text(value: font, name: "role") + " · "
                            + Text(value: type, name: "role") + " · KingdomZombies Élodie 123" };
                    label.AddThemeFontOverride(name: "font", font: ResourceLoader.Load<Font>(path: "res://" + path));
                    label.AddThemeFontSizeOverride(name: "font_size", fontSize: type.GetProperty(
                            propertyName: "size").GetInt32());
                    label.SetMeta(name: "font_source", value: path);
                    parent.AddChild(node: label);
                }
            }
            foreach (JsonProperty color in _uiDefinition.GetProperty(propertyName: "palette").EnumerateObject())
            {
                parent.AddChild(node: new Label { Text = color.Name + " " + color.Value.GetString(),
                    Modulate = Color.FromHtml(rgba: color.Value.GetString() ?? "") });
            }
        }

        private void BuildCorpse()
        {
            _gateway.RequireThread();
            _fixture = Read(path: "content/fixtures/crowds.json");
            var owner = new Node2D { Name = "Corpse" };
            Art.AddChild(node: owner);
            _corpse = CorpsePreview.Open(root: owner, gateway: _gateway, read: _read);
            foreach (string state in new[] { "persistent corpse", "current death", "loot pile" })
            {
                AddState(state: state);
            }

            if (_id is "AN-09")
            {
                var character = new Node2D { Name = "Character", Position = new Vector2(x: -120, y: 0) };
                Art.AddChild(node: character);
                _character = CharacterPreview.Open(root: character, gateway: _gateway, read: _read, body: "CH-01");
                _character.Attach();
                _character.SelectObligation(id: "AN-09");
            }
            else
            {
                var pile = new Node2D { Name = "Pile", Position = new Vector2(x: 120, y: 0) };
                Art.AddChild(node: pile);
                _pile = CorpsePreview.Open(root: pile, gateway: _gateway, read: _read);
                JsonElement row = Actor(kind: "pile");
                _pile.Restore(body: "pile", kind: "pile", equipment: row.GetProperty(propertyName: "equipment"),
                        selected: false);
            }
        }

        private void BuildCrowd()
        {
            _gateway.RequireThread();
            _fixture = Read(path: "content/fixtures/crowds.json");
            _crowd = new CrowdPreview(root: Art, gateway: _gateway, read: _read);
            Crowd.Attach();
            if (_id is "CH-12")
            {
                foreach (JsonElement row in _fixture.GetProperty(propertyName: "actors").EnumerateArray()
                        .Where(predicate: row => row.GetProperty(propertyName: "body").GetString() is "CH-01"
                                && row.GetProperty(propertyName: "kind").GetString() is "alive").Take(count: 4))
                {
                    AddState(state: row.GetProperty(propertyName: "id").GetString()
                            ?? throw new InvalidDataException(message: "RESOURCE_ACTOR_ID"));
                }
            }
            else
            {
                JsonElement wildlife = Read(path: "content/packs/player-identities.json")
                        .GetProperty(propertyName: "wildlife");
                foreach (string body in _id is "AN-17" ? new[] { "rabbit", "deer" }
                        : new[] { _id is "CH-11" ? "deer" : "rabbit" })
                {
                    foreach (JsonProperty action in wildlife.GetProperty(propertyName: body).EnumerateObject())
                    {
                        AddState(state: body + ":" + action.Name);
                    }
                }
            }

            Art.Position = new Vector2(x: _id is "CH-12" ? 24 : -580, y: 100);
            FilterCrowd();
        }

        private void FilterCrowd()
        {
            _gateway.RequireThread();
            foreach (Node node in Art.GetNode(path: "Crowd").GetChildren())
            {
                if (node is CanvasItem item)
                {
                    string name = node.Name.ToString();
                    item.Visible = _id is "CH-12" ? name is "survivor-000" or "survivor-001" or "survivor-002"
                            or "survivor-003" : name is "Wildlife_rabbit" or "Wildlife_deer";
                }
            }
        }

        private JsonElement Actor(string kind)
        {
            return _fixture.GetProperty(propertyName: "actors").EnumerateArray().First(
                    predicate: row => StringComparer.Ordinal.Equals(
                            x: row.GetProperty(propertyName: "kind").GetString(),
                            y: kind));
        }

        private void BuildWorld()
        {
            _gateway.RequireThread();
            _map = Read(path: "content/world/map.json");
            _world = WorldPreview.Open(root: Art, gateway: _gateway, read: _read);
            _world.Attach();
            string[] samples = _id switch
            {
                "EN-04" => ["world.day", "world.night"],
                "EN-05" => ["clearings.intact", "clearings.marked-paid", "clearings.chopping",
                    "clearings.falling-left", "clearings.falling-right", "clearings.cleared-stump"],
                "EN-06" or "EN-07" => ["plants.full", "plants.partial", "plants.empty", "plants.refilled"],
                "EN-10" => ["map.shared-exploration", "map.unknown-portals"],
                _ => ["world.day", "world.night"],
            };
            foreach (string state in samples)
            {
                AddState(state: state);
            }
        }

        private void Changed(long index)
        {
            _gateway.RequireThread();
            RequireActive();
            if (_choices is null || index < 0 || index >= Choices.ItemCount)
            {
                throw new InvalidDataException(message: "RESOURCE_VIEW_SELECTOR " + _id);
            }

            SelectState();
        }

        private void AddState(string state)
        {
            _gateway.RequireThread();
            if (state.Length == 0 || _states.Contains(value: state, comparer: StringComparer.Ordinal))
            {
                throw new InvalidDataException(message: "RESOURCE_VIEW_STATE " + _id);
            }

            _states.Add(item: state);
            Choices.AddItem(label: state);
        }

        private void Seek(double value)
        {
            _gateway.RequireThread();
            RequireActive();
            if (!double.IsFinite(d: value) || value is < 0 or > 60)
            {
                throw new InvalidDataException(message: "RESOURCE_VIEW_TIME " + _id);
            }

            SelectState();
        }

        private void Play(bool value)
        {
            _gateway.RequireThread();
            RequireActive();
            _playing = value;
        }

        private void Step()
        {
            _gateway.RequireThread();
            RequireActive();
            Clock.Value = Math.Min(val1: 60, val2: Clock.Value + 0.125);
        }

        private void SelectState()
        {
            _gateway.RequireThread();
            RequireActive();
            if (Choices.Selected < 0 || Choices.Selected >= _states.Count)
            {
                throw new InvalidDataException(message: "RESOURCE_VIEW_SELECTOR " + _id);
            }
            _state = Choices.GetItemText(idx: Choices.Selected);
            if (!StringComparer.Ordinal.Equals(x: _state, y: _states[Choices.Selected]))
            {
                throw new InvalidDataException(message: "RESOURCE_VIEW_SELECTOR " + _id);
            }
            _epoch = checked(_epoch + 1);
            if (UiHandles(id: _id))
            {
                SelectUi();
            }
            else if (_world is not null)
            {
                _world.Restore(sample: _state);
                _world.Advance(elapsedSeconds: Clock.Value);
                FilterWorld();
            }
            else if (_crowd is not null)
            {
                SelectCrowd();
            }
            else if (_corpse is not null)
            {
                SelectCorpse();
            }

            _root.SetMeta(name: "resource_view", value: JsonSerializer.Serialize(value: new
            {
                id = _id,
                state = _state,
                epoch = _epoch,
                closed = _closed,
                time = Clock.Value,
                controller = UiHandles(id: _id) ? "Source-bound UI presenter" : _world is not null ? "WorldPreview"
                        : _crowd is not null ? "CrowdPreview" : "CorpsePreview",
                corpse = _corpse?.Snapshot,
                crowd = _crowd?.Snapshot,
                source = UiHandles(id: _id) ? _uiSource
                        : _world is not null ? "content/world/map.json" : "content/fixtures/crowds.json",
            }));
            _root.GetNode<Label>(path: "Controls/Identity").Text = _id + " · " + _state + " · " + _status;
        }

        private void SelectCrowd()
        {
            _gateway.RequireThread();
            if (_id is "CH-12")
            {
                string from = Crowd.Snapshot.GetProperty(propertyName: "controlled").GetString() ?? "";
                if (!StringComparer.Ordinal.Equals(x: from, y: _state))
                {
                    Crowd.TransferControl(from: from, to: _state);
                }
                Crowd.Select(id: _state);
            }
            else
            {
                string[] selection = _state.Split(separator: ':');
                Crowd.Wildlife(body: selection[0], action: selection[1], elapsedSeconds: Clock.Value,
                        facing: 1, selected: true);
            }
        }

        private void SelectCorpse()
        {
            _gateway.RequireThread();
            bool pile = _state is "loot pile";
            JsonElement row = Actor(kind: pile ? "pile" : "corpse");
            Corpse.Restore(body: pile ? "pile" : row.GetProperty(propertyName: "body").GetString()
                    ?? throw new InvalidDataException(message: "RESOURCE_CORPSE_BODY"),
                    kind: pile ? "pile" : _state is "current death" ? "alive" : "corpse",
                    equipment: row.GetProperty(propertyName: "equipment"), selected: true);
            if (_state is "current death")
            {
                Corpse.TriggerDeath();
                Corpse.Advance(elapsedSeconds: Clock.Value);
            }
        }

        private void FilterWorld()
        {
            _gateway.RequireThread();
            foreach (Node node in Art.GetNode(path: "WorldPreview").GetChildren())
            {
                if (node is Sprite2D sprite)
                {
                    string part = sprite.HasMeta(name: "part") ? sprite.GetMeta(name: "part").AsString() : "";
                    string id = sprite.HasMeta(name: "resource_id")
                            ? sprite.GetMeta(name: "resource_id").AsString() : "";
                    bool selected = EnvironmentPart(part: part, id: id);
                    sprite.Visible = sprite.Visible && selected;
                }
            }
            if (_id is "EN-08")
            {
                ProjectZones();
            }
        }

        private void ProjectZones()
        {
            _gateway.RequireThread();
            foreach (JsonElement zone in _map.GetProperty(propertyName: "wildlifeZones").EnumerateArray())
            {
                float x = zone.GetProperty(propertyName: "center").GetSingle() * 8;
                float half = zone.GetProperty(propertyName: "width").GetSingle() * 4;
                string id = zone.GetProperty(propertyName: "id").GetString()
                        ?? throw new InvalidDataException(message: "RESOURCE_ZONE_ID");
                var line = new Line2D
                {
                    Name = "Zone_" + id.Replace(oldValue: ".", newValue: "_", comparisonType: StringComparison.Ordinal),
                    Points = [new Vector2(x: x - half, y: -24), new Vector2(x: x + half, y: -24),
                        new Vector2(x: x + half, y: 8), new Vector2(x: x - half, y: 8),
                        new Vector2(x: x - half, y: -24)],
                    Width = 1,
                    DefaultColor = new Color(r: 0.45f, g: 0.74f, b: 0.85f, a: 0.6f),
                };
                line.SetMeta(name: "zone", value: zone.GetRawText());
                Art.GetNode(path: "WorldPreview").AddChild(node: line);
            }
        }

        private bool EnvironmentPart(string part, string id)
        {
            return _id switch
            {
                "EN-01" or "EN-04" => true,
                "EN-02" => part is "ground" or "grass" or "roots" or "contact-shadow",
                "EN-03" => part is "hill" or "cloud" or "trunk-oak" or "trunk-birch" or "leaves-oak" or "leaves-birch",
                "EN-05" => _map.GetProperty(propertyName: "clearings").EnumerateArray().Any(
                        predicate: row => id.StartsWith(value: row.GetProperty(propertyName: "id").GetString() + ".",
                                comparisonType: StringComparison.Ordinal)),
                "EN-06" => part.StartsWith(value: "berry-", comparisonType: StringComparison.Ordinal),
                "EN-07" => part.StartsWith(value: "fruit-", comparisonType: StringComparison.Ordinal),
                "EN-08" => part is "rabbit" or "deer",
                "EN-09" => part is "tree-mark" or "banner-0" or "banner-pole",
                "EN-10" => id.StartsWith(value: "minimap.", comparisonType: StringComparison.Ordinal),
                "EN-11" => part is "rocks" or "log" or "stump" or "roots" or "grass",
                _ => throw new InvalidDataException(message: "RESOURCE_ENVIRONMENT_ID " + _id),
            };
        }
    }

}
