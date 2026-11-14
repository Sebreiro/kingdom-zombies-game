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
        ProofProtocol.Record(id: "engine.lifecycle", location: run.Log);
    }

    /// <summary>Requires serialized engine resources, checked nullable exports and failures before use.</summary>
    [Fact, Trait("Case", "engine.exports")]
    public void Load_SerializedExports_ValidatesBeforeUse()
    {
        var run = EngineRun.Start();
        run.Exit.ShouldBe(expected: 0, customMessage: run.Output);
        foreach (int cycle in new[] { 1, 2 })
        {
            foreach (string surface in new[] { "resource", "exports", "null-validation", "binding" })
            {
                run.Has(surface: surface, cycle: cycle).ShouldBeTrue(customMessage: surface + " cycle " + cycle);
            }
        }

        foreach (string mode in new[] { "missing-reference", "missing-value", "invalid-null" })
        {
            var invalid = EngineRun.Start(mode: mode);
            invalid.Exit.ShouldBe(expected: 2, customMessage: invalid.Output);
            invalid.Output.ShouldContain(expected: "EXPORT_REQUIRED");
            invalid.Output.ShouldContain(expected: "\"uses\":0");
            invalid.Output.ShouldContain(expected: mode);
            invalid.Output.ShouldContain(expected: mode is "missing-value" ? "RequiredValue" : "RequiredResource");
        }

        ProofProtocol.Record(id: "engine.exports", location: run.Log);
    }
}
