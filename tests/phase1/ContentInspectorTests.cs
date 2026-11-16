using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises the real portable pack loader independently of a native game window.</summary>
public sealed class ContentInspectorTests
{
    /// <summary>Every actual inspection member needs its own synchronous captured guard and exact signature.</summary>
    [Fact]
    public void Authorize_Inspection_RequiresExactInvocationTuples()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: InspectionPath));
        EngineAccessPolicy.Check(source: source, path: InspectionPath, consumer: consumer).ShouldBeEmpty();
        string[] signatures = ["InspectionRoot()", "_Ready()", "_Process(double)", "_ExitTree()", "Recheck()",
            "Cancel()", "Read(string)", "ChangeCategory(long)", "ChangeStatus(long)", "Filter(string)",
            "Select(long)", "Details()", "Copy()", "Project()", "Rows()", "Selection()"];
        EngineAccessPolicy.InspectionTuples.ShouldBe(expected: signatures.Select(selector: signature =>
                InspectionPath + "|KingdomZombies.Inspection|InspectionRoot|" + signature));
        SyntaxTree tree = consumer.Parse(source: source, path: InspectionPath);
        BaseMethodDeclarationSyntax[] methods = [.. tree.GetRoot(cancellationToken:
                TestContext.Current.CancellationToken).DescendantNodes()
                .OfType<BaseMethodDeclarationSyntax>()];
        methods.Length.ShouldBe(expected: signatures.Length);
        foreach (BaseMethodDeclarationSyntax method in methods)
        {
            CheckInvocation(consumer: consumer, source: source, method: method);
        }

        CheckOwnership(consumer: consumer, source: source);
    }

    /// <summary>Only the actual cross-assembly immutable contract and engine scene script are public.</summary>
    [Fact]
    public void Authorize_PublicContracts_RejectsAdjacentTypesAndPaths()
    {
        foreach ((string path, string type, int project) in new[]
        {
            ("src/core/ContentPack.cs", "ContentPack", 0),
            (InspectionPath, "InspectionRoot", 2),
        })
        {
            var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root,
                    project: ProjectEvaluator.Expected[project]);
            string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: path));
            ConsumerCompilation.RequireValid(compilation: consumer.Create(source: source, path: path));
            ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: source, path: path,
                    options: consumer.Options).ShouldBeEmpty();
            string adjacent = source + "\npublic sealed class " + type + "Adjacent { }\n";
            ConsumerCompilation.RequireValid(compilation: consumer.Create(source: adjacent, path: path));
            ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: adjacent, path: path,
                    options: consumer.Options).Single().ShouldContain(expected: "R-34: undeclared public type");
            ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: source,
                    path: path.Replace(oldValue: type + ".cs", newValue: "AdjacentInspection.cs",
                            comparisonType: StringComparison.Ordinal), options: consumer.Options).ShouldNotBeEmpty();
            CheckVisibilityRegistry(consumer: consumer, source: source, path: path, type: type);
        }
    }

    /// <summary>A missing independent shard cannot hide the exact valid bow or independent census.</summary>
    [Fact]
    public void Load_PartialPack_PreservesBowAndCensus()
    {
        Type? type = typeof(ProofValue).Assembly.GetType(name: "KingdomZombies.Core.ContentPack");
        MethodInfo? method = type?.GetMethod(name: "Load");
        Func<string, byte[]?> read = path => File.Exists(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/" + path)) ? File.ReadAllBytes(path: Path.Combine(
                        path1: ProofProtocol.Root, path2: "src/godot-game/" + path)) : null;
        object? pack = method?.Invoke(obj: null, parameters:
        [
            read(arg: "content/pack-index.json"), read(arg: "content/expected-obligations.json"), read,
        ]);
        object? entries = type?.GetProperty(name: "Entries")?.GetValue(obj: pack);
        (entries is IDictionary map && map.Contains(key: "common.bow")).ShouldBeTrue(
                customMessage: "A valid real bow remains inspectable despite 28 missing independent shards");
        object? expected = type?.GetProperty(name: "Expected")?.GetValue(obj: pack);
        (expected is IDictionary census ? census.Count : 0).ShouldBe(expected: 180);
    }

    /// <summary>Reordered missing/invalid shards yield exactly the same detached failures and valid output.</summary>
    [Fact]
    public void Load_ReorderedShards_PreservesIndependentDiagnostics()
    {
        JsonObject index = Index();
        ContentPack first = Load(index: index);
        JsonArray shards = index["shards"]?.AsArray() ?? throw new InvalidOperationException();
        JsonNode?[] reversed = [.. shards.Reverse().Select(selector: row => row?.DeepClone())];
        index["shards"] = new JsonArray(items: reversed);
        ContentPack second = Load(index: index);
        second.Entries.Keys.ShouldBe(expected: first.Entries.Keys);
        second.Diagnostics.ShouldBe(expected: first.Diagnostics);
        first.Expected["IT-07"].ShouldContain(expected: "Bow");
        first.Status.ShouldContain(expected: "incomplete");
        index.Remove(propertyName: "expectedObligations");
        Load(index: index).Entries.ContainsKey(key: "common.bow").ShouldBeTrue();
    }

    /// <summary>Unauthenticated expectations cannot redefine the census, even with a new index hash.</summary>
    [Theory]
    [InlineData("missing")]
    [InlineData("duplicate")]
    [InlineData("unknown")]
    [InlineData("case")]
    [InlineData("bytes")]
    [InlineData("malformed-utf8")]
    public void Load_BadExpectations_PreservesCensusAndBow(string mutation)
    {
        JsonObject index = Index();
        byte[]? expectations = Read(path: ContentPack.ExpectedPath);
        JsonObject expected = JsonNode.Parse(utf8Json: expectations ?? [])?.AsObject()
                ?? throw new InvalidOperationException();
        JsonArray rows = expected["obligations"]?.AsArray() ?? throw new InvalidOperationException();
        if (mutation is "duplicate" or "unknown" or "case")
        {
            rows[0]?["id"] = mutation switch { "duplicate" => "CH-02", "case" => "ch-01", _ => "CH-99" };
            expectations = Encoding.UTF8.GetBytes(s: expected.ToJsonString());
        }
        else
        {
            expectations = mutation switch
            {
                "missing" => null,
                "malformed-utf8" => [0xff, 0xfe],
                _ => [.. expectations ?? [], 32],
            };
        }

        index["expectedObligations"]?["sha256"] = expectations is null ? "missing"
                : Convert.ToHexStringLower(inArray: SHA256.HashData(source: expectations));
        ContentPack pack = Load(index: index, expectations: expectations, replaceExpectations: true);
        pack.Expected.Count.ShouldBe(expected: 180);
        pack.Expected["CH-01"].ShouldContain(expected: "Unavailable metadata");
        pack.Entries.ContainsKey(key: "common.bow").ShouldBeTrue();
        pack.Diagnostics.Any(predicate: error => error.Contains(value:
                "res://content/expected-obligations.json", comparisonType: StringComparison.Ordinal)).ShouldBeTrue();
    }

    /// <summary>Malformed indexes retain errors without crashing or trusting derived expectations.</summary>
    [Theory]
    [InlineData("{\"id\":1,\"id\":2}")]
    [InlineData("{\"schemaVersion\":\"one\"}")]
    [InlineData("{\"number\":1e999}")]
    [InlineData("[[[[[[[[[[[[[[[[[[[]]]]]]]]]]]]]]]]]]]")]
    public void Load_MalformedIndex_ReturnsExactFailure(string source)
    {
        var pack = ContentPack.Load(index: Encoding.UTF8.GetBytes(s: source),
                expectations: Read(path: ContentPack.ExpectedPath), read: Read);
        pack.Entries.ShouldBeEmpty();
        pack.Expected.Count.ShouldBe(expected: 180);
        pack.Diagnostics.Any(predicate: error => error.StartsWith(value:
                "pack: res://content/pack-index.json", comparisonType: StringComparison.Ordinal)).ShouldBeTrue();
    }

    /// <summary>Exact path rules reject aliases, escapes and absolute/recovery/source locators.</summary>
    [Theory]
    [InlineData("assets/bow.png")]
    [InlineData("src/godot-game/../bow.png")]
    [InlineData("src/godot-game/assets//bow.png")]
    [InlineData("src/godot-game/Assets/bow.png")]
    [InlineData("src/godot-game/assets/bow.png.")]
    [InlineData("src/godot-game/assets/%62ow.png")]
    [InlineData("D:/recovery/bow.png")]
    [InlineData("src/godot-game/assets\\bow.png")]
    public void ResourcePath_Aliases_Reject(string path)
    {
        Should.Throw<FormatException>(actual: () => ContentPack.ResourcePath(path: path));
    }

    /// <summary>Missing output media invalidates only its own definition; recheck creates a fresh snapshot.</summary>
    [Fact]
    public void Load_MissingMediaAndRecheck_DetachesPublishedState()
    {
        JsonObject index = Index();
        ContentPack valid = Load(index: index);
        var missing = ContentPack.Load(index: Encoding.UTF8.GetBytes(s: index.ToJsonString()),
                expectations: Read(path: ContentPack.ExpectedPath),
                read: path => path.EndsWith(value: ".png", comparisonType: StringComparison.Ordinal)
                        ? null : Read(path: path));
        missing.Entries.ShouldBeEmpty();
        missing.Expected.Count.ShouldBe(expected: 180);
        valid.Entries.Count.ShouldBe(expected: 1);
        ContentPack restored = Load(index: index);
        restored.Entries.Keys.ShouldBe(expected: valid.Entries.Keys);
        Should.Throw<NotSupportedException>(actual: () => ((IDictionary)restored.Expected).Clear());
        ContentPack.Load(index: new byte[ContentPack.MaxDocumentBytes + 1], expectations: null, read: Read)
                .Entries.ShouldBeEmpty();
    }

    /// <summary>Equal display names and explicit shared bytes never collapse distinct output IDs.</summary>
    [Fact]
    public void Load_DuplicateNames_PreservesIdsAndSearchCounts()
    {
        JsonObject index = Index();
        JsonArray outputs = index["outputs"]?.AsArray() ?? throw new InvalidOperationException();
        JsonNode second = outputs[0]?.DeepClone() ?? throw new InvalidOperationException();
        second["id"] = "common.bow.other";
        outputs.Add(item: second);
        JsonObject shard = JsonNode.Parse(utf8Json: Read(path: "content/packs/tracer.json") ?? [])?.AsObject()
                ?? throw new InvalidOperationException();
        JsonArray resources = shard["resources"]?.AsArray() ?? throw new InvalidOperationException();
        JsonNode resource = resources[0]?.DeepClone() ?? throw new InvalidOperationException();
        resource["id"] = "common.bow.other";
        resources.Add(item: resource);
        byte[] shardBytes = Encoding.UTF8.GetBytes(s: shard.ToJsonString());
        JsonNode declared = index["shards"]?[0] ?? throw new InvalidOperationException();
        declared["bytes"] = shardBytes.Length;
        declared["sha256"] = Convert.ToHexStringLower(inArray: SHA256.HashData(source: shardBytes));
        var pack = ContentPack.Load(index: Encoding.UTF8.GetBytes(s: index.ToJsonString()),
                expectations: Read(path: ContentPack.ExpectedPath),
                read: path => StringComparer.Ordinal.Equals(x: path, y: "content/packs/tracer.json")
                        ? shardBytes : Read(path: path));
        pack.Entries.Count.ShouldBe(expected: 2);
        pack.Search(query: "bow", category: "Items", status: "Partial").Count.ShouldBe(expected: 2);
        pack.Search(query: "common.bow.other", category: "Items", status: "Partial").Count.ShouldBe(expected: 1);
        pack.Search(query: "no-match", category: "Items", status: "Partial").ShouldBeEmpty();
        ContentPack.Mappings(entry: pack.Entries["common.bow.other"]).ShouldBe(expected: ["IT-07"]);
    }

    /// <summary>Invalid independent entries produce failures while retaining the valid bow.</summary>
    [Theory]
    [InlineData("duplicate")]
    [InlineData("unknown-mapping")]
    [InlineData("bad-path")]
    [InlineData("missing-id")]
    public void Load_InvalidIndependentOutput_RetainsValidBow(string mutation)
    {
        JsonObject index = Index();
        JsonArray outputs = index["outputs"]?.AsArray() ?? throw new InvalidOperationException();
        JsonNode row = outputs[0]?.DeepClone() ?? throw new InvalidOperationException();
        row["id"] = "bad.independent";
        if (mutation is "unknown-mapping")
        {
            row["obligations"]?[0]?["id"] = "IT-99";
        }
        else if (mutation is "bad-path")
        {
            row["path"] = "src/godot-game/../escape.png";
        }
        else if (mutation is "missing-id")
        {
            row.AsObject().Remove(propertyName: "id");
        }

        outputs.Add(item: row);
        if (mutation is "duplicate")
        {
            outputs.Add(item: row.DeepClone());
        }

        ContentPack pack = Load(index: index);
        pack.Entries.Keys.ShouldBe(expected: ["common.bow"]);
        pack.Diagnostics.Any(predicate: error => error.Contains(value: "field outputs",
                comparisonType: StringComparison.Ordinal)).ShouldBeTrue();
    }

    private const string InspectionPath = "src/godot-game/inspection/InspectionRoot.cs";
    private const string Native = "Godot.GD.Print(what: [1]);";

    private static void CheckInvocation(ConsumerCompilation consumer, string source,
            BaseMethodDeclarationSyntax method)
    {
        string suffix = method is MethodDeclarationSyntax ordinary && ordinary.Identifier.ValueText is "Read"
                ? "return null;" : "";
        const string Guard = "_gateway.RequireThread();";
        string positive = Body(source: source, method: method, body: Guard + Native + suffix);
        EngineAccessPolicy.Check(source: positive, path: InspectionPath, consumer: consumer).ShouldBeEmpty();
        foreach (string body in new[]
        {
            Native + suffix,
            Native + Guard + suffix,
            "if (false) { " + Guard + " }" + Native + suffix,
            "goto Skip; " + Guard + " Skip: " + Native + suffix,
            "new KingdomZombies.Proofs.MainThreadGateway().RequireThread();" + Native + suffix,
            Guard + "System.Action misuse = new Godot.Node().QueueFree;" + suffix,
            Guard + "System.Action misuse = () => { " + Native + " }; misuse();" + suffix,
            Guard + "System.Threading.Tasks.Task.Run(action: () => { " + Native + " });" + suffix,
            Guard + "System.Threading.Tasks.Task.CompletedTask.ContinueWith(continuationAction: task => { "
                    + Native + " });" + suffix,
        })
        {
            EngineAccessPolicy.Check(source: Body(source: source, method: method, body: body),
                    path: InspectionPath, consumer: consumer).ShouldNotBeEmpty(
                            customMessage: "Exact invocation boundary: " + method.ToString());
        }

        EngineAccessPolicy.Check(source: positive.Replace(oldValue: "readonly MainThreadGateway _gateway",
                newValue: "MainThreadGateway _gateway", comparisonType: StringComparison.Ordinal),
                path: InspectionPath, consumer: consumer).ShouldNotBeEmpty();
    }

    private static string Body(string source, BaseMethodDeclarationSyntax method, string body)
    {
        BlockSyntax block = method.Body ?? throw new InvalidOperationException();
        return source[..block.SpanStart] + "{" + body + "}" + source[block.Span.End..];
    }

    private static void CheckVisibilityRegistry(ConsumerCompilation consumer, string source, string path, string type)
    {
        string root = Path.Combine(path1: ProofProtocol.Root, path2: ".phase4/hub-policy-controls/"
                + Guid.NewGuid().ToString(format: "N"));
        string directory = Path.Combine(path1: root, path2: "docs/code");
        Directory.CreateDirectory(path: directory);
        string file = Path.Combine(path1: directory, path2: "visibility-exemptions.json");
        JsonObject registry = JsonNode.Parse(json: File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "docs/code/visibility-exemptions.json")))?.AsObject() ?? throw new InvalidOperationException();
        File.WriteAllText(path: file, contents: registry.ToJsonString());
        ArchitecturePolicy.Visibility(root: root, source: source, path: path, options: consumer.Options)
                .ShouldBeEmpty();
        string adjacent = source + "\npublic sealed class " + type + "Adjacent { }\n";
        ConsumerCompilation.RequireValid(compilation: consumer.Create(source: adjacent, path: path));
        string space = StringComparer.Ordinal.Equals(x: path, y: InspectionPath)
                ? "KingdomZombies.Inspection" : "KingdomZombies.Core";
        registry["permissions"]?.AsArray().Add(item: new JsonObject
        {
            ["path"] = path,
            ["type"] = space + "." + type + "Adjacent",
            ["reason"] = "A forged extra exact-looking permission must not expand the independent authority.",
        });
        File.WriteAllText(path: file, contents: registry.ToJsonString());
        ArchitecturePolicy.Visibility(root: root, source: adjacent, path: path, options: consumer.Options)
                .ShouldBe(expected: ["R-34: invalid or expanded exact visibility registry"]);
        string clone = source.Replace(oldValue: "namespace " + space + ";",
                newValue: "namespace " + space + ".Adjacent;", comparisonType: StringComparison.Ordinal);
        CSharpCompilation compilation = consumer.Create();
        ConsumerCompilation.RequireValid(compilation: compilation.AddSyntaxTrees(consumer.Parse(source: clone,
                path: path)));
        ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: clone, path: path, options: consumer.Options)
                .ShouldNotBeEmpty();
    }

    private static void CheckOwnership(ConsumerCompilation consumer, string source)
    {
        CSharpCompilation compilation = consumer.Create(source: source, path: InspectionPath);
        SyntaxTree tree = compilation.SyntaxTrees.Single(predicate: candidate => StringComparer.Ordinal.Equals(
                x: candidate.FilePath, y: InspectionPath));
        SyntaxTree wrongPath = tree.WithFilePath(path: "src/godot-game/inspection/AdjacentInspection.cs");
        CSharpCompilation wrong = compilation.ReplaceSyntaxTree(oldTree: tree, newTree: wrongPath);
        ConsumerCompilation.RequireValid(compilation: wrong);
        EngineAccessPolicy.CheckTree(tree: wrongPath, compilation: wrong).ShouldNotBeEmpty();
        SyntaxTree clone = consumer.Parse(source: source.Replace(oldValue: "namespace KingdomZombies.Inspection;",
                newValue: "namespace KingdomZombies.Inspection.Adjacent;", comparisonType: StringComparison.Ordinal),
                path: InspectionPath);
        CSharpCompilation sibling = compilation.AddSyntaxTrees(trees: clone);
        ConsumerCompilation.RequireValid(compilation: sibling);
        EngineAccessPolicy.CheckTree(tree: clone, compilation: sibling).ShouldNotBeEmpty();
        TypeDeclarationSyntax type = tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().Single();
        foreach (string member in new[]
        {
            "private void Adjacent() { _gateway.RequireThread(); " + Native + " }",
            "private void Recheck(int extra) { _gateway.RequireThread(); " + Native + " }",
            "private async System.Threading.Tasks.Task Recheck(string extra) { _gateway.RequireThread(); "
                    + "await System.Threading.Tasks.Task.Delay(millisecondsDelay: 1); " + Native + " }",
        })
        {
            MemberDeclarationSyntax added = SyntaxFactory.ParseMemberDeclaration(text: member)
                    ?? throw new InvalidOperationException();
            string changed = tree.GetRoot().ReplaceNode(oldNode: type, newNode: type.AddMembers(added))
                    .ToFullString();
            EngineAccessPolicy.Check(source: changed, path: InspectionPath, consumer: consumer).ShouldNotBeEmpty();
        }
    }

    private static byte[]? Read(string path)
    {
        string file = Path.Combine(path1: ProofProtocol.Root, path2: "src/godot-game/" + path);
        return File.Exists(path: file) ? File.ReadAllBytes(path: file) : null;
    }

    private static JsonObject Index()
    {
        return JsonNode.Parse(utf8Json: Read(path: "content/pack-index.json") ?? [])?.AsObject()
                ?? throw new InvalidOperationException();
    }

    private static ContentPack Load(JsonObject index, byte[]? expectations = null, bool replaceExpectations = false)
    {
        return ContentPack.Load(index: Encoding.UTF8.GetBytes(s: index.ToJsonString()), expectations:
                replaceExpectations ? expectations : Read(path: ContentPack.ExpectedPath), read: Read);
    }
}
