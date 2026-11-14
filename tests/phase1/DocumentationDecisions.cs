using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Checks required decision structure and exact linkage separately from qualitative accuracy review.</summary>
internal static class DocumentationDecisions
{
    /// <summary>Requires each owned decision to be complete and linked by the focused architecture index.</summary>
    internal static IReadOnlyList<string> Check(IReadOnlyDictionary<string, byte[]> files)
    {
        const string Index = "docs/architecture/README.md";
        var errors = new List<string>();
        string index = files.TryGetValue(key: Index, value: out byte[]? bytes)
                ? Encoding.UTF8.GetString(bytes: bytes) : "";
        IEnumerable<string> required = DocumentationValidator.Authored.Where(predicate: path => path.StartsWith(
                value: "docs/architecture/decisions/", comparisonType: StringComparison.Ordinal));
        IEnumerable<string> paths = required.Concat(second: files.Keys.Where(predicate: path => path.StartsWith(
                value: "docs/architecture/decisions/", comparisonType: StringComparison.Ordinal)
                && path.EndsWith(value: ".md", comparisonType: StringComparison.Ordinal))).Distinct(
                        comparer: StringComparer.Ordinal);
        string[] sections = ["context", "decision", "consequences"];
        foreach (string path in paths)
        {
            if (!files.TryGetValue(key: path, value: out byte[]? content))
            {
                errors.Add(item: "DOC_DECISION " + path + ":1 missing");
                continue;
            }

            string text = Encoding.UTF8.GetString(bytes: content);
            string relative = path["docs/architecture/".Length..];
            if (!index.Contains(value: "(" + relative + ")", comparisonType: StringComparison.Ordinal)
                    || !DocumentationLinks.Matches(source: text, pattern: @"^Status:\s*\S+").Any()
                    || sections.Any(predicate: anchor =>
                            DocumentationAnchors.Count(content: text, anchor: anchor) != 1))
            {
                errors.Add(item: "DOC_DECISION " + path + ":1 structure-or-link");
            }
        }

        return errors;
    }
}
