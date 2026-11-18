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
        Bind(pack: Read(bytes: pack), inputs: inputs);
        _data = Read(bytes: inputs[0]);
        Require(condition: Same(left: Text(value: _data, key: "id"), right: "session-save-outcomes")
                && _data.GetProperty(propertyName: "sampleDataOnly").GetBoolean(), code: "SESSION_DATA_SCOPE");
        Unique(rows: _data.GetProperty(propertyName: "samples"));
        foreach (JsonElement sample in _data.GetProperty(propertyName: "samples").EnumerateArray())
        {
            Validate(state: sample);
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
        RequireActive(generation: generation);
        Clear();
        _generation = generation;
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

    private static readonly string[] _paths = ["content/ui/session-save-outcomes.json",
        "assets/ui/InspectionTheme.tres", "assets/ui/LiberationSans.ttf", "assets/ui/PerfectDOSVGA437.ttf",
        "assets/ui/Fallback.ttf", "assets/common/donor-kit.png"];
    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly JsonElement _data;
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
                or "saves" or "host-exit" or "guest-leave" or "victory" or "defeat", code: "SESSION_SURFACE");
        switch (surface)
        {
            case "solo": case "host": case "lobby": ValidateRun(state: state); break;
            case "browser": ValidateBrowser(state: state); break;
            case "manual": _ = Text(value: state, key: "validation"); break;
            case "join": ValidateJoin(state: state); break;
            case "saves": ValidateSaves(state: state); break;
            case "host-exit": case "guest-leave": _ = Text(value: state, key: "reason"); break;
            case "victory": case "defeat": ValidateOutcome(state: state); break;
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
