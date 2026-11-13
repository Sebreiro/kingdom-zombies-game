using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Shouldly;
using Xunit;

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
}
