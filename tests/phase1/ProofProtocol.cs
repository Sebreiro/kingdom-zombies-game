using System.Diagnostics;
using System.Text.Json;

namespace KingdomZombies.Phase1.Tests;

public static class ProofProtocol
{
    public static string Root => Environment.GetEnvironmentVariable("PHASE1_ROOT") ?? Directory.GetCurrentDirectory();

    public static void Record(string id, string diagnostic = "ASSERTION_PASSED", string location = "C# assertion")
    {
        var directory = Environment.GetEnvironmentVariable("PHASE1_RESULT_DIRECTORY");
        if (directory is null) return;
        var log = Path.Combine(directory, id + ".subject.log");
        File.WriteAllText(log, diagnostic + ": " + location + Environment.NewLine);
        var result = new
        {
            id, nonce = Environment.GetEnvironmentVariable("PHASE1_NONCE"),
            input = Environment.GetEnvironmentVariable("PHASE1_INPUT"),
            tool = Environment.GetEnvironmentVariable("PHASE1_TOOL"),
            platform = Environment.GetEnvironmentVariable("PHASE1_PLATFORM"),
            status = "pass", diagnostic, location,
            prerequisites = new[] { "build", "discovery", "execution" },
            evidence = new[] { Path.GetFileName(log) },
        };
        File.WriteAllText(Path.Combine(directory, id + ".proof.json"), JsonSerializer.Serialize(result));
    }

    public static (int Exit, string Output) Node(string script, string? workingDirectory = null)
    {
        var start = new ProcessStartInfo(Environment.GetEnvironmentVariable("PHASE1_NODE") ?? "node")
        {
            WorkingDirectory = workingDirectory ?? Root,
            RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false,
            CreateNoWindow = true,
        };
        start.ArgumentList.Add("-e");
        start.ArgumentList.Add(script);
        using var process = Process.Start(start) ?? throw new InvalidOperationException("Node failed to start");
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(5000))
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
            throw new TimeoutException("Controlled Node child exceeded 5 seconds");
        }
        return (process.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }
}
