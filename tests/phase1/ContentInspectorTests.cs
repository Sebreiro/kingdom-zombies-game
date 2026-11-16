using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises the real portable pack loader independently of a native game window.</summary>
public sealed class ContentInspectorTests
{
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
