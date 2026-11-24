using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Owns native nodes for the latest complete public appearance frame.</summary>
internal sealed class FoundationAppearance : IDisposable
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

    internal FoundationAppearance(Node2D world, MainThreadGateway gateway)
    {
        gateway.RequireThread();
        _gateway = gateway;
        _assets = new FoundationAppearanceAssets(gateway: gateway);
        _root = new Node2D { Name = "RunAppearance", ZIndex = 1, Visible = false };
        world.AddChild(node: _root);
    }

    internal FoundationEquipmentProjection.Frame? Frame => _cursor.Current;
    internal string? RetiredLocal { get; set; }

    /// <summary>Applies current state and leaves a network poll between new subject compositions.</summary>
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

        int remaining = 1;
        ProjectActors(art: _actors, expected: frame.Actors, frame: frame, local: local, aim: aim,
                remaining: ref remaining);
        ProjectActors(art: _corpses, expected: frame.Corpses, frame: frame, local: null, aim: null,
                remaining: ref remaining);
        ProjectArrows(frame: frame);
        if (!_root.Visible && _actors.Count == frame.Actors.Length && _corpses.Count == frame.Corpses.Length)
        {
            _root.Visible = true;
        }
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
            string? local, (double x, double y)? aim, ref int remaining)
    {
        _gateway.RequireThread();
        var present = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (FoundationEquipmentProjection.Subject subject in expected)
        {
            if (ReferenceEquals(objA: art, objB: _actors)
                    && StringComparer.Ordinal.Equals(x: subject.Id, y: RetiredLocal))
            {
                continue;
            }

            _ = present.Add(item: subject.Id);
            if (!art.TryGetValue(key: subject.Id, value: out FoundationActorArt? visual))
            {
                if (remaining <= 0)
                {
                    continue;
                }

                remaining--;
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

    private Vector2 Point(FoundationEquipmentProjection.Frame frame, (double x, double y) position)
    {
        _gateway.RequireThread();
        return new Vector2(x: (float)((position.x - frame.Origin) * 16 / 1000),
                y: 540 - (float)((position.y - frame.Ground) * 16 / 1000));
    }
}
