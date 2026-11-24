using System;
using System.Linq;
using Godot;
using Model = KingdomZombies.Foundation.FoundationInventoryInteraction;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationInventoryWorkspace
{
    private Model.Rect Bounds()
    {
        _gateway.RequireThread();
        Vector2 size = _root.GetViewportRect().Size;
        return new Model.Rect(x: 24, y: 112, width: Math.Max(val1: 320, val2: (int)size.X - 48),
                height: Math.Max(val1: 240, val2: (int)size.Y - 192));
    }

    private Panel Surface(Control parent, string name, Model.Rect frame, float opacity)
    {
        _gateway.RequireThread();
        var panel = new Panel { Name = name, MouseFilter = Control.MouseFilterEnum.Stop };
        Put(control: panel, frame: frame);
        panel.AddThemeStyleboxOverride(name: "panel", stylebox: new StyleBoxFlat
        {
            BgColor = new Color(r: 0.094f, g: 0.145f, b: 0.169f, a: opacity),
            BorderColor = new Color(r: 0.49f, g: 0.37f, b: 0.25f),
            BorderWidthTop = 4,
            BorderWidthBottom = 4,
            BorderWidthLeft = 4,
            BorderWidthRight = 4,
        });
        parent.AddChild(node: panel);
        return panel;
    }

    private void Put(Control control, Model.Rect frame)
    {
        _gateway.RequireThread();
        control.Position = new Vector2(x: frame.X, y: frame.Y);
        control.Size = new Vector2(x: frame.Width, y: frame.Height);
    }

    private VBoxContainer ScrollFlow(Control parent, string name, Model.Rect at)
    {
        _gateway.RequireThread();
        var scroll = new ScrollContainer { Name = name };
        Put(control: scroll, frame: at);
        parent.AddChild(node: scroll);
        var flow = new VBoxContainer { Name = "Rows", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        scroll.AddChild(node: flow);
        return flow;
    }

    private Button ButtonAt(Control parent, string name, string text, Model.Rect at, Action action)
    {
        _gateway.RequireThread();
        var button = new Button { Name = name, Text = text, TooltipText = text };
        Put(control: button, frame: at);
        parent.AddChild(node: button);
        button.Pressed += action;
        return button;
    }

    private Label LabelAt(Control parent, string text, Model.Rect at, bool heading = false)
    {
        _gateway.RequireThread();
        Label label = TextLine(parent: parent, text: text);
        Put(control: label, frame: at);
        if (heading)
        {
            label.AddThemeFontSizeOverride(name: "font_size", fontSize: 24);
            label.AddThemeFontOverride(name: "font", font: new FontVariation
            {
                BaseFont = label.GetThemeFont(name: "font"),
                VariationEmbolden = 0.5f,
            });
        }

        return label;
    }

    private Label TextLine(Control parent, string text)
    {
        _gateway.RequireThread();
        var label = new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart,
            MouseFilter = Control.MouseFilterEnum.Ignore };
        parent.AddChild(node: label);
        return label;
    }

    private Button Option(VBoxContainer parent, string text, Action action)
    {
        _gateway.RequireThread();
        var button = new Button { Text = text, TooltipText = text,
            CustomMinimumSize = new Vector2(x: 0, y: 36) };
        parent.AddChild(node: button);
        button.Pressed += action;
        return button;
    }

    private VBoxContainer Popup(string title, Rect2 source)
    {
        _gateway.RequireThread();
        Control parent = _layer ?? throw new InvalidOperationException(message: "INVENTORY_LAYER");
        Model.Rect bounds = Bounds();
        int left = Math.Clamp(value: (int)source.End.X + 6, min: bounds.X,
                max: Math.Max(val1: bounds.X, val2: bounds.X + bounds.Width - 278));
        int top = Math.Clamp(value: (int)source.Position.Y, min: bounds.Y,
                max: Math.Max(val1: bounds.Y, val2: bounds.Y + bounds.Height - 290));
        _popup = Surface(parent: parent, name: "ItemMenu", frame: new Model.Rect(x: left, y: top,
                width: 272, height: 280), opacity: 1);
        _popup.ZIndex = 20;
        var rows = new VBoxContainer { Name = "Options", Position = new Vector2(x: 12, y: 10),
            Size = new Vector2(x: 248, y: 260) };
        _popup.AddChild(node: rows);
        TextLine(parent: rows, text: title);
        return rows;
    }

    private void FocusFirst(VBoxContainer options)
    {
        _gateway.RequireThread();
        options.GetChildren().OfType<Button>().FirstOrDefault()?.GrabFocus();
    }

    private void Retire(Control? node)
    {
        _gateway.RequireThread();
        if (node is not null && GodotObject.IsInstanceValid(instance: node))
        {
            node.GetParent()?.RemoveChild(node: node);
            node.QueueFree();
        }
    }

    private Model.Rect From(Rect2 rect)
    {
        _gateway.RequireThread();
        return new Model.Rect(x: (int)rect.Position.X, y: (int)rect.Position.Y,
                width: (int)rect.Size.X, height: (int)rect.Size.Y);
    }

    private static Model.Rect Local(Model.Rect frame, Model.Rect origin)
    {
        return frame with { X = frame.X - origin.X, Y = frame.Y - origin.Y };
    }

    private void ShowNotice(string text)
    {
        _gateway.RequireThread();
        RemoveNotice();
        _notice = LabelAt(parent: _root, text: text, at: new Model.Rect(x: 24, y: 76, width: 580, height: 36));
        _notice.ZIndex = 52;
        _noticeUntil = Time.GetTicksMsec() + 2000;
    }

    private void UpdateNotice()
    {
        _gateway.RequireThread();
        if (_notice is not null && Time.GetTicksMsec() >= _noticeUntil)
        {
            RemoveNotice();
        }
    }

    private void RemoveNotice()
    {
        Retire(node: _notice);
        _notice = null;
    }
}
