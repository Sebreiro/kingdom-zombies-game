using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Microsoft.CodeAnalysis;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0002: a <c>//</c>, <c>/* */</c> or <c>///</c> comment between a member body's braces
/// fails in every kind of body, the global statements of a file form one body, and XML docs, comments between
/// members and preprocessor directives pass.
/// </summary>
public sealed class BodyCommentAnalyzerTests
{
    /// <summary>A line comment and a block comment inside a method body are reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-36.negative")]
    public Task MethodLineAndBlockCommentsReport()
    {
        return VerifyAsync(caseId: "R-36.negative", diagnostic: "SCR0002", source: """
                class C
                {
                    void M()
                    {
                        {|SCR0002:// line|}
                        {|SCR0002:/* block */|}
                    }
                }
                """);
    }

    /// <summary>A comment after a statement on the same line is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.Method_TrailingComment_Reports")]
    public Task MethodTrailingCommentReports()
    {
        return VerifyAsync(caseId: "rules.bodycomment.Method_TrailingComment_Reports", diagnostic: "SCR0002",
                source: """
                class C
                {
                    int M()
                    {
                        int x = 1; {|SCR0002:// trailing|}
                        return x {|SCR0002:/* inline */|} + 1;
                    }
                }
                """);
    }

    /// <summary>A comment inside a constructor and a destructor is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.ConstructorAndDestructor_Comments_Report")]
    public Task ConstructorAndDestructorCommentsReport()
    {
        return VerifyAsync(caseId: "rules.bodycomment.ConstructorAndDestructor_Comments_Report", diagnostic:
                "SCR0002", source: """
                class C
                {
                    C()
                    {
                        {|SCR0002:// constructor|}
                    }

                    ~C()
                    {
                        {|SCR0002:// destructor|}
                    }
                }
                """);
    }

    /// <summary>A comment inside property, indexer and event accessors and an expression body is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.Accessors_Comments_Report")]
    public Task AccessorsCommentsReport()
    {
        return VerifyAsync(caseId: "rules.bodycomment.Accessors_Comments_Report", diagnostic: "SCR0002", source: """
                using System;

                class C
                {
                    private int _value;

                    int Value
                    {
                        get
                        {
                            {|SCR0002:// getter|}
                            return _value;
                        }

                        set { _value = value; {|SCR0002:/* setter */|} }
                    }

                    int this[int i] => i {|SCR0002:/* indexer */|} + 1;

                    int Twice => _value {|SCR0002:/* property */|} * 2;

                    event EventHandler Changed
                    {
                        add { {|SCR0002:// add|}
                        }

                        remove { {|SCR0002:// remove|}
                        }
                    }
                }
                """);
    }

    /// <summary>A comment inside an operator and a conversion operator is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.Operators_Comments_Report")]
    public Task OperatorsCommentsReport()
    {
        return VerifyAsync(caseId: "rules.bodycomment.Operators_Comments_Report", diagnostic: "SCR0002", source: """
                class C
                {
                    public static C operator +(C left, C right)
                    {
                        {|SCR0002:// operator|}
                        return left;
                    }

                    public static implicit operator int(C value)
                    {
                        {|SCR0002:// conversion|}
                        return 0;
                    }
                }
                """);
    }

    /// <summary>A comment inside a local function and inside lambdas is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.LocalFunctionAndLambda_Comments_Report")]
    public Task LocalFunctionAndLambdaCommentsReport()
    {
        return VerifyAsync(caseId: "rules.bodycomment.LocalFunctionAndLambda_Comments_Report", diagnostic:
                "SCR0002", source: """
                using System;

                class C
                {
                    int M()
                    {
                        Func<int, int> f = x =>
                        {
                            {|SCR0002:// block lambda|}
                            return x;
                        };
                        return Local() + f(1);

                        int Local()
                        {
                            {|SCR0002:// local function|}
                            return 1;
                        }
                    }
                }
                """);
    }

    /// <summary>A comment inside a lambda in a field initializer is reported: the lambda is a body.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.FieldInitializerLambda_Comment_Reports")]
    public Task FieldInitializerLambdaCommentReports()
    {
        return VerifyAsync(caseId: "rules.bodycomment.FieldInitializerLambda_Comment_Reports", diagnostic:
                "SCR0002", source: ("using System;\n\nclass C\n{\n    private static readonly Fu" +
                        "nc<int, int> _block = x =>\n    {\n        {|SCR0002:// b" +
                        "lock|}\n        return x;\n    };\n\n    private static rea" +
                        "donly Func<int, int> _expression = x => x {|SCR0002:/* " +
                        "expression */|} + 1;\n}"));
    }

    /// <summary>A <c>///</c> comment inside a method body is a body comment.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.Method_DocumentationComment_Reports")]
    public Task MethodDocumentationCommentReports()
    {
        return VerifyAsync(caseId: "rules.bodycomment.Method_DocumentationComment_Reports", diagnostic:
                "SCR0002", source: """
                class C
                {
                    int M()
                    {
                        {|SCR0002:/// <summary>Local.</summary>|}
                        int Local()
                        {
                            return 1;
                        }

                        return Local();
                    }
                }
                """);
    }

    /// <summary>XML docs on members and comments between members, before and after bodies pass.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-36.positive")]
    public Task AnalyzeCommentsOutsideBodiesPass()
    {
        return VerifyAsync(caseId: "R-36.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                // File header.
                using System;

                /// <summary>A type.</summary>
                class C
                {
                    // Between members.
                    private int _value = 1; // After a field.

                    /// <summary>A method.</summary>
                    /** <remarks>Block XML doc.</remarks> */
                    int M()
                    {
                        return _value;
                    } // After the closing brace.

                    /* Between members. */
                    int P { get; set; }
                }
                """);
    }

    /// <summary>
    /// <c>#if</c>, <c>#endif</c>, <c>#region</c> and <c>#endregion</c> inside a body are not comments.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.PreprocessorDirectives_InBody_Pass")]
    public Task PreprocessorDirectivesInBodyPass()
    {
        return VerifyAsync(caseId: "rules.bodycomment.PreprocessorDirectives_InBody_Pass", diagnostic:
                "ANALYZER_ACCEPTED", source: """
                class C
                {
                    int M()
                    {
                #if DEBUG
                        int x = 1;
                #else
                        int x = 2;
                #endif
                        #region Values
                        int y = x;
                        #endregion
                        return y;
                    }
                }
                """);
    }

    /// <summary>
    /// The global statements of a file are one body: comments before, between and after them report.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.GlobalStatements_Comments_Report")]
    public Task GlobalStatementsCommentsReport()
    {
        return VerifyAsync(caseId: "rules.bodycomment.GlobalStatements_Comments_Report", diagnostic: "SCR0002",
                source: """
                        using System;

                        {|SCR0002:// Before the first statement.|}
                        int x = 1;
                        {|SCR0002:/* Between statements. */|}
                        _ = x + 1; {|SCR0002:// After the last statement.|}
                        """,
                outputKind: OutputKind.ConsoleApplication);
    }

    /// <summary>
    /// A comment after the last global statement, before a type, is outside the global-statement body.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.GlobalStatements_CommentBeforeType_Passes")]
    public Task GlobalStatementsCommentBeforeTypePasses()
    {
        return VerifyAsync(caseId: "rules.bodycomment.GlobalStatements_CommentBeforeType_Passes", diagnostic:
                "ANALYZER_ACCEPTED",
                source: """
                        _ = 1;

                        // Before the type.
                        class C
                        {
                        }
                        """,
                outputKind: OutputKind.ConsoleApplication);
    }

    /// <summary>Code marked <c>generated_code = true</c> in an <c>.editorconfig</c> cannot exempt a
    /// handwritten subject.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodycomment.GeneratedConfigurationCannotExemptHandwrittenCode")]
    public Task GeneratedConfigurationCannotExemptHandwrittenCode()
    {
        return VerifyAsync(caseId: "rules.bodycomment.GeneratedConfigurationCannotExemptHandwrittenCode",
                diagnostic: "SCR0002",
                source: """
                        class C
                        {
                            void M()
                            {
                                {|SCR0002:// Generated.|}
                            }
                        }
                        """,
                editorConfig: """
                        root = true

                        [*.cs]
                        generated_code = true
                        """);
    }

    /// <summary>Block XML docs and anonymous-method comments are checked once, including CRLF and first-line
    /// spans.</summary>
    [Fact, Trait("Case", "rules.bodycomment.block-doc-anonymous")]
    public Task BlockDocumentationAndAnonymousMethodsAreChecked()
    {
        return VerifyAsync(caseId: "rules.bodycomment.block-doc-anonymous", diagnostic: "SCR0002", source: (
                "class C\n{\n    static int Run()\n    {\n        {|SCR0002:" +
                        "/** <summary>Local.</summary>|}\n         */\n        int" +
                        " Local() { return 1; }\n        System.Func<int> f = del" +
                        "egate { {|SCR0002:/* body */|} return Local(); };\n     " +
                        "   return f();\n    }\n}").Replace(oldValue: "\n", newValue: "\r\n", comparisonType: System.
                        StringComparison.Ordinal));
    }

    private static Task VerifyAsync(
            string caseId, string diagnostic, string source,
            string? editorConfig = null,
            OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType:
                "BodyCommentAnalyzer",
                source: source,
                editorConfig: editorConfig,
                outputKind: outputKind);
    }
}
