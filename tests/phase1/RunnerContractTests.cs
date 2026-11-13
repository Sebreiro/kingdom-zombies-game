using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

public sealed class RunnerContractTests
{
    [Fact]
    [Trait("Case", "runner.missing-result")]
    public void MissingRequiredResultCannotPass()
    {
        var result = ProofProtocol.Node("const c=require('./tools/phase1/check.cjs'); try { c.reconcile([{id:'required'}], [], {}); process.exitCode=0; } catch(e) { console.log(e.diagnostic); process.exitCode=1; }");
        result.Exit.ShouldBe(1, "missing required evidence must reject acceptance");
        result.Output.ShouldContain("RESULT_MISSING");
        ProofProtocol.Record("runner.missing-result", "RESULT_MISSING", "required");
    }

    [Fact]
    [Trait("Case", "runner.assertion-pass")]
    public void RealAssertionPasses()
    {
        (2 + 2).ShouldBe(4);
        ProofProtocol.Record("runner.assertion-pass");
    }
}
