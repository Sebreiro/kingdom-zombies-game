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

    [Fact, Trait("Case", "register.missing-relation-field")]
    public void Schema_AbsentRelationField_IsActionable() => RejectCapture("register.missing-relation-field", (m, r, e) => r["rules"]![0]!.AsObject().Remove("headingRefs"), "R-01: HEADING_RELATION");

    [Fact, Trait("Case", "ac-05.complete")]
    public void Inventory_CompleteBaseline_HasEveryExactDisposition()
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        input.Manifest["matrix"]!.AsArray().Count.ShouldBe(85);
        input.Manifest["headings"]!.AsArray().Count.ShouldBe(84);
        input.Exemptions["donorExemptions"]!.AsArray().Count.ShouldBe(24);
        input.Exemptions["donorExemptions"]!.AsArray().Any(x => x!["sourceId"]!.GetValue<string>() == "EX-22").ShouldBeFalse();
        ProofProtocol.Record("ac-05.complete", "INVENTORY_EXACT", "complete local immutable source inventory and all dispositions; enforcement separately pending");
    }

    [Fact, Trait("Case", "edge.rule-02.adjacency")]
    public void Schema_ConflictingDuplicate_CannotMaskOriginal() => RejectCapture("edge.rule-02.adjacency", (m, r, e) =>
    {
        var copy = r["rules"]![0]!.DeepClone(); copy["disposition"] = "inapplicable"; r["rules"]!.AsArray().Add(copy);
    }, "R-01: RULE_ID: DUPLICATE_OR_EMPTY");

    [Fact, Trait("Case", "edge.rule-02.empty")]
    public void Schema_EmptyRequiredFields_AlwaysFail()
    {
        var mutations = new (Action<JsonObject, JsonObject, JsonObject> Apply, string Diagnostic)[]
        {
            ((m, r, e) => r["rules"]!.AsArray().Clear(), "REGISTER: RULE_INVENTORY"),
            ((m, r, e) => m["headings"]!.AsArray().Clear(), "CAPTURE: HEADING_INVENTORY"),
            ((m, r, e) => m["files"]!.AsArray().Clear(), "CAPTURE: FILE_INVENTORY"),
            ((m, r, e) => r["rules"]![0]!["reason"] = " ", "R-01: DISPOSITION"),
            ((m, r, e) => r["rules"]![0]!["scope"] = "", "R-01: SCOPE_REQUIRED"),
            ((m, r, e) => r["rules"]![1]!["adaptationReason"] = null, "R-02: ADAPTATION_REASON"),
            ((m, r, e) => r["rules"]![0]!["positiveProof"] = null, "R-01: POSITIVE_PROOF_REQUIRED"),
            ((m, r, e) => r["rules"]![0]!["negativeProof"] = null, "R-01: NEGATIVE_PROOF_REQUIRED"),
            ((m, r, e) => r["rules"]![0]!["documentation"] = null, "R-01: DOCUMENTATION_REFERENCE"),
        };
        foreach (var mutation in mutations)
        {
            var input = Inputs(); mutation.Apply(input.Manifest, input.Register, input.Exemptions);
            RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain(x => x.Contains(mutation.Diagnostic, StringComparison.Ordinal));
        }
        ProofProtocol.Record("edge.rule-02.empty", "REQUIRED_FIELDS_REJECTED", "nine detached empty/missing inventory and required mapping mutations");
    }

    [Fact, Trait("Case", "edge.rule-02.encoding")]
    public void Schema_ExactEncoding_NeverNormalizesIdentity()
    {
        foreach (var value in new[] { "r-01", "R-001", "R-０１", "R-01\u0301", "R-0\u0131" })
        {
            var input = Inputs(); input.Register["rules"]![0]!["sourceId"] = value;
            RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain("REGISTER: RULE_INVENTORY");
        }
        Should.Throw<System.Text.DecoderFallbackException>(() => RegisterValidator.Parse(new byte[] { 0xC3, 0x28 }));
        Should.Throw<InvalidDataException>(() => RegisterValidator.Parse(System.Text.Encoding.UTF8.GetBytes("{\"rules\":[],\"rules\":[]}"))).Message.ShouldContain("JSON_DUPLICATE");
        ProofProtocol.Record("edge.rule-02.encoding", "IDENTITY_EXACT", "case, width, leading zero, combining and dotless characters; malformed UTF-8 and duplicate JSON keys");
    }

    private static void Reverse(JsonObject obj, string name)
    {
        var rows = obj[name]!.AsArray().Select(x => x!.DeepClone()).Reverse().ToArray();
        obj[name] = new JsonArray(rows);
    }

    [Fact, Trait("Case", "edge.rule-02.ordering")]
    public void Schema_Permutations_PreserveVerdictAndExactCoverage()
    {
        var input = Inputs();
        var before = RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions);
        before.ShouldBeEmpty();
        foreach (var name in new[] { "files", "headings", "matrix", "exemptions" }) Reverse(input.Manifest, name);
        Reverse(input.Register, "rules"); Reverse(input.Exemptions, "donorExemptions"); Reverse(input.Exemptions, "local");
        foreach (var heading in input.Manifest["headings"]!.AsArray()) Reverse(heading!.AsObject(), "sourceIds");
        foreach (var rule in input.Register["rules"]!.AsArray()) Reverse(rule!.AsObject(), "headingRefs");
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBe(before);
        input.Register["rules"]![0]!["sourceId"] = "R-unknown";
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain("REGISTER: RULE_INVENTORY");
        ProofProtocol.Record("edge.rule-02.ordering", "PERMUTATION_EQUAL", "row/file/heading/relation permutations preserve full coverage; unknown ID remains rejected");
    }

    [Fact, Trait("Case", "register.reference-case")]
    public void Schema_CaseChangedReference_IsRejected() => RejectCapture("register.reference-case", (m, r, e) => r["rules"]![36]!["mechanism"]!["path"] = "tools/code-rules/bodyLengthAnalyzer.cs", "R-37: MECHANISM_REFERENCE");

    [Fact, Trait("Case", "register.symbol")]
    public void Schema_WrongMechanismSymbol_IsRejected() => RejectCapture("register.symbol", (m, r, e) => r["rules"]![36]!["mechanism"]!["symbol"] = "bodyLengthAnalyzer", "R-37: MECHANISM_REFERENCE");

    [Fact, Trait("Case", "register.source-field")]
    public void Schema_AlteredCapturedMechanism_IsRejected() => RejectCapture("register.source-field", (m, r, e) => m["matrix"]![36]!["mechanism"] = "SCR0007", "R-37: SOURCE_FIELD: mechanism");

    [Fact, Trait("Case", "register.proof-path")]
    public void Schema_WrongProofPath_IsRejected() => RejectCapture("register.proof-path", (m, r, e) => r["rules"]![36]!["positiveProof"]!["path"] = "tests/phase1/Analyzers/bodyLengthAnalyzerTests.cs", "R-37: PROOF_PATH");

    [Fact, Trait("Case", "register.reference-unicode")]
    public void Schema_UnicodeReference_IsRejected()
    {
        foreach (var spelling in new[] { "BodyLéngthAnalyzer.cs", "BodyLe\u0301ngthAnalyzer.cs" })
        {
            var input = Inputs(); input.Register["rules"]![36]!["mechanism"]!["path"] = "tools/code-rules/" + spelling;
            RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain("R-37: MECHANISM_REFERENCE");
        }
        ProofProtocol.Record("register.reference-unicode", "R-37: MECHANISM_REFERENCE", "both composed and decomposed spellings remain distinct from exact ASCII path");
    }

    [Fact, Trait("Case", "register.proof-reverse")]
    public void Schema_ProofForDifferentRule_IsRejected() => RejectCapture("register.proof-reverse", (m, r, e) => r["rules"]![0]!["positiveProof"] = r["rules"]![1]!["positiveProof"]!.DeepClone(), "R-01: PROOF_REVERSE_ID");

    [Fact, Trait("Case", "register.severity")]
    public void Schema_SeverityWeakening_IsRejected() => RejectCapture("register.severity", (m, r, e) => r["rules"]![36]!["mechanism"]!["severity"] = "warning", "R-37: SEVERITY_WEAKENED");

    [Fact, Trait("Case", "register.diagnostic")]
    public void Schema_DiagnosticSubstitution_IsRejected() => RejectCapture("register.diagnostic", (m, r, e) => r["rules"]![36]!["mechanism"]!["diagnostics"] = new JsonArray("SCR0007"), "R-37: DIAGNOSTIC_IDENTITY");

    [Fact, Trait("Case", "register.documentation")]
    public void Schema_WrongDocumentationAnchor_IsRejected() => RejectCapture("register.documentation", (m, r, e) => r["rules"]![0]!["documentation"]!["symbol"] = "r-01", "R-01: DOCUMENTATION_REFERENCE");

    [Fact, Trait("Case", "register.numeric")]
    public void Schema_NumericLimitDrift_IsRejected() => RejectCapture("register.numeric", (m, r, e) => r["rules"]![36]!["numeric"]!["limit"] = 61, "R-37: NUMERIC_LIMIT");

    [Fact, Trait("Case", "register.numeric-semantics")]
    public void Schema_NumericCountingChange_IsRejected() => RejectCapture("register.numeric-semantics", (m, r, e) => r["rules"]![9]!["numeric"]!["unit"] = "Unicode scalar values", "R-10: NUMERIC_SEMANTICS");

    [Fact, Trait("Case", "register.boundary-proof")]
    public void Schema_ReusedBoundaryEvidence_IsRejected() => RejectCapture("register.boundary-proof", (m, r, e) => r["rules"]![36]!["numeric"]!["boundaryProofs"]![1]!["id"] = "rules.body.59", "rules.body.59: BOUNDARY_PROOF: DUPLICATE_OR_EMPTY");

    [Fact, Trait("Case", "register.review")]
    public void Schema_AutomatedHumanJudgment_IsRejected() => RejectCapture("register.review", (m, r, e) => r["rules"]![23]!["qualitativeReview"]!["status"] = "pass", "R-24: HUMAN_REVIEW_PENDING");

    [Fact, Trait("Case", "register.exemption-duplicate")]
    public void Schema_DuplicateExemption_IsRejected() => RejectCapture("register.exemption-duplicate", (m, r, e) => e["local"]!.AsArray().Add(e["local"]![0]!.DeepClone()), "KZ-EX-LOCK-01: LOCAL_EXEMPTION_ID: DUPLICATE_OR_EMPTY");

    [Fact, Trait("Case", "register.exemption-overlap")]
    public void Schema_OverlappingExemptions_AreRejected() => RejectCapture("register.exemption-overlap", (m, r, e) =>
    {
        var clone = e["local"]![0]!.DeepClone(); clone["id"] = "KZ-EX-LOCK-02"; e["local"]!.AsArray().Add(clone);
    }, "KZ-EX-LOCK-02: EXEMPTION_OVERLAP");

    [Fact, Trait("Case", "register.exemption-broad")]
    public void Schema_BroadenedExemptionScope_IsRejected() => RejectCapture("register.exemption-broad", (m, r, e) => e["local"]![0]!["path"] = "tests/**/packages.lock.json", "KZ-EX-LOCK-01: EXEMPTION_SCOPE");

    [Fact, Trait("Case", "register.pending")]
    public void Schema_FutureMappings_CannotCountAsCompletedEvidence()
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        var strict = RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions, requireImplemented: true);
        strict.ShouldContain("R-37: IMPLEMENTATION_PENDING");
        strict.ShouldContain("R-37: PROOF_UNIMPLEMENTED");
        strict.ShouldContain("R-24: HUMAN_REVIEW_PENDING");
        ProofProtocol.Record("register.pending", "IMPLEMENTATION_PENDING", "structural capture passes; unbound enforcement and qualitative judgment cannot satisfy full AC-06");
    }

    private static void Resolution(string id)
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        var row = input.Register["rules"]!.AsArray().Single(x => x!["sourceId"]!.GetValue<string>() == id)!;
        row["disposition"]!.GetValue<string>().ShouldBe("inapplicable");
        row["reason"]!.GetValue<string>().ShouldNotBeNullOrWhiteSpace();
        row["mechanism"].ShouldBeNull();
        ProofProtocol.Record(id + ".register-resolution", "INAPPLICABLE_ACCOUNTED", id + " HTTP/database-only mechanism explicitly accounted without enforcement credit");
    }

    [Fact, Trait("Case", "R-43.register-resolution")]
    public void Resolution_R43_IsExplicit() => Resolution("R-43");
    [Fact, Trait("Case", "R-57.register-resolution")]
    public void Resolution_R57_IsExplicit() => Resolution("R-57");
    [Fact, Trait("Case", "R-76.register-resolution")]
    public void Resolution_R76_IsExplicit() => Resolution("R-76");
    [Fact, Trait("Case", "R-79.register-resolution")]
    public void Resolution_R79_IsExplicit() => Resolution("R-79");
    [Fact, Trait("Case", "R-80.register-resolution")]
    public void Resolution_R80_IsExplicit() => Resolution("R-80");
    [Fact, Trait("Case", "R-85.register-resolution")]
    public void Resolution_R85_IsExplicit() => Resolution("R-85");
}
