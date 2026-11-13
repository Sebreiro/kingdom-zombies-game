using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Microsoft.CodeAnalysis;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0011: a member access qualified with <c>this.</c> fails; every other use of the
/// <c>this</c> keyword passes.
/// </summary>
public sealed class ThisQualificationAnalyzerTests
{
    /// <summary>Assigning a field through <c>this.</c> is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-28.negative")]
    public Task AnalyzeFieldAssignmentReports()
    {
        return VerifyAsync(caseId: "R-28.negative", diagnostic: "SCR0011", source: """
                class C
                {
                    private int _value;

                    public void Set(int value)
                    {
                        {|SCR0011:this._value|} = value; _ = _value;
                    }
                }
                """);
    }

    /// <summary>Calling a method through <c>this.</c> is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.thisqualification.Analyze_MethodCall_Reports")]
    public Task AnalyzeMethodCallReports()
    {
        return VerifyAsync(caseId: "rules.thisqualification.Analyze_MethodCall_Reports", diagnostic: "SCR0011", 
                source: """
                class C
                {
                    public void Run()
                    {
                        {|SCR0011:this.Stop|}();
                    }

                    public void Stop()
                    {
                    }
                }
                """);
    }

    /// <summary>Reading a property and raising an event through <c>this.</c> are both reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.thisqualification.Analyze_PropertyAndEvent_Report")]
    public Task AnalyzePropertyAndEventReport()
    {
        return VerifyAsync(caseId: "rules.thisqualification.Analyze_PropertyAndEvent_Report", diagnostic: 
                "SCR0011", source: """
                using System;

                class C
                {
                    public event Action? Changed;

                    public int Value { get; set; }

                    public int Read()
                    {
                        {|SCR0011:this.Changed|}?.Invoke();
                        return {|SCR0011:this.Value|};
                    }
                }
                """);
    }

    /// <summary>Unqualified member access passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-28.positive")]
    public Task AnalyzeUnqualifiedPasses()
    {
        return VerifyAsync(caseId: "R-28.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                class C
                {
                    private int _value;

                    public void Set(int value)
                    {
                        _value = value; _ = _value;
                        Stop();
                    }

                    public void Stop()
                    {
                    }
                }
                """);
    }

    /// <summary>Constructor chaining with <c>: this(...)</c> passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.thisqualification.Analyze_ConstructorChaining_Passes")]
    public Task AnalyzeConstructorChainingPasses()
    {
        return VerifyAsync(caseId: "rules.thisqualification.Analyze_ConstructorChaining_Passes", diagnostic: 
                "ANALYZER_ACCEPTED", source: """
                class C
                {
                    public C()
                        : this(value: 0)
                    {
                    }

                    public C(int value)
                    {
                    }
                }
                """);
    }

    /// <summary>Passing <c>this</c> as an argument and returning it pass.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.thisqualification.Analyze_ThisAsValue_Passes")]
    public Task AnalyzeThisAsValuePasses()
    {
        return VerifyAsync(caseId: "rules.thisqualification.Analyze_ThisAsValue_Passes", diagnostic: 
                "ANALYZER_ACCEPTED", source: """
                class C
                {
                    public C Self()
                    {
                        Use(target: this);
                        return this;
                    }

                    private static void Use(C target)
                    {
                    }
                }
                """);
    }

    /// <summary>An indexer declaration <c>this[int i]</c> and an element access <c>this[0]</c> pass.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.thisqualification.Analyze_Indexer_Passes")]
    public Task AnalyzeIndexerPasses()
    {
        return VerifyAsync(caseId: "rules.thisqualification.Analyze_Indexer_Passes", diagnostic: 
                "ANALYZER_ACCEPTED", source: """
                class C
                {
                    public int this[int index] => index;

                    public int First()
                    {
                        return this[0];
                    }
                }
                """);
    }

    /// <summary>The <c>this</c> modifier of an extension method's first parameter passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.thisqualification.Analyze_ExtensionMethodParameter_Passes")]
    public Task AnalyzeExtensionMethodParameterPasses()
    {
        return VerifyAsync(caseId: "rules.thisqualification.Analyze_ExtensionMethodParameter_Passes", diagnostic
                : "ANALYZER_ACCEPTED", source: """
                static class Extensions
                {
                    public static int Twice(this int value)
                    {
                        return value * 2;
                    }
                }
                """);
    }

    /// <summary>
    /// Calling an extension method on the current instance through <c>this.</c> passes: the call does not bind
    /// without it.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.thisqualification.ExtensionMethodCall_OnThis_Passes")]
    public Task ExtensionMethodCallOnThisPasses()
    {
        return VerifyAsync(caseId: "rules.thisqualification.ExtensionMethodCall_OnThis_Passes", diagnostic: 
                "ANALYZER_ACCEPTED", source: """
                static class E
                {
                    public static int Twice(this C c)
                    {
                        return 2;
                    }
                }

                class C
                {
                    public int M()
                    {
                        return this.Twice();
                    }
                }
                """);
    }

    /// <summary>
    /// Using a C# 14 extension property and extension method on the current instance through <c>this.</c> passes,
    /// while a real member next to them is still reported.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.thisqualification.ExtensionMember_OnThis_Passes")]
    public Task ExtensionMemberOnThisPasses()
    {
        return VerifyAsync(caseId: "rules.thisqualification.ExtensionMember_OnThis_Passes", diagnostic: 
                "SCR0011", source: ("static class E\n{\n    extension(C c)\n    {\n        publi" +
                        "c int Doubled => 2;\n\n        public int Tripled()\n     " +
                        "   {\n            return 3;\n        }\n    }\n}\n\nclass C\n{" +
                        "\n    public int Value { get; set; }\n\n    public int M()" +
                        "\n    {\n        return this.Doubled + this.Tripled() + {" +
                        "|SCR0011:this.Value|};\n    }\n}"));
    }

    /// <summary>Generated settings cannot exempt handwritten code.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.thisqualification.Analyze_GeneratedCode_IsSkipped")]
    public Task AnalyzeGeneratedCodeIsSkipped()
    {
        return VerifyAsync(caseId: "rules.thisqualification.Analyze_GeneratedCode_IsSkipped", diagnostic: "SCR0011", 
                source: """
                        class C
                        {
                            private int _value;

                            public void Set(int value)
                            {
                                {|SCR0011:this._value|} = value; _ = _value;
                            }
                        }
                        """,
                editorConfig: """
                        root = true

                        [*.cs]
                        generated_code = true
                        """);
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source, string? editorConfig = null
            , string? message = null, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType: 
                "ThisQualificationAnalyzer", source: source, editorConfig: editorConfig, message: message, 
                outputKind: outputKind);
    }
}
