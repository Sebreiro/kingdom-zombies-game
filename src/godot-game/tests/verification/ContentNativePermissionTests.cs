using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using KingdomZombies.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                path2: "src/godot-game/game/.godot/mono/temp/bin/Debug/KingdomZombies.dll");
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
        object? observed = method.Invoke(obj: owner, parameters: [assemblies.RootElement.GetProperty(
                propertyName: "tierDefinitions")[(tower ? 3 : 0) + tier - 1], sample.RootElement]);
        string directory = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY")
                ?? Path.GetTempPath();
        using (var output = new FileStream(path: Path.Combine(path1: directory,
                path2: "repair-observation-" + Guid.NewGuid().ToString(format: "N") + ".json"),
                mode: FileMode.CreateNew, access: FileAccess.Write))
        {
            JsonSerializer.Serialize(utf8Json: output, value: new
            {
                tower,
                tier,
                health,
                observed,
                assemblySha256 = Convert.ToHexStringLower(inArray: SHA256.HashData(
                        source: File.ReadAllBytes(path: file))),
                gameMvid = type.Assembly.ManifestModule.ModuleVersionId,
                coreMvid = typeof(ContentPack).Assembly.ManifestModule.ModuleVersionId,
            });
            output.Flush(flushToDisk: true);
        }

        observed.ShouldBe(expected: fee);
    }

    /// <summary>Exercises the actual finite typed decoder against current independently mapped
    /// production files.</summary>
    [Fact]
    public void Load_CurrentTypedModel_PreservesIndependentDefinitionsAndPendingAcceptance()
    {
        string directory = Path.Combine(path1: WorkspacePaths.Temp(purpose: "asset-pipeline"),
                path2: "04-23-current-model");
        byte[] model = File.ReadAllBytes(path: Path.Combine(path1: directory, path2: "normalized-index.json"));
        var actual = ContentPack.Load(index: model, expectations: Read(path: ContentPack.ExpectedPath),
                read: Read);
        var observation = new
        {
            scope = "Actual compiled current model decoder; source semantic and native policy proof separate",
            available = 180,
            runtimeDefinitions = actual.Entries.Count,
            accepted = 0,
            expected = actual.Expected.Count,
            status = actual.Status,
            entries = actual.Entries.Keys.Order(comparer: StringComparer.Ordinal).ToArray(),
            diagnostics = actual.Diagnostics,
            modelSha256 = Convert.ToHexStringLower(inArray: SHA256.HashData(source: model)),
            coreSha256 = Convert.ToHexStringLower(inArray: SHA256.HashData(source: File.ReadAllBytes(
                    path: typeof(ContentPack).Assembly.Location))),
        };
        string report = Path.Combine(path1: Environment.GetEnvironmentVariable(
                variable: "PHASE1_RESULT_DIRECTORY") ?? directory,
                path2: "core-observation-" + Guid.NewGuid().ToString(format: "N") + ".json");
        using (var output = new FileStream(path: report, mode: FileMode.CreateNew, access: FileAccess.Write))
        {
            JsonSerializer.Serialize(utf8Json: output, value: observation);
            output.Flush(flushToDisk: true);
        }
        actual.Expected.Count.ShouldBe(expected: 180);
        actual.Entries.Count.ShouldBe(expected: 180);
        actual.Entries.Keys.Order(comparer: StringComparer.Ordinal).ShouldBe(expected: actual.Expected.Keys
                .Order(comparer: StringComparer.Ordinal));
        actual.Status.ShouldContain(expected: "pending");
    }

    /// <summary>Only six exact current names receive their existing canonical case allowance.</summary>
    [Theory]
    [InlineData("assets/ui/LiberationSans.ttf")]
    [InlineData("assets/ui/Fallback.ttf")]
    [InlineData("assets/ui/PerfectDOSVGA437.ttf")]
    [InlineData("assets/ui/InspectionTheme.tres")]
    [InlineData("inspection/ItemsWeaponsPreview.tscn")]
    [InlineData("inspection/ItemsWearablesPreview.tscn")]
    public void ResourcePath_CurrentCanonical_RejectsAdjacentAliases(string path)
    {
        ContentPack.ResourcePath(path: "src/godot-game/" + path).ShouldBe(expected: path);
        foreach (string alias in new[] { path.ToLowerInvariant(), path.ToUpperInvariant(),
                path.Replace(oldValue: ".", newValue: "-copy.", comparisonType: StringComparison.Ordinal),
                path.Replace(oldValue: "/", newValue: "%2f", comparisonType: StringComparison.Ordinal),
                path + "/../other" })
        {
            Action subject = () => ContentPack.ResourcePath(path: "src/godot-game/" + alias);
            subject.ShouldThrow<FormatException>();
        }

        JsonObject model = Model();
        JsonNode file = ModelFile(model: model, path: path);
        file["sha256"] = new string(c: '0', count: 64);
        Loaded(model: model).Diagnostics.ShouldContain(elementPredicate: error => error.StartsWith(
                value: NodeField(owner: file, key: "id").GetValue<string>() + ":",
                comparisonType: StringComparison.Ordinal)
                && error.Contains(value: "bytes/hash mismatch", comparisonType: StringComparison.Ordinal));
        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Known heterogeneous headers use explicit bounded publication authority
    /// and required structure.</summary>
    [Theory]
    [InlineData("content/ui/settings-errors.json", "samples")]
    [InlineData("content/schema/fields-foundation.json", "files")]
    [InlineData("content/schema/fields-items-combat.json", "files")]
    [InlineData("content/ui/session-save-outcomes.json", "samples")]
    [InlineData("content/fixtures/audio-states.json", "states")]
    [InlineData("content/ui/camp-panels.json", "samples")]
    [InlineData("content/ui/map-spectator-warnings.json", "samples")]
    [InlineData("content/ui/legacy-stash-targets.json", "samples")]
    public void Load_KnownHeader_RejectsReboundInvalidAuthority(string path, string required)
    {
        foreach (string kind in new[] { "schema-future", "schema-type", "body-zero", "body-incoherent",
                "body-type", "publication-zero", "publication-unsafe", "missing-structure", "wrong-valid-id" })
        {
            JsonObject model = Model();
            JsonObject body = Definition(path: path);
            JsonNode file = ModelFile(model: model, path: path);
            HeaderDefect(body: body, file: file, kind: kind, required: required);
            byte[] changed = Encoding.UTF8.GetBytes(s: body.ToJsonString());
            Rebind(file: file, bytes: changed);
            ContentPack result = Loaded(model: model, replacementPath: path, replacement: changed);
            result.Diagnostics.ShouldContain(elementPredicate: error => error.StartsWith(
                    value: NodeField(owner: file, key: "id").GetValue<string>() + ":",
                    comparisonType: StringComparison.Ordinal));
            result.Expected.Count.ShouldBe(expected: 180);
        }

        JsonObject positive = Model();
        JsonObject definition = Definition(path: path);
        JsonNode reference = ModelFile(model: positive, path: path);
        reference["version"] = 2;
        if (definition.ContainsKey(propertyName: "version"))
        {
            definition["version"] = 2;
        }

        byte[] bytes = definition.ContainsKey(propertyName: "version")
                ? Encoding.UTF8.GetBytes(s: definition.ToJsonString()) : Read(path: path)
                        ?? throw new InvalidOperationException();
        Rebind(file: reference, bytes: bytes);
        HeaderPositive(model: positive, path: path, bytes: bytes).Entries.Count.ShouldBe(expected: 180);
        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>The actual icon rejects rebound malformed directory/frame subjects
    /// and restores its exact bytes.</summary>
    [Fact]
    public void Load_CurrentIcon_RejectsReboundDirectoryAndPngDefects()
    {
        const string Path = "assets/ui/kingdom-zombies.ico";
        byte[] original = Read(path: Path) ?? [];
        original.Length.ShouldBe(expected: 13342);
        foreach (string kind in new[] { "truncated", "type", "count", "dimension", "offset", "overflow",
                "overlap", "png-dimension", "png-crc", "png-content", "wrong-hash" })
        {
            byte[] changed = IconDefect(original: original, kind: kind);
            JsonObject model = Model();
            JsonNode file = ModelFile(model: model, path: Path);
            Rebind(file: file, bytes: changed);
            if (kind is "wrong-hash")
            {
                file["sha256"] = new string(c: '0', count: 64);
            }

            ContentPack result = Loaded(model: model, replacementPath: Path, replacement: changed);
            result.Diagnostics.ShouldContain(elementPredicate: error => error.StartsWith(
                    value: NodeField(owner: file, key: "id").GetValue<string>() + ":",
                    comparisonType: StringComparison.Ordinal));
        }

        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Actual media decoding rejects rebound corrupt containers independently of byte hashes.</summary>
    [Theory]
    [InlineData("assets/common/bow.png")]
    [InlineData("assets/audio/actions.wav")]
    [InlineData("assets/ui/LiberationSans.ttf")]
    public void Load_CurrentMedia_RejectsReboundSemanticDefects(string path)
    {
        byte[] original = Read(path: path) ?? [];
        foreach (string kind in new[] { "truncated", "extent", "format", "payload" })
        {
            byte[] changed = MediaDefect(original: original, path: path, kind: kind);
            JsonObject model = Model();
            JsonNode file = ModelFile(model: model, path: path);
            Rebind(file: file, bytes: changed);
            ContentPack result = Loaded(model: model, replacementPath: path, replacement: changed);
            result.Diagnostics.ShouldContain(elementPredicate: error => error.StartsWith(
                    value: NodeField(owner: file, key: "id").GetValue<string>() + ":",
                    comparisonType: StringComparison.Ordinal));
            result.Expected.Count.ShouldBe(expected: 180);
        }

        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Valid alternate pixels cannot impersonate another retained atlas after index rebinding.</summary>
    [Fact]
    public void Load_CurrentAtlas_RejectsWrongValidMediaWithReboundFileHash()
    {
        const string Target = "assets/buildings/main-tiers.png";
        byte[] alternate = Read(path: "assets/buildings/defenses.png") ?? [];
        JsonObject model = Model();
        JsonNode file = ModelFile(model: model, path: Target);
        Rebind(file: file, bytes: alternate);
        ContentPack result = Loaded(model: model, replacementPath: Target, replacement: alternate);
        result.Diagnostics.ShouldContain(elementPredicate: error => error.Contains(
                value: "actual dependency hash mismatch /atlas", comparisonType: StringComparison.Ordinal));
        result.Entries.ContainsKey(key: "BL-01").ShouldBeFalse();
        result.Entries.ContainsKey(key: "CH-01").ShouldBeTrue();
        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>The existing interface separates packaged reads from repository sources
    /// and keeps queries exact.</summary>
    [Fact]
    public void Load_CurrentQueries_PreserveRuntimeRolesAndIndependentMappings()
    {
        JsonObject model = Model();
        ContentPack result = Loaded(model: model);
        string[] paths = [.. ContentPack.Paths(index: Encoding.UTF8.GetBytes(s: model.ToJsonString()))];
        paths.ShouldContain(elementPredicate: path => StringComparer.Ordinal.Equals(
                x: path, y: "assets/common/bow.png"));
        paths.ShouldContain(elementPredicate: path => StringComparer.Ordinal.Equals(
                x: path, y: "assets/ui/kingdom-zombies.ico"));
        paths.ShouldNotContain(elementPredicate: path => path.StartsWith(value: "production/",
                comparisonType: StringComparison.Ordinal) || path.StartsWith(value: "docs/",
                        comparisonType: StringComparison.Ordinal));
        paths.Distinct(comparer: StringComparer.Ordinal).Count().ShouldBe(expected: paths.Length);
        result.Search(query: "", category: "Audio", status: "Partial").Count.ShouldBe(expected: 30);
        result.Search(query: "independently-absent", category: "", status: "").ShouldBeEmpty();
        foreach (string id in result.Expected.Keys)
        {
            ContentPack.Mappings(entry: result.Entries[id]).ShouldBe(expected: new[] { id });
        }

        result.Diagnostics.Count(predicate: error => error.Contains(value: ", field acceptance,",
                comparisonType: StringComparison.Ordinal)).ShouldBe(expected: 180);
        result.Status.ShouldContain(expected: "pending");
    }

    /// <summary>Exact stale producer bindings remain semantic failures after their outer bytes are rebound.</summary>
    [Theory]
    [InlineData("content/fixtures/world-states.json", "bindings", 8,
            "1ca169e2bd8818fac84b4ff19d282aa9d6ed8b19b91adb70a830ef100c51ae1e")]
    [InlineData("content/fixtures/world-states.json", "bindings", 9,
            "e8ec43336aa2c79c8b52539bc300d5918c3acc75df8b0506b71caa1f2ef26787")]
    [InlineData("content/packs/player-identities.json", "sourceBindings", 0,
            "324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed")]
    [InlineData("content/packs/ui-inventory.json", "bindings", 2,
            "324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed")]
    [InlineData("content/packs/ui-legacy-stash.json", "bindings", 2,
            "324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed")]
    [InlineData("content/packs/ui-camp.json", "bindings", 4,
            "5d0dbde0ed6964996f3c5fbab4334c5c50d732c3e08d82515e2fcd89fe0ad3a1")]
    [InlineData("content/packs/ui-camp.json", "bindings", 5,
            "324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed")]
    [InlineData("content/packs/ui-map-warnings.json", "bindings", 1,
            "5d0dbde0ed6964996f3c5fbab4334c5c50d732c3e08d82515e2fcd89fe0ad3a1")]
    [InlineData("content/packs/ui-map-warnings.json", "bindings", 2,
            "324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed")]
    [InlineData("content/fixtures/crowds.json", "bindings", 1,
            "c482e9a52b796b416b11b7bb73fa49aa6926c3b85bcb15cb47ed8cbccc4f93cf")]
    public void Load_CurrentDependency_RejectsReboundStaleSource(string path, string field, int index,
            string stale)
    {
        JsonObject body = Definition(path: path);
        JsonNode binding = NodeField(owner: body, key: field)[index]
                ?? throw new InvalidOperationException(message: "Exact binding missing");
        string current = NodeField(owner: binding, key: "sha256").GetValue<string>();
        StringComparer.Ordinal.Equals(x: current, y: stale).ShouldBeFalse();
        foreach (string digest in new[] { stale, new string(c: '0', count: 64) })
        {
            binding["sha256"] = digest;
            byte[] changed = Encoding.UTF8.GetBytes(s: body.ToJsonString());
            JsonObject model = Model();
            JsonNode file = ModelFile(model: model, path: path);
            Rebind(file: file, bytes: changed);
            ContentPack result = Loaded(model: model, replacementPath: path, replacement: changed);
            result.Diagnostics.ShouldContain(elementPredicate: error => error.StartsWith(
                    value: NodeField(owner: file, key: "id").GetValue<string>() + ":",
                    comparisonType: StringComparison.Ordinal) && error.Contains(
                            value: "actual dependency hash mismatch /" + field + "/" + index.ToString(
                                    provider: System.Globalization.CultureInfo.InvariantCulture),
                            comparisonType: StringComparison.Ordinal));
        }

        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Independent census identities and selectors cannot be inflated or redirected by valid JSON.</summary>
    [Theory]
    [InlineData("duplicate-output")]
    [InlineData("missing-output")]
    [InlineData("shared-identity")]
    [InlineData("wrong-contract")]
    [InlineData("wrong-valid-selector")]
    [InlineData("selector-order")]
    [InlineData("dependency-missing")]
    [InlineData("dependency-order")]
    [InlineData("unsafe-version")]
    [InlineData("runtime-role")]
    [InlineData("duplicate-file")]
    public void Load_CurrentModel_RejectsIndependentIdentityAndDependencyDefects(string kind)
    {
        JsonObject model = Model();
        JsonArray outputs = NodeField(owner: model, key: "outputs").AsArray();
        JsonNode target = outputs.Single(predicate: row => row is not null
                && StringComparer.Ordinal.Equals(x: NodeField(owner: row, key: "id").GetValue<string>(),
                        y: kind is "selector-order"
                                ? "DT-26" : "CH-01")) ?? throw new InvalidOperationException();
        ModelDefect(model: model, target: target, kind: kind);
        ContentPack result = Loaded(model: model);
        string id = NodeField(owner: target, key: "id").GetValue<string>();
        result.Entries.ContainsKey(key: id).ShouldBeFalse();
        result.Entries.ContainsKey(key: "CH-08").ShouldBeTrue();
        result.Expected.Count.ShouldBe(expected: 180);
        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Actual foreign-atlas regions retain source identities and reject rebound semantic corruption.</summary>
    [Theory]
    [InlineData("theme-identity", "/theme/frames/0")]
    [InlineData("theme-valid-source", "/theme/frames/0")]
    [InlineData("theme-member", "/theme/frames/0")]
    [InlineData("theme-png", "/theme/frames/0")]
    [InlineData("theme-rgba", "/theme/frames/0")]
    [InlineData("theme-region", "/theme/frames/0")]
    [InlineData("theme-unsafe", "/theme/frames/0")]
    [InlineData("theme-outside", "/theme/frames/0")]
    [InlineData("theme-negative", "/theme/frames/0")]
    [InlineData("item-identity", "/items/17")]
    [InlineData("item-texture", "/items/17")]
    [InlineData("item-icon", "/items/17")]
    [InlineData("item-rgba", "/items/17")]
    [InlineData("item-region", "/items/17")]
    [InlineData("item-unsafe", "/items/17")]
    [InlineData("item-outside", "/items/17")]
    [InlineData("item-string", "/items/17")]
    [InlineData("local-outside", "/regions/")]
    [InlineData("source-dependency", "contextual authenticated source definition unavailable")]
    public void Load_ContextualRegion_RejectsReboundWrongSourceAndCoordinates(string kind, string reason)
    {
        const string Path = "content/packs/ui-identity.json";
        JsonObject body = Definition(path: Path);
        JsonObject model = Model();
        ContextDefect(body: body, model: model, kind: kind);
        byte[] changed = Encoding.UTF8.GetBytes(s: body.ToJsonString());
        JsonNode file = ModelFile(model: model, path: Path);
        Rebind(file: file, bytes: changed);
        ContentPack actual = Loaded(model: model, replacementPath: Path, replacement: changed);
        actual.Diagnostics.ShouldContain(elementPredicate: diagnostic => diagnostic.StartsWith(
                value: NodeField(owner: file, key: "id").GetValue<string>() + ":",
                comparisonType: StringComparison.Ordinal) && diagnostic.Contains(
                        value: reason, comparisonType: StringComparison.Ordinal));
        actual.Entries.ContainsKey(key: "UI-03").ShouldBeFalse();
        actual.Entries.ContainsKey(key: "DT-01").ShouldBeTrue();
        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Foreign source regions and local identity symbols remain real independent resources.</summary>
    [Fact]
    public void Load_ContextualRegions_PreservesAllSourcesAndRuntimePaths()
    {
        JsonObject model = Model();
        ContentPack actual = Loaded(model: model);
        actual.Entries.Count.ShouldBe(expected: 180);
        actual.Expected.Count.ShouldBe(expected: 180);
        actual.Entries.ContainsKey(key: "UI-03").ShouldBeTrue();
        ContentPack.Paths(index: Encoding.UTF8.GetBytes(s: model.ToJsonString())).Count.ShouldBe(expected: 102);
        actual.Diagnostics.Where(predicate: diagnostic => diagnostic.StartsWith(value: "file.",
                comparisonType: StringComparison.Ordinal)).ShouldBeEmpty();
        JsonObject body = Definition(path: "content/packs/ui-identity.json");
        NodeField(owner: NodeField(owner: body, key: "theme"), key: "frames").AsArray().Count
                .ShouldBe(expected: 4);
        NodeField(owner: body, key: "items").AsArray().Count.ShouldBe(expected: 24);
        actual.Status.ShouldContain(expected: "pending");
    }

    /// <summary>Rebound font subjects reach actual checksum, mapping and outline guards.</summary>
    [Theory]
    [InlineData("table-checksum", "table checksum")]
    [InlineData("whole-checksum", "whole-font checksum")]
    [InlineData("cmap-offset", "cmap duplicate encoding/subtable offset")]
    [InlineData("cmap-glyph", "nonexistent glyph")]
    [InlineData("cmap-order", "segment ordering")]
    [InlineData("loca", "loca ordering")]
    [InlineData("metrics", "glyph/metric count")]
    [InlineData("glyph-reference", "composite glyph reference")]
    [InlineData("glyph-cycle", "glyph dependency cycle")]
    [InlineData("glyph-contours", "contour/instruction extent")]
    public void Load_FontSemantics_RejectsReboundInvalidMappingsAndOutlines(string kind, string reason)
    {
        const string Path = "assets/ui/LiberationSans.ttf";
        byte[] changed = FontDefect(original: Read(path: Path) ?? [], kind: kind);
        JsonObject model = Model();
        JsonNode file = ModelFile(model: model, path: Path);
        Rebind(file: file, bytes: changed);
        ContentPack actual = Loaded(model: model, replacementPath: Path, replacement: changed);
        GuardObservation(actual: actual, path: Path, kind: kind);
        actual.Diagnostics.ShouldContain(elementPredicate: error => error.StartsWith(
                value: NodeField(owner: file, key: "id").GetValue<string>() + ":",
                comparisonType: StringComparison.Ordinal) && error.Contains(
                        value: reason, comparisonType: StringComparison.Ordinal));
        actual.Entries.ContainsKey(key: "DT-01").ShouldBeTrue();
        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Each retained font participates in the actual coherent model
    /// through the production decoder.</summary>
    [Theory]
    [InlineData("assets/ui/LiberationSans.ttf", 350200)]
    [InlineData("assets/ui/Fallback.ttf", 350200)]
    [InlineData("assets/ui/PerfectDOSVGA437.ttf", 81192)]
    public void Load_ActualFonts_ValidatesCurrentBytesAndDependencies(string path, int expectedBytes)
    {
        byte[] source = Read(path: path) ?? [];
        source.Length.ShouldBe(expected: expectedBytes);
        ContentPack actual = Loaded(model: Model());
        GuardObservation(actual: actual, path: path, kind: "actual-retained-positive");
        actual.Entries.Count.ShouldBe(expected: 180);
        actual.Diagnostics.Where(predicate: error => error.StartsWith(value: "file.",
                comparisonType: StringComparison.Ordinal)).ShouldBeEmpty();
        actual.Status.ShouldContain(expected: "pending");
    }

    /// <summary>Another valid font or mapping cannot replace the retained embedded source silently.</summary>
    [Theory]
    [InlineData("font")]
    [InlineData("mapping")]
    public void Load_FontSource_RejectsWrongValidFontAndMapping(string kind)
    {
        const string Path = "assets/ui/LiberationSans.ttf";
        byte[] changed = kind is "font" ? Read(path: "assets/ui/PerfectDOSVGA437.ttf") ?? []
                : FontDefect(original: Read(path: Path) ?? [], kind: "valid-cmap");
        JsonObject model = Model();
        JsonNode source = ModelFile(model: model, path: Path);
        Rebind(file: source, bytes: changed);
        ContentPack actual = Loaded(model: model, replacementPath: Path, replacement: changed);
        GuardObservation(actual: actual, path: Path, kind: "wrong-valid-" + kind);
        actual.Diagnostics.Where(predicate: error => error.StartsWith(
                value: NodeField(owner: source, key: "id").GetValue<string>() + ":",
                comparisonType: StringComparison.Ordinal)).ShouldBeEmpty();
        actual.Diagnostics.ShouldContain(elementPredicate: error => error.Contains(
                value: "native actual embedded/source font bytes mismatch", comparisonType: StringComparison.Ordinal));
        actual.Entries.ContainsKey(key: "DT-01").ShouldBeTrue();
        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Native resource and embedded-font corruption fails at actual typed graph guards
    /// after rebinding.</summary>
    [Theory]
    [InlineData("missing-reference", "native resource reference unavailable")]
    [InlineData("source-texture", "native exact source texture/type mismatch")]
    [InlineData("region", "native bounded integer source texture region")]
    [InlineData("resource-type", "native node resource type mismatch")]
    [InlineData("node-parent", "native node parent/path unavailable")]
    [InlineData("connection", "native connection source/target/signal/method")]
    [InlineData("font-role", "native embedded-font source role mismatch")]
    [InlineData("font-bytes", "native actual embedded/source font bytes mismatch")]
    [InlineData("resource-cycle", "native resource dependency cycle")]
    public void Load_NativeLayout_RejectsReboundInvalidGraphAndFontSource(string kind, string reason)
    {
        string path = kind.StartsWith(value: "font-", comparisonType: StringComparison.Ordinal)
                || kind is "resource-cycle" ? "assets/ui/InspectionTheme.tres"
                : "inspection/ItemsWeaponsPreview.tscn";
        byte[] changed = NativeDefect(original: Read(path: path) ?? [], kind: kind);
        JsonObject model = Model();
        JsonNode file = ModelFile(model: model, path: path);
        Rebind(file: file, bytes: changed);
        ContentPack actual = Loaded(model: model, replacementPath: path, replacement: changed);
        GuardObservation(actual: actual, path: path, kind: kind);
        actual.Diagnostics.ShouldContain(elementPredicate: error => error.StartsWith(
                value: NodeField(owner: file, key: "id").GetValue<string>() + ":",
                comparisonType: StringComparison.Ordinal) && error.Contains(
                        value: reason, comparisonType: StringComparison.Ordinal));
        actual.Entries.ContainsKey(key: "DT-01").ShouldBeTrue();
        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Independent typed selectors bind actual domain and declared obligation identities.</summary>
    [Theory]
    [InlineData("content/config/items.json", "DT-04", "DT-05")]
    [InlineData("content/text/en.json", "TXT-01", "TXT-02")]
    [InlineData("content/packs/ui-inventory.json", "UI-05", "UI-12")]
    public void Load_SelectedDomain_RejectsReboundForeignIdentity(string path, string id, string other)
    {
        JsonObject body = Definition(path: path);
        if (path is "content/packs/ui-inventory.json")
        {
            NodeField(owner: body, key: "obligations")[0] = other;
        }
        else
        {
            body["id"] = other;
        }

        byte[] changed = Encoding.UTF8.GetBytes(s: body.ToJsonString());
        JsonObject model = Model();
        JsonNode file = ModelFile(model: model, path: path);
        Rebind(file: file, bytes: changed);
        ContentPack actual = Loaded(model: model, replacementPath: path, replacement: changed);
        GuardObservation(actual: actual, path: path, kind: "foreign-selected-identity-" + other);
        bool obligation = path is "content/packs/ui-inventory.json";
        string prefix = obligation ? id : NodeField(owner: file, key: "id").GetValue<string>();
        string reason = obligation ? "actual selected resource identity mismatch"
                : "exact named completed document identity required";
        actual.Diagnostics.ShouldContain(elementPredicate: error => error.StartsWith(value: prefix + ":",
                comparisonType: StringComparison.Ordinal) && error.Contains(
                        value: reason, comparisonType: StringComparison.Ordinal));
        actual.Entries.ContainsKey(key: id).ShouldBeFalse();
        actual.Entries.ContainsKey(key: "DT-01").ShouldBeTrue();
        Loaded(model: Model()).Entries.Count.ShouldBe(expected: 180);
    }

    /// <summary>Actual final native hosts require captured first guards and exact ownership.</summary>
    [Theory]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|A"
                    + "dmit(string,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|A"
                    + "pplyVolume(AudioStreamPlayer,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|B"
                    + "uild()")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|B"
                    + "utton(string,Action)")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|C"
                    + "lose()")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|C"
                    + "urrentMuted()")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|L"
                    + "imits()")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|O"
                    + "pen(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,string)")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|P"
                    + "lay()")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|R"
                    + "econcileLimits()")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|R"
                    + "efresh()")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|R"
                    + "emove(long)")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|S"
                    + "tart(string,string,JsonElement,bool,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|T"
                    + "ick(double)")]
    [InlineData(
            "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|V"
                    + "olume(string,double)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AddAccess(Node2D,int,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AddDefenseGeometry(Node2D,JsonElement,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AddDefenseGuards(Node2D,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AddDefenseSprite(Node,string,string,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AddGuards(Node2D,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AddSeatMark(Node)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AddSelector(Node,string,IEnumerable<string>)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AddSprite(Node,string,string,Vector2)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AddSurface()")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AimGuard(int,float,float,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|Animate()")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AnimateDefense()")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|Attach()")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|AttachDefense()")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|Detach()")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|LightSelected(long)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|Position(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|Project()")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|ProjectDefense()")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|ProjectDefenseMetadata(int,bool,bool,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|ProjectGuardPose(Node2D,int,float,int)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|ResetDefense()")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|Restore(int,int,double,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|RestoreDefense(int,int,double,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|SelectDefenseSample(int)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|SelectSample(int)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|SetDefenseTexture(Sprite2D,string,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|SetTexture(Sprite2D,string)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|ShowDefenseStatus(int,JsonElement,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|ShowStatus(int,JsonElement,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                    + "view|ZoomSelected(long)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Add"
                    + "Action(Node,string,string,string,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Add"
                    + "Icon(Node,string)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Add"
                    + "Label(Node,string,string,string)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Att"
                    + "ach(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Cle"
                    + "ar()")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Cyc"
                    + "le(JsonElement,int,long)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                    + "Controls(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                    + "Markers(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                    + "Point(Control,JsonElement,string)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                    + "Selected(long,long)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                    + "Terrain(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                    + "View(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Mod"
                    + "al(int,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Not"
                    + "ifications(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Peo"
                    + "ple(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Pro"
                    + "ject(byte[],long)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Ral"
                    + "ly(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Sal"
                    + "e(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Sit"
                    + "e(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Spe"
                    + "ctator(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|AddBody(Node2D,JsonElement,int,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|AddBowArms(Node2D,JsonElement,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|AddFittedSprite(Node2D,JsonElement,JsonElement,string,int,Vector2,Vector2,f"
                    + "loat,JsonElement,JsonElement,string)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|AddGuard(Node2D,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|AddSelector(Node,string,string,IEnumerable<string>)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|AddSpentArrow(Node2D,JsonElement,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|AnimationControls(Node)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Attach()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|BowPose(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|CancelWork()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|CloseComposition()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Contact(Node2D,JsonElement,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Detach()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|FitGeometry(JsonElement,JsonElement,JsonElement,string,string,JsonElement,V"
                    + "ector2)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Frame()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|GuardArms(Node2D,Image,JsonElement,Vector2,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|GuardArrow(Node2D,Vector2,int,float)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|GuardLayer(Node2D,string,Image,Vector2,int,float,Vector2)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|GuardLower(Node2D,JsonElement,int,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|ObligationChoice(string,string)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|OpenAttached(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,strin"
                    + "g)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|OpenComposition(Node2D,MainThreadGateway,Func<string, byte[]?>,string)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Play()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Point(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Project()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|ProjectBurst(Node2D,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|ProjectGear(Node2D,JsonElement,JsonElement,JsonElement,int,string)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|ProjectSpit(Node2D,JsonElement,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|ProjectWork(Node2D,JsonElement,JsonElement,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|ReconcileHands()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Reset()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|RestoreComposition(string,string,int,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|RetireArt()")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|SegmentOffset(bool)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|SelectObligation(string)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Selected(string)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|Texture(JsonElement,JsonElement,string)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|TryOpenAttached(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,st"
                    + "ring,Node2D)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|UpdateAim(bool)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|WorkPayload(Node2D,JsonElement,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|WorkPose(JsonElement,JsonElement,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|WorkSprite(Node2D,string,string,Vector2,int,float)")]
    [InlineData(
            "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                    + "review|WorkTool(Node2D,JsonElement,int)")]
    [InlineData(
            "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                    + "|Close()")]
    [InlineData(
            "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                    + "|CommonSprite(Node2D,string,string,Vector2)")]
    [InlineData(
            "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                    + "|Outline(Node2D)")]
    [InlineData(
            "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                    + "|Pile(Node2D,JsonElement,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                    + "|Project()")]
    [InlineData(
            "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                    + "|Retire()")]
    [InlineData(
            "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                    + "|Sprite(Node2D,JsonElement,JsonElement,string,Vector2)")]
    [InlineData(
            "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                    + "|Wildlife(Node2D,string,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|A"
                    + "ttach()")]
    [InlineData(
            "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|C"
                    + "lose()")]
    [InlineData(
            "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|I"
                    + "dentity(string)")]
    [InlineData(
            "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|P"
                    + "ublish()")]
    [InlineData(
            "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|R"
                    + "etireWildlife(Node2D)")]
    [InlineData(
            "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|S"
                    + "etNight(bool)")]
    [InlineData(
            "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|T"
                    + "exture(string)")]
    [InlineData(
            "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|W"
                    + "ildlife(string,string,double,int,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|W"
                    + "ildlifeOutline(Node2D,Sprite2D)")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|Build()")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|Close()")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|Geometry(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|Grid(Node,JsonElement,string)")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|Open(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,string,Node2D)")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|Project()")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|ProjectLibrary()")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|Restore()")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|RestoreBuilding(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|RestoreOwners(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                    + "view|Selector(Node,string,string[],int,ItemSelectedEventHandler)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|AddState(string)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|Build()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|BuildCorpse()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|BuildCrowd()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|BuildIdentityGallery()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|BuildUi()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|Changed(long)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|Close()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|FilterCrowd()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|FilterWorld()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|Open(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,string)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|ProjectTypography(Node)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|ProjectUiSymbol(Node,string)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|ProjectZones()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|RequireActive()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|SelectCorpse()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|SelectCrowd()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|SelectState()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|SelectUi()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|SelectUiGallery()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|Step()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|Tick(double)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|UiItems()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|UiTabs()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                    + "ourceView|UiWidget(string)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Add"
                    + "Label(Node,string,string)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Cle"
                    + "ar()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Pro"
                    + "jectBinding(JsonElement,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Pro"
                    + "jectCondition(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Pro"
                    + "jectPlayer(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Req"
                    + "uireActive()")]
    [InlineData(
            "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Row"
                    + "(string,string,string)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|AddCells(Control,int,int)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|AddIcon(Node,JsonElement,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|AddLabel(Node,string,string)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|AddPlaced(Control,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|Attach(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|Clear()")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|Project(byte[],long)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectActions(JsonElement,Node)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectGrids(JsonElement,Node)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectHistoric(Node,JsonElement,string,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectHistoricItem(Control,JsonElement,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectInventory(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectLegacy(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectSlots(JsonElement,Node)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectStash(JsonElement,Node)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectTargetRegion(Control,JsonElement,string)")]
    [InlineData(
            "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                    + "ayouts|ProjectTargets(JsonElement,Node)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Action(Node,string,string,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|ApplySelectors(HBoxContainer,long,long)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Attach(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Bindings(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Browser(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Clear()")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Diagnostics(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Endpoint(int,int,int,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Exit(JsonElement,VBoxContainer,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|FinishConfirmation(long,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Join(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Label(Node,string,string)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Lobby(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Main(VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Manual(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|ProjectSettings(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|RequestConfirmation(long)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|RequireActive(long)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Saves(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Selector(Node,string,string[])")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|SettingsFields(JsonElement,VBoxContainer)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|Setup(JsonElement,VBoxContainer,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                    + "ts|WireSelectors(HBoxContainer,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                    + "ddCollision(string,float,float,float,float,uint)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                    + "ddInspectorChoice(Node,string,IEnumerable<string>)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                    + "pplyInspectorLight()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                    + "ttach()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                    + "ttachInspector()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|C"
                    + "loseInspector()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|F"
                    + "ilterLibraryWorld(string)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|I"
                    + "nspectorArt()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|L"
                    + "ightInspector(long)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "opulateInspectorStates()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "roject()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectAnimation(JsonElement,Sprite2D,double,string)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectClearingMarkers()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectEffect(EffectState)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectGeometry(string)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectInspector()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectInspectorBuilding()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectInspectorItem()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectInterior(string,float,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectLibrarySites(Node2D)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectLoops(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectMap(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectMapState(Sprite2D,string,JsonElement,JsonElement,double)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectMinimap(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectParticles(EffectState,JsonElement,double)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectPortalClip(JsonElement,string,float)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectPortals(string)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectRuin(JsonElement,float)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                    + "rojectWarning(EffectState,JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                    + "eadColor(JsonElement)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                    + "equireActive()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                    + "esetInspectorItem()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                    + "estoreInspectorChoices()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                    + "etire()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                    + "etireInspector()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|S"
                    + "electLibraryBuilding(string)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|S"
                    + "howInspector()")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|S"
                    + "prite(string,bool,string)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|S"
                    + "tateInspector(long)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|T"
                    + "exture(string,bool)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|T"
                    + "ransformInspector(float,float)")]
    [InlineData(
            "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|T"
                    + "ransformInspectorArt()")]
    public void Check_PresenterNative_RequiresExactGuardedMember(string tuple)
    {
        string[] fields = tuple.Split(separator: '|');
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: fields[0]));
        CSharpCompilation basis = consumer.Create(source: source, path: fields[0]);
        ConsumerCompilation.RequireValid(compilation: basis);
        SyntaxTree original = basis.SyntaxTrees.Single(predicate: tree => StringComparer.Ordinal.Equals(
                x: tree.FilePath, y: fields[0]));
        EngineAccessPolicy.CheckTree(tree: original, compilation: basis).ShouldBeEmpty();
        SemanticModel model = basis.GetSemanticModel(syntaxTree: original);
        BaseMethodDeclarationSyntax method = original.GetRoot(cancellationToken:
                TestContext.Current.CancellationToken).DescendantNodes()
                .OfType<BaseMethodDeclarationSyntax>().Single(predicate: node => PresenterIdentity(
                        method: node, model: model) is string identity && StringComparer.Ordinal.Equals(
                                x: identity, y: fields[1] + "|" + fields[2]));
        PresenterBoundary(consumer: consumer, basis: basis, original: original, source: source, method: method);
    }

    private static string? PresenterIdentity(BaseMethodDeclarationSyntax method, SemanticModel model)
    {
        if (model.GetDeclaredSymbol(declaration: method) is not IMethodSymbol host)
        {
            return null;
        }

        string name = method is MethodDeclarationSyntax ordinary ? ordinary.Identifier.ValueText
                : method is ConstructorDeclarationSyntax constructor ? constructor.Identifier.ValueText : "";
        return host.ContainingType.ToDisplayString() + "|" + name + "(" + string.Join(separator: ",",
                values: host.Parameters.Select(selector: parameter => parameter.Type.ToDisplayString(
                        format: SymbolDisplayFormat.MinimallyQualifiedFormat))) + ")";
    }

    private static void PresenterBoundary(ConsumerCompilation consumer, CSharpCompilation basis,
            SyntaxTree original, string source, BaseMethodDeclarationSyntax method)
    {
        BlockSyntax body = method.Body ?? throw new InvalidOperationException();
        string guard = body.Statements[0].ToString();
        string positive = PresenterBody(source: source, body: body, changed: guard
                + "Godot.GD.Print(what: [1]); throw new System.InvalidOperationException();");
        PresenterSubject(consumer: consumer, basis: basis, original: original, source: positive).ShouldBeEmpty();
        foreach (string changed in PresenterDefects(guard: guard))
        {
            PresenterSubject(consumer: consumer, basis: basis, original: original, source: PresenterBody(
                    source: source, body: body, changed: changed)).ShouldNotBeEmpty(customMessage: changed);
        }

        SyntaxTree wrong = consumer.Parse(source: positive, path: "src/godot-game/game/inspection/Adjacent.cs");
        CSharpCompilation moved = basis.ReplaceSyntaxTree(oldTree: original, newTree: wrong);
        ConsumerCompilation.RequireValid(compilation: moved);
        EngineAccessPolicy.CheckTree(tree: wrong, compilation: moved).ShouldNotBeEmpty();
        string sibling = source + "\ninternal sealed class PresenterSibling { private readonly "
                + "KingdomZombies.Proofs.MainThreadGateway _gateway = new(); internal void Invoke() {"
                + "_gateway.RequireThread(); Godot.GD.Print(what: [1]); } }\n";
        PresenterSubject(consumer: consumer, basis: basis, original: original, source: sibling).ShouldNotBeEmpty();
    }

    private static string[] PresenterDefects(string guard)
    {
        const string Native = "Godot.GD.Print(what: [1]);";
        const string End = "throw new System.InvalidOperationException();";
        return
        [
            Native + End,
            Native + guard + End,
            "if (false) { " + guard + " }" + Native + End,
            "goto Skip; " + guard + " Skip: " + Native + End,
            "new KingdomZombies.Proofs.MainThreadGateway().RequireThread();" + Native + End,
            guard + "System.Action misuse = new Godot.Node().QueueFree;" + End,
            guard + "System.Action misuse = () => { " + Native + " }; misuse();" + End,
            guard + "void Local() { " + Native + " } Local();" + End,
            guard + "System.Threading.Tasks.Task.Run(action: () => { " + Native + " });" + End,
            guard + "System.Threading.Tasks.Task.CompletedTask.ContinueWith(continuationAction: task => { "
                    + Native + " });" + End,
        ];
    }

    private static string PresenterBody(string source, BlockSyntax body, string changed)
    {
        return source[..body.SpanStart] + "{" + changed + "}" + source[body.Span.End..];
    }

    private static IReadOnlyList<string> PresenterSubject(ConsumerCompilation consumer, CSharpCompilation basis,
            SyntaxTree original, string source)
    {
        SyntaxTree changed = consumer.Parse(source: source, path: original.FilePath);
        CSharpCompilation compilation = basis.ReplaceSyntaxTree(oldTree: original, newTree: changed);
        ConsumerCompilation.RequireValid(compilation: compilation);
        return EngineAccessPolicy.CheckTree(tree: changed, compilation: compilation);
    }

    private static void ContextDefect(JsonObject body, JsonObject model, string kind)
    {
        JsonArray frames = NodeField(owner: NodeField(owner: body, key: "theme"), key: "frames").AsArray();
        JsonNode frame = frames[0] ?? throw new InvalidOperationException();
        JsonNode item = NodeField(owner: body, key: "items")[17] ?? throw new InvalidOperationException();
        switch (kind)
        {
            case "theme-identity": frame["id"] = "common.s0.7900"; break;
            case "theme-valid-source":
                JsonNode other = frames[1]?.DeepClone() ?? throw new InvalidOperationException();
                other["state"] = "normal";
                frames[0] = other;
                break;
            case "theme-member": frame["member"] = "images/sharedassets0.assets/7900.png"; break;
            case "theme-png": frame["pngSha256"] = new string(c: '0', count: 64); break;
            case "theme-rgba": frame["rgbaSha256"] = new string(c: '0', count: 64); break;
            case "theme-region": NodeField(owner: frame, key: "atlasRect")[0] = 137; break;
            case "theme-unsafe": NodeField(owner: frame, key: "atlasRect")[0] = 9007199254740993L; break;
            case "theme-outside": NodeField(owner: frame, key: "atlasRect")[0] = 2048; break;
            case "theme-negative": NodeField(owner: frame, key: "atlasRect")[2] = -1; break;
            case "item-identity": item["id"] = "IT-19"; break;
            case "item-texture": item["texture"] = "assets/equipment/items-01-10.png"; break;
            case "item-icon": item["icon"] = "IT_19_master"; break;
            case "item-rgba": item["rgbaSha256"] = new string(c: '0', count: 64); break;
            case "item-region": NodeField(owner: item, key: "atlasRect")[0] = 253; break;
            case "item-unsafe": NodeField(owner: item, key: "atlasRect")[0] = 9007199254740993L; break;
            case "item-outside": NodeField(owner: item, key: "atlasRect")[0] = 512; break;
            case "item-string": NodeField(owner: item, key: "atlasRect")[0] = "12"; break;
            case "local-outside":
                JsonNode local = NodeField(owner: body, key: "regions").AsObject().First().Value
                        ?? throw new InvalidOperationException();
                local[0] = 256;
                break;
            case "source-dependency":
                NodeField(owner: model, key: "files").AsArray().Remove(
                        item: ModelFile(model: model, path: "content/packs/common.json"));
                break;
            default: throw new InvalidOperationException(message: "Unknown actual contextual region subject");
        }
    }

    private static byte[] FontDefect(byte[] original, string kind)
    {
        byte[] value = (byte[])original.Clone();
        (int head, int headOffset, int headBytes) = FontTable(bytes: value, name: "head");
        (int _, int cmap, int _) = FontTable(bytes: value, name: "cmap");
        (int _, int loca, int _) = FontTable(bytes: value, name: "loca");
        (int _, int hhea, int _) = FontTable(bytes: value, name: "hhea");
        if (kind is "table-checksum")
        {
            value[headOffset + 20] ^= 1;
            return value;
        }

        if (kind is "whole-checksum")
        {
            value[headOffset + 8] ^= 1;
            return value;
        }

        int subtable = cmap + (int)FontU32(bytes: value, offset: cmap + 8);
        int segments = FontU16(bytes: value, offset: subtable + 6) / 2;
        switch (kind)
        {
            case "cmap-offset": FontPut32(bytes: value, offset: cmap + 8, number: uint.MaxValue); break;
            case "cmap-glyph":
                FontPut16(bytes: value, offset: subtable + 16 + segments * 4, number: 32767);
                FontPut16(bytes: value, offset: subtable + 16 + segments * 6, number: 0);
                break;
            case "valid-cmap":
                FontPut16(bytes: value, offset: subtable + 16 + segments * 4, number: 1);
                FontPut16(bytes: value, offset: subtable + 16 + segments * 6, number: 0);
                break;
            case "cmap-order":
                FontPut16(bytes: value, offset: subtable + 16 + segments * 2, number: ushort.MaxValue);
                break;
            case "loca": FontPut32(bytes: value, offset: loca, number: 1); break;
            case "metrics": FontPut16(bytes: value, offset: hhea + 34, number: 0); break;
            default: FontGlyphDefect(bytes: value, kind: kind); break;
        }

        FontRepairChecksums(bytes: value, headRecord: head, headOffset: headOffset, headBytes: headBytes);
        return value;
    }

    private static void FontGlyphDefect(byte[] bytes, string kind)
    {
        (int _, int loca, int _) = FontTable(bytes: bytes, name: "loca");
        (int _, int glyf, int _) = FontTable(bytes: bytes, name: "glyf");
        (int _, int maxp, int _) = FontTable(bytes: bytes, name: "maxp");
        int count = FontU16(bytes: bytes, offset: maxp + 4);
        for (int glyph = 0; glyph < count; glyph++)
        {
            uint start = FontU32(bytes: bytes, offset: loca + glyph * 4);
            uint end = FontU32(bytes: bytes, offset: loca + (glyph + 1) * 4);
            if (end <= start)
            {
                continue;
            }

            int offset = glyf + (int)start;
            short contours = unchecked((short)FontU16(bytes: bytes, offset: offset));
            if (kind is "glyph-contours" && contours > 0)
            {
                FontPut16(bytes: bytes, offset: offset, number: 32767);
                return;
            }

            if (kind is not "glyph-contours" && contours == -1)
            {
                FontPut16(bytes: bytes, offset: offset + 12,
                        number: (ushort)(kind is "glyph-cycle" ? glyph : count));
                return;
            }
        }

        throw new InvalidOperationException(message: "Actual retained font lacks requested independent glyph subject");
    }

    private static (int Record, int Offset, int Bytes) FontTable(byte[] bytes, string name)
    {
        int count = FontU16(bytes: bytes, offset: 4);
        for (int index = 0; index < count; index++)
        {
            int record = 12 + index * 16;
            if (StringComparer.Ordinal.Equals(x: Encoding.ASCII.GetString(bytes: bytes, index: record, count: 4),
                    y: name))
            {
                return (record, (int)FontU32(bytes: bytes, offset: record + 8),
                        (int)FontU32(bytes: bytes, offset: record + 12));
            }
        }

        throw new InvalidOperationException(message: "Actual retained table missing " + name);
    }

    private static ushort FontU16(byte[] bytes, int offset)
    {
        return System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(
                source: bytes.AsSpan(start: offset, length: 2));
    }

    private static uint FontU32(byte[] bytes, int offset)
    {
        return System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(
                source: bytes.AsSpan(start: offset, length: 4));
    }

    private static void FontPut16(byte[] bytes, int offset, ushort number)
    {
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16BigEndian(
                destination: bytes.AsSpan(start: offset, length: 2), value: number);
    }

    private static void FontPut32(byte[] bytes, int offset, uint number)
    {
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32BigEndian(
                destination: bytes.AsSpan(start: offset, length: 4), value: number);
    }

    private static void FontRepairChecksums(byte[] bytes, int headRecord, int headOffset, int headBytes)
    {
        FontPut32(bytes: bytes, offset: headOffset + 8, number: 0);
        int count = FontU16(bytes: bytes, offset: 4);
        for (int index = 0; index < count; index++)
        {
            int record = 12 + index * 16;
            int offset = (int)FontU32(bytes: bytes, offset: record + 8);
            int length = (int)FontU32(bytes: bytes, offset: record + 12);
            FontPut32(bytes: bytes, offset: record + 4, number: FontSum(bytes: bytes,
                    offset: offset, length: length));
        }

        FontPut32(bytes: bytes, offset: headRecord + 4,
                number: FontSum(bytes: bytes, offset: headOffset, length: headBytes));
        uint adjustment = unchecked(0xb1b0afba - FontSum(bytes: bytes, offset: 0, length: bytes.Length));
        FontPut32(bytes: bytes, offset: headOffset + 8, number: adjustment);
    }

    private static uint FontSum(byte[] bytes, int offset, int length)
    {
        uint sum = 0;
        for (int position = 0; position < length; position += 4)
        {
            uint word = 0;
            for (int index = 0; index < 4; index++)
            {
                word = word << 8 | (position + index < length ? bytes[offset + position + index] : 0U);
            }

            sum = unchecked(sum + word);
        }

        return sum;
    }

    private static byte[] NativeDefect(byte[] original, string kind)
    {
        string text = Encoding.UTF8.GetString(bytes: original);
        if (kind is "font-bytes")
        {
            return Encoding.UTF8.GetBytes(s: NativeFontDefect(text: text));
        }

        (string before, string after) = kind switch
        {
            "missing-reference" => ("ExtResource(\"1_items\")", "ExtResource(\"missing\")"),
            "source-texture" => ("assets/equipment/items-01-10.png", "assets/equipment/items-11-24.png"),
            "region" => ("Rect2(2, 2, 10, 10)", "Rect2(511, 2, 10, 10)"),
            "resource-type" => ("type=\"AtlasTexture\" id=\"A_IT_01_master\"",
                    "type=\"Animation\" id=\"A_IT_01_master\""),
            "node-parent" => ("parent=\".\"", "parent=\"missing\""),
            "connection" => ("method=\"play\"", "method=\"queue_free\""),
            "font-role" => ("metadata/source_path = \"res://assets/ui/Fallback.ttf\"",
                    "metadata/source_path = \"res://assets/ui/LiberationSans.ttf\""),
            "resource-cycle" => ("id=\"Fallback\"]\n",
                    "id=\"Fallback\"]\nfallbacks = Array[Font]([SubResource(\"Body\")])\n"),
            _ => throw new InvalidOperationException(message: "Unknown actual native subject"),
        };
        string changed = text.Replace(oldValue: before, newValue: after, comparisonType: StringComparison.Ordinal);
        StringComparer.Ordinal.Equals(x: text, y: changed).ShouldBeFalse();
        return Encoding.UTF8.GetBytes(s: changed);
    }

    private static string NativeFontDefect(string text)
    {
        Match match = Regex.Match(input: text, pattern: "data = PackedByteArray\\(\"(?<data>[A-Za-z0-9+/=]+)\"\\)",
                options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
        match.Success.ShouldBeTrue();
        byte[] bytes = Convert.FromBase64String(s: match.Groups["data"].Value);
        bytes[bytes.Length - 1] ^= 1;
        return text[..match.Groups["data"].Index] + Convert.ToBase64String(inArray: bytes)
                + text[(match.Groups["data"].Index + match.Groups["data"].Length)..];
    }

    private static void GuardObservation(ContentPack actual, string path, string kind)
    {
        string directory = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY")
                ?? Path.Combine(path1: WorkspacePaths.Temp(purpose: "asset-pipeline"),
                        path2: "04-23-current-model");
        string file = Path.Combine(path1: directory,
                path2: "font-native-" + Guid.NewGuid().ToString(format: "N") + ".json");
        using var stream = new FileStream(path: file, mode: FileMode.CreateNew, access: FileAccess.Write);
        JsonSerializer.Serialize(utf8Json: stream, value: new
        {
            scope = "Actual current Core guard with rebound subject bytes; restored positive separate",
            path,
            kind,
            admitted = actual.Entries.Count,
            expected = actual.Expected.Count,
            accepted = 0,
            diagnostics = actual.Diagnostics,
        });
        stream.Flush(flushToDisk: true);
    }

    private static ContentPack HeaderPositive(JsonObject model, string path, byte[] bytes)
    {
        var replacements = new Dictionary<string, byte[]>(comparer: StringComparer.Ordinal) { [path] = bytes };
        var fields = new List<string>();
        if (path is "content/ui/legacy-stash-targets.json")
        {
            HeaderClosure(model: model, replacements: replacements, fields: fields);
        }

        var actual = ContentPack.Load(index: Encoding.UTF8.GetBytes(s: model.ToJsonString()),
                expectations: Read(path: ContentPack.ExpectedPath), read: name => replacements.TryGetValue(
                        key: name, value: out byte[]? replacement) ? replacement : Read(path: name));
        HeaderObservation(actual: actual, model: model, path: path, replacements: replacements, fields: fields);
        return actual;
    }

    private static void HeaderClosure(JsonObject model, Dictionary<string, byte[]> replacements, List<string> fields)
    {
        string[] paths = ["content/packs/ui-legacy-stash.json", "content/packs/text-data.json",
            "content/packs/compatibility.json", "tools/phase4/required-cases.json", "content/packs/delivery.json"];
        var documents = new Dictionary<string, JsonObject>(comparer: StringComparer.Ordinal);
        foreach (string path in paths)
        {
            byte[] original = path.StartsWith(value: "tools/", comparisonType: StringComparison.Ordinal)
                    ? File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                            path2: AssetPipelinePaths.Physical(path: path)))
                    : Read(path: path) ?? throw new InvalidOperationException();
            documents.Add(key: path, value: JsonNode.Parse(utf8Json: original)?.AsObject()
                    ?? throw new InvalidOperationException());
        }

        bool changed = true;
        for (int pass = 0; pass < 8 && changed; pass++)
        {
            changed = false;
            foreach ((string path, JsonObject document) in documents)
            {
                bool updated = HeaderBindings(value: document, replacements: replacements,
                        pointer: path + "#", fields: fields);
                if (path is "content/packs/delivery.json")
                {
                    updated |= HeaderCatalogPayload(delivery: document, replacements: replacements, fields: fields);
                }

                if (updated)
                {
                    byte[] value = Encoding.UTF8.GetBytes(s: document.ToJsonString());
                    replacements[path] = value;
                    string source = path.StartsWith(value: "tools/", comparisonType: StringComparison.Ordinal)
                            ? path : "src/godot-game/" + path;
                    JsonNode reference = NodeField(owner: model, key: "files").AsArray().Single(
                            predicate: row => row is not null && StringComparer.Ordinal.Equals(
                                    x: NodeField(owner: row, key: "path").GetValue<string>(), y: source))
                            ?? throw new InvalidOperationException();
                    Rebind(file: reference, bytes: value);
                    fields.Add(item: "model.files[" + source + "]/bytes,sha256");
                    changed = true;
                }
            }
        }

        changed.ShouldBeFalse(customMessage: "Detached header fixture binding graph must converge");
    }

    private static bool HeaderBindings(JsonNode value, Dictionary<string, byte[]> replacements,
            string pointer, List<string> fields)
    {
        bool changed = false;
        if (value is JsonObject record)
        {
            JsonNode? locator = record["path"] ?? record["file"] ?? record["sourcePath"];
            if (record.ContainsKey(propertyName: "sha256") && locator is JsonValue
                    && locator.GetValueKind() is JsonValueKind.String)
            {
                string path = locator.GetValue<string>().Replace(oldValue: "res://", newValue: "",
                        comparisonType: StringComparison.Ordinal);
                const string Prefix = "src/godot-game/";
                path = path.StartsWith(value: Prefix, comparisonType: StringComparison.Ordinal) ? path[Prefix.Length..]
                        : path;
                if (replacements.TryGetValue(key: path, value: out byte[]? replacement))
                {
                    string digest = Convert.ToHexStringLower(inArray: SHA256.HashData(source: replacement));
                    if (!StringComparer.Ordinal.Equals(x: record["sha256"]?.GetValue<string>(), y: digest))
                    {
                        fields.Add(item: pointer + "/sha256 " + record["sha256"]?.GetValue<string>() + " -> " + digest);
                        record["sha256"] = digest;
                        if (record.ContainsKey(propertyName: "bytes"))
                        {
                            record["bytes"] = replacement.Length;
                            fields.Add(item: pointer + "/bytes -> " + replacement.Length.ToString(
                                    provider: System.Globalization.CultureInfo.InvariantCulture));
                        }

                        changed = true;
                    }
                }
            }

            foreach ((string name, JsonNode? child) in record.ToArray())
            {
                if (child is not null)
                {
                    changed |= HeaderBindings(value: child, replacements: replacements,
                            pointer: pointer + "/" + name, fields: fields);
                }
            }
        }
        else if (value is JsonArray array)
        {
            for (int index = 0; index < array.Count; index++)
            {
                if (array[index] is JsonNode child)
                {
                    changed |= HeaderBindings(value: child, replacements: replacements,
                            pointer: pointer + "/" + index.ToString(
                                    provider: System.Globalization.CultureInfo.InvariantCulture), fields: fields);
                }
            }
        }

        return changed;
    }

    private static bool HeaderCatalogPayload(JsonObject delivery, Dictionary<string, byte[]> replacements,
            List<string> fields)
    {
        if (!replacements.TryGetValue(key: "tools/phase4/required-cases.json", value: out byte[]? catalog))
        {
            return false;
        }

        using var output = new MemoryStream();
        using (var gzip = new GZipStream(stream: output, compressionLevel: CompressionLevel.SmallestSize,
                leaveOpen: true))
        {
            gzip.Write(buffer: catalog);
        }

        string encoded = Convert.ToBase64String(inArray: output.ToArray());
        JsonNode payload = NodeField(owner: delivery, key: "fixtureCatalog");
        if (StringComparer.Ordinal.Equals(x: NodeField(owner: payload, key: "gzipBase64").GetValue<string>(),
                y: encoded))
        {
            return false;
        }

        payload["uncompressedBytes"] = catalog.Length;
        payload["gzipBase64"] = encoded;
        fields.Add(item: "content/packs/delivery.json#/fixtureCatalog/uncompressedBytes,gzipBase64");
        return true;
    }

    private static void HeaderObservation(ContentPack actual, JsonObject model, string path,
            Dictionary<string, byte[]> replacements, List<string> fields)
    {
        string directory = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY")
                ?? Path.Combine(path1: WorkspacePaths.Temp(purpose: "asset-pipeline"),
                        path2: "04-23-current-model");
        string file = Path.Combine(path1: directory, path2: "header-positive-"
                + Guid.NewGuid().ToString(format: "N") + ".json");
        var observation = new
        {
            scope = "Detached header/runtime positive only; actual source/recipe/judgment acceptance not conferred",
            path,
            publicationVersion = 2,
            admitted = actual.Entries.Count,
            expected = actual.Expected.Count,
            accepted = 0,
            modelSha256 = Convert.ToHexStringLower(inArray: SHA256.HashData(
                    source: Encoding.UTF8.GetBytes(s: model.ToJsonString()))),
            replacements = replacements.Select(selector: pair => new
            {
                path = pair.Key,
                bytes = pair.Value.Length,
                sha256 = Convert.ToHexStringLower(inArray: SHA256.HashData(source: pair.Value)),
            }).ToArray(),
            fields,
            diagnostics = actual.Diagnostics,
        };
        using var stream = new FileStream(path: file, mode: FileMode.CreateNew, access: FileAccess.Write);
        JsonSerializer.Serialize(utf8Json: stream, value: observation);
        stream.Flush(flushToDisk: true);
    }

    private static void ModelDefect(JsonObject model, JsonNode target, string kind)
    {
        JsonArray outputs = NodeField(owner: model, key: "outputs").AsArray();
        JsonArray dependencies = NodeField(owner: target, key: "dependencies").AsArray();
        switch (kind)
        {
            case "duplicate-output": outputs.Add(item: target.DeepClone()); break;
            case "missing-output": outputs.Remove(item: target); break;
            case "shared-identity": target["sharedFileId"] = "shared.unrelated"; break;
            case "wrong-contract": target["contract"] = "native-layout-v1"; break;
            case "wrong-valid-selector": target["definitionPointers"] = new JsonArray("/bodies/1"); break;
            case "selector-order":
                target["definitionPointers"] = new JsonArray("/fixtureResources", "/fixtureCatalog");
                break;
            case "dependency-missing": dependencies.RemoveAt(index: dependencies.Count - 1); break;
            case "dependency-order":
                JsonNode last = dependencies[dependencies.Count - 1]?.DeepClone()
                        ?? throw new InvalidOperationException();
                dependencies.RemoveAt(index: dependencies.Count - 1);
                dependencies.Insert(index: 0, item: last);
                break;
            case "unsafe-version": target["version"] = 9007199254740993L; break;
            case "runtime-role":
                ModelFile(model: model, path: "content/packs/bodies-a.json")["role"] = "source-support";
                break;
            case "duplicate-file":
                NodeField(owner: model, key: "files").AsArray().Add(item:
                        ModelFile(model: model, path: "content/packs/bodies-a.json").DeepClone());
                break;
            default: throw new InvalidOperationException(message: "Unknown independent model subject");
        }
    }

    private static byte[] MediaDefect(byte[] original, string path, string kind)
    {
        if (kind is "truncated")
        {
            return original[..12];
        }

        byte[] changed = (byte[])original.Clone();
        if (path.EndsWith(value: ".png", comparisonType: StringComparison.Ordinal))
        {
            int position = kind switch { "extent" => 8, "format" => 25, _ => 45 };
            changed[position] ^= 127;
            if (kind is "format")
            {
                WritePngCrc(bytes: changed, position: 8);
            }
            else if (kind is "payload")
            {
                WritePngCrc(bytes: changed, position: 33);
            }
        }
        else if (path.EndsWith(value: ".wav", comparisonType: StringComparison.Ordinal))
        {
            changed[kind switch { "extent" => 4, "format" => 22, _ => 40 }] ^= 127;
        }
        else
        {
            int position = kind is "extent" ? 20 : 0;
            if (kind is "payload")
            {
                for (int table = 12; table + 16 <= changed.Length; table += 16)
                {
                    if (Encoding.ASCII.GetString(bytes: changed, index: table, count: 4) is "head")
                    {
                        position = checked((int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(
                                source: changed.AsSpan(start: table + 8, length: 4))) + 12;
                        break;
                    }
                }
            }

            changed[position] ^= 127;
        }

        return changed;
    }

    private static void WritePngCrc(byte[] bytes, int position)
    {
        int length = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(
                source: bytes.AsSpan(start: position, length: 4));
        uint crc = uint.MaxValue;
        foreach (byte value in bytes.AsSpan(start: position + 4, length: length + 4))
        {
            crc ^= value;
            for (int bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) == 0 ? crc >> 1 : 0xedb88320U ^ (crc >> 1);
            }
        }

        System.Buffers.Binary.BinaryPrimitives.WriteUInt32BigEndian(destination: bytes.AsSpan(
                start: position + 8 + length, length: 4), value: crc ^ uint.MaxValue);
    }

    private static JsonObject Model()
    {
        JsonNode parsed = JsonNode.Parse(utf8Json: File.ReadAllBytes(path: Path.Combine(path1: WorkspacePaths.Temp(
                purpose: "asset-pipeline"), path2: "04-23-current-model/normalized-index.json")))
                ?? throw new InvalidOperationException(message: "Current model JSON missing");
        return parsed.AsObject();
    }

    private static JsonObject Definition(string path)
    {
        JsonNode parsed = JsonNode.Parse(utf8Json: Read(path: path) ?? [])
                ?? throw new InvalidOperationException(message: "Actual definition JSON missing");
        return parsed.AsObject();
    }

    private static JsonNode NodeField(JsonNode owner, string key)
    {
        return owner[key] ?? throw new InvalidOperationException(message: "Missing subject field " + key);
    }

    private static JsonNode ModelFile(JsonObject model, string path)
    {
        return NodeField(owner: model, key: "files").AsArray().Single(predicate: file => file is not null
                && StringComparer.Ordinal.Equals(x: NodeField(owner: file, key: "path").GetValue<string>(),
                        y: "src/godot-game/" + path))
                ?? throw new InvalidOperationException(message: "Missing actual model file " + path);
    }

    private static ContentPack Loaded(JsonObject model, string? replacementPath = null, byte[]? replacement = null)
    {
        return ContentPack.Load(index: Encoding.UTF8.GetBytes(s: model.ToJsonString()),
                expectations: Read(path: ContentPack.ExpectedPath), read: path => StringComparer.Ordinal.Equals(
                        x: path, y: replacementPath) ? replacement : Read(path: path));
    }

    private static void Rebind(JsonNode file, byte[] bytes)
    {
        file["bytes"] = bytes.Length;
        file["sha256"] = Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes));
    }

    private static void HeaderDefect(JsonObject body, JsonNode file, string kind, string required)
    {
        switch (kind)
        {
            case "schema-future": body["schemaVersion"] = 2; break;
            case "schema-type": body["schemaVersion"] = "one"; break;
            case "body-zero": body["version"] = 0; break;
            case "body-incoherent": body["version"] = 2; break;
            case "body-type": body["version"] = "one"; break;
            case "publication-zero": file["version"] = 0; break;
            case "publication-unsafe": file["version"] = 9007199254740993L; break;
            case "missing-structure": body.Remove(propertyName: required); break;
            case "wrong-valid-id": body["id"] = "ui-settings"; break;
            default: throw new InvalidOperationException(message: "Unknown independent header defect");
        }
    }

    private static byte[] IconDefect(byte[] original, string kind)
    {
        byte[] bytes = [.. original];
        int first = System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(
                source: bytes.AsSpan(start: 18, length: 4));
        switch (kind)
        {
            case "truncated": return bytes[..^1];
            case "type": bytes[2] = 2; break;
            case "count": bytes[4] = 6; break;
            case "dimension": bytes[6] = 17; break;
            case "offset": bytes[18]++; break;
            case "overflow": Array.Fill(array: bytes, value: (byte)255, startIndex: 14, count: 4); break;
            case "overlap": original.AsSpan(start: 18, length: 4).CopyTo(destination: bytes.AsSpan(start: 34)); break;
            case "png-dimension": bytes[first + 19] = 17; break;
            case "png-crc": bytes[first + 29] ^= 1; break;
            case "png-content": bytes[^1] ^= 1; break;
            case "wrong-hash": break;
            default: throw new InvalidOperationException(message: "Unknown independent ICO defect");
        }

        return bytes;
    }

    private static ContentPack Common(byte[] bytes, string path, bool authenticate)
    {
        byte[] indexSource = CommonRead(path: "content/pack-index.json") ?? throw new InvalidOperationException();
        byte[] tracer = CommonRead(path: "content/packs/tracer.json") ?? throw new InvalidOperationException();
        byte[] scene = CommonRead(path: "inspection/Inspection.tscn") ?? throw new InvalidOperationException();
        JsonObject index = JsonNode.Parse(utf8Json: indexSource)?.AsObject()
                ?? throw new InvalidOperationException();
        JsonNode row = index["shards"]?.AsArray().Single(predicate: item => StringComparer.Ordinal.Equals(
                x: item?["id"]?.GetValue<string>(), y: "common")) ?? throw new InvalidOperationException();
        row["status"] = "selected";
        row["version"] = 1;
        row["path"] = "src/godot-game/" + path;
        row["bytes"] = bytes.Length;
        row["sha256"] = authenticate ? Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes))
                : new string(c: '0', count: 64);
        var actual = ContentPack.Load(index: Encoding.UTF8.GetBytes(s: index.ToJsonString()), expectations:
                CommonRead(path: ContentPack.ExpectedPath), read: name => StringComparer.Ordinal.Equals(x: name,
                        y: path) ? bytes : CommonRead(path: name));
        string directory = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY")
                ?? Path.GetTempPath();
        using var output = new FileStream(path: Path.Combine(path1: directory,
                path2: "common-load-observation-" + Guid.NewGuid().ToString(format: "N") + ".json"),
                mode: FileMode.CreateNew, access: FileAccess.Write);
        JsonSerializer.Serialize(utf8Json: output, value: new
        {
            scope = "Actual common subject Load in independent authenticated historical partial fixture",
            path,
            authenticate,
            subjectBytes = bytes.Length,
            subjectSha256 = Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes)),
            indexBytes = indexSource.Length,
            tracerBytes = tracer.Length,
            sceneBytes = scene.Length,
            entries = actual.Entries.Keys.Order(comparer: StringComparer.Ordinal).ToArray(),
            diagnostics = actual.Diagnostics,
            status = actual.Status,
            coreMvid = typeof(ContentPack).Assembly.ManifestModule.ModuleVersionId,
        });
        output.Flush(flushToDisk: true);
        return actual;
    }

    private static byte[]? CommonRead(string path)
    {
        if (path is not ("content/pack-index.json" or "content/packs/tracer.json" or "inspection/Inspection.tscn"))
        {
            return Read(path: path);
        }

        (int size, string sha256) = path switch
        {
            "content/pack-index.json" => (9906,
                    "cca59a20691e75e874f7ed4b229e838e43190b683f0885cc2aa5717f6ed92739"),
            "content/packs/tracer.json" => (2430,
                    "e21c933b36bacd5f81206b33f7a9680bedd89bd5e580b12f5004ab212895d1f4"),
            _ => (993, "66702efb9bf9390f6454316ce0ec5bc7808cb5f439addf0430d46abccb589739"),
        };
        MethodInfo reader = typeof(ContentInspectorTests).GetMethod(name: "Read", bindingAttr:
                BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException();
        byte[] source = reader.Invoke(obj: null, parameters: [path]) as byte[]
                ?? throw new InvalidOperationException();
        string observed = Convert.ToHexStringLower(inArray: SHA256.HashData(source: source));
        string directory = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY")
                ?? Path.GetTempPath();
        using var output = new FileStream(path: Path.Combine(path1: directory,
                path2: "common-fixture-observation-" + Guid.NewGuid().ToString(format: "N") + ".json"),
                mode: FileMode.CreateNew, access: FileAccess.Write);
        JsonSerializer.Serialize(utf8Json: output, value: new
        {
            scope = "Exact existing private historical fixture reader; observed before authentication assertions",
            path,
            bytes = source.Length,
            sha256 = observed,
            expectedBytes = size,
            expectedSha256 = sha256,
            reader = reader.ToString(),
            owner = reader.DeclaringType?.FullName,
            testsMvid = reader.Module.ModuleVersionId,
        });
        output.Flush(flushToDisk: true);
        source.Length.ShouldBe(expected: size);
        observed.ShouldBe(expected: sha256);
        return source;
    }

    private static void Set(Type type, object owner, string name, object value)
    {
        FieldInfo field = type.GetField(name: name, bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException();
        field.SetValue(obj: owner, value: value);
    }

    private static byte[]? Read(string path)
    {
        string file = Path.Combine(path1: ProofProtocol.Root, path2: "src/godot-game/game/" + path);
        return File.Exists(path: file) ? File.ReadAllBytes(path: file) : null;
    }
}
