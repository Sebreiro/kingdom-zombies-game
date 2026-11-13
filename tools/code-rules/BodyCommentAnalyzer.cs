using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Sebreiro.CodeRules.Analyzers;

/// <summary>
/// SCR0002: no comment inside a member body, including <c>///</c> and the global statements of a file. All four
/// comment trivia kinds are checked. Preprocessor directives are directive trivia, not comments, so they never
/// report here.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BodyCommentAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the body-comment rule.</summary>
    public const string DiagnosticId = "SCR0002";

    /// <summary>The descriptor of the body-comment rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "No comments inside member bodies",
            messageFormat: "Move this comment out of the member body or delete it",
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

    private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
    {
        SyntaxNode root = context.Tree.GetRoot(cancellationToken: context.CancellationToken);
        List<TextSpan> bodies = [.. MemberBodies.Find(root: root).Select(selector: body => body.Span)];
        if (bodies.Count == 0)
        {
            return;
        }

        SourceText text = context.Tree.GetText(cancellationToken: context.CancellationToken);
        IEnumerable<SyntaxTrivia> comments = root.DescendantTrivia()
                .Where(predicate: IsComment)
                .Where(predicate: trivia => bodies.Any(predicate: body => body.Contains(span: trivia.FullSpan)));
        foreach (SyntaxTrivia comment in comments)
        {
            var location = Location.Create(
                    syntaxTree: context.Tree,
                    textSpan: FirstLine(text: text, comment: comment));
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: Rule, location: location));
        }
    }

    private static bool IsComment(SyntaxTrivia trivia)
    {
        return trivia.IsKind(kind: SyntaxKind.SingleLineCommentTrivia)
                || trivia.IsKind(kind: SyntaxKind.MultiLineCommentTrivia)
                || trivia.IsKind(kind: SyntaxKind.SingleLineDocumentationCommentTrivia)
                || trivia.IsKind(kind: SyntaxKind.MultiLineDocumentationCommentTrivia);
    }

    private static TextSpan FirstLine(SourceText text, SyntaxTrivia comment)
    {
        int start = comment.FullSpan.Start;
        int lineEnd = text.Lines.GetLineFromPosition(position: start).End;
        return TextSpan.FromBounds(start: start, end: Math.Min(val1: comment.FullSpan.End, val2: lineEnd));
    }
}
