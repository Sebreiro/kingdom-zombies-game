using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace KingdomZombies.Phase1.Tests.Verification;

/// <summary>Runs captured markup subjects against the built analyzer assembly with exact diagnostic spans.</summary>
internal static class InheritedAnalyzerVerifier
{
    internal static string ReadUtf8(string path)
    {
        return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: File.ReadAllBytes(path: path));
    }
    internal static async Task VerifyAsync(string caseId, string diagnostic, string analyzerType, string source
            , string? editorConfig = null, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary, string?
            message = null)
    {
        var clean = new StringBuilder();
        var marks = new List<(string Id, int Start, int End)>();
        for (int i = 0; i < source.Length;)
        {
            if (!source.AsSpan(start: i).StartsWith(value: "{|", comparisonType: StringComparison.Ordinal)) {
                    clean.Append(value: source[i++]); continue; }
            int colon = source.IndexOf(value: ':', startIndex: i);
            int close = source.IndexOf(value: "|}", startIndex: colon, comparisonType: StringComparison.Ordinal);
            if (colon < 0 || close < 0) throw new InvalidOperationException(message:
                    "Invalid expected diagnostic markup");
            string id = source[(i + 2)..colon];
            int start = clean.Length;
            clean.Append(value: source.AsSpan(start: colon + 1, length: close - colon - 1));
            marks.Add(item: (id, start, clean.Length));
            i = close + 2;
        }
        string text = clean.ToString();
        var lines = SourceText.From(text: text).Lines;
        var expected = new List<AnalyzerVerifier.Expected>();
        foreach (var mark in marks)
        {
            var start = lines.GetLinePosition(position: mark.Start);
            var end = lines.GetLinePosition(position: mark.End);
            expected.Add(item: new AnalyzerVerifier.Expected(id: mark.Id, severity: DiagnosticSeverity.Error,
                    file: "Subject.cs", line: start.Line + 1, column: start.Character + 1, endLine: end.Line + 1
                    , endColumn: end.Character + 1));
        }
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        var type = assembly.GetType(name: "Sebreiro.CodeRules.Analyzers." + analyzerType);
        var analyzer = type is null ? null : (DiagnosticAnalyzer?)Activator.CreateInstance(type: type);
        if (analyzer is null && expected.Count == 0) throw new InvalidOperationException(message:
                "Analyzer registration missing: " + analyzerType);
        await AnalyzerVerifier.VerifyAsync(source: text, expected: [.. expected], analyzer: analyzer, outputKind
                : outputKind, generated: editorConfig is not null, message: message).ConfigureAwait(
                        continueOnCapturedContext: false);
        ProofProtocol.Record(id: caseId, diagnostic: diagnostic, location: "Subject.cs: " + string.Join(
                separator: "; ", values: expected));
    }
}
