namespace KingdomZombies.Multiplayer.Abstractions;

/// <summary>Declares the independent framework-value port used by composition proofs.</summary>
public interface IProofService
{
    /// <summary>Returns the supplied value through a service boundary.</summary>
    /// <param name="value">The independent integer contract.</param>
    /// <returns>The value supplied to the implementation.</returns>
    int Echo(int value);
}
