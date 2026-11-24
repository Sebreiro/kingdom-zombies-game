using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace KingdomZombies.Foundation;

/// <summary>Reads a single current private HUD publication and derives display-only transitions from it.</summary>
internal sealed class FoundationHudState
{
    internal long Generation { get; private init; }
    internal string Survivor { get; private init; } = "";
    internal long Revision { get; private init; }
    internal long RunTime { get; private init; }
    internal decimal Health { get; private init; }
    internal decimal Stamina { get; private init; }
    internal decimal MaximumHealth { get; private init; }
    internal decimal MaximumStamina { get; private init; }
    internal string Rest { get; private init; } = "";
    internal string Load { get; private init; } = "";
    internal decimal Weight { get; private init; }
    internal decimal Soft { get; private init; }
    internal decimal Severe { get; private init; }
    internal decimal Speed { get; private init; }
    internal decimal Cost { get; private init; }
    internal long Money { get; private init; }
    internal Item? Left { get; private init; }
    internal Item? Right { get; private init; }
    internal bool LeftAttack { get; private init; }
    internal bool RightAttack { get; private init; }
    internal int? Ammo { get; private init; }
    internal decimal? FlinchAt { get; private init; }
    internal decimal AttackShade { get; private init; }
    internal decimal FoodShade { get; private init; }
    internal ImmutableArray<Item?> Food { get; private init; } = [];
    internal string SprintBlock { get; private init; } = "";

    internal string WeightText
    {
        get
        {
            decimal rounded = Math.Round(d: Weight, decimals: 2, mode: MidpointRounding.AwayFromZero);
            string displayed = Band(weight: rounded, soft: Soft, severe: Severe);
            return StringComparer.Ordinal.Equals(x: displayed, y: Load) ? Number(value: Weight)
                    : Weight.ToString(format: "0.############################", provider: CultureInfo.InvariantCulture);
        }
    }

    internal static FoundationHudState? Read(FoundationRunView view, string survivor, long generation)
    {
        JsonElement header = view.Header;
        if (generation <= 0 || view.Epoch != generation
                || !StringComparer.Ordinal.Equals(x: Text(value: header, field: "SurvivorId"), y: survivor)
                || header.GetProperty(propertyName: "Hud") is not { ValueKind: JsonValueKind.Object } hud)
        {
            return null;
        }

        long elapsed = header.GetProperty(propertyName: "RootTime").GetInt64();
        return new FoundationHudState
        {
            Survivor = survivor,
            Generation = generation,
            Revision = view.Revision,
            RunTime = elapsed,
            Health = Take(value: header, field: "Health"),
            Stamina = Take(value: header, field: "Stamina"),
            MaximumHealth = Take(value: header, field: "MaximumHealth"),
            MaximumStamina = Take(value: header, field: "MaximumStamina"),
            Rest = Text(value: hud, field: "Rest"),
            Load = Text(value: hud, field: "Load"),
            Weight = Take(value: hud, field: "Weight"),
            Soft = Take(value: hud, field: "Soft"),
            Severe = Take(value: hud, field: "Severe"),
            Speed = Take(value: hud, field: "Speed"),
            Cost = Take(value: hud, field: "Cost"),
            Money = hud.GetProperty(propertyName: nameof(Money)).GetInt64(),
            Left = ReadItem(value: hud.GetProperty(propertyName: nameof(Left))),
            Right = ReadItem(value: hud.GetProperty(propertyName: nameof(Right))),
            LeftAttack = hud.GetProperty(propertyName: nameof(LeftAttack)).GetBoolean(),
            RightAttack = hud.GetProperty(propertyName: nameof(RightAttack)).GetBoolean(),
            Ammo = (int?)Optional(value: hud, field: "Ammo"),
            FlinchAt = Optional(value: hud, field: "FlinchAt"),
            AttackShade = Sweep(elapsed: elapsed, from: Optional(value: hud, field: "AttackAt"),
                    due: Optional(value: hud, field: "AttackReady")),
            FoodShade = Sweep(elapsed: elapsed, from: Optional(value: hud, field: "FoodAt"),
                    due: Optional(value: hud, field: "FoodReady")),
            Food = [.. hud.GetProperty(propertyName: nameof(Food)).EnumerateArray().Select(selector: ReadItem)],
            SprintBlock = Text(value: hud, field: "SprintBlock"),
        };
    }

    internal static bool ShowsDamage(FoundationHudState? previous, FoundationHudState next)
    {
        return previous is not null && previous.Generation == next.Generation && next.Revision >= previous.Revision
                && StringComparer.Ordinal.Equals(x: previous.Survivor, y: next.Survivor)
                && next.Health > 0 && next.Health < previous.Health && next.FlinchAt.HasValue
                && next.FlinchAt != previous.FlinchAt;
    }

    internal static string? BlockedNotice(string kind, string status)
    {
        return (kind, status) switch
        {
            ("attack", "Cooldown") => "Attack cooling down.",
            ("attack", "InsufficientStamina") => "Not enough stamina to attack.",
            ("attack", "BrokenWeapon") => "Weapon broken. Free a hand or change weapons.",
            ("attack", "HandsBusy" or "IncompatibleHands") => "Hands incompatible. Check your equipped items.",
            ("food", "Cooldown") => "Food cooling down.",
            ("food", "StaleFood" or "InvalidFood") => "Food stack unavailable. Assign carried food.",
            ("jump", "Overloaded") => "Too much weight to jump.",
            ("jump", "InsufficientStamina") => "Not enough stamina to jump.",
            ("sprint", "Overloaded") => "Too much weight to sprint.",
            ("sprint", "InsufficientStamina") => "Not enough stamina to sprint.",
            _ => null,
        };
    }

    internal static string Number(decimal value)
    {
        return value.ToString(format: "0.##", provider: CultureInfo.InvariantCulture);
    }

    private static string Band(decimal weight, decimal soft, decimal severe)
    {
        return weight >= severe ? "Overloaded" : weight > soft ? "Heavy" : "Normal";
    }

    private static decimal Sweep(long elapsed, decimal? from, decimal? due)
    {
        return from is { } start && due is { } end && end > start
                ? Math.Clamp(value: (end - elapsed) / (end - start), min: 0, max: 1) : 0;
    }

    private static Item? ReadItem(JsonElement value)
    {
        return value.ValueKind == JsonValueKind.Null ? null : new Item(
                id: value.GetProperty(propertyName: "Id").GetInt64(), definition: Text(value: value,
                        field: "DefinitionId"), icon: Text(value: value, field: "CanonicalIcon"),
                quantity: value.GetProperty(propertyName: "Quantity").GetInt32(),
                condition: Optional(value: value, field: "Condition"),
                maximum: Optional(value: value, field: "MaximumCondition"));
    }

    private static string Text(JsonElement value, string field)
    {
        return value.GetProperty(propertyName: field).GetString() ?? "";
    }

    private static decimal Take(JsonElement value, string field)
    {
        return value.GetProperty(propertyName: field).GetDecimal();
    }

    private static decimal? Optional(JsonElement value, string field)
    {
        JsonElement result = value.GetProperty(propertyName: field);
        return result.ValueKind == JsonValueKind.Null ? null : result.GetDecimal();
    }

    internal sealed record Item
    {
        internal Item(long id, string definition, string icon, int quantity, decimal? condition, decimal? maximum)
        {
            Id = id;
            Definition = definition;
            Icon = icon;
            Quantity = quantity;
            Condition = condition;
            Maximum = maximum;
        }

        internal long Id { get; }
        internal string Definition { get; }
        internal string Icon { get; }
        internal int Quantity { get; }
        internal decimal? Condition { get; }
        internal decimal? Maximum { get; }
    }
}
