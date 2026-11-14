using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Reconciles current mechanism symbols and exact unique proof methods before coverage delivery.</summary>
internal static class RuleBindingValidator
{
    /// <summary>Checks source definitions and reciprocal proof identities; pending coverage is explicit.</summary>
    internal static IReadOnlyList<string> Check(JsonObject register, JsonObject cases,
            IReadOnlyDictionary<string, string>? changedSources = null)
    {
        var errors = new List<string>();
        JsonArray rules = register["rules"]?.AsArray()
                ?? throw new InvalidDataException(message: "BINDING_RULES_REQUIRED");
        JsonArray declarations = cases["cases"]?.AsArray()
                ?? throw new InvalidDataException(message: "BINDING_CASES_REQUIRED");
        var methods = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonNode? row in rules)
        {
            JsonNode rule = row ?? throw new InvalidDataException(message: "BINDING_RULE_REQUIRED");
            string id = Text(row: rule, key: "sourceId");
            if (Text(row: rule, key: "disposition") is "inapplicable")
            {
                if (rule["binding"] is not null)
                {
                    errors.Add(item: id + ": BINDING_INAPPLICABLE");
                }

                continue;
            }

            CheckRule(rule: rule, declarations: declarations, methods: methods, changedSources: changedSources,
                    errors: errors);
        }

        return errors;
    }

    private static void CheckRule(JsonNode rule, JsonArray declarations, HashSet<string> methods,
            IReadOnlyDictionary<string, string>? changedSources, List<string> errors)
    {
        string id = Text(row: rule, key: "sourceId");
        JsonNode? binding = rule["binding"];
        if (binding is null)
        {
            errors.Add(item: id + ": BINDING_REQUIRED");
            return;
        }

        bool pending = id is "R-49" or "R-82";
        string plannedPath = Text(row: rule["mechanism"], key: "path");
        string plannedSymbol = Text(row: rule["mechanism"], key: "symbol");
        (string Path, string Symbol) expected = RuleDefinitions.Expected(id: id, plannedPath: plannedPath,
                plannedSymbol: plannedSymbol);
        if (!StringComparer.Ordinal.Equals(x: Text(row: binding, key: "path"), y: expected.Path)
                || !StringComparer.Ordinal.Equals(x: Text(row: binding, key: "symbol"), y: expected.Symbol)
                || !StringComparer.Ordinal.Equals(x: Text(row: binding, key: "scope"),
                        y: Text(row: rule, key: "scope")))
        {
            errors.Add(item: id + ": BINDING_DEFINITION");
        }

        if (!StringComparer.Ordinal.Equals(x: Text(row: binding, key: "status"), y: pending ? "pending" : "implemented")
                || !StringComparer.Ordinal.Equals(x: Text(row: binding, key: "owner"),
                        y: pending ? "Phase 5" : "01-14"))
        {
            errors.Add(item: id + ": BINDING_STATUS");
        }

        if (!pending && !HasDefinition(path: expected.Path, symbol: expected.Symbol, changedSources: changedSources))
        {
            errors.Add(item: id + ": BINDING_SOURCE");
        }

        foreach (string polarity in new[] { "positive", "negative" })
        {
            CheckProof(rule: rule, binding: binding, polarity: polarity, declarations: declarations, pending: pending,
                    methods: methods, errors: errors);
        }
    }

    private static void CheckProof(JsonNode rule, JsonNode binding, string polarity, JsonArray declarations,
            bool pending, HashSet<string> methods, List<string> errors)
    {
        string id = Text(row: rule, key: "sourceId");
        string proof = Text(row: rule[polarity + "Proof"], key: "id");
        JsonNode?[] matches = [.. declarations.Where(predicate: row => StringComparer.Ordinal.Equals(
                x: Text(row: row, key: "id"), y: proof))];
        if (matches.Length != 1 || !StringComparer.Ordinal.Equals(x: proof, y: id + "." + polarity))
        {
            errors.Add(item: id + ": BINDING_PROOF_ID");
            return;
        }

        JsonNode declaration = matches[0] ?? throw new InvalidDataException(message: "BINDING_DECLARATION");
        string method = Text(row: binding, key: polarity + "Method");
        if (pending)
        {
            if (method.Length != 0 || Text(row: declaration, key: "implementation") is not "pending"
                    || Text(row: declaration, key: "owner") is not "Phase 5"
                    || declaration["acceptancePhase"]?.ToJsonString() is not "5")
            {
                errors.Add(item: id + ": BINDING_COVERAGE_PENDING");
            }

            return;
        }

        string path = Text(row: rule[polarity + "Proof"], key: "path");
        if (!StringComparer.Ordinal.Equals(x: method, y: Text(row: declaration, key: "test"))
                || Text(row: declaration, key: "implementation") is not "csharp" || !methods.Add(item: method)
                || !HasProof(path: path, method: method, id: proof))
        {
            errors.Add(item: id + ": BINDING_PROOF_METHOD");
        }
    }

    private static bool HasDefinition(string path, string symbol, IReadOnlyDictionary<string, string>? changedSources)
    {
        string full = Path.Combine(path1: ProofProtocol.Root, path2: path);
        if (!File.Exists(path: full))
        {
            return false;
        }

        string source = changedSources?.TryGetValue(key: path, value: out string? changed) == true
                ? changed : File.ReadAllText(path: full);
        if (!path.EndsWith(value: ".cs", comparisonType: StringComparison.Ordinal))
        {
            return source.Split(separator: '\n').Count(predicate: line => symbol is "<ProjectReference"
                    ? line.Trim().StartsWith(value: symbol, comparisonType: StringComparison.Ordinal)
                    : StringComparer.Ordinal.Equals(x: line.Trim(), y: symbol)) == 1;
        }

        string[] parts = symbol.Split(separator: '.');
        return parts.Length == 2 && CSharpSyntaxTree.ParseText(text: source).GetRoot().DescendantNodes()
                .OfType<MethodDeclarationSyntax>().Count(predicate: method => MatchesMethod(method: method,
                        symbol: parts[1]) && method.Parent is TypeDeclarationSyntax type
                        && StringComparer.Ordinal.Equals(x: type.Identifier.ValueText, y: parts[0])) == 1;
    }

    private static bool MatchesMethod(MethodDeclarationSyntax method, string symbol)
    {
        int signature = symbol.IndexOf(value: '(');
        string name = signature < 0 ? symbol : symbol[..signature];
        return StringComparer.Ordinal.Equals(x: method.Identifier.ValueText, y: name)
                && (signature < 0 || symbol.EndsWith(value: ')') && method.ParameterList.Parameters.Select(
                        selector: parameter => parameter.Identifier.ValueText).SequenceEqual(
                                second: symbol[(signature + 1)..^1].Split(separator: ','),
                                comparer: StringComparer.Ordinal));
    }

    private static bool HasProof(string path, string method, string id)
    {
        string full = Path.Combine(path1: ProofProtocol.Root, path2: path);
        if (!File.Exists(path: full))
        {
            return false;
        }

        string source = File.ReadAllText(path: full);
        MethodDeclarationSyntax[] matches = [.. CSharpSyntaxTree.ParseText(text: source).GetRoot().DescendantNodes()
                .OfType<MethodDeclarationSyntax>().Where(predicate: item => method.EndsWith(
                        value: "." + item.Identifier.ValueText, comparisonType: StringComparison.Ordinal))];
        return matches.Length == 1 && TestSourceIndex.TestMethods(content: source).Contains(value: method,
                comparer: StringComparer.Ordinal) && matches[0].AttributeLists.SelectMany(selector: list =>
                        list.Attributes).Any(predicate: attribute => HasTrait(attribute: attribute, id: id));
    }

    private static bool HasTrait(AttributeSyntax attribute, string id)
    {
        return attribute.Name.ToString() is "Trait" && attribute.ArgumentList?.Arguments.Select(
                selector: argument => (argument.Expression as LiteralExpressionSyntax)?.Token.ValueText)
                .SequenceEqual(second: ["Case", id], comparer: StringComparer.Ordinal) == true;
    }

    private static string Text(JsonNode? row, string key)
    {
        return row?[key]?.GetValue<string>() ?? "";
    }
}
