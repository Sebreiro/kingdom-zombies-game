using System;

namespace KingdomZombies.Core;

/// <summary>Applies one opposing hit using equipment condition before that hit.</summary>
internal static class RunDamage
{
    internal static (RunInventory inventory, RunPopulation.Survivor survivor, Hit? receipt) Apply(
            RunInventory inventory, RunCombatDefinitions definitions, RunPopulation.Survivor survivor,
            string sourceId, bool sourceHostile, decimal raw, decimal at)
    {
        if (raw < 0 || survivor.Health <= 0 || survivor.Hostile == sourceHostile)
        {
            return (inventory, survivor, null);
        }

        decimal remaining = 1;
        foreach (string slot in _armorSlots)
        {
            RunInventory.Item? gear = inventory.Equipped(survivorId: survivor.Id, slot: slot);
            if (gear is { Condition: > 0 } && definitions.ArmorByItem.TryGetValue(key: gear.DefinitionId,
                    value: out RunCombatDefinitions.Armor? rule))
            {
                remaining *= 1 - rule.Reduction;
                inventory = Wear(inventory: inventory, item: gear, amount: raw * rule.Wear);
            }
        }

        decimal harm = raw * Math.Max(val1: 1 - definitions.ArmorCap, val2: remaining);
        decimal health = Math.Max(val1: 0, val2: survivor.Health - harm);
        RunPopulation.Survivor next = health == survivor.Health ? survivor : survivor with { Health = health };
        var hit = new Hit(sourceId: sourceId, targetId: survivor.Id, raw: raw, damage: harm, healthBefore:
                survivor.Health, healthAfter: health, at: at);
        return (inventory, next, hit);
    }

    internal static RunInventory Wear(RunInventory inventory, RunInventory.Item item, decimal amount)
    {
        return item.Condition is not { } condition || condition <= 0 || amount <= 0 ? inventory
                : inventory.WithItems(items: inventory.Items.Replace(oldValue: item,
                        newValue: item with { Condition = Math.Max(val1: 0, val2: condition - amount) }));
    }

    private static readonly string[] _armorSlots = ["body-armor", "headwear"];

    internal sealed record Hit
    {
        internal Hit(string sourceId, string targetId, decimal raw, decimal damage, decimal healthBefore,
                decimal healthAfter, decimal at)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Raw = raw;
            Damage = damage;
            HealthBefore = healthBefore;
            HealthAfter = healthAfter;
            At = at;
        }

        internal string SourceId { get; }
        internal string TargetId { get; }
        internal decimal Raw { get; }
        internal decimal Damage { get; }
        internal decimal HealthBefore { get; }
        internal decimal HealthAfter { get; }
        internal decimal At { get; }
        internal bool Lethal => HealthBefore > 0 && HealthAfter == 0;
    }
}
