using System.Collections.Immutable;
using System.Text.Json;

namespace KingdomZombies.Foundation;

/// <summary>Holds the one complete private window accepted for a recipient scope.</summary>
internal sealed class FoundationRunView
{
    internal FoundationRunView(long revision, long epoch, long token, JsonElement header,
            ImmutableArray<JsonElement> rows)
    {
        Revision = revision;
        Epoch = epoch;
        Token = token;
        Header = header;
        Rows = rows;
    }

    internal long Revision { get; }
    internal long Epoch { get; }
    internal long Token { get; }
    internal JsonElement Header { get; }
    internal ImmutableArray<JsonElement> Rows { get; }
}
