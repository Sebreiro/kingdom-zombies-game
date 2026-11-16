using System.Text.Json.Serialization;

namespace KingdomZombies.Phase1.Tests.Models;
/// <summary>Supplies a concrete pinned enum solely for architecture contract proofs.</summary>
internal enum BoundaryKind
{
    [JsonStringEnumMemberName(name: "unknown")]
    Unknown = 0,
    [JsonStringEnumMemberName(name: "ready")]
    Ready = 1,
}
