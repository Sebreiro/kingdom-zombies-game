using System;

namespace KingdomZombies.Core;

/// <summary>Applies bounded resources at exact points and intervals on the shared clock.</summary>
internal static class RunVitals
{
    internal static RunPopulation.Survivor? Spend(RunPopulation.Survivor survivor, decimal amount, decimal now)
    {
        return amount <= 0 || survivor.Stamina < amount ? null : survivor with
        {
            Stamina = survivor.Stamina - amount,
            LastStaminaSpend = now,
        };
    }

    internal static RunPopulation.Survivor RegenerateStamina(RunPopulation.Survivor survivor,
            RunDefinitions definitions, decimal previous, decimal current)
    {
        Settings settings = definitions.Vitals;
        decimal start = survivor.LastStaminaSpend is { } spent ? Math.Max(val1: previous,
                val2: spent + settings.StaminaDelay) : previous;
        decimal elapsed = Math.Max(val1: 0, val2: current - start);
        return survivor with
        {
            Stamina = Math.Min(val1: definitions.MaximumStamina,
                    val2: survivor.Stamina + elapsed * settings.StaminaPerSecond / 1000),
        };
    }

    internal sealed record Settings
    {
        internal Settings(decimal staminaPerSecond, decimal staminaDelay, decimal healthPerSecond,
                decimal restDelay, decimal foodCooldown)
        {
            StaminaPerSecond = staminaPerSecond;
            StaminaDelay = staminaDelay;
            HealthPerSecond = healthPerSecond;
            RestDelay = restDelay;
            FoodCooldown = foodCooldown;
        }

        internal static Settings Default { get; } = new(staminaPerSecond: 14, staminaDelay: 500,
                healthPerSecond: 2, restDelay: 5000, foodCooldown: 2000);
        internal decimal StaminaPerSecond { get; }
        internal decimal StaminaDelay { get; }
        internal decimal HealthPerSecond { get; }
        internal decimal RestDelay { get; }
        internal decimal FoodCooldown { get; }
    }
}
