using System;
using System.Globalization;
using System.Linq;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Projects a nonfocus-stealing ready roster and explicit temporary-session controls.</summary>
internal static class FoundationHud
{
    internal static void Project(Control root, FoundationSnapshot snapshot, string bindingId, bool hosting,
            bool waitingForHost, MainThreadGateway gateway)
    {
        gateway.RequireThread();
        FoundationParticipantState[] ready = [.. snapshot.Participants.Where(predicate: p => p.Ready)
                .OrderBy(keySelector: p => p.Slot)];
        root.GetNode<Control>(path: "Roster").Visible = hosting || ready.Length > 1;
        root.GetNode<Label>(path: "Roster/Waiting").Visible = waitingForHost;
        root.GetNode<Label>(path: "Hint").Text = hosting
                ? "Move: A / D or Left / Right arrows. Esc: End session and return to main menu."
                : "Move: A / D or Left / Right arrows. Esc: Main menu.";
        root.GetNode<Label>(path: "Roster/Header").Text = (hosting ? "Hosting" : "Connected") + " · "
                + ready.Length.ToString(provider: CultureInfo.InvariantCulture) + "/4 "
                + (ready.Length == 1 ? "player" : "players");
        for (int index = 0; index < 4; index++)
        {
            Label row = root.GetNode<Label>(path: "Roster/Player" + (index + 1).ToString(
                    provider: CultureInfo.InvariantCulture));
            row.Visible = index < ready.Length;
            if (index >= ready.Length)
            {
                continue;
            }

            FoundationParticipantState participant = ready[index];
            string marker = "P" + participant.Slot.ToString(provider: CultureInfo.InvariantCulture)
                    + (StringComparer.Ordinal.Equals(x: participant.BindingId, y: bindingId) ? " (You)" : "");
            row.Text = marker + " · " + participant.Label;
            row.TooltipText = row.Text;
        }
    }
}
