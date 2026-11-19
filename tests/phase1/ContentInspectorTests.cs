using System;
using System.Collections;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises the real portable pack loader independently of a native game window.</summary>
public sealed class ContentInspectorTests
{
    /// <summary>Retains real emitted diagnostics, including exact member-suppressed calls, for origin review.</summary>
    [Fact]
    public async Task Observe_Inspection_RecordsActualGeneratedDiagnostics()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        CSharpCompilation compilation = consumer.Create();
        ConsumerCompilation.RequireValid(compilation: compilation);
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        ImmutableArray<DiagnosticAnalyzer> analyzers = [.. assembly.GetTypes().Where(predicate: type =>
                !type.IsAbstract && typeof(DiagnosticAnalyzer).IsAssignableFrom(c: type)).Select(selector: type =>
                        (DiagnosticAnalyzer)(Activator.CreateInstance(type: type)
                                ?? throw new InvalidOperationException()))];
        var options = new CompilationWithAnalyzersOptions(options: new AnalyzerOptions(additionalFiles: []),
                onAnalyzerException: null, concurrentAnalysis: false, logAnalyzerExecutionTime: false,
                reportSuppressedDiagnostics: true);
        ImmutableArray<Diagnostic> diagnostics = await compilation.WithAnalyzers(analyzers: analyzers,
                analysisOptions: options).GetAnalyzerDiagnosticsAsync(cancellationToken:
                        TestContext.Current.CancellationToken).ConfigureAwait(continueOnCapturedContext: true);
        SyntaxTree[] trees = [.. compilation.SyntaxTrees.Where(predicate: tree => tree.FilePath.Contains(
                value: "KingdomZombies.Inspection.InspectionRoot_", comparisonType: StringComparison.Ordinal))];
        trees.ShouldNotBeEmpty();
        var files = trees.Select(selector: tree => new
        {
            path = tree.FilePath,
            sha256 = Hash(bytes: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                    path2: tree.FilePath))),
            textSha256 = Hash(bytes: Encoding.UTF8.GetBytes(s: tree.GetText().ToString())),
            diagnostics = diagnostics.Where(predicate: diagnostic => diagnostic.Location.SourceTree == tree)
                    .Select(selector: diagnostic => new
                    {
                        diagnostic = diagnostic.Id,
                        line = diagnostic.Location.GetLineSpan().StartLinePosition.Line + 1,
                        column = diagnostic.Location.GetLineSpan().StartLinePosition.Character + 1,
                        length = diagnostic.Location.SourceSpan.Length,
                        reason = diagnostic.GetMessage(formatProvider: CultureInfo.InvariantCulture),
                        suppressed = diagnostic.IsSuppressed,
                    }).ToArray(),
        }).Where(predicate: file => file.diagnostics.Length > 0).ToArray();
        files.Sum(selector: file => file.diagnostics.Length).ShouldBeGreaterThan(expected: 0);
        string directory = Path.Combine(path1: ProofProtocol.Root, path2: ".phase4/observed26/"
                + Guid.NewGuid().ToString(format: "N"));
        Directory.CreateDirectory(path: directory);
        File.WriteAllText(path: Path.Combine(path1: directory, path2: "diagnostics.json"), contents:
                JsonSerializer.Serialize(value: files));
        foreach (SyntaxTree tree in trees)
        {
            File.Copy(sourceFileName: Path.Combine(path1: ProofProtocol.Root, path2: tree.FilePath),
                    destFileName: Path.Combine(path1: directory, path2: Path.GetFileName(path: tree.FilePath)));
        }

    }

    /// <summary>Every actual inspection member needs its own synchronous captured guard and exact signature.</summary>
    [Fact]
    public void Authorize_Inspection_RequiresExactInvocationTuples()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: InspectionPath));
        EngineAccessPolicy.Check(source: source, path: InspectionPath, consumer: consumer).ShouldBeEmpty();
        string[] signatures = ["InspectionRoot()", "_Ready()", "_Process(double)", "_ExitTree()", "Recheck()",
            "Cancel()", "Read(string)", "ChangeCategory(long)", "ChangeStatus(long)", "Filter(string)",
            "Select(long)", "Details()", "Copy()", "Project()", "Rows()", "Selection()", "_Input(InputEvent)",
            "AcceptPreview(int,Node)"];
        EngineAccessPolicy.InspectionTuples.Where(predicate: tuple => tuple.StartsWith(value: InspectionPath + "|",
                comparisonType: StringComparison.Ordinal)).ShouldBe(expected: signatures.Select(
                        selector: signature =>
                                InspectionPath + "|KingdomZombies.Inspection|InspectionRoot|" + signature));
        SyntaxTree tree = consumer.Parse(source: source, path: InspectionPath);
        BaseMethodDeclarationSyntax[] methods = [.. tree.GetRoot(cancellationToken:
                TestContext.Current.CancellationToken).DescendantNodes()
                .OfType<BaseMethodDeclarationSyntax>()];
        methods.Length.ShouldBe(expected: signatures.Length);
        CheckOwnership(consumer: consumer, source: source);
    }

    /// <summary>Internal helpers retain exact signatures and compiler-valid invocation rejection controls.</summary>
    [Theory]
    [InlineData("InspectionInput", "Dismiss(InputEvent)|ReconcileFocus()")]
    [InlineData("InspectionShell", "Attach()|Layout()|Tick(double)|Install(Node)|Retire()|Detach()|"
            + "AddButton(Node,string,string,Action)|Play()|Speed(long)|Step()|Reset()|ShowTransport()")]
    public void Authorize_InternalInspection_RequiresExactInvocation(string type, string signatures)
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        string path = "src/godot-game/inspection/" + type + ".cs";
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: path));
        EngineAccessPolicy.Check(source: source, path: path, consumer: consumer).ShouldBeEmpty();
        EngineAccessPolicy.InspectionTuples.Where(predicate: tuple => tuple.StartsWith(value: path + "|",
                comparisonType: StringComparison.Ordinal)).ShouldBe(expected: signatures.Split(separator: '|')
                        .Select(selector: signature => path + "|KingdomZombies.Inspection|" + type + "|" + signature));
        SyntaxTree tree = consumer.Parse(source: source, path: path);
        CSharpCompilation compilation = consumer.Create(source: source, path: path);
        SyntaxTree actual = compilation.SyntaxTrees.Single(predicate: candidate => StringComparer.Ordinal.Equals(
                x: candidate.FilePath, y: path));
        SyntaxTree wrongPath = actual.WithFilePath(path: "src/godot-game/inspection/Adjacent" + type + ".cs");
        CSharpCompilation wrong = compilation.ReplaceSyntaxTree(oldTree: actual, newTree: wrongPath);
        ConsumerCompilation.RequireValid(compilation: wrong);
        EngineAccessPolicy.CheckTree(tree: wrongPath, compilation: wrong).ShouldNotBeEmpty();
        string sibling = source.Replace(oldValue: "namespace KingdomZombies.Inspection;",
                newValue: "namespace KingdomZombies.Inspection.Adjacent;", comparisonType: StringComparison.Ordinal);
        SyntaxTree clone = consumer.Parse(source: sibling, path: path);
        CSharpCompilation adjacent = compilation.AddSyntaxTrees(trees: clone);
        ConsumerCompilation.RequireValid(compilation: adjacent);
        EngineAccessPolicy.CheckTree(tree: clone, compilation: adjacent).ShouldNotBeEmpty();
        string extra = source.Insert(startIndex: source.LastIndexOf(value: '}'),
                value: "private void Adjacent() { _gateway.RequireThread(); " + Native + " }");
        EngineAccessPolicy.Check(source: extra, path: path, consumer: consumer).ShouldNotBeEmpty();
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
        Func<string, byte[]?> read = Read;
        object? pack = method?.Invoke(obj: null, parameters:
        [
            read(arg: "content/pack-index.json"), read(arg: "content/expected-obligations.json"), read,
        ]);
        object? entries = type?.GetProperty(name: "Entries")?.GetValue(obj: pack);
        string directory = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY")
                ?? Path.GetTempPath();
        string corePath = type?.Assembly.Location ?? string.Empty;
        using var observation = new FileStream(path: Path.Combine(path1: directory,
                path2: "partial-pack-observation-" + Guid.NewGuid().ToString(format: "N") + ".json"),
                mode: FileMode.CreateNew, access: FileAccess.Write);
        string[] fixturePaths = ["content/pack-index.json", "content/packs/tracer.json", "inspection/Inspection.tscn"];
        JsonSerializer.Serialize(utf8Json: observation, value: new
        {
            scope = "Actual retained partial-pack reflection Load; materialized before original assertions",
            fixtures = fixturePaths.Select(selector: path => new
            {
                path,
                bytes = read(arg: path),
                extent = read(arg: path)?.Length,
                sha256 = Convert.ToHexStringLower(inArray: SHA256.HashData(source: read(arg: path) ?? [])),
            }).ToArray(),
            entries,
            diagnostics = type?.GetProperty(name: "Diagnostics")?.GetValue(obj: pack),
            status = type?.GetProperty(name: "Status")?.GetValue(obj: pack),
            corePath,
            coreSha256 = File.Exists(path: corePath) ? Convert.ToHexStringLower(inArray:
                    SHA256.HashData(source: File.ReadAllBytes(path: corePath))) : string.Empty,
            coreMvid = type?.Assembly.ManifestModule.ModuleVersionId,
        });
        observation.Flush(flushToDisk: true);
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

    /// <summary>Executes the actual Root member 1 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root01_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 0);
    }

    /// <summary>Executes the actual Root member 2 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root02_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 1);
    }

    /// <summary>Executes the actual Root member 3 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root03_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 2);
    }

    /// <summary>Executes the actual Root member 4 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root04_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 3);
    }

    /// <summary>Executes the actual Root member 5 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root05_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 4);
    }

    /// <summary>Executes the actual Root member 6 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root06_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 5);
    }

    /// <summary>Executes the actual Root member 7 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root07_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 6);
    }

    /// <summary>Executes the actual Root member 8 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root08_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 7);
    }

    /// <summary>Executes the actual Root member 9 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root09_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 8);
    }

    /// <summary>Executes the actual Root member 10 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root10_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 9);
    }

    /// <summary>Executes the actual Root member 11 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root11_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 10);
    }

    /// <summary>Executes the actual Root member 12 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root12_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 11);
    }

    /// <summary>Executes the actual Root member 13 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root13_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 12);
    }

    /// <summary>Executes the actual Root member 14 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root14_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 13);
    }

    /// <summary>Executes the actual Root member 15 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root15_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 14);
    }

    /// <summary>Executes the actual Root member 16 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root16_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 15);
    }

    /// <summary>Executes the actual Root member 17 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root17_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 16);
    }

    /// <summary>Executes the actual Root member 18 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Root18_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionRoot", index: 17);
    }

    /// <summary>Executes the actual Input member 1 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Input01_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionInput", index: 0);
    }

    /// <summary>Executes the actual Input member 2 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Input02_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionInput", index: 1);
    }

    /// <summary>Executes the actual Shell member 1 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell01_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 0);
    }

    /// <summary>Executes the actual Shell member 2 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell02_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 1);
    }

    /// <summary>Executes the actual Shell member 3 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell03_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 2);
    }

    /// <summary>Executes the actual Shell member 4 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell04_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 3);
    }

    /// <summary>Executes the actual Shell member 5 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell05_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 4);
    }

    /// <summary>Executes the actual Shell member 6 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell06_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 5);
    }

    /// <summary>Executes the actual Shell member 7 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell07_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 6);
    }

    /// <summary>Executes the actual Shell member 8 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell08_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 7);
    }

    /// <summary>Executes the actual Shell member 9 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell09_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 8);
    }

    /// <summary>Executes the actual Shell member 10 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell10_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 9);
    }

    /// <summary>Executes the actual Shell member 11 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell11_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 10);
    }

    /// <summary>Executes the actual Shell member 12 positive and eleven compiler-valid negatives.</summary>
    [Fact]
    public void Authorize_Shell12_RequiresExactInvocation()
    {
        CheckMember(type: "InspectionShell", index: 11);
    }

    private const string LegacyIndex =
            "ewogICJzY2hlbWFWZXJzaW9uIjogMSwKICAiaWQiOiAia2luZ2RvbS16b21iaWVzLnYwLjEiLAogICJ2ZXJzaW9uIjogMSwK" +
                    "ICAic3RhdHVzIjogImluY29tcGxldGUiLAogICJhY2NlcHRhbmNlT3duZXIiOiAiMDQtMzIvMDQtMzMiLAogICJleHBlY3Rl" +
                    "ZE9ibGlnYXRpb25zIjogewogICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9leHBlY3RlZC1vYmxpZ2F0aW9u" +
                    "cy5qc29uIiwKICAgICJieXRlcyI6IDI1MzQ1LAogICAgInNoYTI1NiI6ICJhNGQ5MjQzYmRhMWI1MTJiNTljMTg5YTNiZGM0" +
                    "OGE3Y2NjYzgxNzFhMjE4YmJmYzVhZTE2YjAwMjc0ZjJmZGE1IgogIH0sCiAgInNoYXJkcyI6IFsKICAgIHsKICAgICAgImlk" +
                    "IjogInRyYWNlciIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMDEiLAogICAgICAic3RhdHVzIjogInNlbGVjdGVkIiwKICAg" +
                    "ICAgInZlcnNpb24iOiAxLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL3RyYWNlci5qc29u" +
                    "IiwKICAgICAgImJ5dGVzIjogMjQzMCwKICAgICAgInNoYTI1NiI6ICJlMjFjOTMzYjM2YmFjZDVmODEyMDZiMzNmN2E5Njgw" +
                    "YmVkZDg5YmQ1ZTU4MGIxMmY1MDA0YWIyMTI4OTVkMWY0IgogICAgfSwKICAgIHsKICAgICAgImlkIjogImF1ZGlvLWFjdGlv" +
                    "bnMiLAogICAgICAib3duZXJQbGFuIjogIjA0LTIxIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgi" +
                    "OiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy9hdWRpby1hY3Rpb25zLmpzb24iCiAgICB9LAogICAgewogICAgICAi" +
                    "aWQiOiAiYXVkaW8taW50ZXJmYWNlLWFtYmllbmNlIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0yMSIsCiAgICAgICJzdGF0" +
                    "dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvYXVkaW8taW50ZXJm" +
                    "YWNlLWFtYmllbmNlLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAiYm9kaWVzLWEiLAogICAgICAib3duZXJQbGFu" +
                    "IjogIjA0LTA2IiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29u" +
                    "dGVudC9wYWNrcy9ib2RpZXMtYS5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogImJvZGllcy1iIiwKICAgICAgIm93" +
                    "bmVyUGxhbiI6ICIwNC0wNyIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1n" +
                    "YW1lL2NvbnRlbnQvcGFja3MvYm9kaWVzLWIuanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJib2RpZXMtYyIsCiAg" +
                    "ICAgICJvd25lclBsYW4iOiAiMDQtMDciLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMv" +
                    "Z29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2JvZGllcy1jLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAiY2FtcC1h" +
                    "Y2Nlc3MiLAogICAgICAib3duZXJQbGFuIjogIjA0LTExIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBh" +
                    "dGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy9jYW1wLWFjY2Vzcy5qc29uIgogICAgfSwKICAgIHsKICAgICAg" +
                    "ImlkIjogImNvbW1vbiIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMDQiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAog" +
                    "ICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2NvbW1vbi5qc29uIgogICAgfSwKICAgIHsKICAg" +
                    "ICAgImlkIjogImNvbXBhdGliaWxpdHkiLAogICAgICAib3duZXJQbGFuIjogIjA0LTIyIiwKICAgICAgInN0YXR1cyI6ICJw" +
                    "ZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy9jb21wYXRpYmlsaXR5Lmpzb24i" +
                    "CiAgICB9LAogICAgewogICAgICAiaWQiOiAiY29ycHNlcy13aWxkbGlmZSIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTAi" +
                    "LAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tz" +
                    "L2NvcnBzZXMtd2lsZGxpZmUuanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJkZWZlbnNlcyIsCiAgICAgICJvd25l" +
                    "clBsYW4iOiAiMDQtMTIiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2Ft" +
                    "ZS9jb250ZW50L3BhY2tzL2RlZmVuc2VzLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAiZGVsaXZlcnkiLAogICAg" +
                    "ICAib3duZXJQbGFuIjogIjA0LTMxIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dv" +
                    "ZG90LWdhbWUvY29udGVudC9wYWNrcy9kZWxpdmVyeS5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogImVmZmVjdHMt" +
                    "cG9ydGFscyIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTMiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAi" +
                    "cGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2VmZmVjdHMtcG9ydGFscy5qc29uIgogICAgfSwKICAgIHsK" +
                    "ICAgICAgImlkIjogImd1YXJkcyIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTIiLAogICAgICAic3RhdHVzIjogInBlbmRp" +
                    "bmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2d1YXJkcy5qc29uIgogICAgfSwKICAg" +
                    "IHsKICAgICAgImlkIjogIml0ZW1zLTAxLTEwIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0wNSIsCiAgICAgICJzdGF0dXMi" +
                    "OiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvaXRlbXMtMDEtMTAuanNv" +
                    "biIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJpdGVtcy0xMS0yNCIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMDUiLAog" +
                    "ICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2l0" +
                    "ZW1zLTExLTI0Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAibWFpbi1idWlsZGluZ3MiLAogICAgICAib3duZXJQ" +
                    "bGFuIjogIjA0LTExIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUv" +
                    "Y29udGVudC9wYWNrcy9tYWluLWJ1aWxkaW5ncy5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogInBsYXllci1pZGVu" +
                    "dGl0aWVzIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0xMCIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJw" +
                    "YXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvcGxheWVyLWlkZW50aXRpZXMuanNvbiIKICAgIH0sCiAgICB7" +
                    "CiAgICAgICJpZCI6ICJzY2FsZS1waWxvdCIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMDQiLAogICAgICAic3RhdHVzIjog" +
                    "InBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL3NjYWxlLXBpbG90Lmpzb24i" +
                    "CiAgICB9LAogICAgewogICAgICAiaWQiOiAidGV4dC1kYXRhIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0yMiIsCiAgICAg" +
                    "ICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdGV4dC1k" +
                    "YXRhLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWktY2FtcCIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTYi" +
                    "LAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tz" +
                    "L3VpLWNhbXAuanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJ1aS1pZGVudGl0eSIsCiAgICAgICJvd25lclBsYW4i" +
                    "OiAiMDQtMjgiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250" +
                    "ZW50L3BhY2tzL3VpLWlkZW50aXR5Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWktaW52ZW50b3J5IiwKICAg" +
                    "ICAgIm93bmVyUGxhbiI6ICIwNC0xNSIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9n" +
                    "b2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdWktaW52ZW50b3J5Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWkt" +
                    "a2l0IiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0xNCIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRo" +
                    "IjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdWkta2l0Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAi" +
                    "dWktbGVnYWN5LXN0YXNoIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0xNSIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIs" +
                    "CiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdWktbGVnYWN5LXN0YXNoLmpzb24iCiAgICB9" +
                    "LAogICAgewogICAgICAiaWQiOiAidWktbWFwLXdhcm5pbmdzIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0xNiIsCiAgICAg" +
                    "ICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdWktbWFw" +
                    "LXdhcm5pbmdzLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWktc2Vzc2lvbiIsCiAgICAgICJvd25lclBsYW4i" +
                    "OiAiMDQtMTciLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250" +
                    "ZW50L3BhY2tzL3VpLXNlc3Npb24uanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJ1aS1zZXR0aW5ncyIsCiAgICAg" +
                    "ICJvd25lclBsYW4iOiAiMDQtMTciLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29k" +
                    "b3QtZ2FtZS9jb250ZW50L3BhY2tzL3VpLXNldHRpbmdzLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAid29ybGQi" +
                    "LAogICAgICAib3duZXJQbGFuIjogIjA0LTEzIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAi" +
                    "c3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy93b3JsZC5qc29uIgogICAgfQogIF0sCiAgIm9yaWdpbnMiOiBbCiAgICB7" +
                    "CiAgICAgICJvd25lciI6ICJzaGFyZWRhc3NldHMwLmFzc2V0cyIsCiAgICAgICJ0eXBlIjogIlNwcml0ZSIsCiAgICAgICJv" +
                    "YmplY3RJZCI6IDcwMDgsCiAgICAgICJuYW1lIjogInRvb2xzX2JvdyIsCiAgICAgICJvd25lclNoYTI1NiI6ICIzNjQ4NjU4" +
                    "ZDQ3NTE3MDY2MTUxN2M0OTA3ZTUwOTc3YTRhYjkzMjQ0MWI0MDRhNjgxY2QwY2U0ODU1OTJmM2ZmIiwKICAgICAgInJhd1No" +
                    "YTI1NiI6ICIxMmE3MzAzZmEwODE1ZmI4Y2QwYWMxMDYzMGU5MzRkZDkxYjg3Nzc3YzA0MTgzOGFkYTdjZmE4MjU5ZmE0MDIz" +
                    "IiwKICAgICAgInN0cmVhbXMiOiBbCiAgICAgICAgewogICAgICAgICAgInBhdGgiOiAic2hhcmVkYXNzZXRzMC5hc3NldHMu" +
                    "cmVzUyIsCiAgICAgICAgICAib2Zmc2V0IjogMjgwNzE4MTI4LAogICAgICAgICAgImJ5dGVzIjogNDE5NDMwNCwKICAgICAg" +
                    "ICAgICJzaGEyNTYiOiAiMmVmY2NhZTg4MDAyYTAyY2E4NWYwNTBhNmQ2ZGMyZTVmZTg5NGMzMmRlODBlMzZmMzBjZDEyZjhj" +
                    "N2ZmYjVjMiIsCiAgICAgICAgICAicmFuZ2VTaGEyNTYiOiAiNjIwZDNhNmZkM2Y0NDFlYTRlMDJmMDdiZWY4NDc5ZjExOGQ0" +
                    "NTEwNzZlMjk5MTdkMjkyNzk3YmFiYjNiYWQ1MyIKICAgICAgICB9CiAgICAgIF0sCiAgICAgICJkZXBlbmRlbmNpZXMiOiBb" +
                    "CiAgICAgICAgewogICAgICAgICAgIm93bmVyIjogInNoYXJlZGFzc2V0czAuYXNzZXRzIiwKICAgICAgICAgICJ0eXBlIjog" +
                    "IlNwcml0ZUF0bGFzIiwKICAgICAgICAgICJvYmplY3RJZCI6IDI1Mzc2LAogICAgICAgICAgIm5hbWUiOiAib2JqZWN0cyIK" +
                    "ICAgICAgICB9LAogICAgICAgIHsKICAgICAgICAgICJvd25lciI6ICJzaGFyZWRhc3NldHMwLmFzc2V0cyIsCiAgICAgICAg" +
                    "ICAidHlwZSI6ICJUZXh0dXJlMkQiLAogICAgICAgICAgIm9iamVjdElkIjogMTc4LAogICAgICAgICAgIm5hbWUiOiAic2Fj" +
                    "dHgtMC0xMDI0eDEwMjQtVW5jb21wcmVzc2VkLW9iamVjdHMtMDc2NTRjN2IiCiAgICAgICAgfQogICAgICBdCiAgICB9CiAg" +
                    "XSwKICAib3V0cHV0cyI6IFsKICAgIHsKICAgICAgImlkIjogImNvbW1vbi5ib3ciLAogICAgICAibmFtZSI6ICJCb3cgKHBh" +
                    "cnRpYWwgbWFzdGVyKSIsCiAgICAgICJ2ZXJzaW9uIjogMSwKICAgICAgInNoYXJkIjogInRyYWNlciIsCiAgICAgICJwYXRo" +
                    "IjogInNyYy9nb2RvdC1nYW1lL2Fzc2V0cy9jb21tb24vYm93LnBuZyIsCiAgICAgICJieXRlcyI6IDEzNSwKICAgICAgInNo" +
                    "YTI1NiI6ICIwOGI1MzY5OWU0NTlkNzFjZjczZjNlN2JlODRhZjE5ZDA0ODA1ZDVhN2E0ODgxMjFlMzlhNTE0YTgxNzAxOTk5" +
                    "IiwKICAgICAgInJvdXRlIjogInJldXNlIiwKICAgICAgIm9yaWdpbnMiOiBbCiAgICAgICAgewogICAgICAgICAgIm93bmVy" +
                    "IjogInNoYXJlZGFzc2V0czAuYXNzZXRzIiwKICAgICAgICAgICJ0eXBlIjogIlNwcml0ZSIsCiAgICAgICAgICAib2JqZWN0" +
                    "SWQiOiA3MDA4LAogICAgICAgICAgIm5hbWUiOiAidG9vbHNfYm93IiwKICAgICAgICAgICJvd25lclNoYTI1NiI6ICIzNjQ4" +
                    "NjU4ZDQ3NTE3MDY2MTUxN2M0OTA3ZTUwOTc3YTRhYjkzMjQ0MWI0MDRhNjgxY2QwY2U0ODU1OTJmM2ZmIiwKICAgICAgICAg" +
                    "ICJyYXdTaGEyNTYiOiAiMTJhNzMwM2ZhMDgxNWZiOGNkMGFjMTA2MzBlOTM0ZGQ5MWI4Nzc3N2MwNDE4MzhhZGE3Y2ZhODI1" +
                    "OWZhNDAyMyIsCiAgICAgICAgICAic3RyZWFtcyI6IFsKICAgICAgICAgICAgewogICAgICAgICAgICAgICJwYXRoIjogInNo" +
                    "YXJlZGFzc2V0czAuYXNzZXRzLnJlc1MiLAogICAgICAgICAgICAgICJvZmZzZXQiOiAyODA3MTgxMjgsCiAgICAgICAgICAg" +
                    "ICAgImJ5dGVzIjogNDE5NDMwNCwKICAgICAgICAgICAgICAic2hhMjU2IjogIjJlZmNjYWU4ODAwMmEwMmNhODVmMDUwYTZk" +
                    "NmRjMmU1ZmU4OTRjMzJkZTgwZTM2ZjMwY2QxMmY4YzdmZmI1YzIiLAogICAgICAgICAgICAgICJyYW5nZVNoYTI1NiI6ICI2" +
                    "MjBkM2E2ZmQzZjQ0MWVhNGUwMmYwN2JlZjg0NzlmMTE4ZDQ1MTA3NmUyOTkxN2QyOTI3OTdiYWJiM2JhZDUzIgogICAgICAg" +
                    "ICAgICB9CiAgICAgICAgICBdLAogICAgICAgICAgImRlcGVuZGVuY2llcyI6IFsKICAgICAgICAgICAgewogICAgICAgICAg" +
                    "ICAgICJvd25lciI6ICJzaGFyZWRhc3NldHMwLmFzc2V0cyIsCiAgICAgICAgICAgICAgInR5cGUiOiAiU3ByaXRlQXRsYXMi" +
                    "LAogICAgICAgICAgICAgICJvYmplY3RJZCI6IDI1Mzc2LAogICAgICAgICAgICAgICJuYW1lIjogIm9iamVjdHMiCiAgICAg" +
                    "ICAgICAgIH0sCiAgICAgICAgICAgIHsKICAgICAgICAgICAgICAib3duZXIiOiAic2hhcmVkYXNzZXRzMC5hc3NldHMiLAog" +
                    "ICAgICAgICAgICAgICJ0eXBlIjogIlRleHR1cmUyRCIsCiAgICAgICAgICAgICAgIm9iamVjdElkIjogMTc4LAogICAgICAg" +
                    "ICAgICAgICJuYW1lIjogInNhY3R4LTAtMTAyNHgxMDI0LVVuY29tcHJlc3NlZC1vYmplY3RzLTA3NjU0YzdiIgogICAgICAg" +
                    "ICAgICB9CiAgICAgICAgICBdCiAgICAgICAgfQogICAgICBdLAogICAgICAicHJvZHVjdGlvbiI6IHsKICAgICAgICAicGF0" +
                    "aCI6ICJwcm9kdWN0aW9uL3BoYXNlNC90cmFjZXItc291cmNlLmpzb24iLAogICAgICAgICJieXRlcyI6IDE0NTQxLAogICAg" +
                    "ICAgICJzaGEyNTYiOiAiMDM2NzNlMmY4OTIzNjkyNjNhNTRmY2E2YWVlMjY2ODRiYzVmM2YwYjlhZWU1NDBmM2MwMDFkMTkw" +
                    "YmQ0NWZhYyIKICAgICAgfSwKICAgICAgImRlcGVuZGVuY2llcyI6IFsKICAgICAgICB7CiAgICAgICAgICAiaWQiOiAiaW5z" +
                    "cGVjdGlvbi5ib3ciLAogICAgICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvaW5zcGVjdGlvbi9JbnNwZWN0aW9uLnRz" +
                    "Y24iLAogICAgICAgICAgImJ5dGVzIjogOTkzLAogICAgICAgICAgInNoYTI1NiI6ICI2NjcwMmVmYjliZjkzOTBmNjQ1NDMx" +
                    "NmNlMGVjNWJjNzgwOGNiNWY0MzlhZGRmMDQzMGQ0NmFiY2NiNTg5NzM5IgogICAgICAgIH0KICAgICAgXSwKICAgICAgIm1l" +
                    "dGFkYXRhIjogewogICAgICAgICJraW5kIjogImltYWdlIiwKICAgICAgICAid2lkdGgiOiAxNiwKICAgICAgICAiaGVpZ2h0" +
                    "IjogMTgsCiAgICAgICAgInJnYmFTaGEyNTYiOiAiODhiOGU3MDYyZjBlYjljZmI3ODhlZjM2NTgwMjhiYTU4YTZmNjJkNDA4" +
                    "ZDNlMzlhZTJlMTIyOGJmYTZjODkwNSIsCiAgICAgICAgInJlZ2lzdHJhdGlvbiI6IHsKICAgICAgICAgICJsb2dpY2FsUmVj" +
                    "dCI6IHsKICAgICAgICAgICAgIngiOiAwLAogICAgICAgICAgICAieSI6IDAsCiAgICAgICAgICAgICJ3aWR0aCI6IDE2LAog" +
                    "ICAgICAgICAgICAiaGVpZ2h0IjogMTgKICAgICAgICAgIH0sCiAgICAgICAgICAicGl2b3ROb3JtYWxpemVkQm90dG9tTGVm" +
                    "dCI6IHsKICAgICAgICAgICAgIngiOiAwLjM3NSwKICAgICAgICAgICAgInkiOiAwLjUKICAgICAgICAgIH0sCiAgICAgICAg" +
                    "ICAic291cmNlUGl4ZWxzUGVyVW5pdCI6IDMyLAogICAgICAgICAgInNvdXJjZU9mZnNldCI6IHsKICAgICAgICAgICAgIngi" +
                    "OiAtMiwKICAgICAgICAgICAgInkiOiAwCiAgICAgICAgICB9LAogICAgICAgICAgInRyaW1PZmZzZXQiOiB7CiAgICAgICAg" +
                    "ICAgICJ4IjogMCwKICAgICAgICAgICAgInkiOiAwCiAgICAgICAgICB9LAogICAgICAgICAgImF0bGFzUmVjdE9mZnNldCI6" +
                    "IHsKICAgICAgICAgICAgIngiOiA1NzYsCiAgICAgICAgICAgICJ5IjogNTI2CiAgICAgICAgICB9LAogICAgICAgICAgImF0" +
                    "bGFzVGV4dHVyZVJlY3QiOiB7CiAgICAgICAgICAgICJ4IjogNTc4LAogICAgICAgICAgICAieSI6IDUyNiwKICAgICAgICAg" +
                    "ICAgIndpZHRoIjogMTYsCiAgICAgICAgICAgICJoZWlnaHQiOiAxOAogICAgICAgICAgfSwKICAgICAgICAgICJhdGxhc1Nl" +
                    "dHRpbmdzUmF3IjogNjcsCiAgICAgICAgICAiYWxwaGFUZXh0dXJlSWQiOiAwLAogICAgICAgICAgImRpc3BsYXkiOiB7CiAg" +
                    "ICAgICAgICAgICJjZW50ZXJlZCI6IGZhbHNlLAogICAgICAgICAgICAib2Zmc2V0VG9wTGVmdCI6IFsKICAgICAgICAgICAg" +
                    "ICAtNiwKICAgICAgICAgICAgICAtOQogICAgICAgICAgICBdLAogICAgICAgICAgICAic2NhbGUiOiBbCiAgICAgICAgICAg" +
                    "ICAgMSwKICAgICAgICAgICAgICAxCiAgICAgICAgICAgIF0sCiAgICAgICAgICAgICJmaWx0ZXIiOiAibmVhcmVzdCIKICAg" +
                    "ICAgICAgIH0KICAgICAgICB9CiAgICAgIH0sCiAgICAgICJhcHBsaWNhYmlsaXR5IjogewogICAgICAgICJvYmxpZ2F0aW9u" +
                    "cyI6ICJJVC0wNyBwYXJ0aWFsIG1hc3RlciBvbmx5OyBpY29uL2RyYXduL3JlbGVhc2VkL2ZpdHMvY29ycHNlIHN0aWxsIHJl" +
                    "cXVpcmVkIiwKICAgICAgICAibGlzdGVuaW5nIjogIm5vdC1hcHBsaWNhYmxlIgogICAgICB9LAogICAgICAiaW5zcGVjdGlv" +
                    "biI6IHsKICAgICAgICAicm91dGUiOiAicmVzOi8vaW5zcGVjdGlvbi9JbnNwZWN0aW9uLnRzY24iLAogICAgICAgICJkZXBl" +
                    "bmRlbmN5IjogImluc3BlY3Rpb24uYm93IgogICAgICB9LAogICAgICAiZXZpZGVuY2UiOiB7CiAgICAgICAgInNvdXJjZSI6" +
                    "ICJwZW5kaW5nIiwKICAgICAgICAibmF0aXZlIjogInBlbmRpbmciLAogICAgICAgICJsaXN0ZW5pbmciOiAibm90LWFwcGxp" +
                    "Y2FibGUiLAogICAgICAgICJqdWRnbWVudCI6ICJwZW5kaW5nIiwKICAgICAgICAicmVjb3JkcyI6IFtdCiAgICAgIH0sCiAg" +
                    "ICAgICJoaXN0b3JpY2FsSW5wdXRFdmlkZW5jZSI6IHsKICAgICAgICAibGV2ZWwiOiAiViIsCiAgICAgICAgImNyZWRpdCI6" +
                    "ICJObyBjdXJyZW50IHByb2R1Y3Rpb24vbmF0aXZlL3Zpc3VhbCBhY2NlcHRhbmNlIGNyZWRpdCIKICAgICAgfSwKICAgICAg" +
                    "Im9ibGlnYXRpb25zIjogWwogICAgICAgIHsKICAgICAgICAgICJpZCI6ICJJVC0wNyIsCiAgICAgICAgICAiY292ZXJhZ2Ui" +
                    "OiAicGFydGlhbCIKICAgICAgICB9CiAgICAgIF0KICAgIH0KICBdCn0K";

    private const string LegacyTracer =
            "ewogICJzY2hlbWFWZXJzaW9uIjogMSwKICAiaWQiOiAidHJhY2VyIiwKICAidmVyc2lvbiI6IDEsCiAgInN0YXR1cyI6ICJp" +
                    "bmNvbXBsZXRlLXBoYXNlLXBhY2siLAogICJyZXNvdXJjZXMiOiBbCiAgICB7CiAgICAgICJpZCI6ICJjb21tb24uYm93IiwK" +
                    "ICAgICAgIm5hbWUiOiAiQm93IiwKICAgICAgInZlcnNpb24iOiAxLAogICAgICAicGF0aCI6ICJhc3NldHMvY29tbW9uL2Jv" +
                    "dy5wbmciLAogICAgICAic2NlbmUiOiAiaW5zcGVjdGlvbi9JbnNwZWN0aW9uLnRzY24iLAogICAgICAic2hhMjU2IjogIjA4" +
                    "YjUzNjk5ZTQ1OWQ3MWNmNzNmM2U3YmU4NGFmMTlkMDQ4MDVkNWE3YTQ4ODEyMWUzOWE1MTRhODE3MDE5OTkiLAogICAgICAi" +
                    "c291cmNlIjogewogICAgICAgICJvd25lciI6ICJzaGFyZWRhc3NldHMwLmFzc2V0cyIsCiAgICAgICAgInR5cGUiOiAiU3By" +
                    "aXRlIiwKICAgICAgICAib2JqZWN0SWQiOiA3MDA4LAogICAgICAgICJuYW1lIjogInRvb2xzX2JvdyIsCiAgICAgICAgImF0" +
                    "bGFzSWQiOiAyNTM3NiwKICAgICAgICAiYXRsYXNOYW1lIjogIm9iamVjdHMiLAogICAgICAgICJ0ZXh0dXJlSWQiOiAxNzgs" +
                    "CiAgICAgICAgInRleHR1cmVOYW1lIjogInNhY3R4LTAtMTAyNHgxMDI0LVVuY29tcHJlc3NlZC1vYmplY3RzLTA3NjU0Yzdi" +
                    "IiwKICAgICAgICAic3RyZWFtIjogewogICAgICAgICAgIm9mZnNldCI6IDI4MDcxODEyOCwKICAgICAgICAgICJzaXplIjog" +
                    "NDE5NDMwNCwKICAgICAgICAgICJwYXRoIjogInNoYXJlZGFzc2V0czAuYXNzZXRzLnJlc1MiCiAgICAgICAgfQogICAgICB9" +
                    "LAogICAgICAicmVnaXN0cmF0aW9uIjogewogICAgICAgICJsb2dpY2FsUmVjdCI6IHsKICAgICAgICAgICJ4IjogMCwKICAg" +
                    "ICAgICAgICJ5IjogMCwKICAgICAgICAgICJ3aWR0aCI6IDE2LAogICAgICAgICAgImhlaWdodCI6IDE4CiAgICAgICAgfSwK" +
                    "ICAgICAgICAicGl2b3ROb3JtYWxpemVkQm90dG9tTGVmdCI6IHsKICAgICAgICAgICJ4IjogMC4zNzUsCiAgICAgICAgICAi" +
                    "eSI6IDAuNQogICAgICAgIH0sCiAgICAgICAgInNvdXJjZVBpeGVsc1BlclVuaXQiOiAzMiwKICAgICAgICAic291cmNlT2Zm" +
                    "c2V0IjogewogICAgICAgICAgIngiOiAtMiwKICAgICAgICAgICJ5IjogMAogICAgICAgIH0sCiAgICAgICAgInRyaW1PZmZz" +
                    "ZXQiOiB7CiAgICAgICAgICAieCI6IDAsCiAgICAgICAgICAieSI6IDAKICAgICAgICB9LAogICAgICAgICJhdGxhc1JlY3RP" +
                    "ZmZzZXQiOiB7CiAgICAgICAgICAieCI6IDU3NiwKICAgICAgICAgICJ5IjogNTI2CiAgICAgICAgfSwKICAgICAgICAiYXRs" +
                    "YXNUZXh0dXJlUmVjdCI6IHsKICAgICAgICAgICJ4IjogNTc4LAogICAgICAgICAgInkiOiA1MjYsCiAgICAgICAgICAid2lk" +
                    "dGgiOiAxNiwKICAgICAgICAgICJoZWlnaHQiOiAxOAogICAgICAgIH0sCiAgICAgICAgImF0bGFzU2V0dGluZ3NSYXciOiA2" +
                    "NywKICAgICAgICAiYWxwaGFUZXh0dXJlSWQiOiAwLAogICAgICAgICJkaXNwbGF5IjogewogICAgICAgICAgImNlbnRlcmVk" +
                    "IjogZmFsc2UsCiAgICAgICAgICAib2Zmc2V0VG9wTGVmdCI6IFsKICAgICAgICAgICAgLTYsCiAgICAgICAgICAgIC05CiAg" +
                    "ICAgICAgICBdLAogICAgICAgICAgInNjYWxlIjogWwogICAgICAgICAgICAxLAogICAgICAgICAgICAxCiAgICAgICAgICBd" +
                    "LAogICAgICAgICAgImZpbHRlciI6ICJuZWFyZXN0IgogICAgICAgIH0KICAgICAgfSwKICAgICAgImltYWdlIjogewogICAg" +
                    "ICAgICJ3aWR0aCI6IDE2LAogICAgICAgICJoZWlnaHQiOiAxOCwKICAgICAgICAibW9kZSI6ICJSR0JBIiwKICAgICAgICAi" +
                    "YWxwaGFCQm94IjogWwogICAgICAgICAgNiwKICAgICAgICAgIDEsCiAgICAgICAgICAxMCwKICAgICAgICAgIDE3CiAgICAg" +
                    "ICAgXSwKICAgICAgICAidmlzaWJsZVBpeGVscyI6IDQyLAogICAgICAgICJyZ2JhU2hhMjU2IjogIjg4YjhlNzA2MmYwZWI5" +
                    "Y2ZiNzg4ZWYzNjU4MDI4YmE1OGE2ZjYyZDQwOGQzZTM5YWUyZTEyMjhiZmE2Yzg5MDUiLAogICAgICAgICJwbmdTaGEyNTYi" +
                    "OiAiMDhiNTM2OTllNDU5ZDcxY2Y3M2YzZTdiZTg0YWYxOWQwNDgwNWQ1YTdhNDg4MTIxZTM5YTUxNGE4MTcwMTk5OSIsCiAg" +
                    "ICAgICAgInBuZ0J5dGVzIjogMTM1CiAgICAgIH0sCiAgICAgICJyb3V0ZSI6ICJyZXVzZWQtbmF0aXZlLXBpeGVscyIsCiAg" +
                    "ICAgICJ2YWxpZGF0aW9uIjogewogICAgICAgICJzb3VyY2UiOiAidmVyaWZpZWQtZXhwb3J0IiwKICAgICAgICAiaGVhZGxl" +
                    "c3MiOiAicnVuLXNvdXJjZS50cmFjZXItZm9yLWN1cnJlbnQtZXZpZGVuY2UiLAogICAgICAgICJ2aXN1YWwiOiAicGVuZGlu" +
                    "Zy0wNC0zMiIKICAgICAgfSwKICAgICAgIm9ibGlnYXRpb25zIjogWwogICAgICAgICJJVC0wNyIKICAgICAgXSwKICAgICAg" +
                    "Im9ibGlnYXRpb25TdGF0dXMiOiAicGFydGlhbC1tYXN0ZXItb25seSIKICAgIH0KICBdCn0K";

    private const string LegacyScene =
            "W2dkX3NjZW5lIGxvYWRfc3RlcHM9MiBmb3JtYXQ9M10KCltleHRfcmVzb3VyY2UgdHlwZT0iVGV4dHVyZTJEIiBwYXRoPSJy" +
                    "ZXM6Ly9hc3NldHMvY29tbW9uL2Jvdy5wbmciIGlkPSIxX2JvdyJdCgpbbm9kZSBuYW1lPSJJbnNwZWN0aW9uIiB0eXBlPSJO" +
                    "b2RlMkQiXQoKW25vZGUgbmFtZT0iQm93IiB0eXBlPSJTcHJpdGUyRCIgcGFyZW50PSIuIl0KdGV4dHVyZV9maWx0ZXIgPSAx" +
                    "CnBvc2l0aW9uID0gVmVjdG9yMig2NDAsIDM2MCkKdGV4dHVyZSA9IEV4dFJlc291cmNlKCIxX2JvdyIpCmNlbnRlcmVkID0g" +
                    "ZmFsc2UKb2Zmc2V0ID0gVmVjdG9yMigtNiwgLTkpCgpbbm9kZSBuYW1lPSJTdW1tYXJ5IiB0eXBlPSJWQm94Q29udGFpbmVy" +
                    "IiBwYXJlbnQ9Ii4iXQpvZmZzZXRfbGVmdCA9IDI0LjAKb2Zmc2V0X3RvcCA9IDI0LjAKb2Zmc2V0X3JpZ2h0ID0gNzIwLjAK" +
                    "b2Zmc2V0X2JvdHRvbSA9IDE4NC4wCgpbbm9kZSBuYW1lPSJUaXRsZSIgdHlwZT0iTGFiZWwiIHBhcmVudD0iU3VtbWFyeSJd" +
                    "CmxheW91dF9tb2RlID0gMgp0ZXh0ID0gIktpbmdkb21ab21iaWVzIOKAlCBSZXNvdXJjZSBpbnNwZWN0aW9uIgoKW25vZGUg" +
                    "bmFtZT0iUmVzb3VyY2VJZCIgdHlwZT0iTGFiZWwiIHBhcmVudD0iU3VtbWFyeSJdCmxheW91dF9tb2RlID0gMgp0ZXh0ID0g" +
                    "ImNvbW1vbi5ib3ciCgpbbm9kZSBuYW1lPSJSZXNvdXJjZU5hbWUiIHR5cGU9IkxhYmVsIiBwYXJlbnQ9IlN1bW1hcnkiXQps" +
                    "YXlvdXRfbW9kZSA9IDIKdGV4dCA9ICJCb3ciCgpbbm9kZSBuYW1lPSJTdGF0dXMiIHR5cGU9IkxhYmVsIiBwYXJlbnQ9IlN1" +
                    "bW1hcnkiXQpsYXlvdXRfbW9kZSA9IDIKdGV4dCA9ICJTb3VyY2UvaGVhZGxlc3MgdmVyaWZpZWQ7IHZpc3VhbCByZXZpZXcg" +
                    "cGVuZGluZyIKCltub2RlIG5hbWU9IlBhY2tTdGF0dXMiIHR5cGU9IkxhYmVsIiBwYXJlbnQ9IlN1bW1hcnkiXQpsYXlvdXRf" +
                    "bW9kZSA9IDIKdGV4dCA9ICJQYWNrIGluY29tcGxldGUg4oCUIHRyYWNlciBtYXN0ZXIgb25seSIK";

    private const string InspectionPath = "src/godot-game/inspection/InspectionRoot.cs";
    private const string Native = "Godot.GD.Print(what: [1]);";

    private static void CheckMember(string type, int index)
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        string path = "src/godot-game/inspection/" + type + ".cs";
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: path));
        SyntaxTree tree = consumer.Parse(source: source, path: path);
        BaseMethodDeclarationSyntax[] methods = [.. tree.GetRoot(cancellationToken:
                TestContext.Current.CancellationToken).DescendantNodes().OfType<BaseMethodDeclarationSyntax>()];
        methods.Length.ShouldBe(expected: type switch
        {
            "InspectionRoot" => 18,
            "InspectionInput" => 3,
            "InspectionShell" => 13,
            _ => throw new InvalidOperationException(),
        });
        BaseMethodDeclarationSyntax method = type is "InspectionRoot" ? methods[index]
                : methods.OfType<MethodDeclarationSyntax>().ElementAt(index: index);
        CheckInvocation(consumer: consumer, source: source, method: method, path: path);
    }

    private static string Hash(byte[] bytes)
    {
        return Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes));
    }

    private static void CheckInvocation(ConsumerCompilation consumer, string source,
            BaseMethodDeclarationSyntax method, string path)
    {
        string suffix = method is MethodDeclarationSyntax ordinary && ordinary.Identifier.ValueText is "Read"
                ? "return null;" : "";
        const string Guard = "_gateway.RequireThread();";
        CSharpCompilation basis = consumer.Create(source: source, path: path);
        SyntaxTree original = basis.SyntaxTrees.Single(predicate: tree => StringComparer.Ordinal.Equals(
                x: tree.FilePath, y: path));
        string positive = Body(source: source, method: method, body: Guard + Native + suffix);
        CheckSubject(consumer: consumer, basis: basis, original: original, source: positive).ShouldBeEmpty();
        foreach (string body in new[]
        {
            Native + suffix,
            Native + Guard + suffix,
            "if (false) { " + Guard + " }" + Native + suffix,
            "goto Skip; " + Guard + " Skip: " + Native + suffix,
            "new KingdomZombies.Proofs.MainThreadGateway().RequireThread();" + Native + suffix,
            Guard + "System.Action misuse = new Godot.Node().QueueFree;" + suffix,
            Guard + "System.Action misuse = () => { " + Native + " }; misuse();" + suffix,
            Guard + "void Local() { " + Native + " } Local();" + suffix,
            Guard + "System.Threading.Tasks.Task.Run(action: () => { " + Native + " });" + suffix,
            Guard + "System.Threading.Tasks.Task.CompletedTask.ContinueWith(continuationAction: task => { "
                    + Native + " });" + suffix,
        })
        {
            CheckSubject(consumer: consumer, basis: basis, original: original,
                    source: Body(source: source, method: method, body: body)).ShouldNotBeEmpty(
                            customMessage: "Exact invocation boundary: " + method.ToString());
        }

        CheckSubject(consumer: consumer, basis: basis, original: original,
                source: positive.Replace(oldValue: "readonly MainThreadGateway _gateway",
                        newValue: "MainThreadGateway _gateway", comparisonType: StringComparison.Ordinal))
                .ShouldNotBeEmpty();
    }

    private static System.Collections.Generic.IReadOnlyList<string> CheckSubject(ConsumerCompilation consumer,
            CSharpCompilation basis, SyntaxTree original, string source)
    {
        SyntaxTree changed = consumer.Parse(source: source, path: original.FilePath);
        CSharpCompilation compilation = basis.ReplaceSyntaxTree(oldTree: original, newTree: changed);
        ConsumerCompilation.RequireValid(compilation: compilation);
        return EngineAccessPolicy.CheckTree(tree: changed, compilation: compilation);
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
        if (path is "content/pack-index.json")
        {
            return Convert.FromBase64String(s: LegacyIndex);
        }

        if (path is "content/packs/tracer.json")
        {
            return Convert.FromBase64String(s: LegacyTracer);
        }

        if (path is "inspection/Inspection.tscn")
        {
            return Convert.FromBase64String(s: LegacyScene);
        }

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
