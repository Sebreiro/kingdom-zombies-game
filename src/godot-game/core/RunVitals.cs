using System;
using System.Collections.Generic;

namespace KingdomZombies.Core;

/// <summary>Applies bounded resources at exact points and intervals on the shared clock.</summary>
internal static class RunVitals
{
    internal static RunPopulation.Survivor? Spend(RunPopulation.Survivor survivor, decimal amount, decimal now)
    {
        return amount <= 0 || survivor.Stamina < amount ? null : ResetRest(survivor: survivor) with
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
        if (elapsed == 0 || survivor.Stamina >= definitions.MaximumStamina)
        {
            return survivor;
        }

        return survivor with
        {
            Stamina = Math.Min(val1: definitions.MaximumStamina,
                    val2: survivor.Stamina + elapsed * settings.StaminaPerSecond / 1000),
        };
    }

    internal static RunPopulation.Survivor ResetRest(RunPopulation.Survivor survivor)
    {
        return survivor.RestStartedAt.HasValue ? survivor with { RestStartedAt = null } : survivor;
    }

    internal static RunPopulation.Survivor RecheckRest(RunPopulation.Survivor survivor, RunWorld world)
    {
        decimal x = world.Origin.X + survivor.PositionSubunitsExact;
        return survivor.Health > 0 && !survivor.JumpStartedAt.HasValue && world.SafeGroundRest(x: x)
                ? survivor : ResetRest(survivor: survivor);
    }

    internal static RestMode RestState(RunPopulation.Survivor survivor, RunDefinitions definitions, long now)
    {
        if (survivor.RestStartedAt is not { } started)
        {
            return RestMode.Unsafe;
        }

        bool recovering = survivor.Health < definitions.MaximumHealth && definitions.Vitals.HealthPerSecond > 0
                && now >= started + definitions.Vitals.RestDelay;
        return recovering ? RestMode.Recovering : RestMode.Resting;
    }

    internal static RunPopulation.Survivor RestAfterWalk(RunPopulation.Survivor survivor, RunDefinitions definitions,
            RunWorld world, decimal from, decimal velocity, decimal previous, decimal moving, decimal current)
    {
        decimal start = world.Origin.X + from;
        decimal end = world.Origin.X + survivor.PositionSubunitsExact;
        if (start != end)
        {
            survivor = WalkIntervals(survivor: survivor, definitions: definitions, world: world, start: start,
                    velocity: velocity, previous: previous, duration: moving);
        }

        return RestFor(survivor: survivor, definitions: definitions, previous: previous + moving, current: current,
                safe: world.SafeGroundRest(x: end));
    }

    private static RunPopulation.Survivor WalkIntervals(RunPopulation.Survivor survivor, RunDefinitions definitions,
            RunWorld world, decimal start, decimal velocity, decimal previous, decimal duration)
    {
        var stops = new SortedDictionary<decimal, bool>();
        (long left, long right) = world.CampBorders;
        AddStop(stops: stops, at: (left - start) / velocity, duration: duration, dangerous: false);
        AddStop(stops: stops, at: (right - start) / velocity, duration: duration, dangerous: false);
        AddDangerStops(stops: stops, world: world, start: start, velocity: velocity, duration: duration);
        stops[duration] = false;

        RunPopulation.Survivor rested = world.SafeGroundRest(x: start) ? survivor : ResetRest(survivor: survivor);
        decimal last = 0;
        foreach ((decimal at, bool dangerous) in stops)
        {
            decimal midpoint = start + velocity * ((last + at) / 2);
            rested = RestFor(survivor: rested, definitions: definitions, previous: previous + last,
                    current: previous + at, safe: world.SafeGroundRest(x: midpoint));
            if (dangerous)
            {
                rested = ResetRest(survivor: rested);
            }

            last = at;
        }

        return rested;
    }

    private static void AddDangerStops(SortedDictionary<decimal, bool> stops, RunWorld world, decimal start,
            decimal velocity, decimal duration)
    {
        long range = world.Settings.DangerUnits * RunWorld.SubunitsPerUnit;
        long ground = world.Origin.Y + world.GroundContactSubunits;
        foreach (RunWorld.Hostile hostile in world.Hostiles)
        {
            long dy = hostile.Contact.Y - ground;
            if (Math.Abs(value: dy) > range)
            {
                continue;
            }

            decimal square = range * range - dy * dy;
            decimal radius = (decimal)Math.Sqrt(d: (double)square);
            if (radius != 0)
            {
                radius = (radius + square / radius) / 2;
            }

            decimal center = hostile.Contact.X - start;
            AddStop(stops: stops, at: (center - radius) / velocity, duration: duration, dangerous: true);
            AddStop(stops: stops, at: (center + radius) / velocity, duration: duration, dangerous: true);
        }
    }

    private static void AddStop(SortedDictionary<decimal, bool> stops, decimal at, decimal duration, bool dangerous)
    {
        if (at > 0 && at < duration)
        {
            stops[at] = dangerous || stops.GetValueOrDefault(key: at);
        }
    }

    private static RunPopulation.Survivor RestFor(RunPopulation.Survivor survivor, RunDefinitions definitions,
            decimal previous, decimal current, bool safe)
    {
        if (!safe)
        {
            return ResetRest(survivor: survivor);
        }

        Settings settings = definitions.Vitals;
        decimal started = survivor.RestStartedAt ?? previous;
        decimal recovery = Math.Max(val1: 0, val2: current - Math.Max(val1: previous,
                val2: started + settings.RestDelay));
        return survivor with
        {
            RestStartedAt = started,
            Health = Math.Min(val1: definitions.MaximumHealth,
                    val2: survivor.Health + recovery * settings.HealthPerSecond / 1000),
        };
    }

    internal enum RestMode { Unsafe, Resting, Recovering }

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
