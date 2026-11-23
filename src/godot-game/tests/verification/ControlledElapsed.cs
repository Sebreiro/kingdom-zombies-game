using KingdomZombies.Core;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Provides an explicit test delta without observing a clock.</summary>
internal sealed class ControlledElapsed : IElapsedInput
{
    /// <summary>Gets the supplied integer delta.</summary>
    public long Milliseconds { get; }

    /// <summary>Initializes the supplied delta, including invalid test neighbors.</summary>
    internal ControlledElapsed(long milliseconds)
    {
        Milliseconds = milliseconds;
    }
}
