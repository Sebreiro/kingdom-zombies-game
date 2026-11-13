using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Verification;

/// <summary>
/// Adapts the captured LangProj verifier's exact-diagnostic contract to the already pinned Roslyn API.
/// Uses actual evaluated net10.0 reference assemblies, with no additional testing-framework package.
/// Any unexpected compiler warning/error, analyzer diagnostic, severity or source location fails.
/// </summary>
internal static class AnalyzerVerifier
{
    /// <summary>One complete expected source diagnostic; coordinates are one-based.</summary>
    internal sealed record Expected(string Id, DiagnosticSeverity Severity, string File, int Line, int Column, int EndLine, int EndColumn);

    internal static ImmutableArray<MetadataReference> References()
    {
        var result = ConfigurationTests.Dotnet("msbuild", ConfigurationTests.Projects[0], "-nologo", "-target:ResolveReferences", "-getItem:ReferencePath");
        if (result.Exit != 0) throw new InvalidOperationException(message: "Reference resolution failed: " + result.Output);
        using var json = System.Text.Json.JsonDocument.Parse(json: result.Output);
        var paths = json.RootElement.GetProperty(propertyName: "Items").GetProperty(propertyName: "ReferencePath").EnumerateArray().Select(selector: r => r.GetProperty(propertyName: "FullPath").GetString()!).ToArray();
        if (paths.Length < 100 || !paths.All(predicate: p => p.Replace(oldChar: '\\', newChar: '/').Contains(value: "/ref/net10.0/", comparisonType: StringComparison.Ordinal))) throw new InvalidOperationException(message: "Expected evaluated .NET 10 reference pack");
        return paths.Select(selector: p => (MetadataReference)MetadataReference.CreateFromFile(path: p)).ToImmutableArray();
    }

    internal static async Task VerifyAsync(string source, Expected[] expected, IEnumerable<MetadataReference>? additionalReferences = null, DiagnosticAnalyzer? analyzer = null, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary, bool generated = false)
    {
        var references = References().AddRange(items: additionalReferences ?? []);
        var tree = CSharpSyntaxTree.ParseText(text: source, options: new CSharpParseOptions(languageVersion: LanguageVersion.CSharp14), path: "Subject.cs", cancellationToken: TestContext.Current.CancellationToken);
        var compilation = CSharpCompilation.Create(assemblyName: "CompilerProof", syntaxTrees: [tree], references: references, options: new CSharpCompilationOptions(outputKind: outputKind, nullableContextOptions: NullableContextOptions.Enable, allowUnsafe: true).WithSyntaxTreeOptionsProvider(provider: new SubjectTreeOptions(generated: generated)));
        var diagnostics = compilation.GetDiagnostics(cancellationToken: TestContext.Current.CancellationToken);
        if (analyzer is not null)
            diagnostics = diagnostics.AddRange(items: await compilation.WithAnalyzers(analyzers: [analyzer]).GetAnalyzerDiagnosticsAsync(cancellationToken: TestContext.Current.CancellationToken).ConfigureAwait(continueOnCapturedContext: false));
        Match(diagnostics: diagnostics, expected: expected);
        if (!expected.Any(predicate: d => d.Severity == DiagnosticSeverity.Error))
        {
            using var output = new MemoryStream();
            var emitted = compilation.Emit(peStream: output, cancellationToken: TestContext.Current.CancellationToken);
            if (!emitted.Success) throw new InvalidOperationException(message: "Emit failed: " + string.Join(separator: "; ", values: emitted.Diagnostics));
        }
    }

    private sealed class SubjectTreeOptions(bool generated) : SyntaxTreeOptionsProvider
    {
        public override GeneratedKind IsGenerated(SyntaxTree tree, System.Threading.CancellationToken cancellationToken) { return generated ? GeneratedKind.MarkedGenerated : GeneratedKind.NotGenerated; }
        public override bool TryGetDiagnosticValue(SyntaxTree tree, string diagnosticId, System.Threading.CancellationToken cancellationToken, out ReportDiagnostic severity) { severity = ReportDiagnostic.Default; return false; }
        public override bool TryGetGlobalDiagnosticValue(string diagnosticId, System.Threading.CancellationToken cancellationToken, out ReportDiagnostic severity) { severity = ReportDiagnostic.Default; return false; }
    }

    internal static void Match(IEnumerable<Diagnostic> diagnostics, Expected[] expected)
    {
        var actual = diagnostics.Where(predicate: d => d.Severity is DiagnosticSeverity.Warning or DiagnosticSeverity.Error).Select(selector: d =>
        {
            var span = d.Location.GetLineSpan();
            return new Expected(Id: d.Id, Severity: d.Severity, File: span.Path ?? "", Line: span.StartLinePosition.Line + 1, Column: span.StartLinePosition.Character + 1, EndLine: span.EndLinePosition.Line + 1, EndColumn: span.EndLinePosition.Character + 1);
        }).OrderBy(keySelector: d => d.ToString(), comparer: StringComparer.Ordinal).ToArray();
        var wanted = expected.OrderBy(keySelector: d => d.ToString(), comparer: StringComparer.Ordinal).ToArray();
        if (!actual.SequenceEqual(other: wanted)) throw new InvalidOperationException(message: "DIAGNOSTIC_MISMATCH expected=" + string.Join(separator: "; ", values: wanted.Select(selector: d => d.ToString())) + " actual=" + string.Join(separator: "; ", values: actual.Select(selector: d => d.ToString())));
    }
}
