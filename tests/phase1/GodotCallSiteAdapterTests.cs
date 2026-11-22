using System.IO;
using System.Linq;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves sixteen observed Godot call-site permissions stop at their exact generated member.</summary>
public sealed class GodotCallSiteAdapterTests
{
    /// <summary>Rejects an adjacent handwritten call and a colliding handwritten definition for every
    /// permitted symbol.</summary>
    [Fact, Trait("Case", "rules.godot-callsite-boundary")]
    public void Analyze_GodotCallSitePermissions_RejectsOutsideMembers()
    {
        VerifyGeneratedMemberBoundaries();
        ProofProtocol.Record(id: "rules.godot-callsite-boundary", diagnostic: "GODOT_CALLSITE_BOUNDARY_REJECTED",
                location: "SCR0001 adjacent; CS0111 for sixteen exact generated member collisions; clean restored");
    }

    /// <summary>Builds actual exact generated-member collisions and rejects the intended compiler diagnostic.</summary>
    internal static void VerifyGeneratedMemberBoundaries()
    {
        VerifyActualBuild();
        VerifyAdjacent();
        using var registry = JsonDocument.Parse(json: File.ReadAllText(path: Path.Combine(path1: ProofProtocol
                .Root, path2: "docs/code/generated-callsite-adaptations.json")));
        JsonElement[] rows = [..registry.RootElement.GetProperty(propertyName: "local").EnumerateArray().Where(
                predicate: row => row.TryGetProperty(propertyName: "collisionSource", value: out _))];
        EngineGeneratedTests.VerifyInventories(registry: registry.RootElement, baselineCount: 18,
                baselineHash: "ef8ea1e0be872146bcf1f81a2079733bb37be5c2b9718fbf0faa47b164c08d8f");
        VerifyCollisionRows(rows: rows);
    }

    /// <summary>Runs exact original game, cohort, adjacent or collision guards for one declared stage.</summary>
    internal static object VerifyBoundedStage(JsonElement stage)
    {
        string action = stage.GetProperty(propertyName: "action").GetString() ?? throw new InvalidDataException();
        switch (action)
        {
            case "game-build": VerifyActualBuild(); return new { action };
            case "godot-adjacent": VerifyAdjacent(); return new { action };
            case "godot-collision": return VerifyDeclaredCollisions(stage: stage);
            case "godot-cohort": return VerifyBoundedCohort(stage: stage);
            default: throw new System.IO.InvalidDataException(message: "GODOT_STAGE_UNKNOWN");
        }
    }

    private static void VerifyActualBuild()
    {
        (int Exit, string Output) before = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2],
                "--no-restore", "-p:NuGetAudit=false");
        before.Exit.ShouldBe(expected: 0, customMessage: before.Output);
    }

    private static void VerifyAdjacent()
    {
        string adjacent = ConfigurationTests.BuildGameWithSource(
                source: ("namespace KingdomZombies.Proofs; public partial class P"
                        + "hase1 { private static int AdjacentCallSite() => System" + ".Math.Abs(-1); }"));
        adjacent.ShouldContain(expected: "error SCR0001:");
        adjacent.ShouldContain(expected: "Main.cs(");
        (int Exit, string Output) restored = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2],
                "--no-restore", "--disable-build-servers", "-p:UseSharedCompilation=false", "-nodeReuse:false",
                "-p:NuGetAudit=false");
        restored.Exit.ShouldBe(expected: 0, customMessage: restored.Output);
    }

    private static object VerifyBoundedCohort(JsonElement stage)
    {
        using var registry = JsonDocument.Parse(json: File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "docs/code/generated-callsite-adaptations.json")));
        string id = stage.GetProperty(propertyName: "cohort").GetString() ?? throw new System.IO.InvalidDataException();
        JsonElement[] rows = EngineGeneratedTests.VerifyDeclaredCohort(registry: registry.RootElement, id: id,
                calls: true);
        string?[] actual = [.. rows.Select(selector: row => row.GetProperty(propertyName: "id").GetString())];
        actual.SequenceEqual(second: stage.GetProperty(propertyName: "rowIds").EnumerateArray().Select(
                selector: row => row.GetString()), comparer: System.StringComparer.Ordinal).ShouldBeTrue();
        return new { cohort = id, rowIds = actual };
    }

    private static object VerifyDeclaredCollisions(JsonElement stage)
    {
        using var registry = JsonDocument.Parse(json: File.ReadAllText(path: Path.Combine(path1: ProofProtocol
                .Root, path2: "docs/code/generated-callsite-adaptations.json")));
        JsonElement[] rows = [..registry.RootElement.GetProperty(propertyName: "local").EnumerateArray().Where(
                predicate: row => row.TryGetProperty(propertyName: "collisionSource", value: out _))];
        string?[] actual = [.. rows.Select(selector: row => row.GetProperty(propertyName: "id").GetString())];
        actual.SequenceEqual(second: stage.GetProperty(propertyName: "rowIds").EnumerateArray().Select(
                selector: row => row.GetString()), comparer: System.StringComparer.Ordinal).ShouldBeTrue();
        VerifyCollisionRows(rows: rows);
        return new { rowIds = actual };
    }

    private static void VerifyCollisionRows(JsonElement[] rows)
    {
        rows.Take(count: 16).Select(selector: row => row.GetProperty(propertyName: "id").GetString())
                .ShouldBe(expected: Enumerable.Range(start: 1, count: 16).Select(selector: number =>
                        "KZ-EX-GODOT-CALLS-" + number.ToString(provider: System.Globalization.CultureInfo
                                .InvariantCulture)));
        string combined = string.Join(separator: "\n", values: rows.Select(selector: row =>
        {
            string source = row.GetProperty(propertyName: "collisionSource").GetString()
                    ?? throw new System.InvalidOperationException(message: "Required collision source is null");
            int separator = source.IndexOf(value: ';');
            return source[..separator] + " { " + source[(separator + 1)..] + " }";
        }));
        string collision = ConfigurationTests.BuildGameWithSource(source: combined);
        collision.ShouldNotContain(expected: "error CS100");
        collision.ShouldNotContain(expected: "error CS15");
        collision.ShouldNotContain(expected: "error NU");
        collision.ShouldNotContain(expected: "error MSB");
        foreach (JsonElement row in rows)
        {
            string[] identity = (row.GetProperty(propertyName: "target").GetString()
                    ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                    .Split(separator: '(')[0].Split(separator: '.');
            collision.Split(separator: '\n').Any(predicate: line => line.Contains(value: "error CS0111:",
                    comparisonType: System.StringComparison.Ordinal) && line.Contains(value: ".generated.cs(",
                            comparisonType: System.StringComparison.Ordinal)
                    && line.Contains(value: "'" + identity[^1] + "'",
                            comparisonType: System.StringComparison.Ordinal)
                    && line.Contains(value: "'" + identity[^2] + "'",
                            comparisonType: System.StringComparison.Ordinal)).ShouldBeTrue(customMessage: collision);
        }

        (int Exit, string Output) clean = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2],
                "--no-restore", "-p:NuGetAudit=false");
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
    }
}
