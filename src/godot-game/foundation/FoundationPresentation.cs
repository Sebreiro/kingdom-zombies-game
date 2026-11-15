using System;
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
        return snapshot.Participants.Count(predicate: p => p.Ready && p.Slot < participant.Slot
                && Math.Abs(value: p.Actor.PositionSubunits - participant.Actor.PositionSubunits) < 6000) * 28;
    }
}
