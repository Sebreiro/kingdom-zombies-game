using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Resolves the declared Markdown link convention to exact inventory paths and unique anchors.</summary>
internal static class DocumentationLinks
{
    /// <summary>Validates inline and explicit reference links outside fenced examples.</summary>
    internal static void Check(string path, string content, IReadOnlyDictionary<string, string> documents,
            IReadOnlyList<string> inventory, List<string> errors)
    {
        string source = WithoutFences(content: content);
        var definitions = new Dictionary<string, string>(comparer: StringComparer.Ordinal);
        foreach (Match definition in Matches(source: source, pattern: @"^ {0,3}\[(?<id>[^\]\r\n]+)\]:\s*(?<url>\S+)"))
        {
            if (!definitions.TryAdd(key: definition.Groups[groupname: "id"].Value,
                    value: definition.Groups[groupname: "url"].Value))
            {
                errors.Add(item: "DOC_REFERENCE " + path + ":" + Line(source: source, index: definition.Index));
            }
        }

        foreach (Match link in Matches(source: source,
                pattern: @"\[(?<text>[^\]\r\n]*)\]\((?<url><[^>\r\n]+>|[^)\r\n]+)\)"))
        {
            CheckTarget(path: path, line: Line(source: source, index: link.Index),
                    raw: link.Groups[groupname: "url"].Value.Trim().Trim(trimChars: ['<', '>']),
                    documents: documents, inventory: inventory, errors: errors);
        }

        foreach (Match link in Matches(source: source, pattern: @"\[(?<text>[^\]\r\n]+)\]\[(?<id>[^\]\r\n]*)\]"))
        {
            string id = link.Groups[groupname: "id"].Value;
            id = id.Length == 0 ? link.Groups[groupname: "text"].Value : id;
            int line = Line(source: source, index: link.Index);
            if (!definitions.TryGetValue(key: id, value: out string? target))
            {
                errors.Add(item: "DOC_REFERENCE " + path + ":" + line + " " + id);
                continue;
            }

            CheckTarget(path: path, line: line, raw: target, documents: documents, inventory: inventory,
                    errors: errors);
        }
    }

    /// <summary>Preserves line positions while excluding Markdown code fences from link interpretation.</summary>
    internal static string WithoutFences(string content)
    {
        string? fence = null;
        return string.Join(separator: "\n", values: content.Split(separator: '\n').Select(selector: line =>
        {
            string trimmed = line.TrimStart();
            if (trimmed.StartsWith(value: "```", comparisonType: StringComparison.Ordinal)
                    || trimmed.StartsWith(value: "~~~", comparisonType: StringComparison.Ordinal))
            {
                string marker = trimmed[..3];
                fence = fence is null ? marker : StringComparer.Ordinal.Equals(x: fence, y: marker) ? null : fence;
                return new string(c: ' ', count: line.Length);
            }

            return fence is null ? line : new string(c: ' ', count: line.Length);
        }));
    }

    /// <summary>Uses bounded culture-independent matching with named captures only.</summary>
    internal static IEnumerable<Match> Matches(string source, string pattern)
    {
        return Regex.Matches(input: source, pattern: pattern, options: RegexOptions.Multiline
                | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                matchTimeout: TimeSpan.FromSeconds(value: 1)).Cast<Match>();
    }

    private static int Line(string source, int index)
    {
        return source.AsSpan(start: 0, length: index).Count(value: '\n') + 1;
    }

    private static void CheckTarget(string path, int line, string raw,
            IReadOnlyDictionary<string, string> documents, IReadOnlyList<string> inventory, List<string> errors)
    {
        if (Matches(source: raw, pattern: @"^(?:https?|mailto|plugin|app)[:]").Any())
        {
            return;
        }

        string location = path + ":" + line;
        if (!DocumentationPath.TryResolve(source: path, raw: raw, target: out string target,
                anchor: out string anchor, diagnostic: out string diagnostic))
        {
            errors.Add(item: diagnostic + " " + location + " " + raw);
            return;
        }

        if (!inventory.Contains(value: target, comparer: StringComparer.Ordinal))
        {
            string code = inventory.Contains(value: target, comparer: StringComparer.OrdinalIgnoreCase)
                    ? "DOC_CASE" : "DOC_TARGET";
            errors.Add(item: code + " " + location + " " + target);
            return;
        }

        if (anchor.Length > 0 && (!documents.TryGetValue(key: target, value: out string? text)
                || DocumentationAnchors.Count(content: text, anchor: anchor) != 1))
        {
            errors.Add(item: "DOC_ANCHOR " + location + " " + target + "#" + anchor);
        }
    }
}
