namespace KingdomZombies.Phase1.Tests.Models;
/// <summary>Supplies a concrete constructor-only snapshot for structural proofs.</summary>
internal sealed class BoundarySnapshotDto
{
    /// <summary>Copies the supplied primitive contract values.</summary>
    internal BoundarySnapshotDto(int value, string kind)
    {
        Value = value;
        Kind = kind;
    }

    /// <summary>Gets the copied state.</summary>
    internal int Value { get; private set; }
    /// <summary>Gets the independent wire name.</summary>
    internal string Kind { get; private set; }
}
