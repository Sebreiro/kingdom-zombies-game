using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves evaluated allowed and forbidden dependency graphs using real compiled subjects.</summary>
public sealed class GraphTests
{
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

    private static bool IsDirectViolation(string error)
    {
        bool rule = error.Contains(value: "GRAPH_FORBIDDEN", comparisonType: System.StringComparison.Ordinal);
        bool subject = error.Contains(value: "src/core", comparisonType: System.StringComparison.Ordinal);
        bool dependency = error.Contains(value: "godot-game", comparisonType: System.StringComparison.Ordinal);
        return rule && subject && dependency;
    }
}
