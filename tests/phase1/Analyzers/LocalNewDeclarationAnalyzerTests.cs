using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Microsoft.CodeAnalysis;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0012: a local declared with an explicit type and initialized with a target-typed
/// <c>new(…)</c> fails, so <c>var x = new Foo();</c> is the one accepted form. Fields, properties, arguments, return
/// values, collection expressions and locals where <c>var</c> cannot be used pass.
/// </summary>
public sealed class LocalNewDeclarationAnalyzerTests
{
    /// <summary><c>{|SCR0012:Foo x = new()|};</c> is reported on the declaration.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-18.negative")]
    public Task Analyze_TypedLocalWithTargetTypedNew_Reports()
    {
        return VerifyAsync(caseId: "R-18.negative", diagnostic: "SCR0012", source: """
                class Foo
                {
                    public void M()
                    {
                        {|SCR0012:Foo x = new()|};
                    }
                }
                """);
    }

    /// <summary>A target-typed <c>new</c> with arguments and an object initializer is reported too.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", ("rules.localnewdeclaration.Analyze_TargetTypedNewWithArg" +
            "umentsAndInitializer_Reports"))]
    public Task Analyze_TargetTypedNewWithArgumentsAndInitializer_Reports()
    {
        return VerifyAsync(caseId: ("rules.localnewdeclaration.Analyze_TargetTypedNewWithArg" +
                "umentsAndInitializer_Reports"), diagnostic: "SCR0012", source: """
                class Foo
                {
                    public Foo(int start)
                    {
                    }

                    public int Value { get; set; }

                    public static void M()
                    {
                        {|SCR0012:Foo x = new(start: 1) { Value = 2 }|};
                    }
                }
                """);
    }

    /// <summary><c>var x = new Foo();</c> passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-18.positive")]
    public Task Analyze_VarWithNew_Passes()
    {
        return VerifyAsync(caseId: "R-18.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                class Foo
                {
                    public void M()
                    {
                        var x = new Foo();
                    }
                }
                """);
    }

    /// <summary>
    /// <c>Foo x = new Foo();</c> is not SCR0012's: IDE0007 reports it in the build, so SCR0012 stays silent.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_TypedLocalWithExplicitNew_IsNotReported")]
    public Task Analyze_TypedLocalWithExplicitNew_IsNotReported()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_TypedLocalWithExplicitNew_IsNotReported",
                diagnostic: "ANALYZER_ACCEPTED", source: """
                class Foo
                {
                    public void M()
                    {
                        Foo x = new Foo();
                    }
                }
                """);
    }

    /// <summary>A field and a property initialized with target-typed <c>new()</c> pass.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_FieldAndProperty_Pass")]
    public Task Analyze_FieldAndProperty_Pass()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_FieldAndProperty_Pass", diagnostic:
                "ANALYZER_ACCEPTED", source: """
                class Foo
                {
                    private readonly Foo _x = new();

                    public Foo Property { get; } = new();
                }
                """);
    }

    /// <summary>Target-typed <c>new()</c> as an argument and as a return value passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_ArgumentAndReturnValue_Pass")]
    public Task Analyze_ArgumentAndReturnValue_Pass()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_ArgumentAndReturnValue_Pass", diagnostic:
                "ANALYZER_ACCEPTED", source: """
                class Foo
                {
                    public static Foo Create()
                    {
                        Use(value: new());
                        return new();
                    }

                    private static void Use(Foo value)
                    {
                    }
                }
                """);
    }

    /// <summary>A <c>using</c> declaration and an <c>await using</c> declaration are both reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_UsingAndAwaitUsingDeclarations_Report")]
    public Task Analyze_UsingAndAwaitUsingDeclarations_Report()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_UsingAndAwaitUsingDeclarations_Report",
                diagnostic: "SCR0012", source: """
                using System;
                using System.Threading.Tasks;

                class Resource : IDisposable, IAsyncDisposable
                {
                    public void Dispose()
                    {
                    }

                    public ValueTask DisposeAsync()
                    {
                        return default;
                    }

                    public static async Task M()
                    {
                        using {|SCR0012:Resource first = new()|};
                        await using {|SCR0012:Resource second = new()|};
                        await Task.Yield();
                    }
                }
                """);
    }

    /// <summary>A <c>for</c> initializer and a <c>using (…)</c> statement declaration are both reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_ForInitializerAndUsingStatement_Report")]
    public Task Analyze_ForInitializerAndUsingStatement_Report()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_ForInitializerAndUsingStatement_Report",
                diagnostic: "SCR0012", source: ("using System;\n\nclass Counter : IDisposable\n{\n    public" +
                        " int Value { get; set; }\n\n    public void Dispose()\n   " +
                        " {\n    }\n\n    public static void M()\n    {\n        for " +
                        "({|SCR0012:Counter counter = new()|}; counter.Value < 3" +
                        "; counter.Value++)\n        {\n        }\n\n        using (" +
                        "{|SCR0012:Counter other = new()|})\n        {\n        }\n" +
                        "    }\n}"));
    }

    /// <summary>Several declarators in one declaration cannot use <c>var</c>, so they pass.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_SeveralDeclarators_Pass")]
    public Task Analyze_SeveralDeclarators_Pass()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_SeveralDeclarators_Pass", diagnostic:
                "ANALYZER_ACCEPTED", source: """
                class Foo
                {
                    public void M()
                    {
                        Foo first = new(), second = new();
                        for (Foo a = new(), b = new(); a != b; a = b)
                        {
                        }
                    }
                }
                """);
    }

    /// <summary>
    /// A collection expression, an initializer that is not <c>new</c>, a local with no initializer and a
    /// <c>ref</c> local pass.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_OtherInitializers_Pass")]
    public Task Analyze_OtherInitializers_Pass()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_OtherInitializers_Pass", diagnostic:
                "ANALYZER_ACCEPTED", source: """
                using System.Collections.Generic;

                class Foo
                {
                    public static void M(int[] values)
                    {
                        List<int> items = [];
                        Foo created = Create();
                        Foo later;
                        later = created;
                        _ = later;
                        ref int first = ref values[0];
                        for (int index = 0; index < values.Length; index++)
                        {
                        }
                    }

                    private static Foo Create()
                    {
                        return new Foo();
                    }
                }
                """);
    }

    /// <summary>A top-level statement local with target-typed <c>new()</c> is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_TopLevelStatement_Reports")]
    public Task Analyze_TopLevelStatement_Reports()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_TopLevelStatement_Reports", diagnostic:
                "SCR0012",
                source: """
                        using System.Text;

                        {|SCR0012:StringBuilder builder = new()|};
                        builder.Append(value: 1);
                        """,
                outputKind: OutputKind.ConsoleApplication);
    }

    /// <summary>The message names the local and the type to write after <c>new</c>.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_Message_NamesLocalAndType")]
    public Task Analyze_Message_NamesLocalAndType()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_Message_NamesLocalAndType", diagnostic:
                "SCR0012",
                source: """
                        class Foo
                        {
                            public void M()
                            {
                                {|SCR0012:Foo x = new()|};
                            }
                        }
                        """,
                message: "Declare 'x' as 'var x = new Foo(...)'");
    }

    /// <summary>Generated settings cannot exempt handwritten code.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.localnewdeclaration.Analyze_GeneratedCode_IsSkipped")]
    public Task Analyze_GeneratedCode_IsSkipped()
    {
        return VerifyAsync(caseId: "rules.localnewdeclaration.Analyze_GeneratedCode_IsSkipped", diagnostic: "SCR0012",
                source: """
                        class Foo
                        {
                            public void M()
                            {
                                {|SCR0012:Foo x = new()|};
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
                "LocalNewDeclarationAnalyzer", source: source, editorConfig: editorConfig, message: message,
                outputKind: outputKind);
    }
}
