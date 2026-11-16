using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Reconciles module declarations with the shared policy and actual evaluated reference metadata.</summary>
internal static class DocumentationProjects
{
    /// <summary>Checks five declarations and real index-to-policy navigation without changing inputs.</summary>
    internal static IReadOnlyList<string> Check(IReadOnlyDictionary<string, byte[]> files, string root)
    {
        var errors = new List<string>();
        JsonArray policy = RegisterValidator.Parse(bytes: File.ReadAllBytes(path: Path.Combine(path1: root,
                path2: Policy)))["projects"]?.AsArray()
                ?? throw new InvalidDataException(message: "DOC_PROJECT_POLICY");
        foreach (string project in ProjectEvaluator.Expected)
        {
            string page = project[..project.LastIndexOf(value: '/')] + "/docs/README.md";
            CheckPage(files: files, root: root, policy: policy, project: project, page: page, errors: errors);
        }

        return errors.Order(comparer: StringComparer.Ordinal).ToArray();
    }

    private const string Policy = "src/godot-game/tools/verification/projects.json";
    private const string Analyzer = "src/godot-game/tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj";
    private static void CheckPage(IReadOnlyDictionary<string, byte[]> files, string root, JsonArray policy,
            string project, string page, List<string> errors)
    {
        if (!files.TryGetValue(key: page, value: out byte[]? bytes) || bytes.Length is 0 or > 16384)
        {
            errors.Add(item: "DOC_PROJECT_REQUIRED " + page);
            return;
        }

        string text = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: bytes);
        if (!Reachable(files: files, source: "AGENTS.md", target: page)
                || !Reachable(files: files, source: page, target: Policy))
        {
            errors.Add(item: "DOC_PROJECT_UNREACHABLE " + page);
        }

        string[] declarations = [.. DocumentationLinks.Matches(source: text,
                pattern: @"^```project-policy\r?\n(?<json>[^\r\n]+)\r?\n```")
                .Select(selector: match => match.Groups[groupname: "json"].Value)];
        if (declarations.Length != 1)
        {
            errors.Add(item: "DOC_PROJECT_DECLARATION " + page);
            return;
        }

        JsonObject declaration = RegisterValidator.Parse(bytes: Encoding.UTF8.GetBytes(s: declarations[0]));
        JsonNode expected = policy.Single(predicate: row => StringComparer.Ordinal.Equals(
                x: row?["path"]?.GetValue<string>(), y: project))
                ?? throw new InvalidDataException(message: "DOC_PROJECT_POLICY");
        CheckDeclaration(declaration: declaration, expected: expected, project: project, page: page, errors: errors);
        CheckEvaluation(root: root, declaration: declaration, project: project, page: page, errors: errors);
    }

    private static void CheckDeclaration(JsonObject declaration, JsonNode expected, string project,
            string page, List<string> errors)
    {
        string[] production = Values(node: declaration["productionReferences"]);
        string[] inspection = Values(node: declaration["inspectionReferences"]);
        bool tests = StringComparer.Ordinal.Equals(x: project, y: ProjectEvaluator.Expected[4]);
        string[] allowed = Values(node: expected["allowed"]);
        if (declaration.Count != 5 || !StringComparer.Ordinal.Equals(x: declaration["project"]?.GetValue<string>(),
                y: project) || !StringComparer.Ordinal.Equals(x: declaration["policy"]?.GetValue<string>(), y: Policy)
                || !Same(first: tests ? inspection : production, second: allowed)
                || (tests ? production : inspection).Length != 0)
        {
            errors.Add(item: "DOC_PROJECT_DECLARATION " + page);
        }

        JsonNode? analyzer = declaration["analyzerReference"];
        if (StringComparer.Ordinal.Equals(x: project, y: Analyzer) ? analyzer is not null
                : analyzer is null || analyzer.AsObject().Count != 3 || !StringComparer.Ordinal.Equals(
                        x: analyzer["path"]?.GetValue<string>(), y: Analyzer)
                        || !StringComparer.Ordinal.Equals(x: analyzer["OutputItemType"]?.GetValue<string>(),
                                y: "Analyzer") || !StringComparer.Ordinal.Equals(
                                        x: analyzer["ReferenceOutputAssembly"]?.GetValue<string>(), y: "false"))
        {
            errors.Add(item: "DOC_PROJECT_ANALYZER " + page);
        }
    }

    private static void CheckEvaluation(string root, JsonObject declaration, string project, string page,
            List<string> errors)
    {
        (int Exit, string Output) result = GraphFixture.RunAt(root: root,
                arguments: ["msbuild", project, "-nologo", "-getItem:ProjectReference"]);
        if (result.Exit != 0)
        {
            throw new InvalidDataException(message: "DOC_PROJECT_INFRASTRUCTURE " + result.Output);
        }

        JsonArray references = RegisterValidator.Parse(bytes: Encoding.UTF8.GetBytes(s: result.Output))
                ["Items"]?["ProjectReference"]?.AsArray()
                ?? throw new InvalidDataException(message: "DOC_PROJECT_EVALUATION");
        var actual = new List<string>();
        int analyzerCount = 0;
        foreach (JsonNode? reference in references)
        {
            string path = Path.GetRelativePath(relativeTo: root, path: reference?["FullPath"]?.GetValue<string>()
                    ?? throw new InvalidDataException(message: "DOC_PROJECT_REFERENCE")).Replace(
                            oldChar: '\\', newChar: '/');
            bool analyzer = StringComparer.Ordinal.Equals(x: path, y: Analyzer);
            if (analyzer)
            {
                analyzerCount++;
                if (!StringComparer.Ordinal.Equals(x: reference?["OutputItemType"]?.GetValue<string>(), y: "Analyzer")
                        || !StringComparer.Ordinal.Equals(x: reference?["ReferenceOutputAssembly"]?.GetValue<string>(),
                                y: "false"))
                {
                    errors.Add(item: "DOC_PROJECT_METADATA " + page);
                }
            }
            else
            {
                actual.Add(item: path);
            }
        }

        string[] declared = [.. Values(node: declaration["productionReferences"]),
            .. Values(node: declaration["inspectionReferences"])];
        if (!Same(first: actual, second: declared)
                || analyzerCount != (declaration["analyzerReference"] is null ? 0 : 1))
        {
            errors.Add(item: "DOC_PROJECT_EDGES " + page);
        }
    }

    private static string[] Values(JsonNode? node)
    {
        return node is JsonArray values ? [.. values.Select(selector: item => item?.GetValue<string>() ?? "")]
                : ["INVALID_ARRAY"];
    }

    private static bool Same(IEnumerable<string> first, IEnumerable<string> second)
    {
        string[] actual = [.. first];
        return actual.Distinct(comparer: StringComparer.Ordinal).Count() == actual.Length
                && actual.Order(comparer: StringComparer.Ordinal).SequenceEqual(
                        second: second.Order(comparer: StringComparer.Ordinal), comparer: StringComparer.Ordinal);
    }

    private static bool Reachable(IReadOnlyDictionary<string, byte[]> files, string source, string target)
    {
        var visited = new HashSet<string>(comparer: StringComparer.Ordinal);
        var pending = new Queue<string>();
        pending.Enqueue(item: source);
        while (pending.TryDequeue(result: out string? current))
        {
            if (StringComparer.Ordinal.Equals(x: current, y: target))
            {
                return true;
            }

            if (!visited.Add(item: current) || !files.TryGetValue(key: current, value: out byte[]? content))
            {
                continue;
            }

            string text = DocumentationLinks.WithoutFences(content: Encoding.UTF8.GetString(bytes: content));
            foreach (System.Text.RegularExpressions.Match link in DocumentationLinks.Matches(source: text,
                    pattern: @"\[[^\]]+\]\((?<url>[^)]+)\)"))
            {
                if (DocumentationPath.TryResolve(source: current, raw: link.Groups[groupname: "url"].Value,
                        target: out string path, anchor: out _, diagnostic: out _))
                {
                    pending.Enqueue(item: path);
                }
            }
        }

        return false;
    }
}
