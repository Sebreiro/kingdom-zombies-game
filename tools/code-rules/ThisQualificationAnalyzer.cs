using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sebreiro.CodeRules.Analyzers;

/// <summary>
/// SCR0011: members are never qualified with <c>this.</c>. Only a <c>this</c> expression that is the target of a
/// member access (<c>this.X</c>) is reported; constructor chaining (<c>: this(...)</c>), <c>this</c> passed or
/// returned as a value, indexers and the <c>this</c> modifier of an extension method are not. A <c>this.X</c> that
/// binds to an extension method or a C# 14 extension member is not reported either: an extension is not a member
/// of the type, and the call does not bind without <c>this.</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ThisQualificationAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the rule.</summary>
    public const string DiagnosticId = "SCR0011";

    /// <summary>The descriptor of the rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "No this qualification",
            messageFormat: "Remove 'this.' from '{0}'",
            category: "Sebreiro.CodeRules",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(analysisMode: GeneratedCodeAnalysisFlags.Analyze
                | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(action: AnalyzeThis, SyntaxKind.ThisExpression);
    }

    private static void AnalyzeThis(SyntaxNodeAnalysisContext context)
    {
        if (context.Node.Parent is MemberAccessExpressionSyntax access
                && !BindsToExtension(context: context, access: access)
                && !GeneratedSyntax.IsPermitted(diagnostic: DiagnosticId,
                        tree: context.Node.SyntaxTree, span: access.Span))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(
                    descriptor: Rule,
                    location: access.GetLocation(),
                    access.ToString()));
        }
    }

    private static bool BindsToExtension(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax access)
    {
        ISymbol? symbol = context.SemanticModel
                .GetSymbolInfo(expression: access, cancellationToken: context.CancellationToken)
                .Symbol;
        return symbol is IMethodSymbol { IsExtensionMethod: true }
                || symbol is { ContainingType.IsExtension: true };
    }
}
