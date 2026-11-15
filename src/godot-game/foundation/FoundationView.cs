using System;
using System.Globalization;
using System.Linq;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Projects authoritative fixed-point state onto the authored primitive scene.</summary>
internal static class FoundationView
{
    /// <summary>Projects each ready actor while following only the locally assigned actor.</summary>
    internal static void ProjectRoster(Node2D world, FoundationSnapshot snapshot, string bindingId,
            MainThreadGateway gateway)
    {
        gateway.RequireThread();
        FoundationParticipantState local = snapshot.Participants.First(predicate: participant => participant.Ready
                && StringComparer.Ordinal.Equals(x: participant.BindingId, y: bindingId));
        Project(world: world, actor: local.Actor, gateway: gateway);
        for (int slot = 1; slot <= 4; slot++)
        {
            string name = "Participant" + slot.ToString(provider: CultureInfo.InvariantCulture);
            Node2D? obsolete = world.GetNodeOrNull<Node2D>(path: name);
            if (obsolete is not null && !snapshot.Participants.Any(predicate: p => p.Ready && p != local
                    && p.Slot == slot))
            {
                world.RemoveChild(node: obsolete);
                obsolete.QueueFree();
            }
        }

        foreach (FoundationParticipantState participant in snapshot.Participants.Where(predicate: participant =>
                participant.Ready && participant != local))
        {
            string name = "Participant" + participant.Slot.ToString(provider: CultureInfo.InvariantCulture);
            Node2D? remote = world.GetNodeOrNull<Node2D>(path: name);
            if (remote is null)
            {
                remote = (Node2D)world.GetNode<Node2D>(path: "Player").Duplicate();
                remote.Name = name;
                world.AddChild(node: remote);
            }

            remote.Position = new Vector2(x: participant.Actor.PositionSubunits * 16f / 1000, y: 540);
            remote.GetNode<Polygon2D>(path: "Facing").Scale = new Vector2(x: participant.Actor.Facing, y: 1);
            remote.GetNode<Polygon2D>(path: "Body").Color = new Color(r: 0.957f, g: 0.949f, b: 0.91f);
            remote.GetNode<Polygon2D>(path: "Facing").Color = new Color(r: 0.957f, g: 0.949f, b: 0.91f);
        }
    }

    /// <summary>Reads position and facing, then projects them at sixteen logical pixels per unit.</summary>
    internal static void Project(Node2D world, FoundationActorState actor, MainThreadGateway gateway)
    {
        gateway.RequireThread();
        Node2D player = world.GetNode<Node2D>(path: "Player");
        float x = actor.PositionSubunits * 16f / 1000;
        player.Position = new Vector2(x: x, y: 540);
        player.GetNode<Polygon2D>(path: "Facing").Scale = new Vector2(x: actor.Facing, y: 1);
        float target = Math.Clamp(value: x, min: -320, max: 320);
        double previous = world.GetMeta(name: "elapsed", @default: -1).AsDouble();
        double seconds = (actor.ElapsedMilliseconds - previous) / 1000;
        float camera = previous < 0 ? target : (float)(640 - world.Position.X
                + (target - (640 - world.Position.X)) * (1 - Math.Exp(d: -seconds / 0.15)));
        world.Position = new Vector2(x: 640 - Math.Clamp(value: camera, min: -320, max: 320), y: 0);
        world.SetMeta(name: "elapsed", value: actor.ElapsedMilliseconds);
    }
}
