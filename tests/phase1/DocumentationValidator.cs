using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Validates owned documentation against exact portable path and anchor identities.</summary>
internal static class DocumentationValidator
{
    /// <summary>Names required pages explicitly, including pages authored before their first commit.</summary>
    internal static IReadOnlyList<string> Authored =>
    [
        "AGENTS.md", "docs/code/README.md", "docs/architecture/README.md", "docs/code/review-checklist.md",
        "docs/architecture/decisions/0001-enforcement.md", "docs/architecture/decisions/0002-boundaries.md",
        "docs/architecture/decisions/0003-state-and-engine.md", "docs/workflow/setup.md",
        "docs/workflow/ai-collaboration.md", "docs/workflow/gsd.md", "docs/workflow/verification.md"
    ];

    /// <summary>Returns subject diagnostics without modifying documentation or the Git index.</summary>
    internal static IReadOnlyList<string> Check(IReadOnlyDictionary<string, byte[]> files,
            IReadOnlyList<string> required)
    {
        var errors = new List<string>();
        string[] paths = [.. files.Keys];
        string[] segments = [.. paths.SelectMany(selector: path => path.Split(separator: '/')
                .Select(selector: (_, index) => string.Join(separator: "/", values: path.Split(separator: '/')
                        .Take(count: index + 1)))).Distinct(comparer: StringComparer.Ordinal)];
        foreach (IGrouping<string, string> collision in segments.GroupBy(keySelector: path => path,
                comparer: StringComparer.OrdinalIgnoreCase).Where(predicate: group => group.Count() > 1))
        {
            errors.Add(item: "DOC_COLLISION " + string.Join(separator: " | ", values: collision.Order(
                    comparer: StringComparer.Ordinal)));
        }

        var documents = new Dictionary<string, string>(comparer: StringComparer.Ordinal);
        foreach (string path in paths.Where(predicate: IsOwned))
        {
            try
            {
                documents.Add(key: path, value: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false,
                        throwOnInvalidBytes: true).GetString(bytes: files[path]));
            }
            catch (DecoderFallbackException)
            {
                errors.Add(item: "DOC_UTF8 " + path);
            }
        }

        foreach (string path in required)
        {
            if (!documents.TryGetValue(key: path, value: out string? content))
            {
                errors.Add(item: "DOC_REQUIRED " + path);
            }
            else if (string.IsNullOrWhiteSpace(value: content))
            {
                errors.Add(item: "DOC_EMPTY " + path);
            }
        }

        foreach ((string path, string content) in documents)
        {
            DocumentationLinks.Check(path: path, content: content, documents: documents, inventory: paths,
                    errors: errors);
        }

        return errors.Order(comparer: StringComparer.Ordinal).ToArray();
    }

    /// <summary>Includes authored authorities; installed tools, provenance and generated outputs are excluded.
    /// </summary>
    internal static bool IsOwned(string path)
    {
        return path.EndsWith(value: ".md", comparisonType: StringComparison.Ordinal)
                && (path is "AGENTS.md" or ".planning/PROJECT.md" or ".planning/REQUIREMENTS.md"
                        or ".planning/ROADMAP.md" or ".planning/STATE.md" or ".planning/research/SUMMARY.md"
                        || IsTreeDocument(path: path));
    }

    /// <summary>Reads exact tracked paths plus explicitly declared authored pages; never stages any input.</summary>
    internal static IReadOnlyDictionary<string, byte[]> ReadRepository()
    {
        string[] helpers =
        [
            "tests/phase1/DocumentationLinks.cs", "tests/phase1/DocumentationPath.cs",
            "tests/phase1/DocumentationAnchors.cs"
        ];
        IEnumerable<string> authored = Authored.Concat(second: helpers);
        string[] paths = [..SourceFileFormat.ReadTrackedPaths().Concat(second: authored).Distinct(
                comparer: StringComparer.Ordinal)];
        return paths.Where(predicate: path => File.Exists(path: Path.Combine(path1: ProofProtocol.Root,
                path2: path))).ToDictionary(keySelector: path => path, elementSelector: path => IsOwned(path: path)
                        ? File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root, path2: path)) : [],
                        comparer: StringComparer.Ordinal);
    }

    private static bool IsTreeDocument(string path)
    {
        return path.StartsWith(value: "docs/", comparisonType: StringComparison.Ordinal)
                && !path.StartsWith(value: "docs/code/provenance/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "specs/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "src/", comparisonType: StringComparison.Ordinal)
                        && path.Contains(value: "/docs/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "tests/phase1/docs/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "tools/code-rules/docs/", comparisonType: StringComparison.Ordinal);
    }
}
