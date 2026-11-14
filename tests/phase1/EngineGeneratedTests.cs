using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves exact observed source, producer and adjacent boundaries for native fixture generation.</summary>
public sealed class EngineGeneratedTests
{
    /// <summary>Checks every actual emitted native fixture span and the pinned authentic producer payload.</summary>
    [Fact, Trait("Case", "engine.generated-origin")]
    public void Check_EmittedEngineSpans_RequiresAuthenticatedOrigin()
    {
        (int Exit, string Output) build = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2],
                "--no-restore", "-p:NuGetAudit=false");
        build.Exit.ShouldBe(expected: 0, customMessage: build.Output);
        using JsonDocument registry = Registry();
        JsonElement[] rows = [.. registry.RootElement.GetProperty(propertyName: "local").EnumerateArray()];
        rows.Length.ShouldBe(expected: 170);
        foreach (JsonElement row in rows)
        {
            string file = Local(path: Text(row: row, name: "surface"));
            Hash(bytes: File.ReadAllBytes(path: file)).ShouldBe(expected: Text(row: row, name: "sha256"));
            string source = File.ReadAllText(path: file);
            Hash(bytes: Encoding.UTF8.GetBytes(s: source)).ShouldBe(expected: Text(row: row, name: "textSha256"));
            Hash(bytes: File.ReadAllBytes(path: Local(path: Text(row: row, name: "package")))).ShouldBe(
                    expected: "ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e");
            Text(row: row, name: "producer").ShouldBe(expected: "Godot.SourceGenerators/4.7.2");
            SyntaxTree tree = CSharpSyntaxTree.ParseText(text: source, path: file,
                    options: new CSharpParseOptions(preprocessorSymbols: ["TOOLS"]),
                    cancellationToken: TestContext.Current.CancellationToken);
            string member = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken)
                    .FindNode(span: Span(row: row, tree: tree)).AncestorsAndSelf()
                    .OfType<MethodDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText
                    ?? "AssemblyHasScriptsAttribute";
            Text(row: row, name: "member").ShouldBe(expected: member);
            Permitted(row: row, tree: tree, span: Span(row: row, tree: tree)).ShouldBeTrue(
                    customMessage: Text(row: row, name: "id"));
        }

        ExemptionValidator.ReadPermissions().Count.ShouldBe(expected: 18);
        JsonObject exemptions = RegisterValidator.Read(relative: "docs/code/exemptions.json");
        JsonObject register = RegisterValidator.Read(relative: "docs/code/register.json");
        register["rules"]?.AsArray().Single(predicate: rule => StringComparer.Ordinal.Equals(
                x: rule?["sourceId"]?.GetValue<string>(), y: "R-50"))?["engineProofPermissions"]?.ToJsonString()
                .ShouldBe(expected: exemptions["engineProofPermissions"]?.ToJsonString());
        ProofProtocol.Record(id: "engine.generated-origin", diagnostic: "ENGINE_GENERATED_ORIGIN",
                location: "docs/code/engine-generated-spans.json:170 exact spans; actual emitted files and package");
    }

    /// <summary>Rejects changed bytes, adjacent spans and handwritten impostors for every permission.</summary>
    [Fact, Trait("Case", "engine.generated-boundary")]
    public void Check_EngineSpanPermissions_RejectsAdjacentImpostors()
    {
        using JsonDocument registry = Registry();
        foreach (JsonElement row in registry.RootElement.GetProperty(propertyName: "local").EnumerateArray())
        {
            string path = Local(path: Text(row: row, name: "surface"));
            string source = File.ReadAllText(path: path);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(text: source, path: path,
                    cancellationToken: TestContext.Current.CancellationToken);
            TextSpan span = Span(row: row, tree: tree);
            Permitted(row: row, tree: CSharpSyntaxTree.ParseText(text: source + "\n", path: path,
                    cancellationToken: TestContext.Current.CancellationToken), span: span)
                    .ShouldBeFalse();
            Permitted(row: row, tree: CSharpSyntaxTree.ParseText(text: source, path: "Subject.generated.cs",
                    cancellationToken: TestContext.Current.CancellationToken),
                    span: span).ShouldBeFalse();
            Permitted(row: row, tree: tree, span: new TextSpan(start: span.Start + 1, length: span.Length))
                    .ShouldBeFalse();
            Permitted(row: row, tree: tree, span: new TextSpan(start: span.Start, length: span.Length + 1))
                    .ShouldBeFalse();
        }

        RejectSealingNeighbor();
        ProofProtocol.Record(id: "engine.generated-boundary", diagnostic: "ENGINE_GENERATED_BOUNDARY",
                location: "170 exact changed-source/path/span boundaries; adjacent Subject.cs CA1852");
    }

    /// <summary>Builds a genuine companion before rejecting a handwritten partial call at its exact location.</summary>
    [Fact, Trait("Case", "engine.handwritten-partial")]
    public void Build_HandwrittenEnginePartial_RetainsNamedArgumentRule()
    {
        var fixture = new BuildFixture(game: true);
        fixture.Restore();
        string subject = Path.Combine(path1: fixture.DirectoryPath, path2: "src/godot-game/Subject.cs");
        string companion = Path.Combine(path1: fixture.DirectoryPath,
                path2: "src/godot-game/companion/LifecycleFixture.cs");
        Directory.CreateDirectory(path: Path.GetDirectoryName(path: companion)
                ?? throw new InvalidOperationException(message: "COMPANION_DIRECTORY"));
        const string Source = "namespace KingdomZombies.Proofs;\ninternal partial class LifecycleFixture\n{\n"
                + "    private static LifetimeToken Bad()\n    {\n"
                + "        return new LifetimeToken { Value = System.Math.Abs(value: -1) };\n    }\n}\n";
        fixture.Write(source: Source);
        File.Move(sourceFileName: subject, destFileName: companion);
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        File.WriteAllText(path: companion, contents: Source.Replace(oldValue: "value: -1", newValue: "-1",
                comparisonType: StringComparison.Ordinal));
        (int Exit, string Output) bad = fixture.Build();
        BuildFixture.HasDiagnostic(result: bad, id: "SCR0001", line: 6, column: 60, file: "LifecycleFixture.cs")
                .ShouldBeTrue(customMessage: bad.Output);
        ProofProtocol.Record(id: "engine.handwritten-partial", diagnostic: "SCR0001",
                location: companion + "(6,60)");
    }

    /// <summary>A fake generated marker cannot suppress the intended diagnostic in an actual strict consumer.</summary>
    [Fact, Trait("Case", "engine.fake-generated")]
    public void Build_FakeEngineGeneration_RetainsExactDiagnostic()
    {
        var fixture = new BuildFixture(game: true);
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        const string Source = "// <auto-generated/>\nnamespace KingdomZombies.Proofs;\n"
                + "internal static class Subject\n{\n    internal static int Read()\n    {\n"
                + "        return System.Math.Abs(-1);\n    }\n}\n";
        fixture.Write(source: Source);
        (int Exit, string Output) bad = fixture.Build();
        BuildFixture.HasDiagnostic(result: bad, id: "SCR0001", line: 7, column: 32).ShouldBeTrue(
                customMessage: bad.Output);
        ExemptionValidator.CheckSource(path: "Subject.cs", content: Source).Single().ShouldContain(
                expected: "GENERATION_UNPROVEN Subject.cs(1,1)");
        ProofProtocol.Record(id: "engine.fake-generated", diagnostic: "SCR0001",
                location: fixture.DirectoryPath + "/src/godot-game/Subject.cs(7,32)");
    }

    private static readonly MethodInfo _permission = PermissionMethod();

    private static void RejectSealingNeighbor()
    {
        var fixture = new BuildFixture(game: true);
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        fixture.Write(source: BuildFixture.CleanSource.Replace(oldValue: "internal static class Subject",
                newValue: "internal class Subject", comparisonType: StringComparison.Ordinal));
        (int Exit, string Output) bad = fixture.Build();
        BuildFixture.HasDiagnostic(result: bad, id: "CA1852", line: 4, column: 16).ShouldBeTrue(
                customMessage: bad.Output);
    }

    private static JsonDocument Registry()
    {
        return JsonDocument.Parse(json: File.ReadAllText(path: Local(path: "docs/code/engine-generated-spans.json")));
    }

    private static MethodInfo PermissionMethod()
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Local(
                path: "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        return assembly.GetType(name: "Sebreiro.CodeRules.Analyzers.GeneratedSyntax")?.GetMethod(name: "IsPermitted",
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException(message: "ENGINE_PERMISSION_METHOD");
    }

    private static bool Permitted(JsonElement row, SyntaxTree tree, TextSpan span)
    {
        return (bool)(_permission.Invoke(obj: null, parameters: [Text(row: row, name: "diagnostic"), tree, span])
                ?? throw new InvalidOperationException(message: "ENGINE_PERMISSION_RESULT"));
    }

    private static TextSpan Span(JsonElement row, SyntaxTree tree)
    {
        return new(start: tree.GetText().Lines[row.GetProperty(propertyName: "line").GetInt32() - 1].Start
                + row.GetProperty(propertyName: "column").GetInt32() - 1,
                length: row.GetProperty(propertyName: "length").GetInt32());
    }

    private static string Text(JsonElement row, string name)
    {
        return row.GetProperty(propertyName: name).GetString()
                ?? throw new InvalidOperationException(message: "ENGINE_PERMISSION_FIELD " + name);
    }

    private static string Local(string path)
    {
        return Path.Combine(path1: ProofProtocol.Root, path2: path);
    }

    private static string Hash(byte[] bytes)
    {
        return Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes));
    }
}
