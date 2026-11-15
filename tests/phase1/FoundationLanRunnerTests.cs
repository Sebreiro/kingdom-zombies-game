using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves exact LAN evidence, child failures and invocation ownership without opening windows.</summary>
public sealed class FoundationLanRunnerTests
{
    /// <summary>Rejects fabricated, substituted and stale case/peer/image evidence.</summary>
    [Fact]
    public void ExactEvidenceRejectsMalformedSubjects()
    {
        Check(mode: "integrity", id: "runner.integrity");
    }

    /// <summary>Exercises actual child exits, crashes and timeout descendant cleanup.</summary>
    [Fact]
    public void ActualChildFailuresCannotPass()
    {
        Check(mode: "processes", id: "runner.processes");
    }

    /// <summary>Preserves immutable reports and refuses an overlapping desktop owner.</summary>
    [Fact]
    public void FreshRunsRetainExactOwnership()
    {
        Check(mode: "repeatability", id: "runner.repeatability");
    }

    private static void Check(string mode, string id)
    {
        (int Exit, string Output) result = ProofProtocol.Node(script:
                "require('./tools/phase3/runner-fixture.cjs').run('" + mode + "')"
                        + ".catch(error=>{console.error(error.stack);process.exitCode=1;});");
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        result.Output.ShouldContain(expected: "PASS");
        ProofProtocol.Record(id: id, diagnostic: "EXACT_LAN_RUNNER_CONTROLS", location: result.Output);
    }
}
