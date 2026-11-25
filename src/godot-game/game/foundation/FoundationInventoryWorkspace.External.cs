using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using KingdomZombies.Core;
using Model = KingdomZombies.Foundation.FoundationInventoryInteraction;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationInventoryWorkspace
{
    internal string ExternalKind => External is { } view ? Model.ExternalTarget(header: view.Header).kind : "";

    internal int ExternalPage => External is { } view && ExternalKind is "Stash"
            ? view.Header.GetProperty(propertyName: "Page").GetInt32() : 0;

    internal void UpdateTargetMarker(Node2D world)
    {
        _gateway.RequireThread();
        if (!_model.Open || External is not { } view || _readback?.World is not { } data
                || WorldTarget(world: data, target: Model.ExternalTarget(header: view.Header)) is not { } point)
        {
            RemoveTargetMarker();
            return;
        }

        if (_targetMarker is null)
        {
            _targetMarker = new Polygon2D
            {
                Name = "NearbySelection",
                Polygon = [new Vector2(x: -9, y: -17), new Vector2(x: 0, y: -12),
                    new Vector2(x: 9, y: -17), new Vector2(x: 0, y: -7)],
                Color = new Color(r: 0.941f, g: 0.769f, b: 0.416f),
                ZIndex = 56,
            };
            world.AddChild(node: _targetMarker);
            var outline = new Line2D
            {
                Name = "NearbyOutline",
                DefaultColor = _targetMarker.Color,
                Width = 1.5f,
                Closed = true,
                Antialiased = true,
            };
            _targetMarker.AddChild(node: outline);
        }

        (string kind, string id) target = Model.ExternalTarget(header: view.Header);
        _targetMarker.SetMeta(name: "subject", value: target.id);
        Line2D outlineNode = _targetMarker.GetNode<Line2D>(path: "NearbyOutline");
        if (!StringComparer.Ordinal.Equals(x: outlineNode.GetMeta(name: "kind", @default: "").AsString(),
                y: target.kind))
        {
            (float width, float height) size = OutlineSize(kind: target.kind);
            outlineNode.Points = [new Vector2(x: -size.width, y: -size.height),
                new Vector2(x: size.width, y: -size.height),
                new Vector2(x: size.width, y: 2), new Vector2(x: -size.width, y: 2)];
            outlineNode.SetMeta(name: "kind", value: target.kind);
        }

        _targetMarker.Position = new Vector2(x: (float)((point.x
                - data.GetProperty(propertyName: "OriginX").GetDouble()) * 16 / 1000),
                y: 540 - (float)((point.y - data.GetProperty(propertyName: "Ground").GetDouble()) * 16 / 1000));
    }

    private const int NearbyLeft = 784;
    private (string kind, string id)? _selected;
    private (string kind, string id)? _selecting;
    private long _externalToken;
    private (long sequence, (string kind, string id) target)? _deposit;
    private (ImmutableArray<(int x, int y)> cells, int more)? _highlight;
    private ulong _highlightUntil;
    private bool _externalPending;
    private bool _externalDestination;
    private bool _showNearbyChoices;
    private string _externalFeedback = "";
    private Label? _nearbyMessage;
    private Button? _nearbyFocus;
    private Polygon2D? _targetMarker;

    private ImmutableArray<(string kind, string id)> Nearby => Model.Nearby(rows: View(scope: "nearby")?.Rows ?? []);
    private int NearbyCount => View(scope: "nearby")?.Header.GetProperty(propertyName: "Count").GetInt32() ?? 0;
    private int NearbyPage => View(scope: "nearby")?.Header.GetProperty(propertyName: "Page").GetInt32() ?? 1;
    private int NearbyPages => NearbyCount == 0 ? 1 : 1 + (NearbyCount - 1) / FoundationSession.NearbyPageSize;

    private FoundationRunView? AuthorizedExternal()
    {
        FoundationRunView? view = View(scope: "external");
        if (!_model.External || view is null)
        {
            return null;
        }

        (string kind, string id) target = Model.ExternalTarget(header: view.Header);
        long selected = View(scope: "nearby")?.Header.GetProperty(propertyName: "SelectedToken").GetInt64() ?? 0;
        return view.Token == selected && (_selecting is null || _selecting == target) ? view : null;
    }

    private void OpenNearby()
    {
        ImmutableArray<(string kind, string id)> targets = Nearby;
        if (External is null && NearbyCount == 1 && targets.Length == 1)
        {
            SelectNearby(target: targets[0]);
        }
        else if (NearbyCount > 1)
        {
            _showNearbyChoices = true;
        }
    }

    private void SelectNearby((string kind, string id) target)
    {
        if (!Nearby.Contains(item: target) || _pending.HasValue)
        {
            return;
        }

        _showNearbyChoices = false;
        _dirty = true;
        if (target == _selected)
        {
            return;
        }

        DepartExternal(keepOpen: true);
        _selecting = target;
        Send(command: Model.SelectTarget(target: target));
    }

    private void DrawExternal()
    {
        _gateway.RequireThread();
        _nearbyFocus = null;
        _nearbyMessage = null;
        if (_content is null || !_model.External)
        {
            return;
        }

        DrawNearbySelector(parent: _content);
        if (!ShowingTargetList() && External is { } view)
        {
            DrawNearbyContents(view: view, parent: _content);
        }

        _nearbyMessage = LabelAt(parent: _content, text: NearbyStatus(),
                at: new Model.Rect(x: NearbyLeft + 16, y: 460, width: 414, height: 49));
        _nearbyMessage.Name = "NearbyFeedback";
    }

    private void DrawNearbySelector(Control parent)
    {
        _gateway.RequireThread();
        LabelAt(parent: parent, text: "Nearby inventories",
                at: new Model.Rect(x: NearbyLeft + 16, y: 4, width: 235, height: 36), heading: true);
        if (ShowingTargetList())
        {
            DrawNearbyChoices(parent: parent, targets: Nearby);
            return;
        }

        if (_selected is { } target && External is not null)
        {
            Button current = ButtonAt(parent: parent, name: "NearbyTarget",
                    text: TargetLabel(target: target, distinguish: NearbyCount > 1),
                    at: new Model.Rect(x: NearbyLeft + 16, y: 43, width: 414, height: 37),
                    action: ToggleTargetList);
            current.Modulate = new Color(r: 0.941f, g: 0.769f, b: 0.416f);
            current.Disabled = NearbyCount <= 1 || _externalPending;
            current.TooltipText = current.Disabled ? current.Text : current.Text + ". Choose another inventory.";
            _nearbyFocus = current;
            return;
        }

        string empty = Own is null ? "Opening inventories…" : NearbyCount == 0
                ? "No nearby inventories.\nMove closer to a reachable stash, survivor or loot."
                : "Opening inventory…";
        LabelAt(parent: parent, text: empty, at: new Model.Rect(x: NearbyLeft + 16, y: 95,
                width: 414, height: 62));
    }

    private void DrawNearbyChoices(Control parent, ImmutableArray<(string kind, string id)> targets)
    {
        _gateway.RequireThread();
        LabelAt(parent: parent, text: "Choose a nearby inventory", at: new Model.Rect(
                x: NearbyLeft + 16, y: 42, width: 414, height: 38));
        int pages = NearbyPages;
        int page = NearbyPage;
        VBoxContainer rows = ScrollFlow(parent: parent, name: "NearbyChoices",
                at: new Model.Rect(x: NearbyLeft + 16, y: 85, width: 414, height: pages > 1 ? 322 : 370));
        if (pages > 1)
        {
            DrawListPages(parent: parent, page: page, pages: pages, items: false, change: RequestNearbyPage);
        }

        for (int index = 0; index < targets.Length; index++)
        {
            (string kind, string id) target = targets[index];
            bool duplicate = targets.Count(predicate: other =>
                    StringComparer.Ordinal.Equals(x: other.kind, y: target.kind)) > 1;
            Button choice = Option(parent: rows, text: TargetLabel(target: target, distinguish: duplicate),
                    action: () => SelectNearby(target: target));
            choice.Name = TargetControl(target: target);
            choice.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
            if (index == 0 || target == _selected)
            {
                _nearbyFocus = choice;
            }
            if (target == _selected)
            {
                choice.Modulate = new Color(r: 0.941f, g: 0.769f, b: 0.416f);
            }
        }
    }

    private void DrawListPages(Control parent, int page, int pages, bool items, Action<int> change)
    {
        _gateway.RequireThread();
        const int Left = NearbyLeft + 16;
        string suffix = items ? "NearbyItemsPage" : "NearbyPage";
        Button back = ButtonAt(parent: parent, name: "Previous" + suffix, text: "Previous",
                at: new Model.Rect(x: Left, y: 414, width: 95, height: 36),
                action: () => change(obj: page - 1));
        back.Disabled = page <= 1;
        LabelAt(parent: parent, text: Count(value: page) + " of " + Count(value: pages),
                at: new Model.Rect(x: Left + 109, y: 414, width: 194, height: 36));
        Button next = ButtonAt(parent: parent, name: "Next" + suffix, text: "Next",
                at: new Model.Rect(x: Left + 319, y: 414, width: 95, height: 36),
                action: () => change(obj: page + 1));
        next.Disabled = page >= pages;
    }

    private void RequestNearbyPage(int page)
    {
        _ = _coordinator()?.RequestNearbyPage(page: page);
    }

    private bool ShowingTargetList()
    {
        return !_externalPending && !_selecting.HasValue && !Nearby.IsEmpty
                && (_showNearbyChoices || External is null);
    }

    private void ToggleTargetList()
    {
        _showNearbyChoices = !_showNearbyChoices;
        _dirty = true;
    }

    private bool LeaveTargetList()
    {
        if (!_showNearbyChoices || External is null)
        {
            return false;
        }

        ToggleTargetList();
        return true;
    }

    private void DrawNearbyContents(FoundationRunView view, Control parent)
    {
        string kind = Model.ExternalTarget(header: view.Header).kind;
        int top = kind is "Stash" ? 132 : 87;
        JsonElement itemCount = view.Header.GetProperty(propertyName: "ItemCount");
        int itemsCount = itemCount.ValueKind == JsonValueKind.Number ? itemCount.GetInt32() : 0;
        const int Size = FoundationSession.ExternalItemsPageSize;
        int pages = itemsCount == 0 ? 1 : 1 + (itemsCount - 1) / Size;
        VBoxContainer rows = ScrollFlow(parent: parent, name: "NearbyStorage",
                at: new Model.Rect(x: NearbyLeft + 16, y: top, width: 414, height: (pages > 1 ? 407 : 458) - top));
        if (kind is "Stash")
        {
            DrawPager(parent: parent, view: view);
            DrawStash(rows: rows, view: view);
            return;
        }

        if (pages > 1)
        {
            int page = view.Header.GetProperty(propertyName: "ItemPage").GetInt32();
            DrawListPages(parent: parent, page: page, pages: pages, items: true,
                    change: number => RequestExternalItemsPage(token: view.Token, page: number));
        }

        ImmutableArray<Model.Item> items = Items(view: view);
        if (kind is "Npc" or "Corpse")
        {
            DrawNearbyGear(parent: rows, view: view, items: items);
        }

        foreach (Model.Item item in items.Where(predicate: item => item.Location.Kind is "ground"))
        {
            DrawGroundItem(parent: rows, item: item, token: view.Token);
        }
        if (items.IsEmpty)
        {
            TextLine(parent: rows, text: kind is "Corpse" ? "Empty corpse"
                    : kind is "Pile" ? "Empty loot pile" : "No items equipped.");
        }
    }

    private void RequestExternalItemsPage(long token, int page)
    {
        if (!_pending.HasValue && token == External?.Token
                && _coordinator()?.RequestExternalItemsPage(token: token, page: page) == true)
        {
            ClearExternalPreview();
        }
    }

    private void DrawPager(Control parent, FoundationRunView view)
    {
        _gateway.RequireThread();
        int page = view.Header.GetProperty(propertyName: "Page").GetInt32();
        int total = view.Header.GetProperty(propertyName: "PageCount").GetInt32();
        const int Left = NearbyLeft + 16;
        Button previous = ButtonAt(parent: parent, name: "PreviousStashPage", text: "Previous",
                at: new Model.Rect(x: Left, y: 88, width: 84, height: 35),
                action: () => Browse(text: Count(value: page - 1), view: view));
        previous.Disabled = page <= 1 || _externalPending;
        var input = new LineEdit { Name = "StashPage", Text = Count(value: page), PlaceholderText = "Page number",
            TooltipText = "Page number", Editable = !_externalPending, SelectAllOnFocus = true };
        Put(control: input, frame: new Model.Rect(x: Left + 88, y: 88, width: 58, height: 35));
        parent.AddChild(node: input);
        input.TextSubmitted += text => Browse(text: text, view: view);
        LabelAt(parent: parent, text: "of " + Count(value: total),
                at: new Model.Rect(x: Left + 154, y: 90, width: 65, height: 34));
        Button go = ButtonAt(parent: parent, name: "GoToStashPage", text: "Go to page",
                at: new Model.Rect(x: Left + 222, y: 88, width: 105, height: 35),
                action: () => BrowseEntry(input: input, view: view));
        go.Disabled = _externalPending;
        Button next = ButtonAt(parent: parent, name: "NextStashPage", text: "Next",
                at: new Model.Rect(x: Left + 330, y: 88, width: 84, height: 35),
                action: () => Browse(text: Count(value: page + 1), view: view));
        next.Disabled = page >= total || _externalPending;
    }

    private void BrowseEntry(LineEdit input, FoundationRunView view)
    {
        _gateway.RequireThread();
        Browse(text: input.Text, view: view);
    }

    private void DrawStash(VBoxContainer rows, FoundationRunView view)
    {
        AddBoard(parent: rows, grid: Board(view: view, container: null, token: view.Token, ancestors: []),
                external: true, highlights: _highlight?.cells ?? []);
        if (view.Rows.IsEmpty)
        {
            TextLine(parent: rows, text: "Empty stash page");
        }
    }

    private void DrawNearbyGear(VBoxContainer parent, FoundationRunView view, ImmutableArray<Model.Item> items)
    {
        _gateway.RequireThread();
        (string kind, string id) target = Model.ExternalTarget(header: view.Header);
        var gear = new Control { Name = "NearbyEquipment", CustomMinimumSize = new Vector2(x: 392, y: 252) };
        parent.AddChild(node: gear);
        string kind = target.kind is "Corpse" ? "corpse" : "equipment";
        for (int index = 0; index < Model.Slots.Length; index++)
        {
            string slot = Model.Slots[index];
            Model.Item? item = items.FirstOrDefault(predicate: row =>
                    StringComparer.Ordinal.Equals(x: row.Location.Slot, y: slot));
            DrawSlot(parent: gear, slot: slot, item: item, owner: target.id, kind: kind, token: view.Token,
                    frame: new Model.Rect(x: index % 3 * 132, y: index / 3 * 64, width: 124, height: 60),
                    clip: parent.GetParent() as ScrollContainer);
        }

        foreach (Model.Item item in items.Where(predicate: row => row.StorageWidth.HasValue
                && row.Location.Kind is "equipment" or "corpse"))
        {
            AddStorage(parent: parent, item: item, external: true, token: view.Token);
        }
    }

    private void DrawGroundItem(VBoxContainer parent, Model.Item item, long token)
    {
        _gateway.RequireThread();
        var row = new Button
        {
            Name = "NearbyItem_" + item.Id.ToString(provider: CultureInfo.InvariantCulture),
            Text = _art?.Name(item: item) ?? item.Definition,
            Size = new Vector2(x: 390, y: 53),
            CustomMinimumSize = new Vector2(x: 160, y: 53),
            TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis,
            Alignment = HorizontalAlignment.Left,
        };
        parent.AddChild(node: row);
        _grid?.Decorate(button: row, item: item, equipment: true);
        row.GuiInput += input => ItemInput(button: row, item: item, external: true, token: token, input: input);
    }

    private void Browse(string text, FoundationRunView view)
    {
        if (_pending.HasValue || view.Token != External?.Token)
        {
            return;
        }

        int page = view.Header.GetProperty(propertyName: "Page").GetInt32();
        int total = view.Header.GetProperty(propertyName: "PageCount").GetInt32();
        if (Model.PageRequest(text: text, current: page, total: total, token: view.Token) is { } command)
        {
            ClearExternalPreview();
            Send(command: command);
        }
        else
        {
            NearbyFeedback(text: Model.Number(text: text, maximum: total) == page ? ""
                    : "Enter a whole page number from 1 to " + Count(value: total) + ".");
        }
    }

    private void InspectExternal()
    {
        _gateway.RequireThread();
        if (!_model.External)
        {
            return;
        }

        if (External is not { } view)
        {
            InspectMissingTarget();
        }
        else
        {
            (string kind, string id) target = Model.ExternalTarget(header: view.Header);
            if (_externalToken != 0 && (_externalToken != view.Token || _selected != target))
            {
                DepartExternal(keepOpen: true);
            }

            _selected = target;
            _selecting = null;
            _externalToken = view.Token;
            if (_deposit is { } expected && Model.DepositHighlight(header: view.Header,
                    target: expected.target, sequence: expected.sequence) is { } effect)
            {
                _highlight = effect;
                _highlightUntil = Time.GetTicksMsec() + 2000;
                _deposit = null;
                _dirty = true;
            }
        }

        if (_highlight.HasValue && Time.GetTicksMsec() >= _highlightUntil)
        {
            _highlight = null;
            _dirty = true;
        }
    }

    private void InspectMissingTarget()
    {
        if (_selected is not { } target || View(scope: "nearby") is not { } current)
        {
            return;
        }

        bool available = current.Header.GetProperty(propertyName: "SelectedToken").GetInt64() == _externalToken;
        bool lost = !available || current.Header.GetProperty(propertyName: "Availability").GetString() is "AccessLost";
        if (!lost || _selecting.HasValue && _selecting != target)
        {
            if (!_selecting.HasValue)
            {
                ClearExternalPreview();
            }
            return;
        }

        bool siblings = NearbyCount > 0;
        bool disappeared = target.kind is "Corpse" or "Pile" && _readback?.World is { } world
                && WorldTarget(world: world, target: target) is null;
        string title = TargetLabel(target: target, distinguish: false);
        string text = disappeared ? title + " is no longer available. Choose another nearby target."
                : title + " is out of reach. Move closer to reopen.";
        DepartExternal(keepOpen: siblings);
        _selected = null;
        _selecting = null;
        _externalToken = 0;
        _deposit = null;
        NearbyFeedback(text: text);
        if (!siblings)
        {
            ShowNotice(text: "Move closer to a reachable inventory.");
        }
    }

    private void ClearExternalPreview()
    {
        if (_model.Drag is { External: true } || _externalDestination || _press is { External: true })
        {
            CancelPreview();
        }
    }

    private void DepartExternal(bool keepOpen)
    {
        ClearExternalPreview();
        if (_menuItem is { } item && !OwnItem(item: item))
        {
            RemovePopup();
        }
        if (_movingBag is { } moving && _model.Windows.Any(predicate: window =>
                window.Item.Id == moving.id && window.External))
        {
            _movingBag = null;
        }

        _model.AccessLost(keepExternal: keepOpen);
        _highlight = null;
        RemoveTargetMarker();
        _dirty = true;
    }

    private void CloseExternal()
    {
        if (External is { } view)
        {
            _ = _coordinator()?.SubmitRunAction(kind: "close", request: JsonSerializer.SerializeToElement(
                    value: new { view.Token }));
        }

        ResetExternal();
    }

    private void ResetExternal()
    {
        RemoveTargetMarker();
        _selected = null;
        _selecting = null;
        _externalToken = 0;
        _externalFeedback = "";
        _nearbyMessage = null;
        _nearbyFocus = null;
        _deposit = null;
        _highlight = null;
        _externalPending = false;
        _externalDestination = false;
        _showNearbyChoices = false;
    }

    private void RemoveTargetMarker()
    {
        _gateway.RequireThread();
        if (_targetMarker is not null)
        {
            _targetMarker.GetParent()?.RemoveChild(node: _targetMarker);
            _targetMarker.QueueFree();
            _targetMarker = null;
        }
    }

    private bool OwnItem(Model.Item item)
    {
        return item.Location.Kind is "equipment" && StringComparer.Ordinal.Equals(
                x: item.Location.TargetId, y: _owner) || item.Location.Kind is "container"
                        && item.Location.GridId is { } id && WindowView(id: id, external: false) is not null;
    }

    private void NearbyFeedback(string text)
    {
        _gateway.RequireThread();
        _externalFeedback = text;
        if (_nearbyMessage is not null)
        {
            _nearbyMessage.Text = NearbyStatus();
        }
    }

    private string NearbyStatus()
    {
        return _highlight?.more is > 0 and { } count && _externalFeedback.Length == 0
                ? Count(value: count) + (count == 1 ? " more page affected." : " more pages affected.")
                : _externalFeedback;
    }

    private static string TargetLabel((string kind, string id) target, bool distinguish)
    {
        string label = target.kind switch { "Npc" => "Survivor", "Pile" => "Loot pile", _ => target.kind };
        return distinguish ? label + " · " + target.id : label;
    }

    private static string TargetControl((string kind, string id) target)
    {
        return "NearbyChoice_" + (target.kind is "Stash" ? "Stash" : target.kind + "_"
                + Convert.ToHexString(inArray: Encoding.UTF8.GetBytes(s: target.id)));
    }

    private static (float width, float height) OutlineSize(string kind)
    {
        return kind switch
        {
            "Npc" => (15, 42),
            "Corpse" => (22, 12),
            _ => (12, 10),
        };
    }

    private static (double x, double y)? WorldTarget(JsonElement world, (string kind, string id) target)
    {
        string scope = target.kind switch { "Npc" => "Actors", "Corpse" => "Corpses", _ => "Loot" };
        JsonElement row = world.GetProperty(propertyName: scope).EnumerateArray().FirstOrDefault(predicate: entry =>
                StringComparer.Ordinal.Equals(x: entry.GetProperty(propertyName: "Id").GetString(), y: target.id));
        return row.ValueKind == JsonValueKind.Object ? (row.GetProperty(propertyName: "X").GetDouble(),
                row.GetProperty(propertyName: "Y").GetDouble()) : null;
    }

    private static string Count(int value)
    {
        return value.ToString(provider: CultureInfo.InvariantCulture);
    }
}
