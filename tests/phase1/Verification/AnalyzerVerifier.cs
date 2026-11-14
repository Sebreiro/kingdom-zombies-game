using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Verification;
/// <summary>
/// Adapts the captured LangProj verifier's exact-diagnostic contract to the already pinned Roslyn API.
/// Uses actual evaluated net10.0 reference assemblies, with no additional testing-framework package.
/// Any unexpected compiler warning/error, analyzer diagnostic, severity or source location fails.
/// </summary>
internal static class AnalyzerVerifier
{
    internal static ImmutableArray<MetadataReference> References()
    {
        (int Exit, string Output) result = ConfigurationTests.Dotnet("msbuild", ConfigurationTests.Projects[0],
                "-nologo", "-target:ResolveReferences", "-getItem:ReferencePath");
        if (result.Exit != 0)
        {
            throw new InvalidOperationException(message: "Reference resolution failed: " + result.Output);
        }

        using var json = System.Text.Json.JsonDocument.Parse(json: result.Output);
        string[] paths = [..json.RootElement.GetProperty(propertyName: "Items").GetProperty(
                propertyName: "ReferencePath").EnumerateArray().Select(selector: r => (r.GetProperty(
                        propertyName: "FullPath").GetString() ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null")))];
        if (paths.Length < 100 || !paths.All(predicate: p => p.Replace(oldChar: '\\', newChar: '/').Contains(
                value: "/ref/net10.0/", comparisonType: StringComparison.Ordinal)))
        {
            throw new InvalidOperationException(message: "Expected evaluated .NET 10 reference pack");
        }

        return [.. paths.Select(selector: p => (MetadataReference)MetadataReference.CreateFromFile(path: p))];
    }

    internal static async Task VerifyAsync(string source, Expected[] expected,
            IEnumerable<MetadataReference>? additionalReferences = null, DiagnosticAnalyzer? analyzer = null,
            OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary, bool generated = false,
            string? message = null, string sourcePath = "Subject.cs")
    {
        ImmutableArray<MetadataReference> references = References().AddRange(items: additionalReferences ?? []);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(text: source, options: new CSharpParseOptions(
                languageVersion: LanguageVersion.CSharp14), path: sourcePath, cancellationToken: TestContext
                        .Current.CancellationToken);
        var compilation = CSharpCompilation.Create(assemblyName: "CompilerProof", syntaxTrees: [tree],
                references: references, options: new CSharpCompilationOptions(outputKind: outputKind,
                        nullableContextOptions: NullableContextOptions.Enable, allowUnsafe: true)
                        .WithSyntaxTreeOptionsProvider(provider: new SubjectTreeOptions(generated: generated)));
        ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(cancellationToken: TestContext
                .Current.CancellationToken);
        if (analyzer is not null)
        {
            diagnostics = diagnostics.AddRange(items: await compilation.WithAnalyzers(analyzers: [analyzer])
                    .GetAnalyzerDiagnosticsAsync(cancellationToken: TestContext.Current.CancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false));
        }

        Match(diagnostics: diagnostics, expected: expected);
        if (message is not null && !diagnostics.Any(predicate: d => StringComparer.Ordinal.Equals(x: d
                .GetMessage(formatProvider: System.Globalization.CultureInfo.InvariantCulture), y: message)))
        {
            throw new InvalidOperationException(message: "Expected diagnostic message missing: " + message);
        }

        if (!expected.Any(predicate: d => d.Severity == DiagnosticSeverity.Error))
        {
            using var output = new MemoryStream();
            EmitResult emitted = compilation.Emit(peStream: output, cancellationToken: TestContext.Current
                    .CancellationToken);
            if (!emitted.Success)
            {
                throw new InvalidOperationException(message: "Emit failed: " + string.Join(separator: "; ",
                        values: emitted.Diagnostics));
            }
        }
    }

    internal static void Match(IEnumerable<Diagnostic> diagnostics, Expected[] expected)
    {
        Expected[] actual = [..diagnostics.Where(predicate: d => d.Severity is DiagnosticSeverity.Warning
                or DiagnosticSeverity.Error).Select(selector: d =>
                {
            FileLinePositionSpan span = d.Location.GetLineSpan();
            return new Expected(id: d.Id, severity: d.Severity, file: span.Path ?? "", line: span
                    .StartLinePosition.Line + 1, column: span.StartLinePosition.Character + 1, endLine: span
                            .EndLinePosition.Line + 1, endColumn: span.EndLinePosition.Character + 1);
                }).OrderBy(keySelector: d => d.ToString(), comparer: StringComparer.Ordinal)];
        Expected[] wanted = [.. expected.OrderBy(keySelector: d => d.ToString(), comparer: StringComparer.Ordinal)];
        if (!actual.SequenceEqual(other: wanted))
        {
            throw new InvalidOperationException(message: "DIAGNOSTIC_MISMATCH expected=" + string.Join(
                    separator: "; ", values: wanted.Select(selector: d => d.ToString())) + " actual=" + string
                            .Join(separator: "; ", values: actual.Select(selector: d => d.ToString())));
        }
    }

    /// <summary>One complete expected source diagnostic; coordinates are one-based.</summary>
    internal sealed record Expected
    {
        public string Id { get; init; }
        public DiagnosticSeverity Severity { get; init; }
        public string File { get; init; }
        public int Line { get; init; }
        public int Column { get; init; }
        public int EndLine { get; init; }
        public int EndColumn { get; init; }

        internal Expected(string id, DiagnosticSeverity severity, string file, int line, int column,
                int endLine, int endColumn)
        {
            Id = id;
            Severity = severity;
            File = file;
            Line = line;
            Column = column;
            EndLine = endLine;
            EndColumn = endColumn;
        }
    }

    private sealed class SubjectTreeOptions : SyntaxTreeOptionsProvider
    {
        public override GeneratedKind IsGenerated(SyntaxTree tree, System.Threading.CancellationToken cancellationToken)
        {
            return _generated ? GeneratedKind.MarkedGenerated : GeneratedKind.NotGenerated;
        }

        public override bool TryGetDiagnosticValue(SyntaxTree tree, string diagnosticId,
                System.Threading.CancellationToken cancellationToken, out ReportDiagnostic severity)
        {
            severity = ReportDiagnostic.Default;
            return false;
        }

        public override bool TryGetGlobalDiagnosticValue(string diagnosticId,
                System.Threading.CancellationToken cancellationToken, out ReportDiagnostic severity)
        {
            severity = ReportDiagnostic.Default;
            return false;
        }

        internal SubjectTreeOptions(bool generated)
        {
            _generated = generated;
        }

        private readonly bool _generated;
    }
}
