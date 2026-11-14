using System;
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
}
