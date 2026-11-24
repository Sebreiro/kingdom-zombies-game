using System;

namespace KingdomZombies.Core;

/// <summary>Prepares one carried food consumption and the survivor's matching vitals change.</summary>
internal static class RunFood
{
    internal static Status Prepare(FoundationSession.RunPublication current, RunPopulation.Survivor survivor,
            Request request, out RunInventory? inventory, out RunPopulation.Survivor? fed)
    {
        inventory = null;
        fed = null;
        RunDefinitions definitions = current.Definitions;
        RunInventory.ItemPath expected = request.Expected;
        if (expected.Items.IsDefaultOrEmpty || !expected.IsCarriedBy(survivorId: survivor.Id)
                || !definitions.Food.TryGetValue(key: expected.Item.DefinitionId, value: out Nutrition? nutrition))
        {
            return Status.InvalidFood;
        }

        if (request.QuickSlot is { } slot && RunQuickReferences.Resolve(inventory: current.Inventory,
                definitions: definitions, survivorId: survivor.Id, slot: slot)?.Item.Id != expected.Item.Id)
        {
            return Status.StaleFood;
        }

        if (survivor.FoodAvailableAt is { } ready && current.ElapsedMilliseconds < ready)
        {
            return Status.Cooldown;
        }

        inventory = RunQuickReferences.SpendCarriedFood(inventory: current.Inventory, definitions: definitions,
                survivorId: survivor.Id, expected: expected);
        if (inventory is null)
        {
            return Status.StaleFood;
        }

        fed = survivor with
        {
            Health = Math.Min(val1: definitions.MaximumHealth, val2: survivor.Health + nutrition.Health),
            Stamina = Math.Min(val1: definitions.MaximumStamina, val2: survivor.Stamina + nutrition.Stamina),
            FoodAvailableAt = current.ElapsedMilliseconds + definitions.Vitals.FoodCooldown,
        };
        return Status.Consumed;
    }

    internal enum Status { Consumed, AccessLost, StaleRequest, InvalidFood, StaleFood, Cooldown, Rejected }

    internal sealed record Nutrition
    {
        internal Nutrition(decimal health, decimal stamina)
        {
            Health = health;
            Stamina = stamina;
        }

        internal decimal Health { get; }
        internal decimal Stamina { get; }
    }

    internal sealed record Request
    {
        internal Request(RunInventory.ItemPath expected, int? quickSlot = null)
        {
            Expected = expected;
            QuickSlot = quickSlot;
        }

        internal RunInventory.ItemPath Expected { get; }
        internal int? QuickSlot { get; }
    }

    internal sealed record Result
    {
        internal Result(Status status, RunPopulation.Survivor? survivor, bool replayed = false)
        {
            Status = status;
            Survivor = survivor;
            Replayed = replayed;
        }

        internal Status Status { get; }
        internal RunPopulation.Survivor? Survivor { get; }
        internal bool Replayed { get; }
    }
}
