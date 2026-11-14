using KingdomZombies.Core;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Provides one controlled value and observes draw consumption.</summary>
internal sealed class ControlledRandom : IRandomInput
{
    /// <summary>Gets accepted draw consumption.</summary>
    public long DrawPosition { get; private set; }

    /// <summary>Returns the supplied value and records one draw.</summary>
    public long Draw()
    {
        DrawPosition++;
        return _value;
    }

    /// <summary>Initializes the explicit test draw.</summary>
    internal ControlledRandom(long value)
    {
        _value = value;
    }

    private readonly long _value;
}
