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
    private const string DonorDocs = "src/backend/docs/";
    private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    private static readonly int[] CoverageLimits = [90, 80];
    private static readonly int[] BodyBoundaries = [59, 60, 61];
    private static readonly string[] BodyProofs = ["rules.body.59", "rules.body.60", "rules.body.61"];

    /// <summary>Reads a repository-relative JSON object with a four-megabyte size limit and strict parsing.</summary>
    public static JsonObject Read(string relative)
    {
        var file = Path.Combine(path1: ProofProtocol.Root, path2: relative);
        if (new FileInfo(fileName: file).Length > 4_000_000) throw new InvalidDataException(message: "JSON_SIZE_LIMIT");
        return Parse(bytes: File.ReadAllBytes(path: file));
    }

    /// <summary>Parses strict UTF-8 JSON, rejecting duplicate property names and non-object roots.</summary>
    public static JsonObject Parse(byte[] bytes)
    {
        var text = StrictUtf8.GetString(bytes: bytes);
        using var document = JsonDocument.Parse(json: text, options: new JsonDocumentOptions { MaxDepth = 64 });
        CheckProperties(element: document.RootElement);
        return JsonNode.Parse(json: text) as JsonObject ?? throw new InvalidDataException(message: "JSON_OBJECT_REQUIRED");
    }

    private static void CheckProperties(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
            {
                if (!names.Add(item: property.Name)) throw new InvalidDataException(message: "JSON_DUPLICATE: " + property.Name);
                CheckProperties(element: property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
            foreach (var item in element.EnumerateArray()) CheckProperties(element: item);
    }

    /// <summary>Returns capture and mapping diagnostics; requiring implementation also rejects pending mechanisms and reviews.</summary>
    public static IReadOnlyList<string> Validate(JsonObject manifest, JsonObject register, JsonObject exemptions, bool requireImplemented = false)
    {
        var errors = new List<string>();
        try
        {
            ValidateCapture(manifest: manifest, register: register, exemptions: exemptions, errors: errors);
            ValidateMappings(manifest: manifest, register: register, exemptions: exemptions, implemented: requireImplemented, errors: errors);
        }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or KeyNotFoundException or InvalidDataException or DecoderFallbackException)
        {
            errors.Add(item: "REGISTER: INVALID_STRUCTURE: " + error.Message);
        }
        return errors;
    }

    private static readonly Dictionary<string, string> Analyzers = new(comparer: StringComparer.Ordinal)
    {
        ["R-09"] = "ContinuationIndent", ["R-10"] = "LineLength", ["R-14"] = "UsingOrder",
        ["R-18"] = "LocalNewDeclaration", ["R-23"] = "PrimaryConstructor", ["R-24"] = "PrimaryConstructor",
        ["R-25"] = "RegionDirective", ["R-26"] = "NullForgiving", ["R-28"] = "ThisQualification",
        ["R-33"] = "MemberOrder", ["R-36"] = "BodyComment", ["R-37"] = "BodyLength", ["R-38"] = "NamedArgument",
    };

    private static string ExpectedMechanism(string id)
    {
        if (Analyzers.TryGetValue(key: id, value: out var name)) return "tools/code-rules/" + name + "Analyzer.cs";
        var number = int.Parse(s: id[2..], provider: System.Globalization.CultureInfo.InvariantCulture);
        return number switch
        {
            >= 68 and <= 78 => "src/core/SnapshotCodec.cs",
            49 or 82 => "tools/phase1/coverage-policy.json",
            >= 83 => "tests/phase1/DocumentationValidator.cs",
            >= 51 => "tests/phase1/ArchitectureRuleTests.cs",
            <= 8 => "tests/phase1/EnforcementTests.cs",
            _ => ".editorconfig",
        };
    }

    private static bool ExactPath(string relative)
    {
        if (string.IsNullOrWhiteSpace(value: relative) || relative.Contains(value: '\\') || relative.Contains(value: ':') || relative.StartsWith(value: '/')) return false;
        var current = ProofProtocol.Root;
        foreach (var part in relative.Split(separator: '/'))
        {
            if (part is "" or "." or ".." || !Directory.Exists(path: current)) return false;
            var matches = Directory.EnumerateFileSystemEntries(path: current).Where(predicate: p => Path.GetFileName(path: p).Equals(value: part, comparisonType: StringComparison.Ordinal)).ToArray();
            if (matches.Length != 1 || (File.GetAttributes(path: matches[0]) & FileAttributes.ReparsePoint) != FileAttributes.None) return false;
            current = matches[0];
        }
        return File.Exists(path: current) && new FileInfo(fileName: current).Length > 0;
    }

    private static void ValidateMappings(JsonObject manifest, JsonObject register, JsonObject exemptions, bool implemented, List<string> errors)
    {
        var cases = Read(relative: "tools/phase1/required-cases.json")["cases"]!.AsArray();
        var declarations = Unique(rows: cases, key: n => Text(node: n["id"]), diagnostic: "PROOF_DECLARATION", errors: errors);
        var sources = Unique(rows: Rows(node: manifest, name: "matrix"), key: n => Text(node: n["sourceId"]), diagnostic: "MATRIX_ID", errors: errors);
        var reviewRules = new HashSet<string>(collection: ["R-24", "R-34", "R-62", "R-63", "R-65", "R-77", "R-83", "R-84"], comparer: StringComparer.Ordinal);
        var inapplicable = new HashSet<string>(collection: ["R-43", "R-57", "R-76", "R-79", "R-80", "R-85"], comparer: StringComparer.Ordinal);
        var adopted = new HashSet<string>(collection: ["R-02", "R-03", "R-04", "R-05", "R-06", "R-24", "R-34", "R-40", "R-44", "R-45", "R-49", "R-50", "R-51", "R-52", "R-54", "R-55", "R-56", "R-58", "R-59", "R-61", "R-63", "R-64", "R-68", "R-69", "R-70", "R-71", "R-72", "R-73", "R-78", "R-81", "R-82"], comparer: StringComparer.Ordinal);
        foreach (var rule in Rows(node: register, name: "rules"))
        {
            if (rule is null) continue;
            var id = Text(node: rule["sourceId"]);
            if (!sources.TryGetValue(key: id, value: out var source)) continue;
            var expectedDisposition = inapplicable.Contains(item: id) ? "inapplicable" : adopted.Contains(item: id) ? "adapted" : "retained";
            if (!string.Equals(a: Text(node: rule["disposition"]), b: expectedDisposition, comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": DISPOSITION_DRIFT");
            if (string.IsNullOrWhiteSpace(value: Text(node: rule["scope"]))) errors.Add(item: id + ": SCOPE_REQUIRED");
            if (string.Equals(a: expectedDisposition, b: "adapted", comparisonType: StringComparison.Ordinal) && string.IsNullOrWhiteSpace(value: Text(node: rule["adaptationReason"]))) errors.Add(item: id + ": ADAPTATION_REASON");
            var doc = rule["documentation"];
            if (doc is null || !string.Equals(a: Text(node: doc["path"]), b: "docs/code/register.json", comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: doc["symbol"]), b: id, comparisonType: StringComparison.Ordinal) || !ExactPath(relative: Text(node: doc["path"]))) errors.Add(item: id + ": DOCUMENTATION_REFERENCE");
            ValidateReview(rule: rule, id: id, required: reviewRules.Contains(item: id) || Text(node: source["level"]).Contains(value: "PR review", comparisonType: StringComparison.Ordinal), implemented: implemented, errors: errors);
            if (inapplicable.Contains(item: id))
            {
                if (rule["mechanism"] is not null || rule["positiveProof"] is not null || rule["negativeProof"] is not null) errors.Add(item: id + ": INAPPLICABLE_MAPPING");
                if (!declarations.TryGetValue(key: id + ".register-resolution", value: out var resolution) || !string.Equals(a: Text(node: resolution["rule"]), b: id, comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": RESOLUTION_REQUIRED");
                continue;
            }
            ValidateMechanism(rule: rule, source: source, id: id, implemented: implemented, errors: errors);
            ValidateProofs(rule: rule, id: id, declarations: declarations, implemented: implemented, errors: errors);
            ValidateNumeric(id: id, numeric: rule["numeric"], errors: errors);
        }
        ValidateExemptions(manifest: manifest, register: register, exemptions: exemptions, implemented: implemented, errors: errors);
    }

    private static void ValidateReview(JsonNode rule, string id, bool required, bool implemented, List<string> errors)
    {
        var qualitative = rule["qualitativeReview"];
        if (qualitative is null || qualitative["required"]?.GetValue<bool>() != required) errors.Add(item: id + ": REVIEW_REQUIRED");
        else if (required && (!string.Equals(a: Text(node: qualitative["status"]), b: "pending", comparisonType: StringComparison.Ordinal) || string.IsNullOrWhiteSpace(value: Text(node: qualitative["obligation"])))) errors.Add(item: id + ": HUMAN_REVIEW_PENDING");
        else if (!required && !string.Equals(a: Text(node: qualitative["status"]), b: "not-required", comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": REVIEW_STATUS");
        if (implemented && required) errors.Add(item: id + ": HUMAN_REVIEW_PENDING");
    }

    private static void ValidateMechanism(JsonNode rule, JsonNode source, string id, bool implemented, List<string> errors)
    {
            var mechanism = rule["mechanism"];
            if (mechanism is null) errors.Add(item: id + ": MECHANISM_REQUIRED");
            else
            {
                var expectedSymbol = Analyzers.TryGetValue(key: id, value: out var analyzerName) ? analyzerName + "Analyzer" : id;
                if (!string.Equals(a: Text(node: mechanism["path"]), b: ExpectedMechanism(id: id), comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: mechanism["symbol"]), b: expectedSymbol, comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": MECHANISM_REFERENCE");
                var diagnostics = Regex.Matches(input: Text(node: source["mechanism"]), pattern: @"\b(?:SCR|IDE|CA|MA|RCS|RS|CS)\d{4}\b", options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking, matchTimeout: TimeSpan.FromSeconds(seconds: 1)).Select(selector: m => m.Value).Distinct(comparer: StringComparer.Ordinal);
                SameStrings(expected: diagnostics, actual: mechanism["diagnostics"]!.AsArray(), diagnostic: id + ": DIAGNOSTIC_IDENTITY", errors: errors);
                if (!string.Equals(a: Text(node: mechanism["severity"]), b: "error", comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": SEVERITY_WEAKENED");
                if (!string.Equals(a: Text(node: mechanism["status"]), b: "pending", comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": UNVERIFIED_IMPLEMENTATION");
                if (implemented) errors.Add(item: id + ": IMPLEMENTATION_PENDING");
            }
    }

    private static void ValidateProofs(JsonNode rule, string id, Dictionary<string, JsonNode> declarations, bool implemented, List<string> errors)
    {
            foreach (var polarity in new[] { "positive", "negative" })
            {
                var proof = rule[polarity + "Proof"];
                if (proof is null) { errors.Add(item: id + ": " + polarity.ToUpperInvariant() + "_PROOF_REQUIRED"); continue; }
                var proofId = Text(node: proof["id"]);
                if (!string.Equals(a: proofId, b: id + "." + polarity, comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: proof["rule"]), b: id, comparisonType: StringComparison.Ordinal) || !declarations.TryGetValue(key: proofId, value: out var declaration) || !string.Equals(a: Text(node: declaration["rule"]), b: id, comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: declaration["polarity"]), b: polarity, comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": PROOF_REVERSE_ID");
                if (!string.Equals(a: Text(node: proof["status"]), b: "pending", comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": UNVERIFIED_PROOF");
                var number = int.Parse(s: id[2..], provider: System.Globalization.CultureInfo.InvariantCulture);
                var expectedProof = Analyzers.TryGetValue(key: id, value: out var analyzer) ? "tests/phase1/Analyzers/" + analyzer + "AnalyzerTests.cs" : number switch
                {
                    >= 68 and <= 78 => "tests/phase1/SnapshotTests.cs",
                    49 or 82 => "tests/phase1/CoverageTests.cs",
                    >= 83 => "tests/phase1/DocumentationTests.cs",
                    >= 51 => "tests/phase1/ArchitectureRuleTests.cs",
                    _ => "tests/phase1/EnforcementTests.cs",
                };
                if (!string.Equals(a: Text(node: proof["path"]), b: expectedProof, comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": PROOF_PATH");
                if (implemented && (!declarations.TryGetValue(key: proofId, value: out var actual) || !string.Equals(a: Text(node: actual["implementation"]), b: "csharp", comparisonType: StringComparison.Ordinal) || !ExactPath(relative: Text(node: proof["path"])))) errors.Add(item: id + ": PROOF_UNIMPLEMENTED");
            }
    }

    private static void ValidateNumeric(string id, JsonNode? numeric, List<string> errors)
    {
        var limits = new Dictionary<string, int>(comparer: StringComparer.Ordinal) { ["R-07"] = 4, ["R-08"] = 4, ["R-09"] = 8, ["R-10"] = 120, ["R-37"] = 60, ["R-74"] = 0 };
        if (!limits.ContainsKey(key: id) && id is not "R-49" and not "R-82") return;
        if (numeric is null || string.IsNullOrWhiteSpace(value: Text(node: numeric["unit"])) || string.IsNullOrWhiteSpace(value: Text(node: numeric["comparison"])) || string.IsNullOrWhiteSpace(value: Text(node: numeric["rounding"]))) { errors.Add(item: id + ": NUMERIC_SEMANTICS"); return; }
        if (limits.TryGetValue(key: id, value: out var limit) && numeric["limit"]?.GetValue<int>() != limit) errors.Add(item: id + ": NUMERIC_LIMIT");
        if (id is "R-49" or "R-82" && !numeric["limits"]!.AsArray().Select(selector: n => n!.GetValue<int>()).SequenceEqual(second: CoverageLimits)) errors.Add(item: id + ": NUMERIC_LIMIT");
        if (string.Equals(a: id, b: "R-37", comparisonType: StringComparison.Ordinal) && !numeric["boundaries"]!.AsArray().Select(selector: n => n!.GetValue<int>()).SequenceEqual(second: BodyBoundaries)) errors.Add(item: id + ": NUMERIC_BOUNDARIES");
        var units = new Dictionary<string, string>(comparer: StringComparer.Ordinal)
        {
            ["R-07"] = "spaces", ["R-08"] = "spaces", ["R-09"] = "spaces past continued line",
            ["R-10"] = "UTF-16 code units excluding line terminator",
            ["R-37"] = "lines between body braces including blank lines; local functions also count in parent",
            ["R-49"] = "covered/total lines and branches", ["R-82"] = "covered/total lines and branches",
            ["R-74"] = "enum numeric sentinel",
        };
        var comparison = id switch { "R-07" or "R-08" or "R-09" => "exact", "R-49" or "R-82" => "inclusive minimum percent via integer cross multiplication", "R-74" => "Unknown = 0 is never valid saved state", _ => "inclusive maximum" };
        var rounding = string.Equals(a: id, b: "R-37", comparisonType: StringComparison.Ordinal) ? "none; expression/global bodies follow MemberBodies; 40 advised only" : "none";
        if (!string.Equals(a: Text(node: numeric["unit"]), b: units[id], comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: numeric["comparison"]), b: comparison, comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: numeric["rounding"]), b: rounding, comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": NUMERIC_SEMANTICS");
        if (string.Equals(a: id, b: "R-37", comparisonType: StringComparison.Ordinal))
        {
            var proofs = numeric["boundaryProofs"]?.AsArray();
            if (proofs is null) { errors.Add(item: id + ": NUMERIC_BOUNDARY_PROOFS"); return; }
            var unique = Unique(rows: proofs, key: n => Text(node: n["id"]), diagnostic: "BOUNDARY_PROOF", errors: errors);
            if (!unique.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: BodyProofs)) errors.Add(item: id + ": NUMERIC_BOUNDARY_PROOFS");
            foreach (var row in proofs)
                if (!string.Equals(a: Text(node: row?["rule"]), b: id, comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: row?["path"]), b: "tests/phase1/Analyzers/BodyLengthAnalyzerTests.cs", comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: row?["status"]), b: "pending", comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": NUMERIC_BOUNDARY_PROOFS");
        }
    }

    private static void ValidateExemptions(JsonObject manifest, JsonObject register, JsonObject exemptions, bool implemented, List<string> errors)
    {
        var sources = Unique(rows: Rows(node: manifest, name: "exemptions"), key: n => Text(node: n["id"]), diagnostic: "SOURCE_EXEMPTION", errors: errors);
        var local = Unique(rows: Rows(node: exemptions, name: "local"), key: n => Text(node: n["id"]), diagnostic: "LOCAL_EXEMPTION_ID", errors: errors);
        foreach (var entry in Rows(node: exemptions, name: "donorExemptions"))
        {
            if (entry is null) continue;
            var id = Text(node: entry["sourceId"]);
            if (!sources.TryGetValue(key: id, value: out var source) || !string.Equals(a: Text(node: entry["rule"]), b: Text(node: source["sourceId"]), comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": EXEMPTION_REVERSE_ID");
            var expectedDisposition = new[] { "EX-01", "EX-02", "EX-03", "EX-11", "EX-13", "EX-19", "EX-20", "EX-21", "EX-23" }.Contains(value: id, comparer: StringComparer.Ordinal) ? "adapted" : "inapplicable";
            if (!string.Equals(a: Text(node: entry["disposition"]), b: expectedDisposition, comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": EXEMPTION_DISPOSITION");
            SameStrings(expected: local.Where(predicate: pair => string.Equals(a: Text(node: pair.Value["sourceId"]), b: id, comparisonType: StringComparison.Ordinal)).Select(selector: pair => pair.Key), actual: entry["localIds"]!.AsArray(), diagnostic: id + ": LOCAL_EXEMPTION_REFERENCE", errors: errors);
        }
        var scopes = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (var (id, entry) in local)
        {
            foreach (var field in new[] { "sourceId", "rule", "diagnostic", "path", "surface", "subject", "reason", "positiveProof", "negativeProof" })
                if (string.IsNullOrWhiteSpace(value: Text(node: entry[field]))) errors.Add(item: id + ": EXEMPTION_FIELD: " + field);
            var path = Text(node: entry["path"]);
            if (!scopes.Add(item: Text(node: entry["diagnostic"]) + ":" + path)) errors.Add(item: id + ": EXEMPTION_OVERLAP");
            if (!ExactPath(relative: path) || path.IndexOfAny(anyOf: ['*', '?', '{', '}']) >= 0) errors.Add(item: id + ": EXEMPTION_SCOPE");
            if (!string.Equals(a: id, b: "KZ-EX-LOCK-01", comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: entry["sourceId"]), b: "EX-11", comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: entry["rule"]), b: "R-04", comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: entry["diagnostic"]), b: "FORMAT_FINAL_NEWLINE", comparisonType: StringComparison.Ordinal) || !string.Equals(a: path, b: "tests/phase1/packages.lock.json", comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: entry["subject"]), b: "no final newline", comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": EXEMPTION_UNOBSERVED");
            if (!string.Equals(a: Text(node: entry["positiveProof"]), b: Text(node: entry["rule"]) + ".positive", comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: entry["negativeProof"]), b: Text(node: entry["rule"]) + ".negative", comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": EXEMPTION_PROOF");
            if (!string.Equals(a: Text(node: entry["status"]), b: "pending", comparisonType: StringComparison.Ordinal) || implemented) errors.Add(item: id + ": EXEMPTION_BOUNDARY_PENDING");
        }
        foreach (var rule in Rows(node: register, name: "rules"))
            SameStrings(expected: local.Where(predicate: pair => string.Equals(a: Text(node: pair.Value["rule"]), b: Text(node: rule!["sourceId"]), comparisonType: StringComparison.Ordinal)).Select(selector: pair => pair.Key), actual: rule!["exemptions"]!.AsArray(), diagnostic: Text(node: rule["sourceId"]) + ": EXEMPTION_REFERENCE", errors: errors);
    }

    private static string Text(JsonNode? node) => node?.GetValue<string>() ?? "";
    private static JsonArray Rows(JsonObject node, string name) => node[name] as JsonArray ?? throw new InvalidDataException(message: "ARRAY_REQUIRED: " + name);
    private static string Hash(byte[] bytes) => Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes));
    private static string HeadingKey(JsonNode node) => Text(node: node["path"]) + ":" + node["line"]!.GetValue<int>();

    private static Dictionary<string, JsonNode> Unique(JsonArray rows, Func<JsonNode, string> key, string diagnostic, List<string> errors)
    {
        var result = new Dictionary<string, JsonNode>(comparer: StringComparer.Ordinal);
        foreach (var row in rows)
        {
            if (row is null) { errors.Add(item: diagnostic + ": NULL_ROW"); continue; }
            var id = key(row);
            if (string.IsNullOrWhiteSpace(value: id) || !result.TryAdd(key: id, value: row)) errors.Add(item: id + ": " + diagnostic + ": DUPLICATE_OR_EMPTY");
        }
        return result;
    }

    private static void SameStrings(IEnumerable<string> expected, JsonArray? actual, string diagnostic, List<string> errors)
    {
        if (actual is null) { errors.Add(item: diagnostic); return; }
        var values = actual.Select(selector: Text).ToArray();
        if (values.Distinct(comparer: StringComparer.Ordinal).Count() != values.Length || !expected.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: values)) errors.Add(item: diagnostic);
    }

    private static Dictionary<string, byte[]> Archive(List<string> errors)
    {
        var file = Path.Combine(path1: ProofProtocol.Root, path2: "docs/code/provenance/langproj-baseline.zip");
        if (new FileInfo(fileName: file).Length > 20_000_000) throw new InvalidDataException(message: "Archive size limit");
        var bytes = File.ReadAllBytes(path: file);
        if (!string.Equals(a: Hash(bytes: bytes), b: ArchiveHash, comparisonType: StringComparison.Ordinal)) throw new InvalidDataException(message: "CAPTURE: ARCHIVE_HASH");
        using var stream = new MemoryStream(buffer: bytes);
        using var zip = new ZipArchive(stream: stream, mode: ZipArchiveMode.Read);
        var files = new Dictionary<string, byte[]>(comparer: StringComparer.Ordinal);
        long total = 0;
        foreach (var entry in zip.Entries)
        {
            if (entry.FullName.EndsWith(value: '/')) continue;
            if (entry.Length > 4_000_000 || (total += entry.Length) > 20_000_000) throw new InvalidDataException(message: "Expanded size limit");
            if (entry.FullName.StartsWith(value: '/') || entry.FullName.Contains(value: '\\') || entry.FullName.Split(separator: '/').Any(predicate: x => x is ".." or "." or "")) throw new InvalidDataException(message: "Noncanonical ZIP path");
            using var input = entry.Open();
            using var output = new MemoryStream();
            input.CopyTo(destination: output);
            if (!files.TryAdd(key: entry.FullName, value: output.ToArray())) errors.Add(item: "CAPTURE: DUPLICATE_ZIP_PATH: " + entry.FullName);
        }
        return files;
    }

    private static void ValidateCapture(JsonObject manifest, JsonObject register, JsonObject exemptions, List<string> errors)
    {
        if (!string.Equals(a: Text(node: manifest["revision"]), b: Revision, comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: register["revision"]), b: Revision, comparisonType: StringComparison.Ordinal) || manifest["cleanAtCapture"]?.GetValue<bool>() != true) errors.Add(item: "CAPTURE: REVISION");
        if (!string.Equals(a: Text(node: manifest["archiveSha256"]), b: ArchiveHash, comparisonType: StringComparison.Ordinal) || !string.Equals(a: Text(node: register["archiveSha256"]), b: ArchiveHash, comparisonType: StringComparison.Ordinal)) errors.Add(item: "CAPTURE: ARCHIVE_IDENTITY");
        var files = Archive(errors: errors);
        var capturedFiles = Unique(rows: Rows(node: manifest, name: "files"), key: n => Text(node: n["path"]), diagnostic: "CAPTURE_FILE", errors: errors);
        if (!files.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: capturedFiles.Keys)) errors.Add(item: "CAPTURE: FILE_INVENTORY");
        foreach (var (path, row) in capturedFiles)
            if (!files.TryGetValue(key: path, value: out var bytes) || !string.Equals(a: Hash(bytes: bytes), b: Text(node: row["sha256"]), comparisonType: StringComparison.Ordinal) || bytes.Length != row["size"]!.GetValue<long>() || !string.Equals(a: Text(node: row["modification"]), b: "unmodified", comparisonType: StringComparison.Ordinal)) errors.Add(item: "CAPTURE: FILE_HASH: " + path);
        var expectedHeadings = CaptureHeadings(files: files);
        ValidateHeadings(manifest: manifest, expectedHeadings: expectedHeadings, errors: errors);
        ValidateSourceRows(manifest: manifest, register: register, exemptions: exemptions, files: files, expectedHeadings: expectedHeadings, errors: errors);
        if (Text(node: manifest["licenseNotice"]?["finding"]).Length == 0 || Text(node: manifest["licenseNotice"]?["modifications"]).Length == 0) errors.Add(item: "CAPTURE: LICENSE_NOTICE");
    }

    private static List<JsonNode> CaptureHeadings(Dictionary<string, byte[]> files)
    {
        var expectedHeadings = new List<JsonNode>();
        foreach (var name in new[] { "code-style.md", "code-architecture.md" })
        {
            var path = DonorDocs + "CodeRules/" + name;
            var lines = StrictUtf8.GetString(bytes: files[path]).Split(separator: '\n');
            for (var i = 0; i < lines.Length; i++)
            {
                if (!lines[i].StartsWith(value: "### ", comparisonType: StringComparison.Ordinal)) continue;
                var end = i + 1;
                while (end < lines.Length && !lines[end].StartsWith(value: "### ", comparisonType: StringComparison.Ordinal)) end++;
                var enforcement = lines[(i + 1)..end].SingleOrDefault(predicate: x => x.StartsWith(value: "Enforcement:", comparisonType: StringComparison.Ordinal));
                if (enforcement is null) continue;
                var ids = Regex.Matches(input: enforcement, pattern: @"\[(?<id>R-\d+)\]", options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking, matchTimeout: TimeSpan.FromSeconds(seconds: 1)).Select(selector: m => m.Groups["id"].Value).ToArray();
                expectedHeadings.Add(item: new JsonObject { ["path"] = path, ["line"] = i + 1, ["title"] = lines[i][4..].Trim(), ["sourceIds"] = new JsonArray(items: ids.Select(selector: x => (JsonNode?)JsonValue.Create(value: x)).ToArray()) });
            }
        }
        return expectedHeadings;
    }

    private static void ValidateHeadings(JsonObject manifest, List<JsonNode> expectedHeadings, List<string> errors)
    {
        if (expectedHeadings.Count != 84 || expectedHeadings.Count(predicate: n => Text(node: n["path"]).EndsWith(value: "/code-style.md", comparisonType: StringComparison.Ordinal)) != 49) errors.Add(item: "CAPTURE: HEADING_COUNT");
        var headings = Unique(rows: Rows(node: manifest, name: "headings"), key: HeadingKey, diagnostic: "HEADING", errors: errors);
        if (!headings.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: expectedHeadings.Select(selector: HeadingKey))) errors.Add(item: "CAPTURE: HEADING_INVENTORY");
        foreach (var expected in expectedHeadings)
        {
            if (!headings.TryGetValue(key: HeadingKey(node: expected), value: out var actual)) continue;
            if (!string.Equals(a: Text(node: expected["title"]), b: Text(node: actual["title"]), comparisonType: StringComparison.Ordinal)) errors.Add(item: HeadingKey(node: expected) + ": HEADING_TITLE");
            SameStrings(expected: expected["sourceIds"]!.AsArray().Select(selector: Text), actual: actual["sourceIds"]!.AsArray(), diagnostic: HeadingKey(node: expected) + ": HEADING_IDS", errors: errors);
        }
    }

    private static void ValidateSourceRows(JsonObject manifest, JsonObject register, JsonObject exemptions, Dictionary<string, byte[]> files, List<JsonNode> expectedHeadings, List<string> errors)
    {
        var matrix = Unique(rows: Rows(node: manifest, name: "matrix"), key: n => Text(node: n["sourceId"]), diagnostic: "MATRIX_ID", errors: errors);
        var sourceExemptions = Unique(rows: Rows(node: manifest, name: "exemptions"), key: n => Text(node: n["id"]), diagnostic: "SOURCE_EXEMPTION", errors: errors);
        var expectedIds = new List<string>();
        var expectedExemptions = new List<string>();
        var matrixLines = StrictUtf8.GetString(bytes: files[DonorDocs + "rule-matrix.md"]).Split(separator: '\n');
        for (var i = 0; i < matrixLines.Length; i++)
        {
            var match = Regex.Match(input: matrixLines[i], pattern: "^\\| <a id=\"(?:r|ex)-[0-9]+\"></a>(?<id>(?:R|EX)-[0-9]+) \\|", options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking, matchTimeout: TimeSpan.FromSeconds(seconds: 1));
            if (!match.Success) continue;
            var id = match.Groups["id"].Value;
            var isRule = id.StartsWith(value: "R-", comparisonType: StringComparison.Ordinal);
            (isRule ? expectedIds : expectedExemptions).Add(item: id);
            var rows = isRule ? matrix : sourceExemptions;
            if (!rows.TryGetValue(key: id, value: out var row) || !string.Equals(a: Text(node: row["raw"]), b: matrixLines[i], comparisonType: StringComparison.Ordinal) || row["line"]!.GetValue<int>() != i + 1) errors.Add(item: id + ": SOURCE_ROW");
            if (row is not null) ValidateSourceFields(row: row, line: matrixLines[i], id: id, isRule: isRule, errors: errors);
        }
        if (expectedIds.Count != 85 || !matrix.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: expectedIds)) errors.Add(item: "CAPTURE: MATRIX_INVENTORY");
        if (expectedExemptions.Count != 24 || expectedExemptions.Contains(item: "EX-22") || !sourceExemptions.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: expectedExemptions)) errors.Add(item: "CAPTURE: EXEMPTION_INVENTORY");
        var rules = Unique(rows: Rows(node: register, name: "rules"), key: n => Text(node: n["sourceId"]), diagnostic: "RULE_ID", errors: errors);
        if (!rules.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: expectedIds)) errors.Add(item: "REGISTER: RULE_INVENTORY");
        foreach (var (id, rule) in rules)
        {
            if (!matrix.ContainsKey(key: id)) { errors.Add(item: id + ": UNKNOWN_RULE"); continue; }
            SameStrings(expected: expectedHeadings.Where(predicate: h => h["sourceIds"]!.AsArray().Select(selector: Text).Contains(value: id, comparer: StringComparer.Ordinal)).Select(selector: HeadingKey), actual: rule["headingRefs"] as JsonArray, diagnostic: id + ": HEADING_RELATION", errors: errors);
            if (!new[] { "retained", "adapted", "inapplicable" }.Contains(value: Text(node: rule["disposition"]), comparer: StringComparer.Ordinal) || string.IsNullOrWhiteSpace(value: Text(node: rule["reason"]))) errors.Add(item: id + ": DISPOSITION");
        }
        var dispositions = Unique(rows: Rows(node: exemptions, name: "donorExemptions"), key: n => Text(node: n["sourceId"]), diagnostic: "EXEMPTION_ID", errors: errors);
        if (!dispositions.Keys.ToHashSet(comparer: StringComparer.Ordinal).SetEquals(other: expectedExemptions)) errors.Add(item: "REGISTER: DONOR_EXEMPTION_INVENTORY");
        foreach (var (id, entry) in dispositions)
            if (!new[] { "retained", "adapted", "inapplicable" }.Contains(value: Text(node: entry["disposition"]), comparer: StringComparer.Ordinal) || string.IsNullOrWhiteSpace(value: Text(node: entry["reason"]))) errors.Add(item: id + ": DISPOSITION");
    }

    private static void ValidateSourceFields(JsonNode row, string line, string id, bool isRule, List<string> errors)
    {
        var columns = line.Split(separator: '|')[1..^1].Select(selector: x => x.Trim()).ToArray();
        var names = isRule ? new[] { "title", "mechanism", "scope", "proofs", "level" } : new[] { "path", "subject", "reason" };
        for (var column = 0; column < names.Length; column++)
            if (!string.Equals(a: Text(node: row[names[column]]), b: columns[column + 1], comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": SOURCE_FIELD: " + names[column]);
        if (!isRule && !string.Equals(a: Text(node: row["sourceId"]), b: Regex.Match(input: columns[4], pattern: "R-[0-9]+", options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking, matchTimeout: TimeSpan.FromSeconds(seconds: 1)).Value, comparisonType: StringComparison.Ordinal)) errors.Add(item: id + ": SOURCE_RULE_ID");
    }
}
