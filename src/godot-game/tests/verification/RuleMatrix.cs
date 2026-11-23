using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Checks the focused index against current bindings without maintaining a second rule definition.</summary>
internal static class RuleMatrix
{
    /// <summary>Returns the exact index rows in stable rule order, including intentionally pending coverage.</summary>
    internal static string Rows(JsonObject register)
    {
        return string.Join(separator: "\n", values: (register["rules"]?.AsArray()
                ?? throw new System.IO.InvalidDataException(message: "MATRIX_RULES_REQUIRED"))
                .Select(selector: row => row ?? throw new System.IO.InvalidDataException(message: "MATRIX_RULE"))
                .OrderBy(keySelector: row => Text(row: row, key: "sourceId"), comparer: StringComparer.Ordinal)
                .Select(selector: Render));
    }

    /// <summary>Requires one exact current index block; missing or stale rows cannot pass navigation alone.</summary>
    internal static bool Matches(JsonObject register, string source)
    {
        string expected = Start + "\n" + Rows(register: register) + "\n" + End;
        return source.Split(separator: Start).Length == 2 && source.Split(separator: End).Length == 2
                && source.Contains(value: expected, comparisonType: StringComparison.Ordinal);
    }

    private const string Start = "<!-- rule-bindings:start -->";
    private const string End = "<!-- rule-bindings:end -->";
    private static string Render(JsonNode row)
    {
        string id = Text(row: row, key: "sourceId");
        JsonNode? binding = row["binding"];
        string mechanism = binding is null ? "Inapplicable; see register reason"
                : Text(row: binding, key: "status") is "pending"
                        ? "Pending Phase 11: `" + Text(row: binding, key: "path") + "`"
                        : "[" + Text(row: binding, key: "symbol") + "](../../"
                                + Text(row: binding, key: "path") + ")";
        string scopes = Text(row: row, key: "scope");
        string permission = Permissions(row: row);
        return "| <a id=\"" + id.ToLowerInvariant() + "\"></a>" + id + " | " + Text(row: row, key: "disposition")
                + " | " + mechanism + " | " + scopes + " | " + permission + " | "
                + Proof(row: row, polarity: "positive") + " | " + Proof(row: row, polarity: "negative") + " |";
    }

    private static string Permissions(JsonNode row)
    {
        var references = new List<string>(collection: row["exemptions"]?.AsArray().Select(
                selector: item => item?.GetValue<string>() ?? "") ?? []);
        string id = Text(row: row, key: "sourceId");
        string[] diagnostics = [.. row["mechanism"]?["diagnostics"]?.AsArray().Select(
                selector: item => item?.GetValue<string>() ?? "") ?? []];
        string[] files = ["generated-callsite-adaptations.json", "generated-layout-adaptations.json",
            "generated-syntax-adaptations.json", "engine-adaptations.json", "engine-generated-spans.json"];
        foreach (string file in files)
        {
            JsonObject registry = RegisterValidator.Read(relative: "docs/code/" + file);
            bool matches = registry.Where(predicate: pair => pair.Value is JsonArray).SelectMany(
                    selector: pair => pair.Value?.AsArray() ?? []).Any(predicate: entry => diagnostics.Contains(
                            value: Text(row: entry, key: "diagnostic"), comparer: StringComparer.Ordinal));
            if (matches || id is "R-50" or "R-67")
            {
                references.Add(item: "[" + file + "](" + file + ")");
            }
        }

        if (id is "R-34")
        {
            references.Add(item: "[visibility-exemptions.json](visibility-exemptions.json)");
        }

        return references.Count == 0 ? "None" : string.Join(separator: ", ", values: references);
    }

    private static string Proof(JsonNode row, string polarity)
    {
        JsonNode? proof = row[polarity + "Proof"];
        if (proof is null)
        {
            return polarity is "positive" ? Text(row: row, key: "sourceId") + ".register-resolution" : "Inapplicable";
        }

        string id = Text(row: proof, key: "id");
        string method = Text(row: row["binding"], key: polarity + "Method");
        return method.Length == 0 ? id + " (Pending Phase 11)" : "[" + id + " · "
                + method[(method.LastIndexOf(value: '.') + 1)..] + "](../../" + Text(row: proof, key: "path") + ")";
    }

    private static string Text(JsonNode? row, string key)
    {
        return row?[key]?.GetValue<string>() ?? "";
    }
}
