using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Builds isolated port, provider and service-locator subjects under the real inherited policy.</summary>
internal static class BoundaryFixture
{
    /// <summary>Rejects a real concrete provider type referenced from compiled Core.</summary>
    internal static void CoreProvider(string id)
    {
        var fixture = new GraphFixture();
        const string Provider = "src/provider/Provider.csproj";
        fixture.Write(relative: Provider, content: "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>"
                + "<TargetFramework>net10.0</TargetFramework><AssemblyName>KingdomZombies.Provider</AssemblyName>"
                + "</PropertyGroup></Project>\n");
        fixture.Write(relative: "src/provider/Provider.cs", content: "namespace Boundary;\n\n"
                + "/// <summary>Concrete isolated provider probe.</summary>\npublic sealed class Provider\n{\n}\n");
        fixture.Compile(project: Provider, restore: true);
        fixture.Append(project: ProjectEvaluator.Expected[0], xml: "<ItemGroup><ProjectReference Include=\""
                + "../provider/Provider.csproj\"/></ItemGroup>");
        fixture.Write(relative: "src/core/Subject.cs", content: BuildFixture.CleanSource.Replace(oldValue:
                "return 1;", newValue: "return typeof(Boundary.Provider).Name.Length;",
                comparisonType: StringComparison.Ordinal));
        fixture.Compile(project: ProjectEvaluator.Expected[0], restore: true);
        string[] errors = [..TypeBoundaryScanner.Validate(assembly: Path.Combine(path1: fixture.Root,
                path2: "src/core/bin/Debug/net10.0/KingdomZombies.Core.dll"),
                expectedType: "KingdomZombies.Core.ProofValue", boundary: "core")];
        errors.ShouldContain(elementPredicate: error => error.Contains(value: "KingdomZombies.Provider",
                comparisonType: StringComparison.Ordinal));
        ProofProtocol.Record(id: id, diagnostic: "TYPE_FORBIDDEN",
                location: string.Join(separator: "\n", values: errors));
    }

    /// <summary>Compiles an isolated provider and checks its explicitly limited edges.</summary>
    internal static void Provider(string id, bool negative, bool reusable = false)
    {
        var fixture = new GraphFixture();
        string project = "src/provider/Provider.csproj";
        string name = reusable ? "Sebreiro.Helper" : "KingdomZombies.Provider";
        string ports = reusable ? "" : "<ProjectReference Include=\"../multiplayer-abstractions/"
                + "KingdomZombies.Multiplayer.Abstractions.csproj\"/>";
        fixture.Write(relative: project, content: "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>"
                + "<TargetFramework>net10.0</TargetFramework><AssemblyName>" + name
                + "</AssemblyName></PropertyGroup><ItemGroup>" + ports + "</ItemGroup></Project>\n");
        string source = "namespace Boundary;\n\ninternal static class Provider\n{\n"
                + "    internal static int Read()\n    {\n        return 1;\n    }\n}\n";
        if (!reusable)
        {
            source = "namespace Boundary;\n\ninternal sealed class Provider : "
                    + "KingdomZombies.Multiplayer.Abstractions.IProofService\n{\n"
                    + "    /// <summary>Implements only the independent service port.</summary>\n"
                    + "    public int Echo(int value)\n    {\n        return value;\n    }\n}\n";
        }
        fixture.Write(relative: "src/provider/Provider.cs", content: source);
        fixture.Compile(project: project, restore: true);
        if (negative)
        {
            fixture.Append(project: project, xml: "<ItemGroup><ProjectReference Include=\""
                    + "../core/KingdomZombies.Core.csproj\"/></ItemGroup>");
            fixture.Write(relative: "src/provider/Provider.cs", content: source.Replace(oldValue: reusable
                    ? "return 1;" : "return value;",
                    newValue: "return typeof(KingdomZombies.Core.ProofValue).Name.Length;",
                    comparisonType: StringComparison.Ordinal));
            fixture.Compile(project: project, restore: true);
        }

        string[] errors = [..TypeBoundaryScanner.Validate(assembly: Path.Combine(path1: fixture.Root,
                path2: "src/provider/bin/Debug/net10.0/" + name + ".dll"), expectedType: "Boundary.Provider",
                boundary: reusable ? "analyzer" : "provider")];
        string[] graph = [.. ProjectEvaluator.ValidateProvider(root: fixture.Root, project: project,
                reusable: reusable)];
        if (negative)
        {
            errors.ShouldContain(elementPredicate: error => error.StartsWith(value: "TYPE_FORBIDDEN",
                    comparisonType: StringComparison.Ordinal));
            graph.ShouldContain(elementPredicate: error => error.StartsWith(value: "GRAPH_FORBIDDEN",
                    comparisonType: StringComparison.Ordinal));
        }
        else
        {
            errors.ShouldBeEmpty();
            graph.ShouldBeEmpty();
        }

        ProofProtocol.Record(id: id, diagnostic: negative ? "TYPE_FORBIDDEN" : "BOUNDARY_ALLOWED",
                location: project + "\n" + string.Join(separator: "\n", values: errors));
    }

    /// <summary>Requires a compiled service contract to reject Core values, enums and real built game types.</summary>
    internal static void Port(string id, string dependency)
    {
        var fixture = new GraphFixture();
        bool engine = StringComparer.Ordinal.Equals(x: dependency, y: "engine");
        if (StringComparer.Ordinal.Equals(x: dependency, y: "enum"))
        {
            fixture.Write(relative: "src/core/BoundaryKind.cs", content: File.ReadAllText(path: Path.Combine(
                    path1: ProofProtocol.Root, path2: "tests/phase1/Models/BoundaryKind.cs")).Replace(
                            oldValue: "internal enum", newValue: "public enum",
                            comparisonType: StringComparison.Ordinal)
                    .Replace(oldValue: "    [JsonString", newValue: "    /// <summary>Pinned value.</summary>\n"
                            + "    [JsonString", comparisonType: StringComparison.Ordinal));
        }

        string type = engine ? "KingdomZombies.Proofs.Phase1" : dependency is "enum"
                ? "KingdomZombies.Phase1.Tests.Models.BoundaryKind" : "KingdomZombies.Core.ProofValue";
        string reference = engine
                ? "<Reference Include=\"KingdomZombies\"><HintPath>"
                        + "../godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll</HintPath>"
                        + "<Private>false</Private></Reference>"
                : "<ProjectReference Include=\"../core/KingdomZombies.Core.csproj\"/>";
        fixture.Append(project: ProjectEvaluator.Expected[1], xml: "<ItemGroup>" + reference + "</ItemGroup>");
        fixture.Write(relative: "src/multiplayer-abstractions/Subject.cs", content:
                "namespace Boundary;\n\ninternal interface Subject\n{\n    " + type + " Read();\n}\n");
        fixture.Compile(project: ProjectEvaluator.Expected[1], restore: true);
        string assembly = Path.Combine(path1: fixture.Root,
                path2: "src/multiplayer-abstractions/bin/Debug/net10.0/KingdomZombies.Multiplayer.Abstractions.dll");
        string[] errors = [..TypeBoundaryScanner.Validate(assembly: assembly,
                expectedType: "KingdomZombies.Multiplayer.Abstractions.IProofService", boundary: "ports")];
        errors.ShouldContain(elementPredicate: error => error.StartsWith(value: "TYPE_FORBIDDEN",
                comparisonType: StringComparison.Ordinal));
        ProofProtocol.Record(id: id, diagnostic: "TYPE_FORBIDDEN",
                location: string.Join(separator: "\n", values: errors));
    }

    /// <summary>Compiles real locator use before checking its resolved alias and containing type.</summary>
    internal static void Locator(string id, bool negative)
    {
        var fixture = new BuildFixture();
        string owner = negative ? "Outside" : "CompositionRoot";
        string source = "using Provider = System.IServiceProvider;\n\nnamespace KingdomZombies;\n\n"
                + "internal static class " + owner + "\n{\n    internal static object? Read(Provider provider)\n"
                + "    {\n        return provider.GetService(serviceType: typeof(int));\n    }\n}\n";
        string clean = "namespace KingdomZombies;\n\ninternal static class CompositionRoot\n{\n"
                + "    internal static int Read(int value)\n    {\n        return value;\n    }\n}\n";
        File.WriteAllText(path: Path.Combine(path1: fixture.DirectoryPath, path2: "src/core/CompositionRoot.cs"),
                contents: clean);
        fixture.Restore();
        (int Exit, string Output) control = fixture.Build();
        control.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_CLEAN: " + control.Output);
        if (!negative)
        {
            source = clean;
        }

        File.WriteAllText(path: Path.Combine(path1: fixture.DirectoryPath, path2: "src/core/" + owner + ".cs"),
                contents: source);
        (int Exit, string Output) built = fixture.Build();
        if (negative)
        {
            BuildFixture.HasDiagnostic(result: built, id: "RS0030", line: 9, column: 16, file: "Outside.cs")
                    .ShouldBeTrue(customMessage: built.Output);
        }
        else
        {
            built.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_SUBJECT: " + built.Output);
        }
        var consumer = ConsumerCompilation.Read(root: fixture.DirectoryPath, project: ProjectEvaluator.Expected[0]);
        var compilation = CSharpCompilation.Create(assemblyName: "LocatorProbe",
                syntaxTrees: [consumer.Parse(source: source, path: owner + ".cs")],
                references: consumer.References, options: new CSharpCompilationOptions(
                        outputKind: OutputKind.DynamicallyLinkedLibrary,
                        nullableContextOptions: NullableContextOptions.Enable));
        compilation.GetDiagnostics().Where(predicate: diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .ShouldBeEmpty();
        string[] errors = [.. ArchitecturePolicy.ServiceLocator(compilation: compilation)];
        if (negative)
        {
            errors.ShouldNotBeEmpty();
        }
        else
        {
            errors.ShouldBeEmpty();
        }

        ProofProtocol.Record(id: id, diagnostic: negative ? "R-60" : "COMPOSITION_ALLOWED",
                location: owner + ".cs\n" + string.Join(separator: "\n", values: errors));
    }
}
