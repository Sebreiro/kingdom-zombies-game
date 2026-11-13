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
        if (result.Exit != 0) throw new InvalidOperationException("Reference resolution failed: " + result.Output);
        using var json = System.Text.Json.JsonDocument.Parse(result.Output);
        var paths = json.RootElement.GetProperty("Items").GetProperty("ReferencePath").EnumerateArray().Select(r => r.GetProperty("FullPath").GetString()!).ToArray();
        if (paths.Length < 100 || !paths.All(p => p.Replace('\\', '/').Contains("/ref/net10.0/", StringComparison.Ordinal))) throw new InvalidOperationException("Expected evaluated .NET 10 reference pack");
        return paths.Select(p => (MetadataReference)MetadataReference.CreateFromFile(p)).ToImmutableArray();
    }

    internal static async Task VerifyAsync(string source, Expected[] expected, IEnumerable<MetadataReference>? additionalReferences = null, DiagnosticAnalyzer? analyzer = null, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary, bool generated = false)
    {
        var references = References().AddRange(additionalReferences ?? []);
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp14), path: "Subject.cs", cancellationToken: TestContext.Current.CancellationToken);
        var compilation = CSharpCompilation.Create("CompilerProof", [tree], references, new CSharpCompilationOptions(outputKind, nullableContextOptions: NullableContextOptions.Enable, allowUnsafe: true).WithSyntaxTreeOptionsProvider(new SubjectTreeOptions(generated)));
        var diagnostics = compilation.GetDiagnostics(TestContext.Current.CancellationToken);
        if (analyzer is not null)
            diagnostics = diagnostics.AddRange(await compilation.WithAnalyzers([analyzer]).GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken).ConfigureAwait(false));
        Match(diagnostics, expected);
        if (!expected.Any(d => d.Severity == DiagnosticSeverity.Error))
        {
            using var output = new MemoryStream();
            var emitted = compilation.Emit(output, cancellationToken: TestContext.Current.CancellationToken);
            if (!emitted.Success) throw new InvalidOperationException("Emit failed: " + string.Join("; ", emitted.Diagnostics));
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
        var actual = diagnostics.Where(d => d.Severity is DiagnosticSeverity.Warning or DiagnosticSeverity.Error).Select(d =>
        {
            var span = d.Location.GetLineSpan();
            return new Expected(d.Id, d.Severity, span.Path ?? "", span.StartLinePosition.Line + 1, span.StartLinePosition.Character + 1, span.EndLinePosition.Line + 1, span.EndLinePosition.Character + 1);
        }).OrderBy(d => d.ToString(), StringComparer.Ordinal).ToArray();
        var wanted = expected.OrderBy(d => d.ToString(), StringComparer.Ordinal).ToArray();
        if (!actual.SequenceEqual(wanted)) throw new InvalidOperationException("DIAGNOSTIC_MISMATCH expected=" + string.Join("; ", wanted.Select(d => d.ToString())) + " actual=" + string.Join("; ", actual.Select(d => d.ToString())));
    }
}
