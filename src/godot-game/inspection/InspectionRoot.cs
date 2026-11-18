using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Sebreiro.CodeRules", "SCR0001", Scope = "member",
        Target = ("~M:KingdomZombies.Inspection.InspectionRoot.GetGodotMethodList"),
        Justification = "KZ-EX-INSPECTION26-CALLS-1 observed exact generator member and boundary proofs")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Sebreiro.CodeRules", "SCR0001", Scope = "member",
        Target = ("~M:KingdomZombies.Inspection.InspectionRoot.HasGodotClassMethod(Godot." +
                "NativeInterop.godot_string_name@)"),
        Justification = "KZ-EX-INSPECTION26-CALLS-2 observed exact generator member and boundary proofs")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Sebreiro.CodeRules", "SCR0001", Scope = "member",
        Target = ("~M:KingdomZombies.Inspection.InspectionRoot.InvokeGodotClassMethod(God" +
                "ot.NativeInterop.godot_string_name@,Godot.NativeInterop.NativeVariantP" +
                "trArgs,Godot.NativeInterop.godot_variant@)"),
        Justification = "KZ-EX-INSPECTION26-CALLS-3 observed exact generator member and boundary proofs")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Sebreiro.CodeRules", "SCR0001", Scope = "member",
        Target = ("~M:KingdomZombies.Inspection.InspectionRoot.SetGodotClassPropertyValue" +
                "(Godot.NativeInterop.godot_string_name@,Godot.NativeInterop.godot_vari" +
                "ant@)"),
        Justification = "KZ-EX-INSPECTION26-CALLS-4 observed exact generator member and boundary proofs")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Sebreiro.CodeRules", "SCR0001", Scope = "member",
        Target = ("~M:KingdomZombies.Inspection.InspectionRoot.GetGodotClassPropertyValue" +
                "(Godot.NativeInterop.godot_string_name@,Godot.NativeInterop.godot_vari" +
                "ant@)"),
        Justification = "KZ-EX-INSPECTION26-CALLS-5 observed exact generator member and boundary proofs")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Sebreiro.CodeRules", "SCR0001", Scope = "member",
        Target = ("~M:KingdomZombies.Inspection.InspectionRoot.GetGodotPropertyList"),
        Justification = "KZ-EX-INSPECTION26-CALLS-6 observed exact generator member and boundary proofs")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Sebreiro.CodeRules", "SCR0001", Scope = "member",
        Target = ("~M:KingdomZombies.Inspection.InspectionRoot.SaveGodotObjectData(Godot." +
                "Bridge.GodotSerializationInfo)"),
        Justification = "KZ-EX-INSPECTION26-CALLS-7 observed exact generator member and boundary proofs")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Sebreiro.CodeRules", "SCR0001", Scope = "member",
        Target = ("~M:KingdomZombies.Inspection.InspectionRoot.RestoreGodotObjectData(God" +
                "ot.Bridge.GodotSerializationInfo)"),
        Justification = "KZ-EX-INSPECTION26-CALLS-8 observed exact generator member and boundary proofs")]

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
        _shell = new InspectionShell(root: this, gateway: _gateway);
        _input = new InspectionInput(root: this, gateway: _gateway);
        _shell.Attach();
        GetNode<OptionButton>(path: "Shell/Workspace/Navigation/Category").ItemSelected += ChangeCategory;
        GetNode<LineEdit>(path: Browser + "Search").TextChanged += Filter;
        GetNode<OptionButton>(path: Browser + "Status").ItemSelected += ChangeStatus;
        GetNode<ItemList>(path: Browser + "Resources").ItemSelected += Select;
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
        _shell?.Tick(delta: delta);
        _audio?.Tick(delta: delta);
        _data?.Tick(delta: delta);
        _input?.ReconcileFocus();
        if (_pending is null)
        {
            return;
        }

        if (_pending.TryDequeue(result: out string? path))
        {
            _files[path] = Read(path: path);
            _loadedBytes += _files[path]?.Length ?? 0;
            GetNode<Label>(path: "Summary/Status").Text = "Loading " + path + "…";
            return;
        }

        int accepted = _loadGeneration;
        ContentPack candidate = ContentPack.Load(index: _index, expectations: _expectations,
                read: path => _files.GetValueOrDefault(key: path));
        if (_accepting && accepted == _loadGeneration)
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
        _accepting = false;
        _generation++;
        _loadGeneration++;
        _pending = null;
        _data?.Close();
        _data = null;
        _audio?.Close();
        _audio = null;
        _world?.Close();
        _world = null;
        _character?.Detach();
        _character = null;
        _shell?.Detach();
        GetNode<OptionButton>(path: "Shell/Workspace/Navigation/Category").ItemSelected -= ChangeCategory;
        GetNode<LineEdit>(path: Browser + "Search").TextChanged -= Filter;
        GetNode<OptionButton>(path: Browser + "Status").ItemSelected -= ChangeStatus;
        GetNode<ItemList>(path: Browser + "Resources").ItemSelected -= Select;
        GetNode<Button>(path: "Shell/Toolbar/Recheck").Pressed -= Recheck;
        GetNode<Button>(path: "Shell/Toolbar/Cancel").Pressed -= Cancel;
        GetNode<Button>(path: "Shell/Workspace/Preview/Actions/Details").Pressed -= Details;
        GetNode<Button>(path: "Shell/Workspace/Preview/Actions/Copy").Pressed -= Copy;
    }

    /// <summary>Dismisses the innermost owner while native GUI retains activation and text keys.</summary>
    public override void _Input(InputEvent @event)
    {
        _gateway.RequireThread();
        _input?.Dismiss(input: @event);
    }

    /// <summary>Installs a detached native preview only while its exact selection generation accepts results.</summary>
    internal void AcceptPreview(int generation, Node preview)
    {
        _gateway.RequireThread();
        if (!_accepting || generation != _generation)
        {
            preview.QueueFree();
            return;
        }

        _shell?.Install(preview: preview);
    }

    private const string Browser = "Shell/Workspace/Controls/Browser/";
    private readonly MainThreadGateway _gateway = new();
    private readonly Dictionary<string, byte[]?> _files = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, string> _selections = new(comparer: StringComparer.Ordinal);
    private readonly List<string> _readErrors = [];
    private readonly Dictionary<string, (string Query, string Status, double Scroll)> _views = new(
            comparer: StringComparer.Ordinal);
    private InspectionShell? _shell;
    private InspectionInput? _input;
    private CharacterPreview? _character;
    private WorldPreview? _world;
    private AudioPreview? _audio;
    private DataTextPreview? _data;
    private ContentPack _pack = ContentPack.Load(index: null, expectations: null, read: _ => null);
    private Queue<string>? _pending;
    private byte[]? _index;
    private byte[]? _expectations;
    private string[] _rows = [];
    private int _generation;
    private int _loadGeneration;
    private bool _accepting;
    private long _loadedBytes;
    private string _selected = "common.bow";
    private string _category = "Items";
    private string _status = "All statuses";
    private string _query = "";

    private void Recheck()
    {
        _gateway.RequireThread();
        _accepting = false;
        _generation++;
        _loadGeneration++;
        _files.Clear();
        _pending = null;
        _data?.Close();
        _data = null;
        _audio?.Close();
        _audio = null;
        _world?.Close();
        _world = null;
        _character?.Detach();
        _character = null;
        _shell?.Retire();
        _readErrors.Clear();
        _loadedBytes = 0;
        _index = null;
        _expectations = null;
        _index = Read(path: "content/pack-index.json");
        _loadedBytes += _index?.Length ?? 0;
        _expectations = Read(path: ContentPack.ExpectedPath);
        _loadedBytes += _expectations?.Length ?? 0;
        _pack = ContentPack.Load(index: null, expectations: _expectations, read: _ => null);
        _pending = new Queue<string>(collection: ContentPack.Paths(index: _index));
        _accepting = true;
        GetNode<Sprite2D>(path: "Bow").Visible = false;
        GetNode<Label>(path: "Summary/Status").Text = "Loading common.bow…";
        GetNode<TextEdit>(path: "Shell/Workspace/Preview/Failures").Text = "";
        GetNode<Label>(path: "Summary/PackStatus").Text = "0/180 obligations · Pack incomplete — Loading";
        GetNode<Button>(path: "Shell/Toolbar/Cancel").Disabled = false;
        GetNode<Label>(path: "Shell/Toolbar/Operation").Text = "Reading declared packaged resources";
        Rows();
    }

    private void Cancel()
    {
        _gateway.RequireThread();
        _accepting = false;
        _generation++;
        _loadGeneration++;
        _pending = null;
        _data?.Close();
        _data = null;
        _audio?.Close();
        _audio = null;
        _world?.Close();
        _world = null;
        _character?.Detach();
        _character = null;
        _shell?.Retire();
        _files.Clear();
        _loadedBytes = (_index?.LongLength ?? 0) + (_expectations?.LongLength ?? 0);
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
        string native = "res://" + path;
        _readErrors.RemoveAll(match: error => error.StartsWith(value: native + ", field bytes, value read — ",
                comparisonType: StringComparison.Ordinal));
        try
        {
            native = "res://" + WorldPreview.InspectorResourcePath(path: path);
            string disk = ProjectSettings.GlobalizePath(path: native);
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
            limit = path is "content/packs/common.json" ? 2_097_152 : limit;
            if (file is null)
            {
                throw new IOException(message: "missing or oversized resource");
            }

            long expected = WorldPreview.InspectorReadLength(expected: file.GetLength(), limit: limit,
                    retained: _loadedBytes);
            byte[] bytes = file.GetBuffer(length: expected);
            byte[] complete = WorldPreview.InspectorReadComplete(bytes: bytes, expected: expected,
                    successful: file.GetError() is Error.Ok or Error.FileEof);
            GetNode<TextEdit>(path: "Shell/Workspace/Preview/Failures").Text = WorldPreview.InspectorDiagnostics(
                    diagnostics: _pack.Diagnostics, readErrors: _readErrors, category: _category, selected: _selected);
            return complete;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or FormatException
                or InvalidDataException)
        {
            _readErrors.Add(item: native + ", field bytes, value read — " + error.Message);
            GetNode<TextEdit>(path: "Shell/Workspace/Preview/Failures").Text = WorldPreview.InspectorDiagnostics(
                    diagnostics: _pack.Diagnostics, readErrors: _readErrors, category: _category, selected: _selected);
            return null;
        }
    }

    private void ChangeCategory(long index)
    {
        _gateway.RequireThread();
        _accepting = false;
        _generation++;
        _data?.Close();
        _data = null;
        _audio?.Close();
        _audio = null;
        _world?.Close();
        _world = null;
        _character?.Detach();
        _character = null;
        _shell?.Retire();
        _selections[_category] = _selected;
        _views[_category] = (_query, _status, GetNode<ItemList>(path: Browser + "Resources")
                .GetVScrollBar().Value);
        _category = GetNode<OptionButton>(path: "Shell/Workspace/Navigation/Category").GetItemText(idx: (int)index);
        _selected = _selections.GetValueOrDefault(key: _category, defaultValue: "common.bow");
        (string query, string status, double scroll) = _views.GetValueOrDefault(key: _category,
                defaultValue: ("", "All statuses", 0));
        _query = query;
        _status = status;
        GetNode<LineEdit>(path: Browser + "Search").Text = query;
        OptionButton statusControl = GetNode<OptionButton>(path: Browser + "Status");
        for (int row = 0; row < statusControl.ItemCount; row++)
        {
            if (StringComparer.Ordinal.Equals(x: statusControl.GetItemText(idx: row), y: status))
            {
                statusControl.Select(idx: row);
            }
        }

        Rows();
        GetNode<ItemList>(path: Browser + "Resources").GetVScrollBar().Value = scroll;
        _accepting = true;
        Selection();
    }

    private void ChangeStatus(long index)
    {
        _gateway.RequireThread();
        _status = GetNode<OptionButton>(path: Browser + "Status").GetItemText(idx: (int)index);
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
            _accepting = false;
            _generation++;
            _data?.Close();
        _data = null;
        _audio?.Close();
            _audio = null;
            _world?.Close();
            _world = null;
            _character?.Detach();
            _character = null;
            _shell?.Retire();
            _selected = _rows[index];
            _accepting = true;
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
        if (panel.Visible)
        {
            panel.GrabFocus();
        }
        else
        {
            GetNode<Button>(path: "Shell/Workspace/Preview/Actions/Details").GrabFocus();
        }
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
        ItemList list = GetNode<ItemList>(path: Browser + "Resources");
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
        IEnumerable<string> authored = WorldPreview.InspectorIds(category: category).Where(predicate: id =>
                (_status is "All statuses" or "Partial") && id.Contains(value: _query,
                        comparisonType: StringComparison.OrdinalIgnoreCase));
        _rows = outputs.Concat(second: expected).Concat(second: authored).Concat(second: AudioPreview.Ids(
                category: category).Where(predicate: id => (_status is "All statuses" or "Incomplete" or "Error")
                        && id.Contains(value: _query, comparisonType: StringComparison.OrdinalIgnoreCase)))
                .Concat(second: DataTextPreview.Ids(category: category).Where(predicate: id =>
                        (_status is "All statuses" or "Incomplete" or "Error")
                                && id.Contains(value: _query, comparisonType: StringComparison.OrdinalIgnoreCase)))
                .Distinct(comparer: StringComparer.Ordinal)
                .Order(comparer: StringComparer.Ordinal).ToArray();
        foreach (string id in _rows)
        {
            string name = _pack.Entries.TryGetValue(key: id, value: out JsonElement entry)
                    ? entry.GetProperty(propertyName: "name").GetString() ?? "Invalid name"
                    : _pack.Expected.GetValueOrDefault(key: id, defaultValue: "Authored inspection view")
                            .Split(separator: '\n')[0];
            int row = list.AddItem(text: id + " · " + name + " · "
                    + (_pack.Entries.ContainsKey(key: id) || WorldPreview.InspectorHandles(id: id)
                            ? "Partial" : "Incomplete"));
            if (StringComparer.Ordinal.Equals(x: id, y: _selected))
            {
                list.Select(idx: row);
            }
        }

        GetNode<Label>(path: Browser + "Count").Text = _rows.Length.ToString(
                provider: CultureInfo.InvariantCulture) + (_rows.Length == 1 ? " result" : " results")
                + (_rows.Length == 0 ? " — No resources match. Clear the filter or choose another category." : "");
    }

    private void Selection()
    {
        _gateway.RequireThread();
        _data?.Close();
        _audio?.Close();
        _audio = null;
        _world?.Close();
        _world = null;
        _shell?.Retire();
        _character?.Detach();
        _character = null;
        _readErrors.Clear();
        bool found = _pack.Entries.TryGetValue(key: _selected, value: out JsonElement entry);
        GetNode<Label>(path: "Summary/ResourceId").Text = _selected;
        GetNode<Label>(path: "Summary/ResourceName").Text = found ? entry.GetProperty(propertyName: "name").GetString()
                : _pack.Expected.GetValueOrDefault(key: _selected, defaultValue: DataTextPreview.Unknown);
        GetNode<Label>(path: "Summary/Status").Text = found
                ? "Partial — validated bytes; native/visual/listening acceptance pending"
                : "Incomplete — required resource or current evidence missing";
        GetNode<Sprite2D>(path: "Bow").Visible = found && _selected is "common.bow";
        GetNode<TextEdit>(path: "Shell/Workspace/Preview/Metadata").Text = found ? entry.GetRawText()
                + DataTextPreview.Shared + string.Join(separator: ", ", values: ContentPack.Mappings(entry: entry))
                : _pack.Expected.GetValueOrDefault(key: _selected, defaultValue: "No validated definition");
        GetNode<TextEdit>(path: "Shell/Workspace/Preview/Failures").Text = WorldPreview.InspectorDiagnostics(
                diagnostics: _pack.Diagnostics, readErrors: _readErrors, category: _category, selected: _selected);
        _data = DataTextPreview.Open(shell: _shell, gateway: _gateway, read: Read, id: _selected, owner: this);
        if (AudioPreview.Handles(id: _selected))
        {
            _audio = AudioPreview.Open(shell: _shell, gateway: _gateway, read: Read, category: _selected);
            GetNode<Label>(path: "Summary/Status").Text = _audio.Status;
        }
        if (WorldPreview.InspectorHandles(id: _selected))
        {
            var preview = new Node2D { Name = "WorldItems" };
            _shell?.Install(preview: preview);
            _world = WorldPreview.OpenInspector(root: preview, gateway: _gateway, read: Read, id: _selected,
                    choices: _selections);
            GetNode<Label>(path: "Summary/ResourceName").Text = "Authored world/item view · " + _selected;
            GetNode<Label>(path: "Summary/Status").Text = _world.InspectorStatus;
        }
        if (_selected is "CH-01" or "CH-02" or "CH-03" or "CH-04" or "CH-05" or "CH-06" or "CH-07" or "CH-08")
        {
            var preview = new Node2D { Name = "Character" };
            _shell?.Install(preview: preview);
            string resource = "content/animation/locomotion-melee.json";
            try
            {
                _character = CharacterPreview.Open(root: preview, gateway: _gateway,
                        read: path => Read(path: resource = path), body: _selected);
            }
            catch (Exception error) when (error is InvalidDataException or JsonException or FormatException)
            {
                _character = null;
                _shell?.Retire();
                GetNode<TextEdit>(path: "Shell/Workspace/Preview/Failures").Text += "\n" + _selected
                        + ": " + resource + " — " + error.Message;
                GetNode<Label>(path: "Summary/Status").Text = "Error — " + _selected + "; choose Recheck resources";
                return;
            }
            _character.Attach();
        }
    }

}
