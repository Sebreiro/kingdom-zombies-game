using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0007: a line of 120 UTF-16 characters passes and one of 121 fails, the line terminator is
/// not counted, and a Cyrillic letter counts as one character. Long lines are generated in code: a type name alone
/// on its line, so the line holds nothing but letters.
/// </summary>
public sealed class LineLengthAnalyzerTests
{
    /// <summary>A line of exactly 120 Latin letters passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-10.positive")]
    public Task Line120CharactersPasses()
    {
        return VerifyAsync(caseId: "R-10.positive", diagnostic: "ANALYZER_ACCEPTED", source: TypeNamed(name: Letters(letter: 'A', count: 120)));
    }

    /// <summary>A line of 121 Latin letters fails, reported on the characters past the limit.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-10.negative")]
    public Task Line121CharactersFails()
    {
        return VerifyAsync(caseId: "R-10.negative", diagnostic: "SCR0007", source: TypeNamed(name: Letters(letter: 'A', count: 120) + "{|SCR0007:A|}"));
    }

    /// <summary>A line of 120 Cyrillic letters passes: each letter is one UTF-16 character.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.Line_120CyrillicLetters_Passes")]
    public Task Line120CyrillicLettersPasses()
    {
        return VerifyAsync(caseId: "rules.linelength.Line_120CyrillicLetters_Passes", diagnostic: "ANALYZER_ACCEPTED", source: TypeNamed(name: Letters(letter: 'Я', count: 120)));
    }

    /// <summary>A line of 121 Cyrillic letters fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.Line_121CyrillicLetters_Fails")]
    public Task Line121CyrillicLettersFails()
    {
        return VerifyAsync(caseId: "rules.linelength.Line_121CyrillicLetters_Fails", diagnostic: "SCR0007", source: TypeNamed(name: Letters(letter: 'Я', count: 120) + "{|SCR0007:Я|}"));
    }

    /// <summary>A comment line of 121 characters fails as well: every line is checked.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.CommentLine_121Characters_Fails")]
    public Task CommentLine121CharactersFails()
    {
        return VerifyAsync(caseId: "rules.linelength.CommentLine_121Characters_Fails", diagnostic: "SCR0007", source: "class C\n{\n}\n//" + Letters(letter: 'a', count: 118) + "{|SCR0007:a|}\n");
    }

    /// <summary>The CRLF terminator is not counted: a 120-character line ending in CRLF passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.Analyze_CrLfTerminator_IsNotCounted")]
    public Task AnalyzeCrLfTerminatorIsNotCounted()
    {
        return VerifyAsync(caseId: "rules.linelength.Analyze_CrLfTerminator_IsNotCounted", diagnostic: "ANALYZER_ACCEPTED", source: "class\r\n" + Letters(letter: 'A', count: 120) + "\r\n{\r\n}\r\n");
    }

    /// <summary>A generated marker cannot exempt a handwritten violation.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.Analyze_GeneratedCode_IsSkipped")]
    public Task GeneratedConfigurationCannotExemptHandwrittenCode()
    {
        return VerifyAsync(caseId: "rules.linelength.Analyze_GeneratedCode_IsSkipped", diagnostic: "SCR0007", 
                source: TypeNamed(name: Letters(letter: 'A', count: 120) + "{|SCR0007:" + Letters(letter: 'A', count: 80) + "|}"),
                editorConfig: """
                        root = true

                        [*.cs]
                        generated_code = true
                        """);
    }

    private static string Letters(char letter, int count)
    {
        return new string(c: letter, count: count);
    }

    private static string TypeNamed(string name)
    {
        return "class\n" + name + "\n{\n}\n";
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source, string? editorConfig = null, string? message = null)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType: "LineLengthAnalyzer", source: source, editorConfig: editorConfig, message: message);
    }
}
