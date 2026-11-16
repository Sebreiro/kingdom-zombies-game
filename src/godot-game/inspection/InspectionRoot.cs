using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Owns the portable read-only resource hub and every native access on its captured main thread.</summary>
public sealed partial class InspectionRoot : Node2D
{
    /// <summary>Captures the native construction owner before any authored engine access.</summary>
    public InspectionRoot()
    {
        _gateway.RequireThread();
    }

    /// <summary>Wires guarded invocation callbacks and begins the naturally selected real bow inspection.</summary>
    public override void _Ready()
    {
        _gateway.RequireThread();
        GetNode<OptionButton>(path: "Shell/Workspace/Navigation/Category").ItemSelected += ChangeCategory;
        GetNode<LineEdit>(path: "Shell/Workspace/Browser/Search").TextChanged += Filter;
        GetNode<OptionButton>(path: "Shell/Workspace/Browser/Status").ItemSelected += ChangeStatus;
        GetNode<ItemList>(path: "Shell/Workspace/Browser/Resources").ItemSelected += Select;
        GetNode<Button>(path: "Shell/Toolbar/Recheck").Pressed += Recheck;
        GetNode<Button>(path: "Shell/Toolbar/Cancel").Pressed += Cancel;
        GetNode<Button>(path: "Shell/Workspace/Preview/Actions/Details").Pressed += Details;
        GetNode<Button>(path: "Shell/Workspace/Preview/Actions/Copy").Pressed += Copy;
        Recheck();
    }

    /// <summary>Reads one bounded file per frame, then publishes one current detached result.</summary>
    public override void _Process(double delta)
    {
        _gateway.RequireThread();
        _ = delta;
        Vector2 viewport = GetViewportRect().Size;
        GetNode<Control>(path: "Shell").Size = new Vector2(x: Math.Max(val1: 320, val2: viewport.X - 48),
                y: Math.Max(val1: 240, val2: viewport.Y - 218));
        GetNode<BoxContainer>(path: "Shell/Workspace").Vertical = viewport.X < 1040;
        GetNode<Control>(path: "Summary").Size = new Vector2(x: viewport.X - 48, y: 160);
        if (_pending is null)
        {
            return;
        }

        if (_pending.TryDequeue(result: out string? path))
        {
            _files[path] = Read(path: path);
            GetNode<Label>(path: "Summary/Status").Text = "Loading " + path + "…";
            return;
        }

        int accepted = _generation;
        ContentPack candidate = ContentPack.Load(index: _index, expectations: _expectations,
                read: path => _files.GetValueOrDefault(key: path));
        if (accepted == _generation)
        {
            _pack = candidate;
            _pending = null;
            Project();
        }
    }

    /// <summary>Fences all queued work and detaches only callbacks owned by this root.</summary>
    public override void _ExitTree()
    {
        _gateway.RequireThread();
        _generation++;
        _pending = null;
        GetNode<OptionButton>(path: "Shell/Workspace/Navigation/Category").ItemSelected -= ChangeCategory;
        GetNode<LineEdit>(path: "Shell/Workspace/Browser/Search").TextChanged -= Filter;
        GetNode<OptionButton>(path: "Shell/Workspace/Browser/Status").ItemSelected -= ChangeStatus;
        GetNode<ItemList>(path: "Shell/Workspace/Browser/Resources").ItemSelected -= Select;
        GetNode<Button>(path: "Shell/Toolbar/Recheck").Pressed -= Recheck;
        GetNode<Button>(path: "Shell/Toolbar/Cancel").Pressed -= Cancel;
        GetNode<Button>(path: "Shell/Workspace/Preview/Actions/Details").Pressed -= Details;
        GetNode<Button>(path: "Shell/Workspace/Preview/Actions/Copy").Pressed -= Copy;
    }

    private readonly MainThreadGateway _gateway = new();
    private readonly Dictionary<string, byte[]?> _files = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, string> _selections = new(comparer: StringComparer.Ordinal);
    private readonly List<string> _readErrors = [];
    private ContentPack _pack = ContentPack.Load(index: null, expectations: null, read: _ => null);
    private Queue<string>? _pending;
    private byte[]? _index;
    private byte[]? _expectations;
    private string[] _rows = [];
    private int _generation;
    private long _loadedBytes;
    private string _selected = "common.bow";
    private string _category = "Items";
    private string _status = "All statuses";
    private string _query = "";

    private void Recheck()
    {
        _gateway.RequireThread();
        _generation++;
        _files.Clear();
        _readErrors.Clear();
        _loadedBytes = 0;
        _index = Read(path: "content/pack-index.json");
        _expectations = Read(path: ContentPack.ExpectedPath);
        _pack = ContentPack.Load(index: null, expectations: _expectations, read: _ => null);
        _pending = new Queue<string>(collection: ContentPack.Paths(index: _index));
        GetNode<Sprite2D>(path: "Bow").Visible = false;
        GetNode<Label>(path: "Summary/Status").Text = "Loading common.bow…";
        GetNode<Label>(path: "Summary/PackStatus").Text = "0/180 obligations · Pack incomplete — Loading";
        GetNode<Button>(path: "Shell/Toolbar/Cancel").Disabled = false;
        GetNode<Label>(path: "Shell/Toolbar/Operation").Text = "Reading declared packaged resources";
        Rows();
    }

    private void Cancel()
    {
        _gateway.RequireThread();
        _generation++;
        _pending = null;
        _files.Clear();
        _pack = ContentPack.Load(index: null, expectations: _expectations, read: _ => null);
        GetNode<Sprite2D>(path: "Bow").Visible = false;
        GetNode<Label>(path: "Summary/Status").Text = "Unavailable — validation cancelled; choose Recheck resources";
        GetNode<Label>(path: "Summary/PackStatus").Text = "0/180 obligations · Pack incomplete — Cancelled";
        GetNode<Button>(path: "Shell/Toolbar/Cancel").Disabled = true;
        GetNode<Label>(path: "Shell/Toolbar/Operation").Text = "Cancelled; no earlier validation reused";
        Rows();
    }

    private byte[]? Read(string path)
    {
        _gateway.RequireThread();
        string native = "res://" + ContentPack.ResourcePath(path: "src/godot-game/" + path);
        string disk = ProjectSettings.GlobalizePath(path: native);
        try
        {
            if (File.Exists(path: disk))
            {
                string? current = disk;
                while (current is not null)
                {
                    if (File.GetAttributes(path: current).HasFlag(flag: FileAttributes.ReparsePoint))
                    {
                        throw new IOException(message: "reparse resource path rejected");
                    }

                    current = Path.GetDirectoryName(path: current);
                }
            }

            using Godot.FileAccess? file = Godot.FileAccess.Open(path: native, flags: Godot.FileAccess.ModeFlags.Read);
            int limit = path.EndsWith(value: ".json", comparisonType: StringComparison.Ordinal)
                    ? ContentPack.MaxDocumentBytes : ContentPack.MaxResourceBytes;
            if (file is null || file.GetLength() > (ulong)limit)
            {
                throw new IOException(message: "missing or oversized resource");
            }

            if (_loadedBytes + (long)file.GetLength() > 268_435_456)
            {
                throw new IOException(message: "pack read budget exceeds256MiB; resource remains incomplete");
            }

            byte[] bytes = file.GetBuffer(length: (long)file.GetLength());
            _loadedBytes += bytes.Length;
            return file.GetError() is Error.Ok or Error.FileEof ? bytes
                    : throw new IOException(message: "native resource read failed");
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            _readErrors.Add(item: native + ", field bytes, value read — " + error.Message);
            return null;
        }
    }

    private void ChangeCategory(long index)
    {
        _gateway.RequireThread();
        _selections[_category] = _selected;
        _category = GetNode<OptionButton>(path: "Shell/Workspace/Navigation/Category").GetItemText(idx: (int)index);
        _selected = _selections.GetValueOrDefault(key: _category, defaultValue: "common.bow");
        Rows();
        Selection();
    }

    private void ChangeStatus(long index)
    {
        _gateway.RequireThread();
        _status = GetNode<OptionButton>(path: "Shell/Workspace/Browser/Status").GetItemText(idx: (int)index);
        Rows();
    }

    private void Filter(string query)
    {
        _gateway.RequireThread();
        _query = query;
        Rows();
    }

    private void Select(long index)
    {
        _gateway.RequireThread();
        if (index >= 0 && index < _rows.Length)
        {
            _selected = _rows[index];
            Selection();
        }
    }

    private void Details()
    {
        _gateway.RequireThread();
        Control panel = GetNode<Control>(path: "Shell/Workspace/Preview/Metadata");
        panel.Visible = !panel.Visible;
        GetNode<Button>(path: "Shell/Workspace/Preview/Actions/Details").Text = panel.Visible
                ? "Hide details" : "Show details";
    }

    private void Copy()
    {
        _gateway.RequireThread();
        DisplayServer.ClipboardSet(clipboard: GetNode<TextEdit>(path: "Shell/Workspace/Preview/Metadata").Text
                + "\n" + GetNode<TextEdit>(path: "Shell/Workspace/Preview/Failures").Text);
        GetNode<Label>(path: "Shell/Toolbar/Operation").Text = "Copied identity, metadata and exact diagnostics";
    }

    private void Project()
    {
        _gateway.RequireThread();
        GetNode<Label>(path: "Summary/PackStatus").Text = "0/180 obligations · " + _pack.Status
                + " · " + _pack.Entries.Count.ToString(provider: CultureInfo.InvariantCulture) + " inspectable outputs";
        GetNode<Button>(path: "Shell/Toolbar/Cancel").Disabled = true;
        GetNode<Label>(path: "Shell/Toolbar/Operation").Text = "Validated; visual/listening acceptance pending";
        Rows();
        Selection();
    }

    private void Rows()
    {
        _gateway.RequireThread();
        ItemList list = GetNode<ItemList>(path: "Shell/Workspace/Browser/Resources");
        list.Clear();
        string category = _category is "Register" or "Known issues" ? "" : _category;
        IEnumerable<string> outputs = _pack.Search(query: _query, category: category,
                status: _status is "All statuses" ? "" : _status);
        IEnumerable<string> expected = _pack.Expected.Where(predicate: pair =>
                (category.Length == 0 || StringComparer.Ordinal.Equals(x: ContentPack.Category(id: pair.Key),
                        y: category)) && (_status is "All statuses" or "Incomplete" or "Error")
                        && (pair.Key.Contains(value: _query, comparisonType: StringComparison.OrdinalIgnoreCase)
                                || pair.Value.Contains(value: _query,
                                        comparisonType: StringComparison.OrdinalIgnoreCase)))
                .Select(selector: pair => pair.Key);
        _rows = outputs.Concat(second: expected).Order(comparer: StringComparer.Ordinal).ToArray();
        foreach (string id in _rows)
        {
            string name = _pack.Entries.TryGetValue(key: id, value: out JsonElement entry)
                    ? entry.GetProperty(propertyName: "name").GetString() ?? "Invalid name"
                    : _pack.Expected[id].Split(separator: '\n')[0];
            int row = list.AddItem(text: id + " · " + name + " · "
                    + (_pack.Entries.ContainsKey(key: id) ? "Partial" : "Incomplete"));
            if (StringComparer.Ordinal.Equals(x: id, y: _selected))
            {
                list.Select(idx: row);
            }
        }

        GetNode<Label>(path: "Shell/Workspace/Browser/Count").Text = _rows.Length.ToString(
                provider: CultureInfo.InvariantCulture) + (_rows.Length == 1 ? " result" : " results")
                + (_rows.Length == 0 ? " — No resources match. Clear the filter or choose another category." : "");
    }

    private void Selection()
    {
        _gateway.RequireThread();
        bool found = _pack.Entries.TryGetValue(key: _selected, value: out JsonElement entry);
        GetNode<Label>(path: "Summary/ResourceId").Text = _selected;
        GetNode<Label>(path: "Summary/ResourceName").Text = found
                ? entry.GetProperty(propertyName: "name").GetString() : _pack.Expected.GetValueOrDefault(
                        key: _selected, defaultValue: "Unavailable resource; choose a listed ID");
        GetNode<Label>(path: "Summary/Status").Text = found
                ? "Partial — validated bytes; native/visual/listening acceptance pending"
                : "Incomplete — required resource or current evidence missing";
        GetNode<Sprite2D>(path: "Bow").Visible = found && _selected is "common.bow";
        GetNode<TextEdit>(path: "Shell/Workspace/Preview/Metadata").Text = found
                ? entry.GetRawText() + "\nExplicit shared mappings: " + string.Join(separator: ", ",
                        values: ContentPack.Mappings(entry: entry)) : _pack.Expected.GetValueOrDefault(
                                key: _selected, defaultValue: "No validated definition");
        IEnumerable<string> errors = _category is "Known issues" ? _pack.Diagnostics
                : _pack.Diagnostics.Where(predicate: error => error.StartsWith(value: _selected + ":",
                        comparisonType: StringComparison.Ordinal) || error.StartsWith(value: "expected:",
                                comparisonType: StringComparison.Ordinal) || error.StartsWith(value: "pack:",
                                        comparisonType: StringComparison.Ordinal));
        GetNode<TextEdit>(path: "Shell/Workspace/Preview/Failures").Text = string.Join(separator: "\n",
                values: errors.Concat(second: _readErrors));
    }

}
