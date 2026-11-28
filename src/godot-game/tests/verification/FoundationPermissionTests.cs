using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks production native access against successful exact controls and rejecting neighbors.</summary>
public sealed class FoundationPermissionTests
{
    /// <summary>Rejects an unguarded native event subscription after accepting the exact guarded source.</summary>
    [Fact]
    public void EventAccess_ProductionCalls_RequireAuthenticPrecedingGuard()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        CheckEvent(consumer: consumer);
        CheckNativeNeighbors(consumer: consumer);
        CheckEveryProductionMethod(consumer: consumer);
        CheckVisibility(consumer: consumer);
        EngineGeneratedTests.VerifyAuthenticatedOrigin();
        EngineGeneratedTests.VerifyGeneratedBoundaries();
        GodotCallSiteAdapterTests.VerifyGeneratedMemberBoundaries();
        ProofProtocol.Record(id: "core.permissions", diagnostic: "EXACT_PRODUCTION_EVENT_GUARD");
    }

    /// <summary>Executes an exact original native and visibility responsibility without skipping
    /// direct callers.</summary>
    internal static void VerifyNativeStage(string kind)
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        switch (kind)
        {
            case "event": CheckEvent(consumer: consumer); break;
            case "neighbors": CheckNativeNeighbors(consumer: consumer); break;
            case "methods": CheckEveryProductionMethod(consumer: consumer); break;
            case "visibility": CheckVisibility(consumer: consumer); break;
            default: throw new InvalidDataException(message: "FOUNDATION_STAGE_UNKNOWN");
        }
    }

    private static void CheckEvent(ConsumerCompilation consumer)
    {
        const string Path = "src/godot-game/game/foundation/FoundationView.cs";
        const string Prefix = "using Godot; using KingdomZombies.Core; using KingdomZombies.Proofs; "
                + "namespace KingdomZombies.Foundation; internal static class FoundationView { "
                + "internal static void Project(Node2D world, FoundationActorState actor, "
                + "MainThreadGateway gateway) { ";
        const string Suffix = " }"
                + " internal static void ProjectRoster(Node2D world, FoundationSnapshot snapshot, "
                + "string bindingId, MainThreadGateway gateway, double? camera = null, bool ownLiving = true) { } "
                + " private static void Changed() { } }";
        const string Body = "world.VisibilityChanged += Changed;";
        EngineAccessPolicy.Check(source: Prefix + "gateway.RequireThread(); " + Body + Suffix, path: Path,
                consumer: consumer).ShouldBeEmpty();
        EngineAccessPolicy.Check(source: Prefix + Body + Suffix, path: Path, consumer: consumer)
                .Any(predicate: error => error.Contains(value: "Godot.CanvasItem.VisibilityChanged",
                        comparisonType: StringComparison.Ordinal)).ShouldBeTrue();
    }

    private static void CheckNativeNeighbors(ConsumerCompilation consumer)
    {
        const string Path = "src/godot-game/game/foundation/FoundationView.cs";
        const string Prefix = "using Godot; using KingdomZombies.Core; using KingdomZombies.Proofs; "
                + "namespace KingdomZombies.Foundation; internal static class FoundationView { "
                + "internal static void Project(Node2D world, FoundationActorState actor, "
                + "MainThreadGateway gateway) { ";
        const string Guard = "gateway.RequireThread(); ";
        const string Suffix = " }"
                + " internal static void ProjectRoster(Node2D world, FoundationSnapshot snapshot, "
                + "string bindingId, MainThreadGateway gateway, double? camera = null, bool ownLiving = true) { } "
                + " }";
        EngineAccessPolicy.Repository().ShouldBeEmpty();
        foreach (string body in new[] { "world.QueueFree();", "world.QueueFree(); " + Guard,
            "if (false) { " + Guard + "} world.QueueFree();", Guard + "System.Action action = world.QueueFree;",
            Guard + "System.Threading.Tasks.Task.Run(action: () => world.QueueFree());",
            Guard + "void Later() { world.QueueFree(); } Later();" })
        {
            string source = Prefix + body + Suffix;
            EngineAccessPolicy.Check(source: source, path: Path, consumer: consumer).Any(
                    predicate: error => error.StartsWith(value: "ENGINE_ACCESS " + Path + "(1,",
                            comparisonType: StringComparison.Ordinal)
                            && error.Contains(value: "Godot.Node.QueueFree()",
                                    comparisonType: StringComparison.Ordinal)).ShouldBeTrue();
        }

        string valid = Prefix + Guard + "world.QueueFree();" + Suffix;
        CSharpCompilation compilation = consumer.Create(source: valid, path: Path);
        SyntaxTree tree = compilation.SyntaxTrees.Single(predicate: candidate => StringComparer.Ordinal.Equals(
                x: candidate.FilePath, y: Path));
        SyntaxTree wrongPath = tree.WithFilePath(path: "src/godot-game/game/foundation/Adjacent.cs");
        CSharpCompilation renamed = compilation.ReplaceSyntaxTree(oldTree: tree, newTree: wrongPath);
        ConsumerCompilation.RequireValid(compilation: renamed);
        EngineAccessPolicy.CheckTree(tree: wrongPath, compilation: renamed).ShouldNotBeEmpty();
        string adjacent = Prefix + Guard + "Adjacent(world: world, gateway: gateway); } "
                + "private static void Adjacent(Node2D world, MainThreadGateway gateway) { " + Guard
                + "world.QueueFree();" + Suffix;
        EngineAccessPolicy.Check(source: adjacent, path: Path, consumer: consumer).ShouldNotBeEmpty();
        string async = valid.Replace(oldValue: "static void Project", newValue: "static async void Project",
                comparisonType: StringComparison.Ordinal).Replace(oldValue: Guard, newValue: Guard
                        + "await System.Threading.Tasks.Task.Delay(millisecondsDelay: 1).ConfigureAwait(false); ",
                        comparisonType: StringComparison.Ordinal);
        EngineAccessPolicy.Check(source: async, path: Path, consumer: consumer).ShouldNotBeEmpty();
        const string Impostor = "internal static class Adjacent { internal static void Project(Godot.Node2D world, "
                + "KingdomZombies.Proofs.MainThreadGateway gateway) { gateway.RequireThread(); world.QueueFree(); } }";
        EngineAccessPolicy.Check(source: valid + Impostor, path: Path, consumer: consumer).ShouldNotBeEmpty();
        string wrongNamespace = valid.Replace(oldValue: "namespace KingdomZombies.Foundation;",
                newValue: "namespace KingdomZombies.Foundation {", comparisonType: StringComparison.Ordinal)
                + " } namespace Forged { " + Impostor + " }";
        EngineAccessPolicy.Check(source: wrongNamespace, path: Path, consumer: consumer).ShouldNotBeEmpty();
    }

    private static void CheckVisibility(ConsumerCompilation consumer)
    {
        ArchitecturePolicy.VisibilityRepository(root: ProofProtocol.Root).ShouldBeEmpty();
        const string Path = "src/godot-game/core/FoundationSession.cs";
        string source = File.ReadAllText(path: System.IO.Path.Combine(path1: ProofProtocol.Root, path2: Path));
        ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: source, path: Path, options: consumer.Options)
                .ShouldBeEmpty();
        ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: source
                + "\npublic sealed class AdjacentFoundation { }\n", path: Path, options: consumer.Options)
                .ShouldContain(expected: "R-34: undeclared public type KingdomZombies.Core.AdjacentFoundation in "
                        + Path, comparer: StringComparer.Ordinal);
    }

    private static void CheckEveryProductionMethod(ConsumerCompilation consumer)
    {
        foreach (string path in new[] { "src/godot-game/game/foundation/FoundationRoot.cs",
            "src/godot-game/game/foundation/FoundationView.cs" })
        {
            string source = File.ReadAllText(path: System.IO.Path.Combine(path1: ProofProtocol.Root, path2: path));
            SyntaxTree original = consumer.Parse(source: source, path: path);
            int rejected = 0;
            foreach (MethodDeclarationSyntax method in original.GetRoot().DescendantNodes()
                    .OfType<MethodDeclarationSyntax>().Where(predicate: method => method.Identifier.ValueText
                            is not ("LoseFocus" or "Start" or "Host" or "Port")))
            {
                string body = method.ToFullString();
                string guard = path.EndsWith(value: "FoundationView.cs", comparisonType: StringComparison.Ordinal)
                        ? "gateway.RequireThread();" : "_gateway.RequireThread();";
                string mutated = source.Replace(oldValue: body, newValue: body.Replace(oldValue: guard,
                        newValue: new string(c: ' ', count: guard.Length), comparisonType: StringComparison.Ordinal),
                        comparisonType: StringComparison.Ordinal);
                EngineAccessPolicy.Check(source: mutated, path: path, consumer: consumer).ShouldNotBeEmpty(
                        customMessage: path + ":" + method.Identifier.ValueText);
                rejected++;
            }

            rejected.ShouldBe(expected: path.EndsWith(value: "FoundationView.cs",
                    comparisonType: StringComparison.Ordinal) ? 2 : 14);
            CSharpCompilation compilation = consumer.Create(source: source, path: path);
            SyntaxTree tree = compilation.SyntaxTrees.Single(predicate: candidate => StringComparer.Ordinal.Equals(
                    x: candidate.FilePath, y: path));
            SyntaxTree wrongPath = tree.WithFilePath(path: "src/godot-game/game/foundation/Forged.cs");
            CSharpCompilation wrong = compilation.ReplaceSyntaxTree(oldTree: tree, newTree: wrongPath);
            ConsumerCompilation.RequireValid(compilation: wrong);
            EngineAccessPolicy.CheckTree(tree: wrongPath, compilation: wrong).ShouldNotBeEmpty();
        }
    }
}
