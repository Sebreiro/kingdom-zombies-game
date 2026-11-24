using System;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Routes native gameplay input after the focused controls and current workspace take their turn.</summary>
internal sealed class FoundationWorldInput : IDisposable
{
    public void Dispose()
    {
        DetachWorld();
    }

    internal FoundationWorldInput(Control root, MainThreadGateway gateway, FoundationInput input,
            Func<FoundationCoordinator?> coordinator, Func<FoundationInput.IWorkspace?> workspace,
            Func<bool> controlVisible)
    {
        _root = root;
        _gateway = gateway;
        _input = input;
        _coordinator = coordinator;
        _workspace = workspace;
        _controlVisible = controlVisible;
    }

    internal (decimal x, decimal y)? LastAttackAim { get; private set; }

    internal void AttachWorld(Node2D world)
    {
        _gateway.RequireThread();
        _world = world;
        _surface = new Control
        {
            Name = "WorldInput",
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        _root.AddChild(node: _surface);
        _root.MoveChild(childNode: _surface, toIndex: 2);
        _surface.SetAnchorsAndOffsetsPreset(preset: Control.LayoutPreset.FullRect);
        _surface.GuiInput += Click;
        _crosshair = new Node2D { Name = "Aim", Visible = false, ZIndex = 48 };
        _surface.AddChild(node: _crosshair);
        _crosshair.AddChild(node: Stroke(from: new Vector2(x: -3, y: 0), to: new Vector2(x: 4, y: 0)));
        _crosshair.AddChild(node: Stroke(from: new Vector2(x: 0, y: -3), to: new Vector2(x: 0, y: 4)));
        _input.Clear();
    }

    internal void DetachWorld()
    {
        _gateway.RequireThread();
        SetAimVisible(visible: false);
        if (_surface is not null)
        {
            _surface.GuiInput -= Click;
            _root.RemoveChild(node: _surface);
            _surface.QueueFree();
        }

        _world = null;
        _surface = null;
        _crosshair = null;
        _readback = null;
        _frame = null;
        _identity = null;
        _lastMotion = null;
        LastAttackAim = null;
        _input.SetContext(gameplay: false);
    }

    internal void GainFocus()
    {
        _gateway.RequireThread();
        _focused = true;
        if (!Input.IsMouseButtonPressed(button: MouseButton.Left))
        {
            _input.Mouse(pressed: false);
        }
    }

    internal void LoseFocus()
    {
        _gateway.RequireThread();
        _focused = false;
        _input.Clear();
        _input.SetContext(gameplay: false);
        SetAimVisible(visible: false);
        Move();
    }

    internal void Refresh(bool popup)
    {
        _gateway.RequireThread();
        FoundationCoordinator? coordinator = _coordinator();
        (string id, long epoch)? identity = _controlVisible() && coordinator is { Ready: true, WaitingForHost: false }
                && coordinator.LocalRunControl is { generation: > 0 } control
                ? ((string id, long epoch)?)(control.survivorId, control.generation) : null;
        if (_identity != identity)
        {
            _identity = identity;
            _lastMotion = null;
            LastAttackAim = null;
            _input.Clear();
        }

        bool focus = _root.GetViewport().GuiGetFocusOwner() is { } owner && owner.IsVisibleInTree();
        bool gameplay = _world is not null && FoundationInput.WorldAllowed(alive: identity.HasValue,
                focused: _focused, popup: popup, panel: _workspace()?.CapturesWorld == true, guiFocused: focus);
        _input.SetContext(gameplay: gameplay);
        if (_surface is not null)
        {
            _surface.MouseFilter = gameplay ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;
        }
        if (!gameplay)
        {
            SetAimVisible(visible: false);
        }
    }

    internal void Move()
    {
        _gateway.RequireThread();
        if (_identity is null)
        {
            return;
        }

        (int direction, bool sprint) motion = (_input.Direction, _input.Held(key: (int)Key.Shift));
        bool jump = _input.TakeJump();
        if (jump || _lastMotion != motion)
        {
            _ = _coordinator()?.SubmitRunMotion(direction: motion.direction, sprint: motion.sprint, jump: jump);
            _lastMotion = motion;
        }
    }

    internal bool HandleKey(InputEventKey key)
    {
        _gateway.RequireThread();
        if (_world is null || !_focused)
        {
            return false;
        }

        if (key.Keycode is Key.Escape || key.PhysicalKeycode is Key.E or Key.I)
        {
            return HandleWorkspace(key: key);
        }
        if (!_input.Gameplay || key.PhysicalKeycode is not
                (Key.A or Key.D or Key.Left or Key.Right or Key.Shift or Key.Space or Key.Key1 or Key.Key2
                        or Key.Key3 or Key.Key4))
        {
            return false;
        }

        if (key.Pressed && !key.Echo && _input.FreshPress)
        {
            if (key.PhysicalKeycode is Key.Space)
            {
                _input.QueueJump();
            }
            else if (key.PhysicalKeycode is >= Key.Key1 and <= Key.Key4)
            {
                UseFood(slot: 1 + (int)(key.PhysicalKeycode - Key.Key1));
            }
        }

        return true;
    }

    internal (double x, double y)? Point(FoundationRunReadback? readback,
            FoundationEquipmentProjection.Frame? frame)
    {
        _gateway.RequireThread();
        _readback = readback;
        _frame = frame;
        FoundationEquipmentProjection.Subject? actor = frame?.Actors.FirstOrDefault(predicate: subject =>
                StringComparer.Ordinal.Equals(x: subject.Id, y: _identity?.id));
        (decimal x, decimal y)? aim = _input.Gameplay && actor is not null
                && FoundationEquipmentProjection.HasBow(subject: actor) ? Aim() : null;
        SetAimVisible(visible: aim.HasValue);
        if (aim.HasValue && _crosshair is not null && _surface is not null)
        {
            Vector2 point = _surface.GetLocalMousePosition();
            _crosshair.Position = new Vector2(x: MathF.Round(x: point.X), y: MathF.Round(x: point.Y));
        }

        return aim is { } current ? ((double)current.x, (double)current.y) : null;
    }

    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly FoundationInput _input;
    private readonly Func<FoundationCoordinator?> _coordinator;
    private readonly Func<FoundationInput.IWorkspace?> _workspace;
    private readonly Func<bool> _controlVisible;
    private Control? _surface;
    private Node2D? _world;
    private Node2D? _crosshair;
    private FoundationRunReadback? _readback;
    private FoundationEquipmentProjection.Frame? _frame;
    private (string id, long epoch)? _identity;
    private (int direction, bool sprint)? _lastMotion;
    private Input.MouseModeEnum? _previousMouseMode;
    private bool _focused = true;

    private bool HandleWorkspace(InputEventKey key)
    {
        _gateway.RequireThread();
        if (_root.GetViewport().GuiGetFocusOwner() is TextEdit
                || _root.GetViewport().GuiGetFocusOwner() is LineEdit edit && edit.IsEditing())
        {
            return false;
        }

        FoundationInput.IWorkspace? workspace = _workspace();
        FoundationInput.WorkspaceCommand command = key.Keycode is Key.Escape
                ? FoundationInput.WorkspaceCommand.Escape : key.PhysicalKeycode is Key.E
                        ? FoundationInput.WorkspaceCommand.Interact : FoundationInput.WorkspaceCommand.Inventory;
        if ((_identity.HasValue || workspace?.CapturesWorld == true)
                && key.Pressed && !key.Echo && _input.FreshPress)
        {
            if (workspace?.Handle(command: command) == true)
            {
                Refresh(popup: false);
                Move();
                return true;
            }
            if (_input.Gameplay && command is FoundationInput.WorkspaceCommand.Interact)
            {
                SelectNearby();
            }
        }

        return workspace?.CapturesWorld == true || _input.Gameplay
                && command is not FoundationInput.WorkspaceCommand.Escape;
    }

    private void Click(InputEvent inputEvent)
    {
        _gateway.RequireThread();
        if (inputEvent is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true } mouse
                && _input.TakeClick(world: _workspace()?.CapturesWorld != true)
                && Aim(viewport: mouse.GlobalPosition) is { } point)
        {
            _ = _coordinator()?.SubmitRunAction(kind: "attack", request:
                    JsonSerializer.SerializeToElement(value: new { AimX = point.x, AimY = point.y }));
            LastAttackAim = point;
        }
    }

    private (decimal x, decimal y)? Aim(Vector2? viewport = null)
    {
        _gateway.RequireThread();
        if (_world is null || _frame is null)
        {
            return null;
        }

        Vector2 point = _world.GetGlobalTransformWithCanvas().AffineInverse()
                * (viewport ?? _root.GetViewport().GetMousePosition());
        return FoundationInput.Aim(localX: point.X, localY: point.Y, origin: _frame.Origin, ground: _frame.Ground);
    }

    private FoundationRunView? Own()
    {
        return _readback?.Views.TryGetValue(key: "own", value: out FoundationRunView? view) == true
                && view.Epoch == _identity?.epoch ? view : null;
    }

    private void SelectNearby()
    {
        if (_readback?.Views.TryGetValue(key: "nearby", value: out FoundationRunView? view) != true
                || view is null || view.Epoch != _identity?.epoch || view.Rows.Length != 1
                || view.Header.GetProperty(propertyName: "Count").GetInt32() != 1)
        {
            return;
        }

        JsonElement target = view.Rows[0];
        _ = _coordinator()?.SubmitRunAction(kind: "select", request: JsonSerializer.SerializeToElement(value: new
        {
            TargetKind = target.GetProperty(propertyName: "Kind").GetString(),
            TargetId = target.GetProperty(propertyName: "Id").GetString(),
        }));
    }

    private void UseFood(int slot)
    {
        if (Own() is not { } own)
        {
            return;
        }

        foreach (JsonElement row in own.Header.GetProperty(propertyName: "Quick").EnumerateArray())
        {
            if (row.GetProperty(propertyName: "Slot").GetInt32() != slot
                    || row.GetProperty(propertyName: "Stamp").GetString() is not { } stamp)
            {
                continue;
            }

            _ = _coordinator()?.SubmitRunAction(kind: "food", request: JsonSerializer.SerializeToElement(value: new
            {
                Item = new { Id = row.GetProperty(propertyName: "ItemId").GetInt64(), Stamp = stamp },
                QuickSlot = slot,
            }));
            return;
        }

        _coordinator()?.MissingQuickFood();
    }

    private void SetAimVisible(bool visible)
    {
        _gateway.RequireThread();
        if (_crosshair is null || _crosshair.Visible == visible)
        {
            return;
        }

        _crosshair.Visible = visible;
        if (visible)
        {
            _previousMouseMode = Input.MouseMode;
            Input.MouseMode = Input.MouseModeEnum.Hidden;
        }
        else if (_previousMouseMode is { } previous)
        {
            Input.MouseMode = previous;
            _previousMouseMode = null;
        }
    }

    private Line2D Stroke(Vector2 from, Vector2 to)
    {
        _gateway.RequireThread();
        return new Line2D { Points = [from, to], Width = 1, DefaultColor = new Color(r: 0.95f, g: 0.93f, b: 0.85f) };
    }
}
