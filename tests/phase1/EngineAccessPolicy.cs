using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Resolves engine symbols while restricting adapters to exact files, types and guarded methods.</summary>
internal static class EngineAccessPolicy
{
    /// <summary>Rejects direct engine operations outside declared guarded adapter methods.</summary>
    internal static IReadOnlyList<string> Check(string source, string path)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(text: source, path: path);
        var compilation = CSharpCompilation.Create(assemblyName: "EngineAccessSubject",
                syntaxTrees: Trees().Where(predicate: candidate => !StringComparer.Ordinal.Equals(
                        x: candidate.FilePath, y: path)).Append(element: tree), references: References());
        return CheckTree(tree: tree, compilation: compilation);
    }

    /// <summary>Checks resolved operations using the complete actual declaration compilation.</summary>
    internal static IReadOnlyList<string> CheckTree(SyntaxTree tree, CSharpCompilation compilation)
    {
        string path = tree.FilePath;
        SemanticModel model = compilation.GetSemanticModel(syntaxTree: tree);
        var errors = new List<string>();
        foreach (SyntaxNode node in tree.GetRoot().DescendantNodes().Where(predicate: node => node is
                InvocationExpressionSyntax or BaseObjectCreationExpressionSyntax or MemberAccessExpressionSyntax))
        {
            ISymbol? symbol = model.GetSymbolInfo(node: node).Symbol;
            if (node is MemberAccessExpressionSyntax && symbol is not (IPropertySymbol or IFieldSymbol))
            {
                continue;
            }

            if (symbol?.ContainingAssembly?.Name is not ("GodotSharp" or "GodotSharpEditor"))
            {
                continue;
            }

            if (!Allowed(node: node, path: path, model: model))
            {
                FileLinePositionSpan location = node.GetLocation().GetLineSpan();
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
        IReadOnlyList<SyntaxTree> trees = Trees();
        var compilation = CSharpCompilation.Create(assemblyName: "EngineAccessRepository", syntaxTrees: trees,
                references: References());
        foreach (SyntaxTree tree in trees.Where(predicate: tree => !tree.FilePath.Contains(value: "/.godot/",
                comparisonType: StringComparison.Ordinal)))
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
        bool adapter = StringComparer.Ordinal.Equals(x: path, y: "src/godot-game/proofs/" + type + ".cs")
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

    private static List<SyntaxTree> Trees()
    {
        var trees = new List<SyntaxTree>();
        foreach (string file in Directory.EnumerateFiles(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game"), searchPattern: "*.cs", searchOption: SearchOption.AllDirectories))
        {
            string path = Path.GetRelativePath(relativeTo: ProofProtocol.Root, path: file)
                    .Replace(oldChar: '\\', newChar: '/');
            if (!path.Contains(value: "/.godot/", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/obj/Debug/generated/", comparisonType: StringComparison.Ordinal))
            {
                trees.Add(item: CSharpSyntaxTree.ParseText(text: File.ReadAllText(path: file), path: path));
            }
        }

        return trees;
    }

    private static MetadataReference[] References()
    {
        string trusted = AppContext.GetData(name: "TRUSTED_PLATFORM_ASSEMBLIES") as string
                ?? throw new InvalidOperationException(message: "ENGINE_ACCESS_FRAMEWORK");
        string godot = Path.Combine(path1: ProofProtocol.Root,
                path2: ".phase1/packages/godotsharp/4.7.2/lib/net8.0/GodotSharp.dll");
        string core = Path.Combine(path1: ProofProtocol.Root,
                path2: "src/core/bin/Debug/net10.0/KingdomZombies.Core.dll");
        string ports = Path.Combine(path1: ProofProtocol.Root,
                path2: "src/multiplayer-abstractions/bin/Debug/net10.0/KingdomZombies.Multiplayer.Abstractions.dll");
        return [..trusted.Split(separator: Path.PathSeparator).Concat(second: [godot, core, ports])
                .Distinct(comparer: StringComparer.Ordinal).Select(selector: path =>
                        MetadataReference.CreateFromFile(path: path))];
    }
}
