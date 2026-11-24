using System.Collections.Immutable;
using System.Linq;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the host's explicit post-death request and control handover.</summary>
[Trait("Tier", "1")]
public sealed class Phase06SuccessionTests
{
    /// <summary>A stable random choice skips an occupied survivor and keeps all real possessions.</summary>
    [Fact]
    public void Phase06_E111_RespawnChoosesOneRealUncontestedSurvivor()
    {
        FoundationSession session = Start(count: 5);
        _ = Phase06DeathTests.Join(session: session);
        KillHost(session: session);
        session.Step(elapsedMilliseconds: 1000);
        ImmutableArray<RunPopulation.Survivor> candidates = session.Population.Candidates();
        candidates.Select(selector: survivor => survivor.Id).ShouldBe(expected:
                ["survivor-03", "survivor-04", "survivor-05"]);
        RunSuccession.State old = State(session: session);
        var random = new ReplayRandom(seed: old.Seed, runtimeContract: ReplayRandom.PinnedRuntimeContract,
                drawPosition: old.DrawPosition);
        long draw = random.Draw();
        RunPopulation.Survivor expected = candidates[(int)(draw % candidates.Length)];
        RunInventory belongings = session.Inventory;
        RunSuccession.Outcome result = Send(session: session);
        result.Status.ShouldBe(expected: RunSuccession.Status.Assigned);
        result.View.ShouldNotBeNull().SurvivorId.ShouldBe(expected: expected.Id);
        RunPopulation.Lease assigned = session.Population.FindLease(bindingId: "local").ShouldNotBeNull();
        session.Population.ForLease(lease: assigned).ShouldBeSameAs(expected: expected);
        State(session: session).DrawPosition.ShouldBe(expected: 1);
        session.Inventory.ShouldBeSameAs(expected: belongings);
        session.Inventory.CarriedBy(survivorId: expected.Id).Length.ShouldBe(expected: 3);
        session.Population.Leases.Select(selector: lease => lease.SurvivorId).Distinct(
                comparer: System.StringComparer.Ordinal).Count().ShouldBe(expected: 2);
    }

    /// <summary>Time opens the button at one second without automatically assigning a survivor.</summary>
    [Fact]
    public void Phase06_E112_RespawnOpensAtExactlyOneSecondAndRequiresInput()
    {
        FoundationSession session = Start(count: 2);
        KillHost(session: session);
        session.Step(elapsedMilliseconds: 999);
        View(session: session).Status.ShouldBe(expected: RunSuccession.Status.Early);
        View(session: session).OpensAt.ShouldBe(expected: 1000);
        FoundationSnapshot early = session.Snapshot;
        Send(session: session).Status.ShouldBe(expected: RunSuccession.Status.Early);
        session.Snapshot.ShouldBeSameAs(expected: early);
        State(session: session).Waiting.ShouldBeEmpty();
        session.Step(elapsedMilliseconds: 1);
        View(session: session).Status.ShouldBe(expected: RunSuccession.Status.Ready);
        session.Population.FindLease(bindingId: "local").ShouldBeNull();
        Send(session: session).Status.ShouldBe(expected: RunSuccession.Status.Early);
        Send(session: session, sequence: 2).Status.ShouldBe(expected: RunSuccession.Status.Assigned);
        View(session: session).SurvivorId.ShouldBe(expected: "survivor-02");
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 1000);
        CheckRetiredControls(session: session);
    }

    /// <summary>Equal-time requests share a host batch and the lower player slot wins one candidate.</summary>
    [Fact]
    public void Phase06_E113_SameTimeBatchUsesPlayerSlotTieBreak()
    {
        FoundationSession session = Start(count: 3);
        _ = Phase06DeathTests.Join(session: session);
        KillTwo(session: session);
        session.Step(elapsedMilliseconds: 1000);
        long revision = session.Snapshot.Revision;
        RunSuccession.Request guest = Request(session: session, binding: "guest");
        RunSuccession.Request host = Request(session: session);
        ImmutableArray<RunSuccession.Outcome> results = session.RequestRespawns(requests: [guest, host]);
        results.Select(selector: result => result.Status).ShouldBe(expected:
                [RunSuccession.Status.Waiting, RunSuccession.Status.Assigned]);
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
        View(session: session).SurvivorId.ShouldBe(expected: "survivor-03");
        View(session: session, binding: "guest").Status.ShouldBe(expected: RunSuccession.Status.Waiting);
        State(session: session).Waiting.Single().At.ShouldBe(expected: 1000);
        State(session: session).Waiting.Single().Slot.ShouldBe(expected: 2);
        View(session: session, binding: "guest").Teammates.Single().SurvivorId.ShouldBe(expected: "survivor-03");
    }

    /// <summary>A waiting spectator receives a released recruit, while complete defeat has no assignment.</summary>
    [Fact]
    public void Phase06_E114_ZeroCandidatesAndNoLivingOrdinarySurvivorsAreDistinct()
    {
        FoundationSession waiting = Start(count: 2);
        _ = Phase06DeathTests.Join(session: waiting);
        KillHost(session: waiting);
        waiting.Step(elapsedMilliseconds: 1000);
        Send(session: waiting).Status.ShouldBe(expected: RunSuccession.Status.Waiting);
        waiting.Population.Candidates().ShouldBeEmpty();
        View(session: waiting).Teammates.Single().BindingId.ShouldBe(expected: "guest");
        waiting.ItemActions.Read(controller: Phase06MeleeTests.Control()).ShouldBeNull();
        waiting.Attack(controller: Phase06MeleeTests.Control(), sequence: 10).Status
                .ShouldBe(expected: RunCombat.Status.AccessLost);
        waiting.RemoveParticipant(bindingId: "guest").ShouldBeTrue();
        View(session: waiting).SurvivorId.ShouldBe(expected: "survivor-02");
        State(session: waiting).Waiting.ShouldBeEmpty();
        waiting.Population.Survivors.Length.ShouldBe(expected: 3);

        FoundationSession defeated = Start(count: 1);
        KillHost(session: defeated);
        defeated.Step(elapsedMilliseconds: 1000);
        View(session: defeated).Status.ShouldBe(expected: RunSuccession.Status.Defeated);
        View(session: defeated).Teammates.ShouldBeEmpty();
        Send(session: defeated).Status.ShouldBe(expected: RunSuccession.Status.Defeated);
        State(session: defeated).Waiting.ShouldBeEmpty();
        defeated.Population.Survivors.Single(predicate: survivor => survivor.Hostile).Health.ShouldBe(expected: 100);
    }

    /// <summary>Repeated requests and forged generations cannot create a second queue entry or successor.</summary>
    [Fact]
    public void Phase06_E115_RespawnReplayKeepsOneQueueEntryAndOneLease()
    {
        FoundationSession session = Start(count: 2);
        _ = Phase06DeathTests.Join(session: session);
        KillHost(session: session);
        session.Step(elapsedMilliseconds: 1000);
        var stale = new RunSuccession.Request(sessionId: "combat", bindingId: "local", deathGeneration: 99,
                sequence: 1);
        session.RequestRespawns(requests: [stale]).Single().Status.ShouldBe(expected: RunSuccession.Status.StaleDeath);
        Send(session: session).Status.ShouldBe(expected: RunSuccession.Status.Waiting);
        FoundationSnapshot queued = session.Snapshot;
        Send(session: session).Replayed.ShouldBeTrue();
        Send(session: session, sequence: 2).Status.ShouldBe(expected: RunSuccession.Status.Waiting);
        session.Snapshot.ShouldBeSameAs(expected: queued);
        State(session: session).Waiting.Length.ShouldBe(expected: 1);
        session.RemoveParticipant(bindingId: "guest").ShouldBeTrue();
        FoundationSnapshot assigned = session.Snapshot;
        RunSuccession.Outcome duplicate = Send(session: session, sequence: 2);
        duplicate.Replayed.ShouldBeTrue();
        duplicate.View.ShouldNotBeNull().Status.ShouldBe(expected: RunSuccession.Status.Active);
        Send(session: session, sequence: 3).Status.ShouldBe(expected: RunSuccession.Status.Active);
        session.Snapshot.ShouldBeSameAs(expected: assigned);
        session.Population.Leases.Single().SurvivorId.ShouldBe(expected: "survivor-02");
    }

    /// <summary>An older waiter has priority over a later lower slot when an existing controller departs.</summary>
    [Fact]
    public void Phase06_E116_EarlierWaiterReceivesTheReleasedPersistentState()
    {
        FoundationSession session = Start(count: 4);
        _ = Phase06DeathTests.Join(session: session);
        RunItemActions.Controller leaving = Phase06DeathTests.Join(session: session, binding: "leave");
        _ = Phase06DeathTests.Join(session: session, binding: "final");
        KillTwo(session: session);
        session.Step(elapsedMilliseconds: 1000);
        Send(session: session, binding: "guest").Status.ShouldBe(expected: RunSuccession.Status.Waiting);
        session.Step(elapsedMilliseconds: 1);
        Send(session: session).Status.ShouldBe(expected: RunSuccession.Status.Waiting);
        var move = new RunLocomotion.Input(right: true);
        session.TryAcceptMotion(controller: leaving, sequence: 1, input: move).accepted.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 300);
        session.TryUpdateVitals(survivorId: "survivor-03", health: 51.25m, stamina: 62.125m).ShouldBeTrue();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand", owner: "survivor-03");
        RunPopulation.Survivor original = Phase06MeleeTests.Survivor(session: session, id: "survivor-03");
        RunInventory items = session.Inventory;
        session.RemoveParticipant(bindingId: "leave").ShouldBeTrue();
        View(session: session, binding: "guest").SurvivorId.ShouldBe(expected: original.Id);
        View(session: session).Status.ShouldBe(expected: RunSuccession.Status.Waiting);
        Phase06MeleeTests.Survivor(session: session, id: original.Id).ShouldBeSameAs(expected: original);
        session.Inventory.ShouldBeSameAs(expected: items);
        session.Snapshot.Participants.Single(predicate: participant => participant.BindingId is "guest")
                .Actor.PositionSubunits.ShouldBe(expected: -7100);
        session.RemoveParticipant(bindingId: "final").ShouldBeTrue();
        View(session: session).SurvivorId.ShouldBe(expected: "survivor-04");
        State(session: session).Waiting.ShouldBeEmpty();
    }

    /// <summary>A fractional projectile death uses its real time and can never round the button open early.</summary>
    [Fact]
    public void Phase06_E117_FractionalDeathTimeAndControlEpochRemainExact()
    {
        FoundationSession session = Start(count: 2);
        Phase06MeleeTests.Enemy(session: session, id: "archer", x: -5499.5m);
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "left-hand", owner: "archer");
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-02", slot: "right-hand", owner: "archer");
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 1, stamina: 100).ShouldBeTrue();
        session.AttackUncontrolled(survivorId: "archer", sequence: 1,
                aim: new RunCombat.Aim(x: -10000, y: 0)).Status.ShouldBe(expected: RunCombat.Status.Accepted);
        session.Step(elapsedMilliseconds: 91);
        View(session: session).OpensAt.ShouldBe(expected: 1090.02m);
        View(session: session).DeathGeneration.ShouldBe(expected: 1);
        session.Step(elapsedMilliseconds: 999);
        Send(session: session).Status.ShouldBe(expected: RunSuccession.Status.Early);
        session.Step(elapsedMilliseconds: 1);
        Send(session: session, sequence: 2).Status.ShouldBe(expected: RunSuccession.Status.Assigned);
        View(session: session).Generation.ShouldBe(expected: 2);
    }

    private static FoundationSession Start(int count)
    {
        RunDefinitions baseline = RunDefinitions.Default;
        var start = new RunDefinitions.Start(survivorCount: count, playerLimit: 4, role: "woodcutter",
                outfit: baseline.StartingOutfit, stash: []);
        var vitals = new RunDefinitions.SurvivorSettings(maximumHealth: 100, maximumStamina: 100,
                refugeUnits: 12, dangerUnits: 12);
        var inventory = new RunDefinitions.InventorySettings(stashGrid: baseline.StashGrid,
                maximumContainerLevels: 3, quickSlotCount: 4, localUnits: 3, stashUnits: 4);
        var definitions = new RunDefinitions(start: start, survivors: vitals, inventory: inventory,
                items: baseline.Items, food: baseline.Food, combat: baseline.Combat);
        return new FoundationSession(sessionId: "combat", playerId: "host", actorId: "actor",
                world: RunWorld.Foundation, definitions: definitions);
    }

    private static void KillHost(FoundationSession session)
    {
        Phase06MeleeTests.Enemy(session: session, id: "enemy", x: -7500);
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 1, stamina: 100).ShouldBeTrue();
        session.AttackUncontrolled(survivorId: "enemy", sequence: 1).Hits.Single().Lethal.ShouldBeTrue();
    }

    private static void KillTwo(FoundationSession session)
    {
        Phase06MeleeTests.Enemy(session: session, id: "enemy", x: -7500);
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand", owner: "enemy");
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-04", slot: "right-hand", owner: "enemy");
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 1, stamina: 100).ShouldBeTrue();
        session.TryUpdateVitals(survivorId: "survivor-02", health: 1, stamina: 100).ShouldBeTrue();
        session.AttackUncontrolled(survivorId: "enemy", sequence: 1).Hits.Count(predicate: hit => hit.Lethal)
                .ShouldBe(expected: 2);
    }

    private static void CheckRetiredControls(FoundationSession session)
    {
        var move = new RunLocomotion.Input(right: true);
        session.TryAcceptMotion(controller: Phase06MeleeTests.Control(), sequence: 99, input: move).accepted
                .ShouldBeFalse();
        var legacy = new FoundationMovementCommand(sessionId: "combat", playerId: "host", actorId: "actor",
                direction: 1, sequence: 100);
        session.TryAccept(command: legacy, trustedBinding: "local", trustedPlayer: "host", trustedActor: "actor")
                .ShouldBeFalse();
        RunPopulation.Lease lease = session.Population.FindLease(bindingId: "local").ShouldNotBeNull();
        var current = new RunItemActions.Controller(sessionId: "combat", bindingId: "local",
                generation: lease.Generation);
        session.TryAcceptMotion(controller: current, sequence: 1, input: move).accepted.ShouldBeTrue();
    }

    private static RunSuccession.View View(FoundationSession session, string binding = "local")
    {
        return session.ReadSuccession(bindingId: binding).ShouldNotBeNull();
    }

    private static RunSuccession.Request Request(FoundationSession session, string binding = "local",
            long sequence = 1)
    {
        return new RunSuccession.Request(sessionId: "combat", bindingId: binding, deathGeneration:
                View(session: session, binding: binding).DeathGeneration.GetValueOrDefault(), sequence: sequence);
    }

    private static RunSuccession.Outcome Send(FoundationSession session, string binding = "local", long sequence = 1)
    {
        return session.RequestRespawns(requests: [Request(session: session, binding: binding, sequence: sequence)])
                .Single();
    }

    private static RunSuccession.State State(FoundationSession session)
    {
        return session.UnderInventoryAuthority(action: current => current?.Succession).ShouldNotBeNull();
    }
}
