using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Gives R-67 independent current repository and deliberately weakened-policy subjects.</summary>
public sealed class SuppressionTests
{
    /// <summary>Checks tracked inputs and authentic generated permissions without importing donor escapes.</summary>
    [Fact, Trait("Case", "R-67.positive")]
    public void Scan_ActualRepository_AcceptsExactPermissions()
    {
        SuppressionPolicy.CheckRepository().ShouldBeEmpty();
        ProofProtocol.Record(id: "R-67.positive", diagnostic: "SUPPRESSION_REPOSITORY_EXACT",
                location: "tracked C#, MSBuild, editor/global configuration, response and workflow inputs");
    }

    /// <summary>Rejects source, XML, response and workflow suppressions at precise subject locations.</summary>
    [Fact, Trait("Case", "R-67.negative")]
    public void Scan_UndeclaredSuppressions_RejectsEachEntryPoint()
    {
        ExemptionValidator.CheckSource(path: "Subject.cs", content: "class Subject {}\n").ShouldBeEmpty();
        ExemptionValidator.CheckSource(path: "Subject.cs", content:
                "#pragma warning disable SCR0006\nclass Subject {}\n").ShouldContain(
                        elementPredicate: error => error.StartsWith(value: "SUPPRESSION_UNREGISTERED Subject.cs(1,1)",
                                comparisonType: StringComparison.Ordinal));
        ExemptionValidator.CheckEditorConfig(path: "Subject.globalconfig", content:
                "[*.cs]\ndotnet_diagnostic.SCR0006.severity = warning\n").Count.ShouldBe(expected: 1);
        foreach (string property in new[] { "NoWarn", "WarningsNotAsErrors", "RunAnalyzers" })
        {
            string value = property is "RunAnalyzers" ? "false" : "SCR0006";
            SuppressionPolicy.CheckConfiguration(path: "Subject.TARGETS", content:
                    "<Project><PropertyGroup><" + property + ">" + value + "</" + property
                            + "></PropertyGroup></Project>").ShouldContain(
                                    elementPredicate: error => error.StartsWith(
                                            value: "SUPPRESSION_BUILD Subject.TARGETS:1",
                                            comparisonType: StringComparison.Ordinal));
        }

        foreach (string path in new[] { "compiler.rsp", ".github/workflows/build.yml" })
        {
            SuppressionPolicy.CheckConfiguration(path: path, content: "dotnet build -warnaserror\n").ShouldBeEmpty();
            IReadOnlyList<string> errors = SuppressionPolicy.CheckConfiguration(path: path,
                    content: "dotnet build -p:TreatWarningsAsErrors=false\n");
            errors.ShouldContain(expected: "SUPPRESSION_COMMAND " + path + ":1", comparer: StringComparer.Ordinal);
        }

        ProofProtocol.Record(id: "R-67.negative", diagnostic: "SUPPRESSION_ENTRY_POINTS_REJECTED",
                location: "Subject.cs(1,1), Subject.globalconfig(2,1), Subject.TARGETS:1, compiler.rsp:1, build.yml:1");
    }
}
