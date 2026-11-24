using System;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the production host's world time, safety and local access decisions.</summary>
[Trait("Tier", "1")]
public sealed class Phase06WorldTests
{
    /// <summary>Camp edges, danger distance, and interaction ranges include their exact boundaries.</summary>
    [Fact]
    public void Phase06_E001_InclusiveBordersDangerAndReachability()
    {
        FoundationSession session = Session(world: World(defenses: [Defense(id: "wall", x: -8000)],
                blockers: [Barrier(id: "terrain", x: -9500)],
                hostiles: [new RunWorld.Hostile(id: "enemy", contact: Point(x: -20000))],
                anchors: [Anchor(id: RunWorld.StashId, kind: RunWorld.Access.Stash, x: -4000),
                    Anchor(id: "npc", kind: RunWorld.Access.Npc, x: -8000, y: 3000),
                    Anchor(id: "corpse", kind: RunWorld.Access.Corpse, x: -8000, y: 3001),
                    Anchor(id: "pile", kind: RunWorld.Access.Pile, x: -8000, y: 3000),
                    Anchor(id: "blocked", kind: RunWorld.Access.Corpse, x: -10000)]));
        session.Step(elapsedMilliseconds: 0);
        session.LocalWorld.ShouldBe(expected: (0L, true, false, true));
        CanReach(session: session, kind: RunWorld.Access.Npc, id: "npc").ShouldBeTrue();
        CanReach(session: session, kind: RunWorld.Access.Pile, id: "pile").ShouldBeTrue();
        CanReach(session: session, kind: RunWorld.Access.Corpse, id: "corpse").ShouldBeFalse();
        CanReach(session: session, kind: RunWorld.Access.Corpse, id: "blocked").ShouldBeFalse();
        CanReach(session: session, kind: RunWorld.Access.Corpse, id: "pile").ShouldBeFalse();

        Accept(session: session, direction: 1);
        session.Step(elapsedMilliseconds: 1);
        session.Actor.PositionSubunits.ShouldBe(expected: -7997);
        session.LocalWorld.ShouldBe(expected: (1L, true, true, true));
        CanReach(session: session, kind: RunWorld.Access.Npc, id: "npc").ShouldBeFalse();
    }

    /// <summary>Each side without a living completed wall falls back to the main-building refuge.</summary>
    [Fact]
    public void Phase06_E003_TowersAndUnfinishedWallsLeaveTheRefuge()
    {
        FoundationSession session = Session(world: World(mainX: 1,
                defenses: [Defense(id: "left-tower", x: -21000, wall: false),
                    Defense(id: "right-tower", x: 21000, wall: false),
                    Defense(id: "unfinished", x: 23000, completed: false),
                    Defense(id: "destroyed", x: -23000, living: false)]));

        Accept(session: session, direction: -1);
        session.Step(elapsedMilliseconds: 1333);
        session.Actor.PositionSubunits.ShouldBe(expected: -11999);
        session.LocalWorld.insideCamp.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 1);
        session.LocalWorld.insideCamp.ShouldBeFalse();

        Accept(session: session, direction: 1, sequence: 2);
        session.Step(elapsedMilliseconds: 8001);
        session.Actor.PositionSubunits.ShouldBe(expected: 12001);
        session.LocalWorld.insideCamp.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 1);
        session.LocalWorld.insideCamp.ShouldBeFalse();
    }

    /// <summary>Accepting, repeating, rejecting and installing roster changes never advance run time.</summary>
    [Fact]
    public void Phase06_E004_OnlyAnAuthoritativeStepAdvancesTime()
    {
        FoundationSession session = Session();
        FoundationSnapshot initial = session.Snapshot;
        FoundationMovementCommand command = Command(direction: 1, sequence: 2);
        TryAccept(session: session, command: command).ShouldBeTrue();
        TryAccept(session: session, command: command).ShouldBeFalse();
        TryAccept(session: session, command: Command(direction: -1, sequence: 1)).ShouldBeFalse();
        TryAccept(session: session, command: command with { SessionId = "stale", Sequence = 3 }).ShouldBeFalse();
        session.Snapshot.ShouldBeSameAs(expected: initial);
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 0);

        session.Step(elapsedMilliseconds: 500);
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 500);
        session.Actor.PositionSubunits.ShouldBe(expected: -6500);
        session.Step(elapsedMilliseconds: 0);
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 500);
        session.Actor.PositionSubunits.ShouldBe(expected: -6500);

        session.AddParticipant(bindingId: "guest", playerId: "player2", actorId: "actor2", label: "Guest")
                .ShouldBeTrue();
        session.Step(elapsedMilliseconds: 250);
        session.MarkReady(bindingId: "guest").ShouldBeTrue();
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 750);
        session.Snapshot.Participants[1].Actor.ElapsedMilliseconds.ShouldBe(expected: 0);
        session.Step(elapsedMilliseconds: 100);
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 850);
        session.Snapshot.Participants[1].Actor.ElapsedMilliseconds.ShouldBe(expected: 100);
        session.RemoveParticipant(bindingId: "guest").ShouldBeTrue();
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 850);
    }

    /// <summary>The integral clock preserves precise boundaries and rejects invalid steps atomically.</summary>
    [Fact]
    public void Phase06_E006_ElapsedIsExactAndInvalidStepsAreAtomic()
    {
        CheckExactBoundaries();
        CheckRejectedElapsed();
    }

    private static void CheckExactBoundaries()
    {
        FoundationSession whole = Session();
        FoundationSession split = Session();
        long previous = 0;
        foreach (long threshold in new long[] { 500, 1000, 2000, 5000, 180000 })
        {
            whole.Step(elapsedMilliseconds: threshold + 1 - previous);
            split.Step(elapsedMilliseconds: threshold - 1 - previous);
            split.LocalWorld.elapsedMilliseconds.ShouldBe(expected: threshold - 1);
            split.Step(elapsedMilliseconds: 1);
            split.LocalWorld.elapsedMilliseconds.ShouldBe(expected: threshold);
            split.Step(elapsedMilliseconds: 1);
            split.LocalWorld.elapsedMilliseconds.ShouldBe(expected: threshold + 1);
            split.LocalWorld.ShouldBe(expected: whole.LocalWorld);
            previous = threshold + 1;
        }
    }

    private static void CheckRejectedElapsed()
    {
        FoundationSession session = Session();
        session.Step(elapsedMilliseconds: long.MaxValue);
        FoundationSnapshot last = session.Snapshot;
        Should.Throw<ArgumentOutOfRangeException>(actual: () => session.Step(elapsedMilliseconds: -1));
        Should.Throw<OverflowException>(actual: () => session.Step(elapsedMilliseconds: 1));
        Should.Throw<OverflowException>(actual: () => session.Advance(command: Command(direction: -1, sequence: 1),
                elapsedMilliseconds: 1));
        session.Snapshot.ShouldBeSameAs(expected: last);
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: long.MaxValue);

        session.Step(elapsedMilliseconds: 0);
        session.Actor.Facing.ShouldBe(expected: 1, customMessage: "A rejected overflow must leave held intent intact");
        session.Actor.PositionSubunits.ShouldBe(expected: -8000);
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: long.MaxValue);
    }

    private static FoundationSession Session(RunWorld? world = null)
    {
        return new FoundationSession(sessionId: "session", playerId: "player", actorId: "actor",
                world: world ?? RunWorld.Foundation);
    }

    private static RunWorld World(long mainX = 0, RunWorld.Defense[]? defenses = null,
            RunWorld.Blocker[]? blockers = null, RunWorld.Hostile[]? hostiles = null, RunWorld.Anchor[]? anchors = null)
    {
        return new RunWorld(origin: default, groundContactSubunits: 0, mainBuilding: Point(x: mainX),
                ranges: RunWorld.DefaultRanges, defenses: defenses ?? [], blockers: blockers ?? [],
                hostiles: hostiles ?? [], anchors: anchors ?? []);
    }

    private static RunWorld.Point Point(long x, long y = 0)
    {
        return new RunWorld.Point(x: x, y: y);
    }

    private static RunWorld.Defense Defense(string id, long x, bool completed = true, bool living = true,
            bool wall = true)
    {
        return new RunWorld.Defense(id: id, contact: Point(x: x), completed: completed, living: living, isWall: wall);
    }

    private static RunWorld.Blocker Barrier(string id, long x)
    {
        return new RunWorld.Blocker(id: id, minimum: Point(x: x, y: -1000), maximum: Point(x: x, y: 1000));
    }

    private static RunWorld.Anchor Anchor(string id, RunWorld.Access kind, long x, long y = 0)
    {
        return new RunWorld.Anchor(id: id, contact: Point(x: x, y: y), kind: kind);
    }

    private static bool CanReach(FoundationSession session, RunWorld.Access kind, string id)
    {
        return session.CanReach(bindingId: "local", kind: kind, targetId: id);
    }

    private static void Accept(FoundationSession session, int direction, long sequence = 1)
    {
        TryAccept(session: session, command: Command(direction: direction, sequence: sequence)).ShouldBeTrue();
    }

    private static FoundationMovementCommand Command(int direction, long sequence)
    {
        return new FoundationMovementCommand(sessionId: "session", playerId: "player", actorId: "actor",
                direction: direction, sequence: sequence);
    }

    private static bool TryAccept(FoundationSession session, FoundationMovementCommand command)
    {
        return session.TryAccept(command: command, trustedBinding: "local", trustedPlayer: "player",
                trustedActor: "actor");
    }
}
