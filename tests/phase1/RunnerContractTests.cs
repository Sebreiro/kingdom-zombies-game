using Shouldly;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

public sealed class RunnerContractTests
{
    private static void ProcessRejects(string id, string mode, string diagnostic)
    {
        using var fixture = RunnerProcessFixture.Create(mode);
        var source = Path.Combine(fixture.Root, "approved.cs");
        var document = Path.Combine(fixture.Root, "approved.md");
        var sourceHash = RunnerProcessFixture.Hash(source);
        var documentHash = RunnerProcessFixture.Hash(document);
        var result = fixture.Run();
        result.Exit.ShouldBe(1);
        result.Output.ShouldContain(diagnostic);
        if (mode == "timeout")
        {
            File.ReadAllText(Path.Combine(fixture.Root, "descendant.ready")).ShouldBe(File.ReadAllText(Path.Combine(fixture.Root, "descendant.pid")));
        }
        fixture.Summary()["status"]!.GetValue<string>().ShouldBe("fail");
        if (mode != "source") RunnerProcessFixture.Hash(source).ShouldBe(sourceHash);
        if (mode != "document") RunnerProcessFixture.Hash(document).ShouldBe(documentHash);
        if (mode == "source") File.WriteAllText(source, "// Approved isolated fixture source\n");
        if (mode == "document") File.WriteAllText(document, "Approved isolated fixture document.\n");
        RunnerProcessFixture.Hash(source).ShouldBe(sourceHash);
        RunnerProcessFixture.Hash(document).ShouldBe(documentHash);
        AssertPidsExited(fixture);
        ProofProtocol.Record(id, diagnostic, "isolated normal entry point");
    }

    private static void AssertPidsExited(RunnerProcessFixture fixture)
    {
        foreach (var file in Directory.GetFiles(fixture.Root, "*.pid"))
        {
            var pid = int.Parse(File.ReadAllText(file));
            var exited = false;
            try { using var process = Process.GetProcessById(pid); exited = process.WaitForExit(1000); }
            catch (ArgumentException) { exited = true; }
            exited.ShouldBeTrue("child PID " + pid + " must exit");
        }
    }

    [Fact, Trait("Case", "runner.process.missing-executable")]
    public void RealMissingExecutableIsRejected() => ProcessRejects("runner.process.missing-executable", "missing-executable", "TOOL_MISSING");
    [Fact, Trait("Case", "runner.process.zero-discovery")]
    public void RealZeroDiscoveryIsRejected() => ProcessRejects("runner.process.zero-discovery", "zero", "DISCOVERY_EMPTY");
    [Fact, Trait("Case", "runner.process.wrong-filter")]
    public void RealWrongFilterIsRejected() => ProcessRejects("runner.process.wrong-filter", "filter", "DISCOVERY_WRONG");
    [Fact, Trait("Case", "runner.process.skip")]
    public void RealSkipIsRejected() => ProcessRejects("runner.process.skip", "skip", "TEST_SKIPPED");
    [Fact, Trait("Case", "runner.process.crash")]
    public void RealCrashIsRejected() => ProcessRejects("runner.process.crash", "crash", "REPORT_MISSING");
    [Fact, Trait("Case", "runner.process.timeout")]
    public void RealTimeoutKillsDescendants() => ProcessRejects("runner.process.timeout", "timeout", "CHILD_TIMEOUT");
    [Fact, Trait("Case", "runner.process.deleted-log")]
    public void RealDeletedLogIsRejected() => ProcessRejects("runner.process.deleted-log", "deleted-log", "EVIDENCE_MISSING");
    [Fact, Trait("Case", "runner.process.deleted-result")]
    public void RealDeletedResultIsRejected() => ProcessRejects("runner.process.deleted-result", "deleted-result", "RESULT_MISSING");
    [Fact, Trait("Case", "runner.process.duplicate")]
    public void RealDuplicateResultIsRejected() => ProcessRejects("runner.process.duplicate", "duplicate", "RESULT_DUPLICATE");
    [Fact, Trait("Case", "runner.process.nonce")]
    public void RealWrongNonceIsRejected() => ProcessRejects("runner.process.nonce", "nonce", "NONCE_MISMATCH");
    [Fact, Trait("Case", "runner.process.source")]
    public void RealSourceMutationInvalidatesPublication() => ProcessRejects("runner.process.source", "source", "SOURCE_CHANGED");
    [Fact, Trait("Case", "runner.process.document")]
    public void RealDocumentMutationInvalidatesPublication() => ProcessRejects("runner.process.document", "document", "SOURCE_CHANGED");

    [Fact, Trait("Case", "runner.process.repeat")]
    public void UnchangedRunsHaveIdenticalNormalizedVerdicts()
    {
        using var fixture = RunnerProcessFixture.Create("pass");
        fixture.Run().Exit.ShouldBe(0);
        var first = fixture.Summary();
        fixture.Run().Exit.ShouldBe(0);
        var second = fixture.Summary();
        first["nonce"]!.GetValue<string>().ShouldNotBe(second["nonce"]!.GetValue<string>());
        foreach (var key in new[] { "status", "input", "tool", "platform", "selected", "results", "failures" })
        {
            if (key == "results")
            {
                foreach (var row in first[key]!.AsArray()) row!.AsObject().Remove("report");
                foreach (var row in second[key]!.AsArray()) row!.AsObject().Remove("report");
            }
            first[key]!.ToJsonString().ShouldBe(second[key]!.ToJsonString(), key);
        }
        AssertPidsExited(fixture);
        ProofProtocol.Record("runner.process.repeat", "NORMALIZED_EQUAL", "two complete real C# runs");
    }

    [Fact, Trait("Case", "runner.process.stale")]
    public void NewRunCannotBorrowPriorCompleteArtifacts()
    {
        using var fixture = RunnerProcessFixture.Create("pass");
        fixture.Run().Exit.ShouldBe(0);
        var prior = Directory.GetFiles(Path.Combine(fixture.Root, ".phase1", "runs"), "fixture.pass.proof.json", SearchOption.AllDirectories).Single();
        File.Copy(prior, Path.Combine(fixture.Root, "prior.proof.json"));
        using var child = fixture.StartMode("stale");
        var result = RunnerProcessFixture.Finish(child);
        result.Exit.ShouldBe(1);
        result.Output.ShouldContain("NONCE_MISMATCH");
        AssertPidsExited(fixture);
        ProofProtocol.Record("runner.process.stale", "NONCE_MISMATCH", "fresh run with prior proof file");
    }

    private static void AwaitReady(RunnerProcessFixture fixture)
    {
        var ready = Path.Combine(fixture.Root, "child.pid");
        var watch = Stopwatch.StartNew();
        while (!File.Exists(ready) && watch.ElapsedMilliseconds < 2500) Thread.Sleep(10);
        File.Exists(ready).ShouldBeTrue("real child must start before concurrency/interruption mutation");
    }

    [Fact, Trait("Case", "runner.process.concurrent")]
    public void ConcurrentFullInvocationIsExplicitlyRejected()
    {
        using var fixture = RunnerProcessFixture.Create("concurrent");
        using var first = fixture.Start();
        AwaitReady(fixture);
        var second = fixture.Run();
        second.Exit.ShouldBe(1);
        second.Output.ShouldContain("CHECK_LOCKED");
        var firstResult = RunnerProcessFixture.Finish(first);
        firstResult.Exit.ShouldBe(0);
        fixture.Summary()["status"]!.GetValue<string>().ShouldBe("pass");
        fixture.Summary()["selected"]!.AsArray().Count.ShouldBe(1);
        AssertPidsExited(fixture);
        ProofProtocol.Record("runner.process.concurrent", "CHECK_LOCKED", "competing --all invocation");
    }

    [Fact, Trait("Case", "runner.process.interrupted")]
    public void InterruptionCannotPublishOrLendSuccess()
    {
        using var fixture = RunnerProcessFixture.Create("hold");
        using var first = fixture.Start();
        AwaitReady(fixture);
        first.Kill(entireProcessTree: true);
        first.WaitForExit(1000).ShouldBeTrue();
        Directory.GetFiles(Path.Combine(fixture.Root, ".phase1", "runs"), "summary.json", SearchOption.AllDirectories).ShouldBeEmpty();
        AssertPidsExited(fixture);
        fixture.Run().Output.ShouldContain("CHECK_LOCKED");
        // Recovery is local to the fixture, after the owner and descendants are proved dead.
        File.Delete(Path.Combine(fixture.Root, ".phase1", "check.lock"));
        using var next = fixture.StartMode("pass");
        RunnerProcessFixture.Finish(next).Exit.ShouldBe(0);
        Directory.GetFiles(Path.Combine(fixture.Root, ".phase1", "runs"), "summary.json", SearchOption.AllDirectories).Length.ShouldBe(1);
        ProofProtocol.Record("runner.process.interrupted", "NO_PARTIAL_PUBLICATION", "killed real coordinator and fresh rerun");
    }

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
    [Fact(Explicit = true), Trait("Case", "fixture.assertion")]
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
