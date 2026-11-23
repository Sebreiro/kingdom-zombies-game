using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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
        string[] canonical = ["assets/ui/LiberationSans.ttf", "assets/ui/Fallback.ttf",
            "assets/ui/PerfectDOSVGA437.ttf", "assets/ui/InspectionTheme.tres",
            "inspection/ItemsWeaponsPreview.tscn", "inspection/ItemsWearablesPreview.tscn"];
        if (canonical.Contains(value: relative, comparer: StringComparer.Ordinal))
        {
            return relative;
        }

        if (canonical.Contains(value: relative, comparer: StringComparer.OrdinalIgnoreCase))
        {
            throw new FormatException(message: "exact ordinal canonical resource identity required: " + path);
        }

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
            if (root.TryGetProperty(propertyName: "model", value: out JsonElement model)
                    && model.ValueKind is JsonValueKind.String && model.GetString() is "completed-shards-v1")
            {
                return [.. Rows(root: root, field: "files").Where(predicate: row =>
                        Text(row: row, field: "role") is "runtime").Select(selector: row =>
                                ResourcePath(path: Text(row: row, field: "path")))
                        .Distinct(comparer: StringComparer.Ordinal).Order(comparer: StringComparer.Ordinal)];
            }

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
            if (root.TryGetProperty(propertyName: "model", value: out JsonElement model)
                    && model.ValueKind is JsonValueKind.String && model.GetString() is "completed-shards-v1")
            {
                ReadCompleted(root: root, read: read, entries: entries, expected: expected, errors: errors);
            }
            else
            {
                Dictionary<string, JsonElement> shards = ReadShards(root: root, read: read, errors: errors);
                ReadOutputs(root: root, shards: shards, read: read, entries: entries, expected: expected,
                        errors: errors);
            }
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

    private static readonly string[] _completedRequiredFontTables =
            ["cmap", "head", "hhea", "hmtx", "maxp", "name", "loca", "glyf"];

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
                CheckBytes(row: row, bytes: bytes);
                using JsonDocument parsed = Parse(bytes: bytes, path: key);
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

    private static void ReadCompleted(JsonElement root, Func<string, byte[]?> read,
            Dictionary<string, JsonElement> entries, Dictionary<string, string> expected, List<string> errors)
    {
        var files = new Dictionary<string, JsonElement>(comparer: StringComparer.Ordinal);
        var documents = new Dictionary<string, JsonElement>(comparer: StringComparer.Ordinal);
        var pixels = new Dictionary<string, string>(comparer: StringComparer.Ordinal);
        var native = new Dictionary<string, byte[]>(comparer: StringComparer.Ordinal);
        JsonElement[] rows = Rows(root: root, field: "files");
        string[] duplicates = [.. rows.GroupBy(keySelector: row => Identify(row: row, field: "id"),
                comparer: StringComparer.Ordinal).Where(predicate: group => group.Count() > 1)
                .Select(selector: group => group.Key)];
        string[] duplicatePaths = [.. rows.GroupBy(keySelector: row => Identify(row: row, field: "path"),
                comparer: StringComparer.OrdinalIgnoreCase).Where(predicate: group => group.Count() > 1)
                .SelectMany(selector: group => group.Select(selector: row => Identify(row: row, field: "id")))];
        foreach (JsonElement row in rows)
        {
            string id = Identify(row: row, field: "id"), path = Identify(row: row, field: "path");
            try
            {
                Stable(id: id);
                if (duplicates.Contains(value: id, comparer: StringComparer.Ordinal)
                        || duplicatePaths.Contains(value: id, comparer: StringComparer.Ordinal))
                {
                    throw new FormatException(message: "duplicate completed file identity");
                }

                CompletedReference(row: row);
                if (Text(row: row, field: "role") is "runtime")
                {
                    string key = ResourcePath(path: path);
                    byte[]? value = read(arg: key);
                    CheckBytes(row: row, bytes: value);
                    if (key.EndsWith(value: ".json", comparisonType: StringComparison.Ordinal))
                    {
                        using JsonDocument parsed = Parse(bytes: value, path: key);
                        CompletedDocument(path: key, value: parsed.RootElement, reference: row);
                        documents.Add(key: id, value: parsed.RootElement.Clone());
                    }
                    else
                    {
                        pixels.Add(key: path, value: CompletedMedia(path: key, bytes: value ?? []));
                        if (key.EndsWith(value: ".tres", comparisonType: StringComparison.Ordinal)
                                || key.EndsWith(value: ".tscn", comparisonType: StringComparison.Ordinal))
                        {
                            native.Add(key: id, value: value ?? []);
                        }
                    }
                }

                files.Add(key: id, value: row.Clone());
            }
            catch (Exception error) when (error is FormatException or JsonException or DecoderFallbackException)
            {
                errors.Add(item: Diagnostic(id: id, path: path, field: "files", value: row.GetRawText(),
                        reason: error.Message));
            }
        }

        CompletedNativeFiles(files: files, native: native, errors: errors);
        CompletedLinks(files: files, documents: documents, pixels: pixels, errors: errors);
        CompletedRows(root: root, files: files, documents: documents, entries: entries, expected: expected,
                errors: errors);
    }

    private static void CompletedReference(JsonElement row)
    {
        CompletedVersion(row: row);
        string path = Text(row: row, field: "path"), digest = Text(row: row, field: "sha256");
        string role = Text(row: row, field: "role");
        long size = Integer(row: row, field: "bytes");
        string identity = "file." + Digest(bytes: Encoding.UTF8.GetBytes(s: path))[..20];
        if (!StringComparer.Ordinal.Equals(x: Text(row: row, field: "id"), y: identity)
                || digest.Length != 64 || digest.Any(predicate: value => !(value is >= '0' and <= '9'
                        or >= 'a' and <= 'f')) || size <= 0 || size > (path is
                        "production/phase4/audio-interface-ambience.zip" ? 67_108_864 : 33_554_432)
                || role is not "runtime" and not "authoring" and not "source-support"
                || path.Contains(value: ':') || path.Contains(value: '\\') || path.StartsWith(value: '/')
                || path.Split(separator: '/').Any(predicate: part => part is "" or "." or ".."))
        {
            throw new FormatException(message: "exact bounded completed source reference required");
        }

        if (role is "authoring" && !path.StartsWith(value: "production/phase4/",
                comparisonType: StringComparison.Ordinal))
        {
            throw new FormatException(message: "authoring reference requires production/phase4/");
        }

        if ((path.StartsWith(value: "src/godot-game/content/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "src/godot-game/assets/", comparisonType: StringComparison.Ordinal)
                || path is "src/godot-game/inspection/ItemsWeaponsPreview.tscn"
                        or "src/godot-game/inspection/ItemsWearablesPreview.tscn") && role is not "runtime")
        {
            throw new FormatException(message: "packaged resource must retain exact runtime role");
        }
    }

    private static void CompletedLinks(Dictionary<string, JsonElement> files,
            Dictionary<string, JsonElement> documents, Dictionary<string, string> pixels, List<string> errors)
    {
        int remaining = documents.Count;
        bool changed = true;
        while (changed && remaining-- > 0)
        {
            changed = false;
            var paths = files.Values.ToDictionary(keySelector: row =>
                    Text(row: row, field: "path"), elementSelector: row => row, comparer: StringComparer.Ordinal);
            foreach ((string id, JsonElement value) in documents.ToArray())
            {
                JsonElement reference = files[id];
                string path = Text(row: reference, field: "path");
                try
                {
                    CompletedBoundTree(value: value, paths: paths, pixels: pixels, pointer: "", source: path);
                    if (value.TryGetProperty(propertyName: "atlas", value: out JsonElement atlas))
                    {
                        string key = CompletedBoundPath(path: Text(row: atlas, field: "path"));
                        (int width, int height) = CompletedPngSize(path: ResourcePath(path: key));
                        CompletedAtlas(value: atlas, width: width, height: height);
                        Dictionary<string, (int Width, int Height)> contexts = CompletedRegionSources(
                                path: path, value: value, files: files, documents: documents);
                        CompletedRegions(value: value, width: width, height: height,
                                contexts: contexts, pointer: "");
                    }
                }
                catch (FormatException error)
                {
                    errors.Add(item: Diagnostic(id: id, path: path, field: "dependencies", value: id,
                            reason: error.Message));
                    files.Remove(key: id);
                    documents.Remove(key: id);
                    changed = true;
                }
            }
        }
    }

    private static string CompletedBoundPath(string path)
    {
        if (path.StartsWith(value: "res://", comparisonType: StringComparison.Ordinal))
        {
            return "src/godot-game/" + path[6..];
        }

        return path.StartsWith(value: "assets/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "content/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "inspection/", comparisonType: StringComparison.Ordinal)
                ? "src/godot-game/" + path : path;
    }

    private static void CompletedBoundTree(JsonElement value, Dictionary<string, JsonElement> paths,
            Dictionary<string, string> pixels, string pointer, string source)
    {
        if (value.ValueKind is JsonValueKind.Object)
        {
            if (value.TryGetProperty(propertyName: "sha256", value: out _)
                    && (value.TryGetProperty(propertyName: "path", value: out _)
                            || value.TryGetProperty(propertyName: "file", value: out _))
                    && !(source is "src/godot-game/content/packs/scale-pilot.json"
                            && pointer.StartsWith(value: "/resources/", comparisonType: StringComparison.Ordinal)))
            {
                string raw = value.TryGetProperty(propertyName: "path", value: out _)
                        ? Text(row: value, field: "path") : Text(row: value, field: "file");
                string path = CompletedBoundPath(path: raw);
                if (paths.TryGetValue(key: path, value: out JsonElement target))
                {
                    if (!StringComparer.Ordinal.Equals(x: Text(row: value, field: "sha256"),
                            y: Text(row: target, field: "sha256")))
                    {
                        throw new FormatException(message: "actual dependency hash mismatch " + pointer + " " + raw);
                    }

                    if (value.TryGetProperty(propertyName: "rgbaSha256", value: out _)
                            && pixels.TryGetValue(key: path, value: out string? rgba)
                            && !StringComparer.Ordinal.Equals(x: rgba, y: Text(row: value, field: "rgbaSha256")))
                    {
                        throw new FormatException(message: "actual decoded RGBA identity mismatch " + raw);
                    }
                }
                else if (path.StartsWith(value: "src/godot-game/", comparisonType: StringComparison.Ordinal)
                        || path.StartsWith(value: "production/phase4/", comparisonType: StringComparison.Ordinal))
                {
                    throw new FormatException(message: "actual dependency unavailable " + pointer + " " + raw);
                }
            }

            foreach (JsonProperty child in value.EnumerateObject())
            {
                CompletedBoundTree(value: child.Value, paths: paths, pixels: pixels,
                        pointer: pointer + "/" + child.Name, source: source);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            int index = 0;
            foreach (JsonElement child in value.EnumerateArray())
            {
                CompletedBoundTree(value: child, paths: paths, pixels: pixels,
                        pointer: pointer + "/" + index.ToString(provider: CultureInfo.InvariantCulture),
                        source: source);
                index++;
            }
        }
    }

    private static void CompletedAtlas(JsonElement value, int width, int height)
    {
        if (value.TryGetProperty(propertyName: "size", value: out JsonElement size))
        {
            if (size.ValueKind is not JsonValueKind.Array || size.GetArrayLength() != 2
                    || !size[0].TryGetInt32(value: out int x) || !size[1].TryGetInt32(value: out int y)
                    || x != width || y != height)
            {
                throw new FormatException(message: "atlas dimensions disagree with actual decoded PNG");
            }
        }
        else if (Integer(row: value, field: "width") != width || Integer(row: value, field: "height") != height)
        {
            throw new FormatException(message: "atlas extent disagrees with actual decoded PNG");
        }
    }

    private static void CompletedRegions(JsonElement value, int width, int height,
            Dictionary<string, (int Width, int Height)> contexts, string pointer)
    {
        if (value.ValueKind is JsonValueKind.Object)
        {
            foreach (JsonProperty child in value.EnumerateObject())
            {
                if (child.Name is "atlasRect")
                {
                    (int Width, int Height) extent = contexts.GetValueOrDefault(key: pointer,
                            defaultValue: (width, height));
                    CompletedContextRectangle(rectangle: child.Value, width: extent.Width, height: extent.Height,
                            pointer: pointer + "/atlasRect");
                }

                CompletedRegions(value: child.Value, width: width, height: height, contexts: contexts,
                        pointer: pointer + "/" + child.Name);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            int index = 0;
            foreach (JsonElement child in value.EnumerateArray())
            {
                CompletedRegions(value: child, width: width, height: height, contexts: contexts,
                        pointer: pointer + "/" + index.ToString(provider: CultureInfo.InvariantCulture));
                index++;
            }
        }
    }

    private static int[] CompletedRectangle(JsonElement rectangle, int width, int height)
    {
        if (rectangle.ValueKind is not JsonValueKind.Array || rectangle.GetArrayLength() != 4
                || rectangle.EnumerateArray().Any(predicate: value => value.ValueKind is not JsonValueKind.Number)
                || !rectangle[0].TryGetInt32(value: out int x) || !rectangle[1].TryGetInt32(value: out int y)
                || !rectangle[2].TryGetInt32(value: out int w) || !rectangle[3].TryGetInt32(value: out int h)
                || x < 0 || y < 0 || w <= 0 || h <= 0 || (long)x + w > width || (long)y + h > height)
        {
            throw new FormatException(message: "actual bounded integer atlas region required");
        }

        return [x, y, w, h];
    }

    private static int[] CompletedContextRectangle(JsonElement rectangle, int width, int height, string pointer)
    {
        try
        {
            return CompletedRectangle(rectangle: rectangle, width: width, height: height);
        }
        catch (FormatException error)
        {
            throw new FormatException(message: "contextual " + pointer + " " + error.Message, innerException: error);
        }
    }

    private static Dictionary<string, (int Width, int Height)> CompletedRegionSources(string path,
            JsonElement value, Dictionary<string, JsonElement> files, Dictionary<string, JsonElement> documents)
    {
        var contexts = new Dictionary<string, (int Width, int Height)>(comparer: StringComparer.Ordinal);
        if (path is not "src/godot-game/content/packs/ui-identity.json")
        {
            return contexts;
        }

        JsonElement common = CompletedSourceDocument(path: "content/packs/common.json", files: files,
                documents: documents);
        (int width, int height) = CompletedSourceExtent(value: common, files: files);
        JsonElement[] frames = Rows(root: Required(row: value, field: "theme"), field: "frames");
        JsonElement[] items = Rows(root: value, field: "items");
        if (frames.Length != 4 || items.Length != 24)
        {
            throw new FormatException(message: "contextual identity requires all4theme and24canonical item regions");
        }

        for (int index = 0; index < frames.Length; index++)
        {
            CompletedThemeRegion(value: frames[index], common: common, index: index, width: width, height: height);
            contexts.Add(key: "/theme/frames/" + index.ToString(provider: CultureInfo.InvariantCulture),
                    value: (width, height));
        }

        for (int index = 0; index < items.Length; index++)
        {
            (int itemWidth, int itemHeight) = CompletedItemRegion(value: items[index], index: index,
                    files: files, documents: documents);
            contexts.Add(key: "/items/" + index.ToString(provider: CultureInfo.InvariantCulture),
                    value: (itemWidth, itemHeight));
        }

        JsonElement local = Required(row: value, field: "regions");
        foreach (JsonProperty region in local.EnumerateObject())
        {
            CompletedContextRectangle(rectangle: region.Value, width: 256, height: 64,
                    pointer: "/regions/" + region.Name);
        }

        return contexts;
    }

    private static JsonElement CompletedSourceDocument(string path, Dictionary<string, JsonElement> files,
            Dictionary<string, JsonElement> documents)
    {
        JsonElement reference = files.Values.SingleOrDefault(predicate: row => StringComparer.Ordinal.Equals(
                x: Text(row: row, field: "path"), y: "src/godot-game/" + path));
        if (reference.ValueKind is not JsonValueKind.Object || Text(row: reference, field: "role") is not "runtime"
                || !documents.TryGetValue(key: Text(row: reference, field: "id"), value: out JsonElement result))
        {
            throw new FormatException(message: "contextual authenticated source definition unavailable " + path);
        }

        return result;
    }

    private static (int Width, int Height) CompletedSourceExtent(JsonElement value,
            Dictionary<string, JsonElement> files)
    {
        JsonElement atlas = Required(row: value, field: "atlas");
        string path = CompletedBoundPath(path: Text(row: atlas, field: "path"));
        JsonElement reference = files.Values.SingleOrDefault(predicate: row => StringComparer.Ordinal.Equals(
                x: Text(row: row, field: "path"), y: path));
        if (reference.ValueKind is not JsonValueKind.Object || Text(row: reference, field: "role") is not "runtime"
                || !StringComparer.Ordinal.Equals(x: Text(row: reference, field: "sha256"),
                        y: Text(row: atlas, field: "sha256")))
        {
            throw new FormatException(message: "contextual actual decoded source atlas unavailable " + path);
        }

        (int width, int height) = CompletedPngSize(path: ResourcePath(path: path));
        CompletedAtlas(value: atlas, width: width, height: height);
        return (width, height);
    }

    private static void CompletedThemeRegion(JsonElement value, JsonElement common, int index, int width, int height)
    {
        string id = index switch
        {
            0 => "common.s0.7837",
            1 => "common.s0.7900",
            2 => "common.s0.6925",
            3 => "common.s0.8151",
            _ => throw new FormatException(message: "unknown contextual theme index"),
        };
        string state = index switch { 0 => "normal", 1 => "hover", 2 => "pressed", _ => "disabled" };
        JsonElement source = Rows(root: common, field: "resources").SingleOrDefault(predicate: row =>
                StringComparer.Ordinal.Equals(x: Text(row: row, field: "id"), y: id));
        if (source.ValueKind is not JsonValueKind.Object || !StringComparer.Ordinal.Equals(
                x: Text(row: value, field: "id"), y: id) || !StringComparer.Ordinal.Equals(
                        x: Text(row: value, field: "state"), y: state)
                || !StringComparer.Ordinal.Equals(x: Text(row: value, field: "member"),
                        y: Text(row: source, field: "member"))
                || !StringComparer.Ordinal.Equals(x: Text(row: value, field: "pngSha256"),
                        y: Text(row: source, field: "pngSha256"))
                || !StringComparer.Ordinal.Equals(x: Text(row: value, field: "rgbaSha256"),
                        y: Text(row: Required(row: source, field: "image"), field: "rgbaSha256")))
        {
            throw new FormatException(message: "contextual /theme/frames/"
                    + index.ToString(provider: CultureInfo.InvariantCulture)
                    + " source identity/member/PNG/RGBA mismatch");
        }

        CompletedSameRegion(value: value, source: source, width: width, height: height,
                pointer: "/theme/frames/" + index.ToString(provider: CultureInfo.InvariantCulture));
    }

    private static (int Width, int Height) CompletedItemRegion(JsonElement value, int index,
            Dictionary<string, JsonElement> files, Dictionary<string, JsonElement> documents)
    {
        string id = "IT-" + (index + 1).ToString(format: "D2", provider: CultureInfo.InvariantCulture);
        string pack = index < 10 ? "content/packs/items-01-10.json" : "content/packs/items-11-24.json";
        JsonElement document = CompletedSourceDocument(path: pack, files: files, documents: documents);
        JsonElement item = Rows(root: document, field: "items").SingleOrDefault(predicate: row =>
                StringComparer.Ordinal.Equals(x: Text(row: row, field: "id"), y: id));
        string key = item.ValueKind is JsonValueKind.Object ? Text(row: item, field: "canonicalIcon") : "invalid";
        JsonElement part = Rows(root: document, field: "parts").SingleOrDefault(predicate: row =>
                StringComparer.Ordinal.Equals(x: Text(row: row, field: "key"), y: key));
        JsonElement atlas = Required(row: document, field: "atlas");
        if (part.ValueKind is not JsonValueKind.Object || !StringComparer.Ordinal.Equals(
                x: Text(row: value, field: "id"), y: id) || !StringComparer.Ordinal.Equals(
                        x: Text(row: value, field: "icon"), y: key)
                || !StringComparer.Ordinal.Equals(x: CompletedBoundPath(path: Text(row: value, field: "texture")),
                        y: CompletedBoundPath(path: Text(row: atlas, field: "path")))
                || !StringComparer.Ordinal.Equals(x: Text(row: value, field: "rgbaSha256"),
                        y: Text(row: Required(row: part, field: "image"), field: "rgbaSha256")))
        {
            throw new FormatException(message: "contextual /items/"
                    + index.ToString(provider: CultureInfo.InvariantCulture)
                    + " canonical item identity/icon/texture/RGBA mismatch");
        }

        (int width, int height) = CompletedSourceExtent(value: document, files: files);
        CompletedSameRegion(value: value, source: part, width: width, height: height,
                pointer: "/items/" + index.ToString(provider: CultureInfo.InvariantCulture));
        return (width, height);
    }

    private static void CompletedSameRegion(JsonElement value, JsonElement source, int width, int height,
            string pointer)
    {
        int[] rectangle = CompletedContextRectangle(rectangle: Required(row: value, field: "atlasRect"),
                width: width, height: height, pointer: pointer + "/atlasRect");
        int[] expected = CompletedContextRectangle(rectangle: Required(row: source, field: "atlasRect"),
                width: width, height: height, pointer: pointer + "/source/atlasRect");
        if (!rectangle.SequenceEqual(second: expected))
        {
            throw new FormatException(message: "contextual " + pointer + " exact source atlas region mismatch");
        }
    }

    private static void CompletedDocument(string path, JsonElement value, JsonElement reference)
    {
        if (path is ExpectedPath)
        {
            return;
        }

        if (!StringComparer.Ordinal.Equals(x: Text(row: value, field: "id"),
                y: CompletedDocumentIdentity(path: path)))
        {
            throw new FormatException(message: "exact named completed document identity required");
        }

        if (value.TryGetProperty(propertyName: "schemaVersion", value: out _))
        {
            if (Integer(row: value, field: "schemaVersion") != 1)
            {
                throw new FormatException(message: "supported completed document schemaVersion1 required");
            }
        }
        else if (!CompletedSchemaOptional(path: path))
        {
            throw new FormatException(message: "missing completed document schemaVersion");
        }

        if (value.TryGetProperty(propertyName: "version", value: out _))
        {
            CompletedVersion(row: value);
            if (Integer(row: value, field: "version") != Integer(row: reference, field: "version"))
            {
                throw new FormatException(message: "document and publication content versions disagree");
            }
        }
        else if (!CompletedPublicationVersion(path: path))
        {
            throw new FormatException(message: "explicit document content version required");
        }

        CompletedKnownStructure(path: path, value: value);
    }

    private static bool CompletedSchemaOptional(string path)
    {
        return path switch
        {
            "content/config/combat.json" => true,
            "content/config/food-wildlife.json" => true,
            "content/config/inventory.json" => true,
            "content/config/items.json" => true,
            "content/config/jobs.json" => true,
            "content/config/local-preferences.json" => true,
            "content/config/map-sites.json" => true,
            "content/config/portals-assaults.json" => true,
            "content/config/presentation.json" => true,
            "content/config/run.json" => true,
            "content/config/session-save.json" => true,
            "content/config/structures.json" => true,
            "content/config/survivors.json" => true,
            "content/config/time.json" => true,
            "content/config/towers.json" => true,
            "content/config/zombie-loot.json" => true,
            "content/config/zombie-progression.json" => true,
            "content/config/zombies.json" => true,
            "content/schema/fields-economy.json" => true,
            "content/schema/fields-world.json" => true,
            "content/schema/fields-zombies.json" => true,
            "content/ui/legacy-stash-targets.json" => true,
            _ => false,
        };
    }

    private static void CompletedVersion(JsonElement row)
    {
        if (Integer(row: row, field: "version") is <= 0 or > int.MaxValue)
        {
            throw new FormatException(message: "positive bounded content version required");
        }
    }

    private static bool CompletedPublicationVersion(string path)
    {
        return path is "content/packs/ui-settings.json" or "content/ui/settings-errors.json"
                or "content/schema/fields-foundation.json" or "content/schema/fields-items-combat.json"
                or "content/ui/session-save-outcomes.json" or "content/fixtures/audio-states.json"
                or "content/ui/camp-panels.json" or "content/ui/map-spectator-warnings.json";
    }

    private static void CompletedKnownStructure(string path, JsonElement value)
    {
        string field = path switch
        {
            "content/schema/fields-foundation.json" or "content/schema/fields-items-combat.json" => "files",
            "content/fixtures/audio-states.json" => "states",
            "content/ui/settings-errors.json" or "content/ui/session-save-outcomes.json"
                    or "content/ui/camp-panels.json" or "content/ui/map-spectator-warnings.json"
                    or "content/ui/legacy-stash-targets.json" => "samples",
            _ => "",
        };
        if (field.Length > 0 && Rows(root: value, field: field).Length == 0)
        {
            throw new FormatException(message: "known document requires nonempty " + field);
        }

        if (path.StartsWith(value: "content/ui/", comparisonType: StringComparison.Ordinal)
                && field is "samples" && Required(row: value, field: "sampleDataOnly").ValueKind
                        is not JsonValueKind.True)
        {
            throw new FormatException(message: "known UI format requires sampleDataOnly");
        }
    }

    private static void CompletedRows(JsonElement root, Dictionary<string, JsonElement> files,
            Dictionary<string, JsonElement> documents, Dictionary<string, JsonElement> entries,
            Dictionary<string, string> expected, List<string> errors)
    {
        var closures = new Dictionary<string, HashSet<string>>(comparer: StringComparer.Ordinal);
        JsonElement[] outputs = Rows(root: root, field: "outputs");
        string[] duplicates = [.. outputs.GroupBy(keySelector: row => Identify(row: row, field: "id"),
                comparer: StringComparer.Ordinal).Where(predicate: group => group.Count() > 1)
                .Select(selector: group => group.Key)];
        foreach (JsonElement row in outputs.OrderBy(keySelector: row => Identify(row: row, field: "id"),
                comparer: StringComparer.Ordinal))
        {
            string id = Identify(row: row, field: "id");
            try
            {
                if (!expected.ContainsKey(key: id) || duplicates.Contains(value: id, comparer: StringComparer.Ordinal)
                        || !Mappings(entry: row).SequenceEqual(second: new[] { id }, comparer: StringComparer.Ordinal))
                {
                    throw new FormatException(message: "exact independent unique obligation identity required");
                }

                CompletedVersion(row: row);
                string file = Text(row: row, field: "file");
                if (!files.TryGetValue(key: file, value: out JsonElement reference)
                        || !documents.TryGetValue(key: file, value: out JsonElement definition))
                {
                    throw new FormatException(message: "actual named completed definition unavailable");
                }

                CompletedDefinition(row: row, definition: definition, reference: reference);
                CompletedDependencies(row: row, files: files, documents: documents, closures: closures);
                var detached = row.EnumerateObject().ToDictionary(keySelector: property => property.Name,
                        elementSelector: property => (object)property.Value.Clone(), comparer: StringComparer.Ordinal);
                detached["path"] = Text(row: reference, field: "path");
                detached["sha256"] = Text(row: reference, field: "sha256");
                detached["sourceStatus"] = "runtime-definition-validated; authoring source proof separate";
                entries.Add(key: id, value: JsonSerializer.SerializeToElement(value: detached));
            }
            catch (Exception error) when (error is FormatException or JsonException or DecoderFallbackException)
            {
                errors.Add(item: Diagnostic(id: id, path: "content/pack-index.json", field: "outputs",
                        value: row.GetRawText(), reason: error.Message));
            }
        }
    }

    private static void CompletedDependencies(JsonElement row, Dictionary<string, JsonElement> files,
            Dictionary<string, JsonElement> documents, Dictionary<string, HashSet<string>> closures)
    {
        JsonElement[] values = Rows(root: row, field: "dependencies");
        var ids = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement value in values)
        {
            string id = value.ValueKind is JsonValueKind.String ? value.GetString() ?? "" : "";
            if (!ids.Add(item: id) || !files.ContainsKey(key: id))
            {
                throw new FormatException(message: "duplicate or unavailable exact completed dependency");
            }
        }

        string shared = Text(row: row, field: "sharedFileId");
        if (!ids.Contains(item: Text(row: row, field: "file")) || !StringComparer.Ordinal.Equals(x: shared,
                y: "shared." + Text(row: row, field: "file")))
        {
            throw new FormatException(message: "explicit shared completed file identity required");
        }

        string primary = Text(row: row, field: "file");
        if (!closures.TryGetValue(key: primary, value: out HashSet<string>? closure))
        {
            closure = CompletedClosure(file: primary, files: files, documents: documents);
            closures.Add(key: primary, value: closure);
        }

        var required = new HashSet<string>(collection: closure, comparer: StringComparer.Ordinal);
        string owner = Text(row: row, field: "id");
        foreach (JsonElement file in files.Values)
        {
            if (CompletedLibrary(id: owner, file: file))
            {
                required.Add(item: Text(row: file, field: "id"));
            }
        }

        if (!ids.SetEquals(other: required) || !values.Select(selector: value => value.GetString() ?? "")
                .SequenceEqual(second: required.Order(comparer: StringComparer.Ordinal),
                        comparer: StringComparer.Ordinal))
        {
            throw new FormatException(message: "exact ordered actual source dependency closure required");
        }

        JsonElement acceptance = Required(row: row, field: "acceptance");
        if (Required(row: acceptance, field: "accepted").ValueKind is not JsonValueKind.False
                || Text(row: acceptance, field: "native") is not "pending"
                || Text(row: acceptance, field: "judgment") is not "pending"
                || Text(row: acceptance, field: "listening") is not "pending" and not "not-applicable")
        {
            throw new FormatException(message: "source model cannot confer future acceptance");
        }
    }

    private static HashSet<string> CompletedClosure(string file, Dictionary<string, JsonElement> files,
            Dictionary<string, JsonElement> documents)
    {
        var required = new HashSet<string>(comparer: StringComparer.Ordinal);
        var pending = new Queue<string>();
        var paths = files.Values.ToDictionary(keySelector: row => Text(row: row, field: "path"),
                elementSelector: row => Text(row: row, field: "id"), comparer: StringComparer.Ordinal);
        pending.Enqueue(item: file);
        while (pending.TryDequeue(result: out string? current))
        {
            if (!required.Add(item: current) || !documents.TryGetValue(key: current, value: out JsonElement value))
            {
                continue;
            }

            var references = new HashSet<string>(comparer: StringComparer.Ordinal);
            CompletedReferencedFiles(value: value, paths: paths, references: references);
            foreach (string reference in references)
            {
                pending.Enqueue(item: reference);
            }
        }

        return required;
    }

    private static void CompletedReferencedFiles(JsonElement value, Dictionary<string, string> paths,
            HashSet<string> references)
    {
        if (value.ValueKind is JsonValueKind.String)
        {
            string path = CompletedBoundPath(path: value.GetString() ?? "");
            if (!path.StartsWith(value: "src/godot-game/", comparisonType: StringComparison.Ordinal)
                    && !path.StartsWith(value: "production/phase4/", comparisonType: StringComparison.Ordinal)
                    && !path.StartsWith(value: "docs/content/", comparisonType: StringComparison.Ordinal)
                    && !path.StartsWith(value: "tools/phase4/", comparisonType: StringComparison.Ordinal))
            {
                return;
            }

            if (paths.TryGetValue(key: path, value: out string? id))
            {
                references.Add(item: id);
            }
            else
            {
                foreach ((string candidate, string identity) in paths)
                {
                    if (path.StartsWith(value: candidate + "/", comparisonType: StringComparison.Ordinal)
                            || path.StartsWith(value: candidate + "#", comparisonType: StringComparison.Ordinal))
                    {
                        references.Add(item: identity);
                    }
                }
            }
        }
        else if (value.ValueKind is JsonValueKind.Object)
        {
            foreach (JsonProperty child in value.EnumerateObject())
            {
                CompletedReferencedFiles(value: child.Value, paths: paths, references: references);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            foreach (JsonElement child in value.EnumerateArray())
            {
                CompletedReferencedFiles(value: child, paths: paths, references: references);
            }
        }
    }

    private static bool CompletedLibrary(string id, JsonElement file)
    {
        string path = Text(row: file, field: "path");
        return id switch
        {
            "DT-19" => Text(row: file, field: "role") is not "runtime",
            "DT-20" => path.Contains(value: "/animation/", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/bodies-", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/corpses-wildlife", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/guards", comparisonType: StringComparison.Ordinal),
            "DT-21" => path.Contains(value: "/world/", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/main-buildings", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/defenses", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/guards", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/camp-access", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/world", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/effects-portals", comparisonType: StringComparison.Ordinal),
            "DT-22" => path.Contains(value: "/ui/", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/ui-", comparisonType: StringComparison.Ordinal),
            "DT-23" => path.Contains(value: "/audio/", comparisonType: StringComparison.Ordinal)
                    || path.Contains(value: "/packs/audio-", comparisonType: StringComparison.Ordinal),
            _ => false,
        };
    }

    private static void CompletedDefinition(JsonElement row, JsonElement definition, JsonElement reference)
    {
        if (Integer(row: row, field: "version") != Integer(row: reference, field: "version"))
        {
            throw new FormatException(message: "definition and publication content versions disagree");
        }

        string id = Text(row: row, field: "id"), contract = Text(row: row, field: "contract");
        string path = CompletedPrimary(id: id), kind = CompletedKind(id: id);
        if (!StringComparer.Ordinal.Equals(x: Text(row: reference, field: "path"), y: "src/godot-game/" + path)
                || !StringComparer.Ordinal.Equals(x: contract, y: kind))
        {
            throw new FormatException(message: "exact typed obligation source contract required");
        }

        JsonElement[] pointers = Rows(root: row, field: "definitionPointers");
        if (!pointers.Select(selector: pointer => pointer.ValueKind is JsonValueKind.String
                ? pointer.GetString() ?? "invalid" : "invalid").SequenceEqual(
                        second: CompletedSelectors(id: id), comparer: StringComparer.Ordinal))
        {
            throw new FormatException(message: "exact independent ordered definition selectors required");
        }

        foreach (JsonElement pointer in pointers)
        {
            JsonElement selected = CompletedPointer(value: definition, pointer: pointer.GetString() ?? "invalid");
            if (selected.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
                    || selected.ValueKind is JsonValueKind.Array && selected.GetArrayLength() == 0)
            {
                throw new FormatException(message: "actual nonempty completed resource required");
            }

            CompletedSelectedIdentity(pointer: pointer.GetString() ?? "invalid",
                    selected: selected, id: id);
            if (contract is "body-parts-v1" or "item-parts-v1" or "structure-parts-v1"
                    && !StringComparer.Ordinal.Equals(x: Text(row: selected, field: "id"), y: id))
            {
                throw new FormatException(message: "actual selected body/item/tier identity mismatch");
            }
        }
    }

    private static void CompletedSelectedIdentity(string pointer, JsonElement selected, string id)
    {
        bool obligation = pointer.StartsWith(value: "/obligations/", comparisonType: StringComparison.Ordinal);
        if (!obligation)
        {
            return;
        }

        if (selected.ValueKind is not JsonValueKind.String)
        {
            throw new FormatException(message: "actual selected resource identity mismatch");
        }

        string actual = selected.GetString() ?? "";
        if (!StringComparer.Ordinal.Equals(x: actual, y: id))
        {
            throw new FormatException(message: "actual selected resource identity mismatch");
        }
    }

    private static string[] CompletedSelectors(string id)
    {
        return id.Split(separator: '-')[0] switch
        {
            "DT" => CompletedSelectorsDT(id: id),
            "AU" => CompletedSelectorsAU(id: id),
            "CH" => CompletedSelectorsCH(id: id),
            "BL" => CompletedSelectorsBL(id: id),
            "IT" => CompletedSelectorsIT(id: id),
            "TXT" => CompletedSelectorsTXT(id: id),
            "AN" => CompletedSelectorsAN(id: id),
            "EN" => CompletedSelectorsEN(id: id),
            "FX" => CompletedSelectorsFX(id: id),
            "UI" => CompletedSelectorsUI(id: id),
            _ => throw new FormatException(message: "unknown completed selector identity"),
        };
    }

    private static string[] CompletedSelectorsDT(string id)
    {
        return id switch
        {
            "DT-01" => [""],
            "DT-02" => [""],
            "DT-03" => [""],
            "DT-04" => [""],
            "DT-05" => [""],
            "DT-06" => [""],
            "DT-07" => [""],
            "DT-08" => [""],
            "DT-09" => [""],
            "DT-10" => [""],
            "DT-11" => [""],
            "DT-12" => [""],
            "DT-13" => [""],
            "DT-14" => [""],
            "DT-15" => [""],
            "DT-16" => [""],
            "DT-17" => [""],
            "DT-18" => ["/documentation", "/bindings"],
            "DT-19" => ["/fixtureCatalog", "/application"],
            "DT-20" => ["/clips", "/bindings"],
            "DT-21" => [""],
            "DT-22" => [""],
            "DT-23" => ["/events"],
            "DT-24" => ["/obligations/0"],
            "DT-25" => [""],
            "DT-26" => ["/fixtureCatalog", "/fixtureResources"],
            "DT-27" => ["/guides", "/application", "/platformRecords"],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static string[] CompletedSelectorsAU(string id)
    {
        return id switch
        {
            "AU-01" => ["/categories/0"],
            "AU-02" => ["/categories/1"],
            "AU-03" => ["/categories/2"],
            "AU-04" => ["/categories/3"],
            "AU-05" => ["/categories/4"],
            "AU-06" => ["/categories/5"],
            "AU-07" => ["/categories/6"],
            "AU-08" => ["/categories/7"],
            "AU-09" => ["/categories/8"],
            "AU-10" => ["/categories/9"],
            "AU-11" => ["/categories/10"],
            "AU-12" => ["/categories/11"],
            "AU-13" => ["/categories/12"],
            "AU-14" => ["/categories/13"],
            "AU-15" => ["/categories/14"],
            "AU-16" => ["/events"],
            "AU-17" => ["/events"],
            "AU-18" => ["/events"],
            "AU-19" => ["/events"],
            "AU-20" => ["/events"],
            "AU-21" => ["/events"],
            "AU-22" => ["/events"],
            "AU-23" => ["/events"],
            "AU-24" => ["/events"],
            "AU-25" => ["/events"],
            "AU-26" => ["/events"],
            "AU-27" => ["/events"],
            "AU-28" => ["/events"],
            "AU-29" => ["/events"],
            "AU-30" => ["/events"],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static string[] CompletedSelectorsCH(string id)
    {
        return id switch
        {
            "CH-01" => ["/bodies/0"],
            "CH-02" => ["/bodies/1"],
            "CH-03" => ["/bodies/2"],
            "CH-04" => ["/bodies/0"],
            "CH-05" => ["/bodies/1"],
            "CH-06" => ["/bodies/0"],
            "CH-07" => ["/bodies/1"],
            "CH-08" => ["/bodies/2"],
            "CH-09" => ["/obligations/0"],
            "CH-10" => ["/obligations/0"],
            "CH-11" => ["/obligations/1"],
            "CH-12" => ["/players", "/shapePolygons", "/controlTransfer"],
            "CH-13" => ["/obligations/2"],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static string[] CompletedSelectorsBL(string id)
    {
        return id switch
        {
            "BL-01" => ["/tiers/0"],
            "BL-02" => ["/tiers/1"],
            "BL-03" => ["/tiers/2"],
            "BL-04" => ["/tiers/0"],
            "BL-05" => ["/tiers/1"],
            "BL-06" => ["/tiers/2"],
            "BL-07" => ["/tiers/3"],
            "BL-08" => ["/tiers/4"],
            "BL-09" => ["/tiers/5"],
            "BL-10" => ["/walls", "/towers"],
            "BL-11" => ["/frames", "/parts"],
            "BL-12" => ["/frames", "/parts"],
            "BL-13" => ["/stash"],
            "BL-14" => ["/layouts"],
            "BL-15" => ["/obligations/0"],
            "BL-16" => ["/obligations/0"],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static string[] CompletedSelectorsIT(string id)
    {
        return id switch
        {
            "IT-01" => ["/items/0"],
            "IT-02" => ["/items/1"],
            "IT-03" => ["/items/2"],
            "IT-04" => ["/items/3"],
            "IT-05" => ["/items/4"],
            "IT-06" => ["/items/5"],
            "IT-07" => ["/items/6"],
            "IT-08" => ["/items/7"],
            "IT-09" => ["/items/8"],
            "IT-10" => ["/items/9"],
            "IT-11" => ["/items/0"],
            "IT-12" => ["/items/1"],
            "IT-13" => ["/items/2"],
            "IT-14" => ["/items/3"],
            "IT-15" => ["/items/4"],
            "IT-16" => ["/items/5"],
            "IT-17" => ["/items/6"],
            "IT-18" => ["/items/7"],
            "IT-19" => ["/items/8"],
            "IT-20" => ["/items/9"],
            "IT-21" => ["/items/10"],
            "IT-22" => ["/items/11"],
            "IT-23" => ["/items/12"],
            "IT-24" => ["/items/13"],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static string[] CompletedSelectorsTXT(string id)
    {
        return id switch
        {
            "TXT-01" => [""],
            "TXT-02" => [""],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static string[] CompletedSelectorsAN(string id)
    {
        return id switch
        {
            "AN-01" => ["/obligations/0"],
            "AN-02" => ["/obligations/1"],
            "AN-03" => ["/obligations/2"],
            "AN-04" => ["/obligations/3"],
            "AN-05" => ["/obligations/4"],
            "AN-06" => ["/obligations/5"],
            "AN-07" => ["/obligations/0"],
            "AN-08" => ["/obligations/1"],
            "AN-09" => ["/obligations/3"],
            "AN-10" => ["/obligations/0"],
            "AN-11" => ["/obligations/1"],
            "AN-12" => ["/obligations/2"],
            "AN-13" => ["/obligations/3"],
            "AN-14" => ["/obligations/4"],
            "AN-15" => ["/obligations/5"],
            "AN-16" => ["/obligations/6"],
            "AN-17" => ["/obligations/4"],
            "AN-18" => ["/obligations/1"],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static string[] CompletedSelectorsEN(string id)
    {
        return id switch
        {
            "EN-01" => ["/obligations/1"],
            "EN-02" => ["/obligations/2"],
            "EN-03" => ["/obligations/3"],
            "EN-04" => ["/obligations/4"],
            "EN-05" => ["/obligations/5"],
            "EN-06" => ["/obligations/6"],
            "EN-07" => ["/obligations/7"],
            "EN-08" => ["/obligations/8"],
            "EN-09" => ["/obligations/9"],
            "EN-10" => ["/obligations/10"],
            "EN-11" => ["/obligations/11"],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static string[] CompletedSelectorsFX(string id)
    {
        return id switch
        {
            "FX-01" => ["/obligations/1"],
            "FX-02" => ["/obligations/2"],
            "FX-03" => ["/obligations/3"],
            "FX-04" => ["/obligations/4"],
            "FX-05" => ["/obligations/5"],
            "FX-06" => ["/obligations/6"],
            "FX-07" => ["/obligations/7"],
            "FX-08" => ["/obligations/8"],
            "FX-09" => ["/obligations/9"],
            "FX-10" => ["/obligations/10"],
            "FX-11" => ["/obligations/11"],
            "FX-12" => ["/obligations/12"],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static string[] CompletedSelectorsUI(string id)
    {
        return id switch
        {
            "UI-01" => ["/obligations/0"],
            "UI-02" => ["/obligations/1"],
            "UI-03" => ["/obligations/0"],
            "UI-04" => ["/obligations/1"],
            "UI-05" => ["/obligations/0"],
            "UI-06" => ["/obligations/0"],
            "UI-07" => ["/obligations/2"],
            "UI-08" => ["/obligations/3"],
            "UI-09" => ["/obligations/0"],
            "UI-10" => ["/obligations/1"],
            "UI-11" => ["/obligations/2"],
            "UI-12" => ["/obligations/1"],
            "UI-13" => ["/obligations/2"],
            "UI-14" => ["/obligations/1"],
            "UI-15" => ["/obligations/2"],
            "UI-16" => ["/obligations/0"],
            "UI-17" => ["/obligations/1"],
            "UI-18" => ["/obligations/2"],
            "UI-19" => ["/obligations/3"],
            "UI-20" => ["/obligations/0"],
            "UI-21" => ["/obligations/1"],
            "UI-22" => ["/obligations/3"],
            "UI-23" => ["/obligations/4"],
            "UI-24" => ["/obligations/5"],
            "UI-25" => ["/obligations/0"],
            "UI-26" => ["/obligations/2"],
            "UI-27" => ["/obligations/1"],
            _ => throw new FormatException(message: "unknown finite completed selectors"),
        };
    }

    private static JsonElement CompletedPointer(JsonElement value, string pointer)
    {
        if (pointer.Length > 1024 || pointer.Length > 0 && !pointer.StartsWith(value: '/'))
        {
            throw new FormatException(message: "exact bounded JSON selector required");
        }

        foreach (string encoded in pointer.Split(separator: '/').Skip(count: 1))
        {
            string key = encoded.Replace(oldValue: "~1", newValue: "/", comparisonType: StringComparison.Ordinal)
                    .Replace(oldValue: "~0", newValue: "~", comparisonType: StringComparison.Ordinal);
            if (value.ValueKind is JsonValueKind.Array)
            {
                if (!int.TryParse(s: key, style: NumberStyles.None, provider: CultureInfo.InvariantCulture,
                        result: out int index) || index < 0 || index >= value.GetArrayLength())
                {
                    throw new FormatException(message: "actual definition array selector unavailable");
                }

                value = value[index];
            }
            else
            {
                value = Required(row: value, field: key);
            }
        }

        return value;
    }

    private static void CompletedIcon(byte[] bytes)
    {
        if (bytes.Length != 13342 || BinaryPrimitives.ReadUInt16LittleEndian(source: bytes.AsSpan(
                start: 0, length: 2)) != 0 || BinaryPrimitives.ReadUInt16LittleEndian(source: bytes.AsSpan(
                        start: 2, length: 2)) != 1 || BinaryPrimitives.ReadUInt16LittleEndian(source: bytes.AsSpan(
                                start: 4, length: 2)) != 7)
        {
            throw new FormatException(message: "exact ICO extent/type/seven-frame directory required");
        }

        int[] sizes = [16, 24, 32, 48, 64, 128, 256];
        long end = 118;
        for (int index = 0; index < sizes.Length; index++)
        {
            int position = 6 + index * 16, expected = sizes[index];
            int width = bytes[position] == 0 ? 256 : bytes[position];
            int height = bytes[position + 1] == 0 ? 256 : bytes[position + 1];
            uint count = BinaryPrimitives.ReadUInt32LittleEndian(source: bytes.AsSpan(
                    start: position + 8, length: 4));
            uint offset = BinaryPrimitives.ReadUInt32LittleEndian(source: bytes.AsSpan(
                    start: position + 12, length: 4));
            if (width != expected || height != expected || bytes[position + 2] != 0 || bytes[position + 3] != 0
                    || BinaryPrimitives.ReadUInt16LittleEndian(source: bytes.AsSpan(
                            start: position + 4, length: 2)) != 1
                    || BinaryPrimitives.ReadUInt16LittleEndian(source: bytes.AsSpan(
                            start: position + 6, length: 2)) != 32
                    || count < 33 || offset != end || (long)offset + count > bytes.Length)
            {
                throw new FormatException(message: "ICO frame identity/offset/length/overlap bound");
            }

            CompletedIconPng(bytes: bytes.AsSpan(start: (int)offset, length: (int)count), expected: expected);
            CompletedPng(bytes: bytes.AsSpan(start: (int)offset, length: (int)count).ToArray(),
                    width: expected, height: expected);
            end = (long)offset + count;
        }

        if (end != bytes.Length || !StringComparer.Ordinal.Equals(x: Digest(bytes: bytes),
                y: "5ca0170350b621993b7ed6c8d2e501360e677371b3f04df798092e2003b01c21"))
        {
            throw new FormatException(message: "exact retained ICO hash/extent required");
        }
    }

    private static void CompletedIconPng(ReadOnlySpan<byte> bytes, int expected)
    {
        if (bytes.Length < 45 || !bytes[..8].SequenceEqual(other:
                new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })
                || BinaryPrimitives.ReadUInt32BigEndian(source: bytes.Slice(start: 8, length: 4)) != 13
                || Encoding.ASCII.GetString(bytes: bytes.Slice(start: 12, length: 4)) is not "IHDR"
                || BinaryPrimitives.ReadUInt32BigEndian(source: bytes.Slice(start: 16, length: 4)) != expected
                || BinaryPrimitives.ReadUInt32BigEndian(source: bytes.Slice(start: 20, length: 4)) != expected
                || bytes[24] != 8 || bytes[25] != 6 || bytes[26] != 0 || bytes[27] != 0 || bytes[28] != 0)
        {
            throw new FormatException(message: "ICO PNG dimensions/format inconsistent with directory");
        }

        int position = 8;
        bool data = false, ended = false;
        while (position + 12 <= bytes.Length)
        {
            uint length = BinaryPrimitives.ReadUInt32BigEndian(source: bytes.Slice(start: position, length: 4));
            if (length > int.MaxValue || (long)position + 12 + length > bytes.Length)
            {
                throw new FormatException(message: "ICO PNG chunk extent bound");
            }

            string kind = Encoding.ASCII.GetString(bytes: bytes.Slice(start: position + 4, length: 4));
            uint crc = CompletedCrc(bytes: bytes.Slice(start: position + 4, length: (int)length + 4));
            if (crc != BinaryPrimitives.ReadUInt32BigEndian(source: bytes.Slice(
                    start: position + 8 + (int)length, length: 4)))
            {
                throw new FormatException(message: "ICO PNG chunk CRC mismatch");
            }

            data |= kind is "IDAT" && length > 0;
            position += 12 + (int)length;
            if (kind is "IEND")
            {
                ended = length == 0 && position == bytes.Length;
                break;
            }
        }

        if (!data || !ended)
        {
            throw new FormatException(message: "ICO PNG data/IEND extent required");
        }
    }

    private static uint CompletedCrc(ReadOnlySpan<byte> bytes)
    {
        uint crc = uint.MaxValue;
        foreach (byte value in bytes)
        {
            crc ^= value;
            for (int bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) != 0 ? 0xedb88320U ^ (crc >> 1) : crc >> 1;
            }
        }

        return crc ^ uint.MaxValue;
    }


    private static string CompletedMedia(string path, byte[] bytes)
    {
        if (path is "assets/ui/kingdom-zombies.ico")
        {
            CompletedIcon(bytes: bytes);
        }
        else if (path.EndsWith(value: ".png", comparisonType: StringComparison.Ordinal))
        {
            (int width, int height) = CompletedPngSize(path: path);
            return CompletedPng(bytes: bytes, width: width, height: height);
        }
        else if (path.EndsWith(value: ".wav", comparisonType: StringComparison.Ordinal))
        {
            CompletedWave(bytes: bytes);
        }
        else if (path.EndsWith(value: ".tres", comparisonType: StringComparison.Ordinal)
                || path.EndsWith(value: ".tscn", comparisonType: StringComparison.Ordinal))
        {
            int bound = path is "assets/ui/InspectionTheme.tres" ? 2097152 : MaxDocumentBytes;
            if (bytes.Length > bound || !new UTF8Encoding(encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true).GetString(bytes: bytes).StartsWith(value: "[gd_",
                            comparisonType: StringComparison.Ordinal))
            {
                throw new FormatException(message: "actual bounded native resource definition required");
            }
        }
        else if (path.EndsWith(value: ".ttf", comparisonType: StringComparison.Ordinal))
        {
            CompletedFont(bytes: bytes);
        }
        else
        {
            throw new FormatException(message: "unsupported named runtime media contract");
        }

        return "";
    }

    private static (int Width, int Height) CompletedPngSize(string path)
    {
        return path switch
        {
            "assets/buildings/defenses.png" or "assets/buildings/main-tiers.png" => (1024, 512),
            "assets/characters/bodies-a.png" => (768, 520),
            "assets/characters/bodies-b.png" => (768, 480),
            "assets/characters/bodies-c.png" => (768, 320),
            "assets/characters/identity-wildlife.png" => (512, 108),
            "assets/characters/structural-guard.png" or "assets/ui/identity-symbols.png" => (256, 64),
            "assets/characters/work-special.png" => (128, 64),
            "assets/common/bow.png" => (16, 18),
            "assets/common/donor-kit.png" => (2048, 2763),
            "assets/common/scale-pilot.png" => (512, 128),
            "assets/effects/effects.png" => (2048, 2048),
            "assets/equipment/items-01-10.png" or "assets/equipment/items-11-24.png" => (512, 256),
            "assets/ui/kingdom-zombies-icon.png" => (256, 256),
            "assets/world/environment.png" => (2048, 1024),
            _ => throw new FormatException(message: "unknown completed PNG identity"),
        };
    }

    private static string CompletedPng(byte[] bytes, int width, int height)
    {
        if (bytes.Length < 45 || !bytes.AsSpan(start: 0, length: 8).SequenceEqual(
                other: new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }))
        {
            throw new FormatException(message: "actual PNG signature/extent required");
        }

        using var compressed = new MemoryStream();
        int position = 8, chunks = 0;
        bool ended = false;
        while (position + 12 <= bytes.Length)
        {
            uint length = BinaryPrimitives.ReadUInt32BigEndian(source: bytes.AsSpan(start: position, length: 4));
            if (length > MaxResourceBytes || (long)position + 12 + length > bytes.Length)
            {
                throw new FormatException(message: "PNG chunk extent bound");
            }

            string kind = Encoding.ASCII.GetString(bytes: bytes, index: position + 4, count: 4);
            ReadOnlySpan<byte> payload = bytes.AsSpan(start: position + 8, length: (int)length);
            if (CompletedCrc(bytes: bytes.AsSpan(start: position + 4, length: (int)length + 4))
                    != BinaryPrimitives.ReadUInt32BigEndian(source: bytes.AsSpan(
                            start: position + 8 + (int)length, length: 4)))
            {
                throw new FormatException(message: "PNG chunk CRC mismatch");
            }

            CompletedPngChunk(kind: kind, payload: payload, chunks: chunks, width: width, height: height,
                    compressed: compressed);
            position += 12 + (int)length;
            chunks++;
            if (kind is "IEND")
            {
                ended = length == 0 && position == bytes.Length;
                break;
            }
        }

        if (!ended || compressed.Length < 6)
        {
            throw new FormatException(message: "PNG complete data/IEND required");
        }

        return CompletedPngPixels(compressed: compressed, width: width, height: height);
    }

    private static void CompletedPngChunk(string kind, ReadOnlySpan<byte> payload, int chunks,
            int width, int height, MemoryStream compressed)
    {
        if (chunks == 0)
        {
            if (kind is not "IHDR" || payload.Length != 13
                    || BinaryPrimitives.ReadUInt32BigEndian(source: payload[..4]) != width
                    || BinaryPrimitives.ReadUInt32BigEndian(source: payload.Slice(start: 4, length: 4)) != height
                    || payload[8] != 8 || payload[9] != 6 || payload[10] != 0 || payload[11] != 0 || payload[12] != 0)
            {
                throw new FormatException(message: "exact current RGBA8 PNG dimensions/header required");
            }
        }
        else if (kind is "IDAT" && payload.Length > 0)
        {
            if (compressed.Length + payload.Length > MaxResourceBytes)
            {
                throw new FormatException(message: "PNG encoded stream bound");
            }

            compressed.Write(buffer: payload);
        }
        else if (kind is not "IEND")
        {
            throw new FormatException(message: "unexpected current PNG chunk/order");
        }
    }

    private static string CompletedPngPixels(MemoryStream compressed, int width, int height)
    {
        compressed.Position = 0;
        try
        {
            using var decoder = new ZLibStream(stream: compressed, mode: CompressionMode.Decompress,
                    leaveOpen: true);
            using var pixels = IncrementalHash.CreateHash(hashAlgorithm: HashAlgorithmName.SHA256);
            byte[] row = new byte[width * 4 + 1], previous = new byte[width * 4];
            uint first = 1, second = 0;
            for (int index = 0; index < height; index++)
            {
                decoder.ReadExactly(buffer: row);
                foreach (byte value in row)
                {
                    first = (first + value) % 65521;
                    second = (second + first) % 65521;
                }

                CompletedPngRow(row: row, previous: previous);
                row.AsSpan(start: 1).CopyTo(destination: previous);
                pixels.AppendData(data: previous);
            }

            byte[] encoded = compressed.GetBuffer();
            uint expected = BinaryPrimitives.ReadUInt32BigEndian(source: encoded.AsSpan(
                    start: (int)compressed.Length - 4, length: 4));
            if (decoder.ReadByte() != -1 || expected != (second << 16 | first))
            {
                throw new FormatException(message: "PNG decoded extent/Adler mismatch");
            }

            return Convert.ToHexStringLower(inArray: pixels.GetHashAndReset());
        }
        catch (Exception error) when (error is IOException or InvalidDataException)
        {
            throw new FormatException(message: "PNG compressed pixel stream invalid", innerException: error);
        }
    }

    private static void CompletedPngRow(byte[] row, byte[] previous)
    {
        int filter = row[0];
        if (filter > 4)
        {
            throw new FormatException(message: "PNG row filter outside supported RGBA8 contract");
        }

        for (int index = 1; index < row.Length; index++)
        {
            int left = index > 4 ? row[index - 4] : 0, above = previous[index - 1];
            int corner = index > 4 ? previous[index - 5] : 0;
            int delta = filter switch
            {
                1 => left,
                2 => above,
                3 => (left + above) / 2,
                4 => CompletedPaeth(left: left, above: above, corner: corner),
                _ => 0,
            };
            row[index] = (byte)((row[index] + delta) & 255);
        }
    }

    private static int CompletedPaeth(int left, int above, int corner)
    {
        int prediction = left + above - corner;
        int a = Math.Abs(value: prediction - left), b = Math.Abs(value: prediction - above);
        int c = Math.Abs(value: prediction - corner);
        return a <= b && a <= c ? left : b <= c ? above : corner;
    }

    private static void CompletedWave(byte[] bytes)
    {
        if (bytes.Length < 44 || Encoding.ASCII.GetString(bytes: bytes, index: 0, count: 4) is not "RIFF"
                || Encoding.ASCII.GetString(bytes: bytes, index: 8, count: 4) is not "WAVE"
                || BinaryPrimitives.ReadUInt32LittleEndian(source: bytes.AsSpan(start: 4, length: 4))
                        != bytes.Length - 8)
        {
            throw new FormatException(message: "actual bounded WAV container/extent required");
        }

        int position = 12;
        bool format = false, data = false;
        while (position + 8 <= bytes.Length)
        {
            string kind = Encoding.ASCII.GetString(bytes: bytes, index: position, count: 4);
            uint length = BinaryPrimitives.ReadUInt32LittleEndian(source: bytes.AsSpan(
                    start: position + 4, length: 4));
            if (length > MaxResourceBytes || (long)position + 8 + length + (length & 1) > bytes.Length)
            {
                throw new FormatException(message: "WAV chunk extent bound");
            }

            ReadOnlySpan<byte> payload = bytes.AsSpan(start: position + 8, length: (int)length);
            if (kind is "fmt " && !format && !data)
            {
                CompletedWaveFormat(payload: payload);
                format = true;
            }
            else if (kind is "data" && format && !data && length > 0 && length % 4 == 0)
            {
                data = true;
            }
            else
            {
                throw new FormatException(message: "WAV duplicate/unknown/chunk ordering contract");
            }

            position += 8 + (int)length + ((int)length & 1);
        }

        if (!format || !data || position != bytes.Length)
        {
            throw new FormatException(message: "WAV complete PCM format/data required");
        }
    }

    private static void CompletedWaveFormat(ReadOnlySpan<byte> payload)
    {
        if (payload.Length != 16 || BinaryPrimitives.ReadUInt16LittleEndian(source: payload[..2]) != 1
                || BinaryPrimitives.ReadUInt16LittleEndian(source: payload.Slice(start: 2, length: 2)) != 2
                || BinaryPrimitives.ReadUInt32LittleEndian(source: payload.Slice(start: 4, length: 4)) != 44100
                || BinaryPrimitives.ReadUInt32LittleEndian(source: payload.Slice(start: 8, length: 4)) != 176400
                || BinaryPrimitives.ReadUInt16LittleEndian(source: payload.Slice(start: 12, length: 2)) != 4
                || BinaryPrimitives.ReadUInt16LittleEndian(source: payload.Slice(start: 14, length: 2)) != 16)
        {
            throw new FormatException(message: "actual PCM stereo16 44100Hz coherent WAV format required");
        }
    }

    private static void CompletedFont(byte[] bytes)
    {
        if (bytes.Length < 12 || BinaryPrimitives.ReadUInt32BigEndian(source: bytes.AsSpan(
                start: 0, length: 4)) != 0x00010000)
        {
            throw new FormatException(message: "actual TrueType sfnt required");
        }

        int count = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.AsSpan(start: 4, length: 2));
        if (count is < 1 or > 64 || 12 + count * 16 > bytes.Length)
        {
            throw new FormatException(message: "TrueType bounded directory required");
        }

        var tables = new Dictionary<string, ReadOnlyMemory<byte>>(comparer: StringComparer.Ordinal);
        var extents = new List<(long Start, long End)>();
        for (int index = 0; index < count; index++)
        {
            int position = 12 + index * 16;
            string tag = Encoding.ASCII.GetString(bytes: bytes, index: position, count: 4);
            uint offset = BinaryPrimitives.ReadUInt32BigEndian(source: bytes.AsSpan(
                    start: position + 8, length: 4));
            uint length = BinaryPrimitives.ReadUInt32BigEndian(source: bytes.AsSpan(
                    start: position + 12, length: 4));
            long end = (long)offset + length;
            if (tables.ContainsKey(key: tag) || offset < 12 + count * 16 || offset % 4 != 0 || length == 0
                    || end > bytes.Length || extents.Any(predicate: item => offset < item.End && end > item.Start))
            {
                throw new FormatException(message: "TrueType duplicate/table extent/overlap contract");
            }

            extents.Add(item: (offset, end));
            ReadOnlyMemory<byte> block = bytes.AsMemory(start: (int)offset, length: (int)length);
            uint expected = BinaryPrimitives.ReadUInt32BigEndian(source: bytes.AsSpan(
                    start: position + 4, length: 4));
            if (CompletedFontSum(bytes: block.Span, head: tag is "head") != expected)
            {
                throw new FormatException(message: "TrueType table checksum mismatch " + tag);
            }

            tables.Add(key: tag, value: block);
            CompletedFontTable(tag: tag, bytes: bytes.AsSpan(start: (int)offset, length: (int)length));
        }

        if (_completedRequiredFontTables.Any(predicate: tag => !tables.ContainsKey(key: tag)))
        {
            throw new FormatException(message: "TrueType required outline/metric/name/mapping tables missing");
        }

        if (CompletedFontSum(bytes: bytes, head: false) != 0xb1b0afba)
        {
            throw new FormatException(message: "TrueType whole-font checksum adjustment mismatch");
        }

        int glyphs = BinaryPrimitives.ReadUInt16BigEndian(source: tables["maxp"].Span.Slice(start: 4, length: 2));
        CompletedFontMetrics(tables: tables, glyphs: glyphs);
        CompletedFontMappings(bytes: tables["cmap"].Span, glyphs: glyphs);
        CompletedFontOutlines(tables: tables, glyphs: glyphs);
    }

    private static uint CompletedFontSum(ReadOnlySpan<byte> bytes, bool head)
    {
        uint sum = 0;
        for (int position = 0; position < bytes.Length; position += 4)
        {
            uint word = 0;
            for (int index = 0; index < 4; index++)
            {
                int offset = position + index;
                byte value = offset < bytes.Length && !(head && offset is >= 8 and < 12) ? bytes[offset] : (byte)0;
                word = word << 8 | value;
            }

            sum = unchecked(sum + word);
        }

        return sum;
    }

    private static void CompletedFontMetrics(Dictionary<string, ReadOnlyMemory<byte>> tables, int glyphs)
    {
        ReadOnlySpan<byte> hhea = tables["hhea"].Span, maxp = tables["maxp"].Span;
        if (hhea.Length < 36 || maxp.Length < 32
                || BinaryPrimitives.ReadUInt32BigEndian(source: hhea[..4]) != 0x00010000
                || BinaryPrimitives.ReadUInt32BigEndian(source: maxp[..4]) != 0x00010000)
        {
            throw new FormatException(message: "TrueType outline/metric table versions/extents required");
        }

        int count = BinaryPrimitives.ReadUInt16BigEndian(source: hhea.Slice(start: 34, length: 2));
        if (count <= 0 || count > glyphs || tables["hmtx"].Length < count * 4 + (glyphs - count) * 2)
        {
            throw new FormatException(message: "TrueType glyph/metric count relationship invalid");
        }
    }

    private static void CompletedFontMappings(ReadOnlySpan<byte> bytes, int glyphs)
    {
        int count = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: 2, length: 2));
        if (count <= 0 || 4L + count * 8 > bytes.Length)
        {
            throw new FormatException(message: "TrueType cmap encoding directory extent invalid");
        }

        var encodings = new HashSet<(int Platform, int Encoding)>();
        for (int index = 0; index < count; index++)
        {
            int position = 4 + index * 8;
            int platform = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: position, length: 2));
            int encoding = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: position + 2, length: 2));
            uint offset = BinaryPrimitives.ReadUInt32BigEndian(source: bytes.Slice(start: position + 4, length: 4));
            if (!encodings.Add(item: (platform, encoding)) || offset < 4 + count * 8
                    || (long)offset + 6 > bytes.Length)
            {
                throw new FormatException(message: "TrueType cmap duplicate encoding/subtable offset invalid");
            }

            int format = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: (int)offset, length: 2));
            int length = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: (int)offset + 2, length: 2));
            if (length < 10 || (long)offset + length > bytes.Length)
            {
                throw new FormatException(message: "TrueType cmap subtable length invalid");
            }

            ReadOnlySpan<byte> subtable = bytes.Slice(start: (int)offset, length: length);
            if (platform is 0 && encoding is 3 || platform is 3 && encoding is 1)
            {
                if (format != 4)
                {
                    throw new FormatException(message: "TrueType supported Unicode format4 required");
                }

                CompletedCmapSegments(bytes: subtable, glyphs: glyphs);
            }
            else if (platform is 1 && encoding is 0 && format is 6)
            {
                CompletedCmapTrimmed(bytes: subtable, glyphs: glyphs);
            }
            else
            {
                throw new FormatException(message: "unsupported current TrueType cmap encoding/format");
            }
        }

        if (!encodings.Contains(item: (0, 3)) || !encodings.Contains(item: (3, 1)))
        {
            throw new FormatException(message: "TrueType current Unicode mapping authority missing");
        }
    }

    private static void CompletedCmapSegments(ReadOnlySpan<byte> bytes, int glyphs)
    {
        if (bytes.Length < 24)
        {
            throw new FormatException(message: "TrueType cmap format4 header truncated");
        }

        int doubled = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: 6, length: 2));
        int count = doubled / 2;
        if (doubled == 0 || doubled % 2 != 0 || 16L + count * 8 > bytes.Length
                || BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: 14 + count * 2, length: 2)) != 0)
        {
            throw new FormatException(message: "TrueType cmap segment array extent/reserved value invalid");
        }

        int previous = -1;
        for (int index = 0; index < count; index++)
        {
            int end = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: 14 + index * 2, length: 2));
            int start = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(
                    start: 16 + count * 2 + index * 2, length: 2));
            int delta = BinaryPrimitives.ReadInt16BigEndian(source: bytes.Slice(
                    start: 16 + count * 4 + index * 2, length: 2));
            int rangePosition = 16 + count * 6 + index * 2;
            int range = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: rangePosition, length: 2));
            if (start > end || start <= previous || range % 2 != 0)
            {
                throw new FormatException(message: "TrueType cmap segment ordering/offset invalid");
            }

            for (int code = start; code <= end; code++)
            {
                int glyph = CompletedCmapGlyph(bytes: bytes, code: code, start: start, delta: delta,
                        position: rangePosition, range: range, arrayStart: 16 + count * 8);
                if (glyph >= glyphs)
                {
                    throw new FormatException(message: "TrueType cmap references nonexistent glyph");
                }
            }

            previous = end;
        }

        if (previous != ushort.MaxValue)
        {
            throw new FormatException(message: "TrueType cmap final sentinel missing");
        }
    }

    private static int CompletedCmapGlyph(ReadOnlySpan<byte> bytes, int code, int start, int delta,
            int position, int range, int arrayStart)
    {
        if (range == 0)
        {
            return (code + delta) & ushort.MaxValue;
        }

        int offset = position + range + (code - start) * 2;
        if (offset < arrayStart || offset + 2 > bytes.Length)
        {
            throw new FormatException(message: "TrueType cmap glyph-array reference outside subtable");
        }

        int glyph = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: offset, length: 2));
        return glyph == 0 ? 0 : (glyph + delta) & ushort.MaxValue;
    }

    private static void CompletedCmapTrimmed(ReadOnlySpan<byte> bytes, int glyphs)
    {
        int first = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: 6, length: 2));
        int count = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: 8, length: 2));
        if (10L + count * 2 != bytes.Length || (long)first + count > 65536)
        {
            throw new FormatException(message: "TrueType cmap format6 extent/range invalid");
        }

        for (int index = 0; index < count; index++)
        {
            if (BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(
                    start: 10 + index * 2, length: 2)) >= glyphs)
            {
                throw new FormatException(message: "TrueType cmap format6 references nonexistent glyph");
            }
        }
    }

    private static void CompletedFontOutlines(Dictionary<string, ReadOnlyMemory<byte>> tables, int glyphs)
    {
        short format = BinaryPrimitives.ReadInt16BigEndian(source: tables["head"].Span.Slice(start: 50, length: 2));
        int stride = format == 0 ? 2 : 4;
        if (format is not 0 and not 1 || tables["loca"].Length != (glyphs + 1) * stride)
        {
            throw new FormatException(message: "TrueType loca format/entry count invalid");
        }

        ReadOnlySpan<byte> loca = tables["loca"].Span, glyf = tables["glyf"].Span;
        var components = new Dictionary<int, HashSet<int>>();
        uint previous = 0;
        for (int index = 0; index <= glyphs; index++)
        {
            uint offset = format == 0 ? (uint)BinaryPrimitives.ReadUInt16BigEndian(source: loca.Slice(
                    start: index * 2, length: 2)) * 2 : BinaryPrimitives.ReadUInt32BigEndian(source: loca.Slice(
                            start: index * 4, length: 4));
            if (offset < previous || offset > glyf.Length || index == 0 && offset != 0)
            {
                throw new FormatException(message: "TrueType loca ordering/glyph extent invalid");
            }

            if (index > 0)
            {
                components.Add(key: index - 1, value: CompletedGlyph(bytes: glyf.Slice(
                        start: (int)previous, length: (int)(offset - previous)), glyphs: glyphs));
            }

            previous = offset;
        }

        CompletedGlyphGraph(components: components);
    }

    private static HashSet<int> CompletedGlyph(ReadOnlySpan<byte> bytes, int glyphs)
    {
        if (bytes.Length == 0)
        {
            return [];
        }

        if (bytes.Length < 10 || BinaryPrimitives.ReadInt16BigEndian(source: bytes.Slice(start: 2, length: 2))
                > BinaryPrimitives.ReadInt16BigEndian(source: bytes.Slice(start: 6, length: 2))
                || BinaryPrimitives.ReadInt16BigEndian(source: bytes.Slice(start: 4, length: 2))
                        > BinaryPrimitives.ReadInt16BigEndian(source: bytes.Slice(start: 8, length: 2)))
        {
            throw new FormatException(message: "TrueType glyph header/bounding box invalid");
        }

        short contours = BinaryPrimitives.ReadInt16BigEndian(source: bytes[..2]);
        if (contours >= 0)
        {
            CompletedSimpleGlyph(bytes: bytes, contours: contours);
            return [];
        }

        if (contours != -1)
        {
            throw new FormatException(message: "TrueType composite contour marker invalid");
        }

        return CompletedCompositeGlyph(bytes: bytes, glyphs: glyphs);
    }

    private static void CompletedSimpleGlyph(ReadOnlySpan<byte> bytes, int contours)
    {
        int position = 10 + contours * 2;
        if (position + 2 > bytes.Length)
        {
            if (contours == 0 && bytes.Length == 10)
            {
                return;
            }

            throw new FormatException(message: "TrueType simple glyph contour/instruction extent invalid");
        }

        int previous = -1;
        for (int index = 0; index < contours; index++)
        {
            int end = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: 10 + index * 2, length: 2));
            if (end <= previous)
            {
                throw new FormatException(message: "TrueType contour point order invalid");
            }

            previous = end;
        }

        int instructions = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: position, length: 2));
        position += 2 + instructions;
        int points = previous + 1;
        int coordinates = 0, covered = 0;
        while (covered < points && position < bytes.Length)
        {
            byte flags = bytes[position++];
            int repeat = 1;
            if ((flags & 8) != 0)
            {
                if (position >= bytes.Length)
                {
                    throw new FormatException(message: "TrueType glyph flag repeat truncated");
                }

                repeat += bytes[position++];
            }

            coordinates += repeat * (((flags & 2) != 0 ? 1 : (flags & 16) != 0 ? 0 : 2)
                    + ((flags & 4) != 0 ? 1 : (flags & 32) != 0 ? 0 : 2));
            covered += repeat;
        }

        if (covered != points || position > bytes.Length || position + coordinates > bytes.Length)
        {
            throw new FormatException(message: "TrueType glyph point/coordinate extent invalid");
        }
    }

    private static HashSet<int> CompletedCompositeGlyph(ReadOnlySpan<byte> bytes, int glyphs)
    {
        var components = new HashSet<int>();
        int position = 10;
        ushort flags;
        do
        {
            if (position + 4 > bytes.Length)
            {
                throw new FormatException(message: "TrueType composite glyph header truncated");
            }

            flags = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: position, length: 2));
            int glyph = BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: position + 2, length: 2));
            if (glyph >= glyphs || ((flags & 8) != 0 ? 1 : 0) + ((flags & 64) != 0 ? 1 : 0)
                    + ((flags & 128) != 0 ? 1 : 0) > 1)
            {
                throw new FormatException(message: "TrueType composite glyph reference/transform invalid");
            }

            components.Add(item: glyph);
            position += 4 + ((flags & 1) != 0 ? 4 : 2) + ((flags & 8) != 0 ? 2
                    : (flags & 64) != 0 ? 4 : (flags & 128) != 0 ? 8 : 0);
            if (position > bytes.Length)
            {
                throw new FormatException(message: "TrueType composite glyph argument extent invalid");
            }
        }
        while ((flags & 32) != 0);

        if ((flags & 256) != 0)
        {
            if (position + 2 > bytes.Length || position + 2 + BinaryPrimitives.ReadUInt16BigEndian(
                    source: bytes.Slice(start: position, length: 2)) > bytes.Length)
            {
                throw new FormatException(message: "TrueType composite glyph instruction extent invalid");
            }
        }

        return components;
    }

    private static void CompletedGlyphGraph(Dictionary<int, HashSet<int>> components)
    {
        var pending = new Queue<int>();
        var incoming = components.ToDictionary(keySelector: pair => pair.Key,
                elementSelector: pair => pair.Value.Count);
        var parents = components.Keys.ToDictionary(keySelector: key => key, elementSelector: _ => new List<int>());
        foreach ((int parent, HashSet<int> children) in components)
        {
            foreach (int child in children)
            {
                parents[child].Add(item: parent);
            }
        }

        foreach ((int glyph, int count) in incoming)
        {
            if (count == 0)
            {
                pending.Enqueue(item: glyph);
            }
        }

        int processed = 0;
        while (pending.TryDequeue(result: out int glyph))
        {
            processed++;
            foreach (int parent in parents[glyph])
            {
                incoming[parent]--;
                if (incoming[parent] == 0)
                {
                    pending.Enqueue(item: parent);
                }
            }
        }

        if (processed != components.Count)
        {
            throw new FormatException(message: "TrueType composite glyph dependency cycle");
        }
    }

    private static void CompletedFontTable(string tag, ReadOnlySpan<byte> bytes)
    {
        if (tag is "head" && (bytes.Length < 54 || BinaryPrimitives.ReadUInt32BigEndian(
                source: bytes.Slice(start: 12, length: 4)) != 0x5f0f3cf5
                || BinaryPrimitives.ReadUInt16BigEndian(source: bytes.Slice(start: 18, length: 2)) is < 16 or > 16384)
                || tag is "maxp" && (bytes.Length < 6 || BinaryPrimitives.ReadUInt16BigEndian(
                        source: bytes.Slice(start: 4, length: 2)) == 0)
                || tag is "cmap" && (bytes.Length < 12 || BinaryPrimitives.ReadUInt16BigEndian(
                        source: bytes[..2]) != 0 || BinaryPrimitives.ReadUInt16BigEndian(
                                source: bytes.Slice(start: 2, length: 2)) == 0))
        {
            throw new FormatException(message: "TrueType actual header/glyph/mapping semantics invalid");
        }
    }

    private static void CompletedNativeFiles(Dictionary<string, JsonElement> files,
            Dictionary<string, byte[]> native, List<string> errors)
    {
        foreach ((string id, byte[] bytes) in native)
        {
            JsonElement file = files[id];
            string path = Text(row: file, field: "path");
            try
            {
                Dictionary<string, string> sections = CompletedNativeSections(path: path, bytes: bytes);
                CompletedNativeReferences(path: path, sections: sections, files: files);
            }
            catch (Exception error) when (error is FormatException or JsonException or DecoderFallbackException)
            {
                files.Remove(key: id);
                errors.Add(item: Diagnostic(id: id, path: path, field: "native", value: id, reason: error.Message));
            }
        }
    }

    private static Dictionary<string, string> CompletedNativeSections(string path, byte[] bytes)
    {
        string text = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false,
                throwOnInvalidBytes: true).GetString(bytes: bytes);
        string[] lines = text.Split(separator: '\n');
        bool theme = path is "src/godot-game/assets/ui/InspectionTheme.tres";
        if (!Regex.IsMatch(input: lines[0], pattern: theme
                ? "^\\[gd_resource type=\"Theme\" .+format=3\\]$" : "^\\[gd_scene .+format=3\\]$",
                options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking))
        {
            throw new FormatException(message: "native exact current Theme/scene format3 header required");
        }

        var sections = new Dictionary<string, string>(comparer: StringComparer.Ordinal);
        var aliases = new HashSet<string>(comparer: StringComparer.OrdinalIgnoreCase);
        string key = "header";
        var block = new StringBuilder();
        foreach (string line in lines)
        {
            if (line.StartsWith(value: '[') && block.Length > 0)
            {
                sections.Add(key: key, value: block.ToString());
                block.Clear();
            }

            if (line.StartsWith(value: '['))
            {
                key = CompletedNativeSectionKey(line: line, sections: sections, theme: theme);
                if (!aliases.Add(item: key) || sections.Count >= 1024)
                {
                    throw new FormatException(message: "native duplicate/case-alias section or1024section bound");
                }
            }

            block.Append(value: line).Append(value: '\n');
        }

        if (block.Length > 0)
        {
            sections.Add(key: key, value: block.ToString());
        }

        return sections;
    }

    private static string CompletedNativeSectionKey(string line, Dictionary<string, string> sections, bool theme)
    {
        if (line.StartsWith(value: "[gd_", comparisonType: StringComparison.Ordinal))
        {
            return "header";
        }

        if (line.StartsWith(value: "[ext_resource ", comparisonType: StringComparison.Ordinal)
                || line.StartsWith(value: "[sub_resource ", comparisonType: StringComparison.Ordinal))
        {
            string kind = line.StartsWith(value: "[ext_", comparisonType: StringComparison.Ordinal) ? "ext:" : "sub:";
            return kind + CompletedNativeAttribute(line: line, name: "id", required: true);
        }

        if (!theme && line.StartsWith(value: "[node ", comparisonType: StringComparison.Ordinal))
        {
            string name = CompletedNativeAttribute(line: line, name: "name", required: true);
            string parent = CompletedNativeAttribute(line: line, name: "parent", required: false);
            if (name.Contains(value: '/') || parent.Contains(value: "..", comparisonType: StringComparison.Ordinal)
                    || parent.Length > 0 && !sections.ContainsKey(key: "node:" + parent))
            {
                throw new FormatException(message: "native node parent/path unavailable or unsafe");
            }

            return "node:" + (parent.Length == 0 ? "." : parent is "." ? name : parent + "/" + name);
        }

        if (!theme && line.StartsWith(value: "[connection ", comparisonType: StringComparison.Ordinal))
        {
            return "connection:" + sections.Count.ToString(provider: CultureInfo.InvariantCulture);
        }

        if (theme && line is "[resource]")
        {
            return "resource";
        }

        throw new FormatException(message: "unsupported current native section");
    }

    private static string CompletedNativeAttribute(string line, string name, bool required)
    {
        MatchCollection matches = Regex.Matches(input: line, pattern: "(?:^| )" + Regex.Escape(str: name)
                + "=\"(?<value>[^\"]*)\"", options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
        if (matches.Count == 0 && !required)
        {
            return "";
        }

        if (matches.Count != 1 || matches[0].Groups["value"].Value.Length is 0 or > 240
                || matches[0].Groups["value"].Value.Any(predicate: character => char.IsControl(c: character)))
        {
            throw new FormatException(message: "native unique bounded attribute required " + name);
        }

        return matches[0].Groups["value"].Value;
    }

    private static void CompletedNativeReferences(string path, Dictionary<string, string> sections,
            Dictionary<string, JsonElement> files)
    {
        var graph = new Dictionary<string, HashSet<string>>(comparer: StringComparer.Ordinal);
        foreach ((string key, string block) in sections)
        {
            var targets = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (Match match in Regex.Matches(input: block,
                    pattern: "(?<kind>Ext|Sub)Resource\\(\"(?<id>[^\"]+)\"\\)",
                    options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking))
            {
                string target = (match.Groups["kind"].Value is "Ext" ? "ext:" : "sub:") + match.Groups["id"].Value;
                if (!sections.ContainsKey(key: target))
                {
                    throw new FormatException(message: "native resource reference unavailable " + target);
                }

                targets.Add(item: target);
            }

            graph.Add(key: key, value: targets);
            if (key.StartsWith(value: "ext:", comparisonType: StringComparison.Ordinal))
            {
                CompletedNativeExternal(path: path, block: block, files: files);
            }
            else if (key.StartsWith(value: "sub:", comparisonType: StringComparison.Ordinal))
            {
                CompletedNativeResource(key: key, block: block, sections: sections, files: files);
            }
            else if (key.StartsWith(value: "connection:", comparisonType: StringComparison.Ordinal))
            {
                CompletedNativeConnection(block: block, sections: sections);
            }
            else if (key.StartsWith(value: "node:", comparisonType: StringComparison.Ordinal))
            {
                CompletedNativeNode(block: block, sections: sections);
            }
            else if (key is "resource")
            {
                CompletedNativeTheme(block: block, sections: sections);
            }
        }

        CompletedNativeGraph(graph: graph);
    }

    private static void CompletedNativeExternal(string path, string block, Dictionary<string, JsonElement> files)
    {
        string header = block.Split(separator: '\n')[0];
        string resource = CompletedNativeAttribute(line: header, name: "path", required: true);
        string expected = path switch
        {
            "src/godot-game/assets/ui/InspectionTheme.tres" => "res://assets/common/donor-kit.png",
            "src/godot-game/inspection/ItemsWeaponsPreview.tscn" => "res://assets/equipment/items-01-10.png",
            "src/godot-game/inspection/ItemsWearablesPreview.tscn" => "res://assets/equipment/items-11-24.png",
            _ => throw new FormatException(message: "unknown native source contract"),
        };
        if (!StringComparer.Ordinal.Equals(x: resource, y: expected)
                || CompletedNativeAttribute(line: header, name: "type", required: true) is not "Texture2D")
        {
            throw new FormatException(message: "native exact source texture/type mismatch " + resource);
        }

        CompletedNativeFile(path: resource, files: files);
    }

    private static JsonElement CompletedNativeFile(string path, Dictionary<string, JsonElement> files)
    {
        string source = CompletedBoundPath(path: path);
        JsonElement file = files.Values.SingleOrDefault(predicate: row => StringComparer.Ordinal.Equals(
                x: Text(row: row, field: "path"), y: source));
        if (file.ValueKind is not JsonValueKind.Object || Text(row: file, field: "role") is not "runtime")
        {
            throw new FormatException(message: "native authenticated runtime dependency unavailable " + source);
        }

        return file;
    }

    private static void CompletedNativeResource(string key, string block, Dictionary<string, string> sections,
            Dictionary<string, JsonElement> files)
    {
        string type = CompletedNativeAttribute(line: block.Split(separator: '\n')[0], name: "type", required: true);
        CompletedNativeUseTypes(type: type, block: block, sections: sections);
        if (type is "FontFile")
        {
            CompletedNativeFont(key: key, block: block, files: files);
        }
        else if (type is "AtlasTexture" or "StyleBoxTexture")
        {
            string field = type is "AtlasTexture" ? "atlas" : "texture";
            string rectangle = type is "AtlasTexture" ? "region" : "region_rect";
            string reference = CompletedNativeProperty(block: block, name: field);
            Match match = Regex.Match(input: reference, pattern: "^ExtResource\\(\"(?<id>[^\"]+)\"\\)$",
                    options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
            if (!match.Success || !sections.TryGetValue(key: "ext:" + match.Groups["id"].Value,
                    value: out string? external))
            {
                throw new FormatException(message: "native texture reference type/identity mismatch");
            }

            string path = CompletedNativeAttribute(line: external.Split(separator: '\n')[0],
                    name: "path", required: true);
            (int width, int height) = CompletedPngSize(path: ResourcePath(path: CompletedBoundPath(path: path)));
            CompletedNativeRectangle(text: CompletedNativeProperty(block: block, name: rectangle),
                    width: width, height: height);
        }
        else if (type is not "FontVariation" and not "StyleBoxFlat" and not "Animation" and not "AnimationLibrary")
        {
            throw new FormatException(message: "unsupported current native resource type " + type);
        }
    }

    private static void CompletedNativeUseTypes(string type, string block, Dictionary<string, string> sections)
    {
        foreach (Match match in Regex.Matches(input: block, pattern: "SubResource\\(\"(?<id>[^\"]+)\"\\)",
                options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking))
        {
            if (!sections.TryGetValue(key: "sub:" + match.Groups["id"].Value, value: out string? target))
            {
                throw new FormatException(message: "native typed resource target unavailable");
            }

            string actual = CompletedNativeAttribute(line: target.Split(separator: '\n')[0],
                    name: "type", required: true);
            bool valid = type switch
            {
                "FontFile" or "FontVariation" => actual is "FontFile",
                "AnimationLibrary" => actual is "Animation",
                "Animation" => actual is "AtlasTexture",
                "AtlasTexture" or "StyleBoxTexture" or "StyleBoxFlat" => false,
                _ => false,
            };
            if (!valid)
            {
                throw new FormatException(message: "native resource reference type mismatch " + type + " -> " + actual);
            }
        }
    }

    private static string CompletedNativeProperty(string block, string name)
    {
        string[] lines = [.. block.Split(separator: '\n').Where(predicate: line => line.StartsWith(
                value: name + " = ", comparisonType: StringComparison.Ordinal))];
        if (lines.Length != 1)
        {
            throw new FormatException(message: "native exact property required " + name);
        }

        return lines[0][(name.Length + 3)..];
    }

    private static void CompletedNativeRectangle(string text, int width, int height)
    {
        Match match = Regex.Match(input: text,
                pattern: "^Rect2\\((?<x>[0-9]+), (?<y>[0-9]+), (?<width>[0-9]+), (?<height>[0-9]+)\\)$",
                options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
        if (!match.Success || !int.TryParse(s: match.Groups["x"].Value, provider: CultureInfo.InvariantCulture,
                result: out int x) || !int.TryParse(s: match.Groups["y"].Value, provider: CultureInfo.InvariantCulture,
                        result: out int y) || !int.TryParse(s: match.Groups["width"].Value,
                                provider: CultureInfo.InvariantCulture, result: out int w)
                || !int.TryParse(s: match.Groups["height"].Value,
                        provider: CultureInfo.InvariantCulture, result: out int h)
                || w <= 0 || h <= 0 || (long)x + w > width || (long)y + h > height)
        {
            throw new FormatException(message: "native bounded integer source texture region required");
        }
    }

    private static void CompletedNativeFont(string key, string block, Dictionary<string, JsonElement> files)
    {
        string expected = key switch
        {
            "sub:Fallback" => "res://assets/ui/Fallback.ttf",
            "sub:Body" => "res://assets/ui/LiberationSans.ttf",
            "sub:Pixel" => "res://assets/ui/PerfectDOSVGA437.ttf",
            _ => throw new FormatException(message: "native unsupported embedded-font role"),
        };
        string path = JsonSerializer.Deserialize<string>(json: CompletedNativeProperty(block: block,
                name: "metadata/source_path")) ?? "invalid";
        if (!StringComparer.Ordinal.Equals(x: path, y: expected))
        {
            throw new FormatException(message: "native embedded-font source role mismatch");
        }

        JsonElement file = CompletedNativeFile(path: path, files: files);
        string packed = CompletedNativeProperty(block: block, name: "data");
        const string Prefix = "PackedByteArray(\"";
        if (!packed.StartsWith(value: Prefix, comparisonType: StringComparison.Ordinal)
                || !packed.EndsWith(value: "\")", comparisonType: StringComparison.Ordinal))
        {
            throw new FormatException(message: "native embedded font byte encoding invalid");
        }

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(s: packed[Prefix.Length..^2]);
        }
        catch (FormatException error)
        {
            throw new FormatException(message: "native embedded font base64 invalid", innerException: error);
        }

        if (bytes.Length != Integer(row: file, field: "bytes") || !StringComparer.Ordinal.Equals(
                x: Digest(bytes: bytes), y: Text(row: file, field: "sha256")))
        {
            throw new FormatException(message: "native actual embedded/source font bytes mismatch");
        }
    }

    private static void CompletedNativeConnection(string block, Dictionary<string, string> sections)
    {
        string header = block.Split(separator: '\n')[0];
        string from = CompletedNativeAttribute(line: header, name: "from", required: true);
        string target = CompletedNativeAttribute(line: header, name: "to", required: true);
        if (!sections.TryGetValue(key: "node:" + from, value: out string? sender)
                || CompletedNativeAttribute(line: sender.Split(separator: '\n')[0],
                        name: "type", required: true) is not "Button"
                || !sections.TryGetValue(key: "node:" + target, value: out string? receiver)
                || CompletedNativeAttribute(line: header, name: "signal", required: true) is not "pressed"
                || CompletedNativeAttribute(line: header, name: "method", required: true) is not "play"
                || CompletedNativeAttribute(line: receiver.Split(separator: '\n')[0],
                        name: "type", required: true) is not "AnimationPlayer")
        {
            throw new FormatException(message: "native connection source/target/signal/method contract invalid");
        }
    }

    private static void CompletedNativeNode(string block, Dictionary<string, string> sections)
    {
        string type = CompletedNativeAttribute(line: block.Split(separator: '\n')[0], name: "type", required: true);
        if (type is not "AnimationPlayer" and not "Button" and not "Control" and not "HBoxContainer"
                and not "Label" and not "Node2D" and not "ScrollContainer" and not "Sprite2D"
                and not "VBoxContainer")
        {
            throw new FormatException(message: "unsupported current native node type " + type);
        }

        foreach (Match match in Regex.Matches(input: block, pattern: "SubResource\\(\"(?<id>[^\"]+)\"\\)",
                options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking))
        {
            string resource = sections["sub:" + match.Groups["id"].Value];
            string actual = CompletedNativeAttribute(line: resource.Split(separator: '\n')[0],
                    name: "type", required: true);
            if (!(type is "AnimationPlayer" && actual is "AnimationLibrary")
                    && !(type is "Sprite2D" && actual is "AtlasTexture"))
            {
                throw new FormatException(message: "native node resource type mismatch " + type + " -> " + actual);
            }
        }
    }

    private static void CompletedNativeTheme(string block, Dictionary<string, string> sections)
    {
        foreach (string line in block.Split(separator: '\n'))
        {
            foreach (Match match in Regex.Matches(input: line, pattern: "SubResource\\(\"(?<id>[^\"]+)\"\\)",
                    options: RegexOptions.CultureInvariant | RegexOptions.NonBacktracking))
            {
                string target = sections["sub:" + match.Groups["id"].Value];
                string type = CompletedNativeAttribute(line: target.Split(separator: '\n')[0],
                        name: "type", required: true);
                bool font = line.StartsWith(value: "default_font =", comparisonType: StringComparison.Ordinal)
                        || line.Contains(value: "/fonts/", comparisonType: StringComparison.Ordinal);
                bool style = line.Contains(value: "/styles/", comparisonType: StringComparison.Ordinal);
                if (!(font && type is "FontFile" or "FontVariation")
                        && !(style && type is "StyleBoxFlat" or "StyleBoxTexture"))
                {
                    throw new FormatException(message: "native Theme property/resource type mismatch");
                }
            }
        }
    }

    private static void CompletedNativeGraph(Dictionary<string, HashSet<string>> graph)
    {
        var counts = graph.ToDictionary(keySelector: pair => pair.Key, elementSelector: pair => pair.Value.Count,
                comparer: StringComparer.Ordinal);
        var parents = graph.Keys.ToDictionary(keySelector: key => key, elementSelector: _ => new List<string>(),
                comparer: StringComparer.Ordinal);
        foreach ((string parent, HashSet<string> children) in graph)
        {
            foreach (string child in children)
            {
                parents[child].Add(item: parent);
            }
        }

        var ready = new Queue<string>(collection: counts.Where(predicate: pair => pair.Value == 0)
                .Select(selector: pair => pair.Key));
        int processed = 0;
        while (ready.TryDequeue(result: out string? node))
        {
            processed++;
            foreach (string parent in parents[node])
            {
                counts[parent]--;
                if (counts[parent] == 0)
                {
                    ready.Enqueue(item: parent);
                }
            }
        }

        if (processed != graph.Count)
        {
            throw new FormatException(message: "native resource dependency cycle");
        }
    }

    private static string CompletedPrimary(string id)
    {
        return id.Split(separator: '-')[0] switch
        {
            "CH" or "AN" or "IT" => CompletedPrimaryActors(id: id),
            "BL" or "EN" or "FX" => CompletedPrimaryWorld(id: id),
            "UI" => CompletedPrimaryUi(id: id),
            "AU" => CompletedPrimaryAudio(id: id),
            "DT" or "TXT" => CompletedPrimaryData(id: id),
            _ => throw new FormatException(message: "unknown completed obligation domain"),
        };
    }

    private static string CompletedPrimaryActors(string id)
    {
        return id switch
        {
            "CH-01" => "content/packs/bodies-a.json",
            "CH-02" => "content/packs/bodies-a.json",
            "CH-03" => "content/packs/bodies-a.json",
            "CH-04" => "content/packs/bodies-b.json",
            "CH-05" => "content/packs/bodies-b.json",
            "CH-08" => "content/packs/bodies-b.json",
            "CH-06" => "content/packs/bodies-c.json",
            "CH-07" => "content/packs/bodies-c.json",
            "IT-01" => "content/packs/items-01-10.json",
            "IT-02" => "content/packs/items-01-10.json",
            "IT-03" => "content/packs/items-01-10.json",
            "IT-04" => "content/packs/items-01-10.json",
            "IT-05" => "content/packs/items-01-10.json",
            "IT-06" => "content/packs/items-01-10.json",
            "IT-07" => "content/packs/items-01-10.json",
            "IT-08" => "content/packs/items-01-10.json",
            "IT-09" => "content/packs/items-01-10.json",
            "IT-10" => "content/packs/items-01-10.json",
            "IT-11" => "content/packs/items-11-24.json",
            "IT-12" => "content/packs/items-11-24.json",
            "IT-13" => "content/packs/items-11-24.json",
            "IT-14" => "content/packs/items-11-24.json",
            "IT-15" => "content/packs/items-11-24.json",
            "IT-16" => "content/packs/items-11-24.json",
            "IT-17" => "content/packs/items-11-24.json",
            "IT-18" => "content/packs/items-11-24.json",
            "IT-19" => "content/packs/items-11-24.json",
            "IT-20" => "content/packs/items-11-24.json",
            "IT-21" => "content/packs/items-11-24.json",
            "IT-22" => "content/packs/items-11-24.json",
            "IT-23" => "content/packs/items-11-24.json",
            "IT-24" => "content/packs/items-11-24.json",
            "AN-01" => "content/animation/locomotion-melee.json",
            "AN-02" => "content/animation/locomotion-melee.json",
            "AN-03" => "content/animation/locomotion-melee.json",
            "AN-04" => "content/animation/locomotion-melee.json",
            "AN-05" => "content/animation/locomotion-melee.json",
            "AN-06" => "content/animation/locomotion-melee.json",
            "AN-07" => "content/animation/bow.json",
            "AN-08" => "content/animation/bow.json",
            "AN-10" => "content/animation/work-special.json",
            "AN-11" => "content/animation/work-special.json",
            "AN-12" => "content/animation/work-special.json",
            "AN-13" => "content/animation/work-special.json",
            "AN-14" => "content/animation/work-special.json",
            "AN-15" => "content/animation/work-special.json",
            "AN-16" => "content/animation/work-special.json",
            "CH-10" => "content/packs/corpses-wildlife.json",
            "CH-11" => "content/packs/corpses-wildlife.json",
            "CH-13" => "content/packs/corpses-wildlife.json",
            "AN-09" => "content/packs/corpses-wildlife.json",
            "AN-17" => "content/packs/corpses-wildlife.json",
            "CH-09" => "content/packs/guards.json",
            "AN-18" => "content/packs/guards.json",
            "CH-12" => "content/packs/player-identities.json",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedPrimaryWorld(string id)
    {
        return id switch
        {
            "BL-04" => "content/packs/defenses.json",
            "BL-05" => "content/packs/defenses.json",
            "BL-06" => "content/packs/defenses.json",
            "BL-07" => "content/packs/defenses.json",
            "BL-08" => "content/packs/defenses.json",
            "BL-09" => "content/packs/defenses.json",
            "BL-01" => "content/packs/main-buildings.json",
            "BL-02" => "content/packs/main-buildings.json",
            "BL-03" => "content/packs/main-buildings.json",
            "BL-15" => "content/packs/world.json",
            "EN-01" => "content/packs/world.json",
            "EN-02" => "content/packs/world.json",
            "EN-03" => "content/packs/world.json",
            "EN-04" => "content/packs/world.json",
            "EN-05" => "content/packs/world.json",
            "EN-06" => "content/packs/world.json",
            "EN-07" => "content/packs/world.json",
            "EN-08" => "content/packs/world.json",
            "EN-09" => "content/packs/world.json",
            "EN-10" => "content/packs/world.json",
            "EN-11" => "content/packs/world.json",
            "BL-16" => "content/packs/effects-portals.json",
            "FX-01" => "content/packs/effects-portals.json",
            "FX-02" => "content/packs/effects-portals.json",
            "FX-03" => "content/packs/effects-portals.json",
            "FX-04" => "content/packs/effects-portals.json",
            "FX-05" => "content/packs/effects-portals.json",
            "FX-06" => "content/packs/effects-portals.json",
            "FX-07" => "content/packs/effects-portals.json",
            "FX-08" => "content/packs/effects-portals.json",
            "FX-09" => "content/packs/effects-portals.json",
            "FX-10" => "content/packs/effects-portals.json",
            "FX-11" => "content/packs/effects-portals.json",
            "FX-12" => "content/packs/effects-portals.json",
            "BL-10" => "content/world/map.json",
            "BL-11" => "content/packs/defenses.json",
            "BL-12" => "content/packs/defenses.json",
            "BL-13" => "content/packs/camp-access.json",
            "BL-14" => "content/packs/camp-access.json",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedPrimaryUi(string id)
    {
        return id switch
        {
            "UI-01" => "content/packs/ui-kit.json",
            "UI-02" => "content/packs/ui-kit.json",
            "UI-03" => "content/packs/ui-identity.json",
            "UI-04" => "content/packs/ui-identity.json",
            "UI-07" => "content/packs/ui-identity.json",
            "UI-08" => "content/packs/ui-identity.json",
            "UI-05" => "content/packs/ui-inventory.json",
            "UI-12" => "content/packs/ui-inventory.json",
            "UI-13" => "content/packs/ui-inventory.json",
            "UI-06" => "content/packs/ui-legacy-stash.json",
            "UI-14" => "content/packs/ui-legacy-stash.json",
            "UI-15" => "content/packs/ui-legacy-stash.json",
            "UI-16" => "content/packs/ui-camp.json",
            "UI-17" => "content/packs/ui-camp.json",
            "UI-18" => "content/packs/ui-camp.json",
            "UI-19" => "content/packs/ui-camp.json",
            "UI-20" => "content/packs/ui-map-warnings.json",
            "UI-21" => "content/packs/ui-map-warnings.json",
            "UI-26" => "content/packs/ui-map-warnings.json",
            "UI-09" => "content/packs/ui-session.json",
            "UI-10" => "content/packs/ui-session.json",
            "UI-11" => "content/packs/ui-session.json",
            "UI-22" => "content/packs/ui-session.json",
            "UI-23" => "content/packs/ui-session.json",
            "UI-24" => "content/packs/ui-session.json",
            "UI-25" => "content/packs/ui-settings.json",
            "UI-27" => "content/packs/ui-settings.json",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedPrimaryAudio(string id)
    {
        return id switch
        {
            "AU-01" => "content/packs/audio-actions.json",
            "AU-02" => "content/packs/audio-actions.json",
            "AU-03" => "content/packs/audio-actions.json",
            "AU-04" => "content/packs/audio-actions.json",
            "AU-05" => "content/packs/audio-actions.json",
            "AU-06" => "content/packs/audio-actions.json",
            "AU-07" => "content/packs/audio-actions.json",
            "AU-08" => "content/packs/audio-actions.json",
            "AU-09" => "content/packs/audio-actions.json",
            "AU-10" => "content/packs/audio-actions.json",
            "AU-11" => "content/packs/audio-actions.json",
            "AU-12" => "content/packs/audio-actions.json",
            "AU-13" => "content/packs/audio-actions.json",
            "AU-14" => "content/packs/audio-actions.json",
            "AU-15" => "content/packs/audio-actions.json",
            "AU-16" => "content/audio/events.json",
            "AU-17" => "content/audio/events.json",
            "AU-18" => "content/audio/events.json",
            "AU-19" => "content/audio/events.json",
            "AU-20" => "content/audio/events.json",
            "AU-21" => "content/audio/events.json",
            "AU-22" => "content/audio/events.json",
            "AU-23" => "content/audio/events.json",
            "AU-24" => "content/audio/events.json",
            "AU-25" => "content/audio/events.json",
            "AU-26" => "content/audio/events.json",
            "AU-27" => "content/audio/events.json",
            "AU-28" => "content/audio/events.json",
            "AU-29" => "content/audio/events.json",
            "AU-30" => "content/audio/events.json",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedPrimaryData(string id)
    {
        return id switch
        {
            "DT-05" => "content/config/combat.json",
            "DT-08" => "content/config/food-wildlife.json",
            "DT-06" => "content/config/inventory.json",
            "DT-04" => "content/config/items.json",
            "DT-07" => "content/config/jobs.json",
            "DT-25" => "content/config/local-preferences.json",
            "DT-09" => "content/config/map-sites.json",
            "DT-15" => "content/config/portals-assaults.json",
            "DT-17" => "content/config/presentation.json",
            "DT-01" => "content/config/run.json",
            "DT-16" => "content/config/session-save.json",
            "DT-10" => "content/config/structures.json",
            "DT-03" => "content/config/survivors.json",
            "DT-02" => "content/config/time.json",
            "DT-11" => "content/config/towers.json",
            "DT-13" => "content/config/zombie-loot.json",
            "DT-14" => "content/config/zombie-progression.json",
            "DT-12" => "content/config/zombies.json",
            "TXT-01" => "content/text/en.json",
            "TXT-02" => "content/text/expanded.json",
            "DT-24" => "content/packs/compatibility.json",
            "DT-18" => "content/packs/text-data.json",
            "DT-19" => "content/packs/delivery.json",
            "DT-20" => "content/animation/locomotion-melee.json",
            "DT-21" => "content/world/map.json",
            "DT-22" => "content/packs/ui-kit.json",
            "DT-23" => "content/audio/events.json",
            "DT-26" => "content/packs/delivery.json",
            "DT-27" => "content/packs/delivery.json",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedKind(string id)
    {
        return id.Split(separator: '-')[0] switch
        {
            "CH" or "AN" or "IT" => CompletedKindActors(id: id),
            "BL" or "EN" or "FX" => CompletedKindWorld(id: id),
            "UI" => CompletedKindUi(id: id),
            "AU" => CompletedKindAudio(id: id),
            "DT" or "TXT" => CompletedKindData(id: id),
            _ => throw new FormatException(message: "unknown completed obligation domain"),
        };
    }

    private static string CompletedKindActors(string id)
    {
        return id switch
        {
            "CH-01" => "body-parts-v1",
            "CH-02" => "body-parts-v1",
            "CH-03" => "body-parts-v1",
            "CH-04" => "body-parts-v1",
            "CH-05" => "body-parts-v1",
            "CH-08" => "body-parts-v1",
            "CH-06" => "body-parts-v1",
            "CH-07" => "body-parts-v1",
            "IT-01" => "item-parts-v1",
            "IT-02" => "item-parts-v1",
            "IT-03" => "item-parts-v1",
            "IT-04" => "item-parts-v1",
            "IT-05" => "item-parts-v1",
            "IT-06" => "item-parts-v1",
            "IT-07" => "item-parts-v1",
            "IT-08" => "item-parts-v1",
            "IT-09" => "item-parts-v1",
            "IT-10" => "item-parts-v1",
            "IT-11" => "item-parts-v1",
            "IT-12" => "item-parts-v1",
            "IT-13" => "item-parts-v1",
            "IT-14" => "item-parts-v1",
            "IT-15" => "item-parts-v1",
            "IT-16" => "item-parts-v1",
            "IT-17" => "item-parts-v1",
            "IT-18" => "item-parts-v1",
            "IT-19" => "item-parts-v1",
            "IT-20" => "item-parts-v1",
            "IT-21" => "item-parts-v1",
            "IT-22" => "item-parts-v1",
            "IT-23" => "item-parts-v1",
            "IT-24" => "item-parts-v1",
            "AN-01" => "animation-v1",
            "AN-02" => "animation-v1",
            "AN-03" => "animation-v1",
            "AN-04" => "animation-v1",
            "AN-05" => "animation-v1",
            "AN-06" => "animation-v1",
            "AN-07" => "animation-v1",
            "AN-08" => "animation-v1",
            "AN-10" => "animation-v1",
            "AN-11" => "animation-v1",
            "AN-12" => "animation-v1",
            "AN-13" => "animation-v1",
            "AN-14" => "animation-v1",
            "AN-15" => "animation-v1",
            "AN-16" => "animation-v1",
            "CH-10" => "world-parts-v1",
            "CH-11" => "world-parts-v1",
            "CH-13" => "world-parts-v1",
            "AN-09" => "world-parts-v1",
            "AN-17" => "world-parts-v1",
            "CH-09" => "world-parts-v1",
            "AN-18" => "world-parts-v1",
            "CH-12" => "player-identities-v1",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedKindWorld(string id)
    {
        return id switch
        {
            "BL-04" => "structure-parts-v1",
            "BL-05" => "structure-parts-v1",
            "BL-06" => "structure-parts-v1",
            "BL-07" => "structure-parts-v1",
            "BL-08" => "structure-parts-v1",
            "BL-09" => "structure-parts-v1",
            "BL-01" => "structure-parts-v1",
            "BL-02" => "structure-parts-v1",
            "BL-03" => "structure-parts-v1",
            "BL-15" => "world-parts-v1",
            "EN-01" => "world-parts-v1",
            "EN-02" => "world-parts-v1",
            "EN-03" => "world-parts-v1",
            "EN-04" => "world-parts-v1",
            "EN-05" => "world-parts-v1",
            "EN-06" => "world-parts-v1",
            "EN-07" => "world-parts-v1",
            "EN-08" => "world-parts-v1",
            "EN-09" => "world-parts-v1",
            "EN-10" => "world-parts-v1",
            "EN-11" => "world-parts-v1",
            "BL-16" => "world-parts-v1",
            "FX-01" => "world-parts-v1",
            "FX-02" => "world-parts-v1",
            "FX-03" => "world-parts-v1",
            "FX-04" => "world-parts-v1",
            "FX-05" => "world-parts-v1",
            "FX-06" => "world-parts-v1",
            "FX-07" => "world-parts-v1",
            "FX-08" => "world-parts-v1",
            "FX-09" => "world-parts-v1",
            "FX-10" => "world-parts-v1",
            "FX-11" => "world-parts-v1",
            "FX-12" => "world-parts-v1",
            "BL-10" => "world-assembly-v1",
            "BL-11" => "world-assembly-v1",
            "BL-12" => "world-assembly-v1",
            "BL-13" => "world-assembly-v1",
            "BL-14" => "world-assembly-v1",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedKindUi(string id)
    {
        return id switch
        {
            "UI-01" => "native-layout-v1",
            "UI-02" => "native-layout-v1",
            "UI-03" => "native-layout-v1",
            "UI-04" => "native-layout-v1",
            "UI-07" => "native-layout-v1",
            "UI-08" => "native-layout-v1",
            "UI-05" => "native-layout-v1",
            "UI-12" => "native-layout-v1",
            "UI-13" => "native-layout-v1",
            "UI-06" => "native-layout-v1",
            "UI-14" => "native-layout-v1",
            "UI-15" => "native-layout-v1",
            "UI-16" => "native-layout-v1",
            "UI-17" => "native-layout-v1",
            "UI-18" => "native-layout-v1",
            "UI-19" => "native-layout-v1",
            "UI-20" => "native-layout-v1",
            "UI-21" => "native-layout-v1",
            "UI-26" => "native-layout-v1",
            "UI-09" => "native-layout-v1",
            "UI-10" => "native-layout-v1",
            "UI-11" => "native-layout-v1",
            "UI-22" => "native-layout-v1",
            "UI-23" => "native-layout-v1",
            "UI-24" => "native-layout-v1",
            "UI-25" => "native-layout-v1",
            "UI-27" => "native-layout-v1",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedKindAudio(string id)
    {
        return id switch
        {
            "AU-01" => "audio-bank-v1",
            "AU-02" => "audio-bank-v1",
            "AU-03" => "audio-bank-v1",
            "AU-04" => "audio-bank-v1",
            "AU-05" => "audio-bank-v1",
            "AU-06" => "audio-bank-v1",
            "AU-07" => "audio-bank-v1",
            "AU-08" => "audio-bank-v1",
            "AU-09" => "audio-bank-v1",
            "AU-10" => "audio-bank-v1",
            "AU-11" => "audio-bank-v1",
            "AU-12" => "audio-bank-v1",
            "AU-13" => "audio-bank-v1",
            "AU-14" => "audio-bank-v1",
            "AU-15" => "audio-bank-v1",
            "AU-16" => "audio-events-v1",
            "AU-17" => "audio-events-v1",
            "AU-18" => "audio-events-v1",
            "AU-19" => "audio-events-v1",
            "AU-20" => "audio-events-v1",
            "AU-21" => "audio-events-v1",
            "AU-22" => "audio-events-v1",
            "AU-23" => "audio-events-v1",
            "AU-24" => "audio-events-v1",
            "AU-25" => "audio-events-v1",
            "AU-26" => "audio-events-v1",
            "AU-27" => "audio-events-v1",
            "AU-28" => "audio-events-v1",
            "AU-29" => "audio-events-v1",
            "AU-30" => "audio-events-v1",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedKindData(string id)
    {
        return id switch
        {
            "DT-05" => "authored-domain-v1",
            "DT-08" => "authored-domain-v1",
            "DT-06" => "authored-domain-v1",
            "DT-04" => "authored-domain-v1",
            "DT-07" => "authored-domain-v1",
            "DT-25" => "authored-domain-v1",
            "DT-09" => "authored-domain-v1",
            "DT-15" => "authored-domain-v1",
            "DT-17" => "authored-domain-v1",
            "DT-01" => "authored-domain-v1",
            "DT-16" => "authored-domain-v1",
            "DT-10" => "authored-domain-v1",
            "DT-03" => "authored-domain-v1",
            "DT-02" => "authored-domain-v1",
            "DT-11" => "authored-domain-v1",
            "DT-13" => "authored-domain-v1",
            "DT-14" => "authored-domain-v1",
            "DT-12" => "authored-domain-v1",
            "TXT-01" => "text-catalog-v1",
            "TXT-02" => "text-catalog-v1",
            "DT-24" => "compatibility-v1",
            "DT-18" => "designer-documentation-v1",
            "DT-19" => "source-provenance-composition-v1",
            "DT-20" => "animation-library-v1",
            "DT-21" => "world-assembly-library-v1",
            "DT-22" => "native-layout-library-v1",
            "DT-23" => "audio-event-library-v1",
            "DT-26" => "fixture-catalog-v1",
            "DT-27" => "desktop-source-v1",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedDocumentIdentity(string path)
    {
        return path.StartsWith(value: "content/config/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "content/schema/", comparisonType: StringComparison.Ordinal)
                || path.StartsWith(value: "content/text/", comparisonType: StringComparison.Ordinal)
                ? CompletedDataIdentity(path: path) : CompletedProductionIdentity(path: path);
    }

    private static string CompletedDataIdentity(string path)
    {
        return path switch
        {
            "content/config/combat.json" => "DT-05",
            "content/config/food-wildlife.json" => "DT-08",
            "content/config/inventory.json" => "DT-06",
            "content/config/items.json" => "DT-04",
            "content/config/jobs.json" => "DT-07",
            "content/config/local-preferences.json" => "DT-25",
            "content/config/map-sites.json" => "DT-09",
            "content/config/portals-assaults.json" => "DT-15",
            "content/config/presentation.json" => "DT-17",
            "content/config/run.json" => "DT-01",
            "content/config/session-save.json" => "DT-16",
            "content/config/structures.json" => "DT-10",
            "content/config/survivors.json" => "DT-03",
            "content/config/time.json" => "DT-02",
            "content/config/towers.json" => "DT-11",
            "content/config/zombie-loot.json" => "DT-13",
            "content/config/zombie-progression.json" => "DT-14",
            "content/config/zombies.json" => "DT-12",
            "content/schema/fields-economy.json" => "fields-economy",
            "content/schema/fields-foundation.json" => "fields-foundation",
            "content/schema/fields-items-combat.json" => "fields-items-combat",
            "content/schema/fields-world.json" => "fields-world",
            "content/schema/fields-zombies.json" => "fields-zombies",
            "content/text/en.json" => "TXT-01",
            "content/text/expanded.json" => "TXT-02",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static string CompletedProductionIdentity(string path)
    {
        return path switch
        {
            "content/animation/bow.json" => "bow",
            "content/animation/fits-a.json" => "fits-a",
            "content/animation/fits-b.json" => "fits-b",
            "content/animation/fits-c.json" => "fits-c",
            "content/animation/locomotion-melee.json" => "locomotion-melee",
            "content/animation/work-special.json" => "work-special",
            "content/audio/events.json" => "sound-events",
            "content/compatibility/appearance-layout.json" => "DT-24",
            "content/fixtures/audio-states.json" => "audio-states",
            "content/fixtures/crowds.json" => "crowds",
            "content/fixtures/world-states.json" => "world-states",
            "content/packs/audio-actions.json" => "audio-actions",
            "content/packs/audio-interface-ambience.json" => "audio-interface-ambience",
            "content/packs/bodies-a.json" => "bodies-a",
            "content/packs/bodies-b.json" => "bodies-b",
            "content/packs/bodies-c.json" => "bodies-c",
            "content/packs/camp-access.json" => "camp-access",
            "content/packs/common.json" => "common",
            "content/packs/compatibility.json" => "compatibility",
            "content/packs/corpses-wildlife.json" => "corpses-wildlife",
            "content/packs/defenses.json" => "defenses",
            "content/packs/delivery.json" => "delivery",
            "content/packs/effects-portals.json" => "effects-portals",
            "content/packs/guards.json" => "guards",
            "content/packs/items-01-10.json" => "items-01-10",
            "content/packs/items-11-24.json" => "items-11-24",
            "content/packs/main-buildings.json" => "main-buildings",
            "content/packs/player-identities.json" => "player-identities",
            "content/packs/scale-pilot.json" => "scale-pilot",
            "content/packs/text-data.json" => "text-data",
            "content/packs/tracer.json" => "tracer",
            "content/packs/ui-camp.json" => "ui-camp",
            "content/packs/ui-identity.json" => "ui-identity",
            "content/packs/ui-inventory.json" => "ui-inventory",
            "content/packs/ui-kit.json" => "ui-kit",
            "content/packs/ui-legacy-stash.json" => "ui-legacy-stash",
            "content/packs/ui-map-warnings.json" => "ui-map-warnings",
            "content/packs/ui-session.json" => "ui-session",
            "content/packs/ui-settings.json" => "ui-settings",
            "content/packs/world.json" => "world",
            "content/ui/camp-panels.json" => "camp-panels",
            "content/ui/inventory.json" => "inventory-layouts",
            "content/ui/legacy-stash-targets.json" => "legacy-stash-target-layouts",
            "content/ui/map-spectator-warnings.json" => "map-spectator-warnings",
            "content/ui/session-save-outcomes.json" => "session-save-outcomes",
            "content/ui/settings-errors.json" => "settings-errors",
            "content/world/defense-assemblies.json" => "defense-assemblies",
            "content/world/main-assemblies.json" => "main-assemblies",
            "content/world/map.json" => "world-map",
            "content/world/portal-assemblies.json" => "portal-assemblies",
            _ => throw new FormatException(message: "unknown exact completed source contract"),
        };
    }

    private static JsonDocument Parse(byte[]? bytes, string path = "")
    {
        int limit = StringComparer.Ordinal.Equals(x: path, y: "content/packs/common.json")
                ? 2_097_152 : MaxDocumentBytes;
        if (bytes is null || bytes.Length == 0 || bytes.Length > limit)
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
                || Integer(row: root, field: "version") is <= 0 or > int.MaxValue)
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
