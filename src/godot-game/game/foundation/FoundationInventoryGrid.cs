using System;
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

    internal Control Add(Control parent, FoundationInventoryInteraction.Grid grid,
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

        return board;
    }

    internal void Decorate(Button button, FoundationInventoryInteraction.Item item, bool equipment = false)
    {
        _gateway.RequireThread();
        button.TooltipText = Details(item: item);
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

    private readonly MainThreadGateway _gateway;
    private readonly FoundationInventoryArt _art;

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
