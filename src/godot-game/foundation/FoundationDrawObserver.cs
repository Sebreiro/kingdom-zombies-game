using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Observes completed native draws without changing controls, sessions or authoritative state.</summary>
internal sealed class FoundationDrawObserver
{
    internal FoundationDrawObserver(Control root, MainThreadGateway gateway)
    {
        _root = root;
        _gateway = gateway;
    }

    internal string Sample { get; set; } = "{}";

    internal void Start()
    {
        _gateway.RequireThread();
        _directory = FoundationObservationScope.Directory();
        if (_directory is not null && !OS.HasFeature(tagName: "headless"))
        {
            RenderingServer.FramePostDraw += Draw;
            _attached = true;
        }
    }

    internal void Stop()
    {
        _gateway.RequireThread();
        if (_attached) { RenderingServer.FramePostDraw -= Draw; }
        _attached = false;
    }

    internal string InspectControls(string sample)
    {
        using JsonDocument document = JsonDocument.Parse(json: sample);
        _sample = document.RootElement;
        ObserveControls();
        return Serialize(width: 1280, height: 720, focused: false, cameraX: 0);
    }

    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly List<object> _actors = [];
    private readonly List<object> _controls = [];
    private string? _directory;
    private bool _attached;
    private long _frame;
    private ulong _drawMicroseconds;
    private JsonElement _sample;
    private Node2D? _world;
    private string? _checkpoint;
    private static readonly string[] _controlPaths = ["Menu/Start", "Menu/Host", "Menu/Join", "Menu/Quit",
        "Menu/Profiles", "Menu/Name", "Menu/NewProfile", "Menu/Error", "Browser/Refresh", "Browser/Address",
        "Browser/JoinAddress", "Browser/Footer/JoinSession", "Browser/Footer/Cancel", "Browser/Footer/Back",
        "Browser/Feedback", "Browser/Discovery", "Browser/Detail", "Browser/Summary", "Browser/Scroll", "Roster/Header",
        "Roster/Player1", "Roster/Player2", "Roster/Player3", "Roster/Player4", "Roster/Waiting", "Hint"];
    private string Directory => _directory ?? throw new InvalidDataException(message: "DRAW_DIRECTORY");

    private void Draw()
    {
        _gateway.RequireThread();
        _drawMicroseconds = Time.GetTicksUsec();
        _frame++;
        using JsonDocument document = JsonDocument.Parse(json: Sample);
        _sample = document.RootElement;
        _world = _root.GetNodeOrNull<Node2D>(path: "World");
        ObserveActors();
        ObserveControls();
        _checkpoint = Request();
        if (_checkpoint is not null) { Capture(); }
        Write(width: 1280, height: 720);
    }

    private void ObserveActors()
    {
        _gateway.RequireThread();
        _actors.Clear();
        if (_world is null || !_sample.TryGetProperty(propertyName: "snapshot", value: out JsonElement snapshot)
                || snapshot.ValueKind is JsonValueKind.Null) { return; }
        foreach (JsonElement participant in snapshot.GetProperty(propertyName: "Participants").EnumerateArray())
        {
            if (participant.GetProperty(propertyName: "Ready").GetBoolean()) { ObserveActor(participant: participant); }
        }
    }

    private void ObserveActor(JsonElement participant)
    {
        _gateway.RequireThread();
        JsonElement actor = participant.GetProperty(propertyName: "Actor");
        string actorId = actor.GetProperty(propertyName: "ActorId").GetString()
                ?? throw new InvalidDataException(message: "DRAW_ACTOR_ID");
        bool local = IsLocal(participant: participant);
        Node2D world = _world ?? throw new InvalidDataException(message: "DRAW_WORLD");
        Node2D node = world.GetNode<Node2D>(path: local ? "Player" : FoundationActorNode.Name(actorId: actorId));
        Vector2 screen = node.GetGlobalTransformWithCanvas().Origin;
        _actors.Add(item: new { actorId, playerId = actor.GetProperty(propertyName: "PlayerId").GetString(),
            bindingId = participant.GetProperty(propertyName: "BindingId").GetString(), local,
            x = node.Position.X, y = node.Position.Y, screenX = screen.X, screenY = screen.Y,
            facing = node.GetNode<Node2D>(path: "Facing").Scale.X, visible = node.IsVisibleInTree(),
            insideCamera = screen.X >= 16 && screen.X <= 1264 && screen.Y >= 112 && screen.Y <= 704 });
    }

    private bool IsLocal(JsonElement participant)
    {
        return StringComparer.Ordinal.Equals(x: participant.GetProperty(propertyName: "BindingId").GetString(),
                y: _sample.GetProperty(propertyName: "bindingId").GetString());
    }

    private void ObserveControls()
    {
        _gateway.RequireThread();
        _controls.Clear();
        foreach (string path in _controlPaths)
        {
            ObserveControl(path: path);
        }

        foreach (Node child in _root.GetNode<Control>(path: "Browser/Scroll/Rows").GetChildren())
        {
            if (child is Control control)
            {
                ObserveControl(path: _root.GetPathTo(node: control).ToString());
            }
        }
    }

    private void ObserveControl(string path)
    {
        _gateway.RequireThread();
        Control control = _root.GetNode<Control>(path: path);
        Rect2 rect = control.GetGlobalRect();
        _controls.Add(item: new { path, x = rect.GetCenter().X, y = rect.GetCenter().Y,
            session = control.GetMeta(name: "session", @default: "").AsString(),
            width = rect.Size.X, height = rect.Size.Y, visible = control.IsVisibleInTree(),
            selectedIndex = control is OptionButton selector ? selector.Selected : -1,
            popupVisible = control is OptionButton popup ? popup.GetPopup().Visible : false,
            popupFocusedIndex = control is OptionButton highlight ? highlight.GetPopup().GetFocusedItem() : -1,
            focused = control.HasFocus(), text = control is Label label ? label.Text
                    : control is Button button ? button.Text : control is LineEdit edit ? edit.Text : "" });
    }

    private string? Request()
    {
        string file = Path.Combine(path1: Directory, path2: "draw.request");
        return FoundationDrawRequest.Consume(file: file, nonce:
                _sample.GetProperty(propertyName: "nonce").GetString() ?? "");
    }

    private void Capture()
    {
        _gateway.RequireThread();
        using Image image = _root.GetViewport().GetTexture().GetImage();
        string file = Path.Combine(path1: Directory, path2: _checkpoint + ".png");
        if (File.Exists(path: file) || image.GetWidth() != 1280 || image.GetHeight() != 720
                || image.SavePng(path: file) != Error.Ok)
        {
            throw new InvalidDataException(message: "DRAW_IMAGE");
        }
    }

    private void Write(int width, int height)
    {
        _gateway.RequireThread();
        string record = Serialize(width: width, height: height, focused: _root.GetWindow().HasFocus(),
                cameraX: _world is null ? 0 : 640 - _world.Position.X);
        File.AppendAllText(path: Path.Combine(path1: Directory, path2: "draws.ndjson"), contents: record + "\n");
        if (_checkpoint is not null) { FoundationCapturePublication.Publish(file: Path.Combine(path1: Directory,
                path2: _checkpoint + ".json"), record: record); }
    }

    private string Serialize(int width, int height, bool focused, float cameraX)
    {
        return JsonSerializer.Serialize(value: new { nonce = _sample.GetProperty(propertyName: "nonce"),
            pid = System.Environment.ProcessId,
            role = System.Environment.GetEnvironmentVariable(variable: "PHASE3_ROLE"),
            profile = _sample.GetProperty(propertyName: "profile").ValueKind is JsonValueKind.Null ? (string?)null
                    : _sample.GetProperty(propertyName: "profile").GetProperty(propertyName: "PlayerId").GetString(),
            playerId = _sample.GetProperty(propertyName: "playerId"),
            bindingId = _sample.GetProperty(propertyName: "bindingId"),
            snapshot = _sample.GetProperty(propertyName: "snapshot"),
            input = _sample, drawn = _frame > 0, drawFrame = _frame, drawMicroseconds = _drawMicroseconds,
            checkpoint = _checkpoint, width, height, actors = _actors, controls = _controls,
            focused, cameraX });
    }
}
