using System;
using System.Collections.Generic;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Integrates held movement and one-time jump spending for an actual survivor.</summary>
internal static class RunLocomotion
{
    internal static Burden Load(RunInventory inventory, RunDefinitions definitions, string survivorId)
    {
        decimal weight = inventory.CarriedBy(survivorId: survivorId).Sum(selector: item =>
                definitions.Items[item.DefinitionId].WeightKilograms * item.Quantity);
        LoadCurve curve = definitions.Locomotion.Load;
        decimal fraction = Math.Clamp(value: (weight - curve.Soft) / (curve.Severe - curve.Soft), min: 0, max: 1);
        LoadLevel level = weight >= curve.Severe ? LoadLevel.Overloaded : weight > curve.Soft
                ? LoadLevel.Heavy : LoadLevel.Normal;
        return new Burden(weight: weight, level: level, speed: 1 - (1 - curve.MinimumSpeed) * fraction,
                actionCost: 1 + (curve.MaximumCost - 1) * fraction);
    }

    internal static (RunPopulation.Survivor? survivor, Block reason) Jump(RunPopulation.Survivor survivor,
            RunDefinitions definitions, Burden load, long now)
    {
        if (survivor.JumpStartedAt.HasValue)
        {
            return (null, Block.Airborne);
        }

        if (load.Level == LoadLevel.Overloaded)
        {
            return (null, Block.Overloaded);
        }

        RunPopulation.Survivor? paid = RunVitals.Spend(survivor: survivor,
                amount: definitions.Locomotion.JumpCost * load.ActionCost, now: now);
        return paid is null ? (null, Block.InsufficientStamina)
                : (paid with { JumpStartedAt = now }, Block.None);
    }

    internal static RunPopulation.Survivor Advance(RunPopulation.Survivor survivor, RunDefinitions definitions,
            RunInventory inventory, RunWorld world, FoundationHeldIntent? intent, long generation, long previous,
            long current)
    {
        int direction = intent?.Direction ?? 0;
        Burden load = direction == 0 ? Burden.Unloaded
                : Load(inventory: inventory, definitions: definitions, survivorId: survivor.Id);
        decimal groundedFrom = previous;
        if (survivor.JumpStartedAt is { } started)
        {
            groundedFrom = Math.Min(val1: current, val2: started + definitions.Locomotion.JumpDuration);
            survivor = AdvanceAir(survivor: survivor, definitions: definitions, direction: direction,
                    speedScale: load.Speed, start: started, previous: previous, current: groundedFrom);
            if (survivor.JumpStartedAt.HasValue)
            {
                return survivor;
            }
        }

        return AdvanceGround(survivor: survivor, definitions: definitions, intent: intent, generation: generation,
                world: world, load: load, previous: groundedFrom, current: current);
    }

    internal static IEnumerable<Motion> Trace(RunPopulation.Survivor survivor, RunDefinitions definitions,
            RunInventory inventory, FoundationHeldIntent? intent, long generation, long previous, decimal until)
    {
        int direction = intent?.Direction ?? 0;
        Burden load = direction == 0 ? Burden.Unloaded
                : Load(inventory: inventory, definitions: definitions, survivorId: survivor.Id);
        Settings tuning = definitions.Locomotion;
        decimal from = previous;
        decimal walking = direction * tuning.WalkSpeed * load.Speed;
        if (survivor.JumpStartedAt is { } start)
        {
            from = Math.Min(val1: until, val2: start + tuning.JumpDuration);
            foreach (Motion air in BoundedMotion(from: previous, to: from, x: survivor.PositionSubunitsExact,
                    velocity: walking * tuning.AirControl, jump: start))
            {
                yield return air;
            }

            if (from == until)
            {
                yield break;
            }

            survivor = AdvanceAir(survivor: survivor, definitions: definitions, direction: direction,
                    speedScale: load.Speed, start: start, previous: previous, current: from);
        }

        if (intent is { Sprint: true } && direction != 0)
        {
            (RunPopulation.Survivor sprinted, decimal end) = Sprint(survivor: survivor, tuning: tuning,
                    intent: intent, generation: generation, load: load, previous: from, current: until);
            if (end > from)
            {
                foreach (Motion sprint in BoundedMotion(from: from, to: end, x: survivor.PositionSubunitsExact,
                        velocity: direction * tuning.SprintSpeed * load.Speed))
                {
                    yield return sprint;
                }
            }

            (survivor, from) = (sprinted, end);
        }

        foreach (Motion walk in BoundedMotion(from: from, to: until, x: survivor.PositionSubunitsExact,
                velocity: walking))
        {
            yield return walk;
        }
    }

    private static IEnumerable<Motion> BoundedMotion(decimal from, decimal to, decimal x, decimal velocity,
            long? jump = null)
    {
        if (velocity != 0)
        {
            decimal limit = velocity > 0 ? 59000 : -59000;
            decimal stop = from + (limit - x) / velocity;
            if (stop < to)
            {
                if (stop > from)
                {
                    yield return new Motion(from: from, to: stop, x: x, velocity: velocity, jump: jump);
                }

                yield return new Motion(from: stop, to: to, x: limit, velocity: 0, jump: jump);
                yield break;
            }
        }

        yield return new Motion(from: from, to: to, x: x, velocity: velocity, jump: jump);
    }

    private static RunPopulation.Survivor AdvanceAir(RunPopulation.Survivor survivor, RunDefinitions definitions,
            int direction, decimal speedScale, long start, long previous, decimal current)
    {
        Settings tuning = definitions.Locomotion;
        decimal progress = Math.Clamp(value: (current - start) / tuning.JumpDuration, min: 0, max: 1);
        RunPopulation.Survivor airborne = Move(survivor: survivor, direction: direction,
                speed: tuning.WalkSpeed * tuning.AirControl * speedScale, duration: current - previous);
        return RunVitals.RegenerateStamina(survivor: RunVitals.ResetRest(survivor: airborne) with
        {
            HeightSubunits = 4 * tuning.JumpHeight * RunWorld.SubunitsPerUnit * progress * (1 - progress),
            JumpStartedAt = progress == 1 ? null : start,
        }, definitions: definitions, previous: previous, current: current);
    }

    private static RunPopulation.Survivor AdvanceGround(RunPopulation.Survivor survivor, RunDefinitions definitions,
            FoundationHeldIntent? intent, long generation, RunWorld world, Burden load, decimal previous,
            decimal current)
    {
        int direction = intent?.Direction ?? 0;
        decimal walkedFrom = previous;
        decimal before = survivor.PositionSubunitsExact;
        if (intent is { Sprint: true } && direction != 0)
        {
            (survivor, walkedFrom) = Sprint(survivor: survivor, tuning: definitions.Locomotion, intent: intent,
                    generation: generation, load: load, previous: previous, current: current);
        }
        else
        {
            survivor = survivor with { SprintBlock = Block.None };
        }

        if (walkedFrom == current && walkedFrom > previous && survivor.SprintBlock == Block.None)
        {
            return survivor.PositionSubunitsExact == before ? survivor
                    : survivor with { MovementPose = "faster", MovementAt = current };
        }

        RunPopulation.Survivor walked = Walk(survivor: survivor, definitions: definitions, world: world,
                direction: direction, speed: definitions.Locomotion.WalkSpeed * load.Speed, previous: walkedFrom,
                current: current);
        if (walked.PositionSubunitsExact != before)
        {
            walked = walked with { MovementPose = walkedFrom == current ? "faster" : "walk", MovementAt = current };
        }

        return RunVitals.RegenerateStamina(survivor: walked, definitions: definitions, previous: walkedFrom,
                current: current);
    }

    private static RunPopulation.Survivor Walk(RunPopulation.Survivor survivor, RunDefinitions definitions,
            RunWorld world, int direction, decimal speed, decimal previous, decimal current)
    {
        decimal duration = current - previous;
        decimal from = survivor.PositionSubunitsExact;
        decimal limit = direction > 0 ? 59000 - from : from + 59000;
        decimal moving = direction == 0 ? 0 : Math.Min(val1: duration, val2: limit / speed);
        RunPopulation.Survivor next = Move(survivor: survivor, direction: direction, speed: speed, duration: duration);
        return RunVitals.RestAfterWalk(survivor: next, definitions: definitions, world: world, from: from,
                velocity: direction * speed, previous: previous, moving: moving, current: current);
    }

    private static (RunPopulation.Survivor survivor, decimal end) Sprint(RunPopulation.Survivor survivor,
            Settings tuning, FoundationHeldIntent intent, long generation, Burden load, decimal previous,
            decimal current)
    {
        (long generation, long sequence) fence = (generation, intent.Sequence);
        if (survivor.SprintFence == fence)
        {
            return (survivor, previous);
        }

        if (load.Level == LoadLevel.Overloaded || survivor.Stamina <= 0)
        {
            return (survivor with
            {
                SprintFence = fence,
                SprintBlock = load.Level == LoadLevel.Overloaded ? Block.Overloaded : Block.InsufficientStamina,
            }, previous);
        }

        decimal perMillisecond = tuning.SprintCost * load.ActionCost / 1000;
        decimal affordable = survivor.Stamina / perMillisecond;
        decimal duration = Math.Min(val1: current - previous, val2: affordable);
        decimal end = previous + duration;
        RunPopulation.Survivor moved = Move(survivor: survivor, direction: intent.Direction,
                speed: tuning.SprintSpeed * load.Speed, duration: duration);
        decimal cost = duration == affordable ? survivor.Stamina : duration * perMillisecond;
        RunPopulation.Survivor paid = RunVitals.Spend(survivor: moved, amount: cost, now: end) ?? moved;
        return (paid with
        {
            SprintFence = paid.Stamina == 0 ? fence : null,
            SprintBlock = paid.Stamina == 0 ? Block.InsufficientStamina : Block.None,
        }, end);
    }

    private static RunPopulation.Survivor Move(RunPopulation.Survivor survivor, int direction, decimal speed,
            decimal duration)
    {
        return direction == 0 ? survivor : survivor with
        {
            PositionSubunitsExact = Math.Clamp(value: survivor.PositionSubunitsExact + direction * speed * duration,
                    min: -59000, max: 59000),
            Facing = direction,
        };
    }

    internal enum Block { None, Overloaded, InsufficientStamina, Airborne }
    internal enum LoadLevel { Normal, Heavy, Overloaded }

    internal readonly record struct Motion
    {
        internal Motion(decimal from, decimal to, decimal x, decimal velocity, long? jump)
        {
            From = from;
            To = to;
            X = x;
            Velocity = velocity;
            Jump = jump;
        }

        internal decimal From { get; }
        internal decimal To { get; }
        internal decimal X { get; }
        internal decimal Velocity { get; }
        internal long? Jump { get; }
    }

    internal sealed record Input
    {
        internal Input(bool left = false, bool right = false, bool sprint = false, bool jump = false)
        {
            Direction = (right ? 1 : 0) - (left ? 1 : 0);
            Sprint = sprint;
            Jump = jump;
        }

        internal int Direction { get; }
        internal bool Sprint { get; }
        internal bool Jump { get; }
    }

    internal sealed record Burden
    {
        internal Burden(decimal weight, LoadLevel level, decimal speed, decimal actionCost)
        {
            Weight = weight;
            Level = level;
            Speed = speed;
            ActionCost = actionCost;
        }

        internal static Burden Unloaded { get; } = new(weight: 0, level: LoadLevel.Normal, speed: 1, actionCost: 1);
        internal decimal Weight { get; }
        internal LoadLevel Level { get; }
        internal decimal Speed { get; }
        internal decimal ActionCost { get; }
    }

    internal sealed record LoadCurve
    {
        internal LoadCurve(decimal soft, decimal severe, decimal minimumSpeed, decimal maximumCost)
        {
            Soft = soft;
            Severe = severe;
            MinimumSpeed = minimumSpeed;
            MaximumCost = maximumCost;
        }

        internal decimal Soft { get; }
        internal decimal Severe { get; }
        internal decimal MinimumSpeed { get; }
        internal decimal MaximumCost { get; }
    }

    internal sealed record Settings
    {
        internal Settings(decimal walkSpeed, decimal sprintSpeed, decimal jumpHeight, decimal jumpDuration,
                decimal airControl, decimal sprintCost, decimal jumpCost, LoadCurve load)
        {
            WalkSpeed = walkSpeed;
            SprintSpeed = sprintSpeed;
            JumpHeight = jumpHeight;
            JumpDuration = jumpDuration;
            AirControl = airControl;
            SprintCost = sprintCost;
            JumpCost = jumpCost;
            Load = load;
        }

        internal static Settings Default { get; } = new(walkSpeed: 3, sprintSpeed: 5, jumpHeight: 1.5m,
                jumpDuration: 800, airControl: 0.75m, sprintCost: 10, jumpCost: 12,
                load: new LoadCurve(soft: 20, severe: 35, minimumSpeed: 0.55m, maximumCost: 1.5m));
        internal decimal WalkSpeed { get; }
        internal decimal SprintSpeed { get; }
        internal decimal JumpHeight { get; }
        internal decimal JumpDuration { get; }
        internal decimal AirControl { get; }
        internal decimal SprintCost { get; }
        internal decimal JumpCost { get; }
        internal LoadCurve Load { get; }
    }
}
