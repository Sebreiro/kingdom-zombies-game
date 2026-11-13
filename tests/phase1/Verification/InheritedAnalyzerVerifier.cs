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
    internal static async Task VerifyAsync(string caseId, string diagnostic, string analyzerType, string source, string? editorConfig = null, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
    {
        var clean = new StringBuilder();
        var marks = new List<(string Id, int Start, int End)>();
        for (int i = 0; i < source.Length;)
        {
            if (!source.AsSpan(i).StartsWith("{|", StringComparison.Ordinal)) { clean.Append(source[i++]); continue; }
            int colon = source.IndexOf(':', i);
            int close = source.IndexOf("|}", colon, StringComparison.Ordinal);
            if (colon < 0 || close < 0) throw new InvalidOperationException("Invalid expected diagnostic markup");
            string id = source[(i + 2)..colon];
            int start = clean.Length;
            clean.Append(source.AsSpan(colon + 1, close - colon - 1));
            marks.Add((id, start, clean.Length));
            i = close + 2;
        }
        string text = clean.ToString();
        var lines = SourceText.From(text).Lines;
        var expected = new List<AnalyzerVerifier.Expected>();
        foreach (var mark in marks)
        {
            var start = lines.GetLinePosition(mark.Start);
            var end = lines.GetLinePosition(mark.End);
            expected.Add(new AnalyzerVerifier.Expected(mark.Id, DiagnosticSeverity.Error, "Subject.cs", start.Line + 1, start.Character + 1, end.Line + 1, end.Character + 1));
        }
        var assembly = Assembly.LoadFrom(Path.Combine(ProofProtocol.Root, "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll"));
        var type = assembly.GetType("Sebreiro.CodeRules.Analyzers." + analyzerType);
        var analyzer = type is null ? null : (DiagnosticAnalyzer?)Activator.CreateInstance(type);
        await AnalyzerVerifier.VerifyAsync(source: text, expected: [.. expected], analyzer: analyzer, outputKind: outputKind, generated: editorConfig is not null).ConfigureAwait(false);
        ProofProtocol.Record(id: caseId, diagnostic: diagnostic, location: "Subject.cs: " + string.Join("; ", expected));
    }
}
