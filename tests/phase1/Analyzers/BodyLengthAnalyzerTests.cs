using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using KingdomZombies.Phase1.Tests.Verification;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0006: a body of 60 lines passes and one of 61 lines fails, counting every line between
/// the braces (blank lines included) or every physical line of an expression body, for methods, constructors,
/// accessors, operators, local functions and the global statements of a file. Long bodies are generated in code.
/// </summary>
public sealed class BodyLengthAnalyzerTests
{
    /// <summary>A method body of exactly 60 lines between its braces passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.body.60")]
    public Task Method60LinesPasses()
    {
        return VerifyAsync(caseId: "rules.body.60", diagnostic: "ANALYZER_ACCEPTED", source: $$"""
                class C
                {
                    private int _x;

                    void M()
                    {
                {{Repeat(line: Statement, count: 60)}}
                    }
                }
                """);
    }

    /// <summary>A method body of 61 lines between its braces fails, reported on the method name.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.body.61")]
    public Task Method61LinesFails()
    {
        return VerifyAsync(caseId: "rules.body.61", diagnostic: "SCR0006", source: $$"""
                class C
                {
                    private int _x;

                    void {|SCR0006:M|}()
                    {
                {{Repeat(line: Statement, count: 61)}}
                    }
                }
                """);
    }

    /// <summary>Blank lines count: one statement and 60 blank lines make a 61-line body, which fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-37.negative")]
    public Task MethodBlankLinesCount61LinesFails()
    {
        return VerifyAsync(caseId: "R-37.negative", diagnostic: "SCR0006", source: $$"""
                class C
                {
                    private int _x;

                    void {|SCR0006:M|}()
                    {
                {{Statement}}
                {{Repeat(line: string.Empty, count: 60)}}
                    }
                }
                """);
    }

    /// <summary>An empty body passes, on one line or with the braces on their own lines.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-37.positive")]
    public Task AnalyzeEmptyBodyPasses()
    {
        return VerifyAsync(caseId: "R-37.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                class C
                {
                    void M() {}

                    void N()
                    {
                    }
                }
                """);
    }

    /// <summary>An expression-bodied property of 60 physical lines passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.ExpressionBody_60Lines_Passes")]
    public Task ExpressionBody60LinesPasses()
    {
        return VerifyAsync(caseId: "rules.bodylength.ExpressionBody_60Lines_Passes", diagnostic:
                "ANALYZER_ACCEPTED", source: $$"""
                class C
                {
                    int P => 0
                {{Repeat(line: "            + 1", count: 59)}};
                }
                """);
    }

    /// <summary>An expression-bodied property spanning 61 physical lines fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.ExpressionBody_61Lines_Fails")]
    public Task ExpressionBody61LinesFails()
    {
        return VerifyAsync(caseId: "rules.bodylength.ExpressionBody_61Lines_Fails", diagnostic: "SCR0006", source: $$"""
                class C
                {
                    int {|SCR0006:P|} => 0
                {{Repeat(line: "            + 1", count: 60)}};
                }
                """);
    }

    /// <summary>
    /// An expression-bodied property whose 60-line value starts on the line after its <c>=&gt;</c> passes: the
    /// <c>=&gt;</c> line is not counted.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.ExpressionBodyAfterArrow_60Lines_Passes")]
    public Task ExpressionBodyAfterArrow60LinesPasses()
    {
        return VerifyAsync(caseId: "rules.bodylength.ExpressionBodyAfterArrow_60Lines_Passes", diagnostic:
                "ANALYZER_ACCEPTED", source: $$"""
                class C
                {
                    int P =>
                            0
                {{Repeat(line: "            + 1", count: 59)}};
                }
                """);
    }

    /// <summary>
    /// A lambda whose 60-line expression body starts on the line after its <c>=&gt;</c> passes: the <c>=&gt;</c>
    /// line is not counted.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.LambdaAfterArrow_60Lines_Passes")]
    public Task LambdaAfterArrow60LinesPasses()
    {
        return VerifyAsync(caseId: "rules.bodylength.LambdaAfterArrow_60Lines_Passes", diagnostic:
                "ANALYZER_ACCEPTED", source: $$"""
                using System;

                class C
                {
                    private Func<int, int> _f = v =>
                            v
                {{Repeat(line: "            + 1", count: 59)}};
                }
                """);
    }

    /// <summary>
    /// A lambda whose 61-line expression body starts on the line after its <c>=&gt;</c> fails, reported on the
    /// lambda's first token.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.LambdaAfterArrow_61Lines_Fails")]
    public Task LambdaAfterArrow61LinesFails()
    {
        return VerifyAsync(caseId: "rules.bodylength.LambdaAfterArrow_61Lines_Fails", diagnostic: "SCR0006",
                source: $$"""
                using System;

                class C
                {
                    private Func<int, int> _f = {|SCR0006:v|} =>
                            v
                {{Repeat(line: "            + 1", count: 60)}};
                }
                """);
    }

    /// <summary>A constructor body of 61 lines fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.Constructor_61Lines_Fails")]
    public Task Constructor61LinesFails()
    {
        return VerifyAsync(caseId: "rules.bodylength.Constructor_61Lines_Fails", diagnostic: "SCR0006", source: $$"""
                class C
                {
                    private int _x;

                    {|SCR0006:C|}()
                    {
                {{Repeat(line: Statement, count: 61)}}
                    }
                }
                """);
    }

    /// <summary>A property accessor body of 61 lines fails, reported on the accessor keyword.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.Accessor_61Lines_Fails")]
    public Task Accessor61LinesFails()
    {
        return VerifyAsync(caseId: "rules.bodylength.Accessor_61Lines_Fails", diagnostic: "SCR0006", source: $$"""
                class C
                {
                    private int _x;

                    int P
                    {
                        {|SCR0006:get|}
                        {
                {{Repeat(line: Statement, count: 60)}}
                            return _x;
                        }
                    }
                }
                """);
    }

    /// <summary>An operator body of 61 lines fails, reported on the operator token.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.Operator_61Lines_Fails")]
    public Task Operator61LinesFails()
    {
        return VerifyAsync(caseId: "rules.bodylength.Operator_61Lines_Fails", diagnostic: "SCR0006", source: $$"""
                class C
                {
                    private int _x;

                    public static C operator {|SCR0006:+|}(C left, C right)
                    {
                {{Repeat(line: "        left._x++;", count: 60)}}
                        return right;
                    }
                }
                """);
    }

    /// <summary>
    /// A local function body of 61 lines fails on its own, and its lines make the enclosing method fail as well.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.LocalFunction_61Lines_Fails")]
    public Task LocalFunction61LinesFails()
    {
        return VerifyAsync(caseId: "rules.bodylength.LocalFunction_61Lines_Fails", diagnostic: "SCR0006", source: $$"""
                class C
                {
                    private int _x;

                    void {|SCR0006:M|}()
                    {
                        Local();

                        void {|SCR0006:Local|}()
                        {
                {{Repeat(line: Statement, count: 61)}}
                        }
                    }
                }
                """);
    }

    /// <summary>
    /// A 40-line local function inside a method with 30 other lines passes on its own, but its lines count toward
    /// the enclosing method, which is then over 60 lines and fails.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.Analyze_LocalFunction_CountsTowardEnclosingMember")]
    public Task AnalyzeLocalFunctionCountsTowardEnclosingMember()
    {
        return VerifyAsync(caseId: "rules.bodylength.Analyze_LocalFunction_CountsTowardEnclosingMember",
                diagnostic: "SCR0006", source: $$"""
                class C
                {
                    private int _x;

                    void {|SCR0006:M|}()
                    {
                {{Repeat(line: Statement, count: 25)}}
                        Local();

                        void Local()
                        {
                {{Repeat(line: Statement, count: 40)}}
                        }
                    }
                }
                """);
    }

    /// <summary>The global statements of a file form one body: an inclusive span of 60 lines passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.GlobalStatements_60Lines_Passes")]
    public Task GlobalStatements60LinesPasses()
    {
        return VerifyAsync(caseId: "rules.bodylength.GlobalStatements_60Lines_Passes", diagnostic: "ANALYZER_ACCEPTED",
                source: $$"""
                        int x = 0;
                        {{Repeat(line: "x++;", count: 59)}}
                        """,
                outputKind: OutputKind.ConsoleApplication);
    }

    /// <summary>
    /// The global statements of a file spanning 61 lines fail, reported on the first token of the first statement.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.GlobalStatements_61Lines_Fails")]
    public Task GlobalStatements61LinesFails()
    {
        return VerifyAsync(caseId: "rules.bodylength.GlobalStatements_61Lines_Fails", diagnostic: "SCR0006",
                source: $$"""
                        {|SCR0006:int|} x = 0;
                        {{Repeat(line: "x++;", count: 60)}}
                        """,
                outputKind: OutputKind.ConsoleApplication);
    }

    /// <summary>A generated marker cannot exempt a handwritten long body.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.bodylength.Analyze_GeneratedCode_IsSkipped")]
    public Task GeneratedConfigurationCannotExemptHandwrittenCode()
    {
        return VerifyAsync(caseId: "rules.bodylength.Analyze_GeneratedCode_IsSkipped", diagnostic: "SCR0006",
                source: $$"""
                        class C
                        {
                            private int _x;

                            void {|SCR0006:M|}()
                            {
                        {{Repeat(line: Statement, count: 61)}}
                            }
                        }
                        """,
                editorConfig: """
                        root = true

                        [*.cs]
                        generated_code = true
                        """);
    }

    /// <summary>One below the hard limit is accepted independently.</summary>
    [Fact, Trait("Case", "rules.body.59")]
    public Task Method59LinesPasses()
    {
        return VerifyAsync(caseId: "rules.body.59", diagnostic: "ANALYZER_ACCEPTED", source:
                "class C { private int _x; void M()\n{\n" + Repeat(line: Statement, count: 59) + "\n} }");
    }

    /// <summary>Forty lines is advice, not an additional hard limit.</summary>
    [Fact, Trait("Case", "rules.body.40")]
    public Task Method40LinesPasses()
    {
        return VerifyAsync(caseId: "rules.body.40", diagnostic: "ANALYZER_ACCEPTED", source:
                "class C { private int _x; void M()\n{\n" + Repeat(line: Statement, count: 40) + "\n} }");
    }

    private const string Statement = "        _x++;";

    private static string Repeat(string line, int count)
    {
        return string.Join(separator: "\n", values: Enumerable.Repeat(element: line, count: count));
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source, string? editorConfig = null
            , OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType:
                "BodyLengthAnalyzer", source: source, editorConfig: editorConfig, outputKind: outputKind);
    }
}
