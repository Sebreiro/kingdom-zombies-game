using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0005: the null-forgiving operator fails on a variable, before a member access and on the
/// <see langword="null"/> literal, while logical not and <c>!=</c> pass.
/// </summary>
public sealed class NullForgivingAnalyzerTests
{
    /// <summary>The null-forgiving operator on a variable is reported on the operator.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-26.negative")]
    public Task AnalyzeVariableReports()
    {
        return VerifyAsync(caseId: "R-26.negative", diagnostic: "SCR0005", source: """
                #nullable enable
                class C
                {
                    string M(string? value)
                    {
                        return value{|SCR0005:!|};
                    }
                }
                """);
    }

    /// <summary>The null-forgiving operator before a member access is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.nullforgiving.Analyze_BeforeMemberAccess_Reports")]
    public Task AnalyzeBeforeMemberAccessReports()
    {
        return VerifyAsync(caseId: "rules.nullforgiving.Analyze_BeforeMemberAccess_Reports", diagnostic: "SCR0005", source: """
                #nullable enable
                class C
                {
                    int M(string? value)
                    {
                        return value{|SCR0005:!|}.Length;
                    }
                }
                """);
    }

    /// <summary>The null-forgiving operator on the <see langword="null"/> literal is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.nullforgiving.Analyze_NullLiteral_Reports")]
    public Task AnalyzeNullLiteralReports()
    {
        return VerifyAsync(caseId: "rules.nullforgiving.Analyze_NullLiteral_Reports", diagnostic: "SCR0005", source: """
                #nullable enable
                class C
                {
                    public string Name { get; set; } = null{|SCR0005:!|};
                }
                """);
    }

    /// <summary>Logical not and the not-equals operator pass.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-26.positive")]
    public Task AnalyzeLogicalNotAndNotEqualsPass()
    {
        return VerifyAsync(caseId: "R-26.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                #nullable enable
                class C
                {
                    bool M(bool flag, int a, int b)
                    {
                        return !flag && a != b;
                    }
                }
                """);
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source, string? editorConfig = null, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType: "NullForgivingAnalyzer", source: source, editorConfig: editorConfig, outputKind: outputKind);
    }
}
