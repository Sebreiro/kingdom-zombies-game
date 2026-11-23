using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Builds the analyzer's own source as a consumer of the built analyzer rather than assuming
/// self-analysis.</summary>
public sealed class AnalyzerSourceConsumerTests
{
    /// <summary>Every owned analyzer source file passes all currently implemented rules in an actual
    /// compiler.</summary>
    [Fact, Trait("Case", "rules.analyzer-source-consumer")]
    public void Analyze_OwnedAnalyzerSources_PassesAllTwelveRules()
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol
                .Root, path2: "src/godot-game/tools/code-rules/bin/Debug/netstandard2.0/"
                        + "Sebreiro.CodeRules.Analyzers.dll")));
        IEnumerable<Type> types = assembly.GetTypes().Where(predicate: type => type
                .GetCustomAttribute<DiagnosticAnalyzerAttribute>() is not null);
        DiagnosticDescriptor[] descriptors = [..types.SelectMany(selector: type => LoadAnalyzer(type: type)
                .SupportedDiagnostics).OrderBy(keySelector: rule => rule.Id, comparer: StringComparer.Ordinal)];
        descriptors.Select(selector: rule => rule.Id).ShouldBe(expected: _diagnosticIds);
        descriptors.All(predicate: rule => rule.DefaultSeverity == DiagnosticSeverity.Error && rule
                .IsEnabledByDefault).ShouldBeTrue();
        string project = CreateConsumer();
        (int Exit, string Output) restore = ConfigurationTests.Dotnet("restore", project,
                "-p:NuGetAudit=false", "--ignore-failed-sources");
        restore.Exit.ShouldBe(expected: 0, customMessage: restore.Output);
        (int Exit, string Output) build = ConfigurationTests.Dotnet("build", project, "--no-restore",
                "-p:NuGetAudit=false");
        build.Exit.ShouldBe(expected: 0, customMessage: build.Output);
        ProofProtocol.Record(id: "rules.analyzer-source-consumer",
                diagnostic: "ANALYZER_SOURCE_CONSUMER_BUILT",
                location: ("actual throwaway consumer includes every tools/code-rul"
                        + "es source and built custom analyzer"));
    }

    /// <summary>The separate source consumer rejects its own violating subject under the built analyzer.</summary>
    [Fact, Trait("Case", "rules.analyzer-source-boundary")]
    public void Analyze_AnalyzerConsumerViolation_RetainsScr0006()
    {
        string project = CreateConsumer();
        (int Exit, string Output) restore = ConfigurationTests.Dotnet("restore", project,
                "-p:NuGetAudit=false", "--ignore-failed-sources");
        restore.Exit.ShouldBe(expected: 0, customMessage: restore.Output);
        (int Exit, string Output) clean = ConfigurationTests.Dotnet("build", project, "--no-restore",
                "-p:NuGetAudit=false");
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        string source = BuildFixture.CleanSource.Replace(oldValue: "        return 1;",
                newValue: new string(c: '\n', count: 60) + "        return 1;",
                comparisonType: StringComparison.Ordinal);
        File.WriteAllText(path: Path.Combine(path1: Path.GetDirectoryName(path: project)
                ?? throw new InvalidOperationException(message: "Consumer directory required"), path2: "Subject.cs"),
                contents: source);
        string xml = File.ReadAllText(path: project).Replace(oldValue: "</ItemGroup>",
                newValue: "<Compile Include=\"Subject.cs\"/></ItemGroup>", comparisonType: StringComparison.Ordinal);
        File.WriteAllText(path: project, contents: xml);
        (int Exit, string Output) changed = ConfigurationTests.Dotnet("build", project, "--no-restore",
                "-p:NuGetAudit=false");
        BuildFixture.HasDiagnostic(result: changed, id: "SCR0006", line: 6, column: 25).ShouldBeTrue(
                customMessage: changed.Output);
        ProofProtocol.Record(id: "rules.analyzer-source-boundary", diagnostic: "ANALYZER_SOURCE_BOUNDARY_SCR0006",
                location: project + ": Subject.cs(6,25), all owned analyzer sources included");
    }

    private static readonly string[] _diagnosticIds = ["SCR0001", "SCR0002", "SCR0003", "SCR0004", "SCR0005",
        "SCR0006", "SCR0007", "SCR0008", "SCR0009", "SCR0010", "SCR0011", "SCR0012"];

    private static string CreateConsumer()
    {
        string directory = WorkspacePaths.Temp(purpose: "analyzer-source-" + Guid.NewGuid().ToString(format: "N"));
        Directory.CreateDirectory(path: directory);
        string project = Path.Combine(path1: directory, path2: "AnalyzerConsumer.csproj");
        string sources = Path.Combine(path1: ProofProtocol.Root, path2: "src/godot-game/tools/code-rules").Replace(
                oldChar: '\\', newChar: '/');
        string xml = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>"
                + "<TargetFramework>netstandard2.0</TargetFramework><EnforceExtendedAnalyzerRules>true"
                + "</EnforceExtendedAnalyzerRules><IsRoslynComponent>true</IsRoslynComponent>"
                + "<EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup>"
                + "<PackageReference Include=\"Microsoft.CodeAnalysis.CSharp\"/>"
                + "<PackageReference Include=\"Microsoft.CodeAnalysis.Analyzers\"/><Compile Include=\""
                + sources + "/*.cs\"/><AdditionalFiles Include=\"" + sources
                + "/AnalyzerReleases.*.md\"/></ItemGroup></Project>";
        File.WriteAllText(path: project, contents: xml);
        return project;
    }
    private static DiagnosticAnalyzer LoadAnalyzer(Type type)
    {
        return Activator.CreateInstance(type: type) as DiagnosticAnalyzer
                ?? throw new InvalidOperationException(message: "Registered analyzer cannot be loaded");
    }
}
