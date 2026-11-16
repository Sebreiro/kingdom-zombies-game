using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sebreiro.CodeRules.Analyzers;

/// <summary>
/// SCR0012: a local created with <c>new</c> is written <c>var x = new Foo();</c>, never <c>Foo x = new();</c>.
/// A local declaration (including <c>using</c> and <c>await using</c> declarations), a <c>for</c> initializer or a
/// <c>using (...)</c> statement declaration is reported when its type is not <c>var</c>, it has exactly one
/// declarator, and that declarator's value is a target-typed <c>new(...)</c>. Several declarators, which cannot use
/// <c>var</c>, are not reported; neither are fields, properties, arguments, return values or collection expressions.
/// IDE0007 reports <c>Foo x = new Foo();</c> and IDE0090 the fields and properties.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LocalNewDeclarationAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the rule.</summary>
    public const string DiagnosticId = "SCR0012";

    /// <summary>The descriptor of the rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Locals created with new use var",
            messageFormat: "Declare '{0}' as 'var {0} = new {1}(...)'",
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
        context.RegisterSyntaxNodeAction(action: AnalyzeDeclaration, SyntaxKind.VariableDeclaration);
    }

    private static void AnalyzeDeclaration(SyntaxNodeAnalysisContext context)
    {
        var declaration = (VariableDeclarationSyntax)context.Node;
        if (!IsLocal(declaration: declaration) || declaration.Type.IsVar || declaration.Variables.Count != 1)
        {
            return;
        }

        VariableDeclaratorSyntax variable = declaration.Variables[0];
        if (variable.Initializer?.Value is ImplicitObjectCreationExpressionSyntax)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(
                    descriptor: Rule,
                    location: declaration.GetLocation(),
                    variable.Identifier.ValueText,
                    declaration.Type.ToString()));
        }
    }

    private static bool IsLocal(VariableDeclarationSyntax declaration)
    {
        return declaration.Parent is LocalDeclarationStatementSyntax or ForStatementSyntax or UsingStatementSyntax;
    }
}
