using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using KingdomZombies.Phase1.Tests.Verification;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0001: a positional argument fails, a named one passes, and each exemption agreed in the
/// 01-06 spike (E1, E2, E3, E4, E5, E7, E10) passes, while the dropped candidates (E6, E8, E9) still fail.
/// </summary>
public sealed class NamedArgumentAnalyzerTests
{
    /// <summary>A positional argument to a method is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-38.negative")]
    public Task InvocationPositionalArgumentReports()
    {
        return VerifyAsync(caseId: "R-38.negative", diagnostic: "SCR0001", source: """
                class C
                {
                    static void M(int a) { }
                    static void N() { M({|SCR0001:1|}); }
                }
                """);
    }

    /// <summary>A named argument passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-38.positive")]
    public Task InvocationNamedArgumentPasses()
    {
        return VerifyAsync(caseId: "R-38.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                class C
                {
                    static void M(int a, int b) { }
                    static void N() { M(a: 1, b: 2); M(b: 2, a: 1); }
                }
                """);
    }

    /// <summary>Positional arguments to constructors and constructor initializers are reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Construction_PositionalArgument_Reports")]
    public Task ConstructionPositionalArgumentReports()
    {
        return VerifyAsync(caseId: "rules.namedargument.Construction_PositionalArgument_Reports", diagnostic: "SCR0001", source: """
                class B
                {
                    public B(int a) { }
                }

                class C : B
                {
                    public C() : base({|SCR0001:1|}) { }
                    static void N() { var b = new B({|SCR0001:2|}); B c = new({|SCR0001:3|}); }
                }
                """);
    }

    /// <summary>
    /// E1: the elements of a <c>params</c> array or collection pass; a normal parameter before them fails.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Params_Elements_AreExempt")]
    public Task ParamsElementsAreExempt()
    {
        return VerifyAsync(caseId: "rules.namedargument.Params_Elements_AreExempt", diagnostic: "SCR0001", source: """
                using System;
                using System.Collections.Generic;

                class C
                {
                    static int Sum(params int[] values) { return 0; }
                    static int Span(params ReadOnlySpan<int> values) { return 0; }
                    static int List(params List<int> values) { return 0; }
                    static string Join(string separator, params string[] values) { return separator; }
                    static void N()
                    {
                        Sum(1, 2, 3);
                        Span(1, 2);
                        List(1, 2);
                        Join({|SCR0001:", "|}, "a", "b");
                    }
                }
                """);
    }

    /// <summary>E2: the receiver of an extension call passes; the same receiver written as an argument fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_ExtensionReceiver_IsExempt")]
    public Task AnalyzeExtensionReceiverIsExempt()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_ExtensionReceiver_IsExempt", diagnostic: "SCR0001", source: """
                static class Ext
                {
                    public static int Twice(this string s, int times) { return times; }
                }

                class C
                {
                    static void N() { "x".Twice(times: 2); Ext.Twice({|SCR0001:"x"|}, times: 2); }
                }
                """);
    }

    /// <summary>E2: the receiver of a C# 14 extension member passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_ExtensionMemberReceiver_IsExempt")]
    public Task AnalyzeExtensionMemberReceiverIsExempt()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_ExtensionMemberReceiver_IsExempt", diagnostic: "SCR0001", source: """
                static class Ext
                {
                    extension(string s)
                    {
                        public int Thrice(int times) { return times; }
                    }
                }

                class C
                {
                    static void N() { "x".Thrice(times: 3); }
                }
                """);
    }

    /// <summary>E3: attribute arguments pass; the same constructor called in code fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_AttributeArguments_AreExempt")]
    public Task AnalyzeAttributeArgumentsAreExempt()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_AttributeArguments_AreExempt", diagnostic: "SCR0001", source: """
                using System;

                class C
                {
                    [Obsolete("Old.", false)]
                    static void Old() { }

                    [Obsolete("Old.", error: true)]
                    static void Older() { }

                    static object N() { return new ObsoleteAttribute({|SCR0001:"x"|}); }
                }
                """);
    }

    /// <summary>E4: indexer and element-access arguments pass.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_IndexerArguments_AreExempt")]
    public Task AnalyzeIndexerArgumentsAreExempt()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_IndexerArguments_AreExempt", diagnostic: "SCR0001", source: """
                using System.Collections.Generic;

                class C
                {
                    int this[int i] { get { return i; } }

                    int N(int[] values, Dictionary<string, int> map)
                    {
                        map["k"] = values[0];
                        return this[1] + values[^1];
                    }
                }
                """);
    }

    /// <summary>E5: a delegate invocation passes; a method call on the same value fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_DelegateInvocation_IsExempt")]
    public Task AnalyzeDelegateInvocationIsExempt()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_DelegateInvocation_IsExempt", diagnostic: "SCR0001", source: """
                using System;

                class C
                {
                    static int N(Func<int, int> format, Action<string> log)
                    {
                        log("x");
                        log.Invoke("y");
                        return format(1) + Math.Max({|SCR0001:1|}, {|SCR0001:2|});
                    }
                }
                """);
    }

    /// <summary>
    /// E5: a function-pointer invocation passes, because the compiler rejects named arguments there; a method call
    /// next to it fails.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.FunctionPointerInvocation_PositionalArgument_IsExempt")]
    public Task FunctionPointerInvocationPositionalArgumentIsExempt()
    {
        return VerifyAsync(caseId: "rules.namedargument.FunctionPointerInvocation_PositionalArgument_IsExempt", diagnostic: "SCR0001", source: """
                unsafe class C
                {
                    static void Take(int value)
                    {
                    }

                    static void N(delegate*<int, void> take)
                    {
                        take(1);
                        Take({|SCR0001:2|});
                    }
                }
                """);
    }

    /// <summary>E7: <c>nameof</c>, tuples, <c>typeof</c> and <c>default</c> are not arguments.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_NameofTupleTypeofDefault_AreExempt")]
    public Task AnalyzeNameofTupleTypeofDefaultAreExempt()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_NameofTupleTypeofDefault_AreExempt", diagnostic: "SCR0001", source: """
                using System;

                class C
                {
                    static object N(int code)
                    {
                        (string Name, Type Type) pair = (nameof(code), typeof(C));
                        int none = default;
                        return pair.Name + none;
                    }
                }
                """);
    }

    /// <summary>E7: a <c>nameof</c> passed as an argument value still needs the parameter name.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_NameofAsArgumentValue_Reports")]
    public Task AnalyzeNameofAsArgumentValueReports()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_NameofAsArgumentValue_Reports", diagnostic: "SCR0001", source: """
                using System;

                class C
                {
                    static Exception N(int code)
                    {
                        return new ArgumentException({|SCR0001:"Bad."|}, {|SCR0001:nameof(code)|});
                    }
                }
                """);
    }

    /// <summary>E10: code marked <c>generated_code = true</c> in an <c>.editorconfig</c> cannot exempt a handwritten subject.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.GeneratedConfigurationCannotExemptHandwrittenCode")]
    public Task GeneratedConfigurationCannotExemptHandwrittenCode()
    {
        return VerifyAsync(caseId: "rules.namedargument.GeneratedConfigurationCannotExemptHandwrittenCode", diagnostic: "SCR0001", 
                source: """
                        class C
                        {
                            static void M(int a) { }
                            static void N() { M({|SCR0001:1|}); }
                        }
                        """,
                editorConfig: """
                        root = true

                        [*.cs]
                        generated_code = true
                        """);
    }

    /// <summary>
    /// The compiler-made interpolated-string handler arguments and omitted optionals are not reported.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_ImplicitArguments_AreSkipped")]
    public Task AnalyzeImplicitArgumentsAreSkipped()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_ImplicitArguments_AreSkipped", diagnostic: "SCR0001", source: """
                using System.Globalization;
                using System.Text;

                class C
                {
                    static void M(int a, int b = 2) { }
                    static string N(int x)
                    {
                        M(a: 1);
                        var builder = new StringBuilder();
                        builder.Append(provider: CultureInfo.InvariantCulture, handler: $"value {x} and {x:D2}");
                        return builder.ToString();
                    }
                }
                """);
    }

    /// <summary>Dropped E6: a single lambda argument still needs its parameter name.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_SingleLambdaArgument_Reports")]
    public Task AnalyzeSingleLambdaArgumentReports()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_SingleLambdaArgument_Reports", diagnostic: "SCR0001", source: """
                using System.Collections.Generic;
                using System.Linq;

                class C
                {
                    static List<int> N(List<int> items)
                    {
                        return items.Where({|SCR0001:i => i > 0|}).Where(predicate: i => i < 9).ToList();
                    }
                }
                """);
    }

    /// <summary>Dropped E8: an argument to a single-parameter method still needs its parameter name.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_SingleParameterMethod_Reports")]
    public Task AnalyzeSingleParameterMethodReports()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_SingleParameterMethod_Reports", diagnostic: "SCR0001", source: """
                using System;

                class C
                {
                    static bool N(string word)
                    {
                        return string.IsNullOrWhiteSpace({|SCR0001:word|}) || Math.Abs(value: -1) > 0;
                    }
                }
                """);
    }

    /// <summary>Dropped E9: an identifier equal to the parameter name still needs the name.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.namedargument.Analyze_IdentifierEqualToParameterName_Reports")]
    public Task AnalyzeIdentifierEqualToParameterNameReports()
    {
        return VerifyAsync(caseId: "rules.namedargument.Analyze_IdentifierEqualToParameterName_Reports", diagnostic: "SCR0001", source: """
                class C
                {
                    static void M(int code) { }
                    static void N(int code) { M({|SCR0001:code|}); M(code: code); }
                }
                """);
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source, string? editorConfig = null)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType: "NamedArgumentAnalyzer",
                source: source,
                editorConfig: editorConfig,
                outputKind: OutputKind.DynamicallyLinkedLibrary);
    }
}
