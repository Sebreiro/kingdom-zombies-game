namespace KingdomZombies.Foundation;

/// <summary>Holds framework-only runtime feedback and the engine's sampled monotonic clock.</summary>
internal sealed class FoundationRuntimeState
{
    internal bool Hosting { get; set; }
    internal long NowMilliseconds { get; set; }
    internal string Feedback { get; set; } = "";
}
