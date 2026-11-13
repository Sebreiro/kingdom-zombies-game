using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace KingdomZombies.Phase1.Tests;

public static class RegisterValidator
{
    public const string Revision = "6836a9718ff5c939e89abf2b97763760607de192";
    public const string ArchiveHash = "19c3766fddf638b02a2b78d969e222448ecd98a22b79f70e8b28effd858dfcca";
    private const string DonorDocs = "src/backend/docs/";
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    public static JsonObject Read(string relative)
    {
        var file = Path.Combine(ProofProtocol.Root, relative);
        if (new FileInfo(file).Length > 4_000_000) throw new InvalidDataException("JSON_SIZE_LIMIT");
        return Parse(File.ReadAllBytes(file));
    }

    public static JsonObject Parse(byte[] bytes)
    {
        var text = StrictUtf8.GetString(bytes);
        using var document = JsonDocument.Parse(text, new JsonDocumentOptions { MaxDepth = 64 });
        CheckProperties(document.RootElement);
        return JsonNode.Parse(text) as JsonObject ?? throw new InvalidDataException("JSON_OBJECT_REQUIRED");
    }

    private static void CheckProperties(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
            {
                if (!names.Add(property.Name)) throw new InvalidDataException("JSON_DUPLICATE: " + property.Name);
                CheckProperties(property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
            foreach (var item in element.EnumerateArray()) CheckProperties(item);
    }

    public static IReadOnlyList<string> Validate(JsonObject manifest, JsonObject register, JsonObject exemptions, bool requireImplemented = false)
    {
        var errors = new List<string>();
        try
        {
            ValidateCapture(manifest, register, exemptions, errors);
            ValidateMappings(manifest, register, exemptions, requireImplemented, errors);
        }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or KeyNotFoundException or InvalidDataException or DecoderFallbackException)
        {
            errors.Add("REGISTER: INVALID_STRUCTURE: " + error.Message);
        }
        return errors;
    }

    private static readonly Dictionary<string, string> Analyzers = new(StringComparer.Ordinal)
    {
        ["R-09"] = "ContinuationIndent", ["R-10"] = "LineLength", ["R-14"] = "UsingOrder",
        ["R-18"] = "LocalNewDeclaration", ["R-23"] = "PrimaryConstructor", ["R-24"] = "PrimaryConstructor",
        ["R-25"] = "RegionDirective", ["R-26"] = "NullForgiving", ["R-28"] = "ThisQualification",
        ["R-33"] = "MemberOrder", ["R-36"] = "BodyComment", ["R-37"] = "BodyLength", ["R-38"] = "NamedArgument",
    };

    private static string ExpectedMechanism(string id)
    {
        if (Analyzers.TryGetValue(id, out var name)) return "tools/code-rules/" + name + "Analyzer.cs";
        var number = int.Parse(id[2..], System.Globalization.CultureInfo.InvariantCulture);
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
        if (string.IsNullOrWhiteSpace(relative) || relative.Contains('\\') || relative.Contains(':') || relative.StartsWith('/')) return false;
        var current = ProofProtocol.Root;
        foreach (var part in relative.Split('/'))
        {
            if (part is "" or "." or ".." || !Directory.Exists(current)) return false;
            var matches = Directory.EnumerateFileSystemEntries(current).Where(p => Path.GetFileName(p).Equals(part, StringComparison.Ordinal)).ToArray();
            if (matches.Length != 1 || (File.GetAttributes(matches[0]) & FileAttributes.ReparsePoint) != 0) return false;
            current = matches[0];
        }
        return File.Exists(current) && new FileInfo(current).Length > 0;
    }

    private static void ValidateMappings(JsonObject manifest, JsonObject register, JsonObject exemptions, bool implemented, List<string> errors)
    {
        var cases = Read("tools/phase1/required-cases.json")["cases"]!.AsArray();
        var declarations = Unique(cases, n => Text(n["id"]), "PROOF_DECLARATION", errors);
        var sources = Unique(Rows(manifest, "matrix"), n => Text(n["sourceId"]), "MATRIX_ID", errors);
        var reviewRules = new HashSet<string>(["R-24", "R-34", "R-62", "R-63", "R-65", "R-77", "R-83", "R-84"], StringComparer.Ordinal);
        var inapplicable = new HashSet<string>(["R-43", "R-57", "R-76", "R-79", "R-80", "R-85"], StringComparer.Ordinal);
        var adopted = new HashSet<string>(["R-02", "R-03", "R-04", "R-05", "R-06", "R-24", "R-34", "R-40", "R-44", "R-45", "R-49", "R-50", "R-51", "R-52", "R-54", "R-55", "R-56", "R-58", "R-59", "R-61", "R-63", "R-64", "R-68", "R-69", "R-70", "R-71", "R-72", "R-73", "R-78", "R-81", "R-82"], StringComparer.Ordinal);
        foreach (var rule in Rows(register, "rules"))
        {
            if (rule is null) continue;
            var id = Text(rule["sourceId"]);
            if (!sources.TryGetValue(id, out var source)) continue;
            var expectedDisposition = inapplicable.Contains(id) ? "inapplicable" : adopted.Contains(id) ? "adapted" : "retained";
            if (Text(rule["disposition"]) != expectedDisposition) errors.Add(id + ": DISPOSITION_DRIFT");
            if (string.IsNullOrWhiteSpace(Text(rule["scope"]))) errors.Add(id + ": SCOPE_REQUIRED");
            if (expectedDisposition == "adapted" && string.IsNullOrWhiteSpace(Text(rule["adaptationReason"]))) errors.Add(id + ": ADAPTATION_REASON");
            var doc = rule["documentation"];
            if (doc is null || Text(doc["path"]) != "docs/code/register.json" || Text(doc["symbol"]) != id || !ExactPath(Text(doc["path"]))) errors.Add(id + ": DOCUMENTATION_REFERENCE");
            var reviewRequired = reviewRules.Contains(id) || Text(source["level"]).Contains("PR review", StringComparison.Ordinal);
            var qualitative = rule["qualitativeReview"];
            if (qualitative is null || qualitative["required"]?.GetValue<bool>() != reviewRequired) errors.Add(id + ": REVIEW_REQUIRED");
            else if (reviewRequired && (Text(qualitative["status"]) != "pending" || string.IsNullOrWhiteSpace(Text(qualitative["obligation"])))) errors.Add(id + ": HUMAN_REVIEW_PENDING");
            else if (!reviewRequired && Text(qualitative["status"]) != "not-required") errors.Add(id + ": REVIEW_STATUS");
            if (implemented && reviewRequired) errors.Add(id + ": HUMAN_REVIEW_PENDING");
            if (inapplicable.Contains(id))
            {
                if (rule["mechanism"] is not null || rule["positiveProof"] is not null || rule["negativeProof"] is not null) errors.Add(id + ": INAPPLICABLE_MAPPING");
                if (!declarations.TryGetValue(id + ".register-resolution", out var resolution) || Text(resolution["rule"]) != id) errors.Add(id + ": RESOLUTION_REQUIRED");
                continue;
            }
            var mechanism = rule["mechanism"];
            if (mechanism is null) errors.Add(id + ": MECHANISM_REQUIRED");
            else
            {
                var expectedSymbol = Analyzers.TryGetValue(id, out var analyzerName) ? analyzerName + "Analyzer" : id;
                if (Text(mechanism["path"]) != ExpectedMechanism(id) || Text(mechanism["symbol"]) != expectedSymbol) errors.Add(id + ": MECHANISM_REFERENCE");
                var diagnostics = Regex.Matches(Text(source["mechanism"]), @"\b(?:SCR|IDE|CA|MA|RCS|RS|CS)\d{4}\b").Select(m => m.Value).Distinct(StringComparer.Ordinal);
                SameStrings(diagnostics, mechanism["diagnostics"]!.AsArray(), id + ": DIAGNOSTIC_IDENTITY", errors);
                if (Text(mechanism["severity"]) != "error") errors.Add(id + ": SEVERITY_WEAKENED");
                if (Text(mechanism["status"]) != "pending") errors.Add(id + ": UNVERIFIED_IMPLEMENTATION");
                if (implemented) errors.Add(id + ": IMPLEMENTATION_PENDING");
            }
            foreach (var polarity in new[] { "positive", "negative" })
            {
                var proof = rule[polarity + "Proof"];
                if (proof is null) { errors.Add(id + ": " + polarity.ToUpperInvariant() + "_PROOF_REQUIRED"); continue; }
                var proofId = Text(proof["id"]);
                if (proofId != id + "." + polarity || Text(proof["rule"]) != id || !declarations.TryGetValue(proofId, out var declaration) || Text(declaration["rule"]) != id || Text(declaration["polarity"]) != polarity) errors.Add(id + ": PROOF_REVERSE_ID");
                if (Text(proof["status"]) != "pending") errors.Add(id + ": UNVERIFIED_PROOF");
                var number = int.Parse(id[2..], System.Globalization.CultureInfo.InvariantCulture);
                var expectedProof = Analyzers.TryGetValue(id, out var analyzer) ? "tests/phase1/Analyzers/" + analyzer + "AnalyzerTests.cs" : number switch
                {
                    >= 68 and <= 78 => "tests/phase1/SnapshotTests.cs",
                    49 or 82 => "tests/phase1/CoverageTests.cs",
                    >= 83 => "tests/phase1/DocumentationTests.cs",
                    >= 51 => "tests/phase1/ArchitectureRuleTests.cs",
                    _ => "tests/phase1/EnforcementTests.cs",
                };
                if (Text(proof["path"]) != expectedProof) errors.Add(id + ": PROOF_PATH");
                if (implemented && (!declarations.TryGetValue(proofId, out var actual) || Text(actual["implementation"]) != "csharp" || !ExactPath(Text(proof["path"])))) errors.Add(id + ": PROOF_UNIMPLEMENTED");
            }
            ValidateNumeric(id, rule["numeric"], errors);
        }
        ValidateExemptions(manifest, register, exemptions, implemented, errors);
    }

    private static void ValidateNumeric(string id, JsonNode? numeric, List<string> errors)
    {
        var limits = new Dictionary<string, int>(StringComparer.Ordinal) { ["R-07"] = 4, ["R-08"] = 4, ["R-09"] = 8, ["R-10"] = 120, ["R-37"] = 60, ["R-74"] = 0 };
        if (!limits.ContainsKey(id) && id is not "R-49" and not "R-82") return;
        if (numeric is null || string.IsNullOrWhiteSpace(Text(numeric["unit"])) || string.IsNullOrWhiteSpace(Text(numeric["comparison"])) || string.IsNullOrWhiteSpace(Text(numeric["rounding"]))) { errors.Add(id + ": NUMERIC_SEMANTICS"); return; }
        if (limits.TryGetValue(id, out var limit) && numeric["limit"]?.GetValue<int>() != limit) errors.Add(id + ": NUMERIC_LIMIT");
        if (id is "R-49" or "R-82" && !numeric["limits"]!.AsArray().Select(n => n!.GetValue<int>()).SequenceEqual(new[] { 90, 80 })) errors.Add(id + ": NUMERIC_LIMIT");
        if (id == "R-37" && !numeric["boundaries"]!.AsArray().Select(n => n!.GetValue<int>()).SequenceEqual(new[] { 59, 60, 61 })) errors.Add(id + ": NUMERIC_BOUNDARIES");
        var units = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["R-07"] = "spaces", ["R-08"] = "spaces", ["R-09"] = "spaces past continued line",
            ["R-10"] = "UTF-16 code units excluding line terminator",
            ["R-37"] = "lines between body braces including blank lines; local functions also count in parent",
            ["R-49"] = "covered/total lines and branches", ["R-82"] = "covered/total lines and branches",
            ["R-74"] = "enum numeric sentinel",
        };
        var comparison = id switch { "R-07" or "R-08" or "R-09" => "exact", "R-49" or "R-82" => "inclusive minimum percent via integer cross multiplication", "R-74" => "Unknown = 0 is never valid saved state", _ => "inclusive maximum" };
        var rounding = id == "R-37" ? "none; expression/global bodies follow MemberBodies; 40 advised only" : "none";
        if (Text(numeric["unit"]) != units[id] || Text(numeric["comparison"]) != comparison || Text(numeric["rounding"]) != rounding) errors.Add(id + ": NUMERIC_SEMANTICS");
        if (id == "R-37")
        {
            var proofs = numeric["boundaryProofs"]?.AsArray();
            if (proofs is null) { errors.Add(id + ": NUMERIC_BOUNDARY_PROOFS"); return; }
            var unique = Unique(proofs, n => Text(n["id"]), "BOUNDARY_PROOF", errors);
            if (!unique.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(new[] { "rules.body.59", "rules.body.60", "rules.body.61" })) errors.Add(id + ": NUMERIC_BOUNDARY_PROOFS");
            foreach (var row in proofs)
                if (Text(row?["rule"]) != id || Text(row?["path"]) != "tests/phase1/Analyzers/BodyLengthAnalyzerTests.cs" || Text(row?["status"]) != "pending") errors.Add(id + ": NUMERIC_BOUNDARY_PROOFS");
        }
    }

    private static void ValidateExemptions(JsonObject manifest, JsonObject register, JsonObject exemptions, bool implemented, List<string> errors)
    {
        var sources = Unique(Rows(manifest, "exemptions"), n => Text(n["id"]), "SOURCE_EXEMPTION", errors);
        var local = Unique(Rows(exemptions, "local"), n => Text(n["id"]), "LOCAL_EXEMPTION_ID", errors);
        foreach (var entry in Rows(exemptions, "donorExemptions"))
        {
            if (entry is null) continue;
            var id = Text(entry["sourceId"]);
            if (!sources.TryGetValue(id, out var source) || Text(entry["rule"]) != Text(source["sourceId"])) errors.Add(id + ": EXEMPTION_REVERSE_ID");
            var expectedDisposition = new[] { "EX-01", "EX-02", "EX-03", "EX-11", "EX-13", "EX-19", "EX-20", "EX-21", "EX-23" }.Contains(id, StringComparer.Ordinal) ? "adapted" : "inapplicable";
            if (Text(entry["disposition"]) != expectedDisposition) errors.Add(id + ": EXEMPTION_DISPOSITION");
            SameStrings(local.Where(pair => Text(pair.Value["sourceId"]) == id).Select(pair => pair.Key), entry["localIds"]!.AsArray(), id + ": LOCAL_EXEMPTION_REFERENCE", errors);
        }
        var scopes = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (id, entry) in local)
        {
            foreach (var field in new[] { "sourceId", "rule", "diagnostic", "path", "surface", "subject", "reason", "positiveProof", "negativeProof" })
                if (string.IsNullOrWhiteSpace(Text(entry[field]))) errors.Add(id + ": EXEMPTION_FIELD: " + field);
            var path = Text(entry["path"]);
            if (!scopes.Add(Text(entry["diagnostic"]) + ":" + path)) errors.Add(id + ": EXEMPTION_OVERLAP");
            if (!ExactPath(path) || path.IndexOfAny(['*', '?', '{', '}']) >= 0) errors.Add(id + ": EXEMPTION_SCOPE");
            if (id != "KZ-EX-LOCK-01" || Text(entry["sourceId"]) != "EX-11" || Text(entry["rule"]) != "R-04" || Text(entry["diagnostic"]) != "FORMAT_FINAL_NEWLINE" || path != "tests/phase1/packages.lock.json" || Text(entry["subject"]) != "no final newline") errors.Add(id + ": EXEMPTION_UNOBSERVED");
            if (Text(entry["positiveProof"]) != Text(entry["rule"]) + ".positive" || Text(entry["negativeProof"]) != Text(entry["rule"]) + ".negative") errors.Add(id + ": EXEMPTION_PROOF");
            if (Text(entry["status"]) != "pending" || implemented) errors.Add(id + ": EXEMPTION_BOUNDARY_PENDING");
        }
        foreach (var rule in Rows(register, "rules"))
            SameStrings(local.Where(pair => Text(pair.Value["rule"]) == Text(rule!["sourceId"])).Select(pair => pair.Key), rule!["exemptions"]!.AsArray(), Text(rule["sourceId"]) + ": EXEMPTION_REFERENCE", errors);
    }

    private static string Text(JsonNode? node) => node?.GetValue<string>() ?? "";
    private static JsonArray Rows(JsonObject node, string name) => node[name] as JsonArray ?? throw new InvalidDataException("ARRAY_REQUIRED: " + name);
    private static string Hash(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));
    private static string HeadingKey(JsonNode node) => Text(node["path"]) + ":" + node["line"]!.GetValue<int>();

    private static Dictionary<string, JsonNode> Unique(JsonArray rows, Func<JsonNode, string> key, string diagnostic, List<string> errors)
    {
        var result = new Dictionary<string, JsonNode>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            if (row is null) { errors.Add(diagnostic + ": NULL_ROW"); continue; }
            var id = key(row);
            if (string.IsNullOrWhiteSpace(id) || !result.TryAdd(id, row)) errors.Add(id + ": " + diagnostic + ": DUPLICATE_OR_EMPTY");
        }
        return result;
    }

    private static void SameStrings(IEnumerable<string> expected, JsonArray? actual, string diagnostic, List<string> errors)
    {
        if (actual is null) { errors.Add(diagnostic); return; }
        var values = actual.Select(Text).ToArray();
        if (values.Distinct(StringComparer.Ordinal).Count() != values.Length || !expected.ToHashSet(StringComparer.Ordinal).SetEquals(values)) errors.Add(diagnostic);
    }

    private static Dictionary<string, byte[]> Archive(List<string> errors)
    {
        var file = Path.Combine(ProofProtocol.Root, "docs/code/provenance/langproj-baseline.zip");
        if (new FileInfo(file).Length > 20_000_000) throw new InvalidDataException("Archive size limit");
        var bytes = File.ReadAllBytes(file);
        if (Hash(bytes) != ArchiveHash) throw new InvalidDataException("CAPTURE: ARCHIVE_HASH");
        using var stream = new MemoryStream(bytes);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        var files = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        long total = 0;
        foreach (var entry in zip.Entries)
        {
            if (entry.FullName.EndsWith('/')) continue;
            if (entry.Length > 4_000_000 || (total += entry.Length) > 20_000_000) throw new InvalidDataException("Expanded size limit");
            if (entry.FullName.StartsWith('/') || entry.FullName.Contains('\\') || entry.FullName.Split('/').Any(x => x is ".." or "." or "")) throw new InvalidDataException("Noncanonical ZIP path");
            using var input = entry.Open();
            using var output = new MemoryStream();
            input.CopyTo(output);
            if (!files.TryAdd(entry.FullName, output.ToArray())) errors.Add("CAPTURE: DUPLICATE_ZIP_PATH: " + entry.FullName);
        }
        return files;
    }

    private static void ValidateCapture(JsonObject manifest, JsonObject register, JsonObject exemptions, List<string> errors)
    {
        if (Text(manifest["revision"]) != Revision || Text(register["revision"]) != Revision || manifest["cleanAtCapture"]?.GetValue<bool>() != true) errors.Add("CAPTURE: REVISION");
        if (Text(manifest["archiveSha256"]) != ArchiveHash || Text(register["archiveSha256"]) != ArchiveHash) errors.Add("CAPTURE: ARCHIVE_IDENTITY");
        var files = Archive(errors);
        var capturedFiles = Unique(Rows(manifest, "files"), n => Text(n["path"]), "CAPTURE_FILE", errors);
        if (!files.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(capturedFiles.Keys)) errors.Add("CAPTURE: FILE_INVENTORY");
        foreach (var (path, row) in capturedFiles)
            if (!files.TryGetValue(path, out var bytes) || Hash(bytes) != Text(row["sha256"]) || bytes.Length != row["size"]!.GetValue<long>() || Text(row["modification"]) != "unmodified") errors.Add("CAPTURE: FILE_HASH: " + path);
        var expectedHeadings = new List<JsonNode>();
        foreach (var name in new[] { "code-style.md", "code-architecture.md" })
        {
            var path = DonorDocs + "CodeRules/" + name;
            var lines = StrictUtf8.GetString(files[path]).Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                if (!lines[i].StartsWith("### ", StringComparison.Ordinal)) continue;
                var end = i + 1;
                while (end < lines.Length && !lines[end].StartsWith("### ", StringComparison.Ordinal)) end++;
                var enforcement = lines[(i + 1)..end].SingleOrDefault(x => x.StartsWith("Enforcement:", StringComparison.Ordinal));
                if (enforcement is null) continue;
                var ids = Regex.Matches(enforcement, @"\[(R-\d+)\]").Select(m => m.Groups[1].Value).ToArray();
                expectedHeadings.Add(new JsonObject { ["path"] = path, ["line"] = i + 1, ["title"] = lines[i][4..].Trim(), ["sourceIds"] = new JsonArray(ids.Select(x => (JsonNode?)JsonValue.Create(x)).ToArray()) });
            }
        }
        if (expectedHeadings.Count != 84 || expectedHeadings.Count(n => Text(n["path"]).EndsWith("/code-style.md", StringComparison.Ordinal)) != 49) errors.Add("CAPTURE: HEADING_COUNT");
        var headings = Unique(Rows(manifest, "headings"), HeadingKey, "HEADING", errors);
        if (!headings.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(expectedHeadings.Select(HeadingKey))) errors.Add("CAPTURE: HEADING_INVENTORY");
        foreach (var expected in expectedHeadings)
        {
            if (!headings.TryGetValue(HeadingKey(expected), out var actual)) continue;
            if (Text(expected["title"]) != Text(actual["title"])) errors.Add(HeadingKey(expected) + ": HEADING_TITLE");
            SameStrings(expected["sourceIds"]!.AsArray().Select(Text), actual["sourceIds"]!.AsArray(), HeadingKey(expected) + ": HEADING_IDS", errors);
        }
        var matrix = Unique(Rows(manifest, "matrix"), n => Text(n["sourceId"]), "MATRIX_ID", errors);
        var sourceExemptions = Unique(Rows(manifest, "exemptions"), n => Text(n["id"]), "SOURCE_EXEMPTION", errors);
        var expectedIds = new List<string>();
        var expectedExemptions = new List<string>();
        var matrixLines = StrictUtf8.GetString(files[DonorDocs + "rule-matrix.md"]).Split('\n');
        for (var i = 0; i < matrixLines.Length; i++)
        {
            var match = Regex.Match(matrixLines[i], "^\\| <a id=\"(?:r|ex)-[0-9]+\"></a>((?:R|EX)-[0-9]+) \\|");
            if (!match.Success) continue;
            var id = match.Groups[1].Value;
            var isRule = id.StartsWith("R-", StringComparison.Ordinal);
            (isRule ? expectedIds : expectedExemptions).Add(id);
            var rows = isRule ? matrix : sourceExemptions;
            if (!rows.TryGetValue(id, out var row) || Text(row["raw"]) != matrixLines[i] || row["line"]!.GetValue<int>() != i + 1) errors.Add(id + ": SOURCE_ROW");
            if (row is not null)
            {
                var columns = matrixLines[i].Split('|')[1..^1].Select(x => x.Trim()).ToArray();
                var names = isRule ? new[] { "title", "mechanism", "scope", "proofs", "level" } : new[] { "path", "subject", "reason" };
                for (var column = 0; column < names.Length; column++)
                    if (Text(row[names[column]]) != columns[column + 1]) errors.Add(id + ": SOURCE_FIELD: " + names[column]);
                if (!isRule && Text(row["sourceId"]) != Regex.Match(columns[4], "R-[0-9]+").Value) errors.Add(id + ": SOURCE_RULE_ID");
            }
        }
        if (expectedIds.Count != 85 || !matrix.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(expectedIds)) errors.Add("CAPTURE: MATRIX_INVENTORY");
        if (expectedExemptions.Count != 24 || expectedExemptions.Contains("EX-22") || !sourceExemptions.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(expectedExemptions)) errors.Add("CAPTURE: EXEMPTION_INVENTORY");
        var rules = Unique(Rows(register, "rules"), n => Text(n["sourceId"]), "RULE_ID", errors);
        if (!rules.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(expectedIds)) errors.Add("REGISTER: RULE_INVENTORY");
        foreach (var (id, rule) in rules)
        {
            if (!matrix.ContainsKey(id)) { errors.Add(id + ": UNKNOWN_RULE"); continue; }
            SameStrings(expectedHeadings.Where(h => h["sourceIds"]!.AsArray().Select(Text).Contains(id, StringComparer.Ordinal)).Select(HeadingKey), rule["headingRefs"] as JsonArray, id + ": HEADING_RELATION", errors);
            if (!new[] { "retained", "adapted", "inapplicable" }.Contains(Text(rule["disposition"]), StringComparer.Ordinal) || string.IsNullOrWhiteSpace(Text(rule["reason"]))) errors.Add(id + ": DISPOSITION");
        }
        var dispositions = Unique(Rows(exemptions, "donorExemptions"), n => Text(n["sourceId"]), "EXEMPTION_ID", errors);
        if (!dispositions.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(expectedExemptions)) errors.Add("REGISTER: DONOR_EXEMPTION_INVENTORY");
        foreach (var (id, entry) in dispositions)
            if (!new[] { "retained", "adapted", "inapplicable" }.Contains(Text(entry["disposition"]), StringComparer.Ordinal) || string.IsNullOrWhiteSpace(Text(entry["reason"]))) errors.Add(id + ": DISPOSITION");
        if (Text(manifest["licenseNotice"]?["finding"]).Length == 0 || Text(manifest["licenseNotice"]?["modifications"]).Length == 0) errors.Add("CAPTURE: LICENSE_NOTICE");
    }
}
