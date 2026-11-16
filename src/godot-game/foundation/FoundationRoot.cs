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
        _services = new FoundationServices(identity: new LocalProfileService(directory:
                System.Environment.GetEnvironmentVariable(variable: "PHASE3_PROFILE_ROOT")
                        ?? Path.Combine(path1: OS.GetUserDataDir(), path2: "profiles")),
                discovery: new LanDiscoveryService(), connection: () => new EnetConnectionService(gateway: _gateway));
        _menu = new FoundationMenu(root: this, gateway: _gateway, services: _services, join: JoinAddress,
                back: ReturnToMenu, coordinator: () => _coordinator, replace: value =>
                {
                    _runtime.Retire(current: ref _coordinator, closing: ref _closing);
                    _coordinator = value;
                },
                input: _input, runtime: _runtime);
        _menu.Attach();
        _diagnostics.Attach(root: this, gateway: _gateway);
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
        _diagnostics.RecordInput(key: (int)key.PhysicalKeycode, pressed: key.Pressed, echo: key.Echo);
        _input.SetContext(gameplay: _coordinator?.Ready == true);
        if (_menu?.PopupOpen() == true)
        {
            return;
        }

        if (!key.Echo)
        {
            _input.Set(key: (int)key.PhysicalKeycode, pressed: key.Pressed);
        }
        if (key.Keycode == Key.Escape && _input.FreshPress && !key.Echo
                && !(GetViewport().GuiGetFocusOwner() is LineEdit edit && edit.IsEditing()))
        {
            _menu?.Escape();
            GetViewport().SetInputAsHandled();
            return;
        }
        if (_coordinator?.Ready != true && key.Keycode is Key.Enter or Key.Space or Key.KpEnter
                && GetViewport().GuiGetFocusOwner() is Button and not OptionButton)
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
        _runtime.NowMilliseconds = checked((long)Time.GetTicksMsec());
        FoundationAutomation.Tick(root: this, gateway: _gateway);
        PollKeys();
        double exact = delta * 1000 + _remainder;
        long elapsed = checked((long)exact);
        _remainder = exact - elapsed;
        if (_coordinator?.Ready == true && _lastDirection != _input.Direction)
        {
            _coordinator.Submit(direction: _input.Direction);
            _lastDirection = _input.Direction;
        }

        _coordinator?.Update(elapsedMilliseconds: elapsed);
        _closing?.Update(elapsedMilliseconds: 0);
        _runtime.TickRetired();
        if (_coordinator?.Finished == true)
        {
            string reason = _coordinator.Error;
            if (GetNode<Control>(path: "Browser").Visible)
            {
                _menu?.CancelAttempt();
                _menu?.Recover(message: reason);
            }
            else
            {
                ReturnToMenu();
                GetNode<Label>(path: "Menu/Error").Text = FoundationCopy.Error(reason: reason);
                GetNode<Control>(path: "Menu/Error").Visible = true;
            }

            _runtime.Feedback = reason;
        }
        if (_coordinator?.Ready == true)
        {
            ShowWorld();
        }

        _menu?.Tick(nowMilliseconds: _runtime.NowMilliseconds);

        _diagnostics.Observe(coordinator: _coordinator, inputEvents: _inputEvents, direction: _input.Direction,
                nativeActors: GetTree().GetNodesInGroup(group: "foundation-actor").Count,
                polledRight: Input.IsPhysicalKeyPressed(keycode: Key.D), feedback: _runtime.Feedback,
                nowMilliseconds: _runtime.NowMilliseconds, services: _services);
        Observe();
    }

    /// <summary>Closes the lifetime before native teardown removes scene nodes.</summary>
    public override void _ExitTree()
    {
        _gateway.RequireThread();
        _coordinator?.Close();
        _closing?.Close();
        _runtime.CloseRetired();
        _diagnostics.Detach();
        _coordinator = null;
        _input.Clear();
        GetNode<Button>(path: "Menu/Start").Pressed -= Start;
        GetNode<Button>(path: "Menu/Quit").Pressed -= Quit;
        GetNode<Button>(path: "Menu/Host").Pressed -= Host;
        GetNode<Button>(path: "Menu/Join").Pressed -= Browse;
        GetNode<Button>(path: "Browser/JoinAddress").Pressed -= JoinAddress;
        _menu?.Detach();
        GetWindow().FocusExited -= LoseFocus;
        if (_capture is not null)
        {
            RenderingServer.FramePostDraw -= Capture;
        }
    }

    private readonly MainThreadGateway _gateway = new();
    private readonly FoundationInput _input = new();
    private FoundationCoordinator? _coordinator;
    private FoundationCoordinator? _closing;
    private readonly FoundationRuntimeState _runtime = new();
    private readonly FoundationDiagnostics _diagnostics = new();
    private FoundationServices? _services;
    private FoundationMenu? _menu;
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
        if (_coordinator is not null || _menu?.CommitName() != true)
        {
            return;
        }

        _input.Clear();
        try
        {
            _runtime.NowMilliseconds = checked((long)Time.GetTicksMsec());
            _runtime.Feedback = "";
            _generation = checked(_generation + 1);
            _runtime.Hosting = host;
            _coordinator = (_services ?? throw new InvalidOperationException(message: "SERVICES_NOT_READY"))
                    .Create(connected: host, nowMilliseconds: () => _runtime.NowMilliseconds);
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
            GetNode<Label>(path: "Menu/Error").Text = host
                    ? "Could not host a session. Check your network and try Host session again."
                    : "Could not start the scene. Try Start again.";
            GetNode<Control>(path: "Menu/Error").Visible = true;
            GD.Print(what: "Recoverable scene entry: " + error.Message);
        }
    }

    private void Browse()
    {
        _gateway.RequireThread();
        if (_coordinator is not null || _menu?.CommitName() != true)
        {
            return;
        }

        _input.Clear();
        GetNode<Control>(path: "Menu").Visible = false;
        GetNode<Control>(path: "Browser").Visible = true;
        _menu.Browse();
    }

    private void JoinAddress()
    {
        _gateway.RequireThread();
        if (_coordinator is not null)
        {
            return;
        }

        string address = _menu?.RequestedAddress ?? GetNode<LineEdit>(path: "Browser/Address").Text;
        if (!FoundationEndpoint.TryParse(value: address.Trim(), address: out string host, port: out int port))
        {
            GetNode<Label>(path: "Browser/Feedback").Text = "Enter a valid host address, then choose Join address.";
            GetNode<LineEdit>(path: "Browser/Address").GrabFocus();
            return;
        }

        try
        {
            _runtime.NowMilliseconds = checked((long)Time.GetTicksMsec());
            _runtime.Feedback = "";
            _input.Clear();
            _input.SetContext(gameplay: true);
            _runtime.Hosting = false;
            _coordinator = (_services ?? throw new InvalidOperationException(message: "SERVICES_NOT_READY"))
                    .Create(connected: true, nowMilliseconds: () => _runtime.NowMilliseconds);
            _coordinator.Join(address: host, port: port, expectedSessionId: _menu?.RequestedSessionId ?? "");
            _lastDirection = int.MinValue;
            GetNode<Label>(path: "Browser/Feedback").Text = "Connecting to " + host + "…";
            _menu?.Attempt(origin: GetViewport().GuiGetFocusOwner()?.GetPath().ToString()
                    .EndsWith(value: "JoinSession", comparisonType: StringComparison.Ordinal) == true
                    ? "Browser/Footer/JoinSession" : "Browser/JoinAddress");
        }
        catch (Exception error)
        {
            _menu?.CancelAttempt();
            _menu?.Recover(message: "LAN connection is unavailable. Try again after checking your network.");
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
        FoundationHud.Project(root: this, snapshot: snapshot, bindingId: _coordinator.BindingId,
                hosting: _runtime.Hosting, waitingForHost: _coordinator.WaitingForHost, gateway: _gateway);
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
        if (GetViewport().GuiGetFocusOwner() is Button button && button is not OptionButton)
        {
            _menu?.Activate(button: button);
        }
    }

    private static int Port()
    {
        string? value = System.Environment.GetEnvironmentVariable(variable: "PHASE3_PORT");
        return value is null ? 47778 : int.Parse(s: value, provider: CultureInfo.InvariantCulture);
    }

    private void ReturnToMenu()
    {
        _gateway.RequireThread();
        _runtime.Retire(current: ref _coordinator, closing: ref _closing);
        _services?.Discovery.Advertise(advertisement: null);
        _input.Clear();
        _input.SetContext(gameplay: false);
        _remainder = 0;
        Node2D? world = GetNodeOrNull<Node2D>(path: "World");
        if (world is not null)
        {
            RemoveChild(node: world);
            world.QueueFree();
        }
        GetNode<Control>(path: "Browser").Visible = false;
        GetNode<Control>(path: "Hint").Visible = false;
        GetNode<Control>(path: "Roster").Visible = false;
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
        _diagnostics.Quitting();
        GetTree().Quit();
    }

    private void LoseFocus()
    {
        _gateway.RequireThread();
        _diagnostics.LostFocus();
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
        if (!FoundationObservationScope.ValidCheckpoint(value: checkpoint, maximumLength: 48))
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
                    focusedControl = GetViewport().GuiGetFocusOwner()?.Name.ToString(),
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
