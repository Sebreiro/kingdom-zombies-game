using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Projects session inspection resources without running session transactions.</summary>
internal sealed class SessionLayouts
{
    internal SessionLayouts(Control root, MainThreadGateway gateway, byte[] pack, byte[][] inputs)
    {
        _root = root;
        _gateway = gateway;
        JsonElement definition = Read(bytes: pack);
        _settings = Same(left: Text(value: definition, key: "id"), right: "ui-settings");
        if (_settings)
        {
            BindSettings(pack: definition, inputs: inputs);
        }
        else
        {
            Bind(pack: definition, inputs: inputs);
        }
        _data = Read(bytes: inputs[0]);
        Require(condition: Same(left: Text(value: _data, key: "id"),
                right: _settings ? "settings-errors" : "session-save-outcomes")
                && _data.GetProperty(propertyName: "sampleDataOnly").GetBoolean(), code: "SESSION_DATA_SCOPE");
        Unique(rows: _data.GetProperty(propertyName: "samples"));
        foreach (JsonElement sample in _data.GetProperty(propertyName: "samples").EnumerateArray())
        {
            Validate(state: sample);
            Require(condition: _settings == (Text(value: sample, key: "surface") is "settings" or "diagnostics"),
                    code: "SESSION_PACK_SURFACE");
        }
    }

    /// <summary>Returns the authored scope statement for a session departure example.</summary>
    internal static string Departure(bool host)
    {
        return host ? "End session for everyone?" : "Leave session? Other players can continue.";
    }

    /// <summary>Parses a bounded strict detached document before any native attachment.</summary>
    internal static JsonElement Read(byte[] bytes)
    {
        Require(condition: bytes.Length is > 0 and <= 1048576 && bytes[0] != 239,
                code: "SESSION_DOCUMENT_SIZE");
        string text = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: bytes);
        using var document = JsonDocument.Parse(json: text, options: new JsonDocumentOptions { MaxDepth = 20 });
        int count = 0;
        Check(value: document.RootElement, count: ref count);
        return document.RootElement.Clone();
    }

    /// <summary>Selects an authored visual example without running gameplay or filesystem transactions.</summary>
    internal void Open(string id)
    {
        _gateway.RequireThread();
        JsonElement state = Find(rows: _data.GetProperty(propertyName: "samples"), id: id);
        Project(snapshot: JsonSerializer.SerializeToUtf8Bytes(value: state), generation: checked(_generation + 1));
    }

    /// <summary>Validates the entire new candidate before replacing this owner's native controls.</summary>
    internal void Project(byte[] snapshot, long generation)
    {
        _gateway.RequireThread();
        JsonElement state = Read(bytes: snapshot);
        Validate(state: state);
        Require(condition: _settings == (Text(value: state, key: "surface") is "settings" or "diagnostics"),
                code: "SESSION_PACK_SURFACE");
        RequireActive(generation: generation);
        Clear();
        _generation = generation;
        if (_settings)
        {
            ProjectSettings(state: state);
            return;
        }
        VBoxContainer content = Attach(state: state);
        switch (Text(value: state, key: "surface"))
        {
            case "main": Main(content: content); break;
            case "solo": Setup(state: state, content: content, host: false); break;
            case "host": Setup(state: state, content: content, host: true); break;
            case "browser": Browser(state: state, content: content); break;
            case "manual": Manual(state: state, content: content); break;
            case "lobby": Lobby(state: state, content: content); break;
            case "join": Join(state: state, content: content); break;
            case "saves": Saves(state: state, content: content); break;
            case "host-exit": Exit(state: state, content: content, host: true); break;
            case "guest-leave": Exit(state: state, content: content, host: false); break;
            case "victory": Outcome(state: state, content: content, victory: true); break;
            case "defeat": Outcome(state: state, content: content, victory: false); break;
        }
        Diagnostics(state: state, content: content);
    }

    /// <summary>Closes acceptance before retiring only this presenter's owned native nodes.</summary>
    internal void Close()
    {
        _gateway.RequireThread();
        _accepting = false;
        _generation = checked(_generation + 1);
        Clear();
    }

    /// <summary>Inspects a supported authored canvas and UI scale inside the existing hub viewport.</summary>
    internal void Endpoint(int width, int height, int percent, bool fit)
    {
        _gateway.RequireThread();
        if (!_accepting || !_settings || !_root.IsInsideTree() || !_root.HasNode(path: "Settings"))
        {
            throw new InvalidOperationException(message: "SETTINGS_NOT_ACTIVE");
        }
        Require(condition: ((width == 1280 && height == 720) || (width == 1920 && height == 1080))
                && percent is 80 or 100 or 150, code: "SETTINGS_ENDPOINT");
        var viewport = _root.GetNode<ScrollContainer>(path: "Settings/Frame/Viewport");
        var extent = viewport.GetNode<Control>(path: "Extent");
        var canvas = extent.GetNode<Control>(path: "Canvas");
        float scale = percent / 100f;
        Vector2 available = viewport.Size.X > 0 && viewport.Size.Y > 0 ? viewport.Size
                : _root.Size - new Vector2(x: 0, y: 112);
        float zoom = fit ? Math.Min(val1: available.X / width, val2: available.Y / height) : 1;
        canvas.Size = new Vector2(x: width, y: height) / scale;
        canvas.Scale = Vector2.One * scale * zoom;
        extent.CustomMinimumSize = new Vector2(x: width, y: height) * zoom;
        extent.Size = extent.CustomMinimumSize;
        canvas.SetMeta(name: "canvas", value: new Vector2I(x: width, y: height));
        canvas.SetMeta(name: "ui_percent", value: percent);
        canvas.SetMeta(name: "inspection_zoom", value: zoom);
        canvas.SetMeta(name: "fit", value: fit);
        viewport.ScrollHorizontal = 0;
        viewport.ScrollVertical = 0;
        _root.GetNode<Label>(path: "Settings/Frame/Endpoint").Text = width + " × " + height + " / " + percent
                + "% UI · " + (fit ? "Fit to window" : "1:1 inspection — pan locally");
    }

    private static readonly string[] _paths = ["content/ui/session-save-outcomes.json",
        "assets/ui/InspectionTheme.tres", "assets/ui/LiberationSans.ttf", "assets/ui/PerfectDOSVGA437.ttf",
        "assets/ui/Fallback.ttf", "assets/common/donor-kit.png"];
    private static readonly string[] _settingsPaths = ["content/ui/settings-errors.json",
        "assets/ui/InspectionTheme.tres", "assets/ui/LiberationSans.ttf", "assets/ui/PerfectDOSVGA437.ttf",
        "assets/ui/Fallback.ttf", "assets/common/donor-kit.png"];
    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly JsonElement _data;
    private readonly bool _settings;
    private readonly List<Node> _owned = [];
    private bool _accepting = true;
    private long _generation;

    private static void Require(bool condition, string code)
    {
        if (!condition)
        {
            throw new InvalidDataException(message: code);
        }
    }

    private static string Text(JsonElement value, string key)
    {
        Require(condition: value.TryGetProperty(propertyName: key, value: out JsonElement field)
                && field.ValueKind == JsonValueKind.String, code: "SESSION_TEXT " + key);
        string text = field.GetString() ?? string.Empty;
        Require(condition: text.Length is > 0 and <= 4096 && !text.Any(predicate: char.IsControl),
                code: "SESSION_TEXT " + key);
        return text;
    }

    private static int Number(JsonElement value, string key, int minimum, int maximum)
    {
        Require(condition: value.TryGetProperty(propertyName: key, value: out JsonElement field)
                && field.TryGetInt32(value: out int _), code: "SESSION_NUMBER " + key);
        int number = field.GetInt32();
        Require(condition: number >= minimum && number <= maximum, code: "SESSION_RANGE " + key);
        return number;
    }

    private static bool Same(string left, string right)
    {
        return StringComparer.Ordinal.Equals(x: left, y: right);
    }

    private static void Check(JsonElement value, ref int count)
    {
        Require(condition: ++count <= 30000, code: "SESSION_DOCUMENT_COUNT");
        if (value.ValueKind == JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                Require(condition: keys.Add(item: property.Name), code: "SESSION_DUPLICATE_KEY " + property.Name);
                Check(value: property.Value, count: ref count);
            }
        }
        else if (value.ValueKind == JsonValueKind.Array)
        {
            Require(condition: value.GetArrayLength() <= 256, code: "SESSION_ARRAY_COUNT");
            foreach (JsonElement element in value.EnumerateArray())
            {
                Check(value: element, count: ref count);
            }
        }
        else if (value.ValueKind == JsonValueKind.String)
        {
            string text = value.GetString() ?? string.Empty;
            Require(condition: text.Length <= 4096 && !text.Any(predicate: char.IsControl), code: "SESSION_STRING");
        }
        else if (value.ValueKind == JsonValueKind.Number)
        {
            Require(condition: value.TryGetDouble(value: out double number) && double.IsFinite(d: number),
                    code: "SESSION_FINITE");
        }
    }

    private static void Unique(JsonElement rows)
    {
        Require(condition: rows.ValueKind == JsonValueKind.Array && rows.GetArrayLength() > 0,
                code: "SESSION_EMPTY_SAMPLES");
        var ids = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement row in rows.EnumerateArray())
        {
            Require(condition: ids.Add(item: Text(value: row, key: "id")), code: "SESSION_DUPLICATE_ID");
        }
    }

    private static JsonElement Find(JsonElement rows, string id)
    {
        foreach (JsonElement row in rows.EnumerateArray())
        {
            if (Same(left: Text(value: row, key: "id"), right: id))
            {
                return row;
            }
        }
        throw new InvalidDataException(message: "SESSION_ID " + id);
    }

    private static void Bind(JsonElement pack, byte[][] inputs)
    {
        Require(condition: Same(left: Text(value: pack, key: "id"), right: "ui-session")
                && pack.GetProperty(propertyName: "sampleDataOnly").GetBoolean()
                && !pack.GetProperty(propertyName: "gameplayTransactions").GetBoolean(), code: "SESSION_PACK_SCOPE");
        JsonElement bindings = pack.GetProperty(propertyName: "bindings");
        Require(condition: bindings.GetArrayLength() == _paths.Length && inputs.Length == _paths.Length,
                code: "SESSION_BINDING_COUNT");
        for (int index = 0; index < _paths.Length; index++)
        {
            Require(condition: Same(left: Text(value: bindings[index], key: "path"), right: _paths[index]),
                    code: "SESSION_BINDING_PATH " + _paths[index]);
            int maximum = Same(left: _paths[index], right: "assets/ui/InspectionTheme.tres") ? 2097152 : 1048576;
            Require(condition: inputs[index].Length > 0 && inputs[index].Length <= maximum,
                    code: "SESSION_BINDING_SIZE " + _paths[index]);
            string digest = Convert.ToHexStringLower(inArray: SHA256.HashData(source: inputs[index]));
            Require(condition: Same(left: digest, right: Text(value: bindings[index], key: "sha256")),
                    code: "SESSION_BINDING_HASH " + _paths[index]);
        }
    }

    private static void Validate(JsonElement state)
    {
        _ = Text(value: state, key: "id");
        _ = Text(value: state, key: "status");
        _ = Text(value: state, key: "title");
        string surface = Text(value: state, key: "surface");
        Require(condition: surface is "main" or "solo" or "host" or "browser" or "manual" or "lobby" or "join"
                or "saves" or "host-exit" or "guest-leave" or "victory" or "defeat" or "settings" or "diagnostics",
                code: "SESSION_SURFACE");
        switch (surface)
        {
            case "solo": case "host": case "lobby": ValidateRun(state: state); break;
            case "browser": ValidateBrowser(state: state); break;
            case "manual": _ = Text(value: state, key: "validation"); break;
            case "join": ValidateJoin(state: state); break;
            case "saves": ValidateSaves(state: state); break;
            case "host-exit": case "guest-leave": _ = Text(value: state, key: "reason"); break;
            case "victory": case "defeat": ValidateOutcome(state: state); break;
            case "settings": case "diagnostics": ValidateSettings(state: state); break;
        }
        ValidateDiagnostics(state: state);
    }

    private static void ValidateRun(JsonElement state)
    {
        foreach (string key in new[] { "runId", "runName", "host", "address", "compatibility" })
        {
            _ = Text(value: state, key: key);
        }
        _ = Number(value: state, key: "participants", minimum: 1, maximum: 4);
        Require(condition: Number(value: state, key: "capacity", minimum: 4, maximum: 4) == 4,
                code: "SESSION_CAPACITY");
        _ = state.GetProperty(propertyName: "inProgress").GetBoolean();
    }

    private static void ValidateBrowser(JsonElement state)
    {
        var ids = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement server in state.GetProperty(propertyName: "servers").EnumerateArray())
        {
            Require(condition: ids.Add(item: Text(value: server, key: "serverId")), code: "SESSION_SERVER_DUPLICATE");
            ValidateRun(state: server);
            Require(condition: Text(value: server, key: "joinStatus") is "ready" or "full" or "incompatible"
                    or "unreachable", code: "SESSION_JOIN_STATUS");
        }
    }

    private static void ValidateJoin(JsonElement state)
    {
        foreach (string key in new[] { "serverId", "runId", "address", "message", "detail" })
        {
            _ = Text(value: state, key: key);
        }
        _ = Number(value: state, key: "progress", minimum: 0, maximum: 100);
        _ = Number(value: state, key: "timeoutSeconds", minimum: 1, maximum: 300);
        _ = Number(value: state, key: "lossSeconds", minimum: 1, maximum: 300);
    }

    private static void ValidateSaves(JsonElement state)
    {
        var ids = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement save in state.GetProperty(propertyName: "saves").EnumerateArray())
        {
            Require(condition: ids.Add(item: Text(value: save, key: "saveId")), code: "SESSION_SAVE_DUPLICATE");
            foreach (string key in new[] { "runId", "runName", "result", "time", "compatibility", "detail" })
            {
                _ = Text(value: save, key: key);
            }
            _ = Number(value: save, key: "day", minimum: 1, maximum: 1000000);
            Require(condition: Text(value: save, key: "kind") is "Manual" or "Autosave", code: "SESSION_SAVE_KIND");
            _ = save.GetProperty(propertyName: "older").GetBoolean();
        }
    }

    private static void ValidateOutcome(JsonElement state)
    {
        _ = Text(value: state, key: "runId");
        _ = Number(value: state, key: "day", minimum: 1, maximum: 1000000);
        int victoryDay = Number(value: state, key: "earlierVictoryDay", minimum: 0, maximum: 1000000);
        Require(condition: victoryDay <= state.GetProperty(propertyName: "day").GetInt32(),
                code: "SESSION_OUTCOME_CHRONOLOGY");
    }

    private static void ValidateDiagnostics(JsonElement state)
    {
        foreach (JsonElement diagnostic in state.GetProperty(propertyName: "diagnostics").EnumerateArray())
        {
            foreach (string key in new[] { "id", "file", "field", "value", "reason", "kind" })
            {
                _ = Text(value: diagnostic, key: key);
            }
            Require(condition: Text(value: diagnostic, key: "kind") is "text" or "audio" or "data"
                    or "item" or "asset" or "compatibility" or "legacy", code: "SESSION_DIAGNOSTIC_KIND");
            Require(condition: !diagnostic.GetProperty(propertyName: "resourcePass").GetBoolean(),
                    code: "SESSION_DIAGNOSTIC_FALSE_PASS");
        }
    }

    private void RequireActive(long generation)
    {
        _gateway.RequireThread();
        if (!_accepting || generation <= _generation || !_root.IsInsideTree())
        {
            throw new InvalidOperationException(message: "SESSION_NOT_ACTIVE");
        }
    }

    private VBoxContainer Attach(JsonElement state)
    {
        _gateway.RequireThread();
        var panel = new PanelContainer { Name = "Session", Size = new Vector2(x: 1168, y: 640),
                Theme = ResourceLoader.Load<Theme>(path: "res://assets/ui/InspectionTheme.tres"),
                MouseFilter = Control.MouseFilterEnum.Stop };
        _root.AddChild(node: panel);
        _owned.Add(item: panel);
        panel.SetMeta(name: "resource_id", value: Text(value: state, key: "id"));
        var scroll = new ScrollContainer { Name = "Scroll", FollowFocus = true,
                HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill };
        panel.AddChild(node: scroll);
        var content = new VBoxContainer { Name = "Content", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        scroll.AddChild(node: content);
        Label(parent: content, name: "Title", text: Text(value: state, key: "title"));
        Label(parent: content, name: "Identity", text: Text(value: state, key: "id"));
        Label(parent: content, name: "Scope", text: "Layout example — gameplay actions are not active.");
        Label(parent: content, name: "Status", text: Text(value: state, key: "status"));
        return content;
    }

    private void Main(VBoxContainer content)
    {
        _gateway.RequireThread();
        var menu = new VBoxContainer { Name = "MainMenu" };
        content.AddChild(node: menu);
        Label(parent: menu, name: "GameTitle", text: "KingdomZombies");
        foreach (string action in new[] { "New solo run", "Host run", "LAN sessions", "Manual address",
                "Saved runs", "Settings", "Controls", "Exit" })
        {
            Action(parent: menu, name: "Choice" + menu.GetChildCount(), text: action);
        }
    }

    private void Setup(JsonElement state, VBoxContainer content, bool host)
    {
        _gateway.RequireThread();
        var form = new GridContainer { Name = host ? "HostSetup" : "SoloSetup", Columns = 2 };
        content.AddChild(node: form);
        Label(parent: form, name: "NameCaption", text: "Run name");
        var name = new LineEdit { Name = "RunName", Text = Text(value: state, key: "runName"), Editable = false,
                CustomMinimumSize = new Vector2(x: 400, y: 48) };
        form.AddChild(node: name);
        RunFacts(parent: content, state: state);
        Label(parent: content, name: "Authority",
                text: "Host balance applies at new-run start or load; no live reload.");
        Action(parent: content, name: "Start", text: host ? "Create LAN session" : "Start solo run");
    }

    private void RunFacts(Node parent, JsonElement state)
    {
        _gateway.RequireThread();
        foreach (string key in new[] { "runId", "runName", "host", "address", "compatibility" })
        {
            Label(parent: parent, name: key, text: key + ": " + Text(value: state, key: key));
        }
        Label(parent: parent, name: "Capacity", text: "Participants " + state.GetProperty(propertyName: "participants")
                + " / " + state.GetProperty(propertyName: "capacity"));
        Label(parent: parent, name: "Progress", text: state.GetProperty(propertyName: "inProgress").GetBoolean()
                ? "Run in progress" : "Not started");
    }

    private void Browser(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        Action(parent: content, name: "Refresh", text: "Refresh LAN sessions");
        var servers = new VBoxContainer { Name = "Servers" };
        content.AddChild(node: servers);
        foreach (JsonElement server in state.GetProperty(propertyName: "servers").EnumerateArray())
        {
            var row = new PanelContainer { Name = "Server" + servers.GetChildCount() };
            servers.AddChild(node: row);
            row.SetMeta(name: "resource_id", value: Text(value: server, key: "serverId"));
            var facts = new VBoxContainer { Name = "Facts" };
            row.AddChild(node: facts);
            Label(parent: facts, name: "ServerId", text: Text(value: server, key: "serverId"));
            RunFacts(parent: facts, state: server);
            string status = Text(value: server, key: "joinStatus");
            Action(parent: facts, name: "Join", text: "Join session", available: Same(left: status, right: "ready"));
            Label(parent: facts, name: "JoinStatus", text: status);
        }
        int count = servers.GetChildCount();
        Label(parent: content, name: "Count", text: count + (count == 1 ? " session" : " sessions"));
        if (count == 0)
        {
            Label(parent: content, name: "Empty", text:
                    Same(left: Text(value: state, key: "status"), right: "discovering")
                            ? "Discovering LAN sessions…" : "No LAN sessions found. Refresh or enter an address.");
        }
    }

    private void Manual(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        var address = new LineEdit { Name = "Address", Text = state.GetProperty(propertyName: "address").GetString(),
                Editable = false, PlaceholderText = "Host address", CustomMinimumSize = new Vector2(x: 400, y: 48) };
        content.AddChild(node: address);
        Label(parent: content, name: "Validation", text: Text(value: state, key: "validation"));
        Action(parent: content, name: "Connect", text: "Join address",
                available: Same(left: Text(value: state, key: "status"), right: "valid"));
    }

    private void Lobby(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        RunFacts(parent: content, state: state);
        var roster = new GridContainer { Name = "Roster", Columns = 2 };
        content.AddChild(node: roster);
        foreach (JsonElement player in state.GetProperty(propertyName: "players").EnumerateArray())
        {
            Label(parent: roster, name: "Player" + roster.GetChildCount(), text: Text(value: player, key: "id"));
            Label(parent: roster, name: "Name" + roster.GetChildCount(), text: Text(value: player, key: "name"));
        }
        Action(parent: content, name: "Begin", text: state.GetProperty(propertyName: "inProgress").GetBoolean()
                ? "Resume run" : "Start run");
        Label(parent: content, name: "LateJoin",
                text: "The host can start alone. Guests may join later without new supplies.");
    }

    private void Join(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        foreach (string key in new[] { "serverId", "runId", "address", "message", "detail" })
        {
            Label(parent: content, name: key, text: Text(value: state, key: key));
        }
        var progress = new ProgressBar { Name = "Transfer", MaxValue = 100,
                Value = state.GetProperty(propertyName: "progress").GetInt32() };
        content.AddChild(node: progress);
        Label(parent: content, name: "World",
                text: "Existing participants continue playing. Joining actions wait for ready state.");
        Label(parent: content, name: "Timeouts", text: "Join timeout "
                + state.GetProperty(propertyName: "timeoutSeconds")
                + " seconds · Host loss " + state.GetProperty(propertyName: "lossSeconds") + " seconds");
        Action(parent: content, name: "Return", text: "Return to main menu");
    }

    private void Saves(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        Label(parent: content, name: "SelectedRun", text: Text(value: state, key: "selectedRun"));
        var saves = new VBoxContainer { Name = "SavedRuns" };
        content.AddChild(node: saves);
        foreach (JsonElement save in state.GetProperty(propertyName: "saves").EnumerateArray())
        {
            var row = new VBoxContainer { Name = "Save" + saves.GetChildCount() };
            saves.AddChild(node: row);
            row.SetMeta(name: "resource_id", value: Text(value: save, key: "saveId"));
            foreach (string key in new[] { "saveId", "runId", "runName", "result", "time", "kind", "compatibility",
                    "detail" })
            {
                Label(parent: row, name: key, text: Text(value: save, key: key));
            }
            Label(parent: row, name: "Day", text: "Day " + save.GetProperty(propertyName: "day"));
            Label(parent: row, name: "Retained", text: save.GetProperty(propertyName: "older").GetBoolean()
                    ? "Retained older save" : "Latest successful save");
            Action(parent: row, name: "Load", text: "Load selected save",
                    available: Text(value: save, key: "compatibility") is "Compatible" or "Legacy layout");
        }
        Label(parent: content, name: "Count", text: saves.GetChildCount() + " saves");
        Label(parent: content, name: "Integrity",
                text: "Failed saves preserve the last valid save. Originals remain intact.");
    }

    private void Exit(JsonElement state, VBoxContainer content, bool host)
    {
        _gateway.RequireThread();
        Label(parent: content, name: "Departure", text: Departure(host: host));
        Label(parent: content, name: "Reason", text: Text(value: state, key: "reason"));
        if (!host)
        {
            Label(parent: content, name: "Survivor",
                    text: "Your survivor and belongings remain in the world under NPC control, vulnerable.");
            Action(parent: content, name: "Leave", text: "Leave session");
            Action(parent: content, name: "Stay", text: "Stay in session");
            return;
        }
        Label(parent: content, name: "HostScope",
                text: "Host departure ends the session for everyone. There is no host migration.");
        bool failed = Same(left: Text(value: state, key: "status"), right: "save-failed");
        Action(parent: content, name: "Retry", text: failed ? "Retry save" : "Save and end session");
        Action(parent: content, name: "Return", text: "Return to play");
        if (failed)
        {
            Action(parent: content, name: "EndWithoutSave", text: "End without latest save");
            long generation = _generation;
            content.GetNode<Button>(path: "EndWithoutSave").Pressed += () =>
                    RequestConfirmation(generation: generation);
        }
    }

    private void RequestConfirmation(long generation)
    {
        _gateway.RequireThread();
        if (!_accepting || generation != _generation)
        {
            return;
        }
        if (_root.HasNode(path: "SessionConfirmation"))
        {
            return;
        }
        var panel = new PanelContainer { Name = "SessionConfirmation", ZIndex = 50,
                Position = new Vector2(x: 100, y: 100), Size = new Vector2(x: 960, y: 340),
                Theme = ResourceLoader.Load<Theme>(path: "res://assets/ui/InspectionTheme.tres") };
        _root.AddChild(node: panel);
        _owned.Add(item: panel);
        var content = new VBoxContainer { Name = "Content" };
        panel.AddChild(node: content);
        Label(parent: content, name: "Message", text: "End the session without the latest save? "
                + "Progress since the last successful save will be lost.");
        Action(parent: content, name: "Confirm", text: "End without latest save");
        Action(parent: content, name: "Return", text: "Return to play");
        content.GetNode<Button>(path: "Return").GrabFocus();
        content.GetNode<Button>(path: "Confirm").Pressed += () =>
                FinishConfirmation(generation: generation, confirmed: true);
        content.GetNode<Button>(path: "Return").Pressed += () =>
                FinishConfirmation(generation: generation, confirmed: false);
    }

    private void FinishConfirmation(long generation, bool confirmed)
    {
        _gateway.RequireThread();
        if (!_accepting || generation != _generation || !_root.HasNode(path: "SessionConfirmation"))
        {
            return;
        }
        Control panel = _root.GetNode<Control>(path: "SessionConfirmation");
        panel.Visible = false;
        panel.ProcessMode = Node.ProcessModeEnum.Disabled;
        foreach (Button button in panel.GetNode(path: "Content").GetChildren().OfType<Button>())
        {
            button.FocusMode = Control.FocusModeEnum.None;
            button.Disabled = true;
        }
        panel.QueueFree();
        _owned.Remove(item: panel);
        Label(parent: _root.GetNode(path: "Session/Scroll/Content"), name: "ConfirmationResult",
                text: confirmed ? "Confirmed inspection example; no save or session operation executed."
                        : "Returned to play example; progress retained.");
    }

    private void Outcome(JsonElement state, VBoxContainer content, bool victory)
    {
        _gateway.RequireThread();
        Label(parent: content, name: "Run", text: Text(value: state, key: "runId") + " · Day "
                + state.GetProperty(propertyName: "day"));
        Label(parent: content, name: "Result",
                text: victory ? "Victory recorded. Living zombies can still threaten the camp."
                        : "Defeat recorded for this continuation.");
        int earlier = state.GetProperty(propertyName: "earlierVictoryDay").GetInt32();
        Label(parent: content, name: "EarlierVictory", text: earlier > 0 ? "Earlier victory retained · Day " + earlier
                : "No earlier victory recorded");
        if (victory)
        {
            Action(parent: content, name: "Continue", text: "Continue");
        }
        else
        {
            Action(parent: content, name: "Load", text: "Load retained save");
            Action(parent: content, name: "New", text: "New run");
        }
        Action(parent: content, name: "Main", text: "Main menu");
    }

    private void Diagnostics(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        var diagnostics = new VBoxContainer { Name = "Diagnostics" };
        content.AddChild(node: diagnostics);
        foreach (JsonElement row in state.GetProperty(propertyName: "diagnostics").EnumerateArray())
        {
            Label(parent: diagnostics, name: "Diagnostic" + diagnostics.GetChildCount(), text:
                    Text(value: row, key: "id") + ": " + Text(value: row, key: "file") + ", field "
                            + Text(value: row, key: "field") + ", value " + Text(value: row, key: "value")
                            + " — " + Text(value: row, key: "reason") + " · Unavailable · Pack incomplete");
        }
    }

    private void Label(Node parent, string name, string text)
    {
        _gateway.RequireThread();
        var label = new Label { Name = name, Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, CustomMinimumSize = new Vector2(x: 0, y: 32),
                FocusMode = Control.FocusModeEnum.All };
        parent.AddChild(node: label);
    }

    private void Action(Node parent, string name, string text, bool available = true)
    {
        _gateway.RequireThread();
        var action = new Button { Name = name, Text = text, Disabled = !available,
                FocusMode = available ? Control.FocusModeEnum.All : Control.FocusModeEnum.None,
                CustomMinimumSize = new Vector2(x: 0, y: 48), TooltipText = "Inspection example only" };
        parent.AddChild(node: action);
    }

    private static void BindSettings(JsonElement pack, byte[][] inputs)
    {
        Require(condition: Same(left: Text(value: pack, key: "id"), right: "ui-settings")
                && pack.GetProperty(propertyName: "sampleDataOnly").GetBoolean()
                && !pack.GetProperty(propertyName: "gameplayTransactions").GetBoolean(), code: "SETTINGS_PACK_SCOPE");
        JsonElement bindings = pack.GetProperty(propertyName: "bindings");
        Require(condition: bindings.GetArrayLength() == _settingsPaths.Length && inputs.Length == _settingsPaths.Length,
                code: "SETTINGS_BINDING_COUNT");
        for (int index = 0; index < _settingsPaths.Length; index++)
        {
            string path = _settingsPaths[index];
            Require(condition: Same(left: Text(value: bindings[index], key: "path"), right: path),
                    code: "SETTINGS_BINDING_PATH " + path);
            int maximum = Same(left: path, right: "assets/ui/InspectionTheme.tres") ? 2097152 : 1048576;
            Require(condition: inputs[index].Length > 0 && inputs[index].Length <= maximum,
                    code: "SETTINGS_BINDING_SIZE " + path);
            string digest = Convert.ToHexStringLower(inArray: SHA256.HashData(source: inputs[index]));
            Require(condition: Same(left: digest, right: Text(value: bindings[index], key: "sha256")),
                    code: "SETTINGS_BINDING_HASH " + path);
        }
    }

    private static void ValidateSettings(JsonElement state)
    {
        Require(condition: Same(left: Text(value: state, key: "world"), right: "World continues"),
                code: "SETTINGS_WORLD_CONTINUES");
        _ = Text(value: state, key: "placeholder");
        _ = Text(value: state, key: "expanded");
        Require(condition: Text(value: state, key: "backstop") is "none" or "empty" or "loading" or "error"
                or "partial" or "overflow" or "long-text", code: "SETTINGS_BACKSTOP");
        JsonElement preferences = state.GetProperty(propertyName: "preferences");
        Require(condition: Text(value: preferences, key: "resolution") is "1280x720" or "1920x1080",
                code: "SETTINGS_RESOLUTION");
        Require(condition: Number(value: preferences, key: "scale", minimum: 80, maximum: 150) is 80 or 100 or 150,
                code: "SETTINGS_SCALE");
        _ = preferences.GetProperty(propertyName: "fullscreen").GetBoolean();
        foreach (string key in new[] { "master", "effects", "ambience" })
        {
            _ = Number(value: preferences, key: key, minimum: 0, maximum: 100);
        }
        var actions = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement binding in state.GetProperty(propertyName: "bindings").EnumerateArray())
        {
            Require(condition: actions.Add(item: Text(value: binding, key: "action")),
                    code: "SETTINGS_DUPLICATE_ACTION");
            _ = Text(value: binding, key: "current");
            _ = Text(value: binding, key: "default");
            Require(condition: Text(value: binding, key: "state") is "bound" or "conflict" or "unbound",
                    code: "SETTINGS_BINDING_STATE");
        }
    }

    private void ProjectSettings(JsonElement state)
    {
        _gateway.RequireThread();
        var panel = new PanelContainer { Name = "Settings", Size = _root.Size,
                Theme = ResourceLoader.Load<Theme>(path: "res://assets/ui/InspectionTheme.tres"),
                MouseFilter = Control.MouseFilterEnum.Stop };
        _root.AddChild(node: panel);
        _owned.Add(item: panel);
        var frame = new VBoxContainer { Name = "Frame" };
        panel.AddChild(node: frame);
        var selectors = new HBoxContainer { Name = "Selectors" };
        frame.AddChild(node: selectors);
        Selector(parent: selectors, name: "Canvas", choices: ["1280 × 720", "1920 × 1080"]);
        Selector(parent: selectors, name: "Scale", choices: ["80% UI", "100% UI", "150% UI"]);
        Selector(parent: selectors, name: "View", choices: ["Fit to window", "1:1 pan"]);
        Label(parent: frame, name: "Endpoint", text: "Authored layout endpoint");
        var viewport = new ScrollContainer { Name = "Viewport", FollowFocus = true,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill };
        frame.AddChild(node: viewport);
        var extent = new Control { Name = "Extent" };
        viewport.AddChild(node: extent);
        var canvas = new PanelContainer { Name = "Canvas", MouseFilter = Control.MouseFilterEnum.Stop,
                Theme = panel.Theme };
        extent.AddChild(node: canvas);
        var scroll = new ScrollContainer { Name = "LocalScroll", FollowFocus = true,
                HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        canvas.AddChild(node: scroll);
        var content = new VBoxContainer { Name = "Content", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        scroll.AddChild(node: content);
        Label(parent: content, name: "Identity", text: Text(value: state, key: "id"));
        Label(parent: content, name: "Title", text: Text(value: state, key: "title"));
        Label(parent: content, name: "World", text: Text(value: state, key: "world"));
        Label(parent: content, name: "Scope",
                text: "Inspection example — no settings, saves or bindings are persisted.");
        Label(parent: content, name: "Status", text: Text(value: state, key: "status"));
        Label(parent: content, name: "Local", text: "Local preferences — this device only; World continues.");
        SettingsFields(state: state, content: content);
        Label(parent: content, name: "Host", text: "Host balance — host chooses at new-run start or load; "
                + "no live reload. Local preferences are separate.");
        Label(parent: content, name: "Placeholder", text: Text(value: state, key: "placeholder"));
        Label(parent: content, name: "Expanded", text: Text(value: state, key: "expanded"));
        Diagnostics(state: state, content: content);
        Action(parent: content, name: "CopyDetails", text: "Copy complete diagnostics");
        Action(parent: content, name: "Recheck", text: "Recheck resources example");
        Action(parent: content, name: "Return", text: "Return to active world");
        WireSelectors(selectors: selectors, state: state);
    }

    private void Selector(Node parent, string name, string[] choices)
    {
        _gateway.RequireThread();
        var selector = new OptionButton { Name = name, TooltipText = "Inspection " + name,
                CustomMinimumSize = new Vector2(x: 224, y: 48) };
        foreach (string choice in choices)
        {
            selector.AddItem(label: choice);
        }
        parent.AddChild(node: selector);
    }

    private void WireSelectors(HBoxContainer selectors, JsonElement state)
    {
        _gateway.RequireThread();
        var canvas = selectors.GetNode<OptionButton>(path: "Canvas");
        var scale = selectors.GetNode<OptionButton>(path: "Scale");
        var view = selectors.GetNode<OptionButton>(path: "View");
        int requested = state.GetProperty(propertyName: "preferences").GetProperty(propertyName: "scale").GetInt32();
        scale.Select(idx: requested == 80 ? 0 : requested == 100 ? 1 : 2);
        long generation = _generation;
        OptionButton.ItemSelectedEventHandler apply = index => ApplySelectors(selectors: selectors,
                generation: generation, index: index);
        canvas.ItemSelected += apply;
        scale.ItemSelected += apply;
        view.ItemSelected += apply;
        ApplySelectors(selectors: selectors, generation: generation, index: 0);
    }

    private void ApplySelectors(HBoxContainer selectors, long generation, long index)
    {
        _gateway.RequireThread();
        _ = index;
        if (_accepting && generation == _generation)
        {
            var canvas = selectors.GetNode<OptionButton>(path: "Canvas");
            var scale = selectors.GetNode<OptionButton>(path: "Scale");
            var view = selectors.GetNode<OptionButton>(path: "View");
            Endpoint(width: canvas.Selected == 0 ? 1280 : 1920, height: canvas.Selected == 0 ? 720 : 1080,
                    percent: scale.Selected == 0 ? 80 : scale.Selected == 1 ? 100 : 150, fit: view.Selected == 0);
        }
    }

    private void SettingsFields(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        JsonElement preferences = state.GetProperty(propertyName: "preferences");
        var fields = new GridContainer { Name = "Preferences", Columns = 2 };
        content.AddChild(node: fields);
        Label(parent: fields, name: "ResolutionCaption", text: "Display resolution");
        Selector(parent: fields, name: "Resolution", choices: ["1280x720", "1920x1080"]);
        fields.GetNode<OptionButton>(path: "Resolution").Select(idx:
                Same(left: Text(value: preferences, key: "resolution"), right: "1280x720") ? 0 : 1);
        Label(parent: fields, name: "FullscreenCaption", text: "Fullscreen");
        var fullscreen = new CheckButton { Name = "Fullscreen",
                ButtonPressed = preferences.GetProperty(propertyName: "fullscreen").GetBoolean() };
        fields.AddChild(node: fullscreen);
        Label(parent: fields, name: "UiScaleCaption", text: "UI scale — independent of world zoom");
        Selector(parent: fields, name: "UiScale", choices: ["80%", "100%", "150%"]);
        int percent = preferences.GetProperty(propertyName: "scale").GetInt32();
        fields.GetNode<OptionButton>(path: "UiScale").Select(idx: percent == 80 ? 0 : percent == 100 ? 1 : 2);
        foreach (string key in new[] { "master", "effects", "ambience" })
        {
            Label(parent: fields, name: key + "Caption", text: key + " volume (0–100)");
            var slider = new HSlider { Name = key, MinValue = 0, MaxValue = 100, Step = 1,
                    Value = preferences.GetProperty(propertyName: key).GetInt32(),
                    CustomMinimumSize = new Vector2(x: 224, y: 48) };
            fields.AddChild(node: slider);
        }
        Bindings(state: state, content: content);
        bool loading = Same(left: Text(value: state, key: "status"), right: "loading");
        if (loading)
        {
            Label(parent: content, name: "Loading", text: "Loading local preferences — wait; World continues.");
            foreach (Control control in fields.GetChildren().OfType<Control>())
            {
                control.FocusMode = Control.FocusModeEnum.None;
                control.MouseFilter = Control.MouseFilterEnum.Ignore;
                if (control is BaseButton button)
                {
                    button.Disabled = true;
                }
                if (control is Slider slider)
                {
                    slider.Editable = false;
                }
            }
        }
    }

    private void Bindings(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        var bindings = new VBoxContainer { Name = "Bindings" };
        content.AddChild(node: bindings);
        Label(parent: bindings, name: "Caption", text: "Current input references — sample remapping and conflicts");
        foreach (JsonElement binding in state.GetProperty(propertyName: "bindings").EnumerateArray())
        {
            var row = new VBoxContainer { Name = "Binding" + bindings.GetChildCount() };
            bindings.AddChild(node: row);
            Label(parent: row, name: "Action", text: Text(value: binding, key: "action"));
            Label(parent: row, name: "Current", text: "Current: " + Text(value: binding, key: "current"));
            Label(parent: row, name: "Default", text: "Default: " + Text(value: binding, key: "default"));
            Label(parent: row, name: "State", text: Text(value: binding, key: "state"));
        }
        if (state.GetProperty(propertyName: "bindings").GetArrayLength() == 0)
        {
            Label(parent: bindings, name: "Empty", text: "No input actions available — 0 results.");
        }
        Action(parent: bindings, name: "Reset", text: "Reset bindings example");
    }

    private void Clear()
    {
        _gateway.RequireThread();
        foreach (Node node in _owned)
        {
            _root.RemoveChild(node: node);
            node.Free();
        }
        _owned.Clear();
    }
}
