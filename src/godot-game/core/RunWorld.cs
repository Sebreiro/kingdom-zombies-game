using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace KingdomZombies.Core;

/// <summary>Describes detached ground, camp defenses and interaction geometry in signed world coordinates.</summary>
internal sealed class RunWorld
{
    // One world unit is 1,000 foundation subunits (16 authored logical pixels). The authored ground is elevation zero.
    internal const long SubunitsPerUnit = 1000;
    internal const string StashId = "foundation.stash";

    internal RunWorld(Point origin, long groundContactSubunits, Point mainBuilding, Ranges ranges,
            IEnumerable<Defense> defenses, IEnumerable<Blocker> blockers, IEnumerable<Hostile> hostiles,
            IEnumerable<Anchor> anchors)
    {
        Origin = origin;
        GroundContactSubunits = groundContactSubunits;
        MainBuilding = mainBuilding;
        Settings = ranges;
        Defenses = [.. defenses];
        Blockers = [.. blockers];
        Hostiles = [.. hostiles];
        Anchors = [.. anchors];
    }

    internal Point Origin { get; }
    internal long GroundContactSubunits { get; }
    internal Point MainBuilding { get; }
    internal Ranges Settings { get; }
    internal ImmutableArray<Defense> Defenses { get; }
    internal ImmutableArray<Blocker> Blockers { get; }
    internal ImmutableArray<Hostile> Hostiles { get; }
    internal ImmutableArray<Anchor> Anchors { get; }
    internal (long left, long right) CampBorders => Borders();

    internal static Ranges DefaultRanges { get; } = new(refugeUnits: 12, dangerUnits: 12, localUnits: 3,
            stashUnits: 4);

    internal static RunWorld Foundation { get; } = new(origin: default, groundContactSubunits: 0,
            mainBuilding: default, ranges: DefaultRanges, defenses: [], blockers: [], hostiles: [],
            anchors: [new Anchor(id: StashId, contact: default, kind: Access.Stash)]);

#if DEBUG
    internal static RunWorld Observed { get; } = new(origin: default, groundContactSubunits: 0,
            mainBuilding: default, ranges: DefaultRanges,
            defenses: [new Defense(id: "observation.wall", contact: new Point(x: -6500, y: 0),
                    completed: true, living: true, isWall: true)],
            blockers: [new Blocker(id: "observation.barrier", minimum: new Point(x: -6750, y: 1),
                    maximum: new Point(x: -6500, y: 3500))],
            hostiles: [new Hostile(id: "observation.hostile", contact: new Point(x: -18501, y: 0))],
            anchors: [new Anchor(id: StashId, contact: new Point(x: -6000, y: 3000), kind: Access.Stash)]);
#endif

    internal void Validate()
    {
        if (!CoordinatesValid() || !RangesValid() || !FixturesValid())
        {
            throw new InvalidDataException(message: "RUN_WORLD_GEOMETRY");
        }

        IEnumerable<string> ids = Defenses.Select(selector: defense => defense.Id)
                .Concat(second: Blockers.Select(selector: blocker => blocker.Id))
                .Concat(second: Hostiles.Select(selector: hostile => hostile.Id))
                .Concat(second: Anchors.Select(selector: anchor => anchor.Id));
        var occupied = new HashSet<string>(comparer: StringComparer.Ordinal);
        if (ids.Any(predicate: id => string.IsNullOrWhiteSpace(value: id) || id.Length > 128
                || !occupied.Add(item: id)))
        {
            throw new InvalidDataException(message: "RUN_WORLD_FIXTURE_ID");
        }
    }

    internal (bool insideCamp, bool safeDistance) Safety(Point actor)
    {
        (long left, long right) = Borders();
        return (actor.X >= left && actor.X <= right, !Hostiles.Any(predicate: hostile =>
                RunWorldGeometry.Within(first: actor, second: hostile.Contact, rangeUnits: Settings.DangerUnits)));
    }

    internal bool SafeGroundRest(decimal x)
    {
        (long left, long right) = Borders();
        if (x < left || x > right)
        {
            return false;
        }

        long ground = Origin.Y + GroundContactSubunits;
        long range = Settings.DangerUnits * SubunitsPerUnit;
        return !Hostiles.Any(predicate: hostile =>
        {
            decimal dx = x - hostile.Contact.X;
            long dy = ground - hostile.Contact.Y;
            return dx * dx + dy * dy <= range * range;
        });
    }

    internal bool Reachable(Point actor, Access kind, string id)
    {
        foreach (Anchor anchor in Anchors)
        {
            if (anchor.Kind == kind && StringComparer.Ordinal.Equals(x: anchor.Id, y: id))
            {
                return Reachable(actor: actor, kind: kind, contact: anchor.Contact);
            }
        }

        return false;
    }

    internal bool Reachable(Point actor, Access kind, Point contact)
    {
        int range = kind == Access.Stash ? Settings.StashUnits : Settings.LocalUnits;
        return RunWorldGeometry.Within(first: actor, second: contact, rangeUnits: range)
                && !Blockers.Any(predicate: blocker => RunWorldGeometry.Blocks(blocker: blocker,
                        start: actor, end: contact));
    }

    internal Point ActorContact(FoundationActorState actor)
    {
        return new Point(x: Origin.X + actor.PositionSubunits,
                y: Origin.Y + GroundContactSubunits + actor.GroundSubunits);
    }

    internal Point ActorContact(RunPopulation.Survivor survivor)
    {
        return new Point(x: Origin.X + survivor.PositionSubunits,
                y: Origin.Y + GroundContactSubunits + decimal.ToInt64(d: decimal.Round(d: survivor.HeightSubunits)));
    }

    private const long CoordinateLimit = 1_000_000_000;

    private bool CoordinatesValid()
    {
        const long ActorTravelLimit = 59000;
        const long OriginLimit = CoordinateLimit - ActorTravelLimit;
        return Origin.X is >= -OriginLimit and <= OriginLimit && ValidPoint(point: Origin)
                && GroundContactSubunits is >= -CoordinateLimit and <= CoordinateLimit
                && Origin.Y + GroundContactSubunits is >= -CoordinateLimit and <= CoordinateLimit
                && ValidPoint(point: MainBuilding);
    }

    private bool RangesValid()
    {
        return ValidRange(units: Settings.RefugeUnits) && ValidRange(units: Settings.DangerUnits)
                && ValidRange(units: Settings.LocalUnits) && ValidRange(units: Settings.StashUnits);
    }

    private bool FixturesValid()
    {
        return Defenses.All(predicate: defense => ValidPoint(point: defense.Contact))
                && Hostiles.All(predicate: hostile => ValidPoint(point: hostile.Contact))
                && Anchors.All(predicate: anchor => ValidPoint(point: anchor.Contact)
                        && Enum.IsDefined(value: anchor.Kind))
                && Blockers.All(predicate: blocker => ValidPoint(point: blocker.Minimum)
                        && ValidPoint(point: blocker.Maximum) && blocker.Minimum.X <= blocker.Maximum.X
                        && blocker.Minimum.Y <= blocker.Maximum.Y);
    }

    private (long left, long right) Borders()
    {
        long? left = null;
        long? right = null;
        foreach (Defense defense in Defenses)
        {
            if (!defense.IsWall || !defense.Completed || !defense.Living)
            {
                continue;
            }

            long x = defense.Contact.X;
            if (x < MainBuilding.X)
            {
                left = Math.Min(val1: left ?? x, val2: x);
            }
            if (x > MainBuilding.X)
            {
                right = Math.Max(val1: right ?? x, val2: x);
            }
        }

        long refuge = Settings.RefugeUnits * SubunitsPerUnit;
        return (left ?? MainBuilding.X - refuge, right ?? MainBuilding.X + refuge);
    }

    private static bool ValidRange(int units)
    {
        return units > 0 && units * SubunitsPerUnit <= CoordinateLimit;
    }

    private static bool ValidPoint(Point point)
    {
        return point.X is >= -CoordinateLimit and <= CoordinateLimit
                && point.Y is >= -CoordinateLimit and <= CoordinateLimit;
    }

    [StructLayout(layoutKind: LayoutKind.Auto)]
    internal readonly record struct Point
    {
        internal Point(long x, long y)
        {
            X = x;
            Y = y;
        }

        internal long X { get; }
        internal long Y { get; }
    }

    internal readonly record struct Defense
    {
        internal Defense(string id, Point contact, bool completed, bool living, bool isWall)
        {
            Id = id;
            Contact = contact;
            Completed = completed;
            Living = living;
            IsWall = isWall;
        }

        internal string Id { get; }
        internal Point Contact { get; }
        internal bool Completed { get; }
        internal bool Living { get; init; }
        internal bool IsWall { get; }
    }

    internal readonly record struct Blocker
    {
        internal Blocker(string id, Point minimum, Point maximum)
        {
            Id = id;
            Minimum = minimum;
            Maximum = maximum;
        }

        internal string Id { get; }
        internal Point Minimum { get; }
        internal Point Maximum { get; }
    }

    internal readonly record struct Hostile
    {
        internal Hostile(string id, Point contact)
        {
            Id = id;
            Contact = contact;
        }

        internal string Id { get; }
        internal Point Contact { get; }
    }

    internal readonly record struct Anchor
    {
        internal Anchor(string id, Point contact, Access kind)
        {
            Id = id;
            Contact = contact;
            Kind = kind;
        }

        internal string Id { get; }
        internal Point Contact { get; }
        internal Access Kind { get; }
    }

    [StructLayout(layoutKind: LayoutKind.Auto)]
    internal readonly record struct Ranges
    {
        internal Ranges(int refugeUnits, int dangerUnits, int localUnits, int stashUnits)
        {
            RefugeUnits = refugeUnits;
            DangerUnits = dangerUnits;
            LocalUnits = localUnits;
            StashUnits = stashUnits;
        }

        internal int RefugeUnits { get; }
        internal int DangerUnits { get; }
        internal int LocalUnits { get; }
        internal int StashUnits { get; }
    }

    internal enum Access { Stash, Npc, Corpse, Pile }
}
