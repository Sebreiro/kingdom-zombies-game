using System;
using System.Text;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves configured movement and stamina on the production authority.</summary>
[Trait("Tier", "1")]
public sealed class Phase06MovementTests
{
    /// <summary>Opposing holds cancel, and sprint and jump use the configured rates.</summary>
    [Fact]
    public void Phase06_E014_OpposingHoldsSprintAndJumpUseExactBaselines()
    {
        FoundationSession session = Start();
        Motion(session: session, sequence: 1, input: new RunLocomotion.Input(left: true, right: true, sprint: true));
        session.Step(elapsedMilliseconds: 1000);
        session.Actor.PositionSubunits.ShouldBe(expected: -8000);
        Survivor(session: session).Stamina.ShouldBe(expected: 100);
        Motion(session: session, sequence: 2, input: new RunLocomotion.Input(right: true, sprint: true));
        session.Step(elapsedMilliseconds: 1000);
        session.Actor.PositionSubunits.ShouldBe(expected: -3000);
        Survivor(session: session).Stamina.ShouldBe(expected: 90);
        Motion(session: session, sequence: 3, input: new RunLocomotion.Input(right: true, jump: true))
                .ShouldBe(expected: (true, RunLocomotion.Block.None));
        Survivor(session: session).Stamina.ShouldBe(expected: 78);
        session.Step(elapsedMilliseconds: 400);
        Survivor(session: session).HeightSubunits.ShouldBe(expected: 1500);
        session.Actor.PositionSubunits.ShouldBe(expected: -2100);
        session.Step(elapsedMilliseconds: 400);
        Survivor(session: session).HeightSubunits.ShouldBe(expected: 0);
        Survivor(session: session).JumpStartedAt.ShouldBeNull();
        session.Actor.PositionSubunits.ShouldBe(expected: -1200);
        session.Actor.ActorId.ShouldBe(expected: "actor");
    }

    /// <summary>A focus-clear hold wins over older input and a disconnected jump lands without old motion.</summary>
    [Fact]
    public async Task Phase06_E015_FocusClearRacesAndDepartureCannotRepeatTheJump()
    {
        FoundationSession session = Start();
        session.AddParticipant(bindingId: "guest", playerId: "guest-player", actorId: "other", label: "Guest")
                .ShouldBeTrue();
        session.MarkReady(bindingId: "guest").ShouldBeTrue();
        var jump = new RunLocomotion.Input(right: true, jump: true);
        var clear = new RunLocomotion.Input();
        Task<(bool accepted, RunLocomotion.Block reason)> first = Task.Run(function: () =>
                Motion(session: session, sequence: 1, input: jump, binding: "guest"));
        Task<(bool accepted, RunLocomotion.Block reason)> duplicate = Task.Run(function: () =>
                Motion(session: session, sequence: 1, input: jump, binding: "guest"));
        (bool accepted, RunLocomotion.Block reason)[] results = await Task.WhenAll(tasks: [first, duplicate]);
        (results[0].accepted ^ results[1].accepted).ShouldBeTrue();
        Motion(session: session, sequence: 2, input: clear, binding: "guest").accepted.ShouldBeTrue();
        Motion(session: session, sequence: 1, input: jump, binding: "guest").accepted.ShouldBeFalse();
        session.Step(elapsedMilliseconds: 1);
        RunPopulation.Survivor guest = Survivor(session: session, index: 1);
        guest.PositionSubunits.ShouldBe(expected: -8000);
        guest.Stamina.ShouldBe(expected: 88);
        RunItemActions.Controller former = Controller(session: session, binding: "guest");
        session.RemoveParticipant(bindingId: "guest").ShouldBeTrue();
        session.TryAcceptMotion(controller: former, sequence: 3, input: jump).accepted.ShouldBeFalse();
        session.Step(elapsedMilliseconds: 799);
        Survivor(session: session, index: 1).JumpStartedAt.ShouldBeNull();
        Survivor(session: session, index: 1).PositionSubunits.ShouldBe(expected: -8000);
    }

    /// <summary>Exact stamina permits one jump. Failed and duplicate presses never queue a later jump.</summary>
    [Fact]
    public void Phase06_E016_ExactStaminaAndRecoveryDelayNeverQueueAPress()
    {
        FoundationSession session = Start();
        Motion(session: session, sequence: 1, input: new RunLocomotion.Input(right: true, sprint: true));
        session.Step(elapsedMilliseconds: 8800);
        Survivor(session: session).Stamina.ShouldBe(expected: 12);
        var jump = new RunLocomotion.Input(jump: true);
        Motion(session: session, sequence: 2, input: jump).ShouldBe(expected: (true, RunLocomotion.Block.None));
        Motion(session: session, sequence: 2, input: jump).accepted.ShouldBeFalse();
        Motion(session: session, sequence: 3, input: jump).reason.ShouldBe(expected: RunLocomotion.Block.Airborne);
        session.Step(elapsedMilliseconds: 500);
        Survivor(session: session).Stamina.ShouldBe(expected: 0);
        session.Step(elapsedMilliseconds: 1);
        Survivor(session: session).Stamina.ShouldBe(expected: 0.014m);
        session.Step(elapsedMilliseconds: 299);
        Motion(session: session, sequence: 4, input: jump).reason
                .ShouldBe(expected: RunLocomotion.Block.InsufficientStamina);
        session.Step(elapsedMilliseconds: 1000);
        Survivor(session: session).JumpStartedAt.ShouldBeNull();
        Survivor(session: session).Stamina.ShouldBe(expected: 18.2m);
        Motion(session: session, sequence: 4, input: jump).accepted.ShouldBeFalse();
        Motion(session: session, sequence: 5, input: jump).reason.ShouldBe(expected: RunLocomotion.Block.None);
        CheckSprintExhaustion();
    }

    /// <summary>The ordinary session uses an authored fractional walk rate.</summary>
    [Fact]
    public void Phase06_E017_AuthoredFractionalMotionKeepsPartitionPrecision()
    {
        FoundationSession session = ConfiguredWalk();
        var command = new FoundationMovementCommand(sessionId: "movement", playerId: "host",
                actorId: "actor", direction: 1);
        session.Advance(command: command, elapsedMilliseconds: 1000);
        session.Actor.PositionSubunits.ShouldBe(expected: -3500);
        FoundationSession split = ConfiguredWalk();
        split.Advance(command: command, elapsedMilliseconds: 1);
        Survivor(session: split).PositionSubunitsExact.ShouldBe(expected: -7995.5m);
        split.Step(elapsedMilliseconds: 332);
        split.Step(elapsedMilliseconds: 667);
        Survivor(session: split).PositionSubunitsExact.ShouldBe(expected: Survivor(session: session)
                .PositionSubunitsExact);
        split.Actor.PositionSubunits.ShouldBe(expected: -3500);
    }

    private static FoundationSession Start()
    {
        return new FoundationSession(sessionId: "movement", playerId: "host", actorId: "actor");
    }

    private static void CheckSprintExhaustion()
    {
        FoundationSession session = Start();
        var sprint = new RunLocomotion.Input(right: true, sprint: true);
        Motion(session: session, sequence: 1, input: sprint).accepted.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 11000);
        Survivor(session: session).Stamina.ShouldBe(expected: 7);
        session.Actor.PositionSubunits.ShouldBe(expected: 45000);
        Survivor(session: session).SprintBlock.ShouldBe(expected: RunLocomotion.Block.InsufficientStamina);
        session.Step(elapsedMilliseconds: 1000);
        Survivor(session: session).Stamina.ShouldBe(expected: 21);
        session.Actor.PositionSubunits.ShouldBe(expected: 48000);
        Motion(session: session, sequence: 2, input: sprint).accepted.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 100);
        Survivor(session: session).Stamina.ShouldBe(expected: 20);
        session.Actor.PositionSubunits.ShouldBe(expected: 48500);
    }

    private static RunPopulation.Survivor Survivor(FoundationSession session, int index = 0)
    {
        return session.Population.Survivors[index];
    }

    private static (bool accepted, RunLocomotion.Block reason) Motion(FoundationSession session, long sequence,
            RunLocomotion.Input input, string binding = "local")
    {
        return session.TryAcceptMotion(controller: Controller(session: session, binding: binding),
                sequence: sequence, input: input);
    }

    private static RunItemActions.Controller Controller(FoundationSession session, string binding)
    {
        RunPopulation.Lease lease = session.Population.FindLease(bindingId: binding).ShouldNotBeNull();
        return new RunItemActions.Controller(sessionId: lease.SessionId, bindingId: binding,
                generation: lease.Generation);
    }

    private static FoundationSession ConfiguredWalk()
    {
        return FoundationSession.CreateFromResources(sessionId: "movement", playerId: "host", actorId: "actor",
                read: ReadWithWalk);
    }

    private static byte[] ReadWithWalk(string path, int limit)
    {
        byte[] bytes = Phase06LoadTests.Resource(path: path, limit: limit);
        return path is "content/config/survivors.json" ? Encoding.UTF8.GetBytes(s: Encoding.UTF8.GetString(
                bytes: bytes).Replace(oldValue: "\"walkSpeedUnitsPerSecond\": 3",
                        newValue: "\"walkSpeedUnitsPerSecond\": 4.5", comparisonType: StringComparison.Ordinal))
                : bytes;
    }
}
