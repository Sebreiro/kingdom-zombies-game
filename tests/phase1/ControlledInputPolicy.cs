using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Restricts resolved random symbols to the exact seeded Core adapter surface.</summary>
internal static class ControlledInputPolicy
{
    /// <summary>Rejects aliases, adjacent types/files and random construction without the explicit seed.</summary>
    internal static IReadOnlyList<string> Validate(CSharpCompilation compilation)
    {
        var errors = new List<string>();
        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            SemanticModel model = compilation.GetSemanticModel(syntaxTree: tree);
            foreach (NameSyntax name in tree.GetRoot().DescendantNodes().OfType<NameSyntax>())
            {
                if (name.Ancestors().OfType<UsingDirectiveSyntax>().Any())
                {
                    continue;
                }

                ISymbol? symbol = model.GetSymbolInfo(node: name).Symbol;
                string? type = (symbol as INamedTypeSymbol ?? symbol?.ContainingType)?.ToDisplayString();
                if (!StringComparer.Ordinal.Equals(x: type, y: "System.Random"))
                {
                    continue;
                }

                ISymbol? owner = model.GetEnclosingSymbol(position: name.SpanStart);
                bool field = IsAdapterField(name: name);
                bool allowed = StringComparer.Ordinal.Equals(x: tree.FilePath, y: "src/core/ReplayRandom.cs")
                        && StringComparer.Ordinal.Equals(x: (owner as INamedTypeSymbol
                                ?? owner?.ContainingType)?.ToDisplayString(),
                                y: "KingdomZombies.Core.ReplayRandom")
                        && (field || owner is IMethodSymbol method && (method.MethodKind is MethodKind.Constructor
                                || StringComparer.Ordinal.Equals(x: method.Name, y: "Draw")));
                if (!allowed || symbol is IPropertySymbol)
                {
                    errors.Add(item: "RANDOM_SCOPE: " + tree.FilePath + ":" + name.GetLocation()
                            .GetLineSpan().StartLinePosition);
                }
            }

            ValidateConstruction(tree: tree, model: model, errors: errors);
        }

        return errors;
    }

    private static bool IsAdapterField(NameSyntax name)
    {
        return name.Ancestors().OfType<FieldDeclarationSyntax>().Any(predicate: declaration =>
                declaration.Modifiers.Any(predicate: modifier => modifier.IsKind(kind: SyntaxKind.ReadOnlyKeyword))
                        && declaration.Declaration.Variables.Count == 1
                        && StringComparer.Ordinal.Equals(x: declaration.Declaration.Variables[0].Identifier.ValueText,
                                y: "_random"));
    }

    private static void ValidateConstruction(SyntaxTree tree, SemanticModel model, List<string> errors)
    {
        foreach (BaseObjectCreationExpressionSyntax creation in tree.GetRoot().DescendantNodes()
                .OfType<BaseObjectCreationExpressionSyntax>())
        {
            if (model.GetSymbolInfo(node: creation).Symbol is not IMethodSymbol constructor
                    || !StringComparer.Ordinal.Equals(x: constructor.ContainingType.ToDisplayString(),
                            y: "System.Random"))
            {
                continue;
            }

            ISymbol? owner = model.GetEnclosingSymbol(position: creation.SpanStart);
            bool seeded = creation.ArgumentList?.Arguments.Count == 1
                    && model.GetSymbolInfo(node: creation.ArgumentList.Arguments[0].Expression).Symbol
                            is IParameterSymbol seed && seed.Type.SpecialType is SpecialType.System_Int32
                    && StringComparer.Ordinal.Equals(x: seed.Name, y: "seed");
            if (!seeded || owner is not IMethodSymbol { MethodKind: MethodKind.Constructor }
                    || !StringComparer.Ordinal.Equals(x: owner.ContainingType?.ToDisplayString(),
                            y: "KingdomZombies.Core.ReplayRandom")
                    || !StringComparer.Ordinal.Equals(x: tree.FilePath, y: "src/core/ReplayRandom.cs"))
            {
                errors.Add(item: "RANDOM_SCOPE: " + tree.FilePath + ":" + creation.GetLocation()
                        .GetLineSpan().StartLinePosition);
            }
        }
    }
}
