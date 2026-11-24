using System;
using Blocker = KingdomZombies.Core.RunWorld.Blocker;
using Point = KingdomZombies.Core.RunWorld.Point;

namespace KingdomZombies.Core;

/// <summary>Checks exact range and closed terrain intersections for validated world subunits.</summary>
internal static class RunWorldGeometry
{
    internal static bool Within(Point first, Point second, int rangeUnits)
    {
        long x = first.X - second.X;
        long y = first.Y - second.Y;
        long range = rangeUnits * RunWorld.SubunitsPerUnit;
        return x * x + y * y <= range * range;
    }

    internal static bool Blocks(Blocker blocker, Point start, Point end)
    {
        if (Contains(blocker: blocker, point: start) || Contains(blocker: blocker, point: end))
        {
            return true;
        }

        var topLeft = new Point(x: blocker.Minimum.X, y: blocker.Maximum.Y);
        var bottomRight = new Point(x: blocker.Maximum.X, y: blocker.Minimum.Y);
        return Crosses(first: start, second: end, edgeStart: blocker.Minimum, edgeEnd: topLeft)
                || Crosses(first: start, second: end, edgeStart: topLeft, edgeEnd: blocker.Maximum)
                || Crosses(first: start, second: end, edgeStart: blocker.Maximum, edgeEnd: bottomRight)
                || Crosses(first: start, second: end, edgeStart: bottomRight, edgeEnd: blocker.Minimum);
    }

    internal static decimal? FirstContact((decimal x, decimal y) start, (decimal x, decimal y) end,
            (decimal left, decimal bottom, decimal right, decimal top) bounds, decimal duration = 1)
    {
        decimal first = 0;
        decimal last = duration;
        return Clip(start: start.x, end: end.x, minimum: bounds.left, maximum: bounds.right,
                duration: duration, first: ref first, last: ref last)
                && Clip(start: start.y, end: end.y, minimum: bounds.bottom, maximum: bounds.top,
                        duration: duration, first: ref first, last: ref last) ? first : null;
    }

    private static bool Clip(decimal start, decimal end, decimal minimum, decimal maximum, decimal duration,
            ref decimal first, ref decimal last)
    {
        if (Math.Min(val1: start, val2: end) > maximum || Math.Max(val1: start, val2: end) < minimum)
        {
            return false;
        }

        if (start < minimum || start > maximum)
        {
            decimal boundary = start < minimum ? minimum : maximum;
            first = Math.Max(val1: first, val2: (boundary - start) * duration / (end - start));
        }

        if (end < minimum || end > maximum)
        {
            decimal boundary = end < minimum ? minimum : maximum;
            last = Math.Min(val1: last, val2: (boundary - start) * duration / (end - start));
        }

        return first <= last;
    }

    private static bool Contains(Blocker blocker, Point point)
    {
        return point.X >= blocker.Minimum.X && point.X <= blocker.Maximum.X
                && point.Y >= blocker.Minimum.Y && point.Y <= blocker.Maximum.Y;
    }

    private static bool Crosses(Point first, Point second, Point edgeStart, Point edgeEnd)
    {
        return Overlaps(first: first.X, second: second.X, edgeStart: edgeStart.X, edgeEnd: edgeEnd.X)
                && Overlaps(first: first.Y, second: second.Y, edgeStart: edgeStart.Y, edgeEnd: edgeEnd.Y)
                && Opposed(first: Side(start: first, end: second, point: edgeStart),
                        second: Side(start: first, end: second, point: edgeEnd))
                && Opposed(first: Side(start: edgeStart, end: edgeEnd, point: first),
                        second: Side(start: edgeStart, end: edgeEnd, point: second));
    }

    private static bool Overlaps(long first, long second, long edgeStart, long edgeEnd)
    {
        return Math.Max(val1: first, val2: second) >= Math.Min(val1: edgeStart, val2: edgeEnd)
                && Math.Max(val1: edgeStart, val2: edgeEnd) >= Math.Min(val1: first, val2: second);
    }

    private static bool Opposed(long first, long second)
    {
        return first == 0 || second == 0 || Math.Sign(value: first) != Math.Sign(value: second);
    }

    private static long Side(Point start, Point end, Point point)
    {
        return (end.X - start.X) * (point.Y - start.Y) - (end.Y - start.Y) * (point.X - start.X);
    }
}
