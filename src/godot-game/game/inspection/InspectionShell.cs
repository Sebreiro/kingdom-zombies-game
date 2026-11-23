using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Owns inspection layout, native transport, authored samples and replaceable preview children.</summary>
internal sealed class InspectionShell
{
    internal InspectionShell(Node2D root, MainThreadGateway gateway)
    {
        _root = root;
        _gateway = gateway;
    }

    internal void Attach()
    {
        _gateway.RequireThread();
        Control browser = _root.GetNode<Control>(path: "Shell/Workspace/Browser");
        var scroll = new ScrollContainer { Name = "Controls", FollowFocus = true };
        _root.GetNode(path: "Shell/Workspace").AddChild(node: scroll);
        browser.Reparent(newParent: scroll, keepGlobalTransform: false);
        browser.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        var transport = new HFlowContainer { Name = "Transport" };
        _root.GetNode(path: Preview).AddChild(node: transport);
        _root.GetNode(path: Preview).MoveChild(childNode: transport, toIndex: 2);
        AddButton(parent: transport, name: "Play", text: "Play preview", action: Play);
        var speed = new OptionButton { Name = "Speed", CustomMinimumSize = new Vector2(x: 0, y: 48) };
        transport.AddChild(node: speed);
        foreach (string label in new[] { "0.25×", "0.5×", "1×", "2×" })
        {
            speed.AddItem(label: label);
        }

        speed.Select(idx: 2);
        speed.ItemSelected += Speed;
        AddButton(parent: transport, name: "Step", text: "Step frame", action: Step);
        AddButton(parent: transport, name: "Reset", text: "Reset preview", action: Reset);
        var sample = new Label { Name = "Sample", AutowrapMode = TextServer.AutowrapMode.WordSmart };
        _root.GetNode(path: Preview).AddChild(node: sample);
        _root.GetNode(path: Preview).MoveChild(childNode: sample, toIndex: 3);
        var current = new Node2D { Name = "Current" };
        _root.GetNode(path: Preview + "/Surface").AddChild(node: current);
        Layout();
        ShowTransport();
    }

    internal void Layout()
    {
        _gateway.RequireThread();
        Vector2 viewport = _root.GetViewportRect().Size;
        bool compact = viewport.X < 1040;
        Control shell = _root.GetNode<Control>(path: "Shell");
        shell.Size = new Vector2(x: Math.Max(val1: 320, val2: viewport.X - 48),
                y: Math.Max(val1: 240, val2: viewport.Y - 218));
        BoxContainer workspace = _root.GetNode<BoxContainer>(path: "Shell/Workspace");
        workspace.Vertical = compact;
        Control controls = workspace.GetNode<Control>(path: "Controls");
        controls.CustomMinimumSize = new Vector2(x: compact ? 0 : 320, y: compact ? 144 : 0);
        controls.SizeFlagsHorizontal = compact ? Control.SizeFlags.ExpandFill : Control.SizeFlags.Fill;
        controls.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        Control navigation = workspace.GetNode<Control>(path: "Navigation");
        navigation.CustomMinimumSize = new Vector2(x: compact ? 0 : 176, y: 0);
        navigation.GetNode<Control>(path: "Scope").Visible = !compact;
        workspace.MoveChild(childNode: workspace.GetNode(path: "Preview"), toIndex: compact ? 1 : 2);
        workspace.MoveChild(childNode: controls, toIndex: compact ? 2 : 1);
        _root.GetNode<Control>(path: "Summary").Size = new Vector2(x: viewport.X - 48, y: 160);
        _root.GetNode<Sprite2D>(path: "Bow").Position = _root.GetNode<Control>(path: Preview + "/Surface")
                .GetGlobalRect().GetCenter();
    }

    internal void Tick(double delta)
    {
        _gateway.RequireThread();
        Layout();
        if (_playing && _player is not null && double.IsFinite(d: delta) && delta >= 0)
        {
            _time = Math.Min(val1: _length, val2: _time + delta * _speed);
            _player.Seek(seconds: _time, update: true);
            if (_time >= _length)
            {
                _playing = false;
            }
        }

        ShowTransport();
    }

    internal void Install(Node preview)
    {
        _gateway.RequireThread();
        Retire();
        _root.GetNode(path: Preview + "/Surface/Current").AddChild(node: preview);
        _player = preview as AnimationPlayer ?? preview.FindChildren(pattern: "*", type: "AnimationPlayer",
                recursive: true, owned: false).OfType<AnimationPlayer>().FirstOrDefault();
        if (_player is not null)
        {
            string? clip = _player.GetAnimationList().FirstOrDefault(predicate: value => value is not "RESET");
            if (clip is not null)
            {
                _player.Play(name: clip);
                _player.Pause();
                _player.CallbackModeProcess = AnimationMixer.AnimationCallbackModeProcess.Manual;
                Animation animation = _player.GetAnimation(name: clip);
                _length = animation.Length;
                var samples = new HashSet<double> { 0, _length };
                for (int track = 0; track < animation.GetTrackCount(); track++)
                {
                    for (int key = 0; key < animation.TrackGetKeyCount(trackIdx: track); key++)
                    {
                        samples.Add(item: animation.TrackGetKeyTime(trackIdx: track, keyIdx: key));
                    }
                }

                _samples = samples.Order().ToArray();
                _player.Seek(seconds: 0, update: true);
            }
        }

        ShowTransport();
    }

    internal void Retire()
    {
        _gateway.RequireThread();
        _playing = false;
        _player = null;
        _time = 0;
        _length = 0;
        _samples = [];
        foreach (Node node in _root.GetNode(path: Preview + "/Surface/Current").GetChildren())
        {
            foreach (Node audio in node.FindChildren(pattern: "*", type: "AudioStreamPlayer", recursive: true,
                    owned: false))
            {
                ((AudioStreamPlayer)audio).Stop();
            }

            if (node is AudioStreamPlayer player)
            {
                player.Stop();
            }

            _root.GetNode(path: Preview + "/Surface/Current").RemoveChild(node: node);
            node.QueueFree();
        }

        ShowTransport();
    }

    internal void Detach()
    {
        _gateway.RequireThread();
        Retire();
        _root.GetNode<Button>(path: Preview + "/Transport/Play").Pressed -= Play;
        _root.GetNode<Button>(path: Preview + "/Transport/Step").Pressed -= Step;
        _root.GetNode<Button>(path: Preview + "/Transport/Reset").Pressed -= Reset;
        _root.GetNode<OptionButton>(path: Preview + "/Transport/Speed").ItemSelected -= Speed;
    }

    private const string Preview = "Shell/Workspace/Preview";
    private readonly Node2D _root;
    private readonly MainThreadGateway _gateway;
    private AnimationPlayer? _player;
    private double[] _samples = [];
    private double _time;
    private double _length;
    private double _speed = 1;
    private bool _playing;

    private void AddButton(Node parent, string name, string text, Action action)
    {
        _gateway.RequireThread();
        var button = new Button { Name = name, Text = text, CustomMinimumSize = new Vector2(x: 0, y: 48) };
        parent.AddChild(node: button);
        button.Pressed += action;
    }

    private void Play()
    {
        _gateway.RequireThread();
        if (_samples.Length > 0)
        {
            _playing = !_playing;
        }

        ShowTransport();
    }

    private void Speed(long index)
    {
        _gateway.RequireThread();
        _speed = index switch { 0 => 0.25, 1 => 0.5, 2 => 1, 3 => 2, _ => 1 };
    }

    private void Step()
    {
        _gateway.RequireThread();
        if (!_playing && _player is not null && _samples.Length > 0)
        {
            _time = _samples.FirstOrDefault(predicate: value => value > _time + 0.0000001, defaultValue: _length);
            _player.Seek(seconds: _time, update: true);
        }

        ShowTransport();
    }

    private void Reset()
    {
        _gateway.RequireThread();
        _playing = false;
        _time = 0;
        _player?.Seek(seconds: 0, update: true);
        ShowTransport();
    }

    private void ShowTransport()
    {
        _gateway.RequireThread();
        bool available = _samples.Length > 0;
        _root.GetNode<Button>(path: Preview + "/Transport/Play").Text = _playing ? "Pause preview" : "Play preview";
        _root.GetNode<Button>(path: Preview + "/Transport/Play").Disabled = !available;
        _root.GetNode<Button>(path: Preview + "/Transport/Step").Disabled = !available || _playing;
        string sample = Array.FindLastIndex(array: _samples, match: value => value <= _time)
                .ToString(provider: CultureInfo.InvariantCulture);
        string count = (_samples.Length - 1).ToString(provider: CultureInfo.InvariantCulture);
        _root.GetNode<Label>(path: Preview + "/Sample").Text = available
                ? "Authored sample " + sample + "/" + count + " · "
                        + _time.ToString(format: "0.000", provider: CultureInfo.InvariantCulture) + "s"
                : "No authored animation in this resource; playback and stepping are unavailable.";
    }
}
