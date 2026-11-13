using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

public sealed class RunnerProcessFixture : IDisposable
{
    public string Root { get; }
    public string Mode { get; }

    public RunnerProcessFixture()
    {
        Root = ProofProtocol.Root;
        Mode = Environment.GetEnvironmentVariable("PHASE1_MUTATION") ?? "pass";
    }

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
        var method = mode == "skip" ? "Skipped" : mode == "zero" ? "NoSuchTest" : "Child";
        if (mode == "filter") method = "WrongSelection";
        var manifest = new
        {
            schemaVersion = 1, project = (string?)null,
            assembly = "fixture/KingdomZombies.Phase1.Tests.dll",
            executable = mode == "missing-executable" ? "fixture/missing.exe" : (string?)null,
            childTimeoutMs = 1800,
            inputs = new[] { "tools/phase1", "fixture", "global.json", "approved.cs", "approved.md" },
            cases = new[] { new { id = "fixture.pass", group = "runner", rule = "RULE-03", owner = "01-02", implementation = "csharp",
                prerequisites = new[] { "prebuilt-csharp" }, evidence = new[] { "report", "log" },
                test = "KingdomZombies.Phase1.Tests.RunnerProcessFixture." + method } },
        };
        File.WriteAllText(Path.Combine(Root, "tools", "phase1", "required-cases.json"), JsonSerializer.Serialize(manifest));
    }

    public Process Start(params string[] args)
    {
        return StartMode(Mode, args);
    }

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

    public (int Exit, string Output) Run(params string[] args)
    {
        using var child = Start(args);
        return Finish(child);
    }

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

    public JsonNode Summary()
    {
        var reports = Directory.GetFiles(Path.Combine(Root, ".phase1", "runs"), "summary.json", SearchOption.AllDirectories);
        return JsonNode.Parse(File.ReadAllText(reports.OrderBy(File.GetLastWriteTimeUtc).Last()))!;
    }

    public static string Hash(string file) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(file)));

    public void Dispose()
    {
        // Preserve isolated logs/results for inspection; all outputs live under the ignored run directory.
    }

    [Fact(Explicit = true), Trait("Case", "fixture.pass")]
    public void Child()
    {
        var mode = Environment.GetEnvironmentVariable("PHASE1_MUTATION") ?? "pass";
        File.WriteAllText(Path.Combine(ProofProtocol.Root, "child.pid"), Environment.ProcessId.ToString());
        if (mode == "concurrent") Thread.Sleep(1000);
        if (mode == "assertion") (2 + 2).ShouldBe(5, "INTENDED_ASSERTION_FAILURE at RunnerProcessFixture.Child");
        if (mode == "crash") Environment.Exit(71);
        if (mode is "hold" or "timeout")
        {
            if (mode == "timeout")
            {
                var descendant = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true };
                descendant.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
                descendant.ArgumentList.Add("--filter-method");
                descendant.ArgumentList.Add("KingdomZombies.Phase1.Tests.RunnerProcessFixture.Descendant");
                descendant.ArgumentList.Add("--no-ansi");
                descendant.ArgumentList.Add("--progress"); descendant.ArgumentList.Add("off");
                descendant.ArgumentList.Add("--explicit"); descendant.ArgumentList.Add("on");
                using var process = Process.Start(descendant)!;
                File.WriteAllText(Path.Combine(ProofProtocol.Root, "descendant.pid"), process.Id.ToString());
                var ready = Path.Combine(ProofProtocol.Root, "descendant.ready");
                var watch = Stopwatch.StartNew();
                while (!File.Exists(ready) && watch.ElapsedMilliseconds < 1000) Thread.Sleep(10);
                File.Exists(ready).ShouldBeTrue("descendant must actually execute before timeout");
            }
            Thread.Sleep(Timeout.Infinite);
        }
        (2 + 2).ShouldBe(4);
        ProofProtocol.Record("fixture.pass");
        var directory = Environment.GetEnvironmentVariable("PHASE1_RESULT_DIRECTORY")!;
        var file = Path.Combine(directory, "fixture.pass.proof.json");
        var result = JsonNode.Parse(File.ReadAllText(file))!;
        if (mode == "deleted-result") File.Delete(file);
        if (mode == "deleted-log") File.Delete(Path.Combine(directory, "fixture.pass.subject.log"));
        if (mode == "duplicate") File.Copy(file, Path.Combine(directory, "duplicate.proof.json"));
        if (mode == "nonce") { result["nonce"] = "prior-run"; File.WriteAllText(file, result.ToJsonString()); }
        if (mode == "stale") File.WriteAllText(file, File.ReadAllText(Path.Combine(ProofProtocol.Root, "prior.proof.json")));
        if (mode == "source") File.AppendAllText(Path.Combine(ProofProtocol.Root, "approved.cs"), "// unauthorized change\n");
        if (mode == "document") File.AppendAllText(Path.Combine(ProofProtocol.Root, "approved.md"), "Unauthorized change.\n");
    }

    [Fact(Explicit = true, Skip = "Intentional isolated skip control"), Trait("Case", "fixture.pass")]
    public void Skipped() { }

    [Fact(Explicit = true), Trait("Case", "fixture.wrong")]
    public void WrongSelection() { (2 + 2).ShouldBe(4); }

    [Fact(Explicit = true), Trait("Case", "fixture.descendant")]
    public void Descendant()
    {
        File.WriteAllText(Path.Combine(ProofProtocol.Root, "descendant.ready"), Environment.ProcessId.ToString());
        Thread.Sleep(Timeout.Infinite);
    }
}
