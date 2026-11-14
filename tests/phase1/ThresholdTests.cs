using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
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
        JsonObject numeric = register["rules"]?.AsArray().Single(predicate: row => StringComparer.Ordinal.Equals(
                x: row?["sourceId"]?.GetValue<string>(), y: "R-07"))?["numeric"]?.AsObject()
                ?? throw new InvalidOperationException(message: "NUMERIC_SUBJECT_REQUIRED");
        numeric["limit"] = JsonNode.Parse(json: "2147483648");
        string observed;
        try
        {
            IReadOnlyList<string> errors = RegisterValidator.Validate(manifest: manifest, register: register,
                    exemptions: exemptions);
            observed = string.Join(separator: "\n", values: errors);
        }
        catch (Exception error) when (error is FormatException or OverflowException)
        {
            observed = "UNHANDLED_NUMERIC_EXCEPTION " + error.GetType().Name;
        }

        observed.ShouldContain(expected: "R-07: NUMERIC_VALUE");
        ProofProtocol.Record(id: "edge.rule-03.precision", diagnostic: "NUMERIC_VALUE_REJECTED",
                location: "docs/code/register.json R-07 numeric.limit above Int32.MaxValue");
    }
}
