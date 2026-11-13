using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Writes per-case evidence carrying the current invocation identities and runs bounded Node
/// controls.</summary>
public static class ProofProtocol
{
    /// <summary>Gets the explicit proof checkout root, falling back to the current working directory.</summary>
    public static string Root => Environment.GetEnvironmentVariable(variable: "PHASE1_ROOT") ?? Directory
            .GetCurrentDirectory();

    /// <summary>Writes the case log and nonce-bound proof result when a result directory is configured.</summary>
    public static void Record(string id, string diagnostic = "ASSERTION_PASSED", string location = "C# assertion")
    {
        string? directory = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY");
        if (directory is null)
        {
            return;
        }

        string log = Path.Combine(path1: directory, path2: id + ".subject.log");
        File.WriteAllText(path: log, contents: diagnostic + ": " + location + Environment.NewLine);
        var result = new
        {
            id,
            nonce = Environment.GetEnvironmentVariable(variable: "PHASE1_NONCE"),
            input = Environment.GetEnvironmentVariable(variable: "PHASE1_INPUT"),
            tool = Environment.GetEnvironmentVariable(variable: "PHASE1_TOOL"),
            platform = Environment.GetEnvironmentVariable(variable: "PHASE1_PLATFORM"),
            status = "pass",
            diagnostic,
            location,
            prerequisites = new[]
            {
                "build",
                "discovery",
                "execution"
            },
            evidence = new[]
            {
                Path.GetFileName(path: log)
            },
        };
        File.WriteAllText(path: Path.Combine(path1: directory, path2: id + ".proof.json"),
                contents: JsonSerializer.Serialize(value: result));
    }

    /// <summary>Runs a Node script with captured output and a five-second deadline, killing its process tree
    /// on timeout.</summary>
    public static (int Exit, string Output) Node(string script, string? workingDirectory = null)
    {
        var start = new ProcessStartInfo(fileName: Environment.GetEnvironmentVariable(variable: "PHASE1_NODE")
                ?? "node")
        {
            WorkingDirectory = workingDirectory ?? Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        start.ArgumentList.Add(item: "-e");
        start.ArgumentList.Add(item: script);
        using Process process = Process.Start(startInfo: start) ?? throw new InvalidOperationException(
                message: "Node failed to start");
        Task<string> output = process.StandardOutput.ReadToEndAsync();
        Task<string> error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(milliseconds: 5000))
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
            throw new TimeoutException(message: "Controlled Node child exceeded 5 seconds");
        }

        return (process.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }
}
