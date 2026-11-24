using System.Collections.Immutable;
using System.Text.Json;

namespace KingdomZombies.Foundation;

/// <summary>Holds the one complete private window accepted for a recipient scope.</summary>
internal sealed class FoundationRunView
{
    internal FoundationRunView(long revision, long epoch, long token, JsonElement header,
            ImmutableArray<JsonElement> rows, long correlation = 0, long serial = 0)
    {
        Revision = revision;
        Epoch = epoch;
        Token = token;
        Header = header;
        Rows = rows;
        Correlation = correlation;
        Serial = serial;
    }

    internal long Revision { get; }
    internal long Epoch { get; }
    internal long Token { get; }
    internal JsonElement Header { get; }
    internal ImmutableArray<JsonElement> Rows { get; }
    internal long Correlation { get; }
    internal long Serial { get; }
}
