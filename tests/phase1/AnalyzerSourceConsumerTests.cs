using System;
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
    public void AnalyzerSourcesPassTheirOwnRules()
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        var types = assembly.GetTypes().Where(predicate: type =>
                type.GetCustomAttribute<DiagnosticAnalyzerAttribute>() is not null);
        var descriptors = types.SelectMany(selector: type => LoadAnalyzer(type: type).SupportedDiagnostics)
                .OrderBy(keySelector: rule => rule.Id, comparer: StringComparer.Ordinal).ToArray();
        descriptors.Select(selector: rule => rule.Id).ShouldBe(expected: DiagnosticIds);
        descriptors.All(predicate: rule => rule.DefaultSeverity == DiagnosticSeverity.Error
                && rule.IsEnabledByDefault).ShouldBeTrue();
        string directory = Path.Combine(path1: ProofProtocol.Root,
                path2: ".phase1/temp/analyzer-source-" + Guid.NewGuid().ToString(format: "N"));
        Directory.CreateDirectory(path: directory);
        string project = Path.Combine(path1: directory, path2: "AnalyzerConsumer.csproj");
        string sources = Path.Combine(path1: ProofProtocol.Root, path2: "tools/code-rules").Replace(
                oldChar: '\\', newChar: '/');
        string xml = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>"
                + ("<TargetFramework>netstandard2.0</TargetFramework><EnforceExten" +
                        "dedAnalyzerRules>true</EnforceExtendedAnalyzerRules>")
                + ("<IsRoslynComponent>true</IsRoslynComponent><EnableDefau" +
                        "ltCompileItems>false</EnableDefaultCompileItems>")
                + ("</PropertyGroup><ItemGroup><PackageReference Include=\"M" +
                        "icrosoft.CodeAnalysis.CSharp\"/>")
                + "<PackageReference Include=\"Microsoft.CodeAnalysis.Analyzers\"/>"
                + "<Compile Include=\"" + sources + "/*.cs\"/>"
                + "<AdditionalFiles Include=\"" + sources + "/AnalyzerReleases.*.md\"/></ItemGroup></Project>";
        File.WriteAllText(path: project, contents: xml);
        var restore = ConfigurationTests.Dotnet("restore", project, "-p:NuGetAudit=false", "--ignore-failed-sources");
        restore.Exit.ShouldBe(expected: 0, customMessage: restore.Output);
        var build = ConfigurationTests.Dotnet("build", project, "--no-restore", "-p:NuGetAudit=false");
        build.Exit.ShouldBe(expected: 0, customMessage: build.Output);
        ProofProtocol.Record(id: "rules.analyzer-source-consumer", diagnostic: "ANALYZER_SOURCE_CONSUMER_BUILT",
                location: ("actual throwaway consumer includes every tools/code-rul" +
                        "es source and built custom analyzer"));
    }

    private static readonly string[] DiagnosticIds =
    [
        "SCR0001", "SCR0002", "SCR0003", "SCR0004", "SCR0005", "SCR0006",
        "SCR0007", "SCR0008", "SCR0009", "SCR0010", "SCR0011", "SCR0012"
    ];

    private static DiagnosticAnalyzer LoadAnalyzer(Type type)
    {
        return Activator.CreateInstance(type: type) as DiagnosticAnalyzer
                ?? throw new InvalidOperationException(message: "Registered analyzer cannot be loaded");
    }
}
