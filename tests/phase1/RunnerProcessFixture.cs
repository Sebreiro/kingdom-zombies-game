using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Creates isolated runner inputs and explicit child controls without mutating approved repository
/// inputs.</summary>
public sealed class RunnerProcessFixture : IDisposable
{
    /// <summary>Binds the child fixture to the current proof root and requested mutation mode.</summary>
    public RunnerProcessFixture()
    {
        Root = ProofProtocol.Root;
        Mode = Environment.GetEnvironmentVariable(variable: "PHASE1_MUTATION") ?? "pass";
    }

    /// <summary>Gets the checkout or isolated fixture directory used as the child working directory.</summary>
    public string Root { get; }
    /// <summary>Gets the deliberate child mutation selected for this fixture.</summary>
    public string Mode { get; }

    /// <summary>Copies an isolated runner and prebuilt test assembly for the requested mutation.</summary>
    public static RunnerProcessFixture Create(string mode)
    {
        return new(mode: mode);
    }

    /// <summary>Starts the fixture coordinator with this fixture mutation and the supplied selection
    /// arguments.</summary>
    public Process Start(params string[] args)
    {
        return StartMode(mode: Mode, args: args);
    }

    /// <summary>Starts a coordinator with an explicit mutation, redirected output and no visible console.</summary>
    public Process StartMode(string mode, params string[] args)
    {
        var start = new ProcessStartInfo(fileName: Environment.GetEnvironmentVariable(variable: "PHASE1_NODE")
                ?? "node")
        {
            WorkingDirectory = Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        start.ArgumentList.Add(item: Path.Combine(path1: Root, path2: "tools", path3: "phase1", path4: "check.cjs"));
        foreach (string arg in args.Length == 0 ? ["--all"] : args)
        {
            start.ArgumentList.Add(item: arg);
        }

        start.Environment["PHASE1_MUTATION"] = mode;
        return Process.Start(startInfo: start) ?? throw new InvalidOperationException(
                message: "Fixture coordinator did not start");
    }

    /// <summary>Runs the fixture coordinator and returns its exit status and combined captured output.</summary>
    public (int Exit, string Output) Run(params string[] args)
    {
        using Process child = Start(args: args);
        return Finish(child: child);
    }

    /// <summary>Collects coordinator output within five seconds, killing its entire process tree on timeout.</summary>
    public static (int Exit, string Output) Finish(Process child)
    {
        Task<string> output = child.StandardOutput.ReadToEndAsync();
        Task<string> error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(milliseconds: 5000))
        {
            child.Kill(entireProcessTree: true);
            child.WaitForExit();
            throw new TimeoutException(message: "Controlled coordinator exceeded 5 seconds");
        }

        return (child.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }

    /// <summary>Reads the newest summary published by this isolated fixture.</summary>
    public JsonNode Summary()
    {
        string[] reports = Directory.GetFiles(path: Path.Combine(path1: Root, path2: ".phase1", path3: "runs"),
                searchPattern: "summary.json", searchOption: SearchOption.AllDirectories);
        return (JsonNode.Parse(json: File.ReadAllText(path: reports.OrderBy(keySelector: File
                .GetLastWriteTimeUtc).Last())) ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null"));
    }

    /// <summary>Computes an uppercase SHA-256 fingerprint of the specified file bytes.</summary>
    public static string Hash(string file)
    {
        return Convert.ToHexString(inArray: SHA256.HashData(source: File.ReadAllBytes(path: file)));
    }

    /// <summary>Preserves isolated diagnostic artifacts for inspection; child cleanup is owned by bounded
    /// execution.</summary>
    public void Dispose()
    {
    }

    /// <summary>Executes the selected explicit process-failure control and mutates evidence only in the
    /// isolated fixture.</summary>
    [Fact(Explicit = true), Trait("Case", "fixture.pass")]
    public void Run_ExplicitChild_EmitsSelectedResult()
    {
        string mode = Environment.GetEnvironmentVariable(variable: "PHASE1_MUTATION") ?? "pass";
        ExerciseProcessFailure(mode: mode);
        (2 + 2).ShouldBe(expected: 4);
        ProofProtocol.Record(id: "fixture.pass");
        MutateEvidence(mode: mode);
    }

    /// <summary>Provides the intentional explicit skip that proves the parent rejects skipped required cases.</summary>
    [Fact(Explicit = true, Skip = "Intentional isolated skip control"), Trait("Case", "fixture.pass")]
    public void Run_ExplicitSkipped_IsRejectedByParent()
    {
    }

    /// <summary>Provides a passing explicit case with the wrong identity to prove discovery reconciliation.</summary>
    [Fact(Explicit = true), Trait("Case", "fixture.wrong")]
    public void Run_WrongSelection_IsRejectedByParent()
    {
        (2 + 2).ShouldBe(expected: 4);
    }

    /// <summary>Signals actual descendant startup, then waits for the parent timeout cleanup control.</summary>
    [Fact(Explicit = true), Trait("Case", "fixture.descendant")]
    public void Run_Descendant_WaitsForCleanup()
    {
        File.WriteAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: "descendant.ready"),
                contents: Environment.ProcessId.ToString(provider: System.Globalization.CultureInfo.InvariantCulture));
        Thread.Sleep(millisecondsTimeout: Timeout.Infinite);
    }

    private RunnerProcessFixture(string mode)
    {
        Mode = mode;
        string parent = Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY") ?? Path.Combine(
                path1: ProofProtocol.Root, path2: ".phase1", path3: "temp");
        Root = Path.GetFullPath(path: Path.Combine(path1: parent, path2: "fixture-" + Guid.NewGuid()));
        CopyFixtureInputs();
        WriteFixtureManifest(mode: mode);
    }

    private void CopyFixtureInputs()
    {
        Directory.CreateDirectory(path: Path.Combine(path1: Root, path2: "tools", path3: "phase1"));
        foreach (string? file in new[]
        {
            "check.cjs",
            "bootstrap.cjs",
            "toolchain.json"
        }

        )
        {
            File.Copy(sourceFileName: Path.Combine(path1: ProofProtocol.Root, path2: "tools", path3: "phase1",
                    path4: file), destFileName: Path.Combine(path1: Root, path2: "tools", path3: "phase1",
                            path4: file));
        }

        File.Copy(sourceFileName: Path.Combine(path1: ProofProtocol.Root, path2: "global.json"),
                destFileName: Path.Combine(path1: Root, path2: "global.json"));
        Directory.CreateDirectory(path: Path.Combine(path1: Root, path2: "fixture"));
        foreach (string file in Directory.GetFiles(path: AppContext.BaseDirectory))
        {
            File.Copy(sourceFileName: file, destFileName: Path.Combine(path1: Root, path2: "fixture",
                    path3: Path.GetFileName(path: file)));
        }

        File.WriteAllText(path: Path.Combine(path1: Root, path2: "approved.cs"),
                contents: "// Approved isolated fixture source\n");
        File.WriteAllText(path: Path.Combine(path1: Root, path2: "approved.md"),
                contents: "Approved isolated fixture document.\n");
    }

    private void WriteFixtureManifest(string mode)
    {
        string method = StringComparer.Ordinal.Equals(x: mode, y: "skip")
                ? "Run_ExplicitSkipped_IsRejectedByParent" : StringComparer.Ordinal
                        .Equals(x: mode, y: "zero") ? "NoSuchTest" : "Run_ExplicitChild_EmitsSelectedResult";
        if (StringComparer.Ordinal.Equals(x: mode, y: "filter"))
        {
            method = "Run_WrongSelection_IsRejectedByParent";
        }

        var manifest = new
        {
            schemaVersion = 1,
            project = (string?)null,
            assembly = "fixture/KingdomZombies.Phase1.Tests.dll",
            executable = StringComparer.Ordinal.Equals(x: mode, y: "missing-executable")
                    ? "fixture/missing.exe" : (string?)null,
            childTimeoutMs = 1800,
            inputs = new[]
            {
                "tools/phase1",
                "fixture",
                "global.json",
                "approved.cs",
                "approved.md"
            },
            cases = new[]
            {
                new
                {
                    id = "fixture.pass",
                    group = "runner",
                    rule = "RULE-03",
                    owner = "01-02",
                    implementation = "csharp",
                    prerequisites = new[]
                    {
                        "prebuilt-csharp"
                    },
                    evidence = new[]
                    {
                        "report",
                        "log"
                    },
                    test = "KingdomZombies.Phase1.Tests.RunnerProcessFixture." + method
                }
            },
        };
        File.WriteAllText(path: Path.Combine(path1: Root, path2: "tools", path3: "phase1",
                path4: "required-cases.json"), contents: JsonSerializer.Serialize(value: manifest));
    }

    private static void ExerciseProcessFailure(string mode)
    {
        File.WriteAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: "child.pid"),
                contents: Environment.ProcessId.ToString(provider: System.Globalization.CultureInfo.InvariantCulture));
        if (StringComparer.Ordinal.Equals(x: mode, y: "concurrent"))
        {
            Thread.Sleep(millisecondsTimeout: 1000);
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "assertion"))
        {
            (2 + 2).ShouldBe(expected: 5, customMessage: "INTENDED_ASSERTION_FAILURE at RunnerProcessFixture."
                    + "Run_ExplicitChild_EmitsSelectedResult");
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "crash"))
        {
            Environment.Exit(exitCode: 71);
        }

        if (mode is "hold" or "timeout")
        {
            if (StringComparer.Ordinal.Equals(x: mode, y: "timeout"))
            {
                var descendant = new ProcessStartInfo(fileName: "dotnet")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                descendant.ArgumentList.Add(item: Assembly.GetExecutingAssembly().Location);
                descendant.ArgumentList.Add(item: "--filter-method");
                descendant.ArgumentList.Add(item: "KingdomZombies.Phase1.Tests.RunnerProcessFixture."
                        + "Run_Descendant_WaitsForCleanup");
                descendant.ArgumentList.Add(item: "--no-ansi");
                descendant.ArgumentList.Add(item: "--progress");
                descendant.ArgumentList.Add(item: "off");
                descendant.ArgumentList.Add(item: "--explicit");
                descendant.ArgumentList.Add(item: "on");
                using Process process = (Process.Start(startInfo: descendant)
                        ?? throw new System.InvalidOperationException(message: "Required proof value is null"));
                File.WriteAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: "descendant.pid"),
                        contents: process.Id.ToString(provider: System.Globalization.CultureInfo.InvariantCulture));
                string ready = Path.Combine(path1: ProofProtocol.Root, path2: "descendant.ready");
                var watch = Stopwatch.StartNew();
                while (!File.Exists(path: ready) && watch.ElapsedMilliseconds < 1000)
                {
                    Thread.Sleep(millisecondsTimeout: 10);
                }

                File.Exists(path: ready).ShouldBeTrue(customMessage: "descendant must actually execute before timeout");
            }

            Thread.Sleep(millisecondsTimeout: Timeout.Infinite);
        }
    }

    private static void MutateEvidence(string mode)
    {
        string directory = (Environment.GetEnvironmentVariable(variable: "PHASE1_RESULT_DIRECTORY")
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"));
        string file = Path.Combine(path1: directory, path2: "fixture.pass.proof.json");
        JsonNode result = (JsonNode.Parse(json: File.ReadAllText(path: file))
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"));
        if (StringComparer.Ordinal.Equals(x: mode, y: "deleted-result"))
        {
            File.Delete(path: file);
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "deleted-log"))
        {
            File.Delete(path: Path.Combine(path1: directory, path2: "fixture.pass.subject.log"));
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "duplicate"))
        {
            File.Copy(sourceFileName: file, destFileName: Path.Combine(path1: directory,
                    path2: "duplicate.proof.json"));
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "nonce"))
        {
            result["nonce"] = "prior-run";
            File.WriteAllText(path: file, contents: result.ToJsonString());
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "stale"))
        {
            File.WriteAllText(path: file, contents: File.ReadAllText(path: Path.Combine(path1: ProofProtocol
                    .Root, path2: "prior.proof.json")));
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "source"))
        {
            File.AppendAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: "approved.cs"),
                    contents: "// unauthorized change\n");
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "document"))
        {
            File.AppendAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: "approved.md"),
                    contents: "Unauthorized change.\n");
        }
    }
}
