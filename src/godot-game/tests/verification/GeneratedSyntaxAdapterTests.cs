using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves the two observed generated syntax spans without granting a member or folder exemption.</summary>
public sealed class GeneratedSyntaxAdapterTests
{
    /// <summary>Requires exact package/raw output identities and complete Roslyn text plus precise syntax
    /// spans.</summary>
    [Fact, Trait("Case", "rules.generated-syntax-origin")]
    public void Check_GeneratedSyntax_HasPinnedOrigin()
    {
        using JsonDocument registry = Registry();
        foreach (JsonElement row in registry.RootElement.GetProperty(propertyName: "local").EnumerateArray())
        {
            Hash(path: Text(row: row, name: "surface")).ShouldBe(expected: Text(row: row, name: "sha256"));
            Hash(path: Text(row: row, name: "package")).ShouldBe(expected: Text(row: row, name: "packageSha256"));
            string source = File.ReadAllText(path: Local(path: Text(row: row, name: "surface")));
            Convert.ToHexStringLower(inArray: SHA256.HashData(source: Encoding.UTF8.GetBytes(s: source)))
                    .ShouldBe(expected: Text(row: row, name: "textSha256"));
            SyntaxTree tree = CSharpSyntaxTree.ParseText(cancellationToken: TestContext.Current
                    .CancellationToken, text: source, path: Local(path: Text(row: row, name: "surface")));
            Permitted(row: row, tree: tree, span: Span(row: row, tree: tree)).ShouldBeTrue();
        }

        (int Exit, string Output) build = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2],
                "--no-restore", "-p:NuGetAudit=false");
        build.Exit.ShouldBe(expected: 0, customMessage: build.Output);
        ProofProtocol.Record(id: "rules.generated-syntax-origin", diagnostic: "GENERATED_SYNTAX_ORIGIN_EXACT",
                location: ("two exact Godot source/package hashes and diagnostic/pa" + "th/text/span permissions"));
    }

    /// <summary>Changed content/path/span and same-span handwritten impostors fail; adjacent and colliding
    /// members fail actual builds.</summary>
    [Fact, Trait("Case", "rules.generated-syntax-boundary")]
    public async Task Check_GeneratedSyntaxPermissions_StayBounded()
    {
        using JsonDocument registry = Registry();
        foreach (JsonElement row in registry.RootElement.GetProperty(propertyName: "local").EnumerateArray())
        {
            string path = Local(path: Text(row: row, name: "surface"));
            string source = File.ReadAllText(path: path);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(cancellationToken: TestContext.Current
                    .CancellationToken, text: source, path: path);
            TextSpan span = Span(row: row, tree: tree);
            Permitted(row: row, tree: CSharpSyntaxTree.ParseText(cancellationToken: TestContext.Current
                    .CancellationToken, text: source + "\n", path: path), span: span).ShouldBeFalse();
            Permitted(row: row, tree: CSharpSyntaxTree.ParseText(cancellationToken: TestContext.Current
                    .CancellationToken, text: source, path: "Subject.cs"), span: span).ShouldBeFalse();
            Permitted(row: row, tree: tree, span: new TextSpan(start: span.Start + 1, length: span.Length))
                    .ShouldBeFalse();
            Permitted(row: row, tree: tree, span: new TextSpan(start: span.Start, length: span.Length + 1))
                    .ShouldBeFalse();
            await RejectSameSpanAsync(row: row, path: path).ConfigureAwait(continueOnCapturedContext: true);
            string collision = ConfigurationTests.BuildGameWithSource(source: Text(row: row, name: "collisionSource"));
            collision.ShouldContain(expected: "error CS0111:");
        }

        RejectAdjacent();
        ProofProtocol.Record(id: "rules.generated-syntax-boundary",
                diagnostic: "GENERATED_SYNTAX_BOUNDARY_REJECTED",
                location: ("mutated bytes/path/span; exact-span Roslyn impostors; a"
                        + "djacent SCR0009/SCR0011; two CS0111"));
    }

    private static async Task RejectSameSpanAsync(JsonElement row, string path)
    {
        string diagnostic = Text(row: row, name: "diagnostic");
        string source = StringComparer.Ordinal.Equals(x: diagnostic, y: "SCR0009")
                ? "using System;\nclass C\n{\n" + new string(c: '\n', count: 7)
                        + "        public void M(\n            IntPtr value) { }\n}"
                : "class C\n{\n    private int _nonce = 1;\n" + new string(c: '\n', count: 19)
                        + "    public int M() => ".PadRight(totalWidth: 80, paddingChar: ' ') + "this.@_nonce;\n}";
        var expected = new AnalyzerVerifier.Expected(id: diagnostic, severity: DiagnosticSeverity.Error,
                file: path, line: Number(row: row, name: "line"), column: Number(row: row, name: "column"),
                endLine: Number(row: row, name: "line"), endColumn: Number(row: row, name: "column") + Number(
                        row: row, name: "length"));
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Local(
                path: "src/godot-game/tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        string name = StringComparer.Ordinal.Equals(x: diagnostic, y: "SCR0009") ? "ContinuationIndentAnalyzer"
                : "ThisQualificationAnalyzer";
        Type type = assembly.GetType(name: "Sebreiro.CodeRules.Analyzers." + name)
                ?? throw new InvalidOperationException(message: "Missing analyzer");
        DiagnosticAnalyzer analyzer = Activator.CreateInstance(type: type) as DiagnosticAnalyzer
                ?? throw new InvalidOperationException(message: "Missing analyzer instance");
        await AnalyzerVerifier.VerifyAsync(source: source, sourcePath: path, expected: [expected],
                analyzer: analyzer).ConfigureAwait(continueOnCapturedContext: true);
    }

    private static void RejectAdjacent()
    {
        string output = ConfigurationTests.BuildGameWithSource(
                source: "namespace KingdomZombies.Proofs; public partial class Phase1 {\n"
                        + "    private string Adjacent() => this._nonce;\n"
                        + "    private static int M(\n           int value) => value;\n}");
        output.ShouldContain(expected: "error SCR0009:");
        output.ShouldContain(expected: "error SCR0011:");
        (int Exit, string Output) clean = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2],
                "--no-restore", "-p:NuGetAudit=false");
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
    }

    private static bool Permitted(JsonElement row, SyntaxTree tree, TextSpan span)
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Local(
                path: "src/godot-game/tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        MethodInfo method = assembly.GetType(name: "Sebreiro.CodeRules.Analyzers.GeneratedSyntax")?.GetMethod(
                name: "IsPermitted", bindingAttr: BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException(message: "Missing syntax permission mechanism");
        return method.Invoke(obj: null, parameters: [Text(row: row, name: "diagnostic"), tree, span]) is true;
    }

    private static TextSpan Span(JsonElement row, SyntaxTree tree)
    {
        int start = tree.GetText().Lines[Number(row: row, name: "line") - 1].Start + Number(row: row,
                name: "column") - 1;
        return new TextSpan(start: start, length: Number(row: row, name: "length"));
    }

    private static JsonDocument Registry()
    {
        return JsonDocument.Parse(json: File.ReadAllText(path: Local(
                path: "docs/code/generated-syntax-adaptations.json")));
    }

    private static string Local(string path)
    {
        return Path.Combine(path1: ProofProtocol.Root, path2: path);
    }

    private static string Text(JsonElement row, string name)
    {
        return row.GetProperty(propertyName: name).GetString() ?? throw new InvalidOperationException(
                message: "Missing registry string");
    }

    private static int Number(JsonElement row, string name)
    {
        return row.GetProperty(propertyName: name).GetInt32();
    }

    private static string Hash(string path)
    {
        return Convert.ToHexStringLower(inArray: SHA256.HashData(source: File.ReadAllBytes(path: Local(path: path))));
    }
}
