using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the ordinary host's melee and per-hit armor authority.</summary>
[Trait("Tier", "1")]
public sealed class Phase06MeleeTests
{
    /// <summary>Each hand can hit the same living target once while paying one combined action.</summary>
    [Fact]
    public void Phase06_E080_DualMeleeCanHitOneAdjacentTargetTwice()
    {
        FoundationSession session = Start();
        Enemy(session: session, id: "first", x: -7500);
        Enemy(session: session, id: "adjacent", x: -7400);
        RunInventory.Item knife = Equip(session: session, id: "IT-03", slot: "left-hand");
        RunInventory.Item club = Equip(session: session, id: "IT-04", slot: "right-hand");
        RunCombat.Outcome strike = Attack(session: session);
        strike.Status.ShouldBe(expected: RunCombat.Status.Accepted);
        strike.Hits.Select(selector: hit => hit.TargetId).ShouldBe(expected: ["first", "first"]);
        strike.Hits.Select(selector: hit => hit.Raw).ShouldBe(expected: [10m, 18m]);
        Survivor(session: session, id: "first").Health.ShouldBe(expected: 72);
        Survivor(session: session, id: "adjacent").Health.ShouldBe(expected: 100);
        Survivor(session: session).Stamina.ShouldBe(expected: 89);
        Survivor(session: session).AttackAvailableAt.ShouldBe(expected: 900);
        session.Inventory.Find(itemId: knife.Id)?.Condition.ShouldBe(expected: 119);
        session.Inventory.Find(itemId: club.Id)?.Condition.ShouldBe(expected: 149);
    }

    /// <summary>Fists include exact reach and cost boundaries and observe the committed facing.</summary>
    [Fact]
    public void Phase06_E081_ReachFacingAndExactlyEnoughStaminaAreInclusive()
    {
        FoundationSession session = Start();
        Enemy(session: session, id: "back", x: -8001);
        Enemy(session: session, id: "edge", x: -7000);
        session.TryUpdateVitals(survivorId: Owner, health: 100, stamina: 3).ShouldBeTrue();
        RunCombat.Outcome strike = Attack(session: session);
        strike.Hits.Single().TargetId.ShouldBe(expected: "edge");
        strike.Hits.Single().Damage.ShouldBe(expected: 8);
        Survivor(session: session).Stamina.ShouldBe(expected: 0);
        Survivor(session: session, id: "back").Health.ShouldBe(expected: 100);
        FoundationSession shortStamina = Start();
        shortStamina.TryUpdateVitals(survivorId: Owner, health: 100, stamina: 2.999m).ShouldBeTrue();
        FoundationSnapshot before = shortStamina.Snapshot;
        Attack(session: shortStamina).Status.ShouldBe(expected: RunCombat.Status.InsufficientStamina);
        shortStamina.Snapshot.ShouldBeSameAs(expected: before);
    }

    /// <summary>Two submissions at the same sequence publish one swing, wear and revision.</summary>
    [Fact]
    public async Task Phase06_E082_ConcurrentMeleeRequestsCommitOneSwing()
    {
        FoundationSession session = Start();
        Enemy(session: session, id: "enemy", x: -7500);
        RunInventory.Item knife = Equip(session: session, id: "IT-03", slot: "left-hand");
        long revision = session.Snapshot.Revision;
        Task<RunCombat.Outcome> first = Task.Run(function: () => Attack(session: session),
                cancellationToken: TestContext.Current.CancellationToken);
        Task<RunCombat.Outcome> second = Task.Run(function: () => Attack(session: session),
                cancellationToken: TestContext.Current.CancellationToken);
        RunCombat.Outcome[] results = await Task.WhenAll(tasks: [first, second]);
        results.Count(predicate: result => result.Replayed).ShouldBe(expected: 1);
        results.Sum(selector: result => result.Hits.Length).ShouldBe(expected: 1);
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
        Survivor(session: session, id: "enemy").Health.ShouldBe(expected: 90);
        Survivor(session: session).Stamina.ShouldBe(expected: 96);
        session.Inventory.Find(itemId: knife.Id)?.Condition.ShouldBe(expected: 119);
    }

    /// <summary>A miss spends once, while two unusable held items require a hand to be freed.</summary>
    [Fact]
    public void Phase06_E083_MissesAndBrokenHandsPreserveHeldGear()
    {
        FoundationSession session = Start();
        Attack(session: session).Hits.ShouldBeEmpty();
        Survivor(session: session).Stamina.ShouldBe(expected: 97);
        FoundationSession broken = Start();
        RunInventory.Item knife = Equip(session: broken, id: "IT-03", slot: "left-hand", condition: 0);
        RunInventory.Item club = Equip(session: broken, id: "IT-04", slot: "right-hand", condition: 0);
        Attack(session: broken).Status.ShouldBe(expected: RunCombat.Status.BrokenWeapon);
        Survivor(session: broken).AttackAvailableAt.ShouldBeNull();
        Drop(session: broken, item: club);
        Enemy(session: broken, id: "enemy", x: -7500);
        Attack(session: broken, sequence: 2).Hits.Single().Raw.ShouldBe(expected: 8);
        broken.Inventory.Find(itemId: knife.Id).ShouldBeSameAs(expected: knife);
        broken.Inventory.Find(itemId: club.Id)?.Condition.ShouldBe(expected: 0);
    }

    /// <summary>Rejected cooldown presses never turn into an attack at the later deadline.</summary>
    [Fact]
    public void Phase06_E084_CooldownReplaysDoNotQueueAtTheBoundary()
    {
        FoundationSession session = Start();
        Enemy(session: session, id: "enemy", x: -7500);
        Attack(session: session).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 699);
        FoundationSnapshot blocked = session.Snapshot;
        Attack(session: session, sequence: 2).Status.ShouldBe(expected: RunCombat.Status.Cooldown);
        session.Snapshot.ShouldBeSameAs(expected: blocked);
        session.Step(elapsedMilliseconds: 1);
        Attack(session: session, sequence: 2).Status.ShouldBe(expected: RunCombat.Status.Cooldown);
        Survivor(session: session, id: "enemy").Health.ShouldBe(expected: 92);
        Attack(session: session, sequence: 3).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        Survivor(session: session, id: "enemy").Health.ShouldBe(expected: 84);
        Survivor(session: session).AttackAvailableAt.ShouldBe(expected: 1400);
    }

    /// <summary>Equal reach candidates use ordinal identity, and a usable hand works beside a broken hand.</summary>
    [Fact]
    public void Phase06_E085_EqualMeleeContactsHaveAStableWinner()
    {
        FoundationSession session = Start();
        Enemy(session: session, id: "zebra", x: -7500);
        Enemy(session: session, id: "alpha", x: -7500);
        RunInventory.Item broken = Equip(session: session, id: "IT-04", slot: "left-hand", condition: 0);
        _ = Equip(session: session, id: "IT-03", slot: "right-hand");
        Attack(session: session).Hits.Single().TargetId.ShouldBe(expected: "alpha");
        Survivor(session: session, id: "zebra").Health.ShouldBe(expected: 100);
        Survivor(session: session).Stamina.ShouldBe(expected: 96);
        Survivor(session: session).AttackAvailableAt.ShouldBe(expected: 500);
        session.Inventory.Find(itemId: broken.Id).ShouldBeSameAs(expected: broken);
    }

    /// <summary>Nearest fractional positions on either side of the exact reach stay distinct.</summary>
    [Fact]
    public void Phase06_E086_FractionalReachDoesNotRoundIntoAHit()
    {
        FoundationSession outside = Start();
        Enemy(session: outside, id: "enemy", x: -6899.999m);
        RunInventory.Item knife = Equip(session: outside, id: "IT-03", slot: "left-hand");
        Attack(session: outside).Hits.ShouldBeEmpty();
        outside.Inventory.Find(itemId: knife.Id).ShouldBeSameAs(expected: knife);
        FoundationSession inside = Start();
        Enemy(session: inside, id: "enemy", x: -6900.001m);
        _ = Equip(session: inside, id: "IT-03", slot: "left-hand");
        Attack(session: inside).Hits.Single().Raw.ShouldBe(expected: 10);
    }

    /// <summary>Plated armor and helmet multiply their protection and wear from the same raw 32.</summary>
    [Fact]
    public void Phase06_E094_ExactArmorReductionAndRawWear()
    {
        FoundationSession session = Start();
        (RunInventory.Item body, RunInventory.Item helmet) = Armor(session: session, body: 200, helmet: 100);
        BatEnemy(session: session, id: "enemy");
        RunDamage.Hit hit = session.AttackUncontrolled(survivorId: "enemy", sequence: 1).Hits.Single();
        (hit.Raw, hit.Damage, hit.HealthAfter).ShouldBe(expected: (32m, 17.28m, 82.72m));
        session.Inventory.Find(itemId: body.Id)?.Condition.ShouldBe(expected: 193.6m);
        session.Inventory.Find(itemId: helmet.Id)?.Condition.ShouldBe(expected: 96.8m);
    }

    /// <summary>Competing hits serialize behind the same living armor and each sees current condition.</summary>
    [Fact]
    public async Task Phase06_E095_ConcurrentHitsReadTheNewArmorCondition()
    {
        FoundationSession session = Start();
        _ = Armor(session: session, body: 1, helmet: 1);
        BatEnemy(session: session, id: "first");
        BatEnemy(session: session, id: "second");
        Task<RunCombat.Outcome> first = Task.Run(function: () => session.AttackUncontrolled(
                survivorId: "first", sequence: 1), cancellationToken: TestContext.Current.CancellationToken);
        Task<RunCombat.Outcome> second = Task.Run(function: () => session.AttackUncontrolled(
                survivorId: "second", sequence: 1), cancellationToken: TestContext.Current.CancellationToken);
        RunCombat.Outcome[] results = await Task.WhenAll(tasks: [first, second]);
        results.Select(selector: result => result.Hits.Single().Damage).Order()
                .ShouldBe(expected: [17.28m, 32m]);
        Survivor(session: session).Health.ShouldBe(expected: 50.72m);
    }

    /// <summary>A lethal hit is handed off once and a repeated command keeps its spent equipment.</summary>
    [Fact]
    public void Phase06_E096_DeathAndWearCannotBeReplayed()
    {
        FoundationSession session = Start();
        Enemy(session: session, id: "enemy", x: -7500, health: 1);
        RunInventory.Item knife = Equip(session: session, id: "IT-03", slot: "left-hand");
        RunDamage.Hit hit = Attack(session: session).Hits.Single();
        hit.Lethal.ShouldBeTrue();
        Attack(session: session).Replayed.ShouldBeTrue();
        session.UnderInventoryAuthority(action: current => current?.Combat.FatalHits.Single())
                .ShouldBeSameAs(expected: hit);
        session.Inventory.Find(itemId: knife.Id)?.Condition.ShouldBe(expected: 119);
        Survivor(session: session, id: "enemy").Health.ShouldBe(expected: 0);
    }

    /// <summary>The breaking hit receives full protection before the other weapon strikes.</summary>
    [Fact]
    public void Phase06_E097_DualHitsApplyFullProtectionThenNoProtection()
    {
        FoundationSession session = Start();
        (RunInventory.Item body, RunInventory.Item helmet) = Armor(session: session, body: 1, helmet: 1);
        BatEnemy(session: session, id: "enemy");
        _ = Equip(session: session, id: "IT-06", slot: "right-hand", owner: "enemy");
        RunCombat.Outcome attack = session.AttackUncontrolled(survivorId: "enemy", sequence: 1);
        attack.Hits.Select(selector: hit => hit.Damage).ShouldBe(expected: [17.28m, 32m]);
        session.Inventory.Find(itemId: body.Id)?.Condition.ShouldBe(expected: 0);
        session.Inventory.Find(itemId: helmet.Id)?.Condition.ShouldBe(expected: 0);
        session.Inventory.Equipped(survivorId: Owner, slot: "headwear")?.Id.ShouldBe(expected: helmet.Id);
        Survivor(session: session, id: "enemy").Stamina.ShouldBe(expected: 80);
    }

    /// <summary>Zero causes no wear, and fractional damage keeps its exact reduction and limits.</summary>
    [Fact]
    public void Phase06_E098_ZeroAndFractionalRawHitsKeepExactCondition()
    {
        FoundationSession session = Start();
        (RunInventory.Item body, RunInventory.Item helmet) = Armor(session: session, body: 0.03m, helmet: 0.02m);
        RunInventory original = session.Inventory;
        RunPopulation.Survivor survivor = Survivor(session: session);
        (RunInventory inventory, RunPopulation.Survivor survivor, RunDamage.Hit? receipt) zero =
                RunDamage.Apply(inventory: original, definitions: session.Definitions.Combat,
                        survivor: survivor, sourceId: "enemy", sourceHostile: true, raw: 0, at: 0);
        zero.inventory.ShouldBeSameAs(expected: original);
        zero.survivor.ShouldBeSameAs(expected: survivor);
        (RunInventory inventory, RunPopulation.Survivor survivor, RunDamage.Hit? receipt) next =
                RunDamage.Apply(inventory: original, definitions: session.Definitions.Combat,
                        survivor: survivor, sourceId: "enemy", sourceHostile: true, raw: 0.125m, at: 0);
        next.receipt.ShouldNotBeNull().Damage.ShouldBe(expected: 0.0675m);
        next.inventory.Find(itemId: body.Id)?.Condition.ShouldBe(expected: 0.005m);
        next.inventory.Find(itemId: helmet.Id)?.Condition.ShouldBe(expected: 0.0075m);
        CheckArmorCap();
    }

    /// <summary>P01 observes a real allied overlap and identical damage with and without a control lease.</summary>
    [Fact]
    public void P01_CleanControl_PreservesAlliesAndCosmeticCrown()
    {
        FoundationSession controlled = Start();
        FoundationSession ordinary = Start();
        Enemy(session: controlled, id: "enemy", x: -7500);
        Enemy(session: ordinary, id: "enemy", x: -7500);
        controlled.Population.FindLease(bindingId: "local")?.SurvivorId.ShouldBe(expected: Owner);
        ordinary.Population.Leases.Any(predicate: lease => lease.SurvivorId is "survivor-02").ShouldBeFalse();
        const string AllyId = "survivor-03";
        Survivor(session: controlled, id: AllyId).PositionSubunits.ShouldBe(expected:
                Survivor(session: controlled).PositionSubunits);
        decimal allyBefore = Survivor(session: controlled, id: AllyId).Health;
        decimal crowned = Attack(session: controlled).Hits.Single().Damage;
        decimal bare = ordinary.AttackUncontrolled(survivorId: "survivor-02", sequence: 1).Hits.Single().Damage;
        ObservesProhibition(allyBefore: allyBefore, allyAfter: Survivor(session: controlled, id: AllyId).Health,
                crowned: crowned, ordinary: bare, crownedItems: controlled.Inventory.CarriedBy(survivorId: Owner)
                        .Length, ordinaryItems: ordinary.Inventory.CarriedBy(survivorId: "survivor-02").Length)
                .ShouldBeTrue();
    }

    /// <summary>P01 rejects both fabricated allied harm and a fabricated crown effect.</summary>
    [Fact]
    public void P01_ViolationControl_DetectsAllyDamageOrCrownBenefit()
    {
        ObservesProhibition(allyBefore: 100, allyAfter: 99, crowned: 8, ordinary: 8, crownedItems: 3,
                ordinaryItems: 3).ShouldBeFalse();
        ObservesProhibition(allyBefore: 100, allyAfter: 100, crowned: 9, ordinary: 8, crownedItems: 3,
                ordinaryItems: 3).ShouldBeFalse();
        ObservesProhibition(allyBefore: 100, allyAfter: 100, crowned: 8, ordinary: 8, crownedItems: 4,
                ordinaryItems: 3).ShouldBeFalse();
    }

    internal const string Owner = "survivor-01";

    internal static FoundationSession Start(RunWorld? world = null)
    {
        return new FoundationSession(sessionId: "combat", playerId: "host", actorId: "actor",
                world: world ?? RunWorld.Foundation);
    }

    internal static RunItemActions.Controller Control()
    {
        return new RunItemActions.Controller(sessionId: "combat", bindingId: "local", generation: 1);
    }

    internal static void Enemy(FoundationSession session, string id, decimal x, decimal health = 100)
    {
        session.TryAddHostile(id: id, position: x, facing: -1, health: health).ShouldBeTrue();
    }

    internal static RunPopulation.Survivor Survivor(FoundationSession session, string id = Owner)
    {
        return session.Population.Survivors.Single(predicate: survivor =>
                StringComparer.Ordinal.Equals(x: survivor.Id, y: id));
    }

    internal static RunInventory.Item Equip(FoundationSession session, string id, string slot, string owner = Owner,
            decimal? condition = null, int quantity = 1)
    {
        RunDefinitions.Item rule = session.Definitions.Items[id];
        var item = new RunInventory.Item(id: session.Inventory.NextItemId, definitionId: id, quantity: quantity,
                condition: condition ?? rule.MaximumCondition, location: new RunInventory.Equipment(
                        survivorId: owner, slot: slot), storageGrid: rule.Storage);
        session.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(items:
                inventory.Items.Add(item: item), nextItemId: checked(item.Id + 1))).ShouldBeTrue();
        return item;
    }

    private static bool ObservesProhibition(decimal allyBefore, decimal allyAfter, decimal crowned, decimal ordinary,
            int crownedItems, int ordinaryItems)
    {
        return allyBefore == allyAfter && crowned == ordinary && crownedItems == ordinaryItems;
    }

    private static void CheckArmorCap()
    {
        var session = FoundationSession.CreateFromResources(sessionId: "combat", playerId: "host",
                actorId: "actor", read: ReadCappedArmor);
        _ = Armor(session: session, body: 200, helmet: 100);
        BatEnemy(session: session, id: "enemy");
        session.AttackUncontrolled(survivorId: "enemy", sequence: 1).Hits.Single().Damage
                .ShouldBe(expected: 25.6m);
    }

    private static byte[] ReadCappedArmor(string path, int limit)
    {
        byte[] bytes = Phase06LoadTests.Resource(path: path, limit: limit);
        return path is "content/config/combat.json" ? Encoding.UTF8.GetBytes(s: Encoding.UTF8.GetString(
                bytes: bytes).Replace(oldValue: "\"maximumReductionFraction\": 0.6",
                        newValue: "\"maximumReductionFraction\": 0.2", comparisonType: StringComparison.Ordinal))
                : bytes;
    }

    private static RunCombat.Outcome Attack(FoundationSession session, long sequence = 1)
    {
        return session.Attack(controller: Control(), sequence: sequence);
    }

    private static (RunInventory.Item body, RunInventory.Item helmet) Armor(FoundationSession session, decimal body,
            decimal helmet)
    {
        return (Equip(session: session, id: "IT-20", slot: "body-armor", condition: body),
                Equip(session: session, id: "IT-18", slot: "headwear", condition: helmet));
    }

    private static void BatEnemy(FoundationSession session, string id)
    {
        Enemy(session: session, id: id, x: -6400);
        _ = Equip(session: session, id: "IT-06", slot: "left-hand", owner: id);
    }

    private static void Drop(FoundationSession session, RunInventory.Item item)
    {
        RunInventory.ItemPath path = session.Inventory.PathTo(itemId: item.Id).ShouldNotBeNull();
        session.TryEditInventory(edit: (inventory, _, _) => RunItemTransfer.Drop(inventory: inventory,
                expected: path, pileId: "combat-fixture")).ShouldBeTrue();
    }
}
