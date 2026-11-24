using System;
using System.Linq;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves launch, first contact and ammo ownership in the host authority.</summary>
[Trait("Tier", "1")]
public sealed class Phase06BowTests
{
    /// <summary>A friendly arrow passes the allies and camp defense and stops at the first overlapping enemy.</summary>
    [Fact]
    public void Phase06_E087_ArrowPassesAlliesAndStopsAtTheFirstOverlappingEnemy()
    {
        FoundationSession session = Phase06MeleeTests.Start(world: World(defenses: [Wall(x: -6500)]));
        Phase06MeleeTests.Enemy(session: session, id: "first", x: -5000);
        Phase06MeleeTests.Enemy(session: session, id: "adjacent", x: -4900);
        _ = ArmBow(session: session);
        Fire(session: session).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 200);
        RunProjectiles.Impact impact = Combat(session: session).Impacts.Single();
        (impact.Kind, impact.TargetId, impact.Damage, impact.At)
                .ShouldBe(expected: (RunProjectiles.Kind.Survivor, "first", 22m, 110m));
        Combat(session: session).Arrows.ShouldBeEmpty();
        Phase06MeleeTests.Survivor(session: session, id: "first").Health.ShouldBe(expected: 78);
        Phase06MeleeTests.Survivor(session: session, id: "adjacent").Health.ShouldBe(expected: 100);
        Phase06MeleeTests.Survivor(session: session, id: "survivor-02").Health.ShouldBe(expected: 100);
        Defense(session: session).Health.ShouldBe(expected: 100);
    }

    /// <summary>The ready-hand count, sufficient stamina and the exact cooldown gate the real release.</summary>
    [Fact]
    public void Phase06_E088_QuietEmptyAndExactBowBoundariesUseTheOppositeHand()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunInventory.Item bow = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "left-hand");
        RunInventory.Item reserve = CarryReserve(session: session);
        var firstAim = new RunCombat.Aim(x: 500, y: 25);
        var nextAim = new RunCombat.Aim(x: 500, y: 40);
        CheckReadback(session: session, aim: firstAim, bow: bow, ready: 0);
        CheckReadback(session: session, aim: nextAim, bow: bow, ready: 0);
        Fire(session: session, aim: nextAim).Status.ShouldBe(expected: RunCombat.Status.EmptyAmmo);
        RunInventory.Item ammo = Phase06MeleeTests.Equip(session: session, id: "IT-02", slot: "right-hand",
                quantity: 2);
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 100, stamina: 5.999m).ShouldBeTrue();
        Fire(session: session, sequence: 2).Status.ShouldBe(expected: RunCombat.Status.InsufficientStamina);
        session.Inventory.Find(itemId: ammo.Id).ShouldBeSameAs(expected: ammo);
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 100, stamina: 6).ShouldBeTrue();
        Fire(session: session, sequence: 3).ArrowId.ShouldBe(expected: 1);
        Phase06MeleeTests.Survivor(session: session).Stamina.ShouldBe(expected: 0);
        CheckReadback(session: session, aim: nextAim, bow: bow, ready: 1);
        session.Inventory.Find(itemId: bow.Id)?.Condition.ShouldBe(expected: 159);
        session.Step(elapsedMilliseconds: 1199);
        Fire(session: session, sequence: 4).Status.ShouldBe(expected: RunCombat.Status.Cooldown);
        session.Step(elapsedMilliseconds: 1);
        Fire(session: session, sequence: 4).Status.ShouldBe(expected: RunCombat.Status.Cooldown);
        Fire(session: session, sequence: 5).ArrowId.ShouldBe(expected: 2);
        CheckReadback(session: session, aim: nextAim, bow: bow, ready: 0);
        session.Inventory.Find(itemId: ammo.Id).ShouldBeNull();
        session.Inventory.Find(itemId: reserve.Id).ShouldBeSameAs(expected: reserve);
        CheckBrokenAndIncompatible();
    }

    /// <summary>Competing releases publish one shot and one version of the real ammo and durability.</summary>
    [Fact]
    public async Task Phase06_E089_ConcurrentBowRequestsCommitOneRelease()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        (RunInventory.Item bow, RunInventory.Item ammo) = ArmBow(session: session, count: 2);
        long revision = session.Snapshot.Revision;
        Task<RunCombat.Outcome> first = Task.Run(function: () => Fire(session: session),
                cancellationToken: TestContext.Current.CancellationToken);
        Task<RunCombat.Outcome> second = Task.Run(function: () => Fire(session: session),
                cancellationToken: TestContext.Current.CancellationToken);
        RunCombat.Outcome[] results = await Task.WhenAll(tasks: [first, second]);
        results.Count(predicate: result => result.Replayed).ShouldBe(expected: 1);
        results.Count(predicate: result => result.ArrowId == 1).ShouldBe(expected: 1);
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
        session.Inventory.Find(itemId: ammo.Id)?.Quantity.ShouldBe(expected: 1);
        session.Inventory.Find(itemId: bow.Id)?.Condition.ShouldBe(expected: 159);
        Combat(session: session).Arrows.Single().Id.ShouldBe(expected: 1);
    }

    /// <summary>Empty air expires exactly at the lifetime and leaves fired ammunition spent.</summary>
    [Fact]
    public void Phase06_E090_AnExpiredArrowNeverCreatesLootOrRefillsTheHand()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        (RunInventory.Item bow, RunInventory.Item ammo) = ArmBow(session: session);
        Fire(session: session).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 1999);
        RunProjectiles.Arrow arrow = Combat(session: session).Arrows.Single();
        (arrow.At(now: 1999).x, arrow.ExpiresAt).ShouldBe(expected: (41975m, 2000m));
        session.Step(elapsedMilliseconds: 1);
        Combat(session: session).Arrows.ShouldBeEmpty();
        Combat(session: session).Impacts.ShouldBeEmpty();
        session.Inventory.Find(itemId: ammo.Id).ShouldBeNull();
        session.Inventory.Find(itemId: bow.Id)?.Condition.ShouldBe(expected: 159);
        session.Inventory.Items.Any(predicate: item => item.Location is RunInventory.Ground).ShouldBeFalse();
        CheckLifetimeEndpoint();
    }

    /// <summary>Launch and impacts are irrevocable when the shooter dies before the arrow arrives.</summary>
    [Fact]
    public void Phase06_E091_APreviouslyFiredArrowSurvivesShooterDeath()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        (RunInventory.Item bow, RunInventory.Item ammo) = ArmBow(session: session);
        Phase06MeleeTests.Enemy(session: session, id: "target", x: -2500);
        session.TryAddHostile(id: "killer", position: -8500, facing: 1, health: 100).ShouldBeTrue();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-06", slot: "left-hand", owner: "killer");
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 5, stamina: 100).ShouldBeTrue();
        Fire(session: session).ArrowId.ShouldBe(expected: 1);
        session.AttackUncontrolled(survivorId: "killer", sequence: 1).Hits.Single().Lethal.ShouldBeTrue();
        Fire(session: session).Status.ShouldBe(expected: RunCombat.Status.AccessLost);
        session.Step(elapsedMilliseconds: 300);
        RunProjectiles.Impact impact = Combat(session: session).Impacts.Single();
        (impact.ArrowId, impact.TargetId, impact.Damage).ShouldBe(expected: (1L, "target", 22m));
        session.Step(elapsedMilliseconds: 300);
        Phase06MeleeTests.Survivor(session: session, id: "target").Health.ShouldBe(expected: 78);
        session.Inventory.Find(itemId: ammo.Id).ShouldBeNull();
        session.Inventory.Find(itemId: bow.Id)?.Condition.ShouldBe(expected: 159);
    }

    /// <summary>Hostile arrows strike a live defense first and can pass it only after it breaks.</summary>
    [Fact]
    public void Phase06_E092_HostileDefenseTerrainAndSurvivorContactsHaveStableOrder()
    {
        FoundationSession session = Phase06MeleeTests.Start(world: World(defenses: [Wall(x: -5600, health: 30)]));
        StartEnemyBow(session: session, arrows: 3);
        EnemyFire(session: session, sequence: 1).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 1200);
        RunProjectiles.Impact first = Combat(session: session).Impacts.Single();
        (first.Kind, first.Damage, first.At).ShouldBe(expected: (RunProjectiles.Kind.Defense, 22m, 134m));
        Defense(session: session).Health.ShouldBe(expected: 8);
        EnemyFire(session: session, sequence: 2).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 1200);
        Defense(session: session).Living.ShouldBeFalse();
        Combat(session: session).Impacts.Single().Damage.ShouldBe(expected: 8);
        EnemyFire(session: session, sequence: 3).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 300);
        Combat(session: session).Impacts.Single().TargetId.ShouldBe(expected: Phase06MeleeTests.Owner);
        Phase06MeleeTests.Survivor(session: session).Health.ShouldBe(expected: 78);
        CheckTerrainTie();
        CheckActorTie();
    }

    /// <summary>Invalid aim leaves inventory unchanged and fractional aim yields equivalent swept contact.</summary>
    [Fact]
    public void Phase06_E093_AimBoundsAndFractionalSweepStayPrecise()
    {
        FoundationSession whole = Phase06MeleeTests.Start();
        FoundationSession split = Phase06MeleeTests.Start();
        _ = ArmBow(session: whole);
        _ = ArmBow(session: split);
        Phase06MeleeTests.Enemy(session: whole, id: "target", x: -3000.125m);
        Phase06MeleeTests.Enemy(session: split, id: "target", x: -3000.125m);
        FoundationSnapshot before = whole.Snapshot;
        long sequence = 0;
        double[] invalid = [double.NaN, double.PositiveInfinity, -1_000_000_000.25, -8000];
        foreach (double x in invalid)
        {
            Fire(session: whole, sequence: ++sequence, aim: new RunCombat.Aim(x: x, y: 0)).Status
                    .ShouldBe(expected: RunCombat.Status.InvalidAim);
        }

        whole.Snapshot.ShouldBeSameAs(expected: before);
        var aim = new RunCombat.Aim(x: -1000.25, y: 0.125);
        Fire(session: whole, sequence: ++sequence, aim: aim).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        Fire(session: split, aim: aim).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        Combat(session: whole).Arrows.Single().Aim.ShouldBeSameAs(expected: aim);
        whole.Step(elapsedMilliseconds: 200);
        split.Step(elapsedMilliseconds: 100);
        Combat(session: split).Impacts.ShouldBeEmpty();
        split.Step(elapsedMilliseconds: 100);
        RunProjectiles.Impact wholeHit = Combat(session: whole).Impacts.Single();
        RunProjectiles.Impact splitHit = Combat(session: split).Impacts.Single();
        wholeHit.TargetId.ShouldBe(expected: splitHit.TargetId);
        wholeHit.Damage.ShouldBe(expected: splitHit.Damage);
        Math.Abs(value: wholeHit.At - splitHit.At).ShouldBeLessThan(expected: 0.000001m);
    }

    private static void CheckLifetimeEndpoint()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        Phase06MeleeTests.Enemy(session: session, id: "endpoint", x: 42250);
        _ = ArmBow(session: session);
        Fire(session: session).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 2000);
        RunProjectiles.Impact impact = Combat(session: session).Impacts.Single();
        (impact.At, impact.TargetId).ShouldBe(expected: (2000m, "endpoint"));
    }

    private static void CheckBrokenAndIncompatible()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunInventory.Item bow = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "right-hand",
                condition: 0);
        RunInventory.Item weapon = session.Inventory.Items.First(predicate: item => item.DefinitionId is "IT-04");
        RunInventory.ItemPath path = session.Inventory.PathTo(itemId: weapon.Id).ShouldNotBeNull();
        var left = new RunInventory.Equipment(survivorId: Phase06MeleeTests.Owner, slot: "left-hand");
        session.TryEditInventory(edit: (inventory, _, _) => RunItemTransfer.Move(inventory: inventory,
                expected: path, destination: left)).ShouldBeFalse();
        RunInventory.Item ammo = Phase06MeleeTests.Equip(session: session, id: "IT-02", slot: "left-hand");
        Fire(session: session).Status.ShouldBe(expected: RunCombat.Status.BrokenWeapon);
        session.Inventory.Find(itemId: ammo.Id).ShouldBeSameAs(expected: ammo);
        session.Inventory.Find(itemId: bow.Id).ShouldBeSameAs(expected: bow);
    }

    private static void CheckTerrainTie()
    {
        var terrain = new RunWorld.Blocker(id: "terrain", minimum: new RunWorld.Point(x: -5350, y: 0),
                maximum: new RunWorld.Point(x: -5350, y: 2000));
        FoundationSession session = Phase06MeleeTests.Start(world: World(defenses: [Wall(x: -5600)],
                blockers: [terrain]));
        StartEnemyBow(session: session, arrows: 1);
        EnemyFire(session: session, sequence: 1).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 300);
        Combat(session: session).Impacts.Single().Kind.ShouldBe(expected: RunProjectiles.Kind.Terrain);
        Defense(session: session).Health.ShouldBe(expected: 100);
        Phase06MeleeTests.Survivor(session: session).Health.ShouldBe(expected: 100);
    }

    private static void CheckActorTie()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        Phase06MeleeTests.Enemy(session: session, id: "zebra", x: -5000);
        Phase06MeleeTests.Enemy(session: session, id: "alpha", x: -5000);
        _ = ArmBow(session: session);
        Fire(session: session).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 200);
        Combat(session: session).Impacts.Single().TargetId.ShouldBe(expected: "alpha");
        Phase06MeleeTests.Survivor(session: session, id: "zebra").Health.ShouldBe(expected: 100);
    }

    private static RunCombat.Outcome Fire(FoundationSession session, long sequence = 1, RunCombat.Aim? aim = null)
    {
        return session.Attack(controller: Phase06MeleeTests.Control(), sequence: sequence,
                aim: aim ?? new RunCombat.Aim(x: 10000, y: 0));
    }

    private static (RunInventory.Item bow, RunInventory.Item ammo) ArmBow(FoundationSession session, int count = 1)
    {
        return (Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "left-hand"),
                Phase06MeleeTests.Equip(session: session, id: "IT-02", slot: "right-hand", quantity: count));
    }

    private static RunInventory.Item CarryReserve(FoundationSession session)
    {
        RunInventory.Item arrows = session.Inventory.Items.First(predicate: item => item.DefinitionId is "IT-02");
        RunInventory.Item shirt = session.Inventory.Equipped(survivorId: Phase06MeleeTests.Owner,
                slot: "torso-clothing").ShouldNotBeNull();
        RunInventory.ItemPath path = session.Inventory.PathTo(itemId: arrows.Id).ShouldNotBeNull();
        RunInventory.ContainerCell storage = RunItemTransfer.Container(definition:
                session.Definitions.Items[arrows.DefinitionId], containerId: shirt.Id, x: 0, y: 0, rotated: true);
        session.TryEditInventory(edit: (inventory, _, _) => RunItemTransfer.Move(inventory: inventory,
                expected: path, destination: storage)).ShouldBeTrue();
        return session.Inventory.Find(itemId: arrows.Id).ShouldNotBeNull();
    }

    private static void CheckReadback(FoundationSession session, RunCombat.Aim aim, RunInventory.Item bow, int ready)
    {
        RunCombat.BowView view = session.ReadBow(controller: Phase06MeleeTests.Control(), aim: aim).ShouldNotBeNull();
        (view.BowId, view.ReadyArrows, view.Usable).ShouldBe(expected: (bow.Id, ready, true));
        view.Aim.ShouldBeSameAs(expected: aim);
    }

    private static void StartEnemyBow(FoundationSession session, int arrows)
    {
        Phase06MeleeTests.Enemy(session: session, id: "archer", x: -2000);
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "right-hand", owner: "archer");
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-02", slot: "left-hand", owner: "archer",
                quantity: arrows);
    }

    private static RunCombat.Outcome EnemyFire(FoundationSession session, long sequence)
    {
        return session.AttackUncontrolled(survivorId: "archer", sequence: sequence,
                aim: new RunCombat.Aim(x: -10000, y: 0));
    }

    private static RunCombat.State Combat(FoundationSession session)
    {
        return session.UnderInventoryAuthority(action: current => current?.Combat).ShouldNotBeNull();
    }

    private static RunWorld.Defense Defense(FoundationSession session)
    {
        return session.UnderInventoryAuthority(action: current => current?.World.Defenses.Single())
                .ShouldNotBeNull();
    }

    private static RunWorld World(RunWorld.Defense[]? defenses = null, RunWorld.Blocker[]? blockers = null)
    {
        return new RunWorld(origin: default, groundContactSubunits: 0, mainBuilding: default,
                ranges: RunWorld.DefaultRanges, defenses: defenses ?? [], blockers: blockers ?? [],
                hostiles: [], anchors: []);
    }

    private static RunWorld.Defense Wall(long x, decimal health = 100)
    {
        return new RunWorld.Defense(id: "wall", contact: new RunWorld.Point(x: x, y: 0),
                completed: true, living: true, isWall: true, health: health);
    }
}
