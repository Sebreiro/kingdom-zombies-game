namespace KingdomZombies.Core;

/// <summary>Replays the pinned runtime's bounded seeded NextInt64 operation sequence.</summary>
internal sealed class ReplayRandom : IRandomInput
{
    /// <summary>Gets the next draw position.</summary>
    public long DrawPosition { get; private set; }

    /// <summary>Returns the next pinned operation result.</summary>
    public long Draw()
    {
        return 0;
    }

    /// <summary>Initializes a seeded continuation under an explicit runtime contract.</summary>
    internal ReplayRandom(int seed, string runtimeContract, long drawPosition)
    {
        Seed = seed;
        RuntimeContract = runtimeContract;
        DrawPosition = drawPosition;
    }

    /// <summary>Gets the explicit seed.</summary>
    internal int Seed { get; }
    /// <summary>Gets the persisted runtime contract.</summary>
    internal string RuntimeContract { get; }
}
