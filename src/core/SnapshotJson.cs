using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace KingdomZombies.Core;

/// <summary>Enforces bounded JSON syntax before semantic collection conversion.</summary>
internal static class SnapshotJson
{
    /// <summary>Parses exact UTF-8 with no comments, trailing commas or duplicate object names.</summary>
    internal static JsonDocument Parse(ReadOnlyMemory<byte> document)
    {
        if (document.Length is 0 or > SnapshotCodec.MaxBytes)
        {
            throw new FormatException(message: "SNAPSHOT_BYTES: expected 1..1048576 UTF-8 bytes");
        }

        _utf8.GetCharCount(bytes: document.Span);
        var parsed = JsonDocument.Parse(utf8Json: document,
                options: new JsonDocumentOptions { MaxDepth = SnapshotCodec.MaxDepth });
        try
        {
            ValidateNames(element: parsed.RootElement);
            return parsed;
        }
        catch
        {
            parsed.Dispose();
            throw;
        }
    }

    /// <summary>Requires every exact name once and rejects all extra schema fields.</summary>
    internal static void Fields(JsonElement element, string[] names)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new FormatException(message: "SNAPSHOT_OBJECT: nonnull object required");
        }

        var expected = new HashSet<string>(collection: names, comparer: StringComparer.Ordinal);
        foreach (JsonProperty property in element.EnumerateObject())
        {
            if (!expected.Remove(item: property.Name))
            {
                throw new FormatException(message: "SNAPSHOT_FIELDS: unknown property " + property.Name);
            }
        }

        if (expected.Count != 0)
        {
            throw new FormatException(message: "SNAPSHOT_FIELDS: missing required property");
        }
    }

    /// <summary>Requires integer tokens without fractional/exponent conversion or precision loss.</summary>
    internal static long Integer(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Number || !long.TryParse(s: element.GetRawText(),
                style: NumberStyles.AllowLeadingSign, provider: CultureInfo.InvariantCulture, result: out long value))
        {
            throw new FormatException(message: "SNAPSHOT_INTEGER: exact signed Int64 token required");
        }

        return value;
    }

    /// <summary>Requires actual nonnull strings and rejects invalid escaped Unicode.</summary>
    internal static string Text(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.String)
        {
            throw new FormatException(message: "SNAPSHOT_TEXT: nonnull string required");
        }

        return element.GetString() ?? throw new FormatException(message: "SNAPSHOT_TEXT: null string");
    }

    /// <summary>Bounds arrays before allocation or dictionary/set conversion.</summary>
    internal static void Collection(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Array || element.GetArrayLength() > SnapshotCodec.MaxRecords)
        {
            throw new FormatException(message: "SNAPSHOT_COLLECTION: nonnull array of at most 1024 required");
        }
    }

    private static readonly UTF8Encoding _utf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    private static void ValidateNames(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (!names.Add(item: property.Name))
                {
                    throw new FormatException(message: "SNAPSHOT_DUPLICATE_PROPERTY: " + property.Name);
                }

                ValidateNames(element: property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in element.EnumerateArray())
            {
                ValidateNames(element: item);
            }
        }
    }

}
