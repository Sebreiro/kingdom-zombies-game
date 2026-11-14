using System;
using System.Linq;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks production native access against successful exact controls and rejecting neighbors.</summary>
public sealed class FoundationPermissionTests
{
    /// <summary>Rejects an unguarded native event subscription after accepting the exact guarded source.</summary>
    [Fact]
    public void ProductionEventAccessRequiresAuthenticPrecedingGuard()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        const string Path = "src/godot-game/foundation/FoundationView.cs";
        const string Prefix = "using Godot; using KingdomZombies.Core; using KingdomZombies.Proofs; "
                + "namespace KingdomZombies.Foundation; internal static class FoundationView { "
                + "internal static void Project(Node2D world, FoundationActorState actor, "
                + "MainThreadGateway gateway) { ";
        const string Suffix = " } private static void Changed() { } }";
        const string Body = "world.VisibilityChanged += Changed;";
        EngineAccessPolicy.Check(source: Prefix + "gateway.RequireThread(); " + Body + Suffix, path: Path,
                consumer: consumer).ShouldBeEmpty();
        EngineAccessPolicy.Check(source: Prefix + Body + Suffix, path: Path, consumer: consumer)
                .Any(predicate: error => error.Contains(value: "Godot.CanvasItem.VisibilityChanged",
                        comparisonType: StringComparison.Ordinal)).ShouldBeTrue();
        ProofProtocol.Record(id: "core.permissions", diagnostic: "EXACT_PRODUCTION_EVENT_GUARD");
    }
}
