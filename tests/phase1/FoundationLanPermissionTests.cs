using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
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
        ProofProtocol.Record(id: "core.permissions", diagnostic: "EXACT_LAN_PERMISSIONS");
    }
}
