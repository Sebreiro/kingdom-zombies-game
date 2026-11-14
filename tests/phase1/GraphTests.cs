using System.Collections.Generic;
using System.IO;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves evaluated allowed and forbidden dependency graphs using real compiled subjects.</summary>
public sealed class GraphTests
{
    /// <summary>Requires production output policy to reject a compilable unintended executable.</summary>
    [Fact]
    public void Evaluate_CoreExecutable_ReportsOutputKind()
    {
        var fixture = new GraphFixture();
        string core = Path.Combine(path1: fixture.Root, path2: ProjectEvaluator.Expected[0]);
        File.WriteAllText(path: core, contents: File.ReadAllText(path: core).Replace(oldValue: "</Project>",
                newValue: "<PropertyGroup><OutputType>Exe</OutputType></PropertyGroup></Project>",
                comparisonType: System.StringComparison.Ordinal));
        string source = "namespace KingdomZombies.Core;\n\ninternal static class Entry\n{\n"
                + "    internal static int Main()\n    {\n        return 0;\n    }\n}\n";
        File.WriteAllText(path: Path.Combine(path1: fixture.Root, path2: "src/core/Entry.cs"), contents: source);
        (int Exit, string Output) built = fixture.Run("build", core, "--no-restore", "--disable-build-servers",
                "-p:UseSharedCompilation=false", "-nodeReuse:false", "-p:NuGetAudit=false");
        built.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_SUBJECT: " + built.Output);
        ProjectEvaluator.Validate(root: fixture.Root).ShouldContain(elementPredicate: error => error.Contains(
                value: "GRAPH_OUTPUT_KIND", comparisonType: System.StringComparison.Ordinal));
    }

    /// <summary>Requires an intended graph failure after a clean and a violating subject both compile.</summary>
    [Fact, Trait("Case", "graph.allowed-and-direct")]
    public void Evaluate_CoreEngineReference_ReportsForbiddenEdge()
    {
        ProjectEvaluator.Validate(root: ProofProtocol.Root).ShouldBeEmpty();
        var fixture = new GraphFixture();
        fixture.AddDirectEngineEdge();
        IReadOnlyList<string> errors = ProjectEvaluator.Validate(root: fixture.Root);
        errors.ShouldContain(elementPredicate: error => IsDirectViolation(error: error));
        ProofProtocol.Record(id: "graph.allowed-and-direct", diagnostic: "GRAPH_FORBIDDEN",
                location: string.Join(separator: "\n", values: errors));
    }

    /// <summary>Requires the compiled type mechanism to reject a real engine type in Core.</summary>
    [Fact]
    public void Inspect_CoreEngineType_ReportsCompiledLeak()
    {
        var fixture = new GraphFixture();
        fixture.AddDirectEngineEdge();
        string source = "namespace KingdomZombies.Core;\n"
                + "/// <summary>Compiled type boundary probe.</summary>\ninternal static class Subject\n{\n"
                + "    internal static string Read()\n    {\n"
                + "        return typeof(KingdomZombies.Proofs.Phase1).FullName ?? string.Empty;\n    }\n}\n";
        File.WriteAllText(path: Path.Combine(path1: fixture.Root, path2: "src/core/Subject.cs"), contents: source);
        (int Exit, string Output) built = fixture.Run("build", ProjectEvaluator.Expected[0], "--no-restore",
                "--disable-build-servers", "-p:UseSharedCompilation=false", "-nodeReuse:false", "-p:NuGetAudit=false");
        built.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_SUBJECT: " + built.Output);
        IReadOnlyList<string> errors = TypeBoundaryScanner.Validate(assembly: Path.Combine(path1: fixture.Root,
                path2: "src/core/bin/Debug/net10.0/KingdomZombies.Core.dll"),
                expectedType: "KingdomZombies.Core.ProofValue", boundary: "core");
        errors.ShouldContain(elementPredicate: error => error.Contains(value: "TYPE_FORBIDDEN",
                comparisonType: System.StringComparison.Ordinal));
    }

    private static bool IsDirectViolation(string error)
    {
        bool rule = error.Contains(value: "GRAPH_FORBIDDEN", comparisonType: System.StringComparison.Ordinal);
        bool subject = error.Contains(value: "src/core", comparisonType: System.StringComparison.Ordinal);
        bool dependency = error.Contains(value: "godot-game", comparisonType: System.StringComparison.Ordinal);
        return rule && subject && dependency;
    }
}
