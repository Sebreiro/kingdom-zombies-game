using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Sebreiro.CodeRules.Analyzers;

/// <summary>
/// Finds every member body the SCR rules check: methods, constructors, destructors, property, indexer and event
/// accessors, expression-bodied properties and indexers, operators, conversion operators, local functions, lambdas
/// and anonymous methods, and the global statements of a file, which form one body.
/// </summary>
internal static class MemberBodies
{
    /// <summary>
    /// Finds the member bodies under <paramref name="root"/>. Nested bodies (a local function or a lambda inside a
    /// method) are returned on their own as well as inside the span of their enclosing body.
    /// </summary>
    /// <param name="root">The root of a syntax tree.</param>
    /// <returns>
    /// Each body node with its span. A block body's span runs between its braces. An expression body's span runs
    /// from the end of its <c>=&gt;</c> to the end of its expression. The global statements of a file are one entry
    /// whose node is the compilation unit; its span runs from the start of the first global statement's leading
    /// trivia to the end of the last global statement's trailing trivia.
    /// </returns>
    internal static IEnumerable<(SyntaxNode Body, TextSpan Span)> Find(SyntaxNode root)
    {
        foreach (SyntaxNode node in root.DescendantNodes())
        {
            (SyntaxNode Body, TextSpan Span)? body = node switch
            {
                BlockSyntax block when IsMemberBody(block: block) => (block, BetweenBraces(block: block)),
                ArrowExpressionClauseSyntax arrow => (arrow, AfterArrow(arrow: arrow)),
                AnonymousFunctionExpressionSyntax { ExpressionBody: { } expression } function => (
                        function,
                        TextSpan.FromBounds(start: ArrowEnd(function: function), end: expression.Span.End)),
                _ => null,
            };
            if (body is not null)
            {
                yield return body.Value;
            }
        }

        if (root is CompilationUnitSyntax compilationUnit && FindGlobalStatements(unit: compilationUnit) is { } span)
        {
            yield return (compilationUnit, span);
        }
    }

    private static bool IsMemberBody(BlockSyntax block)
    {
        return block.Parent is BaseMethodDeclarationSyntax
                or AccessorDeclarationSyntax
                or LocalFunctionStatementSyntax
                or AnonymousFunctionExpressionSyntax;
    }

    private static TextSpan BetweenBraces(BlockSyntax block)
    {
        return TextSpan.FromBounds(start: block.OpenBraceToken.Span.End, end: block.CloseBraceToken.SpanStart);
    }

    private static TextSpan AfterArrow(ArrowExpressionClauseSyntax arrow)
    {
        return TextSpan.FromBounds(start: arrow.ArrowToken.Span.End, end: arrow.Expression.Span.End);
    }

    private static int ArrowEnd(AnonymousFunctionExpressionSyntax function)
    {
        return function switch
        {
            SimpleLambdaExpressionSyntax simple => simple.ArrowToken.Span.End,
            ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized.ArrowToken.Span.End,
            _ => function.SpanStart,
        };
    }

    private static TextSpan? FindGlobalStatements(CompilationUnitSyntax unit)
    {
        List<GlobalStatementSyntax> statements = [.. unit.Members.OfType<GlobalStatementSyntax>()];
        if (statements.Count == 0)
        {
            return null;
        }

        return TextSpan.FromBounds(
                start: statements[0].FullSpan.Start,
                end: statements[statements.Count - 1].FullSpan.End);
    }
}
