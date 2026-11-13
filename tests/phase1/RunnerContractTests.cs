using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

public sealed class RunnerContractTests
{
    [Fact, Trait("Case", "runner.process.assertion")]
    public void RealAssertionFailureIsActionable()
    {
        using var fixture = RunnerProcessFixture.Create("assertion");
        var result = fixture.Run();
        result.Exit.ShouldBe(1);
        result.Output.ShouldContain("TEST_FAILED");
        result.Output.ShouldContain("INTENDED_ASSERTION_FAILURE");
        result.Output.ShouldContain("[log](");
        ProofProtocol.Record("runner.process.assertion", "TEST_FAILED", "RunnerProcessFixture.Child");
    }

    // Only selected inside isolated negative-control workspaces, never as a passing obligation.
    [Fact, Trait("Case", "fixture.assertion")]
    public void DeliberateAssertionFailure()
    {
        (2 + 2).ShouldBe(5, "INTENDED_ASSERTION_FAILURE at RunnerContractTests.cs");
    }

    private static void RejectMutation(string id, string mutation, string expected)
    {
        var script = "const c=require('./tools/phase1/check.cjs'); const fs=require('node:fs'); const os=require('node:os'); const path=require('node:path'); " +
            "const directory=fs.mkdtempSync(path.join(process.env.TEMP||os.tmpdir(),'reconcile-')); fs.writeFileSync(path.join(directory,'subject.log'),'subject diagnostic'); " +
            "const selected=[{id:'required',expectedDiagnostic:'EXPECTED'}]; const context={nonce:'fresh',input:'input',tool:'tool',platform:'platform',directory}; " +
            "let results=[{id:'required',nonce:'fresh',input:'input',tool:'tool',platform:'platform',status:'pass',diagnostic:'EXPECTED',location:'subject.cs:3',prerequisites:['build','discovery','execution'],evidence:['subject.log']}]; " +
            mutation + "; try { c.reconcile(selected,results,context); console.log('ACCEPTED'); } catch(e) { console.log(e.diagnostic); process.exitCode=1; }";
        var result = ProofProtocol.Node(script);
        result.Exit.ShouldBe(1);
        result.Output.Trim().ShouldBe(expected);
        ProofProtocol.Record(id, expected, "subject.cs:3");
    }

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

    [Fact, Trait("Case", "runner.duplicate-result")]
    public void DuplicateResultIsRejectedBeforeSetConversion() => RejectMutation("runner.duplicate-result", "results.push(results[0])", "RESULT_DUPLICATE");

    [Fact, Trait("Case", "runner.unknown-result")]
    public void UnknownResultCannotSatisfyAnotherObligation() => RejectMutation("runner.unknown-result", "results[0].id='unknown'", "RESULT_UNKNOWN");

    [Fact, Trait("Case", "runner.wrong-nonce")]
    public void WrongNonceCannotReusePriorEvidence() => RejectMutation("runner.wrong-nonce", "results[0].nonce='old'", "NONCE_MISMATCH");

    [Fact, Trait("Case", "runner.changed-input")]
    public void ChangedInputCannotBorrowEvidence() => RejectMutation("runner.changed-input", "results[0].input='changed'", "INPUT_MISMATCH");

    [Fact, Trait("Case", "runner.wrong-tool")]
    public void WrongToolCannotBorrowEvidence() => RejectMutation("runner.wrong-tool", "results[0].tool='different'", "TOOL_MISMATCH");

    [Fact, Trait("Case", "runner.wrong-platform")]
    public void WrongPlatformCannotBorrowEvidence() => RejectMutation("runner.wrong-platform", "results[0].platform='different'", "PLATFORM_MISMATCH");

    [Fact, Trait("Case", "runner.skipped-result")]
    public void SkippedResultCannotPass() => RejectMutation("runner.skipped-result", "results[0].status='skipped'", "RESULT_NOT_PASS");

    [Fact, Trait("Case", "runner.blocked-result")]
    public void BlockedResultCannotPass() => RejectMutation("runner.blocked-result", "results[0].status='blocked'", "RESULT_NOT_PASS");

    [Fact, Trait("Case", "runner.empty-evidence")]
    public void EmptyEvidenceCannotPass() => RejectMutation("runner.empty-evidence", "results[0].evidence=[]", "EVIDENCE_MISSING");

    [Fact, Trait("Case", "runner.escaping-evidence")]
    public void EvidenceCannotEscapeRunDirectory() => RejectMutation("runner.escaping-evidence", "results[0].evidence=['../outside.log']", "PATH_ESCAPE");

    [Fact, Trait("Case", "runner.unrelated-diagnostic")]
    public void InfrastructureFailureCannotStandForSubjectViolation() => RejectMutation("runner.unrelated-diagnostic", "results[0].diagnostic='RESTORE_FAILED'", "SUBJECT_DIAGNOSTIC");

    [Fact, Trait("Case", "runner.missing-prerequisite")]
    public void NegativeEvidenceRequiresSuccessfulInfrastructure() => RejectMutation("runner.missing-prerequisite", "results[0].prerequisites=['build']", "SUBJECT_EVIDENCE");

    [Fact, Trait("Case", "runner.selection")]
    public void WrongAndEmptySelectionsAreRejected()
    {
        var script = "const c=require('./tools/phase1/check.cjs');const m=require('./tools/phase1/required-cases.json');const rows=[['--group','unknown'],['--case','unknown'],['--case'],['--unknown'],['--all','--case','runner.assertion-pass']];for(const args of rows){let rejected=false;try{c.select(m,args)}catch{rejected=true}if(!rejected)process.exitCode=1}console.log('SELECTION_REJECTED')";
        var result = ProofProtocol.Node(script);
        result.Exit.ShouldBe(0);
        result.Output.ShouldContain("SELECTION_REJECTED");
        ProofProtocol.Record("runner.selection", "SELECTION_REJECTED", "CLI selection");
    }
}
