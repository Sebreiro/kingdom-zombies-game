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
    /// <summary>Required check reruns use fresh identities and preserve actual input bytes.</summary>
    [Fact, Trait("Case", "edge.rule-03.idempotency")]
    public void Reconcile_RequiredCheckReruns_PreservesInputsAndFreshness()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "pass");
        string file = Path.Combine(path1: fixture.Root, path2: "approved.cs");
        string before = RunnerProcessFixture.Hash(file: file);
        fixture.Run().Exit.ShouldBe(expected: 0);
        JsonNode first = fixture.Summary();
        fixture.Run().Exit.ShouldBe(expected: 0);
        JsonNode second = fixture.Summary();
        RequiredText(row: first, name: "nonce").ShouldNotBe(expected: RequiredText(row: second, name: "nonce"),
                comparer: StringComparer.Ordinal);
        RequiredText(row: first, name: "input").ShouldBe(expected: RequiredText(row: second, name: "input"));
        RequiredText(row: first, name: "status").ShouldBe(expected: "pass");
        RequiredText(row: second, name: "status").ShouldBe(expected: "pass");
        RunnerProcessFixture.Hash(file: file).ShouldBe(expected: before);
        AssertPidsExited(fixture: fixture);
        ProofProtocol.Record(id: "edge.rule-03.idempotency", diagnostic: "REPEATED_CHECK_INPUTS_IMMUTABLE",
                location: "two actual independent runner invocations with distinct nonces and unchanged source bytes");
    }

    /// <summary>Concurrent required checks reject mixed evidence and retain one actual completed result set.</summary>
    [Fact, Trait("Case", "edge.rule-03.concurrency")]
    public void Reconcile_ConcurrentRequiredChecks_RejectsMixedEvidence()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "concurrent");
        using Process first = fixture.Start();
        AwaitReady(fixture: fixture);
        (int Exit, string Output) competing = fixture.Run();
        competing.Exit.ShouldBe(expected: 1);
        competing.Output.ShouldContain(expected: "CHECK_LOCKED");
        RunnerProcessFixture.Finish(child: first).Exit.ShouldBe(expected: 0);
        JsonNode summary = fixture.Summary();
        RequiredText(row: summary, name: "status").ShouldBe(expected: "pass");
        (summary["results"] ?? throw new InvalidDataException(message: "RESULTS_REQUIRED"))
                .AsArray().Count.ShouldBe(expected: 1);
        Directory.GetFiles(path: Path.Combine(path1: fixture.Root, path2: ".phase1/runs"),
                searchPattern: "summary.json", searchOption: SearchOption.AllDirectories).Length.ShouldBe(expected: 1);
        AssertPidsExited(fixture: fixture);
        ProofProtocol.Record(id: "edge.rule-03.concurrency", diagnostic: "CONCURRENT_CHECK_EVIDENCE_ISOLATED",
                location: "actual competitor rejected; one completed subject/result set and no live children");
    }

    /// <summary>Requires TOOL_MISSING when the isolated runner executable does not exist.</summary>
    [Fact, Trait("Case", "runner.process.missing-executable")]
    public void Reconcile_RealMissingExecutable_Rejected()
    {
        ProcessRejects(id: "runner.process.missing-executable", mode: "missing-executable", diagnostic: "TOOL_MISSING");
    }

    /// <summary>Requires DISCOVERY_EMPTY when the requested method discovers no test.</summary>
    [Fact, Trait("Case", "runner.process.zero-discovery")]
    public void Reconcile_RealZeroDiscovery_Rejected()
    {
        ProcessRejects(id: "runner.process.zero-discovery", mode: "zero", diagnostic: "DISCOVERY_EMPTY");
    }

    /// <summary>Requires DISCOVERY_WRONG when a real child selects a different case.</summary>
    [Fact, Trait("Case", "runner.process.wrong-filter")]
    public void Reconcile_RealWrongFilter_Rejected()
    {
        ProcessRejects(id: "runner.process.wrong-filter", mode: "filter", diagnostic: "DISCOVERY_WRONG");
    }

    /// <summary>Requires TEST_SKIPPED for the intentionally skipped isolated control.</summary>
    [Fact, Trait("Case", "runner.process.skip")]
    public void Reconcile_RealSkip_Rejected()
    {
        ProcessRejects(id: "runner.process.skip", mode: "skip", diagnostic: "TEST_SKIPPED");
    }

    /// <summary>Requires REPORT_MISSING when a real child exits before producing a report.</summary>
    [Fact, Trait("Case", "runner.process.crash")]
    public void Reconcile_RealCrash_Rejected()
    {
        ProcessRejects(id: "runner.process.crash", mode: "crash", diagnostic: "REPORT_MISSING");
    }

    /// <summary>Requires a child timeout and verifies cleanup of its running descendant.</summary>
    [Fact, Trait("Case", "runner.process.timeout")]
    public void Run_TimedOutChild_KillsDescendants()
    {
        ProcessRejects(id: "runner.process.timeout", mode: "timeout", diagnostic: "CHILD_TIMEOUT");
    }

    /// <summary>Rejects a real child result whose required subject log was deleted.</summary>
    [Fact, Trait("Case", "runner.process.deleted-log")]
    public void Reconcile_RealDeletedLog_Rejected()
    {
        ProcessRejects(id: "runner.process.deleted-log", mode: "deleted-log", diagnostic: "EVIDENCE_MISSING");
    }

    /// <summary>Rejects a real child execution whose proof result was deleted.</summary>
    [Fact, Trait("Case", "runner.process.deleted-result")]
    public void Reconcile_RealDeletedResult_Rejected()
    {
        ProcessRejects(id: "runner.process.deleted-result", mode: "deleted-result", diagnostic: "RESULT_MISSING");
    }

    /// <summary>Rejects two real proof files claiming the same required case.</summary>
    [Fact, Trait("Case", "runner.process.duplicate")]
    public void Reconcile_RealDuplicateResult_Rejected()
    {
        ProcessRejects(id: "runner.process.duplicate", mode: "duplicate", diagnostic: "RESULT_DUPLICATE");
    }

    /// <summary>Rejects a real proof file carrying another invocation nonce.</summary>
    [Fact, Trait("Case", "runner.process.nonce")]
    public void Reconcile_RealWrongNonce_Rejected()
    {
        ProcessRejects(id: "runner.process.nonce", mode: "nonce", diagnostic: "NONCE_MISMATCH");
    }

    /// <summary>Prevents publication after a child modifies an approved source input.</summary>
    [Fact, Trait("Case", "runner.process.source")]
    public void Publish_ChangedSource_RejectsStaleEvidence()
    {
        ProcessRejects(id: "runner.process.source", mode: "source", diagnostic: "SOURCE_CHANGED");
    }

    /// <summary>Prevents publication after a child modifies an approved document input.</summary>
    [Fact, Trait("Case", "runner.process.document")]
    public void Publish_ChangedDocument_RejectsStaleEvidence()
    {
        ProcessRejects(id: "runner.process.document", mode: "document", diagnostic: "SOURCE_CHANGED");
    }

    /// <summary>Checks repeated unchanged runs produce identical normalized verdicts.</summary>
    [Fact, Trait("Case", "runner.process.repeat")]
    public void Reconcile_UnchangedRuns_HaveIdenticalNormalizedVerdicts()
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
            "deferred",
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
        VerifyCapacityExecution();
        ProofProtocol.Record(id: "runner.process.repeat", diagnostic: "NORMALIZED_EQUAL",
                location: "two complete real C# runs");
    }

    /// <summary>Requires fresh evidence instead of accepting a previous run result.</summary>
    [Fact, Trait("Case", "runner.process.stale")]
    public void Reconcile_NewRun_RejectsBorrowedPriorArtifacts()
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
    public void Run_ConcurrentFullInvocation_ReportsAlreadyRunning()
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
    public void Publish_InterruptedRun_RejectsSuccessAndBorrowedEvidence()
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
    public void Reconcile_RealAssertionFailure_ReportsActionableFailure()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "assertion");
        (int Exit, string Output) result = fixture.Run();
        result.Exit.ShouldBe(expected: 1);
        result.Output.ShouldContain(expected: "TEST_FAILED");
        result.Output.ShouldContain(expected: "INTENDED_ASSERTION_FAILURE");
        result.Output.ShouldContain(expected: "[log](");
        ProofProtocol.Record(id: "runner.process.assertion", diagnostic: "TEST_FAILED",
                location: "RunnerProcessFixture.Run_ExplicitChild_EmitsSelectedResult");
    }

    // Only selected inside isolated negative-control workspaces, never as a passing obligation.
    /// <summary>Provides an explicitly selected failing assertion for the isolated failure-reporting control.</summary>
    [Fact(Explicit = true), Trait("Case", "fixture.assertion")]
    public void Assert_DeliberateFailure_ReportsExpectedAssertion()
    {
        (2 + 2).ShouldBe(expected: 5, customMessage: "INTENDED_ASSERTION_FAILURE at RunnerContractTests.cs");
    }

    /// <summary>Rejects reconciliation when a required case has no result.</summary>
    [Fact]
    [Trait("Case", "runner.missing-result")]
    public void Reconcile_MissingRequiredResult_Rejected()
    {
        (int Exit, string Output) result = ProofProtocol.Node(
                script: ("const c=require('./src/godot-game/tools/verification/check.cjs'); try { c.re"
                        + "concile([{id:'required'}], [], {}); process.exitCode=0;"
                        + " } catch(e) { console.log(e.diagnostic); process.exitCo" + "de=1; }"));
        result.Exit.ShouldBe(expected: 1, customMessage: "missing required evidence must reject acceptance");
        result.Output.ShouldContain(expected: "RESULT_MISSING");
        ProofProtocol.Record(id: "runner.missing-result", diagnostic: "RESULT_MISSING", location: "required");
    }

    /// <summary>Publishes one fresh positive C# assertion result as the runner control.</summary>
    [Fact]
    [Trait("Case", "runner.assertion-pass")]
    public void Assert_RealControl_Passes()
    {
        (2 + 2).ShouldBe(expected: 4);
        ProofProtocol.Record(id: "runner.assertion-pass");
    }

    /// <summary>Rejects repeated case results before set conversion could hide the duplicate.</summary>
    [Fact, Trait("Case", "runner.duplicate-result")]
    public void Reconcile_DuplicateResult_RejectedBeforeSetConversion()
    {
        RejectMutation(id: "runner.duplicate-result", mutation: "results.push(results[0])",
                expected: "RESULT_DUPLICATE");
    }

    /// <summary>Prevents an unknown case identity from satisfying a required case.</summary>
    [Fact, Trait("Case", "runner.unknown-result")]
    public void Reconcile_UnknownResult_RejectsSubstitutedObligation()
    {
        RejectMutation(id: "runner.unknown-result", mutation: "results[0].id='unknown'", expected: "RESULT_UNKNOWN");
    }

    /// <summary>Rejects a result whose nonce belongs to another invocation.</summary>
    [Fact, Trait("Case", "runner.wrong-nonce")]
    public void Reconcile_WrongNonce_RejectsReusedEvidence()
    {
        RejectMutation(id: "runner.wrong-nonce", mutation: "results[0].nonce='old'", expected: "NONCE_MISMATCH");
    }

    /// <summary>Rejects a result whose input fingerprint differs from the current inputs.</summary>
    [Fact, Trait("Case", "runner.changed-input")]
    public void Reconcile_ChangedInput_RejectsBorrowedEvidence()
    {
        RejectMutation(id: "runner.changed-input", mutation: "results[0].input='changed'", expected: "INPUT_MISMATCH");
    }

    /// <summary>Rejects a result produced by a different tool identity.</summary>
    [Fact, Trait("Case", "runner.wrong-tool")]
    public void Reconcile_WrongTool_RejectsBorrowedEvidence()
    {
        RejectMutation(id: "runner.wrong-tool", mutation: "results[0].tool='different'", expected: "TOOL_MISMATCH");
    }

    /// <summary>Rejects a result produced for a different platform identity.</summary>
    [Fact, Trait("Case", "runner.wrong-platform")]
    public void Reconcile_WrongPlatform_RejectsBorrowedEvidence()
    {
        RejectMutation(id: "runner.wrong-platform", mutation: "results[0].platform='different'",
                expected: "PLATFORM_MISMATCH");
    }

    /// <summary>Prevents a skipped status from satisfying a required passing case.</summary>
    [Fact, Trait("Case", "runner.skipped-result")]
    public void Reconcile_SkippedResult_Rejected()
    {
        RejectMutation(id: "runner.skipped-result", mutation: "results[0].status='skipped'",
                expected: "RESULT_NOT_PASS");
    }

    /// <summary>Prevents a blocked status from satisfying a required passing case.</summary>
    [Fact, Trait("Case", "runner.blocked-result")]
    public void Reconcile_BlockedResult_Rejected()
    {
        RejectMutation(id: "runner.blocked-result", mutation: "results[0].status='blocked'",
                expected: "RESULT_NOT_PASS");
    }

    /// <summary>Requires a nonempty evidence list for an accepted result.</summary>
    [Fact, Trait("Case", "runner.empty-evidence")]
    public void Reconcile_EmptyEvidence_Rejected()
    {
        RejectMutation(id: "runner.empty-evidence", mutation: "results[0].evidence=[]", expected: "EVIDENCE_MISSING");
    }

    /// <summary>Rejects an evidence path that traverses outside the current run directory.</summary>
    [Fact, Trait("Case", "runner.escaping-evidence")]
    public void Reconcile_EscapingEvidence_RejectsOutsidePath()
    {
        RejectMutation(id: "runner.escaping-evidence", mutation: "results[0].evidence=['../outside.log']",
                expected: "PATH_ESCAPE");
    }

    /// <summary>Rejects restore failure as evidence for an intended subject diagnostic.</summary>
    [Fact, Trait("Case", "runner.unrelated-diagnostic")]
    public void Reconcile_InfrastructureFailure_RejectsSubjectCredit()
    {
        RejectMutation(id: "runner.unrelated-diagnostic", mutation: "results[0].diagnostic='RESTORE_FAILED'",
                expected: "SUBJECT_DIAGNOSTIC");
    }

    /// <summary>Requires discovery and execution prerequisites before accepting negative evidence.</summary>
    [Fact, Trait("Case", "runner.missing-prerequisite")]
    public void Reconcile_NegativeEvidence_RequiresSuccessfulInfrastructure()
    {
        RejectMutation(id: "runner.missing-prerequisite", mutation: "results[0].prerequisites=['build']",
                expected: "SUBJECT_EVIDENCE");
    }

    /// <summary>Rejects unknown, incomplete, conflicting and empty CLI selections.</summary>
    [Fact, Trait("Case", "runner.selection")]
    public void Reconcile_WrongAndEmptySelections_Rejected()
    {
        string script = ("const c=require('./src/godot-game/tools/verification/check.cjs');const m=req"
                + "uire('./src/godot-game/tools/verification/required-cases.json');const rows=["
                + "['--group','unknown'],['--case','unknown'],['--case'],["
                + "'--unknown'],['--all','--case','runner.assertion-pass']"
                + ",['--phase'],['--phase','0'],['--phase','-1'],['--phase','1.5'],"
                + "['--phase','01'],['--phase','1e0'],['--phase','NaN'],['--phase','Infinity'],"
                + "['--phase','9007199254740992'],['--phase','1','--all'],"
                + "['--phase','1','--group','docs'],['--case','runner.selection','--phase','1'],"
                + "['--phase','1','--phase','5'],['--list','--list']"
                + "];for(const args of rows){let rejected=false;try{c.sele"
                + "ct(m,args)}catch{rejected=true}if(!rejected)process.exi"
                + "tCode=1}console.log('SELECTION_REJECTED')");
        (int Exit, string Output) result = ProofProtocol.Node(script: script);
        result.Exit.ShouldBe(expected: 0);
        result.Output.ShouldContain(expected: "SELECTION_REJECTED");
        VerifyCapacityDeclarations();
        ProofProtocol.Record(id: "runner.selection", diagnostic: "SELECTION_REJECTED", location: "CLI selection");
    }

    /// <summary>Pins the Phase1 baseline and exact Phase4 export and Phase5 certification partitions.</summary>
    [Fact, Trait("Case", "runner.phase-membership")]
    public void Select_DeclaredPhases_RetainsImplementedCases()
    {
        string script = "const c=require('./src/godot-game/tools/verification/"
                + "check.cjs'),a=require('node:assert/strict');"
                + "const m=require('./src/godot-game/tools/verification/"
                + "required-cases.json'),s=c.select(m,['--phase','1']);"
                + "const ids=['ac-02.complete','ac-06.complete','ac-07.complete','ac-09.complete',"
                + "'R-49.positive','R-49.negative','R-82.positive','R-82.negative'].sort();"
                + "const exports=['rules.export-layout-origin','rules.export-layout-boundary'].sort();"
                + "const deferred=[...ids,...exports].sort();a.equal(m.cases.length,686);"
                + "a.equal(s.selected.length,676);a.deepEqual(s.deferred.map(r=>r.id).sort(),deferred);"
                + "a.deepEqual(s.deferred.filter(r=>ids.includes(r.id)).map(r=>"
                + "[r.id,r.acceptancePhase,r.owner]).sort(),ids.map(id=>[id,5,'Phase 5']));"
                + "a.deepEqual(s.deferred.filter(r=>exports.includes(r.id)).map(r=>"
                + "[r.id,r.acceptancePhase,r.owner]).sort(),exports.map(id=>[id,4,'04-34']));"
                + "a.deepEqual(s.selected,m.cases.filter(r=>!deferred.includes(r.id)));"
                + "a.ok(s.selected.every(r=>(r.acceptancePhase??1)===1&&r.implementation==='csharp'));"
                + "a.deepEqual(m.cases.filter(r=>r.implementation==='csharp'&&!s.selected.includes(r))"
                + ".map(r=>r.id).sort(),exports);"
                + "a.ok(m.cases.filter(r=>r.implementation==='csharp'&&!exports.includes(r.id))"
                + ".every(r=>s.selected.includes(r)));"
                + "a.deepEqual(c.select(m,['--phase','4']).selected,m.cases.filter(r=>!ids.includes(r.id)));"
                + "a.deepEqual(c.select(m,['--all']).selected,m.cases);"
                + "a.deepEqual(c.select(m,['--phase','5']).selected,m.cases);"
                + "a.deepEqual(c.select(m,['--list']).selected,m.cases);"
                + "a.deepEqual(c.select(m,['--phase','1','--list']).selected,s.selected);"
                + "a.equal(c.select(m,['--case','R-49.positive']).selected[0].id,'R-49.positive');"
                + "a.ok(c.select(m,['--group','rules']).selected.some(r=>r.id==='R-49.positive'));"
                + "console.log('PHASE_MEMBERSHIP_EXACT')";
        ProofProtocol.Node(script: script).Exit.ShouldBe(expected: 0);
        ProofProtocol.Record(id: "runner.phase-membership", diagnostic: "PHASE_MEMBERSHIP_EXACT",
                location: "independent declarations: 676 Phase1, two Phase4, eight Phase5; exhaustive686");
    }

    /// <summary>Rejects malformed declarations even when their case would be outside the requested phase.</summary>
    [Fact, Trait("Case", "runner.phase-declarations")]
    public void Select_InvalidDeclaredPhase_RejectsUnselectedCorruption()
    {
        string script = "const c=require('./src/godot-game/tools/verification/"
                + "check.cjs'),a=require('node:assert/strict');"
                + "const m=require('./src/godot-game/tools/verification/required-cases.json'),row=m.cases[0];"
                + "for(const value of [null,0,-1,1.5,'5',true,{},[],9007199254740992]){"
                + "const bad={...m,cases:[row,{...row,id:'fixture.bad',acceptancePhase:value}]};"
                + "a.throws(()=>c.select(bad,['--phase','1']),e=>e.diagnostic==='MANIFEST_PHASE')}"
                + "for(const bad of [null,[],{}, {...row,group:'unknown'},{...row,owner:''},"
                + "{...row,id:'fixture.bad',owner:5},{...row,id:'fixture.bad',evidence:'log'},"
                + "{...row,id:'fixture.bad',test:null},{...row,id:'fixture.bad',implementation:'unknown'}]){"
                + "a.throws(()=>c.select({...m,cases:[row,bad]},['--phase','1']))}"
                + "const pending={...row,id:'fixture.pending',implementation:'pending',acceptancePhase:3};"
                + "a.equal(c.select({...m,cases:[row,pending]},['--phase','2']).selected.length,1);"
                + "a.equal(c.select({...m,cases:[row,pending]},['--phase','3']).selected.length,2);"
                + "console.log('MANIFEST_PHASE_REJECTED')";
        ProofProtocol.Node(script: script).Exit.ShouldBe(expected: 0);
        ProofProtocol.Record(id: "runner.phase-declarations", diagnostic: "MANIFEST_PHASE_REJECTED",
                location: "actual selector: invalid/null/string/unsafe phases and corrupt unselected declarations");
    }

    /// <summary>Publishes deferred owners separately while exhaustive mode still fails pending work.</summary>
    [Fact, Trait("Case", "runner.phase-summary")]
    public void Run_DeclaredPhase_PublishesSeparateDeferredOwners()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "pass");
        AddPendingDeclaration(fixture: fixture, phase: 5);
        fixture.Run(args: ["--phase", "1"]).Exit.ShouldBe(expected: 0);
        JsonNode summary = fixture.Summary();
        summary["selected"]?.AsArray().Count.ShouldBe(expected: 1);
        summary["results"]?.AsArray().Count.ShouldBe(expected: 1);
        summary["failures"]?.AsArray().Count.ShouldBe(expected: 0);
        summary["deferred"]?.AsArray().Count.ShouldBe(expected: 1);
        summary["deferred"]?[0]?["id"]?.GetValue<string>().ShouldBe(expected: "fixture.pending");
        summary["deferred"]?[0]?["owner"]?.GetValue<string>().ShouldBe(expected: "Phase 5");
        summary["deferred"]?[0]?["acceptancePhase"]?.GetValue<int>().ShouldBe(expected: 5);
        string[][] selections = [["--all"], ["--phase", "5"]];
        foreach (string[] args in selections)
        {
            fixture.Run(args: args).Exit.ShouldBe(expected: 1);
            summary = fixture.Summary();
            summary["selected"]?.AsArray().Count.ShouldBe(expected: 2);
            summary["deferred"]?.AsArray().Count.ShouldBe(expected: 0);
            summary["failures"]?[0]?["diagnostic"]?.GetValue<string>().ShouldBe(
                    expected: "IMPLEMENTATION_PENDING");
        }

        ProofProtocol.Record(id: "runner.phase-summary", diagnostic: "PHASE_DEFERRED_SEPARATE",
                location: "actual CLI: phase 1 passes 1/1; exhaustive/phase 5 fail pending 1/2");
    }

    /// <summary>An unresolved default-phase declaration and deleted selected evidence remain actual failures.</summary>
    [Fact, Trait("Case", "runner.phase-required")]
    public void Run_RequiredPhase_RejectsUnresolvedAndMissingEvidence()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "pass");
        AddPendingDeclaration(fixture: fixture, phase: null);
        (int Exit, string Output) result = fixture.Run(args: ["--phase", "1"]);
        result.Exit.ShouldBe(expected: 1);
        result.Output.ShouldContain(expected: "IMPLEMENTATION_PENDING");
        fixture.Summary()["selected"]?.AsArray().Count.ShouldBe(expected: 2);
        fixture.Summary()["deferred"]?.AsArray().Count.ShouldBe(expected: 0);
        using var missing = RunnerProcessFixture.Create(mode: "deleted-result");
        result = missing.Run(args: ["--phase", "1"]);
        result.Exit.ShouldBe(expected: 1);
        result.Output.ShouldContain(expected: "RESULT_MISSING");
        ProofProtocol.Record(id: "runner.phase-required", diagnostic: "PHASE_REQUIRED_FAILURE",
                location: "actual CLI: unresolved default Phase 1 and deleted selected proof reject acceptance");
    }

    private static void VerifyCapacityDeclarations()
    {
        string script = "const c=require('./tools/phase1/check.cjs'),a=require('node:assert/strict');"
                + "const m=require('./tools/phase1/required-cases.json'),row=m.cases[0];"
                + "a.deepEqual(m.cases.filter(r=>Object.hasOwn(r,'childTimeoutMs'))"
                + ".map(r=>[r.id,r.childTimeoutMs]).sort(),[['R-67.positive',60000],['ac-08.complete',75000],"
                + "['engine.generated-boundary',120000],['rules.godot-companion-negative',60000]]);"
                + "for(const [id,argument,outerMs] of [['ac-08.complete','75s',77000],"
                + "['R-67.positive','60s',62000],['R-67.negative','45s',45000],"
                + "['engine.generated-boundary','120s',122000],"
                + "['rules.godot-companion-negative','60s',62000],"
                + "['rules.godot-companion-positive','45s',45000]]){"
                + "a.deepEqual(c.caseTimeout(m.cases.find(r=>r.id===id),m),{argument,outerMs})}"
                + "a.deepEqual(c.caseTimeout(row,{}),{argument:'45s',outerMs:45000});"
                + "a.deepEqual(c.caseTimeout(row,{childTimeoutMs:1800}),{argument:'45s',outerMs:1800});"
                + "for(const n of [1,999,1000,1001,120000]){const r={...row,childTimeoutMs:n};"
                + "a.equal(c.select({...m,cases:[r]},['--all']).selected[0],r);"
                + "const t=c.caseTimeout(r,m),s=Math.ceil(n/1000);"
                + "a.equal(t.argument,s+'s');a.equal(t.outerMs,s*1000+2000);"
                + "a.deepEqual(c.caseTimeout(row,{}),{argument:'45s',outerMs:45000})}"
                + "for(const n of [null,0,-1,1.5,'120000',true,{},[],NaN,Infinity,120001,9007199254740992]){"
                + "const bad={...row,id:'fixture.bad',acceptancePhase:5,childTimeoutMs:n};"
                + "a.throws(()=>c.select({...m,cases:[row,bad]},['--phase','1']),"
                + "e=>e.diagnostic==='MANIFEST_TIMEOUT')}console.log('CASE_CAPACITY_VALIDATED')";
        (int Exit, string Output) result = ProofProtocol.Node(script: script);
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        result.Output.ShouldContain(expected: "CASE_CAPACITY_VALIDATED");
        using var fixture = RunnerProcessFixture.Create(mode: "pass");
        SetCaseCapacity(fixture: fixture, capacity: 0);
        result = fixture.Run();
        result.Exit.ShouldBe(expected: 1);
        result.Output.ShouldContain(expected: "MANIFEST_TIMEOUT");
        File.Exists(path: Path.Combine(path1: fixture.Root, path2: "child.pid")).ShouldBeFalse();
        Directory.Exists(path: Path.Combine(path1: fixture.Root, path2: ".phase1/runs")).ShouldBeFalse();
    }

    private static void VerifyCapacityExecution()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "pass");
        (int? Capacity, string Argument, int Outer)[] cases =
        [
            (120000, "120s", 122000), (75000, "75s", 77000), (60000, "60s", 62000),
            (null, "45s", 1800), (1, "1s", 3000),
        ];
        foreach ((int? Capacity, string Argument, int Outer) item in cases)
        {
            SetCaseCapacity(fixture: fixture, capacity: item.Capacity);
            (int Exit, string Output) result = fixture.Run();
            result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
            JsonNode command = fixture.Summary()["commands"]?.AsArray().Single(predicate: row =>
                    StringComparer.Ordinal.Equals(x: row?["log"]?.GetValue<string>(), y: "test.process.log"))
                    ?? throw new InvalidDataException(message: "CASE_COMMAND_REQUIRED");
            JsonArray args = command["args"]?.AsArray()
                    ?? throw new InvalidDataException(message: "CASE_ARGUMENTS_REQUIRED");
            args[^1]?.GetValue<string>().ShouldBe(expected: item.Argument);
            command["timeoutMs"]?.GetValue<int>().ShouldBe(expected: item.Outer);
            AssertPidsExited(fixture: fixture);
        }
    }

    private static void SetCaseCapacity(RunnerProcessFixture fixture, int? capacity)
    {
        string path = Path.Combine(path1: fixture.Root, path2: "tools/phase1/required-cases.json");
        JsonObject manifest = RegisterValidator.Parse(bytes: File.ReadAllBytes(path: path));
        JsonObject row = manifest["cases"]?.AsArray()[0]?.AsObject()
                ?? throw new InvalidDataException(message: "CASE_CAPACITY_ROW_REQUIRED");
        if (capacity is { } value)
        {
            row["childTimeoutMs"] = value;
        }
        else
        {
            row.Remove(propertyName: "childTimeoutMs");
        }

        File.WriteAllText(path: path, contents: manifest.ToJsonString());
    }

    private static void AddPendingDeclaration(RunnerProcessFixture fixture, int? phase)
    {
        string path = Path.Combine(path1: fixture.Root, path2: "src/godot-game/tools/verification/required-cases.json");
        JsonObject manifest = RegisterValidator.Parse(bytes: File.ReadAllBytes(path: path));
        JsonArray cases = manifest["cases"]?.AsArray() ?? throw new InvalidDataException(message: "CASES_REQUIRED");
        JsonNode pending = cases[0]?.DeepClone() ?? throw new InvalidDataException(message: "CASE_REQUIRED");
        pending["id"] = "fixture.pending";
        pending["implementation"] = "pending";
        pending["owner"] = "Phase 5";
        if (phase is not null)
        {
            pending["acceptancePhase"] = phase.Value;
        }

        cases.Add(item: pending);
        File.WriteAllText(path: path, contents: manifest.ToJsonString());
    }

    private static string RequiredText(JsonNode row, string name)
    {
        return row[name]?.GetValue<string>()
                ?? throw new InvalidDataException(message: "RESULT_FIELD_REQUIRED " + name);
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
        string script = ("const c=require('./src/godot-game/tools/verification/check.cjs'); const fs=r"
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
