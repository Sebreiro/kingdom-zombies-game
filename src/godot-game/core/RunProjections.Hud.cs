using System.Linq;
using System.Text.Json;

namespace KingdomZombies.Core;

internal static partial class RunProjections
{
    private static JsonElement LivingHud(FoundationSession.RunPublication root, RunPopulation.Survivor owner)
    {
        RunInventory inventory = root.Inventory;
        RunDefinitions definitions = root.Definitions;
        RunLocomotion.Burden load = RunLocomotion.Load(inventory: inventory, definitions: definitions,
                survivorId: owner.Id);
        RunInventory.Item? left = inventory.Equipped(survivorId: owner.Id, slot: "left-hand");
        RunInventory.Item? right = inventory.Equipped(survivorId: owner.Id, slot: "right-hand");
        RunCombat.BowView? bow = RunCombat.ViewBow(inventory: inventory, survivor: owner,
                aim: new RunCombat.Aim(x: 0, y: 0));
        (bool left, bool right) hands = TimedHands(definitions: definitions.Combat, left: left, right: right,
                bow: bow);
        return JsonSerializer.SerializeToElement(value: new
        {
            Rest = RunVitals.RestState(survivor: owner, definitions: definitions, now: root.ElapsedMilliseconds)
                    .ToString(),
            load.Weight,
            Load = load.Level.ToString(),
            load.Speed,
            Cost = load.ActionCost,
            Soft = definitions.Locomotion.Load.Soft,
            Severe = definitions.Locomotion.Load.Severe,
            Money = inventory.CarriedBy(survivorId: owner.Id).Where(predicate: item => item.DefinitionId is "IT-01")
                    .Sum(selector: item => (long)item.Quantity),
            Left = HudItem(root: root, item: left),
            Right = HudItem(root: root, item: right),
            LeftAttack = hands.left,
            RightAttack = hands.right,
            Ammo = bow?.ReadyArrows,
            AttackAt = owner.StrikeAt,
            AttackReady = owner.AttackAvailableAt,
            FoodAt = owner.FoodAvailableAt - definitions.Vitals.FoodCooldown,
            FoodReady = owner.FoodAvailableAt,
            owner.FlinchAt,
            SprintBlock = owner.SprintBlock.ToString(),
            Food = Enumerable.Range(start: 1, count: 4).Select(selector: slot =>
                    HudItem(root: root, item: RunQuickReferences.Resolve(inventory: inventory,
                            definitions: definitions, survivorId: owner.Id, slot: slot)?.Item)),
        });
    }

    private static JsonElement? HudItem(FoundationSession.RunPublication root, RunInventory.Item? item)
    {
        if (item is null)
        {
            return null;
        }

        RunDefinitions.Item definition = root.Definitions.Items[item.DefinitionId];
        return JsonSerializer.SerializeToElement(value: new
        {
            item.Id,
            item.DefinitionId,
            definition.Appearance.CanonicalIcon,
            item.Quantity,
            item.Condition,
            definition.MaximumCondition,
        });
    }

    private static (bool left, bool right) TimedHands(RunCombatDefinitions definitions, RunInventory.Item? left,
            RunInventory.Item? right, RunCombat.BowView? bow)
    {
        if (bow is not null)
        {
            return (left?.Id == bow.BowId, right?.Id == bow.BowId);
        }

        bool leftWeapon = MeleeIcon(definitions: definitions, item: left);
        bool rightWeapon = MeleeIcon(definitions: definitions, item: right);
        return leftWeapon || rightWeapon ? (leftWeapon, rightWeapon) : (left is null, right is null);
    }

    private static bool MeleeIcon(RunCombatDefinitions definitions, RunInventory.Item? item)
    {
        return item is not null && definitions.Weapons.TryGetValue(key: item.DefinitionId,
                value: out RunCombatDefinitions.Attack? rule) && rule.Reach.HasValue;
    }
}
