using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Gives R-67 independent current repository and deliberately weakened-policy subjects.</summary>
public sealed class SuppressionTests
{
    /// <summary>Checks both actual symbol branches and rejects drift in exact linked-source ownership.</summary>
    [Fact]
    public void Scan_SharedInput_ChecksEveryDeclaredConsumer()
    {
        const string Path = "src/godot-game/game/foundation/FoundationInput.cs";
        var game = ConsumerCompilation.Read(root: ProofProtocol.Root,
                project: "src/godot-game/game/KingdomZombies.csproj");
        var tests = ConsumerCompilation.Read(root: ProofProtocol.Root,
                project: "src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj");
        string symbol = game.Options.PreprocessorSymbolNames.Except(second:
                tests.Options.PreprocessorSymbolNames, comparer: StringComparer.Ordinal).First();
        string source = "#if " + symbol + "\n[System.Diagnostics.CodeAnalysis.SuppressMessage(\"ApiDesign\","
                + "\"SCR0006\")]\n#else\n[System.Diagnostics.CodeAnalysis.SuppressMessage(\"ApiDesign\","
                + "\"SCR0007\")]\n#endif\nclass Subject {}\n";
        IReadOnlyList<string> errors = ExemptionValidator.CheckOwnedSource(path: Path, content: source,
                permissions: null, owners: [tests, game]);
        errors.Count.ShouldBe(expected: 2);
        errors.ShouldContain(elementPredicate: error => error.EndsWith(value: " SCR0006",
                comparisonType: StringComparison.Ordinal));
        errors.ShouldContain(elementPredicate: error => error.EndsWith(value: " SCR0007",
                comparisonType: StringComparison.Ordinal));
        foreach (string[] owners in new string[][] { [], [game.Project], [game.Project, game.Project],
            [game.Project, "undeclared.csproj"], [game.Project, tests.Project, "undeclared.csproj"] })
        {
            Should.Throw<InvalidDataException>(actual: () => ExemptionValidator.RequireOwners(path: Path,
                    projects: owners));
        }

        Should.Throw<InvalidDataException>(actual: () => ExemptionValidator.RequireOwners(path:
                "src/godot-game/core/Subject.cs", projects: [game.Project, tests.Project]));
    }

    /// <summary>Checks tracked inputs and authentic generated permissions without importing donor escapes.</summary>
    [Fact, Trait("Case", "R-67.positive")]
    public void Scan_ActualRepository_AcceptsExactPermissions()
    {
        CheckLinkedOwnership();
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
    private static void CheckLinkedOwnership()
    {
        string project = Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj");
        string directory = Path.GetDirectoryName(path: project)
                ?? throw new InvalidDataException(message: "Linked-source project directory required");
        string[] actual = [..System.Xml.Linq.XDocument.Load(uri: project).Descendants(name: "Compile")
                .Select(selector: item => item.Attribute(name: "Include")?.Value ?? "")
                .Select(selector: path => Path.GetRelativePath(relativeTo: ProofProtocol.Root,
                        path: Path.GetFullPath(path: Path.Combine(path1: directory, path2: path)))
                        .Replace(oldChar: '\\', newChar: '/'))];
        actual.Order(comparer: StringComparer.Ordinal).ShouldBe(expected: FoundationLinkedSources.Paths
                .Order(comparer: StringComparer.Ordinal));
        foreach (string path in FoundationLinkedSources.Paths)
        {
            string primary = path.StartsWith(value: "src/godot-game/game/", comparisonType: StringComparison.Ordinal)
                    ? "src/godot-game/game/KingdomZombies.csproj"
                    : "src/godot-game/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj";
            const string Tests = "src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj";
            ExemptionValidator.RequireOwners(path: path, projects: [primary, Tests]);
            foreach (string[] owners in new string[][] { [], [primary], [Tests], [primary, primary],
                [primary, Tests, "adjacent.csproj"], [primary, "adjacent.csproj"] })
            {
                Should.Throw<InvalidDataException>(actual: () => ExemptionValidator.RequireOwners(path: path,
                        projects: owners));
            }
        }

        Should.Throw<InvalidDataException>(actual: () => ExemptionValidator.RequireOwners(
                path: "src/godot-game/game/foundation/Adjacent.cs", projects:
                ["src/godot-game/game/KingdomZombies.csproj",
                    "src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj"]));
    }
}
