using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Composes the ordinary native menu, identified local session and read-only smoke observation.</summary>
public sealed partial class FoundationRoot : Control
{
    /// <summary>Initializes native callbacks on the captured startup thread.</summary>
    public FoundationRoot()
    {
        _gateway.RequireThread();
    }

    /// <summary>Connects guarded callbacks and starts in the ordinary menu.</summary>
    public override void _Ready()
    {
        _gateway.RequireThread();
        Label title = GetNode<Label>(path: "Menu/Title");
        var headingFont = new FontVariation
        {
            BaseFont = title.GetThemeFont(name: "font"), VariationEmbolden = 0.8f,
        };
        title.AddThemeFontOverride(name: "font", font: headingFont);
        GetNode<Button>(path: "Menu/Start").Pressed += Start;
        GetNode<Button>(path: "Menu/Quit").Pressed += Quit;
        GetNode<Button>(path: "Menu/Host").Pressed += Host;
        GetNode<Button>(path: "Menu/Join").Pressed += Browse;
        GetNode<Button>(path: "Browser/JoinAddress").Pressed += JoinAddress;
        GetNode<Button>(path: "Browser/Back").Pressed += ReturnToMenu;
        GetWindow().FocusExited += LoseFocus;
        ReturnToMenu();
    }

    /// <summary>Consumes native key events through the same route used by player controls.</summary>
    public override void _Input(InputEvent @event)
    {
        _gateway.RequireThread();
        if (@event is not InputEventKey key)
        {
            return;
        }

        _inputEvents++;
        if (!key.Echo)
        {
            _input.Set(key: (int)key.PhysicalKeycode, pressed: key.Pressed);
        }
        if (_coordinator is not null && key.Keycode == Key.Escape && _input.FreshPress && !key.Echo)
        {
            ReturnToMenu();
            GetViewport().SetInputAsHandled();
            return;
        }

        if (_coordinator is null && key.Keycode is Key.Escape && key.Pressed && !key.Echo)
        {
            ReturnToMenu();
            GetViewport().SetInputAsHandled();
        }
        else if (_coordinator is null && key.Keycode is Key.Enter or Key.Space or Key.KpEnter
                && GetViewport().GuiGetFocusOwner() is Button)
        {
            if (!key.Echo && _input.FreshPress)
            {
                ActivateFocused();
            }

            GetViewport().SetInputAsHandled();
        }
    }

    /// <summary>Converts elapsed time with a carried remainder and submits one identified command.</summary>
    public override void _Process(double delta)
    {
        _gateway.RequireThread();
        FoundationAutomation.Tick(root: this, gateway: _gateway);
        PollKeys();
        double exact = delta * 1000 + _remainder;
        long elapsed = checked((long)exact);
        _remainder = exact - elapsed;
        _coordinator?.Update(elapsedMilliseconds: elapsed);
        if (_coordinator?.Ready == true)
        {
            if (_lastDirection != _input.Direction)
            {
                _coordinator.Submit(direction: _input.Direction);
                _lastDirection = _input.Direction;
            }

            ShowWorld();
        }

        _diagnostics.Observe(coordinator: _coordinator, inputEvents: _inputEvents, direction: _input.Direction,
                nativeActors: GetTree().GetNodesInGroup(group: "foundation-actor").Count,
                polledRight: Input.IsPhysicalKeyPressed(keycode: Key.D));
        Observe();
    }

    /// <summary>Closes the lifetime before native teardown removes scene nodes.</summary>
    public override void _ExitTree()
    {
        _gateway.RequireThread();
        _coordinator?.Close();
        _coordinator = null;
        _input.Clear();
        GetNode<Button>(path: "Menu/Start").Pressed -= Start;
        GetNode<Button>(path: "Menu/Quit").Pressed -= Quit;
        GetNode<Button>(path: "Menu/Host").Pressed -= Host;
        GetNode<Button>(path: "Menu/Join").Pressed -= Browse;
        GetNode<Button>(path: "Browser/JoinAddress").Pressed -= JoinAddress;
        GetNode<Button>(path: "Browser/Back").Pressed -= ReturnToMenu;
        GetWindow().FocusExited -= LoseFocus;
        if (_capture is not null)
        {
            RenderingServer.FramePostDraw -= Capture;
        }
    }

    private readonly MainThreadGateway _gateway = new();
    private readonly FoundationInput _input = new();
    private FoundationCoordinator? _coordinator;
    private readonly FoundationDiagnostics _diagnostics = new();
    private int _lastDirection = int.MinValue;
    private long _generation;
    private long _inputEvents;
    private double _remainder;
    private string? _capture;

    private void Start()
    {
        _gateway.RequireThread();
        Enter(host: false);
    }

    private void Host()
    {
        _gateway.RequireThread();
        Enter(host: true);
    }

    private void Enter(bool host)
    {
        _gateway.RequireThread();
        if (_coordinator is not null)
        {
            return;
        }

        _input.Clear();
        try
        {
            _generation = checked(_generation + 1);
            string player = System.Environment.GetEnvironmentVariable(variable: "PHASE3_PROFILE")
                    ?? FoundationEphemeralProfile.PlayerId;
            _coordinator = new FoundationCoordinator(playerId: player, label: "Player 1",
                    connection: host ? new EnetConnectionService(gateway: _gateway) : null);
            _coordinator.Start(host: host, port: Port());
            if (_generation == 1 && System.Environment.GetEnvironmentVariable(variable: "PHASE2_FAIL_ENTRY_ONCE")
                    is string fault && StringComparer.Ordinal.Equals(x: fault,
                            y: System.Environment.GetEnvironmentVariable(variable: "PHASE2_NONCE")))
            {
                throw new InvalidOperationException(message: "Scoped one-shot scene construction fixture");
            }

            _remainder = 0;
            _lastDirection = int.MinValue;
            ShowWorld();
        }
        catch (Exception error)
        {
            ReturnToMenu();
            GetNode<Label>(path: "Menu/Error").Text = "Could not start the scene. Try Start again.";
            GetNode<Control>(path: "Menu/Error").Visible = true;
            GD.Print(what: "Recoverable scene entry: " + error.Message);
        }
    }

    private void Browse()
    {
        _gateway.RequireThread();
        _input.Clear();
        GetNode<Control>(path: "Menu").Visible = false;
        GetNode<Control>(path: "Browser").Visible = true;
        GetNode<Control>(path: "Browser/Address").GrabFocus();
    }

    private void JoinAddress()
    {
        _gateway.RequireThread();
        if (_coordinator is not null)
        {
            return;
        }

        string address = GetNode<LineEdit>(path: "Browser/Address").Text.Trim();
        if (!FoundationEndpoint.TryParse(value: address, address: out string host, port: out int port))
        {
            GetNode<Label>(path: "Browser/Feedback").Text = "Enter a valid host address, such as 192.168.1.20.";
            return;
        }

        try
        {
            _input.Clear();
            _coordinator = new FoundationCoordinator(playerId: System.Environment.GetEnvironmentVariable(
                    variable: "PHASE3_PROFILE") ?? FoundationEphemeralProfile.PlayerId, label: "Player 2",
                    connection: new EnetConnectionService(gateway: _gateway));
            _coordinator.Join(address: host, port: port);
            _lastDirection = int.MinValue;
            GetNode<Label>(path: "Browser/Feedback").Text = "Connecting to " + host + "…";
        }
        catch (Exception error)
        {
            ReturnToMenu();
            GetNode<Label>(path: "Menu/Error").Text = "Could not join this session. Check the address and try again.";
            GetNode<Control>(path: "Menu/Error").Visible = true;
            GD.Print(what: "Recoverable join: " + error.Message);
        }
    }

    private void ShowWorld()
    {
        _gateway.RequireThread();
        if (_coordinator?.Snapshot is not FoundationSnapshot snapshot || !_coordinator.Ready)
        {
            return;
        }

        Node2D? world = GetNodeOrNull<Node2D>(path: "World");
        if (world is null)
        {
            using PackedScene fixture = GD.Load<PackedScene>(path: "res://foundation/FoundationWorld.tscn");
            world = fixture.Instantiate<Node2D>();
            world.Name = "World";
            AddChild(node: world);
            MoveChild(childNode: world, toIndex: 1);
            _input.Clear();
        }

        FoundationView.ProjectRoster(world: world, snapshot: snapshot, bindingId: _coordinator.BindingId,
                gateway: _gateway);
        GetNode<Control>(path: "Menu/Error").Visible = false;
        GetNode<Control>(path: "Menu").Visible = false;
        GetNode<Control>(path: "Browser").Visible = false;
        GetNode<Control>(path: "Hint").Visible = true;
    }

    private void PollKeys()
    {
        _gateway.RequireThread();
        foreach (Key physical in new[] { Key.A, Key.D, Key.Left, Key.Right, Key.Escape,
            Key.Enter, Key.Space, Key.KpEnter })
        {
            if (!Input.IsPhysicalKeyPressed(keycode: physical))
            {
                _input.Set(key: (int)physical, pressed: false);
            }
        }
    }

    private void ActivateFocused()
    {
        _gateway.RequireThread();
        if (GetNode<Button>(path: "Menu/Start").HasFocus())
        {
            Start();
        }
        else if (GetNode<Button>(path: "Menu/Host").HasFocus())
        {
            Host();
        }
        else if (GetNode<Button>(path: "Menu/Join").HasFocus())
        {
            Browse();
        }
        else if (GetNode<Button>(path: "Menu/Quit").HasFocus())
        {
            Quit();
        }
        else if (GetNode<Button>(path: "Browser/JoinAddress").HasFocus())
        {
            JoinAddress();
        }
        else if (GetNode<Button>(path: "Browser/Back").HasFocus())
        {
            ReturnToMenu();
        }
    }

    private static int Port()
    {
        string? value = System.Environment.GetEnvironmentVariable(variable: "PHASE3_PORT");
        return value is null ? 24680 : int.Parse(s: value, provider: CultureInfo.InvariantCulture);
    }

    private void ReturnToMenu()
    {
        _gateway.RequireThread();
        _coordinator?.Close();
        _coordinator = null;
        _input.Clear();
        _remainder = 0;
        Node2D? world = GetNodeOrNull<Node2D>(path: "World");
        if (world is not null)
        {
            RemoveChild(node: world);
            world.QueueFree();
        }
        GetNode<Control>(path: "Browser").Visible = false;
        GetNode<Control>(path: "Hint").Visible = false;
        GetNode<Control>(path: "Menu").Visible = true;
        GetNode<Button>(path: "Menu/Start").Disabled = false;
        GetNode<Button>(path: "Menu/Quit").Disabled = false;
        GetNode<Button>(path: "Menu/Start").GrabFocus();
    }

    private void Quit()
    {
        _gateway.RequireThread();
        _coordinator?.Close();
        _coordinator = null;
        _input.Clear();
        GetTree().Quit();
    }

    private void LoseFocus()
    {
        _gateway.RequireThread();
        _input.Clear();
    }

    private void Observe()
    {
        _gateway.RequireThread();
        string? nonce = System.Environment.GetEnvironmentVariable(variable: "PHASE2_NONCE");
        string? output = System.Environment.GetEnvironmentVariable(variable: "PHASE2_OUTPUT");
        if (nonce is null || output is null || _capture is not null)
        {
            return;
        }

        string request = Path.Combine(path1: output, path2: "observe.request");
        if (!File.Exists(path: request))
        {
            return;
        }

        string[] parts;
        try
        {
            parts = File.ReadAllText(path: request).Trim().Split(separator: '@');
        }
        catch (IOException)
        {
            return;
        }
        string checkpoint = parts[0];
        if (parts.Length == 2 && _coordinator?.LocalActor is FoundationActorState observed
                && observed.ElapsedMilliseconds < long.Parse(s: parts[1], provider: CultureInfo.InvariantCulture))
        {
            return;
        }
        File.Delete(path: request);
        if (checkpoint.Length is < 1 or > 48 || checkpoint.IndexOfAny(anyOf: ['/', '\\', '.']) >= 0)
        {
            throw new InvalidOperationException(message: "OBSERVATION_CHECKPOINT");
        }

        _capture = checkpoint;
        RenderingServer.FramePostDraw += Capture;
    }

    private void Capture()
    {
        _gateway.RequireThread();
        RenderingServer.FramePostDraw -= Capture;
        string checkpoint = _capture ?? throw new InvalidOperationException(message: "OBSERVATION_CAPTURE");
        string output = System.Environment.GetEnvironmentVariable(variable: "PHASE2_OUTPUT")
                ?? throw new InvalidOperationException(message: "OBSERVATION_OUTPUT");
        using Image image = GetViewport().GetTexture().GetImage();
        Error result = image.SavePng(path: Path.Combine(path1: output, path2: checkpoint + ".png"));
        if (result != Error.Ok)
        {
            throw new InvalidOperationException(message: "OBSERVATION_IMAGE: " + result);
        }

        string observation = Path.Combine(path1: output, path2: checkpoint + ".json");
        File.WriteAllText(path: observation + ".tmp", contents:
                JsonSerializer.Serialize(value: new
                {
                    nonce = System.Environment.GetEnvironmentVariable(variable: "PHASE2_NONCE"),
                    checkpoint, actors = GetTree().GetNodesInGroup(group: "foundation-actor").Count,
                    actor = _coordinator?.LocalActor,
                    inputEvents = _inputEvents, direction = _input.Direction,
                    startFocused = GetNode<Button>(path: "Menu/Start").HasFocus(),
                    quitFocused = GetNode<Button>(path: "Menu/Quit").HasFocus(),
                    startX = GetNode<Button>(path: "Menu/Start").GetGlobalRect().GetCenter().X,
                    startY = GetNode<Button>(path: "Menu/Start").GetGlobalRect().GetCenter().Y,
                    quitX = GetNode<Button>(path: "Menu/Quit").GetGlobalRect().GetCenter().X,
                    quitY = GetNode<Button>(path: "Menu/Quit").GetGlobalRect().GetCenter().Y,
                    errorVisible = GetNode<Control>(path: "Menu/Error").Visible,
                    menuVisible = GetNode<Control>(path: "Menu").Visible,
                    cameraX = _coordinator?.LocalActor is null ? 0 : 640 - GetNode<Node2D>(path: "World").Position.X,
                    focused = GetWindow().HasFocus(), width = image.GetWidth(), height = image.GetHeight(),
                }));
        File.Move(sourceFileName: observation + ".tmp", destFileName: observation);
        _capture = null;
    }
}
