using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Executes every required engine obligation independently with distinct native-process evidence.</summary>
public sealed class EngineCaseTests
{
    /// <summary>Requires every real engine surface in both complete scene generations.</summary>
    [Fact, Trait("Case", "ac-14.complete")]
    public void Run_CompleteIntegration_RequiresAllNativeSurfaces()
    {
        Check(id: "ac-14.complete");
    }

    /// <summary>Requires two complete native loads, uses and synchronous frees.</summary>
    [Fact, Trait("Case", "ac-15.complete")]
    public void Run_RepeatedLifetimes_RequiresCleanTeardown()
    {
        Check(id: "ac-15.complete");
    }

    /// <summary>Requires actual required-reference rejection before value use in a separate native process.</summary>
    [Fact, Trait("Case", "ac-16.complete")]
    public void Load_MissingReference_RejectsBeforeValueUse()
    {
        EngineRun run = CheckInvalid(mode: "missing-reference", member: "RequiredResource",
                scene: "InvalidFixture.tscn");
        ProofProtocol.Record(id: "ac-16.complete", diagnostic: "EXPORT_REQUIRED", location: run.Log,
                artifacts: [run.Log]);
    }

    /// <summary>Requires main-thread mutation, worker rejection and dead-generation rejection together.</summary>
    [Fact, Trait("Case", "ac-17.complete")]
    public void Run_WorkerBoundary_RequiresMainThreadMutation()
    {
        Check(id: "ac-17.complete");
    }

    /// <summary>Requires exactly one intended live handler in each freshly instantiated generation.</summary>
    [Fact, Trait("Case", "edge.rule-06.adjacency")]
    public void Emit_RepeatedGenerations_RequiresSingleHandler()
    {
        Check(id: "edge.rule-06.adjacency");
    }

    /// <summary>Distinguishes optional serialized null from a missing required nullable value.</summary>
    [Fact, Trait("Case", "edge.rule-06.empty")]
    public void Load_MissingNullableValue_RejectsRequiredEmpty()
    {
        var valid = EngineRun.Start();
        valid.Exit.ShouldBe(expected: 0, customMessage: valid.Output);
        EngineEvidencePolicy.Validate(output: valid.Output, nonce: valid.Nonce);
        EngineRun invalid = CheckInvalid(mode: "missing-value", member: "RequiredValue",
                scene: "ValidFixture.tscn");
        ProofProtocol.Record(id: "edge.rule-06.empty", diagnostic: "EXPORT_REQUIRED", location: invalid.Log,
                artifacts: [valid.Log, invalid.Log]);
    }

    /// <summary>Requires actual documented parent and child callback ordering in both cycles.</summary>
    [Fact, Trait("Case", "edge.rule-06.ordering")]
    public void Run_NativeCallbacks_RequiresDocumentedOrder()
    {
        Check(id: "edge.rule-06.ordering");
    }

    /// <summary>Requires distinct fresh scene identities without subscriptions inherited across cycles.</summary>
    [Fact, Trait("Case", "edge.rule-06.idempotency")]
    public void Run_NewSceneInstances_RejectsInheritedState()
    {
        Check(id: "edge.rule-06.idempotency");
    }

    /// <summary>Releases old queued work only after teardown and after the second generation is live.</summary>
    [Fact, Trait("Case", "edge.rule-06.concurrency")]
    public void Release_OldQueuedResult_PreservesBothGenerations()
    {
        Check(id: "edge.rule-06.concurrency");
    }

    private static void Check(string id)
    {
        var run = EngineRun.Start();
        run.Exit.ShouldBe(expected: 0, customMessage: run.Output);
        EngineEvidencePolicy.Validate(output: run.Output, nonce: run.Nonce);
        ProofProtocol.Record(id: id, diagnostic: "ENGINE_ASSERTIONS", location: run.Log, artifacts: [run.Log]);
    }

    private static EngineRun CheckInvalid(string mode, string member, string scene)
    {
        var run = EngineRun.Start(mode: mode);
        run.Exit.ShouldBe(expected: 2, customMessage: run.Output);
        run.Output.ShouldContain(expected: "EXPORT_REQUIRED:" + member);
        run.Output.ShouldContain(expected: "res://proofs/" + scene);
        run.Output.ShouldContain(expected: "\"uses\":0");
        run.Output.ShouldContain(expected: run.Nonce);
        run.Output.ShouldNotContain(expected: "PHASE1_SURFACE ");
        return run;
    }
}
