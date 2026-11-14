using System;
using System.IO;
using System.Linq;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires semantic rejection of direct engine calls in an undeclared handwritten adapter.</summary>
public sealed class EngineBoundaryTests
{
    /// <summary>Builds clean controls before rejecting the exact actual handwritten engine-call subject.</summary>
    [Fact, Trait("Case", "engine.gateway-bypass")]
    public void Check_GatewayBypass_RejectsExactSubject()
    {
        var fixture = new BuildFixture(game: true);
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        const string Source = "namespace KingdomZombies.Proofs;\n\n"
                + "internal static class Subject\n{\n    internal static void Run(Godot.Node node)\n    {\n"
                + "        node.QueueFree();\n    }\n}\n";
        fixture.Write(source: Source);
        (int Exit, string Output) actual = fixture.Build();
        actual.Exit.ShouldBe(expected: 0, customMessage: actual.Output);
        TypeBoundaryScanner.EngineAccess(source: BuildFixture.CleanSource, path: "Subject.cs").ShouldBeEmpty();
        TypeBoundaryScanner.EngineAccess(source: Source, path: "Subject.cs").ShouldContain(
                expected: "ENGINE_ACCESS Subject.cs(7,9) Godot.Node.QueueFree()", comparer: StringComparer.Ordinal);
        ProofProtocol.Record(id: "engine.gateway-bypass", diagnostic: "ENGINE_ACCESS",
                location: fixture.DirectoryPath + "/src/godot-game/Subject.cs(7,9)");
    }

    /// <summary>Checks real sources and rejects aliased, spoofed-adapter, late-guard and worker-lambda calls.</summary>
    [Fact, Trait("Case", "engine.source-policy")]
    public void Check_EngineSource_RequiresDeclaredGuardedAdapters()
    {
        EngineAccessPolicy.Repository().ShouldBeEmpty();
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        const string Prefix = "using Alias = Godot.GD; namespace KingdomZombies.Proofs; "
                + "public partial class Phase1 : Godot.Node { public override void _Ready() { ";
        const string Guard = "KingdomZombies.Proofs.ProofSequence.Gateway.RequireThread(); ";
        const string Suffix = " } private readonly string _nonce = \"fixture\"; "
                + "private void WriteEvidence(string assertion) { } }";
        EngineAccessPolicy.Check(source: Prefix + Guard + "Alias.Print(what: [1]);" + Suffix,
                path: "src/godot-game/proofs/Phase1.cs", consumer: consumer).ShouldBeEmpty();
        foreach (string body in new[] { "Alias.Print(what: [1]);", "System.Threading.Tasks.Task.Run(action: "
                + "() => Alias.Print(what: [1]));", "Alias.Print(what: [1]); " + Guard,
                "if (false) { " + Guard + " } Alias.Print(what: [1]);",
                Guard + "System.Threading.Tasks.Task.Run(action: () => Alias.Print(what: [1]));",
                Guard + "void Work() { Alias.Print(what: [1]); } System.Threading.Tasks.Task.Run(action: Work);" })
        {
            EngineAccessPolicy.Check(source: Prefix + body + Suffix, path: "src/godot-game/proofs/Phase1.cs",
                    consumer: consumer)
                    .Any(predicate: error => error.StartsWith(value: "ENGINE_ACCESS ",
                            comparisonType: StringComparison.Ordinal)).ShouldBeTrue();
        }

        EngineAccessPolicy.Check(source: Prefix.Replace(oldValue: "class Phase1", newValue: "class Other",
                comparisonType: StringComparison.Ordinal) + "Alias.Print(what: [1]); } }",
                path: "src/godot-game/proofs/Other.cs", consumer: consumer).ShouldNotBeEmpty();
        EngineAccessPolicy.Check(source: Prefix.Replace(oldValue: "public override void",
                newValue: "public override async void", comparisonType: StringComparison.Ordinal) + Guard
                + "await System.Threading.Tasks.Task.Delay(millisecondsDelay: 1).ConfigureAwait(false); "
                + "Alias.Print(what: [1]);" + Suffix, path: "src/godot-game/proofs/Phase1.cs", consumer: consumer)
                .ShouldNotBeEmpty();
        EngineAccessPolicy.Check(source: (Prefix + Guard + "Alias.Print(what: [1]); } }")
                .Replace(oldValue: "namespace KingdomZombies.Proofs", newValue: "namespace Other.Proofs",
                        comparisonType: StringComparison.Ordinal), path: "src/godot-game/proofs/Other.cs",
                consumer: consumer)
                .ShouldNotBeEmpty();
        ProofProtocol.Record(id: "engine.source-policy", diagnostic: "ENGINE_SOURCE_GUARDED",
                location: "actual handwritten sources and aliased/spoofed/late-guard/worker subjects");
    }

    /// <summary>Runs real state and snapshots without loading native engine bindings into this host.</summary>
    [Fact, Trait("Case", "engine.core-independence")]
    public void Run_CoreState_PreservesEngineIndependence()
    {
        AppDomain.CurrentDomain.GetAssemblies().Any(predicate: assembly => StringComparer.Ordinal.Equals(
                x: assembly.GetName().Name, y: "GodotSharp")).ShouldBeFalse();
        ExampleState state = SnapshotProofData.State();
        ExampleState restored = SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: state));
        SnapshotProofData.Equal(actual: restored, expected: state);
        TypeBoundaryScanner.Validate(assembly: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/core/bin/Debug/net10.0/KingdomZombies.Core.dll"),
                expectedType: "KingdomZombies.Core.ExampleState", boundary: "core").ShouldBeEmpty();
        AppDomain.CurrentDomain.GetAssemblies().Any(predicate: assembly => StringComparer.Ordinal.Equals(
                x: assembly.GetName().Name, y: "GodotSharp")).ShouldBeFalse();
        ProofProtocol.Record(id: "engine.core-independence", diagnostic: "CORE_ENGINE_INDEPENDENT",
                location: "actual Core state/snapshot operations, compiled metadata and loaded-assembly inventory");
    }
}
