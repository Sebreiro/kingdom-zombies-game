using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace KingdomZombies.Phase1.Tests;

public static class RegisterValidator
{
    public const string Revision = "6836a9718ff5c939e89abf2b97763760607de192";
    public const string ArchiveHash = "19c3766fddf638b02a2b78d969e222448ecd98a22b79f70e8b28effd858dfcca";
    private const string DonorDocs = "src/backend/docs/";
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    public static JsonObject Read(string relative) => JsonNode.Parse(File.ReadAllText(Path.Combine(ProofProtocol.Root, relative)))!.AsObject();

    public static IReadOnlyList<string> Validate(JsonObject manifest, JsonObject register, JsonObject exemptions, bool requireImplemented = false)
    {
        var errors = new List<string>();
        try
        {
            ValidateCapture(manifest, register, exemptions, errors);
        }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or KeyNotFoundException or InvalidDataException or DecoderFallbackException)
        {
            errors.Add("REGISTER: INVALID_STRUCTURE: " + error.Message);
        }
        return errors;
    }

    private static string Text(JsonNode? node) => node?.GetValue<string>() ?? "";
    private static JsonArray Rows(JsonObject node, string name) => node[name]!.AsArray();
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

    private static void SameStrings(IEnumerable<string> expected, JsonArray actual, string diagnostic, List<string> errors)
    {
        var values = actual.Select(Text).ToArray();
        if (values.Distinct(StringComparer.Ordinal).Count() != values.Length || !expected.ToHashSet(StringComparer.Ordinal).SetEquals(values)) errors.Add(diagnostic);
    }

    private static Dictionary<string, byte[]> Archive(List<string> errors)
    {
        var bytes = File.ReadAllBytes(Path.Combine(ProofProtocol.Root, "docs/code/provenance/langproj-baseline.zip"));
        if (Hash(bytes) != ArchiveHash) errors.Add("CAPTURE: ARCHIVE_HASH");
        if (bytes.Length > 20_000_000) throw new InvalidDataException("Archive size limit");
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
        }
        if (expectedIds.Count != 85 || !matrix.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(expectedIds)) errors.Add("CAPTURE: MATRIX_INVENTORY");
        if (expectedExemptions.Count != 24 || expectedExemptions.Contains("EX-22") || !sourceExemptions.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(expectedExemptions)) errors.Add("CAPTURE: EXEMPTION_INVENTORY");
        var rules = Unique(Rows(register, "rules"), n => Text(n["sourceId"]), "RULE_ID", errors);
        if (!rules.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(expectedIds)) errors.Add("REGISTER: RULE_INVENTORY");
        foreach (var (id, rule) in rules)
        {
            if (!matrix.ContainsKey(id)) { errors.Add(id + ": UNKNOWN_RULE"); continue; }
            SameStrings(expectedHeadings.Where(h => h["sourceIds"]!.AsArray().Select(Text).Contains(id, StringComparer.Ordinal)).Select(HeadingKey), rule["headingRefs"]!.AsArray(), id + ": HEADING_RELATION", errors);
            if (!new[] { "retained", "adapted", "inapplicable" }.Contains(Text(rule["disposition"]), StringComparer.Ordinal) || string.IsNullOrWhiteSpace(Text(rule["reason"]))) errors.Add(id + ": DISPOSITION");
        }
        var dispositions = Unique(Rows(exemptions, "donorExemptions"), n => Text(n["sourceId"]), "EXEMPTION_ID", errors);
        if (!dispositions.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(expectedExemptions)) errors.Add("REGISTER: DONOR_EXEMPTION_INVENTORY");
        foreach (var (id, entry) in dispositions)
            if (!new[] { "retained", "adapted", "inapplicable" }.Contains(Text(entry["disposition"]), StringComparer.Ordinal) || string.IsNullOrWhiteSpace(Text(entry["reason"]))) errors.Add(id + ": DISPOSITION");
        if (Text(manifest["licenseNotice"]?["finding"]).Length == 0 || Text(manifest["licenseNotice"]?["modifications"]).Length == 0) errors.Add("CAPTURE: LICENSE_NOTICE");
    }
}
