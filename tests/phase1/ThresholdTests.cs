using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Checks declared numeric units and exact source thresholds without rounding or parser overflow.</summary>
public sealed class ThresholdTests
{
    /// <summary>Out-of-range integer limits fail with their exact rule instead of escaping the validator.</summary>
    [Fact, Trait("Case", "edge.rule-03.precision")]
    public void Validate_NumericLimitOverflow_ReportsExactRule()
    {
        JsonObject manifest = RegisterValidator.Read(relative: "docs/code/provenance/manifest.json");
        JsonObject register = RegisterValidator.Read(relative: "docs/code/register.json");
        JsonObject exemptions = RegisterValidator.Read(relative: "docs/code/exemptions.json");
        RegisterValidator.Validate(manifest: manifest, register: register, exemptions: exemptions).ShouldBeEmpty();
        foreach (string id in _numericRules)
        {
            foreach (string value in _invalidNumbers)
            {
                JsonObject changed = register.DeepClone().AsObject();
                JsonObject numeric = Numeric(register: changed, id: id);
                if (id is "R-49" or "R-82")
                {
                    numeric["limits"] = new JsonArray(items: [JsonNode.Parse(json: value),
                        JsonValue.Create(value: 80)]);
                }
                else
                {
                    numeric["limit"] = JsonNode.Parse(json: value);
                }

                RegisterValidator.Validate(manifest: manifest, register: changed, exemptions: exemptions)
                        .ShouldContain(expected: id + ": NUMERIC_VALUE", comparer: StringComparer.Ordinal);
            }
        }

        ProofProtocol.Record(id: "edge.rule-03.precision", diagnostic: "NUMERIC_VALUE_REJECTED",
                location: "all numeric metadata: exact integer/overflow/fraction rejection; measurement pending 15");
    }

    /// <summary>Actual analyzers enforce nearest neighbors using declared line, space and UTF-16 units.</summary>
    [Fact, Trait("Case", "edge.rule-03.boundary")]
    public async Task Analyze_NumericSourceNeighbors_EnforcesDeclaredUnits()
    {
        DiagnosticAnalyzer line = Load(name: "LineLength");
        foreach (string sequence in _sequences)
        {
            foreach (int units in _lineUnits)
            {
                foreach (string newline in _newlines)
                {
                    await VerifyLineAsync(analyzer: line, units: units, sequence: sequence,
                            newline: newline);
                }
            }
        }

        DiagnosticAnalyzer body = Load(name: "BodyLength");
        foreach (int count in _bodyLines)
        {
            await VerifyBodyAsync(analyzer: body, count: count);
        }

        DiagnosticAnalyzer continuation = Load(name: "ContinuationIndent");
        foreach (int offset in _continuations)
        {
            await VerifyContinuationAsync(analyzer: continuation, offset: offset);
        }

        SourceFileFormat.RequireEditorSetting(source: "tab_width = 4\n", name: "tab_width", expected: "4");
        foreach (int width in new[] { 3, 5 })
        {
            Should.Throw<InvalidDataException>(actual: () => SourceFileFormat.RequireEditorSetting(
                    source: "tab_width = " + width.ToString(provider: System.Globalization.CultureInfo
                            .InvariantCulture),
                    name: "tab_width", expected: "4"));
        }

        ProofProtocol.Record(id: "edge.rule-03.boundary", diagnostic: "NUMERIC_SOURCE_NEIGHBORS_EXACT",
                location: "actual UTF-16 119/120/121 LF/CRLF; bodies 59/60/61; continuation 7/8/9; tab 3/4/5");
    }

    /// <summary>Invalid UTF-8 and byte-probe neighbors retain exact bytes without normalization.</summary>
    [Fact, Trait("Case", "edge.rule-03.encoding")]
    public void Decode_InvalidUtf8AndProbeNeighbors_PreservesExactUnits()
    {
        IReadOnlyList<string> invalid = SourceFileFormat.Check(path: "Subject.txt", content: [0xFF, (byte)'\n']);
        invalid.Count.ShouldBe(expected: 1);
        invalid[0].ShouldBe(expected: "R-02 FORMAT_UTF8 Subject.txt(byte 1)");
        "é".Length.ShouldBe(expected: 1);
        "é".Length.ShouldBe(expected: 2);
        "😀".Length.ShouldBe(expected: 2);
        foreach (int index in new[] { 7998, 7999, 8000 })
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s: new string(c: 'a', count: 8001));
            bytes[index] = 0;
            SourceFileFormat.IsText(content: bytes).ShouldBe(expected: index >= 8000);
        }

        ProofProtocol.Record(id: "edge.rule-03.encoding", diagnostic: "NUMERIC_ENCODING_EXACT",
                location: "UTF-8 byte1; supplementary/combining UTF-16; NUL probe indices7998/7999/8000");
    }

    /// <summary>Permuting register/exemption inputs preserves the exact numeric verdict.</summary>
    [Fact, Trait("Case", "edge.rule-03.ordering")]
    public void Validate_PermutedNumericAndScopeInputs_PreservesVerdicts()
    {
        JsonObject manifest = RegisterValidator.Read(relative: "docs/code/provenance/manifest.json");
        JsonObject register = RegisterValidator.Read(relative: "docs/code/register.json");
        JsonObject exemptions = RegisterValidator.Read(relative: "docs/code/exemptions.json");
        RegisterValidator.Validate(manifest: manifest, register: register, exemptions: exemptions).ShouldBeEmpty();
        Reverse(root: manifest, name: "matrix");
        Reverse(root: manifest, name: "exemptions");
        Reverse(root: register, name: "rules");
        Reverse(root: exemptions, name: "local");
        Reverse(root: exemptions, name: "donorExemptions");
        RegisterValidator.Validate(manifest: manifest, register: register, exemptions: exemptions).ShouldBeEmpty();
        Numeric(register: register, id: "R-10")["limit"] = 121;
        string[] before = [..RegisterValidator.Validate(manifest: manifest, register: register,
                exemptions: exemptions)];
        Reverse(root: register, name: "rules");
        Reverse(root: exemptions, name: "local");
        string[] after = [..RegisterValidator.Validate(manifest: manifest, register: register,
                exemptions: exemptions)];
        before.ShouldContain(expected: "R-10: NUMERIC_LIMIT", comparer: StringComparer.Ordinal);
        after.ShouldBe(expected: before);
        ProofProtocol.Record(id: "edge.rule-03.ordering", diagnostic: "NUMERIC_SCOPE_ORDER_INVARIANT",
                location: "detached exact register/scope permutations and intended R-10 numeric drift");
    }

    private static readonly string[] _numericRules = ["R-07", "R-08", "R-09", "R-10", "R-37", "R-49", "R-74", "R-82"];
    private static readonly string[] _invalidNumbers = ["2147483648", "-2147483649", "9223372036854775808",
        "1.5", "1e1000", "\"4\"", "true"];
    private static readonly string[] _sequences = ["a", "я", "😀", "é"];
    private static readonly string[] _newlines = ["\n", "\r\n"];
    private static readonly int[] _lineUnits = [119, 120, 121];
    private static readonly int[] _bodyLines = [59, 60, 61];
    private static readonly int[] _continuations = [7, 8, 9];

    private static DiagnosticAnalyzer Load(string name)
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll")));
        Type type = assembly.GetType(name: "Sebreiro.CodeRules.Analyzers." + name + "Analyzer")
                ?? throw new InvalidOperationException(message: "NUMERIC_ANALYZER_REQUIRED " + name);
        return Activator.CreateInstance(type: type) as DiagnosticAnalyzer
                ?? throw new InvalidOperationException(message: "NUMERIC_ANALYZER_REQUIRED " + name);
    }

    private static Task VerifyLineAsync(DiagnosticAnalyzer analyzer, int units, string sequence, string newline)
    {
        string source = "//" + string.Concat(values: Enumerable.Repeat(element: sequence,
                count: (units - 2) / sequence.Length));
        source += new string(c: 'a', count: units - source.Length);
        source.Length.ShouldBe(expected: units);
        AnalyzerVerifier.Expected[] expected = units <= 120 ? [] : [new AnalyzerVerifier.Expected(id: "SCR0007",
                severity: DiagnosticSeverity.Error, file: "Subject.cs", line: 1, column: 121, endLine: 1,
                endColumn: 122)];
        return AnalyzerVerifier.VerifyAsync(source: source + newline + "class C { }" + newline,
                expected: expected, analyzer: analyzer);
    }

    private static Task VerifyBodyAsync(DiagnosticAnalyzer analyzer, int count)
    {
        string source = "class C { private int _x; void M()\n{\n" + string.Join(separator: "\n",
                values: Enumerable.Repeat(element: "        _x++;", count: count)) + "\n} }";
        int column = source.IndexOf(value: "M()", comparisonType: StringComparison.Ordinal) + 1;
        AnalyzerVerifier.Expected[] expected = count <= 60 ? [] : [new AnalyzerVerifier.Expected(id: "SCR0006",
                severity: DiagnosticSeverity.Error, file: "Subject.cs", line: 1, column: column, endLine: 1,
                endColumn: column + 1)];
        return AnalyzerVerifier.VerifyAsync(source: source, expected: expected, analyzer: analyzer);
    }

    private static Task VerifyContinuationAsync(DiagnosticAnalyzer analyzer, int offset)
    {
        int spaces = 8 + offset;
        string source = "class C {\n    static int Echo(int value) { return value; }\n    static int Read() {\n"
                + "        return Echo(\n" + new string(c: ' ', count: spaces) + "value: 1);\n    }\n}\n";
        AnalyzerVerifier.Expected[] expected = offset == 8 ? [] : [new AnalyzerVerifier.Expected(id: "SCR0009",
                severity: DiagnosticSeverity.Error, file: "Subject.cs", line: 5, column: spaces + 1, endLine: 5,
                endColumn: spaces + 6)];
        return AnalyzerVerifier.VerifyAsync(source: source, expected: expected, analyzer: analyzer);
    }

    private static JsonObject Numeric(JsonObject register, string id)
    {
        return register["rules"]?.AsArray().Single(predicate: row => StringComparer.Ordinal.Equals(
                x: row?["sourceId"]?.GetValue<string>(), y: id))?["numeric"]?.AsObject()
                ?? throw new InvalidOperationException(message: "NUMERIC_SUBJECT_REQUIRED " + id);
    }

    private static void Reverse(JsonObject root, string name)
    {
        JsonArray rows = root[name]?.AsArray() ?? throw new InvalidOperationException(message: "INPUT_ARRAY_REQUIRED");
        root[name] = new JsonArray(items: [.. rows.Reverse().Select(selector: row => row?.DeepClone())]);
    }
}
