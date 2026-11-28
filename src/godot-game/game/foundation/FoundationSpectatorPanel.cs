using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Renders deliberate spectator actions without providing gameplay or free-camera input.</summary>
internal sealed class FoundationSpectatorPanel : IDisposable
{
    public void Dispose()
    {
        _gateway.RequireThread();
        Reset();
        _card?.QueueFree();
        _transfer?.QueueFree();
    }

    internal FoundationSpectatorPanel(Control root, MainThreadGateway gateway, Func<FoundationCoordinator?> authority)
    {
        _root = root;
        _gateway = gateway;
        _authority = authority;
    }

    internal FoundationSpectator State { get; } = new();

    internal void Observe(Node2D world, FoundationRunReadback? readback, FoundationCoordinator coordinator, long now)
    {
        _gateway.RequireThread();
        double span = _root.GetViewportRect().Size.X;
        State.Observe(readback: coordinator.WaitingForHost ? null : readback, binding: coordinator.BindingId,
                now: now, center: span / 2 - world.Position.X, span: span);
        if (State.Visible || State.TransferNotice.Length > 0)
        {
            Build();
        }

        Draw();
    }

    internal void Remember(Node2D world)
    {
        _gateway.RequireThread();
        double span = _root.GetViewportRect().Size.X;
        State.RememberVisible(center: span / 2 - world.Position.X, span: span);
    }

    internal void Reset()
    {
        _gateway.RequireThread();
        State.Clear();
        Draw();
    }

    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly Func<FoundationCoordinator?> _authority;
    private Panel? _card;
    private Button? _respawn;
    private Button? _previous;
    private Button? _next;
    private Label? _status;
    private Label? _following;
    private VBoxContainer? _choices;
    private Label? _transfer;
    private ImmutableArray<FoundationSpectator.Teammate> _displayed = [];

    private void Build()
    {
        _gateway.RequireThread();
        if (_card is not null)
        {
            return;
        }

        _card = new Panel { Name = "SpectatorCard", ZIndex = 24, FocusMode = Control.FocusModeEnum.All,
            MouseFilter = Control.MouseFilterEnum.Stop, Visible = false };
        _card.AddThemeStyleboxOverride(name: "panel", stylebox: Backplate());
        Place(control: _card, at: new Rect2(x: 24, y: _root.GetViewportRect().Size.Y - 328,
                width: 400, height: 304));
        _root.AddChild(node: _card);
        Label heading = Text(parent: _card, name: "Heading", at: new Rect2(x: 14, y: 8, width: 370, height: 40));
        heading.Text = "Spectating";
        heading.AddThemeFontSizeOverride(name: "font_size", fontSize: 32);
        _respawn = AddButton(parent: _card, name: "Respawn", text: "Respawn", action: Request,
                at: new Rect2(x: 14, y: 54, width: 176, height: 46));
        _status = Text(parent: _card, name: "RespawnStatus", at: new Rect2(x: 204, y: 54, width: 180, height: 46));
        _previous = AddButton(parent: _card, name: "PreviousTeammate", text: "Previous teammate",
                action: () => Cycle(previous: true), at: new Rect2(x: 14, y: 108, width: 176, height: 36));
        _next = AddButton(parent: _card, name: "NextTeammate", text: "Next teammate",
                action: () => Cycle(previous: false), at: new Rect2(x: 204, y: 108, width: 180, height: 36));
        _following = Text(parent: _card, name: "FollowStatus",
                at: new Rect2(x: 14, y: 147, width: 370, height: 58));
        _choices = new VBoxContainer { Name = "Teammates", Position = new Vector2(x: 14, y: 205),
            Size = new Vector2(x: 370, y: 92), MouseFilter = Control.MouseFilterEnum.Pass };
        _card.AddChild(node: _choices);
        _transfer = Text(parent: _root, name: "ControlTransferred",
                at: new Rect2(x: 424, y: 112, width: 432, height: 42));
        _transfer.ZIndex = 53;
        _transfer.HorizontalAlignment = HorizontalAlignment.Center;
        _transfer.AddThemeStyleboxOverride(name: "normal", stylebox: Backplate());
    }

    private void Draw()
    {
        _gateway.RequireThread();
        if (_card is null || _respawn is null || _previous is null || _next is null || _status is null
                || _following is null || _transfer is null)
        {
            return;
        }

        bool entering = State.Visible && !_card.Visible;
        if (!State.Visible && _root.GetViewport().GuiGetFocusOwner() is { } focus
                && (_card == focus || _card.IsAncestorOf(node: focus)))
        {
            focus.ReleaseFocus();
        }

        _card.Visible = State.Visible;
        _respawn.Disabled = !State.CanRespawn;
        _respawn.Modulate = State.CanRespawn ? new Color(r: 0.941f, g: 0.769f, b: 0.416f) : Colors.White;
        _status.Text = State.Status;
        _previous.Disabled = State.Teammates.IsEmpty;
        _next.Disabled = State.Teammates.IsEmpty;
        _following.Text = FollowingText();
        _transfer.Text = State.TransferNotice;
        _transfer.Visible = State.TransferNotice.Length > 0;
        DrawChoices();
        if (entering)
        {
            Control first = State.CanRespawn ? _respawn : State.Teammates.IsEmpty ? _card : _next;
            first.GrabFocus();
        }
    }

    private string FollowingText()
    {
        const string Empty = "No living teammates to follow";
        if (State.FollowNotice.Length > 0)
        {
            return State.Teammates.IsEmpty ? State.FollowNotice + "\n" + Empty : State.FollowNotice;
        }

        return State.Teammates.IsEmpty ? Empty : State.Followed is { } followed
                ? "Following " + NameOf(player: followed) : "Choose a living teammate";
    }

    private void DrawChoices()
    {
        _gateway.RequireThread();
        if (_choices is null)
        {
            return;
        }

        if (!_displayed.SequenceEqual(second: State.Teammates))
        {
            foreach (Node obsolete in _choices.GetChildren())
            {
                _choices.RemoveChild(node: obsolete);
                obsolete.QueueFree();
            }

            _displayed = State.Teammates;
            foreach (FoundationSpectator.Teammate player in _displayed)
            {
                var row = new Button { Name = "Teammate_" + player.Slot.ToString(
                        provider: CultureInfo.InvariantCulture), Text = NameOf(player: player),
                    TooltipText = NameOf(player: player), CustomMinimumSize = new Vector2(x: 0, y: 30),
                    TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis };
                row.Pressed += () => Follow(player: player);
                _choices.AddChild(node: row);
            }
        }

        for (int index = 0; index < _displayed.Length; index++)
        {
            if (_choices.GetChild(idx: index) is Button row)
            {
                row.Modulate = State.Followed?.Same(other: _displayed[index]) == true
                        ? new Color(r: 0.941f, g: 0.769f, b: 0.416f) : Colors.White;
            }
        }
    }

    private void Request()
    {
        _gateway.RequireThread();
        if (State.CanRespawn && _authority()?.SubmitRunAction(kind: "respawn",
                request: JsonSerializer.SerializeToElement(value: new object())) is { } result)
        {
            _ = State.BeginRequest(epoch: State.DeathEpoch, sequence: result.sequence);
            Draw();
        }
    }

    private void Cycle(bool previous)
    {
        _ = State.Cycle(previous: previous);
        Draw();
    }

    private void Follow(FoundationSpectator.Teammate player)
    {
        _ = State.Follow(choice: player);
        Draw();
    }

    private Button AddButton(Control parent, string name, string text, Action action, Rect2 at)
    {
        _gateway.RequireThread();
        var button = new Button { Name = name, Text = text, TooltipText = text };
        Place(control: button, at: at);
        parent.AddChild(node: button);
        button.Pressed += action;
        return button;
    }

    private Label Text(Control parent, string name, Rect2 at)
    {
        _gateway.RequireThread();
        var label = new Label { Name = name, MouseFilter = Control.MouseFilterEnum.Ignore,
            AutowrapMode = TextServer.AutowrapMode.WordSmart, VerticalAlignment = VerticalAlignment.Center };
        Place(control: label, at: at);
        parent.AddChild(node: label);
        return label;
    }

    private StyleBoxFlat Backplate()
    {
        _gateway.RequireThread();
        return new StyleBoxFlat
        {
            BgColor = new Color(r: 0.094f, g: 0.145f, b: 0.169f),
            BorderColor = new Color(r: 0.49f, g: 0.37f, b: 0.25f),
            BorderWidthLeft = 4, BorderWidthTop = 4, BorderWidthRight = 4, BorderWidthBottom = 4,
        };
    }

    private void Place(Control control, Rect2 at)
    {
        _gateway.RequireThread();
        control.Position = at.Position;
        control.Size = at.Size;
    }

    private static string NameOf(FoundationSpectator.Teammate player)
    {
        return "P" + player.Slot.ToString(provider: CultureInfo.InvariantCulture) + " · " + player.Label;
    }
}
