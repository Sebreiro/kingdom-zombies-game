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

    private const string Analyzer = "tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj";
    private static readonly string[] _expected = ["src/core/KingdomZombies.Core.csproj",
        "src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj",
        "src/godot-game/KingdomZombies.csproj", "tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj",
        "tests/phase1/KingdomZombies.Phase1.Tests.csproj"];
    private static readonly string[] _directories = ["src", "tests", "tools"];
    private static Dictionary<string, string[]> ReadPolicy(string root, List<string> errors)
    {
        string path = Path.Combine(path1: root, path2: "tools/phase1/projects.json");
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
            string[] edges = item?["allowed"]?.AsArray().Select(selector: edge => edge?.GetValue<string>() ?? "")
                    .ToArray() ?? [];
            if (!result.TryAdd(key: project, value: edges) || edges.Distinct(comparer: StringComparer.Ordinal)
                    .Count() != edges.Length)
            {
                errors.Add(item: "GRAPH_DUPLICATE: policy " + project);
            }

            string[] expected = StringComparer.Ordinal.Equals(x: project, y: _expected[2])
                    ? [_expected[0], _expected[1]] : [];
            if (item?.AsObject().Count != 2 || !edges.Order(comparer: StringComparer.Ordinal).SequenceEqual(
                    second: expected.Order(comparer: StringComparer.Ordinal), comparer: StringComparer.Ordinal))
            {
                errors.Add(item: "GRAPH_POLICY: forbidden permission " + project);
            }
        }

        if (!result.Keys.Order(comparer: StringComparer.Ordinal).SequenceEqual(second: _expected.Order(
                comparer: StringComparer.Ordinal), comparer: StringComparer.Ordinal))
        {
            errors.Add(item: "GRAPH_EXPECTED: exact five project subjects required");
        }

        return result;
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

        IEnumerable<string> authored = _directories.SelectMany(selector: directory =>
                Directory.EnumerateFiles(path: Path.Combine(path1: root, path2: directory), searchPattern: "*.csproj",
                        searchOption: SearchOption.AllDirectories)).Where(predicate: path => !path.Split(separator:
                                Path.DirectorySeparatorChar).Any(predicate: part => part is "obj" or "bin"));
        foreach (string project in paths.Concat(second: authored.Select(selector: path => Relative(root: root,
                path: path))).Distinct(comparer: StringComparer.Ordinal))
        {
            if (!policy.ContainsKey(key: project))
            {
                errors.Add(item: "GRAPH_UNDECLARED: " + project);
            }
        }
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
            "-getItem:ProjectReference,PackageReference", "-getProperty:TargetFramework,AssemblyName,TargetPath"]);
        if (query.Exit != 0)
        {
            throw new InvalidDataException(message: "GRAPH_INFRASTRUCTURE: " + project + " " + query.Output);
        }

        JsonObject evaluation = JsonNode.Parse(json: query.Output)?.AsObject()
                ?? throw new InvalidDataException(message: "GRAPH_INFRASTRUCTURE: missing evaluation");
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

    private static void CheckPackages(string project, JsonObject evaluation, List<string> errors)
    {
        var seen = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonNode? package in evaluation["Items"]?["PackageReference"]?.AsArray() ?? [])
        {
            string identity = package?["Identity"]?.GetValue<string>() ?? "";
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
