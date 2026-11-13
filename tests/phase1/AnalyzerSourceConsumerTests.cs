using System;
using System.IO;
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
}
