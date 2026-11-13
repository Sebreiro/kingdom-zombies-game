using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Exercises real child-process failures and evidence reconciliation before accepting proof results.</summary>
public sealed class RunnerContractTests
{
    /// <summary>Requires TOOL_MISSING when the isolated runner executable does not exist.</summary>
    [Fact, Trait("Case", "runner.process.missing-executable")]
    public void RealMissingExecutableIsRejected()
    {
        ProcessRejects(id: "runner.process.missing-executable", mode: "missing-executable", diagnostic: "TOOL_MISSING");
    }

    /// <summary>Requires DISCOVERY_EMPTY when the requested method discovers no test.</summary>
    [Fact, Trait("Case", "runner.process.zero-discovery")]
    public void RealZeroDiscoveryIsRejected()
    {
        ProcessRejects(id: "runner.process.zero-discovery", mode: "zero", diagnostic: "DISCOVERY_EMPTY");
    }

    /// <summary>Requires DISCOVERY_WRONG when a real child selects a different case.</summary>
    [Fact, Trait("Case", "runner.process.wrong-filter")]
    public void RealWrongFilterIsRejected()
    {
        ProcessRejects(id: "runner.process.wrong-filter", mode: "filter", diagnostic: "DISCOVERY_WRONG");
    }

    /// <summary>Requires TEST_SKIPPED for the intentionally skipped isolated control.</summary>
    [Fact, Trait("Case", "runner.process.skip")]
    public void RealSkipIsRejected()
    {
        ProcessRejects(id: "runner.process.skip", mode: "skip", diagnostic: "TEST_SKIPPED");
    }

    /// <summary>Requires REPORT_MISSING when a real child exits before producing a report.</summary>
    [Fact, Trait("Case", "runner.process.crash")]
    public void RealCrashIsRejected()
    {
        ProcessRejects(id: "runner.process.crash", mode: "crash", diagnostic: "REPORT_MISSING");
    }

    /// <summary>Requires a child timeout and verifies cleanup of its running descendant.</summary>
    [Fact, Trait("Case", "runner.process.timeout")]
    public void RealTimeoutKillsDescendants()
    {
        ProcessRejects(id: "runner.process.timeout", mode: "timeout", diagnostic: "CHILD_TIMEOUT");
    }

    /// <summary>Rejects a real child result whose required subject log was deleted.</summary>
    [Fact, Trait("Case", "runner.process.deleted-log")]
    public void RealDeletedLogIsRejected()
    {
        ProcessRejects(id: "runner.process.deleted-log", mode: "deleted-log", diagnostic: "EVIDENCE_MISSING");
    }

    /// <summary>Rejects a real child execution whose proof result was deleted.</summary>
    [Fact, Trait("Case", "runner.process.deleted-result")]
    public void RealDeletedResultIsRejected()
    {
        ProcessRejects(id: "runner.process.deleted-result", mode: "deleted-result", diagnostic: "RESULT_MISSING");
    }

    /// <summary>Rejects two real proof files claiming the same required case.</summary>
    [Fact, Trait("Case", "runner.process.duplicate")]
    public void RealDuplicateResultIsRejected()
    {
        ProcessRejects(id: "runner.process.duplicate", mode: "duplicate", diagnostic: "RESULT_DUPLICATE");
    }

    /// <summary>Rejects a real proof file carrying another invocation nonce.</summary>
    [Fact, Trait("Case", "runner.process.nonce")]
    public void RealWrongNonceIsRejected()
    {
        ProcessRejects(id: "runner.process.nonce", mode: "nonce", diagnostic: "NONCE_MISMATCH");
    }

    /// <summary>Prevents publication after a child modifies an approved source input.</summary>
    [Fact, Trait("Case", "runner.process.source")]
    public void RealSourceMutationInvalidatesPublication()
    {
        ProcessRejects(id: "runner.process.source", mode: "source", diagnostic: "SOURCE_CHANGED");
    }

    /// <summary>Prevents publication after a child modifies an approved document input.</summary>
    [Fact, Trait("Case", "runner.process.document")]
    public void RealDocumentMutationInvalidatesPublication()
    {
        ProcessRejects(id: "runner.process.document", mode: "document", diagnostic: "SOURCE_CHANGED");
    }

    /// <summary>Checks repeated unchanged runs produce identical normalized verdicts.</summary>
    [Fact, Trait("Case", "runner.process.repeat")]
    public void UnchangedRunsHaveIdenticalNormalizedVerdicts()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "pass");
        fixture.Run().Exit.ShouldBe(expected: 0);
        JsonNode first = fixture.Summary();
        fixture.Run().Exit.ShouldBe(expected: 0);
        JsonNode second = fixture.Summary();
        (first["nonce"] ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .GetValue<string>().ShouldNotBe(expected: (second["nonce"]
                        ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                        .GetValue<string>(), comparer: StringComparer.Ordinal);
        foreach (string? key in new[]
        {
            "status",
            "input",
            "tool",
            "platform",
            "selected",
            "results",
            "failures"
        }

        )
        {
            if (StringComparer.Ordinal.Equals(x: key, y: "results"))
            {
                foreach (JsonNode? row in (first[key] ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null")).AsArray())
                {
                    (row ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                            .AsObject().Remove(propertyName: "report");
                }

                foreach (JsonNode? row in (second[key] ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null")).AsArray())
                {
                    (row ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                            .AsObject().Remove(propertyName: "report");
                }
            }

            (first[key] ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                    .ToJsonString().ShouldBe(expected: (second[key]
                            ?? throw new System.InvalidOperationException(
                                    message: "Required proof value is null")).ToJsonString(), customMessage: key);
        }

        AssertPidsExited(fixture: fixture);
        ProofProtocol.Record(id: "runner.process.repeat", diagnostic: "NORMALIZED_EQUAL",
                location: "two complete real C# runs");
    }

    /// <summary>Requires fresh evidence instead of accepting a previous run result.</summary>
    [Fact, Trait("Case", "runner.process.stale")]
    public void NewRunCannotBorrowPriorCompleteArtifacts()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "pass");
        fixture.Run().Exit.ShouldBe(expected: 0);
        string prior = Directory.GetFiles(path: Path.Combine(path1: fixture.Root, path2: ".phase1",
                path3: "runs"), searchPattern: "fixture.pass.proof.json", searchOption: SearchOption
                        .AllDirectories).Single();
        File.Copy(sourceFileName: prior, destFileName: Path.Combine(path1: fixture.Root, path2: "prior.proof.json"));
        using Process child = fixture.StartMode(mode: "stale");
        (int Exit, string Output) result = RunnerProcessFixture.Finish(child: child);
        result.Exit.ShouldBe(expected: 1);
        result.Output.ShouldContain(expected: "NONCE_MISMATCH");
        AssertPidsExited(fixture: fixture);
        ProofProtocol.Record(id: "runner.process.stale", diagnostic: "NONCE_MISMATCH",
                location: "fresh run with prior proof file");
    }

    /// <summary>Checks a second full invocation is rejected while the first owns the runner lock.</summary>
    [Fact, Trait("Case", "runner.process.concurrent")]
    public void ConcurrentFullInvocationIsExplicitlyRejected()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "concurrent");
        using Process first = fixture.Start();
        AwaitReady(fixture: fixture);
        (int Exit, string Output) second = fixture.Run();
        second.Exit.ShouldBe(expected: 1);
        second.Output.ShouldContain(expected: "CHECK_LOCKED");
        (int Exit, string Output) firstResult = RunnerProcessFixture.Finish(child: first);
        firstResult.Exit.ShouldBe(expected: 0);
        (fixture.Summary()["status"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).GetValue<string>().ShouldBe(expected: "pass");
        (fixture.Summary()["selected"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Count.ShouldBe(expected: 1);
        AssertPidsExited(fixture: fixture);
        ProofProtocol.Record(id: "runner.process.concurrent", diagnostic: "CHECK_LOCKED",
                location: "competing --all invocation");
    }

    /// <summary>Kills an active run and verifies it cannot publish or supply a later passing result.</summary>
    [Fact, Trait("Case", "runner.process.interrupted")]
    public void InterruptionCannotPublishOrLendSuccess()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "hold");
        using Process first = fixture.Start();
        AwaitReady(fixture: fixture);
        first.Kill(entireProcessTree: true);
        first.WaitForExit(milliseconds: 1000).ShouldBeTrue();
        Directory.GetFiles(path: Path.Combine(path1: fixture.Root, path2: ".phase1", path3: "runs"),
                searchPattern: "summary.json", searchOption: SearchOption.AllDirectories).ShouldBeEmpty();
        AssertPidsExited(fixture: fixture);
        fixture.Run().Output.ShouldContain(expected: "CHECK_LOCKED");
        File.Delete(path: Path.Combine(path1: fixture.Root, path2: ".phase1", path3: "check.lock"));
        using Process next = fixture.StartMode(mode: "pass");
        RunnerProcessFixture.Finish(child: next).Exit.ShouldBe(expected: 0);
        Directory.GetFiles(path: Path.Combine(path1: fixture.Root, path2: ".phase1", path3: "runs"),
                searchPattern: "summary.json", searchOption: SearchOption.AllDirectories).Length.ShouldBe(expected: 1);
        ProofProtocol.Record(id: "runner.process.interrupted", diagnostic: "NO_PARTIAL_PUBLICATION",
                location: "killed real coordinator and fresh rerun");
    }

    /// <summary>Runs the deliberate failing assertion and checks that its diagnostic remains visible.</summary>
    [Fact, Trait("Case", "runner.process.assertion")]
    public void RealAssertionFailureIsActionable()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "assertion");
        (int Exit, string Output) result = fixture.Run();
        result.Exit.ShouldBe(expected: 1);
        result.Output.ShouldContain(expected: "TEST_FAILED");
        result.Output.ShouldContain(expected: "INTENDED_ASSERTION_FAILURE");
        result.Output.ShouldContain(expected: "[log](");
        ProofProtocol.Record(id: "runner.process.assertion", diagnostic: "TEST_FAILED",
                location: "RunnerProcessFixture.Child");
    }

    // Only selected inside isolated negative-control workspaces, never as a passing obligation.
    /// <summary>Provides an explicitly selected failing assertion for the isolated failure-reporting control.</summary>
    [Fact(Explicit = true), Trait("Case", "fixture.assertion")]
    public void DeliberateAssertionFailure()
    {
        (2 + 2).ShouldBe(expected: 5, customMessage: "INTENDED_ASSERTION_FAILURE at RunnerContractTests.cs");
    }

    /// <summary>Rejects reconciliation when a required case has no result.</summary>
    [Fact]
    [Trait("Case", "runner.missing-result")]
    public void MissingRequiredResultCannotPass()
    {
        (int Exit, string Output) result = ProofProtocol.Node(
                script: ("const c=require('./tools/phase1/check.cjs'); try { c.re"
                        + "concile([{id:'required'}], [], {}); process.exitCode=0;"
                        + " } catch(e) { console.log(e.diagnostic); process.exitCo" + "de=1; }"));
        result.Exit.ShouldBe(expected: 1, customMessage: "missing required evidence must reject acceptance");
        result.Output.ShouldContain(expected: "RESULT_MISSING");
        ProofProtocol.Record(id: "runner.missing-result", diagnostic: "RESULT_MISSING", location: "required");
    }

    /// <summary>Publishes one fresh positive C# assertion result as the runner control.</summary>
    [Fact]
    [Trait("Case", "runner.assertion-pass")]
    public void RealAssertionPasses()
    {
        (2 + 2).ShouldBe(expected: 4);
        ProofProtocol.Record(id: "runner.assertion-pass");
    }

    /// <summary>Rejects repeated case results before set conversion could hide the duplicate.</summary>
    [Fact, Trait("Case", "runner.duplicate-result")]
    public void DuplicateResultIsRejectedBeforeSetConversion()
    {
        RejectMutation(id: "runner.duplicate-result", mutation: "results.push(results[0])",
                expected: "RESULT_DUPLICATE");
    }

    /// <summary>Prevents an unknown case identity from satisfying a required case.</summary>
    [Fact, Trait("Case", "runner.unknown-result")]
    public void UnknownResultCannotSatisfyAnotherObligation()
    {
        RejectMutation(id: "runner.unknown-result", mutation: "results[0].id='unknown'", expected: "RESULT_UNKNOWN");
    }

    /// <summary>Rejects a result whose nonce belongs to another invocation.</summary>
    [Fact, Trait("Case", "runner.wrong-nonce")]
    public void WrongNonceCannotReusePriorEvidence()
    {
        RejectMutation(id: "runner.wrong-nonce", mutation: "results[0].nonce='old'", expected: "NONCE_MISMATCH");
    }

    /// <summary>Rejects a result whose input fingerprint differs from the current inputs.</summary>
    [Fact, Trait("Case", "runner.changed-input")]
    public void ChangedInputCannotBorrowEvidence()
    {
        RejectMutation(id: "runner.changed-input", mutation: "results[0].input='changed'", expected: "INPUT_MISMATCH");
    }

    /// <summary>Rejects a result produced by a different tool identity.</summary>
    [Fact, Trait("Case", "runner.wrong-tool")]
    public void WrongToolCannotBorrowEvidence()
    {
        RejectMutation(id: "runner.wrong-tool", mutation: "results[0].tool='different'", expected: "TOOL_MISMATCH");
    }

    /// <summary>Rejects a result produced for a different platform identity.</summary>
    [Fact, Trait("Case", "runner.wrong-platform")]
    public void WrongPlatformCannotBorrowEvidence()
    {
        RejectMutation(id: "runner.wrong-platform", mutation: "results[0].platform='different'",
                expected: "PLATFORM_MISMATCH");
    }

    /// <summary>Prevents a skipped status from satisfying a required passing case.</summary>
    [Fact, Trait("Case", "runner.skipped-result")]
    public void SkippedResultCannotPass()
    {
        RejectMutation(id: "runner.skipped-result", mutation: "results[0].status='skipped'",
                expected: "RESULT_NOT_PASS");
    }

    /// <summary>Prevents a blocked status from satisfying a required passing case.</summary>
    [Fact, Trait("Case", "runner.blocked-result")]
    public void BlockedResultCannotPass()
    {
        RejectMutation(id: "runner.blocked-result", mutation: "results[0].status='blocked'",
                expected: "RESULT_NOT_PASS");
    }

    /// <summary>Requires a nonempty evidence list for an accepted result.</summary>
    [Fact, Trait("Case", "runner.empty-evidence")]
    public void EmptyEvidenceCannotPass()
    {
        RejectMutation(id: "runner.empty-evidence", mutation: "results[0].evidence=[]", expected: "EVIDENCE_MISSING");
    }

    /// <summary>Rejects an evidence path that traverses outside the current run directory.</summary>
    [Fact, Trait("Case", "runner.escaping-evidence")]
    public void EvidenceCannotEscapeRunDirectory()
    {
        RejectMutation(id: "runner.escaping-evidence", mutation: "results[0].evidence=['../outside.log']",
                expected: "PATH_ESCAPE");
    }

    /// <summary>Rejects restore failure as evidence for an intended subject diagnostic.</summary>
    [Fact, Trait("Case", "runner.unrelated-diagnostic")]
    public void InfrastructureFailureCannotStandForSubjectViolation()
    {
        RejectMutation(id: "runner.unrelated-diagnostic", mutation: "results[0].diagnostic='RESTORE_FAILED'",
                expected: "SUBJECT_DIAGNOSTIC");
    }

    /// <summary>Requires discovery and execution prerequisites before accepting negative evidence.</summary>
    [Fact, Trait("Case", "runner.missing-prerequisite")]
    public void NegativeEvidenceRequiresSuccessfulInfrastructure()
    {
        RejectMutation(id: "runner.missing-prerequisite", mutation: "results[0].prerequisites=['build']",
                expected: "SUBJECT_EVIDENCE");
    }

    /// <summary>Rejects unknown, incomplete, conflicting and empty CLI selections.</summary>
    [Fact, Trait("Case", "runner.selection")]
    public void WrongAndEmptySelectionsAreRejected()
    {
        string script = ("const c=require('./tools/phase1/check.cjs');const m=req"
                + "uire('./tools/phase1/required-cases.json');const rows=["
                + "['--group','unknown'],['--case','unknown'],['--case'],["
                + "'--unknown'],['--all','--case','runner.assertion-pass']"
                + "];for(const args of rows){let rejected=false;try{c.sele"
                + "ct(m,args)}catch{rejected=true}if(!rejected)process.exi"
                + "tCode=1}console.log('SELECTION_REJECTED')");
        (int Exit, string Output) result = ProofProtocol.Node(script: script);
        result.Exit.ShouldBe(expected: 0);
        result.Output.ShouldContain(expected: "SELECTION_REJECTED");
        ProofProtocol.Record(id: "runner.selection", diagnostic: "SELECTION_REJECTED", location: "CLI selection");
    }

    private static void ProcessRejects(string id, string mode, string diagnostic)
    {
        using var fixture = RunnerProcessFixture.Create(mode: mode);
        string source = Path.Combine(path1: fixture.Root, path2: "approved.cs");
        string document = Path.Combine(path1: fixture.Root, path2: "approved.md");
        string sourceHash = RunnerProcessFixture.Hash(file: source);
        string documentHash = RunnerProcessFixture.Hash(file: document);
        (int Exit, string Output) result = fixture.Run();
        result.Exit.ShouldBe(expected: 1);
        result.Output.ShouldContain(expected: diagnostic);
        if (StringComparer.Ordinal.Equals(x: mode, y: "timeout"))
        {
            File.ReadAllText(path: Path.Combine(path1: fixture.Root, path2: "descendant.ready")).ShouldBe(
                    expected: File.ReadAllText(path: Path.Combine(path1: fixture.Root, path2: "descendant.pid")));
        }

        (fixture.Summary()["status"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).GetValue<string>().ShouldBe(expected: "fail");
        if (!StringComparer.Ordinal.Equals(x: mode, y: "source"))
        {
            RunnerProcessFixture.Hash(file: source).ShouldBe(expected: sourceHash);
        }

        if (!StringComparer.Ordinal.Equals(x: mode, y: "document"))
        {
            RunnerProcessFixture.Hash(file: document).ShouldBe(expected: documentHash);
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "source"))
        {
            File.WriteAllText(path: source, contents: "// Approved isolated fixture source\n");
        }

        if (StringComparer.Ordinal.Equals(x: mode, y: "document"))
        {
            File.WriteAllText(path: document, contents: "Approved isolated fixture document.\n");
        }

        RunnerProcessFixture.Hash(file: source).ShouldBe(expected: sourceHash);
        RunnerProcessFixture.Hash(file: document).ShouldBe(expected: documentHash);
        AssertPidsExited(fixture: fixture);
        ProofProtocol.Record(id: id, diagnostic: diagnostic, location: "isolated normal entry point");
    }

    private static void AssertPidsExited(RunnerProcessFixture fixture)
    {
        foreach (string file in Directory.GetFiles(path: fixture.Root, searchPattern: "*.pid"))
        {
            int pid = int.Parse(s: File.ReadAllText(path: file), provider: System.Globalization.CultureInfo
                    .InvariantCulture);
            bool exited = false;
            try
            {
                using var process = Process.GetProcessById(processId: pid);
                exited = process.WaitForExit(milliseconds: 1000);
            }
            catch (ArgumentException)
            {
                exited = true;
            }

            exited.ShouldBeTrue(customMessage: "child PID " + pid + " must exit");
        }
    }

    private static void AwaitReady(RunnerProcessFixture fixture)
    {
        string ready = Path.Combine(path1: fixture.Root, path2: "child.pid");
        var watch = Stopwatch.StartNew();
        while (!File.Exists(path: ready) && watch.ElapsedMilliseconds < 2500)
        {
            Thread.Sleep(millisecondsTimeout: 10);
        }

        File.Exists(path: ready).ShouldBeTrue(
                customMessage: "real child must start before concurrency/interruption mutation");
    }

    private static void RejectMutation(string id, string mutation, string expected)
    {
        string script = ("const c=require('./tools/phase1/check.cjs'); const fs=r"
                + "equire('node:fs'); const os=require('node:os'); const p" + "ath=require('node:path'); ")
                + ("const directory=fs.mkdtempSync(path.join(process.env.TE"
                        + "MP||os.tmpdir(),'reconcile-')); fs.writeFileSync(path.j"
                        + "oin(directory,'subject.log'),'subject diagnostic'); ")
                + ("const selected=[{id:'required',expectedDiagnostic:'EXPE"
                        + "CTED'}]; const context={nonce:'fresh',input:'input',too"
                        + "l:'tool',platform:'platform',directory}; ")
                + ("let results=[{id:'required',nonce:'fresh',input:'input'"
                        + ",tool:'tool',platform:'platform',status:'pass',diagnost"
                        + "ic:'EXPECTED',location:'subject.cs:3',prerequisites:['b"
                        + "uild','discovery','execution'],evidence:['subject.log']" + "}]; ") + mutation
                + ("; try { c.reconcile(selected,results,context); console."
                        + "log('ACCEPTED'); } catch(e) { console.log(e.diagnostic)" + "; process.exitCode=1; }");
        (int Exit, string Output) result = ProofProtocol.Node(script: script);
        result.Exit.ShouldBe(expected: 1);
        result.Output.Trim().ShouldBe(expected: expected);
        ProofProtocol.Record(id: id, diagnostic: expected, location: "subject.cs:3");
    }
}
