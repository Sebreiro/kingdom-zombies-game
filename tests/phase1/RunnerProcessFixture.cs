using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Creates isolated runner inputs and explicit child controls without mutating approved repository inputs.</summary>
public sealed class RunnerProcessFixture : IDisposable
{
    /// <summary>Gets the checkout or isolated fixture directory used as the child working directory.</summary>
    public string Root { get; }
    /// <summary>Gets the deliberate child mutation selected for this fixture.</summary>
    public string Mode { get; }

    /// <summary>Binds the child fixture to the current proof root and requested mutation mode.</summary>
    public RunnerProcessFixture()
    {
        Root = ProofProtocol.Root;
        Mode = Environment.GetEnvironmentVariable("PHASE1_MUTATION") ?? "pass";
    }

    /// <summary>Copies an isolated runner and prebuilt test assembly for the requested mutation.</summary>
    public static RunnerProcessFixture Create(string mode) => new(mode);

    private RunnerProcessFixture(string mode)
    {
        Mode = mode;
        var parent = Environment.GetEnvironmentVariable("PHASE1_RESULT_DIRECTORY") ?? Path.Combine(ProofProtocol.Root, ".phase1", "temp");
        Root = Path.GetFullPath(Path.Combine(parent, "fixture-" + Guid.NewGuid()));
        Directory.CreateDirectory(Path.Combine(Root, "tools", "phase1"));
        foreach (var file in new[] { "check.cjs", "bootstrap.cjs", "toolchain.json" })
            File.Copy(Path.Combine(ProofProtocol.Root, "tools", "phase1", file), Path.Combine(Root, "tools", "phase1", file));
        File.Copy(Path.Combine(ProofProtocol.Root, "global.json"), Path.Combine(Root, "global.json"));
        Directory.CreateDirectory(Path.Combine(Root, "fixture"));
        foreach (var file in Directory.GetFiles(AppContext.BaseDirectory))
            File.Copy(file, Path.Combine(Root, "fixture", Path.GetFileName(file)));
        File.WriteAllText(Path.Combine(Root, "approved.cs"), "// Approved isolated fixture source\n");
        File.WriteAllText(Path.Combine(Root, "approved.md"), "Approved isolated fixture document.\n");
        var method = string.Equals(mode, "skip", StringComparison.Ordinal) ? "Skipped" : string.Equals(mode, "zero", StringComparison.Ordinal) ? "NoSuchTest" : "Child";
        if (string.Equals(mode, "filter", StringComparison.Ordinal)) method = "WrongSelection";
        var manifest = new
        {
            schemaVersion = 1, project = (string?)null,
            assembly = "fixture/KingdomZombies.Phase1.Tests.dll",
            executable = string.Equals(mode, "missing-executable", StringComparison.Ordinal) ? "fixture/missing.exe" : (string?)null,
            childTimeoutMs = 1800,
            inputs = new[] { "tools/phase1", "fixture", "global.json", "approved.cs", "approved.md" },
            cases = new[] { new { id = "fixture.pass", group = "runner", rule = "RULE-03", owner = "01-02", implementation = "csharp",
                prerequisites = new[] { "prebuilt-csharp" }, evidence = new[] { "report", "log" },
                test = "KingdomZombies.Phase1.Tests.RunnerProcessFixture." + method } },
        };
        File.WriteAllText(Path.Combine(Root, "tools", "phase1", "required-cases.json"), JsonSerializer.Serialize(manifest));
    }

    /// <summary>Starts the fixture coordinator with this fixture mutation and the supplied selection arguments.</summary>
    public Process Start(params string[] args)
    {
        return StartMode(Mode, args);
    }

    /// <summary>Starts a coordinator with an explicit mutation, redirected output and no visible console.</summary>
    public Process StartMode(string mode, params string[] args)
    {
        var start = new ProcessStartInfo(Environment.GetEnvironmentVariable("PHASE1_NODE") ?? "node")
        {
            WorkingDirectory = Root, RedirectStandardOutput = true, RedirectStandardError = true,
            UseShellExecute = false, CreateNoWindow = true,
        };
        start.ArgumentList.Add(Path.Combine(Root, "tools", "phase1", "check.cjs"));
        foreach (var arg in args.Length == 0 ? new[] { "--all" } : args) start.ArgumentList.Add(arg);
        start.Environment["PHASE1_MUTATION"] = mode;
        return Process.Start(start) ?? throw new InvalidOperationException("Fixture coordinator did not start");
    }

    /// <summary>Runs the fixture coordinator and returns its exit status and combined captured output.</summary>
    public (int Exit, string Output) Run(params string[] args)
    {
        using var child = Start(args);
        return Finish(child);
    }

    /// <summary>Collects coordinator output within five seconds, killing its entire process tree on timeout.</summary>
    public static (int Exit, string Output) Finish(Process child)
    {
        var output = child.StandardOutput.ReadToEndAsync();
        var error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(5000))
        {
            child.Kill(entireProcessTree: true); child.WaitForExit();
            throw new TimeoutException("Controlled coordinator exceeded 5 seconds");
        }
        return (child.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }

    /// <summary>Reads the newest summary published by this isolated fixture.</summary>
    public JsonNode Summary()
    {
        var reports = Directory.GetFiles(Path.Combine(Root, ".phase1", "runs"), "summary.json", SearchOption.AllDirectories);
        return JsonNode.Parse(File.ReadAllText(reports.OrderBy(File.GetLastWriteTimeUtc).Last()))!;
    }

    /// <summary>Computes an uppercase SHA-256 fingerprint of the specified file bytes.</summary>
    public static string Hash(string file) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(file)));

    /// <summary>Preserves isolated diagnostic artifacts for inspection; child cleanup is owned by bounded execution.</summary>
    public void Dispose()
    {
        // Preserve isolated logs/results for inspection; all outputs live under the ignored run directory.
    }

    /// <summary>Executes the selected explicit process-failure control and mutates evidence only in the isolated fixture.</summary>
    [Fact(Explicit = true), Trait("Case", "fixture.pass")]
    public void Child()
    {
        var mode = Environment.GetEnvironmentVariable("PHASE1_MUTATION") ?? "pass";
        ExerciseProcessFailure(mode);
        (2 + 2).ShouldBe(4);
        ProofProtocol.Record("fixture.pass");
        MutateEvidence(mode);
    }

    private static void ExerciseProcessFailure(string mode)
    {
        File.WriteAllText(Path.Combine(ProofProtocol.Root, "child.pid"), Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (string.Equals(mode, "concurrent", StringComparison.Ordinal)) Thread.Sleep(1000);
        if (string.Equals(mode, "assertion", StringComparison.Ordinal)) (2 + 2).ShouldBe(5, "INTENDED_ASSERTION_FAILURE at RunnerProcessFixture.Child");
        if (string.Equals(mode, "crash", StringComparison.Ordinal)) Environment.Exit(71);
        if (mode is "hold" or "timeout")
        {
            if (string.Equals(mode, "timeout", StringComparison.Ordinal))
            {
                var descendant = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true };
                descendant.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
                descendant.ArgumentList.Add("--filter-method");
                descendant.ArgumentList.Add("KingdomZombies.Phase1.Tests.RunnerProcessFixture.Descendant");
                descendant.ArgumentList.Add("--no-ansi");
                descendant.ArgumentList.Add("--progress"); descendant.ArgumentList.Add("off");
                descendant.ArgumentList.Add("--explicit"); descendant.ArgumentList.Add("on");
                using var process = Process.Start(descendant)!;
                File.WriteAllText(Path.Combine(ProofProtocol.Root, "descendant.pid"), process.Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
                var ready = Path.Combine(ProofProtocol.Root, "descendant.ready");
                var watch = Stopwatch.StartNew();
                while (!File.Exists(ready) && watch.ElapsedMilliseconds < 1000) Thread.Sleep(10);
                File.Exists(ready).ShouldBeTrue("descendant must actually execute before timeout");
            }
            Thread.Sleep(Timeout.Infinite);
        }
    }

    private static void MutateEvidence(string mode)
    {
        var directory = Environment.GetEnvironmentVariable("PHASE1_RESULT_DIRECTORY")!;
        var file = Path.Combine(directory, "fixture.pass.proof.json");
        var result = JsonNode.Parse(File.ReadAllText(file))!;
        if (string.Equals(mode, "deleted-result", StringComparison.Ordinal)) File.Delete(file);
        if (string.Equals(mode, "deleted-log", StringComparison.Ordinal)) File.Delete(Path.Combine(directory, "fixture.pass.subject.log"));
        if (string.Equals(mode, "duplicate", StringComparison.Ordinal)) File.Copy(file, Path.Combine(directory, "duplicate.proof.json"));
        if (string.Equals(mode, "nonce", StringComparison.Ordinal)) { result["nonce"] = "prior-run"; File.WriteAllText(file, result.ToJsonString()); }
        if (string.Equals(mode, "stale", StringComparison.Ordinal)) File.WriteAllText(file, File.ReadAllText(Path.Combine(ProofProtocol.Root, "prior.proof.json")));
        if (string.Equals(mode, "source", StringComparison.Ordinal)) File.AppendAllText(Path.Combine(ProofProtocol.Root, "approved.cs"), "// unauthorized change\n");
        if (string.Equals(mode, "document", StringComparison.Ordinal)) File.AppendAllText(Path.Combine(ProofProtocol.Root, "approved.md"), "Unauthorized change.\n");
    }

    /// <summary>Provides the intentional explicit skip that proves the parent rejects skipped required cases.</summary>
    [Fact(Explicit = true, Skip = "Intentional isolated skip control"), Trait("Case", "fixture.pass")]
    public void Skipped() { }

    /// <summary>Provides a passing explicit case with the wrong identity to prove discovery reconciliation.</summary>
    [Fact(Explicit = true), Trait("Case", "fixture.wrong")]
    public void WrongSelection() { (2 + 2).ShouldBe(4); }

    /// <summary>Signals actual descendant startup, then waits for the parent timeout cleanup control.</summary>
    [Fact(Explicit = true), Trait("Case", "fixture.descendant")]
    public void Descendant()
    {
        File.WriteAllText(Path.Combine(ProofProtocol.Root, "descendant.ready"), Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        Thread.Sleep(Timeout.Infinite);
    }
}
