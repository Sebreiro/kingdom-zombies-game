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
        _browser = new ResourceBrowser(root: this);
    }

    /// <summary>Wires guarded invocation callbacks and begins the naturally selected real bow inspection.</summary>
    public override void _Ready()
    {
        _gateway.RequireThread();
        SetMeta(name: "inspection_runtime", value: GetType().FullName + "|"
                + GetType().Assembly.ManifestModule.ModuleVersionId.ToString(format: "D") + "|"
                + typeof(ContentPack).Assembly.ManifestModule.ModuleVersionId.ToString(format: "D") + "|"
                + System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(assembly: GetType().Assembly)?.GetType()
                        .FullName + "|" + ReferenceEquals(
                                objA: System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(
                                        assembly: GetType().Assembly),
                                objB: System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(
                                        assembly: typeof(ContentPack).Assembly)));
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
        _resource?.Tick(delta: delta);
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
        _resource?.Close();
        _resource = null;
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
    private readonly ResourceBrowser _browser;
    private readonly WorldPreview.ItemScenes _itemScenes = new();
    private readonly List<string> _readErrors = [];
    private readonly Dictionary<string, (string Query, string Status, double Scroll)> _views = new(
            comparer: StringComparer.Ordinal);
    private InspectionShell? _shell;
    private InspectionInput? _input;
    private CharacterPreview? _character;
    private WorldPreview? _world;
    private AudioPreview? _audio;
    private DataTextPreview? _data;
    private IdentityKit.ResourceView? _resource;
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
        _browser.ClearErrors();
        _pending = null;
        _resource?.Close();
        _resource = null;
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
        _resource?.Close();
        _resource = null;
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
        if (_browser.StartupSelection)
        {
            _selections[_category] = _selected;
        }

        _browser.StartupSelection = false;
        _views[_category] = (_query, _status, GetNode<ItemList>(path: Browser + "Resources")
                .GetVScrollBar().Value);
        _category = GetNode<OptionButton>(path: "Shell/Workspace/Navigation/Category").GetItemText(idx: (int)index);
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

        _browser.ReconcileSelection(chooseFirst: true, refresh: true);
        GetNode<ItemList>(path: Browser + "Resources").GetVScrollBar().Value = scroll;
    }

    private void ChangeStatus(long index)
    {
        _gateway.RequireThread();
        _status = GetNode<OptionButton>(path: Browser + "Status").GetItemText(idx: (int)index);
        _browser.ApplyFilters();
    }

    private void Filter(string query)
    {
        _gateway.RequireThread();
        _query = query;
        _browser.ApplyFilters();
    }

    private void Select(long index)
    {
        _gateway.RequireThread();
        if (index >= 0 && index < _rows.Length)
        {
            _browser.StartupSelection = false;
            _selected = _rows[index];
            _selections[_category] = _selected;
            if (_pending is null && _accepting)
            {
                Selection();
            }
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
        int sourceReady = 0;
        foreach (JsonElement entry in _pack.Entries.Values)
        {
            if (entry.TryGetProperty(propertyName: "sourceStatus", value: out JsonElement status)
                    && status.GetString() is "runtime-definition-validated; authoring source proof separate")
            {
                sourceReady++;
            }
        }

        GetNode<Label>(path: "Summary/PackStatus").Text = sourceReady == 0
                ? "0/180 obligations · " + _pack.Status + " · "
                        + _pack.Entries.Count.ToString(provider: CultureInfo.InvariantCulture) + " inspectable outputs"
                : sourceReady.ToString(provider: CultureInfo.InvariantCulture)
                        + "/180 source-ready · 0/180 accepted · " + _pack.Status;
        GetNode<Button>(path: "Shell/Toolbar/Cancel").Disabled = true;
        GetNode<Label>(path: "Shell/Toolbar/Operation").Text = "Validated; visual/listening acceptance pending";
        _browser.ReconcileSelection(chooseFirst: true, refresh: true);
    }

    private void Rows()
    {
        _gateway.RequireThread();
        ItemList list = GetNode<ItemList>(path: Browser + "Resources");
        list.Clear();
        List<(string Id, string Text)> rows = _browser.VisibleResources();
        _rows = [.. rows.Select(selector: row => row.Id)];
        _browser.ResolveRows();
        foreach ((string id, string text) in rows)
        {
            int row = list.AddItem(text: text);
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
        _browser.RetireSelection();
        _readErrors.Clear();
        GetNode<TextEdit>(path: "Shell/Workspace/Preview/Failures").Text = WorldPreview.InspectorDiagnostics(
                diagnostics: _pack.Diagnostics, readErrors: _readErrors, category: _category, selected: _selected);
        string? previewStatus = null;
        if (_selected.Length > 0
                && (_browser.OpenIncomplete || _browser.SourceStatus(id: _selected) is not "Incomplete"))
        {
            if (IdentityKit.ResourceView.Handles(id: _selected))
            {
                _resource = IdentityKit.ResourceView.Open(shell: _shell, gateway: _gateway, read: Read, id: _selected);
                previewStatus = _resource.Status;
            }
            else if (AudioPreview.Handles(id: _selected))
            {
                _audio = AudioPreview.Open(shell: _shell, gateway: _gateway, read: Read, category: _selected);
                previewStatus = _audio.Status;
            }
            else if (WorldPreview.InspectorHandles(id: _selected))
            {
                var preview = new Node2D { Name = "WorldItems" };
                _shell?.Install(preview: preview);
                _world = WorldPreview.OpenInspector(root: preview, gateway: _gateway, read: Read, id: _selected,
                        choices: _selections, itemScenes: _itemScenes);
                previewStatus = _world.InspectorStatus;
            }
            else if (_selected is "CH-01" or "CH-02" or "CH-03" or "CH-04" or "CH-05" or "CH-06" or "CH-07" or "CH-08"
                    || CharacterPreview.HandlesObligation(id: _selected))
            {
                _character = CharacterPreview.TryOpenAttached(shell: _shell, gateway: _gateway, read: Read,
                        id: _selected, owner: this);
                previewStatus = _character is null ? GetNode<Label>(path: "Summary/Status").Text : null;
            }
            else
            {
                _data = DataTextPreview.Open(shell: _shell, gateway: _gateway, read: Read, id: _selected, owner: this);
                previewStatus = _data?.Status;
            }
        }

        _browser.RecordPreviewError(status: previewStatus);
        bool found = _pack.Entries.TryGetValue(key: _selected, value: out JsonElement entry);
        GetNode<Label>(path: "Summary/ResourceId").Text = _selected;
        GetNode<Label>(path: "Summary/ResourceName").Text = _browser.SelectionName(found: found);
        GetNode<Label>(path: "Summary/Status").Text = _browser.SelectionStatus(id: _selected);
        GetNode<Sprite2D>(path: "Bow").Visible = found && _selected is "common.bow";
        GetNode<TextEdit>(path: "Shell/Workspace/Preview/Metadata").Text = found ? entry.GetRawText()
                + DataTextPreview.Shared + string.Join(separator: ", ", values: ContentPack.Mappings(entry: entry))
                : _pack.Expected.GetValueOrDefault(key: _selected, defaultValue: "No validated definition");
    }

    private sealed class ResourceBrowser
    {
        internal ResourceBrowser(InspectionRoot root)
        {
            _root = root;
        }

        internal bool StartupSelection { get; set; } = true;
        internal bool OpenIncomplete { get; private set; } = true;

        internal void ClearErrors() => _previewErrors.Clear();

        internal void ApplyFilters()
        {
            _root._gateway.RequireThread();
            if (StartupSelection)
            {
                _root._selections[_root._category] = _root._selected;
            }

            StartupSelection = false;
            ReconcileSelection(chooseFirst: false, refresh: false);
        }

        internal void ReconcileSelection(bool chooseFirst, bool refresh)
        {
            string previous = _root._selected;
            _chooseFirst = chooseFirst;
            _root.Rows();
            if (_root._pending is null && _root._accepting
                    && (refresh || !StringComparer.Ordinal.Equals(x: previous, y: _root._selected)))
            {
                OpenIncomplete = false;
                try
                {
                    _root.Selection();
                }
                finally
                {
                    OpenIncomplete = true;
                }
            }
        }

        internal void ResolveRows()
        {
            bool? chooseFirst = _chooseFirst;
            _chooseFirst = null;
            if (StartupSelection || !chooseFirst.HasValue)
            {
                return;
            }

            string preferred = _root._selections.GetValueOrDefault(key: _root._category, defaultValue: "");
            _root._selected = _root._rows.Contains(value: preferred, comparer: StringComparer.Ordinal) ? preferred
                    : chooseFirst.Value ? _root._rows.FirstOrDefault() ?? "" : "";
            if (preferred.Length == 0 && _root._selected.Length > 0)
            {
                _root._selections[_root._category] = _root._selected;
            }
        }

        internal List<(string Id, string Text)> VisibleResources()
        {
            string category = _root._category is "Register" or "Known issues" ? "" : _root._category;
            IEnumerable<string> outputs = _root._pack.Search(query: "", category: category, status: "");
            IEnumerable<string> expected = _root._pack.Expected.Where(predicate: pair =>
                    category.Length == 0 || StringComparer.Ordinal.Equals(x: ContentPack.Category(id: pair.Key),
                            y: category))
                    .Select(selector: pair => pair.Key);
            if ((category is "" or "Items") && HasSourceError(id: "common.bow"))
            {
                outputs = outputs.Append(element: "common.bow");
            }

            var rows = new List<(string Id, string Text)>();
            foreach (string id in outputs.Concat(second: expected).Concat(second: WorldPreview.InspectorIds(
                    category: category)).Distinct(comparer: StringComparer.Ordinal)
                    .Order(comparer: StringComparer.Ordinal))
            {
                string name = ResourceName(id: id), status = SourceStatus(id: id);
                if ((_root._status is "All statuses" || StringComparer.Ordinal.Equals(x: _root._status, y: status))
                        && (id.Contains(value: _root._query, comparisonType: StringComparison.OrdinalIgnoreCase)
                                || name.Contains(value: _root._query,
                                        comparisonType: StringComparison.OrdinalIgnoreCase)))
                {
                    rows.Add(item: (id, id + " · " + name + " · " + status));
                }
            }

            return rows;
        }

        internal void RecordPreviewError(string? status)
        {
            string id = _root._selected;
            if (status is null || SourceStatus(id: id) is "Incomplete"
                    || (!status.StartsWith(value: "Error", comparisonType: StringComparison.Ordinal)
                            && !status.StartsWith(value: "Unavailable", comparisonType: StringComparison.Ordinal)))
            {
                return;
            }

            _previewErrors[id] = status.StartsWith(value: "Error", comparisonType: StringComparison.Ordinal)
                    ? status : "Error — " + id + " — " + status;
            _root.Rows();
            if (!StartupSelection && !_root._rows.Contains(value: id, comparer: StringComparer.Ordinal))
            {
                _root._selected = "";
                RetireSelection();
            }
        }

        internal string SelectionName(bool found)
        {
            string id = _root._selected;
            if (_root._world is not null)
            {
                return "Authored world/item view · " + id;
            }

            string name = found ? ResourceName(id: id) : _root._pack.Expected.GetValueOrDefault(key: id,
                    defaultValue: DataTextPreview.Unknown);
            return id.Length == 0 ? "No resource selected" : name;
        }

        internal string ResourceName(string id)
        {
            return _root._pack.Entries.TryGetValue(key: id, value: out JsonElement entry)
                    ? entry.GetProperty(propertyName: "name").GetString() ?? "Invalid name"
                    : _root._pack.Expected.GetValueOrDefault(key: id, defaultValue: "Authored inspection view")
                            .Split(separator: '\n')[0];
        }

        internal string SourceStatus(string id)
        {
            if (_previewErrors.ContainsKey(key: id) || HasSourceError(id: id))
            {
                return "Error";
            }

            if (_root._pack.Entries.ContainsKey(key: id))
            {
                return "Partial";
            }

            if (id is "world.assembled" or "world.portals" or "world.effects" or "world.wildlife")
            {
                string[] paths = ["content/fixtures/world-states.json", "content/world/portal-assemblies.json",
                    "content/packs/world.json", "content/world/map.json", "content/packs/effects-portals.json",
                    "content/packs/common.json", "content/audio/events.json", "assets/world/environment.png",
                    "assets/effects/effects.png"];
                if (paths.Any(predicate: path => _root._pack.Diagnostics.Any(predicate: error =>
                        error.Contains(value: ": res://" + path + ", field ",
                                comparisonType: StringComparison.Ordinal))))
                {
                    return "Error";
                }

                if (paths.All(predicate: path => _root._files.GetValueOrDefault(key: path) is not null))
                {
                    return "Partial";
                }
            }

            return "Incomplete";
        }

        internal void RetireSelection()
        {
            _root._gateway.RequireThread();
            _root._generation++;
            _root._resource?.Close();
            _root._resource = null;
            _root._data?.Close();
            _root._data = null;
            _root._audio?.Close();
            _root._audio = null;
            _root._world?.Close();
            _root._world = null;
            _root._character?.Detach();
            _root._character = null;
            _root._shell?.Retire();
        }

        internal string SelectionStatus(string id)
        {
            if (id.Length == 0)
            {
                return "Unavailable — choose a matching resource";
            }

            return SourceStatus(id: id) switch
            {
                "Partial" => "Partial — validated bytes; native/visual/listening acceptance pending",
                "Error" => _previewErrors.GetValueOrDefault(key: id,
                        defaultValue: "Error — " + id + "; choose another resource or Recheck"),
                _ => "Incomplete — required resource or current evidence missing",
            };
        }

        private readonly InspectionRoot _root;
        private readonly Dictionary<string, string> _previewErrors = new(comparer: StringComparer.Ordinal);
        private bool? _chooseFirst;

        private bool HasSourceError(string id)
        {
            string prefix = id + ": ";
            string acceptance = id + ": res://" + ContentPack.ExpectedPath + ", field acceptance, value ";
            return _root._pack.Diagnostics.Any(predicate: error => error.StartsWith(value: prefix,
                    comparisonType: StringComparison.Ordinal) && !error.StartsWith(value: acceptance,
                            comparisonType: StringComparison.Ordinal));
        }
    }
}
