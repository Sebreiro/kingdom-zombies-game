using System;
using System.Collections.Generic;
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
            MainThreadGateway gateway, bool authored = false)
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
            remote.GetNode<Polygon2D>(path: "Outline").Visible = !authored;
            remote.GetNode<Polygon2D>(path: "Body").Visible = !authored;
            remote.GetNode<Polygon2D>(path: "Facing").Visible = !authored;
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

    /// <summary>Owns native nodes for the latest complete public appearance frame.</summary>
    internal sealed class Appearance : IDisposable
    {
        public void Dispose()
        {
            _gateway.RequireThread();
            foreach (FoundationActorArt art in _actors.Values.Concat(second: _corpses.Values))
            {
                art.Dispose();
            }

            _actors.Clear();
            _corpses.Clear();
            _arrows.Clear();
            _root.GetParent().RemoveChild(node: _root);
            _root.QueueFree();
            _assets.Dispose();
        }

        internal Appearance(Node2D world, MainThreadGateway gateway)
        {
            _gateway = gateway;
            _gateway.RequireThread();
            _assets = new FoundationAppearanceAssets(gateway: gateway);
            _root = new Node2D { Name = "RunAppearance", ZIndex = 1 };
            world.AddChild(node: _root);
        }

        internal bool Ready => _cursor.Current is not null;
        internal FoundationEquipmentProjection.Frame? Frame => _cursor.Current;

        internal void Project(FoundationRunReadback readback, string? local, (double x, double y)? aim)
        {
            _gateway.RequireThread();
            if (readback.World is { } world)
            {
                _ = _cursor.Install(revision: readback.Roster.Revision, world: world);
            }
            if (_cursor.Current is not { } frame)
            {
                return;
            }

            ProjectActors(art: _actors, expected: frame.Actors, frame: frame, local: local, aim: aim);
            ProjectActors(art: _corpses, expected: frame.Corpses, frame: frame, local: null, aim: null);
            ProjectArrows(frame: frame);
        }

        private readonly Node2D _root;
        private readonly MainThreadGateway _gateway;
        private readonly FoundationAppearanceAssets _assets;
        private readonly FoundationActorNode.AppearanceCursor _cursor = new();
        private readonly Dictionary<string, FoundationActorArt> _actors = new(comparer: StringComparer.Ordinal);
        private readonly Dictionary<string, FoundationActorArt> _corpses = new(comparer: StringComparer.Ordinal);
        private readonly Dictionary<long, Sprite2D> _arrows = [];

        private void ProjectActors(Dictionary<string, FoundationActorArt> art,
                IEnumerable<FoundationEquipmentProjection.Subject> expected, FoundationEquipmentProjection.Frame frame,
                string? local, (double x, double y)? aim)
        {
            _gateway.RequireThread();
            var present = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (FoundationEquipmentProjection.Subject subject in expected)
            {
                _ = present.Add(item: subject.Id);
                if (!art.TryGetValue(key: subject.Id, value: out FoundationActorArt? visual))
                {
                    var node = new Node2D { Name = FoundationActorNode.Name(actorId: subject.Id) };
                    _root.AddChild(node: node);
                    visual = new FoundationActorArt(root: node, gateway: _gateway, assets: _assets,
                            body: subject.Body, hostile: subject.Hostile);
                    art.Add(key: subject.Id, value: visual);
                }

                visual.Apply(subject: subject, elapsed: frame.Elapsed, at: Point(frame: frame, position: subject.At),
                        cursor: StringComparer.Ordinal.Equals(x: subject.Id, y: local) ? aim : null);
            }

            foreach (string id in art.Keys.Where(predicate: id => !present.Contains(item: id)).ToArray())
            {
                art[id].Dispose();
                _ = art.Remove(key: id);
            }
        }

        private void ProjectArrows(FoundationEquipmentProjection.Frame frame)
        {
            _gateway.RequireThread();
            foreach (FoundationEquipmentProjection.Arrow arrow in frame.Arrows)
            {
                if (!_arrows.TryGetValue(key: arrow.Id, value: out Sprite2D? node))
                {
                    node = new Sprite2D
                    {
                        Name = "Arrow_" + arrow.Id.ToString(provider: CultureInfo.InvariantCulture),
                        Texture = _assets.Arrow,
                        ZIndex = 45,
                    };
                    _root.AddChild(node: node);
                    _arrows.Add(key: arrow.Id, value: node);
                }

                node.Position = Point(frame: frame, position: arrow.At);
                node.Rotation = (float)Math.Atan2(y: -arrow.Velocity.y, x: arrow.Velocity.x);
            }

            foreach (long id in _arrows.Keys.Where(predicate: id => !frame.Arrows.Any(predicate: arrow =>
                    arrow.Id == id)).ToArray())
            {
                Sprite2D node = _arrows[id];
                _root.RemoveChild(node: node);
                node.QueueFree();
                _ = _arrows.Remove(key: id);
            }
        }

        private static Vector2 Point(FoundationEquipmentProjection.Frame frame, (double x, double y) position)
        {
            return new Vector2(x: (float)((position.x - frame.Origin) * 16 / 1000),
                    y: 540 - (float)((position.y - frame.Ground) * 16 / 1000));
        }
    }
}
