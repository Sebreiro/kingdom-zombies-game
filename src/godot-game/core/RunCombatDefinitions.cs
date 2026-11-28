using System.Collections.Immutable;

namespace KingdomZombies.Core;

/// <summary>Contains the validated rules for attacks, arrows and wearable protection.</summary>
internal sealed class RunCombatDefinitions
{
    internal RunCombatDefinitions(Attack fists, ImmutableDictionary<string, Attack> weapons,
            ImmutableDictionary<string, Armor> armorByItem, Projectile arrow, decimal armorCap, decimal meleeWear,
            decimal bowWear)
    {
        Fists = fists;
        Weapons = weapons;
        ArmorByItem = armorByItem;
        Arrow = arrow;
        ArmorCap = armorCap;
        MeleeWear = meleeWear;
        BowWear = bowWear;
    }

    internal Attack Fists { get; }
    internal ImmutableDictionary<string, Attack> Weapons { get; }
    internal ImmutableDictionary<string, Armor> ArmorByItem { get; }
    internal Projectile Arrow { get; }
    internal decimal ArmorCap { get; }
    internal decimal MeleeWear { get; }
    internal decimal BowWear { get; }

    internal sealed record Attack
    {
        internal Attack(decimal damage, decimal interval, decimal? reach, decimal stamina)
        {
            Damage = damage;
            Interval = interval;
            Reach = reach;
            Stamina = stamina;
        }

        internal decimal Damage { get; }
        internal decimal Interval { get; }
        internal decimal? Reach { get; }
        internal decimal Stamina { get; }
    }

    internal sealed record Armor
    {
        internal Armor(decimal reduction, decimal wear)
        {
            Reduction = reduction;
            Wear = wear;
        }

        internal decimal Reduction { get; }
        internal decimal Wear { get; }
    }

    internal sealed record Projectile
    {
        internal Projectile(decimal speed, decimal lifetime)
        {
            Speed = speed;
            Lifetime = lifetime;
        }

        internal decimal Speed { get; }
        internal decimal Lifetime { get; }
    }
}
