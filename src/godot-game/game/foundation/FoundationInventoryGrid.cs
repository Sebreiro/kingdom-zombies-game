using System;
using System.Collections.Immutable;
using System.Globalization;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Renders the same cell and item controls in carried, stash and nested windows.</summary>
internal sealed class FoundationInventoryGrid
{
    internal const int Pitch = 32;

    internal FoundationInventoryGrid(MainThreadGateway gateway, FoundationInventoryArt art)
    {
        _gateway = gateway;
        _art = art;
    }

    internal bool FocusDetailsVisible => _focusedDetails is not null;

    internal Control Add(Control parent, FoundationInventoryInteraction.Grid grid,
            ImmutableArray<(int x, int y)> highlights,
            Action<Button, FoundationInventoryInteraction.Item, InputEvent> itemInput)
    {
        _gateway.RequireThread();
        var board = new Control
        {
            Name = "ItemCells",
            CustomMinimumSize = new Vector2(x: grid.Width * Pitch, y: grid.Height * Pitch),
            MouseFilter = Control.MouseFilterEnum.Pass,
        };
        parent.AddChild(node: board);
        board.Draw += () => DrawCells(board: board, width: grid.Width, height: grid.Height);
        foreach (FoundationInventoryInteraction.Item item in grid.Items)
        {
            FoundationInventoryInteraction.Rect at = item.Location.Area;
            var button = new Button
            {
                Name = "Item_" + item.Id.ToString(provider: CultureInfo.InvariantCulture),
                Position = new Vector2(x: at.X * Pitch, y: at.Y * Pitch),
                Size = new Vector2(x: at.Width * Pitch, y: at.Height * Pitch),
            };
            board.AddChild(node: button);
            Decorate(button: button, item: item);
            button.GuiInput += value => itemInput(arg1: button, arg2: item, arg3: value);
        }

        if (!highlights.IsDefaultOrEmpty)
        {
            AddHighlights(board: board, cells: highlights);
        }

        return board;
    }

    internal void Decorate(Button button, FoundationInventoryInteraction.Item item, bool equipment = false)
    {
        _gateway.RequireThread();
        string details = Details(item: item);
        button.TooltipText = details;
        button.FocusEntered += () => ShowFocusDetails(button: button, details: details);
        button.FocusExited += () => ClearFocusDetails(button: button);
        button.MouseEntered += () => ClearFocusDetails(button: button);
        var image = new TextureRect
        {
            Name = "Icon",
            Texture = _art.Icon(item: item),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Position = new Vector2(x: 4, y: 2),
            Size = new Vector2(x: Math.Max(val1: 20, val2: button.Size.X - 8),
                    y: Math.Max(val1: 22, val2: equipment ? 24 : button.Size.Y - 6)),
        };
        button.AddChild(node: image);
        if (item.Location.Rotated)
        {
            image.PivotOffset = image.Size / 2;
            image.RotationDegrees = 90;
        }

        if (item.MaxStack > 1)
        {
            AddCount(button: button, quantity: item.Quantity);
        }
        if (equipment && item.MaximumCondition is > 0 && item.Condition is { } condition)
        {
            AddCondition(button: button, condition: condition, maximum: item.MaximumCondition.Value);
        }
    }

    internal string Details(FoundationInventoryInteraction.Item item)
    {
        string result = _art.Name(item: item) + "\nWeight " + Exact(value: item.Weight * item.Quantity) + " kg";
        if (item.MaxStack > 1)
        {
            result += "\nQuantity " + item.Quantity.ToString(provider: CultureInfo.InvariantCulture)
                    + " / " + item.MaxStack.ToString(provider: CultureInfo.InvariantCulture);
        }
        if (item.Condition is { } condition && item.MaximumCondition is { } maximum)
        {
            result += "\nCondition " + Exact(value: condition) + " / " + Exact(value: maximum)
                    + (condition == 0 ? "\nBroken" : "");
        }
        if (item.StorageWidth is { } width && item.StorageHeight is { } height)
        {
            result += "\nStorage " + width.ToString(provider: CultureInfo.InvariantCulture) + " × "
                    + height.ToString(provider: CultureInfo.InvariantCulture);
        }

        return result;
    }

    internal void HideFocusDetails()
    {
        _gateway.RequireThread();
        if (_focusedDetails is not null)
        {
            _focusedDetails.GetParent()?.RemoveChild(node: _focusedDetails);
            _focusedDetails.QueueFree();
        }

        _focusedDetails = null;
        _focusedItem = null;
    }

    private readonly MainThreadGateway _gateway;
    private readonly FoundationInventoryArt _art;
    private Panel? _focusedDetails;
    private Button? _focusedItem;

    private void ClearFocusDetails(Button button)
    {
        if (ReferenceEquals(objA: _focusedItem, objB: button))
        {
            HideFocusDetails();
        }
    }

    private void ShowFocusDetails(Button button, string details)
    {
        _gateway.RequireThread();
        HideFocusDetails();
        Rect2 item = button.GetGlobalRect();
        if (item.HasPoint(point: button.GetGlobalMousePosition())
                || button.FindParent(pattern: "InventoryWorkspace") is not Control layer)
        {
            return;
        }

        Vector2 viewport = layer.GetViewportRect().Size;
        const int Width = 300;
        int height = Math.Clamp(value: 22 + details.Length / 30 * 17
                + (details.Split(separator: '\n').Length - 1) * 17, min: 80, max: 300);
        float left = item.End.X + Width + 26 <= viewport.X ? item.End.X + 10 : item.Position.X - Width - 10;
        var panel = new Panel
        {
            Name = "FocusedItemDetails",
            Position = new Vector2(x: Math.Clamp(value: left, min: 24, max: viewport.X - Width - 24),
                    y: Math.Clamp(value: item.Position.Y, min: 24, max: viewport.Y - height - 24)),
            Size = new Vector2(x: Width, y: height),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 24,
        };
        panel.AddThemeStyleboxOverride(name: "panel", stylebox: new StyleBoxFlat
        {
            BgColor = new Color(r: 0.094f, g: 0.145f, b: 0.169f),
            BorderColor = new Color(r: 0.957f, g: 0.949f, b: 0.91f),
            BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1,
        });
        var label = new Label
        {
            Text = details,
            Position = new Vector2(x: 12, y: 8),
            Size = new Vector2(x: Width - 24, y: height - 16),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
        };
        layer.AddChild(node: panel);
        panel.AddChild(node: label);
        _focusedItem = button;
        _focusedDetails = panel;
    }

    private void AddHighlights(Control board, ImmutableArray<(int x, int y)> cells)
    {
        _gateway.RequireThread();
        var overlay = new Control { Name = "ChangedCells", MouseFilter = Control.MouseFilterEnum.Ignore };
        board.AddChild(node: overlay);
        overlay.Draw += () => DrawHighlights(overlay: overlay, cells: cells);
    }

    private void DrawHighlights(Control overlay, ImmutableArray<(int x, int y)> cells)
    {
        _gateway.RequireThread();
        foreach (var (x, y) in cells)
        {
            var frame = new Rect2(x: x * Pitch + 2, y: y * Pitch + 2, width: Pitch - 4, height: Pitch - 4);
            overlay.DrawRect(rect: frame, color: new Color(r: 0.941f, g: 0.769f, b: 0.416f, a: 0.28f));
            overlay.DrawRect(rect: frame, color: new Color(r: 0.941f, g: 0.769f, b: 0.416f), filled: false,
                    width: 2);
        }
    }

    private void DrawCells(Control board, int width, int height)
    {
        _gateway.RequireThread();
        var fill = new Color(r: 0.208f, g: 0.263f, b: 0.247f);
        var seam = new Color(r: 0.435f, g: 0.498f, b: 0.475f);
        board.DrawRect(rect: new Rect2(x: 0, y: 0, width: width * Pitch, height: height * Pitch), color: fill);
        for (int x = 0; x <= width; x++)
        {
            board.DrawLine(from: new Vector2(x: x * Pitch, y: 0),
                    to: new Vector2(x: x * Pitch, y: height * Pitch), color: seam);
        }

        for (int y = 0; y <= height; y++)
        {
            board.DrawLine(from: new Vector2(x: 0, y: y * Pitch),
                    to: new Vector2(x: width * Pitch, y: y * Pitch), color: seam);
        }
    }

    private void AddCount(Button button, int quantity)
    {
        _gateway.RequireThread();
        var badge = new Label
        {
            Text = quantity.ToString(provider: CultureInfo.InvariantCulture),
            Position = new Vector2(x: 2, y: button.Size.Y - 23),
            Size = new Vector2(x: button.Size.X - 4, y: 20),
            HorizontalAlignment = HorizontalAlignment.Right,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        badge.AddThemeColorOverride(name: "font_shadow_color", color: Colors.Black);
        badge.AddThemeConstantOverride(name: "shadow_offset_x", constant: 2);
        badge.AddThemeConstantOverride(name: "shadow_offset_y", constant: 2);
        button.AddChild(node: badge);
    }

    private void AddCondition(Button button, decimal condition, decimal maximum)
    {
        _gateway.RequireThread();
        var strip = new ColorRect
        {
            Position = new Vector2(x: 8, y: button.Size.Y - 8),
            Size = new Vector2(x: (button.Size.X - 16) * (float)(condition / maximum), y: 4),
            Color = new Color(r: 0.569f, g: 0.733f, b: 0.639f),
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        button.AddChild(node: strip);
        if (condition == 0)
        {
            var broken = new Label { Text = "× Broken", Position = new Vector2(x: 4, y: 0),
                MouseFilter = Control.MouseFilterEnum.Ignore };
            button.AddChild(node: broken);
        }
    }

    private static string Exact(decimal value)
    {
        return value.ToString(format: "0.############################", provider: CultureInfo.InvariantCulture);
    }
}
