using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Decodes local URLs strictly without Unicode normalization or filesystem-dependent casing.</summary>
internal static class DocumentationPath
{
    /// <summary>Resolves local segments within the checkout root and preserves exact Unicode identities.</summary>
    internal static bool TryResolve(string source, string raw, out string target, out string anchor,
            out string diagnostic)
    {
        target = "";
        anchor = "";
        diagnostic = "DOC_ENCODING";
        string decoded;
        try
        {
            decoded = Decode(value: raw);
        }
        catch (FormatException)
        {
            return false;
        }
        catch (DecoderFallbackException)
        {
            return false;
        }

        diagnostic = "DOC_ESCAPE";
        if (decoded.Contains(value: '\\') || decoded.Contains(value: ':') || decoded.Contains(value: '\0')
                || decoded.Contains(value: '?') || decoded.StartsWith(value: "//", comparisonType:
                        StringComparison.Ordinal))
        {
            return false;
        }

        int hash = decoded.IndexOf(value: '#', comparisonType: StringComparison.Ordinal);
        anchor = hash < 0 ? "" : decoded[(hash + 1)..];
        string path = hash < 0 ? decoded : decoded[..hash];
        var parts = new List<string>();
        if (!path.StartsWith(value: '/') && path.Length > 0)
        {
            int slash = source.LastIndexOf(value: '/');
            parts.AddRange(collection: slash < 0 ? [] : source[..slash].Split(separator: '/'));
        }

        foreach (string part in path.Split(separator: '/'))
        {
            if (part is "" or ".")
            {
                continue;
            }

            if (StringComparer.Ordinal.Equals(x: part, y: ".."))
            {
                if (parts.Count == 0)
                {
                    return false;
                }

                parts.RemoveAt(index: parts.Count - 1);
            }
            else
            {
                parts.Add(item: part);
            }
        }

        target = path.Length == 0 ? source : string.Join(separator: "/", values: parts);
        return target.Length > 0;
    }

    private static string Decode(string value)
    {
        var result = new StringBuilder();
        for (int index = 0; index < value.Length; index++)
        {
            if (value[index] != '%')
            {
                result.Append(value: value[index]);
                continue;
            }

            var bytes = new List<byte>();
            while (index < value.Length && value[index] == '%')
            {
                if (index + 2 >= value.Length || !byte.TryParse(s: value.AsSpan(start: index + 1, length: 2),
                        style: NumberStyles.AllowHexSpecifier, provider: CultureInfo.InvariantCulture,
                        result: out byte next))
                {
                    throw new FormatException(message: "DOC_ENCODING");
                }

                bytes.Add(item: next);
                index += 3;
            }

            result.Append(value: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                    .GetString(bytes: [.. bytes]));
            index--;
        }

        return result.ToString();
    }
}
