using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KingdomZombies.Foundation;

/// <summary>Contains only gameplay fields with explicitly bounded revision, scope and part identities.</summary>
internal sealed record FoundationRunFrame
{
    [JsonRequired]
    public string Action { get; init; } = "";
    [JsonRequired]
    public string Survivor { get; init; } = "";
    [JsonRequired]
    public string Status { get; init; } = "";
    [JsonRequired]
    public bool Replayed { get; init; }
    [JsonRequired]
    public long Epoch { get; init; }
    [JsonRequired]
    public long View { get; init; }
    [JsonRequired]
    public long Token { get; init; }
    [JsonRequired]
    public string Scope { get; init; } = "";
    [JsonRequired]
    public int Part { get; init; }
    [JsonRequired]
    public int Parts { get; init; }
    [JsonRequired]
    public JsonElement? Body { get; init; }
    [JsonRequired]
    public ImmutableArray<JsonElement> Rows { get; init; } = [];
}
