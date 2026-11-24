using System;

namespace KingdomZombies.Core;

/// <summary>Clips an arrow against linear horizontal motion and exact parabolic jump height.</summary>
internal static class RunTrajectoryContact
{
    internal static decimal? First(RunProjectiles.Arrow arrow,
            RunWorld world, RunLocomotion.Motion[] slices, RunLocomotion.Settings tuning, long from, long to)
    {
        foreach (RunLocomotion.Motion slice in slices)
        {
            decimal start = Math.Max(val1: slice.From, val2: Math.Max(val1: from, val2: arrow.LaunchedAt));
            decimal end = Math.Min(val1: slice.To, val2: Math.Min(val1: to, val2: arrow.ExpiresAt));
            if (end < start)
            {
                continue;
            }

            var path = new RelativePath(arrow: arrow, world: world, slice: slice, tuning: tuning);
            decimal first = 0;
            decimal last = end - start;
            if (RunWorldGeometry.Clip(start: path.X(at: start), end: path.X(at: end), minimum: -250,
                    maximum: 250, duration: last, first: ref first, last: ref last)
                    && Vertical(path: path, from: start + first, to: start + last) is { } contact)
            {
                return Math.Clamp(value: decimal.Round(d: contact, decimals: 6), min: start, max: end);
            }
        }

        return null;
    }

    private static decimal? Vertical(RelativePath path, decimal from, decimal to)
    {
        decimal turn = path.TurningAt();
        if (turn > from && turn < to)
        {
            return Monotone(path: path, from: from, to: turn) ?? Monotone(path: path, from: turn, to: to);
        }

        return Monotone(path: path, from: from, to: to);
    }

    private static decimal? Monotone(RelativePath path, decimal from, decimal to)
    {
        decimal start = path.Y(at: from);
        if (start is >= 0 and <= 1000)
        {
            return from;
        }

        decimal end = path.Y(at: to);
        if (start < 0 ? end < 0 : end > 1000)
        {
            return null;
        }

        for (int refinement = 0; refinement < 60 && to - from > 0.0000000001m; refinement++)
        {
            decimal middle = from + (to - from) / 2;
            if (start < 0 ? path.Y(at: middle) < 0 : path.Y(at: middle) > 1000)
            {
                from = middle;
            }
            else
            {
                to = middle;
            }
        }

        return to;
    }

    private sealed class RelativePath
    {
        internal RelativePath(RunProjectiles.Arrow arrow, RunWorld world, RunLocomotion.Motion slice,
                RunLocomotion.Settings tuning)
        {
            _arrow = arrow;
            _world = world;
            _slice = slice;
            _tuning = tuning;
        }

        internal decimal X(decimal at)
        {
            return _arrow.At(now: at).x - _world.Origin.X - Position(at: at);
        }

        internal decimal Y(decimal at)
        {
            return _arrow.At(now: at).y - _world.Origin.Y - _world.GroundContactSubunits - Height(at: at);
        }

        internal decimal TurningAt()
        {
            decimal duration = _tuning.JumpDuration;
            return _slice.Jump is { } start && _tuning.JumpHeight > 0
                    ? start + duration / 2 - _arrow.Velocity.y * duration * duration
                            / (8 * _tuning.JumpHeight * RunWorld.SubunitsPerUnit)
                    : _slice.From;
        }

        private readonly RunProjectiles.Arrow _arrow;
        private readonly RunWorld _world;
        private readonly RunLocomotion.Motion _slice;
        private readonly RunLocomotion.Settings _tuning;

        private decimal Position(decimal at)
        {
            return _slice.X + (at - _slice.From) * _slice.Velocity;
        }

        private decimal Height(decimal at)
        {
            if (_slice.Jump is not { } start)
            {
                return 0;
            }

            decimal progress = (at - start) / _tuning.JumpDuration;
            return 4 * _tuning.JumpHeight * RunWorld.SubunitsPerUnit * progress * (1 - progress);
        }
    }
}
