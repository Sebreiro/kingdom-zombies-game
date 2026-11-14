using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Retains the captured Method_State_Expected syntax rule for every actual test method.</summary>
public sealed class TestMethodNameTests
{
    /// <summary>Rejects malformed names rather than granting a broad test-project naming waiver.</summary>
    [Fact, Trait("Case", "R-48.negative")]
    public void Names_InvalidShape_Rejected()
    {
        foreach (string name in _invalidNames)
        {
            TestSourceIndex.FollowsMethodStateExpected(methodName: name).ShouldBeFalse(customMessage: name);
        }

        var fixture = new BuildFixture();
        fixture.Restore();
        fixture.Build().Exit.ShouldBe(expected: 0);
        fixture.Write(source: "namespace KingdomZombies.Core;\n\n/// <summary>Boundary.</summary>\n"
                + "public static class Subject\n{\n    /// <summary>Boundary.</summary>\n"
                + "    public static void Read_Empty_Accepted()\n    {\n    }\n}\n");
        (int Exit, string Output) result = fixture.Build();
        BuildFixture.HasDiagnostic(result: result, id: "CA1707", line: 7, column: 24).ShouldBeTrue(
                customMessage: result.Output);
        ProofProtocol.Record(id: "R-48.negative", diagnostic: "CA1707",
                location: fixture.DirectoryPath + "/src/core/Subject.cs(7,24)");
    }

    /// <summary>Finds wrapped, qualified, suffixed, custom, static, generic and nested test declarations.</summary>
    [Fact, Trait("Case", "R-48.positive")]
    public void Names_AllOwnedTests_FollowRequiredShape()
    {
        foreach (string name in _validNames)
        {
            TestSourceIndex.FollowsMethodStateExpected(methodName: name).ShouldBeTrue(customMessage: name);
        }

        TestSourceIndex.TestMethods(content: UnusualSource).ShouldBe(expected: _unusualMethods);
        IReadOnlyList<string> methods = TestSourceIndex.ProductionTestMethods();
        methods.Count.ShouldBeGreaterThan(expected: 400);
        methods.Distinct(comparer: System.StringComparer.Ordinal).Count().ShouldBe(expected: methods.Count);
        RequireCurrentBindings(methods: methods);
        IEnumerable<string> violations = methods.Where(predicate: name => !TestSourceIndex.FollowsMethodStateExpected(
                methodName: name[(name.LastIndexOf(value: '.') + 1)..]));
        violations.ShouldBeEmpty(customMessage: string.Join(separator: "\n", values: violations));
        ProofProtocol.Record(id: "R-48.positive", diagnostic: "TEST_NAMES_ACCEPTED",
                location: "tests/phase1/**/*.cs");
    }

    private const string UnusualSource = "namespace Sample.Tests; class Odd {\n[Theory(\nDisplayName=\"wrapped\")]"
            + " void Wrapped(){}\n[Xunit.Fact] void Qualified(){}\n[FactAttribute] void Suffixed(){}\n"
            + "[RetryFact] void Custom(){}\n[Fact] static void Static(){}\n[Fact] void Generic<T>(){}\n"
            + "const string Sample=\"[Fact] void InString(){}\";\nclass Nested{"
            + "[Trait(\"a\",\"b\"),Fact] void Inner(){}}}";
    private static readonly string[] _invalidNames = ["GetAll", "GeneratedCode_IsSkipped", "Get_All_Seeded_Returns",
        "getAll_Seeded_Returns", "GetAll_seeded_Returns", "GetAll__Returns", "GetAll_Seeded_Returns_", "Ä_B_C"];
    private static readonly string[] _validNames = ["GetAll_SeededDatabase_ReturnsThreeLanguages",
        "Line_120Characters_Passes", "1_2_3"];
    private static readonly string[] _unusualMethods = ["Sample.Tests.Odd.Wrapped", "Sample.Tests.Odd.Qualified",
        "Sample.Tests.Odd.Suffixed", "Sample.Tests.Odd.Custom", "Sample.Tests.Odd.Static", "Sample.Tests.Odd.Generic",
        "Sample.Tests.Odd.Nested.Inner"];

    private static void RequireCurrentBindings(IReadOnlyList<string> methods)
    {
        JsonObject manifest = RegisterValidator.Read(relative: "tools/phase1/required-cases.json");
        JsonArray cases = manifest["cases"]?.AsArray()
                ?? throw new System.IO.InvalidDataException(message: "NAMING_CASES_REQUIRED");
        var bindings = new HashSet<string>(comparer: System.StringComparer.Ordinal);
        foreach (JsonNode? row in cases)
        {
            if (!System.StringComparer.Ordinal.Equals(x: row?["implementation"]?.GetValue<string>(), y: "csharp"))
            {
                continue;
            }

            string name = row?["test"]?.GetValue<string>()
                    ?? throw new System.IO.InvalidDataException(message: "NAMING_BINDING_REQUIRED");
            bindings.Add(item: name).ShouldBeTrue(customMessage: "Duplicate binding: " + name);
            methods.Any(predicate: actual => System.StringComparer.Ordinal.Equals(x: actual, y: name))
                    .ShouldBeTrue(customMessage: "Missing binding: " + name);
        }

        bindings.Count.ShouldBeGreaterThan(expected: 400);
    }
}
