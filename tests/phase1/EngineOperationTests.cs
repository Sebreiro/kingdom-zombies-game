using System;
using System.IO;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires operation checks for native delegates and implicit inherited member references.</summary>
public sealed class EngineOperationTests
{
    /// <summary>Rejects native captures in guarded adapters while accepting guarded wrappers.</summary>
    [Fact, Trait("Case", "engine.method-escape")]
    public void Check_NativeDelegates_RejectsEscapingMethodGroups()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        foreach (string body in new[] { "System.Action work = node.QueueFree; "
                + "return System.Threading.Tasks.Task.Run(action: work);",
            "return System.Threading.Tasks.Task.Run(action: node.QueueFree);" })
        {
            string source = "namespace KingdomZombies.Proofs; internal static class Subject { "
                    + "internal static System.Threading.Tasks.Task Run(Godot.Node node) { " + body + " } }";
            ConsumerCompilation.RequireValid(compilation: consumer.Create(source: source, path: "Subject.cs"));
            EngineAccessPolicy.Check(source: source, path: "Subject.cs", consumer: consumer).ShouldContain(
                    elementPredicate: error => error.Contains(value: "Godot.Node.QueueFree()",
                            comparisonType: StringComparison.Ordinal));
        }

        const string Path = "src/godot-game/proofs/Phase1.cs";
        string actual = File.ReadAllText(path: System.IO.Path.Combine(path1: ProofProtocol.Root, path2: Path));
        string capture = actual.Replace(oldValue: "WriteEvidence(assertion: \"ready\");",
                newValue: "System.Action work = QueueFree; _ = System.Threading.Tasks.Task.Run(action: work);",
                comparisonType: StringComparison.Ordinal);
        EngineAccessPolicy.Check(source: capture, path: Path, consumer: consumer).ShouldContain(
                elementPredicate: error => error.Contains(value: "Godot.Node.QueueFree()",
                        comparisonType: StringComparison.Ordinal));
        string wrapper = capture.Replace(oldValue: "work = QueueFree", newValue: "work = _Ready",
                comparisonType: StringComparison.Ordinal);
        EngineAccessPolicy.Check(source: wrapper, path: Path, consumer: consumer).ShouldBeEmpty();
        string direct = actual.Replace(oldValue: "WriteEvidence(assertion: \"ready\");", newValue: "QueueFree();",
                comparisonType: StringComparison.Ordinal);
        EngineAccessPolicy.Check(source: direct, path: Path, consumer: consumer).ShouldBeEmpty();
        ProofProtocol.Record(id: "engine.method-escape", diagnostic: "ENGINE_METHOD_ESCAPE",
                location: "compiling Task.Run native captures; actual guarded Phase1 capture/wrapper/direct controls");
    }

    /// <summary>Requires bare inherited property and field references in the engine operation inventory.</summary>
    [Fact, Trait("Case", "engine.inherited-members")]
    public void Check_ImplicitNativeMembers_RejectsUnguardedAccess()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        foreach (string declaration in new[] { "internal Godot.StringName Read() { return Name; }",
            "internal static long Read() { return NotificationReady; }" })
        {
            string source = "namespace KingdomZombies.Proofs; internal sealed class Subject : Godot.Node { "
                    + declaration + " }";
            ConsumerCompilation.RequireValid(compilation: consumer.Create(source: source, path: "Subject.cs"));
            EngineAccessPolicy.Check(source: source, path: "Subject.cs", consumer: consumer).ShouldContain(
                    elementPredicate: error => error.StartsWith(value: "ENGINE_ACCESS Subject.cs(",
                            comparisonType: StringComparison.Ordinal));
        }

        ProofProtocol.Record(id: "engine.inherited-members", diagnostic: "ENGINE_IMPLICIT_MEMBER",
                location: "compiling bare Godot.Node.Name property and NotificationReady field references");
    }
}
