using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Sebreiro.CodeRules.Analyzers;

/// <summary>
/// SCR0006: a member body holds at most 60 lines. A block body counts every line strictly between its braces, blank
/// lines included; an expression body counts the physical lines of its expression, so a <c>=&gt;</c> that ends the
/// line before it is not counted; the global statements of a file count the inclusive line span from the first
/// statement to the last. Every body that <see cref="MemberBodies"/> finds is checked on its own, so a local
/// function is checked by itself and its lines also count toward its enclosing member.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BodyLengthAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the rule.</summary>
    public const string DiagnosticId = "SCR0006";

    /// <summary>The descriptor of the rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Member body of 60 lines or fewer",
            messageFormat: "Body is {0} lines long; the limit is 60",
            category: "Sebreiro.CodeRules",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(analysisMode: GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(action: AnalyzeTree);
    }

    private const int MaxLines = 60;

    private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
    {
        SyntaxNode root = context.Tree.GetRoot(cancellationToken: context.CancellationToken);
        SourceText text = context.Tree.GetText(cancellationToken: context.CancellationToken);
        foreach ((SyntaxNode body, TextSpan span) in MemberBodies.Find(root: root))
        {
            int lines = CountLines(text: text, body: body, span: span);
            if (lines > MaxLines)
            {
                context.ReportDiagnostic(diagnostic: Diagnostic.Create(
                        descriptor: Rule,
                        location: NameToken(body: body).GetLocation(),
                        lines));
            }
        }
    }

    private static int CountLines(SourceText text, SyntaxNode body, TextSpan span)
    {
        return body switch
        {
            BlockSyntax block => LineOf(text: text, position: block.CloseBraceToken.SpanStart)
                    - LineOf(text: text, position: block.OpenBraceToken.SpanStart)
                    - 1,
            CompilationUnitSyntax unit => InclusiveLines(text: text, span: GlobalStatementsSpan(unit: unit)),
            ArrowExpressionClauseSyntax arrow => InclusiveLines(text: text, span: arrow.Expression.Span),
            AnonymousFunctionExpressionSyntax { ExpressionBody: { } expression } =>
                    InclusiveLines(text: text, span: expression.Span),
            _ => InclusiveLines(text: text, span: span),
        };
    }

    private static TextSpan GlobalStatementsSpan(CompilationUnitSyntax unit)
    {
        GlobalStatementSyntax[] statements = [.. unit.Members.OfType<GlobalStatementSyntax>()];
        return TextSpan.FromBounds(start: statements[0].SpanStart, end: statements[statements.Length - 1].Span.End);
    }

    private static int InclusiveLines(SourceText text, TextSpan span)
    {
        return LineOf(text: text, position: span.End) - LineOf(text: text, position: span.Start) + 1;
    }

    private static int LineOf(SourceText text, int position)
    {
        return text.Lines.GetLineFromPosition(position: position).LineNumber;
    }

    private static SyntaxToken NameToken(SyntaxNode body)
    {
        SyntaxNode owner = body switch
        {
            CompilationUnitSyntax unit => unit.Members.OfType<GlobalStatementSyntax>().First(),
            AnonymousFunctionExpressionSyntax function => function,
            _ => body.Parent ?? body,
        };
        return owner switch
        {
            MethodDeclarationSyntax method => method.Identifier,
            ConstructorDeclarationSyntax constructor => constructor.Identifier,
            DestructorDeclarationSyntax destructor => destructor.Identifier,
            OperatorDeclarationSyntax @operator => @operator.OperatorToken,
            ConversionOperatorDeclarationSyntax conversion => conversion.OperatorKeyword,
            AccessorDeclarationSyntax accessor => accessor.Keyword,
            LocalFunctionStatementSyntax localFunction => localFunction.Identifier,
            PropertyDeclarationSyntax property => property.Identifier,
            IndexerDeclarationSyntax indexer => indexer.ThisKeyword,
            _ => owner.GetFirstToken(),
        };
    }
}
