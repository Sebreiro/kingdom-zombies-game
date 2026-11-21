using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises foundation evidence rejection without starting a game window.</summary>
public sealed class FoundationRunnerTests
{
    /// <summary>Rejects missing, duplicate, stale and corrupt foundation evidence.</summary>
    [Fact]
    public void Evidence_InvalidRecords_CannotPass()
    {
        Check(mode: "integrity", id: "runner.integrity");
    }

    /// <summary>Observes actual failing children and verifies timeout descendant cleanup.</summary>
    [Fact]
    public void Child_FailingProcesses_CannotPass()
    {
        Check(mode: "processes", id: "runner.processes");
    }

    /// <summary>Preserves original evidence while ordering and focused repair runs remain explicit.</summary>
    [Fact]
    public void Evidence_RepairedRuns_RetainOriginalIdentity()
    {
        Check(mode: "repeatability", id: "runner.repeatability");
    }

    private static void Check(string mode, string id)
    {
        (int Exit, string Output) result = ProofProtocol.Node(script:
                "require('./tools/phase2/runner-fixture.cjs').run('" + mode + "')"
                        + ".catch(error=>{console.error(error.stack);process.exitCode=1;});");
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        result.Output.ShouldContain(expected: "PASS");
        ProofProtocol.Record(id: id, diagnostic: "FOUNDATION_RUNNER_CONTROLS", location: result.Output);
    }
}
