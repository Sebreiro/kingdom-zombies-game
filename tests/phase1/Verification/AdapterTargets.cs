using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KingdomZombies.Phase1.Tests.Verification;

/// <summary>Reads exact constant target arguments even when a long documentation ID is wrapped in source.</summary>
internal static class AdapterTargets
{
    internal static IEnumerable<string> Read(string source)
    {
        foreach (var node in CSharpSyntaxTree.ParseText(text: source).GetRoot().DescendantNodes())
            if (node is AttributeArgumentSyntax { NameEquals.Name.Identifier.ValueText: "Target" } argument)
                yield return Constant(expression: argument.Expression);
    }

    private static string Constant(ExpressionSyntax expression)
    {
        return expression switch
        {
            LiteralExpressionSyntax literal => literal.Token.ValueText,
            ParenthesizedExpressionSyntax group => Constant(expression: group.Expression),
            BinaryExpressionSyntax sum when sum.IsKind(kind: SyntaxKind.AddExpression) =>
                    Constant(expression: sum.Left) + Constant(expression: sum.Right),
            _ => throw new InvalidOperationException(message: "Adapter Target must be a constant string"),
        };
    }
}
