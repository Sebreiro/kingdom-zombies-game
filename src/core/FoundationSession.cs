using System;

namespace KingdomZombies.Core;

/// <summary>Owns one identified local actor and the sole movement mutation boundary.</summary>
public sealed class FoundationSession
{
    /// <summary>Creates a fresh identified actor at the fixture spawn.</summary>
    public FoundationSession(string sessionId, string playerId, string actorId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: sessionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: playerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: actorId);
        Actor = new FoundationActorState(sessionId: sessionId, playerId: playerId, actorId: actorId,
                positionSubunits: -8000, facing: 1, elapsedMilliseconds: 0);
    }

    /// <summary>Gets the current immutable actor publication.</summary>
    public FoundationActorState Actor { get; private set; }

    /// <summary>Fences this lifetime before its native scene is disconnected or freed.</summary>
    public void Close()
    {
        _active = false;
    }

    /// <summary>Consumes identified direction and explicit elapsed time.</summary>
    /// <remarks>Forty seconds spans the fixture; extra movement saturates while elapsed time remains exact.</remarks>
    public void Advance(FoundationMovementCommand command, long elapsedMilliseconds)
    {
        if (!_active)
        {
            throw new InvalidOperationException(message: "FOUNDATION_CLOSED");
        }

        ArgumentNullException.ThrowIfNull(argument: command);
        ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
        if (!StringComparer.Ordinal.Equals(x: command.SessionId, y: Actor.SessionId)
                || !StringComparer.Ordinal.Equals(x: command.PlayerId, y: Actor.PlayerId)
                || !StringComparer.Ordinal.Equals(x: command.ActorId, y: Actor.ActorId))
        {
            throw new ArgumentException(message: "FOUNDATION_OWNER: command ownership mismatch",
                    paramName: nameof(command));
        }

        if (command.Direction is < -1 or > 1)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(command), message: "FOUNDATION_DIRECTION");
        }

        long elapsed = checked(Actor.ElapsedMilliseconds + elapsedMilliseconds);
        long distance = Math.Min(val1: elapsedMilliseconds, val2: 40000) * 3 * command.Direction;
        long position = Math.Clamp(value: checked(Actor.PositionSubunits + distance), min: -59000, max: 59000);
        Actor = new FoundationActorState(sessionId: Actor.SessionId, playerId: Actor.PlayerId,
                actorId: Actor.ActorId, positionSubunits: position,
                facing: command.Direction == 0 ? Actor.Facing : command.Direction, elapsedMilliseconds: elapsed);
    }

    private bool _active = true;
}
