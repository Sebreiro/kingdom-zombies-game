using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Sebreiro.CodeRules.Analyzers;

/// <summary>
/// SCR0001: every argument at every call site is named, except for the exemptions agreed in the 01-06 spike:
/// E1 <c>params</c> elements, E2 extension receivers, E3 attribute arguments, E4 indexer arguments, E5 delegate
/// and function-pointer invocations, E7 <c>nameof</c>, tuples, <c>typeof</c> and <c>default</c> (which are not
/// arguments). Generated-code markers do not exempt handwritten code; observed tool compatibility is symbol-scoped.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NamedArgumentAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the named-argument rule.</summary>
    public const string DiagnosticId = "SCR0001";

    /// <summary>The descriptor of the named-argument rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Use a named argument",
            messageFormat: "Argument for parameter '{0}' must be named",
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
        context.RegisterOperationAction(action: AnalyzeArgument, OperationKind.Argument);
    }

    private static void AnalyzeArgument(OperationAnalysisContext context)
    {
        var argument = (IArgumentOperation)context.Operation;
        if (argument.IsImplicit
                || argument.Parameter is null
                || argument.Syntax is AttributeArgumentSyntax
                || argument.Syntax is not ArgumentSyntax syntax
                || syntax.NameColon is not null
                || IsExempt(argument: argument, syntax: syntax)
                || GeneratedSyntax.IsPermitted(diagnostic: DiagnosticId, tree: syntax.SyntaxTree, span: syntax.Span))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(
                descriptor: Rule,
                location: syntax.GetLocation(),
                argument.Parameter.Name));
    }

    private static bool IsExempt(IArgumentOperation argument, ArgumentSyntax syntax)
    {
        return argument.Parent is { IsImplicit: true }
                || argument.ArgumentKind is ArgumentKind.ParamArray
                        or ArgumentKind.ParamCollection
                        or ArgumentKind.DefaultValue
                || syntax.Parent is BracketedArgumentListSyntax
                || argument.Parent is IInvocationOperation { TargetMethod.MethodKind: MethodKind.DelegateInvoke }
                        or IFunctionPointerInvocationOperation;
    }
}
