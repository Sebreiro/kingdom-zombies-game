using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves exact immutable source-line and member-scoped generated layout compatibility.</summary>
public sealed class GeneratedLayoutAdapterTests
{
    /// <summary>Checks actual generated/package bytes, origin hashes, exact targets and both normal builds.</summary>
    [Fact, Trait("Case", "rules.generated-layout-origin")]
    public void GeneratedLayoutHasPinnedOrigin()
    {
        using JsonDocument registry = ReadRegistry();
        foreach (JsonElement row in registry.RootElement.GetProperty(propertyName: "sources").EnumerateArray())
        {
            Hash(path: Text(row: row, name: "surface")).ShouldBe(expected: Text(row: row, name: "sha256"));
            Hash(path: Text(row: row, name: "origin")).ShouldBe(expected: Text(row: row, name: "originSha256"));
        }

        foreach (JsonElement row in registry.RootElement.GetProperty(propertyName: "members").EnumerateArray())
        {
            Hash(path: Text(row: row, name: "surface")).ShouldBe(expected: Text(row: row, name: "sha256"));
            Hash(path: Text(row: row, name: "package")).ShouldBe(expected: Text(row: row, name: "packageSha256"));
            string source = File.ReadAllText(path: Local(path: Text(row: row, name: "path")));
            AdapterTargets.Read(source: source).ShouldContain(expected: Text(row: row, name: "target"),
                    comparer: StringComparer.Ordinal);
            source.ShouldContain(expected: Text(row: row, name: "diagnostic"));
        }

        foreach (int index in new[]
        {
            2,
            4
        }

        )
        {
            (int Exit, string Output) build = index == 4 ? ConfigurationTests.Dotnet("msbuild",
                    ConfigurationTests.Projects[index], "-target:Compile", "-p:NuGetAudit=false")
                    : ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[index], "--no-restore",
                            "-p:NuGetAudit=false");
            build.Exit.ShouldBe(expected: 0, customMessage: build.Output);
        }

        ProofProtocol.Record(id: "rules.generated-layout-origin", diagnostic: "GENERATED_LAYOUT_ORIGIN_EXACT",
                location: ("four immutable source files, seven diagnostic/member p"
                        + "ermissions, SDK/package origin"));
    }

    /// <summary>Exact original source passes; identical paths with changed bytes and wrong paths fail exact
    /// spans.</summary>
    [Fact, Trait("Case", "rules.generated-layout-boundary")]
    public async Task GeneratedLayoutPermissionsAreBounded()
    {
        using JsonDocument registry = ReadRegistry();
        MetadataReference[] references = PackageReferences();
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Local(
                path: "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        Type type = assembly.GetType(name: "Sebreiro.CodeRules.Analyzers.LineLengthAnalyzer")
                ?? throw new InvalidOperationException(message: "Missing LineLength analyzer");
        DiagnosticAnalyzer analyzer = Activator.CreateInstance(type: type) as DiagnosticAnalyzer
                ?? throw new InvalidOperationException(message: "Missing analyzer instance");
        foreach (JsonElement row in registry.RootElement.GetProperty(propertyName: "sources").EnumerateArray())
        {
            string path = Local(path: Text(row: row, name: "surface"));
            string source = InheritedAnalyzerVerifier.ReadUtf8(path: path);
            await AnalyzerVerifier.VerifyAsync(source: source, expected: [], sourcePath: path,
                    additionalReferences: references, analyzer: analyzer).ConfigureAwait(
                            continueOnCapturedContext: true);
            string changed = source + "\n//" + new string(c: 'a', count: 119);
            await RejectLinesAsync(source: changed, path: path, analyzer: analyzer, references: references)
                    .ConfigureAwait(continueOnCapturedContext: true);
            await RejectLinesAsync(source: source, path: "Subject.cs", analyzer: analyzer,
                    references: references).ConfigureAwait(continueOnCapturedContext: true);
        }

        RejectAdjacentMembers();
        ProofProtocol.Record(id: "rules.generated-layout-boundary",
                diagnostic: "GENERATED_LAYOUT_BOUNDARY_REJECTED",
                location: ("actual Roslyn original/mutated/wrong-path diagnostics p"
                        + "lus adjacent SCR0007/SCR0008 builds"));
    }

    /// <summary>Invalid UTF-8 is rejected before it can become a replacement-character source subject.</summary>
    [Fact, Trait("Case", "rules.line.invalid-utf8")]
    public void InvalidUtf8CannotBecomeACompliantSubject()
    {
        foreach (byte[] bytes in new byte[][]
        {
            [0xc0, 0xaf],
            [0xed, 0xa0, 0x80],
            [0xf0, 0x9f, 0x98]
        }

        )
        {
            string path = Local(path: ".phase1/temp/invalid-source-" + Guid.NewGuid().ToString(format: "N") + ".cs");
            File.WriteAllBytes(path: path, bytes: bytes);
            Should.Throw<DecoderFallbackException>(actual: () => InheritedAnalyzerVerifier.ReadUtf8(path: path));
        }

        ProofProtocol.Record(id: "rules.line.invalid-utf8", diagnostic: "SOURCE_ENCODING_INVALID",
                location: ("strict subject decoding: overlong UTF-8, encoded surrog" + "ate and truncated scalar"));
    }

    private static async Task RejectLinesAsync(string source, string path, DiagnosticAnalyzer analyzer,
            MetadataReference[] references)
    {
        AnalyzerVerifier.Expected[] expected = [..SourceText.From(text: source).Lines.Where(predicate:
                line => line.Span.Length > 120).Select(selector: line => new AnalyzerVerifier.Expected(
                        id: "SCR0007", severity: DiagnosticSeverity.Error, file: path, line: line.LineNumber
                                + 1, column: 121, endLine: line.LineNumber + 1, endColumn: line.Span.Length + 1))];
        expected.Length.ShouldBeGreaterThan(expected: 0);
        await AnalyzerVerifier.VerifyAsync(source: source, expected: expected, sourcePath: path,
                analyzer: analyzer, additionalReferences: references).ConfigureAwait(continueOnCapturedContext: true);
    }

    private static void RejectAdjacentMembers()
    {
        string source = "namespace KingdomZombies.Proofs; internal class AdjacentLayout { "
                + "private int F; public void M() { F = 1; _ = F; } }\n//" + new string(c: 'a', count: 119);
        string result = ConfigurationTests.BuildGameWithSource(source: source);
        result.ShouldContain(expected: "error SCR0007:");
        result.ShouldContain(expected: "error SCR0008:");
        result.ShouldContain(expected: "Main.cs(");
        (int Exit, string Output) clean = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2],
                "--no-restore", "-p:NuGetAudit=false");
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
    }

    private static MetadataReference[] PackageReferences()
    {
        (int Exit, string Output) result = ConfigurationTests.Dotnet("msbuild", ConfigurationTests.Projects[4],
                "-nologo", "-target:ResolveReferences", "-getItem:ReferencePath");
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        using var json = JsonDocument.Parse(json: result.Output);
        return json.RootElement.GetProperty(propertyName: "Items").GetProperty(propertyName: "ReferencePath")
                .EnumerateArray().Select(selector: row => Text(row: row, name: "FullPath")).Where(predicate:
                        path => !path.Replace(oldChar: '\\', newChar: '/').Contains(value: "/ref/net10.0/",
                                comparisonType: StringComparison.Ordinal)).Select(selector:
                                        path => MetadataReference.CreateFromFile(path: path)).ToArray();
    }

    private static JsonDocument ReadRegistry()
    {
        return JsonDocument.Parse(json: File.ReadAllText(path: Local(
                path: "docs/code/generated-layout-adaptations.json")));
    }

    private static string Text(JsonElement row, string name)
    {
        return row.GetProperty(propertyName: name).GetString() ?? throw new InvalidOperationException(
                message: "Missing registry string: " + name);
    }

    private static string Local(string path)
    {
        return Path.Combine(path1: ProofProtocol.Root, path2: path);
    }

    private static string Hash(string path)
    {
        return Convert.ToHexStringLower(inArray: SHA256.HashData(source: File.ReadAllBytes(path: Local(path: path))));
    }
}
