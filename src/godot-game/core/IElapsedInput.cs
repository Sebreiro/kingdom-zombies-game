namespace KingdomZombies.Core;

/// <summary>Supplies explicit elapsed integer milliseconds to a pure operation.</summary>
internal interface IElapsedInput
{
    /// <summary>Gets the elapsed delta; the operation validates its range.</summary>
    long Milliseconds { get; }
}
