using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Pairs exact LAN permission positives with compilable adjacent misuse controls.</summary>
public sealed class FoundationLanPermissionTests
{
    /// <summary>A newly created gateway cannot impersonate the captured native-thread authority.</summary>
    [Fact]
    public void ExactLanPermissionsRejectAdjacentMisuse()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        const string Path = "src/godot-game/foundation/FoundationRoot.cs";
        string source = File.ReadAllText(path: System.IO.Path.Combine(path1: ProofProtocol.Root, path2: Path));
        EngineAccessPolicy.Check(source: source, path: Path, consumer: consumer).ShouldBeEmpty();
        SyntaxTree tree = consumer.Parse(source: source, path: Path);
        MethodDeclarationSyntax method = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken)
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>().Single(predicate: method => StringComparer.Ordinal.Equals(
                        x: method.Identifier.ValueText, y: "_Process"));
        string forged = source.Replace(oldValue: method.ToFullString(), newValue: method.ToFullString().Replace(
                oldValue: "_gateway.RequireThread();", newValue: "new MainThreadGateway().RequireThread();",
                comparisonType: StringComparison.Ordinal), comparisonType: StringComparison.Ordinal);
        EngineAccessPolicy.Check(source: forged, path: Path, consumer: consumer).ShouldNotBeEmpty(
                customMessage: "A fresh gateway would capture a worker thread instead of the original engine thread.");
        CheckNativeTuples(consumer: consumer);
        CheckPublicTypes();
        EngineAccessPolicy.Repository().ShouldBeEmpty();
        ProofProtocol.Record(id: "core.permissions",
                diagnostic: "EXACT_LAN_PERMISSIONS:42_TUPLES_8_PUBLIC_TYPES_WITH_VALID_NEGATIVES");
    }

    private const string Native = " _ = Godot.Input.IsPhysicalKeyPressed(keycode: Godot.Key.D); ";
    private static readonly (string Type, string[] Methods)[] _native =
    [
        ("FoundationRoot", ["Enter", "Browse", "JoinAddress", "ShowWorld", "PollKeys", "ActivateFocused"]),
        ("FoundationView", ["ProjectRoster"]),
        ("EnetConnectionService", ["Host", "Join", "Poll", "Send", "Close", "Check"]),
        ("FoundationAutomation", ["Deliver"]),
        ("FoundationDrawObserver", ["Start", "Stop", "Draw", "ObserveActor", "ObserveControl", "Capture", "Write"]),
        ("FoundationMenu", ["Attach", "Detach", "CommitName", "Browse", "Tick", "Attempt", "Recover", "Activate",
            "PopupOpen", "Escape", "Profiles", "Feedback", "Refresh", "Back", "Lock", "Rows", "AddRow", "Detail",
            "RowLabel", "HideDetail"]),
        ("FoundationHud", ["Project"]),
    ];
    private static readonly (string Path, string Type, int Project)[] _public =
    [
        ("src/core/FoundationSnapshot.cs", "FoundationSnapshot", 0),
        ("src/core/FoundationParticipantState.cs", "FoundationParticipantState", 0),
        ("src/multiplayer-abstractions/IConnectionService.cs", "IConnectionService", 1),
        ("src/multiplayer-abstractions/ConnectionPacket.cs", "ConnectionPacket", 1),
        ("src/multiplayer-abstractions/IDiscoveryService.cs", "IDiscoveryService", 1),
        ("src/multiplayer-abstractions/IIdentityService.cs", "IIdentityService", 1),
        ("src/multiplayer-abstractions/SessionAdvertisement.cs", "SessionAdvertisement", 1),
        ("src/multiplayer-abstractions/PlayerProfile.cs", "PlayerProfile", 1),
    ];

    private static void CheckNativeTuples(ConsumerCompilation consumer)
    {
        CheckInventory();
        int checkedMethods = 0;
        foreach ((string type, string[] methods) in _native)
        {
            string path = "src/godot-game/foundation/" + type + ".cs";
            string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: path));
            SyntaxTree tree = consumer.Parse(source: source, path: path);
            foreach (string name in methods)
            {
                MethodDeclarationSyntax method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
                        .Single(predicate: candidate => StringComparer.Ordinal.Equals(x: candidate.Identifier.ValueText,
                                y: name));
                string guard = type is "FoundationView" or "FoundationAutomation" or "FoundationHud"
                        ? "gateway.RequireThread();"
                        : "_gateway.RequireThread();";
                CheckTuple(consumer: consumer, source: source, path: path, method: method, guard: guard);
                checkedMethods++;
            }
        }

        checkedMethods.ShouldBe(expected: 42);
    }

    private static void CheckInventory()
    {
        string[] inherited = ["FoundationRoot.FoundationRoot", "FoundationRoot._Ready", "FoundationRoot._Input",
            "FoundationRoot._Process", "FoundationRoot._ExitTree", "FoundationRoot.Start",
            "FoundationRoot.ReturnToMenu",
            "FoundationRoot.Quit", "FoundationRoot.LoseFocus", "FoundationRoot.Observe", "FoundationRoot.Capture",
            "FoundationView.Project"];
        string[] expected = [..inherited.Concat(second: _native.SelectMany(selector: tuple => tuple.Methods.Select(
                selector: method => tuple.Type + "." + method))).Order(comparer: StringComparer.Ordinal)];
        EngineAccessPolicy.ProductionTuples.Order(comparer: StringComparer.Ordinal).ShouldBe(expected: expected);
    }

    private static void CheckTuple(ConsumerCompilation consumer, string source, string path,
            MethodDeclarationSyntax method, string guard)
    {
        EngineAccessPolicy.Check(source: source, path: path, consumer: consumer).ShouldBeEmpty();
        string removedGuard = source.Replace(oldValue: method.ToFullString(), newValue: method.ToFullString().Replace(
                oldValue: guard, newValue: "", comparisonType: StringComparison.Ordinal),
                comparisonType: StringComparison.Ordinal);
        EngineAccessPolicy.Check(source: removedGuard, path: path, consumer: consumer).ShouldNotBeEmpty();
        string suffix = method.ReturnType.ToString() is "void" ? ""
                : method.ReturnType.ToString() is "bool" ? " return false; " : " return []; ";
        string positive = ReplaceBody(source: source, method: method, body: guard + Native + suffix);
        EngineAccessPolicy.Check(source: positive, path: path, consumer: consumer).ShouldBeEmpty();
        foreach (string body in new[] { Native + suffix, Native + guard + suffix,
            "if (false) { " + guard + " } " + Native + suffix,
            "new KingdomZombies.Proofs.MainThreadGateway().RequireThread();" + Native + suffix,
            guard + "System.Action misuse = new Godot.Node().QueueFree;" + suffix,
            guard + "System.Threading.Tasks.Task.Run(action: () => { " + Native + " });" + suffix })
        {
            EngineAccessPolicy.Check(source: ReplaceBody(source: source, method: method, body: body), path: path,
                    consumer: consumer).ShouldNotBeEmpty(customMessage: path + ":" + method.Identifier.ValueText);
        }

        CheckMethodAndPath(consumer: consumer, source: positive, path: path, methodName: method.Identifier.ValueText);
        CheckNamespaceAndAsync(consumer: consumer, source: positive, path: path, method: method, guard: guard);
    }

    private static string ReplaceBody(string source, MethodDeclarationSyntax method, string body)
    {
        BlockSyntax block = SyntaxFactory.ParseStatement(text: "{" + body + "}") as BlockSyntax
                ?? throw new InvalidDataException(message: "PROOF_BLOCK");
        return source.Replace(oldValue: method.ToFullString(), newValue: method.WithBody(body: block).ToFullString(),
                comparisonType: StringComparison.Ordinal);
    }

    private static void CheckMethodAndPath(ConsumerCompilation consumer, string source, string path, string methodName)
    {
        CSharpCompilation compilation = consumer.Create(source: source, path: path);
        ConsumerCompilation.RequireValid(compilation: compilation);
        SyntaxTree tree = compilation.SyntaxTrees.Single(predicate: candidate => StringComparer.Ordinal.Equals(
                x: candidate.FilePath, y: path));
        SyntaxTree wrongPath = tree.WithFilePath(path: "src/godot-game/foundation/AdjacentLan.cs");
        CSharpCompilation wrong = compilation.ReplaceSyntaxTree(oldTree: tree, newTree: wrongPath);
        ConsumerCompilation.RequireValid(compilation: wrong);
        EngineAccessPolicy.CheckTree(tree: wrongPath, compilation: wrong).ShouldNotBeEmpty();
        MethodDeclarationSyntax method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Single(predicate: candidate => StringComparer.Ordinal.Equals(x: candidate.Identifier.ValueText,
                        y: methodName));
        TypeDeclarationSyntax type = method.Parent as TypeDeclarationSyntax
                ?? throw new InvalidDataException(message: "PROOF_TYPE");
        SyntaxNode adjacent = tree.GetRoot().ReplaceNode(oldNode: type, newNode: type.AddMembers(method.WithIdentifier(
                identifier: SyntaxFactory.Identifier(text: "AdjacentLan"))));
        EngineAccessPolicy.Check(source: adjacent.ToFullString(), path: path, consumer: consumer).ShouldNotBeEmpty();
    }

    private static void CheckPublicTypes()
    {
        foreach ((string path, string type, int project) in _public)
        {
            var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root,
                    project: ProjectEvaluator.Expected[project]);
            string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: path));
            ConsumerCompilation.RequireValid(compilation: consumer.Create(source: source, path: path));
            ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: source, path: path,
                    options: consumer.Options)
                    .ShouldBeEmpty();
            string adjacent = source + "\npublic sealed class " + type + "Adjacent { }\n";
            ConsumerCompilation.RequireValid(compilation: consumer.Create(source: adjacent, path: path));
            ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: adjacent, path: path,
                    options: consumer.Options)
                    .Single().ShouldContain(expected: "R-34: undeclared public type");
            string wrongPath = "src/" + (project == 0 ? "core" : "multiplayer-abstractions") + "/AdjacentLan.cs";
            ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: source, path: wrongPath,
                    options: consumer.Options).ShouldNotBeEmpty();
        }
    }

    private static void CheckNamespaceAndAsync(ConsumerCompilation consumer, string source, string path,
            MethodDeclarationSyntax method, string guard)
    {
        CSharpCompilation compilation = consumer.Create(source: source, path: path);
        string clone = "using KingdomZombies.Foundation;\n" + source.Replace(
                oldValue: "namespace KingdomZombies.Foundation;",
                newValue: "namespace KingdomZombies.Foundation.Adjacent;", comparisonType: StringComparison.Ordinal);
        SyntaxTree cloneTree = consumer.Parse(source: clone, path: path);
        CSharpCompilation wrongNamespace = compilation.AddSyntaxTrees(cloneTree);
        ConsumerCompilation.RequireValid(compilation: wrongNamespace);
        EngineAccessPolicy.CheckTree(tree: cloneTree, compilation: wrongNamespace).ShouldNotBeEmpty();
        SyntaxTree original = compilation.SyntaxTrees.Single(predicate: tree => StringComparer.Ordinal.Equals(
                x: tree.FilePath, y: path));
        TypeDeclarationSyntax type = original.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().First();
        string qualifiers = method.Modifiers.Any(kind: SyntaxKind.StaticKeyword) ? "private static" : "private";
        MemberDeclarationSyntax async = SyntaxFactory.ParseMemberDeclaration(text: qualifiers
                + " async System.Threading.Tasks.Task " + method.Identifier.ValueText
                + "(KingdomZombies.Proofs.MainThreadGateway gateway, string proofAsync, int proofNumber) { "
                + guard + Native + " await System.Threading.Tasks.Task.Delay(millisecondsDelay: 1); }")
                ?? throw new InvalidDataException(message: "ASYNC_CONTROL");
        string changed = original.GetRoot().ReplaceNode(oldNode: type, newNode: type.AddMembers(async)).ToFullString();
        EngineAccessPolicy.Check(source: changed, path: path, consumer: consumer).ShouldNotBeEmpty();
    }
}
