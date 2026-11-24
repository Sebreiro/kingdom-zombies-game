using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Godot;
using Model = KingdomZombies.Foundation.FoundationInventoryInteraction;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationInventoryWorkspace
{
    internal bool Input(InputEvent input)
    {
        _gateway.RequireThread();
        if (!_model.Open)
        {
            return false;
        }

        if (input is InputEventKey { Pressed: true, Echo: false, Keycode: Key.Escape })
        {
            return Escape();
        }

        if (input is InputEventKey { Pressed: true, Echo: false, PhysicalKeycode: Key.R } && _model.Rotate())
        {
            PaintPreview(point: _root.GetViewport().GetMousePosition());
            return true;
        }
        if (input is InputEventMouseMotion motion)
        {
            PointerMove(point: motion.Position);
            return _model.Drag is not null || _movingBag.HasValue;
        }
        if (input is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false } release)
        {
            return PointerRelease(point: release.Position);
        }

        return false;
    }

    private void ItemInput(Button button, Model.Item item, bool external, long token, InputEvent input)
    {
        _gateway.RequireThread();
        if (_pending.HasValue || external && token != External?.Token)
        {
            return;
        }

        if (input is InputEventMouseButton { Pressed: true } mouse)
        {
            if (mouse.ButtonIndex == MouseButton.Right)
            {
                ItemMenu(button: button, item: item, external: external);
            }
            else if (mouse.ButtonIndex == MouseButton.Left && mouse.CtrlPressed && External is { } target
                    && (external || target.Header.GetProperty(propertyName: "Kind").GetString() is "Stash"))
            {
                Send(command: Model.Quick(item: item, token: target.Token, deposit: !external), nearby: true);
            }
            else if (mouse.ButtonIndex == MouseButton.Left)
            {
                _press = new Press(opener: button, item: item, external: external, token: token,
                        at: mouse.GlobalPosition, split: mouse.ShiftPressed);
            }
        }
        else if (input is InputEventKey { Pressed: true, Echo: false } key
                && (key.Keycode == Key.Menu || key.ShiftPressed && key.Keycode == Key.F10))
        {
            ItemMenu(button: button, item: item, external: external);
            _root.GetViewport().SetInputAsHandled();
        }
    }

    private void PointerMove(Vector2 point)
    {
        _gateway.RequireThread();
        if (_movingBag is { } bag)
        {
            MoveBag(id: bag.id, point: point - bag.offset);
            return;
        }

        if (_model.Drag is null && _press is { } press && point.DistanceTo(to: press.At) >= 4
                && _model.Begin(item: press.Item, external: press.External, token: press.Token, split: press.Split))
        {
            _grid?.HideFocusDetails();
            _opener = press.Opener;
            if (_model.Drag?.ChoosingAmount == true)
            {
                SplitMenu(item: press.Item);
                return;
            }
        }

        PaintPreview(point: point);
    }

    private bool PointerRelease(Vector2 point)
    {
        _gateway.RequireThread();
        _press = null;
        if (_movingBag.HasValue)
        {
            _movingBag = null;
            _dirty = true;
            return true;
        }
        if (_model.Drag is not { ChoosingAmount: false })
        {
            return false;
        }

        (Model.Placement placement, _) = PlacementAt(point: point);
        if (_model.Release(placement: placement) is { } action)
        {
            Send(command: action, nearby: _externalDestination || action.Request.TryGetProperty(
                    propertyName: "Token", value: out System.Text.Json.JsonElement token) && token.GetInt64() > 0);
        }
        else if (placement.Reason is not "Outside")
        {
            ReportAction(text: Copy(reason: placement.Reason), nearby: _externalDestination);
        }

        Retire(node: _preview);
        _preview = null;
        _previewGlyph = null;
        _externalDestination = false;
        PaintSlots(item: null);
        return true;
    }

    private (Model.Placement placement, Vector2 origin) PlacementAt(Vector2 point)
    {
        _gateway.RequireThread();
        TargetRegion? target = _targets.LastOrDefault(predicate: region => Hits(region: region, point: point));
        _externalDestination = target?.Token > 0;
        if (target is null || _model.Drag is not { } drag)
        {
            return (new Model.Placement(reason: "Outside"), point);
        }

        Rect2 box = target.Control.GetGlobalRect();
        if (target.Quick is { } slot)
        {
            return (new Model.Placement(reason: "Incompatible", action: drag.Item.Food && !drag.External
                    && drag.Amount is null ? Model.Assign(item: drag.Item, slot: slot) : null), box.Position);
        }
        if (target.Slot is { } equipment)
        {
            return (_model.CheckSlot(slot: equipment, owner: target.Owner, kind: target.Kind,
                    occupied: target.Occupied, token: target.Token), box.Position);
        }
        if (target.Grid is not { } grid)
        {
            return (new Model.Placement(reason: "Outside"), box.Position);
        }

        Vector2 offset = point - box.Position;
        int x = (int)(offset.X / FoundationInventoryGrid.Pitch);
        int y = (int)(offset.Y / FoundationInventoryGrid.Pitch);
        return (_model.CheckGrid(grid: grid, x: x, y: y), box.Position);
    }

    private bool Hits(TargetRegion region, Vector2 point)
    {
        _gateway.RequireThread();
        return region.Control.GetGlobalRect().HasPoint(point: point)
                && (region.Clip is null || region.Clip.GetGlobalRect().HasPoint(point: point));
    }

    private void PaintPreview(Vector2 point)
    {
        _gateway.RequireThread();
        if (_model.Drag is not { ChoosingAmount: false } drag || _layer is null)
        {
            return;
        }

        (Model.Placement placement, Vector2 origin) = PlacementAt(point: point);
        if (_preview is null)
        {
            _preview = new ColorRect { Name = "PlacementPreview", ZIndex = 12,
                MouseFilter = Control.MouseFilterEnum.Ignore };
            _layer.AddChild(node: _preview);
            _preview.Draw += DrawPreviewOutline;
            _previewGlyph = TextLine(parent: _preview, text: "");
            _previewGlyph.AddThemeFontSizeOverride(name: "font_size", fontSize: 22);
        }
        int pitch = FoundationInventoryGrid.Pitch;
        Model.Rect area = placement.Area ?? new Model.Rect(x: 0, y: 0, width: drag.Width, height: drag.Height);
        _preview.Position = origin + new Vector2(x: area.X * pitch, y: area.Y * pitch);
        _preview.Size = new Vector2(x: area.Width * pitch, y: area.Height * pitch);
        _previewLegal = placement.Action is not null;
        _preview.Color = _previewLegal ? new Color(r: 0.63f, g: 0.83f, b: 0.70f, a: 0.55f)
                : new Color(r: 0.72f, g: 0.28f, b: 0.25f, a: 0.55f);
        if (_previewGlyph is not null)
        {
            _previewGlyph.Text = _previewLegal ? "✓" : "×";
        }

        _preview.QueueRedraw();
        PaintSlots(item: drag.Item);
    }

    private void DrawPreviewOutline()
    {
        _gateway.RequireThread();
        if (_preview is not { } preview)
        {
            return;
        }

        Vector2 size = preview.Size;
        if (_previewLegal)
        {
            preview.DrawRect(rect: new Rect2(position: Vector2.Zero, size: size), color: Colors.PaleGreen,
                    filled: false, width: 3);
            return;
        }

        var right = new Vector2(x: size.X, y: 0);
        var bottom = new Vector2(x: 0, y: size.Y);
        preview.DrawDashedLine(from: Vector2.Zero, to: right, color: Colors.LightCoral, width: 3, dash: 6);
        preview.DrawDashedLine(from: right, to: size, color: Colors.LightCoral, width: 3, dash: 6);
        preview.DrawDashedLine(from: size, to: bottom, color: Colors.LightCoral, width: 3, dash: 6);
        preview.DrawDashedLine(from: bottom, to: Vector2.Zero, color: Colors.LightCoral, width: 3, dash: 6);
    }

    private void PaintSlots(Model.Item? item)
    {
        _gateway.RequireThread();
        foreach (TargetRegion region in _targets)
        {
            if (region.Slot is not { } slot)
            {
                continue;
            }

            region.Control.Modulate = item is not null && Model.Compatible(item: item, slot: slot)
                    ? region.Occupied is null ? Colors.PaleGreen : Colors.LightCoral : Colors.White;
        }
    }

    private void ItemMenu(Button button, Model.Item item, bool external)
    {
        _gateway.RequireThread();
        _grid?.HideFocusDetails();
        CancelPreview();
        RemovePopup();
        _opener = button;
        _menuItem = item;
        Rect2 source = button.GetGlobalRect();
        VBoxContainer options = Popup(title: _art?.Name(item: item) ?? item.Definition, source: source);
        if (item.StorageWidth.HasValue)
        {
            Option(parent: options, text: "Open", action: () =>
                    OpenBag(button: button, item: item, external: external));
        }

        if (!external && item.Food)
        {
            Option(parent: options, text: "Assign to quick slot", action: () => AssignMenu(item: item, source: source));
            Option(parent: options, text: "Consume", action: () => MenuAction(command: Model.Consume(item: item)))
                    .TooltipText = "Consume one " + (_art?.Name(item: item) ?? item.Definition);
        }
        if (!external)
        {
            TextLine(parent: options, text: "Quantity " + item.Quantity.ToString(
                    provider: CultureInfo.InvariantCulture));
            Option(parent: options, text: "Drop", action: () => MenuAction(command: Model.Drop(item: item)));
        }

        Option(parent: options, text: "Close", action: RemovePopup);
        FocusFirst(options: options);
    }

    private void AssignMenu(Model.Item item, Rect2 source)
    {
        _gateway.RequireThread();
        RemovePopup();
        _menuItem = item;
        VBoxContainer options = Popup(title: "Assign to quick slot", source: source);
        for (int index = 1; index <= 4; index++)
        {
            int slot = index;
            Option(parent: options, text: "Slot " + slot.ToString(provider: CultureInfo.InvariantCulture),
                    action: () => MenuAction(command: Model.Assign(item: item, slot: slot)));
        }

        FocusFirst(options: options);
    }

    private void MenuAction(Model.Command command)
    {
        _gateway.RequireThread();
        RemovePopup();
        Send(command: command);
    }

    private void SplitMenu(Model.Item item)
    {
        _gateway.RequireThread();
        RemovePopup();
        Vector2 pointer = _root.GetViewport().GetMousePosition();
        VBoxContainer options = Popup(title: "Split stack",
                source: new Rect2(position: pointer, size: Vector2.Zero));
        _amountField = new LineEdit
        {
            Name = "QuantityToMove",
            PlaceholderText = "Quantity to move",
            Text = (item.Quantity / 2).ToString(provider: CultureInfo.InvariantCulture),
        };
        options.AddChild(node: _amountField);
        _amountText = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart };
        options.AddChild(node: _amountText);
        _amountField.TextChanged += value => UpdateSplit(text: value, total: item.Quantity);
        _amountField.TextSubmitted += value => ConfirmSplit(text: value, total: item.Quantity);
        Option(parent: options, text: "Half", action: () => Half(total: item.Quantity));
        Option(parent: options, text: "Move quantity", action: () => ConfirmSplitField(total: item.Quantity));
        Option(parent: options, text: "Cancel split", action: CancelPreview);
        UpdateSplit(text: _amountField.Text, total: item.Quantity);
        _amountField.GrabFocus();
        _amountField.SelectAll();
    }

    private void Half(int total)
    {
        _gateway.RequireThread();
        if (_amountField is not null)
        {
            _amountField.Text = (total / 2).ToString(provider: CultureInfo.InvariantCulture);
            UpdateSplit(text: _amountField.Text, total: total);
        }
    }

    private void ConfirmSplitField(int total)
    {
        _gateway.RequireThread();
        ConfirmSplit(text: _amountField?.Text ?? "", total: total);
    }

    private void UpdateSplit(string text, int total)
    {
        _gateway.RequireThread();
        if (_amountText is not null)
        {
            _amountText.Text = Model.Number(text: text, maximum: total - 1) is { } amount
                    ? "Moving " + amount.ToString(provider: CultureInfo.InvariantCulture) + " · Remaining "
                            + (total - amount).ToString(provider: CultureInfo.InvariantCulture)
                    : "Enter a whole number from 1 to " + (total - 1).ToString(
                            provider: CultureInfo.InvariantCulture) + ".";
        }
    }

    private void ConfirmSplit(string text, int total)
    {
        _gateway.RequireThread();
        if (_model.Amount(text: text))
        {
            RemovePopup();
            PaintPreview(point: _root.GetViewport().GetMousePosition());
        }
        else
        {
            UpdateSplit(text: text, total: total);
        }
    }

    private void OpenBag(Button button, Model.Item item, bool external)
    {
        _gateway.RequireThread();
        Rect2 source = button.GetGlobalRect();
        long? parent = item.Location.Kind is "container" ? item.Location.GridId : null;
        RemovePopup();
        ImmutableArray<long> requested = DesiredWindows(root: external ? External : Own, external: external);
        if (requested.Length >= 16 && !requested.Contains(item: item.Id))
        {
            ReportAction(text: "Close a bag before opening another.", nearby: external);
            return;
        }

        if (_model.OpenBag(item: item, parent: parent, external: external, source: From(rect: source),
                bounds: Bounds()) is not null)
        {
            _dirty = true;
            RefreshWindows();
        }
    }

    private void Grip(Model.Window window, InputEvent input)
    {
        _gateway.RequireThread();
        if (input is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true } mouse)
        {
            _movingBag = (window.Item.Id, mouse.GlobalPosition - new Vector2(x: window.Frame.X, y: window.Frame.Y));
            if (_bagPanels.TryGetValue(key: window.Item.Id, value: out Control? panel))
            {
                panel.GetParent().MoveChild(childNode: panel, toIndex: -1);
            }
        }
    }

    private void MoveBag(long id, Vector2 point)
    {
        _gateway.RequireThread();
        Model.Rect bounds = Bounds();
        if (_model.MoveBag(id: id, x: (int)point.X, y: (int)point.Y, bounds: bounds) is { } moved
                && _bagPanels.TryGetValue(key: id, value: out Control? panel))
        {
            Put(control: panel, frame: Local(frame: moved.Frame, origin: bounds));
        }
    }

    private void CloseBag(long id)
    {
        _model.CloseBag(id: id);
        _dirty = true;
        RefreshWindows();
    }

    private bool Escape()
    {
        _gateway.RequireThread();
        if (!_model.Open)
        {
            return false;
        }

        if (_model.Drag is not null || _movingBag.HasValue)
        {
            CancelPreview();
        }
        else if (_popup is not null)
        {
            RemovePopup();
        }
        else if (LeaveTargetList())
        {
            return true;
        }
        else if (_model.Windows.Count > 0)
        {
            CloseBag(id: _model.Windows[^1].Item.Id);
        }
        else
        {
            Close();
        }

        return true;
    }

    private void Close()
    {
        _gateway.RequireThread();
        CloseExternal();
        _model.Close();
        RemoveLayer();
        _pending = null;
        _feedback = "";
        _requestedOwn = [];
        _requestedExternal = [];
        _ = _coordinator()?.RequestRunWorkspace(own: [], external: []);
    }

    private void CancelPreview()
    {
        _gateway.RequireThread();
        _model.Cancel();
        _movingBag = null;
        _press = null;
        _externalDestination = false;
        PaintSlots(item: null);
        Retire(node: _preview);
        _preview = null;
        _previewGlyph = null;
        if (_amountField is not null)
        {
            RemovePopup();
        }
    }

    private void RemovePopup()
    {
        _gateway.RequireThread();
        Control? previous = _opener;
        Retire(node: _popup);
        _popup = null;
        _amountField = null;
        _amountText = null;
        _menuItem = null;
        if (previous is not null && GodotObject.IsInstanceValid(instance: previous)
                && previous.IsVisibleInTree())
        {
            previous.GrabFocus();
        }
    }

    private void RemoveLayer()
    {
        _gateway.RequireThread();
        _grid?.HideFocusDetails();
        CancelPreview();
        RemovePopup();
        Retire(node: _layer);
        _layer = null;
        _content = null;
        _message = null;
        _opener = null;
        _targets.Clear();
        _bagPanels.Clear();
    }

    private void InspectSurfaces()
    {
        _gateway.RequireThread();
        InspectExternal();
        if (_menuItem is { } selected && !Current(item: selected))
        {
            RemovePopup();
        }
        if (_model.Drag is { } drag && !Current(item: drag.Item))
        {
            CancelPreview();
            ReportAction(text: Copy(reason: "StaleItem"), nearby: drag.External);
        }

        foreach (Model.Window bag in _model.Windows.ToArray())
        {
            if (!Current(item: bag.Item, stamp: false))
            {
                CloseBag(id: bag.Item.Id);
            }
        }
    }

    private bool Current(Model.Item item, bool stamp = true)
    {
        bool owned = item.Location.Kind is "equipment" && StringComparer.Ordinal.Equals(
                x: item.Location.TargetId, y: _owner);
        FoundationRunView? view = item.Location.GridId is { } grid && item.Location.Kind is "container"
                ? WindowView(id: grid, external: false) ?? WindowView(id: grid, external: true)
                : owned ? Own : External;
        return view?.Rows.Any(predicate: row => row.GetProperty(propertyName: "Id").GetInt64() == item.Id
                && (!stamp || StringComparer.Ordinal.Equals(
                        x: row.GetProperty(propertyName: "Stamp").GetString(), y: item.Stamp))) == true;
    }

    private void Send(Model.Command command, bool nearby = false)
    {
        _gateway.RequireThread();
        if (_pending.HasValue || _coordinator()?.SubmitRunAction(kind: command.Kind, request: command.Request)
                is not { } receipt)
        {
            return;
        }

        nearby |= command.Kind is "select" or "browse";
        _highlight = null;
        _deposit = command.Kind is "quick" && command.Request.GetProperty(propertyName: "Deposit").GetBoolean()
                && _selected is { } stash ? (receipt.sequence, stash) : null;
        _dirty = true;
        if (receipt.status is "Pending")
        {
            _pending = receipt.sequence;
            _externalPending = nearby;
            ReportAction(text: command.Kind switch
            {
                "select" => "Opening inventory…",
                "browse" => "Opening stash page…",
                "food" => "Consuming food…",
                _ => "Moving item…",
            }, nearby: nearby);
        }
        else
        {
            CompleteAction(status: receipt.status, nearby: nearby);
        }
    }

    private void UpdateFeedback()
    {
        _gateway.RequireThread();
        if (_pending is { } pending && _readback?.Feedback is { } feedback && feedback.sequence == pending)
        {
            _pending = null;
            CompleteAction(status: feedback.status, nearby: _externalPending);
            _externalPending = false;
            _dirty = true;
        }
    }

    private void CompleteAction(string status, bool nearby)
    {
        if (nearby && status is not ("Ready" or "Accepted" or "Applied"))
        {
            _selecting = null;
            _deposit = null;
        }

        ReportAction(text: Copy(reason: status), nearby: nearby);
    }

    private void ReportAction(string text, bool nearby)
    {
        if (nearby)
        {
            NearbyFeedback(text: text);
        }
        else
        {
            Feedback(text: text);
        }
    }

    private void Feedback(string text)
    {
        _gateway.RequireThread();
        _feedback = text;
        if (_message is not null)
        {
            _message.Text = text;
        }
    }

    private static string Copy(string reason)
    {
        return reason switch
        {
            "Ready" or "Accepted" or "Applied" or "Consumed" => "",
            "StaleItem" or "StaleRequest" or "StaleFood" =>
                    "That item moved. Choose an item from the updated inventory.",
            "AccessLost" => "Move closer to a reachable inventory.",
            "Occupied" => "Unequip the current item before replacing it.",
            "Capacity" => "No room in this storage. Make space or choose another stack.",
            "InvalidPage" => "That stash page is unavailable. Choose a page from the updated inventory.",
            "Cycle" => "A container cannot hold itself. Choose another container.",
            "Nesting" => "Container nesting limit reached. Choose a shallower grid.",
            "Cooldown" => "Food cooling down.",
            _ => "Item does not fit. Rotate it or choose another grid.",
        };
    }

}
