namespace KingdomZombies.Phase1.Tests;

/// <summary>Supplies explicitly controlled monotonic time without simulation-delta coupling.</summary>
internal sealed class FoundationScenarioClock
{
    internal long Now { get; set; }
}
