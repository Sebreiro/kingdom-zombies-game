using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Owns bounded read-only text, field and quiet saved-appearance inspection.</summary>
internal sealed class DataTextPreview : IDisposable
{
    /// <summary>Revokes this preview before the shell retires its native controls.</summary>
    public void Dispose()
    {
        Close();
    }

    internal const string Unknown = "Unavailable resource; choose a listed ID";
    internal const string Shared = "\nExplicit shared mappings: ";

    internal DataTextPreview(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read, string id)
    {
        _root = root;
        _gateway = gateway;
        _gateway.RequireThread();
        _read = read;
        _id = id;
        try
        {
            Load();
        }
        catch (Exception error) when (error is InvalidDataException or JsonException or FormatException
                or IOException or KeyNotFoundException or InvalidOperationException)
        {
            _failure = error.Message;
        }

        Build();
        Project();
    }

    internal string Status => _failure.Length > 0 ? "Unavailable — " + _failure
            : "Partial — read-only data/text; actual readability acceptance pending";

    internal static string[] Ids(string category)
    {
        return category is "Data/text" or "" ? ["TXT-01", "TXT-02", "DT-18", "DT-24", "DT-25", "DT-26", "DT-27",
            .. Enumerable.Range(start: 1, count: 17).Select(selector: number => "DT-"
                    + number.ToString(format: "00", provider: CultureInfo.InvariantCulture)),
            "DT-19", "DT-20", "DT-21", "DT-22", "DT-23"] : [];
    }

    internal static bool Handles(string id)
    {
        return Ids(category: "Data/text").Contains(value: id, comparer: StringComparer.Ordinal);
    }

    internal static JsonElement UiKitDocument(byte[] bytes)
    {
        JsonElement data = Parse(bytes: bytes, file: "content/packs/ui-kit.json");
        Require(valid: Text(value: data, key: "id") is "ui-kit"
                && data.GetProperty(propertyName: "schemaVersion").GetInt32() == 1
                && data.GetProperty(propertyName: "version").TryGetInt32(value: out int version) && version > 0,
                reason: "ui-kit document identity");
        return data;
    }

    internal static DataTextPreview? Open(InspectionShell? shell, MainThreadGateway gateway,
            Func<string, byte[]?> read, string id, Node2D owner)
    {
        gateway.RequireThread();
        if (!Handles(id: id))
        {
            return null;
        }

        if (shell is null)
        {
            throw new InvalidOperationException(message: "Data/text requires an attached inspection shell");
        }

        var root = new Node2D { Name = "DataText" };
        shell.Install(preview: root);
        var preview = new DataTextPreview(root: root, gateway: gateway, read: read, id: id);
        owner.GetNode<Label>(path: "Summary/Status").Text = preview.Status;
        return preview;
    }

    internal void Close()
    {
        _gateway.RequireThread();
        if (_closed)
        {
            return;
        }

        _closed = true;
        if (_body is not null)
        {
            foreach ((string name, OptionButton.ItemSelectedEventHandler handler) in _selectors)
            {
                _body.GetNode<OptionButton>(path: "Choices/" + name).ItemSelected -= handler;
            }
        }
        _selectors.Clear();
        _audio?.Close();
        _audio = null;
        _world?.Close();
        _world = null;
        _building?.Detach();
        _building = null;
        _appearance?.Reset();
        _appearance = null;
    }

    internal void Tick(double delta)
    {
        _gateway.RequireThread();
        if (!_closed)
        {
            _audio?.Tick(delta: delta);
            _world?.Advance(elapsedSeconds: delta);
            _building?.Tick(delta: delta);
        }
    }

    private const string PackPath = "content/packs/text-data.json";
    private const string CompatibilityPath = "content/compatibility/appearance-layout.json";
    private static readonly string[] _states = ["empty", "loading", "error", "populated", "partial",
        "overflow", "zero-one-many", "long-text"];
    private static readonly int[] _counts = [0, 1, 240];
    private readonly Node2D _root;
    private readonly MainThreadGateway _gateway;
    private readonly Func<string, byte[]?> _read;
    private readonly string _id;
    private readonly Dictionary<string, JsonElement> _documents = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, JsonElement> _messages = new(comparer: StringComparer.Ordinal);
    private readonly List<(string File, JsonElement Field)> _fields = [];
    private string _documentation = "";
    private string _support = "";
    private readonly List<(string File, JsonElement Data)> _libraries = [];
    private readonly Dictionary<string, OptionButton.ItemSelectedEventHandler> _selectors =
            new(comparer: StringComparer.Ordinal);
    private string _failure = "";
    private string _state = "populated";
    private bool _expanded;
    private bool _closed;
    private long _fieldIndex;
    private long _sampleIndex;
    private VBoxContainer? _body;
    private TextEdit? _detail;
    private Label? _identity;
    private AppearanceState? _appearance;
    private WorldPreview? _world;
    private BuildingPreview? _building;
    private AudioPreview? _audio;

    private JsonElement Read(string file)
    {
        string safe = WorldPreview.InspectorResourcePath(path: file);
        byte[] value = _read(safe) ?? throw new InvalidDataException(message: "Missing definition " + _id
                + ": " + safe + ", field bytes; choose Recheck resources");
        return Parse(bytes: value, file: safe);
    }

    private static JsonElement Parse(byte[] bytes, string file)
    {
        Require(valid: bytes.Length is > 0 and <= 1048576 && bytes[0] != 239, reason: file + " bytes bound/BOM");
        string text;
        try
        {
            text = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                    .GetString(bytes: bytes);
        }
        catch (DecoderFallbackException error)
        {
            throw new InvalidDataException(message: file + ", field bytes, value invalid UTF-8 — " + error.Message,
                    innerException: error);
        }
        using var document = JsonDocument.Parse(json: text, options: new JsonDocumentOptions { MaxDepth = 24 });
        int count = 0;
        Check(value: document.RootElement, count: ref count);
        return document.RootElement.Clone();
    }

    private static void Check(JsonElement value, ref int count)
    {
        Require(valid: ++count <= 100000, reason: "data/text node bound");
        if (value.ValueKind is JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                Require(valid: keys.Add(item: property.Name) && property.Name is not "__proto__"
                        and not "constructor" and not "prototype", reason: "duplicate/unsafe field " + property.Name);
                Check(value: property.Value, count: ref count);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            foreach (JsonElement item in value.EnumerateArray())
            {
                Check(value: item, count: ref count);
            }
        }
        else if (value.ValueKind is JsonValueKind.String)
        {
            string text = value.GetString() ?? "";
            Require(valid: text.Length <= 1048576 && text.IsNormalized(normalizationForm: NormalizationForm.FormC),
                    reason: "data/text string identity");
        }
        else if (value.ValueKind is JsonValueKind.Number)
        {
            Require(valid: value.TryGetDouble(value: out double number) && double.IsFinite(d: number),
                    reason: "data/text finite number");
        }
    }

    private void Load()
    {
        JsonElement pack = Read(file: PackPath);
        Require(valid: Text(value: pack, key: "id") is "text-data", reason: PackPath + " /id");
        foreach (JsonElement binding in pack.GetProperty(propertyName: "bindings").EnumerateArray())
        {
            string file = WorldPreview.InspectorResourcePath(path: Text(value: binding, key: "path"));
            byte[] value = _read(file) ?? throw new InvalidDataException(message: "Missing definition " + _id
                    + ": " + file + ", field bytes; choose Recheck resources");
            Require(valid: Same(left: Digest(bytes: value), right: Text(value: binding, key: "sha256")),
                    reason: file + ", field sha256, value current bytes — hash mismatch");
            _documents.Add(key: file, value: Parse(bytes: value, file: file));
        }

        _documentation = Documentation(value: pack.GetProperty(propertyName: "documentation"));
        foreach ((string file, JsonElement data) in _documents)
        {
            if (file.Contains(value: "/schema/", comparisonType: StringComparison.Ordinal))
            {
                Fields(schema: data);
            }
            else if (data.TryGetProperty(propertyName: "fieldDocumentation", value: out JsonElement schema))
            {
                Fields(schema: schema);
            }
        }

        LoadMessages(expanded: _id is "TXT-02");
        int fieldIndex = _fields.FindIndex(match: field => _documents.TryGetValue(
                key: field.File["src/godot-game/".Length..], value: out JsonElement definition)
                && Same(left: Text(value: definition, key: "id"), right: _id));
        _fieldIndex = Math.Max(val1: 0, val2: fieldIndex);
        if (_id is "DT-24")
        {
            Compatibility();
        }

        if (_id is "DT-26" or "DT-27")
        {
            Support();
        }
        LoadLibraries();
    }

    private void LoadLibraries()
    {
        string[] files = _id switch
        {
            "DT-19" => ["content/packs/delivery.json", "content/packs/compatibility.json"],
            "DT-20" => ["content/animation/locomotion-melee.json", "content/animation/bow.json",
                "content/animation/work-special.json", "content/animation/fits-a.json",
                "content/animation/fits-b.json", "content/animation/fits-c.json"],
            "DT-21" => ["content/world/map.json", "content/world/main-assemblies.json",
                "content/world/defense-assemblies.json", "content/world/portal-assemblies.json"],
            "DT-22" => ["content/packs/ui-kit.json", "content/packs/ui-identity.json",
                "content/packs/ui-inventory.json", "content/packs/ui-legacy-stash.json",
                "content/packs/ui-camp.json", "content/packs/ui-map-warnings.json",
                "content/packs/ui-session.json", "content/packs/ui-settings.json"],
            "DT-23" => ["content/audio/events.json", "content/packs/audio-actions.json",
                "content/packs/audio-interface-ambience.json"],
            _ => [],
        };
        foreach (string file in files)
        {
            _libraries.Add(item: (file, Read(file: file)));
        }
        if (_libraries.Count > 0)
        {
            _fieldIndex = 0;
        }
    }

    private void Support()
    {
        JsonElement delivery = Read(file: "content/packs/delivery.json");
        Require(valid: delivery.GetProperty(propertyName: "schemaVersion").GetInt32() == 1
                && Text(value: delivery, key: "id") is "delivery"
                && delivery.GetProperty(propertyName: "version").GetInt32() > 0,
                reason: "content/packs/delivery.json, field schemaVersion/id/version");
        Require(valid: delivery.GetProperty(propertyName: "acceptedObligations").GetArrayLength() == 0
                && delivery.GetProperty(propertyName: "acceptance").GetProperty(propertyName: "completedObligations")
                        .GetArrayLength() == 0, reason: "delivery acceptedObligations: actual acceptance pending");
        JsonElement text = delivery.GetProperty(propertyName: "textPackBinding");
        Require(valid: Text(value: text, key: "path") is PackPath && Same(left: Text(value: text, key: "sha256"),
                right: Digest(bytes: _read(PackPath) ?? [])), reason: "delivery textPackBinding sha256");
        string catalog = Documentation(value: delivery.GetProperty(propertyName: "fixtureCatalog"));
        JsonElement fixtures = Parse(bytes: Encoding.UTF8.GetBytes(s: catalog), file: "delivery fixture catalog");
        SupportCatalog(catalog: fixtures);
        SupportClosure(catalog: fixtures, delivery: delivery);
        string guides = SupportGuides(delivery: delivery);
        var details = new StringBuilder(value: _id + " — read-only source support; actual acceptance pending.\n");
        details.Append(value: _id is "DT-26" ? catalog : guides);
        foreach (JsonElement resource in delivery.GetProperty(propertyName: "fixtureResources").EnumerateArray())
        {
            details.Append(value: "\nAuthenticated fixture: " + SupportResource(resource: resource));
        }

        details.Append(value: "\nPlatform records: " + delivery.GetProperty(propertyName: "platformRecords")
                .GetRawText() + "\nDesigner field documentation: DT-18, content/packs/text-data.json; SHA256 "
                + Digest(bytes: Encoding.UTF8.GetBytes(s: _documentation)));
        _support = details.ToString();
    }

    private static void SupportCatalog(JsonElement catalog)
    {
        Require(valid: catalog.GetProperty(propertyName: "schemaVersion").GetInt32() == 1
                && Text(value: catalog, key: "id") is "phase4-fixture-catalog",
                reason: "delivery fixture catalog schemaVersion/id");
        foreach ((string key, int count) in new (string, int)[] { ("heldout", 34), ("backstops", 70),
                ("acceptance", 24), ("decisions", 32), ("judgments", 4), ("fixtures", 9) })
        {
            JsonElement rows = catalog.GetProperty(propertyName: key);
            var identities = new HashSet<string>(comparer: StringComparer.Ordinal);
            Require(valid: rows.GetArrayLength() == count, reason: "delivery fixture catalog " + key + " count");
            foreach (JsonElement row in rows.EnumerateArray())
            {
                Require(valid: identities.Add(item: Text(value: row, key: "id")),
                        reason: "delivery fixture catalog duplicate " + key);
            }
        }

        Require(valid: catalog.GetProperty(propertyName: "actualAcceptedObligations").GetInt32() == 0,
                reason: "delivery fixture catalog actualAcceptedObligations");
    }

    private string SupportResource(JsonElement resource)
    {
        string file = Text(value: resource, key: "path");
        Require(valid: file.StartsWith(value: "src/godot-game/", comparisonType: StringComparison.Ordinal),
                reason: "delivery fixture path " + file);
        string safe = WorldPreview.InspectorResourcePath(path: file["src/godot-game/".Length..]);
        byte[] value = _read(safe) ?? throw new InvalidDataException(message: "Missing definition " + _id
                + ": " + safe + ", field bytes; choose Recheck resources");
        Require(valid: value.Length == resource.GetProperty(propertyName: "bytes").GetInt32()
                && Same(left: Digest(bytes: value), right: Text(value: resource, key: "sha256")),
                reason: safe + ", field fixture sha256/bytes");
        JsonElement actual = Parse(bytes: value, file: safe);
        return safe + " — " + Text(value: actual, key: "id") + " — SHA256 " + Digest(bytes: value);
    }

    private static void SupportClosure(JsonElement catalog, JsonElement delivery)
    {
        JsonElement[] expected = [.. catalog.GetProperty(propertyName: "fixtures").EnumerateArray()
                .SelectMany(selector: fixture => fixture.GetProperty(propertyName: "resources").EnumerateArray())
                .DistinctBy(keySelector: resource => Text(value: resource, key: "path"),
                        comparer: StringComparer.Ordinal)];
        JsonElement actual = delivery.GetProperty(propertyName: "fixtureResources");
        Require(valid: actual.GetArrayLength() == expected.Length, reason: "delivery fixture resource closure");
        for (int index = 0; index < expected.Length; index++)
        {
            Require(valid: Same(left: Text(value: actual[index], key: "path"),
                    right: Text(value: expected[index], key: "path"))
                    && Same(left: Text(value: actual[index], key: "sha256"),
                            right: Text(value: expected[index], key: "sha256"))
                    && actual[index].GetProperty(propertyName: "bytes").GetInt32()
                            == expected[index].GetProperty(propertyName: "bytes").GetInt32(),
                    reason: "delivery fixture sha256/path/bytes closure");
        }
    }

    private static string SupportGuides(JsonElement delivery)
    {
        var text = new StringBuilder(value: "DT-27 Desktop delivery source guides; final export pending.\n");
        foreach (JsonElement guide in delivery.GetProperty(propertyName: "guides").EnumerateArray())
        {
            string value = Text(value: guide, key: "text");
            byte[] bytes = Encoding.UTF8.GetBytes(s: value);
            Require(valid: Text(value: guide, key: "encoding") is "UTF-8"
                    && bytes.Length == guide.GetProperty(propertyName: "bytes").GetInt32()
                    && Same(left: Digest(bytes: bytes), right: Text(value: guide, key: "sha256")),
                    reason: Text(value: guide, key: "path") + ", field guide sha256/bytes");
            text.Append(value: "\n" + Text(value: guide, key: "path") + "\n" + value);
        }

        return text.ToString();
    }

    private static string Documentation(JsonElement value)
    {
        int expected = value.GetProperty(propertyName: "uncompressedBytes").GetInt32();
        Require(valid: expected is > 0 and <= 16777216, reason: "documentation decoded bound");
        byte[] compressed = Convert.FromBase64String(s: Text(value: value, key: "gzipBase64"));
        using var input = new MemoryStream(buffer: compressed);
        using var gzip = new GZipStream(stream: input, mode: CompressionMode.Decompress);
        using var output = new MemoryStream(capacity: expected);
        byte[] chunk = new byte[8192];
        int count;
        while ((count = gzip.Read(buffer: chunk, offset: 0, count: chunk.Length)) > 0)
        {
            Require(valid: output.Length + count <= expected, reason: "documentation expanded size");
            output.Write(buffer: chunk, offset: 0, count: count);
        }

        byte[] bytes = output.ToArray();
        Require(valid: bytes.Length == expected && Same(left: Digest(bytes: bytes),
                right: Text(value: value, key: "sha256")), reason: "documentation decoded hash/size");
        return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: bytes);
    }

    private void Fields(JsonElement schema)
    {
        foreach (JsonElement file in schema.GetProperty(propertyName: "files").EnumerateArray())
        {
            foreach (JsonElement field in file.GetProperty(propertyName: "fields").EnumerateArray())
            {
                _fields.Add(item: (Text(value: file, key: "path"), field));
            }
        }
    }

    private void LoadMessages(bool expanded)
    {
        _expanded = expanded;
        _messages.Clear();
        string file = expanded ? "content/text/expanded.json" : "content/text/en.json";
        foreach (JsonElement row in _documents[file].GetProperty(propertyName: "rows").EnumerateArray())
        {
            Require(valid: _messages.TryAdd(key: Text(value: row, key: "id"), value: row),
                    reason: file + " duplicate message ID");
        }
    }

    private void Compatibility()
    {
        JsonElement pack = Read(file: "content/packs/compatibility.json");
        foreach (JsonElement binding in pack.GetProperty(propertyName: "bindings").EnumerateArray())
        {
            string file = Text(value: binding, key: "path");
            byte[] bytes = _read(WorldPreview.InspectorResourcePath(path: file))
                    ?? throw new InvalidDataException(message: "Missing definition DT-24: " + file + ", field bytes");
            Require(valid: Same(left: Digest(bytes: bytes), right: Text(value: binding, key: "sha256")),
                    reason: file + " field sha256 — mismatch");
            if (!_documents.ContainsKey(key: file))
            {
                _documents.Add(key: file, value: Parse(bytes: bytes, file: file));
            }
        }

        Require(valid: Text(value: _documents[CompatibilityPath], key: "id") is "DT-24",
                reason: CompatibilityPath + " /id");
    }

    private void Build()
    {
        _gateway.RequireThread();
        _body = new VBoxContainer { Name = "Body", Position = new Vector2(x: 8, y: 8),
            Size = new Vector2(x: 820, y: 430) };
        _root.AddChild(node: _body);
        _identity = new Label { Name = "Identity", AutowrapMode = TextServer.AutowrapMode.WordSmart };
        _body.AddChild(node: _identity);
        var choices = new HFlowContainer { Name = "Choices" };
        _body.AddChild(node: choices);
        Selector(parent: choices, name: "State", items: _states, selected: 3, changed: State);
        Selector(parent: choices, name: "TextLength", items: ["English", "Expanded replacement"],
                selected: _expanded ? 1 : 0, changed: TextLength);
        string[] fields = _libraries.Count > 0 ? [.. _libraries.Select(selector: row => row.File)]
                : [.. _fields.Select(selector: field => field.File.Split(separator: '/')[^1]
                        + " " + Text(value: field.Field, key: "path"))];
        Selector(parent: choices, name: "Field", items: fields.Length > 0 ? fields : ["Unavailable fields"],
                selected: (int)_fieldIndex, changed: Field);
        if (_id is "DT-24" && _failure.Length == 0)
        {
            Selector(parent: choices, name: "Sample", items: [.. _documents[CompatibilityPath]
                    .GetProperty(propertyName: "samples").EnumerateArray().Select(selector: row =>
                            Text(value: row, key: "id"))], selected: 0, changed: Sample);
        }

        _detail = new TextEdit { Name = "Detail", Editable = false,
            WrapMode = TextEdit.LineWrappingMode.Boundary, CustomMinimumSize = new Vector2(x: 780, y: 280),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, SizeFlagsVertical = Control.SizeFlags.ExpandFill };
        _body.AddChild(node: _detail);
    }

    private void Selector(Node parent, string name, string[] items, int selected,
            OptionButton.ItemSelectedEventHandler changed)
    {
        _gateway.RequireThread();
        var control = new OptionButton { Name = name, CustomMinimumSize = new Vector2(x: 180, y: 48),
            ClipText = true };
        foreach (string item in items)
        {
            control.AddItem(label: item);
        }

        control.Select(idx: selected);
        parent.AddChild(node: control);
        control.ItemSelected += changed;
        _selectors.Add(key: name, value: changed);
    }

    private void State(long index)
    {
        _gateway.RequireThread();
        if (!_closed && index >= 0 && index < _states.Length)
        {
            _state = _states[index];
            Project();
        }
    }

    private void TextLength(long index)
    {
        _gateway.RequireThread();
        if (!_closed && _failure.Length == 0)
        {
            LoadMessages(expanded: index == 1);
            Project();
        }
    }

    private void Field(long index)
    {
        _gateway.RequireThread();
        if (!_closed && index >= 0 && index < (_libraries.Count > 0 ? _libraries.Count : _fields.Count))
        {
            _fieldIndex = index;
            Project();
        }
    }

    private void Sample(long index)
    {
        _gateway.RequireThread();
        if (!_closed && _id is "DT-24" && index >= 0 && index < _documents[CompatibilityPath]
                .GetProperty(propertyName: "samples").GetArrayLength())
        {
            _sampleIndex = index;
            Project();
        }
    }

    private void Project()
    {
        _gateway.RequireThread();
        if (_closed || _detail is null || _identity is null)
        {
            return;
        }

        _identity.Text = _id + " · " + _state + " · " + Status;
        ProjectLibrary();
        if (_failure.Length > 0)
        {
            _detail.Text = _failure + "\nRestore the exact definition, then choose Recheck resources.";
            return;
        }

        _detail.WrapMode = _state is "overflow" ? TextEdit.LineWrappingMode.None
                : TextEdit.LineWrappingMode.Boundary;
        _detail.Text = _state switch
        {
            "empty" => "No fields available. Choose another definition.\n0 results",
            "loading" => "Loading " + _id + "…\nPrevious candidate is not current.",
            "error" => "Missing definition TXT-unknown: content/text/en.json, field /rows/TXT-unknown. "
                    + "No substitute is used. Restore the exact definition, then choose Recheck resources.",
            "partial" => "Partial data: available definitions remain usable.\nMissing definition TXT-unknown: "
                    + "content/text/en.json, field /rows/TXT-unknown.\n" + Current(),
            "zero-one-many" => Counts(),
            "long-text" => LongText(),
            "overflow" => _documentation + "\n" + new string(c: 'x', count: 2048),
            _ => Current(),
        };
        if (_id is "DT-24" && _state is "populated")
        {
            Restore();
        }
    }

    private string Current()
    {
        if (_libraries.Count > 0)
        {
            (string libraryFile, JsonElement libraryData) = _libraries[(int)_fieldIndex];
            return _id + " · Authenticated current resource library\nSource: " + libraryFile
                    + "\nRead-only source records; export, platform and judgment acceptance remain pending.\n"
                    + libraryData.GetRawText();
        }
        if (_id is "DT-26" or "DT-27")
        {
            return _support;
        }

        string authority = "Host balance — shared by the run\nLocal preferences — this player only; "
                + "DT-25 never overrides host balance.\nRead-only inspection.\n";
        if (_id is "DT-18")
        {
            return authority + _documentation;
        }

        if (_id is "TXT-01" or "TXT-02")
        {
            return authority + string.Join(separator: "\n\n", values: _messages.Select(selector: pair =>
                    pair.Key + ": " + pair.Value.GetRawText())) + "\nCurrent binding example: "
                    + Format(id: "binding.interact", count: 1);
        }

        (string file, JsonElement field) = _fields[(int)_fieldIndex];
        return authority + "Source: " + file + "\nMeaning / unit / shipped default / allowed / dependencies "
                + "/ source refs:\n" + field.GetRawText() + "\n" + Format(id: "data.read-only", count: 1)
                + "\nIndependently editable progression: "
                + "content/config/zombie-progression.json\n" + _documents["content/config/zombie-progression.json"]
                        .GetRawText() + (_id is "DT-24" ? "\nSaved/current layout:\n" + _documents[CompatibilityPath]
                                .GetProperty(propertyName: "samples")[(int)_sampleIndex].GetRawText() : "");
    }

    private void ProjectLibrary()
    {
        _gateway.RequireThread();
        if (_libraries.Count > 0)
        {
            (string file, JsonElement data) = _libraries[(int)_fieldIndex];
            _root.SetMeta(name: "data_library", value: JsonSerializer.Serialize(value: new
            {
                id = _id, source = file, sources = _libraries.Select(selector: row => row.File).ToArray(),
                sourceId = Text(value: data, key: "id"), readOnly = true,
            }));
        }
    }

    private string Format(string id, int count)
    {
        Require(valid: count >= 0 && _messages.TryGetValue(key: id, value: out _),
                reason: "Missing definition " + id + ": content/text/en.json, field /rows");
        JsonElement row = _messages[id];
        string text = Text(value: row, key: "text");
        if (row.TryGetProperty(propertyName: "plural", value: out JsonElement plural))
        {
            text = Text(value: plural, key: count == 0 ? "zero" : count == 1 ? "one" : "other");
        }

        text = text.Replace(oldValue: "{count}", newValue: count.ToString(provider: CultureInfo.InvariantCulture),
                comparisonType: StringComparison.Ordinal);
        if (row.TryGetProperty(propertyName: "bindingRef", value: out JsonElement action))
        {
            JsonElement binding = _documents["content/config/local-preferences.json"]
                    .GetProperty(propertyName: "bindings").EnumerateArray().Single(predicate: value =>
                            Same(left: Text(value: value, key: "action"), right: action.GetString() ?? ""));
            text = text.Replace(oldValue: "{binding}", newValue: string.Join(separator: " / ",
                    values: binding.GetProperty(propertyName: "defaults").EnumerateArray().Select(
                            selector: value => value.GetString() ?? "")), comparisonType: StringComparison.Ordinal);
        }

        return text;
    }

    private string Counts()
    {
        return string.Join(separator: "\n", values: _counts.Select(selector: count =>
                Format(id: "count.items", count: count))) + "\n" + Format(id: "binding.interact", count: 1);
    }

    private string LongText()
    {
        JsonElement fixtures = _documents["content/text/expanded.json"].GetProperty(propertyName: "fixtures");
        return "Élodie · Jérôme · İpek · Zoë\n" + Text(value: fixtures, key: "multiline") + "\n"
                + Text(value: fixtures, key: "longId") + "\n" + Text(value: fixtures, key: "glyphStatus");
    }

    private void Restore()
    {
        _gateway.RequireThread();
        TextEdit detail = _detail ?? throw new InvalidOperationException(message: "data/text not attached");
        JsonElement sample = _documents[CompatibilityPath].GetProperty(propertyName: "samples")[(int)_sampleIndex];
        try
        {
            _appearance ??= new AppearanceState(definition: _read("content/packs/corpses-wildlife.json")
                    ?? throw new InvalidDataException(message: "content/packs/corpses-wildlife.json field bytes"));
            _appearance.Restore(body: Text(value: sample, key: "body"), kind: Text(value: sample, key: "kind"),
                    equipment: sample.GetProperty(propertyName: "equipment"), selected: true);
            RestoreOwners(sample: sample);
            Geometry(sample: sample);
            detail.Text += "\nQuiet AppearanceState: " + _appearance.Snapshot.GetRawText()
                    + "\nWorld current sample: " + _world?.Sample + "; historical events: "
                    + _world?.EventCount.ToString(provider: CultureInfo.InvariantCulture)
                    + "; work/payment/queues/exploration retained exactly; current loops restored muted.";
        }
        catch (InvalidDataException error)
        {
            string item = sample.GetProperty(propertyName: "equipment").EnumerateObject().First().Value
                    .GetString() ?? "";
            detail.Text += "\nMissing definition " + item + ": " + CompatibilityPath + ", field /samples/"
                    + _sampleIndex.ToString(provider: CultureInfo.InvariantCulture)
                    + "/equipment — " + error.Message + "; no substitute; restore exact definition.";
        }
    }

    private void RestoreOwners(JsonElement sample)
    {
        _gateway.RequireThread();
        if (_world is null)
        {
            var root = new Node2D { Name = "CurrentWorld", Visible = false };
            _root.AddChild(node: root);
            _world = WorldPreview.Open(root: root, gateway: _gateway, read: _read);
            _world.Attach();
        }

        _world.Restore(sample: "portal.ruins");
        if (_audio is null)
        {
            var root = new Node2D { Name = "CurrentLoops", Visible = false };
            _root.AddChild(node: root);
            _audio = new AudioPreview(root: root, gateway: _gateway, read: _read, category: "AU-28");
            root.GetNode<CheckButton>(path: "Audio/Content/Mute").ButtonPressed = true;
        }

        _audio.Restore(routes: [.. sample.GetProperty(propertyName: "loops").EnumerateArray()
                .Select(selector: value => value.GetString() ?? "")]);
        RestoreBuilding(sample: sample);
    }

    private void RestoreBuilding(JsonElement sample)
    {
        _gateway.RequireThread();
        if (_building is null)
        {
            var root = new Node2D { Name = "CurrentWork", Visible = false };
            _root.AddChild(node: root);
            _building = new BuildingPreview(root: root, gateway: _gateway,
                    art: Read(file: "content/packs/main-buildings.json"),
                    assemblies: Read(file: "content/world/main-assemblies.json"),
                    access: Read(file: "content/packs/camp-access.json"), identity: "DT-24");
            _building.Attach();
        }

        JsonElement work = sample.GetProperty(propertyName: "work");
        _building.Restore(completedTier: work.GetProperty(propertyName: "completedTier").GetInt32(),
                requestedTier: work.GetProperty(propertyName: "requestedTier").GetInt32(),
                progress: work.GetProperty(propertyName: "progressSeconds").GetDouble()
                        / work.GetProperty(propertyName: "requiredSeconds").GetDouble(), defeated: false);
    }

    private void Geometry(JsonElement sample)
    {
        _gateway.RequireThread();
        VBoxContainer body = _body ?? throw new InvalidOperationException(message: "data/text not attached");
        if (body.GetNodeOrNull<Control>(path: "Geometry") is Control previous)
        {
            body.RemoveChild(node: previous);
            previous.Free();
        }

        var row = new HBoxContainer { Name = "Geometry" };
        body.AddChild(node: row);
        Grid(parent: row, data: sample.GetProperty(propertyName: "saved"), name: "Saved");
        Grid(parent: row, data: sample.GetProperty(propertyName: "currentDestination"), name: "Destination");
    }

    private void Grid(Node parent, JsonElement data, string name)
    {
        _gateway.RequireThread();
        int columns = data.GetProperty(propertyName: "columns").GetInt32();
        int rows = data.GetProperty(propertyName: "rows").GetInt32();
        Require(valid: columns is > 0 and <= 32 && rows is > 0 and <= 32, reason: "saved grid bounds");
        var grid = new Control { Name = name, CustomMinimumSize = new Vector2(x: columns * 24, y: rows * 24) };
        parent.AddChild(node: grid);
        foreach (JsonElement item in data.GetProperty(propertyName: "items").EnumerateArray())
        {
            int x = item.GetProperty(propertyName: "x").GetInt32();
            int y = item.GetProperty(propertyName: "y").GetInt32();
            JsonElement footprint = item.GetProperty(propertyName: "savedFootprint");
            bool rotated = item.GetProperty(propertyName: "rotated").GetBoolean();
            int width = footprint[rotated ? 1 : 0].GetInt32();
            int height = footprint[rotated ? 0 : 1].GetInt32();
            var control = new Panel { Name = Text(value: item, key: "id"),
                Position = new Vector2(x: x * 24, y: y * 24), Size = new Vector2(x: width * 24, y: height * 24),
                MouseFilter = Control.MouseFilterEnum.Ignore };
            var label = new Label { Text = Text(value: item, key: "definition") + " ×"
                    + item.GetProperty(propertyName: "quantity").GetInt32().ToString(
                            provider: CultureInfo.InvariantCulture), Size = control.Size, ClipText = true,
                MouseFilter = Control.MouseFilterEnum.Ignore };
            control.AddChild(node: label);
            grid.AddChild(node: control);
        }
    }

    private static string Text(JsonElement value, string key)
    {
        return value.GetProperty(propertyName: key).GetString()
                ?? throw new InvalidDataException(message: "data/text string " + key);
    }

    private static bool Same(string left, string right)
    {
        return StringComparer.Ordinal.Equals(x: left, y: right);
    }

    private static string Digest(byte[] bytes)
    {
        return Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes));
    }

    private static void Require(bool valid, string reason)
    {
        if (!valid)
        {
            throw new InvalidDataException(message: reason);
        }
    }
}
