using System;
using System.Globalization;
using Godot;
using KingdomZombies.Proofs;
using Hud = KingdomZombies.Foundation.FoundationHudState;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationHud
{
    public void Dispose()
    {
        _gateway.RequireThread();
        Hide();
        _panel?.QueueFree();
        _damage?.QueueFree();
        _art?.Dispose();
    }

    internal FoundationHud(Control root, MainThreadGateway gateway, Action open)
    {
        _root = root;
        _gateway = gateway;
        _open = open;
    }

    internal Hud? Current { get; private set; }

    internal void Observe(FoundationRunReadback? readback, FoundationCoordinator coordinator, bool browsing, long now)
    {
        _gateway.RequireThread();
        if (coordinator.WaitingForHost || coordinator.LocalRunControl is not { generation: > 0 } control)
        {
            Hide();
            return;
        }

        Hud? next = readback?.Views.TryGetValue(key: "status", value: out FoundationRunView? scope) == true
                ? Hud.Read(view: scope, survivor: control.survivorId, generation: control.generation) : null;
        if (next is null)
        {
            if (Current?.Generation != control.generation)
            {
                Hide();
            }

            return;
        }

        Build();
        if (Hud.ShowsDamage(previous: Current, next: next))
        {
            _pulseUntil = now + 250;
        }

        Current = next;
        UpdateVitals(state: next, note: ActiveNotice(receipt: coordinator.VitalsHudFeedback, state: next, now: now),
                pulse: Math.Clamp(value: (float)(_pulseUntil - now) / 250, min: 0, max: 1));
        UpdateActions(state: next, note: ActiveNotice(receipt: coordinator.ActionHudFeedback, state: next, now: now),
                browsing: browsing);
    }

    internal void Hide()
    {
        _gateway.RequireThread();
        Current = null;
        _pulseUntil = 0;
        _sprintUntil = 0;
        _previousSprint = "";
        if (_panel is not null)
        {
            _panel.Visible = false;
        }
        if (_damage is not null)
        {
            _damage.Visible = false;
        }
    }

    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly Action _open;
    private FoundationInventoryArt? _art;
    private Control? _panel;
    private Control? _damage;
    private Label? _money;
    private Label? _load;
    private Label? _speed;
    private Label? _rest;
    private Label? _movementNotice;
    private Label? _actionNotice;
    private Meter? _health;
    private Meter? _stamina;
    private Tile? _left;
    private Tile? _right;
    private Tile? _ammo;
    private readonly Tile?[] _food = new Tile?[4];
    private Button? _openButton;
    private long _pulseUntil;
    private long _sprintUntil;
    private string _previousSprint = "";

    private void Build()
    {
        _gateway.RequireThread();
        if (_panel is not null)
        {
            return;
        }

        _art = new FoundationInventoryArt(gateway: _gateway);
        _panel = new Control { Name = "RunHud", Theme = _art.Theme, MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 15 };
        _root.AddChild(node: _panel);
        _panel.SetAnchorsAndOffsetsPreset(preset: Control.LayoutPreset.FullRect);
        BuildVitals(parent: _panel);
        BuildActions(parent: _panel);
        BuildDamage();
        Label hint = _root.GetNode<Label>(path: "Hint");
        Place(control: hint, at: new Rect2(x: 702, y: 22, width: 224, height: 82));
        Control roster = _root.GetNode<Control>(path: "Roster");
        roster.Position = new Vector2(x: 936, y: 4);
    }

    private void BuildVitals(Control parent)
    {
        _gateway.RequireThread();
        _health = AddMeter(parent: parent, name: "Health", at: new Rect2(x: 24, y: 24, width: 264, height: 26),
                fill: new Color(r: 0.784f, g: 0.424f, b: 0.392f));
        _stamina = AddMeter(parent: parent, name: "Stamina", at: new Rect2(x: 24, y: 56, width: 264, height: 26),
                fill: new Color(r: 0.569f, g: 0.733f, b: 0.643f));
        _rest = Text(parent: parent, name: "RestState", at: new Rect2(x: 304, y: 24, width: 138, height: 26));
        _money = Text(parent: parent, name: "CarriedMoney", at: new Rect2(x: 452, y: 24, width: 230, height: 26));
        _load = Text(parent: parent, name: "CarriedLoad", at: new Rect2(x: 304, y: 55, width: 350, height: 25));
        _speed = Text(parent: parent, name: "LoadModifiers", at: new Rect2(x: 304, y: 81, width: 384, height: 24));
        _movementNotice = Text(parent: parent, name: "MovementNotice",
                at: new Rect2(x: 24, y: 84, width: 274, height: 26));
    }

    private void BuildActions(Control parent)
    {
        _gateway.RequireThread();
        var row = new Control { Name = "Actions", MouseFilter = Control.MouseFilterEnum.Ignore };
        parent.AddChild(node: row);
        row.SetAnchorsAndOffsetsPreset(preset: Control.LayoutPreset.BottomWide);
        row.OffsetTop = -72;
        row.OffsetBottom = -24;
        _left = AddTile(parent: row, label: "Left hand", x: 24, width: 106);
        _right = AddTile(parent: row, label: "Right hand", x: 138, width: 106);
        _ammo = AddTile(parent: row, label: "Ready arrows", x: 252, width: 94);
        for (int index = 0; index < _food.Length; index++)
        {
            _food[index] = AddTile(parent: row, label: "Slot " + Hud.Number(value: index + 1),
                    x: 354 + index * 76, width: 68);
        }

        _openButton = new Button { Name = "OpenInventory", Text = "Open inventory",
            TooltipText = "Open inventory (I)" };
        Place(control: _openButton, at: new Rect2(x: 660, y: 0, width: 170, height: 48));
        row.AddChild(node: _openButton);
        _openButton.Pressed += _open;
        _actionNotice = Text(parent: row, name: "ActionNotice",
                at: new Rect2(x: 842, y: 0, width: 414, height: 48));
    }

    private void BuildDamage()
    {
        _gateway.RequireThread();
        _damage = new Control { Name = "DamageTint", ZIndex = 58, MouseFilter = Control.MouseFilterEnum.Ignore,
            Visible = false };
        _root.AddChild(node: _damage);
        _damage.SetAnchorsAndOffsetsPreset(preset: Control.LayoutPreset.FullRect);
        var tint = new Color(r: 0.76f, g: 0.13f, b: 0.11f, a: 0.31f);
        Vector2 size = _root.GetViewportRect().Size;
        Fill(parent: _damage, name: "Top", at: new Rect2(x: 0, y: 0, width: size.X, height: 14), color: tint);
        Fill(parent: _damage, name: "Bottom",
                at: new Rect2(x: 0, y: size.Y - 14, width: size.X, height: 14), color: tint);
        Fill(parent: _damage, name: "Left", at: new Rect2(x: 0, y: 14, width: 14, height: size.Y - 28), color: tint);
        Fill(parent: _damage, name: "Right",
                at: new Rect2(x: size.X - 14, y: 14, width: 14, height: size.Y - 28), color: tint);
        Fill(parent: _damage, name: "HealthPulse", at: new Rect2(x: 22, y: 22, width: 268, height: 30),
                color: new Color(r: 1, g: 0.56f, b: 0.51f, a: 0.18f));
    }

    private void UpdateVitals(Hud state, string? note, float pulse)
    {
        _gateway.RequireThread();
        if (_panel is null)
        {
            return;
        }

        _panel.Visible = true;
        UpdateMeter(meter: _health, name: "Health", value: state.Health, maximum: state.MaximumHealth);
        UpdateMeter(meter: _stamina, name: "Stamina", value: state.Stamina, maximum: state.MaximumStamina);
        SetText(label: _rest, value: state.Rest);
        SetText(label: _money, value: "Money " + state.Money.ToString(provider: CultureInfo.InvariantCulture));
        SetText(label: _load, value: "Load " + state.WeightText + " kg · " + state.Load);
        SetText(label: _speed, value: "Move speed " + Hud.Number(value: state.Speed * 100)
                + "% · Stamina cost " + Hud.Number(value: state.Cost * 100) + "%");
        string movement = note ?? SprintNotice(state: state);
        SetText(label: _movementNotice, value: movement);
        Emphasize(label: _load, active: movement.StartsWith(value: "Too much", comparisonType:
                StringComparison.Ordinal));
        Emphasize(label: _stamina?.Label, active: movement.StartsWith(value: "Not enough",
                comparisonType: StringComparison.Ordinal));
        if (_damage is not null)
        {
            _damage.Visible = pulse > 0;
            _damage.Modulate = new Color(r: 1, g: 1, b: 1, a: pulse);
        }
    }

    private void UpdateActions(Hud state, string? note, bool browsing)
    {
        _gateway.RequireThread();
        DrawTile(tile: _left, item: state.Left, shade: state.LeftAttack ? state.AttackShade : 0);
        DrawTile(tile: _right, item: state.Right, shade: state.RightAttack ? state.AttackShade : 0);
        UpdateAmmo(count: state.Ammo);
        for (int index = 0; index < _food.Length; index++)
        {
            Hud.Item? item = state.Food[index];
            DrawTile(tile: _food[index], item: item, shade: item is null ? 0 : state.FoodShade);
        }

        SetText(label: _actionNotice, value: note ?? "");
        bool attackBlocked = note is not null && !note.StartsWith(value: "Food", comparisonType:
                StringComparison.Ordinal);
        Emphasize(label: _left?.Value, active: attackBlocked && state.LeftAttack);
        Emphasize(label: _right?.Value, active: attackBlocked && state.RightAttack);
        if (_openButton is not null)
        {
            if (browsing && _openButton.HasFocus())
            {
                _openButton.ReleaseFocus();
            }

            _openButton.Disabled = browsing;
        }
    }

    private string SprintNotice(Hud state)
    {
        if (!StringComparer.Ordinal.Equals(x: _previousSprint, y: state.SprintBlock))
        {
            _previousSprint = state.SprintBlock;
            _sprintUntil = state.RunTime + 2000;
        }

        return state.RunTime < _sprintUntil ? Hud.BlockedNotice(kind: "sprint", status: state.SprintBlock) ?? "" : "";
    }

    private static string? ActiveNotice((long epoch, long at, string kind, string text)? receipt, Hud state, long now)
    {
        return receipt is { } current && current.epoch == state.Generation && now - current.at < 2000
                ? current.text : null;
    }

    private Meter AddMeter(Control parent, string name, Rect2 at, Color fill)
    {
        _gateway.RequireThread();
        ColorRect backing = Fill(parent: parent, name: name, at: at,
                color: new Color(r: 0.094f, g: 0.145f, b: 0.169f));
        var inner = new Rect2(x: 2, y: 2, width: at.Size.X - 4, height: at.Size.Y - 4);
        ColorRect foreground = Fill(parent: backing, name: "Fill", at: inner,
                color: new Color(r: fill.R * 0.50f, g: fill.G * 0.50f, b: fill.B * 0.50f));
        Label label = Text(parent: backing, name: "Value", at: inner);
        label.HorizontalAlignment = HorizontalAlignment.Center;
        return new Meter(fill: foreground, label: label, width: inner.Size.X);
    }

    private Tile AddTile(Control parent, string label, int x, int width)
    {
        _gateway.RequireThread();
        ColorRect box = Fill(parent: parent, name: label.Replace(oldValue: " ", newValue: "", comparisonType:
                StringComparison.Ordinal), at: new Rect2(x: x, y: 0, width: width, height: 48),
                color: new Color(r: 0.094f, g: 0.145f, b: 0.169f));
        var icon = new TextureRect { Name = "Icon", MouseFilter = Control.MouseFilterEnum.Ignore,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspect,
            Position = new Vector2(x: 5, y: 21), Size = new Vector2(x: 27, y: 24) };
        box.AddChild(node: icon);
        ColorRect condition = Fill(parent: box, name: "Condition", at: new Rect2(x: 34, y: 43,
                width: width - 39, height: 3), color: new Color(r: 0.569f, g: 0.733f, b: 0.643f));
        ColorRect shade = Fill(parent: box, name: "Cooldown", at: new Rect2(x: 3, y: 19, width: width - 6, height: 26),
                color: new Color(r: 0.04f, g: 0.05f, b: 0.07f, a: 0.66f));
        Label title = Text(parent: box, name: "Title", at: new Rect2(x: 4, y: 2, width: width - 8, height: 15));
        title.AddThemeFontSizeOverride(name: "font_size", fontSize: 13);
        title.Text = label;
        Label value = Text(parent: box, name: "Value", at: new Rect2(x: 34, y: 19, width: width - 39, height: 23));
        value.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
        return new Tile(box: box, icon: icon, condition: condition, shade: shade, value: value, title: label,
                width: width);
    }

    private void DrawTile(Tile? tile, Hud.Item? item, decimal shade)
    {
        _gateway.RequireThread();
        if (tile is null)
        {
            return;
        }

        tile.Icon.Texture = item is null ? null : _art?.Icon(definition: item.Definition, icon: item.Icon);
        string name = item is null ? "" : _art?.Name(definition: item.Definition) ?? item.Definition;
        tile.Value.Text = item is null ? "Empty" : item.Condition == 0 ? "× Broken" : Hud.Number(value: item.Quantity);
        tile.Box.TooltipText = item is null ? tile.Title + " empty" : name + " · " + Hud.Number(value: item.Quantity)
                + (item.Condition is { } condition && item.Maximum is { } maximum ? "\nCondition "
                        + Hud.Number(value: condition) + " / " + Hud.Number(value: maximum) : "");
        tile.Condition.Visible = item?.Maximum is > 0;
        if (item?.Maximum is > 0 && item.Condition is { } health)
        {
            tile.Condition.Size = new Vector2(x: (tile.Width - 39) * (float)(health / item.Maximum.Value), y: 3);
        }

        tile.Shade.Visible = shade > 0;
        tile.Shade.Size = new Vector2(x: (tile.Width - 6) * (float)shade, y: 26);
    }

    private void UpdateAmmo(int? count)
    {
        _gateway.RequireThread();
        if (_ammo is not { } tile)
        {
            return;
        }

        tile.Icon.Texture = count.HasValue ? _art?.Icon(definition: "IT-02", icon: "IT_02_master") : null;
        tile.Value.Text = count?.ToString(provider: CultureInfo.InvariantCulture) ?? "";
        tile.Box.TooltipText = count.HasValue ? "Ready arrows " + tile.Value.Text : "Ready arrows inactive";
        tile.Condition.Visible = false;
        tile.Shade.Visible = false;
    }

    private void UpdateMeter(Meter? meter, string name, decimal value, decimal maximum)
    {
        _gateway.RequireThread();
        if (meter is null || maximum <= 0)
        {
            return;
        }

        meter.Fill.Size = new Vector2(x: meter.Width * (float)(value / maximum), y: meter.Fill.Size.Y);
        meter.Label.Text = name + " " + Hud.Number(value: value) + " / " + Hud.Number(value: maximum);
    }

    private void Emphasize(Label? label, bool active)
    {
        _gateway.RequireThread();
        if (label is not null)
        {
            label.Modulate = active ? new Color(r: 1, g: 0.70f, b: 0.62f) : Colors.White;
        }
    }

    private Label Text(Control parent, string name, Rect2 at)
    {
        _gateway.RequireThread();
        var label = new Label { Name = name, MouseFilter = Control.MouseFilterEnum.Ignore,
            VerticalAlignment = VerticalAlignment.Center };
        Place(control: label, at: at);
        parent.AddChild(node: label);
        return label;
    }

    private void SetText(Label? label, string value)
    {
        _gateway.RequireThread();
        if (label is not null)
        {
            label.Text = value;
        }
    }

    private ColorRect Fill(Control parent, string name, Rect2 at, Color color)
    {
        _gateway.RequireThread();
        var panel = new ColorRect { Name = name, Color = color, MouseFilter = Control.MouseFilterEnum.Ignore };
        Place(control: panel, at: at);
        parent.AddChild(node: panel);
        return panel;
    }

    private void Place(Control control, Rect2 at)
    {
        _gateway.RequireThread();
        control.Position = at.Position;
        control.Size = at.Size;
    }

    private sealed record Meter
    {
        internal Meter(ColorRect fill, Label label, float width)
        {
            Fill = fill;
            Label = label;
            Width = width;
        }

        internal ColorRect Fill { get; }
        internal Label Label { get; }
        internal float Width { get; }
    }

    private sealed record Tile
    {
        internal Tile(ColorRect box, TextureRect icon, ColorRect condition, ColorRect shade, Label value,
                string title, int width)
        {
            Box = box;
            Icon = icon;
            Condition = condition;
            Shade = shade;
            Value = value;
            Title = title;
            Width = width;
        }

        internal ColorRect Box { get; }
        internal TextureRect Icon { get; }
        internal ColorRect Condition { get; }
        internal ColorRect Shade { get; }
        internal Label Value { get; }
        internal string Title { get; }
        internal int Width { get; }
    }
}
