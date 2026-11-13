using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sebreiro.CodeRules.Analyzers;

/// <summary>
/// SCR0005: no null-forgiving operator anywhere, tests included.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NullForgivingAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the rule.</summary>
    public const string DiagnosticId = "SCR0005";

    /// <summary>The descriptor of the rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "No null-forgiving operator",
            messageFormat: "Replace the null-forgiving operator with an explicit null check",
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
        context.RegisterSyntaxNodeAction(action: AnalyzeSuppression, SyntaxKind.SuppressNullableWarningExpression);
    }

    private static void AnalyzeSuppression(SyntaxNodeAnalysisContext context)
    {
        var suppression = (PostfixUnaryExpressionSyntax)context.Node;
        context.ReportDiagnostic(diagnostic: Diagnostic.Create(
                descriptor: Rule,
                location: suppression.OperatorToken.GetLocation()));
    }
}
