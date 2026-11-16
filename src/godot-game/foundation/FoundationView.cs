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
        FoundationParticipantState local = FoundationPresentation.Local(snapshot: snapshot, bindingId: bindingId)
                ?? throw new InvalidOperationException(message: "PRESENTATION_LOCAL_NOT_READY");
        Project(world: world, actor: local.Actor, gateway: gateway);
        foreach (Node child in world.GetChildren())
        {
            string childName = child.Name;
            if (childName.StartsWith(value: "Actor_", comparisonType: StringComparison.Ordinal)
                    && !snapshot.Participants.Any(predicate: p => p.Ready && p != local
                            && StringComparer.Ordinal.Equals(x: FoundationActorNode.Name(actorId: p.Actor.ActorId),
                                    y: childName)))
            {
                world.RemoveChild(node: child);
                child.QueueFree();
            }
        }

        foreach (FoundationParticipantState participant in snapshot.Participants.Where(predicate: participant =>
                participant.Ready))
        {
            bool own = participant == local;
            string name = own ? "Player" : FoundationActorNode.Name(actorId: participant.Actor.ActorId);
            Node2D? remote = world.GetNodeOrNull<Node2D>(path: name);
            if (remote is null)
            {
                remote = (Node2D)world.GetNode<Node2D>(path: "Player").Duplicate();
                remote.Name = name;
                world.AddChild(node: remote);
            }

            remote.Position = new Vector2(x: participant.Actor.PositionSubunits * 16f / 1000, y: 540);
            remote.GetNode<Polygon2D>(path: "Facing").Scale = new Vector2(x: participant.Actor.Facing, y: 1);
            var color = own ? new Color(r: 0.941f, g: 0.769f, b: 0.416f)
                    : new Color(r: 0.957f, g: 0.949f, b: 0.91f);
            remote.GetNode<Polygon2D>(path: "Body").Color = color;
            remote.GetNode<Polygon2D>(path: "Facing").Color = color;
            Label marker = remote.GetNode<Label>(path: "Marker");
            marker.Text = "P" + participant.Slot.ToString(provider: CultureInfo.InvariantCulture)
                    + (own ? " · You" : "");
            marker.Modulate = color;
            marker.Position = new Vector2(x: FoundationPresentation.MarkerOffset(
                    actorScreenX: remote.GetGlobalTransformWithCanvas().Origin.X, markerWidth: marker.Size.X,
                    viewportWidth: world.GetViewportRect().Size.X), y: -112 - FoundationPresentation.LabelOffset(
                            snapshot: snapshot, participant: participant));
        }
    }

    /// <summary>Reads position and facing, then projects them at sixteen logical pixels per unit.</summary>
    internal static void Project(Node2D world, FoundationActorState actor, MainThreadGateway gateway)
    {
        gateway.RequireThread();
        Node2D player = world.GetNode<Node2D>(path: "Player");
        float x = actor.PositionSubunits * 16f / 1000;
        x = float.IsFinite(f: x) ? x : 0;
        player.Position = new Vector2(x: x, y: 540);
        player.GetNode<Polygon2D>(path: "Facing").Scale = new Vector2(x: actor.Facing, y: 1);
        float target = Math.Clamp(value: x, min: -320, max: 320);
        double now = Time.GetTicksMsec();
        double previous = world.GetMeta(name: "projection_tick", @default: -1).AsDouble();
        double seconds = Math.Clamp(value: (now - previous) / 1000, min: 0, max: 0.25);
        float camera = previous < 0 ? target : (float)(640 - world.Position.X
                + (target - (640 - world.Position.X)) * (1 - Math.Exp(d: -seconds / 0.15)));
        camera = float.IsFinite(f: camera) ? camera : target;
        world.Position = new Vector2(x: 640 - Math.Clamp(value: camera, min: -320, max: 320), y: 0);
        world.SetMeta(name: "projection_tick", value: now);
    }
}
