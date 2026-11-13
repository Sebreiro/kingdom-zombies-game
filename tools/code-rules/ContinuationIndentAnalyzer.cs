using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Sebreiro.CodeRules.Analyzers;

/// <summary>
/// SCR0009: a continuation line of an agreed kind (plan 01-06 <c>## Agreed</c>) is indented 8 spaces past the line
/// it continues. Only the first token of a physical line is checked, and only when it is one of these:
/// <list type="bullet">
/// <item>the first token of a wrapped argument, attribute argument or parameter: 8 past the line holding the
/// list's <c>(</c> or <c>[</c>;</item>
/// <item>a fluent-chain <c>.</c> or <c>?</c>: 8 past the line of the chain's first token;</item>
/// <item>a binary operator, or the right operand after a trailing one: 8 past the left operand's first line;</item>
/// <item>a conditional operator's <c>?</c> or <c>:</c>, or the branch after a trailing one: 8 past the
/// condition's first line;</item>
/// <item>a constructor initializer, a <c>where</c> clause, an expression-bodied member's <c>=&gt;</c> or a base
/// list: 8 past the declaration's first line, attributes excluded;</item>
/// <item>a value after <c>=</c> or <c>return</c>, or a lambda expression body after <c>=&gt;</c>: 8 past the
/// line holding the <c>=</c>, <c>return</c> or <c>=&gt;</c>.</item>
/// </list>
/// Two block layouts are checked as well, because IDE0055 is neutral there: block-lambda braces sit at the indent
/// of the line where the lambda starts, and the brackets of a collection expression that spans lines sit at the
/// indent of the line that opens them, with elements 4 past it (a collection on one line is a plain value).
/// Everything else (initializers, anonymous objects, switch expressions,
/// query clauses, statements, lines inside string literals) is left to IDE0055 or never checked.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ContinuationIndentAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the rule.</summary>
    public const string DiagnosticId = "SCR0009";

    /// <summary>The descriptor of the rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Continuation lines are indented 8 spaces",
            messageFormat: "{0} is indented {1} spaces; expected {2}",
            category: "Sebreiro.CodeRules",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(analysisMode: GeneratedCodeAnalysisFlags.Analyze | 
                GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(action: AnalyzeTree);
    }

    private const int Continuation = 8;

    private const int BlockContent = 4;

    private const int SameIndent = 0;

    private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
    {
        SyntaxNode root = context.Tree.GetRoot(cancellationToken: context.CancellationToken);
        SourceText text = context.Tree.GetText(cancellationToken: context.CancellationToken);
        foreach (SyntaxToken token in root.DescendantTokens())
        {
            if (!StartsLine(text: text, token: token)
                    || IsInsideInterpolatedString(token: token)
                    || Classify(token: token) is not { } expectation)
            {
                continue;
            }

            int actual = Indent(text: text, position: token.SpanStart);
            int expected = Indent(text: text, position: expectation.Anchor.SpanStart) + expectation.Offset;
            if (actual != expected)
            {
                context.ReportDiagnostic(diagnostic: Diagnostic.Create(
                        descriptor: Rule,
                        location: token.GetLocation(),
                        expectation.Kind,
                        actual,
                        expected));
            }
        }
    }

    private static (string Kind, SyntaxToken Anchor, int Offset)? Classify(SyntaxToken token)
    {
        if (token.Parent is not { } parent)
        {
            return null;
        }

        if (parent.GetFirstToken() != token)
        {
            return ClassifyInterior(token: token, parent: parent);
        }

        SyntaxNode node = parent;
        while (node.Parent is { } up && up.GetFirstToken() == token)
        {
            node = up;
        }

        return ClassifyLeading(node: node) ?? ClassifyChild(node: node);
    }

    private static (string Kind, SyntaxToken Anchor, int Offset)? ClassifyInterior(
            SyntaxToken token,
            SyntaxNode parent)
    {
        return parent switch
        {
            MemberAccessExpressionSyntax access when token == access.OperatorToken =>
                    ("Fluent-chain line", ChainStart(expression: access.Expression), Continuation),
            ConditionalAccessExpressionSyntax access when token == access.OperatorToken =>
                    ("Fluent-chain line", ChainStart(expression: access.Expression), Continuation),
            BinaryExpressionSyntax binary when token == binary.OperatorToken =>
                    ("Binary-operator line", binary.Left.GetFirstToken(), Continuation),
            BinaryPatternSyntax pattern when token == pattern.OperatorToken =>
                    ("Binary-operator line", pattern.Left.GetFirstToken(), Continuation),
            ConditionalExpressionSyntax conditional =>
                    ("Conditional-operator line", conditional.Condition.GetFirstToken(), Continuation),
            BlockSyntax { Parent: LambdaExpressionSyntax lambda } block when token == block.CloseBraceToken =>
                    ("Block-lambda brace", lambda.GetFirstToken(), SameIndent),
            CollectionExpressionSyntax collection when token == collection.CloseBracketToken =>
                    ("Collection bracket", collection.OpenBracketToken, SameIndent),
            _ => null,
        };
    }

    private static (string Kind, SyntaxToken Anchor, int Offset)? ClassifyLeading(SyntaxNode node)
    {
        return node switch
        {
            ArgumentSyntax { Parent: BaseArgumentListSyntax list } =>
                    ("Wrapped argument", list.GetFirstToken(), Continuation),
            AttributeArgumentSyntax { Parent: AttributeArgumentListSyntax list } =>
                    ("Wrapped argument", list.GetFirstToken(), Continuation),
            ParameterSyntax { Parent: BaseParameterListSyntax list } =>
                    ("Wrapped parameter", list.GetFirstToken(), Continuation),
            CollectionElementSyntax { Parent: CollectionExpressionSyntax collection } =>
                    ("Collection element", collection.OpenBracketToken, BlockContent),
            BaseListSyntax { Parent: { } type } =>
                    ("Base list", DeclarationStart(declaration: type), Continuation),
            BaseTypeSyntax { Parent: BaseListSyntax { Parent: { } type } } =>
                    ("Base list", DeclarationStart(declaration: type), Continuation),
            TypeParameterConstraintClauseSyntax { Parent: { } declaration } =>
                    ("where clause", DeclarationStart(declaration: declaration), Continuation),
            ConstructorInitializerSyntax { Parent: { } constructor } =>
                    ("Constructor initializer", DeclarationStart(declaration: constructor), Continuation),
            ArrowExpressionClauseSyntax { Parent: { } member } =>
                    ("Expression-body arrow", DeclarationStart(declaration: member), Continuation),
            _ => null,
        };
    }

    private static (string Kind, SyntaxToken Anchor, int Offset)? ClassifyChild(SyntaxNode node)
    {
        return node.Parent switch
        {
            EqualsValueClauseSyntax clause when node == clause.Value =>
                    Value(node: node, anchor: clause.EqualsToken),
            AssignmentExpressionSyntax assignment
                    when node == assignment.Right && assignment.Parent is not InitializerExpressionSyntax =>
                    Value(node: node, anchor: assignment.OperatorToken),
            ReturnStatementSyntax statement when node == statement.Expression =>
                    Value(node: node, anchor: statement.ReturnKeyword),
            BinaryExpressionSyntax binary when node == binary.Right =>
                    ("Binary-operator line", binary.Left.GetFirstToken(), Continuation),
            BinaryPatternSyntax pattern when node == pattern.Right =>
                    ("Binary-operator line", pattern.Left.GetFirstToken(), Continuation),
            ConditionalExpressionSyntax conditional
                    when node == conditional.WhenTrue || node == conditional.WhenFalse =>
                    ("Conditional-operator line", conditional.Condition.GetFirstToken(), Continuation),
            LambdaExpressionSyntax lambda when node == lambda.ExpressionBody =>
                    ("Lambda expression body", lambda.ArrowToken, Continuation),
            LambdaExpressionSyntax lambda when node == lambda.Block =>
                    ("Block-lambda brace", lambda.GetFirstToken(), SameIndent),
            _ => null,
        };
    }

    private static (string Kind, SyntaxToken Anchor, int Offset) Value(SyntaxNode node, SyntaxToken anchor)
    {
        return node is CollectionExpressionSyntax && SpansLines(node: node)
                ? ("Collection bracket", anchor, SameIndent)
                : ("Value line", anchor, Continuation);
    }

    private static bool SpansLines(SyntaxNode node)
    {
        FileLinePositionSpan span = node.GetLocation().GetLineSpan();
        return span.EndLinePosition.Line > span.StartLinePosition.Line;
    }

    private static SyntaxToken ChainStart(ExpressionSyntax expression)
    {
        ExpressionSyntax start = expression;
        while (EnclosingConditionalAccess(binding: start) is { } conditional)
        {
            start = conditional.Expression;
        }

        return start.GetFirstToken();
    }

    private static ConditionalAccessExpressionSyntax? EnclosingConditionalAccess(ExpressionSyntax binding)
    {
        return binding
                .Ancestors()
                .OfType<ConditionalAccessExpressionSyntax>()
                .FirstOrDefault(predicate: candidate => candidate.WhenNotNull.SpanStart == binding.SpanStart);
    }

    private static SyntaxToken DeclarationStart(SyntaxNode declaration)
    {
        foreach (SyntaxNodeOrToken child in declaration.ChildNodesAndTokens())
        {
            if (child.IsKind(kind: SyntaxKind.AttributeList) || child.Span.IsEmpty)
            {
                continue;
            }

            return child.AsNode() is { } node ? node.GetFirstToken() : child.AsToken();
        }

        return declaration.GetFirstToken();
    }

    private static bool StartsLine(SourceText text, SyntaxToken token)
    {
        if (token.Span.IsEmpty)
        {
            return false;
        }

        TextLine line = text.Lines.GetLineFromPosition(position: token.SpanStart);
        return line.Start + Indent(text: text, position: token.SpanStart) == token.SpanStart;
    }

    private static bool IsInsideInterpolatedString(SyntaxToken token)
    {
        return token.Parent is { } parent
                && parent
                        .AncestorsAndSelf()
                        .OfType<InterpolatedStringExpressionSyntax>()
                        .Any(predicate: interpolated => interpolated.StringStartToken != token);
    }

    private static int Indent(SourceText text, int position)
    {
        TextLine line = text.Lines.GetLineFromPosition(position: position);
        int index = line.Start;
        while (index < line.End && text[index] is ' ' or '\t')
        {
            index++;
        }

        return index - line.Start;
    }
}
