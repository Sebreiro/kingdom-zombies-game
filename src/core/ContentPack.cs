using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace KingdomZombies.Core;

/// <summary>Publishes detached validated pack definitions and an independent, complete required census.</summary>
public sealed class ContentPack
{
    /// <summary>Maximum accepted document bytes before parsing.</summary>
    public const int MaxDocumentBytes = 1_048_576;
    /// <summary>Maximum accepted resource bytes before hashing or native decode.</summary>
    public const int MaxResourceBytes = 16_777_216;
    /// <summary>Canonical independent packaged expectation input.</summary>
    public const string ExpectedPath = "content/expected-obligations.json";
    /// <summary>Authenticates the approved independent names, sources and complete census.</summary>
    public const string ExpectedSha256 = "a4d9243bda1b512b59c189a3bdc48a7cccc8171a218bbfc5ae16b00274f2fda5";

    /// <summary>Immutable definitions keyed by exact output identity with explicit obligation mappings.</summary>
    public IReadOnlyDictionary<string, JsonElement> Entries { get; }
    /// <summary>All180 required IDs, with authenticated names or an explicit unavailable-metadata reason.</summary>
    public IReadOnlyDictionary<string, string> Expected { get; }
    /// <summary>Ordered file, field, value and reason diagnostics outside metadata disclosure.</summary>
    public IReadOnlyList<string> Diagnostics { get; }
    /// <summary>Completion requires separate current native, listening and judgment acceptance.</summary>
    public string Status { get; } = "Pack incomplete — acceptance pending 04-32/04-33";

    /// <summary>Translates only the exact authored project prefix into a canonical portable resource key.</summary>
    public static string ResourcePath(string path)
    {
        ArgumentNullException.ThrowIfNull(argument: path);
        const string Prefix = "src/godot-game/";
        if (!path.StartsWith(value: Prefix, comparisonType: StringComparison.Ordinal))
        {
            throw new FormatException(message: "path requires exact src/godot-game/ prefix: " + path);
        }

        string relative = path[Prefix.Length..];
        if (relative.Length is 0 or > 240 || relative.Any(predicate: character =>
                !(character is >= 'a' and <= 'z' or >= '0' and <= '9' or '/' or '.' or '-' or '_'))
                || relative.Split(separator: '/').Any(predicate: part => part.Length == 0 || part is "." or ".."
                        || part.EndsWith(value: '.')))
        {
            throw new FormatException(message: "canonical relative path required: " + path);
        }

        return relative;
    }

    /// <summary>Enumerates canonical declared reads; invalid rows remain diagnostic loader inputs.</summary>
    public static IReadOnlyList<string> Paths(byte[]? index)
    {
        var paths = new SortedSet<string>(comparer: StringComparer.Ordinal);
        try
        {
            using JsonDocument parsed = Parse(bytes: index);
            JsonElement root = parsed.RootElement;
            Header(root: root);
            foreach (JsonElement row in Rows(root: root, field: "shards").Concat(
                    second: Rows(root: root, field: "outputs")))
            {
                try
                {
                    paths.Add(item: ResourcePath(path: Text(row: row, field: "path")));
                }
                catch (FormatException)
                {
                    continue;
                }
            }
        }
        catch (Exception error) when (error is FormatException or JsonException or DecoderFallbackException)
        {
            return [];
        }

        return [.. paths];
    }

    /// <summary>Validates independent inputs without allowing one bad shard to suppress another valid output.</summary>
    public static ContentPack Load(byte[]? index, byte[]? expectations, Func<string, byte[]?> read)
    {
        ArgumentNullException.ThrowIfNull(argument: read);
        var entries = new Dictionary<string, JsonElement>(comparer: StringComparer.Ordinal);
        Dictionary<string, string> expected = Census();
        var errors = new List<string>();
        ReadExpected(bytes: expectations, expected: expected, errors: errors);
        try
        {
            using JsonDocument parsed = Parse(bytes: index);
            JsonElement root = parsed.RootElement;
            Header(root: root);
            CheckExpectedReference(root: root, errors: errors);
            Dictionary<string, JsonElement> shards = ReadShards(root: root, read: read, errors: errors);
            ReadOutputs(root: root, shards: shards, read: read, entries: entries, expected: expected, errors: errors);
        }
        catch (Exception error) when (error is FormatException or JsonException or DecoderFallbackException)
        {
            errors.Add(item: Diagnostic(id: "pack", path: "content/pack-index.json", field: "$",
                    value: "index", reason: error.Message));
        }

        foreach (string id in expected.Keys)
        {
            errors.Add(item: Diagnostic(id: id, path: ExpectedPath, field: "acceptance", value: id,
                    reason: "Incomplete obligation; current production/native/listening/judgment evidence required"));
        }

        return new ContentPack(entries: entries, expected: expected, errors: errors);
    }

    /// <summary>Returns zero/one/many output IDs matching category, status and ordinal search.</summary>
    public IReadOnlyList<string> Search(string query, string category, string status)
    {
        return [.. Entries.Where(predicate: pair => (category.Length == 0 || Mappings(entry: pair.Value)
                .Any(predicate: id => StringComparer.Ordinal.Equals(x: Category(id: id), y: category)))
                && (status.Length == 0 || StringComparer.Ordinal.Equals(x: status, y: "Partial"))
                && (pair.Key.Contains(value: query, comparisonType: StringComparison.OrdinalIgnoreCase)
                        || Text(row: pair.Value, field: "name").Contains(value: query,
                                comparisonType: StringComparison.OrdinalIgnoreCase)))
                .Select(selector: pair => pair.Key).Order(comparer: StringComparer.Ordinal)];
    }

    /// <summary>Maps required IDs to the six common inspection categories without renaming identities.</summary>
    public static string Category(string id)
    {
        return id.Split(separator: '-')[0] switch
        {
            "CH" or "AN" => "Characters",
            "IT" => "Items",
            "BL" or "EN" or "FX" => "Buildings/world",
            "UI" => "UI",
            "AU" => "Audio",
            "DT" or "TXT" => "Data/text",
            _ => "Unknown",
        };
    }

    /// <summary>Reads explicit shared obligation mappings, preserving each distinct obligation identity.</summary>
    public static IReadOnlyList<string> Mappings(JsonElement entry)
    {
        return [.. Rows(root: entry, field: "obligations").Select(selector: row => Text(row: row, field: "id"))];
    }

    private ContentPack(Dictionary<string, JsonElement> entries, Dictionary<string, string> expected,
            List<string> errors)
    {
        Entries = new ReadOnlyDictionary<string, JsonElement>(dictionary: entries);
        Expected = new ReadOnlyDictionary<string, string>(dictionary: expected);
        Diagnostics = Array.AsReadOnly(array: errors.Distinct(comparer: StringComparer.Ordinal)
                .Order(comparer: StringComparer.Ordinal).ToArray());
    }

    private static Dictionary<string, string> Census()
    {
        var result = new Dictionary<string, string>(comparer: StringComparer.Ordinal);
        foreach ((string prefix, int count) in new (string, int)[]
        {
            ("CH", 13), ("AN", 18), ("IT", 24), ("BL", 16), ("EN", 11), ("FX", 12),
            ("UI", 27), ("AU", 30), ("DT", 27), ("TXT", 2),
        })
        {
            for (int number = 1; number <= count; number++)
            {
                result.Add(key: prefix + "-" + number.ToString(format: "D2", provider: CultureInfo.InvariantCulture),
                        value: "Unavailable metadata — independent register invalid or missing");
            }
        }

        return result;
    }

    private static void ReadExpected(byte[]? bytes, Dictionary<string, string> expected, List<string> errors)
    {
        try
        {
            using JsonDocument parsed = Parse(bytes: bytes);
            JsonElement root = parsed.RootElement;
            if (bytes?.Length != 25345 || !StringComparer.Ordinal.Equals(x: Digest(bytes: bytes), y: ExpectedSha256)
                    || Integer(row: root, field: "schemaVersion") != 1
                    || !StringComparer.Ordinal.Equals(x: Text(row: root, field: "authority"),
                            y: "specs/0.1/assets-and-resources.md"))
            {
                throw new FormatException(message: "approved independent register bytes/header/hash required");
            }

            JsonElement[] rows = Rows(root: root, field: "obligations");
            if (rows.Length != 180 || rows.Select(selector: row => Text(row: row, field: "id"))
                    .Distinct(comparer: StringComparer.Ordinal).Count() != 180
                    || rows.Any(predicate: row => !expected.ContainsKey(key: Text(row: row, field: "id"))))
            {
                throw new FormatException(message: "exact independent180 census required");
            }

            foreach (JsonElement row in rows)
            {
                expected[Text(row: row, field: "id")] = Text(row: row, field: "name") + "\nSource: "
                        + Text(row: row, field: "source");
            }
        }
        catch (Exception error) when (error is FormatException or JsonException or DecoderFallbackException)
        {
            errors.Add(item: Diagnostic(id: "expected", path: ExpectedPath, field: "$", value: "independent180",
                    reason: error.Message));
        }
    }

    private static void CheckExpectedReference(JsonElement root, List<string> errors)
    {
        try
        {
            JsonElement row = Required(row: root, field: "expectedObligations");
            if (StringComparer.Ordinal.Equals(x: Text(row: row, field: "path"), y: "src/godot-game/" + ExpectedPath)
                    && Integer(row: row, field: "bytes") == 25345
                    && StringComparer.Ordinal.Equals(x: Text(row: row, field: "sha256"), y: ExpectedSha256))
            {
                return;
            }

            throw new FormatException(message: "exact approved independent register reference required");
        }
        catch (FormatException error)
        {
            errors.Add(item: Diagnostic(id: "expected", path: ExpectedPath, field: "expectedObligations",
                    value: root.TryGetProperty(propertyName: "expectedObligations", value: out JsonElement row)
                            ? row.GetRawText() : "missing", reason: error.Message));
        }
    }

    private static Dictionary<string, JsonElement> ReadShards(JsonElement root, Func<string, byte[]?> read,
            List<string> errors)
    {
        var result = new Dictionary<string, JsonElement>(comparer: StringComparer.Ordinal);
        JsonElement[] rows = Rows(root: root, field: "shards");
        string[] duplicates = [.. rows.GroupBy(keySelector: row => Identify(row: row, field: "id"),
                comparer: StringComparer.Ordinal).Where(predicate: group => group.Count() > 1)
                .Select(selector: group => group.Key)];
        foreach (JsonElement row in rows.OrderBy(keySelector: row => Identify(row: row, field: "id"),
                comparer: StringComparer.Ordinal))
        {
            string id = Identify(row: row, field: "id"), path = Identify(row: row, field: "path");
            try
            {
                Stable(id: id);
                if (duplicates.Contains(value: id, comparer: StringComparer.Ordinal))
                {
                    throw new FormatException(message: "duplicate shard ID");
                }

                string key = ResourcePath(path: path);
                if (!key.StartsWith(value: "content/packs/", comparisonType: StringComparison.Ordinal))
                {
                    throw new FormatException(message: "declared shard must be in content/packs/");
                }

                byte[]? bytes = read(arg: key);
                using JsonDocument parsed = Parse(bytes: bytes);
                CheckBytes(row: row, bytes: bytes);
                JsonElement shard = parsed.RootElement;
                if (!StringComparer.Ordinal.Equals(x: Text(row: shard, field: "id"), y: id)
                        || Integer(row: shard, field: "schemaVersion") != 1)
                {
                    throw new FormatException(message: "shard identity/schema mismatch");
                }

                result.Add(key: id, value: shard.Clone());
            }
            catch (Exception error) when (error is FormatException or JsonException or DecoderFallbackException)
            {
                errors.Add(item: Diagnostic(id: id, path: path, field: "shards", value: row.GetRawText(),
                        reason: error.Message));
            }
        }

        return result;
    }

    private static void ReadOutputs(JsonElement root, Dictionary<string, JsonElement> shards,
            Func<string, byte[]?> read, Dictionary<string, JsonElement> entries,
            Dictionary<string, string> expected, List<string> errors)
    {
        JsonElement[] rows = Rows(root: root, field: "outputs");
        string[] duplicates = [.. rows.GroupBy(keySelector: row => Identify(row: row, field: "id"),
                comparer: StringComparer.Ordinal).Where(predicate: group => group.Count() > 1)
                .Select(selector: group => group.Key)];
        foreach (JsonElement row in rows.OrderBy(keySelector: row => Identify(row: row, field: "id"),
                comparer: StringComparer.Ordinal))
        {
            string id = Identify(row: row, field: "id"), path = Identify(row: row, field: "path");
            try
            {
                Stable(id: id);
                string key = ResourcePath(path: path);
                if (duplicates.Contains(value: id, comparer: StringComparer.Ordinal)
                        || !shards.TryGetValue(key: Text(row: row, field: "shard"), value: out JsonElement shard))
                {
                    throw new FormatException(message: "duplicate output ID or unavailable declared shard");
                }

                string[] mappings = [.. Mappings(entry: row)];
                if (mappings.Length == 0 || mappings.Distinct(comparer: StringComparer.Ordinal).Count()
                        != mappings.Length || mappings.Any(predicate: mapped => !expected.ContainsKey(key: mapped)))
                {
                    throw new FormatException(message: "unknown/duplicate/empty obligation ID mapping");
                }

                JsonElement[] resources = Rows(root: shard, field: "resources");
                JsonElement[] matches = [.. resources.Where(predicate: item => StringComparer.Ordinal.Equals(
                        x: Identify(row: item, field: "id"), y: id))];
                if (matches.Length != 1 || !StringComparer.Ordinal.Equals(x: Text(row: matches[0], field: "path"),
                        y: key) || !StringComparer.Ordinal.Equals(x: Text(row: matches[0], field: "sha256"),
                                y: Text(row: row, field: "sha256")))
                {
                    throw new FormatException(message: "exact independent shard resource binding required");
                }

                CheckBytes(row: row, bytes: read(arg: key));
                entries.Add(key: id, value: row.Clone());
            }
            catch (Exception error) when (error is FormatException or JsonException or DecoderFallbackException)
            {
                errors.Add(item: Diagnostic(id: id, path: path, field: "outputs", value: row.GetRawText(),
                        reason: error.Message));
            }
        }
    }

    private static JsonDocument Parse(byte[]? bytes)
    {
        if (bytes is null || bytes.Length is 0 or > MaxDocumentBytes)
        {
            throw new FormatException(message: "document missing/empty/oversized");
        }

        _ = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true).GetString(bytes: bytes);
        var parsed = JsonDocument.Parse(utf8Json: bytes, options: new JsonDocumentOptions { MaxDepth = 16 });
        try
        {
            Validate(element: parsed.RootElement);
            return parsed;
        }
        catch
        {
            parsed.Dispose();
            throw;
        }
    }

    private static void Validate(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (!names.Add(item: property.Name))
                {
                    throw new FormatException(message: "duplicate field: " + property.Name);
                }

                Validate(element: property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            if (element.GetArrayLength() > 1024)
            {
                throw new FormatException(message: "collection exceeds1024 records");
            }

            foreach (JsonElement child in element.EnumerateArray())
            {
                Validate(element: child);
            }
        }
        else if (element.ValueKind == JsonValueKind.Number && !double.IsFinite(d: element.GetDouble()))
        {
            throw new FormatException(message: "nonfinite number");
        }
    }

    private static JsonElement Required(JsonElement row, string field)
    {
        if (row.ValueKind != JsonValueKind.Object || !row.TryGetProperty(propertyName: field,
                value: out JsonElement value))
        {
            throw new FormatException(message: "missing field: " + field);
        }

        return value;
    }

    private static string Text(JsonElement row, string field)
    {
        JsonElement value = Required(row: row, field: field);
        string? text = value.ValueKind == JsonValueKind.String ? value.GetString() : null;
        if (string.IsNullOrWhiteSpace(value: text) || text.Length > 8192)
        {
            throw new FormatException(message: "nonempty bounded text required: " + field);
        }

        return text;
    }

    private static JsonElement[] Rows(JsonElement root, string field)
    {
        JsonElement value = Required(row: root, field: field);
        if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() > 1024)
        {
            throw new FormatException(message: "bounded array required: " + field);
        }

        return [.. value.EnumerateArray()];
    }

    private static void Header(JsonElement root)
    {
        if (Integer(row: root, field: "schemaVersion") != 1
                || !StringComparer.Ordinal.Equals(x: Text(row: root, field: "id"), y: "kingdom-zombies.v0.1")
                || Integer(row: root, field: "version") != 1)
        {
            throw new FormatException(message: "exact pack identity/schema/version required");
        }
    }

    private static void Stable(string id)
    {
        if (id.Length is 0 or > 128 || id[0] is < 'a' or > 'z'
                || id.Any(predicate: character => !(character is >= 'a' and <= 'z' or >= '0' and <= '9'
                        or '.' or '-')) || id.Contains(value: "..", comparisonType: StringComparison.Ordinal))
        {
            throw new FormatException(message: "canonical exact stable ID required: " + id);
        }
    }

    private static void CheckBytes(JsonElement row, byte[]? bytes)
    {
        if (bytes is null || bytes.Length is 0 or > MaxResourceBytes
                || Integer(row: row, field: "bytes") != bytes.Length
                || !StringComparer.Ordinal.Equals(x: Text(row: row, field: "sha256"), y: Digest(bytes: bytes)))
        {
            throw new FormatException(message: "resource missing/oversized/bytes/hash mismatch");
        }
    }

    private static string Digest(byte[]? bytes)
    {
        return bytes is null ? "missing" : Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes));
    }

    private static long Integer(JsonElement row, string field)
    {
        JsonElement value = Required(row: row, field: field);
        if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt64(value: out long number))
        {
            throw new FormatException(message: "integer required: " + field);
        }

        return number;
    }

    private static string Identify(JsonElement row, string field)
    {
        return row.ValueKind == JsonValueKind.Object && row.TryGetProperty(propertyName: field,
                value: out JsonElement value) && value.ValueKind == JsonValueKind.String
                ? value.GetString() ?? "<missing>" : "<missing>";
    }

    private static string Diagnostic(string id, string path, string field, string value, string reason)
    {
        return id + ": res://" + (path.StartsWith(value: "src/godot-game/", comparisonType: StringComparison.Ordinal)
                ? path["src/godot-game/".Length..] : path) + ", field " + field + ", value " + value + " — " + reason;
    }
}
