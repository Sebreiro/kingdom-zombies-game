using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Projects inspection-only camp state without executing gameplay purchases.</summary>
internal sealed class CampLayouts
{
    internal CampLayouts(Control root, MainThreadGateway gateway, byte[] pack, byte[][] inputs)
    {
        _root = root;
        _gateway = gateway;
        JsonElement manifest = Read(bytes: pack);
        JsonElement map = ReadMap(manifest: manifest, inputs: inputs);
        _data = Read(bytes: inputs[0]);
        _definitions = _data.GetProperty(propertyName: "definitions");
        _icons = Read(bytes: inputs[5]).GetProperty(propertyName: "items");
        ValidateIconDefinitions();
        ValidateDefinitions(items: Read(bytes: inputs[1]), main: Read(bytes: inputs[2]),
                defenses: Read(bytes: inputs[3]), map: map);
        Unique(rows: _data.GetProperty(propertyName: "samples"));
        foreach (JsonElement sample in _data.GetProperty(propertyName: "samples").EnumerateArray())
        {
            Validate(state: sample);
        }
    }

    /// <summary>Quotes the stash balance after selling selected stored food.</summary>
    internal static int SaleMoney(int stashStock, int selected, int unitPrice, int stashMoney, int carriedFood)
    {
        if (stashStock is < 0 or > 1000000 || selected < 0 || selected > stashStock
                || unitPrice is not (1 or 2 or 4) || stashMoney is < 0 or > 100000000 || carriedFood < 0)
        {
            throw new InvalidDataException(message: "CAMP_SALE_DOMAIN");
        }

        return checked(stashMoney + selected * unitPrice);
    }

    /// <summary>Returns capacity from the last completed main-building tier.</summary>
    internal static int Capacity(int completedTier, int pendingTier)
    {
        if (completedTier is < 1 or > 3 || pendingTier < 0 || pendingTier > 3
                || (pendingTier != 0 && pendingTier != completedTier + 1))
        {
            throw new InvalidDataException(message: "CAMP_TIER_DOMAIN");
        }

        return completedTier * 4 + 4;
    }

    /// <summary>Returns the exact reason an inspection assault action is unavailable.</summary>
    internal static string RallyReason(bool hasTarget, bool active, int ready, int personalCash)
    {
        return !hasTarget ? "No surviving portal on this side."
                : active ? "An assault is already active on this side."
                        : ready < 8 ? "Need8 combat-ready warriors on this side."
                                : personalCash < 25 ? "Need25 personal carried money."
                                        : "Ready";
    }

    /// <summary>Parses detached bounded strict Unicode data before any engine operation.</summary>
    internal static string WarningText(string text, bool muted)
    {
        return text;
    }

    /// <summary>Determines whether a known portal is eligible for any map presentation.</summary>
    internal static bool PortalVisible(bool known)
    {
        return true;
    }

    /// <summary>Explains why a spectator cannot request a controlled survivor action.</summary>
    internal static string SpectatorReason(bool available)
    {
        return "Ready";
    }

    /// <summary>Parses detached bounded strict Unicode data before any engine operation.</summary>
    internal static JsonElement Read(byte[] bytes)
    {
        return ReadDocument(bytes: bytes, mapDependency: false);
    }

    /// <summary>Opens a saved inspection example without buying or changing game state.</summary>
    internal void Open(string id)
    {
        _gateway.RequireThread();
        JsonElement state = Find(rows: _data.GetProperty(propertyName: "samples"), id: id);
        Project(snapshot: JsonSerializer.SerializeToUtf8Bytes(value: state), generation: checked(_generation + 1));
    }

    /// <summary>Validates an entire candidate before replacing this owner's current controls.</summary>
    internal void Project(byte[] snapshot, long generation)
    {
        _gateway.RequireThread();
        JsonElement state = Read(bytes: snapshot);
        Validate(state: state);
        if (!_accepting || generation <= _generation || !_root.IsInsideTree())
        {
            throw new InvalidOperationException(message: "CAMP_NOT_ACTIVE");
        }

        Clear();
        _generation = generation;
        VBoxContainer content = Attach(state: state);
        switch (Text(value: state, key: "surface"))
        {
            case "people": People(state: state, content: content); break;
            case "sale": Sale(state: state, content: content); break;
            case "site": Site(state: state, content: content); break;
            case "rally": Rally(state: state, content: content); break;
        }
    }

    /// <summary>Revokes completion acceptance before freeing only this owner's controls and textures.</summary>
    internal void Close()
    {
        _gateway.RequireThread();
        _accepting = false;
        _generation = checked(_generation + 1);
        Clear();
    }

    private static readonly string[] _paths = ["content/ui/camp-panels.json", "content/config/items.json",
        "content/world/main-assemblies.json", "content/world/defense-assemblies.json", "content/world/map.json",
        "content/packs/ui-identity.json", "assets/ui/InspectionTheme.tres", "assets/ui/LiberationSans.ttf",
        "assets/ui/PerfectDOSVGA437.ttf", "assets/ui/Fallback.ttf", "assets/equipment/items-01-10.png",
        "assets/equipment/items-11-24.png", "assets/common/donor-kit.png"];
    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly JsonElement _data;
    private readonly JsonElement _definitions;
    private readonly JsonElement _icons;
    private readonly List<Node> _owned = [];
    private readonly List<AtlasTexture> _textures = [];
    private bool _accepting = true;
    private long _generation;

    private static JsonElement ReadMap(JsonElement manifest, byte[][] inputs)
    {
        Bind(manifest: manifest, inputs: inputs);
        return ReadDocument(bytes: inputs[4], mapDependency: true);
    }

    private static JsonElement ReadDocument(byte[] bytes, bool mapDependency)
    {
        if (bytes.Length is 0 or > 1048576 || bytes[0] == 239)
        {
            throw new InvalidDataException(message: "CAMP_DOCUMENT_SIZE");
        }

        string text = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: bytes);
        using var document = JsonDocument.Parse(json: text, options: new JsonDocumentOptions { MaxDepth = 20 });
        int count = 0;
        Check(value: document.RootElement, count: ref count, mapPosition: mapDependency ? 0 : -1);
        return document.RootElement.Clone();
    }

    private static bool Same(string left, string right)
    {
        return StringComparer.Ordinal.Equals(x: left, y: right);
    }

    private static string Text(JsonElement value, string key)
    {
        string text = value.GetProperty(propertyName: key).GetString()
                ?? throw new InvalidDataException(message: "CAMP_TEXT " + key);
        if (text.Length is 0 or > 4096)
        {
            throw new InvalidDataException(message: "CAMP_TEXT " + key);
        }

        return text;
    }

    private static int Number(JsonElement value, string key, int minimum, int maximum)
    {
        if (!value.GetProperty(propertyName: key).TryGetInt32(value: out int number)
                || number < minimum || number > maximum)
        {
            throw new InvalidDataException(message: "CAMP_RANGE " + key);
        }

        return number;
    }

    private static void Require(bool condition, string code)
    {
        if (!condition)
        {
            throw new InvalidDataException(message: code);
        }
    }

    private static void Check(JsonElement value, ref int count, int mapPosition)
    {
        Require(condition: ++count <= 30000, code: "CAMP_DOCUMENT_COUNT");
        if (value.ValueKind == JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                Require(condition: keys.Add(item: property.Name)
                        && property.Name is not ("__proto__" or "prototype" or "constructor"),
                        code: "CAMP_DOCUMENT_KEY");
                int position = mapPosition == 0 && Same(left: property.Name, right: "native") ? 1 : -1;
                if (mapPosition == 1 && Same(left: property.Name, right: "nodes"))
                {
                    position = 2;
                }

                Check(value: property.Value, count: ref count, mapPosition: position);
            }
        }
        else if (value.ValueKind == JsonValueKind.Array)
        {
            Require(condition: value.GetArrayLength() <= (mapPosition == 2 ? 512 : 256),
                    code: "CAMP_ARRAY_COUNT");
            foreach (JsonElement child in value.EnumerateArray())
            {
                Check(value: child, count: ref count, mapPosition: -1);
            }
        }
        else if (value.ValueKind == JsonValueKind.String)
        {
            string text = value.GetString() ?? "";
            Require(condition: text.Length <= 4096 && text.IsNormalized(normalizationForm: NormalizationForm.FormC)
                    && !text.Any(predicate: character => char.IsControl(c: character)), code: "CAMP_ENCODING");
        }
        else if (value.ValueKind == JsonValueKind.Number)
        {
            Require(condition: double.IsFinite(d: value.GetDouble()), code: "CAMP_NUMBER");
        }
    }

    private static void Unique(JsonElement rows)
    {
        Require(condition: rows.GetArrayLength() is > 0 and <= 128
                && rows.EnumerateArray().Select(selector: row => Text(value: row, key: "id"))
                        .Distinct(comparer: StringComparer.Ordinal).Count() == rows.GetArrayLength(),
                code: "CAMP_DUPLICATE_ID");
    }

    private static JsonElement Find(JsonElement rows, string id)
    {
        JsonElement[] matches = rows.EnumerateArray().Where(predicate: row =>
                Same(left: Text(value: row, key: "id"), right: id)).ToArray();
        Require(condition: matches.Length == 1, code: "CAMP_REFERENCE " + id);
        return matches[0];
    }

    private static void Bind(JsonElement manifest, byte[][] inputs)
    {
        JsonElement bindings = manifest.GetProperty(propertyName: "bindings");
        Require(condition: Same(left: Text(value: manifest, key: "id"), right: "ui-camp")
                && manifest.GetProperty(propertyName: "sampleDataOnly").GetBoolean()
                && !manifest.GetProperty(propertyName: "gameplayTransactions").GetBoolean()
                && bindings.GetArrayLength() == _paths.Length && inputs.Length == _paths.Length,
                code: "CAMP_BINDINGS");
        for (int index = 0; index < _paths.Length; index++)
        {
            Require(condition: inputs[index].Length > 0 && inputs[index].Length <= 16777216,
                    code: "CAMP_BINDING_SIZE");
            string digest = Convert.ToHexStringLower(inArray: SHA256.HashData(source: inputs[index]));
            Require(condition: Same(left: Text(value: bindings[index], key: "path"), right: _paths[index])
                    && Same(left: Text(value: bindings[index], key: "sha256"), right: digest),
                    code: "CAMP_BINDING_HASH " + _paths[index]);
        }
    }

    private void ValidateDefinitions(JsonElement items, JsonElement main, JsonElement defenses, JsonElement map)
    {
        Require(condition: Same(left: Text(value: _data, key: "id"), right: "camp-panels")
                && _data.GetProperty(propertyName: "sampleDataOnly").GetBoolean()
                && !_data.GetProperty(propertyName: "gameplayTransactions").GetBoolean(), code: "CAMP_SCOPE");
        JsonElement roles = _definitions.GetProperty(propertyName: "roles");
        Unique(rows: roles);
        string[] ids = ["unassigned", "gatherer", "hunter", "melee", "archer"];
        int[] prices = [5, 8, 15, 15, 15];
        for (int index = 0; index < ids.Length; index++)
        {
            Require(condition: Number(value: Find(rows: roles, id: ids[index]), key: "price", minimum: 0,
                    maximum: 100) == prices[index], code: "CAMP_ROLE_PRICE");
        }

        Require(condition: roles.GetArrayLength() == 5
                && Number(value: _definitions, key: "recruitPrice", minimum: 1, maximum: 100) == 12
                && Number(value: _definitions, key: "totalCap", minimum: 1, maximum: 100) == 100,
                code: "CAMP_PEOPLE_BASELINE");
        ValidateFoods(items: items);
        ValidateStructures(main: main, defenses: defenses);
        foreach (JsonElement tier in main.GetProperty(propertyName: "tierDefinitions").EnumerateArray())
        {
            int level = Number(value: tier, key: "tier", minimum: 1, maximum: 3);
            Require(condition: tier.GetProperty(propertyName: "seats").GetInt32()
                    == _definitions.GetProperty(propertyName: "fireside")[level - 1].GetInt32()
                    && tier.GetProperty(propertyName: "seats").GetInt32() == Capacity(completedTier: level,
                            pendingTier: 0), code: "CAMP_CAPACITY_BASELINE");
        }

        Require(condition: map.GetProperty(propertyName: "portals").GetArrayLength() == 6,
                code: "CAMP_WORLD_BASELINE");
    }

    private void ValidateFoods(JsonElement items)
    {
        JsonElement foods = _definitions.GetProperty(propertyName: "foods");
        Unique(rows: foods);
        Require(condition: foods.GetArrayLength() == 3, code: "CAMP_FOOD_ROSTER");
        foreach (JsonElement food in foods.EnumerateArray())
        {
            string id = Text(value: food, key: "id");
            Require(condition: id is "IT-08" or "IT-09" or "IT-10", code: "CAMP_FOOD_ID");
            JsonElement actual = Find(rows: items.GetProperty(propertyName: "definitions"), id: id);
            Require(condition: Same(left: Text(value: food, key: "label"), right: Text(value: actual, key: "name"))
                    && Number(value: food, key: "price", minimum: 1, maximum: 4)
                            == (id is "IT-08" ? 1 : id is "IT-09" ? 2 : 4), code: "CAMP_FOOD_BASELINE");
        }
    }

    private void ValidateStructures(JsonElement main, JsonElement defenses)
    {
        JsonElement structures = _definitions.GetProperty(propertyName: "structures");
        Unique(rows: structures);
        Require(condition: structures.GetArrayLength() == 9, code: "CAMP_STRUCTURE_ROSTER");
        int[] valuations = [100, 300, 800, 20, 80, 220, 40, 130, 310];
        int index = 0;
        foreach (JsonElement definition in structures.EnumerateArray())
        {
            string kind = Text(value: definition, key: "kind");
            int tier = Number(value: definition, key: "tier", minimum: 1, maximum: 3);
            JsonElement prior = kind is "main" ? main.GetProperty(propertyName: "tierDefinitions")[tier - 1]
                    : Find(rows: defenses.GetProperty(propertyName: "tierDefinitions"),
                            id: Text(value: definition, key: "id"));
            foreach (string key in new[] { "health", "price" })
            {
                Require(condition: Number(value: definition, key: key, minimum: 0, maximum: 10000)
                        == prior.GetProperty(propertyName: key).GetInt32(), code: "CAMP_STRUCTURE_BASELINE");
            }

            int valuation = Number(value: definition, key: "valuation", minimum: 1, maximum: 10000);
            Require(condition: valuation == valuations[index++] && Number(value: definition, key: "repair",
                    minimum: 1, maximum: 10000) == (valuation + 1) / 2, code: "CAMP_REPAIR_BASELINE");
        }
    }

    private void Validate(JsonElement state)
    {
        Text(value: state, key: "id");
        Text(value: state, key: "name");
        Text(value: state, key: "reason");
        Number(value: state, key: "personalCash", minimum: 0, maximum: 100000000);
        switch (Text(value: state, key: "surface"))
        {
            case "people": ValidatePeople(state: state); break;
            case "sale": ValidateSale(state: state); break;
            case "site": ValidateSite(state: state); break;
            case "rally": ValidateRally(state: state); break;
            default: throw new InvalidDataException(message: "CAMP_SURFACE");
        }
    }

    private void ValidatePeople(JsonElement state)
    {
        Capacity(completedTier: Number(value: state, key: "completedTier", minimum: 1, maximum: 3),
                pendingTier: Number(value: state, key: "pendingTier", minimum: 0, maximum: 3));
        int total = Number(value: state, key: "total", minimum: 0, maximum: 100);
        int controlled = Number(value: state, key: "controlled", minimum: 0, maximum: 4);
        int unassigned = Number(value: state, key: "unassigned", minimum: 0, maximum: 100);
        Require(condition: controlled + unassigned <= total
                && Number(value: state, key: "fireside", minimum: 0, maximum: 100) <= unassigned,
                code: "CAMP_POPULATION");
        JsonElement people = state.GetProperty(propertyName: "people");
        Unique(rows: people);
        Find(rows: people, id: Text(value: state, key: "selected"));
        foreach (JsonElement person in people.EnumerateArray())
        {
            Find(rows: _definitions.GetProperty(propertyName: "roles"), id: Text(value: person, key: "role"));
            Text(value: person, key: "name");
            Text(value: person, key: "activity");
            person.GetProperty(propertyName: "controlled").GetBoolean();
            person.GetProperty(propertyName: "atMain").GetBoolean();
        }

        Find(rows: _definitions.GetProperty(propertyName: "roles"), id: Text(value: state, key: "selectedRole"));
    }

    private void ValidateSale(JsonElement state)
    {
        JsonElement stock = state.GetProperty(propertyName: "stock");
        Unique(rows: stock);
        Require(condition: stock.GetArrayLength() == 3, code: "CAMP_SALE_ROSTER");
        foreach (JsonElement item in stock.EnumerateArray())
        {
            Find(rows: _definitions.GetProperty(propertyName: "foods"), id: Text(value: item, key: "id"));
            Number(value: item, key: "quantity", minimum: 0, maximum: 1000000);
        }

        JsonElement food = Find(rows: _definitions.GetProperty(propertyName: "foods"),
                id: Text(value: state, key: "food"));
        int quantity = Find(rows: stock, id: Text(value: food, key: "id"))
                .GetProperty(propertyName: "quantity").GetInt32();
        SaleMoney(stashStock: quantity, selected: Number(value: state, key: "selected", minimum: 0,
                maximum: 1000000), unitPrice: food.GetProperty(propertyName: "price").GetInt32(),
                stashMoney: Number(value: state, key: "stashMoney", minimum: 0, maximum: 100000000),
                carriedFood: Number(value: state, key: "carriedFood", minimum: 0, maximum: 1000000));
        Number(value: state, key: "stockRevision", minimum: 0, maximum: 1000000);
        Number(value: state, key: "quotedRevision", minimum: 0, maximum: 1000000);
    }

    private void ValidateSite(JsonElement state)
    {
        JsonElement definition = Find(rows: _definitions.GetProperty(propertyName: "structures"),
                id: Text(value: state, key: "structure"));
        Number(value: state, key: "health", minimum: 0,
                maximum: definition.GetProperty(propertyName: "health").GetInt32());
        Number(value: state, key: "completedTier", minimum: 0, maximum: 3);
        Number(value: state, key: "pendingTier", minimum: 0, maximum: 3);
        Number(value: state, key: "mainTier", minimum: 1, maximum: 3);
        Number(value: state, key: "workers", minimum: 0, maximum: 3);
        Number(value: state, key: "progress", minimum: 0, maximum: 10000);
        Require(condition: Text(value: state, key: "action") is "build" or "upgrade" or "rebuild" or "repair",
                code: "CAMP_ACTION");
        state.GetProperty(propertyName: "paid").GetBoolean();
        state.GetProperty(propertyName: "safe").GetBoolean();
        Require(condition: state.GetProperty(propertyName: "ruin").GetBoolean()
                == (state.GetProperty(propertyName: "health").GetInt32() == 0), code: "CAMP_RUIN");
    }

    private static void ValidateRally(JsonElement state)
    {
        string side = Text(value: state, key: "side");
        Require(condition: side is "left" or "right", code: "CAMP_RALLY_SIDE");
        int stationed = Number(value: state, key: "stationed", minimum: 0, maximum: 100);
        int ready = Number(value: state, key: "ready", minimum: 0, maximum: 100);
        Require(condition: ready <= stationed && Number(value: state, key: "melee", minimum: 0, maximum: 100)
                + Number(value: state, key: "archer", minimum: 0, maximum: 100) == ready, code: "CAMP_SQUAD");
        JsonElement portal = state.GetProperty(propertyName: "portal");
        Require(condition: portal.ValueKind == JsonValueKind.Null
                || Same(left: portal.GetString() ?? "", right: side + ".portal.1"),
                code: "CAMP_PORTAL_REFERENCE");
        state.GetProperty(propertyName: "active").GetBoolean();
        state.GetProperty(propertyName: "returning").GetBoolean();
    }

    private VBoxContainer Attach(JsonElement state)
    {
        _gateway.RequireThread();
        var panel = new PanelContainer { Name = "Camp", Position = new Vector2(x: 24, y: 112),
                Size = new Vector2(x: 1232, y: 584), Theme = ResourceLoader.Load<Theme>(
                        path: "res://assets/ui/InspectionTheme.tres") };
        var scroll = new ScrollContainer { Name = "Scroll",
                HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        var content = new VBoxContainer { Name = "Content", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        _root.AddChild(node: panel);
        _owned.Add(item: panel);
        panel.AddChild(node: scroll);
        scroll.AddChild(node: content);
        panel.SetMeta(name: "resource_id", value: Text(value: state, key: "id"));
        AddLabel(parent: content, name: "Title", text: Text(value: state, key: "name"));
        AddLabel(parent: content, name: "Scope", text: "Layout example · world continues · no gameplay purchase");
        return content;
    }

    private void People(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        int capacity = Capacity(completedTier: state.GetProperty(propertyName: "completedTier").GetInt32(),
                pendingTier: state.GetProperty(propertyName: "pendingTier").GetInt32());
        AddLabel(parent: content, name: "Population", text: "Unassigned "
                + state.GetProperty(propertyName: "unassigned")
                + " / " + capacity.ToString(provider: CultureInfo.InvariantCulture) + " · Fireside now "
                + state.GetProperty(propertyName: "fireside") + " · Total " + state.GetProperty(propertyName: "total")
                + " /100 · Controlled " + state.GetProperty(propertyName: "controlled"));
        var split = new HBoxContainer { Name = "PeopleColumns" };
        var roster = new VBoxContainer { Name = "Roster", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        var jobs = new VBoxContainer { Name = "Jobs", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        content.AddChild(node: split);
        split.AddChild(node: roster);
        split.AddChild(node: jobs);
        foreach (JsonElement person in state.GetProperty(propertyName: "people").EnumerateArray())
        {
            JsonElement role = Find(rows: _definitions.GetProperty(propertyName: "roles"),
                    id: Text(value: person, key: "role"));
            AddLabel(parent: roster, name: "Person", text: Text(value: person, key: "name") + " · "
                    + Text(value: role, key: "label") + " · " + Text(value: person, key: "activity"),
                    id: Text(value: person, key: "id"));
        }

        foreach (JsonElement role in _definitions.GetProperty(propertyName: "roles").EnumerateArray())
        {
            AddLabel(parent: jobs, name: "Job", text: Text(value: role, key: "label") + " · "
                    + role.GetProperty(propertyName: "price") + " carried money · "
                    + Text(value: role, key: "equipment"), id: Text(value: role, key: "id"));
        }

        string reason = Text(value: state, key: "reason");
        AddLabel(parent: content, name: "Funds", text: "Personal carried money "
                + state.GetProperty(propertyName: "personalCash") + " · Buy recruit12");
        AddAction(parent: content, name: "Recruit", text: "Buy recruit ·12", reason: reason,
                available: Same(left: reason, right: "Ready"));
        JsonElement selected = Find(rows: state.GetProperty(propertyName: "people"),
                id: Text(value: state, key: "selected"));
        bool eligible = !selected.GetProperty(propertyName: "controlled").GetBoolean()
                && selected.GetProperty(propertyName: "atMain").GetBoolean();
        AddAction(parent: jobs, name: "Assign", text: "Assign selected job", reason: eligible ? reason
                : "Return an uncontrolled survivor to the main building, deposit cargo and obsolete role gear.",
                available: eligible && Same(left: reason, right: "Ready"));
    }

    private void Sale(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        var ledger = new GridContainer { Name = "StashLedger", Columns = 3 };
        content.AddChild(node: ledger);
        foreach (JsonElement stock in state.GetProperty(propertyName: "stock").EnumerateArray())
        {
            JsonElement food = Find(rows: _definitions.GetProperty(propertyName: "foods"),
                    id: Text(value: stock, key: "id"));
            AddIcon(parent: ledger, id: Text(value: food, key: "id"));
            AddLabel(parent: ledger, name: "Food", text: Text(value: food, key: "label"));
            AddLabel(parent: ledger, name: "Stock", text: "Stash stock " + stock.GetProperty(propertyName: "quantity"),
                    id: Text(value: food, key: "id"));
        }

        JsonElement selected = Find(rows: _definitions.GetProperty(propertyName: "foods"),
                id: Text(value: state, key: "food"));
        int amount = state.GetProperty(propertyName: "selected").GetInt32();
        int unit = selected.GetProperty(propertyName: "price").GetInt32();
        bool current = state.GetProperty(propertyName: "stockRevision").GetInt32()
                == state.GetProperty(propertyName: "quotedRevision").GetInt32();
        int balance = state.GetProperty(propertyName: "stashMoney").GetInt32();
        int stockAmount = Find(rows: state.GetProperty(propertyName: "stock"),
                id: Text(value: selected, key: "id"))
                .GetProperty(propertyName: "quantity").GetInt32();
        int result = current ? SaleMoney(stashStock: stockAmount, selected: amount, unitPrice: unit,
                stashMoney: balance, carriedFood: state.GetProperty(propertyName: "carriedFood").GetInt32()) : balance;
        var quantity = new SpinBox { Name = "Quantity", MinValue = 0, MaxValue = stockAmount, Step = 1,
                Value = amount, Editable = false, CustomMinimumSize = new Vector2(x: 240, y: 48) };
        content.AddChild(node: quantity);
        AddLabel(parent: content, name: "Quote", text: Text(value: selected, key: "label") + " · Selected "
                + amount.ToString(provider: CultureInfo.InvariantCulture) + " · Unit "
                + unit.ToString(provider: CultureInfo.InvariantCulture) + " · Total "
                + (amount * unit).ToString(provider: CultureInfo.InvariantCulture));
        AddLabel(parent: content, name: "StashMoney", text: "Stash money " + balance.ToString(
                provider: CultureInfo.InvariantCulture) + " → "
                + result.ToString(provider: CultureInfo.InvariantCulture));
        AddLabel(parent: content, name: "Deposit", text: "Proceeds are physical money in the shared stash. "
                + "Carried food must be deposited first; personal cash stays unchanged.");
        AddAction(parent: content, name: "Sell", text: "Sell selected stash food", reason: Text(value: state,
                key: "reason"), available: current && amount > 0);
        if (Text(value: state, key: "state") is "quantity")
        {
            Modal(amount: amount, result: result);
        }
    }

    private void Site(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        JsonElement definition = Find(rows: _definitions.GetProperty(propertyName: "structures"),
                id: Text(value: state, key: "structure"));
        string action = Text(value: state, key: "action");
        JsonElement workDefinition = definition;
        int price = definition.GetProperty(propertyName: action is "repair" ? "repair"
                : action is "rebuild" ? "rebuild" : "price").GetInt32();
        if (action is "upgrade" && Text(value: definition, key: "kind") is "main")
        {
            JsonElement destination = Find(rows: _definitions.GetProperty(propertyName: "structures"),
                    id: "main-" + state.GetProperty(propertyName: "pendingTier"));
            price = destination.GetProperty(propertyName: "price").GetInt32();
            workDefinition = destination;
        }

        var facts = new GridContainer { Name = "SiteFacts", Columns = 2 };
        content.AddChild(node: facts);
        AddLabel(parent: facts, name: "Tier", text: Text(value: state, key: "structure")
                + " · Previous completed tier " + state.GetProperty(propertyName: "completedTier"));
        AddLabel(parent: facts, name: "Health", text: "Health " + state.GetProperty(propertyName: "health")
                + " / " + definition.GetProperty(propertyName: "health"));
        AddLabel(parent: facts, name: "Prerequisite", text: "Completed main tier "
                + state.GetProperty(propertyName: "mainTier") + " · Stash unlimited");
        AddLabel(parent: facts, name: "Cash", text: "Personal carried money "
                + state.GetProperty(propertyName: "personalCash") + " · " + action + " quote "
                + price.ToString(provider: CultureInfo.InvariantCulture));
        bool paid = state.GetProperty(propertyName: "paid").GetBoolean();
        AddLabel(parent: content, name: "Order", text: paid ? "Paid order · original price retained · no refund"
                : "No paid order");
        var progress = new ProgressBar { Name = "Work", MinValue = 0, MaxValue = Math.Max(val1: 1,
                val2: action is "repair" ? definition.GetProperty(propertyName: "health").GetInt32()
                        : workDefinition.GetProperty(propertyName: "work").GetInt32()),
                Value = state.GetProperty(propertyName: "progress").GetInt32(),
                CustomMinimumSize = new Vector2(x: 0, y: 32) };
        content.AddChild(node: progress);
        AddLabel(parent: content, name: "Workers", text: "Workers " + state.GetProperty(propertyName: "workers")
                + " /3 · " + (state.GetProperty(propertyName: "safe").GetBoolean() ? "Safe" : "Unsafe: waiting")
                + " · Completed work " + state.GetProperty(propertyName: "progress"));
        AddLabel(parent: content, name: "Policy", text: "Repair fee is fixed50% full valuation, "
                + "independent of missing HP. Active paid repair covers additional damage until full health. "
                + "Replacement workers retain paid progress.");
        string reason = Text(value: state, key: "reason");
        bool available = !paid && (action is "rebuild"
                ? state.GetProperty(propertyName: "ruin").GetBoolean()
                        && Text(value: definition, key: "kind") is not "main"
                : !state.GetProperty(propertyName: "ruin").GetBoolean()
                        && state.GetProperty(propertyName: "health").GetInt32()
                                < definition.GetProperty(propertyName: "health").GetInt32())
                && state.GetProperty(propertyName: "personalCash").GetInt32() >= price
                && Text(value: state, key: "state") is not "locked";
        AddAction(parent: content, name: "Purchase", text: action + " · personal cash", reason: reason,
                available: available);
    }

    private void Rally(JsonElement state, VBoxContainer content)
    {
        _gateway.RequireThread();
        var squad = new HBoxContainer { Name = "Squad" };
        content.AddChild(node: squad);
        AddLabel(parent: squad, name: "Side", text: "Side " + Text(value: state, key: "side"));
        AddLabel(parent: squad, name: "Stationed", text: "Stationed " + state.GetProperty(propertyName: "stationed"));
        AddLabel(parent: squad, name: "Ready", text: "Combat-ready " + state.GetProperty(propertyName: "ready"));
        var mix = new GridContainer { Name = "Mix", Columns = 2 };
        content.AddChild(node: mix);
        AddLabel(parent: mix, name: "Melee", text: "Melee " + state.GetProperty(propertyName: "melee") + " · Desired4");
        AddLabel(parent: mix, name: "Archers", text: "Archers "
                + state.GetProperty(propertyName: "archer") + " · Desired4");
        JsonElement portal = state.GetProperty(propertyName: "portal");
        AddLabel(parent: content, name: "Portal", text: portal.ValueKind == JsonValueKind.Null ? "No target portal"
                : "Target " + portal.GetString());
        bool active = state.GetProperty(propertyName: "active").GetBoolean();
        bool returning = state.GetProperty(propertyName: "returning").GetBoolean();
        AddLabel(parent: content, name: "Mission", text: returning ? "Returning · no teleport/refund"
                : active ? "Active assault · nightfall does not recall warriors" : "Stationed");
        AddLabel(parent: content, name: "Funds", text: "Launch25 personal carried money · Cash "
                + state.GetProperty(propertyName: "personalCash") + " · Squad8 · One assault per side");
        string reason = RallyReason(hasTarget: portal.ValueKind != JsonValueKind.Null, active: active,
                ready: state.GetProperty(propertyName: "ready").GetInt32(),
                personalCash: state.GetProperty(propertyName: "personalCash").GetInt32());
        AddAction(parent: content, name: "Launch", text: "Launch squad", reason: reason,
                available: Same(left: reason, right: "Ready"));
        AddAction(parent: content, name: "Recall", text: "Recall · Free", reason: active && !returning
                ? "Return to this side; launch fee is not refunded." : "No active outbound squad to recall.",
                available: active && !returning);
    }

    private void AddLabel(Node parent, string name, string text, string id = "")
    {
        _gateway.RequireThread();
        var label = new Label { Name = name, Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                CustomMinimumSize = new Vector2(x: 0, y: 32), FocusMode = Control.FocusModeEnum.All };
        parent.AddChild(node: label);
        label.SetMeta(name: "resource_id", value: id);
    }

    private void AddAction(Node parent, string name, string text, string reason, bool available)
    {
        _gateway.RequireThread();
        var button = new Button { Name = name, Text = text, Disabled = !available,
                FocusMode = available ? Control.FocusModeEnum.All : Control.FocusModeEnum.None,
                CustomMinimumSize = new Vector2(x: 0, y: 48), TooltipText = "Inspection example only. " + reason };
        parent.AddChild(node: button);
        AddLabel(parent: parent, name: name + "Reason", text: reason);
    }

    private void AddIcon(Node parent, string id)
    {
        _gateway.RequireThread();
        JsonElement icon = Find(rows: _icons, id: id);
        int[] rectangle = icon.GetProperty(propertyName: "atlasRect").EnumerateArray()
                .Select(selector: number => number.GetInt32()).ToArray();
        string path = Text(value: icon, key: "texture");
        var texture = new AtlasTexture { Atlas = ResourceLoader.Load<Texture2D>(path: "res://" + path),
                Region = new Rect2(x: rectangle[0], y: rectangle[1], width: rectangle[2], height: rectangle[3]) };
        _textures.Add(item: texture);
        var image = new TextureRect { Name = "Icon", Texture = texture,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                CustomMinimumSize = new Vector2(x: 48, y: 48),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered };
        parent.AddChild(node: image);
        image.SetMeta(name: "resource_id", value: id);
    }

    private void ValidateIconDefinitions()
    {
        foreach (string id in new[] { "IT-08", "IT-09", "IT-10" })
        {
            JsonElement icon = Find(rows: _icons, id: id);
            int[] rectangle = icon.GetProperty(propertyName: "atlasRect").EnumerateArray()
                    .Select(selector: number => number.GetInt32()).ToArray();
            string path = Text(value: icon, key: "texture");
            Require(condition: path is "assets/equipment/items-01-10.png" or "assets/equipment/items-11-24.png"
                    && rectangle.Length == 4 && rectangle[0] >= 0 && rectangle[1] >= 0
                    && rectangle[2] > 0 && rectangle[3] > 0 && rectangle[0] + rectangle[2] <= 1024
                    && rectangle[1] + rectangle[3] <= 1024, code: "CAMP_ICON_DOMAIN");
        }
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
        foreach (AtlasTexture texture in _textures)
        {
            texture.Dispose();
        }

        _textures.Clear();
    }

    private void Modal(int amount, int result)
    {
        _gateway.RequireThread();
        var panel = new PanelContainer { Name = "SaleConfirmation", Position = new Vector2(x: 300, y: 180),
                Size = new Vector2(x: 680, y: 300), ZIndex = 50,
                Theme = ResourceLoader.Load<Theme>(path: "res://assets/ui/InspectionTheme.tres"),
                MouseFilter = Control.MouseFilterEnum.Stop };
        var content = new VBoxContainer { Name = "Confirmation" };
        _root.AddChild(node: panel);
        _owned.Add(item: panel);
        panel.AddChild(node: content);
        AddLabel(parent: content, name: "Message", text: "Inspection confirmation · Sell "
                + amount.ToString(provider: CultureInfo.InvariantCulture) + " stash food · Result stash money "
                + result.ToString(provider: CultureInfo.InvariantCulture));
        AddAction(parent: content, name: "Confirm", text: "Confirm example", reason: "No gameplay transaction",
                available: true);
        AddAction(parent: content, name: "Cancel", text: "Return to sale example", reason: "Retain selection",
                available: true);
        content.GetNode<Button>(path: "Confirm").GrabFocus();
    }
}
