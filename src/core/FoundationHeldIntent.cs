namespace KingdomZombies.Core;

/// <summary>Holds accepted sequence and direction separately from elapsed state.</summary>
internal sealed class FoundationHeldIntent
{
    internal FoundationHeldIntent(long sequence, int direction)
    {
        Sequence = sequence;
        Direction = direction;
    }

    internal long Sequence { get; }
    internal int Direction { get; }
}
