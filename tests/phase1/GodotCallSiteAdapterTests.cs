using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves all eight observed Godot call-site permissions stop at their exact generated member.</summary>
public sealed class GodotCallSiteAdapterTests
{
    /// <summary>Rejects an adjacent handwritten call and a colliding handwritten definition for every
    /// permitted symbol.</summary>
    [Fact, Trait("Case", "rules.godot-callsite-boundary")]
    public void GodotCallSitePermissionsCannotEscapeTheirMembers()
    {
        var adjacent = ConfigurationTests.BuildGameWithSource(source: (
                "namespace KingdomZombies.Proofs; public partial class P" +
                "hase1 { private static int AdjacentCallSite() => System" +
                ".Math.Abs(-1); }"));
        adjacent.ShouldContain(expected: "error SCR0001:");
        adjacent.ShouldContain(expected: "Main.cs(");
        using var registry = JsonDocument.Parse(json: File.ReadAllText(path: Path.Combine(path1: ProofProtocol.
                Root, path2: "docs/code/generated-callsite-adaptations.json")));
        var rows = registry.RootElement.GetProperty(propertyName: "local").EnumerateArray().Where(predicate:
                row => row.TryGetProperty(propertyName: "collisionSource", value: out _)).ToArray();
        rows.Length.ShouldBe(expected: 8);
        foreach (var row in rows)
        {
            string source = (row.GetProperty(propertyName: "collisionSource").GetString() ?? throw new System.
                    InvalidOperationException(message: "Required proof value is null"));
            var collision = ConfigurationTests.BuildGameWithSource(source: source);
            collision.ShouldContain(expected: "error CS0111:");
            string method = (row.GetProperty(propertyName: "target").GetString() ?? throw new System.
                    InvalidOperationException(message: "Required proof value is null")).Split(separator: '(')[0]
                    .Split(separator: '.')[^1];
            collision.ShouldContain(expected: method);
            collision.ShouldContain(expected: ".generated.cs(");
        }
        var clean = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2], "--no-restore",
                "-p:NuGetAudit=false");
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        ProofProtocol.Record(id: "rules.godot-callsite-boundary", diagnostic: "GODOT_CALLSITE_BOUNDARY_REJECTED"
                , location: ("SCR0001 adjacent; CS0111 for all eight exact generated " +
                "member collisions; clean restored"));
    }
}
