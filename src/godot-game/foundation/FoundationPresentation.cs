using System;
using System.Collections.Generic;
using System.Linq;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

/// <summary>Selects presentation from accepted actor identity without introducing movement authority.</summary>
internal static class FoundationPresentation
{
    internal static FoundationParticipantState? Local(FoundationSnapshot snapshot, string bindingId)
    {
        return snapshot.Participants.FirstOrDefault(predicate: p => p.Ready
                && StringComparer.Ordinal.Equals(x: p.BindingId, y: bindingId));
    }

    internal static int LabelOffset(FoundationSnapshot snapshot, FoundationParticipantState participant)
    {
        var placed = new List<(long Position, int Level)>();
        foreach (FoundationParticipantState current in snapshot.Participants.Where(predicate: p => p.Ready)
                .OrderBy(keySelector: p => p.Slot))
        {
            int level = 0;
            while (placed.Any(predicate: previous => previous.Level == level
                    && Math.Abs(value: previous.Position - current.Actor.PositionSubunits) < 6000))
            {
                level++;
            }

            if (StringComparer.Ordinal.Equals(x: current.Actor.ActorId, y: participant.Actor.ActorId))
            {
                return level * 28;
            }

            placed.Add(item: (current.Actor.PositionSubunits, level));
        }

        throw new ArgumentException(message: "Marker actor is not a ready participant.",
                paramName: nameof(participant));
    }
}
