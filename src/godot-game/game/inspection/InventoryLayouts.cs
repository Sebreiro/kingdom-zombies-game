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

/// <summary>Owns reusable inventory inspection projections, separate from gameplay transactions.</summary>
internal sealed class InventoryLayouts
{
    internal InventoryLayouts(Control root, MainThreadGateway gateway, byte[] pack, byte[][] inputs)
    {
        _root = root;
        _gateway = gateway;
        _pack = Read(bytes: pack);
        _legacy = Same(left: Text(value: _pack, key: "id"), right: "ui-legacy-stash");
        Bind(inputs: inputs);
        _data = Read(bytes: inputs[0]);
        _definitions = Read(bytes: inputs[1]).GetProperty(propertyName: "definitions").Clone();
        _icons = Read(bytes: inputs[2]).GetProperty(propertyName: "items").Clone();
        ValidateIcons(weapons: Read(bytes: inputs[3]), clothing: Read(bytes: inputs[4]));
        ValidateData();
    }

    /// <summary>Returns the logical equipment identities exposed by this projection.</summary>
    internal static string[] SlotIds()
    {
        return ["headwear", "face", "torso", "legs", "body-armor", "gloves", "boots", "backpack", "belt",
            "left-hand", "right-hand"];
    }

    /// <summary>Returns the displayed cell footprint for an inspection item.</summary>
    internal static int[] Footprint(int width, int height, bool rotated)
    {
        if (width is < 1 or > 16 || height is < 1 or > 16)
        {
            throw new InvalidDataException(message: "INVENTORY_FOOTPRINT");
        }

        return rotated ? [height, width] : [width, height];
    }

    /// <summary>Returns detached saved geometry independently of the current destination preview.</summary>
    internal static JsonElement SavedLayout(JsonElement sample)
    {
        return sample.GetProperty(propertyName: "saved").Clone();
    }

    /// <summary>Returns exact target identities in their displayed selection order.</summary>
    internal static string[] TargetIds(JsonElement sample)
    {
        return sample.GetProperty(propertyName: "targets").EnumerateArray()
                .Select(selector: row => Text(value: row, key: "id")).ToArray();
    }

    /// <summary>Resolves a food shortcut to its actual carried instance.</summary>
    internal static JsonElement? ResolveQuick(JsonElement state, int index)
    {
        if (index is < 0 or > 3)
        {
            throw new InvalidDataException(message: "INVENTORY_QUICK_INDEX");
        }

        JsonElement reference = state.GetProperty(propertyName: "quick")[index];
        if (reference.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        string id = reference.GetString() ?? throw new InvalidDataException(message: "INVENTORY_QUICK");
        foreach (JsonElement item in state.GetProperty(propertyName: "items").EnumerateArray())
        {
            if (Same(left: Text(value: item, key: "id"), right: id))
            {
                return item.Clone();
            }
        }

        return null;
    }

    /// <summary>Reads strict detached UTF-8 data before any native access.</summary>
    internal static JsonElement Read(byte[] bytes)
    {
        if (bytes.Length is 0 or > 1048576 || bytes[0] == 239)
        {
            throw new InvalidDataException(message: "INVENTORY_DOCUMENT_SIZE");
        }

        string text = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: bytes);
        using var document = JsonDocument.Parse(json: text, options: new JsonDocumentOptions { MaxDepth = 24 });
        int count = 0;
        CheckValue(value: document.RootElement, count: ref count);
        return document.RootElement.Clone();
    }

    /// <summary>Projects a named sample, never performing the game operation it describes.</summary>
    internal void Open(string id)
    {
        _gateway.RequireThread();
        JsonElement state = Find(rows: _data.GetProperty(propertyName: "samples"), id: id);
        Project(snapshot: JsonSerializer.SerializeToUtf8Bytes(value: state), generation: checked(_generation + 1));
    }

    /// <summary>Rejects stale generations and invalid candidates before replacing owned controls.</summary>
    internal void Project(byte[] snapshot, long generation)
    {
        _gateway.RequireThread();
        JsonElement state = Read(bytes: snapshot);
        if (_legacy)
        {
            ValidateLegacyState(state: state);
        }
        else
        {
            ValidateState(state: state);
        }

        if (!_accepting || generation <= _generation || !_root.IsInsideTree())
        {
            throw new InvalidOperationException(message: "INVENTORY_NOT_ACTIVE");
        }

        Clear();
        _generation = generation;
        Attach(state: state);
        if (_legacy)
        {
            ProjectLegacy(state: state);
        }
        else
        {
            ProjectInventory(state: state);
        }
    }

    /// <summary>Closes acceptance before native rows and owned atlas views are retired.</summary>
    internal void Close()
    {
        _gateway.RequireThread();
        _accepting = false;
        _generation = checked(_generation + 1);
        Clear();
    }

    private static readonly string[] _paths = ["content/ui/inventory.json", "content/config/items.json",
        "content/packs/ui-identity.json", "content/packs/items-01-10.json", "content/packs/items-11-24.json",
        "assets/equipment/items-01-10.png", "assets/equipment/items-11-24.png", "assets/ui/InspectionTheme.tres",
        "assets/ui/LiberationSans.ttf", "assets/ui/PerfectDOSVGA437.ttf", "assets/ui/Fallback.ttf",
        "assets/common/donor-kit.png"];
    private static readonly string[] _statuses = ["normal", "empty", "broken", "unbound", "reference-lost", "full",
        "invalid-fit", "over-depth", "world-drop", "transfer-success", "transfer-failure", "loading", "error",
        "partial", "overflow", "one", "long-text"];
    private readonly bool _legacy;
    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly JsonElement _pack;
    private readonly JsonElement _data;
    private readonly JsonElement _definitions;
    private readonly JsonElement _icons;
    private readonly List<Node> _owned = [];
    private readonly List<AtlasTexture> _textures = [];
    private readonly List<StyleBoxFlat> _styles = [];
    private bool _accepting = true;
    private long _generation;

    private static bool Same(string left, string right)
    {
        return StringComparer.Ordinal.Equals(x: left, y: right);
    }

    private static string Text(JsonElement value, string key)
    {
        string text = value.GetProperty(propertyName: key).GetString()
                ?? throw new InvalidDataException(message: "INVENTORY_TEXT " + key);
        if (text.Length is 0 or > 4096)
        {
            throw new InvalidDataException(message: "INVENTORY_TEXT " + key);
        }

        return text;
    }

    private static void CheckValue(JsonElement value, ref int count)
    {
        if (++count > 100000)
        {
            throw new InvalidDataException(message: "INVENTORY_DOCUMENT_COUNT");
        }

        if (value.ValueKind == JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                if (!keys.Add(item: property.Name) || property.Name is "__proto__" or "constructor" or "prototype")
                {
                    throw new InvalidDataException(message: "INVENTORY_DOCUMENT_KEY");
                }

                CheckValue(value: property.Value, count: ref count);
            }
        }
        else if (value.ValueKind == JsonValueKind.Array)
        {
            if (value.GetArrayLength() > 4096)
            {
                throw new InvalidDataException(message: "INVENTORY_ARRAY_COUNT");
            }

            foreach (JsonElement child in value.EnumerateArray())
            {
                CheckValue(value: child, count: ref count);
            }
        }
        else if (value.ValueKind == JsonValueKind.String)
        {
            string text = value.GetString() ?? "";
            if (text.Length > 4096 || !text.IsNormalized(normalizationForm: NormalizationForm.FormC)
                    || text.Any(predicate: character => char.IsControl(c: character)))
            {
                throw new InvalidDataException(message: "INVENTORY_ENCODING");
            }
        }
        else if (value.ValueKind == JsonValueKind.Number && !double.IsFinite(d: value.GetDouble()))
        {
            throw new InvalidDataException(message: "INVENTORY_NUMBER");
        }
    }

    private void Bind(byte[][] inputs)
    {
        JsonElement bindings = _pack.GetProperty(propertyName: "bindings");
        if (!Same(left: Text(value: _pack, key: "id"), right: "ui-inventory")
                && !_legacy
                || !_pack.GetProperty(propertyName: "sampleDataOnly").GetBoolean()
                || bindings.GetArrayLength() != _paths.Length || inputs.Length != _paths.Length)
        {
            throw new InvalidDataException(message: "INVENTORY_BINDINGS");
        }

        for (int index = 0; index < _paths.Length; index++)
        {
            string digest = Convert.ToHexStringLower(inArray: SHA256.HashData(source: inputs[index]));
            if (!Same(left: Text(value: bindings[index], key: "path"), right: _legacy && index == 0
                    ? "content/ui/legacy-stash-targets.json" : _paths[index])
                    || !Same(left: Text(value: bindings[index], key: "sha256"), right: digest))
            {
                throw new InvalidDataException(message: "INVENTORY_BINDING_HASH " + _paths[index]);
            }
        }
    }

    private static JsonElement Find(JsonElement rows, string id)
    {
        JsonElement[] matches = rows.EnumerateArray().Where(predicate: row =>
                Same(left: Text(value: row, key: "id"), right: id)).ToArray();
        if (matches.Length != 1)
        {
            throw new InvalidDataException(message: "INVENTORY_REFERENCE " + id);
        }

        return matches[0];
    }

    private static void Unique(JsonElement rows)
    {
        if (rows.GetArrayLength() > 256 || rows.EnumerateArray().Select(selector: row => Text(value: row, key: "id"))
                .Distinct(comparer: StringComparer.Ordinal).Count() != rows.GetArrayLength())
        {
            throw new InvalidDataException(message: "INVENTORY_DUPLICATE_ID");
        }
    }

    private void ValidateIcons(JsonElement weapons, JsonElement clothing)
    {
        Unique(rows: _definitions);
        Unique(rows: _icons);
        if (_definitions.GetArrayLength() != 24 || _icons.GetArrayLength() != 24)
        {
            throw new InvalidDataException(message: "INVENTORY_ITEM_ROSTER");
        }

        foreach (JsonElement definition in _definitions.EnumerateArray())
        {
            string id = Text(value: definition, key: "id");
            JsonElement icon = Find(rows: _icons, id: id);
            ValidateDefinition(definition: definition);
            JsonElement pack = int.Parse(s: id.AsSpan(start: 3), provider: CultureInfo.InvariantCulture) <= 10
                    ? weapons : clothing;
            JsonElement item = Find(rows: pack.GetProperty(propertyName: "items"), id: id);
            string key = Text(value: item, key: "canonicalIcon");
            JsonElement part = pack.GetProperty(propertyName: "parts").EnumerateArray().Single(predicate: row =>
                    Same(left: Text(value: row, key: "key"), right: key));
            ValidateRegion(icon: icon, pack: pack, first: int.Parse(s: id.AsSpan(start: 3),
                    provider: CultureInfo.InvariantCulture) <= 10);
            if (!Same(left: Text(value: icon, key: "icon"), right: key)
                    || !Same(left: Text(value: definition.GetProperty(propertyName: "appearance"),
                            key: "canonicalIcon"), right: key)
                    || !Same(left: Text(value: icon, key: "texture"),
                            right: Text(value: pack.GetProperty(propertyName: "atlas"), key: "path"))
                    || !Same(left: icon.GetProperty(propertyName: "atlasRect").ToString(),
                            right: part.GetProperty(propertyName: "atlasRect").ToString())
                    || !Same(left: icon.GetProperty(propertyName: "maximum").ToString(),
                            right: definition.GetProperty(propertyName: "conditionMaximum").ToString()))
            {
                throw new InvalidDataException(message: "INVENTORY_CANONICAL_ICON " + id);
            }
        }
    }

    private static void ValidateDefinition(JsonElement definition)
    {
        JsonElement footprint = definition.GetProperty(propertyName: "footprint");
        if (footprint.GetArrayLength() != 2
                || definition.GetProperty(propertyName: "maxStack").GetInt32() is < 1 or > 1000
                || definition.GetProperty(propertyName: "weightKilograms").GetDecimal() is < 0 or > 1000)
        {
            throw new InvalidDataException(message: "INVENTORY_DEFINITION_DOMAIN");
        }

        Footprint(width: footprint[0].GetInt32(), height: footprint[1].GetInt32(), rotated: false);
        JsonElement maximum = definition.GetProperty(propertyName: "conditionMaximum");
        if (maximum.ValueKind != JsonValueKind.Null && maximum.GetInt32() is < 1 or > 1000000)
        {
            throw new InvalidDataException(message: "INVENTORY_DEFINITION_DOMAIN");
        }
    }

    private static void ValidateRegion(JsonElement icon, JsonElement pack, bool first)
    {
        JsonElement atlas = pack.GetProperty(propertyName: "atlas");
        string path = first ? "assets/equipment/items-01-10.png" : "assets/equipment/items-11-24.png";
        int[] rect = icon.GetProperty(propertyName: "atlasRect").EnumerateArray()
                .Select(selector: value => value.GetInt32()).ToArray();
        if (!Same(left: Text(value: atlas, key: "path"), right: path) || rect.Length != 4
                || rect[0] < 0 || rect[1] < 0 || rect[2] < 1 || rect[3] < 1
                || (long)rect[0] + rect[2] > atlas.GetProperty(propertyName: "width").GetInt32()
                || (long)rect[1] + rect[3] > atlas.GetProperty(propertyName: "height").GetInt32())
        {
            throw new InvalidDataException(message: "INVENTORY_ICON_BOUNDS");
        }
    }

    private void ValidateData()
    {
        if (_legacy)
        {
            ValidateLegacyData();
            return;
        }

        if (!Same(left: Text(value: _data, key: "id"), right: "inventory-layouts")
                || _data.GetProperty(propertyName: "version").GetInt32() != 1)
        {
            throw new InvalidDataException(message: "INVENTORY_DATA_ID");
        }

        JsonElement samples = _data.GetProperty(propertyName: "samples");
        Unique(rows: samples);
        if (samples.GetArrayLength() is < 1 or > 64)
        {
            throw new InvalidDataException(message: "INVENTORY_SAMPLE_COUNT");
        }

        foreach (JsonElement state in samples.EnumerateArray())
        {
            ValidateState(state: state);
        }
    }

    private void ValidateState(JsonElement state)
    {
        Text(value: state, key: "id");
        Text(value: state, key: "name");
        Text(value: state, key: "warning");
        string status = Text(value: state, key: "status");
        if (!_statuses.Contains(value: status, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "INVENTORY_STATUS");
        }

        JsonElement items = state.GetProperty(propertyName: "items");
        JsonElement grids = state.GetProperty(propertyName: "grids");
        Unique(rows: items);
        Unique(rows: grids);
        foreach (JsonElement item in items.EnumerateArray())
        {
            ValidateItem(item: item, grids: grids);
        }

        foreach (JsonElement grid in grids.EnumerateArray())
        {
            ValidateGrid(grid: grid, items: items);
        }

        ValidateSlots(state: state);
        ValidateOwnership(state: state);
        ValidateActions(state: state);
        ValidateHud(hud: state.GetProperty(propertyName: "hud"));
    }

    private void ValidateItem(JsonElement item, JsonElement grids)
    {
        Text(value: item, key: "label");
        JsonElement definition = Find(rows: _definitions, id: Text(value: item, key: "definition"));
        int quantity = item.GetProperty(propertyName: "quantity").GetInt32();
        JsonElement maximum = definition.GetProperty(propertyName: "conditionMaximum");
        JsonElement current = item.GetProperty(propertyName: "current");
        if (quantity < 1 || quantity > definition.GetProperty(propertyName: "maxStack").GetInt32()
                || (maximum.ValueKind == JsonValueKind.Null ? current.ValueKind != JsonValueKind.Null
                        : !current.TryGetInt32(value: out int number) || number < 0 || number > maximum.GetInt32()))
        {
            throw new InvalidDataException(message: "INVENTORY_ITEM_RANGE");
        }

        JsonElement gridId = item.GetProperty(propertyName: "grid");
        int x = item.GetProperty(propertyName: "x").GetInt32();
        int y = item.GetProperty(propertyName: "y").GetInt32();
        int[] size = ItemSize(item: item);
        if (gridId.ValueKind != JsonValueKind.Null)
        {
            JsonElement grid = Find(rows: grids, id: gridId.GetString() ?? "");
            if (x < 0 || y < 0 || x + size[0] > grid.GetProperty(propertyName: "columns").GetInt32()
                    || y + size[1] > grid.GetProperty(propertyName: "rows").GetInt32())
            {
                throw new InvalidDataException(message: "INVENTORY_ITEM_BOUNDS");
            }
        }
        else if (x != 0 || y != 0)
        {
            throw new InvalidDataException(message: "INVENTORY_EQUIPPED_POSITION");
        }
    }

    private void ValidateGrid(JsonElement grid, JsonElement items)
    {
        Text(value: grid, key: "title");
        int columns = grid.GetProperty(propertyName: "columns").GetInt32();
        int rows = grid.GetProperty(propertyName: "rows").GetInt32();
        int depth = grid.GetProperty(propertyName: "depth").GetInt32();
        JsonElement owner = Find(rows: items, id: Text(value: grid, key: "source"));
        JsonElement storage = Find(rows: _definitions, id: Text(value: owner, key: "definition"))
                .GetProperty(propertyName: "storage");
        if (columns is < 1 or > 16 || rows is < 1 or > 16 || depth is < 1 or > 3
                || storage.ValueKind == JsonValueKind.Null || columns != storage[0].GetInt32()
                || rows != storage[1].GetInt32()
                || grid.GetProperty(propertyName: "breadcrumbs").GetArrayLength() != depth)
        {
            throw new InvalidDataException(message: "INVENTORY_GRID_DOMAIN");
        }

        var occupied = new HashSet<int>();
        foreach (JsonElement item in items.EnumerateArray().Where(predicate: item => Same(left:
                item.GetProperty(propertyName: "grid").ToString(), right: Text(value: grid, key: "id"))))
        {
            int[] size = ItemSize(item: item);
            for (int y = 0; y < size[1]; y++)
            {
                for (int x = 0; x < size[0]; x++)
                {
                    if (!occupied.Add(item: (y + item.GetProperty(propertyName: "y").GetInt32()) * columns
                            + x + item.GetProperty(propertyName: "x").GetInt32()))
                    {
                        throw new InvalidDataException(message: "INVENTORY_ITEM_OVERLAP");
                    }
                }
            }
        }
    }

    private void ValidateSlots(JsonElement state)
    {
        JsonElement slots = state.GetProperty(propertyName: "slots");
        Unique(rows: slots);
        if (!slots.EnumerateArray().Select(selector: slot => Text(value: slot, key: "id"))
                .Order(comparer: StringComparer.Ordinal).SequenceEqual(second: SlotIds()
                        .Order(comparer: StringComparer.Ordinal), comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "INVENTORY_SLOTS");
        }

        var equipped = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement slot in slots.EnumerateArray())
        {
            JsonElement reference = slot.GetProperty(propertyName: "item");
            if (reference.ValueKind == JsonValueKind.Null)
            {
                continue;
            }

            string id = reference.GetString() ?? "";
            JsonElement item = Find(rows: state.GetProperty(propertyName: "items"), id: id);
            JsonElement definition = Find(rows: _definitions, id: Text(value: item, key: "definition"));
            string required = definition.GetProperty(propertyName: "slot").ToString();
            required = required is "torso-clothing" ? "torso" : required;
            string actual = Text(value: slot, key: "id");
            if (!equipped.Add(item: id) || item.GetProperty(propertyName: "grid").ValueKind != JsonValueKind.Null
                    || !(Same(left: required, right: actual) || required is "either-logical-hand"
                            && actual is "left-hand" or "right-hand"))
            {
                throw new InvalidDataException(message: "INVENTORY_SLOT_ITEM");
            }
        }

        JsonElement quick = state.GetProperty(propertyName: "quick");
        if (quick.GetArrayLength() != 4 || quick.EnumerateArray().Any(predicate: reference =>
                reference.ValueKind is not (JsonValueKind.Null or JsonValueKind.String)))
        {
            throw new InvalidDataException(message: "INVENTORY_QUICK");
        }

        for (int index = 0; index < 4; index++)
        {
            JsonElement? item = ResolveQuick(state: state, index: index);
            if (item.HasValue && (item.Value.GetProperty(propertyName: "grid").ValueKind == JsonValueKind.Null
                    || Text(value: item.Value, key: "definition") is not ("IT-08" or "IT-09" or "IT-10")))
            {
                throw new InvalidDataException(message: "INVENTORY_QUICK_FOOD");
            }
        }
    }

    private static void ValidateHud(JsonElement hud)
    {
        foreach (string field in new[] { "health", "stamina" })
        {
            int value = hud.GetProperty(propertyName: field).GetInt32();
            if (value is < 0 or > 100)
            {
                throw new InvalidDataException(message: "INVENTORY_HUD_RANGE");
            }
        }

        if (hud.GetProperty(propertyName: "day").GetInt32() is < 1 or > 1000000
                || hud.GetProperty(propertyName: "hour").GetInt32() is < 0 or > 23
                || Text(value: hud, key: "phase") is not ("Day" or "Night")
                || Text(value: hud, key: "load") is not ("Light" or "Burdened" or "Overloaded")
                || hud.GetProperty(propertyName: "teammates").GetArrayLength() != 4)
        {
            throw new InvalidDataException(message: "INVENTORY_HUD_DOMAIN");
        }
    }

    private static void ValidateOwnership(JsonElement state)
    {
        JsonElement items = state.GetProperty(propertyName: "items");
        JsonElement grids = state.GetProperty(propertyName: "grids");
        foreach (JsonElement item in items.EnumerateArray())
        {
            if (item.GetProperty(propertyName: "grid").ValueKind == JsonValueKind.Null
                    && !state.GetProperty(propertyName: "slots").EnumerateArray().Any(predicate: slot => Same(
                            left: slot.GetProperty(propertyName: "item").ToString(),
                            right: Text(value: item, key: "id"))))
            {
                throw new InvalidDataException(message: "INVENTORY_ORPHAN_ITEM");
            }
        }

        var sources = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement grid in grids.EnumerateArray())
        {
            string source = Text(value: grid, key: "source");
            JsonElement owner = Find(rows: items, id: source);
            string parent = owner.GetProperty(propertyName: "grid").ToString();
            int expected = parent.Length == 0 ? 1 : Find(rows: grids, id: parent)
                    .GetProperty(propertyName: "depth").GetInt32() + 1;
            if (!sources.Add(item: source) || Same(left: parent, right: Text(value: grid, key: "id"))
                    || grid.GetProperty(propertyName: "depth").GetInt32() != expected)
            {
                throw new InvalidDataException(message: "INVENTORY_CONTAINER_PATH");
            }
        }

        string[] held = state.GetProperty(propertyName: "slots").EnumerateArray().Where(predicate: slot =>
                Text(value: slot, key: "id") is "left-hand" or "right-hand")
                .Select(selector: slot => slot.GetProperty(propertyName: "item").ToString())
                .Where(predicate: id => id.Length > 0).Select(selector: id => Text(value: Find(rows: items, id: id),
                        key: "definition")).ToArray();
        if (held.Contains(value: "IT-07", comparer: StringComparer.Ordinal)
                && held.Any(predicate: id => id is not ("IT-07" or "IT-02")))
        {
            throw new InvalidDataException(message: "INVENTORY_BOW_HANDS");
        }
    }

    private static void ValidateActions(JsonElement state)
    {
        JsonElement drag = state.GetProperty(propertyName: "drag");
        if (drag.ValueKind != JsonValueKind.Null)
        {
            Find(rows: state.GetProperty(propertyName: "items"), id: Text(value: drag, key: "item"));
            Find(rows: state.GetProperty(propertyName: "grids"), id: Text(value: drag, key: "grid"));
            drag.GetProperty(propertyName: "rotated").GetBoolean();
            if (drag.GetProperty(propertyName: "depth").GetInt32() is < 1 or > 8
                    || drag.GetProperty(propertyName: "x").GetInt32() is < 0 or > 16
                    || drag.GetProperty(propertyName: "y").GetInt32() is < 0 or > 16)
            {
                throw new InvalidDataException(message: "INVENTORY_DRAG_DOMAIN");
            }
        }

        JsonElement split = state.GetProperty(propertyName: "split");
        if (split.ValueKind != JsonValueKind.Null)
        {
            JsonElement item = Find(rows: state.GetProperty(propertyName: "items"),
                    id: Text(value: split, key: "item"));
            if (split.GetProperty(propertyName: "amount").GetInt32() < 1
                    || split.GetProperty(propertyName: "amount").GetInt32()
                            >= item.GetProperty(propertyName: "quantity").GetInt32())
            {
                throw new InvalidDataException(message: "INVENTORY_SPLIT_DOMAIN");
            }
        }

        Text(value: state, key: "transfer");
        Text(value: state, key: "worldDrop");
    }

    private int[] ItemSize(JsonElement item)
    {
        JsonElement definition = Find(rows: _definitions, id: Text(value: item, key: "definition"));
        JsonElement footprint = definition.GetProperty(propertyName: "footprint");
        return Footprint(width: footprint[0].GetInt32(), height: footprint[1].GetInt32(),
                rotated: item.GetProperty(propertyName: "rotated").GetBoolean());
    }

    private bool Fits(JsonElement state)
    {
        JsonElement drag = state.GetProperty(propertyName: "drag");
        JsonElement item = Find(rows: state.GetProperty(propertyName: "items"), id: Text(value: drag, key: "item"));
        JsonElement grid = Find(rows: state.GetProperty(propertyName: "grids"), id: Text(value: drag, key: "grid"));
        JsonElement size = Find(rows: _definitions, id: Text(value: item, key: "definition"))
                .GetProperty(propertyName: "footprint");
        int[] rotated = Footprint(width: size[0].GetInt32(), height: size[1].GetInt32(),
                rotated: drag.GetProperty(propertyName: "rotated").GetBoolean());
        int x = drag.GetProperty(propertyName: "x").GetInt32();
        int y = drag.GetProperty(propertyName: "y").GetInt32();
        if (drag.GetProperty(propertyName: "depth").GetInt32() > 3
                || x + rotated[0] > grid.GetProperty(propertyName: "columns").GetInt32()
                || y + rotated[1] > grid.GetProperty(propertyName: "rows").GetInt32())
        {
            return false;
        }

        foreach (JsonElement other in state.GetProperty(propertyName: "items").EnumerateArray())
        {
            if (!Same(left: Text(value: other, key: "id"), right: Text(value: item, key: "id"))
                    && Same(left: other.GetProperty(propertyName: "grid").ToString(),
                            right: Text(value: grid, key: "id")))
            {
                int[] otherSize = ItemSize(item: other);
                int otherX = other.GetProperty(propertyName: "x").GetInt32();
                int otherY = other.GetProperty(propertyName: "y").GetInt32();
                if (x < otherX + otherSize[0] && x + rotated[0] > otherX
                        && y < otherY + otherSize[1] && y + rotated[1] > otherY)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void Attach(JsonElement state)
    {
        _gateway.RequireThread();
        var panel = new PanelContainer { Name = "Inventory", Position = new Vector2(x: 0, y: 80),
                Size = new Vector2(x: 1168, y: 600), Theme = ResourceLoader.Load<Theme>(
                        path: "res://assets/ui/InspectionTheme.tres") };
        var scroll = new ScrollContainer { Name = "Scroll" };
        var content = new VBoxContainer { Name = "Content", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        _root.AddChild(node: panel);
        _owned.Add(item: panel);
        panel.AddChild(node: scroll);
        scroll.AddChild(node: content);
        AddLabel(parent: content, name: "Title", text: Text(value: state, key: "name") + " · "
                + Text(value: state, key: "id"));
        AddLabel(parent: content, name: "Scope", text: "Layout example — gameplay actions are not active.");
        var warning = new Label { Name = "Warning", Text = "World continues. " + Text(value: state, key: "warning"),
                ZIndex = 100, AutowrapMode = TextServer.AutowrapMode.WordSmart,
                Size = new Vector2(x: 1168, y: 76), Theme = panel.Theme };
        _root.AddChild(node: warning);
        _owned.Add(item: warning);
    }

    private void ProjectInventory(JsonElement state)
    {
        _gateway.RequireThread();
        Node content = _root.GetNode(path: "Inventory/Scroll/Content");
        JsonElement hud = state.GetProperty(propertyName: "hud");
        AddLabel(parent: content, name: "HUD", text: "Health " + hud.GetProperty(propertyName: "health")
                + " / 100 · Stamina " + hud.GetProperty(propertyName: "stamina") + " / 100 · Day "
                + hud.GetProperty(propertyName: "day") + " · " + hud.GetProperty(propertyName: "hour")
                + ":00 · " + Text(value: hud, key: "phase") + " · " + Text(value: hud, key: "load"));
        decimal weight = 0;
        int money = 0;
        foreach (JsonElement item in state.GetProperty(propertyName: "items").EnumerateArray())
        {
            JsonElement definition = Find(rows: _definitions, id: Text(value: item, key: "definition"));
            weight += definition.GetProperty(propertyName: "weightKilograms").GetDecimal()
                    * item.GetProperty(propertyName: "quantity").GetInt32();
            money += Text(value: item, key: "definition") is "IT-01"
                    ? item.GetProperty(propertyName: "quantity").GetInt32() : 0;
        }

        AddLabel(parent: content, name: "Weight", text: "Carried weight " + weight.ToString(
                provider: CultureInfo.InvariantCulture) + " kg · Money "
                + money.ToString(provider: CultureInfo.InvariantCulture));
        AddLabel(parent: content, name: "Teammates", text: string.Join(separator: " · ", values:
                hud.GetProperty(propertyName: "teammates").EnumerateArray()
                        .Select(selector: value => value.GetString())));
        ProjectSlots(state: state, content: content);
        ProjectGrids(state: state, content: content);
        ProjectActions(state: state, content: content);
    }

    private void ProjectSlots(JsonElement state, Node content)
    {
        _gateway.RequireThread();
        var slots = new VBoxContainer { Name = "Slots" };
        content.AddChild(node: slots);
        foreach (JsonElement slot in state.GetProperty(propertyName: "slots").EnumerateArray())
        {
            string id = Text(value: slot, key: "id");
            var row = new HBoxContainer { Name = id };
            slots.AddChild(node: row);
            row.SetMeta(name: "resource_id", value: id);
            AddLabel(parent: row, name: "Location", text: id);
            JsonElement reference = slot.GetProperty(propertyName: "item");
            if (reference.ValueKind == JsonValueKind.Null)
            {
                AddLabel(parent: row, name: "Empty", text: "Empty");
            }
            else
            {
                JsonElement item = Find(rows: state.GetProperty(propertyName: "items"),
                        id: reference.GetString() ?? "");
                AddIcon(parent: row, item: item, rotated: false);
                AddLabel(parent: row, name: "Item", text: Details(item: item));
            }
        }

        var quick = new VBoxContainer { Name = "Quick" };
        content.AddChild(node: quick);
        for (int index = 0; index < 4; index++)
        {
            JsonElement? item = ResolveQuick(state: state, index: index);
            string text = item.HasValue ? Details(item: item.Value)
                    : state.GetProperty(propertyName: "quick")[index].ValueKind == JsonValueKind.Null
                            ? "Unbound" : "Reference lost — exact stack no longer carried";
            AddLabel(parent: quick, name: "Quick" + (index + 1).ToString(provider: CultureInfo.InvariantCulture),
                    text: "Food reference " + (index + 1).ToString(provider: CultureInfo.InvariantCulture)
                            + " · " + text);
        }
    }

    private void ProjectGrids(JsonElement state, Node content)
    {
        _gateway.RequireThread();
        var grids = new VBoxContainer { Name = "Grids" };
        content.AddChild(node: grids);
        foreach (JsonElement grid in state.GetProperty(propertyName: "grids").EnumerateArray())
        {
            var section = new VBoxContainer { Name = Text(value: grid, key: "id") };
            grids.AddChild(node: section);
            section.SetMeta(name: "resource_id", value: Text(value: grid, key: "id"));
            AddLabel(parent: section, name: "Title", text: Text(value: grid, key: "title") + " · "
                    + Text(value: grid, key: "source") + " · Depth " + grid.GetProperty(propertyName: "depth")
                    + " · " + string.Join(separator: " > ", values: grid.GetProperty(propertyName: "breadcrumbs")
                            .EnumerateArray().Select(selector: value => value.GetString())));
            int columns = grid.GetProperty(propertyName: "columns").GetInt32();
            int rows = grid.GetProperty(propertyName: "rows").GetInt32();
            var surface = new Control { Name = "Cells",
                    CustomMinimumSize = new Vector2(x: columns * 40, y: rows * 40) };
            section.AddChild(node: surface);
            AddCells(parent: surface, columns: columns, rows: rows);
            foreach (JsonElement item in state.GetProperty(propertyName: "items").EnumerateArray().Where(
                    predicate: item => Same(left: item.GetProperty(propertyName: "grid").ToString(),
                            right: Text(value: grid, key: "id"))))
            {
                AddPlaced(parent: surface, item: item);
                AddLabel(parent: section, name: "Details" + Text(value: item, key: "id"), text: Details(item: item));
            }
        }

        int count = state.GetProperty(propertyName: "items").GetArrayLength();
        AddLabel(parent: content, name: "Collection", text: count.ToString(provider: CultureInfo.InvariantCulture)
                + (count == 1 ? " item" : " items") + " · " + Text(value: state, key: "status"));
    }

    private void AddCells(Control parent, int columns, int rows)
    {
        _gateway.RequireThread();
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                parent.AddChild(node: new ColorRect { Name = "Cell" + x.ToString(provider: CultureInfo.InvariantCulture)
                        + "_" + y.ToString(provider: CultureInfo.InvariantCulture),
                        Position = new Vector2(x: x * 40, y: y * 40), Size = new Vector2(x: 39, y: 39),
                        Color = Color.FromHtml(rgba: "#35433F"), MouseFilter = Control.MouseFilterEnum.Ignore });
            }
        }
    }

    private void AddPlaced(Control parent, JsonElement item)
    {
        _gateway.RequireThread();
        int[] size = ItemSize(item: item);
        var box = new Control { Name = Text(value: item, key: "id"),
                Position = new Vector2(x: item.GetProperty(propertyName: "x").GetInt32() * 40,
                        y: item.GetProperty(propertyName: "y").GetInt32() * 40),
                Size = new Vector2(x: size[0] * 40, y: size[1] * 40), TooltipText = Details(item: item), ZIndex = 1 };
        parent.AddChild(node: box);
        box.SetMeta(name: "resource_id", value: Text(value: item, key: "id"));
        AddIcon(parent: box, item: item, rotated: item.GetProperty(propertyName: "rotated").GetBoolean());
        AddLabel(parent: box, name: "Count", text: "×" + item.GetProperty(propertyName: "quantity"));
    }

    private void AddIcon(Node parent, JsonElement item, bool rotated)
    {
        _gateway.RequireThread();
        JsonElement icon = Find(rows: _icons, id: Text(value: item, key: "definition"));
        int[] rect = icon.GetProperty(propertyName: "atlasRect").EnumerateArray()
                .Select(selector: value => value.GetInt32()).ToArray();
        var texture = new AtlasTexture { Atlas = ResourceLoader.Load<Texture2D>(path: "res://"
                + Text(value: icon, key: "texture")), Region = new Rect2(x: rect[0], y: rect[1], width: rect[2],
                        height: rect[3]), FilterClip = true };
        _textures.Add(item: texture);
        var image = new TextureRect { Name = "Icon", Texture = texture, Size = new Vector2(x: 32, y: 32),
                CustomMinimumSize = new Vector2(x: 32, y: 32), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest, RotationDegrees = rotated ? 90 : 0,
                PivotOffset = new Vector2(x: 16, y: 16), MouseFilter = Control.MouseFilterEnum.Ignore };
        image.SetMeta(name: "canonical_icon", value: Text(value: icon, key: "icon"));
        parent.AddChild(node: image);
    }

    private string Details(JsonElement item)
    {
        JsonElement definition = Find(rows: _definitions, id: Text(value: item, key: "definition"));
        int[] size = ItemSize(item: item);
        JsonElement maximum = definition.GetProperty(propertyName: "conditionMaximum");
        string condition = maximum.ValueKind == JsonValueKind.Null ? "No condition wear"
                : item.GetProperty(propertyName: "current") + " / " + maximum
                        + (item.GetProperty(propertyName: "current").GetInt32() == 0 ? " · X Broken" : " · Usable");
        return Text(value: item, key: "label") + " · " + Text(value: item, key: "id") + " · "
                + Text(value: item, key: "definition") + " · Quantity " + item.GetProperty(propertyName: "quantity")
                + " / " + definition.GetProperty(propertyName: "maxStack") + " · " + size[0].ToString(
                        provider: CultureInfo.InvariantCulture) + "×"
                + size[1].ToString(provider: CultureInfo.InvariantCulture)
                + " cells · " + condition + " · Unit weight " + definition.GetProperty(propertyName: "weightKilograms")
                + " kg · Slot " + definition.GetProperty(propertyName: "slot") + " · Storage "
                + definition.GetProperty(propertyName: "storage") + " · Location "
                + (item.GetProperty(propertyName: "grid").ValueKind == JsonValueKind.Null ? "Equipped"
                        : item.GetProperty(propertyName: "grid").ToString()) + " · "
                + Text(value: definition, key: "descriptionKey");
    }

    private void ProjectActions(JsonElement state, Node content)
    {
        _gateway.RequireThread();
        JsonElement drag = state.GetProperty(propertyName: "drag");
        if (drag.ValueKind != JsonValueKind.Null)
        {
            JsonElement item = Find(rows: state.GetProperty(propertyName: "items"), id: Text(value: drag, key: "item"));
            var ghost = new HBoxContainer { Name = "DragGhost" };
            content.AddChild(node: ghost);
            bool rotated = drag.GetProperty(propertyName: "rotated").GetBoolean();
            JsonElement definition = Find(rows: _definitions, id: Text(value: item, key: "definition"));
            JsonElement footprint = definition.GetProperty(propertyName: "footprint");
            int[] size = Footprint(width: footprint[0].GetInt32(), height: footprint[1].GetInt32(), rotated: rotated);
            var layout = new Control { Name = "Footprint", CustomMinimumSize = new Vector2(x: size[0] * 40,
                    y: size[1] * 40), SizeFlagsVertical = Control.SizeFlags.ShrinkCenter };
            ghost.AddChild(node: layout);
            layout.SetMeta(name: "resource_id", value: Text(value: item, key: "id"));
            AddIcon(parent: layout, item: item, rotated: rotated);
            layout.GetNode<TextureRect>(path: "Icon").Position = new Vector2(x: size[0] * 20 - 16,
                    y: size[1] * 20 - 16);
            AddLabel(parent: ghost, name: "Fit", text: (Fits(state: state) ? "Valid fit" : "Invalid fit")
                    + " · " + Text(value: drag, key: "grid") + " · Depth " + drag.GetProperty(propertyName: "depth")
                    + " · Preview only; original item remains in place");
        }

        JsonElement split = state.GetProperty(propertyName: "split");
        if (split.ValueKind != JsonValueKind.Null)
        {
            AddLabel(parent: content, name: "Split", text: "Split preview · " + Text(value: split, key: "item")
                    + " · Move " + split.GetProperty(propertyName: "amount") + " · No quantities changed");
        }

        AddLabel(parent: content, name: "Transfer", text: Text(value: state, key: "transfer"));
        AddLabel(parent: content, name: "WorldDrop", text: Text(value: state, key: "worldDrop"));
        AddLabel(parent: content, name: "Actions", text: "Rotate · Split · Transfer · Assign food reference"
                + " · Drop to world"
                + " — layout samples only");
    }

    private void AddLabel(Node parent, string name, string text)
    {
        _gateway.RequireThread();
        parent.AddChild(node: new Label { Name = name, Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });
    }


    private void ValidateLegacyData()
    {
        if (!Same(left: Text(value: _data, key: "id"), right: "legacy-stash-target-layouts")
                || _data.GetProperty(propertyName: "version").GetInt32() != 1)
        {
            throw new InvalidDataException(message: "LEGACY_DATA_ID");
        }

        JsonElement samples = _data.GetProperty(propertyName: "samples");
        Unique(rows: samples);
        if (samples.GetArrayLength() is < 1 or > 64)
        {
            throw new InvalidDataException(message: "LEGACY_SAMPLE_COUNT");
        }

        foreach (JsonElement state in samples.EnumerateArray())
        {
            ValidateLegacyState(state: state);
        }
    }

    private void ValidateLegacyState(JsonElement state)
    {
        Text(value: state, key: "id");
        Text(value: state, key: "name");
        Text(value: state, key: "warning");
        Text(value: state, key: "status");
        string kind = Text(value: state, key: "kind");
        if (kind is not ("legacy" or "stash" or "target"))
        {
            throw new InvalidDataException(message: "LEGACY_KIND");
        }

        ValidateState(state: state.GetProperty(propertyName: "character"));
        ValidateHistoric(grid: SavedLayout(sample: state), missing: kind is "legacy"
                && Same(left: Text(value: state, key: "status"), right: "missing-definition"));
        ValidateCurrentGrid(grid: state.GetProperty(propertyName: "currentDestination"));
        Text(value: state, key: "guidance");
        Text(value: state, key: "extraction");
        foreach (JsonElement badge in state.GetProperty(propertyName: "badges").EnumerateArray())
        {
            if (badge.GetString() is not ("Old geometry" or "Incompatible slot" or "Over stack limit"
                    or "Over depth limit" or "Missing definition" or "Current rules adopted"))
            {
                throw new InvalidDataException(message: "LEGACY_BADGE");
            }
        }

        ValidateStash(stash: state.GetProperty(propertyName: "stash"));
        ValidateTargets(state: state);
    }

    private void ValidateHistoric(JsonElement grid, bool missing)
    {
        Text(value: grid, key: "id");
        Text(value: grid, key: "title");
        Text(value: grid, key: "slot");
        int columns = grid.GetProperty(propertyName: "columns").GetInt32();
        int rows = grid.GetProperty(propertyName: "rows").GetInt32();
        int depth = grid.GetProperty(propertyName: "depth").GetInt32();
        if (columns is < 1 or > 16 || rows is < 1 or > 16 || depth is < 1 or > 16)
        {
            throw new InvalidDataException(message: "LEGACY_GRID_DOMAIN");
        }

        JsonElement items = grid.GetProperty(propertyName: "items");
        Unique(rows: items);
        foreach (JsonElement item in items.EnumerateArray())
        {
            Text(value: item, key: "label");
            string definition = Text(value: item, key: "definition");
            if (!missing || !Same(left: definition, right: "IT-99"))
            {
                Find(rows: _definitions, id: definition);
            }

            JsonElement footprint = item.GetProperty(propertyName: "savedFootprint");
            if (footprint.GetArrayLength() != 2 || item.GetProperty(propertyName: "quantity").GetInt32()
                    is < 1 or > 1000000)
            {
                throw new InvalidDataException(message: "LEGACY_ITEM_DOMAIN");
            }

            int[] size = Footprint(width: footprint[0].GetInt32(), height: footprint[1].GetInt32(),
                    rotated: item.GetProperty(propertyName: "rotated").GetBoolean());
            int x = item.GetProperty(propertyName: "x").GetInt32();
            int y = item.GetProperty(propertyName: "y").GetInt32();
            if (x < 0 || y < 0 || x + size[0] > columns || y + size[1] > rows)
            {
                throw new InvalidDataException(message: "LEGACY_ITEM_BOUNDS");
            }
        }
    }


    private void ValidateCurrentGrid(JsonElement grid)
    {
        ValidateHistoric(grid: grid, missing: false);
        JsonElement grids = Read(bytes: JsonSerializer.SerializeToUtf8Bytes(value: (JsonElement[])[grid]));
        var occupied = new HashSet<int>();
        foreach (JsonElement item in grid.GetProperty(propertyName: "items").EnumerateArray())
        {
            ValidateItem(item: item, grids: grids);
            int[] size = ItemSize(item: item);
            for (int y = 0; y < size[1]; y++)
            {
                for (int x = 0; x < size[0]; x++)
                {
                    int cell = (item.GetProperty(propertyName: "y").GetInt32() + y) * 16
                            + item.GetProperty(propertyName: "x").GetInt32() + x;
                    if (!occupied.Add(item: cell))
                    {
                        throw new InvalidDataException(message: "LEGACY_CURRENT_OVERLAP");
                    }
                }
            }
        }
    }

    private void ValidateStash(JsonElement stash)
    {
        Text(value: stash, key: "context");
        Text(value: stash, key: "nameFilter");
        Text(value: stash, key: "typeFilter");
        Text(value: stash, key: "feedback");
        int page = stash.GetProperty(propertyName: "page").GetInt32();
        if (page < 1 || !stash.GetProperty(propertyName: "unlimited").GetBoolean()
                || !stash.GetProperty(propertyName: "nextCreatesPage").GetBoolean())
        {
            throw new InvalidDataException(message: "LEGACY_STASH_PAGES");
        }

        JsonElement pages = stash.GetProperty(propertyName: "pages");
        Unique(rows: pages);
        if (pages.GetArrayLength() is < 1 or > 8)
        {
            throw new InvalidDataException(message: "LEGACY_STASH_PAGES");
        }

        foreach (JsonElement grid in pages.EnumerateArray())
        {
            ValidateCurrentGrid(grid: grid);
        }
    }

    private void ValidateTargets(JsonElement state)
    {
        JsonElement targets = state.GetProperty(propertyName: "targets");
        Unique(rows: targets);
        if (targets.GetArrayLength() > 8)
        {
            throw new InvalidDataException(message: "LEGACY_TARGET_COUNT");
        }

        string selected = Text(value: state, key: "selected");
        foreach (string id in TargetIds(sample: state))
        {
            JsonElement target = Find(rows: targets, id: id);
            Text(value: target, key: "name");
            Text(value: target, key: "feedback");
            if (target.GetProperty(propertyName: "permitted").ValueKind
                    is not (JsonValueKind.True or JsonValueKind.False))
            {
                throw new InvalidDataException(message: "LEGACY_TARGET_PERMISSION");
            }

            string kind = Text(value: target, key: "kind");
            if (kind is not ("npc" or "corpse" or "ground"))
            {
                throw new InvalidDataException(message: "LEGACY_TARGET_KIND");
            }

            ValidateState(state: target.GetProperty(propertyName: "gear"));
            ValidateCurrentGrid(grid: target.GetProperty(propertyName: "contents"));
            foreach (JsonElement value in target.GetProperty(propertyName: "region").EnumerateArray())
            {
                if (!value.TryGetInt32(value: out int number) || number is < 0 or > 2000)
                {
                    throw new InvalidDataException(message: "LEGACY_TARGET_REGION");
                }
            }

            if (target.GetProperty(propertyName: "region").GetArrayLength() != 4
                    || target.GetProperty(propertyName: "region")[2].GetInt32() < 1
                    || target.GetProperty(propertyName: "region")[3].GetInt32() < 1)
            {
                throw new InvalidDataException(message: "LEGACY_TARGET_REGION");
            }
        }

        if (targets.GetArrayLength() > 0)
        {
            Find(rows: targets, id: selected);
        }
    }

    private void ProjectLegacy(JsonElement state)
    {
        _gateway.RequireThread();
        Node content = _root.GetNode(path: "Inventory/Scroll/Content");
        AddLabel(parent: content, name: "CharacterContext", text: "Character · "
                + Text(value: state.GetProperty(propertyName: "character"), key: "id"));
        ProjectInventory(state: state.GetProperty(propertyName: "character"));
        var legacy = new VBoxContainer { Name = "Compatibility" };
        content.AddChild(node: legacy);
        AddLabel(parent: legacy, name: "Notice", text: "Saved layout differs from current rules."
                + " Existing contents are shown in their saved positions.");
        AddLabel(parent: legacy, name: "Badges", text: string.Join(separator: " · ", values:
                state.GetProperty(propertyName: "badges").EnumerateArray()
                        .Select(selector: value => value.GetString())));
        ProjectHistoric(parent: legacy, grid: SavedLayout(sample: state), name: "Saved", current: false);
        ProjectHistoric(parent: legacy, grid: state.GetProperty(propertyName: "currentDestination"),
                name: "CurrentDestination", current: true);
        AddLabel(parent: legacy, name: "Guidance", text: Text(value: state, key: "guidance"));
        AddLabel(parent: legacy, name: "Extraction", text: Text(value: state, key: "extraction"));
        ProjectStash(state: state, content: content);
        ProjectTargets(state: state, content: content);
    }

    private void ProjectHistoric(Node parent, JsonElement grid, string name, bool current)
    {
        _gateway.RequireThread();
        var section = new VBoxContainer { Name = name };
        parent.AddChild(node: section);
        section.SetMeta(name: "resource_id", value: Text(value: grid, key: "id"));
        AddLabel(parent: section, name: "Title", text: (current ? "Current destination" : "Saved layout")
                + " · " + Text(value: grid, key: "title") + " · " + Text(value: grid, key: "id")
                + " · Depth " + grid.GetProperty(propertyName: "depth") + " · Slot "
                + Text(value: grid, key: "slot"));
        var surface = new Control { Name = "Cells", ClipContents = true, CustomMinimumSize = new Vector2(
                x: grid.GetProperty(propertyName: "columns").GetInt32() * 40,
                y: grid.GetProperty(propertyName: "rows").GetInt32() * 40) };
        section.AddChild(node: surface);
        AddCells(parent: surface, columns: grid.GetProperty(propertyName: "columns").GetInt32(),
                rows: grid.GetProperty(propertyName: "rows").GetInt32());
        foreach (JsonElement item in grid.GetProperty(propertyName: "items").EnumerateArray())
        {
            ProjectHistoricItem(parent: surface, item: item, current: current);
        }

        AddLabel(parent: section, name: "Count", text: grid.GetProperty(propertyName: "items").GetArrayLength()
                .ToString(provider: CultureInfo.InvariantCulture) + " items · Contents unchanged by preview");
    }

    private void ProjectHistoricItem(Control parent, JsonElement item, bool current)
    {
        _gateway.RequireThread();
        JsonElement saved = item.GetProperty(propertyName: "savedFootprint");
        bool rotated = item.GetProperty(propertyName: "rotated").GetBoolean();
        int[] size = current ? ItemSize(item: item) : Footprint(width: saved[0].GetInt32(),
                height: saved[1].GetInt32(), rotated: rotated);
        var box = new Control { Name = Text(value: item, key: "id"), ZIndex = 2,
                Position = new Vector2(x: item.GetProperty(propertyName: "x").GetInt32() * 40,
                        y: item.GetProperty(propertyName: "y").GetInt32() * 40),
                Size = new Vector2(x: size[0] * 40, y: size[1] * 40) };
        parent.AddChild(node: box);
        box.SetMeta(name: "resource_id", value: Text(value: item, key: "id"));
        if (Same(left: Text(value: item, key: "definition"), right: "IT-99"))
        {
            AddLabel(parent: box, name: "Missing", text: "Missing definition IT-99 · Original save preserved");
        }
        else
        {
            AddIcon(parent: box, item: item, rotated: rotated);
            box.TooltipText = Details(item: item) + " · Saved footprint " + saved;
        }

        AddLabel(parent: box, name: "Count", text: "×" + item.GetProperty(propertyName: "quantity"));
        AddLabel(parent: box, name: "Identity", text: Text(value: item, key: "label") + " · "
                + Text(value: item, key: "id"));
    }

    private void ProjectStash(JsonElement state, Node content)
    {
        _gateway.RequireThread();
        JsonElement data = state.GetProperty(propertyName: "stash");
        var stash = new VBoxContainer { Name = "Stash" };
        content.AddChild(node: stash);
        AddLabel(parent: stash, name: "Context", text: "Shared stash · " + Text(value: data, key: "context"));
        AddLabel(parent: stash, name: "Pages", text: "Page " + data.GetProperty(propertyName: "page")
                + " · Unlimited pages · Previous page / Next page · Create another page when needed");
        AddLabel(parent: stash, name: "Filters", text: "Visual filter example only · Name: "
                + Text(value: data, key: "nameFilter") + " · Type: " + Text(value: data, key: "typeFilter")
                + " · Gameplay filtering deferred to Phase 12");
        AddLabel(parent: stash, name: "Feedback", text: Text(value: data, key: "feedback"));
        foreach (JsonElement grid in data.GetProperty(propertyName: "pages").EnumerateArray())
        {
            ProjectHistoric(parent: stash, grid: grid, name: Text(value: grid, key: "id"), current: true);
        }
    }

    private void ProjectTargets(JsonElement state, Node content)
    {
        _gateway.RequireThread();
        var targets = new VBoxContainer { Name = "Targets" };
        content.AddChild(node: targets);
        AddLabel(parent: targets, name: "Selector", text: "Overlapping target selector · Selected "
                + Text(value: state, key: "selected"));
        var map = new Control { Name = "Regions", CustomMinimumSize = new Vector2(x: 600, y: 160),
                ClipContents = true };
        targets.AddChild(node: map);
        foreach (JsonElement target in state.GetProperty(propertyName: "targets").EnumerateArray())
        {
            ProjectTargetRegion(parent: map, target: target, selected: Text(value: state, key: "selected"));
            var row = new VBoxContainer { Name = Text(value: target, key: "id") };
            targets.AddChild(node: row);
            row.SetMeta(name: "resource_id", value: Text(value: target, key: "id"));
            AddLabel(parent: row, name: "Identity", text: Text(value: target, key: "name") + " · "
                    + Text(value: target, key: "id") + " · " + Text(value: target, key: "kind"));
            AddLabel(parent: row, name: "Transfer", text: target.GetProperty(propertyName: "permitted").GetBoolean()
                    ? "Transfer preview permitted · No belongings moved" : "Transfer unavailable · "
                            + Text(value: target, key: "feedback"));
            AddLabel(parent: row, name: "Feedback", text: Text(value: target, key: "feedback"));
            ProjectSlots(state: target.GetProperty(propertyName: "gear"), content: row);
            ProjectHistoric(parent: row, grid: target.GetProperty(propertyName: "contents"),
                    name: "Contents", current: true);
        }
    }

    private void ProjectTargetRegion(Control parent, JsonElement target, string selected)
    {
        _gateway.RequireThread();
        JsonElement region = target.GetProperty(propertyName: "region");
        var box = new Panel { Name = Text(value: target, key: "id"),
                Position = new Vector2(x: region[0].GetInt32(), y: region[1].GetInt32()),
                Size = new Vector2(x: region[2].GetInt32(), y: region[3].GetInt32()) };
        parent.AddChild(node: box);
        box.SetMeta(name: "resource_id", value: Text(value: target, key: "id"));
        if (Same(left: Text(value: target, key: "id"), right: selected))
        {
            var outline = new StyleBoxFlat { BgColor = new Color(r: 0, g: 0, b: 0, a: 0),
                    BorderColor = Color.FromHtml(rgba: "#F4F2E8"), BorderWidthLeft = 1, BorderWidthRight = 1,
                    BorderWidthTop = 1, BorderWidthBottom = 1 };
            _styles.Add(item: outline);
            box.AddThemeStyleboxOverride(name: "panel", stylebox: outline);
            AddLabel(parent: box, name: "Marker", text: "Selected · " + Text(value: target, key: "id"));
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
        foreach (StyleBoxFlat style in _styles)
        {
            style.Dispose();
        }

        _styles.Clear();
    }
}
