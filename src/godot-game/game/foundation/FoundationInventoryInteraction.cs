using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace KingdomZombies.Foundation;

/// <summary>Owns inventory presentation state and builds requests from committed item references.</summary>
internal sealed partial class FoundationInventoryInteraction
{
    internal bool Open { get; private set; }
    internal bool External { get; private set; }
    internal Preview? Drag { get; private set; }
    internal IReadOnlyList<Window> Windows => _windows;

    internal static ImmutableArray<string> Slots { get; } =
    [
        "headwear", "face", "torso-clothing", "legs", "body-armor", "gloves", "boots", "backpack", "belt",
        "left-hand", "right-hand",
    ];

    internal void Show(bool external)
    {
        Open = true;
        External |= external;
    }

    internal void Close()
    {
        Open = false;
        External = false;
        Drag = null;
        _windows.Clear();
        _projected = null;
    }

    /// <summary>Records the readback version the open workspace drew, whichever input or readback drew it.</summary>
    internal void Projected(string version)
    {
        _projected = Open ? version : null;
    }

    /// <summary>
    /// Gets whether current readback differs from the drawn version. A closed workspace has drawn nothing, so a
    /// reopened one never treats returning windows as already drawn by an earlier opening.
    /// </summary>
    internal bool Stale(string version)
    {
        return !StringComparer.Ordinal.Equals(x: _projected, y: version);
    }

    /// <summary>
    /// Gets whether an item press is modified. A held key counts even when the platform click carries no modifier
    /// flag, so a physical Ctrl or Shift held across the click still selects quick transfer or a split.
    /// </summary>
    internal static bool Modifier(bool flagged, bool held)
    {
        return flagged || held;
    }

    /// <summary>
    /// Identifies every readback view except status by its scope, token and header text, then its rows: the committed
    /// row stamps, or the full row text for nearby. A change to the status view alone never makes a projection stale.
    /// </summary>
    internal static string Version(FoundationRunReadback? readback)
    {
        return string.Join(separator: "|", values: readback?.Views.OrderBy(keySelector: pair => pair.Key,
                comparer: StringComparer.Ordinal).Where(predicate: pair => pair.Key is not "status").Select(
                        selector: pair => pair.Key + pair.Value.Token.ToString(provider: CultureInfo.InvariantCulture)
                                + pair.Value.Header.GetRawText() + string.Join(separator: ",", values:
                                        pair.Value.Rows.Select(selector: row => pair.Key is "nearby"
                                                ? row.GetRawText() : row.GetProperty(
                                                        propertyName: "Stamp").GetString()))) ?? []);
    }

    internal void Reset()
    {
        Close();
        _positions.Clear();
    }

    internal bool Begin(Item item, bool external, long token, bool split)
    {
        if (!Open || Drag is not null || external && token <= 0)
        {
            return false;
        }

        Drag = new Preview(item: item, external: external, token: token, rotated: item.Location.Rotated,
                amount: null, choosingAmount: split && item.MaxStack > 1 && item.Quantity > 1);
        return true;
    }

    internal void Cancel()
    {
        Drag = null;
    }

    internal bool Rotate()
    {
        if (Drag is not { ChoosingAmount: false, Item.Rotatable: true } drag)
        {
            return false;
        }

        Drag = drag with { Rotated = !drag.Rotated };
        return true;
    }

    internal bool Amount(string text)
    {
        if (Drag is not { ChoosingAmount: true } drag || Number(text: text, maximum: drag.Item.Quantity - 1)
                is not { } amount)
        {
            return false;
        }

        Drag = drag with { Amount = amount, ChoosingAmount = false };
        return true;
    }

    internal static int? Number(string text, int maximum)
    {
        return text.Length is > 0 and <= 10 && text.All(predicate: value => value is >= '0' and <= '9')
                && int.TryParse(s: text, style: NumberStyles.None, provider: CultureInfo.InvariantCulture,
                        result: out int amount) && amount >= 1 && amount <= maximum ? amount : null;
    }

    internal static Command? PageRequest(string text, int current, int total, long token)
    {
        return token > 0 && Number(text: text, maximum: total) is { } page && page != current
                ? new Command(kind: "browse", request: JsonSerializer.SerializeToElement(
                        value: new { Token = token, Page = page })) : null;
    }

    internal static ImmutableArray<(string kind, string id)> Nearby(ImmutableArray<JsonElement> rows)
    {
        return [.. rows.Select(selector: row => (Text(row: row, field: "Kind"), Text(row: row, field: "Id")))];
    }

    internal static (string kind, string id) ExternalTarget(JsonElement header)
    {
        return (Text(row: header, field: "Kind"), Text(row: header, field: "TargetId"));
    }

    internal static Command SelectTarget((string kind, string id) target)
    {
        return new Command(kind: "select", request: JsonSerializer.SerializeToElement(
                value: new { TargetKind = target.kind, TargetId = target.id }));
    }

    internal static (ImmutableArray<(int x, int y)> cells, int more)? DepositHighlight(
            JsonElement header, (string kind, string id) target, long sequence)
    {
        JsonElement effect = header.GetProperty(propertyName: "Effect");
        if (sequence <= 0 || target.kind is not "Stash" || ExternalTarget(header: header) != target
                || effect.ValueKind != JsonValueKind.Object
                || effect.GetProperty(propertyName: "Sequence").GetInt64() != sequence
                || effect.GetProperty(propertyName: "PageNumber").GetInt32()
                        != header.GetProperty(propertyName: "Page").GetInt32())
        {
            return null;
        }

        int width = header.GetProperty(propertyName: "Width").GetInt32();
        int height = header.GetProperty(propertyName: "Height").GetInt32();
        byte[]? mask = FoundationProtocol.ReadStashEffectMask(body: header, effect: effect);
        if (mask is null)
        {
            return null;
        }

        ImmutableArray<(int x, int y)> cells = [.. Enumerable.Range(start: 0, count: width * height)
                .Where(predicate: index => (mask[index / 8] & (1 << (index % 8))) != 0)
                .Select(selector: index => (x: index % width, y: index / width))];
        return (cells, effect.GetProperty(propertyName: "MorePages").GetInt32());
    }

    internal Placement CheckGrid(Grid grid, int x, int y)
    {
        if (Drag is not { ChoosingAmount: false } drag)
        {
            return new Placement(reason: "Inactive");
        }

        var area = new Rect(x: x, y: y, width: drag.Width, height: drag.Height);
        string? reason = x < 0 || y < 0 || x + drag.Width > grid.Width || y + drag.Height > grid.Height
                ? "DoesNotFit" : grid.Ancestors.Contains(item: drag.Item.Id) ? "Cycle"
                        : drag.Item.StorageWidth.HasValue && grid.Ancestors.Length >= 3 ? "Nesting" : null;
        if (reason is not null)
        {
            return new Placement(reason: reason, area: area);
        }

        ImmutableArray<Item> occupied = [.. grid.Items.Where(predicate: item =>
                (item.Id != drag.Item.Id || drag.Amount.HasValue) && area.Intersects(other: item.Location.Area))];
        if (occupied.IsEmpty)
        {
            var at = new Location(kind: grid.Kind, gridId: grid.Id, x: x, y: y, rotated: drag.Rotated,
                    width: drag.Width, height: drag.Height);
            return new Placement(reason: "Ready", area: area, action: Place(drag: drag, destination: at,
                    container: grid.Container, token: grid.Token));
        }

        return Merge(drag: drag, occupied: occupied, area: area, token: grid.Token);
    }

    internal Placement CheckSlot(string slot, string owner, string kind, Item? occupied, long token)
    {
        if (Drag is not { ChoosingAmount: false } drag)
        {
            return new Placement(reason: "Inactive");
        }

        bool fits = Compatible(item: drag.Item, slot: slot) && drag.Amount is null;
        if (!fits || occupied is not null && occupied.Id != drag.Item.Id)
        {
            return new Placement(reason: fits ? "Occupied" : "Incompatible");
        }

        var location = new Location(kind: kind, targetId: owner, slot: slot);
        return new Placement(reason: "Ready", action: Place(drag: drag, destination: location,
                container: null, token: token));
    }

    internal static bool Compatible(Item item, string slot)
    {
        return StringComparer.Ordinal.Equals(x: slot, y: item.EquipmentSlot)
                || item.EquipmentSlot is "either-logical-hand" && slot is "left-hand" or "right-hand";
    }

    internal Command? Release(Placement placement)
    {
        if (Drag?.ChoosingAmount == true)
        {
            return null;
        }

        Drag = null;
        return placement.Action;
    }

    internal static Command Assign(Item item, int slot)
    {
        return new Command(kind: "assign", request: JsonSerializer.SerializeToElement(value: new
        {
            Item = Reference(item: item),
            Slot = slot,
        }));
    }

    internal static Command Consume(Item item)
    {
        return new Command(kind: "food", request: JsonSerializer.SerializeToElement(value: new
        {
            Item = Reference(item: item),
            QuickSlot = (int?)null,
        }));
    }

    internal static Command Drop(Item item)
    {
        return new Command(kind: "drop", request: JsonSerializer.SerializeToElement(value: new
        {
            Item = Reference(item: item),
            TargetId = (string?)null,
        }));
    }

    internal static Command Quick(Item item, long token, bool deposit)
    {
        return new Command(kind: "quick", request: JsonSerializer.SerializeToElement(value: new
        {
            Item = Reference(item: item),
            Token = token,
            Deposit = deposit,
        }));
    }

    internal ImmutableArray<long> RequestedWindows(bool external, IEnumerable<long> equipped)
    {
        return [.. _windows.Where(predicate: window => window.External == external)
                .Select(selector: window => window.Item.Id).Concat(second: equipped).Distinct()
                .Take(count: FoundationProtocol.MaximumWindowSelectors)];
    }

    internal Window? OpenBag(Item item, long? parent, bool external, Rect source, Rect bounds)
    {
        if (!Open || item.StorageWidth is null || item.StorageHeight is null
                || !RequestedWindows(external: external, equipped: [item.Id]).Contains(item: item.Id))
        {
            return null;
        }

        int width = Math.Clamp(value: item.StorageWidth.Value * 32 + 28, min: 256, max: 390);
        int height = Math.Clamp(value: item.StorageHeight.Value * 32 + 92, min: 184, max: 392);
        int right = source.X + source.Width + 8;
        int x = right + width <= bounds.X + bounds.Width ? right : source.X - width - 8;
        Rect start = _positions.GetValueOrDefault(key: item.Id,
                defaultValue: new Rect(x: x, y: source.Y, width: width, height: height));
        var window = new Window(item: item, parent: parent, external: external, frame: Clamp(frame: start,
                bounds: bounds));
        _windows.RemoveAll(match: candidate => candidate.Item.Id == item.Id);
        _windows.Add(item: window);
        return window;
    }

    internal Window? MoveBag(long id, int x, int y, Rect bounds)
    {
        Window? current = _windows.Find(match: window => window.Item.Id == id);
        if (current is null)
        {
            return null;
        }

        Rect frame = Clamp(frame: current.Frame with { X = x, Y = y }, bounds: bounds);
        _positions[id] = frame;
        Window moved = current with { Frame = frame };
        _windows.Remove(item: current);
        _windows.Add(item: moved);
        return moved;
    }

    internal void CloseBag(long id)
    {
        foreach (Window child in _windows.Where(predicate: window => window.Parent == id).ToArray())
        {
            CloseBag(id: child.Item.Id);
        }

        _windows.RemoveAll(match: window => window.Item.Id == id);
        if (Drag?.Item.Location.GridId == id)
        {
            Cancel();
        }
    }

    internal void AccessLost(bool keepExternal, bool aimedExternal = false)
    {
        _windows.RemoveAll(match: window => window.External);
        if (Drag is { External: true } || aimedExternal)
        {
            Cancel();
        }

        External = keepExternal && Open;
    }

    internal static Item ReadItem(JsonElement row)
    {
        JsonElement position = row.GetProperty(propertyName: "Location");
        var location = new Location(kind: Text(row: position, field: "Kind"), targetId: Text(
                row: position, field: "TargetId"), slot: Text(row: position, field: "Slot"),
                gridId: Integer(row: position, field: "GridId"), x: (int?)Integer(row: position, field: "X"),
                y: (int?)Integer(row: position, field: "Y"), rotated: Flag(row: position, field: "Rotated"),
                width: (int?)Integer(row: position, field: "Width"), height: (int?)Integer(
                        row: position, field: "Height"));
        int quantity = row.GetProperty(propertyName: "Quantity").GetInt32();
        int maximum = row.GetProperty(propertyName: "MaxStack").GetInt32();
        decimal weight = row.GetProperty(propertyName: "WeightKilograms").GetDecimal();
        int width = row.GetProperty(propertyName: "FootprintWidth").GetInt32();
        int height = row.GetProperty(propertyName: "FootprintHeight").GetInt32();
        int? storageWidth = (int?)Integer(row: row, field: "StorageWidth");
        int? storageHeight = (int?)Integer(row: row, field: "StorageHeight");
        return new Item(id: row.GetProperty(propertyName: "Id").GetInt64(), stamp: Text(row: row, field: "Stamp"),
                definition: Text(row: row, field: "DefinitionId"), name: Text(row: row, field: "Name"),
                icon: Text(row: row, field: "CanonicalIcon"), quantity: quantity, maxStack: maximum, weight: weight,
                condition: Decimal(row: row, field: "Condition"), maximumCondition: Decimal(row: row,
                        field: "MaximumCondition"), width: width, height: height, storageWidth: storageWidth,
                storageHeight: storageHeight,
                equipmentSlot: Text(row: row, field: "EquipmentSlot"), food: Flag(row: row, field: "Food"),
                rotatable: Flag(row: row, field: "Rotatable"), location: location);
    }

    private readonly List<Window> _windows = [];
    private readonly Dictionary<long, Rect> _positions = [];
    private string? _projected;

    private static Command Place(Preview drag, Location destination, Item? container, long token)
    {
        return new Command(kind: "place", request: JsonSerializer.SerializeToElement(value: new
        {
            Item = Reference(item: drag.Item),
            Token = Token(drag: drag, token: token),
            Destination = new
            {
                destination.Kind,
                destination.TargetId,
                destination.Slot,
                destination.GridId,
                destination.X,
                destination.Y,
                Rotated = destination.GridId.HasValue ? destination.Rotated : (bool?)null,
            },
            Container = container is null ? null : Reference(item: container),
            Amount = drag.Amount,
        }));
    }

    private static Placement Merge(Preview drag, ImmutableArray<Item> occupied, Rect area, long token)
    {
        if (occupied.Length != 1 || occupied[0] is not { } target || target.Id == drag.Item.Id
                || !StringComparer.Ordinal.Equals(x: target.Definition, y: drag.Item.Definition)
                || target.Condition != drag.Item.Condition
                || target.StorageWidth.HasValue || target.MaxStack < 2)
        {
            return new Placement(reason: "DoesNotFit", area: area);
        }

        if (target.Quantity >= target.MaxStack)
        {
            return new Placement(reason: "Capacity", area: area);
        }

        var command = new Command(kind: "merge", request: JsonSerializer.SerializeToElement(value: new
        {
            Item = Reference(item: drag.Item),
            Token = Token(drag: drag, token: token),
            Target = Reference(item: target),
            Amount = drag.Amount,
        }));
        return new Placement(reason: "Ready", area: area, action: command);
    }

    private static long? Token(Preview drag, long token)
    {
        return token > 0 ? token : drag.External ? drag.Token : null;
    }

    private static object Reference(Item item)
    {
        return new { item.Id, item.Stamp };
    }

    private static Rect Clamp(Rect frame, Rect bounds)
    {
        return frame with
        {
            X = Math.Clamp(value: frame.X, min: bounds.X, max: Math.Max(val1: bounds.X,
                    val2: bounds.X + bounds.Width - frame.Width)),
            Y = Math.Clamp(value: frame.Y, min: bounds.Y, max: Math.Max(val1: bounds.Y,
                    val2: bounds.Y + bounds.Height - frame.Height)),
        };
    }

    private static string Text(JsonElement row, string field)
    {
        return row.TryGetProperty(propertyName: field, value: out JsonElement value) ? value.GetString() ?? "" : "";
    }

    private static long? Integer(JsonElement row, string field)
    {
        return row.TryGetProperty(propertyName: field, value: out JsonElement value)
                && value.ValueKind is JsonValueKind.Number ? value.GetInt64() : null;
    }

    private static decimal? Decimal(JsonElement row, string field)
    {
        return row.TryGetProperty(propertyName: field, value: out JsonElement value)
                && value.ValueKind is JsonValueKind.Number ? value.GetDecimal() : null;
    }

    private static bool Flag(JsonElement row, string field)
    {
        return row.TryGetProperty(propertyName: field, value: out JsonElement value)
                && value.ValueKind is JsonValueKind.True;
    }

    internal sealed record Command
    {
        internal Command(string kind, JsonElement request)
        {
            Kind = kind;
            Request = request;
        }

        internal string Kind { get; init; }
        internal JsonElement Request { get; init; }
    }

    internal sealed record Item
    {
        internal Item(long id, string stamp, string definition, string name, string icon, int quantity,
                int maxStack, decimal weight, decimal? condition, decimal? maximumCondition, int width, int height,
                int? storageWidth, int? storageHeight, string equipmentSlot, bool food, bool rotatable,
                Location location)
        {
            Id = id;
            Stamp = stamp;
            Definition = definition;
            Name = name;
            Icon = icon;
            Quantity = quantity;
            MaxStack = maxStack;
            Weight = weight;
            Condition = condition;
            MaximumCondition = maximumCondition;
            Width = width;
            Height = height;
            StorageWidth = storageWidth;
            StorageHeight = storageHeight;
            EquipmentSlot = equipmentSlot;
            Food = food;
            Rotatable = rotatable;
            Location = location;
        }

        internal long Id { get; init; }
        internal string Stamp { get; init; }
        internal string Definition { get; init; }
        internal string Name { get; init; }
        internal string Icon { get; init; }
        internal int Quantity { get; init; }
        internal int MaxStack { get; init; }
        internal decimal Weight { get; init; }
        internal decimal? Condition { get; init; }
        internal decimal? MaximumCondition { get; init; }
        internal int Width { get; init; }
        internal int Height { get; init; }
        internal int? StorageWidth { get; init; }
        internal int? StorageHeight { get; init; }
        internal string EquipmentSlot { get; init; }
        internal bool Food { get; init; }
        internal bool Rotatable { get; init; }
        internal Location Location { get; init; }
    }

    internal sealed record Location
    {
        internal Location(string kind, string? targetId = null, string? slot = null, long? gridId = null,
                int? x = null, int? y = null, bool rotated = false, int? width = null, int? height = null)
        {
            Kind = kind;
            TargetId = targetId;
            Slot = slot;
            GridId = gridId;
            X = x;
            Y = y;
            Rotated = rotated;
            Width = width;
            Height = height;
        }

        internal string Kind { get; init; }
        internal string? TargetId { get; init; }
        internal string? Slot { get; init; }
        internal long? GridId { get; init; }
        internal int? X { get; init; }
        internal int? Y { get; init; }
        internal bool Rotated { get; init; }
        internal int? Width { get; init; }
        internal int? Height { get; init; }

        internal Rect Area => new(x: X ?? 0, y: Y ?? 0, width: Width ?? 1, height: Height ?? 1);
    }

    internal sealed record Grid
    {
        internal Grid(string kind, long id, int width, int height, Item? container, long token,
                ImmutableArray<Item> items, ImmutableArray<long> ancestors)
        {
            Kind = kind;
            Id = id;
            Width = width;
            Height = height;
            Container = container;
            Token = token;
            Items = items;
            Ancestors = ancestors;
        }

        internal string Kind { get; init; }
        internal long Id { get; init; }
        internal int Width { get; init; }
        internal int Height { get; init; }
        internal Item? Container { get; init; }
        internal long Token { get; init; }
        internal ImmutableArray<Item> Items { get; init; }
        internal ImmutableArray<long> Ancestors { get; init; }
    }

    internal sealed record Preview
    {
        internal Preview(Item item, bool external, long token, bool rotated, int? amount, bool choosingAmount)
        {
            Item = item;
            External = external;
            Token = token;
            Rotated = rotated;
            Amount = amount;
            ChoosingAmount = choosingAmount;
        }

        internal Item Item { get; init; }
        internal bool External { get; init; }
        internal long Token { get; init; }
        internal bool Rotated { get; init; }
        internal int? Amount { get; init; }
        internal bool ChoosingAmount { get; init; }

        internal int Width => Rotated ? Item.Height : Item.Width;
        internal int Height => Rotated ? Item.Width : Item.Height;
    }

    internal sealed record Placement
    {
        internal Placement(string reason, Rect? area = null, Command? action = null)
        {
            Reason = reason;
            Area = area;
            Action = action;
        }

        internal string Reason { get; init; }
        internal Rect? Area { get; init; }
        internal Command? Action { get; init; }
    }
    internal sealed record Window
    {
        internal Window(Item item, long? parent, bool external, Rect frame)
        {
            Item = item;
            Parent = parent;
            External = external;
            Frame = frame;
        }

        internal Item Item { get; init; }
        internal long? Parent { get; init; }
        internal bool External { get; init; }
        internal Rect Frame { get; init; }
    }

    [StructLayout(layoutKind: LayoutKind.Auto)]
    internal readonly record struct Rect
    {
        internal Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        internal int X { get; init; }
        internal int Y { get; init; }
        internal int Width { get; init; }
        internal int Height { get; init; }

        internal bool Intersects(Rect other)
        {
            return X < other.X + other.Width && X + Width > other.X && Y < other.Y + other.Height
                    && Y + Height > other.Y;
        }
    }
}
