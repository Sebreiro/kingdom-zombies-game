using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Shouldly;
using Xunit;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using KingdomZombies.Phase1.Tests.Verification;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves actual evaluated compiler policy and permitted consumer composition.</summary>
public sealed class ConfigurationTests
{
    private static readonly string[] ProductNames = ["KingdomZombies.Core.csproj", "KingdomZombies.Multiplayer.Abstractions.csproj"];
    internal static readonly string[] Projects = ["src/core/KingdomZombies.Core.csproj", "src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj", "src/godot-game/KingdomZombies.csproj", "tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj", "tests/phase1/KingdomZombies.Phase1.Tests.csproj"];

    internal static (int Exit, string Output) Dotnet(params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet") { WorkingDirectory = ProofProtocol.Root, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        using var child = Process.Start(start) ?? throw new InvalidOperationException("dotnet did not start");
        var output = child.StandardOutput.ReadToEndAsync();
        var error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(30000)) { child.Kill(entireProcessTree: true); child.WaitForExit(); throw new TimeoutException("Compiler proof exceeded 30 seconds"); }
        return (child.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }

    /// <summary>Requires the actual game to compose exactly Core and service ports with an analyzer-only tooling edge.</summary>
    [Fact, Trait("Case", "pins.composition")]
    public void GameHasPermittedEvaluatedComposition()
    {
        var result = Dotnet("msbuild", Projects[2], "-nologo", "-getItem:ProjectReference");
        result.Exit.ShouldBe(0, result.Output);
        using var json = JsonDocument.Parse(result.Output);
        var references = json.RootElement.GetProperty("Items").GetProperty("ProjectReference").EnumerateArray().ToArray();
        var runtime = references.Where(r => !r.TryGetProperty("ReferenceOutputAssembly", out var value) || !string.Equals(value.GetString(), "false", StringComparison.OrdinalIgnoreCase)).ToArray();
        runtime.Length.ShouldBe(2, "Godot must compile with both permitted product references");
        runtime.Select(r => Path.GetFileName(r.GetProperty("FullPath").GetString())).Order(StringComparer.Ordinal).ShouldBe(ProductNames);
        var analyzer = references.Single(r => r.GetProperty("FullPath").GetString()!.EndsWith("Sebreiro.CodeRules.Analyzers.csproj", StringComparison.Ordinal));
        analyzer.GetProperty("OutputItemType").GetString().ShouldBe("Analyzer");
        analyzer.GetProperty("ReferenceOutputAssembly").GetString().ShouldBe("false");
        ProofProtocol.Record("pins.composition", "COMPOSITION_EXACT", Projects[2]);
    }

    /// <summary>Emits an actual Godot consumer against the built game, Core and service-port assemblies.</summary>
    [Fact, Trait("Case", "pins.consumer")]
    public async Task RealGodotConsumerCompiles()
    {
        var build = Dotnet("build", Projects[2], "--no-restore", "-p:NuGetAudit=false");
        build.Exit.ShouldBe(0, build.Output);
        var paths = new[] { "src/core/bin/Debug/net10.0/KingdomZombies.Core.dll", "src/multiplayer-abstractions/bin/Debug/net10.0/KingdomZombies.Multiplayer.Abstractions.dll", "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll", ".phase1/packages/godotsharp/4.7.2/lib/net8.0/GodotSharp.dll" };
        // Godot's actual package targets net8.0; Roslyn reports the precise framework-unification
        // warning that SDK builds already handle. Assert it explicitly instead of hiding diagnostics.
        var frameworkUnification = new AnalyzerVerifier.Expected("CS1701", DiagnosticSeverity.Warning, "", 1, 1, 1, 1);
        await AnalyzerVerifier.VerifyAsync("public sealed class Consumer { public Godot.Node? Engine { get; init; } }", [frameworkUnification], paths.Select(p => MetadataReference.CreateFromFile(Path.Combine(ProofProtocol.Root, p))));
        ProofProtocol.Record("pins.consumer", "CONSUMER_EMITTED", "Subject.cs with actual Godot/game/Core/ports references");
    }

    /// <summary>Accepts the exact compiler diagnostic, then rejects wrong identity, severity, location and additional errors.</summary>
    [Fact, Trait("Case", "pins.verifier")]
    public async Task VerifierRequiresExactCompilerDiagnostic()
    {
        var exact = new AnalyzerVerifier.Expected("CS0246", DiagnosticSeverity.Error, "Subject.cs", 1, 24, 1, 31);
        const string source = "public class Subject : Missing { }";
        await AnalyzerVerifier.VerifyAsync(source, [exact]);
        foreach (var changed in new[] { exact with { Id = "RS0030" }, exact with { Severity = DiagnosticSeverity.Warning }, exact with { Column = 25 }, exact with { File = "Other.cs" }, exact with { EndColumn = 32 } })
            (await Should.ThrowAsync<InvalidOperationException>(() => AnalyzerVerifier.VerifyAsync(source, [changed]))).Message.ShouldContain("DIAGNOSTIC_MISMATCH");
        (await Should.ThrowAsync<InvalidOperationException>(() => AnalyzerVerifier.VerifyAsync(source + " public class Other : Absent { }", [exact]))).Message.ShouldContain("CS0246");
        await Should.ThrowAsync<InvalidOperationException>(() => AnalyzerVerifier.VerifyAsync("public class Clean { }", [exact]));
        ProofProtocol.Record("pins.verifier", "DIAGNOSTIC_EXACT", "Subject.cs(1,24)-(1,31): CS0246");
    }
}
