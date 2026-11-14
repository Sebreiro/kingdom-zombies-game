using System;
using System.Linq;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires assertions from real native-engine scene instances.</summary>
public sealed class EngineEvidenceTests
{
    /// <summary>Runs two distinct native lifetimes and requires every completed cycle surface.</summary>
    [Fact, Trait("Case", "engine.lifecycle")]
    public void Run_TwoNativeCycles_RequiresLifecycleSurfaces()
    {
        var run = EngineRun.Start();
        run.Exit.ShouldBe(expected: 0, customMessage: run.Output);
        EngineEvidencePolicy.Validate(output: run.Output, nonce: run.Nonce);
        foreach (int cycle in new[] { 1, 2 })
        {
            foreach (string surface in new[] { "construction", "callbacks", "process", "signals", "teardown" })
            {
                run.Has(surface: surface, cycle: cycle).ShouldBeTrue(customMessage: surface + " cycle " + cycle);
            }
        }

        run.Has(surface: "late-work", cycle: 1).ShouldBeTrue();
        run.Has(surface: "main-thread", cycle: 2).ShouldBeTrue();
        run.Has(surface: "worker-rejected", cycle: 1).ShouldBeTrue();
        ProofProtocol.Record(id: "engine.lifecycle", location: run.Log, artifacts: [run.Log]);
    }

    /// <summary>Requires serialized engine resources, checked nullable exports and failures before use.</summary>
    [Fact, Trait("Case", "engine.exports")]
    public void Load_SerializedExports_ValidatesBeforeUse()
    {
        var run = EngineRun.Start();
        run.Exit.ShouldBe(expected: 0, customMessage: run.Output);
        EngineEvidencePolicy.Validate(output: run.Output, nonce: run.Nonce);
        foreach (int cycle in new[] { 1, 2 })
        {
            foreach (string surface in new[] { "resource", "exports", "null-validation", "binding" })
            {
                run.Has(surface: surface, cycle: cycle).ShouldBeTrue(customMessage: surface + " cycle " + cycle);
            }
        }

        var logs = new System.Collections.Generic.List<string> { run.Log };
        foreach (string mode in new[] { "missing-reference", "missing-value", "invalid-null" })
        {
            var invalid = EngineRun.Start(mode: mode);
            invalid.Exit.ShouldBe(expected: 2, customMessage: invalid.Output);
            invalid.Output.ShouldContain(expected: "EXPORT_REQUIRED");
            invalid.Output.ShouldContain(expected: "\"uses\":0");
            invalid.Output.ShouldContain(expected: mode);
            invalid.Output.ShouldContain(expected: mode is "missing-value" ? "RequiredValue" : "RequiredResource");
            invalid.Output.ShouldContain(expected: mode is "missing-reference"
                    ? "res://proofs/InvalidFixture.tscn" : "res://proofs/ValidFixture.tscn");
            invalid.Output.ShouldContain(expected: invalid.Nonce);
            logs.Add(item: invalid.Log);
        }

        ProofProtocol.Record(id: "engine.exports", location: run.Log, artifacts: logs);
    }

    /// <summary>Removes each actual surface in turn and rejects zero-exit, stale and duplicate evidence.</summary>
    [Fact, Trait("Case", "engine.evidence-integrity")]
    public void Check_NativeEvidence_RejectsMissingStaleAndDuplicate()
    {
        var run = EngineRun.Start();
        run.Exit.ShouldBe(expected: 0, customMessage: run.Output);
        EngineEvidencePolicy.Validate(output: run.Output, nonce: run.Nonce);
        string[] lines = [..run.Output.Split(separator: '\n').Where(predicate: line => line.StartsWith(
                value: "PHASE1_SURFACE ", comparisonType: StringComparison.Ordinal))];
        lines.Length.ShouldBe(expected: 21);
        foreach (string removed in lines)
        {
            string output = string.Join(separator: "\n", values: lines.Where(predicate: line =>
                    !StringComparer.Ordinal.Equals(x: line, y: removed)));
            Should.Throw<InvalidOperationException>(actual: () => EngineEvidencePolicy.Validate(output: output,
                    nonce: run.Nonce)).Message.ShouldContain(expected: "ENGINE_SURFACE_MISSING");
        }

        Should.Throw<InvalidOperationException>(actual: () => EngineEvidencePolicy.Validate(output: "",
                nonce: run.Nonce)).Message.ShouldContain(expected: "ENGINE_SURFACE_MISSING");
        Should.Throw<InvalidOperationException>(actual: () => EngineEvidencePolicy.Validate(output: run.Output,
                nonce: Guid.NewGuid().ToString(format: "D"))).Message.ShouldContain(expected: "ENGINE_SURFACE_INVALID");
        Should.Throw<InvalidOperationException>(actual: () => EngineEvidencePolicy.Validate(
                output: run.Output + "\n" + lines[0], nonce: run.Nonce)).Message.ShouldContain(
                        expected: "ENGINE_SURFACE_INVALID");
        ProofProtocol.Record(id: "engine.evidence-integrity", diagnostic: "ENGINE_EVIDENCE_EXACT", location: run.Log,
                artifacts: [run.Log]);
    }
}
