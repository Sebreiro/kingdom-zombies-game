using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace KingdomZombies.Core;

/// <summary>Reads detached authored settings used to create a new production run.</summary>
internal sealed class RunDefinitions
{
    internal const int MaximumDocumentBytes = 65536;
    internal const int MaximumStartingStacks = 512;

    internal RunDefinitions(Start start, SurvivorSettings survivors, InventorySettings inventory,
            ImmutableDictionary<string, Item> items)
    {
        SurvivorCount = start.SurvivorCount;
        PlayerLimit = start.PlayerLimit;
        InitialRole = start.Role;
        StartingOutfit = start.Outfit;
        StartingStash = start.Stash;
        MaximumHealth = survivors.MaximumHealth;
        MaximumStamina = survivors.MaximumStamina;
        Items = items;
        StashGrid = inventory.StashGrid;
        MaximumContainerLevels = inventory.MaximumContainerLevels;
        QuickSlotCount = inventory.QuickSlotCount;
        Ranges = new RunWorld.Ranges(refugeUnits: survivors.RefugeUnits, dangerUnits: survivors.DangerUnits,
                localUnits: inventory.LocalUnits, stashUnits: inventory.StashUnits);
    }

    internal static ImmutableArray<string> EquipmentSlots { get; } =
    [
        "headwear", "face", "torso-clothing", "legs", "body-armor", "gloves", "boots", "backpack", "belt",
        "left-hand", "right-hand",
    ];
    internal static RunDefinitions Default => _embedded.Value;
    internal int SurvivorCount { get; }
    internal int PlayerLimit { get; }
    internal string InitialRole { get; }
    internal ImmutableArray<string> StartingOutfit { get; }
    internal ImmutableArray<StartingStack> StartingStash { get; }
    internal decimal MaximumHealth { get; }
    internal decimal MaximumStamina { get; }
    internal ImmutableDictionary<string, Item> Items { get; }
    internal Size StashGrid { get; }
    internal int MaximumContainerLevels { get; }
    internal int QuickSlotCount { get; }
    internal RunWorld.Ranges Ranges { get; }

    internal static RunDefinitions Load(Func<string, int, byte[]?> read)
    {
        ArgumentNullException.ThrowIfNull(argument: read);
        InventorySettings inventory = Read(name: "inventory", id: "DT-06", read: read, parse: ReadInventory);
        SurvivorSettings survivors = Read(name: "survivors", id: "DT-03", read: read, parse: ReadSurvivors);
        ImmutableDictionary<string, Item> items = Read(name: "items", id: "DT-04", read: read,
                parse: root => ReadItems(root: root, stash: inventory.StashGrid));
        Start start = Read(name: "run", id: "DT-01", read: read, parse: root => ReadStart(root: root, items: items));
        _ = Read(name: "combat", id: "DT-05", read: read, parse: root => CheckCombat(root: root, items: items));
        return new RunDefinitions(start: start, survivors: survivors, inventory: inventory, items: items);
    }

    private static readonly Lazy<RunDefinitions> _embedded = new(valueFactory: () => Load(read: ReadEmbedded));
    private static readonly ImmutableArray<string?> _itemSlots = [null, "either-logical-hand",
        "either-logical-hand", "either-logical-hand", "either-logical-hand", "either-logical-hand",
        "either-logical-hand", null, null, null, "torso-clothing", "legs", "torso-clothing", "headwear", "face",
        "gloves", "boots", "headwear", "body-armor", "body-armor", "belt", null, "backpack", "backpack"];
    private static readonly ImmutableArray<string> _outfit = ["IT-11", "IT-12", "IT-17"];
    private static readonly UTF8Encoding _utf8 = new(encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: true);
    private static byte[]? ReadEmbedded(string path, int limit)
    {
        using Stream? stream = typeof(RunDefinitions).Assembly.GetManifestResourceStream(name: path);
        if (stream is null || stream.Length is <= 0 || stream.Length > limit)
        {
            return null;
        }

        byte[] bytes = new byte[checked((int)stream.Length)];
        stream.ReadExactly(buffer: bytes);
        return bytes;
    }

    private static T Read<T>(string name, string id, Func<string, int, byte[]?> read, Func<JsonElement, T> parse)
    {
        string path = "content/config/" + name + ".json";
        try
        {
            byte[]? bytes = read(arg1: path, arg2: MaximumDocumentBytes);
            if (bytes is null || bytes.Length == 0 || bytes.Length > MaximumDocumentBytes)
            {
                throw Invalid(field: "bytes");
            }

            _ = _utf8.GetCharCount(bytes: bytes);
            using var document = JsonDocument.Parse(utf8Json: bytes,
                    options: new JsonDocumentOptions { MaxDepth = 16 });
            JsonElement root = document.RootElement;
            CheckUniqueProperties(value: root);
            Require(condition: StringComparer.Ordinal.Equals(x: Text(parent: root, field: "id"), y: id)
                    && Integer(parent: root, field: "version", minimum: 1, maximum: 1) == 1, field: "header");
            return parse(arg: root);
        }
        catch (Exception error) when (error is JsonException or InvalidDataException or IOException
                or DecoderFallbackException
                or UnauthorizedAccessException)
        {
            throw new InvalidDataException(message: "RUN_DEFINITION_INVALID " + path + ": " + error.Message,
                    innerException: error);
        }
    }

    private static void CheckUniqueProperties(JsonElement value)
    {
        if (value.ValueKind is JsonValueKind.Object)
        {
            var seen = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                Require(condition: seen.Add(item: property.Name), field: property.Name);
                CheckUniqueProperties(value: property.Value);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            foreach (JsonElement child in value.EnumerateArray())
            {
                CheckUniqueProperties(value: child);
            }
        }
    }

    private static InventorySettings ReadInventory(JsonElement root)
    {
        JsonElement stash = Field(parent: root, field: "stash");
        JsonElement nesting = Field(parent: root, field: "nesting");
        JsonElement quickSlots = Field(parent: root, field: "quickSlots");
        JsonElement ranges = Field(parent: root, field: "ranges");
        Size grid = ReadSize(value: Field(parent: stash, field: "pageGrid"), field: "stash.pageGrid");
        Require(condition: Text(parent: stash, field: "pageLimit") is "unlimited"
                && Flag(parent: stash, field: "tierIndependent")
                && !Flag(parent: stash, field: "remoteAccess"), field: "stash");
        Require(condition: Integer(parent: nesting, field: "rootContainerLevel", minimum: 1, maximum: 1) == 1
                && !Flag(parent: nesting, field: "cyclesAllowed"), field: "nesting");
        Require(condition: Flag(parent: quickSlots, field: "referencesOnly")
                && Flag(parent: quickSlots, field: "foodOnly"), field: "quickSlots");
        int local = Integer(parent: ranges, field: "npcUnits", minimum: 1, maximum: 1000);
        Require(condition: local == Integer(parent: ranges, field: "corpseUnits", minimum: 1, maximum: 1000),
                field: "ranges.corpseUnits");
        return new InventorySettings(stashGrid: grid, maximumContainerLevels: Integer(parent: nesting,
                field: "maximumLevels", minimum: 3, maximum: 3),
                quickSlotCount: Integer(parent: quickSlots, field: "count", minimum: 4, maximum: 4),
                localUnits: local, stashUnits: Integer(parent: ranges, field: "mainBuildingUnits", minimum: 1,
                        maximum: 1000));
    }

    private static SurvivorSettings ReadSurvivors(JsonElement root)
    {
        Require(condition: ReadOutfit(parent: Field(parent: root, field: "appearance"), field: "outfitRefs")
                .SequenceEqual(second: _outfit, comparer: StringComparer.Ordinal), field: "appearance.outfitRefs");
        return new SurvivorSettings(
                maximumHealth: Number(parent: root, field: "maximumHealth", minimum: 1, maximum: 10000),
                maximumStamina: Number(parent: root, field: "maximumStamina", minimum: 1, maximum: 10000),
                refugeUnits: Integer(parent: root, field: "refugeDistanceUnits", minimum: 1, maximum: 1000),
                dangerUnits: Integer(parent: root, field: "safeEnemyDistanceUnits", minimum: 1, maximum: 1000));
    }

    private static ImmutableDictionary<string, Item> ReadItems(JsonElement root, Size stash)
    {
        JsonElement definitions = Array(parent: root, field: "definitions");
        Require(condition: definitions.GetArrayLength() == _itemSlots.Length, field: "definitions");
        Dictionary<string, JsonElement> rows = IndexById(rows: definitions, field: "id");
        ImmutableDictionary<string, Item>.Builder items = ImmutableDictionary.CreateBuilder<string, Item>(
                keyComparer: StringComparer.Ordinal);
        for (int index = 0; index < _itemSlots.Length; index++)
        {
            string id = FormattableString.Invariant(formattable: $"IT-{index + 1:D2}");
            if (!rows.TryGetValue(key: id, value: out JsonElement row))
            {
                throw Invalid(field: id);
            }

            try
            {
                items.Add(key: id, value: ReadItem(row: row, id: id, slot: _itemSlots[index], stash: stash));
            }
            catch (InvalidDataException error)
            {
                throw new InvalidDataException(message: id + "." + error.Message, innerException: error);
            }
        }

        return items.ToImmutable();
    }

    private static Item ReadItem(JsonElement row, string id, string? slot, Size stash)
    {
        Size footprint = ReadSize(value: Field(parent: row, field: "footprint"), field: "footprint");
        bool rotatable = Flag(parent: row, field: "rotatable");
        Require(condition: footprint.Width <= stash.Width && footprint.Height <= stash.Height
                || rotatable && footprint.Height <= stash.Width && footprint.Width <= stash.Height,
                field: "footprint");
        Require(condition: StringComparer.Ordinal.Equals(x: NullableText(parent: row, field: "slot"), y: slot),
                field: "slot");
        int maxStack = Integer(parent: row, field: "maxStack", minimum: 1, maximum: 10000);
        decimal kilograms = Number(parent: row, field: "weightKilograms", minimum: 0.000001m, maximum: 1000);
        JsonElement storageValue = Field(parent: row, field: "storage");
        Size? storage = storageValue.ValueKind is JsonValueKind.Null
                ? null : ReadSize(value: storageValue, field: "storage");
        decimal? maximumCondition = OptionalCondition(row: row, id: id);
        Require(condition: storage.HasValue == (id is "IT-11" or "IT-12" or "IT-13" or "IT-21" or "IT-22" or
                "IT-23" or "IT-24"), field: "storage");
        Require(condition: maxStack == 1 || slot is null && storage is null || id is "IT-02", field: "maxStack");
        return new Item(id: id, footprint: footprint, maxStack: maxStack, weightKilograms: kilograms,
                rotatable: rotatable, slot: slot, storage: storage, maximumCondition: maximumCondition);
    }

    private static decimal? OptionalCondition(JsonElement row, string id)
    {
        JsonElement condition = Field(parent: row, field: "conditionMaximum");
        if (id is "IT-03" or "IT-04" or "IT-05" or "IT-06" or "IT-07" or "IT-18" or "IT-19" or "IT-20")
        {
            return Number(parent: row, field: "conditionMaximum", minimum: 1, maximum: 10000);
        }

        Require(condition: condition.ValueKind is JsonValueKind.Null, field: "conditionMaximum");
        return null;
    }

    private static Start ReadStart(JsonElement root, ImmutableDictionary<string, Item> items)
    {
        int survivors = Integer(parent: root, field: "startingSurvivors", minimum: 8, maximum: 8);
        int players = Integer(parent: root, field: "playerLimit", minimum: 4, maximum: 4);
        string role = Text(parent: root, field: "initialUncontrolledRole");
        ImmutableArray<string> outfit = ReadOutfit(parent: root, field: "startingOutfit");
        Require(condition: role is "unassigned" && outfit.SequenceEqual(second: _outfit,
                comparer: StringComparer.Ordinal), field: "startingOutfit");
        Require(condition: Text(parent: root, field: "startingPersonalSupplies") is "outfit-only"
                && Flag(parent: root, field: "stashPerRunNotPerPlayer")
                && !Flag(parent: root, field: "recruitsReplenishStash"), field: "startingPersonalSupplies");
        JsonElement stash = Array(parent: root, field: "initialStash");
        Require(condition: stash.GetArrayLength() <= items.Count, field: "initialStash");
        var seen = new HashSet<string>(comparer: StringComparer.Ordinal);
        ImmutableArray<StartingStack> stacks = [.. stash.EnumerateArray().Select(selector: row =>
                ReadStartingStack(row: row, items: items, seen: seen))];
        Require(condition: stacks.Sum(selector: stack => stack.Count) <= MaximumStartingStacks,
                field: "initialStash.stacks");
        return new Start(survivorCount: survivors, playerLimit: players, role: role, outfit: outfit,
                stash: stacks);
    }

    private static StartingStack ReadStartingStack(JsonElement row, ImmutableDictionary<string, Item> items,
            HashSet<string> seen)
    {
        string id = Text(parent: row, field: "item");
        if (!seen.Add(item: id) || !items.TryGetValue(key: id, value: out Item? definition))
        {
            throw Invalid(field: "initialStash." + id);
        }

        int quantity = Integer(parent: row, field: "stackQuantity", minimum: 1, maximum: definition.MaxStack);
        int count = Integer(parent: row, field: "stacks", minimum: 1, maximum: MaximumStartingStacks);
        int total = Integer(parent: row, field: "quantity", minimum: 1, maximum: definition.MaxStack * count);
        string condition = Text(parent: row, field: "condition");
        Require(condition: total == quantity * count && StringComparer.Ordinal.Equals(x: condition,
                y: definition.MaximumCondition.HasValue ? "full" : "not-applicable")
                && Text(parent: row, field: "contents") is "empty", field: "initialStash." + id);
        return new StartingStack(itemId: id, quantity: quantity, count: count);
    }

    private static bool CheckCombat(JsonElement root, ImmutableDictionary<string, Item> items)
    {
        JsonElement attacks = Array(parent: root, field: "attacks");
        var weapons = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement attack in attacks.EnumerateArray())
        {
            string? id = NullableText(parent: attack, field: "itemRef");
            Require(condition: id is null || (id is "IT-03" or "IT-04" or "IT-05" or "IT-06" or "IT-07")
                    && weapons.Add(item: id), field: "attacks.itemRef");
        }

        Require(condition: weapons.Count == 5, field: "attacks.itemRef");
        var armorIds = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement armor in Array(parent: root, field: "armor").EnumerateArray())
        {
            string id = Text(parent: armor, field: "itemRef");
            Require(condition: (id is "IT-18" or "IT-19" or "IT-20") && armorIds.Add(item: id)
                    && Number(parent: armor, field: "maximumDurability", minimum: 1, maximum: 10000)
                            == items[id].MaximumCondition, field: "armor." + id);
        }

        Require(condition: armorIds.Count == 3, field: "armor.itemRef");
        return true;
    }

    private static Dictionary<string, JsonElement> IndexById(JsonElement rows, string field)
    {
        var indexed = new Dictionary<string, JsonElement>(comparer: StringComparer.Ordinal);
        foreach (JsonElement row in rows.EnumerateArray())
        {
            string id = Text(parent: row, field: field);
            Require(condition: indexed.TryAdd(key: id, value: row), field: field + "." + id);
        }

        return indexed;
    }

    private static ImmutableArray<string> ReadOutfit(JsonElement parent, string field)
    {
        return [.. Array(parent: parent, field: field).EnumerateArray()
                .Select(selector: value => ReadText(value: value, field: field))];
    }

    private static Size ReadSize(JsonElement value, string field)
    {
        Require(condition: value.ValueKind is JsonValueKind.Array && value.GetArrayLength() == 2, field: field);
        return new Size(width: ReadInteger(value: value[0], field: field, minimum: 1, maximum: 64),
                height: ReadInteger(value: value[1], field: field, minimum: 1, maximum: 64));
    }

    private static JsonElement Array(JsonElement parent, string field)
    {
        JsonElement value = Field(parent: parent, field: field);
        Require(condition: value.ValueKind is JsonValueKind.Array, field: field);
        return value;
    }

    private static JsonElement Field(JsonElement parent, string field)
    {
        return parent.ValueKind is JsonValueKind.Object && parent.TryGetProperty(propertyName: field,
                value: out JsonElement value) ? value : throw Invalid(field: field);
    }

    private static bool Flag(JsonElement parent, string field)
    {
        return Field(parent: parent, field: field).ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => throw Invalid(field: field),
        };
    }

    private static string Text(JsonElement parent, string field)
    {
        return ReadText(value: Field(parent: parent, field: field), field: field);
    }

    private static string? NullableText(JsonElement parent, string field)
    {
        JsonElement value = Field(parent: parent, field: field);
        return value.ValueKind is JsonValueKind.Null ? null : ReadText(value: value, field: field);
    }

    private static string ReadText(JsonElement value, string field)
    {
        return value.ValueKind is JsonValueKind.String && value.GetString() is { Length: > 0 and <= 128 } text
                ? text : throw Invalid(field: field);
    }

    private static int Integer(JsonElement parent, string field, int minimum, int maximum)
    {
        return ReadInteger(value: Field(parent: parent, field: field), field: field, minimum: minimum,
                maximum: maximum);
    }

    private static int ReadInteger(JsonElement value, string field, int minimum, int maximum)
    {
        return value.ValueKind is JsonValueKind.Number && value.TryGetInt32(value: out int number)
                && number >= minimum && number <= maximum ? number : throw Invalid(field: field);
    }

    private static decimal Number(JsonElement parent, string field, decimal minimum, decimal maximum)
    {
        JsonElement value = Field(parent: parent, field: field);
        return value.ValueKind is JsonValueKind.Number && value.TryGetDecimal(value: out decimal number)
                && number >= minimum && number <= maximum ? number : throw Invalid(field: field);
    }

    private static void Require(bool condition, string field)
    {
        if (!condition)
        {
            throw Invalid(field: field);
        }
    }

    private static InvalidDataException Invalid(string field)
    {
        return new InvalidDataException(message: field);
    }

    [StructLayout(layoutKind: LayoutKind.Auto)]
    internal readonly record struct Size
    {
        internal Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        internal int Width { get; }
        internal int Height { get; }
    }
    internal sealed record Item
    {
        internal Item(string id, Size footprint, int maxStack, decimal weightKilograms, bool rotatable,
                string? slot, Size? storage, decimal? maximumCondition)
        {
            Id = id;
            Footprint = footprint;
            MaxStack = maxStack;
            WeightKilograms = weightKilograms;
            Rotatable = rotatable;
            Slot = slot;
            Storage = storage;
            MaximumCondition = maximumCondition;
        }

        internal string Id { get; }
        internal Size Footprint { get; }
        internal int MaxStack { get; }
        internal decimal WeightKilograms { get; }
        internal bool Rotatable { get; }
        internal string? Slot { get; }
        internal Size? Storage { get; }
        internal decimal? MaximumCondition { get; }
    }
    internal sealed record StartingStack
    {
        internal StartingStack(string itemId, int quantity, int count)
        {
            ItemId = itemId;
            Quantity = quantity;
            Count = count;
        }

        internal string ItemId { get; }
        internal int Quantity { get; }
        internal int Count { get; }
    }
    internal sealed record Start
    {
        internal Start(int survivorCount, int playerLimit, string role, ImmutableArray<string> outfit,
                ImmutableArray<StartingStack> stash)
        {
            SurvivorCount = survivorCount;
            PlayerLimit = playerLimit;
            Role = role;
            Outfit = outfit;
            Stash = stash;
        }

        internal int SurvivorCount { get; }
        internal int PlayerLimit { get; }
        internal string Role { get; }
        internal ImmutableArray<string> Outfit { get; }
        internal ImmutableArray<StartingStack> Stash { get; }
    }
    internal sealed record SurvivorSettings
    {
        internal SurvivorSettings(decimal maximumHealth, decimal maximumStamina, int refugeUnits, int dangerUnits)
        {
            MaximumHealth = maximumHealth;
            MaximumStamina = maximumStamina;
            RefugeUnits = refugeUnits;
            DangerUnits = dangerUnits;
        }

        internal decimal MaximumHealth { get; }
        internal decimal MaximumStamina { get; }
        internal int RefugeUnits { get; }
        internal int DangerUnits { get; }
    }
    internal sealed record InventorySettings
    {
        internal InventorySettings(Size stashGrid, int maximumContainerLevels, int quickSlotCount,
                int localUnits, int stashUnits)
        {
            StashGrid = stashGrid;
            MaximumContainerLevels = maximumContainerLevels;
            QuickSlotCount = quickSlotCount;
            LocalUnits = localUnits;
            StashUnits = stashUnits;
        }

        internal Size StashGrid { get; }
        internal int MaximumContainerLevels { get; }
        internal int QuickSlotCount { get; }
        internal int LocalUnits { get; }
        internal int StashUnits { get; }
    }
}
