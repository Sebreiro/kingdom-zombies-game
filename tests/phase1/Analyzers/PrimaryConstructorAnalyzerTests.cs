using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0003: a primary constructor on a class or struct and a positional record class or record
/// struct fail, while a non-positional record and a class with an ordinary constructor pass.
/// </summary>
public sealed class PrimaryConstructorAnalyzerTests
{
    /// <summary>A class with a primary constructor is reported on its parameter list.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-23.negative")]
    public Task ClassPrimaryConstructorReports()
    {
        return VerifyAsync(caseId: "R-23.negative", diagnostic: "SCR0003", source: """
                class C{|SCR0003:(int x)|}
                {
                    public int X => x;
                }
                """);
    }

    /// <summary>A class with an empty primary constructor parameter list is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.primaryconstructor.Class_EmptyPrimaryConstructor_Reports")]
    public Task ClassEmptyPrimaryConstructorReports()
    {
        return VerifyAsync(caseId: "rules.primaryconstructor.Class_EmptyPrimaryConstructor_Reports", diagnostic: "SCR0003", source: """
                class C{|SCR0003:()|}
                {
                }
                """);
    }

    /// <summary>A struct with a primary constructor is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.primaryconstructor.Struct_PrimaryConstructor_Reports")]
    public Task StructPrimaryConstructorReports()
    {
        return VerifyAsync(caseId: "rules.primaryconstructor.Struct_PrimaryConstructor_Reports", diagnostic: "SCR0003", source: """
                struct S{|SCR0003:(int x)|}
                {
                    public int X => x;
                }
                """);
    }

    /// <summary>A positional record and a positional record class are reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-24.negative")]
    public Task AnalyzePositionalRecordReports()
    {
        return VerifyAsync(caseId: "R-24.negative", diagnostic: "SCR0003", source: """
                record R{|SCR0003:(int X)|};

                record class Q{|SCR0003:(int X)|};
                """);
    }

    /// <summary>A positional record struct is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.primaryconstructor.Analyze_PositionalRecordStruct_Reports")]
    public Task AnalyzePositionalRecordStructReports()
    {
        return VerifyAsync(caseId: "rules.primaryconstructor.Analyze_PositionalRecordStruct_Reports", diagnostic: "SCR0003", source: """
                record struct P{|SCR0003:(int X)|};
                """);
    }

    /// <summary>A record with init properties and no parameter list passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-24.positive")]
    public Task AnalyzeNonPositionalRecordPasses()
    {
        return VerifyAsync(caseId: "R-24.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                record R
                {
                    public int X { get; init; }
                }

                record struct P
                {
                    public int X { get; init; }
                }
                """);
    }

    /// <summary>A class and a struct with ordinary constructors pass.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-23.positive")]
    public Task AnalyzeOrdinaryConstructorPasses()
    {
        return VerifyAsync(caseId: "R-23.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                class C
                {
                    private readonly int _x;

                    public C(int x)
                    {
                        _x = x;
                    }

                    public int X => _x;
                }

                struct S
                {
                    public S(int x)
                    {
                        X = x;
                    }

                    public int X { get; }
                }
                """);
    }

    /// <summary>
    /// The receiver parameter list of a C# 14 extension block is not a primary constructor and passes.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.primaryconstructor.ExtensionBlock_ReceiverParameter_Passes")]
    public Task ExtensionBlockReceiverParameterPasses()
    {
        return VerifyAsync(caseId: "rules.primaryconstructor.ExtensionBlock_ReceiverParameter_Passes", diagnostic: "ANALYZER_ACCEPTED", source: """
                static class E
                {
                    extension(string s)
                    {
                        public int Doubled => s.Length * 2;
                    }
                }
                """);
    }

    /// <summary>Empty and nested positional forms remain forbidden even on a generated-marked tree.</summary>
    [Fact, Trait("Case", "rules.primaryconstructor.generated-nested")]
    public Task GeneratedNestedPrimaryConstructorsRemainChecked()
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: "rules.primaryconstructor.generated-nested", diagnostic: "SCR0003", analyzerType: "PrimaryConstructorAnalyzer", editorConfig: "generated_code = true", source: """
                // <auto-generated />
                class Outer
                {
                    public record Inner{|SCR0003:()|};
                    public record struct Value{|SCR0003:()|};
                    public class Nested{|SCR0003:()|} { }
                }
                """);
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType: "PrimaryConstructorAnalyzer",source: source);
    }
}
