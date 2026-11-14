using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Checks the evaluated project boundary independently of compiler success.</summary>
internal static class ProjectEvaluator
{
    /// <summary>Lists the independent expected repository projects, including legitimate zero-edge subjects.</summary>
    internal static IReadOnlyList<string> Expected => _expected;

    /// <summary>Returns actionable boundary violations for an actual project closure.</summary>
    internal static IReadOnlyList<string> Validate(string root)
    {
        var errors = new List<string>();
        IReadOnlyDictionary<string, string[]> policy = ReadPolicy(root: root, errors: errors);
        CheckInventory(root: root, policy: policy, errors: errors);
        var visited = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (string project in policy.Keys.Order(comparer: StringComparer.Ordinal))
        {
            Visit(root: root, project: project, policy: policy, visited: visited, errors: errors);
        }

        return errors.Order(comparer: StringComparer.Ordinal).ToArray();
    }

    /// <summary>Checks an isolated provider's declared ports-only inspection boundary.</summary>
    internal static IReadOnlyList<string> ValidateProvider(string root, string project, bool reusable = false)
    {
        var errors = new List<string>();
        var policy = new Dictionary<string, string[]>(comparer: StringComparer.Ordinal)
        {
            [project] = reusable ? [] : [_expected[1]],
            [_expected[1]] = [],
            [Analyzer] = []
        };
        Visit(root: root, project: project, policy: policy,
                visited: new HashSet<string>(comparer: StringComparer.Ordinal), errors: errors);
        if (reusable)
        {
            (int Exit, string Output) query = GraphFixture.RunAt(root: root,
                    arguments: ["msbuild", project, "-nologo", "-getItem:PackageReference"]);
            if (query.Exit != 0)
            {
                throw new InvalidDataException(message: "GRAPH_INFRASTRUCTURE: " + query.Output);
            }

            JsonObject evaluation = JsonNode.Parse(json: query.Output)?.AsObject()
                    ?? throw new InvalidDataException(message: "GRAPH_INFRASTRUCTURE: missing package evaluation");
            CheckPackages(project: project, evaluation: evaluation, errors: errors, productAgnostic: true);
        }

        return errors.Order(comparer: StringComparer.Ordinal).ToArray();
    }

    /// <summary>Unions tracked and newly authored files, with a scoped isolated-fixture fallback.</summary>
    internal static IEnumerable<string> DiscoverFiles(string root, string pattern)
    {
        return Discover(root: root, pattern: pattern);
    }

    private const string Analyzer = "tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj";
    private static readonly string[] _expected = ["src/core/KingdomZombies.Core.csproj",
        "src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj",
        "src/godot-game/KingdomZombies.csproj", "tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj",
        "tests/phase1/KingdomZombies.Phase1.Tests.csproj"];
    private static readonly string[] _types = ["KingdomZombies.Core.ProofValue",
        "KingdomZombies.Multiplayer.Abstractions.IProofService", "KingdomZombies.Proofs.Phase1",
        "Sebreiro.CodeRules.Analyzers.NamedArgumentAnalyzer", "KingdomZombies.Phase1.Tests.GraphTests"];
    private static Dictionary<string, string[]> ReadPolicy(string root, List<string> errors)
    {
        string path = Path.Combine(path1: root, path2: "tools/phase1/projects.json");
        if (new FileInfo(fileName: path).Length is 0 or > 32768)
        {
            throw new InvalidDataException(message: "GRAPH_POLICY: bounded nonempty policy required " + path);
        }

        JsonObject document = JsonNode.Parse(json: File.ReadAllText(path: path))?.AsObject()
                ?? throw new InvalidDataException(message: "GRAPH_POLICY: missing object " + path);
        if (document.Count != 2 || document["schemaVersion"]?.GetValue<int>() != 1)
        {
            errors.Add(item: "GRAPH_POLICY: unexpected schema " + path);
        }

        var result = new Dictionary<string, string[]>(comparer: StringComparer.Ordinal);
        foreach (JsonNode? item in document["projects"]?.AsArray()
                ?? throw new InvalidDataException(message: "GRAPH_POLICY: projects absent"))
        {
            string project = item?["path"]?.GetValue<string>() ?? "";
            if (item?["allowed"] is not JsonArray allowed || !ValidPath(path: project))
            {
                errors.Add(item: "GRAPH_SELECTOR: missing path/allowed selector " + project);
                continue;
            }

            string[] edges = [.. allowed.Select(selector: edge => edge?.GetValue<string>() ?? "")];
            if (edges.Any(predicate: edge => !ValidPath(path: edge)))
            {
                errors.Add(item: "GRAPH_SELECTOR: empty or invalid edge " + project);
                continue;
            }
            if (!result.TryAdd(key: project, value: edges) || edges.Distinct(comparer: StringComparer.Ordinal)
                    .Count() != edges.Length)
            {
                errors.Add(item: "GRAPH_DUPLICATE: policy " + project);
            }

            string[] expected = StringComparer.Ordinal.Equals(x: project, y: _expected[2])
                    ? [_expected[0], _expected[1]] : StringComparer.Ordinal.Equals(x: project, y: _expected[4])
                            ? [_expected[0]] : [];
            if (item?.AsObject().Count != 3 || !edges.Order(comparer: StringComparer.Ordinal).SequenceEqual(
                    second: expected.Order(comparer: StringComparer.Ordinal), comparer: StringComparer.Ordinal))
            {
                errors.Add(item: "GRAPH_POLICY: forbidden permission " + project);
            }
        }

        CheckTypeSelectors(document: document, errors: errors);

        if (!result.Keys.Order(comparer: StringComparer.Ordinal).SequenceEqual(second: _expected.Order(
                comparer: StringComparer.Ordinal), comparer: StringComparer.Ordinal))
        {
            errors.Add(item: "GRAPH_EXPECTED: exact five project subjects required");
        }

        return result;
    }

    private static void CheckTypeSelectors(JsonObject document, List<string> errors)
    {
        foreach (JsonNode? item in document["projects"]?.AsArray() ?? [])
        {
            string path = item?["path"]?.GetValue<string>() ?? "";
            int index = Array.IndexOf(array: _expected, value: path);
            if (index < 0 || item?["expectedTypes"] is not JsonArray types || types.Count != 1
                    || !StringComparer.Ordinal.Equals(x: types[0]?.GetValue<string>(), y: _types[index]))
            {
                errors.Add(item: "TYPE_SELECTOR: exact independent expected type required " + path);
            }
        }
    }

    private static void CheckInventory(string root, IReadOnlyDictionary<string, string[]> policy, List<string> errors)
    {
        string solution = Path.Combine(path1: root, path2: "KingdomZombies.slnx");
        string[] paths = [..XDocument.Load(uri: solution).Descendants(name: "Project").Select(selector: element =>
                element.Attribute(name: "Path")?.Value ?? "")];
        if (paths.Distinct(comparer: StringComparer.Ordinal).Count() != paths.Length)
        {
            errors.Add(item: "GRAPH_DUPLICATE: solution Project");
        }

        foreach (string project in policy.Keys)
        {
            if (!paths.Contains(value: project, comparer: StringComparer.Ordinal) || !File.Exists(path:
                    Path.Combine(path1: root, path2: project)))
            {
                errors.Add(item: "GRAPH_MISSING: " + project);
            }
        }

        foreach (string project in paths.Concat(second: DiscoverProjects(root: root))
                .Distinct(comparer: StringComparer.Ordinal))
        {
            if (!policy.ContainsKey(key: project))
            {
                errors.Add(item: "GRAPH_UNDECLARED: " + project);
            }
        }
    }

    private static IEnumerable<string> Discover(string root, string pattern)
    {
        if (Directory.Exists(path: Path.Combine(path1: root, path2: ".git"))
                || File.Exists(path: Path.Combine(path1: root, path2: ".git")))
        {
            (int Exit, string Output) tracked = GraphFixture.RunAt(root: root, executable: "git",
                    arguments: ["ls-files", "--cached", "--others", "--exclude-standard", "--", pattern]);
            if (tracked.Exit != 0)
            {
                throw new InvalidDataException(message: "GRAPH_INFRASTRUCTURE: inventory " + tracked.Output);
            }

            return tracked.Output.Split(separator: '\n', options: StringSplitOptions.RemoveEmptyEntries)
                    .Select(selector: path => path.TrimEnd(trimChar: '\r'));
        }

        return Directory.EnumerateFiles(path: root, searchPattern: pattern,
                searchOption: SearchOption.AllDirectories)
                .Select(selector: path => Relative(root: root, path: path)).Where(predicate: path => !path.Split(
                        separator: '/').Any(predicate: part => part is "obj" or "bin" or ".godot" or ".phase1"
                                or ".codex" or ".agents" or ".git"));
    }

    private static IEnumerable<string> DiscoverProjects(string root)
    {
        return DiscoverFiles(root: root, pattern: "*.csproj");
    }

    private static bool ValidPath(string path)
    {
        return !string.IsNullOrWhiteSpace(value: path) && !Path.IsPathRooted(path: path)
                && !path.Contains(value: '\\') && !path.Split(separator: '/').Any(predicate: part =>
                        part is "" or "." or "..");
    }

    private static void Visit(string root, string project, IReadOnlyDictionary<string, string[]> policy,
            HashSet<string> visited, List<string> errors)
    {
        if (!visited.Add(item: project))
        {
            return;
        }

        string path = Path.Combine(path1: root, path2: project);
        if (!File.Exists(path: path))
        {
            errors.Add(item: "GRAPH_MISSING: " + project);
            return;
        }

        (int Exit, string Output) query = GraphFixture.RunAt(root: root, arguments: ["msbuild", path, "-nologo",
            "-getItem:ProjectReference,PackageReference",
            "-getProperty:TargetFramework,AssemblyName,TargetPath,OutputType"]);
        if (query.Exit != 0)
        {
            throw new InvalidDataException(message: "GRAPH_INFRASTRUCTURE: " + project + " " + query.Output);
        }

        JsonObject evaluation = JsonNode.Parse(json: query.Output)?.AsObject()
                ?? throw new InvalidDataException(message: "GRAPH_INFRASTRUCTURE: missing evaluation");
        string expectedOutput = StringComparer.Ordinal.Equals(x: project, y: _expected[4]) ? "Exe" : "Library";
        if (!StringComparer.Ordinal.Equals(x: evaluation["Properties"]?["OutputType"]?.GetValue<string>(),
                y: expectedOutput))
        {
            errors.Add(item: "GRAPH_OUTPUT_KIND: " + project + " requires " + expectedOutput);
        }

        var seen = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonNode? reference in evaluation["Items"]?["ProjectReference"]?.AsArray() ?? [])
        {
            CheckReference(root: root, project: project, reference: reference
                    ?? throw new InvalidDataException(message: "GRAPH_METADATA: null reference"), policy: policy,
                    visited: visited, seen: seen, errors: errors);
        }

        CheckPackages(project: project, evaluation: evaluation, errors: errors);
    }

    private static void CheckReference(string root, string project, JsonNode reference,
            IReadOnlyDictionary<string, string[]> policy, HashSet<string> visited, HashSet<string> seen,
            List<string> errors)
    {
        string full = reference["FullPath"]?.GetValue<string>() ?? "";
        string origin = reference["DefiningProjectFullPath"]?.GetValue<string>() ?? "";
        if (string.IsNullOrEmpty(value: full) || string.IsNullOrEmpty(value: origin))
        {
            errors.Add(item: "GRAPH_METADATA: " + project + " missing FullPath/DefiningProjectFullPath");
            return;
        }

        string target = Relative(root: root, path: full);
        string source = project + " -> " + target + " defined by " + Relative(root: root, path: origin);
        if (!seen.Add(item: target))
        {
            errors.Add(item: "GRAPH_DUPLICATE: " + source);
        }

        bool analyzer = StringComparer.Ordinal.Equals(x: target, y: Analyzer)
                && StringComparer.Ordinal.Equals(x: reference["OutputItemType"]?.GetValue<string>(), y: "Analyzer")
                && StringComparer.OrdinalIgnoreCase.Equals(x: reference["ReferenceOutputAssembly"]?.GetValue<string>(),
                        y: "false");
        if (!analyzer && (!policy.TryGetValue(key: project, value: out string[]? allowed)
                || !allowed.Contains(value: target, comparer: StringComparer.Ordinal)))
        {
            errors.Add(item: "GRAPH_FORBIDDEN: " + source);
        }

        if (!policy.ContainsKey(key: target))
        {
            errors.Add(item: "GRAPH_UNDECLARED: " + source);
        }

        Visit(root: root, project: target, policy: policy, visited: visited, errors: errors);
    }

    private static void CheckPackages(string project, JsonObject evaluation, List<string> errors,
            bool productAgnostic = false)
    {
        var seen = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonNode? package in evaluation["Items"]?["PackageReference"]?.AsArray() ?? [])
        {
            string identity = package?["Identity"]?.GetValue<string>() ?? "";
            if ((productAgnostic || StringComparer.Ordinal.Equals(x: project, y: Analyzer))
                    && identity.Contains(value: "KingdomZombies",
                            comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(item: "GRAPH_PACKAGE: reusable analyzer product dependency " + identity);
            }

            if (!seen.Add(item: identity))
            {
                errors.Add(item: "GRAPH_DUPLICATE: " + project + " package " + identity);
            }

            bool pure = StringComparer.Ordinal.Equals(x: project, y: _expected[0])
                    || StringComparer.Ordinal.Equals(x: project, y: _expected[1]);
            if (pure && identity is not ("Meziantou.Analyzer" or "Roslynator.Analyzers"
                    or "Microsoft.CodeAnalysis.BannedApiAnalyzers"))
            {
                errors.Add(item: "GRAPH_PACKAGE: " + project + " -> " + identity + " defined by "
                        + package?["DefiningProjectFullPath"]?.GetValue<string>());
            }
        }
    }

    private static string Relative(string root, string path)
    {
        return Path.GetRelativePath(relativeTo: root, path: path).Replace(oldChar: '\\', newChar: '/');
    }
}
