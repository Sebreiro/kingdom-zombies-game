using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sebreiro.CodeRules.Analyzers;

/// <summary>
/// SCR0010: using directives are ordered. Plain usings come first, then <c>using static</c> directives, then
/// aliases; global usings come before all the others, which the compiler also requires. Inside each group,
/// <c>System</c> and <c>System.*</c> come first, then the rest by name, ordinal and ignoring case. Plain and static
/// usings sort by the namespace or type they import, aliases by the alias name. The usings of a compilation unit and
/// of each namespace are checked on their own, and the first directive out of order is reported.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UsingOrderAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the rule.</summary>
    public const string DiagnosticId = "SCR0010";

    /// <summary>The descriptor of the rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Using order: System first, then alphabetical",
            messageFormat: "'{0}' must come before '{1}'",
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
        context.RegisterSyntaxNodeAction(
                action: AnalyzeContainer,
                SyntaxKind.CompilationUnit,
                SyntaxKind.NamespaceDeclaration,
                SyntaxKind.FileScopedNamespaceDeclaration);
    }

    private const int GroupCount = 3;

    private static void AnalyzeContainer(SyntaxNodeAnalysisContext context)
    {
        SyntaxList<UsingDirectiveSyntax> usings = context.Node is CompilationUnitSyntax unit
                ? unit.Usings
                : ((BaseNamespaceDeclarationSyntax)context.Node).Usings;
        for (int index = 1; index < usings.Count; index++)
        {
            UsingDirectiveSyntax previous = usings[index - 1];
            UsingDirectiveSyntax current = usings[index];
            if (Compare(left: previous, right: current) > 0)
            {
                context.ReportDiagnostic(diagnostic: Diagnostic.Create(
                        descriptor: Rule,
                        location: current.GetLocation(),
                        current.ToString(),
                        previous.ToString()));
                return;
            }
        }
    }

    private static int Compare(UsingDirectiveSyntax left, UsingDirectiveSyntax right)
    {
        int byGroup = Group(directive: left) - Group(directive: right);
        if (byGroup != 0)
        {
            return byGroup;
        }

        string leftName = SortName(directive: left);
        string rightName = SortName(directive: right);
        int bySystem = SystemRank(name: leftName) - SystemRank(name: rightName);
        return bySystem != 0
                ? bySystem
                : StringComparer.OrdinalIgnoreCase.Compare(x: leftName, y: rightName);
    }

    private static int Group(UsingDirectiveSyntax directive)
    {
        int kind = directive.Alias is not null
                ? 2
                : directive.StaticKeyword.IsKind(kind: SyntaxKind.StaticKeyword) ? 1 : 0;
        return directive.GlobalKeyword.IsKind(kind: SyntaxKind.GlobalKeyword) ? kind : GroupCount + kind;
    }

    private static string SortName(UsingDirectiveSyntax directive)
    {
        return directive.Alias is { } alias
                ? alias.Name.Identifier.ValueText
                : directive.NamespaceOrType.ToString();
    }

    private static int SystemRank(string name)
    {
        bool isSystem = string.Equals(a: name, b: "System", comparisonType: StringComparison.Ordinal)
                || name.StartsWith(value: "System.", comparisonType: StringComparison.Ordinal);
        return isSystem ? 0 : 1;
    }
}
