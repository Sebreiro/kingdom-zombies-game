using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves actual completed content boundaries independently of historical partial-pack subjects.</summary>
public sealed class ContentNativePermissionTests
{
    /// <summary>The real canonical common shard is authenticated and parsed without raising other limits.</summary>
    [Fact]
    public void Load_ActualCommon_AuthenticatesCanonicalTwoMiBDocument()
    {
        byte[] source = Read(path: "content/packs/common.json") ?? throw new InvalidOperationException();
        source.Length.ShouldBe(expected: 1_793_375);
        ContentPack actual = Common(bytes: source, path: "content/packs/common.json", authenticate: true);
        actual.Diagnostics.Where(predicate: error => error.StartsWith(value: "common:",
                comparisonType: StringComparison.Ordinal)).ShouldBeEmpty();
        actual.Entries.ContainsKey(key: "common.bow").ShouldBeTrue();
        actual.Status.ShouldContain(expected: "pending");
    }

    /// <summary>Only an authenticated exact common path receives its measured document exception.</summary>
    [Theory]
    [InlineData("content/packs/common.json", 2_097_152, true, true)]
    [InlineData("content/packs/common.json", 2_097_153, true, false)]
    [InlineData("content/packs/Common.json", 2_097_152, true, false)]
    [InlineData("content/packs/common-copy.json", 2_097_152, true, false)]
    [InlineData("content/packs/common.json", 2_097_152, false, false)]
    public void Load_CommonSize_RequiresExactAuthenticatedPath(string path, int size, bool authenticate,
            bool accepted)
    {
        byte[] source = Read(path: "content/packs/common.json") ?? throw new InvalidOperationException();
        byte[] padded = new byte[size];
        Array.Fill(array: padded, value: (byte)' ');
        source.CopyTo(array: padded, index: 0);
        ContentPack pack = Common(bytes: padded, path: path, authenticate: authenticate);
        bool failed = pack.Diagnostics.Any(predicate: error => error.StartsWith(value: "common:",
                comparisonType: StringComparison.Ordinal));
        failed.ShouldBe(expected: !accepted);
        pack.Entries.ContainsKey(key: "common.bow").ShouldBeTrue();
    }

    private static ContentPack Common(byte[] bytes, string path, bool authenticate)
    {
        JsonObject index = JsonNode.Parse(utf8Json: Read(path: "content/pack-index.json") ?? [])?.AsObject()
                ?? throw new InvalidOperationException();
        JsonNode row = index["shards"]?.AsArray().Single(predicate: item => StringComparer.Ordinal.Equals(
                x: item?["id"]?.GetValue<string>(), y: "common")) ?? throw new InvalidOperationException();
        row["status"] = "selected";
        row["version"] = 1;
        row["path"] = "src/godot-game/" + path;
        row["bytes"] = bytes.Length;
        row["sha256"] = authenticate ? Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes))
                : new string(c: '0', count: 64);
        return ContentPack.Load(index: Encoding.UTF8.GetBytes(s: index.ToJsonString()), expectations:
                Read(path: ContentPack.ExpectedPath), read: name => StringComparer.Ordinal.Equals(x: name,
                        y: path) ? bytes : Read(path: name));
    }

    private static byte[]? Read(string path)
    {
        string file = Path.Combine(path1: ProofProtocol.Root, path2: "src/godot-game/" + path);
        return File.Exists(path: file) ? File.ReadAllBytes(path: file) : null;
    }
}
