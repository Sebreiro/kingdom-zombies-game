using System;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Projects authoritative fixed-point state onto the authored primitive scene.</summary>
internal static class FoundationView
{
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
