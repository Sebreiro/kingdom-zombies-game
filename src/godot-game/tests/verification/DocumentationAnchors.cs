using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Resolves explicit IDs and heading slugs; duplicate implicit slugs require unique explicit IDs.</summary>
internal static class DocumentationAnchors
{
    /// <summary>Counts all declarations of an anchor, rejecting ambiguity instead of selecting a first match.</summary>
    internal static int Count(string content, string anchor)
    {
        string source = DocumentationLinks.WithoutFences(content: content);
        int explicitCount = DocumentationLinks.Matches(source: source,
                pattern: "<a\\s+id=[\"'](?<id>[^\"']+)[\"']\\s*></a>").Count(predicate: match =>
                        StringComparer.Ordinal.Equals(x: match.Groups[groupname: "id"].Value, y: anchor));
        int headings = DocumentationLinks.Matches(source: source, pattern: @"^ {0,3}#{1,6}\s+(?<title>[^\r\n]+)")
                .Count(predicate: match => StringComparer.Ordinal.Equals(x: Slug(title:
                        match.Groups[groupname: "title"].Value), y: anchor));
        return explicitCount + headings;
    }

    private static string Slug(string title)
    {
        string clean = Regex.Replace(input: title.Trim().TrimEnd(trimChars: '#').Trim().ToLowerInvariant(),
                pattern: @"[^\p{L}\p{M}\p{Nd}_\s-]", replacement: "", options: RegexOptions.CultureInvariant,
                matchTimeout: TimeSpan.FromSeconds(value: 1));
        return Regex.Replace(input: clean, pattern: @"\s", replacement: "-", options: RegexOptions.CultureInvariant,
                matchTimeout: TimeSpan.FromSeconds(value: 1));
    }
}
