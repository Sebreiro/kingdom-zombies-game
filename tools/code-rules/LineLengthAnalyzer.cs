using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Sebreiro.CodeRules.Analyzers;
/// <summary>
/// SCR0007: a line holds at most 120 UTF-16 characters, excluding the line terminator.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LineLengthAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the rule.</summary>
    public const string DiagnosticId = "SCR0007";
    /// <summary>The descriptor of the rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(id: DiagnosticId,
            title: "Line of 120 characters or fewer",
            messageFormat: "Line is {0} characters long; the limit is 120", category: "Sebreiro.CodeRules",
            defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(analysisMode: GeneratedCodeAnalysisFlags.Analyze
                | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(action: AnalyzeTree);
    }

    private const int MaxLength = 120;
    private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
    {
        SourceText text = context.Tree.GetText(cancellationToken: context.CancellationToken);
        foreach (TextLine line in text.Lines)
        {
            if (line.Span.Length <= MaxLength || GeneratedLayout.IsPermitted(tree: context.Tree, text: text,
                    line: line))
            {
                continue;
            }

            var location = Location.Create(syntaxTree: context.Tree, textSpan: TextSpan.FromBounds(start: line
                    .Start + MaxLength, end: line.End));
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: Rule, location: location, line
                    .Span.Length));
        }
    }
}
