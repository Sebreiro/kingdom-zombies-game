using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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

    /// <summary>Damage changes never prorate a paid full-health repair order's fixed tier valuation fee.</summary>
    [Theory]
    [InlineData(false, 1, 0.1, 10)]
    [InlineData(false, 1, 0.9, 10)]
    [InlineData(false, 2, 0.1, 40)]
    [InlineData(false, 2, 0.9, 40)]
    [InlineData(false, 3, 0.1, 110)]
    [InlineData(false, 3, 0.9, 110)]
    [InlineData(true, 1, 0.1, 20)]
    [InlineData(true, 1, 0.9, 20)]
    [InlineData(true, 2, 0.1, 65)]
    [InlineData(true, 2, 0.9, 65)]
    [InlineData(true, 3, 0.1, 155)]
    [InlineData(true, 3, 0.9, 155)]
    public void Repair_ActualDefensePrice_IsIndependentOfDamage(bool tower, int tier, double health, int fee)
    {
        string file = Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll");
        Type type = Assembly.Load(rawAssembly: File.ReadAllBytes(path: file)).GetType(
                name: "KingdomZombies.Inspection.BuildingPreview", throwOnError: true)
                ?? throw new InvalidOperationException();
        object owner = RuntimeHelpers.GetUninitializedObject(type: type);
        using var assemblies = JsonDocument.Parse(utf8Json: Read(
                path: "content/world/defense-assemblies.json") ?? []);
        Set(type: type, owner: owner, name: "_assemblies", value: assemblies.RootElement.Clone());
        Set(type: type, owner: owner, name: "_defenseTower", value: tower);
        Set(type: type, owner: owner, name: "_completedTier", value: tier);
        using var sample = JsonDocument.Parse(json: "{\"work\":\"repair\",\"hpRatio\":"
                + health.ToString(provider: System.Globalization.CultureInfo.InvariantCulture) + "}");
        MethodInfo method = type.GetMethod(name: "DefenseWorkPrice", bindingAttr:
                BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException();
        method.Invoke(obj: owner, parameters: [assemblies.RootElement.GetProperty(
                propertyName: "tierDefinitions")[(tower ? 3 : 0) + tier - 1], sample.RootElement])
                .ShouldBe(expected: fee);
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

    private static void Set(Type type, object owner, string name, object value)
    {
        FieldInfo field = type.GetField(name: name, bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException();
        field.SetValue(obj: owner, value: value);
    }

    private static byte[]? Read(string path)
    {
        string file = Path.Combine(path1: ProofProtocol.Root, path2: "src/godot-game/" + path);
        return File.Exists(path: file) ? File.ReadAllBytes(path: file) : null;
    }
}
