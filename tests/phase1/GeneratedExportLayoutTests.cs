using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Authenticates actual Windows SDK export sources and their exact line permissions.</summary>
public sealed class GeneratedExportLayoutTests
{
    /// <summary>Requires observed export source and SDK producer bytes, then runs the real line analyzer.</summary>
    [Fact, Trait("Case", "rules.export-layout-origin")]
    public async Task Check_ExportLayouts_HavePinnedOrigin()
    {
        OperatingSystem.IsWindows().ShouldBeTrue(customMessage: "Windows SDK source proof required");
        using JsonDocument registry = Registry();
        registry.RootElement.GetProperty(propertyName: "sources").GetArrayLength().ShouldBe(expected: 4);
        registry.RootElement.GetProperty(propertyName: "members").GetArrayLength().ShouldBe(expected: 7);
        JsonElement rows = registry.RootElement.GetProperty(propertyName: "exportSources");
        rows.GetArrayLength().ShouldBe(expected: 6);
        DiagnosticAnalyzer analyzer = Analyzer();
        var failures = new List<string>();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            Authenticate(row: row);
            string source = InheritedAnalyzerVerifier.ReadUtf8(path: Local(path: Text(row: row, name: "surface")));
            try
            {
                await AnalyzerVerifier.VerifyAsync(source: source, expected: [], sourcePath: Local(path: Text(
                        row: row, name: "path")), analyzer: analyzer).ConfigureAwait(continueOnCapturedContext: true);
            }
            catch (InvalidOperationException error)
            {
                failures.Add(item: error.Message);
            }
        }

        failures.ShouldBeEmpty(customMessage: string.Join(separator: "\n", values: failures));
        ProofProtocol.Record(id: "rules.export-layout-origin", diagnostic: "SDK_EXPORT_LAYOUT_ORIGIN_EXACT",
                location: "six observed Windows SDK export sources; exact producer, path, bytes and line");
    }

    private static void Authenticate(JsonElement row)
    {
        Text(row: row, name: "platform").ShouldBe(expected: "win32-x64");
        Text(row: row, name: "sdk").ShouldBe(expected: "10.0.404");
        Text(row: row, name: "originSymbol").ShouldBe(expected: "GenerateTargetFrameworkMonikerAttribute");
        string origin = Path.GetFullPath(path: Path.Combine(path1: RuntimeEnvironment.GetRuntimeDirectory(),
                path2: "../../..", path3: Text(row: row, name: "origin")));
        Hash(bytes: File.ReadAllBytes(path: origin)).ShouldBe(expected: Text(row: row, name: "originSha256"));
        byte[] bytes = File.ReadAllBytes(path: Local(path: Text(row: row, name: "surface")));
        bytes.Length.ShouldBe(expected: row.GetProperty(propertyName: "bytes").GetInt32());
        Hash(bytes: bytes).ShouldBe(expected: Text(row: row, name: "sha256"));
        string source = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: bytes);
        Hash(bytes: Encoding.UTF8.GetBytes(s: source)).ShouldBe(expected: Text(row: row, name: "textSha256"));
        TextLine line = SourceText.From(text: source).Lines[row.GetProperty(propertyName: "line").GetInt32() - 1];
        line.LineNumber.ShouldBe(expected: 3);
        line.Span.Length.ShouldBe(expected: row.GetProperty(propertyName: "lineLength").GetInt32());
        string path = Text(row: row, name: "path");
        path.ShouldStartWith(expected: Text(row: row, name: "owner") + "/");
        path.ShouldContain(expected: "/obj/" + Text(row: row, name: "configuration") + "/");
    }

    private static DiagnosticAnalyzer Analyzer()
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Local(
                path: "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        Type type = assembly.GetType(name: "Sebreiro.CodeRules.Analyzers.LineLengthAnalyzer")
                ?? throw new InvalidOperationException(message: "Missing real line analyzer");
        return Activator.CreateInstance(type: type) as DiagnosticAnalyzer
                ?? throw new InvalidOperationException(message: "Missing analyzer instance");
    }

    private static JsonDocument Registry()
    {
        return JsonDocument.Parse(json: File.ReadAllText(path: Local(
                path: "docs/code/generated-layout-adaptations.json")));
    }

    private static string Text(JsonElement row, string name)
    {
        return row.GetProperty(propertyName: name).GetString()
                ?? throw new InvalidOperationException(message: "Missing export registry string: " + name);
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
