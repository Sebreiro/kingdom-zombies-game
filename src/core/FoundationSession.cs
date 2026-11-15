using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Owns identified participants and the sole intent and elapsed movement authority.</summary>
public sealed class FoundationSession
{
    /// <summary>Creates one ready local participant at the fixture spawn.</summary>
    public FoundationSession(string sessionId, string playerId, string actorId)
    {
        _snapshot = new FoundationSnapshot(sessionId: sessionId, revision: 0, participants:
                [new FoundationParticipantState(bindingId: "local", label: "Player 1", slot: 1, ready: true,
                        actor: new FoundationActorState(sessionId: sessionId, playerId: playerId, actorId: actorId,
                                positionSubunits: -8000, facing: 1, elapsedMilliseconds: 0))]);
    }

    /// <summary>Gets the inherited view of the host actor from the common publication.</summary>
    public FoundationActorState Actor => _snapshot.Participants[0].Actor;
    /// <summary>Gets one complete immutable publication.</summary>
    public FoundationSnapshot Snapshot => _snapshot;

    /// <summary>Reserves a distinct participant without making its actor controllable.</summary>
    public bool AddParticipant(string bindingId, string playerId, string actorId, string label)
    {
        if (!_active || _snapshot.Participants.Length >= 4)
        {
            return false;
        }

        int slot = Enumerable.Range(start: 1, count: 4).First(predicate: candidate => !_snapshot.Participants.Any(
                predicate: participant => participant.Slot == candidate));
        var participant = new FoundationParticipantState(bindingId: bindingId, label: label, slot: slot,
                ready: false, actor: new FoundationActorState(sessionId: Actor.SessionId, playerId: playerId,
                        actorId: actorId, positionSubunits: -8000, facing: 1, elapsedMilliseconds: 0));
        return TryPublish(participants: _snapshot.Participants.Add(item: participant));
    }

    /// <summary>Enables a reserved actor after its current binding installed a baseline.</summary>
    public bool MarkReady(string bindingId)
    {
        FoundationParticipantState? participant = Find(bindingId: bindingId);
        if (!_active || participant is null || participant.Ready)
        {
            return false;
        }

        return TryPublish(participants: _snapshot.Participants.Select(selector: current => current == participant
                ? new FoundationParticipantState(bindingId: current.BindingId, label: current.Label,
                        slot: current.Slot, ready: true, actor: current.Actor) : current));
    }

    /// <summary>Removes exactly one remote binding and its held intent, preserving remaining actors.</summary>
    public bool RemoveParticipant(string bindingId)
    {
        FoundationParticipantState? participant = Find(bindingId: bindingId);
        if (!_active || participant is null || bindingId is "local")
        {
            return false;
        }

        bool removed = TryPublish(participants: _snapshot.Participants.Where(predicate: p => p != participant));
        if (removed)
        {
            _intents.Remove(key: bindingId);
        }

        return removed;
    }

    /// <summary>Accepts a unique owned intent without advancing time or position.</summary>
    public bool TryAccept(FoundationMovementCommand command, string trustedBinding, string trustedPlayer,
            string trustedActor)
    {
        ArgumentNullException.ThrowIfNull(argument: command);
        FoundationParticipantState? participant = Find(bindingId: trustedBinding);
        if (!_active || participant is null || !participant.Ready || command.Direction is < -1 or > 1
                || command.Sequence <= 0 || !Matches(command: command, participant: participant,
                        trustedPlayer: trustedPlayer, trustedActor: trustedActor)
                || _intents.TryGetValue(key: trustedBinding, value: out FoundationHeldIntent? previous)
                        && command.Sequence <= previous.Sequence)
        {
            return false;
        }

        _intents[trustedBinding] = new FoundationHeldIntent(sequence: command.Sequence, direction: command.Direction);
        return true;
    }

    /// <summary>Moves all ready actors once using integer elapsed time supplied by the host.</summary>
    public void Step(long elapsedMilliseconds)
    {
        RequireActive();
        ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
        FoundationParticipantState[] candidate = [.._snapshot.Participants.Select(selector: participant =>
                AdvanceParticipant(participant: participant, elapsedMilliseconds: elapsedMilliseconds))];
        _snapshot = new FoundationSnapshot(sessionId: Actor.SessionId, revision: checked(_snapshot.Revision + 1),
                participants: candidate);
    }

    /// <summary>Retains inherited callers through the same validation and stepping path.</summary>
    public void Advance(FoundationMovementCommand command, long elapsedMilliseconds)
    {
        RequireActive();
        ArgumentNullException.ThrowIfNull(argument: command);
        ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
        long sequence = _intents.TryGetValue(key: "local", value: out FoundationHeldIntent? held)
                ? checked(held.Sequence + 1) : 1;
        if (!TryAccept(command: command with { Sequence = sequence }, trustedBinding: "local",
                trustedPlayer: Actor.PlayerId, trustedActor: Actor.ActorId))
        {
            throw new ArgumentException(message: "FOUNDATION_OWNER_OR_DIRECTION", paramName: nameof(command));
        }

        Step(elapsedMilliseconds: elapsedMilliseconds);
    }

    /// <summary>Closes acceptance before transport and native scene teardown.</summary>
    public void Close()
    {
        _active = false;
        _intents.Clear();
    }

    private FoundationSnapshot _snapshot;
    private readonly Dictionary<string, FoundationHeldIntent> _intents = new(comparer: StringComparer.Ordinal);
    private bool _active = true;

    private FoundationParticipantState? Find(string bindingId)
    {
        return _snapshot.Participants.FirstOrDefault(predicate: participant => StringComparer.Ordinal.Equals(
                x: participant.BindingId, y: bindingId));
    }

    private static bool Matches(FoundationMovementCommand command, FoundationParticipantState participant,
            string trustedPlayer, string trustedActor)
    {
        return StringComparer.Ordinal.Equals(x: command.SessionId, y: participant.Actor.SessionId)
                && StringComparer.Ordinal.Equals(x: command.BindingId, y: participant.BindingId)
                && StringComparer.Ordinal.Equals(x: command.PlayerId, y: trustedPlayer)
                && StringComparer.Ordinal.Equals(x: command.ActorId, y: trustedActor)
                && StringComparer.Ordinal.Equals(x: trustedPlayer, y: participant.Actor.PlayerId)
                && StringComparer.Ordinal.Equals(x: trustedActor, y: participant.Actor.ActorId);
    }

    private FoundationParticipantState AdvanceParticipant(FoundationParticipantState participant,
            long elapsedMilliseconds)
    {
        if (!participant.Ready)
        {
            return participant;
        }

        int direction = _intents.TryGetValue(key: participant.BindingId, value: out FoundationHeldIntent? intent)
                ? intent.Direction : 0;
        FoundationActorState actor = participant.Actor;
        long position = Math.Clamp(value: actor.PositionSubunits + Math.Min(val1: elapsedMilliseconds, val2: 40000)
                * 3 * direction, min: -59000, max: 59000);
        var moved = new FoundationActorState(sessionId: actor.SessionId, playerId: actor.PlayerId,
                actorId: actor.ActorId, positionSubunits: position, facing: direction == 0 ? actor.Facing : direction,
                elapsedMilliseconds: checked(actor.ElapsedMilliseconds + elapsedMilliseconds));
        return new FoundationParticipantState(bindingId: participant.BindingId, label: participant.Label,
                slot: participant.Slot, ready: participant.Ready, actor: moved);
    }

    private bool TryPublish(IEnumerable<FoundationParticipantState> participants)
    {
        try
        {
            _snapshot = new FoundationSnapshot(sessionId: Actor.SessionId, revision: checked(_snapshot.Revision + 1),
                    participants: participants);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    private void RequireActive()
    {
        if (!_active)
        {
            throw new InvalidOperationException(message: "FOUNDATION_CLOSED");
        }
    }
}
