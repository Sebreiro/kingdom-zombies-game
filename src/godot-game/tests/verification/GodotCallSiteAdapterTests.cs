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
        (int Exit, string Output) before = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2],
                "--no-restore", "-p:NuGetAudit=false");
        before.Exit.ShouldBe(expected: 0, customMessage: before.Output);
        string adjacent = ConfigurationTests.BuildGameWithSource(
                source: ("namespace KingdomZombies.Proofs; public partial class P"
                        + "hase1 { private static int AdjacentCallSite() => System" + ".Math.Abs(-1); }"));
        adjacent.ShouldContain(expected: "error SCR0001:");
        adjacent.ShouldContain(expected: "Main.cs(");
        using var registry = JsonDocument.Parse(json: File.ReadAllText(path: Path.Combine(path1: ProofProtocol
                .Root, path2: "docs/code/generated-callsite-adaptations.json")));
        JsonElement[] rows = [..registry.RootElement.GetProperty(propertyName: "local").EnumerateArray().Where(
                predicate: row => row.TryGetProperty(propertyName: "collisionSource", value: out _))];
        rows.Length.ShouldBe(expected: 16);
        string combined = string.Join(separator: "\n", values: rows.Select(selector: row =>
        {
            string source = row.GetProperty(propertyName: "collisionSource").GetString()
                    ?? throw new System.InvalidOperationException(message: "Required collision source is null");
            int separator = source.IndexOf(value: ';');
            return source[..separator] + " { " + source[(separator + 1)..] + " }";
        }));
        string collision = ConfigurationTests.BuildGameWithSource(source: combined);
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
