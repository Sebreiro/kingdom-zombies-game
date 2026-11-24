namespace KingdomZombies.Core;

/// <summary>Holds accepted sequence and direction separately from elapsed state.</summary>
internal sealed class FoundationHeldIntent
{
    internal FoundationHeldIntent(long sequence, int direction, bool sprint = false)
    {
        Sequence = sequence;
        Direction = direction;
        Sprint = sprint;
    }

    internal long Sequence { get; }
    internal int Direction { get; }
    internal bool Sprint { get; }
}
