using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Names independent actual mechanisms, distinct from historical planned register locators.</summary>
internal static class RuleDefinitions
{
    /// <summary>Resolves an applicable exact rule identity to a current source symbol or literal setting.</summary>
    internal static (string Path, string Symbol) Expected(string id, string plannedPath, string plannedSymbol)
    {
        int number = int.Parse(s: id[2..], provider: CultureInfo.InvariantCulture);
        if (_settings.TryGetValue(key: number, value: out string? setting))
        {
            return (".editorconfig", setting);
        }

        if (StringComparer.Ordinal.Equals(x: plannedPath, y:
                "src/godot-game/tests/verification/TypeBoundaryScanner.cs"))
        {
            return (plannedPath, "TypeBoundaryScanner.Validate(assembly,expectedTypes,boundary)");
        }

        return number switch
        {
            1 => ("src/godot-game/Directory.Build.props", "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>"),
            >= 2 and <= 6 => ("src/godot-game/tests/verification/SourceFileFormat.cs", "SourceFileFormat.Check"),
            7 or 8 => ("src/godot-game/tests/verification/SourceFileFormat.cs",
                    "SourceFileFormat.RequireEditorSetting"),
            16 => ("src/godot-game/Directory.Build.props", "<ImplicitUsings>disable</ImplicitUsings>"),
            34 => ("src/godot-game/tests/verification/ArchitecturePolicy.cs",
                    "ArchitecturePolicy.VisibilityRepository"),
            35 => ("src/godot-game/Directory.Build.props",
                    "<GenerateDocumentationFile>true</GenerateDocumentationFile>"),
            39 or 73 => ("src/godot-game/build/BannedSymbols.txt", "P:System.DateTime.UtcNow;Use an injected clock"),
            40 => ("src/godot-game/build/BannedSymbols.txt", "T:System.Console;Use an explicit log/report adapter"),
            41 => ("src/godot-game/build/BannedSymbols.txt",
                "M:System.Threading.Thread.Sleep(System.Int32);Use cancellable asynchronous coordination"),
            42 => ("src/godot-game/build/BannedSymbols.txt", "M:System.Threading.Tasks.Task.Wait;Use await"),
            46 => ("src/godot-game/Directory.Build.targets", "<ProjectReference"),
            47 => ("src/godot-game/build/BannedSymbols.Tests.txt", "T:Xunit.Assert;Use Shouldly"),
            48 => ("src/godot-game/tests/verification/TestSourceIndex.cs",
                    "TestSourceIndex.FollowsMethodStateExpected"),
            49 or 82 => ("tools/phase1/coverage-policy.json", id),
            50 => ("src/godot-game/tests/verification/ExemptionValidator.cs", "ExemptionValidator.CheckRepository"),
            53 => ("src/godot-game/Directory.Build.props",
                "<DisableTransitiveProjectReferences>true</DisableTransitiveProjectReferences>"),
            67 => ("src/godot-game/tests/verification/SuppressionPolicy.cs", "SuppressionPolicy.CheckRepository"),
            68 => ("src/godot-game/core/SnapshotCodec.cs", "SnapshotCodec.Encode"),
            >= 69 and <= 72 => ("src/godot-game/core/SnapshotCodec.cs", "SnapshotCodec.Decode"),
            83 => ("src/godot-game/tests/verification/DocumentationValidator.cs", "DocumentationValidator.Check"),
            84 => ("src/godot-game/tests/verification/DocumentationDecisions.cs", "DocumentationDecisions.Check"),
            _ => (plannedPath, plannedPath.Split(separator: '/').Last()[..^3] + "."
                    + (plannedSymbol.EndsWith(value: "Analyzer", comparisonType: StringComparison.Ordinal)
                            ? "Initialize" : plannedSymbol))
        };
    }

    private static readonly Dictionary<int, string> _settings = new()
    {
        [11] = "dotnet_diagnostic.IDE0161.severity = error",
        [12] = "dotnet_diagnostic.IDE0011.severity = error",
        [13] = "dotnet_diagnostic.IDE0065.severity = error",
        [15] = "dotnet_diagnostic.IDE0005.severity = error",
        [17] = "dotnet_diagnostic.IDE0008.severity = error",
        [19] = "dotnet_diagnostic.IDE0090.severity = error",
        [20] = "dotnet_diagnostic.IDE0300.severity = error",
        [21] = "dotnet_diagnostic.RCS1248.severity = error",
        [22] = "dotnet_diagnostic.IDE0022.severity = error",
        [27] = "dotnet_naming_rule.private_fields_are_underscore_camel_case.style = underscore_camel_case_style",
        [29] = "dotnet_naming_rule.constants_are_pascal_case.style = pascal_case_style",
        [30] = "dotnet_diagnostic.MA0137.severity = none",
        [31] = "dotnet_diagnostic.CA1852.severity = error",
        [32] = "dotnet_diagnostic.MA0048.severity = error",
        [44] = "dotnet_diagnostic.CA1848.severity = error",
        [45] = "dotnet_diagnostic.MA0004.severity = error"
    };
}
