using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace KingdomZombies.Foundation;

/// <summary>Maps the committed public appearance into bounded authored composition values.</summary>
internal static class FoundationEquipmentProjection
{
    internal static Frame Read(long revision, JsonElement world)
    {
        long elapsed = world.GetProperty(propertyName: "Elapsed").GetInt64();
        ImmutableArray<Subject> actors = [.. world.GetProperty(propertyName: "Actors").EnumerateArray()
                .Select(selector: Live)];
        ImmutableArray<Subject> corpses = [.. world.GetProperty(propertyName: "Corpses").EnumerateArray()
                .Select(selector: Dead)];
        ImmutableArray<Arrow> arrows = [.. world.GetProperty(propertyName: "Arrows").EnumerateArray()
                .Select(selector: ArrowFrom)];
        int actorCount = actors.Select(selector: actor => actor.Id).Distinct(comparer: StringComparer.Ordinal).Count();
        int corpseCount = corpses.Select(selector: corpse => corpse.Id)
                .Distinct(comparer: StringComparer.Ordinal).Count();
        int arrowCount = arrows.Select(selector: arrow => arrow.Id).Distinct().Count();
        if (actorCount != actors.Length || corpseCount != corpses.Length || arrowCount != arrows.Length)
        {
            throw new InvalidDataException(message: "RUN_APPEARANCE_DUPLICATE");
        }

        return new Frame(revision: revision, elapsed: elapsed,
                origin: world.GetProperty(propertyName: "OriginX").GetInt64(),
                ground: world.GetProperty(propertyName: "Ground").GetInt64(),
                actors: actors, corpses: corpses, arrows: arrows);
    }

    internal static (double angle, int facing) BowAngle(double dx, double dy, int facing)
    {
        if (!double.IsFinite(d: dx) || !double.IsFinite(d: dy) || (dx == 0 && dy == 0))
        {
            return (0, facing);
        }

        return (Math.Atan2(y: dy, x: Math.Abs(value: dx)), dx < 0 ? -1 : dx > 0 ? 1 : facing);
    }

    internal static bool HasBow(Subject subject)
    {
        return subject.Left.Item is "IT-07" || subject.Right.Item is "IT-07";
    }

    internal static JsonElement Fits(Subject subject)
    {
        var map = subject.Layers.ToDictionary(keySelector: layer => layer.Slot,
                elementSelector: layer => layer.Item, comparer: StringComparer.Ordinal);
        return JsonSerializer.SerializeToElement(value: map);
    }

    internal static JsonElement Wear(Subject subject)
    {
        var map = subject.Layers.ToDictionary(keySelector: layer => layer.Slot,
                elementSelector: layer => layer.Item, comparer: StringComparer.Ordinal);
        if (subject.Crowned)
        {
            map.Add(key: "crown", value: "K21");
        }

        return JsonSerializer.SerializeToElement(value: map);
    }

    private static readonly string[] _fittedOrder =
            ["headwear", "face", "torso", "legs", "armor", "gloves", "boots", "backpack", "belt", "left", "right"];
    private static readonly string[] _bodies =
            ["CH-01", "CH-02", "CH-03", "CH-04", "CH-05", "CH-06", "CH-07", "CH-08"];

    private static Subject Live(JsonElement data)
    {
        string id = Text(data: data, field: "Id");
        bool hostile = data.GetProperty(propertyName: "Hostile").GetBoolean();
        bool crowned = data.GetProperty(propertyName: "BindingId").ValueKind == JsonValueKind.String && !hostile;
        double? aimX = Number(data: data, field: "AimX");
        double? aimY = Number(data: data, field: "AimY");
        return new Subject(id: id, body: Body(id: id, hostile: hostile), at: Contact(data: data),
                facing: Facing(data: data), layers: Layers(data: data), crowned: crowned, hostile: hostile,
                movement: Text(data: data, field: "Movement"), strikeAt: Number(data: data, field: "StrikeAt"),
                strikeUntil: Number(data: data, field: "StrikeUntil"), aim: aimX.HasValue && aimY.HasValue
                        ? (aimX.Value, aimY.Value) : null, flinchAt: Number(data: data, field: "FlinchAt"),
                arrowReady: data.GetProperty(propertyName: "ArrowReady").GetBoolean());
    }

    private static Subject Dead(JsonElement data)
    {
        bool hostile = data.GetProperty(propertyName: "Hostile").GetBoolean();
        return new Subject(id: Text(data: data, field: "Id"), body: Body(id: Text(data: data, field: "SurvivorId"),
                hostile: hostile), at: Contact(data: data), facing: Facing(data: data), layers: Layers(data: data),
                crowned: false, hostile: hostile, movement: "idle", diedAt: Number(data: data, field: "At"));
    }

    private static Arrow ArrowFrom(JsonElement data)
    {
        return new Arrow(id: data.GetProperty(propertyName: "Id").GetInt64(), at: Contact(data: data),
                velocity: (data.GetProperty(propertyName: "VelocityX").GetDouble(),
                        data.GetProperty(propertyName: "VelocityY").GetDouble()));
    }

    private static ImmutableArray<Layer> Layers(JsonElement data)
    {
        var recognized = new Dictionary<string, Layer>(comparer: StringComparer.Ordinal);
        foreach (JsonElement item in data.GetProperty(propertyName: "Equipment").EnumerateArray())
        {
            string slot = FitSlot(slot: Text(data: item, field: "Slot"));
            string definition = Text(data: item, field: "DefinitionId");
            if (!Allowed(slot: slot, item: definition) || !recognized.TryAdd(key: slot,
                    value: new Layer(slot: slot, item: definition,
                            broken: item.GetProperty(propertyName: "Broken").GetBoolean())))
            {
                throw new InvalidDataException(message: "RUN_APPEARANCE_FIT " + slot + " " + definition);
            }
        }

        return [.. _fittedOrder.Where(predicate: recognized.ContainsKey)
                .Select(selector: slot => recognized[slot])];
    }

    private static bool Allowed(string slot, string item)
    {
        return slot switch
        {
            "headwear" => item is "IT-14" or "IT-18",
            "face" => item is "IT-15",
            "torso" => item is "IT-11" or "IT-13",
            "legs" => item is "IT-12",
            "armor" => item is "IT-19" or "IT-20",
            "gloves" => item is "IT-16",
            "boots" => item is "IT-17",
            "backpack" => item is "IT-23" or "IT-24",
            "belt" => item is "IT-21",
            "left" or "right" => item is "IT-02" or "IT-03" or "IT-04" or "IT-05" or "IT-06"
                    or "IT-07" or "IT-08" or "IT-09" or "IT-10",
            _ => false,
        };
    }

    private static string FitSlot(string slot)
    {
        return slot switch
        {
            "torso-clothing" => "torso",
            "body-armor" => "armor",
            "left-hand" => "left",
            "right-hand" => "right",
            _ => slot,
        };
    }

    private static string Body(string id, bool hostile)
    {
        if (hostile)
        {
            return "CH-08";
        }

        const string Prefix = "survivor-";
        return id.StartsWith(value: Prefix, comparisonType: StringComparison.Ordinal)
                && int.TryParse(s: id.AsSpan(start: Prefix.Length), style: NumberStyles.None,
                        provider: CultureInfo.InvariantCulture, result: out int ordinal) && ordinal > 0
                ? _bodies[(ordinal - 1) % _bodies.Length] : _bodies[0];
    }

    private static string Text(JsonElement data, string field)
    {
        return data.GetProperty(propertyName: field).GetString()
                ?? throw new InvalidDataException(message: "RUN_APPEARANCE_TEXT " + field);
    }

    private static double? Number(JsonElement data, string field)
    {
        JsonElement value = data.GetProperty(propertyName: field);
        return value.ValueKind == JsonValueKind.Null ? null : value.GetDouble();
    }

    private static int Facing(JsonElement data)
    {
        int facing = data.GetProperty(propertyName: "Facing").GetInt32();
        return facing is -1 or 1 ? facing : throw new InvalidDataException(message: "RUN_APPEARANCE_FACING");
    }

    private static (double x, double y) Contact(JsonElement data)
    {
        return (data.GetProperty(propertyName: "X").GetDouble(), data.GetProperty(propertyName: "Y").GetDouble());
    }

    internal sealed class Frame
    {
        internal Frame(long revision, long elapsed, long origin, long ground, ImmutableArray<Subject> actors,
                ImmutableArray<Subject> corpses, ImmutableArray<Arrow> arrows)
        {
            Revision = revision;
            Elapsed = elapsed;
            Origin = origin;
            Ground = ground;
            Actors = actors;
            Corpses = corpses;
            Arrows = arrows;
        }

        internal long Revision { get; }
        internal long Elapsed { get; }
        internal long Origin { get; }
        internal long Ground { get; }
        internal ImmutableArray<Subject> Actors { get; }
        internal ImmutableArray<Subject> Corpses { get; }
        internal ImmutableArray<Arrow> Arrows { get; }
    }

    internal sealed class Subject
    {
        internal Subject(string id, string body, (double x, double y) at, int facing, ImmutableArray<Layer> layers,
                bool crowned, bool hostile, string movement, double? strikeAt = null, double? strikeUntil = null,
                (double x, double y)? aim = null, double? flinchAt = null, bool arrowReady = false,
                double? diedAt = null)
        {
            Id = id;
            Body = body;
            At = at;
            Facing = facing;
            Layers = layers;
            Crowned = crowned;
            Hostile = hostile;
            Movement = movement;
            StrikeAt = strikeAt;
            StrikeUntil = strikeUntil;
            Aim = aim;
            FlinchAt = flinchAt;
            ArrowReady = arrowReady;
            DiedAt = diedAt;
        }

        internal string Id { get; }
        internal string Body { get; }
        internal (double x, double y) At { get; }
        internal int Facing { get; }
        internal ImmutableArray<Layer> Layers { get; }
        internal bool Crowned { get; }
        internal bool Hostile { get; }
        internal string Movement { get; }
        internal double? StrikeAt { get; }
        internal double? StrikeUntil { get; }
        internal (double x, double y)? Aim { get; }
        internal double? FlinchAt { get; }
        internal bool ArrowReady { get; }
        internal double? DiedAt { get; }
        internal Layer Left => Hand(slot: "left");
        internal Layer Right => Hand(slot: "right");

        private Layer Hand(string slot)
        {
            return Layers.FirstOrDefault(predicate: layer => StringComparer.Ordinal.Equals(x: layer.Slot, y: slot))
                    ?? new Layer(slot: slot, item: "Empty", broken: false);
        }
    }

    internal sealed record Layer
    {
        internal Layer(string slot, string item, bool broken)
        {
            Slot = slot;
            Item = item;
            Broken = broken;
        }

        internal string Slot { get; }
        internal string Item { get; }
        internal bool Broken { get; }
    }

    internal sealed record Arrow
    {
        internal Arrow(long id, (double x, double y) at, (double x, double y) velocity)
        {
            Id = id;
            At = at;
            Velocity = velocity;
        }

        internal long Id { get; }
        internal (double x, double y) At { get; }
        internal (double x, double y) Velocity { get; }
    }
}
