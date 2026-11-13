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
        return VerifyAsync(caseId: "R-10.positive", diagnostic: "ANALYZER_ACCEPTED", source: TypeNamed(
                name: Letters(letter: 'A', count: 120)));
    }

    /// <summary>A line of 121 Latin letters fails, reported on the characters past the limit.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-10.negative")]
    public Task Line121CharactersFails()
    {
        return VerifyAsync(caseId: "R-10.negative", diagnostic: "SCR0007", source: TypeNamed(name: Letters(
                letter: 'A', count: 120) + "{|SCR0007:A|}"));
    }

    /// <summary>A line of 120 Cyrillic letters passes: each letter is one UTF-16 character.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.Line_120CyrillicLetters_Passes")]
    public Task Line120CyrillicLettersPasses()
    {
        return VerifyAsync(caseId: "rules.linelength.Line_120CyrillicLetters_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: TypeNamed(name: Letters(letter: 'Я', count: 120)));
    }

    /// <summary>A line of 121 Cyrillic letters fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.Line_121CyrillicLetters_Fails")]
    public Task Line121CyrillicLettersFails()
    {
        return VerifyAsync(caseId: "rules.linelength.Line_121CyrillicLetters_Fails", diagnostic: "SCR0007",
                source: TypeNamed(name: Letters(letter: 'Я', count: 120) + "{|SCR0007:Я|}"));
    }

    /// <summary>A comment line of 121 characters fails as well: every line is checked.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.CommentLine_121Characters_Fails")]
    public Task CommentLine121CharactersFails()
    {
        return VerifyAsync(caseId: "rules.linelength.CommentLine_121Characters_Fails", diagnostic: "SCR0007",
                source: "class C\n{\n}\n//" + Letters(letter: 'a', count: 118) + "{|SCR0007:a|}\n");
    }

    /// <summary>The CRLF terminator is not counted: a 120-character line ending in CRLF passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.Analyze_CrLfTerminator_IsNotCounted")]
    public Task AnalyzeCrLfTerminatorIsNotCounted()
    {
        return VerifyAsync(caseId: "rules.linelength.Analyze_CrLfTerminator_IsNotCounted",
                diagnostic: "ANALYZER_ACCEPTED", source: "class\r\n" + Letters(letter: 'A', count: 120)
                        + "\r\n{\r\n}\r\n");
    }

    /// <summary>A generated marker cannot exempt a handwritten violation.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.linelength.Analyze_GeneratedCode_IsSkipped")]
    public Task GeneratedConfigurationCannotExemptHandwrittenCode()
    {
        return VerifyAsync(caseId: "rules.linelength.Analyze_GeneratedCode_IsSkipped", diagnostic: "SCR0007",
                source: TypeNamed(name: Letters(letter: 'A', count: 120) + "{|SCR0007:" + Letters(letter: 'A',
                        count: 80) + "|}"), editorConfig: """
                        root = true

                        [*.cs]
                        generated_code = true
                        """);
    }

    /// <summary>Checks 119 UTF-16 units using Ascii text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.ascii.119")]
    public Task Ascii119Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.ascii.119", units: 119, sequence: "a");
    }

    /// <summary>Checks 120 UTF-16 units using Ascii text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.ascii.120")]
    public Task Ascii120Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.ascii.120", units: 120, sequence: "a");
    }

    /// <summary>Checks 121 UTF-16 units using Ascii text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.ascii.121")]
    public Task Ascii121Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.ascii.121", units: 121, sequence: "a");
    }

    /// <summary>Checks 119 UTF-16 units using Cyrillic text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.cyrillic.119")]
    public Task Cyrillic119Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.cyrillic.119", units: 119, sequence: "Я");
    }

    /// <summary>Checks 120 UTF-16 units using Cyrillic text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.cyrillic.120")]
    public Task Cyrillic120Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.cyrillic.120", units: 120, sequence: "Я");
    }

    /// <summary>Checks 121 UTF-16 units using Cyrillic text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.cyrillic.121")]
    public Task Cyrillic121Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.cyrillic.121", units: 121, sequence: "Я");
    }

    /// <summary>Checks 119 UTF-16 units using Surrogate text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.surrogate.119")]
    public Task Surrogate119Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.surrogate.119", units: 119, sequence: "😀");
    }

    /// <summary>Checks 120 UTF-16 units using Surrogate text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.surrogate.120")]
    public Task Surrogate120Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.surrogate.120", units: 120, sequence: "😀");
    }

    /// <summary>Checks 121 UTF-16 units using Surrogate text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.surrogate.121")]
    public Task Surrogate121Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.surrogate.121", units: 121, sequence: "😀");
    }

    /// <summary>Checks 119 UTF-16 units using Combining text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.combining.119")]
    public Task Combining119Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.combining.119", units: 119, sequence: "é");
    }

    /// <summary>Checks 120 UTF-16 units using Combining text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.combining.120")]
    public Task Combining120Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.combining.120", units: 120, sequence: "é");
    }

    /// <summary>Checks 121 UTF-16 units using Combining text, excluding the terminator.</summary>
    [Fact, Trait("Case", "rules.line.combining.121")]
    public Task Combining121Utf16Units()
    {
        return Utf16LineAsync(caseId: "rules.line.combining.121", units: 121, sequence: "é");
    }

    private static Task Utf16LineAsync(string caseId, int units, string sequence)
    {
        string content = "//" + string.Concat(values: System.Linq.Enumerable.Repeat(element: sequence,
                count: (units - 2) / sequence.Length));
        content += new string(c: 'a', count: units - content.Length);
        bool reports = units > 120;
        if (reports)
        {
            content = content[..120] + "{|SCR0007:" + content[120..] + "|}";
        }

        return VerifyAsync(caseId: caseId, diagnostic: reports ? "SCR0007" : "ANALYZER_ACCEPTED",
                source: content + "\r\nclass C { }\r\n");
    }

    private static string Letters(char letter, int count)
    {
        return new string(c: letter, count: count);
    }

    private static string TypeNamed(string name)
    {
        return "class\n" + name + "\n{\n}\n";
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source,
            string? editorConfig = null, string? message = null)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic,
                analyzerType: "LineLengthAnalyzer", source: source, editorConfig: editorConfig, message: message);
    }
}
