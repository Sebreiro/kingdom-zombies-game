using System.Text.Json.Nodes;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

public sealed class RegisterTests
{
    private static (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) Inputs() =>
        (RegisterValidator.Read("docs/code/provenance/manifest.json"), RegisterValidator.Read("docs/code/register.json"), RegisterValidator.Read("docs/code/exemptions.json"));

    [Fact, Trait("Case", "register.capture")]
    public void Capture_MissingHeadingRelation_IsRejected()
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        input.Register["rules"]![0]!["headingRefs"]!.AsArray().RemoveAt(0);
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain(x => x.Contains("R-01: HEADING_RELATION", StringComparison.Ordinal));
        ProofProtocol.Record("register.capture", "CAPTURE_EXACT", "85 IDs, 84 heading occurrences, 24 donor exemptions and missing R-01 relation");
    }

    private static void RejectCapture(string id, Action<JsonObject, JsonObject, JsonObject> mutate, string expected)
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        mutate(input.Manifest, input.Register, input.Exemptions);
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain(x => x.Contains(expected, StringComparison.Ordinal));
        ProofProtocol.Record(id, expected, "detached captured inventory mutation");
    }

    [Fact, Trait("Case", "register.omit-heading")]
    public void Capture_MissingHeading_IsRejected() => RejectCapture("register.omit-heading", (m, r, e) => m["headings"]!.AsArray().RemoveAt(0), "CAPTURE: HEADING_INVENTORY");

    [Fact, Trait("Case", "register.omit-matrix")]
    public void Capture_MissingMatrixRow_IsRejected() => RejectCapture("register.omit-matrix", (m, r, e) => m["matrix"]!.AsArray().RemoveAt(0), "R-01: SOURCE_ROW");

    [Fact, Trait("Case", "register.omit-exemption")]
    public void Capture_MissingSourceExemption_IsRejected() => RejectCapture("register.omit-exemption", (m, r, e) => e["donorExemptions"]!.AsArray().RemoveAt(0), "REGISTER: DONOR_EXEMPTION_INVENTORY");

    [Fact, Trait("Case", "register.duplicate")]
    public void Capture_DuplicateId_IsRejectedBeforeSetConversion() => RejectCapture("register.duplicate", (m, r, e) => r["rules"]!.AsArray().Add(r["rules"]![0]!.DeepClone()), "R-01: RULE_ID: DUPLICATE_OR_EMPTY");

    [Fact, Trait("Case", "register.r37-trace")]
    public void Capture_R37_ConnectsBothHeadingsAndPlannedBoundaryProofs()
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        var row = input.Register["rules"]!.AsArray().Single(x => x!["sourceId"]!.GetValue<string>() == "R-37")!;
        row["headingRefs"]!.AsArray().Count.ShouldBe(2);
        row["mechanism"]!["path"]!.GetValue<string>().ShouldBe("tools/code-rules/BodyLengthAnalyzer.cs");
        row["mechanism"]!["diagnostics"]!.AsArray().Select(x => x!.GetValue<string>()).ShouldContain("SCR0006");
        row["mechanism"]!["status"]!.GetValue<string>().ShouldBe("pending");
        row["numeric"]!["boundaries"]!.AsArray().Select(x => x!.GetValue<int>()).ShouldBe(new[] { 59, 60, 61 });
        row["positiveProof"]!["id"]!.GetValue<string>().ShouldBe("R-37.positive");
        row["negativeProof"]!["id"]!.GetValue<string>().ShouldBe("R-37.negative");
        ProofProtocol.Record("register.r37-trace", "R37_TRACE_EXACT", "R-37 source identity → SCR0006 pending implementation → declared 59/60/61 boundaries; no enforcement pass claimed");
    }

    [Fact, Trait("Case", "register.schema")]
    public void Schema_MissingMechanism_IsRejected() => RejectCapture("register.schema", (m, r, e) => r["rules"]![0]!["mechanism"] = null, "R-01: MECHANISM_REQUIRED");
}
