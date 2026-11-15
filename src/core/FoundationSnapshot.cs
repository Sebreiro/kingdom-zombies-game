using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Publishes one coherent identified foundation roster at a monotonic revision.</summary>
public sealed class FoundationSnapshot
{
    /// <summary>Validates all detached participants before publishing an immutable collection.</summary>
    public FoundationSnapshot(string sessionId, long revision, IEnumerable<FoundationParticipantState> participants)
    {
        ArgumentNullException.ThrowIfNull(argument: participants);
        Participants = [.. participants];
        SessionId = sessionId;
        Revision = revision;
        Validate();
    }

    /// <summary>Gets the exact session lifetime identity.</summary>
    public string SessionId { get; }
    /// <summary>Gets the current host revision.</summary>
    public long Revision { get; }
    /// <summary>Gets the immutable complete roster, including unready reservations.</summary>
    public ImmutableArray<FoundationParticipantState> Participants { get; }
    /// <summary>Gets the wire schema.</summary>
    public int SchemaVersion { get; } = 1;
    /// <summary>Gets the exact authored content tuple.</summary>
    public string ContentId { get; } =
            "foundation.fixture.v1|foundation.ground.v1|foundation.building.v1|foundation.player.v1";

    private void Validate()
    {
        if (!ValidId(value: SessionId) || Revision < 0 || Participants.Length is < 1 or > 4)
        {
            throw new InvalidDataException(message: "FOUNDATION_SNAPSHOT");
        }

        foreach (FoundationParticipantState participant in Participants)
        {
            ValidateParticipant(participant: participant);
        }

        bool uniqueBindings = Participants.Select(selector: p => p.BindingId).Distinct(comparer: StringComparer.Ordinal)
                .Count() == Participants.Length;
        bool uniquePlayers = Participants.Select(selector: p => p.Actor.PlayerId)
                .Distinct(comparer: StringComparer.Ordinal).Count() == Participants.Length;
        bool uniqueActors = Participants.Select(selector: p => p.Actor.ActorId)
                .Distinct(comparer: StringComparer.Ordinal).Count() == Participants.Length;
        bool uniqueSlots = Participants.Select(selector: p => p.Slot).Distinct().Count() == Participants.Length;
        bool host = Participants.Any(predicate: p => p.BindingId is "local" && p.Slot == 1 && p.Ready);
        if (!uniqueBindings || !uniquePlayers || !uniqueActors || !uniqueSlots || !host)
        {
            throw new InvalidDataException(message: "FOUNDATION_DUPLICATE");
        }
    }

    private void ValidateParticipant(FoundationParticipantState participant)
    {
        ArgumentNullException.ThrowIfNull(argument: participant);
        FoundationActorState actor = participant.Actor;
        if (actor is null || !StringComparer.Ordinal.Equals(x: actor.SessionId, y: SessionId)
                || !ValidId(value: participant.BindingId) || !ValidId(value: actor.PlayerId)
                || !ValidId(value: actor.ActorId) || string.IsNullOrWhiteSpace(value: participant.Label)
                || participant.Label.EnumerateRunes().Count() > 256 || participant.Slot is < 1 or > 4
                || actor.PositionSubunits is < -59000 or > 59000 || actor.Facing is not (-1 or 1)
                || actor.ElapsedMilliseconds < 0)
        {
            throw new InvalidDataException(message: "FOUNDATION_PARTICIPANT");
        }
    }

    private static bool ValidId(string value)
    {
        return !string.IsNullOrWhiteSpace(value: value) && value.Length <= 128;
    }
}
