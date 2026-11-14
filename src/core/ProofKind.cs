using System.Text.Json.Serialization;

namespace KingdomZombies.Core;

/// <summary>Names the explicit proof-state values; the zero sentinel is never valid saved state.</summary>
internal enum ProofKind
{
    /// <summary>Marks an invalid or absent state kind.</summary>
    [JsonStringEnumMemberName(name: "unknown")]
    Unknown = 0,
    /// <summary>Marks the only supported minimal non-game sample.</summary>
    [JsonStringEnumMemberName(name: "sample")]
    Sample = 1
}
