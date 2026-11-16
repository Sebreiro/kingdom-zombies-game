using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Validates immutable donor capture identity and exact local rule, proof and exemption mappings.</summary>
public static class RegisterValidator
{
    /// <summary>Identifies the original donor Git revision used by the captured source archive.</summary>
    public const string Revision = "6836a9718ff5c939e89abf2b97763760607de192";
    /// <summary>Identifies the SHA-256 digest required for the immutable captured donor archive.</summary>
    public const string ArchiveHash = "19c3766fddf638b02a2b78d969e222448ecd98a22b79f70e8b28effd858dfcca";
    /// <summary>Reads a repository-relative JSON object with a four-megabyte size limit and strict parsing.</summary>
    public static JsonObject Read(string relative)
    {
        string file = Path.Combine(path1: ProofProtocol.Root, path2: relative);
        if (new FileInfo(fileName: file).Length > 4_000_000)
        {
            throw new InvalidDataException(message: "JSON_SIZE_LIMIT");
        }

        return Parse(bytes: File.ReadAllBytes(path: file));
    }

    /// <summary>Parses strict UTF-8 JSON, rejecting duplicate property names and non-object roots.</summary>
    public static JsonObject Parse(byte[] bytes)
    {
        string text = _strictUtf8.GetString(bytes: bytes);
        using var document = JsonDocument.Parse(json: text, options: new JsonDocumentOptions { MaxDepth = 64 });
        CheckProperties(element: document.RootElement);
        return JsonNode.Parse(json: text) as JsonObject ?? throw new InvalidDataException(
                message: "JSON_OBJECT_REQUIRED");
    }

    /// <summary>Returns capture and mapping diagnostics; requiring implementation also rejects pending
    /// mechanisms and reviews.</summary>
    public static IReadOnlyList<string> Validate(JsonObject manifest, JsonObject register,
            JsonObject exemptions, bool requireImplemented = false)
    {
        var errors = new List<string>();
        try
        {
            ValidateCapture(manifest: manifest, register: register, exemptions: exemptions, errors: errors);
            ValidateMappings(manifest: manifest, register: register, exemptions: exemptions,
                    implemented: requireImplemented, errors: errors);
        }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException
                or KeyNotFoundException or InvalidDataException or DecoderFallbackException)
        {
            errors.Add(item: "REGISTER: INVALID_STRUCTURE: " + error.Message);
        }

        return errors;
    }

    private const string DonorDocs = "src/backend/docs/";
    private static readonly UTF8Encoding _strictUtf8 = new(encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: true);
    private static readonly int[] _coverageLimits = [90, 80];
    private static readonly int[] _bodyBoundaries = [59, 60, 61];
    private static readonly string[] _bodyProofs = ["rules.body.59", "rules.body.60", "rules.body.61"];
    private static readonly Dictionary<string, string> _analyzers = new(comparer: StringComparer.Ordinal)
    {
        ["R-09"] = "ContinuationIndent",
        ["R-10"] = "LineLength",
        ["R-14"] = "UsingOrder",
        ["R-18"] = "LocalNewDeclaration",
        ["R-23"] = "PrimaryConstructor",
        ["R-24"] = "PrimaryConstructor",
        ["R-25"] = "RegionDirective",
        ["R-26"] = "NullForgiving",
        ["R-28"] = "ThisQualification",
        ["R-33"] = "MemberOrder",
        ["R-36"] = "BodyComment",
        ["R-37"] = "BodyLength",
        ["R-38"] = "NamedArgument",
    };
    private static readonly Dictionary<string, (string File, string Symbol)> _architecture = new(
            comparer: StringComparer.Ordinal)
    {
        ["R-34"] = ("ArchitecturePolicy", "Visibility"),
        ["R-51"] = ("ProjectEvaluator", "Validate"),
        ["R-52"] = ("ProjectEvaluator", "Validate"),
        ["R-53"] = ("GraphTests", "VerifyCompiler"),
        ["R-54"] = ("ProjectEvaluator", "Validate"),
        ["R-55"] = ("TypeBoundaryScanner", "Validate"),
        ["R-56"] = ("TypeBoundaryScanner", "Validate"),
        ["R-58"] = ("TypeBoundaryScanner", "Validate"),
        ["R-59"] = ("TypeBoundaryScanner", "Validate"),
        ["R-60"] = ("ArchitecturePolicy", "ServiceLocator"),
        ["R-61"] = ("TypeBoundaryScanner", "Validate"),
        ["R-62"] = ("TypeBoundaryScanner", "Validate"),
        ["R-63"] = ("ArchitecturePolicy", "Model"),
        ["R-64"] = ("ArchitecturePolicy", "Model"),
        ["R-65"] = ("ArchitecturePolicy", "Mapping"),
        ["R-66"] = ("ArchitecturePolicy", "Version"),
        ["R-74"] = ("ArchitecturePolicy", "Enum"),
        ["R-75"] = ("ArchitecturePolicy", "Enum"),
        ["R-77"] = ("ArchitecturePolicy", "Enum"),
        ["R-78"] = ("TypeBoundaryScanner", "Validate"),
        ["R-81"] = ("ArchitecturePolicy", "Model"),
    };
    private static void CheckProperties(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (!names.Add(item: property.Name))
                {
                    throw new InvalidDataException(message: "JSON_DUPLICATE: " + property.Name);
                }

                CheckProperties(element: property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in element.EnumerateArray())
            {
                CheckProperties(element: item);
            }
        }
    }

    private static string ExpectedMechanism(string id)
    {
        if (_analyzers.TryGetValue(key: id, value: out string? name))
        {
            return "tools/code-rules/" + name + "Analyzer.cs";
        }

        if (_architecture.TryGetValue(key: id, value: out (string File, string Symbol) actual))
        {
            return "src/godot-game/tests/verification/" + actual.File + ".cs";
        }

        int number = int.Parse(s: id[2..], provider: System.Globalization.CultureInfo.InvariantCulture);
        return number switch
        {
            67 => "src/godot-game/tests/verification/SuppressionPolicy.cs",
            73 => "src/godot-game/build/BannedSymbols.txt",
            >= 68 and <= 78 => "src/godot-game/core/SnapshotCodec.cs",
            48 => "src/godot-game/tests/verification/TestSourceIndex.cs",
            50 => "src/godot-game/tests/verification/ExemptionValidator.cs",
            49 or 82 => "tools/phase1/coverage-policy.json",
            >= 83 => "src/godot-game/tests/verification/DocumentationValidator.cs",
            >= 51 => "src/godot-game/tests/verification/ArchitectureRuleTests.cs",
            <= 8 => "src/godot-game/tests/verification/EnforcementTests.cs",
            _ => ".editorconfig",
        };
    }

    private static bool ExactPath(string relative)
    {
        if (string.IsNullOrWhiteSpace(value: relative) || relative.Contains(value: '\\') || relative.Contains(
                value: ':') || relative.StartsWith(value: '/'))
        {
            return false;
        }

        string current = ProofProtocol.Root;
        foreach (string part in relative.Split(separator: '/'))
        {
            if (part is "" or "." or ".." || !Directory.Exists(path: current))
            {
                return false;
            }

            string[] matches = [..Directory.EnumerateFileSystemEntries(path: current).Where(predicate: p => Path
                    .GetFileName(path: p).Equals(value: part, comparisonType: StringComparison.Ordinal))];
            if (matches.Length != 1 || (File.GetAttributes(path: matches[0]) & FileAttributes.ReparsePoint)
                    != FileAttributes.None)
            {
                return false;
            }

            current = matches[0];
        }

        return File.Exists(path: current) && new FileInfo(fileName: current).Length > 0;
    }

    private static void ValidateMappings(JsonObject manifest, JsonObject register, JsonObject exemptions,
            bool implemented, List<string> errors)
    {
        JsonArray cases = (Read(relative: "tools/phase1/required-cases.json")["cases"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null")).AsArray();
        Dictionary<string, JsonNode> declarations = Unique(rows: cases, key: n => Text(node: n["id"]),
                diagnostic: "PROOF_DECLARATION", errors: errors);
        Dictionary<string, JsonNode> sources = Unique(rows: Rows(node: manifest, name: "matrix"), key:
                n => Text(node: n["sourceId"]), diagnostic: "MATRIX_ID", errors: errors);
        var reviewRules = new HashSet<string>(collection: ["R-24", "R-34", "R-62", "R-63", "R-65", "R-77",
            "R-83", "R-84"], comparer: StringComparer.Ordinal);
        var inapplicable = new HashSet<string>(collection: ["R-43", "R-57", "R-76", "R-79", "R-80", "R-85"],
                comparer: StringComparer.Ordinal);
        var adopted = new HashSet<string>(collection: ["R-02", "R-03", "R-04", "R-05", "R-06", "R-24", "R-34",
            "R-40", "R-44", "R-45", "R-49", "R-50", "R-51", "R-52", "R-54", "R-55", "R-56", "R-58", "R-59",
            "R-61", "R-63", "R-64", "R-68", "R-69", "R-70", "R-71", "R-72", "R-73", "R-78", "R-81", "R-82"],
                comparer: StringComparer.Ordinal);
        foreach (JsonNode? rule in Rows(node: register, name: "rules"))
        {
            if (rule is null)
            {
                continue;
            }

            string id = Text(node: rule["sourceId"]);
            if (!sources.TryGetValue(key: id, value: out JsonNode? source))
            {
                continue;
            }

            ValidateRuleDisposition(rule: rule, id: id, inapplicable: inapplicable, adopted: adopted, errors: errors);
            ValidateReview(rule: rule, id: id, required: reviewRules.Contains(item: id) || Text(node: source[
                    "level"]).Contains(value: "PR review", comparisonType: StringComparison.Ordinal),
                    implemented: implemented, errors: errors);
            if (inapplicable.Contains(item: id))
            {
                if (rule["mechanism"] is not null || rule["positiveProof"] is not null || rule[
                        "negativeProof"] is not null)
                {
                    errors.Add(item: id + ": INAPPLICABLE_MAPPING");
                }

                if (!declarations.TryGetValue(key: id + ".register-resolution",
                        value: out JsonNode? resolution) || !StringComparer.Ordinal.Equals(x: Text(
                                node: resolution["rule"]), y: id))
                {
                    errors.Add(item: id + ": RESOLUTION_REQUIRED");
                }

                continue;
            }

            ValidateMechanism(rule: rule, source: source, id: id, implemented: implemented, errors: errors);
            ValidateProofs(rule: rule, id: id, declarations: declarations, implemented: implemented, errors: errors);
            ValidateNumeric(id: id, numeric: rule["numeric"], errors: errors);
        }

        ValidateExemptions(manifest: manifest, register: register, exemptions: exemptions,
                implemented: implemented, errors: errors);
    }

    private static void ValidateReview(JsonNode rule, string id, bool required, bool implemented, List<string> errors)
    {
        JsonNode? qualitative = rule["qualitativeReview"];
        if (qualitative is null || qualitative["required"]?.GetValue<bool>() != required)
        {
            errors.Add(item: id + ": REVIEW_REQUIRED");
        }
        else if (required && (!StringComparer.Ordinal.Equals(x: Text(node: qualitative["status"]),
                y: "pending") || string.IsNullOrWhiteSpace(value: Text(node: qualitative["obligation"]))))
        {
            errors.Add(item: id + ": HUMAN_REVIEW_PENDING");
        }
        else if (!required && !StringComparer.Ordinal.Equals(x: Text(node: qualitative["status"]), y: "not-required"))
        {
            errors.Add(item: id + ": REVIEW_STATUS");
        }

        if (implemented && required)
        {
            errors.Add(item: id + ": HUMAN_REVIEW_PENDING");
        }
    }

    private static void ValidateMechanism(JsonNode rule, JsonNode source, string id, bool implemented,
            List<string> errors)
    {
        JsonNode? mechanism = rule["mechanism"];
        if (mechanism is null)
        {
            errors.Add(item: id + ": MECHANISM_REQUIRED");
        }
        else
        {
            string expectedSymbol = _analyzers.TryGetValue(key: id, value: out string? analyzerName)
                    ? analyzerName + "Analyzer" : id is "R-67" ? "CheckRepository" : _architecture.TryGetValue(key: id,
                            value: out (string File, string Symbol) actual) ? actual.Symbol : id;
            if (!StringComparer.Ordinal.Equals(x: Text(node: mechanism["path"]), y: ExpectedMechanism(id: id))
                    || !StringComparer.Ordinal.Equals(x: Text(node: mechanism["symbol"]), y: expectedSymbol))
            {
                errors.Add(item: id + ": MECHANISM_REFERENCE");
            }

            IEnumerable<string> diagnostics = Regex.Matches(input: Text(node: source["mechanism"]),
                    pattern: @"\b(?:SCR|IDE|CA|MA|RCS|RS|CS)\d{4}\b", options: RegexOptions.CultureInvariant
                            | RegexOptions.NonBacktracking, matchTimeout: TimeSpan.FromSeconds(seconds: 1))
                    .Select(selector: m => m.Value).Distinct(comparer: StringComparer.Ordinal);
            SameStrings(expected: diagnostics, actual: (mechanism["diagnostics"]
                    ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                    .AsArray(), diagnostic: id + ": DIAGNOSTIC_IDENTITY", errors: errors);
            if (!StringComparer.Ordinal.Equals(x: Text(node: mechanism["severity"]), y: "error"))
            {
                errors.Add(item: id + ": SEVERITY_WEAKENED");
            }

            if (!StringComparer.Ordinal.Equals(x: Text(node: mechanism["status"]), y: "pending"))
            {
                errors.Add(item: id + ": UNVERIFIED_IMPLEMENTATION");
            }

            if (implemented && !StringComparer.Ordinal.Equals(x: Text(node: rule["binding"]?["status"]),
                    y: "implemented"))
            {
                errors.Add(item: id + ": IMPLEMENTATION_PENDING");
            }
        }
    }

    private static void ValidateProofs(JsonNode rule, string id, Dictionary<string, JsonNode> declarations,
            bool implemented, List<string> errors)
    {
        foreach (string? polarity in new[]
        {
            "positive",
            "negative"
        }

        )
        {
            JsonNode? proof = rule[polarity + "Proof"];
            if (proof is null)
            {
                errors.Add(item: id + ": " + polarity.ToUpperInvariant() + "_PROOF_REQUIRED");
                continue;
            }

            string proofId = Text(node: proof["id"]);
            if (!StringComparer.Ordinal.Equals(x: proofId, y: id + "." + polarity) || !StringComparer.Ordinal
                    .Equals(x: Text(node: proof["rule"]), y: id) || !declarations.TryGetValue(key: proofId,
                            value: out JsonNode? declaration) || !StringComparer.Ordinal.Equals(x: Text(
                                    node: declaration["rule"]), y: id) || !StringComparer.Ordinal.Equals(
                                            x: Text(node: declaration["polarity"]), y: polarity))
            {
                errors.Add(item: id + ": PROOF_REVERSE_ID");
            }

            if (!StringComparer.Ordinal.Equals(x: Text(node: proof["status"]), y: "pending"))
            {
                errors.Add(item: id + ": UNVERIFIED_PROOF");
            }

            int number = int.Parse(s: id[2..], provider: System.Globalization.CultureInfo.InvariantCulture);
            string expectedProof = _analyzers.TryGetValue(key: id, value: out string? analyzer)
                    ? "src/godot-game/tests/verification/Analyzers/" + analyzer + "AnalyzerTests.cs" : number switch
                    {
                        34 or 63 or 64 or 65 or 66 or 74 or 75 or 77 or 81 => "src/godot-game/tests/verification/ArchitectureRuleTests.cs",
                        >= 51 and <= 62 or 78 => "src/godot-game/tests/verification/GraphTests.cs",
                        67 => "src/godot-game/tests/verification/SuppressionTests.cs",
                        73 => "src/godot-game/tests/verification/RandomContractTests.cs",
                        >= 68 and <= 78 => "src/godot-game/tests/verification/SnapshotTests.cs",
                        48 => "src/godot-game/tests/verification/TestMethodNameTests.cs",
                        50 => "src/godot-game/tests/verification/ExemptionTests.cs",
                        49 or 82 => "src/godot-game/tests/verification/CoverageTests.cs",
                        >= 83 => "src/godot-game/tests/verification/DocumentationTests.cs",
                        >= 51 => "src/godot-game/tests/verification/ArchitectureRuleTests.cs",
                        _ => "src/godot-game/tests/verification/EnforcementTests.cs",
                    };
            if (!StringComparer.Ordinal.Equals(x: Text(node: proof["path"]), y: expectedProof))
            {
                errors.Add(item: id + ": PROOF_PATH");
            }

            if (implemented && (!declarations.TryGetValue(key: proofId, value: out JsonNode? actual)
                    || !StringComparer.Ordinal.Equals(x: Text(node: actual["implementation"]), y: "csharp")
                    || !ExactPath(relative: Text(node: proof["path"]))))
            {
                errors.Add(item: id + ": PROOF_UNIMPLEMENTED");
            }
        }
    }

    private static void ValidateNumeric(string id, JsonNode? numeric, List<string> errors)
    {
        try
        {
            ValidateNumericValue(id: id, numeric: numeric, errors: errors);
        }
        catch (Exception error) when (error is FormatException or OverflowException or InvalidOperationException)
        {
            errors.Add(item: id + ": NUMERIC_VALUE");
        }
    }

    private static void ValidateNumericValue(string id, JsonNode? numeric, List<string> errors)
    {
        var limits = new Dictionary<string, int>(comparer: StringComparer.Ordinal)
        {
            ["R-07"] = 4,
            ["R-08"] = 4,
            ["R-09"] = 8,
            ["R-10"] = 120,
            ["R-37"] = 60,
            ["R-74"] = 0
        };
        if (!limits.ContainsKey(key: id) && id is not "R-49" and not "R-82")
        {
            return;
        }

        if (numeric is null || string.IsNullOrWhiteSpace(value: Text(node: numeric["unit"])) || string
                .IsNullOrWhiteSpace(value: Text(node: numeric["comparison"])) || string.IsNullOrWhiteSpace(
                        value: Text(node: numeric["rounding"])))
        {
            errors.Add(item: id + ": NUMERIC_SEMANTICS");
            return;
        }

        if (limits.TryGetValue(key: id, value: out int limit) && numeric["limit"]?.GetValue<int>() != limit)
        {
            errors.Add(item: id + ": NUMERIC_LIMIT");
        }

        if (id is "R-49" or "R-82" && !(numeric["limits"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Select(selector: n => (n
                        ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                        .GetValue<int>()).SequenceEqual(second: _coverageLimits))
        {
            errors.Add(item: id + ": NUMERIC_LIMIT");
        }

        if (StringComparer.Ordinal.Equals(x: id, y: "R-37") && !(numeric["boundaries"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsArray().Select(selector: n => (n ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null")).GetValue<int>()).SequenceEqual(
                                second: _bodyBoundaries))
        {
            errors.Add(item: id + ": NUMERIC_BOUNDARIES");
        }

        ValidateNumericDescription(id: id, numeric: numeric, errors: errors);
    }

    private static void ValidateExemptions(JsonObject manifest, JsonObject register, JsonObject exemptions,
            bool implemented, List<string> errors)
    {
        Dictionary<string, JsonNode> sources = Unique(rows: Rows(node: manifest, name: "exemptions"), key:
                n => Text(node: n["id"]), diagnostic: "SOURCE_EXEMPTION", errors: errors);
        Dictionary<string, JsonNode> local = Unique(rows: Rows(node: exemptions, name: "local"), key: n => Text(
                node: n["id"]), diagnostic: "LOCAL_EXEMPTION_ID", errors: errors);
        ValidateLocalInventory(local: local, errors: errors);
        foreach (JsonNode? entry in Rows(node: exemptions, name: "donorExemptions"))
        {
            if (entry is null)
            {
                continue;
            }

            string id = Text(node: entry["sourceId"]);
            if (!sources.TryGetValue(key: id, value: out JsonNode? source) || !StringComparer.Ordinal.Equals(
                    x: Text(node: entry["rule"]), y: Text(node: source["sourceId"])))
            {
                errors.Add(item: id + ": EXEMPTION_REVERSE_ID");
            }

            string expectedDisposition = new[]
            {
                "EX-01",
                "EX-02",
                "EX-03",
                "EX-11",
                "EX-13",
                "EX-19"
            }.Contains(value: id, comparer: StringComparer.Ordinal) ? "adapted" : "inapplicable";
            if (!StringComparer.Ordinal.Equals(x: Text(node: entry["disposition"]), y: expectedDisposition))
            {
                errors.Add(item: id + ": EXEMPTION_DISPOSITION");
            }

            SameStrings(expected: local.Where(predicate: pair => StringComparer.Ordinal.Equals(x: Text(
                    node: pair.Value["sourceId"]), y: id)).Select(selector: pair => pair.Key), actual: (entry[
                            "localIds"] ?? throw new System.InvalidOperationException(
                                    message: "Required proof value is null")).AsArray(), diagnostic: id
                                            + ": LOCAL_EXEMPTION_REFERENCE", errors: errors);
        }

        ValidateLocalExemptions(local: local, implemented: implemented, errors: errors);
        foreach (JsonNode? rule in Rows(node: register, name: "rules"))
        {
            SameStrings(expected: local.Where(predicate: pair => StringComparer.Ordinal.Equals(x: Text(
                    node: pair.Value["rule"]), y: Text(node: (rule
                            ?? throw new System.InvalidOperationException(
                                    message: "Required proof value is null"))["sourceId"]))).Select(selector:
                                            pair => pair.Key), actual: ((rule
                                                    ?? throw new System.InvalidOperationException(
                                                            message: "Required proof value is null"))[
                                                                    "exemptions"]
                                                    ?? throw new System.InvalidOperationException(
                                                            message: "Required proof value is null"))
                                                    .AsArray(), diagnostic: Text(node: rule["sourceId"])
                                                            + ": EXEMPTION_REFERENCE", errors: errors);
        }
    }

    private static void ValidateLocalInventory(Dictionary<string, JsonNode> local, List<string> errors)
    {
        if (!local.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(
                other: ExemptionValidator.ExpectedLocalIds()))
        {
            errors.Add(item: "REGISTER: LOCAL_EXEMPTION_INVENTORY");
        }
    }

    private static string Text(JsonNode? node)
    {
        return node?.GetValue<string>() ?? "";
    }

    private static JsonArray Rows(JsonObject node, string name)
    {
        return node[name] as JsonArray ?? throw new InvalidDataException(message: "ARRAY_REQUIRED: " + name);
    }

    private static string Hash(byte[] bytes)
    {
        return Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes));
    }

    private static string HeadingKey(JsonNode node)
    {
        return Text(node: node["path"]) + ":" + (node["line"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).GetValue<int>();
    }

    private static Dictionary<string, JsonNode> Unique(JsonArray rows, Func<JsonNode, string> key,
            string diagnostic, List<string> errors)
    {
        var result = new Dictionary<string, JsonNode>(comparer: StringComparer.Ordinal);
        foreach (JsonNode? row in rows)
        {
            if (row is null)
            {
                errors.Add(item: diagnostic + ": NULL_ROW");
                continue;
            }

            string id = key(row);
            if (string.IsNullOrWhiteSpace(value: id) || !result.TryAdd(key: id, value: row))
            {
                errors.Add(item: id + ": " + diagnostic + ": DUPLICATE_OR_EMPTY");
            }
        }

        return result;
    }

    private static void SameStrings(IEnumerable<string> expected, JsonArray? actual, string diagnostic,
            List<string> errors)
    {
        if (actual is null)
        {
            errors.Add(item: diagnostic);
            return;
        }

        string[] values = [.. actual.Select(selector: Text)];
        if (values.Distinct(comparer: StringComparer.Ordinal).Count() != values.Length || !expected.ToHashSet(
                comparer: StringComparer.Ordinal).SetEquals(other: values))
        {
            errors.Add(item: diagnostic);
        }
    }

    private static Dictionary<string, byte[]> Archive(List<string> errors)
    {
        string file = Path.Combine(path1: ProofProtocol.Root, path2: "docs/code/provenance/langproj-baseline.zip");
        if (new FileInfo(fileName: file).Length > 20_000_000)
        {
            throw new InvalidDataException(message: "Archive size limit");
        }

        byte[] bytes = File.ReadAllBytes(path: file);
        if (!StringComparer.Ordinal.Equals(x: Hash(bytes: bytes), y: ArchiveHash))
        {
            throw new InvalidDataException(message: "CAPTURE: ARCHIVE_HASH");
        }

        using var stream = new MemoryStream(buffer: bytes);
        using var zip = new ZipArchive(stream: stream, mode: ZipArchiveMode.Read);
        var files = new Dictionary<string, byte[]>(comparer: StringComparer.Ordinal);
        long total = 0;
        foreach (ZipArchiveEntry entry in zip.Entries)
        {
            if (entry.FullName.EndsWith(value: '/'))
            {
                continue;
            }

            if (entry.Length > 4_000_000 || (total += entry.Length) > 20_000_000)
            {
                throw new InvalidDataException(message: "Expanded size limit");
            }

            if (entry.FullName.StartsWith(value: '/') || entry.FullName.Contains(value: '\\') || entry.FullName
                    .Split(separator: '/').Any(predicate: x => x is ".." or "." or ""))
            {
                throw new InvalidDataException(message: "Noncanonical ZIP path");
            }

            using Stream input = entry.Open();
            using var output = new MemoryStream();
            input.CopyTo(destination: output);
            if (!files.TryAdd(key: entry.FullName, value: output.ToArray()))
            {
                errors.Add(item: "CAPTURE: DUPLICATE_ZIP_PATH: " + entry.FullName);
            }
        }

        return files;
    }

    private static void ValidateCapture(JsonObject manifest, JsonObject register, JsonObject exemptions,
            List<string> errors)
    {
        if (!StringComparer.Ordinal.Equals(x: Text(node: manifest["revision"]), y: Revision) || !StringComparer
                .Ordinal.Equals(x: Text(node: register["revision"]), y: Revision) || manifest[
                        "cleanAtCapture"]?.GetValue<bool>() != true)
        {
            errors.Add(item: "CAPTURE: REVISION");
        }

        if (!StringComparer.Ordinal.Equals(x: Text(node: manifest["archiveSha256"]), y: ArchiveHash)
                || !StringComparer.Ordinal.Equals(x: Text(node: register["archiveSha256"]), y: ArchiveHash))
        {
            errors.Add(item: "CAPTURE: ARCHIVE_IDENTITY");
        }

        Dictionary<string, byte[]> files = Archive(errors: errors);
        Dictionary<string, JsonNode> capturedFiles = Unique(rows: Rows(node: manifest, name: "files"), key:
                n => Text(node: n["path"]), diagnostic: "CAPTURE_FILE", errors: errors);
        if (!files.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: capturedFiles.Keys))
        {
            errors.Add(item: "CAPTURE: FILE_INVENTORY");
        }

        foreach ((string? path, JsonNode? row) in capturedFiles)
        {
            if (!files.TryGetValue(key: path, value: out byte[]? bytes) || !StringComparer.Ordinal.Equals(
                    x: Hash(bytes: bytes), y: Text(node: row["sha256"])) || bytes.Length != (row["size"]
                            ?? throw new System.InvalidOperationException(
                                    message: "Required proof value is null")).GetValue<long>()
                    || !StringComparer.Ordinal.Equals(x: Text(node: row["modification"]), y: "unmodified"))
            {
                errors.Add(item: "CAPTURE: FILE_HASH: " + path);
            }
        }

        List<JsonNode> expectedHeadings = CaptureHeadings(files: files);
        ValidateHeadings(manifest: manifest, expectedHeadings: expectedHeadings, errors: errors);
        ValidateSourceRows(manifest: manifest, register: register, exemptions: exemptions, files: files,
                expectedHeadings: expectedHeadings, errors: errors);
        if (Text(node: manifest["licenseNotice"]?["finding"]).Length == 0 || Text(node: manifest[
                "licenseNotice"]?["modifications"]).Length == 0)
        {
            errors.Add(item: "CAPTURE: LICENSE_NOTICE");
        }
    }

    private static List<JsonNode> CaptureHeadings(Dictionary<string, byte[]> files)
    {
        var expectedHeadings = new List<JsonNode>();
        foreach (string? name in new[]
        {
            "code-style.md",
            "code-architecture.md"
        }

        )
        {
            string path = DonorDocs + "CodeRules/" + name;
            string[] lines = _strictUtf8.GetString(bytes: files[path]).Split(separator: '\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].StartsWith(value: "### ", comparisonType: StringComparison.Ordinal))
                {
                    continue;
                }

                int end = i + 1;
                while (end < lines.Length && !lines[end].StartsWith(value: "### ",
                        comparisonType: StringComparison.Ordinal))
                {
                    end++;
                }

                string? enforcement = lines[(i + 1)..end].SingleOrDefault(predicate: x => x.StartsWith(
                        value: "Enforcement:", comparisonType: StringComparison.Ordinal));
                if (enforcement is null)
                {
                    continue;
                }

                string[] ids = [..Regex.Matches(input: enforcement, pattern: @"\[(?<id>R-\d+)\]",
                        options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking,
                        matchTimeout: TimeSpan.FromSeconds(seconds: 1)).Select(selector: m => m.Groups["id"].Value)];
                var sourceIds = new JsonArray();
                foreach (string id in ids)
                {
                    sourceIds.Add(item: JsonValue.Create(value: id));
                }

                var heading = new JsonObject
                {
                    ["path"] = path,
                    ["line"] = i + 1,
                    ["title"] = lines[i][4..].Trim(),
                    ["sourceIds"] = sourceIds
                };
                expectedHeadings.Add(item: heading);
            }
        }

        return expectedHeadings;
    }

    private static void ValidateHeadings(JsonObject manifest, List<JsonNode> expectedHeadings, List<string> errors)
    {
        if (expectedHeadings.Count != 84 || expectedHeadings.Count(predicate: n => Text(node: n["path"])
                .EndsWith(value: "/code-style.md", comparisonType: StringComparison.Ordinal)) != 49)
        {
            errors.Add(item: "CAPTURE: HEADING_COUNT");
        }

        Dictionary<string, JsonNode> headings = Unique(rows: Rows(node: manifest, name: "headings"),
                key: HeadingKey, diagnostic: "HEADING", errors: errors);
        if (!headings.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: expectedHeadings.Select(
                selector: HeadingKey)))
        {
            errors.Add(item: "CAPTURE: HEADING_INVENTORY");
        }

        foreach (JsonNode expected in expectedHeadings)
        {
            if (!headings.TryGetValue(key: HeadingKey(node: expected), value: out JsonNode? actual))
            {
                continue;
            }

            if (!StringComparer.Ordinal.Equals(x: Text(node: expected["title"]), y: Text(node: actual["title"])))
            {
                errors.Add(item: HeadingKey(node: expected) + ": HEADING_TITLE");
            }

            SameStrings(expected: (expected["sourceIds"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null")).AsArray().Select(selector: Text), actual: (actual[
                            "sourceIds"] ?? throw new System.InvalidOperationException(
                                    message: "Required proof value is null")).AsArray(), diagnostic: HeadingKey(
                                            node: expected) + ": HEADING_IDS", errors: errors);
        }
    }

    private static void ValidateSourceRows(JsonObject manifest, JsonObject register, JsonObject exemptions,
            Dictionary<string, byte[]> files, List<JsonNode> expectedHeadings, List<string> errors)
    {
        Dictionary<string, JsonNode> matrix = Unique(rows: Rows(node: manifest, name: "matrix"), key: n => Text(
                node: n["sourceId"]), diagnostic: "MATRIX_ID", errors: errors);
        Dictionary<string, JsonNode> sourceExemptions = Unique(rows: Rows(node: manifest, name: "exemptions"),
                key: n => Text(node: n["id"]), diagnostic: "SOURCE_EXEMPTION", errors: errors);
        var expectedIds = new List<string>();
        var expectedExemptions = new List<string>();
        string[] matrixLines = _strictUtf8.GetString(bytes: files[DonorDocs + "rule-matrix.md"]).Split(separator: '\n');
        for (int i = 0; i < matrixLines.Length; i++)
        {
            Match match = Regex.Match(input: matrixLines[i],
                    pattern: "^\\| <a id=\"(?:r|ex)-[0-9]+\"></a>(?<id>(?:R|EX)-[0-9]+) \\|",
                    options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking,
                    matchTimeout: TimeSpan.FromSeconds(seconds: 1));
            if (!match.Success)
            {
                continue;
            }

            string id = match.Groups["id"].Value;
            bool isRule = id.StartsWith(value: "R-", comparisonType: StringComparison.Ordinal);
            (isRule ? expectedIds : expectedExemptions).Add(item: id);
            Dictionary<string, JsonNode> rows = isRule ? matrix : sourceExemptions;
            if (!rows.TryGetValue(key: id, value: out JsonNode? row) || !StringComparer.Ordinal.Equals(x: Text(
                    node: row["raw"]), y: matrixLines[i]) || (row["line"]
                            ?? throw new System.InvalidOperationException(
                                    message: "Required proof value is null")).GetValue<int>() != i + 1)
            {
                errors.Add(item: id + ": SOURCE_ROW");
            }

            if (row is not null)
            {
                ValidateSourceFields(row: row, line: matrixLines[i], id: id, isRule: isRule, errors: errors);
            }
        }

        if (expectedIds.Count != 85 || !matrix.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(
                other: expectedIds))
        {
            errors.Add(item: "CAPTURE: MATRIX_INVENTORY");
        }

        if (expectedExemptions.Count != 24 || expectedExemptions.Contains(item: "EX-22") || !sourceExemptions
                .Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: expectedExemptions))
        {
            errors.Add(item: "CAPTURE: EXEMPTION_INVENTORY");
        }

        ValidateDispositionInventories(register: register, exemptions: exemptions, matrix: matrix,
                expectedIds: expectedIds, expectedExemptions: expectedExemptions,
                expectedHeadings: expectedHeadings, errors: errors);
    }

    private static void ValidateSourceFields(JsonNode row, string line, string id, bool isRule, List<string> errors)
    {
        string[] columns = [.. line.Split(separator: '|')[1..^1].Select(selector: x => x.Trim())];
        string[] names = isRule ? ["title", "mechanism", "scope", "proofs", "level"] : ["path", "subject", "reason"];
        for (int column = 0; column < names.Length; column++)
        {
            if (!StringComparer.Ordinal.Equals(x: Text(node: row[names[column]]), y: columns[column + 1]))
            {
                errors.Add(item: id + ": SOURCE_FIELD: " + names[column]);
            }
        }

        if (!isRule && !StringComparer.Ordinal.Equals(x: Text(node: row["sourceId"]), y: Regex.Match(
                input: columns[4], pattern: "R-[0-9]+", options: RegexOptions.CultureInvariant | RegexOptions
                        .NonBacktracking, matchTimeout: TimeSpan.FromSeconds(seconds: 1)).Value))
        {
            errors.Add(item: id + ": SOURCE_RULE_ID");
        }
    }

    private static void ValidateNumericDescription(string id, JsonNode numeric, List<string> errors)
    {
        var units = new Dictionary<string, string>(comparer: StringComparer.Ordinal)
        {
            ["R-07"] = "spaces",
            ["R-08"] = "spaces",
            ["R-09"] = "spaces past continued line",
            ["R-10"] = "UTF-16 code units excluding line terminator",
            ["R-37"] = ("lines between body braces including blank lines; local " + "functions also count in parent"),
            ["R-49"] = "covered/total lines and branches",
            ["R-82"] = "covered/total lines and branches",
            ["R-74"] = "enum numeric sentinel",
        };
        string comparison = id switch
        {
            "R-07" or "R-08" or "R-09" => "exact",
            "R-49" or "R-82" => "inclusive minimum percent via integer cross multiplication",
            "R-74" => "Unknown = 0 is never valid saved state",
            _ => "inclusive maximum"
        };
        string rounding = StringComparer.Ordinal.Equals(x: id, y: "R-37")
                ? "none; expression/global bodies follow MemberBodies; 40 advised only" : "none";
        if (!StringComparer.Ordinal.Equals(x: Text(node: numeric["unit"]), y: units[id]) || !StringComparer
                .Ordinal.Equals(x: Text(node: numeric["comparison"]), y: comparison) || !StringComparer.Ordinal
                        .Equals(x: Text(node: numeric["rounding"]), y: rounding))
        {
            errors.Add(item: id + ": NUMERIC_SEMANTICS");
        }

        if (StringComparer.Ordinal.Equals(x: id, y: "R-37"))
        {
            JsonArray? proofs = numeric["boundaryProofs"]?.AsArray();
            if (proofs is null)
            {
                errors.Add(item: id + ": NUMERIC_BOUNDARY_PROOFS");
                return;
            }

            Dictionary<string, JsonNode> unique = Unique(rows: proofs, key: n => Text(node: n["id"]),
                    diagnostic: "BOUNDARY_PROOF", errors: errors);
            if (!unique.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: _bodyProofs))
            {
                errors.Add(item: id + ": NUMERIC_BOUNDARY_PROOFS");
            }

            foreach (JsonNode? row in proofs)
            {
                if (!StringComparer.Ordinal.Equals(x: Text(node: row?["rule"]), y: id) || !StringComparer
                        .Ordinal.Equals(x: Text(node: row?["path"]),
                                y: "src/godot-game/tests/verification/Analyzers/BodyLengthAnalyzerTests.cs") || !StringComparer
                                        .Ordinal.Equals(x: Text(node: row?["status"]), y: "pending"))
                {
                    errors.Add(item: id + ": NUMERIC_BOUNDARY_PROOFS");
                }
            }
        }
    }

    private static void ValidateDispositionInventories(JsonObject register, JsonObject exemptions,
            Dictionary<string, JsonNode> matrix, List<string> expectedIds, List<string> expectedExemptions,
            List<JsonNode> expectedHeadings, List<string> errors)
    {
        Dictionary<string, JsonNode> rules = Unique(rows: Rows(node: register, name: "rules"), key: n => Text(
                node: n["sourceId"]), diagnostic: "RULE_ID", errors: errors);
        if (!rules.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: expectedIds))
        {
            errors.Add(item: "REGISTER: RULE_INVENTORY");
        }

        foreach ((string? id, JsonNode? rule) in rules)
        {
            if (!matrix.ContainsKey(key: id))
            {
                errors.Add(item: id + ": UNKNOWN_RULE");
                continue;
            }

            SameStrings(expected: expectedHeadings.Where(predicate: h => (h["sourceIds"]
                    ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                    .AsArray().Select(selector: Text).Contains(value: id, comparer: StringComparer.Ordinal))
                    .Select(selector: HeadingKey), actual: rule["headingRefs"] as JsonArray, diagnostic: id
                            + ": HEADING_RELATION", errors: errors);
            if (!new[]
            {
                "retained",
                "adapted",
                "inapplicable"
            }.Contains(value: Text(node: rule["disposition"]), comparer: StringComparer.Ordinal) || string
                    .IsNullOrWhiteSpace(value: Text(node: rule["reason"])))
            {
                errors.Add(item: id + ": DISPOSITION");
            }
        }

        Dictionary<string, JsonNode> dispositions = Unique(rows: Rows(node: exemptions,
                name: "donorExemptions"), key: n => Text(node: n["sourceId"]), diagnostic: "EXEMPTION_ID",
                errors: errors);
        if (!dispositions.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: expectedExemptions))
        {
            errors.Add(item: "REGISTER: DONOR_EXEMPTION_INVENTORY");
        }

        foreach ((string? id, JsonNode? entry) in dispositions)
        {
            if (!new[]
            {
                "retained",
                "adapted",
                "inapplicable"
            }.Contains(value: Text(node: entry["disposition"]), comparer: StringComparer.Ordinal) || string
                    .IsNullOrWhiteSpace(value: Text(node: entry["reason"])))
            {
                errors.Add(item: id + ": DISPOSITION");
            }
        }
    }

    private static void ValidateLocalExemptions(Dictionary<string, JsonNode> local, bool implemented,
            List<string> errors)
    {
        var scopes = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach ((string? id, JsonNode? entry) in local)
        {
            foreach (string? field in new[]
            {
                "sourceId",
                "rule",
                "diagnostic",
                "path",
                "surface",
                "subject",
                "reason",
                "positiveProof",
                "negativeProof"
            }

            )
            {
                if (string.IsNullOrWhiteSpace(value: Text(node: entry[field])))
                {
                    errors.Add(item: id + ": EXEMPTION_FIELD: " + field);
                }
            }

            string path = Text(node: entry["path"]);
            if (!scopes.Add(item: Text(node: entry["diagnostic"]) + ":" + path))
            {
                errors.Add(item: id + ": EXEMPTION_OVERLAP");
            }

            if (!ExactPath(relative: path) || path.IndexOfAny(anyOf: ['*', '?', '{', '}']) >= 0)
            {
                errors.Add(item: id + ": EXEMPTION_SCOPE");
            }

            if (!IsObservedLock(entry: entry, id: id, path: path)
                    && !ExemptionValidator.IsObservedLocalPermission(entry: entry, id: id))
            {
                errors.Add(item: id + ": EXEMPTION_UNOBSERVED");
            }

            (string Positive, string Negative) proofs = ExemptionValidator.LocalProofs(id: id,
                    rule: Text(node: entry["rule"]));
            if (!StringComparer.Ordinal.Equals(x: Text(node: entry["positiveProof"]), y: proofs.Positive)
                    || !StringComparer.Ordinal.Equals(x: Text(node: entry["negativeProof"]), y: proofs.Negative))
            {
                errors.Add(item: id + ": EXEMPTION_PROOF");
            }

            if (!StringComparer.Ordinal.Equals(x: Text(node: entry["status"]), y: "pending"))
            {
                errors.Add(item: id + ": EXEMPTION_BOUNDARY_PENDING");
            }
        }
    }

    private static bool IsObservedLock(JsonNode entry, string id, string path)
    {
        return SourceFileFormat.LockExemptions.TryGetValue(key: id, value: out string? observedPath)
                && StringComparer.Ordinal.Equals(x: path, y: observedPath)
                && StringComparer.Ordinal.Equals(x: Text(node: entry["sourceId"]), y: "EX-11")
                && StringComparer.Ordinal.Equals(x: Text(node: entry["rule"]), y: "R-04")
                && StringComparer.Ordinal.Equals(x: Text(node: entry["diagnostic"]), y: "FORMAT_FINAL_NEWLINE")
                && StringComparer.Ordinal.Equals(x: Text(node: entry["surface"]), y: "NuGet-generated lock file")
                && StringComparer.Ordinal.Equals(x: Text(node: entry["subject"]), y: "no final newline");
    }

    private static void ValidateRuleDisposition(JsonNode rule, string id, HashSet<string> inapplicable,
            HashSet<string> adopted, List<string> errors)
    {
        string expectedDisposition = inapplicable.Contains(item: id) ? "inapplicable" : adopted.Contains(
                item: id) ? "adapted" : "retained";
        if (!StringComparer.Ordinal.Equals(x: Text(node: rule["disposition"]), y: expectedDisposition))
        {
            errors.Add(item: id + ": DISPOSITION_DRIFT");
        }

        if (string.IsNullOrWhiteSpace(value: Text(node: rule["scope"])))
        {
            errors.Add(item: id + ": SCOPE_REQUIRED");
        }

        if (StringComparer.Ordinal.Equals(x: expectedDisposition, y: "adapted") && string.IsNullOrWhiteSpace(
                value: Text(node: rule["adaptationReason"])))
        {
            errors.Add(item: id + ": ADAPTATION_REASON");
        }

        JsonNode? doc = rule["documentation"];
        if (doc is null || !StringComparer.Ordinal.Equals(x: Text(node: doc["path"]),
                y: "docs/code/register.json") || !StringComparer.Ordinal.Equals(x: Text(node: doc["symbol"]),
                        y: id) || !ExactPath(relative: Text(node: doc["path"])))
        {
            errors.Add(item: id + ": DOCUMENTATION_REFERENCE");
        }
    }
}
