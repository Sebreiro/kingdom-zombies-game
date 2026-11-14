namespace KingdomZombies.Core;

/// <summary>Supplies an explicitly owned random draw to a pure operation.</summary>
internal interface IRandomInput
{
    /// <summary>Gets the number of accepted draws.</summary>
    long DrawPosition { get; }

    /// <summary>Returns one integer in the half-open range zero through long.MaxValue.</summary>
    long Draw();
}
