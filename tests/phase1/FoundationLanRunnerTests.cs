using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves exact LAN evidence, child failures and invocation ownership without opening windows.</summary>
public sealed class FoundationLanRunnerTests
{
    /// <summary>Rejects fabricated, substituted and stale case/peer/image evidence.</summary>
    [Fact]
    public void Evidence_MalformedSubjects_AreRejected()
    {
        FoundationCapturePublicationTests.Check();
        Check(mode: "integrity", id: "runner.integrity");
    }

    /// <summary>Exercises actual child exits, crashes and timeout descendant cleanup.</summary>
    [Fact]
    public void Child_ActualFailures_CannotPass()
    {
        Check(mode: "processes", id: "runner.processes");
    }

    /// <summary>Preserves immutable reports and refuses an overlapping desktop owner.</summary>
    [Fact]
    public void Runs_FreshSessions_RetainExactOwnership()
    {
        Check(mode: "repeatability", id: "runner.repeatability");
    }

    private static void Check(string mode, string id)
    {
        (int Exit, string Output) result = Node(script:
                "require('./tools/phase3/runner-fixture.cjs').run('" + mode + "')"
                        + ".catch(error=>{console.error(error.stack);process.exitCode=1;});");
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        result.Output.ShouldContain(expected: "PASS");
        ProofProtocol.Record(id: id, diagnostic: "EXACT_LAN_RUNNER_CONTROLS", location: result.Output);
    }

    private static (int Exit, string Output) Node(string script)
    {
        var start = new ProcessStartInfo(fileName: Environment.GetEnvironmentVariable(variable: "PHASE1_NODE")
                ?? "node")
        {
            WorkingDirectory = ProofProtocol.Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        start.ArgumentList.Add(item: "-e");
        start.ArgumentList.Add(item: script);
        using Process child = Process.Start(startInfo: start)
                ?? throw new InvalidOperationException(message: "Node failed to start");
        Task<string> output = child.StandardOutput.ReadToEndAsync();
        Task<string> error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(milliseconds: 40000))
        {
            child.Kill(entireProcessTree: true);
            child.WaitForExit();
            throw new TimeoutException(message: "Phase 3 isolated Git and SDK/child controls exceeded 40 seconds");
        }

        return (child.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }
}
