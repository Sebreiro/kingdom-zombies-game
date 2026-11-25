using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies the explicitly selected stationary combat fixture and its production transactions.</summary>
[Trait("Tier", "1")]
public sealed class Phase06FixtureTests
{
    /// <summary>Adjacent equal-looking hostiles retain distinct identities and stable actual first contact.</summary>
    [Fact]
    public void Phase06_E129_FixtureHasDistinctStationaryTargets()
    {
        FoundationSession session = Start(mode: "field");
        RunPopulation.Survivor a = Phase06MeleeTests.Survivor(session: session, id: RunCombatFixture.First);
        RunPopulation.Survivor b = Phase06MeleeTests.Survivor(session: session, id: RunCombatFixture.Second);
        a.PositionSubunitsExact.ShouldBe(expected: b.PositionSubunitsExact);
        StringComparer.Ordinal.Equals(x: a.Id, y: b.Id).ShouldBeFalse();
        session.Step(elapsedMilliseconds: 200);
        Phase06MeleeTests.Survivor(session: session, id: a.Id).PositionSubunitsExact
                .ShouldBe(expected: a.PositionSubunitsExact);
        RunCombat.Outcome strike = session.Attack(controller: Phase06MeleeTests.Control(), sequence: 1);
        strike.Status.ShouldBe(expected: RunCombat.Status.Accepted);
        strike.Hits.Single().TargetId.ShouldBe(expected: a.Id);
        Phase06MeleeTests.Survivor(session: session, id: b.Id).ShouldBeSameAs(expected: b);
        session.Inventory.Equipped(survivorId: RunCombatFixture.Owner, slot: "left-hand")?.Condition
                .ShouldBe(expected: 0);
    }

    /// <summary>Concurrent replay of one fixture trigger resolves once in the normal attack fence.</summary>
    [Fact]
    public async Task Phase06_E130_FixtureDuplicateUsesTheActualAttackFence()
    {
        FoundationSession session = Start(mode: "field");
        long revision = session.Snapshot.Revision;
        Task<(string status, bool replayed, long revision)> first = Task.Run(function: () =>
                Trigger(session: session, id: RunCombatFixture.First, sequence: 1));
        Task<(string status, bool replayed, long revision)> second = Task.Run(function: () =>
                Trigger(session: session, id: RunCombatFixture.First, sequence: 1));
        (string status, bool replayed, long revision)[] results = await Task.WhenAll(first, second);
        results.All(predicate: reply => reply.status is "Accepted").ShouldBeTrue();
        results.Count(predicate: reply => reply.replayed).ShouldBe(expected: 1);
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
        Phase06MeleeTests.Survivor(session: session).Health.ShouldBeLessThan(expected: 100);
    }

    /// <summary>Absent and unknown targets cannot run a success path or add objects to an ordinary start.</summary>
    [Fact]
    public void Phase06_E131_EmptyOrUnselectedFixtureCannotRunATrigger()
    {
        FoundationSession ordinary = Phase06MeleeTests.Start();
        FoundationSnapshot before = ordinary.Snapshot;
        Trigger(session: ordinary, id: RunCombatFixture.First, sequence: 1).status.ShouldBe(expected: "AccessLost");
        ordinary.Snapshot.ShouldBeSameAs(expected: before);
        Should.Throw<InvalidDataException>(actual: () => Start(mode: ""));
        FoundationSession selected = Start(mode: "field");
        FoundationSnapshot current = selected.Snapshot;
        Trigger(session: selected, id: "", sequence: 1).status.ShouldBe(expected: "InvalidAction");
        selected.Snapshot.ShouldBeSameAs(expected: current);
    }

    /// <summary>Independent resets preserve actual corpses, remove only empty loot and restore the seed kit.</summary>
    [Fact]
    public void Phase06_E132_FreshFixtureLifetimesAndCleanupAreIndependent()
    {
        FoundationSession session = Start(mode: "life");
        Trigger(session: session, id: RunCombatFixture.First, sequence: 1).status.ShouldBe(expected: "Accepted");
        session.Population.FindLease(bindingId: "local").ShouldBeNull();
        session.Step(elapsedMilliseconds: 179999);
        Phase06DeathTests.Loot(session: session).Targets.ContainsKey(key: RunCombatFixture.Empty).ShouldBeTrue();
        session.Step(elapsedMilliseconds: 1);
        RunDeathAndLoot.State loot = Phase06DeathTests.Loot(session: session);
        loot.Targets.ContainsKey(key: RunCombatFixture.Empty).ShouldBeFalse();
        loot.Targets.ContainsKey(key: RunCombatFixture.Remains).ShouldBeTrue();
        loot.Deaths.Count.ShouldBe(expected: 1);
        loot.Targets.ContainsKey(key: loot.Deaths.Values.Single().CorpseId).ShouldBeTrue();
        FoundationSession reset = Start(mode: "life", id: "reset");
        StringComparer.Ordinal.Equals(x: reset.Snapshot.SessionId, y: session.Snapshot.SessionId).ShouldBeFalse();
        reset.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 0);
        Phase06DeathTests.Loot(session: reset).Deaths.ShouldBeEmpty();
        Trigger(session: reset, id: RunCombatFixture.First, sequence: 1).replayed.ShouldBeFalse();
    }

    /// <summary>Real arrows spend the proper hand before time moves them through live defense geometry.</summary>
    [Fact]
    public void Phase06_E133_BowTriggerAndWorldStepHaveDistinctOrderedEffects()
    {
        FoundationSession session = Start(mode: "bow");
        RunInventory.Item ammo = session.Inventory.Equipped(survivorId: RunCombatFixture.Bowman,
                slot: "right-hand").ShouldNotBeNull();
        session.CanReach(bindingId: "local", kind: RunWorld.Access.Stash, targetId: RunWorld.StashId).ShouldBeTrue();
        Trigger(session: session, id: RunCombatFixture.Bowman, sequence: 1).status.ShouldBe(expected: "Accepted");
        session.Inventory.Find(itemId: ammo.Id)?.Quantity.ShouldBe(expected: 2);
        JsonElement world = session.ReadRunPublic().ShouldNotBeNull().world;
        world.GetProperty(propertyName: "Arrows").GetArrayLength().ShouldBe(expected: 1);
        Wall(session: session).ShouldBeTrue();
        session.Step(elapsedMilliseconds: 200);
        Wall(session: session).ShouldBeFalse();
        session.ReadRunPublic().ShouldNotBeNull().world.GetProperty(propertyName: "Arrows")
                .GetArrayLength().ShouldBe(expected: 0);
    }

    /// <summary>Ordinary starts retain the exact eight survivors and authored kit beside the isolated seed.</summary>
    [Fact]
    public void Fixture_NormalStartup_KeepsConfiguredKit()
    {
        FoundationSession ordinary = Phase06MeleeTests.Start();
        FoundationSession seeded = Start(mode: "field");
        FoundationSession next = Phase06MeleeTests.Start();
        ordinary.Population.Survivors.Length.ShouldBe(expected: 8);
        ordinary.Inventory.Items.ShouldBe(expected: next.Inventory.Items);
        ordinary.Inventory.StashPageCount.ShouldBe(expected: next.Inventory.StashPageCount);
        seeded.Population.Survivors.Count(predicate: survivor => survivor.Hostile).ShouldBe(expected: 3);
        seeded.Inventory.Items.Length.ShouldBeGreaterThan(expected: ordinary.Inventory.Items.Length);
        ordinary.FixtureMode.ShouldBeNull();
        next.FixtureMode.ShouldBeNull();
    }

    /// <summary>Two real lethal contacts preserve the armed body and independently expire the empty body.</summary>
    [Fact]
    public void Fixture_TwoLethalContacts_KeepSeparateCorpseOwnership()
    {
        FoundationSession session = Start(mode: "life");
        session.TryUpdateVitals(survivorId: RunCombatFixture.First, health: 1, stamina: 100).ShouldBeTrue();
        session.TryUpdateVitals(survivorId: RunCombatFixture.Second, health: 1, stamina: 100).ShouldBeTrue();
        RunInventory.Item bat = session.Inventory.Equipped(survivorId: RunCombatFixture.First,
                slot: "left-hand").ShouldNotBeNull();
        RunCombat.Outcome first = session.Attack(controller: Phase06MeleeTests.Control(), sequence: 1);
        first.Hits.Single().TargetId.ShouldBe(expected: RunCombatFixture.First);
        session.Step(elapsedMilliseconds: 1000);
        RunCombat.Outcome second = session.Attack(controller: Phase06MeleeTests.Control(), sequence: 2);
        second.Hits.Single().TargetId.ShouldBe(expected: RunCombatFixture.Second);
        RunDeathAndLoot.State loot = Phase06DeathTests.Loot(session: session);
        string full = loot.Deaths.Values.Single(predicate: row =>
                StringComparer.Ordinal.Equals(x: row.SurvivorId, y: RunCombatFixture.First)).CorpseId;
        string empty = loot.Deaths.Values.Single(predicate: row =>
                StringComparer.Ordinal.Equals(x: row.SurvivorId, y: RunCombatFixture.Second)).CorpseId;
        StringComparer.Ordinal.Equals(x: full, y: empty).ShouldBeFalse();
        session.Inventory.Find(itemId: bat.Id)?.Location.ShouldBeOfType<RunInventory.CorpseEquipment>()
                .CorpseId.ShouldBe(expected: full);
        session.Step(elapsedMilliseconds: 180000);
        loot = Phase06DeathTests.Loot(session: session);
        loot.Targets.ContainsKey(key: full).ShouldBeTrue();
        loot.Targets.ContainsKey(key: empty).ShouldBeFalse();
    }

    /// <summary>Near and far fixtures assign genuine candidates with their stored state intact.</summary>
    [Theory]
    [InlineData("life", -8000)]
    [InlineData("far", 30000)]
    public void Fixture_SuccessorRange_PreservesSeededSurvivor(string mode, int position)
    {
        FoundationSession session = Start(mode: mode);
        Trigger(session: session, id: RunCombatFixture.First, sequence: 1).status.ShouldBe(expected: "Accepted");
        session.Step(elapsedMilliseconds: 1000);
        RunInventory before = session.Inventory;
        var request = new RunSuccession.Request(sessionId: session.Snapshot.SessionId, bindingId: "local",
                deathGeneration: 1, sequence: 2);
        RunSuccession.Outcome result = session.RequestRespawns(requests: [request]).Single();
        result.Status.ShouldBe(expected: RunSuccession.Status.Assigned);
        RunPopulation.Lease lease = session.Population.FindLease(bindingId: "local").ShouldNotBeNull();
        session.Population.ForLease(lease: lease).PositionSubunits.ShouldBe(expected: position);
        session.Inventory.ShouldBeSameAs(expected: before);
    }

    private static FoundationSession Start(string mode, string id = "combat")
    {
        return RunCombatFixture.Create(mode: mode, sessionId: id, playerId: "host", actorId: "actor",
                definitions: RunDefinitions.Default);
    }

    private static (string status, bool replayed, long revision) Trigger(FoundationSession session, string id,
            long sequence)
    {
        return session.DispatchRunAction(identity: (session.Snapshot.SessionId, "local", "host"),
                key: (RunCombatFixture.Owner, 1, sequence), kind: "fixture-attack",
                request: JsonSerializer.SerializeToElement(value: new { Target = id }));
    }

    private static bool Wall(FoundationSession session)
    {
        return session.ReadRunPublic().ShouldNotBeNull().world.GetProperty(propertyName: "Defenses").EnumerateArray()
                .Single(predicate: row => row.GetProperty(propertyName: "Id").GetString() is "fixture.living-wall")
                .GetProperty(propertyName: "Living").GetBoolean();
    }
}
