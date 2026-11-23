using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Tests current linkage independently from captured provenance and incomplete phase acceptance.</summary>
public sealed class RuleBindingTests
{
    /// <summary>Requires real definitions and uniquely owned positive/negative methods for 77 rules.</summary>
    [Fact, Trait("Case", "register.binding-precoverage")]
    public void Bind_CurrentMechanisms_RequiresActualPairedMethods()
    {
        JsonObject register = Register();
        RegisterValidator.Validate(manifest: RegisterValidator.Read(relative: "docs/code/provenance/manifest.json"),
                register: register, exemptions: RegisterValidator.Read(relative: "docs/code/exemptions.json"))
                .ShouldBeEmpty();
        RuleBindingValidator.Check(register: register, cases: Cases()).ShouldBeEmpty();
        RuleMatrix.Matches(register: register, source: Matrix()).ShouldBeTrue();
        ProofProtocol.Record(id: "register.binding-precoverage", diagnostic: "BINDING_PRECOVERAGE",
                location: "docs/code/register.json: 77 implemented, 2 Phase 11 pending, 6 inapplicable; human Pending");
    }

    /// <summary>A missing binding cannot be replaced by a historical pending planned locator.</summary>
    [Fact, Trait("Case", "register.binding-missing")]
    public void Bind_MissingCurrentDefinition_RejectsPlaceholder()
    {
        Reject(id: "register.binding-missing", expected: "BINDING_REQUIRED",
                mutate: (register, cases) => Rule(register: register, id: "R-01")["binding"] = null);
    }

    /// <summary>Exchanging two genuine method names still fails the independent polarity identity.</summary>
    [Fact, Trait("Case", "register.binding-reversed")]
    public void Bind_ReversedCurrentMethods_RejectsPolarity()
    {
        Reject(id: "register.binding-reversed", expected: "BINDING_PROOF_METHOD", mutate: (register, cases) =>
        {
            JsonObject binding = Binding(register: register, id: "R-01");
            string positive = binding["positiveMethod"]?.GetValue<string>() ?? "";
            binding["positiveMethod"] = binding["negativeMethod"]?.GetValue<string>();
            binding["negativeMethod"] = positive;
        });
    }

    /// <summary>Duplicating an exact required proof declaration fails before selecting an accidental winner.</summary>
    [Fact, Trait("Case", "register.binding-ambiguous")]
    public void Bind_DuplicateDeclaration_RejectsAmbiguity()
    {
        Reject(id: "register.binding-ambiguous", expected: "BINDING_PROOF_ID", mutate: (register, cases) =>
        {
            JsonArray rows = cases["cases"]?.AsArray() ?? throw new InvalidDataException(message: "CASES_REQUIRED");
            JsonNode duplicate = rows.Single(predicate: row => row?["id"]?.GetValue<string>() is "R-01.positive")
                    ?? throw new InvalidDataException(message: "CASE_REQUIRED");
            rows.Add(item: duplicate.DeepClone());
        });
    }

    /// <summary>Actual severity bytes reject current-rule/config drift even with unchanged locators.</summary>
    [Fact, Trait("Case", "register.binding-config")]
    public void Bind_ChangedLiveSeverity_RejectsCurrentMismatch()
    {
        JsonObject register = Register();
        RuleBindingValidator.Check(register: register, cases: Cases()).ShouldBeEmpty();
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: ".editorconfig"));
        var changed = new Dictionary<string, string>(comparer: StringComparer.Ordinal)
        {
            [".editorconfig"] = source.Replace(oldValue: "dotnet_diagnostic.IDE0161.severity = error",
                    newValue: "dotnet_diagnostic.IDE0161.severity = warning", comparisonType: StringComparison.Ordinal)
        };
        RuleBindingValidator.Check(register: register, cases: Cases(), changedSources: changed).ShouldContain(
                expected: "R-11: BINDING_SOURCE", comparer: StringComparer.Ordinal);
        ProofProtocol.Record(id: "register.binding-config", diagnostic: "BINDING_SOURCE",
                location: ".editorconfig: IDE0161 actual source severity mutation");
    }

    /// <summary>An existing source file with an invented symbol is not an operational rule reference.</summary>
    [Fact, Trait("Case", "register.binding-symbol")]
    public void Bind_InventedCurrentSymbol_RejectsMissingDefinition()
    {
        Reject(id: "register.binding-symbol", expected: "BINDING_DEFINITION", mutate: (register, cases) =>
                Binding(register: register, id: "R-01")["symbol"] = "MissingDefinition");
    }

    /// <summary>Changing the scope alone invalidates its tested current binding and index.</summary>
    [Fact, Trait("Case", "register.binding-scope")]
    public void Bind_ChangedScope_RejectsIndexDrift()
    {
        Reject(id: "register.binding-scope", expected: "BINDING_DEFINITION", mutate: (register, cases) =>
                Rule(register: register, id: "R-01")["scope"] = "All generated code exempt");
    }

    /// <summary>Coverage cannot borrow an unrelated real method to claim a missing collection mechanism.</summary>
    [Fact, Trait("Case", "register.binding-pending")]
    public void Bind_UnimplementedCoverage_RejectsCompletionCredit()
    {
        foreach (string id in new[] { "R-49", "R-82" })
        {
            JsonObject register = Register();
            Binding(register: register, id: id)["owner"] = "01-15";
            RuleBindingValidator.Check(register: register, cases: Cases()).ShouldContain(
                    expected: id + ": BINDING_STATUS", comparer: StringComparer.Ordinal);
            JsonObject cases = Cases();
            JsonNode declaration = cases["cases"]?.AsArray().Single(predicate: row => StringComparer.Ordinal.Equals(
                    x: row?["id"]?.GetValue<string>(), y: id + ".positive"))
                    ?? throw new InvalidDataException(message: "CASE_REQUIRED");
            declaration["acceptancePhase"] = 1;
            RuleBindingValidator.Check(register: Register(), cases: cases).ShouldContain(
                    expected: id + ": BINDING_COVERAGE_PENDING", comparer: StringComparer.Ordinal);
            declaration["acceptancePhase"] = 5;
            declaration["owner"] = "01-15";
            RuleBindingValidator.Check(register: Register(), cases: cases).ShouldContain(
                    expected: id + ": BINDING_COVERAGE_PENDING", comparer: StringComparer.Ordinal);
        }

        Reject(id: "register.binding-pending", expected: "BINDING_COVERAGE_PENDING", mutate: (register, cases) =>
                Binding(register: register, id: "R-49")["positiveMethod"] =
                        Binding(register: register, id: "R-01")["positiveMethod"]?.GetValue<string>());
    }

    /// <summary>Portable link validity alone cannot conceal a removed exact rule index row.</summary>
    [Fact, Trait("Case", "register.binding-index")]
    public void Bind_RemovedIndexRow_RejectsStaleMapping()
    {
        JsonObject register = Register();
        string current = Matrix();
        RuleMatrix.Matches(register: register, source: current).ShouldBeTrue();
        string changed = string.Join(separator: "\n", values: current.Split(separator: '\n').Where(
                predicate: line => !line.StartsWith(value: "| <a id=\"r-01\"",
                        comparisonType: StringComparison.Ordinal)));
        RuleMatrix.Matches(register: register, source: changed).ShouldBeFalse();
        ProofProtocol.Record(id: "register.binding-index", diagnostic: "BINDING_INDEX",
                location: "docs/code/rule-matrix.md: missing R-01 current row");
    }

    /// <summary>Register row order has no effect on real linkage or the stable focused rule index.</summary>
    [Fact, Trait("Case", "register.binding-order")]
    public void Bind_ReorderedCurrentRows_PreservesIndex()
    {
        JsonObject register = Register();
        JsonArray rows = register["rules"]?.AsArray() ?? throw new InvalidDataException(message: "RULES_REQUIRED");
        register["rules"] = new JsonArray(items: [.. rows.Reverse().Select(selector: row => row?.DeepClone())]);
        RuleBindingValidator.Check(register: register, cases: Cases()).ShouldBeEmpty();
        RuleMatrix.Matches(register: register, source: Matrix()).ShouldBeTrue();
        ProofProtocol.Record(id: "register.binding-order", diagnostic: "BINDING_ORDER",
                location: "docs/code/register.json: reversed rows preserve exact stable index and actual definitions");
    }

    private static JsonObject Register()
    {
        return RegisterValidator.Read(relative: "docs/code/register.json");
    }

    private static JsonObject Cases()
    {
        return RegisterValidator.Read(relative: "src/godot-game/tools/verification/required-cases.json");
    }

    private static string Matrix()
    {
        return File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: "docs/code/rule-matrix.md"));
    }

    private static JsonObject Rule(JsonObject register, string id)
    {
        return register["rules"]?.AsArray().Single(predicate: row => StringComparer.Ordinal.Equals(
                x: row?["sourceId"]?.GetValue<string>(), y: id))?.AsObject()
                ?? throw new InvalidDataException(message: "RULE_REQUIRED " + id);
    }

    private static JsonObject Binding(JsonObject register, string id)
    {
        return Rule(register: register, id: id)["binding"]?.AsObject()
                ?? throw new InvalidDataException(message: "BINDING_REQUIRED " + id);
    }

    private static void Reject(string id, string expected, Action<JsonObject, JsonObject> mutate)
    {
        JsonObject register = Register();
        JsonObject cases = Cases();
        RuleBindingValidator.Check(register: register, cases: cases).ShouldBeEmpty();
        mutate(register, cases);
        RuleBindingValidator.Check(register: register, cases: cases).ShouldContain(
                elementPredicate: error => error.EndsWith(value: ": " + expected,
                        comparisonType: StringComparison.Ordinal));
        ProofProtocol.Record(id: id, diagnostic: expected, location: "detached current binding/declaration mutation");
    }
}
