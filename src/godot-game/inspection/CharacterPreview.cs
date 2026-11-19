using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Owns fitted body and equipment layers without becoming another public Godot script.</summary>
internal sealed class CharacterPreview
{
    internal CharacterPreview(Node2D root, MainThreadGateway gateway, JsonElement fits, JsonElement bodies,
            JsonElement weapons, JsonElement clothing, string body)
    {
        _root = root;
        _gateway = gateway;
        _fits = fits;
        _bodies = bodies;
        _packs = [weapons, clothing];
        _body = body;
    }

    internal static JsonElement ReadDefinition(byte[] bytes, string expectedId)
    {
        if (bytes.Length is 0 or > 1048576 || bytes[0] == 239)
        {
            throw new InvalidDataException(message: "CHARACTER_DEFINITION_SIZE");
        }

        using var document = JsonDocument.Parse(utf8Json: bytes, options: new JsonDocumentOptions { MaxDepth = 24 });
        JsonElement root = document.RootElement;
        ValidateValue(value: root);
        if (root.GetProperty(propertyName: "schemaVersion").GetInt32() != 1
                || root.GetProperty(propertyName: "version").GetInt32() != 1
                || !StringComparer.Ordinal.Equals(x: Text(value: root, name: "id"), y: expectedId))
        {
            throw new InvalidDataException(message: "CHARACTER_DEFINITION_ID");
        }

        if (expectedId.StartsWith(value: "fits-", comparisonType: StringComparison.Ordinal))
        {
            ValidateFitting(root: root);
        }

        return root.Clone();
    }



    internal static CharacterPreview Open(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read,
            string body)
    {
        gateway.RequireThread();
        JsonElement[] profiles = [ReadProfile(read: read, profile: "a"), ReadProfile(read: read, profile: "b"),
            ReadProfile(read: read, profile: "c")];
        JsonElement fits = MergeFits(profiles: profiles);
        JsonElement bodies = MergeBodies(profiles: profiles);
        JsonElement weapons = ReadDefinition(bytes: read(arg: "content/packs/items-01-10.json")
                ?? throw new InvalidDataException(message: "CHARACTER_ITEMS_MISSING"), expectedId: "items-01-10");
        JsonElement clothing = ReadDefinition(bytes: read(arg: "content/packs/items-11-24.json")
                ?? throw new InvalidDataException(message: "CHARACTER_ITEMS_MISSING"), expectedId: "items-11-24");
        var preview = new CharacterPreview(root: root, gateway: gateway, fits: fits, bodies: bodies, weapons: weapons,
                clothing: clothing, body: body);
        preview._presentation = ReadPresentation(read: read);
        preview._bow = ReadDefinition(bytes: read(arg: "content/animation/bow.json")
                ?? throw new InvalidDataException(message: "BOW_MISSING"), expectedId: "bow");
        preview._work = ReadWork(read: read);
        return preview;
    }







    internal static CharacterPreview OpenComposition(Node2D root, MainThreadGateway gateway,
            Func<string, byte[]?> read, string body)
    {
        gateway.RequireThread();
        string profile = body is "CH-01" or "CH-02" or "CH-03" ? "a"
                : body is "CH-04" or "CH-05" or "CH-08" ? "b" : "c";
        JsonElement[] profiles = [ReadProfile(read: read, profile: profile)];
        JsonElement weapons = ReadDefinition(bytes: read(arg: "content/packs/items-01-10.json")
                ?? throw new InvalidDataException(message: "CHARACTER_ITEMS_MISSING"), expectedId: "items-01-10");
        JsonElement clothing = ReadDefinition(bytes: read(arg: "content/packs/items-11-24.json")
                ?? throw new InvalidDataException(message: "CHARACTER_ITEMS_MISSING"), expectedId: "items-11-24");
        var preview = new CharacterPreview(root: root, gateway: gateway, fits: MergeFits(profiles: profiles),
                bodies: MergeBodies(profiles: profiles), weapons: weapons, clothing: clothing, body: body);
        if (!preview._fits.GetProperty(propertyName: "bodies").EnumerateArray().Any(predicate: value =>
                StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: body)) || root.HasNode(path: "Art"))
        {
            throw new InvalidDataException(message: "COMPOSITION_OWNER_OR_BODY");
        }

        root.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        root.AddChild(node: new Node2D { Name = "Art" });
        preview._composition = true;
        preview._attached = true;
        return preview;
    }

    internal void RestoreComposition(string body, string pose, int facing, JsonElement equipment)
    {
        _gateway.RequireThread();
        if (!_composition || !_attached || equipment.ValueKind is not JsonValueKind.Object
                || equipment.GetRawText().Length > 4096 || facing is not (-1 or 1))
        {
            throw new InvalidOperationException(message: "COMPOSITION_INACTIVE_OR_SELECTION");
        }

        JsonElement definition = _fits.GetProperty(propertyName: "bodies").EnumerateArray().Single(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: body));
        JsonElement authored = FindPose(body: definition, id: pose);
        var names = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonProperty entry in equipment.EnumerateObject())
        {
            JsonElement slot = _fits.GetProperty(propertyName: "slots").EnumerateArray().Single(
                    predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"),
                            y: entry.Name));
            if (!names.Add(item: entry.Name) || entry.Value.ValueKind is not JsonValueKind.String
                    || !slot.GetProperty(propertyName: "items").EnumerateArray().Any(predicate: value =>
                            StringComparer.Ordinal.Equals(x: value.GetString(), y: entry.Value.GetString())))
            {
                throw new InvalidDataException(message: "COMPOSITION_EQUIPMENT");
            }

            string item = entry.Value.GetString() ?? throw new InvalidDataException(message: "COMPOSITION_ITEM");
            _ = FitItem(fits: _fits, body: body, item: item, pose: pose, facing: facing);
        }

        RetireArt();
        _body = body;
        _sample = default;
        Node2D art = _root.GetNode<Node2D>(path: "Art");
        AddBody(art: art, pose: authored, facing: facing, lower: authored);
        var diagnostics = new List<string>();
        foreach (JsonProperty entry in equipment.EnumerateObject())
        {
            AddItem(art: art, body: definition, pose: authored, slot: entry.Name,
                    itemId: entry.Value.GetString() ?? throw new InvalidDataException(message: "COMPOSITION_ITEM"),
                    facing: facing, diagnostics: diagnostics);
        }

        _root.SetMeta(name: "composition", value: JsonSerializer.Serialize(value: new
        {
            body, pose, facing, equipment = equipment.Clone(), diagnostics, loops = 0, controls = 0,
        }));
    }

    internal void CloseComposition()
    {
        _gateway.RequireThread();
        if (_composition && _attached)
        {
            _attached = false;
            RetireArt();
            Node2D art = _root.GetNode<Node2D>(path: "Art");
            _root.RemoveChild(node: art);
            art.QueueFree();
            _textures.Clear();
            _root.RemoveMeta(name: "composition");
        }
    }

    internal static JsonElement FitItem(JsonElement fits, string body, string item, string pose, int facing)
    {
        bool hasPose = fits.GetProperty(propertyName: "bodies").EnumerateArray().Any(predicate: candidate =>
                StringComparer.Ordinal.Equals(x: Text(value: candidate, name: "id"), y: body)
                        && candidate.GetProperty(propertyName: "poses").EnumerateArray().Any(predicate: value =>
                                StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: pose)));
        JsonElement result = fits.GetProperty(propertyName: "items").EnumerateArray().FirstOrDefault(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: item));
        if (!hasPose || facing is not (-1 or 1) || result.ValueKind is JsonValueKind.Undefined)
        {
            throw new InvalidDataException(message: "FIT_INCOMPLETE body=" + body + " item=" + item + " pose="
                    + pose + " facing=" + facing.ToString(provider: System.Globalization.CultureInfo.InvariantCulture));
        }

        return result;
    }

    internal void Attach()
    {
        _gateway.RequireThread();
        if (_attached)
        {
            throw new InvalidOperationException(message: "CHARACTER_ALREADY_ATTACHED");
        }

        _root.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        var controls = new ScrollContainer
        {
            Name = "Controls",
            Position = new Vector2(x: 390, y: 0),
            Size = new Vector2(x: 260, y: 320),
            FollowFocus = true
        };
        _root.AddChild(node: controls);
        var slots = new VBoxContainer { Name = "Slots", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        controls.AddChild(node: slots);
        int selected = BodyChoice(slots: slots);
        AddSelector(parent: slots, id: "Pose", label: "Authored pose", values: _fits.GetProperty(
                propertyName: "bodies")[0].GetProperty(propertyName: "poses").EnumerateArray()
                .Select(selector: pose => Text(value: pose, name: "id")));
        AddSelector(parent: slots, id: "Facing", label: "Facing", values: ["Right", "Left"]);
        AddSelector(parent: slots, id: "Condition", label: "Equipment condition", values: ["Usable", "Broken"]);
        AddSelector(parent: slots, id: "Zoom", label: "Inspection zoom", values: ["1x", "2x", "4x", "8x"]);
        foreach (JsonElement slot in _fits.GetProperty(propertyName: "slots").EnumerateArray())
        {
            AddSelector(parent: slots, id: Text(value: slot, name: "id"), label: Text(value: slot, name: "label"),
                    values: Empty.Concat(second: slot.GetProperty(propertyName: "items")
                            .EnumerateArray().Select(selector: item => item.GetString()
                                    ?? throw new InvalidDataException(message: "CHARACTER_ITEM_ID"))));
        }

        if (_presentation is not null)
        {
            AnimationControls(slots: slots);
        }

        _root.AddChild(node: new Node2D { Name = "Art", Position = new Vector2(x: 0, y: 0) });
        _root.AddChild(node: new Label
        {
            Name = "Status",
            Position = new Vector2(x: 0, y: 322),
            Size = new Vector2(x: 650, y: 80),
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        });
        _controls["Body"].Select(idx: selected);
        _controls["Zoom"].Select(idx: 3);
        _attached = true;
        Project();
        if (_presentation is not null)
        {
            _tree = _root.GetTree();
            _tree.ProcessFrame += Frame;
        }
    }



    internal void Detach()
    {
        _gateway.RequireThread();
        EndAction();
        _attached = false;
        _playing = false;
        if (_tree is not null)
        {
            _tree.ProcessFrame -= Frame;
            _tree = null;
        }

        foreach (OptionButton control in _controls.Values)
        {
            control.ItemSelected -= Changed;
        }

        if (_presentation is not null)
        {
            _root.GetNode<HSlider>(path: "Controls/Slots/ContinuousAim").ValueChanged -= AimChanged;
        }

        foreach ((string id, Button button) in _buttons)
        {
            button.Pressed -= TransportAction(id: id);
        }

        RetireArt();
    }

    internal void Tick(double delta)
    {
        _gateway.RequireThread();
        if (!_attached)
        {
            throw new InvalidOperationException(message: "CHARACTER_DETACHED");
        }

        _presentation?.Advance(elapsedSeconds: delta);
        AdvanceWork(elapsed: delta);
        if (_presentation is not null)
        {
            _ = _presentation.TryContact(generation: _presentation.Snapshot.GetProperty(propertyName: "generation")
                    .GetInt64());
            _ = _presentation.TryRelease(generation: _presentation.Snapshot.GetProperty(propertyName: "generation")
                    .GetInt64());
        }

        Project();
    }

    internal static CharacterPreview? TryOpenAttached(InspectionShell? shell, MainThreadGateway gateway,
            Func<string, byte[]?> read, string id, Node2D owner)
    {
        gateway.RequireThread();
        string resource = "content/animation/locomotion-melee.json";
        try
        {
            return OpenAttached(shell: shell, gateway: gateway, read: path => read(arg: resource = path), id: id);
        }
        catch (Exception error) when (error is InvalidDataException or JsonException or FormatException)
        {
            shell?.Retire();
            owner.GetNode<TextEdit>(path: "Shell/Workspace/Preview/Failures").Text += "\n" + id
                    + ": " + resource + " — " + error.Message;
            owner.GetNode<Label>(path: "Summary/Status").Text = "Error — " + id + "; choose Recheck resources";
            return null;
        }
    }

    internal static CharacterPreview OpenAttached(InspectionShell? shell, MainThreadGateway gateway,
            Func<string, byte[]?> read, string id)
    {
        gateway.RequireThread();
        if (shell is null)
        {
            throw new InvalidDataException(message: "CHARACTER_SHELL_MISSING");
        }

        var root = new Node2D { Name = "Character" };
        shell.Install(preview: root);
        CharacterPreview? result = null;
        try
        {
            result = Open(root: root, gateway: gateway, read: read, body: ObligationBody(id: id));
            result.Attach();
            if (HandlesObligation(id: id))
            {
                result.SelectObligation(id: id);
            }
        }
        catch (Exception error) when (error is InvalidDataException or JsonException or FormatException)
        {
            result?.Detach();
            throw;
        }

        return result;
    }

    internal static bool HandlesObligation(string id)
    {
        return id is "CH-09" or "AN-01" or "AN-02" or "AN-03" or "AN-04" or "AN-05" or "AN-06"
                or "AN-07" or "AN-08" or "AN-09" or "AN-10" or "AN-11" or "AN-12" or "AN-13"
                or "AN-14" or "AN-15" or "AN-16" or "AN-18";
    }

    internal static string ObligationBody(string id)
    {
        if (id is "CH-01" or "CH-02" or "CH-03" or "CH-04" or "CH-05" or "CH-06" or "CH-07" or "CH-08")
        {
            return id;
        }

        if (!HandlesObligation(id: id))
        {
            throw new InvalidDataException(message: "CHARACTER_OBLIGATION " + id);
        }

        return id switch { "AN-15" => "CH-06", "AN-16" => "CH-08", _ => "CH-01" };
    }

    internal void SelectObligation(string id)
    {
        _gateway.RequireThread();
        if (!_attached || !HandlesObligation(id: id))
        {
            throw new InvalidDataException(message: "CHARACTER_OBLIGATION " + id);
        }

        if (!StringComparer.Ordinal.Equals(x: _body, y: ObligationBody(id: id)))
        {
            throw new InvalidDataException(message: "CHARACTER_OBLIGATION_BODY " + id + " " + _body);
        }

        Reset();
        ObligationChoice(name: "WorkAction", value: "Empty");
        ObligationChoice(name: "Movement", value: id is "AN-02" ? "Walk" : id is "AN-03" ? "Jump" : "Idle");
        if (id is "AN-04" or "AN-05" or "AN-06")
        {
            ObligationChoice(name: "Action", value: "Melee");
            ObligationChoice(name: "left", value: id is "AN-04" ? "Empty" : "IT-03");
            ObligationChoice(name: "right", value: id is "AN-06" ? "IT-04" : "Empty");
        }

        if (id is "AN-07" or "AN-08" or "AN-18" or "CH-09")
        {
            ObligationChoice(name: "Action", value: "Bow");
            ObligationChoice(name: "left", value: "IT-07");
            ObligationChoice(name: "right", value: "IT-02");
            ObligationChoice(name: "Ammunition", value: id is "AN-08" ? "0" : "12");
            ObligationChoice(name: "BowSubject", value: id is "CH-09" or "AN-18" ? "Structural guard" : "Ordinary");
        }

        string work = id switch
        {
            "AN-10" => "Sit",
            "AN-11" => "Gather",
            "AN-12" => "Build",
            "AN-13" => "Pickup",
            "AN-14" => "Eat",
            "AN-15" => "Spit",
            "AN-16" => "Burst",
            _ => "Empty",
        };
        ObligationChoice(name: "WorkAction", value: work);
        if (id is "AN-09")
        {
            PreviewFlinch();
        }

        Project();
        _root.SetMeta(name: "obligation", value: id);
        _root.SetMeta(name: "activation", value: JsonSerializer.Serialize(value: new
        {
            id,
            body = _body,
            movement = Selected(id: "Movement"),
            action = Selected(id: "Action"),
            work = Selected(id: "WorkAction"),
            guard = Selected(id: "BowSubject"),
        }));
    }

    private readonly Node2D _root;
    private static readonly string[] Empty = ["Empty"];
    private readonly MainThreadGateway _gateway;
    private readonly JsonElement _fits;
    private readonly JsonElement _bodies;
    private readonly JsonElement[] _packs;
    private readonly Dictionary<string, OptionButton> _controls = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, Texture2D> _textures = new(comparer: StringComparer.Ordinal);
    private string _body;
    private bool _attached;
    private CharacterPresentation? _presentation;
    private SceneTree? _tree;
    private bool _playing;
    private readonly Dictionary<string, Button> _buttons = new(comparer: StringComparer.Ordinal);
    private readonly List<Texture2D> _ownedTextures = [];
    private JsonElement _sample;
    private string _equipment = "";
    private JsonElement _bow;
    private double _aim;
    private string _aimPreset = "0";
    private JsonElement _work;
    private JsonElement _workClip;
    private string _workSelection = "";
    private double _workElapsed;
    private long _workGeneration;
    private long _workEventCount;
    private long _workEventCycle = -1;
    private bool _workImpact;
    private bool _workOriginReady;
    private float _workOriginX;
    private float _workOriginY;
    private readonly List<string> _workEvents = [];
    private bool _composition;

    private void ObligationChoice(string name, string value)
    {
        _gateway.RequireThread();
        if (!_controls.TryGetValue(key: name, value: out OptionButton? control))
        {
            throw new InvalidDataException(message: "CHARACTER_OBLIGATION_CONTROL " + name);
        }

        for (int index = 0; index < control.ItemCount; index++)
        {
            if (StringComparer.Ordinal.Equals(x: control.GetItemText(idx: index), y: value))
            {
                control.Select(idx: index);
                return;
            }
        }

        throw new InvalidDataException(message: "CHARACTER_OBLIGATION_SELECTOR " + name + " " + value);
    }

    private static CharacterPresentation ReadPresentation(Func<string, byte[]?> read)
    {
        byte[] animation = read(arg: "content/animation/locomotion-melee.json")
                ?? throw new InvalidDataException(message: "ANIMATION_MISSING content/animation/locomotion-melee.json");
        JsonElement definition = ReadDefinition(bytes: animation, expectedId: "locomotion-melee");
        string[] expected = ["content/config/items.json", "content/config/combat.json", "content/animation/fits-a.json",
            "content/packs/bodies-a.json", "assets/characters/bodies-a.png", "content/animation/fits-b.json",
            "content/packs/bodies-b.json", "assets/characters/bodies-b.png", "content/animation/fits-c.json",
            "content/packs/bodies-c.json", "assets/characters/bodies-c.png"];
        if (!definition.GetProperty(propertyName: "bindings").EnumerateArray().Select(selector: binding =>
                ContentPack.ResourcePath(path: Text(value: binding, name: "path"))).SequenceEqual(second: expected,
                        comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "ANIMATION_BINDING_SET");
        }

        foreach (JsonElement binding in definition.GetProperty(propertyName: "bindings").EnumerateArray())
        {
            string path = ContentPack.ResourcePath(path: Text(value: binding, name: "path"));
            byte[] value = read(arg: path) ?? throw new InvalidDataException(message: "ANIMATION_BINDING " + path);
            if (value.Length is 0 or > 16777216 || !StringComparer.Ordinal.Equals(
                    x: Convert.ToHexStringLower(inArray: SHA256.HashData(source: value)),
                    y: Text(value: binding, name: "sha256")))
            {
                throw new InvalidDataException(message: "ANIMATION_BINDING_HASH " + path);
            }
        }

        byte[] bow = read(arg: "content/animation/bow.json")
                ?? throw new InvalidDataException(message: "BOW_MISSING content/animation/bow.json");
        JsonElement bowDefinition = ReadDefinition(bytes: bow, expectedId: "bow");
        string[] bowPaths = ["content/config/items.json", "content/config/combat.json", "assets/common/donor-kit.png"];
        if (!bowDefinition.GetProperty(propertyName: "bindings").EnumerateArray().Select(selector: binding =>
                ContentPack.ResourcePath(path: Text(value: binding, name: "path"))).SequenceEqual(
                        second: bowPaths, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "BOW_BINDING_SET");
        }

        foreach (JsonElement binding in bowDefinition.GetProperty(propertyName: "bindings").EnumerateArray())
        {
            string path = ContentPack.ResourcePath(path: Text(value: binding, name: "path"));
            byte[] value = read(arg: path) ?? throw new InvalidDataException(message: "BOW_BINDING " + path);
            if (value.Length is 0 or > 16777216 || !StringComparer.Ordinal.Equals(
                    x: Convert.ToHexStringLower(inArray: SHA256.HashData(source: value)),
                    y: Text(value: binding, name: "sha256")))
            {
                throw new InvalidDataException(message: "BOW_BINDING_HASH " + path);
            }
        }

        return new CharacterPresentation(animation: animation, items: read(arg: "content/config/items.json")
                ?? throw new InvalidDataException(message: "ANIMATION_ITEMS_MISSING"),
                combat: read(arg: "content/config/combat.json")
                        ?? throw new InvalidDataException(message: "ANIMATION_COMBAT_MISSING"), bow: bow);
    }

    private void AnimationControls(Node slots)
    {
        _gateway.RequireThread();
        AddSelector(parent: slots, id: "Movement", label: "Movement", values: ["Static pose", "Idle", "Walk",
            "Faster", "Jump"]);
        AddSelector(parent: slots, id: "Action", label: "Attack", values: ["None", "Melee", "Bow"]);
        AddSelector(parent: slots, id: "Aim", label: "Bow elevation", values: ["-90", "-60", "-30", "0", "30",
            "60", "90"]);
        _controls["Aim"].Select(idx: 3);
        var aimControl = new HSlider { Name = "ContinuousAim", MinValue = -90, MaxValue = 90, Step = 0.1,
            CustomMinimumSize = new Vector2(x: 220, y: 48) };
        aimControl.ValueChanged += AimChanged;
        slots.AddChild(node: aimControl);
        AddSelector(parent: slots, id: "Ammunition", label: "Actual opposite stack", values: ["0", "1", "12"]);
        _controls["Ammunition"].Select(idx: 1);
        AddSelector(parent: slots, id: "ArrowStack", label: "Selected stack identity", values: ["stack-a", "stack-b"]);
        AddSelector(parent: slots, id: "BowSubject", label: "Bow presentation",
                values: ["Ordinary", "Structural guard"]);
        WorkControls(slots: slots);
        AddSelector(parent: slots, id: "PlaybackSpeed", label: "Preview speed", values: ["0.25×", "0.5×", "1×", "2×"]);
        _controls["PlaybackSpeed"].Select(idx: 2);
        foreach ((string id, string text) in new[] { ("Play", "Play preview"), ("Step", "Step frame"),
            ("Reset", "Reset preview"), ("Flinch", "Preview visual flinch") })
        {
            var button = new Button { Name = id, Text = text, CustomMinimumSize = new Vector2(x: 220, y: 48) };
            button.Pressed += TransportAction(id: id);
            slots.AddChild(node: button);
            _buttons.Add(key: id, value: button);
        }
    }

    private Action TransportAction(string id)
    {
        return id switch
        {
            "Play" => Play,
            "Step" => Step,
            "Reset" => Reset,
            "Flinch" => PreviewFlinch,
            _ => throw new InvalidDataException(message: "ANIMATION_TRANSPORT")
        };
    }

    private void Play()
    {
        _gateway.RequireThread();
        _playing = !_playing;
        _buttons["Play"].Text = _playing ? "Pause preview" : "Play preview";
        _buttons["Step"].Disabled = _playing;
    }

    private void Step()
    {
        _gateway.RequireThread();
        if (!_playing)
        {
            Tick(delta: _presentation?.Snapshot.GetProperty(propertyName: "nextSampleSeconds").GetDouble() ?? 0);
        }
    }

    private void Reset()
    {
        _gateway.RequireThread();
        _playing = false;
        _buttons["Play"].Text = "Play preview";
        _buttons["Step"].Disabled = false;
        foreach (string id in new[] { "Movement", "Action", "left", "right", "Facing" })
        {
            _controls[id].Select(idx: id is "Movement" ? 1 : 0);
        }

        _presentation?.Configure(body: _body, movement: "idle", action: "none", leftItem: "Empty",
                rightItem: "Empty", facing: 1);
        CancelWork();
        Project();
    }

    private void PreviewFlinch()
    {
        _gateway.RequireThread();
        _presentation?.Flinch();
        Project();
    }

    private void Frame()
    {
        _gateway.RequireThread();
        if (_attached && _playing)
        {
            double[] speeds = [0.25, 0.5, 1, 2];
            Tick(delta: _root.GetProcessDeltaTime() * speeds[_controls["PlaybackSpeed"].Selected]);
        }
    }

    private void RetireArt()
    {
        _gateway.RequireThread();
        Node2D art = _root.GetNode<Node2D>(path: "Art");
        foreach (Node child in art.GetChildren())
        {
            art.RemoveChild(node: child);
            child.QueueFree();
        }

        foreach (Texture2D texture in _ownedTextures)
        {
            texture.Dispose();
        }

        _ownedTextures.Clear();
    }

    private static void ValidateFitting(JsonElement root)
    {
        foreach (JsonElement body in root.GetProperty(propertyName: "bodies").EnumerateArray())
        {
            JsonElement bodyWidth = body.GetProperty(propertyName: "width");
            if (bodyWidth.ValueKind is not JsonValueKind.Number || !bodyWidth.TryGetInt32(value: out int count)
                    || count is < 2 or > 16)
            {
                throw new InvalidDataException(message: "CHARACTER_FIT_WIDTH body");
            }

            if (body.TryGetProperty(propertyName: "componentWidths", value: out JsonElement widths))
            {
                foreach (JsonProperty width in widths.EnumerateObject())
                {
                    if (width.Value.ValueKind is not JsonValueKind.Number
                            || !width.Value.TryGetInt32(value: out int value) || value is < 1 or > 16)
                    {
                        throw new InvalidDataException(message: "CHARACTER_FIT_WIDTH " + width.Name);
                    }
                }
            }

            foreach (JsonElement pose in body.GetProperty(propertyName: "poses").EnumerateArray())
            {
                if (pose.TryGetProperty(propertyName: "unlinkedRotation", value: out JsonElement rotation))
                {
                    double angle = rotation.GetDouble();
                    double turns = angle / (Math.PI / 2);
                    if (!double.IsFinite(d: angle) || Math.Abs(value: turns) > 2
                            || Math.Abs(value: turns - Math.Round(a: turns)) > 0.000001)
                    {
                        throw new InvalidDataException(message: "CHARACTER_FIT_ROTATION "
                                + Text(value: body, name: "id") + " " + Text(value: pose, name: "id"));
                    }
                }
            }
        }
    }

    private static JsonElement ReadProfile(Func<string, byte[]?> read, string profile)
    {
        JsonElement fits = ReadDefinition(bytes: read(arg: "content/animation/fits-" + profile + ".json")
                ?? throw new InvalidDataException(message: "CHARACTER_FITS_MISSING " + profile),
                expectedId: "fits-" + profile);
        JsonElement bodies = ReadDefinition(bytes: read(arg: "content/packs/bodies-" + profile + ".json")
                ?? throw new InvalidDataException(message: "CHARACTER_BODIES_MISSING " + profile),
                expectedId: "bodies-" + profile);
        string atlas = "res://assets/characters/bodies-" + profile + ".png";
        if (!StringComparer.Ordinal.Equals(x: Text(value: bodies.GetProperty(propertyName: "atlas"),
                name: "path"), y: atlas))
        {
            throw new InvalidDataException(message: "CHARACTER_PROFILE_ATLAS " + profile);
        }

        string[] expected = profile is "a" ? ["CH-01", "CH-02", "CH-03"]
                : profile is "b" ? ["CH-04", "CH-05", "CH-08"] : ["CH-06", "CH-07"];
        string[] actual = bodies.GetProperty(propertyName: "bodies").EnumerateArray()
                .Select(selector: value => Text(value: value, name: "id")).ToArray();
        string[] fitted = fits.GetProperty(propertyName: "bodies").EnumerateArray()
                .Select(selector: value => Text(value: value, name: "id")).ToArray();
        if (!actual.SequenceEqual(second: expected, comparer: StringComparer.Ordinal)
                || !fitted.SequenceEqual(second: expected, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "CHARACTER_PROFILE_BODIES " + profile);
        }

        return JsonSerializer.SerializeToElement(value: new { fits, bodies, atlas });
    }

    private static JsonElement MergeFits(JsonElement[] profiles)
    {
        JsonElement first = profiles[0].GetProperty(propertyName: "fits");
        return JsonSerializer.SerializeToElement(value: new
        {
            bodies = profiles.SelectMany(selector: value => value.GetProperty(propertyName: "fits")
                    .GetProperty(propertyName: "bodies").EnumerateArray()).OrderBy(
                            keySelector: value => Text(value: value, name: "id"), comparer: StringComparer.Ordinal),
            slots = first.GetProperty(propertyName: "slots"),
            items = first.GetProperty(propertyName: "items"),
        });
    }

    private static JsonElement MergeBodies(JsonElement[] profiles)
    {
        return JsonSerializer.SerializeToElement(value: new
        {
            parts = profiles.SelectMany(selector: value => value.GetProperty(propertyName: "bodies")
                    .GetProperty(propertyName: "parts").EnumerateArray()),
            atlases = profiles.SelectMany(selector: profile => profile.GetProperty(propertyName: "bodies")
                    .GetProperty(propertyName: "bodies").EnumerateArray().Select(selector: body =>
                            new KeyValuePair<string, string>(key: Text(value: body, name: "id"),
                                    value: Text(value: profile, name: "atlas"))))
                    .ToDictionary(keySelector: value => value.Key, elementSelector: value => value.Value,
                            comparer: StringComparer.Ordinal),
        });
    }

    private int BodyChoice(Node slots)
    {
        _gateway.RequireThread();
        string[] bodies = _fits.GetProperty(propertyName: "bodies").EnumerateArray()
                .Select(selector: value => Text(value: value, name: "id")).ToArray();
        int selected = Array.FindIndex(array: bodies, match: value => StringComparer.Ordinal.Equals(
                x: value, y: _body));
        if (selected < 0 || bodies.Distinct(comparer: StringComparer.Ordinal).Count() != bodies.Length)
        {
            throw new InvalidDataException(message: "CHARACTER_BODY_SELECTION " + _body);
        }

        AddSelector(parent: slots, id: "Body", label: "Body", values: bodies);
        return selected;
    }

    private static string Text(JsonElement value, string name)
    {
        return value.GetProperty(propertyName: name).GetString()
                ?? throw new InvalidDataException(message: "CHARACTER_TEXT " + name);
    }

    private static void ValidateValue(JsonElement value)
    {
        if (value.ValueKind is JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                if (!keys.Add(item: property.Name))
                {
                    throw new InvalidDataException(message: "CHARACTER_DUPLICATE_KEY");
                }

                ValidateValue(value: property.Value);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            foreach (JsonElement element in value.EnumerateArray())
            {
                ValidateValue(value: element);
            }
        }
        else if (value.ValueKind is JsonValueKind.Number && !double.IsFinite(d: value.GetDouble()))
        {
            throw new InvalidDataException(message: "CHARACTER_NONFINITE");
        }
    }

    private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)
    {
        _gateway.RequireThread();
        var row = new VBoxContainer { Name = id };
        parent.AddChild(node: row);
        row.AddChild(node: new Label { Name = "Label", Text = label });
        var control = new OptionButton { Name = "Select", CustomMinimumSize = new Vector2(x: 220, y: 48) };
        row.AddChild(node: control);
        foreach (string value in values)
        {
            control.AddItem(label: value);
        }

        control.ItemSelected += Changed;
        _controls.Add(key: id, value: control);
    }

    private void Changed(long index)
    {
        _gateway.RequireThread();
        ArgumentOutOfRangeException.ThrowIfNegative(value: index);
        if (!_attached)
        {
            throw new InvalidOperationException(message: "CHARACTER_DETACHED");
        }

        Project();
    }

    private void AimChanged(double value)
    {
        _gateway.RequireThread();
        if (!_attached)
        {
            return;
        }

        _aim = value * Math.PI / 180;
        Project();
    }

    private string Selected(string id)
    {
        _gateway.RequireThread();
        OptionButton control = _controls[id];
        return control.GetItemText(idx: control.Selected);
    }

    private void Project()
    {
        _gateway.RequireThread();
        RetireArt();
        Node2D art = _root.GetNode<Node2D>(path: "Art");
        _body = Selected(id: "Body");
        ReconcileHands();
        ReconcileEquipment();
        string poseId = Selected(id: "Pose");
        int facing = Selected(id: "Facing") is "Right" ? 1 : -1;
        int zoom = 1 << _controls["Zoom"].Selected;
        art.Scale = new Vector2(x: zoom, y: zoom);
        JsonElement body = _fits.GetProperty(propertyName: "bodies").EnumerateArray().First(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: _body));
        JsonElement pose = body.GetProperty(propertyName: "poses").EnumerateArray().First(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: poseId));
        (pose, JsonElement lower) = Compose(body: body, pose: pose, facing: facing);
        (pose, lower) = WorkPose(body: body, pose: pose, lower: lower);

        if (_sample.ValueKind is JsonValueKind.Object && Selected(id: "Action") is "Bow"
                && Selected(id: "BowSubject") is "Structural guard")
        {
            AddGuard(art: art, facing: facing);
            _root.SetMeta(name: "sample", value: _sample.GetRawText());
            _root.GetNode<Label>(path: "Status").Text = "Structural guard · built-in bow · no loot";
            return;
        }

        AddBody(art: art, pose: pose, facing: facing, lower: lower);
        ProjectGear(art: art, body: body, pose: pose, lower: lower, facing: facing, poseId: poseId);
        Contact(art: art, pose: pose, facing: facing);
        ProjectWork(art: art, body: body, pose: pose, facing: facing);
        if (_sample.ValueKind is JsonValueKind.Object)
        {
            _root.SetMeta(name: "sample", value: _sample.GetRawText());
        }
    }

    private (JsonElement Upper, JsonElement Lower) Compose(JsonElement body, JsonElement pose, int facing)
    {
        JsonElement lower = pose;
        _sample = default;
        if (_presentation is not null && (Selected(id: "Movement") is not "Static pose"
                || Selected(id: "Action") is "Melee" or "Bow"))
        {
            string movement = Selected(id: "Movement") switch
            {
                "Walk" => "walk",
                "Faster" => "faster",
                "Jump" when _body is "CH-01" => "jump",
                _ => "idle"
            };
            string action = Selected(id: "Action") switch { "Melee" => "melee", "Bow" => "bow", _ => "none" };
            bool guard = action is "bow" && Selected(id: "BowSubject") is "Structural guard";
            bool rightBow = Selected(id: "right") is "IT-07" && Selected(id: "left") is not "IT-07";
            _presentation.Configure(body: _body, movement: movement, action: action,
                    leftItem: guard ? rightBow ? "Empty" : "IT-07" : Selected(id: "left"),
                    rightItem: guard ? rightBow ? "IT-07" : "Empty" : Selected(id: "right"), facing: facing);
            UpdateAim(guard: guard);
            _sample = _presentation.Snapshot;
            lower = FindPose(body: body, id: Text(value: _sample, name: "lowerPose"));
            pose = FindPose(body: body, id: Text(value: _sample, name: "upperPose"));
            if (action is "bow" && Text(value: _sample, name: "bowStage") is not "inactive")
            {
                pose = BowPose(pose: pose);
            }
        }

        return (pose, lower);
    }

    private static JsonElement ReadWork(Func<string, byte[]?> read)
    {
        JsonElement work = ReadDefinition(bytes: read(arg: "content/animation/work-special.json")
                ?? throw new InvalidDataException(message: "WORK_MISSING"), expectedId: "work-special");
        JsonElement atlas = work.GetProperty(propertyName: "atlas");
        string path = ContentPack.ResourcePath(path: Text(value: atlas, name: "path"));
        if (path is not "assets/characters/work-special.png")
        {
            throw new InvalidDataException(message: "WORK_ATLAS_PATH");
        }

        byte[] pixels = read(arg: path) ?? throw new InvalidDataException(message: "WORK_ATLAS_MISSING");
        if (pixels.Length is 0 or > 16777216 || !StringComparer.Ordinal.Equals(
                x: Convert.ToHexStringLower(inArray: SHA256.HashData(source: pixels)),
                y: Text(value: atlas, name: "sha256")))
        {
            throw new InvalidDataException(message: "WORK_ATLAS_HASH");
        }

        if (Text(value: work, name: "clock") is not "explicit-elapsed-only"
                || atlas.GetProperty(propertyName: "width").GetInt32() != 128
                || atlas.GetProperty(propertyName: "height").GetInt32() != 64
                || work.GetProperty(propertyName: "burst").GetProperty(propertyName: "damage").GetInt32() != 0
                || work.GetProperty(propertyName: "burst").GetProperty(propertyName: "slowdown").GetInt32() != 0
                || work.GetProperty(propertyName: "burst").GetProperty(propertyName: "blocksMovement").GetBoolean()
                || !work.GetProperty(propertyName: "burst").GetProperty(propertyName: "corpseIndependent").GetBoolean())
        {
            throw new InvalidDataException(message: "WORK_CONTRACT");
        }

        ValidateWork(work: work);
        return work;
    }

    private static void ValidateWork(JsonElement work)
    {
        string[] names = ["Sit", "Stand", "Gather", "Put away", "Build", "Hammer", "Repair", "Chop", "Pickup",
            "Carry", "Deliver", "Stash", "Equip", "Eat", "Spit", "Burst"];
        if (!work.GetProperty(propertyName: "actions").EnumerateArray().Select(selector: clip =>
                Text(value: clip, name: "id")).SequenceEqual(second: names, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "WORK_ACTIONS");
        }

        foreach (JsonElement clip in work.GetProperty(propertyName: "actions").EnumerateArray())
        {
            ValidateWorkClip(clip: clip);
        }

        var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement part in work.GetProperty(propertyName: "components").EnumerateArray())
        {
            JsonElement rect = part.GetProperty(propertyName: "atlasRect");
            if (!keys.Add(item: Text(value: part, name: "key")) || rect.GetArrayLength() != 4
                    || rect[0].GetInt32() < 0 || rect[1].GetInt32() < 0 || rect[2].GetInt32() is < 1 or > 32
                    || rect[3].GetInt32() is < 1 or > 32 || rect[0].GetInt32() + rect[2].GetInt32() > 128
                    || rect[1].GetInt32() + rect[3].GetInt32() > 64)
            {
                throw new InvalidDataException(message: "WORK_COMPONENT");
            }
        }

        JsonElement spit = work.GetProperty(propertyName: "spit");
        string[] expected = ["hammer", "spit.0", "spit.1", "spit.2", "spit.3", "splash.0", "splash.1", "splash.2",
            "splash.3", "stain.0", "stain.1", "stain.2", "stain.3", "axe.0", "axe.1", "axe.2", "axe.3"];
        if (!keys.SetEquals(other: expected) || spit.GetProperty(propertyName: "speedUnitsPerSecond").GetDouble() != 8
                || spit.GetProperty(propertyName: "intervalSeconds").GetDouble() != 3
                || spit.GetProperty(propertyName: "lifetimeSeconds").GetDouble() != 3
                || !spit.GetProperty(propertyName: "stopAtFirstEligibleHit").GetBoolean()
                || spit.GetProperty(propertyName: "areaDamage").GetBoolean()
                || spit.GetProperty(propertyName: "lingeringEffect").GetBoolean())
        {
            throw new InvalidDataException(message: "WORK_SPIT");
        }
    }

    private static void ValidateWorkClip(JsonElement clip)
    {
        JsonElement samples = clip.GetProperty(propertyName: "samples");
        string body = Text(value: clip, name: "id") is "Spit" ? "CH-06"
                : Text(value: clip, name: "id") is "Burst" ? "CH-08" : "CH-01";
        if (!StringComparer.Ordinal.Equals(x: Text(value: clip, name: "body"), y: body)
                || samples.GetArrayLength() is < 1 or > 8 || samples.EnumerateArray().Any(predicate: sample =>
                        sample.GetProperty(propertyName: "duration").GetDouble() is < 0.05 or > 6
                                || Math.Abs(value: sample.GetProperty(propertyName: "angle").GetDouble()) > Math.PI
                                || Text(value: sample, name: "pose") is not ("idle" or "sit" or "gather" or "carry"
                                        or "recover" or "windup" or "strike" or "eat" or "aim" or "hit"
                                        or "collapse-a" or "collapse-b" or "corpse"))
                || Math.Abs(value: samples.EnumerateArray().Sum(selector: sample => sample.GetProperty(
                        propertyName: "duration").GetDouble()) - clip.GetProperty(propertyName: "duration")
                                .GetDouble()) > 0.000000001)
        {
            throw new InvalidDataException(message: "WORK_TIMING");
        }

        JsonElement effect = clip.GetProperty(propertyName: "event");
        if (effect.ValueKind is JsonValueKind.Object && (effect.GetProperty(propertyName: "time").GetDouble() < 0
                || effect.GetProperty(propertyName: "time").GetDouble() > clip.GetProperty(
                        propertyName: "duration").GetDouble() || Text(value: effect, name: "sound")
                                is not ("AU-08" or "AU-12" or "AU-13" or "AU-18" or "AU-19" or "AU-29" or "AU-30")))
        {
            throw new InvalidDataException(message: "WORK_EVENT");
        }
    }

    private void WorkControls(Node slots)
    {
        _gateway.RequireThread();
        if (_work.ValueKind is not JsonValueKind.Object)
        {
            return;
        }

        AddSelector(parent: slots, id: "WorkAction", label: "Work / specialist pose", values: Empty.Concat(
                second: _work.GetProperty(propertyName: "actions").EnumerateArray().Select(
                        selector: clip => Text(value: clip, name: "id"))));
        AddSelector(parent: slots, id: "WorkEvents", label: "Preview event behavior",
                values: ["Silent restore", "Trigger events"]);
        AddSelector(parent: slots, id: "WorkRestore", label: "Restore sample", values: ["Start", "Middle", "End"]);
        AddSelector(parent: slots, id: "WorkHand", label: "Work tool hand", values: ["Left", "Right"]);
        AddSelector(parent: slots, id: "WorkTransfer", label: "Actual sample transfer",
                values: ["Empty", "Plant", "Carried", "Delivered", "Equipped"]);
        AddSelector(parent: slots, id: "SpitContact", label: "Projectile sample path",
                values: ["Clear", "First contact"]);
    }

    private void CancelWork()
    {
        _gateway.RequireThread();
        _workClip = default;
        _workSelection = "";
        _workElapsed = 0;
        _workEventCount = 0;
        _workEventCycle = -1;
        _workImpact = false;
        _workOriginReady = false;
        _workOriginX = default;
        _workOriginY = default;
        _workEvents.Clear();
        _workGeneration = checked(_workGeneration + 1);
        if (_controls.TryGetValue(key: "WorkAction", value: out OptionButton? control))
        {
            control.Select(idx: 0);
        }
    }

    private (JsonElement Upper, JsonElement Lower) WorkPose(JsonElement body, JsonElement pose, JsonElement lower)
    {
        _gateway.RequireThread();
        if (_work.ValueKind is not JsonValueKind.Object || Selected(id: "WorkAction") is "Empty")
        {
            if (_workClip.ValueKind is JsonValueKind.Object || _workSelection.Length > 0)
            {
                CancelWork();
            }

            return (pose, lower);
        }

        if (Selected(id: "Action") is not "None")
        {
            CancelWork();
            return (pose, lower);
        }

        JsonElement clip = _work.GetProperty(propertyName: "actions").EnumerateArray().Single(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"),
                        y: Selected(id: "WorkAction")));
        if (!StringComparer.Ordinal.Equals(x: Text(value: clip, name: "body"), y: _body))
        {
            _workClip = default;
            _workSelection = "";
            _root.SetMeta(name: "work", value: JsonSerializer.Serialize(value: new
            {
                available = false,
                requiredBody = Text(value: clip, name: "body"),
            }));
            return (pose, lower);
        }

        string selection = _body + "|" + Selected(id: "WorkAction") + "|" + Selected(id: "WorkEvents") + "|"
                + Selected(id: "WorkRestore");
        if (!StringComparer.Ordinal.Equals(x: selection, y: _workSelection))
        {
            _workClip = clip;
            _workSelection = selection;
            _workElapsed = clip.GetProperty(propertyName: "duration").GetDouble() * (Selected(id: "WorkRestore")
                    is "Middle" ? 0.5 : Selected(id: "WorkRestore") is "End" ? 0.999999 : 0);
            _workGeneration = checked(_workGeneration + 1);
            _workEventCount = 0;
            _workEventCycle = -1;
            _workImpact = false;
            _workOriginReady = false;
            _workEvents.Clear();
            WorkEvent();
        }

        string id = Text(value: WorkSample(), name: "pose");
        pose = FindPose(body: body, id: id);
        if (Selected(id: "WorkAction") is "Sit" or "Stand" or "Burst")
        {
            lower = pose;
        }

        return (pose, lower);
    }

    private JsonElement WorkSample()
    {
        double time = _workElapsed;
        JsonElement samples = _workClip.GetProperty(propertyName: "samples");
        foreach (JsonElement sample in samples.EnumerateArray())
        {
            time -= sample.GetProperty(propertyName: "duration").GetDouble();
            if (time < 0)
            {
                return sample;
            }
        }

        return samples[samples.GetArrayLength() - 1];
    }

    private void AdvanceWork(double elapsed)
    {
        if (_workClip.ValueKind is not JsonValueKind.Object)
        {
            return;
        }

        double duration = _workClip.GetProperty(propertyName: "duration").GetDouble();
        if (Text(value: _workClip, name: "id") is "Spit")
        {
            duration += 0.6;
        }

        double next = _workElapsed + elapsed;
        if (_workClip.GetProperty(propertyName: "loop").GetBoolean() && next >= duration)
        {
            _workGeneration = checked(_workGeneration + (long)Math.Floor(d: next / duration));
            _workEventCycle = -1;
            next %= duration;
        }

        _workElapsed = Math.Min(val1: next, val2: duration);
        WorkEvent();
        if (next >= duration && !_workClip.GetProperty(propertyName: "persistent").GetBoolean()
                && !_workClip.GetProperty(propertyName: "loop").GetBoolean())
        {
            CancelWork();
        }
    }

    private void WorkEvent()
    {
        if (Text(value: _workClip, name: "id") is "Eat" && Selected(id: "WorkTransfer") is not "Carried")
        {
            return;
        }

        JsonElement row = _workClip.GetProperty(propertyName: "event");
        if (Selected(id: "WorkEvents") is "Trigger events" && row.ValueKind is JsonValueKind.Object
                && _workElapsed >= row.GetProperty(propertyName: "time").GetDouble()
                && _workEventCycle != _workGeneration)
        {
            RecordWorkEvent(sound: Text(value: row, name: "sound"));
            _workEventCycle = _workGeneration;
        }
    }

    private void RecordWorkEvent(string sound)
    {
        if (_workEvents.Count >= 16)
        {
            _workEvents.RemoveAt(index: 0);
        }

        _workEvents.Add(item: sound);
        _workEventCount = checked(_workEventCount + 1);
    }

    private void ProjectWork(Node2D art, JsonElement body, JsonElement pose, int facing)
    {
        _gateway.RequireThread();
        if (_workClip.ValueKind is not JsonValueKind.Object)
        {
            _root.SetMeta(name: "work", value: JsonSerializer.Serialize(value: new
            {
                available = false,
                action = "Empty",
                elapsed = _workElapsed,
                generation = _workGeneration,
                eventCount = _workEventCount,
                events = _workEvents,
                firstContact = _workImpact,
            }));
            return;
        }

        string id = Text(value: _workClip, name: "id");
        WorkEvent();
        WorkTool(art: art, pose: pose, facing: facing);
        WorkPayload(art: art, pose: pose, facing: facing);
        if (id is "Spit")
        {
            ProjectSpit(art: art, body: body, facing: facing);
        }

        if (id is "Burst")
        {
            ProjectBurst(art: art, facing: facing);
        }

        if (id is "Eat" && Selected(id: "WorkEvents") is "Trigger events" && _workElapsed < 0.2
                && Selected(id: "WorkTransfer") is "Carried")
        {
            art.AddChild(node: new Label { Name = "FoodFeedback", Text = "Food used", Position = new Vector2(
                    x: 8, y: -8), Scale = new Vector2(x: 0.25f, y: 0.25f) });
        }

        _root.SetMeta(name: "work", value: JsonSerializer.Serialize(value: new
        {
            available = true,
            action = id,
            body = _body,
            pose = Text(value: WorkSample(), name: "pose"),
            elapsed = _workElapsed,
            generation = _workGeneration,
            eventCount = _workEventCount,
            events = _workEvents,
            firstContact = _workImpact,
            inventoryMutation = false,
            toolInventoryItem = false,
            freeCapacity = false,
            gameplayEvents = 0,
            acidDamage = 0,
            corpseIndependent = true,
            transfer = Selected(id: "WorkTransfer"),
        }));
    }

    private Sprite2D WorkSprite(Node2D art, string name, string key, Vector2 at, int facing, float angle = 0)
    {
        _gateway.RequireThread();
        JsonElement part = _work.GetProperty(propertyName: "components").EnumerateArray().Single(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "key"), y: key));
        var sprite = new Sprite2D
        {
            Name = name,
            Texture = Texture(pack: _work, part: part, atlas: "assets/characters/work-special.png"),
            Centered = false,
            Offset = -Point(value: part.GetProperty(propertyName: "pivot")),
            Position = new Vector2(x: facing == 1 ? at.X : 48 - at.X, y: at.Y),
            Scale = new Vector2(x: facing, y: 1),
            Rotation = facing * angle,
            ZIndex = 35
        };
        sprite.SetMeta(name: "part", value: key);
        sprite.SetMeta(name: "gameplay", value: false);
        sprite.SetMeta(name: "recoverable", value: false);
        art.AddChild(node: sprite);
        return sprite;
    }

    private void WorkTool(Node2D art, JsonElement pose, int facing)
    {
        _gateway.RequireThread();
        JsonElement tool = _workClip.GetProperty(propertyName: "tool");
        if (tool.ValueKind is not JsonValueKind.String)
        {
            return;
        }

        string hand = Selected(id: "WorkHand") is "Left" ? "left" : "right";
        Vector2 at = Point(value: pose.GetProperty(propertyName: "anchors").GetProperty(propertyName: hand + "Hand"))
                + SegmentOffset(lower: false);
        string key = tool.GetString() is "axe" ? "axe.0" : "hammer";
        Sprite2D sprite = WorkSprite(art: art, name: "WorkTool", key: key, at: at, facing: facing,
                angle: WorkSample().GetProperty(propertyName: "angle").GetSingle());
        sprite.SetMeta(name: "logical_hand", value: hand);
        sprite.SetMeta(name: "inventory_item", value: false);
    }

    private void WorkPayload(Node2D art, JsonElement pose, int facing)
    {
        _gateway.RequireThread();
        string transfer = Selected(id: "WorkTransfer");
        string id = Text(value: _workClip, name: "id");
        if (transfer is not ("Plant" or "Carried") || id is "Eat" or "Spit" or "Burst")
        {
            return;
        }

        JsonElement pack = _packs[0];
        JsonElement part = pack.GetProperty(propertyName: "parts").EnumerateArray().Single(
                predicate: value => Text(value: value, name: "key") is "IT_08_master");
        Vector2 at = transfer is "Plant" ? new Vector2(x: 38, y: 29)
                : Point(value: pose.GetProperty(propertyName: "anchors").GetProperty(propertyName: "rightHand"))
                        + SegmentOffset(lower: false);
        var payload = new Sprite2D { Name = "WorkPayload", Texture = Texture(pack: pack, part: part,
                atlas: "assets/equipment/items-01-10.png"), Position = new Vector2(x: facing == 1 ? at.X : 48 - at.X,
                        y: at.Y), Scale = new Vector2(x: facing, y: 1), ZIndex = 34 };
        payload.SetMeta(name: "item", value: "IT-08");
        payload.SetMeta(name: "sample_transfer", value: transfer);
        art.AddChild(node: payload);
    }

    private void ProjectSpit(Node2D art, JsonElement body, int facing)
    {
        _gateway.RequireThread();
        double time = _workElapsed - 0.6;
        if (time < 0)
        {
            return;
        }

        if (!_workOriginReady)
        {
            JsonElement release = FindPose(body: body, id: "strike");
            Vector2 origin = Point(value: release.GetProperty(propertyName: "anchors")
                    .GetProperty(propertyName: "face"))
                    + new Vector2(x: 1, y: 1) + SegmentOffset(lower: false);
            _workOriginX = origin.X;
            _workOriginY = origin.Y;
            _workOriginReady = true;
        }

        var capturedOrigin = new Vector2(x: _workOriginX, y: _workOriginY);
        bool contact = Selected(id: "SpitContact") is "First contact" && time >= 0.5;
        if (contact && !_workImpact)
        {
            _workImpact = true;
            if (Selected(id: "WorkEvents") is "Trigger events")
            {
                RecordWorkEvent(sound: "AU-08");
            }
        }

        if (!contact && !_workImpact && time < 3)
        {
            Sprite2D sprite = WorkSprite(art: art, name: "SpitProjectile", key: "spit."
                    + ((int)(time / 0.1) % 4).ToString(provider: CultureInfo.InvariantCulture),
                    at: capturedOrigin + new Vector2(x: (float)time * 16, y: 0), facing: facing);
            sprite.SetMeta(name: "mouth", value: capturedOrigin);
            sprite.SetMeta(name: "first_hit_only", value: true);
        }
        else if (_workImpact && time is >= 0.5 and < 0.9)
        {
            _ = WorkSprite(art: art, name: "SpitImpact", key: "splash."
                    + Math.Min(val1: 3, val2: (int)((time - 0.5) / 0.1)).ToString(
                            provider: CultureInfo.InvariantCulture), at: capturedOrigin + new Vector2(x: 8, y: 0),
                    facing: facing);
        }
    }

    private void ProjectBurst(Node2D art, int facing)
    {
        _gateway.RequireThread();
        double time = _workElapsed - 0.1;
        if (time is >= 0 and < 0.4)
        {
            _ = WorkSprite(art: art, name: "BloaterBurst", key: "splash."
                    + Math.Min(val1: 3, val2: (int)(time / 0.1)).ToString(provider: CultureInfo.InvariantCulture),
                    at: new Vector2(x: 24, y: 29), facing: facing);
        }

        if (time is >= 0 and < 1.5)
        {
            _ = WorkSprite(art: art, name: "CosmeticStain", key: "stain."
                    + Math.Min(val1: 3, val2: (int)(time / 0.375)).ToString(provider: CultureInfo.InvariantCulture),
                    at: new Vector2(x: 24, y: 38), facing: facing);
        }
    }

    private void UpdateAim(bool guard)
    {
        _gateway.RequireThread();
        if (!StringComparer.Ordinal.Equals(x: _aimPreset, y: Selected(id: "Aim")))
        {
            _aimPreset = Selected(id: "Aim");
            _aim = double.Parse(s: _aimPreset, provider: CultureInfo.InvariantCulture) * Math.PI / 180;
            _root.GetNode<HSlider>(path: "Controls/Slots/ContinuousAim").SetValueNoSignal(value: _aim * 180 / Math.PI);
        }

        int arrows = int.Parse(s: Selected(id: "Ammunition"), provider: CultureInfo.InvariantCulture);
        _presentation?.ConfigureBow(aimRadians: _aim,
                leftArrows: !guard && Selected(id: "left") is "IT-02" ? arrows : 0,
                rightArrows: !guard && Selected(id: "right") is "IT-02" ? arrows : 0,
                leftUsable: guard || Selected(id: "Condition") is "Usable",
                rightUsable: guard || Selected(id: "Condition") is "Usable",
                stackIdentity: Selected(id: "ArrowStack"), intrinsicAmmunition: guard);
    }

    private JsonElement BowPose(JsonElement pose)
    {
        _gateway.RequireThread();
        JsonNode candidate = JsonNode.Parse(json: pose.GetRawText())
                ?? throw new InvalidDataException(message: "BOW_POSE");
        JsonNode anchors = candidate["anchors"] ?? throw new InvalidDataException(message: "BOW_ANCHORS");
        double angle = _sample.GetProperty(propertyName: "aimRadians").GetDouble();
        string bowHand = Text(value: _sample, name: "bowHand");
        foreach (string hand in new[] { "left", "right" })
        {
            Vector2 shoulder = Point(value: pose.GetProperty(propertyName: "anchors")
                    .GetProperty(propertyName: hand + "Shoulder"));
            double reach = StringComparer.Ordinal.Equals(x: hand, y: bowHand) ? 11 : 5;
            double fraction = _sample.GetProperty(propertyName: "actionSeconds").GetDouble()
                    / _sample.GetProperty(propertyName: "intervalSeconds").GetDouble();
            double prepare = _bow.GetProperty(propertyName: "prepareFraction").GetDouble();
            reach *= Math.Min(val1: 1, val2: 0.6 + fraction / prepare * 0.4);
            if (Text(value: _sample, name: "bowStage") is "recovery")
            {
                reach *= 1 - (fraction - _bow.GetProperty(propertyName: "releaseFraction").GetDouble()) * 0.5;
            }
            anchors[hand + "Hand"] = new JsonArray(shoulder.X + Math.Cos(d: angle) * reach,
                    shoulder.Y + Math.Sin(a: angle) * reach);
        }

        return JsonSerializer.SerializeToElement(value: candidate);
    }

    private void AddGuard(Node2D art, int facing)
    {
        _gateway.RequireThread();
        JsonElement guard = _bow.GetProperty(propertyName: "guard");
        JsonElement composition = guard.GetProperty(propertyName: "composition");
        JsonElement frame = guard.GetProperty(propertyName: "frames").EnumerateArray().Single(predicate: value =>
                StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"),
                        y: Text(value: composition, name: "sourceFrame")));
        using Image source = Texture(pack: _bow, part: frame, atlas: "assets/common/donor-kit.png").GetImage();
        using Image upper = source.Duplicate() as Image ?? throw new InvalidDataException(message: "GUARD_IMAGE");
        foreach (JsonElement cutout in composition.GetProperty(propertyName: "upperCutouts").EnumerateArray())
        {
            upper.FillRect(rect: new Rect2I(x: cutout[0].GetInt32(), y: cutout[1].GetInt32(),
                    width: cutout[2].GetInt32(), height: cutout[3].GetInt32()), color: Colors.Transparent);
        }

        int split = composition.GetProperty(propertyName: "lowerLogicalY").GetInt32();
        int top = frame.GetProperty(propertyName: "offsetTopLeft")[1].GetInt32();
        upper.FillRect(rect: new Rect2I(x: 0, y: split - top, width: upper.GetWidth(),
                height: upper.GetHeight() - split + top), color: Colors.Transparent);
        Vector2 origin = new Vector2(x: 24, y: 8) + Point(value: frame.GetProperty(propertyName: "offsetTopLeft"));
        Sprite2D torso = GuardLayer(art: art, name: "StructuralGuard", image: upper, at: origin,
                facing: facing, angle: 0, offset: Vector2.Zero);
        torso.SetMeta(name: "source", value: Text(value: frame, name: "id"));
        torso.SetMeta(name: "loot", value: false);
        GuardLower(art: art, guard: guard, split: split, facing: facing);
        GuardArms(art: art, source: source, composition: composition, origin: origin, facing: facing);
    }

    private Sprite2D GuardLayer(Node2D art, string name, Image image, Vector2 at, int facing, float angle,
            Vector2 offset)
    {
        _gateway.RequireThread();
        ImageTexture texture = ImageTexture.CreateFromImage(image: image);
        _ownedTextures.Add(item: texture);
        var sprite = new Sprite2D { Name = name, Texture = texture, Centered = false, Offset = offset,
            Position = new Vector2(x: facing == 1 ? at.X : 48 - at.X, y: at.Y),
            Scale = new Vector2(x: facing, y: 1), Rotation = facing * angle,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
        art.AddChild(node: sprite);
        return sprite;
    }

    private void GuardLower(Node2D art, JsonElement guard, int split, int facing)
    {
        _gateway.RequireThread();
        string movement = Text(value: _sample, name: "movement");
        JsonElement clip = guard.GetProperty(propertyName: "locomotion").EnumerateArray().Single(predicate: value =>
                StringComparer.Ordinal.Equals(x: Text(value: value, name: "movement"),
                        y: movement is "faster" or "walk" ? movement : "idle"));
        double seconds = _sample.GetProperty(propertyName: "strideSeconds").GetDouble()
                / clip.GetProperty(propertyName: "stridePeriod").GetDouble()
                * clip.GetProperty(propertyName: "duration").GetDouble();
        JsonElement frame = clip.GetProperty(propertyName: "frames").EnumerateArray().Last(predicate: value =>
                value.GetProperty(propertyName: "seconds").GetDouble() <= seconds);
        using Image source = Texture(pack: _bow, part: frame, atlas: "assets/common/donor-kit.png").GetImage();
        Vector2 offset = Point(value: frame.GetProperty(propertyName: "offsetTopLeft"));
        int y = split - (int)MathF.Round(x: offset.Y);
        using Image lower = source.GetRegion(region: new Rect2I(x: 0, y: y, width: source.GetWidth(),
                height: source.GetHeight() - y));
        Sprite2D sprite = GuardLayer(art: art, name: "GuardLower", image: lower,
                at: new Vector2(x: 24 + offset.X, y: 8 + offset.Y + y), facing: facing, angle: 0,
                offset: Vector2.Zero);
        sprite.SetMeta(name: "source", value: Text(value: frame, name: "id"));
    }

    private void GuardArms(Node2D art, Image source, JsonElement composition, Vector2 origin, int facing)
    {
        _gateway.RequireThread();
        float angle = _sample.GetProperty(propertyName: "aimRadians").GetSingle();
        JsonElement rect = composition.GetProperty(propertyName: "armRect");
        string bowHand = Text(value: _sample, name: "bowHand");
        Vector2 bowAt = origin;
        foreach (string hand in new[] { "left", "right" })
        {
            Vector2 shoulder = Point(value: composition.GetProperty(propertyName: "shoulders")
                    .GetProperty(propertyName: hand));
            double fraction = _sample.GetProperty(propertyName: "actionSeconds").GetDouble()
                    / _sample.GetProperty(propertyName: "intervalSeconds").GetDouble();
            double scale = Math.Min(val1: 1, val2: 0.6 + fraction
                    / _bow.GetProperty(propertyName: "prepareFraction").GetDouble() * 0.4);
            if (Text(value: _sample, name: "bowStage") is "recovery")
            {
                scale *= 1 - (fraction - _bow.GetProperty(propertyName: "releaseFraction").GetDouble()) * 0.5;
            }

            int reach = Math.Max(val1: 1, val2: (int)Math.Round(a:
                    (StringComparer.Ordinal.Equals(x: hand, y: bowHand) ? 7 : 3) * scale));
            using Image arm = source.GetRegion(region: new Rect2I(x: rect[0].GetInt32(), y: rect[1].GetInt32(),
                    width: rect[2].GetInt32(), height: rect[3].GetInt32()));
            arm.Resize(width: reach, height: 2, interpolation: Image.Interpolation.Nearest);
            _ = GuardLayer(art: art, name: "GuardArm_" + hand, image: arm, at: origin + shoulder,
                    facing: facing, angle: angle, offset: new Vector2(x: 0, y: -1));
            if (StringComparer.Ordinal.Equals(x: hand, y: bowHand))
            {
                bowAt = origin + shoulder + new Vector2(x: MathF.Cos(x: angle), y: MathF.Sin(x: angle)) * reach;
            }
        }

        rect = composition.GetProperty(propertyName: "bowRect");
        using Image bow = source.GetRegion(region: new Rect2I(x: rect[0].GetInt32(), y: rect[1].GetInt32(),
                width: rect[2].GetInt32(), height: rect[3].GetInt32()));
        foreach (JsonElement row in composition.GetProperty(propertyName: "clearBowRows").EnumerateArray())
        {
            bow.FillRect(rect: new Rect2I(x: 0, y: row.GetInt32(), width: bow.GetWidth(), height: 1),
                    color: Colors.Transparent);
        }

        _ = GuardLayer(art: art, name: "GuardBow", image: bow, at: bowAt, facing: facing, angle: angle,
                offset: -Point(value: composition.GetProperty(propertyName: "bowGrip")));
        if (_sample.GetProperty(propertyName: "nocked").GetBoolean()
                || _sample.GetProperty(propertyName: "released").GetBoolean())
        {
            GuardArrow(art: art, at: bowAt, facing: facing, angle: angle);
        }
    }

    private void GuardArrow(Node2D art, Vector2 at, int facing, float angle)
    {
        _gateway.RequireThread();
        JsonElement part = _packs[0].GetProperty(propertyName: "parts").EnumerateArray().Single(
                predicate: value => Text(value: value, name: "key") is "IT_02_master");
        using Image arrow = Texture(pack: _packs[0], part: part, atlas: "assets/equipment/items-01-10.png")
                .GetImage();
        bool released = _sample.GetProperty(propertyName: "released").GetBoolean();
        if (released)
        {
            float fraction = _bow.GetProperty(propertyName: "releaseFraction").GetSingle();
            float seconds = _sample.GetProperty(propertyName: "actionSeconds").GetSingle()
                    - _sample.GetProperty(propertyName: "intervalSeconds").GetSingle() * fraction;
            at += new Vector2(x: MathF.Cos(x: angle), y: MathF.Sin(x: angle)) * Math.Clamp(value: seconds * 100,
                    min: 0, max: 36);
        }

        Sprite2D sprite = GuardLayer(art: art, name: released ? "SpentArrow" : "GuardArrow", image: arrow,
                at: at, facing: facing, angle: angle, offset: new Vector2(x: 0, y: -1));
        sprite.SetMeta(name: "recoverable", value: false);
        sprite.SetMeta(name: "gameplay", value: false);
    }

    private void EndAction()
    {
        CancelWork();
        if (_presentation is not null)
        {
            JsonElement sample = _presentation.Snapshot;
            _presentation.Configure(body: Text(value: sample, name: "body"),
                    movement: Text(value: sample, name: "movement"), action: "none",
                    leftItem: Text(value: sample, name: "leftItem"), rightItem: Text(value: sample, name: "rightItem"),
                    facing: sample.GetProperty(propertyName: "facing").GetInt32());
        }
    }

    private void ReconcileEquipment()
    {
        _gateway.RequireThread();
        string equipment = string.Join(separator: "|", values: _fits.GetProperty(propertyName: "slots")
                .EnumerateArray().Select(selector: slot => Selected(id: Text(value: slot, name: "id"))))
                + "|" + Selected(id: "Condition");
        if (_equipment.Length > 0 && !StringComparer.Ordinal.Equals(x: equipment, y: _equipment))
        {
            EndAction();
        }

        _equipment = equipment;
    }

    private void Contact(Node2D art, JsonElement pose, int facing)
    {
        _gateway.RequireThread();
        if (_sample.ValueKind is JsonValueKind.Object
                && _sample.GetProperty(propertyName: "contactVisible").GetBoolean())
        {
            Vector2 at = Point(value: pose.GetProperty(propertyName: "anchors")
                    .GetProperty(propertyName: "rightHand"));
            var contact = new Polygon2D
            {
                Name = "Contact",
                Polygon = [new Vector2(x: 0, y: 0), new Vector2(x: 2, y: 0), new Vector2(x: 2, y: 2),
                    new Vector2(x: 0, y: 2)],
                Position = new Vector2(x: facing == 1 ? at.X : 48 - at.X, y: at.Y)
                        + SegmentOffset(lower: false) * new Vector2(x: facing, y: 1),
                Color = new Color(r: 1, g: 0.95f, b: 0.8f),
                ZIndex = 30
            };
            art.AddChild(node: contact);
        }
    }

    private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,
            string poseId)
    {
        _gateway.RequireThread();
        var diagnostics = new List<string>();
        foreach (JsonElement slot in _fits.GetProperty(propertyName: "slots").EnumerateArray())
        {
            string id = Text(value: slot, name: "id");
            string item = Selected(id: id);
            bool emptyArrows = item is "IT-02" && _sample.ValueKind is JsonValueKind.Object
                    && Text(value: _sample, name: "action") is "bow"
                    && (!_sample.GetProperty(propertyName: "nocked").GetBoolean());
            if (item is not "Empty" && !emptyArrows)
            {
                JsonElement fittedPose = id is "legs" or "boots" ? lower : pose;
                AddItem(art: art, body: body, pose: fittedPose, slot: id, itemId: item, facing: facing,
                        diagnostics: diagnostics);
            }
        }

        if (_sample.ValueKind is JsonValueKind.Object && _sample.GetProperty(propertyName: "released").GetBoolean())
        {
            AddSpentArrow(art: art, pose: pose, facing: facing);
        }

        string state = diagnostics.Count == 0 ? "Fitted — visual acceptance pending" : "Incomplete — "
                + string.Join(separator: "; ", values: diagnostics);
        _root.GetNode<Label>(path: "Status").Text = _body + " · " + poseId + " · " + state;
        _root.SetMeta(name: "body", value: _body);
        _root.SetMeta(name: "pose", value: poseId);
        _root.SetMeta(name: "facing", value: facing);
        _root.SetMeta(name: "diagnostics", value: string.Join(separator: "\n", values: diagnostics));
        if (_sample.ValueKind is JsonValueKind.Object)
        {
            _root.SetMeta(name: "sample", value: _sample.GetRawText());
            _root.GetNode<Label>(path: "Status").Text += " · " + Text(value: _sample, name: "stage") + " · t="
                    + _sample.GetProperty(propertyName: "actionSeconds").GetDouble().ToString(format: "F3",
                            provider: CultureInfo.InvariantCulture);
        }
    }

    private void AddSpentArrow(Node2D art, JsonElement pose, int facing)
    {
        _gateway.RequireThread();
        JsonElement item = _fits.GetProperty(propertyName: "items").EnumerateArray().Single(
                predicate: value => Text(value: value, name: "id") is "IT-02");
        JsonElement definition = _packs[0].GetProperty(propertyName: "parts").EnumerateArray().Single(
                predicate: value => Text(value: value, name: "key") is "IT_02_master");
        Vector2 at = Point(value: pose.GetProperty(propertyName: "anchors").GetProperty(
                propertyName: Text(value: _sample, name: "bowHand") + "Hand"));
        float angle = _sample.GetProperty(propertyName: "aimRadians").GetSingle();
        float time = _sample.GetProperty(propertyName: "actionSeconds").GetSingle()
                - _sample.GetProperty(propertyName: "intervalSeconds").GetSingle() * 0.5f;
        at += new Vector2(x: MathF.Cos(x: angle), y: MathF.Sin(x: angle)) * Math.Clamp(value: time * 100,
                min: 0, max: 36) + SegmentOffset(lower: false);
        var sprite = new Sprite2D
        {
            Name = "SpentArrow", Texture = Texture(pack: _packs[0], part: definition,
                    atlas: Text(value: item, name: "atlas")), Position = new Vector2(
                            x: facing == 1 ? at.X : 48 - at.X, y: at.Y), Rotation = facing * angle,
            Scale = new Vector2(x: facing, y: 1), ZIndex = 31,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };
        sprite.SetMeta(name: "recoverable", value: false);
        sprite.SetMeta(name: "gameplay", value: false);
        art.AddChild(node: sprite);
    }

    private static JsonElement FindPose(JsonElement body, string id)
    {
        return body.GetProperty(propertyName: "poses").EnumerateArray().First(predicate: value =>
                StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: id));
    }

    private Vector2 SegmentOffset(bool lower)
    {
        _gateway.RequireThread();
        if (_sample.ValueKind is not JsonValueKind.Object)
        {
            return Vector2.Zero;
        }

        Vector2 offset = Point(value: _sample.GetProperty(propertyName: "lowerOffset"));
        if (!lower)
        {
            offset += Point(value: _sample.GetProperty(propertyName: "upperOffset"));
            offset.X += _sample.GetProperty(propertyName: "flinchOffset").GetSingle();
            offset.Y += _sample.GetProperty(propertyName: "transitionOffset").GetSingle();
        }

        return offset;
    }

    private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)
    {
        _gateway.RequireThread();
        foreach (string part in new[] { "lower", "upper", "head" })
        {
            JsonElement segment = part is "lower" ? lower : pose;
            string key = Text(value: segment.GetProperty(propertyName: "bodyParts"), name: part);
            JsonElement definition = _bodies.GetProperty(propertyName: "parts").EnumerateArray()
                    .First(predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "key"),
                            y: key));
            var sprite = new Sprite2D
            {
                Name = "Body_" + part,
                Centered = false,
                Texture = Texture(pack: _bodies, part: definition, atlas: BodyAtlas()),
                Position = new Vector2(x: facing == 1 ? 0 : 48, y: 0)
                        + SegmentOffset(lower: part is "lower") * new Vector2(x: facing, y: 1),
                Scale = new Vector2(x: facing, y: 1),
                ZIndex = 10,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest
            };
            sprite.SetMeta(name: "part", value: key);
            art.AddChild(node: sprite);
        }

        if (BowActive())
        {
            AddBowArms(art: art, pose: pose, facing: facing);
        }
    }

    private bool BowActive()
    {
        return _sample.ValueKind is JsonValueKind.Object && Text(value: _sample, name: "action") is "bow"
                && Text(value: _sample, name: "bowStage") is not "inactive";
    }

    private void AddBowArms(Node2D art, JsonElement pose, int facing)
    {
        _gateway.RequireThread();
        Sprite2D torso = art.GetNode<Sprite2D>(path: "Body_upper");
        using Image normal = torso.Texture.GetImage();
        JsonElement anchors = pose.GetProperty(propertyName: "anchors");
        JsonElement composition = _bow.GetProperty(propertyName: "bodyComposition").EnumerateArray().Single(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: _body));
        foreach (JsonElement rect in composition.GetProperty(propertyName: "retiredArms").EnumerateArray())
        {
            normal.FillRect(rect: new Rect2I(x: rect[0].GetInt32(), y: rect[1].GetInt32(),
                    width: rect[2].GetInt32(), height: rect[3].GetInt32()), color: Colors.Transparent);
        }

        ImageTexture fitted = ImageTexture.CreateFromImage(image: normal);
        _ownedTextures.Add(item: fitted);
        torso.Texture = fitted;
        JsonElement body = _fits.GetProperty(propertyName: "bodies").EnumerateArray().First(predicate: value =>
                StringComparer.Ordinal.Equals(x: Text(value: value, name: "id"), y: _body));
        JsonElement aim = FindPose(body: body, id: "aim");
        JsonElement part = _bodies.GetProperty(propertyName: "parts").EnumerateArray().First(predicate: value =>
                StringComparer.Ordinal.Equals(x: Text(value: value, name: "key"),
                        y: Text(value: aim.GetProperty(propertyName: "bodyParts"), name: "upper")));
        using Image source = Texture(pack: _bodies, part: part, atlas: BodyAtlas()).GetImage();
        foreach (string hand in new[] { "left", "right" })
        {
            Vector2 shoulder = Point(value: anchors.GetProperty(propertyName: hand + "Shoulder"));
            Vector2 grip = Point(value: anchors.GetProperty(propertyName: hand + "Hand"));
            JsonElement rect = composition.GetProperty(propertyName: "armRect");
            using Image strip = source.GetRegion(region: new Rect2I(x: rect[0].GetInt32(), y: rect[1].GetInt32(),
                    width: rect[2].GetInt32(), height: rect[3].GetInt32()));
            strip.Resize(width: Math.Max(val1: 1, val2: (int)MathF.Round(x: (grip - shoulder).Length())), height: 3,
                    interpolation: Image.Interpolation.Nearest);
            ImageTexture texture = ImageTexture.CreateFromImage(image: strip);
            _ownedTextures.Add(item: texture);
            var arm = new Sprite2D { Name = "BowArm_" + hand, Texture = texture, Centered = false,
                Offset = new Vector2(x: 0, y: -1), Position = new Vector2(x: facing == 1 ? shoulder.X
                        : 48 - shoulder.X, y: shoulder.Y) + SegmentOffset(lower: false)
                                * new Vector2(x: facing, y: 1), Scale = new Vector2(x: facing, y: 1),
                Rotation = facing * (grip - shoulder).Angle(), ZIndex = hand is "left" ? 9 : 11,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
            arm.SetMeta(name: "logical_hand", value: hand);
            art.AddChild(node: arm);
        }
    }

    private string BodyAtlas()
    {
        _gateway.RequireThread();
        string atlas = _bodies.TryGetProperty(propertyName: "atlases", value: out JsonElement atlases)
                ? Text(value: atlases, name: _body)
                : Text(value: _bodies.GetProperty(propertyName: "atlas"), name: "path");
        string[] allowed = ["res://assets/characters/bodies-a.png", "res://assets/characters/bodies-b.png",
            "res://assets/characters/bodies-c.png"];
        if (!allowed.Contains(value: atlas, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "CHARACTER_PROFILE_ATLAS " + _body);
        }

        return atlas[6..];
    }

    private void AddItem(Node2D art, JsonElement body, JsonElement pose, string slot, string itemId, int facing,
            List<string> diagnostics)
    {
        _gateway.RequireThread();
        JsonElement item;
        try
        {
            item = FitItem(fits: _fits, body: _body, item: itemId, pose: Text(value: pose, name: "id"),
                    facing: facing);
        }
        catch (InvalidDataException error)
        {
            diagnostics.Add(item: error.Message);
            return;
        }

        foreach (JsonElement layer in item.GetProperty(propertyName: "layers").EnumerateArray())
        {
            string surface = Text(value: layer, name: "surface");
            if (surface is "both" || StringComparer.Ordinal.Equals(x: surface,
                    y: facing == 1 ? "front" : "rear"))
            {
                AddLayer(art: art, body: body, pose: pose, item: item, layer: layer, slot: slot, facing: facing);
            }
        }
    }

    private void ReconcileHands()
    {
        _gateway.RequireThread();
        foreach (string slot in new[] { "left", "right" })
        {
            string opposite = Selected(id: slot is "left" ? "right" : "left");
            OptionButton control = _controls[slot];
            for (int index = 0; index < control.ItemCount; index++)
            {
                string item = control.GetItemText(idx: index);
                bool disabled = item is not "Empty" && (opposite is "IT-07" && item is not "IT-02"
                        || item is "IT-07" && opposite is not ("Empty" or "IT-02"));
                control.SetItemDisabled(idx: index, disabled: disabled);
                control.SetItemTooltip(idx: index, tooltip: disabled
                        ? "Bow permits only arrows in the other hand" : "");
            }
        }
    }

    private void AddLayer(Node2D art, JsonElement body, JsonElement pose, JsonElement item, JsonElement layer,
            string slot, int facing)
    {
        _gateway.RequireThread();
        string anchor = Text(value: layer, name: "anchor");
        anchor = anchor is "hand" ? slot + "Hand" : anchor;
        JsonElement anchors = pose.GetProperty(propertyName: "anchors");
        Vector2 at = Point(value: anchors.GetProperty(propertyName: anchor));
        (Vector2 size, float angle) = FitGeometry(body: body, pose: pose, layer: layer, slot: slot,
                anchor: anchor, anchors: anchors, at: at);
        at += SegmentOffset(lower: slot is "legs" or "boots");
        if (_sample.ValueKind is JsonValueKind.Object && slot is "left" or "right")
        {
            angle += _sample.GetProperty(propertyName: slot + "Angle").GetSingle();
        }

        if (BowActive() && slot is "gloves")
        {
            angle = _sample.GetProperty(propertyName: "aimRadians").GetSingle();
        }
        JsonElement pack = _packs.First(predicate: value => value.GetProperty(propertyName: "items")
                .EnumerateArray().Any(predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value,
                        name: "id"), y: Text(value: item, name: "id"))));
        string key = Text(value: layer, name: "part");
        bool broken = !_composition && Selected(id: "Condition") is "Broken"
                && item.GetProperty(propertyName: "broken").ValueKind is JsonValueKind.String;
        if (broken && (slot is "left" or "right" or "headwear" || anchor is "torso"))
        {
            key = Text(value: item, name: "broken");
        }

        JsonElement definition = pack.GetProperty(propertyName: "parts").EnumerateArray().First(
                predicate: value => StringComparer.Ordinal.Equals(x: Text(value: value, name: "key"), y: key));
        AddFittedSprite(art: art, item: item, layer: layer, slot: slot, facing: facing, at: at, size: size,
                angle: angle, pack: pack, definition: definition, key: key);
    }

    private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,
            string slot, string anchor, JsonElement anchors, Vector2 at)
    {
        _gateway.RequireThread();
        Vector2 size = Point(value: layer.GetProperty(propertyName: "size"));
        float angle = 0;
        if (body.TryGetProperty(propertyName: "componentWidths", value: out JsonElement widths)
                && widths.TryGetProperty(propertyName: Text(value: layer, name: "component"),
                        value: out JsonElement width))
        {
            size.X = width.GetInt32();
        }

        if (layer.GetProperty(propertyName: "end").ValueKind is JsonValueKind.String)
        {
            Vector2 delta = Point(value: anchors.GetProperty(propertyName: Text(value: layer, name: "end"))) - at;
            size.Y = Math.Max(val1: 1, val2: MathF.Round(x: delta.Length()));
            angle = delta.Angle() - MathF.PI / 2;
        }
        else if (anchor is "torso")
        {
            size.X = body.GetProperty(propertyName: "width").GetInt32() + (slot is "armor" ? 2 : 0);
            size.Y = pose.GetProperty(propertyName: "torsoLength").GetInt32();
            angle = (Point(value: anchors.GetProperty(propertyName: "hips")) - at).Angle() - MathF.PI / 2;
        }
        else if (slot is not ("left" or "right") && pose.TryGetProperty(propertyName: "unlinkedRotation",
                value: out JsonElement rotation))
        {
            angle = rotation.GetSingle();
        }

        return (size, angle);
    }

    private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,
            Vector2 at, Vector2 size, float angle, JsonElement pack, JsonElement definition, string key)
    {
        _gateway.RequireThread();
        Texture2D source = Texture(pack: pack, part: definition, atlas: Text(value: item, name: "atlas"));
        using Image image = source.GetImage();
        image.Resize(width: (int)size.X, height: (int)size.Y, interpolation: Image.Interpolation.Nearest);
        bool hand = slot is "left" or "right";
        Vector2 pivot = hand ? Point(value: layer.GetProperty(propertyName: "pivot"))
                : new Vector2(x: size.X / 2, y: slot is "headwear" or "face" ? size.Y / 2 : 0);
        ImageTexture fitted = ImageTexture.CreateFromImage(image: image);
        _ownedTextures.Add(item: fitted);
        var sprite = new Sprite2D
        {
            Name = slot + "_" + Text(value: layer, name: "component"),
            Centered = false,
            Texture = fitted,
            Offset = -pivot.Round(),
            Position = new Vector2(x: facing == 1 ? at.X : 48 - at.X, y: at.Y),
            Scale = new Vector2(x: facing, y: 1),
            Rotation = facing * angle,
            ZIndex = LayerOrder(layer: layer, slot: slot, facing: facing),
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };
        sprite.SetMeta(name: "slot", value: slot);
        sprite.SetMeta(name: "item", value: Text(value: item, name: "id"));
        sprite.SetMeta(name: "part", value: key);
        sprite.SetMeta(name: "logical_hand", value: hand ? slot : "none");
        art.AddChild(node: sprite);
    }

    private int LayerOrder(JsonElement layer, string slot, int facing)
    {
        _gateway.RequireThread();
        string side = slot is "left" or "right" ? slot : Text(value: layer, name: "side");
        bool far = side is "left" or "right" && !StringComparer.Ordinal.Equals(x: side,
                y: facing == 1 ? "right" : "left");
        return far && (slot is "left" or "right" or "gloves" || Text(value: layer,
                name: "component").StartsWith(value: "sleeve", comparisonType: StringComparison.Ordinal))
                ? 8 : layer.GetProperty(propertyName: "z").GetInt32();
    }

    private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)
    {
        _gateway.RequireThread();
        _ = pack;
        if (!_textures.TryGetValue(key: atlas, value: out Texture2D? texture))
        {
            texture = ResourceLoader.Load<Texture2D>(path: "res://" + atlas)
                    ?? throw new InvalidDataException(message: "CHARACTER_ATLAS_MISSING " + atlas);
            _textures.Add(key: atlas, value: texture);
        }

        JsonElement rect = part.GetProperty(propertyName: "atlasRect");
        int x = rect[0].GetInt32();
        int y = rect[1].GetInt32();
        int width = rect[2].GetInt32();
        int height = rect[3].GetInt32();
        if (x < 0 || y < 0 || width <= 0 || height <= 0 || x + width > texture.GetWidth()
                || y + height > texture.GetHeight())
        {
            throw new InvalidDataException(message: "CHARACTER_ATLAS_BOUNDS");
        }

        var result = new AtlasTexture
        {
            Atlas = texture,
            Region = new Rect2(x: x, y: y, width: width, height: height),
            FilterClip = true
        };
        _ownedTextures.Add(item: result);
        return result;
    }

    private Vector2 Point(JsonElement value)
    {
        _gateway.RequireThread();
        return new Vector2(x: value[0].GetSingle(), y: value[1].GetSingle());
    }
}
