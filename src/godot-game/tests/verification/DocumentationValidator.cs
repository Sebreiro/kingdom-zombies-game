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
        "src/godot-game/tools/README.md",
        "README.md", "tools/README.md", "src/godot-game/README.md", "src/godot-game/build/README.md",
        "docs/architecture/repository-layout.md", "docs/workflow/code-review.md",
        "AGENTS.md", "docs/code/README.md", "docs/architecture/README.md", "docs/code/review-checklist.md",
        "docs/architecture/decisions/0001-enforcement.md", "docs/architecture/decisions/0002-boundaries.md",
        "docs/architecture/decisions/0003-state-and-engine.md", "docs/workflow/setup.md",
        "docs/workflow/ai-collaboration.md", "docs/workflow/gsd.md", "docs/workflow/verification.md",
        "src/godot-game/core/docs/README.md", "src/godot-game/multiplayer-abstractions/docs/README.md",
        "src/godot-game/game/docs/README.md", "tools/code-rules/docs/README.md",
        "src/godot-game/tests/verification/docs/README.md",
        "docs/code/rule-matrix.md", "docs/code/review.md", "docs/architecture/boundaries.md",
        "docs/architecture/decisions/0001-development-baseline.md"
    ];

    /// <summary>Returns subject diagnostics without modifying documentation or the Git index.</summary>
    internal static IReadOnlyList<string> Check(IReadOnlyDictionary<string, byte[]> files,
            IReadOnlyList<string> required)
    {
        var errors = new List<string>();
        string[] paths = [.. files.Keys.Where(predicate: IsInventory)];
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
            else if (path is "AGENTS.md" or "docs/code/README.md" or "docs/architecture/README.md"
                    && !DocumentationLinks.Matches(source: DocumentationLinks.WithoutFences(content: content),
                            pattern: @"\[[^\]]+\](?:\([^)]+\)|\[[^\]]*\])").Any())
            {
                errors.Add(item: "DOC_INDEX " + path);
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
                && (path is "src/godot-game/tools/README.md" or "README.md" or "tools/README.md" or
                        "src/godot-game/README.md"
                        or "src/godot-game/build/README.md" or "AGENTS.md" or ".planning/PROJECT.md" or
                        ".planning/REQUIREMENTS.md"
                        or ".planning/ROADMAP.md" or ".planning/STATE.md" or ".planning/research/SUMMARY.md"
                        || IsTreeDocument(path: path));
    }

    /// <summary>Combines strict owned-document navigation with complete focused decision structure.</summary>
    internal static IReadOnlyList<string> CheckDecisions(IReadOnlyDictionary<string, byte[]> files)
    {
        return [.. Check(files: files, required: Authored), .. DocumentationDecisions.Check(files: files)];
    }

    /// <summary>Rejects installed, vendor, generated and provenance archive inputs from authored inventory.</summary>
    internal static bool IsInventory(string path)
    {
        string[] excluded =
        [
            ".codex", ".agents", ".claude", ".git", ".phase1", "vendor", "plugins",
            "node_modules", "bin", "obj", ".godot"
        ];
        return !path.Split(separator: '/').Any(predicate: part => excluded.Contains(value: part,
                comparer: StringComparer.OrdinalIgnoreCase)) && !path.EndsWith(value: ".zip",
                        comparisonType: StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Reads exact tracked paths plus explicitly declared authored pages; never stages any input.</summary>
    internal static Dictionary<string, byte[]> ReadRepository()
    {
        string[] helpers =
        [
            "src/godot-game/tests/verification/DocumentationLinks.cs",
            "src/godot-game/tests/verification/DocumentationPath.cs",
            "src/godot-game/tests/verification/DocumentationAnchors.cs",
            "src/godot-game/tests/verification/RuleBindingValidator.cs",
            "src/godot-game/tests/verification/RuleMatrix.cs",
            "src/godot-game/tests/verification/SuppressionPolicy.cs",
            "src/godot-game/tests/verification/SuppressionTests.cs",
            ".planning/phases/01-code-rules-and-documentation/01-HUMAN-CHECK.md"
        ];
        IEnumerable<string> authored = Authored.Concat(second: helpers);
        string[] paths = [..SourceFileFormat.ReadTrackedPaths().Concat(second: authored).Distinct(
                comparer: StringComparer.Ordinal)];
        return paths.Where(predicate: IsInventory).Where(predicate: path => File.Exists(path: Path.Combine(
                path1: ProofProtocol.Root,
                path2: path))).ToDictionary(keySelector: path => path, elementSelector: path => IsOwned(path: path)
                        ? File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root, path2: path)) : [],
                        comparer: StringComparer.Ordinal);
    }

    private static bool IsTreeDocument(string path)
    {
        return path.StartsWith(value: "docs/", comparisonType: StringComparison.Ordinal)
                && !path.StartsWith(value: "docs/code/provenance/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "specs/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "src/godot-game/core/docs/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "src/godot-game/multiplayer-abstractions/docs/", comparisonType:
                        StringComparison.Ordinal)
                || path.StartsWith(value: "src/godot-game/game/docs/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "src/godot-game/tests/verification/docs/", comparisonType:
                        StringComparison.Ordinal)
                || path.StartsWith(value: "tools/code-rules/docs/", comparisonType: StringComparison.Ordinal);
    }
}
