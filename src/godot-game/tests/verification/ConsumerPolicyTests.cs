using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Closes active-branch false acceptances against the actual selected consumer compiler inputs.</summary>
public sealed class ConsumerPolicyTests
{
    /// <summary>Actual Debug and Godot/platform symbols activate calls; inverted branches stay inert.</summary>
    [Fact, Trait("Case", "engine.consumer-branches")]
    public void Check_EvaluatedBranches_RejectsActiveEngineCalls()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root,
                project: ProjectEvaluator.Expected[2]);
        string platform = consumer.Options.PreprocessorSymbolNames.Single(predicate: symbol =>
                symbol is "GODOT_WINDOWS" or "GODOT_LINUX" or "GODOT_LINUXBSD");
        foreach (string symbol in new[] { "DEBUG", "TOOLS", platform })
        {
            consumer.Options.PreprocessorSymbolNames.ShouldContain(expected: symbol, comparer: StringComparer.Ordinal);
            string body = "namespace KingdomZombies.Proofs; internal static class Subject { "
                    + "internal static void Run(Godot.Node node) { node.QueueFree(); } }\n#endif\n";
            string active = "#if " + symbol + "\n" + body;
            EngineAccessPolicy.Check(source: active, path: "Subject.cs", consumer: consumer).ShouldContain(
                    elementPredicate: error => error.StartsWith(value: "ENGINE_ACCESS ",
                            comparisonType: StringComparison.Ordinal));
            EngineAccessPolicy.Check(source: "#if !" + symbol + "\n" + body, path: "Subject.cs",
                    consumer: consumer).ShouldBeEmpty();
        }

        Should.Throw<InvalidDataException>(actual: () => EngineAccessPolicy.Check(source:
                "class Subject { void Run() { UnknownEngine.QueueFree(); } }", path: "Subject.cs", consumer: consumer))
                .Message.ShouldContain(expected: "CONSUMER_COMPILATION");
        ProofProtocol.Record(id: "engine.consumer-branches", diagnostic: "CONSUMER_BRANCHES_CHECKED",
                location: "actual evaluated DEBUG/TOOLS/" + platform + "; inverse branches and unresolved subject");
    }

    /// <summary>An active unregistered suppression silences the analyzer but the source guard rejects it.</summary>
    [Fact, Trait("Case", "rules.suppression-conditional")]
    public async Task Check_ConditionalSuppression_RejectsAnalyzerEscape()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root,
                project: ProjectEvaluator.Expected[0]);
        consumer.Options.PreprocessorSymbolNames.ShouldContain(expected: "DEBUG", comparer: StringComparer.Ordinal);
        const string Attribute = "[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(\n"
                + "\"Sebreiro.CodeRules\", \"SCR0001\", Scope = \"member\",\n"
                + "Target = \"~M:KingdomZombies.Core.Subject.Read\", Justification = \"unregistered\")]\n";
        const string Body = "namespace KingdomZombies.Core; internal static class Subject { "
                + "internal static int Read() { return System.Math.Abs(-1); } }\n";
        string active = "#if DEBUG\n" + Attribute + "#endif\n" + Body;
        (await Diagnostics(consumer: consumer, source: active).ConfigureAwait(continueOnCapturedContext: true))
                .ShouldBeEmpty();
        ExemptionValidator.CheckSource(path: "Subject.cs", content: active, consumer: consumer).ShouldContain(
                elementPredicate: error => error.StartsWith(value: "SUPPRESSION_UNREGISTERED Subject.cs(2,12)",
                        comparisonType: StringComparison.Ordinal));
        string inactive = "#if !DEBUG\n" + Attribute + "#endif\n" + Body;
        (await Diagnostics(consumer: consumer, source: inactive).ConfigureAwait(continueOnCapturedContext: true))
                .ShouldContain(elementPredicate: diagnostic => diagnostic.Id == "SCR0001");
        ExemptionValidator.CheckSource(path: "Subject.cs", content: inactive, consumer: consumer).ShouldBeEmpty();
        (await Diagnostics(consumer: consumer, source: Body).ConfigureAwait(continueOnCapturedContext: true))
                .ShouldContain(elementPredicate: diagnostic => diagnostic.Id == "SCR0001");
        ProofProtocol.Record(id: "rules.suppression-conditional", diagnostic: "CONDITIONAL_SUPPRESSION_REJECTED",
                location: "Subject.cs(2,12), actual NamedArgumentAnalyzer suppressed/unsuppressed controls");
    }

    private static async Task<ImmutableArray<Diagnostic>> Diagnostics(ConsumerCompilation consumer, string source)
    {
        SyntaxTree tree = consumer.Parse(source: source, path: "Subject.cs");
        var compilation = CSharpCompilation.Create(assemblyName: "ConditionalSuppression", syntaxTrees: [tree],
                references: consumer.References, options: new CSharpCompilationOptions(
                        outputKind: OutputKind.DynamicallyLinkedLibrary));
        ConsumerCompilation.RequireValid(compilation: compilation);
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        Type type = assembly.GetType(name: "Sebreiro.CodeRules.Analyzers.NamedArgumentAnalyzer", throwOnError: true)
                ?? throw new InvalidDataException(message: "Actual analyzer type required");
        DiagnosticAnalyzer analyzer = Activator.CreateInstance(type: type) as DiagnosticAnalyzer
                ?? throw new InvalidDataException(message: "Actual analyzer instance required");
        return await compilation.WithAnalyzers(analyzers: [analyzer]).GetAnalyzerDiagnosticsAsync()
                .ConfigureAwait(continueOnCapturedContext: false);
    }
}
