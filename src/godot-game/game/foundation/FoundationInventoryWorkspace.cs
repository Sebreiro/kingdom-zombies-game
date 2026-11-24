using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;
using Model = KingdomZombies.Foundation.FoundationInventoryInteraction;

namespace KingdomZombies.Foundation;

/// <summary>Connects the normal game's inventory surfaces to current recipient-authorized state.</summary>
internal sealed partial class FoundationInventoryWorkspace : FoundationInput.IWorkspace, IDisposable
{
    public bool CapturesWorld => _model.Open;

    public bool Handle(FoundationInput.WorkspaceCommand command)
    {
        _gateway.RequireThread();
        if (command is FoundationInput.WorkspaceCommand.Escape)
        {
            return Escape();
        }

        if (_epoch <= 0)
        {
            return false;
        }

        if (command is FoundationInput.WorkspaceCommand.Inventory && _model.Open)
        {
            return Escape();
        }

        _model.Show(external: command is FoundationInput.WorkspaceCommand.Interact);
        if (command is FoundationInput.WorkspaceCommand.Interact)
        {
            OpenNearby();
        }

        _dirty = true;
        RefreshWindows();
        Project();
        return true;
    }

    public void Dispose()
    {
        _gateway.RequireThread();
        Release();
        _art?.Dispose();
        _art = null;
        _grid = null;
    }

    internal FoundationInventoryWorkspace(Control root, MainThreadGateway gateway,
            Func<FoundationCoordinator?> coordinator)
    {
        _root = root;
        _gateway = gateway;
        _coordinator = coordinator;
    }

    internal int VisibleSlotCount => _targets.Count(predicate: region =>
            region.Kind is "equipment" && region.Token == 0);
    internal bool FocusDetailsVisible => _grid?.FocusDetailsVisible == true;

    internal void Observe(FoundationRunReadback? readback, string? owner, long epoch, bool connected)
    {
        _gateway.RequireThread();
        if (!connected || epoch <= 0 || epoch != _epoch
                || !StringComparer.Ordinal.Equals(x: owner, y: _owner))
        {
            Release();
            _epoch = connected ? epoch : 0;
            _owner = owner ?? "";
        }

        _readback = connected ? readback : null;
        UpdateNotice();
        if (!_model.Open)
        {
            return;
        }

        UpdateFeedback();
        InspectSurfaces();
        RefreshWindows();
        string current = Fingerprint();
        if (_dirty || !StringComparer.Ordinal.Equals(x: current, y: _version))
        {
            _version = current;
            Project();
        }
    }

    internal void Release()
    {
        _gateway.RequireThread();
        ResetExternal();
        _model.Reset();
        RemoveLayer();
        _readback = null;
        _requestedOwn = [];
        _requestedExternal = [];
        _pending = null;
        _epoch = 0;
        _owner = "";
        RemoveNotice();
    }

    internal void LoseFocus()
    {
        _gateway.RequireThread();
        _grid?.HideFocusDetails();
        CancelPreview();
        RemovePopup();
        _movingBag = null;
    }

    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly Func<FoundationCoordinator?> _coordinator;
    private readonly Model _model = new();
    private readonly List<TargetRegion> _targets = [];
    private readonly Dictionary<long, Control> _bagPanels = [];
    private FoundationInventoryArt? _art;
    private FoundationInventoryGrid? _grid;
    private Control? _layer;
    private Control? _content;
    private Control? _popup;
    private Label? _message;
    private Label? _notice;
    private ulong _noticeUntil;
    private Control? _opener;
    private ColorRect? _preview;
    private Label? _previewGlyph;
    private bool _previewLegal;
    private LineEdit? _amountField;
    private Label? _amountText;
    private FoundationRunReadback? _readback;
    private string _version = "";
    private string _owner = "";
    private long _epoch;
    private long? _pending;
    private bool _dirty;
    private string _feedback = "";
    private Press? _press;
    private Model.Item? _menuItem;
    private (long id, Vector2 offset)? _movingBag;
    private ImmutableArray<long> _requestedOwn = [];
    private ImmutableArray<long> _requestedExternal = [];

    private FoundationRunView? Own => View(scope: "own");
    private FoundationRunView? External => AuthorizedExternal();

    private void Project()
    {
        try
        {
            EnsureLayer();
            Draw();
        }
        catch (Exception error) when (error is InvalidDataException or JsonException or KeyNotFoundException
                or InvalidOperationException)
        {
            Close();
            ShowNotice(text: "Inventory unavailable. Reopen it to try again.");
        }
    }

    private void EnsureLayer()
    {
        _gateway.RequireThread();
        if (_layer is not null)
        {
            return;
        }

        _art ??= new FoundationInventoryArt(gateway: _gateway);
        _grid ??= new FoundationInventoryGrid(gateway: _gateway, art: _art);
        _layer = new Control
        {
            Name = "InventoryWorkspace",
            ZIndex = 50,
            MouseFilter = Control.MouseFilterEnum.Stop,
            Theme = _art.Theme,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
        };
        _root.AddChild(node: _layer);
        _layer.SetAnchorsAndOffsetsPreset(preset: Control.LayoutPreset.FullRect);
        _layer.SetMeta(name: "capturing", value: true);
    }

    private void Draw()
    {
        _gateway.RequireThread();
        if (_layer is null)
        {
            return;
        }

        Control? control = _root.GetViewport().GuiGetFocusOwner();
        using NodePath? focused = _content is not null && control is not null && _content.IsAncestorOf(node: control)
                ? _content.GetPathTo(node: control) : null;
        _grid?.HideFocusDetails();
        Retire(node: _content);
        _targets.Clear();
        _bagPanels.Clear();
        Model.Rect bounds = Bounds();
        _content = Surface(parent: _layer, name: "InventoryPanels", frame: bounds, opacity: 0.88f);
        Button close = ButtonAt(parent: _content, name: "CloseInventory", text: "Close inventory",
                at: new Model.Rect(x: bounds.Width - 166, y: 4, width: 156, height: 44), action: Close);
        _message = LabelAt(parent: _content, text: _feedback, at: new Model.Rect(x: 16, y: 464,
                width: (_model.External ? NearbyLeft : bounds.Width) - 32, height: 46));
        DrawEquipment();
        DrawStorage();
        DrawExternal();
        DrawBags();
        PaintSlots(item: _model.Drag?.Item);
        RestoreFocus(previous: focused?.ToString(), fallback: _nearbyFocus ?? close);
        _dirty = false;
    }

    private void RestoreFocus(string? previous, Button fallback)
    {
        _gateway.RequireThread();
        if (_popup is not null || _content is null)
        {
            return;
        }

        Control? focus = previous is null ? null : _content.GetNodeOrNull<Control>(path: previous);
        (focus ?? fallback).GrabFocus();
    }

    private void DrawEquipment()
    {
        _gateway.RequireThread();
        if (_content is null || _art is null)
        {
            return;
        }

        LabelAt(parent: _content, text: "Equipment", at: new Model.Rect(x: 16, y: 4, width: 300, height: 36),
                heading: true);
        var silhouette = new TextureRect
        {
            Texture = _art.Silhouette(),
            Position = new Vector2(x: 120, y: 78),
            Size = new Vector2(x: 80, y: 280),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        _content.AddChild(node: silhouette);
        ImmutableArray<Model.Item> equipment = Items(view: Own);
        for (int index = 0; index < Model.Slots.Length; index++)
        {
            string slot = Model.Slots[index];
            Model.Item? item = equipment.FirstOrDefault(predicate: entry =>
                    StringComparer.Ordinal.Equals(x: entry.Location.Slot, y: slot));
            var rect = new Model.Rect(x: index < 6 ? 16 : 212, y: 48 + index % 6 * 59, width: 96, height: 51);
            DrawSlot(parent: _content, slot: slot, item: item, owner: _owner,
                    kind: "equipment", token: 0, frame: rect);
        }

        DrawQuick();
    }

    private void DrawStorage()
    {
        _gateway.RequireThread();
        if (_content is null)
        {
            return;
        }

        int width = _model.External ? 416 : Bounds().Width - 348;
        const int Left = 336;
        LabelAt(parent: _content, text: "Carried storage",
                at: new Model.Rect(x: Left, y: 4, width: width, height: 36), heading: true);
        VBoxContainer content = ScrollFlow(parent: _content, name: "CarriedStorage",
                at: new Model.Rect(x: Left, y: 50, width: width, height: 400));
        Model.Item[] storage = [.. Items(view: Own).Where(predicate: item => item.StorageWidth.HasValue)];
        if (storage.Length == 0)
        {
            TextLine(parent: content, text: "No storage equipped.\nEquip clothing, a belt or a backpack with storage.");
        }

        foreach (Model.Item item in storage)
        {
            AddStorage(parent: content, item: item, external: false, token: 0);
        }
    }

    private void DrawSlot(Control parent, string slot, Model.Item? item, string owner, string kind, long token,
            Model.Rect frame, ScrollContainer? clip = null)
    {
        _gateway.RequireThread();
        string label = SlotName(slot: slot);
        var button = new Button
        {
            Name = (token > 0 ? "nearby_" : "") + kind + "_" + slot,
            Text = item is null ? label + "\nEmpty" : label,
            TooltipText = item is null ? label + " empty" : "",
            Alignment = HorizontalAlignment.Center,
        };
        button.AddThemeFontSizeOverride(name: "font_size", fontSize: 12);
        Put(control: button, frame: frame);
        parent.AddChild(node: button);
        if (item is not null)
        {
            _grid?.Decorate(button: button, item: item, equipment: true);
            button.GuiInput += input => ItemInput(button: button, item: item, external: token > 0,
                    token: token, input: input);
        }

        _targets.Add(item: new TargetRegion(control: button, clip: clip, grid: null, slot: slot, owner: owner,
                kind: kind, occupied: item, token: token, quick: null));
    }

    private void DrawQuick()
    {
        _gateway.RequireThread();
        if (_content is null)
        {
            return;
        }

        JsonElement? header = Own?.Header;
        for (int slot = 1; slot <= 4; slot++)
        {
            int number = slot;
            JsonElement reference = header is { } data ? data.GetProperty(propertyName: "Quick").EnumerateArray()
                    .FirstOrDefault(predicate: row => row.GetProperty(propertyName: "Slot").GetInt32() == number)
                    : default;
            string title = number.ToString(provider: CultureInfo.InvariantCulture);
            var button = new Button
            {
                Name = "Quick_" + title,
                Text = title,
                TooltipText = reference.ValueKind == JsonValueKind.Object
                        ? "Slot " + title + " assigned" : "Slot " + title + " empty. Drag carried food here.",
            };
            Put(control: button, frame: new Model.Rect(x: 16 + (slot - 1) * 58, y: 408,
                    width: 52, height: 46));
            _content.AddChild(node: button);
            _targets.Add(item: new TargetRegion(control: button, clip: null, grid: null, slot: null,
                    owner: "", kind: "", occupied: null, token: 0, quick: slot));
        }
    }

    private void AddStorage(VBoxContainer parent, Model.Item item, bool external, long token)
    {
        _gateway.RequireThread();
        FoundationRunView? view = WindowView(id: item.Id, external: external);
        TextLine(parent: parent, text: _art?.Name(item: item) ?? item.Definition);
        if (view is null)
        {
            bool requested = DesiredWindows(root: external ? External : Own, external: external)
                    .Contains(item: item.Id);
            TextLine(parent: parent, text: requested ? "Opening inventory…" : "Close a bag to see this inventory.");
            return;
        }

        AddBoard(parent: parent, grid: Board(view: view, container: item, token: token, ancestors: [item.Id]),
                external: external);
    }

    private void AddBoard(VBoxContainer parent, Model.Grid grid, bool external,
            ImmutableArray<(int x, int y)> highlights = default)
    {
        _gateway.RequireThread();
        if (_grid is null)
        {
            return;
        }

        Control board = _grid.Add(parent: parent, grid: grid, highlights: highlights,
                itemInput: (button, item, input) => ItemInput(button: button, item: item, external: external,
                        token: grid.Token, input: input));
        _targets.Add(item: new TargetRegion(control: board, clip: parent.GetParent() as ScrollContainer,
                grid: grid, slot: null, owner: "", kind: "", occupied: null, token: grid.Token, quick: null));
    }

    private void DrawBags()
    {
        _gateway.RequireThread();
        if (_layer is null)
        {
            return;
        }

        foreach (Model.Window window in _model.Windows)
        {
            DrawBag(window: window);
        }
    }

    private void DrawBag(Model.Window window)
    {
        _gateway.RequireThread();
        if (_content is null || _layer is null)
        {
            return;
        }

        Model.Rect frame = Local(frame: window.Frame, origin: Bounds());
        Control panel = Surface(parent: _content, name: "Bag_" + window.Item.Id.ToString(
                provider: CultureInfo.InvariantCulture), frame: frame, opacity: 0.88f);
        panel.ZIndex = 2;
        _bagPanels[window.Item.Id] = panel;
        string title = _art?.Name(item: window.Item) ?? window.Item.Definition;
        var grip = new Button { Name = "MoveBag", Text = title, TooltipText = "Move " + title };
        Put(control: grip, frame: new Model.Rect(x: 8, y: 4, width: frame.Width - 98, height: 38));
        panel.AddChild(node: grip);
        grip.GuiInput += input => Grip(window: window, input: input);
        ButtonAt(parent: panel, name: "CloseBag_" + window.Item.Id.ToString(provider: CultureInfo.InvariantCulture),
                text: "Close bag", at: new Model.Rect(x: frame.Width - 88, y: 4, width: 82, height: 38),
                action: () => CloseBag(id: window.Item.Id));
        ImmutableArray<long> ancestors = Ancestors(window: window);
        LabelAt(parent: panel, text: (window.External ? "Nearby" : "Carried") + " / " + title + " · Depth "
                + ancestors.Length.ToString(provider: CultureInfo.InvariantCulture),
                at: new Model.Rect(x: 12, y: 43, width: frame.Width - 24, height: 28));
        VBoxContainer content = ScrollFlow(parent: panel, name: "BagStorage",
                at: new Model.Rect(x: 12, y: 76, width: frame.Width - 24, height: frame.Height - 88));
        if (WindowView(id: window.Item.Id, external: window.External) is { } view)
        {
            AddBoard(parent: content, grid: Board(view: view, container: window.Item, token: view.Token,
                    ancestors: ancestors), external: window.External);
            if (view.Rows.IsEmpty)
            {
                TextLine(parent: content, text: "Empty container");
            }
        }
        else
        {
            TextLine(parent: content, text: "Opening inventory…");
        }
    }

    private void RefreshWindows()
    {
        if (!_model.Open || _pending.HasValue || _coordinator() is not { } coordinator)
        {
            return;
        }

        ImmutableArray<long> own = DesiredWindows(root: Own, external: false);
        ImmutableArray<long> external = _model.External ? DesiredWindows(root: External, external: true) : [];
        if ((!own.SequenceEqual(second: _requestedOwn) || !external.SequenceEqual(second: _requestedExternal))
                && coordinator.RequestRunWorkspace(own: own, external: external))
        {
            _requestedOwn = own;
            _requestedExternal = external;
        }
    }

    private ImmutableArray<long> DesiredWindows(FoundationRunView? root, bool external)
    {
        IEnumerable<long> equipped = Items(view: root).Where(predicate: item => item.StorageWidth.HasValue
                && item.Location.Kind is "equipment" or "corpse").Select(selector: item => item.Id);
        return _model.RequestedWindows(external: external, equipped: equipped);
    }

    private FoundationRunView? View(string scope)
    {
        return _readback?.Views.TryGetValue(key: scope, value: out FoundationRunView? view) == true
                && view.Epoch == _epoch ? view : null;
    }

    private FoundationRunView? WindowView(long id, bool external)
    {
        long? token = external ? External?.Token : 0;
        return _readback?.Views.FirstOrDefault(predicate: entry =>
                FoundationProtocol.WindowScope(scope: entry.Key, external: external)
                        && entry.Value.Epoch == _epoch && entry.Value.Token == token
                        && entry.Value.Header.GetProperty(propertyName: "ContainerId").GetInt64() == id).Value;
    }

    private static ImmutableArray<Model.Item> Items(FoundationRunView? view)
    {
        return view is null ? [] : [.. view.Rows.Select(selector: Model.ReadItem)];
    }

    private static Model.Grid Board(FoundationRunView view, Model.Item? container, long token,
            ImmutableArray<long> ancestors)
    {
        return new Model.Grid(kind: container is null ? "stash" : "container",
                id: container?.Id ?? view.Header.GetProperty(propertyName: "Page").GetInt32() - 1,
                width: view.Header.GetProperty(propertyName: "Width").GetInt32(),
                height: view.Header.GetProperty(propertyName: "Height").GetInt32(), container: container,
                token: token, items: Items(view: view), ancestors: ancestors);
    }

    private ImmutableArray<long> Ancestors(Model.Window window)
    {
        var path = ImmutableArray.CreateBuilder<long>();
        Model.Window? current = window;
        while (current is not null && path.Count < 4)
        {
            path.Add(item: current.Item.Id);
            long? parent = current.Parent;
            current = _model.Windows.FirstOrDefault(predicate: candidate => candidate.Item.Id == parent);
            if (current is null && parent.HasValue)
            {
                path.Add(item: parent.Value);
            }
        }

        return path.ToImmutable();
    }

    private string Fingerprint()
    {
        return string.Join(separator: "|", values: _readback?.Views.OrderBy(keySelector: pair => pair.Key,
                comparer: StringComparer.Ordinal).Where(predicate: pair => pair.Key is not "status").Select(
                        selector: pair => pair.Key + pair.Value.Token.ToString(provider: CultureInfo.InvariantCulture)
                                + pair.Value.Header.GetRawText() + string.Join(separator: ",", values:
                                        pair.Value.Rows.Select(selector: row => pair.Key is "nearby"
                                                ? row.GetRawText() : row.GetProperty(
                                                        propertyName: "Stamp").GetString()))) ?? []);
    }

    private static string SlotName(string slot)
    {
        return slot switch
        {
            "body-armor" => "Body armor",
            "torso-clothing" => "Torso clothing",
            "left-hand" => "Left hand",
            "right-hand" => "Right hand",
            _ => char.ToUpperInvariant(c: slot[0]) + slot[1..],
        };
    }

    private sealed record TargetRegion
    {
        internal TargetRegion(Control control, ScrollContainer? clip, Model.Grid? grid, string? slot, string owner,
                string kind, Model.Item? occupied, long token, int? quick)
        {
            Control = control;
            Clip = clip;
            Grid = grid;
            Slot = slot;
            Owner = owner;
            Kind = kind;
            Occupied = occupied;
            Token = token;
            Quick = quick;
        }

        internal Control Control { get; init; }
        internal ScrollContainer? Clip { get; init; }
        internal Model.Grid? Grid { get; init; }
        internal string? Slot { get; init; }
        internal string Owner { get; init; }
        internal string Kind { get; init; }
        internal Model.Item? Occupied { get; init; }
        internal long Token { get; init; }
        internal int? Quick { get; init; }
    }

    private sealed record Press
    {
        internal Press(Button opener, Model.Item item, bool external, long token, Vector2 at, bool split)
        {
            Opener = opener;
            Item = item;
            External = external;
            Token = token;
            At = at;
            Split = split;
        }

        internal Button Opener { get; init; }
        internal Model.Item Item { get; init; }
        internal bool External { get; init; }
        internal long Token { get; init; }
        internal Vector2 At { get; init; }
        internal bool Split { get; init; }
    }
}
