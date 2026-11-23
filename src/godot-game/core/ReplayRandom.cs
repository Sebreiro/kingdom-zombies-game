using System;

namespace KingdomZombies.Core;

/// <summary>Replays the pinned runtime's bounded seeded NextInt64 operation sequence.</summary>
internal sealed class ReplayRandom : IRandomInput
{
    /// <summary>Gets the next draw position.</summary>
    public long DrawPosition { get; private set; }

    /// <summary>Returns the next pinned operation result.</summary>
    public long Draw()
    {
        if (DrawPosition == MaxDrawPosition)
        {
            throw new InvalidOperationException(message: "RANDOM_EXHAUSTED: bounded stream has no next draw");
        }

        long value = _random.NextInt64(minValue: 0, maxValue: long.MaxValue);
        DrawPosition++;
        return value;
    }

    /// <summary>Names the exact runtime and operation sequence required for continuation.</summary>
    internal const string PinnedRuntimeContract = "dotnet-10.0.15/next-int64-v1";
    /// <summary>Bounds persisted replay work and accepted draw consumption.</summary>
    internal const long MaxDrawPosition = 1_000_000;

    /// <summary>Initializes a seeded continuation under an explicit runtime contract.</summary>
    internal ReplayRandom(int seed, string runtimeContract, long drawPosition)
    {
        ArgumentNullException.ThrowIfNull(argument: runtimeContract);
        ArgumentOutOfRangeException.ThrowIfNegative(value: drawPosition);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value: drawPosition, other: MaxDrawPosition);
        if (!StringComparer.Ordinal.Equals(x: runtimeContract, y: PinnedRuntimeContract)
                || !StringComparer.Ordinal.Equals(x: Environment.Version.ToString(), y: "10.0.15"))
        {
            throw new ArgumentException(message: "RANDOM_CONTRACT: pinned runtime/operation mismatch",
                    paramName: nameof(runtimeContract));
        }

        Seed = seed;
        RuntimeContract = runtimeContract;
        _random = new Random(Seed: seed);
        for (long position = 0; position < drawPosition; position++)
        {
            _random.NextInt64(minValue: 0, maxValue: long.MaxValue);
        }

        DrawPosition = drawPosition;
    }

    /// <summary>Gets the explicit seed.</summary>
    internal int Seed { get; }
    /// <summary>Gets the persisted runtime contract.</summary>
    internal string RuntimeContract { get; }

    private readonly Random _random;
}
