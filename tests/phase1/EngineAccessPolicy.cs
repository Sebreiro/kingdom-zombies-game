using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Resolves engine symbols while restricting adapters to exact files, types and guarded methods.</summary>
internal static class EngineAccessPolicy
{
    /// <summary>Rejects direct engine operations outside declared guarded adapter methods.</summary>
    internal static IReadOnlyList<string> Check(string source, string path, ConsumerCompilation? consumer = null)
    {
        consumer ??= ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        CSharpCompilation compilation = consumer.Create(source: source, path: path);
        ConsumerCompilation.RequireValid(compilation: compilation);
        SyntaxTree tree = compilation.SyntaxTrees.Single(predicate: candidate => StringComparer.Ordinal.Equals(
                x: candidate.FilePath, y: path));
        return CheckTree(tree: tree, compilation: compilation);
    }

    /// <summary>Checks resolved operations using the complete actual declaration compilation.</summary>
    internal static IReadOnlyList<string> CheckTree(SyntaxTree tree, CSharpCompilation compilation)
    {
        string path = tree.FilePath;
        SemanticModel model = compilation.GetSemanticModel(syntaxTree: tree);
        var errors = new List<string>();
        IEnumerable<IOperation> operations = tree.GetRoot().DescendantNodes().Select(selector: node =>
                model.GetOperation(node: node)).OfType<IOperation>().DistinctBy(keySelector: operation =>
                        (operation.Kind, operation.Syntax.Span));
        foreach (IOperation operation in operations)
        {
            ISymbol? symbol = operation switch
            {
                IInvocationOperation invocation => invocation.TargetMethod,
                IObjectCreationOperation creation => creation.Constructor,
                IMethodReferenceOperation reference => reference.Method,
                IPropertyReferenceOperation property => property.Property,
                IFieldReferenceOperation field => field.Field,
                _ => null
            };
            if (symbol?.ContainingAssembly?.Name is not ("GodotSharp" or "GodotSharpEditor"))
            {
                continue;
            }

            if (operation is IMethodReferenceOperation || !Allowed(node: operation.Syntax, path: path, model: model))
            {
                FileLinePositionSpan location = operation.Syntax.GetLocation().GetLineSpan();
                errors.Add(item: "ENGINE_ACCESS " + path + "(" + (location.StartLinePosition.Line + 1)
                        .ToString(provider: CultureInfo.InvariantCulture) + "," + (location.StartLinePosition
                                .Character + 1).ToString(provider: CultureInfo.InvariantCulture) + ") "
                        + symbol.ToDisplayString());
            }
        }

        return errors;
    }

    /// <summary>Checks all actual handwritten game sources, retaining their checkout-relative identities.</summary>
    internal static IReadOnlyList<string> Repository()
    {
        var errors = new List<string>();
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root,
                project: ProjectEvaluator.Expected[2]);
        CSharpCompilation compilation = consumer.Create();
        ConsumerCompilation.RequireValid(compilation: compilation);
        foreach (SyntaxTree tree in compilation.SyntaxTrees.Where(predicate: tree => consumer.AuthoredPaths.Contains(
                value: tree.FilePath, comparer: StringComparer.Ordinal)))
        {
            errors.AddRange(collection: CheckTree(tree: tree, compilation: compilation));
        }

        return errors;
    }

    private static readonly Dictionary<string, string[]> _adapters = new(comparer: StringComparer.Ordinal)
    {
        ["Phase1"] = ["Phase1", "_Ready", "_ExitTree", "WriteEvidence"],
        ["LifecycleFixture"] = ["LifecycleFixture", "_EnterTree", "_Ready", "_Process", "_ExitTree", "Use",
            "ApplyMode", "CheckedExports"],
        ["ProofResource"] = ["ProofResource", "Read"],
        ["ProofSequence"] = ["Reject", "Tick", "Load", "Finish", "Cleanup", "Publish"]
    };

    private static bool Allowed(SyntaxNode node, string path, SemanticModel model)
    {
        string type = node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText ?? "";
        BaseMethodDeclarationSyntax? method = node.Ancestors().OfType<BaseMethodDeclarationSyntax>().FirstOrDefault();
        string name = method is MethodDeclarationSyntax ordinary ? ordinary.Identifier.ValueText
                : method is ConstructorDeclarationSyntax constructor ? constructor.Identifier.ValueText : "";
        bool production = StringComparer.Ordinal.Equals(x: path, y: "src/godot-game/foundation/" + type + ".cs")
                && StringComparer.Ordinal.Equals(x: node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
                        .FirstOrDefault()?.Name.ToString(), y: "KingdomZombies.Foundation")
                && node.Ancestors().OfType<TypeDeclarationSyntax>().Count() == 1
                && (type is "FoundationRoot" && new[] { "FoundationRoot", "_Ready", "_Input", "_Process",
                    "_ExitTree", "Start", "ReturnToMenu", "Quit", "LoseFocus", "Observe", "Capture" }
                        .Contains(value: name, comparer: StringComparer.Ordinal)
                        || type is "FoundationView" && name is "Project");
        bool adapter = production || StringComparer.Ordinal.Equals(x: path,
                y: "src/godot-game/proofs/" + type + ".cs")
                && StringComparer.Ordinal.Equals(x: node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
                        .FirstOrDefault()?.Name.ToString(), y: "KingdomZombies.Proofs")
                && node.Ancestors().OfType<TypeDeclarationSyntax>().Count() == 1
                && _adapters.TryGetValue(key: type, value: out string[]? methods)
                && methods.Contains(value: name, comparer: StringComparer.Ordinal);
        return adapter && !node.Ancestors().OfType<AnonymousFunctionExpressionSyntax>().Any()
                && !node.Ancestors().OfType<LocalFunctionStatementSyntax>().Any()
                && !(method?.Modifiers.Any(kind: SyntaxKind.AsyncKeyword) ?? false)
                && (method?.Body?.Statements.OfType<ExpressionStatementSyntax>().Any(predicate: statement =>
                        statement.SpanStart < node.SpanStart && statement.Expression is InvocationExpressionSyntax guard
                                && model.GetSymbolInfo(node: guard).Symbol is IMethodSymbol symbol
                                && StringComparer.Ordinal.Equals(x: symbol.Name, y: "RequireThread")
                                && StringComparer.Ordinal.Equals(x: symbol.ContainingType.ToDisplayString(),
                                        y: "KingdomZombies.Proofs.MainThreadGateway")) ?? false);
    }

}
