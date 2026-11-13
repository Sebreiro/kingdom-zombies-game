using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0010: plain using directives run <c>System</c> and <c>System.*</c> first, then by
/// namespace name (ordinal, ignoring case); <c>using static</c> directives follow in the same order, and aliases
/// come last, ordered by alias name. Global usings come before the others. The first directive out of order is
/// reported, once per compilation unit and once per namespace.
/// </summary>
public sealed class UsingOrderAnalyzerTests
{
    /// <summary><c>System</c> and <c>System.*</c> first, then the other namespaces alphabetically, passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-14.positive")]
    public Task AnalyzeSystemFirstThenAlphabeticalPasses()
    {
        return VerifyAsync(caseId: "R-14.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                using System;
                using System.Collections.Generic;
                using System.Text;
                using Alpha.Beta;
                using Alpha.Gamma;
                using Microsoft.Tools;
                using SystemTools;
                """ + Namespaces);
    }

    /// <summary><c>using System.Text;</c> before <c>using System;</c> is reported on <c>using System;</c>.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-14.negative")]
    public Task AnalyzeSystemTextBeforeSystemReports()
    {
        return VerifyAsync(caseId: "R-14.negative", diagnostic: "SCR0010", source: """
                using System.Text;
                {|SCR0010:using System;|}
                """ + Namespaces);
    }

    /// <summary>A <c>Microsoft.*</c> using before a <c>System.*</c> using is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_MicrosoftBeforeSystem_Reports")]
    public Task AnalyzeMicrosoftBeforeSystemReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_MicrosoftBeforeSystem_Reports", diagnostic: 
                "SCR0010", source: """
                using Microsoft.Tools;
                {|SCR0010:using System.Text;|}
                """ + Namespaces);
    }

    /// <summary><c>using Alpha.Gamma;</c> before <c>using Alpha.Beta;</c> is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_NonSystemOutOfOrder_Reports")]
    public Task AnalyzeNonSystemOutOfOrderReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_NonSystemOutOfOrder_Reports", diagnostic: "SCR0010"
                , source: """
                using System;
                using Alpha.Gamma;
                {|SCR0010:using Alpha.Beta;|}
                """ + Namespaces);
    }

    /// <summary>
    /// <c>SystemTools</c> is not a <c>System</c> namespace: it sorts with the others, so it may follow
    /// <c>Alpha.Beta</c>.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.SystemPrefixWithoutDot_SortsWithOthers_Passes")]
    public Task SystemPrefixWithoutDotSortsWithOthersPasses()
    {
        return VerifyAsync(caseId: "rules.usingorder.SystemPrefixWithoutDot_SortsWithOthers_Passes", diagnostic
                : "ANALYZER_ACCEPTED", source: """
                using Alpha.Beta;
                using SystemTools;
                """ + Namespaces);
    }

    /// <summary>The comparison ignores case: <c>alpha</c> before <c>Beta</c> passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_CaseIgnored_Passes")]
    public Task AnalyzeCaseIgnoredPasses()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_CaseIgnored_Passes", diagnostic: 
                "ANALYZER_ACCEPTED", source: """
                using alpha;
                using Beta;
                """ + LowerCaseNamespaces);
    }

    /// <summary>The comparison ignores case: <c>Beta</c> before <c>alpha</c> is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_CaseIgnored_Reports")]
    public Task AnalyzeCaseIgnoredReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_CaseIgnored_Reports", diagnostic: "SCR0010", source: """
                using Beta;
                {|SCR0010:using alpha;|}
                """ + LowerCaseNamespaces);
    }

    /// <summary>
    /// Plain usings, then <c>using static</c> directives, then aliases, each group in the same order, passes.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.StaticAfterPlain_AliasesLast_Passes")]
    public Task StaticAfterPlainAliasesLastPasses()
    {
        return VerifyAsync(caseId: "rules.usingorder.StaticAfterPlain_AliasesLast_Passes", diagnostic: 
                "ANALYZER_ACCEPTED", source: """
                using System;
                using Alpha.Beta;
                using static System.Math;
                using static Alpha.Beta.Helpers;
                using Builder = System.Text.StringBuilder;
                using Helpers = Alpha.Gamma.Helpers;
                """ + Namespaces);
    }

    /// <summary>A <c>using static</c> directive before a plain one is reported on the plain one.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_StaticBeforePlain_Reports")]
    public Task AnalyzeStaticBeforePlainReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_StaticBeforePlain_Reports", diagnostic: "SCR0010", 
                source: """
                using static System.Math;
                {|SCR0010:using System;|}
                """ + Namespaces);
    }

    /// <summary>An alias before a <c>using static</c> directive is reported on the static one.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_AliasBeforeStatic_Reports")]
    public Task AnalyzeAliasBeforeStaticReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_AliasBeforeStatic_Reports", diagnostic: "SCR0010", 
                source: """
                using System;
                using Builder = System.Text.StringBuilder;
                {|SCR0010:using static System.Math;|}
                """ + Namespaces);
    }

    /// <summary>Inside the static group, <c>System.*</c> types come first.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_StaticGroupOutOfOrder_Reports")]
    public Task AnalyzeStaticGroupOutOfOrderReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_StaticGroupOutOfOrder_Reports", diagnostic: 
                "SCR0010", source: """
                using static Alpha.Beta.Helpers;
                {|SCR0010:using static System.Math;|}
                """ + Namespaces);
    }

    /// <summary>Aliases are ordered by alias name: <c>Helpers</c> before <c>Builder</c> is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_AliasesOutOfOrder_Reports")]
    public Task AnalyzeAliasesOutOfOrderReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_AliasesOutOfOrder_Reports", diagnostic: "SCR0010", 
                source: """
                using Helpers = Alpha.Gamma.Helpers;
                {|SCR0010:using Builder = System.Text.StringBuilder;|}
                """ + Namespaces);
    }

    /// <summary>Global usings come before the others, which the compiler also requires.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_GlobalUsingsFirst_Pass")]
    public Task AnalyzeGlobalUsingsFirstPass()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_GlobalUsingsFirst_Pass", diagnostic: 
                "ANALYZER_ACCEPTED", source: """
                global using System.Text;
                global using Alpha.Beta;
                using System;
                using Alpha.Gamma;
                """ + Namespaces);
    }

    /// <summary>Global usings are ordered among themselves.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_GlobalUsingsOutOfOrder_Report")]
    public Task AnalyzeGlobalUsingsOutOfOrderReport()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_GlobalUsingsOutOfOrder_Report", diagnostic: 
                "SCR0010", source: """
                global using Alpha.Beta;
                {|SCR0010:global using System.Text;|}
                using System;
                """ + Namespaces);
    }

    /// <summary>Only the first directive out of order in a list is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_OnlyFirstViolation_Reports")]
    public Task AnalyzeOnlyFirstViolationReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_OnlyFirstViolation_Reports", diagnostic: "SCR0010"
                , source: """
                using Microsoft.Tools;
                {|SCR0010:using Alpha.Gamma;|}
                using Alpha.Beta;
                """ + Namespaces);
    }

    /// <summary>
    /// The usings of a namespace block are checked on their own: each list is in order, so nothing is reported even
    /// though the namespace's <c>System</c> using follows the file's <c>Alpha</c> using.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.NamespaceBlock_CheckedOnItsOwn_Passes")]
    public Task NamespaceBlockCheckedOnItsOwnPasses()
    {
        return VerifyAsync(caseId: "rules.usingorder.NamespaceBlock_CheckedOnItsOwn_Passes", diagnostic: 
                "ANALYZER_ACCEPTED", source: """
                using Alpha.Beta;

                namespace Outer
                {
                    using System;
                    using System.Text;

                    class C
                    {
                    }
                }
                """ + Namespaces);
    }

    /// <summary>A namespace block's usings out of order are reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_NamespaceBlockOutOfOrder_Reports")]
    public Task AnalyzeNamespaceBlockOutOfOrderReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_NamespaceBlockOutOfOrder_Reports", diagnostic: 
                "SCR0010", source: """
                namespace Outer
                {
                    using System.Text;
                    {|SCR0010:using System;|}

                    class C
                    {
                    }
                }
                """ + Namespaces);
    }

    /// <summary>The usings inside a file-scoped namespace are checked too.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_FileScopedNamespaceOutOfOrder_Reports")]
    public Task AnalyzeFileScopedNamespaceOutOfOrderReports()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_FileScopedNamespaceOutOfOrder_Reports", diagnostic
                : "SCR0010", source: """
                namespace Outer;

                using System.Text;
                {|SCR0010:using System;|}

                class C
                {
                }
                """);
    }

    /// <summary>The message names the directive out of order and the one it must come before.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_Message_NamesBothDirectives")]
    public Task AnalyzeMessageNamesBothDirectives()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_Message_NamesBothDirectives", diagnostic: "SCR0010", 
                source: """
                        using System.Text;
                        {|SCR0010:using System;|}
                        """,
                message: "'using System;' must come before 'using System.Text;'");
    }

    /// <summary>Generated settings cannot exempt handwritten code.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.usingorder.Analyze_GeneratedCode_IsSkipped")]
    public Task AnalyzeGeneratedCodeIsSkipped()
    {
        return VerifyAsync(caseId: "rules.usingorder.Analyze_GeneratedCode_IsSkipped", diagnostic: "SCR0010", 
                source: """
                        using System.Text;
                        {|SCR0010:using System;|}
                        """,
                editorConfig: """
                        root = true

                        [*.cs]
                        generated_code = true
                        """);
    }

    private const string Namespaces = """


            namespace Alpha.Beta
            {
                public static class Helpers
                {
                }
            }

            namespace Alpha.Gamma
            {
                public static class Helpers
                {
                }
            }

            namespace Microsoft.Tools
            {
                public static class Helpers
                {
                }
            }

            namespace SystemTools
            {
                public static class Helpers
                {
                }
            }
            """;

    private const string LowerCaseNamespaces = """


            namespace alpha
            {
                public static class Helpers
                {
                }
            }

            namespace Beta
            {
                public static class Helpers
                {
                }
            }
            """;

    private static Task VerifyAsync(string caseId, string diagnostic, string source, string? editorConfig = null
            , string? message = null, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType: 
                "UsingOrderAnalyzer", source: source, editorConfig: editorConfig, message: message, outputKind: 
                outputKind);
    }
}
