using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Owns one bounded native process and preserves its unchanged nonce-tagged output.</summary>
internal sealed class EngineRun
{
    /// <summary>Gets the actual native process exit code.</summary>
    internal int Exit { get; private init; }
    /// <summary>Gets both unchanged native output streams.</summary>
    internal string Output { get; private init; } = "";
    /// <summary>Gets the actual native output artifact path.</summary>
    internal string Log { get; private init; } = "";
    /// <summary>Gets the distinct native-process nonce required by every surface record.</summary>
    internal string Nonce { get; private init; } = "";

    /// <summary>Starts the exact local pinned engine without loading its managed API in this test host.</summary>
    internal static EngineRun Start(string mode = "valid")
    {
        (int Exit, string Output) build = ConfigurationTests.Dotnet("build", ConfigurationTests.Projects[2],
                "--no-restore", "-p:NuGetAudit=false");
        if (build.Exit != 0)
        {
            throw new InvalidOperationException(message: "ENGINE_BUILD: " + build.Output);
        }

        string nonce = Guid.NewGuid().ToString(format: "D");
        string tool = OperatingSystem.IsWindows()
                ? "Godot_v4.7.2-stable_mono_win64/Godot_v4.7.2-stable_mono_win64_console.exe"
                : "Godot_v4.7.2-stable_mono_linux_x86_64/Godot_v4.7.2-stable_mono_linux.x86_64";
        var start = new ProcessStartInfo(fileName: Path.Combine(path1: ProofProtocol.Root,
                path2: ".phase1/tools/" + tool))
        {
            WorkingDirectory = ProofProtocol.Root,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        foreach (string argument in new[] { "--headless", "--path", "src/godot-game", "--rendering-method",
                "gl_compatibility", "--scene", "res://proofs/Phase1.tscn" })
        {
            start.ArgumentList.Add(item: argument);
        }

        start.Environment[key: "PHASE1_NONCE"] = nonce;
        start.Environment[key: "PHASE1_ENGINE_MODE"] = mode;
        using Process process = Process.Start(startInfo: start)
                ?? throw new InvalidOperationException(message: "ENGINE_START");
        Task<string> output = process.StandardOutput.ReadToEndAsync();
        Task<string> error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(milliseconds: 45000))
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
            throw new TimeoutException(message: "ENGINE_TIMEOUT: 45 seconds");
        }

        string text = output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult();
        string directory = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY")
                ?? Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/temp");
        string log = Path.Combine(path1: directory, path2: "engine-" + mode + "-" + nonce + ".log");
        File.WriteAllText(path: log, contents: text);
        return new() { Exit = process.ExitCode, Output = text, Nonce = nonce, Log = log };
    }

    /// <summary>Requires exactly one fresh engine assertion for the requested actual surface and cycle.</summary>
    internal bool Has(string surface, int cycle)
    {
        string[] lines = [..Output.Split(separator: '\n').Where(predicate: line => line.StartsWith(
                value: "PHASE1_SURFACE ", comparisonType: StringComparison.Ordinal))];
        return lines.Count(predicate: line => Matches(line: line, surface: surface, cycle: cycle)) == 1;
    }

    private bool Matches(string line, string surface, int cycle)
    {
        using var json = JsonDocument.Parse(json: line[15..]);
        JsonElement row = json.RootElement;
        return StringComparer.Ordinal.Equals(x: row.GetProperty(propertyName: "nonce").GetString(), y: Nonce)
                && StringComparer.Ordinal.Equals(x: row.GetProperty(propertyName: "surface").GetString(), y: surface)
                && row.GetProperty(propertyName: "cycle").GetInt32() == cycle;
    }
}
